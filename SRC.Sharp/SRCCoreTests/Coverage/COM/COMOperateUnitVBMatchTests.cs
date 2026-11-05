using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using SRCCore.Filesystem;
using SRCCore.Lib;
using SRCCore.Units;
using System;
using System.Linq;

namespace SRCCore.Coverage.Tests.COMTests
{
    /// <summary>
    /// COM.OperateUnit を VB 版（COM.bas の OperateUnit）と突き合わせたときに見つかった違いのテスト。
    /// 期待値は元の SRC の動作から取っている。
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class COMOperateUnitVBMatchTests
    {
        private static COMTestField CreateField(bool autoDefense = true)
        {
            var f = new COMTestField();
            // 戦闘アニメは使わない。味方の反撃・防御は既定では自動反撃モードで自動で選ぶ
            f.Src.SystemConfig = new LocalFileConfig { BattleAnimation = false, WeaponAnimation = false, ExtendedAnimation = false, AutoDefense = autoDefense };
            f.Src.ScenarioConfig = null;
            f.Src.GUI.MessageWait = 0;
            // ファイルを持たない空のファイルシステム（効果音などは見つからない扱い）
            f.Src.FileSystem = new LocalFileSystem();
            // イベントは無い（戦闘の前後のイベントの呼び出しが空振りするだけ）
            f.Src.Event.InitEventData();
            return f;
        }

        private static void Operate(COMTestField f, Unit u)
        {
            f.Src.Commands.SelectedUnit = u;
            f.Src.COM.OperateUnit();
        }

        // 戦術が 150 以上の敵（技量 200・レベル 1）を (5, 5) に置く
        private static Unit DeployTacticalAttacker(COMTestField f, string wclass)
        {
            var ud = f.DefineUnit("砲台", speed: 0);
            COMTestField.AddWeapon(ud, "砲", 2000, minRange: 1, maxRange: 4, wclass: wclass);
            f.DefinePilot("指揮官", technique: 200);
            var u = f.Deploy("砲台", "指揮官", "敵", 5, 5);
            Assert.IsTrue(u.MainPilot().TacticalTechnique() >= 150);
            return u;
        }

        private static void DefineTargetUnit(COMTestField f, string name, string abilityEffects = null, int armor = 1000, int counterRange = 1)
        {
            var ud = f.DefineUnit(name, armor: armor, speed: 0);
            COMTestField.AddWeapon(ud, "パンチ", 1000, maxRange: counterRange);
            if (abilityEffects != null)
            {
                var ad = ud.AddAbility("修理");
                ad.SetEffect(abilityEffects);
                ad.Class = "";
                ad.MinRange = 1;
                ad.MaxRange = 1;
            }
        }

        [TestMethod]
        public void OperateUnit_HealingAbilityNotFirstEffect_IsNotImportantTarget()
        {
            // VB: 重要なユニットと見なす回復アビリティは .EffectType(1) = "回復"（1 番目の効果）だけ。
            // 1 番目が補給で 2 番目が回復のアビリティは重要と見なさないので、
            // ダメージの期待値が同じなら後に見たユニット（>= で置き換わる）を狙う
            var f = CreateField();
            var attacker = DeployTacticalAttacker(f, "射");
            DefineTargetUnit(f, "補給機", "補給 回復Lv1");
            DefineTargetUnit(f, "通常機");
            f.Deploy("補給機", "標的A", "ＮＰＣ", 5, 7);
            var plain = f.Deploy("通常機", "標的B", "ＮＰＣ", 7, 5);

            Operate(f, attacker);

            Assert.AreSame(plain, f.Src.Commands.SelectedTarget);
        }

        [TestMethod]
        public void OperateUnit_HealingAbilityFirstEffect_IsImportantTarget()
        {
            // 1 番目の効果が回復のアビリティを持つユニットは重要と見なし、優先して狙う
            var f = CreateField();
            var attacker = DeployTacticalAttacker(f, "射");
            DefineTargetUnit(f, "回復機", "回復Lv1 補給");
            DefineTargetUnit(f, "通常機");
            var healer = f.Deploy("回復機", "標的A", "ＮＰＣ", 5, 7);
            f.Deploy("通常機", "標的B", "ＮＰＣ", 7, 5);

            Operate(f, attacker);

            Assert.AreSame(healer, f.Src.Commands.SelectedTarget);
        }

        [TestMethod]
        public void OperateUnit_IndirectAttackCheck_UsesWeaponForEvaluatedTarget()
        {
            // 間接攻撃かどうかは、評価しているターゲットへの武器（tmp_w）で判定する。
            // VB 版は w（それまでに選んだターゲットへの武器。最初は 0）を見ていて、最初に見たユニットだけ
            // 間接攻撃と見なさない書き誤りがある。その不具合は再現しないので、どちらも 1.5 倍になり、
            // ダメージの大きい装甲の薄いほうを狙う（#1501）
            var f = CreateField();
            var attacker = DeployTacticalAttacker(f, "射間");
            DefineTargetUnit(f, "薄装甲機", armor: 950, counterRange: 4);
            DefineTargetUnit(f, "通常機", counterRange: 4);
            var weak = f.Deploy("薄装甲機", "標的A", "ＮＰＣ", 5, 7);
            var plain = f.Deploy("通常機", "標的B", "ＮＰＣ", 7, 5);

            // 前提: 装甲が薄いほうがダメージは大きいが、1.5 倍の差は無い
            f.Src.COM.SelectWeapon(attacker, weak, "移動可能", out _, out var dmgWeak);
            f.Src.COM.SelectWeapon(attacker, plain, "移動可能", out _, out var dmgPlain);
            Assert.IsTrue(dmgWeak > dmgPlain, $"{dmgWeak} {dmgPlain}");
            Assert.IsTrue(dmgWeak < 1.5 * dmgPlain, $"{dmgWeak} {dmgPlain}");

            Operate(f, attacker);

            Assert.AreSame(weak, f.Src.Commands.SelectedTarget);
        }

        private static int RandomCountOfPreemptiveCounter(string targetWeaponClass, double readAhead)
        {
            var f = CreateField();
            var aud = f.DefineUnit("攻撃機", speed: 0);
            COMTestField.AddWeapon(aud, "パンチ", 100);
            var attacker = f.Deploy("攻撃機", "攻撃者", "敵", 5, 5);
            var tud = f.DefineUnit("反撃機", speed: 0);
            COMTestField.AddWeapon(tud, "キック", 100, wclass: targetWeaponClass);
            var pd = f.DefinePilot("反撃者");
            if (readAhead > 0)
            {
                pd.AddSkill("先読み", readAhead, "", 1);
            }
            f.Deploy("反撃機", "反撃者", "ＮＰＣ", 5, 6);

            var savedSeed = GeneralLib.RndSeed;
            try
            {
                GeneralLib.RndSeed = 12345;
                GeneralLib.RndReset();
                Operate(f, attacker);
                return GeneralLib.RndIndex;
            }
            finally
            {
                GeneralLib.RndSeed = savedSeed;
                GeneralLib.RndReset();
            }
        }

        [TestMethod]
        public void OperateUnit_PreemptiveCounterWithFirstStrikeWeapon_ConsumesReadAheadDice()
        {
            // VB: If .IsWeaponClassifiedAs(tw, "先") Or .MainPilot.SkillLevel("先読み") >= Dice(16) Or ...
            // VB の Or は短絡しないので、反撃の武器が「先」でも Dice(16) を 1 回呼ぶ。
            // 「先」の武器で先制する場合と、先読み Lv16（必ず先制）で先制する場合とで、乱数の消費は同じになる
            var withFirstStrike = RandomCountOfPreemptiveCounter("格先", 0);
            var withReadAhead = RandomCountOfPreemptiveCounter("格", 16);

            Assert.AreEqual(withReadAhead, withFirstStrike);
        }

        [TestMethod]
        public void OperateUnit_ManualCounter_AllChoicesDisabledAndCancelled_EndsSelection()
        {
            // VB: 手動反撃の選択で何も選ばなかった（Case Else）とき、反撃・防御・回避がすべて選べなければ Exit Do する
            var f = CreateField(autoDefense: false);
            var aud = f.DefineUnit("攻撃機", speed: 0);
            COMTestField.AddWeapon(aud, "パンチ", 100);
            var attacker = f.Deploy("攻撃機", "攻撃者", "敵", 5, 5);
            var tud = f.DefineUnit("的", speed: 0);
            tud.AddFeature("防御不可");
            tud.AddFeature("回避不可");
            f.Deploy("的", "味方A", "味方", 5, 6);

            var calls = 0;
            f.GUI.Handle("ListBox", _ =>
            {
                calls++;
                if (calls > 3)
                {
                    throw new InvalidOperationException("反撃手段の選択が終わらない");
                }
                return 0;
            });

            Operate(f, attacker);

            Assert.AreEqual(1, calls);
        }
    
        [TestMethod]
        public void OperateUnit_ManualCounter_SupportGuardLabel_HasSpaceBeforeParenthesisWithLifeSizeOption()
        {
            // VB: 等身大基準のとき list(4) = "援護防御：使用する (" & SupportGuardUnit.Nickname & ")"（括弧の前に空白がある）
            // （等身大基準ではユニットの愛称はパイロットの愛称になる）
            var f = CreateField(autoDefense: false);
            f.Src.Expression.DefineGlobalVariable("Option(等身大基準)");
            var aud = f.DefineUnit("攻撃機", speed: 0);
            COMTestField.AddWeapon(aud, "パンチ", 100);
            var attacker = f.Deploy("攻撃機", "攻撃者", "敵", 5, 5);
            f.DefineUnit("的", speed: 0);
            f.DefineUnit("護衛機", speed: 0);
            f.Deploy("的", "味方A", "味方", 5, 6);
            f.DefinePilot("護衛者").AddSkill("援護防御", 1, "", 1);
            f.Deploy("護衛機", "護衛者", "味方", 6, 6);

            string[] texts = null;
            f.GUI.Handle("ListBox", args =>
            {
                texts = ((ListBoxArgs)args[0]).Items.Select(x => x.Text).ToArray();
                // 防御を選ぶ
                return 2;
            });

            Operate(f, attacker);

            Assert.IsNotNull(texts);
            CollectionAssert.Contains(texts, "援護防御：使用する (護衛者)", string.Join(" / ", texts));
        }
    }
}
