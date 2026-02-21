using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// GeneralLib クラスの追加ユニットテスト
    /// (LNormalize, IsSpace, InStr2, StrToDbl, StrToLng, StrToHiragana, MaxLng/MinLng, LeftPaddedString/RightPaddedString)
    /// </summary>
    [TestClass]
    public class GeneralLibFurtherTests
    {
        // ──────────────────────────────────────────────
        // LNormalize
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LNormalize_MultipleSpaces_CollapsesToSingle()
        {
            var result = GeneralLib.LNormalize("a  b   c");
            Assert.AreEqual("a b c", result);
        }

        [TestMethod]
        public void LNormalize_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.LNormalize(""));
        }

        [TestMethod]
        public void LNormalize_Null_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.LNormalize(null));
        }

        [TestMethod]
        public void LNormalize_SingleWord_ReturnsSame()
        {
            Assert.AreEqual("hello", GeneralLib.LNormalize("hello"));
        }

        [TestMethod]
        public void LNormalize_LeadingAndTrailingSpaces_Removed()
        {
            var result = GeneralLib.LNormalize("  hello world  ");
            Assert.AreEqual("hello world", result);
        }

        // ──────────────────────────────────────────────
        // IsSpace
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsSpace_EmptyString_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsSpace(""));
        }

        [TestMethod]
        public void IsSpace_Null_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsSpace(null));
        }

        [TestMethod]
        public void IsSpace_SingleSpace_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsSpace(" "));
        }

        [TestMethod]
        public void IsSpace_MultipleSpaces_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsSpace("   "));
        }

        [TestMethod]
        public void IsSpace_NonSpace_ReturnsFalse()
        {
            Assert.IsFalse(GeneralLib.IsSpace("a"));
        }

        [TestMethod]
        public void IsSpace_TabCharacter_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsSpace("\t"));
        }

        // ──────────────────────────────────────────────
        // InStr2 (末尾から検索)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStr2_FindsLastOccurrence()
        {
            // "hello" の中の最後の "l" は4番目
            Assert.AreEqual(4, GeneralLib.InStr2("hello", "l"));
        }

        [TestMethod]
        public void InStr2_SingleOccurrence_ReturnsPosition()
        {
            Assert.AreEqual(1, GeneralLib.InStr2("hello", "h"));
        }

        [TestMethod]
        public void InStr2_NotFound_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.InStr2("hello", "x"));
        }

        [TestMethod]
        public void InStr2_MultiCharSearch_FindsPosition()
        {
            // "abcabc" の最後の "abc" は4番目
            Assert.AreEqual(4, GeneralLib.InStr2("abcabc", "abc"));
        }

        // ──────────────────────────────────────────────
        // StrToDbl
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrToDbl_ValidNumber_ReturnsDouble()
        {
            Assert.AreEqual(3.14, GeneralLib.StrToDbl("3.14"), 1e-10);
        }

        [TestMethod]
        public void StrToDbl_Integer_ReturnsDouble()
        {
            Assert.AreEqual(42d, GeneralLib.StrToDbl("42"));
        }

        [TestMethod]
        public void StrToDbl_Empty_ReturnsZero()
        {
            Assert.AreEqual(0d, GeneralLib.StrToDbl(""));
        }

        [TestMethod]
        public void StrToDbl_NonNumeric_ReturnsZero()
        {
            Assert.AreEqual(0d, GeneralLib.StrToDbl("abc"));
        }

        [TestMethod]
        public void StrToDbl_Negative_ReturnsNegative()
        {
            Assert.AreEqual(-5.5, GeneralLib.StrToDbl("-5.5"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // StrToLng
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrToLng_ValidInt_ReturnsInt()
        {
            Assert.AreEqual(100, GeneralLib.StrToLng("100"));
        }

        [TestMethod]
        public void StrToLng_Empty_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.StrToLng(""));
        }

        [TestMethod]
        public void StrToLng_Float_TruncatesToInt()
        {
            Assert.AreEqual(3, GeneralLib.StrToLng("3.7"));
        }

        [TestMethod]
        public void StrToLng_Null_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.StrToLng(null));
        }

        // ──────────────────────────────────────────────
        // StrToHiragana
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrToHiragana_KatakanaToHiragana()
        {
            var result = GeneralLib.StrToHiragana("ア");
            Assert.AreEqual("あ", result);
        }

        [TestMethod]
        public void StrToHiragana_AlreadyHiragana_ReturnsSame()
        {
            var result = GeneralLib.StrToHiragana("あいう");
            Assert.AreEqual("あいう", result);
        }

        [TestMethod]
        public void StrToHiragana_Empty_ReturnsEmpty()
        {
            var result = GeneralLib.StrToHiragana("");
            Assert.AreEqual("", result);
        }

        // ──────────────────────────────────────────────
        // MaxLng / MinLng
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MaxLng_ReturnsLarger()
        {
            Assert.AreEqual(10, GeneralLib.MaxLng(5, 10));
            Assert.AreEqual(10, GeneralLib.MaxLng(10, 5));
        }

        [TestMethod]
        public void MaxLng_EqualValues_ReturnsSame()
        {
            Assert.AreEqual(7, GeneralLib.MaxLng(7, 7));
        }

        [TestMethod]
        public void MaxLng_NegativeValues_ReturnsLessNegative()
        {
            Assert.AreEqual(-1, GeneralLib.MaxLng(-5, -1));
        }

        [TestMethod]
        public void MinLng_ReturnsSmaller()
        {
            Assert.AreEqual(5, GeneralLib.MinLng(5, 10));
            Assert.AreEqual(5, GeneralLib.MinLng(10, 5));
        }

        [TestMethod]
        public void MinLng_NegativeValues_ReturnsMostNegative()
        {
            Assert.AreEqual(-5, GeneralLib.MinLng(-5, -1));
        }

        // ──────────────────────────────────────────────
        // MaxDbl / MinDbl
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MaxDbl_ReturnsLarger()
        {
            Assert.AreEqual(3.14, GeneralLib.MaxDbl(1.0, 3.14), 1e-10);
        }

        [TestMethod]
        public void MaxDbl_NegativeValues_ReturnsLessNegative()
        {
            Assert.AreEqual(-0.5, GeneralLib.MaxDbl(-1.0, -0.5), 1e-10);
        }

        [TestMethod]
        public void MinDbl_ReturnsSmaller()
        {
            Assert.AreEqual(1.0, GeneralLib.MinDbl(1.0, 3.14), 1e-10);
        }

        [TestMethod]
        public void MinDbl_NegativeValues_ReturnsMostNegative()
        {
            Assert.AreEqual(-1.0, GeneralLib.MinDbl(-1.0, -0.5), 1e-10);
        }

        // ──────────────────────────────────────────────
        // LeftPaddedString / RightPaddedString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LeftPaddedString_PadsToWidth()
        {
            // "abc" を幅5にする → "  abc"
            var result = GeneralLib.LeftPaddedString("abc", 5);
            Assert.AreEqual("  abc", result);
            Assert.AreEqual(5, GeneralLib.StrWidth(result));
        }

        [TestMethod]
        public void LeftPaddedString_ExactWidth_NoPadding()
        {
            var result = GeneralLib.LeftPaddedString("abc", 3);
            Assert.AreEqual("abc", result);
        }

        [TestMethod]
        public void LeftPaddedString_WiderThanWidth_NoPadding()
        {
            var result = GeneralLib.LeftPaddedString("abcdef", 3);
            Assert.AreEqual("abcdef", result);
        }

        [TestMethod]
        public void RightPaddedString_PadsToWidth()
        {
            // "abc" を幅5にする → "abc  "
            var result = GeneralLib.RightPaddedString("abc", 5);
            Assert.AreEqual("abc  ", result);
            Assert.AreEqual(5, GeneralLib.StrWidth(result));
        }

        [TestMethod]
        public void RightPaddedString_ExactWidth_NoPadding()
        {
            var result = GeneralLib.RightPaddedString("abc", 3);
            Assert.AreEqual("abc", result);
        }

        // ──────────────────────────────────────────────
        // LSplit
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LSplit_SpaceDelimited_ReturnsArray()
        {
            var count = GeneralLib.LSplit("a b c", out var arr);
            Assert.AreEqual(3, count);
            Assert.AreEqual("a", arr[0]);
            Assert.AreEqual("b", arr[1]);
            Assert.AreEqual("c", arr[2]);
        }

        [TestMethod]
        public void LSplit_Empty_ReturnsEmptyArray()
        {
            var count = GeneralLib.LSplit("", out var arr);
            Assert.AreEqual(0, count);
            Assert.AreEqual(0, arr.Length);
        }

        // ──────────────────────────────────────────────
        // ListTail
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ListTail_FromSecond_ReturnsTailElements()
        {
            var result = GeneralLib.ListTail("a b c d", 2);
            Assert.AreEqual("b c d", result);
        }

        [TestMethod]
        public void ListTail_FromFirst_ReturnsEmpty()
        {
            // idx=1はidx > 1 の条件に合わないので空文字列
            var result = GeneralLib.ListTail("a b c", 1);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void ListTail_BeyondLength_ReturnsEmpty()
        {
            var result = GeneralLib.ListTail("a b c", 10);
            Assert.AreEqual("", result);
        }
    }
}
