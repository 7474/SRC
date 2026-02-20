using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// For/Next, If/ElseIf/Else/EndIf, GoTo, Call/Return, Break, Skip, Local コマンドのユニットテスト
    /// ヘルプの記載に基づく期待値を検証する
    /// </summary>
    [TestClass]
    public class ControlCmdTests
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

        /// <summary>
        /// コマンドリストからSRCとコマンド配列を構築する
        /// </summary>
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
            // ラベルを登録（ラベル行に対してAddLabelを手動呼び出し）
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

        /// <summary>
        /// 単純なイベントループをシミュレートして変数の最終状態を得る
        /// </summary>
        private void RunEvent(SRC src, CmdData[] cmds, int startId = 0, int maxSteps = 1000)
        {
            var current = startId;
            for (var step = 0; step < maxSteps && current >= 0 && current < cmds.Length; step++)
            {
                current = cmds[current].Exec();
            }
        }

        // ──────────────────────────────────────────────
        // ForCmd / NextCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ForCmd_SimpleLoop_RunsCorrectTimes()
        {
            // ヘルプ: counterの値をinitialに設定し、counterの値がlastになるまでcommands を繰り返し実行
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("count", 0d);

            // For i = 1 To 3
            //   Incr count
            // Next
            var cmds = BuildEvent(src,
                "For i = 1 To 3",   // ID=0
                "Incr count",       // ID=1
                "Next"              // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("count"));
            Assert.AreEqual(4d, src.Expression.GetValueAsDouble("i")); // ループ終了後のcounterは最終値+step
        }

        [TestMethod]
        public void ForCmd_WithStep_RunsWithCustomStep()
        {
            // ヘルプ: counterの値はNextに達した時にstep増やされます
            var src = CreateSrc();

            // For i = 1 To 5 Step 2 → i = 1, 3, 5 (3回)
            var cmds = BuildEvent(src,
                "For i = 1 To 5 Step 2",    // ID=0
                "Incr count",               // ID=1
                "Next"                      // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("count"));
        }

        [TestMethod]
        public void ForCmd_InitialGtLast_NeverExecutes()
        {
            // ヘルプ: initialの値がlastの値より大きい場合にはcommandsは一度も実行されません
            var src = CreateSrc();

            // For i = 5 To 1 (Step省略→1なので条件を満たさない)
            var cmds = BuildEvent(src,
                "For i = 5 To 1",   // ID=0
                "Incr count",       // ID=1
                "Next"              // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("count"));
        }

        [TestMethod]
        public void ForCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "For i = 1",
                "Next"
            );
            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ForCmd_MissingNext_ReturnsError()
        {
            var src = CreateSrc();
            // Nextなしのループ
            var cmds = BuildEvent(src,
                "For i = 5 To 1"  // range never satisfied, but no Next
            );
            // initial > last なので最初からNextを探しに行くが、Nextがない
            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // IfCmd / ElseCmd / ElseIfCmd / EndIfCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IfCmd_TrueCondition_ExecutesThenBlock()
        {
            // ヘルプ: 書式3 - If condition Then → condition が成り立つとき commands を実行
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 5d);

            // If x > 3 Then → true
            //   Set result "yes"
            // EndIf
            var cmds = BuildEvent(src,
                "If (x > 3) Then",  // ID=0
                "Set result \"yes\"", // ID=1
                "EndIf"             // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual("yes", src.Expression.GetValueAsString("result"));
        }

        [TestMethod]
        public void IfCmd_FalseCondition_SkipsThenBlock()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 1d);

            var cmds = BuildEvent(src,
                "If (x > 3) Then",  // ID=0: false → skip to EndIf
                "Set result \"yes\"", // ID=1: skipped
                "EndIf"             // ID=2
            );

            RunEvent(src, cmds);

            Assert.IsFalse(src.Expression.IsVariableDefined("result"));
        }

        [TestMethod]
        public void IfCmd_WithElse_TrueExecutesThen()
        {
            // ヘルプ: 書式4 - 条件が成り立ったときIfの後のcommandsが、成り立たなかったときElseの後のcommandsが実行
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 5d);

            var cmds = BuildEvent(src,
                "If (x > 3) Then",      // ID=0: true
                "Set branch \"then\"",  // ID=1
                "Else",                 // ID=2
                "Set branch \"else\"",  // ID=3
                "EndIf"                 // ID=4
            );

            RunEvent(src, cmds);

            Assert.AreEqual("then", src.Expression.GetValueAsString("branch"));
        }

        [TestMethod]
        public void IfCmd_WithElse_FalseExecutesElse()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 1d);

            var cmds = BuildEvent(src,
                "If (x > 3) Then",      // ID=0: false
                "Set branch \"then\"",  // ID=1: skipped
                "Else",                 // ID=2
                "Set branch \"else\"",  // ID=3
                "EndIf"                 // ID=4
            );

            RunEvent(src, cmds);

            Assert.AreEqual("else", src.Expression.GetValueAsString("branch"));
        }

        [TestMethod]
        public void IfCmd_WithElseIf_MatchesElseIf()
        {
            // ヘルプ: 書式5 - ElseIf condition Then → ElseIf の条件式をチェック
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 2d);

            var cmds = BuildEvent(src,
                "If (x > 5) Then",          // ID=0: false
                "Set branch \"if\"",         // ID=1: skipped
                "ElseIf (x > 1) Then",      // ID=2: true
                "Set branch \"elseif\"",     // ID=3
                "Else",                      // ID=4
                "Set branch \"else\"",       // ID=5: skipped
                "EndIf"                      // ID=6
            );

            RunEvent(src, cmds);

            Assert.AreEqual("elseif", src.Expression.GetValueAsString("branch"));
        }

        [TestMethod]
        public void IfCmd_ExitForm_TrueReturnsNegOne()
        {
            // ヘルプ: 書式1 - If condition Exit → Exitを実行し、イベントを終了
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 5d);

            var cmds = BuildEvent(src,
                "If (x > 3) Exit"   // ID=0
            );

            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void IfCmd_ExitForm_FalseContinues()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 1d);

            var cmds = BuildEvent(src,
                "If (x > 3) Exit"   // ID=0
            );

            var result = cmds[0].Exec();
            Assert.AreEqual(1, result); // NextID = 1
        }

        [TestMethod]
        public void IfCmd_GotoForm_TrueJumpsToLabel()
        {
            // ヘルプ: 書式2 - If condition GoTo label → ラベルlabelにジャンプ
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 5d);

            var cmds = BuildEvent(src,
                "If (x > 3) GoTo target",   // ID=0
                "Set skipped 1",            // ID=1
                "target:"                   // ID=2 (label)
            );

            var result = cmds[0].Exec();
            // ラベルtargetはID=2、GoToはラベルの次の行ID(=3)を返す
            Assert.AreEqual(3, result);
        }

        // ──────────────────────────────────────────────
        // GotoCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GotoCmd_JumpsToLabel()
        {
            // ヘルプ: ラベルlabelにジャンプしてコマンドの実行の流れを変更します
            var src = CreateSrc();

            var cmds = BuildEvent(src,
                "GoTo target",      // ID=0
                "Set skipped 1",    // ID=1: skipped
                "target:",          // ID=2: label
                "Set reached 1"     // ID=3
            );

            // GoToはラベルID=2の次の実行行ID(=3)を返す
            var result = cmds[0].Exec();
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void GotoCmd_LabelNotFound_ReturnsError()
        {
            var src = CreateSrc();
            var cmds = BuildEvent(src, "GoTo nonexistent");
            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void GotoCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmds = BuildEvent(src, "GoTo");
            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // CallCmd / ReturnCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CallCmd_CallsSubroutineAndReturns()
        {
            // ヘルプ: ラベルlabelに対してサブルーチンコールを行います。
            // サブルーチンを実行後にCallコマンドの直後のイベントコマンドへ実行の流れが戻ります。
            var src = CreateSrc();

            var cmds = BuildEvent(src,
                "Call mysub",           // ID=0
                "Set afterCall 1",      // ID=1: ここに戻ってくる
                "GoTo done",            // ID=2
                "mysub:",               // ID=3: subroutine label
                "Set inSub 1",          // ID=4
                "Return",               // ID=5
                "done:"                 // ID=6
            );

            RunEvent(src, cmds);

            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("inSub"));
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("afterCall"));
        }

        [TestMethod]
        public void CallCmd_LabelNotFound_ReturnsError()
        {
            var src = CreateSrc();
            var cmds = BuildEvent(src, "Call nonexistent");
            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ReturnCmd_WithoutCall_ReturnsError()
        {
            // ヘルプ: CallコマンドとReturnコマンドが対応していなければなりません
            var src = CreateSrc();
            var cmds = BuildEvent(src, "Return");
            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // BreakCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BreakCmd_ExitsForLoop()
        {
            // ヘルプ: Breakコマンドが実行されると繰り返し実行を停止し、Next行の次の行へと実行の流れを移します
            var src = CreateSrc();

            // For i = 1 To 10
            //   If i = 3 Then
            //     Break
            //   EndIf
            //   Incr count
            // Next
            var cmds = BuildEvent(src,
                "For i = 1 To 10",          // ID=0
                "If (i = 3) Then",          // ID=1
                "Break",                    // ID=2
                "EndIf",                    // ID=3
                "Incr count",               // ID=4
                "Next"                      // ID=5
            );

            RunEvent(src, cmds);

            // i=1,2 はループを回る (count=2)、i=3でBreak
            Assert.AreEqual(2d, src.Expression.GetValueAsDouble("count"));
        }

        [TestMethod]
        public void BreakCmd_OutsideLoop_ReturnsError()
        {
            var src = CreateSrc();
            var cmds = BuildEvent(src, "Break");
            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SkipCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SkipCmd_JumpsToNextIteration()
        {
            // ヘルプ: ループの最終行であるNext行へと実行の流れが移り、
            // Skipコマンド以降のコマンドはその回のループではキャンセルされます
            var src = CreateSrc();

            // For i = 1 To 3
            //   If i = 2 Then
            //     Skip
            //   EndIf
            //   Incr count
            // Next
            var cmds = BuildEvent(src,
                "For i = 1 To 3",           // ID=0
                "If (i = 2) Then",          // ID=1
                "Skip",                     // ID=2
                "EndIf",                    // ID=3
                "Incr count",               // ID=4
                "Next"                      // ID=5
            );

            RunEvent(src, cmds);

            // i=1,3 はcountをインクリメント、i=2はSkipされる → count=2
            Assert.AreEqual(2d, src.Expression.GetValueAsDouble("count"));
        }

        [TestMethod]
        public void SkipCmd_OutsideLoop_ReturnsError()
        {
            var src = CreateSrc();
            var cmds = BuildEvent(src, "Skip");
            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // LocalCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LocalCmd_CreatesSubroutineLocalVariable()
        {
            // ヘルプ: 特定のサブルーチンでのみ有効なサブルーチンローカル変数を作成します
            // サブルーチン内でLocalコマンドを使う
            var src = CreateSrc();

            var cmds = BuildEvent(src,
                "Call mysub",       // ID=0
                "GoTo done",        // ID=1
                "mysub:",           // ID=2
                "Local tmpVar",     // ID=3
                "Set tmpVar 42",    // ID=4
                "Return",           // ID=5
                "done:"             // ID=6
            );

            RunEvent(src, cmds);

            // サブルーチンローカル変数はサブルーチン外では参照できない
            // (0 as undefined)
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("tmpVar", true));
        }

        [TestMethod]
        public void LocalCmd_WithInitialValue_SetsValue()
        {
            // ヘルプ: 書式2 - Local var = expr で初期値を指定できる
            var src = CreateSrc();

            var cmds = BuildEvent(src,
                "Call mysub",           // ID=0
                "GoTo done",            // ID=1
                "mysub:",               // ID=2
                "Local result = 100",   // ID=3
                "Set outerVal result",  // ID=4: ローカル変数の値を外部変数にコピー
                "Return",               // ID=5
                "done:"                 // ID=6
            );

            RunEvent(src, cmds);

            Assert.AreEqual(100d, src.Expression.GetValueAsDouble("outerVal"));
        }
    }
}
