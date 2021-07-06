// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using Newtonsoft.Json;
using SRCCore.Events;

namespace SRCCore.Expressions
{
    public partial class Expression
    {
        protected SRC SRC { get; }
        private Event Event => SRC.Event;
        private BCVariable BCVariable => Event.BCVariable;
        private Commands.Command Commands => SRC.Commands;
        private IGUI GUI => SRC.GUI;

        public Expression(SRC src)
        {
            SRC = src;
        }

        public string DumpVariables()
        {
            return JsonConvert.SerializeObject(Event);
        }
    }
}
