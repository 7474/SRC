using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Themes.Fluent;
using Microsoft.Extensions.Logging;
using SRCCore.Config;
using SRCCore.Logging;
using SRCRender;
using SRCSound;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SRCSharpAvalonia
{
    public class App : Application
    {
        private ScreenSession session;

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
                desktop.Exit += (s, e) =>
                {
                    session?.Dispose();
                    session?.SRC.Sound.Player.Dispose();
                };
                window.Opened += async (s, e) => await StartAsync(window, desktop, desktop.Args ?? new string[0]);
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

        private async Task StartAsync(MainWindow window, IClassicDesktopStyleApplicationLifetime desktop, string[] args)
        {
            var file = args.FirstOrDefault(File.Exists);
            if (file == null && args.Contains(SmokeOption))
            {
                // 起動の確かめでは、ファイルを選ぶ画面で止まらないようにする
                System.Console.WriteLine("SRC#Avalonia smoke: シナリオのファイルがありません");
                // Opened の中で Shutdown するとライフタイムの開始の処理が落ちるため、UI スレッドに積んで後で終える
                Avalonia.Threading.Dispatcher.UIThread.Post(() => desktop.Shutdown(3));
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

            // 音声は OpenAL で再生する。MIDI の SoundFont は設定（Option/SoundFont）・環境変数・SRC のフォルダなどから探す
            var loggerFactory = Program.LogHost?.LoggerFactory;
            var player = new OpenALSoundPlayer(() => SoundFontLocator.Load(
                session?.SRC.AppPath,
                session?.SRC.SystemConfig.GetItem("Option", "SoundFont")),
                loggerFactory?.CreateLogger(LogCategories.Sound));
            session = new ScreenSession(window, soundPlayer: player, loggerFactory: loggerFactory);
            window.View.GUI = session.GUI;
            window.View.Focus();
            session.Start(file);

            if (args.Contains(SmokeOption))
            {
                _ = Task.Run(() => SmokeAsync(desktop));
            }
        }

        /// <summary>起動の確かめ（--smoke）。配る形（自己完結の単一ファイル）で起動できるかを CI で見る（#1359）</summary>
        private const string SmokeOption = "--smoke";

        /// <summary>
        /// シナリオが最初のメッセージかマップを出すまで待ってから終わる。出たら終了コード 0、60 秒で出なければ 2
        /// </summary>
        private async Task SmokeAsync(IClassicDesktopStyleApplicationLifetime desktop)
        {
            var started = System.Diagnostics.Stopwatch.StartNew();
            var ok = false;
            while (started.Elapsed < System.TimeSpan.FromSeconds(60))
            {
                var state = session?.GUI.State;
                if (state != null && state.Dialog == null && (state.Message.Visible || state.ShowMap))
                {
                    ok = true;
                    break;
                }
                await Task.Delay(200);
            }
            System.Console.WriteLine(ok ? "SRC#Avalonia smoke: OK" : "SRC#Avalonia smoke: シナリオが始まらない");
            Avalonia.Threading.Dispatcher.UIThread.Post(() => desktop.Shutdown(ok ? 0 : 2));
        }
    }
}
