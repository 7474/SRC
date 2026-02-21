using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// VB.Information クラスのさらなる追加ユニットテスト
    /// </summary>
    [TestClass]
    public class InformationFurtherTests
    {
        // ──────────────────────────────────────────────
        // IsNumeric - 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsNumeric_PositiveSign_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("+42"));
        }

        [TestMethod]
        public void IsNumeric_NegativeSign_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("-42"));
        }

        [TestMethod]
        public void IsNumeric_DecimalOnly_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric("."));
        }

        [TestMethod]
        public void IsNumeric_JustDecimalPoint_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric("1.2.3"));
        }

        [TestMethod]
        public void IsNumeric_LargeNumber_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("9999999999999999"));
        }

        [TestMethod]
        public void IsNumeric_IntegerWithHyphen_ReturnsFalse()
        {
            // "1-2" is not a valid number
            Assert.IsFalse(Information.IsNumeric("1-2"));
        }

        [TestMethod]
        public void IsNumeric_TabCharacterOnly_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric("\t"));
        }

        [TestMethod]
        public void IsNumeric_AllWhitespace_ReturnsFalse()
        {
            // Whitespace gets stripped but then the string is empty, so false
            Assert.IsFalse(Information.IsNumeric("   "));
        }

        [TestMethod]
        public void IsNumeric_NumberInterspersedWithSpaces_ReturnsTrue()
        {
            // Spaces are stripped, then "42" is numeric
            Assert.IsTrue(Information.IsNumeric(" 4 2 "));
        }

        [TestMethod]
        public void IsNumeric_ZeroFloat_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("0.0"));
        }

        [TestMethod]
        public void IsNumeric_NegativeDecimal_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("-3.14"));
        }

        [TestMethod]
        public void IsNumeric_NonNumericObject_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric((object)"abc"));
        }

        [TestMethod]
        public void IsNumeric_NumericObject_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric((object)"42"));
        }
    }
}
