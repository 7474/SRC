using SRCAuthoring.Maps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace SRCAuthoring.Templates
{
    /// <summary>フォームで直せる出来事（7474/SRC#1692）。イベントファイルの中の位置と、フォームの項目と、フォームに出ない行</summary>
    public sealed class EventBlockMatch
    {
        /// <summary>出来事の始まり（ラベルの上の注釈から。1 始まり）と終わり（Exit の行）</summary>
        public int Start { get; set; }
        public int End { get; set; }
        /// <summary>フォームの項目（出来事を足すと同じ JSON: trigger・turn・phase・unit・target・ratio・talk・retreat）</summary>
        public string Json { get; set; }
        /// <summary>フォームに出ない行（援軍・加入など）。会話の前のものと後のもの。直したあとも同じ所に残す</summary>
        public List<string> KeepBefore { get; } = new List<string>();
        public List<string> KeepAfter { get; } = new List<string>();
        /// <summary>直せないときの理由（null なら直せる）</summary>
        public string Error { get; set; }

        public string ToJson()
        {
            using var stream = new MemoryStream();
            using (var w = new Utf8JsonWriter(stream, new JsonWriterOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }))
            {
                w.WriteStartObject();
                w.WriteNumber("start", Start);
                w.WriteNumber("end", End);
                if (Error != null)
                {
                    w.WriteString("error", Error);
                }
                else
                {
                    w.WritePropertyName("event");
                    w.WriteRawValue(Json);
                    w.WriteNumber("kept", KeepBefore.Count + KeepAfter.Count);
                }
                w.WriteEndObject();
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }

    public static partial class QuickBattleTemplate
    {
        private static readonly string[] FlowCommands =
        {
            "if", "elseif", "else", "endif", "switch", "case", "endsw", "do", "loop", "for", "next", "foreach", "goto", "call", "return", "continue", "break",
        };

        /// <summary>
        /// 出来事を直す（7474/SRC#1692）。イベントファイルの line 行目（1 始まり）を含む出来事を、出来事を足すフォームの項目に読み戻す。
        /// 読めるのは、かんたん作成・出来事を足すと同じ形の出来事（ラベルがターン・攻撃・損傷率・破壊、会話は 1 か所、分岐が無い）。
        /// 援軍などフォームに出ない行は、そのまま残す。見つからない・直せないときは Error に理由
        /// </summary>
        public static EventBlockMatch FindEvent(string text, int line)
        {
            var lines = (text ?? "").Replace("\r\n", "\n").Split('\n');
            var m = new EventBlockMatch();
            var i = Math.Max(0, Math.Min(lines.Length - 1, line - 1));
            // 注釈の上なら下のラベルへ、そうでなければ上のラベルへ
            var label = -1;
            if (lines[i].TrimStart().StartsWith("#", StringComparison.Ordinal))
            {
                for (var j = i; j < lines.Length && lines[j].TrimStart().StartsWith("#", StringComparison.Ordinal); j++)
                {
                    if (j + 1 < lines.Length && IsLabelLine(lines[j + 1]))
                    {
                        label = j + 1;
                    }
                }
            }
            if (label < 0)
            {
                for (var j = i; j >= 0; j--)
                {
                    if (IsLabelLine(lines[j]))
                    {
                        label = j;
                        break;
                    }
                    if (j < i && string.Equals(lines[j].Trim(), "Exit", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
            if (label < 0)
            {
                m.Error = "カーソルのある所に出来事（ラベル）がありません";
                return m;
            }
            var start = label;
            while (start > 0 && lines[start - 1].TrimStart().StartsWith("#", StringComparison.Ordinal))
            {
                start--;
            }
            m.Start = start + 1;

            // ラベル: ターン N 味方|敵・攻撃 A B・損傷率 X R・破壊 X
            var head = lines[label].Trim();
            head = head.Substring(0, head.Length - 1);
            if (!EventPlacementParser.TrySplitWithSpans(head, out var tokens) || tokens.Count == 0)
            {
                m.Error = $"ラベル「{head}」を読めません";
                return m;
            }
            var t = tokens.Select(x => EventPlacementParser.Unquote(x.Value)).ToList();
            var ev = new QuickEvent();
            if (t[0] == "ターン" && t.Count == 3 && int.TryParse(t[1], out var turn) && (t[2] == "味方" || t[2] == "敵"))
            {
                ev.Trigger = "turn";
                ev.Turn = turn;
                ev.Phase = t[2];
            }
            else if (t[0] == "攻撃" && t.Count == 3)
            {
                ev.Trigger = "attack";
                ev.Unit = t[1];
                ev.Target = t[2];
            }
            else if (t[0] == "損傷率" && t.Count == 3 && int.TryParse(t[2], out var ratio))
            {
                ev.Trigger = "damaged";
                ev.Unit = t[1];
                ev.Ratio = ratio;
            }
            else if (t[0] == "破壊" && t.Count == 2)
            {
                ev.Trigger = "destroyed";
                ev.Unit = t[1];
            }
            else
            {
                m.Error = $"「{head}」はフォームで直せるきっかけ（ターン・攻撃・損傷率・破壊）ではありません";
                return m;
            }

            // 中身: 会話（1 か所）・撤退・そのほかの行
            var talkSeen = false;
            var exit = -1;
            for (var j = label + 1; j < lines.Length; j++)
            {
                var raw = lines[j].TrimEnd();
                var trimmed = raw.Trim();
                var first = trimmed.Split(' ', '\t', '　')[0].ToLowerInvariant();
                if (trimmed.Length == 0)
                {
                    continue;
                }
                if (first == "exit")
                {
                    exit = j;
                    break;
                }
                if (IsLabelLine(raw))
                {
                    break;
                }
                if (first == "talk")
                {
                    if (talkSeen)
                    {
                        m.Error = "会話が 2 か所にあります。フォームで直せないので、手で直してください";
                        return m;
                    }
                    talkSeen = true;
                    var end = ReadTalk(lines, j, ev.Talk);
                    if (end < 0)
                    {
                        m.Error = "会話（Talk）の書き方がフォームで直せる形ではありません（位置や表情の指定・End が無いなど）。手で直してください";
                        return m;
                    }
                    j = end;
                    continue;
                }
                if (FlowCommands.Contains(first) || first == "autotalk" || first == "suspend")
                {
                    m.Error = $"「{trimmed}」のような分岐・呼び出しはフォームで直せません。手で直してください";
                    return m;
                }
                if (first == "escape" && !string.IsNullOrEmpty(ev.Unit) && NextIsExit(lines, j)
                    && EventPlacementParser.TrySplitWithSpans(trimmed, out var esc) && esc.Count == 2 && EventPlacementParser.Unquote(esc[1].Value) == ev.Unit)
                {
                    ev.Retreat = true;
                    continue;
                }
                (talkSeen ? m.KeepAfter : m.KeepBefore).Add(raw);
            }
            if (exit < 0)
            {
                m.Error = "出来事の終わり（Exit）がありません";
                return m;
            }
            m.End = exit + 1;
            m.Json = EventJson(ev);
            return m;
        }

        /// <summary>直した出来事の書き方（フォームの項目から作り、フォームに出ない行を元の所に戻す）</summary>
        public static string RewriteEvent(string json, EventBlockMatch kept, IEnumerable<string> named = null)
        {
            var lines = EventText(json, named).TrimEnd('\r', '\n').Split("\r\n").ToList();
            var labelIndex = lines.FindIndex(IsLabelLine);
            // 残す行に援軍（Create・Launch・Unit）があれば、注釈の「すること」にも援軍と書く（かんたん作成と同じ順: 会話・援軍・退く）
            var keptLines = (kept?.KeepBefore ?? new List<string>()).Concat(kept?.KeepAfter ?? new List<string>());
            if (labelIndex > 0 && keptLines.Any(l => new[] { "create", "launch", "unit" }.Contains(l.TrimStart().Split(' ', '\t')[0].ToLowerInvariant())))
            {
                var note = lines[labelIndex - 1];
                var colon = note.LastIndexOf(": ", StringComparison.Ordinal);
                if (note.StartsWith("# ", StringComparison.Ordinal) && colon > 0)
                {
                    var what = note.Substring(colon + 2).Split('・').ToList();
                    if (!what.Contains("援軍"))
                    {
                        what.Insert(what.Contains("会話") ? what.IndexOf("会話") + 1 : 0, "援軍");
                        lines[labelIndex - 1] = note.Substring(0, colon + 2) + string.Join("・", what);
                    }
                }
            }
            var talkEnd = lines.FindIndex(l => l == "End");
            lines.InsertRange(labelIndex + 1, kept?.KeepBefore ?? new List<string>());
            var after = talkEnd >= 0 ? talkEnd + 1 + (kept?.KeepBefore.Count ?? 0) : labelIndex + 1 + (kept?.KeepBefore.Count ?? 0);
            lines.InsertRange(after, kept?.KeepAfter ?? new List<string>());
            return string.Join("\r\n", lines) + "\r\n";
        }

        private static bool IsLabelLine(string raw)
        {
            var t = (raw ?? "").Trim();
            return t.Length > 1 && t.EndsWith(":", StringComparison.Ordinal) && !t.StartsWith("#", StringComparison.Ordinal) && !t.StartsWith("//", StringComparison.Ordinal)
                && raw.Length > 0 && !char.IsWhiteSpace(raw[0]) && raw[0] != '　';
        }

        private static bool NextIsExit(string[] lines, int j)
        {
            for (var k = j + 1; k < lines.Length; k++)
            {
                var t = lines[k].Trim();
                if (t.Length > 0)
                {
                    return string.Equals(t, "Exit", StringComparison.OrdinalIgnoreCase);
                }
            }
            return false;
        }

        /// <summary>Talk 話す人 … End を読む（話す人が変わるところで Talk を書き直す形）。End の行を返す。読めなければ -1</summary>
        private static int ReadTalk(string[] lines, int j, List<QuickLine> talk)
        {
            string speaker = null;
            for (var k = j; k < lines.Length; k++)
            {
                var raw = lines[k].TrimEnd();
                var trimmed = raw.Trim();
                // 全角空白で始まる行はセリフ（命令に見える行を、かんたん作成は全角空白を付けて書く）
                var first = raw.TrimStart(' ', '\t').Split(' ', '\t')[0].ToLowerInvariant();
                if (first == "end")
                {
                    return speaker == null ? -1 : k;
                }
                if (first == "talk")
                {
                    if (!EventPlacementParser.TrySplitWithSpans(trimmed, out var tk) || tk.Count > 2)
                    {
                        return -1;
                    }
                    speaker = tk.Count == 2 ? EventPlacementParser.Unquote(tk[1].Value) : "";
                    continue;
                }
                if (first == "exit" || first == "suspend" || first == "autotalk" || IsLabelLine(raw) || speaker == null)
                {
                    return -1;
                }
                if (trimmed.Length == 0)
                {
                    continue;
                }
                // かんたん作成が全角空白を付けて書いたセリフ（命令に見える行）は、空白を外して戻す
                var text = raw.TrimStart(' ', '\t');
                if (text.StartsWith("　", StringComparison.Ordinal))
                {
                    var rest = text.Substring(1);
                    var restFirst = rest.TrimStart().Split(' ', '\t')[0].ToLowerInvariant();
                    if (Keywords.Contains(restFirst) || rest.TrimStart().StartsWith("#", StringComparison.Ordinal) || rest.TrimStart().StartsWith("'", StringComparison.Ordinal))
                    {
                        text = rest;
                    }
                }
                talk.Add(new QuickLine { Speaker = speaker, Text = text });
            }
            return -1;
        }

        private static string EventJson(QuickEvent e)
        {
            using var stream = new MemoryStream();
            using (var w = new Utf8JsonWriter(stream, new JsonWriterOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }))
            {
                w.WriteStartObject();
                w.WriteString("trigger", e.Trigger);
                switch (e.Trigger)
                {
                    case "turn":
                        w.WriteNumber("turn", e.Turn);
                        w.WriteString("phase", e.Phase);
                        break;
                    case "attack":
                        w.WriteString("unit", e.Unit);
                        w.WriteString("target", e.Target);
                        break;
                    case "damaged":
                        w.WriteString("unit", e.Unit);
                        w.WriteNumber("ratio", e.Ratio);
                        break;
                    default:
                        w.WriteString("unit", e.Unit);
                        break;
                }
                w.WriteStartArray("talk");
                foreach (var l in e.Talk)
                {
                    w.WriteStartObject();
                    w.WriteString("speaker", l.Speaker ?? "");
                    w.WriteString("text", l.Text ?? "");
                    w.WriteEndObject();
                }
                w.WriteEndArray();
                w.WriteBoolean("retreat", e.Retreat);
                w.WriteEndObject();
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}
