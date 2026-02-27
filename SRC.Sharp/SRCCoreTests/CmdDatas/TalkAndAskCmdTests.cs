using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// TalkCmd / AskCmd のユニットテスト
    /// ヘルプの記載に基づく期待値を検証する
    /// </summary>
    [TestClass]
    public class TalkAndAskCmdTests
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

        private CmdData[] BuildEvent(SRC src, params string[] lines)
        {
            var parser = new CmdParser();
            var cmds = new CmdData[lines.Length];
            for (var i = 0; i < lines.Length; i++)
            {
                var line = new EventDataLine(i, EventDataSource.Scenario, "test", i, lines[i]);
                src.Event.EventData.Add(line);
                var cmd = parser.Parse(src, line);
                src.Event.EventCmd.Add(cmd);
                cmds[i] = cmd;
            }
            return cmds;
        }

        // ──────────────────────────────────────────────
        // TalkCmd
        // ヘルプ: キャラクター間の会話などのメッセージを表示
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TalkCmd_SimpleMessage_CallsDisplayMessage()
        {
            // ヘルプ: Talk行の後の行がメッセージとして一行ずつメッセージウィンドウに表示される
            var (src, gui) = CreateSrc();
            string capturedMsg = null;
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (pname, msg, mode) => { capturedMsg = msg; };
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk",             // ID=0
                "テストメッセージ",  // ID=1: message body
                "End"               // ID=2
            );

            cmds[0].Exec();

            Assert.AreEqual("テストメッセージ", capturedMsg);
        }

        [TestMethod]
        public void TalkCmd_SimpleMessage_ReturnsIdAfterEnd()
        {
            // ヘルプ: End行まで来るとメッセージは終了する
            var (src, gui) = CreateSrc();
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (p, m, o) => { };
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk",         // ID=0
                "メッセージ",    // ID=1
                "End",          // ID=2 → NextID=3
                "Set x 1"       // ID=3 (not reached by Talk)
            );

            var result = cmds[0].Exec();

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void TalkCmd_WithSistema_DisplaysMessageWithoutPilotCheck()
        {
            // ヘルプ: システムを指定するとメッセージからパイロット名とかぎ括弧が省略される
            // システムはパイロットリストになくてもエラーにならない
            var (src, gui) = CreateSrc();
            string capturedPname = null;
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (pname, msg, mode) => { capturedPname = pname; };
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk システム",     // ID=0: special speaker "システム"
                "システムメッセージ", // ID=1
                "End"               // ID=2
            );

            var result = cmds[0].Exec();

            Assert.AreEqual(3, result);
            Assert.AreEqual("システム", capturedPname);
        }

        [TestMethod]
        public void TalkCmd_MultipleMessages_DisplaysAll()
        {
            // ヘルプ: Talk行の後の行がメッセージとして一行ずつ表示される（複数行可）
            var (src, gui) = CreateSrc();
            var messages = new List<string>();
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (pname, msg, mode) => { messages.Add(msg); };
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk",      // ID=0
                "一行目",    // ID=1
                "二行目",    // ID=2
                "三行目",    // ID=3
                "End"        // ID=4
            );

            cmds[0].Exec();

            Assert.AreEqual(3, messages.Count);
            Assert.AreEqual("一行目", messages[0]);
            Assert.AreEqual("二行目", messages[1]);
            Assert.AreEqual("三行目", messages[2]);
        }

        [TestMethod]
        public void TalkCmd_MissingEnd_ReturnsError()
        {
            // ヘルプ: End行が無い場合、エラーになる
            var (src, gui) = CreateSrc();
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (p, m, o) => { };
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk",      // ID=0
                "メッセージ" // ID=1: no End follows
            );

            var result = cmds[0].Exec();

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void TalkCmd_WithSuspend_ReturnsAfterSuspend()
        {
            // ヘルプ: EndコマンドのかわりにSuspendコマンドを使うと
            // メッセージウィンドウがそのまま表示され続ける
            var (src, gui) = CreateSrc();
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (p, m, o) => { };
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk",         // ID=0
                "メッセージ",    // ID=1
                "Suspend",      // ID=2 → NextID=3
                "Set x 1"       // ID=3 (reached after Suspend)
            );

            var result = cmds[0].Exec();

            // SuspendCmd returns NextID (3), message window stays open
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void TalkCmd_WithSuspend_DoesNotCloseMessageForm()
        {
            // ヘルプ: Suspend行を使った場合、メッセージウィンドウはそのまま表示され続ける
            var (src, gui) = CreateSrc();
            var closedForm = false;
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (p, m, o) => { };
            gui.CloseMessageFormHandler = () => { closedForm = true; };

            var cmds = BuildEvent(src,
                "Talk",
                "メッセージ",
                "Suspend"
            );

            cmds[0].Exec();

            Assert.IsFalse(closedForm);
        }

        [TestMethod]
        public void TalkCmd_WithEnd_ClosesMessageForm()
        {
            // ヘルプ: EndコマンドでメッセージウィンドウをClose
            var (src, gui) = CreateSrc();
            var closedForm = false;
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (p, m, o) => { };
            gui.CloseMessageFormHandler = () => { closedForm = true; };

            var cmds = BuildEvent(src,
                "Talk",
                "メッセージ",
                "End"
            );

            cmds[0].Exec();

            Assert.IsTrue(closedForm);
        }

        [TestMethod]
        public void TalkCmd_UndefinedPilot_ReturnsError()
        {
            // ヘルプ: character に定義されていないパイロット名を指定するとエラー
            var (src, gui) = CreateSrc();
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk 存在しないパイロット", // ID=0: undefined pilot
                "メッセージ",               // ID=1
                "End"                      // ID=2
            );

            var result = cmds[0].Exec();

            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // AskCmd
        // ヘルプ: 選択肢の一覧からなるダイアログを表示し、プレイヤーの選択を促す
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AskCmd_SelectFirstChoice_SetsSelectedAlternativeToOne()
        {
            // ヘルプ: 先頭の選択肢が選択されたときに選択=1（書式１の場合）
            var (src, gui) = CreateSrc();
            gui.ReduceListBoxHeightHandler = () => { };
            gui.ListBoxHandler = args => 1; // user selects first item

            var cmds = BuildEvent(src,
                "Ask どれを選ぶ？",  // ID=0
                "グー",              // ID=1
                "チョキ",            // ID=2
                "パー",              // ID=3
                "End"               // ID=4
            );

            cmds[0].Exec();

            Assert.AreEqual("1", src.Event.SelectedAlternative);
        }

        [TestMethod]
        public void AskCmd_SelectSecondChoice_SetsCorrectValue()
        {
            // ヘルプ: 2番目の選択肢が選択された場合は選択=2
            var (src, gui) = CreateSrc();
            gui.ReduceListBoxHeightHandler = () => { };
            gui.ListBoxHandler = args => 2; // user selects second item

            var cmds = BuildEvent(src,
                "Ask どれを選ぶ？",  // ID=0
                "グー",              // ID=1
                "チョキ",            // ID=2
                "パー",              // ID=3
                "End"               // ID=4
            );

            cmds[0].Exec();

            Assert.AreEqual("2", src.Event.SelectedAlternative);
        }

        [TestMethod]
        public void AskCmd_ReturnsNextIdAfterEnd()
        {
            // ヘルプ: 選択後にEnd直後のコマンドへ進む
            var (src, gui) = CreateSrc();
            gui.ReduceListBoxHeightHandler = () => { };
            gui.ListBoxHandler = args => 1;

            var cmds = BuildEvent(src,
                "Ask どれを選ぶ？",  // ID=0
                "選択肢A",           // ID=1
                "End",              // ID=2 → NextID=3
                "Set x 1"           // ID=3
            );

            var result = cmds[0].Exec();

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void AskCmd_CustomMessage_PassedToListBox()
        {
            // ヘルプ: messageには選択肢のダイアログに表示される説明文を指定する
            var (src, gui) = CreateSrc();
            string capturedInfo = null;
            gui.ReduceListBoxHeightHandler = () => { };
            gui.ListBoxHandler = args =>
            {
                capturedInfo = args.lb_info;
                return 1;
            };

            var cmds = BuildEvent(src,
                "Ask どれを出す？",  // ID=0
                "グー",             // ID=1
                "チョキ",            // ID=2
                "End"               // ID=3
            );

            cmds[0].Exec();

            Assert.AreEqual("どれを出す？", capturedInfo);
        }

        [TestMethod]
        public void AskCmd_DefaultMessage_UsedWhenNoMessage()
        {
            // ヘルプ: messageはmessageを省略した場合にのみ省略可
            // 省略した場合はデフォルトの説明文が表示される
            var (src, gui) = CreateSrc();
            string capturedInfo = null;
            gui.ReduceListBoxHeightHandler = () => { };
            gui.ListBoxHandler = args =>
            {
                capturedInfo = args.lb_info;
                return 1;
            };

            var cmds = BuildEvent(src,
                "Ask",          // ID=0: no message arg
                "選択肢A",       // ID=1
                "End"           // ID=2
            );

            cmds[0].Exec();

            Assert.AreEqual("いずれかを選んでください", capturedInfo);
        }

        [TestMethod]
        public void AskCmd_NoChoices_SetsSelectedAlternativeToZero()
        {
            // ヘルプ: キャンセル時の選択の値は0（書式１の場合）
            // 選択肢が存在しない場合はダイアログを表示せずに終了
            var (src, gui) = CreateSrc();
            // ListBox should NOT be called
            gui.ListBoxHandler = args =>
            {
                Assert.Fail("ListBox should not be called when there are no choices");
                return 0;
            };

            var cmds = BuildEvent(src,
                "Ask どれを選ぶ？",  // ID=0
                "End"               // ID=1: no choices before End
            );

            cmds[0].Exec();

            Assert.AreEqual("0", src.Event.SelectedAlternative);
        }

        [TestMethod]
        public void AskCmd_MissingEnd_ReturnsError()
        {
            // ヘルプ: AskとEndが対応していない場合はエラー
            var (src, gui) = CreateSrc();

            var cmds = BuildEvent(src,
                "Ask どれを選ぶ？",  // ID=0
                "グー",              // ID=1
                "チョキ"             // ID=2: no End
            );

            var result = cmds[0].Exec();

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void AskCmd_Owari_ClosesListBox()
        {
            // ヘルプ: 終了オプションを使う場合はAsk 終了だけを指定し、選択肢及びEnd行は指定しない
            var (src, gui) = CreateSrc();
            var closedListBox = false;
            gui.CloseListBoxHandler = () => { closedListBox = true; };
            gui.ReduceListBoxHeightHandler = () => { };

            var cmds = BuildEvent(src,
                "Ask 終了"   // ID=0: close dialog
            );

            var result = cmds[0].Exec();

            Assert.IsTrue(closedListBox);
            Assert.AreEqual(1, result); // NextID
        }

        [TestMethod]
        public void AskCmd_InvalidOption_ReturnsError()
        {
            // ヘルプ: オプションが不正な場合はエラー
            var (src, gui) = CreateSrc();

            var cmds = BuildEvent(src,
                "Ask メッセージ 不正オプション1 不正オプション2 不正オプション3", // ID=0
                "End" // ID=1
            );

            var result = cmds[0].Exec();

            Assert.AreEqual(-1, result);
        }
    }
}
