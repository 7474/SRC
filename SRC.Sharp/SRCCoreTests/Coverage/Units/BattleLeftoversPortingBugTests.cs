using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SRCCore.Coverage.Tests.UnitAttack;
using SRCCore.Models;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// 戦闘まわりの移植の不具合（元の SRC の Unit.cls と挙動が異なっていた箇所）を、
    /// 元の SRC の挙動を期待値として確認するテスト。
    /// </summary>
    [TestClass]
    public class BattleLeftoversPortingBugTests
    {
        #region 合体技パートナー・武器使用時の気／霊／失属性（VB は Integer/Long への代入で丸める）

        // パートナーが同名の武器を持たない場合（VB: Unit.cls Attack「同名の武器がなかった場合は自分のデータを使って処理」）
        // 気L0.3: IncreaseMorale(-5×0.3 = -1.5) は Integer 引数への変換で -2 になる
        // 失L0.0096: MaxHP 1000 × 0.0096 = 9.6 → 10 \ 10 = 1 だけＨＰが減る
        [TestMethod]
        public void Attack_Combination_PartnerWithoutWeapon_RoundsMoraleAndHpLoss()
        {
            using (UnitAttackFixture.FixDice(0.5))
            {
                var f = new UnitAttackFixture();
                var a = f.AddUnit("攻撃機", "味方", 2, 2,
                    weapons: new[] { f.Weapon("合体剣", 1500, "合気L0.3失L0.0096", 1, 3) },
                    features: new[] { "合体技=合体剣 僚機" });
                var p = f.AddUnit("僚機", "味方", 6, 8);
                var t = f.AddUnit("標的機", "敵", 3, 2, hp: 5000);

                a.Attack(a.Weapons.First(), t, "", "");

                Assert.AreEqual(98, p.MainPilot().Morale);
                Assert.AreEqual(999, p.HP);
                // 攻撃側自身（UseWeapon）も同じく丸める
                Assert.AreEqual(98, a.MainPilot().Morale);
                Assert.AreEqual(999, a.HP);
            }
        }

        // 霊L0.26: Plana(100) - 5×0.26 = 98.7 → Long への代入で 99
        [TestMethod]
        public void Attack_Combination_PartnerWithoutWeapon_RoundsPlana()
        {
            using (UnitAttackFixture.FixDice(0.5))
            {
                var f = new UnitAttackFixture();
                var skills = new[] { ("霊力", 200d) };
                var a = f.AddUnit("攻撃機", "味方", 2, 2,
                    weapons: new[] { f.Weapon("合体剣", 1500, "合霊L0.26", 1, 3) },
                    features: new[] { "合体技=合体剣 僚機" },
                    skills: skills);
                var p = f.AddUnit("僚機", "味方", 6, 8, skills: skills);
                var t = f.AddUnit("標的機", "敵", 3, 2, hp: 5000);
                a.MainPilot().Plana = 100;
                p.MainPilot().Plana = 100;
                Assert.AreEqual(100, p.MainPilot().Plana);

                a.Attack(a.Weapons.First(), t, "", "");

                Assert.AreEqual(99, p.MainPilot().Plana);
                Assert.AreEqual(99, a.MainPilot().Plana);
            }
        }

        #endregion

        #region SyncBullet / SetBullet / Update の武器引き継ぎ

        private static UnitAbilityCoverageFixture CreateFixture() => new UnitAbilityCoverageFixture();

        private static WeaponData AddWeapon(UnitData ud, string name, string wclass, int bullet, int power = 1000)
        {
            var wd = ud.AddWeapon(name);
            wd.Power = power;
            wd.MinRange = 1;
            wd.MaxRange = 1;
            wd.Bullet = bullet;
            wd.Adaption = "AAAA";
            wd.Class = wclass;
            wd.NecessarySkill = "";
            wd.NecessaryCondition = "";
            return wd;
        }

        // 元の SRC の SyncBullet は lv（Integer）に WeaponLevel を代入するので丸められ（共L1.6 → 2）、
        // 他の武器の WeaponLevel（Double）とはそのまま比較する。
        // 共L1.6（最大2・残0）の lv=2 は 共L2 の武器と一致するので 共L2 の武器の弾数も 0 に合わせる
        [TestMethod]
        public void SyncBullet_SharedLevel_IsRoundedOnAssignment()
        {
            var fx = CreateFixture();
            var ud = fx.AddUnitData("試験機");
            AddWeapon(ud, "ミサイルA", "共L1.6", 2);
            AddWeapon(ud, "ミサイルB", "共L2", 5);
            var u = fx.CreateUnit("試験機", "試験者");
            u.Weapon(1).SetBullet(0);

            u.SyncBullet();

            Assert.AreEqual(0, u.Weapon(1).Bullet());
            Assert.AreEqual(0, u.Weapon(2).Bullet());
        }

        // 逆向き：共L2（残0）の lv=2 と 共L2.4 の WeaponLevel 2.4 は一致しないので合わせない
        [TestMethod]
        public void SyncBullet_SharedLevel_ComparesWithUnroundedLevel()
        {
            var fx = CreateFixture();
            var ud = fx.AddUnitData("試験機");
            AddWeapon(ud, "ミサイルA", "共L2", 2);
            AddWeapon(ud, "ミサイルB", "共L2.4", 5);
            var u = fx.CreateUnit("試験機", "試験者");
            u.Weapon(1).SetBullet(0);

            u.SyncBullet();

            Assert.AreEqual(0, u.Weapon(1).Bullet());
            Assert.AreEqual(5, u.Weapon(2).Bullet());
        }

        // 元の SRC の SetBullet は new_bullet < 0 のときだけ 0、最大弾数 0 の武器なら比率 1 を設定する。
        // 弾数無しの斉属性武器は SyncBullet で SetBullet(0) されるが、比率は 1 のまま残るので
        // 同名の武器データに差し替わって弾数を持つようになっても満タンのまま
        [TestMethod]
        public void SetBullet_ZeroOnWeaponWithoutAmmo_KeepsFullRatio()
        {
            var fx = CreateFixture();
            var ud = fx.AddUnitData("試験機");
            AddWeapon(ud, "フルバースト", "斉", 0);
            AddWeapon(ud, "ミサイル", "", 4);
            var u = fx.CreateUnit("試験機", "試験者");

            u.Weapon(1).SetBullet(0);
            // 武器データの弾数を後から増やして（データの再定義相当）比率を確認する
            ud.Weapons.First().Bullet = 6;
            u.Update();

            Assert.AreEqual(6, u.Weapon(1).Bullet());
        }

        // 元の SRC の Unit.Update は更新前の武器データとオブジェクトとして同一の武器から弾数を引き継ぎ、
        // 一度引き継いだ武器は再び使わない。同名の武器が 2 つあってもそれぞれ別の武器として残る
        [TestMethod]
        public void Update_TwoWeaponsWithSameName_KeepEachWeapon()
        {
            var fx = CreateFixture();
            var ud = fx.AddUnitData("試験機");
            AddWeapon(ud, "ミサイル", "", 4, 1000);
            AddWeapon(ud, "ミサイル", "", 8, 2000);
            var u = fx.CreateUnit("試験機", "試験者");
            u.Weapon(2).SetBullet(2);

            u.Update();

            Assert.AreNotSame(u.Weapon(1), u.Weapon(2));
            Assert.AreEqual(1000, u.Weapon(1).WeaponData.Power);
            Assert.AreEqual(2000, u.Weapon(2).WeaponData.Power);
            Assert.AreEqual(4, u.Weapon(1).Bullet());
            Assert.AreEqual(2, u.Weapon(2).Bullet());
        }

        // セーブデータから復元した武器（武器データを持たない）は名前で照合して弾数を引き継ぐ
        [TestMethod]
        public void Update_AfterRestore_KeepsBulletsByName()
        {
            var fx = CreateFixture();
            var ud = fx.AddUnitData("試験機");
            AddWeapon(ud, "ミサイル", "", 4);
            AddWeapon(ud, "ミサイル", "", 8);
            var u = fx.CreateUnit("試験機", "試験者");
            u.Weapon(1).SetBullet(1);
            u.Weapon(2).SetBullet(2);

            var json = JsonConvert.SerializeObject(u);
            var restored = JsonConvert.DeserializeObject<Unit>(json);
            restored.Restore(fx.Src);
            restored.Update();

            Assert.AreEqual(1, restored.Weapon(1).Bullet());
            Assert.AreEqual(2, restored.Weapon(2).Bullet());
        }

        #endregion

        #region 装填=武器名

        private static Unit CreateReloadUser(UnitAbilityCoverageFixture fx, string edata)
        {
            var ud = fx.AddUnitData("補給機");
            var ad = ud.AddAbility("補給");
            ad.SetEffect("装填=" + edata);
            ad.MinRange = 1;
            ad.MaxRange = 1;
            return fx.CreateUnit("補給機", "補給者");
        }

        // 元の SRC: 弾数が減っていない武器は対象外で、回復したことにならない（メッセージも出ない）
        [TestMethod]
        public void Reload_SpecificWeapon_FullWeaponIsNotReloaded()
        {
            var fx = CreateFixture();
            var user = CreateReloadUser(fx, "ライフル");
            var tud = fx.AddUnitData("対象機");
            AddWeapon(tud, "ライフル", "", 6);
            var target = fx.CreateUnit("対象機", "対象者");

            user.ExecuteAbility(user.Ability(1), target);

            Assert.AreEqual(6, target.Weapon(1).Bullet());
            Assert.IsFalse(fx.SysMessages.Any(m => m.Contains("の武装の使用回数が回復した。")));
        }

        // 元の SRC: 他形態の武器は回復するが、回復したかどうかの判定には使わない
        [TestMethod]
        public void Reload_SpecificWeapon_OtherFormOnlyDoesNotCountAsReloaded()
        {
            var fx = CreateFixture();
            var user = CreateReloadUser(fx, "ライフル");
            var tud = fx.AddUnitData("対象機");
            AddWeapon(tud, "ライフル", "", 6);
            var fud = fx.AddUnitData("対象機改");
            AddWeapon(fud, "ライフル", "", 6);
            var target = fx.CreateUnit("対象機", "対象者");
            var form = fx.Src.UList.Add("対象機改", 0, "味方");
            form.Update();
            form.FullRecover();
            target.AddOtherForm(form);
            form.Weapon(1).SetBullet(0);

            user.ExecuteAbility(user.Ability(1), target);

            Assert.AreEqual(6, form.Weapon(1).Bullet());
            Assert.IsFalse(fx.SysMessages.Any(m => m.Contains("の武装の使用回数が回復した。")));
        }

        // 元の SRC: 回復した武器が共属性なら同じレベルの共属性の武器・アビリティも満タンにしてから SyncBullet する
        [TestMethod]
        public void Reload_SpecificWeapon_SharedWeaponsAreRefilled()
        {
            var fx = CreateFixture();
            var user = CreateReloadUser(fx, "ライフル");
            var tud = fx.AddUnitData("対象機");
            AddWeapon(tud, "ライフル", "共", 6);
            AddWeapon(tud, "ミサイル", "共", 4);
            var target = fx.CreateUnit("対象機", "対象者");
            target.Weapon(1).SetBullet(0);
            target.Weapon(2).SetBullet(0);

            user.ExecuteAbility(user.Ability(1), target);

            Assert.AreEqual(6, target.Weapon(1).Bullet());
            Assert.AreEqual(4, target.Weapon(2).Bullet());
            Assert.IsTrue(fx.SysMessages.Any(m => m.Contains("の武装の使用回数が回復した。")));
        }

        #endregion
    }
}
