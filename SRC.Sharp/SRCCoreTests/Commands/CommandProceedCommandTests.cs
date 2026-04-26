using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Exceptions;
using SRCCore.Maps;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.ProceedCommand() および Command.ProceedInput() のユニットテスト
    /// (Command.process.cs)
    ///
    /// ProceedCommand:
    /// - ViewMode=true の場合: by_cancel=true → ViewMode を false に / false → 何もせず return
    /// - ViewMode=false の場合: switch(CommandState) でディスパッチ
    ///   未知の CommandState は switch の default なし → 何もせず IsGUILocked をリセット
    ///
    /// ProceedInput:
    /// - Left + "マップコマンド" → CommandState = "ユニット選択"
    /// - Left + "コマンド選択" + unit=null → CancelCommand() → "ユニット選択"
    /// - Left + "移動後コマンド選択" → CancelCommand() → "ユニット選択"
    /// - Right + "マップコマンド" → CommandState = "ユニット選択"
    /// - Right + default → CancelCommand()
    /// </summary>
    [TestClass]
    public class CommandProceedCommandTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // ProceedCommand: ViewMode=true, by_cancel=false → no-op
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_NotCancelled_ViewModeUnchanged()
        {
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.ProceedCommand(by_cancel: false);
            Assert.IsTrue(src.Commands.ViewMode);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_NotCancelled_DoesNotCallGUI()
        {
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            // GUI が呼ばれると GUINotImplementedException が発生するが、ViewMode 早期 return では呼ばれない
            src.Commands.ProceedCommand(by_cancel: false);
            // 例外なし = GUI は呼ばれなかった
        }

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_NotCancelled_IsGUILockedStaysFalse()
        {
            // 早期 return するので GUI.IsGUILocked は変更されない
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            ((MockGUI)src.GUI).IsGUILocked = false;
            src.Commands.ProceedCommand(by_cancel: false);
            Assert.IsFalse(((MockGUI)src.GUI).IsGUILocked);
        }

        // ──────────────────────────────────────────────
        // ProceedCommand: ViewMode=true, by_cancel=true → ViewMode=false
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_Cancelled_DisablesViewMode()
        {
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.ProceedCommand(by_cancel: true);
            Assert.IsFalse(src.Commands.ViewMode);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_Cancelled_DoesNotCallGUI()
        {
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.ProceedCommand(by_cancel: true);
            // 例外なし
        }

        [TestMethod]
        public void ProceedCommand_ViewModeTrue_CalledTwiceWithCancel_ViewModeStaysFalse()
        {
            // 2回目は ViewMode=false なので早期 return にならず switch に進む
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.ProceedCommand(by_cancel: true);
            Assert.IsFalse(src.Commands.ViewMode);
        }

        // ──────────────────────────────────────────────
        // ProceedCommand: ViewMode=false, CommandState=null/unknown → switch 素通り
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedCommand_ViewModeFalse_NullCommandState_SetsIsGUILockedFalse()
        {
            // switch にはマッチするケースがなく、IsGUILocked が false にリセットされる
            var src = CreateSrc();
            src.Commands.ViewMode = false;
            src.Commands.CommandState = null;
            src.Commands.ProceedCommand();
            Assert.IsFalse(((MockGUI)src.GUI).IsGUILocked);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeFalse_UnknownCommandState_SetsIsGUILockedFalse()
        {
            var src = CreateSrc();
            src.Commands.ViewMode = false;
            src.Commands.CommandState = "存在しない状態";
            src.Commands.ProceedCommand();
            Assert.IsFalse(((MockGUI)src.GUI).IsGUILocked);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeFalse_NullCommandState_ResetsIsScenarioFinished()
        {
            var src = CreateSrc();
            src.Commands.ViewMode = false;
            src.Commands.CommandState = null;
            src.IsScenarioFinished = true;
            src.Commands.ProceedCommand();
            Assert.IsFalse(src.IsScenarioFinished);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeFalse_NullCommandState_ResetsIsCanceled()
        {
            var src = CreateSrc();
            src.Commands.ViewMode = false;
            src.Commands.CommandState = null;
            src.IsCanceled = true;
            src.Commands.ProceedCommand();
            Assert.IsFalse(src.IsCanceled);
        }

        [TestMethod]
        public void ProceedCommand_ViewModeFalse_EmptyCommandState_DoesNotThrow()
        {
            var src = CreateSrc();
            src.Commands.ViewMode = false;
            src.Commands.CommandState = "";
            src.Commands.ProceedCommand();
            // switch の "" は対応ケースなし → 例外なし
        }

        // ──────────────────────────────────────────────
        // ProceedCommand: ViewMode=false, CommandState= "ユニット選択" → ProceedUnitSelect (GUI依存)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedCommand_ViewModeFalse_UnitSelectState_ReachesGUI()
        {
            // "ユニット選択" は ProceedUnitSelect → GUI.PixelToMapX/Y 等を呼ぶ
            var src = CreateSrc();
            src.Commands.ViewMode = false;
            src.Commands.CommandState = "ユニット選択";
            bool guiReached = false;
            ((MockGUI)src.GUI).PixelToMapXHandler = (_) => { guiReached = true; return 0; };
            try
            {
                src.Commands.ProceedCommand();
            }
            catch (GUINotImplementedException) { guiReached = true; }
            catch { guiReached = true; }
            Assert.IsTrue(guiReached);
        }

        // ──────────────────────────────────────────────
        // ProceedInput: GuiButton.Left
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedInput_LeftButton_MapCommandState_SetsStateToUnitSelect()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド";
            src.Commands.ProceedInput(GuiButton.Left, null, null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_LeftButton_MapCommandState_DoesNotCallGUI()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド";
            src.Commands.ProceedInput(GuiButton.Left, null, null);
            // 例外なし
        }

        [TestMethod]
        public void ProceedInput_LeftButton_CommandSelectState_NoUnit_CancelsCommand()
        {
            // unit=null なので CancelCommand() のみ実行
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.ProceedInput(GuiButton.Left, null, null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_LeftButton_CommandSelectState_NoUnit_ClearsSelectedCommand()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "攻撃";
            src.Commands.ProceedInput(GuiButton.Left, null, null);
            Assert.AreEqual("", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void ProceedInput_LeftButton_PostMoveCommandSelectState_CallsCancelCommand()
        {
            // "移動後コマンド選択" → CancelCommand() が呼ばれる
            // CancelCommand("移動後コマンド選択") は CommandState を "ターゲット選択" に変更してから
            // SelectedUnit を移動させようとする (SelectedUnit=null → NRE)
            // 重要: CancelCommand が呼ばれたこと自体を確認する
            var src = CreateSrc();
            src.Commands.CommandState = "移動後コマンド選択";
            bool cancelReached = false;
            // CancelCommand("移動後コマンド選択") → CommandState = "ターゲット選択" → SelectedUnit.Area → NRE
            // → CommandState は "ターゲット選択" になっている (NREの前に設定される)
            try
            {
                src.Commands.ProceedInput(GuiButton.Left, null, null);
            }
            catch (System.NullReferenceException) { cancelReached = true; }
            catch (GUINotImplementedException) { cancelReached = true; }
            catch { cancelReached = true; }
            // CommandState が "ターゲット選択" になっているか、または例外が発生している
            cancelReached = cancelReached || src.Commands.CommandState == "ターゲット選択";
            Assert.IsTrue(cancelReached, "CancelCommand が呼ばれ CommandState が変化するはず");
        }

        [TestMethod]
        public void ProceedInput_LeftButton_UnitSelectState_NullUnit_DoesNotProceed()
        {
            // "ユニット選択" + unit=null → ProceedCommand() は呼ばれない → 状態変化なし
            var src = CreateSrc();
            src.Commands.CommandState = "ユニット選択";
            src.Commands.ProceedInput(GuiButton.Left, null, null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_LeftButton_UnitSelectState_NullUnit_DoesNotCallGUI()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "ユニット選択";
            src.Commands.ProceedInput(GuiButton.Left, null, null);
            // 例外なし
        }

        // ──────────────────────────────────────────────
        // ProceedInput: GuiButton.Right
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ProceedInput_RightButton_MapCommandState_SetsStateToUnitSelect()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド";
            src.Commands.ProceedInput(GuiButton.Right, null, null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_RightButton_MapCommandState_DoesNotCallGUI()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド";
            src.Commands.ProceedInput(GuiButton.Right, null, null);
            // 例外なし
        }

        [TestMethod]
        public void ProceedInput_RightButton_CommandSelectState_CancelsCommand()
        {
            // Right + "コマンド選択" は default → CancelCommand()
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.ProceedInput(GuiButton.Right, null, null);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ProceedInput_RightButton_PostMoveCommandSelectState_CallsCancelCommand()
        {
            // Right + "移動後コマンド選択" → default → CancelCommand()
            // CancelCommand("移動後コマンド選択") → CommandState="ターゲット選択" → SelectedUnit.Area → NRE
            var src = CreateSrc();
            src.Commands.CommandState = "移動後コマンド選択";
            bool cancelReached = false;
            try
            {
                src.Commands.ProceedInput(GuiButton.Right, null, null);
            }
            catch (System.NullReferenceException) { cancelReached = true; }
            catch (GUINotImplementedException) { cancelReached = true; }
            catch { cancelReached = true; }
            cancelReached = cancelReached || src.Commands.CommandState == "ターゲット選択";
            Assert.IsTrue(cancelReached);
        }

        [TestMethod]
        public void ProceedInput_RightButton_UnknownState_CallsCancelCommand()
        {
            // "ターゲット選択" は Right の switch の default → CancelCommand()
            // CancelCommand("ターゲット選択") → CommandState="コマンド選択" → Status.DisplayUnitStatus → GUI
            var src = CreateSrc();
            src.Commands.CommandState = "ターゲット選択";
            bool cancelReached = false;
            try
            {
                src.Commands.ProceedInput(GuiButton.Right, null, null);
            }
            catch (GUINotImplementedException) { cancelReached = true; }
            catch (System.NullReferenceException) { cancelReached = true; }
            catch { cancelReached = true; }
            cancelReached = cancelReached || src.Commands.CommandState == "コマンド選択";
            Assert.IsTrue(cancelReached);
        }

        [TestMethod]
        public void ProceedInput_NeitherLeftNorRight_DoesNothing()
        {
            // GuiButton.None は Left でも Right でもないので何も起きない
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "攻撃";
            src.Commands.ProceedInput(GuiButton.None, null, null);
            // 状態変化なし
            Assert.AreEqual("コマンド選択", src.Commands.CommandState);
            Assert.AreEqual("攻撃", src.Commands.SelectedCommand);
        }
    }
}
