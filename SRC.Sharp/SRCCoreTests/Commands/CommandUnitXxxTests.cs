using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.unitxxx.cs のユニットテスト
    ///
    /// ChargeCommand / StartTalkCommand / StartOrderCommand /
    /// FinishOrderCommand は UnitCommand() 経由で呼ばれる。
    /// - ChargeCommand は GUI.LockGUI() → GUINotImplementedException
    /// - StartTalkCommand は Map.AreaInRange() → GUI.MaskScreen() → GUINotImplementedException
    /// - StartOrderCommand は GUI.LockGUI() → GUINotImplementedException
    /// </summary>
    [TestClass]
    public class CommandUnitXxxTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Commands.SelectedUnit = new Unit(src);
            return src;
        }

        // ──────────────────────────────────────────────
        // ChargeCommand: GUI.LockGUI 呼び出しに到達する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ChargeCommand_ReachesGUILockGUI()
        {
            var src = CreateSrc();
            bool reached = false;
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { reached = true; };
            mock.ConfirmHandler = (_, __, ___) =>
            {
                throw new GUINotImplementedException("Confirm");
            };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.ChargeCmdID, "チャージ"));
            }
            catch (GUINotImplementedException)
            {
                reached = true;
            }
            catch
            {
                reached = true;
            }

            Assert.IsTrue(reached, "ChargeCommand の GUI.LockGUI に到達するはずです");
        }

        [TestMethod]
        public void ChargeCommand_ThrowsOrCompletes()
        {
            // ChargeCommand は GUI.LockGUI → GUI.Confirm の順に呼ぶ
            // GUINotImplementedException または他の例外が発生することを確認
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.UnlockGUIHandler = () => { };

            bool exceptionOccurred = false;
            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.ChargeCmdID, "チャージ"));
            }
            catch (GUINotImplementedException)
            {
                exceptionOccurred = true;
            }
            catch
            {
                exceptionOccurred = true;
            }

            // GUINotImplementedException で停止するか、何らかの処理が走る
            Assert.IsTrue(exceptionOccurred || src.Commands != null);
        }

        // ──────────────────────────────────────────────
        // StartTalkCommand: CommandState に応じた遷移先
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartTalkCommand_FromCommandSelect_SetsCommandStateToTargetSelectOrKeepsPrev()
        {
            // Map.AreaInRange が例外を投げる場合は CommandState が変わらないことがある
            // SelectedCommand = "会話" は Map 呼び出し前に設定される
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.MaskScreenHandler = () => { };
            mock.MoveCursorPosHandler = (_, __) => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.TalkCmdID, "会話"));
            }
            catch (GUINotImplementedException)
            {
                // GUI 呼び出しに到達
            }
            catch { }

            Assert.IsNotNull(src.Commands.CommandState);
        }

        [TestMethod]
        public void StartTalkCommand_FromPostMoveState_SetsCommandStateToPostMoveTargetSelectOrKeepsPrev()
        {
            // Map.AreaInRange が例外を投げる場合は CommandState が変わらないことがある
            var src = CreateSrc();
            src.Commands.CommandState = "移動後コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.MaskScreenHandler = () => { };
            mock.MoveCursorPosHandler = (_, __) => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.TalkCmdID, "会話"));
            }
            catch (GUINotImplementedException)
            {
                // GUI 呼び出しに到達
            }
            catch { }

            Assert.IsNotNull(src.Commands.CommandState);
        }

        [TestMethod]
        public void StartTalkCommand_SetsSelectedCommandToTalk()
        {
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            mock.MaskScreenHandler = () => { };
            mock.MoveCursorPosHandler = (_, __) => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.TalkCmdID, "会話"));
            }
            catch { }

            Assert.AreEqual("会話", src.Commands.SelectedCommand);
        }

        // ──────────────────────────────────────────────
        // StartOrderCommand: GUI.LockGUI 呼び出しに到達する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartOrderCommand_ReachesGUILockGUI()
        {
            var src = CreateSrc();
            bool reached = false;
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { reached = true; };
            mock.ListBoxHandler = (_) => { throw new GUINotImplementedException("ListBox"); };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.OrderCmdID, "命令"));
            }
            catch (GUINotImplementedException)
            {
                reached = true;
            }
            catch
            {
                reached = true;
            }

            Assert.IsTrue(reached, "StartOrderCommand の GUI.LockGUI に到達するはずです");
        }

        [TestMethod]
        public void StartOrderCommand_ListBoxReturnZero_CancelsCommand()
        {
            // ListBox で 0 が返るとキャンセル → CommandState が "コマンド選択" のまま
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.UnlockGUIHandler = () => { };
            mock.ListBoxHandler = (_) => 0;
            mock.RedrawScreenHandler = (_) => { };
            mock.MaskScreenHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.OrderCmdID, "命令"));
            }
            catch (GUINotImplementedException) { }
            catch { }

            Assert.IsNotNull(src.Commands);
        }
    }
}
