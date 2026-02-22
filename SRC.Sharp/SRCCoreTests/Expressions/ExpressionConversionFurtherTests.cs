using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Expressions;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression の IsNumeric/IsDefined/NumConvert など変換関連のさらなるユニットテスト
    /// </summary>
    [TestClass]
    public class ExpressionConversionFurtherTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // IsNumeric 関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsNumeric_NumberString_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("IsNumeric(\"42\")"));
        }

        [TestMethod]
        public void IsNumeric_NonNumberString_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("IsNumeric(\"abc\")"));
        }

        [TestMethod]
        public void IsNumeric_FloatString_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("IsNumeric(\"3.14\")"));
        }

        [TestMethod]
        public void IsNumeric_EmptyString_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("IsNumeric(\"\")"));
        }

        [TestMethod]
        public void IsNumeric_NegativeNumberString_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("IsNumeric(\"-100\")"));
        }

        [TestMethod]
        public void IsNumeric_SpacesOnly_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("IsNumeric(\"   \")"));
        }

        [TestMethod]
        public void IsNumeric_IntegerVariable_ReturnsOne()
        {
            var exp = Create();
            exp.SetVariableAsDouble("num", 42d);
            Assert.AreEqual(1d, exp.GetValueAsDouble("IsNumeric(num)"));
        }

        [TestMethod]
        public void IsNumeric_NonNumericVariable_ReturnsZero()
        {
            var exp = Create();
            exp.SetVariableAsString("str", "abc");
            Assert.AreEqual(0d, exp.GetValueAsDouble("IsNumeric(str)"));
        }

        // ──────────────────────────────────────────────
        // LCase 関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LCase_UpperCase_ReturnsLowerCase()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("LCase(\"HELLO\")"));
        }

        [TestMethod]
        public void LCase_MixedCase_ReturnsLowerCase()
        {
            var exp = Create();
            Assert.AreEqual("hello world", exp.GetValueAsString("LCase(\"Hello World\")"));
        }

        [TestMethod]
        public void LCase_AlreadyLowerCase_ReturnsSame()
        {
            var exp = Create();
            Assert.AreEqual("abc", exp.GetValueAsString("LCase(\"abc\")"));
        }

        [TestMethod]
        public void LCase_EmptyString_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("LCase(\"\")"));
        }

        // ──────────────────────────────────────────────
        // Asc 関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Asc_A_Returns65()
        {
            var exp = Create();
            Assert.AreEqual(65d, exp.GetValueAsDouble("Asc(\"A\")"));
        }

        [TestMethod]
        public void Asc_a_Returns97()
        {
            var exp = Create();
            Assert.AreEqual(97d, exp.GetValueAsDouble("Asc(\"a\")"));
        }

        [TestMethod]
        public void Asc_0_Returns48()
        {
            var exp = Create();
            Assert.AreEqual(48d, exp.GetValueAsDouble("Asc(\"0\")"));
        }

        // ──────────────────────────────────────────────
        // Chr 関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Chr_65_ReturnsA()
        {
            var exp = Create();
            Assert.AreEqual("A", exp.GetValueAsString("Chr(65)"));
        }

        [TestMethod]
        public void Chr_97_Returnsa()
        {
            var exp = Create();
            Assert.AreEqual("a", exp.GetValueAsString("Chr(97)"));
        }

        [TestMethod]
        public void Chr_32_ReturnsSpace()
        {
            var exp = Create();
            Assert.AreEqual(" ", exp.GetValueAsString("Chr(32)"));
        }

        // ──────────────────────────────────────────────
        // Replace 関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Replace_BasicReplacement_ReturnsReplaced()
        {
            var exp = Create();
            // "hello"の"l"を"X"に置換 → "heXXo"
            Assert.AreEqual("heXXo", exp.GetValueAsString("Replace(\"hello\",\"l\",\"X\")"));
        }

        [TestMethod]
        public void Replace_NotFound_ReturnsSame()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("Replace(\"hello\",\"xyz\",\"ABC\")"));
        }

        [TestMethod]
        public void Replace_EmptyReplacement_RemovesMatches()
        {
            var exp = Create();
            Assert.AreEqual("heo", exp.GetValueAsString("Replace(\"hello\",\"l\",\"\")"));
        }
    }
}
