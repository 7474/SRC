// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using System;

namespace SRC.Core.Expressions
{
    public static partial class Expression
    {
        // 式を評価
        public static ValueType EvalExpr(string expr, ValueType etype, out string str_result, out double num_result)
        {
            throw new NotImplementedException();
        }

        // 項を評価
        public static ValueType EvalTerm(string expr, ValueType etype, out string str_result, out double num_result)
        {
            throw new NotImplementedException();
        }
    }
}
