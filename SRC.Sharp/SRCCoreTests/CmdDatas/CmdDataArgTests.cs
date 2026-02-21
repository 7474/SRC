using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.Expressions;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// CmdData 基底クラスの引数アクセスメソッドのユニットテスト
    /// </summary>
    [TestClass]
    public class CmdDataArgTests
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

        private SetCmd CreateSetCmd(SRC src, string cmdText, int id = 0)
        {
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, cmdText);
            src.Event.EventData.Add(line);
            var cmd = new SetCmd(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        // ──────────────────────────────────────────────
        // ArgNum
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArgNum_WithTwoArgs_IsTwo()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x");
            Assert.AreEqual(2, cmd.ArgNum);
        }

        [TestMethod]
        public void ArgNum_WithThreeArgs_IsThree()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 42");
            Assert.AreEqual(3, cmd.ArgNum);
        }

        // ──────────────────────────────────────────────
        // GetArg
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetArg_Index1_ReturnsCommandName()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 100");
            Assert.AreEqual("Set", cmd.GetArg(1));
        }

        [TestMethod]
        public void GetArg_Index2_ReturnsFirstParam()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set myVar 50");
            Assert.AreEqual("myVar", cmd.GetArg(2));
        }

        [TestMethod]
        public void GetArg_Index3_ReturnsSecondParam()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 99");
            Assert.AreEqual("99", cmd.GetArg(3));
        }

        [TestMethod]
        public void GetArg_OutOfRange_ReturnsEmpty()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 1");
            Assert.AreEqual("", cmd.GetArg(10));
        }

        // ──────────────────────────────────────────────
        // GetArgAsString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetArgAsString_StringLiteral_ReturnsString()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x \"hello\"");
            Assert.AreEqual("hello", cmd.GetArgAsString(3));
        }

        [TestMethod]
        public void GetArgAsString_NumericArg_ReturnsFormattedString()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 42");
            var val = cmd.GetArgAsString(3);
            Assert.AreEqual("42", val);
        }

        [TestMethod]
        public void GetArgAsString_VariableArg_ReturnsVariableValue()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsString("myVar", "テスト");
            var cmd = CreateSetCmd(src, "Set x myVar");
            // myVar is undefined type, so it evaluates
            Assert.AreEqual("テスト", cmd.GetArgAsString(3));
        }

        // ──────────────────────────────────────────────
        // GetArgAsLong
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetArgAsLong_NumericArg_ReturnsInt()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 123");
            Assert.AreEqual(123, cmd.GetArgAsLong(3));
        }

        [TestMethod]
        public void GetArgAsLong_StringArg_ReturnsZero()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x \"abc\"");
            Assert.AreEqual(0, cmd.GetArgAsLong(3));
        }

        // ──────────────────────────────────────────────
        // GetArgAsDouble
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetArgAsDouble_NumericArg_ReturnsDouble()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 3.14");
            Assert.AreEqual(3.14d, cmd.GetArgAsDouble(3), 1e-10);
        }

        [TestMethod]
        public void GetArgAsDouble_StringArg_ReturnsZero()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x \"text\"");
            Assert.AreEqual(0d, cmd.GetArgAsDouble(3));
        }

        // ──────────────────────────────────────────────
        // GetArgRaw
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetArgRaw_ValidIndex_ReturnsArgument()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 42");
            var arg = cmd.GetArgRaw(3);
            Assert.IsNotNull(arg);
            Assert.AreEqual(ValueType.NumericType, arg.argType);
            Assert.AreEqual(42, arg.lngArg);
        }

        [TestMethod]
        public void GetArgRaw_OutOfRange_ReturnsEmpty()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 1");
            var arg = cmd.GetArgRaw(99);
            Assert.AreSame(CmdArgument.Empty, arg);
        }

        // ──────────────────────────────────────────────
        // CmdData.ToString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ContainsCommandNameAndData()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 10");
            var str = cmd.ToString();
            Assert.IsTrue(str.Contains("Set"));
        }

        // ──────────────────────────────────────────────
        // ParseArgs - 引数の型判定
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ParseArgs_StringLiteralWithQuotes_IsStringType()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x \"hello\"");
            var arg = cmd.GetArgRaw(3);
            Assert.AreEqual(ValueType.StringType, arg.argType);
        }

        [TestMethod]
        public void ParseArgs_NumericArg_IsNumericType()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 123");
            var arg = cmd.GetArgRaw(3);
            Assert.AreEqual(ValueType.NumericType, arg.argType);
        }

        [TestMethod]
        public void ParseArgs_NegativeNumericArg_IsNumericType()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x -5");
            var arg = cmd.GetArgRaw(3);
            Assert.AreEqual(ValueType.NumericType, arg.argType);
            Assert.AreEqual(-5, arg.lngArg);
        }

        [TestMethod]
        public void ParseArgs_UndefinedTypeForVariables()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x myVar");
            var arg = cmd.GetArgRaw(3);
            Assert.AreEqual(ValueType.UndefinedType, arg.argType);
        }

        [TestMethod]
        public void ParseArgs_BacktickQuoted_IsStringType()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x `hello`");
            var arg = cmd.GetArgRaw(3);
            Assert.AreEqual(ValueType.StringType, arg.argType);
            Assert.AreEqual("hello", arg.strArg);
        }

        [TestMethod]
        public void ParseArgs_FloatNumber_IsNumericType()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 1.5");
            var arg = cmd.GetArgRaw(3);
            Assert.AreEqual(ValueType.NumericType, arg.argType);
            Assert.AreEqual(1.5d, arg.dblArg);
        }

        // ──────────────────────────────────────────────
        // GetArgs (IEnumerable)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetArgs_ReturnsAllArgs()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 10");
            var args = new System.Collections.Generic.List<CmdArgument>(cmd.GetArgs());
            Assert.AreEqual(3, args.Count);
        }
    }
}
