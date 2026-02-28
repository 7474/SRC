using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// VB.Strings クラスのさらに詳細な追加テスト
    /// </summary>
    [TestClass]
    public class StringsMoreEdgeCaseTests
    {
        // ──────────────────────────────────────────────
        // StrComp
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrComp_EqualStrings_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.StrComp("hello", "hello"));
        }

        [TestMethod]
        public void StrComp_FirstSmaller_ReturnsMinusOne()
        {
            Assert.AreEqual(-1, Strings.StrComp("abc", "abd"));
        }

        [TestMethod]
        public void StrComp_FirstLarger_ReturnsOne()
        {
            Assert.AreEqual(1, Strings.StrComp("abd", "abc"));
        }

        [TestMethod]
        public void StrComp_EmptyStrings_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.StrComp("", ""));
        }

        [TestMethod]
        public void StrComp_EmptyAndNonEmpty_ReturnsMinusOne()
        {
            Assert.AreEqual(-1, Strings.StrComp("", "a"));
        }

        [TestMethod]
        public void StrComp_NonEmptyAndEmpty_ReturnsOne()
        {
            Assert.AreEqual(1, Strings.StrComp("a", ""));
        }

        // ──────────────────────────────────────────────
        // LenB
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LenB_AsciiString_ReturnsByteCount()
        {
            // ASCII文字: 1バイト = 2バイト (Unicode)
            var result = Strings.LenB("abc");
            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public void LenB_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.LenB(""));
        }

        [TestMethod]
        public void LenB_NullString_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.LenB(null));
        }

        // ──────────────────────────────────────────────
        // LeftB / RightB / MidB
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LeftB_ZeroBytes_ReturnsEmpty()
        {
            var result = Strings.LeftB("hello", 0);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void RightB_ZeroBytes_ReturnsEmpty()
        {
            var result = Strings.RightB("hello", 0);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void MidB_StartAtEnd_ReturnsEmpty()
        {
            // Len の後ろから開始
            var lenB = Strings.LenB("hello");
            var result = Strings.MidB("hello", lenB + 2);
            Assert.AreEqual("", result);
        }

        // ──────────────────────────────────────────────
        // Trim - 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Trim_LeadingSpaces_Removed()
        {
            Assert.AreEqual("hello", Strings.Trim("  hello"));
        }

        [TestMethod]
        public void Trim_TrailingSpaces_Removed()
        {
            Assert.AreEqual("hello", Strings.Trim("hello  "));
        }

        [TestMethod]
        public void Trim_BothSides_Removed()
        {
            Assert.AreEqual("hello", Strings.Trim("  hello  "));
        }

        [TestMethod]
        public void Trim_NoSpaces_ReturnsOriginal()
        {
            Assert.AreEqual("hello", Strings.Trim("hello"));
        }

        [TestMethod]
        public void Trim_AllSpaces_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Trim("   "));
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
        // LCase - 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LCase_UppercaseAscii_Lowercased()
        {
            Assert.AreEqual("hello", Strings.LCase("HELLO"));
        }

        [TestMethod]
        public void LCase_AlreadyLowercase_Unchanged()
        {
            Assert.AreEqual("hello", Strings.LCase("hello"));
        }

        [TestMethod]
        public void LCase_NullString_ThrowsNullReferenceException()
        {
            // VB の LCase は null に対して NullReferenceException をスローする
            Assert.Throws<System.NullReferenceException>(() => Strings.LCase(null));
        }

        [TestMethod]
        public void LCase_MixedCase_AllLowercased()
        {
            Assert.AreEqual("abcxyz", Strings.LCase("AbcXyz"));
        }

        // ──────────────────────────────────────────────
        // Len - 追加ケース
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
            Assert.AreEqual(3, Strings.Len("日本語"));
        }

        [TestMethod]
        public void Len_MixedString_ReturnsCharCount()
        {
            Assert.AreEqual(5, Strings.Len("ab日本c"));
        }

        [TestMethod]
        public void Len_SingleChar_ReturnsOne()
        {
            Assert.AreEqual(1, Strings.Len("X"));
        }

        // ──────────────────────────────────────────────
        // InStrB / InStrRevB - 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStrB_NotFound_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.InStrB("hello", "xyz"));
        }

        [TestMethod]
        public void InStrB_EmptyHaystack_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.InStrB("", "a"));
        }

        [TestMethod]
        public void InStrRevB_NotFound_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.InStrRevB("hello", "xyz"));
        }

        [TestMethod]
        public void InStrRevB_EmptyHaystack_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.InStrRevB("", "a"));
        }
    }
}
