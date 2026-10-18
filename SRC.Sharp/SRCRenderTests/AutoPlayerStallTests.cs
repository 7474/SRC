using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using SRCCore.Lib;
using SRCPlay;
using System.IO;
using System.Linq;

namespace SRCRender.Tests
{
    /// <summary>
    /// 待ちの状態でないまま進まないとき（KeyState を待つループなど）に、AutoPlayer がボタンを押す処理。
    /// </summary>
    [TestClass]
    public class AutoPlayerStallTests
    {
        private ScenarioWorkspace workspace;

        [TestInitialize]
        public void Initialize()
        {
            workspace = new ScenarioWorkspace();
            workspace.WriteMap("test.map", 20, 20, (x, y) => (x + y) % 2);
        }

        [TestCleanup]
        public void Cleanup()
        {
            workspace.Dispose();
        }

        [TestMethod]
        public void KeyStatePolls_CountsKeyStateReads()
        {
            workspace.Write("test.eve",
                "プロローグ:",
                "ChangeMap test.map",
                "Wait Click",
                "Talk システム",
                "Z=$(KeyState(90)) X=$(KeyState(88))",
                "End",
                "Quit",
                "");
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitFor(s => runner.GUI.IsWaitingClick, "wait click");
                var polls = runner.GUI.KeyStatePolls;
                runner.GUI.KeyPressed(ScreenKey.Enter);
                Assert.AreEqual("Z=0 X=0", runner.WaitForMessage());
                Assert.IsTrue(runner.GUI.KeyStatePolls >= polls + 2, $"{polls} -> {runner.GUI.KeyStatePolls}");
            }
        }

        // GeneralLib.SkipWait はプロセス全体の設定なので、ほかのテストと並べて走らせない
        [TestMethod]
        [DoNotParallelize]
        public void SkipWait_PressesWhileKeyStateIsPolled()
        {
            workspace.Write("test.eve",
                "プロローグ:",
                "ChangeMap test.map",
                "Do Until KeyState(1)",
                "Wait 1",
                "Loop",
                "Talk システム",
                "押された",
                "End",
                "Quit",
                "");
            GeneralLib.SkipWait = true;
            try
            {
                using (var runner = new ScreenScenarioRunner(workspace.AppPath))
                {
                    runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                    var player = new AutoPlayer(runner)
                    {
                        StallClickMillis = 10,
                        // 読まれていないときの押下（従来の動作）では抜けないようにする
                        StallFallbackMillis = 60000,
                    };
                    player.Run(20, 10000);
                    Assert.IsTrue(player.Messages.Any(m => m.Contains("押された")), string.Join(" / ", player.Messages));
                    Assert.AreEqual(1, player.Stalls);
                }
            }
            finally
            {
                GeneralLib.SkipWait = false;
            }
        }
    }
}
