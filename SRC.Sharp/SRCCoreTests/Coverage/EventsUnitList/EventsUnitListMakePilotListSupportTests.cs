using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.Pilots;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Coverage.Tests.EventsUnitList
{
    /// <summary>
    /// MakePilotList が追加サポートを一覧から除外する条件のテスト。
    /// VB の ExecMakePilotListCmd は 名称 以外のモードでのみ追加サポートを除外し、
    /// 名称 モードでは追加パイロットだけを除外する（CmdData.cls:7697-7706, 7763-7768）。
    /// </summary>
    [TestClass]
    public class EventsUnitListMakePilotListSupportTests
    {
        private static (SRC src, HeadlessGUI gui) CreateSrc()
        {
            var src = new SRC();
            var gui = new HeadlessGUI();
            gui.Install(src);
            src.GUI.MainWidth = 20;
            src.GUI.MainHeight = 45;
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            return (src, gui);
        }

        private static int Exec(SRC src, string cmdText)
        {
            var line = new EventDataLine(src.Event.EventData.Count, EventDataSource.Scenario, "test", 0, cmdText);
            src.Event.EventData.Add(line);
            var cmd = new CmdParser().Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd.Exec();
        }

        private static Pilot AddPilot(SRC src, string name, int level = 1)
        {
            var pd = src.PDList.Add(name);
            pd.Nickname = name;
            pd.KanaName = name;
            pd.SP = 10;
            pd.Adaption = "AAAA";
            return src.PList.Add(name, level, "味方");
        }

        private static Unit AddUnitWithAdditionalSupport(SRC src)
        {
            var ud = src.UDList.Add("母機");
            ud.Nickname = "母機";
            ud.KanaName = "ぼき";
            ud.HP = 1000;
            ud.EN = 100;
            ud.PilotNum = 1;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            var u = src.UList.Add("母機", 0, "味方");
            u.Status = "待機";
            AddPilot(src, "主操縦者").Ride(u);

            // 追加サポートはユニットに所属するがサポート一覧には載らない
            var sp = AddPilot(src, "追加サポ");
            sp.IsAdditionalSupport = true;
            sp.Unit = u;
            return u;
        }

        private static List<string> DrawnStrings(HeadlessGUI gui)
        {
            return gui.CallsOf(nameof(IGUI.DrawString))
                .Select(c => (string)c.Args[0])
                .ToList();
        }

        [TestMethod]
        public void MakePilotList_NameMode_IncludesAdditionalSupport()
        {
            // 名称 モードでは追加サポートを除外しない
            var (src, gui) = CreateSrc();
            AddUnitWithAdditionalSupport(src);

            Exec(src, "MakePilotList 名称");

            var names = DrawnStrings(gui);
            CollectionAssert.Contains(names, "主操縦者");
            CollectionAssert.Contains(names, "追加サポ");
        }

        [TestMethod]
        public void MakePilotList_LevelMode_ExcludesAdditionalSupport()
        {
            // 名称 以外のモードでは追加サポートを勘定に入れない
            var (src, gui) = CreateSrc();
            AddUnitWithAdditionalSupport(src);

            Exec(src, "MakePilotList レベル");

            var names = DrawnStrings(gui);
            CollectionAssert.Contains(names, "主操縦者");
            CollectionAssert.DoesNotContain(names, "追加サポ");
        }
    }
}
