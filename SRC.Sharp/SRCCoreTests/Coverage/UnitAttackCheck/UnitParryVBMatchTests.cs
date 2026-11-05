using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Coverage.Tests.SpecialPower;
using SRCCore.Lib;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitAttackCheck
{
    /// <summary>
    /// VB 版 Unit.cls の CheckParryFeature と突き合わせて見つけた違いのテスト（7474/SRC#1500）。
    /// 期待値は元の SRC の動作。
    /// </summary>
    [TestClass]
    public class UnitParryVBMatchTests
    {
        private const string Pilots = @"
攻撃者
攻撃者, こうげきしゃ, 男性, 汎用, AAAA, 100
特殊能力なし
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

迎撃手
迎撃手, げいげきしゅ, 男性, 汎用, AAAA, 100
特殊能力
迎撃Lv16, 1
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid
";

        private const string Units = @"
攻撃機
攻撃機, こうげきき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力なし
3000, 200, 1000, 100
AAAA, -.bmp
ミサイル, 1500, 1, 3, +0, -, -, -, AAAA, +0, 実

迎撃機
迎撃機, げいげきき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力なし
3000, 200, 1000, 100
AAAA, -.bmp
バルカン, 500, 1, 2, +0, 10, -, -, AAAA, +0, 射Ｐ

弾切れ迎撃機
弾切れ迎撃機, たまぎれげいげきき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力なし
3000, 200, 1000, 100
AAAA, -.bmp
バルカン, 500, 1, 2, +0, 9, -, -, AAAA, +0, 射Ｐ
";

        private static UnitAttackCheckFixture CreateFixture()
        {
            var f = new UnitAttackCheckFixture();
            f.LoadPilots(Pilots);
            f.LoadUnits(Units);
            return f;
        }

        private static bool Parry(Unit a, string wname, Unit t)
        {
            var msg = "";
            var w = a.Weapons.First(x => x.Name == wname);
            return a.CheckParryFeature(w, ref t, t.x, t.y, "", "", 1500, ref msg, true);
        }

        private static UnitWeapon WeaponOf(Unit u, string name)
        {
            return u.Weapons.First(x => x.Name == name);
        }

        [TestMethod]
        public void Intercept_AutoSelect_UsesWeaponDataBulletNotRemainingBullet()
        {
            // VB: (t.Weapon(i).Bullet >= 10 Or ...) の Bullet は武器データの弾数で、いまの残弾ではない。
            // 弾数 10 の武器は残弾が 5 でも迎撃に使える
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("迎撃機", "迎撃手", "敵", 2, 1);
            WeaponOf(t, "バルカン").SetBullet(5);

            Assert.IsTrue(Parry(a, "ミサイル", t));
            CollectionAssert.Contains(f.SysMessages(), "迎撃機は[バルカン]で[ミサイル]を阻止した。");
            Assert.AreEqual(4, WeaponOf(t, "バルカン").Bullet());
        }

        [TestMethod]
        public void Intercept_NoSuitableWeapon_StillRollsDice()
        {
            // VB: If 0 < i And i <= t.CountWeapon And (slevel >= Dice(16) Or ...) Then
            // VB の And は短絡しないので、迎撃用の武器が無くても Dice(16) を 1 回振る
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("弾切れ迎撃機", "迎撃手", "敵", 2, 1);

            using (var r = new FixedRandom())
            {
                r.Next(0.0);
                Assert.IsFalse(Parry(a, "ミサイル", t));
                Assert.AreEqual(1, GeneralLib.RndIndex);
            }
        }
    }
}
