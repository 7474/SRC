using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Maps;
using SRCCore.Models;

namespace SRCCore.Maps.Tests
{
    /// <summary>
    /// MapCell クラスのさらに詳細なテスト
    /// </summary>
    [TestClass]
    public class MapCellFurtherTests
    {
        // ──────────────────────────────────────────────
        // TerrainType の設定・読み取り
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TerrainType_CanBeSetToLargeValue()
        {
            var cell = new MapCell { TerrainType = 9999 };
            Assert.AreEqual(9999, cell.TerrainType);
        }

        [TestMethod]
        public void TerrainType_CanBeSetToZero()
        {
            var cell = new MapCell { TerrainType = 0 };
            Assert.AreEqual(0, cell.TerrainType);
        }

        [TestMethod]
        public void TerrainType_DefaultIsZero()
        {
            var cell = new MapCell();
            Assert.AreEqual(0, cell.TerrainType);
        }

        // ──────────────────────────────────────────────
        // BitmapNo の設定・読み取り
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BitmapNo_CanBeSetToLargeValue()
        {
            var cell = new MapCell { BitmapNo = 9999 };
            Assert.AreEqual(9999, cell.BitmapNo);
        }

        [TestMethod]
        public void BitmapNo_DefaultIsZero()
        {
            var cell = new MapCell();
            Assert.AreEqual(0, cell.BitmapNo);
        }

        // ──────────────────────────────────────────────
        // LayerType の設定・読み取り
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LayerType_CanBeSetToArbitraryValue()
        {
            var cell = new MapCell { LayerType = 5 };
            Assert.AreEqual(5, cell.LayerType);
        }

        [TestMethod]
        public void LayerType_DefaultIsNoLayerNum()
        {
            var cell = new MapCell();
            Assert.AreEqual(MapCell.NO_LAYER_NUM, cell.LayerType);
        }

        // ──────────────────────────────────────────────
        // EmptyCell 詳細テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EmptyCell_BitmapNo_IsZero()
        {
            Assert.AreEqual(0, MapCell.EmptyCell.BitmapNo);
        }

        [TestMethod]
        public void EmptyCell_LayerBitmapNo_IsZeroNotNoLayerNum()
        {
            // EmptyCell は LayerBitmapNo = 0 (NO_LAYER_NUM ではない)
            Assert.AreEqual(0, MapCell.EmptyCell.LayerBitmapNo);
        }

        [TestMethod]
        public void EmptyCell_BoxType_IsUnder()
        {
            // EmptyCell は明示的に BoxTypes.Under = 1 に設定されている
            Assert.AreEqual(BoxTypes.Under, MapCell.EmptyCell.BoxType);
        }

        [TestMethod]
        public void EmptyCell_UnderTerrain_IsEmptyTerrain()
        {
            Assert.AreEqual(TerrainData.EmptyTerrain, MapCell.EmptyCell.UnderTerrain);
        }

        [TestMethod]
        public void EmptyCell_UpperTerrain_IsEmptyTerrain()
        {
            Assert.AreEqual(TerrainData.EmptyTerrain, MapCell.EmptyCell.UpperTerrain);
        }

        // ──────────────────────────────────────────────
        // X, Y の境界値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void X_CanBeSetToNegative()
        {
            var cell = new MapCell { X = -1 };
            Assert.AreEqual(-1, cell.X);
        }

        [TestMethod]
        public void Y_CanBeSetToNegative()
        {
            var cell = new MapCell { Y = -1 };
            Assert.AreEqual(-1, cell.Y);
        }

        [TestMethod]
        public void XY_CanBeSetToLargeValues()
        {
            var cell = new MapCell { X = 999, Y = 999 };
            Assert.AreEqual(999, cell.X);
            Assert.AreEqual(999, cell.Y);
        }

        [TestMethod]
        public void X_DefaultIsZero()
        {
            var cell = new MapCell();
            Assert.AreEqual(0, cell.X);
        }

        [TestMethod]
        public void Y_DefaultIsZero()
        {
            var cell = new MapCell();
            Assert.AreEqual(0, cell.Y);
        }

        // ──────────────────────────────────────────────
        // Terrain プロパティ - null terrain
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Terrain_UpperBoxType_NullUpperTerrain_ReturnsNull()
        {
            var cell = new MapCell
            {
                BoxType = BoxTypes.Upper,
                UnderTerrain = new TerrainData { Name = "下層" },
                UpperTerrain = null
            };
            Assert.IsNull(cell.Terrain);
        }

        [TestMethod]
        public void Terrain_UnderBoxType_NullUnderTerrain_ReturnsNull()
        {
            var cell = new MapCell
            {
                BoxType = BoxTypes.Under,
                UnderTerrain = null
            };
            Assert.IsNull(cell.Terrain);
        }

        // ──────────────────────────────────────────────
        // NO_LAYER_NUM 定数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NO_LAYER_NUM_IsPositive()
        {
            Assert.IsTrue(MapCell.NO_LAYER_NUM > 0);
        }

        [TestMethod]
        public void NO_LAYER_NUM_IsGreaterThanTypicalTerrainTypes()
        {
            // NO_LAYER_NUM は通常の地形タイプより大きい
            Assert.IsTrue(MapCell.NO_LAYER_NUM > 1000);
        }
    }
}
