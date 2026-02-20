using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression経由で呼び出す文字列関数のユニットテスト
    /// </summary>
    [TestClass]
    public class StringFunctionTests
    {
        private Expression Create()
        {
            var src = new SRC
            {
                GUI = new MockGUI(),
            };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // Len
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Len_AsciiString_ReturnsCharCount()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("Len(\"hello\")"));
        }

        [TestMethod]
        public void Len_EmptyString_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Len(\"\")"));
        }

        [TestMethod]
        public void Len_JapaneseString_ReturnsCharCount()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("Len(\"あいう\")"));
        }

        // ──────────────────────────────────────────────
        // Left
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Left_BasicUsage_ReturnsPrefix()
        {
            var exp = Create();
            Assert.AreEqual("hel", exp.GetValueAsString("Left(\"hello\",3)"));
        }

        [TestMethod]
        public void Left_LongerThanString_ReturnsFullString()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("Left(\"hello\",10)"));
        }

        // ──────────────────────────────────────────────
        // Right
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Right_BasicUsage_ReturnsSuffix()
        {
            var exp = Create();
            Assert.AreEqual("llo", exp.GetValueAsString("Right(\"hello\",3)"));
        }

        [TestMethod]
        public void Right_LongerThanString_ReturnsFullString()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("Right(\"hello\",10)"));
        }

        // ──────────────────────────────────────────────
        // Mid
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Mid_WithLength_ReturnsSubstring()
        {
            var exp = Create();
            Assert.AreEqual("ell", exp.GetValueAsString("Mid(\"hello\",2,3)"));
        }

        [TestMethod]
        public void Mid_WithoutLength_ReturnsFromPosition()
        {
            var exp = Create();
            Assert.AreEqual("ello", exp.GetValueAsString("Mid(\"hello\",2)"));
        }

        // ──────────────────────────────────────────────
        // InStr
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStr_Found_ReturnsPosition()
        {
            var exp = Create();
            Assert.AreEqual(2d, exp.GetValueAsDouble("InStr(\"hello\",\"el\")"));
        }

        [TestMethod]
        public void InStr_NotFound_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("InStr(\"hello\",\"xyz\")"));
        }

        [TestMethod]
        public void InStr_WithStartPosition_SearchesFromPosition()
        {
            var exp = Create();
            // 3番目以降でlを検索 → 3番目のl
            Assert.AreEqual(3d, exp.GetValueAsDouble("InStr(\"hello\",\"l\",3)"));
        }

        // ──────────────────────────────────────────────
        // InStrRev
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStrRev_FindsLastOccurrence()
        {
            var exp = Create();
            // "hello" に2つのlがある。末尾検索で4番目のlが返る
            Assert.AreEqual(4d, exp.GetValueAsDouble("InStrRev(\"hello\",\"l\")"));
        }

        [TestMethod]
        public void InStrRev_NotFound_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("InStrRev(\"hello\",\"xyz\")"));
        }

        // ──────────────────────────────────────────────
        // Replace
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Replace_BasicSubstitution()
        {
            var exp = Create();
            Assert.AreEqual("hXllo", exp.GetValueAsString("Replace(\"hello\",\"e\",\"X\")"));
        }

        [TestMethod]
        public void Replace_NoMatch_ReturnsOriginal()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("Replace(\"hello\",\"z\",\"X\")"));
        }

        // ──────────────────────────────────────────────
        // Wide
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Wide_AsciiToFullWidth()
        {
            var exp = Create();
            Assert.AreEqual("ＡＢＣ", exp.GetValueAsString("Wide(\"ABC\")"));
        }

        // ──────────────────────────────────────────────
        // IsNumeric
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsNumeric_NumberString_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("IsNumeric(\"42\")"));
        }

        [TestMethod]
        public void IsNumeric_NonNumberString_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("IsNumeric(\"abc\")"));
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
        public void StrComp_LessThan_ReturnsNegative()
        {
            var exp = Create();
            Assert.IsTrue(exp.GetValueAsDouble("StrComp(\"abc\",\"abd\")") < 0);
        }

        // ──────────────────────────────────────────────
        // Asc / Chr
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Asc_ReturnsAsciiCode()
        {
            var exp = Create();
            Assert.AreEqual(65d, exp.GetValueAsDouble("Asc(\"A\")"));
        }

        [TestMethod]
        public void Chr_ReturnsCharacter()
        {
            var exp = Create();
            Assert.AreEqual("A", exp.GetValueAsString("Chr(65)"));
        }

        // ──────────────────────────────────────────────
        // LCase / Trim
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LCase_ConvertsToLowercase()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("LCase(\"HELLO\")"));
        }

        [TestMethod]
        public void Trim_RemovesWhitespace()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("Trim(\"  hello  \")"));
        }

        // ──────────────────────────────────────────────
        // String
        // ──────────────────────────────────────────────

        [TestMethod]
        public void String_RepeatString()
        {
            var exp = Create();
            Assert.AreEqual("aaa", exp.GetValueAsString("String(3,\"a\")"));
        }

        [TestMethod]
        public void String_ZeroCount_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("String(0,\"a\")"));
        }

        // ──────────────────────────────────────────────
        // LSet / RSet
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LSet_PadsStringToWidth()
        {
            var exp = Create();
            // 半角3文字を幅5に左側パディング → "  abc"（右揃え）
            Assert.AreEqual("  abc", exp.GetValueAsString("LSet(\"abc\",5)"));
        }

        [TestMethod]
        public void RSet_PadsStringToWidth()
        {
            var exp = Create();
            // 半角3文字を幅5に右側パディング → "abc  "（左揃え）
            Assert.AreEqual("abc  ", exp.GetValueAsString("RSet(\"abc\",5)"));
        }

        // ──────────────────────────────────────────────
        // LenB / LeftB / RightB / MidB
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LenB_FullWidthJapanese_ReturnsByteCount()
        {
            var exp = Create();
            // 全角3文字 = Shift-JIS で6バイト
            Assert.AreEqual(6d, exp.GetValueAsDouble("LenB(\"あいう\")"));
        }

        [TestMethod]
        public void LeftB_ReturnsLeftBytes()
        {
            var exp = Create();
            // 2バイト → 全角1文字 "あ"
            Assert.AreEqual("あ", exp.GetValueAsString("LeftB(\"あいう\",2)"));
        }

        [TestMethod]
        public void RightB_ReturnsRightBytes()
        {
            var exp = Create();
            // 2バイト → 全角1文字 "う"
            Assert.AreEqual("う", exp.GetValueAsString("RightB(\"あいう\",2)"));
        }

        [TestMethod]
        public void MidB_ReturnsSubstringByBytes()
        {
            var exp = Create();
            // 3バイト目から2バイト → 全角2文字目 "い"
            Assert.AreEqual("い", exp.GetValueAsString("MidB(\"あいう\",3,2)"));
        }

        // ──────────────────────────────────────────────
        // InStrB / InStrRevB
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStrB_ReturnsFirstBytePosition()
        {
            var exp = Create();
            // "い" は3バイト目から
            Assert.AreEqual(3d, exp.GetValueAsDouble("InStrB(\"あいう\",\"い\")"));
        }

        [TestMethod]
        public void InStrRevB_ReturnsLastBytePosition()
        {
            var exp = Create();
            // "あいあ" で最後の "あ" は5バイト目から
            Assert.AreEqual(5d, exp.GetValueAsDouble("InStrRevB(\"あいあ\",\"あ\")"));
        }
    }
}
