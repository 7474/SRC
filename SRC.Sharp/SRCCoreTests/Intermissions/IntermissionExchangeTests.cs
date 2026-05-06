using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Intermissions.Tests
{
    /// <summary>
    /// InterMission クラスの Exchange 系コマンド (ExchangeUnitCommand / ExchangeItemCommand /
    /// ExchangeFormCommand) のユニットテスト。
    /// これらのメソッドは GUI のリストボックスダイアログを使用するため、
    /// MockGUI のハンドラを利用して各種パスを検証する。
    /// </summary>
    [TestClass]
    public class IntermissionExchangeTests
    {
        private (SRC src, MockGUI gui) CreateSrc()
        {
            var gui = new MockGUI();
            var src = new SRC { GUI = gui };
            return (src, gui);
        }

        // ──────────────────────────────────────────────
        // ExchangeUnitCommand (乗り換え)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExchangeUnitCommand_Cancel_CompletesWithoutError()
        {
            // パイロット一覧 ListBox でキャンセル (ret=0) した場合にメソッドが正常終了する
            var (src, gui) = CreateSrc();
            // キャンセル選択: ListBox が 0 を返す
            gui.ListBoxHandler = _ => 0;

            var im = new InterMission(src);

            // 例外が発生しないことを確認
            im.ExchangeUnitCommand();
        }

        [TestMethod]
        public void ExchangeUnitCommand_EmptyPilotList_ShowsListBoxOnce()
        {
            // PList が空の状態でも ListBox は1回呼ばれる (ヘッダ行のみ)
            var (src, gui) = CreateSrc();
            int listBoxCallCount = 0;
            gui.ListBoxHandler = _ => { listBoxCallCount++; return 0; };

            var im = new InterMission(src);
            im.ExchangeUnitCommand();

            Assert.AreEqual(1, listBoxCallCount, "PList が空でも ListBox は一度呼ばれるべき");
        }

        [TestMethod]
        public void ExchangeUnitCommand_SortModeSelection_ThenCancel_CompletesWithoutError()
        {
            // ソートモード選択 (ret=1) → キャンセル (ret=0) の連続操作で正常終了する
            // ListBox: 1回目は1 (ソート選択行), ソート確認ダイアログで0 (キャンセル), 最終的に0
            var (src, gui) = CreateSrc();
            int callCount = 0;
            gui.ListBoxHandler = _ =>
            {
                callCount++;
                // 1回目 (メインリスト): "▽並べ替え▽" 行 (index 1) を選択 → ソートダイアログ表示
                // 2回目 (ソートダイアログ): キャンセル (0)
                // 3回目 (メインリスト再表示): キャンセル (0)
                return callCount == 1 ? 1 : 0;
            };

            var im = new InterMission(src);
            im.ExchangeUnitCommand();
            // 例外なく完了すれば OK
        }

        // ──────────────────────────────────────────────
        // ExchangeFormCommand (換装)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExchangeFormCommand_NoEligibleUnits_CompletesWithoutError()
        {
            // 換装可能なユニットが存在しない (UList が空) 場合、ListBox には空リストが渡され、
            // キャンセルで正常終了する
            var (src, gui) = CreateSrc();
            gui.ListBoxHandler = _ => 0;

            var im = new InterMission(src);
            im.ExchangeFormCommand();
        }

        [TestMethod]
        public void ExchangeFormCommand_EmptyUnitList_ShowsListBoxOnce()
        {
            // UList が空の場合でも ListBox は一度呼ばれる
            var (src, gui) = CreateSrc();
            int listBoxCallCount = 0;
            gui.ListBoxHandler = _ => { listBoxCallCount++; return 0; };

            var im = new InterMission(src);
            im.ExchangeFormCommand();

            Assert.AreEqual(1, listBoxCallCount, "UList が空でも ListBox は一度呼ばれるべき");
        }

        // ──────────────────────────────────────────────
        // ExchangeItemCommand (アイテム交換)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExchangeItemCommand_Cancel_CompletesWithoutError()
        {
            // ユニット選択 ListBox でキャンセル (ret=0) した場合に正常終了する
            var (src, gui) = CreateSrc();
            gui.ListBoxHandler = _ => 0;

            var im = new InterMission(src);
            im.ExchangeItemCommand();
        }

        [TestMethod]
        public void ExchangeItemCommand_EmptyUnitList_ShowsListBoxOnce()
        {
            // UList が空の状態でも ListBox は一度呼ばれる
            var (src, gui) = CreateSrc();
            int listBoxCallCount = 0;
            gui.ListBoxHandler = _ => { listBoxCallCount++; return 0; };

            var im = new InterMission(src);
            im.ExchangeItemCommand();

            Assert.AreEqual(1, listBoxCallCount, "UList が空でも ListBox は一度呼ばれるべき");
        }

        [TestMethod]
        public void ExchangeItemCommand_SortModeSelection_ThenCancel_CompletesWithoutError()
        {
            // ソートモード選択 (ret=1) → ソートダイアログキャンセル → メインリストキャンセル
            var (src, gui) = CreateSrc();
            int callCount = 0;
            gui.ListBoxHandler = _ =>
            {
                callCount++;
                return callCount == 1 ? 1 : 0;
            };

            var im = new InterMission(src);
            im.ExchangeItemCommand();
        }
    }
}
