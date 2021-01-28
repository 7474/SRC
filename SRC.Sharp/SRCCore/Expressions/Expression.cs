// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using Newtonsoft.Json;
using SRC.Core.Events;
using System;

namespace SRC.Core.Expressions
{
    public partial class Expression
    {
        private WeakReference<SRC> _src;
        private SRC SRC
        {
            get
            {
                SRC res;
                _src.TryGetTarget(out res);
                return res;
            }
        }
        private Event Event => SRC.Event;

        public Expression(SRC src)
        {
            // XXX これでいいかは知らん。
            _src = new WeakReference<SRC>(src, true);
        }

        public string DumpVariables()
        {
            return JsonConvert.SerializeObject(Event);
        }
    }
}
