using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCAuthoring.Analysis
{
    /// <summary>名前を変える 1 か所（0 始まりの行と文字の位置）</summary>
    public readonly struct RenameSpan
    {
        public RenameSpan(int line, int start, int length)
        {
            Line = line;
            Start = start;
            Length = length;
        }

        public int Line { get; }
        public int Start { get; }
        public int Length { get; }
    }

    /// <summary>
    /// 名前を変える（7474/SRC#1692。LSP の rename）。ユニット・パイロット・変数・ラベルの名前を、シナリオのイベントとデータの全体で一度に変える。
    /// 変えるのは語として書いた所だけ: イベントは命令・ラベルの引数と式（会話の文は $(名前) の中だけ）、データは名前の行とカンマで区切った欄。
    /// 前後が区切り（空白・引用符・括弧・カンマ・演算子・コロン）でない所は変えない（「リオ」を変えても「リオン」は変わらない）
    /// </summary>
    public static class ScenarioRename
    {
        private static readonly string[] DataFiles =
        {
            "robot.txt", "unit.txt", "pilot.txt", "non_pilot.txt", "pilot_message.txt", "pilot_dialog.txt", "item.txt", "alias.txt",
        };

        private const string Boundaries = " \t　\"'`(),=<>+-*/:[]{}&|;";

        /// <summary>シナリオのフォルダの、名前を変える対象のファイル（イベントと Data の下のデータ。.srcsharp は除く）</summary>
        public static IEnumerable<string> TargetFiles(string scenarioRoot)
        {
            if (string.IsNullOrEmpty(scenarioRoot) || !Directory.Exists(scenarioRoot))
            {
                yield break;
            }
            foreach (var file in Directory.EnumerateFiles(scenarioRoot, "*", SearchOption.AllDirectories))
            {
                var rel = Path.GetRelativePath(scenarioRoot, file).Replace('\\', '/');
                if (rel.StartsWith(".srcsharp/", StringComparison.Ordinal) || rel.StartsWith(".git/", StringComparison.Ordinal))
                {
                    continue;
                }
                var name = Path.GetFileName(file);
                if (name.EndsWith(".eve", StringComparison.OrdinalIgnoreCase)
                    || DataFiles.Any(d => string.Equals(d, name, StringComparison.OrdinalIgnoreCase)))
                {
                    yield return file;
                }
            }
        }

        /// <summary>1 つのファイルの中で、oldName を語として書いた所</summary>
        public static List<RenameSpan> Find(string fileName, string text, string oldName)
        {
            var result = new List<RenameSpan>();
            if (string.IsNullOrEmpty(oldName))
            {
                return result;
            }
            var isEvent = fileName.EndsWith(".eve", StringComparison.OrdinalIgnoreCase);
            var lines = (text ?? "").Replace("\r\n", "\n").Split('\n');
            var inTalk = false;
            for (var n = 0; n < lines.Length; n++)
            {
                var line = lines[n];
                var trimmed = line.TrimStart();
                if (trimmed.StartsWith("#", StringComparison.Ordinal) || trimmed.StartsWith("//", StringComparison.Ordinal))
                {
                    continue;
                }
                if (isEvent && inTalk)
                {
                    var head = FirstToken(trimmed);
                    if (Is(head, "End") || Is(head, "Suspend"))
                    {
                        inTalk = false;
                        continue;
                    }
                    if (!Is(head, "Talk") && !Is(head, "AutoTalk"))
                    {
                        // 会話の文は $(…) の中だけ
                        var i = 0;
                        while ((i = line.IndexOf("$(", i, StringComparison.Ordinal)) >= 0)
                        {
                            var close = line.IndexOf(')', i + 2);
                            if (close < 0)
                            {
                                break;
                            }
                            Scan(line, n, oldName, i + 2, close, result);
                            i = close + 1;
                        }
                        continue;
                    }
                }
                if (isEvent && (Is(FirstToken(trimmed), "Talk") || Is(FirstToken(trimmed), "AutoTalk")))
                {
                    inTalk = true;
                }
                Scan(line, n, oldName, 0, line.Length, result);
            }
            return result;
        }

        private static void Scan(string line, int lineNumber, string oldName, int from, int to, List<RenameSpan> result)
        {
            var i = from;
            while ((i = line.IndexOf(oldName, i, StringComparison.Ordinal)) >= 0 && i + oldName.Length <= to)
            {
                var before = i == from ? (from == 0 ? ' ' : line[i - 1]) : line[i - 1];
                var afterIndex = i + oldName.Length;
                var after = afterIndex >= to ? ' ' : line[afterIndex];
                if ((i == from || Boundaries.IndexOf(before) >= 0) && (afterIndex >= to || Boundaries.IndexOf(after) >= 0))
                {
                    result.Add(new RenameSpan(lineNumber, i, oldName.Length));
                }
                i += oldName.Length;
            }
        }

        /// <summary>シナリオの全体（読み込みは readText。開いている文書は保存前の内容を渡す）で、名前を変える所</summary>
        public static Dictionary<string, List<RenameSpan>> FindAll(string scenarioRoot, string oldName, Func<string, string> readText)
        {
            var result = new Dictionary<string, List<RenameSpan>>(StringComparer.OrdinalIgnoreCase);
            foreach (var file in TargetFiles(scenarioRoot))
            {
                var spans = Find(file, readText(file), oldName);
                if (spans.Count > 0)
                {
                    result[file] = spans;
                }
            }
            return result;
        }

        /// <summary>変えられる名前か（空・空白や引用符やカンマを含む名前・数は変えない。括弧を含む名前（敵兵(ザコ)）は変えられる）</summary>
        public static bool IsRenamable(string name) =>
            !string.IsNullOrWhiteSpace(name) && name.IndexOfAny(new[] { ' ', '\t', '　', '"', '\'', '`', ',' }) < 0 && !double.TryParse(name, out _);

        /// <summary>行の character の位置にある名前（空白・引用符・カンマ・行末のコロンで区切る。括弧は名前に含める）と、その始まり</summary>
        public static (string name, int start) NameAt(string line, int character)
        {
            if (string.IsNullOrEmpty(line))
            {
                return ("", 0);
            }
            static bool Sep(char c) => c == ' ' || c == '\t' || c == '　' || c == '"' || c == '\'' || c == '`' || c == ',' || c == '=' || c == '<' || c == '>';
            character = Math.Max(0, Math.Min(character, line.Length));
            var start = character;
            while (start > 0 && !Sep(line[start - 1]))
            {
                start--;
            }
            var end = character;
            while (end < line.Length && !Sep(line[end]))
            {
                end++;
            }
            var name = line.Substring(start, end - start);
            if (name.EndsWith(":", StringComparison.Ordinal))
            {
                name = name.Substring(0, name.Length - 1);
            }
            // $(名前) の中なら名前だけ
            if (name.StartsWith("$(", StringComparison.Ordinal) && name.EndsWith(")", StringComparison.Ordinal))
            {
                name = name.Substring(2, name.Length - 3);
                start += 2;
            }
            return (name, start);
        }

        private static string FirstToken(string trimmed)
        {
            var i = trimmed.IndexOfAny(new[] { ' ', '\t', '　' });
            return i < 0 ? trimmed : trimmed.Substring(0, i);
        }

        private static bool Is(string token, string command) => string.Equals(token, command, StringComparison.OrdinalIgnoreCase);
    }
}
