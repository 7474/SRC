using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// Strings クラスの追加テスト（エッジケース）
    /// </summary>
    [TestClass]
    public class StringsAdditionalTests2
    {
        // ──────────────────────────────────────────────
        // InStr 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void InStr_StartPosBeyondString_ThrowsException()
        {
            _ = Strings.InStr(100, "hello", "l");
        }

        [TestMethod]
        public void InStr_SearchInEmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.InStr("", "a"));
        }

        [TestMethod]
        public void InStr_ExactMatch_ReturnsOne()
        {
            Assert.AreEqual(1, Strings.InStr("hello", "hello"));
        }

        [TestMethod]
        public void InStr_StartPos1_SameAsNoStartPos()
        {
            Assert.AreEqual(Strings.InStr("hello", "l"), Strings.InStr(1, "hello", "l"));
        }

        // ──────────────────────────────────────────────
        // Left 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void Left_NegativeLength_ThrowsException()
        {
            _ = Strings.Left("hello", -1);
        }

        [TestMethod]
        public void Left_OneChar_ReturnsFirstChar()
        {
            Assert.AreEqual("h", Strings.Left("hello", 1));
        }

        // ──────────────────────────────────────────────
        // Right 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void Right_NegativeLength_ThrowsException()
        {
            _ = Strings.Right("hello", -1);
        }

        [TestMethod]
        public void Right_OneChar_ReturnsLastChar()
        {
            Assert.AreEqual("o", Strings.Right("hello", 1));
        }

        // ──────────────────────────────────────────────
        // Mid 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Mid_NullInput_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Mid(null, 1, 3));
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void Mid_StartPosZero_ThrowsException()
        {
            _ = Strings.Mid("hello", 0, 2);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void Mid_StartPosNegative_ThrowsException()
        {
            _ = Strings.Mid("hello", -1, 2);
        }

        [TestMethod]
        public void Mid_NoLength_ReturnsFromPosition()
        {
            Assert.AreEqual("lo", Strings.Mid("hello", 4));
        }

        [TestMethod]
        public void Mid_StartAtEnd_ReturnsLastChar()
        {
            Assert.AreEqual("o", Strings.Mid("hello", 5));
        }

        // ──────────────────────────────────────────────
        // Len 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Len_NullString_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.Len(null));
        }

        [TestMethod]
        public void Len_OnlySpaces_ReturnsSpaceCount()
        {
            Assert.AreEqual(3, Strings.Len("   "));
        }

        [TestMethod]
        public void Len_SingleChar_ReturnsOne()
        {
            Assert.AreEqual(1, Strings.Len("a"));
        }

        // ──────────────────────────────────────────────
        // LCase 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LCase_AlreadyLower_ReturnsUnchanged()
        {
            Assert.AreEqual("hello", Strings.LCase("hello"));
        }

        [TestMethod]
        public void LCase_Mixed_ReturnsAllLower()
        {
            Assert.AreEqual("test123", Strings.LCase("TEST123"));
        }

        [TestMethod]
        public void LCase_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.LCase(""));
        }

        // ──────────────────────────────────────────────
        // Trim 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Trim_OnlySpaces_ReturnsEmpty()
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
        // Space 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Space_TwoSpaces_ReturnsTwoSpaces()
        {
            Assert.AreEqual("  ", Strings.Space(2));
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void Space_NegativeSpaces_ThrowsException()
        {
            _ = Strings.Space(-1);
        }

        // ──────────────────────────────────────────────
        // StrConv 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrConv_NarrowToWide_ConvertsToFullWidth()
        {
            var result = Strings.StrConv("ABC", VbStrConv.Wide);
            Assert.AreEqual("ＡＢＣ", result);
        }

        [TestMethod]
        public void StrConv_WideToNarrow_ConvertsToHalfWidth()
        {
            var result = Strings.StrConv("ＡＢＣ", VbStrConv.Narrow);
            Assert.AreEqual("ABC", result);
        }

        [TestMethod]
        public void StrConv_KatakanaToHiragana_Converts()
        {
            var result = Strings.StrConv("アイウ", VbStrConv.Hiragana);
            Assert.AreEqual("あいう", result);
        }

        // ──────────────────────────────────────────────
        // StrDup 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrDup_RepeatString_ReturnsRepeatedString()
        {
            Assert.AreEqual("abcabc", Strings.StrDup("abc", 2));
        }

        [TestMethod]
        public void StrDup_ZeroRepeat_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrDup("abc", 0));
        }

        [TestMethod]
        public void StrDup_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrDup("", 5));
        }

        // ──────────────────────────────────────────────
        // Asc 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Asc_UpperA_Returns65()
        {
            Assert.AreEqual(65, Strings.Asc("A"));
        }

        [TestMethod]
        public void Asc_LowerA_Returns97()
        {
            Assert.AreEqual(97, Strings.Asc("a"));
        }

        [TestMethod]
        public void Asc_CharOverload_ReturnsCode()
        {
            Assert.AreEqual(65, Strings.Asc('A'));
        }
    }
}
