using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    [TestClass]
    public class StringsMoreTests3
    {
        // ──────────────────────────────────────────────
        // InStr
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStr_WithStartPosition_FindsFromStart()
        {
            Assert.AreEqual(5, Strings.InStr(3, "abcabcabc", "bc"));
        }

        [TestMethod]
        public void InStr_StartPositionSkipsFirstOccurrence()
        {
            // "xabcabc" starting at position 4 skips the first "abc" and finds the second one at position 5
            Assert.AreEqual(5, Strings.InStr(4, "xabcabc", "abc"));
        }

        [TestMethod]
        public void InStr_NotFound_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.InStr("hello", "xyz"));
        }

        [TestMethod]
        public void InStr_StartBeyondString_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.InStr(100, "hello", "e"));
        }

        [TestMethod]
        public void InStr_EmptySearchString_ReturnsOne()
        {
            // IndexOf("") returns 0, so InStr returns 1
            Assert.AreEqual(1, Strings.InStr("hello", ""));
        }

        // ──────────────────────────────────────────────
        // Left
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Left_LenZero_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Left("hello", 0));
        }

        [TestMethod]
        public void Left_LenGreaterThanString_ReturnsFullString()
        {
            Assert.AreEqual("hello", Strings.Left("hello", 100));
        }

        [TestMethod]
        public void Left_NormalCase_ReturnsPrefix()
        {
            Assert.AreEqual("hel", Strings.Left("hello", 3));
        }

        [TestMethod]
        public void Left_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Left("", 5));
        }

        // ──────────────────────────────────────────────
        // Right
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Right_LenZero_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Right("hello", 0));
        }

        [TestMethod]
        public void Right_LenGreaterThanString_ReturnsFullString()
        {
            Assert.AreEqual("hello", Strings.Right("hello", 100));
        }

        [TestMethod]
        public void Right_NormalCase_ReturnsSuffix()
        {
            Assert.AreEqual("llo", Strings.Right("hello", 3));
        }

        [TestMethod]
        public void Right_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Right("", 3));
        }

        // ──────────────────────────────────────────────
        // Mid
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Mid_NormalCase_ReturnsSubstring()
        {
            Assert.AreEqual("ell", Strings.Mid("hello", 2, 3));
        }

        [TestMethod]
        public void Mid_StartAtEnd_ReturnsLastChar()
        {
            Assert.AreEqual("o", Strings.Mid("hello", 5, 1));
        }

        [TestMethod]
        public void Mid_LengthExceedsString_ReturnsToEnd()
        {
            Assert.AreEqual("ello", Strings.Mid("hello", 2, 100));
        }

        [TestMethod]
        public void Mid_StartBeyondString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Mid("hello", 10, 3));
        }

        [TestMethod]
        public void Mid_WithoutLength_ReturnsToEnd()
        {
            Assert.AreEqual("llo", Strings.Mid("hello", 3));
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
        public void Len_JapaneseCharacters_ReturnsCharCount()
        {
            Assert.AreEqual(3, Strings.Len("あいう"));
        }

        [TestMethod]
        public void Len_NormalString_ReturnsLength()
        {
            Assert.AreEqual(5, Strings.Len("hello"));
        }

        // ──────────────────────────────────────────────
        // LCase
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LCase_UpperCase_ReturnsLower()
        {
            Assert.AreEqual("hello world", Strings.LCase("HELLO WORLD"));
        }

        [TestMethod]
        public void LCase_AlreadyLower_ReturnsSame()
        {
            Assert.AreEqual("abc", Strings.LCase("abc"));
        }

        // ──────────────────────────────────────────────
        // Trim
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Trim_WhitespaceAround_RemovesWhitespace()
        {
            Assert.AreEqual("hello", Strings.Trim("  hello  "));
        }

        [TestMethod]
        public void Trim_NoWhitespace_ReturnsSame()
        {
            Assert.AreEqual("hello", Strings.Trim("hello"));
        }

        [TestMethod]
        public void Trim_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Trim(""));
        }

        [TestMethod]
        public void Trim_OnlyWhitespace_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Trim("   "));
        }

        // ──────────────────────────────────────────────
        // Space
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Space_Three_ReturnsThreeSpaces()
        {
            Assert.AreEqual("   ", Strings.Space(3));
        }

        [TestMethod]
        public void Space_Zero_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Space(0));
        }

        [TestMethod]
        public void Space_Negative_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Space(-1));
        }

        // ──────────────────────────────────────────────
        // StrDup (String_ equivalent)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrDup_CountZero_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrDup("a", 0));
        }

        [TestMethod]
        public void StrDup_SingleChar_RepeatsCorrectly()
        {
            Assert.AreEqual("aaa", Strings.StrDup("a", 3));
        }

        [TestMethod]
        public void StrDup_MultiChar_RepeatsCorrectly()
        {
            Assert.AreEqual("abab", Strings.StrDup("ab", 2));
        }
    }
}
