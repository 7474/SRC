using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Maps;
using SRCCore.Models;

namespace SRCCore.Maps.Tests
{
    /// <summary>
    /// MapCell クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class MapCellTests
    {
        // ──────────────────────────────────────────────
        // EmptyCell
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EmptyCell_IsNotNull()
        {
            Assert.IsNotNull(MapCell.EmptyCell);
        }

        [TestMethod]
        public void EmptyCell_HasExpectedTerrainType()
        {
            Assert.AreEqual(Map.MAX_TERRAIN_DATA_NUM, MapCell.EmptyCell.TerrainType);
        }

        [TestMethod]
        public void EmptyCell_HasNoLayerForLayerType()
        {
            Assert.AreEqual(MapCell.NO_LAYER_NUM, MapCell.EmptyCell.LayerType);
        }

        // ──────────────────────────────────────────────
        // コンストラクタ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_SetsDefaultValues()
        {
            var cell = new MapCell();
            Assert.AreEqual(0, cell.TerrainType);
            Assert.AreEqual(0, cell.BitmapNo);
            Assert.AreEqual(MapCell.NO_LAYER_NUM, cell.LayerType);
            Assert.AreEqual(MapCell.NO_LAYER_NUM, cell.LayerBitmapNo);
            Assert.AreEqual(BoxTypes.Under, cell.BoxType);
        }

        // ──────────────────────────────────────────────
        // Terrain プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Terrain_UnderBoxType_ReturnsUnderTerrain()
        {
            var underTerrain = new TerrainData { Name = "平地" };
            var upperTerrain = new TerrainData { Name = "山岳" };
            var cell = new MapCell
            {
                BoxType = BoxTypes.Under,
                UnderTerrain = underTerrain,
                UpperTerrain = upperTerrain
            };
            Assert.AreEqual(underTerrain, cell.Terrain);
        }

        [TestMethod]
        public void Terrain_UpperBmpOnlyBoxType_ReturnsUnderTerrain()
        {
            var underTerrain = new TerrainData { Name = "平地" };
            var upperTerrain = new TerrainData { Name = "山岳" };
            var cell = new MapCell
            {
                BoxType = BoxTypes.UpperBmpOnly,
                UnderTerrain = underTerrain,
                UpperTerrain = upperTerrain
            };
            Assert.AreEqual(underTerrain, cell.Terrain);
        }

        [TestMethod]
        public void Terrain_UpperBoxType_ReturnsUpperTerrain()
        {
            var underTerrain = new TerrainData { Name = "平地" };
            var upperTerrain = new TerrainData { Name = "山岳" };
            var cell = new MapCell
            {
                BoxType = BoxTypes.Upper,
                UnderTerrain = underTerrain,
                UpperTerrain = upperTerrain
            };
            Assert.AreEqual(upperTerrain, cell.Terrain);
        }

        [TestMethod]
        public void Terrain_UpperDataOnlyBoxType_ReturnsUpperTerrain()
        {
            var underTerrain = new TerrainData { Name = "平地" };
            var upperTerrain = new TerrainData { Name = "山岳" };
            var cell = new MapCell
            {
                BoxType = BoxTypes.UpperDataOnly,
                UnderTerrain = underTerrain,
                UpperTerrain = upperTerrain
            };
            Assert.AreEqual(upperTerrain, cell.Terrain);
        }

        // ──────────────────────────────────────────────
        // NO_LAYER_NUM 定数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NoLayerNum_IsExpectedValue()
        {
            Assert.AreEqual(10000, MapCell.NO_LAYER_NUM);
        }

        // ──────────────────────────────────────────────
        // X, Y プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void XY_CanBeSetAndRead()
        {
            var cell = new MapCell { X = 5, Y = 10 };
            Assert.AreEqual(5, cell.X);
            Assert.AreEqual(10, cell.Y);
        }
    }
}
