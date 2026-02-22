using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// 文字列B系関数（バイト単位操作）のエッジケーステスト
    /// </summary>
    [TestClass]
    public class StringBFunctionEdgeCaseTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // LenB
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LenB_AsciiString_ReturnsCharCount()
        {
            var exp = Create();
            // 半角5文字 = 5バイト (Shift_JIS)
            Assert.AreEqual(5d, exp.GetValueAsDouble("LenB(\"hello\")"));
        }

        [TestMethod]
        public void LenB_EmptyString_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("LenB(\"\")"));
        }

        [TestMethod]
        public void LenB_MixedWidthString_ReturnsByteCount()
        {
            var exp = Create();
            // "aあ" = 1 + 2 = 3バイト (Shift_JIS)
            Assert.AreEqual(3d, exp.GetValueAsDouble("LenB(\"aあ\")"));
        }

        [TestMethod]
        public void LenB_StringReturn_ReturnsFormattedNumber()
        {
            var exp = Create();
            Assert.AreEqual("5", exp.GetValueAsString("LenB(\"hello\")"));
        }

        // ──────────────────────────────────────────────
        // LeftB
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LeftB_AsciiString_ReturnsPrefix()
        {
            var exp = Create();
            Assert.AreEqual("hel", exp.GetValueAsString("LeftB(\"hello\",3)"));
        }

        [TestMethod]
        public void LeftB_ZeroBytes_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("LeftB(\"hello\",0)"));
        }

        // ──────────────────────────────────────────────
        // RightB
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RightB_AsciiString_ReturnsSuffix()
        {
            var exp = Create();
            Assert.AreEqual("llo", exp.GetValueAsString("RightB(\"hello\",3)"));
        }

        [TestMethod]
        public void RightB_ZeroBytes_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("RightB(\"hello\",0)"));
        }

        // ──────────────────────────────────────────────
        // MidB with 2 args (no length)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MidB_TwoArgs_ReturnsFromBytePosition()
        {
            var exp = Create();
            // "hello" の3バイト目以降 = "llo"
            Assert.AreEqual("llo", exp.GetValueAsString("MidB(\"hello\",3)"));
        }

        [TestMethod]
        public void MidB_TwoArgs_FullWidth_ReturnsFromBytePosition()
        {
            var exp = Create();
            // "あいう" の3バイト目以降 = "いう"
            Assert.AreEqual("いう", exp.GetValueAsString("MidB(\"あいう\",3)"));
        }

        [TestMethod]
        public void MidB_ThreeArgs_AsciiString()
        {
            var exp = Create();
            // "hello" の2バイト目から3バイト = "ell"
            Assert.AreEqual("ell", exp.GetValueAsString("MidB(\"hello\",2,3)"));
        }

        // ──────────────────────────────────────────────
        // InStrB with start position
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStrB_AsciiString_ReturnsPosition()
        {
            var exp = Create();
            // "hello" で "l" の最初の位置 = 3バイト目
            Assert.AreEqual(3d, exp.GetValueAsDouble("InStrB(\"hello\",\"l\")"));
        }

        [TestMethod]
        public void InStrB_NotFound_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("InStrB(\"hello\",\"z\")"));
        }

        [TestMethod]
        public void InStrB_WithStartPosition_SearchesFromPosition()
        {
            var exp = Create();
            // "hello" で "l" を4バイト目から検索 → 4バイト目
            Assert.AreEqual(4d, exp.GetValueAsDouble("InStrB(\"hello\",\"l\",4)"));
        }

        // ──────────────────────────────────────────────
        // InStrRevB
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStrRevB_AsciiString_ReturnsLastPosition()
        {
            var exp = Create();
            // "hello" で "l" の最後のバイト位置 = 4
            Assert.AreEqual(4d, exp.GetValueAsDouble("InStrRevB(\"hello\",\"l\")"));
        }

        [TestMethod]
        public void InStrRevB_NotFound_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("InStrRevB(\"hello\",\"z\")"));
        }

        [TestMethod]
        public void InStrRevB_WithStartPosition_LimitsSearch()
        {
            var exp = Create();
            // "hello" で "l" を3バイト目までで検索 → 3バイト目
            Assert.AreEqual(3d, exp.GetValueAsDouble("InStrRevB(\"hello\",\"l\",3)"));
        }
    }
}
