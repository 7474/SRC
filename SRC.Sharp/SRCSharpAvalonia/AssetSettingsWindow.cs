using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using SRCSound;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCSharpAvalonia
{
    /// <summary>
    /// 素材の場所の設定（srcs.json の ExtDataPath・ExtDataPath2・Option/SoundFont）。
    /// </summary>
    public sealed class AssetSettings
    {
        public string ExtDataPath { get; set; } = "";
        public string ExtDataPath2 { get; set; } = "";
        public string SoundFont { get; set; } = "";
    }

    /// <summary>
    /// 素材フォルダの設定を補助するウィンドウ。フォルダ・ファイルを選ばせ、中身（Bitmap・Data などのフォルダ、SoundFont）が
    /// 見つかるかをその場で示す。閉じるときの結果は設定、キャンセル時は null。
    /// </summary>
    public class AssetSettingsWindow : Window
    {
        /// <summary>素材のフォルダにあるはずのフォルダ（元の SRC の本体・開発パックと同じ構成）</summary>
        internal static readonly string[] AssetFolders = { "Bitmap", "Data", "Midi", "Sound" };

        private readonly string appPath;

        public AssetSettingsWindow(AssetSettings settings, string appPath)
        {
            this.appPath = appPath;
            Title = Strings.AssetSettingsTitle;
            CanResize = false;
            SizeToContent = SizeToContent.WidthAndHeight;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowInTaskbar = false;

            ExtDataPath = new PathField(Strings.ExtDataPathLabel, settings.ExtDataPath, DescribeFolder, BrowseFolderAsync);
            ExtDataPath2 = new PathField(Strings.ExtDataPath2Label, settings.ExtDataPath2, DescribeFolder, BrowseFolderAsync);
            SoundFont = new PathField(Strings.SoundFontLabel, settings.SoundFont, DescribeSoundFont, BrowseSoundFontAsync);

            var save = new Button { Content = Strings.Save, IsDefault = true, MinWidth = 90 };
            var cancel = new Button { Content = Strings.Cancel, IsCancel = true, MinWidth = 90 };
            save.Click += (s, e) => Close(Result());
            cancel.Click += (s, e) => Close(null);

            Content = new StackPanel
            {
                Margin = new Thickness(12),
                Spacing = 12,
                Width = 560,
                Children =
                {
                    new TextBlock { Text = string.Format(Strings.AssetSettingsDescription, appPath), TextWrapping = TextWrapping.Wrap },
                    ExtDataPath.Panel,
                    ExtDataPath2.Panel,
                    SoundFont.Panel,
                    new TextBlock { Text = Strings.AssetSettingsApplyNote, TextWrapping = TextWrapping.Wrap, Opacity = 0.75 },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing = 8,
                        Children = { save, cancel },
                    },
                },
            };
        }

        internal PathField ExtDataPath { get; }
        internal PathField ExtDataPath2 { get; }
        internal PathField SoundFont { get; }

        /// <summary>入力している設定</summary>
        internal AssetSettings Result()
        {
            return new AssetSettings
            {
                ExtDataPath = ExtDataPath.Value,
                ExtDataPath2 = ExtDataPath2.Value,
                SoundFont = SoundFont.Value,
            };
        }

        /// <summary>
        /// 素材のフォルダの状態。空欄なら空文字列。warning が真なら、指定が誤っていそうなことを示す
        /// </summary>
        internal static (string Text, bool Warning) DescribeFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return ("", false);
            }
            if (!Directory.Exists(path))
            {
                return (Strings.FolderMissing, true);
            }
            // Bitmap などのフォルダそのものを選んだ（その親を指定する）
            var name = Path.GetFileName(Path.TrimEndingDirectorySeparator(path));
            var self = AssetFolders.FirstOrDefault(x => string.Equals(x, name, StringComparison.OrdinalIgnoreCase));
            if (self != null)
            {
                return (string.Format(Strings.FolderIsAssetFolder, self), true);
            }
            var found = FoundAssetFolders(path);
            return found.Count == 0
                ? (Strings.FolderHasNoAssets, true)
                : (string.Format(Strings.FolderFound, string.Join(", ", found)), false);
        }

        /// <summary>フォルダにある素材のフォルダ（大文字・小文字は区別しない）</summary>
        internal static IReadOnlyList<string> FoundAssetFolders(string path)
        {
            var dirs = Directory.EnumerateDirectories(path).Select(Path.GetFileName).ToList();
            return AssetFolders.Where(x => dirs.Any(d => string.Equals(d, x, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        private (string Text, bool Warning) DescribeSoundFont(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                var found = SoundFontLocator.Find(appPath);
                return found == null ? (Strings.SoundFontNotFound, true) : (string.Format(Strings.SoundFontAuto, found), false);
            }
            return File.Exists(path) ? ("", false) : (Strings.FileMissing, true);
        }

        private async System.Threading.Tasks.Task<string> BrowseFolderAsync(string current)
        {
            var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = Strings.AssetSettingsTitle,
                AllowMultiple = false,
                SuggestedStartLocation = await StartLocation(current),
            });
            return folders.FirstOrDefault()?.TryGetLocalPath();
        }

        private async System.Threading.Tasks.Task<string> BrowseSoundFontAsync(string current)
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = Strings.SoundFontLabel,
                AllowMultiple = false,
                SuggestedStartLocation = await StartLocation(string.IsNullOrEmpty(current) ? null : Path.GetDirectoryName(current)),
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("SoundFont (*.sf2)") { Patterns = new[] { "*.sf2" } },
                    new FilePickerFileType(Strings.AllFiles) { Patterns = new[] { "*" } },
                },
            });
            return files.FirstOrDefault()?.TryGetLocalPath();
        }

        private async System.Threading.Tasks.Task<IStorageFolder> StartLocation(string path)
        {
            return string.IsNullOrEmpty(path) || !Directory.Exists(path) ? null : await StorageProvider.TryGetFolderFromPathAsync(path);
        }

        /// <summary>
        /// 見出し・入力欄・参照・クリアと、状態の表示からなる 1 項目。
        /// </summary>
        internal sealed class PathField
        {
            private readonly Func<string, (string Text, bool Warning)> describe;

            public PathField(string label, string value, Func<string, (string Text, bool Warning)> describe, Func<string, System.Threading.Tasks.Task<string>> browse)
            {
                this.describe = describe;
                TextBox = new TextBox { Text = value ?? "" };
                Status = new TextBlock { TextWrapping = TextWrapping.Wrap, FontSize = 12 };
                var browseButton = new Button { Content = Strings.Browse };
                var clearButton = new Button { Content = Strings.Clear };
                browseButton.Click += async (s, e) =>
                {
                    var selected = await browse(Value);
                    if (!string.IsNullOrEmpty(selected))
                    {
                        TextBox.Text = selected;
                    }
                };
                clearButton.Click += (s, e) => TextBox.Text = "";
                TextBox.PropertyChanged += (s, e) =>
                {
                    if (e.Property == TextBox.TextProperty)
                    {
                        Update();
                    }
                };

                var row = new DockPanel();
                var buttons = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4, Margin = new Thickness(4, 0, 0, 0), Children = { browseButton, clearButton } };
                DockPanel.SetDock(buttons, Dock.Right);
                row.Children.Add(buttons);
                row.Children.Add(TextBox);
                Panel = new StackPanel
                {
                    Spacing = 4,
                    Children = { new TextBlock { Text = label, FontWeight = FontWeight.Bold }, row, Status },
                };
                Update();
            }

            public TextBox TextBox { get; }
            public TextBlock Status { get; }
            public StackPanel Panel { get; }
            public string Value => (TextBox.Text ?? "").Trim();

            /// <summary>状態の表示を、入力に合わせて更新する</summary>
            public void Update()
            {
                var (text, warning) = describe(Value);
                Status.Text = text;
                Status.IsVisible = !string.IsNullOrEmpty(text);
                Status.Foreground = warning ? Brushes.DarkRed : Brushes.DarkGreen;
            }
        }
    }
}
