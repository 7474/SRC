using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression の算術演算の追加ユニットテスト（ExpressionArithmeticMoreTests3）
    /// </summary>
    [TestClass]
    public class ExpressionArithmeticMoreTests3
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // 加算
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Add_ZeroPlusZero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0, exp.GetValueAsLong("0 + 0"));
        }

        [TestMethod]
        public void Add_NegativeNumbers_ReturnsNegativeSum()
        {
            var exp = Create();
            Assert.AreEqual(-7, exp.GetValueAsLong("-3 + -4"));
        }

        [TestMethod]
        public void Add_LargeNumbers_ReturnsCorrectSum()
        {
            var exp = Create();
            Assert.AreEqual(10000, exp.GetValueAsLong("9999 + 1"));
        }

        [TestMethod]
        public void Add_ThreeTerms_ReturnsCorrectSum()
        {
            var exp = Create();
            Assert.AreEqual(15, exp.GetValueAsLong("5 + 5 + 5"));
        }

        // ──────────────────────────────────────────────
        // 減算
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Subtract_SameNumbers_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0, exp.GetValueAsLong("7 - 7"));
        }

        [TestMethod]
        public void Subtract_ResultNegative_ReturnsNegativeValue()
        {
            var exp = Create();
            Assert.AreEqual(-3, exp.GetValueAsLong("2 - 5"));
        }

        [TestMethod]
        public void Subtract_ThreeTerms_ReturnsCorrectResult()
        {
            var exp = Create();
            Assert.AreEqual(1, exp.GetValueAsLong("10 - 5 - 4"));
        }

        // ──────────────────────────────────────────────
        // 乗算
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Multiply_ByZero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0, exp.GetValueAsLong("5 * 0"));
        }

        [TestMethod]
        public void Multiply_NegativeByPositive_ReturnsNegative()
        {
            var exp = Create();
            Assert.AreEqual(-12, exp.GetValueAsLong("-3 * 4"));
        }

        [TestMethod]
        public void Multiply_NegativeByNegative_ReturnsPositive()
        {
            var exp = Create();
            Assert.AreEqual(6, exp.GetValueAsLong("-2 * -3"));
        }

        // ──────────────────────────────────────────────
        // 除算
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Divide_ExactDivision_ReturnsInteger()
        {
            var exp = Create();
            Assert.AreEqual(4d, exp.GetValueAsDouble("12 / 3"));
        }

        [TestMethod]
        public void Divide_FractionalResult_ReturnsDouble()
        {
            var exp = Create();
            Assert.AreEqual(2.5d, exp.GetValueAsDouble("5 / 2"));
        }

        [TestMethod]
        public void Divide_NegativeDividend_ReturnsNegativeResult()
        {
            var exp = Create();
            Assert.AreEqual(-5d, exp.GetValueAsDouble("-10 / 2"));
        }

        // ──────────────────────────────────────────────
        // 複合式
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Complex_AddAndMultiply_RespectsOperatorPrecedence()
        {
            var exp = Create();
            // 2 + 3 * 4 = 14 (乗算が先)
            Assert.AreEqual(14, exp.GetValueAsLong("2 + 3 * 4"));
        }

        [TestMethod]
        public void Complex_ParenthesesOverridePrecedence()
        {
            var exp = Create();
            // (2 + 3) * 4 = 20
            Assert.AreEqual(20, exp.GetValueAsLong("(2 + 3) * 4"));
        }

        [TestMethod]
        public void Complex_NestedParentheses_ComputesCorrectly()
        {
            var exp = Create();
            // ((2 + 3) * (4 - 1)) = 15
            Assert.AreEqual(15, exp.GetValueAsLong("(2 + 3) * (4 - 1)"));
        }

        [TestMethod]
        public void GetValueAsLong_DecimalResult_TruncatesToInt()
        {
            var exp = Create();
            // 7 / 2 = 3.5 → GetValueAsLong → 3
            Assert.AreEqual(3, exp.GetValueAsLong("7 / 2"));
        }
    }
}
