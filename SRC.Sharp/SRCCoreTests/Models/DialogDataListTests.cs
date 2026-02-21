using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// DialogDataList クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class DialogDataListTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // Add / Count
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Add_IncreasesCount()
        {
            var src = CreateSRC();
            var list = src.DDList;
            var initial = list.Count();
            list.Add("アムロ");
            Assert.AreEqual(initial + 1, list.Count());
        }

        [TestMethod]
        public void Add_ReturnsDialogDataWithCorrectName()
        {
            var src = CreateSRC();
            var list = src.DDList;
            var dd = list.Add("シャア");
            Assert.AreEqual("シャア", dd.Name);
        }

        [TestMethod]
        public void Add_Multiple_AllAdded()
        {
            var src = CreateSRC();
            var list = src.DDList;
            list.Add("アムロ");
            list.Add("シャア");
            list.Add("セイラ");
            Assert.AreEqual(3, list.Count());
        }

        // ──────────────────────────────────────────────
        // IsDefined
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsDefined_ReturnsTrueAfterAdd()
        {
            var src = CreateSRC();
            var list = src.DDList;
            list.Add("カミーユ");
            Assert.IsTrue(list.IsDefined("カミーユ"));
        }

        [TestMethod]
        public void IsDefined_ReturnsFalseForMissing()
        {
            var src = CreateSRC();
            var list = src.DDList;
            Assert.IsFalse(list.IsDefined("存在しないパイロット_テスト"));
        }

        // ──────────────────────────────────────────────
        // Item
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Item_ReturnsCorrectDialogData()
        {
            var src = CreateSRC();
            var list = src.DDList;
            list.Add("ジュドー");
            var item = list.Item("ジュドー");
            Assert.IsNotNull(item);
            Assert.AreEqual("ジュドー", item.Name);
        }

        [TestMethod]
        public void Item_ForMissing_ReturnsNull()
        {
            var src = CreateSRC();
            var list = src.DDList;
            var item = list.Item("存在しない");
            Assert.IsNull(item);
        }

        // ──────────────────────────────────────────────
        // Delete
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Delete_DecreasesCount()
        {
            var src = CreateSRC();
            var list = src.DDList;
            list.Add("イノーバ");
            var before = list.Count();
            list.Delete("イノーバ");
            Assert.AreEqual(before - 1, list.Count());
        }

        [TestMethod]
        public void Delete_RemovedItem_IsNoLongerDefined()
        {
            var src = CreateSRC();
            var list = src.DDList;
            list.Add("削除対象");
            list.Delete("削除対象");
            Assert.IsFalse(list.IsDefined("削除対象"));
        }

        // ──────────────────────────────────────────────
        // Items コレクション
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Items_ReturnsAllAddedItems()
        {
            var src = CreateSRC();
            var list = src.DDList;
            list.Add("キャラ1");
            list.Add("キャラ2");
            Assert.AreEqual(2, list.Items.Count);
        }
    }
}
