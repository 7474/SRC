using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// BattleConfigData クラスの追加ユニットテスト（BattleConfigDataMoreTests3）
    /// </summary>
    [TestClass]
    public class BattleConfigDataMoreTests3
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // Name フィールド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { Name = "ダメージ計算" };
            Assert.AreEqual("ダメージ計算", cd.Name);
        }

        [TestMethod]
        public void Name_EmptyString_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { Name = "" };
            Assert.AreEqual("", cd.Name);
        }

        // ──────────────────────────────────────────────
        // ConfigCalc フィールド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ConfigCalc_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { ConfigCalc = "100 * 2" };
            Assert.AreEqual("100 * 2", cd.ConfigCalc);
        }

        // ──────────────────────────────────────────────
        // Calculate の追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Calculate_SimpleAddition_ReturnsSum()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { ConfigCalc = "100 + 50" };
            Assert.AreEqual(150d, cd.Calculate(), 1e-10);
        }

        [TestMethod]
        public void Calculate_LargeNumber_ReturnsCorrectValue()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { ConfigCalc = "9999" };
            Assert.AreEqual(9999d, cd.Calculate(), 1e-10);
        }

        [TestMethod]
        public void Calculate_FloatDivision_ReturnsDecimalResult()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { ConfigCalc = "1 / 4" };
            Assert.AreEqual(0.25d, cd.Calculate(), 1e-10);
        }

        [TestMethod]
        public void Calculate_MaxMinExpression_ReturnsMax()
        {
            var src = CreateSRC();
            // Max(5, 10) は SRC の組み込み関数
            var cd = new BattleConfigData(src) { ConfigCalc = "Max(5, 10)" };
            Assert.AreEqual(10d, cd.Calculate(), 1e-10);
        }

        [TestMethod]
        public void Calculate_MinExpression_ReturnsMin()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { ConfigCalc = "Min(5, 10)" };
            Assert.AreEqual(5d, cd.Calculate(), 1e-10);
        }

        [TestMethod]
        public void Calculate_ConstantOne_ReturnsOne()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { ConfigCalc = "1" };
            Assert.AreEqual(1d, cd.Calculate(), 1e-10);
        }

        [TestMethod]
        public void Calculate_SubtractionToNegative_ReturnsNegative()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { ConfigCalc = "3 - 8" };
            Assert.AreEqual(-5d, cd.Calculate(), 1e-10);
        }
    }
}
