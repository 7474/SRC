using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitAttack
{
    /// <summary>
    /// Unit.MapAttack（Unit.attackmap.cs / VB: Unit.cls MapAttack）の効果範囲・対象選択・消費・獲得資金のテスト。
    /// 乱数は FixDice(0.5) で固定し、武器は必中・クリティカル無しにする。
    /// </summary>
    [TestClass]
    public class UnitAttackMapAttackTests
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

        private static (UnitAttackFixture f, Unit a) Setup(string wclass, int minRange = 1, int maxRange = 2,
            int bullet = 0, int en = 0, string party = "味方", string wname = "マップ兵器")
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", party, 5, 5,
                weapons: new[] { f.Weapon(wname, 1500, wclass, minRange, maxRange, bullet, en) });
            return (f, a);
        }

        // Ｍ全：射程内の全ユニット（味方も含む）に攻撃し、自分自身と射程外は対象外
        [TestMethod]
        public void MapAttack_All_HitsEveryUnitInRangeExceptSelf()
        {
            var (f, a) = Setup("Ｍ全");
            var ally = f.AddUnit("僚機", "味方", 4, 5, hp: 5000);
            var near = f.AddUnit("近い敵", "敵", 6, 5, hp: 5000);
            var far = f.AddUnit("遠い敵", "敵", 5, 7, hp: 5000);
            var outside = f.AddUnit("射程外", "敵", 8, 5, hp: 5000);

            a.MapAttack(a.Weapons.First(), 6, 5);

            Assert.IsTrue(ally.HP < 5000);
            Assert.IsTrue(near.HP < 5000);
            Assert.IsTrue(far.HP < 5000);
            Assert.AreEqual(5000, outside.HP);
            Assert.AreEqual(a.MaxHP, a.HP);
            Assert.AreEqual("攻撃機は[マップ兵器]で攻撃をかけた。", f.SysMessages()[0]);
        }

        // 攻撃は起点から近い順に行われる
        [TestMethod]
        public void MapAttack_All_AttacksNearestFirst()
        {
            var (f, a) = Setup("Ｍ全", maxRange: 3);
            var far = f.AddUnit("遠い敵", "敵", 2, 5, hp: 5000);
            var near = f.AddUnit("近い敵", "敵", 6, 5, hp: 5000);
            var mid = f.AddUnit("中間の敵", "敵", 5, 7, hp: 5000);

            a.MapAttack(a.Weapons.First(), 6, 5);

            // 味方の攻撃では UpdateMessageForm(標的, 攻撃側) の順で個々の攻撃を表示する
            var order = f.Calls(nameof(IGUI.UpdateMessageForm))
                .Where(c => ReferenceEquals(c.Args[1], a) && c.Args[0] != null && !ReferenceEquals(c.Args[0], a))
                .Select(c => (Unit)c.Args[0])
                .Distinct()
                .ToList();
            CollectionAssert.AreEqual(new[] { near, mid, far }, order);
        }

        // 識属性：味方は攻撃対象から除外される
        [TestMethod]
        public void MapAttack_Identify_ExcludesAllies()
        {
            var (f, a) = Setup("Ｍ全識");
            var ally = f.AddUnit("僚機", "味方", 4, 5, hp: 5000);
            var npc = f.AddUnit("ＮＰＣ機", "ＮＰＣ", 5, 4, hp: 5000);
            var enemy = f.AddUnit("敵機", "敵", 6, 5, hp: 5000);

            a.MapAttack(a.Weapons.First(), 6, 5);

            Assert.AreEqual(5000, ally.HP);
            Assert.AreEqual(5000, npc.HP);
            Assert.IsTrue(enemy.HP < 5000);
        }

        // Ｍ直：目標方向の直線上のみが対象
        [TestMethod]
        public void MapAttack_Line_HitsOnlyTargetDirection()
        {
            var (f, a) = Setup("Ｍ直", maxRange: 3);
            var north1 = f.AddUnit("北1", "敵", 5, 4, hp: 5000);
            var north3 = f.AddUnit("北3", "敵", 5, 2, hp: 5000);
            var east = f.AddUnit("東", "敵", 6, 5, hp: 5000);
            var south = f.AddUnit("南", "敵", 5, 6, hp: 5000);

            a.MapAttack(a.Weapons.First(), 5, 4);

            Assert.IsTrue(north1.HP < 5000);
            Assert.IsTrue(north3.HP < 5000);
            Assert.AreEqual(5000, east.HP);
            Assert.AreEqual(5000, south.HP);
        }

        // Ｍ投：目標地点を中心にレベルの半径が対象
        [TestMethod]
        public void MapAttack_Throw_HitsAroundTargetPoint()
        {
            var (f, a) = Setup("Ｍ投L1", maxRange: 4);
            var center = f.AddUnit("中心", "敵", 8, 5, hp: 5000);
            var adjacent = f.AddUnit("隣", "敵", 8, 6, hp: 5000);
            var away = f.AddUnit("離れ", "敵", 6, 5, hp: 5000);

            a.MapAttack(a.Weapons.First(), 8, 5);

            Assert.IsTrue(center.HP < 5000);
            Assert.IsTrue(adjacent.HP < 5000);
            Assert.AreEqual(5000, away.HP);
        }

        // 弾数・ＥＮの消費は標的の数によらず 1 回だけ
        [TestMethod]
        public void MapAttack_ConsumesWeaponOnce()
        {
            var (f, a) = Setup("Ｍ全", bullet: 3, en: 20);
            f.AddUnit("敵1", "敵", 6, 5, hp: 5000);
            f.AddUnit("敵2", "敵", 4, 5, hp: 5000);
            var w = a.Weapons.First();

            a.MapAttack(w, 6, 5);

            Assert.AreEqual(2, w.Bullet());
            Assert.AreEqual(80, a.EN);
        }

        // マップ攻撃の個々の攻撃では命中しても攻撃側の気力は上がらないが、標的は +1
        [TestMethod]
        public void MapAttack_IndividualAttack_TargetMoraleUp()
        {
            var (f, a) = Setup("Ｍ全");
            var enemy = f.AddUnit("敵機", "敵", 6, 5, hp: 5000);
            a.MapAttack(a.Weapons.First(), 6, 5);
            Assert.AreEqual(101, enemy.MainPilot().Morale);
        }

        // 味方による破壊では資金（修理費の半分）を得てメッセージを表示する
        [TestMethod]
        public void MapAttack_Destroy_EarnsHalfOfValue()
        {
            var (f, a) = Setup("Ｍ全");
            var enemy = f.AddUnit("敵機", "敵", 6, 5, hp: 500);
            f.Src.UDList.Item("敵機").Value = 3000;
            var prevMoney = f.Src.Money;

            a.MapAttack(a.Weapons.First(), 6, 5);

            Assert.AreEqual("破壊", enemy.Status);
            Assert.AreEqual(prevMoney + 1500, f.Src.Money);
            Assert.IsTrue(f.SysMessages().Contains("1500の資金を得た。"));
        }

        // 敵陣営のマップ攻撃では資金は得られない
        [TestMethod]
        public void MapAttack_EnemyAttacker_EarnsNothing()
        {
            var (f, a) = Setup("Ｍ全", party: "敵");
            var player = f.AddUnit("自機", "味方", 6, 5, hp: 500);
            f.Src.UDList.Item("自機").Value = 3000;
            var prevMoney = f.Src.Money;

            a.MapAttack(a.Weapons.First(), 6, 5);

            Assert.AreEqual("破壊", player.Status);
            Assert.AreEqual(prevMoney, f.Src.Money);
        }

        // 攻撃不能状態では（イベントによる攻撃でない限り）何もせずに終了する
        [TestMethod]
        public void MapAttack_CannotAttack_DoesNothing()
        {
            var (f, a) = Setup("Ｍ全", bullet: 3);
            var enemy = f.AddUnit("敵機", "敵", 6, 5, hp: 5000);
            a.AddCondition("攻撃不能", 1);
            var w = a.Weapons.First();

            a.MapAttack(w, 6, 5);

            Assert.AreEqual(5000, enemy.HP);
            Assert.AreEqual(3, w.Bullet());
        }

        // 自属性：攻撃後に自身は破壊される
        [TestMethod]
        public void MapAttack_SelfDestruct_DestroysAttacker()
        {
            var (f, a) = Setup("Ｍ全自");
            var enemy = f.AddUnit("敵機", "敵", 6, 5, hp: 5000);

            a.MapAttack(a.Weapons.First(), 6, 5);

            Assert.IsTrue(enemy.HP < 5000);
            Assert.AreEqual("破壊", a.Status);
        }

        // Ｍ移：攻撃側は目標地点に移動し、通過した経路上のユニットを攻撃する
        [TestMethod]
        public void MapAttack_Move_JumpsToTargetPoint()
        {
            var (f, a) = Setup("Ｍ移", maxRange: 4);
            var onPath = f.AddUnit("経路上", "敵", 7, 5, hp: 5000);

            a.MapAttack(a.Weapons.First(), 9, 5);

            Assert.IsTrue(onPath.HP < 5000);
            Assert.AreEqual(9, a.x);
            Assert.AreEqual(5, a.y);
            Assert.AreSame(a, f.Src.Map.MapDataForUnit[9, 5]);
        }

        // ステルスを持つユニットは攻撃するとステルス無効状態になる
        [TestMethod]
        public void MapAttack_Stealth_BecomesRevealed()
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 5, 5,
                weapons: new[] { f.Weapon("マップ兵器", 1500, "Ｍ全", 1, 2) },
                features: new[] { "ステルス" });
            f.AddUnit("敵機", "敵", 6, 5, hp: 5000);

            a.MapAttack(a.Weapons.First(), 6, 5);

            Assert.IsTrue(a.IsConditionSatisfied("ステルス無効"));
        }

        // 合体技のマップ攻撃：パートナー全員の同名武器の弾数を消費し、メッセージにパートナー名を含める
        [TestMethod]
        public void MapAttack_Combination_ConsumesEveryPartner()
        {
            var f = new UnitAttackFixture();
            var a = f.AddUnit("攻撃機", "味方", 5, 5,
                weapons: new[] { f.Weapon("合体剣", 1500, "Ｍ全合", 1, 2, bullet: 3) },
                features: new[] { "合体技=合体剣 僚機Ａ 僚機Ｂ" });
            var p1 = f.AddUnit("僚機Ａ", "味方", 9, 9, weapons: new[] { f.Weapon("合体剣", 1500, "Ｍ全合", 1, 2, bullet: 3) });
            var p2 = f.AddUnit("僚機Ｂ", "味方", 10, 9, weapons: new[] { f.Weapon("合体剣", 1500, "Ｍ全合", 1, 2, bullet: 3) });
            var enemy = f.AddUnit("敵機", "敵", 6, 5, hp: 5000);

            a.MapAttack(a.Weapons.First(), 6, 5);

            Assert.IsTrue(enemy.HP < 5000);
            Assert.AreEqual("攻撃機は[僚機Ａ]、[僚機Ｂ]と共に[合体剣]で攻撃をかけた。", f.SysMessages()[0]);
            Assert.AreEqual(2, a.Weapons.First().Bullet());
            Assert.AreEqual(2, p1.Weapons.First().Bullet());
            Assert.AreEqual(2, p2.Weapons.First().Bullet());
        }
    }
}
