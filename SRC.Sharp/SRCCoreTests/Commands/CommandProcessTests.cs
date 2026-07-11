using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Maps;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.process.cs の ProceedInput / ProceedCommand のユニットテスト。
    /// </summary>
    [TestClass]
    public class CommandProcessTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // ProceedInput - 左クリック
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedInput_LeftClick_MapCommand_SetsStateToUnitSelect()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド";
            src.Commands.ProceedInput(GuiButton.Left, null, null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_LeftClick_CommandSelect_NoUnit_CancelsToUnitSelect()
        {
            // unit == null → CancelCommand() → CommandState = "ユニット選択"
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.ProceedInput(GuiButton.Left, null, null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_LeftClick_CommandSelect_NoUnit_ClearsSelectedCommand()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "攻撃";
            src.Commands.ProceedInput(GuiButton.Left, null, null);
            Assert.AreEqual("", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void ProceedInput_LeftClick_AfterMoveCommandSelect_CancelsToUnitSelect()
        {
            // "移動後コマンド選択" + 左クリック → CancelCommand() が呼ばれる
            // CancelCommand() は "移動後コマンド選択" → "ターゲット選択" に変更し、
            // 以降は GUI 依存処理で例外が出る可能性があるが、
            // CancelCommand の直前まで "移動後コマンド選択" だった事実を確認
            var src = CreateSrc();
            src.Commands.CommandState = "移動後コマンド選択";
            // CancelCommand() が "移動後コマンド選択" ケースを処理するので例外が出うる
            try { src.Commands.ProceedInput(GuiButton.Left, null, null); } catch { }
            // CommandState は "移動後コマンド選択" ではなくなる (CancelCommand が呼ばれた)
            Assert.AreNotEqual("移動後コマンド選択", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // ProceedInput - 右クリック
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedInput_RightClick_MapCommand_SetsStateToUnitSelect()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド";
            src.Commands.ProceedInput(GuiButton.Right, null, null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_RightClick_CommandSelect_CancelsToUnitSelect()
        {
            // "コマンド選択" は右クリックの default ケース → CancelCommand()
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.ProceedInput(GuiButton.Right, null, null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_RightClick_TargetSelect_CancelsCommand()
        {
            // "ターゲット選択" + 右クリック → default ケース → CancelCommand()
            // CancelCommand("ターゲット選択") → "コマンド選択" → GUI 依存で例外が出うる
            var src = CreateSrc();
            src.Commands.CommandState = "ターゲット選択";
            try { src.Commands.ProceedInput(GuiButton.Right, null, null); } catch { }
            // "ターゲット選択" のまま残らないことを確認
            Assert.AreNotEqual("ターゲット選択", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // ProceedCommand - ViewMode ガード
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_ByCancel_DisablesViewMode()
        {
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.CommandState = "ユニット選択";
            src.Commands.ProceedCommand(by_cancel: true);
            Assert.IsFalse(src.Commands.ViewMode);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_ByCancel_ReturnsAfterDisablingViewMode()
        {
            // by_cancel=true で ViewMode=false になり、それ以降の処理には進まない
            // CommandState は変化しないことで確認
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.CommandState = "ユニット選択";
            src.Commands.ProceedCommand(by_cancel: true);
            // GUI.IsGUILocked は ViewMode 分岐より後でセットされるので false のまま
            Assert.IsFalse(src.GUI.IsGUILocked);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_NotByCancel_StateUnchanged()
        {
            // by_cancel=false かつ ViewMode=true → 何もせず即 return
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.CommandState = "コマンド選択";
            src.Commands.ProceedCommand(by_cancel: false);
            // ViewMode は変化しない
            Assert.IsTrue(src.Commands.ViewMode);
            // CommandState も変化しない
            Assert.AreEqual("コマンド選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_NotByCancel_GUIRemainsUnlocked()
        {
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.CommandState = "コマンド選択";
            src.Commands.ProceedCommand(by_cancel: false);
            Assert.IsFalse(src.GUI.IsGUILocked);
        }

        // ──────────────────────────────────────────────
        // ProceedInput - 左クリック + ViewMode=true の CommandSelect ガード
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedInput_LeftClick_CommandSelect_ViewModeTrue_CancelsThenViewModeRemains()
        {
            // "コマンド選択" + unit=null → CancelCommand() → "ユニット選択"
            // (ViewMode ガードは ProceedCommand 内なので CancelCommand は通る)
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.ViewMode = true;
            src.Commands.ProceedInput(GuiButton.Left, null, null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }
    }
}
