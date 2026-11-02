using Avalonia.Headless;
using Avalonia.Interactivity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
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
        public void GameBgmAndDeveloperSettings_RoundTripThroughConfig()
        {
            var config = new LocalFileConfig();
            var settings = new AppSettings
            {
                BattleAnimation = false, ExtendedAnimation = false, WeaponAnimation = true, SpecialPowerAnimation = false, MoveAnimation = false,
                AutoMoveCursor = false, ShowSquareLine = true, ShowTurn = true, KeepEnemyBGM = true, AutoDefense = true, SoundVolume = 80,
                Bgm = new System.Collections.Generic.Dictionary<string, string> { ["Map1"] = "地上.mid", ["default"] = "battle.mid" },
                DebugMode = true, CompatibleRead = true, CompatibleWrite = false, ImageBufferSize = 64, MaxImageBufferByteSize = 32, KeepStretchedImage = true,
            };
            App.WriteSettings(config, settings);

            Assert.IsFalse(config.BattleAnimation);
            Assert.IsTrue(config.ShowSquareLine);
            Assert.AreEqual("On", config.GetItem("Option", "Turn"));
            Assert.IsTrue(config.AutoDefense);
            Assert.AreEqual(80, config.SoundVolume);
            Assert.AreEqual("地上.mid", config.GetItem("BGM", "Map1"));
            Assert.AreEqual("battle.mid", config.GetItem("BGM", "default"));
            Assert.AreEqual("", config.GetItem("BGM", "Map2"));
            Assert.AreEqual("On", config.GetItem("Option", "DebugMode"));
            Assert.AreEqual(SRCCompatibilityMode.Read, config.SRCCompatibilityMode);
            Assert.AreEqual(64, config.ImageBufferSize);

            var read = App.ReadSettings(config);
            Assert.IsFalse(read.BattleAnimation);
            Assert.IsTrue(read.WeaponAnimation);
            Assert.IsTrue(read.ShowTurn);
            Assert.IsTrue(read.KeepEnemyBGM);
            Assert.IsTrue(read.AutoDefense);
            Assert.AreEqual(80, read.SoundVolume);
            Assert.AreEqual("地上.mid", read.Bgm["Map1"]);
            Assert.AreEqual("", read.Bgm["Map2"]);
            Assert.IsTrue(read.DebugMode);
            Assert.IsTrue(read.CompatibleRead);
            Assert.IsFalse(read.CompatibleWrite);
            Assert.AreEqual(32, read.MaxImageBufferByteSize);
            Assert.IsTrue(read.KeepStretchedImage);
        }

        [TestMethod]
        public void Window_ImportsBgmFromSrcIni()
        {
            // 元の SRC の Src.ini（Shift_JIS）
            var ini = Path.Combine(root, "Src.ini");
            File.WriteAllBytes(ini, SRCCore.Filesystem.ZipNameEncoding.ShiftJis.GetBytes(
                "[Option]\r\nDebugMode=Off\r\n[BGM]\r\nOpening=オープニング.mid\r\nMap1=GSC_Map1.mid\r\nUnknown=x.mid\r\n[Other]\r\nMap2=no.mid\r\n"));
            using (var session = HeadlessUnitTestSession.StartNew(typeof(TestApp)))
            {
                session.Dispatch(() =>
                {
                    var window = new SettingsWindow(new AppSettings(), root, initialTab: SettingsWindow.DeveloperTabIndex);
                    window.Show();
                    Assert.AreEqual(SettingsWindow.DeveloperTabIndex, window.GetLogicalDescendants().OfType<Avalonia.Controls.TabControl>().First().SelectedIndex);
                    Assert.AreEqual(2, window.ImportBgm(ini));
                    var result = window.Result();
                    Assert.AreEqual("オープニング.mid", result.Bgm["Opening"]);
                    Assert.AreEqual("GSC_Map1.mid", result.Bgm["Map1"]);
                    Assert.AreEqual("", result.Bgm["Map2"], "[BGM] 以外の項目は読まない");
                    // 戦闘アニメを表示しなければ、拡張戦闘アニメ・武器準備アニメを選べない
                    window.BattleAnimation.IsChecked = false;
                    Assert.IsFalse(window.ExtendedAnimation.IsEnabled);
                    Assert.IsFalse(window.WeaponAnimation.IsEnabled);
                    window.Close();
                    return true;
                }, CancellationToken.None).GetAwaiter().GetResult();
            }
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
                    var dir = Path.Combine(AppContext.BaseDirectory, "TestResults", "avalonia");
                    Directory.CreateDirectory(dir);
                    foreach (var (index, name) in new[] { (1, "game"), (2, "bgm"), (4, "log"), (SettingsWindow.DeveloperTabIndex, "developer") })
                    {
                        tabs.SelectedIndex = index;
                        Avalonia.Headless.AvaloniaHeadlessPlatform.ForceRenderTimerTick();
                        window.CaptureRenderedFrame()?.Save(Path.Combine(dir, "settings-" + name + ".png"));
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
                    var requested = new System.Collections.Generic.List<int>();
                    window.SettingsRequested += tab => requested.Add(tab);
                    window.SettingsItem.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.MenuItem.ClickEvent));
                    window.DeveloperSettingsItem.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.MenuItem.ClickEvent));
                    // 「開発者向けの設定...」は開発者向けのタブを開く
                    CollectionAssert.AreEqual(new[] { 0, SettingsWindow.DeveloperTabIndex }, requested);
                    window.Close();
                    return true;
                }, CancellationToken.None).GetAwaiter().GetResult();
            }
        }
    }
}
