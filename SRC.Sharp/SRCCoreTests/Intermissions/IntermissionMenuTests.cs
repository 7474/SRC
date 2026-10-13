using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Intermissions.Tests
{
    /// <summary>
    /// InterMission.InterMissionCommand のメニュー構成と選択分岐のテスト。
    /// </summary>
    [TestClass]
    public class IntermissionMenuTests
    {
        private sealed class MockGUIMap : IGUIMap
        {
            public void InitMapSize(int w, int h) { }
            public void SetMapSize(int w, int h) { }
        }

        private (SRC src, MockGUI gui) CreateSrc()
        {
            var gui = new MockGUI();
            var src = new SRC { GUI = gui, GUIMap = new MockGUIMap() };
            src.Event.EventData = new List<EventDataLine>
            {
                new EventDataLine(0, EventDataSource.System, "system", 0, ""),
            };
            src.Event.EventQue = new List<string>();
            gui.RedrawScreenHandler = _ => { };
            gui.EnlargeListBoxHeightHandler = () => { };
            gui.CloseListBoxHandler = () => { };
            gui.ReduceListBoxHeightHandler = () => { };
            gui.ConfirmHandler = (_, _, _) => GuiDialogResult.Ok;
            src.Expression.DefineGlobalVariable("BGM(Intermission)");
            src.Expression.SetVariableAsString("BGM(Intermission)", "");
            src.Expression.DefineGlobalVariable("次ステージ");
            src.Expression.SetVariableAsString("次ステージ", "next.eve");
            return (src, gui);
        }

        private static void SetOption(SRC src, string name)
        {
            src.Expression.DefineGlobalVariable("Option(" + name + ")");
        }

        // メニューを 1 回取得し、「次のステージへ」(先頭) を選んで終了する
        private static List<string> CaptureMenu(SRC src, MockGUI gui)
        {
            List<string> items = null;
            gui.ListBoxHandler = args =>
            {
                items = args.Items.Select(x => x.Text).ToList();
                return 1;
            };
            src.InterMission.InterMissionCommand(skip_update: true);
            return items;
        }

        [TestMethod]
        public void InterMissionCommand_DefaultMenu_ContainsStandardEntries()
        {
            var (src, gui) = CreateSrc();
            var items = CaptureMenu(src, gui);
            CollectionAssert.AreEqual(
                new[] { "次のステージへ", "データセーブ", "機体改造", "パイロットステータス", "ユニットステータス", "SRCを終了" },
                items);
        }

        [TestMethod]
        public void InterMissionCommand_NoNextStage_OmitsNextStageEntry()
        {
            var (src, gui) = CreateSrc();
            src.Expression.SetVariableAsString("次ステージ", "");
            List<string> items = null;
            gui.ListBoxHandler = args =>
            {
                items = args.Items.Select(x => x.Text).ToList();
                // SRCを終了 を選択 → 確認 OK → ExitGame
                return args.Items.Count;
            };
            try { src.InterMission.InterMissionCommand(skip_update: true); } catch { }

            Assert.IsFalse(items.Contains("次のステージへ"));
            Assert.AreEqual("データセーブ", items[0]);
        }

        [TestMethod]
        public void InterMissionCommand_SaveDisabled_OmitsSaveEntry()
        {
            var (src, gui) = CreateSrc();
            SetOption(src, "データセーブ不可");
            Assert.IsFalse(CaptureMenu(src, gui).Contains("データセーブ"));
        }

        [TestMethod]
        public void InterMissionCommand_SaveDisabledWithDebug_KeepsSaveEntry()
        {
            var (src, gui) = CreateSrc();
            SetOption(src, "データセーブ不可");
            SetOption(src, "デバッグ");
            Assert.IsTrue(CaptureMenu(src, gui).Contains("データセーブ"));
        }

        [TestMethod]
        public void InterMissionCommand_UpgradeDisabled_OmitsUpgradeEntry()
        {
            var (src, gui) = CreateSrc();
            SetOption(src, "改造不可");
            var items = CaptureMenu(src, gui);
            Assert.IsFalse(items.Contains("機体改造"));
            Assert.IsFalse(items.Contains("ユニットの強化"));
        }

        [TestMethod]
        public void InterMissionCommand_HumanScaleOption_UsesUnitEnhanceAndHidesPilotStatus()
        {
            var (src, gui) = CreateSrc();
            SetOption(src, "等身大基準");
            var items = CaptureMenu(src, gui);
            Assert.IsTrue(items.Contains("ユニットの強化"));
            Assert.IsFalse(items.Contains("機体改造"));
            Assert.IsFalse(items.Contains("パイロットステータス"));
        }

        [TestMethod]
        public void InterMissionCommand_ExchangeOptions_AddEntries()
        {
            var (src, gui) = CreateSrc();
            SetOption(src, "乗り換え");
            SetOption(src, "アイテム交換");
            var items = CaptureMenu(src, gui);
            Assert.IsTrue(items.Contains("乗り換え"));
            Assert.IsTrue(items.Contains("アイテム交換"));
        }

        [TestMethod]
        public void InterMissionCommand_NoExchangeOptions_OmitsEntries()
        {
            var (src, gui) = CreateSrc();
            var items = CaptureMenu(src, gui);
            Assert.IsFalse(items.Contains("乗り換え"));
            Assert.IsFalse(items.Contains("アイテム交換"));
            Assert.IsFalse(items.Contains("換装"));
        }

        [TestMethod]
        public void InterMissionCommand_UserDefinedCommand_AppearsInMenu()
        {
            var (src, gui) = CreateSrc();
            src.Expression.DefineGlobalVariable("IntermissionCommand(カスタム)");
            var items = CaptureMenu(src, gui);
            Assert.IsTrue(items.Contains("カスタム") || items.Any(x => x.Contains("カスタム")));
            Assert.AreEqual("SRCを終了", items.Last());
        }

        [TestMethod]
        public void InterMissionCommand_ListBoxCaptionAndMode_AreCommandContinuous()
        {
            var (src, gui) = CreateSrc();
            string caption = null, mode = null;
            gui.ListBoxHandler = args => { caption = args.lb_caption; mode = args.lb_mode; return 1; };
            src.InterMission.InterMissionCommand(skip_update: true);
            Assert.AreEqual("コマンド", caption);
            Assert.AreEqual("連続表示", mode);
        }

        [TestMethod]
        public void InterMissionCommand_ZeroSelection_ReopensMenu()
        {
            var (src, gui) = CreateSrc();
            var calls = 0;
            gui.ListBoxHandler = _ => ++calls < 3 ? 0 : 1;
            src.InterMission.InterMissionCommand(skip_update: true);
            Assert.AreEqual(3, calls);
        }

        [TestMethod]
        public void InterMissionCommand_RankUpSelection_ReturnsToMenuAfterRankUp()
        {
            var (src, gui) = CreateSrc();
            var calls = 0;
            // 1: 機体改造 → (RankUp 内の ListBox: キャンセル) → 再度 メニューで次のステージへ
            gui.ListBoxHandler = args =>
            {
                calls++;
                if (args.lb_caption == "コマンド")
                {
                    return calls == 1 ? args.Items.Select(x => x.Text).ToList().IndexOf("機体改造") + 1 : 1;
                }
                return 0;
            };
            src.InterMission.InterMissionCommand(skip_update: true);
            Assert.IsTrue(calls >= 3);
        }

        [TestMethod]
        public void InterMissionCommand_ConfirmNextStage_SetsStageName()
        {
            var (src, gui) = CreateSrc();
            gui.ListBoxHandler = _ => 1;
            src.Stage = "別";
            src.InterMission.InterMissionCommand(skip_update: true);
            Assert.AreEqual("インターミッション", src.Stage);
            Assert.IsFalse(src.IsSubStage);
        }
    }
}
