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

using SRCCore.Config;

namespace SRCSharpAvalonia
{
    /// <summary>
    /// 設定のウィンドウで変える設定（srcs.json）。
    /// </summary>
    public sealed class AppSettings
    {
        /// <summary>拡張データフォルダ（ExtDataPath・ExtDataPath2）</summary>
        public string ExtDataPath { get; set; } = "";
        public string ExtDataPath2 { get; set; } = "";
        /// <summary>MIDI の SoundFont（Option/SoundFont）</summary>
        public string SoundFont { get; set; } = "";
        /// <summary>表示言語（Option/Language。対応言語のカルチャ名か "auto"）</summary>
        public string Language { get; set; } = SRCCore.UiLanguages.Default;
        /// <summary>ログのレベル（Log/Level。空なら既定）</summary>
        public string LogLevel { get; set; } = "";
        /// <summary>ログをファイルに出すか（Log/File）</summary>
        public bool LogFile { get; set; } = true;
        /// <summary>ログのフォルダ（Log/Path。空なら既定）</summary>
        public string LogPath { get; set; } = "";
        /// <summary>ログを残す日数（Log/RetainDays）</summary>
        public int LogRetainDays { get; set; } = SRCCore.Logging.LogOptions.DefaultRetainDays;

        // ゲーム（元の SRC の設定変更の画面と同じ項目と、自動反撃モード）
        public bool BattleAnimation { get; set; } = true;
        public bool ExtendedAnimation { get; set; } = true;
        public bool WeaponAnimation { get; set; } = true;
        public bool SpecialPowerAnimation { get; set; } = true;
        public bool MoveAnimation { get; set; } = true;
        public bool AutoMoveCursor { get; set; } = true;
        public bool ShowSquareLine { get; set; }
        /// <summary>味方フェイズ開始時にターン表示を行う（Option/Turn）</summary>
        public bool ShowTurn { get; set; }
        public bool KeepEnemyBGM { get; set; }
        /// <summary>自動反撃モード（Option/AutoDefense）</summary>
        public bool AutoDefense { get; set; }
        /// <summary>音量（0〜100）</summary>
        public int SoundVolume { get; set; } = 50;

        /// <summary>場面ごとの BGM のファイル名（BGM セクション。空なら標準の名前）</summary>
        public Dictionary<string, string> Bgm { get; set; } = new Dictionary<string, string>();

        // 開発者向け
        /// <summary>デバッグモード（Option/DebugMode）</summary>
        public bool DebugMode { get; set; }
        /// <summary>テキストを読むとき Shift_JIS も読む・ファイルに書くとき Shift_JIS で書く（SRCCompatibilityMode）</summary>
        public bool CompatibleRead { get; set; } = true;
        public bool CompatibleWrite { get; set; } = true;
        /// <summary>画像のバッファの設定（SRC# の描画では使わない。Info 関数で読める）</summary>
        public int ImageBufferSize { get; set; }
        public int MaxImageBufferByteSize { get; set; }
        public bool KeepStretchedImage { get; set; }
    }

    /// <summary>
    /// 設定のウィンドウ（素材・表示・ログ）。素材はフォルダ・ファイルを選ばせ、中身（Bitmap・Data などのフォルダ、SoundFont）が
    /// 見つかるかをその場で示す。閉じるときの結果は設定、キャンセル時は null。
    /// </summary>
    public class SettingsWindow : Window
    {
        /// <summary>素材のフォルダにあるはずのフォルダ（元の SRC の本体・開発パックと同じ構成。素材パックと共通）</summary>
        internal static IReadOnlyList<string> AssetFolders => SRCCore.Filesystem.AssetPacks.AssetFolders;

        private readonly string appPath;

        /// <summary>表示言語の選択肢（"auto" と対応言語）</summary>
        internal static IReadOnlyList<(string Code, string Name)> Languages =>
            new[] { (SRCCore.UiLanguages.Auto, Strings.LanguageAuto) }
                .Concat(SRCCore.UiLanguages.Supported.Select(x => (x.Code, x.NativeName)))
                .ToList();

