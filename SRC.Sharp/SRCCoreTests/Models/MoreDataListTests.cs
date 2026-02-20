using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// PilotDataList, ItemDataList, UnitDataList, SpecialPowerDataList クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class MoreDataListTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // PilotDataList
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PilotDataList_Add_IncreasesCount()
        {
            var src = CreateSRC();
            var list = src.PDList;
            var initial = list.Count();
            list.Add("アムロ");
            Assert.AreEqual(initial + 1, list.Count());
        }

        [TestMethod]
        public void PilotDataList_IsDefined_ReturnsTrueAfterAdd()
        {
            var src = CreateSRC();
            var list = src.PDList;
            list.Add("シャア");
            Assert.IsTrue(list.IsDefined("シャア"));
        }

        [TestMethod]
        public void PilotDataList_IsDefined_ReturnsFalseForMissing()
        {
            var src = CreateSRC();
            var list = src.PDList;
            Assert.IsFalse(list.IsDefined("存在しないパイロット_テスト用"));
        }

        [TestMethod]
        public void PilotDataList_Item_ReturnsAddedData()
        {
            var src = CreateSRC();
            var list = src.PDList;
            list.Add("ヒイロ");
            var item = list.Item("ヒイロ");
            Assert.IsNotNull(item);
            Assert.AreEqual("ヒイロ", item.Name);
        }

        // ──────────────────────────────────────────────
        // ItemDataList
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ItemDataList_Add_IncreasesCount()
        {
            var src = CreateSRC();
            var list = src.IDList;
            var initial = list.Count();
            list.Add("ミノフスキー粒子");
            Assert.AreEqual(initial + 1, list.Count());
        }

        [TestMethod]
        public void ItemDataList_IsDefined_ReturnsTrueAfterAdd()
        {
            var src = CreateSRC();
            var list = src.IDList;
            list.Add("ビームライフル");
            Assert.IsTrue(list.IsDefined("ビームライフル"));
        }

        [TestMethod]
        public void ItemDataList_IsDefined_ReturnsFalseForMissing()
        {
            var src = CreateSRC();
            var list = src.IDList;
            Assert.IsFalse(list.IsDefined("存在しないアイテム_テスト用"));
        }

        [TestMethod]
        public void ItemDataList_Item_ReturnsAddedData()
        {
            var src = CreateSRC();
            var list = src.IDList;
            list.Add("シールド");
            var item = list.Item("シールド");
            Assert.IsNotNull(item);
            Assert.AreEqual("シールド", item.Name);
        }

        // ──────────────────────────────────────────────
        // UnitDataList
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitDataList_Add_IncreasesCount()
        {
            var src = CreateSRC();
            var list = src.UDList;
            var initial = list.Count();
            list.Add("ガンダム");
            Assert.AreEqual(initial + 1, list.Count());
        }

        [TestMethod]
        public void UnitDataList_IsDefined_ReturnsTrueAfterAdd()
        {
            var src = CreateSRC();
            var list = src.UDList;
            list.Add("ザク");
            Assert.IsTrue(list.IsDefined("ザク"));
        }

        [TestMethod]
        public void UnitDataList_IsDefined_ReturnsFalseForMissing()
        {
            var src = CreateSRC();
            var list = src.UDList;
            Assert.IsFalse(list.IsDefined("存在しないユニット_テスト用"));
        }

        [TestMethod]
        public void UnitDataList_Item_ReturnsAddedData()
        {
            var src = CreateSRC();
            var list = src.UDList;
            list.Add("νガンダム");
            var item = list.Item("νガンダム");
            Assert.IsNotNull(item);
            Assert.AreEqual("νガンダム", item.Name);
        }

        // ──────────────────────────────────────────────
        // SpecialPowerDataList
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SpecialPowerDataList_Add_IncreasesCount()
        {
            var src = CreateSRC();
            var list = src.SPDList;
            var initial = list.Count();
            list.Add("熱血");
            Assert.AreEqual(initial + 1, list.Count());
        }

        [TestMethod]
        public void SpecialPowerDataList_IsDefined_ReturnsTrueAfterAdd()
        {
            var src = CreateSRC();
            var list = src.SPDList;
            list.Add("覚醒");
            Assert.IsTrue(list.IsDefined("覚醒"));
        }

        [TestMethod]
        public void SpecialPowerDataList_IsDefined_ReturnsFalseForMissing()
        {
            var src = CreateSRC();
            var list = src.SPDList;
            Assert.IsFalse(list.IsDefined("存在しないSP_テスト用"));
        }

        [TestMethod]
        public void SpecialPowerDataList_Item_ReturnsAddedData()
        {
            var src = CreateSRC();
            var list = src.SPDList;
            list.Add("必中");
            var item = list.Item("必中");
            Assert.IsNotNull(item);
            Assert.AreEqual("必中", item.Name);
        }

        [TestMethod]
        public void SpecialPowerDataList_Delete_RemovesData()
        {
            var src = CreateSRC();
            var list = src.SPDList;
            list.Add("幸運_テスト");
            Assert.IsTrue(list.IsDefined("幸運_テスト"));
            list.Delete("幸運_テスト");
            Assert.IsFalse(list.IsDefined("幸運_テスト"));
        }
    }
}
