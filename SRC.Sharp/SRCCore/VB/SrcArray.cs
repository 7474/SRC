using System.Collections.Generic;

namespace SRCCore.VB
{
    // VB配列っぽいものをとりあえず置き換えておくための実装
    public class SrcArray<T> : List<T>
    {
        public new T this[int index]
        {
            get => base[index - 1];
            set => base[index - 1] = value;
        }

        // 1 から数えるのはインデクサーだけで、ほかの List<T> のメソッド（IndexOf・RemoveAt など）は 0 から数える。SRCCore では使っていない
    }
}
