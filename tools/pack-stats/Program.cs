// GSC データパックの実態の集計。7474/SRC-Data の docs/データ作成ガイドライン.md の表を再現する。
// 使い方は README.md。
using SRCCore;
using SRCCore.Filesystem;
using SRCCore.Models;
using SRCCore.TestLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PackStats
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("PackStats <rounding|bands|tiers|map|map-list> <パックのフォルダ> [引数]");
                Console.Error.WriteLine("  rounding <パック>                     数値の刻みに合う割合");
                Console.Error.WriteLine("  bands <パック>                        経験値の帯・ザコ・汎用ごとの能力値、性格、SP、ユニット、サイズ");
                Console.Error.WriteLine("  tiers <パック> <格の一覧.tsv>         格ごとの比較（格<TAB>パイロット[<TAB>ユニット]）");
                Console.Error.WriteLine("  map <パック>                          マップ兵器の種類ごとの実態");
                Console.Error.WriteLine("  map-list <パック> <属性の正規表現>    属性に合う武器の一覧と、ユニットの経験値の帯ごとの集計");
                return 2;
            }
            var data = Data.Load(args[1]);
            switch (args[0])
            {
                case "rounding": Rounding(data); break;
                case "bands": Bands(data); break;
                case "tiers": Tiers(data, args[2]); break;
                case "map": Map(data); break;
                case "map-list": MapList(data, args[2]); break;
                default: Console.Error.WriteLine($"不明なコマンド: {args[0]}"); return 2;
            }
            return 0;
        }

        // ---- 共通

        private static readonly string[] PilotBands = { "200以上", "180-199", "160-179", "120-159", "120未満", "汎用", "ザコ" };
        private static readonly string[] UnitBands = { "200以上", "180-199", "160-179", "120-159", "120未満" };

        private static string ExpBand(int e) => e >= 200 ? "200以上" : e >= 180 ? "180-199" : e >= 160 ? "160-179" : e >= 120 ? "120-159" : "120未満";

        private static string PilotBand(PilotData p)
        {
            if (p.Name.Contains("(ザコ)")) return "ザコ";
            if (p.Name.Contains("(汎用)")) return "汎用";
            return ExpBand(p.ExpValue);
        }

        // SP を持つパイロット（サポートパイロットなどを除く）と、ザコ・汎用
        private static List<PilotData> Pilots(Data d) => d.Src.PDList.Items
            .Where(p => p.SP > 0 || p.Name.Contains("(ザコ)") || p.Name.Contains("(汎用)")).ToList();

        // 武器を持つユニット
        private static List<UnitData> Units(Data d) => d.Src.UDList.Items.Where(u => u.Weapons.Any() && u.HP > 0).ToList();

        private static int Total(PilotData p) => p.Infight + p.Shooting + p.Hit + p.Dodge + p.Technique + p.Intuition;

        private static int Quantile(IEnumerable<int> xs, double q)
        {
            var a = xs.OrderBy(x => x).ToList();
            return a.Count == 0 ? 0 : a[(int)Math.Round(q * (a.Count - 1))];
        }

        // 25%〜75%（中央値）
        private static string Iqr(IEnumerable<int> xs)
        {
            var a = xs.ToList();
            if (a.Count == 0) return "-";
            return $"{Quantile(a, .25)}〜{Quantile(a, .75)}（{Quantile(a, .5)}）";
        }

        // 最小〜最大（中央値）
        private static string Range(IEnumerable<int> xs)
        {
            var a = xs.ToList();
            if (a.Count == 0) return "-";
            return $"{a.Min()}〜{a.Max()}（{Quantile(a, .5)}）";
        }

        private static string Pct(int n, int total) => total == 0 ? "-" : $"{100.0 * n / total:F1}%";

        private static bool HasSp(PilotData p, string name) => p.DefinedSpecialPowers().Any(x => x.Name == name);

        // ---- rounding

        private static void Rounding(Data d)
        {
            var us = d.Src.UDList.Items.ToList();
            var ws = us.SelectMany(u => u.Weapons).ToList();
            Console.WriteLine($"# {d.Name} 数値の刻み（ユニット {us.Count} 体、武器 {ws.Count} 個）");
            Console.WriteLine();
            Console.WriteLine("| 項目 | 刻み | 合う割合 |");
            Console.WriteLine("|---|---|--:|");
            void Row(string item, int step, IEnumerable<int> xs) { var a = xs.ToList(); Console.WriteLine($"| {item} | {step} | {Pct(a.Count(x => x % step == 0), a.Count)} |"); }
            Row("HP", 100, us.Select(u => u.HP));
            Row("HP", 50, us.Select(u => u.HP));
            Row("EN", 10, us.Select(u => u.EN));
            Row("EN", 5, us.Select(u => u.EN));
            Row("装甲", 100, us.Select(u => u.Armor));
            Row("装甲", 50, us.Select(u => u.Armor));
            Row("運動性", 5, us.Select(u => u.Mobility));
            Row("運動性", 10, us.Select(u => u.Mobility));
            Row("武器の攻撃力", 100, ws.Select(w => w.Power));
            Row("武器の消費EN", 5, ws.Select(w => w.ENConsumption));
        }

        // ---- bands

        private static void Bands(Data d)
        {
            var ps = Pilots(d);
            Console.WriteLine($"# {d.Name}");
            Console.WriteLine();
            Console.WriteLine("## パイロット（25%〜75%（中央値））");
            Console.WriteLine();
            Console.WriteLine("| 層 | 人数 | 経験値 | 格闘 | 射撃 | 命中 | 回避 | 技量 | 反応 | 6 能力の合計 | SP |");
            Console.WriteLine("|---|--:|---|---|---|---|---|---|---|---|---|");
            foreach (var b in PilotBands)
            {
                var g = ps.Where(p => PilotBand(p) == b).ToList();
                if (g.Count == 0) continue;
                Console.WriteLine($"| {b} | {g.Count} | {Iqr(g.Select(p => p.ExpValue))} | {Iqr(g.Select(p => p.Infight))} | {Iqr(g.Select(p => p.Shooting))} | {Iqr(g.Select(p => p.Hit))} | {Iqr(g.Select(p => p.Dodge))} | {Iqr(g.Select(p => p.Technique))} | {Iqr(g.Select(p => p.Intuition))} | {Iqr(g.Select(Total))} | {Iqr(g.Select(p => p.SP))} |");
            }

            string[] pers = { "超強気", "強気", "普通", "弱気", "機械" };
            string[] sps = { "魂", "覚醒", "熱血", "ひらめき", "集中", "必中", "激励", "応援", "信頼", "補給", "幸運", "努力" };
            Console.WriteLine();
            Console.WriteLine("## 性格の割合");
            Console.WriteLine();
            Console.WriteLine("| 層 | " + string.Join(" | ", pers) + " |");
            Console.WriteLine("|---|" + string.Concat(pers.Select(_ => "--:|")));
            foreach (var b in PilotBands)
            {
                var g = ps.Where(p => PilotBand(p) == b).ToList();
                if (g.Count == 0) continue;
                Console.WriteLine($"| {b} | " + string.Join(" | ", pers.Select(x => Pct(g.Count(p => p.Personality == x), g.Count))) + " |");
            }
            var other = ps.Select(p => p.Personality).Distinct().Where(x => !pers.Contains(x)).ToList();
            if (other.Any()) Console.WriteLine($"\nほかの性格: {string.Join("、", other)}");

            Console.WriteLine();
            Console.WriteLine("## SP を持つ割合");
            Console.WriteLine();
            Console.WriteLine("| 層 | " + string.Join(" | ", sps) + " |");
            Console.WriteLine("|---|" + string.Concat(sps.Select(_ => "--:|")));
            foreach (var b in PilotBands)
            {
                var g = ps.Where(p => PilotBand(p) == b).ToList();
                if (g.Count == 0) continue;
                Console.WriteLine($"| {b} | " + string.Join(" | ", sps.Select(x => Pct(g.Count(p => HasSp(p, x)), g.Count))) + " |");
            }

            var us = Units(d);
            Console.WriteLine();
            Console.WriteLine("## ユニット（ユニットの経験値の帯。25%〜75%（中央値））");
            Console.WriteLine();
            Console.WriteLine("| ユニットの経験値 | 体数 | HP | EN | 装甲 | 運動性 | 移動力 | 最大攻撃力 | 武器数 |");
            Console.WriteLine("|---|--:|---|---|---|---|---|---|---|");
            foreach (var b in UnitBands)
            {
                var g = us.Where(u => ExpBand(u.ExpValue) == b).ToList();
                if (g.Count == 0) continue;
                Console.WriteLine($"| {b} | {g.Count} | {Iqr(g.Select(u => u.HP))} | {Iqr(g.Select(u => u.EN))} | {Iqr(g.Select(u => u.Armor))} | {Iqr(g.Select(u => u.Mobility))} | {Iqr(g.Select(u => u.Speed))} | {Iqr(g.Select(u => u.Weapons.Max(w => w.Power)))} | {Iqr(g.Select(u => u.Weapons.Count))} |");
            }

            Console.WriteLine();
            Console.WriteLine("## サイズの割合");
            Console.WriteLine();
            Console.WriteLine("| サイズ | 割合 |");
            Console.WriteLine("|---|--:|");
            foreach (var g in us.GroupBy(u => u.Size).OrderByDescending(g => g.Count()))
            {
                Console.WriteLine($"| {g.Key} | {Pct(g.Count(), us.Count)} |");
            }

            var ws = us.SelectMany(u => u.Weapons).ToList();
            Console.WriteLine();
            Console.WriteLine("## 武器（25%〜75%（中央値））");
            Console.WriteLine();
            Console.WriteLine($"- 攻撃力（攻撃するもの）: {Iqr(ws.Where(w => w.Power > 0).Select(w => w.Power))}");
            Console.WriteLine($"- 命中補正: {Iqr(ws.Select(w => w.Precision))}");
            Console.WriteLine($"- クリティカル率: {Iqr(ws.Select(w => w.Critical))}");
        }

        // ---- tiers

        private static void Tiers(Data d, string listPath)
        {
            var rows = File.ReadAllLines(listPath)
                .Where(l => l.Trim() != "" && !l.StartsWith("#"))
                .Select(l => l.Split('\t'))
                .Where(t => t.Length >= 2)
                .Select(t => (tier: t[0], pilotName: t[1], unitName: t.Length > 2 && t[2] != "" ? t[2] : t[1]))
                .ToList();
            var missing = rows.Where(r => !d.Src.PDList.IsDefined(r.pilotName)).Select(r => r.pilotName).ToList();
            if (missing.Any()) Console.Error.WriteLine($"見つからないパイロット: {string.Join("、", missing)}");
            var found = rows.Where(r => d.Src.PDList.IsDefined(r.pilotName))
                .Select(r => (r.tier, p: d.Src.PDList.Item(r.pilotName), u: d.Src.UDList.IsDefined(r.unitName) ? d.Src.UDList.Item(r.unitName) : null))
                .ToList();

            Console.WriteLine($"# {d.Name} 格ごとの比較（最小〜最大（中央値））");
            Console.WriteLine();
            Console.WriteLine("| 格 | 人数 | 経験値 | 6 能力の合計 | SP | 魂・覚醒を持つ | 超強気・強気 | ユニットの HP | EN | 装甲 | 運動性 | 最大攻撃力 |");
            Console.WriteLine("|---|--:|---|---|---|--:|--:|---|---|---|---|---|");
            foreach (var g in found.GroupBy(r => r.tier))
            {
                var ps = g.Select(r => r.p).ToList();
                var us = g.Where(r => r.u != null && r.u.Weapons.Any()).Select(r => r.u).ToList();
                Console.WriteLine($"| {g.Key} | {ps.Count} | {Range(ps.Select(p => p.ExpValue))} | {Range(ps.Select(Total))} | {Range(ps.Select(p => p.SP))} | {Pct(ps.Count(p => HasSp(p, "魂") || HasSp(p, "覚醒")), ps.Count)} | {Pct(ps.Count(p => p.Personality.Contains("強気")), ps.Count)} | {Range(us.Select(u => u.HP))} | {Range(us.Select(u => u.EN))} | {Range(us.Select(u => u.Armor))} | {Range(us.Select(u => u.Mobility))} | {Range(us.Select(u => u.Weapons.Max(w => w.Power)))} |");
            }
            Console.WriteLine();
            Console.WriteLine("## キャラごと");
            Console.WriteLine();
            Console.WriteLine("| 格 | パイロット | 経験値 | 格闘 | 射撃 | 命中 | 回避 | 技量 | 反応 | 合計 | SP | 性格 |");
            Console.WriteLine("|---|---|--:|--:|--:|--:|--:|--:|--:|--:|--:|---|");
            foreach (var (tier, p, _) in found)
            {
                Console.WriteLine($"| {tier} | {p.Name} | {p.ExpValue} | {p.Infight} | {p.Shooting} | {p.Hit} | {p.Dodge} | {p.Technique} | {p.Intuition} | {Total(p)} | {p.SP} | {p.Personality} |");
            }
        }

        // ---- map

        private static readonly string[] MapKinds = { "Ｍ直", "Ｍ拡", "Ｍ扇", "Ｍ全", "Ｍ投", "Ｍ移", "Ｍ線" };

        private static bool IsMap(WeaponData w) => MapKinds.Any(k => w.Class.Contains(k));

        private static void Map(Data d)
        {
            var us = Units(d);
            var withMap = us.Count(u => u.Weapons.Any(IsMap));
            Console.WriteLine($"# {d.Name} マップ兵器");
            Console.WriteLine();
            Console.WriteLine($"マップ兵器を持つユニット: {withMap} / {us.Count}（{Pct(withMap, us.Count)}）");
            Console.WriteLine();
            Console.WriteLine("| 種類 | 件数（攻撃するもの） | 攻撃力 | EN | 気力 | 識の割合 | 弾数制の割合 |");
            Console.WriteLine("|---|--:|---|---|---|--:|--:|");
            var ws = us.SelectMany(u => u.Weapons).ToList();
            foreach (var k in MapKinds)
            {
                var g = ws.Where(w => w.Class.Contains(k) && w.Power > 0).ToList();
                if (g.Count == 0) continue;
                Console.WriteLine($"| {k} | {g.Count} | {Iqr(g.Select(w => w.Power))} | {Iqr(g.Where(w => w.ENConsumption > 0).Select(w => w.ENConsumption))} | {Iqr(g.Where(w => w.NecessaryMorale > 0).Select(w => w.NecessaryMorale))} | {Pct(g.Count(w => w.Class.Contains("識")), g.Count)} | {Pct(g.Count(w => w.Bullet > 0), g.Count)} |");
            }
            var ratio = us
                .Where(u => u.Weapons.Any(w => IsMap(w) && w.Power > 0) && u.Weapons.Any(w => !IsMap(w) && w.Power > 0))
                .Select(u => (int)(100.0 * u.Weapons.Where(IsMap).Max(w => w.Power) / u.Weapons.Where(w => !IsMap(w)).Max(w => w.Power)));
            Console.WriteLine();
            Console.WriteLine($"マップ兵器の最大攻撃力 ÷ 単体武器の最大攻撃力（%）: {Iqr(ratio)}");
        }

        private static void MapList(Data d, string pattern)
        {
            var rx = new Regex(pattern);
            var rows = Units(d).SelectMany(u => u.Weapons.Where(w => rx.IsMatch(w.Class) && w.Power > 0).Select(w => (u, w))).ToList();
            Console.WriteLine($"# {d.Name} 属性が /{pattern}/ に合う武器（攻撃するもの {rows.Count} 件）");
            Console.WriteLine();
            Console.WriteLine("| ユニットの経験値 | 識 | 件数 | 攻撃力 | EN | 気力 |");
            Console.WriteLine("|---|---|--:|---|---|---|");
            foreach (var g in rows.GroupBy(x => (band: ExpBand(x.u.ExpValue), id: x.w.Class.Contains("識"))).OrderBy(g => Array.IndexOf(UnitBands, g.Key.band)).ThenBy(g => g.Key.id))
            {
                var ws = g.Select(x => x.w).ToList();
                Console.WriteLine($"| {g.Key.band} | {(g.Key.id ? "あり" : "なし")} | {ws.Count} | {Iqr(ws.Select(w => w.Power))} | {Iqr(ws.Where(w => w.ENConsumption > 0).Select(w => w.ENConsumption))} | {Iqr(ws.Where(w => w.NecessaryMorale > 0).Select(w => w.NecessaryMorale))} |");
            }
            Console.WriteLine();
            Console.WriteLine("| 作品 | ユニット | ユニットの経験値 | 武器 | 攻撃力 | 射程 | EN | 弾数 | 気力 | 命中 | CT | 属性 |");
            Console.WriteLine("|---|---|--:|---|--:|---|--:|--:|--:|--:|--:|---|");
            foreach (var (u, w) in rows.OrderByDescending(x => x.w.Power).ThenBy(x => x.u.Name, StringComparer.Ordinal))
            {
                string N(int v) => v > 0 ? v.ToString() : "-";
                Console.WriteLine($"| {d.UnitWork.GetValueOrDefault(u.Name)} | {u.Name} | {u.ExpValue} | {w.Name} | {w.Power} | {w.MinRange}-{w.MaxRange} | {N(w.ENConsumption)} | {N(w.Bullet)} | {N(w.NecessaryMorale)} | {w.Precision:+0;-0;+0} | {w.Critical:+0;-0;+0} | {w.Class} |");
            }
        }
    }

    // パックの読み込み。読み込みの順を固定する（sp.txt を先に、ほかはパスの順）
    public class Data
    {
        public SRC Src;
        public string Name;
        public Dictionary<string, string> UnitWork = new Dictionary<string, string>();

        public static Data Load(string root)
        {
            var d = new Data
            {
                Src = new SRC { GUI = new MockGUI(), FileSystem = new LocalFileSystem() },
                Name = Path.GetFileName(Path.GetFullPath(root).TrimEnd('/', '\\')),
            };
            var files = Directory.EnumerateFiles(root, "*.txt", SearchOption.AllDirectories)
                .OrderBy(f => Path.GetFileName(f).ToLowerInvariant() == "sp.txt" ? 0 : 1)
                .ThenBy(f => f, StringComparer.Ordinal);
            foreach (var f in files)
            {
                var name = Path.GetFileName(f).ToLowerInvariant();
                var work = Path.GetFileName(Path.GetDirectoryName(f));
                var before = d.Src.UDList.Items.Select(x => x.Name).ToHashSet();
                try
                {
                    using var s = File.OpenRead(f);
                    if (name == "sp.txt") d.Src.SPDList.Load(name, s);
                    else if (name == "robot.txt" || name == "unit.txt") d.Src.UDList.Load(name, s);
                    else if (name == "pilot.txt") d.Src.PDList.Load(name, s);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"読み込めない: {f}: {ex.Message}");
                }
                foreach (var u in d.Src.UDList.Items.Where(x => !before.Contains(x.Name))) d.UnitWork[u.Name] = work;
                d.Src.ClearDataError();
            }
            return d;
        }
    }
}
