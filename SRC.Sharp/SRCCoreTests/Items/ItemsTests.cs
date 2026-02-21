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
    }
}
