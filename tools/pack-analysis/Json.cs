// 組の評価の JSON（--json）、役割表の名前の確かめ（--check-roles）、役割表のひな形（--roles-template）（7474/SRC#1607）
using SRCCore.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace PackAnalysis
{
    internal sealed partial class Analysis
    {
        /// <summary>JSON の書式の版。項目を変えたら上げる（データビューアの「比べる」が見る）</summary>
        public const int JsonFormat = 1;

        /// <summary>格（役割表の格、無ければ格の一覧の格）</summary>
        private string RankOf(Pair t)
        {
            var declared = roles.FirstOrDefault(r => r.Kind == "pilot" && r.Name == t.Pilot.Name && r.Rank != "");
            if (declared != null) return declared.Rank;
            var tier = TierPairs().FirstOrDefault(x => x.pair.Pilot.Name == t.Pilot.Name);
            return tier.tier ?? "";
        }

        private static readonly string[] StatKeys = { "格闘", "射撃", "命中", "回避", "技量", "反応" };

        /// <summary>組の評価を JSON に書く。Markdown と同じ組（作品ごとに WorkPairs）</summary>
        private void WriteJson(string path)
        {
            var dir = Path.GetDirectoryName(Path.GetFullPath(path));
            Directory.CreateDirectory(dir);
            using var stream = File.Create(path);
            using var w = new Utf8JsonWriter(stream, new JsonWriterOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, Indented = false });
            w.WriteStartObject();
            w.WriteNumber("format", JsonFormat);
            w.WriteString("pack", opt.Pack);
            w.WriteString("generatedAt", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            w.WriteNumber("level", opt.Level);
            w.WriteNumber("seed", opt.Seed);
            w.WriteNumber("sample", opt.Sample);
            WriteStrings(w, "uses", Uses);
            WriteStrings(w, "indices", Indices.Select(x => x.label));
            WriteStrings(w, "bands", Bands);

            w.WriteStartObject("opponents");
            w.WriteString("zako", $"{zakoOpponent.p.Name}／{zakoOpponent.u.Name}");
            WriteStrings(w, "boss", bossPanel.Select(b => $"{b.p.Name}／{b.u.Name}"));
            w.WriteStartObject("band");
            foreach (var b in Bands.Where(bandReference.ContainsKey))
            {
                var x = BandOpponent(b);
                w.WriteString(b, $"{x.p.Name}／{x.u.Name}");
            }
            w.WriteEndObject();
            w.WriteEndObject();

            w.WriteStartObject("referenceCounts");
            foreach (var b in Bands.Where(bandReference.ContainsKey))
            {
                w.WriteNumber(b, bandReference[b].Count);
            }
            w.WriteEndObject();

            // 作品ごとの、役割表で組んだ組の数（役割表の無い作品を見つけるのに使う）
            w.WriteStartArray("works");
            foreach (var work in works)
            {
                var all = TargetPairs().Where(t => pilotWork[t.Pilot.Name] == work).ToList();
                w.WriteStartObject();
                w.WriteString("name", work);
                w.WriteNumber("pairs", all.Count);
                w.WriteNumber("declared", all.Count(t => t.How == "役割表"));
                w.WriteEndObject();
            }
            w.WriteEndArray();

            w.WriteStartArray("pairs");
            foreach (var work in works)
            {
                foreach (var t in WorkPairs(work))
                {
                    WritePair(w, work, t);
                }
            }
            w.WriteEndArray();
            w.WriteEndObject();
        }

        private void WritePair(Utf8JsonWriter w, string work, Pair t)
        {
            var e = Evaluate(t.Pilot, t.Unit);
            var band = Band(t.Pilot);
            var p = t.Pilot;
            var u = t.Unit;
            w.WriteStartObject();
            w.WriteString("work", work);
            w.WriteString("how", t.How);
            w.WriteString("band", band);
            w.WriteString("role", PilotRole(p));
            w.WriteString("rank", RankOf(t));

            w.WriteStartObject("pilot");
            w.WriteString("name", p.Name);
            w.WriteString("nickname", p.Nickname ?? "");
            w.WriteString("bitmap", p.Bitmap0 ?? "");
            w.WriteString("class", p.Class ?? "");
            w.WriteNumber("exp", p.ExpValue);
            w.WriteNumber("sp", p.SP);
            w.WriteStartObject("stats");
            var stats = new[] { p.Infight, p.Shooting, p.Hit, p.Dodge, p.Technique, p.Intuition };
            for (var i = 0; i < StatKeys.Length; i++)
            {
                w.WriteNumber(StatKeys[i], stats[i]);
            }
            w.WriteEndObject();
            WriteStrings(w, "specialPowers", p.DefinedSpecialPowers().Select(x => x.Name));
            w.WriteEndObject();

            w.WriteStartObject("unit");
            w.WriteString("name", u.Name);
            w.WriteString("nickname", u.Nickname ?? "");
            w.WriteString("bitmap", u.Bitmap0 ?? "");
            w.WriteString("class", u.Class ?? "");
            w.WriteNumber("exp", u.ExpValue);
            w.WriteNumber("hp", u.HP);
            w.WriteNumber("en", u.EN);
            w.WriteNumber("armor", u.Armor);
            w.WriteNumber("mobility", u.Mobility);
            w.WriteNumber("speed", u.Speed);
            w.WriteString("size", u.Size ?? "");
            var top = u.Weapons.Where(x => !IsMap(x)).OrderByDescending(x => x.Power).FirstOrDefault();
            if (top != null)
            {
                w.WriteStartObject("topWeapon");
                w.WriteString("name", top.Name);
                w.WriteNumber("power", top.Power);
                w.WriteNumber("minRange", top.MinRange);
                w.WriteNumber("maxRange", top.MaxRange);
                w.WriteEndObject();
            }
            w.WriteEndObject();

            if (subPilots.TryGetValue(u.Name, out var subs))
            {
                WriteStrings(w, "subPilots", subs.Where(s => s.Name != p.Name).Select(s => s.Name));
            }

            w.WriteBoolean("evaluated", e.Count > 0);
            if (e.Count > 0)
            {
                var enough = bandReference.TryGetValue(band, out var reference) && reference.Count >= 20;
                w.WriteStartObject("grades");
                foreach (var use in Uses)
                {
                    w.WriteString(use, enough ? Grade(Percentile(band, use, e[use])) : "-");
                }
                w.WriteEndObject();
                w.WriteStartObject("percentiles");
                foreach (var use in Uses)
                {
                    var pct = enough ? Percentile(band, use, e[use]) : -1;
                    if (pct >= 0)
                    {
                        w.WriteNumber(use, Math.Round(pct, 1));
                    }
                }
                w.WriteEndObject();
                // 指数（同じ帯の比べる組の中央値 = 100）。無い（持たない・比べる組が足りない）ものは書かない
                w.WriteStartObject("indices");
                foreach (var x in Indices)
                {
                    var eff = Index(band, x.eff, e.GetValueOrDefault(x.eff));
                    var raw = Index(band, x.raw, e.GetValueOrDefault(x.raw));
                    w.WriteStartObject(x.label);
                    if (eff >= 0) w.WriteNumber("effective", Math.Round(eff));
                    if (raw >= 0) w.WriteNumber("simple", Math.Round(raw));
                    w.WriteEndObject();
                }
                w.WriteEndObject();
                var gap = Gap(band, e);
                if (!double.IsNaN(gap))
                {
                    w.WriteNumber("gap", Math.Round(gap));
                }
                // 計算の元の値（ボスの HP を 1 回で削る割合・命中率など）
                w.WriteStartObject("values");
                foreach (var (k, v) in e.OrderBy(x => x.Key, StringComparer.Ordinal))
                {
                    if (!double.IsNaN(v) && !double.IsInfinity(v))
                    {
                        w.WriteNumber(k, Math.Round(v, 2));
                    }
                }
                w.WriteEndObject();
            }
            w.WriteEndObject();
        }

        private static void WriteStrings(Utf8JsonWriter w, string name, IEnumerable<string> values)
        {
            w.WriteStartArray(name);
            foreach (var v in values)
            {
                w.WriteStringValue(v);
            }
            w.WriteEndArray();
        }

        /// <summary>
        /// 役割表と格の一覧の名前（パイロット・ユニット）がデータにあるかを確かめる。無ければ「ファイル(行): warning: …」で出し、false
        /// </summary>
        public bool CheckRoles(TextWriter o)
        {
            LoadRoles();
            LoadData();
            var ok = true;
            void Warn(string file, int line, string message)
            {
                o.WriteLine($"{file}({line}): warning: {message}");
                ok = false;
            }
            foreach (var r in roles)
            {
                if (r.Kind == "pilot" && !src.PDList.IsDefined2(r.Name))
                {
                    Warn(opt.RolesCsv, r.LineNo, $"パイロット「{r.Name}」がデータにありません");
                }
                if (r.Kind == "unit" && !unitByName.ContainsKey(r.Name))
                {
                    Warn(opt.RolesCsv, r.LineNo, $"ユニット「{r.Name}」がデータにありません");
                }
                foreach (var unit in (r.Unit ?? "").Split(';').Select(x => x.Trim()).Where(x => x != ""))
                {
                    if (!unitByName.ContainsKey(unit))
                    {
                        Warn(opt.RolesCsv, r.LineNo, $"乗るユニット「{unit}」がデータにありません（{r.Name}）");
                    }
                }
            }
            if (opt.TiersFile != null)
            {
                var lineNo = 0;
                foreach (var line in File.ReadLines(opt.TiersFile))
                {
                    lineNo++;
                    if (line.Trim() == "" || line.StartsWith("#")) continue;
                    var c = line.Split('\t');
                    if (c.Length < 2)
                    {
                        Warn(opt.TiersFile, lineNo, "格とパイロットをタブで区切ってください");
                        continue;
                    }
                    if (!src.PDList.IsDefined2(c[1]))
                    {
                        Warn(opt.TiersFile, lineNo, $"パイロット「{c[1]}」がデータにありません");
                    }
                    if (c.Length > 2 && c[2] != "" && !unitByName.ContainsKey(c[2]))
                    {
                        Warn(opt.TiersFile, lineNo, $"乗るユニット「{c[2]}」がデータにありません（{c[1]}）");
                    }
                }
            }
            if (opt.WorksFile != null && File.Exists(opt.WorksFile))
            {
                var lineNo = 0;
                var dirs = Directory.EnumerateDirectories(opt.PackDir).Select(Path.GetFileName).ToHashSet();
                foreach (var line in File.ReadLines(opt.WorksFile))
                {
                    lineNo++;
                    // 作品のフォルダの名前の後ろに、タブで注記（選んだ根拠など）を書ける
                    var t = line.Split('\t')[0].Trim();
                    if (t == "" || t.StartsWith("#")) continue;
                    if (!dirs.Contains(t))
                    {
                        Warn(opt.WorksFile, lineNo, $"作品のフォルダ「{t}」がパックにありません");
                    }
                }
            }
            o.WriteLine(ok ? "OK: 役割表・格の一覧・対象作品の名前はすべてデータにあります" : "名前がデータに無いものがあります");
            return ok;
        }

        /// <summary>
        /// 作品の、自動で組んだ組を役割表（CSV）の行の形で出す。格・系統は空にし、手で埋める。役割表にある組は「#」で出す
        /// </summary>
        public void WriteRolesTemplate(TextWriter o)
        {
            LoadRoles();
            LoadData();
            BuildPairs();
            var work = opt.RolesTemplate;
            o.WriteLine($"# {work} の役割表のひな形。格（主人公格・準主役格・サブキャラ格・端役格・名前なし）と系統を埋め、要らない行は消す");
            o.WriteLine("# 種類,名前,役割,格,系統,乗るユニット");
            var list = pairs.Where(t => pilotWork.TryGetValue(t.Pilot.Name, out var w) && w == work)
                .OrderByDescending(t => IsZako(t.Pilot) ? -1 : t.Pilot.ExpValue).ThenBy(t => t.Pilot.Name, StringComparer.Ordinal).ToList();
            foreach (var t in list)
            {
                var mark = t.How == "役割表" ? "# 役割表にある: " : "";
                o.WriteLine($"{mark}pilot,{t.Pilot.Name},{PilotRole(t.Pilot)},,,{t.Unit.Name}");
            }
            if (list.Count == 0)
            {
                o.WriteLine($"# 作品「{work}」の組が見つかりません（作品のフォルダの名前を確かめてください）");
            }
        }
    }
}
