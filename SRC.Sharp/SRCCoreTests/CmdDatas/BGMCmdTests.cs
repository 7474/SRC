using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// KeepBGMCmd / StopBGMCmd コマンドのユニットテスト
    /// </summary>
    [TestClass]
    public class BGMCmdTests
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
        // KeepBGMCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void KeepBGMCmd_SetsKeepBGMTrue()
        {
            var src = CreateSrc();
            src.Sound.KeepBGM = false;
            var cmd = CreateCmd(src, "KeepBGM");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.IsTrue(src.Sound.KeepBGM);
        }

        [TestMethod]
        public void KeepBGMCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "KeepBGM extra");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void KeepBGMCmd_AlreadyTrue_StaysTrue()
        {
            var src = CreateSrc();
            src.Sound.KeepBGM = true;
            var cmd = CreateCmd(src, "KeepBGM");
            cmd.Exec();
            Assert.IsTrue(src.Sound.KeepBGM);
        }

        // ──────────────────────────────────────────────
        // StopBGMCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StopBGMCmd_SetsKeepBGMFalse()
        {
            var src = CreateSrc();
            src.Sound.KeepBGM = true;
            var cmd = CreateCmd(src, "StopBGM");
            cmd.Exec();
            // Sound.KeepBGM は StopBGM 呼び出し前に false に設定される
            Assert.IsFalse(src.Sound.KeepBGM);
        }

        [TestMethod]
        public void StopBGMCmd_SetsBossBGMFalse()
        {
            var src = CreateSrc();
            src.Sound.BossBGM = true;
            var cmd = CreateCmd(src, "StopBGM");
            cmd.Exec();
            // Sound.BossBGM は StopBGM 呼び出し前に false に設定される
            Assert.IsFalse(src.Sound.BossBGM);
        }

        [TestMethod]
        public void StopBGMCmd_BGMFileNameIsCleared()
        {
            var src = CreateSrc();
            // Player が null の場合: StopBGM の finally ブロックで BGMFileName がクリアされる
            src.Sound.BGMFileName = "";
            var cmd = CreateCmd(src, "StopBGM");
            cmd.Exec();
            Assert.AreEqual("", src.Sound.BGMFileName);
        }
    }
}
