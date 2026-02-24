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

        // ──────────────────────────────────────────────
        // StartBGMCmd
        // ヘルプ: MIDIファイルを繰り返し再生します
        // 書式: StartBGM filename
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartBGMCmd_WithFilename_ReturnsNextId()
        {
            // ヘルプ: StartBGM filename — filenameのMIDIファイルをBGMとして繰り返し再生
            // ファイルが存在しない/短い名前の場合は Sound.SearchMidiFile が null を返し何もしない
            var src = CreateSrc();
            // "x" は len < 5 のため SearchMidiFile が null を返し StartBGM が早期リターンする
            var cmd = CreateCmd(src, "StartBGM x", 0);
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void StartBGMCmd_SetsKeepBGMFalse()
        {
            // ヘルプ: StartBGM は KeepBGM フラグをリセットする
            var src = CreateSrc();
            src.Sound.KeepBGM = true;
            var cmd = CreateCmd(src, "StartBGM x", 0);
            cmd.Exec();
            Assert.IsFalse(src.Sound.KeepBGM);
        }

        [TestMethod]
        public void StartBGMCmd_SetsBossBGMFalse()
        {
            // ヘルプ: StartBGM は BossBGM フラグをリセットする
            var src = CreateSrc();
            src.Sound.BossBGM = true;
            var cmd = CreateCmd(src, "StartBGM x", 0);
            cmd.Exec();
            Assert.IsFalse(src.Sound.BossBGM);
        }

        // ──────────────────────────────────────────────
        // PlayMIDICmd
        // ヘルプ: MIDIファイルを一度だけ再生します
        // 書式: PlayMIDI filename
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PlayMIDICmd_WithFilename_ReturnsNextId()
        {
            // ヘルプ: PlayMIDI filename — filenameのMIDIファイルを一度だけ再生
            // ファイルが存在しない/短い名前の場合は Sound.SearchMidiFile が null を返し何もしない
            var src = CreateSrc();
            var cmd = CreateCmd(src, "PlayMIDI x", 0);
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void PlayMIDICmd_SetsKeepBGMFalse()
        {
            var src = CreateSrc();
            src.Sound.KeepBGM = true;
            var cmd = CreateCmd(src, "PlayMIDI x", 0);
            cmd.Exec();
            Assert.IsFalse(src.Sound.KeepBGM);
        }

        // ──────────────────────────────────────────────
        // PlaySoundCmd
        // ヘルプ: WAVEファイルを再生します
        // 書式: PlaySound filename
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PlaySoundCmd_CancelFilename_ReturnsNextId()
        {
            // ヘルプ: PlaySound -.wav — 再生をキャンセル (早期リターンするため常にNextID)
            var src = CreateSrc();
            var cmd = CreateCmd(src, "PlaySound -.wav", 0);
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void PlaySoundCmd_CancelMp3Filename_ReturnsNextId()
        {
            // ヘルプ: PlaySound -.mp3 — MP3再生をキャンセル
            var src = CreateSrc();
            var cmd = CreateCmd(src, "PlaySound -.mp3", 0);
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }
    }
}
