using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// ステージ関連コマンドの追加ユニットテスト
    /// (GameClear, GameOver の正常系, QuitCmd, MapAbilityCmd等)
    /// </summary>
    [TestClass]
    public class StageAdditionalCmdTests
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
        // GameClearCmd - 正常系
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GameClearCmd_NoArgs_CallsTerminate()
        {
            var src = CreateSrc();
            var mockGui = (MockGUI)src.GUI;
            var cmd = CreateCmd(src, "GameClear");
            cmd.Exec();
            // GameClear → TerminateSRC() → GUI.Terminate() が呼ばれることを検証
            Assert.IsTrue(mockGui.TerminateCalled);
        }

        [TestMethod]
        public void GameClearCmd_NoArgs_ReturnsMinusOne()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "GameClear");
            var result = cmd.Exec();
            // Terminate() が false を返すため Environment.Exit は呼ばれず、
            // GameClearCmd は -1 を返す（イベント終了）
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void GameClearCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "GameClear extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // QuitCmd - 正常系
        // ──────────────────────────────────────────────

        [TestMethod]
        public void QuitCmd_CallsTerminate()
        {
            var src = CreateSrc();
            var mockGui = (MockGUI)src.GUI;
            var cmd = CreateCmd(src, "Quit");
            cmd.Exec();
            // Quit → TerminateSRC() → GUI.Terminate() が呼ばれることを検証
            Assert.IsTrue(mockGui.TerminateCalled);
        }

        [TestMethod]
        public void QuitCmd_ReturnsMinusOne()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Quit");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // BreakCmd - ループ外では EventError
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BreakCmd_OutsideLoop_ReturnsError()
        {
            // ループの外で Break が使われると EventErrorException → -1 を返す
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Break");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // GotoCmd - 指定ラベルにジャンプ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GotoCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Goto");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ReturnCmd - プロシージャ終了
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReturnCmd_Returns_MinusOne()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Return");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }
    }
}
