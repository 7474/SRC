using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
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
using static System.FormattableString;

namespace SRCCore.Golden.Tests
{
    /// <summary>
    /// 攻撃 1 回の結果と、武器の属性による計算の結果を、組み合わせごとの表にして固定する（#1395）。
    /// 攻撃の表（Golden/Attack/expected.txt）は、乱数の種を組み合わせごとに固定して Attack コマンド（通常戦闘）を 1 回動かし、
    /// 防御側・攻撃側の HP・EN・位置と、戦闘のメッセージ（どの防御の特殊能力が働いたか・反撃の結果）を記録する。
    /// 計算の表（Golden/Attack/calc.txt）は、命中率・ダメージ・クリティカル率・期待ダメージ（ExpDamage）を記録する。
    /// データは Golden/Attack/Data。Golden/Battle/Data とは分け、ComGoldenTests の記録に響かないようにする。
    /// 数値は文化に依らない書き方（Invariant）で書き、Windows と Linux で同じ表になるようにする。
    /// 意図して変えたときは、SRC_GOLDEN_UPDATE=1 で表を作り直し、差分を PR に載せる（docs/testing.md）。
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class AttackGoldenTests
    {
        // 防御側（表の名前・ユニット・パイロット・気力・隣に置く味方）
        private static readonly (string Label, string Unit, string Pilot, int Morale, string Helper)[] Defenders =
        {
            ("素体機", "素体機", "防御パイロット", 100, null),
            ("シールド機", "シールド機", "防御パイロット", 100, null),
            ("大型シールド機", "大型シールド機", "防御パイロット", 100, null),
            ("小型シールド機", "小型シールド機", "防御パイロット", 100, null),
            ("アクティブシールド機", "アクティブシールド機", "防御パイロット", 100, null),
            ("エネルギーシールド機", "エネルギーシールド機", "防御パイロット", 100, null),
            ("盾機", "盾機", "防御パイロット", 100, null),
            ("バリア機", "バリア機", "防御パイロット", 100, null),
            ("手動バリア機", "手動バリア機", "防御パイロット", 100, null),
            ("フィールド機", "フィールド機", "防御パイロット", 100, null),
            ("プロテクション機", "プロテクション機", "防御パイロット", 100, null),
            ("バリアシールド機", "バリアシールド機", "防御パイロット", 100, null),
            ("アクティブフィールド機", "アクティブフィールド機", "防御パイロット", 100, null),
            ("アクティブプロテクション機", "アクティブプロテクション機", "防御パイロット", 100, null),
            ("反射機", "反射機", "防御パイロット", 100, null),
            ("当て身機", "当て身機", "防御パイロット", 100, null),
            ("阻止機", "阻止機", "防御パイロット", 100, null),
            ("分身機（気力 130）", "分身機", "防御パイロット", 130, null),
            ("超回避機", "超回避機", "防御パイロット", 100, null),
            ("緊急テレポート機", "緊急テレポート機", "防御パイロット", 100, null),
            ("ダミー機", "ダミー機", "防御パイロット", 100, null),
            ("ビーム吸収機", "ビーム吸収機", "防御パイロット", 100, null),
            ("変換機", "変換機", "防御パイロット", 100, null),
            ("融合機", "融合機", "防御パイロット", 100, null),
            ("吸収機", "吸収機", "防御パイロット", 100, null),
            ("耐性機", "耐性機", "防御パイロット", 100, null),
            ("装甲機", "装甲機", "防御パイロット", 100, null),
            ("自動反撃機", "自動反撃機", "防御パイロット", 100, null),
            ("切り払い（パイロット）", "素体機", "切り払いパイロット", 100, null),
            ("迎撃（パイロット）", "素体機", "迎撃パイロット", 100, null),
            ("分身（パイロット）", "素体機", "分身パイロット", 100, null),
            ("広域バリアの隣", "素体機", "防御パイロット", 100, "広域バリア機"),
            ("広域フィールドの隣", "素体機", "防御パイロット", 100, "広域フィールド機"),
            ("広域プロテクションの隣", "素体機", "防御パイロット", 100, "広域プロテクション機"),
            ("広域阻止の隣", "素体機", "防御パイロット", 100, "広域阻止機"),
            ("能力バリア機", "能力バリア機", "能力パイロット", 100, null),
            ("能力フィールド機", "能力フィールド機", "能力パイロット", 100, null),
            ("能力プロテクション機", "能力プロテクション機", "能力パイロット", 100, null),
            ("能力アクティブ機", "能力アクティブ機", "能力パイロット", 100, null),
            ("能力反射機", "能力反射機", "能力パイロット", 100, null),
            ("能力阻止機", "能力阻止機", "能力パイロット", 100, null),
            ("能力当て身機", "能力当て身機", "能力パイロット", 100, null),
            ("能力自動反撃機", "能力自動反撃機", "能力パイロット", 100, null),
            ("特殊効果無効機", "特殊効果無効機", "防御パイロット", 100, null),
            ("機械（パイロット）", "素体機", "機械パイロット", 100, null),
            ("極小機", "極小機", "防御パイロット", 100, null),
            ("小型機", "小型機", "防御パイロット", 100, null),
            ("大型機", "大型機", "防御パイロット", 100, null),
            ("特大機", "特大機", "防御パイロット", 100, null),
            ("超大型機", "超大型機", "防御パイロット", 100, null),
        };

        // 攻撃の表で動かす防御の仕方（Attack コマンドの 5 番目の引数）と乱数の種
        private static readonly (string Option, int[] Seeds)[] DefenseOptions =
        {
            ("自動", new[] { 1, 2 }),
            ("防御", new[] { 3 }),
            ("回避", new[] { 4 }),
        };

        // 計算の表だけで使う防御側と武器（攻撃の表では結果が素体機と変わらないため省く）
        private static readonly HashSet<string> CalcOnlyDefenders = new HashSet<string> { "極小機", "小型機", "大型機", "特殊効果無効機", "機械（パイロット）" };
        private static readonly HashSet<string> CalcOnlyWeapons = new HashSet<string> { "拡散砲", "固定砲", "麻痺弾", "石化光線", "封印剣", "睡眠波", "弱体化砲", "転移砲", "撹乱波" };

        // 計算の表で、オプション（Option コマンド）を変えて見る防御側と、オプションの組み合わせ
        private static readonly string[] OptionDefenders = { "素体機", "装甲機", "特大機" };
        private static readonly string[][] OptionSets =
        {
            new[] { "距離修正", "地形適応命中率修正", "防御力倍率低下", "ダメージ倍率低下", "気力効果小", "ダメージ下限１" },
            new[] { "距離修正", "大型マップ", "地形適応修正緩和", "ダメージ下限解除" },
            new[] { "距離修正", "小型マップ", "地形適応修正繰り下げ" },
        };

        private static readonly string[] CalcPilots = { "攻撃パイロット", "技能パイロット" };

        private const int TargetX = 6;
        private const int Y = 2;

        private static HeadlessGUI LastGui;
        // 戦闘のメッセージ（DisplaySysMessage・ErrorMessage）。攻撃のたびに空にする
        private static readonly List<string> Messages = new List<string>();

        [TestMethod]
        public void AttackResults_MatchGolden()
        {
            // 入力を待つ画面などで止まったときは、最後の GUI の呼び出しを出して落とす（CI を止めないため）
            var task = System.Threading.Tasks.Task.Run(() => RenderAttacks());
            if (!task.Wait(TimeSpan.FromSeconds(120)))
            {
                Assert.Fail("攻撃が 120 秒で終わらない。最後の GUI の呼び出し:\n" + string.Join("\n", LastGui.Calls.Skip(Math.Max(0, LastGui.Calls.Count - 40)).Select(c => c.ToString())));
            }
            Compare("expected.txt", task.Result, "攻撃の結果");
        }

        [TestMethod]
        public void WeaponCalc_MatchGolden()
        {
            Compare("calc.txt", RenderCalc(), "武器の計算の結果");
        }

        private static void Compare(string fileName, string actual, string what)
        {
            var expectedPath = Path.Combine(GoldenDir(), fileName);
            if (Environment.GetEnvironmentVariable("SRC_GOLDEN_UPDATE") == "1")
            {
                File.WriteAllText(expectedPath, actual, new UTF8Encoding(false));
                Assert.Inconclusive(fileName + " を作り直した（SRC_GOLDEN_UPDATE=1）");
            }
            Assert.IsTrue(File.Exists(expectedPath), fileName + " が無い。SRC_GOLDEN_UPDATE=1 dotnet test --filter AttackGoldenTests で作る");
            var expected = File.ReadAllText(expectedPath).Replace("\r\n", "\n");
            if (expected == actual)
            {
                return;
            }
            // どの組み合わせが変わったかを先頭から 20 件だけ出す
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
            Assert.Fail(what + "が変わった。意図した変更なら SRC_GOLDEN_UPDATE=1 で作り直して差分を PR に載せる\n" + string.Join("\n", diffs));
        }

        private static string RenderAttacks()
        {
            var sb = new StringBuilder();
            sb.Append("# 攻撃 1 回の結果（AttackGoldenTests が作る。手で直さない）\n");
            sb.Append("# 武器 | 防御側 | 防御の仕方 | 乱数の種 => 防御側の HP EN 位置 状態 / 攻撃側の HP EN 位置 状態 [/ 隣の味方の EN] | 戦闘のメッセージ\n");
            // データの読み込みに時間がかかるため、戦場は武器ごとに 1 つ作り、攻撃のたびにユニットを置き直す
            var weapons = WeaponNames().Where(x => !CalcOnlyWeapons.Contains(x));
            foreach (var weapon in weapons)
            {
                var src = CreateField(out _);
                foreach (var d in Defenders.Where(x => !CalcOnlyDefenders.Contains(x.Label)))
                {
                    foreach (var opt in DefenseOptions)
                    {
                        foreach (var seed in opt.Seeds)
                        {
                            sb.Append(Invariant($"{weapon} | {d.Label} | {opt.Option} | {seed} => "))
                                .Append(RunAttack(src, weapon, d, opt.Option, seed))
                                .Append('\n');
                        }
                    }
                }
            }
            return sb.ToString();
        }

        private static string RunAttack(SRC src, string weapon, (string Label, string Unit, string Pilot, int Morale, string Helper) d, string option, int seed)
        {
            var b = Deploy(src, "攻撃パイロット", weapon, d);
            // 配置のあとで乱数を固定する（組み合わせごとに同じ系列から始める）
            GeneralLib.RndSeed = seed;
            GeneralLib.RndReset();
            Messages.Clear();

            var line = new EventDataLine(0, EventDataSource.Scenario, "golden", 0,
                $"Attack {b.Attacker.MainPilot().Name} {weapon} {b.Target.MainPilot().Name} {option} 通常戦闘");
            src.Event.EventData.Add(line);
            var cmd = new CmdParser().Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            var ret = cmd.Exec();
            src.Event.EventData.Remove(line);
            src.Event.EventCmd.Remove(cmd);

            // 残りの HP は別に記録するため、メッセージからは除く
            var msgs = Messages.Select(m => string.Join(";", m.Split(';').Where(x => !x.StartsWith("残りＨＰは"))));
            var result = new StringBuilder();
            if (ret < 0)
            {
                result.Append("エラー / ");
            }
            result.Append(State(b.Target)).Append(" / ").Append(State(b.Attacker));
            if (b.Helper != null)
            {
                result.Append(Invariant($" / 隣 EN {b.Helper.EN}"));
            }
            result.Append(" | ").Append(string.Join(" / ", msgs));
            return result.ToString();
        }

        private static string State(Unit u)
        {
            u = u.CurrentForm();
            return u.Status == "出撃"
                ? Invariant($"{u.HP} {u.EN} ({u.x},{u.y}) {u.MainPilot().Morale}")
                : Invariant($"{u.HP} {u.EN} {u.Status}");
        }

        private static string RenderCalc()
        {
            var sb = new StringBuilder();
            sb.Append("# 武器の計算の結果（AttackGoldenTests が作る。手で直さない）\n");
            sb.Append("# [オプション] 攻撃側のパイロット | 武器 | 防御側 | 距離 => 使える 命中率 ダメージ クリティカル率 期待ダメージ\n");
            var weapons = WeaponNames();
            foreach (var pilot in CalcPilots)
            {
                RenderCalc(sb, "", pilot, weapons, Defenders);
            }
            var optionDefenders = Defenders.Where(x => OptionDefenders.Contains(x.Label)).ToArray();
            foreach (var options in OptionSets)
            {
                RenderCalc(sb, string.Join(" ", options), "攻撃パイロット", weapons, optionDefenders);
            }
            // バトルコンフィグデータ（battle.txt）で計算式を定義し直したとき
            RenderCalc(sb, "バトルコンフィグ", "攻撃パイロット", weapons, optionDefenders);
            return sb.ToString();
        }

        // label はオプションを空白で区切って並べたもの（「バトルコンフィグ」のときは battle.txt を読む）
        private static void RenderCalc(StringBuilder sb, string label, string pilot, List<string> weapons, (string Label, string Unit, string Pilot, int Morale, string Helper)[] defenders)
        {
            var src = CreateField(out _);
            var options = label;
            if (label == "バトルコンフィグ")
            {
                src.BCList.Load(Path.Combine(GoldenDir(), "battle.txt"));
                options = "";
            }
            foreach (var option in options.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                src.Expression.DefineGlobalVariable("Option(" + option + ")");
                src.Expression.SetVariableAsLong("Option(" + option + ")", 1);
            }
            var prefix = label == "" ? "" : "[" + label + "] ";
            foreach (var weapon in weapons)
            {
                // 射程の内側の端と、攻撃の表と同じ距離の 2 通り
                var wd = src.UDList.Item("攻撃機").Weapons.First(x => x.Name == weapon);
                foreach (var distance in new[] { Math.Max(wd.MinRange, 1), AttackDistance(wd) }.Distinct())
                {
                    // ユニットを置くのに時間がかかるため、攻撃側はそのままにして防御側だけを置き直す
                    var attacker = Deploy(src, pilot, weapon, defenders[0], distance).Attacker;
                    var w = attacker.Weapons.First(x => x.Name == weapon);
                    foreach (var d in defenders)
                    {
                        var target = ReplaceTarget(src, d);
                        sb.Append(Invariant($"{prefix}{pilot} | {weapon} | {d.Label} | {distance} => "))
                            .Append(w.IsWeaponAvailable("移動前") ? "可" : "不可")
                            .Append(Invariant($" {w.HitProbability(target, true)} {w.Damage(target, true)} {w.CriticalProbability(target)} {w.ExpDamage(target, true)}\n"));
                    }
                }
            }
        }

        private static List<string> WeaponNames()
        {
            var src = CreateField(out _);
            return Deploy(src, "攻撃パイロット", null, Defenders[0]).Attacker.Weapons.Select(w => w.Name).ToList();
        }

        private class Battle
        {
            public Unit Attacker;
            public Unit Target;
            public Unit Helper;
        }

        // 攻撃の表で攻撃側を置く距離。武器の最大射程（3 まで）。広域の防御能力（効果範囲 2）の外から撃てるようにする
        private static int AttackDistance(WeaponData wd)
        {
            return Math.Max(wd.MinRange, Math.Min(wd.MaxRange, 3));
        }

        // 防御側を (6, 2)、隣の味方を (7, 2) に置き、攻撃側を distance（0 のときは AttackDistance）だけ離して置く。
        // 前の組み合わせのユニット・パイロットを取り除いてから置く
        private static Battle Deploy(SRC src, string attackerPilot, string weapon, (string Label, string Unit, string Pilot, int Morale, string Helper) d, int distance = 0)
        {
            src.UList.Clear();
            foreach (var p in src.PList.Items.ToList())
            {
                src.PList.Delete(p.ID);
            }
            src.Map.MapDataForUnit = new Src2DArray<Unit>(src.Map.MapWidth, src.Map.MapHeight);
            src.Commands.SelectedUnit = null;
            src.Commands.SelectedTarget = null;
            var (target, helper) = PlaceTarget(src, d);
            if (distance == 0)
            {
                distance = weapon == null ? 1 : AttackDistance(src.UDList.Item("攻撃機").Weapons.First(x => x.Name == weapon));
            }
            var attacker = Place(src, "攻撃機", attackerPilot, "味方", TargetX - distance, Y);
            attacker.Update();
            return new Battle { Attacker = attacker, Target = target, Helper = helper };
        }

        // 防御側（と隣の味方）を置く
        private static (Unit Target, Unit Helper) PlaceTarget(SRC src, (string Label, string Unit, string Pilot, int Morale, string Helper) d)
        {
            var target = Place(src, d.Unit, d.Pilot, "敵", TargetX, Y);
            target.MainPilot().Morale = d.Morale;
            Unit helper = null;
            if (d.Helper != null)
            {
                helper = Place(src, d.Helper, "補助パイロット", "敵", TargetX + 1, Y);
            }
            target.Update();
            return (target, helper);
        }

        // 敵（防御側と隣の味方）だけを取り除いて置き直す
        private static Unit ReplaceTarget(SRC src, (string Label, string Unit, string Pilot, int Morale, string Helper) d)
        {
            foreach (var u in src.UList.Items.Where(x => x.Party0 == "敵").ToList())
            {
                foreach (var p in u.AllPilots.ToList())
                {
                    src.PList.Delete(p.ID);
                }
                src.Map.MapDataForUnit[u.x, u.y] = null;
                src.UList.Delete(u.ID);
            }
            return PlaceTarget(src, d).Target;
        }

        private static Unit Place(SRC src, string unitName, string pilotName, string party, int x, int y)
        {
            var u = src.UList.Add(unitName, 0, party);
            var p = src.PList.Add(pilotName, 30, party);
            p.Ride(u);
            u.FullRecover();
            u.StandBy(x, y);
            return u;
        }

        private static SRC CreateField(out HeadlessGUI gui)
        {
            gui = new HeadlessGUI();
            LastGui = gui;
            // 戦闘アニメは使わない。味方の反撃・防御は自動反撃モードで自動で選ぶ
            var src = new SRC { SystemConfig = new SRCCore.Config.LocalFileConfig { BattleAnimation = false, WeaponAnimation = false, ExtendedAnimation = false, AutoDefense = true } };
            gui.Install(src);
            gui.Handle(nameof(IGUI.DisplaySysMessage), args => { Messages.Add(args[0]?.ToString() ?? ""); return null; });
            gui.Handle(nameof(IGUI.ErrorMessage), args => { Messages.Add("エラー: " + (args[0]?.ToString() ?? "")); return null; });
            // 戦闘のメッセージの表示時間の待ちを無くす
            src.GUI.MessageWait = 0;
            var fs = new LocalFileSystem();
            fs.AddPath(GoldenDir());
            src.FileSystem = fs;
            // イベントは無い（戦闘の前後のイベントの呼び出しが空振りするだけ）
            src.Event.InitEventData();
            src.SPDList.Load(Path.Combine(BasicDataSystemDir(), "sp.txt"));
            src.LoadDataDirectory(Path.Combine(GoldenDir(), "Data"));

            const int width = 9;
            const int height = 3;
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

        private static string GoldenDir([CallerFilePath] string path = "")
        {
            return Path.Combine(Path.GetDirectoryName(path), "Attack");
        }

        private static string BasicDataSystemDir([CallerFilePath] string path = "")
        {
            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), "..", "..", "Content", "basic-data", "Data", "System"));
        }
    }
}
