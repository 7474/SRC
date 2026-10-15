using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.Units;
using System;
using static SRCCore.Coverage.Tests.UnitWeaponCov.UnitWeaponCovHelper;

namespace SRCCore.Coverage.Tests.UnitWeaponCov
{
    /// <summary>
    /// UnitWeapon の IsWeaponAvailable / UseWeapon / HitProbability の追加分のテスト。
    /// 期待値は VB 版 Unit.cls の IsWeaponAvailable / UseWeapon / SyncBullet / HitProbability / Update に従う。
    /// </summary>
    [TestClass]
    public class UnitWeaponCovExtraTests
    {
        private static Unit CreatePlayer(SRC src, Action<UnitData> configure, Action<PilotData> configurePilot = null)
        {
            src.Stage = "味方";
            return CreateUnit(src, configure: configure, configurePilot: configurePilot);
        }

        private static void AddSpecialPower(SRC src, Unit u, string name, string effect)
        {
            src.SPDList.Add(name).SetEffect(effect);
            u.MakeSpecialPowerInEffect(name);
        }

        // ══════════════════════════════════════════════
        // IsWeaponAvailable
        // ══════════════════════════════════════════════

        [TestMethod]
        public void IsWeaponAvailable_Plana_NeedsFiveTimesLevel()
        {
            // 霊L2: 霊力 5 × 2 = 10 以上が必要（VB IsWeaponAvailable の霊力消費攻撃）
            var src = CreateSrc();
            var u = CreatePlayer(src, ud => AddWeapon(ud, "霊撃", wclass: "射霊L2", maxRange: 3),
                pd => pd.AddSkill("霊力", 50d, "", 1));
            var w = Weapon(u, "霊撃");
            var p = u.MainPilot();
            p.Plana = 9;
            Assert.AreEqual(9, p.Plana);
            Assert.IsFalse(w.IsWeaponAvailable("移動前"));
            // ステータス表示では霊力を見ない
            Assert.IsTrue(w.IsWeaponAvailable(""));
            p.Plana = 10;
            Assert.IsTrue(w.IsWeaponAvailable("移動前"));
        }

        [TestMethod]
        public void IsWeaponAvailable_PsychoPower_NeedsFiveTimesLevel()
        {
            // プL1: 霊力 5 以上が必要
            var src = CreateSrc();
            var u = CreatePlayer(src, ud => AddWeapon(ud, "念動", wclass: "射プL1", maxRange: 3),
                pd => pd.AddSkill("霊力", 50d, "", 1));
            var w = Weapon(u, "念動");
            u.MainPilot().Plana = 4;
            Assert.IsFalse(w.IsWeaponAvailable("移動前"));
            u.MainPilot().Plana = 5;
            Assert.IsTrue(w.IsWeaponAvailable("移動前"));
        }

        [TestMethod]
        public void IsWeaponAvailable_Immobile_BlocksMovingMapAttackOnly()
        {
            // 移動不能状態では移動型マップ攻撃（Ｍ移）だけが使えない
            var src = CreateSrc();
            var u = CreatePlayer(src, ud =>
            {
                AddWeapon(ud, "突撃", wclass: "Ｍ移", maxRange: 3);
                AddWeapon(ud, "全体攻撃", wclass: "Ｍ全", maxRange: 3);
            });
            Assert.IsTrue(Weapon(u, "突撃").IsWeaponAvailable("移動前"));
            u.AddCondition("移動不能", 1);
            Assert.IsFalse(Weapon(u, "突撃").IsWeaponAvailable("移動前"));
            Assert.IsTrue(Weapon(u, "全体攻撃").IsWeaponAvailable("移動前"));
        }

