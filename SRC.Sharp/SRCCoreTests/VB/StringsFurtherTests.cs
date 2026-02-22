using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// Strings クラスの追加ユニットテスト (Space, StrDup, StrConv, Trim, Mid edge cases)
    /// </summary>
    [TestClass]
    public class StringsFurtherTests
    {
        // ──────────────────────────────────────────────
        // Space
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Space_ZeroCount_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Space(0));
        }

        [TestMethod]
        public void Space_NegativeCount_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Space(-1));
        }

        [TestMethod]
        public void Space_One_ReturnsSingleSpace()
        {
            Assert.AreEqual(" ", Strings.Space(1));
        }

        [TestMethod]
        public void Space_Five_ReturnsCorrectLength()
        {
            var result = Strings.Space(5);
            Assert.AreEqual("     ", result);
            Assert.AreEqual(5, result.Length);
        }

        [TestMethod]
        public void Space_Ten_ReturnsCorrectString()
        {
            var result = Strings.Space(10);
            Assert.AreEqual(10, result.Length);
            Assert.IsTrue(result == new string(' ', 10));
        }

        // ──────────────────────────────────────────────
        // StrDup
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrDup_SingleChar_RepeatsN()
        {
            Assert.AreEqual("aaa", Strings.StrDup("a", 3));
        }

        [TestMethod]
        public void StrDup_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrDup("", 5));
        }

        [TestMethod]
        public void StrDup_NullString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrDup(null, 3));
        }

        [TestMethod]
        public void StrDup_ZeroCount_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrDup("abc", 0));
        }

        [TestMethod]
        public void StrDup_NegativeCount_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrDup("abc", -1));
        }

        [TestMethod]
        public void StrDup_MulticharString_Repeats()
        {
            Assert.AreEqual("abcabc", Strings.StrDup("abc", 2));
        }

        [TestMethod]
        public void StrDup_JapaneseChar_RepeatsCorrectly()
        {
            Assert.AreEqual("あああ", Strings.StrDup("あ", 3));
        }

        // ──────────────────────────────────────────────
        // StrConv (Wide/Narrow/Hiragana)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrConv_Wide_ConvertsSingleAsciiChar()
        {
            var result = Strings.StrConv("A", VbStrConv.Wide);
            Assert.AreEqual("Ａ", result);
        }

        [TestMethod]
        public void StrConv_Wide_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrConv("", VbStrConv.Wide));
        }

        [TestMethod]
        public void StrConv_Narrow_ConvertFullWidthToHalfWidth()
        {
            // Ａ(0xFF21) → A(0x41)
            var result = Strings.StrConv("Ａ", VbStrConv.Narrow);
            Assert.AreEqual("A", result);
        }

        [TestMethod]
        public void StrConv_Hiragana_ConvertKatakanaToHiragana()
        {
            // ア(0x30A2) → あ(0x3042)
            var result = Strings.StrConv("ア", VbStrConv.Hiragana);
            Assert.AreEqual("あ", result);
        }

        [TestMethod]
        public void StrConv_Hiragana_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrConv("", VbStrConv.Hiragana));
        }

        [TestMethod]
        public void StrConv_Wide_Null_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrConv(null, VbStrConv.Wide));
        }

        // ──────────────────────────────────────────────
        // Trim
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Trim_NullString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Trim(null));
        }

        [TestMethod]
        public void Trim_AlreadyTrimmed_ReturnsSame()
        {
            Assert.AreEqual("hello", Strings.Trim("hello"));
        }

        [TestMethod]
        public void Trim_OnlySpaces_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Trim("   "));
        }

        [TestMethod]
        public void Trim_TabAndSpace_Removed()
        {
            Assert.AreEqual("hello", Strings.Trim("\t hello \t"));
        }

        // ──────────────────────────────────────────────
        // Mid edge cases
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Mid_StartZero_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Mid("hello", 0, 2));
        }

        [TestMethod]
        public void Mid_StartBeyondLength_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Mid("hello", 10, 2));
        }

        [TestMethod]
        public void Mid_LengthZero_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Mid("hello", 2, 0));
        }

        [TestMethod]
        public void Mid_LengthNegative_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Mid("hello", 2, -1));
        }

        [TestMethod]
        public void Mid_NullString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Mid(null, 1, 5));
        }

        [TestMethod]
        public void Mid_1ArgNull_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Mid(null, 1));
        }

        // ──────────────────────────────────────────────
        // Len
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Len_Null_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.Len(null));
        }

        [TestMethod]
        public void Len_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.Len(""));
        }

        [TestMethod]
        public void Len_SingleChar_ReturnsOne()
        {
            Assert.AreEqual(1, Strings.Len("a"));
        }

        [TestMethod]
        public void Len_JapaneseChars_CountsCorrectly()
        {
            Assert.AreEqual(3, Strings.Len("あいう"));
        }

        // ──────────────────────────────────────────────
        // Left / Right edge cases
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Left_Null_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Left(null, 3));
        }

        [TestMethod]
        public void Left_ZeroLength_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Left("hello", 0));
        }

        [TestMethod]
        public void Right_ZeroLength_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Right("hello", 0));
        }

        [TestMethod]
        public void Right_NegativeLength_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Right("hello", -1));
        }

        [TestMethod]
        public void Right_Null_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Right(null, 3));
        }
    }
}
