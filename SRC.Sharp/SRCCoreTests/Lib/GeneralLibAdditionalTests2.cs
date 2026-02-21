using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using System.Linq;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// GeneralLib の追加エッジケーステスト（未カバーパス向け）
    /// </summary>
    [TestClass]
    public class GeneralLibAdditionalTests2
    {
        // ──────────────────────────────────────────────
        // FormatNum のエッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FormatNum_Zero_ReturnsZeroString()
        {
            Assert.AreEqual("0", GeneralLib.FormatNum(0d));
        }

        [TestMethod]
        public void FormatNum_NegativeInteger_ReturnsNegativeString()
        {
            Assert.AreEqual("-100", GeneralLib.FormatNum(-100d));
        }

        [TestMethod]
        public void FormatNum_LargeInteger_NoExponent()
        {
            Assert.AreEqual("1000000", GeneralLib.FormatNum(1000000d));
        }

        [TestMethod]
        public void FormatNum_SmallDecimal_NoExponent()
        {
            var result = GeneralLib.FormatNum(0.001d);
            Assert.IsFalse(result.Contains("E"), $"Expected no exponent in '{result}'");
            Assert.AreEqual("0.001", result);
        }

        [TestMethod]
        public void FormatNum_Half_ReturnsHalfString()
        {
            Assert.AreEqual("0.5", GeneralLib.FormatNum(0.5d));
        }

        [TestMethod]
        public void FormatNum_NegativeDecimal_ReturnsNegativeDecimalString()
        {
            var result = GeneralLib.FormatNum(-3.14d);
            Assert.IsTrue(result.StartsWith("-"), $"Expected '-' prefix in '{result}'");
        }

        [TestMethod]
        public void FormatNum_WholeNumberDouble_NoDecimalPoint()
        {
            var result = GeneralLib.FormatNum(42.0d);
            Assert.IsFalse(result.Contains("."), $"Whole number should have no decimal point: '{result}'");
            Assert.AreEqual("42", result);
        }

        // ──────────────────────────────────────────────
        // StrWidth のエッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrWidth_JapaneseKatakana_CountsAsWide()
        {
            // 全角カタカナは2文字換算
            Assert.AreEqual(4, GeneralLib.StrWidth("アイ"));
        }

        [TestMethod]
        public void StrWidth_HalfWidthKatakana_CountsAsHalf()
        {
            // 半角カタカナは1文字換算
            Assert.AreEqual(3, GeneralLib.StrWidth("ｱｲｳ"));
        }

        [TestMethod]
        public void StrWidth_MixedHalfAndFullWidth()
        {
            // "Ａ" (全角) = 2 + "B" (半角) = 1 → 計3
            Assert.AreEqual(3, GeneralLib.StrWidth("ＡB"));
        }

        [TestMethod]
        public void StrWidth_Numbers_CountAsHalf()
        {
            // ASCII数字は半角1文字換算
            Assert.AreEqual(4, GeneralLib.StrWidth("1234"));
        }

        [TestMethod]
        public void StrWidth_SingleHiragana_IsTwoWidth()
        {
            Assert.AreEqual(2, GeneralLib.StrWidth("あ"));
        }

        [TestMethod]
        public void StrWidth_SingleAscii_IsOneWidth()
        {
            Assert.AreEqual(1, GeneralLib.StrWidth("a"));
        }

        // ──────────────────────────────────────────────
        // IsNumber のエッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsNumber_PositiveInteger_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsNumber("100"));
        }

        [TestMethod]
        public void IsNumber_NegativeInteger_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsNumber("-50"));
        }

        [TestMethod]
        public void IsNumber_PositiveFloat_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsNumber("1.23"));
        }

        [TestMethod]
        public void IsNumber_Zero_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsNumber("0"));
        }

        [TestMethod]
        public void IsNumber_Null_ReturnsFalse()
        {
            Assert.IsFalse(GeneralLib.IsNumber(null));
        }

        [TestMethod]
        public void IsNumber_JapaneseString_ReturnsFalse()
        {
            Assert.IsFalse(GeneralLib.IsNumber("テスト"));
        }

        [TestMethod]
        public void IsNumber_LettersOnly_ReturnsFalse()
        {
            Assert.IsFalse(GeneralLib.IsNumber("abc"));
        }

        [TestMethod]
        public void IsNumber_ParenthesizedNumber_ReturnsFalse()
        {
            // 括弧付きはIsNumber=falseであることを確認
            Assert.IsFalse(GeneralLib.IsNumber("(1)"));
        }

        // ──────────────────────────────────────────────
        // InStr2 のエッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStr2_EmptySearchString_ReturnsCorrectPosition()
        {
            // 空文字列の場合、末尾から検索するとLen(str) - 0 + 1 = Len(str)+1 = 6 が返る
            var result = GeneralLib.InStr2("hello", "");
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void InStr2_SingleCharOccurrence_ReturnsLastPosition()
        {
            // "abcabc" に "a" が1番目と4番目 → 末尾検索で4
            Assert.AreEqual(4, GeneralLib.InStr2("abcabc", "a"));
        }

        [TestMethod]
        public void InStr2_SubstringAtEnd_ReturnsEndPosition()
        {
            // "helloworld" の "rld" は8文字目から(r=8,l=9,d=10)
            Assert.AreEqual(8, GeneralLib.InStr2("helloworld", "rld"));
        }

        [TestMethod]
        public void InStr2_SubstringAtBeginning_ReturnsBeginning()
        {
            // "hello" に "hel" が1番目のみ
            Assert.AreEqual(1, GeneralLib.InStr2("hello", "hel"));
        }

        [TestMethod]
        public void InStr2_NotFound_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.InStr2("hello", "xyz"));
        }

        [TestMethod]
        public void InStr2_FullMatchAtEnd_ReturnsPosition()
        {
            Assert.AreEqual(4, GeneralLib.InStr2("abcabc", "abc"));
        }

        // ──────────────────────────────────────────────
        // MaxLng / MinLng のエッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MaxLng_BothNegative_ReturnsLessNegative()
        {
            Assert.AreEqual(-1, GeneralLib.MaxLng(-1, -5));
        }

        [TestMethod]
        public void MinLng_BothNegative_ReturnsMoreNegative()
        {
            Assert.AreEqual(-5, GeneralLib.MinLng(-1, -5));
        }

        [TestMethod]
        public void MaxLng_SameValues_ReturnsSameValue()
        {
            Assert.AreEqual(7, GeneralLib.MaxLng(7, 7));
        }

        [TestMethod]
        public void MinLng_SameValues_ReturnsSameValue()
        {
            Assert.AreEqual(7, GeneralLib.MinLng(7, 7));
        }

        [TestMethod]
        public void MaxLng_MinValue_ReturnsMaxValue()
        {
            Assert.AreEqual(int.MaxValue, GeneralLib.MaxLng(int.MinValue, int.MaxValue));
        }

        // ──────────────────────────────────────────────
        // LeftPaddedString / RightPaddedString のエッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LeftPaddedString_JapaneseString_PadsCorrectly()
        {
            // "あ" はstrWidth=2、length=4なら2スペース左に追加
            var result = GeneralLib.LeftPaddedString("あ", 4);
            Assert.AreEqual(4, GeneralLib.StrWidth(result));
        }

        [TestMethod]
        public void RightPaddedString_JapaneseString_PadsCorrectly()
        {
            var result = GeneralLib.RightPaddedString("あ", 4);
            Assert.AreEqual(4, GeneralLib.StrWidth(result));
        }

        [TestMethod]
        public void LeftPaddedString_EmptyString_PadsAllSpaces()
        {
            var result = GeneralLib.LeftPaddedString("", 3);
            Assert.AreEqual("   ", result);
        }

        [TestMethod]
        public void RightPaddedString_EmptyString_PadsAllSpaces()
        {
            var result = GeneralLib.RightPaddedString("", 3);
            Assert.AreEqual("   ", result);
        }

        // ──────────────────────────────────────────────
        // GetClassBundle のエッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetClassBundle_EffPrefix_ReturnsTwoChars()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("効炎", ref idx);
            Assert.AreEqual("効炎", result);
            Assert.AreEqual(2, idx);
        }

        [TestMethod]
        public void GetClassBundle_KirimPrefix_ReturnsTwoChars()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("剋炎", ref idx);
            Assert.AreEqual("剋炎", result);
            Assert.AreEqual(2, idx);
        }

        [TestMethod]
        public void GetClassBundle_DoublePrefix_ReturnsThreeChars()
        {
            // 弱効炎 → 弱→効→炎 で3文字
            int idx = 1;
            var result = GeneralLib.GetClassBundle("弱効炎", ref idx);
            Assert.AreEqual("弱効炎", result);
            Assert.AreEqual(3, idx);
        }

        [TestMethod]
        public void GetClassBundle_LowDefPrefix_ReturnsCorrect()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("低防", ref idx);
            Assert.AreEqual("低防", result);
            Assert.AreEqual(2, idx);
        }

        [TestMethod]
        public void GetClassBundle_LowMovePrefix_ReturnsCorrect()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("低移", ref idx);
            Assert.AreEqual("低移", result);
            Assert.AreEqual(2, idx);
        }

        // ──────────────────────────────────────────────
        // InStrNotNest のエッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStrNotNest_TargetAtStart_ReturnsOne()
        {
            Assert.AreEqual(1, GeneralLib.InStrNotNest("炎水", "炎"));
        }

        [TestMethod]
        public void InStrNotNest_TargetNotPresent_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.InStrNotNest("水風", "炎"));
        }

        [TestMethod]
        public void InStrNotNest_TargetAfterNormal_ReturnsPosition()
        {
            Assert.AreEqual(2, GeneralLib.InStrNotNest("水炎", "炎"));
        }

        [TestMethod]
        public void InStrNotNest_TargetWithKirim_IsNested()
        {
            // 「剋炎」の「炎」はネストなので見つからない
            Assert.AreEqual(0, GeneralLib.InStrNotNest("剋炎", "炎"));
        }

        [TestMethod]
        public void InStrNotNest_TargetWithKoka_IsNested()
        {
            // 「効炎」の「炎」はネストなので見つからない
            Assert.AreEqual(0, GeneralLib.InStrNotNest("効炎", "炎"));
        }

        // ──────────────────────────────────────────────
        // ToL のエッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToL_TabSeparated_NotSplitByTab()
        {
            // ToL はスペースのみで分割する（タブは分割しない）
            var result = GeneralLib.ToL("a\tb");
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void ToL_LeadingAndTrailingSpaces_Trimmed()
        {
            var result = GeneralLib.ToL("  a  b  ");
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("a", result[0]);
            Assert.AreEqual("b", result[1]);
        }
    }
}
