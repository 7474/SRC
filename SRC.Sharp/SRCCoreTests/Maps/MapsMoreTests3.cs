using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Maps;
using SRCCore.TestLib;

namespace SRCCore.Maps.Tests
{
    /// <summary>
    /// Map クラスの追加ユニットテスト（その3）
    /// </summary>
    [TestClass]
    public class MapsMoreTests3
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // SetMapWidth/Height は SetMapSize 経由だと GUI.MainWidth で例外になるため
        // public フィールドに直接代入してテストする
        private void SetSize(Map map, int w, int h)
        {
            map.MapWidth = w;
            map.MapHeight = h;
        }

        // ──────────────────────────────────────────────
        // MapWidth / MapHeight
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapWidth_CanBeSetAndRead()
        {
            var src = CreateSrc();
            SetSize(src.Map, 10, 5);
            Assert.AreEqual(10, src.Map.MapWidth);
        }

        [TestMethod]
        public void MapHeight_CanBeSetAndRead()
        {
            var src = CreateSrc();
            SetSize(src.Map, 10, 5);
            Assert.AreEqual(5, src.Map.MapHeight);
        }

        [TestMethod]
        public void MapSize_Square_BothEqual()
        {
            var src = CreateSrc();
            SetSize(src.Map, 8, 8);
            Assert.AreEqual(8, src.Map.MapWidth);
            Assert.AreEqual(8, src.Map.MapHeight);
        }

        [TestMethod]
        public void MapSize_OneByOne()
        {
            var src = CreateSrc();
            SetSize(src.Map, 1, 1);
            Assert.AreEqual(1, src.Map.MapWidth);
            Assert.AreEqual(1, src.Map.MapHeight);
        }

        // ──────────────────────────────────────────────
        // IsInside
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsInside_CenterCell_ReturnsTrue()
        {
            var src = CreateSrc();
            SetSize(src.Map, 5, 5);
            Assert.IsTrue(src.Map.IsInside(3, 3));
        }

        [TestMethod]
        public void IsInside_TopLeftCorner_ReturnsTrue()
        {
            var src = CreateSrc();
            SetSize(src.Map, 5, 5);
            Assert.IsTrue(src.Map.IsInside(1, 1));
        }

        [TestMethod]
        public void IsInside_BottomRightCorner_ReturnsTrue()
        {
            var src = CreateSrc();
            SetSize(src.Map, 5, 5);
            Assert.IsTrue(src.Map.IsInside(5, 5));
        }

        [TestMethod]
        public void IsInside_XBeyondWidth_ReturnsFalse()
        {
            var src = CreateSrc();
            SetSize(src.Map, 5, 5);
            Assert.IsFalse(src.Map.IsInside(6, 3));
        }

        [TestMethod]
        public void IsInside_YBeyondHeight_ReturnsFalse()
        {
            var src = CreateSrc();
            SetSize(src.Map, 5, 5);
            Assert.IsFalse(src.Map.IsInside(3, 6));
        }

        [TestMethod]
        public void IsInside_XZero_ReturnsFalse()
        {
            var src = CreateSrc();
            SetSize(src.Map, 5, 5);
            Assert.IsFalse(src.Map.IsInside(0, 3));
        }

        [TestMethod]
        public void IsInside_YZero_ReturnsFalse()
        {
            var src = CreateSrc();
            SetSize(src.Map, 5, 5);
            Assert.IsFalse(src.Map.IsInside(3, 0));
        }

        [TestMethod]
        public void IsInside_NegativeX_ReturnsFalse()
        {
            var src = CreateSrc();
            SetSize(src.Map, 5, 5);
            Assert.IsFalse(src.Map.IsInside(-1, 3));
        }

        [TestMethod]
        public void IsInside_NegativeY_ReturnsFalse()
        {
            var src = CreateSrc();
            SetSize(src.Map, 5, 5);
            Assert.IsFalse(src.Map.IsInside(3, -1));
        }

        [TestMethod]
        public void IsInside_XAtBoundary_ReturnsTrue()
        {
            var src = CreateSrc();
            SetSize(src.Map, 3, 3);
            Assert.IsTrue(src.Map.IsInside(3, 2));
        }

        [TestMethod]
        public void IsInside_YAtBoundary_ReturnsTrue()
        {
            var src = CreateSrc();
            SetSize(src.Map, 3, 3);
            Assert.IsTrue(src.Map.IsInside(2, 3));
        }

        // ──────────────────────────────────────────────
        // MapFileName / IsStatusView
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapFileName_CanBeSetAndRead()
        {
            var src = CreateSrc();
            src.Map.MapFileName = "scenario.map";
            Assert.AreEqual("scenario.map", src.Map.MapFileName);
        }

        [TestMethod]
        public void IsStatusView_NullFileName_IsTrue()
        {
            var src = CreateSrc();
            src.Map.MapFileName = null;
            Assert.IsTrue(src.Map.IsStatusView);
        }

        [TestMethod]
        public void IsStatusView_NonEmptyFileName_IsFalse()
        {
            var src = CreateSrc();
            src.Map.MapFileName = "map.dat";
            Assert.IsFalse(src.Map.IsStatusView);
        }
    }
}
