using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;
using static SRCCore.Coverage.Tests.UnitWeaponCov.UnitWeaponCovHelper;

namespace SRCCore.Coverage.Tests.UnitWeaponCov
{
    /// <summary>
    /// UnitWeapon.IsWeaponAvailable / IsTargetWithinRange / IsWeaponUseful / CanUseIntercept のテスト。
    /// 期待値は VB 版 Unit.cls の IsWeaponAvailable / IsTargetWithinRange に従う。
    /// </summary>
    [TestClass]
    public class UnitWeaponCovAvailabilityTests
    {
        private static (SRC src, Unit u, UnitWeapon w) Setup(string wclass = "射", int maxRange = 3,
            int bullet = 0, int en = 0, int morale = 0, string party = "味方", string stage = "味方",
            System.Action<Models.UnitData> configure = null)
        {
            var src = CreateSrc();
            src.Stage = stage;
            var u = CreateUnit(src, party: party, configure: ud =>
            {
                configure?.Invoke(ud);
                AddWeapon(ud, "武器", wclass: wclass, maxRange: maxRange, bullet: bullet, en: en, morale: morale);
            });
            return (src, u, Weapon(u, "武器"));
        }

        // ══════════════════════════════════════════════
        // IsWeaponAvailable
        // ══════════════════════════════════════════════

        [TestMethod]
        public void IsWeaponAvailable_Disabled_IsFalse()
        {
            // Disable コマンドで封印された武器は使えない
            var (src, _, w) = Setup();
            Assert.IsTrue(w.IsWeaponAvailable("移動前"));
            src.Expression.DefineGlobalVariable("Disable(武器)");
            Assert.IsFalse(w.IsWeaponAvailable("移動前"));
            Assert.IsFalse(w.IsEnable());
        }

        [TestMethod]
        public void IsWeaponAvailable_StatusModes_IgnoreMorale()
        {
            // 必要気力を満たさなくても、ステータス表示（""）やインターミッションでは使用可能と表示する
            var (_, _, w) = Setup(morale: 120);
            Assert.IsTrue(w.IsWeaponAvailable(""));
            Assert.IsTrue(w.IsWeaponAvailable("インターミッション"));
            Assert.IsFalse(w.IsWeaponAvailable("移動前"));
        }

        [TestMethod]
        public void IsWeaponAvailable_MoraleEnough_IsTrue()
        {
            var (_, u, w) = Setup(morale: 120);
            u.MainPilot().Morale = 120;
            Assert.IsTrue(w.IsWeaponAvailable("移動前"));
        }

        [TestMethod]
        public void IsWeaponAvailable_NoPilot_AlwaysTrue()
        {
            // パイロットが乗っていなければ常に使用可能と判定
            var src = CreateSrc();
            var u = CreateUnit(src, withPilot: false, configure: ud => AddWeapon(ud, "武器", wclass: "射", maxRange: 3, morale: 150, bullet: 1));
            var w = Weapon(u, "武器");
            w.SetBullet(0);
            Assert.IsTrue(w.IsWeaponAvailable("移動前"));
        }

        [TestMethod]
        public void IsWeaponAvailable_NoBulletOrEn_IsFalse()
        {
            var (_, _, wb) = Setup(bullet: 2);
            Assert.IsTrue(wb.IsWeaponAvailable("移動前"));
            wb.SetBullet(0);
            Assert.IsFalse(wb.IsWeaponAvailable("移動前"));

            var (_, u, we) = Setup(en: 30);
            u.EN = 29;
            Assert.IsFalse(we.IsWeaponAvailable("移動前"));
            u.EN = 30;
            Assert.IsTrue(we.IsWeaponAvailable("移動前"));
        }

        [TestMethod]
        public void IsWeaponAvailable_MoneyWeapon_RequiresMoney()
        {
            // 銭属性: 味方は 最大(レベル,1) × ユニット価値 / 10 の資金が必要
            var (src, u, w) = Setup(wclass: "射銭", configure: ud => ud.Value = 10000);
            var cost = u.Value / 10;
            Assert.IsTrue(cost > 0);
            src.Money = cost - 1;
            Assert.IsFalse(w.IsWeaponAvailable("移動前"));
            src.Money = cost;
            Assert.IsTrue(w.IsWeaponAvailable("移動前"));
        }

