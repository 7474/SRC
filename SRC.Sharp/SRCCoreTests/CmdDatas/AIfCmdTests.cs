using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// AIfCmd（IfCmd / ElseIfCmd の抽象基底クラス）に対するユニットテスト
    /// ヘルプ: Ifコマンド.md の記載に基づく
    ///
    /// 主にテストする機能:
    /// 1. PrepareArgs() のエラーケース（引数不足、Then/Exit/GoTo なし）
    /// 2. Evaluate() のパイロット名条件判定（ExprTermCount=1/2）
    ///    - ヘルプ: 「パイロット名 — パイロットが乗っているユニットが出撃していれば成り立ちます」
    ///    - ヘルプ: 「先頭に Not が付けられている場合は条件式の結果は逆になります」
    /// </summary>
    [TestClass]
    public class AIfCmdTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
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

        private void RunEvent(SRC src, CmdData[] cmds, int startId = 0, int maxSteps = 200)
        {
            var current = startId;
            for (var step = 0; step < maxSteps && current >= 0 && current < cmds.Length; step++)
            {
                current = cmds[current].Exec();
            }
        }

        /// <summary>
        /// パイロットを PDList と PList に登録して返すヘルパー
        /// </summary>
        private Pilots.Pilot CreatePilot(SRC src, string name, int level = 1)
        {
            src.PDList.Add(name);
            return src.PList.Add(name, level, "味方");
        }

        // ──────────────────────────────────────────────
        // PrepareArgs() — エラーケース（パース時エラー）
        // 注: IfCmd のコンストラクタで PrepareArgs が呼ばれるため、エラーはパース時に発生する
        // CmdParser.Parse() がこの例外を catch し NopCmd を返すため、Exec() は NextID を返す
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IfCmd_OnlyIfKeyword_ParseFallsBackToNopCmd()
        {
            // "If" のみ (ArgNum=1) → PrepareArgs がコンストラクタ内で例外を投げる
            // → CmdParser が NopCmd を返す → Exec() は NextID = 1 を返す
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "If"    // ID=0: ArgNum=1 → 書式エラー → NopCmd
            );

            // NopCmd は NextID を返す (= 1)
            var result = cmds[0].Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void IfCmd_NoThenExitGoto_ParseFallsBackToNopCmd()
        {
            // Then/Exit/GoTo のいずれもない → PrepareArgs がコンストラクタ内で例外を投げる
            // → CmdParser が NopCmd を返す → Exec() は NextID = 1 を返す
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "If x y z"  // ID=0: Then/Exit/GoTo なし → NopCmd
            );

            var result = cmds[0].Exec();
            Assert.AreEqual(1, result);
        }

        // ──────────────────────────────────────────────
        // AIfCmd.Evaluate() — パイロット名条件 (ExprTermCount = 1)
        // ヘルプ: 「パイロット名 — パイロットが乗っているユニットが出撃していれば成り立ちます」
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IfCmd_PilotCondition_PilotDefinedNoUnit_EvaluatesFalse()
        {
            // ヘルプ: パイロットが乗っているユニットが出撃していれば成り立つ
            // → パイロットが存在するがユニットに乗っていない場合は成り立たない (false)
            var src = CreateSrc();
            CreatePilot(src, "テストパイロット"); // Unit は null のまま

            // If テストパイロット Then
            //   Set result "taken"
            // EndIf
            var cmds = BuildEvent(src,
                "If テストパイロット Then",  // ID=0: pilot defined, no unit → false
                "Set result \"taken\"",     // ID=1: スキップされるはず
                "EndIf"                      // ID=2
            );

            RunEvent(src, cmds);

            // 条件が false → Then ブロックをスキップ → result は未定義
            Assert.IsFalse(src.Expression.IsVariableDefined("result"));
        }

        [TestMethod]
        public void IfCmd_PilotCondition_PilotNotDefined_ZeroVariable_EvaluatesFalse()
        {
            // ヘルプ: パイロット名として定義されていない場合、式として評価される
            // 変数 "noSuchPilot" が 0 なら false
            var src = CreateSrc();
            // パイロット未登録、変数 noSuchPilot = 0 (デフォルト)

            var cmds = BuildEvent(src,
                "If noSuchPilot Then",  // ID=0: not in PList, variable = 0 → false
                "Set result \"taken\"", // ID=1: スキップ
                "EndIf"                 // ID=2
            );

            RunEvent(src, cmds);

            Assert.IsFalse(src.Expression.IsVariableDefined("result"));
        }

        [TestMethod]
        public void IfCmd_PilotCondition_PilotNotDefined_NonZeroVariable_EvaluatesTrue()
        {
            // ヘルプ: パイロット名として定義されていない場合、式として評価される
            // 変数 "counter" が 5 (≠ 0) なら true
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("counter", 5d);

            var cmds = BuildEvent(src,
                "If counter Then",      // ID=0: not in PList, variable = 5 → true
                "Set result \"taken\"", // ID=1: 実行される
                "EndIf"                 // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual("taken", src.Expression.GetValueAsString("result"));
        }

        // ──────────────────────────────────────────────
        // AIfCmd.Evaluate() — Not + パイロット名条件 (ExprTermCount = 2)
        // ヘルプ: 「先頭に Not が付けられている場合は条件式の結果は逆になります」
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IfCmd_NotPilotCondition_PilotDefinedNoUnit_EvaluatesTrue()
        {
            // ヘルプ: Not パイロット名 → パイロットが乗っていなければ成り立つ
            // パイロット定義あり、ユニットなし → Not で逆転 → true
            var src = CreateSrc();
            CreatePilot(src, "ネリィ"); // Unit は null

            var cmds = BuildEvent(src,
                "If Not ネリィ Then",    // ID=0: pilot defined, no unit → Not → true
                "Set result \"taken\"",  // ID=1: 実行されるはず
                "EndIf"                  // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual("taken", src.Expression.GetValueAsString("result"));
        }

        [TestMethod]
        public void IfCmd_NotPilotCondition_PilotNotDefined_ZeroVariable_EvaluatesTrue()
        {
            // ヘルプ: Not 変数 → 変数が 0 なら Not で逆転 → true
            var src = CreateSrc();
            // noSuchPilot: not in PList, variable = 0

            var cmds = BuildEvent(src,
                "If Not noSuchPilot Then",  // ID=0: not in PList, variable=0 → Not → true
                "Set result \"taken\"",     // ID=1: 実行される
                "EndIf"                     // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual("taken", src.Expression.GetValueAsString("result"));
        }

        [TestMethod]
        public void IfCmd_NotPilotCondition_PilotNotDefined_NonZeroVariable_EvaluatesFalse()
        {
            // ヘルプ: Not 変数 → 変数が非 0 なら Not で逆転 → false
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("hp", 100d);

            var cmds = BuildEvent(src,
                "If Not hp Then",       // ID=0: not in PList, variable=100 → Not → false
                "Set result \"taken\"", // ID=1: スキップ
                "EndIf"                 // ID=2
            );

            RunEvent(src, cmds);

            Assert.IsFalse(src.Expression.IsVariableDefined("result"));
        }

        // ──────────────────────────────────────────────
        // 回帰テスト: 式条件が引き続き正常に動作すること
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IfCmd_ExpressionCondition_RegressionTest_StillWorks()
        {
            // AIfCmd の ExprTermCount 修正後も式条件が正しく動作すること
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 10d);

            var cmds = BuildEvent(src,
                "If (x > 5) Then",      // ID=0: expression → ExprTermCount=0 → true
                "Set result \"ok\"",    // ID=1
                "EndIf"                 // ID=2
            );

            RunEvent(src, cmds);

            Assert.AreEqual("ok", src.Expression.GetValueAsString("result"));
        }

        [TestMethod]
        public void IfCmd_ExpressionFalseCondition_RegressionTest_SkipsBlock()
        {
            // 式条件が false の場合、ブロックをスキップすること（回帰）
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 1d);

            var cmds = BuildEvent(src,
                "If (x > 5) Then",      // ID=0: false
                "Set result \"ok\"",    // ID=1: スキップ
                "EndIf"                 // ID=2
            );

            RunEvent(src, cmds);

            Assert.IsFalse(src.Expression.IsVariableDefined("result"));
        }

        // ──────────────────────────────────────────────
        // AIfCmd.ToEnd() — EndIf が見つからない場合のエラー
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IfCmd_MissingEndIf_ReturnsError()
        {
            // ヘルプ: If ... Then に対応する EndIf がない場合はエラー
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("x", 0d); // false → EndIf を探しに行く

            var cmds = BuildEvent(src,
                "If (x > 5) Then",  // ID=0: false → EndIf を探す
                "Set result 1"      // ID=1: EndIf なし → エラー
            );

            var result = cmds[0].Exec();

            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ExprTermCount プロパティの確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IfCmd_PilotSingleTerm_ExprTermCountIsOne()
        {
            // 単一パイロット名 "テスト" → ExprTermCount が 1 になること
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "If テスト Then",   // ID=0: single non-$ non-( term
                "EndIf"             // ID=1
            );

            var ifCmd = cmds[0] as AIfCmd;
            Assert.IsNotNull(ifCmd);
            Assert.AreEqual(1, ifCmd.ExprTermCount);
        }

        [TestMethod]
        public void IfCmd_NotPilotTwoTerms_ExprTermCountIsTwo()
        {
            // Not + パイロット名 "Not テスト" → ExprTermCount が 2 になること
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "If Not テスト Then",   // ID=0: two terms, first is "Not"
                "EndIf"                 // ID=1
            );

            var ifCmd = cmds[0] as AIfCmd;
            Assert.IsNotNull(ifCmd);
            Assert.AreEqual(2, ifCmd.ExprTermCount);
        }

        [TestMethod]
        public void IfCmd_ExpressionInParens_ExprTermCountIsZero()
        {
            // 括弧付き式 → ExprTermCount が 0 になること
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "If (x > 0) Then",  // ID=0: parenthesized expression → 0
                "EndIf"             // ID=1
            );

            var ifCmd = cmds[0] as AIfCmd;
            Assert.IsNotNull(ifCmd);
            Assert.AreEqual(0, ifCmd.ExprTermCount);
        }

        [TestMethod]
        public void IfCmd_VariableExpression_ExprTermCountIsZero()
        {
            // $変数 形式 → ExprTermCount が 0 になること
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "If $myvar Then",   // ID=0: starts with $ → expression → 0
                "EndIf"             // ID=1
            );

            var ifCmd = cmds[0] as AIfCmd;
            Assert.IsNotNull(ifCmd);
            Assert.AreEqual(0, ifCmd.ExprTermCount);
        }

        [TestMethod]
        public void IfCmd_MultiTermExpression_ExprTermCountIsZero()
        {
            // 複数項式 "x > 5" (3 terms) → ExprTermCount が 0 になること
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "If x > 5 Then",    // ID=0: 3 terms → default → ExprTermCount=0
                "EndIf"             // ID=1
            );

            var ifCmd = cmds[0] as AIfCmd;
            Assert.IsNotNull(ifCmd);
            Assert.AreEqual(0, ifCmd.ExprTermCount);
        }
    }
}
