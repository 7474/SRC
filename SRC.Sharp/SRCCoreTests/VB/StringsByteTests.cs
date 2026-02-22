using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// Strings クラスの InStr / InStrRev / LenB / LeftB / RightB / MidB 等のさらなるテスト
    /// </summary>
    [TestClass]
    public class StringsByteTests
    {
        // ──────────────────────────────────────────────
        // LenB (Shift-JISバイト長)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LenB_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.LenB(""));
        }

        [TestMethod]
        public void LenB_Null_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.LenB(null));
        }

        [TestMethod]
        public void LenB_AsciiOnly_ReturnsCharCount()
        {
            // ASCII文字は1バイト
            Assert.AreEqual(5, Strings.LenB("hello"));
        }

        [TestMethod]
        public void LenB_JapaneseChars_Returns2BytesEach()
        {
            // 全角文字はShift-JISで2バイト
            Assert.AreEqual(6, Strings.LenB("あいう"));
        }

        [TestMethod]
        public void LenB_MixedChars_ReturnsCorrectByteCount()
        {
            // "a" (1byte) + "あ" (2bytes) = 3 bytes
            Assert.AreEqual(3, Strings.LenB("aあ"));
        }

        // ──────────────────────────────────────────────
        // LeftB (バイトベースのLeft)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LeftB_AsciiOnly_ReturnsFirstNChars()
        {
            // ASCII のみ: "hello" の先頭3バイト = "hel"
            Assert.AreEqual("hel", Strings.LeftB("hello", 3));
        }

        [TestMethod]
        public void LeftB_Zero_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.LeftB("hello", 0));
        }

        [TestMethod]
        public void LeftB_Null_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.LeftB(null, 3));
        }

        [TestMethod]
        public void LeftB_LargerThanLength_ReturnsFullString()
        {
            Assert.AreEqual("abc", Strings.LeftB("abc", 10));
        }

        // ──────────────────────────────────────────────
        // InStr (追加テスト)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStr_SingleChar_FindsPosition()
        {
            Assert.AreEqual(3, Strings.InStr("hello", "l"));
        }

        [TestMethod]
        public void InStr_MultiChar_FindsPosition()
        {
            Assert.AreEqual(2, Strings.InStr("hello", "ell"));
        }

        [TestMethod]
        public void InStr_NotFound_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.InStr("hello", "xyz"));
        }

        [TestMethod]
        public void InStr_EmptySearchString_ReturnsOne()
        {
            Assert.AreEqual(1, Strings.InStr("hello", ""));
        }

        [TestMethod]
        public void InStr_EmptySource_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.InStr("", "hello"));
        }

        [TestMethod]
        public void InStr_NullSource_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.InStr(null, "hello"));
        }

        [TestMethod]
        public void InStr_WithStart_StartPosition()
        {
            // "hello" の2番目から "l" を探す → 3番目
            Assert.AreEqual(3, Strings.InStr(2, "hello", "l"));
        }

        [TestMethod]
        public void InStr_WithStart_SkipsFirst()
        {
            // 4番目から "l" を探す → 4番目の "l"
            Assert.AreEqual(4, Strings.InStr(4, "hello", "l"));
        }

        // ──────────────────────────────────────────────
        // LCase (char)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LCase_UppercaseChar_ReturnsLowercase()
        {
            Assert.AreEqual('a', Strings.LCase('A'));
        }

        [TestMethod]
        public void LCase_LowercaseChar_ReturnsSame()
        {
            Assert.AreEqual('a', Strings.LCase('a'));
        }

        [TestMethod]
        public void LCase_Digit_ReturnsSame()
        {
            Assert.AreEqual('1', Strings.LCase('1'));
        }
    }
}
