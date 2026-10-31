using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SRCPlay;

namespace SRCRender.Tests
{
    /// <summary>
    /// E2E のコーパス（SRC.Sharp/E2E/ケース名/）を、それぞれの play.txt（PlayScript）でプレイする。
    /// ケースのフォルダはシナリオのフォルダとして使う（test.eve から始める）。素材はテスト用の最小構成（ScenarioWorkspace）を使う。
    /// スナップショットは TestResults/e2e/ケース名/ に保存する。
    /// </summary>
    [TestClass]
    public class E2ECorpusTests
    {
        public static string CorpusDirectory
        {
            get
            {
                var dir = new DirectoryInfo(AppContext.BaseDirectory);
                while (dir != null)
                {
                    var candidate = Path.Combine(dir.FullName, "E2E");
                    if (Directory.Exists(candidate) && File.Exists(Path.Combine(dir.FullName, "SRC.Sharp.sln")))
                    {
                        return candidate;
                    }
                    dir = dir.Parent;
                }
                throw new DirectoryNotFoundException("SRC.Sharp/E2E が見つかりません");
            }
        }

        public static IEnumerable<object[]> Cases => Directory.GetDirectories(CorpusDirectory)
            .Where(x => File.Exists(Path.Combine(x, "play.txt")))
            .OrderBy(x => x, StringComparer.Ordinal)
            .Select(x => new object[] { Path.GetFileName(x) });

        internal static void CopyDirectory(string from, string to)
        {
            Directory.CreateDirectory(to);
            foreach (var file in Directory.GetFiles(from))
            {
                File.Copy(file, Path.Combine(to, Path.GetFileName(file)), true);
            }
            foreach (var dir in Directory.GetDirectories(from))
            {
                CopyDirectory(dir, Path.Combine(to, Path.GetFileName(dir)));
            }
        }

        [TestMethod]
        [DynamicData(nameof(Cases))]
        public void Play(string name)
        {
            var source = Path.Combine(CorpusDirectory, name);
            using (var workspace = new ScenarioWorkspace())
            {
                CopyDirectory(source, workspace.AppPath);
                // SRC 本体と同じく、インターミッションのステータス表示などの Lib を AppPath に置く（SRC.Sharp/Content/basic-data/Lib）
                CopyDirectory(Path.Combine(Path.GetDirectoryName(CorpusDirectory), "Content", "basic-data", "Lib"), Path.Combine(workspace.AppPath, "Lib"));
                using (var runner = new ScreenScenarioRunner(workspace.AppPath))
                {
                    var script = new PlayScript(runner)
                    {
                        SnapshotDirectory = Path.Combine(AppContext.BaseDirectory, "TestResults", "e2e", name),
                    };
                    runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                    try
                    {
                        script.Run(File.ReadAllLines(Path.Combine(source, "play.txt")));
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail(ex.Message + Environment.NewLine
                            + string.Join(Environment.NewLine, script.TraceLog.Skip(Math.Max(0, script.TraceLog.Count - 40)))
                            + Environment.NewLine + runner.Error);
                    }
                }
            }
        }
    }
}
