using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;
using System;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// SrcCollection クラスの追加ユニットテスト
    /// </summary>
    [TestClass]
    public class SrcCollectionMoreTests
    {
        // ──────────────────────────────────────────────
        // 基本操作
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Add_And_GetByKey_ReturnsValue()
        {
            var col = new SrcCollection<string>();
            col["key1"] = "value1";
            Assert.AreEqual("value1", col["key1"]);
        }

        [TestMethod]
        public void Add_MultipleItems_CountIncreases()
        {
            var col = new SrcCollection<int>();
            col.Add("a", 1);
            col.Add("b", 2);
            col.Add("c", 3);
            Assert.AreEqual(3, col.Count);
        }

        [TestMethod]
        public void Clear_RemovesAllItems()
        {
            var col = new SrcCollection<string>();
            col["k1"] = "v1";
            col["k2"] = "v2";
            col.Clear();
            Assert.AreEqual(0, col.Count);
        }

        // ──────────────────────────────────────────────
        // ContainsKey
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ContainsKey_ExistingKey_ReturnsTrue()
        {
            var col = new SrcCollection<string>();
            col["myKey"] = "myValue";
            Assert.IsTrue(col.ContainsKey("myKey"));
        }

        [TestMethod]
        public void ContainsKey_NonExistingKey_ReturnsFalse()
        {
            var col = new SrcCollection<string>();
            col["myKey"] = "myValue";
            Assert.IsFalse(col.ContainsKey("other"));
        }

        [TestMethod]
        public void ContainsKey_NullKey_ReturnsFalse()
        {
            var col = new SrcCollection<string>();
            Assert.IsFalse(col.ContainsKey(null));
        }

        // ──────────────────────────────────────────────
        // ケース非感受性（大文字小文字）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetByKey_CaseInsensitive_ReturnsValue()
        {
            var col = new SrcCollection<string>();
            col["MyKey"] = "value";
            Assert.AreEqual("value", col["mykey"]);
            Assert.AreEqual("value", col["MYKEY"]);
        }

        // ──────────────────────────────────────────────
        // 1ベースインデックスアクセス
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IndexAccessByOne_FirstItem_ReturnsFirst()
        {
            var col = new SrcCollection<string>();
            col["k1"] = "first";
            col["k2"] = "second";
            Assert.AreEqual("first", col[1]);
        }

        [TestMethod]
        public void IndexAccessByTwo_SecondItem_ReturnsSecond()
        {
            var col = new SrcCollection<string>();
            col["k1"] = "first";
            col["k2"] = "second";
            Assert.AreEqual("second", col[2]);
        }

        [TestMethod]
        public void IndexAccessOutOfRange_ThrowsException()
        {
            var col = new SrcCollection<string>();
            col["k1"] = "first";
            Assert.Throws<IndexOutOfRangeException>(() => { var _ = col[0]; });
            Assert.Throws<IndexOutOfRangeException>(() => { var _ = col[2]; });
        }

        // ──────────────────────────────────────────────
        // Remove
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Remove_ExistingKey_ReturnsTrueAndDecreaseCount()
        {
            var col = new SrcCollection<string>();
            col["k1"] = "v1";
            col["k2"] = "v2";
            Assert.IsTrue(col.Remove("k1"));
            Assert.AreEqual(1, col.Count);
            Assert.IsFalse(col.ContainsKey("k1"));
        }

        [TestMethod]
        public void Remove_NonExistingKey_ReturnsFalse()
        {
            var col = new SrcCollection<string>();
            col["k1"] = "v1";
            Assert.IsFalse(col.Remove("nonexistent"));
        }

        // ──────────────────────────────────────────────
        // TryGetValue
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TryGetValue_ExistingKey_ReturnsTrueAndValue()
        {
            var col = new SrcCollection<string>();
            col["k1"] = "v1";
            Assert.IsTrue(col.TryGetValue("k1", out var value));
            Assert.AreEqual("v1", value);
        }

        [TestMethod]
        public void TryGetValue_NonExistingKey_ReturnsFalseAndDefault()
        {
            var col = new SrcCollection<string>();
            Assert.IsFalse(col.TryGetValue("missing", out var value));
            Assert.IsNull(value);
        }

        // ──────────────────────────────────────────────
        // Keys / Values / List
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Keys_ContainsAllAddedKeys()
        {
            var col = new SrcCollection<string>();
            col["a"] = "1";
            col["b"] = "2";
            Assert.IsTrue(col.Keys.Contains("a"));
            Assert.IsTrue(col.Keys.Contains("b"));
        }

        [TestMethod]
        public void Values_ContainsAllAddedValues()
        {
            var col = new SrcCollection<string>();
            col["a"] = "val1";
            col["b"] = "val2";
            Assert.IsTrue(col.Values.Contains("val1"));
            Assert.IsTrue(col.Values.Contains("val2"));
        }

        [TestMethod]
        public void List_ReturnsAllValues()
        {
            var col = new SrcCollection<int>();
            col.Add("x", 10);
            col.Add("y", 20);
            var list = col.List;
            Assert.AreEqual(2, list.Count);
            Assert.IsTrue(list.Contains(10));
            Assert.IsTrue(list.Contains(20));
        }

        // ──────────────────────────────────────────────
        // GetByKey - 未登録は default
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetByKey_NonExistingKey_ReturnsDefault()
        {
            var col = new SrcCollection<string>();
            var result = col["missing"];
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetByKeyInt_NonExistingKey_ReturnsZero()
        {
            var col = new SrcCollection<int>();
            var result = col["missing"];
            Assert.AreEqual(0, result);
        }

        // ──────────────────────────────────────────────
        // Contains(item)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Contains_ExistingItem_ReturnsTrue()
        {
            var col = new SrcCollection<string>();
            col["k"] = "found";
            Assert.IsTrue(col.Contains("found"));
        }

        [TestMethod]
        public void Contains_NonExistingItem_ReturnsFalse()
        {
            var col = new SrcCollection<string>();
            col["k"] = "found";
            Assert.IsFalse(col.Contains("notfound"));
        }

        // ──────────────────────────────────────────────
        // RemoveAt
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RemoveAt_ValidIndex_RemovesItem()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 1);
            col.Add("k2", 2);
            col.RemoveAt(1);
            Assert.AreEqual(1, col.Count);
        }

        // ──────────────────────────────────────────────
        // IndexOf
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IndexOf_ExistingItem_ReturnsZeroBasedIndex()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 10);
            col.Add("k2", 20);
            Assert.AreEqual(0, col.IndexOf(10));
            Assert.AreEqual(1, col.IndexOf(20));
        }

        [TestMethod]
        public void IndexOf_NonExistingItem_ReturnsMinusOne()
        {
            var col = new SrcCollection<int>();
            col.Add("k1", 10);
            Assert.AreEqual(-1, col.IndexOf(99));
        }
    }
}
