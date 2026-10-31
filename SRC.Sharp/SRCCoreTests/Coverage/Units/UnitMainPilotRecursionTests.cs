using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Maps;
using SRCCore.Lib;
using SRCCore.Units;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Unit.MainPilot と、気力の設定・パイロットの更新・支援の修正の見直しが互いを呼び続けないこと（#1411）。
    /// 元はどれもスタックオーバーフローになり、テストのプロセスごと落ちていた。
    /// </summary>
    [TestClass]
    public class UnitMainPilotRecursionTests
    {
        private static void SetupMap(UnitAbilityCoverageFixture fx)
        {
            var m = fx.Src.Map;
            m.MapWidth = 10;
            m.MapHeight = 10;
            m.MapDataForUnit = new Src2DArray<Unit>(10, 10);
            m.MaskData = new Src2DArray<bool>(10, 10);
            // マップの上にいるユニットとして扱わせる（空だとステータスの表示用のマップとみなされる）
            m.MapFileName = "test.map";
            m.MapData = new Src2DArray<MapCell>(10, 10);
            for (var x = 1; x <= 10; x++)
            {
                for (var y = 1; y <= 10; y++)
                {
                    m.MapData[x, y] = MapCell.EmptyCell;
                }
            }
        }

        private static Unit Deploy(UnitAbilityCoverageFixture fx, string uname, string pname, int x, int y)
        {
            var u = fx.Src.UList.Add(uname, 0, "味方");
            var p = fx.Src.PList.Add(pname, 1, "味方");
            p.Ride(u);
            u.Status = "出撃";
            u.x = x;
            u.y = y;
            fx.Src.Map.MapDataForUnit[x, y] = u;
            return u;
        }

        [TestMethod]
        public void MainPilot_BerserkPilot_DoesNotRecurse()
        {
            // 暴走時パイロットは呼ばれるたびに気力を合わせる。気力の設定は気力上限の技能を見るために MainPilot を呼ぶ
            var fx = new UnitAbilityCoverageFixture();
            var ud = fx.AddUnitData("暴走機");
            ud.PilotNum = 1;
            ud.AddFeature("暴走時パイロット=暴走パイロット");
            fx.AddPilotData("乗るパイロット");
            fx.AddPilotData("暴走パイロット");
            var u = fx.Src.UList.Add("暴走機", 0, "味方");
            var rider = fx.Src.PList.Add("乗るパイロット", 1, "味方");
            rider.Ride(u);
            u.AddCondition("暴走", 3);

            // 1 回目は暴走時パイロットを作る。2 回目は作ってある暴走時パイロットに合わせる
            var first = u.MainPilot();
            rider.Morale = 120;
            var second = u.MainPilot();

            Assert.AreEqual("暴走パイロット", first.Name);
            Assert.AreSame(first, second);
            Assert.AreSame(u, second.Unit);
            // VB 版と同じく、呼ばれるたびに乗っているパイロットの気力に合わせる
            Assert.AreEqual(120, second.Morale);
        }

        [TestMethod]
        public void StandBy_UncontrollableUnitWithBerserkPilot_DoesNotRecurse()
        {
            // 制御不可のユニットは出撃するときに暴走の状態になり、そのまま Update で暴走時パイロットを使う
            var fx = new UnitAbilityCoverageFixture();
            SetupMap(fx);
            var ud = fx.AddUnitData("制御不可機");
            ud.PilotNum = 1;
            ud.AddFeature("制御不可");
            ud.AddFeature("暴走時パイロット=暴走パイロット");
            fx.AddPilotData("乗るパイロット");
            fx.AddPilotData("暴走パイロット");
            var u = fx.Src.UList.Add("制御不可機", 0, "味方");
            fx.Src.PList.Add("乗るパイロット", 1, "味方").Ride(u);

            u.StandBy(5, 5);

            Assert.IsTrue(u.IsConditionSatisfied("暴走"));
            Assert.AreEqual("暴走パイロット", u.MainPilot().Name);
        }

        [TestMethod]
        public void MainPilot_TwoNearbyUnitsSharingNamedAdditionalPilot_DoesNotRecurse()
        {
            // 名前付きの追加パイロットは、それを指すすべてのユニットで使い回される。
            // 近くにいる 2 体の間で、支援の修正の見直しのたびにパイロットを取り合い続けていた
            var fx = new UnitAbilityCoverageFixture();
            SetupMap(fx);
            foreach (var name in new[] { "機体Ａ", "機体Ｂ" })
            {
                var ud = fx.AddUnitData(name);
                ud.PilotNum = 1;
                ud.AddFeature("追加パイロット=共有パイロット");
                ud.AddFeature("パイロット能力付加=\"素質=自己進化能力\"");
            }
            fx.AddPilotData("Ａ");
            fx.AddPilotData("Ｂ");
            fx.AddPilotData("共有パイロット");
            var u1 = Deploy(fx, "機体Ａ", "Ａ", 3, 3);
            var u2 = Deploy(fx, "機体Ｂ", "Ｂ", 3, 4);

            Assert.AreEqual("共有パイロット", u1.MainPilot().Name);
            Assert.AreEqual("共有パイロット", u2.MainPilot().Name);
            Assert.AreEqual("共有パイロット", u1.MainPilot().Name);
            u1.Update();
            u2.Update();
            fx.Src.PList.UpdateSupportMod();
        }
    }
}
