using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression の演算・比較・論理演算の追加テスト（エッジケース）
    /// </summary>
    [TestClass]
    public class ExpressionAdvancedTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // 算術演算 (追加ケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Add_IntegerAndFloat_ReturnsSum()
        {
            var exp = Create();
            Assert.AreEqual(3.5d, exp.GetValueAsDouble("1 + 2.5"));
        }

        [TestMethod]
        public void Subtract_ResultNegative_ReturnsNegative()
        {
            var exp = Create();
            Assert.AreEqual(-3d, exp.GetValueAsDouble("2 - 5"));
        }

        [TestMethod]
        public void Multiply_ByZero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("999 * 0"));
        }

        [TestMethod]
        public void Divide_IntegerDivision_ReturnsFloat()
        {
            var exp = Create();
            Assert.AreEqual(0.5d, exp.GetValueAsDouble("1 / 2"));
        }

        [TestMethod]
        public void Exponent_NegativeBase_ReturnsCorrect()
        {
            var exp = Create();
            Assert.AreEqual(-8d, exp.GetValueAsDouble("-2 ^ 3"));
        }

        [TestMethod]
        public void Modulo_LargeDivisor_ReturnsOriginal()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("5 Mod 100"));
        }

        [TestMethod]
        public void Modulo_DivisorEqualsNumber_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("10 Mod 10"));
        }

        [TestMethod]
        public void IntDivide_ResultZero_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("3 \\ 10"));
        }

        // ──────────────────────────────────────────────
        // 比較演算子
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Compare_LessThan_True_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("3 < 5"));
        }

        [TestMethod]
        public void Compare_LessThan_False_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("5 < 3"));
        }

        [TestMethod]
        public void Compare_GreaterThan_True_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("10 > 5"));
        }

        [TestMethod]
        public void Compare_GreaterThan_False_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("5 > 10"));
        }

        [TestMethod]
        public void Compare_Equal_True_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("5 = 5"));
        }

        [TestMethod]
        public void Compare_Equal_False_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("5 = 6"));
        }

        [TestMethod]
        public void Compare_NotEqual_True_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("5 <> 6"));
        }

        [TestMethod]
        public void Compare_NotEqual_False_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("5 <> 5"));
        }

        [TestMethod]
        public void Compare_LessOrEqual_Equal_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("5 <= 5"));
        }

        [TestMethod]
        public void Compare_LessOrEqual_Less_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("4 <= 5"));
        }

        [TestMethod]
        public void Compare_LessOrEqual_Greater_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("6 <= 5"));
        }

        [TestMethod]
        public void Compare_GreaterOrEqual_Equal_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("5 >= 5"));
        }

        [TestMethod]
        public void Compare_GreaterOrEqual_Greater_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("6 >= 5"));
        }

        [TestMethod]
        public void Compare_GreaterOrEqual_Less_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("4 >= 5"));
        }

        // ──────────────────────────────────────────────
        // 論理演算子
        // ──────────────────────────────────────────────

        [TestMethod]
        public void And_BothTrue_ReturnsTrue()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("1 And 1"));
        }

        [TestMethod]
        public void And_BothFalse_ReturnsFalse()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("0 And 0"));
        }

        [TestMethod]
        public void And_FirstFalse_ReturnsFalse()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("0 And 1"));
        }

        [TestMethod]
        public void Or_BothTrue_ReturnsTrue()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("1 Or 1"));
        }

        [TestMethod]
        public void Or_BothFalse_ReturnsFalse()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("0 Or 0"));
        }

        [TestMethod]
        public void Or_FirstFalse_SecondTrue_ReturnsTrue()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("0 Or 1"));
        }

        [TestMethod]
        public void Not_Zero_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("Not 0"));
        }

        [TestMethod]
        public void Not_One_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Not 1"));
        }

        [TestMethod]
        public void Not_LargeNumber_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Not 999"));
        }

        // ──────────────────────────────────────────────
        // 文字列連結演算子
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Concat_TwoStrings_ReturnsJoined()
        {
            var exp = Create();
            Assert.AreEqual("helloworld", exp.GetValueAsString("\"hello\" & \"world\""));
        }

        [TestMethod]
        public void Concat_StringAndNumber_ReturnsJoined()
        {
            var exp = Create();
            Assert.AreEqual("count:5", exp.GetValueAsString("\"count:\" & 5"));
        }

        [TestMethod]
        public void Concat_TwoEmptyStrings_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("\"\" & \"\""));
        }

        [TestMethod]
        public void Concat_JapaneseStrings_ReturnsJoined()
        {
            var exp = Create();
            Assert.AreEqual("こんにちは世界", exp.GetValueAsString("\"こんにちは\" & \"世界\""));
        }

        // ──────────────────────────────────────────────
        // 複雑な式
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Complex_NestedArithmetic_ComputesCorrectly()
        {
            var exp = Create();
            Assert.AreEqual(14d, exp.GetValueAsDouble("2 + 3 * 4"));
        }

        [TestMethod]
        public void Complex_ParenthesesOverrideOrder_ComputesCorrectly()
        {
            var exp = Create();
            Assert.AreEqual(20d, exp.GetValueAsDouble("(2 + 3) * 4"));
        }

        [TestMethod]
        public void Complex_VariableInFormula_ComputesCorrectly()
        {
            var exp = Create();
            exp.SetVariableAsDouble("hp", 100d);
            exp.SetVariableAsDouble("maxhp", 300d);
            Assert.IsTrue(exp.GetValueAsDouble("hp * 100 / maxhp") > 33d);
        }

        [TestMethod]
        public void Complex_ComparisonWithArithmetic()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("2 + 3 > 4"));
        }

        [TestMethod]
        public void Complex_ChainedComparisons()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("(1 < 2) And (3 > 2)"));
        }

        // ──────────────────────────────────────────────
        // 変数 (追加ケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Variable_MultipleVariables_IndependentValues()
        {
            var exp = Create();
            exp.SetVariableAsDouble("x", 10d);
            exp.SetVariableAsDouble("y", 20d);
            exp.SetVariableAsDouble("z", 30d);

            Assert.AreEqual(10d, exp.GetValueAsDouble("x"));
            Assert.AreEqual(20d, exp.GetValueAsDouble("y"));
            Assert.AreEqual(30d, exp.GetValueAsDouble("z"));
        }

        [TestMethod]
        public void Variable_OverwriteValue_ReturnsNewValue()
        {
            var exp = Create();
            exp.SetVariableAsDouble("x", 5d);
            exp.SetVariableAsDouble("x", 99d);
            Assert.AreEqual(99d, exp.GetValueAsDouble("x"));
        }

        [TestMethod]
        public void Variable_StringType_ReturnsString()
        {
            var exp = Create();
            exp.SetVariableAsString("name", "テスト");
            Assert.AreEqual("テスト", exp.GetValueAsString("name"));
        }

        [TestMethod]
        public void Variable_NumericTypeAsString_ReturnsFormattedString()
        {
            var exp = Create();
            exp.SetVariableAsDouble("score", 1234d);
            Assert.AreEqual("1234", exp.GetValueAsString("score"));
        }

        [TestMethod]
        public void Variable_ArrayIndexed_AccessByIndex()
        {
            var exp = Create();
            exp.SetVariableAsDouble("items[1]", 100d);
            exp.SetVariableAsDouble("items[2]", 200d);
            exp.SetVariableAsDouble("items[3]", 300d);

            Assert.AreEqual(100d, exp.GetValueAsDouble("items[1]"));
            Assert.AreEqual(200d, exp.GetValueAsDouble("items[2]"));
            Assert.AreEqual(300d, exp.GetValueAsDouble("items[3]"));
        }

        [TestMethod]
        public void Variable_UndefinedArray_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("undefined[99]"));
        }

        // ──────────────────────────────────────────────
        // 単項マイナス
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnaryMinus_PositiveNumber_ReturnsNegative()
        {
            var exp = Create();
            Assert.AreEqual(-5d, exp.GetValueAsDouble("-5"));
        }

        [TestMethod]
        public void UnaryMinus_InExpression_ComputesCorrectly()
        {
            var exp = Create();
            Assert.AreEqual(-7d, exp.GetValueAsDouble("3 + (-10)"));
        }
    }
}
