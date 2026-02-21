using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// AbilityEffect / SpecialPowerEffect の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class EffectAndSpecialPowerTests
    {
        // ──────────────────────────────────────────────
        // AbilityEffect 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AbilityEffect_DefaultValues_NameIsNull()
        {
            var ae = new AbilityEffect();
            Assert.IsNull(ae.Name);
        }

        [TestMethod]
        public void AbilityEffect_DefaultValues_LevelIsZero()
        {
            var ae = new AbilityEffect();
            Assert.AreEqual(0d, ae.Level);
        }

        [TestMethod]
        public void AbilityEffect_DefaultValues_DataIsNull()
        {
            var ae = new AbilityEffect();
            Assert.IsNull(ae.Data);
        }

        [TestMethod]
        public void AbilityEffect_EffectType_WhenNameIsNull_ReturnsNull()
        {
            var ae = new AbilityEffect { Name = null };
            Assert.IsNull(ae.EffectType);
        }

        [TestMethod]
        public void AbilityEffect_EffectType_AlwaysMatchesName()
        {
            var ae = new AbilityEffect { Name = "HPアップ" };
            ae.Name = "ENアップ";
            // EffectType は Name と同一参照
            Assert.AreEqual("ENアップ", ae.EffectType);
        }

        [TestMethod]
        public void AbilityEffect_Level_CanBeNegative()
        {
            var ae = new AbilityEffect { Level = -5d };
            Assert.AreEqual(-5d, ae.Level);
        }

        [TestMethod]
        public void AbilityEffect_TwoInstances_AreIndependent()
        {
            var ae1 = new AbilityEffect { Name = "A", Level = 1d, Data = "d1" };
            var ae2 = new AbilityEffect { Name = "B", Level = 2d, Data = "d2" };
            Assert.AreNotEqual(ae1.Name, ae2.Name);
            Assert.AreNotEqual(ae1.Level, ae2.Level);
        }

        // ──────────────────────────────────────────────
        // SpecialPowerEffect 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SpecialPowerEffect_ZeroLevel_CanBeSetAndRead()
        {
            var eff = new SpecialPowerEffect { dblEffectLevel = 0d };
            Assert.AreEqual(0d, eff.dblEffectLevel);
        }

        [TestMethod]
        public void SpecialPowerEffect_NegativeLevel_CanBeSetAndRead()
        {
            var eff = new SpecialPowerEffect { dblEffectLevel = -2d };
            Assert.AreEqual(-2d, eff.dblEffectLevel);
        }

        [TestMethod]
        public void SpecialPowerEffect_EmptyStrings_CanBeSetAndRead()
        {
            var eff = new SpecialPowerEffect
            {
                strEffectType = "",
                strEffectData = ""
            };
            Assert.AreEqual("", eff.strEffectType);
            Assert.AreEqual("", eff.strEffectData);
        }

        [TestMethod]
        public void SpecialPowerEffect_TwoInstances_AreIndependent()
        {
            var e1 = new SpecialPowerEffect { strEffectType = "X", dblEffectLevel = 1d };
            var e2 = new SpecialPowerEffect { strEffectType = "Y", dblEffectLevel = 2d };
            Assert.AreNotSame(e1, e2);
            Assert.AreNotEqual(e1.strEffectType, e2.strEffectType);
        }

        [TestMethod]
        public void SpecialPowerEffect_OverwriteValues_ReturnsNewValues()
        {
            var eff = new SpecialPowerEffect { strEffectType = "初期値", dblEffectLevel = 1d };
            eff.strEffectType = "新値";
            eff.dblEffectLevel = 99d;
            Assert.AreEqual("新値", eff.strEffectType);
            Assert.AreEqual(99d, eff.dblEffectLevel);
        }
    }
}
