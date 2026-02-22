using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// Strings クラスの追加ユニットテスト (StringsMoreTests4)
    /// </summary>
    [TestClass]
    public class StringsMoreTests4
    {
        // ──────────────────────────────────────────────
        // Asc(char)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Asc_CharA_Returns65()
        {
            Assert.AreEqual(65, Strings.Asc('A'));
        }

        [TestMethod]
        public void Asc_CharLowerA_Returns97()
        {
            Assert.AreEqual(97, Strings.Asc('a'));
        }

        [TestMethod]
        public void Asc_CharZero_Returns48()
        {
            Assert.AreEqual(48, Strings.Asc('0'));
        }

        [TestMethod]
        public void Asc_Space_Returns32()
        {
            Assert.AreEqual(32, Strings.Asc(' '));
        }

        // ──────────────────────────────────────────────
        // Asc(string)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Asc_StringA_Returns65()
        {
            Assert.AreEqual(65, Strings.Asc("A"));
        }

        [TestMethod]
        public void Asc_StringABC_Returns65()
        {
            // Returns code for first character only
            Assert.AreEqual(65, Strings.Asc("ABC"));
        }

        // ──────────────────────────────────────────────
        // StrComp
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrComp_EqualStrings_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.StrComp("abc", "abc"));
        }

        [TestMethod]
        public void StrComp_LessThan_ReturnsNegative()
        {
            Assert.IsTrue(Strings.StrComp("abc", "abd") < 0);
        }

        [TestMethod]
        public void StrComp_GreaterThan_ReturnsPositive()
        {
            Assert.IsTrue(Strings.StrComp("abd", "abc") > 0);
        }

        // ──────────────────────────────────────────────
        // StrDup
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrDup_SingleChar_RepeatsTimes()
        {
            Assert.AreEqual("aaa", Strings.StrDup("a", 3));
        }

        [TestMethod]
        public void StrDup_MultiChar_RepeatsTimes()
        {
            Assert.AreEqual("abab", Strings.StrDup("ab", 2));
        }

        [TestMethod]
        public void StrDup_Zero_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrDup("a", 0));
        }

        [TestMethod]
        public void StrDup_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrDup("", 5));
        }

        // ──────────────────────────────────────────────
        // Trim
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Trim_LeadingAndTrailingSpaces_RemovesBoth()
        {
            Assert.AreEqual("hello", Strings.Trim("  hello  "));
        }

        [TestMethod]
        public void Trim_NoSpaces_ReturnsSame()
        {
            Assert.AreEqual("hello", Strings.Trim("hello"));
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

        // ──────────────────────────────────────────────
        // StrConv Wide/Narrow
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrConv_Wide_ConvertASCIIToFullWidth()
        {
            // 'A'(0x41) + 0xFEE0 = 0xFF21 → '！' in full-width
            var result = Strings.StrConv("A", VbStrConv.Wide);
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual(0xFF21, (int)result[0]);
        }

        [TestMethod]
        public void StrConv_Narrow_ConvertFullWidthToASCII()
        {
            // full-width 'Ａ'(0xFF21) - 0xFEE0 = 0x41 = 'A'
            var result = Strings.StrConv("\uFF21", VbStrConv.Narrow);
            Assert.AreEqual("A", result);
        }

        [TestMethod]
        public void StrConv_Wide_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrConv("", VbStrConv.Wide));
        }

        [TestMethod]
        public void StrConv_Narrow_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrConv("", VbStrConv.Narrow));
        }

        // ──────────────────────────────────────────────
        // LenB
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LenB_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.LenB(""));
        }

        [TestMethod]
        public void LenB_ASCIIString_ReturnsSameAsLength()
        {
            Assert.AreEqual(5, Strings.LenB("hello"));
        }
    }
}
