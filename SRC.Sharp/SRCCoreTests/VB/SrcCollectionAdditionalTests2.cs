using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;
using System;
using System.Collections.Generic;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// SrcCollection クラスの追加ユニットテスト（エッジケース）
    /// </summary>
    [TestClass]
    public class SrcCollectionAdditionalTests2
    {
        // ──────────────────────────────────────────────
        // Add と Count
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Count_AfterMultipleAdds_ReturnsCorrectCount()
        {
            var col = new SrcCollection<string>();
            col["key1"] = "a";
            col["key2"] = "b";
            col["key3"] = "c";
            Assert.AreEqual(3, col.Count);
        }

        [TestMethod]
        public void Add_NullValue_CanBeAdded()
        {
            var col = new SrcCollection<string>();
            col["nullKey"] = null;
            Assert.AreEqual(1, col.Count);
            Assert.IsNull(col["nullKey"]);
        }

        // ──────────────────────────────────────────────
        // キー検索
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Indexer_KeyNotFound_ReturnsNull()
        {
            var col = new SrcCollection<string>();
            col["key1"] = "value";
            Assert.IsNull(col["nonexistent"]);
        }

        [TestMethod]
        public void Indexer_CaseInsensitive_ReturnsSameForDifferentCase()
        {
            // SrcCollection のキー比較は IgnoreCase なので "key" と "KEY" は同じキー
            var col = new SrcCollection<string>();
            col["key"] = "value";
            Assert.AreEqual("value", col["key"]);
            Assert.AreEqual("value", col["KEY"]);
            Assert.AreEqual("value", col["Key"]);
        }

        [TestMethod]
        public void ContainsKey_ExistingKey_ReturnsTrue()
        {
            var col = new SrcCollection<string>();
            col["existingKey"] = "value";
            Assert.IsTrue(col.ContainsKey("existingKey"));
        }

        [TestMethod]
        public void ContainsKey_NonExistingKey_ReturnsFalse()
        {
            var col = new SrcCollection<string>();
            Assert.IsFalse(col.ContainsKey("nothere"));
        }

        // ──────────────────────────────────────────────
        // Remove
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Remove_ExistingKey_DecreasesCount()
        {
            var col = new SrcCollection<string>();
            col["key1"] = "a";
            col["key2"] = "b";
            col.Remove("key1");
            Assert.AreEqual(1, col.Count);
            Assert.IsNull(col["key1"]);
        }

        [TestMethod]
        public void Remove_NonExistingKey_ReturnsFalse()
        {
            var col = new SrcCollection<string>();
            col["key"] = "val";
            var result = col.Remove("nonexistent");
            Assert.IsFalse(result);
            Assert.AreEqual(1, col.Count);
        }

        [TestMethod]
        public void Remove_ExistingKey_ReturnsTrue()
        {
            var col = new SrcCollection<string>();
            col["key"] = "val";
            var result = col.Remove("key");
            Assert.IsTrue(result);
            Assert.AreEqual(0, col.Count);
        }

        // ──────────────────────────────────────────────
        // 1オフセットインデクサ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void OneBasedIndexer_FirstElement_ReturnsFirstAdded()
        {
            var col = new SrcCollection<string>();
            col["k1"] = "first";
            col["k2"] = "second";
            Assert.AreEqual("first", col[1]);
        }

        [TestMethod]
        public void OneBasedIndexer_LastElement_ReturnsLastAdded()
        {
            var col = new SrcCollection<string>();
            col["k1"] = "first";
            col["k2"] = "second";
            col["k3"] = "third";
            Assert.AreEqual("third", col[3]);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void OneBasedIndexer_IndexZero_ThrowsException()
        {
            var col = new SrcCollection<string>();
            col["k1"] = "only";
            _ = col[0]; // 0は範囲外
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void OneBasedIndexer_BeyondCount_ThrowsException()
        {
            var col = new SrcCollection<string>();
            col["k1"] = "only";
            _ = col[2]; // 2は範囲外
        }

        // ──────────────────────────────────────────────
        // Values
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Values_ReturnsAllAddedValues()
        {
            var col = new SrcCollection<string>();
            col["a"] = "alpha";
            col["b"] = "beta";
            col["c"] = "gamma";
            var values = new List<string>(col.Values);
            Assert.IsTrue(values.Contains("alpha"));
            Assert.IsTrue(values.Contains("beta"));
            Assert.IsTrue(values.Contains("gamma"));
        }

        [TestMethod]
        public void Values_EmptyCollection_ReturnsEmpty()
        {
            var col = new SrcCollection<string>();
            Assert.AreEqual(0, new List<string>(col.Values).Count);
        }

        // ──────────────────────────────────────────────
        // Keys
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Keys_ReturnsAllAddedKeys()
        {
            var col = new SrcCollection<string>();
            col["key1"] = "v1";
            col["key2"] = "v2";
            var keys = new List<string>(col.Keys);
            Assert.IsTrue(keys.Contains("key1"));
            Assert.IsTrue(keys.Contains("key2"));
        }

        // ──────────────────────────────────────────────
        // 日本語キー
        // ──────────────────────────────────────────────

        [TestMethod]
        public void JapaneseKey_CanBeAddedAndRetrieved()
        {
            var col = new SrcCollection<string>();
            col["機体名"] = "ロボット";
            Assert.AreEqual("ロボット", col["機体名"]);
        }

        [TestMethod]
        public void JapaneseKey_FullWidthAndHalfWidth_SameKey()
        {
            // IgnoreWidth により全角・半角は同じキーとして扱われる
            var col = new SrcCollection<string>();
            col["Ａ"] = "full-width";
            Assert.AreEqual("full-width", col["A"]); // 半角 A で同じキーを引ける
        }

        // ──────────────────────────────────────────────
        // 型の汎用性テスト (int型)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IntType_CanBeAddedAndRetrieved()
        {
            var col = new SrcCollection<int>();
            col["answer"] = 42;
            Assert.AreEqual(42, col["answer"]);
        }

        [TestMethod]
        public void IntType_DefaultValue_WhenKeyNotFound()
        {
            var col = new SrcCollection<int>();
            Assert.AreEqual(default(int), col["missing"]); // default(int) = 0
        }

        // ──────────────────────────────────────────────
        // TryGetValue
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TryGetValue_ExistingKey_ReturnsTrueAndValue()
        {
            var col = new SrcCollection<string>();
            col["key"] = "value";
            var found = col.TryGetValue("key", out var result);
            Assert.IsTrue(found);
            Assert.AreEqual("value", result);
        }

        [TestMethod]
        public void TryGetValue_NonExistingKey_ReturnsFalse()
        {
            var col = new SrcCollection<string>();
            var found = col.TryGetValue("missing", out var result);
            Assert.IsFalse(found);
            Assert.IsNull(result);
        }

        // ──────────────────────────────────────────────
        // Clear
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Clear_RemovesAllItems()
        {
            var col = new SrcCollection<string>();
            col["k1"] = "v1";
            col["k2"] = "v2";
            col.Clear();
            Assert.AreEqual(0, col.Count);
        }

        [TestMethod]
        public void Clear_OnEmptyCollection_NoException()
        {
            var col = new SrcCollection<string>();
            col.Clear();
            Assert.AreEqual(0, col.Count);
        }

        // ──────────────────────────────────────────────
        // Contains (value-based)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Contains_ExistingValue_ReturnsTrue()
        {
            var col = new SrcCollection<string>();
            col["k"] = "existing";
            Assert.IsTrue(col.Contains("existing"));
        }

        [TestMethod]
        public void Contains_NonExistingValue_ReturnsFalse()
        {
            var col = new SrcCollection<string>();
            col["k"] = "existing";
            Assert.IsFalse(col.Contains("nothere"));
        }
    }
}
