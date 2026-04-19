using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Tests;
using SRCCore.Exceptions;

namespace SRCCore.Intermissions.Tests
{
    /// <summary>
    /// Intermission.cs の public メソッドのユニットテスト
    /// InterMissionCommand, RankUpCommand
    /// </summary>
    [TestClass]
    public class InterMissionCommandTests
    {
        private SRC CreateSrc()
        {
            return new SRC
            {
                GUI = new MockGUI(),
                GUIMap = new MockGUIMap(),
                GUIStatus = new MockGUIStatus(),
            };
        }

        private void SetupClearMapHandlers(SRC src)
        {
            // Map.ClearMap() は SetMapSize(1,1) → GUIMap.SetMapSize (no-op via MockGUIMap)
            // その後 GUI.RedrawScreen() を呼ぶ
            ((MockGUI)src.GUI).RedrawScreenHandler = (_) => { };
        }

        // ──────────────────────────────────────────────
        // InterMissionCommand: Stage と IsSubStage の設定
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InterMissionCommand_SetsStageName()
        {
            var src = CreateSrc();
            SetupClearMapHandlers(src);
            ((MockGUI)src.GUI).EnlargeListBoxHeightHandler = () => { };

            // ListBox が呼ばれたら GUINotImplementedException を投げてテスト終了
            // (Stage はその前に設定される)
            try
            {
                src.InterMission.InterMissionCommand(skip_update: true);
            }
            catch (GUINotImplementedException) { }
            catch { }

            Assert.AreEqual("インターミッション", src.Stage);
        }

        [TestMethod]
        public void InterMissionCommand_SetsIsSubStageFalse()
        {
            var src = CreateSrc();
            src.IsSubStage = true; // あらかじめ true に設定
            SetupClearMapHandlers(src);
            ((MockGUI)src.GUI).EnlargeListBoxHeightHandler = () => { };

            try
            {
                src.InterMission.InterMissionCommand(skip_update: true);
            }
            catch (GUINotImplementedException) { }
            catch { }

            Assert.IsFalse(src.IsSubStage);
        }

        [TestMethod]
        public void InterMissionCommand_SetsStageAndIsSubStage_BeforeGUICalls()
        {
            // Stage と IsSubStage は GUI 呼び出しより前に設定される
            var src = CreateSrc();
            src.IsSubStage = true;
            SetupClearMapHandlers(src);
            ((MockGUI)src.GUI).EnlargeListBoxHeightHandler = () => { };

            try
            {
                src.InterMission.InterMissionCommand(skip_update: true);
            }
            catch { }

            Assert.AreEqual("インターミッション", src.Stage);
            Assert.IsFalse(src.IsSubStage);
        }

        // ──────────────────────────────────────────────
        // RankUpCommand: キャンセルパス (ListBox → 0)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RankUpCommand_ListBoxCancel_ReturnsWithoutException()
        {
            // UList が空のとき、ListBox で 0 を返すとキャンセルして return
            var src = CreateSrc();
            ((MockGUI)src.GUI).ListBoxHandler = (_) => 0;

            // 例外なく完了するはず
            src.InterMission.RankUpCommand();
        }

        [TestMethod]
        public void RankUpCommand_ListBoxCancel_TopItemSetToOne()
        {
            // RankUpCommand は GUI.TopItem = 1 を設定する
            var src = CreateSrc();
            ((MockGUI)src.GUI).ListBoxHandler = (_) => 0;

            src.InterMission.RankUpCommand();

            Assert.AreEqual(1, src.GUI.TopItem);
        }

        [TestMethod]
        public void RankUpCommand_ListBoxCalled()
        {
            // ListBox が呼ばれることを確認
            var src = CreateSrc();
            bool listBoxCalled = false;
            ((MockGUI)src.GUI).ListBoxHandler = (_) =>
            {
                listBoxCalled = true;
                return 0;
            };

            src.InterMission.RankUpCommand();

            Assert.IsTrue(listBoxCalled);
        }

        [TestMethod]
        public void RankUpCommand_WithUnit_ListBoxContainsSortOption()
        {
            // ユニットがある場合でもキャンセルできる
            var src = CreateSrc();
            if (!src.UDList.IsDefined("テスト機体"))
            {
                src.UDList.Add("テスト機体");
            }
            src.UList.Add("テスト機体", 0, "味方");

            bool sortItemSeen = false;
            ((MockGUI)src.GUI).ListBoxHandler = (args) =>
            {
                // 最初の項目はソート (▽並べ替え▽)
                if (args.Items.Count > 0 && args.Items[0].Text == "▽並べ替え▽")
                {
                    sortItemSeen = true;
                }
                return 0; // キャンセル
            };

            src.InterMission.RankUpCommand();

            Assert.IsTrue(sortItemSeen, "最初の項目は '▽並べ替え▽' であるはずです");
        }
    }
}
