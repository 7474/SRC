using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;

namespace SRCCore.Lib.Tests
{
    [TestClass]
    public class GeneralLibMoreTests4
    {
        // ──────────────────────────────────────────────
        // MaxLng / MinLng
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MaxLng_ReturnsLarger()
        {
            Assert.AreEqual(5, GeneralLib.MaxLng(5, 3));
        }

        [TestMethod]
        public void MaxLng_NegativeValues_ReturnsLarger()
        {
            Assert.AreEqual(-1, GeneralLib.MaxLng(-1, -2));
        }

        [TestMethod]
        public void MaxLng_EqualValues_ReturnsSame()
        {
            Assert.AreEqual(4, GeneralLib.MaxLng(4, 4));
        }

        [TestMethod]
        public void MinLng_ReturnsSmaller()
        {
            Assert.AreEqual(3, GeneralLib.MinLng(5, 3));
        }

        [TestMethod]
        public void MinLng_NegativeValues_ReturnsSmaller()
        {
            Assert.AreEqual(-2, GeneralLib.MinLng(-1, -2));
        }

        // ──────────────────────────────────────────────
        // MaxDbl / MinDbl
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MaxDbl_ReturnsLarger()
        {
            Assert.AreEqual(5.5, GeneralLib.MaxDbl(5.5, 3.3), 0.0001);
        }

        [TestMethod]
        public void MinDbl_ReturnsSmaller()
        {
            Assert.AreEqual(3.3, GeneralLib.MinDbl(5.5, 3.3), 0.0001);
        }

        // ──────────────────────────────────────────────
        // LLength
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LLength_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.LLength(""));
        }

        [TestMethod]
        public void LLength_SingleToken_ReturnsOne()
        {
            Assert.AreEqual(1, GeneralLib.LLength("abc"));
        }

        [TestMethod]
        public void LLength_MultipleTokens_ReturnsCount()
        {
            Assert.AreEqual(3, GeneralLib.LLength("a b c"));
        }

        [TestMethod]
        public void LLength_ExtraSpaces_CountsTokens()
        {
            Assert.AreEqual(2, GeneralLib.LLength("  foo  bar  "));
        }

        // ──────────────────────────────────────────────
        // LIndex
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LIndex_Position1_ReturnsFirstToken()
        {
            Assert.AreEqual("alpha", GeneralLib.LIndex("alpha beta gamma", 1));
        }

        [TestMethod]
        public void LIndex_Position2_ReturnsSecondToken()
        {
            Assert.AreEqual("beta", GeneralLib.LIndex("alpha beta gamma", 2));
        }

        [TestMethod]
        public void LIndex_OutOfRange_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.LIndex("alpha beta", 5));
        }

        [TestMethod]
        public void LIndex_ZeroOrNegative_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.LIndex("alpha beta", 0));
            Assert.AreEqual("", GeneralLib.LIndex("alpha beta", -1));
        }

        // ──────────────────────────────────────────────
        // FormatNum
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FormatNum_Zero_ReturnsZeroString()
        {
            Assert.AreEqual("0", GeneralLib.FormatNum(0));
        }

        [TestMethod]
        public void FormatNum_Integer_ReturnsIntString()
        {
            Assert.AreEqual("100", GeneralLib.FormatNum(100));
        }

        [TestMethod]
        public void FormatNum_Negative_ReturnsNegativeString()
        {
            Assert.AreEqual("-5", GeneralLib.FormatNum(-5));
        }

        // ──────────────────────────────────────────────
        // StrToLng
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrToLng_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.StrToLng(""));
        }

        [TestMethod]
        public void StrToLng_ValidNumber_ReturnsInt()
        {
            Assert.AreEqual(123, GeneralLib.StrToLng("123"));
        }

        [TestMethod]
        public void StrToLng_NonNumeric_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.StrToLng("abc"));
        }

        // ──────────────────────────────────────────────
        // InStrNotNest
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStrNotNest_FindsSimplePattern()
        {
            int result = GeneralLib.InStrNotNest("abc", "b");
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void InStrNotNest_NotFound_ReturnsZero()
        {
            int result = GeneralLib.InStrNotNest("hello", "xyz");
            Assert.AreEqual(0, result);
        }
    }
}
