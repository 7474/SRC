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

        /// <summary>SRC# のヘルプ（SRC.Sharp.Help）</summary>
        public static readonly Uri HelpUri = new Uri("https://srch.7474.jp/");

        /// <summary>SRC# のサイト（リポジトリ）</summary>
        public static readonly Uri WebsiteUri = new Uri("https://github.com/7474/SRC");

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
            // 素材フォルダなどの設定（設定 > 素材フォルダの設定...）
            var assetSettingsItem = new MenuItem { Header = Strings.AssetSettingsMenu };
            assetSettingsItem.Click += (s, e) => AssetSettingsRequested?.Invoke();
            AssetSettingsItem = assetSettingsItem;
            // ファイル（シナリオを開く・最初からやり直す・終了）。シナリオの切り替えは App が行う
            OpenScenarioItem = new MenuItem { Header = Strings.OpenScenarioMenu };
            OpenScenarioItem.Click += (s, e) => OpenScenarioRequested?.Invoke();
            RestartItem = new MenuItem { Header = Strings.RestartMenu };
            RestartItem.Click += (s, e) => RestartRequested?.Invoke();
            ExitItem = new MenuItem { Header = Strings.ExitMenu };
            ExitItem.Click += (s, e) => ExitRequested?.Invoke();
            // ヘルプ（ヘルプ・サイトはブラウザで開く）
            var helpItem = new MenuItem { Header = Strings.HelpContentsMenu };
            helpItem.Click += async (s, e) => await Launcher.LaunchUriAsync(HelpUri);
            var websiteItem = new MenuItem { Header = Strings.WebsiteMenu };
            websiteItem.Click += async (s, e) => await Launcher.LaunchUriAsync(WebsiteUri);
            AboutItem = new MenuItem { Header = Strings.AboutMenu };
            AboutItem.Click += async (s, e) =>
            {
                await new AboutWindow().ShowDialog(this);
                View.Focus();
            };
            var menu = new Menu
            {
                ItemsSource = new[]
                {
                    new MenuItem { Header = Strings.FileMenu, ItemsSource = new object[] { OpenScenarioItem, RestartItem, new Separator(), ExitItem } },
                    new MenuItem { Header = Strings.ViewMenu, ItemsSource = scaleItems },
                    new MenuItem { Header = Strings.SettingsMenu, ItemsSource = new[] { assetSettingsItem } },
                    new MenuItem { Header = Strings.HelpMenu, ItemsSource = new object[] { helpItem, websiteItem, new Separator(), AboutItem } },
                },
            };
            DockPanel.SetDock(menu, Dock.Top);
            Content = new DockPanel { Children = { menu, View } };
            SetDisplayScale(View.DisplayScale);
            SetScenarioRunning(false);
        }

        private readonly MenuItem[] scaleItems;

        /// <summary>ファイル・ヘルプのメニュー項目（テスト用）</summary>
        internal MenuItem OpenScenarioItem { get; }
        internal MenuItem RestartItem { get; }
        internal MenuItem ExitItem { get; }
        internal MenuItem AboutItem { get; }

        /// <summary>メニューで「シナリオを開く...」を選んだときに呼ばれる</summary>
        public event Action OpenScenarioRequested;

        /// <summary>メニューで「最初からやり直す」を選んだときに呼ばれる</summary>
        public event Action RestartRequested;

        /// <summary>メニューで「終了」を選んだときに呼ばれる</summary>
        public event Action ExitRequested;

        /// <summary>シナリオを実行しているか。実行していなければ「最初からやり直す」を選べない</summary>
        public void SetScenarioRunning(bool running)
        {
            RestartItem.IsEnabled = running;
        }

        /// <summary>「素材フォルダの設定...」のメニュー項目（テスト用）</summary>
        internal MenuItem AssetSettingsItem { get; }

        /// <summary>表示倍率のメニュー項目（テスト用）</summary>
        internal System.Collections.Generic.IReadOnlyList<MenuItem> ScaleItems => scaleItems;

        public ScreenView View { get; }

        /// <summary>メニューで「素材フォルダの設定...」を選んだときに呼ばれる</summary>
        public event Action AssetSettingsRequested;

        /// <summary>メニューで表示倍率を選んだときに呼ばれる（設定に保存する）</summary>
        public event Action<double> DisplayScaleChosen;

        /// <summary>
        /// 表示倍率を変える。ウィンドウの大きさは画面に合わせて変わる（SizeToContent）
        /// </summary>
        public void SetDisplayScale(double scale)
        {
            View.DisplayScale = scale;
            foreach (var item in scaleItems)
            {
                item.IsChecked = (double)item.Tag == scale;
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
