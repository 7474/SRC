using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression 経由で呼び出す文字列関数の更なる追加テスト
    /// </summary>
    [TestClass]
    public class StringFunctionMoreTests2
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // Len 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Len_SingleChar_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("Len(\"a\")"));
        }

        [TestMethod]
        public void Len_Numeric_ReturnsStringLength()
        {
            var exp = Create();
            // Len(123) → 数値を文字列化した長さ = 3
            Assert.AreEqual(3d, exp.GetValueAsDouble("Len(123)"));
        }

        // ──────────────────────────────────────────────
        // Left 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Left_ExactLength_ReturnsWholeString()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("Left(\"hello\",5)"));
        }

        [TestMethod]
        public void Left_Zero_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("Left(\"hello\",0)"));
        }

        [TestMethod]
        public void Left_Japanese_ReturnsCorrectChars()
        {
            var exp = Create();
            Assert.AreEqual("あい", exp.GetValueAsString("Left(\"あいう\",2)"));
        }

        // ──────────────────────────────────────────────
        // Right 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Right_ExactLength_ReturnsWholeString()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("Right(\"hello\",5)"));
        }

        [TestMethod]
        public void Right_Zero_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("Right(\"hello\",0)"));
        }

        [TestMethod]
        public void Right_OneChar_ReturnsLastChar()
        {
            var exp = Create();
            Assert.AreEqual("o", exp.GetValueAsString("Right(\"hello\",1)"));
        }

        // ──────────────────────────────────────────────
        // Mid 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Mid_FromPosition1_WithLength4()
        {
            var exp = Create();
            Assert.AreEqual("hell", exp.GetValueAsString("Mid(\"hello\",1,4)"));
        }

        [TestMethod]
        public void Mid_BeyondEnd_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("Mid(\"hello\",10)"));
        }

        [TestMethod]
        public void Mid_FromLastChar_ReturnsLastChar()
        {
            var exp = Create();
            Assert.AreEqual("o", exp.GetValueAsString("Mid(\"hello\",5)"));
        }

        // ──────────────────────────────────────────────
        // InStr 追加ケース
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
        public void InStr_WithStartPos_SearchesFromPosition()
        {
            var exp = Create();
            // InStr(str, search, startPos)
            Assert.AreEqual(3d, exp.GetValueAsDouble("InStr(\"hello\",\"l\",3)"));
        }

        [TestMethod]
        public void InStr_StartPosBeyondFirst_FindsLast()
        {
            var exp = Create();
            // "hello" で "l" を位置4から検索 → 4
            Assert.AreEqual(4d, exp.GetValueAsDouble("InStr(\"hello\",\"l\",4)"));
        }

        // ──────────────────────────────────────────────
        // LCase / Trim 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LCase_MixedInput_ReturnsAllLower()
        {
            var exp = Create();
            Assert.AreEqual("test123", exp.GetValueAsString("LCase(\"Test123\")"));
        }

        [TestMethod]
        public void Trim_LeadingOnly_RemovesLeading()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("Trim(\"  hello\")"));
        }

        [TestMethod]
        public void Trim_NoSpaces_Unchanged()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("Trim(\"hello\")"));
        }

        // ──────────────────────────────────────────────
        // Asc 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Asc_LowerA_Returns97()
        {
            var exp = Create();
            Assert.AreEqual(97d, exp.GetValueAsDouble("Asc(\"a\")"));
        }

        // ──────────────────────────────────────────────
        // Replace 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Replace_MultipleOccurrences_ReplaceAll()
        {
            var exp = Create();
            Assert.AreEqual("heXXo", exp.GetValueAsString("Replace(\"hello\",\"l\",\"X\")"));
        }

        [TestMethod]
        public void Replace_EmptyReplacement_RemovesSubstring()
        {
            var exp = Create();
            Assert.AreEqual("heo", exp.GetValueAsString("Replace(\"hello\",\"ll\",\"\")"));
        }

        // ──────────────────────────────────────────────
        // String 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void String_RepeatFive_ReturnsFiveChars()
        {
            var exp = Create();
            Assert.AreEqual("aaaaa", exp.GetValueAsString("String(5,\"a\")"));
        }

        [TestMethod]
        public void String_RepeatOne_ReturnsOneChar()
        {
            var exp = Create();
            Assert.AreEqual("x", exp.GetValueAsString("String(1,\"x\")"));
        }

        // ──────────────────────────────────────────────
        // Wide 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Wide_Numbers_ConvertsToFullWidth()
        {
            var exp = Create();
            Assert.AreEqual("１２３", exp.GetValueAsString("Wide(\"123\")"));
        }
    }
}
