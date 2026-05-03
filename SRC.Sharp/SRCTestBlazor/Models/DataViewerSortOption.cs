using System;

namespace SRCTestBlazor.Models
{
    /// <summary>
    /// DataViewerのソート条件を管理するクラス。
    /// 条件名とソートキー抽出関数のペアで管理することで、
    /// 最大攻撃力のような算出値でのソートにも対応する。
    /// KeySelector が null の場合は元のデータ定義順（ソートなし）を表す。
    /// </summary>
    public class DataViewerSortOption<T>
    {
        public string Label { get; }
        public Func<T, IComparable> KeySelector { get; }

        /// <summary>定義順（ソートなし）を表すオプションを作成する。</summary>
        public DataViewerSortOption(string label)
        {
            Label = label;
            KeySelector = null;
        }

        public DataViewerSortOption(string label, Func<T, IComparable> keySelector)
        {
            Label = label;
            KeySelector = keySelector;
        }
    }
}
