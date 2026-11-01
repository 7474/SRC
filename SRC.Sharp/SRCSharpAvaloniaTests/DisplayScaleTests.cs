using Avalonia.Headless;
using Avalonia.Interactivity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using System.Linq;
using System.Threading;

namespace SRCSharpAvalonia.Tests
{
    /// <summary>
    /// 表示倍率（メニューの「表示」で選び、srcs.json の Option/DisplayScale に保存する）。
    /// </summary>
    [TestClass]
    public class DisplayScaleTests
    {
        [TestMethod]
        [DataRow("100", 1.0)]
        [DataRow("150", 1.5)]
        [DataRow("200", 2.0)]
        [DataRow("300", null)]
        [DataRow("125", null)]
        [DataRow("", null)]
        [DataRow("abc", null)]
        public void ReadDisplayScale_AcceptsSupportedPercentages(string value, double? expected)
        {
            var config = new LocalFileConfig();
            config.SetItem(App.DisplayScaleSection, App.DisplayScaleName, value);
            Assert.AreEqual(expected, App.ReadDisplayScale(config));
        }

        [TestMethod]
        public void MenuChoice_ChangesScaleAndNotifies()
        {
            using (var session = HeadlessUnitTestSession.StartNew(typeof(TestApp)))
            {
                session.Dispatch(() =>
                {
                    var window = new MainWindow();
                    window.Show();
                    Assert.AreEqual(1, window.View.DisplayScale);
                    Assert.IsTrue(window.ScaleItems[0].IsChecked);

                    CollectionAssert.AreEqual(new[] { "100%", "150%", "200%" }, window.ScaleItems.Select(item => (string)item.Header).ToArray());
                    double? chosen = null;
                    window.DisplayScaleChosen += scale => chosen = scale;
                    window.ScaleItems.Single(item => (double)item.Tag == 1.5).RaiseEvent(new RoutedEventArgs(Avalonia.Controls.MenuItem.ClickEvent));

                    Assert.AreEqual(1.5, chosen);
                    Assert.AreEqual(1.5, window.View.DisplayScale);
                    Assert.AreEqual(1080, window.View.Width);
                    CollectionAssert.AreEqual(new[] { false, true, false }, window.ScaleItems.Select(item => item.IsChecked).ToArray());
                    window.Close();
                    return true;
                }, CancellationToken.None).GetAwaiter().GetResult();
            }
        }
    }
}
