using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitAttack
{
    /// <summary>
    /// Unit.MapAttack（VB: Unit.cls MapAttack）を元の SRC の動作に合わせたことのテスト（7474/SRC#1502）。
    /// 乱数は FixDice(0.5) で固定し、武器は必中・クリティカル無しにする。
    /// </summary>
    [TestClass]
    public class UnitMapAttackVBMatchTests
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

        private static SpecialPowerData DefineSP(UnitAttackFixture f, string name, string duration, string effects)
        {
            var sd = f.Src.SPDList.Add(name);
            sd.ShortName = name;
            sd.KanaName = name;
            sd.SPConsumption = 10;
            sd.TargetType = "自分";
            sd.Duration = duration;
            sd.NecessaryCondition = "";
            sd.Animation = "";
            sd.SetEffect(effects);
            return sd;
        }

        // 元の SRC の並べ替えは安定でない選択ソートで、同じ距離のユニットの順が走査順と入れ替わる。
        // 走査順（x ごとに y の小さい順）は A(4,5)=1, B(5,3)=2, C(5,7)=2, D(6,5)=1。
        // i=1: A のまま。i=2: 最小の D と B を入れ替えて A,D,C,B。i=3: C と B は同じ距離なので C のまま。
        [TestMethod]
        public void MapAttack_SortsTargetsWithVBSelectionSort()
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 5, 5,
                weapons: new[] { f.Weapon("マップ兵器", 1500, "Ｍ全", 1, 2) });
            var ua = f.AddUnit("Ａ", "敵", 4, 5, hp: 5000);
            var ub = f.AddUnit("Ｂ", "敵", 5, 3, hp: 5000);
            var uc = f.AddUnit("Ｃ", "敵", 5, 7, hp: 5000);
            var ud = f.AddUnit("Ｄ", "敵", 6, 5, hp: 5000);

            a.MapAttack(a.Weapons.First(), 6, 5);

            var order = f.Calls(nameof(IGUI.UpdateMessageForm))
                .Where(c => c.Args.Length > 1 && ReferenceEquals(c.Args[1], a) && c.Args[0] != null && !ReferenceEquals(c.Args[0], a))
                .Select(c => (Unit)c.Args[0])
                .Distinct()
                .ToList();
            CollectionAssert.AreEqual(new[] { ua, ud, uc, ub }, order);
        }

        // かばわれた場合、元の SRC はターゲットをかばったユニットに置き換え、
        // 経験値・資金はかばったユニットについて計算する
        // （かばったユニットは攻撃側のフェイズ（IsDefense が偽）のときだけ SupportGuardUnit に記録される）
        [TestMethod]
        public void MapAttack_Guarded_TargetIsReplacedByGuardUnit()
        {
            var f = new UnitAttackFixture();
            f.Src.Stage = "味方";
            DefineSP(f, "みがわり", "みがわり", "みがわり");
            var a = f.AddUnit("攻撃機", "味方", 5, 5,
                weapons: new[] { f.Weapon("マップ兵器", 1500, "Ｍ全", 1, 2) });
            var enemy = f.AddUnit("敵機", "敵", 6, 5, hp: 5000);
            var guard = f.AddUnit("護衛機", "敵", 9, 9, hp: 500);
            f.Src.UDList.Item("護衛機").Value = 3000;
            enemy.MakeSpecialPowerInEffect("みがわり", guard.MainPilot().ID);
            var prevMoney = f.Src.Money;

            a.MapAttack(a.Weapons.First(), 6, 5);

            Assert.AreEqual(5000, enemy.HP);
            Assert.AreEqual("破壊", guard.Status);
            Assert.AreEqual(prevMoney + 1500, f.Src.Money);
        }

        // 資金の用語名は最後のターゲットの「用語名」で表示する（元の SRC の Term("資金", t)）
        [TestMethod]
        public void MapAttack_MoneyMessage_UsesLastTargetTerm()
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 5, 5,
                weapons: new[] { f.Weapon("マップ兵器", 1500, "Ｍ全", 1, 2) });
            f.AddUnit("敵機", "敵", 6, 5, hp: 500, features: new[] { "用語名=資金 ゴールド" });
            f.Src.UDList.Item("敵機").Value = 3000;

            a.MapAttack(a.Weapons.First(), 6, 5);

            CollectionAssert.Contains(f.SysMessages().ToList(), "1500のゴールドを得た。");
        }

        // 「敵破壊」のスペシャルパワーは、最後に破壊したユニットから得た資金が 0 なら解除しない
        [TestMethod]
        public void MapAttack_DestroyedWithoutEarnings_KeepsEnemyDestroySP()
        {
            var f = new UnitAttackFixture();
            DefineSP(f, "幸運", "敵破壊", "獲得資金増加Lv10");
            var a = f.AddUnit("攻撃機", "味方", 5, 5,
                weapons: new[] { f.Weapon("マップ兵器", 1500, "Ｍ全", 1, 2) });
            var enemy = f.AddUnit("敵機", "敵", 6, 5, hp: 500);
            f.Src.UDList.Item("敵機").Value = 1;
            a.MakeSpecialPowerInEffect("幸運");

            a.MapAttack(a.Weapons.First(), 6, 5);

            Assert.AreEqual("破壊", enemy.Status);
            Assert.IsTrue(a.IsSpecialPowerInEffect("幸運"));
        }

        [TestMethod]
        public void MapAttack_DestroyedWithEarnings_RemovesEnemyDestroySP()
        {
            var f = new UnitAttackFixture();
            DefineSP(f, "幸運", "敵破壊", "獲得資金増加Lv10");
            var a = f.AddUnit("攻撃機", "味方", 5, 5,
                weapons: new[] { f.Weapon("マップ兵器", 1500, "Ｍ全", 1, 2) });
            f.AddUnit("敵機", "敵", 6, 5, hp: 500);
            f.Src.UDList.Item("敵機").Value = 3000;
            a.MakeSpecialPowerInEffect("幸運");

            a.MapAttack(a.Weapons.First(), 6, 5);

            Assert.IsFalse(a.IsSpecialPowerInEffect("幸運"));
        }

        // 合体技のパートナーも、同名の武器が自属性なら破壊される
        [TestMethod]
        public void MapAttack_Combination_PartnerSelfDestructs()
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 5, 5,
                weapons: new[] { f.Weapon("合体自爆", 1500, "Ｍ全合自", 1, 2) },
                features: new[] { "合体技=合体自爆 僚機" });
            var p = f.AddUnit("僚機", "味方", 9, 9, weapons: new[] { f.Weapon("合体自爆", 1500, "Ｍ全合自", 1, 2) });
            f.AddUnit("敵機", "敵", 6, 5, hp: 5000);

            a.MapAttack(a.Weapons.First(), 6, 5);

            Assert.AreEqual("破壊", a.Status);
            Assert.AreEqual("破壊", p.Status);
        }

        // 合体技のパートナーに同名の武器が無ければ、攻撃側の武器の自属性で破壊される
        [TestMethod]
        public void MapAttack_Combination_PartnerWithoutWeapon_SelfDestructsByAttackerWeapon()
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 5, 5,
                weapons: new[] { f.Weapon("合体自爆", 1500, "Ｍ全合自", 1, 2) },
                features: new[] { "合体技=合体自爆 僚機" });
            var p = f.AddUnit("僚機", "味方", 9, 9);
            f.AddUnit("敵機", "敵", 6, 5, hp: 5000);

            a.MapAttack(a.Weapons.First(), 6, 5);

            Assert.AreEqual("破壊", p.Status);
        }

        // 使い捨てアイテムの武器を撃ち尽くすと、そのアイテムを削除する
        [TestMethod]
        public void MapAttack_ItemWeaponUsedUp_DeletesItem()
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 5, 5);
            var id = f.Src.IDList.Add("爆弾");
            id.Nickname = "爆弾";
            id.Class = "汎用";
            id.Part = "非表示";
            var wd = id.AddWeapon("投下爆弾");
            wd.Power = 1500;
            wd.MinRange = 1;
            wd.MaxRange = 2;
            wd.Precision = 1000;
            wd.Critical = -1000;
            wd.Bullet = 1;
            wd.Adaption = "AAAA";
            wd.Class = "Ｍ全";
            wd.NecessarySkill = "アイテム";
            wd.NecessaryCondition = "";
            var itm = f.Src.IList.Add("爆弾");
            a.AddItem(itm);
            a.Update();
            var enemy = f.AddUnit("敵機", "敵", 6, 5, hp: 5000);
            var w = a.Weapons.First(x => x.Name == "投下爆弾");

            a.MapAttack(w, 6, 5);

            Assert.IsTrue(enemy.HP < 5000);
            Assert.IsFalse(a.ItemList.Contains(itm));
            Assert.IsFalse(itm.Exist);
        }
    }
}
