using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using SRCPlay;

namespace SRCRender.Tests
{
    /// <summary>
    /// 遊べるシナリオ（SRC.Sharp/Content/lantern）を、自作の最小の汎用データ（SRC.Sharp/Content/basic-data）だけで動かす。
    /// SRC#Web の組み込みのカタログと同じ組み合わせ。最後までのプレイ（Content/lantern.play）は時間がかかるため、最初のステージの数ターンにとどめる。
    /// </summary>
    [TestClass]
    public class LanternTests
    {
        private static string ContentDirectory => Path.Combine(Path.GetDirectoryName(E2ECorpusTests.CorpusDirectory), "Content");

        [TestMethod]
        public void FirstStage_PlaysWithBasicData()
        {
            var app = Path.Combine(Path.GetTempPath(), "srcs-basic-data-" + Guid.NewGuid());
            var scenario = Path.Combine(Path.GetTempPath(), "srcs-lantern-" + Guid.NewGuid());
            try
            {
                Copy(Path.Combine(ContentDirectory, "basic-data"), app);
                Copy(Path.Combine(ContentDirectory, "lantern"), scenario);
                // 最後までのプレイの手順から、ターン数とクリアの確認を変える
                var lines = File.ReadAllLines(Path.Combine(ContentDirectory, "lantern.play"))
                    .Where(x => !x.StartsWith("expect", StringComparison.Ordinal))
                    .Select(x => x.StartsWith("turns ", StringComparison.Ordinal) ? "turns 3" : x)
                    .Append("expect error none");
                using (var runner = new ScreenScenarioRunner(app))
                {
                    var script = new PlayScript(runner)
                    {
                        SnapshotDirectory = Path.Combine(AppContext.BaseDirectory, "TestResults", "e2e", "lantern"),
                    };
                    runner.Start(Path.Combine(scenario, "01_灯の港.eve"));
                    try
                    {
                        script.Run(lines.Append("snapshot first-stage"));
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail(ex.Message + Environment.NewLine
                            + string.Join(Environment.NewLine, script.TraceLog.Skip(Math.Max(0, script.TraceLog.Count - 40)))
                            + Environment.NewLine + runner.Error);
                    }
                    Assert.IsTrue(script.TraceLog.Any(x => x.Contains("PHASE")), "マップのフェイズまで進んでいない");
                }
            }
            finally
            {
                foreach (var dir in new[] { app, scenario })
                {
                    if (Directory.Exists(dir))
                    {
                        Directory.Delete(dir, true);
                    }
                }
            }
        }

        private static void Copy(string from, string to)
        {
            Directory.CreateDirectory(to);
            foreach (var file in Directory.GetFiles(from))
            {
                File.Copy(file, Path.Combine(to, Path.GetFileName(file)), true);
            }
            foreach (var dir in Directory.GetDirectories(from))
            {
                Copy(dir, Path.Combine(to, Path.GetFileName(dir)));
            }
        }
    }
}
