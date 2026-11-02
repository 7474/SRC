using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SRCSharpAvalonia.Tests
{
    /// <summary>
    /// メニューやダイアログの文字を OS の文字（大きさ）に合わせる（App.ApplyTheme）。
    /// </summary>
    [TestClass]
    public class ThemeTests
    {
        [TestMethod]
        public void MenuAndDialogTextUseSystemFontSize()
        {
            using (var session = HeadlessUnitTestSession.StartNew(typeof(TestApp)))
            {
                session.Dispatch(() =>
                {
                    var window = new MainWindow();
                    window.Show();
                    Dispatcher.UIThread.RunJobs();

                    var items = window.GetLogicalDescendants().OfType<MenuItem>().ToList();
                    Assert.IsTrue(items.Count > 0);
                    foreach (var item in items)
                    {
                        Assert.AreEqual(TestApp.TestFontSize, item.FontSize, item.Header?.ToString());
                    }

                    var settings = new SettingsWindow(new AppSettings(), System.IO.Path.GetTempPath());
                    settings.Show();
                    Dispatcher.UIThread.RunJobs();
                    Assert.AreEqual(TestApp.TestFontSize, settings.FontSize);
                    // タブの見出し・中身（Fluent の既定では見出しの 24 ピクセルを中身も受け継ぐ）
                    var tab = settings.GetLogicalDescendants().OfType<TabItem>().First();
                    Assert.AreEqual(TestApp.TestFontSize, tab.FontSize);
                    var check = settings.GetLogicalDescendants().OfType<CheckBox>().First();
                    Assert.AreEqual(TestApp.TestFontSize, check.FontSize);
                    settings.Close();
                    window.Close();
                }, System.Threading.CancellationToken.None).Wait();
            }
        }

        [TestMethod]
        public void Detect_ReadsWindowsMenuFont()
        {
            var font = SystemUiFont.Detect();
            if (!System.OperatingSystem.IsWindows())
            {
                Assert.Inconclusive("Windows のみ");
            }
            Assert.IsNotNull(font);
            Assert.IsFalse(string.IsNullOrEmpty(font.FamilyName));
            Assert.IsTrue(font.Size >= 8 && font.Size <= 48, font.Size.ToString());
        }
    }
}
