using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Maps;
using SRCCore.TestLib;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.process.cs: ProceedInput / ProceedCommand のユニットテスト。
    ///
    /// テスト対象:
    ///   - ProceedInput: GuiButton x CommandState の状態遷移
    ///   - ProceedCommand: ViewMode による早期終了
    /// </summary>
    [TestClass]
    public class CommandProcessInputTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // ProceedInput — 左クリック: CommandState == "マップコマンド"
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedInput_LeftClick_MapCommandState_SetsUnitSelectState()
        {
            // "マップコマンド" で左クリック → "ユニット選択" に遷移
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド";
            src.Commands.ProceedInput(GuiButton.Left, null, null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // ProceedInput — 右クリック: CommandState == "マップコマンド"
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedInput_RightClick_MapCommandState_SetsUnitSelectState()
        {
            // "マップコマンド" で右クリック → "ユニット選択" に遷移
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド";
            src.Commands.ProceedInput(GuiButton.Right, null, null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // ProceedInput — 左クリック: CommandState == "ユニット選択", unit == null
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedInput_LeftClick_UnitSelectState_NullUnit_StateUnchanged()
        {
            // "ユニット選択" かつ unit が null の場合は ProceedCommand を呼ばない
            var src = CreateSrc();
            src.Commands.CommandState = "ユニット選択";
            src.Commands.ProceedInput(GuiButton.Left, null, null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // ProceedInput — 右クリック: CommandState == "コマンド選択"
        // → CancelCommand が呼ばれ "ユニット選択" に戻る
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedInput_RightClick_CommandSelectState_CancelsToUnitSelect()
        {
            // "コマンド選択" で右クリック → CancelCommand → "ユニット選択"
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "攻撃";
            src.Commands.ProceedInput(GuiButton.Right, null, null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_RightClick_CommandSelectState_ClearsSelectedCommand()
        {
            // CancelCommand は SelectedCommand を空にリセットする
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "移動";
            src.Commands.ProceedInput(GuiButton.Right, null, null);
            Assert.AreEqual("", src.Commands.SelectedCommand);
        }

        // ──────────────────────────────────────────────
        // ProceedInput — 左クリック: CommandState == "ターゲット選択", cell == null
        // cell が null の場合は ProceedCommand を呼ばない
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedInput_LeftClick_TargetSelectState_NullCell_StateUnchanged()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "ターゲット選択";
            src.Commands.ProceedInput(GuiButton.Left, null, null);
            Assert.AreEqual("ターゲット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_LeftClick_AfterMoveTargetSelectState_NullCell_StateUnchanged()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "移動後ターゲット選択";
            src.Commands.ProceedInput(GuiButton.Left, null, null);
            Assert.AreEqual("移動後ターゲット選択", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // ProceedInput — 左クリック: CommandState == "コマンド選択", unit == null
        // CancelCommand が呼ばれるが ProceedCommand は呼ばれない
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedInput_LeftClick_CommandSelectState_NullUnit_CancelsToUnitSelect()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "攻撃";
            src.Commands.ProceedInput(GuiButton.Left, null, null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_LeftClick_CommandSelectState_NullUnit_ClearsSelectedCommand()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "移動";
            src.Commands.ProceedInput(GuiButton.Left, null, null);
            Assert.AreEqual("", src.Commands.SelectedCommand);
        }

        // ──────────────────────────────────────────────
        // ProceedInput — 右クリック: default (不明な CommandState)
        // → CancelCommand が呼ばれる
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedInput_RightClick_UnknownState_CancelsToUnitSelect()
        {
            // 不明な状態での右クリック → CancelCommand
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.ProceedInput(GuiButton.Right, null, null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // ProceedCommand — ViewMode = true
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_ByCancel_ExitsViewMode()
        {
            // 閲覧モードでキャンセル操作 → ViewMode が false になる
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.ProceedCommand(by_cancel: true);
            Assert.IsFalse(src.Commands.ViewMode);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_NotByCancel_ViewModeRemainsTrue()
        {
            // 閲覧モードで通常入力 → ViewMode は変わらない
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.ProceedCommand(by_cancel: false);
            Assert.IsTrue(src.Commands.ViewMode);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_IsGUILocked_NotSet()
        {
            // ViewMode=true の早期 return ではロックを取得しないことを確認
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            src.Commands.ViewMode = true;
            mock.IsGUILocked = false;
            src.Commands.ProceedCommand(by_cancel: false);
            // early return なので IsGUILocked はセットされない
            Assert.IsFalse(mock.IsGUILocked);
        }
    }
}
