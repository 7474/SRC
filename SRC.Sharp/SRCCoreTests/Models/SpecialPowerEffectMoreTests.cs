using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// SpecialPowerEffect クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class SpecialPowerEffectAdditionalTests
    {
        // ──────────────────────────────────────────────
        // プロパティの設定・読み取り
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Properties_CanBeSetAndRead()
        {
            var effect = new SpecialPowerEffect
            {
                strEffectType = "HP回復",
                dblEffectLevel = 2.5,
                strEffectData = "回復データ"
            };

            Assert.AreEqual("HP回復", effect.strEffectType);
            Assert.AreEqual(2.5, effect.dblEffectLevel);
            Assert.AreEqual("回復データ", effect.strEffectData);
        }

        [TestMethod]
        public void EffectType_DefaultIsNull()
        {
            var effect = new SpecialPowerEffect();
            Assert.IsNull(effect.strEffectType);
        }

        [TestMethod]
        public void EffectLevel_DefaultIsZero()
        {
            var effect = new SpecialPowerEffect();
            Assert.AreEqual(0d, effect.dblEffectLevel);
        }

        [TestMethod]
        public void EffectData_DefaultIsNull()
        {
            var effect = new SpecialPowerEffect();
            Assert.IsNull(effect.strEffectData);
        }

        [TestMethod]
        public void EffectType_CanBeUpdated()
        {
            var effect = new SpecialPowerEffect { strEffectType = "初期" };
            effect.strEffectType = "更新";
            Assert.AreEqual("更新", effect.strEffectType);
        }

        [TestMethod]
        public void EffectLevel_CanBeNegative()
        {
            var effect = new SpecialPowerEffect { dblEffectLevel = -1.0 };
            Assert.AreEqual(-1.0, effect.dblEffectLevel);
        }

        [TestMethod]
        public void EffectData_EmptyString_IsAllowed()
        {
            var effect = new SpecialPowerEffect { strEffectData = "" };
            Assert.AreEqual("", effect.strEffectData);
        }

        [TestMethod]
        public void AllProperties_SetAtOnce()
        {
            var effect = new SpecialPowerEffect
            {
                strEffectType = "EN回復",
                dblEffectLevel = 5.0,
                strEffectData = "some data"
            };

            Assert.AreEqual("EN回復", effect.strEffectType);
            Assert.AreEqual(5.0, effect.dblEffectLevel);
            Assert.AreEqual("some data", effect.strEffectData);
        }

        [TestMethod]
        public void MultipleInstances_AreIndependent()
        {
            var effect1 = new SpecialPowerEffect { strEffectType = "HP回復", dblEffectLevel = 1.0 };
            var effect2 = new SpecialPowerEffect { strEffectType = "EN回復", dblEffectLevel = 2.0 };

            Assert.AreNotEqual(effect1.strEffectType, effect2.strEffectType);
            Assert.AreNotEqual(effect1.dblEffectLevel, effect2.dblEffectLevel);
        }
    }
}
