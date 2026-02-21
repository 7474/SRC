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
    /// CmdArgument クラスの追加ユニットテスト（CmdArgumentTests.cs に未収録のケース）
    /// </summary>
    [TestClass]
    public class CmdArgumentMoreTests
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
        // デフォルトコンストラクタ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DefaultConstructor_AllFieldsAreDefaultValues()
        {
            // new CmdArgument() で作成した場合、各フィールドは .NET デフォルト値
            var arg = new CmdArgument();
            Assert.AreEqual(0, arg.lngArg);
            Assert.AreEqual(0d, arg.dblArg);
            Assert.IsNull(arg.strArg);
            // argType は enum のデフォルト値 (通常 0 = UndefinedType)
            Assert.AreEqual(ValueType.UndefinedType, arg.argType);
        }

        [TestMethod]
        public void DefaultConstructor_ToStringDoesNotThrow()
        {
            var arg = new CmdArgument();
            // null strArg でも ToString が例外を投げないことを確認
            var s = arg.ToString();
            Assert.IsNotNull(s);
        }

        // ──────────────────────────────────────────────
        // プロパティの読み書き
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Properties_StrArgCanBeEmptyString()
        {
            var arg = new CmdArgument { strArg = "" };
            Assert.AreEqual("", arg.strArg);
        }

        [TestMethod]
        public void Properties_LngArgCanBeNegative()
        {
            var arg = new CmdArgument { lngArg = -999 };
            Assert.AreEqual(-999, arg.lngArg);
        }

        [TestMethod]
        public void Properties_DblArgCanBeVerySmall()
        {
            var arg = new CmdArgument { dblArg = 1e-15 };
            Assert.AreEqual(1e-15, arg.dblArg, 1e-25);
        }

        [TestMethod]
        public void Properties_ArgTypeCanBeStringType()
        {
            var arg = new CmdArgument { argType = ValueType.StringType };
            Assert.AreEqual(ValueType.StringType, arg.argType);
        }

        [TestMethod]
        public void Properties_ArgTypeCanBeNumericType()
        {
            var arg = new CmdArgument { argType = ValueType.NumericType };
            Assert.AreEqual(ValueType.NumericType, arg.argType);
        }

        // ──────────────────────────────────────────────
        // Empty 静的フィールドの不変性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Empty_IsSameInstanceOnMultipleAccesses()
        {
            // Empty は static readonly なので同一インスタンスを参照する
            Assert.AreSame(CmdArgument.Empty, CmdArgument.Empty);
        }

        [TestMethod]
        public void Empty_ArgTypeIsUndefined()
        {
            Assert.AreEqual(ValueType.UndefinedType, CmdArgument.Empty.argType);
        }

        // ──────────────────────────────────────────────
        // ParseArgs エッジケース（SetCmd 経由）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ParseArgs_JapaneseStringLiteral_IsStringType()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x \"日本語テスト\"");
            var arg = cmd.GetArgRaw(3);
            Assert.AreEqual(ValueType.StringType, arg.argType);
            Assert.AreEqual("日本語テスト", arg.strArg);
        }

        [TestMethod]
        public void ParseArgs_ZeroLiteral_IsNumericTypeWithZero()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 0");
            var arg = cmd.GetArgRaw(3);
            Assert.AreEqual(ValueType.NumericType, arg.argType);
            Assert.AreEqual(0, arg.lngArg);
            Assert.AreEqual(0d, arg.dblArg);
        }

        [TestMethod]
        public void ParseArgs_LargePositiveNumber_IsNumericType()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 1000000");
            var arg = cmd.GetArgRaw(3);
            Assert.AreEqual(ValueType.NumericType, arg.argType);
            Assert.AreEqual(1000000, arg.lngArg);
        }

        [TestMethod]
        public void ParseArgs_EmptyStringLiteral_IsStringType()
        {
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x \"\"");
            var arg = cmd.GetArgRaw(3);
            Assert.AreEqual(ValueType.StringType, arg.argType);
            Assert.AreEqual("", arg.strArg);
        }

        [TestMethod]
        public void ParseArgs_CommandNameArg_IsUndefinedType()
        {
            // 引数1（コマンド名）は UndefinedType
            var src = CreateSrc();
            var cmd = CreateSetCmd(src, "Set x 1");
            var arg = cmd.GetArgRaw(1);
            Assert.AreEqual(ValueType.UndefinedType, arg.argType);
            Assert.AreEqual("Set", arg.strArg);
        }

        [TestMethod]
        public void ParseArgs_SpecialCharInString_HandledAsString()
        {
            var src = CreateSrc();
            // 括弧を含む文字列リテラル
            var cmd = CreateSetCmd(src, "Set x \"(hello)\"");
            var arg = cmd.GetArgRaw(3);
            Assert.AreEqual(ValueType.StringType, arg.argType);
            Assert.AreEqual("(hello)", arg.strArg);
        }

        // ──────────────────────────────────────────────
        // ToString フォーマット
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ContainsDblArg()
        {
            var arg = new CmdArgument { strArg = "v", lngArg = 2, dblArg = 9.9 };
            var s = arg.ToString();
            // フォーマット: "{strArg}({lngArg}L)({dblArg}d)"
            Assert.IsTrue(s.Contains("9.9"), $"Expected '9.9' in '{s}'");
        }

        [TestMethod]
        public void ToString_ContainsLSuffix()
        {
            var arg = new CmdArgument { strArg = "q", lngArg = 7, dblArg = 0d };
            var s = arg.ToString();
            Assert.IsTrue(s.Contains("7L"), $"Expected '7L' in '{s}'");
        }
    }
}
