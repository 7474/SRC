using Avalonia.Headless;
using Avalonia.Interactivity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using System;
using System.IO;
using System.Linq;
using Avalonia.LogicalTree;
using System.Threading;

namespace SRCSharpAvalonia.Tests
{
    /// <summary>
    /// 素材フォルダの設定（メニューの「設定 > 素材フォルダの設定...」）。
    /// </summary>
    [TestClass]
    public class SettingsTests
    {
        private string root;

        [TestInitialize]
        public void Initialize()
        {
            root = Path.Combine(Path.GetTempPath(), "srcs-asset-settings-" + Guid.NewGuid());
            Directory.CreateDirectory(root);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete(root, true);
        }

        [TestMethod]
        public void DescribeFolder_ListsAssetFolders()
        {
            Directory.CreateDirectory(Path.Combine(root, "bitmap"));
            Directory.CreateDirectory(Path.Combine(root, "Data"));
            Directory.CreateDirectory(Path.Combine(root, "Other"));

            var (text, warning) = SettingsWindow.DescribeFolder(root);

            Assert.IsFalse(warning);
            CollectionAssert.AreEqual(new[] { "Bitmap", "Data" }, (System.Collections.ICollection)SettingsWindow.FoundAssetFolders(root));
            StringAssert.Contains(text, "Bitmap, Data");
        }

        [TestMethod]
        public void DescribeFolder_WarnsAboutLikelyMistakes()
        {
            // 空欄は何も示さない
            Assert.AreEqual(("", false), SettingsWindow.DescribeFolder(""));
            // 無いフォルダ
            Assert.IsTrue(SettingsWindow.DescribeFolder(Path.Combine(root, "missing")).Warning);
            // 素材のフォルダが一つも無い
            Assert.IsTrue(SettingsWindow.DescribeFolder(root).Warning);
            // Bitmap フォルダそのものを選んだ（親を指定する）
            var bitmap = Directory.CreateDirectory(Path.Combine(root, "Bitmap")).FullName;
            var (text, warning) = SettingsWindow.DescribeFolder(bitmap + Path.DirectorySeparatorChar);
            Assert.IsTrue(warning);
            StringAssert.Contains(text, "Bitmap");
        }

        [TestMethod]
        public void Packs_AreListedWithTheirAssetFolders()
        {
            var source = Path.Combine(root, "source");
            Directory.CreateDirectory(source);
            var good = Path.Combine(source, "graphics.zip");
            using (var archive = System.IO.Compression.ZipFile.Open(good, System.IO.Compression.ZipArchiveMode.Create))
            {
                archive.CreateEntry("汎用グラフィック集/Bitmap/Unit/a.bmp");
                archive.CreateEntry("汎用グラフィック集/Sound/a.wav");
            }
            var bad = Path.Combine(source, "readme.zip");
            using (var archive = System.IO.Compression.ZipFile.Open(bad, System.IO.Compression.ZipArchiveMode.Create))
            {
                archive.CreateEntry("readme.txt");
            }
            File.WriteAllText(Path.Combine(source, "broken.zip"), "not a zip");

            Assert.AreEqual((string.Format(Strings.FolderFound, "Bitmap, Sound"), false), SettingsWindow.DescribePack(good));
            Assert.AreEqual((Strings.PackHasNoAssets, true), SettingsWindow.DescribePack(bad));
            Assert.AreEqual((Strings.PackUnreadable, true), SettingsWindow.DescribePack(Path.Combine(source, "broken.zip")));

            using (var session = HeadlessUnitTestSession.StartNew(typeof(TestApp)))
            {
                session.Dispatch(() =>
                {
                    var window = new SettingsWindow(new AppSettings(), root);
                    Assert.AreEqual(1, window.PackList.Children.Count, "no packs");
                    // 追加すると Packs フォルダに写し、一覧に出す
                    window.AddPacks(new[] { good, bad });
                    Assert.IsTrue(File.Exists(Path.Combine(root, "Packs", "graphics.zip")));
                    Assert.AreEqual(2, window.PackList.Children.Count);
                    StringAssert.StartsWith(((Avalonia.Controls.TextBlock)window.PackList.Children[0]).Text, "graphics.zip");
                    return true;
                }, CancellationToken.None).GetAwaiter().GetResult();
            }
        }

