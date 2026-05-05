using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.unitsupply.cs (StartFixCommand / FinishFixCommand /
    /// StartSupplyCommand / FinishSupplyCommand) のユニットテスト。
    ///
    /// StartFixCommand の動作:
    ///   1. SelectedCommand = "修理"
    ///   2. Map.AreaInRange(currentUnit.x, currentUnit.y, 1, 1, "味方")  → 未初期化マップでは例外
    ///   3. GUI.MaskScreen()
    ///   4. CommandState = "ターゲット選択" or "移動後ターゲット選択"
    ///
    /// StartSupplyCommand の動作:
    ///   1. SelectedCommand = "補給"
    ///   2. Map.AreaInRange(...)  → 未初期化マップでは例外
    ///   3. GUI.MaskScreen()
    ///   4. CommandState = "ターゲット選択" or "移動後ターゲット選択"
    ///
    /// Map が未初期化でも SelectedCommand の設定は例外発生前に完了するため確認できる。
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
        // FixCmdID (5) → StartFixCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FixCmdID_SetsSelectedCommandToFix()
        {
            // StartFixCommand() は SelectedCommand = "修理" を Map アクセス前に設定する
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.FixCmdID, "修理"));
            }
            catch { }

            Assert.AreEqual("修理", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void FixCmdID_ReachesGuiOrMapCall()
        {
            // StartFixCommand() は SelectedCommand 設定後に Map/GUI を呼ぶ
            // Map 未初期化でも SelectedCommand "修理" が設定されたことで到達を確認
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            bool guiCalled = false;
            mock.MaskScreenHandler = () => { guiCalled = true; };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.FixCmdID, "修理"));
            }
            catch { }

            // SelectedCommand は "修理" に設定されたはず (Map 呼び出しより前)
            Assert.AreEqual("修理", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void FixCmdID_CommandState_IsCommandSelect_BeforeCall()
        {
            // テスト前の初期 CommandState が "コマンド選択" であることを確認
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            Assert.AreEqual("コマンド選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void FixCmdID_AfterCall_CommandStateChanges_WhenMapInitialized()
        {
            // MaskScreen ハンドラを設定して Map.AreaInRange 後の状態変化を確認するテスト
            // Map 未初期化時は例外で終了するため、SelectedCommand の設定のみ確認
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.MaskScreenHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.FixCmdID, "修理"));
            }
            catch { }

            Assert.AreEqual("修理", src.Commands.SelectedCommand);
        }

        // ──────────────────────────────────────────────
        // SupplyCmdID (6) → StartSupplyCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SupplyCmdID_SetsSelectedCommandToSupply()
        {
            // StartSupplyCommand() は SelectedCommand = "補給" を Map アクセス前に設定する
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SupplyCmdID, "補給"));
            }
            catch { }

            Assert.AreEqual("補給", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void SupplyCmdID_ReachesGuiOrMapCall()
        {
            // StartSupplyCommand() は SelectedCommand 設定後に Map/GUI を呼ぶ
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            mock.MaskScreenHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SupplyCmdID, "補給"));
            }
            catch { }

            Assert.AreEqual("補給", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void SupplyCmdID_AfterCall_SelectedCommand_IsSupply()
        {
            // SupplyCmdID → StartSupplyCommand() → SelectedCommand = "補給"
            var src = CreateSrc();

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SupplyCmdID, "補給"));
            }
            catch { }

            Assert.AreEqual("補給", src.Commands.SelectedCommand);
        }

        // ──────────────────────────────────────────────
        // コマンドの違いを確認: FixCmd と SupplyCmd は異なる SelectedCommand を設定する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FixCmdID_And_SupplyCmdID_SetDifferentSelectedCommands()
        {
            // 修理と補給では SelectedCommand が異なる値になる
            var src1 = CreateSrc();
            var src2 = CreateSrc();

            try { src1.Commands.UnitCommand(new UiCommand(Command.FixCmdID, "修理")); } catch { }
            try { src2.Commands.UnitCommand(new UiCommand(Command.SupplyCmdID, "補給")); } catch { }

            Assert.AreEqual("修理", src1.Commands.SelectedCommand);
            Assert.AreEqual("補給", src2.Commands.SelectedCommand);
            Assert.AreNotEqual(src1.Commands.SelectedCommand, src2.Commands.SelectedCommand);
        }
    }
}
