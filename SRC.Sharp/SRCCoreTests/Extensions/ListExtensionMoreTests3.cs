using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Extensions.Tests
{
    [TestClass]
    public class ListExtensionMoreTests3
    {
        [TestMethod]
        public void SafeRefOneOffset_ValidIndex_ReturnsValue()
        {
            var list = new List<string> { "a", "b", "c" };
            Assert.AreEqual("a", list.SafeRefOneOffset(1));
            Assert.AreEqual("b", list.SafeRefOneOffset(2));
            Assert.AreEqual("c", list.SafeRefOneOffset(3));
        }

        [TestMethod]
        public void SafeRefOneOffset_ZeroIndex_ReturnsDefault()
        {
            var list = new List<string> { "a", "b" };
            Assert.IsNull(list.SafeRefOneOffset(0));
        }

        [TestMethod]
        public void SafeRefOneOffset_OutOfRange_ReturnsDefault()
        {
            var list = new List<int> { 10, 20 };
            Assert.AreEqual(0, list.SafeRefOneOffset(5));
        }

        [TestMethod]
        public void SafeRefZeroOffset_ValidIndex_ReturnsValue()
        {
            var list = new List<int> { 7, 8, 9 };
            Assert.AreEqual(7, list.SafeRefZeroOffset(0));
            Assert.AreEqual(9, list.SafeRefZeroOffset(2));
        }

        [TestMethod]
        public void SafeRefZeroOffset_NegativeIndex_ReturnsDefault()
        {
            var list = new List<int> { 1, 2 };
            Assert.AreEqual(0, list.SafeRefZeroOffset(-1));
        }

        [TestMethod]
        public void RemoveItem_MatchingPredicate_RemovesItems()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };
            list.RemoveItem(x => x % 2 == 0);
            CollectionAssert.AreEqual(new[] { 1, 3, 5 }, list.ToArray());
        }

        [TestMethod]
        public void RemoveItem_NoMatch_ListUnchanged()
        {
            var list = new List<int> { 1, 3, 5 };
            list.RemoveItem(x => x % 2 == 0);
            Assert.AreEqual(3, list.Count);
        }

        [TestMethod]
        public void CloneList_ReturnsNewListWithSameContent()
        {
            var original = new List<string> { "x", "y", "z" };
            var clone = original.CloneList();
            CollectionAssert.AreEqual(original, clone);
            Assert.AreNotSame(original, clone);
        }

        [TestMethod]
        public void AppendRange_EmptyAppend_ReturnsOriginal()
        {
            IEnumerable<int> list = new List<int> { 1, 2 };
            var result = list.AppendRange(new List<int>());
            CollectionAssert.AreEqual(new[] { 1, 2 }, result.ToList());
        }

        [TestMethod]
        public void AppendRange_MultipleItems_AppendsAll()
        {
            IEnumerable<string> list = new List<string> { "a" };
            var result = list.AppendRange(new List<string> { "b", "c" });
            CollectionAssert.AreEqual(new[] { "a", "b", "c" }, result.ToList());
        }

        [TestMethod]
        public void SafeRefZeroOffset_ExactlyAtBoundary_ReturnsValue()
        {
            var list = new List<int> { 100 };
            Assert.AreEqual(100, list.SafeRefZeroOffset(0));
            Assert.AreEqual(0, list.SafeRefZeroOffset(1));
        }
    }
}
