using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Collections.Generic;

namespace SRCCore.Coverage.Tests.Units
{
    /// <summary>
    /// Unit.map.cs / Unit.action2.cs のテスト用の共通準備。
    /// 地形を指定できる小さなマップと、GUI 呼び出しを記録する MockGUI を用意する。
    /// </summary>
    internal class UnitMapActionFixture
    {
        // ステータス表示の解除（Escape から呼ばれる）を記録する
        internal class RecordingGUIStatus : IGUIStatus
        {
            public Unit DisplayedUnit { get; set; }
            public Pilot DisplayedPilot { get; set; }
            public int ClearCount { get; private set; }
            public void DisplayGlobalStatus() { }
            public void DisplayUnitStatus(Unit u, Pilot p = null) { }
            public void DisplayPilotStatus(Pilot p) { }
            public void InstantUnitStatusDisplay(int X, int Y) { }
            public void ClearUnitStatus() { ClearCount++; DisplayedUnit = null; }
        }

        public SRC Src { get; }
        public MockGUI GUI { get; }
        public RecordingGUIStatus Status { get; }

        public List<(int x, int y, bool refresh)> Erased { get; } = new List<(int, int, bool)>();
        public List<(Unit u, string mode)> Painted { get; } = new List<(Unit, string)>();
        public List<Unit> MovedBitmaps { get; } = new List<Unit>();
        public List<string> SysMessages { get; } = new List<string>();
        public List<Unit> DieAnimations { get; } = new List<Unit>();
        public List<(string size, int x, int y)> Explosions { get; } = new List<(string, int, int)>();
        public int OpenMessageFormCount { get; private set; }

        public UnitMapActionFixture(int width = 10, int height = 10)
        {
            GUI = new MockGUI
            {
                MainWidth = width,
                MainHeight = height,
            };
            GUI.EraseUnitBitmapHandler = (x, y, r) => Erased.Add((x, y, r));
            GUI.PaintUnitBitmapHandler = (u, m) => Painted.Add((u, m));
            GUI.MoveUnitBitmap2Handler = (u, w, d) => MovedBitmaps.Add(u);
            GUI.DisplaySysMessageHandler = (m, b) => SysMessages.Add(m);
            GUI.DieAnimationHandler = (u) => DieAnimations.Add(u);
            GUI.ExplodeAnimationHandler = (s, x, y) => Explosions.Add((s, x, y));
            GUI.ClearPictureHandler = () => { };
            GUI.SleepHandler = (ms, e) => { };
            GUI.OpenMessageFormHandler = (u1, u2) => OpenMessageFormCount++;
            GUI.CloseMessageFormHandler = () => { };
            GUI.ClearMessageFormHandler = () => { };
            GUI.UpdateMessageFormHandler = (u1, u2) => { };
            GUI.DrawSysStringHandler = (x, y, s, b) => { };
            GUI.RefreshScreenHandler = (a, b) => { };
            GUI.UpdateScreenHandler = () => { };
            GUI.RedrawScreenHandler = (b) => { };
            GUI.CenterHandler = (x, y) => { };
            GUI.LockGUIHandler = () => { };
            GUI.UnlockGUIHandler = () => { };
            GUI.ResetDrawStringHandler = () => { };

            Src = new SRC { GUI = GUI };
            Status = new RecordingGUIStatus();
            Src.GUIStatus = Status;
            Src.Event.EventData = new List<EventDataLine>();
            Src.Event.EventCmd = new List<CmdData>();
            Src.Event.EventFileNames = new List<string>();
            Src.Event.AdditionalEventFileNames = new List<string>();
            Src.Event.EventQue = new List<string>();
            Src.Titles = new List<string>();

            // マップファイルなし（ステータス表示用）の空マップ
            var map = Src.Map;
            map.MapFileName = "";
            map.MapWidth = width;
            map.MapHeight = height;
            map.MapData = new Src2DArray<MapCell>(width, height);
            for (var x = 1; x <= width; x++)
            {
                for (var y = 1; y <= height; y++)
                {
                    map.MapData[x, y] = MapCell.EmptyCell;
                }
            }
            map.MapDataForUnit = new Src2DArray<Unit>(width, height);
            map.MaskData = new Src2DArray<bool>(width, height);
        }

        public Map Map => Src.Map;

        // マップ上で戦闘中の状態にする（MapFileName が空でない）
        public void SetBattleMap()
        {
            Src.Map.MapFileName = "test.map";
        }

        // 指定地点の地形を設定する
        public void SetTerrain(int x, int y, string cls, int moveCost = 1, string name = null)
        {
            var td = new TerrainData
            {
                Name = name ?? cls,
                Class = cls,
                MoveCost = moveCost,
            };
            Src.Map.MapData[x, y] = new MapCell
            {
                X = x,
                Y = y,
                TerrainType = 0,
                LayerType = MapCell.NO_LAYER_NUM,
                BoxType = BoxTypes.Under,
                UnderTerrain = td,
                UpperTerrain = TerrainData.EmptyTerrain,
            };
        }

        public UnitData AddUnitData(string name, int hp = 1000, int en = 100,
            string transportation = "陸", string adaption = "AAAA", params string[] features)
        {
            var ud = Src.UDList.Add(name);
            ud.Nickname = name;
            ud.Transportation = transportation;
            ud.Adaption = adaption;
            ud.HP = hp;
            ud.EN = en;
            ud.PilotNum = 1;
            ud.Size = "M";
            foreach (var f in features)
            {
                ud.AddFeature(f);
            }
            return ud;
        }

        public PilotData AddPilotData(string name, string personality = "普通", int expValue = 0)
        {
            var pd = Src.PDList.Add(name);
            pd.Nickname = name;
            pd.Personality = personality;
            pd.ExpValue = expValue;
            return pd;
        }

        // ユニットを作成し、パイロットを乗せる（パイロット名を省略した場合は無人）
        public Unit CreateUnit(string uname, string party = "味方", params Pilot[] pilots)
        {
            var u = Src.UList.Add(uname, 0, party);
            foreach (var p in pilots)
            {
                u.AddPilot(p);
                p.Unit = u;
            }
            u.Update();
            return u;
        }

        public Pilot CreatePilot(string pname, int level = 1, string party = "味方")
        {
            if (!Src.PDList.IsDefined(pname))
            {
                AddPilotData(pname);
            }
            return Src.PList.Add(pname, level, party);
        }

        // ユニットを指定地点に直接出撃させる
        public Unit Deploy(Unit u, int x, int y)
        {
            u.StandBy(x, y);
            return u;
        }
    }
}
