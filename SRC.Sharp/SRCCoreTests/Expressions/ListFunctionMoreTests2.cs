using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression 経由のリスト関数追加テスト
    /// </summary>
    [TestClass]
    public class ListFunctionMoreTests2
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // LLength 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LLength_FourItems_ReturnsFour()
        {
            var exp = Create();
            Assert.AreEqual(4d, exp.GetValueAsDouble("LLength(\"a b c d\")"));
        }

        [TestMethod]
        public void LLength_JapaneseItems_CountsCorrectly()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("LLength(\"炎 水 風\")"));
        }

        [TestMethod]
        public void LLength_NumericItems_CountsCorrectly()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("LLength(\"10 20 30 40 50\")"));
        }

        // ──────────────────────────────────────────────
        // LIndex 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LIndex_LastItem_ReturnsLast()
        {
            var exp = Create();
            Assert.AreEqual("cherry", exp.GetValueAsString("LIndex(\"apple banana cherry\",3)"));
        }

        [TestMethod]
        public void LIndex_JapaneseItems_ReturnsCorrect()
        {
            var exp = Create();
            Assert.AreEqual("水", exp.GetValueAsString("LIndex(\"炎 水 風\",2)"));
        }

        [TestMethod]
        public void LIndex_BeyondList_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("LIndex(\"a b c\",10)"));
        }

        [TestMethod]
        public void LIndex_FirstElement_ReturnsFirst()
        {
            var exp = Create();
            Assert.AreEqual("first", exp.GetValueAsString("LIndex(\"first second third\",1)"));
        }

        // ──────────────────────────────────────────────
        // LSearch 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LSearch_JapaneseItem_FindsPosition()
        {
            var exp = Create();
            Assert.AreEqual(2d, exp.GetValueAsDouble("LSearch(\"炎 水 風\",\"水\")"));
        }

        [TestMethod]
        public void LSearch_ItemNotExists_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("LSearch(\"a b c\",\"z\")"));
        }

        [TestMethod]
        public void LSearch_LastItem_FindsLastPosition()
        {
            var exp = Create();
            Assert.AreEqual(4d, exp.GetValueAsDouble("LSearch(\"a b c d\",\"d\")"));
        }

        [TestMethod]
        public void LSearch_WithStartPos_SearchesFromPosition()
        {
            var exp = Create();
            // "a b a c" で "a" を位置2から検索 → 3
            Assert.AreEqual(3d, exp.GetValueAsDouble("LSearch(\"a b a c\",\"a\",2)"));
        }

        // ──────────────────────────────────────────────
        // List 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void List_FourArgs_ReturnsFourItems()
        {
            var exp = Create();
            var result = exp.GetValueAsString("List(\"a\",\"b\",\"c\",\"d\")");
            Assert.AreEqual("a b c d", result);
        }

        [TestMethod]
        public void List_JapaneseArgs_ReturnsSpaceSeparated()
        {
            var exp = Create();
            Assert.AreEqual("炎 水 風", exp.GetValueAsString("List(\"炎\",\"水\",\"風\")"));
        }

        [TestMethod]
        public void List_MixedTypes_ReturnsSpaceSeparated()
        {
            var exp = Create();
            Assert.AreEqual("test 42", exp.GetValueAsString("List(\"test\",42)"));
        }
    }
}
