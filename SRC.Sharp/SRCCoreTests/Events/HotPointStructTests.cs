using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// HotPoint 構造体の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class HotPointStructTests
    {
        // ──────────────────────────────────────────────
        // フィールド設定テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HotPoint_AllFields_CanBeSetAndRead()
        {
            var hp = new HotPoint
            {
                Name = "button1",
                Left = 10,
                Top = 20,
                Width = 100,
                Height = 50,
                Caption = "クリック"
            };

            Assert.AreEqual("button1", hp.Name);
            Assert.AreEqual(10, hp.Left);
            Assert.AreEqual(20, hp.Top);
            Assert.AreEqual(100, hp.Width);
            Assert.AreEqual(50, hp.Height);
            Assert.AreEqual("クリック", hp.Caption);
        }

        [TestMethod]
        public void HotPoint_DefaultValues_AreDefault()
        {
            var hp = new HotPoint();
            Assert.IsNull(hp.Name);
            Assert.AreEqual(0, hp.Left);
            Assert.AreEqual(0, hp.Top);
            Assert.AreEqual(0, hp.Width);
            Assert.AreEqual(0, hp.Height);
            Assert.IsNull(hp.Caption);
        }

        // ──────────────────────────────────────────────
        // ToString テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ReturnsFormattedString()
        {
            var hp = new HotPoint
            {
                Name = "btn",
                Left = 5,
                Top = 10,
                Width = 80,
                Height = 30,
                Caption = "テスト"
            };
            var result = hp.ToString();
            Assert.AreEqual("btn(5,10,80,30): テスト", result);
        }

        [TestMethod]
        public void ToString_WithZeroValues_FormatsCorrectly()
        {
            var hp = new HotPoint
            {
                Name = "zero",
                Left = 0,
                Top = 0,
                Width = 0,
                Height = 0,
                Caption = ""
            };
            var result = hp.ToString();
            Assert.AreEqual("zero(0,0,0,0): ", result);
        }

        // ──────────────────────────────────────────────
        // 値型（struct）の特性テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HotPoint_IsCopiedByValue()
        {
            var hp1 = new HotPoint { Name = "original", Left = 10 };
            var hp2 = hp1;
            hp2.Name = "copy";
            hp2.Left = 20;

            // 元の値が変更されていないことを確認
            Assert.AreEqual("original", hp1.Name);
            Assert.AreEqual(10, hp1.Left);
            Assert.AreEqual("copy", hp2.Name);
            Assert.AreEqual(20, hp2.Left);
        }
    }
}