        [TestMethod]
        public void IsWeaponAvailable_Conditions()
        {
            // 攻撃不能状態では使えない（ステータス表示では使用可能と表示）
            var (_, u, w) = Setup();
            u.AddCondition("攻撃不能", 1);
            Assert.IsFalse(w.IsWeaponAvailable("移動前"));
            Assert.IsTrue(w.IsWeaponAvailable("ステータス"));
        }

        [TestMethod]
        public void IsWeaponAvailable_AttributeSealedCondition()
        {
            // 「○属性使用不能」状態では該当属性の武器が使えない
            var (_, u, w) = Setup(wclass: "射火");
            u.AddCondition("火属性使用不能", 1);
            Assert.IsFalse(w.IsWeaponAvailable("移動前"));
        }

        [TestMethod]
        public void IsWeaponAvailable_Silence_BlocksSpellAndSound()
        {
            // 沈黙状態では術・音属性の武器が使えない
            var (_, u, spell) = Setup(wclass: "術");
            var (_, u2, sound) = Setup(wclass: "音");
            var (_, u3, normal) = Setup(wclass: "射");
            u.AddCondition("沈黙", 1);
            u2.AddCondition("沈黙", 1);
            u3.AddCondition("沈黙", 1);
            Assert.IsFalse(spell.IsWeaponAvailable("移動前"));
            Assert.IsFalse(sound.IsWeaponAvailable("移動前"));
            Assert.IsTrue(normal.IsWeaponAvailable("移動前"));
        }

        [TestMethod]
        public void IsWeaponAvailable_Underground_IsFalse()
        {
            var (_, u, w) = Setup();
            u.Area = "地中";
            Assert.IsFalse(w.IsWeaponAvailable("移動前"));
        }

        [TestMethod]
        public void IsWeaponAvailable_DesperateOnlyAtLowHp()
        {
            // 瀕属性: ＨＰが最大値の 1/4 以下のときだけ使える
            var (_, u, w) = Setup(wclass: "射瀕");
            Assert.IsFalse(w.IsWeaponAvailable("移動前"));
            u.HP = u.MaxHP / 4;
            Assert.IsTrue(w.IsWeaponAvailable("移動前"));
        }

        [TestMethod]
        public void IsWeaponAvailable_Recharging_IsFalse()
        {
            // 自動チャージ攻撃の再充填中は使えない
            var (_, u, w) = Setup(wclass: "射");
            u.AddCondition("武器充填中", 2);
            Assert.IsFalse(w.IsWeaponAvailable("移動前"));
        }

        [TestMethod]
        public void IsWeaponAvailable_Forbidden_IsFalse()
        {
            var (_, _, w) = Setup(wclass: "射禁");
            Assert.IsFalse(w.IsWeaponAvailable("移動前"));
            // ただし必要技能だけを見るステータス表示では使用可能
            Assert.IsTrue(w.IsWeaponAvailable(""));
        }

        [TestMethod]
        public void IsWeaponAvailable_ChargeWeapon_NeedsChargeComplete()
        {
            // Ｃ属性: チャージ完了状態が必要。チャージ判定モードではその前で使用可能とする
            var (_, u, w) = Setup(wclass: "射Ｃ");
            Assert.IsFalse(w.IsWeaponAvailable("移動前"));
            Assert.IsTrue(w.IsWeaponAvailable("チャージ"));
            u.AddCondition("チャージ完了", 1);
            Assert.IsTrue(w.IsWeaponAvailable("移動前"));
        }

        [TestMethod]
        public void IsWeaponAvailable_Counterattack_ExcludesMapAndAttackOnly()
        {
            // 自軍フェイズでない（反撃時）はマップ攻撃・攻撃専用武器が使えない。低攻 は攻撃専用ではない
            Assert.IsFalse(Setup(wclass: "Ｍ全", stage: "敵").w.IsWeaponAvailable("移動前"));
            Assert.IsFalse(Setup(wclass: "射攻", stage: "敵").w.IsWeaponAvailable("移動前"));
            Assert.IsTrue(Setup(wclass: "射低攻", stage: "敵").w.IsWeaponAvailable("移動前"));
            Assert.IsTrue(Setup(wclass: "射反", stage: "敵").w.IsWeaponAvailable("移動前"));
            // 自軍フェイズでは反撃専用武器が使えない
            Assert.IsFalse(Setup(wclass: "射反", stage: "味方").w.IsWeaponAvailable("移動前"));
        }

