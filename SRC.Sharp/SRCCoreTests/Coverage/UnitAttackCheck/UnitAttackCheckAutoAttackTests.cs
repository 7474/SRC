using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitAttackCheck
{
    /// <summary>
    /// Unit.CheckAutoAttack / Unit.CheckAdditionalAttack（VB: Unit.cls CheckAutoAttack / CheckAdditionalAttack）のテスト。
    /// 発動確率は100か0になる設定だけを使う。発動時は実際に Attack が走るので、
    /// 結果のうち乱数に左右されない部分（発動メッセージとＥＮ消費）だけを確かめる。
    /// </summary>
    [TestClass]
    public class UnitAttackCheckAutoAttackTests
    {
        private const string Pilots = @"
操縦者
操縦者, そうじゅうしゃ, 男性, 汎用, AAAA, 100
特殊能力なし
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

防御者
防御者, ぼうぎょしゃ, 男性, 汎用, AAAA, 100
特殊能力なし
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

予備
予備, よび, 男性, 汎用, AAAA, 100
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
パンチ, 100, 1, 1, +0, -, -, -, AAAA, +0, 突
ビーム, 100, 1, 3, +0, -, -, -, AAAA, +0, Ｂ
間接砲, 100, 1, 3, +0, -, -, -, AAAA, +0, 間
マップ砲, 100, 1, 3, +0, -, -, -, AAAA, +0, Ｍ直

反撃機
反撃機, はんげきき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
自動反撃Lv1=反撃 カウンター 全 100 10
3000, 200, 1000, 100
AAAA, -.bmp
カウンター, 100, 1, 1, +0, -, -, -, AAAA, +0, 突

完全自動反撃機
完全自動反撃機, かんぜんじどうはんげきき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
自動反撃Lv1=反撃 カウンター 全 100 0 0 完全自動
3000, 200, 1000, 100
AAAA, -.bmp
カウンター, 100, 1, 1, +0, -, -, -, AAAA, +0, 突

追撃機
追撃機, ついげきき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
追加攻撃=追撃 追撃砲 全 100 10 0 連鎖不可
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 100, 1, 1, +0, -, -, -, AAAA, +0, 突
ビーム, 100, 1, 3, +0, -, -, -, AAAA, +0, Ｂ
追撃砲, 100, 1, 3, +0, -, -, -, AAAA, +0, 実

二段追撃機
二段追撃機, にだんついげきき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
追加攻撃=追撃 追撃砲 全 100 0 0 連鎖不可
追加攻撃=再追撃 追撃砲 全 100 0 0 連鎖不可
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 100, 1, 1, +0, -, -, -, AAAA, +0, 突
追撃砲, 100, 1, 3, +0, -, -, -, AAAA, +0, 実

命中時追撃機
命中時追撃機, めいちゅうじついげきき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
追加攻撃=追撃 追撃砲 @パンチ 100 0 0 連鎖不可 命中時限定
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 100, 1, 1, +0, -, -, -, AAAA, +0, 突
ビーム, 100, 1, 3, +0, -, -, -, AAAA, +0, Ｂ
追撃砲, 100, 1, 3, +0, -, -, -, AAAA, +0, 実

標的機
標的機, ひょうてきき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力なし
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 100, 1, 1, +0, -, -, -, AAAA, +0, 突
";

        private static UnitAttackCheckFixture CreateFixture()
        {
            var f = new UnitAttackCheckFixture();
            f.LoadPilots(Pilots);
            f.LoadUnits(Units);
            // 発動時に Attack が地形を参照する
            f.FillPlainTerrain();
            return f;
        }

        private static UnitWeapon WeaponOf(Unit u, string name)
        {
            return u.Weapons.First(x => x.Name == name);
        }

        private static void AutoAttack(Unit a, string wname, ref Unit t, string attack_mode = "", string def_mode = "", int dmg = 100)
        {
            a.CheckAutoAttack(WeaponOf(a, wname), ref t, attack_mode, def_mode, dmg, true);
        }

        private static void AdditionalAttack(Unit a, string wname, ref Unit t, string attack_mode = "", int dmg = 100)
        {
            a.CheckAdditionalAttack(WeaponOf(a, wname), ref t, true, attack_mode, "", dmg);
        }

        // ──────────────────────────────────────────────
        // 自動反撃
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AutoAttack_Melee_CountersAndConsumesEN()
        {
            // 確率100・ＥＮ10 の自動反撃。格闘攻撃（突）に対しては射程を問わず反撃する
            var f = CreateFixture();
            var a = f.Place("攻撃機", "操縦者", "味方", 1, 1);
            var t = f.Place("反撃機", "防御者", "敵", 2, 1);
            var en = t.EN;

            AutoAttack(a, "パンチ", ref t);

            CollectionAssert.Contains(f.SysMessages(), "反撃機はカウンターで反撃した。");
            Assert.AreEqual(en - 10, t.EN);
        }

        [TestMethod]
        public void AutoAttack_RangedAttackOutOfCounterRange_NoCounter()
        {
            // 格闘以外の攻撃に対しては反撃武器の射程内にいなければ反撃しない（距離2、射程1）
            var f = CreateFixture();
            var a = f.Place("攻撃機", "操縦者", "味方", 1, 1);
            var t = f.Place("反撃機", "防御者", "敵", 3, 1);
            var en = t.EN;

            AutoAttack(a, "ビーム", ref t);

            Assert.AreEqual(0, f.SysMessages().Length);
            Assert.AreEqual(en, t.EN);
        }

        [TestMethod]
        public void AutoAttack_RangedAttackWithinCounterRange_Counters()
        {
            var f = CreateFixture();
            var a = f.Place("攻撃機", "操縦者", "味方", 1, 1);
            var t = f.Place("反撃機", "防御者", "敵", 2, 1);

            AutoAttack(a, "ビーム", ref t);

            CollectionAssert.Contains(f.SysMessages(), "反撃機はカウンターで反撃した。");
        }

        [TestMethod]
        [DataRow("反射")]
        [DataRow("自動反撃")]
        [DataRow("当て身技")]
        public void AutoAttack_AgainstCounterAttacks_NoCounter(string attack_mode)
        {
            // 反撃系の攻撃に対しては自動反撃しない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "操縦者", "味方", 1, 1);
            var t = f.Place("反撃機", "防御者", "敵", 2, 1);
            var en = t.EN;

            AutoAttack(a, "パンチ", ref t, attack_mode);

            Assert.AreEqual(0, f.SysMessages().Length);
            Assert.AreEqual(en, t.EN);
        }

        [TestMethod]
        [DataRow("間接砲")]
        [DataRow("マップ砲")]
        public void AutoAttack_IndirectOrMapWeapon_NoCounter(string wname)
        {
            // 間接攻撃（間）とマップ攻撃（Ｍ）には自動反撃できない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "操縦者", "味方", 1, 1);
            var t = f.Place("反撃機", "防御者", "敵", 2, 1);

            AutoAttack(a, wname, ref t);

            Assert.AreEqual(0, f.SysMessages().Length);
        }

        [TestMethod]
        public void AutoAttack_NotEnoughEN_NoCounter()
        {
            var f = CreateFixture();
            var a = f.Place("攻撃機", "操縦者", "味方", 1, 1);
            var t = f.Place("反撃機", "防御者", "敵", 2, 1);
            t.EN = 9;

            AutoAttack(a, "パンチ", ref t);

            Assert.AreEqual(0, f.SysMessages().Length);
            Assert.AreEqual(9, t.EN);
        }

        [TestMethod]
        public void AutoAttack_CannotAct_NoCounterUnlessFullyAutomatic()
        {
            // 行動できない（行動不能状態）ときは発動しないが、「完全自動」指定があれば発動する
            var f = CreateFixture();
            var a = f.Place("攻撃機", "操縦者", "味方", 1, 1);
            var t1 = f.Place("反撃機", "防御者", "敵", 2, 1);
            t1.AddCondition("行動不能", 1);

            AutoAttack(a, "パンチ", ref t1);
            Assert.AreEqual(0, f.SysMessages().Length);

            f.Src.Map.MapDataForUnit[2, 1] = null;
            t1.Status = "待機";
            var t2 = f.Place("完全自動反撃機", "予備", "敵", 1, 2);
            t2.AddCondition("行動不能", 1);

            AutoAttack(a, "パンチ", ref t2);
            CollectionAssert.Contains(f.SysMessages(), "完全自動反撃機はカウンターで反撃した。");
        }

        // ──────────────────────────────────────────────
        // 追加攻撃
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AdditionalAttack_Triggers_AndConsumesEN()
        {
            // 確率100・ＥＮ10 の追加攻撃。攻撃後に指定武器でさらに攻撃する
            var f = CreateFixture();
            var a = f.Place("追撃機", "操縦者", "味方", 1, 1);
            var t = f.Place("標的機", "防御者", "敵", 2, 1);
            var en = a.EN;

            AdditionalAttack(a, "パンチ", ref t);

            CollectionAssert.Contains(f.SysMessages(), "追撃機はさらに[追撃砲]で攻撃を加えた。");
            Assert.AreEqual(en - 10, a.EN);
        }

        [TestMethod]
        public void AdditionalAttack_NoChain_NotAfterAdditionalAttack()
        {
            // 連鎖不可: 追加攻撃による攻撃からは追加攻撃しない
            var f = CreateFixture();
            var a = f.Place("追撃機", "操縦者", "味方", 1, 1);
            var t = f.Place("標的機", "防御者", "敵", 2, 1);
            var en = a.EN;

            AdditionalAttack(a, "パンチ", ref t, attack_mode: "追加攻撃");

            Assert.AreEqual(0, f.SysMessages().Length);
            Assert.AreEqual(en, a.EN);
        }

        [TestMethod]
        public void AdditionalAttack_NoChain_SecondFeatureSkippedAfterFirst()
        {
            // 連鎖不可: 同じ攻撃で既に別の追加攻撃が発動していれば発動しない
            var f = CreateFixture();
            var a = f.Place("二段追撃機", "操縦者", "味方", 1, 1);
            var t = f.Place("標的機", "防御者", "敵", 2, 1);

            AdditionalAttack(a, "パンチ", ref t);

            Assert.AreEqual(1, f.SysMessages().Count(m => m == "二段追撃機はさらに[追撃砲]で攻撃を加えた。"));
        }

        [TestMethod]
        public void AdditionalAttack_NotEnoughEN_NotTriggered()
        {
            var f = CreateFixture();
            var a = f.Place("追撃機", "操縦者", "味方", 1, 1);
            var t = f.Place("標的機", "防御者", "敵", 2, 1);
            a.EN = 9;

            AdditionalAttack(a, "パンチ", ref t);

            Assert.AreEqual(0, f.SysMessages().Length);
            Assert.AreEqual(9, a.EN);
        }

        [TestMethod]
        public void AdditionalAttack_TargetOutOfRange_NotTriggered()
        {
            // 追加攻撃の武器（射程3）の射程外にいれば発動しない
            var f = CreateFixture();
            var a = f.Place("追撃機", "操縦者", "味方", 1, 1);
            var t = f.Place("標的機", "防御者", "敵", 6, 1);

            AdditionalAttack(a, "ビーム", ref t);

            Assert.AreEqual(0, f.SysMessages().Length);
        }

        [TestMethod]
        public void AdditionalAttack_HitOnly_RequiresDamage()
        {
            // 命中時限定: ダメージが0以下（外れ）なら発動しない
            var f = CreateFixture();
            var a = f.Place("命中時追撃機", "操縦者", "味方", 1, 1);
            var t = f.Place("標的機", "防御者", "敵", 2, 1);

            AdditionalAttack(a, "パンチ", ref t, dmg: 0);
            Assert.AreEqual(0, f.SysMessages().Length);

            AdditionalAttack(a, "パンチ", ref t, dmg: 100);
            CollectionAssert.Contains(f.SysMessages(), "命中時追撃機はさらに[追撃砲]で攻撃を加えた。");
        }

        [TestMethod]
        public void AdditionalAttack_WeaponNameFilter_OtherWeaponNotTriggered()
        {
            // @武器名 指定: 指定した武器（パンチ）以外での攻撃では発動しない
            var f = CreateFixture();
            var a = f.Place("命中時追撃機", "操縦者", "味方", 1, 1);
            var t = f.Place("標的機", "防御者", "敵", 2, 1);

            AdditionalAttack(a, "ビーム", ref t);

            Assert.AreEqual(0, f.SysMessages().Length);
        }
    }
}