        /// <summary>ログのレベルの選択肢（空は既定）</summary>
        internal static readonly IReadOnlyList<string> LogLevels = new[] { "", "Trace", "Debug", "Information", "Warning", "Error", "Critical", "None" };

        /// <summary>場面ごとの BGM の名前（元の SRC の Src.ini の [BGM]・Sound.BGMName）</summary>
        internal static readonly IReadOnlyList<string> BgmNames = new[] { "Opening", "Map1", "Map2", "Map3", "Map4", "Map5", "Map6", "Briefing", "Intermission", "Subtitle", "End", "default" };

        /// <summary>BGM の名前の説明</summary>
        internal static string BgmDescription(string name)
        {
            switch (name)
            {
                case "Opening": return Strings.BgmOpening;
                case "Map1": return Strings.BgmMap1;
                case "Map2": return Strings.BgmMap2;
                case "Map3": return Strings.BgmMap3;
                case "Map4": return Strings.BgmMap4;
                case "Map5": return Strings.BgmMap5;
                case "Map6": return Strings.BgmMap6;
                case "Briefing": return Strings.BgmBriefing;
                case "Intermission": return Strings.BgmIntermission;
                case "Subtitle": return Strings.BgmSubtitle;
                case "End": return Strings.BgmEnd;
                default: return Strings.BgmBattle;
            }
        }

        /// <summary>開発者向けのタブの位置（メニューの「開発者向けの設定...」で開く）</summary>
        public const int DeveloperTabIndex = 5;

        public SettingsWindow(AppSettings settings, string appPath, bool logLevelFixed = false, int initialTab = 0)
        {
            this.appPath = appPath;
            Title = Strings.SettingsTitle;
            CanResize = false;
            SizeToContent = SizeToContent.WidthAndHeight;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowInTaskbar = false;

            ExtDataPath = new PathField(Strings.ExtDataPathLabel, settings.ExtDataPath, DescribeFolder, BrowseFolderAsync);
            ExtDataPath2 = new PathField(Strings.ExtDataPath2Label, settings.ExtDataPath2, DescribeFolder, BrowseFolderAsync);
            SoundFont = new PathField(Strings.SoundFontLabel, settings.SoundFont, DescribeSoundFont, BrowseSoundFontAsync);

            // 素材パック（Packs フォルダの zip）。追加は zip を Packs フォルダに写す
            PackList = new StackPanel { Spacing = 2 };
            var addPack = new Button { Content = Strings.AddPack };
            var openPacks = new Button { Content = Strings.OpenPacksFolder };
            addPack.Click += async (s, e) => await AddPacksAsync();
            openPacks.Click += async (s, e) =>
            {
                Directory.CreateDirectory(PacksDirectory);
                await Launcher.LaunchDirectoryInfoAsync(new DirectoryInfo(PacksDirectory));
            };
            var packsPanel = new StackPanel
            {
                Spacing = 4,
                Children =
                {
                    new TextBlock { Text = Strings.PacksLabel, FontWeight = FontWeight.Bold },
                    new TextBlock { Text = string.Format(Strings.PacksDescription, PacksDirectory), TextWrapping = TextWrapping.Wrap, FontSize = 12 },
                    PackList,
                    new StackPanel { Orientation = Orientation.Horizontal, Spacing = 4, Children = { addPack, openPacks } },
                },
            };
            UpdatePackList();

            var save = new Button { Content = Strings.Save, IsDefault = true, MinWidth = 90 };
            var cancel = new Button { Content = Strings.Cancel, IsCancel = true, MinWidth = 90 };
            save.Click += (s, e) => Close(Result());
            cancel.Click += (s, e) => Close(null);

            // 表示: 表示言語（表示倍率はメニューの「表示」）
            Language = new ComboBox
            {
                ItemsSource = Languages.Select(x => x.Name).ToList(),
                SelectedIndex = Math.Max(0, Languages.ToList().FindIndex(x => x.Code == settings.Language)),
                MinWidth = 240,
            };
            var displayPanel = new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new TextBlock { Text = Strings.LanguageLabel, FontWeight = FontWeight.Bold },
                    Language,
                    new TextBlock { Text = Strings.DisplayScaleNote, TextWrapping = TextWrapping.Wrap, FontSize = 12, Opacity = 0.75 },
                },
            };

