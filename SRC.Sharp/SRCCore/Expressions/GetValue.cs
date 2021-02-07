// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

namespace SRCCore.Expressions
{
    public partial class Expression
    {
        // 式を文字列として評価
        public string GetValueAsString(string expr, bool is_term = false)
        {
            string str;
            if (is_term)
            {
                EvalTerm(expr, ValueType.StringType, out str, out _);
            }
            else
            {
                EvalExpr(expr, ValueType.StringType, out str, out _);
            }
            return str;
        }

        // 式を浮動小数点数として評価
        public double GetValueAsDouble(string expr, bool is_term = false)
        {
            double num;
            if (is_term)
            {
                EvalTerm(expr, ValueType.NumericType, out _, out num);
            }
            else
            {
                EvalExpr(expr, ValueType.NumericType, out _, out num);
            }

            return num;
        }

        // 式を整数として評価
        public int GetValueAsLong(string expr, bool is_term = false)
        {
            double num;
            if (is_term)
            {
                EvalTerm(expr, ValueType.NumericType, out _, out num);
            }
            else
            {
                EvalExpr(expr, ValueType.NumericType, out _, out num);
            }

            return (int)num;
        }
    }
}
