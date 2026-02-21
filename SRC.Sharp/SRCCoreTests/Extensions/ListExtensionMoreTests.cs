using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Extensions.Tests
{
    /// <summary>
    /// ListExtension の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class ListExtensionMoreTests
    {
        // ──────────────────────────────────────────────
        // AppendRange
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AppendRange_EmptyBase_AppendsAll()
        {
            IEnumerable<int> list = new List<int>();
            var result = list.AppendRange(new[] { 1, 2, 3 }).ToList();
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
        }

        [TestMethod]
        public void AppendRange_BothEmpty_ReturnsEmpty()
        {
            IEnumerable<int> list = new List<int>();
            var result = list.AppendRange(new List<int>()).ToList();
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void AppendRange_PreservesOriginalOrder()
        {
            IEnumerable<string> list = new[] { "a", "b" };
            var result = list.AppendRange(new[] { "c", "d" }).ToList();
            CollectionAssert.AreEqual(new[] { "a", "b", "c", "d" }, result);
        }

        [TestMethod]
        public void AppendRange_SingleElement()
        {
            IEnumerable<int> list = new[] { 1 };
            var result = list.AppendRange(new[] { 2 }).ToList();
            CollectionAssert.AreEqual(new[] { 1, 2 }, result);
        }

        // ──────────────────────────────────────────────
        // RemoveItem
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RemoveItem_MatchingItems_AreRemoved()
        {
            IList<int> list = new List<int> { 1, 2, 3, 4, 5 };
            list.RemoveItem(x => x % 2 == 0);
            CollectionAssert.AreEqual(new[] { 1, 3, 5 }, list.ToArray());
        }

        [TestMethod]
        public void RemoveItem_NoMatch_ListUnchanged()
        {
            IList<int> list = new List<int> { 1, 3, 5 };
            list.RemoveItem(x => x % 2 == 0);
            Assert.AreEqual(3, list.Count);
        }

        [TestMethod]
        public void RemoveItem_AllMatch_ListEmpty()
        {
            IList<int> list = new List<int> { 2, 4, 6 };
            list.RemoveItem(x => x % 2 == 0);
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void RemoveItem_ReturnsUpdatedList()
        {
            IList<string> list = new List<string> { "apple", "banana", "apricot" };
            var result = list.RemoveItem(x => x.StartsWith("a"));
            Assert.AreSame(list, result);
            CollectionAssert.AreEqual(new[] { "banana" }, result.ToArray());
        }

        // ──────────────────────────────────────────────
        // CloneList
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CloneList_ProducesEqualButSeparateList()
        {
            IList<int> original = new List<int> { 1, 2, 3 };
            var clone = original.CloneList();

            CollectionAssert.AreEqual(original.ToArray(), clone.ToArray());
            Assert.AreNotSame(original, clone);
        }

        [TestMethod]
        public void CloneList_ModifyingClone_DoesNotAffectOriginal()
        {
            IList<int> original = new List<int> { 1, 2, 3 };
            var clone = original.CloneList();
            clone.Add(4);

            Assert.AreEqual(3, original.Count);
            Assert.AreEqual(4, clone.Count);
        }

        [TestMethod]
        public void CloneList_EmptyList_ReturnsEmptyList()
        {
            IList<string> original = new List<string>();
            var clone = original.CloneList();
            Assert.AreEqual(0, clone.Count);
        }

        // ──────────────────────────────────────────────
        // SafeRefOneOffset
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SafeRefOneOffset_NegativeIndex_ReturnsDefault()
        {
            var list = new List<int> { 10, 20, 30 };
            Assert.AreEqual(0, list.SafeRefOneOffset(-1));
        }

        [TestMethod]
        public void SafeRefOneOffset_LargeIndex_ReturnsDefault()
        {
            var list = new List<string> { "a", "b" };
            Assert.IsNull(list.SafeRefOneOffset(100));
        }

        // ──────────────────────────────────────────────
        // SafeRefZeroOffset
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SafeRefZeroOffset_ValidIndex_ReturnsItem()
        {
            var list = new List<int> { 10, 20, 30 };
            Assert.AreEqual(10, list.SafeRefZeroOffset(0));
            Assert.AreEqual(20, list.SafeRefZeroOffset(1));
            Assert.AreEqual(30, list.SafeRefZeroOffset(2));
        }

        [TestMethod]
        public void SafeRefZeroOffset_EmptyList_ReturnsDefault()
        {
            var list = new List<string>();
            Assert.IsNull(list.SafeRefZeroOffset(0));
        }
    }
}
