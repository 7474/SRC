using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// GeneralLib の InStrNotNest / GetClassBundle / ToList の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class GeneralLibSearchMoreTests3
    {
        // ──────────────────────────────────────────────
        // InStrNotNest の追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStrNotNest_SimpleAscii_FindsPosition()
        {
            Assert.AreEqual(3, GeneralLib.InStrNotNest("abcde", "c"));
        }

        [TestMethod]
        public void InStrNotNest_AfterKou_SkipsOccurrence()
        {
            // "効炎" において "炎" の前が "効" なのでスキップ → 0
            Assert.AreEqual(0, GeneralLib.InStrNotNest("効炎", "炎"));
        }

        [TestMethod]
        public void InStrNotNest_AfterKoku_SkipsOccurrence()
        {
            // "剋炎" において "炎" の前が "剋" なのでスキップ → 0
            Assert.AreEqual(0, GeneralLib.InStrNotNest("剋炎", "炎"));
        }

        [TestMethod]
        public void InStrNotNest_TwoOccurrences_FirstPrecededByWeak_ReturnsSecond()
        {
            // "弱炎水炎" → 最初の炎(位置2)は弱の後なのでスキップ、次の炎(位置4)を返す
            var result = GeneralLib.InStrNotNest("弱炎水炎", "炎");
            Assert.AreEqual(4, result);
        }

        [TestMethod]
        public void InStrNotNest_StartParam_StartsFromGivenPosition()
        {
            // "abcabc" の position 4 から "c" を探す → 位置 6
            Assert.AreEqual(6, GeneralLib.InStrNotNest("abcabc", "c", 4));
        }

        [TestMethod]
        public void InStrNotNest_EmptyNeedle_ReturnsStart()
        {
            // 空文字列の検索は start (1) を返す
            Assert.AreEqual(1, GeneralLib.InStrNotNest("abc", ""));
        }

        [TestMethod]
        public void InStrNotNest_NullHaystack_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.InStrNotNest(null, "a"));
        }

        // ──────────────────────────────────────────────
        // GetClassBundle の追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetClassBundle_EmptyString_ReturnsEmpty()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("", ref idx);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void GetClassBundle_SingleKanji_ReturnsSingleKanji()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("水", ref idx);
            Assert.AreEqual("水", result);
        }

        [TestMethod]
        public void GetClassBundle_KouPrefix_ReturnsTwoChars()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("効水", ref idx);
            Assert.AreEqual("効水", result);
        }

        [TestMethod]
        public void GetClassBundle_KokuPrefix_ReturnsTwoChars()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("剋土", ref idx);
            Assert.AreEqual("剋土", result);
        }

        [TestMethod]
        public void GetClassBundle_StartFromSecondChar_SkipsFirstChar()
        {
            // "炎水" の2文字目から → "水"
            int idx = 2;
            var result = GeneralLib.GetClassBundle("炎水", ref idx);
            Assert.AreEqual("水", result);
        }

        // ──────────────────────────────────────────────
        // ToL vs ToList の差異
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToL_SpaceSeparated_ReturnsSimpleTokens()
        {
            var result = GeneralLib.ToL("a b c");
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void ToList_WithParentheses_TreatsParensAsOneToken()
        {
            // ToList は括弧内のスペースを区切りとして扱わない
            var result = GeneralLib.ToList("a (b c) d");
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("(b c)", result[1]);
        }

        [TestMethod]
        public void ToL_WithParentheses_TreatsParensContentAsMultipleTokens()
        {
            // ToL は括弧を考慮しない
            var result = GeneralLib.ToL("a (b c) d");
            // "a", "(b", "c)", "d" の4つ
            Assert.AreEqual(4, result.Count);
        }
    }
}
