using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Themes.Fluent;
using Microsoft.Extensions.Logging;
using SRCCore;
using SRCCore.Config;
using SRCCore.Logging;
using SRCRender;
using SRCSound;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SRCSharpAvalonia
{
    public class App : Application
    {
        private ScreenSession session;

        /// <summary>実行中のシナリオ（始める前は null）</summary>
        internal ScreenSession Session => session;

        /// <summary>音声の再生を作る。null なら OpenAL で再生する（<see cref="CreateDefaultSoundPlayer"/>）。テストで差し替える</summary>
        internal Func<IPlaySound> CreateSoundPlayer { get; set; }

        /// <summary>起動の確かめ（--smoke）で、シナリオが始まるのを待つ時間</summary>
        internal TimeSpan SmokeTimeout { get; set; } = TimeSpan.FromSeconds(60);

        public override void Initialize()
        {
            Styles.Add(new FluentTheme());
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // 表示言語（srcs.json の Option/Language）は、ウィンドウを作る前に設定する
                var config = LoadConfig();
                var window = new MainWindow();
                desktop.MainWindow = window;
                desktop.Exit += (s, e) => Stop();
                var savedScale = ReadDisplayScale(config);
                if (savedScale.HasValue)
                {
                    window.SetDisplayScale(savedScale.Value);
                }
                window.DisplayScaleChosen += SaveDisplayScale;
                window.SettingsRequested += async () => await EditSettingsAsync(window);
                window.OpenScenarioRequested += async () => await OpenScenarioAsync(window);
                window.RestartRequested += async () => await RestartAsync(window);
                window.ExitRequested += async () => await ExitAsync(window);
                window.Opened += async (s, e) =>
                {
                    if (!savedScale.HasValue)
                    {
                        // 設定が無ければ、画面（作業領域）に収まるいちばん大きな倍率にする
                        FitToScreen(window);
                    }
                    await StartAsync(window, desktop.Args ?? new string[0], desktop.Shutdown);
                };
            }
            base.OnFrameworkInitializationCompleted();
        }

        private static LocalFileConfig LoadConfig()
        {
            var config = new LocalFileConfig();
            try
            {
                config.Load();
            }
            catch
            {
                // 設定ファイルが無い場合は既定（日本語）
            }
            UiLanguage.Apply(config);
            // ログの設定（Log セクション）
            Program.LogHost?.ApplyConfig(config);
            return config;
        }

        /// <summary>表示倍率の設定（srcs.json の Option/DisplayScale。100 = 100%）</summary>
        internal const string DisplayScaleSection = "Option";
        internal const string DisplayScaleName = "DisplayScale";

        /// <summary>設定の表示倍率。無いか選べない値なら null</summary>
        internal static double? ReadDisplayScale(ISystemConfig config)
        {
            if (int.TryParse(config.GetItem(DisplayScaleSection, DisplayScaleName), out var percent)
                && Array.IndexOf(ScreenView.DisplayScales, percent / 100.0) >= 0)
            {
                return percent / 100.0;
            }
            return null;
        }

        /// <summary>
        /// ウィンドウ（タイトルバー・メニューを含む）が画面の作業領域に収まる、いちばん大きな倍率にして、画面の中央に置く
        /// </summary>
        internal static void FitToScreen(MainWindow window)
        {
            var screen = window.Screens.ScreenFromWindow(window) ?? window.Screens.Primary;
            if (screen == null)
            {
                return;
            }
            var area = screen.WorkingArea;
            var scaling = screen.Scaling;
            // 画面の表示以外の大きさ（タイトルバー・枠・メニュー）
            var frame = window.FrameSize ?? window.ClientSize;
            var extraWidth = frame.Width - window.View.Width;
            var extraHeight = frame.Height - window.View.Height;
            var scale = ScreenView.FitScale(area.Width / scaling - extraWidth, area.Height / scaling - extraHeight);
            window.SetDisplayScale(scale);
            var width = (int)Math.Round((ScreenLayout.Width * scale + extraWidth) * scaling);
            var height = (int)Math.Round((ScreenLayout.Height * scale + extraHeight) * scaling);
            window.Position = new PixelPoint(
                area.X + Math.Max(0, (area.Width - width) / 2),
                area.Y + Math.Max(0, (area.Height - height) / 2));
        }

        /// <summary>前回ファイル選択で開いたシナリオのフォルダ（srcs.json の Option/LastScenarioFolder）</summary>
        internal const string LastScenarioFolderSection = "Option";
        internal const string LastScenarioFolderName = "LastScenarioFolder";

        /// <summary>前回開いたシナリオのフォルダ。無いか、もう存在しなければ null</summary>
        internal static string ReadLastScenarioFolder(ISystemConfig config)
        {
            var folder = config.GetItem(LastScenarioFolderSection, LastScenarioFolderName);
            return !string.IsNullOrEmpty(folder) && Directory.Exists(folder) ? folder : null;
        }

        /// <summary>
        /// 素材フォルダの設定のウィンドウを開き、保存を選んだら設定に書く。
        /// 素材の場所はシナリオを始めるときに読むため、次に起動したときから効く
        /// </summary>
        internal async Task EditSettingsAsync(MainWindow window)
        {
            var current = await ReadConfigAsync(ReadSettings);
            var appPath = session?.SRC.AppPath ?? new LocalFileConfig().AppPath;
            var result = await new SettingsWindow(current, appPath, Program.LogHost?.LevelFixed ?? false).ShowDialog<AppSettings>(window);
            if (result != null)
            {
                UpdateConfig(config =>
                {
                    WriteSettings(config, result);
                    // ログの設定は保存したときから効かせる（引数・環境変数で指定したものは上書きしない）
                    Program.LogHost?.ApplyConfig(config);
                });
            }
            window.View.Focus();
        }

        internal static AppSettings ReadSettings(ISystemConfig config)
        {
            return new AppSettings
            {
                ExtDataPath = config.ExtDataPath ?? "",
                ExtDataPath2 = config.ExtDataPath2 ?? "",
                SoundFont = config.GetItem("Option", "SoundFont"),
                Language = SRCCore.UiLanguages.SelectorValue(config.GetItem(UiLanguage.ConfigSection, UiLanguage.ConfigKey)),
                LogLevel = SRCCore.Logging.LogSettings.ParseLevel(config.GetItem(SRCCore.Logging.LogSettings.ConfigSection, "Level"))?.ToString() ?? "",
                LogFile = !string.Equals(config.GetItem(SRCCore.Logging.LogSettings.ConfigSection, "File"), "Off", StringComparison.OrdinalIgnoreCase),
                LogPath = config.GetItem(SRCCore.Logging.LogSettings.ConfigSection, "Path"),
                LogRetainDays = int.TryParse(config.GetItem(SRCCore.Logging.LogSettings.ConfigSection, "RetainDays"), out var days) && days > 0
                    ? days
                    : SRCCore.Logging.LogOptions.DefaultRetainDays,
            };
        }

        internal static void WriteSettings(ISystemConfig config, AppSettings settings)
        {
            config.ExtDataPath = settings.ExtDataPath ?? "";
            config.ExtDataPath2 = settings.ExtDataPath2 ?? "";
            config.SetItem("Option", "SoundFont", settings.SoundFont ?? "");
            config.SetItem(UiLanguage.ConfigSection, UiLanguage.ConfigKey, settings.Language ?? "");
            config.SetItem(SRCCore.Logging.LogSettings.ConfigSection, "Level", settings.LogLevel ?? "");
            config.SetItem(SRCCore.Logging.LogSettings.ConfigSection, "File", settings.LogFile ? "On" : "Off");
            config.SetItem(SRCCore.Logging.LogSettings.ConfigSection, "Path", settings.LogPath ?? "");
            config.SetItem(SRCCore.Logging.LogSettings.ConfigSection, "RetainDays", settings.LogRetainDays.ToString());
        }

        /// <summary>設定を読む。シナリオの実行中は、その設定（ゲームスレッドが持つ）から読む</summary>
        private Task<T> ReadConfigAsync<T>(Func<ISystemConfig, T> read)
        {
            if (session == null)
            {
                return Task.FromResult(read(LoadConfigFile()));
            }
            var done = new TaskCompletionSource<T>();
            session.Game.Post(() =>
            {
                try
                {
                    done.SetResult(read(session.SRC.SystemConfig));
                }
                catch (Exception ex)
                {
                    done.SetException(ex);
                }
            });
            return done.Task;
        }

        /// <summary>
        /// 設定を書き換えて srcs.json に保存する。シナリオの実行中は、その設定（ゲームスレッドが持つ）に書く
        /// （別に読んだ設定を保存すると、ゲーム中に変えた設定を上書きしてしまうため）
        /// </summary>
        private void UpdateConfig(Action<ISystemConfig> update)
        {
            void Save(ISystemConfig config)
            {
                try
                {
                    update(config);
                    config.Save();
                }
                catch (Exception ex)
                {
                    Program.LogHost?.LoggerFactory?.CreateLogger("SRCSharpAvalonia").LogWarning(ex, "設定を保存できませんでした");
                }
            }
            if (session != null)
            {
                session.Game.Post(() => Save(session.SRC.SystemConfig));
            }
            else
            {
                Save(LoadConfigFile());
            }
        }

        private static LocalFileConfig LoadConfigFile()
        {
            var config = new LocalFileConfig();
            try
            {
                config.Load();
            }
            catch
            {
                // 設定ファイルが無ければ、シナリオを始めるときと同じ既定値にする（保存すると作る）
                ScreenSession.ApplyDefaultConfig(config);
            }
            return config;
        }

        /// <summary>
        /// メニューで選んだ表示倍率を設定に保存する。シナリオの実行中は、その設定（ゲームスレッドが持つ）に書く
        /// </summary>
        private void SaveDisplayScale(double scale)
        {
            var value = ((int)Math.Round(scale * 100)).ToString();
            UpdateConfig(config => config.SetItem(DisplayScaleSection, DisplayScaleName, value));
        }

        /// <summary>終わるときに、シナリオと音声の再生を片付ける</summary>
        internal void Stop()
        {
            session?.Dispose();
            session?.SRC.Sound.Player.Dispose();
        }

        /// <summary>
        /// 引数のファイル（無ければ選ばせたファイル）でシナリオを始める。
        /// shutdown は終了コードを受け取ってアプリを終える（デスクトップのライフタイムの Shutdown）
        /// </summary>
        internal async Task StartAsync(MainWindow window, string[] args, Action<int> shutdown)
        {
            var file = args.FirstOrDefault(File.Exists);
            if (file == null && args.Contains(SmokeOption))
            {
                // 起動の確かめでは、ファイルを選ぶ画面で止まらないようにする
                System.Console.WriteLine("SRC#Avalonia smoke: シナリオのファイルがありません");
                // Opened の中で Shutdown するとライフタイムの開始の処理が落ちるため、UI スレッドに積んで後で終える
                Avalonia.Threading.Dispatcher.UIThread.Post(() => shutdown(3));
                return;
            }
            if (file == null)
            {
                file = await PickScenarioAsync(window);
            }
            if (string.IsNullOrEmpty(file))
            {
                window.Close();
                return;
            }

            StartSession(window, file);

            if (args.Contains(SmokeOption))
            {
                _ = Task.Run(() => SmokeAsync(shutdown));
            }
        }

        /// <summary>今のシナリオ（最初からやり直すときに使う）</summary>
        internal string CurrentFile { get; private set; }

        /// <summary>今のシナリオからウィンドウへの窓口（止めるときに切り離す）</summary>
        private SessionHost sessionHost;

        /// <summary>
        /// シナリオを選ばせる。前回ファイル選択で開いたシナリオのフォルダから選ばせ、選んだらそのフォルダを覚える。選ばなければ null
        /// </summary>
        private async Task<string> PickScenarioAsync(MainWindow window)
        {
            var lastFolder = ReadLastScenarioFolder(LoadConfigFile());
            var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = Strings.SelectScenario,
                AllowMultiple = false,
                SuggestedStartLocation = lastFolder == null ? null : await window.StorageProvider.TryGetFolderFromPathAsync(lastFolder),
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("SRC# files") { Patterns = new[] { "*.eve", "*.srcs", "*.srcq" } },
                },
            });
            var file = files.FirstOrDefault()?.TryGetLocalPath();
            if (!string.IsNullOrEmpty(file))
            {
                // シナリオを始める前（設定を読む前）に保存する
                var folder = Path.GetDirectoryName(Path.GetFullPath(file));
                UpdateConfig(config => config.SetItem(LastScenarioFolderSection, LastScenarioFolderName, folder));
            }
            return file;
        }

        /// <summary>シナリオを始める</summary>
        internal void StartSession(MainWindow window, string file)
        {
            var loggerFactory = Program.LogHost?.LoggerFactory;
            var player = CreateSoundPlayer?.Invoke() ?? CreateDefaultSoundPlayer();
            sessionHost = new SessionHost(window);
            session = new ScreenSession(sessionHost, soundPlayer: player, loggerFactory: loggerFactory);
            CurrentFile = file;
            window.View.GUI = session.GUI;
            window.SetScenarioRunning(true);
            window.View.Focus();
            session.Start(file);
        }

        /// <summary>
        /// 今のシナリオを止める（ゲームスレッドを止め、音を止めて、ウィンドウから切り離す）。
        /// 止まりきる前に描いたものなどは、新しいシナリオに混ざらないよう捨てる（SessionHost）
        /// </summary>
        internal void StopSession(MainWindow window)
        {
            sessionHost?.Deactivate();
            window.View.GUI = null;
            Stop();
            session = null;
            sessionHost = null;
            window.SetScenarioRunning(false);
        }

        /// <summary>シナリオの実行中なら、進行状況が失われることを確かめる。続けてよければ true</summary>
        private async Task<bool> ConfirmDiscardAsync(MainWindow window)
        {
            if (session == null)
            {
                return true;
            }
            return await new ConfirmWindow(Strings.ConfirmTitle, Strings.ConfirmDiscardProgress).ShowDialog<bool>(window);
        }

        /// <summary>メニューの「シナリオを開く...」。選んだシナリオを始める（選ばなければ今のシナリオを続ける）</summary>
        internal async Task OpenScenarioAsync(MainWindow window)
        {
            if (!await ConfirmDiscardAsync(window))
            {
                window.View.Focus();
                return;
            }
            var file = await PickScenarioAsync(window);
            if (string.IsNullOrEmpty(file))
            {
                window.View.Focus();
                return;
            }
            StopSession(window);
            StartSession(window, file);
        }

        /// <summary>メニューの「最初からやり直す」。今のシナリオ（ファイル）を始め直す</summary>
        internal async Task RestartAsync(MainWindow window)
        {
            if (CurrentFile == null || !await ConfirmDiscardAsync(window))
            {
                window.View.Focus();
                return;
            }
            var file = CurrentFile;
            StopSession(window);
            StartSession(window, file);
        }

        /// <summary>メニューの「終了」</summary>
        internal async Task ExitAsync(MainWindow window)
        {
            if (!await ConfirmDiscardAsync(window))
            {
                window.View.Focus();
                return;
            }
            window.Close();
        }

        /// <summary>
        /// 音声は OpenAL で再生する。MIDI の SoundFont は設定（Option/SoundFont）・環境変数・SRC のフォルダなどから探す
        /// </summary>
        internal IPlaySound CreateDefaultSoundPlayer()
        {
            return new OpenALSoundPlayer(() => SoundFontLocator.Load(
                session?.SRC.AppPath,
                session?.SRC.SystemConfig.GetItem("Option", "SoundFont")),
                Program.LogHost?.LoggerFactory?.CreateLogger(LogCategories.Sound));
        }

        /// <summary>起動の確かめ（--smoke）。配る形（自己完結の単一ファイル）で起動できるかを CI で見る（#1359）</summary>
        private const string SmokeOption = "--smoke";

        /// <summary>
        /// シナリオが最初のメッセージかマップを出すまで待ってから終わる。出たら終了コード 0、60 秒（SmokeTimeout）で出なければ 2
        /// </summary>
        private async Task SmokeAsync(Action<int> shutdown)
        {
            var started = System.Diagnostics.Stopwatch.StartNew();
            var ok = false;
            while (started.Elapsed < SmokeTimeout)
            {
                if (HasStarted(session?.GUI.State))
                {
                    ok = true;
                    break;
                }
                await Task.Delay(200);
            }
            System.Console.WriteLine(ok ? "SRC#Avalonia smoke: OK" : "SRC#Avalonia smoke: シナリオが始まらない");
            Avalonia.Threading.Dispatcher.UIThread.Post(() => shutdown(ok ? 0 : 2));
        }

        /// <summary>シナリオが始まったか（ダイアログを出さずに、メッセージかマップを出している）</summary>
        internal static bool HasStarted(ScreenState state)
        {
            return state != null && state.Dialog == null && (state.Message.Visible || state.ShowMap);
        }
    }
}
