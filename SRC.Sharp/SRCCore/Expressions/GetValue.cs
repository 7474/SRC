// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

namespace SRC.Core.Expressions
{
    public static partial class Expression
    {
        // 式を文字列として評価
        public static string GetValueAsString(string expr, bool is_term = false)
        {
            string result;
            if (is_term)
            {
                EvalTerm(expr, ValueType.StringType, out result, out _);
            }
            else
            {
                EvalExpr(expr, ValueType.StringType, out result, out _);
            }
            return result;
        }
    }
}
