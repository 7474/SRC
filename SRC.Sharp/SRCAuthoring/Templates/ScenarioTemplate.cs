using SRCAuthoring.Maps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCAuthoring.Templates
{
    /// <summary>
    /// 新しいシナリオのひな形（srcsharp new・VS Code の「新しいシナリオ」。7474/SRC#1640）。
    /// SRC# の最小の汎用データ（SRC.Sharp/Content/basic-data）か SRC のフルパッケージの汎用データで動く 1 話と、
    /// VS Code の設定（デバッガの構成）、自動プレイの手順（.play）、作者のリポジトリで動かす CI（GitHub Actions）を作る。
    /// ファイルは UTF-8（BOM なし）、改行は CRLF（元の SRC・MapEdit と同じ）。
    /// </summary>
    public static class ScenarioTemplate
    {
        /// <summary>最初の話のイベントファイルの名前（拡張子なし）</summary>
        public const string DefaultEpisode = "第1話";

        /// <summary>作るファイル（シナリオのフォルダからの相対パスと中身）</summary>
        public static IReadOnlyDictionary<string, string> Files(string title, string appPath = null, string episode = DefaultEpisode, int episodes = 1)
        {
            if (episodes < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(episodes), "話数は 1 以上にしてください");
            }
            if (string.IsNullOrWhiteSpace(title))
            {
                title = "新しいシナリオ";
            }
            Dictionary<string, string> files;
            if (episodes == 1)
            {
                files = new Dictionary<string, string>
                {
                    [episode + ".eve"] = Event(title, episode),
                    [episode + ".map"] = Map(),
                    [episode + ".play"] = Play(episode),
                    ["Data/robot.txt"] = Units(),
                    ["Data/pilot.txt"] = Pilots(),
                    ["README.md"] = Readme(title, episode),
                    [".vscode/launch.json"] = Launch(episode),
                    [".github/workflows/src-check.yml"] = Workflow(episode),
                    [".gitignore"] = Lines("# SRC のセーブデータ・ログ", "*.src", "Save/", "Logs/", "srcsharp-logs/", "# 試し戦闘（srcsharp trial・VS Code の「試し戦闘」が作る）", ".srcsharp/"),
                };
            }
            else
            {
                var names = Enumerable.Range(1, episodes).Select(i => $"第{i}話").ToList();
                files = new Dictionary<string, string>
                {
                    ["Lib/include.eve"] = MultiInclude(title),
                    ["Lib/作戦会議.eve"] = IntermissionBriefing(),
                    ["全話.play"] = CampaignPlay(episodes),
                    ["Data/robot.txt"] = Units(),
                    ["Data/pilot.txt"] = Pilots(),
                    ["README.md"] = MultiReadme(title, names),
                    [".vscode/launch.json"] = MultiLaunch(names),
                    [".github/workflows/src-check.yml"] = MultiWorkflow(names),
                    [".gitignore"] = Lines("# SRC のセーブデータ・ログ", "*.src", "*.srcq", "Save/", "Logs/", "srcsharp-logs/", "# 試し戦闘（srcsharp trial・VS Code の「試し戦闘」が作る）", ".srcsharp/"),
                };
                for (var i = 1; i <= episodes; i++)
                {
                    var ep = names[i - 1];
                    var next = i < episodes ? names[i] : null;
                    files[ep + ".eve"] = MultiEvent(title, ep, i, episodes, next);
                    files[ep + ".map"] = Map();
                    files[ep + ".play"] = MultiEpisodePlay(ep, i == episodes);
                }
            }
            if (!string.IsNullOrEmpty(appPath))
            {
                files[".vscode/settings.json"] = Lines("{", $"  \"srcsharp.appPath\": {Json(appPath)}", "}");
            }
            return files;
        }

        /// <summary>
        /// フォルダに書く。既にあるファイルは上書きしない（overwrite で上書き）。書いたファイルの相対パスを返す。
        /// 書かなかった（既にある）ファイルは skipped に入れる
        /// </summary>
        public static IReadOnlyList<string> Write(string folder, string title, string appPath, bool overwrite, out IReadOnlyList<string> skipped, int episodes = 1)
        {
            var written = new List<string>();
            var notWritten = new List<string>();
            foreach (var (path, text) in Files(title, appPath, DefaultEpisode, episodes))
            {
                var full = Path.Combine(folder, path.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(full) && !overwrite)
                {
                    notWritten.Add(path);
                    continue;
                }
                Directory.CreateDirectory(Path.GetDirectoryName(full));
                File.WriteAllText(full, text, new UTF8Encoding(false));
                written.Add(path);
            }
            skipped = notWritten;
            return written;
        }

        internal static string Lines(params string[] lines) => string.Join("\r\n", lines) + "\r\n";

        private static string Json(string s) => "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";

        private static string Event(string title, string episode) => Lines(
            $"# {title}　{episode}",
            "# SRC# のシナリオのひな形（srcsharp new）。書き方はヘルプの「SRC# でシナリオ・データを作る」と「イベントデータ」",
            "# VS Code なら、このファイルを開いて F5（デバッグ）で、画面付きで動かせる",
            "",
            "# 最初に一度だけ動くイベント（マップを出す前）",
            "プロローグ:",
            "Talk システム",
            $"「{title}」のはじまり。",
            "End",
            "Exit",
            "",
            "# マップを出し、ユニットを置く",
            "スタート:",
            $"ChangeMap {episode}.map",
            "Create 味方 見習い機 0 主人公 1 4 8",
            "Create 敵 偵察機 0 敵兵(ザコ) 1 14 6",
            "Create 敵 偵察機 0 敵兵(ザコ) 1 15 10",
            "Talk 主人公",
            "敵の偵察機だ。片付けよう！",
            "End",
            "Exit",
            "",
            "# 敵が全滅したら勝ち",
            "全滅 敵:",
            "Talk システム",
            "勝利！",
            "End",
            "GameClear",
            "",
            "# 味方が全滅したら負け",
            "全滅 味方:",
            "GameOver");

        private static string MultiInclude(string title) => Lines(
            $"# {title} 全話共通のイベント",
            "# シナリオのフォルダの Lib/include.eve は、どの話のイベントを読み込むときも自動で取り込まれる",
            "",
            "#---------------------------------------------------------------",
            "# 敗北（味方の全滅）",
            "#---------------------------------------------------------------",
            "全滅 味方:",
            "Call 敗北",
            "Exit",
            "",
            "敗北:",
            "StopBGM",
            "Talk システム",
            "部隊は撤退した……。",
            "End",
            "Ask どうする？",
            "クイックセーブした所からやり直す",
            "あきらめる",
            "End",
            "If 選択 = 1 Then",
            "  QuickLoad",
            "EndIf",
            "GameOver",
            "Return",
            "",
            "#---------------------------------------------------------------",
            "# 章の題： Call 章題 \"第1話\"",
            "#---------------------------------------------------------------",
            "章題:",
            "FadeOut 8",
            "Telop $(Args(1))",
            "FadeIn 8",
            "Return",
            "",
            "#---------------------------------------------------------------",
            "# 報酬： Call 報酬 金額 理由",
            "#---------------------------------------------------------------",
            "報酬:",
            "Local 額",
            "額 = Args(1)",
            "Money 額",
            "Talk システム",
            "$(Args(2))の報酬として、資金$(額)を受け取った。",
            "End",
            "Return",
            "",
            "#---------------------------------------------------------------",
            "# 単体で始めたときの部隊： Call 単体の部隊 話数",
            "# 前の話のデータが無いとき（第2話以降をファイル単体で開いて始めたとき）に呼ぶ",
            "#---------------------------------------------------------------",
            "単体の部隊:",
            "Option アイテム交換",
            "If Args(1) >= 2 Then",
            "  Option 機体改造",
            "EndIf",
            @"IntermissionCommand 作戦会議 Lib\作戦会議.eve",
            "Unit 見習い機 0",
            "Pilot 主人公 (2 * Args(1) - 1)",
            "Ride 主人公",
            "Return");

        private static string IntermissionBriefing() => Lines(
            "# インターミッションのメニュー「作戦会議」から呼ばれるイベント",
            "# 終わりに引数の無い Continue を置くとインターミッションに戻る",
            "",
            "プロローグ:",
            "Talk 主人公",
            "次の戦いに備えて、機体や装備を整えておこう。",
            "End",
            "Continue",
            "",
            "スタート:",
            "Exit");

        private static string MultiEvent(string title, string episode, int index, int total, string nextEpisode)
        {
            var lines = new List<string>
            {
                $"# {title}　{episode}" + (index == total ? "（最終話）" : ""),
                "# SRC# の複数話のシナリオのひな形（srcsharp new --episodes）。各話を単体でも、第1話から通しでも動かせる",
                "",
                "#===============================================================",
                "# プロローグ",
                "#===============================================================",
                "プロローグ:",
            };
            if (index == 1)
            {
                lines.Add("Option アイテム交換");
                lines.Add(@"IntermissionCommand 作戦会議 Lib\作戦会議.eve");
                lines.Add("Talk システム");
                lines.Add($"「{title}」のはじまり。");
                lines.Add("End");
            }
            else
            {
                lines.Add("Talk 主人公");
                lines.Add(index == total ? $"{episode}（最終話）。いよいよ決戦だ！" : $"{episode}。新たな敵が現れた。");
                lines.Add("End");
            }
            lines.Add("Exit");
            lines.Add("");
            lines.Add("#===============================================================");
            lines.Add("# スタート");
            lines.Add("#===============================================================");
            lines.Add("スタート:");
            if (index > 1)
            {
                lines.Add($"# 第{index}話から単体で始めたとき（前の話のデータが無いとき）は、仮の部隊を作る");
                lines.Add("If IsDefined(主人公, パイロット) = 0 Then");
                lines.Add($"  Call 単体の部隊 {index}");
                lines.Add("EndIf");
            }
            lines.Add($"Call 章題 \"{episode}\"");
            lines.Add($"ChangeMap {episode}.map");
            if (index == 1)
            {
                lines.Add("Create 味方 見習い機 0 主人公 1 4 8");
            }
            else
            {
                lines.Add("Launch 主人公 4 8");
            }
            lines.Add($"Create 敵 偵察機 0 敵兵(ザコ) {index} 14 6");
            lines.Add($"Create 敵 偵察機 0 敵兵(ザコ) {index} 15 10");
            lines.Add("Talk 主人公");
            lines.Add(index == total ? "これが最後の戦いだ。負けるわけにはいかない！" : "敵の偵察機だ。片付けよう！");
            lines.Add("End");
            lines.Add("Exit");
            lines.Add("");
            lines.Add("#===============================================================");
            lines.Add("# 勝利");
            lines.Add("#===============================================================");
            lines.Add("全滅 敵:");
            lines.Add("Talk システム");
            lines.Add($"{episode}クリア！");
            if (nextEpisode == null)
            {
                lines.Add($"「{title}」をクリアしました！");
                lines.Add("End");
                lines.Add("GameClear");
            }
            else
            {
                lines.Add("End");
                lines.Add($"Call 報酬 {1000 * index} {episode}");
                if (index == 1)
                {
                    lines.Add("Option 機体改造");
                }
                lines.Add($"Continue {nextEpisode}.eve");
                lines.Add("");
                lines.Add("#===============================================================");
                lines.Add("# エピローグ（Continue でインターミッションに入る前に動く）");
                lines.Add("#===============================================================");
                lines.Add("エピローグ:");
                lines.Add("Talk 主人公");
                lines.Add($"次は {nextEpisode} だ。インターミッションで機体を整えておこう。");
                lines.Add("End");
                lines.Add("Exit");
            }
            return Lines(lines.ToArray());
        }

        /// <summary>20×15 の平地に、林と川（最小の汎用データの地形の番号）</summary>
        private static string Map()
        {
            var doc = new MapDocument(20, 15, new MapTile(0, 0));
            for (var y = 1; y <= 15; y++)
            {
                doc[10, y] = new MapTile(19, 0);
            }
            doc[10, 8] = new MapTile(25, 0);
            foreach (var (x, y) in new[] { (5, 3), (6, 3), (5, 4), (13, 12), (14, 12), (14, 13), (17, 4) })
            {
                doc[x, y] = new MapTile(11, 0);
            }
            return doc.Serialize();
        }

        private static string Play(string episode) => Lines(
            $"# {episode} を自動で最後まで遊ぶ手順（srcsharp play の PlayScript。書式は docs/gui/e2e-testing.md）",
            $"#   srcsharp play {episode}.eve --script {episode}.play --app-path <SRC のフォルダか最小の汎用データ> --skip-wait",
            "# 味方フェイズを自動で動かし（300 手まで）、20 ターンまでに勝つ",
            "auto 300",
            "turns 20",
            "expect message 勝利",
            "expect finished",
            "expect error none");

        private static string MultiEpisodePlay(string episode, bool isFinal) => isFinal
            ? Lines(
                $"# {episode}（最終話）を単体で最後まで遊ぶ手順",
                $"#   srcsharp play {episode}.eve --script {episode}.play --app-path <SRC のフォルダか最小の汎用データ> --skip-wait",
                "choose どうする あきらめる",
                "auto 300",
                "turns 20",
                "expect message クリアしました",
                "expect finished",
                "expect error none")
            : Lines(
                $"# {episode} を単体でクリア後のインターミッションまで遊ぶ手順",
                $"#   srcsharp play {episode}.eve --script {episode}.play --app-path <SRC のフォルダか最小の汎用データ> --skip-wait",
                "choose どうする あきらめる",
                "holdlist コマンド",
                "auto 300",
                "turns 20",
                $"expect message {episode}クリア！",
                "expect error none");

        private static string CampaignPlay(int episodes) => Lines(
            $"# 第1話から第{episodes}話（最終話）まで通しで最後まで遊ぶ手順",
            $"#   srcsharp play 第1話.eve --script 全話.play --app-path <SRC のフォルダか最小の汎用データ> --skip-wait",
            "choose どうする あきらめる",
            "choose コマンド 次のステージへ",
            "auto 300",
            $"turns {20 * episodes}",
            "expect message クリアしました",
            "expect finished",
            "expect error none");

        private static string Units() => Lines(
            "# ユニットデータ。書式はヘルプの「ユニットデータ」。画像は SRC の汎用データ（最小の汎用データにもある）の名前",
            "見習い機",
            "見習い機, みならいき, 汎用, 1, 2",
            "陸, 5, M, 2000, 120",
            "特殊能力なし",
            "3000, 120, 900, 80",
            "AABA, OBJ_Robot01.bmp",
            "ブレード, 1400, 1, 1, +15, -, -, -, AAAA, +10, 武",
            "ライフル, 1200, 1, 4, +10, 10, -, -, AAAA, +0, 実",
            "===",
            "",
            "偵察機",
            "偵察機, ていさつき, 汎用, 1, 2",
            "陸, 4, M, 1500, 80",
            "特殊能力なし",
            "1600, 60, 600, 60",
            "AABA, OBJ_Robot02.bmp",
            "マシンガン, 900, 1, 2, +0, 8, -, -, AAAA, +0, 実",
            "===");

        private static string Pilots() => Lines(
            "# パイロットデータ。書式はヘルプの「パイロットデータ」",
            "主人公",
            "主人公, しゅじんこう, 男性, 汎用, AABA, 120",
            "特殊能力なし",
            "150, 150, 150, 150, 160, 150, 強気",
            "ＳＰ, 40, 集中, 1, 必中, 5, 熱血, 10",
            "-.bmp, -.mid",
            "",
            "敵兵(ザコ)",
            "敵兵, てきへい, 男性, 汎用, AABA, 60",
            "特殊能力なし",
            "110, 110, 110, 110, 120, 110, 普通",
            "ＳＰ, 0",
            "-.bmp, -.mid");

        private static string Readme(string title, string episode) => Lines(
            $"# {title}",
            "",
            "SRC# のシナリオのひな形（`srcsharp new`）から作ったシナリオです。",
            "",
            "| ファイル | 内容 |",
            "|---|---|",
            $"| `{episode}.eve` | イベント（最初の話） |",
            $"| `{episode}.map` | マップ。VS Code で開くか、[SRC#Studio](https://7474.github.io/SRC/studio/) のマップ作成ツールで描く |",
            "| `Data/robot.txt`・`Data/pilot.txt` | ユニット・パイロットのデータ |",
            $"| `{episode}.play` | 最後まで自動で遊ぶ手順（`srcsharp play`）。CI で動かす |",
            "| `.vscode/launch.json` | VS Code で F5 を押すと、開いているイベントを画面付きで動かす |",
            "| `.github/workflows/src-check.yml` | GitHub に置くと、push のたびにデータを検査し、最後まで自動で遊べるかを確かめる |",
            "",
            "## 動かす",
            "",
            "SRC の汎用データ（`Data/System/terrain.txt` などのある SRC のフォルダ）か、SRC# の最小の汎用データ（https://github.com/7474/SRC の `SRC.Sharp/Content/basic-data`）が要ります。",
            "",
            "- VS Code: 拡張「SRC# Authoring」を入れ、設定 `srcsharp.appPath` に SRC のフォルダを書き、イベントを開いて F5",
            "- SRC#Avalonia・SRC#Web: このフォルダを開いて遊ぶ",
            $"- コマンドライン: `srcsharp play {episode}.eve --script {episode}.play --app-path <SRC のフォルダ> --skip-wait`",
            "",
            "作り方の流れは、SRC# のヘルプの「SRC# でシナリオ・データを作る」を見てください。");

        private static string MultiReadme(string title, IReadOnlyList<string> episodes)
        {
            var lines = new List<string>
            {
                $"# {title}",
                "",
                $"SRC# のシナリオのひな形（`srcsharp new --episodes {episodes.Count}`）から作った全 {episodes.Count} 話のシナリオです。",
                "",
                "| ファイル | 内容 |",
                "|---|---|",
                "| `Lib/include.eve` | 全話共通のイベント（敗北・章題・報酬・単体で始めたときの部隊） |",
                "| `Lib/作戦会議.eve` | インターミッションのコマンド「作戦会議」のイベント |",
            };
            foreach (var ep in episodes)
            {
                lines.Add($"| `{ep}.eve`・`{ep}.map`・`{ep}.play` | {ep} のイベント・マップ・単体自動プレイの手順 |");
            }
            lines.Add("| `全話.play` | 第1話から最終話まで通しで自動で遊ぶ手順 |");
            lines.Add("| `Data/robot.txt`・`Data/pilot.txt` | ユニット・パイロットのデータ |");
            lines.Add("| `.vscode/launch.json` | VS Code で F5 を押すと、開いているイベントを画面付きで動かす |");
            lines.Add("| `.github/workflows/src-check.yml` | GitHub に置くと、push のたびにデータ検査・話のつながり確認・自動プレイを動かす |");
            lines.Add("");
            lines.Add("## 動かす");
            lines.Add("");
            lines.Add("SRC の汎用データ（`Data/System/terrain.txt` などのある SRC のフォルダ）か、SRC# の最小の汎用データ（https://github.com/7474/SRC の `SRC.Sharp/Content/basic-data`）が要ります。");
            lines.Add("");
            lines.Add("- VS Code: 拡張「SRC# Authoring」を入れ、設定 `srcsharp.appPath` に SRC のフォルダを書き、イベントを開いて F5");
            lines.Add("- 話のつながりの図: VS Code のコマンド「SRC#: 話のつながりの図を表示」、または `srcsharp flow .`");
            lines.Add("- 通しで自動プレイ: `srcsharp play 第1話.eve --script 全話.play --app-path <SRC のフォルダ> --skip-wait`");
            lines.Add($"- 各話を単体で自動プレイ: `srcsharp play {episodes[1]}.eve --script {episodes[1]}.play --app-path <SRC のフォルダ> --skip-wait`");
            lines.Add("");
            lines.Add("作り方の流れは、SRC# のヘルプの「SRC# でシナリオ・データを作る」を見てください。");
            return Lines(lines.ToArray());
        }

        internal static string Launch(string episode) => Lines(
            "{",
            "  \"version\": \"0.2.0\",",
            "  \"configurations\": [",
            "    {",
            "      \"type\": \"srcsharp\",",
            "      \"request\": \"launch\",",
            "      \"name\": \"開いているイベントを遊ぶ（画面付き）\",",
            "      \"program\": \"${file}\",",
            "      \"interactive\": true",
            "    },",
            "    {",
            "      \"type\": \"srcsharp\",",
            "      \"request\": \"launch\",",
            $"      \"name\": \"{episode} を自動で最後まで\",",
            $"      \"program\": \"${{workspaceFolder}}/{episode}.eve\",",
            $"      \"script\": \"${{workspaceFolder}}/{episode}.play\",",
            "      \"skipWait\": true",
            "    }",
            "  ]",
            "}");

        internal static string MultiLaunch(IReadOnlyList<string> episodes)
        {
            var configs = new List<string>
            {
                string.Join("\r\n",
                    "    {",
                    "      \"type\": \"srcsharp\",",
                    "      \"request\": \"launch\",",
                    "      \"name\": \"開いているイベントを遊ぶ（画面付き）\",",
                    "      \"program\": \"${file}\",",
                    "      \"interactive\": true",
                    "    }"),
                string.Join("\r\n",
                    "    {",
                    "      \"type\": \"srcsharp\",",
                    "      \"request\": \"launch\",",
                    "      \"name\": \"全話を自動で通しプレイ\",",
                    "      \"program\": \"${workspaceFolder}/第1話.eve\",",
                    "      \"script\": \"${workspaceFolder}/全話.play\",",
                    "      \"skipWait\": true",
                    "    }"),
            };
            foreach (var ep in episodes)
            {
                configs.Add(string.Join("\r\n",
                    "    {",
                    "      \"type\": \"srcsharp\",",
                    "      \"request\": \"launch\",",
                    $"      \"name\": \"{ep} を自動でプレイ（単体）\",",
                    $"      \"program\": \"${{workspaceFolder}}/{ep}.eve\",",
                    $"      \"script\": \"${{workspaceFolder}}/{ep}.play\",",
                    "      \"skipWait\": true",
                    "    }"));
            }
            return Lines(
                "{",
                "  \"version\": \"0.2.0\",",
                "  \"configurations\": [",
                string.Join(",\r\n", configs),
                "  ]",
                "}");
        }

        internal static string Workflow(string episode) => Lines(
            "# SRC# のデータの検査と、最後まで自動で遊べるかの確かめ（srcsharp new のひな形）",
            "# 汎用データは SRC# の最小の汎用データ（7474/SRC の SRC.Sharp/Content/basic-data）を使う。",
            "# SRC のフルパッケージの汎用データを使うシナリオは、その置き場所に合わせて --app-path を変える",
            "name: SRC check",
            "",
            "on:",
            "  push:",
            "  pull_request:",
            "",
            "permissions:",
            "  contents: read",
            "  security-events: write",
            "",
            "jobs:",
            "  check:",
            "    runs-on: ubuntu-latest",
            "    steps:",
            "      - uses: actions/checkout@v4",
            "",
            "      - uses: actions/setup-dotnet@v4",
            "        with:",
            "          dotnet-version: '10.0.x'",
            "",
            "      - name: Install srcsharp",
            "        run: dotnet tool install -g SRCSharpTools",
            "",
            "      - name: Get SRC# minimal basic data",
            "        uses: actions/checkout@v4",
            "        with:",
            "          repository: 7474/SRC",
            "          path: .srcsharp",
            "          sparse-checkout: SRC.Sharp/Content/basic-data",
            "",
            "      # 汎用データをシナリオのフォルダの外に移す（lint の対象にしない）",
            "      - name: Move basic data",
            "        run: mv .srcsharp \"$RUNNER_TEMP/srcsharp\"",
            "",
            "      # データとイベントの検査。結果は GitHub のコードスキャン（Security タブ）とプルリクエストの注釈に出る",
            "      - name: Lint",
            "        run: srcsharp lint . --app-path \"$RUNNER_TEMP/srcsharp/SRC.Sharp/Content/basic-data\" --format sarif --output srcsharp.sarif || true",
            "",
            "      - uses: github/codeql-action/upload-sarif@v3",
            "        with:",
            "          sarif_file: srcsharp.sarif",
            "          category: srcsharp",
            "",
            "      - name: Lint (fail on errors)",
            "        run: srcsharp lint . --app-path \"$RUNNER_TEMP/srcsharp/SRC.Sharp/Content/basic-data\"",
            "",
            $"      - name: Play {episode} to the end",
            $"        run: srcsharp play \"{episode}.eve\" --script \"{episode}.play\" --app-path \"$RUNNER_TEMP/srcsharp/SRC.Sharp/Content/basic-data\" --skip-wait --seed 1");

        internal static string MultiWorkflow(IReadOnlyList<string> episodes)
        {
            var steps = new List<string>
            {
                Workflow(episodes[0]).TrimEnd('\r', '\n'),
                "",
                "      - name: Play all episodes through",
                "        run: srcsharp play \"第1話.eve\" --script \"全話.play\" --app-path \"$RUNNER_TEMP/srcsharp/SRC.Sharp/Content/basic-data\" --skip-wait --seed 1",
            };
            foreach (var ep in episodes.Skip(1))
            {
                steps.Add("");
                steps.Add($"      - name: Play {ep} standalone");
                steps.Add($"        run: srcsharp play \"{ep}.eve\" --script \"{ep}.play\" --app-path \"$RUNNER_TEMP/srcsharp/SRC.Sharp/Content/basic-data\" --skip-wait --seed 1");
            }
            return string.Join("\r\n", steps) + "\r\n";
        }
    }
}
