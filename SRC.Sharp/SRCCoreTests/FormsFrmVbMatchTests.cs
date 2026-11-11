using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Coverage.Tests.SpecialPower;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Tests
{
    /// <summary>
    /// VB のフォーム（*.frm）の手続きに当たる処理を、VB の動作を期待値にして確かめる（#1498）。
    /// </summary>
    [TestClass]
    public class FormsFrmVbMatchTests
    {
        private static (SpecialPowerTestFixture f, List<ListBoxItem> items) CreateUnits(params (string name, string kana, int level, int exp)[] units)
        {
            var f = new SpecialPowerTestFixture();
            var items = new List<ListBoxItem>();
            var x = 1;
            foreach (var (name, kana, level, exp) in units)
            {
                var u = f.AddUnit(name, name + "のパイロット", "味方", x, 1, level);
                f.Src.UDList.Item(name).KanaName = kana;
                u.MainPilot().Exp = exp;
                items.Add(new ListBoxItem(name, u.ID));
                x++;
            }
            return (f, items);
        }

        private static string[] Names(IEnumerable<ListBoxItem> items) => items.Select(x => x.Text).ToArray();

        // MultiSelectListBox.frm の cmdSort_Click: レベル順は 500 * Level + Exp の大きい順（選択ソート）
        [TestMethod]
        public void MultiSelectSortByLevel_DescendingByLevelAndExp()
        {
            var (f, items) = CreateUnits(
                ("A", "あ", 1, 0),
                ("B", "い", 5, 10),
                ("C", "う", 5, 300),
                ("D", "え", 3, 0));
            items[0].ListItemFlag = true;

            MultiSelectListBoxSort.SortByLevel(items, f.Src.UList);

            CollectionAssert.AreEqual(new[] { "C", "B", "D", "A" }, Names(items));
            // 選択の印は項目と一緒に動く（VB は ItemFlag も入れ替える）
            Assert.IsTrue(items[3].ListItemFlag);
        }

        // VB の選択ソートは最大のものと入れ替えるので、同じ値の項目の順は保たれないことがある
        [TestMethod]
        public void MultiSelectSortByLevel_SwapsLikeSelectionSort()
        {
            var (f, items) = CreateUnits(
                ("A", "あ", 1, 0),
                ("B", "い", 1, 0),
                ("C", "う", 2, 0));

            MultiSelectListBoxSort.SortByLevel(items, f.Src.UList);

            // i=1: 最大の C と A を入れ替え → C B A。i=2: B と A は同じ値なので入れ替えない
            CollectionAssert.AreEqual(new[] { "C", "B", "A" }, Names(items));
        }

        // 名称順は読み仮名の小さい順（StrComp の vbTextCompare。ひらがな・カタカナを区別しない）
        [TestMethod]
        public void MultiSelectSortByName_AscendingByKanaName()
        {
            var (f, items) = CreateUnits(
                ("A", "う", 1, 0),
                ("B", "ア", 1, 0),
                ("C", "い", 1, 0));

            MultiSelectListBoxSort.SortByName(items, f.Src.UList);

            CollectionAssert.AreEqual(new[] { "B", "C", "A" }, Names(items));
        }
    }
}
