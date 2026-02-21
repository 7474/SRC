using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;
using System;
using System.Collections.Generic;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// SrcCollection の EnumerateAsGenericDictionary テスト（IEnumerable）
    /// </summary>
    [TestClass]
    public class SrcCollectionEnumerationTests
    {
        // ──────────────────────────────────────────────
        // IEnumerable<V> GetEnumerator
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetEnumerator_IteratesAllValues()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 10);
            col.Add("k2", 20);
            col.Add("k3", 30);

            var sum = 0;
            foreach (var v in col)
            {
                sum += v;
            }
            Assert.AreEqual(60, sum);
        }

        [TestMethod]
        public void GetEnumerator_EmptyCollection_DoesNotIterate()
        {
            var col = new SrcCollection<int>();
            int count = 0;
            foreach (var v in col) count++;
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void GetEnumerator_PreservesInsertionOrder()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 100);
            col.Add("k2", 200);
            col.Add("k3", 300);

            var list = new List<int>();
            foreach (var v in col) list.Add(v);

            Assert.AreEqual(100, list[0]);
            Assert.AreEqual(200, list[1]);
            Assert.AreEqual(300, list[2]);
        }

        // ──────────────────────────────────────────────
        // IDictionary<string, V> インターフェース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InterfaceMethod_ContainsKeyValuePair_ReturnsTrue()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 99);
            var kvp = new System.Collections.Generic.KeyValuePair<string, int>("k1", 99);
            Assert.IsTrue(col.Contains(kvp));
        }

        [TestMethod]
        public void InterfaceMethod_ContainsKeyValuePair_WrongValue_ReturnsFalse()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 99);
            var kvp = new System.Collections.Generic.KeyValuePair<string, int>("k1", 0);
            Assert.IsFalse(col.Contains(kvp));
        }

        [TestMethod]
        public void InterfaceMethod_RemoveKeyValuePair_ReturnsTrue()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 42);
            var kvp = new System.Collections.Generic.KeyValuePair<string, int>("k1", 42);
            Assert.IsTrue(col.Remove(kvp));
            Assert.IsFalse(col.ContainsKey("k1"));
        }

        // ──────────────────────────────────────────────
        // 大きなコレクションのテスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LargeCollection_AddAndRetrieve_AllItemsAccessible()
        {
            var col = new SrcCollection<int>();
            for (int i = 0; i < 100; i++)
            {
                col.Add($"key_{i}", i * 10);
            }

            Assert.AreEqual(100, col.Count);
            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(i * 10, col[$"key_{i}"], $"key_{i} の値が不正");
            }
        }

        [TestMethod]
        public void LargeCollection_RemoveAllItems_CountIsZero()
        {
            var col = new SrcCollection<int>();
            for (int i = 0; i < 50; i++)
            {
                col.Add($"key_{i}", i);
            }
            for (int i = 0; i < 50; i++)
            {
                col.Remove($"key_{i}");
            }
            Assert.AreEqual(0, col.Count);
        }

        // ──────────────────────────────────────────────
        // 1オフセットインデクサー
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IntIndexer_1_ReturnsFirstElement()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 11);
            col.Add("k2", 22);
            Assert.AreEqual(11, col[1]);
        }

        [TestMethod]
        public void IntIndexer_LastElement_ReturnsLastElement()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 1);
            col.Add("k2", 2);
            col.Add("k3", 3);
            Assert.AreEqual(3, col[3]);
        }

        [TestMethod]
        public void IntIndexer_OutOfRange_ThrowsIndexOutOfRangeException()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 1);
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var _ = col[5]; });
        }

        [TestMethod]
        public void IntIndexer_Zero_ThrowsIndexOutOfRangeException()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 1);
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var _ = col[0]; });
        }

        [TestMethod]
        public void IntIndexer_Negative_ThrowsIndexOutOfRangeException()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 1);
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var _ = col[-1]; });
        }
    }
}
