using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.TestLib;
using SRCCore.Tests;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.unitsp.cs のユニットテスト
    /// StartSpecialPowerCommand
    /// </summary>
    [TestClass]
    public class CommandUnitSpTests
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
        // StartSpecialPowerCommand: SelectedCommand が設定される
        // (PilotsHaveSpecialPower() の呼び出し前に SelectedCommand が設定される)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartSpecialPowerCommand_SetsSelectedCommandBeforeException()
        {
            // パイロットなしのユニットでは PilotsHaveSpecialPower → MainPilot → TerminateException が発生するが
            // SelectedCommand = "スペシャルパワー" は先に設定される
            var src = CreateSrc();
            ((MockGUI)src.GUI).LockGUIHandler = () => { };
            src.Commands.CommandState = "コマンド選択";

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "精神"));
            }
            catch { }

            Assert.AreEqual("スペシャルパワー", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void StartSpecialPowerCommand_LockGUICalledFirst()
        {
            // LockGUI が最初に呼ばれることを確認
            var src = CreateSrc();
            bool lockCalled = false;
            ((MockGUI)src.GUI).LockGUIHandler = () => { lockCalled = true; };
            src.Commands.CommandState = "コマンド選択";

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "精神"));
            }
            catch { }

            Assert.IsTrue(lockCalled);
        }

        [TestMethod]
        public void StartSpecialPowerCommand_SpecialPowerCmdID_IsDispatchedCorrectly()
        {
            // SpecialPowerCmdID (9) で StartSpecialPowerCommand が呼ばれる
            var src = CreateSrc();
            ((MockGUI)src.GUI).LockGUIHandler = () => { };
            src.Commands.CommandState = "コマンド選択";

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "精神"));
            }
            catch { }

            // SelectedCommand が "スペシャルパワー" になっていることで StartSpecialPowerCommand が呼ばれたと確認
            Assert.AreEqual("スペシャルパワー", src.Commands.SelectedCommand);
        }
    }
}
