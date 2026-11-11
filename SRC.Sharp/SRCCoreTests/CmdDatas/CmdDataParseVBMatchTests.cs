using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// VB 版 CmdData.Parse・Exec（SRC/SRC_20121125/CmdData.cls）の動作に合わせたテスト（#1503）
    /// </summary>
    [TestClass]
    public class CmdDataParseVBMatchTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            return src;
        }

        private CmdData Parse(SRC src, string cmdText)
        {
            var id = src.Event.EventData.Count;
            var line = new EventDataLine(id, EventDataSource.Scenario, "test.eve", id + 10, cmdText);
            src.Event.EventData.Add(line);
            var cmd = new CmdParser().Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        private static string[] ErrorMessages(SRC src)
        {
            return src.DataErrors.Select(x => x.msg).ToArray();
        }

        // ──────────────────────────────────────────────
        // 代入式（Case Else の「list(2) = "="」）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Parse_Assignment_SingleTerm_KeepsTermAsIs()
        {
            // VB: 値が項（ArgNum = 3）なら引数の処理をそのまま使う。「-」は数値ではないので文字列
            var src = CreateSrc();
            var cmd = Parse(src, "a = -");
            Assert.IsInstanceOfType(cmd, typeof(SetCmd));
            Assert.AreEqual(3, cmd.ArgNum);
            Assert.AreEqual(Expressions.ValueType.StringType, cmd.GetArgRaw(3).argType);
            Assert.AreEqual("-", cmd.GetArg(3));
            cmd.Exec();
            Assert.AreEqual("-", src.Expression.GetValueAsString("a"));
        }

        [TestMethod]
        public void Parse_Assignment_NumericTerm_IsNumeric()
        {
            var src = CreateSrc();
            var cmd = Parse(src, "a = 007");
            Assert.AreEqual(Expressions.ValueType.NumericType, cmd.GetArgRaw(3).argType);
            cmd.Exec();
            Assert.AreEqual(7d, src.Expression.GetValueAsDouble("a"));
        }

        [TestMethod]
        public void Parse_Assignment_MultipleTerms_IsExpressionInParentheses()
        {
            // VB: strArgs(3) = "(" & ListTail(edata, 3) & ")"
            var src = CreateSrc();
            var cmd = Parse(src, "a = 1 + 2");
            Assert.AreEqual(3, cmd.ArgNum);
            Assert.AreEqual("(1 + 2)", cmd.GetArg(3));
            Assert.AreEqual(Expressions.ValueType.UndefinedType, cmd.GetArgRaw(3).argType);
            cmd.Exec();
            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("a"));
        }

        [TestMethod]
        public void Parse_Assignment_UnbalancedParen_IsNop()
        {
            // VB: ArgNum = -1 では「ArgNum >= 3」が成り立たず、NopCmd になる
            var src = CreateSrc();
            var cmd = Parse(src, "a = (1 + 2");
            Assert.IsInstanceOfType(cmd, typeof(NopCmd));
        }

        [TestMethod]
        [DataRow("a = \"abc")]
        [DataRow("a = (1 + 2")]
        [DataRow("サブルーチン (1")]
        public void Parse_UnclosedAtEnd_UnknownCommandIsNop(string line)
        {
            // VB の ListSplit は末尾でクォートや括弧が閉じていないと -1 を返し、定義済みでないコマンドは NopCmd になる
            var src = CreateSrc();
            var cmd = Parse(src, line);
            Assert.IsInstanceOfType(cmd, typeof(NopCmd));
            Assert.AreEqual(-1, cmd.ArgNum);
        }

        [TestMethod]
        [DataRow("Talk (システム")]
        [DataRow("Set a \"abc")]
        public void Parse_UnclosedAtEnd_ArgNumIsMinusOne(string line)
        {
            // VB: 引数の数は ListSplit の戻り値の -1 のまま（読み込み時の「括弧の対応が取れていません」の判定に使う）
            var src = CreateSrc();
            var cmd = Parse(src, line);
            Assert.AreEqual(-1, cmd.ArgNum);
        }

        // ──────────────────────────────────────────────
        // If・ElseIf の構文解析
        // ──────────────────────────────────────────────

        [TestMethod]
        [DataRow("If アムロ Then", 1)]
        [DataRow("If Not アムロ Then", 2)]
        [DataRow("If アムロ Goto ラベル", 1)]
        [DataRow("If Not アムロ Goto ラベル", 2)]
        // VB: 条件式の項数は ArgNum - 2（Goto は ArgNum - 3）。Then の後ろに項があると式として扱う
        [DataRow("If アムロ Then 余分", 0)]
        [DataRow("If アムロ Exit 余分", 0)]
        [DataRow("If Not アムロ Then 余分", 0)]
        [DataRow("If アムロ Goto ラベル 余分", 0)]
        public void Parse_If_ExprTermCount(string line, int expected)
        {
            var src = CreateSrc();
            var cmd = (AIfCmd)Parse(src, line);
            Assert.AreEqual(expected, cmd.ExprTermCount);
        }

        [TestMethod]
        public void Parse_If_WithoutCondition_ShowsFormatError()
        {
            // VB: DisplayEventErrorMessage CurrentLineNum, "Ifコマンドの書式に合っていません"
            var src = CreateSrc();
            Parse(src, "Set a 1");
            Parse(src, "If");
            var errors = src.DataErrors.ToArray();
            Assert.AreEqual(1, errors.Length);
            Assert.AreEqual("Ifコマンドの書式に合っていません", errors[0].msg);
            Assert.AreEqual(11, errors[0].line_num);
        }

        [TestMethod]
        public void Parse_If_GotoWithoutLabel_IsInvalidCommand()
        {
            // VB: GetArg(i + 1) が配列の範囲外になり、ErrorHandler の「イベントコマンドの内容が不正です」
            var src = CreateSrc();
            Parse(src, "If a Goto");
            CollectionAssert.AreEqual(new[] { "イベントコマンドの内容が不正です" }, ErrorMessages(src));
        }

        [TestMethod]
        public void Parse_If_WithoutThen_TerminatesSRC()
        {
            // VB: 「Ifに対応する Then または Exit または Goto がありません」を表示して TerminateSRC
            var src = CreateSrc();
            Parse(src, "If a = 1");
            CollectionAssert.AreEqual(new[] { "Ifに対応する Then または Exit または Goto がありません" }, ErrorMessages(src));
            Assert.IsTrue(((MockGUI)src.GUI).TerminateCalled);
        }

        [TestMethod]
        public void Parse_ElseIf_WithoutThen_TerminatesSRC()
        {
            var src = CreateSrc();
            Parse(src, "ElseIf a = 1");
            CollectionAssert.AreEqual(new[] { "ElseIfに対応する Then または Exit または Goto がありません" }, ErrorMessages(src));
            Assert.IsTrue(((MockGUI)src.GUI).TerminateCalled);
        }

        // ──────────────────────────────────────────────
        // PaintString の構文解析
        // ──────────────────────────────────────────────

        [TestMethod]
        [DataRow("PaintString 007", "007")]
        [DataRow("PaintString 1.50", "1.50")]
        public void PaintString_SingleUnquotedNumber_IsShownAsWritten(string line, string expected)
        {
            // VB: 引数が 1 個で引用符の無い表示文字列は型を未定（項）にし直し、GetValueAsString(buf, True) で評価する
            var src = CreateSrc();
            string msg = null;
            ((MockGUI)src.GUI).DrawStringHandler = (m, x, y, wcr) => msg = m;
            Parse(src, line).Exec();
            Assert.AreEqual(expected, msg);
        }

        // ──────────────────────────────────────────────
        // Exec
        // ──────────────────────────────────────────────

        [TestMethod]
        [DataRow("End")]
        [DataRow("Suspend")]
        public void Exec_EndOrSuspend_IsNotExecutableCommand(string line)
        {
            // VB: Exec の Select Case に EndCmd・SuspendCmd が無く、Case Else の「…というコマンドは存在しません」
            var src = CreateSrc();
            var cmd = Parse(src, line);
            Assert.AreEqual(-1, cmd.Exec());
            CollectionAssert.AreEqual(new[] { line + "というコマンドは存在しません" }, ErrorMessages(src));
        }
    }
}
