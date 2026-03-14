using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// Option, Event系コマンドのユニットテスト
    /// </summary>
    [TestClass]
    public class OptionAndEventCmdTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new System.Collections.Generic.Queue<string>();
            return src;
        }

        private CmdData CreateCmd(SRC src, string cmdText, int id = 0)
        {
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, cmdText);
            src.Event.EventData.Add(line);
            var parser = new CmdParser();
            var cmd = parser.Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        // ──────────────────────────────────────────────
        // OptionCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void OptionCmd_TwoArgs_SetsGlobalVariable()
        {
            // ヘルプ: Option 名前 → Option(名前)=1 のグローバル変数を設定
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Option 新ＧＵＩ");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("Option(新ＧＵＩ)"));
            Assert.AreEqual(1, src.Expression.GetValueAsLong("Option(新ＧＵＩ)"));
        }

        [TestMethod]
        public void OptionCmd_TwoArgs_AlreadyDefined_SetsValue()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(機能)");
            src.Expression.SetVariableAsLong("Option(機能)", 0);
            var cmd = CreateCmd(src, "Option 機能");
            cmd.Exec();
            Assert.AreEqual(1, src.Expression.GetValueAsLong("Option(機能)"));
        }

        [TestMethod]
        public void OptionCmd_ThreeArgs_Off_RemovesVariable()
        {
            // ヘルプ: Option 名前 Off → 変数を削除
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(機能)");
            var cmd = CreateCmd(src, "Option 機能 Off");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.IsFalse(src.Expression.IsGlobalVariableDefined("Option(機能)"));
        }

        [TestMethod]
        public void OptionCmd_ThreeArgs_NotDefined_NoError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Option 存在しない機能 Off");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void OptionCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Option");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void OptionCmd_MultipleDifferentOptions_AllSet()
        {
            var src = CreateSrc();
            var cmd1 = CreateCmd(src, "Option 機能A", 0);
            var cmd2 = CreateCmd(src, "Option 機能B", 1);
            cmd1.Exec();
            cmd2.Exec();
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("Option(機能A)"));
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("Option(機能B)"));
        }

        // ──────────────────────────────────────────────
        // ClearEventCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ClearEventCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ClearEvent arg1 arg2 arg3");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ClearEventCmd_NoArgs_ExecutesSuccessfully()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ClearEvent", 0);
            var result = cmd.Exec();
            // CurrentLabel < 0 なら何もしないが、エラーにもならない
            Assert.AreEqual(1, result);
        }

        // ──────────────────────────────────────────────
        // RestoreEventCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RestoreEventCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RestoreEvent");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void RestoreEventCmd_WithLabel_ReturnsNextId()
        {
            // ヘルプ: RestoreEvent label — 無効化されているラベルを再び有効化する
            // 存在しないラベルでも正常に処理が完了する
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RestoreEvent SomeLabel", 3);
            var result = cmd.Exec();
            Assert.AreEqual(4, result);
        }

        // ──────────────────────────────────────────────
        // ExitCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExitCmd_NoArgs_ReturnsMinusOne()
        {
            // ヘルプ: イベントを終了します — Exitコマンドが実行されるまではイベントの実行は終わりません
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Exit");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SetMessageCmd (arg count error cases)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetMessageCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetMessage");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void SetMessageCmd_OnlyKey_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetMessage OnlyKey");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SetStockCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetStockCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetStock");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SetRelationCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetRelationCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SetRelation");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // IntermissionCommandCmd
        // ヘルプ: インターミッションのメニューに新規コマンドを追加する
        // 書式: IntermissionCommand command file
        //       IntermissionCommand command 削除
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IntermissionCommandCmd_RegistersCommand_SetsGlobalVariable()
        {
            // ヘルプ: プレイヤーがコマンドcommandを選択するとイベントファイルfileが実行される
            var src = CreateSrc();
            var cmd = CreateCmd(src, "IntermissionCommand あらすじ あらすじ.eve");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("IntermissionCommand(あらすじ)"));
            Assert.AreEqual("あらすじ.eve", src.Expression.GetValueAsString("IntermissionCommand(あらすじ)"));
        }

        [TestMethod]
        public void IntermissionCommandCmd_DeleteCommand_UndefinresVariable()
        {
            // ヘルプ: 「削除」と指定することで追加したインターミッションコマンドを削除できる
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("IntermissionCommand(あらすじ)");
            src.Expression.SetVariableAsString("IntermissionCommand(あらすじ)", "あらすじ.eve");

            var cmd = CreateCmd(src, "IntermissionCommand あらすじ 削除");
            var result = cmd.Exec();

            Assert.AreEqual(1, result);
            Assert.IsFalse(src.Expression.IsGlobalVariableDefined("IntermissionCommand(あらすじ)"));
        }

        [TestMethod]
        public void IntermissionCommandCmd_OverwriteExistingCommand_UpdatesFile()
        {
            // ヘルプ: 同じcommandに対してIntermissionCommandコマンドを再度実行すればファイル名を変更できる
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("IntermissionCommand(あらすじ)");
            src.Expression.SetVariableAsString("IntermissionCommand(あらすじ)", "旧ファイル.eve");

            var cmd = CreateCmd(src, "IntermissionCommand あらすじ 新ファイル.eve");
            cmd.Exec();

            Assert.AreEqual("新ファイル.eve", src.Expression.GetValueAsString("IntermissionCommand(あらすじ)"));
        }

        [TestMethod]
        public void IntermissionCommandCmd_WrongArgCount_ReturnsError()
        {
            // ArgNum != 3 の場合はエラー (引数が2つ以下 or 4つ以上)
            var src = CreateSrc();
            var cmd = CreateCmd(src, "IntermissionCommand あらすじ");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void IntermissionCommandCmd_NoArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "IntermissionCommand");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void IntermissionCommandCmd_DeleteNonExistent_NoError()
        {
            // 未定義のコマンドを削除しようとしても例外が出ない
            var src = CreateSrc();
            var cmd = CreateCmd(src, "IntermissionCommand 存在しない 削除");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.IsFalse(src.Expression.IsGlobalVariableDefined("IntermissionCommand(存在しない)"));
        }
    }
}
