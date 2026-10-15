using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// 元の SRC（Unit.cls / GUI.bas）と挙動が異なる箇所を、元の SRC の挙動を期待値として記録するテスト。
    /// 移植の不具合が修正されるまで Ignore にしている。
    /// </summary>
    [TestClass]
    public class UnitAbilityCoveragePortingBugTests
    {
        private UnitAbilityCoverageFixture fx;

        [TestInitialize]
        public void Setup()
        {
            fx = new UnitAbilityCoverageFixture();
        }

        private Unit CreateUser(string effect, string aclass, int maxRange = 1)
        {
            var ud = fx.AddUnitData("使用機");
            var ad = ud.AddAbility("テストアビリティ");
            ad.SetEffect(effect);
            ad.Class = aclass;
            ad.MinRange = 1;
            ad.MaxRange = maxRange;
            return fx.CreateUnit("使用機", "使用者");
        }

        [TestMethod]
        public void AbilityLevel_StopsAtNonNumericCharacter()
        {
            // 元の SRC の AbilityLevel は数字・"-"・"." 以外の文字で Exit Do する
            var u = CreateUser("回復Lv1", "難L3魔");
            Assert.AreEqual(3d, u.Ability(1).AbilityLevel("難"));
        }

        [TestMethod]
        public void Available_SummonWithUndefinedUnit_IsFalse()
        {
            // 元の SRC では召喚ユニットのデータ定義チェックは Servant の有無に関係なく行う
            var u = CreateUser("召喚=未定義ユニット", "");
            Assert.IsFalse(u.Ability(1).IsAbilityAvailable("移動前"));
        }

        [TestMethod]
        public void IdDisplayFor_HidesUnavailableCombinationAbility()
        {
            // 元の SRC（GUI.bas AbilityListBox）: 合体技は IsCombinationAbilityAvailable(i, True) でなければ表示しない
            var u = CreateUser("回復Lv1", "合");
            // パートナーがいないので合体技は使用できない
            Assert.IsFalse(u.Ability(1).IsCombinationAbilityAvailable(true));
            Assert.IsFalse(u.Ability(1).IdDisplayFor(AbilityListMode.List));
        }

        [TestMethod]
        public void TargetWithinRange_CombinationWithoutPartner_IsFalse()
        {
            // 元の SRC: 合体技で射程1なら相手を囲むパートナーが必要。見つからなければ射程外
            var u = CreateUser("回復Lv1", "合");
            fx.AddUnitData("対象機");
            var t = fx.CreateUnit("対象機", "対象者");
            u.x = 5; u.y = 5;
            t.x = 5; t.y = 6;
            Assert.IsFalse(u.Ability(1).IsTargetWithinAbilityRange(t));
        }

        [TestMethod]
        public void Reload_SpecificWeapon_RefillsTargetWeapon()
        {
            // 元の SRC の「装填」（データ指定あり）は対象ユニット t の該当武器の弾数を最大にする
            var ud = fx.AddUnitData("使用機");
            var ad = ud.AddAbility("補給");
            ad.SetEffect("装填=ライフル");
            ad.MinRange = 1;
            ad.MaxRange = 1;
            var wd0 = ud.AddWeapon("ライフル");
            wd0.Bullet = 6;
            wd0.MaxRange = 3;
            var tud = fx.AddUnitData("対象機");
            var wd = tud.AddWeapon("ライフル");
            wd.Bullet = 6;
            wd.MaxRange = 3;
            var user = fx.CreateUnit("使用機", "使用者");
            var target = fx.CreateUnit("対象機", "対象者");
            user.Weapon(1).SetBullet(0);
            target.Weapon(1).SetBullet(0);

            user.ExecuteAbility(user.Ability(1), target);

            Assert.AreEqual(6, target.Weapon(1).Bullet());
            Assert.AreEqual(0, user.Weapon(1).Bullet());
        }
    }
}
