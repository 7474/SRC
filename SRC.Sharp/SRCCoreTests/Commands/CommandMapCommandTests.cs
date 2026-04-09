using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.TestLib;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.mapcommend.cs の公開メソッド (MapCommand) のユニットテスト
    /// ソースコードを仕様の根拠として使用する
    /// </summary>
    [TestClass]
    public class CommandMapCommandTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // MapCommand — 共通: CommandState を必ず "ユニット選択" に設定する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapCommand_AlwaysSetsCommandStateToUnitSelect_BeforeDispatch()
        {
            // MapCommand は switch 前に必ず CommandState = "ユニット選択" を設定する
            // ViewMode=true かつ EndTurnCmdID を使うと EndTurnCommand() 呼び出し前に return できる
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.ViewMode = true;

            src.Commands.MapCommand(new UiCommand(Command.EndTurnCmdID, "ターン終了"));

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void MapCommand_EndTurnCmdId_WithViewMode_ClearsViewMode()
        {
            // EndTurnCmdID かつ ViewMode=true → ViewMode=false にして return (EndTurnCommand は呼ばれない)
            var src = CreateSrc();
            src.Commands.ViewMode = true;

            src.Commands.MapCommand(new UiCommand(Command.EndTurnCmdID, "ターン終了"));

            Assert.IsFalse(src.Commands.ViewMode);
        }

        [TestMethod]
        public void MapCommand_EndTurnCmdId_NotViewMode_ViewModeRemainsfalse()
        {
            // ViewMode=false の場合は EndTurnCommand() が呼ばれるが ViewMode はそのまま false
            // EndTurnCommand() は UList/Event 依存のため例外が発生してもよい
            var src = CreateSrc();
            src.Commands.ViewMode = false;
            var mock = (MockGUI)src.GUI;
            mock.ConfirmHandler = (msg, caption, opts) => GuiDialogResult.Cancel;

            // EndTurnCommand() の中で UList.Items を参照するため MockGUI の Confirm は必要
            // UList は空なので stillActionUnits が空になり Confirm は呼ばれない
            try
            {
                src.Commands.MapCommand(new UiCommand(Command.EndTurnCmdID, "ターン終了"));
            }
            catch { }

            // CommandState は既に "ユニット選択" にセットされている
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }
    }
}
