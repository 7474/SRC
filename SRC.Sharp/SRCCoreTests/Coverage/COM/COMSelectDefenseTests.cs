using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCCore.Coverage.Tests.COMTests
{
    /// <summary>
    /// COM.SelectDefense（攻撃を受けた側の防御行動の選択）のテスト。
    /// VB 版 COM.bas の SelectDefense に対応する。
    /// 戻り値 "" は反撃（または防御行動なし）、"防御"、"回避"。
    /// </summary>
    [TestClass]
    public class COMSelectDefenseTests
    {
        // 攻撃側: 敵、パンチ(約250ダメージ)・キック(約980ダメージ)・マップ兵器
        private static COMTestField CreateField(int defenderHp = 3000, int attackerHp = 3000, int attackerHit = 100)
        {
            var f = new COMTestField();
            var ud = f.DefineUnit("攻撃機", hp: attackerHp);
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            COMTestField.AddWeapon(ud, "キック", 1800);
            COMTestField.AddWeapon(ud, "マップ兵器", 1800, 1, 3, wclass: "Ｍ全");
            f.DefinePilot("敵P", attackerHit);
            var td = f.DefineUnit("防御機", hp: defenderHp);
            COMTestField.AddWeapon(td, "パンチ", 1200);
            COMTestField.AddWeapon(td, "必殺", 4000, wclass: "突先");
            // 先制攻撃でない反撃用の武器 (ダメージ: 約980 / 約740 / 約500 / 約380)
            COMTestField.AddWeapon(td, "強打", 1800);
            COMTestField.AddWeapon(td, "中打", 1600);
            COMTestField.AddWeapon(td, "軽打", 1400);
            COMTestField.AddWeapon(td, "小打", 1300);
            return f;
        }

        [TestMethod]
        public void SelectDefense_MapAttack_NoDefenseAction()
        {
            // マップ攻撃に対しては防御行動が取れない
            var f = CreateField();
            var u = f.Deploy("攻撃機", "敵P", "敵", 3, 3);
            var t = f.Deploy("防御機", "味方P", "味方", 4, 3);

            Assert.AreEqual("", f.Src.COM.SelectDefense(u, 3, t, 0));
        }

        [TestMethod]
        public void SelectDefense_Dancing_IsTreatedAsEvasion()
        {
            // 踊っている場合は回避扱い
            var f = CreateField();
            var u = f.Deploy("攻撃機", "敵P", "敵", 3, 3);
            var t = f.Deploy("防御機", "味方P", "味方", 4, 3);
            t.AddCondition("踊り", 1);

            Assert.AreEqual("回避", f.Src.COM.SelectDefense(u, 2, t, 0));
        }

        [TestMethod]
        public void SelectDefense_Berserk_NoDefenseAction()
        {
            // 狂戦士状態の際は防御行動を取らない
            var f = CreateField();
            var u = f.Deploy("攻撃機", "敵P", "敵", 3, 3);
            var t = f.Deploy("防御機", "味方P", "味方", 4, 3);
            t.AddCondition("狂戦士", 1);

            Assert.AreEqual("", f.Src.COM.SelectDefense(u, 2, t, 0));
        }

        [TestMethod]
        public void SelectDefense_EnemyDefender_RequiresOptionAndSkilledNamedPilot()
        {
            // 敵ユニットは「敵ユニット防御使用」オプションがあり、
            // 技量 160 以上のザコでないパイロットの場合のみ防御行動を取る
            var f = CreateField();
            f.DefinePilot("達人", 100, technique: 200);
            f.DefinePilot("兵士(ザコ)", 100, technique: 200);
            var u = f.Deploy("攻撃機", "味方P", "味方", 3, 3);
            var master = f.Deploy("防御機", "達人", "敵", 4, 3);
            var grunt = f.Deploy("防御機", "兵士(ザコ)", "敵", 3, 4);

            // オプションなし
            Assert.AreEqual("", f.Src.COM.SelectDefense(u, 2, master, 0));

            f.Src.Expression.DefineGlobalVariable("Option(敵ユニット防御使用)");
            // 反撃手段がないので防御行動を取る
            Assert.AreEqual("防御", f.Src.COM.SelectDefense(u, 2, master, 0));
            // ザコは防御行動を取らない
            Assert.AreEqual("", f.Src.COM.SelectDefense(u, 2, grunt, 0));
        }

        [TestMethod]
        public void SelectDefense_NoCounterWeapon_DefendsWhenItAvoidsDestruction()
        {
            // 反撃できない場合は防御側が劣勢と見なし、防御すれば耐えられるなら防御
            var f = CreateField();
            var u = f.Deploy("攻撃機", "敵P", "敵", 3, 3);
            var t = f.Deploy("防御機", "味方P", "味方", 4, 3);

            Assert.AreEqual("防御", f.Src.COM.SelectDefense(u, 2, t, 0));
        }

        [TestMethod]
        public void SelectDefense_StrongerCounter_ChoosesCounterAttack()
        {
            // 反撃の効果 (ダメージ×命中率) の方が高い場合は防御行動を取らず反撃する
            // 先制攻撃で倒せる場合の判定に掛からないよう、先制攻撃でなく相手を倒せない反撃を使う
            var f = CreateField();
            var u = f.Deploy("攻撃機", "敵P", "敵", 3, 3);
            var t = f.Deploy("防御機", "味方P", "味方", 4, 3);
            var dmg = u.Weapon(1).ExpDamage(t, true);
            var prob = System.Math.Min(u.Weapon(1).HitProbability(t, true), 100);
            var tdmg = t.Weapon(3).ExpDamage(u, true);
            var tprob = System.Math.Min(t.Weapon(3).HitProbability(u, true), 100);
            Assert.IsTrue(tdmg < u.HP, $"前提: 反撃ダメージ {tdmg} で攻撃側は倒れない");
            Assert.IsTrue(dmg < t.HP, $"前提: 攻撃ダメージ {dmg} で防御側は倒れない");
            Assert.IsTrue(dmg * prob < tdmg * tprob, "前提: 反撃の効果の方が高い");

            // 攻撃: パンチ(約250)、反撃: 強打(約980)
            Assert.AreEqual("", f.Src.COM.SelectDefense(u, 1, t, 3));
        }

        [TestMethod]
        public void SelectDefense_EnemyDefender_DefendsOnlyWhenAttackIsTwiceAsEffective()
        {
            // 敵ユニットは相手の攻撃の効果が反撃の 2 倍を超える場合のみ劣勢と見なす
            // (味方ユニットは単に上回れば劣勢)。VB: dmg * prob \ 2 > tdmg * tprob
            var f = CreateField();
            f.DefinePilot("達人", 100, technique: 200);
            f.Src.Expression.DefineGlobalVariable("Option(敵ユニット防御使用)");
            var allyAttacker = f.Deploy("攻撃機", "味方Q", "味方", 3, 3);
            var enemy = f.Deploy("防御機", "達人", "敵", 4, 3);
            var enemyAttacker = f.Deploy("攻撃機", "敵P", "敵", 6, 6);
            var ally = f.Deploy("防御機", "味方P", "味方", 6, 7);

            // 攻撃: キック(約980)、反撃: 中打(約740) → 1 倍は超えるが 2 倍は超えない
            var dmg = allyAttacker.Weapon(2).ExpDamage(enemy, true);
            var tdmg = enemy.Weapon(4).ExpDamage(allyAttacker, true);
            Assert.IsTrue(dmg > tdmg && dmg / 2 < tdmg, $"前提: dmg={dmg}, tdmg={tdmg}");
            Assert.AreEqual(dmg, enemyAttacker.Weapon(2).ExpDamage(ally, true), "前提: 味方側も同じダメージ");
            Assert.AreEqual(100, allyAttacker.Weapon(2).HitProbability(enemy, true));
            Assert.AreEqual(100, enemy.Weapon(4).HitProbability(allyAttacker, true));

            Assert.AreEqual("", f.Src.COM.SelectDefense(allyAttacker, 2, enemy, 4));
            Assert.AreEqual("防御", f.Src.COM.SelectDefense(enemyAttacker, 2, ally, 4));

            // 反撃: 小打(約380) なら 2 倍を超えるので敵ユニットも防御する
            var tdmg2 = enemy.Weapon(6).ExpDamage(allyAttacker, true);
            Assert.IsTrue(dmg / 2 > tdmg2, $"前提: dmg={dmg}, tdmg={tdmg2}");
            Assert.AreEqual("防御", f.Src.COM.SelectDefense(allyAttacker, 2, enemy, 6));
        }

        [TestMethod]
        public void SelectDefense_AttackerUnderDamageBoost_AllyDefendsAtHalfThreshold()
        {
            // 相手が気合の一撃 (ダメージ増加) を使っている場合、味方ユニットは
            // 相手の攻撃の効果が反撃の半分を超えるだけで防御してやり過ごす
            var f = CreateField();
            f.Src.SPDList.Add("気合").SetEffect("ダメージ増加");
            var u = f.Deploy("攻撃機", "敵P", "敵", 3, 3);
            var t = f.Deploy("防御機", "味方P", "味方", 4, 3);

            // 攻撃: パンチ(約250)、反撃: 軽打(約500)
            Assert.AreEqual("", f.Src.COM.SelectDefense(u, 1, t, 5));

            u.MakeSpecialPowerInEffect("気合");
            var dmg = u.Weapon(1).ExpDamage(t, true);
            var tdmg = t.Weapon(5).ExpDamage(u, true);
            Assert.IsTrue(dmg < tdmg && 2 * dmg > tdmg, $"前提: dmg={dmg}, tdmg={tdmg}");
            Assert.AreEqual("防御", f.Src.COM.SelectDefense(u, 1, t, 5));
        }

        [TestMethod]
        public void SelectDefense_CannotAct_DefendsOnlyWhenChargingOrExhausted()
        {
            // 行動不能なユニットは防御行動が取れないが、チャージ中・消耗中は常に防御
            var f = CreateField();
            var u = f.Deploy("攻撃機", "敵P", "敵", 3, 3);
            var paralyzed = f.Deploy("防御機", "味方P", "味方", 4, 3);
            var charging = f.Deploy("防御機", "味方Q", "味方", 3, 4);
            var exhausted = f.Deploy("防御機", "味方R", "味方", 2, 3);
            paralyzed.AddCondition("麻痺", 1);
            charging.AddCondition("チャージ", 1);
            exhausted.AddCondition("消耗", 1);

            // いずれも通常なら反撃手段がないので防御する状況
            // (SelectDefense_NoCounterWeapon_DefendsWhenItAvoidsDestruction)
            Assert.AreEqual("", f.Src.COM.SelectDefense(u, 2, paralyzed, 0));
            Assert.AreEqual("防御", f.Src.COM.SelectDefense(u, 2, charging, 0));
            Assert.AreEqual("防御", f.Src.COM.SelectDefense(u, 2, exhausted, 0));
        }

        [TestMethod]
        public void SelectDefense_Defenseless_NoDefenseAction()
        {
            // 無防備状態 (スペシャルパワー) のユニットは防御行動が取れない
            var f = CreateField();
            f.Src.SPDList.Add("脱力").SetEffect("無防備");
            var u = f.Deploy("攻撃機", "敵P", "敵", 3, 3);
            var t = f.Deploy("防御機", "味方P", "味方", 4, 3);
            t.MakeSpecialPowerInEffect("脱力");

            Assert.AreEqual("", f.Src.COM.SelectDefense(u, 2, t, 0));
        }

        [TestMethod]
        public void SelectDefense_AnnihilationAttack_EvadesInsteadOfDefending()
        {
            // 抹殺攻撃 (殺) は防御しても防げないので、防御すれば耐えられる場合でも回避を選ぶ
            var f = CreateField(defenderHp: 900);
            COMTestField.AddWeapon(f.Src.UDList.Item("攻撃機"), "抹殺", 1800, wclass: "突殺");
            var u = f.Deploy("攻撃機", "敵P", "敵", 3, 3);
            var t = f.Deploy("防御機", "味方P", "味方", 4, 3);
            var dmg = u.Weapon(4).ExpDamage(t, true);
            Assert.IsTrue(dmg > t.HP && dmg / 2 < t.HP, $"前提: dmg={dmg}");

            // 同じ威力の通常攻撃 (キック) なら防御
            Assert.AreEqual("防御", f.Src.COM.SelectDefense(u, 2, t, 0));
            Assert.AreEqual("回避", f.Src.COM.SelectDefense(u, 4, t, 0));
        }

        [TestMethod]
        public void SelectDefense_WeakerCounter_ChoosesDefense()
        {
            // 相手の攻撃によるダメージの方が反撃より大きい場合は防御
            var f = CreateField();
            var u = f.Deploy("攻撃機", "敵P", "敵", 3, 3);
            var t = f.Deploy("防御機", "味方P", "味方", 4, 3);

            // 攻撃: キック(約980)、反撃: パンチ(約250)
            Assert.AreEqual("防御", f.Src.COM.SelectDefense(u, 2, t, 1));
        }

        [TestMethod]
        public void SelectDefense_FirstStrikeThatKills_AlwaysCounters()
        {
            // 先制攻撃で確実に倒せる場合は劣勢でも迷わず反撃
            // 防御側ＨＰ 900 はキック(約980)で破壊されるが、攻撃側ＨＰ 500 は先制の必殺で倒せる
            var f = CreateField(defenderHp: 900, attackerHp: 500);
            var u = f.Deploy("攻撃機", "敵P", "敵", 3, 3);
            var t = f.Deploy("防御機", "味方P", "味方", 4, 3);

            Assert.AreEqual("", f.Src.COM.SelectDefense(u, 2, t, 2));
        }

        [TestMethod]
        public void SelectDefense_LethalHitSurvivableByDefending_Defends()
        {
            // 命中すれば一撃死で、防御すれば耐えられる攻撃は必ず防御
            // 防御側ＨＰ 900 < キック(約980)、半減すれば耐えられる
            var f = CreateField(defenderHp: 900);
            var u = f.Deploy("攻撃機", "敵P", "敵", 3, 3);
            var t = f.Deploy("防御機", "味方P", "味方", 4, 3);

            Assert.AreEqual("防御", f.Src.COM.SelectDefense(u, 2, t, 1));
        }

        [TestMethod]
        public void SelectDefense_CannotDefend_Evades()
        {
            // 防御不可の場合は回避を選ぶ
            var f = CreateField(defenderHp: 900);
            f.Src.UDList.Item("防御機").AddFeature("防御不可");
            var u = f.Deploy("攻撃機", "敵P", "敵", 3, 3);
            var t = f.Deploy("防御機", "味方P", "味方", 4, 3);

            Assert.AreEqual("回避", f.Src.COM.SelectDefense(u, 2, t, 1));
        }

        [TestMethod]
        public void SelectDefense_CannotDefendNorEvade_ReturnsEmpty()
        {
            // 防御不可かつ回避不可なら防御行動は取れない
            var f = CreateField(defenderHp: 900);
            var td = f.Src.UDList.Item("防御機");
            td.AddFeature("防御不可");
            td.AddFeature("回避不可");
            var u = f.Deploy("攻撃機", "敵P", "敵", 3, 3);
            var t = f.Deploy("防御機", "味方P", "味方", 4, 3);

            Assert.AreEqual("", f.Src.COM.SelectDefense(u, 2, t, 1));
        }

        [TestMethod]
        public void SelectDefense_LowHitProbability_Evades()
        {
            // 相手の命中率が 50% 未満なら回避
            var f = CreateField(attackerHit: 1);
            f.DefinePilot("味方P", 250);
            var u = f.Deploy("攻撃機", "敵P", "敵", 3, 3);
            var t = f.Deploy("防御機", "味方P", "味方", 4, 3);
            var prob = u.Weapon(2).HitProbability(t, true);
            Assert.IsTrue(prob < 50, $"前提: 命中率 {prob} が 50 未満");

            Assert.AreEqual("回避", f.Src.COM.SelectDefense(u, 2, t, 0));
        }
    }
}
