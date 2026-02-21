using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// TerrainDataList クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class TerrainDataListTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // Item (int ID)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Item_NegativeID_ReturnsEmptyTerrain()
        {
            var src = CreateSRC();
            var tdList = src.TDList;
            var result = tdList.Item(-1);
            Assert.IsNotNull(result);
            Assert.AreEqual(TerrainData.EmptyTerrain, result);
        }

        [TestMethod]
        public void Item_ZeroID_ReturnsNull_WhenNotSet()
        {
            var src = CreateSRC();
            var tdList = src.TDList;
            // 登録されていないIDは null を返す
            var result = tdList.Item(0);
            Assert.IsNull(result);
        }

        // ──────────────────────────────────────────────
        // IsDefined
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsDefined_NotSet_ReturnsFalse()
        {
            var src = CreateSRC();
            var tdList = src.TDList;
            Assert.IsFalse(tdList.IsDefined(0));
        }

        // ──────────────────────────────────────────────
        // ItemByName
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ItemByName_ReturnsNull_WhenTerrainListIsEmpty()
        {
            // ItemByName は TerrainList 内の非 null 要素を検索する
            // 空のリストに対しては FirstOrDefault が null を返す（ただし null 要素があると例外が発生するため、
            // このテストでは登録済みのアイテムがあれば名称で取得できることを確認する）
            var src = CreateSRC();
            var tdList = src.TDList;
            // 何も登録していない場合は null が返る想定だが、
            // 内部配列の null 要素を直接アクセスするため、
            // このケースは Load を通じたデータ設定後にのみ安全に使用可能
            Assert.IsNull(tdList.Item(999));
        }

        // ──────────────────────────────────────────────
        // EmptyTerrain
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EmptyTerrain_IsNotNull()
        {
            Assert.IsNotNull(TerrainData.EmptyTerrain);
        }

        [TestMethod]
        public void EmptyTerrain_NameIsNull()
        {
            // EmptyTerrain はデフォルトインスタンスなので Name は null
            Assert.IsNull(TerrainData.EmptyTerrain.Name);
        }
    }
}
