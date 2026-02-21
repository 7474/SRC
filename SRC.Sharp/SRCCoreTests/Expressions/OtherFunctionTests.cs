using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression経由で呼び出すその他の関数（Count, Eval, IIf, RegExp, Format）のユニットテスト
    /// </summary>
    [TestClass]
    public class OtherFunctionTests
    {
        private (Expression exp, SRC src) Create()
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
            return (src.Expression, src);
        }

        // ──────────────────────────────────────────────
        // Eval
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Eval_VariableExpression_EvaluatesVariable()
        {
            var (exp, src) = Create();
            exp.SetVariableAsDouble("x", 42d);
            Assert.AreEqual(42d, exp.GetValueAsDouble("Eval(\"x\")"));
        }

        [TestMethod]
        public void Eval_NumericLiteral_ReturnsNumber()
        {
            var (exp, src) = Create();
            Assert.AreEqual(42d, exp.GetValueAsDouble("Eval(\"42\")"));
        }

        // ──────────────────────────────────────────────
        // IIf
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IIf_TrueCondition_ReturnsTrueValue()
        {
            var (exp, src) = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("IIf(1,1,0)"));
        }

        [TestMethod]
        public void IIf_FalseCondition_ReturnsFalseValue()
        {
            var (exp, src) = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("IIf(0,1,0)"));
        }

        [TestMethod]
        public void IIf_StringBranch_ReturnsCorrectString()
        {
            var (exp, src) = Create();
            Assert.AreEqual("yes", exp.GetValueAsString("IIf(1,\"yes\",\"no\")"));
            Assert.AreEqual("no", exp.GetValueAsString("IIf(0,\"yes\",\"no\")"));
        }

        // ──────────────────────────────────────────────
        // Count
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Count_NoArrayElements_ReturnsZero()
        {
            var (exp, src) = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Count(arr)"));
        }

        [TestMethod]
        public void Count_WithArrayElements_ReturnsCount()
        {
            var (exp, src) = Create();
            exp.SetVariableAsDouble("arr[1]", 10d);
            exp.SetVariableAsDouble("arr[2]", 20d);
            exp.SetVariableAsDouble("arr[3]", 30d);
            Assert.AreEqual(3d, exp.GetValueAsDouble("Count(arr)"));
        }

        // ──────────────────────────────────────────────
        // RegExp
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RegExp_Match_ReturnsMatchedString()
        {
            var (exp, src) = Create();
            // パターン [a-z]+ で "hello" にマッチ
            var result = exp.GetValueAsString("RegExp(\"hello world\",\"[a-z]+\")");
            Assert.AreEqual("hello", result);
        }

        [TestMethod]
        public void RegExp_NoMatch_ReturnsEmpty()
        {
            var (exp, src) = Create();
            var result = exp.GetValueAsString("RegExp(\"hello\",\"[0-9]+\")");
            Assert.AreEqual("", result);
        }

        // ──────────────────────────────────────────────
        // RegExpReplace
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RegExpReplace_BasicSubstitution_ReturnsReplaced()
        {
            var (exp, src) = Create();
            var result = exp.GetValueAsString("RegExpReplace(\"hello world\",\"world\",\"SRC\")");
            Assert.AreEqual("hello SRC", result);
        }

        [TestMethod]
        public void RegExpReplace_PatternSubstitution_ReplacesAll()
        {
            var (exp, src) = Create();
            var result = exp.GetValueAsString("RegExpReplace(\"aababc\",\"a\",\"X\")");
            Assert.AreEqual("XXbXbc", result);
        }

        // ──────────────────────────────────────────────
        // Format (expression function)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Format_NumberWithFormat_ReturnsFormattedString()
        {
            var (exp, src) = Create();
            Assert.AreEqual("007", exp.GetValueAsString("Format(7,\"000\")"));
        }

        [TestMethod]
        public void Format_DoubleWithFormat_ReturnsFormattedString()
        {
            var (exp, src) = Create();
            Assert.AreEqual("3.14", exp.GetValueAsString("Format(3.14,\"0.##\")"));
        }

        // ──────────────────────────────────────────────
        // 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IIf_NonZeroNumericCondition_ReturnsTrueValue()
        {
            var (exp, src) = Create();
            Assert.AreEqual(100d, exp.GetValueAsDouble("IIf(5,100,0)"));
        }

        [TestMethod]
        public void Count_AfterSettingMoreElements_ReturnsNewCount()
        {
            var (exp, src) = Create();
            exp.SetVariableAsDouble("items[1]", 1d);
            exp.SetVariableAsDouble("items[2]", 2d);
            Assert.AreEqual(2d, exp.GetValueAsDouble("Count(items)"));
        }

        [TestMethod]
        public void Eval_VariableDouble_ReturnsValue()
        {
            var (exp, src) = Create();
            exp.SetVariableAsDouble("myNum", 55d);
            Assert.AreEqual(55d, exp.GetValueAsDouble("Eval(\"myNum\")"));
        }

        [TestMethod]
        public void RegExp_MatchGroupCapture_ReturnsFirstMatch()
        {
            var (exp, src) = Create();
            var result = exp.GetValueAsString("RegExp(\"abc123\",\"[0-9]+\")");
            Assert.AreEqual("123", result);
        }
    }
}
