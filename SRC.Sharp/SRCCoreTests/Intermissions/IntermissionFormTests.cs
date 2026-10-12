using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Intermissions.Tests
{
    /// <summary>
    /// InterMission.ExchangeFormCommand() のユニットテスト。
    ///
    /// ExchangeFormCommand の動作:
    ///   1. UList から「待機中の味方」かつ「換装」特殊能力を持つユニットのリストを構築
    ///   2. GUI.ListBox でユニット選択 (ret=0 でキャンセル → return)
    ///   3. 選択されたユニットの換装先リストを構築して GUI.ListBox で形態選択
    ///   4. 形態選択をキャンセルすると先頭ループに戻る (goto Beginning)
    ///   5. 換装を実施して再びループ (goto Beginning)
    ///
    /// GUI 依存が深いため、ユニットリストが空の場合と
    /// ユニット選択をキャンセルするパスを中心にテストする。
    /// </summary>
    [TestClass]
    public class IntermissionFormTests
    {
        private (SRC src, MockGUI gui) CreateSrc()
        {
            var gui = new MockGUI();
            var src = new SRC { GUI = gui };
            return (src, gui);
        }

        // ──────────────────────────────────────────────
        // ユニットリストが空の場合 (換装可能ユニットなし)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExchangeFormCommand_EmptyUnitList_ShowsListBoxOnce()
        {
            var (src, gui) = CreateSrc();
            int callCount = 0;
            gui.ListBoxHandler = _ => { callCount++; return 0; };

            var im = new InterMission(src);
            im.ExchangeFormCommand();

            Assert.AreEqual(1, callCount, "換装可能ユニットがなくても ListBox は一度呼ばれるべき");
        }

        [TestMethod]
        public void ExchangeFormCommand_EmptyUnitList_Cancel_CompletesWithoutError()
        {
            var (src, gui) = CreateSrc();
            gui.ListBoxHandler = _ => 0;

            var im = new InterMission(src);
            // 例外が発生しないことを確認
            im.ExchangeFormCommand();
        }

        // ──────────────────────────────────────────────
        // ユニット選択でキャンセル (ret=0)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExchangeFormCommand_UnitSelectCancel_ReturnImmediately()
        {
            var (src, gui) = CreateSrc();
            // ret=0 → 即 return
            gui.ListBoxHandler = _ => 0;

            var im = new InterMission(src);
            im.ExchangeFormCommand();
            // ここまで到達すれば正常終了
        }

        [TestMethod]
        public void ExchangeFormCommand_UnitSelectCancel_ListBoxCalledExactlyOnce()
        {
            var (src, gui) = CreateSrc();
            int count = 0;
            gui.ListBoxHandler = _ => { count++; return 0; };

            var im = new InterMission(src);
            im.ExchangeFormCommand();

            Assert.AreEqual(1, count);
        }

        // ──────────────────────────────────────────────
        // GUI.TopItem が設定されることの確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExchangeFormCommand_SetsTopItemBeforeListBox()
        {
            var (src, gui) = CreateSrc();
            gui.TopItem = 99;
            int topItemAtCall = -1;
            gui.ListBoxHandler = args =>
            {
                topItemAtCall = gui.TopItem;
                return 0;
            };

            var im = new InterMission(src);
            im.ExchangeFormCommand();

            Assert.AreEqual(1, topItemAtCall, "GUI.TopItem は ListBox 呼び出し前に 1 に設定されるはず");
        }

        // ──────────────────────────────────────────────
        // ListBox に渡されるキャプションの確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExchangeFormCommand_ListBoxCaption_IsUnitSelect()
        {
            var (src, gui) = CreateSrc();
            string captionUsed = null;
            gui.ListBoxHandler = args =>
            {
                captionUsed = args.lb_caption;
                return 0;
            };

            var im = new InterMission(src);
            im.ExchangeFormCommand();

            Assert.AreEqual("ユニット選択", captionUsed);
        }

        [TestMethod]
        public void ExchangeFormCommand_ListBoxMode_ContainsRenzoHyoji()
        {
            var (src, gui) = CreateSrc();
            string modeUsed = null;
            gui.ListBoxHandler = args =>
            {
                modeUsed = args.lb_mode;
                return 0;
            };

            var im = new InterMission(src);
            im.ExchangeFormCommand();

            StringAssert.Contains(modeUsed, "連続表示");
        }

        [TestMethod]
        public void ExchangeFormCommand_ListBoxMode_ContainsComment()
        {
            var (src, gui) = CreateSrc();
            string modeUsed = null;
            gui.ListBoxHandler = args =>
            {
                modeUsed = args.lb_mode;
                return 0;
            };

            var im = new InterMission(src);
            im.ExchangeFormCommand();

            StringAssert.Contains(modeUsed, "コメント");
        }
    }
}
