using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// ATalkCmd (TalkCmd / AutoTalkCmd) のオプション引数に関するユニットテスト。
    /// TalkAndAskCmdTests.cs でカバーされていない Talk 行のオプション動作を検証する。
    /// </summary>
    [TestClass]
    public class ATalkCmdOptionTests
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
        // 枠外オプション (MessageWindowIsOut)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TalkCmd_WithOptionWakugai_MessageWindowIsOutTrueDuringDisplay()
        {
            // ATalkCmd.ProcessTalk: "枠外" オプション指定で GUI.MessageWindowIsOut = true になる
            var (src, gui) = CreateSrc();
            bool wasOutDuringMessage = false;
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (pname, msg, mode) =>
            {
                wasOutDuringMessage = gui.MessageWindowIsOut;
            };
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk システム 枠外",   // ID=0: 枠外オプション
                "テストメッセージ",      // ID=1: メッセージ本文
                "End"                   // ID=2
            );

            cmds[0].Exec();

            Assert.IsTrue(wasOutDuringMessage, "枠外オプション指定中は GUI.MessageWindowIsOut=true になるべき");
        }

        [TestMethod]
        public void TalkCmd_WithoutOptionWakugai_MessageWindowIsOutFalseDuringDisplay()
        {
            // 枠外オプションなしの場合、MessageWindowIsOut はデフォルトの false のまま
            var (src, gui) = CreateSrc();
            bool wasOutDuringMessage = false;
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (pname, msg, mode) =>
            {
                wasOutDuringMessage = gui.MessageWindowIsOut;
            };
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk システム",
                "テストメッセージ",
                "End"
            );

            cmds[0].Exec();

            Assert.IsFalse(wasOutDuringMessage, "枠外オプションなし時は MessageWindowIsOut=false のまま");
        }

        [TestMethod]
        public void TalkCmd_AfterEnd_MessageWindowIsOutResetToFalse()
        {
            // End コマンドで Talk ブロックを閉じた後、MessageWindowIsOut は false にリセットされる
            var (src, gui) = CreateSrc();
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (p, m, mo) => { };
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk システム 枠外",
                "メッセージ",
                "End"
            );

            cmds[0].Exec();

            Assert.IsFalse(gui.MessageWindowIsOut, "End後は GUI.MessageWindowIsOut=false にリセットされるべき");
        }

        // ──────────────────────────────────────────────
        // 画像表示オプション (DisplayMessage の mode 引数に渡る)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TalkCmd_WithMonochromeOption_ModePassedToDisplayMessage()
        {
            // "白黒" 画像表示オプションが DisplayMessage の mode に渡される
            // ATalkCmd.ProcessTalk: j > 2 のオプションは options 文字列として DisplayMessage に渡される
            var (src, gui) = CreateSrc();
            string capturedMode = null;
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (pname, msg, mode) => capturedMode = mode;
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk システム 白黒",   // ID=0: options="白黒 "
                "テストメッセージ",      // ID=1
                "End"                   // ID=2
            );

            cmds[0].Exec();

            Assert.IsNotNull(capturedMode, "mode はnullでないはず");
            StringAssert.Contains(capturedMode, "白黒");
        }

        [TestMethod]
        public void TalkCmd_WithSepiaOption_ModePassedToDisplayMessage()
        {
            // "セピア" 画像表示オプションが DisplayMessage の mode に渡される
            var (src, gui) = CreateSrc();
            string capturedMode = null;
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (pname, msg, mode) => capturedMode = mode;
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk システム セピア",
                "テストメッセージ",
                "End"
            );

            cmds[0].Exec();

            StringAssert.Contains(capturedMode ?? "", "セピア");
        }

        [TestMethod]
        public void TalkCmd_WithSilhouetteOption_ModePassedToDisplayMessage()
        {
            // "シルエット" オプションが DisplayMessage の mode に渡される
            var (src, gui) = CreateSrc();
            string capturedMode = null;
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (pname, msg, mode) => capturedMode = mode;
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk システム シルエット",
                "テストメッセージ",
                "End"
            );

            cmds[0].Exec();

            StringAssert.Contains(capturedMode ?? "", "シルエット");
        }

        [TestMethod]
        public void TalkCmd_WithDarkOption_ModePassedToDisplayMessage()
        {
            // "暗" オプションが DisplayMessage の mode に渡される
            var (src, gui) = CreateSrc();
            string capturedMode = null;
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (pname, msg, mode) => capturedMode = mode;
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk システム 暗",
                "テストメッセージ",
                "End"
            );

            cmds[0].Exec();

            StringAssert.Contains(capturedMode ?? "", "暗");
        }

        [TestMethod]
        public void TalkCmd_WithNegaPozaOption_ModePassedToDisplayMessage()
        {
            // "ネガポジ反転" オプションが DisplayMessage の mode に渡される
            var (src, gui) = CreateSrc();
            string capturedMode = null;
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (pname, msg, mode) => capturedMode = mode;
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk システム ネガポジ反転",
                "テストメッセージ",
                "End"
            );

            cmds[0].Exec();

            StringAssert.Contains(capturedMode ?? "", "ネガポジ反転");
        }

        [TestMethod]
        public void TalkCmd_WithRotationOption_RotationAndAnglePassedToMode()
        {
            // "右回転 角度" オプションが "右回転 角度 " として mode に渡される
            var (src, gui) = CreateSrc();
            string capturedMode = null;
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (pname, msg, mode) => capturedMode = mode;
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk システム 右回転 45",   // ID=0: 右回転+角度
                "テストメッセージ",           // ID=1
                "End"                        // ID=2
            );

            cmds[0].Exec();

            StringAssert.Contains(capturedMode ?? "", "右回転");
            StringAssert.Contains(capturedMode ?? "", "45");
        }

        [TestMethod]
        public void TalkCmd_WithLeftRotationOption_RotationAndAnglePassedToMode()
        {
            // "左回転 角度" オプションが mode に渡される
            var (src, gui) = CreateSrc();
            string capturedMode = null;
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (pname, msg, mode) => capturedMode = mode;
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk システム 左回転 90",
                "テストメッセージ",
                "End"
            );

            cmds[0].Exec();

            StringAssert.Contains(capturedMode ?? "", "左回転");
            StringAssert.Contains(capturedMode ?? "", "90");
        }

        [TestMethod]
        public void TalkCmd_WithMultipleImageOptions_AllPassedToDisplayMessage()
        {
            // 複数の画像表示オプションはすべて mode に含まれる
            var (src, gui) = CreateSrc();
            string capturedMode = null;
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (pname, msg, mode) => capturedMode = mode;
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk システム 白黒 セピア",   // 複数オプション
                "テストメッセージ",
                "End"
            );

            cmds[0].Exec();

            StringAssert.Contains(capturedMode ?? "", "白黒");
            StringAssert.Contains(capturedMode ?? "", "セピア");
        }

        [TestMethod]
        public void TalkCmd_OptionsResetBetweenTalkLines()
        {
            // 次の Talk 行では前の行のオプションがリセットされる
            var (src, gui) = CreateSrc();
            var capturedModes = new List<string>();
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (pname, msg, mode) => capturedModes.Add(mode);
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk システム 白黒",   // ID=0: 白黒オプション付き
                "一行目",               // ID=1: message → mode = "白黒 "
                "Talk システム",        // ID=2: オプションなし (options リセット)
                "二行目",               // ID=3: message → mode = ""
                "End"                   // ID=4
            );

            cmds[0].Exec();

            Assert.AreEqual(2, capturedModes.Count, "2つのメッセージが表示されるはず");
            StringAssert.Contains(capturedModes[0], "白黒");
            // 2行目のオプションは空か白黒を含まない
            Assert.IsFalse(
                capturedModes[1].Contains("白黒"),
                $"2行目では白黒オプションはリセットされるはず (actual: '{capturedModes[1]}')"
            );
        }

        // ──────────────────────────────────────────────
        // AutoTalkCmd のオプション
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AutoTalkCmd_WithOptionWakugai_MessageWindowIsOutTrueDuringDisplay()
        {
            // AutoTalkCmd でも "枠外" オプションが正しく機能する
            var (src, gui) = CreateSrc();
            bool wasOutDuringMessage = false;
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayBattleMessageHandler = (pname, msg, mode) =>
            {
                wasOutDuringMessage = gui.MessageWindowIsOut;
            };
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "AutoTalk システム 枠外",
                "テストメッセージ",
                "End"
            );

            cmds[0].Exec();

            Assert.IsTrue(wasOutDuringMessage, "AutoTalkCmd でも 枠外オプションが機能するべき");
        }

        [TestMethod]
        public void AutoTalkCmd_WithMonochromeOption_ModePassedToDisplayBattleMessage()
        {
            // "白黒" オプションが AutoTalkCmd の DisplayBattleMessage の mode に渡される
            var (src, gui) = CreateSrc();
            string capturedMode = null;
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayBattleMessageHandler = (pname, msg, mode) => capturedMode = mode;
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "AutoTalk システム 白黒",
                "テストメッセージ",
                "End"
            );

            cmds[0].Exec();

            StringAssert.Contains(capturedMode ?? "", "白黒");
        }

        // ──────────────────────────────────────────────
        // Talk行の画面位置指定オプション
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TalkCmd_WithMirrorVerticalOption_ModePassedToDisplayMessage()
        {
            // "上下反転" オプションが mode に渡される
            var (src, gui) = CreateSrc();
            string capturedMode = null;
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (pname, msg, mode) => capturedMode = mode;
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk システム 上下反転",
                "テストメッセージ",
                "End"
            );

            cmds[0].Exec();

            StringAssert.Contains(capturedMode ?? "", "上下反転");
        }

        [TestMethod]
        public void TalkCmd_WithMirrorHorizontalOption_ModePassedToDisplayMessage()
        {
            // "左右反転" オプションが mode に渡される
            var (src, gui) = CreateSrc();
            string capturedMode = null;
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (pname, msg, mode) => capturedMode = mode;
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk システム 左右反転",
                "テストメッセージ",
                "End"
            );

            cmds[0].Exec();

            StringAssert.Contains(capturedMode ?? "", "左右反転");
        }

        [TestMethod]
        public void TalkCmd_WithNormalOption_ModePassedToDisplayMessage()
        {
            // "通常" オプションが mode に渡される (他のオプションをリセット)
            var (src, gui) = CreateSrc();
            string capturedMode = null;
            gui.OpenMessageFormHandler = (u1, u2) => { };
            gui.DisplayMessageHandler = (pname, msg, mode) => capturedMode = mode;
            gui.CloseMessageFormHandler = () => { };

            var cmds = BuildEvent(src,
                "Talk システム 通常",
                "テストメッセージ",
                "End"
            );

            cmds[0].Exec();

            StringAssert.Contains(capturedMode ?? "", "通常");
        }
    }
}
