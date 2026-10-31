using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCPlay;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCRender.Tests
{
    /// <summary>
    /// セーブデータの版をまたぐ互換（#1357）。E2E のコーパス save-compat で作った、各版のセーブ
    /// （SRC.Sharp/E2E/saves/v&lt;版&gt;/）のすべてから再開し、最後まで進められることを確かめる。
    /// リリースのときに、その版のセーブを足す（tools/release.py saves。SRC_SAVE_GENERATE=&lt;版&gt; で GenerateSaves が作る）。
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class SaveCompatibilityTests
    {
        // セーブを作るときと再開するときで、シナリオのフォルダを変える（#1375）。
        // 中断・クイックセーブ・リスタートのデータは、シナリオのファイルをシナリオのフォルダからの相対パスで持つ。
        // v3.5.5 までのセーブは絶対パス（作ったときの一時フォルダ srcs-save-compat）で持っていて、それでも再開できることも確かめる
        private const string GenerateWorkspaceName = "srcs-save-compat";

        private static string ScenarioDirectory => Path.Combine(E2ECorpusTests.CorpusDirectory, "save-compat");
        private static string SavesDirectory => Path.Combine(E2ECorpusTests.CorpusDirectory, "saves");

        // セーブの種類 → 再開の手順のファイル名
        private static readonly Dictionary<string, string> Kinds = new Dictionary<string, string>
        {
            ["_quik.srcq"] = "resume-quik.txt",
            ["*.srcs"] = "resume-save.txt",
            ["_リスタート.srcq"] = "resume-restart.txt",
        };

        public static IEnumerable<object[]> Saves => Directory.Exists(SavesDirectory)
            ? Directory.GetDirectories(SavesDirectory)
                .SelectMany(dir => Kinds.Keys.SelectMany(pattern => Directory.GetFiles(dir, pattern))
                    .Select(file => new object[] { Path.GetFileName(dir), Path.GetFileName(file) }))
                .OrderBy(x => (string)x[0]).ThenBy(x => (string)x[1])
            : Enumerable.Empty<object[]>();

        [TestMethod]
        public void EveryVersionHasThreeSaves()
        {
            var versions = Directory.GetDirectories(SavesDirectory);
            Assert.IsTrue(versions.Length > 0, "SRC.Sharp/E2E/saves にセーブが無い");
            foreach (var dir in versions)
            {
                foreach (var pattern in Kinds.Keys)
                {
                    Assert.AreEqual(1, Directory.GetFiles(dir, pattern).Length, $"{Path.GetFileName(dir)} に {pattern} が 1 つ無い");
                }
            }
        }

        [TestMethod]
        [DynamicData(nameof(Saves))]
        public void ResumeFromSave(string version, string save)
        {
            var kind = Kinds.First(k => k.Key.StartsWith("*") ? save.EndsWith(k.Key.Substring(1)) : save == k.Key);
            using (var workspace = new ScenarioWorkspace())
            {
                Prepare(workspace);
                var path = Path.Combine(workspace.AppPath, save);
                File.Copy(Path.Combine(SavesDirectory, version, save), path, true);
                using (var runner = new ScreenScenarioRunner(workspace.AppPath))
                {
                    var script = new PlayScript(runner);
                    runner.Start(path);
                    Run(script, runner, File.ReadAllLines(Path.Combine(ScenarioDirectory, kind.Value)), $"{version} の {save} から再開");
                }
            }
        }

        /// <summary>
        /// SRC_SAVE_GENERATE=&lt;版&gt;（例 3.5.5）のときだけ、save-compat をプレイして SRC.Sharp/E2E/saves/v&lt;版&gt;/ にセーブを置く。
        /// </summary>
        [TestMethod]
        public void GenerateSaves()
        {
            var version = Environment.GetEnvironmentVariable("SRC_SAVE_GENERATE");
            if (string.IsNullOrEmpty(version))
            {
                Assert.Inconclusive("SRC_SAVE_GENERATE が無いので作らない");
            }
            string requested = null;
            using (var workspace = new ScenarioWorkspace(name: GenerateWorkspaceName))
            {
                Prepare(workspace);
                using (var runner = new ScreenScenarioRunner(workspace.AppPath))
                {
                    // データセーブの保存先は、ダイアログの既定の名前（無ければ save.srcs）のままシナリオのフォルダにする
                    runner.SaveFileHandler = request => requested = Path.Combine(
                        string.IsNullOrEmpty(request.InitialDirectory) ? workspace.AppPath : request.InitialDirectory,
                        string.IsNullOrEmpty(request.InitialFile) ? "save.srcs" : request.InitialFile);
                    var script = new PlayScript(runner);
                    runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                    Run(script, runner, File.ReadAllLines(Path.Combine(ScenarioDirectory, "play.txt")), "save-compat のプレイ");
                }
                var dest = Path.Combine(SavesDirectory, "v" + version);
                Directory.CreateDirectory(dest);
                foreach (var pattern in Kinds.Keys)
                {
                    var files = Directory.GetFiles(workspace.AppPath, pattern);
                    Assert.AreEqual(1, files.Length, $"{pattern} ができていない（保存先 {requested}、フォルダ: {string.Join(", ", Directory.GetFiles(workspace.AppPath).Select(Path.GetFileName))}）");
                    File.Copy(files[0], Path.Combine(dest, Path.GetFileName(files[0])), true);
                }
            }
        }

        private static void Prepare(ScenarioWorkspace workspace)
        {
            E2ECorpusTests.CopyDirectory(ScenarioDirectory, workspace.AppPath);
            E2ECorpusTests.CopyDirectory(Path.Combine(Path.GetDirectoryName(E2ECorpusTests.CorpusDirectory), "Content", "basic-data", "Lib"), Path.Combine(workspace.AppPath, "Lib"));
        }

        private static void Run(PlayScript script, ScreenScenarioRunner runner, string[] lines, string what)
        {
            try
            {
                script.Run(lines);
            }
            catch (Exception ex)
            {
                var state = runner.State;
                Assert.Fail(what + ": " + ex.Message + Environment.NewLine
                    + $"画面: メッセージ={state.Message.Visible}/{state.Message.Waiting} [{string.Join(" ", state.Message.Lines)}] リスト={state.List != null} メニュー={string.Join(",", state.Menu?.Items ?? new List<string>())} ダイアログ={state.Dialog?.Text} 終了={runner.IsFinished}" + Environment.NewLine
                    + string.Join(Environment.NewLine, script.TraceLog.Skip(Math.Max(0, script.TraceLog.Count - 40)))
                    + Environment.NewLine + runner.Error);
            }
        }
    }
}
