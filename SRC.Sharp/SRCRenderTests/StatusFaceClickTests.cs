using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using SRCPlay;
using System;
using System.IO;
using System.Threading;

namespace SRCRender.Tests
{
    /// <summary>
    /// ステータスウインドウの顔グラフィックのクリックで、表示するパイロットを順に切り替える（Main.frm の picFace_Click、7474/SRC#1485）。
    /// </summary>
    [TestClass]
    public class StatusFaceClickTests
    {
        private ScenarioWorkspace workspace;

        [TestInitialize]
        public void Initialize()
        {
            workspace = new ScenarioWorkspace();
            workspace.WriteMap("test.map", 20, 20, (x, y) => 0);
            workspace.Write(Path.Combine("Data", "robot.txt"),
                "複座機",
                "複座機, ふくざき, 汎用, 2, 2",
                "陸, 4, M, 1000, 100",
                "特殊能力なし",
                "1000, 100, 1000, 60",
                "AAAA, -.bmp",
                "===",
                "");
            workspace.Write(Path.Combine("Data", "pilot.txt"),
                "メイン",
                "メイン, めいん, 男性, 汎用, AAAA, 100",
                "特殊能力なし",
                "100, 100, 100, 100, 100, 100, 普通",
                "ＳＰなし",
                "-.bmp, -.mid",
                "",
                "サブ",
                "サブ, さぶ, 男性, 汎用, AAAA, 100",
                "特殊能力なし",
                "100, 100, 100, 100, 100, 100, 普通",
                "ＳＰなし",
                "-.bmp, -.mid",
                "");
            workspace.Write("test.eve",
                "スタート:",
                "ChangeMap test.map",
                "Create 味方 複座機 0 メイン 1 3 3",
                "Pilot サブ 1",
                "Ride サブ 複座機",
                "Exit",
                "");
        }

        [TestCleanup]
        public void Cleanup()
        {
            workspace.Dispose();
        }

        private static void RunOnGame(ScreenScenarioRunner runner, Action action)
        {
            Exception error = null;
            using (var done = new ManualResetEventSlim())
            {
                runner.Session.Game.Post(() =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }
                    finally
                    {
                        done.Set();
                    }
                });
                Assert.IsTrue(done.Wait(ScreenScenarioRunner.Timeout));
            }
            Assert.IsNull(error, error?.ToString());
        }

        private static void ClickFace(ScreenScenarioRunner runner)
        {
            // 顔グラフィックはステータスウィンドウの左上（StatusArea からの相対で (4, 4)、68×68）
            var x = ScreenLayout.StatusArea.Left + 4 + 34;
            var y = ScreenLayout.StatusArea.Top + 4 + 34;
            runner.GUI.PointerPressed(x, y, GuiButton.Left);
            runner.GUI.PointerReleased(x, y, GuiButton.Left);
        }

        [TestMethod]
        public void ClickFace_SwitchesDisplayedPilot()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                var src = runner.Session.SRC;
                runner.WaitFor(s => !runner.GUI.IsGUILocked && src.Commands.CommandState == "ユニット選択" && !runner.Session.Game.IsBusy,
                    "ユニット選択");

                RunOnGame(runner, () => runner.GUI.DisplayUnitStatus(src.Map.MapDataForUnit[3, 3]));
                Assert.IsNull(runner.GUI.DisplayedPilot);
                Assert.AreEqual(1, runner.GUI.DisplayedPilotInd);

                // クリックするとサブパイロットを表示する
                ClickFace(runner);
                // 表示するパイロット（DisplayedPilot）を変えた後で番号（DisplayedPilotInd）を変えるため、両方を待つ
                runner.WaitFor(s => runner.GUI.DisplayedPilot?.Name == "サブ" && runner.GUI.DisplayedPilotInd == 2, "サブパイロットの表示");

                // もう一度クリックするとメインパイロットに戻る
                ClickFace(runner);
                runner.WaitFor(s => runner.GUI.DisplayedPilot?.Name == "メイン" && runner.GUI.DisplayedPilotInd == 1, "メインパイロットの表示");
            }
        }
    }
}
