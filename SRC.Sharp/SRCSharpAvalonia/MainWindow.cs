using Avalonia;
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
    public class MainWindow : Window, IScreenHost, IScreenDialogHost, IScreenCursorHost
    {
        public const string AppTitle = "SRC#Avalonia";

        public MainWindow()
        {
            Title = AppTitle;
            CanResize = false;
            SizeToContent = SizeToContent.WidthAndHeight;
            View = new ScreenView();
            // 表示倍率を選ぶメニュー（表示 > 100%〜400%）
            scaleItems = ScreenView.DisplayScales.Select(scale =>
            {
                var item = new MenuItem
                {
                    Header = $"{scale * 100}%",
                    ToggleType = MenuItemToggleType.Radio,
                    GroupName = "DisplayScale",
                    Tag = scale,
                };
                item.Click += (s, e) =>
                {
                    SetDisplayScale(scale);
                    DisplayScaleChosen?.Invoke(scale);
                    View.Focus();
                };
                return item;
            }).ToArray();
            var menu = new Menu
            {
                ItemsSource = new[] { new MenuItem { Header = Strings.ViewMenu, ItemsSource = scaleItems } },
            };
            DockPanel.SetDock(menu, Dock.Top);
            Content = new DockPanel { Children = { menu, View } };
            SetDisplayScale(View.DisplayScale);
        }

        private readonly MenuItem[] scaleItems;

        /// <summary>表示倍率のメニュー項目（テスト用）</summary>
        internal System.Collections.Generic.IReadOnlyList<MenuItem> ScaleItems => scaleItems;

        public ScreenView View { get; }

        /// <summary>メニューで表示倍率を選んだときに呼ばれる（設定に保存する）</summary>
        public event Action<int> DisplayScaleChosen;

        /// <summary>
        /// 表示倍率を変える。ウィンドウの大きさは画面に合わせて変わる（SizeToContent）
        /// </summary>
        public void SetDisplayScale(int scale)
        {
            View.DisplayScale = scale;
            foreach (var item in scaleItems)
            {
                item.IsChecked = (int)item.Tag == scale;
            }
        }

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

        public bool TryGetCursorPosition(out float x, out float y)
        {
            var result = Dispatcher.UIThread.Invoke(() =>
            {
                if (!NativeCursor.TryGetPosition(out var sx, out var sy))
                {
                    return ((float, float)?)null;
                }
                var p = View.PointToClient(ToPixelPoint(sx, sy));
                return ((float)(p.X / View.DisplayScale), (float)(p.Y / View.DisplayScale));
            });
            x = result?.Item1 ?? 0;
            y = result?.Item2 ?? 0;
            return result.HasValue;
        }

        public bool SetCursorPosition(float x, float y)
        {
            return Dispatcher.UIThread.Invoke(() =>
            {
                var p = View.PointToScreen(new Avalonia.Point(x * View.DisplayScale, y * View.DisplayScale));
                if (OperatingSystem.IsMacOS())
                {
                    // macOS の CoreGraphics はポイント単位
                    return NativeCursor.SetPosition((int)Math.Round(p.X / RenderScaling), (int)Math.Round(p.Y / RenderScaling));
                }
                return NativeCursor.SetPosition(p.X, p.Y);
            });
        }

        private Avalonia.PixelPoint ToPixelPoint(int x, int y)
        {
            return OperatingSystem.IsMacOS()
                ? new Avalonia.PixelPoint((int)Math.Round(x * RenderScaling), (int)Math.Round(y * RenderScaling))
                : new Avalonia.PixelPoint(x, y);
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
                new FilePickerFileType(Strings.AllFiles) { Patterns = new[] { "*" } },
            };
        }
    }
}
