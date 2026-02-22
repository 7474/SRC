using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// TerrainData クラスの追加カバレッジテスト
    /// </summary>
    [TestClass]
    public class TerrainDataAdditionalTests
    {
        // ──────────────────────────────────────────────
        // コンストラクタのデフォルト値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_DefaultBitmap_IsNull()
        {
            var td = new TerrainData();
            Assert.IsNull(td.Bitmap);
        }

        [TestMethod]
        public void Constructor_DefaultClass_IsNull()
        {
            var td = new TerrainData();
            Assert.IsNull(td.Class);
        }

        [TestMethod]
        public void Constructor_ColFeature_IsNotNull()
        {
            var td = new TerrainData();
            Assert.IsNotNull(td.colFeature);
        }

        // ──────────────────────────────────────────────
        // AddFeature パース: データ付き (=)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddFeature_WithData_ParsesCorrectly()
        {
            var td = new TerrainData();
            td.AddFeature("地形効果=追加情報");
            var fd = td.Feature("地形効果");
            Assert.IsNotNull(fd);
            Assert.AreEqual("地形効果", fd.Name);
            Assert.AreEqual("追加情報", fd.StrData);
        }

        [TestMethod]
        public void AddFeature_WithLevelAndData_ParsesCorrectly()
        {
            var td = new TerrainData();
            td.AddFeature("回復Lv5=回復データ");
            var fd = td.Feature("回復");
            Assert.IsNotNull(fd);
            Assert.AreEqual(5d, fd.Level);
            Assert.AreEqual("回復データ", fd.StrData);
        }

        [TestMethod]
        public void AddFeature_SimpleNameOnly_SetsDefaultLevel()
        {
            var td = new TerrainData();
            td.AddFeature("移動停止");
            var fd = td.Feature("移動停止");
            Assert.IsNotNull(fd);
            Assert.AreEqual("移動停止", fd.Name);
            Assert.AreEqual(Constants.DEFAULT_LEVEL, fd.Level);
        }

        [TestMethod]
        public void AddFeature_DuplicateName_BothStored()
        {
            var td = new TerrainData();
            td.AddFeature("ＨＰ回復Lv1");
            td.AddFeature("ＨＰ回復Lv2");
            Assert.AreEqual(2, td.CountFeature());
        }

        // ──────────────────────────────────────────────
        // FeatureLevel デフォルトレベル
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FeatureLevel_DefaultLevel_ReturnsOne()
        {
            var td = new TerrainData();
            td.AddFeature("移動停止");
            Assert.AreEqual(1d, td.FeatureLevel("移動停止"));
        }

        // ──────────────────────────────────────────────
        // EffectForHPRecover / EffectForENRecover 具体値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EffectForHPRecover_WithLv5_Returns50()
        {
            var td = new TerrainData();
            td.AddFeature("ＨＰ回復Lv5");
            Assert.AreEqual(50, td.EffectForHPRecover());
        }

        [TestMethod]
        public void EffectForENRecover_WithLv3_Returns30()
        {
            var td = new TerrainData();
            td.AddFeature("ＥＮ回復Lv3");
            Assert.AreEqual(30, td.EffectForENRecover());
        }

        [TestMethod]
        public void EffectForHPRecover_DefaultLevel_Returns10()
        {
            var td = new TerrainData();
            td.AddFeature("ＨＰ回復");
            Assert.AreEqual(10, td.EffectForHPRecover());
        }

        [TestMethod]
        public void EffectForENRecover_DefaultLevel_Returns10()
        {
            var td = new TerrainData();
            td.AddFeature("ＥＮ回復");
            Assert.AreEqual(10, td.EffectForENRecover());
        }

        // ──────────────────────────────────────────────
        // DoNotEnter 両方の表記
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DoNotEnter_WithShinnyuuKinshi_ReturnsTrue()
        {
            var td = new TerrainData();
            td.AddFeature("侵入禁止");
            Assert.IsTrue(td.DoNotEnter());
        }

        [TestMethod]
        public void DoNotEnter_WithShinnyuuKinshi_NewSpelling_ReturnsTrue()
        {
            var td = new TerrainData();
            td.AddFeature("進入禁止");
            Assert.IsTrue(td.DoNotEnter());
        }

        // ──────────────────────────────────────────────
        // HasMoveStop with feature
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HasMoveStop_WithFeature_ReturnsTrue()
        {
            var td = new TerrainData();
            td.AddFeature("移動停止");
            Assert.IsTrue(td.HasMoveStop());
        }

        // ──────────────────────────────────────────────
        // IsFeatureAvailable after adding
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsFeatureAvailable_AfterAdd_ReturnsTrue()
        {
            var td = new TerrainData();
            td.AddFeature("特殊地形=データ");
            Assert.IsTrue(td.IsFeatureAvailable("特殊地形"));
        }

        [TestMethod]
        public void IsFeatureAvailable_NullName_ReturnsFalse()
        {
            var td = new TerrainData();
            Assert.IsFalse(td.IsFeatureAvailable("テスト"));
        }

        // ──────────────────────────────────────────────
        // EmptyTerrain フィールド確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EmptyTerrain_Name_IsNull()
        {
            Assert.IsNull(TerrainData.EmptyTerrain.Name);
        }

        [TestMethod]
        public void EmptyTerrain_HasNoFeatures()
        {
            Assert.AreEqual(0, TerrainData.EmptyTerrain.CountFeature());
        }

        // ──────────────────────────────────────────────
        // 境界値テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MoveCost_NegativeValue_CanBeSet()
        {
            var td = new TerrainData { MoveCost = -1 };
            Assert.AreEqual(-1, td.MoveCost);
        }

        [TestMethod]
        public void MoveCost_LargeValue_CanBeSet()
        {
            var td = new TerrainData { MoveCost = 9999 };
            Assert.AreEqual(9999, td.MoveCost);
        }

        [TestMethod]
        public void HitMod_LargePositive_CanBeSet()
        {
            var td = new TerrainData { HitMod = 100 };
            Assert.AreEqual(100, td.HitMod);
        }

        [TestMethod]
        public void DamageMod_LargeNegative_CanBeSet()
        {
            var td = new TerrainData { DamageMod = -100 };
            Assert.AreEqual(-100, td.DamageMod);
        }
    }
}
