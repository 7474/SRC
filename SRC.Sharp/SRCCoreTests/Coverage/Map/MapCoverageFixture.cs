using SRCCore.Lib;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Map のテスト用に、地形データ・マップファイル・ユニットデータを文字列から組み立てる補助。
    /// </summary>
    internal sealed class MapCoverageFixture
    {
        public sealed class StubGUIMap : IGUIMap
        {
            public List<(int w, int h)> SetMapSizeCalls { get; } = new List<(int w, int h)>();
            public void InitMapSize(int w, int h) { }
            public void SetMapSize(int w, int h) { SetMapSizeCalls.Add((w, h)); }
        }

        // 地形番号（移動コストは実際の値。内部では２倍で記録される）
        public const int Plain = 0;      // 平地 陸 1
        public const int Mountain = 1;   // 山 陸 3
        public const int Sea = 2;        // 海 水 2
        public const int DeepSea = 3;    // 深海 深水 2
        public const int Wall = 4;       // 壁 陸 -
        public const int Bulwark = 5;    // 防壁 陸 1
        public const int Swamp = 6;      // 沼 陸 1 移動停止
        public const int Sky = 7;        // 空 空 1

        public const string TerrainText =
            "0\r\n平地, plain\r\n陸, 1, 0, 0\r\n\r\n" +
            "1\r\n山, mountain\r\n陸, 3, 30, 30\r\n\r\n" +
            "2\r\n海, sea\r\n水, 2, 5, 5\r\n\r\n" +
            "3\r\n深海, deep\r\n深水, 2, 0, 0\r\n\r\n" +
            "4\r\n壁, wall\r\n陸, -, 0, 0\r\n\r\n" +
            "5\r\n防壁, bulwark\r\n陸, 1, 0, 0\r\n\r\n" +
            "6\r\n沼, swamp\r\n陸, 1, 0, 0\r\n移動停止\r\n\r\n" +
            "7\r\n空, sky\r\n空, 1, 0, 0\r\n\r\n";

        public static readonly IReadOnlyDictionary<char, int> Legend = new Dictionary<char, int>
        {
            ['.'] = Plain,
            ['M'] = Mountain,
            ['W'] = Sea,
            ['D'] = DeepSea,
            ['#'] = Wall,
            ['B'] = Bulwark,
            ['S'] = Swamp,
            ['K'] = Sky,
        };

        public SRC Src { get; }
        public MockGUI GUI { get; }
        public StubGUIMap GUIMap { get; }
        public SRCCore.Maps.Map Map => Src.Map;

        public MapCoverageFixture()
        {
            GUI = new MockGUI { MainWidth = 15, MainHeight = 15 };
            GUI.RedrawScreenHandler = _ => { };
            GUIMap = new StubGUIMap();
            Src = new SRC { GUI = GUI, GUIMap = GUIMap };
            Src.TDList.Load("terrain.txt", ToStream(TerrainText));
        }

        public static Stream ToStream(string text)
        {
            return new MemoryStream(new UTF8Encoding(false).GetBytes(text));
        }

        /// <summary>
        /// rows[y-1][x-1] の文字を地形に対応させた新形式のマップファイル文字列を作る。
        /// </summary>
        public static string BuildMapText(params string[] rows)
        {
            var h = rows.Length;
            var w = rows[0].Length;
            var sb = new StringBuilder();
            sb.Append("\"MapData\"\r\n\"\"\r\n");
            sb.Append(w + "," + h + "\r\n");
            for (var x = 1; x <= w; x++)
            {
                for (var y = 1; y <= h; y++)
                {
                    sb.Append(Legend[rows[y - 1][x - 1]] + ",0\r\n");
                }
            }
            return sb.ToString();
        }

        public void LoadGrid(params string[] rows)
        {
            Map.LoadMapData("test.map", ToStream(BuildMapText(rows)));
        }

        /// <summary>
        /// 移動タイプ・移動力・特殊能力を指定してユニットデータを定義する。
        /// </summary>
        public void DefineUnit(string name, string trans, int speed, string adaption = "AA-A", params string[] features)
        {
            var sb = new StringBuilder();
            sb.Append(name + "\r\n");
            sb.Append(name + ", てすと, テスト, 1, 1\r\n");
            sb.Append(trans + ", " + speed + ", M, 100, 10\r\n");
            if (features.Any())
            {
                sb.Append("特殊能力\r\n");
                foreach (var f in features)
                {
                    sb.Append(f + "\r\n");
                }
            }
            else
            {
                sb.Append("特殊能力なし\r\n");
            }
            sb.Append("1000, 100, 100, 100\r\n");
            sb.Append(adaption + ", dummy.bmp\r\n\r\n");
            Src.UDList.Load(name + ".txt", ToStream(sb.ToString()));
        }

        public Unit Place(string name, string party, int x, int y, string area = "地上")
        {
            var u = Src.UList.Add(name, 0, party);
            u.Update();
            u.x = x;
            u.y = y;
            u.Area = area;
            u.Status = "出撃";
            Map.MapDataForUnit[x, y] = u;
            return u;
        }

        /// <summary>
        /// マスクが外れている（選択可能な）座標を "x,y" の集合で返す。
        /// </summary>
        public ISet<string> Unmasked()
        {
            var set = new HashSet<string>();
            for (var x = 1; x <= Map.MapWidth; x++)
            {
                for (var y = 1; y <= Map.MapHeight; y++)
                {
                    if (!Map.MaskData[x, y])
                    {
                        set.Add(x + "," + y);
                    }
                }
            }
            return set;
        }

        public static ISet<string> Points(params string[] points)
        {
            return new HashSet<string>(points);
        }
    }
}
