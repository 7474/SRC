using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;

namespace SRCCore.Extensions.Tests
{
    /// <summary>
    /// StringExtension の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class StringExtensionMoreTests
    {
        // ──────────────────────────────────────────────
        // RemoveLineComment
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RemoveLineComment_NoComment_ReturnsOriginal()
        {
            Assert.AreEqual("abc def", "abc def".RemoveLineComment());
        }

        [TestMethod]
        public void RemoveLineComment_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", "".RemoveLineComment());
        }

        [TestMethod]
        public void RemoveLineComment_OnlyComment_ReturnsEmpty()
        {
            Assert.AreEqual("", "// comment".RemoveLineComment());
        }

        [TestMethod]
        public void RemoveLineComment_CodeAndComment_ReturnsCodePart()
        {
            Assert.AreEqual("Set x 1 ", "Set x 1 // set x".RemoveLineComment());
        }

        [TestMethod]
        public void RemoveLineComment_CommentInsideSingleQuote_NotRemoved()
        {
            // シングルクォート内の // はコメントとして扱わない
            Assert.AreEqual("'// not a comment'", "'// not a comment'".RemoveLineComment());
        }

        [TestMethod]
        public void RemoveLineComment_CommentInsideDoubleQuote_NotRemoved()
        {
            // ダブルクォート内の // はコメントとして扱わない
            Assert.AreEqual("\"// not a comment\"", "\"// not a comment\"".RemoveLineComment());
        }

        [TestMethod]
        public void RemoveLineComment_SingleSlash_NotRemoved()
        {
            // スラッシュが1つだけはコメントでない
            Assert.AreEqual("a/b", "a/b".RemoveLineComment());
        }

        // ──────────────────────────────────────────────
        // ReplaceNewLine
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReplaceNewLine_CRLFToSpace()
        {
            Assert.AreEqual("a b", "a\r\nb".ReplaceNewLine(" "));
        }

        [TestMethod]
        public void ReplaceNewLine_LFToSpace()
        {
            Assert.AreEqual("a b", "a\nb".ReplaceNewLine(" "));
        }

        [TestMethod]
        public void ReplaceNewLine_CRToSpace()
        {
            Assert.AreEqual("a b", "a\rb".ReplaceNewLine(" "));
        }

        [TestMethod]
        public void ReplaceNewLine_EmptyReplace()
        {
            Assert.AreEqual("ab", "a\nb".ReplaceNewLine(""));
        }

        [TestMethod]
        public void ReplaceNewLine_NoNewline_Unchanged()
        {
            Assert.AreEqual("hello", "hello".ReplaceNewLine(" "));
        }

        [TestMethod]
        public void ReplaceNewLine_MultipleNewlines()
        {
            Assert.AreEqual("a_b_c", "a\nb\nc".ReplaceNewLine("_"));
        }

        // ──────────────────────────────────────────────
        // ArrayIndexByName
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayIndexByName_WithNumericIndex_ReturnsIndex()
        {
            Assert.AreEqual("42", "var[42]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_WithStringIndex_ReturnsIndex()
        {
            Assert.AreEqual("key", "var[key]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_WithNestedBrackets_ReturnsOuterIndex()
        {
            Assert.AreEqual("inner[1]", "outer[inner[1]]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_NoPrefix_ReturnsEmpty()
        {
            Assert.AreEqual("", "[1]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_NoSuffix_ReturnsEmpty()
        {
            Assert.AreEqual("", "var".ArrayIndexByName());
        }

        // ──────────────────────────────────────────────
        // InsideKakko
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InsideKakko_WithParens_ReturnsInner()
        {
            Assert.AreEqual("test", "prefix(test)suffix".InsideKakko());
        }

        [TestMethod]
        public void InsideKakko_EmptyParens_ReturnsEmpty()
        {
            // 括弧の中身が空の場合は正規表現が (.+) のため空を返す
            Assert.AreEqual("", "()".InsideKakko());
        }

        [TestMethod]
        public void InsideKakko_NoParens_ReturnsEmpty()
        {
            Assert.AreEqual("", "noparen".InsideKakko());
        }

        [TestMethod]
        public void InsideKakko_NestedParens_ReturnsInnerIncludingParens()
        {
            Assert.AreEqual("inner(x)", "(inner(x))".InsideKakko());
        }
    }
}
