using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCCore.Coverage.Tests.COMTests
{
    /// <summary>
    /// COM.SelectWeapon（攻撃に使う武器の選択）のテスト。
    /// VB 版 COM.bas の SelectWeapon に対応する。
    /// 地形修正なしの平地で、装甲 1000 のユニット同士を隣接させて判定する。
    /// </summary>
    [TestClass]
    public class COMSelectWeaponTests
    {
        private static COMTestField CreateField(int targetHp = 3000)
        {
            var f = new COMTestField();
            COMTestField.AddWeapon(f.DefineUnit("標的", hp: targetHp), "パンチ", 1200);
            return f;
        }

        [TestMethod]
        public void SelectWeapon_ChoosesWeaponWithHighestExpectedDamage()
        {
            // 破壊できない相手にはダメージの期待値が最も高い武器を選ぶ
            var f = CreateField();
            var ud = f.DefineUnit("攻撃機");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            COMTestField.AddWeapon(ud, "キック", 1800);
            COMTestField.AddWeapon(ud, "ビーム", 1500, 1, 4, wclass: "Ｂ");
            var u = f.Deploy("攻撃機", "敵A", "敵", 3, 3);
            var t = f.Deploy("標的", "味方A", "味方", 4, 3);

            var w = f.Src.COM.SelectWeapon(u, t, "", out var maxProb, out var maxDmg);

            Assert.AreEqual(2, w);
            // 破壊確率が 50% 以下なので max_prob は 0
            Assert.AreEqual(0, maxProb);
            // max_dmg は攻撃側ＨＰに対する期待ダメージの百分率
            Assert.IsTrue(maxDmg > 0 && maxDmg < 100, $"max_dmg={maxDmg}");
        }

        [TestMethod]
        public void SelectWeapon_SkipsWeaponsOutOfRange()
        {
            // 射程外の武器は選ばない
            var f = CreateField();
            var ud = f.DefineUnit("攻撃機");
            COMTestField.AddWeapon(ud, "ミサイル", 1800, 2, 4, wclass: "実");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            var u = f.Deploy("攻撃機", "敵A", "敵", 3, 3);
            var adjacent = f.Deploy("標的", "味方A", "味方", 4, 3);
            var far = f.Deploy("標的", "味方B", "味方", 3, 6);

            Assert.AreEqual(2, f.Src.COM.SelectWeapon(u, adjacent, "", out _, out _));
            Assert.AreEqual(1, f.Src.COM.SelectWeapon(u, far, "", out _, out _));
        }

        [TestMethod]
        public void SelectWeapon_NoWeaponInRange_ReturnsZero()
        {
            var f = CreateField();
            var ud = f.DefineUnit("攻撃機");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            var u = f.Deploy("攻撃機", "敵A", "敵", 1, 1);
            var t = f.Deploy("標的", "味方A", "味方", 8, 8);

            Assert.AreEqual(0, f.Src.COM.SelectWeapon(u, t, "", out var maxProb, out _));
            Assert.AreEqual(0, maxProb);
        }

        [TestMethod]
        public void SelectWeapon_AllyWithSureKill_PrefersLowerNumberedWeapon()
        {
            // 味方ユニットは確実に破壊できる武器が複数あれば番号の小さい（低コストの）武器を使う
            var f = CreateField(targetHp: 100);
            var ud = f.DefineUnit("攻撃機");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            COMTestField.AddWeapon(ud, "キック", 1800);
            var u = f.Deploy("攻撃機", "味方A", "味方", 3, 3);
            var t = f.Deploy("標的", "敵A", "敵", 4, 3);

            var w = f.Src.COM.SelectWeapon(u, t, "", out var maxProb, out _);

            Assert.AreEqual(1, w);
            Assert.AreEqual(100, maxProb);
        }

        [TestMethod]
        public void SelectWeapon_EnemyWithSureKill_PrefersHigherExpectedDamage()
        {
            // 敵ユニットは破壊確率が同じならコストを無視して期待値の高い武器を使う
            // (味方が相手の場合は 2 倍のダメージで破壊と見なす)
            var f = CreateField(targetHp: 100);
            var ud = f.DefineUnit("攻撃機");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            COMTestField.AddWeapon(ud, "キック", 1800);
            var u = f.Deploy("攻撃機", "敵A", "敵", 3, 3);
            var t = f.Deploy("標的", "味方A", "味方", 4, 3);

            var w = f.Src.COM.SelectWeapon(u, t, "", out var maxProb, out _);

            Assert.AreEqual(2, w);
            Assert.AreEqual(100, maxProb);
        }

        [TestMethod]
        public void SelectWeapon_DestroyProbabilityAgainstAllyRequiresDoubleHp()
        {
            // 味方を相手にした場合、ダメージがＨＰの 2 倍以上の武器のみ破壊可能と見なす
            // パンチ(約250) < 2×ＨＰ(600) <= キック(約980)
            var f = CreateField(targetHp: 300);
            var ud = f.DefineUnit("攻撃機");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            COMTestField.AddWeapon(ud, "キック", 1800, wclass: "突消");
            var u = f.Deploy("攻撃機", "敵A", "敵", 3, 3);
            var t = f.Deploy("標的", "味方A", "味方", 4, 3);

            // ＥＮ消耗攻撃でも破壊できるなら使う
            Assert.AreEqual(2, f.Src.COM.SelectWeapon(u, t, "", out var maxProb, out _));
            Assert.AreEqual(100, maxProb);
        }

        [TestMethod]
        public void SelectWeapon_EnemyUsesENExhaustingWeaponOnlyWhenItDestroys()
        {
            // 敵ユニット(ボス以外)は相手を倒せないときＥＮ消耗攻撃(消)を使わない
            var f = CreateField(targetHp: 3000);
            var ud = f.DefineUnit("攻撃機");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            COMTestField.AddWeapon(ud, "キック", 1800, wclass: "突消");
            var u = f.Deploy("攻撃機", "敵A", "敵", 3, 3);
            var t = f.Deploy("標的", "味方A", "味方", 4, 3);

            Assert.AreEqual(1, f.Src.COM.SelectWeapon(u, t, "", out _, out _));
        }

        [TestMethod]
        public void SelectWeapon_FeasibilityCheck_ReturnsFirstDamagingWeapon()
        {
            // 「可否判定」ではダメージを与えられる最初の武器を即座に返す
            var f = CreateField();
            var ud = f.DefineUnit("攻撃機");
            COMTestField.AddWeapon(ud, "ミサイル", 1800, 2, 4, wclass: "実");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            COMTestField.AddWeapon(ud, "キック", 1800);
            var u = f.Deploy("攻撃機", "敵A", "敵", 3, 3);
            var t = f.Deploy("標的", "味方A", "味方", 4, 3);

            Assert.AreEqual(2, f.Src.COM.SelectWeapon(u, t, "可否判定", out _, out _));
        }

        [TestMethod]
        public void SelectWeapon_Dancing_ReturnsMinusOne()
        {
            // 踊っているユニットは攻撃しない
            var f = CreateField();
            var ud = f.DefineUnit("攻撃機");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            var u = f.Deploy("攻撃機", "敵A", "敵", 3, 3);
            var t = f.Deploy("標的", "味方A", "味方", 4, 3);
            u.AddCondition("踊り", 1);

            Assert.AreEqual(-1, f.Src.COM.SelectWeapon(u, t, "", out _, out _));
        }

        [TestMethod]
        public void SelectWeapon_CharmedUnitAgainstMaster_ReturnsMinusOne()
        {
            // 魅了されたユニットは御主人さまを攻撃しない（他の相手は攻撃する）
            var f = CreateField();
            var ud = f.DefineUnit("攻撃機");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            var u = f.Deploy("攻撃機", "敵A", "敵", 3, 3);
            var master = f.Deploy("標的", "味方A", "味方", 4, 3);
            var other = f.Deploy("標的", "味方B", "味方", 3, 4);
            u.AddCondition("魅了", 1);
            u.Master = master;

            Assert.AreEqual(-1, f.Src.COM.SelectWeapon(u, master, "", out _, out _));
            Assert.AreEqual(1, f.Src.COM.SelectWeapon(u, other, "", out _, out _));
        }

        [TestMethod]
        public void SelectWeapon_Ally_SkipsLowAmmoWeaponsExceptInEvents()
        {
            // 味方ユニットは最大弾数 2～3 の武器（残弾僅少）を自動選択しない。
            // イベントでの選択(イベント)ではこの制限はない
            var f = CreateField();
            var ud = f.DefineUnit("攻撃機");
            COMTestField.AddWeapon(ud, "バズーカ", 1800, wclass: "実", bullet: 2);
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            var u = f.Deploy("攻撃機", "味方A", "味方", 3, 3);
            var t = f.Deploy("標的", "敵A", "敵", 4, 3);

            Assert.AreEqual(2, f.Src.COM.SelectWeapon(u, t, "", out _, out _));
            Assert.AreEqual(1, f.Src.COM.SelectWeapon(u, t, "イベント", out _, out _));
        }

        [TestMethod]
        public void SelectWeapon_Ally_SkipsCostlyENWeapons()
        {
            // 味方ユニットは現在ＥＮの半分以上を消費する武器を自動選択しない
            var f = CreateField();
            var ud = f.DefineUnit("攻撃機", en: 200);
            COMTestField.AddWeapon(ud, "大砲", 1800, en: 100);
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            var u = f.Deploy("攻撃機", "味方A", "味方", 3, 3);
            var t = f.Deploy("標的", "敵A", "敵", 4, 3);

            Assert.AreEqual(2, f.Src.COM.SelectWeapon(u, t, "", out _, out _));

            // 同じ武器でも敵ユニットなら使う
            var e = f.Deploy("攻撃機", "敵B", "敵", 6, 6);
            var t2 = f.Deploy("標的", "味方B", "味方", 6, 7);
            Assert.AreEqual(1, f.Src.COM.SelectWeapon(e, t2, "", out _, out _));
        }

        [TestMethod]
        public void SelectWeapon_Ally_NeverAutoSelectsSelfDestruct()
        {
            // 自爆攻撃は手動選択でのみ使う
            var f = CreateField();
            var ud = f.DefineUnit("攻撃機");
            COMTestField.AddWeapon(ud, "自爆", 3000, wclass: "突自");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            var u = f.Deploy("攻撃機", "味方A", "味方", 3, 3);
            var t = f.Deploy("標的", "敵A", "敵", 4, 3);

            Assert.AreEqual(2, f.Src.COM.SelectWeapon(u, t, "", out _, out _));
        }

        [TestMethod]
        public void SelectWeapon_Boss_UsesSelfDestructOnlyWhenHpBelowQuarter()
        {
            // ボスユニットが自爆攻撃を使うのはＨＰが最大値の 1/4 以下の非常時のみ
            var f = CreateField();
            var ud = f.DefineUnit("攻撃機");
            COMTestField.AddWeapon(ud, "自爆", 3000, wclass: "突自");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            var u = f.Deploy("攻撃機", "敵A", "敵", 3, 3);
            var t = f.Deploy("標的", "味方A", "味方", 4, 3);
            u.BossRank = 1;
            u.FullRecover();

            Assert.AreEqual(2, f.Src.COM.SelectWeapon(u, t, "", out _, out _));

            u.HP = u.MaxHP / 4;
            Assert.AreEqual(1, f.Src.COM.SelectWeapon(u, t, "", out _, out _));
        }

        [TestMethod]
        public void SelectWeapon_SelfDestructReservedForModeTarget()
        {
            // 思考モードで特定のパイロットを狙っている場合、自爆攻撃はそのターゲットにしか使わない
            var f = CreateField();
            var ud = f.DefineUnit("攻撃機");
            COMTestField.AddWeapon(ud, "自爆", 3000, wclass: "突自");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            var u = f.Deploy("攻撃機", "敵A", "敵", 3, 3);
            var marked = f.Deploy("標的", "味方A", "味方", 4, 3);
            var other = f.Deploy("標的", "味方B", "味方", 3, 4);
            u.Mode = "味方A";

            Assert.AreEqual(1, f.Src.COM.SelectWeapon(u, marked, "", out _, out _));
            Assert.AreEqual(2, f.Src.COM.SelectWeapon(u, other, "", out _, out _));
        }

        [TestMethod]
        public void SelectWeapon_CounterAttack_SkipsCombinationAttack()
        {
            // 合体技は自分から攻撃をかける場合にのみ使う（反撃では使わない）
            var f = CreateField();
            var ud = f.DefineUnit("攻撃機");
            COMTestField.AddWeapon(ud, "合体技", 3000, wclass: "突合");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            var u = f.Deploy("攻撃機", "敵A", "敵", 3, 3);
            var t = f.Deploy("標的", "味方A", "味方", 4, 3);

            Assert.AreEqual(2, f.Src.COM.SelectWeapon(u, t, "反撃", out _, out _));
        }

        [TestMethod]
        [Ignore("移植の不具合: max_dmg を VB の Long 代入(丸め)ではなく int キャスト(切り捨て)で求めている")]
        public void SelectWeapon_MaxDmgIsRoundedLikeVBLongAssignment()
        {
            // VB: max_dmg = 100 * (max_exp_dmg / .HP) は Long への代入で四捨五入(銀行丸め)される
            // キック: 期待値 986 / ＨＰ 3000 → 32.87 → 33
            var f = CreateField();
            var ud = f.DefineUnit("攻撃機");
            COMTestField.AddWeapon(ud, "キック", 1800);
            var u = f.Deploy("攻撃機", "敵A", "敵", 3, 3);
            var t = f.Deploy("標的", "味方A", "味方", 4, 3);
            var weapon = u.Weapon(1);
            var dmg = weapon.ExpDamage(t, false);
            var ct = weapon.CriticalProbability(t);
            // 期待値 = dmg + (0.5 * dmg * ct) \ 100 (命中率 100%)
            var expDmg = dmg + (int)System.Math.Round(0.5 * dmg * ct, System.MidpointRounding.ToEven) / 100;
            Assert.AreEqual(100, weapon.HitProbability(t, false));
            var expected = (int)System.Math.Round(100d * expDmg / u.HP, System.MidpointRounding.ToEven);

            f.Src.COM.SelectWeapon(u, t, "", out _, out var maxDmg);

            Assert.AreEqual(expected, maxDmg);
        }
    }
}
