using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using SRCPlay;

namespace SRCRender.Tests
{
    /// <summary>
    /// 互換性の検証用のシナリオ「トレス島試験記録」（SRC.Sharp/Compat/torres。#1549）を、自作の最小の汎用データ（SRC.Sharp/Content/basic-data）で動かす。
    /// LanternTests と同じ作り。PlayScript（Compat/torres.play・Compat/torres-ep02.play〜torres-ep08.play）のうち、各話の初めの決まった手順
    /// （合体技・使い切りのアイテム・マップ攻撃・スペシャルパワー・変形・合体・分離など）と、その結果の expect を流す。
    /// 最後まで自動で進める部分は時間がかかるため流さない（srcsharp play --skip-wait なら数秒で最後まで進む。Compat/README.md）。
    /// </summary>
    [TestClass]
    // 乱数（GeneralLib の Rnd）はプロセスで 1 つなので、ほかのテストと並べて流すと系列が乱れ、
    // 戦闘の結果（クリティカルなど）で各話の expect が揺れる。種を compat_play.py と同じ 1 にして、並べずに流す
    [DoNotParallelize]
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

        /// <summary>第三話を単体で始め、水上・水中・地形適応・ＺＯＣとＺＯＣ無効化・パーツ分離・着艦と発進・中断とクイックセーブからの再開までを進める</summary>
        [TestMethod]
        public void ThirdEpisode_PlaysWaterwayAndZoc()
        {
            Play("torres-ep03.play", "ep03.eve", "compat-ep03");
        }

        /// <summary>第四話を単体で始め、テレポート・ジャンプ・ハイパーモードとノーマルモード・追加パイロット・暴走・必要技能までを進める</summary>
        [TestMethod]
        public void FourthEpisode_PlaysSkyCorridor()
        {
            Play("torres-ep04.play", "ep04.eve", "compat-ep04");
        }

        /// <summary>第五話を単体で始め、属性（固・連・吸・減・奪）・先制攻撃・追加サポート・召喚と召喚解除・地中までを進める</summary>
        [TestMethod]
        public void FifthEpisode_PlaysAttributesAndSummon()
        {
            Play("torres-ep05.play", "ep05.eve", "compat-ep05");
        }

        /// <summary>第六話を単体で始め、Load・宇宙・中断から再開した後の Load したデータでの増援までを進める</summary>
        [TestMethod]
        public void SixthEpisode_PlaysLoadAndSpace()
        {
            Play("torres-ep06.play", "ep06.eve", "compat-ep06");
        }

        /// <summary>第七話を単体で始め、合体したままの中断からの再開と、クイックセーブからハイパーモードの前に戻るまでを進める</summary>
        [TestMethod]
        public void SeventhEpisode_PlaysResumeWhileCombined()
        {
            Play("torres-ep07.play", "ep07.eve", "compat-ep07");
        }

        /// <summary>第八話を単体で始め、中断とクイックセーブからの再開と、ダメージ固定の武器で標的を倒すところまでを進める</summary>
        [TestMethod]
        public void EighthEpisode_PlaysFinalTest()
        {
            Play("torres-ep08.play", "ep08.eve", "compat-ep08");
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
                    runner.Session.SRC.InitialRndSeed = 1;
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
