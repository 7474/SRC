using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;
using System.Linq;
using System.Reflection;

namespace SRCCore.Coverage.Tests.UnitAttackCheck
{
    /// <summary>
    /// 広域バリア・プロテクション・バリア中和攻撃・緊急テレポートのテスト
    /// （VB: Unit.cls CheckDefenseFeature / CauseEffect / CheckDodgeFeature）。
    /// </summary>
    [TestClass]
    public class UnitAttackCheckAreaDefenseTests
    {
        private const string Pilots = @"
攻撃者
攻撃者, こうげきしゃ, 男性, 汎用, AAAA, 100
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

僚機
僚機, りょうき, 男性, 汎用, AAAA, 100
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
ビーム, 1500, 1, 3, +0, -, -, -, AAAA, +0, Ｂ
中和砲, 1500, 1, 3, +0, -, -, -, AAAA, +0, Ｂ中

普通機
普通機, ふつうき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力なし
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

障壁機
障壁機, しょうへきき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
広域バリアLv1=広域障壁 2 全 30
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

防護機
防護機, ぼうごき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
プロテクションLv5=防護膜 全 10
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

完全防護機
完全防護機, かんぜんぼうごき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
プロテクションLv10=防護膜 全 10
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

バリア機
バリア機, ばりあき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
バリアLv1=光の壁 全 10 0
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

転移機
転移機, てんいき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
緊急テレポートLv10=緊急転移 3 20
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突
";

        private static UnitAttackCheckFixture CreateFixture()
        {
            var f = new UnitAttackCheckFixture();
            f.FillPlainTerrain();
            f.LoadPilots(Pilots);
            f.LoadUnits(Units);
            return f;
        }

        private static UnitWeapon WeaponOf(Unit u, string name)
        {
            return u.Weapons.First(w => w.Name == name);
        }

        private static bool Defense(Unit a, UnitWeapon w, Unit t, ref int dmg, ref string msg)
        {
            var mi = typeof(Unit).GetMethod("CheckDefenseFeature", BindingFlags.NonPublic | BindingFlags.Instance);
            var args = new object[] { w, t, t.x, t.y, "", "", dmg, msg, true, false };
            var ret = (bool)mi.Invoke(a, args);
            dmg = (int)args[6];
            msg = (string)args[7];
            return ret;
        }

        // ──────────────────────────────────────────────
        // 広域バリア
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AreaBarrier_AllyNegatesDamageAndPaysENOncePerTurn()
        {
            // 範囲内の味方の広域バリアが1000以下のダメージを無効化する。ＥＮ消費はターン中1回だけ
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("普通機", "防御者", "敵", 4, 1);
            var u = f.Place("障壁機", "僚機", "敵", 5, 1);
            var en = u.EN;
            var dmg = 900;
            var msg = "";

            Assert.IsTrue(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg));
            Assert.AreEqual(en - 30, u.EN);
            Assert.IsTrue(u.IsConditionSatisfied("バリア発動"));
            CollectionAssert.Contains(f.SysMessages(), "障壁機は[広域障壁]を発動させた。");

