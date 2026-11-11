using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using SRCRender.Compat;
using System.IO;
using System.Threading;
using SRCPlay;

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
                // 汎用戦闘アニメが無ければ戦闘アニメは使えない（ダイアログはゲームが使う値を見せる）
                var battle = runner.Session.SRC.BattleAnimation;
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
        public void Configure_ChangesMessageSpeed()
        {
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(Path.Combine(workspace.AppPath, "test.eve"));
                runner.WaitForMessage();
                var config = runner.Session.SRC.SystemConfig;
                // 設定ファイルの値（既定は「普通」）でメッセージを送る
                Assert.AreEqual(700, runner.GUI.MessageWait);

                var closed = new ManualResetEventSlim();
                runner.Session.Game.Post(() =>
                {
                    runner.GUI.Configure();
                    closed.Set();
                });
                runner.WaitFor(s => s.Config != null, "config window");
                var w = runner.State.Config;
                var combo = w.cboMessageSpeed;
                // 元の SRC と同じ選択肢
                CollectionAssert.AreEqual(new[] { "手動送り", "低速", "普通", "高速", "超高速", "神の領域" }, combo.Items);
                Assert.AreEqual("普通", combo.Text);

                // 欄を押すと選択肢を開き、選ぶと閉じる
                Click(runner, w, combo);
                runner.WaitFor(s => combo.DroppedDown, "dropped down");
                var drop = combo.DropDownRect;
                var x = w.ScreenLeft + drop.Left + drop.Width / 2;
                var y = w.ScreenTop + Form.TitleBarHeight + drop.Top + 1 + ComboBox.ItemHeight * 4.5f;
                runner.GUI.PointerPressed(x, y, GuiButton.Left);
                runner.GUI.PointerReleased(x, y, GuiButton.Left);
                runner.WaitFor(s => !combo.DroppedDown && combo.Text == "超高速", "very fast selected");

                // 上下キーでも選べる
                runner.GUI.KeyPressed(ScreenKey.Down);
                runner.WaitFor(s => combo.Text == "神の領域", "instant selected");
                runner.GUI.KeyPressed(ScreenKey.Up);
                runner.WaitFor(s => combo.Text == "超高速", "very fast selected again");

                Click(runner, w, w.cmdOK);
                Assert.IsTrue(closed.Wait(ScreenScenarioRunner.Timeout), "closed");
                Assert.AreEqual(200, runner.GUI.MessageWait);
                Assert.AreEqual(200, config.MessageWait);
                StringAssert.Contains(File.ReadAllText(Path.Combine(workspace.AppPath, "srcs.json")), "\"MessageWait\": 200");
            }
        }

        [TestMethod]
        public void MessageWait_IsReadFromScenarioSrcIni()
        {
            // 元の SRC と同じく、シナリオのフォルダの Src.ini の [Option] MessageWait でメッセージのウェイトを決める
            workspace.Write(Path.Combine("scenario", "Src.ini"), "[Option]", "MessageWait=1000", "");
            var eve = workspace.Write(Path.Combine("scenario", "test.eve"), "プロローグ:", "Talk システム", "設定のテスト", "End", "Quit", "");
            using (var runner = new ScreenScenarioRunner(workspace.AppPath))
            {
                runner.Start(eve);
                runner.WaitForMessage();
                Assert.AreEqual(1000, runner.GUI.MessageWait);
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
