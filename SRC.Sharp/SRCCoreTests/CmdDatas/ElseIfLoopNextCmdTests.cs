using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// ElseCmd / ElseIfCmd / EndIfCmd / LoopCmd / NextCmd のユニットテスト
    /// ヘルプの記載に基づく期待値を検証する
    /// </summary>
    [TestClass]
    public class ElseIfLoopNextCmdTests
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
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ElseCmd_ReachedAfterTrueBranch_SkipsToEndIf()
        {
            // ヘルプ: Else コマンドは直前の If や ElseIf が True だった場合、EndIf まで処理をスキップする
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 1d);

            // If (x > 0) Then
            //   Set branch "then"
            // Else                  ← reached after true If, should skip to EndIf
            //   Set branch "else"
            // EndIf
            var cmds = BuildEvent(src,
                "If (x > 0) Then",      // ID=0: true
                "Set branch \"then\"",  // ID=1: executed
                "Else",                 // ID=2: skips to EndIf
                "Set branch \"else\"",  // ID=3: skipped
                "EndIf"                 // ID=4
            );

            RunEvent(src, cmds);

            Assert.AreEqual("then", src.Expression.GetValueAsString("branch"));
        }

        [TestMethod]
        public void ElseCmd_ReachedAfterFalseBranch_ExecutesElseBody()
        {
            // ヘルプ: If の条件が偽のとき、Else ブロックの内容が実行される
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 0d);

            var cmds = BuildEvent(src,
                "If (x > 0) Then",      // ID=0: false → skip to Else
                "Set branch \"then\"",  // ID=1: skipped
                "Else",                 // ID=2: entered
                "Set branch \"else\"",  // ID=3: executed
                "EndIf"                 // ID=4
            );

            RunEvent(src, cmds);

            Assert.AreEqual("else", src.Expression.GetValueAsString("branch"));
        }

        [TestMethod]
        public void ElseCmd_MissingEndIf_ReturnsError()
        {
            // EndIf なしのElse はエラー
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 1d);

            var cmds = BuildEvent(src,
                "If (x > 0) Then",      // ID=0: true
                "Set ok 1",             // ID=1
                "Else"                  // ID=2: no EndIf → error
            );

            // Else が実行された時点でエラー
            var result = cmds[2].Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ElseIfCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ElseIfCmd_ConditionTrue_ExecutesBranch()
        {
            // ヘルプ: ElseIf expression Then でelseif の条件が真のとき、続くブロックを実行する
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 2d);

            // If (x = 1) Then
            //   Set result "one"
            // ElseIf (x = 2) Then
            //   Set result "two"
            // EndIf
            var cmds = BuildEvent(src,
                "If (x = 1) Then",          // ID=0: false
                "Set result \"one\"",       // ID=1: skipped
                "ElseIf (x = 2) Then",      // ID=2: true → execute this branch
                "Set result \"two\"",       // ID=3: executed
                "EndIf"                     // ID=4
            );

            RunEvent(src, cmds);

            Assert.AreEqual("two", src.Expression.GetValueAsString("result"));
        }

        [TestMethod]
        public void ElseIfCmd_ReachedAfterTrueBranch_SkipsToEndIf()
        {
            // ヘルプ: 直前の If が True だった場合、ElseIf は EndIf までスキップする
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 1d);

            var cmds = BuildEvent(src,
                "If (x = 1) Then",          // ID=0: true
                "Set result \"one\"",       // ID=1: executed
                "ElseIf (x = 2) Then",      // ID=2: reached after true → skip
                "Set result \"two\"",       // ID=3: skipped
                "EndIf"                     // ID=4
            );

            RunEvent(src, cmds);

            Assert.AreEqual("one", src.Expression.GetValueAsString("result"));
        }

        [TestMethod]
        public void ElseIfCmd_MultipleChain_FirstMatchExecuted()
        {
            // ヘルプ: ElseIf を複数連ねることができ、最初にマッチしたブロックが実行される
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 3d);

            var cmds = BuildEvent(src,
                "If (x = 1) Then",          // ID=0: false
                "Set result \"one\"",       // ID=1: skipped
                "ElseIf (x = 2) Then",      // ID=2: false
                "Set result \"two\"",       // ID=3: skipped
                "ElseIf (x = 3) Then",      // ID=4: true
                "Set result \"three\"",     // ID=5: executed
                "EndIf"                     // ID=6
            );

            RunEvent(src, cmds);

            Assert.AreEqual("three", src.Expression.GetValueAsString("result"));
        }

        // ──────────────────────────────────────────────
        // EndIfCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EndIfCmd_ReturnsNextId()
        {
            // EndIf は処理の区切りとして次のコマンドへ進む
            var src = CreateSrc();

            var cmds = BuildEvent(src,
                "If (1 = 1) Then",  // ID=0: true
                "Set ok 1",         // ID=1
                "EndIf",            // ID=2
                "Set after 1"       // ID=3
            );

            RunEvent(src, cmds);

            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("ok"));
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("after"));
        }

        // ──────────────────────────────────────────────
        // LoopCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LoopCmd_WithoutCondition_AlwaysLoopsBack()
        {
            // ヘルプ: Loop だけの場合は無条件で Do に戻る（Break で脱出可能）
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("i", 0d);

            // Do
            //   Incr i
            //   If (i = 3) Then
            //     Break
            //   EndIf
            // Loop
            var cmds = BuildEvent(src,
                "Do",               // ID=0
                "Incr i",           // ID=1
                "If (i = 3) Then",  // ID=2
                "Break",            // ID=3
                "EndIf",            // ID=4
                "Loop"              // ID=5
            );

            RunEvent(src, cmds);

            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("i"));
        }

        [TestMethod]
        public void LoopCmd_WhileConditionFalse_ExitsLoop()
        {
            // ヘルプ: Loop While expression → expression が偽になったらループ終了
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("i", 0d);

            var cmds = BuildEvent(src,
                "Do",                   // ID=0
                "Incr i",               // ID=1
                "Loop While (i < 3)"    // ID=2: continues while i<3
            );

            RunEvent(src, cmds);

            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("i"));
        }

        [TestMethod]
        public void LoopCmd_UntilConditionTrue_ExitsLoop()
        {
            // ヘルプ: Loop Until expression → expression が真になったらループ終了
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("i", 0d);

            var cmds = BuildEvent(src,
                "Do",                       // ID=0
                "Incr i",                   // ID=1
                "Loop Until (i >= 3)"       // ID=2: exits when i>=3
            );

            RunEvent(src, cmds);

            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("i"));
        }

        [TestMethod]
        public void LoopCmd_WrongArgCount_ReturnsError()
        {
            // Loop に不正な引数はエラー
            var src = CreateSrc();

            var cmds = BuildEvent(src,
                "Do",               // ID=0
                "Loop Invalid arg"  // ID=1: 不正
            );

            var result = cmds[1].Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void LoopCmd_MissingDo_ReturnsError()
        {
            // 対応する Do なしの Loop はエラー
            var src = CreateSrc();

            var cmds = BuildEvent(src,
                "Loop"  // ID=0: no Do → error
            );

            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // NextCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NextCmd_IncrementsForLoopVariable()
        {
            // ヘルプ: Next はインデックス変数を1増やして For に戻る
            var src = CreateSrc();

            var cmds = BuildEvent(src,
                "For i = 1 To 3",   // ID=0
                "Incr count",       // ID=1
                "Next"              // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("count"));
        }

        [TestMethod]
        public void NextCmd_ExitsWhenBeyondLimit()
        {
            // ヘルプ: インデックス変数が To の値を超えたらループ終了し Next の後へ進む
            var src = CreateSrc();

            var cmds = BuildEvent(src,
                "For i = 1 To 2",   // ID=0
                "Incr count",       // ID=1
                "Next",             // ID=2
                "Set after 1"       // ID=3
            );

            RunEvent(src, cmds);

            Assert.AreEqual(2d, src.Expression.GetValueAsDouble("count"));
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("after"));
        }

        [TestMethod]
        public void NextCmd_MissingFor_ReturnsError()
        {
            // 対応する For なしの Next はエラー
            var src = CreateSrc();

            var cmds = BuildEvent(src,
                "Next"  // ID=0: no For → error
            );

            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }
    }
}
