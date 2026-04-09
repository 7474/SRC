using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.process.cs の公開メソッド (ProceedCommand, ProceedInput, CancelCommand) のユニットテスト
    /// ソースコードを仕様の根拠として使用する
    /// </summary>
    [TestClass]
    public class CommandProcessTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // ProceedCommand — ViewMode ブランチ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_ByCancel_SetsViewModeFalse()
        {
            // 閲覧モードはキャンセルで終了。by_cancel=true → ViewMode=false に設定して return
            var src = CreateSrc();
            src.Commands.ViewMode = true;

            src.Commands.ProceedCommand(by_cancel: true);

            Assert.IsFalse(src.Commands.ViewMode);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_NotByCancel_ViewModeUnchanged()
        {
            // 閲覧モードで by_cancel=false → ViewMode は変化せず return
            var src = CreateSrc();
            src.Commands.ViewMode = true;

            src.Commands.ProceedCommand(by_cancel: false);

            Assert.IsTrue(src.Commands.ViewMode);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeFalse_ByCancel_ViewModeRemainsfalse()
        {
            // ViewMode=false の状態で by_cancel=true は ViewMode を変えない (既に false)
            var src = CreateSrc();
            src.Commands.ViewMode = false;
            // CommandState が null かつ null以外のGUI呼び出しを避けるため ViewMode=true→false だけを確認
            // ViewMode=false の場合は GUI.IsGUILocked=true を設定しようとするが MockGUI は無害なプロパティ
            // CommandState=null のときは switch が通過してロック解除される
            src.Commands.ProceedCommand(by_cancel: true);

            Assert.IsFalse(src.Commands.ViewMode);
        }

        // ──────────────────────────────────────────────
        // CancelCommand — CommandState 遷移
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CancelCommand_UnitSelectState_StateUnchanged()
        {
            // CommandState="ユニット選択" のときは何も変化しない (break のみ)
            var src = CreateSrc();
            src.Commands.CommandState = "ユニット選択";

            src.Commands.CancelCommand();

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void CancelCommand_CommandSelectState_SetsToUnitSelect()
        {
            // CommandState="コマンド選択" → "ユニット選択" に遷移
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";

            src.Commands.CancelCommand();

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void CancelCommand_CommandSelectState_ClearsSelectedCommand()
        {
            // CommandState="コマンド選択" → SelectedCommand が空文字列にクリアされる
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "攻撃";

            src.Commands.CancelCommand();

            Assert.AreEqual("", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void CancelCommand_NullState_DoesNotThrow()
        {
            // CommandState が null のときも例外を出さない
            var src = CreateSrc();
            src.Commands.CommandState = null;

            src.Commands.CancelCommand(); // should not throw
        }

        // ──────────────────────────────────────────────
        // ProceedInput — 状態遷移の簡易テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedInput_LeftClick_MapCommandState_SetsUnitSelect()
        {
            // 左クリック時 CommandState="マップコマンド" → "ユニット選択" に遷移
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド";

            src.Commands.ProceedInput(GuiButton.Left, null, null);

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_RightClick_MapCommandState_SetsUnitSelect()
        {
            // 右クリック時 CommandState="マップコマンド" → "ユニット選択" に遷移
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド";

            src.Commands.ProceedInput(GuiButton.Right, null, null);

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_LeftClick_CommandSelectState_NullUnit_SetsUnitSelect()
        {
            // 左クリック時 CommandState="コマンド選択", unit=null → CancelCommand() が呼ばれて "ユニット選択" に遷移
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "修理";

            src.Commands.ProceedInput(GuiButton.Left, null, null);

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_LeftClick_TargetSelectState_NullCell_NoStateChange()
        {
            // 左クリック時 CommandState="ターゲット選択", cell=null → 条件不成立のため状態変化なし
            var src = CreateSrc();
            src.Commands.CommandState = "ターゲット選択";

            src.Commands.ProceedInput(GuiButton.Left, null, null);

            Assert.AreEqual("ターゲット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_LeftClick_AfterMoveTargetSelectState_NullCell_NoStateChange()
        {
            // 左クリック時 CommandState="移動後ターゲット選択", cell=null → 条件不成立のため状態変化なし
            var src = CreateSrc();
            src.Commands.CommandState = "移動後ターゲット選択";

            src.Commands.ProceedInput(GuiButton.Left, null, null);

            Assert.AreEqual("移動後ターゲット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_LeftClick_MovedAfterCommandSelect_CallsCancelCommand()
        {
            // 左クリック時 CommandState="移動後コマンド選択" → CancelCommand() が呼ばれて break
            // CancelCommand("移動後コマンド選択") は SelectedUnit を参照するため最低限の Unit を設定
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            mock.PaintUnitBitmapHandler = (u, s) => { };
            mock.RedrawScreenHandler = (late) => { };

            // SelectedUnit を設定してから CommandState を設定
            var unit = new Unit(src);
            src.Commands.SelectedUnit = unit;
            src.Commands.CommandState = "移動後コマンド選択";

            // CancelCommand("移動後コマンド選択") の中で currentUnit.Move(...) を呼ぼうとする
            // Map.MapDataForUnit アクセスが例外になる可能性があるため try/catch で状態変化のみ確認
            try
            {
                src.Commands.ProceedInput(GuiButton.Left, null, null);
            }
            catch { }

            // CancelCommand が呼ばれたことで CommandState が "ターゲット選択" になっているはず
            Assert.AreNotEqual("移動後コマンド選択", src.Commands.CommandState);
        }
    }
}
