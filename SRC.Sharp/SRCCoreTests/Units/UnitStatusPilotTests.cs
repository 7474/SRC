using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// ステータスウィンドウで顔グラフィックのクリックにより表示するパイロットを切り替える順番
    /// （Main.frm の picFace_Click、Status.bas の DisplayUnitStatus の pindex）のテスト（7474/SRC#1485）
    /// </summary>
    [TestClass]
    public class UnitStatusPilotTests
    {
        private static (Unit u, Pilots.Pilot main, Pilots.Pilot sub, Pilots.Pilot support) CreateUnit()
        {
            var src = new SRC { GUI = new MockGUI() };
            foreach (var name in new[] { "メイン", "サブ", "サポート" })
            {
                src.PDList.Add(name).SP = 10;
            }
            var main = src.PList.Add("メイン", 1, "味方");
            var sub = src.PList.Add("サブ", 1, "味方");
            var support = src.PList.Add("サポート", 1, "味方");
            var ud = src.UDList.Add("複座機");
            ud.HP = 1000;
            ud.EN = 100;
            ud.PilotNum = 2;
            var u = src.UList.Add("複座機", 0, "味方");
            main.Ride(u);
            sub.Ride(u);
            u.AddSupport(support);
            support.Unit = u;
            u.Update();
            return (u, main, sub, support);
        }

        [TestMethod]
        public void StatusPilot_CyclesPilotsThenSupports()
        {
            var (u, main, sub, support) = CreateUnit();

            Assert.AreEqual(3, u.CountStatusPilot());
            Assert.AreSame(main, u.StatusPilot(0));
            Assert.AreSame(main, u.StatusPilot(1));
            Assert.AreSame(sub, u.StatusPilot(2));
            Assert.AreSame(support, u.StatusPilot(3));

            // メインパイロットを表示しているときは 1 番目。クリックごとに次へ進み、最後の次は 1 番目に戻る
            var pindex = u.StatusPilotIndex(null);
            Assert.AreEqual(1, pindex);
            pindex = u.NextStatusPilotIndex(pindex);
            Assert.AreEqual(2, pindex);
            pindex = u.NextStatusPilotIndex(pindex);
            Assert.AreEqual(3, pindex);
            pindex = u.NextStatusPilotIndex(pindex);
            Assert.AreEqual(1, pindex);
        }

        [TestMethod]
        public void StatusPilotIndex_ReturnsIndexOfDisplayedPilot()
        {
            var (u, main, sub, support) = CreateUnit();

            Assert.AreEqual(1, u.StatusPilotIndex(main));
            Assert.AreEqual(2, u.StatusPilotIndex(sub));
            Assert.AreEqual(3, u.StatusPilotIndex(support));
        }
    }
}
