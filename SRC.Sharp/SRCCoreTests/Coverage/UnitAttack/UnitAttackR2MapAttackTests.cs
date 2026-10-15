using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitAttack
{
    /// <summary>
    /// Unit.MapAttack（Unit.attackmap.cs / VB: Unit.cls MapAttack）の追加テスト：
    /// Ｍ拡・Ｍ扇・Ｍ線の効果範囲の向き、敵側の識属性、獲得資金の補正。
    /// 乱数は FixDice(0.5) で固定し、武器は必中・クリティカル無しにする。
    /// </summary>
    [TestClass]
    public class UnitAttackR2MapAttackTests
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

        private static (UnitAttackFixture f, Unit a) Setup(string wclass, int minRange = 1, int maxRange = 3,
            string party = "味方", (string, double)[] skills = null)
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", party, 5, 5,
                weapons: new[] { f.Weapon("マップ兵器", 1500, wclass, minRange, maxRange) },
                skills: skills);
            return (f, a);
        }

        // Ｍ拡：目標方向（ここでは北）に広がる範囲。中央列は射程全体、両隣の列は射程の中間のみ
        [TestMethod]
        public void MapAttack_Cone_NorthDirection()
        {
            var (f, a) = Setup("Ｍ拡");
            var center = f.AddUnit("中央", "敵", 5, 2, hp: 5000);
            var side = f.AddUnit("斜め", "敵", 4, 3, hp: 5000);
            var sideNear = f.AddUnit("斜め手前", "敵", 4, 4, hp: 5000);
            var south = f.AddUnit("南", "敵", 5, 6, hp: 5000);

            a.MapAttack(a.Weapons.First(), 5, 3);

            Assert.IsTrue(center.HP < 5000);
            Assert.IsTrue(side.HP < 5000);
            Assert.AreEqual(5000, sideNear.HP);
            Assert.AreEqual(5000, south.HP);
        }

        // Ｍ扇：目標方向（ここでは西）にレベルに応じた扇形の範囲。Lv4 は距離 i で横幅 ±i
        [TestMethod]
        public void MapAttack_Sector_WestDirection()
        {
            var (f, a) = Setup("Ｍ扇L4");
            var wide = f.AddUnit("西2上2", "敵", 3, 3, hp: 5000);
            var tooWide = f.AddUnit("西1上2", "敵", 4, 3, hp: 5000);
            var east = f.AddUnit("東", "敵", 6, 5, hp: 5000);

            a.MapAttack(a.Weapons.First(), 3, 5);

            Assert.IsTrue(wide.HP < 5000);
            Assert.AreEqual(5000, tooWide.HP);
            Assert.AreEqual(5000, east.HP);
        }

        // Ｍ線：攻撃側から目標地点までの直線上のユニットだけが対象（目標地点より先は対象外）
        [TestMethod]
        public void MapAttack_PointToPoint_HitsOnlyUnitsOnSegment()
        {
            var (f, a) = Setup("Ｍ線");
            var first = f.AddUnit("線上1", "敵", 6, 5, hp: 5000);
            var end = f.AddUnit("終点", "敵", 8, 5, hp: 5000);
            var beyond = f.AddUnit("終点の先", "敵", 9, 5, hp: 5000);
            var off = f.AddUnit("線外", "敵", 7, 6, hp: 5000);

            a.MapAttack(a.Weapons.First(), 8, 5);

            Assert.IsTrue(first.HP < 5000);
            Assert.IsTrue(end.HP < 5000);
            Assert.AreEqual(5000, beyond.HP);
            Assert.AreEqual(5000, off.HP);
            // Ｍ線は Ｍ移 と違って攻撃側は移動しない
            Assert.AreEqual(5, a.x);
        }

        // 敵陣営の識属性：敵（同陣営）は除外し、味方とＮＰＣは攻撃する
        [TestMethod]
        public void MapAttack_Identify_EnemyAttackerExcludesOnlyEnemies()
        {
            var (f, a) = Setup("Ｍ全識", maxRange: 2, party: "敵");
            var enemyAlly = f.AddUnit("敵僚機", "敵", 4, 5, hp: 5000);
            var player = f.AddUnit("自機", "味方", 6, 5, hp: 5000);
            var npc = f.AddUnit("ＮＰＣ機", "ＮＰＣ", 5, 4, hp: 5000);

            a.MapAttack(a.Weapons.First(), 6, 5);

            Assert.AreEqual(5000, enemyAlly.HP);
            Assert.IsTrue(player.HP < 5000);
            Assert.IsTrue(npc.HP < 5000);
        }

        private static void AddEarningsSp(UnitAttackFixture f, Unit a)
        {
            var sp = f.Src.SPDList.Add("幸運");
            sp.ShortName = "幸";
            sp.Duration = "ターン";
            sp.SetEffect("獲得資金増加Lv10");
            a.MakeSpecialPowerInEffect("幸運");
        }

        // 資金獲得スキル：獲得資金が (10 + レベル) / 10 倍になる（修理費 3000 の半分 1500 × 1.3）
        [TestMethod]
        public void MapAttack_Earnings_MoneySkill()
        {
            var (f, a) = Setup("Ｍ全", maxRange: 2, skills: new[] { ("資金獲得", 3d) });
            f.AddUnit("敵機", "敵", 6, 5, hp: 500);
            f.Src.UDList.Item("敵機").Value = 3000;
            var prevMoney = f.Src.Money;

            a.MapAttack(a.Weapons.First(), 6, 5);

            Assert.AreEqual(prevMoney + 1950, f.Src.Money);
        }

        // 獲得資金増加の効果：(1 + 0.1 × レベル) 倍。資金獲得スキルとは重複しない
        [TestMethod]
        public void MapAttack_Earnings_SpecialPowerOverridesSkill()
        {
            var (f, a) = Setup("Ｍ全", maxRange: 2, skills: new[] { ("資金獲得", 3d) });
            AddEarningsSp(f, a);
            f.AddUnit("敵機", "敵", 6, 5, hp: 500);
            f.Src.UDList.Item("敵機").Value = 3000;
            var prevMoney = f.Src.Money;

            a.MapAttack(a.Weapons.First(), 6, 5);

            Assert.AreEqual(prevMoney + 3000, f.Src.Money);
            Assert.IsTrue(f.SysMessages().Contains("3000の資金を得た。"));
        }

        // オプション「収得効果重複」では獲得資金増加と資金獲得スキルの両方が掛かる（1500 × 2 × 1.3）
        [TestMethod]
        public void MapAttack_Earnings_StackOption()
        {
            var (f, a) = Setup("Ｍ全", maxRange: 2, skills: new[] { ("資金獲得", 3d) });
            f.Src.Expression.DefineGlobalVariable("Option(収得効果重複)");
            AddEarningsSp(f, a);
            f.AddUnit("敵機", "敵", 6, 5, hp: 500);
            f.Src.UDList.Item("敵機").Value = 3000;
            var prevMoney = f.Src.Money;

            a.MapAttack(a.Weapons.First(), 6, 5);

            Assert.AreEqual(prevMoney + 3900, f.Src.Money);
        }
    }
}
