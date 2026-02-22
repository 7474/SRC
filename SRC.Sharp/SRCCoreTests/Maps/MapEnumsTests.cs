using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Maps;

namespace SRCCore.Maps.Tests
{
    /// <summary>
    /// BoxTypes / MapImageFileType enum のユニットテスト
    /// </summary>
    [TestClass]
    public class MapEnumsTests
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
        public void BoxTypes_HasFourValues()
        {
            Assert.AreEqual(4, System.Enum.GetValues(typeof(BoxTypes)).Length);
        }

        [TestMethod]
        public void BoxTypes_IsDefined_ValidValues()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(BoxTypes), 1));
            Assert.IsTrue(System.Enum.IsDefined(typeof(BoxTypes), 2));
            Assert.IsTrue(System.Enum.IsDefined(typeof(BoxTypes), 3));
            Assert.IsTrue(System.Enum.IsDefined(typeof(BoxTypes), 4));
            Assert.IsFalse(System.Enum.IsDefined(typeof(BoxTypes), 0));
            Assert.IsFalse(System.Enum.IsDefined(typeof(BoxTypes), 5));
        }

        [TestMethod]
        public void BoxTypes_Parse_ReturnsCorrectValues()
        {
            Assert.AreEqual(BoxTypes.Under, (BoxTypes)System.Enum.Parse(typeof(BoxTypes), "Under"));
            Assert.AreEqual(BoxTypes.Upper, (BoxTypes)System.Enum.Parse(typeof(BoxTypes), "Upper"));
            Assert.AreEqual(BoxTypes.UpperDataOnly, (BoxTypes)System.Enum.Parse(typeof(BoxTypes), "UpperDataOnly"));
            Assert.AreEqual(BoxTypes.UpperBmpOnly, (BoxTypes)System.Enum.Parse(typeof(BoxTypes), "UpperBmpOnly"));
        }

        [TestMethod]
        public void BoxTypes_ValuesAreUnique()
        {
            var values = System.Enum.GetValues(typeof(BoxTypes));
            var distinct = new System.Collections.Generic.HashSet<int>();
            foreach (var v in values) distinct.Add((int)v);
            Assert.AreEqual(values.Length, distinct.Count);
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
        public void MapImageFileType_HasThreeValues()
        {
            Assert.AreEqual(3, System.Enum.GetValues(typeof(MapImageFileType)).Length);
        }

        [TestMethod]
        public void MapImageFileType_IsDefined_ValidValues()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(MapImageFileType), 0));
            Assert.IsTrue(System.Enum.IsDefined(typeof(MapImageFileType), 1));
            Assert.IsTrue(System.Enum.IsDefined(typeof(MapImageFileType), 2));
            Assert.IsFalse(System.Enum.IsDefined(typeof(MapImageFileType), -1));
            Assert.IsFalse(System.Enum.IsDefined(typeof(MapImageFileType), 3));
        }

        [TestMethod]
        public void MapImageFileType_Parse_ReturnsCorrectValues()
        {
            Assert.AreEqual(MapImageFileType.OldMapImageFileType, (MapImageFileType)System.Enum.Parse(typeof(MapImageFileType), "OldMapImageFileType"));
            Assert.AreEqual(MapImageFileType.FourFiguresMapImageFileType, (MapImageFileType)System.Enum.Parse(typeof(MapImageFileType), "FourFiguresMapImageFileType"));
            Assert.AreEqual(MapImageFileType.SeparateDirMapImageFileType, (MapImageFileType)System.Enum.Parse(typeof(MapImageFileType), "SeparateDirMapImageFileType"));
        }

        [TestMethod]
        public void MapImageFileType_ValuesAreUnique()
        {
            var values = System.Enum.GetValues(typeof(MapImageFileType));
            var distinct = new System.Collections.Generic.HashSet<int>();
            foreach (var v in values) distinct.Add((int)v);
            Assert.AreEqual(values.Length, distinct.Count);
        }
    }
}
