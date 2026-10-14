using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.process.cs の ProceedInput / ProceedCommand / CancelCommand の追加テスト。
    /// </summary>
    [TestClass]
    public class CommandProcessExtraTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        [TestMethod]
        public void ProceedInput_LeftClick_NullState_ViewMode_StaysViewMode()
        {
            var src = CreateSrc();
            src.Commands.CommandState = null;
            src.Commands.ViewMode = true;

            src.Commands.ProceedInput(GuiButton.Left, null, null);

            Assert.IsTrue(src.Commands.ViewMode);
        }

        [TestMethod]
        public void ProceedInput_RightClick_MapCommandState_DoesNotClearSelectedCommand()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド";
            src.Commands.SelectedCommand = "攻撃";

            src.Commands.ProceedInput(GuiButton.Right, null, null);

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
            Assert.AreEqual("攻撃", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void ProceedInput_LeftClick_MapCommandState_DoesNotClearSelectedCommand()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド";
            src.Commands.SelectedCommand = "攻撃";

            src.Commands.ProceedInput(GuiButton.Left, null, null);

            Assert.AreEqual("攻撃", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void ProceedCommand_UnknownState_ClearsScenarioFinishedFlag()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "未定義";
            src.IsScenarioFinished = true;
            src.IsCanceled = true;

            src.Commands.ProceedCommand();

            Assert.IsFalse(src.IsScenarioFinished);
            Assert.IsFalse(src.IsCanceled);
            Assert.IsFalse(src.GUI.IsGUILocked);
        }

        [TestMethod]
        public void ProceedCommand_ViewMode_DoesNotResetScenarioFlags()
        {
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.IsScenarioFinished = true;

            src.Commands.ProceedCommand();

            Assert.IsTrue(src.IsScenarioFinished);
        }

        [TestMethod]
        public void CancelCommand_EmptyState_DoesNotThrowAndKeepsCommand()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "";
            src.Commands.SelectedCommand = "移動";

            src.Commands.CancelCommand();

            Assert.AreEqual("移動", src.Commands.SelectedCommand);
            Assert.AreEqual("", src.Commands.CommandState);
        }
    }
}
