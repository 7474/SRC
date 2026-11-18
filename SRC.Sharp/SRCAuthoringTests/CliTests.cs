using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SRCAuthoring.Tests
{
    /// <summary>
    /// srcsharp の各コマンド（Program.Main）を、このプロセスの中で小さなシナリオに対して動かし、終了コードと出力を確かめる（#1399）。
    /// 標準出力・標準エラーを差し替えるため、ほかのテストと並べて動かさない
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class CliTests
    {
        private static string corpus;

        [ClassInitialize]
        public static void FindCorpus(TestContext context)
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (!File.Exists(Path.Combine(dir.FullName, "SRC.Sharp.sln")))
            {
                dir = dir.Parent;
            }
            corpus = Path.Combine(dir.FullName, "E2E");
        }

        private static string Case(string name, params string[] path) => Path.Combine(new[] { corpus, name }.Concat(path).ToArray());

        /// <summary>E2E のコーパスのシナリオを一時フォルダに写す（セーブデータなどを書いてもコーパスを変えない）</summary>
        private static void CopyCase(string name, string destination)
        {
            var source = Case(name);
            foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
            {
                var target = Path.Combine(destination, Path.GetRelativePath(source, file));
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                File.Copy(file, target, true);
            }
        }

        private static (int exitCode, string stdout, string stderr) Run(params string[] args)
        {
            var stdout = new StringWriter();
            var stderr = new StringWriter();
            var savedOut = Console.Out;
            var savedError = Console.Error;
            var savedCulture = CultureInfo.CurrentUICulture;
            var savedDefault = CultureInfo.DefaultThreadCurrentUICulture;
            var savedLang = Environment.GetEnvironmentVariable(SRCSharpTools.UiLanguage.EnvironmentVariable);
            var savedSkipWait = SRCCore.Lib.GeneralLib.SkipWait;
            Environment.SetEnvironmentVariable(SRCSharpTools.UiLanguage.EnvironmentVariable, null);
            Console.SetOut(stdout);
            Console.SetError(stderr);
            try
            {
                var exitCode = SRCSharpTools.Program.Main(args).GetAwaiter().GetResult();
                return (exitCode, stdout.ToString(), stderr.ToString());
            }
            finally
            {
                Console.SetOut(savedOut);
                Console.SetError(savedError);
                CultureInfo.CurrentUICulture = savedCulture;
                CultureInfo.DefaultThreadCurrentUICulture = savedDefault;
                Environment.SetEnvironmentVariable(SRCSharpTools.UiLanguage.EnvironmentVariable, savedLang);
                SRCCore.Lib.GeneralLib.SkipWait = savedSkipWait;
            }
        }

        /// <summary>
        /// srcsharp new のひな形は、最小の汎用データで lint が通り、最後まで自動で遊べる（7474/SRC#1640）。
        /// 作者のリポジトリの CI（ひな形の .github/workflows/src-check.yml）と同じ手順
        /// </summary>
        [TestMethod]
        public void New_TemplateLintsCleanAndPlaysToTheEnd()
        {
            var folder = Path.Combine(Path.GetTempPath(), "srcs-new-" + Guid.NewGuid());
            var basicData = Path.Combine(Path.GetDirectoryName(corpus), "Content", "basic-data");
            try
            {
                var (exitCode, stdout, stderr) = Run("new", folder, "--title", "テストの物語");
                Assert.AreEqual(0, exitCode, stderr);
                StringAssert.Contains(stdout, "第1話.eve");
                StringAssert.Contains(File.ReadAllText(Path.Combine(folder, "README.md")), "# テストの物語");

                (exitCode, stdout, stderr) = Run("lint", folder, "--app-path", basicData);
                Assert.AreEqual(0, exitCode, stdout + stderr);
                // 汎用データを渡さなければ、汎用データのスペシャルパワーが無いと知らせる
                (exitCode, stdout, stderr) = Run("lint", folder);
                Assert.AreEqual(1, exitCode);
                StringAssert.Contains(stdout + stderr, "集中");

                (exitCode, stdout, stderr) = Run("play", Path.Combine(folder, "第1話.eve"), "--script", Path.Combine(folder, "第1話.play"), "--app-path", basicData, "--skip-wait", "--seed", "1");
                Assert.AreEqual(0, exitCode, stdout + stderr);

                // 試し戦闘（7474/SRC#1643・#1651）: データのユニット・パイロット・MessageType の一覧と、各焦点の試し戦闘イベント
                (exitCode, stdout, stderr) = Run("trial", folder, "--list", "--app-path", basicData);
                Assert.AreEqual(0, exitCode, stderr);
                StringAssert.Contains(stdout, "\"units\":[\"見習い機\",\"偵察機\"]");
                StringAssert.Contains(stdout, "\"messageTypes\":");
                (exitCode, stdout, stderr) = Run("trial", folder, "--unit", "偵察機", "--app-path", basicData);
                Assert.AreEqual(0, exitCode, stderr);
                var trial = stdout.Trim();
                StringAssert.EndsWith(trial, "試し戦闘.eve");
                StringAssert.Contains(File.ReadAllText(trial), "Create 味方 \"偵察機\" 0 \"主人公\" 30");
                StringAssert.Contains(File.ReadAllText(trial), "Create 敵 \"偵察機\" 0 \"敵兵(ザコ)\"");
                var trialPlay = Path.Combine(folder, "trial.play");
                File.WriteAllText(trialPlay, "auto 200\nturns 15\nexpect finished\nexpect error none\n");
                (exitCode, stdout, stderr) = Run("play", trial, "--script", trialPlay, "--app-path", basicData, "--skip-wait", "--seed", "1");
                Assert.AreEqual(0, exitCode, stdout + stderr);

                // --focus attack で隣接した瀕死の敵が生成され、エラーなく最後まで遊べる（evade / damage / destroy も有効な .eve を生成）
                (exitCode, stdout, stderr) = Run("trial", folder, "--unit", "偵察機", "--focus", "attack", "--app-path", basicData);
                Assert.AreEqual(0, exitCode, stderr);
                StringAssert.Contains(File.ReadAllText(trial), "RecoverHP 10");
                (exitCode, stdout, stderr) = Run("play", trial, "--script", trialPlay, "--app-path", basicData, "--skip-wait", "--seed", "1");
                Assert.AreEqual(0, exitCode, stdout + stderr);

                foreach (var (focus, expectedSnippet) in new[] { ("evade", "Create 味方 \"偵察機\" 10"), ("damage", "RecoverHP 70"), ("destroy", "RecoverHP 1") })
                {
                    (exitCode, stdout, stderr) = Run("trial", folder, "--unit", "偵察機", "--focus", focus, "--app-path", basicData);
                    Assert.AreEqual(0, exitCode, stderr);
                    StringAssert.Contains(File.ReadAllText(trial), expectedSnippet);
                }

                (exitCode, _, stderr) = Run("trial", folder, "--unit", "無い機体", "--app-path", basicData);
                Assert.AreEqual(1, exitCode);
                StringAssert.Contains(stderr, "無い機体");

                // 既にあるファイルは上書きしない
                File.WriteAllText(Path.Combine(folder, "第1話.eve"), "書き換えた");
                (exitCode, _, stderr) = Run("new", folder);
                Assert.AreEqual(0, exitCode);
                StringAssert.Contains(stderr, "第1話.eve");
                Assert.AreEqual("書き換えた", File.ReadAllText(Path.Combine(folder, "第1話.eve")));
            }
            finally
            {
                if (Directory.Exists(folder))
                {
                    Directory.Delete(folder, true);
                }
            }
        }

        /// <summary>
        /// 複数話のひな形（srcsharp new --episodes 3）が、通しプレイ（全話.play）と各話単体プレイの両方で通り、
        /// srcsharp flow が話のつながりの図を出力できること（7474/SRC#1652）
        /// </summary>
        [TestMethod]
        public void New_MultiEpisodeTemplateLintsPlaysAndFlows()
        {
            var folder = Path.Combine(Path.GetTempPath(), "srcs-multi-" + Guid.NewGuid());
            var basicData = Path.Combine(Path.GetDirectoryName(corpus), "Content", "basic-data");
            try
            {
                var (exitCode, _, stderr) = Run("new", folder, "--episodes", "0");
                Assert.AreEqual(2, exitCode);
                StringAssert.Contains(stderr, "1");

                string stdout;
                (exitCode, stdout, stderr) = Run("new", folder, "--title", "長編の物語", "--episodes", "3");
                Assert.AreEqual(0, exitCode, stderr);
                StringAssert.Contains(stdout, "Lib/include.eve");
                StringAssert.Contains(stdout, "Lib/作戦会議.eve");
                StringAssert.Contains(stdout, "第3話.eve");
                StringAssert.Contains(stdout, "全話.play");

                (exitCode, stdout, stderr) = Run("lint", folder, "--app-path", basicData);
                Assert.AreEqual(0, exitCode, stdout + stderr);

                // 第1話から第3話まで通しで最後まで遊ぶ
                (exitCode, stdout, stderr) = Run("play", Path.Combine(folder, "第1話.eve"), "--script", Path.Combine(folder, "全話.play"), "--app-path", basicData, "--skip-wait", "--seed", "1");
                Assert.AreEqual(0, exitCode, stdout + stderr);

                // 各話を単体で遊ぶ（第2話は 単体の部隊 を呼んで動く）
                foreach (var ep in new[] { "第1話", "第2話", "第3話" })
                {
                    (exitCode, stdout, stderr) = Run("play", Path.Combine(folder, ep + ".eve"), "--script", Path.Combine(folder, ep + ".play"), "--app-path", basicData, "--skip-wait", "--seed", "1");
                    Assert.AreEqual(0, exitCode, $"{ep} 単体失敗:\n{stdout}\n{stderr}");
                }

                // srcsharp flow の各フォーマット
                (exitCode, stdout, stderr) = Run("flow", folder);
                Assert.AreEqual(0, exitCode, stderr);
                StringAssert.Contains(stdout, "```mermaid");
                StringAssert.Contains(stdout, "第1話.eve");
                StringAssert.Contains(stdout, "第3話.eve");

                (exitCode, stdout, stderr) = Run("flow", folder, "--format", "mmd");
                Assert.AreEqual(0, exitCode, stderr);
                StringAssert.StartsWith(stdout, "flowchart TD");
                StringAssert.Contains(stdout, "Lib/作戦会議");

                var jsonOut = Path.Combine(folder, "flow.json");
                (exitCode, stdout, stderr) = Run("flow", folder, "--format", "json", "--output", jsonOut);
                Assert.AreEqual(0, exitCode, stderr);
                var json = File.ReadAllText(jsonOut);
                StringAssert.Contains(json, "\"第2話.eve\"");
                StringAssert.Contains(json, "\"warnings\": []");

                (exitCode, _, _) = Run("flow", Path.Combine(folder, "no-such-dir"));
                Assert.AreEqual(2, exitCode);
                (exitCode, _, _) = Run("flow", folder, "--format", "yaml");
                Assert.AreEqual(2, exitCode);
            }
            finally
            {
                if (Directory.Exists(folder))
                {
                    Directory.Delete(folder, true);
                }
            }
        }

        [TestMethod]
        public void Assets_ReportsStatusAndSupportsFailOnMissing()
        {
            var folder = Path.Combine(Path.GetTempPath(), "srcs-assets-cli-" + Guid.NewGuid());
            var app = Path.Combine(folder, "app");
            var scenario = Path.Combine(folder, "scenario");
            try
            {
                Directory.CreateDirectory(Path.Combine(scenario, "Data"));
                Directory.CreateDirectory(Path.Combine(scenario, "Bitmap", "Event"));
                Directory.CreateDirectory(Path.Combine(app, "Sound"));
                File.WriteAllText(Path.Combine(scenario, "ep1.eve"), "スタート:\nPaintPicture Event\\ある.bmp 0 0\nPaintPicture Event\\足りない.bmp 0 0\nPlaySound 本体の音.wav\nPaintPicture $(式).bmp 0 0\nExit\n");
                File.WriteAllText(Path.Combine(scenario, "Bitmap", "Event", "ある.bmp"), "x");
                File.WriteAllText(Path.Combine(scenario, "Bitmap", "Event", "余り.bmp"), "x");
                File.WriteAllText(Path.Combine(app, "Sound", "本体の音.wav"), "x");

                var (exitCode, stdout, stderr) = Run("assets", scenario, "--app-path", app);
                Assert.AreEqual(0, exitCode, stderr);
                StringAssert.Contains(stdout, "`Event/足りない.bmp`");
                StringAssert.Contains(stdout, "**見つからない**");
                StringAssert.Contains(stdout, "SRC 本体: `Sound/本体の音.wav`");
                StringAssert.Contains(stdout, "Bitmap/Event/余り.bmp");
                StringAssert.Contains(stderr, "見つからない 1");

                // --format readme は不足・外部素材のみ出す
                (exitCode, stdout, stderr) = Run("assets", scenario, "--app-path", app, "--format", "readme");
                Assert.AreEqual(0, exitCode, stderr);
                StringAssert.Contains(stdout, "`Event/足りない.bmp`");
                StringAssert.Contains(stdout, "`本体の音.wav`");
                Assert.IsFalse(stdout.Contains("ある.bmp"));

                // --fail-on-missing は見つからない素材があると終了コード 1（CI 用）
                (exitCode, stdout, stderr) = Run("assets", scenario, "--app-path", app, "--format", "json", "--fail-on-missing");
                Assert.AreEqual(1, exitCode, stderr);
                StringAssert.Contains(stdout, "\"missing\": 1");

                (exitCode, _, _) = Run("assets", scenario, "--format", "bad");
                Assert.AreEqual(2, exitCode);
            }
            finally
            {
                if (Directory.Exists(folder))
                {
                    Directory.Delete(folder, true);
                }
            }
        }

        /// <summary>
        /// かんたん作成（7474/SRC#1659）で作ったシナリオは、作品のデータを Data の下のフォルダに置けば、最小の汎用データで lint が通り、最後まで自動で遊べる
        /// （SRC#Studio が作る zip と同じ形）
        /// </summary>
        [TestMethod]
        public void QuickBattle_LintsCleanAndPlaysToTheEnd()
        {
            var folder = Path.Combine(Path.GetTempPath(), "srcs-quick-" + Guid.NewGuid());
            var basicData = Path.Combine(Path.GetDirectoryName(corpus), "Content", "basic-data");
            try
            {
                // 作品のデータは srcsharp new のひな形のユニット・パイロットを「テスト作品」のフォルダに置く
                var template = Path.Combine(folder, "template");
                Assert.AreEqual(0, Run("new", template).exitCode);
                var data = Path.Combine(folder, "scenario", "Data", "テスト作品");
                Directory.CreateDirectory(data);
                File.Copy(Path.Combine(template, "Data", "robot.txt"), Path.Combine(data, "robot.txt"));
                File.Copy(Path.Combine(template, "Data", "pilot.txt"), Path.Combine(data, "pilot.txt"));

                var battle = SRCAuthoring.Templates.QuickBattleTemplate.FromJson(@"{
  ""title"": ""荒野の戦い"", ""dataTitles"": [""テスト作品""], ""width"": 12, ""height"": 9,
  ""allies"": [{ ""unit"": ""見習い機"", ""pilot"": ""主人公"", ""level"": 20, ""x"": 3, ""y"": 5 }],
  ""enemies"": [{ ""unit"": ""偵察機"", ""pilot"": ""敵兵(ザコ)"", ""level"": 1, ""rank"": 1, ""bossRank"": 1, ""x"": 6, ""y"": 5 }],
  ""opening"": [{ ""speaker"": """", ""text"": ""荒野に風が吹く。"" }, { ""speaker"": ""主人公"", ""text"": ""行くぞ！"" }],
  ""victory"": [{ ""speaker"": ""主人公"", ""text"": ""やった！"" }],
  ""defeat"": [{ ""speaker"": ""主人公"", ""text"": ""くっ……"" }]
}");
                Assert.AreEqual(0, SRCAuthoring.Templates.QuickBattleTemplate.Validate(battle).Count);
                foreach (var (path, text) in SRCAuthoring.Templates.QuickBattleTemplate.Files(battle))
                {
                    File.WriteAllText(Path.Combine(folder, "scenario", path), text);
                }
                var eve = Path.Combine(folder, "scenario", "荒野の戦い.eve");

                var (exitCode, stdout, stderr) = Run("lint", Path.Combine(folder, "scenario"), "--app-path", basicData);
                Assert.AreEqual(0, exitCode, stdout + stderr);

                var play = Path.Combine(folder, "quick.play");
                File.WriteAllText(play, "auto 200\nturns 15\nexpect message やった！\nexpect finished\nexpect error none\n");
                (exitCode, stdout, stderr) = Run("play", eve, "--script", play, "--app-path", basicData, "--skip-wait", "--seed", "1");
                Assert.AreEqual(0, exitCode, stdout + stderr);
            }
            finally
            {
                if (Directory.Exists(folder))
                {
                    Directory.Delete(folder, true);
                }
            }
        }

        [TestMethod]
        public void NoArguments_PrintsUsageAndFails()
        {
            var (exitCode, stdout, _) = Run();
            Assert.AreEqual(2, exitCode);
            StringAssert.Contains(stdout, "使い方:");
        }

        [TestMethod]
        [DataRow("--help")]
        [DataRow("-h")]
        [DataRow("help")]
        public void Help_PrintsUsage(string arg)
        {
            var (exitCode, stdout, _) = Run(arg);
            Assert.AreEqual(0, exitCode);
            StringAssert.Contains(stdout, "使い方:");
        }

        [TestMethod]
        public void Version_PrintsLibraryVersion()
        {
            var (exitCode, stdout, _) = Run("--version");
            Assert.AreEqual(0, exitCode);
            // ライブラリの版（0.<マイナー>.<パッチ>）。ビルドの情報（+ 以降）は付けない
            StringAssert.Matches(stdout.Trim(), new Regex(@"^0\.\d+\.\d+(-[0-9A-Za-z.-]+)?$"));
        }

        [TestMethod]
        public void UnknownCommand_Fails()
        {
            var (exitCode, stdout, stderr) = Run("frobnicate");
            Assert.AreEqual(2, exitCode);
            Assert.AreEqual("", stdout);
            StringAssert.Contains(stderr, "不明なコマンド: frobnicate");
            StringAssert.Contains(stderr, "使い方:");
        }

        [TestMethod]
        public void UnknownLanguage_FallsBackToJapanese()
        {
            var (exitCode, _, stderr) = Run("--lang", "xx", "frobnicate");
            Assert.AreEqual(2, exitCode);
            StringAssert.Contains(stderr, "不明な言語の指定: xx");
            StringAssert.Contains(stderr, "不明なコマンド: frobnicate");
        }

        [TestMethod]
        public void Lint_CleanScenario()
        {
            using (var scenario = new TestScenario())
            {
                scenario.Write("test.eve", "スタート:", "Talk システム", "テスト", "End", "Exit");
                var (exitCode, stdout, stderr) = Run("lint", scenario.Root);
                Assert.AreEqual(0, exitCode, stderr);
                StringAssert.Matches(stdout, new Regex(@"\d+ ファイルを検査、問題 0 件"));
            }
        }

        [TestMethod]
        public void Lint_ErrorFailsAndWritesSarif()
        {
            using (var scenario = new TestScenario())
            {
                // If に対応する EndIf が無い
                scenario.Write("test.eve", "スタート:", "If 1 = 1 Then", "Exit");
                var (exitCode, stdout, stderr) = Run("lint", scenario.Root);
                Assert.AreEqual(1, exitCode, stderr);
                StringAssert.Contains(stderr, "test.eve");
                StringAssert.Matches(stdout, new Regex(@"問題 [1-9]\d* 件"));

                var sarif = Path.Combine(scenario.Root, "lint.sarif");
                (exitCode, stdout, _) = Run("lint", scenario.Root, "--format", "sarif", "--output", sarif);
                Assert.AreEqual(1, exitCode);
                Assert.AreEqual("", stdout);
                var text = File.ReadAllText(sarif);
                StringAssert.Contains(text, "2.1.0");
                StringAssert.Contains(text, "test.eve");
            }
        }

        [TestMethod]
        public void Play_RequiresOneEventFile()
        {
            var (exitCode, _, stderr) = Run("play");
            Assert.AreEqual(2, exitCode);
            StringAssert.Contains(stderr, "イベントファイルを 1 つ指定してください");
        }

        [TestMethod]
        public void Play_Script()
        {
            using (var scenario = new TestScenario())
            {
                // データセーブ・Continue で次のイベントファイルへ進む
                CopyCase("intermission-save", scenario.Root);
                var (exitCode, stdout, stderr) = Run("play", Path.Combine(scenario.Root, "test.eve"), "--script", Path.Combine(scenario.Root, "play.txt"), "--skip-wait");
                Assert.AreEqual(0, exitCode, stderr);
                StringAssert.Contains(stdout, "terminated");
            }
        }

        [TestMethod]
        public void Play_ScriptFailureFails()
        {
            using (var scenario = new TestScenario())
            {
                CopyCase("message-wrap", scenario.Root);
                var script = scenario.Write("fail.txt", "auto 30", "expect message 出ないメッセージ");
                var (exitCode, _, stderr) = Run("play", Path.Combine(scenario.Root, "test.eve"), "--script", script, "--skip-wait");
                Assert.AreEqual(1, exitCode);
                StringAssert.Contains(stderr, "error: ");
            }
        }

        /// <summary>
        /// イベントのエラーは例外にならず、ダイアログを出してイベントを打ち切る（元の SRC と同じ）。
        /// expect error none は、そのダイアログが出たことも失敗にする（#1551。検証用のシナリオの通しのプレイで見つけるため）
        /// </summary>
        [TestMethod]
        public void Play_EventErrorFailsExpectErrorNone()
        {
            using (var scenario = new TestScenario())
            {
                CopyCase("message-wrap", scenario.Root);
                scenario.Write("test.eve", "プロローグ:", "Exit", "", "スタート:", "ChangeMap test.map", "GameClear 1", "Quit");
                var script = scenario.Write("error.txt", "auto 30", "expect error none");
                var (exitCode, _, stderr) = Run("play", Path.Combine(scenario.Root, "test.eve"), "--script", script, "--skip-wait");
                Assert.AreEqual(1, exitCode, stderr);
                StringAssert.Contains(stderr, "エラーのダイアログが出ています");
                StringAssert.Contains(stderr, "GameClearコマンドの引数の数が違います");
            }
        }

        /// <summary>
        /// --seed で乱数の種を決め、--state に各話（イベントファイル）の終わりの状態を書く（#1551）。
        /// 同じ種なら同じ状態になる。Continue で次のイベントファイルに移ったときに、前の話の終わりを記録する
        /// </summary>
        [TestMethod]
        public void Play_SeedAndStateAreRepeatable()
        {
            var states = new string[2];
            for (var i = 0; i < states.Length; i++)
            {
                using (var scenario = new TestScenario())
                {
                    CopyCase("intermission-save", scenario.Root);
                    var state = Path.Combine(scenario.Root, "state.txt");
                    var (exitCode, stdout, stderr) = Run("play", Path.Combine(scenario.Root, "test.eve"), "--script", Path.Combine(scenario.Root, "play.txt"), "--skip-wait", "--seed", "1551", "--state", state);
                    Assert.AreEqual(0, exitCode, stderr);
                    StringAssert.Contains(stdout, "terminated");
                    states[i] = File.ReadAllText(state);
                }
            }
            StringAssert.Contains(states[0], "## test.eve の終わり（次は next.eve）");
            StringAssert.Contains(states[0], "## next.eve の終わり（シナリオの終わり）");
            StringAssert.Contains(states[0], "資金: ");
            StringAssert.Contains(states[0], "テスト機");
            StringAssert.Contains(states[0], "テストパイロット");
            StringAssert.Contains(states[0], "## イベントのエラー\nなし");
            Assert.AreEqual(states[0], states[1]);
        }

        [TestMethod]
        public void Play_AutoWritesSnapshots()
        {
            using (var scenario = new TestScenario())
            {
                CopyCase("message-wrap", scenario.Root);
                var outDir = Path.Combine(scenario.Root, "out");
                var (exitCode, stdout, stderr) = Run("play", Path.Combine(scenario.Root, "test.eve"), "--steps", "20", "--out", outDir, "--skip-wait", "--app-path", scenario.Root, "--scenario-root", scenario.Root);
                Assert.AreEqual(0, exitCode, stderr);
                // 自動プレイで送ったメッセージを出す
                StringAssert.Contains(stdout, "message: いろはにほへと");
                StringAssert.Contains(stdout, "terminated");
                Assert.IsTrue(Directory.GetFiles(outDir, "*.png").Length > 0, "画面のスナップショット");
            }
        }

        [TestMethod]
        public void Balance_Markdown()
        {
            var (exitCode, stdout, stderr) = Run("balance", Case("battle-basic"));
            Assert.AreEqual(0, exitCode, stderr);
            StringAssert.Contains(stdout, "テスト機");
            StringAssert.Contains(stdout, "|");
        }

        [TestMethod]
        [DataRow("csv", "テスト機")]
        [DataRow("weapons-csv", "テストビーム")]
        public void Balance_CsvToFile(string format, string expected)
        {
            using (var scenario = new TestScenario())
            {
                var output = Path.Combine(scenario.Root, format + ".csv");
                var (exitCode, stdout, stderr) = Run("balance", Case("battle-basic"), "--format", format, "--output", output);
                Assert.AreEqual(0, exitCode, stderr);
                Assert.AreEqual("", stdout);
                var csv = File.ReadAllText(output);
                StringAssert.Contains(csv.Split('\n')[0], ",");
                StringAssert.Contains(csv, expected);
            }
        }

        [TestMethod]
        public void Help_Topic()
        {
            var (exitCode, stdout, _) = Run("help", "Askコマンド");
            Assert.AreEqual(0, exitCode);
            StringAssert.Contains(stdout, "プレイヤーによる選択肢入力");
        }

        [TestMethod]
        public void Help_MissingAndAmbiguous()
        {
            var (exitCode, _, stderr) = Run("help", "存在しない項目XYZ");
            Assert.AreEqual(1, exitCode);
            StringAssert.Contains(stderr, "ヘルプの項目が見つかりません: 存在しない項目XYZ");

            (exitCode, _, stderr) = Run("help", "Clear");
            Assert.AreEqual(1, exitCode);
            StringAssert.Contains(stderr, "Clear に当てはまる項目が複数あります");
        }

        [TestMethod]
        public void Help_ListAndGrep()
        {
            var (exitCode, stdout, _) = Run("help", "--list");
            Assert.AreEqual(0, exitCode);
            StringAssert.Contains(stdout, "Askコマンド");

            (exitCode, stdout, _) = Run("help", "--grep", "プレイヤーによる選択肢入力");
            Assert.AreEqual(0, exitCode);
            StringAssert.Contains(stdout, "Askコマンド:");

            (exitCode, stdout, _) = Run("help", "--grep", "存在しない文字列XYZ");
            Assert.AreEqual(1, exitCode);
            Assert.AreEqual("", stdout);
        }

        [TestMethod]
        public void Help_Search()
        {
            var (exitCode, stdout, _) = Run("help", "--search", "選択肢 入力", "--method", "bm25", "--limit", "3");
            Assert.AreEqual(0, exitCode);
            StringAssert.Contains(stdout, "Askコマンド");

            (exitCode, _, var stderr) = Run("help", "--search", "選択肢", "--method", "fuzzy");
            Assert.AreEqual(2, exitCode);
            StringAssert.Contains(stderr, "不明な検索の方法: fuzzy");
        }

        [TestMethod]
        public void Help_Export()
        {
            using (var scenario = new TestScenario())
            {
                var dir = Path.Combine(scenario.Root, "help");
                var (exitCode, _, stderr) = Run("help", "--export", dir);
                Assert.AreEqual(0, exitCode);
                StringAssert.Contains(stderr, dir);
                Assert.IsTrue(File.Exists(Path.Combine(dir, "Askコマンド.md")));
            }
        }

        [TestMethod]
        public void EventFlow_TalkPreview_And_AnimePreview_Commands()
        {
            using (var scenario = new TestScenario())
            {
                var eve = scenario.Write("ep01.eve",
                    "スタート:",
                    "Call 会話テスト",
                    "Exit",
                    "",
                    "会話テスト:",
                    "Talk システム",
                    "テストメッセージ",
                    "End",
                    "Return");

                var (exitCode, stdout, stderr) = Run("event-flow", eve, "--format", "json");
                Assert.AreEqual(0, exitCode, stderr);
                StringAssert.Contains(stdout, "会話テスト");
                StringAssert.Contains(stdout, "\"edges\"");

                (exitCode, stdout, stderr) = Run("talk-preview", eve, "--line", "7", "--format", "json", "--no-images");
                Assert.AreEqual(0, exitCode, stderr);
                StringAssert.Contains(stdout, "テストメッセージ");
                StringAssert.Contains(stdout, "\"blocks\"");

                scenario.Write(Path.Combine("Data", "robot.txt"),
                    "試作機\n試作機, しさくき, 汎用, 1, 2\n陸, 4, M, 1000, 100\n特殊能力なし\n3000, 100, 600, 60\nAAAA, -.bmp\n===\n");
                scenario.Write(Path.Combine("Data", "pilot.txt"),
                    "主人公\n主人公, しゅじんこう, 男性, 汎用, AAAA, 120\n特殊能力なし\n150, 145, 150, 150, 150, 155, 普通\nＳＰ, 0\n-.bmp, -.mid\n");
                var anim = scenario.Write(Path.Combine("Data", "animation.txt"),
                    "試作機",
                    "攻撃, 斬撃");

                (exitCode, stdout, stderr) = Run("anime-preview", anim, "--line", "2", "--list");
                Assert.AreEqual(0, exitCode, stderr);
                StringAssert.Contains(stdout, "\"situation\": \"攻撃\"");
                StringAssert.Contains(stdout, "\"selectedIndex\": 0");

                (exitCode, stdout, stderr) = Run("anime-preview", anim, "--line", "2");
                Assert.AreEqual(0, exitCode, stderr);
                StringAssert.Contains(stdout, "演出プレビュー.eve");
                StringAssert.Contains(stdout, "\"animationPreview\"");
            }
        }
    }
}
