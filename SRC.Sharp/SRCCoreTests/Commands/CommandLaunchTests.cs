using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.TestLib;
using SRCCore.Tests;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.launch.cs のユニットテスト
    /// StartLaunchCommand
    /// </summary>
    [TestClass]
    public class CommandLaunchTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC
            {
                GUI = new MockGUI(),
                GUIMap = new MockGUIMap(),
                GUIStatus = new MockGUIStatus(),
            };
            var unit = new Unit(src);
            unit.x = 1;
            unit.y = 1;
            src.Commands.SelectedUnit = unit;
            return src;
        }

        // ──────────────────────────────────────────────
        // StartLaunchCommand: キャンセルパス (ListBox → 0)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartLaunchCommand_ListBoxCancel_CommandStateResetToUnitSelect()
        {
            // 搭載ユニットなしのとき ListBox は空リストで呼ばれ、0 を返すとキャンセル
            var src = CreateSrc();
            ((MockGUI)src.GUI).ListBoxHandler = (_) => 0;
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.LaunchCmdID, "発進"));

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void StartLaunchCommand_ListBoxCancel_SelectedCommandCleared()
        {
            var src = CreateSrc();
            ((MockGUI)src.GUI).ListBoxHandler = (_) => 0;
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "発進";

            src.Commands.UnitCommand(new UiCommand(Command.LaunchCmdID, "発進"));

            Assert.AreEqual("", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void StartLaunchCommand_EmptyOnBoard_ListBoxCalledWithEmptyList()
        {
            // 搭載ユニットが空のとき ListBox は空 items で呼ばれる
            var src = CreateSrc();
            bool listBoxCalled = false;
            ((MockGUI)src.GUI).ListBoxHandler = (args) =>
            {
                listBoxCalled = true;
                // args.Items には搭載ユニット分の行がある（ここでは0個）
                return 0;
            };
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.LaunchCmdID, "発進"));

            Assert.IsTrue(listBoxCalled);
        }

        [TestMethod]
        public void StartLaunchCommand_ListBoxCancel_DoesNotSetSelectedCommand()
        {
            // キャンセル時は "発進" が SelectedCommand に設定されない
            // (SelectedCommand = "発進" は ListBox が 0 以外を返した後に設定される)
            var src = CreateSrc();
            ((MockGUI)src.GUI).ListBoxHandler = (_) => 0;
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "";

            src.Commands.UnitCommand(new UiCommand(Command.LaunchCmdID, "発進"));

            Assert.AreNotEqual("発進", src.Commands.SelectedCommand);
        }
    }
}
