using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// HotPoint 構造体のユニットテスト
    /// </summary>
    [TestClass]
    public class HotPointTests
    {
        // ──────────────────────────────────────────────
        // フィールドの設定・読み取り
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Fields_CanBeSetAndRead()
        {
            var hp = new HotPoint
            {
                Name = "攻撃ボタン",
                Left = 10,
                Top = 20,
                Width = 100,
                Height = 50,
                Caption = "攻撃"
            };

            Assert.AreEqual("攻撃ボタン", hp.Name);
            Assert.AreEqual(10, hp.Left);
            Assert.AreEqual(20, hp.Top);
            Assert.AreEqual(100, hp.Width);
            Assert.AreEqual(50, hp.Height);
            Assert.AreEqual("攻撃", hp.Caption);
        }

        [TestMethod]
        public void DefaultStruct_HasDefaultValues()
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
        // ToString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ReturnsFormattedString()
        {
            var hp = new HotPoint
            {
                Name = "ボタン1",
                Left = 5,
                Top = 10,
                Width = 80,
                Height = 30,
                Caption = "OK"
            };

            var result = hp.ToString();

            Assert.AreEqual("ボタン1(5,10,80,30): OK", result);
        }

        [TestMethod]
        public void ToString_WithZeroCoordinates_ReturnsFormattedString()
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
        // 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Fields_CanBeModifiedIndividually()
        {
            var hp = new HotPoint
            {
                Name = "btn",
                Left = 1,
                Top = 2,
                Width = 3,
                Height = 4,
                Caption = "A"
            };
            hp.Left = 10;
            hp.Caption = "B";

            Assert.AreEqual(10, hp.Left);
            Assert.AreEqual("B", hp.Caption);
            // 他のフィールドは変わらない
            Assert.AreEqual(2, hp.Top);
            Assert.AreEqual(3, hp.Width);
            Assert.AreEqual(4, hp.Height);
        }

        [TestMethod]
        public void Name_CanBeSetToEmptyString()
        {
            var hp = new HotPoint { Name = "" };
            Assert.AreEqual("", hp.Name);
        }

        [TestMethod]
        public void ToString_WithNegativeCoordinates_ReturnsFormattedString()
        {
            var hp = new HotPoint
            {
                Name = "neg",
                Left = -5,
                Top = -10,
                Width = 100,
                Height = 50,
                Caption = "test"
            };
            var result = hp.ToString();
            Assert.AreEqual("neg(-5,-10,100,50): test", result);
        }
    }
}
