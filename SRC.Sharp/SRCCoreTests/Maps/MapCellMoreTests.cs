using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Maps;
using SRCCore.Models;

namespace SRCCore.Maps.Tests
{
    /// <summary>
    /// MapCell クラスの追加テスト（更なるエッジケース）
    /// </summary>
    [TestClass]
    public class MapCellMoreTests
    {
        // ──────────────────────────────────────────────
        // TerrainType の書き込みと読み込み
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TerrainType_CanBeSetAndRead()
        {
            var cell = new MapCell();
            cell.TerrainType = 5;
            Assert.AreEqual(5, cell.TerrainType);
        }

        [TestMethod]
        public void TerrainType_MaxTerrainDataNum_IsSpecialValue()
        {
            // EmptyCell のデフォルト値
            Assert.AreEqual(Map.MAX_TERRAIN_DATA_NUM, MapCell.EmptyCell.TerrainType);
        }

        // ──────────────────────────────────────────────
        // LayerType の書き込みと読み込み
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LayerType_CanBeSetAndRead()
        {
            var cell = new MapCell();
            cell.LayerType = 3;
            Assert.AreEqual(3, cell.LayerType);
        }

        [TestMethod]
        public void LayerType_DefaultValue_IsNoLayerNum()
        {
            var cell = new MapCell();
            Assert.AreEqual(MapCell.NO_LAYER_NUM, cell.LayerType);
        }

        // ──────────────────────────────────────────────
        // BitmapNo
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BitmapNo_CanBeSetAndRead()
        {
            var cell = new MapCell();
            cell.BitmapNo = 7;
            Assert.AreEqual(7, cell.BitmapNo);
        }

        [TestMethod]
        public void BitmapNo_DefaultValue_IsZero()
        {
            var cell = new MapCell();
            Assert.AreEqual(0, cell.BitmapNo);
        }

        // ──────────────────────────────────────────────
        // LayerBitmapNo
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LayerBitmapNo_CanBeSetAndRead()
        {
            var cell = new MapCell();
            cell.LayerBitmapNo = 2;
            Assert.AreEqual(2, cell.LayerBitmapNo);
        }

        [TestMethod]
        public void LayerBitmapNo_DefaultValue_IsNoLayerNum()
        {
            var cell = new MapCell();
            Assert.AreEqual(MapCell.NO_LAYER_NUM, cell.LayerBitmapNo);
        }

        // ──────────────────────────────────────────────
        // BoxType
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BoxType_CanBeSetAndRead()
        {
            var cell = new MapCell();
            cell.BoxType = BoxTypes.Upper;
            Assert.AreEqual(BoxTypes.Upper, cell.BoxType);
        }

        [TestMethod]
        public void BoxType_DefaultValue_IsUnder()
        {
            var cell = new MapCell();
            Assert.AreEqual(BoxTypes.Under, cell.BoxType);
        }

        // ──────────────────────────────────────────────
        // NO_LAYER_NUM 定数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NoLayerNum_IsTenThousand()
        {
            Assert.AreEqual(10000, MapCell.NO_LAYER_NUM);
        }

        // ──────────────────────────────────────────────
        // 複数のセル独立性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TwoCells_AreIndependent()
        {
            var cell1 = new MapCell();
            var cell2 = new MapCell();
            cell1.TerrainType = 1;
            cell2.TerrainType = 2;
            Assert.AreNotEqual(cell1.TerrainType, cell2.TerrainType);
        }
    }
}
