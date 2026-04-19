using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.TestLib;
using SRCCore.Tests;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.unitsupply.cs のユニットテスト
    /// StartFixCommand, StartSupplyCommand
    /// </summary>
    [TestClass]
    public class CommandSupplyTests
    {
        private SRC CreateSrcWithMap()
        {
            var src = new SRC
            {
                GUI = new MockGUI(),
                GUIMap = new MockGUIMap(),
                GUIStatus = new MockGUIStatus(),
            };
            src.Map.SetMapSize(5, 5);
            src.Map.MapFileName = "test.map";
            var unit = new Unit(src);
            unit.x = 1;
            unit.y = 1;
            src.Commands.SelectedUnit = unit;
            return src;
        }

        // ──────────────────────────────────────────────
        // StartFixCommand: SelectedCommand が "修理" に設定される
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartFixCommand_SetsSelectedCommandToFix()
        {
            var src = CreateSrcWithMap();
            ((MockGUI)src.GUI).MaskScreenHandler = () => { };
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.FixCmdID, "修理"));

            Assert.AreEqual("修理", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void StartFixCommand_FromCommandSelect_SetsTargetSelectState()
        {
            var src = CreateSrcWithMap();
            ((MockGUI)src.GUI).MaskScreenHandler = () => { };
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.FixCmdID, "修理"));

            Assert.AreEqual("ターゲット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void StartFixCommand_FromPostMoveCommandSelect_SetsPostMoveTargetSelectState()
        {
            var src = CreateSrcWithMap();
            ((MockGUI)src.GUI).MaskScreenHandler = () => { };
            src.Commands.CommandState = "移動後コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.FixCmdID, "修理"));

            Assert.AreEqual("移動後ターゲット選択", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // StartSupplyCommand: SelectedCommand が "補給" に設定される
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartSupplyCommand_SetsSelectedCommandToSupply()
        {
            var src = CreateSrcWithMap();
            ((MockGUI)src.GUI).MaskScreenHandler = () => { };
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.SupplyCmdID, "補給"));

            Assert.AreEqual("補給", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void StartSupplyCommand_FromCommandSelect_SetsTargetSelectState()
        {
            var src = CreateSrcWithMap();
            ((MockGUI)src.GUI).MaskScreenHandler = () => { };
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.SupplyCmdID, "補給"));

            Assert.AreEqual("ターゲット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void StartSupplyCommand_FromPostMoveCommandSelect_SetsPostMoveTargetSelectState()
        {
            var src = CreateSrcWithMap();
            ((MockGUI)src.GUI).MaskScreenHandler = () => { };
            src.Commands.CommandState = "移動後コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.SupplyCmdID, "補給"));

            Assert.AreEqual("移動後ターゲット選択", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // Fix/Supply: FixCmdID と SupplyCmdID のディスパッチ確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FixCmdID_IsDispatchedToStartFixCommand()
        {
            // FixCmdID (5) で StartFixCommand が呼ばれ、SelectedCommand="修理" になる
            var src = CreateSrcWithMap();
            ((MockGUI)src.GUI).MaskScreenHandler = () => { };
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.FixCmdID, "修理装置"));

            Assert.AreEqual("修理", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void SupplyCmdID_IsDispatchedToStartSupplyCommand()
        {
            // SupplyCmdID (6) で StartSupplyCommand が呼ばれ、SelectedCommand="補給" になる
            var src = CreateSrcWithMap();
            ((MockGUI)src.GUI).MaskScreenHandler = () => { };
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.SupplyCmdID, "補給装置"));

            Assert.AreEqual("補給", src.Commands.SelectedCommand);
        }
    }
}
