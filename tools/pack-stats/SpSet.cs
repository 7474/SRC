// SP の構成の集計（sp-set）
using SRCCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PackStats
{
    public static partial class Program
    {
        private static bool IsNamed(PilotData p) => !p.Name.Contains("(ザコ)") && !p.Name.Contains("(汎用)");

        private static int SpCost(Data d, PilotDataSpecialPower x) =>
            x.SPConsumption > 0 ? x.SPConsumption : d.Src.SPDList.IsDefined(x.Name) ? d.Src.SPDList.Item(x.Name).SPConsumption : 0;

        private static bool IsHitDodge(Data d, string sp)
        {
            var c = SpCategory(d, sp);
            return c.Contains("命中") || c.Contains("回避");
        }

        private static void SpSet(Data d, string listPath)
        {
            var all = Pilots(d);
            var named = all.Where(p => IsNamed(p) && p.ExpValue >= 120).ToList();
            var bands = PilotBands.Where(b => b != "120未満").ToList();
            Console.WriteLine($"# {d.Name} SP の構成");
            Console.WriteLine();

            Console.WriteLine("## SP の数・消費（25%〜75%（中央値））");
            Console.WriteLine();
            Console.WriteLine("| 層 | 人数 | SP の数 | 5 つ以下 | 6 つ | 7 つ以上 | SP の最大値 | 消費の合計 | 最も重い SP の消費 |");
            Console.WriteLine("|---|--:|---|--:|--:|--:|---|---|---|");
            foreach (var b in bands)
            {
                var g = all.Where(p => PilotBand(p) == b).ToList();
                if (g.Count == 0) continue;
                var n = g.Select(p => p.DefinedSpecialPowers().Count).ToList();
                var cost = g.Where(p => p.DefinedSpecialPowers().Any()).Select(p => p.DefinedSpecialPowers().Sum(x => SpCost(d, x))).ToList();
                var top = g.Where(p => p.DefinedSpecialPowers().Any()).Select(p => p.DefinedSpecialPowers().Max(x => SpCost(d, x))).ToList();
                Console.WriteLine($"| {b} | {g.Count} | {Iqr(n)} | {Pct(n.Count(x => x <= 5), n.Count)} | {Pct(n.Count(x => x == 6), n.Count)} | {Pct(n.Count(x => x >= 7), n.Count)} | {Iqr(g.Select(p => p.SP))} | {Iqr(cost)} | {Iqr(top)} |");
            }

            Console.WriteLine();
            Console.WriteLine("## SP ごとの持つ割合（経験値 120 以上の名前付きで持つ人数の上位 40 と、切り札になる SP）");
            Console.WriteLine();
            Console.WriteLine("覚えるレベルは経験値 120 以上の名前付きで、持つパイロットのみ。");
            Console.WriteLine();
            var names = named.SelectMany(p => p.DefinedSpecialPowers().Select(x => x.Name).Distinct())
                .GroupBy(x => x).OrderByDescending(g => g.Count()).ThenBy(g => g.Key, StringComparer.Ordinal).Take(40).Select(g => g.Key).ToList();
            // 切り札になる SP は上位に入らなくても載せる
            foreach (var k in new[] { "魂", "覚醒", "勇気", "奇跡", "愛", "友情", "心眼", "気迫", "決意", "祈り", "再動" })
                if (!names.Contains(k) && named.Any(p => HasSp(p, k))) names.Add(k);
            Console.WriteLine("| SP | 消費 | 分類 | " + string.Join(" | ", bands) + " | 覚えるレベル | レベル 1 で覚える |");
            Console.WriteLine("|---|--:|---|" + string.Concat(bands.Select(_ => "--:|")) + "---|--:|");
            foreach (var n in names)
            {
                var cost = d.Src.SPDList.IsDefined(n) ? d.Src.SPDList.Item(n).SPConsumption.ToString() : "-";
                var lv = named.SelectMany(p => p.DefinedSpecialPowers().Where(x => x.Name == n).Select(x => x.NecessaryLevel)).ToList();
                Console.WriteLine($"| {n} | {cost} | {string.Join("・", SpCategory(d, n).OrderBy(c => Array.IndexOf(SpCategories, c)))} | " + string.Join(" | ", bands.Select(b =>
                {
                    var g = all.Where(p => PilotBand(p) == b).ToList();
                    return Pct(g.Count(p => HasSp(p, n)), g.Count);
                })) + $" | {Iqr(lv)} | {Pct(lv.Count(l => l <= 1), lv.Count)} |");
            }

            Console.WriteLine();
            Console.WriteLine("## レベル 1 で覚える SP（経験値 120 以上の名前付き）");
            Console.WriteLine();
            var first = named.SelectMany(p => p.DefinedSpecialPowers().Where(x => x.NecessaryLevel <= 1).Select(x => x.Name)).ToList();
            Console.WriteLine($"レベル 1 で覚える SP の数: {Iqr(named.Select(p => p.DefinedSpecialPowers().Count(x => x.NecessaryLevel <= 1)))}");
            Console.WriteLine();
            Console.WriteLine("| 分類 | レベル 1 の SP に占める割合 |");
            Console.WriteLine("|---|--:|");
            foreach (var c in SpCategories)
            {
                Console.WriteLine($"| {c} | {Pct(first.Count(x => SpCategory(d, x).Contains(c)), first.Count)} |");
            }

            Console.WriteLine();
            Console.WriteLine("## よく組み合わされる SP の対（経験値 120 以上の名前付き。両方を持つ割合の上位 20）");
            Console.WriteLine();
            var pairs = named.SelectMany(p =>
            {
                var l = p.DefinedSpecialPowers().Select(x => x.Name).Distinct().OrderBy(x => x, StringComparer.Ordinal).ToList();
                return l.SelectMany((a, i) => l.Skip(i + 1).Select(b => a + "＋" + b));
            }).GroupBy(x => x).OrderByDescending(g => g.Count()).ThenBy(g => g.Key, StringComparer.Ordinal).Take(20);
            Console.WriteLine("| 組み合わせ | 割合 |");
            Console.WriteLine("|---|--:|");
            foreach (var g in pairs) Console.WriteLine($"| {g.Key} | {Pct(g.Count(), named.Count)} |");

            if (listPath == null) return;
            var rows = File.ReadAllLines(listPath).Where(l => l.Trim() != "" && !l.StartsWith("#")).Select(l => l.Split('\t'))
                .Where(t => t.Length >= 2 && d.Src.PDList.IsDefined(t[1])).Select(t => (tier: t[0], p: d.Src.PDList.Item(t[1]))).ToList();
            Console.WriteLine();
            Console.WriteLine("## 格ごとの SP（格の一覧のキャラ）");
            Console.WriteLine();
            Console.WriteLine("| 格 | 人数 | SP の数 | 命中・回避の SP の数 | 持つ割合の高い SP（上位 12） |");
            Console.WriteLine("|---|--:|---|---|---|");
            foreach (var g in rows.GroupBy(r => r.tier))
            {
                var ps = g.Select(r => r.p).ToList();
                var top = ps.SelectMany(p => p.DefinedSpecialPowers().Select(x => x.Name).Distinct()).GroupBy(x => x)
                    .OrderByDescending(x => x.Count()).ThenBy(x => x.Key, StringComparer.Ordinal).Take(12)
                    .Select(x => $"{x.Key} {100 * x.Count() / ps.Count}%");
                Console.WriteLine($"| {g.Key} | {ps.Count} | {Range(ps.Select(p => p.DefinedSpecialPowers().Count))} | {Range(ps.Select(p => p.DefinedSpecialPowers().Count(x => IsHitDodge(d, x.Name))))} | {string.Join("、", top)} |");
            }
            Console.WriteLine();
            Console.WriteLine("| 格 | パイロット | SP（覚えるレベル） |");
            Console.WriteLine("|---|---|---|");
            foreach (var (tier, p) in rows)
            {
                Console.WriteLine($"| {tier} | {p.Name} | {string.Join("・", p.DefinedSpecialPowers().Select(x => $"{x.Name}{x.NecessaryLevel}"))} |");
            }
        }
    }
}
