using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Maps;

namespace SRCCore.Maps.Tests
{
    /// <summary>
    /// BoxTypes・MapImageFileType enum のユニットテスト
    /// </summary>
    [TestClass]
    public class MapsEnumTests
    {
        // ──────────────────────────────────────────────
        // BoxTypes
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BoxTypes_Under_IsOne()
        {
            Assert.AreEqual(1, (int)BoxTypes.Under);
        }

        [TestMethod]
        public void BoxTypes_Upper_IsTwo()
        {
            Assert.AreEqual(2, (int)BoxTypes.Upper);
        }

        [TestMethod]
        public void BoxTypes_UpperDataOnly_IsThree()
        {
            Assert.AreEqual(3, (int)BoxTypes.UpperDataOnly);
        }

        [TestMethod]
        public void BoxTypes_UpperBmpOnly_IsFour()
        {
            Assert.AreEqual(4, (int)BoxTypes.UpperBmpOnly);
        }

        [TestMethod]
        public void BoxTypes_AllValuesAreDistinct()
        {
            var values = System.Enum.GetValues(typeof(BoxTypes));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (BoxTypes v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複した値が見つかりました: {v} = {(int)v}");
            }
        }

        [TestMethod]
        public void BoxTypes_CanBeParsedFromString()
        {
            Assert.AreEqual(BoxTypes.Under, System.Enum.Parse<BoxTypes>("Under"));
            Assert.AreEqual(BoxTypes.Upper, System.Enum.Parse<BoxTypes>("Upper"));
            Assert.AreEqual(BoxTypes.UpperDataOnly, System.Enum.Parse<BoxTypes>("UpperDataOnly"));
            Assert.AreEqual(BoxTypes.UpperBmpOnly, System.Enum.Parse<BoxTypes>("UpperBmpOnly"));
        }

        [TestMethod]
        public void BoxTypes_IsDefined_ForAllValues()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(BoxTypes), BoxTypes.Under));
            Assert.IsTrue(System.Enum.IsDefined(typeof(BoxTypes), BoxTypes.Upper));
            Assert.IsTrue(System.Enum.IsDefined(typeof(BoxTypes), BoxTypes.UpperDataOnly));
            Assert.IsTrue(System.Enum.IsDefined(typeof(BoxTypes), BoxTypes.UpperBmpOnly));
        }

        [TestMethod]
        public void BoxTypes_HasFourValues()
        {
            Assert.AreEqual(4, System.Enum.GetValues(typeof(BoxTypes)).Length);
        }

        // ──────────────────────────────────────────────
        // MapImageFileType
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapImageFileType_OldMapImageFileType_IsZero()
        {
            Assert.AreEqual(0, (int)MapImageFileType.OldMapImageFileType);
        }

        [TestMethod]
        public void MapImageFileType_FourFiguresMapImageFileType_IsOne()
        {
            Assert.AreEqual(1, (int)MapImageFileType.FourFiguresMapImageFileType);
        }

        [TestMethod]
        public void MapImageFileType_SeparateDirMapImageFileType_IsTwo()
        {
            Assert.AreEqual(2, (int)MapImageFileType.SeparateDirMapImageFileType);
        }

        [TestMethod]
        public void MapImageFileType_AllValuesAreDistinct()
        {
            var values = System.Enum.GetValues(typeof(MapImageFileType));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (MapImageFileType v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複した値が見つかりました: {v} = {(int)v}");
            }
        }

        [TestMethod]
        public void MapImageFileType_CanBeParsedFromString()
        {
            Assert.AreEqual(MapImageFileType.OldMapImageFileType,
                System.Enum.Parse<MapImageFileType>("OldMapImageFileType"));
            Assert.AreEqual(MapImageFileType.FourFiguresMapImageFileType,
                System.Enum.Parse<MapImageFileType>("FourFiguresMapImageFileType"));
            Assert.AreEqual(MapImageFileType.SeparateDirMapImageFileType,
                System.Enum.Parse<MapImageFileType>("SeparateDirMapImageFileType"));
        }

        [TestMethod]
        public void MapImageFileType_HasThreeValues()
        {
            Assert.AreEqual(3, System.Enum.GetValues(typeof(MapImageFileType)).Length);
        }

        [TestMethod]
        public void MapImageFileType_IsDefined_ForAllValues()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(MapImageFileType), MapImageFileType.OldMapImageFileType));
            Assert.IsTrue(System.Enum.IsDefined(typeof(MapImageFileType), MapImageFileType.FourFiguresMapImageFileType));
            Assert.IsTrue(System.Enum.IsDefined(typeof(MapImageFileType), MapImageFileType.SeparateDirMapImageFileType));
        }
    }
}
