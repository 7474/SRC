using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression クラスの算術・論理演算の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class ExpressionArithmeticMoreTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // 整数除算 (\)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IntegerDivision_ReturnsFloor()
        {
            var exp = Create();
            // 7 \ 2 = 3
            Assert.AreEqual(3d, exp.GetValueAsDouble("7 \\ 2"));
        }

        [TestMethod]
        public void IntegerDivision_NegativeResult()
        {
            var exp = Create();
            // -7 \ 2 = -3 (切り捨て方向はVB仕様)
            var result = exp.GetValueAsDouble("-7 \\ 2");
            Assert.IsTrue(result == -3d || result == -4d);
        }

        // ──────────────────────────────────────────────
        // 剰余 (Mod)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Mod_BasicUsage_ReturnsRemainder()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("7 Mod 3"));
        }

        [TestMethod]
        public void Mod_EvenDivision_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("6 Mod 3"));
        }

        [TestMethod]
        public void Mod_LargerDivisor_ReturnsOriginal()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("3 Mod 10"));
        }

        // ──────────────────────────────────────────────
        // 冪乗 (^)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Power_Square_ReturnsSquare()
        {
            var exp = Create();
            Assert.AreEqual(9d, exp.GetValueAsDouble("3 ^ 2"), 1e-10);
        }

        [TestMethod]
        public void Power_ZeroExponent_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("5 ^ 0"), 1e-10);
        }

        [TestMethod]
        public void Power_OneExponent_ReturnsSame()
        {
            var exp = Create();
            Assert.AreEqual(7d, exp.GetValueAsDouble("7 ^ 1"), 1e-10);
        }

        [TestMethod]
        public void Power_FractionalExponent_ReturnsRoot()
        {
            var exp = Create();
            // 4 ^ 0.5 = 2
            Assert.AreEqual(2d, exp.GetValueAsDouble("4 ^ 0.5"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // 文字列結合 (&)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StringConcat_TwoStrings_ConcatenatesThem()
        {
            var exp = Create();
            Assert.AreEqual("helloworld", exp.GetValueAsString("\"hello\" & \"world\""));
        }

        [TestMethod]
        public void StringConcat_NumberAndString_ConcatenatesFormats()
        {
            var exp = Create();
            Assert.AreEqual("42abc", exp.GetValueAsString("42 & \"abc\""));
        }

        [TestMethod]
        public void StringConcat_EmptyStrings_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("\"\" & \"\""));
        }

        // ──────────────────────────────────────────────
        // 複雑な算術式
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ComplexArithmetic_MixedOperations()
        {
            var exp = Create();
            // (3 + 4) * 2 = 14
            Assert.AreEqual(14d, exp.GetValueAsDouble("(3 + 4) * 2"));
        }

        [TestMethod]
        public void ComplexArithmetic_NestedParentheses()
        {
            var exp = Create();
            // ((2 + 3) * (4 - 1)) = 15
            Assert.AreEqual(15d, exp.GetValueAsDouble("((2 + 3) * (4 - 1))"));
        }

        [TestMethod]
        public void ComplexArithmetic_DivisionAndMultiplication()
        {
            var exp = Create();
            // 10 / 2 + 3 * 2 = 5 + 6 = 11
            Assert.AreEqual(11d, exp.GetValueAsDouble("10 / 2 + 3 * 2"));
        }

        // ──────────────────────────────────────────────
        // 比較演算子
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GreaterThan_True_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("5 > 3"));
        }

        [TestMethod]
        public void GreaterThan_False_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("3 > 5"));
        }

        [TestMethod]
        public void LessThanOrEqual_Equal_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("5 <= 5"));
        }

        [TestMethod]
        public void GreaterThanOrEqual_True_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("5 >= 5"));
        }

        [TestMethod]
        public void NotEqual_True_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("5 <> 3"));
        }

        [TestMethod]
        public void NotEqual_False_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("5 <> 5"));
        }

        // ──────────────────────────────────────────────
        // 論理演算子
        // ──────────────────────────────────────────────

        [TestMethod]
        public void And_BothTrue_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("(1 = 1) And (2 = 2)"));
        }

        [TestMethod]
        public void And_OneFalse_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("(1 = 1) And (2 = 3)"));
        }

        [TestMethod]
        public void Or_OneTrue_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("(1 = 1) Or (2 = 3)"));
        }

        [TestMethod]
        public void Or_BothFalse_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("(1 = 2) Or (3 = 4)"));
        }

        [TestMethod]
        public void Not_True_ReturnsFalse()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Not (1 = 1)"));
        }

        [TestMethod]
        public void Not_False_ReturnsTrue()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("Not (1 = 2)"));
        }
    }
}
