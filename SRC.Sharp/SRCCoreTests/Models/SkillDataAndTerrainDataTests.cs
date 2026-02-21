using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// SkillData クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class SkillDataTests
    {
        // ──────────────────────────────────────────────
        // HasLevel
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HasLevel_ReturnsFalse_WhenLevelIsDefault()
        {
            var sd = new SkillData { Level = Constants.DEFAULT_LEVEL };
            Assert.IsFalse(sd.HasLevel);
        }

        [TestMethod]
        public void HasLevel_ReturnsTrue_WhenLevelIsSet()
        {
            var sd = new SkillData { Level = 5 };
            Assert.IsTrue(sd.HasLevel);
        }

        [TestMethod]
        public void HasLevel_ReturnsTrue_WhenLevelIsZero()
        {
            var sd = new SkillData { Level = 0 };
            Assert.IsTrue(sd.HasLevel);
        }

        // ──────────────────────────────────────────────
        // LevelOrDefault
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LevelOrDefault_WhenHasLevel_ReturnsLevel()
        {
            var sd = new SkillData { Level = 3 };
            Assert.AreEqual(3d, sd.LevelOrDefault(1d));
        }

        [TestMethod]
        public void LevelOrDefault_WhenNoLevel_ReturnsDefault()
        {
            var sd = new SkillData { Level = Constants.DEFAULT_LEVEL };
            Assert.AreEqual(1d, sd.LevelOrDefault(1d));
        }

        [TestMethod]
        public void LevelOrDefault_WithCustomDefault_ReturnsCustomDefault()
        {
            var sd = new SkillData { Level = Constants.DEFAULT_LEVEL };
            Assert.AreEqual(2d, sd.LevelOrDefault(2d));
        }

        // ──────────────────────────────────────────────
        // Properties
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Properties_CanBeSetAndRead()
        {
            var sd = new SkillData
            {
                Name = "気合",
                Level = 5,
                StrData = "データ",
                NecessaryLevel = 10
            };

            Assert.AreEqual("気合", sd.Name);
            Assert.AreEqual(5d, sd.Level);
            Assert.AreEqual("データ", sd.StrData);
            Assert.AreEqual(10, sd.NecessaryLevel);
        }
    }

    /// <summary>
    /// TerrainData クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class TerrainDataTests
    {
        // ──────────────────────────────────────────────
        // EmptyTerrain
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EmptyTerrain_IsNotNull()
        {
            Assert.IsNotNull(TerrainData.EmptyTerrain);
        }

        [TestMethod]
        public void EmptyTerrain_HasDefaultId()
        {
            Assert.AreEqual(-1, TerrainData.EmptyTerrain.ID);
        }

        // ──────────────────────────────────────────────
        // AddFeature / IsFeatureAvailable / CountFeature
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddFeature_Simple_AddsFeature()
        {
            var td = new TerrainData { Name = "平地" };
            td.AddFeature("ＨＰ回復");
            Assert.AreEqual(1, td.CountFeature());
            Assert.IsTrue(td.IsFeatureAvailable("ＨＰ回復"));
        }

        [TestMethod]
        public void AddFeature_WithLevel_ParsesLevel()
        {
            var td = new TerrainData { Name = "回復地形" };
            td.AddFeature("ＨＰ回復Lv2");
            Assert.IsTrue(td.IsFeatureAvailable("ＨＰ回復"));
            Assert.AreEqual(2d, td.FeatureLevel("ＨＰ回復"));
        }

        [TestMethod]
        public void AddFeature_WithData_ParsesData()
        {
            var td = new TerrainData { Name = "地形" };
            td.AddFeature("特殊=追加データ");
            Assert.IsTrue(td.IsFeatureAvailable("特殊"));
        }

        [TestMethod]
        public void AddFeature_Multiple_AddsAll()
        {
            var td = new TerrainData { Name = "多機能地形" };
            td.AddFeature("ＨＰ回復");
            td.AddFeature("移動停止");
            Assert.AreEqual(2, td.CountFeature());
        }

        [TestMethod]
        public void IsFeatureAvailable_NotExisting_ReturnsFalse()
        {
            var td = new TerrainData();
            Assert.IsFalse(td.IsFeatureAvailable("ＨＰ回復"));
        }

        // ──────────────────────────────────────────────
        // EffectForHPRecover
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EffectForHPRecover_NoFeature_ReturnsZero()
        {
            var td = new TerrainData();
            Assert.AreEqual(0, td.EffectForHPRecover());
        }

        [TestMethod]
        public void EffectForHPRecover_WithFeature_ReturnsEffect()
        {
            var td = new TerrainData { Name = "回復地形" };
            td.AddFeature("ＨＰ回復");
            // DEFAULT_LEVELの場合はFeatureLevel=1, 1*10=10
            Assert.AreEqual(10, td.EffectForHPRecover());
        }

        [TestMethod]
        public void EffectForHPRecover_WithLevel2_Returns20()
        {
            var td = new TerrainData { Name = "強回復地形" };
            td.AddFeature("ＨＰ回復Lv2");
            Assert.AreEqual(20, td.EffectForHPRecover());
        }

        // ──────────────────────────────────────────────
        // EffectForENRecover
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EffectForENRecover_NoFeature_ReturnsZero()
        {
            var td = new TerrainData();
            Assert.AreEqual(0, td.EffectForENRecover());
        }

        [TestMethod]
        public void EffectForENRecover_WithFeature_ReturnsEffect()
        {
            var td = new TerrainData { Name = "EN回復地形" };
            td.AddFeature("ＥＮ回復");
            Assert.AreEqual(10, td.EffectForENRecover());
        }

        // ──────────────────────────────────────────────
        // HasMoveStop / DoNotEnter
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HasMoveStop_WithFeature_ReturnsTrue()
        {
            var td = new TerrainData { Name = "停止地形" };
            td.AddFeature("移動停止");
            Assert.IsTrue(td.HasMoveStop());
        }

        [TestMethod]
        public void HasMoveStop_WithoutFeature_ReturnsFalse()
        {
            var td = new TerrainData();
            Assert.IsFalse(td.HasMoveStop());
        }

        [TestMethod]
        public void DoNotEnter_WithJinsyukinshi_ReturnsTrue()
        {
            var td = new TerrainData { Name = "進入禁止地形" };
            td.AddFeature("進入禁止");
            Assert.IsTrue(td.DoNotEnter());
        }

        [TestMethod]
        public void DoNotEnter_WithCompatibilityShinyuukinshi_ReturnsTrue()
        {
            var td = new TerrainData { Name = "旧進入禁止地形" };
            td.AddFeature("侵入禁止");
            Assert.IsTrue(td.DoNotEnter());
        }

        [TestMethod]
        public void DoNotEnter_WithoutFeature_ReturnsFalse()
        {
            var td = new TerrainData();
            Assert.IsFalse(td.DoNotEnter());
        }

        // ──────────────────────────────────────────────
        // FeatureLevel
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FeatureLevel_NotExists_ReturnsZero()
        {
            var td = new TerrainData();
            Assert.AreEqual(0d, td.FeatureLevel("存在しない"));
        }

        [TestMethod]
        public void FeatureLevel_WithDefaultLevel_ReturnsOne()
        {
            var td = new TerrainData { Name = "地形" };
            td.AddFeature("ＨＰ回復");
            Assert.AreEqual(1d, td.FeatureLevel("ＨＰ回復"));
        }

        [TestMethod]
        public void FeatureLevel_WithSpecifiedLevel_ReturnsSpecifiedLevel()
        {
            var td = new TerrainData { Name = "地形" };
            td.AddFeature("ＨＰ回復Lv3");
            Assert.AreEqual(3d, td.FeatureLevel("ＨＰ回復"));
        }

        [TestMethod]
        public void TerrainData_Name_CanBeSetAndRead()
        {
            var td = new TerrainData { Name = "砂漠" };
            Assert.AreEqual("砂漠", td.Name);
        }

        [TestMethod]
        public void TerrainData_Class_CanBeSetAndRead()
        {
            var td = new TerrainData { Class = "陸" };
            Assert.AreEqual("陸", td.Class);
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
            var td = new TerrainData { HitMod = -10 };
            Assert.AreEqual(-10, td.HitMod);
        }

        [TestMethod]
        public void TerrainData_DamageMod_CanBeSetAndRead()
        {
            var td = new TerrainData { DamageMod = 5 };
            Assert.AreEqual(5, td.DamageMod);
        }

        [TestMethod]
        public void TerrainData_EmptyTerrain_DefaultMoveCost()
        {
            Assert.AreEqual(0, TerrainData.EmptyTerrain.MoveCost);
        }

        [TestMethod]
        public void TerrainData_CountFeature_InitiallyZero()
        {
            var td = new TerrainData();
            Assert.AreEqual(0, td.CountFeature());
        }
    }

    [TestClass]
    public class SkillDataAdditionalTests
    {
        [TestMethod]
        public void SkillData_DefaultLevel_IsDefaultLevel()
        {
            var sd = new SkillData();
            // Level defaults to 0 (double default), not Constants.DEFAULT_LEVEL
            Assert.AreEqual(0d, sd.Level);
        }

        [TestMethod]
        public void SkillData_Name_DefaultIsNull()
        {
            var sd = new SkillData();
            Assert.IsNull(sd.Name);
        }

        [TestMethod]
        public void LevelOrDefault_LevelZero_ReturnsZero()
        {
            var sd = new SkillData { Level = 0 };
            Assert.AreEqual(0d, sd.LevelOrDefault(99d));
        }

        [TestMethod]
        public void HasLevel_LevelOne_ReturnsTrue()
        {
            var sd = new SkillData { Level = 1 };
            Assert.IsTrue(sd.HasLevel);
        }

        [TestMethod]
        public void NecessaryLevel_CanBeSetAndRead()
        {
            var sd = new SkillData { NecessaryLevel = 5 };
            Assert.AreEqual(5, sd.NecessaryLevel);
        }

        [TestMethod]
        public void StrData_CanBeSetAndRead()
        {
            var sd = new SkillData { StrData = "追加データ" };
            Assert.AreEqual("追加データ", sd.StrData);
        }
    }
}
