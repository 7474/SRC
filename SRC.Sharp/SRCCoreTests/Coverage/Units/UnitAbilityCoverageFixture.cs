using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// UnitAbility 系カバレッジテスト共通の組み立て。
    /// HeadlessGUI で GUI 呼び出しを記録し、表示メッセージを検証できるようにする。
    /// </summary>
    internal class UnitAbilityCoverageFixture
    {
        public SRC Src { get; }
        public HeadlessGUI Gui { get; }

        public UnitAbilityCoverageFixture()
        {
            Src = new SRC();
            Gui = new HeadlessGUI();
            Gui.Install(Src);
            Src.Event.EventData = new List<EventDataLine>();
            Src.Event.EventCmd = new List<CmdData>();
            Src.Event.EventFileNames = new List<string>();
            Src.Event.AdditionalEventFileNames = new List<string>();
            Src.Event.EventQue = new List<string>();
            Src.Titles = new List<string>();
        }

        public UnitData AddUnitData(string name, int hp = 1000, int en = 100)
        {
            var ud = Src.UDList.Add(name);
            ud.HP = hp;
            ud.EN = en;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.Nickname = name;
            return ud;
        }

        public PilotData AddPilotData(string name)
        {
            var pd = Src.PDList.Add(name);
            pd.Nickname = name;
            return pd;
        }

        /// <summary>
        /// ユニットデータ・パイロットデータから出撃前のユニットを作り、全回復させる。
        /// </summary>
        public Unit CreateUnit(string uname, string pname, string party = "味方")
        {
            if (!Src.PDList.IsDefined(pname))
            {
                AddPilotData(pname);
            }
            var unit = Src.UList.Add(uname, 0, party);
            var pilot = Src.PList.Add(pname, 1, party);
            unit.AddPilot(pilot);
            pilot.Unit = unit;
            unit.Update();
            unit.FullRecover();
            return unit;
        }

        public IEnumerable<string> SysMessages =>
            Gui.CallsOf(nameof(IGUI.DisplaySysMessage)).Select(x => (string)x.Args[0]);
    }
}
