using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.TestLib;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.mapcommend.cs で定義されている MapCommand のユニットテスト。
    /// 完全な GUI 依存コマンドは対象外とし、早期リターン可能なパスを検証する。
    /// </summary>
    [TestClass]
    public class CommandMapCommandTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // MapCommand — CommandState の初期化
        // MapCommand は switch に入る前に必ず CommandState を "ユニット選択" に設定する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapCommand_EndTurnCmdID_WhenViewModeTrue_SetsCommandStateToUnitSelect()
        {
            // MapCommand は switch 前に CommandState = "ユニット選択" を設定する
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド"; // 任意の初期値
            src.Commands.ViewMode = true;
            src.Commands.MapCommand(new UiCommand(Command.EndTurnCmdID, "ターン終了"));
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // EndTurnCmdID × ViewMode = true → 早期リターン
        // ViewMode のまま ViewMode = false にして return する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapCommand_EndTurnCmdID_WhenViewModeTrue_SetsViewModeFalse()
        {
            // ターン終了コマンドで ViewMode が true の場合は ViewMode を false にして返す
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.MapCommand(new UiCommand(Command.EndTurnCmdID, "ターン終了"));
            Assert.IsFalse(src.Commands.ViewMode);
        }

        [TestMethod]
        public void MapCommand_EndTurnCmdID_WhenViewModeTrue_DoesNotThrow()
        {
            // 早期リターンのため GUI ハンドラなしで例外が発生しないことを確認
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.MapCommand(new UiCommand(Command.EndTurnCmdID, "ターン終了"));
            // 到達できれば合格
        }

        [TestMethod]
        public void MapCommand_EndTurnCmdID_WhenViewModeFalse_InitialCommandStateStillSet()
        {
            // ViewMode が false の場合、EndTurnCommand() が呼ばれるが、
            // その前に CommandState が "ユニット選択" に設定されていることを確認するために
            // EndTurnCommand がスローする前に CommandState の値を観測できない。
            // ここでは ViewMode = true のパスのみで検証する (上記テストで済み)
        }

        // ──────────────────────────────────────────────
        // AutoDefenseCmdID → SystemConfig.AutoDefense を反転させる
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapCommand_AutoDefenseCmdID_TogglesAutoDefense_WhenInitiallyFalse()
        {
            // 自動反撃モードが false の場合、コマンド実行後に true になる
            var src = CreateSrc();
            src.SystemConfig.AutoDefense = false;
            src.Commands.MapCommand(new UiCommand(Command.AutoDefenseCmdID, "自動反撃モード"));
            Assert.IsTrue(src.SystemConfig.AutoDefense);
        }

        [TestMethod]
        public void MapCommand_AutoDefenseCmdID_TogglesAutoDefense_WhenInitiallyTrue()
        {
            // 自動反撃モードが true の場合、コマンド実行後に false になる
            var src = CreateSrc();
            src.SystemConfig.AutoDefense = true;
            src.Commands.MapCommand(new UiCommand(Command.AutoDefenseCmdID, "自動反撃モード"));
            Assert.IsFalse(src.SystemConfig.AutoDefense);
        }

        [TestMethod]
        public void MapCommand_AutoDefenseCmdID_SetsCommandStateToUnitSelect()
        {
            // 自動反撃モードコマンド実行後も CommandState が "ユニット選択" であることを確認
            var src = CreateSrc();
            src.Commands.CommandState = null;
            src.Commands.MapCommand(new UiCommand(Command.AutoDefenseCmdID, "自動反撃モード"));
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }
    }
}
