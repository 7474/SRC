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
        public void EffectForRecover_FractionalLevel_RoundsToEven()
        {
            // VB: Map.bas TerrainEffectForHPRecover / TerrainEffectForENRecover は
            // As Integer の関数なので 10 * Lv を偶数丸めで返す
            var td = new TerrainData();
            td.AddFeature("ＨＰ回復Lv0.36");
            td.AddFeature("ＥＮ回復Lv0.25");
            // 3.6 → 4（切り捨てなら 3）
            Assert.AreEqual(4, td.EffectForHPRecover());
            // 2.5 → 2（偶数丸め）
            Assert.AreEqual(2, td.EffectForENRecover());
        }

        [TestMethod]
        public void Load_FractionalMoveCost_RoundsDoubledCostToEven()
        {
            // VB: TerrainDataList.cls Load の .MoveCost = CInt(2 * CDbl(buf2))。
            // 1.3 → 2.6 → 3（切り捨てなら 2）、1.25 → 2.5 → 2（偶数丸め）
            var src = new SRC { GUI = new SRCCore.TestLib.MockGUI() };
            var text = "0\r\n平地, plain\r\n陸, 1.3, 0, 0\r\n\r\n" +
                "1\r\n草地, grass\r\n陸, 1.25, 0, 0\r\n\r\n";
            src.TDList.Load("terrain.txt", new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(text)));
            Assert.AreEqual(3, src.TDList.Item(0).MoveCost);
            Assert.AreEqual(2, src.TDList.Item(1).MoveCost);
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
