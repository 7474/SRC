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
                ApplyLanguage();
                var window = new MainWindow();
                desktop.MainWindow = window;
                desktop.Exit += (s, e) => Stop();
                window.Opened += async (s, e) => await StartAsync(window, desktop.Args ?? new string[0], desktop.Shutdown);
            }
            base.OnFrameworkInitializationCompleted();
        }

        private static void ApplyLanguage()
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
                var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = Strings.SelectScenario,
                    AllowMultiple = false,
                    FileTypeFilter = new[]
                    {
                        new FilePickerFileType("SRC# files") { Patterns = new[] { "*.eve", "*.srcs", "*.srcq" } },
                    },
                });
                file = files.FirstOrDefault()?.TryGetLocalPath();
            }
            if (string.IsNullOrEmpty(file))
            {
                window.Close();
                return;
            }

            var loggerFactory = Program.LogHost?.LoggerFactory;
            var player = CreateSoundPlayer?.Invoke() ?? CreateDefaultSoundPlayer();
            session = new ScreenSession(window, soundPlayer: player, loggerFactory: loggerFactory);
            window.View.GUI = session.GUI;
            window.View.Focus();
            session.Start(file);

            if (args.Contains(SmokeOption))
            {
                _ = Task.Run(() => SmokeAsync(shutdown));
            }
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
