using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// Return・PaintString・PSet・Question・Read・RemoveFolder を元の SRC（CmdData.cls の
    /// ExecReturnCmd・ExecPaintStringCmd・ExecPSetCmd・ExecQuestionCmd・ExecReadCmd・ExecRemoveFolderCmd）と突き合わせたテスト
    /// </summary>
    [TestClass]
    public class ReturnToRemoveFolderCmdVBMatchTests
    {
        private static (SRC src, MockGUI gui) CreateSrc()
        {
            var gui = new MockGUI();
            var src = new SRC { GUI = gui };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            return (src, gui);
        }

        private static CmdData[] BuildEvent(SRC src, params string[] lines)
        {
            var parser = new CmdParser();
            var cmds = new CmdData[lines.Length];
            for (var i = 0; i < lines.Length; i++)
            {
                var line = new EventDataLine(i, EventDataSource.Scenario, "test", i, lines[i]);
                src.Event.EventData.Add(line);
                cmds[i] = parser.Parse(src, line);
                src.Event.EventCmd.Add(cmds[i]);
                var trimmed = lines[i].Trim();
                if (trimmed.EndsWith(":"))
                {
                    src.Event.AddLabel(trimmed.Substring(0, trimmed.Length - 1), i);
                }
            }
            return cmds;
        }

        private static IList<string> Errors(SRC src)
        {
            return src.DataErrors.Select(x => x.msg).ToList();
        }

        // ── ExecReturnCmd ──

        [TestMethod]
        public void Return_AtEventTopWithoutCallFromDepth1_IsError()
        {
            // VB: CallDepth = 1 And CallStack(1) = 0 のときは「CallコマンドとReturnコマンドが対応していません」
            // （HandleEvent は CallStack を積まずに CallDepth を 1 にするので、イベントの最上位の Return が当たる）
            var (src, _) = CreateSrc();
            var cmds = BuildEvent(src, "Set a 1", "Return");
            src.Event.CallDepth = 1;
            src.Event.CallStack[0] = 5;
            src.Event.CallStack[1] = 0;
            Assert.AreEqual(-1, cmds[1].Exec());
            Assert.AreEqual(1, src.Event.CallDepth);
            CollectionAssert.Contains(Errors(src).ToList(), "CallコマンドとReturnコマンドが対応していません");
        }

        [TestMethod]
        public void Return_AtDepth1WithCallStack1Set_ReturnsToCaller()
        {
            // VB: CallStack(1) が 0 でなければ（1 階層目から Call したことがあれば）CallStack(0) + 1 に戻る
            var (src, _) = CreateSrc();
            var cmds = BuildEvent(src, "Set a 1", "Return");
            src.Event.CallDepth = 1;
            src.Event.CallStack[0] = 5;
            src.Event.CallStack[1] = 3;
            Assert.AreEqual(6, cmds[1].Exec());
            Assert.AreEqual(0, src.Event.CallDepth);
        }

        [TestMethod]
        public void Return_FromSubroutineCalledInEvent_ReturnsToCaller()
        {
            var (src, _) = CreateSrc();
            var cmds = BuildEvent(src, "Call sub", "Exit", "sub:", "Return");
            src.Event.CallDepth = 1;
            Assert.AreEqual(3, cmds[0].Exec());
            Assert.AreEqual(2, src.Event.CallDepth);
            Assert.AreEqual(1, cmds[3].Exec());
            Assert.AreEqual(1, src.Event.CallDepth);
        }

        // ── ExecPaintStringCmd ──

        [TestMethod]
        public void PaintString_FixedCoordinateExpressionNotNumeric_IsError()
        {
            // VB: 構文解析で最初の 2 項が「(」で始まる式なので座標指定が確定し、CInt("abc") が実行時エラー
            var (src, gui) = CreateSrc();
            var drawn = new List<string>();
            gui.DrawStringHandler = (m, x, y, wcr) => drawn.Add(m);
            src.Expression.SetVariableAsString("x", "abc");
            var cmds = BuildEvent(src, "PaintString (x) 10 文字");
            Assert.AreEqual(-1, cmds[0].Exec());
            Assert.AreEqual(0, drawn.Count);
            CollectionAssert.Contains(Errors(src).ToList(), "イベントデータが不正です");
        }

        [TestMethod]
        public void PaintString_FixedCoordinateExpressionNumeric_DrawsAtPosition()
        {
            var (src, gui) = CreateSrc();
            (string m, int x, int y) drawn = default;
            gui.DrawStringHandler = (m, x, y, wcr) => drawn = (m, x, y);
            src.Expression.SetVariableAsString("x", "12");
            var cmds = BuildEvent(src, "PaintString (x) 34 文字");
            Assert.AreEqual(1, cmds[0].Exec());
            Assert.AreEqual(("文字", 12, 34), drawn);
        }

        [TestMethod]
        public void PaintString_UndecidedTermsNotNumeric_DrawsWholeTail()
        {
            // VB: 「x」は「-」・数値・式のどれでもないので実行時まで不明。値が数値でなければ全体を文字列として表示する
            var (src, gui) = CreateSrc();
            (string m, int x, int y) drawn = default;
            gui.DrawStringHandler = (m, x, y, wcr) => drawn = (m, x, y);
            src.Expression.SetVariableAsString("x", "abc");
            var cmds = BuildEvent(src, "PaintString x 10 文字");
            Assert.AreEqual(1, cmds[0].Exec());
            Assert.AreEqual(("x 10 文字", -1, -1), drawn);
        }

        [TestMethod]
        public void PaintString_UndecidedTermsNumeric_DrawsAtPosition()
        {
            var (src, gui) = CreateSrc();
            (string m, int x, int y) drawn = default;
            gui.DrawStringHandler = (m, x, y, wcr) => drawn = (m, x, y);
            src.Expression.SetVariableAsString("x", "7");
            var cmds = BuildEvent(src, "PaintString x 10 文字");
            Assert.AreEqual(1, cmds[0].Exec());
            Assert.AreEqual(("文字", 7, 10), drawn);
        }

        // ── ExecPSetCmd ──

        private static (SRC src, MockGUIScrean screan) CreatePSetSrc()
        {
            var (src, gui) = CreateSrc();
            gui.MapPWidth = 640;
            gui.MapPHeight = 480;
            gui.SaveScreenHandler = () => { };
            var screan = new MockGUIScrean();
            src.GUIScrean = screan;
            return (src, screan);
        }

        [TestMethod]
        public void PSet_OutOfScreenWithInvalidColor_IsNotError()
        {
            // VB: 画面の外なら色指定を確かめる前に終わる
            var (src, _) = CreatePSetSrc();
            var cmds = BuildEvent(src, "PSet 1000 10 BADCOLOR");
            Assert.AreEqual(1, cmds[0].Exec());
        }

        [TestMethod]
        [DataRow("PSet 10 10 #FFF")]
        [DataRow("PSet 10 10 Red")]
        [DataRow("PSet 10 10 #GGGGGG")]
        public void PSet_ColorNotSevenCharHex_IsError(string line)
        {
            // VB: Asc(opt) <> 35 Or Len(opt) <> 7、または 16 進数として読めなければ「色指定が不正です」
            var (src, _) = CreatePSetSrc();
            var cmds = BuildEvent(src, line);
            Assert.AreEqual(-1, cmds[0].Exec());
            CollectionAssert.Contains(Errors(src).ToList(), "色指定が不正です");
        }

        [TestMethod]
        public void PSet_SevenCharHexColor_Draws()
        {
            var (src, _) = CreatePSetSrc();
            var cmds = BuildEvent(src, "PSet 10 10 #FF0000");
            Assert.AreEqual(1, cmds[0].Exec());
        }

        // ── ExecQuestionCmd ──

        [TestMethod]
        public void Question_EndWithArgument_IsChoice()
        {
            // VB: 式置換の後の行が「end」と等しいときだけ終わる。「End x」は選択肢
            var (src, gui) = CreateSrc();
            List<string> items = null;
            gui.LIPSHandler = (args, t) =>
            {
                items = args.Items.Select(x => x.Text).ToList();
                return 2;
            };
            var cmds = BuildEvent(src, "Question 0", "選択肢A", "End x", "END", "Set a 1");
            Assert.AreEqual(4, cmds[0].Exec());
            CollectionAssert.AreEqual(new[] { "選択肢A", "End x" }, items);
            Assert.AreEqual("2", src.Event.SelectedAlternative);
        }

        [TestMethod]
        public void Question_TooManyArgsWithChoices_IsError()
        {
            // VB: 引数の数は Select Case ArgNum の Case 3・Case 2 だけ。それ以外は「Questionコマンドの引数の数が違います」
            var (src, gui) = CreateSrc();
            gui.LIPSHandler = (args, t) => 1;
            var cmds = BuildEvent(src, "Question 0 メッセージ 余分", "選択肢A", "End");
            Assert.AreEqual(-1, cmds[0].Exec());
            CollectionAssert.Contains(Errors(src).ToList(), "Questionコマンドの引数の数が違います");
        }

        [TestMethod]
        [DataRow("Question")]
        [DataRow("Question 0 メッセージ 余分")]
        public void Question_WrongArgCountWithoutChoices_IsNotError(string line)
        {
            // VB: 選択肢が無いときは引数の数を確かめずに SelectedItem = 0
            var (src, _) = CreateSrc();
            var cmds = BuildEvent(src, line, "", "End");
            Assert.AreEqual(3, cmds[0].Exec());
            Assert.AreEqual("0", src.Event.SelectedAlternative);
        }

        // ── ExecReadCmd ──

        [TestMethod]
        public void Read_PastEndOfFile_IsError()
        {
            // VB: Input # でファイルの終わりを越えて読むと実行時エラー（62）になり「イベントデータが不正です」
            var (src, _) = CreateSrc();
            using var ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes("Hello\r\n"));
            var handle = src.FileHandleManager.Add(
                SRCCore.Filesystem.SafeOpenMode.Read,
                SRCCompatibilityMode.None,
                ms);
            var cmds = BuildEvent(src, $"Read {handle.Handle} A B");
            Assert.AreEqual(-1, cmds[0].Exec());
            Assert.AreEqual("Hello", src.Expression.GetValueAsString("A"));
            CollectionAssert.Contains(Errors(src).ToList(), "イベントデータが不正です");
        }

        // ── ExecRemoveFolderCmd ──

        [TestMethod]
        [DataRow(@"RemoveFolder ..\evil", @"ファイル指定に「..\」は使えません")]
        [DataRow("RemoveFolder ../evil", "ファイル指定に「../」は使えません")]
        public void RemoveFolder_ParentPath_ErrorMessageSaysFile(string line, string expected)
        {
            // VB: ExecRemoveFolderCmd の文言は「ファイル指定に…」（CreateFolder は「フォルダ指定に…」）
            var (src, _) = CreateSrc();
            var cmds = BuildEvent(src, line);
            Assert.AreEqual(-1, cmds[0].Exec());
            CollectionAssert.Contains(Errors(src).ToList(), expected);
        }
    }
}
