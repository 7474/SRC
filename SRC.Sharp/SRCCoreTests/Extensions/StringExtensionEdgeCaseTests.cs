using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;

namespace SRCCore.Extensions.Tests
{
    /// <summary>
    /// StringExtension クラスの追加エッジケーステスト
    /// </summary>
    [TestClass]
    public class StringExtensionEdgeCaseTests
    {
        // ──────────────────────────────────────────────
        // ArrayIndexByName - 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayIndexByName_NumericIndex_ReturnsIndex()
        {
            Assert.AreEqual("123", "variable[123]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_JapaneseIndex_ReturnsIndex()
        {
            Assert.AreEqual("日本語", "variable[日本語]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_WithBracketsInIndex_ReturnsNestedIndex()
        {
            Assert.AreEqual("arr[i]", "myvar[arr[i]]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_EmptyIndex_ReturnsEmpty()
        {
            // "var[]" では [] 内が空なのでマッチしない
            Assert.AreEqual("", "var[]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", "".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_OnlyBrackets_ReturnsEmpty()
        {
            Assert.AreEqual("", "[abc]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_SpaceInIndex_ReturnsIndex()
        {
            Assert.AreEqual("a b", "var[a b]".ArrayIndexByName());
        }

        // ──────────────────────────────────────────────
        // InsideKakko - 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InsideKakko_EmptyParens_ReturnsEmpty()
        {
            Assert.AreEqual("", "()".InsideKakko());
        }

        [TestMethod]
        public void InsideKakko_JapaneseContent_ReturnsContent()
        {
            Assert.AreEqual("日本語", "テスト(日本語)".InsideKakko());
        }

        [TestMethod]
        public void InsideKakko_NumberInParens_ReturnsNumber()
        {
            Assert.AreEqual("42", "value(42)".InsideKakko());
        }

        [TestMethod]
        public void InsideKakko_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", "".InsideKakko());
        }

        [TestMethod]
        public void InsideKakko_NoParens_ReturnsEmpty()
        {
            Assert.AreEqual("", "hello world".InsideKakko());
        }

        [TestMethod]
        public void InsideKakko_MultipleParens_ReturnsGreedyMatch()
        {
            // 実装の正規表現は貪欲マッチ。"(first)(second)" -> "first)(second"
            var result = "(first)(second)".InsideKakko();
            Assert.AreEqual("first)(second", result);
        }

        // ──────────────────────────────────────────────
        // ReplaceNewLine - 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReplaceNewLine_CrLf_ReplacedCorrectly()
        {
            var result = "hello\r\nworld".ReplaceNewLine(" ");
            Assert.AreEqual("hello world", result);
        }

        [TestMethod]
        public void ReplaceNewLine_Cr_ReplacedCorrectly()
        {
            var result = "hello\rworld".ReplaceNewLine(" ");
            Assert.AreEqual("hello world", result);
        }

        [TestMethod]
        public void ReplaceNewLine_Lf_ReplacedCorrectly()
        {
            var result = "hello\nworld".ReplaceNewLine(" ");
            Assert.AreEqual("hello world", result);
        }

        [TestMethod]
        public void ReplaceNewLine_NoNewLine_ReturnsOriginal()
        {
            var result = "hello world".ReplaceNewLine(" ");
            Assert.AreEqual("hello world", result);
        }

        [TestMethod]
        public void ReplaceNewLine_EmptyString_ReturnsEmpty()
        {
            var result = "".ReplaceNewLine(" ");
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void ReplaceNewLine_ReplaceWithEmpty_RemovesNewLines()
        {
            var result = "a\nb\nc".ReplaceNewLine("");
            Assert.AreEqual("abc", result);
        }

        [TestMethod]
        public void ReplaceNewLine_MultipleNewLines_AllReplaced()
        {
            var result = "a\n\n\nb".ReplaceNewLine("X");
            Assert.AreEqual("aXXXb", result);
        }

        // ──────────────────────────────────────────────
        // RemoveLineComment - 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RemoveLineComment_JapaneseBeforeComment_KeepsJapanese()
        {
            Assert.AreEqual("日本語 ", "日本語 // コメント".RemoveLineComment());
        }

        [TestMethod]
        public void RemoveLineComment_SlashNotDouble_KeepsSlash()
        {
            // 単独の "/" はコメントではない
            Assert.AreEqual("a/b", "a/b".RemoveLineComment());
        }

        [TestMethod]
        public void RemoveLineComment_ThreeSlashes_CutsAtSecondSlash()
        {
            // "///comment" -> 2番目の "/" が来た時点でコメント開始。Left("///comment", 0) = ""
            var result = "///comment".RemoveLineComment();
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void RemoveLineComment_DataEqualsWithComment_OnlyKeepsData()
        {
            Assert.AreEqual("x=5 ", "x=5 // comment".RemoveLineComment());
        }
    }
}
