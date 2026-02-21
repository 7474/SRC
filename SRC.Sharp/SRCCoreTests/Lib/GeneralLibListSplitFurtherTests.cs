using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// GeneralLib.ListSplit のユニットテスト（括弧対応・エラーケース）
    /// </summary>
    [TestClass]
    public class GeneralLibListSplitFurtherTests
    {
        // ──────────────────────────────────────────────
        // ListSplit - 基本ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ListSplit_SingleElement_ReturnsOneElement()
        {
            var count = GeneralLib.ListSplit("abc", out var arr);
            Assert.AreEqual(1, count);
            Assert.AreEqual("abc", arr[0]);
        }

        [TestMethod]
        public void ListSplit_TwoElements_ReturnsTwoElements()
        {
            var count = GeneralLib.ListSplit("abc def", out var arr);
            Assert.AreEqual(2, count);
            Assert.AreEqual("abc", arr[0]);
            Assert.AreEqual("def", arr[1]);
        }

        [TestMethod]
        public void ListSplit_EmptyString_ReturnsZero()
        {
            var count = GeneralLib.ListSplit("", out var arr);
            Assert.AreEqual(0, count);
            Assert.AreEqual(0, arr.Length);
        }

        [TestMethod]
        public void ListSplit_Null_ReturnsZero()
        {
            var count = GeneralLib.ListSplit(null, out var arr);
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void ListSplit_MultipleSpaces_SkipsBlanks()
        {
            var count = GeneralLib.ListSplit("a  b  c", out var arr);
            Assert.AreEqual(3, count);
            Assert.AreEqual("a", arr[0]);
            Assert.AreEqual("b", arr[1]);
            Assert.AreEqual("c", arr[2]);
        }

        [TestMethod]
        public void ListSplit_WithParentheses_TreatedAsOneElement()
        {
            var count = GeneralLib.ListSplit("a (b c) d", out var arr);
            Assert.AreEqual(3, count);
            Assert.AreEqual("a", arr[0]);
            Assert.AreEqual("(b c)", arr[1]);
            Assert.AreEqual("d", arr[2]);
        }

        [TestMethod]
        public void ListSplit_UnbalancedParentheses_ReturnsNegativeOne()
        {
            var count = GeneralLib.ListSplit("a ) b", out var arr);
            Assert.AreEqual(-1, count);
        }

        // ──────────────────────────────────────────────
        // ListLength - 基本ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ListLength_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.ListLength(""));
        }

        [TestMethod]
        public void ListLength_Null_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.ListLength(null));
        }

        [TestMethod]
        public void ListLength_SingleElement_ReturnsOne()
        {
            Assert.AreEqual(1, GeneralLib.ListLength("abc"));
        }

        [TestMethod]
        public void ListLength_ThreeElements_ReturnsThree()
        {
            Assert.AreEqual(3, GeneralLib.ListLength("a b c"));
        }

        [TestMethod]
        public void ListLength_WithParentheses_CountsAsOne()
        {
            Assert.AreEqual(3, GeneralLib.ListLength("a (b c) d"));
        }

        // ──────────────────────────────────────────────
        // ListIndex - 基本ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ListIndex_FirstElement_ReturnsFirst()
        {
            Assert.AreEqual("a", GeneralLib.ListIndex("a b c", 1));
        }

        [TestMethod]
        public void ListIndex_LastElement_ReturnsLast()
        {
            Assert.AreEqual("c", GeneralLib.ListIndex("a b c", 3));
        }

        [TestMethod]
        public void ListIndex_ZeroIndex_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.ListIndex("a b c", 0));
        }

        [TestMethod]
        public void ListIndex_OutOfRange_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.ListIndex("a b c", 10));
        }

        [TestMethod]
        public void ListIndex_NegativeIndex_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.ListIndex("a b c", -1));
        }

        [TestMethod]
        public void ListIndex_ParenthesizedElement_ReturnsWithParentheses()
        {
            // 括弧を含む要素はそのまま返す
            Assert.AreEqual("(b c)", GeneralLib.ListIndex("a (b c) d", 2));
        }

        // ──────────────────────────────────────────────
        // ToL - 基本ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToL_EmptyString_ReturnsEmptyList()
        {
            Assert.AreEqual(0, GeneralLib.ToL("").Count);
        }

        [TestMethod]
        public void ToL_Null_ReturnsEmptyList()
        {
            Assert.AreEqual(0, GeneralLib.ToL(null).Count);
        }

        [TestMethod]
        public void ToL_SpaceDelimited_ReturnsElements()
        {
            var list = GeneralLib.ToL("x y z");
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("x", list[0]);
        }

        [TestMethod]
        public void LLength_Null_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.LLength(null));
        }

        [TestMethod]
        public void LLength_Empty_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.LLength(""));
        }

        [TestMethod]
        public void LLength_ThreeWords_ReturnsThree()
        {
            Assert.AreEqual(3, GeneralLib.LLength("a b c"));
        }

        [TestMethod]
        public void LIndex_FirstWord_ReturnsFirst()
        {
            Assert.AreEqual("a", GeneralLib.LIndex("a b c", 1));
        }

        [TestMethod]
        public void LIndex_OutOfRange_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.LIndex("a b c", 5));
        }

        [TestMethod]
        public void LIndex_ZeroIndex_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.LIndex("a b c", 0));
        }
    }
}
