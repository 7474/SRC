using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.preview.cs (FeatureListCommand / WeaponListCommand / AbilityListCommand /
    /// ShowAreaInSpeedCommand / ShowAreaInRangeCommand) のユニットテスト。
    ///
    /// FeatureListCommand:
    ///   1. GUI.LockGUI() を呼ぶ → リスト構築 → GUI.ListBox
    /// WeaponListCommand / AbilityListCommand:
    ///   1. GUI.LockGUI() → 対応 ListBox → null 返却でキャンセル → CommandState = "ユニット選択"
    /// ShowAreaInSpeedCommand / ShowAreaInRangeCommand:
    ///   1. SelectedCommand を設定してから Map.AreaInXxx を呼ぶ
    ///   2. Map が未初期化でも SelectedCommand の設定は確認できる
    /// </summary>
    [TestClass]
    public class CommandPreviewTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Commands.SelectedUnit = new Unit(src);
            return src;
        }

        // ──────────────────────────────────────────────
        // FeatureListCmdID (23) → FeatureListCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FeatureListCmdID_CallsGuiLockGUI()
        {
            // FeatureListCommand() は GUI.LockGUI() を最初に呼ぶ
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            bool lockCalled = false;
            mock.LockGUIHandler = () => { lockCalled = true; };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.FeatureListCmdID, "特殊能力一覧"));
            }
            catch { }

            Assert.IsTrue(lockCalled, "GUI.LockGUI() が呼ばれるはずです");
        }

        // ──────────────────────────────────────────────
        // WeaponListCmdID (24) → WeaponListCommand() キャンセルパス
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WeaponListCmdID_Cancel_CommandStateBecomesUnitSelect()
        {
            // WeaponListBox が null を返すとキャンセルパスへ → CommandState = "ユニット選択"
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.WeaponListBoxHandler = (u, list, cap, mode, bgm) => null;
            mock.CloseListBoxHandler = () => { };
            mock.UnlockGUIHandler = () => { };

            src.Commands.UnitCommand(new UiCommand(Command.WeaponListCmdID, "武器一覧"));

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void WeaponListCmdID_Cancel_CallsCloseListBox()
        {
            // キャンセル時に GUI.CloseListBox() が呼ばれることを確認
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            bool closeListBoxCalled = false;
            mock.LockGUIHandler = () => { };
            mock.WeaponListBoxHandler = (u, list, cap, mode, bgm) => null;
            mock.CloseListBoxHandler = () => { closeListBoxCalled = true; };
            mock.UnlockGUIHandler = () => { };

            src.Commands.UnitCommand(new UiCommand(Command.WeaponListCmdID, "武器一覧"));

            Assert.IsTrue(closeListBoxCalled, "キャンセル時に CloseListBox() が呼ばれるはずです");
        }

        [TestMethod]
        public void WeaponListCmdID_Cancel_WeaponListBoxCalledWithUnit()
        {
            // WeaponListBox に SelectedUnit が渡されることを確認
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            Units.Unit capturedUnit = null;
            mock.LockGUIHandler = () => { };
            mock.WeaponListBoxHandler = (u, list, cap, mode, bgm) => { capturedUnit = u; return null; };
            mock.CloseListBoxHandler = () => { };
            mock.UnlockGUIHandler = () => { };

            src.Commands.UnitCommand(new UiCommand(Command.WeaponListCmdID, "武器一覧"));

            Assert.AreSame(src.Commands.SelectedUnit, capturedUnit, "WeaponListBox に SelectedUnit が渡されるはずです");
        }

        // ──────────────────────────────────────────────
        // AbilityListCmdID (25) → AbilityListCommand() キャンセルパス
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AbilityListCmdID_Cancel_CommandStateBecomesUnitSelect()
        {
            // AbilityListBox が null を返すとキャンセルパスへ → CommandState = "ユニット選択"
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.AbilityListBoxHandler = (u, list, cap, mode, isItem) => null;
            mock.CloseListBoxHandler = () => { };
            mock.UnlockGUIHandler = () => { };

            src.Commands.UnitCommand(new UiCommand(Command.AbilityListCmdID, "アビリティ一覧"));

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void AbilityListCmdID_Cancel_CallsCloseListBox()
        {
            // キャンセル時に GUI.CloseListBox() が呼ばれることを確認
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            bool closeListBoxCalled = false;
            mock.LockGUIHandler = () => { };
            mock.AbilityListBoxHandler = (u, list, cap, mode, isItem) => null;
            mock.CloseListBoxHandler = () => { closeListBoxCalled = true; };
            mock.UnlockGUIHandler = () => { };

            src.Commands.UnitCommand(new UiCommand(Command.AbilityListCmdID, "アビリティ一覧"));

            Assert.IsTrue(closeListBoxCalled, "キャンセル時に CloseListBox() が呼ばれるはずです");
        }

        [TestMethod]
        public void AbilityListCmdID_Cancel_SelectedAbilitySetToZero()
        {
            // キャンセル時に SelectedAbility = 0 に設定されることを確認
            var src = CreateSrc();
            src.Commands.SelectedAbility = 99;
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.AbilityListBoxHandler = (u, list, cap, mode, isItem) => null;
            mock.CloseListBoxHandler = () => { };
            mock.UnlockGUIHandler = () => { };

            src.Commands.UnitCommand(new UiCommand(Command.AbilityListCmdID, "アビリティ一覧"));

            Assert.AreEqual(0, src.Commands.SelectedAbility, "キャンセル時に SelectedAbility = 0 になるはずです");
        }

        [TestMethod]
        public void AbilityListCmdID_CallsAbilityListBoxWithListMode()
        {
            // AbilityListCommand は "一覧" モードで AbilityListBox を呼ぶ
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            string capturedMode = null;
            mock.LockGUIHandler = () => { };
            mock.AbilityListBoxHandler = (u, list, cap, mode, isItem) => { capturedMode = mode; return null; };
            mock.CloseListBoxHandler = () => { };
            mock.UnlockGUIHandler = () => { };

            src.Commands.UnitCommand(new UiCommand(Command.AbilityListCmdID, "アビリティ一覧"));

            Assert.AreEqual("一覧", capturedMode, "AbilityListCommand は '一覧' モードで呼ぶはずです");
        }

        // ──────────────────────────────────────────────
        // MoveCmdID (0) + non-"移動" label → ShowAreaInSpeedCommand()
        // ShowAreaInSpeedCommand: SelectedCommand = "移動範囲" を Map 呼び出し前に設定
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MoveCmdID_WithNonMoveLabel_SetsSelectedCommandToMoveRange()
        {
            // label が "移動" 以外 → ShowAreaInSpeedCommand() → SelectedCommand = "移動範囲"
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動速度"));
            }
            catch { }

            Assert.AreEqual("移動範囲", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void MoveCmdID_WithMoveLabel_SetsSelectedCommandToMove()
        {
            // label が "移動" → StartMoveCommand() → SelectedCommand = "移動"
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            }
            catch { }

            Assert.AreEqual("移動", src.Commands.SelectedCommand);
        }

        // ──────────────────────────────────────────────
        // AttackCmdID (4) + non-"攻撃" label → ShowAreaInRangeCommand()
        // ShowAreaInRangeCommand: SelectedCommand = "射程範囲" を Map 呼び出し前に設定
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AttackCmdID_WithNonAttackLabel_SetsSelectedCommandToFireRange()
        {
            // label が "攻撃" 以外 → ShowAreaInRangeCommand() → SelectedCommand = "射程範囲"
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "射程範囲"));
            }
            catch { }

            Assert.AreEqual("射程範囲", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void AttackCmdID_WithAttackLabel_CallsGuiLockGUI()
        {
            // label が "攻撃" → StartAttackCommand() → GUI.LockGUI() が呼ばれる
            // StartAttackCommand は LockGUI の後にパイロット参照があるため
            // 空ユニットでは例外が発生するが、LockGUI 到達は確認できる
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            bool lockCalled = false;
            mock.LockGUIHandler = () => { lockCalled = true; };
            src.Commands.CommandState = "コマンド選択";

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));
            }
            catch { }

            Assert.IsTrue(lockCalled, "StartAttackCommand() の最初に GUI.LockGUI() が呼ばれるはずです");
        }
    }
}
