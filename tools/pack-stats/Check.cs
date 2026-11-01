// 作品のデータを、パックの実態と比べて総合的に評価する（check）
using SRCCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PackStats
{
    public static partial class Program
    {
        // x がパックの値の分布で何 % の位置か（同じ値は半分に数える）
        private static int Pctl(IReadOnlyList<int> sorted, int x)
        {
            if (sorted.Count == 0) return 50;
            int lo = 0, hi = 0;
            foreach (var v in sorted) { if (v < x) lo++; else if (v == x) hi++; }
            return (int)Math.Round(100.0 * (lo + hi / 2.0) / sorted.Count);
        }

        // 25%〜75% の外を ↑↓、5%〜95% の外を ↑↑↓↓
        private static string Pos(int p) => p < 5 ? "↓↓" : p < 25 ? "↓" : p > 95 ? "↑↑" : p > 75 ? "↑" : "";

        private static string Cell(int v, IReadOnlyList<int> sorted)
        {
            var p = Pctl(sorted, v);
            return $"{v}（{Pos(p)}P{p}）";
        }

        private class Finding
        {
            public string Target;
            public string Text;
            public string Level; // 要確認・参考
        }

        // パックから作る比べる値は、作品ごとに作り直さない（site で全作品を評価するため）
        private static readonly Dictionary<string, object> memo = new Dictionary<string, object>();

        private static T Memo<T>(Data pack, string key, Func<T> f)
        {
            var k = pack.Root + "\n" + key;
            if (!memo.TryGetValue(k, out var v))
            {
                v = f();
                memo[k] = v;
            }
            return (T)v;
        }

        // 直前の Check の件数（site の一覧に出す）
        private static (int pilots, int units, int must, int info) lastCheck;

        private static void Check(Data pack, string dir)
        {
            var spFile = File.Exists(Path.Combine(dir, "sp.txt")) ? null
                : Directory.EnumerateFiles(pack.Root, "sp.txt", SearchOption.AllDirectories)
                    .OrderBy(f => Path.GetFileName(Path.GetDirectoryName(f)) == "System" ? 0 : 1).ThenBy(f => f, StringComparer.Ordinal).FirstOrDefault();
            var t = Data.Load(dir, spFile);
            var fs = new List<Finding>();
            void Add(string level, string target, string text) => fs.Add(new Finding { Level = level, Target = target, Text = text });

            Console.WriteLine($"# {t.Name} の評価（基準: {pack.Name}）");
            Console.WriteLine();
            Console.WriteLine("数値の横の「P」は、パックの同じ層の中での位置（%）。↑↓ は 25%〜75% の外、↑↑↓↓ は 5%〜95% の外。外れていても誤りではないが、原作の描写や役どころで理由を説明できるようにする。");
            Console.WriteLine();

            var refPilots = Memo(pack, "pilots", () => Pilots(pack));
            var tPilots = t.Src.PDList.Items.Where(p => !IsSystemPilot(p)).ToList();
            Func<PilotData, int>[] stat = { p => p.Infight, p => p.Shooting, p => p.Hit, p => p.Dodge, p => p.Technique, p => p.Intuition };
            string[] statName = { "格闘", "射撃", "命中", "回避", "技量", "反応" };
            var bandStats = Memo(pack, "bandStats", () => PilotBands.ToDictionary(b => b, b =>
            {
                var g = refPilots.Where(p => PilotBand(p) == b).ToList();
                return new
                {
                    Exp = g.Select(p => p.ExpValue).OrderBy(x => x).ToList(),
                    Stats = stat.Select(f => g.Select(f).OrderBy(x => x).ToList()).ToArray(),
                    Sp = g.Select(p => p.SP).OrderBy(x => x).ToList(),
                    SpCount = g.Select(p => p.DefinedSpecialPowers().Count).OrderBy(x => x).ToList(),
                    Cats = g.Where(p => IsNamed(p)).Select(p => p.DefinedSpecialPowers().SelectMany(x => SpCategory(pack, x.Name)).Distinct().Count()).OrderBy(x => x).ToList(),
                    Cost = g.Where(p => p.DefinedSpecialPowers().Any()).Select(p => p.DefinedSpecialPowers().Sum(x => SpCost(pack, x))).OrderBy(x => x).ToList(),
                    Pers = g.GroupBy(p => p.Personality ?? "").ToDictionary(x => x.Key, x => 100.0 * x.Count() / g.Count),
                    Count = g.Count,
                    Group = g,
                    Total = g.Select(Total).OrderBy(x => x).ToList(),
                };
            }));

            // ---- パイロット
            Console.WriteLine("## パイロット");
            Console.WriteLine();
            Console.WriteLine("| パイロット | 層 | 経験値 | 格闘 | 射撃 | 命中 | 回避 | 技量 | 反応 | 6 能力の合計 | SP の最大値 | 性格 | 得意・苦手 |");
            Console.WriteLine("|---|---|---|---|---|---|---|---|---|---|---|---|---|");
            foreach (var p in tPilots)
            {
                var b = PilotBand(p);
                var s = bandStats[b];
                var total = s.Total;
                var pos = stat.Select((f, i) => Pctl(s.Stats[i], f(p))).ToArray();
                var hi = pos.Count(x => x > 75);
                var lo = pos.Count(x => x < 25);
                var pers = s.Pers.GetValueOrDefault(p.Personality ?? "");
                Console.WriteLine($"| {p.Name} | {b} | {Cell(p.ExpValue, s.Exp)} | " + string.Join(" | ", stat.Select((f, i) => Cell(f(p), s.Stats[i]))) + $" | {Cell(Total(p), total)} | {Cell(p.SP, s.Sp)} | {p.Personality}（{pers:F0}%） | {hi}・{lo} |");

                if (b == "ザコ" || b == "汎用")
                {
                    var ep = Pctl(s.Exp, p.ExpValue);
                    if (ep < 5 || ep > 95) Add("要確認", p.Name, $"経験値 {p.ExpValue} が{b}の 5%〜95% の外（{Iqr(s.Exp)}）");
                }
                else if (b != "120未満" && hi == 0 && lo == 0)
                {
                    Add("要確認", p.Name, "同じ帯の中で得意（75% 超）も苦手（25% 未満）も無い。原作の戦い方に合わせた偏りを付けるか確かめる（名前付きで全能力が中ほどなのは 1 割未満）");
                }
                for (var i = 0; i < 6; i++)
                {
                    if (pos[i] < 5 || pos[i] > 95) Add("参考", p.Name, $"{statName[i]} {stat[i](p)} が同じ層の 5%〜95% の外（P{pos[i]}）");
                }
                var spp = Pctl(s.Sp, p.SP);
                if (p.SP > 0 && (spp < 5 || spp > 95)) Add("参考", p.Name, $"SP の最大値 {p.SP} が同じ層の 5%〜95% の外（{Iqr(s.Sp)}）");
                if (pers < 5) Add("参考", p.Name, $"性格「{p.Personality}」は同じ層で {pers:F1}% しかいない");
            }

            // ---- SP
            Console.WriteLine();
            Console.WriteLine("## SP");
            Console.WriteLine();
            Console.WriteLine("覚えるレベルの P は、パックで同じ SP を持つ経験値 120 以上の名前付きのパイロットの中での位置。持つ割合は、パックの同じ層でその SP を持つ割合。");
            Console.WriteLine();
            Console.WriteLine("| パイロット | SP（覚えるレベル P・持つ割合） | SP の数 | 命中・回避 | 分類の種類 | 消費の合計 |");
            Console.WriteLine("|---|---|---|--:|---|---|");
            var refNamed = Memo(pack, "named", () => refPilots.Where(p => IsNamed(p) && p.ExpValue >= 120).ToList());
            var lvRef = Memo(pack, "lvRef", () => new Dictionary<string, List<int>>());
            List<int> LvRef(string sp)
            {
                if (!lvRef.TryGetValue(sp, out var l))
                {
                    l = refNamed.SelectMany(p => p.DefinedSpecialPowers().Where(x => x.Name == sp).Select(x => x.NecessaryLevel)).OrderBy(x => x).ToList();
                    lvRef[sp] = l;
                }
                return l;
            }
            var hdRef = Memo(pack, "hdRef", () => refNamed.Select(p => p.DefinedSpecialPowers().Count(x => IsHitDodge(pack, x.Name))).ToList());
            var haveRate = Memo(pack, "haveRate", () => new Dictionary<string, (int have, int count)>());
            (int have, int count) Have(string band, string sp)
            {
                var k = band + "/" + sp;
                if (!haveRate.TryGetValue(k, out var r))
                {
                    var g = bandStats[band];
                    r = (g.Group.Count(q => HasSp(q, sp)), g.Count);
                    haveRate[k] = r;
                }
                return r;
            }
            foreach (var p in tPilots.Where(p => p.DefinedSpecialPowers().Any()))
            {
                var b = PilotBand(p);
                var s = bandStats[b];
                var sps = p.DefinedSpecialPowers().ToList();
                var cells = sps.Select(x =>
                {
                    var (have, cnt) = Have(b, x.Name);
                    var rate = cnt == 0 ? 0 : 100.0 * have / cnt;
                    var lv = LvRef(x.Name);
                    var lp = Pctl(lv, x.NecessaryLevel);
                    return $"{x.Name}{x.NecessaryLevel}（{(lv.Count >= 10 ? Pos(lp) + "P" + lp : "-")}・{rate:F0}%）";
                });
                var hd = sps.Count(x => IsHitDodge(t, x.Name));
                var cats = sps.SelectMany(x => SpCategory(t, x.Name)).Distinct().ToList();
                var cost = sps.Sum(x => SpCost(t, x));
                Console.WriteLine($"| {p.Name} | {string.Join("・", cells)} | {Cell(sps.Count, s.SpCount)} | {hd} | {(IsNamed(p) ? Cell(cats.Count, s.Cats) : cats.Count.ToString())} | {Cell(cost, s.Cost)} |");

                foreach (var x in sps.Where(x => !t.Src.SPDList.IsDefined(x.Name))) Add("要確認", p.Name, $"SP「{x.Name}」が sp.txt に無い");
                var dup = sps.GroupBy(x => x.Name).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                if (dup.Any()) Add("要確認", p.Name, $"同じ SP を重ねて持つ: {string.Join("、", dup)}");
                if (!IsNamed(p))
                {
                    if (sps.Count > 3) Add("要確認", p.Name, $"{b}で SP が {sps.Count} 個（{b}の SP の数は {Iqr(s.SpCount)}）");
                    continue;
                }
                var cp = Pctl(s.SpCount, sps.Count);
                if (cp < 5 || cp > 95) Add("要確認", p.Name, $"SP の数 {sps.Count} が同じ層の 5%〜95% の外（{Iqr(s.SpCount)}）");
                if (hd >= 3) Add("要確認", p.Name, $"命中・回避に効く SP が {hd} 個（{string.Join("・", sps.Where(x => IsHitDodge(t, x.Name)).Select(x => x.Name))}）。3 つ以上はパックで {Pct(hdRef.Count(n => n >= 3), hdRef.Count)}");
                var ctp = Pctl(s.Cats, cats.Count);
                if (ctp < 5) Add("参考", p.Name, $"SP の分類が {cats.Count} 種類（{string.Join("・", cats)}）で、用途が偏っている（同じ層は {Iqr(s.Cats)}）");
                var later = sps.Where(x => x.NecessaryLevel > 1).ToList();
                if (later.Count >= 3 && later.All(x => x.NecessaryLevel % 5 == 0)) Add("要確認", p.Name, "レベル 1 以外の SP を覚えるレベルが全て 5 の倍数（パックで 1% ほど）");
                var lvs = sps.Select(x => x.NecessaryLevel).OrderBy(x => x).ToList();
                var gaps = lvs.Zip(lvs.Skip(1), (a, c) => c - a).Where(g => g > 0).ToList();
                if (gaps.Count >= 3 && gaps.Distinct().Count() == 1) Add("要確認", p.Name, $"SP を覚えるレベルの間隔が全て {gaps[0]}（一定にしない）");
                foreach (var x in sps)
                {
                    var lv = LvRef(x.Name);
                    // 比べるのは経験値 120 以上の名前付きなので、それより低い帯は覚えるレベルを見ない
                    if (lv.Count < 10 || p.ExpValue < 120) continue;
                    var lp = Pctl(lv, x.NecessaryLevel);
                    if (lp < 5) Add(lp < 3 ? "要確認" : "参考", p.Name, $"{x.Name}をレベル {x.NecessaryLevel} で覚えるのは早い（パックは {Iqr(lv)}）");
                    if (lp > 95) Add(lp > 97 ? "要確認" : "参考", p.Name, $"{x.Name}をレベル {x.NecessaryLevel} で覚えるのは遅い（パックは {Iqr(lv)}）");
                    var (have, cnt) = Have(b, x.Name);
                    if (cnt > 0 && 100.0 * have / cnt < 1) Add("参考", p.Name, $"{x.Name}は同じ層で持つのが {Pct(have, cnt)}");
                }
            }

            // ---- ユニット
            var refUnits = Memo(pack, "units", () => Units(pack));
            var tUnits = Units(t);
            Func<UnitData, int>[] ustat = { u => u.HP, u => u.EN, u => u.Armor, u => u.Mobility, u => u.Speed, u => u.Weapons.Max(w => w.Power), u => u.Weapons.Max(w => w.MaxRange), u => u.Weapons.Count };
            string[] ustatName = { "HP", "EN", "装甲", "運動性", "移動力", "最大攻撃力", "最大射程", "武器数" };
            var ub = Memo(pack, "unitBands", () => UnitBands.ToDictionary(b => b, b =>
            {
                var g = refUnits.Where(u => ExpBand(u.ExpValue) == b).ToList();
                return new
                {
                    Stats = ustat.Select(f => g.Select(f).OrderBy(x => x).ToList()).ToArray(),
                    Q = new[] { 0, 2, 3 }.Select(i => (q25: Quantile(g.Select(ustat[i]), .25), q50: Quantile(g.Select(ustat[i]), .5), q75: Quantile(g.Select(ustat[i]), .75))).ToArray(),
                    Top = (q75: Quantile(g.Select(ustat[5]), .75), x: 0),
                    Uses = g.Where(u => u.Weapons.Any(w => !IsMap(w) && w.Power > 0)).Select(u => Uses(u, u.Weapons.Where(w => !IsMap(w) && w.Power > 0).OrderByDescending(w => w.Power).First())).ToList(),
                };
            }));
            string UType(UnitData u)
            {
                var b = ub[ExpBand(u.ExpValue)];
                var top = u.Weapons.Max(w => w.Power);
                if (u.Mobility >= b.Q[2].q75 && u.HP <= b.Q[0].q50 && u.Armor <= b.Q[1].q50) return "リアル系";
                if ((u.HP >= b.Q[0].q75 || u.Armor >= b.Q[1].q75) && u.Mobility <= b.Q[2].q25) return "スーパー系";
                if (top >= b.Top.q75) return "グレー系";
                return "-";
            }
            Console.WriteLine();
            Console.WriteLine("## ユニット");
            Console.WriteLine();
            Console.WriteLine("| ユニット | 経験値 | " + string.Join(" | ", ustatName) + " | 系統 | 最大攻撃力の武器の回数 |");
            Console.WriteLine("|---|--:|" + string.Concat(ustatName.Select(_ => "---|")) + "---|---|");
            foreach (var u in tUnits)
            {
                var b = ExpBand(u.ExpValue);
                var s = ub[b];
                var topW = u.Weapons.Where(w => !IsMap(w) && w.Power > 0).OrderByDescending(w => w.Power).FirstOrDefault();
                var uses = topW == null ? "-" : Uses(u, topW) < 0 ? "制限なし" : Uses(u, topW).ToString();
                Console.WriteLine($"| {u.Name} | {u.ExpValue} | " + string.Join(" | ", ustat.Select((f, i) => Cell(f(u), s.Stats[i]))) + $" | {UType(u)} | {uses} |");

                if (u.HP % 100 != 0) Add("要確認", u.Name, $"HP {u.HP} が 100 刻みでない");
                if (u.EN % 10 != 0) Add("要確認", u.Name, $"EN {u.EN} が 10 刻みでない");
                if (u.Armor % 50 != 0) Add("要確認", u.Name, $"装甲 {u.Armor} が 100（細かくても 50）刻みでない");
                if (u.Mobility % 5 != 0) Add("要確認", u.Name, $"運動性 {u.Mobility} が 5 刻みでない");
                for (var i = 0; i < ustat.Length; i++)
                {
                    var p = Pctl(s.Stats[i], ustat[i](u));
                    if (p < 5 || p > 95) Add("参考", u.Name, $"{ustatName[i]} {ustat[i](u)} が同じ帯の 5%〜95% の外（P{p}）");
                }
                foreach (var w in u.Weapons)
                {
                    if (w.Power % 100 != 0) Add("要確認", u.Name, $"{w.Name}の攻撃力 {w.Power} が 100 刻みでない");
                    if (w.ENConsumption % 5 != 0) Add("要確認", u.Name, $"{w.Name}の消費 EN {w.ENConsumption} が 5 刻みでない");
                    if (!IsMap(w) && w.MaxRange >= 2 && w.Class.Contains("射")) Add("要確認", u.Name, $"{w.Name}は射程 2 以上なのに「射」がある");
                    if (!IsMap(w) && w.MaxRange == 2 && w.Power > 0 && !w.Class.Contains("Ｐ")) Add("参考", u.Name, $"{w.Name}は射程 2 で移動後に使えない（射程 2 の武器は拡張で 78%、ロボで 89% が Ｐ を持つ）");
                }
            }

            // ---- 武器
            Console.WriteLine();
            Console.WriteLine("## 武器");
            Console.WriteLine();
            Console.WriteLine("| ユニット | 武器 | 攻撃力 | 射程 | 命中 | 弾数 | 消費 EN | 気力 | 地形適応 | クリティカル | 属性 |");
            Console.WriteLine("|---|---|--:|---|--:|--:|--:|--:|---|--:|---|");
            foreach (var u in tUnits)
            {
                foreach (var w in u.Weapons)
                {
                    string N(int v) => v > 0 ? v.ToString() : "-";
                    Console.WriteLine($"| {u.Name} | {w.Name} | {w.Power} | {w.MinRange}-{w.MaxRange} | {w.Precision:+0;-0;0} | {N(w.Bullet)} | {N(w.ENConsumption)} | {N(w.NecessaryMorale)} | {w.Adaption} | {w.Critical:+0;-0;0} | {w.Class} |");
                }
            }

            // ---- マップ兵器
            var maps = tUnits.SelectMany(u => u.Weapons.Where(w => IsMap(w) && w.Power > 0).Select(w => (u, w))).ToList();
            if (maps.Any())
            {
                var refMaps = Memo(pack, "maps", () => refUnits.SelectMany(u => u.Weapons.Where(w => IsMap(w) && w.Power > 0).Select(w => (u, w))).ToList());
                Console.WriteLine();
                Console.WriteLine("## マップ兵器");
                Console.WriteLine();
                Console.WriteLine("P は、パックの同じ種類（Ｍ扇・Ｍ投は範囲のレベルも同じ）のマップ兵器の中での位置。");
                Console.WriteLine();
                Console.WriteLine("| ユニット | マップ兵器 | 種類 | 件数 | 攻撃力 | 最大射程 | 消費 EN | 気力 | 回数 | 単体武器との比 | 識 | 型 |");
                Console.WriteLine("|---|---|---|--:|---|---|---|---|---|--:|---|---|");
                foreach (var (u, w) in maps)
                {
                    var kind = MapKinds.First(k => w.Class.Contains(k));
                    var lv = kind == "Ｍ扇" || kind == "Ｍ投" ? MapLevel(w, kind) : -1;
                    var same = refMaps.Where(x => x.w.Class.Contains(kind) && (lv < 0 || MapLevel(x.w, kind) == lv)).ToList();
                    var label = lv >= 0 ? $"{kind}L{lv}" : kind;
                    var single = u.Weapons.Where(x => !IsMap(x) && x.Power > 0).Select(x => x.Power).DefaultIfEmpty(0).Max();
                    var mw = u.Weapons.Where(x => IsMap(x) && x.Power > 0).ToList();
                    var top = mw.OrderByDescending(x => x.Power).First();
                    var type = top.Power < single * 0.9 ? "補助型" : mw.Count == 1 && Uses(u, top) >= 0 && Uses(u, top) <= 2 ? "切り札型" : "マップ兵器屋";
                    var n = Uses(u, w);
                    var sk = same.Count == 0 ? "-" : Pct(same.Count(x => x.w.Class.Contains("識")), same.Count);
                    Console.WriteLine($"| {u.Name} | {w.Name} | {label} | {same.Count} | {Cell(w.Power, same.Select(x => x.w.Power).OrderBy(x => x).ToList())} | {Cell(w.MaxRange, same.Select(x => x.w.MaxRange).OrderBy(x => x).ToList())} | {Cell(w.ENConsumption, same.Select(x => x.w.ENConsumption).OrderBy(x => x).ToList())} | {Cell(w.NecessaryMorale, same.Select(x => x.w.NecessaryMorale).OrderBy(x => x).ToList())} | {(n < 0 ? "制限なし" : n.ToString())} | {(single == 0 ? "-" : $"{100 * w.Power / single}%")} | {(w.Class.Contains("識") ? "あり" : "なし")}（同じ種類で {sk}） | {type} |");
                    var pp = Pctl(same.Select(x => x.w.Power).OrderBy(x => x).ToList(), w.Power);
                    if (same.Count >= 10 && pp > 95) Add("参考", u.Name, $"{w.Name}の攻撃力 {w.Power} が同じ種類（{label}）の 95% を超える（{Iqr(same.Select(x => x.w.Power))}）");
                }
            }

            // ---- メッセージ
            Console.WriteLine();
            Console.WriteLine("## メッセージ");
            Console.WriteLine();
            var wn = WeaponNames(t);
            wn.UnionWith(Memo(pack, "weaponNames", () => WeaponNames(pack)));
            var blocks = t.Messages.GroupBy(m => m.Pilot).ToDictionary(g => g.Key, g => g.SelectMany(x => x.Lines).ToList());
            var refBlocks = Memo(pack, "blocks", () => pack.Messages.GroupBy(m => m.Pilot).ToDictionary(g => g.Key, g => g.Sum(x => x.Lines.Count)));
            var refLines = Memo(pack, "lineBands", () => PilotBands.ToDictionary(b => b, b => refPilots.Where(x => PilotBand(x) == b && refBlocks.ContainsKey(x.Name)).Select(x => refBlocks[x.Name]).OrderBy(x => x).ToList()));
            Console.WriteLine("| パイロット | 行数 | 回避 | ダメージ小・中・大 | 破壊 | 射程外 | 攻撃 | 武器名 | (反撃) | (とどめ) | (対…) | SP 名 | ヘルプに無いもの |");
            Console.WriteLine("|---|---|--:|---|--:|--:|--:|--:|--:|--:|--:|--:|--:|");
            foreach (var p in tPilots)
            {
                if (!blocks.TryGetValue(p.Name, out var lines) && !blocks.TryGetValue(p.Nickname ?? "", out lines))
                {
                    Add(IsNamed(p) && p.ExpValue >= 120 ? "要確認" : "参考", p.Name, "メッセージが無い");
                    continue;
                }
                var b = PilotBand(p);
                var refN = refLines[b];
                var k = lines.Select(x => (x, s: MessageBlock.Split(x.Situation))).Select(x => (x.x, x.s.Subs, kind: SituationKind(t, wn, x.s.Base))).ToList();
                int C(string kind) => k.Count(x => x.kind == kind);
                Console.WriteLine($"| {p.Name} | {Cell(lines.Count, refN)} | {C("回避")} | {C("ダメージ小")}・{C("ダメージ中")}・{C("ダメージ大")} | {C("破壊")} | {C("射程外")} | {C("攻撃")} | {C("武器名・アビリティ名")} | {k.Count(x => x.Subs.Contains("反撃"))} | {k.Count(x => x.Subs.Contains("とどめ"))} | {k.Count(x => x.Subs.Any(s => s.StartsWith("対")))} | {C("SP 名")} | {C("ヘルプに無いもの")} |");
                foreach (var need in new[] { "回避", "ダメージ小", "ダメージ中", "ダメージ大", "破壊", "攻撃" })
                {
                    if (C(need) == 0) Add("要確認", p.Name, $"メッセージに「{need}」が無い（パックではほぼ全員が持つ）");
                }
                var spLines = k.Where(x => x.kind == "SP 名" && x.x.Situation != "復活" && x.x.Situation != "自爆").ToList();
                if (spLines.Any()) Add("要確認", p.Name, $"SP 使用時のメッセージがある（{string.Join("・", spLines.Select(x => x.x.Situation).Distinct())}）。パックで SP のメッセージを持つパイロットは 3% ほどで、原作の決め台詞に限られる");
                foreach (var x in k.Where(x => x.kind == "ヘルプに無いもの").Select(x => x.x.Situation).Distinct())
                    Add("要確認", p.Name, $"シチュエーション「{x}」は SRC のヘルプに無く、武器・アビリティの名前でもない（表示されない）");
                foreach (var x in k.Where(x => x.kind == "武器名・アビリティ名" && !WeaponNames(t).Contains(MessageBlock.Split(x.x.Situation).Base)).Select(x => x.x.Situation).Distinct())
                    Add("参考", p.Name, $"シチュエーション「{x}」の武器・アビリティがこの作品に無い");
                foreach (var x in k.SelectMany(x => x.Subs).Where(s => s.StartsWith("対")).Distinct())
                {
                    var target = x.Substring(1);
                    if (!t.Src.PDList.IsDefined(target) && !t.Src.PDList.Items.Any(q => q.Nickname == target) && !t.Src.UDList.IsDefined(target)
                        && !new[] { "男性", "女性", "瀕死", "ザコ", "強敵", "高回避率", "低回避率", "高命中率", "低命中率", "自分" }.Contains(target))
                        Add("参考", p.Name, $"「({x})」の相手がこの作品のパイロット・ユニットに無い");
                }
            }
            foreach (var name in blocks.Keys.Where(n => !t.Src.PDList.IsDefined(n) && !tPilots.Any(q => q.Nickname == n)))
                Add("参考", name, "メッセージの名前がこの作品のパイロットデータに無い（ほかの作品のパイロットか、名前の誤り）");

            // ---- 指摘
            Console.WriteLine();
            Console.WriteLine("## 指摘");
            Console.WriteLine();
            Console.WriteLine("「要確認」はガイドラインの目安から外れているもの、「参考」は外れ値の情報。役どころの評価（組の評価）は tools/pack-analysis で行う。");
            Console.WriteLine();
            lastCheck = (tPilots.Count, tUnits.Count, fs.Count(f => f.Level == "要確認"), fs.Count(f => f.Level == "参考"));
            foreach (var level in new[] { "要確認", "参考" })
            {
                var g = fs.Where(f => f.Level == level).ToList();
                Console.WriteLine($"### {level}（{g.Count} 件）");
                Console.WriteLine();
                if (g.Count == 0) Console.WriteLine("なし");
                foreach (var f in g) Console.WriteLine($"- {f.Target}: {f.Text}");
                Console.WriteLine();
            }
        }
    }
}
