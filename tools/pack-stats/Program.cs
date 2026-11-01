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
    public static partial class Program
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
                Console.Error.WriteLine("  skills <パック>                       パイロットの特殊能力・ユニットの特殊能力の保有率");
                Console.Error.WriteLine("  endurance <パック>                    最大攻撃力の武器を撃てる回数、切り札の気力");
                Console.Error.WriteLine("  growth <パック>                       要の SP を覚えるレベル、レベルの刻み");
                Console.Error.WriteLine("  sp-mix <パック>                       SP の効果の分類ごとの持ち方");
                Console.Error.WriteLine("  spread <パック>                       同じ帯の中での得意・苦手の数（個性の偏り）");
                Console.Error.WriteLine("  types <パック>                        同じ帯の中での位置による系統（リアル・スーパー・グレー）");
                Console.Error.WriteLine("  map-types <パック>                    マップ兵器の持たせ方の型");
                Console.Error.WriteLine("  sp-set <パック> [格の一覧]            SP ごとの持つ割合・消費・覚えるレベル、SP の数、格ごとの SP");
                Console.Error.WriteLine("  messages <パック>                     パイロットのメッセージ（pilot_message.txt）の量とシチュエーション");
                Console.Error.WriteLine("  check <パック> <作品のフォルダ>       作品のデータを、パックの実態と比べて総合的に評価する");
                Console.Error.WriteLine("  site <出力> <GSCデータパック> [作品のルート …]  GitHub Pages 用の、作品ごとのデータと評価のページ");
                Console.Error.WriteLine("  roles <組の評価の結果.md> <格の一覧>  組の評価（tools/pack-analysis）で、格ごとに決め手の使いどころを持つ割合");
                return 2;
            }
            if (args[0] == "roles")
            {
                Roles(args[1], args[2]);
                return 0;
            }
            if (args[0] == "site")
            {
                Site(args[1], args[2], args.Skip(3));
                return 0;
            }
            if (args[0] == "check")
            {
                Check(Data.Load(args[1]), args[2]);
                return 0;
            }
            var data = Data.Load(args[1]);
            switch (args[0])
            {
                case "rounding": Rounding(data); break;
                case "bands": Bands(data); break;
                case "tiers": Tiers(data, args[2]); break;
                case "map": Map(data); break;
                case "map-list": MapList(data, args[2]); break;
                case "skills": Skills(data); break;
                case "endurance": Endurance(data); break;
                case "growth": Growth(data); break;
                case "sp-mix": SpMix(data); break;
                case "spread": Spread(data); break;
                case "types": Types(data, args.Length > 2 ? args[2] : null); break;
                case "map-types": MapTypes(data); break;
                case "sp-set": SpSet(data, args.Length > 2 ? args[2] : null); break;
                case "messages": MessagesStats(data); break;
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
            .Where(p => !IsSystemPilot(p) && (p.SP > 0 || p.Name.Contains("(ザコ)") || p.Name.Contains("(汎用)"))).ToList();

        // SRCCore が自動で加えるパイロット（ステータス表示用ダミーパイロット など）
        private static bool IsSystemPilot(PilotData p) => p.Name.StartsWith("ステータス表示用");

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

            // パイロットの能力値（サポートパイロットなど、6 能力がすべて 1 のものは除く）
            var ps = d.Src.PDList.Items.Where(p => !(p.Infight == 1 && p.Shooting == 1 && p.Hit == 1 && p.Dodge == 1 && p.Technique == 1 && p.Intuition == 1)).ToList();
            var values = ps.SelectMany(p => new[] { p.Infight, p.Shooting, p.Hit, p.Dodge, p.Technique, p.Intuition }).ToList();
            Console.WriteLine();
            Console.WriteLine($"パイロット {ps.Count} 人の能力値（格闘〜反応）");
            Console.WriteLine();
            Console.WriteLine("| 項目 | 合う割合 |");
            Console.WriteLine("|---|--:|");
            Console.WriteLine($"| 能力値のうち 5 の倍数 | {Pct(values.Count(x => x % 5 == 0), values.Count)} |");
            Console.WriteLine($"| 6 能力がすべて 5 の倍数のパイロット | {Pct(ps.Count(p => new[] { p.Infight, p.Shooting, p.Hit, p.Dodge, p.Technique, p.Intuition }.All(x => x % 5 == 0)), ps.Count)} |");
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
            Console.WriteLine("| ユニットの経験値 | 体数 | HP | EN | 装甲 | 運動性 | 移動力 | 最大攻撃力 | 最大射程 | 武器数 |");
            Console.WriteLine("|---|--:|---|---|---|---|---|---|---|---|");
            foreach (var b in UnitBands)
            {
                var g = us.Where(u => ExpBand(u.ExpValue) == b).ToList();
                if (g.Count == 0) continue;
                Console.WriteLine($"| {b} | {g.Count} | {Iqr(g.Select(u => u.HP))} | {Iqr(g.Select(u => u.EN))} | {Iqr(g.Select(u => u.Armor))} | {Iqr(g.Select(u => u.Mobility))} | {Iqr(g.Select(u => u.Speed))} | {Iqr(g.Select(u => u.Weapons.Max(w => w.Power)))} | {Iqr(g.Select(u => u.Weapons.Max(w => w.MaxRange)))} | {Iqr(g.Select(u => u.Weapons.Count))} |");
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

            // 射程ごと（マップ兵器を除く、攻撃するもの）
            var sw = ws.Where(w => !IsMap(w) && w.Power > 0).ToList();
            Console.WriteLine();
            Console.WriteLine("## 武器の射程（マップ兵器を除く。25%〜75%（中央値））");
            Console.WriteLine();
            Console.WriteLine("| 最大射程 | 件数 | 割合 | 最小射程 2 以上 | 攻撃力 | 命中補正 | 移動後に使える（Ｐ） |");
            Console.WriteLine("|---|--:|--:|--:|---|---|--:|");
            foreach (var (label, f) in new (string, Func<WeaponData, bool>)[] {
                ("1", w => w.MaxRange == 1), ("2", w => w.MaxRange == 2), ("3", w => w.MaxRange == 3),
                ("4", w => w.MaxRange == 4), ("5", w => w.MaxRange == 5), ("6 以上", w => w.MaxRange >= 6) })
            {
                var g = sw.Where(f).ToList();
                if (g.Count == 0) continue;
                Console.WriteLine($"| {label} | {g.Count} | {Pct(g.Count, sw.Count)} | {Pct(g.Count(w => w.MinRange >= 2), g.Count)} | {Iqr(g.Select(w => w.Power))} | {Iqr(g.Select(w => w.Precision))} | {Pct(g.Count(w => w.Class.Contains("Ｐ")), g.Count)} |");
            }
            Console.WriteLine();
            Console.WriteLine("ユニットごとの射程 3 以上の武器（マップ兵器を除く）を持つ割合（ユニットの経験値の帯）");
            Console.WriteLine();
            Console.WriteLine("| ユニットの経験値 | 割合 |");
            Console.WriteLine("|---|--:|");
            foreach (var b in UnitBands)
            {
                var g = us.Where(u => ExpBand(u.ExpValue) == b).ToList();
                if (g.Count == 0) continue;
                Console.WriteLine($"| {b} | {Pct(g.Count(u => u.Weapons.Any(w => !IsMap(w) && w.Power > 0 && w.MaxRange >= 3)), g.Count)} |");
            }
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
            Console.WriteLine("| 種類 | 件数（攻撃するもの） | 攻撃力 | 最大射程 | EN | 気力 | 識の割合 | 弾数制の割合 |");
            Console.WriteLine("|---|--:|---|---|---|---|--:|--:|");
            var ws = us.SelectMany(u => u.Weapons).ToList();
            foreach (var k in MapKinds)
            {
                var g = ws.Where(w => w.Class.Contains(k) && w.Power > 0).ToList();
                if (g.Count == 0) continue;
                Console.WriteLine($"| {k} | {g.Count} | {Iqr(g.Select(w => w.Power))} | {Iqr(g.Select(w => w.MaxRange))} | {Iqr(g.Where(w => w.ENConsumption > 0).Select(w => w.ENConsumption))} | {Iqr(g.Where(w => w.NecessaryMorale > 0).Select(w => w.NecessaryMorale))} | {Pct(g.Count(w => w.Class.Contains("識")), g.Count)} | {Pct(g.Count(w => w.Bullet > 0), g.Count)} |");
            }
            var ratio = us
                .Where(u => u.Weapons.Any(w => IsMap(w) && w.Power > 0) && u.Weapons.Any(w => !IsMap(w) && w.Power > 0))
                .Select(u => (int)(100.0 * u.Weapons.Where(IsMap).Max(w => w.Power) / u.Weapons.Where(w => !IsMap(w)).Max(w => w.Power)));
            Console.WriteLine();
            Console.WriteLine($"マップ兵器の最大攻撃力 ÷ 単体武器の最大攻撃力（%）: {Iqr(ratio)}");

            // 範囲のレベル（Ｍ扇・Ｍ投）
            foreach (var k in new[] { "Ｍ扇", "Ｍ投" })
            {
                var g = ws.Where(w => w.Class.Contains(k) && w.Power > 0).ToList();
                if (g.Count == 0) continue;
                Console.WriteLine();
                Console.WriteLine($"{k} の範囲のレベルごと（25%〜75%（中央値））");
                Console.WriteLine();
                Console.WriteLine("| レベル | 件数 | 攻撃力 | 最大射程 | EN | 気力 |");
                Console.WriteLine("|---|--:|---|---|---|---|");
                foreach (var lg in g.GroupBy(w => MapLevel(w, k)).OrderBy(x => x.Key))
                {
                    var a = lg.ToList();
                    Console.WriteLine($"| {(lg.Key < 0 ? "書かれていない" : "L" + lg.Key)} | {a.Count} | {Iqr(a.Select(w => w.Power))} | {Iqr(a.Select(w => w.MaxRange))} | {Iqr(a.Where(w => w.ENConsumption > 0).Select(w => w.ENConsumption))} | {Iqr(a.Where(w => w.NecessaryMorale > 0).Select(w => w.NecessaryMorale))} |");
                }
            }
        }

        // 「Ｍ投L2」のような属性のレベル。書かれていなければ -1
        private static int MapLevel(WeaponData w, string kind)
        {
            var m = Regex.Match(w.Class, kind + "L(\\d+)");
            return m.Success ? int.Parse(m.Groups[1].Value) : -1;
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


        // ---- sp-mix

        private static readonly string[] SpCategories = { "攻撃", "命中", "回避", "防御", "気力", "移動・射程", "行動", "支援", "妨害", "その他" };

        // SP の効果の分類。複合の SP（スペシャルパワー=…）は中身をたどる
        private static HashSet<string> SpCategory(Data d, string name, int depth = 0)
        {
            var r = new HashSet<string>();
            if (depth > 3 || !d.Src.SPDList.IsDefined(name)) { r.Add("その他"); return r; }
            var sp = d.Src.SPDList.Item(name);
            var target = sp.TargetType ?? "";
            if (target.Contains("味方")) { r.Add("支援"); return r; }
            if (target.Contains("敵")) { r.Add(sp.Effects.All(e => (e.strEffectType ?? "") == "偵察") ? "その他" : "妨害"); return r; }
            foreach (var e in sp.Effects)
            {
                var t = e.strEffectType ?? "";
                if (t == "スペシャルパワー") { r.UnionWith(SpCategory(d, e.strEffectData, depth + 1)); continue; }
                if (t.Contains("ダメージ増加") || t.Contains("クリティカル率増加") || t.Contains("無効化") && !t.Contains("被")) r.Add("攻撃");
                else if (t.Contains("命中")) r.Add("命中");
                else if (t.Contains("回避")) r.Add("回避");
                else if (t.Contains("装甲") || t.Contains("被ダメージ") || t.Contains("ＨＰ回復")) r.Add("防御");
                else if (t.Contains("気力増加")) r.Add("気力");
                else if (t.Contains("移動") || t.Contains("射程")) r.Add("移動・射程");
                else if (t.Contains("行動数")) r.Add("行動");
            }
            if (r.Count == 0) r.Add("その他");
            return r;
        }

        private static void SpMix(Data d)
        {
            var ps = Pilots(d).Where(p => !p.Name.Contains("(ザコ)") && !p.Name.Contains("(汎用)") && p.ExpValue >= 120).ToList();
            Console.WriteLine($"# {d.Name} SP の効果の分類ごとの持ち方（経験値 120 以上の名前付きのパイロット）");
            Console.WriteLine();
            Console.WriteLine("分類は sp.txt の効果から決める（複合の SP は中身をたどる）: 攻撃（ダメージ増加・クリティカル・防御無効化）、命中、回避、防御（装甲・被ダメージ低下・自分の HP 回復）、気力、移動・射程、行動（行動数増加）、支援（味方が対象）、妨害（敵が対象）、その他（資金・経験値・偵察など）");
            Console.WriteLine();
            Console.WriteLine("分類ごとの、その分類の SP を持つ割合と、持つ数の平均（帯ごと）");
            Console.WriteLine();
            Console.WriteLine("| 層 | " + string.Join(" | ", SpCategories) + " |");
            Console.WriteLine("|---|" + string.Concat(SpCategories.Select(_ => "---|")));
            foreach (var b in PilotBands.Take(4))
            {
                var g = ps.Where(p => PilotBand(p) == b).ToList();
                var cats = g.Select(p => p.DefinedSpecialPowers().Select(x => SpCategory(d, x.Name)).ToList()).ToList();
                Console.WriteLine($"| {b} | " + string.Join(" | ", SpCategories.Select(c =>
                {
                    var have = cats.Count(l => l.Any(x => x.Contains(c)));
                    var avg = cats.Average(l => l.Count(x => x.Contains(c)));
                    return $"{Pct(have, g.Count)}（{avg:F1}）";
                })) + " |");
            }
            Console.WriteLine();
            Console.WriteLine("命中・回避に効く SP（集中・必中・ひらめき・心眼など）の数");
            Console.WriteLine();
            Console.WriteLine("| 層 | 0 | 1 | 2 | 3 | 4 以上 |");
            Console.WriteLine("|---|--:|--:|--:|--:|--:|");
            foreach (var b in PilotBands.Take(4))
            {
                var g = ps.Where(p => PilotBand(p) == b).ToList();
                var n = g.Select(p => p.DefinedSpecialPowers().Count(x => { var c = SpCategory(d, x.Name); return c.Contains("命中") || c.Contains("回避"); })).ToList();
                Console.WriteLine($"| {b} | {Pct(n.Count(x => x == 0), n.Count)} | {Pct(n.Count(x => x == 1), n.Count)} | {Pct(n.Count(x => x == 2), n.Count)} | {Pct(n.Count(x => x == 3), n.Count)} | {Pct(n.Count(x => x >= 4), n.Count)} |");
            }
            Console.WriteLine();
            Console.WriteLine("持つ SP の分類の種類の数");
            Console.WriteLine();
            Console.WriteLine("| 層 | 25%〜75%（中央値） |");
            Console.WriteLine("|---|---|");
            foreach (var b in PilotBands.Take(4))
            {
                var g = ps.Where(p => PilotBand(p) == b).ToList();
                Console.WriteLine($"| {b} | {Iqr(g.Select(p => p.DefinedSpecialPowers().SelectMany(x => SpCategory(d, x.Name)).Distinct().Count()))} |");
            }
        }

        // ---- skills

        private static void Skills(Data d)
        {
            var ps = Pilots(d).Where(p => !p.Name.Contains("(ザコ)")).ToList();
            Console.WriteLine($"# {d.Name} 特殊能力");
            Console.WriteLine();
            Console.WriteLine("## パイロットの特殊能力を持つ割合（レベル 30 までに覚えるもの。上位 20）");
            Console.WriteLine();
            var names = ps.SelectMany(p => p.Skills.Where(k => k.NecessaryLevel <= 30).Select(k => k.Name).Distinct())
                .GroupBy(x => x).OrderByDescending(g => g.Count()).Take(20).Select(g => g.Key).ToList();
            var bands = PilotBands.Where(b => b != "ザコ").ToList();
            Console.WriteLine("| 特殊能力 | " + string.Join(" | ", bands) + " |");
            Console.WriteLine("|---|" + string.Concat(bands.Select(_ => "--:|")));
            foreach (var n in names)
            {
                Console.WriteLine($"| {n} | " + string.Join(" | ", bands.Select(b =>
                {
                    var g = ps.Where(p => PilotBand(p) == b).ToList();
                    return Pct(g.Count(p => p.Skills.Any(k => k.Name == n && k.NecessaryLevel <= 30)), g.Count);
                })) + " |");
            }

            var us = Units(d);
            Console.WriteLine();
            Console.WriteLine("## ユニットの特殊能力を持つ割合（上位 25）");
            Console.WriteLine();
            var fnames = us.SelectMany(u => u.Features.Select(f => f.Name).Distinct())
                .GroupBy(x => x).OrderByDescending(g => g.Count()).Take(25).Select(g => g.Key).ToList();
            Console.WriteLine("| 特殊能力 | " + string.Join(" | ", UnitBands) + " | レベル（25%〜75%（中央値）） |");
            Console.WriteLine("|---|" + string.Concat(UnitBands.Select(_ => "--:|")) + "---|");
            foreach (var n in fnames)
            {
                var lv = us.SelectMany(u => u.Features.Where(f => f.Name == n && f.HasLevel).Select(f => (int)Math.Round(f.Level))).ToList();
                Console.WriteLine($"| {n} | " + string.Join(" | ", UnitBands.Select(b =>
                {
                    var g = us.Where(u => ExpBand(u.ExpValue) == b).ToList();
                    return Pct(g.Count(u => u.Features.Any(f => f.Name == n)), g.Count);
                })) + $" | {(lv.Count > 0 ? Iqr(lv) : "-")} |");
            }
        }

        // ---- endurance

        // 1 回の出撃で撃てる回数（弾数か、EN ÷ 消費 EN。どちらも無ければ制限なし = -1）
        private static int Uses(UnitData u, WeaponData w)
        {
            if (w.Bullet > 0) return w.ENConsumption > 0 ? Math.Min(w.Bullet, u.EN / w.ENConsumption) : w.Bullet;
            if (w.ENConsumption > 0) return u.EN / w.ENConsumption;
            return -1;
        }

        private static void Endurance(Data d)
        {
            var us = Units(d);
            Console.WriteLine($"# {d.Name} 継戦能力");
            Console.WriteLine();
            Console.WriteLine("最大攻撃力の武器（マップ兵器を除く）を、1 回の出撃で撃てる回数（弾数か、ユニットの EN ÷ 消費 EN）");
            Console.WriteLine();
            Console.WriteLine("| ユニットの経験値 | 体数 | 制限なし | 1 回 | 2 回 | 3〜4 回 | 5 回以上 | 最大攻撃力の武器の気力 | 気力の条件あり |");
            Console.WriteLine("|---|--:|--:|--:|--:|--:|--:|---|--:|");
            foreach (var b in UnitBands)
            {
                var g = us.Where(u => ExpBand(u.ExpValue) == b && u.Weapons.Any(w => !IsMap(w) && w.Power > 0)).ToList();
                if (g.Count == 0) continue;
                var top = g.Select(u => (u, w: u.Weapons.Where(w => !IsMap(w) && w.Power > 0).OrderByDescending(w => w.Power).First())).ToList();
                var n = top.Select(x => Uses(x.u, x.w)).ToList();
                Console.WriteLine($"| {b} | {g.Count} | {Pct(n.Count(x => x < 0), n.Count)} | {Pct(n.Count(x => x == 1 || x == 0), n.Count)} | {Pct(n.Count(x => x == 2), n.Count)} | {Pct(n.Count(x => x >= 3 && x <= 4), n.Count)} | {Pct(n.Count(x => x >= 5), n.Count)} | {Iqr(top.Where(x => x.w.NecessaryMorale > 0).Select(x => x.w.NecessaryMorale))} | {Pct(top.Count(x => x.w.NecessaryMorale > 0), top.Count)} |");
            }
        }

        // ---- growth

        private static void Growth(Data d)
        {
            var ps = Pilots(d).Where(p => !p.Name.Contains("(ザコ)") && !p.Name.Contains("(汎用)")).ToList();
            string[] sps = { "熱血", "魂", "覚醒", "集中", "ひらめき", "必中", "信頼", "激励" };
            Console.WriteLine($"# {d.Name} SP を覚えるレベル（持つパイロットのみ。25%〜75%（中央値））");
            Console.WriteLine();
            Console.WriteLine("| 層 | " + string.Join(" | ", sps) + " |");
            Console.WriteLine("|---|" + string.Concat(sps.Select(_ => "---|")));
            foreach (var b in PilotBands.Take(5))
            {
                var g = ps.Where(p => PilotBand(p) == b).ToList();
                if (g.Count == 0) continue;
                Console.WriteLine($"| {b} | " + string.Join(" | ", sps.Select(x => Iqr(g.SelectMany(p => p.DefinedSpecialPowers().Where(s => s.Name == x).Select(s => s.NecessaryLevel))))) + " |");
            }
            // 覚えるレベルの刻み（レベル 1 を除く）
            var lvs = ps.SelectMany(p => p.DefinedSpecialPowers().Select(x => x.NecessaryLevel).Where(l => l > 1)).ToList();
            var named = ps.Where(p => p.DefinedSpecialPowers().Count(x => x.NecessaryLevel > 1) >= 3).ToList();
            Console.WriteLine();
            Console.WriteLine("覚えるレベルの刻み（レベル 1 を除く。名前付きのパイロット）");
            Console.WriteLine();
            Console.WriteLine("| 項目 | 割合 |");
            Console.WriteLine("|---|--:|");
            Console.WriteLine($"| 覚えるレベルのうち 5 の倍数 | {Pct(lvs.Count(l => l % 5 == 0), lvs.Count)} |");
            Console.WriteLine($"| レベル 1 以外を 3 つ以上覚え、その全てが 5 の倍数のパイロット | {Pct(named.Count(p => p.DefinedSpecialPowers().Where(x => x.NecessaryLevel > 1).All(x => x.NecessaryLevel % 5 == 0)), named.Count)} |");
            var gaps = ps.SelectMany(p => { var l = p.DefinedSpecialPowers().Select(x => x.NecessaryLevel).OrderBy(x => x).ToList(); return l.Zip(l.Skip(1), (a, b) => b - a); }).ToList();
            Console.WriteLine($"| 続けて覚える SP のレベルの間隔（25%〜75%（中央値）） | {Iqr(gaps)} |");
            Console.WriteLine();
            Console.WriteLine("性格ごとの、熱血・魂のどちらかを覚えるレベル（早いほう。25%〜75%（中央値））");
            Console.WriteLine();
            Console.WriteLine("| 性格 | 人数 | 熱血か魂を持つ | 覚えるレベル |");
            Console.WriteLine("|---|--:|--:|---|");
            foreach (var per in new[] { "超強気", "強気", "普通", "弱気", "機械" })
            {
                var g = ps.Where(p => p.Personality == per && p.ExpValue >= 120).ToList();
                if (g.Count == 0) continue;
                var lv = g.Select(p => p.DefinedSpecialPowers().Where(s => s.Name == "熱血" || s.Name == "魂").Select(s => s.NecessaryLevel).DefaultIfEmpty(-1).Min()).Where(x => x >= 0).ToList();
                Console.WriteLine($"| {per} | {g.Count} | {Pct(lv.Count, g.Count)} | {Iqr(lv)} |");
            }
        }

        // ---- spread

        private static void Spread(Data d)
        {
            var ps = Pilots(d).Where(p => !p.Name.Contains("(ザコ)") && !p.Name.Contains("(汎用)") && p.ExpValue >= 120).ToList();
            Func<PilotData, int>[] f = { p => p.Infight, p => p.Shooting, p => p.Hit, p => p.Dodge, p => p.Technique, p => p.Intuition };
            Console.WriteLine($"# {d.Name} 個性の偏り（経験値 120 以上の名前付きのパイロット）");
            Console.WriteLine();
            Console.WriteLine("同じ帯の中で 75% 以上の能力（得意）と 25% 以下の能力（苦手）の数");
            Console.WriteLine();
            var q = new Dictionary<string, (int lo, int hi)[]>();
            foreach (var b in PilotBands.Take(4))
            {
                var g = ps.Where(p => PilotBand(p) == b).ToList();
                q[b] = f.Select(fn => (Quantile(g.Select(fn), .25), Quantile(g.Select(fn), .75))).ToArray();
            }
            var rows = ps.Select(p =>
            {
                var t = q[PilotBand(p)];
                var hi = Enumerable.Range(0, 6).Count(i => f[i](p) > t[i].hi);
                var lo = Enumerable.Range(0, 6).Count(i => f[i](p) < t[i].lo);
                return (hi, lo);
            }).ToList();
            Console.WriteLine("| 得意＼苦手 | 0 | 1 | 2 | 3 以上 |");
            Console.WriteLine("|---|--:|--:|--:|--:|");
            foreach (var h in new[] { 0, 1, 2, 3 })
            {
                Console.WriteLine($"| {(h == 3 ? "3 以上" : h.ToString())} | " + string.Join(" | ", new[] { 0, 1, 2, 3 }.Select(l =>
                    Pct(rows.Count(r => (h == 3 ? r.hi >= 3 : r.hi == h) && (l == 3 ? r.lo >= 3 : r.lo == l)), rows.Count))) + " |");
            }
            Console.WriteLine();
            Console.WriteLine($"得意も苦手も無い（全能力が 25%〜75% に収まる）: {Pct(rows.Count(r => r.hi == 0 && r.lo == 0), rows.Count)}");
            Console.WriteLine();
            Console.WriteLine("各能力の 25%〜75% の幅（帯ごと）");
            Console.WriteLine();
            Console.WriteLine("| 層 | 格闘 | 射撃 | 命中 | 回避 | 技量 | 反応 |");
            Console.WriteLine("|---|--:|--:|--:|--:|--:|--:|");
            foreach (var b in q.Keys) Console.WriteLine($"| {b} | " + string.Join(" | ", q[b].Select(x => x.hi - x.lo)) + " |");
        }

        // ---- types

        private static void Types(Data d, string listPath)
        {
            var us = Units(d).Where(u => u.ExpValue >= 120).ToList();
            Console.WriteLine($"# {d.Name} 系統（ユニットの経験値 120 以上。同じ帯の中での位置）");
            Console.WriteLine();
            Console.WriteLine("- リアル系: 運動性が 75% 以上で、HP・装甲がどちらも 50% 以下");
            Console.WriteLine("- スーパー系: HP か装甲が 75% 以上で、運動性が 25% 以下");
            Console.WriteLine("- グレー系: 最大攻撃力が 75% 以上で、リアル系・スーパー系に当たらない");
            Console.WriteLine();
            var med = new Dictionary<string, (int[] q25, int[] q50, int[] q75)>();
            Func<UnitData, int>[] f = { u => u.HP, u => u.Armor, u => u.Mobility, u => u.Weapons.Max(w => w.Power) };
            foreach (var b in UnitBands.Take(4))
            {
                var g = us.Where(u => ExpBand(u.ExpValue) == b).ToList();
                med[b] = (f.Select(fn => Quantile(g.Select(fn), .25)).ToArray(), f.Select(fn => Quantile(g.Select(fn), .5)).ToArray(), f.Select(fn => Quantile(g.Select(fn), .75)).ToArray());
            }
            string Type(UnitData u)
            {
                var (q25, q50, q75) = med[ExpBand(u.ExpValue)];
                if (u.Mobility >= q75[2] && u.HP <= q50[0] && u.Armor <= q50[1]) return "リアル系";
                if ((u.HP >= q75[0] || u.Armor >= q75[1]) && u.Mobility <= q25[2]) return "スーパー系";
                if (u.Weapons.Max(w => w.Power) >= q75[3]) return "グレー系";
                return "どれにも当たらない";
            }
            var t = us.Select(u => (u, type: Type(u))).ToList();
            Console.WriteLine("| 系統 | 割合 | HP | 装甲 | 運動性 | 最大攻撃力 |");
            Console.WriteLine("|---|--:|---|---|---|---|");
            foreach (var g in t.GroupBy(x => x.type).OrderBy(g => g.Key))
            {
                var a = g.Select(x => x.u).ToList();
                Console.WriteLine($"| {g.Key} | {Pct(a.Count, t.Count)} | {Iqr(a.Select(u => u.HP))} | {Iqr(a.Select(u => u.Armor))} | {Iqr(a.Select(u => u.Mobility))} | {Iqr(a.Select(u => u.Weapons.Max(w => w.Power)))} |");
            }
            if (listPath == null) return;
            Console.WriteLine();
            Console.WriteLine("格の一覧のキャラのユニットの系統");
            Console.WriteLine();
            Console.WriteLine("| 格 | ユニット | 系統 | HP | 装甲 | 運動性 | 最大攻撃力 |");
            Console.WriteLine("|---|---|---|--:|--:|--:|--:|");
            foreach (var r in File.ReadAllLines(listPath).Where(l => l.Trim() != "" && !l.StartsWith("#")).Select(l => l.Split('\t')).Where(x => x.Length >= 2))
            {
                var name = r.Length > 2 && r[2] != "" ? r[2] : r[1];
                var u = t.FirstOrDefault(x => x.u.Name == name).u;
                if (u == null) continue;
                Console.WriteLine($"| {r[0]} | {u.Name} | {Type(u)} | {u.HP} | {u.Armor} | {u.Mobility} | {u.Weapons.Max(w => w.Power)} |");
            }
        }

        // ---- map-types

        private static void MapTypes(Data d)
        {
            var us = Units(d).Where(u => u.Weapons.Any(w => IsMap(w) && w.Power > 0)).ToList();
            Console.WriteLine($"# {d.Name} マップ兵器の持たせ方の型（攻撃するマップ兵器を持つユニット {us.Count} 体）");
            Console.WriteLine();
            Console.WriteLine("- 補助型: 最も強いマップ兵器が、単体武器の最大攻撃力の 9 割未満");
            Console.WriteLine("- 切り札型: 補助型に当たらず、攻撃するマップ兵器が 1 つだけで、2 回以下しか撃てない");
            Console.WriteLine("- マップ兵器屋: 補助型に当たらず、攻撃するマップ兵器が 2 つ以上か、3 回以上撃てる（マップ兵器が主力）");
            Console.WriteLine();
            string Type(UnitData u)
            {
                var mw = u.Weapons.Where(w => IsMap(w) && w.Power > 0).ToList();
                var top = mw.OrderByDescending(w => w.Power).First();
                var single = u.Weapons.Where(w => !IsMap(w) && w.Power > 0).Select(w => w.Power).DefaultIfEmpty(0).Max();
                if (top.Power < single * 0.9) return "補助型";
                var n = Uses(u, top);
                if (mw.Count == 1 && n >= 0 && n <= 2) return "切り札型";
                return "マップ兵器屋";
            }
            var t = us.Select(u => (u, type: Type(u))).ToList();
            Console.WriteLine("| 型 | 割合 | 最も強いマップ兵器の攻撃力 | 単体武器の最大攻撃力 | 最も強いマップ兵器の回数 | 気力 | 識あり |");
            Console.WriteLine("|---|--:|---|---|---|---|--:|");
            foreach (var g in t.GroupBy(x => x.type).OrderBy(g => g.Key))
            {
                var a = g.Select(x => (x.u, w: x.u.Weapons.Where(w => IsMap(w) && w.Power > 0).OrderByDescending(w => w.Power).First())).ToList();
                Console.WriteLine($"| {g.Key} | {Pct(a.Count, t.Count)} | {Iqr(a.Select(x => x.w.Power))} | {Iqr(a.Select(x => x.u.Weapons.Where(w => !IsMap(w) && w.Power > 0).Select(w => w.Power).DefaultIfEmpty(0).Max()))} | {Iqr(a.Select(x => Uses(x.u, x.w)).Where(n => n >= 0))} | {Iqr(a.Where(x => x.w.NecessaryMorale > 0).Select(x => x.w.NecessaryMorale))} | {Pct(a.Count(x => x.w.Class.Contains("識")), a.Count)} |");
            }
        }

        // ---- roles

        // tools/pack-analysis の結果（Markdown）から、格の一覧のパイロットの組の評価を拾う
        private static void Roles(string analysisPath, string listPath)
        {
            var tiers = File.ReadAllLines(listPath).Where(l => l.Trim() != "" && !l.StartsWith("#")).Select(l => l.Split('\t'))
                .Where(t => t.Length >= 2).GroupBy(t => t[1]).ToDictionary(g => g.Key, g => g.First()[0]);
            var grades = new Dictionary<string, string[]>();
            foreach (var line in File.ReadAllLines(analysisPath))
            {
                var c = line.Trim().Trim('|').Split('|').Select(x => x.Trim()).ToArray();
                if (c.Length != 10 || !Regex.IsMatch(c[3], "^[SABCD-]$")) continue;
                var pilot = Regex.Replace(c[0].Split('／')[0], "（サブ:.*", "");
                if (tiers.ContainsKey(pilot) && !grades.ContainsKey(pilot)) grades[pilot] = c.Skip(3).Take(6).ToArray();
            }
            Console.WriteLine("# 格ごとの、組の評価で決め手（S・A）を持つ割合");
            Console.WriteLine();
            Console.WriteLine("使いどころ: ボス戦・ザコ掃討・回避・囮・遊撃・長距離・支援。評価は同じ経験値の帯の中での順位（S 上位 5%、A 上位 25%）");
            Console.WriteLine();
            Console.WriteLine("| 格 | 人数（評価あり） | S を持つ | S か A を持つ | どれも B 以下 |");
            Console.WriteLine("|---|--:|--:|--:|--:|");
            foreach (var g in grades.GroupBy(x => tiers[x.Key]))
            {
                var a = g.ToList();
                Console.WriteLine($"| {g.Key} | {a.Count} | {Pct(a.Count(x => x.Value.Contains("S")), a.Count)} | {Pct(a.Count(x => x.Value.Any(v => v == "S" || v == "A")), a.Count)} | {Pct(a.Count(x => !x.Value.Any(v => v == "S" || v == "A")), a.Count)} |");
            }
            Console.WriteLine();
            Console.WriteLine("| 格 | パイロット | ボス戦 | ザコ掃討 | 回避・囮 | 遊撃 | 長距離 | 支援 |");
            Console.WriteLine("|---|---|---|---|---|---|---|---|");
            foreach (var x in grades.OrderBy(x => tiers[x.Key])) Console.WriteLine($"| {tiers[x.Key]} | {x.Key} | " + string.Join(" | ", x.Value) + " |");
        }
    }

    // パックの読み込み。読み込みの順を固定する（sp.txt を先に、ほかはパスの順）
    public class Data
    {
        public SRC Src;
        public string Name;
        public Dictionary<string, string> UnitWork = new Dictionary<string, string>();

        public string Root;
        public List<MessageBlock> Messages = new List<MessageBlock>();

        // spFile: 作品のフォルダに sp.txt が無いとき、先に読む sp.txt
        public static Data Load(string root, string spFile = null)
        {
            var d = new Data
            {
                Src = new SRC { GUI = new MockGUI(), FileSystem = new LocalFileSystem() },
                Name = Path.GetFileName(Path.GetFullPath(root).TrimEnd('/', '\\')),
                Root = root,
            };
            if (spFile != null)
            {
                using var s = File.OpenRead(spFile);
                d.Src.SPDList.Load("sp.txt", s);
            }
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
                    else if (name == "pilot_message.txt") d.Messages.AddRange(MessageBlock.Parse(f));
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
