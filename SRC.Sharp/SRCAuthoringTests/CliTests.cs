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
    }
}
