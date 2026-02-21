using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;
using System.Linq;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// SrcCollection の追加エッジケーステスト
    /// </summary>
    [TestClass]
    public class SrcCollectionFurtherTests
    {
        // ──────────────────────────────────────────────
        // ContainsKey テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ContainsKey_NullKey_ReturnsFalse()
        {
            var col = new SrcCollection<int>();
            col.Add("key1", 1);
            Assert.IsFalse(col.ContainsKey(null));
        }

        [TestMethod]
        public void ContainsKey_AddedKey_ReturnsTrue()
        {
            var col = new SrcCollection<int>();
            col.Add("mykey", 42);
            Assert.IsTrue(col.ContainsKey("mykey"));
        }

        [TestMethod]
        public void ContainsKey_NotAddedKey_ReturnsFalse()
        {
            var col = new SrcCollection<int>();
            col.Add("key1", 100);
            Assert.IsFalse(col.ContainsKey("otherkey"));
        }

        // ──────────────────────────────────────────────
        // Contains (by value)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Contains_Value_ReturnsTrue_WhenAdded()
        {
            var col = new SrcCollection<int>();
            col.Add("k", 42);
            Assert.IsTrue(col.Contains(42));
        }

        [TestMethod]
        public void Contains_Value_ReturnsFalse_WhenNotAdded()
        {
            var col = new SrcCollection<int>();
            col.Add("k", 42);
            Assert.IsFalse(col.Contains(99));
        }

        // ──────────────────────────────────────────────
        // Remove メソッド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Remove_ByKey_ExistingKey_ReturnsTrue()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 100);
            Assert.IsTrue(col.Remove("k1"));
        }

        [TestMethod]
        public void Remove_ByKey_ExistingKey_DecreasesCount()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 100);
            col.Add("k2", 200);
            col.Remove("k1");
            Assert.AreEqual(1, col.Count);
            Assert.IsFalse(col.ContainsKey("k1"));
        }

        [TestMethod]
        public void Remove_ByKey_NonExistingKey_ReturnsFalse()
        {
            var col = new SrcCollection<int>();
            Assert.IsFalse(col.Remove("nonexistent"));
        }

        [TestMethod]
        public void Remove_ByValue_ExistingValue_ReturnsTrue()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 777);
            Assert.IsTrue(col.Remove(777));
            Assert.IsFalse(col.ContainsKey("k1"));
        }

        [TestMethod]
        public void Remove_ByValue_NonExistingValue_ReturnsFalse()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 1);
            Assert.IsFalse(col.Remove(999));
        }

        // ──────────────────────────────────────────────
        // RemoveAt
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RemoveAt_Index1_RemovesFirst()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 10);
            col.Add("k2", 20);
            col.RemoveAt(1);
            Assert.AreEqual(1, col.Count);
            Assert.IsFalse(col.ContainsKey("k1"));
            Assert.IsTrue(col.ContainsKey("k2"));
        }

        [TestMethod]
        public void RemoveAt_LastIndex_RemovesLast()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 10);
            col.Add("k2", 20);
            col.Add("k3", 30);
            col.RemoveAt(3);
            Assert.AreEqual(2, col.Count);
            Assert.IsTrue(col.ContainsKey("k1"));
            Assert.IsTrue(col.ContainsKey("k2"));
            Assert.IsFalse(col.ContainsKey("k3"));
        }

        // ──────────────────────────────────────────────
        // 大文字/小文字の無視
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ContainsKey_CaseInsensitive_ReturnsTrue()
        {
            var col = new SrcCollection<int>();
            col.Add("HelloWorld", 1);
            Assert.IsTrue(col.ContainsKey("helloworld"));
            Assert.IsTrue(col.ContainsKey("HELLOWORLD"));
        }

        [TestMethod]
        public void Indexer_IntKey_CaseInsensitive_ReturnsValue()
        {
            var col = new SrcCollection<int>();
            col.Add("TestKey", 42);
            Assert.AreEqual(42, col["testkey"]);
            Assert.AreEqual(42, col["TESTKEY"]);
        }

        // ──────────────────────────────────────────────
        // IndexOf
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IndexOf_FindsAddedValue_ReturnsZeroBasedIndex()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 100);
            col.Add("k2", 200);
            col.Add("k3", 300);
            Assert.AreEqual(1, col.IndexOf(200)); // 0-based
        }

        [TestMethod]
        public void IndexOf_NotFound_ReturnsMinusOne()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 100);
            Assert.AreEqual(-1, col.IndexOf(999));
        }

        // ──────────────────────────────────────────────
        // Keys / Values
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Keys_ContainsAddedKeys()
        {
            var col = new SrcCollection<int>();
            col.Add("keyA", 1);
            col.Add("keyB", 2);
            var keys = col.Keys.ToList();
            CollectionAssert.Contains(keys, "keyA");
            CollectionAssert.Contains(keys, "keyB");
        }

        [TestMethod]
        public void Values_ContainsAddedValues()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 1);
            col.Add("k2", 2);
            var values = col.Values.ToList();
            CollectionAssert.Contains(values, 1);
            CollectionAssert.Contains(values, 2);
        }

        // ──────────────────────────────────────────────
        // TryGetValue
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TryGetValue_ExistingKey_ReturnsTrueAndValue()
        {
            var col = new SrcCollection<int>();
            col.Add("x", 42);
            Assert.IsTrue(col.TryGetValue("x", out int val));
            Assert.AreEqual(42, val);
        }

        [TestMethod]
        public void TryGetValue_MissingKey_ReturnsFalse()
        {
            var col = new SrcCollection<int>();
            Assert.IsFalse(col.TryGetValue("missing", out int val));
        }

        // ──────────────────────────────────────────────
        // List プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void List_ReturnsAllValues_InInsertionOrder()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 10);
            col.Add("k2", 20);
            col.Add("k3", 30);
            var list = col.List;
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(10, list[0]);
            Assert.AreEqual(20, list[1]);
            Assert.AreEqual(30, list[2]);
        }

        // ──────────────────────────────────────────────
        // IsReadOnly
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsReadOnly_IsFalse()
        {
            var col = new SrcCollection<int>();
            Assert.IsFalse(col.IsReadOnly);
        }

        // ──────────────────────────────────────────────
        // Clear
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Clear_RemovesAllElements()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 1);
            col.Add("k2", 2);
            col.Clear();
            Assert.AreEqual(0, col.Count);
        }
    }
}
