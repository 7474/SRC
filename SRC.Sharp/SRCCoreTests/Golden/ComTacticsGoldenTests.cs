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
    /// 敵の思考（COM.OperateUnit）のうち、ComGoldenTests の戦場では通らない行動を固定する（#1396）。
    /// 回復・補給のアビリティを持つ敵と傷ついた仲間、まとまって並ぶ味方とマップ兵器、変形できる敵、
    /// スペシャルパワーを使う敵（オプション「敵ユニットスペシャルパワー使用」）を出す。
    /// 乱数を固定して敵を決まった順に 5 回ずつ動かし、行動ごとに出たメッセージ（何を使ったか・誰に）と、
    /// 全ユニットの形態・位置・HP・EN・気力・SP・効いているスペシャルパワーを Golden/ComTactics/expected.txt と比べる。
    /// 意図して変えたときは、SRC_GOLDEN_UPDATE=1 で作り直し、差分を PR に載せる（docs/testing.md）。
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class ComTacticsGoldenTests
    {
        private const int Seed = 20261101;
        private const int Rounds = 5;

        // 敵（ユニット・パイロット・位置・初めの HP）。この順に動かす。HP が 0 なら満タン
        private static readonly (string Unit, string Pilot, int X, int Y, int HP)[] Enemies =
        {
            // 傷ついた仲間を囲んで、マップ型の回復アビリティを使う
            ("救護機", "救護パイロット", 10, 5, 0),
            // 傷ついている。仲間を回復アビリティで直す
            ("修理機", "修理パイロット", 11, 4, 2000),
            // 傷ついている。まとまった味方に投下型のマップ兵器を落とす
            ("爆撃機", "爆撃パイロット", 9, 6, 3000),
            // 傷ついている。並んだ味方に直線のマップ兵器を撃つ
            ("砲撃機", "砲撃パイロット", 9, 4, 2500),
            // 敵から遠いので、移動力の高い形態に変形して近づく
            ("変形機Ａ", "変形パイロット", 15, 9, 0),
            // スペシャルパワーを多く持つ。HP が減っている
            ("指揮官機", "指揮官パイロット", 13, 2, 3500),
        };

        // 味方。マップ兵器が何体も巻き込めるよう、まとめて置く
        private static readonly (string Unit, string Pilot, int X, int Y)[] Allies =
        {
            ("防衛機", "味方パイロット甲", 4, 4),
            ("防衛機", "味方パイロット乙", 5, 4),
            ("防衛機", "味方パイロット丙", 4, 5),
        };

        // 行動の中身として記録する GUI の呼び出し
        private static readonly string[] MessageMembers = { "DisplaySysMessage", "DisplayMessage", "DisplayBattleMessage" };

        private static HeadlessGUI LastGui;

        [TestMethod]
        public void EnemyTactics_MatchGolden()
        {
            // 入力を待つ画面などで止まったときは、最後の GUI の呼び出しを出して落とす（CI を止めないため）
            var task = System.Threading.Tasks.Task.Run(() => Render());
            if (!task.Wait(TimeSpan.FromSeconds(60)))
            {
                Assert.Fail("敵の行動が 60 秒で終わらない。最後の GUI の呼び出し:\n" + string.Join("\n", LastGui.Calls.Skip(Math.Max(0, LastGui.Calls.Count - 40)).Select(c => c.ToString())));
            }
            GoldenFile.AssertMatch(task.Result, Path.Combine(GoldenDir("ComTactics"), "expected.txt"), "ComTacticsGoldenTests", "敵の行動");
        }

        private static string Render()
        {
            var src = CreateField(16, 10, out var gui);
            // 敵もスペシャルパワーを使う
            src.Expression.DefineGlobalVariable("Option(敵ユニットスペシャルパワー使用)");
            GeneralLib.RndSeed = Seed;
            GeneralLib.RndReset();

            var enemies = Enemies.Select(e => Deploy(src, e.Unit, e.Pilot, "敵", e.X, e.Y, e.HP)).ToList();
            var allies = Allies.Select(a => Deploy(src, a.Unit, a.Pilot, "味方", a.X, a.Y, 0)).ToList();
            var all = enemies.Concat(allies).ToList();

            var sb = new StringBuilder();
            sb.Append("# 敵の行動の記録（ComTacticsGoldenTests が作る。手で直さない）\n");
            sb.Append("# 行動ごとに、出たメッセージ（> の行）と、全ユニットの 形態 位置 HP EN 気力 SP [効いているスペシャルパワー]。乱数の種 " + Seed + "\n");
            sb.Append("初め:\n");
            AppendStates(sb, all);
            src.Stage = "敵";
            for (var round = 1; round <= Rounds; round++)
            {
                // ターン数で使うかが変わるスペシャルパワーがある
                src.Turn = round;
                foreach (var e in enemies)
                {
                    var u = e.CurrentForm();
                    if (u.Status != "出撃")
                    {
                        continue;
                    }
                    var before = gui.Calls.Count;
                    var name = Name(u);
                    src.Commands.SelectedUnit = u;
                    src.COM.OperateUnit();
                    sb.Append($"{round} 回目 {name}:\n");
                    foreach (var c in gui.Calls.Skip(before).Where(c => MessageMembers.Contains(c.Member)))
                    {
                        var text = c.Member == "DisplaySysMessage" ? (string)c.Args[0] : c.Args[0] + "「" + c.Args[1] + "」";
                        sb.Append("> " + text.Replace("\n", " ") + "\n");
                    }
                    AppendStates(sb, all);
                }
                // 次の回に動けるよう、行動済みを戻す
                foreach (var u in all.Select(x => x.CurrentForm()).Where(x => x.Status == "出撃"))
                {
                    u.UsedAction = 0;
                }
            }
            return sb.ToString();
        }

        private static string Name(Unit u)
        {
            return $"{u.Party}:{u.Name}({u.MainPilot().Name})";
        }

        private static void AppendStates(StringBuilder sb, IEnumerable<Unit> units)
        {
            foreach (var u0 in units)
            {
                var u = u0.CurrentForm();
                if (u.Status != "出撃")
                {
                    sb.Append($"  {u.Party}:{u.Name} {u.Status}\n");
                    continue;
                }
                var p = u.MainPilot();
                var sps = u.SpecialPowerInEffect();
                sb.Append($"  {Name(u)} ({u.x},{u.y}) {u.HP} {u.EN} {p.Morale} {p.SP} [{sps}]\n");
            }
        }

        private static SRC CreateField(int width, int height, out HeadlessGUI gui)
        {
            gui = new HeadlessGUI();
            LastGui = gui;
            // 戦闘アニメは使わない。味方の反撃・防御は自動反撃モードで自動で選ぶ
            var src = new SRC { SystemConfig = new SRCCore.Config.LocalFileConfig { BattleAnimation = false, WeaponAnimation = false, ExtendedAnimation = false, AutoDefense = true } };
            gui.Install(src);
            // 戦闘のメッセージの表示時間の待ちを無くす
            src.GUI.MessageWait = 0;
            var fs = new LocalFileSystem();
            fs.AddPath(GoldenDir("ComTactics"));
            src.FileSystem = fs;
            // イベントは無い（使用・変形などのイベントの呼び出しが空振りするだけ）
            src.Event.InitEventData();
            src.SPDList.Load(Path.Combine(BasicDataSystemDir(), "sp.txt"));
            src.LoadDataDirectory(Path.Combine(GoldenDir("ComTactics"), "Data"));

            var plain = new TerrainData { ID = 0, Name = "平地", Bitmap = "plain", Class = "陸", MoveCost = 2 };
            var map = src.Map;
            map.MapFileName = "";
            map.MapWidth = width;
            map.MapHeight = height;
            map.MapData = new Src2DArray<MapCell>(width, height);
            for (var x = 1; x <= width; x++)
            {
                for (var y = 1; y <= height; y++)
                {
                    map.MapData[x, y] = new MapCell { X = x, Y = y, UnderTerrain = plain, UpperTerrain = plain };
                }
            }
            map.MapDataForUnit = new Src2DArray<Unit>(width, height);
            map.MaskData = new Src2DArray<bool>(width, height);
            map.TotalMoveCost = new int[width + 2, height + 2];
            map.PointInZOC = new int[width + 2, height + 2];
            return src;
        }

        private static Unit Deploy(SRC src, string unitName, string pilotName, string party, int x, int y, int hp)
        {
            var u = src.UList.Add(unitName, 0, party);
            var p = src.PList.Add(pilotName, 30, party);
            p.Ride(u);
            u.FullRecover();
            if (hp > 0)
            {
                u.HP = hp;
            }
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
