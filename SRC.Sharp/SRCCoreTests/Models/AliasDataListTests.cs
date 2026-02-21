using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// AliasDataList クラスの RefName メソッドのユニットテスト
    /// </summary>
    [TestClass]
    public class AliasDataListTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // RefName
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RefName_NoneMatching_ReturnsInputName()
        {
            var src = CreateSRC();
            var list = src.ALDList;
            var result = list.RefName("格闘");
            Assert.AreEqual("格闘", result);
        }

        [TestMethod]
        public void RefName_EmptyList_ReturnsInputName()
        {
            var src = CreateSRC();
            var list = src.ALDList;
            var result = list.RefName("特殊");
            Assert.AreEqual("特殊", result);
        }

        [TestMethod]
        public void RefName_WhenAliasMatches_ReturnsAliasName()
        {
            var src = CreateSRC();
            var list = src.ALDList;
            var ad = list.Add("パワーアタック");
            ad.AddAlias("格闘");
            // 最初の要素の strAliasType が "格闘" ならその AliasDataType の Name を返す
            var result = list.RefName("格闘");
            Assert.AreEqual("パワーアタック", result);
        }

        // ──────────────────────────────────────────────
        // IsDefined / Count
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsDefined_ReturnsTrueForAddedAlias()
        {
            var src = CreateSRC();
            var list = src.ALDList;
            list.Add("テストエリアス");
            Assert.IsTrue(list.IsDefined("テストエリアス"));
        }

        [TestMethod]
        public void IsDefined_ReturnsFalseForNotAdded()
        {
            var src = CreateSRC();
            var list = src.ALDList;
            Assert.IsFalse(list.IsDefined("存在しない名前テスト用_XXXYYY"));
        }

        [TestMethod]
        public void Count_IncreasesAfterAdd()
        {
            var src = CreateSRC();
            var list = src.ALDList;
            var initial = list.Count();
            list.Add("新規エリアス1");
            list.Add("新規エリアス2");
            Assert.AreEqual(initial + 2, list.Count());
        }

        // ──────────────────────────────────────────────
        // Items コレクション
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Items_ReturnsAddedAliasData()
        {
            var src = CreateSRC();
            var list = src.ALDList;
            list.Add("テスト用エリアスA");
            Assert.IsTrue(list.Items.Count >= 1);
        }

        // ──────────────────────────────────────────────
        // Delete
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Delete_RemovesItem()
        {
            var src = CreateSRC();
            var list = src.ALDList;
            list.Add("削除テスト用");
            list.Delete("削除テスト用");
            Assert.IsFalse(list.IsDefined("削除テスト用"));
        }

        [TestMethod]
        public void Delete_DecreasesCount()
        {
            var src = CreateSRC();
            var list = src.ALDList;
            list.Add("カウントテスト用");
            var before = list.Count();
            list.Delete("カウントテスト用");
            Assert.AreEqual(before - 1, list.Count());
        }
    }
}
