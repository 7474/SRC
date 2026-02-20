using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    [TestClass]
    public class UnitAbilityEffectiveTests
    {
        private SRC CreateSrc()
        {
            return new SRC
            {
                GUI = new MockGUI(),
            };
        }

        private (Unit unit, UnitAbility ability) CreateHealAbility(SRC src, string ownerParty = "味方")
        {
            var unit = new Unit(src);
            unit.Party = ownerParty;

            var ad = new AbilityData(src);
            ad.SetEffect("回復Lv1");

            var ability = new UnitAbility(src, unit, ad);
            return (unit, ability);
        }

        // ===== Party alignment check =====

        [TestMethod]
        public void IsAbilityEffective_ReturnsFalse_WhenAllyAbilityUsedOnEnemy()
        {
            var src = CreateSrc();
            var (_, ability) = CreateHealAbility(src, "味方");

            var enemy = new Unit(src);
            enemy.Party = "敵";

            Assert.IsFalse(ability.IsAbilityEffective(enemy));
        }

        [TestMethod]
        public void IsAbilityEffective_ReturnsTrue_WhenAllyAbilityUsedOnAlly()
        {
            var src = CreateSrc();
            var (ownerUnit, ability) = CreateHealAbility(src, "味方");

            var ally = new Unit(src);
            ally.Party = "味方";
            // HP is 0, MaxHP is at least 1 → HP < MaxHP → heal is effective

            Assert.IsTrue(ability.IsAbilityEffective(ally));
        }

        [TestMethod]
        public void IsAbilityEffective_ReturnsTrue_WhenAllyAbilityUsedOnNPC()
        {
            var src = CreateSrc();
            var (_, ability) = CreateHealAbility(src, "味方");

            var npc = new Unit(src);
            npc.Party = "ＮＰＣ";

            Assert.IsTrue(ability.IsAbilityEffective(npc));
        }

        [TestMethod]
        public void IsAbilityEffective_ReturnsFalse_WhenEnemyAbilityUsedOnAlly()
        {
            var src = CreateSrc();
            var (_, ability) = CreateHealAbility(src, "敵");

            var ally = new Unit(src);
            ally.Party = "味方";

            Assert.IsFalse(ability.IsAbilityEffective(ally));
        }

        // ===== Heal (回復) effect =====

        [TestMethod]
        public void IsAbilityEffective_ReturnsFalse_ForHeal_WhenHPFull()
        {
            var src = CreateSrc();
            var (_, ability) = CreateHealAbility(src, "味方");

            var target = new Unit(src);
            target.Party = "味方";
            // Set HP to MaxHP (both default to produce full HP)
            // lngHP is private; we can set via HP property which caps to MaxHP
            target.HP = target.MaxHP;

            Assert.IsFalse(ability.IsAbilityEffective(target));
        }

        [TestMethod]
        public void IsAbilityEffective_ReturnsTrue_ForHeal_WhenHPNotFull()
        {
            var src = CreateSrc();
            var (_, ability) = CreateHealAbility(src, "味方");

            var target = new Unit(src);
            target.Party = "味方";
            // Default HP is 0, MaxHP is at least 1 → HP < MaxHP

            Assert.IsTrue(ability.IsAbilityEffective(target));
        }

        // ===== 治癒 (cure) effect =====

        [TestMethod]
        public void IsAbilityEffective_ReturnsFalse_ForCure_WhenNoConditions()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Party = "味方";

            var ad = new AbilityData(src);
            ad.SetEffect("治癒Lv1");
            var ability = new UnitAbility(src, unit, ad);

            var target = new Unit(src);
            target.Party = "味方";
            // No conditions → cure is not effective

            Assert.IsFalse(ability.IsAbilityEffective(target));
        }

        [TestMethod]
        public void IsAbilityEffective_ReturnsTrue_ForCure_WhenHasCurableCondition()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Party = "味方";

            var ad = new AbilityData(src);
            ad.SetEffect("治癒Lv1");
            var ability = new UnitAbility(src, unit, ad);

            var target = new Unit(src);
            target.Party = "味方";
            target.AddCondition("麻痺", 3);

            Assert.IsTrue(ability.IsAbilityEffective(target));
        }

        // ===== 補給 (resupply EN) effect =====

        [TestMethod]
        public void IsAbilityEffective_ReturnsTrue_ForResupply_WhenENNotFull()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Party = "味方";

            var ad = new AbilityData(src);
            ad.SetEffect("補給Lv1");
            var ability = new UnitAbility(src, unit, ad);

            var target = new Unit(src);
            target.Party = "味方";
            // Default EN is 0, MaxEN is at least 5 → EN < MaxEN

            Assert.IsTrue(ability.IsAbilityEffective(target));
        }

        // ===== 状態 (status effect) =====

        [TestMethod]
        public void IsAbilityEffective_ReturnsFalse_ForStatusEffect_WhenAlreadyApplied()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Party = "味方";

            var ad = new AbilityData(src);
            ad.SetEffect("状態Lv1=混乱");
            var ability = new UnitAbility(src, unit, ad);

            var target = new Unit(src);
            target.Party = "味方";
            target.AddCondition("混乱", 3);

            Assert.IsFalse(ability.IsAbilityEffective(target));
        }

        [TestMethod]
        public void IsAbilityEffective_ReturnsTrue_ForStatusEffect_WhenNotApplied()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Party = "味方";

            var ad = new AbilityData(src);
            ad.SetEffect("状態Lv1=混乱");
            var ability = new UnitAbility(src, unit, ad);

            var target = new Unit(src);
            target.Party = "味方";
            // No conditions → status effect is useful

            Assert.IsTrue(ability.IsAbilityEffective(target));
        }

        // ===== No effects (fallback) =====

        [TestMethod]
        public void IsAbilityEffective_ReturnsTrue_WhenNoEffects()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.Party = "味方";

            var ad = new AbilityData(src);
            // No effects set
            var ability = new UnitAbility(src, unit, ad);

            var target = new Unit(src);
            target.Party = "味方";

            // No effects → always considered usable (include等で特殊効果を定義していると仮定)
            Assert.IsTrue(ability.IsAbilityEffective(target));
        }
    }
}
