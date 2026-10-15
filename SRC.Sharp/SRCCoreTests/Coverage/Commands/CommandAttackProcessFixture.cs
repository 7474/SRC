using SRCCore.Commands;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Coverage.Tests.CommandsCoverage
{
    /// <summary>
    /// Command.attack.cs / Command.process.cs のテスト用の共通環境。
    /// HeadlessGUI で GUI 呼び出しを記録し、地形付きのマップにユニットを配置する。
    /// </summary>
    internal sealed class CommandAttackProcessFixture
    {
        public SRC SRC { get; }
        public HeadlessGUI Headless { get; }
        public Command Commands => SRC.Commands;

        // 平地（陸）
        public static TerrainData Plain() => new TerrainData
        {
            ID = 1,
            Name = "平地",
            Class = "陸",
            MoveCost = 2,
        };

        public static TerrainData Water() => new TerrainData
        {
            ID = 2,
            Name = "海",
            Class = "水",
            MoveCost = 2,
        };

        public CommandAttackProcessFixture(int width = 10, int height = 10, string mapFileName = "test.map")
        {
            SRC = new SRC();
            // 効果音ファイルの探索でファイルシステムを参照する（書き込みは行わない）
            SRC.FileSystem = new Filesystem.LocalFileSystem();
            Headless = new HeadlessGUI();
            Headless.Install(SRC);
            SRC.GUI.MainWidth = 15;
            SRC.GUI.MainHeight = 15;
            // イベントデータは空（HandleEvent が動く最低限の初期化）
            SRC.Event.EventData = new List<Events.EventDataLine>();
            SRC.Event.EventCmd = new List<CmdDatas.CmdData>();
            SRC.Event.EventFileNames = new List<string>();
            SRC.Event.AdditionalEventFileNames = new List<string>();
            SRC.Event.EventQue = new List<string>();
            SRC.Map.SetMapSize(width, height);
            SRC.Map.MapFileName = mapFileName;
            for (var x = 1; x <= width; x++)
            {
                for (var y = 1; y <= height; y++)
                {
                    SetTerrain(x, y, Plain());
                }
            }
        }

        public void SetTerrain(int x, int y, TerrainData td)
        {
            var cell = SRC.Map.MapData[x, y];
            cell.UnderTerrain = td;
            cell.UpperTerrain = td;
            cell.TerrainType = td.ID;
            cell.BoxType = Maps.BoxTypes.Under;
        }

        public UnitData AddUnitData(string name, int speed = 4, params string[] features)
        {
            var ud = SRC.UDList.Add(name);
            ud.Nickname = name;
            ud.KanaName = name;
            ud.Class = "汎用";
            ud.PilotNum = 1;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.Speed = speed;
            ud.HP = 1000;
            ud.EN = 100;
            ud.Armor = 500;
            ud.Mobility = 50;
            ud.Size = "M";
            foreach (var f in features)
            {
                ud.AddFeature(f);
            }
            return ud;
        }

        public WeaponData AddWeapon(UnitData ud, string name, int power = 1000, int minRange = 1, int maxRange = 1, string wclass = "", string adaption = "AAAA")
        {
            var wd = ud.AddWeapon(name);
            wd.Power = power;
            wd.MinRange = minRange;
            wd.MaxRange = maxRange;
            wd.Precision = 0;
            wd.Bullet = 0;
            wd.ENConsumption = 0;
            wd.NecessaryMorale = 0;
            wd.Adaption = adaption;
            wd.Critical = 0;
            wd.Class = wclass;
            wd.NecessarySkill = "";
            wd.NecessaryCondition = "";
            return wd;
        }

        public PilotData AddPilotData(string name)
        {
            var pd = SRC.PDList.Add(name);
            pd.Nickname = name;
            pd.KanaName = name;
            pd.Sex = "男性";
            pd.Class = "汎用";
            pd.Adaption = "AAAA";
            pd.ExpValue = 100;
            pd.Infight = 100;
            pd.Shooting = 100;
            pd.Hit = 100;
            pd.Dodge = 100;
            pd.Intuition = 100;
            pd.Technique = 100;
            pd.Personality = "普通";
            pd.SP = 0;
            pd.BGM = "";
            return pd;
        }

        /// <summary>
        /// ユニットとパイロットを作成して (x, y) に出撃させる。
        /// </summary>
        public Unit Deploy(string unitName, string pilotName, string party, int x, int y)
        {
            if (!SRC.PDList.IsDefined(pilotName))
            {
                AddPilotData(pilotName);
            }
            var p = SRC.PList.Add(pilotName, 1, party);
            var u = SRC.UList.Add(unitName, 0, party);
            p.Ride(u);
            u.FullRecover();
            u.StandBy(x, y);
            return u;
        }

        /// <summary>
        /// ShowUnitCommandMenu に渡されたコマンド一覧（最後の呼び出し）。
        /// </summary>
        public IList<UiCommand> LastUnitCommandMenu()
        {
            var call = Headless.CallsOf("ShowUnitCommandMenu").LastOrDefault();
            return (IList<UiCommand>)call?.Args[0];
        }

        public IList<UiCommand> LastMapCommandMenu()
        {
            var call = Headless.CallsOf("ShowMapCommandMenu").LastOrDefault();
            return (IList<UiCommand>)call?.Args[0];
        }

        public static List<int> Ids(IEnumerable<UiCommand> commands) => commands.Select(x => x.Id).ToList();

        public static string Label(IEnumerable<UiCommand> commands, int id) => commands.FirstOrDefault(x => x.Id == id)?.Label;

        /// <summary>
        /// ラベルを直接登録する（イベントファイルを読み込まずにラベルだけ定義する）。
        /// </summary>
        public void AddEventLabel(string label, int eventDataId)
        {
            // イベントファイル読み込み時と同様に末尾の「:」は含めない
            SRC.Event.AddLabel(label.TrimEnd(':'), eventDataId);
        }
    }
}
