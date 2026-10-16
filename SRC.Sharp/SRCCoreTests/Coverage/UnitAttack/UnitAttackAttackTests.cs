using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitAttack
{
    /// <summary>
    /// Unit.Attack（Unit.attack.cs / VB: Unit.cls Attack）の命中・ダメージ・破壊・各種属性処理のテスト。
    /// 乱数は FixDice(0.5) で固定する（Dice(100)=51, Dice(32)=17, Dice(16)=9）。
    /// 武器の命中補正 1000 で命中率は 1100、命中補正 p の場合は 100+p になる。
    /// </summary>
    [TestClass]
    public class UnitAttackAttackTests
    {
        private System.IDisposable dice;

        [TestInitialize]
        public void Init()
        {
            dice = UnitAttackFixture.FixDice(0.5);
        }

        [TestCleanup]
        public void Cleanup()
        {
            dice.Dispose();
        }

        private static (UnitAttackFixture f, Units.Unit a, Units.Unit t) Setup(
            string wname = "ビームライフル", int power = 1500, string wclass = "",
            int targetHp = 5000, int precision = 1000, int critical = -1000,
            int bullet = 0, int en = 0,
            string[] attackerFeatures = null, string[] targetFeatures = null,
            (string, double)[] attackerSkills = null, (string, double)[] targetSkills = null,
            int attackerHp = 1000)
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 2, 2, hp: attackerHp,
                weapons: new[] { f.Weapon(wname, power, wclass, 1, 3, bullet, en, precision, critical) },
                features: attackerFeatures, skills: attackerSkills);
            var t = f.AddUnit("標的機", "敵", 3, 2, hp: targetHp, features: targetFeatures, skills: targetSkills);
            return (f, a, t);
        }

        // 命中時は Damage() のダメージがそのまま適用され、攻撃メッセージとダメージメッセージが表示される
        [TestMethod]
        public void Attack_Hit_AppliesDamageAndShowsMessages()
        {
            var (f, a, t) = Setup();
            var w = a.Weapons.First();
            var expected = w.Damage(t, true);

            a.Attack(w, t, "", "");

            Assert.AreEqual(5000 - expected, t.HP);
            var msgs = f.SysMessages();
            Assert.AreEqual("攻撃機は[ビームライフル]で攻撃をかけた。;命中率 = 100％（1％）", msgs[0]);
            Assert.AreEqual("標的機は[" + expected + "]のダメージを受けた。;残りＨＰは" + (5000 - expected)
                + "（損傷率 = " + (100 * expected / 5000) + "％）", msgs.Last());
            // ダメージ量は標的の位置に表示される
            var draw = f.Calls(nameof(IGUI.DrawSysString)).Single();
            Assert.AreEqual(3, draw.Args[0]);
            Assert.AreEqual(2, draw.Args[1]);
            Assert.AreEqual(expected.ToString(), draw.Args[2]);
        }

        // 攻撃が命中すると標的の気力が +1 される（攻撃側は変化しない）
        [TestMethod]
        public void Attack_Hit_IncreasesTargetMoraleByOne()
        {
            var (_, a, t) = Setup();
            a.Attack(a.Weapons.First(), t, "", "");
            Assert.AreEqual(101, t.MainPilot().Morale);
            Assert.AreEqual(100, a.MainPilot().Morale);
        }

        // 命中時気力増加／損傷時気力増加はスキルレベル分だけ気力を上げる
        [TestMethod]
        public void Attack_Hit_MoraleSkillsAddLevel()
        {
            var (_, a, t) = Setup(attackerSkills: new[] { ("命中時気力増加", 3d) },
                targetSkills: new[] { ("損傷時気力増加", 2d) });
            a.Attack(a.Weapons.First(), t, "", "");
            Assert.AreEqual(103, a.MainPilot().Morale);
            // 被弾 +1 と損傷時気力増加 +2
            Assert.AreEqual(103, t.MainPilot().Morale);
        }

        // 防御態勢ではダメージが半減し、防御行動のメッセージが前置される
        [TestMethod]
        public void Attack_DefenseMode_HalvesDamage()
        {
            var (f, a, t) = Setup();
            var w = a.Weapons.First();
            var full = w.Damage(t, true);

            a.Attack(w, t, "", "防御");

            Assert.AreEqual(5000 - full / 2, t.HP);
            StringAssert.StartsWith(f.SysMessages().Last(), "標的機は防御行動をとった。;標的機は[" + (full / 2) + "]のダメージを受けた。");
        }

        // 抹殺攻撃（殺）は防御で半減しないが、一撃で倒せない場合は効果が無い
        [TestMethod]
        public void Attack_Kill_NoEffectWhenTargetSurvives()
        {
            var (f, a, t) = Setup(wclass: "殺");
            a.Attack(a.Weapons.First(), t, "", "");
            Assert.AreEqual(5000, t.HP);
            Assert.AreEqual("標的機は攻撃による影響を受けなかった。", f.SysMessages().Last());
        }

        [TestMethod]
        public void Attack_Kill_DestroysWhenDamageExceedsHp()
        {
            var (f, a, t) = Setup(wclass: "殺", targetHp: 1000);
            a.Attack(a.Weapons.First(), t, "", "");
            Assert.AreEqual(0, t.HP);
            Assert.AreEqual("破壊", t.Status);
        }

        // 回避態勢では命中率が半減する（60％→30％ で Dice(100)=51 は外れ）
        [TestMethod]
        public void Attack_EvadeMode_HalvesHitRateAndMisses()
        {
            var (f, a, t) = Setup(precision: -40);
            Assert.AreEqual(60, a.Weapons.First().HitProbability(t, true));

            a.Attack(a.Weapons.First(), t, "", "回避");

            Assert.AreEqual(5000, t.HP);
            var msgs = f.SysMessages();
            StringAssert.EndsWith(msgs[0], "命中率 = 30％（1％）");
            Assert.AreEqual("標的機は回避運動をとり、攻撃をかわした。", msgs.Last());
            // 外れた場合は標的の気力は上がらない
            Assert.AreEqual(100, t.MainPilot().Morale);
        }

        // 同じ命中率でも通常態勢なら命中する（60 >= 51）
        [TestMethod]
        public void Attack_NormalMode_SameRateHits()
        {
            var (_, a, t) = Setup(precision: -40);
            a.Attack(a.Weapons.First(), t, "", "");
            Assert.IsTrue(t.HP < 5000);
        }

        [TestMethod]
        [DataRow("", "標的機は攻撃をかわした。")]
        [DataRow("防御", "標的機は防御行動をとったが、攻撃は外れた。")]
        public void Attack_Miss_MessageDependsOnDefenseMode(string defMode, string expected)
        {
            var (f, a, t) = Setup(precision: -60);
            a.Attack(a.Weapons.First(), t, "", defMode);
            Assert.AreEqual(5000, t.HP);
            Assert.AreEqual(expected, f.SysMessages().Last());
        }

        // 失敗時気力増加は攻撃側、回避時気力増加は標的側の気力を上げる
        [TestMethod]
        public void Attack_Miss_MoraleSkillsAddLevel()
        {
            var (_, a, t) = Setup(precision: -60,
                attackerSkills: new[] { ("失敗時気力増加", 2d) },
                targetSkills: new[] { ("回避時気力増加", 4d) });
            a.Attack(a.Weapons.First(), t, "", "");
            Assert.AreEqual(102, a.MainPilot().Morale);
            Assert.AreEqual(104, t.MainPilot().Morale);
        }

        // 武器使用で弾数とＥＮを消費する
        [TestMethod]
        public void Attack_ConsumesBulletAndEn()
        {
            var (_, a, t) = Setup(bullet: 3, en: 10);
            var w = a.Weapons.First();
            a.Attack(w, t, "", "");
            Assert.AreEqual(2, w.Bullet());
            Assert.AreEqual(90, a.EN);
        }

        // クリティカル時はダメージ 1.5 倍
        [TestMethod]
        public void Attack_Critical_MultipliesDamageBy1_5()
        {
            var (f, a, t) = Setup(critical: 1000);
            var w = a.Weapons.First();
            var basic = w.Damage(t, true);

            a.Attack(w, t, "", "");

            var dmg = (int)System.Math.Round(1.5 * basic, System.MidpointRounding.ToEven);
            Assert.AreEqual(5000 - dmg, t.HP);
            StringAssert.StartsWith(f.SysMessages().Last(), "クリティカル！;標的機は[" + dmg + "]のダメージを受けた。");
        }

        // オプション「ダメージ倍率低下」ではクリティカル倍率が 1.2 倍になる
        [TestMethod]
        public void Attack_Critical_ReducedMultiplierOption()
        {
            var (f, a, t) = Setup(critical: 1000);
            f.Src.Expression.DefineGlobalVariable("Option(ダメージ倍率低下)");
            var w = a.Weapons.First();
            var basic = w.Damage(t, true);

            a.Attack(w, t, "", "");

            var dmg = (int)System.Math.Round(1.2 * basic, System.MidpointRounding.ToEven);
            Assert.AreEqual(5000 - dmg, t.HP);
        }

        // 痛属性のクリティカルは (1 + 0.25 * (レベル + 2)) 倍
        [TestMethod]
        public void Attack_Critical_PainLevelMultiplier()
        {
            var (_, a, t) = Setup(wclass: "痛L2", critical: 1000);
            var w = a.Weapons.First();
            var basic = w.Damage(t, true);

            a.Attack(w, t, "", "");

            Assert.AreEqual(5000 - 2 * basic, t.HP);
        }

        // 連属性はレベル回数だけ命中判定を行い、命中回数をメッセージに含める
        [TestMethod]
        public void Attack_MultiHit_ReportsHitCount()
        {
            var (f, a, t) = Setup(wclass: "連L3");
            var w = a.Weapons.First();
            var expected = w.Damage(t, true);

            a.Attack(w, t, "", "");

            Assert.AreEqual(5000 - expected, t.HP);
            StringAssert.StartsWith(f.SysMessages().Last(), "3回命中し、標的機は[" + expected + "]のダメージを受けた。");
        }

        // 標的を破壊した場合：破壊メッセージ、ステータス変化、マップからの除去
        [TestMethod]
        public void Attack_Destroy_RemovesTarget()
        {
            var (f, a, t) = Setup(targetHp: 500);
            a.Attack(a.Weapons.First(), t, "", "");

            Assert.AreEqual(0, t.HP);
            Assert.AreEqual("破壊", t.Status);
            Assert.IsNull(f.Src.Map.MapDataForUnit[3, 2]);
            // ダメージは残りＨＰで頭打ちになる
            Assert.IsTrue(f.SysMessages().Contains("標的機は[500]のダメージを受け破壊された。"));
        }

        // 破壊時の気力変動：攻撃側 +4（+3 と陣営 +1）、同陣営 +1（機械は除く）、破壊された側の陣営は性格に応じて変化
        [TestMethod]
        public void Attack_Destroy_ChangesMoraleByPartyAndPersonality()
        {
            var (f, a, t) = Setup(targetHp: 500);
            var ally = f.AddUnit("僚機", "味方", 8, 8);
            var machine = f.AddUnit("無人機", "味方", 8, 9, personality: "機械");
            var bold = f.AddUnit("強気機", "敵", 9, 8, personality: "強気");
            var superBold = f.AddUnit("超強気機", "敵", 9, 9, personality: "超強気");
            var timid = f.AddUnit("弱気機", "敵", 9, 10, personality: "弱気");
            var normal = f.AddUnit("普通機", "敵", 10, 10);

            a.Attack(a.Weapons.First(), t, "", "");

            Assert.AreEqual(104, a.MainPilot().Morale);
            Assert.AreEqual(101, ally.MainPilot().Morale);
            Assert.AreEqual(100, machine.MainPilot().Morale);
            Assert.AreEqual(101, bold.MainPilot().Morale);
            Assert.AreEqual(102, superBold.MainPilot().Morale);
            Assert.AreEqual(99, timid.MainPilot().Morale);
            Assert.AreEqual(100, normal.MainPilot().Morale);
        }

        // オプション「破壊時味方気力変化５倍」は味方陣営の性格による変化量を 5 倍にする
        [TestMethod]
        public void Attack_Destroy_AllyMoraleFiveTimesOption()
        {
            var f = new UnitAttackFixture();
            f.Src.Expression.DefineGlobalVariable("Option(破壊時味方気力変化５倍)");
            var a = f.AddUnit("攻撃機", "敵", 2, 2, weapons: new[] { f.Weapon("ビームライフル", 1500, "", 1, 3) });
            var t = f.AddUnit("標的機", "味方", 3, 2, hp: 500);
            var bold = f.AddUnit("強気機", "味方", 9, 8, personality: "強気");

            a.Attack(a.Weapons.First(), t, "", "");

            Assert.AreEqual("破壊", t.Status);
            Assert.AreEqual(105, bold.MainPilot().Morale);
        }

        // 英雄：Dice(16) がスキルレベル以下なら最大ＨＰの半分で復活する
        [TestMethod]
        public void Attack_Destroy_HeroSkillResurrects()
        {
            var (f, a, t) = Setup(targetHp: 500, targetSkills: new[] { ("英雄", 9d) });
            a.Attack(a.Weapons.First(), t, "", "");

            Assert.AreEqual("出撃", t.Status);
            Assert.AreEqual(250, t.HP);
            // 復活時は気力 +10（被弾 +1 と合わせて +11）
            Assert.AreEqual(111, t.MainPilot().Morale);
            Assert.IsTrue(f.SysMessages().Contains("標的機パイロットの熱き英雄の心が[標的機]を復活させた！"));
        }

        [TestMethod]
        public void Attack_Destroy_HeroSkillBelowDiceDoesNotResurrect()
        {
            var (_, a, t) = Setup(targetHp: 500, targetSkills: new[] { ("英雄", 8d) });
            a.Attack(a.Weapons.First(), t, "", "");
            Assert.AreEqual("破壊", t.Status);
        }

        // イベントによる攻撃（Attack コマンド）では英雄による復活は起こらない
        [TestMethod]
        public void Attack_Destroy_EventAttackSkipsHero()
        {
            var (_, a, t) = Setup(targetHp: 500, targetSkills: new[] { ("英雄", 16d) });
            a.Attack(a.Weapons.First(), t, "", "", is_event: true);
            Assert.AreEqual("破壊", t.Status);
        }

        // 再生：Dice(16) がスキルレベル以下なら復活する
        [TestMethod]
        public void Attack_Destroy_RegenerationSkillResurrects()
        {
            var (f, a, t) = Setup(targetHp: 500, targetSkills: new[] { ("再生", 9d) });
            a.Attack(a.Weapons.First(), t, "", "");

            Assert.AreEqual("出撃", t.Status);
            Assert.AreEqual(250, t.HP);
            Assert.IsTrue(f.SysMessages().Contains("標的機は再生の力で一瞬にして復活した！"));
        }

        // 浄属性の武器は浄化を持つパイロットがいれば再生による復活を防ぐ
        [TestMethod]
        public void Attack_Destroy_PurifyPreventsRegeneration()
        {
            var (f, a, t) = Setup(wclass: "浄", targetHp: 500,
                attackerSkills: new[] { ("浄化", 1d) },
                targetSkills: new[] { ("再生", 16d) });
            a.Attack(a.Weapons.First(), t, "", "");

            Assert.AreEqual("破壊", t.Status);
            Assert.IsTrue(f.SysMessages().Contains("攻撃機パイロットは浄化を行って[標的機]の復活を防いだ。"));
        }

        // 不死身状態のユニットはＨＰ10 を残して耐える
        [TestMethod]
        public void Attack_Immortal_LeavesTenHp()
        {
            var (_, a, t) = Setup(targetHp: 500);
            t.AddCondition("不死身", -1);
            a.Attack(a.Weapons.First(), t, "", "");
            Assert.AreEqual(10, t.HP);
            Assert.AreEqual("出撃", t.Status);
        }

        // 無敵状態のユニットは攻撃を無効化する
        [TestMethod]
        public void Attack_Invincible_NullifiesAttack()
        {
            var (f, a, t) = Setup();
            t.AddCondition("無敵", 1);
            a.Attack(a.Weapons.First(), t, "", "");
            Assert.AreEqual(5000, t.HP);
            Assert.AreEqual("標的機は[ビームライフル]を無効化した！", f.SysMessages().Last());
        }

        // ダメージを受けると睡眠・凍結が解除される
        [TestMethod]
        [DataRow("睡眠", "標的機は眠りから覚めた。")]
        [DataRow("凍結", "標的機は凍結状態から開放された。")]
        public void Attack_Damage_ClearsSleepAndFreeze(string condition, string message)
        {
            var (f, a, t) = Setup();
            t.AddCondition(condition, 3);
            a.Attack(a.Weapons.First(), t, "", "");
            Assert.IsFalse(t.IsConditionSatisfied(condition));
            Assert.IsTrue(f.SysMessages().Contains(message));
        }

        // 眠属性の武器では睡眠は解除されない
        [TestMethod]
        public void Attack_SleepWeapon_DoesNotWakeTarget()
        {
            var (_, a, t) = Setup(wclass: "眠");
            t.AddCondition("睡眠", 3);
            a.Attack(a.Weapons.First(), t, "", "");
            Assert.IsTrue(t.IsConditionSatisfied("睡眠"));
        }

        // 不安定：ＨＰが 1/4 以下になると暴走状態になる
        [TestMethod]
        public void Attack_Unstable_GoesBerserkBelowQuarterHp()
        {
            var (f, a, t) = Setup(targetHp: 2000, targetFeatures: new[] { "不安定" });
            var w = a.Weapons.First();
            Assert.IsTrue(2000 - w.Damage(t, true) <= 500);

            a.Attack(w, t, "", "");

            Assert.IsTrue(t.IsConditionSatisfied("暴走"));
            Assert.IsTrue(f.SysMessages().Any(m => m.StartsWith("標的機は") && m.Contains("制御不能に陥った。")));
        }

        // 吸属性：攻撃側のＨＰが減っていれば与えたダメージの 1/4 を吸収する
        [TestMethod]
        public void Attack_DrainHp_RecoversQuarterOfDamage()
        {
            var (f, a, t) = Setup(wclass: "吸");
            a.HP = 500;
            var w = a.Weapons.First();
            var dmg = w.Damage(t, true);

            a.Attack(w, t, "", "");

            Assert.AreEqual(500 + dmg / 4, a.HP);
            StringAssert.StartsWith(f.SysMessages().Last(), "攻撃機は[標的機]のＨＰを吸収した。;");
        }

        // 減属性：ダメージ割合に応じて標的のＥＮを減らす
        [TestMethod]
        public void Attack_ReduceEn_LowersTargetEnByDamageRatio()
        {
            var (_, a, t) = Setup(wclass: "減");
            var w = a.Weapons.First();
            var dmg = w.Damage(t, true);

            a.Attack(w, t, "", "");

            Assert.AreEqual((int)System.Math.Round(100 - 100 * (dmg / 5000d), System.MidpointRounding.ToEven), t.EN);
        }

        // 奪属性：減らしたＥＮの半分を攻撃側が得る
        [TestMethod]
        public void Attack_StealEn_GivesHalfToAttacker()
        {
            var (_, a, t) = Setup(wclass: "奪", en: 30);
            var w = a.Weapons.First();
            var dmg = w.Damage(t, true);

            a.Attack(w, t, "", "");

            var targetEn = (int)System.Math.Round(100 - 100 * (dmg / 5000d), System.MidpointRounding.ToEven);
            Assert.AreEqual(targetEn, t.EN);
            Assert.AreEqual(70 + (100 - targetEn) / 2, a.EN);
        }

        // 再属性：Dice(16) が属性レベル以下なら同じ標的に再攻撃する
        [TestMethod]
        public void Attack_ReattackClass_AttacksTwice()
        {
            var (f, a, t) = Setup(wclass: "再L9");
            var w = a.Weapons.First();
            var dmg = w.Damage(t, true);

            a.Attack(w, t, "", "");

            Assert.AreEqual(5000 - 2 * dmg, t.HP);
            Assert.IsTrue(f.SysMessages().Contains("攻撃機は再度[ビームライフル]で攻撃をかけた。;命中率 = 100％（1％）"));
        }

        [TestMethod]
        public void Attack_ReattackClassBelowDice_AttacksOnce()
        {
            var (_, a, t) = Setup(wclass: "再L8");
            var w = a.Weapons.First();
            var dmg = w.Damage(t, true);
            a.Attack(w, t, "", "");
            Assert.AreEqual(5000 - dmg, t.HP);
        }

        // 再攻撃スキル：直感が相手以上ならレベルの 2 倍が Dice(32) 以上で再攻撃
        [TestMethod]
        public void Attack_ReattackSkill_AttacksTwice()
        {
            var (_, a, t) = Setup(attackerSkills: new[] { ("再攻撃", 9d) });
            var w = a.Weapons.First();
            var dmg = w.Damage(t, true);
            a.Attack(w, t, "", "");
            Assert.AreEqual(5000 - 2 * dmg, t.HP);
        }

        // 武器名に応じて攻撃メッセージの言い回しが変わる
        [TestMethod]
        [DataRow("ミサイル", "実", "攻撃機は[ミサイル]を発射した。;")]
        [DataRow("ファイアブレス", "", "攻撃機は[ファイアブレス]を放った。;")]
        [DataRow("体当たり攻撃", "突", "攻撃機は[体当たり攻撃]をかけた。;")]
        [DataRow("魔法の歌", "", "攻撃機は[魔法の歌]を歌った。;")]
        [DataRow("剣の踊り", "", "攻撃機は[剣の踊り]を踊った。;")]
        public void Attack_AttackMessage_DependsOnWeaponName(string wname, string wclass, string expectedPrefix)
        {
            var (f, a, t) = Setup(wname: wname, wclass: wclass);
            a.Attack(a.Weapons.First(), t, "", "");
            StringAssert.StartsWith(f.SysMessages()[0], expectedPrefix);
        }

        // カウンター攻撃では「先制攻撃！」で始まり先手を取ったことを示す
        [TestMethod]
        public void Attack_CounterMode_ShowsPreemptiveMessage()
        {
            var (f, a, t) = Setup();
            a.Attack(a.Weapons.First(), t, "カウンター", "");
            StringAssert.StartsWith(f.SysMessages()[0], "先制攻撃！;攻撃機は先手を取り[ビームライフル]で攻撃をかけた。;");
        }

        // ステルスを持つユニットは攻撃するとステルス無効状態になる
        [TestMethod]
        public void Attack_Stealth_BecomesRevealed()
        {
            var (_, a, t) = Setup(attackerFeatures: new[] { "ステルス" });
            a.Attack(a.Weapons.First(), t, "", "");
            Assert.IsTrue(a.IsConditionSatisfied("ステルス無効"));
        }

        // 攻撃終了後は攻撃前の選択状況（SelectedUnit 等）に戻る
        [TestMethod]
        public void Attack_RestoresSelections()
        {
            var (f, a, t) = Setup();
            var other = f.AddUnit("別機", "味方", 8, 8);
            f.Src.Commands.SelectedUnit = other;
            f.Src.Commands.SelectedTarget = null;
            f.Src.Commands.SelectedWeapon = 5;

            a.Attack(a.Weapons.First(), t, "", "");

            Assert.AreSame(other, f.Src.Commands.SelectedUnit);
            Assert.IsNull(f.Src.Commands.SelectedTarget);
            Assert.AreEqual(5, f.Src.Commands.SelectedWeapon);
        }

        // 反射による攻撃は命中率が半減し、弾数を消費しない
        [TestMethod]
        public void Attack_ReflectMode_HalvesHitRateWithoutConsumingBullet()
        {
            var (f, a, t) = Setup(precision: -40, bullet: 3);
            var w = a.Weapons.First();
            a.Attack(w, t, "反射", "");
            // 60％→30％ で外れる
            Assert.AreEqual(5000, t.HP);
            Assert.AreEqual(3, w.Bullet());
        }

        private static (UnitAttackFixture f, Units.Unit a, Units.Unit t, Units.Unit[] partners) SetupCombination(params string[] partnerNames)
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 2, 2,
                weapons: new[] { f.Weapon("合体剣", 1500, "合", 1, 3, bullet: 3) },
                features: new[] { "合体技=合体剣 " + string.Join(" ", partnerNames) });
            var partners = partnerNames.Select((n, i) => f.AddUnit(n, "味方", 5 + i, 8,
                weapons: new[] { f.Weapon("合体剣", 1500, "合", 1, 3, bullet: 3) },
                features: new[] { "合体技=合体剣 攻撃機" })).ToArray();
            var t = f.AddUnit("標的機", "敵", 3, 2, hp: 5000);
            return (f, a, t, partners);
        }

        // 合体技：パートナー名を含む攻撃メッセージを表示し、自分とパートナーの弾数を消費する
        [TestMethod]
        public void Attack_Combination_OnePartner_MessageAndConsumption()
        {
            var (f, a, t, partners) = SetupCombination("僚機");
            a.Attack(a.Weapons.First(), t, "", "");

            StringAssert.StartsWith(f.SysMessages()[0], "攻撃機は[僚機]と共に[合体剣]で攻撃をかけた。;");
            Assert.IsTrue(t.HP < 5000);
            Assert.AreEqual(2, a.Weapons.First().Bullet());
            Assert.AreEqual(2, partners[0].Weapons.First().Bullet());
        }

        [TestMethod]
        public void Attack_Combination_TwoPartners_Message()
        {
            var (f, a, t, _) = SetupCombination("僚機Ａ", "僚機Ｂ");
            a.Attack(a.Weapons.First(), t, "", "");
            StringAssert.StartsWith(f.SysMessages()[0], "攻撃機は[僚機Ａ]、[僚機Ｂ]と共に[合体剣]で攻撃をかけた。;");
        }

        // VB の Attack ではパートナーごとに同名武器を探し（Exit For は武器ループのみ抜ける）、全パートナーの弾数を消費する
        [TestMethod]
        public void Attack_Combination_TwoPartners_ConsumesEveryPartner()
        {
            var (_, a, t, partners) = SetupCombination("僚機Ａ", "僚機Ｂ");
            a.Attack(a.Weapons.First(), t, "", "");
            Assert.AreEqual(2, partners[0].Weapons.First().Bullet());
            Assert.AreEqual(2, partners[1].Weapons.First().Bullet());
        }

        // 攻撃力 0 の武器が命中しても効果が無かった場合のメッセージ
        [TestMethod]
        public void Attack_ZeroPower_NoEffectMessage()
        {
            var (f, a, t) = Setup(power: 0);
            a.Attack(a.Weapons.First(), t, "", "");
            Assert.AreEqual(5000, t.HP);
            Assert.AreEqual("標的機は攻撃による影響を受けなかった。", f.SysMessages().Last());
        }

        // 盗属性：盗みに失敗した場合、すかんぴんかどうかでメッセージが変わる
        [TestMethod]
        [DataRow(false, "標的機は素早く持ち物を守った。")]
        [DataRow(true, "標的機は盗める物を持っていなかった。")]
        public void Attack_StealFailed_Message(bool penniless, string expected)
        {
            var (f, a, t) = Setup(wname: "スティール", power: 0, wclass: "盗");
            if (penniless)
            {
                t.AddCondition("すかんぴん", -1);
            }
            a.Attack(a.Weapons.First(), t, "", "");
            Assert.AreEqual(expected, f.SysMessages().Last());
            StringAssert.StartsWith(f.SysMessages()[0], "攻撃機は[標的機]の持ち物を盗もうとした。;");
        }

        // 習属性：習得可能な技が無い場合のメッセージ
        [TestMethod]
        public void Attack_LearnFailed_NoLearnableSkill()
        {
            var (f, a, t) = Setup(wname: "ラーニング", power: 0, wclass: "習");
            a.Attack(a.Weapons.First(), t, "", "");
            StringAssert.StartsWith(f.SysMessages()[0], "攻撃機は[標的機]の技を習得しようと試みた。;");
            Assert.AreEqual("標的機は習得可能な技を持っていなかった。", f.SysMessages().Last());
        }

        // 自属性：攻撃後に攻撃側は破壊される
        [TestMethod]
        public void Attack_SelfDestruct_DestroysAttacker()
        {
            var (_, a, t) = Setup(wname: "自爆", wclass: "自");
            a.Attack(a.Weapons.First(), t, "", "");
            Assert.IsTrue(t.HP < 5000);
            Assert.AreEqual("破壊", a.Status);
        }

        // 破壊された標的がパーツ分離を持つ場合は別形態に変化してＨＰ全快で残る
        [TestMethod]
        public void Attack_Destroy_PartsSeparation()
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 2, 2, weapons: new[] { f.Weapon("ビームライフル", 1500, "", 1, 3) });
            var core = f.Src.UDList.Add("標的機コア");
            core.Nickname = "標的機コア";
            core.Transportation = "陸";
            core.Adaption = "AAAA";
            core.HP = 800;
            core.EN = 100;
            core.Size = "M";
            core.PilotNum = 1;
            core.Class = "汎用";
            var t = f.AddUnit("標的機", "敵", 3, 2, hp: 500, features: new[] { "パーツ分離=分離 標的機コア" });

            a.Attack(a.Weapons.First(), t, "", "");

            var cf = t.CurrentForm();
            Assert.AreEqual("標的機コア", cf.Name);
            Assert.AreEqual("出撃", cf.Status);
            Assert.AreEqual(800, cf.HP);
            Assert.IsTrue(f.SysMessages().Contains("標的機は破壊されたパーツを分離させた。"));
        }
    }
}
