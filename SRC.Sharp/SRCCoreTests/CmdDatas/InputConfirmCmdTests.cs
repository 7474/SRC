using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// ConfirmCmd / InputCmd のユニットテスト
    /// ヘルプの記載に基づく期待値を検証する
    /// </summary>
    [TestClass]
    public class InputConfirmCmdTests
    {
        private (SRC src, MockGUI gui) CreateSrc()
        {
            var gui = new MockGUI();
            var src = new SRC { GUI = gui };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new System.Collections.Generic.Queue<string>();
            return (src, gui);
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
        // ConfirmCmd
        // ヘルプ: OK→選択=1、キャンセル→選択=0
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ConfirmCmd_OkSelected_SetsSelectedAlternativeToOne()
        {
            // ヘルプ: ＯＫを選択した場合にはシステム変数選択に1が設定される
            var (src, gui) = CreateSrc();
            gui.ConfirmHandler = (msg, title, opts) => GuiDialogResult.Ok;
            var cmd = CreateCmd(src, "Confirm 本当に実行しますか？");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.AreEqual("1", src.Event.SelectedAlternative);
        }

        [TestMethod]
        public void ConfirmCmd_CancelSelected_SetsSelectedAlternativeToZero()
        {
            // ヘルプ: キャンセルの場合は0が設定される
            var (src, gui) = CreateSrc();
            gui.ConfirmHandler = (msg, title, opts) => GuiDialogResult.Cancel;
            var cmd = CreateCmd(src, "Confirm 本当に実行しますか？");
            cmd.Exec();
            Assert.AreEqual("0", src.Event.SelectedAlternative);
        }

        [TestMethod]
        public void ConfirmCmd_PassesMessageToGUI()
        {
            // ヘルプ: message には選択肢のダイアログに表示される説明文を指定する
            var (src, gui) = CreateSrc();
            string capturedMsg = null;
            gui.ConfirmHandler = (msg, title, opts) => { capturedMsg = msg; return GuiDialogResult.Ok; };
            var cmd = CreateCmd(src, "Confirm 本当に実行しますか？");
            cmd.Exec();
            Assert.AreEqual("本当に実行しますか？", capturedMsg);
        }

        [TestMethod]
        public void ConfirmCmd_WrongArgCount_ReturnsError()
        {
            // 引数なし → エラー
            var (src, _) = CreateSrc();
            var cmd = CreateCmd(src, "Confirm");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ConfirmCmd_TooManyArgs_ReturnsError()
        {
            // 引数3個 → エラー
            var (src, _) = CreateSrc();
            var cmd = CreateCmd(src, "Confirm メッセージ 余分な引数");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // InputCmd
        // ヘルプ: 入力した文字列をvariableに格納。キャンセル時は空文字列
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InputCmd_OkWithInput_SetsVariable()
        {
            // ヘルプ: 入力された文字列は変数variableに格納される
            var (src, gui) = CreateSrc();
            gui.InputHandler = (msg, title, def) => (GuiDialogResult.Ok, "テスト入力");
            var cmd = CreateCmd(src, "Input myVar メッセージを入力してください");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.AreEqual("テスト入力", src.Expression.GetValueAsString("myVar"));
        }

        [TestMethod]
        public void InputCmd_CancelSelected_SetsVariableToEmpty()
        {
            // ヘルプ: 入力がキャンセルされた場合は空文字列が格納される
            var (src, gui) = CreateSrc();
            gui.InputHandler = (msg, title, def) => (GuiDialogResult.Cancel, "");
            var cmd = CreateCmd(src, "Input myVar メッセージを入力してください");
            cmd.Exec();
            Assert.AreEqual("", src.Expression.GetValueAsString("myVar"));
        }

        [TestMethod]
        public void InputCmd_WithDefaultValue_PassesDefaultToGUI()
        {
            // ヘルプ: valueを指定すると指定した文字列がダイアログの入力欄に表示される
            var (src, gui) = CreateSrc();
            string capturedDefault = null;
            gui.InputHandler = (msg, title, def) => { capturedDefault = def; return (GuiDialogResult.Ok, def); };
            var cmd = CreateCmd(src, "Input myVar メッセージ デフォルト値");
            cmd.Exec();
            Assert.AreEqual("デフォルト値", capturedDefault);
        }

        [TestMethod]
        public void InputCmd_WithoutDefaultValue_PassesEmptyDefault()
        {
            // ヘルプ: valueを省略した場合は空白が表示される
            var (src, gui) = CreateSrc();
            string capturedDefault = null;
            gui.InputHandler = (msg, title, def) => { capturedDefault = def; return (GuiDialogResult.Ok, ""); };
            var cmd = CreateCmd(src, "Input myVar メッセージ");
            cmd.Exec();
            Assert.AreEqual("", capturedDefault);
        }

        [TestMethod]
        public void InputCmd_WrongArgCount_ReturnsError()
        {
            // 引数が1個（variable のみ）→ エラー
            var (src, _) = CreateSrc();
            var cmd = CreateCmd(src, "Input myVar");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void InputCmd_TooManyArgs_ReturnsError()
        {
            // 引数が5個以上 → エラー
            var (src, _) = CreateSrc();
            var cmd = CreateCmd(src, "Input myVar msg default extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }
    }
}
