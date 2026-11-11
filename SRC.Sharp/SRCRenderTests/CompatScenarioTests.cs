using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using SRCPlay;

namespace SRCRender.Tests
{
    /// <summary>
    /// 互換性の検証用のシナリオ「トレス島試験記録」（SRC.Sharp/Compat/torres。#1549）を、自作の最小の汎用データ（SRC.Sharp/Content/basic-data）で動かす。
    /// LanternTests と同じ作り。PlayScript（Compat/torres.play・Compat/torres-ep02.play）のうち、各話の初めの決まった手順
    /// （合体技・使い切りのアイテム・マップ攻撃・スペシャルパワー・変形・合体・分離など）と、その結果の expect を流す。
    /// 最後まで自動で進める部分は時間がかかるため流さない（srcsharp play --skip-wait なら数秒で最後まで進む。Compat/README.md）。
    /// </summary>
    [TestClass]
    public class CompatScenarioTests
    {
        private const string StopMarker = "# ここから最後まで自動で進める";

        private static string SharpDirectory => Path.GetDirectoryName(E2ECorpusTests.CorpusDirectory);

        /// <summary>第一話を始め、1・2 ターン目の決まった手順と 3 ターン目の初め（スペシャルパワーの自動発動）までを進める</summary>
        [TestMethod]
        public void FirstEpisode_PlaysFirstTurns()
        {
            Play("torres.play", "ep01.eve", "compat-ep01");
        }

        /// <summary>第二話を単体で始め、変形・合体・マップ攻撃（識）・母艦からの発進・使い切りのアビリティ・分離までを進める</summary>
        [TestMethod]
        public void SecondEpisode_PlaysFormChanges()
        {
            Play("torres-ep02.play", "ep02.eve", "compat-ep02");
        }

        private static void Play(string playFile, string eveFile, string name)
        {
            var app = Path.Combine(Path.GetTempPath(), "srcs-basic-data-" + Guid.NewGuid());
            var scenario = Path.Combine(Path.GetTempPath(), "srcs-torres-" + Guid.NewGuid());
            try
            {
                E2ECorpusTests.CopyDirectory(Path.Combine(SharpDirectory, "Content", "basic-data"), app);
                E2ECorpusTests.CopyDirectory(Path.Combine(SharpDirectory, "Compat", "torres"), scenario);
                var lines = File.ReadAllLines(Path.Combine(SharpDirectory, "Compat", playFile))
                    .TakeWhile(x => !x.StartsWith(StopMarker, StringComparison.Ordinal))
                    .ToList();
                Assert.IsTrue(lines.Count < File.ReadAllLines(Path.Combine(SharpDirectory, "Compat", playFile)).Length, $"{playFile} に「{StopMarker}」の行が無い");
                lines.Add("expect error none");
                using (var runner = new ScreenScenarioRunner(app))
                {
                    var script = new PlayScript(runner)
                    {
                        SnapshotDirectory = Path.Combine(AppContext.BaseDirectory, "TestResults", "e2e", name),
                    };
                    runner.Start(Path.Combine(scenario, eveFile));
                    try
                    {
                        script.Run(lines.Append("snapshot " + name));
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail(ex.Message + Environment.NewLine
                            + string.Join(Environment.NewLine, script.TraceLog.Skip(Math.Max(0, script.TraceLog.Count - 40)))
                            + Environment.NewLine + runner.Error);
                    }
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
    }
}
