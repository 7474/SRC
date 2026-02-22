using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Format 関数、Replace（開始位置指定）、String 関数のエッジケーステスト
    /// </summary>
    [TestClass]
    public class FormatAndReplaceExtendedTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // Format
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Format_IntegerWithPadding_ReturnsZeroPadded()
        {
            var exp = Create();
            Assert.AreEqual("042", exp.GetValueAsString("Format(42,\"000\")"));
        }

        [TestMethod]
        public void Format_DecimalWithFixedPlaces_FormatsCorrectly()
        {
            var exp = Create();
            Assert.AreEqual("3.10", exp.GetValueAsString("Format(3.1,\"0.00\")"));
        }

        [TestMethod]
        public void Format_ZeroWithFormat_ReturnsFormatted()
        {
            var exp = Create();
            Assert.AreEqual("000", exp.GetValueAsString("Format(0,\"000\")"));
        }

        [TestMethod]
        public void Format_LargeNumber_FormatsCorrectly()
        {
            var exp = Create();
            Assert.AreEqual("1234", exp.GetValueAsString("Format(1234,\"0\")"));
        }

        [TestMethod]
        public void Format_NumericReturn_ReturnsNumericValue()
        {
            var exp = Create();
            Assert.AreEqual(42d, exp.GetValueAsDouble("Format(42,\"0\")"));
        }

        // ──────────────────────────────────────────────
        // Replace with start position (4 args)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Replace_WithStartPosition_ReplacesAfterPosition()
        {
            var exp = Create();
            // "ababa" で3文字目以降の"a"を"X"に置換
            // 1-2文字目="ab", 3文字目以降="aba" → "XbX"
            // 結果: "ab" + "XbX" = "abXbX"
            var result = exp.GetValueAsString("Replace(\"ababa\",\"a\",\"X\",3)");
            Assert.AreEqual("abXbX", result);
        }

        [TestMethod]
        public void Replace_WithStartPositionOne_ReplacesAll()
        {
            var exp = Create();
            var result = exp.GetValueAsString("Replace(\"ababa\",\"a\",\"X\",1)");
            Assert.AreEqual("XbXbX", result);
        }

        // ──────────────────────────────────────────────
        // String (文字列繰り返し) エッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void String_SingleRepetition_ReturnsSameString()
        {
            var exp = Create();
            Assert.AreEqual("a", exp.GetValueAsString("String(1,\"a\")"));
        }

        [TestMethod]
        public void String_MultiCharRepeat_RepeatsFullString()
        {
            var exp = Create();
            Assert.AreEqual("ababab", exp.GetValueAsString("String(3,\"ab\")"));
        }

        [TestMethod]
        public void String_NegativeCount_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("String(-1,\"a\")"));
        }

        [TestMethod]
        public void String_JapaneseRepeat_RepeatsCorrectly()
        {
            var exp = Create();
            Assert.AreEqual("ああ", exp.GetValueAsString("String(2,\"あ\")"));
        }

        // ──────────────────────────────────────────────
        // IsNumeric エッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsNumeric_EmptyString_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("IsNumeric(\"\")"));
        }

        [TestMethod]
        public void IsNumeric_DecimalNumber_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("IsNumeric(\"3.14\")"));
        }

        [TestMethod]
        public void IsNumeric_NegativeNumber_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("IsNumeric(\"-5\")"));
        }

        [TestMethod]
        public void IsNumeric_StringReturn_ReturnsStringOneOrZero()
        {
            var exp = Create();
            Assert.AreEqual("1", exp.GetValueAsString("IsNumeric(\"42\")"));
            Assert.AreEqual("0", exp.GetValueAsString("IsNumeric(\"abc\")"));
        }

        // ──────────────────────────────────────────────
        // Asc / Chr 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Chr_ZeroCharCode_ReturnsNullChar()
        {
            var exp = Create();
            var result = exp.GetValueAsString("Chr(0)");
            Assert.AreEqual("\0", result);
        }

        [TestMethod]
        public void Asc_Digit9_Returns57()
        {
            var exp = Create();
            Assert.AreEqual(57d, exp.GetValueAsDouble("Asc(\"9\")"));
        }

        [TestMethod]
        public void Asc_StringReturn_ReturnsFormattedNumber()
        {
            var exp = Create();
            Assert.AreEqual("65", exp.GetValueAsString("Asc(\"A\")"));
        }
    }
}
