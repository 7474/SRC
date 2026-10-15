using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Collections.Generic;

namespace SRCCore.Coverage.Tests.SpecialPower
{
    /// <summary>
    /// スペシャルパワー関連テスト用の共通フィクスチャ。
    /// GUI 呼び出しは記録するだけのモックにし、ユニット・パイロットは実データで組み立てる。
    /// </summary>
    internal class SpecialPowerTestFixture
    {
        private class NullGUIStatus : IGUIStatus
        {
            public Unit DisplayedUnit { get; set; }
            public Pilot DisplayedPilot { get; set; }
            public void DisplayGlobalStatus() { }
            public void DisplayUnitStatus(Unit u, Pilot p = null) { }
            public void DisplayPilotStatus(Pilot p) { }
            public void InstantUnitStatusDisplay(int X, int Y) { }
            public void ClearUnitStatus() { }
        }

        public SRC Src { get; }
        public MockGUI GUI { get; }
        public List<string> SysMessages { get; } = new List<string>();
        public List<string> SysStrings { get; } = new List<string>();

        public SpecialPowerTestFixture()
        {
            GUI = new MockGUI();
            GUI.CenterHandler = (x, y) => { };
            GUI.RedrawScreenHandler = (late) => { };
            GUI.RefreshScreenHandler = (a, b) => { };
            GUI.EraseUnitBitmapHandler = (x, y, r) => { };
            GUI.PaintUnitBitmapHandler = (u, m) => { };
            GUI.OpenMessageFormHandler = (u1, u2) => { GUI.MessageFormVisible = true; };
            GUI.CloseMessageFormHandler = () => { GUI.MessageFormVisible = false; };
            GUI.UpdateMessageFormHandler = (u1, u2) => { };
            GUI.DisplaySysMessageHandler = (m, b) => { SysMessages.Add(m); };
            GUI.DrawSysStringHandler = (x, y, m, b) => { SysStrings.Add(m); };
            GUI.UnlockGUIHandler = () => { };
            GUI.LockGUIHandler = () => { };
            GUI.SleepHandler = (ms, e) => { };
            GUI.UpdateScreenHandler = () => { };

            Src = new SRC { GUI = GUI };
            Src.GUIStatus = new NullGUIStatus();
            Src.Event.EventData = new List<EventDataLine>();
            Src.Event.EventCmd = new List<CmdData>();
            Src.Event.EventFileNames = new List<string>();
            Src.Event.AdditionalEventFileNames = new List<string>();
            Src.Event.EventQue = new List<string>();
            Src.Titles = new List<string>();
            // マップは 10x10 をフィールドに直接置く (SetMapSize は GUI 実装を要する)
            Src.Map.MapWidth = 10;
            Src.Map.MapHeight = 10;
            Src.Map.MapDataForUnit = new Src2DArray<Unit>(10, 10);
            Src.Map.MaskData = new Src2DArray<bool>(10, 10);
            Src.Map.MapFileName = "";
            Src.Map.MapData = new Src2DArray<MapCell>(10, 10);
            for (var x = 1; x <= 10; x++)
            {
                for (var y = 1; y <= 10; y++)
                {
                    Src.Map.MapData[x, y] = MapCell.EmptyCell;
                }
            }
        }

        /// <summary>スペシャルパワーを定義する</summary>
        public SpecialPowerData DefineSP(string name, string target, string duration, string effects,
            int cost = 10, string necessaryCondition = "")
        {
            var sd = Src.SPDList.Add(name);
            sd.ShortName = name;
            sd.KanaName = name;
            sd.SPConsumption = cost;
            sd.TargetType = target;
            sd.Duration = duration;
            sd.NecessaryCondition = necessaryCondition;
            sd.Animation = name;
            sd.SetEffect(effects);
            return sd;
        }

        /// <summary>パイロットデータを定義する</summary>
        public PilotData DefinePilot(string name, int sp = 50, int technique = 100)
        {
            var pd = Src.PDList.Add(name);
            pd.Nickname = name;
            pd.SP = sp;
            pd.Technique = technique;
            pd.Personality = "普通";
            return pd;
        }

        /// <summary>パイロットを乗せたユニットを出撃させる</summary>
        public Unit AddUnit(string uname, string pname, string party, int x, int y,
            int level = 1, int hp = 1000, int en = 100)
        {
            if (!Src.PDList.IsDefined(pname))
            {
                DefinePilot(pname);
            }
            var ud = Src.UDList.IsDefined(uname) ? Src.UDList.Item(uname) : Src.UDList.Add(uname);
            ud.Nickname = uname;
            ud.HP = hp;
            ud.EN = en;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            var u = Src.UList.Add(uname, 0, party);
            var p = Src.PList.Add(pname, level, party);
            u.AddPilot(p);
            p.Unit = u;
            u.Update();
            u.FullRecover();
            u.Status = "出撃";
            u.x = x;
            u.y = y;
            Src.Map.MapDataForUnit[x, y] = u;
            return u;
        }
    }
}
