using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.Filesystem;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// CommandUnitOps 系テスト用の共通準備。
    /// HeadlessGUI で GUI 呼び出しを記録し、平地だけの小さなマップにユニットを直接配置する。
    /// </summary>
    internal sealed class CommandUnitOpsFixture
    {
        public SRC Src { get; }
        public HeadlessGUI Gui { get; }
        public TerrainData Plain { get; }

        public CommandUnitOpsFixture(int width = 10, int height = 10)
        {
            Src = new SRC();
            Gui = new HeadlessGUI();
            Gui.Install(Src);
            // 効果音の検索で参照されるため（探索パスは追加しない）
            Src.FileSystem = new LocalFileSystem();

            // イベントデータは空（どのラベルも定義されていない）
            Src.Event.EventData = new List<EventDataLine>();
            Src.Event.EventCmd = new List<CmdData>();
            Src.Event.EventFileNames = new List<string>();
            Src.Event.AdditionalEventFileNames = new List<string>();
            Src.Event.EventQue = new List<string>();
            Src.Titles = new List<string>();

            // 移動コスト 1 の平地だけのマップ
            Plain = new TerrainData
            {
                ID = 0,
                Name = "平地",
                Class = "陸",
                MoveCost = 2,
            };
            Src.Map.SetMapSize(width, height);
            foreach (var x in Src.Map.MapData.XRange)
            {
                foreach (var y in Src.Map.MapData.YRange)
                {
                    var cell = Src.Map.MapData[x, y];
                    cell.TerrainType = 0;
                    cell.UnderTerrain = Plain;
                    cell.UpperTerrain = Plain;
                }
            }
        }

        public SRCCore.Commands.Command Commands => Src.Commands;

        public UnitData DefineUnit(string name, int hp = 1000, int en = 100, int speed = 5, params string[] features)
        {
            var ud = Src.UDList.Add(name);
            ud.Nickname = name;
            ud.KanaName = name;
            ud.Class = "汎用";
            ud.PilotNum = 1;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.HP = hp;
            ud.EN = en;
            ud.Speed = speed;
            foreach (var f in features)
            {
                ud.AddFeature(f);
            }
            return ud;
        }

        public PilotData DefinePilot(string name, int sp = 0)
        {
            var pd = Src.PDList.Add(name);
            pd.Nickname = name;
            pd.KanaName = name;
            pd.SP = sp;
            pd.Adaption = "AAAA";
            return pd;
        }

        public SpecialPowerData DefineSpecialPower(string name, string shortName, int cost, string target, string effect, string kana = null)
        {
            var spd = Src.SPDList.Add(name);
            spd.KanaName = kana ?? name;
            spd.ShortName = shortName;
            spd.SPConsumption = cost;
            spd.TargetType = target;
            spd.Duration = "即効";
            spd.NecessaryCondition = "";
            spd.Animation = "";
            spd.Comment = name + "の解説";
            spd.SetEffect(effect);
            return spd;
        }

        /// <summary>
        /// ユニットを作成してパイロットを乗せ、マップの (x, y) に出撃させる。
        /// </summary>
        public Unit Place(string unitName, string pilotName, string party, int x, int y, int level = 1)
        {
            var u = Src.UList.Add(unitName, 0, party);
            if (!string.IsNullOrEmpty(pilotName))
            {
                if (!Src.PDList.IsDefined(pilotName))
                {
                    DefinePilot(pilotName);
                }
                var p = Src.PList.Add(pilotName, level, party);
                p.Ride(u);
            }
            u.Update();
            u.FullRecover();
            u.x = x;
            u.y = y;
            u.Status = "出撃";
            u.Area = "地上";
            u.UsedAction = 0;
            Src.Map.MapDataForUnit[x, y] = u;
            return u;
        }

        public MapCell Cell(int x, int y) => Src.Map.MapData[x, y];

        public IEnumerable<HeadlessGUICall> Calls(string member) => Gui.CallsOf(member);

        public bool Called(string member) => Gui.CallsOf(member).Any();

        public void ClearMask(bool value = false)
        {
            foreach (var x in Src.Map.MaskData.XRange)
            {
                foreach (var y in Src.Map.MaskData.YRange)
                {
                    Src.Map.MaskData[x, y] = value;
                }
            }
        }
    }
}
