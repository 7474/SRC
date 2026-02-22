using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// 変数操作コマンドの追加ユニットテスト（GlobalCmd, UnSetCmd, IncrCmd, SwapCmd の各種ケース）
    /// </summary>
    [TestClass]
    public class VariableCmdAdditionalTests
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

        private CmdData CreateCmd(SRC src, string cmdText, int id = 0)
        {
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, cmdText);
            src.Event.EventData.Add(line);
            var parser = new CmdParser();
            var cmd = parser.Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        // ──────────────────────────────────────────────
        // GlobalCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GlobalCmd_DefinesSingleVariable()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Global myVar");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("myVar"));
        }

        [TestMethod]
        public void GlobalCmd_DefinesMultipleVariables()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Global a b c");
            cmd.Exec();
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("a"));
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("b"));
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("c"));
        }

        [TestMethod]
        public void GlobalCmd_AlreadyDefined_DoesNotThrow()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("existingVar");
            var cmd = CreateCmd(src, "Global existingVar");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("existingVar"));
        }

        [TestMethod]
        public void GlobalCmd_WithDollarPrefix_DefinesVariable()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Global $strVar");
            cmd.Exec();
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("strVar"));
        }

        [TestMethod]
        public void GlobalCmd_NoArguments_ReturnsNextId()
        {
            var src = CreateSrc();
            // Global コマンドに引数なしでも次のIDを返す
            var cmd = CreateCmd(src, "Global");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        // ──────────────────────────────────────────────
        // UnSetCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnSetCmd_UndefinedVariable_ReturnsNextId()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("myVar");
            src.Expression.SetVariableAsDouble("myVar", 42d);
            var cmd = CreateCmd(src, "UnSet myVar");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void UnSetCmd_RemovesVariable()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("removeMe");
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("removeMe"));
            var cmd = CreateCmd(src, "UnSet removeMe");
            cmd.Exec();
            Assert.IsFalse(src.Expression.IsGlobalVariableDefined("removeMe"));
        }

        [TestMethod]
        public void UnSetCmd_NonExistentVariable_DoesNotThrow()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "UnSet nonExistent");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        // ──────────────────────────────────────────────
        // IncrCmd - 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IncrCmd_ByOne_IncreasesValue()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("counter", 5d);
            var cmd = CreateCmd(src, "Incr counter");
            cmd.Exec();
            Assert.AreEqual(6d, src.Expression.GetValueAsDouble("counter"));
        }

        [TestMethod]
        public void IncrCmd_ByAmount_IncreasesValue()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("val", 10d);
            var cmd = CreateCmd(src, "Incr val 5");
            cmd.Exec();
            Assert.AreEqual(15d, src.Expression.GetValueAsDouble("val"));
        }

        [TestMethod]
        public void IncrCmd_ByNegativeAmount_DecreasesValue()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("val", 10d);
            var cmd = CreateCmd(src, "Incr val -3");
            cmd.Exec();
            Assert.AreEqual(7d, src.Expression.GetValueAsDouble("val"));
        }

        [TestMethod]
        public void IncrCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Incr a b c");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void IncrCmd_FromZero_ByOne()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("zero", 0d);
            var cmd = CreateCmd(src, "Incr zero");
            cmd.Exec();
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("zero"));
        }

        // ──────────────────────────────────────────────
        // SwapCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SwapCmd_SwapsNumericValues()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("a", 10d);
            src.Expression.SetVariableAsDouble("b", 20d);
            var cmd = CreateCmd(src, "Swap a b");
            cmd.Exec();
            Assert.AreEqual(20d, src.Expression.GetValueAsDouble("a"));
            Assert.AreEqual(10d, src.Expression.GetValueAsDouble("b"));
        }

        [TestMethod]
        public void SwapCmd_SwapsStringValues()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsString("x", "hello");
            src.Expression.SetVariableAsString("y", "world");
            var cmd = CreateCmd(src, "Swap x y");
            cmd.Exec();
            Assert.AreEqual("world", src.Expression.GetValueAsString("x"));
            Assert.AreEqual("hello", src.Expression.GetValueAsString("y"));
        }

        [TestMethod]
        public void SwapCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Swap a");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void SwapCmd_ReturnsNextId()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("p", 1d);
            src.Expression.SetVariableAsDouble("q", 2d);
            var cmd = CreateCmd(src, "Swap p q");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        // ──────────────────────────────────────────────
        // OptionCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void OptionCmd_SetOption_DefinesGlobalVariable()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Option テスト");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("Option(テスト)"));
        }

        [TestMethod]
        public void OptionCmd_SetOption_ValueIsOne()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Option テスト");
            cmd.Exec();
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("Option(テスト)"));
        }

        [TestMethod]
        public void OptionCmd_SetOptionThenUnset_RemovesVariable()
        {
            var src = CreateSrc();
            // まず Set
            CreateCmd(src, "Option アニメ").Exec();
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("Option(アニメ)"));
            // 次に Unset（3引数形式）
            CreateCmd(src, "Option アニメ off").Exec();
            Assert.IsFalse(src.Expression.IsGlobalVariableDefined("Option(アニメ)"));
        }

        [TestMethod]
        public void OptionCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Option");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void OptionCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Option a b c");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }
    }
}