        [TestMethod]
        public void IsWeaponAvailable_CopyWeapon_BlockedWhenAlreadyTransformed()
        {
            // 写/化: ノーマルモードを持つ（変身済み）ユニットはコピー出来ない
            var src = CreateSrc();
            var normal = CreatePlayer(src, ud =>
            {
                AddWeapon(ud, "コピー", wclass: "射写", maxRange: 3);
                AddWeapon(ud, "変化", wclass: "射化", maxRange: 3);
            });
            Assert.IsTrue(Weapon(normal, "コピー").IsWeaponAvailable("移動前"));
            Assert.IsTrue(Weapon(normal, "変化").IsWeaponAvailable("移動前"));

            // ノーマルモードの戻り先形態のデータ
            var normalForm = src.UDList.Add("元の姿");
            normalForm.HP = 1000;
            normalForm.EN = 100;
            normalForm.Adaption = "AAAA";
            normalForm.Transportation = "陸";
            normalForm.Size = "M";
            var transformed = CreateUnit(src, x: 2, configure: ud =>
            {
                ud.AddFeature("ノーマルモード=元の姿");
                AddWeapon(ud, "コピー", wclass: "射写", maxRange: 3);
                AddWeapon(ud, "変化", wclass: "射化", maxRange: 3);
            });
            Assert.IsFalse(Weapon(transformed, "コピー").IsWeaponAvailable("移動前"));
            Assert.IsFalse(Weapon(transformed, "変化").IsWeaponAvailable("移動前"));
        }

        [TestMethod]
        public void IsWeaponAvailable_SharedWeapon_BlockedWhileSameLevelRecharging()
        {
            // 共: 同じ共レベルの武器が充填中なら使えない。レベルが違えば影響しない
            var src = CreateSrc();
            var u = CreatePlayer(src, ud =>
            {
                AddWeapon(ud, "砲Ａ", wclass: "射共L1", maxRange: 3);
                AddWeapon(ud, "砲Ｂ", wclass: "射共L1", maxRange: 3);
                AddWeapon(ud, "砲Ｃ", wclass: "射共L2", maxRange: 3);
            });
            u.AddCondition("砲Ｂ充填中", 2);
            Assert.IsFalse(Weapon(u, "砲Ａ").IsWeaponAvailable("移動前"));
            Assert.IsFalse(Weapon(u, "砲Ｂ").IsWeaponAvailable("移動前"));
            Assert.IsTrue(Weapon(u, "砲Ｃ").IsWeaponAvailable("移動前"));
        }

        [TestMethod]
        public void IsWeaponAvailable_AllWeaponsAfterMoveSpecialPower_ExceptMapAttack()
        {
            // 全武器移動後使用可能: Ｐ属性の無い射撃武器も移動後に使えるが、マップ攻撃は使えないまま
            var src = CreateSrc();
            var u = CreatePlayer(src, ud =>
            {
                AddWeapon(ud, "ライフル", wclass: "射", maxRange: 3);
                AddWeapon(ud, "全体攻撃", wclass: "Ｍ全", maxRange: 3);
            });
            src.Commands.SelectedUnit = u;
            Assert.IsFalse(Weapon(u, "ライフル").IsWeaponAvailable("移動後"));

            AddSpecialPower(src, u, "突撃", "全武器移動後使用可能");
            Assert.IsTrue(Weapon(u, "ライフル").IsWeaponAvailable("移動後"));
            Assert.IsFalse(Weapon(u, "全体攻撃").IsWeaponAvailable("移動後"));
        }

