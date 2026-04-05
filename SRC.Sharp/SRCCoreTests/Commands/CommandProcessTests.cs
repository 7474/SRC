using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.process.cs で定義されている ProceedCommand / ProceedInput / CancelCommand の
    /// ユニットテスト。GUI 依存の深いパスは対象外とし、早期リターン可能なパスと
    /// 純粋な状態遷移を検証する。
    /// </summary>
    [TestClass]
    public class CommandProcessTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // ProceedCommand — ViewMode = true のとき早期リターン
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_ByCancel_SetsViewModeFalse()
        {
            // by_cancel = true のとき ViewMode を false にして早期リターンする
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.ProceedCommand(by_cancel: true);
            Assert.IsFalse(src.Commands.ViewMode);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_NotByCancel_ViewModeRemainsTrue()
        {
            // by_cancel = false のとき ViewMode は変更されずに早期リターンする
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.ProceedCommand(by_cancel: false);
            Assert.IsTrue(src.Commands.ViewMode);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_DoesNotThrow()
        {
            // ViewMode = true のとき GUI ハンドラ未設定でも例外が発生しないことを確認
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.ProceedCommand(); // デフォルト引数を使用
            // 到達できれば合格
        }

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_ByCancel_CommandStateUnchanged()
        {
            // ViewMode 早期リターンでは CommandState は変更されない
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.CommandState = "テスト状態";
            src.Commands.ProceedCommand(by_cancel: true);
            Assert.AreEqual("テスト状態", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // ProceedInput — マップコマンド状態でのボタン入力
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedInput_LeftButton_MapCommandState_SetsCommandStateToUnitSelect()
        {
            // 左クリック × "マップコマンド" → CommandState を "ユニット選択" に変更
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド";
            src.Commands.ProceedInput(GuiButton.Left, null, null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_RightButton_MapCommandState_SetsCommandStateToUnitSelect()
        {
            // 右クリック × "マップコマンド" → CommandState を "ユニット選択" に変更
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド";
            src.Commands.ProceedInput(GuiButton.Right, null, null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_LeftButton_MapCommandState_DoesNotThrow()
        {
            // 左クリック × "マップコマンド" で GUI ハンドラ未設定でも例外なし
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド";
            src.Commands.ProceedInput(GuiButton.Left, null, null);
            // 到達できれば合格
        }

        // ──────────────────────────────────────────────
        // CancelCommand — コマンド状態に応じた状態遷移
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CancelCommand_WhenCommandStateIsUnitSelect_DoesNotThrow()
        {
            // "ユニット選択" 状態のキャンセルは何もせずに返る (break のみ)
            var src = CreateSrc();
            src.Commands.CommandState = "ユニット選択";
            src.Commands.CancelCommand();
            // 到達できれば合格
        }

        [TestMethod]
        public void CancelCommand_WhenCommandStateIsUnitSelect_CommandStateUnchanged()
        {
            // "ユニット選択" 状態では CommandState が変更されないことを確認
            var src = CreateSrc();
            src.Commands.CommandState = "ユニット選択";
            src.Commands.CancelCommand();
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void CancelCommand_WhenCommandStateIsCommandSelect_SetsCommandStateToUnitSelect()
        {
            // "コマンド選択" 状態のキャンセルは CommandState を "ユニット選択" に変更する
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.CancelCommand();
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void CancelCommand_WhenCommandStateIsCommandSelect_ClearsSelectedCommand()
        {
            // "コマンド選択" 状態のキャンセルは SelectedCommand を空文字列にリセットする
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "攻撃";
            src.Commands.CancelCommand();
            Assert.AreEqual("", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void CancelCommand_WhenCommandStateIsCommandSelect_DoesNotThrow()
        {
            // "コマンド選択" 状態のキャンセルで GUI ハンドラ未設定でも例外なし
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.CancelCommand();
            // 到達できれば合格
        }

        [TestMethod]
        public void CancelCommand_WhenCommandStateIsNull_DoesNotThrow()
        {
            // CommandState が null のとき ("" として扱われる) も例外なし
            var src = CreateSrc();
            src.Commands.CommandState = null;
            src.Commands.CancelCommand();
            // 到達できれば合格
        }
    }
}
