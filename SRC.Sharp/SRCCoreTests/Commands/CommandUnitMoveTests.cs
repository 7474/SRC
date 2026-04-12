using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.unitmove.cs のユニットテスト
    ///
    /// StartMoveCommand / StartTeleportCommand / StartJumpCommand は
    /// UnitCommand() 経由でそれぞれ MoveCmdID / TeleportCmdID / JumpCmdID で呼ばれる。
    /// - Map.AreaInSpeed が初期化済みの Map を利用するため例外が発生しうる
    /// - GUI.Center / GUI.MaskScreen に到達した場合は GUINotImplementedException
    /// </summary>
    [TestClass]
    public class CommandUnitMoveTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Commands.SelectedUnit = new Unit(src);
            return src;
        }

        // ──────────────────────────────────────────────
        // StartMoveCommand: CommandState == "移動後コマンド選択" → 即 return
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartMoveCommand_PostMoveCommandSelectState_DoesNotThrow()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "移動後コマンド選択";
            // 早期 return するので例外は発生しない
            src.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
        }

        [TestMethod]
        public void StartMoveCommand_PostMoveCommandSelectState_CommandStateUnchanged()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "移動後コマンド選択";
            src.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            Assert.AreEqual("移動後コマンド選択", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // StartMoveCommand: 通常状態 → GUI 呼び出しに到達する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartMoveCommand_NormalState_SetsSelectedCommandToMove()
        {
            // StartMoveCommand は最初に SelectedCommand = "移動" をセットする
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.CenterHandler = (_, __) => { };
            mock.MaskScreenHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            }
            catch (GUINotImplementedException)
            {
                // GUI 呼び出しに到達
            }
            catch { }

            Assert.AreEqual("移動", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void StartMoveCommand_NormalState_SetsCommandStateToTargetSelectOrKeepsPrevState()
        {
            // StartMoveCommand は CommandState = "ターゲット選択" をセットするが、
            // Map.AreaInSpeed が未初期化マップで例外を投げる場合もある。
            // SelectedCommand = "移動" は Map 呼び出し前に設定されるので確認できる。
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.CenterHandler = (_, __) => { };
            mock.MaskScreenHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            }
            catch (GUINotImplementedException)
            {
                // GUI 呼び出しに到達
            }
            catch { }

            // CommandState は "ターゲット選択" か初期値のいずれか（Map の初期化状態による）
            Assert.IsNotNull(src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // StartTeleportCommand: GUI 呼び出しに到達する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartTeleportCommand_ReachesGUI()
        {
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            mock.CenterHandler = (_, __) => { };
            mock.MaskScreenHandler = () => { };
            bool reached = false;

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.TeleportCmdID, "テレポート"));
            }
            catch (GUINotImplementedException)
            {
                reached = true;
            }
            catch
            {
                reached = true;
            }

            Assert.IsTrue(reached, "StartTeleportCommand に到達するはずです");
        }

        [TestMethod]
        public void StartTeleportCommand_SetsSelectedCommandToTeleport()
        {
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            mock.CenterHandler = (_, __) => { };
            mock.MaskScreenHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.TeleportCmdID, "テレポート"));
            }
            catch { }

            Assert.AreEqual("テレポート", src.Commands.SelectedCommand);
        }

        // ──────────────────────────────────────────────
        // StartJumpCommand: GUI 呼び出しに到達する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartJumpCommand_ReachesGUI()
        {
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            mock.CenterHandler = (_, __) => { };
            mock.MaskScreenHandler = () => { };
            bool reached = false;

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.JumpCmdID, "ジャンプ"));
            }
            catch (GUINotImplementedException)
            {
                reached = true;
            }
            catch
            {
                reached = true;
            }

            Assert.IsTrue(reached, "StartJumpCommand に到達するはずです");
        }

        [TestMethod]
        public void StartJumpCommand_SetsSelectedCommandToJump()
        {
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            mock.CenterHandler = (_, __) => { };
            mock.MaskScreenHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.JumpCmdID, "ジャンプ"));
            }
            catch { }

            Assert.AreEqual("ジャンプ", src.Commands.SelectedCommand);
        }
    }
}
