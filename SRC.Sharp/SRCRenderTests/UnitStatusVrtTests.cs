using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCPlay;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCRender.Tests
{
    /// <summary>
    /// ユニットの種類ごとのステータスの欄と、戦闘の予想の画面の VRT（#1400）。E2E のコーパス vrt-units を play.txt で進め、
    /// snapshot で撮った画面を基準画像（Baselines/vrt-units-&lt;名前&gt;.png）と比べる。母艦・変形・合体・分離・召喚・
    /// アイテムの装備・特殊能力と武器の多いユニット・情報を隠したユニットのステータス、攻撃の相手を選ぶときの予想、
    /// 敵の攻撃への対応を選ぶリストを見る。基準画像の更新は SRC_VRT_UPDATE=1。
    /// </summary>
    [TestClass]
    public class UnitStatusVrtTests
    {
        private const string Corpus = "vrt-units";

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
