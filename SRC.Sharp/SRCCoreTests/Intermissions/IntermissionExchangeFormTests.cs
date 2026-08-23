using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Intermissions.Tests
{
    /// <summary>
    /// InterMission.ExchangeFormCommand() (換装コマンド) のユニットテスト。
    ///
    /// ExchangeFormCommand の動作:
    ///   1. 待機中の味方ユニットのうち換装能力を持つものをリスト化
    ///   2. GUI.ListBox でユニットを選択
    ///   3. キャンセル (ret=0) の場合: 即 return
    ///   4. ユニットを選択した場合: 換装先リストを表示し Transform() を実行
    ///      キャンセル時は先頭 (Beginning:) に戻ってループ
    ///
    /// UList が空のため換装可能ユニットはゼロ → ListBox に空リストが渡されることを確認。
    /// </summary>
    [TestClass]
    public class IntermissionExchangeFormTests
    {
        private (SRC src, MockGUI gui) CreateSrc()
        {
            var gui = new MockGUI();
            var src = new SRC { GUI = gui };
            return (src, gui);
        }

        // ──────────────────────────────────────────────
        // キャンセル時: 即 return して例外なし
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExchangeFormCommand_Cancel_CompletesWithoutError()
        {
            // ListBox でキャンセル (ret=0) した場合にメソッドが正常終了する
            var (src, gui) = CreateSrc();
            gui.ListBoxHandler = _ => 0;

            var im = new InterMission(src);
            im.ExchangeFormCommand();
            // 例外なし
        }

        [TestMethod]
        public void ExchangeFormCommand_Cancel_DoesNotModifyUnitList()
        {
            // キャンセル時はユニットリストに変更がない
            var (src, gui) = CreateSrc();
            gui.ListBoxHandler = _ => 0;

            var im = new InterMission(src);
            var unitCountBefore = src.UList.Items.Count;
            im.ExchangeFormCommand();
            Assert.AreEqual(unitCountBefore, src.UList.Items.Count);
        }

        // ──────────────────────────────────────────────
        // 換装可能ユニットが存在しない場合: ListBox に空リストが渡される
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExchangeFormCommand_NoEligibleUnits_ListBoxCalledWithEmptyItems()
        {
            // UList が空の場合、換装対象ユニットはゼロ → ListBox の Items は空
            var (src, gui) = CreateSrc();
            int itemCount = -1;
            gui.ListBoxHandler = args =>
            {
                itemCount = args.Items.Count;
                return 0;
            };

            var im = new InterMission(src);
            im.ExchangeFormCommand();

            Assert.AreEqual(0, itemCount, "換装可能ユニットがいない場合 Items は空のはずです");
        }

        [TestMethod]
        public void ExchangeFormCommand_NoEligibleUnits_ListBoxCalledOnce()
        {
            // ユニットがいない場合、ListBox は1回だけ呼ばれてキャンセルで終了
            var (src, gui) = CreateSrc();
            int callCount = 0;
            gui.ListBoxHandler = _ => { callCount++; return 0; };

            var im = new InterMission(src);
            im.ExchangeFormCommand();

            Assert.AreEqual(1, callCount, "ListBox は1回呼ばれるはずです");
        }

        // ──────────────────────────────────────────────
        // TopItem: ListBox 呼び出し前に GUI.TopItem が設定される
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExchangeFormCommand_FirstCall_TopItemIsOne()
        {
            // ExchangeFormCommand は最初の ListBox 前に TopItem = 1 を設定する
            var (src, gui) = CreateSrc();
            gui.TopItem = 99;
            int topItemWhenCalled = -1;
            gui.ListBoxHandler = args =>
            {
                topItemWhenCalled = gui.TopItem;
                return 0;
            };

            var im = new InterMission(src);
            im.ExchangeFormCommand();

            Assert.AreEqual(1, topItemWhenCalled, "ListBox 呼び出し前に TopItem = 1 が設定されるはずです");
        }

        // ──────────────────────────────────────────────
        // Caption と Mode の確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExchangeFormCommand_ListBoxCaption_IsUnitSelect()
        {
            // ListBox のキャプションは "ユニット選択"
            var (src, gui) = CreateSrc();
            string caption = null;
            gui.ListBoxHandler = args =>
            {
                caption = args.lb_caption;
                return 0;
            };

            var im = new InterMission(src);
            im.ExchangeFormCommand();

            Assert.AreEqual("ユニット選択", caption, "キャプションは \"ユニット選択\" のはずです");
        }

        [TestMethod]
        public void ExchangeFormCommand_ListBoxMode_ContainsComment()
        {
            // lb_mode に "コメント" が含まれる (アイテムリストを表示するため)
            var (src, gui) = CreateSrc();
            string mode = null;
            gui.ListBoxHandler = args =>
            {
                mode = args.lb_mode;
                return 0;
            };

            var im = new InterMission(src);
            im.ExchangeFormCommand();

            Assert.IsTrue(mode != null && mode.Contains("コメント"),
                $"lb_mode に \"コメント\" が含まれるはずです (実際: {mode})");
        }
    }
}
