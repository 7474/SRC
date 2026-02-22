using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Maps;
using SRCCore.TestLib;
using System.Drawing;

namespace SRCCore.Maps.Tests
{
    /// <summary>
    /// Map クラスのプロパティデフォルト値のユニットテスト
    /// </summary>
    [TestClass]
    public class MapPropertiesTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // コンストラクタのデフォルト値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_MapFileName_IsNull()
        {
            var src = CreateSrc();
            var map = new Map(src);
            Assert.IsNull(map.MapFileName);
        }

        [TestMethod]
        public void Constructor_MapWidth_IsZero()
        {
            var src = CreateSrc();
            var map = new Map(src);
            Assert.AreEqual(0, map.MapWidth);
        }

        [TestMethod]
        public void Constructor_MapHeight_IsZero()
        {
            var src = CreateSrc();
            var map = new Map(src);
            Assert.AreEqual(0, map.MapHeight);
        }

        // ──────────────────────────────────────────────
        // IsStatusView
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsStatusView_WhenMapFileNameIsNull_ReturnsTrue()
        {
            var src = CreateSrc();
            var map = new Map(src);
            map.MapFileName = null;
            Assert.IsTrue(map.IsStatusView);
        }

        [TestMethod]
        public void IsStatusView_WhenMapFileNameIsEmpty_ReturnsTrue()
        {
            var src = CreateSrc();
            var map = new Map(src);
            map.MapFileName = "";
            Assert.IsTrue(map.IsStatusView);
        }

        [TestMethod]
        public void IsStatusView_WhenMapFileNameIsSet_ReturnsFalse()
        {
            var src = CreateSrc();
            var map = new Map(src);
            map.MapFileName = "test.map";
            Assert.IsFalse(map.IsStatusView);
        }

        // ──────────────────────────────────────────────
        // MapDrawFilterColor
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapDrawFilterColor_DefaultIsBlack()
        {
            var src = CreateSrc();
            var map = new Map(src);
            Assert.AreEqual(Color.Black, map.MapDrawFilterColor);
        }

        // ──────────────────────────────────────────────
        // MapDrawFilterTransPercent
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapDrawFilterTransPercent_DefaultIsZero()
        {
            var src = CreateSrc();
            var map = new Map(src);
            Assert.AreEqual(0d, map.MapDrawFilterTransPercent);
        }

        // ──────────────────────────────────────────────
        // MapDrawIsMapOnly
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapDrawIsMapOnly_DefaultIsFalse()
        {
            var src = CreateSrc();
            var map = new Map(src);
            Assert.IsFalse(map.MapDrawIsMapOnly);
        }

        // ──────────────────────────────────────────────
        // MAX_TERRAIN_DATA_NUM
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MAX_TERRAIN_DATA_NUM_IsTwoThousand()
        {
            Assert.AreEqual(2000, Map.MAX_TERRAIN_DATA_NUM);
        }
    }
}
