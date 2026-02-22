using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Items;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Items.Tests
{
    /// <summary>
    /// Items クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class ItemsTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // 初期状態
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Count_Initial_IsZero()
        {
            var src = CreateSRC();
            Assert.AreEqual(0, src.IList.Count());
        }

        [TestMethod]
        public void List_Initial_IsEmpty()
        {
            var src = CreateSRC();
            Assert.AreEqual(0, src.IList.List.Count);
        }

        // ──────────────────────────────────────────────
        // Add (IDListに定義がない場合は null を返す)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Add_WithoutItemData_ReturnsNull()
        {
            var src = CreateSRC();
            // IDListに存在しないアイテムを追加しようとすると null を返す
            var item = src.IList.Add("存在しないアイテム_テスト");
            Assert.IsNull(item);
        }

        [TestMethod]
        public void Add_WithoutItemData_DoesNotIncreaseCount()
        {
            var src = CreateSRC();
            src.IList.Add("存在しないアイテム");
            Assert.AreEqual(0, src.IList.Count());
        }

        // ──────────────────────────────────────────────
        // IsDefined / IsDefined2
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsDefined_ForNonExistingItem_ReturnsFalse()
        {
            var src = CreateSRC();
            Assert.IsFalse(src.IList.IsDefined("存在しないアイテムID"));
        }

        [TestMethod]
        public void IsDefined2_ForNonExistingItem_ReturnsFalse()
        {
            var src = CreateSRC();
            Assert.IsFalse(src.IList.IsDefined2("存在しないID"));
        }

        // ──────────────────────────────────────────────
        // Item (存在しない場合は null を返す)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Item_ForNonExistingItem_ReturnsNull()
        {
            var src = CreateSRC();
            Assert.IsNull(src.IList.Item("存在しない"));
        }

        // ──────────────────────────────────────────────
        // Add (IDListに定義がある場合)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Add_WithRegisteredItemData_CreatesItem()
        {
            var src = CreateSRC();
            // IDListにアイテムデータを登録
            var id = src.IDList.Add("テスト装備");
            var item = src.IList.Add("テスト装備");
            Assert.IsNotNull(item);
        }

        [TestMethod]
        public void Add_WithRegisteredItemData_IncreasesCount()
        {
            var src = CreateSRC();
            src.IDList.Add("装備1");
            src.IList.Add("装備1");
            Assert.AreEqual(1, src.IList.Count());
        }

        [TestMethod]
        public void Add_WithRegisteredItemData_SetsNameCorrectly()
        {
            var src = CreateSRC();
            src.IDList.Add("装備2");
            var item = src.IList.Add("装備2");
            Assert.AreEqual("装備2", item.Name);
        }

        [TestMethod]
        public void Add_ItemExistFieldIsTrue()
        {
            var src = CreateSRC();
            src.IDList.Add("装備3");
            var item = src.IList.Add("装備3");
            Assert.IsTrue(item.Exist);
        }

        [TestMethod]
        public void IsDefined_AfterAdd_ReturnsTrue()
        {
            var src = CreateSRC();
            src.IDList.Add("装備4");
            var item = src.IList.Add("装備4");
            Assert.IsTrue(src.IList.IsDefined(item.ID));
        }

        [TestMethod]
        public void IsDefined2_AfterAdd_ReturnsTrue()
        {
            var src = CreateSRC();
            src.IDList.Add("装備5");
            var item = src.IList.Add("装備5");
            Assert.IsTrue(src.IList.IsDefined2(item.ID));
        }

        // ──────────────────────────────────────────────
        // Item (IDで検索 / 名前で検索)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Item_ByID_ReturnsItem()
        {
            var src = CreateSRC();
            src.IDList.Add("装備A");
            var item = src.IList.Add("装備A");
            var found = src.IList.Item(item.ID);
            Assert.IsNotNull(found);
            Assert.AreEqual(item.ID, found.ID);
        }

        [TestMethod]
        public void Item_ByName_ReturnsItem()
        {
            var src = CreateSRC();
            src.IDList.Add("装備B");
            var item = src.IList.Add("装備B");
            var found = src.IList.Item("装備B");
            Assert.IsNotNull(found);
            Assert.AreEqual("装備B", found.Name);
        }

        // ──────────────────────────────────────────────
        // Count (複数追加)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Count_AfterMultipleAdds_ReturnsCorrectCount()
        {
            var src = CreateSRC();
            src.IDList.Add("装備X");
            src.IDList.Add("装備Y");
            src.IList.Add("装備X");
            src.IList.Add("装備Y");
            Assert.AreEqual(2, src.IList.Count());
        }

        // ──────────────────────────────────────────────
        // Clear
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Clear_RemovesAllItems()
        {
            var src = CreateSRC();
            src.IDList.Add("装備C");
            src.IList.Add("装備C");
            src.IList.Clear();
            Assert.AreEqual(0, src.IList.Count());
        }

        [TestMethod]
        public void Clear_ListBecomesEmpty()
        {
            var src = CreateSRC();
            src.IDList.Add("装備D");
            src.IList.Add("装備D");
            src.IList.Clear();
            Assert.AreEqual(0, src.IList.List.Count);
        }

        // ──────────────────────────────────────────────
        // Delete
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Delete_RemovesItemByID()
        {
            var src = CreateSRC();
            src.IDList.Add("装備E");
            var item = src.IList.Add("装備E");
            src.IList.Delete(item.ID);
            Assert.AreEqual(0, src.IList.Count());
        }

        [TestMethod]
        public void Delete_RemovedItemIsNotDefined()
        {
            var src = CreateSRC();
            src.IDList.Add("装備F");
            var item = src.IList.Add("装備F");
            var id = item.ID;
            src.IList.Delete(id);
            Assert.IsFalse(src.IList.IsDefined2(id));
        }

        // ──────────────────────────────────────────────
        // Restore
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Restore_SetsSRCReference()
        {
            var src = CreateSRC();
            src.IDList.Add("装備G");
            var item = src.IList.Add("装備G");

            var src2 = CreateSRC();
            src2.IDList.Add("装備G");
            src.IList.Restore(src2);

            // Restore後もアイテムにアクセスできること
            Assert.IsNotNull(src.IList.Item(item.ID));
        }
    }
}
