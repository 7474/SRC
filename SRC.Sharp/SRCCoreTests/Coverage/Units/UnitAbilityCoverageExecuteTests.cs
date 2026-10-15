using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Unit.ExecuteAbility（元の SRC の Unit.ExecuteAbility）のアビリティ効果適用のテスト。
    /// 「難」属性を持たないアビリティは Dice(10) <= 0 が成立しないため乱数の影響を受けない。
    /// </summary>
    [TestClass]
    public class UnitAbilityCoverageExecuteTests
    {
        private UnitAbilityCoverageFixture fx;

        [TestInitialize]
        public void Setup()
        {
            fx = new UnitAbilityCoverageFixture();
        }

        // 使用者ユニット（アビリティ1つ）と対象ユニットを作る
        private (Unit user, Unit target, UnitAbility ability) Create(
            string effect, string aclass = "", int en = 0, int maxRange = 1, int stock = 0)
        {
            var ud = fx.AddUnitData("使用機");
            var ad = ud.AddAbility("テストアビリティ");
            ad.SetEffect(effect);
            ad.Class = aclass;
            ad.ENConsumption = en;
            ad.MinRange = maxRange == 0 ? 0 : 1;
            ad.MaxRange = maxRange;
            ad.Stock = stock;
            fx.AddUnitData("対象機");
            var user = fx.CreateUnit("使用機", "使用者");
            var target = fx.CreateUnit("対象機", "対象者");
            return (user, target, user.Ability(1));
        }

        [TestMethod]
        public void Heal_RestoresHp500PerLevel_ConsumesEn_AndReturnsUseful()
        {
            // 回復LvN（術でない）は 500×N の HP を回復する
            var (user, target, a) = Create("回復Lv1", en: 10);
            target.HP = 300;

            var useful = user.ExecuteAbility(a, target);

            Assert.IsTrue(useful);
            Assert.AreEqual(800, target.HP);
            Assert.AreEqual(90, user.EN);
            CollectionAssert.Contains(fx.SysMessages.ToList(), "使用機は[対象機]に[テストアビリティ]を使った。");
            Assert.IsTrue(fx.SysMessages.Any(m => m.StartsWith("対象機のＨＰが[500]回復した;残りＨＰは800")));
        }

        [TestMethod]
        public void Heal_IsCappedAtMaxHp()
        {
            var (user, target, a) = Create("回復Lv2");
            target.HP = 900;

            user.ExecuteAbility(a, target);

            Assert.AreEqual(1000, target.HP);
            Assert.IsTrue(fx.SysMessages.Any(m => m.StartsWith("対象機のＨＰが[100]回復した")));
        }

        [TestMethod]
        public void Heal_OnFullHpTarget_IsNotUseful()
        {
            // ＨＰが最大値なら回復処理は行われず有効ではない
            var (user, target, a) = Create("回復Lv1", en: 10);

            var useful = user.ExecuteAbility(a, target);

            Assert.IsFalse(useful);
            Assert.AreEqual(1000, target.HP);
            // ＥＮは使用時点で消費される
            Assert.AreEqual(90, user.EN);
        }

        [TestMethod]
        public void Heal_OnZombieTarget_IsNotUseful()
        {
            // ゾンビ状態のユニットは回復しない
            var (user, target, a) = Create("回復Lv1");
            target.HP = 300;
            target.AddCondition("ゾンビ", 3);

            var useful = user.ExecuteAbility(a, target);

            Assert.IsFalse(useful);
            Assert.AreEqual(300, target.HP);
        }

        [TestMethod]
        public void Heal_RepairSkillIncreasesAmount()
        {
            // 修理技能 Lv2 で回復量は (10+2)/10 倍
            var (user, target, a) = Create("回復Lv1");
            fx.Src.PDList.Item("使用者").AddSkill("修理", 2d, "", 1);
            user.MainPilot().Update();
            target.HP = 100;

            user.ExecuteAbility(a, target);

            Assert.AreEqual(700, target.HP);
        }

        [TestMethod]
        public void Heal_FavoriteTechniqueMultipliesLevelBy1_2()
        {
            // 得意技に該当する属性のアビリティは効果レベルが 1.2 倍
            var (user, target, a) = Create("回復Lv1", aclass: "魔");
            fx.Src.PDList.Item("使用者").AddSkill("得意技", 1d, "魔", 1);
            user.MainPilot().Update();
            target.HP = 100;

            user.ExecuteAbility(a, target);

            Assert.AreEqual(700, target.HP);
        }

        [TestMethod]
        public void NegativeHeal_ReducesHp()
        {
            // 回復Lv-1 は 500 の HP を減少させる（有効扱いにはならない）
            var (user, target, a) = Create("回復Lv-1");

            var useful = user.ExecuteAbility(a, target);

            Assert.IsFalse(useful);
            Assert.AreEqual(500, target.HP);
            Assert.IsTrue(fx.SysMessages.Any(m => m.StartsWith("対象機のＨＰが[500]減少した")));
        }

        [TestMethod]
        public void Supply_RestoresEn50PerLevel()
        {
            var (user, target, a) = Create("補給Lv1");
            target.EN = 20;

            var useful = user.ExecuteAbility(a, target);

            Assert.IsTrue(useful);
            Assert.AreEqual(70, target.EN);
            Assert.IsTrue(fx.SysMessages.Any(m => m == "対象機のＥＮが[50]回復した;残りＥＮは70"));
        }

        [TestMethod]
        public void Supply_OnFullEnTarget_IsNotUseful()
        {
            var (user, target, a) = Create("補給Lv1");

            Assert.IsFalse(user.ExecuteAbility(a, target));
            Assert.AreEqual(100, target.EN);
        }

        [TestMethod]
        public void NegativeSupply_ReducesEn_AndSkipsWhenAlreadyZero()
        {
            var (user, target, a) = Create("補給Lv-1");
            target.EN = 80;

            user.ExecuteAbility(a, target);
            Assert.AreEqual(30, target.EN);

            // ＥＮが 0 のときは何もしない
            target.EN = 0;
            var before = fx.SysMessages.Count();
            user.ExecuteAbility(a, target);
            Assert.AreEqual(0, target.EN);
            Assert.IsFalse(fx.SysMessages.Skip(before).Any(m => m.Contains("減少した")));
        }

        [TestMethod]
        public void MoraleUp_Increases10PerLevel()
        {
            var (user, target, a) = Create("気力増加Lv1");
            var p = target.MainPilot();
            var prev = p.Morale;

            var useful = user.ExecuteAbility(a, target);

            Assert.IsTrue(useful);
            Assert.AreEqual(prev + 10, p.Morale);
            Assert.IsTrue(fx.SysMessages.Any(m => m == "対象者の気力が10増加した。"));
        }

        [TestMethod]
        public void Cure_WithoutData_RemovesAllBadConditions()
        {
            // 治癒（データ指定なし）は全てのステータス異常を回復する
            var (user, target, a) = Create("治癒");
            target.AddCondition("毒", 3);
            target.AddCondition("麻痺", 2);
            target.AddCondition("火属性使用不能", 2);
            target.AddCondition("攻撃力ＵＰ", 2);

            var useful = user.ExecuteAbility(a, target);

            Assert.IsTrue(useful);
            Assert.IsFalse(target.IsConditionSatisfied("毒"));
            Assert.IsFalse(target.IsConditionSatisfied("麻痺"));
            Assert.IsFalse(target.IsConditionSatisfied("火属性使用不能"));
            // ステータス異常でない状態は残る
            Assert.IsTrue(target.IsConditionSatisfied("攻撃力ＵＰ"));
            CollectionAssert.Contains(fx.SysMessages.ToList(), "対象機の状態が回復した。");
        }

        [TestMethod]
        public void Cure_WithData_RemovesOnlySpecifiedConditions()
        {
            var (user, target, a) = Create("治癒=毒");
            target.AddCondition("毒", 3);
            target.AddCondition("麻痺", 2);

            var useful = user.ExecuteAbility(a, target);

            Assert.IsTrue(useful);
            Assert.IsFalse(target.IsConditionSatisfied("毒"));
            Assert.IsTrue(target.IsConditionSatisfied("麻痺"));
            CollectionAssert.Contains(fx.SysMessages.ToList(), "対象機の[毒]が回復した。");
        }

        [TestMethod]
        public void Cure_WithoutConditions_IsNotUseful()
        {
            var (user, target, a) = Create("治癒");
            Assert.IsFalse(user.ExecuteAbility(a, target));
        }

        [TestMethod]
        public void Attach_AddsFeatureConditionWithLifetimeLevelAndData()
        {
            // 付加LvN=能力 は N ターン持続する「能力付加」状態を付ける
            var (user, target, a) = Create("付加Lv3=耐性Lv1=火");

            var useful = user.ExecuteAbility(a, target);

            Assert.IsTrue(useful);
            Assert.AreEqual(3, target.ConditionLifetime("耐性付加"));
            Assert.AreEqual(1d, target.ConditionLevel("耐性付加"));
            Assert.AreEqual("火", target.ConditionData("耐性付加"));
            CollectionAssert.Contains(fx.SysMessages.ToList(), "対象機は[火]属性に対する[耐性]能力を得た。");
        }

        [TestMethod]
        public void Attach_WithoutLevel_LastsSemiPermanently()
        {
            // レベル指定がない付加は 10000 ターン（半永久）持続
            var (user, target, a) = Create("付加=耐性Lv1=火");

            user.ExecuteAbility(a, target);

            Assert.AreEqual(10000, target.ConditionLifetime("耐性付加"));
        }

        [TestMethod]
        public void Attach_AlreadyActive_IsSkipped()
        {
            var (user, target, a) = Create("付加Lv3=耐性Lv1=火");
            target.AddCondition("耐性付加", 5, 2d, "雷");

            var useful = user.ExecuteAbility(a, target);

            Assert.IsFalse(useful);
            Assert.AreEqual(5, target.ConditionLifetime("耐性付加"));
            Assert.AreEqual("雷", target.ConditionData("耐性付加"));
        }

        [TestMethod]
        public void Attach_SelfRangeZero_IsNotUseful()
        {
            // 射程 0（自分自身のみ）の付加は経験値対象の「有効」扱いにならない
            var (user, _, a) = Create("付加Lv3=耐性Lv1=火", maxRange: 0);

            var useful = user.ExecuteAbility(a, user);

            Assert.IsFalse(useful);
            Assert.AreEqual(3, user.ConditionLifetime("耐性付加"));
        }

        [TestMethod]
        public void Enhance_AddsSkillEnhanceCondition()
        {
            // 強化LvN=技能Lvx は N ターン持続する「技能強化」状態を付ける
            var (user, target, a) = Create("強化Lv2=格闘Lv1");

            var useful = user.ExecuteAbility(a, target);

            Assert.IsTrue(useful);
            Assert.AreEqual(2, target.ConditionLifetime("格闘強化"));
            Assert.AreEqual(1d, target.ConditionLevel("格闘強化"));
            Assert.IsTrue(fx.SysMessages.Any(m => m.StartsWith("対象機の[") && m.EndsWith("]レベルが1上がった。")));
        }

        [TestMethod]
        public void State_AddsConditionForLevelTurns()
        {
            var (user, target, a) = Create("状態Lv3=攻撃力ＵＰ");

            var useful = user.ExecuteAbility(a, target);

            Assert.IsTrue(useful);
            Assert.AreEqual(3, target.ConditionLifetime("攻撃力ＵＰ"));
            CollectionAssert.Contains(fx.SysMessages.ToList(), "対象機は攻撃力ＵＰの状態になった。");
        }

        [TestMethod]
        public void State_ArmorDegrade_UsesTermInMessage()
        {
            var (user, target, a) = Create("状態Lv2=装甲劣化");

            user.ExecuteAbility(a, target);

            Assert.AreEqual(2, target.ConditionLifetime("装甲劣化"));
            CollectionAssert.Contains(fx.SysMessages.ToList(), "対象機は装甲劣化の状態になった。");
        }

        [TestMethod]
        public void ReAction_OnOtherUnit_MakesItActionable()
        {
            // 再行動: 行動済みの他ユニットを行動可能にする
            var (user, target, a) = Create("再行動");
            target.UsedAction = target.MaxAction();
            Assert.AreEqual(0, target.Action);

            var useful = user.ExecuteAbility(a, target);

            Assert.IsTrue(useful);
            Assert.AreEqual(1, target.Action);
            CollectionAssert.Contains(fx.SysMessages.ToList(), "対象機を行動可能にした。");
        }

        [TestMethod]
        public void ReAction_OnUnitThatCanStillAct_IsNotUseful()
        {
            var (user, target, a) = Create("再行動");
            target.UsedAction = 0;

            Assert.IsFalse(user.ExecuteAbility(a, target));
            Assert.AreEqual(0, target.UsedAction);
        }

        [TestMethod]
        public void ReAction_OnSelf_DecrementsUsedAction()
        {
            // 自分への再行動は使用した行動回数を 1 戻す
            var (user, _, a) = Create("再行動", maxRange: 0);
            user.UsedAction = 1;

            user.ExecuteAbility(a, user);

            Assert.AreEqual(0, user.UsedAction);
        }

        [TestMethod]
        public void Difficulty10_AlwaysFails_ButConsumesEn()
        {
            // 難L10 は Dice(10) <= 10 が必ず成立するため常に失敗する
            var (user, target, a) = Create("回復Lv1", aclass: "難L10", en: 10);
            target.HP = 300;

            var useful = user.ExecuteAbility(a, target);

            Assert.IsFalse(useful);
            Assert.AreEqual(300, target.HP);
            Assert.AreEqual(90, user.EN);
            CollectionAssert.Contains(fx.SysMessages.ToList(), "しかし何もおきなかった…");
        }

        [TestMethod]
        public void MoraleDrainClass_Reduces10Morale()
        {
            // 「脱」属性は対象の気力を 10 下げる
            var (user, target, a) = Create("回復Lv1", aclass: "脱");
            var p = target.MainPilot();
            var prev = p.Morale;

            user.ExecuteAbility(a, target);

            Assert.AreEqual(prev - 10, p.Morale);
        }

        [TestMethod]
        public void RemoveClass_DeletesEnhancementsExceptNormalMode()
        {
            // 「除」属性は付加・強化・ＵＰ状態を解除する（ノーマルモード付加は除く）
            var (user, target, a) = Create("回復Lv1", aclass: "除");
            target.AddCondition("耐性付加", 3, 1d, "火");
            target.AddCondition("格闘強化", 3, 1d, "");
            target.AddCondition("攻撃力ＵＰ", 3);
            target.AddCondition("ノーマルモード付加", -1, 1d, "対象機");
            target.AddCondition("毒", 3);

            user.ExecuteAbility(a, target);

            Assert.IsFalse(target.IsConditionSatisfied("耐性付加"));
            Assert.IsFalse(target.IsConditionSatisfied("格闘強化"));
            Assert.IsFalse(target.IsConditionSatisfied("攻撃力ＵＰ"));
            Assert.IsTrue(target.IsConditionSatisfied("ノーマルモード付加"));
            Assert.IsTrue(target.IsConditionSatisfied("毒"));
        }

        [TestMethod]
        public void SpellAbility_MessageSaysCast()
        {
            // 術属性のアビリティは「～の呪文を唱えた。」と表示する
            var (user, target, a) = Create("回復Lv1", aclass: "術");
            target.HP = 300;

            user.ExecuteAbility(a, target);

            CollectionAssert.Contains(fx.SysMessages.ToList(), "使用機は[対象機]に[テストアビリティ]の呪文を唱えた。");
        }

        [TestMethod]
        public void SpellHeal_AmountDependsOnShooting()
        {
            // 術の回復量は 5×レベル×射撃
            var (user, target, a) = Create("回復Lv1", aclass: "術");
            target.HP = 1;
            var shooting = user.MainPilot().Shooting;

            user.ExecuteAbility(a, target);

            Assert.AreEqual(System.Math.Min(1000, 1 + 5 * shooting), target.HP);
        }

        [TestMethod]
        public void UsefulAbility_GivesExpToPilot_UnlessEvent()
        {
            var (user, target, a) = Create("回復Lv1");
            var p = user.MainPilot();
            target.HP = 100;
            var exp0 = p.Exp;

            user.ExecuteAbility(a, target, is_event: true);
            Assert.AreEqual(exp0, p.Exp, "イベントからの使用では経験値を得ない");

            target.HP = 100;
            user.ExecuteAbility(a, target);
            Assert.IsTrue(p.Exp > exp0);
        }

        [TestMethod]
        public void SelectionsAreRestoredAfterExecution()
        {
            // 実行中に切り替えた選択状況は終了時に復元される
            var (user, target, a) = Create("回復Lv1");
            fx.Src.Commands.SelectedUnit = target;
            fx.Src.Commands.SelectedTarget = null;

            user.ExecuteAbility(a, target);

            Assert.AreSame(target, fx.Src.Commands.SelectedUnit);
            Assert.IsNull(fx.Src.Commands.SelectedTarget);
        }
    }
}
