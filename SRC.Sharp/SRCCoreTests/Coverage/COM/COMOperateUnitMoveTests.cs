using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Filesystem;
using SRCCore.Units;
using System;

namespace SRCCore.Coverage.Tests.COMTests
{
    /// <summary>
    /// COM.OperateUnit の移動に関する思考のテスト。
    /// 攻撃が発生しない（射程内に敵がいない）配置で、移動先の決定を確認する。
    /// VB 版 COM.bas の OperateUnit に対応する。
    /// </summary>
    [TestClass]
    public class COMOperateUnitMoveTests
    {
        private static COMTestField CreateField(int speed = 3)
        {
            var f = new COMTestField();
            // ファイルを持たない空のファイルシステム（効果音などは見つからない扱い）
            f.Src.FileSystem = new LocalFileSystem();
            COMTestField.AddWeapon(f.DefineUnit("機体", speed: speed), "パンチ", 1200);
            return f;
        }

        private static void Operate(COMTestField f, Unit u)
        {
            f.Src.Commands.SelectedUnit = u;
            f.Src.COM.OperateUnit();
        }

        private static int Distance(Unit a, Unit b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

        [TestMethod]
        public void OperateUnit_NoEnemyInReach_MovesTowardNearestEnemy()
        {
            // 射程内に敵がいなければ最も近い敵に向かって移動力いっぱいに移動する
            var f = CreateField(speed: 3);
            var u = f.Deploy("機体", "敵A", "敵", 1, 1);
            f.Deploy("機体", "味方A", "味方", 9, 1);
            f.Deploy("機体", "味方B", "味方", 1, 10);

            Operate(f, u);

            Assert.AreEqual(4, u.x);
            Assert.AreEqual(1, u.y);
            Assert.AreSame(u, f.Src.Map.MapDataForUnit[4, 1]);
            Assert.IsNull(f.Src.Map.MapDataForUnit[1, 1]);
        }

        [TestMethod]
        public void OperateUnit_StandbyMode_DoesNotMove()
        {
            // 思考モード「待機」では射程内に敵がいなければ動かない
            var f = CreateField();
            var u = f.Deploy("機体", "敵A", "敵", 1, 1);
            f.Deploy("機体", "味方A", "味方", 9, 1);
            u.Mode = "待機";

            Operate(f, u);

            Assert.AreEqual(1, u.x);
            Assert.AreEqual(1, u.y);
        }

        [TestMethod]
        public void OperateUnit_NoEnemyAtAll_StaysInPlace()
        {
            // 敵が一体もいなければ移動先が決まらないので動かない
            var f = CreateField();
            var u = f.Deploy("機体", "敵A", "敵", 5, 5);
            f.Deploy("機体", "敵B", "敵", 9, 9);

            Operate(f, u);

            Assert.AreEqual(5, u.x);
            Assert.AreEqual(5, u.y);
        }

        [TestMethod]
        public void OperateUnit_DestinationMode_MovesAndSwitchesToStandbyOnArrival()
        {
            // 思考モード「X Y」では指定地点を目指し、到着すると「待機」に切り替わる
            var f = CreateField(speed: 3);
            var u = f.Deploy("機体", "敵A", "敵", 1, 1);
            u.Mode = "3 2";

            Operate(f, u);

            Assert.AreEqual(3, u.x);
            Assert.AreEqual(2, u.y);
            Assert.AreEqual("待機", u.Mode);
        }

        [TestMethod]
        public void OperateUnit_DestinationMode_FarDestination_KeepsMode()
        {
            // 到着できなければ移動力の範囲で目的地に近付き、思考モードはそのまま
            var f = CreateField(speed: 3);
            var u = f.Deploy("機体", "敵A", "敵", 1, 1);
            u.Mode = "1 9";

            Operate(f, u);

            Assert.AreEqual(1, u.x);
            Assert.AreEqual(4, u.y);
            Assert.AreEqual("1 9", u.Mode);
        }

        [TestMethod]
        public void OperateUnit_EscapeMode_MovesAwayFromEnemy()
        {
            // 思考モード「逃亡」では移動範囲内で敵から最も遠い地点へ移動する
            var f = CreateField(speed: 3);
            var u = f.Deploy("機体", "敵A", "敵", 5, 5);
            var enemy = f.Deploy("機体", "味方A", "味方", 6, 5);
            u.Mode = "逃亡";

            Operate(f, u);

            Assert.AreEqual(4, Distance(u, enemy));
        }

        [TestMethod]
        public void OperateUnit_Dancing_DoesNothing()
        {
            // 踊っているユニットは行動しない
            var f = CreateField();
            var u = f.Deploy("機体", "敵A", "敵", 1, 1);
            f.Deploy("機体", "味方A", "味方", 9, 1);
            u.AddCondition("踊り", 1);

            Operate(f, u);

            Assert.AreEqual(1, u.x);
            Assert.AreEqual(1, u.y);
        }

        [TestMethod]
        public void OperateUnit_ResetsSelectionState()
        {
            // 行動開始時に選択中のターゲットや武器はリセットされる
            var f = CreateField();
            var u = f.Deploy("機体", "敵A", "敵", 5, 5);
            f.Src.Commands.SelectedWeapon = 3;
            f.Src.Commands.SelectedAbility = 2;

            Operate(f, u);

            Assert.AreSame(u, f.Src.Event.SelectedUnitForEvent);
            Assert.AreEqual(0, f.Src.Commands.SelectedWeapon);
            Assert.AreEqual(0, f.Src.Commands.SelectedAbility);
            Assert.IsNull(f.Src.Commands.SelectedTarget);
        }

        [TestMethod]
        public void OperateUnit_GuardMode_RepairsDamagedGuardTarget()
        {
            // 思考モードが味方パイロット名の場合、そのユニットを護衛し、損傷していれば修理する
            var f = CreateField();
            var ud = f.DefineUnit("修理機");
            ud.AddFeature("修理装置");
            var u = f.Deploy("修理機", "敵A", "敵", 1, 1);
            var guarded = f.Deploy("機体", "敵B", "敵", 1, 4);
            guarded.HP = 1000;
            u.Mode = "敵B";

            Operate(f, u);

            Assert.AreEqual(1900, guarded.HP);
            Assert.AreEqual(1, Distance(u, guarded));
        }

        [TestMethod]
        [Timeout(10000)]
        public void OperateUnit_Teleport_ConsumesENFromFeatureData()
        {
            // VB: テレポート=名称 消費EN の場合、消費ＥＮは LIndex(FeatureData("テレポート"), 2)
            // テレポートLv5=瞬間移動 20 → ＥＮを 20 消費して移動する
            var f = CreateField(speed: 3);
            var ud = f.DefineUnit("テレポート機", en: 200, speed: 3);
            ud.AddFeature("テレポートLv5=瞬間移動 20");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            var u = f.Deploy("テレポート機", "敵A", "敵", 1, 1);
            // テレポートしても攻撃が届かない位置に置く
            f.Deploy("機体", "味方A", "味方", 10, 10);

            Operate(f, u);

            Assert.AreEqual(180, u.EN);
        }

        [TestMethod]
        [Timeout(10000)]
        public void OperateUnit_Jump_ConsumesENFromFeatureData()
        {
            // VB: ジャンプ=名称 消費EN の場合、消費ＥＮは LIndex(FeatureData("ジャンプ"), 2)
            // ジャンプLv2=跳躍 30 → 地上にいればＥＮを 30 消費してジャンプ移動する
            var f = CreateField(speed: 3);
            var ud = f.DefineUnit("ジャンプ機", en: 200, speed: 3);
            ud.AddFeature("ジャンプLv2=跳躍 30");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            var u = f.Deploy("ジャンプ機", "敵A", "敵", 1, 1);
            f.Deploy("機体", "味方A", "味方", 10, 10);

            Operate(f, u);

            Assert.AreEqual(170, u.EN);
        }

        [TestMethod]
        public void OperateUnit_FixedMode_DoesNotMove()
        {
            // 思考モード「固定」では射程内に敵がいなくても動かない
            var f = CreateField();
            var u = f.Deploy("機体", "敵A", "敵", 1, 1);
            f.Deploy("機体", "味方A", "味方", 9, 1);
            u.Mode = "固定";

            Operate(f, u);

            Assert.AreEqual(1, u.x);
            Assert.AreEqual(1, u.y);
        }
    }
}
