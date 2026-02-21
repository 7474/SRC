using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using System.Linq;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// GeneralLib の追加エッジケーステスト
    /// </summary>
    [TestClass]
    public class GeneralLibAdditionalTests
    {
        // ──────────────────────────────────────────────
        // FormatNum
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FormatNum_Integer_ReturnsIntegerString()
        {
            Assert.AreEqual("42", GeneralLib.FormatNum(42d));
        }

        [TestMethod]
        public void FormatNum_Zero_ReturnsZeroString()
        {
            Assert.AreEqual("0", GeneralLib.FormatNum(0d));
        }

        [TestMethod]
        public void FormatNum_NegativeInteger_ReturnsNegativeString()
        {
            Assert.AreEqual("-10", GeneralLib.FormatNum(-10d));
        }

        [TestMethod]
        public void FormatNum_FloatNumber_ReturnsDecimalString()
        {
            var result = GeneralLib.FormatNum(3.14d);
            Assert.AreEqual("3.14", result);
        }

        [TestMethod]
        public void FormatNum_LargeInteger_ReturnsWithoutExponent()
        {
            Assert.AreEqual("1000000", GeneralLib.FormatNum(1000000d));
        }

        [TestMethod]
        public void FormatNum_NegativeDecimal_ReturnsNegativeDecimalString()
        {
            Assert.AreEqual("-0.5", GeneralLib.FormatNum(-0.5d));
        }

        // ──────────────────────────────────────────────
        // IsNumber
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsNumber_Integer_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsNumber("42"));
        }

        [TestMethod]
        public void IsNumber_Negative_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsNumber("-5"));
        }

        [TestMethod]
        public void IsNumber_Float_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsNumber("3.14"));
        }

        [TestMethod]
        public void IsNumber_ParenthesizedNumber_ReturnsFalse()
        {
            // "(1)"のような文字列は数値として扱わない
            Assert.IsFalse(GeneralLib.IsNumber("(1)"));
        }

        [TestMethod]
        public void IsNumber_AlphaString_ReturnsFalse()
        {
            Assert.IsFalse(GeneralLib.IsNumber("abc"));
        }

        [TestMethod]
        public void IsNumber_EmptyString_ReturnsFalse()
        {
            Assert.IsFalse(GeneralLib.IsNumber(""));
        }

        [TestMethod]
        public void IsNumber_WhitespaceOnly_ReturnsFalse()
        {
            Assert.IsFalse(GeneralLib.IsNumber("   "));
        }

        // ──────────────────────────────────────────────
        // IsSpace
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsSpace_SingleSpace_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsSpace(" "));
        }

        [TestMethod]
        public void IsSpace_Tab_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsSpace("\t"));
        }

        [TestMethod]
        public void IsSpace_Letter_ReturnsFalse()
        {
            Assert.IsFalse(GeneralLib.IsSpace("a"));
        }

        [TestMethod]
        public void IsSpace_EmptyString_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsSpace(""));
        }

        // ──────────────────────────────────────────────
        // MaxLng / MinLng / MaxDbl / MinDbl
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MaxLng_FirstIsLarger_ReturnsFirst()
        {
            Assert.AreEqual(10, GeneralLib.MaxLng(10, 5));
        }

        [TestMethod]
        public void MaxLng_SecondIsLarger_ReturnsSecond()
        {
            Assert.AreEqual(10, GeneralLib.MaxLng(5, 10));
        }

        [TestMethod]
        public void MaxLng_BothEqual_ReturnsValue()
        {
            Assert.AreEqual(7, GeneralLib.MaxLng(7, 7));
        }

        [TestMethod]
        public void MaxLng_NegativeValues_ReturnsLarger()
        {
            Assert.AreEqual(-1, GeneralLib.MaxLng(-1, -5));
        }

        [TestMethod]
        public void MinLng_FirstIsSmaller_ReturnsFirst()
        {
            Assert.AreEqual(3, GeneralLib.MinLng(3, 10));
        }

        [TestMethod]
        public void MinLng_SecondIsSmaller_ReturnsSecond()
        {
            Assert.AreEqual(3, GeneralLib.MinLng(10, 3));
        }

        [TestMethod]
        public void MinLng_BothEqual_ReturnsValue()
        {
            Assert.AreEqual(5, GeneralLib.MinLng(5, 5));
        }

        [TestMethod]
        public void MaxDbl_FirstIsLarger_ReturnsFirst()
        {
            Assert.AreEqual(10.5d, GeneralLib.MaxDbl(10.5, 3.2));
        }

        [TestMethod]
        public void MinDbl_SecondIsSmaller_ReturnsSecond()
        {
            Assert.AreEqual(1.1d, GeneralLib.MinDbl(5.0, 1.1));
        }

        // ──────────────────────────────────────────────
        // StrToDbl
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrToDbl_Integer_ReturnsDouble()
        {
            Assert.AreEqual(42d, GeneralLib.StrToDbl("42"));
        }

        [TestMethod]
        public void StrToDbl_Float_ReturnsDouble()
        {
            Assert.AreEqual(3.14d, GeneralLib.StrToDbl("3.14"), 1e-10);
        }

        [TestMethod]
        public void StrToDbl_NonNumeric_ReturnsZero()
        {
            Assert.AreEqual(0d, GeneralLib.StrToDbl("abc"));
        }

        [TestMethod]
        public void StrToDbl_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0d, GeneralLib.StrToDbl(""));
        }

        // ──────────────────────────────────────────────
        // StrToLng
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrToLng_Integer_ReturnsInt()
        {
            Assert.AreEqual(100, GeneralLib.StrToLng("100"));
        }

        [TestMethod]
        public void StrToLng_Float_TruncatesToInt()
        {
            Assert.AreEqual(3, GeneralLib.StrToLng("3.9"));
        }

        [TestMethod]
        public void StrToLng_NonNumeric_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.StrToLng("abc"));
        }

        // ──────────────────────────────────────────────
        // LNormalize
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LNormalize_SpaceSeparated_ReturnsNormalized()
        {
            var result = GeneralLib.LNormalize("a b  c");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void LNormalize_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.LNormalize(""));
        }

        [TestMethod]
        public void LNormalize_SingleItem_ReturnsSame()
        {
            Assert.AreEqual("abc", GeneralLib.LNormalize("abc"));
        }

        // ──────────────────────────────────────────────
        // ListTail
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ListTail_FromIndexTwo_ReturnsTail()
        {
            // "a b c d" から idx=2 以降 → "b c d"
            Assert.AreEqual("b c d", GeneralLib.ListTail("a b c d", 2));
        }

        [TestMethod]
        public void ListTail_FromIndexOne_ReturnsEmpty()
        {
            // idx=1 は戻り値""
            Assert.AreEqual("", GeneralLib.ListTail("a b c", 1));
        }

        [TestMethod]
        public void ListTail_IndexBeyondLength_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.ListTail("a b c", 10));
        }

        [TestMethod]
        public void ListTail_FromIndexThree_ReturnsTailFromThird()
        {
            Assert.AreEqual("c d", GeneralLib.ListTail("a b c d", 3));
        }

        // ──────────────────────────────────────────────
        // LIndex
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LIndex_FirstElement_ReturnsFirst()
        {
            Assert.AreEqual("alpha", GeneralLib.LIndex("alpha beta gamma", 1));
        }

        [TestMethod]
        public void LIndex_SecondElement_ReturnsSecond()
        {
            Assert.AreEqual("beta", GeneralLib.LIndex("alpha beta gamma", 2));
        }

        [TestMethod]
        public void LIndex_OutOfBounds_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.LIndex("a b", 5));
        }

        // ──────────────────────────────────────────────
        // LLength
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LLength_ThreeElements_ReturnsThree()
        {
            Assert.AreEqual(3, GeneralLib.LLength("a b c"));
        }

        [TestMethod]
        public void LLength_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.LLength(""));
        }

        [TestMethod]
        public void LLength_SingleElement_ReturnsOne()
        {
            Assert.AreEqual(1, GeneralLib.LLength("abc"));
        }

        // ──────────────────────────────────────────────
        // InStr2
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStr2_Found_ReturnsPosition()
        {
            Assert.AreEqual(2, GeneralLib.InStr2("abcdef", "bc"));
        }

        [TestMethod]
        public void InStr2_NotFound_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.InStr2("abc", "xyz"));
        }

        [TestMethod]
        public void InStr2_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.InStr2("", "a"));
        }

        // ──────────────────────────────────────────────
        // StrToHiragana
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrToHiragana_Katakana_ConvertsToHiragana()
        {
            var result = GeneralLib.StrToHiragana("アイウエオ");
            Assert.AreEqual("あいうえお", result);
        }

        [TestMethod]
        public void StrToHiragana_Hiragana_Unchanged()
        {
            var result = GeneralLib.StrToHiragana("あいう");
            Assert.AreEqual("あいう", result);
        }

        [TestMethod]
        public void StrToHiragana_AsciiString_Unchanged()
        {
            var result = GeneralLib.StrToHiragana("abc");
            Assert.AreEqual("abc", result);
        }

        // ──────────────────────────────────────────────
        // StrWidth
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrWidth_AsciiString_ReturnsCharCount()
        {
            Assert.AreEqual(5, GeneralLib.StrWidth("hello"));
        }

        [TestMethod]
        public void StrWidth_JapaneseString_ReturnsDoubleCharCount()
        {
            // 全角文字は2文字換算
            Assert.AreEqual(6, GeneralLib.StrWidth("あいう"));
        }

        [TestMethod]
        public void StrWidth_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.StrWidth(""));
        }

        [TestMethod]
        public void StrWidth_MixedString_ReturnsMixedCount()
        {
            // "a" (1) + "あ" (2) = 3
            Assert.AreEqual(3, GeneralLib.StrWidth("aあ"));
        }

        // ──────────────────────────────────────────────
        // GetClassBundle (追加ケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetClassBundle_JapaneseCharAtFirstIdx_ReturnsChar()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("火水木", ref idx);
            Assert.AreEqual("火", result);
        }

        [TestMethod]
        public void GetClassBundle_WeakPrefix_ReturnsPrefixAndChar()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("弱火", ref idx);
            Assert.AreEqual("弱火", result);
        }

        [TestMethod]
        public void GetClassBundle_MultipleWeakPrefixes_ReturnsAll()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("弱弱火", ref idx);
            Assert.AreEqual("弱弱火", result);
        }

        // ──────────────────────────────────────────────
        // InStrNotNest (追加ケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStrNotNest_EmptyHaystack_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.InStrNotNest("", "炎"));
        }

        [TestMethod]
        public void InStrNotNest_EmptyNeedle_ReturnsStartPosition()
        {
            // Strings.InStr で空の needle は start 位置(1)を返す
            Assert.AreEqual(1, GeneralLib.InStrNotNest("abc", ""));
        }

        [TestMethod]
        public void InStrNotNest_AtStart_ReturnsOne()
        {
            Assert.AreEqual(1, GeneralLib.InStrNotNest("炎水", "炎"));
        }

        [TestMethod]
        public void InStrNotNest_PrecededByWeakPrefix_SkipsAndFindsNext()
        {
            // "弱炎水炎" → 最初の"炎"は"弱"に続くのでスキップ → 次の"炎"は"水"に続く → 位置4
            Assert.AreEqual(4, GeneralLib.InStrNotNest("弱炎水炎", "炎"));
        }

        [TestMethod]
        public void InStrNotNest_AllPrecededByPrefix_ReturnsZero()
        {
            // "弱炎" → "炎"は"弱"に続く → スキップして見つからない → 0
            Assert.AreEqual(0, GeneralLib.InStrNotNest("弱炎", "炎"));
        }

        // ──────────────────────────────────────────────
        // ToList (追加ケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToList_SpaceDelimited_SplitsElements()
        {
            var result = GeneralLib.ToList("a b c");
            CollectionAssert.AreEqual(new[] { "a", "b", "c" }, result.ToArray());
        }

        [TestMethod]
        public void ToList_BacktickQuotedTokenPreservesContent()
        {
            // バッククォートで囲まれた文字列はスペースを含んでも一つのトークン（クォートは保持される）
            var result = GeneralLib.ToList("`hello world`");
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("`hello world`", result[0]);
        }

        [TestMethod]
        public void ToList_EmptyString_ReturnsEmpty()
        {
            var result = GeneralLib.ToList("");
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ToList_ParenthesisGroups_KeptTogether()
        {
            // "(a b)" はひとつのトークン
            var result = GeneralLib.ToList("x (a b) y");
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("(a b)", result[1]);
        }

        // ──────────────────────────────────────────────
        // LeftPaddedString / RightPaddedString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LeftPaddedString_ShortString_PadsLeft()
        {
            var result = GeneralLib.LeftPaddedString("abc", 6);
            Assert.AreEqual("   abc", result);
        }

        [TestMethod]
        public void LeftPaddedString_ExactLength_NoPadding()
        {
            var result = GeneralLib.LeftPaddedString("abc", 3);
            Assert.AreEqual("abc", result);
        }

        [TestMethod]
        public void LeftPaddedString_LongerThanWidth_ReturnsOriginal()
        {
            var result = GeneralLib.LeftPaddedString("abcdef", 3);
            Assert.AreEqual("abcdef", result);
        }

        [TestMethod]
        public void RightPaddedString_ShortString_PadsRight()
        {
            var result = GeneralLib.RightPaddedString("abc", 6);
            Assert.AreEqual("abc   ", result);
        }

        [TestMethod]
        public void RightPaddedString_ExactLength_NoPadding()
        {
            var result = GeneralLib.RightPaddedString("abc", 3);
            Assert.AreEqual("abc", result);
        }

        [TestMethod]
        public void RightPaddedString_LongerThanWidth_ReturnsOriginal()
        {
            var result = GeneralLib.RightPaddedString("abcdef", 3);
            Assert.AreEqual("abcdef", result);
        }
    }
}
