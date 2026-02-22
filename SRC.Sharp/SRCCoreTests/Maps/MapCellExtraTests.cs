using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Maps;
using SRCCore.Models;

namespace SRCCore.Maps.Tests
{
    [TestClass]
    public class MapCellExtraTests
    {
        // ===== MapCell creation and defaults =====

        [TestMethod]
        public void MapCell_DefaultConstructor_TerrainTypeIsZero()
        {
            var cell = new MapCell();
            Assert.AreEqual(0, cell.TerrainType);
        }

        [TestMethod]
        public void MapCell_DefaultConstructor_BitmapNoIsZero()
        {
            var cell = new MapCell();
            Assert.AreEqual(0, cell.BitmapNo);
        }

        [TestMethod]
        public void MapCell_DefaultConstructor_LayerTypeIsNoLayerNum()
        {
            var cell = new MapCell();
            Assert.AreEqual(MapCell.NO_LAYER_NUM, cell.LayerType);
        }

        [TestMethod]
        public void MapCell_DefaultConstructor_BoxTypeIsUnder()
        {
            var cell = new MapCell();
            Assert.AreEqual(BoxTypes.Under, cell.BoxType);
        }

        // ===== MapCell property setters =====

        [TestMethod]
        public void MapCell_X_CanBeSetAndRead()
        {
            var cell = new MapCell { X = 5 };
            Assert.AreEqual(5, cell.X);
        }

        [TestMethod]
        public void MapCell_Y_CanBeSetAndRead()
        {
            var cell = new MapCell { Y = 10 };
            Assert.AreEqual(10, cell.Y);
        }

        [TestMethod]
        public void MapCell_TerrainType_CanBeSetAndRead()
        {
            var cell = new MapCell { TerrainType = 3 };
            Assert.AreEqual(3, cell.TerrainType);
        }

        [TestMethod]
        public void MapCell_BitmapNo_CanBeSetAndRead()
        {
            var cell = new MapCell { BitmapNo = 7 };
            Assert.AreEqual(7, cell.BitmapNo);
        }

        // ===== Terrain property (BoxType-based) =====

        [TestMethod]
        public void Terrain_BoxTypeUnder_ReturnsUnderTerrain()
        {
            var underTerrain = new TerrainData();
            var upperTerrain = new TerrainData();
            var cell = new MapCell
            {
                BoxType = BoxTypes.Under,
                UnderTerrain = underTerrain,
                UpperTerrain = upperTerrain,
            };
            Assert.AreSame(underTerrain, cell.Terrain);
        }

        [TestMethod]
        public void Terrain_BoxTypeUpperBmpOnly_ReturnsUnderTerrain()
        {
            var underTerrain = new TerrainData();
            var upperTerrain = new TerrainData();
            var cell = new MapCell
            {
                BoxType = BoxTypes.UpperBmpOnly,
                UnderTerrain = underTerrain,
                UpperTerrain = upperTerrain,
            };
            Assert.AreSame(underTerrain, cell.Terrain);
        }

        [TestMethod]
        public void Terrain_BoxTypeUpper_ReturnsUpperTerrain()
        {
            var underTerrain = new TerrainData();
            var upperTerrain = new TerrainData();
            var cell = new MapCell
            {
                BoxType = BoxTypes.Upper,
                UnderTerrain = underTerrain,
                UpperTerrain = upperTerrain,
            };
            Assert.AreSame(upperTerrain, cell.Terrain);
        }

        // ===== EmptyCell static =====

        [TestMethod]
        public void EmptyCell_IsNotNull()
        {
            Assert.IsNotNull(MapCell.EmptyCell);
        }

        [TestMethod]
        public void EmptyCell_TerrainType_IsMaxTerrainDataNum()
        {
            Assert.AreEqual(Map.MAX_TERRAIN_DATA_NUM, MapCell.EmptyCell.TerrainType);
        }

        [TestMethod]
        public void NO_LAYER_NUM_Is10000()
        {
            Assert.AreEqual(10000, MapCell.NO_LAYER_NUM);
        }
    }
}
