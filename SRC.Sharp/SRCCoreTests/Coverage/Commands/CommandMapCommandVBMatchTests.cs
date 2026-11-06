using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Config;
using SRCCore.Filesystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Command.bas の EndTurnCommand・UnitListCommand・SearchSpecialPowerCommand を元の SRC と突き合わせたテスト（#1498）。
    /// </summary>
    [TestClass]
    public class CommandMapCommandVBMatchTests
    {
        private static List<ListBoxArgs> ScriptListBox(CommandUnitOpsFixture f, params int[] results)
        {
            var calls = new List<ListBoxArgs>();
            var queue = new Queue<int>(results);
            f.Gui.Handle("ListBox", args =>
            {
                calls.Add((ListBoxArgs)args[0]);
                return queue.Count > 0 ? queue.Dequeue() : 0;
            });
            return calls;
        }

        private static string LoadEvents(CommandUnitOpsFixture f, params string[] lines)
        {
            var dir = Path.Combine(Path.GetTempPath(), "srcs-mapcmdvb-" + Guid.NewGuid());
            Directory.CreateDirectory(dir);
            var file = Path.Combine(dir, "test.eve");
            File.WriteAllText(file, string.Join("\r\n", lines) + "\r\n", new UTF8Encoding(false));
            f.Src.SystemConfig = new LocalFileConfig { AppPath = dir };
            var fs = new LocalFileSystem();
            fs.AddPath(dir);
            f.Src.FileSystem = fs;
            f.Src.ScenarioPath = dir;
            f.Src.ScenarioFileName = file;
            f.Src.Event.InitEventData();
            f.Src.Event.LoadEventData(file);
            return dir;
        }

        [TestMethod]
        [Timeout(10000)]
        public void EndTurn_UnitThatStoppedByEarlierEvent_IsSkipped()
        {
            // 元の SRC は For Each で回しながら、ユニットごとにその時点で「行動していないか」を判定する。
            // 先の行動終了イベントで撤退したユニットの行動終了イベントは行わない
            var f = new CommandUnitOpsFixture();
            f.DefineUnit("機体");
            f.Place("機体", "アルファ", "味方", 2, 2);
            f.Place("機体", "ガンマ", "味方", 4, 4);
            var dir = LoadEvents(f,
                "ダミー:", "Exit",
                "行動終了 アルファ:", "Set 終了アルファ 1", "Escape ガンマ", "Exit",
                "行動終了 ガンマ:", "Set 終了ガンマ 1", "Exit");
            try
            {
                f.Gui.Handle("Confirm", args => GuiDialogResult.Ok);

                f.Commands.MapCommand(new UiCommand(Command.EndTurnCmdID, "ターン終了"));

                Assert.AreEqual(1, f.Src.Expression.GetValueAsLong("終了アルファ"));
                Assert.AreEqual(0, f.Src.Expression.GetValueAsLong("終了ガンマ"));
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        [TestMethod]
        public void UnitList_SortByLevel_TiesAreOrderedLikeVBSelectionSort()
        {
            // 元の SRC の並べ替えは安定でない選択ソート。
            // UList の順が A（Lv1）・B（Lv1）・C（Lv2）なら、C と A を入れ替えて C・B・A の順になる
            var f = new CommandUnitOpsFixture();
            f.DefineUnit("機体");
            var a = f.Place("機体", "パイロットA", "味方", 2, 2, level: 1);
            var b = f.Place("機体", "パイロットB", "味方", 3, 3, level: 1);
            var c = f.Place("機体", "パイロットC", "味方", 4, 4, level: 2);
            var calls = ScriptListBox(f, 0);

            f.Commands.MapCommand(new UiCommand(Command.UnitListCmdID, "部隊表"));

            CollectionAssert.AreEqual(
                new[] { "▽陣営変更・並べ替え▽", c.ID, b.ID, a.ID },
                calls.Single().Items.Select(x => x.ListItemID).ToList());
        }

        [TestMethod]
        public void UnitList_SortByUnitName_ComparesKanaLikeVBTextCompare()
        {
            // 文字列の並べ替えは StrComp(…, vbTextCompare)。ひらがなとカタカナを区別しない
            var f = new CommandUnitOpsFixture();
            f.DefineUnit("機体い").KanaName = "イ";
            f.DefineUnit("機体あ").KanaName = "あ";
            f.DefineUnit("機体う").KanaName = "う";
            var ui = f.Place("機体い", "パイロット1", "味方", 2, 2);
            var ua = f.Place("機体あ", "パイロット2", "味方", 3, 3);
            var uu = f.Place("機体う", "パイロット3", "味方", 4, 4);
            // 部隊表 → ▽陣営変更・並べ替え▽ → ユニット名称で並べ替え（8 番目）→ キャンセル
            var calls = ScriptListBox(f, 1, 8, 0);

            f.Commands.MapCommand(new UiCommand(Command.UnitListCmdID, "部隊表"));

            CollectionAssert.AreEqual(
                new[] { "▽陣営変更・並べ替え▽", ua.ID, ui.ID, uu.ID },
                calls.Last().Items.Select(x => x.ListItemID).ToList());
        }

        [TestMethod]
        public void SearchSpecialPower_PossessedUnitPilot_IsListedButSpecialPowerIsMasked()
        {
            // 元の SRC は「使えるパイロットがいるか」の判定では憑依中のユニットを除くが、
            // パイロットの一覧を作るときは憑依を見ない
            var f = new CommandUnitOpsFixture();
            f.DefineSpecialPower("集中", "集", 15, "自分", "気力増加Lv1", kana: "しゅうちゅう");
            f.DefineUnit("機体");
            var pd = f.DefinePilot("エース", sp: 30);
            pd.AddSpecialPower("集中", 1, 0);
            var unit = f.Place("機体", "エース", "味方", 4, 6);
            var pilot = unit.MainPilot();
            unit.AddCondition("憑依", -1);
            ListBoxArgs spArgs = null;
            f.Gui.Handle("MultiColumnListBox", args => { spArgs = (ListBoxArgs)args[0]; return 1; });
            var calls = ScriptListBox(f, 1);

            f.Commands.MapCommand(new UiCommand(Command.SearchSpecialPowerCmdID, "スペシャルパワー検索"));

            Assert.IsTrue(spArgs.Items.Single().ListItemFlag, "使えるパイロットがいない");
            Assert.AreEqual(pilot.ID, calls.Single().Items.Single().ListItemID);
        }

        [TestMethod]
        public void SearchSpecialPower_SortsByKanaLikeVBTextCompare()
        {
            // スペシャルパワーの並べ替えも StrComp(…, vbTextCompare) の選択ソート
            var f = new CommandUnitOpsFixture();
            f.DefineSpecialPower("鉄壁", "鉄", 10, "自分", "気力増加Lv1", kana: "テッペキ");
            f.DefineSpecialPower("熱血", "熱", 40, "自分", "気力増加Lv1", kana: "ねっけつ");
            f.DefineSpecialPower("集中", "集", 15, "自分", "気力増加Lv1", kana: "しゅうちゅう");
            ListBoxArgs spArgs = null;
            f.Gui.Handle("MultiColumnListBox", args => { spArgs = (ListBoxArgs)args[0]; return 0; });

            f.Commands.MapCommand(new UiCommand(Command.SearchSpecialPowerCmdID, "スペシャルパワー検索"));

            CollectionAssert.AreEqual(new[] { "集中", "鉄壁", "熱血" }, spArgs.Items.Select(x => x.ListItemID).ToList());
        }
    }
}
