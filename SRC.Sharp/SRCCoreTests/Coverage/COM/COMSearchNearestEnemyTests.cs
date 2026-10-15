using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCCore.Coverage.Tests.COMTests
{
    /// <summary>
    /// COM.SearchNearestEnemy / IsAbleToCounterAttack のテスト。
    /// VB 版 COM.bas の SearchNearestEnemy / IsAbleToCounterAttack に対応する。
    /// </summary>
    [TestClass]
    public class COMSearchNearestEnemyTests
    {
        private static COMTestField CreateField()
        {
            var f = new COMTestField();
            COMTestField.AddWeapon(f.DefineUnit("機体"), "パンチ", 1200);
            return f;
        }

        [TestMethod]
        public void SearchNearestEnemy_ReturnsClosestEnemyByManhattanDistance()
        {
            // 敵から見て味方ユニットのうちマンハッタン距離が最も近いものを返す
            var f = CreateField();
            var u = f.Deploy("機体", "敵A", "敵", 5, 5);
            f.Deploy("機体", "味方A", "味方", 9, 9);   // 距離 8
            var near = f.Deploy("機体", "味方B", "味方", 3, 4); // 距離 3
            f.Deploy("機体", "味方C", "味方", 5, 1);   // 距離 4

            Assert.AreSame(near, f.Src.COM.SearchNearestEnemy(u));
        }

        [TestMethod]
        public void SearchNearestEnemy_IgnoresAllies()
        {
            // 自軍のユニットはどれだけ近くても対象にしない
            var f = CreateField();
            var u = f.Deploy("機体", "敵A", "敵", 5, 5);
            f.Deploy("機体", "敵B", "敵", 5, 6);
            var enemy = f.Deploy("機体", "味方A", "味方", 10, 10);

            Assert.AreSame(enemy, f.Src.COM.SearchNearestEnemy(u));
        }

        [TestMethod]
        public void SearchNearestEnemy_NoEnemy_ReturnsNull()
        {
            var f = CreateField();
            var u = f.Deploy("機体", "敵A", "敵", 5, 5);
            f.Deploy("機体", "敵B", "敵", 5, 6);

            Assert.IsNull(f.Src.COM.SearchNearestEnemy(u));
        }

        [TestMethod]
        public void SearchNearestEnemy_PartyMode_OnlyTargetsThatParty()
        {
            // 思考モードが陣営名 (ＮＰＣ) の場合はその陣営のユニットのみを狙う
            var f = CreateField();
            var u = f.Deploy("機体", "敵A", "敵", 5, 5);
            f.Deploy("機体", "味方A", "味方", 5, 6);
            var npc = f.Deploy("機体", "ＮＰＣA", "ＮＰＣ", 1, 1);
            u.Mode = "ＮＰＣ";

            Assert.AreSame(npc, f.Src.COM.SearchNearestEnemy(u));
        }

        [TestMethod]
        public void SearchNearestEnemy_UnderGroundTarget_IsInvisible()
        {
            // 地中にいるユニットは目視できない
            var f = CreateField();
            var u = f.Deploy("機体", "敵A", "敵", 5, 5);
            var hidden = f.Deploy("機体", "味方A", "味方", 5, 6);
            var visible = f.Deploy("機体", "味方B", "味方", 9, 9);
            hidden.Area = "地中";

            Assert.AreSame(visible, f.Src.COM.SearchNearestEnemy(u));
        }

        [TestMethod]
        public void SearchNearestEnemy_HiddenBySpecialPower_IsInvisible()
        {
            // 隠れ身 (スペシャルパワー) 使用中のユニットは目視できない
            var f = CreateField();
            f.Src.SPDList.Add("隠れ身").SetEffect("隠れ身");
            var u = f.Deploy("機体", "敵A", "敵", 5, 5);
            var hidden = f.Deploy("機体", "味方A", "味方", 5, 6);
            var visible = f.Deploy("機体", "味方B", "味方", 9, 9);

            Assert.AreSame(hidden, f.Src.COM.SearchNearestEnemy(u));

            hidden.MakeSpecialPowerInEffect("隠れ身");
            Assert.AreSame(visible, f.Src.COM.SearchNearestEnemy(u));
        }

        [TestMethod]
        public void SearchNearestEnemy_StealthNullifiedCondition_IsVisibleFromAfar()
        {
            // ステルス無効状態のユニットは遠くからでも発見できる
            var f = CreateField();
            f.DefineUnit("ステルス機").AddFeature("ステルス");
            var u = f.Deploy("機体", "敵A", "敵", 1, 1);
            var stealth = f.Deploy("ステルス機", "味方A", "味方", 1, 8);
            Assert.IsNull(f.Src.COM.SearchNearestEnemy(u));

            stealth.AddCondition("ステルス無効", 1);
            Assert.AreSame(stealth, f.Src.COM.SearchNearestEnemy(u));
        }

        [TestMethod]
        public void SearchNearestEnemy_StealthWithoutLevel_VisibleOnlyWithin3()
        {
            // レベル指定のないステルスは距離 3 以内でなければ発見できない
            var f = CreateField();
            f.DefineUnit("ステルス機").AddFeature("ステルス");
            var u = f.Deploy("機体", "敵A", "敵", 1, 1);
            var stealth = f.Deploy("ステルス機", "味方A", "味方", 1, 5); // 距離 4
            var other = f.Deploy("機体", "味方B", "味方", 8, 8);

            Assert.AreSame(other, f.Src.COM.SearchNearestEnemy(u));

            // 距離 3 まで近付けば発見できる
            u.Move(1, 2);
            Assert.AreSame(stealth, f.Src.COM.SearchNearestEnemy(u));
        }

        [TestMethod]
        public void SearchNearestEnemy_StealthWithLevel_UsesLevelAsRange()
        {
            // ステルスLv1 は距離 1 でなければ発見できない
            var f = CreateField();
            f.DefineUnit("ステルス機").AddFeature("ステルスLv1");
            var u = f.Deploy("機体", "敵A", "敵", 1, 1);
            f.Deploy("ステルス機", "味方A", "味方", 1, 3); // 距離 2

            Assert.IsNull(f.Src.COM.SearchNearestEnemy(u));
        }

        [TestMethod]
        public void SearchNearestEnemy_StealthNullifier_SeesStealthUnit()
        {
            // ステルス無効化を持つユニットからはステルスが効かない
            var f = CreateField();
            f.DefineUnit("ステルス機").AddFeature("ステルス");
            var ud = f.DefineUnit("索敵機");
            ud.AddFeature("ステルス無効化");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            var u = f.Deploy("索敵機", "敵A", "敵", 1, 1);
            var stealth = f.Deploy("ステルス機", "味方A", "味方", 1, 8);

            Assert.AreSame(stealth, f.Src.COM.SearchNearestEnemy(u));
        }

        [TestMethod]
        public void IsAbleToCounterAttack_TrueOnlyWhenTargetWithinWeaponRange()
        {
            // 反撃可能な武器 (射程内でダメージを与えられる武器) があるかどうか
            var f = CreateField();
            var u = f.Deploy("機体", "味方A", "味方", 5, 5);
            var adjacent = f.Deploy("機体", "敵A", "敵", 5, 6);
            var far = f.Deploy("機体", "敵B", "敵", 5, 8);

            Assert.IsTrue(f.Src.COM.IsAbleToCounterAttack(u, adjacent));
            Assert.IsFalse(f.Src.COM.IsAbleToCounterAttack(u, far));
        }

        [TestMethod]
        public void IsAbleToCounterAttack_MapWeaponIsNotUsedForCounter()
        {
            // マップ攻撃は反撃に使えない
            var f = CreateField();
            var ud = f.DefineUnit("マップ兵器機");
            COMTestField.AddWeapon(ud, "マップ兵器", 1500, 1, 3, wclass: "Ｍ全");
            var u = f.Deploy("マップ兵器機", "味方A", "味方", 5, 5);
            var t = f.Deploy("機体", "敵A", "敵", 5, 6);

            Assert.IsFalse(f.Src.COM.IsAbleToCounterAttack(u, t));
        }
    }
}
