using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// VB Strings 関数の追加エッジケーステスト
    /// </summary>
    [TestClass]
    public class StringsEdgeCaseTests
    {
        // ──────────────────────────────────────────────
        // Asc - 特殊文字テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Asc_NullChar_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.Asc('\0'));
        }

        [TestMethod]
        public void Asc_TabChar_Returns9()
        {
            Assert.AreEqual(9, Strings.Asc('\t'));
        }

        [TestMethod]
        public void Asc_LineFeedChar_Returns10()
        {
            Assert.AreEqual(10, Strings.Asc('\n'));
        }

        [TestMethod]
        public void Asc_CarriageReturnChar_Returns13()
        {
            Assert.AreEqual(13, Strings.Asc('\r'));
        }

        [TestMethod]
        public void Asc_SpaceChar_Returns32()
        {
            Assert.AreEqual(32, Strings.Asc(' '));
        }

        [TestMethod]
        public void Asc_DigitZero_Returns48()
        {
            Assert.AreEqual(48, Strings.Asc('0'));
        }

        [TestMethod]
        public void Asc_DigitNine_Returns57()
        {
            Assert.AreEqual(57, Strings.Asc('9'));
        }

        // ──────────────────────────────────────────────
        // Left - 各種エッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Left_NegativeLength_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Left("hello", -1));
        }

        [TestMethod]
        public void Left_JapaneseString_ReturnsCorrectChars()
        {
            Assert.AreEqual("あい", Strings.Left("あいうえお", 2));
        }

        [TestMethod]
        public void Left_MixedString_ReturnsCorrectChars()
        {
            Assert.AreEqual("aあ", Strings.Left("aあbい", 2));
        }

        // ──────────────────────────────────────────────
        // Right - 各種エッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Right_NegativeLength_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Right("hello", -1));
        }

        [TestMethod]
        public void Right_JapaneseString_ReturnsCorrectChars()
        {
            Assert.AreEqual("うえお", Strings.Right("あいうえお", 3));
        }

        [TestMethod]
        public void Right_SingleChar_ReturnsSingleChar()
        {
            Assert.AreEqual("z", Strings.Right("xyz", 1));
        }

        // ──────────────────────────────────────────────
        // Mid - 各種エッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Mid_StartFromLastChar_ReturnsLastChar()
        {
            Assert.AreEqual("o", Strings.Mid("hello", 5, 1));
        }

        [TestMethod]
        public void Mid_JapaneseString_ReturnsCorrectChars()
        {
            Assert.AreEqual("い", Strings.Mid("あいう", 2, 1));
        }

        [TestMethod]
        public void Mid_NegativeLength_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Mid("hello", 1, -1));
        }

        // ──────────────────────────────────────────────
        // InStr - 各種エッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStr_StartBeyondLength_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.InStr(100, "hello", "l"));
        }

        [TestMethod]
        public void InStr_StartNegative_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.InStr(-1, "hello", "l"));
        }

        [TestMethod]
        public void InStr_JapaneseString_FindsCharacter()
        {
            Assert.AreEqual(2, Strings.InStr("あいう", "い"));
        }

        [TestMethod]
        public void InStr_StringAtEnd_ReturnsLastPosition()
        {
            Assert.AreEqual(4, Strings.InStr("abcdef", "def"));
        }

        // ──────────────────────────────────────────────
        // Space - 各種エッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Space_NegativeNumber_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Space(-1));
        }

        [TestMethod]
        public void Space_LargeNumber_ReturnsCorrectSpaces()
        {
            var result = Strings.Space(100);
            Assert.AreEqual(100, result.Length);
            Assert.IsTrue(result.TrimStart().Length == 0);
        }

        // ──────────────────────────────────────────────
        // StrDup - 各種エッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrDup_NegativeN_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.StrDup("a", -1));
        }

        [TestMethod]
        public void StrDup_JapaneseChar_RepeatsCorrectly()
        {
            Assert.AreEqual("あああ", Strings.StrDup("あ", 3));
        }

        [TestMethod]
        public void StrDup_TwoCharString_RepeatsCorrectly()
        {
            Assert.AreEqual("abab", Strings.StrDup("ab", 2));
        }

        // ──────────────────────────────────────────────
        // StrComp - 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrComp_JapaneseStrings_Equal_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.StrComp("あいう", "あいう"));
        }

        [TestMethod]
        public void StrComp_JapaneseStrings_Different_ReturnsNonZero()
        {
            Assert.AreNotEqual(0, Strings.StrComp("あいう", "あいえ"));
        }

        // ──────────────────────────────────────────────
        // Len - 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Len_LongString_ReturnsCorrectLength()
        {
            var str = new string('x', 1000);
            Assert.AreEqual(1000, Strings.Len(str));
        }

        [TestMethod]
        public void Len_MixedJapaneseAndAscii_ReturnsCharCount()
        {
            // "aあ" は2文字
            Assert.AreEqual(2, Strings.Len("aあ"));
        }

        // ──────────────────────────────────────────────
        // StrConv Hiragana - 詳細テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrConv_Hiragana_SokuonKatakana_ToHiragana()
        {
            // "ッ" (促音カタカナ) -> "っ" (促音ひらがな)
            Assert.AreEqual("っ", Strings.StrConv("ッ", VbStrConv.Hiragana));
        }

        [TestMethod]
        public void StrConv_Hiragana_MixedKataAndHira_OnlyKataConverted()
        {
            // "アあイい" -> "ああいい"
            Assert.AreEqual("ああいい", Strings.StrConv("アあイい", VbStrConv.Hiragana));
        }

        // ──────────────────────────────────────────────
        // LCase - 特殊ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LCase_Digits_UnchangedAsString()
        {
            Assert.AreEqual("123", Strings.LCase("123"));
        }

        [TestMethod]
        public void LCase_SpecialChars_Unchanged()
        {
            Assert.AreEqual("abc!@#", Strings.LCase("ABC!@#"));
        }

        [TestMethod]
        public void LCase_Char_LowercaseLetter_ReturnsLowercase()
        {
            Assert.AreEqual('a', Strings.LCase('a'));
        }

        [TestMethod]
        public void LCase_Char_DigitChar_ReturnsSame()
        {
            Assert.AreEqual('5', Strings.LCase('5'));
        }
    }
}
