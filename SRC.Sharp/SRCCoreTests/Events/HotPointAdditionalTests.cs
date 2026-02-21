using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// HotPoint 構造体の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class HotPointMoreTests
    {
        [TestMethod]
        public void Width_LargeValue_CanBeSetAndRead()
        {
            var hp = new HotPoint { Width = 9999 };
            Assert.AreEqual(9999, hp.Width);
        }

        [TestMethod]
        public void Height_LargeValue_CanBeSetAndRead()
        {
            var hp = new HotPoint { Height = 8888 };
            Assert.AreEqual(8888, hp.Height);
        }

        [TestMethod]
        public void Caption_JapaneseString_CanBeSetAndRead()
        {
            var hp = new HotPoint { Caption = "テスト文字" };
            Assert.AreEqual("テスト文字", hp.Caption);
        }

        [TestMethod]
        public void TwoStructs_SameValues_EqualProperties()
        {
            var hp1 = new HotPoint { Name = "test", Left = 10, Top = 20, Width = 30, Height = 40, Caption = "cap" };
            var hp2 = new HotPoint { Name = "test", Left = 10, Top = 20, Width = 30, Height = 40, Caption = "cap" };
            Assert.AreEqual(hp1.Name, hp2.Name);
            Assert.AreEqual(hp1.Left, hp2.Left);
            Assert.AreEqual(hp1.Caption, hp2.Caption);
        }

        [TestMethod]
        public void ToString_LargeValues_FormatsCorrectly()
        {
            var hp = new HotPoint { Name = "big", Left = 1000, Top = 2000, Width = 3000, Height = 4000, Caption = "X" };
            var result = hp.ToString();
            Assert.AreEqual("big(1000,2000,3000,4000): X", result);
        }

        [TestMethod]
        public void Name_LongJapaneseName_CanBeSetAndRead()
        {
            var longName = "非常に長いボタン名テスト";
            var hp = new HotPoint { Name = longName };
            Assert.AreEqual(longName, hp.Name);
        }
    }
}
