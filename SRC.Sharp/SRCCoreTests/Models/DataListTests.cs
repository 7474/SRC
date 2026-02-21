using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// AliasDataList, BattleConfigDataList, MessageDataList クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class DataListTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // AliasDataList
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AliasDataList_Add_IncreasesCount()
        {
            var src = CreateSRC();
            var list = src.ALDList;
            var initial = list.Count();
            list.Add("格闘エリアス");
            Assert.AreEqual(initial + 1, list.Count());
        }

        [TestMethod]
        public void AliasDataList_IsDefined_ReturnsTrueAfterAdd()
        {
            var src = CreateSRC();
            var list = src.ALDList;
            list.Add("特殊エリアス");
            Assert.IsTrue(list.IsDefined("特殊エリアス"));
        }

        [TestMethod]
        public void AliasDataList_IsDefined_ReturnsFalseForMissing()
        {
            var src = CreateSRC();
            var list = src.ALDList;
            Assert.IsFalse(list.IsDefined("存在しないエリアス"));
        }

        [TestMethod]
        public void AliasDataList_Item_ReturnsAddedData()
        {
            var src = CreateSRC();
            var list = src.ALDList;
            var added = list.Add("テストエリアス");
            var item = list.Item("テストエリアス");
            Assert.IsNotNull(item);
            Assert.AreEqual("テストエリアス", item.Name);
        }

        [TestMethod]
        public void AliasDataList_Delete_RemovesData()
        {
            var src = CreateSRC();
            var list = src.ALDList;
            list.Add("削除テスト");
            Assert.IsTrue(list.IsDefined("削除テスト"));
            list.Delete("削除テスト");
            Assert.IsFalse(list.IsDefined("削除テスト"));
        }

        // ──────────────────────────────────────────────
        // MessageDataList
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MessageDataList_Add_IncreasesCount()
        {
            var src = CreateSRC();
            var list = src.MDList;
            var initial = list.Count();
            list.Add("アムロ");
            Assert.AreEqual(initial + 1, list.Count());
        }

        [TestMethod]
        public void MessageDataList_IsDefined_ReturnsTrueAfterAdd()
        {
            var src = CreateSRC();
            var list = src.MDList;
            list.Add("シャア");
            Assert.IsTrue(list.IsDefined("シャア"));
        }

        [TestMethod]
        public void MessageDataList_IsDefined_ReturnsFalseForMissing()
        {
            var src = CreateSRC();
            var list = src.MDList;
            Assert.IsFalse(list.IsDefined("存在しないパイロット"));
        }

        [TestMethod]
        public void MessageDataList_Item_ReturnsAddedData()
        {
            var src = CreateSRC();
            var list = src.MDList;
            list.Add("アムロ");
            var item = list.Item("アムロ");
            Assert.IsNotNull(item);
            Assert.AreEqual("アムロ", item.Name);
        }

        [TestMethod]
        public void MessageDataList_Delete_RemovesData()
        {
            var src = CreateSRC();
            var list = src.MDList;
            list.Add("削除キャラ");
            Assert.IsTrue(list.IsDefined("削除キャラ"));
            list.Delete("削除キャラ");
            Assert.IsFalse(list.IsDefined("削除キャラ"));
        }

        // ──────────────────────────────────────────────
        // BattleConfigDataList
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BattleConfigDataList_Add_IncreasesCount()
        {
            var src = CreateSRC();
            var list = src.BCList;
            var initial = list.Count();
            list.Add("ダメージ計算式");
            Assert.AreEqual(initial + 1, list.Count());
        }

        [TestMethod]
        public void BattleConfigDataList_IsDefined_ReturnsTrueAfterAdd()
        {
            var src = CreateSRC();
            var list = src.BCList;
            list.Add("命中率計算式");
            Assert.IsTrue(list.IsDefined("命中率計算式"));
        }

        [TestMethod]
        public void BattleConfigDataList_IsDefined_ReturnsFalseForMissing()
        {
            var src = CreateSRC();
            var list = src.BCList;
            Assert.IsFalse(list.IsDefined("存在しない計算式"));
        }

        [TestMethod]
        public void BattleConfigDataList_Item_ReturnsAddedData()
        {
            var src = CreateSRC();
            var list = src.BCList;
            list.Add("回避率計算式");
            var item = list.Item("回避率計算式");
            Assert.IsNotNull(item);
            Assert.AreEqual("回避率計算式", item.Name);
        }

        [TestMethod]
        public void BattleConfigDataList_Delete_RemovesData()
        {
            var src = CreateSRC();
            var list = src.BCList;
            list.Add("テスト計算式");
            Assert.IsTrue(list.IsDefined("テスト計算式"));
            list.Delete("テスト計算式");
            Assert.IsFalse(list.IsDefined("テスト計算式"));
        }

        [TestMethod]
        public void AliasDataList_MultipleAdd_CountIncreases()
        {
            var src = CreateSRC();
            var list = src.ALDList;
            var initial = list.Count();
            list.Add("エリアスA");
            list.Add("エリアスB");
            list.Add("エリアスC");
            Assert.AreEqual(initial + 3, list.Count());
        }

        [TestMethod]
        public void MessageDataList_Delete2_RemovesData()
        {
            var src = CreateSRC();
            var list = src.MDList;
            list.Add("削除メッセージ2");
            Assert.IsTrue(list.IsDefined("削除メッセージ2"));
            list.Delete("削除メッセージ2");
            Assert.IsFalse(list.IsDefined("削除メッセージ2"));
        }

        [TestMethod]
        public void MessageDataList_MultipleAdd_CountIncreases()
        {
            var src = CreateSRC();
            var list = src.MDList;
            var initial = list.Count();
            list.Add("キャラA");
            list.Add("キャラB");
            Assert.AreEqual(initial + 2, list.Count());
        }

        [TestMethod]
        public void BattleConfigDataList_MultipleAdd_AllDefined()
        {
            var src = CreateSRC();
            var list = src.BCList;
            list.Add("計算式X");
            list.Add("計算式Y");
            Assert.IsTrue(list.IsDefined("計算式X"));
            Assert.IsTrue(list.IsDefined("計算式Y"));
        }
    }
}
