using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression 経由で呼び出すリスト関数のさらなる追加テスト
    /// （ListFunctionTests.cs 未カバー分）
    /// </summary>
    [TestClass]
    public class ListFunctionMoreTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // LIndex: 括弧付き要素 → 括弧を除去する仕様
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LIndex_ParenthesizedElement_RemovesParens()
        {
            var exp = Create();
            // "(abc)" は括弧を外して "abc" が返る
            Assert.AreEqual("abc", exp.GetValueAsString("LIndex(\"(abc) def\",1)"));
        }

        [TestMethod]
        public void LIndex_NumericResult_ReturnsNumber()
        {
            var exp = Create();
            Assert.AreEqual(20d, exp.GetValueAsDouble("LIndex(\"10 20 30\",2)"));
        }

        [TestMethod]
        public void LIndex_NegativeIndex_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("LIndex(\"a b c\",-1)"));
        }

        // ──────────────────────────────────────────────
        // LLength: 文字列型で返す
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LLength_StringResult_ReturnsFormattedNumber()
        {
            var exp = Create();
            Assert.AreEqual("3", exp.GetValueAsString("LLength(\"a b c\")"));
        }

        [TestMethod]
        public void LLength_SingleNumericElement_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("LLength(\"42\")"));
        }

        // ──────────────────────────────────────────────
        // LSearch: 文字列型で返す・開始位置エッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LSearch_StringResult_ReturnsFormattedPosition()
        {
            var exp = Create();
            Assert.AreEqual("2", exp.GetValueAsString("LSearch(\"a b c\",\"b\")"));
        }

        [TestMethod]
        public void LSearch_StringResult_NotFound_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual("0", exp.GetValueAsString("LSearch(\"a b c\",\"z\")"));
        }

        [TestMethod]
        public void LSearch_WithStartBeyondAllMatches_ReturnsZero()
        {
            var exp = Create();
            // "a b c" でインデックス3以降から "a" を探す → 見つからない → 0
            Assert.AreEqual(0d, exp.GetValueAsDouble("LSearch(\"a b c\",\"a\",3)"));
        }

        [TestMethod]
        public void LSearch_WithStartEqualToMatch_ReturnsPosition()
        {
            var exp = Create();
            // "a b a" で 3 番目から "a" を探す → 3
            Assert.AreEqual(3d, exp.GetValueAsDouble("LSearch(\"a b a\",\"a\",3)"));
        }

        [TestMethod]
        public void LSearch_DuplicateElements_ReturnsFirstFromStart()
        {
            var exp = Create();
            // "x x x" で先頭から探す → 1
            Assert.AreEqual(1d, exp.GetValueAsDouble("LSearch(\"x x x\",\"x\")"));
        }

        // ──────────────────────────────────────────────
        // List: 混合型・追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void List_MixedStringAndNumber_ReturnSpaceSeparated()
        {
            var exp = Create();
            Assert.AreEqual("hello 42 world", exp.GetValueAsString("List(\"hello\",42,\"world\")"));
        }

        [TestMethod]
        public void List_TwoArgs_ReturnSpaceSeparated()
        {
            var exp = Create();
            Assert.AreEqual("foo bar", exp.GetValueAsString("List(\"foo\",\"bar\")"));
        }

        [TestMethod]
        public void List_JapaneseArgs_ReturnSpaceSeparated()
        {
            var exp = Create();
            Assert.AreEqual("りんご みかん", exp.GetValueAsString("List(\"りんご\",\"みかん\")"));
        }

        // ──────────────────────────────────────────────
        // LIndex: さらなるエッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LIndex_SingleElement_LengthOne_ReturnsElement()
        {
            var exp = Create();
            Assert.AreEqual("only", exp.GetValueAsString("LIndex(\"only\",1)"));
        }

        [TestMethod]
        public void LIndex_JapaneseNumericList_ReturnsElement()
        {
            var exp = Create();
            Assert.AreEqual("20", exp.GetValueAsString("LIndex(\"10 20 30\",2)"));
        }

        // ──────────────────────────────────────────────
        // LLength / LSearch: 数値リスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LLength_NumericList_ReturnsCount()
        {
            var exp = Create();
            Assert.AreEqual(4d, exp.GetValueAsDouble("LLength(\"10 20 30 40\")"));
        }

        [TestMethod]
        public void LSearch_NumericList_FindsElement()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("LSearch(\"10 20 30\",\"30\")"));
        }

        [TestMethod]
        public void LSearch_NumericList_NotFound_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("LSearch(\"10 20 30\",\"99\")"));
        }
    }
}
