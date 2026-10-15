using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.TestLib;
using SRCCore.Units;
using System.IO;
using System.Text;

namespace SRCCore.Coverage.Tests.Units
{
    /// <summary>
    /// Unit.lookup.cs のテスト用に、テキストのデータ定義からユニットとパイロットを作り、
    /// 小さなマップに配置するための補助。
    /// </summary>
    internal sealed class UnitLookupTestHelper
    {
        public SRC Src { get; }

        public UnitLookupTestHelper(int width = 7, int height = 7)
        {
            Src = new SRC { GUI = new MockGUI() };
            var map = Src.Map;
            map.MapWidth = width;
            map.MapHeight = height;
            map.MapDataForUnit = new Src2DArray<Unit>(width, height);
            map.MaskData = new Src2DArray<bool>(width, height);
            map.MapData = new Src2DArray<MapCell>(width, height);
            var plain = new TerrainData { ID = 0, Name = "平地", Class = "陸", MoveCost = 2 };
            for (var x = 1; x <= width; x++)
            {
                for (var y = 1; y <= height; y++)
                {
                    map.MapData[x, y] = new MapCell { X = x, Y = y, UnderTerrain = plain };
                }
            }
        }

        private static Stream ToStream(string text) => new MemoryStream(Encoding.UTF8.GetBytes(text));

        public void LoadPilots(string text) => Src.PDList.Load("pilot.txt", ToStream(text));
        public void LoadUnits(string text) => Src.UDList.Load("robot.txt", ToStream(text));
        public void LoadMessages(string text) => Src.MDList.Load("message.txt", false, ToStream(text));

        /// <summary>
        /// 特殊能力行・能力値などを指定してパイロットデータを1件定義する。
        /// </summary>
        public void DefinePilot(string name, string sex = "男性", string skills = null,
            string stats = "150, 150, 150, 150, 150, 150, 普通", string pclass = "汎用")
        {
            var sb = new StringBuilder();
            sb.AppendLine(name);
            sb.AppendLine($"{name}, {name}, {sex}, {pclass}, AAAA, 100");
            if (string.IsNullOrEmpty(skills))
            {
                sb.AppendLine("特殊能力なし");
            }
            else
            {
                sb.AppendLine("特殊能力");
                sb.AppendLine(skills);
            }
            sb.AppendLine(stats);
            sb.AppendLine("ＳＰ, 10, 根性, 1");
            sb.AppendLine("-.bmp, -.mid");
            sb.AppendLine();
            LoadPilots(sb.ToString());
        }

        /// <summary>
        /// 特殊能力・武器を指定してユニットデータを1件定義する。
        /// </summary>
        public void DefineUnit(string name, string features = null, string weapons = null,
            string uclass = "汎用", string trans = "陸", string adaption = "AAAA",
            string stats = "3000, 200, 1000, 100", string abilities = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine(name);
            sb.AppendLine($"{name}, {name}, {uclass}, 1, 2");
            sb.AppendLine($"{trans}, 4, M, 1000, 100");
            if (string.IsNullOrEmpty(features))
            {
                sb.AppendLine("特殊能力なし");
            }
            else
            {
                sb.AppendLine("特殊能力");
                sb.AppendLine(features);
            }
            sb.AppendLine(stats);
            sb.AppendLine($"{adaption}, -.bmp");
            if (!string.IsNullOrEmpty(weapons))
            {
                sb.AppendLine(weapons);
            }
            if (!string.IsNullOrEmpty(abilities))
            {
                sb.AppendLine("===");
                sb.AppendLine(abilities);
            }
            sb.AppendLine();
            LoadUnits(sb.ToString());
        }

        /// <summary>
        /// ユニットを作成してパイロットを乗せる（マップには置かない）。
        /// </summary>
        public Unit Create(string uname, string pname, string party = "味方", int level = 1, int rank = 0)
        {
            var u = Src.UList.Add(uname, rank, party);
            if (pname != null)
            {
                var p = Src.PList.Add(pname, level, party);
                p.Ride(u);
            }
            u.Update();
            u.FullRecover();
            return u;
        }

        /// <summary>
        /// ユニットを作成してマップの (x, y) に出撃させる。
        /// </summary>
        public Unit Place(string uname, string pname, int x, int y, string party = "味方", string area = "地上", int level = 1, int rank = 0)
        {
            var u = Create(uname, pname, party, level, rank);
            PutOnMap(u, x, y, area);
            return u;
        }

        public void PutOnMap(Unit u, int x, int y, string area = "地上")
        {
            u.Status = "出撃";
            u.x = x;
            u.y = y;
            u.Area = area;
            Src.Map.MapDataForUnit[x, y] = u;
        }

        public void SetTerrain(int x, int y, string name, string tclass, int moveCost = 2)
        {
            Src.Map.MapData[x, y].UnderTerrain = new TerrainData { ID = 1, Name = name, Class = tclass, MoveCost = moveCost };
        }

        public void SetRelation(Pilot a, Pilot b, int value)
        {
            Src.Expression.SetVariableAsLong("関係:" + a.Name + ":" + b.Name, value);
        }
    }
}
