using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression 経由で呼び出す Format 関数の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class FormatFunctionTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // Format 関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Format_ZeroPaddedInteger_ReturnsZeroPadded()
        {
            var exp = Create();
            Assert.AreEqual("007", exp.GetValueAsString("Format(7,\"000\")"));
        }

        [TestMethod]
        public void Format_IntegerNoFormat_ReturnsDecimal()
        {
            var exp = Create();
            Assert.AreEqual("42", exp.GetValueAsString("Format(42,\"0\")"));
        }

        [TestMethod]
        public void Format_TwoDecimalPlaces_ReturnsFormatted()
        {
            var exp = Create();
            Assert.AreEqual("3.14", exp.GetValueAsString("Format(3.14,\"0.00\")"));
        }

        [TestMethod]
        public void Format_ZeroValue_Returns0()
        {
            var exp = Create();
            Assert.AreEqual("0", exp.GetValueAsString("Format(0,\"0\")"));
        }

        [TestMethod]
        public void Format_NegativeInteger_ReturnsWithSign()
        {
            var exp = Create();
            Assert.AreEqual("-05", exp.GetValueAsString("Format(-5,\"00\")"));
        }

        [TestMethod]
        public void Format_LargeNumber_NoOverflow()
        {
            var exp = Create();
            var result = exp.GetValueAsString("Format(1000000,\"0\")");
            Assert.AreEqual("1000000", result);
        }

        [TestMethod]
        public void Format_AsNumericResult_ReturnsParsedValue()
        {
            var exp = Create();
            // Format の結果を数値として評価
            Assert.AreEqual(7d, exp.GetValueAsDouble("Format(7,\"000\")"));
        }

        // ──────────────────────────────────────────────
        // Wide 関数 (全角変換)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Wide_AsciiDigits_ReturnsFullWidth()
        {
            var exp = Create();
            // '1' → '１'
            Assert.AreEqual("１２３", exp.GetValueAsString("Wide(\"123\")"));
        }

        [TestMethod]
        public void Wide_EmptyString_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("Wide(\"\")"));
        }

        // ──────────────────────────────────────────────
        // Trim 関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Trim_RemovesLeadingSpaces()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("Trim(\"   hello\")"));
        }

        [TestMethod]
        public void Trim_RemovesTrailingSpaces()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("Trim(\"hello   \")"));
        }

        [TestMethod]
        public void Trim_EmptyString_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("Trim(\"\")"));
        }

        [TestMethod]
        public void Trim_OnlySpaces_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("Trim(\"     \")"));
        }

        // ──────────────────────────────────────────────
        // LCase 関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LCase_UpperCase_ReturnsLower()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("LCase(\"HELLO\")"));
        }

        [TestMethod]
        public void LCase_MixedCase_ReturnsLower()
        {
            var exp = Create();
            Assert.AreEqual("hello world", exp.GetValueAsString("LCase(\"Hello World\")"));
        }

        [TestMethod]
        public void LCase_AlreadyLower_ReturnsSame()
        {
            var exp = Create();
            Assert.AreEqual("abc", exp.GetValueAsString("LCase(\"abc\")"));
        }
    }
}
