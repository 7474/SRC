using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// SrcArray の追加テスト（既存テストとの重複を避けた新規ケース）
    /// </summary>
    [TestClass]
    public class SrcArrayMoreTests
    {
        // ──────────────────────────────────────────────
        // Remove（List 継承）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Remove_ExistingItem_ReturnsTrueAndDecreasesCount()
        {
            var arr = new SrcArray<int>();
            arr.Add(10);
            arr.Add(20);
            arr.Add(30);

            bool removed = arr.Remove(20);
            Assert.IsTrue(removed);
            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual(10, arr[1]);
            Assert.AreEqual(30, arr[2]);
        }

        [TestMethod]
        public void Remove_NonExistingItem_ReturnsFalse()
        {
            var arr = new SrcArray<int>();
            arr.Add(10);
            Assert.IsFalse(arr.Remove(999));
            Assert.AreEqual(1, arr.Count);
        }

        // ──────────────────────────────────────────────
        // Clear（List 継承）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Clear_RemovesAllItems()
        {
            var arr = new SrcArray<string>();
            arr.Add("a");
            arr.Add("b");
            arr.Add("c");

            arr.Clear();
            Assert.AreEqual(0, arr.Count);
        }

        [TestMethod]
        public void Clear_EmptyArray_DoesNotThrow()
        {
            var arr = new SrcArray<int>();
            arr.Clear();
            Assert.AreEqual(0, arr.Count);
        }

        // ──────────────────────────────────────────────
        // IndexOf（List 継承）※ 0ベース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IndexOf_ExistingItem_ReturnsZeroBasedIndex()
        {
            var arr = new SrcArray<string>();
            arr.Add("alpha");
            arr.Add("beta");
            arr.Add("gamma");

            // List.IndexOf は 0ベース
            Assert.AreEqual(0, arr.IndexOf("alpha"));
            Assert.AreEqual(1, arr.IndexOf("beta"));
            Assert.AreEqual(2, arr.IndexOf("gamma"));
        }

        [TestMethod]
        public void IndexOf_NonExistingItem_ReturnsMinusOne()
        {
            var arr = new SrcArray<string>();
            arr.Add("hello");
            Assert.AreEqual(-1, arr.IndexOf("world"));
        }

        // ──────────────────────────────────────────────
        // foreach / IEnumerable（List 継承）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Foreach_IteratesAllItems()
        {
            var arr = new SrcArray<int>();
            arr.Add(1);
            arr.Add(2);
            arr.Add(3);

            var collected = new List<int>();
            foreach (var item in arr)
            {
                collected.Add(item);
            }
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, collected);
        }

        // ──────────────────────────────────────────────
        // Sort（List 継承）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Sort_SortsItems_ThenOneBasedAccessWorks()
        {
            var arr = new SrcArray<int>();
            arr.Add(30);
            arr.Add(10);
            arr.Add(20);

            arr.Sort();

            Assert.AreEqual(10, arr[1]);
            Assert.AreEqual(20, arr[2]);
            Assert.AreEqual(30, arr[3]);
        }

        // ──────────────────────────────────────────────
        // Insert（List 継承・0ベース）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Insert_AtZeroIndex_ShiftsExistingItems()
        {
            var arr = new SrcArray<string>();
            arr.Add("b");
            arr.Add("c");

            // List.Insert は 0ベース
            arr.Insert(0, "a");

            Assert.AreEqual(3, arr.Count);
            Assert.AreEqual("a", arr[1]); // 1ベースアクセス
            Assert.AreEqual("b", arr[2]);
            Assert.AreEqual("c", arr[3]);
        }

        // ──────────────────────────────────────────────
        // RemoveAt（List 継承・0ベース）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RemoveAt_ZeroBasedIndex_RemovesCorrectItem()
        {
            var arr = new SrcArray<int>();
            arr.Add(10);
            arr.Add(20);
            arr.Add(30);

            // List.RemoveAt は 0ベース
            arr.RemoveAt(1); // 20 を削除

            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual(10, arr[1]);
            Assert.AreEqual(30, arr[2]);
        }

        // ──────────────────────────────────────────────
        // double 型の SrcArray
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SrcArrayDouble_OneBasedAccess()
        {
            var arr = new SrcArray<double>();
            arr.Add(1.1);
            arr.Add(2.2);
            arr.Add(3.3);

            Assert.AreEqual(1.1, arr[1], 1e-9);
            Assert.AreEqual(2.2, arr[2], 1e-9);
            Assert.AreEqual(3.3, arr[3], 1e-9);
        }

        // ──────────────────────────────────────────────
        // SrcArray<object>
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SrcArrayObject_MixedValues_OneBasedAccess()
        {
            var arr = new SrcArray<object>();
            arr.Add(42);
            arr.Add("hello");
            arr.Add(3.14);

            Assert.AreEqual(42, arr[1]);
            Assert.AreEqual("hello", arr[2]);
            Assert.AreEqual(3.14, arr[3]);
        }

        // ──────────────────────────────────────────────
        // 空の配列に対するアクセスは例外
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EmptyArray_IndexAccess_ThrowsArgumentOutOfRange()
        {
            var arr = new SrcArray<int>();
            Assert.Throws<ArgumentOutOfRangeException>(() => { var _ = arr[1]; });
        }

        // ──────────────────────────────────────────────
        // LINQ との互換性（List 継承）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LinqSelect_WorksOnSrcArray()
        {
            var arr = new SrcArray<int>();
            arr.Add(1);
            arr.Add(2);
            arr.Add(3);

            var doubled = arr.Select(x => x * 2).ToList();
            CollectionAssert.AreEqual(new[] { 2, 4, 6 }, doubled);
        }

        [TestMethod]
        public void LinqWhere_WorksOnSrcArray()
        {
            var arr = new SrcArray<int>();
            arr.Add(1);
            arr.Add(2);
            arr.Add(3);
            arr.Add(4);

            var even = arr.Where(x => x % 2 == 0).ToList();
            CollectionAssert.AreEqual(new[] { 2, 4 }, even);
        }
    }
}
