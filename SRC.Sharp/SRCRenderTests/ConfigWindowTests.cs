using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using SRCRender.Compat;
using System.IO;
using System.Threading;

namespace SRCRender.Tests
{
    /// <summary>
    /// 設定変更ダイアログ。
    /// </summary>
    [TestClass]
    public class ConfigWindowTests
    {
        private ScenarioWorkspace workspace;

        [TestInitialize]
        public void Initialize()
        {
            workspace = new ScenarioWorkspace();
            workspace.WriteMap("test.map", 20, 20, (x, y) => 0);
            workspace.Write("test.eve",
                "プロローグ:",
                "ChangeMap test.map",
                "Talk システム",
                "設定のテスト",
                "End",
                "Quit",
                "");
        }

        [TestCleanup]
        public void Cleanup()
        {
            workspace.Dispose();
        }

        private static void Click(ScreenScenarioRunner runner, Form form, Control control, float dx = -1)
        {
            var x = form.ScreenLeft + control.Left + (dx < 0 ? control.Width / 2f : dx);
            var y = form.ScreenTop + Form.TitleBarHeight + control.Top + control.Height / 2f;
            runner.GUI.PointerPressed(x, y, GuiButton.Left);
            runner.GUI.PointerReleased(x, y, GuiButton.Left);
        }

        [TestMethod]
        public void Configure_ChangesAndSavesSettings()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitForMessage();
                var config = runner.Session.SRC.SystemConfig;
                var battle = config.BattleAnimation;
                var move = config.MoveAnimation;

                var closed = new ManualResetEventSlim();
                runner.Session.Game.Post(() =>
                {
                    runner.GUI.Configure();
                    closed.Set();
                });
                runner.WaitFor(s => s.Config != null, "config window");
                var w = runner.State.Config;
                Assert.AreEqual(battle, w.chkBattleAnimation.Checked);
                Assert.AreEqual(battle, w.chkWeaponAnimation.Enabled);

                // 戦闘アニメを表示しない場合は拡張・武器準備アニメは選べない
                Click(runner, w, w.chkBattleAnimation);
                runner.WaitFor(s => w.chkBattleAnimation.Checked != battle, "battle animation toggled");
                Assert.AreEqual(!battle, w.chkWeaponAnimation.Enabled);
                Click(runner, w, w.chkMoveAnimation);
                runner.WaitFor(s => w.chkMoveAnimation.Checked != move, "move animation toggled");
                // 音量はスクロールバーの左端で 0
                Click(runner, w, w.hscMP3Volume, 1);
                runner.WaitFor(s => w.txtMP3Volume.Text == "0", "volume 0");
                VisualAssert.MatchesBaseline(runner.SnapshotPng(), "config-01-changed");

                Click(runner, w, w.cmdOK);
                Assert.IsTrue(closed.Wait(ScreenScenarioRunner.Timeout), "closed");
                Assert.IsNull(runner.State.Config);
                Assert.AreEqual(!battle, config.BattleAnimation);
                Assert.AreEqual(!move, config.MoveAnimation);
                StringAssert.Contains(File.ReadAllText(Path.Combine(workspace.AppPath, "srcs.json")), "\"MoveAnimation\": " + (!move).ToString().ToLower());
            }
        }

        [TestMethod]
        public void Configure_CancelKeepsSettings()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitForMessage();
                var config = runner.Session.SRC.SystemConfig;
                var move = config.MoveAnimation;
                var closed = new ManualResetEventSlim();
                runner.Session.Game.Post(() =>
                {
                    runner.GUI.Configure();
                    closed.Set();
                });
                runner.WaitFor(s => s.Config != null, "config window");
                var w = runner.State.Config;
                Click(runner, w, w.chkMoveAnimation);
                runner.WaitFor(s => w.chkMoveAnimation.Checked != move, "move animation toggled");
                runner.GUI.KeyPressed(ScreenKey.Escape);
                Assert.IsTrue(closed.Wait(ScreenScenarioRunner.Timeout), "closed");
                Assert.AreEqual(move, config.MoveAnimation);
            }
        }
    }
}
