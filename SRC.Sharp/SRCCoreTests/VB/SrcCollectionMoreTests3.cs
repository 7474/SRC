using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.VB.Tests
{
    [TestClass]
    public class SrcCollectionMoreTests3
    {
        [TestMethod]
        public void NewCollection_IsEmpty()
        {
            var sc = new SrcCollection<string>();
            Assert.AreEqual(0, sc.Count);
        }

        [TestMethod]
        public void Add_KeyValue_IncreasesCount()
        {
            var sc = new SrcCollection<string>();
            sc["key1"] = "value1";
            Assert.AreEqual(1, sc.Count);
        }

        [TestMethod]
        public void Add_MultipleItems_CountIsCorrect()
        {
            var sc = new SrcCollection<int>();
            sc["a"] = 1;
            sc["b"] = 2;
            sc["c"] = 3;
            Assert.AreEqual(3, sc.Count);
        }

        [TestMethod]
        public void Indexer_ByKey_ReturnsCorrectValue()
        {
            var sc = new SrcCollection<string>();
            sc["name"] = "Alice";
            Assert.AreEqual("Alice", sc["name"]);
        }

        [TestMethod]
        public void Indexer_ByKey_CaseInsensitive()
        {
            var sc = new SrcCollection<string>();
            sc["Name"] = "Bob";
            Assert.AreEqual("Bob", sc["name"]);
            Assert.AreEqual("Bob", sc["NAME"]);
        }

        [TestMethod]
        public void Indexer_ByIndex_ReturnsFirstItem()
        {
            var sc = new SrcCollection<int>();
            sc["first"] = 100;
            sc["second"] = 200;
            Assert.AreEqual(100, sc[1]);
        }

        [TestMethod]
        public void Indexer_ByIndex_ReturnsSecondItem()
        {
            var sc = new SrcCollection<int>();
            sc["first"] = 100;
            sc["second"] = 200;
            Assert.AreEqual(200, sc[2]);
        }

        [TestMethod]
        public void Indexer_IndexZero_ThrowsIndexOutOfRangeException()
        {
            var sc = new SrcCollection<int>();
            sc["key"] = 1;
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var _ = sc[0]; });
        }

        [TestMethod]
        public void Indexer_IndexBeyondCount_ThrowsIndexOutOfRangeException()
        {
            var sc = new SrcCollection<int>();
            sc["key"] = 1;
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var _ = sc[2]; });
        }

        [TestMethod]
        public void ContainsKey_ExistingKey_ReturnsTrue()
        {
            var sc = new SrcCollection<string>();
            sc["mykey"] = "val";
            Assert.IsTrue(sc.ContainsKey("mykey"));
        }

        [TestMethod]
        public void ContainsKey_NonExistingKey_ReturnsFalse()
        {
            var sc = new SrcCollection<string>();
            Assert.IsFalse(sc.ContainsKey("missing"));
        }

        [TestMethod]
        public void Remove_ByKey_RemovesItem()
        {
            var sc = new SrcCollection<string>();
            sc["k"] = "v";
            sc.Remove("k");
            Assert.AreEqual(0, sc.Count);
        }

        [TestMethod]
        public void Clear_RemovesAllItems()
        {
            var sc = new SrcCollection<int>();
            sc["a"] = 1;
            sc["b"] = 2;
            sc.Clear();
            Assert.AreEqual(0, sc.Count);
        }

        [TestMethod]
        public void List_ReturnsItemsInInsertionOrder()
        {
            var sc = new SrcCollection<string>();
            sc["x"] = "first";
            sc["y"] = "second";
            sc["z"] = "third";
            var list = sc.List;
            Assert.AreEqual("first", list[0]);
            Assert.AreEqual("second", list[1]);
            Assert.AreEqual("third", list[2]);
        }

        [TestMethod]
        public void Keys_ReturnsAllKeys()
        {
            var sc = new SrcCollection<int>();
            sc["alpha"] = 1;
            sc["beta"] = 2;
            Assert.AreEqual(2, sc.Keys.Count);
            Assert.IsTrue(sc.Keys.Contains("alpha"));
            Assert.IsTrue(sc.Keys.Contains("beta"));
        }

        [TestMethod]
        public void Values_ReturnsAllValues()
        {
            var sc = new SrcCollection<int>();
            sc["a"] = 10;
            sc["b"] = 20;
            var values = sc.Values.ToList();
            CollectionAssert.Contains(values, 10);
            CollectionAssert.Contains(values, 20);
        }

        [TestMethod]
        public void TryGetValue_ExistingKey_ReturnsTrueAndValue()
        {
            var sc = new SrcCollection<string>();
            sc["key"] = "hello";
            Assert.IsTrue(sc.TryGetValue("key", out var val));
            Assert.AreEqual("hello", val);
        }

        [TestMethod]
        public void TryGetValue_MissingKey_ReturnsFalse()
        {
            var sc = new SrcCollection<string>();
            Assert.IsFalse(sc.TryGetValue("nope", out var val));
        }
    }
}
