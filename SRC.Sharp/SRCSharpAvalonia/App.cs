using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Themes.Fluent;
using SRCCore.Config;
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
                window.Opened += async (s, e) => await StartAsync(window, desktop.Args ?? new string[0]);
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
        }

        private async Task StartAsync(MainWindow window, string[] args)
        {
            var file = args.FirstOrDefault(File.Exists);
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
            var player = new OpenALSoundPlayer(() => SoundFontLocator.Load(
                session?.SRC.AppPath,
                session?.SRC.SystemConfig.GetItem("Option", "SoundFont")));
            session = new ScreenSession(window, soundPlayer: player);
            window.View.GUI = session.GUI;
            window.View.Focus();
            session.Start(file);
        }
    }
}
