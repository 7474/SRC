using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Extensions.Tests
{
    /// <summary>
    /// ListExtension クラスの追加エッジケーステスト
    /// </summary>
    [TestClass]
    public class ListExtensionEdgeCaseTests
    {
        // ──────────────────────────────────────────────
        // SafeRefOneOffset - 追加エッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SafeRefOneOffset_NegativeIndex_ReturnsDefault()
        {
            var list = new List<int> { 1, 2, 3 };
            Assert.AreEqual(0, list.SafeRefOneOffset(-5));
        }

        [TestMethod]
        public void SafeRefOneOffset_LargeIndex_ReturnsDefault()
        {
            var list = new List<int> { 1, 2, 3 };
            Assert.AreEqual(0, list.SafeRefOneOffset(100));
        }

        [TestMethod]
        public void SafeRefOneOffset_StringList_ValidIndex_ReturnsValue()
        {
            var list = new List<string> { "first", "second", "third" };
            Assert.AreEqual("second", list.SafeRefOneOffset(2));
        }

        [TestMethod]
        public void SafeRefOneOffset_StringList_OutOfRange_ReturnsNull()
        {
            var list = new List<string> { "first", "second" };
            Assert.IsNull(list.SafeRefOneOffset(10));
        }

        // ──────────────────────────────────────────────
        // SafeRefZeroOffset - 追加エッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SafeRefZeroOffset_NegativeIndex_ReturnsDefault()
        {
            var list = new List<int> { 1, 2, 3 };
            Assert.AreEqual(0, list.SafeRefZeroOffset(-1));
        }

        [TestMethod]
        public void SafeRefZeroOffset_LargeIndex_ReturnsDefault()
        {
            var list = new List<int> { 1, 2, 3 };
            Assert.AreEqual(0, list.SafeRefZeroOffset(100));
        }

        [TestMethod]
        public void SafeRefZeroOffset_LastIndex_ReturnsLastValue()
        {
            var list = new List<int> { 10, 20, 30 };
            Assert.AreEqual(30, list.SafeRefZeroOffset(2));
        }

        [TestMethod]
        public void SafeRefZeroOffset_ExactlyBeyondEnd_ReturnsDefault()
        {
            var list = new List<string> { "a", "b", "c" };
            Assert.IsNull(list.SafeRefZeroOffset(3)); // index 3 is out of bounds for 3-element list
        }

        // ──────────────────────────────────────────────
        // CloneList - 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CloneList_EmptyList_ReturnsEmptyList()
        {
            var list = new List<int>();
            var clone = list.CloneList();
            Assert.AreEqual(0, clone.Count);
        }

        [TestMethod]
        public void CloneList_SingleElement_ReturnsCloneWithSameElement()
        {
            var list = new List<string> { "hello" };
            var clone = list.CloneList();
            Assert.AreEqual("hello", clone[0]);
        }

        [TestMethod]
        public void CloneList_IsNotSameReference()
        {
            var list = new List<int> { 1, 2, 3 };
            var clone = list.CloneList();
            Assert.AreNotSame(list, clone);
        }

        [TestMethod]
        public void CloneList_ModifyClone_OriginalUnchanged()
        {
            var list = new List<int> { 1, 2, 3 };
            var clone = list.CloneList();
            clone.Add(4);
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(4, clone.Count);
        }

        // ──────────────────────────────────────────────
        // RemoveItem - 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RemoveItem_RemoveAll_ListBecomesEmpty()
        {
            var list = new List<int> { 2, 4, 6, 8 };
            list.RemoveItem(x => x % 2 == 0);
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void RemoveItem_RemoveNone_ListUnchanged()
        {
            var list = new List<int> { 1, 3, 5 };
            list.RemoveItem(x => x % 2 == 0);
            Assert.AreEqual(3, list.Count);
        }

        [TestMethod]
        public void RemoveItem_StringList_RemovesMatchingElements()
        {
            var list = new List<string> { "apple", "banana", "cherry", "avocado" };
            list.RemoveItem(x => x.StartsWith("a"));
            CollectionAssert.DoesNotContain(list, "apple");
            CollectionAssert.DoesNotContain(list, "avocado");
            CollectionAssert.Contains(list, "banana");
            CollectionAssert.Contains(list, "cherry");
        }

        [TestMethod]
        public void RemoveItem_ReturnsTheSameList()
        {
            var list = new List<int> { 1, 2, 3 };
            var result = list.RemoveItem(x => x == 2);
            Assert.AreSame(list, result);
        }

        // ──────────────────────────────────────────────
        // AppendRange - 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AppendRange_AppendEmptyToEmpty_ReturnsEmpty()
        {
            IEnumerable<int> empty = new List<int>();
            var result = empty.AppendRange(new List<int>());
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void AppendRange_AppendMultipleElements_CorrectOrder()
        {
            IEnumerable<string> list = new List<string> { "a", "b" };
            var appended = list.AppendRange(new List<string> { "c", "d", "e" });
            var arr = appended.ToArray();
            Assert.AreEqual("a", arr[0]);
            Assert.AreEqual("b", arr[1]);
            Assert.AreEqual("c", arr[2]);
            Assert.AreEqual("d", arr[3]);
            Assert.AreEqual("e", arr[4]);
        }

        // ──────────────────────────────────────────────
        // Dice - 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Dice_SingleElementList_AlwaysReturnsThatElement()
        {
            var list = new List<string> { "only" };
            SRCCore.Lib.GeneralLib.RndSeed = 42;
            SRCCore.Lib.GeneralLib.RndReset();
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual("only", list.Dice());
            }
        }

        [TestMethod]
        public void Dice_LargeList_ReturnsElementFromList()
        {
            var list = Enumerable.Range(0, 100).Select(i => i).ToList();
            SRCCore.Lib.GeneralLib.RndSeed = 99;
            SRCCore.Lib.GeneralLib.RndReset();
            for (int i = 0; i < 50; i++)
            {
                var result = list.Dice();
                Assert.IsTrue(result >= 0 && result < 100, $"Dice returned {result} which is out of range [0, 100)");
            }
        }
    }
}
