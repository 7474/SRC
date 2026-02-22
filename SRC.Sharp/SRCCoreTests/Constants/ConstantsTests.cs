using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;

namespace SRCCoreTests.Constants
{
    /// <summary>
    /// Constants クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class ConstantsTests
    {
        // ──────────────────────────────────────────────
        // DEFAULT_LEVEL
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DEFAULT_LEVEL_IsNegativeThousand()
        {
            Assert.AreEqual(-1000, SRCCore.Constants.DEFAULT_LEVEL);
        }

        [TestMethod]
        public void DEFAULT_LEVEL_IsLessThanZero()
        {
            Assert.IsTrue(SRCCore.Constants.DEFAULT_LEVEL < 0);
        }

        // ──────────────────────────────────────────────
        // vbCr
        // ──────────────────────────────────────────────

        [TestMethod]
        public void vbCr_IsCarriageReturn()
        {
            Assert.AreEqual("\r", SRCCore.Constants.vbCr);
        }

        [TestMethod]
        public void vbCr_LengthIsOne()
        {
            Assert.AreEqual(1, SRCCore.Constants.vbCr.Length);
        }

        // ──────────────────────────────────────────────
        // vbLf
        // ──────────────────────────────────────────────

        [TestMethod]
        public void vbLf_IsLineFeed()
        {
            Assert.AreEqual("\n", SRCCore.Constants.vbLf);
        }

        [TestMethod]
        public void vbLf_LengthIsOne()
        {
            Assert.AreEqual(1, SRCCore.Constants.vbLf.Length);
        }

        // ──────────────────────────────────────────────
        // vbTab
        // ──────────────────────────────────────────────

        [TestMethod]
        public void vbTab_IsTab()
        {
            Assert.AreEqual("\t", SRCCore.Constants.vbTab);
        }

        [TestMethod]
        public void vbTab_LengthIsOne()
        {
            Assert.AreEqual(1, SRCCore.Constants.vbTab.Length);
        }

        // ──────────────────────────────────────────────
        // DIRECTIONS
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DIRECTIONS_HasFourElements()
        {
            Assert.AreEqual(4, SRCCore.Constants.DIRECTIONS.Length);
        }

        [TestMethod]
        public void DIRECTIONS_ContainsNorth()
        {
            CollectionAssert.Contains(SRCCore.Constants.DIRECTIONS, "N");
        }

        [TestMethod]
        public void DIRECTIONS_ContainsSouth()
        {
            CollectionAssert.Contains(SRCCore.Constants.DIRECTIONS, "S");
        }

        [TestMethod]
        public void DIRECTIONS_ContainsWest()
        {
            CollectionAssert.Contains(SRCCore.Constants.DIRECTIONS, "W");
        }

        [TestMethod]
        public void DIRECTIONS_ContainsEast()
        {
            CollectionAssert.Contains(SRCCore.Constants.DIRECTIONS, "E");
        }

        [TestMethod]
        public void DIRECTIONS_IsNotNull()
        {
            Assert.IsNotNull(SRCCore.Constants.DIRECTIONS);
        }

        [TestMethod]
        public void DIRECTIONS_ElementsAreInCorrectOrder()
        {
            var expected = new string[] { "N", "S", "W", "E" };
            CollectionAssert.AreEqual(expected, SRCCore.Constants.DIRECTIONS);
        }

        // ──────────────────────────────────────────────
        // vbCr と vbLf は異なる
        // ──────────────────────────────────────────────

        [TestMethod]
        public void vbCr_DiffersFrom_vbLf()
        {
            Assert.AreNotEqual(SRCCore.Constants.vbCr, SRCCore.Constants.vbLf);
        }

        [TestMethod]
        public void vbCr_DiffersFrom_vbTab()
        {
            Assert.AreNotEqual(SRCCore.Constants.vbCr, SRCCore.Constants.vbTab);
        }

        [TestMethod]
        public void vbLf_DiffersFrom_vbTab()
        {
            Assert.AreNotEqual(SRCCore.Constants.vbLf, SRCCore.Constants.vbTab);
        }
    }
}
