using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// SrcCollection クラスのさらなる追加ユニットテスト
    /// </summary>
    [TestClass]
    public class SrcCollectionFurtherTests2
    {
        // ──────────────────────────────────────────────
        // Add / Count
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Add_SingleItem_CountIsOne()
        {
            var col = new SrcCollection<string> { ["key1"] = "value1" };
            Assert.AreEqual(1, col.Count);
        }

        [TestMethod]
        public void Add_TwoItems_CountIsTwo()
        {
            var col = new SrcCollection<string> { ["key1"] = "a", ["key2"] = "b" };
            Assert.AreEqual(2, col.Count);
        }

        [TestMethod]
        public void Add_TenItems_CountIsTen()
        {
            var col = new SrcCollection<int>();
            for (int i = 0; i < 10; i++)
            {
                col[$"key{i}"] = i;
            }
            Assert.AreEqual(10, col.Count);
        }

        // ──────────────────────────────────────────────
        // Clear
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Clear_AfterAdding_CountIsZero()
        {
            var col = new SrcCollection<string> { ["k1"] = "a", ["k2"] = "b" };
            col.Clear();
            Assert.AreEqual(0, col.Count);
        }

        [TestMethod]
        public void Clear_EmptyCollection_CountStillZero()
        {
            var col = new SrcCollection<string>();
            col.Clear();
            Assert.AreEqual(0, col.Count);
        }

        // ──────────────────────────────────────────────
        // List プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void List_Empty_ReturnsEmptyList()
        {
            var col = new SrcCollection<string>();
            Assert.IsNotNull(col.List);
            Assert.AreEqual(0, col.List.Count);
        }

        [TestMethod]
        public void List_AfterAdd_ContainsAddedItem()
        {
            var col = new SrcCollection<string> { ["mykey"] = "test" };
            Assert.AreEqual(1, col.List.Count);
            Assert.AreEqual("test", col.List[0]);
        }

        [TestMethod]
        public void List_MultipleItems_ContainsAllItems()
        {
            var col = new SrcCollection<int> { ["k1"] = 1, ["k2"] = 2, ["k3"] = 3 };
            var list = col.List;
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
        }

        // ──────────────────────────────────────────────
        // Named Add (with key)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetByKey_CanBeAccessedByKey()
        {
            var col = new SrcCollection<string> { ["key1"] = "value1" };
            Assert.AreEqual("value1", col["key1"]);
        }

        [TestMethod]
        public void SetByKey_CaseInsensitive()
        {
            var col = new SrcCollection<string> { ["TestKey"] = "value1" };
            Assert.AreEqual("value1", col["testkey"]);
        }

        // ──────────────────────────────────────────────
        // ContainsKey
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ContainsKey_ExistingKey_ReturnsTrue()
        {
            var col = new SrcCollection<string> { ["existingKey"] = "value" };
            Assert.IsTrue(col.ContainsKey("existingKey"));
        }

        [TestMethod]
        public void ContainsKey_NonExistingKey_ReturnsFalse()
        {
            var col = new SrcCollection<string>();
            Assert.IsFalse(col.ContainsKey("nonExistingKey"));
        }

        // ──────────────────────────────────────────────
        // Remove
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Remove_ExistingKey_DecreasesCount()
        {
            var col = new SrcCollection<string> { ["myKey"] = "value" };
            col.Remove("myKey");
            Assert.AreEqual(0, col.Count);
        }

        [TestMethod]
        public void Remove_ExistingKey_KeyNoLongerExists()
        {
            var col = new SrcCollection<string> { ["myKey"] = "value" };
            col.Remove("myKey");
            Assert.IsFalse(col.ContainsKey("myKey"));
        }

        // ──────────────────────────────────────────────
        // 1-indexed access
        // ──────────────────────────────────────────────

        [TestMethod]
        public void OneIndexed_FirstItemAtIndex1()
        {
            var col = new SrcCollection<string> { ["a"] = "first", ["b"] = "second" };
            Assert.AreEqual("first", col[1]);
        }

        [TestMethod]
        public void OneIndexed_SecondItemAtIndex2()
        {
            var col = new SrcCollection<string> { ["a"] = "first", ["b"] = "second" };
            Assert.AreEqual("second", col[2]);
        }

        // ──────────────────────────────────────────────
        // Overwrite
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetDuplicateKey_ThrowsException()
        {
            var col = new SrcCollection<string> { ["k"] = "initial" };
            // 既存キーへの再追加は例外
            Assert.ThrowsException<System.ArgumentException>(() => { col["k"] = "updated"; });
        }

        // ──────────────────────────────────────────────
        // 型: int
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IntCollection_StoresValues()
        {
            var col = new SrcCollection<int> { ["x"] = 42 };
            Assert.AreEqual(42, col["x"]);
        }

        [TestMethod]
        public void IntCollection_DefaultForMissingKey_ReturnsZero()
        {
            var col = new SrcCollection<int>();
            Assert.AreEqual(0, col["missing"]);
        }
    }
}
