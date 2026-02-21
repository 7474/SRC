using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression の演算子追加テスト（エッジケース）
    /// </summary>
    [TestClass]
    public class ExpressionOperatorMoreTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // 基本算術 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Add_TwoIntegers_ReturnsSum()
        {
            var exp = Create();
            Assert.AreEqual(15d, exp.GetValueAsDouble("10 + 5"));
        }

        [TestMethod]
        public void Subtract_TwoIntegers_ReturnsDiff()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("10 - 5"));
        }

        [TestMethod]
        public void Multiply_TwoIntegers_ReturnsProduct()
        {
            var exp = Create();
            Assert.AreEqual(50d, exp.GetValueAsDouble("10 * 5"));
        }

        [TestMethod]
        public void Divide_TwoIntegers_ReturnsQuotient()
        {
            var exp = Create();
            Assert.AreEqual(2d, exp.GetValueAsDouble("10 / 5"));
        }

        [TestMethod]
        public void IntDivide_TwoIntegers_ReturnsIntQuotient()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("10 \\ 3"));
        }

        [TestMethod]
        public void Mod_TwoIntegers_ReturnsRemainder()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("10 Mod 3"));
        }

        [TestMethod]
        public void Power_ExpoOp_ReturnsResult()
        {
            var exp = Create();
            Assert.AreEqual(8d, exp.GetValueAsDouble("2 ^ 3"));
        }

        [TestMethod]
        public void Add_DecimalNumbers_ReturnsCorrectResult()
        {
            var exp = Create();
            Assert.AreEqual(1.5d, exp.GetValueAsDouble("0.5 + 1"), 1e-10);
        }

        [TestMethod]
        public void Subtract_NegativeResult_ReturnsNegative()
        {
            var exp = Create();
            Assert.AreEqual(-3d, exp.GetValueAsDouble("2 - 5"));
        }

        // ──────────────────────────────────────────────
        // 比較演算子 追加ケース（1d/0d）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Equal_SameNumbers_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("5 = 5"));
        }

        [TestMethod]
        public void Equal_DiffNumbers_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("5 = 6"));
        }

        [TestMethod]
        public void NotEqual_DiffNumbers_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("5 <> 6"));
        }

        [TestMethod]
        public void LessThan_SmallerOnLeft_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("3 < 5"));
        }

        [TestMethod]
        public void LessThan_SameNumbers_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("5 < 5"));
        }

        [TestMethod]
        public void LessOrEqual_SameNumbers_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("5 <= 5"));
        }

        [TestMethod]
        public void GreaterThan_LargerOnLeft_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("7 > 5"));
        }

        [TestMethod]
        public void GreaterOrEqual_SameNumbers_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("5 >= 5"));
        }

        // ──────────────────────────────────────────────
        // 論理演算子 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void And_BothTrue_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("1 And 1"));
        }

        [TestMethod]
        public void And_OneFalse_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("1 And 0"));
        }

        [TestMethod]
        public void Or_OneFalse_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("0 Or 1"));
        }

        [TestMethod]
        public void Or_BothFalse_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("0 Or 0"));
        }

        [TestMethod]
        public void Not_TrueInput_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Not 1 = 1"));
        }

        [TestMethod]
        public void Not_FalseInput_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("Not 1 = 2"));
        }

        // ──────────────────────────────────────────────
        // 文字列連結
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CatOp_TwoStrings_Concatenates()
        {
            var exp = Create();
            Assert.AreEqual("HelloWorld", exp.GetValueAsString("\"Hello\" & \"World\""));
        }

        [TestMethod]
        public void CatOp_StringAndNumber_Concatenates()
        {
            var exp = Create();
            Assert.AreEqual("HP100", exp.GetValueAsString("\"HP\" & 100"));
        }

        // ──────────────────────────────────────────────
        // ネストした演算 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Nested_Parentheses_EvaluatedFirst()
        {
            var exp = Create();
            Assert.AreEqual(14d, exp.GetValueAsDouble("(2 + 5) * 2"));
        }

        [TestMethod]
        public void ChainedAddition_ReturnsCorrectSum()
        {
            var exp = Create();
            Assert.AreEqual(10d, exp.GetValueAsDouble("1 + 2 + 3 + 4"));
        }

        [TestMethod]
        public void ChainedComparison_WithAnd_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("1 < 2 And 2 < 3 And 3 < 4"));
        }

        [TestMethod]
        public void MixedArithmeticAndComparison_EvaluatesCorrectly()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("2 + 3 > 4"));
        }
    }
}
