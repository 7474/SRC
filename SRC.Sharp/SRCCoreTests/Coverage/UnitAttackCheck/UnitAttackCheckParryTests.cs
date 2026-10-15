using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitAttackCheck
{
    /// <summary>
    /// Unit.CheckParryFeature（VB: Unit.cls CheckParryFeature）の切り払い・阻止・迎撃のテスト。
    /// 発動率が 100% か 0% になる条件だけを使っている。
    /// </summary>
    [TestClass]
    public class UnitAttackCheckParryTests
    {
        private const string Pilots = @"
攻撃者
攻撃者, こうげきしゃ, 男性, 汎用, AAAA, 100
特殊能力なし
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
パンチ, 1500, 1, 1, +0, -, -, -, AAAA, +0, 突
斬撃, 1500, 1, 1, +0, -, -, -, AAAA, +0, 武
掌底, 1500, 1, 1, +0, -, -, -, AAAA, +0, 突接
無属性砲, 1500, 1, 3, +0, -, -, -, AAAA, +0, 無
ビーム, 1500, 1, 3, +0, -, -, -, AAAA, +0, Ｂ

剣持ち機
剣持ち機, けんもちき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力なし
3000, 200, 1000, 100
AAAA, -.bmp
ビームサーベル, 1200, 1, 1, +0, -, -, -, AAAA, +0, 武

素手機
素手機, すでき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力なし
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 1200, 1, 1, +0, -, -, -, AAAA, +0, 突

阻止機
阻止機, そしき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
阻止Lv1=阻止結界 全 100 10
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 1200, 1, 1, +0, -, -, -, AAAA, +0, 突

弱阻止機
弱阻止機, じゃくそしき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
阻止Lv2=阻止結界 Ｂ 100 10
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 1200, 1, 1, +0, -, -, -, AAAA, +0, 突
";

        private static UnitAttackCheckFixture CreateFixture()
        {
            var f = new UnitAttackCheckFixture();
            f.LoadPilots(Pilots);
            f.LoadUnits(Units);
            return f;
        }

        private static bool Parry(Unit a, string wname, Unit t, int dmg = 1500)
        {
            var msg = "";
            var w = a.Weapons.First(x => x.Name == wname);
            return a.CheckParryFeature(w, ref t, t.x, t.y, "", "", dmg, ref msg, true);
        }

        // ──────────────────────────────────────────────
        // 切り払い
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Parry_PhysicalProjectile_IsShotDown()
        {
            // 実弾は 2×切り払いLv（16なら32/32）で叩き落とす
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("剣持ち機", "剣豪", "敵", 2, 1);

            Assert.IsTrue(Parry(a, "ミサイル", t));
            CollectionAssert.Contains(f.SysMessages(), "剣持ち機は[ビームサーベル]で[ミサイル]を叩き落とした。");
        }

        [TestMethod]
        public void Parry_PunchWeapon_IsParried()
        {
            // 突属性は「受け流した」
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("剣持ち機", "剣豪", "敵", 2, 1);

            Assert.IsTrue(Parry(a, "パンチ", t));
            CollectionAssert.Contains(f.SysMessages(), "剣持ち機は[ビームサーベル]で[パンチ]を受け流した。");
        }

        [TestMethod]
        public void Parry_SwordWeapon_IsBlocked()
        {
            // 武属性は「受けとめた」
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("剣持ち機", "剣豪", "敵", 2, 1);

            Assert.IsTrue(Parry(a, "斬撃", t));
            CollectionAssert.Contains(f.SysMessages(), "剣持ち機は[ビームサーベル]で[斬撃]を受けとめた。");
        }

        [TestMethod]
        public void Parry_ContactWeapon_CannotBeParried()
        {
            // 接属性は突属性を持っていても切り払いの対象外
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("剣持ち機", "剣豪", "敵", 2, 1);

            Assert.IsFalse(Parry(a, "掌底", t));
            Assert.AreEqual(0, f.SysMessages().Length);
        }

        [TestMethod]
        public void Parry_BeamWeapon_CannotBeParried()
        {
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("剣持ち機", "剣豪", "敵", 2, 1);

            Assert.IsFalse(Parry(a, "ビーム", t));
        }

        [TestMethod]
        public void Parry_WithoutSwordWeapon_Fails()
        {
            // 武属性の武器（切り払い用武器）を持たなければ切り払えない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("素手機", "剣豪", "敵", 2, 1);

            Assert.IsFalse(Parry(a, "ミサイル", t));
        }

        [TestMethod]
        public void Parry_WithoutSkill_Fails()
        {
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("剣持ち機", "防御者", "敵", 2, 1);

            Assert.IsFalse(Parry(a, "ミサイル", t));
        }

        [TestMethod]
        public void Parry_MeleeAgainstSkilledAttacker_ProbabilityReduced()
        {
            // 格闘攻撃では 2×防御側Lv − 攻撃側Lv。Lv8 対 Lv16 なら0で発動しない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "剣豪", "味方", 1, 1);
            var t = f.Place("剣持ち機", "剣士", "敵", 2, 1);

            Assert.IsFalse(Parry(a, "斬撃", t));
            Assert.IsFalse(Parry(a, "パンチ", t));
        }

        [TestMethod]
        public void Parry_AttackerUnderSureHit_Fails()
        {
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("剣持ち機", "剣豪", "敵", 2, 1);
            f.ApplySpecialPower(a, "必中テスト", "絶対命中");

            Assert.IsFalse(Parry(a, "ミサイル", t));
        }

        // ──────────────────────────────────────────────
        // 阻止
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Block_Probability100_BlocksAndConsumesEN()
        {
            // 阻止Lv1（ダメージ上限なし）・確率100・ＥＮ10
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("阻止機", "防御者", "敵", 2, 1);
            var en = t.EN;

            Assert.IsTrue(Parry(a, "ビーム", t, 5000));
            Assert.AreEqual(en - 10, t.EN);
            CollectionAssert.Contains(f.SysMessages(), "阻止機は[阻止結界]で[ビーム]を防いだ。");
        }

        [TestMethod]
        public void Block_NoAttributeWeapon_IsNotBlocked()
        {
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("阻止機", "防御者", "敵", 2, 1);
            var en = t.EN;

            Assert.IsFalse(Parry(a, "無属性砲", t));
            Assert.AreEqual(en, t.EN);
        }

        [TestMethod]
        public void Block_NotEnoughEN_Fails()
        {
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("阻止機", "防御者", "敵", 2, 1);
            t.EN = 9;

            Assert.IsFalse(Parry(a, "ビーム", t));
            Assert.AreEqual(9, t.EN);
        }

        [TestMethod]
        public void Block_SureHitAttacker_Fails()
        {
            // 絶対命中下では阻止できない（特殊防御発動があれば別）
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("阻止機", "防御者", "敵", 2, 1);
            f.ApplySpecialPower(a, "必中テスト", "絶対命中");

            Assert.IsFalse(Parry(a, "ビーム", t));
        }

        [TestMethod]
        public void Block_Level2_AttributeAndDamageLimit()
        {
            // 阻止Lv2 は対象属性（Ｂ）のみ、500×Lv=1000 以下のダメージだけ阻止する
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("弱阻止機", "防御者", "敵", 2, 1);

            Assert.IsFalse(Parry(a, "ミサイル", t, 500));
            Assert.IsFalse(Parry(a, "ビーム", t, 1001));
            Assert.IsTrue(Parry(a, "ビーム", t, 1000));
        }
    }
}
