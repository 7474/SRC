using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// SpecialPowerData クラスの追加カバレッジテスト
    /// </summary>
    [TestClass]
    public class SpecialPowerDataAdditionalTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // コンストラクタのデフォルト値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_KanaName_DefaultIsNull()
        {
            var sp = new SpecialPowerData(CreateSrc());
            Assert.IsNull(sp.KanaName);
        }

        [TestMethod]
        public void Constructor_ShortName_DefaultIsNull()
        {
            var sp = new SpecialPowerData(CreateSrc());
            Assert.IsNull(sp.ShortName);
        }

        [TestMethod]
        public void Constructor_TargetType_DefaultIsNull()
        {
            var sp = new SpecialPowerData(CreateSrc());
            Assert.IsNull(sp.TargetType);
        }

        [TestMethod]
        public void Constructor_Duration_DefaultIsNull()
        {
            var sp = new SpecialPowerData(CreateSrc());
            Assert.IsNull(sp.Duration);
        }

        [TestMethod]
        public void Constructor_NecessaryCondition_DefaultIsNull()
        {
            var sp = new SpecialPowerData(CreateSrc());
            Assert.IsNull(sp.NecessaryCondition);
        }

        [TestMethod]
        public void Constructor_Animation_DefaultIsNull()
        {
            var sp = new SpecialPowerData(CreateSrc());
            Assert.IsNull(sp.Animation);
        }

        [TestMethod]
        public void Constructor_Comment_DefaultIsNull()
        {
            var sp = new SpecialPowerData(CreateSrc());
            Assert.IsNull(sp.Comment);
        }

        // ──────────────────────────────────────────────
        // NecessaryCondition フィールド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NecessaryCondition_CanBeSetAndRead()
        {
            var sp = new SpecialPowerData(CreateSrc()) { NecessaryCondition = "気力120以上" };
            Assert.AreEqual("気力120以上", sp.NecessaryCondition);
        }

        [TestMethod]
        public void NecessaryCondition_EmptyString_CanBeSet()
        {
            var sp = new SpecialPowerData(CreateSrc()) { NecessaryCondition = "" };
            Assert.AreEqual("", sp.NecessaryCondition);
        }

        // ──────────────────────────────────────────────
        // Animation フィールド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Animation_CanBeSetAndRead()
        {
            var sp = new SpecialPowerData(CreateSrc()) { Animation = "flash.gif" };
            Assert.AreEqual("flash.gif", sp.Animation);
        }

        // ──────────────────────────────────────────────
        // SetEffect 詳細パース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetEffect_WithLevel_ParsesEffectLevel()
        {
            var sp = new SpecialPowerData(CreateSrc());
            sp.SetEffect("攻撃力上昇Lv2");
            Assert.AreEqual(1, sp.CountEffect());
            Assert.AreEqual("攻撃力上昇", sp.Effects[0].strEffectType);
            Assert.AreEqual(2d, sp.Effects[0].dblEffectLevel);
        }

        [TestMethod]
        public void SetEffect_WithData_ParsesEffectData()
        {
            var sp = new SpecialPowerData(CreateSrc());
            sp.SetEffect("付加=テストデータ");
            Assert.AreEqual(1, sp.CountEffect());
            Assert.AreEqual("付加", sp.Effects[0].strEffectType);
            Assert.IsNotNull(sp.Effects[0].strEffectData);
        }

        [TestMethod]
        public void SetEffect_WithLevelAndData_ParsesBoth()
        {
            var sp = new SpecialPowerData(CreateSrc());
            sp.SetEffect("効果Lv3=追加データ");
            Assert.AreEqual(1, sp.CountEffect());
            Assert.AreEqual("効果", sp.Effects[0].strEffectType);
            Assert.AreEqual(3d, sp.Effects[0].dblEffectLevel);
            Assert.IsNotNull(sp.Effects[0].strEffectData);
        }

        [TestMethod]
        public void SetEffect_MultipleEffects_ParsesAll()
        {
            var sp = new SpecialPowerData(CreateSrc());
            sp.SetEffect("必中 加速 ひらめき");
            Assert.AreEqual(3, sp.CountEffect());
            Assert.AreEqual("必中", sp.Effects[0].strEffectType);
            Assert.AreEqual("加速", sp.Effects[1].strEffectType);
            Assert.AreEqual("ひらめき", sp.Effects[2].strEffectType);
        }

        [TestMethod]
        public void SetEffect_SimpleEffect_DefaultLevel()
        {
            var sp = new SpecialPowerData(CreateSrc());
            sp.SetEffect("必中");
            Assert.AreEqual(Constants.DEFAULT_LEVEL, sp.Effects[0].dblEffectLevel);
        }

        [TestMethod]
        public void SetEffect_SimpleEffect_NullData()
        {
            var sp = new SpecialPowerData(CreateSrc());
            sp.SetEffect("必中");
            Assert.IsNull(sp.Effects[0].strEffectData);
        }

        // ──────────────────────────────────────────────
        // CountEffect 連続追加
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetEffect_CalledTwice_Accumulates()
        {
            var sp = new SpecialPowerData(CreateSrc());
            sp.SetEffect("必中");
            sp.SetEffect("加速");
            Assert.AreEqual(2, sp.CountEffect());
        }

        // ──────────────────────────────────────────────
        // SPConsumption 境界値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SPConsumption_Zero_CanBeSet()
        {
            var sp = new SpecialPowerData(CreateSrc()) { SPConsumption = 0 };
            Assert.AreEqual(0, sp.SPConsumption);
        }

        [TestMethod]
        public void SPConsumption_LargeValue_CanBeSet()
        {
            var sp = new SpecialPowerData(CreateSrc()) { SPConsumption = 999 };
            Assert.AreEqual(999, sp.SPConsumption);
        }

        // ──────────────────────────────────────────────
        // Effects リスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Effects_IsNotNull_AfterConstruction()
        {
            var sp = new SpecialPowerData(CreateSrc());
            Assert.IsNotNull(sp.Effects);
        }

        [TestMethod]
        public void Effects_ReflectsSetEffectAdditions()
        {
            var sp = new SpecialPowerData(CreateSrc());
            sp.SetEffect("熱血");
            Assert.AreEqual(1, sp.Effects.Count);
            Assert.AreEqual("熱血", sp.Effects[0].strEffectType);
        }
    }
}
