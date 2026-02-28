using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.Expressions;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// CmdData 基底クラスおよび CmdArgument のエッジケーステスト
    /// </summary>
    [TestClass]
    public class CmdDataBaseTests
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

        private SetCmd CreateSetCmd(SRC src, string cmdText, int id = 0)
        {
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, cmdText);
            src.Event.EventData.Add(line);
            var cmd = new SetCmd(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        // ──────────────────────────────────────────────
        // GetArgRaw 境界値テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetArgRaw_IndexZero_ThrowsArgumentOutOfRange()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 1");
            // idx=0 は args[-1] にアクセスし例外発生
            Assert.Throws<System.ArgumentOutOfRangeException>(() => cmd.GetArgRaw(0));
        }

        [TestMethod]
        public void GetArgRaw_NegativeIndex_ThrowsArgumentOutOfRange()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 1");
            Assert.Throws<System.ArgumentOutOfRangeException>(() => cmd.GetArgRaw(-1));
        }

        [TestMethod]
        public void GetArgRaw_ExactArgNum_ReturnsValidArg()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 1");
            // ArgNum == 3, so index 3 should be valid
            var arg = cmd.GetArgRaw(cmd.ArgNum);
            Assert.AreNotSame(CmdArgument.Empty, arg);
        }

        [TestMethod]
        public void GetArgRaw_ArgNumPlusOne_ReturnsEmpty()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 1");
            var arg = cmd.GetArgRaw(cmd.ArgNum + 1);
            Assert.AreSame(CmdArgument.Empty, arg);
        }

        [TestMethod]
        public void GetArgRaw_VeryLargeIndex_ReturnsEmpty()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 1");
            var arg = cmd.GetArgRaw(int.MaxValue);
            Assert.AreSame(CmdArgument.Empty, arg);
        }

        // ──────────────────────────────────────────────
        // GetArg 境界値テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetArg_IndexZero_ThrowsArgumentOutOfRange()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 1");
            Assert.Throws<System.ArgumentOutOfRangeException>(() => cmd.GetArg(0));
        }

        [TestMethod]
        public void GetArg_NegativeIndex_ThrowsArgumentOutOfRange()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 1");
            Assert.Throws<System.ArgumentOutOfRangeException>(() => cmd.GetArg(-5));
        }

        // ──────────────────────────────────────────────
        // GetArgs 列挙テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetArgs_SingleWordCommand_ReturnsOneArg()
        {
            var src = CreateSrc();
            // "Exit" のような単一コマンド名
            var line = new EventDataLine(0, EventDataSource.Scenario, "test", 0, "Exit");
            src.Event.EventData.Add(line);
            var cmd = new SetCmd(src, line);
            src.Event.EventCmd.Add(cmd);

            var args = cmd.GetArgs().ToList();
            Assert.AreEqual(1, args.Count);
            Assert.AreEqual("Exit", args[0].strArg);
        }

        [TestMethod]
        public void GetArgs_ReturnsArgsInOrder()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set myVar 42");
            var args = cmd.GetArgs().ToList();

            Assert.AreEqual("Set", args[0].strArg);
            Assert.AreEqual("myVar", args[1].strArg);
            Assert.AreEqual("42", args[2].strArg);
        }

        [TestMethod]
        public void GetArgs_MultipleEnumeration_ReturnsSameResults()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 10");
            var first = cmd.GetArgs().ToList();
            var second = cmd.GetArgs().ToList();

            Assert.AreEqual(first.Count, second.Count);
            for (int i = 0; i < first.Count; i++)
            {
                Assert.AreSame(first[i], second[i]);
            }
        }

        // ──────────────────────────────────────────────
        // ParseArgs 型判定 - 括弧で始まる式
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ParseArgs_ParenthesisExpression_IsUndefinedType()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x (1+2)");
            var arg = cmd.GetArgRaw(3);
            // `(` で始まる引数は式として UndefinedType のまま
            Assert.AreEqual(ValueType.UndefinedType, arg.argType);
            Assert.AreEqual("(1+2)", arg.strArg);
        }

        [TestMethod]
        public void ParseArgs_NegativeFloat_IsNumericType()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x -3.14");
            var arg = cmd.GetArgRaw(3);
            Assert.AreEqual(ValueType.NumericType, arg.argType);
            Assert.AreEqual(-3.14d, arg.dblArg, 1e-10);
        }

        [TestMethod]
        public void ParseArgs_NegativeNonNumeric_IsStringType()
        {
            var src = CreateSrc();
            // "-abc" は数値ではないので StringType
            var cmd = CreateSetCmd(src, "Set x -abc");
            var arg = cmd.GetArgRaw(3);
            Assert.AreEqual(ValueType.StringType, arg.argType);
        }

        [TestMethod]
        public void ParseArgs_BacktickWithoutClosing_IsStringType()
        {
            var src = CreateSrc();
            // 閉じバッククォートがない場合
            var cmd = CreateSetCmd(src, "Set x `hello");
            var arg = cmd.GetArgRaw(3);
            Assert.AreEqual(ValueType.StringType, arg.argType);
            // 閉じがないのでstrArgはそのまま
            Assert.AreEqual("`hello", arg.strArg);
        }

        [TestMethod]
        public void ParseArgs_DoubleQuoteWithoutClosing_IsStringType()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x \"hello");
            var arg = cmd.GetArgRaw(3);
            Assert.AreEqual(ValueType.StringType, arg.argType);
        }

        [TestMethod]
        public void ParseArgs_DigitStartingNonNumeric_IsStringType()
        {
            var src = CreateSrc();
            // "1abc" は IsNumeric=false なので StringType
            var cmd = CreateSetCmd(src, "Set x 1abc");
            var arg = cmd.GetArgRaw(3);
            Assert.AreEqual(ValueType.StringType, arg.argType);
        }

        // ──────────────────────────────────────────────
        // GetArgAsLong / GetArgAsDouble - NumericType 境界
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetArgAsLong_NegativeNumeric_ReturnsNegative()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x -99");
            Assert.AreEqual(-99, cmd.GetArgAsLong(3));
        }

        [TestMethod]
        public void GetArgAsDouble_NegativeFloat_ReturnsNegativeDouble()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x -2.5");
            Assert.AreEqual(-2.5d, cmd.GetArgAsDouble(3), 1e-10);
        }

        [TestMethod]
        public void GetArgAsLong_ZeroLiteral_ReturnsZero()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 0");
            Assert.AreEqual(0, cmd.GetArgAsLong(3));
        }

        [TestMethod]
        public void GetArgAsDouble_ZeroLiteral_ReturnsZero()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 0");
            Assert.AreEqual(0d, cmd.GetArgAsDouble(3), 1e-10);
        }

        [TestMethod]
        public void GetArgAsString_NumericZero_ReturnsZeroString()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 0");
            Assert.AreEqual("0", cmd.GetArgAsString(3));
        }

        // ──────────────────────────────────────────────
        // ArgNum テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArgNum_SingleWord_IsOne()
        {
            var src = CreateSrc();
            var line = new EventDataLine(0, EventDataSource.Scenario, "test", 0, "Exit");
            src.Event.EventData.Add(line);
            var cmd = new SetCmd(src, line);
            Assert.AreEqual(1, cmd.ArgNum);
        }

        [TestMethod]
        public void ArgNum_FourArgs_IsFour()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 1 extra");
            Assert.AreEqual(4, cmd.ArgNum);
        }

        // ──────────────────────────────────────────────
        // AfterEventIdRange / BeforeEventIdRange
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AfterEventIdRange_ReturnsRangeFromNextID()
        {
            var src = CreateSrc();
            // IDが0のコマンドを追加（EventCmdは3つ）
            for (int i = 0; i < 3; i++)
            {
                var line = new EventDataLine(i, EventDataSource.Scenario, "test", i, "Set x " + i);
                src.Event.EventData.Add(line);
                var c = new SetCmd(src, line);
                src.Event.EventCmd.Add(c);
            }
            var cmd = src.Event.EventCmd[0]; // ID=0, NextID=1
            var range = cmd.AfterEventIdRange().ToList();
            // NextID=1 から EventCmd.Count(3)-1 まで
            Assert.IsTrue(range.Count > 0);
            Assert.AreEqual(1, range[0]);
        }

        [TestMethod]
        public void BeforeEventIdRange_ReturnsRangeDescending()
        {
            var src = CreateSrc();
            for (int i = 0; i < 3; i++)
            {
                var line = new EventDataLine(i, EventDataSource.Scenario, "test", i, "Set x " + i);
                src.Event.EventData.Add(line);
                var c = new SetCmd(src, line);
                src.Event.EventCmd.Add(c);
            }
            var cmd = src.Event.EventCmd[2]; // ID=2
            var range = cmd.BeforeEventIdRange().ToList();
            // ID-1=1 から 0 まで降順
            Assert.AreEqual(2, range.Count);
            Assert.AreEqual(1, range[0]);
            Assert.AreEqual(0, range[1]);
        }

        [TestMethod]
        public void BeforeEventIdRange_FirstCmd_ReturnsEmpty()
        {
            var src = CreateSrc();
            var line = new EventDataLine(0, EventDataSource.Scenario, "test", 0, "Set x 1");
            src.Event.EventData.Add(line);
            var cmd = new SetCmd(src, line);
            src.Event.EventCmd.Add(cmd);
            // ID=0 の BeforeEventIdRange は空
            var range = cmd.BeforeEventIdRange().ToList();
            Assert.AreEqual(0, range.Count);
        }

        // ──────────────────────────────────────────────
        // CmdArgument.Empty のプロパティ確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CmdArgument_Empty_ToString_ContainsZeroValues()
        {
            var str = CmdArgument.Empty.ToString();
            Assert.IsTrue(str.Contains("0L"));
            Assert.IsTrue(str.Contains("0d"));
        }

        [TestMethod]
        public void CmdArgument_Empty_StrArg_IsNotNull()
        {
            Assert.IsNotNull(CmdArgument.Empty.strArg);
            Assert.AreEqual(0, CmdArgument.Empty.strArg.Length);
        }

        // ──────────────────────────────────────────────
        // CmdData.ToString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ContainsCmdTypeName()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 10");
            var str = cmd.ToString();
            Assert.IsTrue(str.Contains("Set"));
        }

        [TestMethod]
        public void ToString_ContainsEventDataContent()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set myVar 999");
            var str = cmd.ToString();
            // EventData.ToString() は "ID: Data" 形式
            Assert.IsTrue(str.Contains("myVar") || str.Contains("999"));
        }
    }
}
