using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// UnitAbility の使用可否判定・消費処理・射程・使用回数のテスト。
    /// 元の SRC の Unit.IsAbilityAvailable / UseAbility / AbilityENConsumption /
    /// IsTargetWithinAbilityRange / MaxStock / SetStock に対応する。
    /// </summary>
    [TestClass]
    public class UnitAbilityCoverageAvailabilityTests
    {
        private UnitAbilityCoverageFixture fx;

        [TestInitialize]
        public void Setup()
        {
            fx = new UnitAbilityCoverageFixture();
        }

        private (Unit user, AbilityData ad) Create(
            string effect = "回復Lv1", string aclass = "", int en = 0, int maxRange = 1, int stock = 0, int morale = 0)
        {
            var ud = fx.AddUnitData("使用機");
            ud.Value = 10000;
            var ad = ud.AddAbility("テストアビリティ");
            ad.SetEffect(effect);
            ad.Class = aclass;
            ad.ENConsumption = en;
            ad.MinRange = maxRange == 0 ? 0 : 1;
            ad.MaxRange = maxRange;
            ad.Stock = stock;
            ad.NecessaryMorale = morale;
            var user = fx.CreateUnit("使用機", "使用者");
            return (user, ad);
        }

        // ===== IsAbilityAvailable =====

        [TestMethod]
        public void Available_Basic_BeforeAndAfterMove()
        {
            // 射程1の通常アビリティは移動前・移動後とも使用可能
            var (u, _) = Create();
            Assert.IsTrue(u.Ability(1).IsAbilityAvailable("移動前"));
            Assert.IsTrue(u.Ability(1).IsAbilityAvailable("移動後"));
        }

        [TestMethod]
        public void Available_DisabledByEvent_IsFalse()
        {
            // Disable コマンドで封印されたアビリティは使用不可
            var (u, _) = Create();
            fx.Src.Expression.DefineGlobalVariable("Disable(テストアビリティ)");
            Assert.IsFalse(u.Ability(1).IsAbilityAvailable("移動前"));
            Assert.IsFalse(u.Ability(1).IsAbilityAvailable("インターミッション"));
        }

        [TestMethod]
        public void Available_InterMission_IgnoresEnAndMorale()
        {
            // インターミッション・ステータス表示では必要技能だけ満たせばＯＫ
            var (u, _) = Create(en: 500, morale: 150);
            Assert.IsTrue(u.Ability(1).IsAbilityAvailable("インターミッション"));
            Assert.IsTrue(u.Ability(1).IsAbilityAvailable(""));
            Assert.IsFalse(u.Ability(1).IsAbilityAvailable("移動前"));
        }

        [TestMethod]
        public void Available_RequiresMorale()
        {
            var (u, _) = Create(morale: 120);
            var p = u.MainPilot();
            p.Morale = 110;
            Assert.IsFalse(u.Ability(1).IsAbilityAvailable("移動前"));
            p.Morale = 120;
            Assert.IsTrue(u.Ability(1).IsAbilityAvailable("移動前"));
        }

        [TestMethod]
        public void Available_RequiresEnoughEn()
        {
            var (u, _) = Create(en: 30);
            u.EN = 29;
            Assert.IsFalse(u.Ability(1).IsAbilityAvailable("移動前"));
            u.EN = 30;
            Assert.IsTrue(u.Ability(1).IsAbilityAvailable("移動前"));
        }

        [TestMethod]
        public void Available_RequiresStock()
        {
            var (u, _) = Create(stock: 2);
            var a = u.Ability(1);
            Assert.IsTrue(a.IsAbilityAvailable("移動前"));
            a.SetStock(0);
            Assert.IsFalse(a.IsAbilityAvailable("移動前"));
        }

        [TestMethod]
        public void Available_RequiresMoneyForMoneyClass()
        {
            // 「銭」属性: 味方は 銭Lv × 修理費 / 10 の資金が必要
            var (u, _) = Create(aclass: "銭L2");
            var cost = 2 * u.Value / 10;
            fx.Src.Money = cost - 1;
            Assert.IsFalse(u.Ability(1).IsAbilityAvailable("移動前"));
            fx.Src.Money = cost;
            Assert.IsTrue(u.Ability(1).IsAbilityAvailable("移動前"));
        }

        [TestMethod]
        public void Available_AttributeDisabledConditions()
        {
            // 「○○属性使用不能」状態では該当属性のアビリティが使えない
            var (u, _) = Create(aclass: "火");
            u.AddCondition("火属性使用不能", 2);
            Assert.IsFalse(u.Ability(1).IsAbilityAvailable("移動前"));
        }

        [TestMethod]
        public void Available_SpellDisabledCondition()
        {
            var (u, _) = Create(aclass: "術");
            u.AddCondition("術使用不能", 2);
            Assert.IsFalse(u.Ability(1).IsAbilityAvailable("移動前"));
        }

        [TestMethod]
        public void Available_SilenceBlocksSpellAndVoice_ButNotOthers()
        {
            // 沈黙状態では術と音声技（音属性）が使用不能
            var (u, _) = Create(aclass: "音");
            u.AddCondition("沈黙", 2);
            Assert.IsFalse(u.Ability(1).IsAbilityAvailable("移動前"));

            var fx2 = new UnitAbilityCoverageFixture();
            fx = fx2;
            var (u2, _) = Create(aclass: "魔");
            u2.AddCondition("沈黙", 2);
            Assert.IsTrue(u2.Ability(1).IsAbilityAvailable("移動前"));
        }

        [TestMethod]
        public void Available_BerserkBlocksSpell()
        {
            var (u, _) = Create(aclass: "術");
            u.AddCondition("狂戦士", 2);
            Assert.IsFalse(u.Ability(1).IsAbilityAvailable("移動前"));
        }

        [TestMethod]
        public void Available_ImmobileBlocksMoveMapAbility()
        {
            var (u, _) = Create(aclass: "Ｍ移");
            u.AddCondition("移動不能", 2);
            Assert.IsFalse(u.Ability(1).IsAbilityAvailable("移動前"));
        }

        [TestMethod]
        public void Available_DyingOnly_RequiresHpQuarterOrLess()
        {
            // 「瀕」属性は HP が最大値の 1/4 以下のときのみ使用可能
            var (u, _) = Create(aclass: "瀕");
            u.HP = 251;
            Assert.IsFalse(u.Ability(1).IsAbilityAvailable("移動前"));
            u.HP = 250;
            Assert.IsTrue(u.Ability(1).IsAbilityAvailable("移動前"));
        }

        [TestMethod]
        public void Available_ChargingConditionBlocks()
        {
            // 自動充填中のアビリティは使用不可
            var (u, _) = Create();
            u.AddCondition("テストアビリティ充填中", 2);
            Assert.IsFalse(u.Ability(1).IsAbilityAvailable("移動前"));
        }

        [TestMethod]
        public void Available_ForbiddenClass_IsFalse()
        {
            var (u, _) = Create(aclass: "禁");
            Assert.IsFalse(u.Ability(1).IsAbilityAvailable("移動前"));
            // 必要技能だけを見るステータス表示では表示可能
            Assert.IsTrue(u.Ability(1).IsAbilityAvailable("インターミッション"));
        }

        [TestMethod]
        public void Available_ChargeClass_RequiresChargeCompleted_ExceptChargeCheck()
        {
            // 「Ｃ」属性はチャージ完了状態が必要。ただしチャージ判定ではそこまででＯＫ
            var (u, _) = Create(aclass: "Ｃ");
            Assert.IsTrue(u.Ability(1).IsAbilityAvailable("チャージ"));
            Assert.IsFalse(u.Ability(1).IsAbilityAvailable("移動前"));
            u.AddCondition("チャージ完了", 1);
            Assert.IsTrue(u.Ability(1).IsAbilityAvailable("移動前"));
        }

        [TestMethod]
        public void Available_AfterMove_RangeRules()
        {
            // 射程2以上（または0）のアビリティは「Ｐ」属性がなければ移動後使用不可
            var (u, _) = Create(maxRange: 3);
            Assert.IsTrue(u.Ability(1).IsAbilityAvailable("移動前"));
            Assert.IsFalse(u.Ability(1).IsAbilityAvailable("移動後"));
            Assert.IsFalse(u.Ability(1).CanUseAfterMove());
        }

        [TestMethod]
        public void CanUseAfterMove_PAndQClasses()
        {
            var (u, ad) = Create(aclass: "Ｐ", maxRange: 0);
            var a = u.Ability(1);
            Assert.IsTrue(a.CanUseAfterMove(), "射程0でもＰ属性なら移動後使用可");

            ad.MaxRange = 1;
            ad.Class = "Ｑ";
            Assert.IsFalse(a.CanUseAfterMove(), "射程1でもＱ属性なら移動後使用不可");

            ad.Class = "";
            Assert.IsTrue(a.CanUseAfterMove());
        }

        [TestMethod]
        public void Available_SharedChargingBlocksSameSharedLevel()
        {
            // 共属性: 同じ共レベルのアビリティが充填中なら使用不可
            var ud = fx.AddUnitData("使用機");
            var ad1 = ud.AddAbility("共アビ1");
            ad1.SetEffect("回復Lv1");
            ad1.Class = "共L1";
            ad1.MinRange = 1;
            ad1.MaxRange = 1;
            var ad2 = ud.AddAbility("共アビ2");
            ad2.SetEffect("補給Lv1");
            ad2.Class = "共L1";
            ad2.MinRange = 1;
            ad2.MaxRange = 1;
            var u = fx.CreateUnit("使用機", "使用者");

            Assert.IsTrue(u.Ability(1).IsAbilityAvailable("移動前"));
            u.AddCondition("共アビ2充填中", 2);
            Assert.IsFalse(u.Ability(1).IsAbilityAvailable("移動前"));
        }

        [TestMethod]
        public void Available_NoPilot_AlwaysTrue()
        {
            // パイロットが乗っていなければ常に使用可能と判定
            var ud = fx.AddUnitData("無人機");
            var ad = ud.AddAbility("テストアビリティ");
            ad.SetEffect("回復Lv1");
            ad.ENConsumption = 999;
            var u = fx.Src.UList.Add("無人機", 0, "味方");
            u.Update();
            Assert.IsTrue(u.Ability(1).IsAbilityAvailable("移動前"));
        }

        // ===== AbilityENConsumption =====

        [TestMethod]
        public void ENConsumption_ReducedByENSaveFeature()
        {
            // ＥＮ消費減少LvN で消費ＥＮは (1 - 0.1N) 倍
            var ud = fx.AddUnitData("使用機");
            ud.AddFeature("ＥＮ消費減少Lv2");
            var ad = ud.AddAbility("テストアビリティ");
            ad.SetEffect("回復Lv1");
            ad.ENConsumption = 50;
            var u = fx.CreateUnit("使用機", "使用者");

            Assert.AreEqual(40, u.Ability(1).AbilityENConsumption());
        }

        [TestMethod]
        public void ENConsumption_SpellReducedBySpellSkill_WithMinimum5()
        {
            // 術技能Lv3 で術の消費ＥＮは 0.8 倍。ただし 5 未満にはならない（元の値以下）
            var (u, ad) = Create(aclass: "術", en: 50);
            fx.Src.PDList.Item("使用者").AddSkill("術", 3d, "", 1);
            u.MainPilot().Update();
            Assert.AreEqual(40, u.Ability(1).AbilityENConsumption());

            ad.ENConsumption = 6;
            Assert.AreEqual(5, u.Ability(1).AbilityENConsumption());

            ad.ENConsumption = 3;
            Assert.AreEqual(3, u.Ability(1).AbilityENConsumption());
        }

        [TestMethod]
        public void ENConsumption_FeatReducedByFeatSkillLevel10()
        {
            var (u, _) = Create(aclass: "技", en: 100);
            fx.Src.PDList.Item("使用者").AddSkill("技", 10d, "", 1);
            u.MainPilot().Update();
            Assert.AreEqual(30, u.Ability(1).AbilityENConsumption());
        }

        // ===== 射程 =====

        [TestMethod]
        public void MinRange_IncreasedBySmallClass_CappedByMaxRange()
        {
            // 「小L」属性は最小射程を伸ばす（最大射程まで）
            var (u, ad) = Create(aclass: "小L2", maxRange: 4);
            Assert.AreEqual(3, u.Ability(1).AbilityMinRange());
            ad.Class = "小L9";
            Assert.AreEqual(4, u.Ability(1).AbilityMinRange());
        }

        [TestMethod]
        public void TargetWithinRange_ChecksManhattanDistance()
        {
            var (u, _) = Create(maxRange: 3);
            fx.AddUnitData("対象機");
            var t = fx.CreateUnit("対象機", "対象者");
            u.x = 5; u.y = 5;

            t.x = 7; t.y = 6;
            Assert.IsTrue(u.Ability(1).IsTargetWithinAbilityRange(t));
            t.x = 7; t.y = 7;
            Assert.IsFalse(u.Ability(1).IsTargetWithinAbilityRange(t), "距離4は射程外");
            t.x = 5; t.y = 5;
            Assert.IsFalse(u.Ability(1).IsTargetWithinAbilityRange(t), "距離0は最小射程1未満");
        }

        // ===== 使用回数 =====

        [TestMethod]
        public void MaxStock_ScaledByBossRank()
        {
            // ボスランク N では最大使用回数が (5+N)/5 倍
            var (u, _) = Create(stock: 5);
            Assert.AreEqual(5, u.Ability(1).MaxStock());
            u.BossRank = 5;
            Assert.AreEqual(10, u.Ability(1).MaxStock());
        }

        [TestMethod]
        public void SetStock_ClampsNegativeToZero_AndKeepsRatio()
        {
            var (u, _) = Create(stock: 4);
            var a = u.Ability(1);
            a.SetStock(-3);
            Assert.AreEqual(0, a.Stock());
            a.SetStock(3);
            Assert.AreEqual(3, a.Stock());
            a.SetStockFull();
            Assert.AreEqual(4, a.Stock());
        }

        // ===== UseAbility =====

        [TestMethod]
        public void Use_ConsumesEnAndStock()
        {
            var (u, _) = Create(en: 20, stock: 3);
            var a = u.Ability(1);

            a.UseAbility();

            Assert.AreEqual(80, u.EN);
            Assert.AreEqual(2, a.Stock());
        }

        [TestMethod]
        public void Use_ExhaustClass_AddsExhaustion()
        {
            // 「消」属性は使用後に消耗状態になる
            var (u, _) = Create(aclass: "消");
            u.Ability(1).UseAbility();
            Assert.AreEqual(1, u.ConditionLifetime("消耗"));
        }

        [TestMethod]
        public void Use_AllEnClass_SetsEnZero()
        {
            var (u, _) = Create(aclass: "尽", en: 10);
            u.Ability(1).UseAbility();
            Assert.AreEqual(0, u.EN);
        }

        [TestMethod]
        public void Use_AutoChargeClass_AddsChargingCondition()
        {
            // 「ＡL」属性は指定ターン数の充填中状態になり、その間は使用不可
            var (u, _) = Create(aclass: "ＡL2");
            var a = u.Ability(1);
            a.UseAbility();
            Assert.AreEqual(2, u.ConditionLifetime("テストアビリティ充填中"));
            Assert.IsFalse(a.IsAbilityAvailable("移動前"));
        }

        [TestMethod]
        public void Use_ChargeClass_ConsumesChargeCompleted()
        {
            var (u, _) = Create(aclass: "Ｃ");
            u.AddCondition("チャージ完了", 1);
            u.Ability(1).UseAbility();
            Assert.IsFalse(u.IsConditionSatisfied("チャージ完了"));
        }

        [TestMethod]
        public void Use_MoraleClass_Consumes5PerLevel()
        {
            var (u, _) = Create(aclass: "気L2");
            var p = u.MainPilot();
            p.Morale = 120;
            u.Ability(1).UseAbility();
            Assert.AreEqual(110, p.Morale);
        }

        [TestMethod]
        public void Use_HpLossClass_LosesTenthPerLevel()
        {
            // 「失L」属性は最大HP×Lv/10 の HP を失う（0未満にはならない）
            var (u, _) = Create(aclass: "失L3");
            u.Ability(1).UseAbility();
            Assert.AreEqual(700, u.HP);

            u.HP = 100;
            u.Ability(1).UseAbility();
            Assert.AreEqual(0, u.HP);
        }

        [TestMethod]
        public void Use_MoneyClass_ConsumesMoneyForAlly()
        {
            var (u, _) = Create(aclass: "銭L2");
            fx.Src.Money = 100000;
            var cost = 2 * u.Value / 10;
            u.Ability(1).UseAbility();
            Assert.AreEqual(100000 - cost, fx.Src.Money);
        }

        [TestMethod]
        public void Use_SimultaneousClass_EmptiesOtherAbilitiesStockRatio()
        {
            // 「斉」属性（一斉使用）は他のアビリティの使用回数も同じ割合にする
            var ud = fx.AddUnitData("使用機");
            var ad1 = ud.AddAbility("一斉");
            ad1.SetEffect("回復Lv1");
            ad1.Class = "斉";
            ad1.Stock = 1;
            var ad2 = ud.AddAbility("通常");
            ad2.SetEffect("補給Lv1");
            ad2.Stock = 4;
            var u = fx.CreateUnit("使用機", "使用者");

            u.Ability(1).UseAbility();

            Assert.AreEqual(0, u.Ability(1).Stock());
            Assert.AreEqual(0, u.Ability(2).Stock());
        }

        // ===== 表示判定 =====

        [TestMethod]
        public void IdDisplayFor_HidesDisabledAndUnmasteredAbilities()
        {
            var (u, ad) = Create();
            var a = u.Ability(1);
            Assert.IsTrue(a.IdDisplayFor(AbilityListMode.List));
            Assert.IsTrue(a.CanUseFor(AbilityListMode.List, null));

            // 必要技能を満たさないアビリティは表示しない
            ad.NecessarySkill = "存在しない技能";
            Assert.IsFalse(a.IdDisplayFor(AbilityListMode.List));
            ad.NecessarySkill = "";

            fx.Src.Expression.DefineGlobalVariable("Disable(テストアビリティ)");
            Assert.IsFalse(a.IdDisplayFor(AbilityListMode.List));
        }

        // ===== 属性レベル =====

        [TestMethod]
        public void AbilityLevel_ParsesTrailingLevel()
        {
            var (u, _) = Create(aclass: "魔気L3");
            Assert.AreEqual(3d, u.Ability(1).AbilityLevel("気"));
            Assert.AreEqual(0d, u.Ability(1).AbilityLevel("魔"));
        }
    }
}
