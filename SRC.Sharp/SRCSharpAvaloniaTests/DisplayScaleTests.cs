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
        [DataRow("100", 1)]
        [DataRow("200", 2)]
        [DataRow("400", 4)]
        [DataRow("150", null)]
        [DataRow("500", null)]
        [DataRow("", null)]
        [DataRow("abc", null)]
        public void ReadDisplayScale_AcceptsSupportedPercentages(string value, int? expected)
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

                    int? chosen = null;
                    window.DisplayScaleChosen += scale => chosen = scale;
                    window.ScaleItems.Single(item => (int)item.Tag == 3).RaiseEvent(new RoutedEventArgs(Avalonia.Controls.MenuItem.ClickEvent));

                    Assert.AreEqual(3, chosen);
                    Assert.AreEqual(3, window.View.DisplayScale);
                    Assert.AreEqual(2160, window.View.Width);
                    CollectionAssert.AreEqual(new[] { false, false, true, false }, window.ScaleItems.Select(item => item.IsChecked).ToArray());
                    window.Close();
                    return true;
                }, CancellationToken.None).GetAwaiter().GetResult();
            }
        }
    }
}
