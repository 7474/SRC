using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.unitability.cs (StartAbilityCommand / FinishAbilityCommand / MapAbilityCommand)
    /// のユニットテスト。
    ///
    /// StartAbilityCommand (AbilityCmdID) の動作:
    ///   1. GUI.LockGUI()
    ///   2. GUI.AbilityListBox(SelectedUnit, ...) → null ならキャンセルパス
    ///   3. キャンセル時: SelectedAbility=0 → CancelCommand() → GUI.UnlockGUI()
    ///
    /// ItemCmdID も StartAbilityCommand(is_item=true) を呼ぶ。
    ///   is_item=true の場合キャプションが "アイテム選択" になる。
    ///
    /// GUI 依存が深いため、キャンセルパスのステート変化と GUI 呼び出し到達を確認する。
    /// </summary>
    [TestClass]
    public class CommandUnitAbilityTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Commands.SelectedUnit = new Unit(src);
            return src;
        }

        // ──────────────────────────────────────────────
        // AbilityCmdID (7) → StartAbilityCommand(false) 到達確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AbilityCmdID_CallsGuiLockGUI()
        {
            // StartAbilityCommand() は GUI.LockGUI() を最初に呼ぶ
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            bool lockCalled = false;
            mock.LockGUIHandler = () => { lockCalled = true; };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "アビリティ"));
            }
            catch { }

            Assert.IsTrue(lockCalled, "GUI.LockGUI() が呼ばれるはずです");
        }

        [TestMethod]
        public void AbilityCmdID_CallsAbilityListBox()
        {
            // StartAbilityCommand() は GUI.AbilityListBox() を呼ぶ
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            bool abilityListBoxCalled = false;
            mock.LockGUIHandler = () => { };
            mock.AbilityListBoxHandler = (u, list, cap, mode, isItem) =>
            {
                abilityListBoxCalled = true;
                return null;
            };
            mock.UnlockGUIHandler = () => { };

            src.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "アビリティ"));

            Assert.IsTrue(abilityListBoxCalled, "GUI.AbilityListBox() が呼ばれるはずです");
        }

        [TestMethod]
        public void AbilityCmdID_AbilityListBoxCalledWithBeforeMoveMode_WhenCommandSelect()
        {
            // CommandState == "コマンド選択" のとき "移動前" モードで AbilityListBox を呼ぶ
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            string capturedMode = null;
            mock.LockGUIHandler = () => { };
            mock.AbilityListBoxHandler = (u, list, cap, mode, isItem) =>
            {
                capturedMode = mode;
                return null;
            };
            mock.UnlockGUIHandler = () => { };

            src.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "アビリティ"));

            Assert.AreEqual("移動前", capturedMode, "コマンド選択時は '移動前' モードで呼ぶはずです");
        }

        [TestMethod]
        public void AbilityCmdID_AbilityListBoxCalledWithAfterMoveMode_WhenPostMoveCommandSelect()
        {
            // CommandState == "移動後コマンド選択" のとき "移動後" モードで AbilityListBox を呼ぶ
            // CancelCommand() は "移動後コマンド選択" の場合 Unit.Move を呼ぶが、
            // AbilityListBox の capturedMode は その前に設定済み
            var src = CreateSrc();
            src.Commands.CommandState = "移動後コマンド選択";
            var mock = (MockGUI)src.GUI;
            string capturedMode = null;
            mock.LockGUIHandler = () => { };
            mock.AbilityListBoxHandler = (u, list, cap, mode, isItem) =>
            {
                capturedMode = mode;
                return null;
            };
            mock.UnlockGUIHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "アビリティ"));
            }
            catch { }

            Assert.AreEqual("移動後", capturedMode, "移動後コマンド選択時は '移動後' モードで呼ぶはずです");
        }

        // ──────────────────────────────────────────────
        // AbilityCmdID キャンセルパス (AbilityListBox returns null)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AbilityCmdID_Cancel_CommandStateBecomesUnitSelect()
        {
            // キャンセル → CancelCommand() (CommandState=="コマンド選択") → "ユニット選択"
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.AbilityListBoxHandler = (u, list, cap, mode, isItem) => null;
            mock.UnlockGUIHandler = () => { };

            src.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "アビリティ"));

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void AbilityCmdID_Cancel_SelectedCommandCleared()
        {
            // キャンセル → CancelCommand() → SelectedCommand = ""
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "アビリティ前";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.AbilityListBoxHandler = (u, list, cap, mode, isItem) => null;
            mock.UnlockGUIHandler = () => { };

            src.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "アビリティ"));

            Assert.AreEqual("", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void AbilityCmdID_Cancel_SelectedAbilitySetToZero()
        {
            // キャンセル → SelectedAbility = 0
            var src = CreateSrc();
            src.Commands.SelectedAbility = 42;
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.AbilityListBoxHandler = (u, list, cap, mode, isItem) => null;
            mock.UnlockGUIHandler = () => { };

            src.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "アビリティ"));

            Assert.AreEqual(0, src.Commands.SelectedAbility, "キャンセル時は SelectedAbility = 0 になるはずです");
        }

        // ──────────────────────────────────────────────
        // ItemCmdID → StartAbilityCommand(is_item=true)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ItemCmdID_CallsAbilityListBox_WithIsItemTrue()
        {
            // ItemCmdID → StartAbilityCommand(is_item=true) → AbilityListBox(..., is_item=true)
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            bool? capturedIsItem = null;
            mock.LockGUIHandler = () => { };
            mock.AbilityListBoxHandler = (u, list, cap, mode, isItem) =>
            {
                capturedIsItem = isItem;
                return null;
            };
            mock.UnlockGUIHandler = () => { };

            src.Commands.UnitCommand(new UiCommand(Command.ItemCmdID, "アイテム"));

            Assert.AreEqual(true, capturedIsItem, "ItemCmdID の場合 is_item=true で呼ぶはずです");
        }

        [TestMethod]
        public void ItemCmdID_Cancel_CommandStateBecomesUnitSelect()
        {
            // ItemCmdID キャンセル → CommandState = "ユニット選択"
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.AbilityListBoxHandler = (u, list, cap, mode, isItem) => null;
            mock.UnlockGUIHandler = () => { };

            src.Commands.UnitCommand(new UiCommand(Command.ItemCmdID, "アイテム"));

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }
    }
}