        [TestMethod]
        public void IsWeaponAvailable_AfterMove_DependsOnPAndQ()
        {
            // 移動後: 射程 2 以上はＰ属性が必要、射程 1 はＱ属性が無ければ使える
            (SRC, UnitWeapon) AfterMove(string wclass, int maxRange)
            {
                var (src, u, w) = Setup(wclass: wclass, maxRange: maxRange);
                src.Commands.SelectedUnit = u;
                return (src, w);
            }
            Assert.IsFalse(AfterMove("射", 3).Item2.IsWeaponAvailable("移動後"));
            Assert.IsTrue(AfterMove("射Ｐ", 3).Item2.IsWeaponAvailable("移動後"));
            Assert.IsTrue(AfterMove("格", 1).Item2.IsWeaponAvailable("移動後"));
            Assert.IsFalse(AfterMove("格Ｑ", 1).Item2.IsWeaponAvailable("移動後"));
            // 移動前なら射程に関係なく使える
            Assert.IsTrue(AfterMove("射", 3).Item2.IsWeaponAvailable("移動前"));
        }

        [TestMethod]
        public void IsWeaponAvailable_AfterMove_OtherUnitSelected_IsTrue()
        {
            // 選択中のユニットでなければ移動後の制限は掛からない
            var (src, _, w) = Setup(wclass: "射", maxRange: 3);
            src.Commands.SelectedUnit = null;
            Assert.IsTrue(w.IsWeaponAvailable("移動後"));
        }

        [TestMethod]
        public void IsWeaponAvailable_NecessaryMorale_IgnoredWhenSkillIgnoreMode()
        {
            // 必要技能無視モードでは必要技能を見ない（必要気力は見る）
            var src = CreateSrc();
            src.Stage = "味方";
            var u = CreateUnit(src, configure: ud =>
            {
                var wd = AddWeapon(ud, "武器", wclass: "射", maxRange: 3);
                wd.NecessarySkill = "存在しない技能";
            });
            var w = Weapon(u, "武器");
            Assert.IsFalse(w.IsWeaponMastered());
            Assert.IsFalse(w.IsWeaponAvailable("移動前"));
            Assert.IsTrue(w.IsWeaponAvailable("必要技能無視"));
        }

        // ══════════════════════════════════════════════
        // IsTargetWithinRange
        // ══════════════════════════════════════════════

        [TestMethod]
        public void IsTargetWithinRange_ChecksMinAndMaxRange()
        {
            var src = CreateSrc();
            var u = CreateUnit(src, x: 1, y: 1, configure: ud => AddWeapon(ud, "武器", wclass: "射", minRange: 2, maxRange: 3));
            var w = Weapon(u, "武器");
            var near = CreateUnit(src, party: "敵", x: 2, y: 1);
            var mid = CreateUnit(src, party: "敵", x: 2, y: 2);
            var far = CreateUnit(src, party: "敵", x: 3, y: 3);
            Assert.IsFalse(w.IsTargetWithinRange(near));
            Assert.IsTrue(w.IsTargetWithinRange(mid));
            Assert.IsFalse(w.IsTargetWithinRange(far));
        }

        [TestMethod]
        public void IsTargetWithinRange_DropMapAttack_ExtendsRange()
        {
            // Ｍ投 は目標地点からさらにレベル分だけ範囲が広がる
            var src = CreateSrc();
            var u = CreateUnit(src, x: 1, y: 1, configure: ud => AddWeapon(ud, "武器", wclass: "Ｍ投L1", minRange: 1, maxRange: 2));
            var target = CreateUnit(src, party: "敵", x: 4, y: 1);
            Assert.IsTrue(Weapon(u, "武器").IsTargetWithinRange(target));
        }

