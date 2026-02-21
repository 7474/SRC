using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;

namespace SRCCore.Extensions.Tests
{
    /// <summary>
    /// StringExtension クラスの追加テスト（エッジケース）
    /// </summary>
    [TestClass]
    public class StringExtensionAdditionalTests
    {
        // ──────────────────────────────────────────────
        // ArrayIndexByName (追加ケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayIndexByName_NumericIndex_ReturnsIndex()
        {
            Assert.AreEqual("42", "var[42]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_JapaneseKey_ReturnsKey()
        {
            Assert.AreEqual("キー", "変数[キー]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_EmptyIndex_ReturnsEmpty()
        {
            // "var[]" は空インデックス
            Assert.AreEqual("", "var[]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_ComplexVarName_ReturnsIndex()
        {
            Assert.AreEqual("1", "very_long_variable_name[1]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_MultipleSquareBrackets_ReturnsOutermostContent()
        {
            // "a[b[c]]" → returns "b[c]"
            Assert.AreEqual("b[c]", "a[b[c]]".ArrayIndexByName());
        }

        // ──────────────────────────────────────────────
        // InsideKakko (追加ケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InsideKakko_EmptyParens_ReturnsEmpty()
        {
            Assert.AreEqual("", "func()".InsideKakko());
        }

        [TestMethod]
        public void InsideKakko_JapaneseContent_ReturnsContent()
        {
            Assert.AreEqual("テスト", "func(テスト)".InsideKakko());
        }

        [TestMethod]
        public void InsideKakko_NumericContent_ReturnsContent()
        {
            Assert.AreEqual("123", "func(123)".InsideKakko());
        }

        [TestMethod]
        public void InsideKakko_MultipleParensGroups_ReturnsFirst()
        {
            // 最初の"("から最後の")"まで
            Assert.AreEqual("a)(b", "f(a)(b)".InsideKakko());
        }

        [TestMethod]
        public void InsideKakko_NoParens_ReturnsEmpty()
        {
            Assert.AreEqual("", "no parens here".InsideKakko());
        }

        // ──────────────────────────────────────────────
        // ReplaceNewLine (追加ケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReplaceNewLine_CRLF_Replaced()
        {
            var input = "line1\r\nline2";
            Assert.AreEqual("line1|line2", input.ReplaceNewLine("|"));
        }

        [TestMethod]
        public void ReplaceNewLine_CR_Only_Replaced()
        {
            var input = "line1\rline2";
            Assert.AreEqual("line1 line2", input.ReplaceNewLine(" "));
        }

        [TestMethod]
        public void ReplaceNewLine_LF_Only_Replaced()
        {
            var input = "line1\nline2";
            Assert.AreEqual("line1 line2", input.ReplaceNewLine(" "));
        }

        [TestMethod]
        public void ReplaceNewLine_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", "".ReplaceNewLine(" "));
        }

        [TestMethod]
        public void ReplaceNewLine_NoNewlines_Unchanged()
        {
            Assert.AreEqual("hello world", "hello world".ReplaceNewLine(" "));
        }

        [TestMethod]
        public void ReplaceNewLine_MultipleNewlines_AllReplaced()
        {
            var input = "a\nb\nc\nd";
            Assert.AreEqual("a-b-c-d", input.ReplaceNewLine("-"));
        }

        [TestMethod]
        public void ReplaceNewLine_ReplaceWithEmpty_RemovesNewlines()
        {
            var input = "hello\nworld";
            Assert.AreEqual("helloworld", input.ReplaceNewLine(""));
        }

        // ──────────────────────────────────────────────
        // RemoveLineComment (追加ケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RemoveLineComment_CommentAtBeginning_ReturnsEmpty()
        {
            Assert.AreEqual("", "// full comment".RemoveLineComment());
        }

        [TestMethod]
        public void RemoveLineComment_DataThenComment_ReturnsDataWithTrailingSpace()
        {
            Assert.AreEqual("data ", "data // comment here".RemoveLineComment());
        }

        [TestMethod]
        public void RemoveLineComment_SingleSlash_NotComment()
        {
            Assert.AreEqual("a/b", "a/b".RemoveLineComment());
        }

        [TestMethod]
        public void RemoveLineComment_JapaneseText_WithComment_RemovesComment()
        {
            Assert.AreEqual("日本語 ", "日本語 // コメント".RemoveLineComment());
        }

        [TestMethod]
        public void RemoveLineComment_DoubleQuoteProtects_SlashInQuotes()
        {
            // ダブルクオート内の//はコメントではない（未閉じの場合）
            Assert.AreEqual("\"a//b", "\"a//b".RemoveLineComment());
        }

        [TestMethod]
        public void RemoveLineComment_SingleQuoteProtects_SlashInQuotes()
        {
            Assert.AreEqual("'a//b'", "'a//b'".RemoveLineComment());
        }
    }
}
