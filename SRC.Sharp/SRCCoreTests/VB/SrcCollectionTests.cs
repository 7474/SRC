using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRCCore.VB.Tests
{
    [TestClass()]
    public class SrcCollectionTests
    {
        [TestMethod()]
        public void SrcCollectionTest()
        {
            var sc = new SrcCollection<object>();
            Assert.IsNotNull(sc);
        }

        [TestMethod()]
        public void IndexerListTest()
        {
            var sc = new SrcCollection<int>
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };

            Assert.AreEqual(1, sc[1]);
            Assert.AreEqual(2, sc[2]);
            Assert.AreEqual(3, sc[3]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => sc[0]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => sc[4]);
        }

        [TestMethod()]
        public void IndexerDictTest()
        {
            var sc = new SrcCollection<string>
            {
                ["1"] = "1",
                ["Two"] = "2",
                ["Ｔｒｅｅ"] = "3",
            };

            Assert.AreEqual("1", sc["1"]);
            Assert.AreEqual("2", sc["two"]);
            Assert.AreEqual("3", sc["tree"]);
            Assert.AreEqual(null, sc[""]);
            Assert.AreEqual(null, sc["2"]);
        }

        [TestMethod()]
        public void AddTest()
        {
            // Add(V item) は NotSupportedException をスローする
            var sc = new SrcCollection<string>();
            Assert.ThrowsException<NotSupportedException>(() => sc.Add("value"));
        }

        [TestMethod()]
        public void AddTest1()
        {
            // Add(string key, string value) [Obsolete] は NotSupportedException をスローする
            var sc = new SrcCollection<string>();
#pragma warning disable CS0612
            Assert.ThrowsException<NotSupportedException>(() => sc.Add("key", "value"));
#pragma warning restore CS0612
        }

        [TestMethod()]
        public void AddTest2()
        {
            // Add(V value, string key) は正常に追加できる（V=int の場合）
            var sc = new SrcCollection<int>();
            sc.Add(42, "key");
            Assert.AreEqual(42, sc["key"]);
        }

        [TestMethod()]
        public void AddTest3()
        {
            // Add(string key, V value) は正常に追加できる
            var sc = new SrcCollection<int>();
            sc.Add("key", 42);
            Assert.AreEqual(42, sc["key"]);
        }

        [TestMethod()]
        public void AddTest4()
        {
            // Add(KeyValuePair<string, V>) は正常に追加できる
            var sc = new SrcCollection<string>();
            sc.Add(new System.Collections.Generic.KeyValuePair<string, string>("key", "value"));
            Assert.AreEqual("value", sc["key"]);
        }

        [TestMethod()]
        public void ClearTest()
        {
            var sc = new SrcCollection<string>
            {
                ["a"] = "1",
                ["b"] = "2",
            };
            Assert.AreEqual(2, sc.Count);
            sc.Clear();
            Assert.AreEqual(0, sc.Count);
        }

        [TestMethod()]
        public void ContainsTest()
        {
            // Contains(V item) は値の存在確認
            var sc = new SrcCollection<string>
            {
                ["key"] = "hello",
            };
            Assert.IsTrue(sc.Contains("hello"));
            Assert.IsFalse(sc.Contains("world"));
        }

        [TestMethod()]
        public void ContainsTest1()
        {
            // Contains(KeyValuePair<string, V>) はキーと値の両方を確認
            var sc = new SrcCollection<string>
            {
                ["key"] = "hello",
            };
            Assert.IsTrue(sc.Contains(new System.Collections.Generic.KeyValuePair<string, string>("key", "hello")));
            Assert.IsFalse(sc.Contains(new System.Collections.Generic.KeyValuePair<string, string>("key", "world")));
            Assert.IsFalse(sc.Contains(new System.Collections.Generic.KeyValuePair<string, string>("other", "hello")));
        }

        [TestMethod()]
        public void ContainsKeyTest()
        {
            var sc = new SrcCollection<string>
            {
                ["MyKey"] = "value",
            };
            Assert.IsTrue(sc.ContainsKey("MyKey"));
            Assert.IsTrue(sc.ContainsKey("mykey")); // 大文字小文字を区別しない
            Assert.IsFalse(sc.ContainsKey("other"));
            Assert.IsFalse(sc.ContainsKey(null));
        }

        [TestMethod()]
        public void CopyToTest()
        {
            // CopyTo(V[], int) は NotSupportedException をスローする
            var sc = new SrcCollection<string> { ["key"] = "val" };
            Assert.ThrowsException<NotSupportedException>(() => sc.CopyTo(new string[1], 0));
        }

        [TestMethod()]
        public void CopyToTest1()
        {
            // CopyTo(KeyValuePair[], int) は NotSupportedException をスローする
            var sc = new SrcCollection<string> { ["key"] = "val" };
            Assert.ThrowsException<NotSupportedException>(() =>
                sc.CopyTo(new System.Collections.Generic.KeyValuePair<string, string>[1], 0));
        }

        [TestMethod()]
        public void GetEnumeratorTest()
        {
            var sc = new SrcCollection<int>
            {
                ["a"] = 1,
                ["b"] = 2,
                ["c"] = 3,
            };
            var list = new System.Collections.Generic.List<int>();
            foreach (var item in sc)
            {
                list.Add(item);
            }
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, list);
        }

        [TestMethod()]
        public void IndexOfTest()
        {
            var sc = new SrcCollection<string>
            {
                ["first"] = "a",
                ["second"] = "b",
                ["third"] = "c",
            };
            Assert.AreEqual(0, sc.IndexOf("a")); // 0ベース
            Assert.AreEqual(1, sc.IndexOf("b"));
            Assert.AreEqual(2, sc.IndexOf("c"));
            Assert.AreEqual(-1, sc.IndexOf("z"));
        }

        [TestMethod()]
        public void InsertTest()
        {
            // Insert は NotSupportedException をスローする
            var sc = new SrcCollection<string>();
            Assert.ThrowsException<NotSupportedException>(() => sc.Insert(1, "value"));
        }

        [TestMethod()]
        public void RemoveTest()
        {
            // Remove(V item) は値で削除する（V=int を使うことで Remove(string key) との曖昧さを回避）
            var sc = new SrcCollection<int>
            {
                ["key"] = 42,
            };
            Assert.IsTrue(sc.Remove(42));
            Assert.AreEqual(0, sc.Count);
            Assert.IsFalse(sc.Remove(99));
        }

        [TestMethod()]
        public void RemoveTest1()
        {
            // Remove(string key) はキーで削除する
            var sc = new SrcCollection<string>
            {
                ["key"] = "hello",
            };
            Assert.IsTrue(sc.Remove("key"));
            Assert.AreEqual(0, sc.Count);
            Assert.IsFalse(sc.Remove("nonexistent"));
        }

        [TestMethod()]
        public void RemoveTest2()
        {
            // Remove(KeyValuePair<string, V>) はキーで削除する
            var sc = new SrcCollection<string>
            {
                ["key"] = "hello",
            };
            Assert.IsTrue(sc.Remove(new System.Collections.Generic.KeyValuePair<string, string>("key", "hello")));
            Assert.AreEqual(0, sc.Count);
        }

        [TestMethod()]
        public void RemoveAtTest()
        {
            // RemoveAt(int) は1ベースのインデックスで削除する
            var sc = new SrcCollection<string>
            {
                ["first"] = "a",
                ["second"] = "b",
                ["third"] = "c",
            };
            sc.RemoveAt(2); // 1ベースなので2番目="b"を削除
            Assert.AreEqual(2, sc.Count);
            Assert.AreEqual("a", sc[1]);
            Assert.AreEqual("c", sc[2]);
        }

        [TestMethod()]
        public void TryGetValueTest()
        {
            var sc = new SrcCollection<string>
            {
                ["key"] = "hello",
            };
            Assert.IsTrue(sc.TryGetValue("key", out var value));
            Assert.AreEqual("hello", value);
            Assert.IsFalse(sc.TryGetValue("nonexistent", out var missing));
            Assert.IsNull(missing);
        }
    }
}