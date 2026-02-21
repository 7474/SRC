using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Maps;

namespace SRCCore.Maps.Tests
{
    /// <summary>
    /// BoxTypes / MapImageFileType enum の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class MapsAdditionalTests
    {
        // ──────────────────────────────────────────────
        // BoxTypes: int からのキャスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BoxTypes_CastFromInt_1_IsUnder()
        {
            Assert.AreEqual(BoxTypes.Under, (BoxTypes)1);
        }

        [TestMethod]
        public void BoxTypes_CastFromInt_2_IsUpper()
        {
            Assert.AreEqual(BoxTypes.Upper, (BoxTypes)2);
        }

        [TestMethod]
        public void BoxTypes_CastFromInt_3_IsUpperDataOnly()
        {
            Assert.AreEqual(BoxTypes.UpperDataOnly, (BoxTypes)3);
        }

        [TestMethod]
        public void BoxTypes_CastFromInt_4_IsUpperBmpOnly()
        {
            Assert.AreEqual(BoxTypes.UpperBmpOnly, (BoxTypes)4);
        }

        // ──────────────────────────────────────────────
        // BoxTypes: Enum.GetName
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BoxTypes_GetName_Under_ReturnsCorrectName()
        {
            Assert.AreEqual("Under", System.Enum.GetName(typeof(BoxTypes), BoxTypes.Under));
        }

        [TestMethod]
        public void BoxTypes_GetName_Upper_ReturnsCorrectName()
        {
            Assert.AreEqual("Upper", System.Enum.GetName(typeof(BoxTypes), BoxTypes.Upper));
        }

        [TestMethod]
        public void BoxTypes_GetName_UpperDataOnly_ReturnsCorrectName()
        {
            Assert.AreEqual("UpperDataOnly", System.Enum.GetName(typeof(BoxTypes), BoxTypes.UpperDataOnly));
        }

        [TestMethod]
        public void BoxTypes_GetName_UpperBmpOnly_ReturnsCorrectName()
        {
            Assert.AreEqual("UpperBmpOnly", System.Enum.GetName(typeof(BoxTypes), BoxTypes.UpperBmpOnly));
        }

        // ──────────────────────────────────────────────
        // BoxTypes: 値の比較
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BoxTypes_Under_IsLessThan_Upper()
        {
            Assert.IsTrue((int)BoxTypes.Under < (int)BoxTypes.Upper);
        }

        [TestMethod]
        public void BoxTypes_Upper_IsLessThan_UpperDataOnly()
        {
            Assert.IsTrue((int)BoxTypes.Upper < (int)BoxTypes.UpperDataOnly);
        }

        [TestMethod]
        public void BoxTypes_UpperDataOnly_IsLessThan_UpperBmpOnly()
        {
            Assert.IsTrue((int)BoxTypes.UpperDataOnly < (int)BoxTypes.UpperBmpOnly);
        }

        // ──────────────────────────────────────────────
        // MapImageFileType: int からのキャスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapImageFileType_CastFromInt_0_IsOldMapImageFileType()
        {
            Assert.AreEqual(MapImageFileType.OldMapImageFileType, (MapImageFileType)0);
        }

        [TestMethod]
        public void MapImageFileType_CastFromInt_1_IsFourFigures()
        {
            Assert.AreEqual(MapImageFileType.FourFiguresMapImageFileType, (MapImageFileType)1);
        }

        [TestMethod]
        public void MapImageFileType_CastFromInt_2_IsSeparateDir()
        {
            Assert.AreEqual(MapImageFileType.SeparateDirMapImageFileType, (MapImageFileType)2);
        }

        // ──────────────────────────────────────────────
        // MapImageFileType: Enum.GetName
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapImageFileType_GetName_OldMapImageFileType_ReturnsCorrectName()
        {
            Assert.AreEqual("OldMapImageFileType",
                System.Enum.GetName(typeof(MapImageFileType), MapImageFileType.OldMapImageFileType));
        }

        [TestMethod]
        public void MapImageFileType_GetName_FourFigures_ReturnsCorrectName()
        {
            Assert.AreEqual("FourFiguresMapImageFileType",
                System.Enum.GetName(typeof(MapImageFileType), MapImageFileType.FourFiguresMapImageFileType));
        }

        [TestMethod]
        public void MapImageFileType_GetName_SeparateDir_ReturnsCorrectName()
        {
            Assert.AreEqual("SeparateDirMapImageFileType",
                System.Enum.GetName(typeof(MapImageFileType), MapImageFileType.SeparateDirMapImageFileType));
        }

        // ──────────────────────────────────────────────
        // MapImageFileType: 値の比較
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapImageFileType_OldMapImageFileType_IsLessThan_FourFigures()
        {
            Assert.IsTrue((int)MapImageFileType.OldMapImageFileType < (int)MapImageFileType.FourFiguresMapImageFileType);
        }

        [TestMethod]
        public void MapImageFileType_FourFigures_IsLessThan_SeparateDir()
        {
            Assert.IsTrue((int)MapImageFileType.FourFiguresMapImageFileType < (int)MapImageFileType.SeparateDirMapImageFileType);
        }
    }
}
