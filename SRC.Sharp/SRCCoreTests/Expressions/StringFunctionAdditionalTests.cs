using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression 経由で呼び出す文字列関数の追加テスト（エッジケース・未カバー機能）
    /// </summary>
    [TestClass]
    public class StringFunctionAdditionalTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // Format
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Format_IntWithThreeDigits_ReturnsFormatted()
        {
            var exp = Create();
            Assert.AreEqual("007", exp.GetValueAsString("Format(7,\"000\")"));
        }

        [TestMethod]
        public void Format_HundredWithThreeDigits_ReturnsFormatted()
        {
            var exp = Create();
            Assert.AreEqual("100", exp.GetValueAsString("Format(100,\"000\")"));
        }

        [TestMethod]
        public void Format_DoubleWithDecimalFormat_ReturnsFormatted()
        {
            var exp = Create();
            Assert.AreEqual("3.14", exp.GetValueAsString("Format(3.14,\"0.##\")"));
        }

        [TestMethod]
        public void Format_WholeDoubleWithDecimalFormat_RemovesTrailingZero()
        {
            var exp = Create();
            Assert.AreEqual("3", exp.GetValueAsString("Format(3.0,\"0.##\")"));
        }

        [TestMethod]
        public void Format_NegativeNumber_ReturnsNegativeFormatted()
        {
            var exp = Create();
            Assert.AreEqual("-05", exp.GetValueAsString("Format(-5,\"00\")"));
        }

        // ──────────────────────────────────────────────
        // Replace (start position)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Replace_WithStartPosition_ReplacesFromPosition()
        {
            var exp = Create();
            // Replace("abcabc","a","X",4) → 4文字目以降の"a"を置換 → "abcXbc"
            Assert.AreEqual("abcXbc", exp.GetValueAsString("Replace(\"abcabc\",\"a\",\"X\",4)"));
        }

        [TestMethod]
        public void Replace_WithStartPositionAtBeginning_ReplacesAll()
        {
            var exp = Create();
            // Replace("aaa","a","b",1) → すべての"a"を"b"に置換 → "bbb"
            Assert.AreEqual("bbb", exp.GetValueAsString("Replace(\"aaa\",\"a\",\"b\",1)"));
        }

        [TestMethod]
        public void Replace_WithStartAndCount_ReplacesOnlyInWindow()
        {
            var exp = Create();
            // Replace("abcabc","a","X",1,3) → 1文字目から3文字"abc"内の"a"を置換
            // Left("abcabc",0)="" + Mid("abcabc",1,3).Replace("a","X")="Xbc" + Right("abcabc",2)="bc" → "Xbcbc"
            Assert.AreEqual("Xbcbc", exp.GetValueAsString("Replace(\"abcabc\",\"a\",\"X\",1,3)"));
        }

        [TestMethod]
        public void Replace_WithCountZero_WindowIsEmpty()
        {
            var exp = Create();
            // Replace("abc","a","X",1,0) → 0文字ウィンドウ → ""置換
            // Left("abc",0)="" + ""(空window) + Right("abc",2)="bc" → "bc"
            Assert.AreEqual("bc", exp.GetValueAsString("Replace(\"abc\",\"a\",\"X\",1,0)"));
        }

        // ──────────────────────────────────────────────
        // InStr (日本語)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStr_JapaneseText_Found_ReturnsPosition()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("InStr(\"あいうえお\",\"う\")"));
        }

        [TestMethod]
        public void InStr_JapaneseText_NotFound_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("InStr(\"あいう\",\"え\")"));
        }

        [TestMethod]
        public void InStr_ThreeArgs_SearchesFromPosition()
        {
            var exp = Create();
            // InStr("abcabc","a",4) → 4文字目以降から"a"を探す → 4
            Assert.AreEqual(4d, exp.GetValueAsDouble("InStr(\"abcabc\",\"a\",4)"));
        }

        // ──────────────────────────────────────────────
        // InStrRev
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStrRev_MultipleOccurrences_ReturnsLast()
        {
            var exp = Create();
            Assert.AreEqual(4d, exp.GetValueAsDouble("InStrRev(\"abcabc\",\"a\")"));
        }

        [TestMethod]
        public void InStrRev_SingleOccurrence_ReturnsPosition()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("InStrRev(\"abcdef\",\"a\")"));
        }

        [TestMethod]
        public void InStrRev_EmptyNeedle_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("InStrRev(\"hello\",\"\")"));
        }

        [TestMethod]
        public void InStrRev_WithStartPosition_SearchesFromPosition()
        {
            var exp = Create();
            // InStrRev("abcabc","a",3) → 3文字目以前から"a"を後方検索 → 1
            Assert.AreEqual(1d, exp.GetValueAsDouble("InStrRev(\"abcabc\",\"a\",3)"));
        }

        // ──────────────────────────────────────────────
        // LSet / RSet
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LSet_StringShorterThanWidth_PadsLeft()
        {
            var exp = Create();
            // "hello" (5文字=5バイト) → 幅8 → 3スペース + "hello"
            Assert.AreEqual("   hello", exp.GetValueAsString("LSet(\"hello\",8)"));
        }

        [TestMethod]
        public void LSet_StringExactWidth_NoPadding()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("LSet(\"hello\",5)"));
        }

        [TestMethod]
        public void LSet_StringLongerThanWidth_ReturnsOriginal()
        {
            var exp = Create();
            Assert.AreEqual("hello world", exp.GetValueAsString("LSet(\"hello world\",3)"));
        }

        [TestMethod]
        public void RSet_StringShorterThanWidth_PadsRight()
        {
            var exp = Create();
            Assert.AreEqual("hello   ", exp.GetValueAsString("RSet(\"hello\",8)"));
        }

        [TestMethod]
        public void RSet_StringExactWidth_NoPadding()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("RSet(\"hello\",5)"));
        }

        [TestMethod]
        public void RSet_StringLongerThanWidth_ReturnsOriginal()
        {
            var exp = Create();
            Assert.AreEqual("hello world", exp.GetValueAsString("RSet(\"hello world\",3)"));
        }

        // ──────────────────────────────────────────────
        // String (文字繰り返し)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void String_RepeatsSingleChar()
        {
            var exp = Create();
            Assert.AreEqual("aaaaa", exp.GetValueAsString("String(5,\"a\")"));
        }

        [TestMethod]
        public void String_RepeatsMultiCharString()
        {
            var exp = Create();
            Assert.AreEqual("ababab", exp.GetValueAsString("String(3,\"ab\")"));
        }

        [TestMethod]
        public void String_ZeroRepeat_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("String(0,\"x\")"));
        }

        [TestMethod]
        public void String_NegativeRepeat_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("String(-1,\"x\")"));
        }

        [TestMethod]
        public void String_RepeatJapaneseChar()
        {
            var exp = Create();
            Assert.AreEqual("あああ", exp.GetValueAsString("String(3,\"あ\")"));
        }

        // ──────────────────────────────────────────────
        // Wide (全角変換)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Wide_AsciiNumbers_ConvertsToFullWidth()
        {
            var exp = Create();
            var result = exp.GetValueAsString("Wide(\"123\")");
            Assert.AreEqual("１２３", result);
        }

        [TestMethod]
        public void Wide_EmptyString_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("Wide(\"\")"));
        }

        [TestMethod]
        public void Wide_MixedString_ConvertsAsciiPart()
        {
            var exp = Create();
            var result = exp.GetValueAsString("Wide(\"ABC\")");
            Assert.AreEqual("ＡＢＣ", result);
        }

        // ──────────────────────────────────────────────
        // IsNumeric
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsNumeric_PositiveInteger_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("IsNumeric(\"42\")"));
        }

        [TestMethod]
        public void IsNumeric_NegativeNumber_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("IsNumeric(\"-5\")"));
        }

        [TestMethod]
        public void IsNumeric_FloatNumber_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("IsNumeric(\"3.14\")"));
        }

        [TestMethod]
        public void IsNumeric_AlphaString_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("IsNumeric(\"abc\")"));
        }

        [TestMethod]
        public void IsNumeric_EmptyString_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("IsNumeric(\"\")"));
        }

        // ──────────────────────────────────────────────
        // StrComp
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrComp_EqualStrings_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("StrComp(\"abc\",\"abc\")"));
        }

        [TestMethod]
        public void StrComp_FirstLess_ReturnsNegative()
        {
            var exp = Create();
            Assert.IsTrue(exp.GetValueAsDouble("StrComp(\"abc\",\"abd\")") < 0);
        }

        [TestMethod]
        public void StrComp_FirstGreater_ReturnsPositive()
        {
            var exp = Create();
            Assert.IsTrue(exp.GetValueAsDouble("StrComp(\"abd\",\"abc\")") > 0);
        }

        // ──────────────────────────────────────────────
        // LenB / LeftB / RightB / MidB / InStrB / InStrRevB (追加エッジケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LenB_EmptyString_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("LenB(\"\")"));
        }

        [TestMethod]
        public void LenB_MixedAsciiAndJapanese_ReturnsByteCount()
        {
            var exp = Create();
            // "Aあ" = 1 + 2 = 3 bytes in Shift-JIS
            Assert.AreEqual(3d, exp.GetValueAsDouble("LenB(\"Aあ\")"));
        }

        [TestMethod]
        public void LeftB_ZeroBytes_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("LeftB(\"hello\",0)"));
        }

        [TestMethod]
        public void LeftB_MoreThanLength_ReturnsFullString()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("LeftB(\"hello\",100)"));
        }

        [TestMethod]
        public void RightB_ZeroBytes_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("RightB(\"hello\",0)"));
        }

        [TestMethod]
        public void RightB_MoreThanLength_ReturnsFullString()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("RightB(\"hello\",100)"));
        }

        [TestMethod]
        public void MidB_FromMiddle_ReturnsSubstring()
        {
            var exp = Create();
            // MidB("hello", 2, 3) → bytes 2-4 → "ell"
            Assert.AreEqual("ell", exp.GetValueAsString("MidB(\"hello\",2,3)"));
        }

        [TestMethod]
        public void InStrB_WithStartPosition_SearchesFromByte()
        {
            var exp = Create();
            // InStrB("abcabc","a",2) → バイト2以降で"a"を探す → 4
            Assert.AreEqual(4d, exp.GetValueAsDouble("InStrB(\"abcabc\",\"a\",2)"));
        }

        [TestMethod]
        public void InStrRevB_MultipleOccurrences_ReturnsLast()
        {
            var exp = Create();
            Assert.AreEqual(4d, exp.GetValueAsDouble("InStrRevB(\"abcabc\",\"ab\")"));
        }

        // ──────────────────────────────────────────────
        // Chr
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Chr_65_ReturnsA()
        {
            var exp = Create();
            Assert.AreEqual("A", exp.GetValueAsString("Chr(65)"));
        }

        [TestMethod]
        public void Chr_97_ReturnsLowercaseA()
        {
            var exp = Create();
            Assert.AreEqual("a", exp.GetValueAsString("Chr(97)"));
        }

        [TestMethod]
        public void Chr_48_ReturnsZeroChar()
        {
            var exp = Create();
            Assert.AreEqual("0", exp.GetValueAsString("Chr(48)"));
        }

        // ──────────────────────────────────────────────
        // Trim
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Trim_LeadingAndTrailing_RemovesBoth()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("Trim(\"  hello  \")"));
        }

        [TestMethod]
        public void Trim_NoSpaces_Unchanged()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("Trim(\"hello\")"));
        }

        [TestMethod]
        public void Trim_OnlySpaces_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("Trim(\"   \")"));
        }

        // ──────────────────────────────────────────────
        // LCase
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LCase_UppercaseASCII_ReturnsLowercase()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("LCase(\"HELLO\")"));
        }

        [TestMethod]
        public void LCase_MixedCase_ReturnsAllLower()
        {
            var exp = Create();
            Assert.AreEqual("abc123def", exp.GetValueAsString("LCase(\"ABC123DEF\")"));
        }

        [TestMethod]
        public void LCase_AlreadyLower_Unchanged()
        {
            var exp = Create();
            Assert.AreEqual("test", exp.GetValueAsString("LCase(\"test\")"));
        }

        // ──────────────────────────────────────────────
        // Mid (追加エッジケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Mid_StartBeyondLength_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("Mid(\"hello\",10)"));
        }

        [TestMethod]
        public void Mid_LengthExceedsString_ReturnsTail()
        {
            var exp = Create();
            Assert.AreEqual("lo", exp.GetValueAsString("Mid(\"hello\",4,100)"));
        }

        [TestMethod]
        public void Mid_StartOne_ReturnsFullString()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("Mid(\"hello\",1)"));
        }

        // ──────────────────────────────────────────────
        // Left / Right (追加エッジケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Left_EmptyString_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("Left(\"\",5)"));
        }

        [TestMethod]
        public void Right_EmptyString_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("Right(\"\",5)"));
        }

        [TestMethod]
        public void Left_JapaneseString_ReturnsPrefix()
        {
            var exp = Create();
            Assert.AreEqual("あい", exp.GetValueAsString("Left(\"あいうえお\",2)"));
        }

        [TestMethod]
        public void Right_JapaneseString_ReturnsSuffix()
        {
            var exp = Create();
            Assert.AreEqual("えお", exp.GetValueAsString("Right(\"あいうえお\",2)"));
        }
    }
}
