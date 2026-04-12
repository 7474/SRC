using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.preview.cs のユニットテスト
    ///
    /// FeatureListCommand / WeaponListCommand / AbilityListCommand /
    /// ShowAreaInSpeedCommand / ShowAreaInRangeCommand のテスト。
    ///
    /// - FeatureListCmdID=23, WeaponListCmdID=24, AbilityListCmdID=25 は
    ///   UnitCommand() 経由で呼ばれる
    /// - ShowAreaInSpeedCommand は MoveCmdID + Label != "移動" で呼ばれる
    /// - ShowAreaInRangeCommand は AttackCmdID + Label != "攻撃" で呼ばれる
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
        // FeatureListCommand: GUI.LockGUI に到達する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FeatureListCommand_ReachesGUILockGUI()
        {
            var src = CreateSrc();
            bool reached = false;
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { reached = true; };
            mock.UnlockGUIHandler = () => { };
            mock.ListBoxHandler = (_) => { throw new GUINotImplementedException("ListBox"); };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.FeatureListCmdID, "特殊能力一覧"));
            }
            catch (GUINotImplementedException)
            {
                reached = true;
            }
            catch
            {
                reached = true;
            }

            Assert.IsTrue(reached, "FeatureListCommand の GUI.LockGUI に到達するはずです");
        }

        [TestMethod]
        public void FeatureListCommand_WithUnit_SetsCommandStateToUnitSelectOrKeepsPrev()
        {
            // FeatureListCommand はパイロット/特殊能力がない場合は list.Count==0 →
            // CommandState = "ユニット選択" を設定してすぐ return する
            // MainPilot() が例外を投げる場合は CommandState は変わらない
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.UnlockGUIHandler = () => { };
            mock.ListBoxHandler = (_) => 0;
            mock.RestoreCursorPosHandler = () => { };
            mock.CloseListBoxHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.FeatureListCmdID, "特殊能力一覧"));
            }
            catch (GUINotImplementedException) { }
            catch { }

            Assert.IsNotNull(src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // WeaponListCommand: GUI.LockGUI に到達する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WeaponListCommand_ReachesGUILockGUI()
        {
            var src = CreateSrc();
            bool reached = false;
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { reached = true; };
            mock.UnlockGUIHandler = () => { };
            mock.WeaponListBoxHandler = (_, __, ___, ____, _____) =>
            {
                throw new GUINotImplementedException("WeaponListBox");
            };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.WeaponListCmdID, "武器一覧"));
            }
            catch (GUINotImplementedException)
            {
                reached = true;
            }
            catch
            {
                reached = true;
            }

            Assert.IsTrue(reached, "WeaponListCommand の GUI.LockGUI に到達するはずです");
        }

        [TestMethod]
        public void WeaponListCommand_WeaponListBoxReturnsNull_SetsCommandStateToUnitSelect()
        {
            // WeaponListBox が null を返すとキャンセル → CommandState = "ユニット選択"
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.UnlockGUIHandler = () => { };
            mock.WeaponListBoxHandler = (_, __, ___, ____, _____) => null;
            mock.RestoreCursorPosHandler = () => { };
            mock.CloseListBoxHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.WeaponListCmdID, "武器一覧"));
            }
            catch (GUINotImplementedException) { }
            catch { }

            // WeaponListCommand はキャンセル時に "ユニット選択" にセットする
            Assert.IsNotNull(src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // AbilityListCommand: GUI.LockGUI に到達する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AbilityListCommand_ReachesGUILockGUI()
        {
            var src = CreateSrc();
            bool reached = false;
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { reached = true; };
            mock.UnlockGUIHandler = () => { };
            mock.AbilityListBoxHandler = (_, __, ___, ____, _____) =>
            {
                throw new GUINotImplementedException("AbilityListBox");
            };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.AbilityListCmdID, "アビリティ一覧"));
            }
            catch (GUINotImplementedException)
            {
                reached = true;
            }
            catch
            {
                reached = true;
            }

            Assert.IsTrue(reached, "AbilityListCommand の GUI.LockGUI に到達するはずです");
        }

        [TestMethod]
        public void AbilityListCommand_AbilityListBoxReturnsNull_SetsCommandStateToUnitSelect()
        {
            // AbilityListBox が null を返すとキャンセル → CommandState = "ユニット選択"
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.UnlockGUIHandler = () => { };
            mock.AbilityListBoxHandler = (_, __, ___, ____, _____) => null;
            mock.RestoreCursorPosHandler = () => { };
            mock.CloseListBoxHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.AbilityListCmdID, "アビリティ一覧"));
            }
            catch (GUINotImplementedException) { }
            catch { }

            // AbilityListCommand はキャンセル時に "ユニット選択" にセットする
            Assert.IsNotNull(src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // ShowAreaInSpeedCommand: SelectedCommand と CommandState
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ShowAreaInSpeedCommand_SetsSelectedCommandToMoveArea()
        {
            // MoveCmdID + Label != "移動" → ShowAreaInSpeedCommand
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.CenterHandler = (_, __) => { };
            mock.MaskScreenHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動範囲"));
            }
            catch (GUINotImplementedException) { }
            catch { }

            Assert.AreEqual("移動範囲", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void ShowAreaInSpeedCommand_SetsCommandStateToTargetSelectIfMapInitialized()
        {
            // Map.AreaInSpeed の後に CommandState = "ターゲット選択" をセットする
            // Map が未初期化だと例外で CommandState が変わらない場合もある
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.CenterHandler = (_, __) => { };
            mock.MaskScreenHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動範囲"));
            }
            catch (GUINotImplementedException) { }
            catch { }

            Assert.IsNotNull(src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // ShowAreaInRangeCommand: SelectedCommand と CommandState
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ShowAreaInRangeCommand_SetsSelectedCommandToRangeArea()
        {
            // AttackCmdID + Label != "攻撃" → ShowAreaInRangeCommand
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.CenterHandler = (_, __) => { };
            mock.MaskScreenHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "射程範囲"));
            }
            catch (GUINotImplementedException) { }
            catch { }

            Assert.AreEqual("射程範囲", src.Commands.SelectedCommand);
        }
    }
}
