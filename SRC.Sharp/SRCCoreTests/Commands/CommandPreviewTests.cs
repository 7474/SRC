using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.TestLib;
using SRCCore.Tests;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.preview.cs のユニットテスト
    /// WeaponListCommand, AbilityListCommand
    /// </summary>
    [TestClass]
    public class CommandPreviewTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC
            {
                GUI = new MockGUI(),
                GUIMap = new MockGUIMap(),
                GUIStatus = new MockGUIStatus(),
            };
            var unit = new Unit(src);
            src.Commands.SelectedUnit = unit;
            return src;
        }

        // ──────────────────────────────────────────────
        // WeaponListCommand (WeaponListCmdID = 24)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WeaponListCommand_WeaponListBoxNull_CommandStateIsUnitSelect()
        {
            // WeaponListBox が null を返す (キャンセル) → CommandState = "ユニット選択"
            var src = CreateSrc();
            ((MockGUI)src.GUI).LockGUIHandler = () => { };
            ((MockGUI)src.GUI).UnlockGUIHandler = () => { };
            ((MockGUI)src.GUI).CloseListBoxHandler = () => { };
            ((MockGUI)src.GUI).WeaponListBoxHandler = (_, __, ___, ____, _____) => null;
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.WeaponListCmdID, "武器一覧"));

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void WeaponListCommand_WeaponListBoxNull_CloseListBoxCalled()
        {
            var src = CreateSrc();
            ((MockGUI)src.GUI).LockGUIHandler = () => { };
            ((MockGUI)src.GUI).UnlockGUIHandler = () => { };
            bool closeCalled = false;
            ((MockGUI)src.GUI).CloseListBoxHandler = () => { closeCalled = true; };
            ((MockGUI)src.GUI).WeaponListBoxHandler = (_, __, ___, ____, _____) => null;
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.WeaponListCmdID, "武器一覧"));

            Assert.IsTrue(closeCalled);
        }

        [TestMethod]
        public void WeaponListCommand_WeaponListBoxNull_DoesNotSetSelectedWeapon()
        {
            var src = CreateSrc();
            ((MockGUI)src.GUI).LockGUIHandler = () => { };
            ((MockGUI)src.GUI).UnlockGUIHandler = () => { };
            ((MockGUI)src.GUI).CloseListBoxHandler = () => { };
            ((MockGUI)src.GUI).WeaponListBoxHandler = (_, __, ___, ____, _____) => null;
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.WeaponListCmdID, "武器一覧"));

            Assert.AreEqual(0, src.Commands.SelectedWeapon);
        }

        // ──────────────────────────────────────────────
        // AbilityListCommand (AbilityListCmdID = 25)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AbilityListCommand_AbilityListBoxNull_CommandStateIsUnitSelect()
        {
            // AbilityListBox が null を返す (キャンセル) → CommandState = "ユニット選択"
            var src = CreateSrc();
            ((MockGUI)src.GUI).LockGUIHandler = () => { };
            ((MockGUI)src.GUI).UnlockGUIHandler = () => { };
            ((MockGUI)src.GUI).CloseListBoxHandler = () => { };
            ((MockGUI)src.GUI).AbilityListBoxHandler = (_, __, ___, ____, _____) => null;
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.AbilityListCmdID, "アビリティ一覧"));

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void AbilityListCommand_AbilityListBoxNull_SelectedAbilitySetToZero()
        {
            var src = CreateSrc();
            ((MockGUI)src.GUI).LockGUIHandler = () => { };
            ((MockGUI)src.GUI).UnlockGUIHandler = () => { };
            ((MockGUI)src.GUI).CloseListBoxHandler = () => { };
            ((MockGUI)src.GUI).AbilityListBoxHandler = (_, __, ___, ____, _____) => null;
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.AbilityListCmdID, "アビリティ一覧"));

            Assert.AreEqual(0, src.Commands.SelectedAbility);
        }

        [TestMethod]
        public void AbilityListCommand_AbilityListBoxNull_CloseListBoxCalled()
        {
            var src = CreateSrc();
            ((MockGUI)src.GUI).LockGUIHandler = () => { };
            ((MockGUI)src.GUI).UnlockGUIHandler = () => { };
            bool closeCalled = false;
            ((MockGUI)src.GUI).CloseListBoxHandler = () => { closeCalled = true; };
            ((MockGUI)src.GUI).AbilityListBoxHandler = (_, __, ___, ____, _____) => null;
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.AbilityListCmdID, "アビリティ一覧"));

            Assert.IsTrue(closeCalled);
        }
    }
}
