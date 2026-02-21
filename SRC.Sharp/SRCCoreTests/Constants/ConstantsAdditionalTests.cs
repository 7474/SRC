using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;

namespace SRCCore.ConstantsTests
{
    /// <summary>
    /// Constants クラスの追加エッジケーステスト
    /// </summary>
    [TestClass]
    public class ConstantsAdditionalTests
    {
        // ──────────────────────────────────────────────
        // DIRECTIONS 配列テスト
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
        public void DIRECTIONS_FirstIsN()
        {
            Assert.AreEqual("N", SRCCore.Constants.DIRECTIONS[0]);
        }

        [TestMethod]
        public void DIRECTIONS_AllDistinct()
        {
            var set = new System.Collections.Generic.HashSet<string>();
            foreach (var d in SRCCore.Constants.DIRECTIONS)
            {
                Assert.IsTrue(set.Add(d), $"重複方向: {d}");
            }
        }

        // ──────────────────────────────────────────────
        // DEFAULT_LEVEL と他の値との比較テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DEFAULT_LEVEL_IsNegative()
        {
            Assert.IsTrue(SRCCore.Constants.DEFAULT_LEVEL < 0);
        }

        [TestMethod]
        public void DEFAULT_LEVEL_IsLessThanMinusHundred()
        {
            Assert.IsTrue(SRCCore.Constants.DEFAULT_LEVEL < -100);
        }

        [TestMethod]
        public void DEFAULT_LEVEL_IsConsistentAcrossAccesses()
        {
            // 複数回アクセスして同じ値が返る
            var v1 = SRCCore.Constants.DEFAULT_LEVEL;
            var v2 = SRCCore.Constants.DEFAULT_LEVEL;
            Assert.AreEqual(v1, v2);
        }

        // ──────────────────────────────────────────────
        // vbTab テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void vbTab_IsTabCharacter()
        {
            Assert.AreEqual("\t", SRCCore.Constants.vbTab);
        }

        [TestMethod]
        public void vbTab_LengthIsOne()
        {
            Assert.AreEqual(1, SRCCore.Constants.vbTab.Length);
        }

        // ──────────────────────────────────────────────
        // 文字列定数の内容テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void vbCr_And_vbLf_AreDifferent()
        {
            Assert.AreNotEqual(SRCCore.Constants.vbCr, SRCCore.Constants.vbLf);
        }

        [TestMethod]
        public void vbCr_And_vbTab_AreDifferent()
        {
            Assert.AreNotEqual(SRCCore.Constants.vbCr, SRCCore.Constants.vbTab);
        }

        [TestMethod]
        public void vbLf_And_vbTab_AreDifferent()
        {
            Assert.AreNotEqual(SRCCore.Constants.vbLf, SRCCore.Constants.vbTab);
        }
    }
}
