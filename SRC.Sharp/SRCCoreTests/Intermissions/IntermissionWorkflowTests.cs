using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Collections.Generic;

namespace SRCCore.Intermissions.Tests
{
    [TestClass]
    public class IntermissionWorkflowTests
    {
        private sealed class MockGUIMap : IGUIMap
        {
            public int Width { get; private set; }
            public int Height { get; private set; }

            public void InitMapSize(int w, int h)
            {
                Width = w;
                Height = h;
            }

            public void SetMapSize(int w, int h)
            {
                Width = w;
                Height = h;
            }
        }

        private (SRC src, MockGUI gui) CreateSrc()
        {
            var gui = new MockGUI();
            var src = new SRC
            {
                GUI = gui,
                GUIMap = new MockGUIMap(),
            };
            src.Event.EventData = new List<EventDataLine>
            {
                new EventDataLine(0, EventDataSource.System, "system", 0, ""),
            };
            src.Event.EventQue = new List<string>();
            gui.RedrawScreenHandler = _ => { };
            gui.EnlargeListBoxHeightHandler = () => { };
            gui.CloseListBoxHandler = () => { };
            gui.ReduceListBoxHeightHandler = () => { };
            src.Expression.DefineGlobalVariable("BGM(Intermission)");
            src.Expression.SetVariableAsString("BGM(Intermission)", "");
            return (src, gui);
        }

        private static Unit AddUnit(SRC src, string name = "テスト機体")
        {
            src.UDList.Add(name);
            return src.UList.Add(name, 0, "味方");
        }

        [TestMethod]
        public void InterMissionCommand_ConfirmNextStage_UpdatesStageAndReturns()
        {
            var (src, gui) = CreateSrc();
            src.Expression.DefineGlobalVariable("次ステージ");
            src.Expression.SetVariableAsString("次ステージ", "next.eve");
            src.Sound.KeepBGM = true;
            src.Sound.BossBGM = true;
            var selectedItems = 0;
            gui.ListBoxHandler = args =>
            {
                selectedItems = args.Items.Count;
                Assert.AreEqual("次のステージへ", args.Items[0].Text);
                return 1;
            };
            gui.ConfirmHandler = (_, title, _) =>
            {
                Assert.AreEqual("次ステージ", title);
                return GuiDialogResult.Ok;
            };

            src.InterMission.InterMissionCommand(skip_update: true);

            Assert.AreEqual("インターミッション", src.Stage);
            Assert.IsFalse(src.IsSubStage);
            Assert.IsTrue(selectedItems > 1);
            Assert.IsFalse(src.Sound.KeepBGM);
            Assert.IsFalse(src.Sound.BossBGM);
            Assert.AreEqual(1, ((MockGUIMap)src.GUIMap).Width);
            Assert.AreEqual(1, ((MockGUIMap)src.GUIMap).Height);
        }

        [TestMethod]
        public void InterMissionCommand_CancelNextStageConfirmation_ReopensMenu()
        {
            var (src, gui) = CreateSrc();
            src.Expression.DefineGlobalVariable("次ステージ");
            src.Expression.SetVariableAsString("次ステージ", "next.eve");
            var listBoxCalls = 0;
            var confirmCalls = 0;
            gui.ListBoxHandler = _ =>
            {
                listBoxCalls++;
                return 1;
            };
            gui.ConfirmHandler = (_, _, _) =>
            {
                confirmCalls++;
                return confirmCalls == 1 ? GuiDialogResult.Cancel : GuiDialogResult.Ok;
            };

            src.InterMission.InterMissionCommand(skip_update: true);

            Assert.AreEqual(2, listBoxCalls);
            Assert.AreEqual(2, confirmCalls);
        }

        [TestMethod]
        public void RankUpCommand_Cancel_PreservesUnitRankAndMoney()
        {
            var (src, gui) = CreateSrc();
            var unit = AddUnit(src);
            src.Money = 50000;
            var firstItemFlag = false;
            gui.ListBoxHandler = args =>
            {
                firstItemFlag = args.Items[1].ListItemFlag;
                return 0;
            };

            src.InterMission.RankUpCommand();

            Assert.IsFalse(firstItemFlag);
            Assert.AreEqual(0, unit.Rank);
            Assert.AreEqual(50000, src.Money);
        }

        [TestMethod]
        public void RankUpCommand_SortSelection_OffersSortOptionsAndCanCancel()
        {
            var (src, gui) = CreateSrc();
            AddUnit(src);
            var listBoxCalls = 0;
            var sortOptions = 0;
            gui.ListBoxHandler = args =>
            {
                listBoxCalls++;
                if (listBoxCalls == 1)
                {
                    Assert.AreEqual("▽並べ替え▽", args.Items[0].Text);
                    return 1;
                }

                if (listBoxCalls == 2)
                {
                    sortOptions = args.Items.Count;
                    Assert.AreEqual("どれで並べ替えますか？", args.lb_caption);
                    return 6;
                }

                return 0;
            };

            src.InterMission.RankUpCommand();

            Assert.AreEqual(7, sortOptions);
            Assert.AreEqual(3, listBoxCalls);
        }

        [TestMethod]
        public void RankUpCommand_DeclineConfirmation_PreservesUnitRankAndMoney()
        {
            var (src, gui) = CreateSrc();
            var unit = AddUnit(src);
            src.Money = 50000;
            var listBoxCalls = 0;
            gui.ListBoxHandler = _ => ++listBoxCalls == 1 ? 2 : 0;
            gui.ConfirmHandler = (_, _, _) => GuiDialogResult.Cancel;

            src.InterMission.RankUpCommand();

            Assert.AreEqual(2, listBoxCalls);
            Assert.AreEqual(0, unit.Rank);
            Assert.AreEqual(50000, src.Money);
        }

        [TestMethod]
        public void RankUpCommand_ConfirmUpgrade_DeductsCostAndRaisesRank()
        {
            var (src, gui) = CreateSrc();
            var unit = AddUnit(src);
            src.Money = 50000;
            var listBoxCalls = 0;
            gui.ListBoxHandler = _ => ++listBoxCalls == 1 ? 2 : 0;
            gui.ConfirmHandler = (_, title, _) =>
            {
                Assert.AreEqual("改造", title);
                return GuiDialogResult.Ok;
            };

            src.InterMission.RankUpCommand();

            Assert.AreEqual(2, listBoxCalls);
            Assert.AreEqual(1, unit.Rank);
            Assert.AreEqual(40000, src.Money);
        }
    }
}
