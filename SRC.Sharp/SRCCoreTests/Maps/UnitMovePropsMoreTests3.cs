using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Maps;
using SRCCore.Models;

namespace SRCCore.Maps.Tests
{
    [TestClass]
    public class UnitMovePropsMoreTests3
    {
        [TestMethod]
        public void MapCell_DefaultConstructor_TerrainTypeIsZero()
        {
            var cell = new MapCell();
            Assert.AreEqual(0, cell.TerrainType);
        }

        [TestMethod]
        public void MapCell_DefaultConstructor_LayerBitmapNoIsNoLayerNum()
        {
            var cell = new MapCell();
            Assert.AreEqual(MapCell.NO_LAYER_NUM, cell.LayerBitmapNo);
        }

        [TestMethod]
        public void MapCell_DefaultConstructor_BoxTypeIsUnder()
        {
            var cell = new MapCell();
            Assert.AreEqual(BoxTypes.Under, cell.BoxType);
        }

        [TestMethod]
        public void MapCell_SetX_CanRead()
        {
            var cell = new MapCell { X = 3 };
            Assert.AreEqual(3, cell.X);
        }

        [TestMethod]
        public void MapCell_SetY_CanRead()
        {
            var cell = new MapCell { Y = 7 };
            Assert.AreEqual(7, cell.Y);
        }

        [TestMethod]
        public void MapCell_SetLayerBitmapNo_CanRead()
        {
            var cell = new MapCell { LayerBitmapNo = 5 };
            Assert.AreEqual(5, cell.LayerBitmapNo);
        }

        [TestMethod]
        public void MapCell_BoxTypeUpperDataOnly_TerrainReturnsUpperTerrain()
        {
            var underTerrain = new TerrainData { Name = "砂漠" };
            var upperTerrain = new TerrainData { Name = "森" };
            var cell = new MapCell
            {
                BoxType = BoxTypes.UpperDataOnly,
                UnderTerrain = underTerrain,
                UpperTerrain = upperTerrain
            };
            Assert.AreEqual("森", cell.Terrain.Name);
        }

        [TestMethod]
        public void MapCell_EmptyCell_BoxTypeIsUnder()
        {
            Assert.AreEqual(BoxTypes.Under, MapCell.EmptyCell.BoxType);
        }

        [TestMethod]
        public void MapCell_EmptyCell_BitmapNoIsZero()
        {
            Assert.AreEqual(0, MapCell.EmptyCell.BitmapNo);
        }

        [TestMethod]
        public void MapCell_UnderTerrain_CanBeSet()
        {
            var td = new TerrainData { Name = "草原" };
            var cell = new MapCell { UnderTerrain = td };
            Assert.AreEqual("草原", cell.UnderTerrain.Name);
        }

        [TestMethod]
        public void MapCell_UpperTerrain_CanBeSet()
        {
            var td = new TerrainData { Name = "城壁" };
            var cell = new MapCell { UpperTerrain = td };
            Assert.AreEqual("城壁", cell.UpperTerrain.Name);
        }

        [TestMethod]
        public void NoLayerNum_Is10000()
        {
            Assert.AreEqual(10000, MapCell.NO_LAYER_NUM);
        }
    }
}
