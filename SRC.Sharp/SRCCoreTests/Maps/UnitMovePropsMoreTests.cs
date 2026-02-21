using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Maps;
using SRCCore.Models;

namespace SRCCore.Maps.Tests
{
    /// <summary>
    /// MapCell クラスのさらなるユニットテスト
    /// </summary>
    [TestClass]
    public class UnitMovePropsMoreTests
    {
        // ──────────────────────────────────────────────
        // 基本プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void XY_CanBeSetAndRead()
        {
            var cell = new MapCell { X = 5, Y = 10 };
            Assert.AreEqual(5, cell.X);
            Assert.AreEqual(10, cell.Y);
        }

        [TestMethod]
        public void TerrainType_CanBeSetAndRead()
        {
            var cell = new MapCell { TerrainType = 3 };
            Assert.AreEqual(3, cell.TerrainType);
        }

        [TestMethod]
        public void BitmapNo_CanBeSetAndRead()
        {
            var cell = new MapCell { BitmapNo = 7 };
            Assert.AreEqual(7, cell.BitmapNo);
        }

        [TestMethod]
        public void LayerType_CanBeSetAndRead()
        {
            var cell = new MapCell { LayerType = 2 };
            Assert.AreEqual(2, cell.LayerType);
        }

        [TestMethod]
        public void BoxType_DefaultIsUnder()
        {
            var cell = new MapCell();
            // デフォルト値はBoxTypes.Under (0)
            Assert.AreEqual(BoxTypes.Under, cell.BoxType);
        }

        // ──────────────────────────────────────────────
        // NO_LAYER_NUM 定数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NoLayerNum_ConstantIs10000()
        {
            Assert.AreEqual(10000, MapCell.NO_LAYER_NUM);
        }

        // ──────────────────────────────────────────────
        // EmptyCell
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EmptyCell_IsNotNull()
        {
            Assert.IsNotNull(MapCell.EmptyCell);
        }

        [TestMethod]
        public void EmptyCell_LayerTypeIsNoLayerNum()
        {
            Assert.AreEqual(MapCell.NO_LAYER_NUM, MapCell.EmptyCell.LayerType);
        }

        // ──────────────────────────────────────────────
        // Terrain プロパティ (BoxType に基づく)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Terrain_BoxTypeUnder_ReturnsUnderTerrain()
        {
            var underTerrain = new TerrainData { Name = "平地" };
            var upperTerrain = new TerrainData { Name = "山岳" };
            var cell = new MapCell
            {
                BoxType = BoxTypes.Under,
                UnderTerrain = underTerrain,
                UpperTerrain = upperTerrain
            };

            Assert.AreEqual("平地", cell.Terrain.Name);
        }

        [TestMethod]
        public void Terrain_BoxTypeUpper_ReturnsUpperTerrain()
        {
            var underTerrain = new TerrainData { Name = "平地" };
            var upperTerrain = new TerrainData { Name = "山岳" };
            var cell = new MapCell
            {
                BoxType = BoxTypes.Upper,
                UnderTerrain = underTerrain,
                UpperTerrain = upperTerrain
            };

            Assert.AreEqual("山岳", cell.Terrain.Name);
        }

        [TestMethod]
        public void Terrain_BoxTypeUpperBmpOnly_ReturnsUnderTerrain()
        {
            var underTerrain = new TerrainData { Name = "草原" };
            var upperTerrain = new TerrainData { Name = "城" };
            var cell = new MapCell
            {
                BoxType = BoxTypes.UpperBmpOnly,
                UnderTerrain = underTerrain,
                UpperTerrain = upperTerrain
            };

            Assert.AreEqual("草原", cell.Terrain.Name);
        }
    }
}
