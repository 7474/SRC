using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// SpecialPowerEffect クラスの追加ユニットテスト (SpecialPowerEffectMoreTests3)
    /// </summary>
    [TestClass]
    public class SpecialPowerEffectMoreTests3
    {
        [TestMethod]
        public void EffectLevel_Zero_IsAllowed()
        {
            var effect = new SpecialPowerEffect { dblEffectLevel = 0 };
            Assert.AreEqual(0.0, effect.dblEffectLevel);
        }

        [TestMethod]
        public void EffectLevel_Negative_IsAllowed()
        {
            var effect = new SpecialPowerEffect { dblEffectLevel = -3.5 };
            Assert.AreEqual(-3.5, effect.dblEffectLevel, 1e-10);
        }

        [TestMethod]
        public void EffectLevel_VeryLarge_IsAllowed()
        {
            var effect = new SpecialPowerEffect { dblEffectLevel = 1e12 };
            Assert.AreEqual(1e12, effect.dblEffectLevel, 1.0);
        }

        [TestMethod]
        public void EffectType_CanBeUpdated()
        {
            var effect = new SpecialPowerEffect { strEffectType = "初期" };
            effect.strEffectType = "更新後";
            Assert.AreEqual("更新後", effect.strEffectType);
        }

        [TestMethod]
        public void EffectData_CanBeUpdated()
        {
            var effect = new SpecialPowerEffect { strEffectData = "古いデータ" };
            effect.strEffectData = "新しいデータ";
            Assert.AreEqual("新しいデータ", effect.strEffectData);
        }

        [TestMethod]
        public void EffectLevel_CanBeUpdated()
        {
            var effect = new SpecialPowerEffect { dblEffectLevel = 1.0 };
            effect.dblEffectLevel = 5.5;
            Assert.AreEqual(5.5, effect.dblEffectLevel, 1e-10);
        }

        [TestMethod]
        public void TwoInstances_AreIndependent()
        {
            var e1 = new SpecialPowerEffect { strEffectType = "EN回復", dblEffectLevel = 1.0 };
            var e2 = new SpecialPowerEffect { strEffectType = "HP回復", dblEffectLevel = 2.0 };
            Assert.AreNotEqual(e1.strEffectType, e2.strEffectType);
            Assert.AreNotEqual(e1.dblEffectLevel, e2.dblEffectLevel);
        }

        [TestMethod]
        public void EffectData_EmptyString_IsAllowed()
        {
            var effect = new SpecialPowerEffect { strEffectData = "" };
            Assert.AreEqual("", effect.strEffectData);
        }

        [TestMethod]
        public void EffectType_WithSpecialChars_IsAllowed()
        {
            var effect = new SpecialPowerEffect { strEffectType = "特殊+効果" };
            Assert.AreEqual("特殊+効果", effect.strEffectType);
        }

        [TestMethod]
        public void AllPropertiesSet_CanBeReadBack()
        {
            var effect = new SpecialPowerEffect
            {
                strEffectType = "EN消費",
                dblEffectLevel = 3.0,
                strEffectData = "消費データ"
            };
            Assert.AreEqual("EN消費", effect.strEffectType);
            Assert.AreEqual(3.0, effect.dblEffectLevel, 1e-10);
            Assert.AreEqual("消費データ", effect.strEffectData);
        }

        [TestMethod]
        public void EffectLevel_Fractional_StoresCorrectly()
        {
            var effect = new SpecialPowerEffect { dblEffectLevel = 0.333 };
            Assert.AreEqual(0.333, effect.dblEffectLevel, 1e-10);
        }
    }
}
