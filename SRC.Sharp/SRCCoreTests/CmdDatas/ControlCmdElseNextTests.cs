using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// Else/ElseIf/EndIf/Loop(エラー)/Next(エラー) コマンドのユニットテスト
    /// ヘルプの記載に基づく期待値を検証する
    /// </summary>
    [TestClass]
    public class ControlCmdElseNextTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC
            {
                GUI = new MockGUI(),
            };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new Queue<string>();
            return src;
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

        private void RunEvent(SRC src, CmdData[] cmds, int startId = 0, int maxSteps = 1000)
        {
            var current = startId;
            for (var step = 0; step < maxSteps && current >= 0 && current < cmds.Length; step++)
            {
                current = cmds[current].Exec();
            }
        }

        // ──────────────────────────────────────────────
        // ElseCmd
        // ヘルプ: If～ElseIfによって実行されたブロックの終端を意味する。
        //         Else以降のコマンドはEndIfまでスキップされる。
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ElseCmd_WhenIfWasTrue_SkipsElseBlock()
        {
            // If条件がTrueのとき、Then節が実行された後にElseに到達する。
            // ElseはEndIfの次へジャンプするため、Else節は実行されない。
            var src = CreateSrc();

            // If (1) Then
            //   Set result "then"
            // Else
            //   Set result "else"
            // EndIf
            // Set after 1
            var cmds = BuildEvent(src,
                "If (1) Then",          // ID=0
                "Set result \"then\"",  // ID=1
                "Else",                 // ID=2: jumps to after EndIf
                "Set result \"else\"",  // ID=3: skipped
                "EndIf",                // ID=4
                "Set after 1"           // ID=5
            );

            RunEvent(src, cmds);

            Assert.AreEqual("then", src.Expression.GetValueAsString("result"));
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("after"));
        }

        [TestMethod]
        public void ElseCmd_MissingEndIf_ReturnsError()
        {
            // ElseにはEndIfが対応している必要がある。EndIfがなければエラー。
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "Else"  // ID=0: EndIfなし → エラー
            );

            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ElseIfCmd
        // ヘルプ: 直前の If/ElseIf のTrue節終端として機能する。
        //         条件が成立する場合は ElseIf 節を実行する。
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ElseIfCmd_WhenPreviousConditionTrue_SkipsElseIfBlock()
        {
            // Ifが真のとき、Then節終了後にElseIfに到達する。
            // ElseIfはElseと同じくEndIfの次へジャンプするため、ElseIf節は実行されない。
            var src = CreateSrc();

            // If (1) Then
            //   Set result "then"
            // ElseIf (1) Then
            //   Set result "elseif"
            // EndIf
            // Set after 1
            var cmds = BuildEvent(src,
                "If (1) Then",              // ID=0
                "Set result \"then\"",      // ID=1
                "ElseIf (1) Then",          // ID=2: jumps to after EndIf
                "Set result \"elseif\"",    // ID=3: skipped
                "EndIf",                    // ID=4
                "Set after 1"               // ID=5
            );

            RunEvent(src, cmds);

            Assert.AreEqual("then", src.Expression.GetValueAsString("result"));
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("after"));
        }

        [TestMethod]
        public void ElseIfCmd_WhenPreviousConditionFalse_ConditionChecked()
        {
            // ヘルプ: IfがFalseのとき、ElseIf条件が評価される。Trueなら ElseIf 節を実行。
            var src = CreateSrc();
            src.Expression.SetVariableAsString("x", "2");

            // If (x = 1) Then
            //   Set result "if"
            // ElseIf (x = 2) Then
            //   Set result "elseif"
            // EndIf
            var cmds = BuildEvent(src,
                "If (x = 1) Then",          // ID=0: false
                "Set result \"if\"",        // ID=1: skipped
                "ElseIf (x = 2) Then",      // ID=2: true → execute ElseIf block
                "Set result \"elseif\"",    // ID=3
                "EndIf"                     // ID=4
            );

            RunEvent(src, cmds);

            Assert.AreEqual("elseif", src.Expression.GetValueAsString("result"));
        }

        [TestMethod]
        public void ElseIfCmd_MissingEndIf_ReturnsError()
        {
            // ElseIfにはEndIfが対応している必要がある。EndIfがなければエラー。
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "If (1) Then",          // ID=0: true
                "Set dummy 1",          // ID=1
                "ElseIf (1) Then"       // ID=2: executed after true If, but no EndIf
            );

            // ElseIfCmdに到達したとき（Ifがtrue）、ToEndでEndIfを探すがない → エラー
            RunEvent(src, cmds);
            // cmds[2] would have caused -1 if reached; verify ElseIf was executed
            var directResult = cmds[2].Exec();
            Assert.AreEqual(-1, directResult);
        }

        // ──────────────────────────────────────────────
        // EndIfCmd
        // ヘルプ: If/ElseIf/Else ブロックの終端を意味する。
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EndIfCmd_ReturnsNextId()
        {
            // ヘルプ: EndIfは単に次のコマンドへ移る
            var src = CreateSrc();
            var cmds = BuildEvent(src, "EndIf");

            var result = cmds[0].Exec();
            Assert.AreEqual(1, result); // NextID = 1
        }

        [TestMethod]
        public void EndIfCmd_IsCorrectType()
        {
            // EndIfコマンドが正しくパースされることを確認
            var src = CreateSrc();
            var cmds = BuildEvent(src, "EndIf");

            Assert.IsInstanceOfType(cmds[0], typeof(EndIfCmd));
        }

        // ──────────────────────────────────────────────
        // LoopCmd
        // ヘルプ: Do文とLoop文の間のコマンドを繰り返し実行する。
        //         Loop While/Until で終了条件を指定できる。
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LoopCmd_WrongArgCount_ReturnsError()
        {
            // ヘルプ: Loop または Loop While/Until expr の3形式。それ以外はエラー。
            var src = CreateSrc();
            var cmds = BuildEvent(src, "Loop a b c d");  // 5 args → error

            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void LoopCmd_InvalidKeyword_ReturnsError()
        {
            // "While"/"Until"以外のキーワードはエラー
            var src = CreateSrc();
            var cmds = BuildEvent(src, "Loop BadKeyword 1");  // ID=0

            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void LoopCmd_MissingDo_ReturnsError()
        {
            // LoopにはDo文が必要。DoなしのLoopはエラー。
            var src = CreateSrc();
            var cmds = BuildEvent(src, "Loop");  // ID=0: no Do before it

            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void LoopCmd_WhileCondition_ExitsWhenFalse()
        {
            // ヘルプ: Loop While expression - 条件がFalseになったらループ終了
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("i", 0d);

            // Do
            //   Incr i
            // Loop While (i < 3)
            var cmds = BuildEvent(src,
                "Do",                       // ID=0
                "Incr i",                   // ID=1
                "Loop While (i < 3)"        // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("i"));
        }

        [TestMethod]
        public void LoopCmd_UntilCondition_ExitsWhenTrue()
        {
            // ヘルプ: Loop Until expression - 条件がTrueになったらループ終了
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("i", 0d);

            // Do
            //   Incr i
            // Loop Until (i = 3)
            var cmds = BuildEvent(src,
                "Do",                       // ID=0
                "Incr i",                   // ID=1
                "Loop Until (i = 3)"        // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("i"));
        }

        // ──────────────────────────────────────────────
        // NextCmd
        // ヘルプ: Forループの終端を意味する。変数をインクリメントしてFor先頭へ戻る。
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NextCmd_MissingFor_ReturnsError()
        {
            // NextにはFor文が必要。ForなしのNextはエラー。
            var src = CreateSrc();
            var cmds = BuildEvent(src, "Next");  // ID=0: no For before it

            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void NextCmd_IsCorrectType()
        {
            // Nextコマンドが正しくパースされることを確認
            var src = CreateSrc();
            var cmds = BuildEvent(src, "Next");

            Assert.IsInstanceOfType(cmds[0], typeof(NextCmd));
        }
    }
}