            // ログ: レベル・ファイルに出すか・フォルダ・残す日数
            LogLevel = new ComboBox
            {
                ItemsSource = LogLevels.Select(x => x == "" ? Strings.LogLevelDefault : x).ToList(),
                SelectedIndex = Math.Max(0, LogLevels.ToList().FindIndex(x => string.Equals(x, settings.LogLevel, StringComparison.OrdinalIgnoreCase))),
                MinWidth = 240,
            };
            LogFile = new CheckBox { Content = Strings.LogFileLabel, IsChecked = settings.LogFile };
            LogPath = new PathField(Strings.LogFolderLabel, settings.LogPath, DescribeLogFolder, BrowseFolderAsync);
            LogRetainDays = new NumericUpDown { Value = settings.LogRetainDays, Minimum = 1, Maximum = 365, Increment = 1, FormatString = "0", MinWidth = 140, HorizontalAlignment = HorizontalAlignment.Left };
            var openLogs = new Button { Content = Strings.OpenLogFolder };
            openLogs.Click += async (s, e) => await OpenLogFolderAsync(this, LogPath.Value);
            var logPanel = new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new TextBlock { Text = Strings.LogLevelLabel, FontWeight = FontWeight.Bold },
                    LogLevel,
                    new TextBlock { Text = Strings.LogLevelFixedNote, TextWrapping = TextWrapping.Wrap, FontSize = 12, Foreground = Brushes.DarkRed, IsVisible = logLevelFixed },
                    LogFile,
                    LogPath.Panel,
                    new TextBlock { Text = Strings.LogRetainDaysLabel, FontWeight = FontWeight.Bold },
                    LogRetainDays,
                    openLogs,
                },
            };

            // ゲーム（元の SRC の設定変更の画面と同じ項目と、自動反撃モード・音量）
            BattleAnimation = new CheckBox { Content = SRCRender.GameConfigLabels.BattleAnimation, IsChecked = settings.BattleAnimation };
            ExtendedAnimation = new CheckBox { Content = SRCRender.GameConfigLabels.ExtendedAnimation, IsChecked = settings.ExtendedAnimation, Margin = new Thickness(20, 0, 0, 0) };
            WeaponAnimation = new CheckBox { Content = SRCRender.GameConfigLabels.WeaponAnimation, IsChecked = settings.WeaponAnimation, Margin = new Thickness(20, 0, 0, 0) };
            SpecialPowerAnimation = new CheckBox { Content = SRCRender.GameConfigLabels.SpecialPowerAnimation, IsChecked = settings.SpecialPowerAnimation };
            MoveAnimation = new CheckBox { Content = SRCRender.GameConfigLabels.MoveAnimation, IsChecked = settings.MoveAnimation };
            AutoMoveCursor = new CheckBox { Content = SRCRender.GameConfigLabels.AutoMoveCursor, IsChecked = settings.AutoMoveCursor };
            ShowSquareLine = new CheckBox { Content = SRCRender.GameConfigLabels.ShowSquareLine, IsChecked = settings.ShowSquareLine };
            ShowTurn = new CheckBox { Content = SRCRender.GameConfigLabels.ShowTurn, IsChecked = settings.ShowTurn };
            KeepEnemyBGM = new CheckBox { Content = SRCRender.GameConfigLabels.KeepEnemyBGM, IsChecked = settings.KeepEnemyBGM };
            AutoDefense = new CheckBox { Content = Strings.AutoDefenseLabel, IsChecked = settings.AutoDefense };
            // 戦闘アニメを表示しない場合は、拡張戦闘アニメ・武器準備アニメを選べない（元の SRC と同じ）
            void UpdateAnimationEnabled()
            {
                ExtendedAnimation.IsEnabled = BattleAnimation.IsChecked == true;
                WeaponAnimation.IsEnabled = BattleAnimation.IsChecked == true;
            }
            BattleAnimation.IsCheckedChanged += (s, e) => UpdateAnimationEnabled();
            UpdateAnimationEnabled();
            SoundVolume = new Slider { Minimum = 0, Maximum = 100, Value = settings.SoundVolume, Width = 240, TickFrequency = 1, IsSnapToTickEnabled = true };
            var volumeText = new TextBlock { Text = settings.SoundVolume.ToString(), VerticalAlignment = VerticalAlignment.Center, MinWidth = 32 };
            SoundVolume.PropertyChanged += (s, e) =>
            {
                if (e.Property == Slider.ValueProperty)
                {
                    volumeText.Text = ((int)SoundVolume.Value).ToString();
                }
            };
            var gamePanel = new StackPanel
            {
                Spacing = 6,
                Children =
                {
                    BattleAnimation, ExtendedAnimation, WeaponAnimation, SpecialPowerAnimation, MoveAnimation,
                    AutoMoveCursor, ShowSquareLine, ShowTurn, KeepEnemyBGM, AutoDefense,
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 8,
                        Children = { new TextBlock { Text = SRCRender.GameConfigLabels.Volume, VerticalAlignment = VerticalAlignment.Center }, SoundVolume, volumeText },
                    },
                },
            };

            // BGM（場面ごとの曲のファイル名）
            BgmFields = BgmNames.ToDictionary(name => name, name =>
            {
                settings.Bgm.TryGetValue(name, out var file);
                return new TextBox { Text = file ?? "", PlaceholderText = name + ".mid", MinWidth = 300 };
            });
            var bgmGrid = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,*"), RowSpacing = 4, ColumnSpacing = 8 };
            for (var i = 0; i < BgmNames.Count; i++)
            {
                bgmGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                var label = new TextBlock { Text = BgmNames[i] + "（" + BgmDescription(BgmNames[i]) + "）", VerticalAlignment = VerticalAlignment.Center };
                Grid.SetRow(label, i);
                var box = BgmFields[BgmNames[i]];
                Grid.SetRow(box, i);
                Grid.SetColumn(box, 1);
                bgmGrid.Children.Add(label);
                bgmGrid.Children.Add(box);
            }
            BgmImportStatus = new TextBlock { TextWrapping = TextWrapping.Wrap, FontSize = 12, IsVisible = false };
            var importBgm = new Button { Content = Strings.BgmImport };
            importBgm.Click += async (s, e) => await ImportBgmAsync(ExtDataPath.Value);
            var bgmPanel = new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new TextBlock { Text = Strings.BgmDescriptionText, TextWrapping = TextWrapping.Wrap, FontSize = 12 },
                    bgmGrid,
                    importBgm,
                    BgmImportStatus,
                },
            };

            // 開発者向け
            DebugMode = new CheckBox { Content = Strings.DebugModeLabel, IsChecked = settings.DebugMode };
            // 「Shift_JIS」の _ をアクセスキーにしないよう、文字は TextBlock で示す
            CompatibleRead = new CheckBox { Content = new TextBlock { Text = Strings.CompatibleReadLabel }, IsChecked = settings.CompatibleRead };
            CompatibleWrite = new CheckBox { Content = new TextBlock { Text = Strings.CompatibleWriteLabel }, IsChecked = settings.CompatibleWrite };
            ImageBufferSize = new NumericUpDown { Value = settings.ImageBufferSize, Minimum = 0, Maximum = 100000, FormatString = "0", MinWidth = 160, HorizontalAlignment = HorizontalAlignment.Left };
            MaxImageBufferByteSize = new NumericUpDown { Value = settings.MaxImageBufferByteSize, Minimum = 0, Maximum = 100000, FormatString = "0", MinWidth = 160, HorizontalAlignment = HorizontalAlignment.Left };
            KeepStretchedImage = new CheckBox { Content = Strings.KeepStretchedImageLabel, IsChecked = settings.KeepStretchedImage };
            var developerPanel = new StackPanel
            {
                Spacing = 6,
                Children =
                {
                    new TextBlock { Text = Strings.DeveloperDescription, TextWrapping = TextWrapping.Wrap, FontSize = 12, Foreground = Brushes.DarkRed },
                    DebugMode,
                    new TextBlock { Text = Strings.DebugModeNote, TextWrapping = TextWrapping.Wrap, FontSize = 12, Opacity = 0.75, Margin = new Thickness(28, 0, 0, 4) },
                    CompatibleRead,
                    CompatibleWrite,
                    new TextBlock { Text = Strings.CompatibleNote, TextWrapping = TextWrapping.Wrap, FontSize = 12, Opacity = 0.75, Margin = new Thickness(28, 0, 0, 4) },
                    new TextBlock { Text = Strings.ImageBufferSizeLabel, FontWeight = FontWeight.Bold },
                    ImageBufferSize,
                    new TextBlock { Text = Strings.MaxImageBufferByteSizeLabel, FontWeight = FontWeight.Bold },
                    MaxImageBufferByteSize,
                    KeepStretchedImage,
                    new TextBlock { Text = Strings.ImageBufferNote, TextWrapping = TextWrapping.Wrap, FontSize = 12, Opacity = 0.75 },
                },
            };

            Content = new StackPanel
            {
                Margin = new Thickness(12),
                Spacing = 12,
                Width = 640,
                Children =
                {
                    new TabControl
                    {
                        ItemsSource = new[]
                        {
                            new TabItem
                            {
                                Header = Strings.AssetsTab,
                                Content = new StackPanel
                                {
                                    Spacing = 12,
                                    Margin = new Thickness(0, 8, 0, 0),
                                    Children =
                                    {
                                        new TextBlock { Text = string.Format(Strings.AssetSettingsDescription, appPath), TextWrapping = TextWrapping.Wrap },
                                        ExtDataPath.Panel,
                                        ExtDataPath2.Panel,
                                        SoundFont.Panel,
                                        packsPanel,
                                    },
                                },
                            },
                            new TabItem { Header = Strings.GameTab, Content = new StackPanel { Margin = new Thickness(0, 8, 0, 0), Children = { gamePanel } } },
                            new TabItem { Header = Strings.BgmTab, Content = new ScrollViewer { MaxHeight = 480, Margin = new Thickness(0, 8, 0, 0), Content = bgmPanel } },
                            new TabItem { Header = Strings.DisplayTab, Content = new StackPanel { Margin = new Thickness(0, 8, 0, 0), Children = { displayPanel } } },
                            new TabItem { Header = Strings.LogTab, Content = new StackPanel { Margin = new Thickness(0, 8, 0, 0), Children = { logPanel } } },
                            new TabItem { Header = Strings.DeveloperTab, Content = new StackPanel { Margin = new Thickness(0, 8, 0, 0), Children = { developerPanel } } },
                        },
                        SelectedIndex = initialTab,
                    },
                    new TextBlock { Text = Strings.SettingsApplyNote, TextWrapping = TextWrapping.Wrap, Opacity = 0.75 },
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
        internal ComboBox Language { get; }
        internal ComboBox LogLevel { get; }
        internal CheckBox LogFile { get; }
        internal PathField LogPath { get; }
        internal NumericUpDown LogRetainDays { get; }
        internal CheckBox BattleAnimation { get; }
        internal CheckBox ExtendedAnimation { get; }
        internal CheckBox WeaponAnimation { get; }
        internal CheckBox SpecialPowerAnimation { get; }
        internal CheckBox MoveAnimation { get; }
        internal CheckBox AutoMoveCursor { get; }
        internal CheckBox ShowSquareLine { get; }
        internal CheckBox ShowTurn { get; }
        internal CheckBox KeepEnemyBGM { get; }
        internal CheckBox AutoDefense { get; }
        internal Slider SoundVolume { get; }
        internal IReadOnlyDictionary<string, TextBox> BgmFields { get; }
        internal TextBlock BgmImportStatus { get; }
        internal CheckBox DebugMode { get; }
        internal CheckBox CompatibleRead { get; }
        internal CheckBox CompatibleWrite { get; }
        internal NumericUpDown ImageBufferSize { get; }
        internal NumericUpDown MaxImageBufferByteSize { get; }
        internal CheckBox KeepStretchedImage { get; }

        /// <summary>
        /// 元の SRC の設定ファイル（Src.ini）の [BGM] を読んで、BGM の欄に入れる。入れた数を返す
        /// </summary>
        internal int ImportBgm(string srcIniPath)
        {
            var bgm = SrcIni.ReadSection(srcIniPath, App.BgmSection);
            var count = 0;
            foreach (var name in BgmNames)
            {
                if (bgm.TryGetValue(name, out var file) && !string.IsNullOrWhiteSpace(file))
                {
                    BgmFields[name].Text = file.Trim();
                    count++;
                }
            }
            BgmImportStatus.Text = count > 0 ? string.Format(Strings.BgmImported, count, srcIniPath) : string.Format(Strings.BgmImportNone, srcIniPath);
            BgmImportStatus.Foreground = count > 0 ? Brushes.DarkGreen : Brushes.DarkRed;
            BgmImportStatus.IsVisible = true;
            return count;
        }

        /// <summary>Src.ini を選ばせて取り込む。拡張データフォルダ 1 に Src.ini があれば、そこから選ばせる</summary>
        private async System.Threading.Tasks.Task ImportBgmAsync(string extDataPath)
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = Strings.BgmImport,
                AllowMultiple = false,
                SuggestedStartLocation = await StartLocation(extDataPath),
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Src.ini") { Patterns = new[] { "*.ini" } },
                    new FilePickerFileType(Strings.AllFiles) { Patterns = new[] { "*" } },
                },
            });
            var file = files.FirstOrDefault()?.TryGetLocalPath();
            if (!string.IsNullOrEmpty(file))
            {
                ImportBgm(file);
            }
        }

        /// <summary>ログのフォルダの状態。空欄なら既定のフォルダを示す</summary>
        internal static (string Text, bool Warning) DescribeLogFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return (string.Format(Strings.LogFolderDefault, SRCCore.Logging.LogOptions.DefaultDirectory()), false);
            }
            return Directory.Exists(path) ? ("", false) : (Strings.LogFolderWillBeCreated, false);
        }

        /// <summary>ログのフォルダ（空なら既定のフォルダ）を開く。無ければ作る</summary>
        internal static async System.Threading.Tasks.Task OpenLogFolderAsync(TopLevel topLevel, string path)
        {
            var dir = string.IsNullOrEmpty(path) ? SRCCore.Logging.LogOptions.DefaultDirectory() : path;
            try
            {
                Directory.CreateDirectory(dir);
                await topLevel.Launcher.LaunchDirectoryInfoAsync(new DirectoryInfo(dir));
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                // 作れない・開けないときは何もしない
            }
        }

        /// <summary>素材パックの一覧（zip ごとに 1 行）</summary>
        internal StackPanel PackList { get; }

        /// <summary>素材パックを置くフォルダ（SRC# のフォルダの Packs）</summary>
        internal string PacksDirectory => SRCCore.Filesystem.AssetPacks.Directory(appPath);

        /// <summary>素材パックの一覧を、Packs フォルダの中身に合わせて作り直す</summary>
        internal void UpdatePackList()
        {
            PackList.Children.Clear();
            var packs = SRCCore.Filesystem.AssetPacks.Find(appPath);
            if (packs.Count == 0)
            {
                PackList.Children.Add(new TextBlock { Text = Strings.NoPacks, FontSize = 12, Opacity = 0.75 });
                return;
            }
            foreach (var pack in packs)
            {
                var (text, warning) = DescribePack(pack);
                PackList.Children.Add(new TextBlock
                {
                    Text = Path.GetFileName(pack) + " — " + text,
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 12,
                    Foreground = warning ? Brushes.DarkRed : Brushes.DarkGreen,
                });
            }
        }

        /// <summary>素材パックの状態（中の素材のフォルダ）。素材のフォルダが無いか読めなければ warning</summary>
        internal static (string Text, bool Warning) DescribePack(string zip)
        {
            try
            {
                using (var archive = System.IO.Compression.ZipFile.Open(zip, System.IO.Compression.ZipArchiveMode.Read, SRCCore.Filesystem.ZipNameEncoding.Instance))
                {
                    var root = SRCCore.Filesystem.AssetPacks.FindRoot(archive);
                    if (root == null)
                    {
                        return (Strings.PackHasNoAssets, true);
                    }
                    var found = SRCCore.Filesystem.AssetPacks.FoundAssetFolders(archive, root);
                    return (string.Format(Strings.FolderFound, string.Join(", ", found)), false);
                }
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidDataException || ex is UnauthorizedAccessException)
            {
                return (Strings.PackUnreadable, true);
            }
        }

        /// <summary>
        /// 選んだ zip を Packs フォルダに写す（同じ名前のものは置き換える）。
        /// 写せなかったもの（シナリオの実行中に読んでいる zip の置き換えなど）は、一覧の後ろに理由を示す
        /// </summary>
        internal void AddPacks(IEnumerable<string> zips)
        {
            var failed = new List<string>();
            foreach (var zip in zips)
            {
                try
                {
                    Directory.CreateDirectory(PacksDirectory);
                    File.Copy(zip, Path.Combine(PacksDirectory, Path.GetFileName(zip)), true);
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    failed.Add(Path.GetFileName(zip));
                }
            }
            UpdatePackList();
            foreach (var name in failed)
            {
                PackList.Children.Add(new TextBlock { Text = string.Format(Strings.PackCopyFailed, name), TextWrapping = TextWrapping.Wrap, FontSize = 12, Foreground = Brushes.DarkRed });
            }
        }

        private async System.Threading.Tasks.Task AddPacksAsync()
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = Strings.AddPack,
                AllowMultiple = true,
                FileTypeFilter = new[] { new FilePickerFileType("zip (*.zip)") { Patterns = new[] { "*.zip" } } },
            });
            AddPacks(files.Select(x => x.TryGetLocalPath()).Where(x => !string.IsNullOrEmpty(x)));
        }

        /// <summary>入力している設定</summary>
        internal AppSettings Result()
        {
            return new AppSettings
            {
                ExtDataPath = ExtDataPath.Value,
                ExtDataPath2 = ExtDataPath2.Value,
                SoundFont = SoundFont.Value,
                Language = Languages[Math.Max(0, Language.SelectedIndex)].Code,
                LogLevel = LogLevels[Math.Max(0, LogLevel.SelectedIndex)],
                LogFile = LogFile.IsChecked ?? true,
                LogPath = LogPath.Value,
                LogRetainDays = (int)(LogRetainDays.Value ?? SRCCore.Logging.LogOptions.DefaultRetainDays),
                BattleAnimation = BattleAnimation.IsChecked == true,
                ExtendedAnimation = ExtendedAnimation.IsChecked == true,
                WeaponAnimation = WeaponAnimation.IsChecked == true,
                SpecialPowerAnimation = SpecialPowerAnimation.IsChecked == true,
                MoveAnimation = MoveAnimation.IsChecked == true,
                AutoMoveCursor = AutoMoveCursor.IsChecked == true,
                ShowSquareLine = ShowSquareLine.IsChecked == true,
                ShowTurn = ShowTurn.IsChecked == true,
                KeepEnemyBGM = KeepEnemyBGM.IsChecked == true,
                AutoDefense = AutoDefense.IsChecked == true,
                SoundVolume = (int)SoundVolume.Value,
                Bgm = BgmFields.ToDictionary(x => x.Key, x => (x.Value.Text ?? "").Trim()),
                DebugMode = DebugMode.IsChecked == true,
                CompatibleRead = CompatibleRead.IsChecked == true,
                CompatibleWrite = CompatibleWrite.IsChecked == true,
                ImageBufferSize = (int)(ImageBufferSize.Value ?? 0),
                MaxImageBufferByteSize = (int)(MaxImageBufferByteSize.Value ?? 0),
                KeepStretchedImage = KeepStretchedImage.IsChecked == true,
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
                Title = Strings.SettingsTitle,
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
