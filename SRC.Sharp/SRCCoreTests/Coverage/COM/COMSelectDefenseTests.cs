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
        public void SelectDefense_InvalidWeapon_ReturnsEmpty()
        {
            var f = CreateField();
            var u = f.Deploy("攻撃機", "敵P", "敵", 3, 3);
            var t = f.Deploy("防御機", "味方P", "味方", 4, 3);

            Assert.AreEqual("", f.Src.COM.SelectDefense(u, 99, t, 0));
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
            // 反撃の方が効果が高い場合は防御行動を取らず反撃する
            var f = CreateField();
            var u = f.Deploy("攻撃機", "敵P", "敵", 3, 3);
            var t = f.Deploy("防御機", "味方P", "味方", 4, 3);

            // 攻撃: パンチ(約250)、反撃: キック相当の必殺(大ダメージ)
            Assert.AreEqual("", f.Src.COM.SelectDefense(u, 1, t, 2));
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
