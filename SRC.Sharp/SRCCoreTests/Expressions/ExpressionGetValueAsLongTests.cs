using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression.GetValueAsLong のユニットテスト
    /// </summary>
    [TestClass]
    public class ExpressionGetValueAsLongTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // 整数値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetValueAsLong_IntegerLiteral_ReturnsInt()
        {
            var exp = Create();
            Assert.AreEqual(42, exp.GetValueAsLong("42"));
        }

        [TestMethod]
        public void GetValueAsLong_Zero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0, exp.GetValueAsLong("0"));
        }

        [TestMethod]
        public void GetValueAsLong_NegativeInteger_ReturnsNegative()
        {
            var exp = Create();
            Assert.AreEqual(-10, exp.GetValueAsLong("-10"));
        }

        // ──────────────────────────────────────────────
        // 小数値の切り捨て
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetValueAsLong_PositiveDecimal_TruncatesTowardZero()
        {
            var exp = Create();
            // 3.9 → 3 (切り捨て)
            Assert.AreEqual(3, exp.GetValueAsLong("3.9"));
        }

        [TestMethod]
        public void GetValueAsLong_NegativeDecimal_TruncatesTowardZero()
        {
            var exp = Create();
            // -3.9 → -3 (ゼロ方向への切り捨て: (int)double)
            Assert.AreEqual(-3, exp.GetValueAsLong("-3.9"));
        }

        [TestMethod]
        public void GetValueAsLong_DecimalPoint5_TruncatesDown()
        {
            var exp = Create();
            Assert.AreEqual(2, exp.GetValueAsLong("2.5"));
        }

        // ──────────────────────────────────────────────
        // 算術式の結果
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetValueAsLong_Addition_ReturnsIntResult()
        {
            var exp = Create();
            Assert.AreEqual(7, exp.GetValueAsLong("3 + 4"));
        }

        [TestMethod]
        public void GetValueAsLong_Multiplication_ReturnsIntResult()
        {
            var exp = Create();
            Assert.AreEqual(12, exp.GetValueAsLong("3 * 4"));
        }

        [TestMethod]
        public void GetValueAsLong_Division_TruncatesDecimal()
        {
            var exp = Create();
            // 5 / 2 = 2.5 → 2
            Assert.AreEqual(2, exp.GetValueAsLong("5 / 2"));
        }

        // ──────────────────────────────────────────────
        // 変数からの取得
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetValueAsLong_Variable_ReturnsIntValue()
        {
            var exp = Create();
            exp.SetVariableAsDouble("n", 100d);
            Assert.AreEqual(100, exp.GetValueAsLong("n"));
        }

        [TestMethod]
        public void GetValueAsLong_VariableWithDecimal_Truncates()
        {
            var exp = Create();
            exp.SetVariableAsDouble("x", 7.8d);
            Assert.AreEqual(7, exp.GetValueAsLong("x"));
        }

        // ──────────────────────────────────────────────
        // isTerm フラグ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetValueAsLong_IsTerm_ReturnsIntFromTerm()
        {
            var exp = Create();
            Assert.AreEqual(10, exp.GetValueAsLong("10", is_term: true));
        }

        [TestMethod]
        public void GetValueAsLong_IsTerm_WithVariable_ReturnsValue()
        {
            var exp = Create();
            exp.SetVariableAsDouble("myVal", 55d);
            Assert.AreEqual(55, exp.GetValueAsLong("myVal", is_term: true));
        }
    }
}
