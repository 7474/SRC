using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// GeneralLib.GetClassBundle / InStrNotNest のユニットテスト
    /// </summary>
    [TestClass]
    public class GeneralLibGetClassBundleTests
    {
        // ──────────────────────────────────────────────
        // GetClassBundle - 基本動作
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetClassBundle_SingleChar_ReturnsThatChar()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("炎", ref idx);
            Assert.AreEqual("炎", result);
        }

        [TestMethod]
        public void GetClassBundle_WithWeak_ReturnsTwoChars()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("弱炎", ref idx);
            Assert.AreEqual("弱炎", result);
        }

        [TestMethod]
        public void GetClassBundle_WithEffect_ReturnsTwoChars()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("効炎", ref idx);
            Assert.AreEqual("効炎", result);
        }

        [TestMethod]
        public void GetClassBundle_WithBeat_ReturnsTwoChars()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("剋炎", ref idx);
            Assert.AreEqual("剋炎", result);
        }

        [TestMethod]
        public void GetClassBundle_NormalChar_ReturnsIt()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("A", ref idx);
            Assert.AreEqual("A", result);
        }

        [TestMethod]
        public void GetClassBundle_StartFromMiddle_ReturnsCharAtIndex()
        {
            int idx = 3;
            var result = GeneralLib.GetClassBundle("炎水弱土", ref idx);
            Assert.AreEqual("弱土", result);
        }

        [TestMethod]
        public void GetClassBundle_LengthOne_ReturnsSingleChar()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("弱炎", ref idx, 1);
            Assert.AreEqual("弱", result);
        }

        [TestMethod]
        public void GetClassBundle_WithLow_ReturnsTwoChars()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("低攻", ref idx);
            Assert.AreEqual("低攻", result);
        }

        [TestMethod]
        public void GetClassBundle_WithLowBo_ReturnsTwoChars()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("低防", ref idx);
            Assert.AreEqual("低防", result);
        }

        // ──────────────────────────────────────────────
        // InStrNotNest - 基本動作
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStrNotNest_NoMatch_ReturnsZero()
        {
            var result = GeneralLib.InStrNotNest("炎水土", "風");
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void InStrNotNest_DirectMatch_ReturnsPosition()
        {
            var result = GeneralLib.InStrNotNest("炎水土", "水");
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void InStrNotNest_FirstChar_ReturnsOne()
        {
            var result = GeneralLib.InStrNotNest("炎水土", "炎");
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void InStrNotNest_AfterWeak_ReturnsZero()
        {
            // "弱炎" の場合、炎は弱効剋の後なので通常の炎とは別扱い
            var result = GeneralLib.InStrNotNest("弱炎", "炎");
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void InStrNotNest_AfterEffect_ReturnsZero()
        {
            var result = GeneralLib.InStrNotNest("効炎", "炎");
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void InStrNotNest_AfterBeat_ReturnsZero()
        {
            var result = GeneralLib.InStrNotNest("剋炎", "炎");
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void InStrNotNest_WithStart_SearchesFromPosition()
        {
            // "炎炎" で start=2 から検索すると2番目の位置を返す
            var result = GeneralLib.InStrNotNest("炎炎", "炎", 2);
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void InStrNotNest_EmptySearchString_ReturnsStartPosition()
        {
            var result = GeneralLib.InStrNotNest("炎水土", "");
            // 空文字列の場合は先頭に一致
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void InStrNotNest_NullFirstString_ReturnsZero()
        {
            var result = GeneralLib.InStrNotNest(null, "炎");
            Assert.AreEqual(0, result);
        }
    }
}
