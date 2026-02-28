using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;
using System.Collections.Generic;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// Strings クラスの追加ユニットテスト
    /// </summary>
    [TestClass]
    public class StringsMoreTests
    {
        // ──────────────────────────────────────────────
        // Asc
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Asc_SpaceChar_Returns32()
        {
            Assert.AreEqual(32, Strings.Asc(' '));
        }

        [TestMethod]
        public void Asc_EmptyString_ThrowsIndexOutOfRange()
        {
            Assert.Throws<System.IndexOutOfRangeException>(() => Strings.Asc(""));
        }

        [TestMethod]
        public void Asc_NullString_ThrowsNullReference()
        {
            // null 文字列へのアクセスは NullReferenceException or IndexOutOfRangeException
            Assert.Throws<System.NullReferenceException>(() => Strings.Asc((string)null));
        }

        [TestMethod]
        public void Asc_ForwardSlash_Returns47()
        {
            Assert.AreEqual(47, Strings.Asc('/'));
        }

        [TestMethod]
        public void Asc_Asterisk_Returns42()
        {
            Assert.AreEqual(42, Strings.Asc('*'));
        }

        // ──────────────────────────────────────────────
        // LCase
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LCase_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.LCase(""));
        }

        [TestMethod]
        public void LCase_AlreadyLowercase_Unchanged()
        {
            Assert.AreEqual("hello", Strings.LCase("hello"));
        }

        [TestMethod]
        public void LCase_MixedCase_ConvertsToLower()
        {
            Assert.AreEqual("hello world", Strings.LCase("Hello World"));
        }

        // ──────────────────────────────────────────────
        // InStr
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStr_NotFound_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.InStr("hello", "xyz"));
        }

        [TestMethod]
        public void InStr_Found_ReturnsOneBasedPosition()
        {
            Assert.AreEqual(1, Strings.InStr("hello", "h"));
            Assert.AreEqual(5, Strings.InStr("hello", "o"));
        }

        [TestMethod]
        public void InStr_EmptyHaystack_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.InStr("", "x"));
        }

        [TestMethod]
        public void InStr_EmptyNeedle_ReturnsOne()
        {
            // 空文字列は必ず位置1で見つかる
            Assert.AreEqual(1, Strings.InStr("hello", ""));
        }

        [TestMethod]
        public void InStr_WithStart_SearchesFromPosition()
        {
            // "abcabc" で start=2 から "abc" を探すと位置4
            Assert.AreEqual(4, Strings.InStr(2, "abcabc", "abc"));
        }

        [TestMethod]
        public void InStr_WithStart_FirstOccurrenceBeforeStart_NotFound()
        {
            // "abcabc" で start=2 から "a" を探すと位置4
            Assert.AreEqual(4, Strings.InStr(2, "abcabc", "a"));
        }

        // ──────────────────────────────────────────────
        // Left
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Left_LengthZero_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Left("hello", 0));
        }

        [TestMethod]
        public void Left_LengthBeyondString_ReturnsWholeString()
        {
            Assert.AreEqual("hello", Strings.Left("hello", 100));
        }

        [TestMethod]
        public void Left_NullString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Left(null, 5));
        }

        // ──────────────────────────────────────────────
        // Right
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Right_LastChars()
        {
            Assert.AreEqual("lo", Strings.Right("hello", 2));
        }

        [TestMethod]
        public void Right_LengthZero_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Right("hello", 0));
        }

        [TestMethod]
        public void Right_LengthBeyondString_ReturnsWholeString()
        {
            Assert.AreEqual("hello", Strings.Right("hello", 100));
        }

        [TestMethod]
        public void Right_NullString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Right(null, 3));
        }

        // ──────────────────────────────────────────────
        // Mid
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Mid_FromMiddle()
        {
            Assert.AreEqual("ell", Strings.Mid("hello", 2, 3));
        }

        [TestMethod]
        public void Mid_FromStart()
        {
            Assert.AreEqual("hel", Strings.Mid("hello", 1, 3));
        }

        [TestMethod]
        public void Mid_BeyondLength_ReturnsUpToEnd()
        {
            Assert.AreEqual("lo", Strings.Mid("hello", 4, 100));
        }

        [TestMethod]
        public void Mid_WithoutLength_ReturnsToEnd()
        {
            Assert.AreEqual("llo", Strings.Mid("hello", 3));
        }

        [TestMethod]
        public void Mid_StartPastEnd_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Mid("hello", 10));
        }

        // ──────────────────────────────────────────────
        // Len
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Len_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.Len(""));
        }

        [TestMethod]
        public void Len_NullString_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.Len(null));
        }

        [TestMethod]
        public void Len_JapaneseString_ReturnsCharCount()
        {
            Assert.AreEqual(3, Strings.Len("あいう"));
        }

        // ──────────────────────────────────────────────
        // Space
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Space_NegativeNumber_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Space(-1));
        }

        [TestMethod]
        public void Space_FiveSpaces()
        {
            Assert.AreEqual("     ", Strings.Space(5));
        }

        // ──────────────────────────────────────────────
        // StrDup
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrDup_NullInput_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrDup(null, 3));
        }

        [TestMethod]
        public void StrDup_ZeroN_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrDup("xyz", 0));
        }

        [TestMethod]
        public void StrDup_NegativeN_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrDup("x", -1));
        }

        // ──────────────────────────────────────────────
        // StrComp
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrComp_Equal_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.StrComp("abc", "abc"));
        }

        [TestMethod]
        public void StrComp_FirstLess_ReturnsNegative()
        {
            Assert.IsTrue(Strings.StrComp("abc", "abd") < 0);
        }

        [TestMethod]
        public void StrComp_FirstGreater_ReturnsPositive()
        {
            Assert.IsTrue(Strings.StrComp("abd", "abc") > 0);
        }

        // ──────────────────────────────────────────────
        // Trim
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Trim_WithSpaces_RemovesLeadingTrailing()
        {
            Assert.AreEqual("hello", Strings.Trim("  hello  "));
        }

        [TestMethod]
        public void Trim_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Trim(""));
        }

        [TestMethod]
        public void Trim_NullString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Trim(null));
        }

        [TestMethod]
        public void Trim_NoSpaces_Unchanged()
        {
            Assert.AreEqual("hello", Strings.Trim("hello"));
        }

        [TestMethod]
        public void Trim_WithTabAndSpace_RemovesBoth()
        {
            Assert.AreEqual("hello", Strings.Trim("\t hello \t"));
        }

        // ──────────────────────────────────────────────
        // InStrB (byte-based)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStrB_AsciiFound_ReturnsOneBasedPosition()
        {
            // "abc" の "a" はバイト位置 1
            Assert.AreEqual(1, Strings.InStrB("abc", "a"));
        }

        [TestMethod]
        public void InStrB_NotFound_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.InStrB("abc", "x"));
        }
    }
}
