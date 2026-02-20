using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    [TestClass()]
    public class StringsTests
    {
        // ──────────────────────────────────────────────
        // Asc
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void AscTest_AsciiChar()
        {
            Assert.AreEqual(65, Strings.Asc('A'));
            Assert.AreEqual(97, Strings.Asc('a'));
            Assert.AreEqual(48, Strings.Asc('0'));
        }

        [TestMethod()]
        public void AscTest_String()
        {
            Assert.AreEqual(65, Strings.Asc("ABC"));  // 先頭文字 'A'
            Assert.AreEqual(49, Strings.Asc("123"));  // 先頭文字 '1'
        }

        // ──────────────────────────────────────────────
        // LCase
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void LCaseTest_String()
        {
            Assert.AreEqual("hello", Strings.LCase("HELLO"));
            Assert.AreEqual("hello world", Strings.LCase("Hello World"));
            Assert.AreEqual("abc123", Strings.LCase("ABC123"));
        }

        [TestMethod()]
        public void LCaseTest_Char()
        {
            Assert.AreEqual('a', Strings.LCase('A'));
            Assert.AreEqual('z', Strings.LCase('Z'));
            Assert.AreEqual('1', Strings.LCase('1')); // 数字はそのまま
        }

        // ──────────────────────────────────────────────
        // Len
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void LenTest()
        {
            Assert.AreEqual(0, Strings.Len(null));
            Assert.AreEqual(0, Strings.Len(""));
            Assert.AreEqual(5, Strings.Len("hello"));
            Assert.AreEqual(3, Strings.Len("あいう")); // 文字数（バイト数でなく）
        }

        // ──────────────────────────────────────────────
        // Left
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void LeftTest_BasicUsage()
        {
            Assert.AreEqual("hel", Strings.Left("hello", 3));
            Assert.AreEqual("hello", Strings.Left("hello", 5));
            Assert.AreEqual("hello", Strings.Left("hello", 10)); // 文字列より長い
        }

        [TestMethod()]
        public void LeftTest_ZeroLength()
        {
            Assert.AreEqual("", Strings.Left("hello", 0));
        }

        [TestMethod()]
        public void LeftTest_EmptyString()
        {
            Assert.AreEqual("", Strings.Left("", 3));
            Assert.AreEqual("", Strings.Left(null, 3));
        }

        // ──────────────────────────────────────────────
        // Right
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void RightTest_BasicUsage()
        {
            Assert.AreEqual("llo", Strings.Right("hello", 3));
            Assert.AreEqual("hello", Strings.Right("hello", 5));
            Assert.AreEqual("hello", Strings.Right("hello", 10)); // 文字列より長い
        }

        [TestMethod()]
        public void RightTest_ZeroLength()
        {
            Assert.AreEqual("", Strings.Right("hello", 0));
        }

        [TestMethod()]
        public void RightTest_EmptyString()
        {
            Assert.AreEqual("", Strings.Right("", 3));
            Assert.AreEqual("", Strings.Right(null, 3));
        }

        // ──────────────────────────────────────────────
        // Mid
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void MidTest_WithLengthArg()
        {
            Assert.AreEqual("ell", Strings.Mid("hello", 2, 3));
            Assert.AreEqual("h", Strings.Mid("hello", 1, 1));
            Assert.AreEqual("hello", Strings.Mid("hello", 1, 10)); // 文字列より長い
        }

        [TestMethod()]
        public void MidTest_WithoutLengthArg()
        {
            Assert.AreEqual("ello", Strings.Mid("hello", 2));
            Assert.AreEqual("hello", Strings.Mid("hello", 1));
        }

        [TestMethod()]
        public void MidTest_EmptyString()
        {
            Assert.AreEqual("", Strings.Mid("", 1));
            Assert.AreEqual("", Strings.Mid(null, 1));
        }

        [TestMethod()]
        public void MidTest_ZeroLength()
        {
            Assert.AreEqual("", Strings.Mid("hello", 1, 0));
        }

        [TestMethod()]
        public void MidTest_StartBeyondEnd()
        {
            Assert.AreEqual("", Strings.Mid("hello", 10));
        }

        // ──────────────────────────────────────────────
        // InStr
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void InStrTest_Found()
        {
            Assert.AreEqual(2, Strings.InStr("hello", "el")); // 2文字目から一致
            Assert.AreEqual(1, Strings.InStr("hello", "h"));  // 先頭
            Assert.AreEqual(5, Strings.InStr("hello", "o"));  // 末尾
        }

        [TestMethod()]
        public void InStrTest_NotFound()
        {
            Assert.AreEqual(0, Strings.InStr("hello", "xyz"));
        }

        [TestMethod()]
        public void InStrTest_WithStartPosition()
        {
            Assert.AreEqual(3, Strings.InStr(3, "hello", "l")); // 3文字目以降から検索 → 3文字目の'l'
            Assert.AreEqual(4, Strings.InStr(4, "hello", "l")); // 4文字目以降から検索 → 4文字目の'l'
            Assert.AreEqual(0, Strings.InStr(5, "hello", "l")); // 5文字目以降には'l'がない
        }

        [TestMethod()]
        public void InStrTest_EmptyStrings()
        {
            Assert.AreEqual(1, Strings.InStr("hello", "")); // 空文字列は位置1で一致
            Assert.AreEqual(0, Strings.InStr("", "hello"));
            Assert.AreEqual(0, Strings.InStr(null, "hello"));
        }

        // ──────────────────────────────────────────────
        // Space
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void SpaceTest()
        {
            Assert.AreEqual("   ", Strings.Space(3));
            Assert.AreEqual("", Strings.Space(0));
            Assert.AreEqual(" ", Strings.Space(1));
        }

        // ──────────────────────────────────────────────
        // StrDup
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void StrDupTest_SingleChar()
        {
            Assert.AreEqual("aaa", Strings.StrDup("a", 3));
            Assert.AreEqual("a", Strings.StrDup("a", 1));
            Assert.AreEqual("", Strings.StrDup("a", 0));
        }

        [TestMethod()]
        public void StrDupTest_MultiChar()
        {
            Assert.AreEqual("abcabcabc", Strings.StrDup("abc", 3));
        }

        [TestMethod()]
        public void StrDupTest_EmptyString()
        {
            Assert.AreEqual("", Strings.StrDup("", 3));
            Assert.AreEqual("", Strings.StrDup(null, 3));
        }

        // ──────────────────────────────────────────────
        // StrComp
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void StrCompTest_EqualStrings()
        {
            Assert.AreEqual(0, Strings.StrComp("hello", "hello"));
        }

        [TestMethod()]
        public void StrCompTest_LessThan()
        {
            Assert.IsTrue(Strings.StrComp("abc", "abd") < 0);
        }

        [TestMethod()]
        public void StrCompTest_GreaterThan()
        {
            Assert.IsTrue(Strings.StrComp("abd", "abc") > 0);
        }

        // ──────────────────────────────────────────────
        // Trim
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void TrimTest()
        {
            Assert.AreEqual("hello", Strings.Trim("  hello  "));
            Assert.AreEqual("hello", Strings.Trim("hello"));
            Assert.AreEqual("", Strings.Trim("   "));
            Assert.AreEqual("", Strings.Trim(""));
            Assert.AreEqual("", Strings.Trim(null));
        }

        // ──────────────────────────────────────────────
        // StrConv
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void StrConvTest_EmptyOrNull()
        {
            Assert.AreEqual("", Strings.StrConv("", VbStrConv.Wide));
            Assert.AreEqual("", Strings.StrConv(null, VbStrConv.Wide));
        }

        [TestMethod()]
        public void StrConvTest_Wide_AsciiToFullWidth()
        {
            var result = Strings.StrConv("ABC", VbStrConv.Wide);
            Assert.AreEqual("ＡＢＣ", result);
        }

        [TestMethod()]
        public void StrConvTest_Narrow_FullWidthToAscii()
        {
            var result = Strings.StrConv("ＡＢＣ", VbStrConv.Narrow);
            Assert.AreEqual("ABC", result);
        }

        [TestMethod()]
        public void StrConvTest_Hiragana_KatakanaToHiragana()
        {
            var result = Strings.StrConv("アイウエオ", VbStrConv.Hiragana);
            Assert.AreEqual("あいうえお", result);
        }

        [TestMethod()]
        public void StrConvTest_Hiragana_HiraganaUnchanged()
        {
            var result = Strings.StrConv("あいうえお", VbStrConv.Hiragana);
            Assert.AreEqual("あいうえお", result);
        }

        [TestMethod()]
        public void StrConvTest_Unknown_ReturnsOriginal()
        {
            var result = Strings.StrConv("abc", (VbStrConv)999);
            Assert.AreEqual("abc", result);
        }

        // ──────────────────────────────────────────────
        // Asc edge cases
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void AscTest_JapaneseChar_ReturnsCodePoint()
        {
            // 日本語文字のコードポイントは128より大きい
            int code = Strings.Asc('あ');
            Assert.IsTrue(code > 128, $"Asc('あ') should be > 128, got {code}");
        }

        [TestMethod()]
        public void AscTest_SpaceChar_Returns32()
        {
            Assert.AreEqual(32, Strings.Asc(' '));
        }

        // ──────────────────────────────────────────────
        // LCase edge cases
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void LCaseTest_MixedCaseWithNumbers()
        {
            Assert.AreEqual("abc123def", Strings.LCase("ABC123DEF"));
        }

        [TestMethod()]
        public void LCaseTest_AlreadyLower_Unchanged()
        {
            Assert.AreEqual("hello", Strings.LCase("hello"));
        }

        // ──────────────────────────────────────────────
        // Trim edge cases
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void TrimTest_OnlyWhitespace_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Trim("   "));
            Assert.AreEqual("", Strings.Trim("\t\t"));
        }

        [TestMethod()]
        public void TrimTest_NoWhitespace_Unchanged()
        {
            Assert.AreEqual("hello", Strings.Trim("hello"));
        }

        // ──────────────────────────────────────────────
        // InStr edge cases
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void InStrTest_EmptyNeedle_ReturnsOne()
        {
            // VB互換: 空文字検索は1を返す
            Assert.AreEqual(1, Strings.InStr("hello", ""));
        }

        [TestMethod()]
        public void InStrTest_NeedleLongerThanHaystack_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.InStr("hi", "hello"));
        }

        [TestMethod()]
        public void InStrTest_WithStart_FindsFromPosition()
        {
            // start=3 で "hello" の 'l' を探す -> 3
            Assert.AreEqual(3, Strings.InStr(3, "hello", "l"));
        }

        [TestMethod()]
        public void InStrTest_Found_ReturnsPosition()
        {
            Assert.AreEqual(3, Strings.InStr("abcabc", "c"));
        }

        // ──────────────────────────────────────────────
        // Space
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void SpaceTest_ReturnsSpaces()
        {
            Assert.AreEqual("   ", Strings.Space(3));
            Assert.AreEqual("", Strings.Space(0));
        }

        // ──────────────────────────────────────────────
        // StrDup
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void StrDupTest_RepeatsString()
        {
            Assert.AreEqual("abcabc", Strings.StrDup("abc", 2));
            Assert.AreEqual("", Strings.StrDup("abc", 0));
            Assert.AreEqual("abc", Strings.StrDup("abc", 1));
        }

        // ──────────────────────────────────────────────
        // Len edge cases
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void LenTest_JapaneseSingleChar()
        {
            Assert.AreEqual(1, Strings.Len("あ"));
        }

        // ──────────────────────────────────────────────
        // Mid edge cases
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void MidTest_StartBeyondLength_ReturnsEmpty()
        {
            Assert.AreEqual("", Strings.Mid("hello", 10));
        }

        [TestMethod()]
        public void MidTest_WithLength_Clamps()
        {
            Assert.AreEqual("lo", Strings.Mid("hello", 4, 100));
        }

        // ──────────────────────────────────────────────
        // StrComp
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void StrCompTest_EqualStrings_ReturnsZero()
        {
            Assert.AreEqual(0, Strings.StrComp("abc", "abc"));
        }

        [TestMethod()]
        public void StrCompTest_LessThan_ReturnsNegative()
        {
            Assert.IsTrue(Strings.StrComp("abc", "abd") < 0);
        }

        [TestMethod()]
        public void StrCompTest_GreaterThan_ReturnsPositive()
        {
            Assert.IsTrue(Strings.StrComp("abd", "abc") > 0);
        }
    }
}
