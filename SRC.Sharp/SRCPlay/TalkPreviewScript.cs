using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRCPlay
{
    public sealed class TalkPreviewOptionFlag
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public int Line { get; set; }
    }

    public sealed class TalkPreviewStep
    {
        public int Line { get; set; }
        public string Command { get; set; }
        public string Speaker { get; set; }
        public string ImageOptions { get; set; }
        public string Text { get; set; }
        public bool ClearFaceOnly { get; set; }
    }

    public sealed class TalkPreviewBlock
    {
        public int Index { get; set; }
        public string Command { get; set; }
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public string EndKind { get; set; }
        public string EnclosingLabel { get; set; }
        public List<TalkPreviewStep> Steps { get; } = new List<TalkPreviewStep>();
    }

    public sealed class TalkPreviewDocument
    {
        public List<TalkPreviewOptionFlag> Options { get; } = new List<TalkPreviewOptionFlag>();
        public List<TalkPreviewBlock> Blocks { get; } = new List<TalkPreviewBlock>();

        /// <summary>
        /// カーソル位置（1 始まり）にいちばん近いブロックと、その中の開始ステップ（0 始まり）を選ぶ。
        /// ブロックの途中にカーソルがあればその行以前で最も近い発話から始め、前に戻ることもできる。
        /// </summary>
        public (TalkPreviewBlock block, int startStepIndex) SelectAtLine(int cursorLine)
        {
            if (Blocks.Count == 0)
            {
                return (null, 0);
            }
            if (cursorLine <= 0)
            {
                return (Blocks[0], 0);
            }
            var enclosing = Blocks.FirstOrDefault(b => cursorLine >= b.StartLine && cursorLine <= b.EndLine);
            if (enclosing == null)
            {
                enclosing = Blocks.FirstOrDefault(b => b.StartLine >= cursorLine) ?? Blocks[Blocks.Count - 1];
                return (enclosing, 0);
            }
            var stepIdx = 0;
            for (var i = 0; i < enclosing.Steps.Count; i++)
            {
                if (enclosing.Steps[i].Line <= cursorLine)
                {
                    stepIdx = i;
                }
                else
                {
                    break;
                }
            }
            return (enclosing, stepIdx);
        }
    }

    /// <summary>
    /// イベントファイル（*.eve）の Talk / AutoTalk .. End / Suspend ブロックと Option コマンドを解析する（7474/SRC#1655）。
    /// </summary>
    public static class TalkPreviewScript
    {
        private static readonly HashSet<string> PilotImageOptions = new HashSet<string>(StringComparer.Ordinal)
        {
            "白黒", "セピア", "明", "暗", "上下反転", "左右反転",
            "上半分", "下半分", "右半分", "左半分",
            "右上", "左上", "右下", "左下",
            "ネガポジ反転", "シルエット", "夕焼け", "水中", "通常",
        };

        public static TalkPreviewDocument Parse(string text)
        {
            var doc = new TalkPreviewDocument();
            var logicalLines = ReadLogicalLines(text ?? "");
            string currentLabel = null;
            TalkPreviewBlock currentBlock = null;
            string currentSpeaker = "";
            string currentImageOptions = "";

            foreach (var (lineNum, raw) in logicalLines)
            {
                var trimmed = raw.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#", StringComparison.Ordinal) || trimmed.StartsWith("//", StringComparison.Ordinal))
                {
                    continue;
                }

                if (currentBlock == null)
                {
                    if (trimmed.Length > 1 && trimmed.EndsWith(":", StringComparison.Ordinal))
                    {
                        currentLabel = trimmed.Substring(0, trimmed.Length - 1);
                        continue;
                    }

                    var tokens = SplitArgs(trimmed);
                    if (tokens.Count == 0)
                    {
                        continue;
                    }
                    var head = tokens[0];
                    if (head.Equals("Option", StringComparison.OrdinalIgnoreCase) && tokens.Count >= 2)
                    {
                        doc.Options.Add(new TalkPreviewOptionFlag
                        {
                            Name = tokens[1],
                            Enabled = tokens.Count == 2,
                            Line = lineNum,
                        });
                        continue;
                    }
                    if (head.Equals("Talk", StringComparison.OrdinalIgnoreCase) || head.Equals("AutoTalk", StringComparison.OrdinalIgnoreCase))
                    {
                        var cmdName = head.Equals("AutoTalk", StringComparison.OrdinalIgnoreCase) ? "AutoTalk" : "Talk";
                        currentBlock = new TalkPreviewBlock
                        {
                            Index = doc.Blocks.Count,
                            Command = cmdName,
                            StartLine = lineNum,
                            EndLine = lineNum,
                            EndKind = "EOF",
                            EnclosingLabel = currentLabel,
                        };
                        (currentSpeaker, currentImageOptions) = ParseTalkHeader(tokens);
                    }
                    continue;
                }

                // Inside Talk / AutoTalk block
                currentBlock.EndLine = lineNum;
                var blockTokens = SplitArgs(trimmed);
                var first = blockTokens.Count > 0 ? blockTokens[0] : "";

                if (first.Equals("End", StringComparison.OrdinalIgnoreCase) && blockTokens.Count == 1)
                {
                    currentBlock.EndKind = "End";
                    doc.Blocks.Add(currentBlock);
                    currentBlock = null;
                    currentSpeaker = "";
                    currentImageOptions = "";
                    continue;
                }
                if (first.Equals("Suspend", StringComparison.OrdinalIgnoreCase) && blockTokens.Count == 1)
                {
                    currentBlock.EndKind = "Suspend";
                    doc.Blocks.Add(currentBlock);
                    currentBlock = null;
                    currentSpeaker = "";
                    currentImageOptions = "";
                    continue;
                }

                if (first.Equals("Talk", StringComparison.OrdinalIgnoreCase) || first.Equals("AutoTalk", StringComparison.OrdinalIgnoreCase))
                {
                    var (nextSpeaker, nextOptions) = ParseTalkHeader(blockTokens);
                    if (nextSpeaker == null)
                    {
                        // 引数なしの Talk / AutoTalk（ATalkCmd の lnum <= 1 と同じ: 直前の話者を確定して解除）
                        if (!string.IsNullOrEmpty(currentSpeaker))
                        {
                            currentBlock.Steps.Add(new TalkPreviewStep
                            {
                                Line = lineNum,
                                Command = currentBlock.Command,
                                Speaker = currentSpeaker,
                                ImageOptions = nextOptions,
                                Text = "",
                                ClearFaceOnly = true,
                            });
                        }
                        currentSpeaker = "";
                        currentImageOptions = nextOptions;
                    }
                    else
                    {
                        currentSpeaker = nextSpeaker;
                        currentImageOptions = nextOptions;
                    }
                    continue;
                }

                // 発話行（元の行テキストを使う）
                currentBlock.Steps.Add(new TalkPreviewStep
                {
                    Line = lineNum,
                    Command = currentBlock.Command,
                    Speaker = currentSpeaker,
                    ImageOptions = currentImageOptions,
                    Text = raw.TrimStart(),
                });
            }

            if (currentBlock != null)
            {
                doc.Blocks.Add(currentBlock);
            }

            return doc;
        }

        private static (string speaker, string options) ParseTalkHeader(List<string> tokens)
        {
            if (tokens.Count <= 1)
            {
                return (null, "");
            }

            var speakerArg = tokens[1];
            if (speakerArg.StartsWith("@", StringComparison.Ordinal))
            {
                speakerArg = speakerArg.Substring(1);
            }

            var options = new StringBuilder();
            var i = 1;
            var lnum = 1;
            while (i < tokens.Count)
            {
                var opt = tokens[i];
                switch (opt)
                {
                    case "非表示":
                    case "枠外":
                        break;

                    case "右回転":
                        if (i + 1 < tokens.Count)
                        {
                            i++;
                            options.Append("右回転 ").Append(tokens[i]).Append(' ');
                        }
                        break;

                    case "左回転":
                        if (i + 1 < tokens.Count)
                        {
                            i++;
                            options.Append("左回転 ").Append(tokens[i]).Append(' ');
                        }
                        break;

                    case "フィルタ":
                        if (i + 2 < tokens.Count)
                        {
                            options.Append("フィルタ ").Append(tokens[i + 1]).Append(' ').Append(tokens[i + 2]).Append(' ');
                            i += 2;
                        }
                        break;

                    case "":
                        break;

                    default:
                        if (PilotImageOptions.Contains(opt))
                        {
                            if (i > 1)
                            {
                                options.Append(opt).Append(' ');
                            }
                            else
                            {
                                lnum = i + 1;
                            }
                        }
                        else
                        {
                            lnum = i + 1;
                        }
                        break;
                }
                i++;
            }

            if (lnum <= 1)
            {
                return (null, options.ToString());
            }
            return (speakerArg, options.ToString());
        }

        private static List<(int lineNum, string text)> ReadLogicalLines(string text)
        {
            var result = new List<(int, string)>();
            var rawLines = text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');
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
                    var withoutUnderscore = trimmedEnd.Substring(0, trimmedEnd.Length - 1);
                    if (buffer.Length > 0)
                    {
                        buffer.Append(line.TrimStart());
                    }
                    else
                    {
                        buffer.Append(withoutUnderscore);
                    }
                    continue;
                }

                if (buffer.Length > 0)
                {
                    buffer.Append(line.TrimStart());
                    result.Add((startLine, buffer.ToString()));
                    buffer.Clear();
                    startLine = 0;
                }
                else
                {
                    result.Add((lineNum, line));
                    startLine = 0;
                }
            }

            if (buffer.Length > 0)
            {
                result.Add((startLine > 0 ? startLine : rawLines.Length, buffer.ToString()));
            }
            return result;
        }

        private static List<string> SplitArgs(string line)
        {
            var tokens = new List<string>();
            var sb = new StringBuilder();
            var inQuotes = false;

            foreach (var ch in line)
            {
                if (ch == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }
                if (!inQuotes && (ch == ' ' || ch == '\t' || ch == '\u3000'))
                {
                    if (sb.Length > 0)
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                    }
                    continue;
                }
                sb.Append(ch);
            }
            if (sb.Length > 0)
            {
                tokens.Add(sb.ToString());
            }
            return tokens;
        }
    }
}
