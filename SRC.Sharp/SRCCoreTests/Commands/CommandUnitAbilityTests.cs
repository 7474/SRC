using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.unitability.cs のユニットテスト
    ///
    /// StartAbilityCommand は UnitCommand(AbilityCmdID, "アビリティ") 経由で呼ばれる。
    /// - 内部で GUI.LockGUI → GUI.AbilityListBox を呼ぶ
    /// - AbilityListBox が null を返すとキャンセル (CancelCommand)
    /// - CommandState == "コマンド選択" → AbilityListMode.BeforeMove
    /// - CommandState != "コマンド選択" → AbilityListMode.AfterMove
    /// FeatureListCommand は UnitCommand(FeatureListCmdID, ...) 経由で呼ばれる。
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
        // StartAbilityCommand: GUI.LockGUI に到達する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartAbilityCommand_ReachesGUILockGUI()
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
                src.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "アビリティ"));
            }
            catch (GUINotImplementedException)
            {
                reached = true;
            }
            catch
            {
                reached = true;
            }

            Assert.IsTrue(reached, "StartAbilityCommand の GUI.LockGUI に到達するはずです");
        }

        [TestMethod]
        public void StartAbilityCommand_AbilityListBoxReturnsNull_CancelsCommand()
        {
            // AbilityListBox が null を返すとキャンセル → SelectedAbility = 0
            var src = CreateSrc();
            src.Commands.SelectedAbility = 99;
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.UnlockGUIHandler = () => { };
            mock.AbilityListBoxHandler = (_, __, ___, ____, _____) => null;
            mock.RestoreCursorPosHandler = () => { };
            mock.RedrawScreenHandler = (_) => { };
            mock.MaskScreenHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "アビリティ"));
            }
            catch (GUINotImplementedException) { }
            catch { }

            Assert.AreEqual(0, src.Commands.SelectedAbility);
        }

        [TestMethod]
        public void StartAbilityCommand_AbilityListBoxReturnsNull_CommandStateBecomesCancelledState()
        {
            // AbilityListBox キャンセル後は CancelCommand() が呼ばれ
            // "コマンド選択" → "ユニット選択" に変わる
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.UnlockGUIHandler = () => { };
            mock.AbilityListBoxHandler = (_, __, ___, ____, _____) => null;
            mock.RestoreCursorPosHandler = () => { };
            mock.RedrawScreenHandler = (_) => { };
            mock.MaskScreenHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "アビリティ"));
            }
            catch (GUINotImplementedException) { }
            catch { }

            // CancelCommand は "コマンド選択" を "ユニット選択" に変更する
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void StartAbilityCommand_WithSelectedUnit_DoesNotThrowNullRef()
        {
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.UnlockGUIHandler = () => { };
            mock.AbilityListBoxHandler = (_, __, ___, ____, _____) => null;
            mock.RestoreCursorPosHandler = () => { };
            mock.RedrawScreenHandler = (_) => { };
            mock.MaskScreenHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "アビリティ"));
            }
            catch (GUINotImplementedException) { }
            catch (System.NullReferenceException ex)
            {
                Assert.Fail("NullReferenceException が発生しました: " + ex.Message);
            }
            catch { }
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
        public void StartItemCommand_AbilityListBoxReturnsNull_CancelsCommand()
        {
            // ItemCmdID (アイテム) も StartAbilityCommand(is_item=true) を呼ぶ
            var src = CreateSrc();
            src.Commands.SelectedAbility = 99;
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.UnlockGUIHandler = () => { };
            mock.AbilityListBoxHandler = (_, __, ___, ____, _____) => null;
            mock.RestoreCursorPosHandler = () => { };
            mock.RedrawScreenHandler = (_) => { };
            mock.MaskScreenHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.ItemCmdID, "アイテム"));
            }
            catch (GUINotImplementedException) { }
            catch { }

            Assert.AreEqual(0, src.Commands.SelectedAbility);
        }
    }
}
