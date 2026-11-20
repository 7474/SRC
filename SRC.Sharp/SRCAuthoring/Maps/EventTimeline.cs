using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace SRCAuthoring.Maps
{
    /// <summary>人形劇の 1 手の時点で、マップにいるユニット</summary>
    public sealed class TimelineUnit
    {
        /// <summary>見分けるための名前（パイロットがいればパイロット、いなければユニット）</summary>
        public string Key { get; set; } = "";
        public string Party { get; set; } = "";
        public string Unit { get; set; } = "";
        public string Pilot { get; set; } = "";
        public string Bitmap { get; set; } = "";
        public int X { get; set; }
        public int Y { get; set; }

        public TimelineUnit Clone() => (TimelineUnit)MemberwiseClone();
    }

    /// <summary>人形劇の 1 手（イベントの 1 行。会話は文の 1 行ずつ）</summary>
    public sealed class TimelineStep
    {
        public int Index { get; set; }
        /// <summary>1 始まりの行番号</summary>
        public int Line { get; set; }
        /// <summary>create・launch・move・escape・center・talk・wait・changemap・other</summary>
        public string Kind { get; set; } = "";
        public string Command { get; set; } = "";
        public string Text { get; set; } = "";
        public string Speaker { get; set; }
        public string Message { get; set; }
        /// <summary>動かす・出す・退かせるユニット（Key）</summary>
        public string Target { get; set; }
        public int? FromX { get; set; }
        public int? FromY { get; set; }
        public int? ToX { get; set; }
        public int? ToY { get; set; }
        /// <summary>命令に書いた行き先（埋まっている・入れないときは ToX・ToY と違う）</summary>
        public int? RequestedX { get; set; }
        public int? RequestedY { get; set; }
        /// <summary>「アニメ表示」のときの通り道（始まりと行き先を含む）。VB 版の MoveUnitBitmap と同じ L 字</summary>
        public List<int[]> Path { get; set; }
        public int? CenterX { get; set; }
        public int? CenterY { get; set; }
        /// <summary>この手の後のマップのユニット</summary>
        public List<TimelineUnit> Units { get; set; } = new List<TimelineUnit>();
        public List<string> Warnings { get; set; } = new List<string>();
    }

    /// <summary>イベントの塊（ラベルから次のラベルまで）</summary>
    public sealed class TimelineBlock
    {
        public string Label { get; set; } = "";
        /// <summary>ラベルの行（1 始まり）</summary>
        public int Line { get; set; }
        public int LastLine { get; set; }
        /// <summary>この塊にある動き（Create・Launch・Move・Escape）と会話の数（一覧で人形劇らしい塊を選びやすくする）</summary>
        public int Moves { get; set; }
        public int Talks { get; set; }
    }

    /// <summary>行き先を決めるためのマップの情報（無ければ地形は見ずに、埋まっているかだけで決める）</summary>
    public sealed class TimelineMap
    {
        public int Width { get; set; }
        public int Height { get; set; }
        /// <summary>(x, y)（1 始まり）の地形タイプと移動コスト。分からなければ null</summary>
        public Func<int, int, (string terrainClass, double moveCost)?> Terrain { get; set; }

        public static TimelineMap From(MapDocument map, TerrainCatalog terrains)
        {
            if (map == null)
            {
                return null;
            }
            var rows = map.TileRows();
            var byId = (terrains?.Terrains ?? Enumerable.Empty<TerrainInfo>()).GroupBy(t => t.Id).ToDictionary(g => g.Key, g => g.Last());
            return new TimelineMap
            {
                Width = map.Width,
                Height = map.Height,
                Terrain = (x, y) =>
                {
                    var i = ((y - 1) * map.Width + (x - 1)) * 2;
                    if (i < 0 || i >= rows.Length || !byId.TryGetValue(rows[i], out var info))
                    {
                        return null;
                    }
                    var cost = info.MoveCost == "-" ? 1000
                        : double.TryParse(info.MoveCost, NumberStyles.Float, CultureInfo.InvariantCulture, out var c) ? c : 1;
                    return (info.Class, cost);
                },
            };
        }
    }

    /// <summary>
    /// 人形劇（マップの上でユニットを動かしながら会話する場面）を一手ずつ進めた状態を作る（7474/SRC#1694）。
    /// SRC#Studio のマップ作成ツールと VS Code（同じ画面）、srcsharp timeline が使う。
    /// 行き先は VB 版の Unit.Jump と同じ規則で決める: 埋まっている・入れないマスなら、行き先からマス目の距離 0〜10 の順に空いたマスを探す。
    /// 「アニメ表示」の通り道は VB 版の MoveUnitBitmap と同じ L 字（長い方の軸を先に進む。地形は見ない）。
    /// </summary>
    public static class EventTimeline
    {
        private static readonly string[] Parties = { "味方", "敵", "中立", "ＮＰＣ" };

        /// <summary>イベントの塊（ラベルの行から次のラベルの前まで）の一覧</summary>
        public static IReadOnlyList<TimelineBlock> Blocks(string text)
        {
            var lines = SplitLines(text);
            var result = new List<TimelineBlock>();
            TimelineBlock current = null;
            var inTalk = false;
            for (var n = 0; n < lines.Length; n++)
            {
                var trimmed = lines[n].Trim();
                if (IsSkippable(trimmed))
                {
                    continue;
                }
                if (inTalk)
                {
                    if (IsEnd(trimmed))
                    {
                        inTalk = false;
                    }
                    else if (current != null && !IsTalkStart(trimmed))
                    {
                        current.Talks++;
                    }
                    continue;
                }
                if (IsLabel(trimmed))
                {
                    if (current != null)
                    {
                        current.LastLine = n;
                    }
                    current = new TimelineBlock { Label = trimmed.Substring(0, trimmed.Length - 1).Trim(), Line = n + 1 };
                    result.Add(current);
                    continue;
                }
                if (IsTalkStart(trimmed))
                {
                    inTalk = true;
                    continue;
                }
                var cmd = FirstToken(trimmed);
                if (current != null && (Is(cmd, "Create") || Is(cmd, "Launch") || Is(cmd, "Move") || Is(cmd, "Escape")))
                {
                    current.Moves++;
                }
            }
            if (current != null)
            {
                current.LastLine = lines.Length;
            }
            return result;
        }

        /// <summary>
        /// line（1 始まり）を含む塊を、ラベルの次の行から一手ずつ進める。
        /// 塊の外の行なら、その行から次のラベルか Exit まで。
        /// </summary>
        public static IReadOnlyList<TimelineStep> Build(string text, int line, TimelineMap map = null, PlacementUnitCatalog catalog = null)
        {
            catalog ??= PlacementUnitCatalog.Empty;
            var lines = SplitLines(text);
            var block = Blocks(text).LastOrDefault(b => b.Line <= line);
            var start = block != null ? block.Line : Math.Max(1, line) - 1;
            var steps = new List<TimelineStep>();
            var units = new List<TimelineUnit>();
            // この塊に出てくる人（後で出る人も含む）。会話の話し手が「その時点でいない」のを知らせる
            var castLater = CollectCast(lines, start, catalog);
            // 「先陣 = UnitID(煤鴉兵(ザコ))」のように、ユニットを指す変数
            var aliases = new Dictionary<string, string>(StringComparer.Ordinal);
            TimelineUnit FindUnit(string name, PlacementUnitCatalog c) =>
                Find(units, name != null && aliases.TryGetValue(name, out var alias) ? alias : name, c);
            TimelineUnit selected = null;
            var speaker = "";
            var inTalk = false;

            for (var n = start; n < lines.Length; n++)
            {
                var raw = lines[n];
                var trimmed = raw.Trim();
                if (IsSkippable(trimmed))
                {
                    continue;
                }
                if (inTalk)
                {
                    if (IsEnd(trimmed))
                    {
                        inTalk = false;
                        continue;
                    }
                    if (IsTalkStart(trimmed))
                    {
                        speaker = TalkSpeaker(trimmed);
                        continue;
                    }
                    var talk = NewStep(steps, n, "talk", "Talk", trimmed, units);
                    talk.Speaker = speaker;
                    talk.Message = trimmed;
                    if (!string.IsNullOrEmpty(speaker) && castLater.Contains(speaker) && !OnMap(units, speaker, catalog))
                    {
                        talk.Warnings.Add($"話す人「{speaker}」は、この時点でマップにいない");
                    }
                    continue;
                }
                if (IsLabel(trimmed))
                {
                    break;
                }
                if (!EventPlacementParser.TrySplitWithSpans(raw, out var spans) || spans.Count == 0)
                {
                    continue;
                }
                var tokens = spans.Select(s => s.Value).ToList();
                var cmd = tokens[0];
                if (TryUnitIdAssignment(trimmed, out var variable, out var referred))
                {
                    aliases[variable] = referred;
                    continue;
                }
                if (IsTalkStart(trimmed))
                {
                    inTalk = true;
                    speaker = TalkSpeaker(trimmed);
                    continue;
                }
                if (Is(cmd, "Exit"))
                {
                    break;
                }
                if (Is(cmd, "ChangeMap"))
                {
                    units.Clear();
                    selected = null;
                    NewStep(steps, n, "changemap", cmd, trimmed, units);
                    continue;
                }
                if (Is(cmd, "Create"))
                {
                    var step = NewStep(steps, n, "create", cmd, trimmed, units);
                    if (tokens.Count < 8 || !TryInt(tokens[6], out var x) || !TryInt(tokens[7], out var y))
                    {
                        step.Warnings.Add("位置が式なので、置く場所が分からない");
                        continue;
                    }
                    var unitName = EventPlacementParser.Unquote(tokens[2]);
                    var pilot = EventPlacementParser.Unquote(tokens[4]);
                    var u = new TimelineUnit
                    {
                        Key = string.IsNullOrEmpty(pilot) ? unitName : pilot,
                        Party = NormalizeParty(EventPlacementParser.Unquote(tokens[1])),
                        Unit = unitName,
                        Pilot = pilot,
                        Bitmap = catalog.FindUnit(unitName)?.Bitmap ?? "",
                    };
                    Place(step, units, u, x, y, map, catalog);
                    selected = u;
                    continue;
                }
                if (Is(cmd, "Launch"))
                {
                    var step = NewStep(steps, n, "launch", cmd, trimmed, units);
                    var args = ArgsWithoutOptions(tokens);
                    string name;
                    int xi;
                    if (args.Count >= 4)
                    {
                        name = EventPlacementParser.Unquote(args[1]);
                        xi = 2;
                    }
                    else if (args.Count == 3)
                    {
                        name = selected?.Key ?? "";
                        xi = 1;
                    }
                    else
                    {
                        continue;
                    }
                    if (!TryInt(args[xi], out var x) || !TryInt(args[xi + 1], out var y))
                    {
                        step.Warnings.Add("位置が式なので、置く場所が分からない");
                        continue;
                    }
                    var u = FindUnit(name, catalog)?.Clone() ?? new TimelineUnit
                    {
                        Key = name,
                        Party = "味方",
                        Unit = name,
                        Bitmap = catalog.FindUnit(name)?.Bitmap ?? "",
                    };
                    units.RemoveAll(o => o.Key == u.Key);
                    Place(step, units, u, x, y, map, catalog);
                    selected = u;
                    continue;
                }
                if (Is(cmd, "Move"))
                {
                    var step = NewStep(steps, n, "move", cmd, trimmed, units);
                    string name;
                    int xi;
                    if (tokens.Count >= 3 && TryInt(tokens[1], out _))
                    {
                        name = selected?.Key ?? "";
                        xi = 1;
                    }
                    else if (tokens.Count >= 4)
                    {
                        name = EventPlacementParser.Unquote(tokens[1]);
                        xi = 2;
                    }
                    else
                    {
                        continue;
                    }
                    if (!TryInt(tokens[xi], out var x) || !TryInt(tokens[xi + 1], out var y))
                    {
                        step.Warnings.Add("行き先が式なので、動く場所が分からない");
                        continue;
                    }
                    var u = FindUnit(name, catalog);
                    if (u == null)
                    {
                        step.Target = name;
                        step.RequestedX = x;
                        step.RequestedY = y;
                        step.Warnings.Add($"「{name}」は、この時点でマップにいない（まだ出ていないか、もう退いた）");
                        continue;
                    }
                    var animated = tokens.Skip(xi + 2).Any(t => EventPlacementParser.Unquote(t).StartsWith("アニメ表示", StringComparison.Ordinal));
                    var (fx, fy) = (u.X, u.Y);
                    Place(step, units, u, x, y, map, catalog);
                    step.FromX = fx;
                    step.FromY = fy;
                    if (animated)
                    {
                        step.Path = LPath(fx, fy, u.X, u.Y);
                    }
                    selected = u;
                    continue;
                }
                if (Is(cmd, "Escape"))
                {
                    var step = NewStep(steps, n, "escape", cmd, trimmed, units);
                    var args = ArgsWithoutOptions(tokens);
                    var name = args.Count >= 2 ? EventPlacementParser.Unquote(args[1]) : selected?.Key ?? "";
                    var party = Parties.FirstOrDefault(p => p == NormalizeParty(name));
                    if (party != null)
                    {
                        units.RemoveAll(o => o.Party == party);
                    }
                    else
                    {
                        var u = FindUnit(name, catalog);
                        if (u == null)
                        {
                            step.Warnings.Add($"「{name}」は、この時点でマップにいない");
                        }
                        else
                        {
                            step.Target = u.Key;
                            step.FromX = u.X;
                            step.FromY = u.Y;
                            units.Remove(u);
                        }
                    }
                    step.Units = Snapshot(units);
                    continue;
                }
                if (Is(cmd, "Center"))
                {
                    var step = NewStep(steps, n, "center", cmd, trimmed, units);
                    if (tokens.Count >= 3 && TryInt(tokens[1], out var cx) && TryInt(tokens[2], out var cy))
                    {
                        step.CenterX = cx;
                        step.CenterY = cy;
                    }
                    else if (tokens.Count >= 2 && FindUnit(EventPlacementParser.Unquote(tokens[1]), catalog) is TimelineUnit cu)
                    {
                        step.CenterX = cu.X;
                        step.CenterY = cu.Y;
                    }
                    continue;
                }
                if (Is(cmd, "Wait"))
                {
                    NewStep(steps, n, "wait", cmd, trimmed, units);
                    continue;
                }
                NewStep(steps, n, "other", cmd, trimmed, units);
            }
            return steps;
        }

        /// <summary>場面の再生（7474/SRC#1694）で作るイベントファイルの名前</summary>
        public const string SceneEventName = "場面の再生.eve";

        /// <summary>VS Code が場面の再生のイベントファイルを置く、シナリオのフォルダからのフォルダ（試し戦闘の .srcsharp/trial と同じ扱い）</summary>
        public const string SceneFolder = ".srcsharp/scene";

        private const string SceneEndLabel = "場面の再生の終わり";

        private static readonly string[] PrologueSettings = { "Option", "RenameTerm", "RenameBGM", "Load", "Global", "Set", "Local", "Require" };

        private static readonly string[] StageSetup = { "ChangeMap", "Create", "Launch", "Organize", "BossRank", "Move", "Escape" };

        /// <summary>
        /// line を含む塊（場面）だけを再生するイベントファイルを作る（7474/SRC#1694）。
        /// プロローグからは設定の命令（Option・Load・Global・Set など。Input は既定の値の Set にする）だけを持ってきて、会話や選択は飛ばす。
        /// スタートで場面のマップ（mapPath。場面が ChangeMap していなければ）に移り、場面がスタートでなければ、スタートのユニットの配置（Create・Launch・Move など）をしてから、
        /// 場面を流す。場面の Exit と最後で「ここまで」と出して GameClear で終える。ほかの塊（Call の先や、場面の中で起きるイベント）はそのまま残す。
        /// </summary>
        public static string SceneScript(string text, int line, string mapPath)
        {
            var eol = (text ?? "").Contains("\r\n") ? "\r\n" : "\n";
            var lines = SplitLines(text);
            var blocks = Blocks(text);
            var scene = blocks.LastOrDefault(b => b.Line <= line);
            if (scene == null)
            {
                return null;
            }
            var prologue = blocks.FirstOrDefault(b => b.Label == "プロローグ");
            var start = blocks.FirstOrDefault(b => b.Label == "スタート");
            IEnumerable<string> Body(TimelineBlock b) => lines.Skip(b.Line).Take(Math.Max(0, b.LastLine - b.Line));

            var output = new List<string>();
            // 最初のラベルの前（シナリオの宣言など）はそのまま
            output.AddRange(lines.Take(blocks.Count > 0 ? blocks[0].Line - 1 : 0));
            output.Add($"# 場面の再生（SRC#Studio・VS Code の人形劇が作る。7474/SRC#1694）: {scene.Label}（{scene.Line} 行目）");

            output.Add("プロローグ:");
            if (prologue != null)
            {
                output.AddRange(SetupLines(Body(prologue), PrologueSettings, prologue: true));
            }
            output.Add("Exit");

            output.Add("スタート:");
            var setup = start != null && start != scene ? SetupLines(Body(start), StageSetup, prologue: false) : new List<string>();
            var body = Body(scene).Select(l => Is(l.Trim(), "Exit") ? l.Substring(0, l.Length - l.TrimStart().Length) + "Goto " + SceneEndLabel : l).ToList();
            var first = setup.Concat(body).Select(l => FirstToken(l.Trim())).FirstOrDefault(t => Is(t, "ChangeMap") || Is(t, "Create") || Is(t, "Launch") || Is(t, "Organize"));
            if (!Is(first ?? "", "ChangeMap") && !string.IsNullOrEmpty(mapPath))
            {
                output.Add("ChangeMap " + Quote(mapPath.Replace('/', '\\')));
            }
            output.AddRange(setup);
            output.AddRange(body);
            output.Add("Goto " + SceneEndLabel);
            output.Add(SceneEndLabel + ":");
            output.Add("Telop 場面の再生はここまで");
            output.Add("GameClear");

            // ほかの塊（Call の先・場面の中で起きるイベント）
            foreach (var b in blocks.Where(b => b != prologue && b != start && b != scene))
            {
                output.Add("");
                output.Add(lines[b.Line - 1]);
                output.AddRange(Body(b));
            }
            return string.Join(eol, output) + eol;
        }

        private static List<string> SetupLines(IEnumerable<string> body, string[] commands, bool prologue)
        {
            var result = new List<string>();
            var inTalk = false;
            var depth = 0;
            foreach (var raw in body)
            {
                var trimmed = raw.Trim();
                if (IsSkippable(trimmed))
                {
                    continue;
                }
                if (inTalk)
                {
                    inTalk = !IsEnd(trimmed);
                    continue;
                }
                if (IsTalkStart(trimmed) || Is(FirstToken(trimmed), "Question") || Is(FirstToken(trimmed), "Menu") || Is(FirstToken(trimmed), "Ask"))
                {
                    // 会話・選択の塊（End まで）は飛ばす
                    inTalk = true;
                    continue;
                }
                var cmd = FirstToken(trimmed);
                // If・Switch・Do などの中（条件しだいの命令）は持ってこない
                if (Is(cmd, "If") && !trimmed.EndsWith("Then", StringComparison.OrdinalIgnoreCase) && trimmed.IndexOf(" Then ", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    continue;
                }
                if (Is(cmd, "If") || Is(cmd, "Switch") || Is(cmd, "Do") || Is(cmd, "For") || Is(cmd, "ForEach"))
                {
                    depth++;
                    continue;
                }
                if (Is(cmd, "EndIf") || Is(cmd, "EndSw") || Is(cmd, "Loop") || Is(cmd, "Next"))
                {
                    depth = Math.Max(0, depth - 1);
                    continue;
                }
                if (depth > 0)
                {
                    continue;
                }
                if (prologue && Is(cmd, "Input") && EventPlacementParser.TrySplitWithSpans(trimmed, out var spans) && spans.Count >= 4)
                {
                    result.Add($"Set {spans[1].Value} {spans[spans.Count - 1].Value}");
                    continue;
                }
                if (commands.Any(c => Is(cmd, c)) || (!prologue && TryUnitIdAssignment(trimmed, out _, out _)))
                {
                    // 配置は場面の前の下ごしらえなので、動きを見せずに置く
                    result.Add(Is(cmd, "Move") ? System.Text.RegularExpressions.Regex.Replace(raw, @"\s+アニメ表示\s*$", "") : raw);
                }
            }
            return result;
        }

        /// <summary>
        /// after（1 始まり）の行の後に Move を差し込む。会話の途中（Talk と End の間）なら、会話をそこで切り、
        /// End・Move・Talk（同じ話す人）を差し込んで、会話の続きはそのまま残す。差し込んだ Move の行番号も返す
        /// </summary>
        public static (string text, int line) InsertMove(string text, int after, string name, int x, int y)
        {
            var eol = (text ?? "").Contains("\r\n") ? "\r\n" : "\n";
            var lines = SplitLines(text).ToList();
            after = Math.Max(0, Math.Min(after, lines.Count));
            var (inTalk, speaker) = TalkStateAfter(lines, after);
            var indent = IndentOf(lines, after);
            var move = $"{indent}Move {Quote(name)} {x} {y}";
            var insert = new List<string>();
            var moveLine = after + 1;
            if (inTalk)
            {
                var nextIndex = NextSignificant(lines, after);
                var next = nextIndex >= 0 ? lines[nextIndex].Trim() : "";
                if (IsEnd(next))
                {
                    // 会話の終わり（End）の後に差し込む
                    lines.Insert(nextIndex + 1, move);
                    return (string.Join(eol, lines), nextIndex + 2);
                }
                insert.Add($"{indent}End");
                insert.Add(move);
                moveLine = after + 2;
                if (!IsTalkStart(next))
                {
                    insert.Add($"{indent}Talk{(string.IsNullOrEmpty(speaker) ? "" : " " + Quote(speaker))}");
                }
            }
            else
            {
                insert.Add(move);
            }
            lines.InsertRange(after, insert);
            return (string.Join(eol, lines), moveLine);
        }

        public static string BlocksToJson(IReadOnlyList<TimelineBlock> blocks)
        {
            using var stream = new MemoryStream();
            using (var w = new Utf8JsonWriter(stream))
            {
                w.WriteStartArray();
                foreach (var b in blocks)
                {
                    w.WriteStartObject();
                    w.WriteString("label", b.Label);
                    w.WriteNumber("line", b.Line);
                    w.WriteNumber("lastLine", b.LastLine);
                    w.WriteNumber("moves", b.Moves);
                    w.WriteNumber("talks", b.Talks);
                    w.WriteEndObject();
                }
                w.WriteEndArray();
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        public static string StepsToJson(IReadOnlyList<TimelineStep> steps)
        {
            using var stream = new MemoryStream();
            using (var w = new Utf8JsonWriter(stream))
            {
                w.WriteStartArray();
                foreach (var s in steps)
                {
                    w.WriteStartObject();
                    w.WriteNumber("index", s.Index);
                    w.WriteNumber("line", s.Line);
                    w.WriteString("kind", s.Kind);
                    w.WriteString("command", s.Command);
                    w.WriteString("text", s.Text);
                    if (s.Speaker != null)
                    {
                        w.WriteString("speaker", s.Speaker);
                    }
                    if (s.Message != null)
                    {
                        w.WriteString("message", s.Message);
                    }
                    if (s.Target != null)
                    {
                        w.WriteString("target", s.Target);
                    }
                    WriteOptional(w, "fromX", s.FromX);
                    WriteOptional(w, "fromY", s.FromY);
                    WriteOptional(w, "toX", s.ToX);
                    WriteOptional(w, "toY", s.ToY);
                    WriteOptional(w, "requestedX", s.RequestedX);
                    WriteOptional(w, "requestedY", s.RequestedY);
                    WriteOptional(w, "centerX", s.CenterX);
                    WriteOptional(w, "centerY", s.CenterY);
                    if (s.Path != null)
                    {
                        w.WriteStartArray("path");
                        foreach (var p in s.Path)
                        {
                            w.WriteStartArray();
                            w.WriteNumberValue(p[0]);
                            w.WriteNumberValue(p[1]);
                            w.WriteEndArray();
                        }
                        w.WriteEndArray();
                    }
                    w.WriteStartArray("units");
                    foreach (var u in s.Units)
                    {
                        w.WriteStartObject();
                        w.WriteString("key", u.Key);
                        w.WriteString("party", u.Party);
                        w.WriteString("unit", u.Unit);
                        w.WriteString("pilot", u.Pilot);
                        w.WriteString("bitmap", u.Bitmap);
                        w.WriteNumber("x", u.X);
                        w.WriteNumber("y", u.Y);
                        w.WriteEndObject();
                    }
                    w.WriteEndArray();
                    w.WriteStartArray("warnings");
                    foreach (var warning in s.Warnings)
                    {
                        w.WriteStringValue(warning);
                    }
                    w.WriteEndArray();
                    w.WriteEndObject();
                }
                w.WriteEndArray();
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        /// <summary>VB 版の MoveUnitBitmap と同じ L 字の通り道（長い方の軸を先に進み、次にもう一方の軸）</summary>
        public static List<int[]> LPath(int x1, int y1, int x2, int y2)
        {
            var path = new List<int[]> { new[] { x1, y1 } };
            var (x, y) = (x1, y1);
            void Step(int dx, int dy, int count)
            {
                for (var i = 0; i < count; i++)
                {
                    x += dx;
                    y += dy;
                    path.Add(new[] { x, y });
                }
            }
            if (Math.Abs(x2 - x1) > Math.Abs(y2 - y1))
            {
                Step(Math.Sign(x2 - x1), 0, Math.Abs(x2 - x1));
                Step(0, Math.Sign(y2 - y1), Math.Abs(y2 - y1));
            }
            else
            {
                Step(0, Math.Sign(y2 - y1), Math.Abs(y2 - y1));
                Step(Math.Sign(x2 - x1), 0, Math.Abs(x2 - x1));
            }
            return path;
        }

        // ---- 行き先 ----

        /// <summary>ユニットを (x, y) に置く。埋まっている・入れないなら、VB 版の Unit.Jump と同じ順で空いたマスを探す</summary>
        private static void Place(TimelineStep step, List<TimelineUnit> units, TimelineUnit u, int x, int y, TimelineMap map, PlacementUnitCatalog catalog)
        {
            units.Remove(u);
            var width = map?.Width ?? int.MaxValue;
            var height = map?.Height ?? int.MaxValue;
            var tx = Math.Max(1, Math.Min(x, width));
            var ty = Math.Max(1, Math.Min(y, height));
            var app = catalog.FindUnit(u.Unit);
            var found = false;
            for (var i = 0; i <= 10 && !found; i++)
            {
                for (var j = Math.Max(tx - i, 1); j <= Math.Min(tx + i, width) && !found; j++)
                {
                    for (var k = Math.Max(ty - i, 1); k <= Math.Min(ty + i, height); k++)
                    {
                        if (Math.Abs(tx - j) + Math.Abs(ty - k) != i || units.Any(o => o.X == j && o.Y == k) || !CanEnter(map, j, k, app))
                        {
                            continue;
                        }
                        u.X = j;
                        u.Y = k;
                        found = true;
                        break;
                    }
                }
            }
            units.Add(u);
            step.Target = u.Key;
            step.ToX = u.X;
            step.ToY = u.Y;
            step.RequestedX = x;
            step.RequestedY = y;
            if (u.X != x || u.Y != y)
            {
                step.Warnings.Add($"({x},{y}) は埋まっているか入れないので、({u.X},{u.Y}) に置かれる");
            }
            step.Units = Snapshot(units);
        }

        private static bool CanEnter(TimelineMap map, int x, int y, UnitAppearance app)
        {
            var terrain = map?.Terrain?.Invoke(x, y);
            if (terrain == null)
            {
                return true;
            }
            var (terrainClass, cost) = terrain.Value;
            if (cost > 100)
            {
                return false;
            }
            if (app == null)
            {
                return true;
            }
            var trans = app.Transportation ?? "";
            switch (terrainClass)
            {
                case "空":
                    return trans.Contains("空");
                case "水":
                case "深水":
                    var water = (app.Adaption ?? "").Length >= 3 ? app.Adaption[2] : 'A';
                    return trans.Contains("水") || trans.Contains("空") || water != '-';
                default:
                    return true;
            }
        }

        // ---- 下まわり ----

        private static TimelineStep NewStep(List<TimelineStep> steps, int n, string kind, string command, string text, List<TimelineUnit> units)
        {
            var step = new TimelineStep
            {
                Index = steps.Count,
                Line = n + 1,
                Kind = kind,
                Command = command,
                Text = text,
                Units = Snapshot(units),
            };
            steps.Add(step);
            return step;
        }

        private static List<TimelineUnit> Snapshot(List<TimelineUnit> units) => units.Select(u => u.Clone()).ToList();

        private static readonly System.Text.RegularExpressions.Regex UnitIdAssignment = new System.Text.RegularExpressions.Regex(
            @"^(?:Set\s+(?<v>\S+)\s+|(?<v>\S+)\s*=\s*)UnitID\(\s*(?<n>.+?)\s*\)\s*$",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.CultureInvariant);

        /// <summary>「先陣 = UnitID(煤鴉兵(ザコ))」「Set 先陣 UnitID(…)」</summary>
        private static bool TryUnitIdAssignment(string line, out string variable, out string referred)
        {
            var m = UnitIdAssignment.Match(line);
            variable = m.Success ? m.Groups["v"].Value : null;
            referred = m.Success ? EventPlacementParser.Unquote(m.Groups["n"].Value) : null;
            return m.Success;
        }

        private static TimelineUnit Find(List<TimelineUnit> units, string name, PlacementUnitCatalog catalog)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            var canon = catalog.CanonicalPilot(name);
            return units.LastOrDefault(u => u.Key == name || u.Pilot == name || u.Unit == name
                || (!string.IsNullOrEmpty(canon) && (u.Pilot == canon || catalog.CanonicalPilot(u.Pilot) == canon)));
        }

        private static bool OnMap(List<TimelineUnit> units, string speaker, PlacementUnitCatalog catalog) => Find(units, speaker, catalog) != null;

        /// <summary>start の後の塊に出てくる人（Create・Launch のパイロット・ユニット、と愛称）</summary>
        private static HashSet<string> CollectCast(string[] lines, int start, PlacementUnitCatalog catalog)
        {
            var cast = new HashSet<string>(StringComparer.Ordinal);
            for (var n = start; n < lines.Length; n++)
            {
                var trimmed = lines[n].Trim();
                if (IsLabel(trimmed))
                {
                    break;
                }
                if (!EventPlacementParser.TrySplitWithSpans(lines[n], out var spans) || spans.Count == 0)
                {
                    continue;
                }
                var tokens = spans.Select(s => EventPlacementParser.Unquote(s.Value)).ToList();
                if (Is(tokens[0], "Create") && tokens.Count >= 5)
                {
                    Add(tokens[4]);
                    Add(tokens[2]);
                }
                else if (Is(tokens[0], "Launch") && tokens.Count >= 4 && !TryInt(tokens[1], out _))
                {
                    Add(tokens[1]);
                }
            }
            return cast;

            void Add(string name)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return;
                }
                cast.Add(name);
                var nick = catalog.CanonicalPilot(name);
                if (!string.IsNullOrEmpty(nick))
                {
                    cast.Add(nick);
                }
            }
        }

        private static (bool inTalk, string speaker) TalkStateAfter(List<string> lines, int after)
        {
            var inTalk = false;
            var speaker = "";
            for (var n = 0; n < after; n++)
            {
                var trimmed = lines[n].Trim();
                if (IsSkippable(trimmed))
                {
                    continue;
                }
                if (inTalk)
                {
                    if (IsEnd(trimmed))
                    {
                        inTalk = false;
                    }
                    else if (IsTalkStart(trimmed))
                    {
                        speaker = TalkSpeaker(trimmed);
                    }
                    continue;
                }
                if (IsTalkStart(trimmed))
                {
                    inTalk = true;
                    speaker = TalkSpeaker(trimmed);
                }
            }
            return (inTalk, speaker);
        }

        private static int NextSignificant(List<string> lines, int from)
        {
            for (var n = from; n < lines.Count; n++)
            {
                if (!IsSkippable(lines[n].Trim()))
                {
                    return n;
                }
            }
            return -1;
        }

        private static string IndentOf(List<string> lines, int after)
        {
            for (var n = Math.Min(after, lines.Count) - 1; n >= 0; n--)
            {
                var line = lines[n];
                var trimmed = line.Trim();
                if (trimmed.Length == 0 || IsLabel(trimmed))
                {
                    continue;
                }
                return line.Substring(0, line.Length - line.TrimStart().Length);
            }
            return "";
        }

        private static string TalkSpeaker(string trimmed)
        {
            if (!EventPlacementParser.TrySplitWithSpans(trimmed, out var spans) || spans.Count < 2)
            {
                return "";
            }
            return EventPlacementParser.Unquote(spans[1].Value);
        }

        private static List<string> ArgsWithoutOptions(List<string> tokens)
            => tokens.Where((t, i) => i == 0 || !new[] { "非同期", "アニメ表示", "アニメ非表示" }.Any(o => EventPlacementParser.Unquote(t).StartsWith(o, StringComparison.Ordinal))).ToList();

        private static string NormalizeParty(string party) => party switch
        {
            "NPC" or "ＮＰＣ" => "ＮＰＣ",
            _ => party,
        };

        private static string Quote(string name)
        {
            name ??= "";
            return name.Any(c => char.IsWhiteSpace(c) || c == ',' || c == '(' || c == ')') ? "\"" + name.Replace("\"", "") + "\"" : name;
        }

        private static bool TryInt(string token, out int value) => int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);

        private static string FirstToken(string trimmed)
        {
            var i = trimmed.IndexOfAny(new[] { ' ', '\t' });
            return i < 0 ? trimmed : trimmed.Substring(0, i);
        }

        private static bool Is(string token, string command) => string.Equals(token, command, StringComparison.OrdinalIgnoreCase);

        private static bool IsTalkStart(string trimmed)
        {
            var cmd = FirstToken(trimmed);
            return Is(cmd, "Talk") || Is(cmd, "AutoTalk");
        }

        private static bool IsEnd(string trimmed) => Is(trimmed, "End") || Is(trimmed, "Suspend");

        private static bool IsLabel(string trimmed) => trimmed.Length > 1 && trimmed.EndsWith(":", StringComparison.Ordinal) && !trimmed.StartsWith("#", StringComparison.Ordinal);

        private static bool IsSkippable(string trimmed) => trimmed.Length == 0 || trimmed.StartsWith("#", StringComparison.Ordinal) || trimmed.StartsWith("//", StringComparison.Ordinal);

        private static string[] SplitLines(string text) => (text ?? "").Replace("\r\n", "\n").Split('\n');

        private static void WriteOptional(Utf8JsonWriter w, string name, int? value)
        {
            if (value.HasValue)
            {
                w.WriteNumber(name, value.Value);
            }
        }
    }
}
