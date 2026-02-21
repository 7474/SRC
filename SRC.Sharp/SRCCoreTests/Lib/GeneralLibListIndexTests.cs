using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using System.Linq;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// GeneralLib の括弧考慮リスト処理 (ListIndex, ListLength, ListSplit, ToL) のユニットテスト
    /// </summary>
    [TestClass]
    public class GeneralLibListIndexTests
    {
        // ──────────────────────────────────────────────
        // ListIndex - 括弧を考慮した要素取得
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ListIndex_FirstElement_ReturnsFirst()
        {
            Assert.AreEqual("alpha", GeneralLib.ListIndex("alpha beta gamma", 1));
        }

        [TestMethod]
        public void ListIndex_SecondElement_ReturnsSecond()
        {
            Assert.AreEqual("beta", GeneralLib.ListIndex("alpha beta gamma", 2));
        }

        [TestMethod]
        public void ListIndex_ThirdElement_ReturnsThird()
        {
            Assert.AreEqual("gamma", GeneralLib.ListIndex("alpha beta gamma", 3));
        }

        [TestMethod]
        public void ListIndex_OutOfBounds_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.ListIndex("a b", 5));
        }

        [TestMethod]
        public void ListIndex_NegativeIndex_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.ListIndex("a b c", -1));
        }

        [TestMethod]
        public void ListIndex_ZeroIndex_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.ListIndex("a b c", 0));
        }

        [TestMethod]
        public void ListIndex_ElementWithParens_TreatedAsSingleToken()
        {
            // "(a b)"は括弧内なので一つのトークン
            var result = GeneralLib.ListIndex("x (a b) y", 2);
            Assert.AreEqual("(a b)", result);
        }

        [TestMethod]
        public void ListIndex_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.ListIndex("", 1));
        }

        [TestMethod]
        public void ListIndex_NullString_ReturnsEmpty()
        {
            Assert.AreEqual("", GeneralLib.ListIndex(null, 1));
        }

        [TestMethod]
        public void ListIndex_NestedParens_TreatedAsSingleToken()
        {
            var result = GeneralLib.ListIndex("a (b (c d) e) f", 2);
            Assert.AreEqual("(b (c d) e)", result);
        }

        [TestMethod]
        public void ListIndex_SquareBrackets_TreatedAsSingleToken()
        {
            var result = GeneralLib.ListIndex("a [b c] d", 2);
            Assert.AreEqual("[b c]", result);
        }

        // ──────────────────────────────────────────────
        // ListLength - 括弧を考慮した要素数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ListLength_ThreeElements_ReturnsThree()
        {
            Assert.AreEqual(3, GeneralLib.ListLength("a b c"));
        }

        [TestMethod]
        public void ListLength_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.ListLength(""));
        }

        [TestMethod]
        public void ListLength_NullString_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.ListLength(null));
        }

        [TestMethod]
        public void ListLength_SingleElement_ReturnsOne()
        {
            Assert.AreEqual(1, GeneralLib.ListLength("abc"));
        }

        [TestMethod]
        public void ListLength_ElementWithParens_CountsAsSingle()
        {
            // "(a b)" は括弧内なので一つのトークンとして数える
            Assert.AreEqual(3, GeneralLib.ListLength("x (a b) y"));
        }

        [TestMethod]
        public void ListLength_MultipleParenGroups_CountsCorrectly()
        {
            Assert.AreEqual(3, GeneralLib.ListLength("(a b) (c d) e"));
        }

        [TestMethod]
        public void ListLength_NestedParens_CountsAsSingle()
        {
            Assert.AreEqual(2, GeneralLib.ListLength("a (b (c))"));
        }

        // ──────────────────────────────────────────────
        // ListSplit
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ListSplit_ThreeElements_ReturnsArrayOfThree()
        {
            int count = GeneralLib.ListSplit("a b c", out string[] arr);
            Assert.AreEqual(3, count);
            Assert.AreEqual(3, arr.Length);
            Assert.AreEqual("a", arr[0]);
            Assert.AreEqual("b", arr[1]);
            Assert.AreEqual("c", arr[2]);
        }

        [TestMethod]
        public void ListSplit_EmptyString_ReturnsZeroAndEmptyArray()
        {
            int count = GeneralLib.ListSplit("", out string[] arr);
            Assert.AreEqual(0, count);
            Assert.AreEqual(0, arr.Length);
        }

        [TestMethod]
        public void ListSplit_SingleElement_ReturnsArrayOfOne()
        {
            int count = GeneralLib.ListSplit("abc", out string[] arr);
            Assert.AreEqual(1, count);
            Assert.AreEqual("abc", arr[0]);
        }

        [TestMethod]
        public void ListSplit_ParenthesizedGroup_TreatedAsSingleToken()
        {
            int count = GeneralLib.ListSplit("a (b c) d", out string[] arr);
            Assert.AreEqual(3, count);
            Assert.AreEqual("(b c)", arr[1]);
        }

        [TestMethod]
        public void ListSplit_UnmatchedCloseParen_ReturnsMinusOne()
        {
            int count = GeneralLib.ListSplit("a ) b", out string[] arr);
            Assert.AreEqual(-1, count);
        }

        // ──────────────────────────────────────────────
        // ToL - シンプルなスペース区切り分割
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToL_ThreeElements_ReturnsThree()
        {
            var result = GeneralLib.ToL("a b c");
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void ToL_EmptyString_ReturnsEmpty()
        {
            var result = GeneralLib.ToL("");
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ToL_NullString_ReturnsEmpty()
        {
            var result = GeneralLib.ToL(null);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ToL_MultipleSpaces_IgnoresExtraSpaces()
        {
            var result = GeneralLib.ToL("a  b  c");
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("a", result[0]);
            Assert.AreEqual("b", result[1]);
            Assert.AreEqual("c", result[2]);
        }

        [TestMethod]
        public void ToL_SingleElement_ReturnsOne()
        {
            var result = GeneralLib.ToL("hello");
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("hello", result[0]);
        }

        [TestMethod]
        public void ToL_LeadingAndTrailingSpaces_IgnoresSpaces()
        {
            var result = GeneralLib.ToL(" a b ");
            Assert.AreEqual(2, result.Count);
        }

        // ──────────────────────────────────────────────
        // timeGetTime
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TimeGetTime_ReturnsNonNegativeValue()
        {
            var time = GeneralLib.timeGetTime();
            Assert.IsTrue(time >= 0);
        }

        [TestMethod]
        public void TimeGetTime_CalledTwice_SecondIsGreaterOrEqual()
        {
            var t1 = GeneralLib.timeGetTime();
            var t2 = GeneralLib.timeGetTime();
            Assert.IsTrue(t2 >= t1);
        }
    }
}
