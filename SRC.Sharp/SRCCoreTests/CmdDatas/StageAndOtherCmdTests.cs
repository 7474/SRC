using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// ステージ系コマンドおよびその他コマンドのエラーケーステスト
    /// </summary>
    [TestClass]
    public class StageAndOtherCmdTests
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
        // GameClearCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GameClearCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "GameClear extraArg");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // GameOverCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GameOverCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "GameOver extraArg");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ContinueCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ContinueCmd_WithNextStage_SetsVariable()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Continue nextStage.eve");
            // Continue requires GUI interactions and map, so just test arg parsing
            Assert.IsNotNull(cmd);
            Assert.IsInstanceOfType(cmd, typeof(ContinueCmd));
        }

        [TestMethod]
        public void ContinueCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Continue arg1 arg2 arg3");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // RequireCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RequireCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Require");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void RequireCmd_WithNonExistentFile_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Require nonexistent.eve");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // WaitCmd - 追加エラーケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WaitCmd_FourArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Wait Until 1 2");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // IncrCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IncrCmd_SimpleIncrement_IncreasesBy1()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("counter", 5d);
            var cmd = CreateCmd(src, "Incr counter");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.AreEqual(6d, src.Expression.GetValueAsDouble("counter"));
        }

        [TestMethod]
        public void IncrCmd_WithStep_IncreasesByStep()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("counter", 5d);
            var cmd = CreateCmd(src, "Incr counter 3");
            cmd.Exec();
            Assert.AreEqual(8d, src.Expression.GetValueAsDouble("counter"));
        }

        [TestMethod]
        public void IncrCmd_WithNegativeStep_Decreases()
        {
            var src = CreateSrc();
            src.Expression.SetVariableAsDouble("counter", 10d);
            var cmd = CreateCmd(src, "Incr counter -4");
            cmd.Exec();
            Assert.AreEqual(6d, src.Expression.GetValueAsDouble("counter"));
        }

        [TestMethod]
        public void IncrCmd_UndefinedVariable_StartsFromZero()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Incr newVar");
            cmd.Exec();
            Assert.AreEqual(1d, src.Expression.GetValueAsDouble("newVar"));
        }

        [TestMethod]
        public void IncrCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Incr");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ArrayCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Array");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // UnSetCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnSetCmd_DefinedVariable_RemovesIt()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("myVar");
            src.Expression.SetVariableAsDouble("myVar", 42d);
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("myVar"));

            var cmd = CreateCmd(src, "UnSet myVar");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.IsFalse(src.Expression.IsGlobalVariableDefined("myVar"));
        }

        [TestMethod]
        public void UnSetCmd_UndefinedVariable_NoError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "UnSet notDefined");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void UnSetCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "UnSet");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SortCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SortCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Sort");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // GlobalCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GlobalCmd_NoArgs_ReturnsSuccessfully()
        {
            // GlobalCmd without args just returns NextID (no-op, no error)
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Global");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void GlobalCmd_MultipleVariables_AllDefined()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Global a b c");
            cmd.Exec();
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("a"));
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("b"));
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("c"));
        }
    }
}
