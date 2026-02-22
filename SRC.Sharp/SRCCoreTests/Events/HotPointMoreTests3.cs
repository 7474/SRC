using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// HotPoint 構造体の追加ユニットテスト (HotPointMoreTests3)
    /// </summary>
    [TestClass]
    public class HotPointMoreTests3
    {
        [TestMethod]
        public void Left_NegativeValue_IsAllowed()
        {
            var hp = new HotPoint { Left = -10 };
            Assert.AreEqual(-10, hp.Left);
        }

        [TestMethod]
        public void Top_NegativeValue_IsAllowed()
        {
            var hp = new HotPoint { Top = -20 };
            Assert.AreEqual(-20, hp.Top);
        }

        [TestMethod]
        public void Width_Zero_IsAllowed()
        {
            var hp = new HotPoint { Width = 0 };
            Assert.AreEqual(0, hp.Width);
        }

        [TestMethod]
        public void Height_Zero_IsAllowed()
        {
            var hp = new HotPoint { Height = 0 };
            Assert.AreEqual(0, hp.Height);
        }

        [TestMethod]
        public void Name_LongString_IsAllowed()
        {
            var longName = new string('あ', 100);
            var hp = new HotPoint { Name = longName };
            Assert.AreEqual(longName, hp.Name);
        }

        [TestMethod]
        public void Caption_EmptyString_IsAllowed()
        {
            var hp = new HotPoint { Caption = "" };
            Assert.AreEqual("", hp.Caption);
        }

        [TestMethod]
        public void ToString_WithZeroWidthHeight_IsFormatted()
        {
            var hp = new HotPoint { Name = "a", Left = 0, Top = 0, Width = 0, Height = 0, Caption = "c" };
            var result = hp.ToString();
            Assert.AreEqual("a(0,0,0,0): c", result);
        }

        [TestMethod]
        public void TwoHotPoints_ModifyOneDoesNotAffectOther()
        {
            var hp1 = new HotPoint { Name = "HP1", Left = 1, Top = 2 };
            var hp2 = hp1; // struct copy
            hp2.Name = "HP2";
            hp2.Left = 99;

            Assert.AreEqual("HP1", hp1.Name);
            Assert.AreEqual(1, hp1.Left);
            Assert.AreEqual("HP2", hp2.Name);
            Assert.AreEqual(99, hp2.Left);
        }

        [TestMethod]
        public void Name_CanBeReassigned()
        {
            var hp = new HotPoint { Name = "初期名" };
            hp.Name = "変更後";
            Assert.AreEqual("変更後", hp.Name);
        }

        [TestMethod]
        public void Caption_CanBeReassigned()
        {
            var hp = new HotPoint { Caption = "旧" };
            hp.Caption = "新";
            Assert.AreEqual("新", hp.Caption);
        }

        [TestMethod]
        public void Width_LargeValue_IsAllowed()
        {
            var hp = new HotPoint { Width = 10000 };
            Assert.AreEqual(10000, hp.Width);
        }

        [TestMethod]
        public void Height_LargeValue_IsAllowed()
        {
            var hp = new HotPoint { Height = 10000 };
            Assert.AreEqual(10000, hp.Height);
        }

        [TestMethod]
        public void ToString_ContainsName()
        {
            var hp = new HotPoint { Name = "testName", Left = 1, Top = 2, Width = 3, Height = 4, Caption = "cap" };
            Assert.IsTrue(hp.ToString().Contains("testName"));
        }

        [TestMethod]
        public void ToString_ContainsCaption()
        {
            var hp = new HotPoint { Name = "n", Left = 0, Top = 0, Width = 0, Height = 0, Caption = "captionText" };
            Assert.IsTrue(hp.ToString().Contains("captionText"));
        }
    }
}
