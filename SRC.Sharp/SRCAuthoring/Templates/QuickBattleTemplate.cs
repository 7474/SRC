using SRCAuthoring.Maps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SRCAuthoring.Templates
{
    /// <summary>かんたん作成で置くユニット（ユニット・パイロット・レベル・ランク・ボスランク・位置）</summary>
    public sealed class QuickUnit
    {
        public string Unit { get; set; }
        public string Pilot { get; set; }
        public int Level { get; set; } = 1;
        /// <summary>ユニットのランク（Create の改造段階。0 から）</summary>
        public int Rank { get; set; }
        /// <summary>ボスランク（BossRank。-1 はボスにしない、0〜5）</summary>
        public int BossRank { get; set; } = -1;
        /// <summary>マップの位置（1 から）</summary>
        public int X { get; set; }
        public int Y { get; set; }
    }

    /// <summary>セリフの 1 行。Speaker が空ならナレーション（名前を出さない）</summary>
    public sealed class QuickLine
    {
        public string Speaker { get; set; }
        public string Text { get; set; }
    }

    /// <summary>勝利の条件（7474/SRC#1689）。Kind は annihilate（敵の全滅）・boss（Target を倒す）・survive（Turns ターン守りきる）・reach（味方か Target が X,Y に着く）</summary>
    public sealed class QuickGoal
    {
        public string Kind { get; set; } = "annihilate";
        public string Target { get; set; }
        public int Turns { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }

    /// <summary>敗北の条件（7474/SRC#1689）。Kind は annihilate（味方の全滅）・unit（Target が落ちる）・turns（Turns ターンを過ぎる）</summary>
    public sealed class QuickLoss
    {
        public string Kind { get; set; } = "annihilate";
        public string Target { get; set; }
        public int Turns { get; set; }
    }

    /// <summary>
    /// 戦闘中の出来事（7474/SRC#1689）。きっかけ（Trigger）が起きたら、1 回だけ、会話・援軍・撤退をする。
    /// Trigger は turn（Turn ターン目の Phase のフェイズの始め）・destroyed（Unit が落ちた）・attack（Unit と Target が戦う）・damaged（Unit の損傷率が Ratio % 以上）
    /// </summary>
    public sealed class QuickEvent
    {
        public string Trigger { get; set; } = "turn";
        public int Turn { get; set; } = 2;
        /// <summary>turn のときのフェイズ（味方・敵）</summary>
        public string Phase { get; set; } = "敵";
        public string Unit { get; set; }
        public string Target { get; set; }
        public int Ratio { get; set; } = 50;
        public List<QuickLine> Talk { get; set; } = new List<QuickLine>();
        /// <summary>援軍（置くユニット）</summary>
        public List<QuickUnit> Allies { get; set; } = new List<QuickUnit>();
        public List<QuickUnit> Enemies { get; set; } = new List<QuickUnit>();
        /// <summary>Unit が退く（Escape）</summary>
        public bool Retreat { get; set; }
    }

    /// <summary>かんたん作成の場面（誰が・どこで・何を言うか）</summary>
    public sealed class QuickBattle
    {
        /// <summary>シナリオの名前（ファイル名にも使う）</summary>
        public string Title { get; set; }
        /// <summary>読み込む作品のデータ（シナリオの Data の下のフォルダの名前。イベントの「@作品」）</summary>
        public List<string> DataTitles { get; set; } = new List<string>();
        public int Width { get; set; } = 20;
        public int Height { get; set; } = 15;
        /// <summary>マップ（.map のテキスト）。空なら Width×Height の平地</summary>
        public string Map { get; set; }
        public List<QuickUnit> Allies { get; set; } = new List<QuickUnit>();
        public List<QuickUnit> Enemies { get; set; } = new List<QuickUnit>();
        /// <summary>戦闘前の会話（マップを出してユニットを置いた後）</summary>
        public List<QuickLine> Opening { get; set; } = new List<QuickLine>();
        /// <summary>勝ったとき（敵の全滅）</summary>
        public List<QuickLine> Victory { get; set; } = new List<QuickLine>();
        /// <summary>負けたとき（味方の全滅）</summary>
        public List<QuickLine> Defeat { get; set; } = new List<QuickLine>();
        public QuickGoal Goal { get; set; } = new QuickGoal();
        public QuickLoss Loss { get; set; } = new QuickLoss();
        public List<QuickEvent> Events { get; set; } = new List<QuickEvent>();

        /// <summary>出来事の援軍も含めた、置くユニット（陣営と一緒に）</summary>
        public IEnumerable<(QuickUnit unit, string side)> AllUnits() =>
            Allies.Select(x => (x, "味方")).Concat(Enemies.Select(x => (x, "敵")))
                .Concat(Events.SelectMany(e => e.Allies.Select(x => (x, "味方")).Concat(e.Enemies.Select(x => (x, "敵")))));
    }

    /// <summary>
    /// かんたん作成（7474/SRC#1659）。ブラウザの SRC#Studio で選んだ作品・ユニット・配置・セリフから、
    /// 普通のイベント（.eve）とマップ（.map）を作る。データ（作品のフォルダ）は画面の側がシナリオの Data の下に置く。
    /// 作ったものは VS Code でそのまま開いて続きを書ける。
    /// </summary>
    public static class QuickBattleTemplate
    {
        public const int MaxSize = 50;
        public const int MaxRank = 20;
        public const int MaxBossRank = 5;

        /// <summary>
        /// 画面（JavaScript）から渡す JSON を読む。名前は camelCase（title・dataTitles・width・height・map・allies・enemies・opening・victory・defeat、
        /// ユニットは unit・pilot・level・rank・bossRank・x・y、セリフは speaker・text）。WebAssembly の切り詰めでも動くよう、リフレクションを使わずに読む
        /// </summary>
        public static QuickBattle FromJson(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var r = doc.RootElement;
            var b = new QuickBattle
            {
                Title = Str(r, "title"),
                Width = Int(r, "width", 20),
                Height = Int(r, "height", 15),
                Map = Str(r, "map"),
            };
            if (r.TryGetProperty("dataTitles", out var titles) && titles.ValueKind == JsonValueKind.Array)
            {
                b.DataTitles = titles.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.String).Select(x => x.GetString()).ToList();
            }
            b.Allies = Units(r, "allies");
            b.Enemies = Units(r, "enemies");
            b.Opening = Talks(r, "opening");
            b.Victory = Talks(r, "victory");
            b.Defeat = Talks(r, "defeat");
            if (r.TryGetProperty("goal", out var goal) && goal.ValueKind == JsonValueKind.Object)
            {
                b.Goal = new QuickGoal { Kind = Str(goal, "kind") ?? "annihilate", Target = Str(goal, "target"), Turns = Int(goal, "turns", 0), X = Int(goal, "x", 0), Y = Int(goal, "y", 0) };
            }
            if (r.TryGetProperty("loss", out var loss) && loss.ValueKind == JsonValueKind.Object)
            {
                b.Loss = new QuickLoss { Kind = Str(loss, "kind") ?? "annihilate", Target = Str(loss, "target"), Turns = Int(loss, "turns", 0) };
            }
            if (r.TryGetProperty("events", out var events) && events.ValueKind == JsonValueKind.Array)
            {
                b.Events = events.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.Object).Select(x => new QuickEvent
                {
                    Trigger = Str(x, "trigger") ?? "turn",
                    Turn = Int(x, "turn", 2),
                    Phase = Str(x, "phase") ?? "敵",
                    Unit = Str(x, "unit"),
                    Target = Str(x, "target"),
                    Ratio = Int(x, "ratio", 50),
                    Talk = Talks(x, "talk"),
                    Allies = Units(x, "allies"),
                    Enemies = Units(x, "enemies"),
                    Retreat = x.TryGetProperty("retreat", out var retreat) && retreat.ValueKind == JsonValueKind.True,
                }).ToList();
            }
            return b;
        }

        private static string Str(JsonElement e, string name) =>
            e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;

        private static int Int(JsonElement e, string name, int fallback) =>
            e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Number && v.TryGetInt32(out var n) ? n : fallback;

        private static List<QuickUnit> Units(JsonElement e, string name) =>
            e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Array
                ? v.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.Object).Select(x => new QuickUnit
                {
                    Unit = Str(x, "unit"),
                    Pilot = Str(x, "pilot"),
                    Level = Int(x, "level", 1),
                    Rank = Int(x, "rank", 0),
                    BossRank = Int(x, "bossRank", -1),
                    X = Int(x, "x", 0),
                    Y = Int(x, "y", 0),
                }).ToList()
                : new List<QuickUnit>();

        private static List<QuickLine> Talks(JsonElement e, string name) =>
            e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.Array
                ? v.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.Object).Select(x => new QuickLine
                {
                    Speaker = Str(x, "speaker"),
                    Text = Str(x, "text"),
                }).ToList()
                : new List<QuickLine>();

        /// <summary>ファイル名に使えない文字を除いたシナリオの名前（空なら「かんたん作成」）</summary>
        public static string FileName(QuickBattle b)
        {
            var invalid = "\\/:*?\"<>|".ToCharArray();
            var name = new string((b.Title ?? "").Where(c => !invalid.Contains(c) && !char.IsControl(c)).ToArray()).Trim().TrimEnd('.');
            return string.IsNullOrEmpty(name) ? "かんたん作成" : name;
        }

        /// <summary>場面が揃っているか。足りなければ理由（日本語）の一覧</summary>
        public static IReadOnlyList<string> Validate(QuickBattle b)
        {
            var errors = new List<string>();
            if (b.Width < 1 || b.Height < 1 || b.Width > MaxSize || b.Height > MaxSize)
            {
                errors.Add($"マップの大きさは 1〜{MaxSize} にしてください");
            }
            if (b.Allies.Count == 0)
            {
                errors.Add("味方のユニットがありません");
            }
            if (b.Enemies.Count == 0)
            {
                errors.Add("敵のユニットがありません");
            }
            var places = new HashSet<(int, int)>();
            var initial = new HashSet<QuickUnit>(b.Allies.Concat(b.Enemies));
            foreach (var (u, side) in b.AllUnits())
            {
                if (string.IsNullOrWhiteSpace(u.Unit) || string.IsNullOrWhiteSpace(u.Pilot))
                {
                    errors.Add($"{side}のユニットかパイロットが選ばれていません");
                    continue;
                }
                if (u.Rank < 0 || u.Rank > MaxRank)
                {
                    errors.Add($"{side}の「{u.Unit}」のランクは 0〜{MaxRank} にしてください");
                }
                if (u.BossRank < -1 || u.BossRank > MaxBossRank)
                {
                    errors.Add($"{side}の「{u.Unit}」のボスランクは 0〜{MaxBossRank}（ボスにしないなら -1）にしてください");
                }
                if (u.X < 1 || u.Y < 1 || u.X > b.Width || u.Y > b.Height)
                {
                    errors.Add($"{side}の「{u.Unit}」がマップの外にあります");
                }
                else if (initial.Contains(u) && !places.Add((u.X, u.Y)))
                {
                    errors.Add($"{side}の「{u.Unit}」がほかのユニットと同じ場所にあります");
                }
            }
            var speakers = new HashSet<string>(b.AllUnits().Select(x => x.unit.Pilot ?? ""));
            var enemies = new HashSet<string>(b.AllUnits().Where(x => x.side == "敵").Select(x => x.unit.Pilot ?? ""));
            var allies = new HashSet<string>(b.AllUnits().Where(x => x.side == "味方").Select(x => x.unit.Pilot ?? ""));
            ValidateConditions(b, errors, speakers, enemies, allies);
            foreach (var line in b.Opening.Concat(b.Victory).Concat(b.Defeat).Concat(b.Events.SelectMany(e => e.Talk)))
            {
                if (!string.IsNullOrEmpty(line.Speaker) && line.Speaker != "システム" && !speakers.Contains(line.Speaker))
                {
                    errors.Add($"セリフの「{line.Speaker}」は、置いたユニットのパイロットではありません");
                }
            }
            return errors.Distinct().ToList();
        }

        private static void ValidateConditions(QuickBattle b, List<string> errors, HashSet<string> speakers, HashSet<string> enemies, HashSet<string> allies)
        {
            switch (b.Goal?.Kind ?? "annihilate")
            {
                case "annihilate":
                    break;
                case "boss":
                    if (string.IsNullOrWhiteSpace(b.Goal.Target) || !enemies.Contains(b.Goal.Target))
                    {
                        errors.Add("勝利の条件で倒す相手は、置いた敵のパイロットから選んでください");
                    }
                    break;
                case "survive":
                    if (b.Goal.Turns < 1)
                    {
                        errors.Add("勝利の条件の守りきるターン数は 1 以上にしてください");
                    }
                    break;
                case "reach":
                    if (b.Goal.X < 1 || b.Goal.Y < 1 || b.Goal.X > b.Width || b.Goal.Y > b.Height)
                    {
                        errors.Add("勝利の条件の着くマスをマップの中で選んでください");
                    }
                    if (!string.IsNullOrWhiteSpace(b.Goal.Target) && !allies.Contains(b.Goal.Target))
                    {
                        errors.Add("勝利の条件で着くユニットは、置いた味方のパイロットから選んでください");
                    }
                    break;
                default:
                    errors.Add($"勝利の条件「{b.Goal.Kind}」は分かりません");
                    break;
            }
            switch (b.Loss?.Kind ?? "annihilate")
            {
                case "annihilate":
                    break;
                case "unit":
                    if (string.IsNullOrWhiteSpace(b.Loss.Target) || !allies.Contains(b.Loss.Target))
                    {
                        errors.Add("敗北の条件で落ちてはいけないユニットは、置いた味方のパイロットから選んでください");
                    }
                    break;
                case "turns":
                    if (b.Loss.Turns < 1)
                    {
                        errors.Add("敗北の条件のターン数は 1 以上にしてください");
                    }
                    else if (b.Goal?.Kind == "survive" && b.Goal.Turns >= b.Loss.Turns)
                    {
                        errors.Add("守りきるターン数は、敗北の条件のターン数より少なくしてください");
                    }
                    break;
                default:
                    errors.Add($"敗北の条件「{b.Loss.Kind}」は分かりません");
                    break;
            }
            var named = new HashSet<string>();
            foreach (var u in b.Allies.Concat(b.Enemies).Where(u => IsNamed(u.Pilot)))
            {
                if (!named.Add(u.Pilot))
                {
                    errors.Add($"「{u.Pilot}」は名前のあるパイロットなので、2 機には乗せられません（ザコ・汎用のパイロットなら何機でも置けます）");
                }
            }
            for (var i = 0; i < b.Events.Count; i++)
            {
                var e = b.Events[i];
                var name = $"出来事 {i + 1}";
                foreach (var u in e.Allies.Concat(e.Enemies).Where(u => IsNamed(u.Pilot)))
                {
                    // 落ちたパイロットが別の機体で出直すのはよい（乗り換えて出す）。まだ乗っている人は 2 機目に乗れない
                    if (!named.Add(u.Pilot) && !(e.Trigger == "destroyed" && e.Unit == u.Pilot))
                    {
                        errors.Add($"{name}: 「{u.Pilot}」はもう出ています。名前のあるパイロットが別の機体で出直せるのは、そのパイロットが落ちたときの出来事だけです");
                    }
                }
                switch (e.Trigger)
                {
                    case "turn":
                        if (e.Turn < 1)
                        {
                            errors.Add($"{name}: ターンは 1 以上にしてください");
                        }
                        if (e.Phase != "味方" && e.Phase != "敵")
                        {
                            errors.Add($"{name}: フェイズは味方か敵にしてください");
                        }
                        break;
                    case "destroyed":
                    case "damaged":
                    case "attack":
                        if (string.IsNullOrWhiteSpace(e.Unit) || !speakers.Contains(e.Unit))
                        {
                            errors.Add($"{name}: きっかけのユニットを、置いたユニットのパイロットから選んでください");
                        }
                        if (e.Trigger == "attack" && (string.IsNullOrWhiteSpace(e.Target) || !speakers.Contains(e.Target)))
                        {
                            errors.Add($"{name}: 戦う相手を、置いたユニットのパイロットから選んでください");
                        }
                        if (e.Trigger == "damaged" && (e.Ratio < 1 || e.Ratio > 99))
                        {
                            errors.Add($"{name}: 損傷率は 1〜99 にしてください");
                        }
                        break;
                    default:
                        errors.Add($"{name}: きっかけ「{e.Trigger}」は分かりません");
                        break;
                }
                if (e.Retreat && (e.Trigger == "destroyed" || string.IsNullOrWhiteSpace(e.Unit)))
                {
                    errors.Add($"{name}: 退くのは、落ちる前のユニットだけです（きっかけを損傷率などにしてください）");
                }
                if (e.Talk.All(l => string.IsNullOrWhiteSpace(l.Text)) && e.Allies.Count == 0 && e.Enemies.Count == 0 && !e.Retreat)
                {
                    errors.Add($"{name}: 会話・援軍・退くのどれかを足してください");
                }
            }
        }

        /// <summary>イベントラベルの塊。同じきっかけの出来事は 1 つのラベルにまとめる（同じラベルを 2 度書くと後のものは起きない）</summary>
        private sealed class Block
        {
            public string Label;
            public List<string> Notes = new List<string>();
            public List<QuickLine> Talk = new List<QuickLine>();
            public List<string> Commands = new List<string>();
            public List<QuickLine> EndTalk;
            public string End;
        }

        private static string TurnLabel(int turn, string phase) => $"ターン {turn} {phase}:";

        private static string ConditionText(QuickBattle b, out string loss)
        {
            var goal = (b.Goal?.Kind ?? "annihilate") switch
            {
                "boss" => $"{b.Goal.Target}の撃破",
                "survive" => $"{b.Goal.Turns} ターンの間、守りきる",
                "reach" => string.IsNullOrWhiteSpace(b.Goal.Target) ? $"味方が ({b.Goal.X},{b.Goal.Y}) に着く" : $"{b.Goal.Target}が ({b.Goal.X},{b.Goal.Y}) に着く",
                _ => "敵の全滅",
            };
            loss = (b.Loss?.Kind ?? "annihilate") switch
            {
                "unit" => $"味方の全滅、または{b.Loss.Target}の撃墜",
                "turns" => $"味方の全滅、または {b.Loss.Turns} ターンを過ぎる",
                _ => "味方の全滅",
            };
            return goal;
        }

        /// <summary>作る話のファイルの名前（srcsharp new のひな形と同じ。話を足すときは 第2話…）</summary>
        public const string Episode = "第1話";

        /// <summary>
        /// 作るファイル（シナリオのフォルダからの相対パスと中身）。srcsharp new のひな形と同じ形（7474/SRC#1691）:
        /// 第1話.eve・第1話.map・第1話.play（自動で最後まで遊ぶ手順）・.vscode/launch.json・.github/workflows/src-check.yml・README.md（次にやること）・.gitignore。
        /// シナリオの名前（FileName）は、zip とフォルダの名前に使う
        /// </summary>
        public static IReadOnlyDictionary<string, string> Files(QuickBattle b)
        {
            var name = FileName(b);
            return new Dictionary<string, string>
            {
                [Episode + ".eve"] = EpisodeEvent(b, name, Episode, "GameClear", new Dictionary<string, string>()),
                [Episode + ".map"] = MapText(b),
                [Episode + ".play"] = ScenarioTemplate.Lines(
                    $"# {Episode} を自動で最後まで遊ぶ手順（srcsharp play の PlayScript。書式は 7474/SRC の docs/gui/e2e-testing.md）",
                    $"#   srcsharp play {Episode}.eve --script {Episode}.play --app-path <SRC のフォルダか最小の汎用データ> --skip-wait",
                    "# 味方フェイズを自動で動かし、30 ターンまで遊んで、終わること・イベントのエラーが無いことを確かめる。",
                    "# 勝ち負けは乱数で変わるので確かめない。強さは srcsharp playtest（VS Code の「この話の強さを確かめる」）で見る",
                    "auto 400",
                    "turns 30",
                    "expect finished",
                    "expect error none"),
                ["README.md"] = Readme(name),
                [".vscode/launch.json"] = ScenarioTemplate.Launch(Episode),
                [".github/workflows/src-check.yml"] = ScenarioTemplate.Workflow(Episode),
                [".gitignore"] = Gitignore,
            };
        }

        internal static readonly string Gitignore = ScenarioTemplate.Lines("# SRC のセーブデータ・ログ", "*.src", "*.srcq", "Save/", "Logs/", "srcsharp-logs/", "# 試し戦闘・場面の再生など（VS Code・srcsharp が作る）", ".srcsharp/");

        internal static string MapText(QuickBattle b) =>
            string.IsNullOrWhiteSpace(b.Map) ? new MapDocument(b.Width, b.Height, new MapTile(0, 0)).Serialize() : b.Map;

        /// <summary>
        /// 1 話のイベント。win は勝ったときの命令（最後の話は GameClear、続く話は Continue 次の話.eve）。
        /// carried は前の話から続けて出る味方（パイロット → 前の話の機体）。Launch で出し、機体が変われば乗り換える。
        /// その話を単体で始めたとき（前の話のデータが無いとき）は、Create で作る（srcsharp new の複数話と同じ考え方）
        /// </summary>
        internal static string EpisodeEvent(QuickBattle b, string name, string episode, string win, IReadOnlyDictionary<string, string> carried)
        {
            var lines = new List<string>
            {
                $"# {name} {episode}",
                "# SRC#Studio の「かんたん作成」で作ったシナリオ。このまま VS Code で開いて続きを書ける（README.md の「次にやること」と、ヘルプの「SRC#でシナリオ・データを作る」）",
                "# 各ラベルの上の注釈（#）が、そのイベントがいつ起きて何をするか。直したいところを書き換えてよい（書き換えた後はただのイベント）",
                "",
                "# 読み込む作品のデータ（Data の下のフォルダ）",
            };
            lines.AddRange(b.DataTitles.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().Select(x => "@" + x.Trim()));
            lines.Add("");
            lines.Add("# マップを出し、ユニットを置いて、戦闘前の会話");
            lines.Add("スタート:");
            lines.Add($"ChangeMap {episode}.map");
            foreach (var u in b.Allies)
            {
                if (carried.TryGetValue(u.Pilot ?? "", out var previousUnit))
                {
                    lines.AddRange(CarriedAlly(u, previousUnit));
                }
                else
                {
                    lines.AddRange(Create("味方", u));
                }
            }
            lines.AddRange(b.Enemies.SelectMany(u => Create("敵", u)));
            lines.AddRange(Talk(b.Opening));
            lines.Add("Exit");
            lines.AddRange(EventBlocks(b, win, carried.Keys));
            return string.Join("\r\n", lines) + "\r\n";
        }

        /// <summary>前の話から続けて出る味方。前の話のデータがあれば Launch（機体が変われば Unit・Ride で乗り換える）、無ければ Create</summary>
        private static IEnumerable<string> CarriedAlly(QuickUnit u, string previousUnit)
        {
            yield return $"# {u.Pilot}は前の話から続けて出る（この話を単体で始めたときは作る）";
            yield return $"If IsDefined({Quote(u.Pilot)}, パイロット) = 0 Then";
            foreach (var line in Create("味方", u))
            {
                yield return "  " + line;
            }
            yield return "Else";
            if (!string.Equals(previousUnit, u.Unit, StringComparison.Ordinal))
            {
                yield return $"  # {previousUnit}から{u.Unit}に乗り換える";
                yield return $"  Unit {Quote(u.Unit)} {Math.Max(0, Math.Min(MaxRank, u.Rank))}";
                yield return $"  Ride {Quote(u.Pilot)}";
            }
            yield return $"  Launch {Quote(u.Pilot)} {u.X} {u.Y}";
            yield return "EndIf";
        }

        /// <summary>README.md: 中身と、次にやること（VS Code で続きを書く → 遊ぶ → 出来事・話を足す → 強さを確かめる → 公開する）</summary>
        private static string Readme(string name) => ScenarioTemplate.Lines(
            $"# {name}",
            "",
            "SRC#Studio の「かんたん作成」で作ったシナリオです。中身は `srcsharp new` のひな形と同じ形の、普通の SRC のシナリオです。",
            "",
            "| ファイル | 内容 |",
            "|---|---|",
            $"| `{Episode}.eve` | イベント。勝ち負けの条件や出来事は、注釈（`#`）付きの普通のイベントラベル |",
            $"| `{Episode}.map` | マップ |",
            "| `Data/作品/…` | 選んだ作品のデータ（GSC データパック） |",
            $"| `{Episode}.play` | 自動で最後まで遊ぶ手順（`srcsharp play`）。CI で動かす |",
            "| `.vscode/launch.json` | VS Code で F5 を押すと、開いているイベントを画面付きで動かす |",
            "| `.github/workflows/src-check.yml` | GitHub に置くと、push のたびにデータを検査し、最後まで自動で遊べるかを確かめる |",
            "",
            "## 次にやること",
            "",
            "1. **VS Code で開く**: 拡張「SRC# Authoring」を入れ、このフォルダを開く。設定 `srcsharp.appPath` に SRC のフォルダ（無ければ SRC# の最小の汎用データ。7474/SRC の `SRC.Sharp/Content/basic-data`）を書く",
            $"2. **遊ぶ**: `{Episode}.eve` を開いて F5。遊びながらイベントやデータを直して保存すると、その場で読み直す（ホットリロード）",
            $"3. **出来事を足す・直す**: `{Episode}.eve` の注釈を手がかりに、会話・援軍・勝ち負けの条件を書き足す。マップ作成ツールの「配置」「人形劇」で、ユニットの位置や、動かしながら会話する場面を見ながら書ける",
            "4. **話を足す**: `第2話.eve` を作り、`エピローグ:` で `Continue 第2話.eve` を書く（`srcsharp new --episodes 3` の形が見本。話のつながりは「SRC#: 話のつながりの図を表示」で見る）",
            "5. **強さを確かめる**: 「SRC#: この話の強さを確かめる」（`srcsharp playtest`）で、自動で何回か遊んで勝ち負け・ターン数を見る",
            "6. **公開する**: SRC#Web で遊べるように置く・zip にして配る（ヘルプの「SRC#Webの使い方」）",
            "",
            "作り方の流れは、SRC# のヘルプの「SRC#でシナリオ・データを作る」を見てください。");

        /// <summary>
        /// 勝利条件・出来事・勝ち負けのイベント。同じラベルになるものは 1 つにまとめ、会話 → 援軍 → 退く → 勝ち負けの順に書く。
        /// ラベルは VB 版の SRC と同じく 1 回だけ起きる（常時イベントにしない）
        /// </summary>
        private static IEnumerable<string> EventBlocks(QuickBattle b, string win = "GameClear", IEnumerable<string> carried = null)
        {
            var victory = b.Victory.Count > 0 ? b.Victory : new List<QuickLine> { new QuickLine { Speaker = "システム", Text = "勝利！" } };
            var blocks = new List<Block>();
            Block At(string label)
            {
                var found = blocks.FirstOrDefault(x => x.Label == label);
                if (found == null)
                {
                    found = new Block { Label = label };
                    blocks.Add(found);
                }
                return found;
            }
            void Finish(Block block, List<QuickLine> talk, string end, string note)
            {
                block.Notes.Add(note);
                block.EndTalk = talk;
                block.End = end;
            }

            // 名前のあるパイロット（ザコ・汎用でない）は 1 人だけ。落ちたあとに別の機体で出直すときは、乗り換えて出す
            var named = new HashSet<string>(b.Allies.Concat(b.Enemies).Select(u => u.Pilot).Where(IsNamed).Concat(carried ?? Enumerable.Empty<string>()));
            var goalText = ConditionText(b, out var lossText);
            var conditions = At("勝利条件:");
            conditions.Notes.Add("マップのコマンド「作戦目的」で出す");
            conditions.Talk.Add(new QuickLine { Speaker = "システム", Text = $"勝利条件：{goalText}\n敗北条件：{lossText}" });

            foreach (var e in b.Events)
            {
                string label;
                string note;
                switch (e.Trigger)
                {
                    case "destroyed":
                        label = $"破壊 {Quote(e.Unit)}:";
                        note = $"{e.Unit}が落ちたとき";
                        break;
                    case "attack":
                        label = $"攻撃 {Quote(e.Unit)} {Quote(e.Target)}:";
                        note = $"{e.Unit}と{e.Target}が戦うとき（どちらから仕掛けても。戦う前）";
                        break;
                    case "damaged":
                        label = $"損傷率 {Quote(e.Unit)} {e.Ratio}:";
                        note = $"{e.Unit}の損傷率が {e.Ratio}% 以上になったとき";
                        break;
                    default:
                        label = TurnLabel(e.Turn, e.Phase);
                        note = $"{e.Turn} ターン目の{e.Phase}のフェイズの始め";
                        break;
                }
                var block = At(label);
                var what = new List<string>();
                if (e.Talk.Any(l => !string.IsNullOrWhiteSpace(l.Text)))
                {
                    what.Add("会話");
                }
                if (e.Allies.Count + e.Enemies.Count > 0)
                {
                    what.Add("援軍");
                }
                if (e.Retreat)
                {
                    what.Add($"{e.Unit}が退く");
                }
                block.Notes.Add($"{note}: {string.Join("・", what)}");
                block.Talk.AddRange(e.Talk);
                block.Commands.AddRange(e.Allies.SelectMany(u => Reinforce("味方", u, named)));
                block.Commands.AddRange(e.Enemies.SelectMany(u => Reinforce("敵", u, named)));
                if (e.Retreat)
                {
                    block.Commands.Add($"Escape {Quote(e.Unit)}");
                    // 倒す相手が退いたら勝ち
                    if (b.Goal?.Kind == "boss" && b.Goal.Target == e.Unit)
                    {
                        Finish(block, victory, win, $"{e.Unit}が退いたら勝ち");
                    }
                }
            }

            switch (b.Goal?.Kind)
            {
                case "boss":
                    Finish(At($"破壊 {Quote(b.Goal.Target)}:"), victory, win, $"{b.Goal.Target}を倒したら勝ち");
                    break;
                case "survive":
                    Finish(At(TurnLabel(b.Goal.Turns + 1, "味方")), victory, win, $"{b.Goal.Turns} ターン守りきったら勝ち（{b.Goal.Turns + 1} ターン目の味方のフェイズの始め）");
                    break;
                case "reach":
                    var who = string.IsNullOrWhiteSpace(b.Goal.Target) ? "味方" : Quote(b.Goal.Target);
                    Finish(At($"進入 {who} {b.Goal.X} {b.Goal.Y}:"), victory, win, $"{(who == "味方" ? "味方" : b.Goal.Target)}が ({b.Goal.X},{b.Goal.Y}) に着いたら勝ち");
                    break;
            }
            switch (b.Loss?.Kind)
            {
                case "unit":
                    Finish(At($"破壊 {Quote(b.Loss.Target)}:"), b.Defeat, "GameOver", $"{b.Loss.Target}が落ちたら負け");
                    break;
                case "turns":
                    Finish(At(TurnLabel(b.Loss.Turns + 1, "味方")), b.Defeat, "GameOver", $"{b.Loss.Turns} ターンを過ぎたら負け（{b.Loss.Turns + 1} ターン目の味方のフェイズの始め）");
                    break;
            }
            // 敵の全滅はいつでも勝ち、味方の全滅はいつでも負け
            Finish(At("全滅 敵:"), victory, win, "敵が全滅したら勝ち");
            Finish(At("全滅 味方:"), b.Defeat, "GameOver", "味方が全滅したら負け");

            foreach (var block in blocks)
            {
                yield return "";
                foreach (var note in block.Notes)
                {
                    yield return "# " + note;
                }
                yield return block.Label;
                foreach (var line in Talk(block.Talk))
                {
                    yield return line;
                }
                foreach (var line in block.Commands)
                {
                    yield return line;
                }
                if (block.End != null)
                {
                    foreach (var line in Talk(block.EndTalk ?? new List<QuickLine>()))
                    {
                        yield return line;
                    }
                    yield return block.End;
                }
                else
                {
                    yield return "Exit";
                }
            }
        }

        /// <summary>名前のあるパイロットか（ザコ・汎用は同じ名前で何人でも作れる。名前のあるパイロットは 1 人だけで、2 度目の Create はエラー）</summary>
        public static bool IsNamed(string pilot) =>
            !string.IsNullOrWhiteSpace(pilot) && !pilot.Contains("(ザコ)") && !pilot.Contains("(汎用)");

        /// <summary>
        /// 援軍を置く。もう出ている名前のあるパイロット（落ちたユニットのパイロット）は Create できないので、
        /// Unit で機体を作って Ride で乗せ、ChangeParty で陣営を戻して（Unit の機体は味方）、Launch で出す（VB 版と同じ書き方）
        /// </summary>
        private static IEnumerable<string> Reinforce(string side, QuickUnit u, HashSet<string> named)
        {
            if (!IsNamed(u.Pilot) || named.Add(u.Pilot))
            {
                foreach (var line in Create(side, u))
                {
                    yield return line;
                }
                yield break;
            }
            yield return $"# {u.Pilot}は落ちたユニットから、新しい機体に乗り換えて出る";
            yield return $"Unit {Quote(u.Unit)} {Math.Max(0, Math.Min(MaxRank, u.Rank))}";
            yield return $"Ride {Quote(u.Pilot)}";
            if (side != "味方")
            {
                yield return $"ChangeParty {Quote(u.Pilot)} {side}";
            }
            if (u.BossRank >= 0)
            {
                yield return $"BossRank {Math.Min(MaxBossRank, u.BossRank)}";
            }
            yield return $"Launch {Quote(u.Pilot)} {u.X} {u.Y}";
        }

        /// <summary>ユニットを置く。ボスなら続けて BossRank（Create で選ばれたユニットに付く）</summary>
        private static IEnumerable<string> Create(string side, QuickUnit u)
        {
            yield return $"Create {side} {Quote(u.Unit)} {Math.Max(0, Math.Min(MaxRank, u.Rank))} {Quote(u.Pilot)} {Math.Max(1, Math.Min(99, u.Level))} {u.X} {u.Y}";
            if (u.BossRank >= 0)
            {
                yield return $"BossRank {Math.Min(MaxBossRank, u.BossRank)}";
            }
        }

        /// <summary>名前を引数に書く。空白・カンマ・引用符を含む名前は "…" で囲む（"" は SRC の引数で書けないので除く）</summary>
        private static string Quote(string s)
        {
            s = (s ?? "").Trim().Replace("\"", "");
            return s.Any(c => char.IsWhiteSpace(c) || c == ',' || c == '(' || c == ')') ? "\"" + s + "\"" : s;
        }

        /// <summary>セリフを Talk の塊にする。話し手が変わるたびに Talk を書く。空なら何も書かない</summary>
        private static IEnumerable<string> Talk(IEnumerable<QuickLine> talk)
        {
            var result = new List<string>();
            string current = null;
            foreach (var line in talk)
            {
                var text = MessageLines(line.Text).ToList();
                if (text.Count == 0)
                {
                    continue;
                }
                var speaker = (line.Speaker ?? "").Trim();
                if (speaker != current)
                {
                    result.Add(speaker.Length == 0 ? "Talk" : "Talk " + Quote(speaker));
                    current = speaker;
                }
                result.AddRange(text);
            }
            if (result.Count > 0)
            {
                result.Add("End");
            }
            return result;
        }

        private static readonly string[] Keywords = { "end", "suspend", "talk", "autotalk", "exit" };

        /// <summary>
        /// セリフの文。空行を除き、イベントの命令や注釈に読まれる行（End・Talk などで始まる行・# や ' で始まる行）は、頭に全角の空白を足して文として読ませる
        /// </summary>
        private static IEnumerable<string> MessageLines(string text)
        {
            foreach (var raw in (text ?? "").Replace("\r\n", "\n").Split('\n'))
            {
                var line = raw.TrimEnd();
                if (line.Trim().Length == 0)
                {
                    continue;
                }
                var first = line.TrimStart().Split(' ', '\t')[0].ToLowerInvariant();
                if (Keywords.Contains(first) || line.TrimStart().StartsWith("#", StringComparison.Ordinal) || line.TrimStart().StartsWith("'", StringComparison.Ordinal))
                {
                    line = "　" + line;
                }
                yield return line;
            }
        }
    }
}
