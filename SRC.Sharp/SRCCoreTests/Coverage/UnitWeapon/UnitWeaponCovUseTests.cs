using Microsoft.VisualStudio.TestTools.UnitTesting;
using static SRCCore.Coverage.Tests.UnitWeaponCov.UnitWeaponCovHelper;

namespace SRCCore.Coverage.Tests.UnitWeaponCov
{
    /// <summary>
    /// UnitWeapon.UseWeapon と弾数（Bullet / SetBullet / SetMaxBulletRate）のテスト。
    /// 期待値は VB 版 Unit.cls の UseWeapon / Bullet / SetBullet に従う。
    /// </summary>
    [TestClass]
    public class UnitWeaponCovUseTests
    {
        [TestMethod]
        public void UseWeapon_ConsumesEnAndBullet()
        {
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "武器", wclass: "射", maxRange: 3, bullet: 5, en: 20));
            var w = Weapon(u, "武器");
            w.UseWeapon();
            Assert.AreEqual(80, u.EN);
            Assert.AreEqual(4, w.Bullet());
        }

        [TestMethod]
        public void UseWeapon_EnConsumptionUsesReducedCost()
        {
            // 消費されるＥＮは ＥＮ消費減少 などの補正後の値
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                ud.AddFeature("ＥＮ消費減少Lv5");
                AddWeapon(ud, "武器", wclass: "射", maxRange: 3, en: 40);
            });
            Weapon(u, "武器").UseWeapon();
            Assert.AreEqual(80, u.EN);
        }

        [TestMethod]
        public void UseWeapon_Eternal_DoesNotConsumeBullet()
        {
            // 永属性は弾数を消費しない
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "武器", wclass: "射永", maxRange: 3, bullet: 5));
            var w = Weapon(u, "武器");
            w.UseWeapon();
            Assert.AreEqual(5, w.Bullet());
        }

        [TestMethod]
        public void UseWeapon_SideEffects()
        {
            // 消: 消耗状態、尽: ＥＮ 0、ＡLv: 充填中状態（レベル分のターン）
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                AddWeapon(ud, "消耗技", wclass: "射消", maxRange: 3);
                AddWeapon(ud, "全力", wclass: "射尽", maxRange: 3);
                AddWeapon(ud, "チャージ砲", wclass: "射ＡL2", maxRange: 3);
            });
            Weapon(u, "消耗技").UseWeapon();
            Assert.IsTrue(u.IsConditionSatisfied("消耗"));

            Weapon(u, "全力").UseWeapon();
            Assert.AreEqual(0, u.EN);

            Weapon(u, "チャージ砲").UseWeapon();
            Assert.IsTrue(u.IsConditionSatisfied("チャージ砲充填中"));
            Assert.AreEqual(2, u.ConditionLifetime("チャージ砲充填中"));
            // 充填中は使用不能
            Assert.IsFalse(Weapon(u, "チャージ砲").IsWeaponAvailable("移動前"));
        }

        [TestMethod]
        public void UseWeapon_ChargeWeapon_ConsumesChargeComplete()
        {
            // Ｃ属性の武器を使うとチャージ完了状態が解除される
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "武器", wclass: "射Ｃ", maxRange: 3));
            u.AddCondition("チャージ完了", 1);
            Weapon(u, "武器").UseWeapon();
            Assert.IsFalse(u.IsConditionSatisfied("チャージ完了"));
        }

        [TestMethod]
        public void UseWeapon_MoraleCost()
        {
            // 気Lv2: 気力 -10
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "武器", wclass: "射気L2", maxRange: 3));
            u.MainPilot().Morale = 120;
            Weapon(u, "武器").UseWeapon();
            Assert.AreEqual(110, u.MainPilot().Morale);
        }

        [TestMethod]
        public void UseWeapon_HpCost()
        {
            // 失Lv2: 最大ＨＰの 2 割を失う
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "武器", wclass: "射失L2", maxRange: 3));
            Weapon(u, "武器").UseWeapon();
            Assert.AreEqual(800, u.HP);
            u.HP = 100;
            Weapon(u, "武器").UseWeapon();
            Assert.AreEqual(0, u.HP);
        }

        [TestMethod]
        public void UseWeapon_MoneyCost_OnlyForPlayer()
        {
            // 銭: 味方なら 最大(レベル,1) × ユニット価値 / 10 の資金を消費する（価値 10000 → 1000）
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                ud.Value = 10000;
                AddWeapon(ud, "武器", wclass: "射銭", maxRange: 3);
            });
            src.Money = 5000;
            Weapon(u, "武器").UseWeapon();
            Assert.AreEqual(4000, src.Money);

            // 銭L2 は 2 倍の 2000
            var u2 = CreateUnit(src, x: 3, configure: ud =>
            {
                ud.Value = 10000;
                AddWeapon(ud, "武器", wclass: "射銭L2", maxRange: 3);
            });
            Weapon(u2, "武器").UseWeapon();
            Assert.AreEqual(2000, src.Money);

            var enemy = CreateUnit(src, party: "敵", x: 2, configure: ud =>
            {
                ud.Value = 10000;
                AddWeapon(ud, "武器", wclass: "射銭", maxRange: 3);
            });
            var before = src.Money;
            Weapon(enemy, "武器").UseWeapon();
            Assert.AreEqual(before, src.Money);
        }

        [TestMethod]
        public void UseWeapon_FullBurst_DoesNotRefillOtherWeapons()
        {
            // VB: SetBullet i, MinLng(MaxBullet(i) * dblBullet(w), Bullet(i))
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                AddWeapon(ud, "一斉発射", wclass: "射斉", maxRange: 3, bullet: 3);
                AddWeapon(ud, "ミサイル", wclass: "射", maxRange: 3, bullet: 10);
            });
            var missile = Weapon(u, "ミサイル");
            missile.SetBullet(2);
            Weapon(u, "一斉発射").UseWeapon();
            Assert.AreEqual(2, missile.Bullet());
        }

        [TestMethod]
        public void UseWeapon_FullBurst_ReducesOtherWeaponsToSameRatio()
        {
            // 斉: 他の武器の弾数も一斉発射の残弾比率に合わせて減る（10 発 × 1/2 = 5）
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                AddWeapon(ud, "一斉発射", wclass: "射斉", maxRange: 3, bullet: 2);
                AddWeapon(ud, "ミサイル", wclass: "射", maxRange: 3, bullet: 10);
            });
            Weapon(u, "一斉発射").UseWeapon();
            Assert.AreEqual(1, Weapon(u, "一斉発射").Bullet());
            Assert.AreEqual(5, Weapon(u, "ミサイル").Bullet());
        }

        [TestMethod]
        public void UseWeapon_OtherWeapon_RoundsFullBurstAmmoUp()
        {
            // VB: SetBullet i, MinLng(CInt(MaxBullet(i) * dblBullet(w) + 0.49999), Bullet(i))
            // 2 発中 1 発使用 → 比率 0.5 → 斉 3 発 × 0.5 + 0.49999 = 1.99999 → CInt で 2
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud =>
            {
                AddWeapon(ud, "一斉発射", wclass: "射斉", maxRange: 3, bullet: 3);
                AddWeapon(ud, "ミサイル", wclass: "射", maxRange: 3, bullet: 2);
            });
            Weapon(u, "ミサイル").UseWeapon();
            Assert.AreEqual(2, Weapon(u, "一斉発射").Bullet());
        }

        // ══════════════════════════════════════════════
        // 弾数
        // ══════════════════════════════════════════════

        [TestMethod]
        public void SetMaxBulletRate_ClampsTo0And99()
        {
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "武器", wclass: "射", maxRange: 3, bullet: 40));
            var w = Weapon(u, "武器");
            w.SetMaxBulletRate(1.5d);
            Assert.AreEqual(60, w.MaxBullet());
            w.SetMaxBulletRate(3d);
            Assert.AreEqual(99, w.MaxBullet());
            w.SetMaxBulletRate(-1d);
            Assert.AreEqual(0, w.MaxBullet());
        }

        [TestMethod]
        public void Bullet_KeepsRatioWhenMaxChanges()
        {
            // 弾数は比率で保持しているため、最大弾数が増えると残弾も比例して増える
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "武器", wclass: "射", maxRange: 3, bullet: 10));
            var w = Weapon(u, "武器");
            w.SetBullet(5);
            w.SetMaxBulletRate(2d);
            Assert.AreEqual(20, w.MaxBullet());
            Assert.AreEqual(10, w.Bullet());
        }

        [TestMethod]
        public void Bullet_RoundsRatioTimesMax()
        {
            // VB: Bullet = dblBullet(w) * intMaxBullet(w)（Integer への代入で丸め）
            // 1 / 49 × 49 = 0.9999999999999999 → 1
            var src = CreateSrc();
            var u = CreateUnit(src, configure: ud => AddWeapon(ud, "武器", wclass: "射", maxRange: 3, bullet: 49));
            var w = Weapon(u, "武器");
            w.SetBullet(1);
            Assert.AreEqual(1, w.Bullet());
        }
    }
}
