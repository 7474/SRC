using SRCCore.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Extensions.Tests
{
    [TestClass()]
    public class ListExtensionTests
    {
        [TestMethod()]
        public void SafeRefOneOffsetTest()
        {
            var list = new List<int>() { 1, 2, 3 };
            Assert.AreEqual(0, list.SafeRefOneOffset(0));
            Assert.AreEqual(1, list.SafeRefOneOffset(1));
            Assert.AreEqual(2, list.SafeRefOneOffset(2));
            Assert.AreEqual(3, list.SafeRefOneOffset(3));
            Assert.AreEqual(0, list.SafeRefOneOffset(4));
        }

        [TestMethod()]
        public void SafeRefZeroOffsetTest()
        {
            var list = new List<string>() { "1", "2", "3" };
            Assert.AreEqual(null, list.SafeRefZeroOffset(-1));
            Assert.AreEqual("1", list.SafeRefZeroOffset(0));
            Assert.AreEqual("2", list.SafeRefZeroOffset(1));
            Assert.AreEqual("3", list.SafeRefZeroOffset(2));
            Assert.AreEqual(null, list.SafeRefZeroOffset(3));
        }

        [TestMethod()]
        public void CloneListTest()
        {
            var orgArray = new string[] { "1", "2", "3" };
            var clone1 = orgArray.CloneList();
            var clone2 = clone1.CloneList();
            var clone3 = clone2.CloneList();

            Assert.IsTrue(orgArray.SequenceEqual(clone1));
            Assert.IsTrue(orgArray.SequenceEqual(clone2));
            Assert.IsTrue(orgArray.SequenceEqual(clone3));
            Assert.AreNotEqual(clone1, clone2);
            Assert.AreNotEqual(clone2, clone3);
        }

        [TestMethod()]
        public void AppendRangeTest()
        {
            IEnumerable<int> list = new List<int> { 1, 2, 3 };
            var appended = list.AppendRange(new List<int> { 4, 5 });
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, appended.ToList());
        }

        [TestMethod()]
        public void AppendRangeTest_EmptyAppend()
        {
            IEnumerable<int> list = new List<int> { 1, 2 };
            var result = list.AppendRange(new List<int>());
            CollectionAssert.AreEqual(new[] { 1, 2 }, result.ToList());
        }

        [TestMethod()]
        public void RemoveItemTest()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };
            list.RemoveItem(x => x % 2 == 0);
            CollectionAssert.AreEqual(new[] { 1, 3, 5 }, list);
        }

        [TestMethod()]
        public void RemoveItemTest_NoMatch()
        {
            var list = new List<string> { "a", "b", "c" };
            list.RemoveItem(x => x == "z");
            CollectionAssert.AreEqual(new[] { "a", "b", "c" }, list);
        }

        [TestMethod()]
        public void DiceTest_ReturnsElementFromList()
        {
            var list = new List<string> { "a", "b", "c" };
            SRCCore.Lib.GeneralLib.RndSeed = 1;
            SRCCore.Lib.GeneralLib.RndReset();
            for (int i = 0; i < 30; i++)
            {
                var picked = list.Dice();
                Assert.IsTrue(list.Contains(picked), $"Dice returned '{picked}' which is not in the list");
            }
        }

        [TestMethod()]
        public void RemoveItemTest_EmptyList_DoesNotThrow()
        {
            var list = new List<int>();
            list.RemoveItem(x => x > 0);
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod()]
        public void AppendRange_OnEmptyOriginalList()
        {
            IEnumerable<int> empty = new List<int>();
            var result = empty.AppendRange(new List<int> { 1, 2, 3 });
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result.ToList());
        }

        [TestMethod()]
        public void SafeRefOneOffset_EmptyList_ReturnsDefault()
        {
            var list = new List<int>();
            Assert.AreEqual(0, list.SafeRefOneOffset(1));
            Assert.AreEqual(0, list.SafeRefOneOffset(0));
        }

        [TestMethod()]
        public void SafeRefZeroOffset_EmptyList_ReturnsDefault()
        {
            var list = new List<string>();
            Assert.IsNull(list.SafeRefZeroOffset(0));
            Assert.IsNull(list.SafeRefZeroOffset(1));
        }
    }
}