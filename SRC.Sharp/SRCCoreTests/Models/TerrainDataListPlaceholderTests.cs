using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// TerrainDataList.DefinePlaceholder のユニットテスト
    /// </summary>
    [TestClass]
    public class TerrainDataListPlaceholderTests
    {
        [TestMethod]
        public void DefinePlaceholder_UndefinedId_DefinesLandTerrain()
        {
            var src = new SRC { GUI = new MockGUI() };
            Assert.IsFalse(src.TDList.IsDefined(5));

            var td = src.TDList.DefinePlaceholder(5);

            Assert.IsTrue(src.TDList.IsDefined(5));
            Assert.AreSame(td, src.TDList.Item(5));
            Assert.AreEqual(5, td.ID);
            Assert.AreEqual("陸", td.Class);
            Assert.AreEqual(1, td.MoveCost);
            Assert.AreEqual(0, td.HitMod);
            Assert.AreEqual(0, td.DamageMod);
        }

        [TestMethod]
        public void DefinePlaceholder_DefinedId_ReturnsExisting()
        {
            var src = new SRC { GUI = new MockGUI() };
            var first = src.TDList.DefinePlaceholder(7);
            var second = src.TDList.DefinePlaceholder(7);
            Assert.AreSame(first, second);
        }
    }
}
