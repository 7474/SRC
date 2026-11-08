// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.VB;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore
{
    /// <summary>
    /// 出撃ユニット選択用リストボックス（MultiSelectListBox.frm の cmdSort_Click）の並べ替え。
    /// 項目（ユニットの ID と選択の印）をそのまま並べ替える。VB は ListItemID も並べ替えるので、
    /// 呼び出し元（Organize コマンド）が出撃させる順もこの順になる。
    /// </summary>
    public static class MultiSelectListBoxSort
    {
        /// <summary>
        /// メインパイロットのレベル順（レベル × 500 ＋ 経験値の大きい順）に並べ替える。
        /// VB と同じ選択ソート（最大のものと入れ替える）なので、同じ値の項目の順は保たれないことがある。
        /// </summary>
        public static void SortByLevel(IList<ListBoxItem> items, Units.Units units)
        {
            var keys = items.Select(x =>
            {
                var p = units.Item(x.ListItemID).MainPilot();
                return 500L * p.Level + p.Exp;
            }).ToList();
            for (var i = 0; i < items.Count - 1; i++)
            {
                var max_item = i;
                var max_value = keys[i];
                for (var j = i + 1; j < items.Count; j++)
                {
                    if (keys[j] > max_value)
                    {
                        max_item = j;
                        max_value = keys[j];
                    }
                }
                if (max_item != i)
                {
                    (items[i], items[max_item]) = (items[max_item], items[i]);
                    keys[max_item] = keys[i];
                }
            }
        }

        /// <summary>
        /// ユニットの読み仮名（KanaName）の小さい順に並べ替える。
        /// VB は StrComp(…, vbTextCompare) = -1 で最小のものを選ぶ選択ソート
        /// （大文字・小文字、全角・半角、ひらがな・カタカナを区別しない）。
        /// </summary>
        public static void SortByName(IList<ListBoxItem> items, Units.Units units)
        {
            var comparer = SrcCollection<string>.KeyComparer;
            var keys = items.Select(x => units.Item(x.ListItemID).KanaName ?? "").ToList();
            for (var i = 0; i < items.Count - 1; i++)
            {
                var max_item = i;
                var max_str = keys[i];
                for (var j = i + 1; j < items.Count; j++)
                {
                    if (comparer.Compare(keys[j], max_str) < 0)
                    {
                        max_item = j;
                        max_str = keys[j];
                    }
                }
                if (max_item != i)
                {
                    (items[i], items[max_item]) = (items[max_item], items[i]);
                    keys[max_item] = keys[i];
                }
            }
        }
    }
}