            dmg = 900;
            Assert.IsTrue(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg));
            Assert.AreEqual(en - 30, u.EN);
        }

        [TestMethod]
        public void AreaBarrier_DamageOverLimit_IsPenetrated()
        {
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("普通機", "防御者", "敵", 4, 1);
            f.Place("障壁機", "僚機", "敵", 5, 1);
            var dmg = 1001;
            var msg = "";

            Assert.IsFalse(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg));
            Assert.AreEqual("ビームが[広域障壁]を貫いた。;", msg);
            Assert.AreEqual(1001, dmg);
        }

        [TestMethod]
        public void AreaBarrier_AttackerInsideRange_NotApplied()
        {
            // 攻撃側がバリアの効果範囲内にいれば防げない
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 3, 1);
            var t = f.Place("普通機", "防御者", "敵", 4, 1);
            var u = f.Place("障壁機", "僚機", "敵", 5, 1);
            var en = u.EN;
            var dmg = 900;
            var msg = "";

            Assert.IsFalse(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg));
            Assert.AreEqual(en, u.EN);
        }

        // ──────────────────────────────────────────────
        // プロテクション
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Protection_ReducesDamageByTenPercentPerLevel()
        {
            // プロテクションLv5: ダメージ × (100 − 10×5) / 100
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("防護機", "防御者", "敵", 2, 1);
            var en = t.EN;
            var dmg = 1000;
            var msg = "";

            Assert.IsFalse(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg));
            Assert.AreEqual(500, dmg);
            Assert.AreEqual(en - 10, t.EN);
            Assert.AreEqual("[防護膜]がダメージを減少させた。;", msg);
        }

        [TestMethod]
        public void Protection_Level10_NegatesDamage()
        {
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("完全防護機", "防御者", "敵", 2, 1);
            var hp = t.HP;
            var dmg = 1000;
            var msg = "";

            Assert.IsTrue(Defense(a, WeaponOf(a, "ビーム"), t, ref dmg, ref msg));
            Assert.AreEqual(0, dmg);
            Assert.AreEqual(hp, t.HP);
        }

        // ──────────────────────────────────────────────
        // バリア中和攻撃（CauseEffect）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BarrierNeutralize_AddsBarrierDisabledCondition()
        {
            // 「中」属性: バリア系能力を持つ相手に「バリア無効化」を1ターン付ける
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("バリア機", "防御者", "敵", 2, 1);
            f.ApplySpecialPower(a, "発動テスト", "特殊効果発動");
            var msg = "";
            var ct = "";

            Assert.IsTrue(a.CauseEffect(WeaponOf(a, "中和砲"), t, ref msg, ref ct, "", false));
            Assert.AreEqual(1, t.ConditionLifetime("バリア無効化"));
            Assert.AreEqual("[バリア機]の光の壁を無効化した。;", msg);
            Assert.AreEqual(" バリア中和", ct);
        }

        [TestMethod]
        public void BarrierNeutralize_TargetWithoutBarrier_NoEffect()
        {
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("普通機", "防御者", "敵", 2, 1);
            f.ApplySpecialPower(a, "発動テスト", "特殊効果発動");
            var msg = "";
            var ct = "";

            Assert.IsFalse(a.CauseEffect(WeaponOf(a, "中和砲"), t, ref msg, ref ct, "", false));
            Assert.IsFalse(t.IsConditionSatisfied("バリア無効化"));
        }

        // ──────────────────────────────────────────────
        // 緊急テレポート（CheckDodgeFeature）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EmergencyTeleport_Level10_EscapesAndConsumesEN()
        {
            // 緊急テレポートLv10 は必ず発動し、ＥＮ20 を消費して別の地点へ移動する
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("転移機", "防御者", "敵", 5, 5);
            var en = t.EN;

            Assert.IsTrue(a.CheckDodgeFeature(WeaponOf(a, "ビーム"), t, t.x, t.y, "", "", 1500, true));
            Assert.AreEqual(en - 20, t.EN);
            Assert.IsFalse(t.x == 5 && t.y == 5);
            Assert.AreSame(t, f.Src.Map.MapDataForUnit[t.x, t.y]);
            CollectionAssert.Contains(f.SysMessages(), "転移機は[緊急転移]を使って攻撃をかわした。");
        }

        [TestMethod]
        public void EmergencyTeleport_NotEnoughEN_Stays()
        {
            var f = CreateFixture();
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place("転移機", "防御者", "敵", 5, 5);
            t.EN = 19;

            Assert.IsFalse(a.CheckDodgeFeature(WeaponOf(a, "ビーム"), t, t.x, t.y, "", "", 1500, true));
            Assert.AreEqual(5, t.x);
            Assert.AreEqual(5, t.y);
        }
    }
}
