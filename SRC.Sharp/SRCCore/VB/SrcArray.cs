using System;
using System.Collections.Generic;
using System.Text;

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

        // XXX 他のIndexアクセスするやつ
    }
}
