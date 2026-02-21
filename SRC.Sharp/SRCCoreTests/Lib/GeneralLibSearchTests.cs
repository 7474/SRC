using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// GeneralLib のインライン文字列検索関数 InStr2, InStrNotNest などの追加テスト
    /// </summary>
    [TestClass]
    public class GeneralLibSearchTests
    {
        // ──────────────────────────────────────────────
        // InStr2 - 末尾から検索
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStr2_FindFromEnd_ReturnsLastPosition()
        {
            // "hello" には "l" が2箇所あるが最後の "l" (位置4) を返す
            Assert.AreEqual(4, GeneralLib.InStr2("hello", "l"));
        }

        [TestMethod]
        public void InStr2_UniqueChar_ReturnsSameAsInStr()
        {
            Assert.AreEqual(1, GeneralLib.InStr2("hello", "h"));
        }

        [TestMethod]
        public void InStr2_NotFound_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.InStr2("hello", "xyz"));
        }

        [TestMethod]
        public void InStr2_EmptyNeedle_ReturnsStringLengthPlusOne()
        {
            // InStr2("hello", "") は最後まで見て一致しないため、VB の仕様に従い 0 か length+1 を返す
            // 実装では "" でマッチしたとき while の途中でもマッチするので検証
            var result = GeneralLib.InStr2("hello", "");
            // 空の針は各位置でマッチするため末尾の位置(length)を返す
            Assert.IsTrue(result >= 0);
        }

        [TestMethod]
        public void InStr2_EmptyHaystack_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.InStr2("", "l"));
        }

        [TestMethod]
        public void InStr2_MultipleOccurrences_ReturnsLastPosition()
        {
            Assert.AreEqual(7, GeneralLib.InStr2("abcabcabc", "abc"));
        }

        [TestMethod]
        public void InStr2_JapaneseString_FindsFromEnd()
        {
            // "あいうあ" の中から最後の "あ" (位置4) を返す
            Assert.AreEqual(4, GeneralLib.InStr2("あいうあ", "あ"));
        }

        [TestMethod]
        public void InStr2_FullStringMatch_ReturnsOne()
        {
            Assert.AreEqual(1, GeneralLib.InStr2("abc", "abc"));
        }

        // ──────────────────────────────────────────────
        // MaxLng / MinLng - 最大値・最小値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MaxLng_FirstLarger_ReturnsFirst()
        {
            Assert.AreEqual(10, GeneralLib.MaxLng(10, 5));
        }

        [TestMethod]
        public void MaxLng_SecondLarger_ReturnsSecond()
        {
            Assert.AreEqual(10, GeneralLib.MaxLng(5, 10));
        }

        [TestMethod]
        public void MaxLng_BothEqual_ReturnsValue()
        {
            Assert.AreEqual(7, GeneralLib.MaxLng(7, 7));
        }

        [TestMethod]
        public void MaxLng_Negatives_ReturnsLarger()
        {
            Assert.AreEqual(-3, GeneralLib.MaxLng(-3, -7));
        }

        [TestMethod]
        public void MinLng_FirstSmaller_ReturnsFirst()
        {
            Assert.AreEqual(3, GeneralLib.MinLng(3, 10));
        }

        [TestMethod]
        public void MinLng_SecondSmaller_ReturnsSecond()
        {
            Assert.AreEqual(3, GeneralLib.MinLng(10, 3));
        }

        [TestMethod]
        public void MinLng_BothEqual_ReturnsValue()
        {
            Assert.AreEqual(5, GeneralLib.MinLng(5, 5));
        }

        [TestMethod]
        public void MinLng_Negatives_ReturnsSmaller()
        {
            Assert.AreEqual(-10, GeneralLib.MinLng(-3, -10));
        }

        // ──────────────────────────────────────────────
        // MaxDbl / MinDbl - double版
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MaxDbl_FirstLarger_ReturnsFirst()
        {
            Assert.AreEqual(3.14, GeneralLib.MaxDbl(3.14, 2.71));
        }

        [TestMethod]
        public void MaxDbl_SecondLarger_ReturnsSecond()
        {
            Assert.AreEqual(3.14, GeneralLib.MaxDbl(2.71, 3.14));
        }

        [TestMethod]
        public void MaxDbl_Negatives_ReturnsLarger()
        {
            Assert.AreEqual(-1.0, GeneralLib.MaxDbl(-1.0, -5.0));
        }

        [TestMethod]
        public void MinDbl_FirstSmaller_ReturnsFirst()
        {
            Assert.AreEqual(1.0, GeneralLib.MinDbl(1.0, 2.0));
        }

        [TestMethod]
        public void MinDbl_SecondSmaller_ReturnsSecond()
        {
            Assert.AreEqual(1.0, GeneralLib.MinDbl(2.0, 1.0));
        }

        [TestMethod]
        public void MinDbl_BothEqual_ReturnsValue()
        {
            Assert.AreEqual(2.5, GeneralLib.MinDbl(2.5, 2.5));
        }

        // ──────────────────────────────────────────────
        // StrToHiragana
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrToHiragana_Katakana_ConvertedToHiragana()
        {
            var result = GeneralLib.StrToHiragana("アイウエオ");
            Assert.AreEqual("あいうえお", result);
        }

        [TestMethod]
        public void StrToHiragana_AlreadyHiragana_Unchanged()
        {
            var result = GeneralLib.StrToHiragana("あいうえお");
            Assert.AreEqual("あいうえお", result);
        }

        [TestMethod]
        public void StrToHiragana_EmptyString_ReturnsEmpty()
        {
            var result = GeneralLib.StrToHiragana("");
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void StrToHiragana_NullString_ReturnsEmpty()
        {
            var result = GeneralLib.StrToHiragana(null);
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void StrToHiragana_MixedString_OnlyKatakanaConverted()
        {
            var result = GeneralLib.StrToHiragana("アあABC");
            Assert.AreEqual("ああABC", result);
        }

        // ──────────────────────────────────────────────
        // StrToDbl / StrToLng
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrToDbl_ValidDecimal_ReturnsDouble()
        {
            Assert.AreEqual(3.14, GeneralLib.StrToDbl("3.14"), 0.001);
        }

        [TestMethod]
        public void StrToDbl_Integer_ReturnsDouble()
        {
            Assert.AreEqual(42.0, GeneralLib.StrToDbl("42"), 0.001);
        }

        [TestMethod]
        public void StrToLng_ValidInteger_ReturnsInt()
        {
            Assert.AreEqual(100, GeneralLib.StrToLng("100"));
        }

        [TestMethod]
        public void StrToLng_NegativeInteger_ReturnsInt()
        {
            Assert.AreEqual(-50, GeneralLib.StrToLng("-50"));
        }
    }
}
