using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// Money/Debug/Global コマンドのユニットテスト
    /// </summary>
    [TestClass]
    public class MiscCmdTests
    {
        private SRC CreateSrc()
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
        // MoneyCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MoneyCmd_IncreasesMoney()
        {
            var src = CreateSrc();
            src.Money = 1000;
            var cmd = CreateCmd(src, "Money 500");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.AreEqual(1500, src.Money);
        }

        [TestMethod]
        public void MoneyCmd_DecreasesMoney()
        {
            var src = CreateSrc();
            src.Money = 1000;
            var cmd = CreateCmd(src, "Money -500");
            cmd.Exec();
            Assert.AreEqual(500, src.Money);
        }

        [TestMethod]
        public void MoneyCmd_ClampsToZero_WhenMoneyGoesNegative()
        {
            var src = CreateSrc();
            src.Money = 100;
            var cmd = CreateCmd(src, "Money -1000");
            cmd.Exec();
            Assert.AreEqual(0, src.Money);
        }

        [TestMethod]
        public void MoneyCmd_ClampsToMax()
        {
            var src = CreateSrc();
            src.Money = 999999000;
            var cmd = CreateCmd(src, "Money 9999");
            cmd.Exec();
            Assert.AreEqual(999999999, src.Money);
        }

        // ──────────────────────────────────────────────
        // DebugCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DebugCmd_ExecutesAndReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Debug テストメッセージ", 5);
            var result = cmd.Exec();
            // NextID = ID + 1 = 6
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void DebugCmd_WithMultipleArgs_ExecutesSuccessfully()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Debug arg1 arg2 arg3", 3);
            var result = cmd.Exec();
            Assert.AreEqual(4, result);
        }

        // ──────────────────────────────────────────────
        // GlobalCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GlobalCmd_DefinesGlobalVariable()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Global myVar");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("myVar"));
        }

        [TestMethod]
        public void GlobalCmd_WithInitialValue_SetsValue()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Global score = 100");
            cmd.Exec();
            Assert.IsTrue(src.Expression.IsGlobalVariableDefined("score"));
        }

        // ──────────────────────────────────────────────
        // RenameTermCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RenameTermCmd_RenameTerm_StoresInGlobalVariable()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RenameTerm HP ヒットポイント");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.AreEqual("ヒットポイント", src.Expression.GetValueAsString("ShortTerm(HP)"));
        }

        [TestMethod]
        public void RenameTermCmd_RenameEN_StoresInGlobalVariable()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RenameTerm EN エナジー");
            cmd.Exec();
            Assert.AreEqual("エナジー", src.Expression.GetValueAsString("ShortTerm(EN)"));
        }

        [TestMethod]
        public void RenameTermCmd_RenameCustomTerm_StoresInTermVariable()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RenameTerm ユニット 機体");
            cmd.Exec();
            Assert.AreEqual("機体", src.Expression.GetValueAsString("Term(ユニット)"));
        }

        [TestMethod]
        public void RenameTermCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RenameTerm HP");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void RenameTermCmd_SP_StoresInShortTerm()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RenameTerm SP スピリット");
            cmd.Exec();
            Assert.AreEqual("スピリット", src.Expression.GetValueAsString("ShortTerm(SP)"));
        }

        [TestMethod]
        public void RenameTermCmd_CT_StoresInShortTerm()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RenameTerm CT クリティカル");
            cmd.Exec();
            Assert.AreEqual("クリティカル", src.Expression.GetValueAsString("ShortTerm(CT)"));
        }

        // ──────────────────────────────────────────────
        // FreeMemoryCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FreeMemoryCmd_ExecutesWithoutError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FreeMemory", 0);
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void FreeMemoryCmd_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "FreeMemory", 5);
            var result = cmd.Exec();
            Assert.AreEqual(6, result);
        }

        // ──────────────────────────────────────────────
        // WaitCmd - 引数エラーのテスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WaitCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Wait");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void WaitCmd_StartSubcommand_SetsWaitStartTime()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Wait Start");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void WaitCmd_ResetSubcommand_ResetsWaitState()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Wait Reset");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.AreEqual(-1, src.Event.WaitStartTime);
        }

        // ──────────────────────────────────────────────
        // MoneyCmd - 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MoneyCmd_ZeroAmount_NoChange()
        {
            var src = CreateSrc();
            src.Money = 500;
            var cmd = CreateCmd(src, "Money 0");
            cmd.Exec();
            Assert.AreEqual(500, src.Money);
        }

        [TestMethod]
        public void MoneyCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Money");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ForgetCmd
        // ヘルプ: 指定した作品のデータをプレイ再開時にロードしないようにします
        // 書式: Forget title
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ForgetCmd_RemovesTitleFromList()
        {
            // ヘルプ: Forget コマンドを使用した時点ではデータの削除は行われない
            // Titles リストから削除される
            var src = CreateSrc();
            src.Titles = new System.Collections.Generic.List<string> { "シン・サーガ", "他の作品" };
            var cmd = CreateCmd(src, "Forget シン・サーガ");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.IsFalse(src.Titles.Contains("シン・サーガ"));
            Assert.IsTrue(src.Titles.Contains("他の作品"));
        }

        [TestMethod]
        public void ForgetCmd_NonExistentTitle_NoError()
        {
            // 存在しないタイトルを Forget しても問題なし（削除しないだけ）
            var src = CreateSrc();
            src.Titles = new System.Collections.Generic.List<string> { "既存の作品" };
            var cmd = CreateCmd(src, "Forget 存在しない作品");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ForgetCmd_WrongArgCount_ReturnsError()
        {
            // 書式: Forget title (引数1個必須)
            var src = CreateSrc();
            src.Titles = new System.Collections.Generic.List<string>();
            var cmd = CreateCmd(src, "Forget");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }
    }
}
