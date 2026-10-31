// 準拠データパックの分析の試作。docs/tools/balance-pack.md の「役割と格の決め方」「組の評価」を、
// GSC データパック（7474/SRC-Data の GSCデータパック）にかけて Markdown を出す。
// srcsharp の機能ではない。使い方と再現の手順は README.md。
using SRCCore;
using SRCCore.Filesystem;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.TestLib;
using SRCCore.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PackAnalysis
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length < 4)
            {
                Console.Error.WriteLine("使い方: PackAnalysis <パックのフォルダ> <ロボ|拡張> <役割表.csv|-> <対象作品.txt> [--sample 数] [--seed 数] [--level 数]");
                return 1;
            }
            var options = new Options
            {
                PackDir = args[0],
                Pack = args[1],
                RolesCsv = args[2],
                WorksFile = args[3],
            };
            for (var i = 4; i + 1 < args.Length; i += 2)
            {
                switch (args[i])
                {
                    case "--sample": options.Sample = int.Parse(args[i + 1]); break;
                    case "--seed": options.Seed = int.Parse(args[i + 1]); break;
                    case "--level": options.Level = int.Parse(args[i + 1]); break;
                }
            }
            new Analysis(options).Run(Console.Out);
            return 0;
        }
    }

    internal sealed class Options
    {
        public string PackDir;
        public string Pack;
        public string RolesCsv;
        public string WorksFile;
        public int Sample = 400;
        public int Seed = 1;
        public int Level = 30;
    }

    /// <summary>役割表の 1 行</summary>
    internal sealed class RoleEntry
    {
        public string Kind;
        public string Name;
        public string Role;
        public string Rank;
        public string Line;
        public string Unit;
    }

    internal sealed class Pair
    {
        public PilotData Pilot;
        public UnitData Unit;
        public string How;
    }

    internal sealed class Analysis
    {
        private const int MapWidth = 12;
        private const int MapHeight = 3;
        private static readonly string[] Uses = { "ボス戦", "ザコ掃討", "回避・囮", "遊撃", "長距離", "支援" };
        private static readonly string[] Bands = { "経験値200以上", "経験値180-199", "経験値160-179", "経験値120-159", "経験値120未満", "名前なし" };

        private readonly Options opt;
        private readonly SRC src = new SRC { GUI = new MockGUI(), FileSystem = new LocalFileSystem() };
        private readonly Dictionary<string, string> pilotWork = new Dictionary<string, string>();
        private readonly Dictionary<string, string> unitWork = new Dictionary<string, string>();
        private readonly List<(string pilot, string unit)> recommended = new List<(string, string)>();
        private readonly List<RoleEntry> roles = new List<RoleEntry>();
        private readonly List<string> works = new List<string>();
        private readonly List<string> worksNotes = new List<string>();
        private HashSet<string> additionalPilots;
        private Dictionary<string, UnitData> unitByName;
        private readonly List<Pair> pairs = new List<Pair>();
        private readonly Dictionary<string, List<PilotData>> subPilots = new Dictionary<string, List<PilotData>>();
        private (PilotData p, UnitData u) zakoOpponent;
        private (PilotData p, UnitData u) zakoAlternative;
        private readonly List<(PilotData p, UnitData u)> bossPanel = new List<(PilotData, UnitData)>();
        private readonly Dictionary<string, (PilotData p, UnitData u)> bandOpponent = new Dictionary<string, (PilotData, UnitData)>();
        private readonly Dictionary<string, Dictionary<string, double>> cache = new Dictionary<string, Dictionary<string, double>>();
        private readonly Dictionary<string, List<Dictionary<string, double>>> bandReference = new Dictionary<string, List<Dictionary<string, double>>>();
        private readonly List<(Unit u, Pilot p)> spawned = new List<(Unit, Pilot)>();
        private string mapKind = "陸";
        private int skipped;

        public Analysis(Options opt)
        {
            this.opt = opt;
        }

        public void Run(TextWriter output)
        {
            LoadWorks();
            LoadRoles();
            LoadData();
            BuildPairs();
            SetupMap();
            ChooseOpponents();
            BuildReference();
            Write(output);
        }

        // ---- 入力 ----

        private void LoadWorks()
        {
            foreach (var line in File.ReadLines(opt.WorksFile))
            {
                var t = line.Trim();
                if (t.StartsWith("#"))
                {
                    worksNotes.Add(t.TrimStart('#').Trim());
                }
                else if (t != "")
                {
                    works.Add(t.Split('\t')[0]);
                }
            }
        }

        private void LoadRoles()
        {
            if (opt.RolesCsv == "-")
            {
                return;
            }
            foreach (var line in File.ReadLines(opt.RolesCsv).Skip(1))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                {
                    continue;
                }
                var c = line.Split(',');
                roles.Add(new RoleEntry { Kind = c[0], Name = c[1], Role = c[2], Rank = c[3], Line = c[4], Unit = c.Length > 5 ? c[5] : "" });
            }
        }

        private void LoadData()
        {
            // 読み込みの順を固定する（sp.txt を先に、ほかはパスの順）
            var files = Directory.EnumerateFiles(opt.PackDir, "*.txt", SearchOption.AllDirectories)
                .OrderBy(f => Path.GetFileName(f).ToLowerInvariant() == "sp.txt" ? 0 : 1)
                .ThenBy(f => f, StringComparer.Ordinal)
                .ToList();
            foreach (var f in files)
            {
                var name = Path.GetFileName(f).ToLowerInvariant();
                var work = Path.GetFileName(Path.GetDirectoryName(f));
                var beforeP = src.PDList.Items.Select(x => x.Name).ToHashSet();
                var beforeU = src.UDList.Items.Select(x => x.Name).ToHashSet();
                try
                {
                    using (var s = File.OpenRead(f))
                    {
                        if (name == "sp.txt") src.SPDList.Load(name, s);
                        else if (name == "robot.txt" || name == "unit.txt") src.UDList.Load(name, s);
                        else if (name == "pilot.txt") src.PDList.Load(name, s);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"読み込めない: {f}: {ex.Message}");
                }
                foreach (var p in src.PDList.Items.Where(x => !beforeP.Contains(x.Name))) pilotWork[p.Name] = work;
                foreach (var u in src.UDList.Items.Where(x => !beforeU.Contains(x.Name))) unitWork[u.Name] = work;
                if (name == "pilot.txt")
                {
                    // 「#推奨ユニット：」のコメント
                    string last = null;
                    foreach (var line in File.ReadLines(f))
                    {
                        var t = line.Trim();
                        if (t != "" && !t.StartsWith("#") && !t.Contains(",") && src.PDList.IsDefined(t)) last = t;
                        var m = Regex.Match(t, @"推奨ユニット[：:]\s*([^、,(（＋]+)");
                        if (m.Success && last != null) recommended.Add((last, m.Groups[1].Value.Trim()));
                    }
                }
                src.ClearDataError();
            }
            additionalPilots = src.UDList.Items
                .SelectMany(u => new[] { "追加パイロット", "暴走時パイロット" }.Where(u.IsFeatureAvailable).Select(u.FeatureData))
                .ToHashSet();
            unitByName = src.UDList.Items.ToDictionary(u => u.Name);
        }

        // ---- 役割と格 ----

        private static bool IsZako(PilotData p) => p.Name.Contains("(ザコ)") || p.Name.Contains("(汎用)");
        private static bool IsSupport(PilotData p) => (p.Class ?? "").Contains("サポート");

        private string PilotRole(PilotData p)
        {
            var declared = roles.FirstOrDefault(r => r.Kind == "pilot" && r.Name == p.Name);
            if (declared != null && declared.Role != "" && declared.Role != "ボス") return declared.Role;
            if (additionalPilots.Contains(p.Name)) return "追加パイロット";
            if (IsZako(p)) return "名前なし";
            if (IsSupport(p)) return "サポート";
            if ((p.Class ?? "").Contains("戦艦")) return "艦長";
            return "戦闘";
        }

        private static string Band(PilotData p)
        {
            if (IsZako(p)) return "名前なし";
            if (p.ExpValue >= 200) return "経験値200以上";
            if (p.ExpValue >= 180) return "経験値180-199";
            if (p.ExpValue >= 160) return "経験値160-179";
            if (p.ExpValue >= 120) return "経験値120-159";
            return "経験値120未満";
        }

        private static string BaseName(string s) => Regex.Replace(s ?? "", @"[\(（].*$", "").Trim();

        /// <summary>自動で組にするユニットか（追加パイロット・暴走時パイロットを持つものは、乗る人が分からないので除く）</summary>
        private static bool AutoPairable(UnitData u) => !u.IsFeatureAvailable("追加パイロット") && !u.IsFeatureAvailable("暴走時パイロット");

        /// <summary>変形・換装・ハイパーモード・他形態でたどれる形態のうち、経験値・最大攻撃力・HP の最も高いもの</summary>
        private UnitData Strongest(UnitData u)
        {
            var seen = new HashSet<string> { u.Name };
            var forms = new List<UnitData> { u };
            for (var depth = 0; depth < 3; depth++)
            {
                foreach (var x in forms.ToList())
                {
                    foreach (var f in new[] { "変形", "換装", "ハイパーモード", "他形態" })
                    {
                        if (!x.IsFeatureAvailable(f)) continue;
                        var skip = f == "換装" || f == "他形態" ? 0 : 1;
                        foreach (var n in GeneralLib.ToL(x.FeatureData(f)).Skip(skip))
                        {
                            if (seen.Add(n) && unitByName.TryGetValue(n, out var y) && AutoPairable(y) && y.Weapons.Any()) forms.Add(y);
                        }
                    }
                }
            }
            return forms.OrderByDescending(x => x.ExpValue)
                .ThenByDescending(x => x.Weapons.Select(w => w.Power).DefaultIfEmpty(0).Max())
                .ThenByDescending(x => x.HP)
                .First();
        }

        private void BuildPairs()
        {
            // 役割表（サブ・ボスの行は組にしない。サブは乗るユニットごとにまとめる）
            foreach (var r in roles.Where(r => r.Kind == "pilot"))
            {
                if (!src.PDList.IsDefined(r.Name) || (r.Unit != "" && !unitByName.ContainsKey(r.Unit)))
                {
                    Console.Error.WriteLine($"役割表の名前が見つからない: {r.Name} / {r.Unit}");
                    continue;
                }
                if (r.Role == "サブ")
                {
                    if (!subPilots.ContainsKey(r.Unit)) subPilots[r.Unit] = new List<PilotData>();
                    subPilots[r.Unit].Add(src.PDList.Item(r.Name));
                    continue;
                }
                if (r.Role == "ボス" || r.Unit == "") continue;
                pairs.Add(new Pair { Pilot = src.PDList.Item(r.Name), Unit = unitByName[r.Unit], How = "役割表" });
            }
            var declared = pairs.Select(x => x.Pilot.Name).Concat(roles.Where(r => r.Role == "サブ").Select(r => r.Name)).ToHashSet();
            var unitsByBase = src.UDList.Items.Where(u => AutoPairable(u) && unitWork.ContainsKey(u.Name))
                .GroupBy(u => (unitWork[u.Name], BaseName(u.Name)))
                .ToDictionary(g => g.Key, g => g.OrderByDescending(u => u.ExpValue).First());

            // 名前の一致
            foreach (var p in src.PDList.Items)
            {
                if (declared.Contains(p.Name) || IsSupport(p) || additionalPilots.Contains(p.Name)) continue;
                if (unitByName.TryGetValue(p.Name, out var u) && AutoPairable(u))
                {
                    var s = Strongest(u);
                    pairs.Add(new Pair { Pilot = p, Unit = s, How = s == u ? "名前の一致" : "名前の一致（最も強い形態）" });
                }
                else if (opt.Pack == "拡張" && pilotWork.TryGetValue(p.Name, out var w) && unitsByBase.TryGetValue((w, BaseName(p.Name)), out var u2))
                {
                    var s = Strongest(u2);
                    pairs.Add(new Pair { Pilot = p, Unit = s, How = s == u2 ? "名前の一致（括弧書きを除く）" : "名前の一致（括弧書きを除く・最も強い形態）" });
                }
            }

            if (opt.Pack == "ロボ")
            {
                // #推奨ユニット のコメント
                foreach (var (pn, un) in recommended)
                {
                    if (declared.Contains(pn) || !src.PDList.IsDefined(pn) || !unitByName.ContainsKey(un) || pairs.Any(x => x.Pilot.Name == pn)) continue;
                    pairs.Add(new Pair { Pilot = src.PDList.Item(pn), Unit = unitByName[un], How = "推奨ユニットのコメント" });
                }
                // クラスの一致: 作品の中で、経験値の高いパイロットから順に、乗れるユニットのうち経験値の高いものを 1 機ずつ（名前なしは最も低いもの）
                var unitsByWork = src.UDList.Items.Where(u => AutoPairable(u) && unitWork.ContainsKey(u.Name) && u.Weapons.Any())
                    .GroupBy(u => unitWork[u.Name]).ToDictionary(g => g.Key, g => g.ToList());
                var paired = pairs.Select(x => x.Pilot.Name).Concat(declared).ToHashSet();
                foreach (var g in src.PDList.Items.Where(p => !IsSupport(p) && !additionalPilots.Contains(p.Name) && !paired.Contains(p.Name) && pilotWork.ContainsKey(p.Name))
                    .GroupBy(p => pilotWork[p.Name]))
                {
                    if (!unitsByWork.TryGetValue(g.Key, out var candidates)) continue;
                    var used = pairs.Select(x => x.Unit.Name).ToHashSet();
                    foreach (var p in g.Where(p => !IsZako(p)).OrderByDescending(p => p.ExpValue))
                    {
                        var classes = (p.Class ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        var pick = candidates
                            .Where(u => !used.Contains(u.Name) && classes.Contains(u.Class) && (PilotRole(p) == "艦長") == u.IsFeatureAvailable("母艦"))
                            .OrderByDescending(u => u.ExpValue).FirstOrDefault();
                        if (pick == null) continue;
                        used.Add(pick.Name);
                        pairs.Add(new Pair { Pilot = p, Unit = pick, How = "クラスの一致" });
                    }
                    foreach (var p in g.Where(IsZako))
                    {
                        var classes = (p.Class ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        var pick = candidates.Where(u => classes.Contains(u.Class) && !u.IsFeatureAvailable("母艦")).OrderBy(u => u.ExpValue).FirstOrDefault();
                        if (pick != null) pairs.Add(new Pair { Pilot = p, Unit = pick, How = "クラスの一致" });
                    }
                }
            }
            Console.Error.WriteLine("組: " + string.Join("、", pairs.GroupBy(x => x.How).Select(g => $"{g.Key} {g.Count()}")));
        }

        // ---- 仮のマップと配置 ----

        private void SetupMap()
        {
            src.Map.MapWidth = MapWidth;
            src.Map.MapHeight = MapHeight;
            src.Map.MapDataForUnit = new Src2DArray<Unit>(MapWidth, MapHeight);
            src.Map.MaskData = new Src2DArray<bool>(MapWidth, MapHeight);
            src.Map.MapData = new Src2DArray<MapCell>(MapWidth, MapHeight);
            src.Stage = "味方";
            SetTerrain("陸");
        }

        private void SetTerrain(string kind)
        {
            mapKind = kind;
            for (var x = 1; x <= MapWidth; x++)
            {
                for (var y = 1; y <= MapHeight; y++)
                {
                    var t = kind == "宇宙" ? new TerrainData { Name = "宇宙", Class = "宇宙" }
                        : kind == "水" ? new TerrainData { Name = "海", Class = "水" }
                        : new TerrainData { Name = "平地", Class = "陸" };
                    src.Map.MapData[x, y] = new MapCell { X = x, Y = y, UnderTerrain = t };
                }
            }
        }

        /// <summary>ユニットに合う地形（陸か空を移動できれば平地、水だけなら海、宇宙だけなら宇宙）</summary>
        private static string TerrainFor(UnitData ud)
        {
            var ad = ud.Adaption ?? "AAAA";
            var t = ud.Transportation ?? "";
            if (ad.Length > 1 && ad[1] != '-') return "陸";
            if (t.Contains("空") && ad[0] != '-') return "陸";
            if (t.Contains("水") && ad.Length > 2 && ad[2] != '-') return "水";
            if (t.Contains("宇宙")) return "宇宙";
            return "陸";
        }

        private string AreaFor(UnitData ud)
        {
            var ad = ud.Adaption ?? "AAAA";
            var flies = ad[0] != '-' && (ud.Transportation ?? "").Contains("空");
            if (mapKind == "宇宙") return "宇宙";
            if (mapKind == "水") return ad.Length > 2 && ad[2] != '-' ? "水中" : flies ? "空中" : "水上";
            if (ad.Length > 1 && ad[1] != '-') return "地上";
            return flies ? "空中" : "地上";
        }

        private (Unit u, Pilot p) Spawn(PilotData pd, UnitData ud, string party, int x)
        {
            var u = src.UList.Add(ud.Name, 0, party);
            var p = src.PList.Add(pd.Name, opt.Level, party);
            p.Ride(u);
            if (party == "味方" && subPilots.TryGetValue(ud.Name, out var subs))
            {
                foreach (var sd in subs.Where(s => s.Name != pd.Name))
                {
                    var sp = src.PList.Add(sd.Name, opt.Level, party);
                    if (sp != null)
                    {
                        sp.Ride(u);
                        spawned.Add((null, sp));
                    }
                }
            }
            u.Update();
            u.Status = "出撃";
            u.x = x;
            u.y = 1;
            u.Area = AreaFor(ud);
            u.HP = u.MaxHP;
            u.EN = u.MaxEN;
            src.Map.MapDataForUnit[x, 1] = u;
            p.Morale = 100;
            spawned.Add((u, p));
            return (u, p);
        }

        private void DespawnAll()
        {
            foreach (var (u, p) in spawned)
            {
                if (u != null)
                {
                    try { src.UList.Delete(u.ID); } catch { }
                }
                try { src.PList.Delete(p.ID); } catch { }
            }
            spawned.Clear();
            for (var x = 1; x <= MapWidth; x++)
            {
                for (var y = 1; y <= MapHeight; y++)
                {
                    src.Map.MapDataForUnit[x, y] = null;
                }
            }
        }

        // ---- 計算 ----

        private static bool Usable(UnitWeapon w, string mode)
        {
            try { return w.IsWeaponAvailable(mode); }
            catch { return false; }
        }

        /// <summary>a から t への 1 回の攻撃の期待ダメージ（命中率 × ダメージ）の最大。距離は武器の射程の中で最も有利なもの</summary>
        private (double damage, double hit) Best(Unit a, Unit t, Func<UnitWeapon, bool> filter, int minDistance = 1, bool capByHp = true)
        {
            double best = 0, bestHit = 0;
            foreach (var w in a.Weapons)
            {
                if (!filter(w)) continue;
                for (var d = Math.Max(minDistance, w.WeaponMinRange()); d <= Math.Min(w.WeaponMaxRange(), MapWidth - 2); d++)
                {
                    try
                    {
                        src.Map.MapDataForUnit[t.x, t.y] = null;
                        t.x = a.x + d;
                        t.y = 1;
                        src.Map.MapDataForUnit[t.x, 1] = t;
                        var hit = Math.Min(100, w.HitProbability(t, true));
                        var dmg = w.ExpDamage(t, true);
                        if (capByHp) dmg = Math.Min(dmg, t.MaxHP);
                        var v = hit / 100.0 * dmg;
                        if (v > best)
                        {
                            best = v;
                            bestHit = hit;
                        }
                    }
                    catch { }
                }
            }
            return (best, bestHit);
        }

        /// <summary>ユニットに乗るパイロット（メインとサブ）が今のレベルで使える SP と、その SP を使えるパイロット</summary>
        private List<(SpecialPowerData sp, Pilot p)> AvailableSpecialPowers(Unit u)
        {
            var list = new List<(SpecialPowerData, Pilot)>();
            foreach (var p in u.Pilots.Concat(u.SupportPilots))
            {
                foreach (var s in p.Data.DefinedSpecialPowers())
                {
                    if (s.NecessaryLevel <= p.Level && src.SPDList.IsDefined(s.Name)) list.Add((src.SPDList.Item(s.Name), p));
                }
            }
            return list;
        }

        /// <summary>SP の効果（入れ子の SP を含む）の種類と強さ</summary>
        private IEnumerable<SpecialPowerEffect> Effects(SpecialPowerData sp, int depth = 0)
        {
            foreach (var e in sp.Effects)
            {
                if (e.strEffectType == "スペシャルパワー" && depth < 3 && src.SPDList.IsDefined(e.strEffectData))
                {
                    foreach (var n in Effects(src.SPDList.Item(e.strEffectData), depth + 1)) yield return n;
                }
                else
                {
                    yield return e;
                }
            }
        }

        private double EffectLevel(SpecialPowerData sp, string type) =>
            Effects(sp).Where(e => e.strEffectType == type).Select(e => e.dblEffectLevel <= 0 ? 1 : e.dblEffectLevel).DefaultIfEmpty(0).Max();

        /// <summary>SP の効果をユニットにかける（入れ子の SP も展開する）</summary>
        private void Apply(Unit u, SpecialPowerData sp, int depth = 0)
        {
            u.MakeSpecialPowerInEffect(sp.Name);
            foreach (var e in sp.Effects.Where(e => e.strEffectType == "スペシャルパワー" && depth < 3 && src.SPDList.IsDefined(e.strEffectData)))
            {
                Apply(u, src.SPDList.Item(e.strEffectData), depth + 1);
            }
        }

        /// <summary>効果の種類ごとに、最も強い SP を 1 つずつかける。かけた SP の名前を返す</summary>
        private List<string> ApplyBest(Unit u, params string[] types)
        {
            var sps = AvailableSpecialPowers(u);
            var applied = new List<string>();
            foreach (var type in types)
            {
                var best = sps.Select(x => x.sp).Where(sp => EffectLevel(sp, type) > 0).OrderByDescending(sp => EffectLevel(sp, type)).FirstOrDefault();
                if (best != null && !applied.Contains(best.Name))
                {
                    Apply(u, best);
                    applied.Add(best.Name);
                }
            }
            return applied;
        }

        private Dictionary<string, double> Evaluate(PilotData pd, UnitData ud)
        {
            var key = pd.Name + "/" + ud.Name;
            if (cache.TryGetValue(key, out var cached)) return cached;
            var r = new Dictionary<string, double>();
            if (ud.IsFeatureAvailable("追加パイロット") && ud.IsFeatureAvailable("パイロット能力付加"))
            {
                // SRCCore がスタックオーバーフローで落ちる（https://github.com/7474/SRC/issues/1407）
                skipped++;
                cache[key] = r;
                return r;
            }
            try
            {
                var kind = TerrainFor(ud);

                // ザコ掃討: 補給なしで 30 回行動して倒せる名前なしの数（相手の HP を減らしながら数える）
                SetTerrain(kind);
                var (a, _) = Spawn(pd, ud, "味方", 1);
                var zakoOpp = zakoOpponent.p.Name != pd.Name ? zakoOpponent : zakoAlternative;
                var (z, _) = Spawn(zakoOpp.p, zakoOpp.u, "敵", 2);
                var en = a.EN;
                var bullets = a.Weapons.ToDictionary(w => w.WeaponNo(), w => w.MaxBullet());
                double zakoHp = z.MaxHP, kills = 0;
                var attacks = 0;
                for (var i = 0; i < 30; i++)
                {
                    double bv = 0;
                    UnitWeapon bw = null;
                    foreach (var w in a.Weapons)
                    {
                        if (!Usable(w, "移動前") || w.WeaponENConsumption() > en || (w.MaxBullet() > 0 && bullets[w.WeaponNo()] <= 0)) continue;
                        var (f, _) = Best(a, z, x => x == w);
                        if (f > bv)
                        {
                            bv = f;
                            bw = w;
                        }
                    }
                    if (bw == null) break;
                    attacks++;
                    en -= bw.WeaponENConsumption();
                    if (bw.MaxBullet() > 0) bullets[bw.WeaponNo()]--;
                    zakoHp -= bv;
                    while (zakoHp <= 0)
                    {
                        kills++;
                        zakoHp += z.MaxHP;
                    }
                }
                r["ザコ掃討"] = kills + (1 - zakoHp / z.MaxHP);
                r["攻撃回数"] = attacks;
                r["移動後の火力"] = Best(a, z, w => Usable(w, "移動後")).damage;
                r["長距離"] = Best(a, z, w => Usable(w, "移動前"), 3).damage;
                r["遊撃"] = r["移動後の火力"] * a.Speed / 4.0;
                var cover = 0;
                for (var d = 1; d <= 3; d++)
                {
                    if (a.Weapons.Any(w => Usable(w, "移動前") && w.WeaponMinRange() <= d && w.WeaponMaxRange() >= d)) cover++;
                }
                r["反撃できる距離"] = cover;
                DespawnAll();

                // ボス戦: ボスの候補それぞれに、気力 130 で、ダメージ・命中を上げる SP を 1 つずつ使ったときの 1 回の期待ダメージ ÷ ボスの最大 HP（平均）
                double ratioSum = 0, hitSum = 0, dmgSum = 0;
                var usedSp = new HashSet<string>();
                // 自分自身はボスの候補から外す（同じパイロットを 2 人作れない）
                var panel = bossPanel.Where(b => b.p.Name != pd.Name).ToList();
                foreach (var boss in panel)
                {
                    SetTerrain(kind);
                    var (ab, ap) = Spawn(pd, ud, "味方", 1);
                    var (b, _) = Spawn(boss.p, boss.u, "敵", 2);
                    ap.Morale = 130;
                    foreach (var n in ApplyBest(ab, "ダメージ増加", "絶対命中", "命中強化")) usedSp.Add(n);
                    var (fb, hb) = Best(ab, b, w => Usable(w, "移動前"), 1, false);
                    ratioSum += fb / Math.Max(1, b.MaxHP);
                    hitSum += hb;
                    dmgSum += fb;
                    DespawnAll();
                }
                r["ボス戦"] = ratioSum / panel.Count;
                r["ボスへの期待ダメージ"] = dmgSum / panel.Count;
                r["ボスへの命中率"] = hitSum / panel.Count;
                r["ボス戦のSP"] = usedSp.Count;

                // 回避・囮: 同じ帯の代表の組から受ける攻撃に、倒されるまで何回耐えるか（回避・被ダメージを下げる SP を使い、絶対回避の SP は使える回数を足す）
                SetTerrain(kind);
                var (ad, _) = Spawn(pd, ud, "味方", 1);
                var opp = BandOpponent(Band(pd), pd.Name);
                var (o, _) = Spawn(opp.p, opp.u, "敵", 2);
                ApplyBest(ad, "回避強化", "被ダメージ低下");
                var (od, oh) = Best(o, ad, w => Usable(w, "移動前"), 1, false);
                var absoluteEvasion = 0;
                foreach (var p in ad.Pilots.Concat(ad.SupportPilots))
                {
                    var cheapest = AvailableSpecialPowers(ad).Where(x => x.p == p && EffectLevel(x.sp, "絶対回避") > 0)
                        .Select(x => p.Data.SpecialPowerCost(x.sp.Name)).Where(c => c > 0).DefaultIfEmpty(0).Min();
                    if (cheapest > 0) absoluteEvasion += p.MaxSP / cheapest;
                }
                r["相手の命中率"] = oh;
                r["回避・囮"] = ad.MaxHP / Math.Max(1, od) + absoluteEvasion;
                r["絶対回避の回数"] = absoluteEvasion;
                DespawnAll();

                // 支援: 味方に効く SP の数（乗る全員）と、修理・補給・回復のアビリティ
                var riders = new[] { pd }.Concat(subPilots.TryGetValue(ud.Name, out var subs) ? subs : new List<PilotData>()).Distinct();
                var allySp = riders.SelectMany(p => p.DefinedSpecialPowers()).Select(s => s.Name).Distinct()
                    .Count(n => src.SPDList.IsDefined(n) && (src.SPDList.Item(n).TargetType ?? "").Contains("味方"));
                var support = ud.Abilities.Count(ab => Regex.IsMatch(ab.Name ?? "", "修理|補給|回復"));
                r["支援"] = allySp + 2 * support;
                r["地形"] = kind == "陸" ? 0 : kind == "水" ? 1 : 2;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"評価できない: {key}: {ex.Message}");
                r.Clear();
                DespawnAll();
            }
            cache[key] = r;
            return r;
        }

        // ---- 相手 ----

        private static (PilotData p, UnitData u) Medoid(IEnumerable<Pair> source)
        {
            var l = source.Where(x => x.Unit.Weapons.Any()).ToList();
            double Median(Func<Pair, double> f) => l.Select(f).OrderBy(v => v).ElementAt(l.Count / 2);
            var mh = Median(x => x.Unit.HP);
            var ma = Median(x => x.Unit.Armor);
            var mm = Median(x => x.Unit.Mobility);
            var mp = Median(x => x.Unit.Weapons.Max(w => w.Power));
            var ms = Median(x => x.Pilot.Infight + x.Pilot.Shooting);
            var best = l.OrderBy(x =>
                Math.Abs(x.Unit.HP / mh - 1) + Math.Abs(x.Unit.Armor / ma - 1) + Math.Abs(x.Unit.Mobility / mm - 1)
                + Math.Abs(x.Unit.Weapons.Max(w => w.Power) / mp - 1) + Math.Abs((x.Pilot.Infight + x.Pilot.Shooting) / ms - 1))
                .ThenBy(x => x.Pilot.Name, StringComparer.Ordinal)
                .First();
            return (best.Pilot, best.Unit);
        }

        /// <summary>能力値が中央に近いパイロットとユニットを別々に選ぶ</summary>
        private (PilotData p, UnitData u) Representative(Func<PilotData, bool> pf, Func<UnitData, bool> uf)
        {
            var ps = src.PDList.Items.Where(pf).ToList();
            var us = src.UDList.Items.Where(u => uf(u) && AutoPairable(u) && !u.IsFeatureAvailable("パイロット能力付加") && u.Weapons.Any()
                && !u.IsFeatureAvailable("母艦") && (u.Adaption ?? "").Length > 1 && u.Adaption[1] != '-').ToList();
            double Median(IEnumerable<double> v)
            {
                var a = v.OrderBy(x => x).ToArray();
                return a[a.Length / 2];
            }
            var mh = Median(us.Select(u => (double)u.HP));
            var ma = Median(us.Select(u => (double)u.Armor));
            var mm = Median(us.Select(u => (double)u.Mobility));
            var mp = Median(us.Select(u => (double)u.Weapons.Max(w => w.Power)));
            var unit = us.OrderBy(u => Math.Abs(u.HP / mh - 1) + Math.Abs(u.Armor / ma - 1) + Math.Abs(u.Mobility / mm - 1) + Math.Abs(u.Weapons.Max(w => w.Power) / mp - 1))
                .ThenBy(u => u.Name, StringComparer.Ordinal).First();
            var ms = Median(ps.Select(p => (double)p.Infight + p.Shooting));
            var mt = Median(ps.Select(p => (double)p.Technique));
            var mhd = Median(ps.Select(p => (double)p.Hit + p.Dodge));
            var pilot = ps.OrderBy(p => Math.Abs((p.Infight + p.Shooting) / ms - 1) + Math.Abs(p.Technique / mt - 1) + Math.Abs((p.Hit + p.Dodge) / mhd - 1))
                .ThenBy(p => p.Name, StringComparer.Ordinal).First();
            return (pilot, unit);
        }

        private void ChooseOpponents()
        {
            zakoOpponent = Representative(p => IsZako(p) && p.Infight + p.Shooting > 2, u => u.ExpValue >= 40 && u.ExpValue < 70);
            zakoAlternative = Representative(p => IsZako(p) && p.Infight + p.Shooting > 2 && p.Name != zakoOpponent.p.Name, u => u.ExpValue >= 40 && u.ExpValue < 70);
            // ボス戦の相手: 役割表の「ボス」。無ければパックの経験値 200 以上の代表
            foreach (var r in roles.Where(r => r.Role == "ボス"))
            {
                if (!src.PDList.IsDefined(r.Name)) continue;
                var unit = r.Unit != "" && unitByName.ContainsKey(r.Unit) ? unitByName[r.Unit]
                    : pairs.FirstOrDefault(x => x.Pilot.Name == r.Name)?.Unit;
                if (unit == null)
                {
                    Console.Error.WriteLine($"ボスのユニットが決まらない: {r.Name}");
                    continue;
                }
                bossPanel.Add((src.PDList.Item(r.Name), unit));
            }
            if (bossPanel.Count == 0)
            {
                bossPanel.Add(Representative(p => !IsZako(p) && !IsSupport(p) && PilotRole(p) == "戦闘" && p.ExpValue >= 200, u => u.ExpValue >= 150 && u.ExpValue < 250));
            }
        }

        /// <summary>同じ帯の代表の組。評価する組のパイロットと同じなら、それを除いた中の代表</summary>
        private (PilotData p, UnitData u) BandOpponent(string band, string exceptPilot = null)
        {
            var key = band + "/" + exceptPilot;
            if (!bandOpponent.TryGetValue(band, out var o))
            {
                o = BandMedoid(band, null);
                bandOpponent[band] = o;
            }
            if (exceptPilot != null && o.p.Name == exceptPilot)
            {
                if (!bandOpponent.TryGetValue(key, out o))
                {
                    o = BandMedoid(band, exceptPilot);
                    bandOpponent[key] = o;
                }
            }
            return o;
        }

        private (PilotData p, UnitData u) BandMedoid(string band, string exceptPilot)
        {
            var l = pairs.Where(x => Band(x.Pilot) == band && x.Pilot.Name != exceptPilot && (PilotRole(x.Pilot) == "戦闘" || PilotRole(x.Pilot) == "名前なし")
                && TerrainFor(x.Unit) == "陸" && !x.Unit.IsFeatureAvailable("パイロット能力付加")).ToList();
            return l.Count >= 5 ? Medoid(l) : zakoOpponent;
        }

        // ---- 比べる組 ----

        private void BuildReference()
        {
            var targets = TargetPairs().ToList();
            foreach (var band in targets.Select(t => Band(t.Pilot)).Distinct().OrderBy(b => Array.IndexOf(Bands, b)))
            {
                var rnd = new Random(opt.Seed);
                var l = pairs.Where(x => Band(x.Pilot) == band && PilotRole(x.Pilot) != "艦長")
                    .OrderBy(x => x.Pilot.Name, StringComparer.Ordinal).ThenBy(x => x.Unit.Name, StringComparer.Ordinal)
                    .Select(x => (x, k: rnd.Next())).OrderBy(x => x.k).Select(x => x.x)
                    .Take(opt.Sample).ToList();
                bandReference[band] = l.Select(x => Evaluate(x.Pilot, x.Unit)).Where(d => d.Count > 0).ToList();
                Console.Error.WriteLine($"比べる組 {band}: {bandReference[band].Count}");
            }
        }

        private IEnumerable<Pair> TargetPairs() =>
            pairs.Where(x => pilotWork.TryGetValue(x.Pilot.Name, out var w) && works.Contains(w));

        /// <summary>上位何 % か（同じ値は平均の順位）。比べる組が 20 組に満たなければ -1</summary>
        private double Percentile(string band, string use, double v)
        {
            var l = bandReference[band];
            if (l.Count < 20) return -1;
            if (use == "支援" && v <= 0) return -1;
            return 100.0 * (l.Count(d => d.GetValueOrDefault(use) > v) + 0.5 * l.Count(d => d.GetValueOrDefault(use) == v)) / l.Count;
        }

        private static string Grade(double pct) => pct < 0 ? "-" : pct <= 5 ? "S" : pct <= 25 ? "A" : pct <= 60 ? "B" : pct <= 85 ? "C" : "D";

        // ---- 出力 ----

        private void Write(TextWriter o)
        {
            o.WriteLine($"前提: パック {opt.Pack}、パイロットのレベル {opt.Level}、比べる組は同じ帯から最大 {opt.Sample} 組（乱数の種 {opt.Seed}）。");
            o.WriteLine();
            foreach (var n in worksNotes) o.WriteLine($"- {n}");
            o.WriteLine($"- ザコ掃討の相手: {zakoOpponent.p.Name}／{zakoOpponent.u.Name}");
            o.WriteLine($"- ボス戦の相手（{bossPanel.Count} 組の平均）: " + string.Join("、", bossPanel.Select(b => $"{b.p.Name}／{b.u.Name}")));
            o.WriteLine("- 回避・囮の相手（同じ帯の代表の組）: " + string.Join("、", Bands.Where(bandReference.ContainsKey).Select(b =>
            {
                var x = BandOpponent(b);
                return $"{b}: {x.p.Name}／{x.u.Name}";
            })));
            o.WriteLine("- 組の決め方: " + string.Join("、", pairs.GroupBy(x => x.How).Select(g => $"{g.Key} {g.Count()}")));
            o.WriteLine("- 比べた組の数: " + string.Join("、", Bands.Where(bandReference.ContainsKey).Select(b => $"{b} {bandReference[b].Count}")));
            o.WriteLine($"- 追加パイロットとパイロット能力付加を両方持つユニットは評価しない（https://github.com/7474/SRC/issues/1407 。{skipped} 組）。追加パイロット・暴走時パイロットとして使われるパイロットは単独では評価しない");

            foreach (var w in works)
            {
                o.WriteLine();
                o.WriteLine($"### {w}");
                o.WriteLine();
                var tp = TargetPairs().Where(t => pilotWork[t.Pilot.Name] == w && (PilotRole(t.Pilot) == "戦闘" || PilotRole(t.Pilot) == "名前なし"))
                    .OrderBy(t => t.How == "役割表" ? 0 : 1).ThenByDescending(t => IsZako(t.Pilot) ? -1 : t.Pilot.ExpValue)
                    .ThenBy(t => t.Pilot.Name, StringComparer.Ordinal).Take(14).ToList();
                if (tp.Count == 0)
                {
                    o.WriteLine("（組が無い）");
                    continue;
                }
                o.WriteLine("| 組（パイロット／ユニット） | 組の決め方 | 帯 | " + string.Join(" | ", Uses) + " | 役割 |");
                o.WriteLine("|---|---|---|" + string.Concat(Uses.Select(_ => "---|")) + "---|");
                foreach (var t in tp)
                {
                    var e = Evaluate(t.Pilot, t.Unit);
                    var band = Band(t.Pilot);
                    if (e.Count == 0)
                    {
                        o.WriteLine($"| {t.Pilot.Name}／{t.Unit.Name} | {t.How} | {band} | " + string.Join(" | ", Uses.Select(_ => "?")) + " | 評価できない |");
                        continue;
                    }
                    var grades = Uses.Select(u => Grade(Percentile(band, u, e[u]))).ToList();
                    var role = Uses.Where(u =>
                    {
                        var q = Percentile(band, u, e[u]);
                        return q >= 0 && q <= 25;
                    }).ToList();
                    var sub = subPilots.TryGetValue(t.Unit.Name, out var subs) ? "（サブ: " + string.Join("・", subs.Where(s => s.Name != t.Pilot.Name).Select(s => s.Name)) + "）" : "";
                    o.WriteLine($"| {t.Pilot.Name}／{t.Unit.Name}{sub} | {t.How} | {band} | " + string.Join(" | ", grades)
                        + $" | {(bandReference[band].Count < 20 ? "比べる組が足りない" : role.Count > 0 ? string.Join("・", role) : "見当たらない")} |");
                }
                o.WriteLine();
                o.WriteLine("| 組 | ボスの HP を 1 回で削る割合 | ボスへの期待ダメージ | ボスへの命中率 | 30 回の行動で倒せるザコ | 攻撃回数 | 同じ帯の相手の命中率 | 同じ帯の相手の攻撃に耐える回数（うち絶対回避） | 移動後の火力 | 射程 3 以上の火力 | 反撃できる距離（1〜3） |");
                o.WriteLine("|---|--:|--:|--:|--:|--:|--:|--:|--:|--:|--:|");
                foreach (var t in tp)
                {
                    var e = Evaluate(t.Pilot, t.Unit);
                    if (e.Count == 0) continue;
                    o.WriteLine($"| {t.Pilot.Name}／{t.Unit.Name} | {e["ボス戦"]:0.00} | {e["ボスへの期待ダメージ"]:0} | {e["ボスへの命中率"]:0}% | {e["ザコ掃討"]:0.0} | {e["攻撃回数"]:0} | {e["相手の命中率"]:0}% | {e["回避・囮"]:0.0}（{e["絶対回避の回数"]:0}） | {e["移動後の火力"]:0} | {e["長距離"]:0} | {e["反撃できる距離"]:0} |");
                }
                // 逆転: 経験値が上の組が、下の組に全ての使いどころで並ばれるか上回られている
                var named = tp.Where(t => !IsZako(t.Pilot)).Select(t => (t, e: Evaluate(t.Pilot, t.Unit))).Where(x => x.e.Count > 0).ToList();
                var inversions = new List<string>();
                foreach (var a in named)
                {
                    foreach (var b in named)
                    {
                        if (a.t.Pilot.ExpValue <= b.t.Pilot.ExpValue) continue;
                        if (Uses.All(u => b.e[u] >= a.e[u]) && Uses.Where(u => u != "支援").Any(u => b.e[u] > a.e[u] * 1.1))
                        {
                            inversions.Add($"{a.t.Pilot.Name}／{a.t.Unit.Name}（経験値 {a.t.Pilot.ExpValue}）は、{b.t.Pilot.Name}／{b.t.Unit.Name}（経験値 {b.t.Pilot.ExpValue}）に全ての使いどころで並ばれるか上回られている");
                        }
                    }
                }
                o.WriteLine();
                o.WriteLine("逆転（経験値が上の組が、下の組に全ての使いどころで並ばれるか上回られている）: " + (inversions.Count == 0 ? "なし" : ""));
                foreach (var s in inversions.Take(6)) o.WriteLine($"- {s}");
            }
        }
    }
}
