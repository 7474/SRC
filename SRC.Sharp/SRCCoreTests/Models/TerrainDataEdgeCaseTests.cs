using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// TerrainData クラスの追加エッジケーステスト
    /// </summary>
    [TestClass]
    public class TerrainDataEdgeCaseTests
    {
        // ──────────────────────────────────────────────
        // デフォルト値テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TerrainData_DefaultID_IsMinusOne()
        {
            var td = new TerrainData();
            Assert.AreEqual(-1, td.ID);
        }

        [TestMethod]
        public void TerrainData_DefaultName_IsNull()
        {
            var td = new TerrainData();
            Assert.IsNull(td.Name);
        }

        [TestMethod]
        public void TerrainData_DefaultMoveCost_IsZero()
        {
            var td = new TerrainData();
            Assert.AreEqual(0, td.MoveCost);
        }

        [TestMethod]
        public void TerrainData_DefaultHitMod_IsZero()
        {
            var td = new TerrainData();
            Assert.AreEqual(0, td.HitMod);
        }

        [TestMethod]
        public void TerrainData_DefaultDamageMod_IsZero()
        {
            var td = new TerrainData();
            Assert.AreEqual(0, td.DamageMod);
        }

        // ──────────────────────────────────────────────
        // フィールドの設定・読み取り
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TerrainData_Name_CanBeSetAndRead()
        {
            var td = new TerrainData { Name = "平地" };
            Assert.AreEqual("平地", td.Name);
        }

        [TestMethod]
        public void TerrainData_ID_CanBeSetAndRead()
        {
            var td = new TerrainData { ID = 5 };
            Assert.AreEqual(5, td.ID);
        }

        [TestMethod]
        public void TerrainData_MoveCost_CanBeSetAndRead()
        {
            var td = new TerrainData { MoveCost = 3 };
            Assert.AreEqual(3, td.MoveCost);
        }

        [TestMethod]
        public void TerrainData_HitMod_CanBeSetAndRead()
        {
            var td = new TerrainData { HitMod = 15 };
            Assert.AreEqual(15, td.HitMod);
        }

        [TestMethod]
        public void TerrainData_DamageMod_CanBeSetAndRead()
        {
            var td = new TerrainData { DamageMod = -10 };
            Assert.AreEqual(-10, td.DamageMod);
        }

        [TestMethod]
        public void TerrainData_Class_CanBeSetAndRead()
        {
            var td = new TerrainData { Class = "陸" };
            Assert.AreEqual("陸", td.Class);
        }

        [TestMethod]
        public void TerrainData_Bitmap_CanBeSetAndRead()
        {
            var td = new TerrainData { Bitmap = "plain.bmp" };
            Assert.AreEqual("plain.bmp", td.Bitmap);
        }

        // ──────────────────────────────────────────────
        // EmptyTerrain 定数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EmptyTerrain_IsNotNull()
        {
            Assert.IsNotNull(TerrainData.EmptyTerrain);
        }

        [TestMethod]
        public void EmptyTerrain_IsSameInstanceEachTime()
        {
            Assert.AreSame(TerrainData.EmptyTerrain, TerrainData.EmptyTerrain);
        }

        [TestMethod]
        public void EmptyTerrain_ID_IsMinusOne()
        {
            Assert.AreEqual(-1, TerrainData.EmptyTerrain.ID);
        }

        // ──────────────────────────────────────────────
        // FeatureCount
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TerrainData_NewInstance_HasNoFeatures()
        {
            var td = new TerrainData();
            Assert.AreEqual(0, td.CountFeature());
        }

        // ──────────────────────────────────────────────
        // IsFeatureAvailable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsFeatureAvailable_NonExistentFeature_ReturnsFalse()
        {
            var td = new TerrainData { Name = "草原" };
            Assert.IsFalse(td.IsFeatureAvailable("非存在特性"));
        }

        // ──────────────────────────────────────────────
        // DoNotEnter
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DoNotEnter_NoFeature_ReturnsFalse()
        {
            var td = new TerrainData { Name = "平地" };
            Assert.IsFalse(td.DoNotEnter());
        }

        // ──────────────────────────────────────────────
        // EffectForHPRecover / EffectForENRecover
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EffectForHPRecover_NoFeature_ReturnsZero()
        {
            var td = new TerrainData();
            Assert.AreEqual(0, td.EffectForHPRecover());
        }

        [TestMethod]
        public void EffectForENRecover_NoFeature_ReturnsZero()
        {
            var td = new TerrainData();
            Assert.AreEqual(0, td.EffectForENRecover());
        }

        // ──────────────────────────────────────────────
        // HasMoveStop
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HasMoveStop_NoFeature_ReturnsFalse()
        {
            var td = new TerrainData { Name = "砂漠" };
            Assert.IsFalse(td.HasMoveStop());
        }
    }
}
