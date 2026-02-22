using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// HotPoint struct のさらなる追加ユニットテスト
    /// </summary>
    [TestClass]
    public class HotPointFurtherTests
    {
        // ──────────────────────────────────────────────
        // デフォルト値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DefaultValues_AreZeroOrNull()
        {
            var hp = new HotPoint();
            Assert.AreEqual(0, hp.Left);
            Assert.AreEqual(0, hp.Top);
            Assert.AreEqual(0, hp.Width);
            Assert.AreEqual(0, hp.Height);
            Assert.IsNull(hp.Name);
            Assert.IsNull(hp.Caption);
        }

        // ──────────────────────────────────────────────
        // 設定・読み取り
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetAllFields_CanBeRead()
        {
            var hp = new HotPoint
            {
                Name = "TestHotPoint",
                Left = 10,
                Top = 20,
                Width = 100,
                Height = 50,
                Caption = "テストキャプション"
            };

            Assert.AreEqual("TestHotPoint", hp.Name);
            Assert.AreEqual(10, hp.Left);
            Assert.AreEqual(20, hp.Top);
            Assert.AreEqual(100, hp.Width);
            Assert.AreEqual(50, hp.Height);
            Assert.AreEqual("テストキャプション", hp.Caption);
        }

        [TestMethod]
        public void NegativeCoordinates_CanBeSetAndRead()
        {
            var hp = new HotPoint { Left = -5, Top = -10 };
            Assert.AreEqual(-5, hp.Left);
            Assert.AreEqual(-10, hp.Top);
        }

        [TestMethod]
        public void LargeValues_CanBeSetAndRead()
        {
            var hp = new HotPoint { Width = int.MaxValue, Height = int.MaxValue };
            Assert.AreEqual(int.MaxValue, hp.Width);
            Assert.AreEqual(int.MaxValue, hp.Height);
        }

        // ──────────────────────────────────────────────
        // ToString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ReturnsFormattedString()
        {
            var hp = new HotPoint
            {
                Name = "button",
                Left = 10,
                Top = 20,
                Width = 100,
                Height = 50,
                Caption = "クリック"
            };

            var str = hp.ToString();
            Assert.IsTrue(str.Contains("button"));
            Assert.IsTrue(str.Contains("10"));
            Assert.IsTrue(str.Contains("20"));
            Assert.IsTrue(str.Contains("100"));
            Assert.IsTrue(str.Contains("50"));
            Assert.IsTrue(str.Contains("クリック"));
        }

        [TestMethod]
        public void ToString_DefaultValues_FormatsWithZeros()
        {
            var hp = new HotPoint { Name = "test" };
            var str = hp.ToString();
            Assert.IsTrue(str.Contains("test"));
            Assert.IsTrue(str.Contains("0"));
        }

        // ──────────────────────────────────────────────
        // 複数インスタンス
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TwoInstances_AreIndependent()
        {
            var hp1 = new HotPoint { Left = 5, Top = 10 };
            var hp2 = new HotPoint { Left = 15, Top = 25 };

            Assert.AreNotEqual(hp1.Left, hp2.Left);
            Assert.AreNotEqual(hp1.Top, hp2.Top);
        }

        [TestMethod]
        public void ModifyOneInstance_DoesNotAffectOther()
        {
            var hp1 = new HotPoint { Left = 5, Top = 10 };
            var hp2 = hp1; // value copy (struct)
            hp2.Left = 99;

            Assert.AreEqual(5, hp1.Left); // original unchanged
            Assert.AreEqual(99, hp2.Left);
        }
    }
}
