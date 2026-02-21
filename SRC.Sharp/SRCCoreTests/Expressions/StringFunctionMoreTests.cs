using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression 経由で呼び出す文字列関数のさらなる追加テスト
    /// （StringFunctionTests.cs / StringFunctionAdditionalTests.cs 未カバー分）
    /// </summary>
    [TestClass]
    public class StringFunctionMoreTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // Asc: 未カバーケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Asc_DigitZero_Returns48()
        {
            var exp = Create();
            Assert.AreEqual(48d, exp.GetValueAsDouble("Asc(\"0\")"));
        }

        [TestMethod]
        public void Asc_UpperZ_Returns90()
        {
            var exp = Create();
            Assert.AreEqual(90d, exp.GetValueAsDouble("Asc(\"Z\")"));
        }

        [TestMethod]
        public void Asc_StringResult_ReturnsFormattedNumber()
        {
            var exp = Create();
            Assert.AreEqual("65", exp.GetValueAsString("Asc(\"A\")"));
        }

        // ──────────────────────────────────────────────
        // Chr: 未カバーケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Chr_90_ReturnsUpperZ()
        {
            var exp = Create();
            Assert.AreEqual("Z", exp.GetValueAsString("Chr(90)"));
        }

        [TestMethod]
        public void Chr_13_ReturnsCarriageReturn()
        {
            var exp = Create();
            Assert.AreEqual("\r", exp.GetValueAsString("Chr(13)"));
        }

        // ──────────────────────────────────────────────
        // LCase: 未カバーケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LCase_EmptyString_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("LCase(\"\")"));
        }

        [TestMethod]
        public void LCase_JapaneseString_Unchanged()
        {
            var exp = Create();
            // 日本語文字は LCase で変化しない
            Assert.AreEqual("てすと", exp.GetValueAsString("LCase(\"てすと\")"));
        }

        // ──────────────────────────────────────────────
        // Trim: 未カバーケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Trim_LeadingSpacesOnly_RemovesLeading()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("Trim(\"  hello\")"));
        }

        [TestMethod]
        public void Trim_TrailingSpacesOnly_RemovesTrailing()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("Trim(\"hello  \")"));
        }

        [TestMethod]
        public void Trim_InternalSpaces_Preserved()
        {
            var exp = Create();
            Assert.AreEqual("hello world", exp.GetValueAsString("Trim(\"  hello world  \")"));
        }

        // ──────────────────────────────────────────────
        // Len: 未カバーケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Len_StringResult_ReturnsFormattedNumber()
        {
            var exp = Create();
            Assert.AreEqual("5", exp.GetValueAsString("Len(\"hello\")"));
        }

        [TestMethod]
        public void Len_SpaceString_ReturnsCount()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("Len(\"   \")"));
        }

        // ──────────────────────────────────────────────
        // Left / Right: ゼロ文字数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Left_ZeroChars_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("Left(\"hello\",0)"));
        }

        [TestMethod]
        public void Right_ZeroChars_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("Right(\"hello\",0)"));
        }

        [TestMethod]
        public void Left_StringResult_ReturnsString()
        {
            var exp = Create();
            Assert.AreEqual("hel", exp.GetValueAsString("Left(\"hello\",3)"));
        }

        // ──────────────────────────────────────────────
        // Mid: 未カバーケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Mid_ZeroLength_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("Mid(\"hello\",2,0)"));
        }

        [TestMethod]
        public void Mid_JapaneseString_WithLength()
        {
            var exp = Create();
            Assert.AreEqual("いう", exp.GetValueAsString("Mid(\"あいうえお\",2,2)"));
        }

        // ──────────────────────────────────────────────
        // StrComp: 空文字列ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrComp_BothEmpty_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("StrComp(\"\",\"\")"));
        }

        [TestMethod]
        public void StrComp_FirstEmpty_SecondNotEmpty_ReturnsNegative()
        {
            var exp = Create();
            Assert.IsTrue(exp.GetValueAsDouble("StrComp(\"\",\"a\")") < 0);
        }

        [TestMethod]
        public void StrComp_FirstNotEmpty_SecondEmpty_ReturnsPositive()
        {
            var exp = Create();
            Assert.IsTrue(exp.GetValueAsDouble("StrComp(\"a\",\"\")") > 0);
        }

        [TestMethod]
        public void StrComp_StringResult_ReturnsFormattedNumber()
        {
            var exp = Create();
            Assert.AreEqual("0", exp.GetValueAsString("StrComp(\"x\",\"x\")"));
        }

        // ──────────────────────────────────────────────
        // IsNumeric: 未カバーケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsNumeric_Zero_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("IsNumeric(\"0\")"));
        }

        [TestMethod]
        public void IsNumeric_StringResult_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual("1", exp.GetValueAsString("IsNumeric(\"42\")"));
        }

        [TestMethod]
        public void IsNumeric_SpaceOnly_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("IsNumeric(\" \")"));
        }

        // ──────────────────────────────────────────────
        // Replace: 空文字置換（削除）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Replace_EmptyReplacement_DeletesOccurrences()
        {
            var exp = Create();
            Assert.AreEqual("hllo", exp.GetValueAsString("Replace(\"hello\",\"e\",\"\")"));
        }

        [TestMethod]
        public void Replace_EmptyTarget_ThrowsArgumentException()
        {
            var exp = Create();
            // C# の String.Replace は空文字列を oldValue に渡すと ArgumentException をスローする
            Assert.ThrowsException<System.ArgumentException>(() =>
                exp.GetValueAsString("Replace(\"hello\",\"\",\"X\")"));
        }

        // ──────────────────────────────────────────────
        // MidB: 2 引数（長さなし）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MidB_TwoArgs_ReturnsFromPosition()
        {
            var exp = Create();
            // MidB("hello", 3) → 3バイト目から末尾まで → "llo"
            Assert.AreEqual("llo", exp.GetValueAsString("MidB(\"hello\",3)"));
        }

        [TestMethod]
        public void MidB_TwoArgs_Japanese_ReturnsFromBytePosition()
        {
            var exp = Create();
            // "あいう" の3バイト目から → "いう"
            Assert.AreEqual("いう", exp.GetValueAsString("MidB(\"あいう\",3)"));
        }

        // ──────────────────────────────────────────────
        // InStrRevB: 3 引数（開始位置）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStrRevB_WithStartPosition_SearchesFromByte()
        {
            var exp = Create();
            // InStrRevB("abcabc","a",3) → 3バイト目以前からの最後の "a" → 1
            Assert.AreEqual(1d, exp.GetValueAsDouble("InStrRevB(\"abcabc\",\"a\",3)"));
        }

        // ──────────────────────────────────────────────
        // Wide: 未カバーケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Wide_LowercaseAscii_ConvertsToFullWidth()
        {
            var exp = Create();
            Assert.AreEqual("ａｂｃ", exp.GetValueAsString("Wide(\"abc\")"));
        }

        // ──────────────────────────────────────────────
        // String 関数: 数値型返却
        // ──────────────────────────────────────────────

        [TestMethod]
        public void String_NumericResult_ReturnsZeroForNonNumericRepeat()
        {
            var exp = Create();
            // String(3,"a") をそのまま文字列として返すケースの確認
            Assert.AreEqual("aaa", exp.GetValueAsString("String(3,\"a\")"));
        }

        // ──────────────────────────────────────────────
        // InStr: 3 引数のさらなるケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStr_ThreeArgs_StartBeyondMatch_ReturnsZero()
        {
            var exp = Create();
            // InStr("hello","h",2) → 2番目以降で "h" を探す → 見つからない → 0
            Assert.AreEqual(0d, exp.GetValueAsDouble("InStr(\"hello\",\"h\",2)"));
        }

        [TestMethod]
        public void InStr_ThreeArgs_FindsOccurrenceAtExactStart()
        {
            var exp = Create();
            // InStr("hello","l",3) → 3番目から "l" を探す → 3
            Assert.AreEqual(3d, exp.GetValueAsDouble("InStr(\"hello\",\"l\",3)"));
        }

        // ──────────────────────────────────────────────
        // InStrRev: 空文字列に対する挙動
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStrRev_EmptyHaystack_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("InStrRev(\"\",\"a\")"));
        }
    }
}
