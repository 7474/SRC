using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitAttack
{
    /// <summary>
    /// Unit.Attack（Unit.attack.cs / VB: Unit.cls Attack）の追加テスト：
    /// 合体技パートナーの消費と副作用、援護攻撃時の気力、てかげん、吹き飛ばし、
    /// レベル付きパーツ分離、失・変属性、盗・習の成功時。
    /// 乱数は FixDice(0.5) で固定する（Dice(100)=51, Dice(16)=9, Dice(8)=5）。
    /// </summary>
    [TestClass]
    public class UnitAttackR2AttackTests
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

        // 他形態として使うユニットデータを登録する
        private static void AddFormData(UnitAttackFixture f, string name, int hp = 800)
        {
            var ud = f.Src.UDList.Add(name);
            ud.Nickname = name;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.HP = hp;
            ud.EN = 100;
            ud.Speed = 5;
            ud.Size = "M";
            ud.PilotNum = 1;
            ud.Class = "汎用";
        }

        #region 合体技パートナーの消費と副作用

        // パートナーが同名の武器を持たない場合は攻撃側の武器データでＥＮ消費と消属性の消耗を適用する
        // （VB: Unit.cls Attack「同名の武器がなかった場合は自分のデータを使って処理」）
        [TestMethod]
        public void Attack_Combination_PartnerWithoutWeapon_UsesAttackerWeaponData()
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 2, 2,
                weapons: new[] { f.Weapon("合体剣", 1500, "合消", 1, 3, en: 20) },
                features: new[] { "合体技=合体剣 僚機" });
            var p = f.AddUnit("僚機", "味方", 6, 8);
            var t = f.AddUnit("標的機", "敵", 3, 2, hp: 5000);

            a.Attack(a.Weapons.First(), t, "", "");

            Assert.IsTrue(t.HP < 5000);
            Assert.AreEqual(80, a.EN);
            Assert.AreEqual(80, p.EN);
            Assert.IsTrue(p.IsConditionSatisfied("消耗"));
        }

        // パートナー側の同名武器が自属性ならパートナーが破壊される（攻撃側の武器は自属性でないので攻撃側は残る）
        [TestMethod]
        public void Attack_Combination_PartnerSelfDestructWeapon_DestroysPartnerOnly()
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 2, 2,
                weapons: new[] { f.Weapon("合体剣", 1500, "合", 1, 3) },
                features: new[] { "合体技=合体剣 僚機" });
            var p = f.AddUnit("僚機", "味方", 6, 8,
                weapons: new[] { f.Weapon("合体剣", 1500, "合自", 1, 3) });
            var t = f.AddUnit("標的機", "敵", 3, 2, hp: 5000);

            a.Attack(a.Weapons.First(), t, "", "");

            Assert.AreEqual("破壊", p.Status);
            Assert.AreEqual("出撃", a.Status);
        }

        // パートナー側の同名武器が変属性なら、変形技で指定された形態に変形する
        [TestMethod]
        public void Attack_Combination_PartnerTransformWeapon_TransformsPartner()
        {
            var f = new UnitAttackFixture();
            AddFormData(f, "僚機改");
            var a = f.AddUnit("攻撃機", "味方", 2, 2,
                weapons: new[] { f.Weapon("合体剣", 1500, "合", 1, 3) },
                features: new[] { "合体技=合体剣 僚機" });
            var p = f.AddUnit("僚機", "味方", 6, 8,
                weapons: new[] { f.Weapon("合体剣", 1500, "合変", 1, 3) },
                features: new[] { "変形技=合体剣 僚機改" });
            var t = f.AddUnit("標的機", "敵", 3, 2, hp: 5000);

            a.Attack(a.Weapons.First(), t, "", "");

            Assert.AreEqual("僚機改", p.CurrentForm().Name);
            Assert.AreEqual("出撃", p.CurrentForm().Status);
            Assert.AreEqual("攻撃機", a.CurrentForm().Name);
        }

        // パートナーが同名の武器を持たず攻撃側の武器が自属性の場合、
        // パートナーはパーツ分離の第 2 引数（分離後の形態）に変化してＨＰ全快で残る
        // （VB: Unit.cls Attack の j > .CountWeapon 側も LIndex(.FeatureData("パーツ分離"), 2)）
        [TestMethod]
        public void Attack_Combination_PartnerWithoutWeapon_SelfDestructSeparatesParts()
        {
            var f = new UnitAttackFixture();
            AddFormData(f, "僚機コア", 800);
            var a = f.AddUnit("攻撃機", "味方", 2, 2,
                weapons: new[] { f.Weapon("合体剣", 1500, "合自", 1, 3) },
                features: new[] { "合体技=合体剣 僚機" });
            var p = f.AddUnit("僚機", "味方", 6, 8, features: new[] { "パーツ分離=分離 僚機コア" });
            var t = f.AddUnit("標的機", "敵", 3, 2, hp: 5000);

            a.Attack(a.Weapons.First(), t, "", "");

            var cf = p.CurrentForm();
            Assert.AreEqual("僚機コア", cf.Name);
            Assert.AreEqual("出撃", cf.Status);
            Assert.AreEqual(800, cf.HP);
        }

        #endregion

        // 援護攻撃で標的を破壊した場合、破壊による気力 +3 は援護した本人ではなく援護された攻撃ユニット（AttackUnit）が得る
        // （VB: Unit.cls Attack「If InStr(attack_mode, "援護攻撃") > 0 Then AttackUnit.CurrentForm.IncreaseMorale 3」）
        [TestMethod]
        public void Attack_SupportAttack_DestroyMoraleGoesToAttackUnit()
        {
            var f = new UnitAttackFixture();
            var main = f.AddUnit("主攻撃機", "味方", 2, 3);
            var a = f.AddUnit("援護機", "味方", 2, 2, weapons: new[] { f.Weapon("ビームライフル", 1500, "", 1, 3) });
            var t = f.AddUnit("標的機", "敵", 3, 2, hp: 500);
            f.Src.Commands.AttackUnit = main;

            a.Attack(a.Weapons.First(), t, "援護攻撃", "");

            Assert.AreEqual("破壊", t.Status);
            // 主攻撃機：+3 と同陣営 +1
            Assert.AreEqual(104, main.MainPilot().Morale);
            // 援護機：同陣営 +1 のみ
            Assert.AreEqual(101, a.MainPilot().Morale);
        }

        private static void AddTegakenSp(UnitAttackFixture f, Unit a)
        {
            var sp = f.Src.SPDList.Add("てかげん");
            sp.ShortName = "て";
            sp.Duration = "ターン";
            sp.SetEffect("てかげん");
            a.MakeSpecialPowerInEffect("てかげん");
        }

        // てかげん：攻撃側の技量が標的より高ければＨＰ10 を残して破壊しない
        [TestMethod]
        public void Attack_Tegaken_HigherTechniqueLeavesTenHp()
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 2, 2, weapons: new[] { f.Weapon("ビームライフル", 1500, "", 1, 3) });
            var t = f.AddUnit("標的機", "敵", 3, 2, hp: 500);
            f.Src.PDList.Item("攻撃機パイロット").Technique = 150;
            a.MainPilot().Update();
            AddTegakenSp(f, a);
            Assert.IsTrue(a.MainPilot().Technique > t.MainPilot().Technique);

            a.Attack(a.Weapons.First(), t, "", "");

            Assert.AreEqual(10, t.HP);
            Assert.AreEqual("出撃", t.Status);
        }

        // 技量が同じなら てかげん は働かず破壊される
        [TestMethod]
        public void Attack_Tegaken_EqualTechniqueDestroys()
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 2, 2, weapons: new[] { f.Weapon("ビームライフル", 1500, "", 1, 3) });
            var t = f.AddUnit("標的機", "敵", 3, 2, hp: 500);
            AddTegakenSp(f, a);

            a.Attack(a.Weapons.First(), t, "", "");

            Assert.AreEqual("破壊", t.Status);
        }

        // 吹属性：標的を攻撃側から離れる方向にレベル分だけ吹き飛ばす
        [TestMethod]
        public void Attack_Blow_PushesTargetAway()
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 2, 2, weapons: new[] { f.Weapon("ハンマー", 1500, "吹L2", 1, 1) });
            var t = f.AddUnit("標的機", "敵", 3, 2, hp: 5000);
            var w = a.Weapons.First();
            var dmg = w.Damage(t, true);

            a.Attack(w, t, "", "");

            Assert.AreEqual(5, t.x);
            Assert.AreEqual(2, t.y);
            Assert.AreSame(t, f.Src.Map.MapDataForUnit[5, 2]);
            Assert.IsNull(f.Src.Map.MapDataForUnit[3, 2]);
            Assert.AreEqual(5000 - dmg, t.HP);
            StringAssert.StartsWith(f.SysMessages().Last(), "標的機を吹き飛ばした。;");
        }

        // 吹き飛ばし先にユニットがいると激突し、標的は追加ダメージ、巻き添えのユニットもダメージを受ける
        [TestMethod]
        public void Attack_Blow_CrashIntoUnitAddsDamage()
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 2, 2, weapons: new[] { f.Weapon("ハンマー", 1500, "吹L2", 1, 1) });
            var t = f.AddUnit("標的機", "敵", 3, 2, hp: 5000);
            var blocker = f.AddUnit("障害機", "敵", 5, 2, hp: 5000);
            var w = a.Weapons.First();
            var dmg = w.Damage(t, true);

            a.Attack(w, t, "", "");

            // 1 マスだけ進んで止まる
            Assert.AreEqual(4, t.x);
            // 装甲 0 なので激突の追加ダメージは元のダメージの半分
            Assert.AreEqual(5000 - (dmg + dmg / 2), t.HP);
            Assert.AreEqual(5000 - dmg / 2, blocker.HP);
            Assert.AreEqual(5, blocker.x);
        }

        // レベル付きパーツ分離は Dice(100) <= 10 * レベル の場合のみ発動する（Dice(100)=51）
        [DataTestMethod]
        [DataRow(6, true)]
        [DataRow(5, false)]
        public void Attack_Destroy_PartsSeparationWithLevel(int level, bool separates)
        {
            var f = new UnitAttackFixture();
            AddFormData(f, "標的機コア", 800);
            var a = f.AddUnit("攻撃機", "味方", 2, 2, weapons: new[] { f.Weapon("ビームライフル", 1500, "", 1, 3) });
            var t = f.AddUnit("標的機", "敵", 3, 2, hp: 500,
                features: new[] { "パーツ分離Lv" + level + "=分離 標的機コア" });

            a.Attack(a.Weapons.First(), t, "", "");

            if (separates)
            {
                Assert.AreEqual("標的機コア", t.CurrentForm().Name);
                Assert.AreEqual("出撃", t.CurrentForm().Status);
            }
            else
            {
                Assert.AreEqual("標的機", t.CurrentForm().Name);
                Assert.AreEqual("破壊", t.Status);
            }
        }

        // 失属性：使用時に最大ＨＰ×レベル/10 を失い、ＨＰが 0 になれば攻撃後に破壊される
        [DataTestMethod]
        [DataRow(10, 0, "破壊")]
        [DataRow(5, 500, "出撃")]
        public void Attack_HpCostWeapon_KillsAttackerAtZeroHp(int level, int expectedHp, string expectedStatus)
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 2, 2, weapons: new[] { f.Weapon("捨て身", 1500, "失L" + level, 1, 3) });
            var t = f.AddUnit("標的機", "敵", 3, 2, hp: 5000);

            a.Attack(a.Weapons.First(), t, "", "");

            Assert.IsTrue(t.HP < 5000);
            Assert.AreEqual(expectedHp, a.HP);
            Assert.AreEqual(expectedStatus, a.Status);
        }

        // 変属性：変形技で指定された形態に攻撃後変形する
        [TestMethod]
        public void Attack_TransformWeapon_TransformsAttacker()
        {
            var f = new UnitAttackFixture();
            AddFormData(f, "攻撃機改");
            var a = f.AddUnit("攻撃機", "味方", 2, 2,
                weapons: new[] { f.Weapon("変形斬り", 1500, "変", 1, 3) },
                features: new[] { "変形技=変形斬り 攻撃機改" });
            var t = f.AddUnit("標的機", "敵", 3, 2, hp: 5000);

            a.Attack(a.Weapons.First(), t, "", "");

            Assert.IsTrue(t.HP < 5000);
            Assert.AreEqual("攻撃機改", a.CurrentForm().Name);
            Assert.AreEqual(2, a.CurrentForm().x);
            Assert.AreEqual(2, a.CurrentForm().y);
        }

        // 変属性で変形技が無い場合はノーマルモードの形態に戻る
        [TestMethod]
        public void Attack_TransformWeapon_ReturnsToNormalMode()
        {
            var f = new UnitAttackFixture();
            AddFormData(f, "攻撃機通常");
            var a = f.AddUnit("攻撃機", "味方", 2, 2,
                weapons: new[] { f.Weapon("全力攻撃", 1500, "変", 1, 3) },
                features: new[] { "ノーマルモード=攻撃機通常" });
            var t = f.AddUnit("標的機", "敵", 3, 2, hp: 5000);

            a.Attack(a.Weapons.First(), t, "", "");

            Assert.AreEqual("攻撃機通常", a.CurrentForm().Name);
        }

        // 盗属性：アイテムを持たない敵からは資金（修理費の 1/4）を盗み、以後はすかんぴんになる
        [TestMethod]
        public void Attack_Steal_Money()
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 2, 2,
                weapons: new[] { f.Weapon("スティール", 0, "盗", 1, 3, critical: 1000) });
            var t = f.AddUnit("標的機", "敵", 3, 2, hp: 5000);
            f.Src.UDList.Item("標的機").Value = 3000;
            var prevMoney = f.Src.Money;

            a.Attack(a.Weapons.First(), t, "", "");

            Assert.AreEqual(prevMoney + 750, f.Src.Money);
            Assert.IsTrue(t.IsConditionSatisfied("すかんぴん"));
            Assert.IsTrue(f.SysMessages().Last().Contains("[標的機]から資金750を盗んだ。"));
        }

        // 盗属性：アイテム所有の敵からはそのアイテムを盗む（資金は増えない）
        [TestMethod]
        public void Attack_Steal_Item()
        {
            var f = new UnitAttackFixture();
            var id = f.Src.IDList.Add("宝石");
            id.Nickname = "輝く宝石";
            var a = f.AddUnit("攻撃機", "味方", 2, 2,
                weapons: new[] { f.Weapon("スティール", 0, "盗", 1, 3, critical: 1000) });
            var t = f.AddUnit("標的機", "敵", 3, 2, hp: 5000, features: new[] { "アイテム所有=宝石" });
            f.Src.UDList.Item("標的機").Value = 3000;
            var prevMoney = f.Src.Money;

            a.Attack(a.Weapons.First(), t, "", "");

            Assert.IsTrue(f.Src.IList.List.Any(x => x.Name == "宝石"));
            Assert.AreEqual(prevMoney, f.Src.Money);
            Assert.IsTrue(f.SysMessages().Last().Contains("[標的機]から輝く宝石を盗んだ。"));
        }

        // 習属性：ラーニング可能技を持つ敵から技を習得し、Ability 変数に記録する
        [TestMethod]
        public void Attack_Learn_RecordsAbilityVariable()
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 2, 2,
                weapons: new[] { f.Weapon("ラーニング", 0, "習", 1, 3, critical: 1000) });
            var t = f.AddUnit("標的機", "敵", 3, 2, hp: 5000, features: new[] { "ラーニング可能技=底力" });
            var pid = a.MainPilot().ID;

            a.Attack(a.Weapons.First(), t, "", "");

            Assert.AreEqual("底力", f.Src.Expression.GetValueAsString("Ability(" + pid + ")"));
            // データが 1 要素だけの場合は非表示で習得する
            Assert.AreEqual("-1 非表示", f.Src.Expression.GetValueAsString("Ability(" + pid + ",底力)"));
            Assert.IsTrue(f.SysMessages().Last().Contains("[攻撃機パイロット]は「底力」を習得した。"));
        }
    }
}
