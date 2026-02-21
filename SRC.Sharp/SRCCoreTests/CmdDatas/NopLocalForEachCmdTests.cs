using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// NopCmd, LocalCmd, ForEachCmd の基本動作テスト
    /// </summary>
    [TestClass]
    public class NopLocalForEachCmdTests
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
        // NopCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NopCmd_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmds = BuildEvent(src, "");
            var result = cmds[0].Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void NopCmd_EmptyLine_IsNopCmd()
        {
            var src = CreateSrc();
            var cmds = BuildEvent(src, "");
            Assert.IsInstanceOfType(cmds[0], typeof(NopCmd));
        }

        [TestMethod]
        public void NopCmd_LabelLine_IsNopCmd()
        {
            var src = CreateSrc();
            var cmds = BuildEvent(src, "myLabel:");
            Assert.IsInstanceOfType(cmds[0], typeof(NopCmd));
        }

        [TestMethod]
        public void NopCmd_MultipleNops_ExecuteSequentially()
        {
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "",          // ID=0
                "",          // ID=1
                "Set x 5"   // ID=2
            );
            RunEvent(src, cmds);
            Assert.AreEqual(5d, src.Expression.GetValueAsDouble("x"));
        }

        // ──────────────────────────────────────────────
        // LocalCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LocalCmd_DefinesLocalVariable()
        {
            // ヘルプ: ローカル変数variableを作成します（SubLocalVar として作成）
            var src = CreateSrc();
            var cmds = BuildEvent(src, "Local myLocal");
            var result = cmds[0].Exec();
            Assert.AreEqual(1, result);
            Assert.IsTrue(src.Expression.IsSubLocalVariableDefined("myLocal"));
        }

        [TestMethod]
        public void LocalCmd_MultipleLocalVariables_AllDefined()
        {
            var src = CreateSrc();
            var cmds = BuildEvent(src, "Local a b c");
            cmds[0].Exec();
            Assert.IsTrue(src.Expression.IsSubLocalVariableDefined("a"));
            Assert.IsTrue(src.Expression.IsSubLocalVariableDefined("b"));
            Assert.IsTrue(src.Expression.IsSubLocalVariableDefined("c"));
        }

        [TestMethod]
        public void LocalCmd_NewVariable_InitiallyZero()
        {
            // ヘルプ: 作成時の変数の値は空文字列（数値として扱う場合は0）
            var src = CreateSrc();
            var cmds = BuildEvent(src, "Local counter");
            cmds[0].Exec();
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("counter"));
        }

        [TestMethod]
        public void LocalCmd_ThenSetValue_ValuePersists()
        {
            // LocalCmd は Call/Return ブロック内（CallDepth > 0）でのみ正しく動作する。
            // Call 内で Local 変数に値を設定し、別の変数経由で確認する。
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "Call mysub",          // ID=0
                "GoTo done",           // ID=1
                "mysub:",              // ID=2
                "Local x",             // ID=3
                "Set x 42",            // ID=4: SubLocalVar x に代入
                "Set outerVal x",      // ID=5: x の値をグローバル変数にコピー
                "Return",              // ID=6
                "done:"                // ID=7
            );
            RunEvent(src, cmds);
            // Call 外から outerVal を確認する
            Assert.AreEqual(42d, src.Expression.GetValueAsDouble("outerVal"));
        }

        // ──────────────────────────────────────────────
        // ForEachCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ForEachCmd_WithList_IteratesAllElements()
        {
            // ヘルプ: ForEach element In list → list 内の各要素について繰り返す (4引数形式)
            // リスト変数（スペース区切り）に対するForEach
            var src = CreateSrc();
            src.Expression.SetVariableAsString("myList", "アリス ボブ キャロル");
            src.Expression.DefineGlobalVariable("myList");
            src.Expression.SetVariableAsDouble("count", 0d);

            var cmds = BuildEvent(src,
                "ForEach item In myList", // ID=0 (4引数: ForEach item In myList)
                "Incr count",             // ID=1
                "Next"                    // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("count"));
        }

        [TestMethod]
        public void ForEachCmd_WithEmptyList_SkipsBody()
        {
            var src = CreateSrc();
            // 空文字列のリスト変数を定義
            src.Expression.DefineGlobalVariable("emptyList");
            src.Expression.SetVariableAsString("emptyList", "");
            src.Expression.SetVariableAsDouble("count", 0d);

            var cmds = BuildEvent(src,
                "ForEach item In emptyList", // ID=0
                "Incr count",               // ID=1
                "Next"                      // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("count"));
        }

        [TestMethod]
        public void ForEachCmd_SetsElementVariable()
        {
            var src = CreateSrc();
            // スペース区切りリストを使用
            src.Expression.DefineGlobalVariable("myList");
            src.Expression.SetVariableAsString("myList", "first second");

            var cmds = BuildEvent(src,
                "ForEach item In myList", // ID=0
                "Set captured item",      // ID=1: item に現在の要素が設定されている
                "Next"                    // ID=2
            );

            RunEvent(src, cmds);

            // After iteration, the last value set in "captured" should be "second"
            // item は ForEach が毎回更新するスペース区切りリストの要素
            var captured = src.Expression.GetValueAsString("captured");
            Assert.AreEqual("second", captured);
        }

        // ──────────────────────────────────────────────
        // BreakCmd と ContinueCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BreakCmd_InForLoop_ExitsLoop()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("count", 0d);

            var cmds = BuildEvent(src,
                "For i = 1 To 10",  // ID=0
                "Incr count",       // ID=1
                "If (i >= 3) Then", // ID=2 - when i=3, break
                "Break",            // ID=3
                "EndIf",            // ID=4
                "Next"              // ID=5
            );

            RunEvent(src, cmds);

            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("count"));
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
        // GotoCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GotoCmd_JumpsToLabel()
        {
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "Goto skipSet",    // ID=0
                "Set x 999",       // ID=1 - should be skipped
                "skipSet:",        // ID=2
                "Set x 1"          // ID=3
            );
            // Register label
            src.Event.AddLabel("skipSet", 2);
            RunEvent(src, cmds);
            // x should be 1 (from ID=3), not 999 (from ID=1)
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("x"));
        }

        [TestMethod]
        public void GotoCmd_UnknownLabel_ReturnsError()
        {
            var src = CreateSrc();
            var cmds = BuildEvent(src, "Goto nonExistentLabel");
            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SkipCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SkipCmd_ReturnsNegativeOne()
        {
            // ヘルプ: イベントを終了します
            var src = CreateSrc();
            var cmds = BuildEvent(src, "Skip");
            var result = cmds[0].Exec();
            Assert.AreEqual(-1, result);
        }
    }
}
