using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using SRCPlay;

namespace SRCRender.Tests
{
    /// <summary>
    /// SRC のフルパッケージ（素材とサンプルシナリオ）を使ったスモークテスト。
    /// 素材は再配布できないため、環境変数 SRC_SAMPLE_DATA にフルパッケージを展開したフォルダを指定した場合のみ実行する。
    /// </summary>
    [TestClass]
    public class SampleScenarioSmokeTests
    {
        public TestContext TestContext { get; set; }

        private static string SampleData => Environment.GetEnvironmentVariable("SRC_SAMPLE_DATA");

        [TestMethod]
        [DataRow("Welcome.eve", 200)]
        public void AutoPlay_SampleScenario(string eve, int maxSteps)
        {
            if (string.IsNullOrEmpty(SampleData) || !Directory.Exists(SampleData))
            {
                Assert.Inconclusive("SRC_SAMPLE_DATA が指定されていません");
            }
            var path = Path.Combine(SampleData, "サンプルシナリオ", eve);
            using (var runner = new ScreenScenarioRunner(SampleData))
            {
                runner.Start(path);
                var player = new AutoPlayer(runner)
                {
                    SnapshotDirectory = Path.Combine(AppContext.BaseDirectory, "TestResults", "sample", Path.GetFileNameWithoutExtension(eve)),
                };
                player.Run(maxSteps);

                TestContext.WriteLine("steps: " + player.Steps);
                foreach (var m in player.Messages)
                {
                    TestContext.WriteLine("MSG " + m);
                }
                foreach (var d in player.Dialogs)
                {
                    TestContext.WriteLine("DLG " + d);
                }
                TestContext.WriteLine("error: " + runner.Error);
                Assert.IsTrue(player.Steps > 0, "no progress");
            }
        }
    }
}
