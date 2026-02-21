using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// Cancel/End/Suspend コマンドのユニットテスト
    /// ヘルプの記載に基づく期待値を検証する
    /// </summary>
    [TestClass]
    public class EventCmdMoreTests
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
        // CancelCmd
        // ヘルプ: イベント後のユニットの行動やイベント処理をキャンセルします
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CancelCmd_SetsIsCanceledToTrue()
        {
            // ヘルプ: イベント後の行動やイベント処理をキャンセル
            var src = CreateSrc();
            Assert.IsFalse(src.IsCanceled);

            var cmd = CreateCmd(src, "Cancel");
            cmd.Exec();

            Assert.IsTrue(src.IsCanceled);
        }

        [TestMethod]
        public void CancelCmd_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Cancel", 3);
            var result = cmd.Exec();
            Assert.AreEqual(4, result);
        }

        [TestMethod]
        public void CancelCmd_AlreadyCanceled_RemainsTrue()
        {
            // 二度実行しても問題なし
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Cancel");
            cmd.Exec();
            cmd.Exec();
            Assert.IsTrue(src.IsCanceled);
        }

        // ──────────────────────────────────────────────
        // EndCmd
        // ヘルプ: Talk...End でメッセージウィンドウを閉じる
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EndCmd_ReturnsNextId()
        {
            // End は TalkCmd 処理の終端として機能する（単体でも NextID を返す）
            var src = CreateSrc();
            var cmd = CreateCmd(src, "End", 5);
            var result = cmd.Exec();
            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void EndCmd_IsCorrectType()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "End");
            Assert.IsInstanceOfType(cmd, typeof(EndCmd));
        }

        // ──────────────────────────────────────────────
        // SuspendCmd
        // ヘルプ: Talk 中のメッセージを一時停止する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SuspendCmd_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Suspend", 2);
            var result = cmd.Exec();
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void SuspendCmd_IsCorrectType()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Suspend");
            Assert.IsInstanceOfType(cmd, typeof(SuspendCmd));
        }
    }
}
