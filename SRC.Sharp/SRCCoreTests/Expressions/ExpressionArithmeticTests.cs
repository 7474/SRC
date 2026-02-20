using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression の演算子テスト（算術・比較・論理・文字列連結）
    /// </summary>
    [TestClass]
    public class ExpressionArithmeticTests
    {
        private Expression Create()
        {
            var src = new SRC
            {
                GUI = new MockGUI(),
            };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // 算術演算子
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Add_TwoIntegers_ReturnsSum()
        {
            var exp = Create();
            Assert.AreEqual(7d, exp.GetValueAsDouble("3 + 4"));
        }

        [TestMethod]
        public void Subtract_TwoIntegers_ReturnsDifference()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("5 - 4"));
        }

        [TestMethod]
        public void Multiply_TwoIntegers_ReturnsProduct()
        {
            var exp = Create();
            Assert.AreEqual(12d, exp.GetValueAsDouble("3 * 4"));
        }

        [TestMethod]
        public void Divide_TwoIntegers_ReturnsQuotient()
        {
            var exp = Create();
            Assert.AreEqual(2.5d, exp.GetValueAsDouble("5 / 2"));
        }

        [TestMethod]
        public void IntDivide_TwoIntegers_ReturnsTruncatedQuotient()
        {
            var exp = Create();
            Assert.AreEqual(2d, exp.GetValueAsDouble("5 \\ 2"));
        }

        [TestMethod]
        public void Exponent_TwoIntegers_ReturnsPower()
        {
            var exp = Create();
            Assert.AreEqual(8d, exp.GetValueAsDouble("2 ^ 3"));
        }

        [TestMethod]
        public void Modulo_TwoIntegers_ReturnsRemainder()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("7 Mod 3"));
        }

        [TestMethod]
        public void Modulo_EvenDivision_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("6 Mod 3"));
        }

        [TestMethod]
        public void UnaryMinus_NegatesValue()
        {
            var exp = Create();
            Assert.AreEqual(-5d, exp.GetValueAsDouble("-5"));
        }

        [TestMethod]
        public void ComplexExpression_OrderOfOperations()
        {
            var exp = Create();
            // 2 + 3 * 4 = 14 (掛け算優先)
            Assert.AreEqual(14d, exp.GetValueAsDouble("2 + 3 * 4"));
        }

        [TestMethod]
        public void Parentheses_ChangePrecedence()
        {
            var exp = Create();
            // (2 + 3) * 4 = 20
            Assert.AreEqual(20d, exp.GetValueAsDouble("( 2 + 3 ) * 4"));
        }

        // ──────────────────────────────────────────────
        // 比較演算子
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Equal_SameValues_ReturnsTrue()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("3 = 3"));
        }

        [TestMethod]
        public void Equal_DifferentValues_ReturnsFalse()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("3 = 4"));
        }

        [TestMethod]
        public void NotEqual_DifferentValues_ReturnsTrue()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("3 <> 4"));
        }

        [TestMethod]
        public void LessThan_WhenLess_ReturnsTrue()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("3 < 4"));
        }

        [TestMethod]
        public void LessThan_WhenGreater_ReturnsFalse()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("5 < 4"));
        }

        [TestMethod]
        public void GreaterThan_WhenGreater_ReturnsTrue()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("5 > 4"));
        }

        [TestMethod]
        public void LessThanOrEqual_WhenEqual_ReturnsTrue()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("4 <= 4"));
        }

        [TestMethod]
        public void GreaterThanOrEqual_WhenGreater_ReturnsTrue()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("5 >= 4"));
        }

        // ──────────────────────────────────────────────
        // 論理演算子
        // ──────────────────────────────────────────────

        [TestMethod]
        public void And_BothTrue_ReturnsTrue()
        {
            var exp = Create();
            // 1 = True, 0 = False の規約で And
            var result = exp.GetValueAsDouble("1 And 1");
            Assert.AreEqual(1d, result);
        }

        [TestMethod]
        public void And_OneFalse_ReturnsFalse()
        {
            var exp = Create();
            var result = exp.GetValueAsDouble("1 And 0");
            Assert.AreEqual(0d, result);
        }

        [TestMethod]
        public void Or_OneFalse_ReturnsTrue()
        {
            var exp = Create();
            var result = exp.GetValueAsDouble("0 Or 1");
            Assert.AreEqual(1d, result);
        }

        [TestMethod]
        public void Not_True_ReturnsFalse()
        {
            var exp = Create();
            var result = exp.GetValueAsDouble("Not 1");
            Assert.AreEqual(0d, result);
        }

        [TestMethod]
        public void Not_False_ReturnsTrue()
        {
            var exp = Create();
            var result = exp.GetValueAsDouble("Not 0");
            Assert.AreEqual(1d, result);
        }

        // ──────────────────────────────────────────────
        // 文字列結合演算子
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Concatenate_TwoStrings_ReturnsJoined()
        {
            var exp = Create();
            Assert.AreEqual("こんにちは世界", exp.GetValueAsString("\"こんにちは\" & \"世界\""));
        }

        [TestMethod]
        public void Concatenate_StringAndNumber_ReturnsJoined()
        {
            var exp = Create();
            Assert.AreEqual("値:42", exp.GetValueAsString("\"値:\" & 42"));
        }

        // ──────────────────────────────────────────────
        // 変数を使った式
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Variable_InArithmetic_ComputesCorrectly()
        {
            var exp = Create();
            exp.SetVariableAsDouble("x", 10d);
            exp.SetVariableAsDouble("y", 3d);
            Assert.AreEqual(13d, exp.GetValueAsDouble("x + y"));
            Assert.AreEqual(7d, exp.GetValueAsDouble("x - y"));
            Assert.AreEqual(30d, exp.GetValueAsDouble("x * y"));
        }
    }
}
