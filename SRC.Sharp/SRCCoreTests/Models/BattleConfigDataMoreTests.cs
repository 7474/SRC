using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// BattleConfigData クラスの追加ユニットテスト
    /// </summary>
    [TestClass]
    public class BattleConfigDataMoreTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // Calculate の追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Calculate_MultiplicationExpression_ReturnsProduct()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { ConfigCalc = "3 * 7" };
            Assert.AreEqual(21d, cd.Calculate(), 1e-10);
        }

        [TestMethod]
        public void Calculate_DivisionExpression_ReturnsQuotient()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { ConfigCalc = "10 / 2" };
            Assert.AreEqual(5d, cd.Calculate(), 1e-10);
        }

        [TestMethod]
        public void Calculate_Zero_ReturnsZero()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { ConfigCalc = "0" };
            Assert.AreEqual(0d, cd.Calculate(), 1e-10);
        }

        [TestMethod]
        public void Calculate_NegativeResult_ReturnsNegative()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { ConfigCalc = "5 - 10" };
            Assert.AreEqual(-5d, cd.Calculate(), 1e-10);
        }

        [TestMethod]
        public void Calculate_ComplexExpression_ReturnsCorrect()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { ConfigCalc = "(5 + 3) * 2" };
            Assert.AreEqual(16d, cd.Calculate(), 1e-10);
        }

        [TestMethod]
        public void Calculate_IsConfigFalseAfterCalculate()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { ConfigCalc = "100" };
            cd.Calculate();
            Assert.IsFalse(src.Event.BCVariable.IsConfig);
        }

        [TestMethod]
        public void Calculate_IsConfigFalseAfterException()
        {
            var src = CreateSRC();
            // 計算中にエラーが起きてもIsConfigはfalseに戻る
            var cd = new BattleConfigData(src) { ConfigCalc = "invalid_expr" };
            try { cd.Calculate(); } catch { }
            Assert.IsFalse(src.Event.BCVariable.IsConfig);
        }

        // ──────────────────────────────────────────────
        // Name プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { Name = "ダメージ計算" };
            Assert.AreEqual("ダメージ計算", cd.Name);
        }

        [TestMethod]
        public void Name_EmptyString_IsAllowed()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { Name = "" };
            Assert.AreEqual("", cd.Name);
        }

        // ──────────────────────────────────────────────
        // ConfigCalc プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ConfigCalc_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var cd = new BattleConfigData(src) { ConfigCalc = "ATK - DEF" };
            Assert.AreEqual("ATK - DEF", cd.ConfigCalc);
        }
    }
}
