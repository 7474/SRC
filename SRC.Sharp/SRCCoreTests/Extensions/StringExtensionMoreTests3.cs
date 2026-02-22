using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;

namespace SRCCore.Extensions.Tests
{
    [TestClass]
    public class StringExtensionMoreTests3
    {
        // ArrayIndexByName

        [TestMethod]
        public void ArrayIndexByName_SimpleIndex_ReturnsIndex()
        {
            Assert.AreEqual("1", "var[1]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_StringIndex_ReturnsIndex()
        {
            Assert.AreEqual("key", "arr[key]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_NoSquareBracket_ReturnsEmpty()
        {
            Assert.AreEqual("", "noindex".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_BracketAtStart_ReturnsEmpty()
        {
            Assert.AreEqual("", "[1]name".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_NestedBracket_ReturnsInnerExpression()
        {
            Assert.AreEqual("inner[x]", "outer[inner[x]]".ArrayIndexByName());
        }

        // InsideKakko

        [TestMethod]
        public void InsideKakko_NoBracket_ReturnsEmpty()
        {
            Assert.AreEqual("", "hello".InsideKakko());
        }

        [TestMethod]
        public void InsideKakko_WithContent_ReturnsContent()
        {
            Assert.AreEqual("abc", "prefix(abc)".InsideKakko());
        }

        [TestMethod]
        public void InsideKakko_Nested_ReturnsInnerWithNested()
        {
            Assert.AreEqual("a(b)", "(a(b))".InsideKakko());
        }

        // ReplaceNewLine

        [TestMethod]
        public void ReplaceNewLine_CRLF_IsReplaced()
        {
            Assert.AreEqual("a b", "a\r\nb".ReplaceNewLine(" "));
        }

        [TestMethod]
        public void ReplaceNewLine_LF_IsReplaced()
        {
            Assert.AreEqual("a|b", "a\nb".ReplaceNewLine("|"));
        }

        [TestMethod]
        public void ReplaceNewLine_CR_IsReplaced()
        {
            Assert.AreEqual("a-b", "a\rb".ReplaceNewLine("-"));
        }

        [TestMethod]
        public void ReplaceNewLine_NoNewline_ReturnsOriginal()
        {
            Assert.AreEqual("hello", "hello".ReplaceNewLine(" "));
        }

        // RemoveLineComment

        [TestMethod]
        public void RemoveLineComment_CommentOnly_ReturnsEmpty()
        {
            Assert.AreEqual("", "//comment".RemoveLineComment());
        }

        [TestMethod]
        public void RemoveLineComment_CodeBeforeComment_ReturnsCode()
        {
            Assert.AreEqual("code", "code//comment".RemoveLineComment());
        }

        [TestMethod]
        public void RemoveLineComment_NoSlash_ReturnsOriginal()
        {
            Assert.AreEqual("abc def", "abc def".RemoveLineComment());
        }

        [TestMethod]
        public void RemoveLineComment_DoubleQuotedContent_PreservesSlashes()
        {
            Assert.AreEqual("\"a//b", "\"a//b".RemoveLineComment());
        }
    }
}
