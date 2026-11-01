// パイロットのメッセージ（pilot_message.txt）の集計（messages）
using SRCCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PackStats
{
    public class MessageBlock
    {
        public string Pilot;
        public string File;
        public List<(string Situation, string Message)> Lines = new List<(string, string)>();

        // 名前の行（カンマを含まない行）の後に「シチュエーション, メッセージ」が続く
        public static IEnumerable<MessageBlock> Parse(string path)
        {
            MessageBlock cur = null;
            foreach (var raw in System.IO.File.ReadLines(path))
            {
                var l = raw.Trim().TrimStart('﻿');
                if (l == "" || l.StartsWith("#") || l.StartsWith("//")) continue;
                var i = l.IndexOf(',');
                if (i < 0)
                {
                    if (cur != null) yield return cur;
                    cur = new MessageBlock { Pilot = l, File = path };
                    continue;
                }
                cur?.Lines.Add((l.Substring(0, i).Trim(), l.Substring(i + 1).Trim()));
            }
            if (cur != null) yield return cur;
        }

        // 「攻撃(反撃)(対瀕死)」→ ("攻撃", ["反撃", "対瀕死"])
        public static (string Base, List<string> Subs) Split(string situation)
        {
            var m = Regex.Match(situation, @"^([^(（]*)(.*)$");
            var subs = Regex.Matches(m.Groups[2].Value, @"[(（]([^)）]*)[)）]").Select(x => x.Groups[1].Value).ToList();
            return (m.Groups[1].Value.Trim(), subs);
        }
    }

    public static partial class Program
    {
        // SRC のヘルプ（メッセージデータ・特殊能力）に載っているシチュエーション
        private static readonly HashSet<string> KnownSituations = new HashSet<string>
        {
            "回避", "切り払い", "迎撃", "反射", "当て身技", "阻止", "ダミー", "攻撃無効化", "ビーム無効化", "シールド防御",
            "バリア貫通", "バリア発動", "フィールド発動", "プロテクション発動", "自動反撃",
            "ダメージ小", "ダメージ中", "ダメージ大", "破壊", "脱出", "射程外", "かばう",
            "サポートアタック", "サポートガード", "攻撃", "格闘", "射撃", "かけ声",
            "分身", "修理", "補給", "合体", "分離", "破壊時分離", "変形", "ハイパーモード", "ノーマルモード",
            "復活", "自爆", "発進", "テレポート", "緊急テレポート", "ジャンプ", "再攻撃", "追加攻撃", "カウンター", "チャージ",
            "浄化", "混乱", "恐怖", "レベルアップ", "地上", "水上", "水中", "空中", "地中", "アビリティ",
        };

        // シチュエーションの分類
        private static string SituationKind(Data d, HashSet<string> weaponNames, string b)
        {
            if (KnownSituations.Contains(b)) return b;
            if (d.Src.SPDList.IsDefined(b)) return "SP 名";
            if (weaponNames.Contains(b)) return "武器名・アビリティ名";
            return "ヘルプに無いもの";
        }

        // 武器・アビリティ・ユニットの特殊能力の名前（特殊能力の発動もシチュエーションになる）
        private static HashSet<string> WeaponNames(Data d) => d.Src.UDList.Items
            .SelectMany(u => u.Weapons.Select(w => w.Name).Concat(u.Abilities.Select(a => a.Name)).Concat(u.Features.Select(f => f.Name)))
            .Concat(d.Src.PDList.Items.SelectMany(p => p.Abilities.Select(a => a.Name)))
            .ToHashSet();

        private static string MessageLayer(Data d, string pilot)
        {
            if (pilot.Contains("(ザコ)")) return "ザコ";
            if (pilot.Contains("(汎用)")) return "汎用";
            if (!d.Src.PDList.IsDefined(pilot)) return "パイロットデータに無い名前";
            return ExpBand(d.Src.PDList.Item(pilot).ExpValue);
        }

        private static readonly string[] MessageRows =
        {
            "回避", "ダメージ小", "ダメージ中", "ダメージ大", "破壊", "射程外", "攻撃", "格闘", "射撃", "武器名・アビリティ名",
            "(反撃)", "(とどめ)", "(対…)", "かけ声", "切り払い", "攻撃無効化", "サポートアタック", "サポートガード", "かばう", "脱出", "SP 名", "ヘルプに無いもの",
        };

        private static List<string> BlockKinds(Data d, HashSet<string> wn, MessageBlock m) =>
            m.Lines.SelectMany(x =>
            {
                var (b, subs) = MessageBlock.Split(x.Situation);
                var r = new List<string> { SituationKind(d, wn, b) };
                if (subs.Contains("反撃")) r.Add("(反撃)");
                if (subs.Contains("とどめ")) r.Add("(とどめ)");
                if (subs.Any(s => s.StartsWith("対"))) r.Add("(対…)");
                return r;
            }).ToList();

        private static void MessagesStats(Data d)
        {
            var wn = WeaponNames(d);
            var blocks = d.Messages.GroupBy(m => m.Pilot).Select(g => new MessageBlock { Pilot = g.Key, File = g.First().File, Lines = g.SelectMany(x => x.Lines).ToList() }).ToList();
            var byName = blocks.ToDictionary(b => b.Pilot);
            var works = blocks.Select(b => Path.GetDirectoryName(b.File)).Distinct().Count();
            var allLines = blocks.Sum(b => b.Lines.Count);
            Console.WriteLine($"# {d.Name} パイロットのメッセージ（{works} 作品、{blocks.Count} 人、{allLines} 行）");
            Console.WriteLine();

            Console.WriteLine("## 量（25%〜75%（中央値））");
            Console.WriteLine();
            Console.WriteLine("| 層 | パイロット | メッセージを持つ | 1 人の行数 |");
            Console.WriteLine("|---|--:|--:|---|");
            foreach (var b in PilotBands)
            {
                var g = Pilots(d).Where(p => PilotBand(p) == b).ToList();
                if (g.Count == 0) continue;
                var have = g.Where(p => byName.ContainsKey(p.Name)).ToList();
                Console.WriteLine($"| {b} | {g.Count} | {Pct(have.Count, g.Count)} | {Iqr(have.Select(p => byName[p.Name].Lines.Count))} |");
            }

            string[] layers = { "200以上", "180-199", "160-179", "120-159", "120未満", "汎用", "ザコ" };
            Console.WriteLine();
            Console.WriteLine("## シチュエーションを持つパイロットの割合（メッセージを持つパイロットのうち）");
            Console.WriteLine();
            Console.WriteLine("「武器名・アビリティ名」はパックのどこかのユニット・パイロットにある名前、「SP 名」は sp.txt にある名前（復活・自爆など SP と同名の特殊能力のシチュエーションを含む）、「ヘルプに無いもの」はそのどちらでもなく SRC のヘルプにも無い名前。");
            Console.WriteLine();
            var kinds = blocks.ToDictionary(b => b.Pilot, b => BlockKinds(d, wn, b));
            var layerOf = blocks.ToDictionary(b => b.Pilot, b => MessageLayer(d, b.Pilot));
            Console.WriteLine("| シチュエーション | " + string.Join(" | ", layers) + " | 行数の割合 |");
            Console.WriteLine("|---|" + string.Concat(layers.Select(_ => "--:|")) + "--:|");
            var lineKinds = kinds.Values.SelectMany(x => x).ToList();
            foreach (var r in MessageRows)
            {
                Console.WriteLine($"| {r} | " + string.Join(" | ", layers.Select(l =>
                {
                    var g = blocks.Where(b => layerOf[b.Pilot] == l).ToList();
                    return Pct(g.Count(b => kinds[b.Pilot].Contains(r)), g.Count);
                })) + $" | {Pct(lineKinds.Count(x => x == r), allLines)} |");
            }

            Console.WriteLine();
            Console.WriteLine("## シチュエーションごとの行数（持つパイロットのみ。経験値 120 以上の名前付き。25%〜75%（中央値））");
            Console.WriteLine();
            var named = blocks.Where(b => new[] { "200以上", "180-199", "160-179", "120-159" }.Contains(layerOf[b.Pilot])).ToList();
            Console.WriteLine("| シチュエーション | 行数 |");
            Console.WriteLine("|---|---|");
            foreach (var r in new[] { "回避", "ダメージ小", "ダメージ中", "ダメージ大", "破壊", "射程外", "攻撃", "武器名・アビリティ名", "SP 名" })
            {
                Console.WriteLine($"| {r} | {Iqr(named.Select(b => kinds[b.Pilot].Count(x => x == r)).Where(n => n > 0))} |");
            }

            Console.WriteLine();
            Console.WriteLine("## SP 名のシチュエーション（使うパイロットの数の上位 20）");
            Console.WriteLine();
            var spUse = blocks.SelectMany(b => b.Lines.Select(x => MessageBlock.Split(x.Situation).Base).Where(x => d.Src.SPDList.IsDefined(x)).Distinct())
                .GroupBy(x => x).OrderByDescending(g => g.Count()).ThenBy(g => g.Key, StringComparer.Ordinal).Take(20);
            Console.WriteLine("| SP | パイロット |");
            Console.WriteLine("|---|--:|");
            foreach (var g in spUse) Console.WriteLine($"| {g.Key} | {g.Count()} |");

            Console.WriteLine();
            Console.WriteLine("## ヘルプに無いシチュエーション（使うパイロットの数の上位 30）");
            Console.WriteLine();
            var unknown = blocks.SelectMany(b => b.Lines.Select(x => MessageBlock.Split(x.Situation).Base).Where(x => SituationKind(d, wn, x) == "ヘルプに無いもの").Distinct())
                .GroupBy(x => x).OrderByDescending(g => g.Count()).ThenBy(g => g.Key, StringComparer.Ordinal).Take(30);
            Console.WriteLine("| シチュエーション | パイロット |");
            Console.WriteLine("|---|--:|");
            foreach (var g in unknown) Console.WriteLine($"| {g.Key} | {g.Count()} |");
        }
    }
}
