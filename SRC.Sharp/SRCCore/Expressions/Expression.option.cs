// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using Newtonsoft.Json;
using SRCCore.Events;
using System;

namespace SRCCore.Expressions
{
    public partial class Expression
    {
        // 指定したオプションが設定されているか？
        public bool IsOptionDefined(string oname)
        {
            return Event.GlobalVariableList.ContainsKey("Option(" + oname + ")");
        }
    }
}
