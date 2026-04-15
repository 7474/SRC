using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// ABGMCmd (抽象 BGM コマンド) の連続コマンドバッチ処理のユニットテスト。
    ///
    /// ABGMCmd の主要機能:
    ///   同一コマンド (PlayMIDI / StartBGM) が連続して並んでいる場合、後ろのコマンドほど
    ///   優先度が高い。最後に見つかった MIDI ファイルを再生し、最後の連続コマンドの
    ///   次の位置 (play_bgm_end + 1) に実行位置を進める。
    ///
    /// ヘルプ (PlayMIDIコマンド.md):
    ///   「後ろの PlayMIDI コマンドに指定されたファイルほど優先度が高くなります。」
    ///   「最初のファイルから検索が行われ、最初に見つかったファイルが再生されます。」
    ///
    /// この動作は ABGMCmd の ExecInternal() で実装されており、
    /// 各具象サブクラス (PlayMIDICmd, StartBGMCmd) の単体テストでは確認されていない。
    /// </summary>
    [TestClass]
    public class ABGMBatchTests
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
        // PlayMIDI 連続バッチ処理
        // ヘルプ: 連続した PlayMIDI コマンドは後ろほど優先度が高く、
        //         最後の PlayMIDI コマンドの次の位置に実行を進める。
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PlayMIDICmd_TwoConsecutive_ReturnsIdAfterLastCmd()
        {
            // 連続する2つの PlayMIDI コマンドのうち最初のものを実行すると
            // 実行位置は2番目のコマンドの次 (ID=2) に進む
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "PlayMIDI x",   // ID=0: 短い名前 → SearchMidiFile が null を返す
                "PlayMIDI y");  // ID=1: 同上

            var result = cmds[0].Exec();
            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void PlayMIDICmd_ThreeConsecutive_ReturnsIdAfterLastCmd()
        {
            // 連続する3つの PlayMIDI コマンドを実行すると
            // 実行位置は最後のコマンドの次 (ID=3) に進む
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "PlayMIDI x",
                "PlayMIDI y",
                "PlayMIDI z");

            var result = cmds[0].Exec();
            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void PlayMIDICmd_SingleCmd_ReturnsNextId()
        {
            // 単独の PlayMIDI コマンドは次のコマンド位置 (ID=1) を返す
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "PlayMIDI x",
                "NOP");

            var result = cmds[0].Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void PlayMIDICmd_TwoConsecutive_SetsKeepBGMFalse()
        {
            // 連続 PlayMIDI でも KeepBGM フラグはリセットされる
            var src = CreateSrc();
            src.Sound.KeepBGM = true;
            var cmds = BuildEvent(src,
                "PlayMIDI x",
                "PlayMIDI y");

            cmds[0].Exec();
            Assert.IsFalse(src.Sound.KeepBGM);
        }

        [TestMethod]
        public void PlayMIDICmd_TwoConsecutive_SetsBossBGMFalse()
        {
            // 連続 PlayMIDI でも BossBGM フラグはリセットされる
            var src = CreateSrc();
            src.Sound.BossBGM = true;
            var cmds = BuildEvent(src,
                "PlayMIDI x",
                "PlayMIDI y");

            cmds[0].Exec();
            Assert.IsFalse(src.Sound.BossBGM);
        }

        // ──────────────────────────────────────────────
        // StartBGM 連続バッチ処理
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartBGMCmd_TwoConsecutive_ReturnsIdAfterLastCmd()
        {
            // 連続する2つの StartBGM コマンドを実行すると
            // 実行位置は最後のコマンドの次 (ID=2) に進む
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "StartBGM x",
                "StartBGM y");

            var result = cmds[0].Exec();
            Assert.AreEqual(2, result);
        }

        // ──────────────────────────────────────────────
        // バッチの境界: 異なるコマンドが挟まれると連続扱いしない
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PlayMIDICmd_FollowedByStartBGM_OnlyCountsSameNameCmd()
        {
            // PlayMIDI の次が StartBGM の場合は PlayMIDI 単独のバッチ
            // → 実行位置は StartBGM の前 (ID=1) に進む
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "PlayMIDI x",   // ID=0
                "StartBGM y",   // ID=1: 名前が異なるのでバッチに含まれない
                "NOP");         // ID=2

            var result = cmds[0].Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void PlayMIDICmd_WithNopBetween_OnlyCountsFirstPlayMIDI()
        {
            // PlayMIDI → NOP → PlayMIDI の場合、2番目の PlayMIDI はバッチに含まれない
            // 最初の PlayMIDI 単独のバッチ → 実行位置は NOP の位置 (ID=1) に進む
            var src = CreateSrc();
            var cmds = BuildEvent(src,
                "PlayMIDI x",   // ID=0
                "NOP",          // ID=1: 連続を中断
                "PlayMIDI z");  // ID=2: 別のバッチ

            var result = cmds[0].Exec();
            Assert.AreEqual(1, result);
        }

        // ──────────────────────────────────────────────
        // KeepBGM / BossBGM フラグのリセット確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartBGMCmd_TwoConsecutive_SetsKeepBGMFalse()
        {
            var src = CreateSrc();
            src.Sound.KeepBGM = true;
            var cmds = BuildEvent(src,
                "StartBGM x",
                "StartBGM y");

            cmds[0].Exec();
            Assert.IsFalse(src.Sound.KeepBGM);
        }
    }
}
