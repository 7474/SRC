using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// SpecialPowerEffect クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class SpecialPowerEffectTests
    {
        // ──────────────────────────────────────────────
        // プロパティ設定・取得
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrEffectType_CanBeSetAndRead()
        {
            var effect = new SpecialPowerEffect { strEffectType = "HP回復" };
            Assert.AreEqual("HP回復", effect.strEffectType);
        }

        [TestMethod]
        public void DblEffectLevel_CanBeSetAndRead()
        {
            var effect = new SpecialPowerEffect { dblEffectLevel = 3.5 };
            Assert.AreEqual(3.5, effect.dblEffectLevel);
        }

        [TestMethod]
        public void StrEffectData_CanBeSetAndRead()
        {
            var effect = new SpecialPowerEffect { strEffectData = "100" };
            Assert.AreEqual("100", effect.strEffectData);
        }

        [TestMethod]
        public void DefaultStrEffectType_IsNull()
        {
            var effect = new SpecialPowerEffect();
            Assert.IsNull(effect.strEffectType);
        }

        [TestMethod]
        public void DefaultDblEffectLevel_IsZero()
        {
            var effect = new SpecialPowerEffect();
            Assert.AreEqual(0.0, effect.dblEffectLevel);
        }

        [TestMethod]
        public void DefaultStrEffectData_IsNull()
        {
            var effect = new SpecialPowerEffect();
            Assert.IsNull(effect.strEffectData);
        }

        [TestMethod]
        public void AllFields_CanBeSetTogether()
        {
            var effect = new SpecialPowerEffect
            {
                strEffectType = "EN回復",
                dblEffectLevel = 2.0,
                strEffectData = "50"
            };
            Assert.AreEqual("EN回復", effect.strEffectType);
            Assert.AreEqual(2.0, effect.dblEffectLevel);
            Assert.AreEqual("50", effect.strEffectData);
        }

        [TestMethod]
        public void StrEffectData_CanBeNull()
        {
            var effect = new SpecialPowerEffect
            {
                strEffectType = "攻撃力アップ",
                strEffectData = null
            };
            Assert.IsNull(effect.strEffectData);
        }

        [TestMethod]
        public void DblEffectLevel_DefaultLevel_CanBeSet()
        {
            var effect = new SpecialPowerEffect { dblEffectLevel = Constants.DEFAULT_LEVEL };
            Assert.AreEqual(Constants.DEFAULT_LEVEL, effect.dblEffectLevel);
        }
    }
}
