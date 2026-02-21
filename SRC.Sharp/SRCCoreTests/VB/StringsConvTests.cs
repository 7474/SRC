using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// Space / StrDup / StrComp / StrConv / Trim の追加テスト（既存テストとの重複を避けた新規ケース）
    /// </summary>
    [TestClass]
    public class StringsConvTests
    {
        // ──────────────────────────────────────────────
        // Space
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Space_TenCount_ReturnsTenSpaces()
        {
            Assert.AreEqual("          ", Strings.Space(10));
        }

        [TestMethod]
        public void Space_One_ReturnsSingleSpace()
        {
            var result = Strings.Space(1);
            Assert.AreEqual(" ", result);
            Assert.AreEqual(1, result.Length);
        }

        // ──────────────────────────────────────────────
        // StrDup
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrDup_TwoCharStringThreeTimes_ReturnsRepeated()
        {
            // "ab" × 3 = "ababab"
            Assert.AreEqual("ababab", Strings.StrDup("ab", 3));
        }

        [TestMethod]
        public void StrDup_TwoCharStringOnce_ReturnsSelf()
        {
            Assert.AreEqual("ab", Strings.StrDup("ab", 1));
        }

        [TestMethod]
        public void StrDup_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrDup("", 3));
        }

        [TestMethod]
        public void StrDup_JapaneseChar_Repeats()
        {
            Assert.AreEqual("あああ", Strings.StrDup("あ", 3));
        }

        // ──────────────────────────────────────────────
        // StrComp
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrComp_SimpleAB_LessThan()
        {
            Assert.IsTrue(Strings.StrComp("a", "b") < 0);
        }

        [TestMethod]
        public void StrComp_SimpleBA_GreaterThan()
        {
            Assert.IsTrue(Strings.StrComp("b", "a") > 0);
        }

        [TestMethod]
        public void StrComp_SimpleAA_EqualZero()
        {
            Assert.AreEqual(0, Strings.StrComp("a", "a"));
        }

        [TestMethod]
        public void StrComp_EmptyVsNonEmpty_NegativeOrZero()
        {
            // 空文字列は非空文字列より前に来る
            Assert.IsTrue(Strings.StrComp("", "a") < 0);
        }

        [TestMethod]
        public void StrComp_NonEmptyVsEmpty_Positive()
        {
            Assert.IsTrue(Strings.StrComp("a", "") > 0);
        }

        // ──────────────────────────────────────────────
        // StrConv – Wide（半角→全角）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrConv_Wide_SpaceBecomesFullWidthSpace()
        {
            // 半角スペース(0x20) → 全角スペース(0x3000)
            var result = Strings.StrConv(" ", VbStrConv.Wide);
            Assert.AreEqual("\u3000", result);
        }

        [TestMethod]
        public void StrConv_Wide_SymbolExclamation()
        {
            // '!' (0x21) → '！' (0xFF01)
            var result = Strings.StrConv("!", VbStrConv.Wide);
            Assert.AreEqual("！", result);
        }

        [TestMethod]
        public void StrConv_Wide_Digits_BecomesFullWidth()
        {
            var result = Strings.StrConv("09", VbStrConv.Wide);
            Assert.AreEqual("０９", result);
        }

        [TestMethod]
        public void StrConv_Wide_AlreadyFullWidth_Unchanged()
        {
            // 全角文字はそのまま
            var result = Strings.StrConv("あいう", VbStrConv.Wide);
            Assert.AreEqual("あいう", result);
        }

        // ──────────────────────────────────────────────
        // StrConv – Narrow（全角→半角）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrConv_Narrow_FullWidthSpaceBecomesHalfWidthSpace()
        {
            // 全角スペース(0x3000) → 半角スペース(0x20)
            var result = Strings.StrConv("\u3000", VbStrConv.Narrow);
            Assert.AreEqual(" ", result);
        }

        [TestMethod]
        public void StrConv_Narrow_FullWidthDigits()
        {
            var result = Strings.StrConv("０９", VbStrConv.Narrow);
            Assert.AreEqual("09", result);
        }

        [TestMethod]
        public void StrConv_Narrow_AlreadyHalfWidth_Unchanged()
        {
            // 半角文字はそのまま
            var result = Strings.StrConv("abc", VbStrConv.Narrow);
            Assert.AreEqual("abc", result);
        }

        [TestMethod]
        public void StrConv_Narrow_HiraganaUnchanged()
        {
            // ひらがなは Narrow 変換の対象外
            var result = Strings.StrConv("あいう", VbStrConv.Narrow);
            Assert.AreEqual("あいう", result);
        }

        // ──────────────────────────────────────────────
        // StrConv – Hiragana（カタカナ→ひらがな）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrConv_Hiragana_BasicKatakana_AiU()
        {
            // ア(0x30A2)→あ(0x3042), イ(0x30A4)→い(0x3044), ウ(0x30A6)→う(0x3046)
            var result = Strings.StrConv("アイウ", VbStrConv.Hiragana);
            Assert.AreEqual("あいう", result);
        }

        [TestMethod]
        public void StrConv_Hiragana_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrConv("", VbStrConv.Hiragana));
        }

        [TestMethod]
        public void StrConv_Hiragana_MixedContent_OnlyKatakanaConverted()
        {
            // ABC と数字はそのまま、カタカナのみ変換
            var result = Strings.StrConv("アABC1", VbStrConv.Hiragana);
            Assert.AreEqual("あABC1", result);
        }

        // ──────────────────────────────────────────────
        // StrConv – null / empty
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrConv_NullInput_Wide_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrConv(null, VbStrConv.Wide));
        }

        [TestMethod]
        public void StrConv_NullInput_Narrow_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrConv(null, VbStrConv.Narrow));
        }

        [TestMethod]
        public void StrConv_EmptyInput_Hiragana_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrConv("", VbStrConv.Hiragana));
        }

        // ──────────────────────────────────────────────
        // Trim
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Trim_NullInput_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Trim(null));
        }

        [TestMethod]
        public void Trim_EmptyInput_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Trim(""));
        }

        [TestMethod]
        public void Trim_PaddedString_RemovesBothSides()
        {
            Assert.AreEqual("abc", Strings.Trim("  abc  "));
        }

        [TestMethod]
        public void Trim_InternalSpaces_Preserved()
        {
            // 内部スペースは保持される
            Assert.AreEqual("a b c", Strings.Trim("  a b c  "));
        }

        [TestMethod]
        public void Trim_LeadingOnly_RemovesLeading()
        {
            Assert.AreEqual("hello", Strings.Trim("   hello"));
        }

        [TestMethod]
        public void Trim_TrailingOnly_RemovesTrailing()
        {
            Assert.AreEqual("hello", Strings.Trim("hello   "));
        }
    }
}
