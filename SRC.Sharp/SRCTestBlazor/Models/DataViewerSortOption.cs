using System;

namespace SRCTestBlazor.Models
{
    /// <summary>
    /// DataViewerのソート条件を管理するクラス。
    /// 条件名とキーセレクタ（コンパレータ）のペアで管理することで、
    /// 最大攻撃力のような算出値でのソートにも対応する。
    /// </summary>
    public class DataViewerSortOption<T>
    {
        public string Label { get; }
        public Func<T, IComparable> KeySelector { get; }

        public DataViewerSortOption(string label, Func<T, IComparable> keySelector)
        {
            Label = label;
            KeySelector = keySelector;
        }
    }
}
