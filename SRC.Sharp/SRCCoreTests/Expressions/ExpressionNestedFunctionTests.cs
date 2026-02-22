using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// ネストされた関数呼び出しや複合式のユニットテスト
    /// </summary>
    [TestClass]
    public class ExpressionNestedFunctionTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // ネストされた数学関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Abs_OfInt_NegativeDecimal_ReturnsPositiveFloor()
        {
            var exp = Create();
            // Int(-3.7) = -4, Abs(-4) = 4
            Assert.AreEqual(4d, exp.GetValueAsDouble("Abs(Int(-3.7))"));
        }

        [TestMethod]
        public void Int_OfSqr_ReturnsFloorOfRoot()
        {
            var exp = Create();
            // Sqr(10) ≈ 3.162, Int(3.162) = 3
            Assert.AreEqual(3d, exp.GetValueAsDouble("Int(Sqr(10))"));
        }

        [TestMethod]
        public void Max_OfAbsValues_ReturnsLargerAbsolute()
        {
            var exp = Create();
            // Abs(-10) = 10, Abs(5) = 5, Max(10, 5) = 10
            Assert.AreEqual(10d, exp.GetValueAsDouble("Max(Abs(-10),Abs(5))"));
        }

        [TestMethod]
        public void Min_OfAbsValues_ReturnsSmallerAbsolute()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("Min(Abs(-10),Abs(5))"));
        }

        [TestMethod]
        public void Round_OfSqr_RoundsCorrectly()
        {
            var exp = Create();
            // Sqr(2) ≈ 1.414, Round(1.414, 2) = 1.41
            Assert.AreEqual(1.41d, exp.GetValueAsDouble("Round(Sqr(2),2)"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // ネストされた文字列関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Len_OfLeft_ReturnsSubstringLength()
        {
            var exp = Create();
            // Left("hello", 3) = "hel", Len("hel") = 3
            Assert.AreEqual(3d, exp.GetValueAsDouble("Len(Left(\"hello\",3))"));
        }

        [TestMethod]
        public void Left_OfRight_ExtractsMidSection()
        {
            var exp = Create();
            // Right("hello", 4) = "ello", Left("ello", 2) = "el"
            Assert.AreEqual("el", exp.GetValueAsString("Left(Right(\"hello\",4),2)"));
        }

        [TestMethod]
        public void Mid_OfReplace_ProcessesCorrectly()
        {
            var exp = Create();
            // Replace("abcde", "c", "X") = "abXde", Mid("abXde", 2, 3) = "bXd"
            Assert.AreEqual("bXd", exp.GetValueAsString("Mid(Replace(\"abcde\",\"c\",\"X\"),2,3)"));
        }

        // ──────────────────────────────────────────────
        // 数学関数と算術演算の組み合わせ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Abs_OfDifference_ReturnsAbsoluteDifference()
        {
            var exp = Create();
            Assert.AreEqual(7d, exp.GetValueAsDouble("Abs(3 - 10)"));
        }

        [TestMethod]
        public void Int_OfDivision_ReturnsFlooredQuotient()
        {
            var exp = Create();
            // 7 / 3 ≈ 2.333, Int(2.333) = 2
            Assert.AreEqual(2d, exp.GetValueAsDouble("Int(7 / 3)"));
        }

        [TestMethod]
        public void Max_WithArithmetic_ReturnsLarger()
        {
            var exp = Create();
            Assert.AreEqual(12d, exp.GetValueAsDouble("Max(3 + 4, 5 + 7)"));
        }

        // ──────────────────────────────────────────────
        // 変数を含むネスト式
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Abs_OfVariable_ReturnsAbsoluteValue()
        {
            var exp = Create();
            exp.SetVariableAsDouble("x", -42d);
            Assert.AreEqual(42d, exp.GetValueAsDouble("Abs(x)"));
        }

        [TestMethod]
        public void Max_OfVariables_ReturnsLarger()
        {
            var exp = Create();
            exp.SetVariableAsDouble("a", 10d);
            exp.SetVariableAsDouble("b", 20d);
            Assert.AreEqual(20d, exp.GetValueAsDouble("Max(a,b)"));
        }

        [TestMethod]
        public void Len_OfStringVariable_ReturnsLength()
        {
            var exp = Create();
            exp.SetVariableAsString("msg", "テスト");
            Assert.AreEqual(3d, exp.GetValueAsDouble("Len(msg)"));
        }

        // ──────────────────────────────────────────────
        // IIf と関数の組み合わせ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IIf_WithFunctionArgs_EvaluatesCorrectly()
        {
            var exp = Create();
            // Abs(-5)=5 > 3 → true → returns Max(10,20)=20
            Assert.AreEqual(20d, exp.GetValueAsDouble("IIf(Abs(-5) > 3, Max(10,20), Min(10,20))"));
        }

        [TestMethod]
        public void IIf_WithLen_SwitchesOnStringLength()
        {
            var exp = Create();
            Assert.AreEqual("short", exp.GetValueAsString("IIf(Len(\"hi\") < 5, \"short\", \"long\")"));
        }
    }
}
