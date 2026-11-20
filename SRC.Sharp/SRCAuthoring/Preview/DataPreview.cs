using SRCAuthoring.Maps;
using SRCAuthoring.Rules;
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
        /// <summary>標準のメッセージシチュエーション（一覧表の列）</summary>
        private static readonly string[] StandardCoverageSituations =
        {
            "攻撃", "攻撃(反撃)", "格闘", "射撃", "回避",
            "ダメージ小", "ダメージ中", "ダメージ大", "破壊", "脱出",
            "射程外", "サポートアタック", "サポートガード",
        };

        /// <summary>
        /// target のデータを読み、プレビューの JSON を返す。before は先に読むデータ（名前と中身）。
        /// 結果: { "kind": "unit"|"pilot"|"message"|"dialog"|"", "entries": [...], "errors": [...] }
        /// </summary>
        public static string ToJson(string targetName, string targetText, IEnumerable<(string name, string text)> before = null)
        {
            var src = new SRCCore.SRC { GUI = new SRCCore.TestLib.MockGUI() };
            var errors = new List<string>();
            var beforeList = (before ?? Enumerable.Empty<(string name, string text)>()).ToList();
            foreach (var (name, text) in beforeList.OrderBy(x => LoadOrder(x.name)))
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
                if (kind == "message" || kind == "dialog")
                {
                    w.WriteStartArray("standardSituations");
                    foreach (var s in StandardCoverageSituations)
                    {
                        w.WriteStringValue(s);
                    }
                    w.WriteEndArray();
                }
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
                else if (kind == "message")
                {
                    WriteMessages(w, src, targetName, lineCount, beforeList);
                }
                else if (kind == "dialog")
                {
                    WriteDialogs(w, src, targetName, lineCount, beforeList);
                }
                w.WriteEndArray();
                if (kind == "message" || kind == "dialog")
                {
                    WriteMissingPilots(w, src, targetName, kind, beforeList);
                }
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

        /// <summary>ファイル名からデータの種類（unit・pilot・message・dialog。ほかは空）</summary>
        public static string KindOf(string fileName)
        {
            var name = Path.GetFileName(fileName ?? "").ToLowerInvariant();
            return name switch
            {
                "robot.txt" or "unit.txt" => "unit",
                "pilot.txt" => "pilot",
                "pilot_message.txt" => "message",
                "pilot_dialog.txt" => "dialog",
                _ => "",
            };
        }

        private static int LoadOrder(string fileName)
        {
            return Path.GetFileName(fileName ?? "").ToLowerInvariant() switch
            {
                "sp.txt" => 0,
                "pilot.txt" => 1,
                "non_pilot.txt" => 2,
                "robot.txt" or "unit.txt" => 3,
                "item.txt" => 4,
                _ => 5,
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
                    case "non_pilot.txt":
                        src.NPDList.Load(name, stream);
                        break;
                    case "pilot_message.txt":
                        if (report)
                        {
                            src.MDList.Load(name, isEffect: false, stream);
                        }
                        break;
                    case "pilot_dialog.txt":
                        if (report)
                        {
                            src.DDList.Load(name, stream);
                        }
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
            if (u.Location != null)
            {
                w.WriteStartObject("statLines");
                w.WriteNumber("move", u.Location.FieldLine("移動可能地形"));
                w.WriteNumber("hp", u.Location.FieldLine("最大ＨＰ"));
                w.WriteNumber("adaption", u.Location.FieldLine("地形適応"));
                w.WriteEndObject();
            }
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
                if (a.Location != null)
                {
                    w.WriteNumber("line", a.Location.Line);
                }
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
            if (p.Location != null)
            {
                w.WriteStartObject("statLines");
                w.WriteNumber("adaption", p.Location.FieldLine("地形適応"));
                w.WriteNumber("stats", p.Location.FieldLine("格闘"));
                w.WriteEndObject();
            }
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

        private sealed class MessageItemModel
        {
            public string Situation { get; set; }
            public int Line { get; set; }
            public List<(string speaker, string nickname, string bitmap, string text, int line)> Messages { get; set; }
        }

        private static void WriteMessages(Utf8JsonWriter w, SRCCore.SRC src, string targetName, int lineCount, IReadOnlyList<(string name, string text)> before)
        {
            var pilotUnits = CollectPilotUnits(before);
            var list = src.MDList.Items.Where(m => m.Location?.FileName == targetName).OrderBy(m => m.Location.Line).ToList();
            for (var i = 0; i < list.Count; i++)
            {
                var m = list[i];
                var endLine = i + 1 < list.Count ? list[i + 1].Location.Line - 1 : lineCount;
                var (pilot, nickname, cls, bitmap) = ResolveSpeaker(src, m.Name);
                var items = m.Items.Select(x => new MessageItemModel
                {
                    Situation = x.Situation ?? "",
                    Line = x.Location?.Line ?? m.Location.Line,
                    Messages = new List<(string, string, string, string, int)>
                    {
                        (pilot?.Name ?? m.Name, nickname, bitmap, x.Message ?? "", x.Location?.Line ?? m.Location.Line),
                    },
                }).ToList();
                WriteMessageBlock(w, src, m.Name, nickname, cls, bitmap, m.Location, endLine, items, pilotUnits);
            }
        }

        private static void WriteDialogs(Utf8JsonWriter w, SRCCore.SRC src, string targetName, int lineCount, IReadOnlyList<(string name, string text)> before)
        {
            var pilotUnits = CollectPilotUnits(before);
            var list = src.DDList.Items.Where(d => d.Location?.FileName == targetName).OrderBy(d => d.Location.Line).ToList();
            for (var i = 0; i < list.Count; i++)
            {
                var d = list[i];
                var endLine = i + 1 < list.Count ? list[i + 1].Location.Line - 1 : lineCount;
                var tokens = (d.Name ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var primaryName = tokens.FirstOrDefault() ?? d.Name ?? "";
                var (_, nickname, cls, bitmap) = ResolveSpeaker(src, primaryName);
                var items = d.Items.Select(dlg =>
                {
                    var line = dlg.Location?.Line ?? dlg.Items.FirstOrDefault()?.Location?.Line ?? d.Location.Line;
                    var msgs = dlg.Items.Select(di =>
                    {
                        var sp = di.strName ?? "";
                        var lookup = sp.StartsWith("@", StringComparison.Ordinal) ? sp.Substring(1) : sp;
                        if (lookup == "システム")
                        {
                            return (sp, "システム", "-.bmp", di.strMessage ?? "", di.Location?.Line ?? line);
                        }
                        var (_, sNick, _, sBmp) = ResolveSpeaker(src, lookup);
                        return (sp, sNick, sBmp, di.strMessage ?? "", di.Location?.Line ?? line);
                    }).ToList();
                    return new MessageItemModel
                    {
                        Situation = dlg.Situation ?? "",
                        Line = line,
                        Messages = msgs,
                    };
                }).ToList();
                WriteMessageBlock(w, src, d.Name, tokens.Length > 1 ? d.Name : nickname, cls, bitmap, d.Location, endLine, items, pilotUnits);
            }
        }

        private static void WriteMissingPilots(
            Utf8JsonWriter w,
            SRCCore.SRC src,
            string targetName,
            string kind,
            IReadOnlyList<(string name, string text)> before)
        {
            var defined = new HashSet<string>(StringComparer.Ordinal);
            if (kind == "message")
            {
                foreach (var m in src.MDList.Items.Where(m => m.Location?.FileName == targetName))
                {
                    defined.Add(m.Name);
                }
            }
            else if (kind == "dialog")
            {
                foreach (var d in src.DDList.Items.Where(d => d.Location?.FileName == targetName))
                {
                    foreach (var token in (d.Name ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    {
                        defined.Add(token);
                    }
                }
            }
            var pilotUnits = CollectPilotUnits(before);
            w.WriteStartArray("missingPilots");
            foreach (var p in src.PDList.Items.Where(p => p.Location != null))
            {
                if (IsGenericName(p.Name))
                {
                    continue;
                }
                var messageType = p.Skills.FirstOrDefault(s => s.Name == "メッセージ" && !string.IsNullOrEmpty(s.StrData))?.StrData;
                if (defined.Contains(p.Name) || (!string.IsNullOrEmpty(p.Nickname) && defined.Contains(p.Nickname))
                    || (!string.IsNullOrEmpty(messageType) && defined.Contains(messageType)))
                {
                    continue;
                }
                var (units, weapons) = ResolveUnitsAndWeapons(src, p.Name, new List<MessageItemModel>(), pilotUnits);
                w.WriteStartObject();
                w.WriteString("name", p.Name);
                w.WriteString("nickname", p.Nickname ?? p.Name);
                w.WriteString("class", p.Class ?? "");
                w.WriteString("bitmap", p.Bitmap0 ?? "");
                w.WriteString("file", p.Location.FileName);
                w.WriteNumber("line", p.Location.Line);
                w.WriteStartArray("units");
                foreach (var u in units)
                {
                    w.WriteStringValue(u.Name);
                }
                w.WriteEndArray();
                w.WriteStartArray("weapons");
                foreach (var wp in weapons)
                {
                    w.WriteStringValue(wp.name);
                }
                w.WriteEndArray();
                w.WriteEndObject();
            }
            w.WriteEndArray();
        }

        private static bool IsGenericName(string name) =>
            !string.IsNullOrEmpty(name) && (name.Contains("(ザコ)", StringComparison.Ordinal) || name.Contains("(汎用)", StringComparison.Ordinal));

        private static void WriteMessageBlock(
            Utf8JsonWriter w,
            SRCCore.SRC src,
            string blockName,
            string nickname,
            string cls,
            string bitmap,
            DataLocation location,
            int endLine,
            List<MessageItemModel> items,
            Dictionary<string, List<string>> pilotUnits)
        {
            var lastLine = location.Line;
            foreach (var item in items)
            {
                if (item.Line > lastLine) lastLine = item.Line;
                foreach (var msg in item.Messages)
                {
                    if (msg.line > lastLine) lastLine = msg.line;
                }
            }
            var (units, weapons) = ResolveUnitsAndWeapons(src, blockName, items, pilotUnits);

            w.WriteStartObject();
            w.WriteString("name", blockName);
            w.WriteString("nickname", nickname);
            w.WriteString("class", cls);
            w.WriteString("bitmap", bitmap);
            WriteRange(w, location, endLine);
            w.WriteNumber("lastLine", lastLine);
            w.WriteStartArray("units");
            foreach (var u in units)
            {
                w.WriteStringValue(u.Name);
            }
            w.WriteEndArray();
            w.WriteStartArray("weapons");
            foreach (var wp in weapons)
            {
                w.WriteStartObject();
                w.WriteString("name", wp.name);
                w.WriteString("class", wp.cls);
                w.WriteString("range", wp.range);
                w.WriteEndObject();
            }
            w.WriteEndArray();

            w.WriteStartArray("items");
            foreach (var item in items)
            {
                w.WriteStartObject();
                w.WriteString("situation", item.Situation);
                w.WriteNumber("line", item.Line);
                w.WriteStartArray("messages");
                foreach (var msg in item.Messages)
                {
                    w.WriteStartObject();
                    w.WriteString("speaker", msg.speaker);
                    w.WriteString("nickname", msg.nickname);
                    w.WriteString("bitmap", msg.bitmap);
                    w.WriteString("text", msg.text);
                    w.WriteNumber("line", msg.line);
                    w.WriteEndObject();
                }
                w.WriteEndArray();
                w.WriteEndObject();
            }
            w.WriteEndArray();

            w.WriteStartArray("coverage");
            foreach (var std in StandardCoverageSituations)
            {
                var lines = items.Where(it => MatchesCoverageSlot(it.Situation, std, isWeapon: false)).Select(it => it.Line).ToList();
                WriteCoverageEntry(w, std, std, "standard", lines);
            }
            foreach (var wp in weapons)
            {
                var lines = items.Where(it => MatchesCoverageSlot(it.Situation, wp.name, isWeapon: true)).Select(it => it.Line).ToList();
                WriteCoverageEntry(w, wp.name, wp.name, "weapon", lines);
            }
            w.WriteEndArray();
            w.WriteEndObject();
        }

        private static void WriteCoverageEntry(Utf8JsonWriter w, string key, string label, string kind, List<int> lines)
        {
            w.WriteStartObject();
            w.WriteString("key", key);
            w.WriteString("label", label);
            w.WriteString("kind", kind);
            w.WriteNumber("count", lines.Count);
            w.WriteStartArray("lines");
            foreach (var l in lines)
            {
                w.WriteNumberValue(l);
            }
            w.WriteEndArray();
            w.WriteEndObject();
        }

        private static bool MatchesCoverageSlot(string situation, string slot, bool isWeapon)
        {
            if (string.IsNullOrEmpty(situation))
            {
                return false;
            }
            if (isWeapon)
            {
                var b = SrcRules.SituationBase(situation);
                return b == slot || StripWeaponBrackets(b) == StripWeaponBrackets(slot);
            }
            if (slot == "攻撃(反撃)")
            {
                return situation == "攻撃(反撃)"
                    || (SrcRules.SituationBase(situation) == "攻撃" && situation.Contains("(反撃)", StringComparison.Ordinal));
            }
            if (slot == "攻撃")
            {
                return situation == "攻撃"
                    || (SrcRules.SituationBase(situation) == "攻撃" && !situation.Contains("(反撃)", StringComparison.Ordinal));
            }
            return SrcRules.SituationBase(situation) == slot;
        }

        private static string StripWeaponBrackets(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "";
            }
            var idx = name.IndexOfAny(new[] { '(', '<', '＜' });
            return (idx < 0 ? name : name.Substring(0, idx)).Trim();
        }

        private static (PilotData pilot, string nickname, string cls, string bitmap) ResolveSpeaker(SRCCore.SRC src, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return (null, "", "", "");
            }
            var p = src.PDList.Items.FirstOrDefault(x => x.Name == name || x.Nickname == name)
                ?? src.PDList.Items.FirstOrDefault(x => x.Skills.Any(s => s.Name == "メッセージ" && s.StrData == name));
            if (p != null)
            {
                return (p, string.IsNullOrEmpty(p.Nickname) ? p.Name : p.Nickname, p.Class ?? "", p.Bitmap0 ?? "");
            }
            var np = src.NPDList.Item(name) ?? src.NPDList.Items.FirstOrDefault(x => x.Name == name || x.Nickname == name);
            if (np != null)
            {
                return (null, string.IsNullOrEmpty(np.Nickname) ? np.Name : np.Nickname, "ノンパイロット", np.Bitmap ?? "");
            }
            return (null, name, "", "");
        }

        private static (List<UnitData> units, List<(string name, string cls, string range)> weapons) ResolveUnitsAndWeapons(
            SRCCore.SRC src,
            string blockName,
            List<MessageItemModel> items,
            Dictionary<string, List<string>> pilotUnits)
        {
            var tokens = (blockName ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var matchedPilots = new List<PilotData>();
            foreach (var token in tokens)
            {
                matchedPilots.AddRange(src.PDList.Items.Where(p =>
                    p.Name == token || p.Nickname == token
                    || p.Skills.Any(s => s.Name == "メッセージ" && s.StrData == token)));
            }
            var pilotKeys = new HashSet<string>(tokens, StringComparer.Ordinal);
            foreach (var p in matchedPilots)
            {
                if (!string.IsNullOrEmpty(p.Name)) pilotKeys.Add(p.Name);
                if (!string.IsNullOrEmpty(p.Nickname)) pilotKeys.Add(p.Nickname);
            }

            var units = new List<UnitData>();
            var seenUnits = new HashSet<string>(StringComparer.Ordinal);
            void AddUnit(UnitData u)
            {
                if (u != null && seenUnits.Add(u.Name))
                {
                    units.Add(u);
                }
            }

            // 1. イベントの Create / Unit+Pilot で紐づいたユニット
            foreach (var pk in pilotKeys)
            {
                if (pilotUnits.TryGetValue(pk, out var uNames))
                {
                    foreach (var un in uNames)
                    {
                        AddUnit(src.UDList.Items.FirstOrDefault(u => u.Name == un || u.Nickname == un));
                    }
                }
            }
            // 2. ユニット名や「〜専用」がパイロット名と一致するユニット
            foreach (var u in src.UDList.Items.Where(x => x.Location != null))
            {
                if (pilotKeys.Contains(u.Name) || (!string.IsNullOrEmpty(u.Nickname) && pilotKeys.Contains(u.Nickname))
                    || pilotKeys.Any(k => !string.IsNullOrEmpty(u.Class) && u.Class.Contains($"({k}専用)", StringComparison.Ordinal)))
                {
                    AddUnit(u);
                }
            }
            // 3. まだ見つからず、メッセージ中の武器名がいずれかのユニットの武器と一意に一致する場合
            if (units.Count == 0 && items.Count > 0)
            {
                foreach (var it in items)
                {
                    var baseSit = SrcRules.SituationBase(it.Situation);
                    if (string.IsNullOrEmpty(baseSit) || SrcRules.StandardSituations.Contains(baseSit))
                    {
                        continue;
                    }
                    var owners = src.UDList.Items.Where(u => u.Location != null && u.Weapons.Any(wp => wp.Name == baseSit)).ToList();
                    if (owners.Count > 0 && owners.Count <= 2)
                    {
                        foreach (var o in owners)
                        {
                            AddUnit(o);
                        }
                    }
                }
            }

            var weapons = new List<(string name, string cls, string range)>();
            var seenWeapons = new HashSet<string>(StringComparer.Ordinal);
            void AddWeapon(string wName, string wCls, string wRange)
            {
                if (string.IsNullOrEmpty(wName)) return;
                if (seenWeapons.Add(wName))
                {
                    weapons.Add((wName, wCls ?? "", wRange ?? ""));
                }
            }

            foreach (var u in units)
            {
                foreach (var wp in u.Weapons)
                {
                    var r = wp.MinRange == wp.MaxRange ? wp.MaxRange.ToString(CultureInfo.InvariantCulture) : $"{wp.MinRange}-{wp.MaxRange}";
                    AddWeapon(wp.Name, wp.Class, r);
                }
            }
            foreach (var p in matchedPilots)
            {
                foreach (var wp in p.Weapons)
                {
                    var r = wp.MinRange == wp.MaxRange ? wp.MaxRange.ToString(CultureInfo.InvariantCulture) : $"{wp.MinRange}-{wp.MaxRange}";
                    AddWeapon(wp.Name, wp.Class, r);
                }
            }
            return (units, weapons);
        }

        private static Dictionary<string, List<string>> CollectPilotUnits(IReadOnlyList<(string name, string text)> before)
        {
            var map = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            void Add(string pilot, string unit)
            {
                if (string.IsNullOrEmpty(pilot) || string.IsNullOrEmpty(unit)) return;
                if (!map.TryGetValue(pilot, out var list))
                {
                    list = new List<string>();
                    map[pilot] = list;
                }
                if (!list.Contains(unit))
                {
                    list.Add(unit);
                }
            }
            foreach (var (name, text) in before)
            {
                if (string.IsNullOrEmpty(text) || !name.EndsWith(".eve", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                string currentUnit = null;
                foreach (var rawLine in text.Replace("\r\n", "\n").Split('\n'))
                {
                    var trimmed = rawLine.Trim();
                    if (trimmed == "" || trimmed.StartsWith("#", StringComparison.Ordinal) || trimmed.StartsWith("//", StringComparison.Ordinal))
                    {
                        continue;
                    }
                    if (!EventPlacementParser.TrySplitWithSpans(rawLine, out var tokens) || tokens.Count == 0)
                    {
                        continue;
                    }
                    var cmd = tokens[0].Value;
                    if (cmd.Equals("Create", StringComparison.OrdinalIgnoreCase) && tokens.Count >= 5)
                    {
                        var unit = EventPlacementParser.Unquote(tokens[2].Value);
                        var pilot = EventPlacementParser.Unquote(tokens[4].Value);
                        Add(pilot, unit);
                    }
                    else if (cmd.Equals("Unit", StringComparison.OrdinalIgnoreCase) && tokens.Count >= 2)
                    {
                        currentUnit = EventPlacementParser.Unquote(tokens[1].Value);
                    }
                    else if (cmd.Equals("Pilot", StringComparison.OrdinalIgnoreCase) && tokens.Count >= 2 && !string.IsNullOrEmpty(currentUnit))
                    {
                        Add(EventPlacementParser.Unquote(tokens[1].Value), currentUnit);
                    }
                }
            }
            return map;
        }
    }
}
