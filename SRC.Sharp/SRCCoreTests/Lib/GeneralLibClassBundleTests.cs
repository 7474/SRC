using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// GeneralLib の FormatNum, InStrNotNest, LeftPaddedString, RightPaddedString の追加テスト
    /// </summary>
    [TestClass]
    public class GeneralLibMiscTests
    {
        // ──────────────────────────────────────────────
        // FormatNum - 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FormatNum_PositiveInteger_ReturnsIntegerString()
        {
            Assert.AreEqual("42", GeneralLib.FormatNum(42));
        }

        [TestMethod]
        public void FormatNum_Zero_ReturnsZeroString()
        {
            Assert.AreEqual("0", GeneralLib.FormatNum(0));
        }

        [TestMethod]
        public void FormatNum_NegativeInteger_ReturnsNegativeString()
        {
            Assert.AreEqual("-7", GeneralLib.FormatNum(-7));
        }

        [TestMethod]
        public void FormatNum_Decimal_ReturnsDecimalString()
        {
            var result = GeneralLib.FormatNum(3.14);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.StartsWith("3.14"), $"Expected to start with '3.14' but got '{result}'");
        }

        [TestMethod]
        public void FormatNum_LargeNumber_ReturnsCorrectString()
        {
            var result = GeneralLib.FormatNum(1000000);
            Assert.AreEqual("1000000", result);
        }

        [TestMethod]
        public void FormatNum_SmallDecimal_ReturnsCorrectString()
        {
            var result = GeneralLib.FormatNum(0.5);
            Assert.IsTrue(result.Contains("0.5") || result.Contains(".5"), $"Got: {result}");
        }

        // ──────────────────────────────────────────────
        // InStrNotNest - 追加テスト
        // InStrNotNest は 弱/効/剋 プレフィックスが前にある場合の文字をスキップする
        // 括弧はスキップしない
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStrNotNest_SimpleFind_ReturnsPosition()
        {
            var result = GeneralLib.InStrNotNest("abcdef", "d");
            Assert.AreEqual(4, result);
        }

        [TestMethod]
        public void InStrNotNest_EmptyHaystack_ReturnsZero()
        {
            var result = GeneralLib.InStrNotNest("", "a");
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void InStrNotNest_NotFound_ReturnsZero()
        {
            var result = GeneralLib.InStrNotNest("abcdef", "z");
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void InStrNotNest_MultipleOccurrences_ReturnsFirst()
        {
            var result = GeneralLib.InStrNotNest("xaxbxa", "a");
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void InStrNotNest_KouPrefix_SkipsKouPrefixed()
        {
            // "効" の直後の文字はスキップされる
            var result = GeneralLib.InStrNotNest("効炎 炎", "炎");
            // 最初の "炎" は "効" の直後なのでスキップされ、2番目の "炎" を返す
            Assert.AreEqual(4, result); // 2番目の "炎" は位置4
        }

        [TestMethod]
        public void InStrNotNest_JapaneseChar_FindsCorrectly()
        {
            var result = GeneralLib.InStrNotNest("火水木", "水");
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void InStrNotNest_AtStart_ReturnsOne()
        {
            var result = GeneralLib.InStrNotNest("abc", "a");
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void InStrNotNest_AtEnd_ReturnsLastPosition()
        {
            var result = GeneralLib.InStrNotNest("abc", "c");
            Assert.AreEqual(3, result);
        }

        // ──────────────────────────────────────────────
        // LeftPaddedString - 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LeftPaddedString_ShorterThanPadLength_PadsLeft()
        {
            var result = GeneralLib.LeftPaddedString("hi", 5);
            Assert.AreEqual(5, result.Length);
            Assert.IsTrue(result.EndsWith("hi"));
        }

        [TestMethod]
        public void LeftPaddedString_EmptyString_ReturnsPaddedSpaces()
        {
            var result = GeneralLib.LeftPaddedString("", 3);
            Assert.AreEqual(3, result.Length);
        }

        [TestMethod]
        public void LeftPaddedString_JapaneseString_PadsCorrectly()
        {
            var result = GeneralLib.LeftPaddedString("あ", 4);
            // 全角1文字は幅2なので、幅4に合わせて2スペースを左に追加
            Assert.IsTrue(result.EndsWith("あ"));
        }

        [TestMethod]
        public void LeftPaddedString_SingleAsciiChar_PadsToLength()
        {
            var result = GeneralLib.LeftPaddedString("X", 3);
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("  X", result);
        }

        // ──────────────────────────────────────────────
        // RightPaddedString - 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RightPaddedString_ShorterThanPadLength_PadsRight()
        {
            var result = GeneralLib.RightPaddedString("hi", 5);
            Assert.AreEqual(5, result.Length);
            Assert.IsTrue(result.StartsWith("hi"));
        }

        [TestMethod]
        public void RightPaddedString_EmptyString_ReturnsPaddedSpaces()
        {
            var result = GeneralLib.RightPaddedString("", 3);
            Assert.AreEqual(3, result.Length);
        }

        [TestMethod]
        public void RightPaddedString_ExactLength_NoPaddingAdded()
        {
            var result = GeneralLib.RightPaddedString("abc", 3);
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("abc", result);
        }

        [TestMethod]
        public void RightPaddedString_SingleAsciiChar_PadsToLength()
        {
            var result = GeneralLib.RightPaddedString("X", 3);
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual("X  ", result);
        }

        [TestMethod]
        public void RightPaddedString_JapaneseString_PadsCorrectly()
        {
            var result = GeneralLib.RightPaddedString("あ", 4);
            // 全角1文字は幅2なので、幅4に合わせて2スペースを右に追加
            Assert.IsTrue(result.StartsWith("あ"));
        }

        // ──────────────────────────────────────────────
        // GetClassBundle - 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetClassBundle_KouPrefix_Returns効AndChar()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("効炎", ref idx);
            Assert.AreEqual("効炎", result);
        }

        [TestMethod]
        public void GetClassBundle_KokuPrefix_Returns剋AndChar()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("剋水", ref idx);
            Assert.AreEqual("剋水", result);
        }

        [TestMethod]
        public void GetClassBundle_SimpleChar_ReturnsSingleChar()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("火水木", ref idx);
            Assert.AreEqual("火", result);
        }
    }
}
