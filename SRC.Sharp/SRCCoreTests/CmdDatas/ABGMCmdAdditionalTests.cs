using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// ABGMCmd（PlayMIDICmd / StartBGMCmd の抽象基底）に対するユニットテスト
    /// ヘルプ: StartBGMコマンド.md / PlayMIDIコマンド.md の記載に基づく
    ///
    /// 抽象基底クラス ABGMCmd の主要機能:
    /// 「PlayMIDI/StartBGM コマンドが連続している場合、最後のコマンドの位置を検索し、
    ///   後ろのコマンドほど優先度が高くなる」（ヘルプ: 複数のファイル指定時の動作）
    /// </summary>
    [TestClass]
    public class ABGMCmdAdditionalTests
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

        private CmdData[] BuildEvent(SRC src, params string[] lines)
        {
            var parser = new CmdParser();
            var cmds = new CmdData[lines.Length];
            for (var i = 0; i < lines.Length; i++)
            {
                var line = new EventDataLine(i, EventDataSource.Scenario, "test", i, lines[i]);
                src.Event.EventData.Add(line);
                var cmd = parser.Parse(src, line);
                src.Event.EventCmd.Add(cmd);
                cmds[i] = cmd;
            }
            return cmds;
        }

        // ──────────────────────────────────────────────
        // ABGMCmd — 連続コマンドの折り畳み処理
        // ヘルプ: 「後ろの StartBGM コマンドに指定されたファイルほど優先度が高くなります」
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartBGMCmd_ConsecutiveTwoCommands_ReturnsAfterLast()
        {
            // ヘルプ: 連続した StartBGM コマンドは最後のコマンドの位置まで読み進める
            // StartBGM x   ← ID=0
            // StartBGM y   ← ID=1
            // Set result 1 ← ID=2 (別コマンド)
            // ID=0 実行 → playCmds=[cmd0,cmd1], play_bgm_end=1 → return 2 (=play_bgm_end+1)
            // 注: "x", "y" は len < 5 のため SearchMidiFile が null を返し StartBGM が早期リターンする
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "StartBGM x",   // ID=0
                "StartBGM y",   // ID=1
                "Set result 1"  // ID=2
            );

            var result = cmds[0].Exec();

            // 2 連続 StartBGM → 最後の ID=1 の次 = 2 を返す
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void StartBGMCmd_ConsecutiveThreeCommands_ReturnsAfterLast()
        {
            // ヘルプ: 複数の StartBGM を列挙した場合、最後のコマンドの後に実行位置が進む
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "StartBGM a",   // ID=0
                "StartBGM b",   // ID=1
                "StartBGM c",   // ID=2
                "Set done 1"    // ID=3
            );

            var result = cmds[0].Exec();

            // 3 連続 StartBGM → 最後の ID=2 の次 = 3 を返す
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void StartBGMCmd_ConsecutiveCommands_ResetsKeepBGM()
        {
            // ヘルプ: StartBGM 実行時に KeepBGM フラグはリセットされる
            var src = CreateSrc();
            src.Sound.KeepBGM = true;
            var cmds = BuildEvent(src,
                "StartBGM f",   // ID=0
                "StartBGM g",   // ID=1
                "Set end 1"     // ID=2
            );

            cmds[0].Exec();

            Assert.IsFalse(src.Sound.KeepBGM);
        }

        [TestMethod]
        public void StartBGMCmd_ConsecutiveCommands_ResetsBossBGM()
        {
            // ヘルプ: StartBGM 実行時に BossBGM フラグもリセットされる
            var src = CreateSrc();
            src.Sound.BossBGM = true;
            var cmds = BuildEvent(src,
                "StartBGM h",   // ID=0
                "StartBGM i",   // ID=1
                "Set end 1"     // ID=2
            );

            cmds[0].Exec();

            Assert.IsFalse(src.Sound.BossBGM);
        }

        [TestMethod]
        public void PlayMIDICmd_ConsecutiveTwoCommands_ReturnsAfterLast()
        {
            // ヘルプ: PlayMIDI も連続している場合、最後のコマンドの後に実行位置が進む
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "PlayMIDI p",   // ID=0
                "PlayMIDI q",   // ID=1
                "Set end 1"     // ID=2
            );

            var result = cmds[0].Exec();

            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void PlayMIDICmd_ConsecutiveThreeCommands_ReturnsAfterLast()
        {
            // ヘルプ: PlayMIDI も複数列挙した場合、最後のコマンド位置の次を返す
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "PlayMIDI r",   // ID=0
                "PlayMIDI s",   // ID=1
                "PlayMIDI t",   // ID=2
                "Set end 1"     // ID=3
            );

            var result = cmds[0].Exec();

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void PlayMIDICmd_ConsecutiveCommands_ResetsKeepBGM()
        {
            // ヘルプ: PlayMIDI 実行時に KeepBGM フラグはリセットされる
            var src = CreateSrc();
            src.Sound.KeepBGM = true;
            var cmds = BuildEvent(src,
                "PlayMIDI u",   // ID=0
                "PlayMIDI v",   // ID=1
                "Set end 1"     // ID=2
            );

            cmds[0].Exec();

            Assert.IsFalse(src.Sound.KeepBGM);
        }

        [TestMethod]
        public void StartBGMCmd_SingleCommand_ReturnsNextId()
        {
            // 連続していない単独コマンド: 通常通り NextID (= ID+1) を返す
            // 注: "x" は len < 5 のため SearchMidiFile が null を返し早期リターンする
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "StartBGM x",   // ID=0
                "Set other 1"   // ID=1
            );

            var result = cmds[0].Exec();

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void StartBGMCmd_ConsecutiveGroupStartsAtNonZero_ReturnsCorrectly()
        {
            // 連続グループが ID=0 以外から始まる場合でも正しく動作する
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "Set dummy 0",  // ID=0
                "StartBGM w",   // ID=1
                "StartBGM z",   // ID=2
                "Set done 1"    // ID=3
            );

            // ID=1 から実行: playCmds=[cmd1,cmd2], play_bgm_end=2 → return 3
            var result = cmds[1].Exec();

            Assert.AreEqual(3, result);
        }
    }
}
