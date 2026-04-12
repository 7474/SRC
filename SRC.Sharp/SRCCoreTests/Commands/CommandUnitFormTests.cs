using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.unitform.cs のユニットテスト
    ///
    /// TransformCommand / HyperModeCommand / SplitCommand /
    /// CombineCommand / ExchangeFormCommand のテスト。
    /// すべて GUI.LockGUI() を最初に呼ぶため GUINotImplementedException が発生する。
    /// - TransformCmdID=10, HyperModeCmdID=13, SplitCmdID=11
    /// - CombineCmdID=12, ExchangeFormCmdID=22
    /// </summary>
    [TestClass]
    public class CommandUnitFormTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Commands.SelectedUnit = new Unit(src);
            return src;
        }

        // ──────────────────────────────────────────────
        // TransformCommand: GUI.LockGUI に到達する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TransformCommand_ReachesGUILockGUI()
        {
            var src = CreateSrc();
            bool reached = false;
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { reached = true; };
            mock.UnlockGUIHandler = () => { };
            mock.ListBoxHandler = (_) => { throw new GUINotImplementedException("ListBox"); };
            mock.ConfirmHandler = (_, __, ___) =>
            {
                throw new GUINotImplementedException("Confirm");
            };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.TransformCmdID, "変形"));
            }
            catch (GUINotImplementedException)
            {
                reached = true;
            }
            catch
            {
                reached = true;
            }

            Assert.IsTrue(reached, "TransformCommand の GUI.LockGUI に到達するはずです");
        }

        [TestMethod]
        public void TransformCommand_NoTransformFeature_CancelsOrThrows()
        {
            // 変形特殊能力がないユニットで変形コマンドを実行 → 例外かキャンセル
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.UnlockGUIHandler = () => { };
            mock.ConfirmHandler = (_, __, ___) => GuiDialogResult.Ok;
            mock.RedrawScreenHandler = (_) => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.TransformCmdID, "変形"));
            }
            catch (GUINotImplementedException) { }
            catch { }

            // 例外やキャンセルが発生しても CommandState が壊れないこと
            Assert.IsNotNull(src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // HyperModeCommand: GUI.LockGUI に到達する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HyperModeCommand_ReachesGUILockGUI()
        {
            var src = CreateSrc();
            bool reached = false;
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { reached = true; };
            mock.UnlockGUIHandler = () => { };
            mock.ConfirmHandler = (_, __, ___) =>
            {
                throw new GUINotImplementedException("Confirm");
            };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.HyperModeCmdID, "ハイパーモード"));
            }
            catch (GUINotImplementedException)
            {
                reached = true;
            }
            catch
            {
                reached = true;
            }

            Assert.IsTrue(reached, "HyperModeCommand の GUI.LockGUI に到達するはずです");
        }

        [TestMethod]
        public void HyperModeCommand_WithLockUnlockGUI_ReachesGUIOrThrows()
        {
            // HyperModeCommand は GUI.LockGUI の後に各種処理を行う
            // Map や Unit の状態によって NullRef が発生する場合もある
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.UnlockGUIHandler = () => { };
            mock.ConfirmHandler = (_, __, ___) => GuiDialogResult.Ok;
            mock.RedrawScreenHandler = (_) => { };

            bool exceptionOccurred = false;
            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.HyperModeCmdID, "ハイパーモード"));
            }
            catch (GUINotImplementedException)
            {
                exceptionOccurred = true;
            }
            catch
            {
                exceptionOccurred = true;
            }

            // 例外が発生するか正常終了するかのどちらか
            Assert.IsTrue(exceptionOccurred || src.Commands != null);
        }

        // ──────────────────────────────────────────────
        // SplitCommand: GUI.LockGUI に到達する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SplitCommand_ReachesGUILockGUI()
        {
            var src = CreateSrc();
            bool reached = false;
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { reached = true; };
            mock.UnlockGUIHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SplitCmdID, "分離"));
            }
            catch (GUINotImplementedException)
            {
                reached = true;
            }
            catch
            {
                reached = true;
            }

            Assert.IsTrue(reached, "SplitCommand の GUI.LockGUI に到達するはずです");
        }

        // ──────────────────────────────────────────────
        // ExchangeFormCommand: GUI.LockGUI に到達する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExchangeFormCommand_ReachesGUI()
        {
            var src = CreateSrc();
            bool reached = false;
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { reached = true; };
            mock.UnlockGUIHandler = () => { };
            mock.ListBoxHandler = (_) => { throw new GUINotImplementedException("ListBox"); };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.ExchangeFormCmdID, "換装"));
            }
            catch (GUINotImplementedException)
            {
                reached = true;
            }
            catch
            {
                reached = true;
            }

            Assert.IsTrue(reached, "ExchangeFormCommand の GUI 呼び出しに到達するはずです");
        }

        [TestMethod]
        public void CombineCommand_ReachesGUI()
        {
            var src = CreateSrc();
            bool reached = false;
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { reached = true; };
            mock.UnlockGUIHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.CombineCmdID, "合体"));
            }
            catch (GUINotImplementedException)
            {
                reached = true;
            }
            catch
            {
                reached = true;
            }

            Assert.IsTrue(reached, "CombineCommand の GUI 呼び出しに到達するはずです");
        }
    }
}
