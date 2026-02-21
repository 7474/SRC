using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// Information クラスの追加テスト（IsNumeric エッジケース）
    /// </summary>
    [TestClass]
    public class InformationAdditionalTests2
    {
        // ──────────────────────────────────────────────
        // IsNumeric 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsNumeric_LargePositiveInt_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("999999999"));
        }

        [TestMethod]
        public void IsNumeric_LargeNegativeInt_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("-999999999"));
        }

        [TestMethod]
        public void IsNumeric_DoubleWithE_ReturnsFalse()
        {
            // SRC の IsNumeric は科学的表記を数値として扱わない
            Assert.IsFalse(Information.IsNumeric("1E5"));
        }

        [TestMethod]
        public void IsNumeric_OnlyDot_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric("."));
        }

        [TestMethod]
        public void IsNumeric_LeadingZero_ReturnsTrue()
        {
            // "007" は数値として扱われる
            Assert.IsTrue(Information.IsNumeric("007"));
        }

        [TestMethod]
        public void IsNumeric_TrailingZero_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("1.0"));
        }

        [TestMethod]
        public void IsNumeric_Whitespace_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric(" "));
        }

        [TestMethod]
        public void IsNumeric_PositiveSign_ReturnsFalse()
        {
            // "+" は数値として扱われない
            Assert.IsFalse(Information.IsNumeric("+"));
        }

        [TestMethod]
        public void IsNumeric_NegativeSign_ReturnsFalse()
        {
            // "-" 単体は数値でない
            Assert.IsFalse(Information.IsNumeric("-"));
        }

        [TestMethod]
        public void IsNumeric_IntegerWithTrailingSpace_ReturnsTrue()
        {
            // IsNumericは空白を除去してから判断するためtrueを返す
            Assert.IsTrue(Information.IsNumeric("42 "));
        }

        [TestMethod]
        public void IsNumeric_IntegerWithLeadingSpace_ReturnsTrue()
        {
            // IsNumericは空白を除去してから判断するためtrueを返す
            Assert.IsTrue(Information.IsNumeric(" 42"));
        }

        [TestMethod]
        public void IsNumeric_JapaneseNumber_ReturnsFalse()
        {
            // 全角数字は数値でない
            Assert.IsFalse(Information.IsNumeric("１２３"));
        }

        [TestMethod]
        public void IsNumeric_Hex_ReturnsFalse()
        {
            // 16進表記は数値でない
            Assert.IsFalse(Information.IsNumeric("0xFF"));
        }

        [TestMethod]
        public void IsNumeric_MultipleDots_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric("1.2.3"));
        }

        [TestMethod]
        public void IsNumeric_NegativeFloat_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("-3.14"));
        }
    }
}
