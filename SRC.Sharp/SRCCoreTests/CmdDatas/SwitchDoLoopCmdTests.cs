using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// Switch/Case/EndSw, Do/Loop コマンドのユニットテスト
    /// ヘルプの記載に基づく期待値を検証する
    /// </summary>
    [TestClass]
    public class SwitchDoLoopCmdTests
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
        // SwitchCmd / CaseCmd / CaseElseCmd / EndSwCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SwitchCmd_MatchesFirstCase_ExecutesThatCase()
        {
            // ヘルプ: 条件式expressionの値がvalueの値に等しいCase文以降のcommandsを実行します
            var src = CreateSrc();
            src.Expression.SetVariableAsString("emotion", "喜");

            // Switch emotion
            // Case 同 友
            //   Set result "いい心がけ"
            // Case 喜
            //   Set result "余裕"
            // EndSw
            var cmds = BuildEvent(src,
                "Switch emotion",       // ID=0
                "Case 同 友",           // ID=1
                "Set result \"いい心がけ\"", // ID=2
                "Case 喜",              // ID=3
                "Set result \"余裕\"",  // ID=4
                "EndSw"                 // ID=5
            );

            RunEvent(src, cmds);

            Assert.AreEqual("余裕", src.Expression.GetValueAsString("result"));
        }

        [TestMethod]
        public void SwitchCmd_MultipleCaseValues_MatchesAny()
        {
            // ヘルプ: 一つのCase文に複数のvalueを設定することもできます
            var src = CreateSrc();
            src.Expression.SetVariableAsString("emotion", "友");

            var cmds = BuildEvent(src,
                "Switch emotion",           // ID=0
                "Case 同 友",               // ID=1: matches "友"
                "Set result \"いい心がけ\"", // ID=2
                "Case 喜",                  // ID=3
                "Set result \"余裕\"",      // ID=4
                "EndSw"                     // ID=5
            );

            RunEvent(src, cmds);

            Assert.AreEqual("いい心がけ", src.Expression.GetValueAsString("result"));
        }

        [TestMethod]
        public void SwitchCmd_CaseElse_ExecutedWhenNoMatch()
        {
            // ヘルプ: valueにElseを指定すると、その Case 文は expression の値にかかわらず実行される
            var src = CreateSrc();
            src.Expression.SetVariableAsString("emotion", "悩");

            var cmds = BuildEvent(src,
                "Switch emotion",           // ID=0
                "Case 同 友",               // ID=1
                "Set result \"いい心がけ\"", // ID=2
                "Case Else",                // ID=3
                "Set result \"どうしたの？\"", // ID=4
                "EndSw"                     // ID=5
            );

            RunEvent(src, cmds);

            Assert.AreEqual("どうしたの？", src.Expression.GetValueAsString("result"));
        }

        [TestMethod]
        public void SwitchCmd_NoMatchNoCaseElse_SkipsToEndSw()
        {
            // ヘルプ: どのCase文にも一致しない場合はEndSwの後へ
            var src = CreateSrc();
            src.Expression.SetVariableAsString("val", "X");

            var cmds = BuildEvent(src,
                "Switch val",           // ID=0
                "Case A",               // ID=1
                "Set result \"A\"",     // ID=2
                "Case B",               // ID=3
                "Set result \"B\"",     // ID=4
                "EndSw",                // ID=5
                "Set afterSw 1"         // ID=6
            );

            RunEvent(src, cmds);

            Assert.IsFalse(src.Expression.IsVariableDefined("result"));
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("afterSw"));
        }

        [TestMethod]
        public void SwitchCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "Switch",   // ID=0: ArgNum=1, 不正
                "EndSw"     // ID=1
            );

            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void SwitchCmd_MissingEndSw_ReturnsError()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsString("val", "X");

            // EndSwなし
            var cmds = BuildEvent(src,
                "Switch val",   // ID=0
                "Case A"        // ID=1
            );

            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void SwitchCmd_FirstMatchTakesPriority()
        {
            // ヘルプ: expressionの値に該当するCase文の検索は上から順に行われます
            var src = CreateSrc();
            src.Expression.SetVariableAsString("val", "1");

            var cmds = BuildEvent(src,
                "Switch val",           // ID=0
                "Case 1",               // ID=1: 最初のCase、"1"にマッチ
                "Set result \"first\"", // ID=2
                "Case 1",               // ID=3: 2番目のCase（到達しない）
                "Set result \"second\"",// ID=4
                "EndSw"                 // ID=5
            );

            RunEvent(src, cmds);

            Assert.AreEqual("first", src.Expression.GetValueAsString("result"));
        }

        // ──────────────────────────────────────────────
        // DoCmd / LoopCmd (Do While / Loop)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DoWhileCmd_TrueCondition_ExecutesLoop()
        {
            // ヘルプ: 条件式expressionの値が0でない間、Do行からLoop行までのコマンドを繰り返し実行します
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("i", 0d);

            // Do While (i < 3)
            //   Incr i
            // Loop
            var cmds = BuildEvent(src,
                "Do While (i < 3)", // ID=0
                "Incr i",           // ID=1
                "Loop"              // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("i"));
        }

        [TestMethod]
        public void DoWhileCmd_FalseCondition_SkipsLoop()
        {
            // ヘルプ: 条件式がFalseの場合はループ本体を実行しない（書式1）
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("i", 10d);

            var cmds = BuildEvent(src,
                "Do While (i < 3)", // ID=0: false → skip
                "Incr count",       // ID=1: skipped
                "Loop"              // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("count"));
        }

        [TestMethod]
        public void DoLoopWhileCmd_ExecutesAtLeastOnce()
        {
            // ヘルプ: 書式2 - Do / Loop While expression は条件に関わらず1回は実行される
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("i", 10d); // 条件は最初からfalse

            // Do
            //   Incr count
            // Loop While (i < 3)
            var cmds = BuildEvent(src,
                "Do",                   // ID=0
                "Incr count",           // ID=1
                "Loop While (i < 3)"    // ID=2: false → exit after first iteration
            );

            RunEvent(src, cmds);

            // 1回だけ実行される
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("count"));
        }

        [TestMethod]
        public void DoLoopWhileCmd_LoopsWhileConditionTrue()
        {
            // ヘルプ: Loop Whileで条件式が0でない間繰り返す
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("i", 0d);

            var cmds = BuildEvent(src,
                "Do",               // ID=0
                "Incr i",           // ID=1
                "Loop While (i < 3)" // ID=2: continues until i >= 3
            );

            RunEvent(src, cmds);

            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("i"));
        }

        [TestMethod]
        public void DoCmd_WrongArgCount_ReturnsError()
        {
            // 書式以外の引数はエラー
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "Do InvalidKeyword 1",  // ID=0: 不正
                "Loop"                  // ID=1
            );

            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void DoWhileCmd_MissingLoop_ReturnsError()
        {
            // Loopなしのループはエラー
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 10d);

            var cmds = BuildEvent(src,
                "Do While (x < 3)"  // ID=0: false, looks for Loop but none
            );

            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void DoWhileCmd_BreakExitsLoop()
        {
            // ヘルプ: SkipコマンドやBreakコマンドを使えば実行の流れを変えることができます
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("i", 0d);

            // Do While (i < 10)
            //   Incr i
            //   If (i = 3) Then
            //     Break
            //   EndIf
            // Loop
            var cmds = BuildEvent(src,
                "Do While (i < 10)",    // ID=0
                "Incr i",               // ID=1
                "If (i = 3) Then",      // ID=2
                "Break",                // ID=3
                "EndIf",                // ID=4
                "Loop"                  // ID=5
            );

            RunEvent(src, cmds);

            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("i"));
        }
    }
}
