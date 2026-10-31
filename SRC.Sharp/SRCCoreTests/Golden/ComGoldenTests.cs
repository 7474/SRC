using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Filesystem;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SRCCore.Golden.Tests
{
    /// <summary>
    /// 敵の思考（COM.OperateUnit）の行動を、乱数を固定して記録し、固定する（#1356）。
    /// 敵 5 体・味方 3 体の戦場で、敵を決まった順に 5 回ずつ動かし、行動のたびに全ユニットの位置・HP・EN・気力を記録して
    /// Golden/Com/expected.txt と比べる。移動先・武器・対象の選び方や、命中・ダメージの判定が変わると記録が変わる。
    /// 意図して変えたときは、SRC_GOLDEN_UPDATE=1 で作り直し、差分を PR に載せる（docs/testing.md）。
    /// データは BattleGoldenTests と同じ Golden/Battle/Data を使う。
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class ComGoldenTests
    {
        private const int Seed = 20261031;
        private const int Rounds = 5;

        // 敵（ユニット・パイロット・位置）。この順に動かす
        private static readonly (string Unit, string Pilot, int X, int Y)[] Enemies =
        {
            ("基準機", "エースパイロット", 12, 2),
            ("重装甲機", "防御側パイロット", 13, 4),
            ("高機動機", "敵パイロットA", 12, 6),
            ("空戦機", "敵パイロットB", 14, 3),
            ("巨大機", "敵パイロットC", 14, 5),
        };

        private static readonly (string Unit, string Pilot, int X, int Y)[] Allies =
        {
            ("基準機", "標準パイロット", 3, 2),
            ("重装甲機", "味方パイロットB", 2, 4),
            ("高機動機", "味方パイロットC", 3, 6),
        };

        private static HeadlessGUI LastGui;

        [TestMethod]
        public void EnemyActions_MatchGolden()
        {
            // 入力を待つ画面などで止まったときは、最後の GUI の呼び出しを出して落とす（CI を止めないため）
            var task = System.Threading.Tasks.Task.Run(() => Render());
            if (!task.Wait(TimeSpan.FromSeconds(60)))
            {
                Assert.Fail("敵の行動が 60 秒で終わらない。最後の GUI の呼び出し:\n" + string.Join("\n", LastGui.Calls.Skip(Math.Max(0, LastGui.Calls.Count - 40)).Select(c => c.ToString())));
            }
            var actual = task.Result;
            var expectedPath = Path.Combine(GoldenDir("Com"), "expected.txt");
            if (Environment.GetEnvironmentVariable("SRC_GOLDEN_UPDATE") == "1")
            {
                File.WriteAllText(expectedPath, actual, new UTF8Encoding(false));
                Assert.Inconclusive("expected.txt を作り直した（SRC_GOLDEN_UPDATE=1）");
            }
            Assert.IsTrue(File.Exists(expectedPath), "expected.txt が無い。SRC_GOLDEN_UPDATE=1 dotnet test --filter ComGoldenTests で作る");
            var expected = File.ReadAllText(expectedPath).Replace("\r\n", "\n");
            if (expected == actual)
            {
                return;
            }
            var expectedLines = expected.Split('\n');
            var actualLines = actual.Split('\n');
            var diffs = new List<string>();
            for (var i = 0; i < Math.Max(expectedLines.Length, actualLines.Length) && diffs.Count < 20; i++)
            {
                var e = i < expectedLines.Length ? expectedLines[i] : "(無し)";
                var a = i < actualLines.Length ? actualLines[i] : "(無し)";
                if (e != a)
                {
                    diffs.Add($"- {e}\n+ {a}");
                }
            }
            Assert.Fail("敵の行動が変わった。意図した変更なら SRC_GOLDEN_UPDATE=1 で作り直して差分を PR に載せる\n" + string.Join("\n", diffs));
        }

        private static string Render()
        {
            var src = CreateField(16, 8);
            GeneralLib.RndSeed = Seed;
            GeneralLib.RndReset();

            var enemies = Enemies.Select(e => Deploy(src, e.Unit, e.Pilot, "敵", e.X, e.Y)).ToList();
            var allies = Allies.Select(a => Deploy(src, a.Unit, a.Pilot, "味方", a.X, a.Y)).ToList();
            var all = enemies.Concat(allies).ToList();
            string Name(Unit u) => $"{u.Party}:{u.Name}({u.MainPilot().Name})";

            var sb = new StringBuilder();
            sb.Append("# 敵の行動の記録（ComGoldenTests が作る。手で直さない）\n");
            sb.Append("# 行動のあとの全ユニットの 位置 HP EN 気力。乱数の種 " + Seed + "\n");
            sb.Append("初め: " + string.Join(" / ", all.Select(u => State(u, Name(u)))) + "\n");
            src.Stage = "敵";
            for (var round = 1; round <= Rounds; round++)
            {
                foreach (var u in enemies)
                {
                    if (u.Status != "出撃")
                    {
                        continue;
                    }
                    src.Commands.SelectedUnit = u;
                    src.COM.OperateUnit();
                    sb.Append($"{round} 回目 {Name(u)}: " + string.Join(" / ", all.Select(x => State(x, Name(x)))) + "\n");
                }
                // 次の回に動けるよう、行動済みを戻す
                foreach (var u in all.Where(x => x.Status == "出撃"))
                {
                    u.UsedAction = 0;
                }
            }
            return sb.ToString();
        }

        private static string State(Unit u, string name)
        {
            return u.Status == "出撃"
                ? $"{name} ({u.x},{u.y}) {u.HP} {u.EN} {u.MainPilot().Morale}"
                : $"{name} {u.Status}";
        }

        private static SRC CreateField(int width, int height)
        {
            var gui = new HeadlessGUI();
            LastGui = gui;
            // 戦闘アニメは使わない（汎用戦闘アニメの Lib を読まない）。味方の反撃・防御は自動反撃モードで自動で選ぶ
            var src = new SRC { SystemConfig = new SRCCore.Config.LocalFileConfig { BattleAnimation = false, WeaponAnimation = false, ExtendedAnimation = false, AutoDefense = true } };
            gui.Install(src);
            // 戦闘のメッセージの表示時間の待ちを無くす
            src.GUI.MessageWait = 0;
            var fs = new LocalFileSystem();
            fs.AddPath(GoldenDir("Battle"));
            src.FileSystem = fs;
            // イベントは無い（戦闘の前後のイベントの呼び出しが空振りするだけ）
            src.Event.InitEventData();
            src.SPDList.Load(Path.Combine(BasicDataSystemDir(), "sp.txt"));
            src.LoadDataDirectory(Path.Combine(GoldenDir("Battle"), "Data"));

            var plain = new TerrainData { ID = 0, Name = "平地", Bitmap = "plain", Class = "陸", MoveCost = 2 };
            var woods = new TerrainData { ID = 11, Name = "林", Bitmap = "woods", Class = "陸", MoveCost = 4, HitMod = 15, DamageMod = 10 };
            var map = src.Map;
            map.MapFileName = "";
            map.MapWidth = width;
            map.MapHeight = height;
            map.MapData = new Src2DArray<MapCell>(width, height);
            for (var x = 1; x <= width; x++)
            {
                for (var y = 1; y <= height; y++)
                {
                    // 真ん中の列に林を置き、地形を考えた移動先の選び方も記録に出す
                    var t = x >= 7 && x <= 9 && y % 2 == 0 ? woods : plain;
                    map.MapData[x, y] = new MapCell { X = x, Y = y, UnderTerrain = t, UpperTerrain = t };
                }
            }
            map.MapDataForUnit = new Src2DArray<Unit>(width, height);
            map.MaskData = new Src2DArray<bool>(width, height);
            map.TotalMoveCost = new int[width + 2, height + 2];
            map.PointInZOC = new int[width + 2, height + 2];
            return src;
        }

        private static Unit Deploy(SRC src, string unitName, string pilotName, string party, int x, int y)
        {
            var u = src.UList.Add(unitName, 0, party);
            var p = src.PList.Add(pilotName, 30, party);
            p.Ride(u);
            u.FullRecover();
            u.StandBy(x, y);
            return u;
        }

        private static string GoldenDir(string name, [CallerFilePath] string path = "")
        {
            return Path.Combine(Path.GetDirectoryName(path), name);
        }

        private static string BasicDataSystemDir([CallerFilePath] string path = "")
        {
            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), "..", "..", "Content", "basic-data", "Data", "System"));
        }
    }
}
