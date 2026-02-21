using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Maps;

namespace SRCCore.Maps.Tests
{
    /// <summary>
    /// MapImageFileType enum のユニットテスト
    /// </summary>
    [TestClass]
    public class MapImageFileTypeEnumTests
    {
        // ──────────────────────────────────────────────
        // 各値の IsDefined
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsDefined_OldMapImageFileType_ReturnsTrue()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(MapImageFileType), MapImageFileType.OldMapImageFileType));
        }

        [TestMethod]
        public void IsDefined_FourFiguresMapImageFileType_ReturnsTrue()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(MapImageFileType), MapImageFileType.FourFiguresMapImageFileType));
        }

        [TestMethod]
        public void IsDefined_SeparateDirMapImageFileType_ReturnsTrue()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(MapImageFileType), MapImageFileType.SeparateDirMapImageFileType));
        }

        // ──────────────────────────────────────────────
        // 数値確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void OldMapImageFileType_IsZero()
        {
            Assert.AreEqual(0, (int)MapImageFileType.OldMapImageFileType);
        }

        [TestMethod]
        public void FourFiguresMapImageFileType_IsOne()
        {
            Assert.AreEqual(1, (int)MapImageFileType.FourFiguresMapImageFileType);
        }

        [TestMethod]
        public void SeparateDirMapImageFileType_IsTwo()
        {
            Assert.AreEqual(2, (int)MapImageFileType.SeparateDirMapImageFileType);
        }

        // ──────────────────────────────────────────────
        // 全値が異なる
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AllValues_AreDistinct()
        {
            var values = System.Enum.GetValues(typeof(MapImageFileType));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (MapImageFileType v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複: {v}");
            }
        }

        // ──────────────────────────────────────────────
        // 文字列からのパース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ParseFromString_OldMapImageFileType()
        {
            Assert.AreEqual(MapImageFileType.OldMapImageFileType,
                System.Enum.Parse<MapImageFileType>("OldMapImageFileType"));
        }

        [TestMethod]
        public void ParseFromString_FourFiguresMapImageFileType()
        {
            Assert.AreEqual(MapImageFileType.FourFiguresMapImageFileType,
                System.Enum.Parse<MapImageFileType>("FourFiguresMapImageFileType"));
        }

        [TestMethod]
        public void ParseFromString_SeparateDirMapImageFileType()
        {
            Assert.AreEqual(MapImageFileType.SeparateDirMapImageFileType,
                System.Enum.Parse<MapImageFileType>("SeparateDirMapImageFileType"));
        }
    }
}
