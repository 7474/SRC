using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.TestLib;
using SRCCore.Tests;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.unitability.cs のユニットテスト
    /// StartAbilityCommand (AbilityCmdID 経由)
    /// </summary>
    [TestClass]
    public class CommandUnitAbilityTests
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
        // StartAbilityCommand: AbilityListBox が null を返すとキャンセル
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartAbilityCommand_AbilityListBoxNull_CommandStateResetToUnitSelect()
        {
            var src = CreateSrc();
            ((MockGUI)src.GUI).LockGUIHandler = () => { };
            ((MockGUI)src.GUI).UnlockGUIHandler = () => { };
            ((MockGUI)src.GUI).AbilityListBoxHandler = (_, __, ___, ____, _____) => null;
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "アビリティ"));

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void StartAbilityCommand_AbilityListBoxNull_SelectedAbilitySetToZero()
        {
            var src = CreateSrc();
            ((MockGUI)src.GUI).LockGUIHandler = () => { };
            ((MockGUI)src.GUI).UnlockGUIHandler = () => { };
            ((MockGUI)src.GUI).AbilityListBoxHandler = (_, __, ___, ____, _____) => null;
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "アビリティ"));

            Assert.AreEqual(0, src.Commands.SelectedAbility);
        }

        [TestMethod]
        public void StartAbilityCommand_AbilityListBoxNull_SelectedCommandCleared()
        {
            var src = CreateSrc();
            ((MockGUI)src.GUI).LockGUIHandler = () => { };
            ((MockGUI)src.GUI).UnlockGUIHandler = () => { };
            ((MockGUI)src.GUI).AbilityListBoxHandler = (_, __, ___, ____, _____) => null;
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "アビリティ";

            src.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "アビリティ"));

            Assert.AreEqual("", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void StartAbilityCommand_FromCommandSelectState_AbilityListBoxCalledWithBeforeMoveMode()
        {
            // コマンド選択状態では AbilityListBox に "移動前" モードが渡される
            var src = CreateSrc();
            ((MockGUI)src.GUI).LockGUIHandler = () => { };
            ((MockGUI)src.GUI).UnlockGUIHandler = () => { };
            string capturedMode = null;
            ((MockGUI)src.GUI).AbilityListBoxHandler = (_, __, ___, mode, _____) =>
            {
                capturedMode = mode;
                return null;
            };
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "アビリティ"));

            Assert.AreEqual("移動前", capturedMode);
        }

        [TestMethod]
        public void StartAbilityCommand_AbilityListBoxCalledWithCorrectUnit()
        {
            // AbilityListBox は SelectedUnit を受け取る
            var src = CreateSrc();
            ((MockGUI)src.GUI).LockGUIHandler = () => { };
            ((MockGUI)src.GUI).UnlockGUIHandler = () => { };
            Units.Unit capturedUnit = null;
            ((MockGUI)src.GUI).AbilityListBoxHandler = (u, __, ___, ____, _____) =>
            {
                capturedUnit = u;
                return null;
            };
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "アビリティ"));

            Assert.AreSame(src.Commands.SelectedUnit, capturedUnit);
        }
    }
}
