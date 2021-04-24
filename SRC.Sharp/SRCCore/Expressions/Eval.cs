// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Linq;

namespace SRCCore.Expressions
{
    public partial class Expression
    {
        // 式を評価
        public ValueType EvalExpr(string expr, ValueType etype, out string str_result, out double num_result)
        {
            ValueType EvalExprRet = ValueType.UndefinedType;
            str_result = "";
            num_result = 0d;

            int op_idx, op_pri;
            var op_type = default(OperatorType);
            string lop, rop;
            bool is_lop_term = default, is_rop_term = default;

            // 式をあらかじめ要素に分解
            string[] terms;
            GeneralLib.ListSplit(expr, out terms);
            switch (terms.Length)
            {
                // 空白
                case 0:
                    {
                        return etype;
                    }

                // 項
                case 1:
                    {
                        return EvalTerm(terms[0], etype, out str_result, out num_result);
                    }

                // 括弧の対応が取れてない文字列
                case -1:
                    {
                        if (etype == ValueType.NumericType)
                        {
                            // 0とみなす
                            EvalExprRet = ValueType.NumericType;
                        }
                        else
                        {
                            EvalExprRet = ValueType.StringType;
                            str_result = expr;
                        }

                        return EvalExprRet;
                    }
            }

            // 項数が２個以上の場合は演算子を含む式

            // 優先度に合わせ、どの演算が実行されるかを判定
            op_idx = -1;
            op_pri = 100;
            for (var i = 0; i < terms.Length; i++)
            {
                // 演算子の種類を判定
                var ret = Strings.Asc(terms[i]);
                if (ret < 0)
                {
                    continue;
                }

                if (ret > 111)
                {
                    continue;
                }

                switch (Strings.Len(terms[i]))
                {
                    case 1:
                        {
                            switch (ret)
                            {
                                case 94: // ^
                                    {
                                        if (op_pri >= 10)
                                        {
                                            op_type = OperatorType.ExpoOp;
                                            op_pri = 10;
                                            op_idx = i;
                                        }

                                        break;
                                    }

                                case 42: // *
                                    {
                                        if (op_pri >= 9)
                                        {
                                            op_type = OperatorType.MultOp;
                                            op_pri = 9;
                                            op_idx = i;
                                        }

                                        break;
                                    }

                                case 47: // /
                                    {
                                        if (op_pri >= 9)
                                        {
                                            op_type = OperatorType.DivOp;
                                            op_pri = 9;
                                            op_idx = i;
                                        }

                                        break;
                                    }

                                case 92: // \
                                    {
                                        if (op_pri >= 8)
                                        {
                                            op_type = OperatorType.IntDivOp;
                                            op_pri = 8;
                                            op_idx = i;
                                        }

                                        break;
                                    }

                                case 43: // +
                                    {
                                        if (op_pri >= 6)
                                        {
                                            op_type = OperatorType.PlusOp;
                                            op_pri = 6;
                                            op_idx = i;
                                        }

                                        break;
                                    }

                                case 45: // -
                                    {
                                        if (op_pri >= 6)
                                        {
                                            op_type = OperatorType.MinusOp;
                                            op_pri = 6;
                                            op_idx = i;
                                        }

                                        break;
                                    }

                                case 38: // &
                                    {
                                        if (op_pri >= 5)
                                        {
                                            op_type = OperatorType.CatOp;
                                            op_pri = 5;
                                            op_idx = i;
                                        }

                                        break;
                                    }

                                case 60: // <
                                    {
                                        if (op_pri >= 4)
                                        {
                                            op_type = OperatorType.LtOp;
                                            op_pri = 4;
                                            op_idx = i;
                                        }

                                        break;
                                    }

                                case 61: // =
                                    {
                                        if (op_pri >= 4)
                                        {
                                            op_type = OperatorType.EqOp;
                                            op_pri = 4;
                                            op_idx = i;
                                        }

                                        break;
                                    }

                                case 62: // >
                                    {
                                        if (op_pri >= 4)
                                        {
                                            op_type = OperatorType.GtOp;
                                            op_pri = 4;
                                            op_idx = i;
                                        }

                                        break;
                                    }
                            }

                            break;
                        }

                    case 2:
                        {
                            switch (ret)
                            {
                                case 33: // !=
                                    {
                                        if (op_pri >= 4)
                                        {
                                            if (Strings.Right(terms[i], 1) == "=")
                                            {
                                                op_type = OperatorType.NotEqOp;
                                                op_pri = 4;
                                                op_idx = i;
                                            }
                                        }

                                        break;
                                    }

                                case 60: // <>, <=
                                    {
                                        if (op_pri >= 4)
                                        {
                                            switch (Strings.Right(terms[i], 1) ?? "")
                                            {
                                                case ">":
                                                    {
                                                        op_type = OperatorType.NotEqOp;
                                                        op_pri = 4;
                                                        op_idx = i;
                                                        break;
                                                    }

                                                case "=":
                                                    {
                                                        op_type = OperatorType.LtEqOp;
                                                        op_pri = 4;
                                                        op_idx = i;
                                                        break;
                                                    }
                                            }
                                        }

                                        break;
                                    }

                                case 62: // >=
                                    {
                                        if (op_pri >= 4)
                                        {
                                            if (Strings.Right(terms[i], 1) == "=")
                                            {
                                                op_type = OperatorType.GtEqOp;
                                                op_pri = 4;
                                                op_idx = i;
                                            }
                                        }

                                        break;
                                    }

                                case 79:
                                case 111: // or
                                    {
                                        if (op_pri > 1)
                                        {
                                            if (Strings.LCase(terms[i]) == "or")
                                            {
                                                op_type = OperatorType.OrOp;
                                                op_pri = 1;
                                                op_idx = i;
                                            }
                                        }

                                        break;
                                    }
                            }

                            break;
                        }

                    case 3:
                        {
                            switch (ret)
                            {
                                case 77:
                                case 109: // mod
                                    {
                                        if (op_pri >= 7)
                                        {
                                            if (Strings.LCase(terms[i]) == "mod")
                                            {
                                                op_type = OperatorType.ModOp;
                                                op_pri = 7;
                                                op_idx = i;
                                            }
                                        }

                                        break;
                                    }

                                case 78:
                                case 110: // not
                                    {
                                        if (op_pri > 3)
                                        {
                                            if (Strings.LCase(terms[i]) == "not")
                                            {
                                                op_type = OperatorType.NotOp;
                                                op_pri = 3;
                                                op_idx = i;
                                            }
                                        }

                                        break;
                                    }

                                case 65:
                                case 97: // and
                                    {
                                        if (op_pri > 2)
                                        {
                                            if (Strings.LCase(terms[i]) == "and")
                                            {
                                                op_type = OperatorType.AndOp;
                                                op_pri = 2;
                                                op_idx = i;
                                            }
                                        }

                                        break;
                                    }
                            }

                            break;
                        }

                    case 4:
                        {
                            switch (ret)
                            {
                                case 76:
                                case 108: // like
                                    {
                                        if (op_pri >= 7)
                                        {
                                            if (Strings.LCase(terms[i]) == "like")
                                            {
                                                op_type = OperatorType.LikeOp;
                                                op_pri = 4;
                                                op_idx = i;
                                            }
                                        }

                                        break;
                                    }
                            }

                            break;
                        }
                }
            }

            if (op_idx < 0)
            {
                // 単なる文字列
                EvalExprRet = ValueType.StringType;
                str_result = expr;
                return EvalExprRet;
            }

            // 演算子の引数の作成
            switch (op_idx)
            {
                case 0:
                    {
                        // 左辺引数無し
                        is_lop_term = true;
                        lop = "";
                        break;
                    }

                case 1:
                    {
                        // 左辺引数は項
                        is_lop_term = true;
                        lop = terms[0];
                        break;
                    }

                default:
                    {
                        // 左辺引数の連結処理
                        lop = string.Join(" ", terms.Take(op_idx));
                        break;
                    }
            }

            if (op_idx == terms.Length - 1)
            {
                // 右辺引数は項
                is_rop_term = true;
                rop = terms.Last();
            }
            else
            {
                //// 右辺引数の連結処理
                rop = string.Join(" ", terms.Skip(op_idx + 1));
            }

            string lstr;
            double lnum;
            string rstr;
            double rnum;
            // 演算の実施
            switch (op_type)
            {
                case OperatorType.PlusOp: // +
                    {
                        if (is_lop_term)
                        {
                            EvalTerm(lop, ValueType.NumericType, out lstr, out lnum);
                        }
                        else
                        {
                            EvalExpr(lop, ValueType.NumericType, out lstr, out lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(rop, ValueType.NumericType, out rstr, out rnum);
                        }
                        else
                        {
                            EvalExpr(rop, ValueType.NumericType, out rstr, out rnum);
                        }

                        if (etype == ValueType.StringType)
                        {
                            EvalExprRet = ValueType.StringType;
                            str_result = GeneralLib.FormatNum(lnum + rnum);
                        }
                        else
                        {
                            EvalExprRet = ValueType.NumericType;
                            num_result = lnum + rnum;
                        }

                        break;
                    }

                case OperatorType.MinusOp: // -
                    {
                        if (is_lop_term)
                        {
                            EvalTerm(lop, ValueType.NumericType, out lstr, out lnum);
                        }
                        else
                        {
                            EvalExpr(lop, ValueType.NumericType, out lstr, out lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(rop, ValueType.NumericType, out rstr, out rnum);
                        }
                        else
                        {
                            EvalExpr(rop, ValueType.NumericType, out rstr, out rnum);
                        }

                        if (etype == ValueType.StringType)
                        {
                            EvalExprRet = ValueType.StringType;
                            str_result = GeneralLib.FormatNum(lnum - rnum);
                        }
                        else
                        {
                            EvalExprRet = ValueType.NumericType;
                            num_result = lnum - rnum;
                        }

                        break;
                    }

                case OperatorType.MultOp:
                    {
                        if (is_lop_term)
                        {
                            EvalTerm(lop, ValueType.NumericType, out lstr, out lnum);
                        }
                        else
                        {
                            EvalExpr(lop, ValueType.NumericType, out lstr, out lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(rop, ValueType.NumericType, out rstr, out rnum);
                        }
                        else
                        {
                            EvalExpr(rop, ValueType.NumericType, out rstr, out rnum);
                        }

                        if (etype == ValueType.StringType)
                        {
                            EvalExprRet = ValueType.StringType;
                            str_result = GeneralLib.FormatNum(lnum * rnum);
                        }
                        else
                        {
                            EvalExprRet = ValueType.NumericType;
                            num_result = lnum * rnum;
                        }

                        break;
                    }

                case OperatorType.DivOp: // /
                    {
                        if (is_lop_term)
                        {
                            EvalTerm(lop, ValueType.NumericType, out lstr, out lnum);
                        }
                        else
                        {
                            EvalExpr(lop, ValueType.NumericType, out lstr, out lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(rop, ValueType.NumericType, out rstr, out rnum);
                        }
                        else
                        {
                            EvalExpr(rop, ValueType.NumericType, out rstr, out rnum);
                        }

                        if (rnum != 0d)
                        {
                            num_result = lnum / rnum;
                        }
                        else
                        {
                            num_result = 0d;
                        }

                        if (etype == ValueType.StringType)
                        {
                            EvalExprRet = ValueType.StringType;
                            str_result = GeneralLib.FormatNum(num_result);
                        }
                        else
                        {
                            EvalExprRet = ValueType.NumericType;
                        }

                        break;
                    }

                case OperatorType.IntDivOp: // \
                    {
                        if (is_lop_term)
                        {
                            EvalTerm(lop, ValueType.NumericType, out lstr, out lnum);
                        }
                        else
                        {
                            EvalExpr(lop, ValueType.NumericType, out lstr, out lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(rop, ValueType.NumericType, out rstr, out rnum);
                        }
                        else
                        {
                            EvalExpr(rop, ValueType.NumericType, out rstr, out rnum);
                        }

                        if (rnum != 0d)
                        {
                            num_result = (long)lnum / (long)rnum;
                        }
                        else
                        {
                            num_result = 0d;
                        }

                        if (etype == ValueType.StringType)
                        {
                            EvalExprRet = ValueType.StringType;
                            str_result = GeneralLib.FormatNum(num_result);
                        }
                        else
                        {
                            EvalExprRet = ValueType.NumericType;
                        }

                        break;
                    }

                case OperatorType.ModOp: // Mod
                    {
                        if (is_lop_term)
                        {
                            EvalTerm(lop, ValueType.NumericType, out lstr, out lnum);
                        }
                        else
                        {
                            EvalExpr(lop, ValueType.NumericType, out lstr, out lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(rop, ValueType.NumericType, out rstr, out rnum);
                        }
                        else
                        {
                            EvalExpr(rop, ValueType.NumericType, out rstr, out rnum);
                        }

                        if (etype == ValueType.StringType)
                        {
                            EvalExprRet = ValueType.StringType;
                            // UPGRADE_WARNING: Mod に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                            str_result = GeneralLib.FormatNum(lnum % rnum);
                        }
                        else
                        {
                            EvalExprRet = ValueType.NumericType;
                            // UPGRADE_WARNING: Mod に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                            num_result = lnum % rnum;
                        }

                        break;
                    }

                case OperatorType.ExpoOp: // ^
                    {
                        if (is_lop_term)
                        {
                            EvalTerm(lop, ValueType.NumericType, out lstr, out lnum);
                        }
                        else
                        {
                            EvalExpr(lop, ValueType.NumericType, out lstr, out lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(rop, ValueType.NumericType, out rstr, out rnum);
                        }
                        else
                        {
                            EvalExpr(rop, ValueType.NumericType, out rstr, out rnum);
                        }

                        if (etype == ValueType.StringType)
                        {
                            EvalExprRet = ValueType.StringType;
                            str_result = GeneralLib.FormatNum(Math.Pow(lnum, rnum));
                        }
                        else
                        {
                            EvalExprRet = ValueType.NumericType;
                            num_result = Math.Pow(lnum, rnum);
                        }

                        break;
                    }

                case OperatorType.CatOp: // &
                    {
                        if (is_lop_term)
                        {
                            EvalTerm(lop, ValueType.StringType, out lstr, out lnum);
                        }
                        else
                        {
                            EvalExpr(lop, ValueType.StringType, out lstr, out lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(rop, ValueType.StringType, out rstr, out rnum);
                        }
                        else
                        {
                            EvalExpr(rop, ValueType.StringType, out rstr, out rnum);
                        }

                        if (etype == ValueType.NumericType)
                        {
                            EvalExprRet = ValueType.NumericType;
                            num_result = Conversions.ToDouble(lstr + rstr);
                        }
                        else
                        {
                            EvalExprRet = ValueType.StringType;
                            str_result = lstr + rstr;
                        }

                        break;
                    }

                case OperatorType.EqOp: // =
                    {
                        if (GeneralLib.IsNumber(lop) | GeneralLib.IsNumber(rop))
                        {
                            if (is_lop_term)
                            {
                                EvalTerm(lop, ValueType.NumericType, out lstr, out lnum);
                            }
                            else
                            {
                                EvalExpr(lop, ValueType.NumericType, out lstr, out lnum);
                            }

                            if (is_rop_term)
                            {
                                EvalTerm(rop, ValueType.NumericType, out rstr, out rnum);
                            }
                            else
                            {
                                EvalExpr(rop, ValueType.NumericType, out rstr, out rnum);
                            }

                            if (etype == ValueType.StringType)
                            {
                                EvalExprRet = ValueType.StringType;
                                if (lnum == rnum)
                                {
                                    str_result = "1";
                                }
                                else
                                {
                                    str_result = "0";
                                }
                            }
                            else
                            {
                                EvalExprRet = ValueType.NumericType;
                                if (lnum == rnum)
                                {
                                    num_result = 1d;
                                }
                                else
                                {
                                    num_result = 0d;
                                }
                            }
                        }
                        else
                        {
                            if (is_lop_term)
                            {
                                EvalTerm(lop, ValueType.StringType, out lstr, out lnum);
                            }
                            else
                            {
                                EvalExpr(lop, ValueType.StringType, out lstr, out lnum);
                            }

                            if (is_rop_term)
                            {
                                EvalTerm(rop, ValueType.StringType, out rstr, out rnum);
                            }
                            else
                            {
                                EvalExpr(rop, ValueType.StringType, out rstr, out rnum);
                            }

                            if (etype == ValueType.StringType)
                            {
                                EvalExprRet = ValueType.StringType;
                                if ((lstr ?? "") == (rstr ?? ""))
                                {
                                    str_result = "1";
                                }
                                else
                                {
                                    str_result = "0";
                                }
                            }
                            else
                            {
                                EvalExprRet = ValueType.NumericType;
                                if ((lstr ?? "") == (rstr ?? ""))
                                {
                                    num_result = 1d;
                                }
                                else
                                {
                                    num_result = 0d;
                                }
                            }
                        }

                        break;
                    }

                case OperatorType.NotEqOp: // <>, !=
                    {
                        if (GeneralLib.IsNumber(lop) | GeneralLib.IsNumber(rop))
                        {
                            if (is_lop_term)
                            {
                                EvalTerm(lop, ValueType.NumericType, out lstr, out lnum);
                            }
                            else
                            {
                                EvalExpr(lop, ValueType.NumericType, out lstr, out lnum);
                            }

                            if (is_rop_term)
                            {
                                EvalTerm(rop, ValueType.NumericType, out rstr, out rnum);
                            }
                            else
                            {
                                EvalExpr(rop, ValueType.NumericType, out rstr, out rnum);
                            }

                            if (etype == ValueType.StringType)
                            {
                                EvalExprRet = ValueType.StringType;
                                if (lnum != rnum)
                                {
                                    str_result = "1";
                                }
                                else
                                {
                                    str_result = "0";
                                }
                            }
                            else
                            {
                                EvalExprRet = ValueType.NumericType;
                                if (lnum != rnum)
                                {
                                    num_result = 1d;
                                }
                                else
                                {
                                    num_result = 0d;
                                }
                            }
                        }
                        else
                        {
                            if (is_lop_term)
                            {
                                EvalTerm(lop, ValueType.StringType, out lstr, out lnum);
                            }
                            else
                            {
                                EvalExpr(lop, ValueType.StringType, out lstr, out lnum);
                            }

                            if (is_rop_term)
                            {
                                EvalTerm(rop, ValueType.StringType, out rstr, out rnum);
                            }
                            else
                            {
                                EvalExpr(rop, ValueType.StringType, out rstr, out rnum);
                            }

                            if (etype == ValueType.StringType)
                            {
                                EvalExprRet = ValueType.StringType;
                                if ((lstr ?? "") != (rstr ?? ""))
                                {
                                    str_result = "1";
                                }
                                else
                                {
                                    str_result = "0";
                                }
                            }
                            else
                            {
                                EvalExprRet = ValueType.NumericType;
                                if ((lstr ?? "") != (rstr ?? ""))
                                {
                                    num_result = 1d;
                                }
                                else
                                {
                                    num_result = 0d;
                                }
                            }
                        }

                        break;
                    }

                case OperatorType.LtOp: // <
                    {
                        if (is_lop_term)
                        {
                            EvalTerm(lop, ValueType.NumericType, out lstr, out lnum);
                        }
                        else
                        {
                            EvalExpr(lop, ValueType.NumericType, out lstr, out lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(rop, ValueType.NumericType, out rstr, out rnum);
                        }
                        else
                        {
                            EvalExpr(rop, ValueType.NumericType, out rstr, out rnum);
                        }

                        if (etype == ValueType.StringType)
                        {
                            EvalExprRet = ValueType.StringType;
                            if (lnum < rnum)
                            {
                                str_result = "1";
                            }
                            else
                            {
                                str_result = "0";
                            }
                        }
                        else
                        {
                            EvalExprRet = ValueType.NumericType;
                            if (lnum < rnum)
                            {
                                num_result = 1d;
                            }
                            else
                            {
                                num_result = 0d;
                            }
                        }

                        break;
                    }

                case OperatorType.LtEqOp: // <=
                    {
                        if (is_lop_term)
                        {
                            EvalTerm(lop, ValueType.NumericType, out lstr, out lnum);
                        }
                        else
                        {
                            EvalExpr(lop, ValueType.NumericType, out lstr, out lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(rop, ValueType.NumericType, out rstr, out rnum);
                        }
                        else
                        {
                            EvalExpr(rop, ValueType.NumericType, out rstr, out rnum);
                        }

                        if (etype == ValueType.StringType)
                        {
                            EvalExprRet = ValueType.StringType;
                            if (lnum <= rnum)
                            {
                                str_result = "1";
                            }
                            else
                            {
                                str_result = "0";
                            }
                        }
                        else
                        {
                            EvalExprRet = ValueType.NumericType;
                            if (lnum <= rnum)
                            {
                                num_result = 1d;
                            }
                            else
                            {
                                num_result = 0d;
                            }
                        }

                        break;
                    }

                case OperatorType.GtOp: // >
                    {
                        if (is_lop_term)
                        {
                            EvalTerm(lop, ValueType.NumericType, out lstr, out lnum);
                        }
                        else
                        {
                            EvalExpr(lop, ValueType.NumericType, out lstr, out lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(rop, ValueType.NumericType, out rstr, out rnum);
                        }
                        else
                        {
                            EvalExpr(rop, ValueType.NumericType, out rstr, out rnum);
                        }

                        if (etype == ValueType.StringType)
                        {
                            EvalExprRet = ValueType.StringType;
                            if (lnum > rnum)
                            {
                                str_result = "1";
                            }
                            else
                            {
                                str_result = "0";
                            }
                        }
                        else
                        {
                            EvalExprRet = ValueType.NumericType;
                            if (lnum > rnum)
                            {
                                num_result = 1d;
                            }
                            else
                            {
                                num_result = 0d;
                            }
                        }

                        break;
                    }

                case OperatorType.GtEqOp: // >=
                    {
                        if (is_lop_term)
                        {
                            EvalTerm(lop, ValueType.NumericType, out lstr, out lnum);
                        }
                        else
                        {
                            EvalExpr(lop, ValueType.NumericType, out lstr, out lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(rop, ValueType.NumericType, out rstr, out rnum);
                        }
                        else
                        {
                            EvalExpr(rop, ValueType.NumericType, out rstr, out rnum);
                        }

                        if (etype == ValueType.StringType)
                        {
                            EvalExprRet = ValueType.StringType;
                            if (lnum >= rnum)
                            {
                                str_result = "1";
                            }
                            else
                            {
                                str_result = "0";
                            }
                        }
                        else
                        {
                            EvalExprRet = ValueType.NumericType;
                            if (lnum >= rnum)
                            {
                                num_result = 1d;
                            }
                            else
                            {
                                num_result = 0d;
                            }
                        }

                        break;
                    }

                case OperatorType.LikeOp: // Like
                    {
                        if (is_lop_term)
                        {
                            EvalTerm(lop, ValueType.StringType, out lstr, out lnum);
                        }
                        else
                        {
                            EvalExpr(lop, ValueType.StringType, out lstr, out lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(rop, ValueType.StringType, out rstr, out rnum);
                        }
                        else
                        {
                            EvalExpr(rop, ValueType.StringType, out rstr, out rnum);
                        }

                        if (etype == ValueType.StringType)
                        {
                            EvalExprRet = ValueType.StringType;
                            // XXX カルチャ次第で落ちるかも？
                            //if (LikeOperator.LikeString(lstr, rstr, CompareMethod.Binary))
                            if (lstr == rstr)
                            {
                                str_result = "1";
                            }
                            else
                            {
                                str_result = "0";
                            }
                        }
                        else
                        {
                            EvalExprRet = ValueType.NumericType;
                            // XXX カルチャ次第で落ちるかも？
                            if (lstr == rstr)
                            {
                                num_result = 1d;
                            }
                            else
                            {
                                num_result = 0d;
                            }
                        }

                        break;
                    }

                case OperatorType.NotOp: // Not
                    {
                        if (is_rop_term)
                        {
                            EvalTerm(rop, ValueType.NumericType, out rstr, out rnum);
                        }
                        else
                        {
                            EvalExpr(rop, ValueType.NumericType, out rstr, out rnum);
                        }

                        if (etype == ValueType.StringType)
                        {
                            EvalExprRet = ValueType.StringType;
                            if (rnum != 0d)
                            {
                                str_result = "0";
                            }
                            else
                            {
                                str_result = "1";
                            }
                        }
                        else
                        {
                            EvalExprRet = ValueType.NumericType;
                            if (rnum != 0d)
                            {
                                num_result = 0d;
                            }
                            else
                            {
                                num_result = 1d;
                            }
                        }

                        break;
                    }

                case OperatorType.AndOp: // And
                    {
                        if (is_lop_term)
                        {
                            EvalTerm(lop, ValueType.NumericType, out lstr, out lnum);
                        }
                        else
                        {
                            EvalExpr(lop, ValueType.NumericType, out lstr, out lnum);
                        }

                        if (lnum == 0d)
                        {
                            if (etype == ValueType.StringType)
                            {
                                EvalExprRet = ValueType.StringType;
                                str_result = "0";
                            }
                            else
                            {
                                EvalExprRet = ValueType.NumericType;
                                num_result = 0d;
                            }

                            return EvalExprRet;
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(rop, ValueType.NumericType, out rstr, out rnum);
                        }
                        else
                        {
                            EvalExpr(rop, ValueType.NumericType, out rstr, out rnum);
                        }

                        if (etype == ValueType.StringType)
                        {
                            EvalExprRet = ValueType.StringType;
                            if (rnum == 0d)
                            {
                                str_result = "0";
                            }
                            else
                            {
                                str_result = "1";
                            }
                        }
                        else
                        {
                            EvalExprRet = ValueType.NumericType;
                            if (rnum == 0d)
                            {
                                num_result = 0d;
                            }
                            else
                            {
                                num_result = 1d;
                            }
                        }

                        break;
                    }

                case OperatorType.OrOp: // Or
                    {
                        if (is_lop_term)
                        {
                            EvalTerm(lop, ValueType.NumericType, out lstr, out lnum);
                        }
                        else
                        {
                            EvalExpr(lop, ValueType.NumericType, out lstr, out lnum);
                        }

                        if (lnum != 0d)
                        {
                            if (etype == ValueType.StringType)
                            {
                                EvalExprRet = ValueType.StringType;
                                str_result = "1";
                            }
                            else
                            {
                                EvalExprRet = ValueType.NumericType;
                                num_result = 1d;
                            }

                            return EvalExprRet;
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(rop, ValueType.NumericType, out rstr, out rnum);
                        }
                        else
                        {
                            EvalExpr(rop, ValueType.NumericType, out rstr, out rnum);
                        }

                        if (etype == ValueType.StringType)
                        {
                            EvalExprRet = ValueType.StringType;
                            if (rnum == 0d)
                            {
                                str_result = "0";
                            }
                            else
                            {
                                str_result = "1";
                            }
                        }
                        else
                        {
                            EvalExprRet = ValueType.NumericType;
                            if (rnum == 0d)
                            {
                                num_result = 0d;
                            }
                            else
                            {
                                num_result = 1d;
                            }
                        }

                        break;
                    }
            }

            return EvalExprRet;
        }

        // 項を評価
        public ValueType EvalTerm(string expr, ValueType etype, out string str_result, out double num_result)
        {
            ValueType EvalTermRet = ValueType.UndefinedType;
            str_result = "";
            num_result = 0d;

            // 空白？
            if (string.IsNullOrEmpty(expr))
            {
                return EvalTermRet;
            }

            // 先頭の一文字で見分ける
            switch (expr[0])
            {
                case '\t': // タブ
                    {
                        // タブをTrimするためEvalExprで評価
                        EvalTermRet = EvalExpr(expr, etype, out str_result, out num_result);
                        return EvalTermRet;
                    }

                case ' ': // 空白
                    {
                        // Trimされてない？
                        EvalTermRet = EvalTerm(Strings.Trim(expr), etype, out str_result, out num_result);
                        return EvalTermRet;
                    }

                case '"': // "
                    {
                        // ダブルクォートで囲まれた文字列
                        if (Strings.Right(expr, 1) == "\"")
                        {
                            EvalTermRet = ValueType.StringType;
                            str_result = Strings.Mid(expr, 2, Strings.Len(expr) - 2);
                            ReplaceSubExpression(ref str_result);
                        }
                        else
                        {
                            str_result = expr;
                        }

                        if (etype != ValueType.StringType)
                        {
                            num_result = Conversions.ToDouble(str_result);
                        }

                        EvalTermRet = ValueType.StringType;
                        return EvalTermRet;
                    }

                case '#': // #
                    {
                        // 色指定
                        EvalTermRet = ValueType.StringType;
                        str_result = expr;
                        return EvalTermRet;
                    }

                case '(': // (
                    {
                        // カッコで囲まれた式
                        if (Strings.Right(expr, 1) == ")")
                        {
                            EvalTermRet = EvalExpr(Strings.Mid(expr, 2, Strings.Len(expr) - 2), etype, out str_result, out num_result);
                        }
                        else
                        {
                            str_result = expr;
                            if (etype != ValueType.StringType)
                            {
                                num_result = Conversions.ToDouble(str_result);
                            }

                            EvalTermRet = ValueType.StringType;
                        }

                        return EvalTermRet;
                    }

                case '+':
                case '-':
                case char x when x >= '0' && x <= '9':
                    // +, -, 0～9
                    {
                        // 数値？
                        if (Information.IsNumeric(expr))
                        {
                            switch (etype)
                            {
                                case ValueType.StringType:
                                    {
                                        str_result = expr;
                                        EvalTermRet = ValueType.StringType;
                                        break;
                                    }

                                case ValueType.NumericType:
                                case ValueType.UndefinedType:
                                    {
                                        num_result = Conversions.ToDouble(expr);
                                        EvalTermRet = ValueType.NumericType;
                                        break;
                                    }
                            }

                            return EvalTermRet;
                        }

                        break;
                    }

                case '`': // `
                    {
                        // バッククォートで囲まれた文字列
                        if (Strings.Right(expr, 1) == "`")
                        {
                            str_result = Strings.Mid(expr, 2, Strings.Len(expr) - 2);
                        }
                        else
                        {
                            str_result = expr;
                        }

                        if (etype != ValueType.StringType)
                        {
                            num_result = Conversions.ToDouble(str_result);
                        }

                        EvalTermRet = ValueType.StringType;
                        return EvalTermRet;
                    }
            }

            // 関数呼び出し？
            EvalTermRet = CallFunction(expr, etype, out str_result, out num_result);
            if (EvalTermRet != ValueType.UndefinedType)
            {
                return EvalTermRet;
            }

            // 変数？
            EvalTermRet = GetVariable(expr, etype, out str_result, out num_result);
            return EvalTermRet;
        }
    }
}
