using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;

namespace SRCCore.Extensions.Tests
{
    /// <summary>
    /// StringExtension クラスの追加テスト（エッジケース）
    /// </summary>
    [TestClass]
    public class StringExtensionMoreTests2
    {
        // ──────────────────────────────────────────────
        // ArrayIndexByName
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayIndexByName_WithIndex_ReturnsIndex()
        {
            Assert.AreEqual("1", "items[1]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_WithStringIndex_ReturnsIndex()
        {
            Assert.AreEqual("key", "map[key]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_WithNestedName_ReturnsIndex()
        {
            Assert.AreEqual("sub", "parent.child[sub]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_NoIndex_ReturnsEmpty()
        {
            Assert.AreEqual("", "items".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_EmptyBrackets_ReturnsEmpty()
        {
            // "x[]" → []内が空なのでマッチしない
            Assert.AreEqual("", "x[]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_WithJapaneseIndex_ReturnsIndex()
        {
            Assert.AreEqual("あ", "名前[あ]".ArrayIndexByName());
        }

        // ──────────────────────────────────────────────
        // InsideKakko
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InsideKakko_WithParens_ReturnsContent()
        {
            Assert.AreEqual("hello", "test(hello)".InsideKakko());
        }

        [TestMethod]
        public void InsideKakko_NoParens_ReturnsEmpty()
        {
            Assert.AreEqual("", "noparen".InsideKakko());
        }

        [TestMethod]
        public void InsideKakko_EmptyParens_ReturnsEmpty()
        {
            // "()" は内部が空なのでマッチしない
            Assert.AreEqual("", "test()".InsideKakko());
        }

        [TestMethod]
        public void InsideKakko_JapaneseContent_ReturnsContent()
        {
            Assert.AreEqual("テスト", "関数(テスト)".InsideKakko());
        }

        [TestMethod]
        public void InsideKakko_NumbersInsideParens_ReturnsNumbers()
        {
            Assert.AreEqual("123", "func(123)".InsideKakko());
        }

        // ──────────────────────────────────────────────
        // ReplaceNewLine
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReplaceNewLine_CrLf_Replaces()
        {
            var result = "a\r\nb".ReplaceNewLine(" ");
            Assert.AreEqual("a b", result);
        }

        [TestMethod]
        public void ReplaceNewLine_LfOnly_Replaces()
        {
            var result = "a\nb".ReplaceNewLine(" ");
            Assert.AreEqual("a b", result);
        }

        [TestMethod]
        public void ReplaceNewLine_CrOnly_Replaces()
        {
            var result = "a\rb".ReplaceNewLine(" ");
            Assert.AreEqual("a b", result);
        }

        [TestMethod]
        public void ReplaceNewLine_NoNewLine_ReturnsOriginal()
        {
            var result = "hello world".ReplaceNewLine(" ");
            Assert.AreEqual("hello world", result);
        }

        [TestMethod]
        public void ReplaceNewLine_MultipleNewLines_ReplacesAll()
        {
            var result = "a\nb\nc".ReplaceNewLine(",");
            Assert.AreEqual("a,b,c", result);
        }

        [TestMethod]
        public void ReplaceNewLine_EmptyReplacement_RemovesNewLines()
        {
            var result = "a\nb\nc".ReplaceNewLine("");
            Assert.AreEqual("abc", result);
        }

        // ──────────────────────────────────────────────
        // RemoveLineComment 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RemoveLineComment_NestedSingleQuoteAndComment_PreservesQuoted()
        {
            // シングルクォート内の // はコメントにならない
            var result = "'url://test'".RemoveLineComment();
            Assert.AreEqual("'url://test'", result);
        }

        [TestMethod]
        public void RemoveLineComment_SpaceBeforeComment_TrimmedCorrectly()
        {
            var result = "code // comment".RemoveLineComment();
            Assert.AreEqual("code ", result);
        }
    }
}
