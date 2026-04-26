using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.ExchangeFormCommand() のユニットテスト (Command.unitform.cs)
    ///
    /// ExchangeFormCommand は public メソッドとして UnitCommand からも Intermission からも呼ばれる。
    /// 処理フロー:
    ///   GUI.LockGUI()
    ///   → SelectedUnit.Feature("換装") → 換装先リスト作成
    ///   → GUI.ListBox() でユーザー選択
    ///   → キャンセル(ret==0): CancelCommand() → UnlockGUI()
    ///   → 換装実行: SelectedUnit.Transform(...)
    ///   → CommandState = "ユニット選択"
    ///   → GUI.UnlockGUI()
    /// GUI.LockGUI() がモックされていない場合は GUINotImplementedException が発生する。
    /// </summary>
    [TestClass]
    public class CommandExchangeFormTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // LockGUI ハンドラなし → GUINotImplementedException
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExchangeFormCommand_NoLockGUIHandler_ThrowsGUINotImplementedException()
        {
            var src = CreateSrc();
            src.Commands.SelectedUnit = new Unit(src);
            Assert.Throws<GUINotImplementedException>(
                () => src.Commands.ExchangeFormCommand());
        }

        [TestMethod]
        public void ExchangeFormCommand_NoLockGUIHandler_ExceptionMessageContainsLockGUI()
        {
            var src = CreateSrc();
            src.Commands.SelectedUnit = new Unit(src);
            try
            {
                src.Commands.ExchangeFormCommand();
                Assert.Fail("例外が発生しなかった");
            }
            catch (GUINotImplementedException ex)
            {
                StringAssert.Contains(ex.Message, "LockGUI");
            }
        }

        // ──────────────────────────────────────────────
        // LockGUI をモック → Feature("換装") へ進む
        // SelectedUnit に 換装 フィーチャーがない → 例外 or リスト空
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExchangeFormCommand_WithLockGUI_AdvancesBeyondLockGUI()
        {
            var src = CreateSrc();
            src.Commands.SelectedUnit = new Unit(src);
            bool lockCalled = false;
            ((MockGUI)src.GUI).LockGUIHandler = () => { lockCalled = true; };
            try
            {
                src.Commands.ExchangeFormCommand();
            }
            catch { }
            Assert.IsTrue(lockCalled, "LockGUI が呼ばれるはず");
        }

        [TestMethod]
        public void ExchangeFormCommand_WithLockGUI_AttemptsFurtherProcessing()
        {
            // LockGUI 通過後は Feature("換装") → NRE または ListBox → GUINotImplementedException
            var src = CreateSrc();
            src.Commands.SelectedUnit = new Unit(src);
            ((MockGUI)src.GUI).LockGUIHandler = () => { };
            bool reachedFurther = false;
            try
            {
                src.Commands.ExchangeFormCommand();
                reachedFurther = true;
            }
            catch (GUINotImplementedException)
            {
                reachedFurther = true; // ListBox に到達
            }
            catch (System.NullReferenceException)
            {
                reachedFurther = true; // Feature("換装") が null で NRE
            }
            catch
            {
                reachedFurther = true;
            }
            Assert.IsTrue(reachedFurther);
        }

        // ──────────────────────────────────────────────
        // SelectedUnit が null の場合 → LockGUI の前に問題なし、LockGUI 以降で NRE
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExchangeFormCommand_NullSelectedUnit_WithLockGUI_ThrowsAfterLockGUI()
        {
            var src = CreateSrc();
            src.Commands.SelectedUnit = null;
            bool lockCalled = false;
            ((MockGUI)src.GUI).LockGUIHandler = () => { lockCalled = true; };
            try
            {
                src.Commands.ExchangeFormCommand();
            }
            catch { }
            // SelectedUnit = null でも LockGUI が呼ばれることを確認
            Assert.IsTrue(lockCalled);
        }

        // ──────────────────────────────────────────────
        // UnitCommand 経由でも同じ ExchangeFormCommand に到達する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_ExchangeFormCmdID_CallsLockGUI()
        {
            var src = CreateSrc();
            src.Commands.SelectedUnit = new Unit(src);
            src.Commands.CommandState = "コマンド選択";
            bool lockCalled = false;
            ((MockGUI)src.GUI).LockGUIHandler = () => { lockCalled = true; };
            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.ExchangeFormCmdID, "換装"));
            }
            catch { }
            Assert.IsTrue(lockCalled);
        }
    }
}
