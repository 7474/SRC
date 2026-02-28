using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// SrcCollection の追加テスト（既存テストとの重複を避けた新規ケース）
    /// </summary>
    [TestClass]
    public class SrcCollectionExtraTests
    {
        // ──────────────────────────────────────────────
        // int インデクサーの set は NotSupportedException
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IntIndexer_Set_ThrowsNotSupportedException()
        {
            var sc = new SrcCollection<string>();
            sc["k"] = "v";
            Assert.Throws<NotSupportedException>(() => { sc[1] = "new"; });
        }

        // ──────────────────────────────────────────────
        // string インデクサーで同じキーを二度セットすると例外
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StringIndexer_DuplicateKey_ThrowsException()
        {
            var sc = new SrcCollection<string>();
            sc["key"] = "first";
            // OrderedDictionary は重複キーで ArgumentException
            Assert.Throws<ArgumentException>(() => { sc["key"] = "second"; });
        }

        // ──────────────────────────────────────────────
        // Add(string key, V value) で重複キーは例外
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Add_DuplicateKey_ThrowsException()
        {
            var sc = new SrcCollection<int>();
            sc.Add("k", 1);
            Assert.Throws<ArgumentException>(() => sc.Add("k", 2));
        }

        // ──────────────────────────────────────────────
        // IEnumerable (非ジェネリック) GetEnumerator
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NonGenericGetEnumerator_IteratesValues()
        {
            var sc = new SrcCollection<int>
            {
                ["x"] = 10,
                ["y"] = 20,
            };
            var results = new List<object>();
            var enumerator = ((System.Collections.IEnumerable)sc).GetEnumerator();
            while (enumerator.MoveNext())
            {
                results.Add(enumerator.Current);
            }
            Assert.AreEqual(2, results.Count);
            CollectionAssert.Contains(results, 10);
            CollectionAssert.Contains(results, 20);
        }

        // ──────────────────────────────────────────────
        // Remove(V) で存在しない値は false
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Remove_Value_NotPresent_ReturnsFalse()
        {
            var sc = new SrcCollection<int>
            {
                ["a"] = 1,
            };
            Assert.IsFalse(sc.Remove(999));
        }

        // ──────────────────────────────────────────────
        // Contains(KeyValuePair) – 値が違う場合は false
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ContainsKVP_KeyMatchButValueMismatch_ReturnsFalse()
        {
            var sc = new SrcCollection<int>
            {
                ["key"] = 42,
            };
            Assert.IsFalse(sc.Contains(new KeyValuePair<string, int>("key", 99)));
        }

        [TestMethod]
        public void ContainsKVP_ExactMatch_ReturnsTrue()
        {
            var sc = new SrcCollection<int>
            {
                ["key"] = 42,
            };
            Assert.IsTrue(sc.Contains(new KeyValuePair<string, int>("key", 42)));
        }

        // ──────────────────────────────────────────────
        // Remove(KeyValuePair) で存在しないキーは false
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Remove_KVP_NonExistentKey_ReturnsFalse()
        {
            var sc = new SrcCollection<string>
            {
                ["key"] = "val",
            };
            Assert.IsFalse(sc.Remove(new KeyValuePair<string, string>("missing", "val")));
        }

        // ──────────────────────────────────────────────
        // IsReadOnly
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsReadOnly_NewCollection_ReturnsFalse()
        {
            var sc = new SrcCollection<object>();
            Assert.IsFalse(sc.IsReadOnly);
        }

        // ──────────────────────────────────────────────
        // Keys / Values 順序の保持
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Keys_MaintainsInsertionOrder()
        {
            var sc = new SrcCollection<int>();
            sc.Add("z", 3);
            sc.Add("a", 1);
            sc.Add("m", 2);
            var keyList = sc.Keys.ToList();
            Assert.AreEqual("z", keyList[0]);
            Assert.AreEqual("a", keyList[1]);
            Assert.AreEqual("m", keyList[2]);
        }

        [TestMethod]
        public void Values_MaintainsInsertionOrder()
        {
            var sc = new SrcCollection<string>
            {
                ["first"] = "one",
                ["second"] = "two",
                ["third"] = "three",
            };
            var valList = sc.Values.ToList();
            Assert.AreEqual("one", valList[0]);
            Assert.AreEqual("two", valList[1]);
            Assert.AreEqual("three", valList[2]);
        }

        // ──────────────────────────────────────────────
        // RemoveAt – 範囲外インデックスは IndexOutOfRangeException
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RemoveAt_ZeroIndex_ThrowsIndexOutOfRange()
        {
            var sc = new SrcCollection<string>
            {
                ["k"] = "v",
            };
            // 1ベースなので 0 は範囲外
            Assert.Throws<IndexOutOfRangeException>(() => sc.RemoveAt(0));
        }

        [TestMethod]
        public void RemoveAt_BeyondCount_ThrowsIndexOutOfRange()
        {
            var sc = new SrcCollection<string>
            {
                ["k"] = "v",
            };
            Assert.Throws<IndexOutOfRangeException>(() => sc.RemoveAt(2));
        }

        // ──────────────────────────────────────────────
        // ContainsKey – 全角/半角 混在キー
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ContainsKey_FullWidthDigitKey_MatchesHalfWidth()
        {
            var sc = new SrcCollection<string>
            {
                ["１２３"] = "value",
            };
            Assert.IsTrue(sc.ContainsKey("123"));
        }

        // ──────────────────────────────────────────────
        // Count – Clear 後に再 Add
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Clear_ThenReAdd_CountReflectsNew()
        {
            var sc = new SrcCollection<int>
            {
                ["a"] = 1,
                ["b"] = 2,
                ["c"] = 3,
            };
            sc.Clear();
            sc.Add("x", 10);
            Assert.AreEqual(1, sc.Count);
            Assert.AreEqual(10, sc["x"]);
        }
    }
}
