using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using SRCPlay;
using System;
using System.IO;
using System.Threading;

namespace SRCRender.Tests
{
    /// <summary>
    /// Main.frm の picFace_Click: ダブルクリックの 2 回目では Click が起きないので、表示するパイロットは 1 つだけ進む（#1498）。
    /// </summary>
    [TestClass]
    public class FormsFrmFaceDoubleClickTests
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

        [TestMethod]
        public void DoubleClickFace_SwitchesPilotOnce()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                var src = runner.Session.SRC;
                runner.WaitFor(s => !runner.GUI.IsGUILocked && src.Commands.CommandState == "ユニット選択" && !runner.Session.Game.IsBusy,
                    "ユニット選択");

                using (var done = new ManualResetEventSlim())
                {
                    runner.Session.Game.Post(() =>
                    {
                        runner.GUI.DisplayUnitStatus(src.Map.MapDataForUnit[3, 3]);
                        done.Set();
                    });
                    Assert.IsTrue(done.Wait(ScreenScenarioRunner.Timeout));
                }
                Assert.AreEqual(1, runner.GUI.DisplayedPilotInd);

                var x = ScreenLayout.StatusArea.Left + 4 + 34;
                var y = ScreenLayout.StatusArea.Top + 4 + 34;
                runner.GUI.PointerPressed(x, y, GuiButton.Left, 1);
                runner.GUI.PointerReleased(x, y, GuiButton.Left);
                runner.GUI.PointerPressed(x, y, GuiButton.Left, 2);
                runner.GUI.PointerReleased(x, y, GuiButton.Left);
                runner.WaitFor(s => runner.GUI.DisplayedPilotInd == 2, "サブパイロットの表示");
                runner.Sync();
                Thread.Sleep(200);
                runner.Sync();
                Assert.AreEqual(2, runner.GUI.DisplayedPilotInd);
                Assert.AreEqual("サブ", runner.GUI.DisplayedPilot?.Name);
            }
        }
    }
}
