using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Pilots;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.unitcommand.cs の公開メソッド (UnitCommand) のユニットテスト
    /// ソースコードを仕様の根拠として使用する
    /// </summary>
    [TestClass]
    public class CommandUnitCommandTests
    {
        /// <summary>テスト用の最小限 IGUIStatus 実装</summary>
        private class StubGUIStatus : IGUIStatus
        {
            public Unit DisplayedUnit { get; set; }
            public Pilot DisplayedPilot { get; set; }
            public void DisplayGlobalStatus() { }
            public void DisplayUnitStatus(Unit u, Pilot p = null) { }
            public void DisplayPilotStatus(Pilot p) { }
            public void InstantUnitStatusDisplay(int X, int Y) { }
            public void ClearUnitStatus() { }
        }

        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.GUIStatus = new StubGUIStatus();
            return src;
        }

        // ──────────────────────────────────────────────
        // UnitCommand — 早期 return パス (移動後コマンド選択)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_MoveCmdId_AfterMoveCommandSelect_ReturnsEarlyWithoutStateChange()
        {
            // 「移動」コマンド (MoveCmdID=0) を CommandState="移動後コマンド選択" のときに呼ぶと
            // なんらかの原因により選択がうまくいかなかった場合として early return する
            var src = CreateSrc();
            src.Commands.SelectedUnit = new Unit(src);
            src.Commands.CommandState = "移動後コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));

            // early return のため CommandState は変化しない
            Assert.AreEqual("移動後コマンド選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void UnitCommand_UnknownCmdId_AfterMoveCommandSelect_ReturnsEarlyWithoutStateChange()
        {
            // 未知のコマンド ID を CommandState="移動後コマンド選択" のときに呼ぶと
            // default ブランチで early return する
            var src = CreateSrc();
            src.Commands.SelectedUnit = new Unit(src);
            src.Commands.CommandState = "移動後コマンド選択";

            src.Commands.UnitCommand(new UiCommand(999, "不明"));

            // default の early return のため CommandState は変化しない
            Assert.AreEqual("移動後コマンド選択", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // UnitCommand — WaitCmdID (「待機」コマンド)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_WaitCmdId_WithPilotlessUnit_SetsUnitSelectState()
        {
            // 「待機」コマンド (WaitCmdID=255) を呼ぶと WaitCommand() が実行される
            // WaitCommand() でパイロットなし → "ユニット選択" に遷移してすぐ return
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            mock.RedrawScreenHandler = (_) => { };

            var unit = new Unit(src);
            // パイロットなし (CountPilot() == 0 になる)
            src.Commands.SelectedUnit = unit;
            src.Commands.CommandState = "コマンド選択";

            src.Commands.UnitCommand(new UiCommand(Command.WaitCmdID, "待機"));

            // WaitCommand() でパイロットなしのため "ユニット選択" に遷移
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }
    }
}

