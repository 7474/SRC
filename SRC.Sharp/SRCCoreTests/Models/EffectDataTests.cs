using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// SpecialPowerEffect, AbilityEffect, MessageDataItem クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class EffectDataTests
    {
        // ──────────────────────────────────────────────
        // SpecialPowerEffect
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SpecialPowerEffect_Properties_CanBeSetAndRead()
        {
            var eff = new SpecialPowerEffect
            {
                strEffectType = "攻撃力アップ",
                dblEffectLevel = 1.2,
                strEffectData = "追加データ"
            };

            Assert.AreEqual("攻撃力アップ", eff.strEffectType);
            Assert.AreEqual(1.2, eff.dblEffectLevel);
            Assert.AreEqual("追加データ", eff.strEffectData);
        }

        [TestMethod]
        public void SpecialPowerEffect_DefaultValues_AreNull()
        {
            var eff = new SpecialPowerEffect();
            Assert.IsNull(eff.strEffectType);
            Assert.AreEqual(0d, eff.dblEffectLevel);
            Assert.IsNull(eff.strEffectData);
        }

        // ──────────────────────────────────────────────
        // AbilityEffect
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AbilityEffect_Properties_CanBeSetAndRead()
        {
            var ae = new AbilityEffect
            {
                Name = "回復",
                Level = 2.5,
                Data = "追加データ"
            };

            Assert.AreEqual("回復", ae.Name);
            Assert.AreEqual(2.5, ae.Level);
            Assert.AreEqual("追加データ", ae.Data);
        }

        [TestMethod]
        public void AbilityEffect_EffectType_ReturnsSameAsName()
        {
            var ae = new AbilityEffect { Name = "攻撃" };
            Assert.AreEqual("攻撃", ae.EffectType);
        }

        // ──────────────────────────────────────────────
        // MessageDataItem
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MessageDataItem_Properties_SetInConstructor()
        {
            var item = new MessageDataItem("格闘", "やるぜ！");
            Assert.AreEqual("格闘", item.Situation);
            Assert.AreEqual("やるぜ！", item.Message);
        }

        [TestMethod]
        public void MessageDataItem_IsAvailable_AlwaysReturnsTrue()
        {
            var item = new MessageDataItem("攻撃", "ふぁいとー");
            Assert.IsTrue(item.IsAvailable(null, false));
            Assert.IsTrue(item.IsAvailable(null, true));
        }
    }
}
