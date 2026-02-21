using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// SrcCollection の追加エッジケーステスト
    /// </summary>
    [TestClass]
    public class SrcCollectionAdditionalTests
    {
        // ──────────────────────────────────────────────
        // 全角/半角の同一視
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ContainsKey_FullWidthKey_MatchesHalfWidth()
        {
            // 全角と半角を同一視する
            var sc = new SrcCollection<string>
            {
                ["Ａｂｃ"] = "value",
            };
            Assert.IsTrue(sc.ContainsKey("Abc"));
            Assert.IsTrue(sc.ContainsKey("ａｂｃ"));
        }

        [TestMethod]
        public void ContainsKey_HalfWidthKey_MatchesFullWidth()
        {
            var sc = new SrcCollection<string>
            {
                ["Abc"] = "value",
            };
            Assert.IsTrue(sc.ContainsKey("Ａｂｃ"));
        }

        // ──────────────────────────────────────────────
        // 大文字小文字の同一視
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ContainsKey_UppercaseKey_MatchesLowercase()
        {
            var sc = new SrcCollection<int>
            {
                ["HELLO"] = 42,
            };
            Assert.IsTrue(sc.ContainsKey("hello"));
            Assert.IsTrue(sc.ContainsKey("Hello"));
            Assert.IsTrue(sc.ContainsKey("HELLO"));
        }

        [TestMethod]
        public void IndexerString_CaseInsensitive_ReturnsValue()
        {
            var sc = new SrcCollection<string>
            {
                ["Player"] = "Alice",
            };
            Assert.AreEqual("Alice", sc["player"]);
            Assert.AreEqual("Alice", sc["PLAYER"]);
        }

        // ──────────────────────────────────────────────
        // 順序の保持
        // ──────────────────────────────────────────────

        [TestMethod]
        public void List_MaintainsInsertionOrder()
        {
            var sc = new SrcCollection<int>();
            sc.Add(3, "z");
            sc.Add(1, "a");
            sc.Add(2, "m");

            var list = sc.List;
            Assert.AreEqual(3, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(2, list[2]);
        }

        [TestMethod]
        public void IntIndexer_MaintainsOrder_AfterMultipleAdds()
        {
            var sc = new SrcCollection<int>();
            sc.Add(1, "c");
            sc.Add(2, "a");
            sc.Add(3, "b");

            Assert.AreEqual(1, sc[1]);
            Assert.AreEqual(2, sc[2]);
            Assert.AreEqual(3, sc[3]);
        }

        // ──────────────────────────────────────────────
        // 削除後の再追加
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Remove_ThenAdd_Works()
        {
            var sc = new SrcCollection<int>
            {
                ["key"] = 100,
            };
            sc.Remove("key");
            sc.Add("key", 200);
            Assert.AreEqual(200, sc["key"]);
        }

        [TestMethod]
        public void RemoveAt_MiddleItem_RemainingItemsAdjust()
        {
            var sc = new SrcCollection<string>
            {
                ["a"] = "first",
                ["b"] = "second",
                ["c"] = "third",
            };
            sc.RemoveAt(2); // "second" を削除
            Assert.AreEqual(2, sc.Count);
            Assert.AreEqual("first", sc[1]);
            Assert.AreEqual("third", sc[2]);
        }

        // ──────────────────────────────────────────────
        // TryGetValue
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TryGetValue_ExistingKey_ReturnsTrueAndValue()
        {
            var sc = new SrcCollection<int>
            {
                ["score"] = 1000,
            };
            Assert.IsTrue(sc.TryGetValue("score", out var val));
            Assert.AreEqual(1000, val);
        }

        [TestMethod]
        public void TryGetValue_MissingKey_ReturnsFalseAndDefault()
        {
            var sc = new SrcCollection<int>();
            Assert.IsFalse(sc.TryGetValue("missing", out var val));
            Assert.AreEqual(0, val);
        }

        [TestMethod]
        public void TryGetValue_CaseInsensitive_ReturnsValue()
        {
            var sc = new SrcCollection<string>
            {
                ["Name"] = "Alice",
            };
            Assert.IsTrue(sc.TryGetValue("name", out var val));
            Assert.AreEqual("Alice", val);
        }

        // ──────────────────────────────────────────────
        // Keys / Values / Count
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Count_EmptyCollection_IsZero()
        {
            var sc = new SrcCollection<int>();
            Assert.AreEqual(0, sc.Count);
        }

        [TestMethod]
        public void Count_AfterAdds_MatchesAdded()
        {
            var sc = new SrcCollection<int>();
            sc.Add(1, "a");
            sc.Add(2, "b");
            sc.Add(3, "c");
            Assert.AreEqual(3, sc.Count);
        }

        [TestMethod]
        public void Count_AfterRemove_Decrements()
        {
            var sc = new SrcCollection<int>
            {
                ["x"] = 1,
                ["y"] = 2,
            };
            sc.Remove("x");
            Assert.AreEqual(1, sc.Count);
        }

        [TestMethod]
        public void Keys_EmptyCollection_ReturnsEmpty()
        {
            var sc = new SrcCollection<int>();
            Assert.AreEqual(0, sc.Keys.Count);
        }

        [TestMethod]
        public void Values_EmptyCollection_ReturnsEmpty()
        {
            var sc = new SrcCollection<int>();
            Assert.AreEqual(0, sc.Values.Count);
        }

        // ──────────────────────────────────────────────
        // foreach (IEnumerable<V>)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Foreach_ValuesOnlyIteration_WorksCorrectly()
        {
            var sc = new SrcCollection<int>
            {
                ["one"] = 1,
                ["two"] = 2,
                ["three"] = 3,
            };
            var collected = new List<int>();
            foreach (var v in sc)
            {
                collected.Add(v);
            }
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, collected);
        }

        // ──────────────────────────────────────────────
        // Contains
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Contains_ExistingValue_ReturnsTrue()
        {
            var sc = new SrcCollection<int>
            {
                ["k"] = 99,
            };
            Assert.IsTrue(sc.Contains(99));
        }

        [TestMethod]
        public void Contains_MissingValue_ReturnsFalse()
        {
            var sc = new SrcCollection<int>
            {
                ["k"] = 99,
            };
            Assert.IsFalse(sc.Contains(0));
        }

        // ──────────────────────────────────────────────
        // IndexOf
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IndexOf_ExistingItem_ReturnsZeroBasedIndex()
        {
            var sc = new SrcCollection<string>
            {
                ["a"] = "alpha",
                ["b"] = "beta",
            };
            Assert.AreEqual(0, sc.IndexOf("alpha"));
            Assert.AreEqual(1, sc.IndexOf("beta"));
        }

        [TestMethod]
        public void IndexOf_MissingItem_ReturnsNegativeOne()
        {
            var sc = new SrcCollection<string>
            {
                ["a"] = "alpha",
            };
            Assert.AreEqual(-1, sc.IndexOf("gamma"));
        }

        // ──────────────────────────────────────────────
        // Clear
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Clear_EmptyCollection_DoesNotThrow()
        {
            var sc = new SrcCollection<int>();
            sc.Clear(); // Should not throw
            Assert.AreEqual(0, sc.Count);
        }

        [TestMethod]
        public void Clear_AfterClear_CanAddAgain()
        {
            var sc = new SrcCollection<int>
            {
                ["x"] = 10,
            };
            sc.Clear();
            sc.Add("x", 20);
            Assert.AreEqual(20, sc["x"]);
        }
    }
}
