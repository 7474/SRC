using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using SkiaSharp;
using SRCRender;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SRCSharpAvalonia
{
    public class MainWindow : Window, IScreenHost, IScreenDialogHost
    {
        public const string AppTitle = "SRC#Avalonia";

        public MainWindow()
        {
            Title = AppTitle;
            CanResize = false;
            SizeToContent = SizeToContent.WidthAndHeight;
            View = new ScreenView();
            Content = View;
        }

        public ScreenView View { get; }

        /// <summary>
        /// 終了要求時の処理。既定ではアプリケーションを終了する。
        /// </summary>
        public Func<bool> TerminateHandler { get; set; }

        public void Present(SKBitmap frame)
        {
            View.SetFrame(frame);
            Dispatcher.UIThread.Post(View.Flush);
        }

        public void SetTitle(string title)
        {
            var text = string.IsNullOrEmpty(title) ? AppTitle : title + " - " + AppTitle;
            Dispatcher.UIThread.Post(() => Title = text);
        }

        public bool Terminate()
        {
            return TerminateHandler?.Invoke() ?? true;
        }

        public Task<string> InputTextAsync(string title, string message, string defaultValue)
        {
            return Dispatcher.UIThread.InvokeAsync(() => new InputWindow(title, message, defaultValue).ShowDialog<string>(this));
        }

        public Task<string> OpenFileAsync(FileDialogRequest request)
        {
            return Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = request.Title,
                    AllowMultiple = false,
                    SuggestedStartLocation = await StartLocation(request),
                    FileTypeFilter = FileTypes(request),
                });
                return files.FirstOrDefault()?.TryGetLocalPath();
            });
        }

        public Task<string> SaveFileAsync(FileDialogRequest request)
        {
            return Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = request.Title,
                    SuggestedFileName = request.InitialFile,
                    DefaultExtension = request.FileExtension,
                    SuggestedStartLocation = await StartLocation(request),
                    FileTypeChoices = FileTypes(request),
                });
                return file?.TryGetLocalPath();
            });
        }

        private async Task<IStorageFolder> StartLocation(FileDialogRequest request)
        {
            return string.IsNullOrEmpty(request.InitialDirectory)
                ? null
                : await StorageProvider.TryGetFolderFromPathAsync(request.InitialDirectory);
        }

        private static FilePickerFileType[] FileTypes(FileDialogRequest request)
        {
            if (string.IsNullOrEmpty(request.FileExtension) || string.IsNullOrEmpty(request.FileType))
            {
                return null;
            }
            return new[]
            {
                new FilePickerFileType($"{request.FileType} (*.{request.FileExtension})") { Patterns = new[] { "*." + request.FileExtension } },
                new FilePickerFileType("すべてのファイル (*.*)") { Patterns = new[] { "*" } },
            };
        }
    }
}
