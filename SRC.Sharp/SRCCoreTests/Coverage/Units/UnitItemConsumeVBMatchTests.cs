using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Coverage.Tests.UnitAttack;
using SRCCore.Items;
using SRCCore.Models;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// 使い切った武器・アビリティのアイテムを消すとき、元の SRC と同じく
    /// 武器・アビリティの番号がどのアイテムの範囲に入るかでアイテムを決めることのテスト（7474/SRC#1508）。
    /// VB: Unit.cls の Attack・MapAttack・ExecuteAbility・ExecuteMapAbility の「アイテムを消費」。
    /// 同名の武器・アビリティを持つアイテムを 2 つ持ち、2 つ目のアイテムのものを使い切ると、2 つ目のアイテムが消える。
    /// </summary>
    [TestClass]
    public class UnitItemConsumeVBMatchTests
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

        private static Item AddWeaponItem(UnitAttackFixture f, string iname, string wname, string wclass)
        {
            var id = f.Src.IDList.Add(iname);
            id.Nickname = iname;
            id.Class = "汎用";
            id.Part = "非表示";
            var wd = id.AddWeapon(wname);
            wd.Power = 1500;
            wd.MinRange = 1;
            wd.MaxRange = 2;
            wd.Precision = 1000;
            wd.Critical = -1000;
            wd.Bullet = 1;
            wd.Adaption = "AAAA";
            wd.Class = wclass;
            wd.NecessarySkill = "アイテム";
            wd.NecessaryCondition = "";
            return f.Src.IList.Add(iname);
        }

        private static Item AddAbilityItem(UnitAttackFixture f, string iname, string aname, string aclass, int maxRange)
        {
            var id = f.Src.IDList.Add(iname);
            id.Nickname = iname;
            id.Class = "汎用";
            id.Part = "非表示";
            var ad = id.AddAbility(aname);
            ad.SetEffect("回復Lv1");
            ad.Class = aclass;
            ad.MinRange = maxRange == 0 ? 0 : 1;
            ad.MaxRange = maxRange;
            ad.Stock = 1;
            ad.NecessarySkill = "アイテム";
            ad.NecessaryCondition = "";
            return f.Src.IList.Add(iname);
        }

        [TestMethod]
        public void Attack_SecondItemWeaponUsedUp_DeletesSecondItem()
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 5, 5);
            var itm1 = AddWeaponItem(f, "爆弾Ａ", "爆弾", "");
            var itm2 = AddWeaponItem(f, "爆弾Ｂ", "爆弾", "");
            a.AddItem(itm1);
            a.AddItem(itm2);
            a.Update();
            var t = f.AddUnit("敵機", "敵", 6, 5, hp: 5000);
            var w = a.Weapons.Where(x => x.Name == "爆弾").ElementAt(1);

            a.Attack(w, t, "", "");

            Assert.IsTrue(a.ItemList.Contains(itm1));
            Assert.IsTrue(itm1.Exist);
            Assert.IsFalse(a.ItemList.Contains(itm2));
            Assert.IsFalse(itm2.Exist);
        }

        [TestMethod]
        public void MapAttack_SecondItemWeaponUsedUp_DeletesSecondItem()
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 5, 5);
            var itm1 = AddWeaponItem(f, "爆弾Ａ", "投下爆弾", "Ｍ全");
            var itm2 = AddWeaponItem(f, "爆弾Ｂ", "投下爆弾", "Ｍ全");
            a.AddItem(itm1);
            a.AddItem(itm2);
            a.Update();
            f.AddUnit("敵機", "敵", 6, 5, hp: 5000);
            var w = a.Weapons.Where(x => x.Name == "投下爆弾").ElementAt(1);

            a.MapAttack(w, 6, 5);

            Assert.IsTrue(a.ItemList.Contains(itm1));
            Assert.IsFalse(a.ItemList.Contains(itm2));
            Assert.IsFalse(itm2.Exist);
        }

        [TestMethod]
        public void ExecuteAbility_SecondItemAbilityUsedUp_DeletesSecondItem()
        {
            var f = new UnitAttackFixture();
            var u = f.AddUnit("使用機", "味方", 5, 5);
            var itm1 = AddAbilityItem(f, "薬Ａ", "回復薬", "", 0);
            var itm2 = AddAbilityItem(f, "薬Ｂ", "回復薬", "", 0);
            u.AddItem(itm1);
            u.AddItem(itm2);
            u.Update();
            u.HP = 300;
            var a = u.Abilities.Where(x => x.Data.Name == "回復薬").ElementAt(1);

            u.ExecuteAbility(a, u);

            Assert.IsTrue(u.ItemList.Contains(itm1));
            Assert.IsTrue(itm1.Exist);
            Assert.IsFalse(u.ItemList.Contains(itm2));
            Assert.IsFalse(itm2.Exist);
        }

        [TestMethod]
        public void ExecuteMapAbility_SecondItemAbilityUsedUp_DeletesSecondItem()
        {
            var f = new UnitAttackFixture();
            var u = f.AddUnit("使用機", "味方", 5, 5);
            var itm1 = AddAbilityItem(f, "薬Ａ", "全体回復薬", "Ｍ全", 2);
            var itm2 = AddAbilityItem(f, "薬Ｂ", "全体回復薬", "Ｍ全", 2);
            u.AddItem(itm1);
            u.AddItem(itm2);
            u.Update();
            var ally = f.AddUnit("僚機", "味方", 6, 5);
            ally.HP = 300;
            var a = u.Abilities.Where(x => x.Data.Name == "全体回復薬").ElementAt(1);

            u.ExecuteMapAbility(a, 5, 5, true);

            Assert.IsTrue(u.ItemList.Contains(itm1));
            Assert.IsFalse(u.ItemList.Contains(itm2));
            Assert.IsFalse(itm2.Exist);
        }
    }
}
