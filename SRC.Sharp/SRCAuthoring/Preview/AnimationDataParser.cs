using SRCCore.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SRCAuthoring.Preview
{
    public static class AnimationPreviewKind
    {
        public const string Animation = "animation";
        public const string ExtAnimation = "ext_animation";
        public const string Effect = "effect";
        public const string Subroutine = "subroutine";
    }

    public sealed class AnimationPreviewEntry
    {
        public string Kind { get; set; }
        public string File { get; set; }
        public int Line { get; set; }
        public string Target { get; set; }
        public string Situation { get; set; }
        public string Spec { get; set; }
    }

    /// <summary>
    /// animation.txt・ext_animation.txt・effect.txt の定義、および汎用戦闘アニメ（戦闘アニメ_*）の
    /// 定義・呼び出しを解析する（7474/SRC#1655）。
    /// </summary>
    public static class AnimationDataParser
    {
        private static readonly Regex CallSpaceRegex = new Regex(
            @"^\s*Call\s+(戦闘アニメ_[^\s(,;]+)(?:\s+(.*))?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex CallParenRegex = new Regex(
            @"^\s*Call\s*\(\s*[`""]?(戦闘アニメ_[^`""\s,)]+)[`""]?\s*(?:,\s*(.*))?\)\s*$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex AtSubroutineRegex = new Regex(
            @"^\s*@(戦闘アニメ_[^\s(;]+)(?:\s+(.*))?$",
            RegexOptions.Compiled);

        public static string InferKind(string filePath)
        {
            var name = Path.GetFileName(filePath ?? "").ToLowerInvariant();
            if (name == "ext_animation.txt" || name.EndsWith("ext_animation.txt", StringComparison.Ordinal))
            {
                return AnimationPreviewKind.ExtAnimation;
            }
            if (name == "animation.txt" || name.EndsWith("animation.txt", StringComparison.Ordinal))
            {
                return AnimationPreviewKind.Animation;
            }
            if (name == "effect.txt" || name.EndsWith("effect.txt", StringComparison.Ordinal))
            {
                return AnimationPreviewKind.Effect;
            }
            return AnimationPreviewKind.Subroutine;
        }

        public static List<AnimationPreviewEntry> ParseFile(string filePath, string content = null, string overrideKind = null)
        {
            var fullPath = string.IsNullOrEmpty(filePath) ? "" : Path.GetFullPath(filePath);
            var text = content ?? (File.Exists(fullPath) ? ReadText(fullPath) : "");
            return ParseText(fullPath, text, overrideKind);
        }

        public static List<AnimationPreviewEntry> ParseText(string filePath, string content, string overrideKind = null)
        {
            var kind = string.IsNullOrEmpty(overrideKind) ? InferKind(filePath) : overrideKind;
            if (kind == AnimationPreviewKind.Subroutine)
            {
                return ParseSubroutines(filePath, content);
            }
            return ParseEffectOrAnimationData(filePath, content, kind);
        }

        public static AnimationPreviewEntry SelectAtLine(IReadOnlyList<AnimationPreviewEntry> entries, int cursorLine)
        {
            if (entries == null || entries.Count == 0)
            {
                return null;
            }
            if (cursorLine <= 0)
            {
                return entries[0];
            }
            var exact = entries.FirstOrDefault(e => e.Line == cursorLine);
            if (exact != null)
            {
                return exact;
            }
            // 直後にある同じブロック（ターゲット見出し行の直下のシチュエーションなど）があればそちらを優先
            var after = entries.FirstOrDefault(e => e.Line > cursorLine);
            var before = entries.LastOrDefault(e => e.Line < cursorLine);
            if (before != null && after != null)
            {
                // すぐ次がターゲット内（たとえば見出し行から数行以内）で前に無いか、離れている場合は近い方
                return (cursorLine - before.Line) <= (after.Line - cursorLine) ? before : after;
            }
            return before ?? after ?? entries[0];
        }

        private static List<AnimationPreviewEntry> ParseEffectOrAnimationData(string filePath, string content, string kind)
        {
            var entries = new List<AnimationPreviewEntry>();
            var bytes = new UTF8Encoding(false).GetBytes(content ?? "");
            using var ms = new MemoryStream(bytes);
            using var reader = new SrcDataReader(filePath ?? "", ms);

            string currentTarget = null;
            while (reader.HasMore)
            {
                var line = reader.GetLine();
                var lineNum = reader.LineNumber;
                if (string.IsNullOrEmpty(line))
                {
                    currentTarget = null;
                    continue;
                }

                if (currentTarget == null)
                {
                    if (!line.Contains(','))
                    {
                        currentTarget = line.Trim();
                    }
                    continue;
                }

                var comma = line.IndexOf(',');
                if (comma <= 0)
                {
                    // 区切りが無い行は、空行無しの見出しとして扱うか無視
                    continue;
                }
                var situation = line.Substring(0, comma).Trim();
                var spec = line.Substring(comma + 1).Trim('\t', ' ', '\u3000');
                if (string.IsNullOrEmpty(situation))
                {
                    continue;
                }

                entries.Add(new AnimationPreviewEntry
                {
                    Kind = kind,
                    File = filePath ?? "",
                    Line = lineNum,
                    Target = currentTarget,
                    Situation = situation,
                    Spec = spec,
                });
            }
            return entries;
        }

        private static List<AnimationPreviewEntry> ParseSubroutines(string filePath, string content)
        {
            var entries = new List<AnimationPreviewEntry>();
            var rawLines = (content ?? "").Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
            var buffer = new StringBuilder();
            var startLine = 0;

            for (var i = 0; i < rawLines.Length; i++)
            {
                var line = rawLines[i];
                var lineNum = i + 1;
                if (startLine == 0)
                {
                    startLine = lineNum;
                }
                var trimmedEnd = line.TrimEnd();
                if (trimmedEnd.EndsWith(" _", StringComparison.Ordinal) || trimmedEnd.EndsWith("\t_", StringComparison.Ordinal) || trimmedEnd == "_")
                {
                    buffer.Append(buffer.Length > 0 ? line.TrimStart() : trimmedEnd.Substring(0, trimmedEnd.Length - 1));
                    continue;
                }

                var logical = buffer.Length > 0 ? buffer.ToString() + line.TrimStart() : line;
                var logicalLine = buffer.Length > 0 ? startLine : lineNum;
                buffer.Clear();
                startLine = 0;

                var trimmed = logical.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#", StringComparison.Ordinal) || trimmed.StartsWith("//", StringComparison.Ordinal))
                {
                    continue;
                }

                if (trimmed.StartsWith("戦闘アニメ_", StringComparison.Ordinal) && trimmed.EndsWith(":", StringComparison.Ordinal))
                {
                    var label = trimmed.Substring(0, trimmed.Length - 1);
                    entries.Add(new AnimationPreviewEntry
                    {
                        Kind = AnimationPreviewKind.Subroutine,
                        File = filePath ?? "",
                        Line = logicalLine,
                        Target = "汎用戦闘アニメ",
                        Situation = label,
                        Spec = label,
                    });
                    continue;
                }

                if (TryExtractSubroutineCall(trimmed, out var sname, out var argsText))
                {
                    entries.Add(new AnimationPreviewEntry
                    {
                        Kind = AnimationPreviewKind.Subroutine,
                        File = filePath ?? "",
                        Line = logicalLine,
                        Target = "汎用戦闘アニメ",
                        Situation = sname,
                        Spec = string.IsNullOrEmpty(argsText) ? sname : $"{sname} {argsText}",
                    });
                }
            }

            if (buffer.Length > 0)
            {
                var trimmed = buffer.ToString().Trim();
                if (TryExtractSubroutineCall(trimmed, out var sname, out var argsText))
                {
                    entries.Add(new AnimationPreviewEntry
                    {
                        Kind = AnimationPreviewKind.Subroutine,
                        File = filePath ?? "",
                        Line = startLine > 0 ? startLine : rawLines.Length,
                        Target = "汎用戦闘アニメ",
                        Situation = sname,
                        Spec = string.IsNullOrEmpty(argsText) ? sname : $"{sname} {argsText}",
                    });
                }
            }

            return entries;
        }

        private static bool TryExtractSubroutineCall(string trimmed, out string name, out string argsText)
        {
            var m = CallSpaceRegex.Match(trimmed);
            if (m.Success)
            {
                name = m.Groups[1].Value;
                argsText = m.Groups[2].Value.Trim();
                return true;
            }
            m = CallParenRegex.Match(trimmed);
            if (m.Success)
            {
                name = m.Groups[1].Value;
                var rawArgs = m.Groups[2].Value.Trim();
                if (string.IsNullOrEmpty(rawArgs))
                {
                    argsText = "";
                }
                else
                {
                    // Call("戦闘アニメ_...", a, b) のカンマ区切りを ShowAnimation が期待するスペース区切りに直す
                    var parts = rawArgs.Split(',')
                        .Select(p => p.Trim().Trim('"', '`'))
                        .Where(p => p.Length > 0);
                    argsText = string.Join(" ", parts);
                }
                return true;
            }
            m = AtSubroutineRegex.Match(trimmed);
            if (m.Success)
            {
                name = m.Groups[1].Value;
                argsText = m.Groups[2].Value.Trim();
                return true;
            }
            name = null;
            argsText = null;
            return false;
        }

        private static string ReadText(string path)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var bytes = File.ReadAllBytes(path);
            try
            {
                return new UTF8Encoding(false, true).GetString(bytes);
            }
            catch (DecoderFallbackException)
            {
                return Encoding.GetEncoding(932).GetString(bytes);
            }
        }
    }
}
