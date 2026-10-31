using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCPlay;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCRender.Tests
{
    /// <summary>
    /// ゲームの画面の VRT（#1358）。E2E のコーパス vrt-screens を play.txt で進め、snapshot で撮った画面を
    /// 基準画像（Baselines/vrt-screens-&lt;名前&gt;.png）と比べる。顔の絵つきのメッセージ・ステータスの欄・
    /// 特殊能力一覧とその解説・武装一覧など、ほかの VRT に無い画面を見る。基準画像の更新は SRC_VRT_UPDATE=1。
    /// </summary>
    [TestClass]
    public class GameScreenVrtTests
    {
        private const string Corpus = "vrt-screens";

        [TestMethod]
        public void Screens_MatchBaselines()
        {
            var source = Path.Combine(E2ECorpusTests.CorpusDirectory, Corpus);
            var snapshots = new List<(string Name, byte[] Png)>();
            using (var workspace = new ScenarioWorkspace())
            {
                E2ECorpusTests.CopyDirectory(source, workspace.AppPath);
                using (var runner = new ScreenScenarioRunner(workspace.AppPath))
                {
                    var script = new PlayScript(runner) { SnapshotTaken = (name, png) => snapshots.Add((name, png)) };
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
            Assert.IsTrue(snapshots.Count > 0, "snapshot がありません");
            foreach (var (name, png) in snapshots)
            {
                VisualAssert.MatchesBaseline(png, $"{Corpus}-{name}");
            }
        }
    }
}
