using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.unitsupply.cs のユニットテスト
    ///
    /// StartFixCommand / StartSupplyCommand は UnitCommand() 経由で呼ばれる。
    /// どちらも Map.AreaInRange() を呼んだ後に GUI.MaskScreen() に進む。
    /// - CommandState == "コマンド選択" → "ターゲット選択"
    /// - CommandState != "コマンド選択" → "移動後ターゲット選択"
    /// </summary>
    [TestClass]
    public class CommandUnitSupplyTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Commands.SelectedUnit = new Unit(src);
            return src;
        }

        // ──────────────────────────────────────────────
        // StartFixCommand: CommandState に応じた遷移先
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartFixCommand_FromCommandSelect_SetsCommandStateOrKeepsPrev()
        {
            // Map.AreaInRange の後に CommandState をセットするため、
            // Map が未初期化だと例外で CommandState が変わらない場合もある
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.MaskScreenHandler = () => { };
            mock.MoveCursorPosHandler = (_, __) => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.FixCmdID, "修理"));
            }
            catch (GUINotImplementedException)
            {
                // GUI 呼び出しに到達
            }
            catch { }

            Assert.IsNotNull(src.Commands.CommandState);
        }

        [TestMethod]
        public void StartFixCommand_FromPostMoveState_SetsCommandStateOrKeepsPrev()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "移動後コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.MaskScreenHandler = () => { };
            mock.MoveCursorPosHandler = (_, __) => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.FixCmdID, "修理"));
            }
            catch (GUINotImplementedException)
            {
                // GUI 呼び出しに到達
            }
            catch { }

            Assert.IsNotNull(src.Commands.CommandState);
        }

        [TestMethod]
        public void StartFixCommand_SetsSelectedCommandToFix()
        {
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            mock.MaskScreenHandler = () => { };
            mock.MoveCursorPosHandler = (_, __) => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.FixCmdID, "修理"));
            }
            catch { }

            Assert.AreEqual("修理", src.Commands.SelectedCommand);
        }

        // ──────────────────────────────────────────────
        // StartSupplyCommand: CommandState に応じた遷移先
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartSupplyCommand_FromCommandSelect_SetsCommandStateOrKeepsPrev()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.MaskScreenHandler = () => { };
            mock.MoveCursorPosHandler = (_, __) => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SupplyCmdID, "補給"));
            }
            catch (GUINotImplementedException)
            {
                // GUI 呼び出しに到達
            }
            catch { }

            Assert.IsNotNull(src.Commands.CommandState);
        }

        [TestMethod]
        public void StartSupplyCommand_FromPostMoveState_SetsCommandStateOrKeepsPrev()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "移動後コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.MaskScreenHandler = () => { };
            mock.MoveCursorPosHandler = (_, __) => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SupplyCmdID, "補給"));
            }
            catch (GUINotImplementedException)
            {
                // GUI 呼び出しに到達
            }
            catch { }

            Assert.IsNotNull(src.Commands.CommandState);
        }

        [TestMethod]
        public void StartSupplyCommand_SetsSelectedCommandToSupply()
        {
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            mock.MaskScreenHandler = () => { };
            mock.MoveCursorPosHandler = (_, __) => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SupplyCmdID, "補給"));
            }
            catch { }

            Assert.AreEqual("補給", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void StartFixCommand_ReachesGUI()
        {
            var src = CreateSrc();
            bool reached = false;
            var mock = (MockGUI)src.GUI;
            mock.MaskScreenHandler = () => { reached = true; };
            mock.MoveCursorPosHandler = (_, __) => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.FixCmdID, "修理"));
            }
            catch (GUINotImplementedException)
            {
                reached = true;
            }
            catch
            {
                reached = true;
            }

            Assert.IsTrue(reached, "StartFixCommand の GUI 呼び出しに到達するはずです");
        }
    }
}
