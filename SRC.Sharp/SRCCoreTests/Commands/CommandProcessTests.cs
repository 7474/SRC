using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Maps;
using SRCCore.TestLib;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.ProceedInput / Command.ProceedCommand のユニットテスト (Command.process.cs)
    ///
    /// テスト可能な範囲:
    /// - ProceedCommand: ViewMode フラグの処理 (GUI 依存の少ない部分)
    /// - ProceedInput: "マップコマンド" 状態でのクリック処理 (CommandState の変化のみ確認)
    /// - ProceedInput: "コマンド選択" / "移動後コマンド選択" での左クリック (CancelCommand 呼び出し)
    ///
    /// 注意: ProceedUnitSelect / ProceedCommandSelect 等の深い処理は GUI 依存が強いため、
    ///       ユニットなし・スペシャルケースに限定してテストする。
    /// </summary>
    [TestClass]
    public class CommandProcessTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // ProceedCommand — ViewMode
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_ByCancel_SetsViewModeToFalse()
        {
            // ViewMode=true かつ by_cancel=true → ViewMode が false になる
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.ProceedCommand(by_cancel: true);
            Assert.IsFalse(src.Commands.ViewMode);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_NotByCancel_ViewModeRemainsTrue()
        {
            // ViewMode=true かつ by_cancel=false → ViewMode は変わらない (early return)
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.ProceedCommand(by_cancel: false);
            Assert.IsTrue(src.Commands.ViewMode);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_ByCancel_CommandStateUnchanged()
        {
            // ViewMode のアーリーリターンにより CommandState は変化しない
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.CommandState = "ユニット選択";
            src.Commands.ProceedCommand(by_cancel: true);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeFalse_ByCancel_ViewModeRemainsDefault()
        {
            // ViewMode=false は変化しない
            var src = CreateSrc();
            src.Commands.ViewMode = false;
            src.Commands.CommandState = null;
            // CommandState=null → ProceedUnitSelect(unit=null) が呼ばれるが
            // MockGUI は ShowMapCommandMenu をデフォルト空実装するため完了する
            src.Commands.ProceedCommand(by_cancel: true);
            Assert.IsFalse(src.Commands.ViewMode);
        }

        // ──────────────────────────────────────────────
        // ProceedInput — "マップコマンド" 状態
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedInput_MapCommandState_LeftClick_SetsCommandStateToUnitSelect()
        {
            // "マップコマンド" + 左クリック → "ユニット選択"
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド";
            src.Commands.ProceedInput(GuiButton.Left, cell: null, unit: null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_MapCommandState_RightClick_SetsCommandStateToUnitSelect()
        {
            // "マップコマンド" + 右クリック → "ユニット選択"
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド";
            src.Commands.ProceedInput(GuiButton.Right, cell: null, unit: null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // ProceedInput — "コマンド選択" 状態
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedInput_CommandSelectState_LeftClick_NoUnit_CancelsToUnitSelect()
        {
            // "コマンド選択" + 左クリック + unit=null → CancelCommand() → "ユニット選択"
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "攻撃";
            src.Commands.ProceedInput(GuiButton.Left, cell: null, unit: null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_CommandSelectState_LeftClick_NoUnit_ClearsSelectedCommand()
        {
            // CancelCommand が呼ばれ SelectedCommand がクリアされる
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "攻撃";
            src.Commands.ProceedInput(GuiButton.Left, cell: null, unit: null);
            Assert.AreEqual("", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void ProceedInput_CommandSelectState_RightClick_CancelsToUnitSelect()
        {
            // "コマンド選択" + 右クリック → CancelCommand() → "ユニット選択"
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.ProceedInput(GuiButton.Right, cell: null, unit: null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // ProceedInput — "ユニット選択" 状態 (unit=null)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedInput_UnitSelectState_LeftClick_NoUnit_DoesNotCallProceedCommand()
        {
            // "ユニット選択" + 左クリック + unit=null → 分岐で unit==null なので ProceedCommand は呼ばれない
            var src = CreateSrc();
            src.Commands.CommandState = "ユニット選択";
            src.Commands.ProceedInput(GuiButton.Left, cell: null, unit: null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // ProceedInput — "ターゲット選択" 状態 (cell=null → マスク判定スキップ)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedInput_TargetSelectState_LeftClick_NullCell_DoesNotProceed()
        {
            // "ターゲット選択" + 左クリック + cell=null → ProceedCommand は呼ばれない
            var src = CreateSrc();
            src.Commands.CommandState = "ターゲット選択";
            src.Commands.ProceedInput(GuiButton.Left, cell: null, unit: null);
            Assert.AreEqual("ターゲット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_AfterMoveTargetSelectState_LeftClick_NullCell_DoesNotProceed()
        {
            // "移動後ターゲット選択" + 左クリック + cell=null → ProceedCommand は呼ばれない
            var src = CreateSrc();
            src.Commands.CommandState = "移動後ターゲット選択";
            src.Commands.ProceedInput(GuiButton.Left, cell: null, unit: null);
            Assert.AreEqual("移動後ターゲット選択", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // ProceedInput — NoneButton (イベント外の呼び出し)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedInput_NoneButton_DoesNotThrow()
        {
            // GuiButton.None を渡した場合は何もしない (左でも右でもない)
            var src = CreateSrc();
            src.Commands.CommandState = "ユニット選択";
            src.Commands.ProceedInput(GuiButton.None, cell: null, unit: null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }
    }
}
