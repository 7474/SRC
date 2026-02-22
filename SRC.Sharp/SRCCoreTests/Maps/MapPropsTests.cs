using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Maps;
using SRCCore.TestLib;

namespace SRCCore.Maps.Tests
{
    /// <summary>
    /// Map クラスのプロパティと簡単なメソッドのユニットテスト
    /// </summary>
    [TestClass]
    public class MapPropsTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // MapFileName プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapFileName_DefaultIsNull()
        {
            var src = CreateSrc();
            Assert.IsNull(src.Map.MapFileName);
        }

        [TestMethod]
        public void MapFileName_CanBeSet()
        {
            var src = CreateSrc();
            src.Map.MapFileName = "test.map";
            Assert.AreEqual("test.map", src.Map.MapFileName);
        }

        // ──────────────────────────────────────────────
        // IsStatusView プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsStatusView_WhenMapFileNameIsNull_ReturnsTrue()
        {
            var src = CreateSrc();
            src.Map.MapFileName = null;
            Assert.IsTrue(src.Map.IsStatusView);
        }

        [TestMethod]
        public void IsStatusView_WhenMapFileNameIsEmpty_ReturnsTrue()
        {
            var src = CreateSrc();
            src.Map.MapFileName = "";
            Assert.IsTrue(src.Map.IsStatusView);
        }

        [TestMethod]
        public void IsStatusView_WhenMapFileNameIsSet_ReturnsFalse()
        {
            var src = CreateSrc();
            src.Map.MapFileName = "test.map";
            Assert.IsFalse(src.Map.IsStatusView);
        }

        // ──────────────────────────────────────────────
        // MapWidth / MapHeight
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapWidth_DefaultIsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Map.MapWidth);
        }

        [TestMethod]
        public void MapHeight_DefaultIsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Map.MapHeight);
        }

        [TestMethod]
        public void MapWidth_CanBeSet()
        {
            var src = CreateSrc();
            src.Map.MapWidth = 20;
            Assert.AreEqual(20, src.Map.MapWidth);
        }

        [TestMethod]
        public void MapHeight_CanBeSet()
        {
            var src = CreateSrc();
            src.Map.MapHeight = 15;
            Assert.AreEqual(15, src.Map.MapHeight);
        }

        // ──────────────────────────────────────────────
        // MapDrawMode
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapDrawMode_DefaultIsNull()
        {
            var src = CreateSrc();
            Assert.IsNull(src.Map.MapDrawMode);
        }

        [TestMethod]
        public void MapDrawMode_CanBeSet()
        {
            var src = CreateSrc();
            src.Map.MapDrawMode = "夜";
            Assert.AreEqual("夜", src.Map.MapDrawMode);
        }

        // ──────────────────────────────────────────────
        // MapDrawIsMapOnly
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapDrawIsMapOnly_DefaultIsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.Map.MapDrawIsMapOnly);
        }

        [TestMethod]
        public void MapDrawIsMapOnly_CanBeSetTrue()
        {
            var src = CreateSrc();
            src.Map.MapDrawIsMapOnly = true;
            Assert.IsTrue(src.Map.MapDrawIsMapOnly);
        }

        // ──────────────────────────────────────────────
        // IsMapDirty
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsMapDirty_DefaultIsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.Map.IsMapDirty);
        }

        [TestMethod]
        public void IsMapDirty_CanBeSetTrue()
        {
            var src = CreateSrc();
            src.Map.IsMapDirty = true;
            Assert.IsTrue(src.Map.IsMapDirty);
        }

        // ──────────────────────────────────────────────
        // IsInside メソッド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsInside_WhenMapIsZeroByZero_ReturnsFalse()
        {
            var src = CreateSrc();
            // MapWidth=0, MapHeight=0 → 全ての座標が範囲外
            Assert.IsFalse(src.Map.IsInside(1, 1));
        }

        [TestMethod]
        public void IsInside_WhenCoordinatesValid_ReturnsTrue()
        {
            var src = CreateSrc();
            src.Map.MapWidth = 10;
            src.Map.MapHeight = 10;
            Assert.IsTrue(src.Map.IsInside(5, 5));
        }

        [TestMethod]
        public void IsInside_WhenOutOfRange_ReturnsFalse()
        {
            var src = CreateSrc();
            src.Map.MapWidth = 10;
            src.Map.MapHeight = 10;
            Assert.IsFalse(src.Map.IsInside(0, 5));
        }

        [TestMethod]
        public void IsInside_WhenXTooLarge_ReturnsFalse()
        {
            var src = CreateSrc();
            src.Map.MapWidth = 10;
            src.Map.MapHeight = 10;
            Assert.IsFalse(src.Map.IsInside(11, 5));
        }

        [TestMethod]
        public void IsInside_WhenBoundary_ReturnsTrue()
        {
            var src = CreateSrc();
            src.Map.MapWidth = 10;
            src.Map.MapHeight = 10;
            Assert.IsTrue(src.Map.IsInside(1, 1));
            Assert.IsTrue(src.Map.IsInside(10, 10));
        }

        // ──────────────────────────────────────────────
        // MAX_TERRAIN_DATA_NUM 定数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MaxTerrainDataNum_IsTwoThousand()
        {
            Assert.AreEqual(2000, Map.MAX_TERRAIN_DATA_NUM);
        }

        // ──────────────────────────────────────────────
        // MapDrawFilterTransPercent
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapDrawFilterTransPercent_DefaultIsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0d, src.Map.MapDrawFilterTransPercent);
        }

        [TestMethod]
        public void MapDrawFilterTransPercent_CanBeSet()
        {
            var src = CreateSrc();
            src.Map.MapDrawFilterTransPercent = 0.5;
            Assert.AreEqual(0.5, src.Map.MapDrawFilterTransPercent);
        }
    }
}