        [TestMethod]
        public void Settings_RoundTripThroughConfig()
        {
            var config = new LocalFileConfig();
            App.WriteSettings(config, new AppSettings
            {
                ExtDataPath = "/a", ExtDataPath2 = "/b", SoundFont = "/c.sf2",
                Language = "en", LogLevel = "Debug", LogFile = false, LogPath = "/logs", LogRetainDays = 3,
            });

            Assert.AreEqual("/a", config.ExtDataPath);
            Assert.AreEqual("/b", config.ExtDataPath2);
            Assert.AreEqual("/c.sf2", config.GetItem("Option", "SoundFont"));
            Assert.AreEqual("en", config.GetItem("Option", "Language"));
            Assert.AreEqual("Debug", config.GetItem("Log", "Level"));
            Assert.AreEqual("Off", config.GetItem("Log", "File"));
            Assert.AreEqual("/logs", config.GetItem("Log", "Path"));
            Assert.AreEqual("3", config.GetItem("Log", "RetainDays"));
            // ログの設定として読める（docs/logging.md の Log セクション）
            var log = SRCCore.Logging.LogSettings.FromConfig(config);
            Assert.AreEqual(Microsoft.Extensions.Logging.LogLevel.Debug, log.Level);
            Assert.AreEqual(false, log.File);
            Assert.AreEqual("/logs", log.Directory);
            Assert.AreEqual(3, log.RetainDays);

            var read = App.ReadSettings(config);
            Assert.AreEqual("/a", read.ExtDataPath);
            Assert.AreEqual("/b", read.ExtDataPath2);
            Assert.AreEqual("/c.sf2", read.SoundFont);
            Assert.AreEqual("en", read.Language);
            Assert.AreEqual("Debug", read.LogLevel);
            Assert.IsFalse(read.LogFile);
            Assert.AreEqual("/logs", read.LogPath);
            Assert.AreEqual(3, read.LogRetainDays);
        }

        [TestMethod]
        public void Settings_DefaultsWhenEmpty()
        {
            var read = App.ReadSettings(new LocalFileConfig());
            // 設定が無ければ日本語・既定のレベル・ファイルに出す・7 日
            Assert.AreEqual("ja", read.Language);
            Assert.AreEqual("", read.LogLevel);
            Assert.IsTrue(read.LogFile);
            Assert.AreEqual("", read.LogPath);
            Assert.AreEqual(7, read.LogRetainDays);
        }

        [TestMethod]
        public void Window_ReturnsDisplayAndLogSettings()
        {
            using (var session = HeadlessUnitTestSession.StartNew(typeof(TestApp)))
            {
                session.Dispatch(() =>
                {
                    var window = new SettingsWindow(new AppSettings { Language = "auto", LogLevel = "Warning" }, root, logLevelFixed: true);
                    window.Show();
                    Assert.AreEqual(0, window.Language.SelectedIndex, "auto");
                    Assert.AreEqual("Warning", SettingsWindow.LogLevels[window.LogLevel.SelectedIndex]);
                    // 空欄なら既定のフォルダを示す
                    StringAssert.Contains(window.LogPath.Status.Text, SRCCore.Logging.LogOptions.DefaultDirectory());

                    window.Language.SelectedIndex = SettingsWindow.Languages.ToList().FindIndex(x => x.Code == "en");
                    window.LogLevel.SelectedIndex = SettingsWindow.LogLevels.ToList().IndexOf("Trace");
                    window.LogFile.IsChecked = false;
                    window.LogRetainDays.Value = 30;
                    var result = window.Result();
                    Assert.AreEqual("en", result.Language);
                    Assert.AreEqual("Trace", result.LogLevel);
                    Assert.IsFalse(result.LogFile);
                    Assert.AreEqual(30, result.LogRetainDays);

                    // ログのタブの見た目を確かめるため、画面を保存する
                    var tabs = window.GetLogicalDescendants().OfType<Avalonia.Controls.TabControl>().First();
                    tabs.SelectedIndex = 2;
                    Avalonia.Headless.AvaloniaHeadlessPlatform.ForceRenderTimerTick();
                    var frame = window.CaptureRenderedFrame();
                    if (frame != null)
                    {
                        var dir = Path.Combine(AppContext.BaseDirectory, "TestResults", "avalonia");
                        Directory.CreateDirectory(dir);
                        frame.Save(Path.Combine(dir, "settings-log.png"));
                    }
                    window.Close();
                    return true;
                }, CancellationToken.None).GetAwaiter().GetResult();
            }
        }

