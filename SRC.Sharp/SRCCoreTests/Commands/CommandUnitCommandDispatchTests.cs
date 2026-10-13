using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.UnitCommand() のラベル別ディスパッチと StartTalkCommand / StartJumpCommand の追加テスト。
    /// </summary>
    [TestClass]
    public class CommandUnitCommandDispatchTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Commands.SelectedUnit = new Unit(src);
            return src;
        }

        private static void TryUnitCommand(SRC src, UiCommand cmd)
        {
            try { src.Commands.UnitCommand(cmd); } catch { }
        }

        [TestMethod]
        public void UnitCommand_MoveCmdIDWithOtherLabel_SelectsMoveRange()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            TryUnitCommand(src, new UiCommand(Command.MoveCmdID, "移動範囲"));
            Assert.AreEqual("移動範囲", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void UnitCommand_AttackCmdIDWithOtherLabel_SelectsAttackRange()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            TryUnitCommand(src, new UiCommand(Command.AttackCmdID, "射程範囲"));
            Assert.AreEqual("射程範囲", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void UnitCommand_TalkCmdID_SetsSelectedCommandToTalk()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            TryUnitCommand(src, new UiCommand(Command.TalkCmdID, "会話"));
            Assert.AreEqual("会話", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void UnitCommand_UnknownIdInPostMoveState_ReturnsWithoutStateChange()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "移動後コマンド選択";
            src.Commands.UnitCommand(new UiCommand(9999, "不明"));
            Assert.AreEqual("移動後コマンド選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void UnitCommand_UnknownIdInNormalState_DoesNothing()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.UnitCommand(new UiCommand(9999, "不明"));
            Assert.AreEqual("コマンド選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void UnitCommand_JumpCmdIDInPostMoveState_SetsSelectedCommand()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "移動後コマンド選択";
            TryUnitCommand(src, new UiCommand(Command.JumpCmdID, "ジャンプ"));
            Assert.AreEqual("ジャンプ", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void UnitCommand_PreservesPreviousCommandWithoutThrowingForUnknownId()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "移動";
            src.Commands.UnitCommand(new UiCommand(9999, "不明"));
            Assert.AreEqual("移動", src.Commands.SelectedCommand);
        }
    }
}
