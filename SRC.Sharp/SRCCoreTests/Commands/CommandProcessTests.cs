using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Maps;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.process.cs の ProceedInput / ProceedCommand のユニットテスト。
    ///
    /// テスト観点:
    /// - ProceedInput: CommandState による左クリック・右クリックの状態遷移
    /// - ProceedCommand: ViewMode フラグによる早期リターン
    /// </summary>
    [TestClass]
    public class CommandProcessTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ══════════════════════════════════════════════════════
        // ProceedInput — 左クリック (GuiButton.Left)
        // ══════════════════════════════════════════════════════

        [TestMethod]
        public void ProceedInput_LeftClick_MapCommandState_SetsUnitSelectState()
        {
            // "マップコマンド" で左クリック → "ユニット選択" に遷移する
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド";

            src.Commands.ProceedInput(GuiButton.Left, null, null);

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_LeftClick_UnitSelectState_NullUnit_StateUnchanged()
        {
            // "ユニット選択" で unit == null なら何もしない
            var src = CreateSrc();
            src.Commands.CommandState = "ユニット選択";

            src.Commands.ProceedInput(GuiButton.Left, null, null);

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_LeftClick_CommandSelectState_NullUnit_TransitionsToUnitSelect()
        {
            // "コマンド選択" で左クリック(unit==null) → CancelCommand() が呼ばれ "ユニット選択" になる
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "攻撃";

            src.Commands.ProceedInput(GuiButton.Left, null, null);

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_LeftClick_CommandSelectState_NullUnit_ClearsSelectedCommand()
        {
            // "コマンド選択" で左クリック(unit==null) → SelectedCommand がクリアされる
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "攻撃";

            src.Commands.ProceedInput(GuiButton.Left, null, null);

            Assert.AreEqual("", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void ProceedInput_LeftClick_TargetSelectState_NullCell_StateUnchanged()
        {
            // "ターゲット選択" で cell == null なら何もしない (MaskData チェック前に null ガード)
            var src = CreateSrc();
            src.Commands.CommandState = "ターゲット選択";

            src.Commands.ProceedInput(GuiButton.Left, null, null);

            Assert.AreEqual("ターゲット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_LeftClick_AfterMoveTargetSelectState_NullCell_StateUnchanged()
        {
            // "移動後ターゲット選択" で cell == null なら何もしない
            var src = CreateSrc();
            src.Commands.CommandState = "移動後ターゲット選択";

            src.Commands.ProceedInput(GuiButton.Left, null, null);

            Assert.AreEqual("移動後ターゲット選択", src.Commands.CommandState);
        }

        // ══════════════════════════════════════════════════════
        // ProceedInput — 右クリック (GuiButton.Right)
        // ══════════════════════════════════════════════════════

        [TestMethod]
        public void ProceedInput_RightClick_MapCommandState_SetsUnitSelectState()
        {
            // "マップコマンド" で右クリック → "ユニット選択" に遷移する
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド";

            src.Commands.ProceedInput(GuiButton.Right, null, null);

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_RightClick_CommandSelectState_TransitionsToUnitSelect()
        {
            // "コマンド選択" 以外の右クリック → CancelCommand() が呼ばれる
            // CancelCommand "コマンド選択" → "ユニット選択"
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "移動";

            src.Commands.ProceedInput(GuiButton.Right, null, null);

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_RightClick_NullCommandState_StateRemainsNull()
        {
            // null 状態 (= デフォルト) で右クリック → CancelCommand (no-op) → null のまま
            var src = CreateSrc();
            src.Commands.CommandState = null;

            src.Commands.ProceedInput(GuiButton.Right, null, null);

            Assert.IsNull(src.Commands.CommandState);
        }

        // ══════════════════════════════════════════════════════
        // ProceedCommand — ViewMode フラグ
        // ══════════════════════════════════════════════════════

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_NotByCancel_ViewModStaysTrue()
        {
            // ViewMode == true で by_cancel == false のとき、ViewMode は変わらず早期リターンする
            var src = CreateSrc();
            src.Commands.ViewMode = true;

            src.Commands.ProceedCommand(by_cancel: false);

            Assert.IsTrue(src.Commands.ViewMode);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_ByCancel_ViewModeFalse()
        {
            // ViewMode == true で by_cancel == true のとき、ViewMode が false になる
            var src = CreateSrc();
            src.Commands.ViewMode = true;

            src.Commands.ProceedCommand(by_cancel: true);

            Assert.IsFalse(src.Commands.ViewMode);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_NotByCancel_DoesNotLockGUI()
        {
            // ViewMode == true パスでは GUI.IsGUILocked に触れない (早期リターン)
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            mock.IsGUILocked = false;
            src.Commands.ViewMode = true;

            src.Commands.ProceedCommand(by_cancel: false);

            // IsGUILocked が変化していないことを確認
            Assert.IsFalse(mock.IsGUILocked);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_ByCancel_DoesNotLockGUI()
        {
            // ViewMode == true + by_cancel == true パスも早期リターンなので GUI.IsGUILocked に触れない
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            mock.IsGUILocked = false;
            src.Commands.ViewMode = true;

            src.Commands.ProceedCommand(by_cancel: true);

            Assert.IsFalse(mock.IsGUILocked);
        }
    }
}
