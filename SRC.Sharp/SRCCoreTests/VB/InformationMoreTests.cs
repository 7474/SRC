using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// Information クラスの追加ユニットテスト
    /// </summary>
    [TestClass]
    public class InformationMoreTests
    {
        // ──────────────────────────────────────────────
        // IsNumeric
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsNumeric_Integer_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric(42));
            Assert.IsTrue(Information.IsNumeric(0));
            Assert.IsTrue(Information.IsNumeric(-10));
        }

        [TestMethod]
        public void IsNumeric_DoubleValue_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric(3.14));
            Assert.IsTrue(Information.IsNumeric(-0.5));
        }

        [TestMethod]
        public void IsNumeric_NumericString_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("42"));
            Assert.IsTrue(Information.IsNumeric("3.14"));
            Assert.IsTrue(Information.IsNumeric("-10"));
            Assert.IsTrue(Information.IsNumeric("0"));
        }

        [TestMethod]
        public void IsNumeric_StringWithWhitespace_ReturnsTrue()
        {
            // 空白は無視されて数値判定される
            Assert.IsTrue(Information.IsNumeric(" 42 "));
            Assert.IsTrue(Information.IsNumeric("3.14 "));
        }

        [TestMethod]
        public void IsNumeric_NonNumericString_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric("abc"));
            Assert.IsFalse(Information.IsNumeric("hello"));
            Assert.IsFalse(Information.IsNumeric("1a2"));
        }

        [TestMethod]
        public void IsNumeric_EmptyString_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric(""));
        }

        [TestMethod]
        public void IsNumeric_Null_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric(null));
        }

        [TestMethod]
        public void IsNumeric_BoolTrue_ReturnsFalse()
        {
            // bool は数値として扱わない
            Assert.IsFalse(Information.IsNumeric(true));
        }

        [TestMethod]
        public void IsNumeric_BoolFalse_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric(false));
        }

        [TestMethod]
        public void IsNumeric_LargeNumber_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("999999999999"));
        }

        [TestMethod]
        public void IsNumeric_NegativeDecimal_ReturnsTrue()
        {
            Assert.IsTrue(Information.IsNumeric("-3.14"));
        }

        [TestMethod]
        public void IsNumeric_PlusSignNumeric_ReturnsTrue()
        {
            // .NET の decimal.TryParse は "+42" を有効と見なす
            Assert.IsTrue(Information.IsNumeric("+42"));
        }

        [TestMethod]
        public void IsNumeric_OnlyDot_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric("."));
        }

        [TestMethod]
        public void IsNumeric_OnlyMinus_ReturnsFalse()
        {
            Assert.IsFalse(Information.IsNumeric("-"));
        }
    }
}