        [TestMethod]
        public void IsWeaponAvailable_Counterattack_AttackOnlyAtHeadOfClass()
        {
            // 攻 が属性の先頭にある場合も攻撃専用（VB の i = 1 の分岐）
            var src = CreateSrc();
            src.Stage = "敵";
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "武器", wclass: "攻", maxRange: 1));
            Assert.IsFalse(Weapon(u, "武器").IsWeaponAvailable("移動前"));
            src.Stage = "味方";
            Assert.IsTrue(Weapon(u, "武器").IsWeaponAvailable("移動前"));
        }

        // ══════════════════════════════════════════════
        // UseWeapon
        // ══════════════════════════════════════════════

        [TestMethod]
        public void UseWeapon_Plana_ConsumesFiveTimesLevel()
        {
            // 霊L2: 霊力を 10 消費し、ＨＰ・ＥＮは割合を保つ
            var src = CreateSrc();
            var u = CreatePlayer(src, ud => AddWeapon(ud, "霊撃", wclass: "射霊L2", maxRange: 3),
                pd => pd.AddSkill("霊力", 50d, "", 1));
            var p = u.MainPilot();
            p.Plana = 30;
            u.HP = 500;
            u.EN = 40;
            Weapon(u, "霊撃").UseWeapon();
            Assert.AreEqual(20, p.Plana);
            Assert.AreEqual(500, u.HP);
            Assert.AreEqual(40, u.EN);
        }

        [TestMethod]
        public void UseWeapon_PsychoPower_ConsumesFiveTimesLevel()
        {
            // プL3: 霊力を 15 消費する
            var src = CreateSrc();
            var u = CreatePlayer(src, ud => AddWeapon(ud, "念動", wclass: "射プL3", maxRange: 3),
                pd => pd.AddSkill("霊力", 50d, "", 1));
            u.MainPilot().Plana = 30;
            Weapon(u, "念動").UseWeapon();
            Assert.AreEqual(15, u.MainPilot().Plana);
        }

        [TestMethod]
        public void UseWeapon_SharedWeapon_SyncsBulletWithSameLevel()
        {
            // 共: 同じ共レベルの武器と弾数を共有する（VB SyncBullet）
            // 同じ最大弾数 4 発同士 → 1 発使えばもう一方も 3 発
            // 最大 10 発の武器は 2 発中 1 発使用の比率に合わせ 10 × 1 \ 2 = 5 発
            var src = CreateSrc();
            var u = CreatePlayer(src, ud =>
            {
                AddWeapon(ud, "砲Ａ", wclass: "射共L1", maxRange: 3, bullet: 4);
                AddWeapon(ud, "砲Ｂ", wclass: "射共L1", maxRange: 3, bullet: 4);
                AddWeapon(ud, "砲Ｃ", wclass: "射共L2", maxRange: 3, bullet: 2);
                AddWeapon(ud, "砲Ｄ", wclass: "射共L2", maxRange: 3, bullet: 10);
                AddWeapon(ud, "ミサイル", wclass: "射", maxRange: 3, bullet: 4);
            });
            Weapon(u, "砲Ａ").UseWeapon();
            Assert.AreEqual(3, Weapon(u, "砲Ａ").Bullet());
            Assert.AreEqual(3, Weapon(u, "砲Ｂ").Bullet());
            // 共レベルが違う武器、共でない武器は変わらない
            Assert.AreEqual(2, Weapon(u, "砲Ｃ").Bullet());
            Assert.AreEqual(4, Weapon(u, "ミサイル").Bullet());

            Weapon(u, "砲Ｃ").UseWeapon();
            Assert.AreEqual(1, Weapon(u, "砲Ｃ").Bullet());
            Assert.AreEqual(5, Weapon(u, "砲Ｄ").Bullet());
        }

        [TestMethod]
        public void SetMaxBulletRate_RoundsLikeVbIntegerAssignment()
        {
            // VB Update: intMaxBullet(i) = (1 + rate) * intMaxBullet(i)（Integer への代入で丸め）
            // 3 発 × 1.2 = 3.6 → 4
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "武器", wclass: "射", maxRange: 3, bullet: 3));
            var w = Weapon(u, "武器");
            w.SetMaxBulletRate(1.2d);
            Assert.AreEqual(4, w.MaxBullet());
        }

        // ══════════════════════════════════════════════
        // HitProbability
        // 攻撃側 命中 50・武器命中 10、防御側 回避 40 → 基本命中率 120
        // ══════════════════════════════════════════════

        private static (SRC src, Unit a, Unit t, UnitWeapon w) Battle(string wclass = "射", int tx = 2,
            Action<UnitData> configureTarget = null)
        {
            var src = CreateSrc();
            src.Stage = "味方";
            var a = CreateUnit(src, x: 1, y: 1, configure: ud =>
                AddWeapon(ud, "武器", power: 1000, wclass: wclass, maxRange: 5, precision: 10));
            a.MainPilot().Hit = 50;
            var t = CreateUnit(src, party: "敵", x: tx, y: 1, configure: ud =>
            {
                ud.Adaption = "BBBB";
                configureTarget?.Invoke(ud);
            });
            t.MainPilot().Dodge = 40;
            return (src, a, t, Weapon(a, "武器"));
        }

        [TestMethod]
        public void HitProbability_TargetStealth_ReducesBeyondLevel()
        {
            // 相手のステルス: レベル指定なしは距離 3 を超えると 0.8 倍、Lv指定時はレベルを超えると 0.8 倍
            var (_, _, t, w) = Battle(tx: 5, configureTarget: ud => ud.AddFeature("ステルス"));
            Assert.AreEqual(96, w.HitProbability(t, true));
            var (_, _, tNear, wNear) = Battle(tx: 4, configureTarget: ud => ud.AddFeature("ステルス"));
            Assert.AreEqual(120, wNear.HitProbability(tNear, true));
            var (_, _, tLv, wLv) = Battle(tx: 4, configureTarget: ud => ud.AddFeature("ステルスLv2"));
            Assert.AreEqual(96, wLv.HitProbability(tLv, true));
        }

        [TestMethod]
        public void HitProbability_TargetTerrainAdaption_Minus10()
        {
            // 地形適応: 相手が指定地形にいれば -10
            var (_, _, t, w) = Battle(configureTarget: ud => ud.AddFeature("地形適応=平地戦用 平地"));
            Assert.AreEqual(110, w.HitProbability(t, true));
            var (_, _, t2, w2) = Battle(configureTarget: ud => ud.AddFeature("地形適応=砂漠戦用 砂漠"));
            Assert.AreEqual(120, w2.HitProbability(t2, true));
        }

        [TestMethod]
        public void HitProbability_Ecm_ReducesPerLevel_DoubleForHoming()
        {
            // ＥＣＭLv2: 通常 (100 - 5×2)%、ホーミング（Ｈ）は (100 - 10×2)%
            // 誘導機能を持つ攻撃（誘）には無効
            var (_, _, t, w) = Battle(configureTarget: ud => ud.AddFeature("ＥＣＭLv2"));
            Assert.AreEqual(108, w.HitProbability(t, true));
            var (_, _, th, wh) = Battle(wclass: "射Ｈ", configureTarget: ud => ud.AddFeature("ＥＣＭLv2"));
            Assert.AreEqual(96, wh.HitProbability(th, true));
            var (_, _, tg, wg) = Battle(wclass: "射誘", configureTarget: ud => ud.AddFeature("ＥＣＭLv2"));
            Assert.AreEqual(120, wg.HitProbability(tg, true));
        }

        [TestMethod]
        public void HitProbability_Ecm_CanceledByAttackerSideEcm()
        {
            // 相手の近くに攻撃側の味方のＥＣＭがあれば、そのレベル分だけ効果が減る
            var (src, _, t, w) = Battle(configureTarget: ud => ud.AddFeature("ＥＣＭLv2"));
            CreateUnit(src, x: 3, y: 1, configure: ud => ud.AddFeature("ＥＣＭLv1"));
            Assert.AreEqual(114, w.HitProbability(t, true));
        }

        [TestMethod]
        public void HitProbability_AbsoluteSpecialPowers()
        {
            // 絶対命中は 1000、相手の絶対回避は 0（絶対回避が優先）
            var (src, a, t, w) = Battle();
            AddSpecialPower(src, a, "必中", "絶対命中");
            Assert.AreEqual(1000, w.HitProbability(t, true));
            AddSpecialPower(src, t, "ひらめき", "絶対回避");
            Assert.AreEqual(0, w.HitProbability(t, true));
        }

        [TestMethod]
        public void HitProbability_TargetConditions_OneAndHalf()
        {
            // 相手が チャージ / 消耗 / 狂戦士 / 移動不能 なら 1.5 倍（重複すると掛け合わせ）
            foreach (var cond in new[] { "チャージ", "消耗", "狂戦士", "移動不能" })
            {
                var (_, _, t, w) = Battle();
                t.AddCondition(cond, 1);
                Assert.AreEqual(180, w.HitProbability(t, true), cond);
            }
            var (_, _, t2, w2) = Battle();
            t2.AddCondition("チャージ", 1);
            t2.AddCondition("消耗", 1);
            Assert.AreEqual(270, w2.HitProbability(t2, true));
        }
    }
}
