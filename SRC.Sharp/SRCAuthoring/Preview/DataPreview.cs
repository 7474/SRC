using SRCCore.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace SRCAuthoring.Preview
{
    /// <summary>
    /// データのプレビュー（SRC#Studio。7474/SRC#1643）。データのファイル（robot.txt・unit.txt・pilot.txt）を SRCCore の読み込み処理で読み、
    /// そのファイルで定義したユニット・パイロットの能力値・特殊能力・武器・アビリティを、定義の行の範囲と一緒に JSON にする。
    /// 先に読むファイル（汎用データの sp.txt など）を渡すと、スペシャルパワーの参照を解ける。
    /// </summary>
    public static class DataPreview
    {
        /// <summary>
        /// target のデータを読み、プレビューの JSON を返す。before は先に読むデータ（名前と中身）。
        /// 結果: { "kind": "unit"|"pilot"|"", "entries": [{ "name", "nickname", "startLine", "endLine", ... }], "errors": [...] }
        /// </summary>
        public static string ToJson(string targetName, string targetText, IEnumerable<(string name, string text)> before = null)
        {
            var src = new SRCCore.SRC { GUI = new SRCCore.TestLib.MockGUI() };
            var errors = new List<string>();
            foreach (var (name, text) in before ?? Enumerable.Empty<(string, string)>())
            {
                Load(src, name, text, errors, report: false);
            }
            var kind = KindOf(targetName);
            Load(src, targetName, targetText, errors, report: true);
            var lineCount = targetText.Replace("\r\n", "\n").Split('\n').Length;

            using var stream = new MemoryStream();
            using (var w = new Utf8JsonWriter(stream))
            {
                w.WriteStartObject();
                w.WriteString("kind", kind);
                w.WriteStartArray("entries");
                if (kind == "unit")
                {
                    var units = src.UDList.Items.Where(u => u.Location?.FileName == targetName).OrderBy(u => u.Location.Line).ToList();
                    for (var i = 0; i < units.Count; i++)
                    {
                        WriteUnit(w, units[i], i + 1 < units.Count ? units[i + 1].Location.Line - 1 : lineCount);
                    }
                }
                else if (kind == "pilot")
                {
                    var pilots = src.PDList.Items.Where(p => p.Location?.FileName == targetName).OrderBy(p => p.Location.Line).ToList();
                    for (var i = 0; i < pilots.Count; i++)
                    {
                        WritePilot(w, pilots[i], i + 1 < pilots.Count ? pilots[i + 1].Location.Line - 1 : lineCount);
                    }
                }
                w.WriteEndArray();
                w.WriteStartArray("errors");
                foreach (var e in errors.Distinct())
                {
                    w.WriteStringValue(e);
                }
                w.WriteEndArray();
                w.WriteEndObject();
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>ファイル名からデータの種類（unit・pilot。ほかは空）</summary>
        public static string KindOf(string fileName)
        {
            var name = Path.GetFileName(fileName ?? "").ToLowerInvariant();
            return name switch
            {
                "robot.txt" or "unit.txt" => "unit",
                "pilot.txt" => "pilot",
                _ => "",
            };
        }

        private static void Load(SRCCore.SRC src, string name, string text, List<string> errors, bool report)
        {
            try
            {
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
                switch (Path.GetFileName(name).ToLowerInvariant())
                {
                    case "sp.txt":
                        src.SPDList.Load(name, stream);
                        break;
                    case "robot.txt":
                    case "unit.txt":
                        src.UDList.Load(name, stream);
                        break;
                    case "pilot.txt":
                        src.PDList.Load(name, stream);
                        break;
                    case "item.txt":
                        src.IDList.Load(name, stream);
                        break;
                }
            }
            catch (Exception ex)
            {
                if (report)
                {
                    errors.Add((ex.InnerException ?? ex).Message);
                }
            }
            if (report)
            {
                errors.AddRange(src.DataErrors.Select(x => $"{x.line_num} 行目: {x.msg}" + (string.IsNullOrEmpty(x.dname) ? "" : $" [{x.dname}]")));
            }
            src.ClearDataError();
        }

        private static void WriteRange(Utf8JsonWriter w, DataLocation location, int endLine)
        {
            w.WriteNumber("startLine", location.Line);
            w.WriteNumber("endLine", Math.Max(location.Line, endLine));
        }

        private static string FeatureText(string name, double level, string data, bool hasLevel)
        {
            var s = name;
            if (hasLevel)
            {
                s += "Lv" + level.ToString(CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(data))
            {
                s += "=" + data;
            }
            return s;
        }

        private static void WriteUnit(Utf8JsonWriter w, UnitData u, int endLine)
        {
            w.WriteStartObject();
            w.WriteString("name", u.Name);
            w.WriteString("nickname", u.Nickname);
            WriteRange(w, u.Location, endLine);
            w.WriteString("class", u.Class);
            w.WriteString("bitmap", u.Bitmap0 ?? "");
            w.WriteNumber("hp", u.HP);
            w.WriteNumber("en", u.EN);
            w.WriteNumber("armor", u.Armor);
            w.WriteNumber("mobility", u.Mobility);
            w.WriteNumber("speed", u.Speed);
            w.WriteString("size", u.Size);
            w.WriteString("transportation", u.Transportation);
            w.WriteString("adaption", u.Adaption);
            w.WriteNumber("exp", u.ExpValue);
            w.WriteNumber("pilots", u.PilotNum);
            w.WriteNumber("items", u.ItemNum);
            w.WriteStartArray("features");
            foreach (var f in u.Features)
            {
                w.WriteStringValue(FeatureText(f.Name, f.Level, f.StrData, f.HasLevel));
            }
            w.WriteEndArray();
            w.WriteStartArray("weapons");
            foreach (var x in u.Weapons)
            {
                w.WriteStartObject();
                w.WriteString("name", x.Name);
                w.WriteNumber("power", x.Power);
                w.WriteString("range", x.MinRange == x.MaxRange ? x.MaxRange.ToString(CultureInfo.InvariantCulture) : $"{x.MinRange}-{x.MaxRange}");
                w.WriteNumber("hit", x.Precision);
                w.WriteNumber("bullet", x.Bullet);
                w.WriteNumber("en", x.ENConsumption);
                w.WriteNumber("morale", x.NecessaryMorale);
                w.WriteString("adaption", x.Adaption);
                w.WriteNumber("critical", x.Critical);
                w.WriteString("class", x.Class);
                if (x.Location != null)
                {
                    w.WriteNumber("line", x.Location.Line);
                }
                w.WriteEndObject();
            }
            w.WriteEndArray();
            w.WriteStartArray("abilities");
            foreach (var a in u.Abilities)
            {
                w.WriteStartObject();
                w.WriteString("name", a.Name);
                w.WriteString("range", a.MinRange == a.MaxRange ? a.MaxRange.ToString(CultureInfo.InvariantCulture) : $"{a.MinRange}-{a.MaxRange}");
                w.WriteNumber("stock", a.Stock);
                w.WriteNumber("en", a.ENConsumption);
                w.WriteString("class", a.Class);
                w.WriteEndObject();
            }
            w.WriteEndArray();
            w.WriteEndObject();
        }

        private static void WritePilot(Utf8JsonWriter w, PilotData p, int endLine)
        {
            w.WriteStartObject();
            w.WriteString("name", p.Name);
            w.WriteString("nickname", p.Nickname);
            WriteRange(w, p.Location, endLine);
            w.WriteString("class", p.Class);
            w.WriteString("sex", p.Sex);
            w.WriteString("bitmap", p.Bitmap0 ?? "");
            w.WriteString("adaption", p.Adaption);
            w.WriteNumber("exp", p.ExpValue);
            w.WriteString("personality", p.Personality);
            w.WriteStartObject("stats");
            w.WriteNumber("格闘", p.Infight);
            w.WriteNumber("射撃", p.Shooting);
            w.WriteNumber("命中", p.Hit);
            w.WriteNumber("回避", p.Dodge);
            w.WriteNumber("技量", p.Technique);
            w.WriteNumber("反応", p.Intuition);
            w.WriteEndObject();
            w.WriteNumber("sp", p.SP);
            w.WriteStartArray("specialPowers");
            foreach (var s in p.DefinedSpecialPowers())
            {
                w.WriteStringValue($"{s.Name}（Lv{s.NecessaryLevel}）");
            }
            w.WriteEndArray();
            w.WriteStartArray("skills");
            foreach (var s in p.Skills)
            {
                w.WriteStringValue(FeatureText(s.Name, s.Level, s.StrData, s.HasLevel) + (s.NecessaryLevel > 1 ? $"（Lv{s.NecessaryLevel}）" : ""));
            }
            w.WriteEndArray();
            w.WriteEndObject();
        }
    }
}
