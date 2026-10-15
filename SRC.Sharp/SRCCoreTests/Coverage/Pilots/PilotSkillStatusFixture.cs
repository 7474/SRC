using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.Expressions;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Collections.Generic;

namespace SRCCore.Coverage.Tests.PilotsCoverage
{
    /// <summary>
    /// PilotSkillStatus 系テストで共通に使う SRC / パイロット / ユニットの組み立て
    /// </summary>
    internal static class PilotSkillStatusFixture
    {
        public static SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            src.Titles = new List<string>();
            return src;
        }

        // 能力値をすべて 100、地形適応 BBBB のパイロットデータを作る
        public static PilotData AddPilotData(SRC src, string name)
        {
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            pd.Infight = 100;
            pd.Shooting = 100;
            pd.Hit = 100;
            pd.Dodge = 100;
            pd.Technique = 100;
            pd.Intuition = 100;
            pd.Adaption = "BBBB";
            return pd;
        }

        public static Pilot AddPilot(SRC src, string name, int level = 1, string party = "味方")
        {
            return src.PList.Add(name, level, party);
        }

        public static UnitData AddUnitData(SRC src, string name, params string[] features)
        {
            var ud = src.UDList.Add(name);
            ud.HP = 1000;
            ud.EN = 100;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.PilotNum = 2;
            foreach (var f in features)
            {
                ud.AddFeature(f);
            }
            return ud;
        }

        // ユニットを作り、指定したパイロットを順に搭乗させる（先頭がメインパイロット）
        public static Unit CreateUnit(SRC src, string uname, string party, params Pilot[] pilots)
        {
            var unit = src.UList.Add(uname, 0, party);
            foreach (var p in pilots)
            {
                unit.AddPilot(p);
                p.Unit = unit;
            }
            unit.Update();
            return unit;
        }

        // オプション（Option(xxx) 変数）を定義する
        public static void DefineOption(SRC src, string oname)
        {
            src.Expression.DefineGlobalVariable("Option(" + oname + ")");
            src.Expression.SetVariableAsLong("Option(" + oname + ")", 1);
        }

        // 名前に括弧やカンマを含むグローバル変数を（式評価を通さず）直接定義する
        public static void SetGlobalString(SRC src, string vname, string value)
        {
            if (src.Event.GlobalVariableList.ContainsKey(vname))
            {
                src.Event.GlobalVariableList.Remove(vname);
            }
            src.Event.GlobalVariableList.Add(vname, new VarData(vname, ValueType.StringType, value, 0d));
        }

        // SetSkill コマンド相当: Ability(ID) と Ability(ID,能力名) を設定する
        public static void SetSkill(SRC src, Pilot p, string sname, string levelAndData)
        {
            var listName = "Ability(" + p.ID + ")";
            var list = src.Event.GlobalVariableList.ContainsKey(listName)
                ? src.Event.GlobalVariableList[listName].StringValue + " " + sname
                : sname;
            SetGlobalString(src, listName, list);
            SetGlobalString(src, "Ability(" + p.ID + "," + sname + ")", levelAndData);
        }

        // マップ上に出撃しているマップを用意する（MapFileName を設定して支援効果を有効にする）
        public static void SetupMap(SRC src, int w = 10, int h = 10)
        {
            var map = src.Map;
            map.MapFileName = "test.map";
            map.MapWidth = w;
            map.MapHeight = h;
            map.MapData = new Src2DArray<MapCell>(w, h);
            for (var x = 1; x <= w; x++)
            {
                for (var y = 1; y <= h; y++)
                {
                    map.MapData[x, y] = MapCell.EmptyCell;
                }
            }
            map.MapDataForUnit = new Src2DArray<Unit>(w, h);
        }

        // ユニットを出撃状態でマップに配置する
        public static void Place(SRC src, Unit u, int x, int y)
        {
            u.x = x;
            u.y = y;
            u.Status = "出撃";
            src.Map.MapDataForUnit[x, y] = u;
        }
    }
}
