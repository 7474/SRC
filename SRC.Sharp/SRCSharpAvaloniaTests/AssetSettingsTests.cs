using Avalonia.Headless;
using Avalonia.Interactivity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using System;
using System.IO;
using System.Threading;

namespace SRCSharpAvalonia.Tests
{
    /// <summary>
    /// 素材フォルダの設定（メニューの「設定 > 素材フォルダの設定...」）。
    /// </summary>
    [TestClass]
    public class AssetSettingsTests
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

            var (text, warning) = AssetSettingsWindow.DescribeFolder(root);

            Assert.IsFalse(warning);
            CollectionAssert.AreEqual(new[] { "Bitmap", "Data" }, (System.Collections.ICollection)AssetSettingsWindow.FoundAssetFolders(root));
            StringAssert.Contains(text, "Bitmap, Data");
        }

        [TestMethod]
        public void DescribeFolder_WarnsAboutLikelyMistakes()
        {
            // 空欄は何も示さない
            Assert.AreEqual(("", false), AssetSettingsWindow.DescribeFolder(""));
            // 無いフォルダ
            Assert.IsTrue(AssetSettingsWindow.DescribeFolder(Path.Combine(root, "missing")).Warning);
            // 素材のフォルダが一つも無い
            Assert.IsTrue(AssetSettingsWindow.DescribeFolder(root).Warning);
            // Bitmap フォルダそのものを選んだ（親を指定する）
            var bitmap = Directory.CreateDirectory(Path.Combine(root, "Bitmap")).FullName;
            var (text, warning) = AssetSettingsWindow.DescribeFolder(bitmap + Path.DirectorySeparatorChar);
            Assert.IsTrue(warning);
            StringAssert.Contains(text, "Bitmap");
        }

        [TestMethod]
        public void AssetSettings_RoundTripsThroughConfig()
        {
            var config = new LocalFileConfig();
            App.WriteAssetSettings(config, new AssetSettings { ExtDataPath = "/a", ExtDataPath2 = "/b", SoundFont = "/c.sf2" });

            Assert.AreEqual("/a", config.ExtDataPath);
            Assert.AreEqual("/b", config.ExtDataPath2);
            Assert.AreEqual("/c.sf2", config.GetItem("Option", "SoundFont"));
            var read = App.ReadAssetSettings(config);
            Assert.AreEqual("/a", read.ExtDataPath);
            Assert.AreEqual("/b", read.ExtDataPath2);
            Assert.AreEqual("/c.sf2", read.SoundFont);
        }

        [TestMethod]
        public void Window_ShowsStatusAndReturnsEditedSettings()
        {
            Directory.CreateDirectory(Path.Combine(root, "Sound"));
            using (var session = HeadlessUnitTestSession.StartNew(typeof(TestApp)))
            {
                session.Dispatch(() =>
                {
                    var window = new AssetSettingsWindow(new AssetSettings { ExtDataPath = "", ExtDataPath2 = "", SoundFont = "" }, root);
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
        public void MenuItem_RequestsAssetSettings()
        {
            using (var session = HeadlessUnitTestSession.StartNew(typeof(TestApp)))
            {
                session.Dispatch(() =>
                {
                    var window = new MainWindow();
                    window.Show();
                    var requested = false;
                    window.AssetSettingsRequested += () => requested = true;
                    window.AssetSettingsItem.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.MenuItem.ClickEvent));
                    Assert.IsTrue(requested);
                    window.Close();
                    return true;
                }, CancellationToken.None).GetAwaiter().GetResult();
            }
        }
    }
}
