using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// 制御フローコマンドの追加ユニットテスト（ControlCmdTests.cs・SwitchDoLoopCmdTests.cs に未収録のケース）
    /// </summary>
    [TestClass]
    public class ControlCmdMoreTests
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
            // ラベルを登録
            for (var i = 0; i < lines.Length; i++)
            {
                var trimmed = lines[i].Trim();
                if (trimmed.EndsWith(":"))
                {
                    var labelName = trimmed.Substring(0, trimmed.Length - 1).Trim();
                    src.Event.AddLabel(labelName, i);
                }
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
        // Do/Loop Until パターン
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DoUntilCmd_FalseCondition_ExecutesLoop()
        {
            // Do Until condition → condition が 0（偽）の間ループを実行する
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("i", 0d);

            // Do Until (i >= 3)
            //   Incr i
            // Loop
            var cmds = BuildEvent(src,
                "Do Until (i >= 3)",    // ID=0: condition=false → enter loop
                "Incr i",               // ID=1
                "Loop"                  // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("i"));
        }

        [TestMethod]
        public void DoUntilCmd_TrueCondition_SkipsLoop()
        {
            // Do Until condition → condition が 0 でない（真）ならループ本体をスキップする
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("i", 10d); // i >= 3 は真

            var cmds = BuildEvent(src,
                "Do Until (i >= 3)",    // ID=0: condition=true → skip loop
                "Incr count",           // ID=1: skipped
                "Loop"                  // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("count"));
        }

        [TestMethod]
        public void DoLoopUntilCmd_ExecutesAtLeastOnce()
        {
            // Do / Loop Until expression → 条件に関わらず1回は実行される
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("i", 10d); // 最初から条件が真

            // Do
            //   Incr count
            // Loop Until (i >= 3)  ← 真なので1回で終了
            var cmds = BuildEvent(src,
                "Do",                       // ID=0
                "Incr count",               // ID=1
                "Loop Until (i >= 3)"       // ID=2: condition=true → exit after first run
            );

            RunEvent(src, cmds);

            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("count"));
        }

        [TestMethod]
        public void DoLoopUntilCmd_LoopsUntilConditionTrue()
        {
            // Do / Loop Until (i >= 3) → i が 3 以上になるまで繰り返す
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("i", 0d);

            var cmds = BuildEvent(src,
                "Do",                       // ID=0
                "Incr i",                   // ID=1
                "Loop Until (i >= 3)"       // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("i"));
        }

        // ──────────────────────────────────────────────
        // Switch/Case/Case Else
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SwitchCmd_NumericMatch_ExecutesCorrectCase()
        {
            // 数値による Switch/Case マッチング
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("code", 2d);

            var cmds = BuildEvent(src,
                "Switch code",          // ID=0
                "Case 1",               // ID=1
                "Set result \"one\"",   // ID=2
                "Case 2",               // ID=3
                "Set result \"two\"",   // ID=4
                "Case 3",               // ID=5
                "Set result \"three\"", // ID=6
                "EndSw"                 // ID=7
            );

            RunEvent(src, cmds);

            Assert.AreEqual("two", src.Expression.GetValueAsString("result"));
        }

        [TestMethod]
        public void SwitchCmd_CaseElse_FallbackWhenNoMatch()
        {
            // どの Case にも一致しないとき Case Else が実行される
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 99d);

            var cmds = BuildEvent(src,
                "Switch x",                     // ID=0
                "Case 1",                        // ID=1
                "Set result \"one\"",            // ID=2
                "Case 2",                        // ID=3
                "Set result \"two\"",            // ID=4
                "Case Else",                     // ID=5
                "Set result \"other\"",          // ID=6
                "EndSw"                          // ID=7
            );

            RunEvent(src, cmds);

            Assert.AreEqual("other", src.Expression.GetValueAsString("result"));
        }

        [TestMethod]
        public void SwitchCmd_CaseElse_NotExecutedWhenMatched()
        {
            // Case にマッチした場合 Case Else は実行されない
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 1d);

            var cmds = BuildEvent(src,
                "Switch x",                     // ID=0
                "Case 1",                        // ID=1
                "Set result \"matched\"",        // ID=2
                "Case Else",                     // ID=3
                "Set result \"fallback\"",       // ID=4
                "EndSw"                          // ID=5
            );

            RunEvent(src, cmds);

            Assert.AreEqual("matched", src.Expression.GetValueAsString("result"));
        }

        // ──────────────────────────────────────────────
        // 入れ子 If/ElseIf/Else
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NestedIf_OuterTrueInnerTrue_ExecutesInnerThen()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("a", 5d);
            src.Expression.SetVariableAsDouble("b", 3d);

            // If (a > 3) Then
            //   If (b > 1) Then
            //     Set result "inner"
            //   EndIf
            // EndIf
            var cmds = BuildEvent(src,
                "If (a > 3) Then",          // ID=0: true
                "If (b > 1) Then",          // ID=1: true
                "Set result \"inner\"",     // ID=2
                "EndIf",                    // ID=3
                "EndIf"                     // ID=4
            );

            RunEvent(src, cmds);

            Assert.AreEqual("inner", src.Expression.GetValueAsString("result"));
        }

        [TestMethod]
        public void NestedIf_OuterTrueInnerFalse_SkipsInnerBody()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("a", 5d);
            src.Expression.SetVariableAsDouble("b", 0d); // inner condition false

            var cmds = BuildEvent(src,
                "If (a > 3) Then",          // ID=0: true
                "If (b > 1) Then",          // ID=1: false → skip
                "Set inner 1",              // ID=2: skipped
                "EndIf",                    // ID=3
                "Set outer 1",              // ID=4: executed
                "EndIf"                     // ID=5
            );

            RunEvent(src, cmds);

            Assert.IsFalse(src.Expression.IsVariableDefined("inner"));
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("outer"));
        }

        [TestMethod]
        public void NestedIf_OuterFalse_SkipsEntireBlock()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("a", 1d); // outer false

            var cmds = BuildEvent(src,
                "If (a > 3) Then",          // ID=0: false → skip
                "If (a > 0) Then",          // ID=1: skipped
                "Set inner 1",              // ID=2: skipped
                "EndIf",                    // ID=3: skipped
                "EndIf"                     // ID=4
            );

            RunEvent(src, cmds);

            Assert.IsFalse(src.Expression.IsVariableDefined("inner"));
        }

        [TestMethod]
        public void NestedIf_ElseInOuter_ExecutedWhenOuterFalse()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 1d); // outer false

            // If (x > 5) Then
            //   Set branch "then"
            // Else
            //   If (x > 0) Then
            //     Set branch "else-inner"
            //   EndIf
            // EndIf
            var cmds = BuildEvent(src,
                "If (x > 5) Then",              // ID=0: false
                "Set branch \"then\"",          // ID=1: skipped
                "Else",                          // ID=2
                "If (x > 0) Then",              // ID=3: true
                "Set branch \"else-inner\"",    // ID=4
                "EndIf",                         // ID=5
                "EndIf"                          // ID=6
            );

            RunEvent(src, cmds);

            Assert.AreEqual("else-inner", src.Expression.GetValueAsString("branch"));
        }

        // ──────────────────────────────────────────────
        // For ループ - 負のステップ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ForCmd_NegativeStep_CountsDown()
        {
            // For i = 5 To 1 Step -1 → i = 5, 4, 3, 2, 1 （5回）
            var src = CreateSrc();

            var cmds = BuildEvent(src,
                "For i = 5 To 1 Step -1",   // ID=0
                "Incr count",               // ID=1
                "Next"                      // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual(5d, src.Expression.GetValueAsDouble("count"));
        }

        [TestMethod]
        public void ForCmd_NegativeStep_VariableDecrements()
        {
            // For i = 3 To 1 Step -1 → i が 3→2→1 と減少する
            var src = CreateSrc();

            var cmds = BuildEvent(src,
                "For i = 3 To 1 Step -1",   // ID=0
                "Incr count",               // ID=1
                "Next"                      // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("count"));
            // ループ終了後 i は last - step = 1 - (-1) = 0 相当に進む
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("i"));
        }

        [TestMethod]
        public void ForCmd_NegativeStep_InitialLessThanLast_NeverExecutes()
        {
            // For i = 1 To 5 Step -1 → 開始値 < 終了値 かつ step < 0 → 0回
            var src = CreateSrc();

            var cmds = BuildEvent(src,
                "For i = 1 To 5 Step -1",   // ID=0: 条件不成立
                "Incr count",               // ID=1: never
                "Next"                      // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("count"));
        }

        [TestMethod]
        public void ForCmd_NegativeStep_WithBreak_ExitsEarly()
        {
            // For i = 10 To 1 Step -2 → 10, 8, 6 で Break
            var src = CreateSrc();

            var cmds = BuildEvent(src,
                "For i = 10 To 1 Step -2",  // ID=0
                "If (i = 6) Then",          // ID=1
                "Break",                    // ID=2
                "EndIf",                    // ID=3
                "Incr count",               // ID=4
                "Next"                      // ID=5
            );

            RunEvent(src, cmds);

            // i=10,8 でカウント増加（count=2）、i=6でBreak
            Assert.AreEqual(2d, src.Expression.GetValueAsDouble("count"));
        }
    }
}
