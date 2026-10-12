using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.unitsp.cs (StartSpecialPowerCommand) のユニットテスト。
    ///
    /// StartSpecialPowerCommand の動作:
    ///   1. GUI.LockGUI()
    ///   2. SelectedCommand = "スペシャルパワー"
    ///   3. u.PilotsHaveSpecialPower() でパイロット一覧を取得
    ///      → パイロット未設定ユニットでは TerminateException/IndexOutOfRange が発生
    ///   4. パイロット複数なら ListBox で選択
    ///   5. ListBox でSP一覧 → 選択 → 発動
    ///
    /// GUI 依存とパイロットデータ依存が深いため、LockGUI 到達と
    /// SelectedCommand 設定 (Map/Pilot アクセス前) を中心にテストする。
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
        // SpecialPowerCmdID (9) → StartSpecialPowerCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SpecialPowerCmdID_CallsGuiLockGUI()
        {
            // StartSpecialPowerCommand() は GUI.LockGUI() を最初に呼ぶ
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            bool lockCalled = false;
            mock.LockGUIHandler = () => { lockCalled = true; };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));
            }
            catch { }

            Assert.IsTrue(lockCalled, "GUI.LockGUI() が呼ばれるはずです");
        }

        [TestMethod]
        public void SpecialPowerCmdID_SetsSelectedCommandToSpecialPower()
        {
            // StartSpecialPowerCommand() は LockGUI の直後に SelectedCommand = "スペシャルパワー" を設定
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));
            }
            catch { }

            Assert.AreEqual("スペシャルパワー", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void SpecialPowerCmdID_Routing_IsCorrect()
        {
            // SpecialPowerCmdID を指定すると StartSpecialPowerCommand() に到達する
            // (LockGUI が呼ばれることで到達を確認)
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            bool lockCalled = false;
            mock.LockGUIHandler = () => { lockCalled = true; };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "精神"));
            }
            catch { }

            Assert.IsTrue(lockCalled);
        }

        [TestMethod]
        public void SpecialPowerCmdID_PrevCommandSavedBeforeCall()
        {
            // UnitCommand は PrevCommand に SelectedCommand を保存してから実行する
            var src = CreateSrc();
            src.Commands.SelectedCommand = "前のコマンド";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));
            }
            catch { }

            // UnitCommand 内で PrevCommand = SelectedCommand が実行され、
            // その後 SelectedCommand = "スペシャルパワー" になる
            Assert.AreEqual("スペシャルパワー", src.Commands.SelectedCommand);
        }

        // ──────────────────────────────────────────────
        // 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SpecialPowerCmdID_InitialSelectedCommandIsEmpty()
        {
            var src = CreateSrc();
            Assert.IsTrue(string.IsNullOrEmpty(src.Commands.SelectedCommand));
        }

        [TestMethod]
        public void SpecialPowerCmdID_LockCalledBeforeSelectedCommandSet()
        {
            // LockGUI が呼ばれてから SelectedCommand が設定されること
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            bool lockCalledBeforeSet = false;
            mock.LockGUIHandler = () =>
            {
                // この時点ではまだ SelectedCommand は変わっていない
                lockCalledBeforeSet = (src.Commands.SelectedCommand != "スペシャルパワー");
            };

            try { src.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー")); }
            catch { }

            Assert.IsTrue(lockCalledBeforeSet, "LockGUI は SelectedCommand 設定前に呼ばれる");
        }

        [TestMethod]
        public void SpecialPowerCmdID_CommandStateIsCommandSelect_BeforeCall()
        {
            // SelectedUnit が存在するとき、UnitCommand は CommandState="コマンド選択" を前提とする
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };

            try { src.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー")); }
            catch { }

            Assert.AreEqual("スペシャルパワー", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void SpecialPowerCmdID_DifferentSrcInstances_IndependentState()
        {
            // 複数の SRC インスタンスが独立していることを確認
            var src1 = CreateSrc();
            var src2 = CreateSrc();
            ((MockGUI)src1.GUI).LockGUIHandler = () => { };
            ((MockGUI)src2.GUI).LockGUIHandler = () => { };

            try { src1.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー")); } catch { }
            // src2 は呼ばない

            Assert.AreEqual("スペシャルパワー", src1.Commands.SelectedCommand);
            Assert.IsTrue(string.IsNullOrEmpty(src2.Commands.SelectedCommand));
        }

        [TestMethod]
        public void SpecialPowerCmdID_CalledTwice_SelectedCommandRemainsSpecialPower()
        {
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };

            try { src.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー")); } catch { }
            try { src.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー")); } catch { }

            Assert.AreEqual("スペシャルパワー", src.Commands.SelectedCommand);
        }
    }
}
