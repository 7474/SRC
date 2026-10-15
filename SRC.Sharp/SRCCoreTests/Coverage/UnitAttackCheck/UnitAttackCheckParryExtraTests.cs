using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitAttackCheck
{
    /// <summary>
    /// Unit.CheckParryFeature（VB: Unit.cls CheckParryFeature）の迎撃・反射・当て身技と、
    /// 切り払いの細則（永続武器・思念誘導）、阻止のオプションのテスト。
    /// 発動率が 100% か 0% になる条件だけを使っている。
    /// </summary>
    [TestClass]
    public class UnitAttackCheckParryExtraTests
    {
        private const string Pilots = @"
攻撃者
攻撃者, こうげきしゃ, 男性, 汎用, AAAA, 100
特殊能力なし
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

感応者
感応者, かんのうしゃ, 男性, 汎用, AAAA, 100
特殊能力
超感覚Lv16, 1
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

迎撃手
迎撃手, げいげきしゅ, 男性, 汎用, AAAA, 100
特殊能力
迎撃Lv16, 1
切り払いLv8, 1
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

見習い迎撃手
見習い迎撃手, みならいげいげきしゅ, 男性, 汎用, AAAA, 100
特殊能力
迎撃Lv4, 1
切り払いLv8, 1
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

剣豪
剣豪, けんごう, 男性, 汎用, AAAA, 100
特殊能力
切り払いLv16, 1
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

感応剣豪
感応剣豪, かんのうけんごう, 男性, 汎用, AAAA, 100
特殊能力
切り払いLv16, 1
超感覚Lv16, 1
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

剣士
剣士, けんし, 男性, 汎用, AAAA, 100
特殊能力
切り払いLv8, 1
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

防御者
防御者, ぼうぎょしゃ, 男性, 汎用, AAAA, 100
特殊能力なし
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
ファンネル, 1500, 1, 3, +0, 3, -, -, AAAA, +0, 実永
誘導弾, 1500, 1, 3, +0, -, -, -, AAAA, +0, 実サ
パンチ, 1500, 1, 1, +0, -, -, -, AAAA, +0, 突
ビーム, 1500, 1, 3, +0, -, -, -, AAAA, +0, Ｂ
メガビーム, 1500, 1, 3, +0, -, -, -, AAAA, +0, ＢＭ直

迎撃機
迎撃機, げいげきき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力なし
3000, 200, 1000, 100
AAAA, -.bmp
ビームサーベル, 1200, 1, 1, +0, -, -, -, AAAA, +0, 武
バルカン, 500, 1, 2, +0, 10, -, -, AAAA, +0, 射Ｐ

指定迎撃機
指定迎撃機, していげいげきき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
迎撃武器=迎撃ミサイル
3000, 200, 1000, 100
AAAA, -.bmp
バルカン, 500, 1, 2, +0, 10, -, -, AAAA, +0, 射Ｐ
迎撃ミサイル, 800, 1, 4, +0, 2, -, -, AAAA, +0, 実

弾切れ迎撃機
弾切れ迎撃機, たまぎれげいげきき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力なし
3000, 200, 1000, 100
AAAA, -.bmp
バルカン, 500, 1, 2, +0, 9, -, -, AAAA, +0, 射Ｐ

剣持ち機
剣持ち機, けんもちき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力なし
3000, 200, 1000, 100
AAAA, -.bmp
ビームサーベル, 1200, 1, 1, +0, -, -, -, AAAA, +0, 武

近接無効阻止機
近接無効阻止機, きんせつむこうそしき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
阻止Lv1=阻止結界 全 100 0 0 近接無効
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 1200, 1, 1, +0, -, -, -, AAAA, +0, 突

手動阻止機
手動阻止機, しゅどうそしき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
阻止Lv1=阻止結界 全 100 0 0 手動
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 1200, 1, 1, +0, -, -, -, AAAA, +0, 突

反射機
反射機, はんしゃき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
反射Lv1=反射装甲 Ｂ 100 20
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 1200, 1, 1, +0, -, -, -, AAAA, +0, 突

無名反射機
無名反射機, むめいはんしゃき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
反射Lv1=反射 全 100
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 1200, 1, 1, +0, -, -, -, AAAA, +0, 突

当て身機
当て身機, あてみき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
当て身技Lv1=受け技 カウンター 突 100
3000, 200, 1000, 100
AAAA, -.bmp
カウンター, 1200, 1, 1, +0, -, -, -, AAAA, +0, 突
";

        private static UnitAttackCheckFixture CreateFixture()
        {
            var f = new UnitAttackCheckFixture();
            f.LoadPilots(Pilots);
            f.LoadUnits(Units);
            return f;
        }

        private static bool Parry(Unit a, string wname, ref Unit t, string attack_mode = "", string def_mode = "", int dmg = 1500)
        {
            var msg = "";
            var w = a.Weapons.First(x => x.Name == wname);
            return a.CheckParryFeature(w, ref t, t.x, t.y, attack_mode, def_mode, dmg, ref msg, true);
        }

        private static bool Parry(Unit a, string wname, Unit t, string attack_mode = "", string def_mode = "", int dmg = 1500)
        {
            return Parry(a, wname, ref t, attack_mode, def_mode, dmg);
        }

        private static UnitWeapon WeaponOf(Unit u, string name)
        {
            return u.Weapons.First(x => x.Name == name);
        }

        // ──────────────────────────────────────────────
        // 迎撃
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Intercept_AutoSelectedShootingWeapon_StopsMissileAndUsesBullet()
        {
            // 迎撃Lv(16) > 切り払いLv(8) なら切り払い用武器があっても迎撃を優先する。
            // 迎撃武器の指定がなければ、移動後攻撃可・射撃系・弾数10以上の武器を自動で選ぶ
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("迎撃機", "迎撃手", "敵", 2, 1);

            Assert.IsTrue(Parry(a, "ミサイル", t));
            CollectionAssert.Contains(f.SysMessages(), "迎撃機は[バルカン]で[ミサイル]を阻止した。");
            Assert.AreEqual(9, WeaponOf(t, "バルカン").Bullet());
        }

        [TestMethod]
        public void Intercept_DesignatedWeapon_IsUsedEvenIfNotAutoCandidate()
        {
            // 迎撃武器で指定された武器は自動選択の条件（移動後攻撃可・弾数10以上）を満たさなくても使う
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("指定迎撃機", "迎撃手", "敵", 2, 1);

            Assert.IsTrue(Parry(a, "ミサイル", t));
            CollectionAssert.Contains(f.SysMessages(), "指定迎撃機は[迎撃ミサイル]で[ミサイル]を阻止した。");
            Assert.AreEqual(1, WeaponOf(t, "迎撃ミサイル").Bullet());
            Assert.AreEqual(10, WeaponOf(t, "バルカン").Bullet());
        }

        [TestMethod]
        public void Intercept_NoSuitableWeapon_NotIntercepted()
        {
            // 弾数が10未満の武器は迎撃用に選ばれない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("弾切れ迎撃機", "迎撃手", "敵", 2, 1);

            Assert.IsFalse(Parry(a, "ミサイル", t));
            Assert.AreEqual(9, WeaponOf(t, "バルカン").Bullet());
        }

        [TestMethod]
        public void Intercept_NonPhysicalWeapon_NotIntercepted()
        {
            // 迎撃できるのは実属性の武器だけ
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("迎撃機", "迎撃手", "敵", 2, 1);

            Assert.IsFalse(Parry(a, "ビーム", t));
            Assert.AreEqual(10, WeaponOf(t, "バルカン").Bullet());
        }

        [TestMethod]
        public void Intercept_LowerThanParryLevel_ParryTakesPrecedence()
        {
            // 迎撃Lv(4) <= 切り払いLv(8) で切り払い用武器があれば切り払いが優先される。
            // 見切り（特殊防御発動）で切り払いを確定させて確かめる
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("迎撃機", "見習い迎撃手", "敵", 2, 1);
            f.ApplySpecialPower(t, "見切りテスト", "特殊防御発動");

            Assert.IsTrue(Parry(a, "ミサイル", t));
            CollectionAssert.Contains(f.SysMessages(), "迎撃機は[ビームサーベル]で[ミサイル]を叩き落とした。");
            Assert.AreEqual(10, WeaponOf(t, "バルカン").Bullet());
        }

        // ──────────────────────────────────────────────
        // 切り払いの細則
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Parry_PermanentWeapon_ConsumesAttackerBullet()
        {
            // 切り払われた永続武器（永）は使用回数が減る
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("剣持ち機", "剣豪", "敵", 2, 1);

            Assert.IsTrue(Parry(a, "ファンネル", t));
            Assert.AreEqual(2, WeaponOf(a, "ファンネル").Bullet());
        }

        [TestMethod]
        public void Parry_NonPermanentWeapon_KeepsAttackerBullet()
        {
            // 永属性がなければ切り払われても弾数は変わらない（弾数無制限の武器は 0 のまま）
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("剣持ち機", "剣豪", "敵", 2, 1);
            var before = WeaponOf(a, "ミサイル").Bullet();

            Assert.IsTrue(Parry(a, "ミサイル", t));
            Assert.AreEqual(before, WeaponOf(a, "ミサイル").Bullet());
        }

        [TestMethod]
        public void Parry_PsychoGuided_FromNonNewtype_UsesFullRate()
        {
            // 思念誘導（サ）でも攻撃側に超感覚がなければ 2×切り払いLv（16なら32/32）
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("剣持ち機", "剣豪", "敵", 2, 1);

            Assert.IsTrue(Parry(a, "誘導弾", t));
            CollectionAssert.Contains(f.SysMessages(), "剣持ち機は[ビームサーベル]で[誘導弾]を叩き落とした。");
        }

        [TestMethod]
        public void Parry_PsychoGuided_ReducedByAttackerNewtypeLevel()
        {
            // 思念誘導は (防御側の超感覚 − 攻撃側の超感覚)（正なら0）だけ切り払い確率が下がる。
            // 攻撃側 超感覚Lv16 対 防御側 切り払いLv8 なら 2×8−16 = 0 で発動しない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "感応者", "味方", 1, 1);
            var t = f.Place("剣持ち機", "剣士", "敵", 2, 1);

            Assert.IsFalse(Parry(a, "誘導弾", t));
            Assert.AreEqual(0, f.SysMessages().Length);
        }

        [TestMethod]
        public void Parry_PsychoGuided_DefenderNewtypeCancelsReduction()
        {
            // 防御側も同じ超感覚Lvを持てば差は0になり、2×16 = 32/32 で必ず切り払う
            var f = CreateFixture();
            var a = f.Place("攻撃機", "感応者", "味方", 1, 1);
            var t = f.Place("剣持ち機", "感応剣豪", "敵", 2, 1);

            Assert.IsTrue(Parry(a, "誘導弾", t));
        }

        // ──────────────────────────────────────────────
        // 阻止のオプション
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Block_MeleeInvalidOption_DoesNotBlockMelee()
        {
            // 近接無効: 武・突・接属性の攻撃は阻止できない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("近接無効阻止機", "防御者", "敵", 2, 1);

            Assert.IsFalse(Parry(a, "パンチ", t));
            Assert.IsTrue(Parry(a, "ビーム", t));
        }

        [TestMethod]
        public void Block_ManualOption_OnlyWhenDefending()
        {
            // 手動: 防御を選んだときだけ発動する
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("手動阻止機", "防御者", "敵", 2, 1);

            Assert.IsFalse(Parry(a, "ビーム", t, def_mode: "回避"));
            Assert.IsTrue(Parry(a, "ビーム", t, def_mode: "防御"));
        }

        // ──────────────────────────────────────────────
        // 反射
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Reflect_MapWeapon_ReflectsAndConsumesEN()
        {
            // 反射Lv1（ダメージ上限なし）・Ｂ属性・確率100・ＥＮ20。
            // マップ攻撃（Ｍ）は弾き返すだけで撃ち返しはしない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("反射機", "防御者", "敵", 2, 1);
            var en = t.EN;
            var hp = a.HP;

            Assert.IsTrue(Parry(a, "メガビーム", t));
            Assert.AreEqual(en - 20, t.EN);
            Assert.AreEqual(hp, a.HP);
            CollectionAssert.Contains(f.SysMessages(), "反射機は[反射装甲]で[メガビーム]を弾き返した。");
        }

        [TestMethod]
        public void Reflect_AttributeMismatch_NotReflected()
        {
            // 対象属性（Ｂ）を持たない攻撃は反射しない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("反射機", "防御者", "敵", 2, 1);
            var en = t.EN;

            Assert.IsFalse(Parry(a, "ミサイル", t));
            Assert.AreEqual(en, t.EN);
        }

        [TestMethod]
        public void Reflect_NotEnoughEN_NotReflected()
        {
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("反射機", "防御者", "敵", 2, 1);
            t.EN = 19;

            Assert.IsFalse(Parry(a, "メガビーム", t));
            Assert.AreEqual(19, t.EN);
        }

        [TestMethod]
        public void Reflect_Unnamed_DefaultMessage()
        {
            // 表示名が「反射」のときは名称を省いて「〇〇は[武器]を弾き返した。」と表示する
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("無名反射機", "防御者", "敵", 2, 1);

            Assert.IsTrue(Parry(a, "メガビーム", t));
            CollectionAssert.Contains(f.SysMessages(), "無名反射機は[メガビーム]を弾き返した。");
        }

        [TestMethod]
        public void Reflect_DefenseNullified_NotReflected()
        {
            // 防御能力無効化下の攻撃は反射できない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("無名反射機", "防御者", "敵", 2, 1);
            f.ApplySpecialPower(a, "貫通テスト", "防御能力無効化");

            Assert.IsFalse(Parry(a, "メガビーム", t));
        }

        // ──────────────────────────────────────────────
        // 当て身技
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Counter_AttributeMatched_ReceivesAndCounterAttacks()
        {
            // 当て身技は対象属性（突）の攻撃を受け止め、指定の武器で反撃する
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            f.FillPlainTerrain();
            var t = f.Place("当て身機", "防御者", "敵", 2, 1);

            Assert.IsTrue(Parry(a, "パンチ", ref t));
            CollectionAssert.Contains(f.SysMessages(), "当て身機は[受け技]で[パンチ]を受け止めた。");
        }

        [TestMethod]
        public void Counter_AttributeMismatch_NotTriggered()
        {
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("当て身機", "防御者", "敵", 2, 1);

            Assert.IsFalse(Parry(a, "ビーム", t));
            Assert.AreEqual(0, f.SysMessages().Length);
        }

        [TestMethod]
        public void Counter_AgainstReflectedAttack_NotTriggered()
        {
            // 反射や当て身技による攻撃に対しては当て身技を使えない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("当て身機", "防御者", "敵", 2, 1);

            Assert.IsFalse(Parry(a, "パンチ", t, attack_mode: "反射"));
            Assert.IsFalse(Parry(a, "パンチ", t, attack_mode: "当て身技"));
        }
    }
}
