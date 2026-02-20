using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression経由で呼び出すリスト関数のユニットテスト
    /// </summary>
    [TestClass]
    public class ListFunctionTests
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
        // List
        // ──────────────────────────────────────────────

        [TestMethod]
        public void List_SingleArg_ReturnsSingleElement()
        {
            var exp = Create();
            Assert.AreEqual("abc", exp.GetValueAsString("List(\"abc\")"));
        }

        [TestMethod]
        public void List_MultipleArgs_ReturnSpaceSeparated()
        {
            var exp = Create();
            Assert.AreEqual("a b c", exp.GetValueAsString("List(\"a\",\"b\",\"c\")"));
        }

        [TestMethod]
        public void List_NumericArgs_ReturnSpaceSeparated()
        {
            var exp = Create();
            Assert.AreEqual("1 2 3", exp.GetValueAsString("List(1,2,3)"));
        }

        // ──────────────────────────────────────────────
        // LLength
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LLength_SpaceSeparated_ReturnsCount()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("LLength(\"a b c\")"));
        }

        [TestMethod]
        public void LLength_SingleElement_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("LLength(\"abc\")"));
        }

        [TestMethod]
        public void LLength_EmptyString_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("LLength(\"\")"));
        }

        // ──────────────────────────────────────────────
        // LIndex
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LIndex_FirstElement_ReturnsFirst()
        {
            var exp = Create();
            Assert.AreEqual("a", exp.GetValueAsString("LIndex(\"a b c\",1)"));
        }

        [TestMethod]
        public void LIndex_SecondElement_ReturnsSecond()
        {
            var exp = Create();
            Assert.AreEqual("b", exp.GetValueAsString("LIndex(\"a b c\",2)"));
        }

        [TestMethod]
        public void LIndex_OutOfBounds_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("LIndex(\"a b c\",5)"));
        }

        [TestMethod]
        public void LIndex_JapaneseList_ReturnsCorrectElement()
        {
            var exp = Create();
            Assert.AreEqual("みかん", exp.GetValueAsString("LIndex(\"りんご みかん ぶどう\",2)"));
        }

        // ──────────────────────────────────────────────
        // LSearch
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LSearch_Found_ReturnsPosition()
        {
            var exp = Create();
            Assert.AreEqual(2d, exp.GetValueAsDouble("LSearch(\"a b c\",\"b\")"));
        }

        [TestMethod]
        public void LSearch_NotFound_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("LSearch(\"a b c\",\"z\")"));
        }

        [TestMethod]
        public void LSearch_WithStartPosition_SearchesFromPosition()
        {
            var exp = Create();
            // "a b a c" でaを2番目以降から検索 → 3番目のaが返る
            Assert.AreEqual(3d, exp.GetValueAsDouble("LSearch(\"a b a c\",\"a\",2)"));
        }
    }
}
