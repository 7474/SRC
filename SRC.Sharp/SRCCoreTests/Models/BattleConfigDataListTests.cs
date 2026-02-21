using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// BattleConfigDataList クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class BattleConfigDataListTests
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
            var list = src.BCList;
            var initial = list.Count();
            list.Add("ダメージ計算");
            Assert.AreEqual(initial + 1, list.Count());
        }

        [TestMethod]
        public void Add_ReturnsDataWithCorrectName()
        {
            var src = CreateSRC();
            var list = src.BCList;
            var cd = list.Add("命中率");
            Assert.AreEqual("命中率", cd.Name);
        }

        [TestMethod]
        public void Add_Multiple_AllAdded()
        {
            var src = CreateSRC();
            var list = src.BCList;
            list.Add("計算1");
            list.Add("計算2");
            list.Add("計算3");
            Assert.AreEqual(3, list.Count());
        }

        // ──────────────────────────────────────────────
        // IsDefined
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsDefined_ReturnsTrueAfterAdd()
        {
            var src = CreateSRC();
            var list = src.BCList;
            list.Add("回避率");
            Assert.IsTrue(list.IsDefined("回避率"));
        }

        [TestMethod]
        public void IsDefined_ReturnsFalseForMissing()
        {
            var src = CreateSRC();
            var list = src.BCList;
            Assert.IsFalse(list.IsDefined("存在しない計算式"));
        }

        // ──────────────────────────────────────────────
        // Item
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Item_ReturnsCorrectData()
        {
            var src = CreateSRC();
            var list = src.BCList;
            list.Add("クリティカル");
            var item = list.Item("クリティカル");
            Assert.IsNotNull(item);
            Assert.AreEqual("クリティカル", item.Name);
        }

        [TestMethod]
        public void Item_ForMissing_ReturnsNull()
        {
            var src = CreateSRC();
            var list = src.BCList;
            var item = list.Item("存在しないアイテム");
            Assert.IsNull(item);
        }

        // ──────────────────────────────────────────────
        // Delete
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Delete_DecreasesCount()
        {
            var src = CreateSRC();
            var list = src.BCList;
            list.Add("テスト計算");
            var before = list.Count();
            list.Delete("テスト計算");
            Assert.AreEqual(before - 1, list.Count());
        }

        [TestMethod]
        public void Delete_RemovedItem_IsNoLongerDefined()
        {
            var src = CreateSRC();
            var list = src.BCList;
            list.Add("削除テスト");
            list.Delete("削除テスト");
            Assert.IsFalse(list.IsDefined("削除テスト"));
        }
    }
}
