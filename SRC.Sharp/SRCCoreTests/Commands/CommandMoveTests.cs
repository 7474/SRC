using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.TestLib;
using SRCCore.Tests;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.unitmove.cs のユニットテスト
    /// StartMoveCommand, StartTeleportCommand, StartJumpCommand
    /// </summary>
    [TestClass]
    public class CommandMoveTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC
            {
                GUI = new MockGUI(),
                GUIMap = new MockGUIMap(),
                GUIStatus = new MockGUIStatus(),
            };
            src.Map.SetMapSize(5, 5);
            src.Map.MapFileName = "test.map";
            var unit = new Unit(src);
            unit.x = 1;
            unit.y = 1;
            src.Commands.SelectedUnit = unit;
            return src;
        }

        // ──────────────────────────────────────────────
        // StartMoveCommand: SelectedCommand が "移動" に設定される
        // (AreaInSpeed がマップの地形データ不足で例外を投げても、SelectedCommand は設定済み)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartMoveCommand_SetsSelectedCommandToMove()
        {
            var src = CreateSrc();
            ((MockGUI)src.GUI).CenterHandler = (_, __) => { };
            ((MockGUI)src.GUI).MaskScreenHandler = () => { };
            src.Commands.CommandState = "コマンド選択";

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            }
            catch { }

            Assert.AreEqual("移動", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void StartMoveCommand_MoveCmdIDWithMoveLabel_InvokesStartMoveNotShowArea()
        {
            // Label="移動" の UiCommand → StartMoveCommand が呼ばれる (SelectedCommand="移動" が設定)
            var src = CreateSrc();
            ((MockGUI)src.GUI).CenterHandler = (_, __) => { };
            ((MockGUI)src.GUI).MaskScreenHandler = () => { };
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "未設定";

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            }
            catch { }

            // StartMoveCommand は "移動" をセットする
            Assert.AreEqual("移動", src.Commands.SelectedCommand);
        }

        // ──────────────────────────────────────────────
        // StartTeleportCommand: SelectedCommand が "テレポート" に設定される
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartTeleportCommand_SetsSelectedCommandToTeleport()
        {
            var src = CreateSrc();
            ((MockGUI)src.GUI).CenterHandler = (_, __) => { };
            ((MockGUI)src.GUI).MaskScreenHandler = () => { };
            src.Commands.CommandState = "コマンド選択";

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.TeleportCmdID, "テレポート"));
            }
            catch { }

            Assert.AreEqual("テレポート", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void StartTeleportCommand_IsTriggeredByTeleportCmdID()
        {
            // TeleportCmdID = 1 で StartTeleportCommand が呼ばれる
            var src = CreateSrc();
            bool centerCalled = false;
            ((MockGUI)src.GUI).CenterHandler = (_, __) => { centerCalled = true; };
            ((MockGUI)src.GUI).MaskScreenHandler = () => { };
            src.Commands.CommandState = "コマンド選択";

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.TeleportCmdID, "テレポート"));
            }
            catch { }

            // SelectedCommand が設定されていることで StartTeleportCommand が呼ばれたことを確認
            Assert.AreEqual("テレポート", src.Commands.SelectedCommand);
        }

        // ──────────────────────────────────────────────
        // StartJumpCommand: SelectedCommand が "ジャンプ" に設定される
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartJumpCommand_SetsSelectedCommandToJump()
        {
            var src = CreateSrc();
            ((MockGUI)src.GUI).CenterHandler = (_, __) => { };
            ((MockGUI)src.GUI).MaskScreenHandler = () => { };
            src.Commands.CommandState = "コマンド選択";

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.JumpCmdID, "ジャンプ"));
            }
            catch { }

            Assert.AreEqual("ジャンプ", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void StartJumpCommand_IsTriggeredByJumpCmdID()
        {
            // JumpCmdID = 2 で StartJumpCommand が呼ばれる
            var src = CreateSrc();
            ((MockGUI)src.GUI).CenterHandler = (_, __) => { };
            ((MockGUI)src.GUI).MaskScreenHandler = () => { };
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "未設定";

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.JumpCmdID, "ジャンプ"));
            }
            catch { }

            Assert.AreEqual("ジャンプ", src.Commands.SelectedCommand);
        }

        // ──────────────────────────────────────────────
        // SelectedCommand の設定タイミング確認 (例外前に設定される)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MoveCommands_SelectedCommandSetBeforeMapOperations()
        {
            // 各移動コマンドは SelectedCommand を設定した後に Map 操作を行う
            // 例外が発生してもコマンドが記録される
            var testCases = new[] {
                (Command.MoveCmdID, "移動", "移動"),
                (Command.TeleportCmdID, "テレポート", "テレポート"),
                (Command.JumpCmdID, "ジャンプ", "ジャンプ"),
            };

            foreach (var (cmdId, label, expectedCommand) in testCases)
            {
                var src = CreateSrc();
                ((MockGUI)src.GUI).CenterHandler = (_, __) => { };
                ((MockGUI)src.GUI).MaskScreenHandler = () => { };
                src.Commands.CommandState = "コマンド選択";

                try { src.Commands.UnitCommand(new UiCommand(cmdId, label)); } catch { }

                Assert.AreEqual(expectedCommand, src.Commands.SelectedCommand,
                    $"cmdId={cmdId}, label={label}");
            }
        }
    }
}
