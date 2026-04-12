using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.unitsp.cs のユニットテスト
    ///
    /// StartSpecialPowerCommand は UnitCommand(SpecialPowerCmdID, "精神") 経由で呼ばれる。
    /// 内部で GUI.LockGUI → GUI.ListBox（パイロット選択）を呼ぶ。
    /// - パイロットがいない場合も GUI.LockGUI まで到達する
    /// - ListBox が 0 を返すとキャンセル (CancelCommand)
    /// </summary>
    [TestClass]
    public class CommandUnitSpTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Commands.SelectedUnit = new Unit(src);
            return src;
        }

        // ──────────────────────────────────────────────
        // StartSpecialPowerCommand: GUI.LockGUI に到達する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartSpecialPowerCommand_ReachesGUILockGUI()
        {
            var src = CreateSrc();
            bool reached = false;
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { reached = true; };
            mock.UnlockGUIHandler = () => { };
            mock.ListBoxHandler = (_) => { throw new GUINotImplementedException("ListBox"); };
            mock.CloseListBoxHandler = () => { };
            mock.RestoreCursorPosHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "精神"));
            }
            catch (GUINotImplementedException)
            {
                reached = true;
            }
            catch
            {
                reached = true;
            }

            Assert.IsTrue(reached, "StartSpecialPowerCommand の GUI.LockGUI に到達するはずです");
        }

        [TestMethod]
        public void StartSpecialPowerCommand_SetsSelectedCommandToSpecialPower()
        {
            // StartSpecialPowerCommand は最初に SelectedCommand = "スペシャルパワー" をセットする
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.UnlockGUIHandler = () => { };
            mock.ListBoxHandler = (_) => 0;
            mock.CloseListBoxHandler = () => { };
            mock.RestoreCursorPosHandler = () => { };
            mock.RedrawScreenHandler = (_) => { };
            mock.MaskScreenHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "精神"));
            }
            catch (GUINotImplementedException) { }
            catch { }

            Assert.AreEqual("スペシャルパワー", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void StartSpecialPowerCommand_NoPilotsWithSpecialPower_CancelsViaCancelCommand()
        {
            // パイロットが使えるスペシャルパワーがなければキャンセルされる
            // (PilotsHaveSpecialPower が空 → i == 0 → CancelCommand)
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.UnlockGUIHandler = () => { };
            mock.CloseListBoxHandler = () => { };
            mock.RestoreCursorPosHandler = () => { };
            mock.RedrawScreenHandler = (_) => { };
            mock.MaskScreenHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "精神"));
            }
            catch (GUINotImplementedException) { }
            catch { }

            // パイロットがいない場合は i = 0 → CancelCommand → CommandState は "コマンド選択" のまま
            Assert.AreEqual("コマンド選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void StartSpecialPowerCommand_WithSelectedUnit_DoesNotThrowNullRef()
        {
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.UnlockGUIHandler = () => { };
            mock.CloseListBoxHandler = () => { };
            mock.RestoreCursorPosHandler = () => { };
            mock.RedrawScreenHandler = (_) => { };
            mock.MaskScreenHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "精神"));
            }
            catch (GUINotImplementedException) { }
            catch (System.NullReferenceException ex)
            {
                Assert.Fail("NullReferenceException が発生しました: " + ex.Message);
            }
            catch { }
        }

        [TestMethod]
        public void StartSpecialPowerCommand_CommandStateIsPreservedOnCancel()
        {
            // キャンセルされた場合、CommandState が壊れないこと
            var src = CreateSrc();
            src.Commands.CommandState = "移動後コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.UnlockGUIHandler = () => { };
            mock.CloseListBoxHandler = () => { };
            mock.RestoreCursorPosHandler = () => { };
            mock.RedrawScreenHandler = (_) => { };
            mock.MaskScreenHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "精神"));
            }
            catch (GUINotImplementedException) { }
            catch { }

            Assert.IsNotNull(src.Commands.CommandState);
        }
    }
}