        [TestMethod]
        public void ReadLastScenarioFolder_ReturnsExistingFolderOnly()
        {
            var config = new LocalFileConfig();
            Assert.IsNull(App.ReadLastScenarioFolder(config));
            config.SetItem(App.LastScenarioFolderSection, App.LastScenarioFolderName, root);
            Assert.AreEqual(root, App.ReadLastScenarioFolder(config));
            // 消えたフォルダは使わない（OS の既定の場所から選ばせる）
            config.SetItem(App.LastScenarioFolderSection, App.LastScenarioFolderName, Path.Combine(root, "removed"));
            Assert.IsNull(App.ReadLastScenarioFolder(config));
        }

        [TestMethod]
        public void Window_ShowsStatusAndReturnsEditedSettings()
        {
            Directory.CreateDirectory(Path.Combine(root, "Sound"));
            using (var session = HeadlessUnitTestSession.StartNew(typeof(TestApp)))
            {
                session.Dispatch(() =>
                {
                    var window = new SettingsWindow(new AppSettings { ExtDataPath = "", ExtDataPath2 = "", SoundFont = "" }, root);
                    window.Show();
                    Assert.IsFalse(window.ExtDataPath.Status.IsVisible);

                    window.ExtDataPath.TextBox.Text = root;
                    Assert.IsTrue(window.ExtDataPath.Status.IsVisible);
                    StringAssert.Contains(window.ExtDataPath.Status.Text, "Sound");
                    window.SoundFont.TextBox.Text = Path.Combine(root, "missing.sf2");
                    Assert.IsTrue(window.SoundFont.Status.IsVisible);

                    // 見た目を確かめるため、画面を保存する
                    Avalonia.Headless.AvaloniaHeadlessPlatform.ForceRenderTimerTick();
                    var frame = window.CaptureRenderedFrame();
                    if (frame != null)
                    {
                        var dir = Path.Combine(AppContext.BaseDirectory, "TestResults", "avalonia");
                        Directory.CreateDirectory(dir);
                        frame.Save(Path.Combine(dir, "asset-settings.png"));
                    }

                    var result = window.Result();
                    Assert.AreEqual(root, result.ExtDataPath);
                    Assert.AreEqual("", result.ExtDataPath2);
                    Assert.AreEqual(Path.Combine(root, "missing.sf2"), result.SoundFont);
                    window.Close();
                    return true;
                }, CancellationToken.None).GetAwaiter().GetResult();
            }
        }

        [TestMethod]
        public void MenuItem_RequestsSettings()
        {
            using (var session = HeadlessUnitTestSession.StartNew(typeof(TestApp)))
            {
                session.Dispatch(() =>
                {
                    var window = new MainWindow();
                    window.Show();
                    var requested = false;
                    window.SettingsRequested += () => requested = true;
                    window.SettingsItem.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.MenuItem.ClickEvent));
                    Assert.IsTrue(requested);
                    window.Close();
                    return true;
                }, CancellationToken.None).GetAwaiter().GetResult();
            }
        }
    }
}
