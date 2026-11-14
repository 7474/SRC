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
        public static IReadOnlyDictionary<string, string> Files(string title, string appPath = null, string episode = DefaultEpisode)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                title = "新しいシナリオ";
            }
            var files = new Dictionary<string, string>
            {
                [episode + ".eve"] = Event(title, episode),
                [episode + ".map"] = Map(),
                [episode + ".play"] = Play(episode),
                ["Data/robot.txt"] = Units(),
                ["Data/pilot.txt"] = Pilots(),
                ["README.md"] = Readme(title, episode),
                [".vscode/launch.json"] = Launch(episode),
                [".github/workflows/src-check.yml"] = Workflow(episode),
                [".gitignore"] = Lines("# SRC のセーブデータ・ログ", "*.src", "Save/", "Logs/", "srcsharp-logs/"),
            };
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
        public static IReadOnlyList<string> Write(string folder, string title, string appPath, bool overwrite, out IReadOnlyList<string> skipped)
        {
            var written = new List<string>();
            var notWritten = new List<string>();
            foreach (var (path, text) in Files(title, appPath))
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

        private static string Lines(params string[] lines) => string.Join("\r\n", lines) + "\r\n";

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

        private static string Launch(string episode) => Lines(
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

        private static string Workflow(string episode) => Lines(
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
    }
}
