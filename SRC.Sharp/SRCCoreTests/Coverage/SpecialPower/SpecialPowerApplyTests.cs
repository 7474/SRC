using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCCore.Coverage.Tests.SpecialPower
{
    /// <summary>
    /// SpecialPowerData.Effective / Apply の効果別の判定と適用。
    /// 元の SRC の SpecialPowerData.Effective / Apply に対応。
    /// </summary>
    [TestClass]
    public class SpecialPowerApplyTests
    {
        // ──────────────────────────────────────────────
        // ＨＰ・ＥＮ・ＳＰ・気力
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HPRecovery_EffectiveOnlyWhenDamaged_AndRecoversTenPercentPerLevel()
        {
            // ＨＰ回復Lvn は最大ＨＰの n×10% を回復。ＨＰ満タンなら効果なし
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("根性", "自分", "即効", "ＨＰ回復Lv3");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            var p = u.MainPilot();

            Assert.IsFalse(sd.Effective(p, u), "ＨＰ満タンでは効果なし");

            u.HP = 100;
            Assert.IsTrue(sd.Effective(p, u));
            sd.Apply(p, u, is_event: true);
            Assert.AreEqual(400, u.HP);
        }

        [TestMethod]
        public void HPRecovery_Zombie_NotEffectiveAndNotApplied()
        {
            // ゾンビ状態のユニットはＨＰ回復を受け付けない
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("根性", "自分", "即効", "ＨＰ回復Lv3");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            u.HP = 100;
            u.AddCondition("ゾンビ", 3);

            Assert.IsFalse(sd.Effective(u.MainPilot(), u));
            sd.Apply(u.MainPilot(), u, is_event: true);
            Assert.AreEqual(100, u.HP);
        }

        [TestMethod]
        public void HPRecovery_NegativeLevel_AlwaysEffectiveAndReducesHP()
        {
            // 負のレベルはＨＰ満タンでも有効で、ＨＰを減らす
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("呪い", "敵", "即効", "ＨＰ回復Lv-2");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            var enemy = f.AddUnit("敵機", "敵パイロット", "敵", 5, 5);

            Assert.IsTrue(sd.Effective(me.MainPilot(), enemy));
            sd.Apply(me.MainPilot(), enemy, is_event: true);
            Assert.AreEqual(800, enemy.HP);
        }

        [TestMethod]
        public void HPIncrease_AddsThousandPerLevel_ClampedToMax()
        {
            // ＨＰ増加Lvn は 1000×n だけＨＰを増やす (最大ＨＰで頭打ち)
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("ＨＰ増加", "自分", "即効", "ＨＰ増加Lv1");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1, hp: 3000);
            u.HP = 500;

            sd.Apply(u.MainPilot(), u, is_event: true);
            Assert.AreEqual(1500, u.HP);

            u.HP = 2500;
            sd.Apply(u.MainPilot(), u, is_event: true);
            Assert.AreEqual(3000, u.HP);
        }

        [TestMethod]
        public void HPRecovery_NotEvent_DisplaysRecoveredAmount()
        {
            // イベント外の使用では回復量を表示する
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("根性", "自分", "即効", "ＨＰ回復Lv5");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            f.Src.Commands.SelectedUnit = u;
            u.HP = 200;

            var waited = sd.Apply(u.MainPilot(), u);

            Assert.IsTrue(waited, "表示を行った場合はウェイトが必要と返す");
            Assert.AreEqual(700, u.HP);
            CollectionAssert.Contains(f.SysStrings, "+500");
            Assert.AreEqual(1, f.SysMessages.Count);
            StringAssert.Contains(f.SysMessages[0], "500回復した。");
        }

        [TestMethod]
        public void ENRecoveryAndIncrease()
        {
            // ＥＮ回復Lvn は最大ＥＮの n×10%、ＥＮ増加Lvn は 10×n を回復
            var f = new SpecialPowerTestFixture();
            var recover = f.DefineSP("補給", "自分", "即効", "ＥＮ回復Lv5");
            var increase = f.DefineSP("充電", "自分", "即効", "ＥＮ増加Lv2");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1, en: 200);
            var p = u.MainPilot();

            Assert.IsFalse(recover.Effective(p, u), "ＥＮ満タンでは効果なし");

            u.EN = 10;
            Assert.IsTrue(recover.Effective(p, u));
            recover.Apply(p, u, is_event: true);
            Assert.AreEqual(110, u.EN);

            increase.Apply(p, u, is_event: true);
            Assert.AreEqual(130, u.EN);
        }

        [TestMethod]
        public void SPRecovery_SinglePilot_RecoversTenPerLevel()
        {
            // パイロット１人の場合は 10×Lv のＳＰを回復
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("癒し", "味方", "即効", "ＳＰ回復Lv5");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1, level: 30);
            var p = u.MainPilot();
            Assert.IsFalse(sd.Effective(p, u), "ＳＰ満タンでは効果なし");

            p.SP = 0;
            Assert.IsTrue(sd.Effective(p, u));
            sd.Apply(p, u, is_event: true);
            Assert.AreEqual(50, p.SP);
        }

        [TestMethod]
        public void MoraleIncrease_AddsTenPerLevel_ExceptMachine()
        {
            // 気力増加Lvn は気力を 10×n 増やす。性格が機械のパイロットには無効
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("気合", "自分", "即効", "気力増加Lv1");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            var p = u.MainPilot();
            Assert.AreEqual(100, p.Morale);

            Assert.IsTrue(sd.Effective(p, u));
            sd.Apply(p, u, is_event: true);
            Assert.AreEqual(110, p.Morale);

            p.Morale = p.MaxMorale;
            Assert.IsFalse(sd.Effective(p, u), "気力上限では効果なし");

            f.DefinePilot("ロボ").Personality = "機械";
            var robo = f.AddUnit("機体", "ロボ", "味方", 2, 1);
            Assert.IsFalse(sd.Effective(robo.MainPilot(), robo));
            sd.Apply(robo.MainPilot(), robo, is_event: true);
            Assert.AreEqual(100, robo.MainPilot().Morale);
        }

        [TestMethod]
        public void MoraleDecrease_SubtractsTenPerLevel_UntilMin()
        {
            // 気力低下Lvn は気力を 10×n 下げる。下限なら効果なし
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("戦慄", "敵", "即効", "気力低下Lv1");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            var enemy = f.AddUnit("敵機", "敵パイロット", "敵", 5, 5);
            var ep = enemy.MainPilot();

            sd.Apply(me.MainPilot(), enemy, is_event: true);
            Assert.AreEqual(90, ep.Morale);

            ep.Morale = ep.MinMorale;
            Assert.IsFalse(sd.Effective(me.MainPilot(), enemy));
        }

        [TestMethod]
        public void HPAndENDecrease_ReduceByTenPercentPerLevel_UnlessInvincible()
        {
            // ＨＰ減少Lvn / ＥＮ減少Lvn は現在値の n×10% を減らす。無敵状態には無効
            var f = new SpecialPowerTestFixture();
            var hpDown = f.DefineSP("脱力", "敵", "即効", "ＨＰ減少Lv5");
            var enDown = f.DefineSP("吸収", "敵", "即効", "ＥＮ減少Lv3");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            var enemy = f.AddUnit("敵機", "敵パイロット", "敵", 5, 5);
            var p = me.MainPilot();

            Assert.IsTrue(hpDown.Effective(p, enemy));
            hpDown.Apply(p, enemy, is_event: true);
            Assert.AreEqual(500, enemy.HP);

            enDown.Apply(p, enemy, is_event: true);
            Assert.AreEqual(70, enemy.EN);

            enemy.AddCondition("無敵", 1);
            Assert.IsFalse(hpDown.Effective(p, enemy));
            hpDown.Apply(p, enemy, is_event: true);
            Assert.AreEqual(500, enemy.HP);
        }

        [TestMethod]
        public void HPDecrease_UnstableUnit_GoesBerserkAtQuarterHP()
        {
            // 「不安定」を持つユニットはＨＰが最大の1/4以下になると暴走する
            var f = new SpecialPowerTestFixture();
            var hpDown = f.DefineSP("脱力", "敵", "即効", "ＨＰ減少Lv8");
            f.Src.UDList.Add("暴れ機").AddFeature("不安定");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            var enemy = f.AddUnit("暴れ機", "敵パイロット", "敵", 5, 5);

            hpDown.Apply(me.MainPilot(), enemy, is_event: true);

            Assert.AreEqual(200, enemy.HP);
            Assert.IsTrue(enemy.IsConditionSatisfied("暴走"));
        }

        [TestMethod]
        public void RandomDamage_DealsTenTimesDiceOfTenTimesLevel_ClampedToTen()
        {
            // ランダムダメージLvn は 10×Dice(10×n) のダメージ。ただしＨＰは 10 未満にならない
            // (VB: .HP = MaxLng(.HP - 10 * Dice(10 * EffectLevel(i)), 10))
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("激怒", "全敵", "即効", "ランダムダメージLv10");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            var enemy = f.AddUnit("敵機", "敵パイロット", "敵", 5, 5, hp: 2000);

            using (var rnd = new FixedRandom())
            {
                // Dice(100) = (int)(100 × 0.305) + 1 = 31 → 310 ダメージ
                rnd.Next(0.305);
                enemy.HP = 1000;
                sd.Apply(me.MainPilot(), enemy, is_event: true);
                Assert.AreEqual(690, enemy.HP);

                // Dice(100) = 100 → 1000 ダメージだがＨＰ 500 から 10 で止まる
                rnd.Next(0.999);
                enemy.HP = 500;
                sd.Apply(me.MainPilot(), enemy, is_event: true);
                Assert.AreEqual(10, enemy.HP);
            }
        }

        // ──────────────────────────────────────────────
        // 状態回復・行動数・スペシャルパワー付与
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StatusRecovery_RemovesAbnormalConditionsOnly()
        {
            // 状態回復はステータス異常と「～属性使用不能」を解除し、それ以外の状態は残す
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("浄化", "味方", "即効", "状態回復");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            var p = u.MainPilot();
            Assert.IsFalse(sd.Effective(p, u), "ステータス異常がなければ効果なし");

            u.AddCondition("毒", 3);
            u.AddCondition("混乱", 2);
            u.AddCondition("炎属性使用不能", 2);
            u.AddCondition("運動性ＵＰ", 2);
            Assert.IsTrue(sd.Effective(p, u));

            sd.Apply(p, u, is_event: true);

            Assert.IsFalse(u.IsConditionSatisfied("毒"));
            Assert.IsFalse(u.IsConditionSatisfied("混乱"));
            Assert.IsFalse(u.IsConditionSatisfied("炎属性使用不能"));
            Assert.IsTrue(u.IsConditionSatisfied("運動性ＵＰ"));
        }

        [TestMethod]
        public void StatusRecovery_AttributeDisableOnly_IsEffective()
        {
            // 「～属性使用不能」だけでも状態回復は有効
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("浄化", "味方", "即効", "状態回復");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            u.AddCondition("雷属性使用不能", 2);

            Assert.IsTrue(sd.Effective(u.MainPilot(), u));
        }

        [TestMethod]
        public void ActionRecovery_OnlyForActedUnit()
        {
            // 行動数回復は行動済み (残り行動数0) のユニットを再行動可能にする
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("再動", "味方", "即効", "行動数回復");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            var p = u.MainPilot();
            Assert.IsFalse(sd.Effective(p, u), "未行動なら効果なし");

            u.UseAction();
            Assert.AreEqual(0, u.Action);
            Assert.IsTrue(sd.Effective(p, u));

            sd.Apply(p, u, is_event: true);
            Assert.AreEqual(1, u.Action);
        }

        [TestMethod]
        public void ActionIncrease_AddsOneAction()
        {
            // 行動数増加は行動数を１つ増やす
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("覚醒", "自分", "即効", "行動数増加");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            Assert.IsTrue(sd.Effective(u.MainPilot(), u));

            sd.Apply(u.MainPilot(), u, is_event: true);
            Assert.AreEqual(2, u.Action);
        }

        [TestMethod]
        public void NestedSpecialPower_MakesInnerSPInEffect()
        {
            // 「スペシャルパワー=X」効果は対象を X の影響下に置く。既に影響下なら効果なし
            var f = new SpecialPowerTestFixture();
            f.DefineSP("加速", "自分", "ターン", "移動力強化Lv2");
            var sd = f.DefineSP("応援", "味方", "即効", "スペシャルパワー=加速");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            var ally = f.AddUnit("僚機", "僚友", "味方", 2, 1);

            Assert.IsTrue(sd.Effective(me.MainPilot(), ally));
            sd.Apply(me.MainPilot(), ally, is_event: true);

            Assert.IsTrue(ally.IsSpecialPowerInEffect("加速"));
            Assert.IsFalse(sd.Effective(me.MainPilot(), ally));
        }

        // ──────────────────────────────────────────────
        // 持続効果
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Persistent_ApplyMarksInEffect_AndBecomesIneffective()
        {
            // 即効以外のスペシャルパワーは影響下に置くだけで、二重には掛からない
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("熱血", "自分", "攻撃", "ダメージ増加Lv1");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            var p = u.MainPilot();
            Assert.IsTrue(sd.Effective(p, u));

            var waited = sd.Apply(p, u, is_event: true);

            Assert.IsFalse(waited);
            Assert.IsTrue(u.IsSpecialPowerInEffect("熱血"));
            Assert.AreEqual(p.ID, u.SpecialPowerData(1), "使用者のパイロットＩＤを記録する");
            Assert.IsFalse(sd.Effective(p, u));
        }

        [TestMethod]
        public void Persistent_AsInstant_AppliesEffectsImmediately()
        {
            // as_instant 指定時は持続効果を持つものでも効果を即座に適用する
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("根性継続", "自分", "ターン", "ＨＰ回復Lv5");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            u.HP = 100;

            sd.Apply(u.MainPilot(), u, is_event: true, as_instant: true);

            Assert.AreEqual(600, u.HP);
            Assert.IsFalse(u.IsSpecialPowerInEffect("根性継続"));
        }

        [TestMethod]
        public void Migawari_CannotTargetSelf()
        {
            // みがわりは自分自身には使えない
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("かばう", "味方", "みがわり", "みがわり");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            var ally = f.AddUnit("僚機", "僚友", "味方", 2, 1);

            Assert.IsFalse(sd.Effective(me.MainPilot(), me));
            Assert.IsTrue(sd.Effective(me.MainPilot(), ally));
        }

        // ──────────────────────────────────────────────
        // 適用条件・無効化
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Condition_Technique_RequiresHigherOrEqualTechnique()
        {
            // 適用条件「技量」: 使用者の技量が相手のメインパイロット以上のときのみ有効
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("足かせ", "敵", "ターン", "移動力強化Lv-2", necessaryCondition: "技量");
            f.DefinePilot("自分", technique: 120);
            f.DefinePilot("達人", technique: 150);
            f.DefinePilot("素人", technique: 100);
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            var strong = f.AddUnit("敵機", "達人", "敵", 5, 5);
            var weak = f.AddUnit("敵機", "素人", "敵", 6, 5);
            var p = me.MainPilot();

            Assert.IsFalse(sd.Effective(p, strong));
            Assert.IsTrue(sd.Effective(p, weak));

            Assert.IsFalse(sd.Apply(p, strong, is_event: true));
            Assert.IsFalse(strong.IsSpecialPowerInEffect("足かせ"));
            sd.Apply(p, weak, is_event: true);
            Assert.IsTrue(weak.IsSpecialPowerInEffect("足かせ"));
        }

        [TestMethod]
        public void Condition_NonBoss_ExcludesBossRankUnits()
        {
            // 適用条件「非ボス」: ボスランクを持つユニットには無効
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("威圧", "敵", "ターン", "行動不能", necessaryCondition: "非ボス");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            var enemy = f.AddUnit("敵機", "敵パイロット", "敵", 5, 5);
            Assert.IsTrue(sd.Effective(me.MainPilot(), enemy));

            enemy.BossRank = 0;
            Assert.IsFalse(sd.Effective(me.MainPilot(), enemy));
            sd.Apply(me.MainPilot(), enemy, is_event: true);
            Assert.IsFalse(enemy.IsSpecialPowerInEffect("威圧"));
        }

        [TestMethod]
        public void Condition_Support_ExcludesSelf()
        {
            // 適用条件「支援」: 自分自身には使えない
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("信頼", "味方", "即効", "ＨＰ回復Lv3", necessaryCondition: "支援");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            var ally = f.AddUnit("僚機", "僚友", "味方", 3, 3);
            me.HP = 100;
            ally.HP = 100;

            Assert.IsFalse(sd.Effective(me.MainPilot(), me));
            Assert.IsTrue(sd.Effective(me.MainPilot(), ally));
            sd.Apply(me.MainPilot(), me, is_event: true);
            Assert.AreEqual(100, me.HP);
        }

        [TestMethod]
        public void Condition_AdjacentAndRange_UseManhattanDistance()
        {
            // 「隣接」は距離1のみ、「射程Lvn」は距離n以内
            var f = new SpecialPowerTestFixture();
            var adj = f.DefineSP("手当て", "味方", "ターン", "移動力強化Lv1", necessaryCondition: "隣接");
            var range = f.DefineSP("遠隔", "味方", "ターン", "移動力強化Lv1", necessaryCondition: "射程Lv2");
            var me = f.AddUnit("自機", "自分", "味方", 5, 5);
            var near = f.AddUnit("僚機", "僚友1", "味方", 5, 6);
            var mid = f.AddUnit("僚機", "僚友2", "味方", 6, 6);
            var far = f.AddUnit("僚機", "僚友3", "味方", 8, 5);
            var p = me.MainPilot();

            Assert.IsTrue(adj.Effective(p, near));
            Assert.IsFalse(adj.Effective(p, mid));
            Assert.IsTrue(range.Effective(p, mid));
            Assert.IsFalse(range.Effective(p, far));

            range.Apply(p, far, is_event: true);
            Assert.IsFalse(far.IsSpecialPowerInEffect("遠隔"));
            range.Apply(p, mid, is_event: true);
            Assert.IsTrue(mid.IsSpecialPowerInEffect("遠隔"));
        }

        [TestMethod]
        public void SpecialPowerNullification_BlocksEnemyTargetedOnly()
        {
            // 「スペシャルパワー無効化」状態の敵には敵対象のスペシャルパワーが効かない
            var f = new SpecialPowerTestFixture();
            var enemySp = f.DefineSP("脱力", "敵", "即効", "気力低下Lv1");
            var allySp = f.DefineSP("激励", "味方", "即効", "気力増加Lv1");
            var me = f.AddUnit("自機", "自分", "味方", 1, 1);
            var enemy = f.AddUnit("敵機", "敵パイロット", "敵", 5, 5);
            enemy.AddCondition("スペシャルパワー無効化", 3);

            Assert.IsFalse(enemySp.Effective(me.MainPilot(), enemy));
            Assert.IsTrue(allySp.Effective(me.MainPilot(), enemy), "味方対象のものは無効化の対象外");
        }
    }
}
