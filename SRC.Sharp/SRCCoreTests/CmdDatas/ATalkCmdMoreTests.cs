using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// ATalkCmd（TalkCmd / AutoTalkCmd の抽象基底クラス）に対する追加ユニットテスト
    /// ヘルプ: AutoTalkコマンド.md の記載に基づく
    ///
    /// TalkAndAskCmdTests.cs に未収録のケース:
    /// - Talk と AutoTalk が混在する場合のエラー
    /// - End / Suspend の引数過多エラー
    /// - 枠外オプション (GUI.MessageWindowIsOut の設定)
    /// </summary>
    [TestClass]
    public class ATalkCmdMoreTests
    {
        private (SRC src, MockGUI gui) CreateSrc()
        {
            var gui = new MockGUI();
            var src = new SRC { GUI = gui };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new Queue<string>();
            return (src, gui);
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
        // Talk / AutoTalk 混在エラー
        // ヘルプ: AutoTalk は TalkCmd と同じ使用法（同一ブロック内での混在は不可）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TalkCmd_ContainsAutoTalk_ReturnsError()
        {
            // Talk ブロック内に AutoTalk が出現した場合はエラーを返す
            // (ATalkCmd.ProcessTalk: "Talk中またはAutoTalk中に他方のコマンドが実行されました")
            var (src, gui) = CreateSrc();
            gui.OpenMessageFormHandler = (u1, u2) => { };

            // Talk
            //   AutoTalk  ← Talk 中に AutoTalk → エラー
            //   End
            var cmds = BuildEvent(src,
                "Talk",         // ID=0: TalkCmd (Name = TalkCmd)
                "AutoTalk",     // ID=1: AutoTalkCmd ← Name != TalkCmd → エラー
                "End"           // ID=2
            );

            var result = cmds[0].Exec();

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void AutoTalkCmd_ContainsTalk_ReturnsError()
        {
            // AutoTalk ブロック内に Talk が出現した場合はエラーを返す
            var (src, gui) = CreateSrc();
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayBattleMessageHandler = (p, m, o) => { };

            // AutoTalk
            //   Talk  ← AutoTalk 中に Talk → エラー
            //   End
            var cmds = BuildEvent(src,
                "AutoTalk",     // ID=0: AutoTalkCmd (Name = AutoTalkCmd)
                "Talk",         // ID=1: TalkCmd ← Name != AutoTalkCmd → エラー
                "End"           // ID=2
            );

            var result = cmds[0].Exec();

            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // End 部分の引数過多エラー
        // ATalkCmd.ProcessTalk: "End部分の引数の数が違います"
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TalkCmd_EndWithExtraArg_ReturnsError()
        {
            // End に余分な引数が付いている場合はエラー
            var (src, gui) = CreateSrc();
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.CloseMessageFormHandler = () => { };

            // Talk
            //   End extra  ← ArgNum = 2 ≠ 1 → エラー
            var cmds = BuildEvent(src,
                "Talk",         // ID=0
                "End extra"     // ID=1: EndCmd with extra arg → error
            );

            var result = cmds[0].Exec();

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void AutoTalkCmd_EndWithExtraArg_ReturnsError()
        {
            // AutoTalk でも End に余分な引数があればエラー
            var (src, gui) = CreateSrc();
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "AutoTalk",     // ID=0
                "End extra"     // ID=1: error
            );

            var result = cmds[0].Exec();

            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // Suspend 部分の引数過多エラー
        // ATalkCmd.ProcessTalk: "Suspend部分の引数の数が違います"
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TalkCmd_SuspendWithExtraArg_ReturnsError()
        {
            // Suspend に余分な引数がある場合はエラー
            var (src, gui) = CreateSrc();
            gui.OpenMessageFormHandler = (u1, u2) => { };

            // Talk
            //   Suspend extra  ← ArgNum = 2 ≠ 1 → エラー
            var cmds = BuildEvent(src,
                "Talk",             // ID=0
                "Suspend extra"     // ID=1: SuspendCmd with extra arg → error
            );

            var result = cmds[0].Exec();

            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void AutoTalkCmd_SuspendWithExtraArg_ReturnsError()
        {
            // AutoTalk でも Suspend に余分な引数があればエラー
            var (src, gui) = CreateSrc();
            gui.OpenMessageFormHandler = (u1, u2) => { };

            var cmds = BuildEvent(src,
                "AutoTalk",         // ID=0
                "Suspend extra"     // ID=1: error
            );

            var result = cmds[0].Exec();

            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // 枠外オプション: GUI.MessageWindowIsOut が設定されること
        // ATalkCmd.ProcessTalk: case "枠外": GUI.MessageWindowIsOut = true
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TalkCmd_WakuGaiOption_SetsMessageWindowIsOut()
        {
            // ヘルプ (AutoTalkコマンド.md / Talkコマンド.md):
            // option 枠外 → メッセージウィンドウを枠外に表示
            // Talk システム 枠外 → GUI.MessageWindowIsOut = true が設定される
            var (src, gui) = CreateSrc();
            gui.OpenMessageFormHandler = (u1, u2) => { };

            gui.MessageWindowIsOut = false;

            // Talk システム 枠外 → processes talk with 枠外 option → sets MessageWindowIsOut
            // Suspend で終了 (Suspend は MessageWindowIsOut をリセットしない)
            var cmds = BuildEvent(src,
                "Talk システム 枠外",  // ID=0: 枠外 option
                "Suspend"              // ID=1: End ではなく Suspend で終了
            );

            cmds[0].Exec();

            Assert.IsTrue(gui.MessageWindowIsOut);
        }

        [TestMethod]
        public void AutoTalkCmd_WakuGaiOption_SetsMessageWindowIsOut()
        {
            // AutoTalk でも 枠外 オプションが機能すること
            var (src, gui) = CreateSrc();
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayBattleMessageHandler = (p, m, o) => { };

            gui.MessageWindowIsOut = false;

            var cmds = BuildEvent(src,
                "AutoTalk システム 枠外",  // ID=0: 枠外 option
                "Suspend"                  // ID=1
            );

            cmds[0].Exec();

            Assert.IsTrue(gui.MessageWindowIsOut);
        }

        [TestMethod]
        public void TalkCmd_EndResetsMessageWindowIsOut()
        {
            // End コマンドは GUI.MessageWindowIsOut を false にリセットする
            // ATalkCmd.ProcessTalk: case CmdType.EndCmd: GUI.MessageWindowIsOut = false;
            var (src, gui) = CreateSrc();
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.CloseMessageFormHandler = () => { };

            gui.MessageWindowIsOut = true;

            var cmds = BuildEvent(src,
                "Talk",     // ID=0
                "End"       // ID=1: resets MessageWindowIsOut
            );

            cmds[0].Exec();

            Assert.IsFalse(gui.MessageWindowIsOut);
        }

        // ──────────────────────────────────────────────
        // Talk — TalkとEnd間にメッセージがない場合の正常動作
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TalkCmd_EmptyBlock_ReturnsAfterEnd()
        {
            // Talk 行とすぐに End が来る場合 (メッセージなし) → 正常に End 後の ID を返す
            var (src, gui) = CreateSrc();
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk",     // ID=0
                "End",      // ID=1
                "Set x 1"   // ID=2
            );

            var result = cmds[0].Exec();

            // End は ID=1 なので return i+1 = 2
            Assert.AreEqual(2, result);
        }
    }
}