        [TestMethod]
        public void IsTargetWithinRange_Stealth_LimitsDistance()
        {
            // ステルス: レベル（省略時 3）より離れていると狙えない。ステルス無効化があれば狙える
            var src = CreateSrc();
            var u = CreateUnit(src, x: 1, y: 1, configure: ud => AddWeapon(ud, "武器", wclass: "射", maxRange: 5));
            var w = Weapon(u, "武器");
            var t2 = CreateUnit(src, party: "敵", x: 3, y: 1, configure: ud => ud.AddFeature("ステルスLv2"));
            var t3 = CreateUnit(src, party: "敵", x: 4, y: 1, configure: ud => ud.AddFeature("ステルスLv2"));
            Assert.IsTrue(w.IsTargetWithinRange(t2));
            Assert.IsFalse(w.IsTargetWithinRange(t3));

            var anti = CreateUnit(src, x: 1, y: 2, configure: ud =>
            {
                ud.AddFeature("ステルス無効化");
                AddWeapon(ud, "武器", wclass: "射", maxRange: 5);
            });
            // (1,2)→(4,1) は距離 4
            Assert.IsTrue(Weapon(anti, "武器").IsTargetWithinRange(t3));
        }

        [TestMethod]
        public void IsTargetWithinRange_UnattackableArea_IsFalse()
        {
            // 武器の地形適応が「-」の地形にいる相手は射程外扱い
            var src = CreateSrc();
            var u = CreateUnit(src, x: 1, y: 1, configure: ud => AddWeapon(ud, "武器", wclass: "射", maxRange: 3, adaption: "-AAA"));
            var flying = CreateUnit(src, party: "敵", x: 2, y: 1, area: "空中");
            var ground = CreateUnit(src, party: "敵", x: 1, y: 2);
            Assert.IsFalse(Weapon(u, "武器").IsTargetWithinRange(flying));
            Assert.IsTrue(Weapon(u, "武器").IsTargetWithinRange(ground));
        }

        // ══════════════════════════════════════════════
        // IsWeaponUseful
        // ══════════════════════════════════════════════

        [TestMethod]
        public void IsWeaponUseful_TrueOnlyWhenEnemyInRange()
        {
            // 射程内に敵がいて、ダメージを与えられる場合に有用
            var src = CreateSrc();
            src.Stage = "味方";
            var u = CreateUnit(src, x: 1, y: 1, configure: ud => AddWeapon(ud, "武器", wclass: "射", maxRange: 2));
            var w = Weapon(u, "武器");
            // 味方だけでは有用でない
            CreateUnit(src, party: "味方", x: 2, y: 1);
            Assert.IsFalse(w.IsWeaponUseful("移動前"));
            // 射程外の敵
            CreateUnit(src, party: "敵", x: 5, y: 5);
            Assert.IsFalse(w.IsWeaponUseful("移動前"));
            // 射程内の敵
            CreateUnit(src, party: "敵", x: 1, y: 3);
            Assert.IsTrue(w.IsWeaponUseful("移動前"));
        }

        [TestMethod]
        public void IsWeaponUseful_ConfusedAllyCanBeTarget()
        {
            // 混乱などのステータス異常の味方は排除対象として有用とみなす
            var src = CreateSrc();
            src.Stage = "味方";
            var u = CreateUnit(src, x: 1, y: 1, configure: ud => AddWeapon(ud, "武器", wclass: "射", maxRange: 2));
            var ally = CreateUnit(src, party: "味方", x: 2, y: 1);
            ally.AddCondition("混乱", 1);
            Assert.IsTrue(Weapon(u, "武器").IsWeaponUseful("移動前"));
        }

        // ══════════════════════════════════════════════
        // CanUseIntercept
        // ══════════════════════════════════════════════

        [TestMethod]
        public void CanUseIntercept_RequiresShootingAndAmmoOrCheapEn()
        {
            // 迎撃: 移動後に使える射撃系で、弾数 10 以上か、弾数無制限かつ消費ＥＮ 5 以下
            Assert.IsTrue(Setup(wclass: "射", maxRange: 1, bullet: 10).w.CanUseIntercept());
            Assert.IsFalse(Setup(wclass: "射", maxRange: 1, bullet: 9).w.CanUseIntercept());
            Assert.IsTrue(Setup(wclass: "射", maxRange: 1, en: 5).w.CanUseIntercept());
            Assert.IsFalse(Setup(wclass: "射", maxRange: 1, en: 6).w.CanUseIntercept());
            Assert.IsFalse(Setup(wclass: "格", maxRange: 1, bullet: 10).w.CanUseIntercept());
        }
    }
}
