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
    /// スペシャルパワーとアビリティの効果を固定する（#1397）。
    /// basic-data の sp.txt と Golden/SpecialPower/Data/sp.txt（効果の種類と適用条件の足りない分）のスペシャルパワーを、
    /// 1 つずつ、対象の種類に合わせたユニットに使い、使ったあとと効果が切れたあとの全ユニットの状態を記録する。
    /// アビリティ（回復・補給・状態・付加・強化・変身・召喚など）とマップアビリティ（範囲の形ごと）も 1 つずつ使って記録する。
    /// 記録は Golden/SpecialPower/expected.txt と比べる。毎回、同じ配置・同じ乱数の種から始める。
    /// 意図して変えたときは、SRC_GOLDEN_UPDATE=1 で作り直し、差分を PR に載せる（docs/testing.md）。
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class SpecialPowerGoldenTests
    {
        private const int Seed = 20261102;

        // 記録するメッセージ
        private static readonly string[] MessageMembers = { "DisplaySysMessage", "DisplayMessage", "ErrorMessage" };

        // ユニットの通称・ユニット・パイロット・陣営・位置
        private static readonly (string Key, string Unit, string Pilot, string Party, int X, int Y)[] Units =
        {
            ("術者", "術者機", "術者", "味方", 3, 4),
            ("隣の味方", "僚機", "僚友", "味方", 4, 4),
            ("離れた味方", "僚機", "僚友２", "味方", 3, 6),
            ("支援", "支援機", "支援者", "味方", 6, 4),
            ("隣の敵", "標的機", "敵兵", "敵", 3, 3),
            ("ボス", "標的機", "敵将", "敵", 8, 4),
            ("離れた敵", "標的機", "敵兵２", "敵", 8, 6),
            ("倒れた味方", "僚機", "倒れた僚友", "味方", 1, 7),
        };

        // 隣の味方をいろいろな状態異常にしてから使う
        private const string BadStatusAlly = "状態異常の味方";

        // 対象を選ぶスペシャルパワーを、どのユニットに使うか
        private static readonly Dictionary<string, string[]> SpTargets = new Dictionary<string, string[]>
        {
            ["味方"] = new[] { "隣の味方", "離れた味方", "術者", BadStatusAlly },
            ["敵"] = new[] { "隣の敵", "ボス" },
            ["任意"] = new[] { "隣の味方", "隣の敵", BadStatusAlly },
            ["破壊味方"] = new[] { "倒れた味方" },
        };

        // 術者が使うアビリティ。1 行が 1 つの場面で、左から順に使う
        private static readonly (string Ability, string Target)[][] AbilityCases =
        {
            new[] { ("修理", "隣の味方") },
            new[] { ("修理", "術者") },
            new[] { ("補給", "隣の味方") },
            new[] { ("霊力注入", "隣の味方") },
            new[] { ("ＳＰ注入", "隣の味方") },
            new[] { ("鼓舞", "隣の味方") },
            new[] { ("装填", "隣の味方") },
            new[] { ("治療", "隣の味方") },
            new[] { ("霧の守り", "隣の味方"), ("霧の守り", "隣の味方") },
            new[] { ("剣術指南", "隣の味方"), ("剣術指南", "隣の味方") },
            new[] { ("闘志", "隣の味方") },
            new[] { ("鈍足", "隣の敵") },
            new[] { ("霧の守り", "隣の味方"), ("剣術指南", "隣の味方"), ("解呪", "隣の味方") },
            new[] { ("再行動", "離れた味方") },
            new[] { ("失敗しやすい修理", "隣の味方") },
            new[] { ("解説付き修理", "隣の味方") },
            new[] { ("連携修理", "隣の味方") },
            new[] { ("癒しの呪文", "隣の味方") },
            new[] { ("癒しの杖", "術者") },
            new[] { ("癒しの秘術", "隣の味方") },
            new[] { ("癒しの歌", "隣の味方") },
            new[] { ("回復の踊り", "隣の味方") },
            new[] { (BadStatus, "隣の味方"), ("万能薬", "隣の味方") },
            new[] { ("疲れる修理", "隣の味方") },
            new[] { ("気合の修理", "隣の味方") },
            new[] { ("捨て身の修理", "隣の味方") },
            new[] { ("模倣", "隣の敵") },
            new[] { ("召喚", "術者") },
            new[] { ("変身", "術者") },
        };

        // 支援が使うマップアビリティと、向き・落とす先
        private static readonly (string Ability, int X, int Y)[] MapAbilityCases =
        {
            ("全体修理", 6, 4),
            ("全体補給", 6, 4),
            ("投下修理", 4, 4),
            ("直線鼓舞", 3, 4),
            ("拡散治療", 5, 4),
            ("扇形強化", 5, 4),
            ("光線修理", 4, 4),
            ("突進修理", 4, 5),
            ("連携全体修理", 6, 4),
            ("捨て身の全体修理", 6, 4),
        };

        // アビリティの代わりに、対象をいろいろな状態異常にする
        private const string BadStatus = "（状態異常にする）";
        private static readonly string[] BadStatuses =
        {
            "攻撃不能", "移動不能", "装甲劣化", "混乱", "恐怖", "踊り", "狂戦士", "ゾンビ", "回復不能", "石化", "凍結", "麻痺", "睡眠", "毒", "盲目", "沈黙", "魅了", "憑依", "撹乱",
        };

        private static HeadlessGUI LastGui;

        [TestMethod]
        public void SpecialPowersAndAbilities_MatchGolden()
        {
            // 入力を待つ画面などで止まったときは、最後の GUI の呼び出しを出して落とす（CI を止めないため）
            var task = System.Threading.Tasks.Task.Run(() => Render());
            if (!task.Wait(TimeSpan.FromSeconds(60)))
            {
                Assert.Fail("スペシャルパワーとアビリティが 60 秒で終わらない。最後の GUI の呼び出し:\n" + string.Join("\n", LastGui.Calls.Skip(Math.Max(0, LastGui.Calls.Count - 40)).Select(c => c.ToString())));
            }
            GoldenFile.AssertMatch(task.Result, Path.Combine(GoldenDir("SpecialPower"), "expected.txt"), "SpecialPowerGoldenTests", "スペシャルパワーとアビリティの効果");
        }

        private static string Render()
        {
            var sb = new StringBuilder();
            sb.Append("# スペシャルパワーとアビリティの効果の記録（SpecialPowerGoldenTests が作る。手で直さない）\n");
            sb.Append("# 状態は 位置 HP EN 気力 SP 霊力 行動数 弾数の合計 [効いているスペシャルパワー] {特殊状態:残りターン}。場面ごとに初めの状態から始め、前の記録から変わったユニットだけを出す。乱数の種 " + Seed + "\n");
            sb.Append("初め:\n");
            foreach (var line in new Field().States())
            {
                sb.Append("  " + line + "\n");
            }

            // スペシャルパワー
            var names = new Field().Src.SPDList.Items.Select(x => x.Name).ToList();
            foreach (var name in names)
            {
                var sd = new Field().Src.SPDList.Item(name);
                var targets = SpTargets.TryGetValue(sd.TargetType ?? "", out var keys) ? keys : new string[] { null };
                foreach (var key in targets)
                {
                    RenderSpecialPower(sb, name, key);
                }
            }

            // アビリティ
            foreach (var steps in AbilityCases)
            {
                var field = new Field();
                var user = field.Unit("術者");
                foreach (var (ability, target) in steps)
                {
                    if (ability == BadStatus)
                    {
                        var bt = field.Unit(target);
                        foreach (var cond in BadStatuses)
                        {
                            bt.AddCondition(cond, 2);
                        }
                        sb.Append($"== {target}を {string.Join("・", BadStatuses)} にする\n");
                        field.AppendStates(sb);
                        continue;
                    }
                    var ua = user.CurrentForm().Abilities.First(x => x.Data.Name == ability);
                    var t = field.Unit(target).CurrentForm();
                    sb.Append($"== アビリティ {ability} → {target}（効く {ua.IsAbilityEffective(t)}・使える {ua.IsAbilityApplicable(t)}）\n");
                    var result = field.Record(sb, () => user.CurrentForm().ExecuteAbility(ua, t));
                    sb.Append($"  戻り値 {result}\n");
                    field.AppendStates(sb);
                }
            }

            // マップアビリティ
            foreach (var (ability, x, y) in MapAbilityCases)
            {
                var field = new Field();
                var user = field.Unit("支援");
                var ua = user.Abilities.First(a => a.Data.Name == ability);
                sb.Append($"== マップアビリティ {ability} → ({x},{y})\n");
                field.Record(sb, () => { user.ExecuteMapAbility(ua, x, y); return true; });
                field.AppendStates(sb);
            }
            return sb.ToString();
        }

        private static void RenderSpecialPower(StringBuilder sb, string name, string targetKey)
        {
            var field = new Field();
            var src = field.Src;
            var sd = src.SPDList.Item(name);
            var user = field.Unit("術者");
            var p = user.MainPilot();
            Unit t = null;
            if (targetKey == BadStatusAlly)
            {
                t = field.Unit("隣の味方");
                foreach (var cond in BadStatuses)
                {
                    t.AddCondition(cond, 2);
                }
                field.TakeSnapshot();
            }
            else if (targetKey != null)
            {
                t = field.Unit(targetKey);
            }
            var effects = string.Join(" ", sd.Effects.Select(e => e.strEffectType + (e.dblEffectLevel != 0d ? "Lv" + e.dblEffectLevel.ToString(System.Globalization.CultureInfo.InvariantCulture) : "") + (string.IsNullOrEmpty(e.strEffectData) ? "" : "=" + e.strEffectData)));
            sb.Append($"== {name}（{sd.TargetType}・{sd.Duration}・{sd.NecessaryCondition}）{effects}");
            sb.Append(t == null ? "\n" : $" → {targetKey}\n");
            sb.Append($"  役に立つ {sd.Useful(p)}・対象の数 {sd.CountTarget(p)}" + (t == null ? "" : $"・効く {sd.Effective(p, t)}") + "\n");
            src.Commands.SelectedUnit = user;
            src.Commands.SelectedTarget = t;
            field.Record(sb, () => { p.UseSpecialPower(name); return true; });
            field.AppendStates(sb);

            // 効果が続くものは、切れたとき（RemoveSpecialPowerInEffect）にも効果を出すので、それも記録する
            if (sd.Duration != "即効")
            {
                var affected = field.All.Select(u => u.CurrentForm()).Where(u => u.IsSpecialPowerInEffect(name)).ToList();
                if (affected.Any())
                {
                    sb.Append($"  「{sd.Duration}」の効果が切れる:\n");
                    field.Record(sb, () =>
                    {
                        foreach (var u in affected)
                        {
                            u.RemoveSpecialPowerInEffect(sd.Duration);
                        }
                        return true;
                    });
                    field.AppendStates(sb);
                }
            }
        }

        // 毎回同じ配置から始める戦場
        private class Field
        {
            public SRC Src { get; }
            public HeadlessGUI Gui { get; }
            private readonly Dictionary<string, Unit> units = new Dictionary<string, Unit>();
            public IEnumerable<Unit> All => Units.Select(x => units[x.Key]);

            public Field()
            {
                Gui = new HeadlessGUI();
                LastGui = Gui;
                var src = new SRC { SystemConfig = new SRCCore.Config.LocalFileConfig { BattleAnimation = false, WeaponAnimation = false, ExtendedAnimation = false, AutoDefense = true } };
                Src = src;
                Gui.Install(src);
                src.GUI.MessageWait = 0;
                var fs = new LocalFileSystem();
                fs.AddPath(GoldenDir("SpecialPower"));
                src.FileSystem = fs;
                // イベントは無い（使用・復活などのイベントの呼び出しが空振りするだけ）
                src.Event.InitEventData();
                src.SPDList.Load(Path.Combine(BasicDataSystemDir(), "sp.txt"));
                src.LoadDataDirectory(Path.Combine(GoldenDir("SpecialPower"), "Data"));
                src.SPDList.Load(Path.Combine(GoldenDir("SpecialPower"), "Data", "sp.txt"));

                var plain = new TerrainData { ID = 0, Name = "平地", Bitmap = "plain", Class = "陸", MoveCost = 2 };
                const int width = 10;
                const int height = 8;
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
                src.Stage = "味方";
                src.Turn = 3;

                foreach (var d in Units)
                {
                    var u = src.UList.Add(d.Unit, 0, d.Party);
                    var p = src.PList.Add(d.Pilot, 30, d.Party);
                    p.Ride(u);
                    u.FullRecover();
                    u.StandBy(d.X, d.Y);
                    units[d.Key] = u;
                }

                // 回復や補給の効果が見えるよう、減らしておく
                var user = units["術者"];
                user.HP = 2000;
                user.EN = 40;
                user.Weapon(1).SetBullet(1);
                user.MainPilot().Plana = 10;

                var ally = units["隣の味方"];
                ally.HP = 2000;
                ally.EN = 30;
                ally.UsedAction = 1;
                ally.Weapon(1).SetBullet(2);
                ally.MainPilot().Plana = 0;
                ally.MainPilot().SP = 20;
                ally.AddCondition("麻痺", 2);
                ally.AddCondition("毒", 2);
                ally.AddCondition("攻撃不能", 1);

                var far = units["離れた味方"];
                far.HP = 3500;
                far.EN = 100;
                far.UsedAction = 1;

                var enemy = units["隣の敵"];
                enemy.HP = 3000;

                // ボスはスペシャルパワーが効かない
                var boss = units["ボス"];
                boss.BossRank = 1;
                boss.AddCondition("スペシャルパワー無効", -1);

                units["倒れた味方"].Die();

                GeneralLib.RndSeed = Seed;
                GeneralLib.RndReset();
                TakeSnapshot();
            }

            public Unit Unit(string key)
            {
                return units[key];
            }

            // 動かして、そのあいだに出たメッセージを記録する
            public T Record<T>(StringBuilder sb, Func<T> action)
            {
                var before = Gui.Calls.Count;
                var result = action();
                foreach (var c in Gui.Calls.Skip(before).Where(c => MessageMembers.Contains(c.Member)))
                {
                    var text = c.Member == "DisplayMessage" ? c.Args[0] + "「" + c.Args[1] + "」" : (string)c.Args[0];
                    sb.Append("  > " + (c.Member == "ErrorMessage" ? "エラー: " : "") + text.Replace("\n", " ") + "\n");
                }
                return result;
            }

            // 前に記録したときから変わったユニットだけを記録する
            public void AppendStates(StringBuilder sb)
            {
                var lines = States();
                var changed = lines.Where(x => !lastStates.Contains(x)).ToList();
                foreach (var line in changed)
                {
                    sb.Append("  " + line + "\n");
                }
                if (!changed.Any())
                {
                    sb.Append("  （変化なし）\n");
                }
                lastStates = lines;
            }

            private List<string> lastStates = new List<string>();

            // いまの状態を、変化を見る元にする
            public void TakeSnapshot()
            {
                lastStates = States();
            }

            public List<string> States()
            {
                var lines = new List<string>();
                foreach (var d in Units)
                {
                    var u = units[d.Key].CurrentForm();
                    if (u.Status != "出撃")
                    {
                        lines.Add($"{d.Key}:{u.Name} {u.Status}");
                        continue;
                    }
                    var p = u.MainPilot();
                    var bullets = u.Weapons.Sum(w => w.Bullet());
                    var conds = string.Join(" ", u.Conditions.Select(c => c.Name + ":" + c.Lifetime));
                    lines.Add($"{d.Key}:{u.Name} ({u.x},{u.y}) {u.HP} {u.EN} {p.Morale} {p.SP} {p.Plana} {u.Action} {bullets} [{u.SpecialPowerInEffect()}] {{{conds}}}");
                }
                // 召喚などで増えたユニット
                foreach (var u in Src.UList.Items.Where(x => x.Status == "出撃" && !units.Values.Any(y => ReferenceEquals(y.CurrentForm(), x.CurrentForm()))))
                {
                    lines.Add($"増えた:{u.Name}({u.MainPilot().Name}) {u.Party} ({u.x},{u.y}) {u.HP} {u.EN}");
                }
                return lines;
            }
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
