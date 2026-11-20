using SRCAuthoring.Maps;
using SRCCore.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SRCAuthoring.Analysis
{
    /// <summary>出来事（イベントラベルの塊）。7474/SRC#1692</summary>
    public sealed class OutlineEvent
    {
        public string File { get; set; }
        public int Line { get; set; }
        public string Label { get; set; }
        /// <summary>start・turn・destroyed・damaged・annihilated・attack・talk・enter・other（きっかけの種類）・sub（Call の先）・entry（プロローグ・エピローグなど）</summary>
        public string Kind { get; set; }
        /// <summary>いつ起きるか（日本語）</summary>
        public string When { get; set; }
        /// <summary>常時イベント（* 付き。1 回で消えない）</summary>
        public bool Always { get; set; }
        /// <summary>中の条件（If・ElseIf・Case の式）</summary>
        public List<string> Conditions { get; } = new List<string>();
        /// <summary>すること（日本語の要約。会話・援軍・加入・撤退・勝ち負け・次の話など）</summary>
        public List<string> Actions { get; } = new List<string>();
        public int TalkLines { get; set; }
    }

    /// <summary>台本の 1 行（会話・テロップ）</summary>
    public sealed class OutlineLine
    {
        public string Episode { get; set; }
        public string Event { get; set; }
        public string File { get; set; }
        public int Line { get; set; }
        /// <summary>talk・autotalk・telop</summary>
        public string Kind { get; set; }
        public string Speaker { get; set; }
        public string Text { get; set; }
    }

    /// <summary>変数（Set・Incr・代入で設定し、条件・式・メッセージの $() で読む）</summary>
    public sealed class OutlineVariable
    {
        public string Name { get; set; }
        public bool Global { get; set; }
        public List<(string file, int line)> Sets { get; } = new List<(string, int)>();
        public List<(string file, int line)> Reads { get; } = new List<(string, int)>();
    }

    /// <summary>話ごとの部隊と登場人物</summary>
    public sealed class OutlineEpisode
    {
        public string File { get; set; }
        public string Title { get; set; }
        public List<OutlineEvent> Events { get; } = new List<OutlineEvent>();
        /// <summary>Create 味方 で作る（加入）パイロット</summary>
        public List<string> Joins { get; } = new List<string>();
        /// <summary>Launch で出す（続けて出る）パイロット</summary>
        public List<string> Launches { get; } = new List<string>();
        /// <summary>Leave で離れるパイロット</summary>
        public List<string> Leaves { get; } = new List<string>();
        /// <summary>Create 敵・中立 で出す相手</summary>
        public List<string> Enemies { get; } = new List<string>();
        /// <summary>話す人 → 行数</summary>
        public Dictionary<string, int> Speakers { get; } = new Dictionary<string, int>(StringComparer.Ordinal);
        /// <summary>次の話（Continue の先）</summary>
        public List<string> Next { get; } = new List<string>();
        /// <summary>この話で作る・出すユニットの名前（破壊・攻撃などのきっかけの相手を確かめる）</summary>
        public HashSet<string> UnitNames { get; } = new HashSet<string>(StringComparer.Ordinal);
    }

    /// <summary>見通しから気づいたこと（7474/SRC#1692 の検査）</summary>
    public sealed class OutlineFinding
    {
        public string File { get; set; }
        public int Line { get; set; }
        public string Message { get; set; }
    }

    /// <summary>シナリオの見通し（7474/SRC#1692）: 出来事の一覧・台本・変数・部隊と登場人物</summary>
    public sealed class ScenarioOutline
    {
        public string ScenarioRoot { get; set; }
        public List<OutlineEpisode> Episodes { get; } = new List<OutlineEpisode>();
        public List<OutlineLine> Script { get; } = new List<OutlineLine>();
        public List<OutlineVariable> Variables { get; } = new List<OutlineVariable>();
        public List<OutlineFinding> Findings { get; } = new List<OutlineFinding>();

        private static readonly string[] Triggers =
        {
            "ターン", "損傷率", "破壊", "マップ攻撃破壊", "全滅", "攻撃", "攻撃後", "会話", "接触", "進入", "脱出",
            "収納", "使用", "使用後", "変形", "合体", "分離", "行動終了", "レベルアップ",
        };

        private static readonly string[] Entries = { "プロローグ", "スタート", "エピローグ", "勝利条件", "再開" };

        /// <summary>イベントのラベルの名前（* と : を除いたもの）から、種類といつ起きるか</summary>
        public static (string kind, string when) Classify(string label)
        {
            var tokens = Tokens(label);
            if (tokens.Count == 0)
            {
                return ("sub", "");
            }
            var head = tokens[0];
            string Arg(int i) => i < tokens.Count ? EventPlacementParser.Unquote(tokens[i]) : "";
            switch (head)
            {
                case "プロローグ":
                    return ("entry", "話の始め（マップの前）");
                case "スタート":
                    return ("start", "戦闘の始め（マップを出したあと）");
                case "エピローグ":
                    return ("entry", "話の終わり");
                case "勝利条件":
                    return ("entry", "作戦目的を見たとき");
                case "再開":
                    return ("entry", "中断から再開したとき");
                case "ターン":
                    return ("turn", Arg(1) == "全" ? $"毎ターンの{Arg(2)}のフェイズの始め" : $"{Arg(1)} ターン目の{Arg(2)}のフェイズの始め");
                case "破壊":
                case "マップ攻撃破壊":
                    return ("destroyed", $"{Arg(1)}が落ちたとき");
                case "損傷率":
                    return ("damaged", $"{Arg(1)}の損傷率が {Arg(2)}% 以上になったとき");
                case "全滅":
                    return ("annihilated", $"{Arg(1)}が全滅したとき");
                case "攻撃":
                    return ("attack", $"{Arg(1)}と{Arg(2)}が戦うとき");
                case "攻撃後":
                    return ("attack", $"{Arg(1)}と{Arg(2)}が戦ったあと");
                case "会話":
                    return ("talk", $"{Arg(1)}が{Arg(2)}に話しかけたとき");
                case "接触":
                    return ("other", $"{Arg(1)}と{Arg(2)}が隣り合ったとき");
                case "進入":
                    return ("enter", tokens.Count >= 4 ? $"{Arg(1)}が ({Arg(2)},{Arg(3)}) に入ったとき" : $"{Arg(1)}が{Arg(2)}に入ったとき");
                case "脱出":
                    return ("enter", $"{Arg(1)}がマップの端から出たとき");
                default:
                    return Triggers.Contains(head) ? ("other", $"{head} {string.Join(" ", tokens.Skip(1).Select(EventPlacementParser.Unquote))}") : ("sub", "Call・Goto の先");
            }
        }

        internal static List<string> Tokens(string text) =>
            EventPlacementParser.TrySplitWithSpans(text ?? "", out var spans) ? spans.Select(x => x.Value).ToList() : new List<string>();

        private static readonly Regex Assign = new Regex(@"^(?<v>[^\s=<>!""'$()]+)\s*=\s*(?<e>.+)$", RegexOptions.CultureInvariant);
        private static readonly Regex Interpolation = new Regex(@"\$\((?<v>[^()]+)\)", RegexOptions.CultureInvariant);

        /// <summary>シナリオのフォルダかイベントファイルを解析する。話の順は話のつながり（Continue など）の順</summary>
        public static ScenarioOutline Analyze(string target, string appPath = null)
        {
            var flow = new ScenarioFlowAnalyzer { AppPath = appPath }.Analyze(target);
            var outline = new ScenarioOutline { ScenarioRoot = flow.ScenarioRoot };
            var files = flow.Nodes.Where(n => !string.IsNullOrEmpty(n.FullPath) && File.Exists(n.FullPath)).Select(n => n.FullPath).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (files.Count == 0 && File.Exists(target))
            {
                files.Add(Path.GetFullPath(target));
            }
            var variables = new Dictionary<string, OutlineVariable>(StringComparer.Ordinal);
            var reads = new List<(string text, string file, int line)>();
            foreach (var file in files)
            {
                var analysis = new ScenarioAnalyzer { AppPath = appPath }.Analyze(file, null, flow.ScenarioRoot);
                outline.Episodes.Add(AnalyzeEpisode(outline, analysis, file, variables, reads));
            }
            // 読む: 条件・式・$() に出てくる変数（設定している変数の名前を、語として探す）
            foreach (var (text, file, line) in reads)
            {
                foreach (var v in variables.Values)
                {
                    if (ContainsWord(text, v.Name) && !v.Reads.Contains((file, line)))
                    {
                        v.Reads.Add((file, line));
                    }
                }
            }
            outline.Variables.AddRange(variables.Values.OrderBy(v => v.Name, StringComparer.Ordinal));
            outline.Check(flow);
            return outline;
        }

        private static readonly string[] Parties = { "味方", "敵", "中立", "ＮＰＣ", "NPC", "全" };

        /// <summary>
        /// 気づいたこと（7474/SRC#1692 の「検査」）: 起きない出来事（どの話でも出てこない相手の破壊・攻撃・損傷率）、
        /// 設定するだけで読まない変数・書き間違いらしい変数（1 文字違い）、勝ち負けの無い話、Continue の先が無い話
        /// </summary>
        private void Check(ScenarioFlow flow)
        {
            // 出てくる人・ユニット（話をまたいで続けて出るので、シナリオ全体で見る）
            var known = new HashSet<string>(Episodes.SelectMany(e => e.Joins.Concat(e.Launches).Concat(e.Enemies).Concat(e.UnitNames)), StringComparer.Ordinal);
            foreach (var e in Episodes)
            {
                foreach (var ev in e.Events.Where(x => x.Kind == "destroyed" || x.Kind == "damaged" || x.Kind == "attack"))
                {
                    var tokens = Tokens(ev.Label).Skip(1).Take(ev.Kind == "attack" ? 2 : 1).Select(EventPlacementParser.Unquote);
                    foreach (var who in tokens.Where(t => !Parties.Contains(t) && !known.Contains(t) && !t.Contains("$(")))
                    {
                        Findings.Add(new OutlineFinding { File = ev.File, Line = ev.Line, Message = $"「{ev.Label}」の「{who}」は、どの話でも Create・Launch されていない（起きない出来事かもしれない）" });
                    }
                }
                // 戦闘のある話（スタートでマップを出すかユニットを置く）だけを見る。インターミッションのメニューなどは除く
                var hasStart = e.Events.Any(x => x.Kind == "start" && x.Actions.Any(a => a.StartsWith("マップ") || a.StartsWith("配置") || a.StartsWith("加入") || a.StartsWith("出撃")));
                var ends = e.Events.SelectMany(x => x.Actions).Any(a => a.StartsWith("勝ち") || a.StartsWith("次の話へ")) || e.Events.Any(x => x.Label == "全滅 敵");
                if (hasStart && !ends && e.Events.Count > 0)
                {
                    Findings.Add(new OutlineFinding { File = e.Events[0].File, Line = e.Events[0].Line, Message = $"{e.Title}に勝ちの終わり方（全滅 敵・GameClear・Continue）が無い" });
                }
            }
            foreach (var v in Variables)
            {
                if (v.Sets.Count > 0 && v.Reads.Count == 0)
                {
                    Findings.Add(new OutlineFinding { File = v.Sets[0].file, Line = v.Sets[0].line, Message = $"変数「{v.Name}」は設定するだけで、どこでも読んでいない" });
                }
            }
            var names = Variables.Select(v => v.Name).Where(n => n.Length >= 3).ToList();
            for (var i = 0; i < names.Count; i++)
            {
                for (var j = i + 1; j < names.Count; j++)
                {
                    if (OneEditApart(names[i], names[j]))
                    {
                        var v = Variables.First(x => x.Name == names[j]);
                        var site = v.Sets.Concat(v.Reads).FirstOrDefault();
                        Findings.Add(new OutlineFinding { File = site.file, Line = site.line, Message = $"変数「{names[i]}」と「{names[j]}」は 1 文字違い（書き間違いかもしれない）" });
                    }
                }
            }
            foreach (var w in flow.Warnings)
            {
                Findings.Add(new OutlineFinding { File = string.IsNullOrEmpty(w.FullFile) ? w.File : Rel(w.FullFile), Line = w.Line, Message = w.Message });
            }
        }

        private static bool OneEditApart(string a, string b)
        {
            if (Math.Abs(a.Length - b.Length) > 1 || a == b)
            {
                return false;
            }
            int i = 0, j = 0, edits = 0;
            while (i < a.Length && j < b.Length)
            {
                if (a[i] == b[j])
                {
                    i++;
                    j++;
                    continue;
                }
                if (++edits > 1)
                {
                    return false;
                }
                if (a.Length > b.Length)
                {
                    i++;
                }
                else if (a.Length < b.Length)
                {
                    j++;
                }
                else
                {
                    i++;
                    j++;
                }
            }
            return edits + (a.Length - i) + (b.Length - j) <= 1;
        }

        private static bool ContainsWord(string text, string name)
        {
            var i = 0;
            while ((i = text.IndexOf(name, i, StringComparison.Ordinal)) >= 0)
            {
                var before = i == 0 ? ' ' : text[i - 1];
                var after = i + name.Length >= text.Length ? ' ' : text[i + name.Length];
                if (!IsWordChar(before) && !IsWordChar(after))
                {
                    return true;
                }
                i += name.Length;
            }
            return false;
        }

        private static bool IsWordChar(char c) => char.IsLetterOrDigit(c) || c == '_' || c > 0x2fff;

        private string Rel(string file) => string.IsNullOrEmpty(ScenarioRoot) ? file : Path.GetRelativePath(ScenarioRoot, file).Replace('\\', '/');

        private static OutlineEpisode AnalyzeEpisode(ScenarioOutline outline, ScenarioAnalysis analysis, string file, Dictionary<string, OutlineVariable> variables,
            List<(string text, string file, int line)> reads)
        {
            var episode = new OutlineEpisode { File = outline.Rel(file), Title = Path.GetFileNameWithoutExtension(file) };
            OutlineEvent current = null;
            string speaker = null;
            var inTalk = false;
            var talkKind = "talk";
            string switchExpr = null;
            foreach (var line in analysis.Lines.Where(l => !l.IsSystemData))
            {
                var data = (line.Data ?? "").Trim();
                if (data.Length == 0)
                {
                    continue;
                }
                var rel = outline.Rel(string.IsNullOrEmpty(line.File) ? file : line.File);
                if (inTalk)
                {
                    var head = FirstToken(data);
                    if (Is(head, "End") || Is(head, "Suspend"))
                    {
                        inTalk = false;
                        continue;
                    }
                    if (Is(head, "Talk") || Is(head, "AutoTalk"))
                    {
                        speaker = TalkSpeaker(data);
                        continue;
                    }
                    outline.Script.Add(new OutlineLine { Episode = episode.Title, Event = current?.Label, File = rel, Line = line.LineNum, Kind = talkKind, Speaker = speaker, Text = data });
                    if (current != null)
                    {
                        current.TalkLines++;
                    }
                    var who = string.IsNullOrEmpty(speaker) ? "（地の文）" : speaker;
                    episode.Speakers[who] = episode.Speakers.TryGetValue(who, out var n) ? n + 1 : 1;
                    foreach (Match m in Interpolation.Matches(data))
                    {
                        reads.Add((m.Groups["v"].Value, rel, line.LineNum));
                    }
                    continue;
                }
                if (IsLabelLine(data))
                {
                    var name = data.TrimEnd(':').Trim();
                    var always = name.StartsWith("*", StringComparison.Ordinal);
                    name = name.TrimStart('*').Trim();
                    var (kind, when) = Classify(name);
                    current = new OutlineEvent { File = rel, Line = line.LineNum, Label = name, Kind = kind, When = when, Always = always };
                    episode.Events.Add(current);
                    continue;
                }
                var tokens = Tokens(data);
                if (tokens.Count == 0)
                {
                    continue;
                }
                var cmd = tokens[0];
                string A(int i) => i < tokens.Count ? EventPlacementParser.Unquote(tokens[i]) : "";
                void Act(string text)
                {
                    if (current != null && !current.Actions.Contains(text))
                    {
                        current.Actions.Add(text);
                    }
                }
                if (Is(cmd, "Talk") || Is(cmd, "AutoTalk"))
                {
                    inTalk = true;
                    talkKind = Is(cmd, "AutoTalk") ? "autotalk" : "talk";
                    speaker = TalkSpeaker(data);
                    Act("会話");
                    continue;
                }
                if (Is(cmd, "Telop"))
                {
                    var text = string.Join(" ", tokens.Skip(1).Select(EventPlacementParser.Unquote));
                    outline.Script.Add(new OutlineLine { Episode = episode.Title, Event = current?.Label, File = rel, Line = line.LineNum, Kind = "telop", Text = text });
                    Act("テロップ");
                    continue;
                }
                if (Is(cmd, "If") || Is(cmd, "ElseIf") || Is(cmd, "Case") || Is(cmd, "Switch") || Is(cmd, "Do") || Is(cmd, "Loop"))
                {
                    var expr = Regex.Replace(data.Substring(cmd.Length).Trim(), @"\s+Then(\s.*)?$", "", RegexOptions.IgnoreCase);
                    if (expr.Length > 0)
                    {
                        if (Is(cmd, "Switch"))
                        {
                            switchExpr = expr;
                        }
                        else
                        {
                            var shown = Is(cmd, "Case") ? (Is(expr, "Else") ? $"{switchExpr} がほかのとき" : $"{switchExpr} = {expr}") : expr;
                            if (current != null && !current.Conditions.Contains(shown))
                            {
                                current.Conditions.Add(shown);
                            }
                        }
                        reads.Add((expr, rel, line.LineNum));
                    }
                    continue;
                }
                // 命令の引数に書いた変数（Ride パイロット 変数 など）も読むとして数える
                if (!Assign.IsMatch(data) && !Is(cmd, "Set") && !Is(cmd, "Incr") && !Is(cmd, "Global") && !Is(cmd, "Local"))
                {
                    reads.Add((string.Join(" ", tokens.Skip(1)), rel, line.LineNum));
                }
                switch (cmd.ToLowerInvariant())
                {
                    case "create":
                        if (tokens.Count >= 5)
                        {
                            var party = A(1);
                            var who = $"{A(4)}（{A(2)}）";
                            if (party == "味方")
                            {
                                Act($"加入・出撃: {who}");
                                AddOnce(episode.Joins, A(4));
                                episode.UnitNames.Add(A(2));
                            }
                            else
                            {
                                Act(current?.Kind == "start" || current?.Kind == "entry" ? $"配置: {who}" : $"援軍: {who}");
                                AddOnce(episode.Enemies, A(4));
                                episode.UnitNames.Add(A(2));
                            }
                        }
                        break;
                    case "launch":
                        if (tokens.Count >= 4)
                        {
                            Act($"出撃: {A(1)}");
                            AddOnce(episode.Launches, A(1));
                        }
                        break;
                    case "join":
                        Act($"加入: {A(1)}");
                        AddOnce(episode.Joins, A(1));
                        break;
                    case "leave":
                        Act($"離脱: {A(1)}");
                        AddOnce(episode.Leaves, A(1));
                        break;
                    case "escape":
                        Act($"撤退: {A(1)}");
                        break;
                    case "ride":
                        Act($"乗り換え: {A(1)}");
                        break;
                    case "changeparty":
                        Act($"陣営を変える: {string.Join(" ", tokens.Skip(1).Select(EventPlacementParser.Unquote))}");
                        break;
                    case "gameclear":
                        Act("勝ち（GameClear）");
                        break;
                    case "gameover":
                        Act("負け（GameOver）");
                        break;
                    case "continue":
                        if (tokens.Count >= 2)
                        {
                            Act($"次の話へ: {A(1)}");
                            AddOnce(episode.Next, A(1));
                        }
                        break;
                    case "changemap":
                        Act($"マップ: {A(1)}");
                        break;
                    case "call":
                    case "goto":
                        Act($"{cmd}: {A(1)}");
                        break;
                    case "set":
                    case "incr":
                    case "global":
                    case "local":
                        foreach (var name in Is(cmd, "global") || Is(cmd, "local") ? tokens.Skip(1).Select(EventPlacementParser.Unquote) : new[] { A(1) })
                        {
                            SetVariable(variables, name, Is(cmd, "global"), rel, line.LineNum, Is(cmd, "global") || Is(cmd, "local"));
                        }
                        if ((Is(cmd, "set") || Is(cmd, "incr")) && tokens.Count >= 3)
                        {
                            reads.Add((string.Join(" ", tokens.Skip(2)), rel, line.LineNum));
                        }
                        break;
                    default:
                        var assign = Assign.Match(data);
                        if (assign.Success && !Is(cmd, "Talk"))
                        {
                            SetVariable(variables, assign.Groups["v"].Value, false, rel, line.LineNum, false);
                            reads.Add((assign.Groups["e"].Value, rel, line.LineNum));
                        }
                        break;
                }
                foreach (Match m in Interpolation.Matches(data))
                {
                    reads.Add((m.Groups["v"].Value, rel, line.LineNum));
                }
            }
            return episode;
        }

        private static bool IsLabelLine(string data) =>
            data.Length > 1 && data.EndsWith(":", StringComparison.Ordinal) && !data.StartsWith("#", StringComparison.Ordinal)
            && !Regex.IsMatch(data, @"^\S+\s*=");

        private static void SetVariable(Dictionary<string, OutlineVariable> variables, string name, bool global, string file, int line, bool declareOnly)
        {
            if (string.IsNullOrWhiteSpace(name) || name.StartsWith("$", StringComparison.Ordinal))
            {
                return;
            }
            // 配列の要素（名前[添字]）は、配列の名前で数える
            var bracket = name.IndexOf('[');
            if (bracket > 0)
            {
                name = name.Substring(0, bracket);
            }
            if (!variables.TryGetValue(name, out var v))
            {
                v = new OutlineVariable { Name = name };
                variables[name] = v;
            }
            v.Global |= global;
            if (!declareOnly)
            {
                v.Sets.Add((file, line));
            }
        }

        private static void AddOnce(List<string> list, string value)
        {
            if (!string.IsNullOrEmpty(value) && !list.Contains(value))
            {
                list.Add(value);
            }
        }

        private static string FirstToken(string data)
        {
            var i = data.IndexOfAny(new[] { ' ', '\t', '　' });
            return i < 0 ? data : data.Substring(0, i);
        }

        private static bool Is(string token, string command) => string.Equals(token, command, StringComparison.OrdinalIgnoreCase);

        private static string TalkSpeaker(string data)
        {
            var tokens = Tokens(data);
            return tokens.Count >= 2 ? EventPlacementParser.Unquote(tokens[1]) : "";
        }

        // ---- 書き出し ----

        private string linkBase;

        private string Link(string file, int line)
        {
            var target = string.IsNullOrEmpty(linkBase) || string.IsNullOrEmpty(ScenarioRoot)
                ? file
                : Path.GetRelativePath(linkBase, Path.Combine(ScenarioRoot, file)).Replace('\\', '/');
            return $"[{file}:{line}]({string.Join("/", target.Split('/').Select(Uri.EscapeDataString))}#L{line})";
        }

        /// <summary>Markdown。linkBase は書き出すフォルダ（行へのリンクをそこからの相対にする。省くとシナリオのフォルダから）</summary>
        public string ToMarkdown(string linkBase = null)
        {
            this.linkBase = linkBase;
            var sb = new StringBuilder();
            sb.Append("# シナリオの見通し\n\n");
            sb.Append($"話 {Episodes.Count}・出来事 {Episodes.Sum(e => e.Events.Count)}・台本 {Script.Count} 行・変数 {Variables.Count}・気づいたこと {Findings.Count}\n\n");
            if (Findings.Count > 0)
            {
                sb.Append("## 気づいたこと\n\n");
                foreach (var f in Findings)
                {
                    sb.Append($"- {(string.IsNullOrEmpty(f.File) ? "" : Link(f.File, f.Line) + " ")}{Escape(f.Message)}\n");
                }
                sb.Append('\n');
            }

            sb.Append("## 部隊と登場人物\n\n| 話 | 加入・出撃 | 続けて出る | 離脱 | 相手 | 次の話 |\n|---|---|---|---|---|---|\n");
            foreach (var e in Episodes)
            {
                sb.Append($"| {e.Title} | {Join(e.Joins)} | {Join(e.Launches)} | {Join(e.Leaves)} | {Join(e.Enemies)} | {Join(e.Next)} |\n");
            }
            var people = Episodes.SelectMany(e => e.Speakers.Select(s => (episode: e.Title, who: s.Key, n: s.Value))).GroupBy(x => x.who).OrderByDescending(g => g.Sum(x => x.n));
            sb.Append("\n### 話す人\n\n| 話す人 | 行数 | 出る話 |\n|---|---|---|\n");
            foreach (var g in people)
            {
                sb.Append($"| {g.Key} | {g.Sum(x => x.n)} | {string.Join("、", g.Select(x => $"{x.episode}（{x.n}）"))} |\n");
            }

            sb.Append("\n## 出来事\n");
            foreach (var e in Episodes)
            {
                sb.Append($"\n### {e.Title}\n\n| 行 | きっかけ | いつ | 条件 | すること |\n|---|---|---|---|---|\n");
                foreach (var ev in e.Events)
                {
                    var actions = ev.Actions.Select(a => a == "会話" ? $"会話 {ev.TalkLines} 行" : a);
                    sb.Append($"| {Link(ev.File, ev.Line)} | {Escape(ev.Label)}{(ev.Always ? "（常時）" : "")} | {Escape(ev.When)} | {Escape(string.Join("／", ev.Conditions))} | {Escape(string.Join("、", actions))} |\n");
                }
            }

            sb.Append("\n## 変数\n\n| 変数 | 設定 | 読む |\n|---|---|---|\n");
            foreach (var v in Variables)
            {
                sb.Append($"| {Escape(v.Name)}{(v.Global ? "（Global）" : "")} | {string.Join(" ", v.Sets.Select(x => Link(x.file, x.line)))} | {string.Join(" ", v.Reads.Select(x => Link(x.file, x.line)))} |\n");
            }

            sb.Append("\n## 台本\n");
            foreach (var g in Script.GroupBy(x => x.Episode))
            {
                sb.Append($"\n### {g.Key}\n");
                string lastEvent = null;
                string lastSpeaker = null;
                foreach (var l in g)
                {
                    if (l.Event != lastEvent)
                    {
                        sb.Append($"\n**{Escape(l.Event ?? "")}**\n\n");
                        lastEvent = l.Event;
                        lastSpeaker = null;
                    }
                    if (l.Kind == "telop")
                    {
                        sb.Append($"> 〔テロップ〕{Escape(l.Text)} {Link(l.File, l.Line)}\n\n");
                        lastSpeaker = null;
                        continue;
                    }
                    var who = string.IsNullOrEmpty(l.Speaker) ? "" : l.Speaker;
                    if (who != lastSpeaker)
                    {
                        sb.Append(string.IsNullOrEmpty(who) ? "" : $"{Escape(who)}「");
                    }
                    sb.Append($"{Escape(l.Text)} {Link(l.File, l.Line)}\n\n");
                    lastSpeaker = who;
                }
            }
            return sb.ToString();
        }

        private static string Join(IEnumerable<string> values) => Escape(string.Join("、", values));

        private static string Escape(string s) => (s ?? "").Replace("|", "\\|").Replace("\n", " ");

        public string ToJson()
        {
            var options = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true };
            using var stream = new MemoryStream();
            using (var w = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }))
            {
                w.WriteStartObject();
                w.WriteString("scenarioRoot", ScenarioRoot);
                w.WriteStartArray("episodes");
                foreach (var e in Episodes)
                {
                    w.WriteStartObject();
                    w.WriteString("file", e.File);
                    w.WriteString("title", e.Title);
                    WriteList(w, "joins", e.Joins);
                    WriteList(w, "launches", e.Launches);
                    WriteList(w, "leaves", e.Leaves);
                    WriteList(w, "enemies", e.Enemies);
                    WriteList(w, "next", e.Next);
                    w.WriteStartObject("speakers");
                    foreach (var (k, v) in e.Speakers)
                    {
                        w.WriteNumber(k, v);
                    }
                    w.WriteEndObject();
                    w.WriteStartArray("events");
                    foreach (var ev in e.Events)
                    {
                        w.WriteStartObject();
                        w.WriteString("file", ev.File);
                        w.WriteNumber("line", ev.Line);
                        w.WriteString("label", ev.Label);
                        w.WriteString("kind", ev.Kind);
                        w.WriteString("when", ev.When);
                        w.WriteBoolean("always", ev.Always);
                        WriteList(w, "conditions", ev.Conditions);
                        WriteList(w, "actions", ev.Actions);
                        w.WriteNumber("talkLines", ev.TalkLines);
                        w.WriteEndObject();
                    }
                    w.WriteEndArray();
                    w.WriteEndObject();
                }
                w.WriteEndArray();
                w.WriteStartArray("script");
                foreach (var l in Script)
                {
                    w.WriteStartObject();
                    w.WriteString("episode", l.Episode);
                    w.WriteString("event", l.Event);
                    w.WriteString("file", l.File);
                    w.WriteNumber("line", l.Line);
                    w.WriteString("kind", l.Kind);
                    w.WriteString("speaker", l.Speaker);
                    w.WriteString("text", l.Text);
                    w.WriteEndObject();
                }
                w.WriteEndArray();
                w.WriteStartArray("findings");
                foreach (var f in Findings)
                {
                    w.WriteStartObject();
                    w.WriteString("file", f.File);
                    w.WriteNumber("line", f.Line);
                    w.WriteString("message", f.Message);
                    w.WriteEndObject();
                }
                w.WriteEndArray();
                w.WriteStartArray("variables");
                foreach (var v in Variables)
                {
                    w.WriteStartObject();
                    w.WriteString("name", v.Name);
                    w.WriteBoolean("global", v.Global);
                    WriteSites(w, "sets", v.Sets);
                    WriteSites(w, "reads", v.Reads);
                    w.WriteEndObject();
                }
                w.WriteEndArray();
                w.WriteEndObject();
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private static void WriteList(Utf8JsonWriter w, string name, IEnumerable<string> values)
        {
            w.WriteStartArray(name);
            foreach (var v in values)
            {
                w.WriteStringValue(v);
            }
            w.WriteEndArray();
        }

        private static void WriteSites(Utf8JsonWriter w, string name, IEnumerable<(string file, int line)> sites)
        {
            w.WriteStartArray(name);
            foreach (var (file, line) in sites)
            {
                w.WriteStartObject();
                w.WriteString("file", file);
                w.WriteNumber("line", line);
                w.WriteEndObject();
            }
            w.WriteEndArray();
        }
    }
}
