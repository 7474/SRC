using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// Information.IsNumeric の追加エッジケーステスト
    /// </summary>
    [TestClass]
    public class InformationEdgeCaseTests
    {
        // ──────────────────────────────────────────────
        // 数値文字列テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsNumeric_PositiveDecimal_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("1.23"));
        }

        [TestMethod]
        public void IsNumeric_NegativeDecimal_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("-1.23"));
        }

        [TestMethod]
        public void IsNumeric_LargeNumber_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("1000000"));
        }

        [TestMethod]
        public void IsNumeric_Zero_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("0"));
        }

        [TestMethod]
        public void IsNumeric_NegativeZero_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("-0"));
        }

        [TestMethod]
        public void IsNumeric_LeadingPlus_ReturnsTrue()
        {
            // "+3" は数値として扱われるか？
            // decimal.TryParse では扱われる
            Assert.IsTrue(Information.IsNumeric("+3"));
        }

        // ──────────────────────────────────────────────
        // 非数値文字列テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsNumeric_DoubleDecimalPoint_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric("1.2.3"));
        }

        [TestMethod]
        public void IsNumeric_OnlyMinus_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric("-"));
        }

        [TestMethod]
        public void IsNumeric_OnlyPlus_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric("+"));
        }

        [TestMethod]
        public void IsNumeric_OnlyDecimalPoint_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric("."));
        }

        [TestMethod]
        public void IsNumeric_JapaneseDigits_ReturnsFalse()
        {
            // 全角数字はdecimal.TryParseに通らない（通常）
            Assert.IsFalse(Information.IsNumeric("１２３"));
        }

        [TestMethod]
        public void IsNumeric_MixedAlphaNumeric_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric("abc123"));
        }

        [TestMethod]
        public void IsNumeric_JapaneseWord_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric("攻撃"));
        }

        // ──────────────────────────────────────────────
        // null / 空白 テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsNumeric_SingleTab_ReturnsFalse()
        {
            // タブだけはdecimal.TryParseに通らない
            Assert.IsFalse(Information.IsNumeric("\t"));
        }

        [TestMethod]
        public void IsNumeric_NumberWithTabs_ReturnsTrue()
        {
            // タブは空白として無視される
            Assert.IsTrue(Information.IsNumeric("\t42\t"));
        }

        [TestMethod]
        public void IsNumeric_NumberWithNewlines_ReturnsTrue()
        {
            // 改行も空白として無視される
            Assert.IsTrue(Information.IsNumeric("\n42\n"));
        }

        // ──────────────────────────────────────────────
        // 特殊数値テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsNumeric_VerySmallDecimal_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("0.0001"));
        }

        [TestMethod]
        public void IsNumeric_DecimalWithLeadingZeros_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("00042"));
        }

        [TestMethod]
        public void IsNumeric_NumberObject_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric((object)42));
        }

        [TestMethod]
        public void IsNumeric_DoubleObject_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric((object)3.14));
        }
    }
}
