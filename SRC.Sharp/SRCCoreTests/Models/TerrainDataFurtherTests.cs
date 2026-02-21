using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// TerrainData クラスのさらなるユニットテスト
    /// </summary>
    [TestClass]
    public class TerrainDataFurtherTests
    {
        // ──────────────────────────────────────────────
        // プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var td = new TerrainData { Name = "平地" };
            Assert.AreEqual("平地", td.Name);
        }

        [TestMethod]
        public void MoveCost_CanBeSetAndRead()
        {
            var td = new TerrainData { MoveCost = 2 };
            Assert.AreEqual(2, td.MoveCost);
        }

        [TestMethod]
        public void HitMod_CanBeSetAndRead()
        {
            var td = new TerrainData { HitMod = 10 };
            Assert.AreEqual(10, td.HitMod);
        }

        [TestMethod]
        public void DamageMod_CanBeSetAndRead()
        {
            var td = new TerrainData { DamageMod = -5 };
            Assert.AreEqual(-5, td.DamageMod);
        }

        [TestMethod]
        public void DefaultValues_AreDefault()
        {
            var td = new TerrainData();
            Assert.IsNull(td.Name);
            Assert.AreEqual(0, td.MoveCost);
            Assert.AreEqual(0, td.HitMod);
            Assert.AreEqual(0, td.DamageMod);
            Assert.AreEqual(-1, td.ID);
        }

        [TestMethod]
        public void EmptyTerrain_StaticInstance_IsNotNull()
        {
            Assert.IsNotNull(TerrainData.EmptyTerrain);
        }

        [TestMethod]
        public void Name_EmptyString_IsAllowed()
        {
            var td = new TerrainData { Name = "" };
            Assert.AreEqual("", td.Name);
        }

        [TestMethod]
        public void TwoInstances_AreIndependent()
        {
            var td1 = new TerrainData { Name = "平地", MoveCost = 1 };
            var td2 = new TerrainData { Name = "山岳", MoveCost = 3 };
            Assert.AreNotEqual(td1.Name, td2.Name);
            Assert.AreNotEqual(td1.MoveCost, td2.MoveCost);
        }

        // ──────────────────────────────────────────────
        // IsFeatureAvailable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsFeatureAvailable_NoFeatures_ReturnsFalse()
        {
            var td = new TerrainData();
            Assert.IsFalse(td.IsFeatureAvailable("ＨＰ回復"));
        }

        [TestMethod]
        public void IsFeatureAvailable_WithMatchingFeature_ReturnsTrue()
        {
            var td = new TerrainData();
            td.AddFeature("ＨＰ回復Lv3");
            Assert.IsTrue(td.IsFeatureAvailable("ＨＰ回復"));
        }

        // ──────────────────────────────────────────────
        // CountFeature
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CountFeature_NoFeatures_ReturnsZero()
        {
            var td = new TerrainData();
            Assert.AreEqual(0, td.CountFeature());
        }

        [TestMethod]
        public void CountFeature_OneFeature_ReturnsOne()
        {
            var td = new TerrainData();
            td.AddFeature("移動停止");
            Assert.AreEqual(1, td.CountFeature());
        }

        [TestMethod]
        public void CountFeature_TwoFeatures_ReturnsTwo()
        {
            var td = new TerrainData();
            td.AddFeature("ＨＰ回復");
            td.AddFeature("移動停止");
            Assert.AreEqual(2, td.CountFeature());
        }

        // ──────────────────────────────────────────────
        // FeatureLevel
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FeatureLevel_NoFeature_ReturnsZero()
        {
            var td = new TerrainData();
            Assert.AreEqual(0d, td.FeatureLevel("ＨＰ回復"), 1e-10);
        }

        [TestMethod]
        public void FeatureLevel_WithFeature_ReturnsLevel()
        {
            var td = new TerrainData();
            td.AddFeature("ＨＰ回復Lv3");
            Assert.AreEqual(3d, td.FeatureLevel("ＨＰ回復"), 1e-10);
        }

        // ──────────────────────────────────────────────
        // HasMoveStop / DoNotEnter
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HasMoveStop_NoFeature_ReturnsFalse()
        {
            var td = new TerrainData();
            Assert.IsFalse(td.HasMoveStop());
        }

        [TestMethod]
        public void HasMoveStop_WithFeature_ReturnsTrue()
        {
            var td = new TerrainData();
            td.AddFeature("移動停止");
            Assert.IsTrue(td.HasMoveStop());
        }

        [TestMethod]
        public void DoNotEnter_NoFeature_ReturnsFalse()
        {
            var td = new TerrainData();
            Assert.IsFalse(td.DoNotEnter());
        }

        [TestMethod]
        public void DoNotEnter_WithFeature_ReturnsTrue()
        {
            var td = new TerrainData();
            td.AddFeature("進入禁止");
            Assert.IsTrue(td.DoNotEnter());
        }
    }
}
