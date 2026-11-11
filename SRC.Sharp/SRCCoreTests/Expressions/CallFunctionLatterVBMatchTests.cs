using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression.bas の CallFunction の後半（isvardefined〜・ユーザー定義関数）を、
    /// 元の SRC の動作を期待値にして確かめる（7474/SRC#1498）。
    /// </summary>
    [TestClass]
    public class CallFunctionLatterVBMatchTests
    {
        private static SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            return src;
        }

        private static void BuildEvent(SRC src, params string[] lines)
        {
            var parser = new CmdParser();
            for (var i = 0; i < lines.Length; i++)
            {
                var line = new EventDataLine(i, EventDataSource.Scenario, "test", i, lines[i]);
                src.Event.EventData.Add(line);
                src.Event.EventCmd.Add(parser.Parse(src, line));
                var trimmed = lines[i].Trim();
                if (trimmed.EndsWith(":"))
                {
                    src.Event.AddLabel(trimmed.Substring(0, trimmed.Length - 1).Trim(), i);
                }
            }
        }

        [TestMethod]
        public void UserFunction_KeepsCallersUpVarLevel()
        {
            // 元の SRC: UpVarLevelStack(CallDepth) = UpVarLevel（Call コマンドと違い累計しない）。
            // 戻ったあとの UpVarLevel は呼ぶ前と同じ
            var src = CreateSrc();
            BuildEvent(src,
                "Set dummy 0",
                "myfunc:",
                "Return 5");
            src.Event.CallDepth = 2;
            src.Event.UpVarLevelStack[1] = 1;
            src.Event.UpVarLevel = 1;

            Assert.AreEqual(5d, src.Expression.GetValueAsDouble("myfunc()"));
            Assert.AreEqual(1, src.Event.UpVarLevel);
            Assert.AreEqual(1, src.Event.UpVarLevelStack[2]);
            Assert.AreEqual(2, src.Event.CallDepth);
        }

        [TestMethod]
        public void UserFunction_CanBeCalledAtMaxCallDepth()
        {
            // 元の SRC: If CallDepth > MaxCallDepth Then（MaxCallDepth のときはまだ呼べる。Call コマンドと同じ）
            var src = CreateSrc();
            BuildEvent(src,
                "Set dummy 0",
                "myfunc:",
                "Return 7");
            src.Event.CallDepth = Event.MaxCallDepth;

            Assert.AreEqual(7d, src.Expression.GetValueAsDouble("myfunc()"));
            Assert.AreEqual(Event.MaxCallDepth, src.Event.CallDepth);
        }

        [TestMethod]
        public void SystemFunctionReturningUndefined_DoesNotCallSameNamedLabel()
        {
            // 元の SRC: システム関数の Case は Exit Function で抜け、同じ名前のラベルを呼ばない
            // （DiffTime の日付の変換に失敗したときは UndefinedType を返し、EvalTerm が GetVariable に回す）
            var src = CreateSrc();
            BuildEvent(src,
                "Set dummy 0",
                "difftime:",
                "Set called 1",
                "Return 1");
            src.Event.CallDepth = 1;

            src.Expression.GetValueAsDouble("DiffTime(\"x\",\"y\")");

            Assert.IsFalse(src.Expression.IsVariableDefined("called"));
        }

        [TestMethod]
        public void Relation_UnknownFirstPilot_DoesNotEvaluateSecondArgument()
        {
            // 元の SRC: 1 人目のパイロットがいなければ、2 つ目の引数を評価せずに 0
            var src = CreateSrc();
            BuildEvent(src,
                "Set dummy 0",
                "sidefx:",
                "Set called 1",
                "Return \"x\"");
            src.Event.CallDepth = 1;

            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("Relation(\"いない\", sidefx())"));
            Assert.IsFalse(src.Expression.IsVariableDefined("called"));
        }

        [TestMethod]
        public void LSet_PadsByShiftJisByteLength()
        {
            // 元の SRC: LenB(StrConv(buf, vbFromUnicode))。「×」は Shift_JIS で 2 バイト
            var src = CreateSrc();
            Assert.AreEqual("×1 ", src.Expression.GetValueAsString("LSet(\"×1\", 4)"));
            Assert.AreEqual("あ  ", src.Expression.GetValueAsString("LSet(\"あ\", 4)"));
        }

        [TestMethod]
        public void RSet_PadsByShiftJisByteLength()
        {
            var src = CreateSrc();
            Assert.AreEqual(" ×1", src.Expression.GetValueAsString("RSet(\"×1\", 4)"));
            Assert.AreEqual("abcde", src.Expression.GetValueAsString("RSet(\"abcde\", 3)"));
        }

        [TestMethod]
        public void MaxMin_WithoutArguments_ReturnZero()
        {
            // 元の SRC: 空の params(1) を評価して 0
            var src = CreateSrc();
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("Max()"));
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("Min()"));
            Assert.AreEqual(3d, src.Expression.GetValueAsDouble("Max(1, 3, 2)"));
            Assert.AreEqual(-1d, src.Expression.GetValueAsDouble("Min(1, -1, 2)"));
        }

        [TestMethod]
        public void RegExp_DigitClassMatchesAsciiOnly()
        {
            // 元の SRC は VBScript.RegExp。\d は半角の数字だけ、\s は全角の空白に合わない
            var src = CreateSrc();
            Assert.AreEqual("", src.Expression.GetValueAsString("RegExp(\"第２話\", \"\\d+\")"));
            Assert.AreEqual("12", src.Expression.GetValueAsString("RegExp(\"第12話\", \"\\d+\")"));
            Assert.AreEqual("全角　空白", src.Expression.GetValueAsString("RegExpReplace(\"全角　空白\", \"\\s\", \"_\")"));
        }

        [TestMethod]
        public void RegExpReplace_FourthArgumentIgnoresCase()
        {
            var src = CreateSrc();
            Assert.AreEqual("AxC", src.Expression.GetValueAsString("RegExpReplace(\"ABC\", \"b\", \"x\", \"大小区別なし\")"));
            Assert.AreEqual("ABC", src.Expression.GetValueAsString("RegExpReplace(\"ABC\", \"b\", \"x\")"));
        }

        [TestMethod]
        public void RegExp_InvalidPattern_ShowsErrorAndContinues()
        {
            // 元の SRC: On Error GoTo RegExp_Error でエラーを表示し、例外にせずに続ける
            var src = CreateSrc();
            BuildEvent(src, "Set dummy 0");
            var before = src.DataErrors.Count;

            src.Expression.GetValueAsString("RegExp(\"abc\", \"(\")");

            Assert.AreEqual(before + 1, src.DataErrors.Count);
        }

        [TestMethod]
        public void Weekday_TooManyArguments_ReturnsEmpty()
        {
            var src = CreateSrc();
            Assert.AreEqual("", src.Expression.GetValueAsString("Weekday(\"2024/01/01\", 1)"));
            Assert.AreEqual("月曜", src.Expression.GetValueAsString("Weekday(\"2024/01/01\")"));
        }

        [TestMethod]
        public void LevelMorale_SelectedUnitWithoutPilot_ReturnZero()
        {
            // 元の SRC: Level()・Morale() は選択中のユニットの CountPilot > 0 を確かめてから MainPilot を見る
            var src = CreateSrc();
            var ud = src.UDList.Add("無人機");
            ud.HP = 1000;
            ud.EN = 100;
            var u = src.UList.Add("無人機", 0, "味方");
            src.Event.SelectedUnitForEvent = u;

            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("Level()"));
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("Morale()"));
        }

        [TestMethod]
        public void Level_TooManyArguments_ReturnsZero()
        {
            var src = CreateSrc();
            src.PDList.Add("アムロ");
            src.PList.Add("アムロ", 5, "味方");
            Assert.AreEqual(5d, src.Expression.GetValueAsDouble("Level(\"アムロ\")"));
            Assert.AreEqual(0d, src.Expression.GetValueAsDouble("Level(\"アムロ\", 1)"));
        }
    }
}
