using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Themes.Fluent;
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

        private async Task StartAsync(MainWindow window, string[] args)
        {
            var file = args.FirstOrDefault(File.Exists);
            if (file == null)
            {
                var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "シナリオを選択",
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
