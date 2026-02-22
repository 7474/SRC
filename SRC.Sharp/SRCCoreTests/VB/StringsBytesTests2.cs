using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// Strings クラスの RightB / MidB の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class StringsBytesTests2
    {
        // ──────────────────────────────────────────────
        // RightB
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RightB_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.RightB("", 3));
        }

        [TestMethod]
        public void RightB_Null_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.RightB(null, 3));
        }

        [TestMethod]
        public void RightB_ZeroByteCount_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.RightB("hello", 0));
        }

        [TestMethod]
        public void RightB_NegativeByteCount_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.RightB("hello", -1));
        }

        [TestMethod]
        public void RightB_AsciiOnly_ReturnsLastNBytes()
        {
            // "hello" の末尾3バイト = "llo"
            Assert.AreEqual("llo", Strings.RightB("hello", 3));
        }

        [TestMethod]
        public void RightB_AsciiOnly_LargerThanLength_ReturnsFullString()
        {
            Assert.AreEqual("hello", Strings.RightB("hello", 10));
        }

        [TestMethod]
        public void RightB_AsciiOnly_ExactLength_ReturnsFullString()
        {
            Assert.AreEqual("hello", Strings.RightB("hello", 5));
        }

        [TestMethod]
        public void RightB_JapaneseChars_ReturnsTwoBytes()
        {
            // "あいう" = 6bytes; 末尾2バイト = "う"
            var result = Strings.RightB("あいう", 2);
            Assert.AreEqual("う", result);
        }

        [TestMethod]
        public void RightB_JapaneseChars_Return4Bytes_ReturnsLast2Chars()
        {
            // "あいう" = 6bytes; 末尾4バイト = "いう"
            var result = Strings.RightB("あいう", 4);
            Assert.AreEqual("いう", result);
        }

        // ──────────────────────────────────────────────
        // MidB (2-arg)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MidB_AsciiOnly_FromStart_ReturnsFullString()
        {
            Assert.AreEqual("hello", Strings.MidB("hello", 1));
        }

        [TestMethod]
        public void MidB_AsciiOnly_FromMiddle_ReturnsSubstring()
        {
            // "hello", startByte=3 → "llo"
            Assert.AreEqual("llo", Strings.MidB("hello", 3));
        }

        [TestMethod]
        public void MidB_Null_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.MidB(null, 1));
        }

        [TestMethod]
        public void MidB_StartBeyondLength_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.MidB("hello", 10));
        }

        // ──────────────────────────────────────────────
        // MidB (3-arg)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MidB3_AsciiOnly_ReturnsCorrectSubstring()
        {
            // "hello", start=2, count=3 → "ell"
            Assert.AreEqual("ell", Strings.MidB("hello", 2, 3));
        }

        [TestMethod]
        public void MidB3_ZeroByteCount_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.MidB("hello", 1, 0));
        }

        [TestMethod]
        public void MidB3_ExceedsLength_ReturnsTruncated()
        {
            // "hello", start=4, count=10 → "lo"
            Assert.AreEqual("lo", Strings.MidB("hello", 4, 10));
        }

        [TestMethod]
        public void MidB3_JapaneseChars_ReturnsFirstChar()
        {
            // "あいう" = 6 bytes; start=1, count=2 → "あ"
            Assert.AreEqual("あ", Strings.MidB("あいう", 1, 2));
        }
    }
}
