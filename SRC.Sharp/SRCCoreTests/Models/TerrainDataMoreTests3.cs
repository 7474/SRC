using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    [TestClass]
    public class TerrainDataMoreTests3
    {
        [TestMethod]
        public void Name_Sky_CanBeSetAndRead()
        {
            var td = new TerrainData { Name = "空" };
            Assert.AreEqual("空", td.Name);
        }

        [TestMethod]
        public void Name_Ground_CanBeSetAndRead()
        {
            var td = new TerrainData { Name = "地" };
            Assert.AreEqual("地", td.Name);
        }

        [TestMethod]
        public void Name_Sea_CanBeSetAndRead()
        {
            var td = new TerrainData { Name = "海" };
            Assert.AreEqual("海", td.Name);
        }

        [TestMethod]
        public void ID_CanBeSetAndRead()
        {
            var td = new TerrainData { ID = 42 };
            Assert.AreEqual(42, td.ID);
        }

        [TestMethod]
        public void Bitmap_CanBeSetAndRead()
        {
            var td = new TerrainData { Bitmap = "plains.bmp" };
            Assert.AreEqual("plains.bmp", td.Bitmap);
        }

        [TestMethod]
        public void Class_CanBeSetAndRead()
        {
            var td = new TerrainData { Class = "地" };
            Assert.AreEqual("地", td.Class);
        }

        [TestMethod]
        public void MoveCost_Zero_CanBeSetAndRead()
        {
            var td = new TerrainData { MoveCost = 0 };
            Assert.AreEqual(0, td.MoveCost);
        }

        [TestMethod]
        public void HitMod_Negative_CanBeSetAndRead()
        {
            var td = new TerrainData { HitMod = -20 };
            Assert.AreEqual(-20, td.HitMod);
        }

        [TestMethod]
        public void DamageMod_Positive_CanBeSetAndRead()
        {
            var td = new TerrainData { DamageMod = 15 };
            Assert.AreEqual(15, td.DamageMod);
        }

        [TestMethod]
        public void EffectForHPRecover_NoFeature_ReturnsZero()
        {
            var td = new TerrainData();
            Assert.AreEqual(0, td.EffectForHPRecover());
        }

        [TestMethod]
        public void EffectForHPRecover_WithFeatureLv2_Returns20()
        {
            var td = new TerrainData();
            td.AddFeature("ＨＰ回復Lv2");
            Assert.AreEqual(20, td.EffectForHPRecover());
        }

        [TestMethod]
        public void EffectForENRecover_NoFeature_ReturnsZero()
        {
            var td = new TerrainData();
            Assert.AreEqual(0, td.EffectForENRecover());
        }

        [TestMethod]
        public void EffectForENRecover_WithFeatureLv1_Returns10()
        {
            var td = new TerrainData();
            td.AddFeature("ＥＮ回復Lv1");
            Assert.AreEqual(10, td.EffectForENRecover());
        }

        [TestMethod]
        public void DoNotEnter_LegacyFeature_ReturnsTrue()
        {
            var td = new TerrainData();
            td.AddFeature("侵入禁止");
            Assert.IsTrue(td.DoNotEnter());
        }

        [TestMethod]
        public void AddFeature_MultipleFeatures_AllStored()
        {
            var td = new TerrainData();
            td.AddFeature("ＨＰ回復Lv3");
            td.AddFeature("ＥＮ回復Lv1");
            td.AddFeature("移動停止");
            Assert.AreEqual(3, td.CountFeature());
        }

        [TestMethod]
        public void Feature_ReturnsFeatureByKey()
        {
            var td = new TerrainData();
            td.AddFeature("ＨＰ回復Lv5");
            var f = td.Feature("ＨＰ回復");
            Assert.IsNotNull(f);
            Assert.AreEqual("ＨＰ回復", f.Name);
        }

        [TestMethod]
        public void EmptyTerrain_ID_IsMinusOne()
        {
            Assert.AreEqual(-1, TerrainData.EmptyTerrain.ID);
        }
    }
}
