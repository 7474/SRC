using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VB = Microsoft.VisualBasic;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    static class Expression
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // イベントデータの式計算を行うモジュール

        // 演算子の種類
        public enum OperatorType
        {
            PlusOp,
            MinusOp,
            MultOp,
            DivOp,
            IntDivOp,
            ExpoOp,
            ModOp,
            CatOp,
            EqOp,
            NotEqOp,
            LtOp,
            LtEqOp,
            GtOp,
            GtEqOp,
            NotOp,
            AndOp,
            OrOp,
            LikeOp
        }

        // 型の種類
        public enum ValueType
        {
            UndefinedType = 0,
            StringType,
            NumericType
        }

        // 正規表現
        private static object RegEx;
        private static object Matches;


        // 式を評価
        public static ValueType EvalExpr(ref string expr, ref ValueType etype, ref string str_result, ref double num_result)
        {
            ValueType EvalExprRet = default;
            var terms = default(string[]);
            short tnum;
            short op_idx, op_pri;
            var op_type = default(OperatorType);
            string lop, rop;
            string lstr = default, rstr = default;
            double lnum = default, rnum = default;
            bool is_lop_term = default, is_rop_term = default;
            short osize, i, ret, tsize;
            string buf;

            // 式をあらかじめ要素に分解
            tnum = GeneralLib.ListSplit(ref expr, ref terms);
            switch (tnum)
            {
                // 空白
                case 0:
                    {
                        EvalExprRet = etype;
                        return EvalExprRet;
                    }

                // 項
                case 1:
                    {
                        EvalExprRet = EvalTerm(ref terms[1], ref etype, ref str_result, ref num_result);
                        return EvalExprRet;
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
            op_idx = 0;
            op_pri = 100;
            var loopTo = (short)(tnum - 1);
            for (i = 1; i <= loopTo; i++)
            {
                // 演算子の種類を判定
                ret = (short)Strings.Asc(terms[i]);
                if (ret < 0)
                {
                    goto NextTerm;
                }

                if (ret > 111)
                {
                    goto NextTerm;
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

                NextTerm:
                ;
            }

            if (op_idx == 0)
            {
                // 単なる文字列
                EvalExprRet = ValueType.StringType;
                str_result = expr;
                return EvalExprRet;
            }

            // 演算子の引数の作成
            switch (op_idx)
            {
                case 1:
                    {
                        // 左辺引数無し
                        is_lop_term = true;
                        lop = "";
                        break;
                    }

                case 2:
                    {
                        // 左辺引数は項
                        is_lop_term = true;
                        lop = terms[1];
                        break;
                    }

                default:
                    {
                        // 左辺引数の連結処理 (高速化のため、Midを使用)
                        buf = new string(Conversions.ToChar(Constants.vbNullChar), Strings.Len(expr));
                        tsize = (short)Strings.Len(terms[1]);
                        var midTmp = terms[1];
                        StringType.MidStmtStr(ref buf, 1, tsize, midTmp);
                        osize = tsize;
                        var loopTo1 = (short)(op_idx - 1);
                        for (i = 2; i <= loopTo1; i++)
                        {
                            StringType.MidStmtStr(ref buf, osize + 1, 1, " ");
                            tsize = (short)Strings.Len(terms[i]);
                            var midTmp1 = terms[i];
                            StringType.MidStmtStr(ref buf, osize + 2, tsize, midTmp1);
                            osize = (short)(osize + tsize + 1);
                        }

                        lop = Strings.Left(buf, osize);
                        break;
                    }
            }

            if (op_idx == tnum - 1)
            {
                // 右辺引数は項
                is_rop_term = true;
                rop = terms[tnum];
            }
            else
            {
                // 右辺引数の連結処理 (高速化のため、Midを使用)
                buf = new string(Conversions.ToChar(Constants.vbNullChar), Strings.Len(expr));
                tsize = (short)Strings.Len(terms[op_idx + 1]);
                var midTmp2 = terms[op_idx + 1];
                StringType.MidStmtStr(ref buf, 1, tsize, midTmp2);
                osize = tsize;
                var loopTo2 = tnum;
                for (i = (short)(op_idx + 2); i <= loopTo2; i++)
                {
                    StringType.MidStmtStr(ref buf, osize + 1, 1, " ");
                    tsize = (short)Strings.Len(terms[i]);
                    var midTmp3 = terms[i];
                    StringType.MidStmtStr(ref buf, osize + 2, tsize, midTmp3);
                    osize = (short)(osize + tsize + 1);
                }

                rop = Strings.Left(buf, osize);
            }

            // 演算の実施
            switch (op_type)
            {
                case OperatorType.PlusOp: // +
                    {
                        if (is_lop_term)
                        {
                            EvalTerm(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }
                        else
                        {
                            EvalExpr(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
                        }
                        else
                        {
                            EvalExpr(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
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
                            EvalTerm(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }
                        else
                        {
                            EvalExpr(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
                        }
                        else
                        {
                            EvalExpr(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
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
                            EvalTerm(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }
                        else
                        {
                            EvalExpr(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
                        }
                        else
                        {
                            EvalExpr(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
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
                            EvalTerm(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }
                        else
                        {
                            EvalExpr(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
                        }
                        else
                        {
                            EvalExpr(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
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
                            EvalTerm(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }
                        else
                        {
                            EvalExpr(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
                        }
                        else
                        {
                            EvalExpr(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
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
                            EvalTerm(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }
                        else
                        {
                            EvalExpr(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
                        }
                        else
                        {
                            EvalExpr(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
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
                            EvalTerm(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }
                        else
                        {
                            EvalExpr(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
                        }
                        else
                        {
                            EvalExpr(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
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
                            EvalTerm(ref lop, ref ValueType.StringType, ref lstr, ref lnum);
                        }
                        else
                        {
                            EvalExpr(ref lop, ref ValueType.StringType, ref lstr, ref lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(ref rop, ref ValueType.StringType, ref rstr, ref rnum);
                        }
                        else
                        {
                            EvalExpr(ref rop, ref ValueType.StringType, ref rstr, ref rnum);
                        }

                        if (etype == ValueType.NumericType)
                        {
                            EvalExprRet = ValueType.NumericType;
                            string argexpr = lstr + rstr;
                            num_result = GeneralLib.StrToDbl(ref argexpr);
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
                        if (GeneralLib.IsNumber(ref lop) | GeneralLib.IsNumber(ref rop))
                        {
                            if (is_lop_term)
                            {
                                EvalTerm(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                            }
                            else
                            {
                                EvalExpr(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                            }

                            if (is_rop_term)
                            {
                                EvalTerm(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
                            }
                            else
                            {
                                EvalExpr(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
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
                                EvalTerm(ref lop, ref ValueType.StringType, ref lstr, ref lnum);
                            }
                            else
                            {
                                EvalExpr(ref lop, ref ValueType.StringType, ref lstr, ref lnum);
                            }

                            if (is_rop_term)
                            {
                                EvalTerm(ref rop, ref ValueType.StringType, ref rstr, ref rnum);
                            }
                            else
                            {
                                EvalExpr(ref rop, ref ValueType.StringType, ref rstr, ref rnum);
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
                        if (GeneralLib.IsNumber(ref lop) | GeneralLib.IsNumber(ref rop))
                        {
                            if (is_lop_term)
                            {
                                EvalTerm(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                            }
                            else
                            {
                                EvalExpr(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                            }

                            if (is_rop_term)
                            {
                                EvalTerm(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
                            }
                            else
                            {
                                EvalExpr(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
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
                                EvalTerm(ref lop, ref ValueType.StringType, ref lstr, ref lnum);
                            }
                            else
                            {
                                EvalExpr(ref lop, ref ValueType.StringType, ref lstr, ref lnum);
                            }

                            if (is_rop_term)
                            {
                                EvalTerm(ref rop, ref ValueType.StringType, ref rstr, ref rnum);
                            }
                            else
                            {
                                EvalExpr(ref rop, ref ValueType.StringType, ref rstr, ref rnum);
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
                            EvalTerm(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }
                        else
                        {
                            EvalExpr(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
                        }
                        else
                        {
                            EvalExpr(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
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
                            EvalTerm(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }
                        else
                        {
                            EvalExpr(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
                        }
                        else
                        {
                            EvalExpr(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
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
                            EvalTerm(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }
                        else
                        {
                            EvalExpr(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
                        }
                        else
                        {
                            EvalExpr(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
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
                            EvalTerm(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }
                        else
                        {
                            EvalExpr(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
                        }
                        else
                        {
                            EvalExpr(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
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
                            EvalTerm(ref lop, ref ValueType.StringType, ref lstr, ref lnum);
                        }
                        else
                        {
                            EvalExpr(ref lop, ref ValueType.StringType, ref lstr, ref lnum);
                        }

                        if (is_rop_term)
                        {
                            EvalTerm(ref rop, ref ValueType.StringType, ref rstr, ref rnum);
                        }
                        else
                        {
                            EvalExpr(ref rop, ref ValueType.StringType, ref rstr, ref rnum);
                        }

                        if (etype == ValueType.StringType)
                        {
                            EvalExprRet = ValueType.StringType;
                            if (LikeOperator.LikeString(lstr, rstr, CompareMethod.Binary))
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
                            if (LikeOperator.LikeString(lstr, rstr, CompareMethod.Binary))
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
                            EvalTerm(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
                        }
                        else
                        {
                            EvalExpr(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
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
                            EvalTerm(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }
                        else
                        {
                            EvalExpr(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
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
                            EvalTerm(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
                        }
                        else
                        {
                            EvalExpr(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
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
                            EvalTerm(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
                        }
                        else
                        {
                            EvalExpr(ref lop, ref ValueType.NumericType, ref lstr, ref lnum);
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
                            EvalTerm(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
                        }
                        else
                        {
                            EvalExpr(ref rop, ref ValueType.NumericType, ref rstr, ref rnum);
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
        public static ValueType EvalTerm(ref string expr, ref ValueType etype, ref string str_result, ref double num_result)
        {
            ValueType EvalTermRet = default;

            // 空白？
            if (Strings.Len(expr) == 0)
            {
                return EvalTermRet;
            }

            // 先頭の一文字で見分ける
            switch (Strings.Asc(expr))
            {
                case 9: // タブ
                    {
                        // タブをTrimするためEvalExprで評価
                        EvalTermRet = EvalExpr(ref expr, ref etype, ref str_result, ref num_result);
                        return EvalTermRet;
                    }

                case 32: // 空白
                    {
                        // Trimされてない？
                        string argexpr = Strings.Trim(expr);
                        EvalTermRet = EvalTerm(ref argexpr, ref etype, ref str_result, ref num_result);
                        return EvalTermRet;
                    }

                case 34: // "
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
                            num_result = GeneralLib.StrToDbl(ref str_result);
                        }

                        EvalTermRet = ValueType.StringType;
                        return EvalTermRet;
                    }

                case 35: // #
                    {
                        // 色指定
                        EvalTermRet = ValueType.StringType;
                        str_result = expr;
                        return EvalTermRet;
                    }

                case 40: // (
                    {
                        // カッコで囲まれた式
                        if (Strings.Right(expr, 1) == ")")
                        {
                            string argexpr1 = Strings.Mid(expr, 2, Strings.Len(expr) - 2);
                            EvalTermRet = EvalExpr(ref argexpr1, ref etype, ref str_result, ref num_result);
                        }
                        else
                        {
                            str_result = expr;
                            if (etype != ValueType.StringType)
                            {
                                num_result = GeneralLib.StrToDbl(ref str_result);
                            }

                            EvalTermRet = ValueType.StringType;
                        }

                        return EvalTermRet;
                    }

                case 43:
                case 45:
                case var @case when 48 <= @case && @case <= 57: // +, -, 0～9
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

                case 96: // `
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
                            num_result = GeneralLib.StrToDbl(ref str_result);
                        }

                        EvalTermRet = ValueType.StringType;
                        return EvalTermRet;
                    }
            }

            // 関数呼び出し？
            EvalTermRet = CallFunction(ref expr, ref etype, ref str_result, ref num_result);
            if (EvalTermRet != ValueType.UndefinedType)
            {
                return EvalTermRet;
            }

            // 変数？
            EvalTermRet = GetVariable(ref expr, ref etype, ref str_result, ref num_result);
            return EvalTermRet;
        }


        // === 関数に関する処理 ===

        // 式を関数呼び出しとして構文解析し、実行
        public static ValueType CallFunction(ref string expr, ref ValueType etype, ref string str_result, ref double num_result)
        {
            ValueType CallFunctionRet = default;
            string fname;
            short start_idx;
            short num, i, j, num2;
            string buf, buf2;
            double ldbl, rdbl;
            string pname2, pname, uname;
            int ret;
            short cur_depth;
            VarData var;
            Item it;
            short depth;
            bool in_single_quote, in_double_quote;
            var @params = new string[(Event_Renamed.MaxArgIndex + 1)];
            short pcount;
            var is_term = new bool[(Event_Renamed.MaxArgIndex + 1)];
            string dir_path;
            ;
#error Cannot convert LocalDeclarationStatementSyntax - see comment for details
            /* Cannot convert LocalDeclarationStatementSyntax, System.NotSupportedException: StaticKeyword not supported!
               場所 ICSharpCode.CodeConverter.CSharp.SyntaxKindExtensions.ConvertToken(SyntaxKind t, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifier(SyntaxToken m, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.<ConvertModifiersCore>d__43.MoveNext()
               場所 System.Linq.Enumerable.<ConcatIterator>d__59`1.MoveNext()
               場所 System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
               場所 System.Linq.Buffer`1..ctor(IEnumerable`1 source)
               場所 System.Linq.OrderedEnumerable`1.<GetEnumerator>d__1.MoveNext()
               場所 Microsoft.CodeAnalysis.SyntaxTokenList.CreateNode(IEnumerable`1 tokens)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifiers(SyntaxNode node, IReadOnlyCollection`1 modifiers, TokenContext context, Boolean isVariableOrConst, SyntaxKind[] extraCsModifierKinds)
               場所 ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.<VisitLocalDeclarationStatement>d__31.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.HoistedNodeStateVisitor.<AddLocalVariablesAsync>d__6.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.<DefaultVisitInnerAsync>d__3.MoveNext()

            Input:
                    Static dir_list() As String

             */
            ;
#error Cannot convert LocalDeclarationStatementSyntax - see comment for details
            /* Cannot convert LocalDeclarationStatementSyntax, System.NotSupportedException: StaticKeyword not supported!
               場所 ICSharpCode.CodeConverter.CSharp.SyntaxKindExtensions.ConvertToken(SyntaxKind t, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifier(SyntaxToken m, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.<ConvertModifiersCore>d__43.MoveNext()
               場所 System.Linq.Enumerable.<ConcatIterator>d__59`1.MoveNext()
               場所 System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
               場所 System.Linq.Buffer`1..ctor(IEnumerable`1 source)
               場所 System.Linq.OrderedEnumerable`1.<GetEnumerator>d__1.MoveNext()
               場所 Microsoft.CodeAnalysis.SyntaxTokenList.CreateNode(IEnumerable`1 tokens)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifiers(SyntaxNode node, IReadOnlyCollection`1 modifiers, TokenContext context, Boolean isVariableOrConst, SyntaxKind[] extraCsModifierKinds)
               場所 ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.<VisitLocalDeclarationStatement>d__31.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.HoistedNodeStateVisitor.<AddLocalVariablesAsync>d__6.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.<DefaultVisitInnerAsync>d__3.MoveNext()

            Input:
                    Static dir_index As Short

             */
            ;

            // 関数呼び出しの書式に合っているかチェック
            if (Strings.Right(expr, 1) != ")")
            {
                CallFunctionRet = ValueType.UndefinedType;
                return CallFunctionRet;
            }

            i = (short)Strings.InStr(expr, " ");
            j = (short)Strings.InStr(expr, "(");
            if (i > 0)
            {
                if (i < j)
                {
                    CallFunctionRet = ValueType.UndefinedType;
                    return CallFunctionRet;
                }
            }
            else if (j == 0)
            {
                CallFunctionRet = ValueType.UndefinedType;
                return CallFunctionRet;
            }

            // ここまでくれば関数呼び出しと断定

            // パラメータの抽出
            pcount = 0;
            start_idx = (short)(j + 1);
            depth = 0;
            in_single_quote = false;
            in_double_quote = false;
            num = (short)Strings.Len(expr);
            short counter;
            counter = start_idx;
            var loopTo = (short)(num - 1);
            for (i = counter; i <= loopTo; i++)
            {
                if (in_single_quote)
                {
                    if (Strings.Asc(Strings.Mid(expr, i, 1)) == 96) // `
                    {
                        in_single_quote = false;
                    }
                }
                else if (in_double_quote)
                {
                    if (Strings.Asc(Strings.Mid(expr, i, 1)) == 34) // "
                    {
                        in_double_quote = false;
                    }
                }
                else
                {
                    switch (Strings.Asc(Strings.Mid(expr, i, 1)))
                    {
                        case 9:
                        case 32: // タブ, 空白
                            {
                                if (start_idx == i)
                                {
                                    start_idx = (short)(i + 1);
                                }
                                else
                                {
                                    is_term[pcount + 1] = false;
                                }

                                break;
                            }

                        case 40:
                        case 91: // (, [
                            {
                                depth = (short)(depth + 1);
                                break;
                            }

                        case 41:
                        case 93: // ), ]
                            {
                                depth = (short)(depth - 1);
                                break;
                            }

                        case 44: // ,
                            {
                                if (depth == 0)
                                {
                                    pcount = (short)(pcount + 1);
                                    @params[pcount] = Strings.Mid(expr, start_idx, i - start_idx);
                                    start_idx = (short)(i + 1);
                                    is_term[pcount + 1] = true;
                                }

                                break;
                            }

                        case 96: // `
                            {
                                in_single_quote = true;
                                break;
                            }

                        case 34: // "
                            {
                                in_double_quote = true;
                                break;
                            }
                    }
                }
            }

            if (num > start_idx)
            {
                pcount = (short)(pcount + 1);
                @params[pcount] = Strings.Mid(expr, start_idx, num - start_idx);
            }

            // 先頭の文字で関数の種類を判断する
            switch (Strings.Asc(expr))
            {
                case 95: // _
                    {
                        // 必ずユーザー定義関数
                        fname = Strings.Left(expr, j - 1);
                        goto LookUpUserDefinedID;
                        break;
                    }

                case var @case when 65 <= @case && @case <= 90:
                case var case1 when 97 <= case1 && case1 <= 122: // A To z
                    {
                        // システム関数の可能性あり
                        fname = Strings.Left(expr, j - 1);
                        break;
                    }

                default:
                    {
                        // 先頭がアルファベットでなければ必ずユーザー定義関数
                        // ただし括弧を含むユニット名等である場合があるため、チェックが必要
                        object argIndex1 = expr;
                        if (SRC.UDList.IsDefined(ref argIndex1))
                        {
                            CallFunctionRet = ValueType.UndefinedType;
                            return CallFunctionRet;
                        }

                        object argIndex2 = expr;
                        if (SRC.PDList.IsDefined(ref argIndex2))
                        {
                            CallFunctionRet = ValueType.UndefinedType;
                            return CallFunctionRet;
                        }

                        object argIndex3 = expr;
                        if (SRC.NPDList.IsDefined(ref argIndex3))
                        {
                            CallFunctionRet = ValueType.UndefinedType;
                            return CallFunctionRet;
                        }

                        object argIndex4 = expr;
                        if (SRC.IDList.IsDefined(ref argIndex4))
                        {
                            CallFunctionRet = ValueType.UndefinedType;
                            return CallFunctionRet;
                        }

                        fname = Strings.Left(expr, j - 1);
                        goto LookUpUserDefinedID;
                        break;
                    }
            }

            // システム関数？
            var PT = default(GUI.POINTAPI);
            var in_window = default(bool);
            short x2, x1, y1, y2;
            DateTime d1, d2;
            var list = default(string[]);
            bool flag;
            switch (Strings.LCase(fname) ?? "")
            {
                // 多用される関数を先に判定
                case "args":
                    {
                        // UpVarコマンドの呼び出し回数を累計
                        num = Event_Renamed.UpVarLevel;
                        i = Event_Renamed.CallDepth;
                        while ((int)num > 0)
                        {
                            i = (short)(i - num);
                            if ((int)i < 1)
                            {
                                i = (short)1;
                                break;
                            }

                            num = Event_Renamed.UpVarLevelStack[(int)i];
                        }

                        if ((int)i < 1)
                        {
                            i = (short)1;
                        }

                        // 引数の範囲内に納まっているかチェック
                        num = (short)GetValueAsLong(ref @params[1], is_term[1]);
                        if (num <= (short)(Event_Renamed.ArgIndex - Event_Renamed.ArgIndexStack[(int)i - 1]))
                        {
                            str_result = Event_Renamed.ArgStack[Event_Renamed.ArgIndex - num + 1];
                        }

                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(ref str_result);
                            CallFunctionRet = ValueType.NumericType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.StringType;
                        }

                        return CallFunctionRet;
                    }

                case "call":
                    {
                        // サブルーチンの場所は？
                        // まずはサブルーチン名が式でないと仮定して検索
                        ret = Event_Renamed.FindNormalLabel(ref @params[1]);
                        if (ret == 0)
                        {
                            // 式で指定されている？
                            string arglname = GetValueAsString(ref @params[1], is_term[1]);
                            ret = Event_Renamed.FindNormalLabel(ref arglname);
                            if (ret == 0)
                            {
                                Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, "指定されたサブルーチン「" + @params[1] + "」が見つかりません");
                                return CallFunctionRet;
                            }
                        }

                        ret = ret + 1;

                        // 呼び出し階層をチェック
                        if (Event_Renamed.CallDepth > Event_Renamed.MaxCallDepth)
                        {
                            Event_Renamed.CallDepth = Event_Renamed.MaxCallDepth;
                            Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, GeneralLib.FormatNum((double)Event_Renamed.MaxCallDepth) + "階層を越えるサブルーチンの呼び出しは出来ません");
                            return CallFunctionRet;
                        }

                        // 引数用スタックが溢れないかチェック
                        if ((short)(Event_Renamed.ArgIndex + pcount) > Event_Renamed.MaxArgIndex)
                        {
                            Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, "サブルーチンの引数の総数が" + GeneralLib.FormatNum((double)Event_Renamed.MaxArgIndex) + "個を超えています");
                            return CallFunctionRet;
                        }

                        // 引数を評価しておく
                        var loopTo1 = pcount;
                        for (i = (short)2; i <= loopTo1; i++)
                            @params[(int)i] = GetValueAsString(ref @params[(int)i], is_term[(int)i]);

                        // 現在の状態を保存
                        Event_Renamed.CallStack[(int)Event_Renamed.CallDepth] = Event_Renamed.CurrentLineNum;
                        Event_Renamed.ArgIndexStack[(int)Event_Renamed.CallDepth] = Event_Renamed.ArgIndex;
                        Event_Renamed.VarIndexStack[(int)Event_Renamed.CallDepth] = Event_Renamed.VarIndex;
                        Event_Renamed.ForIndexStack[(int)Event_Renamed.CallDepth] = Event_Renamed.ForIndex;

                        // UpVarが実行された場合、UpVar実行数は累計する
                        if ((int)Event_Renamed.UpVarLevel > 0)
                        {
                            Event_Renamed.UpVarLevelStack[(int)Event_Renamed.CallDepth] = (short)(Event_Renamed.UpVarLevel + Event_Renamed.UpVarLevelStack[(int)Event_Renamed.CallDepth - 1]);
                        }
                        else
                        {
                            Event_Renamed.UpVarLevelStack[(int)Event_Renamed.CallDepth] = (short)0;
                        }

                        // UpVarの階層数を初期化
                        Event_Renamed.UpVarLevel = (short)0;

                        // 呼び出し階層数をインクリメント
                        Event_Renamed.CallDepth = (short)((int)Event_Renamed.CallDepth + 1);
                        cur_depth = Event_Renamed.CallDepth;

                        // 引数をスタックに積む
                        Event_Renamed.ArgIndex = (short)(Event_Renamed.ArgIndex + pcount - 1);
                        var loopTo2 = pcount;
                        for (i = (short)2; i <= loopTo2; i++)
                            Event_Renamed.ArgStack[Event_Renamed.ArgIndex - i + 2] = @params[(int)i];

                        // サブルーチン本体を実行
                        do
                        {
                            Event_Renamed.CurrentLineNum = ret;
                            if (Event_Renamed.CurrentLineNum > Information.UBound(Event_Renamed.EventCmd))
                            {
                                break;
                            }

                            {
                                var withBlock = Event_Renamed.EventCmd[Event_Renamed.CurrentLineNum];
                                if (cur_depth == Event_Renamed.CallDepth & withBlock.Name == Event_Renamed.CmdType.ReturnCmd)
                                {
                                    break;
                                }

                                ret = withBlock.Exec();
                            }
                        }
                        while (ret > 0);

                        // 返り値
                        {
                            var withBlock1 = Event_Renamed.EventCmd[Event_Renamed.CurrentLineNum];
                            if ((int)withBlock1.ArgNum == 2)
                            {
                                str_result = withBlock1.GetArgAsString((short)2);
                            }
                            else
                            {
                                str_result = "";
                            }
                        }

                        // 呼び出し階層数をデクリメント
                        Event_Renamed.CallDepth = (short)((int)Event_Renamed.CallDepth - 1);

                        // サブルーチン実行前の状態に復帰
                        Event_Renamed.CurrentLineNum = Event_Renamed.CallStack[(int)Event_Renamed.CallDepth];
                        Event_Renamed.ArgIndex = Event_Renamed.ArgIndexStack[(int)Event_Renamed.CallDepth];
                        Event_Renamed.VarIndex = Event_Renamed.VarIndexStack[(int)Event_Renamed.CallDepth];
                        Event_Renamed.ForIndex = Event_Renamed.ForIndexStack[(int)Event_Renamed.CallDepth];
                        Event_Renamed.UpVarLevel = Event_Renamed.UpVarLevelStack[(int)Event_Renamed.CallDepth];
                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(ref str_result);
                            CallFunctionRet = ValueType.NumericType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.StringType;
                        }

                        return CallFunctionRet;
                    }

                case "info":
                    {
                        var loopTo3 = pcount;
                        for (i = (short)1; i <= loopTo3; i++)
                            @params[(int)i] = GetValueAsString(ref @params[(int)i], is_term[(int)i]);
                        str_result = EvalInfoFunc(ref @params);
                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(ref str_result);
                            CallFunctionRet = ValueType.NumericType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.StringType;
                        }

                        return CallFunctionRet;
                    }

                case "instr":
                    {
                        if ((int)pcount == 2)
                        {
                            i = (short)Strings.InStr(GetValueAsString(ref @params[1], is_term[1]), GetValueAsString(ref @params[2], is_term[2]));
                        }
                        else
                        {
                            // params(3)が指定されている場合は、それを検索開始位置似設定
                            // VBのInStrは引数1が開始位置になりますが、現仕様との兼ね合いを考え、
                            // eve上では引数3に設定するようにしています
                            i = (short)Strings.InStr(GetValueAsLong(ref @params[3], is_term[3]), GetValueAsString(ref @params[1], is_term[1]), GetValueAsString(ref @params[2], is_term[2]));
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum((double)i);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = (double)i;
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "instrb":
                    {
                        if ((int)pcount == 2)
                        {
                            // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                            // UPGRADE_ISSUE: InStrB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                            i = InStrB(Strings.StrConv(GetValueAsString(ref @params[1], is_term[1]), vbFromUnicode), Strings.StrConv(GetValueAsString(ref @params[2], is_term[2]), vbFromUnicode));
                        }
                        else
                        {
                            // params(3)が指定されている場合は、それを検索開始位置似設定
                            // VBのInStrは引数1が開始位置になりますが、現仕様との兼ね合いを考え、
                            // eve上では引数3に設定するようにしています
                            // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                            // UPGRADE_ISSUE: InStrB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                            i = InStrB(GetValueAsLong(ref @params[3], is_term[3]), Strings.StrConv(GetValueAsString(ref @params[1], is_term[1]), vbFromUnicode), Strings.StrConv(GetValueAsString(ref @params[2], is_term[2]), vbFromUnicode));
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum((double)i);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = (double)i;
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "lindex":
                    {
                        string arglist = GetValueAsString(ref @params[1], is_term[1]);
                        str_result = GeneralLib.ListIndex(ref arglist, (short)GetValueAsLong(ref @params[2], is_term[2]));

                        // 全体が()で囲まれている場合は()を外す
                        if (Strings.Left(str_result, 1) == "(" & Strings.Right(str_result, 1) == ")")
                        {
                            str_result = Strings.Mid(str_result, 2, Strings.Len(str_result) - 2);
                        }

                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(ref str_result);
                            CallFunctionRet = ValueType.NumericType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.StringType;
                        }

                        return CallFunctionRet;
                    }

                case "llength":
                    {
                        string arglist1 = GetValueAsString(ref @params[1], is_term[1]);
                        i = GeneralLib.ListLength(ref arglist1);
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum((double)i);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = (double)i;
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "list":
                    {
                        str_result = GetValueAsString(ref @params[1], is_term[1]);
                        var loopTo4 = pcount;
                        for (i = (short)2; i <= loopTo4; i++)
                            str_result = str_result + " " + GetValueAsString(ref @params[(int)i], is_term[(int)i]);
                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                // これ以降はアルファベット順
                case "abs":
                    {
                        num_result = Math.Abs(GetValueAsDouble(ref @params[1], is_term[1]));
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "action":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex6 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex6))
                                    {
                                        Unit localItem2() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        num_result = (double)localItem2().Action;
                                    }
                                    else if (localIsDefined())
                                    {
                                        object argIndex5 = (object)pname;
                                        {
                                            var withBlock2 = SRC.PList.Item(ref argIndex5);
                                            if (withBlock2.Unit_Renamed is object)
                                            {
                                                {
                                                    var withBlock3 = withBlock2.Unit_Renamed;
                                                    if (withBlock3.Status_Renamed == "出撃" | withBlock3.Status_Renamed == "格納")
                                                    {
                                                        num_result = (double)withBlock3.Action;
                                                    }
                                                    else
                                                    {
                                                        num_result = 0d;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        num_result = (double)Event_Renamed.SelectedUnitForEvent.Action;
                                    }

                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "area":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined1() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex8 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex8))
                                    {
                                        Unit localItem21() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        str_result = localItem21().Area;
                                    }
                                    else if (localIsDefined1())
                                    {
                                        object argIndex7 = (object)pname;
                                        {
                                            var withBlock4 = SRC.PList.Item(ref argIndex7);
                                            if (withBlock4.Unit_Renamed is object)
                                            {
                                                str_result = withBlock4.Unit_Renamed.Area;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        str_result = Event_Renamed.SelectedUnitForEvent.Area;
                                    }

                                    break;
                                }
                        }

                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "asc":
                    {
                        num_result = (double)Strings.Asc(GetValueAsString(ref @params[1], is_term[1]));
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "atn":
                    {
                        num_result = Math.Atan(GetValueAsDouble(ref @params[1], is_term[1]));
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "chr":
                    {
                        str_result = Conversions.ToString((char)GetValueAsLong(ref @params[1], is_term[1]));
                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "condition":
                    {
                        switch (pcount)
                        {
                            case 2:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    buf = GetValueAsString(ref @params[2], is_term[2]);
                                    bool localIsDefined2() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex12 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex12))
                                    {
                                        Unit localItem22() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        object argIndex9 = (object)buf;
                                        if (localItem22().IsConditionSatisfied(ref argIndex9))
                                        {
                                            num_result = 1d;
                                        }
                                    }
                                    else if (localIsDefined2())
                                    {
                                        object argIndex11 = (object)pname;
                                        {
                                            var withBlock5 = SRC.PList.Item(ref argIndex11);
                                            if (withBlock5.Unit_Renamed is object)
                                            {
                                                object argIndex10 = (object)buf;
                                                if (withBlock5.Unit_Renamed.IsConditionSatisfied(ref argIndex10))
                                                {
                                                    num_result = 1d;
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        buf = GetValueAsString(ref @params[1], is_term[1]);
                                        object argIndex13 = (object)buf;
                                        if (Event_Renamed.SelectedUnitForEvent.IsConditionSatisfied(ref argIndex13))
                                        {
                                            num_result = 1d;
                                        }
                                    }

                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "count":
                    {
                        expr = Strings.Trim(expr);
                        buf = Strings.Mid(expr, 7, Strings.Len(expr) - 7) + "[";
                        num = (short)0;

                        // サブルーチンローカル変数を検索
                        if ((int)Event_Renamed.CallDepth > 0)
                        {
                            var loopTo5 = Event_Renamed.VarIndex;
                            for (i = (short)((int)Event_Renamed.VarIndexStack[(int)Event_Renamed.CallDepth - 1] + 1); i <= loopTo5; i++)
                            {
                                if (Strings.InStr(Event_Renamed.VarStack[(int)i].Name, buf) == 1)
                                {
                                    num = (short)((int)num + 1);
                                }
                            }

                            if ((int)num > 0)
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = GeneralLib.FormatNum((double)num);
                                    CallFunctionRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = (double)num;
                                    CallFunctionRet = ValueType.NumericType;
                                }

                                return CallFunctionRet;
                            }
                        }

                        // ローカル変数を検索
                        foreach (VarData currentVar in Event_Renamed.LocalVariableList)
                        {
                            var = currentVar;
                            if (Strings.InStr(var.Name, buf) == 1)
                            {
                                num = (short)((int)num + 1);
                            }
                        }

                        if ((int)num > 0)
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = GeneralLib.FormatNum((double)num);
                                CallFunctionRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = (double)num;
                                CallFunctionRet = ValueType.NumericType;
                            }

                            return CallFunctionRet;
                        }

                        // グローバル変数を検索
                        foreach (VarData currentVar1 in Event_Renamed.GlobalVariableList)
                        {
                            var = currentVar1;
                            if (Strings.InStr(var.Name, buf) == 1)
                            {
                                num = (short)((int)num + 1);
                            }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum((double)num);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = (double)num;
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "countitem":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined3() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex15 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex15))
                                    {
                                        Unit localItem23() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        num = localItem23().CountItem();
                                    }
                                    else if (!localIsDefined3())
                                    {
                                        if (pname == "未装備")
                                        {
                                            num = (short)0;
                                            foreach (Item currentIt in SRC.IList)
                                            {
                                                it = currentIt;
                                                if (it.Unit_Renamed is null & it.Exist)
                                                {
                                                    num = (short)((int)num + 1);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        object argIndex14 = (object)pname;
                                        {
                                            var withBlock6 = SRC.PList.Item(ref argIndex14);
                                            if (withBlock6.Unit_Renamed is object)
                                            {
                                                num = withBlock6.Unit_Renamed.CountItem();
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        num = Event_Renamed.SelectedUnitForEvent.CountItem();
                                    }

                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum((double)num);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = (double)num;
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "countpartner":
                    {
                        num_result = (double)Information.UBound(Commands.SelectedPartners);
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "countpilot":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    object argIndex18 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex18))
                                    {
                                        object argIndex16 = (object)pname;
                                        {
                                            var withBlock7 = SRC.UList.Item2(ref argIndex16);
                                            num_result = (double)(withBlock7.CountPilot() + withBlock7.CountSupport());
                                        }
                                    }
                                    else
                                    {
                                        object argIndex17 = (object)pname;
                                        {
                                            var withBlock8 = SRC.PList.Item(ref argIndex17);
                                            if (withBlock8.Unit_Renamed is object)
                                            {
                                                {
                                                    var withBlock9 = withBlock8.Unit_Renamed;
                                                    num_result = (double)(withBlock9.CountPilot() + withBlock9.CountSupport());
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        {
                                            var withBlock10 = Event_Renamed.SelectedUnitForEvent;
                                            num_result = (double)(withBlock10.CountPilot() + withBlock10.CountSupport());
                                        }
                                    }

                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "cos":
                    {
                        num_result = Math.Cos(GetValueAsDouble(ref @params[1], is_term[1]));
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "damage":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined4() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    Pilot localItem1() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                    object argIndex20 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex20))
                                    {
                                        object argIndex19 = (object)pname;
                                        {
                                            var withBlock11 = SRC.UList.Item2(ref argIndex19);
                                            num_result = (double)(100 * (withBlock11.MaxHP - withBlock11.HP) / withBlock11.MaxHP);
                                        }
                                    }
                                    else if (!localIsDefined4())
                                    {
                                        num_result = 100d;
                                    }
                                    else if (localItem1().Unit_Renamed is null)
                                    {
                                        num_result = 100d;
                                    }
                                    else
                                    {
                                        Pilot localItem() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                        {
                                            var withBlock12 = localItem().Unit_Renamed;
                                            num_result = (double)(100 * (withBlock12.MaxHP - withBlock12.HP) / withBlock12.MaxHP);
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    {
                                        var withBlock13 = Event_Renamed.SelectedUnitForEvent;
                                        num_result = (double)(100 * (withBlock13.MaxHP - withBlock13.HP) / withBlock13.MaxHP);
                                    }

                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "dir":
                    {
                        CallFunctionRet = ValueType.StringType;
                        switch (pcount)
                        {
                            case 2:
                                {
                                    fname = GetValueAsString(ref @params[1], is_term[1]);

                                    // フルパス指定でなければシナリオフォルダを起点に検索
                                    if (Strings.Mid(fname, 2, 1) != ":")
                                    {
                                        fname = SRC.ScenarioPath + fname;
                                    }

                                    switch (GetValueAsString(ref @params[2], is_term[2]) ?? "")
                                    {
                                        case "ファイル":
                                            {
                                                // UPGRADE_ISSUE: vbNormal をアップグレードする定数を決定できません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3B44E51-B5F1-4FD7-AA29-CAD31B71F487"' をクリックしてください。
                                                num = (short)Constants.vbNormal;
                                                break;
                                            }

                                        case "フォルダ":
                                            {
                                                num = (short)FileAttribute.Directory;
                                                break;
                                            }
                                    }
                                    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                                    str_result = FileSystem.Dir(fname, (FileAttribute)num);
                                    if (Strings.Len(str_result) == 0)
                                    {
                                        return CallFunctionRet;
                                    }

                                    // ファイル属性チェック用に検索パスを作成
                                    dir_path = fname;
                                    if ((int)num == (int)FileAttribute.Directory)
                                    {
                                        string argstr2 = @"\";
                                        i = GeneralLib.InStr2(ref fname, ref argstr2);
                                        if ((int)i > 0)
                                        {
                                            dir_path = Strings.Left(fname, (int)i);
                                        }
                                    }

                                    // 単一ファイルの検索？
                                    if (Strings.InStr(fname, "*") == 0)
                                    {
                                        // フォルダの検索の場合は見つかったファイルがフォルダ
                                        // かどうかチェックする
                                        if ((int)num == (int)FileAttribute.Directory)
                                        {
                                            if (((int)FileSystem.GetAttr(dir_path + str_result) & (int)num) == 0)
                                            {
                                                str_result = "";
                                            }
                                        }

                                        return CallFunctionRet;
                                    }

                                    if (str_result == ".")
                                    {
                                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                                        str_result = FileSystem.Dir();
                                    }

                                    if (str_result == "..")
                                    {
                                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                                        str_result = FileSystem.Dir();
                                    }

                                    // 検索されたファイル一覧を作成
                                    dir_list = new string[1];
                                    if ((int)num == (int)FileAttribute.Directory)
                                    {
                                        while (Strings.Len(str_result) > 0)
                                        {
                                            // フォルダの検索の場合は見つかったファイルがフォルダ
                                            // かどうかチェックする
                                            if (((int)FileSystem.GetAttr(dir_path + str_result) & (int)num) != 0)
                                            {
                                                Array.Resize(ref dir_list, Information.UBound(dir_list) + 1 + 1);
                                                dir_list[Information.UBound(dir_list)] = str_result;
                                            }
                                            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                                            str_result = FileSystem.Dir();
                                        }
                                    }
                                    else
                                    {
                                        while (Strings.Len(str_result) > 0)
                                        {
                                            Array.Resize(ref dir_list, Information.UBound(dir_list) + 1 + 1);
                                            dir_list[Information.UBound(dir_list)] = str_result;
                                            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                                            str_result = FileSystem.Dir();
                                        }
                                    }

                                    if (Information.UBound(dir_list) > 0)
                                    {
                                        str_result = dir_list[1];
                                        dir_index = (short)2;
                                    }
                                    else
                                    {
                                        str_result = "";
                                        dir_index = (short)1;
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    fname = GetValueAsString(ref @params[1], is_term[1]);

                                    // フルパス指定でなければシナリオフォルダを起点に検索
                                    if (Strings.Mid(fname, 2, 1) != ":")
                                    {
                                        fname = SRC.ScenarioPath + fname;
                                    }

                                    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                                    str_result = FileSystem.Dir(fname, FileAttribute.Directory);
                                    if (Strings.Len(str_result) == 0)
                                    {
                                        return CallFunctionRet;
                                    }

                                    // 単一ファイルの検索？
                                    if (Strings.InStr(fname, "*") == 0)
                                    {
                                        return CallFunctionRet;
                                    }

                                    if (str_result == ".")
                                    {
                                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                                        str_result = FileSystem.Dir();
                                    }

                                    if (str_result == "..")
                                    {
                                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                                        str_result = FileSystem.Dir();
                                    }

                                    // 検索されたファイル一覧を作成
                                    dir_list = new string[1];
                                    while (Strings.Len(str_result) > 0)
                                    {
                                        Array.Resize(ref dir_list, Information.UBound(dir_list) + 1 + 1);
                                        dir_list[Information.UBound(dir_list)] = str_result;
                                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                                        str_result = FileSystem.Dir();
                                    }

                                    if (Information.UBound(dir_list) > 0)
                                    {
                                        str_result = dir_list[1];
                                        dir_index = (short)2;
                                    }
                                    else
                                    {
                                        str_result = "";
                                        dir_index = (short)1;
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if ((int)dir_index <= Information.UBound(dir_list))
                                    {
                                        str_result = dir_list[(int)dir_index];
                                        dir_index = (short)((int)dir_index + 1);
                                    }
                                    else
                                    {
                                        str_result = "";
                                    }

                                    break;
                                }
                        }

                        return CallFunctionRet;
                    }

                case "eof":
                    {
                        if (etype == ValueType.StringType)
                        {
                            if (FileSystem.EOF(GetValueAsLong(ref @params[1], is_term[1])))
                            {
                                str_result = "1";
                            }
                            else
                            {
                                str_result = "0";
                            }

                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            if (FileSystem.EOF(GetValueAsLong(ref @params[1], is_term[1])))
                            {
                                num_result = 1d;
                            }

                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "en":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined5() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex22 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex22))
                                    {
                                        Unit localItem24() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        num_result = (double)localItem24().EN;
                                    }
                                    else if (localIsDefined5())
                                    {
                                        object argIndex21 = (object)pname;
                                        {
                                            var withBlock14 = SRC.PList.Item(ref argIndex21);
                                            if (withBlock14.Unit_Renamed is object)
                                            {
                                                num_result = (double)withBlock14.Unit_Renamed.EN;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        num_result = (double)Event_Renamed.SelectedUnitForEvent.EN;
                                    }

                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "eval":
                    {
                        buf = Strings.Trim(GetValueAsString(ref @params[1], is_term[1]));
                        CallFunctionRet = EvalExpr(ref buf, ref etype, ref str_result, ref num_result);
                        return CallFunctionRet;
                    }

                case "font":
                    {
                        switch (GetValueAsString(ref @params[1], is_term[1]) ?? "")
                        {
                            case "フォント名":
                                {
                                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    str_result = GUI.MainForm.picMain(0).Font.Name;
                                    CallFunctionRet = ValueType.StringType;
                                    break;
                                }

                            case "サイズ":
                                {
                                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    num_result = GUI.MainForm.picMain(0).Font.Size;
                                    if (etype == ValueType.StringType)
                                    {
                                        str_result = GeneralLib.FormatNum(num_result);
                                        CallFunctionRet = ValueType.StringType;
                                    }
                                    else
                                    {
                                        CallFunctionRet = ValueType.NumericType;
                                    }

                                    break;
                                }

                            case "太字":
                                {
                                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    if (GUI.MainForm.picMain(0).Font.Bold)
                                    {
                                        num_result = 1d;
                                    }
                                    else
                                    {
                                        num_result = 0d;
                                    }

                                    if (etype == ValueType.StringType)
                                    {
                                        str_result = GeneralLib.FormatNum(num_result);
                                        CallFunctionRet = ValueType.StringType;
                                    }
                                    else
                                    {
                                        CallFunctionRet = ValueType.NumericType;
                                    }

                                    break;
                                }

                            case "斜体":
                                {
                                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    if (GUI.MainForm.picMain(0).Font.Italic)
                                    {
                                        num_result = 1d;
                                    }
                                    else
                                    {
                                        num_result = 0d;
                                    }

                                    if (etype == ValueType.StringType)
                                    {
                                        str_result = GeneralLib.FormatNum(num_result);
                                        CallFunctionRet = ValueType.StringType;
                                    }
                                    else
                                    {
                                        CallFunctionRet = ValueType.NumericType;
                                    }

                                    break;
                                }

                            case "色":
                                {
                                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    str_result = Hex(GUI.MainForm.picMain(0).ForeColor);
                                    var loopTo6 = (short)(6 - Strings.Len(str_result));
                                    for (i = (short)1; i <= loopTo6; i++)
                                        str_result = "0" + str_result;
                                    str_result = "#" + str_result;
                                    CallFunctionRet = ValueType.StringType;
                                    break;
                                }

                            case "書き込み":
                                {
                                    if (GUI.PermanentStringMode)
                                    {
                                        str_result = "背景";
                                    }
                                    else if (GUI.KeepStringMode)
                                    {
                                        str_result = "保持";
                                    }
                                    else
                                    {
                                        str_result = "通常";
                                    }

                                    CallFunctionRet = ValueType.StringType;
                                    break;
                                }
                        }

                        return CallFunctionRet;
                    }

                case "format":
                    {
                        str_result = VB.Compatibility.VB6.Support.Format(GetValueAsString(ref @params[1], is_term[1]), GetValueAsString(ref @params[2], is_term[2]));
                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(ref str_result);
                            CallFunctionRet = ValueType.NumericType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.StringType;
                        }

                        return CallFunctionRet;
                    }

                case "keystate":
                    {
                        if ((int)pcount != 1)
                        {
                            return CallFunctionRet;
                        }

                        // キー番号
                        i = (short)GetValueAsLong(ref @params[1], is_term[1]);

                        // 左利き設定に対応
                        switch (i)
                        {
                            case (short)Keys.LButton:
                                {
                                    i = (short)GUI.LButtonID;
                                    break;
                                }

                            case (short)Keys.RButton:
                                {
                                    i = (short)GUI.RButtonID;
                                    break;
                                }
                        }

                        if ((int)i == (int)Keys.LButton | (int)i == (int)Keys.RButton)
                        {
                            // マウスカーソルの位置を参照
                            GUI.GetCursorPos(ref PT);

                            // メインウインドウ上でマウスボタンを押している？
                            if (ReferenceEquals(Form.ActiveForm, GUI.MainForm))
                            {
                                {
                                    var withBlock15 = GUI.MainForm;
                                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    x1 = (long)VB.Compatibility.VB6.Support.PixelsToTwipsX((double)withBlock15.Left) / (long)VB.Compatibility.VB6.Support.TwipsPerPixelX() + withBlock15.picMain(0).Left + 3;
                                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    y1 = (long)VB.Compatibility.VB6.Support.PixelsToTwipsY((double)withBlock15.Top) / (long)VB.Compatibility.VB6.Support.TwipsPerPixelY() + withBlock15.picMain(0).Top + 28;
                                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    x2 = x1 + withBlock15.picMain(0).Width;
                                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    y2 = y1 + withBlock15.picMain(0).Height;
                                }

                                if ((int)x1 <= PT.X & PT.X <= (int)x2 & (int)y1 <= PT.Y & PT.Y <= (int)y2)
                                {
                                    in_window = true;
                                }
                            }
                        }
                        // メインウィンドウがアクティブになっている？
                        else if (ReferenceEquals(Form.ActiveForm, GUI.MainForm))
                        {
                            in_window = true;
                        }

                        // ウィンドウが選択されていない場合は常に0を返す
                        if (!in_window)
                        {
                            num_result = 0d;
                            if (etype == ValueType.StringType)
                            {
                                str_result = "0";
                                CallFunctionRet = ValueType.StringType;
                            }
                            else
                            {
                                CallFunctionRet = ValueType.NumericType;
                            }

                            return CallFunctionRet;
                        }

                        // キーの状態を参照
                        if (Conversions.ToBoolean((int)GUI.GetAsyncKeyState((int)i) & 0x8000))
                        {
                            num_result = 1d;
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "gettime":
                    {
                        num_result = (double)GeneralLib.timeGetTime();
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "hp":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined6() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex24 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex24))
                                    {
                                        Unit localItem25() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        num_result = (double)localItem25().HP;
                                    }
                                    else if (localIsDefined6())
                                    {
                                        object argIndex23 = (object)pname;
                                        {
                                            var withBlock16 = SRC.PList.Item(ref argIndex23);
                                            if (withBlock16.Unit_Renamed is object)
                                            {
                                                num_result = (double)withBlock16.Unit_Renamed.HP;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        num_result = (double)Event_Renamed.SelectedUnitForEvent.HP;
                                    }

                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "iif":
                    {
                        num = GeneralLib.ListSplit(ref @params[1], ref list);
                        switch (num)
                        {
                            case 1:
                                {
                                    var tmp1 = list;
                                    object argIndex26 = (object)tmp1[1];
                                    if (SRC.PList.IsDefined(ref argIndex26))
                                    {
                                        var tmp = list;
                                        object argIndex25 = (object)tmp[1];
                                        {
                                            var withBlock17 = SRC.PList.Item(ref argIndex25);
                                            if (withBlock17.Unit_Renamed is null)
                                            {
                                                flag = false;
                                            }
                                            else
                                            {
                                                {
                                                    var withBlock18 = withBlock17.Unit_Renamed;
                                                    if (withBlock18.Status_Renamed == "出撃" | withBlock18.Status_Renamed == "格納")
                                                    {
                                                        flag = true;
                                                    }
                                                    else
                                                    {
                                                        flag = false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (GetValueAsLong(ref @params[1]) != 0)
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        flag = false;
                                    }

                                    break;
                                }

                            case 2:
                                {
                                    pname = GeneralLib.ListIndex(ref expr, (short)2);
                                    var tmp3 = list;
                                    object argIndex28 = (object)tmp3[2];
                                    if (SRC.PList.IsDefined(ref argIndex28))
                                    {
                                        var tmp2 = list;
                                        object argIndex27 = (object)tmp2[2];
                                        {
                                            var withBlock19 = SRC.PList.Item(ref argIndex27);
                                            if (withBlock19.Unit_Renamed is null)
                                            {
                                                flag = true;
                                            }
                                            else
                                            {
                                                {
                                                    var withBlock20 = withBlock19.Unit_Renamed;
                                                    if (withBlock20.Status_Renamed == "出撃" | withBlock20.Status_Renamed == "格納")
                                                    {
                                                        flag = false;
                                                    }
                                                    else
                                                    {
                                                        flag = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (GetValueAsLong(ref @params[1], true) == 0)
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        flag = false;
                                    }

                                    break;
                                }

                            default:
                                {
                                    if (GetValueAsLong(ref @params[1]) != 0)
                                    {
                                        flag = true;
                                    }
                                    else
                                    {
                                        flag = false;
                                    }

                                    break;
                                }
                        }

                        if (flag)
                        {
                            str_result = GetValueAsString(ref @params[2], is_term[2]);
                        }
                        else
                        {
                            str_result = GetValueAsString(ref @params[3], is_term[3]);
                        }

                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(ref str_result);
                            CallFunctionRet = ValueType.NumericType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.StringType;
                        }

                        return CallFunctionRet;
                    }

                case "instrrev":
                    {
                        buf = GetValueAsString(ref @params[1], is_term[1]);
                        buf2 = GetValueAsString(ref @params[2], is_term[2]);
                        if (Strings.Len(buf2) > 0 & Strings.Len(buf) >= Strings.Len(buf2))
                        {
                            if ((int)pcount == 2)
                            {
                                num = (short)Strings.Len(buf);
                            }
                            else
                            {
                                num = (short)GetValueAsLong(ref @params[3], is_term[3]);
                            }

                            i = (short)((int)num - Strings.Len(buf2) + 1);
                            do
                            {
                                j = (short)Strings.InStr((int)i, buf, buf2);
                                if (i == j)
                                {
                                    break;
                                }

                                i = (short)((int)i - 1);
                            }
                            while ((int)i != 0);
                        }
                        else
                        {
                            i = (short)0;
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum((double)i);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = (double)i;
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "instrrevb":
                    {
                        buf = GetValueAsString(ref @params[1], is_term[1]);
                        buf2 = GetValueAsString(ref @params[2], is_term[2]);

                        // UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                        if (LenB(buf2) > 0 & LenB(buf) >= LenB(buf2))
                        {
                            if ((int)pcount == 2)
                            {
                                // UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                                num = LenB(buf);
                            }
                            else
                            {
                                num = (short)GetValueAsLong(ref @params[3], is_term[3]);
                            }

                            // UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                            i = num - LenB(buf2) + 1;
                            do
                            {
                                // UPGRADE_ISSUE: InStrB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                                j = InStrB(i, buf, buf2);
                                if (i == j)
                                {
                                    break;
                                }

                                i = (short)((int)i - 1);
                            }
                            while ((int)i != 0);
                        }
                        else
                        {
                            i = (short)0;
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum((double)i);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = (double)i;
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "int":
                    {
                        num_result = Conversion.Int(GetValueAsDouble(ref @params[1], is_term[1]));
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "isavailable":
                    {
                        switch (pcount)
                        {
                            case 2:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    buf = GetValueAsString(ref @params[2], is_term[2]);

                                    // エリアスが定義されている？
                                    object argIndex30 = (object)buf;
                                    if (SRC.ALDList.IsDefined(ref argIndex30))
                                    {
                                        object argIndex29 = (object)buf;
                                        {
                                            var withBlock21 = SRC.ALDList.Item(ref argIndex29);
                                            var loopTo7 = withBlock21.Count;
                                            for (i = (short)1; i <= loopTo7; i++)
                                            {
                                                string localLIndex() { string arglist = withBlock21.get_AliasData(i); var ret = GeneralLib.LIndex(ref arglist, (short)1); withBlock21.get_AliasData(i) = arglist; return ret; }

                                                if ((localLIndex() ?? "") == (buf ?? ""))
                                                {
                                                    buf = withBlock21.get_AliasType(i);
                                                    break;
                                                }
                                            }

                                            if (i > withBlock21.Count)
                                            {
                                                buf = withBlock21.get_AliasType((short)1);
                                            }
                                        }
                                    }

                                    bool localIsDefined7() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex32 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex32))
                                    {
                                        Unit localItem26() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        if (localItem26().IsFeatureAvailable(ref buf))
                                        {
                                            num_result = 1d;
                                        }
                                    }
                                    else if (localIsDefined7())
                                    {
                                        object argIndex31 = (object)pname;
                                        {
                                            var withBlock22 = SRC.PList.Item(ref argIndex31);
                                            if (withBlock22.Unit_Renamed is object)
                                            {
                                                if (withBlock22.Unit_Renamed.IsFeatureAvailable(ref buf))
                                                {
                                                    num_result = 1d;
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    buf = GetValueAsString(ref @params[1], is_term[1]);

                                    // エリアスが定義されている？
                                    object argIndex33 = (object)buf;
                                    if (SRC.ALDList.IsDefined(ref argIndex33))
                                    {
                                        AliasDataType localItem3() { object argIndex1 = (object)buf; var ret = SRC.ALDList.Item(ref argIndex1); return ret; }

                                        buf = localItem3().get_AliasType((short)1);
                                    }

                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        if (Event_Renamed.SelectedUnitForEvent.IsFeatureAvailable(ref buf))
                                        {
                                            num_result = 1d;
                                        }
                                    }

                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "isdefined":
                    {
                        pname = GetValueAsString(ref @params[1], is_term[1]);
                        switch (pcount)
                        {
                            case 2:
                                {
                                    switch (GetValueAsString(ref @params[2], is_term[2]) ?? "")
                                    {
                                        case "パイロット":
                                            {
                                                object argIndex34 = (object)pname;
                                                if (SRC.PList.IsDefined(ref argIndex34))
                                                {
                                                    Pilot localItem4() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                                    if (localItem4().Alive)
                                                    {
                                                        num_result = 1d;
                                                    }
                                                }

                                                break;
                                            }

                                        case "ユニット":
                                            {
                                                object argIndex35 = (object)pname;
                                                if (SRC.UList.IsDefined(ref argIndex35))
                                                {
                                                    Unit localItem5() { object argIndex1 = (object)pname; var ret = SRC.UList.Item(ref argIndex1); return ret; }

                                                    if (localItem5().Status_Renamed != "破棄")
                                                    {
                                                        num_result = 1d;
                                                    }
                                                }

                                                break;
                                            }

                                        case "アイテム":
                                            {
                                                object argIndex36 = (object)pname;
                                                if (SRC.IList.IsDefined(ref argIndex36))
                                                {
                                                    num_result = 1d;
                                                }

                                                break;
                                            }
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    bool localIsDefined8() { object argIndex1 = (object)pname; var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

                                    bool localIsDefined9() { object argIndex1 = (object)pname; var ret = SRC.IList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex37 = (object)pname;
                                    if (SRC.PList.IsDefined(ref argIndex37))
                                    {
                                        Pilot localItem6() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                        if (localItem6().Alive)
                                        {
                                            num_result = 1d;
                                        }
                                    }
                                    else if (localIsDefined8())
                                    {
                                        Unit localItem7() { object argIndex1 = (object)pname; var ret = SRC.UList.Item(ref argIndex1); return ret; }

                                        if (localItem7().Status_Renamed != "破棄")
                                        {
                                            num_result = 1d;
                                        }
                                    }
                                    else if (localIsDefined9())
                                    {
                                        num_result = 1d;
                                    }

                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "isequiped":
                    {
                        switch (pcount)
                        {
                            case 2:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined10() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex39 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex39))
                                    {
                                        Unit localItem27() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        string arginame = GetValueAsString(ref @params[2], is_term[2]);
                                        if (localItem27().IsEquiped(ref arginame))
                                        {
                                            num_result = 1d;
                                        }
                                    }
                                    else if (localIsDefined10())
                                    {
                                        object argIndex38 = (object)pname;
                                        {
                                            var withBlock23 = SRC.PList.Item(ref argIndex38);
                                            if (withBlock23.Unit_Renamed is object)
                                            {
                                                string arginame1 = GetValueAsString(ref @params[2], is_term[2]);
                                                if (withBlock23.Unit_Renamed.IsEquiped(ref arginame1))
                                                {
                                                    num_result = 1d;
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        string arginame2 = GetValueAsString(ref @params[1], is_term[1]);
                                        if (Event_Renamed.SelectedUnitForEvent.IsEquiped(ref arginame2))
                                        {
                                            num_result = 1d;
                                        }
                                    }

                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "lsearch":
                    {
                        buf = GetValueAsString(ref @params[1], is_term[1]);
                        buf2 = GetValueAsString(ref @params[2], is_term[2]);
                        num = Conversions.ToShort(Interaction.IIf((int)pcount < 3, (object)1, (object)GetValueAsLong(ref @params[3], is_term[3])));
                        num2 = GeneralLib.ListLength(ref buf);
                        var loopTo8 = num2;
                        for (i = num; i <= loopTo8; i++)
                        {
                            if ((GeneralLib.ListIndex(ref buf, i) ?? "") == (buf2 ?? ""))
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format((object)i);
                                    CallFunctionRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = (double)i;
                                    CallFunctionRet = ValueType.NumericType;
                                }

                                return CallFunctionRet;
                            }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = "0";
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = 0d;
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "isnumeric":
                    {
                        string argstr_Renamed = GetValueAsString(ref @params[1], is_term[1]);
                        if (GeneralLib.IsNumber(ref argstr_Renamed))
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = "1";
                                CallFunctionRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = 1d;
                                CallFunctionRet = ValueType.NumericType;
                            }
                        }
                        else if (etype == ValueType.StringType)
                        {
                            str_result = "0";
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = 0d;
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "isvardefined":
                    {
                        string argvar_name = Strings.Trim(Strings.Mid(expr, 14, Strings.Len(expr) - 14));
                        if (IsVariableDefined(ref argvar_name))
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = "1";
                                CallFunctionRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = 1d;
                                CallFunctionRet = ValueType.NumericType;
                            }
                        }
                        else if (etype == ValueType.StringType)
                        {
                            str_result = "0";
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = 0d;
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "item":
                    {
                        switch (pcount)
                        {
                            case 2:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined11() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    Pilot localItem11() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                    Pilot localItem12() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                    object argIndex41 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex41))
                                    {
                                        i = (short)GetValueAsLong(ref @params[2], is_term[2]);
                                        object argIndex40 = (object)pname;
                                        {
                                            var withBlock24 = SRC.UList.Item2(ref argIndex40);
                                            if (1 <= (int)i & i <= withBlock24.CountItem())
                                            {
                                                Item localItem8() { object argIndex1 = (object)i; var ret = withBlock24.Item(ref argIndex1); return ret; }

                                                str_result = localItem8().Name;
                                            }
                                        }
                                    }
                                    else if (!localIsDefined11())
                                    {
                                        if (pname == "未装備")
                                        {
                                            i = (short)0;
                                            j = (short)GetValueAsLong(ref @params[2], is_term[2]);
                                            foreach (Item currentIt1 in SRC.IList)
                                            {
                                                it = currentIt1;
                                                if (it.Unit_Renamed is null & it.Exist)
                                                {
                                                    i = (short)((int)i + 1);
                                                    if (i == j)
                                                    {
                                                        str_result = it.Name;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (localItem12().Unit_Renamed is object)
                                    {
                                        i = (short)GetValueAsLong(ref @params[2], is_term[2]);
                                        Pilot localItem10() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                        {
                                            var withBlock25 = localItem10().Unit_Renamed;
                                            if (1 <= (int)i & i <= withBlock25.CountItem())
                                            {
                                                Item localItem9() { object argIndex1 = (object)i; var ret = withBlock25.Item(ref argIndex1); return ret; }

                                                str_result = localItem9().Name;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        i = (short)GetValueAsLong(ref @params[1], is_term[1]);
                                        {
                                            var withBlock26 = Event_Renamed.SelectedUnitForEvent;
                                            if (1 <= (int)i & i <= withBlock26.CountItem())
                                            {
                                                Item localItem13() { object argIndex1 = (object)i; var ret = withBlock26.Item(ref argIndex1); return ret; }

                                                str_result = localItem13().Name;
                                            }
                                        }
                                    }

                                    break;
                                }
                        }

                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "itemid":
                    {
                        switch (pcount)
                        {
                            case 2:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined12() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    Pilot localItem17() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                    Pilot localItem18() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                    object argIndex43 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex43))
                                    {
                                        i = (short)GetValueAsLong(ref @params[2], is_term[2]);
                                        object argIndex42 = (object)pname;
                                        {
                                            var withBlock27 = SRC.UList.Item2(ref argIndex42);
                                            if (1 <= (int)i & i <= withBlock27.CountItem())
                                            {
                                                Item localItem14() { object argIndex1 = (object)i; var ret = withBlock27.Item(ref argIndex1); return ret; }

                                                str_result = localItem14().ID;
                                            }
                                        }
                                    }
                                    else if (!localIsDefined12())
                                    {
                                        if (pname == "未装備")
                                        {
                                            i = (short)0;
                                            j = (short)GetValueAsLong(ref @params[2], is_term[2]);
                                            foreach (Item currentIt2 in SRC.IList)
                                            {
                                                it = currentIt2;
                                                if (it.Unit_Renamed is null & it.Exist)
                                                {
                                                    i = (short)((int)i + 1);
                                                    if (i == j)
                                                    {
                                                        str_result = it.ID;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (localItem18().Unit_Renamed is object)
                                    {
                                        i = (short)GetValueAsLong(ref @params[2], is_term[2]);
                                        Pilot localItem16() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                        {
                                            var withBlock28 = localItem16().Unit_Renamed;
                                            if (1 <= (int)i & i <= withBlock28.CountItem())
                                            {
                                                Item localItem15() { object argIndex1 = (object)i; var ret = withBlock28.Item(ref argIndex1); return ret; }

                                                str_result = localItem15().ID;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        i = (short)GetValueAsLong(ref @params[1], is_term[1]);
                                        {
                                            var withBlock29 = Event_Renamed.SelectedUnitForEvent;
                                            if (1 <= (int)i & i <= withBlock29.CountItem())
                                            {
                                                Item localItem19() { object argIndex1 = (object)i; var ret = withBlock29.Item(ref argIndex1); return ret; }

                                                str_result = localItem19().ID;
                                            }
                                        }
                                    }

                                    break;
                                }
                        }

                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "left":
                    {
                        str_result = Strings.Left(GetValueAsString(ref @params[1], is_term[1]), GetValueAsLong(ref @params[2], is_term[2]));
                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(ref str_result);
                            CallFunctionRet = ValueType.NumericType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.StringType;
                        }

                        return CallFunctionRet;
                    }

                case "leftb":
                    {
                        buf = GetValueAsString(ref @params[1], is_term[1]);
                        // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                        // UPGRADE_ISSUE: LeftB$ 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                        str_result = LeftB(Strings.StrConv(buf, vbFromUnicode), GetValueAsLong(ref @params[2], is_term[2]));
                        // UPGRADE_ISSUE: 定数 vbUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                        str_result = Strings.StrConv(str_result, vbUnicode);
                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(ref str_result);
                            CallFunctionRet = ValueType.NumericType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.StringType;
                        }

                        return CallFunctionRet;
                    }

                case "len":
                    {
                        num_result = (double)Strings.Len(GetValueAsString(ref @params[1], is_term[1]));
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "lenb":
                    {
                        buf = GetValueAsString(ref @params[1], is_term[1]);
                        // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                        // UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                        num_result = LenB(Strings.StrConv(buf, vbFromUnicode));
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "level":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined13() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex44 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex44))
                                    {
                                        Unit localItem28() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        num = localItem28().MainPilot().Level;
                                    }
                                    else if (!localIsDefined13())
                                    {
                                        num_result = 0d;
                                    }
                                    else
                                    {
                                        Pilot localItem20() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                        num_result = (double)localItem20().Level;
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        {
                                            var withBlock30 = Event_Renamed.SelectedUnitForEvent;
                                            if ((int)withBlock30.CountPilot() > 0)
                                            {
                                                num_result = (double)withBlock30.MainPilot().Level;
                                            }
                                        }
                                    }

                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "lcase":
                    {
                        str_result = Strings.LCase(GetValueAsString(ref @params[1], is_term[1]));
                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "lset":
                    {
                        buf = GetValueAsString(ref @params[1], is_term[1]);
                        i = (short)GetValueAsLong(ref @params[2], is_term[2]);
                        // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                        // UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                        if (LenB(Strings.StrConv(buf, vbFromUnicode)) < i)
                        {
                            // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                            // UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                            str_result = buf + Strings.Space(i - LenB(Strings.StrConv(buf, vbFromUnicode)));
                        }
                        else
                        {
                            str_result = buf;
                        }

                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "max":
                    {
                        num_result = GetValueAsDouble(ref @params[1], is_term[1]);
                        var loopTo9 = pcount;
                        for (i = (short)2; i <= loopTo9; i++)
                        {
                            rdbl = GetValueAsDouble(ref @params[(int)i], is_term[(int)i]);
                            if (num_result < rdbl)
                            {
                                num_result = rdbl;
                            }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "mid":
                    {
                        buf = GetValueAsString(ref @params[1], is_term[1]);
                        switch (pcount)
                        {
                            case 3:
                                {
                                    i = (short)GetValueAsLong(ref @params[2], is_term[2]);
                                    j = (short)GetValueAsLong(ref @params[3], is_term[3]);
                                    str_result = Strings.Mid(buf, (int)i, (int)j);
                                    break;
                                }

                            case 2:
                                {
                                    i = (short)GetValueAsLong(ref @params[2], is_term[2]);
                                    str_result = Strings.Mid(buf, (int)i);
                                    break;
                                }
                        }

                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(ref str_result);
                            CallFunctionRet = ValueType.NumericType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.StringType;
                        }

                        return CallFunctionRet;
                    }

                case "midb":
                    {
                        buf = GetValueAsString(ref @params[1], is_term[1]);
                        switch (pcount)
                        {
                            case 3:
                                {
                                    i = (short)GetValueAsLong(ref @params[2], is_term[2]);
                                    j = (short)GetValueAsLong(ref @params[3], is_term[3]);
                                    // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                                    // UPGRADE_ISSUE: MidB$ 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                                    str_result = MidB(Strings.StrConv(buf, vbFromUnicode), i, j);
                                    break;
                                }

                            case 2:
                                {
                                    i = (short)GetValueAsLong(ref @params[2], is_term[2]);
                                    // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                                    // UPGRADE_ISSUE: MidB$ 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                                    str_result = MidB(Strings.StrConv(buf, vbFromUnicode), i);
                                    break;
                                }
                        }
                        // UPGRADE_ISSUE: 定数 vbUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                        str_result = Strings.StrConv(str_result, vbUnicode);
                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(ref str_result);
                            CallFunctionRet = ValueType.NumericType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.StringType;
                        }

                        return CallFunctionRet;
                    }

                case "min":
                    {
                        num_result = GetValueAsDouble(ref @params[1], is_term[1]);
                        var loopTo10 = pcount;
                        for (i = (short)2; i <= loopTo10; i++)
                        {
                            rdbl = GetValueAsDouble(ref @params[(int)i], is_term[(int)i]);
                            if (num_result > rdbl)
                            {
                                num_result = rdbl;
                            }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "morale":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined14() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex45 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex45))
                                    {
                                        Unit localItem29() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        num_result = (double)localItem29().MainPilot().Morale;
                                    }
                                    else if (localIsDefined14())
                                    {
                                        Pilot localItem30() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                        num_result = (double)localItem30().Morale;
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        {
                                            var withBlock31 = Event_Renamed.SelectedUnitForEvent;
                                            if ((int)withBlock31.CountPilot() > 0)
                                            {
                                                num_result = (double)withBlock31.MainPilot().Morale;
                                            }
                                        }
                                    }

                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "nickname":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    buf = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined15() { object argIndex1 = (object)buf; var ret = SRC.PDList.IsDefined(ref argIndex1); return ret; }

                                    bool localIsDefined16() { object argIndex1 = (object)buf; var ret = SRC.NPDList.IsDefined(ref argIndex1); return ret; }

                                    bool localIsDefined17() { object argIndex1 = (object)buf; var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

                                    bool localIsDefined18() { object argIndex1 = (object)buf; var ret = SRC.UDList.IsDefined(ref argIndex1); return ret; }

                                    bool localIsDefined19() { object argIndex1 = (object)buf; var ret = SRC.IDList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex46 = (object)buf;
                                    if (SRC.PList.IsDefined(ref argIndex46))
                                    {
                                        Pilot localItem31() { object argIndex1 = (object)buf; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                        str_result = localItem31().get_Nickname(false);
                                    }
                                    else if (localIsDefined15())
                                    {
                                        PilotData localItem32() { object argIndex1 = (object)buf; var ret = SRC.PDList.Item(ref argIndex1); return ret; }

                                        str_result = localItem32().Nickname;
                                    }
                                    else if (localIsDefined16())
                                    {
                                        NonPilotData localItem33() { object argIndex1 = (object)buf; var ret = SRC.NPDList.Item(ref argIndex1); return ret; }

                                        str_result = localItem33().Nickname;
                                    }
                                    else if (localIsDefined17())
                                    {
                                        Unit localItem34() { object argIndex1 = (object)buf; var ret = SRC.UList.Item(ref argIndex1); return ret; }

                                        str_result = localItem34().Nickname0;
                                    }
                                    else if (localIsDefined18())
                                    {
                                        UnitData localItem35() { object argIndex1 = (object)buf; var ret = SRC.UDList.Item(ref argIndex1); return ret; }

                                        str_result = localItem35().Nickname;
                                    }
                                    else if (localIsDefined19())
                                    {
                                        ItemData localItem36() { object argIndex1 = (object)buf; var ret = SRC.IDList.Item(ref argIndex1); return ret; }

                                        str_result = localItem36().Nickname;
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        str_result = Event_Renamed.SelectedUnitForEvent.Nickname0;
                                    }

                                    break;
                                }
                        }

                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "partner":
                    {
                        i = (short)GetValueAsLong(ref @params[1], is_term[1]);
                        if ((int)i == 0)
                        {
                            str_result = Event_Renamed.SelectedUnitForEvent.ID;
                        }
                        else if (1 <= (int)i & (int)i <= Information.UBound(Commands.SelectedPartners))
                        {
                            str_result = Commands.SelectedPartners[(int)i].ID;
                        }

                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "party":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined20() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex47 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex47))
                                    {
                                        Unit localItem210() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        str_result = localItem210().Party0;
                                    }
                                    else if (localIsDefined20())
                                    {
                                        Pilot localItem37() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                        str_result = localItem37().Party;
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        str_result = Event_Renamed.SelectedUnitForEvent.Party0;
                                    }

                                    break;
                                }
                        }

                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "pilot":
                    {
                        switch (pcount)
                        {
                            case 2:
                                {
                                    uname = GetValueAsString(ref @params[1], is_term[1]);
                                    object argIndex49 = (object)uname;
                                    if (SRC.UList.IsDefined(ref argIndex49))
                                    {
                                        i = (short)GetValueAsLong(ref @params[2], is_term[2]);
                                        object argIndex48 = (object)uname;
                                        {
                                            var withBlock32 = SRC.UList.Item(ref argIndex48);
                                            if (0 < (int)i & i <= withBlock32.CountPilot())
                                            {
                                                Pilot localPilot() { object argIndex1 = (object)i; var ret = withBlock32.Pilot(ref argIndex1); return ret; }

                                                str_result = localPilot().Name;
                                            }
                                            else if (withBlock32.CountPilot() < i & i <= (short)(withBlock32.CountPilot() + withBlock32.CountSupport()))
                                            {
                                                Pilot localSupport() { object argIndex1 = (object)(i - withBlock32.CountPilot()); var ret = withBlock32.Support(ref argIndex1); return ret; }

                                                str_result = localSupport().Name;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    uname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined21() { object argIndex1 = (object)uname; var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

                                    if (GeneralLib.IsNumber(ref uname))
                                    {
                                        if (Event_Renamed.SelectedUnitForEvent is object)
                                        {
                                            i = Conversions.ToShort(uname);
                                            {
                                                var withBlock33 = Event_Renamed.SelectedUnitForEvent;
                                                if (0 < (int)i & i <= withBlock33.CountPilot())
                                                {
                                                    Pilot localPilot1() { object argIndex1 = (object)i; var ret = withBlock33.Pilot(ref argIndex1); return ret; }

                                                    str_result = localPilot1().Name;
                                                }
                                                else if (withBlock33.CountPilot() < i & i <= (short)(withBlock33.CountPilot() + withBlock33.CountSupport()))
                                                {
                                                    Pilot localSupport1() { object argIndex1 = (object)(i - withBlock33.CountPilot()); var ret = withBlock33.Support(ref argIndex1); return ret; }

                                                    str_result = localSupport1().Name;
                                                }
                                            }
                                        }
                                    }
                                    else if (localIsDefined21())
                                    {
                                        object argIndex50 = (object)uname;
                                        {
                                            var withBlock34 = SRC.UList.Item(ref argIndex50);
                                            if ((int)withBlock34.CountPilot() > 0)
                                            {
                                                str_result = withBlock34.MainPilot().Name;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        {
                                            var withBlock35 = Event_Renamed.SelectedUnitForEvent;
                                            if ((int)withBlock35.CountPilot() > 0)
                                            {
                                                str_result = withBlock35.MainPilot().Name;
                                            }
                                        }
                                    }

                                    break;
                                }
                        }

                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "pilotid":
                    {
                        switch (pcount)
                        {
                            case 2:
                                {
                                    uname = GetValueAsString(ref @params[1], is_term[1]);
                                    object argIndex52 = (object)uname;
                                    if (SRC.UList.IsDefined(ref argIndex52))
                                    {
                                        i = (short)GetValueAsLong(ref @params[2], is_term[2]);
                                        object argIndex51 = (object)uname;
                                        {
                                            var withBlock36 = SRC.UList.Item(ref argIndex51);
                                            if (0 < (int)i & i <= withBlock36.CountPilot())
                                            {
                                                Pilot localPilot2() { object argIndex1 = (object)i; var ret = withBlock36.Pilot(ref argIndex1); return ret; }

                                                str_result = localPilot2().ID;
                                            }
                                            else if (withBlock36.CountPilot() < i & i <= (short)(withBlock36.CountPilot() + withBlock36.CountSupport()))
                                            {
                                                Pilot localSupport2() { object argIndex1 = (object)(i - withBlock36.CountPilot()); var ret = withBlock36.Support(ref argIndex1); return ret; }

                                                str_result = localSupport2().ID;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    uname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined22() { object argIndex1 = (object)uname; var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

                                    if (GeneralLib.IsNumber(ref uname))
                                    {
                                        if (Event_Renamed.SelectedUnitForEvent is object)
                                        {
                                            i = Conversions.ToShort(uname);
                                            {
                                                var withBlock37 = Event_Renamed.SelectedUnitForEvent;
                                                if (0 < (int)i & i <= withBlock37.CountPilot())
                                                {
                                                    Pilot localPilot3() { object argIndex1 = (object)i; var ret = withBlock37.Pilot(ref argIndex1); return ret; }

                                                    str_result = localPilot3().ID;
                                                }
                                                else if (withBlock37.CountPilot() < i & i <= (short)(withBlock37.CountPilot() + withBlock37.CountSupport()))
                                                {
                                                    Pilot localSupport3() { object argIndex1 = (object)(i - withBlock37.CountPilot()); var ret = withBlock37.Support(ref argIndex1); return ret; }

                                                    str_result = localSupport3().ID;
                                                }
                                            }
                                        }
                                    }
                                    else if (localIsDefined22())
                                    {
                                        object argIndex53 = (object)uname;
                                        {
                                            var withBlock38 = SRC.UList.Item(ref argIndex53);
                                            if ((int)withBlock38.CountPilot() > 0)
                                            {
                                                str_result = withBlock38.MainPilot().ID;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        {
                                            var withBlock39 = Event_Renamed.SelectedUnitForEvent;
                                            if ((int)withBlock39.CountPilot() > 0)
                                            {
                                                str_result = withBlock39.MainPilot().ID;
                                            }
                                        }
                                    }

                                    break;
                                }
                        }

                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "plana":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined23() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex54 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex54))
                                    {
                                        Unit localItem211() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        num_result = (double)localItem211().MainPilot().Plana;
                                    }
                                    else if (localIsDefined23())
                                    {
                                        Pilot localItem38() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                        num_result = (double)localItem38().Plana;
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        num_result = (double)Event_Renamed.SelectedUnitForEvent.MainPilot().Plana;
                                    }

                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "random":
                    {
                        num_result = (double)GeneralLib.Dice(GetValueAsLong(ref @params[1], is_term[1]));
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "rank":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined24() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex56 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex56))
                                    {
                                        Unit localItem212() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        num_result = (double)localItem212().Rank;
                                    }
                                    else if (!localIsDefined24())
                                    {
                                        num_result = 0d;
                                    }
                                    else
                                    {
                                        object argIndex55 = (object)pname;
                                        {
                                            var withBlock40 = SRC.PList.Item(ref argIndex55);
                                            if (withBlock40.Unit_Renamed is object)
                                            {
                                                num_result = (double)withBlock40.Unit_Renamed.Rank;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        num_result = (double)Event_Renamed.SelectedUnitForEvent.Rank;
                                    }

                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "regexp":
                    {
                        ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
                        /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo RegExp_Error' at character 111360


                        Input:
                                        On Error GoTo RegExp_Error

                         */
                        if (RegEx is null)
                        {
                            RegEx = Interaction.CreateObject("VBScript.RegExp");
                        }

                        // RegExp(文字列, パターン[,大小区別あり|大小区別なし])
                        buf = "";
                        if ((int)pcount > 0)
                        {
                            // 文字列全体を検索
                            // UPGRADE_WARNING: オブジェクト RegEx.Global の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            RegEx.Global = (object)true;
                            // 大文字小文字の区別（True=区別しない）
                            // UPGRADE_WARNING: オブジェクト RegEx.IgnoreCase の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            RegEx.IgnoreCase = (object)false;
                            if ((int)pcount >= 3)
                            {
                                if (GetValueAsString(ref @params[3], is_term[3]) == "大小区別なし")
                                {
                                    // UPGRADE_WARNING: オブジェクト RegEx.IgnoreCase の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    RegEx.IgnoreCase = (object)true;
                                }
                            }
                            // 検索パターン
                            // UPGRADE_WARNING: オブジェクト RegEx.Pattern の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            RegEx.Pattern = GetValueAsString(ref @params[2], is_term[2]);
                            // UPGRADE_WARNING: オブジェクト RegEx.Execute の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            Matches = RegEx.Execute(GetValueAsString(ref @params[1], is_term[1]));
                            // UPGRADE_WARNING: オブジェクト Matches.Count の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(Matches.Count, 0, false)))
                            {
                                regexp_index = (short)-1;
                            }
                            else
                            {
                                regexp_index = (short)0;
                                // UPGRADE_WARNING: オブジェクト Matches() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                buf = Conversions.ToString(Expression.Matches((object)regexp_index));
                            }
                        }
                        else if ((int)regexp_index >= 0)
                        {
                            regexp_index = (short)((int)regexp_index + 1);
                            // UPGRADE_WARNING: オブジェクト Matches.Count の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectLessEqual(regexp_index, Operators.SubtractObject(Matches.Count, 1), false)))
                            {
                                // UPGRADE_WARNING: オブジェクト Matches() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                buf = Conversions.ToString(Expression.Matches((object)regexp_index));
                            }
                        }

                        str_result = buf;
                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                        RegExp_Error:
                        ;
                        Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, "VBScriptがインストールされていません");
                        return CallFunctionRet;
                    }
                // RegExpReplace(文字列, 検索パターン, 置換パターン[,大小区別あり|大小区別なし])

                case "regexpreplace":
                    {
                        ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
                        /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo RegExpReplace...' at character 114835


                        Input:
                                        'RegExpReplace(文字列, 検索パターン, 置換パターン[,大小区別あり|大小区別なし])

                                        On Error GoTo RegExpReplace_Error

                         */
                        if (RegEx is null)
                        {
                            RegEx = Interaction.CreateObject("VBScript.RegExp");
                        }

                        // 文字列全体を検索
                        // UPGRADE_WARNING: オブジェクト RegEx.Global の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        RegEx.Global = (object)true;
                        // 大文字小文字の区別（True=区別しない）
                        // UPGRADE_WARNING: オブジェクト RegEx.IgnoreCase の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        RegEx.IgnoreCase = (object)false;
                        if ((int)pcount >= 4)
                        {
                            if (GetValueAsString(ref @params[4], is_term[4]) == "大小区別なし")
                            {
                                // UPGRADE_WARNING: オブジェクト RegEx.IgnoreCase の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                RegEx.IgnoreCase = (object)true;
                            }
                        }
                        // 検索パターン
                        // UPGRADE_WARNING: オブジェクト RegEx.Pattern の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        RegEx.Pattern = GetValueAsString(ref @params[2], is_term[2]);

                        // 置換実行
                        // UPGRADE_WARNING: オブジェクト RegEx.Replace の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        buf = Conversions.ToString(RegEx.Replace(GetValueAsString(ref @params[1], is_term[1]), GetValueAsString(ref @params[3], is_term[3])));
                        str_result = buf;
                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                        RegExpReplace_Error:
                        ;
                        Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, "VBScriptがインストールされていません");
                        return CallFunctionRet;
                    }

                case "relation":
                    {
                        pname = GetValueAsString(ref @params[1], is_term[1]);
                        bool localIsDefined25() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                        if (!localIsDefined25())
                        {
                            num_result = 0d;
                            if (etype == ValueType.StringType)
                            {
                                str_result = "0";
                                CallFunctionRet = ValueType.StringType;
                            }
                            else
                            {
                                CallFunctionRet = ValueType.NumericType;
                            }

                            return CallFunctionRet;
                        }

                        Pilot localItem39() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                        pname = localItem39().Name;
                        pname2 = GetValueAsString(ref @params[2], is_term[2]);
                        bool localIsDefined26() { object argIndex1 = (object)pname2; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                        if (!localIsDefined26())
                        {
                            num_result = 0d;
                            if (etype == ValueType.StringType)
                            {
                                str_result = "0";
                                CallFunctionRet = ValueType.StringType;
                            }
                            else
                            {
                                CallFunctionRet = ValueType.NumericType;
                            }

                            return CallFunctionRet;
                        }

                        Pilot localItem40() { object argIndex1 = (object)pname2; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                        pname2 = localItem40().Name;
                        string argexpr = "関係:" + pname + ":" + pname2;
                        num_result = (double)GetValueAsLong(ref argexpr);
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "replace":
                    {
                        switch (pcount)
                        {
                            case 4:
                                {
                                    buf = GetValueAsString(ref @params[1], is_term[1]);
                                    num = (short)GetValueAsLong(ref @params[4], is_term[4]);
                                    buf2 = Strings.Right(buf, Strings.Len(buf) - (int)num + 1);
                                    string args2 = GetValueAsString(ref @params[2], is_term[2]);
                                    string args3 = GetValueAsString(ref @params[3], is_term[3]);
                                    GeneralLib.ReplaceString(ref buf2, ref args2, ref args3);
                                    str_result = Strings.Left(buf, (int)num - 1) + buf2;
                                    break;
                                }

                            case 5:
                                {
                                    buf = GetValueAsString(ref @params[1], is_term[1]);
                                    num = (short)GetValueAsLong(ref @params[4], is_term[4]);
                                    num2 = (short)GetValueAsLong(ref @params[5], is_term[5]);
                                    buf2 = Strings.Mid(buf, (int)num, (int)num2);
                                    string args21 = GetValueAsString(ref @params[2], is_term[2]);
                                    string args31 = GetValueAsString(ref @params[3], is_term[3]);
                                    GeneralLib.ReplaceString(ref buf2, ref args21, ref args31);
                                    str_result = Strings.Left(buf, (int)num - 1) + buf2 + Strings.Right(buf, Strings.Len(buf) - (num + num2 - 1) - 1);
                                    break;
                                }

                            default:
                                {
                                    str_result = GetValueAsString(ref @params[1], is_term[1]);
                                    string args22 = GetValueAsString(ref @params[2], is_term[2]);
                                    string args32 = GetValueAsString(ref @params[3], is_term[3]);
                                    GeneralLib.ReplaceString(ref str_result, ref args22, ref args32);
                                    break;
                                }
                        }

                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(ref str_result);
                            CallFunctionRet = ValueType.NumericType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.StringType;
                        }

                        return CallFunctionRet;
                    }

                case "rgb":
                    {
                        buf = Conversion.Hex(Information.RGB(GetValueAsLong(ref @params[1], is_term[1]), GetValueAsLong(ref @params[2], is_term[2]), GetValueAsLong(ref @params[3], is_term[3])));
                        var loopTo11 = (short)(6 - Strings.Len(buf));
                        for (i = (short)1; i <= loopTo11; i++)
                            buf = "0" + buf;
                        str_result = "#000000";
                        var midTmp = Strings.Mid(buf, 5, 2);
                        StringType.MidStmtStr(ref str_result, 2, 2, midTmp);
                        var midTmp1 = Strings.Mid(buf, 3, 2);
                        StringType.MidStmtStr(ref str_result, 4, 2, midTmp1);
                        var midTmp2 = Strings.Mid(buf, 1, 2);
                        StringType.MidStmtStr(ref str_result, 6, 2, midTmp2);
                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "right":
                    {
                        str_result = Strings.Right(GetValueAsString(ref @params[1], is_term[1]), GetValueAsLong(ref @params[2], is_term[2]));
                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(ref str_result);
                            CallFunctionRet = ValueType.NumericType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.StringType;
                        }

                        return CallFunctionRet;
                    }

                case "rightb":
                    {
                        buf = GetValueAsString(ref @params[1], is_term[1]);
                        // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                        // UPGRADE_ISSUE: RightB$ 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                        str_result = RightB(Strings.StrConv(buf, vbFromUnicode), GetValueAsLong(ref @params[2], is_term[2]));
                        // UPGRADE_ISSUE: 定数 vbUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                        str_result = Strings.StrConv(str_result, vbUnicode);
                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(ref str_result);
                            CallFunctionRet = ValueType.NumericType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.StringType;
                        }

                        return CallFunctionRet;
                    }

                case "round":
                case "rounddown":
                case "roundup":
                    {
                        ldbl = GetValueAsDouble(ref @params[1], is_term[1]);
                        if ((int)pcount == 1)
                        {
                            num2 = (short)0;
                        }
                        else
                        {
                            num2 = (short)GetValueAsLong(ref @params[2], is_term[2]);
                        }

                        num = (short)Conversion.Int(ldbl * Math.Pow(10d, (double)num2));
                        switch (Strings.LCase(fname) ?? "")
                        {
                            case "round":
                                {
                                    if (ldbl * Math.Pow(10d, (double)num2) - (double)num >= 0.5d)
                                    {
                                        num = (short)((int)num + 1);
                                    }

                                    break;
                                }

                            case "roundup":
                                {
                                    if (ldbl * Math.Pow(10d, (double)num2) - (double)num > 0d)
                                    {
                                        num = (short)((int)num + 1);
                                    }

                                    break;
                                }
                        }

                        num_result = (double)num / Math.Pow(10d, (double)num2);
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "rset":
                    {
                        buf = GetValueAsString(ref @params[1], is_term[1]);
                        i = (short)GetValueAsLong(ref @params[2], is_term[2]);
                        // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                        // UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                        if (LenB(Strings.StrConv(buf, vbFromUnicode)) < i)
                        {
                            // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                            // UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                            str_result = Strings.Space(i - LenB(Strings.StrConv(buf, vbFromUnicode))) + buf;
                        }
                        else
                        {
                            str_result = buf;
                        }

                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "sin":
                    {
                        num_result = Math.Sin(GetValueAsDouble(ref @params[1], is_term[1]));
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "skill":
                    {
                        switch (pcount)
                        {
                            case 2:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    buf = GetValueAsString(ref @params[2], is_term[2]);

                                    // エリアスが定義されている？
                                    object argIndex57 = (object)buf;
                                    if (SRC.ALDList.IsDefined(ref argIndex57))
                                    {
                                        AliasDataType localItem41() { object argIndex1 = (object)buf; var ret = SRC.ALDList.Item(ref argIndex1); return ret; }

                                        buf = localItem41().get_AliasType((short)1);
                                    }

                                    bool localIsDefined27() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex60 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex60))
                                    {
                                        Unit localItem213() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        object argIndex58 = (object)buf;
                                        string argref_mode = "";
                                        num_result = localItem213().MainPilot().SkillLevel(ref argIndex58, ref_mode: ref argref_mode);
                                    }
                                    else if (localIsDefined27())
                                    {
                                        Pilot localItem42() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                        object argIndex59 = (object)buf;
                                        string argref_mode1 = "";
                                        num_result = localItem42().SkillLevel(ref argIndex59, ref_mode: ref argref_mode1);
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    buf = GetValueAsString(ref @params[1], is_term[1]);

                                    // エリアスが定義されている？
                                    object argIndex61 = (object)buf;
                                    if (SRC.ALDList.IsDefined(ref argIndex61))
                                    {
                                        AliasDataType localItem43() { object argIndex1 = (object)buf; var ret = SRC.ALDList.Item(ref argIndex1); return ret; }

                                        buf = localItem43().get_AliasType((short)1);
                                    }

                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        object argIndex62 = (object)buf;
                                        string argref_mode2 = "";
                                        num_result = Event_Renamed.SelectedUnitForEvent.MainPilot().SkillLevel(ref argIndex62, ref_mode: ref argref_mode2);
                                    }

                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "sp":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined28() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex63 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex63))
                                    {
                                        Unit localItem214() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        num_result = (double)localItem214().MainPilot().SP;
                                    }
                                    else if (localIsDefined28())
                                    {
                                        Pilot localItem44() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                        num_result = (double)localItem44().SP;
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        num_result = (double)Event_Renamed.SelectedUnitForEvent.MainPilot().SP;
                                    }

                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "specialpower":
                case "mind":
                    {
                        switch (pcount)
                        {
                            case 2:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    buf = GetValueAsString(ref @params[2], is_term[2]);
                                    bool localIsDefined29() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex65 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex65))
                                    {
                                        Unit localItem215() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        if (localItem215().IsSpecialPowerInEffect(ref buf))
                                        {
                                            num_result = 1d;
                                        }
                                    }
                                    else if (localIsDefined29())
                                    {
                                        object argIndex64 = (object)pname;
                                        {
                                            var withBlock41 = SRC.PList.Item(ref argIndex64);
                                            if (withBlock41.Unit_Renamed is object)
                                            {
                                                if (withBlock41.Unit_Renamed.IsSpecialPowerInEffect(ref buf))
                                                {
                                                    num_result = 1d;
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        buf = GetValueAsString(ref @params[1], is_term[1]);
                                        if (Event_Renamed.SelectedUnitForEvent.IsSpecialPowerInEffect(ref buf))
                                        {
                                            num_result = 1d;
                                        }
                                    }

                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "sqr":
                    {
                        num_result = Math.Sqrt(GetValueAsDouble(ref @params[1], is_term[1]));
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "status":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined30() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex67 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex67))
                                    {
                                        Unit localItem216() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        str_result = localItem216().Status_Renamed;
                                    }
                                    else if (localIsDefined30())
                                    {
                                        object argIndex66 = (object)pname;
                                        {
                                            var withBlock42 = SRC.PList.Item(ref argIndex66);
                                            if (withBlock42.Unit_Renamed is object)
                                            {
                                                str_result = withBlock42.Unit_Renamed.Status_Renamed;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        str_result = Event_Renamed.SelectedUnitForEvent.Status_Renamed;
                                    }

                                    break;
                                }
                        }

                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "strcomp":
                    {
                        num_result = (double)Strings.StrComp(GetValueAsString(ref @params[1], is_term[1]), GetValueAsString(ref @params[2], is_term[2]));
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "string":
                    {
                        buf = GetValueAsString(ref @params[2], is_term[2]);
                        if (Strings.Len(buf) <= 1)
                        {
                            str_result = new string(Conversions.ToChar(buf), GetValueAsLong(ref @params[1], is_term[1]));
                        }
                        else
                        {
                            // String関数では文字列の先頭しか繰り返しされないので、
                            // 長さが2以上の文字列の場合は別処理
                            str_result = "";
                            var loopTo12 = (short)GetValueAsLong(ref @params[1], is_term[1]);
                            for (i = (short)1; i <= loopTo12; i++)
                                str_result = str_result + buf;
                        }

                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(ref str_result);
                            CallFunctionRet = ValueType.NumericType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.StringType;
                        }

                        return CallFunctionRet;
                    }

                case "tan":
                    {
                        num_result = Math.Tan(GetValueAsDouble(ref @params[1], is_term[1]));
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "term":
                    {
                        switch (pcount)
                        {
                            case 2:
                                {
                                    pname = GetValueAsString(ref @params[2], is_term[2]);
                                    object argIndex68 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex68))
                                    {
                                        Unit localItem217() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        string argtname = GetValueAsString(ref @params[1], is_term[1]);
                                        var argu = localItem217();
                                        str_result = Term(ref argtname, ref argu);
                                    }
                                    else
                                    {
                                        string argtname1 = GetValueAsString(ref @params[1], is_term[1]);
                                        Unit argu1 = null;
                                        str_result = Term(ref argtname1, u: ref argu1);
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    string argtname2 = GetValueAsString(ref @params[1], is_term[1]);
                                    Unit argu2 = null;
                                    str_result = Term(ref argtname2, u: ref argu2);
                                    break;
                                }
                        }

                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "textheight":
                    {
                        // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                        num_result = GUI.MainForm.picMain(0).TextHeight(GetValueAsString(ref @params[1], is_term[1]));
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "textwidth":
                    {
                        // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                        num_result = GUI.MainForm.picMain(0).TextWidth(GetValueAsString(ref @params[1], is_term[1]));
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "trim":
                    {
                        str_result = Strings.Trim(GetValueAsString(ref @params[1], is_term[1]));
                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "unit":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined31() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex70 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex70))
                                    {
                                        Unit localItem218() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        str_result = localItem218().Name;
                                    }
                                    else if (localIsDefined31())
                                    {
                                        object argIndex69 = (object)pname;
                                        {
                                            var withBlock43 = SRC.PList.Item(ref argIndex69);
                                            if (withBlock43.Unit_Renamed is object)
                                            {
                                                str_result = withBlock43.Unit_Renamed.Name;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        str_result = Event_Renamed.SelectedUnitForEvent.Name;
                                    }

                                    break;
                                }
                        }

                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "unitid":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined32() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    object argIndex72 = (object)pname;
                                    if (SRC.UList.IsDefined2(ref argIndex72))
                                    {
                                        Unit localItem219() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        str_result = localItem219().ID;
                                    }
                                    else if (localIsDefined32())
                                    {
                                        object argIndex71 = (object)pname;
                                        {
                                            var withBlock44 = SRC.PList.Item(ref argIndex71);
                                            if (withBlock44.Unit_Renamed is object)
                                            {
                                                str_result = withBlock44.Unit_Renamed.ID;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        str_result = Event_Renamed.SelectedUnitForEvent.ID;
                                    }

                                    break;
                                }
                        }

                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "x":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    switch (pname ?? "")
                                    {
                                        case "目標地点":
                                            {
                                                num_result = (double)Commands.SelectedX;
                                                break;
                                            }

                                        case "マウス":
                                            {
                                                num_result = (double)GUI.MouseX;
                                                break;
                                            }

                                        default:
                                            {
                                                bool localIsDefined33() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                                object argIndex74 = (object)pname;
                                                if (SRC.UList.IsDefined2(ref argIndex74))
                                                {
                                                    Unit localItem220() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                                    num_result = (double)localItem220().x;
                                                }
                                                else if (localIsDefined33())
                                                {
                                                    object argIndex73 = (object)pname;
                                                    {
                                                        var withBlock45 = SRC.PList.Item(ref argIndex73);
                                                        if (withBlock45.Unit_Renamed is object)
                                                        {
                                                            num_result = (double)withBlock45.Unit_Renamed.x;
                                                        }
                                                    }
                                                }

                                                break;
                                            }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        num_result = (double)Event_Renamed.SelectedUnitForEvent.x;
                                    }

                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "y":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    switch (pname ?? "")
                                    {
                                        case "目標地点":
                                            {
                                                num_result = (double)Commands.SelectedY;
                                                break;
                                            }

                                        case "マウス":
                                            {
                                                num_result = (double)GUI.MouseY;
                                                break;
                                            }

                                        default:
                                            {
                                                bool localIsDefined34() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                                object argIndex76 = (object)pname;
                                                if (SRC.UList.IsDefined2(ref argIndex76))
                                                {
                                                    Unit localItem221() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                                    num_result = (double)localItem221().y;
                                                }
                                                else if (localIsDefined34())
                                                {
                                                    object argIndex75 = (object)pname;
                                                    {
                                                        var withBlock46 = SRC.PList.Item(ref argIndex75);
                                                        if (withBlock46.Unit_Renamed is object)
                                                        {
                                                            num_result = (double)withBlock46.Unit_Renamed.y;
                                                        }
                                                    }
                                                }

                                                break;
                                            }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        num_result = (double)Event_Renamed.SelectedUnitForEvent.y;
                                    }

                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }
                // ADD START 240a
                case "windowwidth":
                    {
                        if (etype == ValueType.NumericType)
                        {
                            num_result = (double)GUI.MainPWidth;
                            CallFunctionRet = ValueType.NumericType;
                        }
                        else if (etype == ValueType.StringType)
                        {
                            str_result = GUI.MainPWidth.ToString();
                            CallFunctionRet = ValueType.StringType;
                        }

                        return CallFunctionRet;
                    }

                case "windowheight":
                    {
                        if (etype == ValueType.NumericType)
                        {
                            num_result = (double)GUI.MainPHeight;
                            CallFunctionRet = ValueType.NumericType;
                        }
                        else if (etype == ValueType.StringType)
                        {
                            str_result = GUI.MainPHeight.ToString();
                            CallFunctionRet = ValueType.StringType;
                        }

                        return CallFunctionRet;
                    }
                // ADD  END  240a
                case "wx":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined210() { object argIndex1 = (object)pname; var ret = SRC.UList.IsDefined2(ref argIndex1); return ret; }

                                    bool localIsDefined35() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    if (GeneralLib.IsNumber(ref pname))
                                    {
                                        num_result = (double)GeneralLib.StrToLng(ref pname);
                                    }
                                    else if (pname == "目標地点")
                                    {
                                        num_result = (double)Commands.SelectedX;
                                    }
                                    else if (localIsDefined210())
                                    {
                                        Unit localItem222() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        num_result = (double)localItem222().x;
                                    }
                                    else if (localIsDefined35())
                                    {
                                        object argIndex77 = (object)pname;
                                        {
                                            var withBlock47 = SRC.PList.Item(ref argIndex77);
                                            if (withBlock47.Unit_Renamed is object)
                                            {
                                                num_result = (double)withBlock47.Unit_Renamed.x;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        num_result = (double)Event_Renamed.SelectedUnitForEvent.x;
                                    }

                                    break;
                                }
                        }

                        num_result = (double)GUI.MapToPixelX((short)num_result);
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "wy":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(ref @params[1], is_term[1]);
                                    bool localIsDefined211() { object argIndex1 = (object)pname; var ret = SRC.UList.IsDefined2(ref argIndex1); return ret; }

                                    bool localIsDefined36() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                                    if (GeneralLib.IsNumber(ref pname))
                                    {
                                        num_result = (double)GeneralLib.StrToLng(ref pname);
                                    }
                                    else if (pname == "目標地点")
                                    {
                                        num_result = (double)Commands.SelectedY;
                                    }
                                    else if (localIsDefined211())
                                    {
                                        Unit localItem223() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                        num_result = (double)localItem223().y;
                                    }
                                    else if (localIsDefined36())
                                    {
                                        object argIndex78 = (object)pname;
                                        {
                                            var withBlock48 = SRC.PList.Item(ref argIndex78);
                                            if (withBlock48.Unit_Renamed is object)
                                            {
                                                num_result = (double)withBlock48.Unit_Renamed.y;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event_Renamed.SelectedUnitForEvent is object)
                                    {
                                        num_result = (double)Event_Renamed.SelectedUnitForEvent.y;
                                    }

                                    break;
                                }
                        }

                        num_result = (double)GUI.MapToPixelY((short)num_result);
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "wide":
                    {
                        str_result = Strings.StrConv(GetValueAsString(ref @params[1], is_term[1]), VbStrConv.Wide);
                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                // Date型の処理
                case "year":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    buf = GetValueAsString(ref @params[1], is_term[1]);
                                    if (Information.IsDate(buf))
                                    {
                                        num_result = (double)DateAndTime.Year(Conversions.ToDate(buf));
                                    }
                                    else
                                    {
                                        num_result = 0d;
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    num_result = (double)DateAndTime.Year(DateAndTime.Now);
                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "month":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    buf = GetValueAsString(ref @params[1], is_term[1]);
                                    if (Information.IsDate(buf))
                                    {
                                        num_result = (double)DateAndTime.Month(Conversions.ToDate(buf));
                                    }
                                    else
                                    {
                                        num_result = 0d;
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    num_result = (double)DateAndTime.Month(DateAndTime.Now);
                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "weekday":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    buf = GetValueAsString(ref @params[1], is_term[1]);
                                    if (Information.IsDate(buf))
                                    {
                                        switch (DateAndTime.Weekday(Conversions.ToDate(buf)))
                                        {
                                            case (int)FirstDayOfWeek.Sunday:
                                                {
                                                    str_result = "日曜";
                                                    break;
                                                }

                                            case (int)FirstDayOfWeek.Monday:
                                                {
                                                    str_result = "月曜";
                                                    break;
                                                }

                                            case (int)FirstDayOfWeek.Tuesday:
                                                {
                                                    str_result = "火曜";
                                                    break;
                                                }

                                            case (int)FirstDayOfWeek.Wednesday:
                                                {
                                                    str_result = "水曜";
                                                    break;
                                                }

                                            case (int)FirstDayOfWeek.Thursday:
                                                {
                                                    str_result = "木曜";
                                                    break;
                                                }

                                            case (int)FirstDayOfWeek.Friday:
                                                {
                                                    str_result = "金曜";
                                                    break;
                                                }

                                            case (int)FirstDayOfWeek.Saturday:
                                                {
                                                    str_result = "土曜";
                                                    break;
                                                }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    switch (DateAndTime.Weekday(DateAndTime.Now))
                                    {
                                        case (int)FirstDayOfWeek.Sunday:
                                            {
                                                str_result = "日曜";
                                                break;
                                            }

                                        case (int)FirstDayOfWeek.Monday:
                                            {
                                                str_result = "月曜";
                                                break;
                                            }

                                        case (int)FirstDayOfWeek.Tuesday:
                                            {
                                                str_result = "火曜";
                                                break;
                                            }

                                        case (int)FirstDayOfWeek.Wednesday:
                                            {
                                                str_result = "水曜";
                                                break;
                                            }

                                        case (int)FirstDayOfWeek.Thursday:
                                            {
                                                str_result = "木曜";
                                                break;
                                            }

                                        case (int)FirstDayOfWeek.Friday:
                                            {
                                                str_result = "金曜";
                                                break;
                                            }

                                        case (int)FirstDayOfWeek.Saturday:
                                            {
                                                str_result = "土曜";
                                                break;
                                            }
                                    }

                                    break;
                                }
                        }

                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "day":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    buf = GetValueAsString(ref @params[1], is_term[1]);
                                    if (Information.IsDate(buf))
                                    {
                                        num_result = (double)DateAndTime.Day(Conversions.ToDate(buf));
                                    }
                                    else
                                    {
                                        num_result = 0d;
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    num_result = (double)DateAndTime.Day(DateAndTime.Now);
                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "hour":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    buf = GetValueAsString(ref @params[1], is_term[1]);
                                    if (Information.IsDate(buf))
                                    {
                                        num_result = (double)DateAndTime.Hour(Conversions.ToDate(buf));
                                    }
                                    else
                                    {
                                        num_result = 0d;
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    num_result = (double)DateAndTime.Hour(DateAndTime.Now);
                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "minute":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    buf = GetValueAsString(ref @params[1], is_term[1]);
                                    if (Information.IsDate(buf))
                                    {
                                        num_result = (double)DateAndTime.Minute(Conversions.ToDate(buf));
                                    }
                                    else
                                    {
                                        num_result = 0d;
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    num_result = (double)DateAndTime.Minute(DateAndTime.Now);
                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "second":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    buf = GetValueAsString(ref @params[1], is_term[1]);
                                    if (Information.IsDate(buf))
                                    {
                                        num_result = (double)DateAndTime.Second(Conversions.ToDate(buf));
                                    }
                                    else
                                    {
                                        num_result = 0d;
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    num_result = (double)DateAndTime.Second(DateAndTime.Now);
                                    break;
                                }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                case "difftime":
                    {
                        if ((int)pcount == 2)
                        {
                            if (@params[1] == "Now")
                            {
                                d1 = DateAndTime.Now;
                            }
                            else
                            {
                                buf = GetValueAsString(ref @params[1], is_term[1]);
                                if (!Information.IsDate(buf))
                                {
                                    return CallFunctionRet;
                                }

                                d1 = Conversions.ToDate(buf);
                            }

                            if (@params[2] == "Now")
                            {
                                d2 = DateAndTime.Now;
                            }
                            else
                            {
                                buf = GetValueAsString(ref @params[2], is_term[2]);
                                if (!Information.IsDate(buf))
                                {
                                    return CallFunctionRet;
                                }

                                d2 = Conversions.ToDate(buf);
                            }

                            num_result = (double)DateAndTime.Second(DateTime.FromOADate(d2.ToOADate() - d1.ToOADate()));
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(num_result);
                            CallFunctionRet = ValueType.StringType;
                        }
                        else
                        {
                            CallFunctionRet = ValueType.NumericType;
                        }

                        return CallFunctionRet;
                    }

                // ダイアログ表示
                case "loadfiledialog":
                    {
                        switch (pcount)
                        {
                            case 2:
                                {
                                    string argdtitle = "ファイルを開く";
                                    string argdefault_file = "";
                                    string argftype = GetValueAsString(ref @params[1], is_term[1]);
                                    string argfsuffix = GetValueAsString(ref @params[2], is_term[2]);
                                    string argftype2 = "";
                                    string argfsuffix2 = "";
                                    string argftype3 = "";
                                    string argfsuffix3 = "";
                                    str_result = FileDialog.LoadFileDialog(ref argdtitle, ref SRC.ScenarioPath, ref argdefault_file, (short)2, ref argftype, ref argfsuffix, ftype2: ref argftype2, fsuffix2: ref argfsuffix2, ftype3: ref argftype3, fsuffix3: ref argfsuffix3);
                                    break;
                                }

                            case 3:
                                {
                                    string argdtitle1 = "ファイルを開く";
                                    string argdefault_file1 = GetValueAsString(ref @params[3], is_term[3]);
                                    string argftype1 = GetValueAsString(ref @params[1], is_term[1]);
                                    string argfsuffix1 = GetValueAsString(ref @params[2], is_term[2]);
                                    string argftype21 = "";
                                    string argfsuffix21 = "";
                                    string argftype31 = "";
                                    string argfsuffix31 = "";
                                    str_result = FileDialog.LoadFileDialog(ref argdtitle1, ref SRC.ScenarioPath, ref argdefault_file1, (short)2, ref argftype1, ref argfsuffix1, ftype2: ref argftype21, fsuffix2: ref argfsuffix21, ftype3: ref argftype31, fsuffix3: ref argfsuffix31);
                                    break;
                                }

                            case 4:
                                {
                                    string argdtitle2 = "ファイルを開く";
                                    string argfpath = SRC.ScenarioPath + GetValueAsString(ref @params[4], is_term[4]);
                                    string argdefault_file2 = GetValueAsString(ref @params[3], is_term[3]);
                                    string argftype4 = GetValueAsString(ref @params[1], is_term[1]);
                                    string argfsuffix4 = GetValueAsString(ref @params[2], is_term[2]);
                                    string argftype22 = "";
                                    string argfsuffix22 = "";
                                    string argftype32 = "";
                                    string argfsuffix32 = "";
                                    str_result = FileDialog.LoadFileDialog(ref argdtitle2, ref argfpath, ref argdefault_file2, (short)2, ref argftype4, ref argfsuffix4, ftype2: ref argftype22, fsuffix2: ref argfsuffix22, ftype3: ref argftype32, fsuffix3: ref argfsuffix32);
                                    break;
                                }
                        }

                        CallFunctionRet = ValueType.StringType;

                        // 本当はこれだけでいいはずだけど……
                        if (Strings.InStr(str_result, SRC.ScenarioPath) > 0)
                        {
                            str_result = Strings.Mid(str_result, Strings.Len(SRC.ScenarioPath) + 1);
                            return CallFunctionRet;
                        }

                        // フルパス指定ならここで終了
                        if (Strings.Right(Strings.Left(str_result, 3), 2) == @":\")
                        {
                            str_result = "";
                            return CallFunctionRet;
                        }

                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                        while (string.IsNullOrEmpty(FileSystem.Dir(SRC.ScenarioPath + str_result, FileAttribute.Normal)))
                        {
                            if (Strings.InStr(str_result, @"\") == 0)
                            {
                                // シナリオフォルダ外のファイルだった
                                str_result = "";
                                return CallFunctionRet;
                            }

                            str_result = Strings.Mid(str_result, Strings.InStr(str_result, @"\") + 1);
                        }

                        return CallFunctionRet;
                    }

                case "savefiledialog":
                    {
                        switch (pcount)
                        {
                            case 2:
                                {
                                    string argdtitle3 = "ファイルを保存";
                                    string argdefault_file3 = "";
                                    string argftype5 = GetValueAsString(ref @params[1], is_term[1]);
                                    string argfsuffix5 = GetValueAsString(ref @params[2], is_term[2]);
                                    string argftype23 = "";
                                    string argfsuffix23 = "";
                                    string argftype33 = "";
                                    string argfsuffix33 = "";
                                    str_result = FileDialog.SaveFileDialog(ref argdtitle3, ref SRC.ScenarioPath, ref argdefault_file3, (short)2, ref argftype5, ref argfsuffix5, ftype2: ref argftype23, fsuffix2: ref argfsuffix23, ftype3: ref argftype33, fsuffix3: ref argfsuffix33);
                                    break;
                                }

                            case 3:
                                {
                                    string argdtitle4 = "ファイルを保存";
                                    string argdefault_file4 = GetValueAsString(ref @params[3], is_term[3]);
                                    string argftype6 = GetValueAsString(ref @params[1], is_term[1]);
                                    string argfsuffix6 = GetValueAsString(ref @params[2], is_term[2]);
                                    string argftype24 = "";
                                    string argfsuffix24 = "";
                                    string argftype34 = "";
                                    string argfsuffix34 = "";
                                    str_result = FileDialog.SaveFileDialog(ref argdtitle4, ref SRC.ScenarioPath, ref argdefault_file4, (short)2, ref argftype6, ref argfsuffix6, ftype2: ref argftype24, fsuffix2: ref argfsuffix24, ftype3: ref argftype34, fsuffix3: ref argfsuffix34);
                                    break;
                                }

                            case 4:
                                {
                                    string argdtitle5 = "ファイルを保存";
                                    string argfpath1 = SRC.ScenarioPath + GetValueAsString(ref @params[4], is_term[4]);
                                    string argdefault_file5 = GetValueAsString(ref @params[3], is_term[3]);
                                    string argftype7 = GetValueAsString(ref @params[1], is_term[1]);
                                    string argfsuffix7 = GetValueAsString(ref @params[2], is_term[2]);
                                    string argftype25 = "";
                                    string argfsuffix25 = "";
                                    string argftype35 = "";
                                    string argfsuffix35 = "";
                                    str_result = FileDialog.SaveFileDialog(ref argdtitle5, ref argfpath1, ref argdefault_file5, (short)2, ref argftype7, ref argfsuffix7, ftype2: ref argftype25, fsuffix2: ref argfsuffix25, ftype3: ref argftype35, fsuffix3: ref argfsuffix35);
                                    break;
                                }
                        }

                        CallFunctionRet = ValueType.StringType;

                        // 本当はこれだけでいいはずだけど……
                        if (Strings.InStr(str_result, SRC.ScenarioPath) > 0)
                        {
                            str_result = Strings.Mid(str_result, Strings.Len(SRC.ScenarioPath) + 1);
                            return CallFunctionRet;
                        }

                        if (Strings.InStr(str_result, @"\") == 0)
                        {
                            return CallFunctionRet;
                        }

                        var loopTo13 = (short)Strings.Len(str_result);
                        for (i = (short)1; i <= loopTo13; i++)
                        {
                            if (Strings.Mid(str_result, Strings.Len(str_result) - (int)i + 1, 1) == @"\")
                            {
                                break;
                            }
                        }

                        buf = Strings.Left(str_result, Strings.Len(str_result) - (int)i);
                        str_result = Strings.Mid(str_result, Strings.Len(str_result) - (int)i + 2);
                        while (Strings.InStr(buf, @"\") > 0)
                        {
                            buf = Strings.Mid(buf, Strings.InStr(buf, @"\") + 1);
                            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                            if (!string.IsNullOrEmpty(FileSystem.Dir(SRC.ScenarioPath + buf, FileAttribute.Directory)))
                            {
                                str_result = buf + @"\" + str_result;
                                return CallFunctionRet;
                            }
                        }

                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                        if (!string.IsNullOrEmpty(FileSystem.Dir(SRC.ScenarioPath + buf, FileAttribute.Directory)))
                        {
                            str_result = buf + @"\" + str_result;
                        }

                        return CallFunctionRet;
                    }
            }

            LookUpUserDefinedID:
            ;

            // ユーザー定義関数？
            ret = Event_Renamed.FindNormalLabel(ref fname);
            if (ret > 0)
            {
                // 関数が見つかった
                ret = ret + 1;

                // 呼び出し階層をチェック
                if (Event_Renamed.CallDepth > Event_Renamed.MaxCallDepth)
                {
                    Event_Renamed.CallDepth = Event_Renamed.MaxCallDepth;
                    Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, GeneralLib.FormatNum(Event_Renamed.MaxCallDepth) + "階層を越えるサブルーチンの呼び出しは出来ません");
                    return CallFunctionRet;
                }

                // 引数用スタックが溢れないかチェック
                if ((short)(Event_Renamed.ArgIndex + pcount) > Event_Renamed.MaxArgIndex)
                {
                    Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, "サブルーチンの引数の総数が" + GeneralLib.FormatNum(Event_Renamed.MaxArgIndex) + "個を超えています");
                    return CallFunctionRet;
                }

                // 引数の値を先に求めておく
                // (スタックに積みながら計算すると、引数での関数呼び出しで不正になる)
                var loopTo14 = pcount;
                for (i = 1; i <= loopTo14; i++)
                    @params[i] = GetValueAsString(ref @params[i], is_term[i]);

                // 現在の状態を保存
                Event_Renamed.CallStack[Event_Renamed.CallDepth] = Event_Renamed.CurrentLineNum;
                Event_Renamed.ArgIndexStack[Event_Renamed.CallDepth] = Event_Renamed.ArgIndex;
                Event_Renamed.VarIndexStack[Event_Renamed.CallDepth] = Event_Renamed.VarIndex;
                Event_Renamed.ForIndexStack[Event_Renamed.CallDepth] = Event_Renamed.ForIndex;
                Event_Renamed.UpVarLevelStack[Event_Renamed.CallDepth] = Event_Renamed.UpVarLevel;

                // UpVarの階層数を初期化
                Event_Renamed.UpVarLevel = 0;

                // 呼び出し階層数をインクリメント
                Event_Renamed.CallDepth = (short)(Event_Renamed.CallDepth + 1);
                cur_depth = Event_Renamed.CallDepth;

                // 引数をスタックに積む
                Event_Renamed.ArgIndex = (short)(Event_Renamed.ArgIndex + pcount);
                var loopTo15 = pcount;
                for (i = 1; i <= loopTo15; i++)
                    Event_Renamed.ArgStack[Event_Renamed.ArgIndex - i + 1] = @params[i];

                // サブルーチン本体を実行
                do
                {
                    Event_Renamed.CurrentLineNum = ret;
                    if (Event_Renamed.CurrentLineNum > Information.UBound(Event_Renamed.EventCmd))
                    {
                        break;
                    }

                    {
                        var withBlock49 = Event_Renamed.EventCmd[Event_Renamed.CurrentLineNum];
                        if (cur_depth == Event_Renamed.CallDepth & withBlock49.Name == Event_Renamed.CmdType.ReturnCmd)
                        {
                            break;
                        }

                        ret = withBlock49.Exec();
                    }
                }
                while (ret > 0);

                // 返り値
                {
                    var withBlock50 = Event_Renamed.EventCmd[Event_Renamed.CurrentLineNum];
                    if (withBlock50.ArgNum > 1)
                    {
                        str_result = withBlock50.GetArgAsString(2);
                    }
                    else
                    {
                        str_result = "";
                    }
                }

                // 呼び出し階層数をデクリメント
                Event_Renamed.CallDepth = (short)(Event_Renamed.CallDepth - 1);

                // サブルーチン実行前の状態に復帰
                Event_Renamed.CurrentLineNum = Event_Renamed.CallStack[Event_Renamed.CallDepth];
                Event_Renamed.ArgIndex = Event_Renamed.ArgIndexStack[Event_Renamed.CallDepth];
                Event_Renamed.VarIndex = Event_Renamed.VarIndexStack[Event_Renamed.CallDepth];
                Event_Renamed.ForIndex = Event_Renamed.ForIndexStack[Event_Renamed.CallDepth];
                Event_Renamed.UpVarLevel = Event_Renamed.UpVarLevelStack[Event_Renamed.CallDepth];
                if (etype == ValueType.NumericType)
                {
                    num_result = GeneralLib.StrToDbl(ref str_result);
                    CallFunctionRet = ValueType.NumericType;
                }
                else
                {
                    CallFunctionRet = ValueType.StringType;
                }

                return CallFunctionRet;
            }

            // 実はシステム定義のグローバル変数？
            if (IsGlobalVariableDefined(ref expr))
            {
                {
                    var withBlock51 = Event_Renamed.GlobalVariableList[expr];
                    switch (etype)
                    {
                        case ValueType.NumericType:
                            {
                                // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item(expr).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(withBlock51.VariableType, ValueType.NumericType, false)))
                                {
                                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    num_result = Conversions.ToDouble(withBlock51.NumericValue);
                                }
                                else
                                {
                                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    string argexpr1 = Conversions.ToString(withBlock51.StringValue);
                                    num_result = GeneralLib.StrToDbl(ref argexpr1);
                                }

                                CallFunctionRet = ValueType.NumericType;
                                break;
                            }

                        case ValueType.StringType:
                            {
                                // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item(expr).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(withBlock51.VariableType, ValueType.StringType, false)))
                                {
                                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    str_result = Conversions.ToString(withBlock51.StringValue);
                                }
                                else
                                {
                                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    str_result = GeneralLib.FormatNum(Conversions.ToDouble(withBlock51.NumericValue));
                                }

                                CallFunctionRet = ValueType.StringType;
                                break;
                            }

                        case ValueType.UndefinedType:
                            {
                                // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item(expr).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(withBlock51.VariableType, ValueType.StringType, false)))
                                {
                                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    str_result = Conversions.ToString(withBlock51.StringValue);
                                    CallFunctionRet = ValueType.StringType;
                                }
                                else
                                {
                                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    num_result = Conversions.ToDouble(withBlock51.NumericValue);
                                    CallFunctionRet = ValueType.NumericType;
                                }

                                break;
                            }
                    }
                }

                return CallFunctionRet;
            }

            // 結局ただの文字列……
            str_result = expr;
            CallFunctionRet = ValueType.StringType;
            return CallFunctionRet;
        }

        // Info関数の評価
        private static string EvalInfoFunc(ref string[] @params)
        {
            string EvalInfoFuncRet = default;
            Unit u;
            UnitData ud;
            Pilot p;
            PilotData pd;
            NonPilotData nd;
            Item it;
            ItemData itd;
            SpecialPowerData spd;
            short i, idx, j = default;
            string buf;
            string aname;
            int max_value;
            EvalInfoFuncRet = "";

            // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            u = null;
            // UPGRADE_NOTE: オブジェクト ud をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            ud = null;
            // UPGRADE_NOTE: オブジェクト p をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            p = null;
            // UPGRADE_NOTE: オブジェクト pd をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            pd = null;
            // UPGRADE_NOTE: オブジェクト nd をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            nd = null;
            // UPGRADE_NOTE: オブジェクト it をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            it = null;
            // UPGRADE_NOTE: オブジェクト itd をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            itd = null;
            // UPGRADE_NOTE: オブジェクト spd をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            spd = null;

            // 各オブジェクトの設定
            switch (@params[1] ?? "")
            {
                case "ユニット":
                    {
                        var tmp = @params;
                        object argIndex1 = tmp[2];
                        u = SRC.UList.Item(ref argIndex1);
                        idx = 3;
                        break;
                    }

                case "ユニットデータ":
                    {
                        var tmp1 = @params;
                        object argIndex2 = tmp1[2];
                        ud = SRC.UDList.Item(ref argIndex2);
                        idx = 3;
                        break;
                    }

                case "パイロット":
                    {
                        var tmp2 = @params;
                        object argIndex3 = tmp2[2];
                        p = SRC.PList.Item(ref argIndex3);
                        idx = 3;
                        break;
                    }

                case "パイロットデータ":
                    {
                        var tmp3 = @params;
                        object argIndex4 = tmp3[2];
                        pd = SRC.PDList.Item(ref argIndex4);
                        idx = 3;
                        break;
                    }

                case "非戦闘員":
                    {
                        var tmp4 = @params;
                        object argIndex5 = tmp4[2];
                        nd = SRC.NPDList.Item(ref argIndex5);
                        idx = 3;
                        break;
                    }

                case "アイテム":
                    {
                        var tmp7 = @params;
                        object argIndex8 = tmp7[2];
                        if (SRC.IList.IsDefined(ref argIndex8))
                        {
                            var tmp5 = @params;
                            object argIndex6 = tmp5[2];
                            it = SRC.IList.Item(ref argIndex6);
                        }
                        else
                        {
                            var tmp6 = @params;
                            object argIndex7 = tmp6[2];
                            itd = SRC.IDList.Item(ref argIndex7);
                        }

                        idx = 3;
                        break;
                    }

                case "アイテムデータ":
                    {
                        var tmp8 = @params;
                        object argIndex9 = tmp8[2];
                        itd = SRC.IDList.Item(ref argIndex9);
                        idx = 3;
                        break;
                    }

                case "スペシャルパワー":
                    {
                        var tmp9 = @params;
                        object argIndex10 = tmp9[2];
                        spd = SRC.SPDList.Item(ref argIndex10);
                        idx = 3;
                        break;
                    }

                case "マップ":
                case "オプション":
                    {
                        idx = 1;
                        break;
                    }

                case var @case when @case == "":
                    {
                        return EvalInfoFuncRet;
                    }

                default:
                    {
                        var tmp10 = @params;
                        object argIndex11 = tmp10[1];
                        u = SRC.UList.Item(ref argIndex11);
                        var tmp11 = @params;
                        object argIndex12 = tmp11[1];
                        ud = SRC.UDList.Item(ref argIndex12);
                        var tmp12 = @params;
                        object argIndex13 = tmp12[1];
                        p = SRC.PList.Item(ref argIndex13);
                        var tmp13 = @params;
                        object argIndex14 = tmp13[1];
                        pd = SRC.PDList.Item(ref argIndex14);
                        var tmp14 = @params;
                        object argIndex15 = tmp14[1];
                        nd = SRC.NPDList.Item(ref argIndex15);
                        var tmp15 = @params;
                        object argIndex16 = tmp15[1];
                        it = SRC.IList.Item(ref argIndex16);
                        var tmp16 = @params;
                        object argIndex17 = tmp16[1];
                        itd = SRC.IDList.Item(ref argIndex17);
                        var tmp17 = @params;
                        object argIndex18 = tmp17[1];
                        spd = SRC.SPDList.Item(ref argIndex18);
                        idx = 2;
                        break;
                    }
            }

            // UPGRADE_NOTE: my は my_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
            short mx = default, my_Renamed = default;
            switch (@params[idx] ?? "")
            {
                case "名称":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = u.Name;
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = ud.Name;
                        }
                        else if (p is object)
                        {
                            EvalInfoFuncRet = p.Name;
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = pd.Name;
                        }
                        else if (nd is object)
                        {
                            EvalInfoFuncRet = nd.Name;
                        }
                        else if (it is object)
                        {
                            EvalInfoFuncRet = it.Name;
                        }
                        else if (itd is object)
                        {
                            EvalInfoFuncRet = itd.Name;
                        }
                        else if (spd is object)
                        {
                            EvalInfoFuncRet = spd.Name;
                        }

                        break;
                    }

                case "読み仮名":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = u.KanaName;
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = ud.KanaName;
                        }
                        else if (p is object)
                        {
                            EvalInfoFuncRet = p.KanaName;
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = pd.KanaName;
                        }
                        else if (it is object)
                        {
                            EvalInfoFuncRet = it.Data.KanaName;
                        }
                        else if (itd is object)
                        {
                            EvalInfoFuncRet = itd.KanaName;
                        }
                        else if (spd is object)
                        {
                            EvalInfoFuncRet = spd.KanaName;
                        }

                        break;
                    }

                case "愛称":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = u.Nickname0;
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = ud.Nickname;
                        }
                        else if (p is object)
                        {
                            EvalInfoFuncRet = p.get_Nickname(false);
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = pd.Nickname;
                        }
                        else if (nd is object)
                        {
                            EvalInfoFuncRet = nd.Nickname;
                        }
                        else if (it is object)
                        {
                            EvalInfoFuncRet = it.Nickname();
                        }
                        else if (itd is object)
                        {
                            EvalInfoFuncRet = itd.Nickname;
                        }

                        break;
                    }

                case "性別":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = p.Sex;
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = pd.Sex;
                        }

                        return EvalInfoFuncRet;
                    }

                case "ユニットクラス":
                case "機体クラス":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = u.Class_Renamed;
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = ud.Class_Renamed;
                        }
                        else if (p is object)
                        {
                            EvalInfoFuncRet = p.Class_Renamed;
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = pd.Class_Renamed;
                        }

                        break;
                    }

                case "地形適応":
                    {
                        if (u is object)
                        {
                            for (i = 1; i <= 4; i++)
                            {
                                switch (u.get_Adaption(i))
                                {
                                    case 5:
                                        {
                                            EvalInfoFuncRet = EvalInfoFuncRet + "S";
                                            break;
                                        }

                                    case 4:
                                        {
                                            EvalInfoFuncRet = EvalInfoFuncRet + "A";
                                            break;
                                        }

                                    case 3:
                                        {
                                            EvalInfoFuncRet = EvalInfoFuncRet + "B";
                                            break;
                                        }

                                    case 2:
                                        {
                                            EvalInfoFuncRet = EvalInfoFuncRet + "C";
                                            break;
                                        }

                                    case 1:
                                        {
                                            EvalInfoFuncRet = EvalInfoFuncRet + "D";
                                            break;
                                        }

                                    default:
                                        {
                                            EvalInfoFuncRet = EvalInfoFuncRet + "E";
                                            break;
                                        }
                                }
                            }
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = ud.Adaption;
                        }
                        else if (p is object)
                        {
                            EvalInfoFuncRet = p.Adaption;
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = pd.Adaption;
                        }

                        break;
                    }

                case "経験値":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = u.ExpValue.ToString();
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = ud.ExpValue.ToString();
                        }
                        else if (p is object)
                        {
                            EvalInfoFuncRet = p.ExpValue.ToString();
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = pd.ExpValue.ToString();
                        }

                        break;
                    }

                case "格闘":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.Infight);
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(pd.Infight);
                        }

                        break;
                    }

                case "射撃":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.Shooting);
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(pd.Shooting);
                        }

                        return EvalInfoFuncRet;
                    }

                case "命中":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.Hit);
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(pd.Hit);
                        }

                        break;
                    }

                case "回避":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.Dodge);
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(pd.Dodge);
                        }

                        break;
                    }

                case "技量":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.Technique);
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(pd.Technique);
                        }

                        break;
                    }

                case "反応":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.Intuition);
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(pd.Intuition);
                        }

                        break;
                    }

                case "防御":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.Defense);
                        }

                        break;
                    }

                case "格闘基本値":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.InfightBase);
                        }

                        break;
                    }

                case "射撃基本値":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.ShootingBase);
                        }

                        break;
                    }

                case "命中基本値":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.HitBase);
                        }

                        break;
                    }

                case "回避基本値":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.DodgeBase);
                        }

                        break;
                    }

                case "技量基本値":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.TechniqueBase);
                        }

                        break;
                    }

                case "反応基本値":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.IntuitionBase);
                        }

                        break;
                    }

                case "格闘修正値":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.InfightMod);
                        }

                        break;
                    }

                case "射撃修正値":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.ShootingMod);
                        }

                        break;
                    }

                case "命中修正値":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.HitMod);
                        }

                        break;
                    }

                case "回避修正値":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.DodgeMod);
                        }

                        break;
                    }

                case "技量修正値":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.TechniqueMod);
                        }

                        break;
                    }

                case "反応修正値":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.IntuitionMod);
                        }

                        break;
                    }

                case "格闘支援修正値":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.InfightMod2);
                        }

                        break;
                    }

                case "射撃支援修正値":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.ShootingMod2);
                        }

                        break;
                    }

                case "命中支援修正値":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.HitMod2);
                        }

                        break;
                    }

                case "回避支援修正値":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.DodgeMod2);
                        }

                        break;
                    }

                case "技量支援修正値":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.TechniqueMod2);
                        }

                        break;
                    }

                case "反応支援修正値":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.IntuitionMod2);
                        }

                        break;
                    }

                case "性格":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = p.Personality;
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = pd.Personality;
                        }

                        break;
                    }

                case "最大ＳＰ":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.MaxSP);
                            if (p.MaxSP == 0 & p.Unit_Renamed is object)
                            {
                                if (ReferenceEquals(p, p.Unit_Renamed.MainPilot()))
                                {
                                    object argIndex19 = 1;
                                    EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.Unit_Renamed.Pilot(ref argIndex19).MaxSP);
                                }
                            }
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(pd.SP);
                        }

                        break;
                    }

                case "ＳＰ":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.SP);
                            if (p.MaxSP == 0 & p.Unit_Renamed is object)
                            {
                                if (ReferenceEquals(p, p.Unit_Renamed.MainPilot()))
                                {
                                    object argIndex20 = 1;
                                    EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.Unit_Renamed.Pilot(ref argIndex20).SP);
                                }
                            }
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(pd.SP);
                        }

                        break;
                    }

                case "グラフィック":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = u.get_Bitmap(true);
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = ud.Bitmap0;
                        }
                        else if (p is object)
                        {
                            EvalInfoFuncRet = p.get_Bitmap(true);
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = pd.Bitmap0;
                        }
                        else if (nd is object)
                        {
                            EvalInfoFuncRet = nd.Bitmap0;
                        }

                        break;
                    }

                case "ＭＩＤＩ":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = p.BGM;
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = pd.BGM;
                        }

                        break;
                    }

                case "レベル":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.Level);
                        }

                        break;
                    }

                case "累積経験値":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.Exp);
                        }

                        break;
                    }

                case "気力":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.Morale);
                        }

                        break;
                    }

                case "最大霊力":
                case "最大プラーナ":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.MaxPlana());
                        }
                        else if (pd is object)
                        {
                            string argsname = "霊力";
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(pd.SkillLevel(0, ref argsname));
                        }

                        break;
                    }

                case "霊力":
                case "プラーナ":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.Plana);
                        }
                        else if (pd is object)
                        {
                            string argsname1 = "霊力";
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(pd.SkillLevel(0, ref argsname1));
                        }

                        break;
                    }

                case "同調率":
                case "シンクロ率":
                    {
                        if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.SynchroRate());
                        }
                        else if (pd is object)
                        {
                            string argsname2 = "同調率";
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(pd.SkillLevel(0, ref argsname2));
                        }

                        break;
                    }

                case "スペシャルパワー":
                case "精神コマンド":
                case "精神":
                    {
                        if (p is object)
                        {
                            if (p.MaxSP == 0 & p.Unit_Renamed is object)
                            {
                                if (ReferenceEquals(p, p.Unit_Renamed.MainPilot()))
                                {
                                    object argIndex21 = 1;
                                    p = p.Unit_Renamed.Pilot(ref argIndex21);
                                }
                            }

                            {
                                var withBlock = p;
                                var loopTo = withBlock.CountSpecialPower;
                                for (i = 1; i <= loopTo; i++)
                                    EvalInfoFuncRet = EvalInfoFuncRet + " " + withBlock.get_SpecialPower(i);
                            }

                            EvalInfoFuncRet = Strings.Trim(EvalInfoFuncRet);
                        }
                        else if (pd is object)
                        {
                            var loopTo1 = pd.CountSpecialPower(100);
                            for (i = 1; i <= loopTo1; i++)
                                EvalInfoFuncRet = EvalInfoFuncRet + " " + pd.SpecialPower(100, i);
                            EvalInfoFuncRet = Strings.Trim(EvalInfoFuncRet);
                        }

                        break;
                    }

                case "スペシャルパワー所有":
                case "精神コマンド所有":
                    {
                        if (p is object)
                        {
                            if (p.MaxSP == 0 & p.Unit_Renamed is object)
                            {
                                if (ReferenceEquals(p, p.Unit_Renamed.MainPilot()))
                                {
                                    object argIndex22 = 1;
                                    p = p.Unit_Renamed.Pilot(ref argIndex22);
                                }
                            }

                            if (p.IsSpecialPowerAvailable(ref @params[idx + 1]))
                            {
                                EvalInfoFuncRet = "1";
                            }
                            else
                            {
                                EvalInfoFuncRet = "0";
                            }
                        }
                        else if (pd is object)
                        {
                            if (pd.IsSpecialPowerAvailable(100, ref @params[idx + 1]))
                            {
                                EvalInfoFuncRet = "1";
                            }
                            else
                            {
                                EvalInfoFuncRet = "0";
                            }
                        }

                        break;
                    }

                case "スペシャルパワーコスト":
                case "精神コマンドコスト":
                    {
                        if (p is object)
                        {
                            if (p.MaxSP == 0 & p.Unit_Renamed is object)
                            {
                                if (ReferenceEquals(p, p.Unit_Renamed.MainPilot()))
                                {
                                    object argIndex23 = 1;
                                    p = p.Unit_Renamed.Pilot(ref argIndex23);
                                }
                            }

                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.SpecialPowerCost(ref @params[idx + 1]));
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(pd.SpecialPowerCost(@params[idx + 1]));
                        }

                        break;
                    }

                case "特殊能力数":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.CountFeature());
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(ud.CountFeature());
                        }
                        else if (p is object)
                        {
                            EvalInfoFuncRet = p.CountSkill().ToString();
                        }
                        else if (pd is object)
                        {
                            short localLLength() { string arglist = pd.Skill(100); var ret = GeneralLib.LLength(ref arglist); return ret; }

                            short localLLength1() { string arglist = pd.Skill(100); var ret = GeneralLib.LLength(ref arglist); return ret; }

                            EvalInfoFuncRet = localLLength1().ToString();
                        }
                        else if (it is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(it.CountFeature());
                        }
                        else if (itd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(itd.CountFeature());
                        }

                        break;
                    }

                case "特殊能力":
                    {
                        if (u is object)
                        {
                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                            {
                                object argIndex24 = Conversions.ToShort(@params[idx + 1]);
                                EvalInfoFuncRet = u.Feature(ref argIndex24);
                            }
                        }
                        else if (ud is object)
                        {
                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                            {
                                object argIndex25 = Conversions.ToShort(@params[idx + 1]);
                                EvalInfoFuncRet = ud.Feature(ref argIndex25);
                            }
                        }
                        else if (p is object)
                        {
                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                            {
                                object argIndex26 = Conversions.ToShort(@params[idx + 1]);
                                EvalInfoFuncRet = p.Skill(ref argIndex26);
                            }
                        }
                        else if (pd is object)
                        {
                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                            {
                                string arglist = pd.Skill(100);
                                EvalInfoFuncRet = GeneralLib.LIndex(ref arglist, Conversions.ToShort(@params[idx + 1]));
                            }
                        }
                        else if (it is object)
                        {
                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                            {
                                object argIndex27 = Conversions.ToShort(@params[idx + 1]);
                                EvalInfoFuncRet = it.Feature(ref argIndex27);
                            }
                        }
                        else if (itd is object)
                        {
                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                            {
                                object argIndex28 = Conversions.ToShort(@params[idx + 1]);
                                EvalInfoFuncRet = itd.Feature(ref argIndex28);
                            }
                        }

                        break;
                    }

                case "特殊能力名称":
                    {
                        aname = @params[idx + 1];

                        // エリアスが定義されている？
                        object argIndex30 = aname;
                        if (SRC.ALDList.IsDefined(ref argIndex30))
                        {
                            object argIndex29 = aname;
                            {
                                var withBlock1 = SRC.ALDList.Item(ref argIndex29);
                                var loopTo2 = withBlock1.Count;
                                for (i = 1; i <= loopTo2; i++)
                                {
                                    string localLIndex() { string arglist = withBlock1.get_AliasData(i); var ret = GeneralLib.LIndex(ref arglist, 1); withBlock1.get_AliasData(i) = arglist; return ret; }

                                    if ((localLIndex() ?? "") == (aname ?? ""))
                                    {
                                        aname = withBlock1.get_AliasType(i);
                                        break;
                                    }
                                }

                                if (i > withBlock1.Count)
                                {
                                    aname = withBlock1.get_AliasType(1);
                                }
                            }
                        }

                        if (u is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                object argIndex31 = Conversions.ToShort(@params[idx + 1]);
                                EvalInfoFuncRet = u.FeatureName(ref argIndex31);
                            }
                            else
                            {
                                object argIndex32 = aname;
                                EvalInfoFuncRet = u.FeatureName(ref argIndex32);
                            }
                        }
                        else if (ud is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                object argIndex33 = Conversions.ToShort(aname);
                                EvalInfoFuncRet = ud.FeatureName(ref argIndex33);
                            }
                            else
                            {
                                object argIndex34 = aname;
                                EvalInfoFuncRet = ud.FeatureName(ref argIndex34);
                            }
                        }
                        else if (p is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                object argIndex35 = Conversions.ToShort(aname);
                                EvalInfoFuncRet = p.SkillName(ref argIndex35);
                            }
                            else
                            {
                                object argIndex36 = aname;
                                EvalInfoFuncRet = p.SkillName(ref argIndex36);
                            }
                        }
                        else if (pd is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                string localLIndex1() { string arglist = pd.Skill(100); var ret = GeneralLib.LIndex(ref arglist, Conversions.ToShort(aname)); return ret; }

                                string argsname3 = localLIndex1();
                                EvalInfoFuncRet = pd.SkillName(100, ref argsname3);
                            }
                            else
                            {
                                EvalInfoFuncRet = pd.SkillName(100, ref aname);
                            }
                        }
                        else if (it is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                object argIndex37 = Conversions.ToShort(aname);
                                EvalInfoFuncRet = it.FeatureName(ref argIndex37);
                            }
                            else
                            {
                                object argIndex38 = aname;
                                EvalInfoFuncRet = it.FeatureName(ref argIndex38);
                            }
                        }
                        else if (itd is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                object argIndex39 = Conversions.ToShort(aname);
                                EvalInfoFuncRet = itd.FeatureName(ref argIndex39);
                            }
                            else
                            {
                                object argIndex40 = aname;
                                EvalInfoFuncRet = itd.FeatureName(ref argIndex40);
                            }
                        }

                        break;
                    }

                case "特殊能力所有":
                    {
                        aname = @params[idx + 1];

                        // エリアスが定義されている？
                        object argIndex42 = aname;
                        if (SRC.ALDList.IsDefined(ref argIndex42))
                        {
                            object argIndex41 = aname;
                            {
                                var withBlock2 = SRC.ALDList.Item(ref argIndex41);
                                var loopTo3 = withBlock2.Count;
                                for (i = 1; i <= loopTo3; i++)
                                {
                                    string localLIndex2() { string arglist = withBlock2.get_AliasData(i); var ret = GeneralLib.LIndex(ref arglist, 1); withBlock2.get_AliasData(i) = arglist; return ret; }

                                    if ((localLIndex2() ?? "") == (aname ?? ""))
                                    {
                                        aname = withBlock2.get_AliasType(i);
                                        break;
                                    }
                                }

                                if (i > withBlock2.Count)
                                {
                                    aname = withBlock2.get_AliasType(1);
                                }
                            }
                        }

                        if (u is object)
                        {
                            if (u.IsFeatureAvailable(ref aname))
                            {
                                EvalInfoFuncRet = "1";
                            }
                            else
                            {
                                EvalInfoFuncRet = "0";
                            }
                        }
                        else if (ud is object)
                        {
                            if (ud.IsFeatureAvailable(ref aname))
                            {
                                EvalInfoFuncRet = "1";
                            }
                            else
                            {
                                EvalInfoFuncRet = "0";
                            }
                        }
                        else if (p is object)
                        {
                            if (p.IsSkillAvailable(ref aname))
                            {
                                EvalInfoFuncRet = "1";
                            }
                            else
                            {
                                EvalInfoFuncRet = "0";
                            }
                        }
                        else if (pd is object)
                        {
                            if (pd.IsSkillAvailable(100, ref aname))
                            {
                                EvalInfoFuncRet = "1";
                            }
                            else
                            {
                                EvalInfoFuncRet = "0";
                            }
                        }
                        else if (it is object)
                        {
                            if (it.IsFeatureAvailable(ref aname))
                            {
                                EvalInfoFuncRet = "1";
                            }
                            else
                            {
                                EvalInfoFuncRet = "0";
                            }
                        }
                        else if (itd is object)
                        {
                            if (itd.IsFeatureAvailable(ref aname))
                            {
                                EvalInfoFuncRet = "1";
                            }
                            else
                            {
                                EvalInfoFuncRet = "0";
                            }
                        }

                        break;
                    }

                case "特殊能力レベル":
                    {
                        aname = @params[idx + 1];

                        // エリアスが定義されている？
                        object argIndex44 = aname;
                        if (SRC.ALDList.IsDefined(ref argIndex44))
                        {
                            object argIndex43 = aname;
                            {
                                var withBlock3 = SRC.ALDList.Item(ref argIndex43);
                                var loopTo4 = withBlock3.Count;
                                for (i = 1; i <= loopTo4; i++)
                                {
                                    string localLIndex3() { string arglist = withBlock3.get_AliasData(i); var ret = GeneralLib.LIndex(ref arglist, 1); withBlock3.get_AliasData(i) = arglist; return ret; }

                                    if ((localLIndex3() ?? "") == (aname ?? ""))
                                    {
                                        aname = withBlock3.get_AliasType(i);
                                        break;
                                    }
                                }

                                if (i > withBlock3.Count)
                                {
                                    aname = withBlock3.get_AliasType(1);
                                }
                            }
                        }

                        if (u is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                double localFeatureLevel() { object argIndex1 = Conversions.ToShort(aname); var ret = u.FeatureLevel(ref argIndex1); return ret; }

                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(localFeatureLevel());
                            }
                            else
                            {
                                double localFeatureLevel1() { object argIndex1 = aname; var ret = u.FeatureLevel(ref argIndex1); return ret; }

                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(localFeatureLevel1());
                            }
                        }
                        else if (ud is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                double localFeatureLevel2() { object argIndex1 = Conversions.ToShort(aname); var ret = ud.FeatureLevel(ref argIndex1); return ret; }

                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(localFeatureLevel2());
                            }
                            else
                            {
                                double localFeatureLevel3() { object argIndex1 = aname; var ret = ud.FeatureLevel(ref argIndex1); return ret; }

                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(localFeatureLevel3());
                            }
                        }
                        else if (p is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                double localSkillLevel() { object argIndex1 = Conversions.ToShort(aname); string argref_mode = ""; var ret = p.SkillLevel(ref argIndex1, ref_mode: ref argref_mode); return ret; }

                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(localSkillLevel());
                            }
                            else
                            {
                                double localSkillLevel1() { object argIndex1 = aname; string argref_mode = ""; var ret = p.SkillLevel(ref argIndex1, ref_mode: ref argref_mode); return ret; }

                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(localSkillLevel1());
                            }
                        }
                        else if (pd is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                string localLIndex4() { string arglist = pd.Skill(100); var ret = GeneralLib.LIndex(ref arglist, Conversions.ToShort(aname)); return ret; }

                                double localSkillLevel2() { string argsname = hs69a3321344d140f8b91f1e9add379ed5(); var ret = pd.SkillLevel(100, ref argsname); return ret; }

                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(localSkillLevel2());
                            }
                            else
                            {
                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(pd.SkillLevel(100, ref aname));
                            }
                        }
                        else if (it is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                double localFeatureLevel4() { object argIndex1 = Conversions.ToShort(aname); var ret = it.FeatureLevel(ref argIndex1); return ret; }

                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(localFeatureLevel4());
                            }
                            else
                            {
                                double localFeatureLevel5() { object argIndex1 = aname; var ret = it.FeatureLevel(ref argIndex1); return ret; }

                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(localFeatureLevel5());
                            }
                        }
                        else if (itd is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                double localFeatureLevel6() { object argIndex1 = Conversions.ToShort(aname); var ret = itd.FeatureLevel(ref argIndex1); return ret; }

                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(localFeatureLevel6());
                            }
                            else
                            {
                                double localFeatureLevel7() { object argIndex1 = aname; var ret = itd.FeatureLevel(ref argIndex1); return ret; }

                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(localFeatureLevel7());
                            }
                        }

                        break;
                    }

                case "特殊能力データ":
                    {
                        aname = @params[idx + 1];

                        // エリアスが定義されている？
                        object argIndex46 = aname;
                        if (SRC.ALDList.IsDefined(ref argIndex46))
                        {
                            object argIndex45 = aname;
                            {
                                var withBlock4 = SRC.ALDList.Item(ref argIndex45);
                                var loopTo5 = withBlock4.Count;
                                for (i = 1; i <= loopTo5; i++)
                                {
                                    string localLIndex5() { string arglist = withBlock4.get_AliasData(i); var ret = GeneralLib.LIndex(ref arglist, 1); withBlock4.get_AliasData(i) = arglist; return ret; }

                                    if ((localLIndex5() ?? "") == (aname ?? ""))
                                    {
                                        aname = withBlock4.get_AliasType(i);
                                        break;
                                    }
                                }

                                if (i > withBlock4.Count)
                                {
                                    aname = withBlock4.get_AliasType(1);
                                }
                            }
                        }

                        if (u is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                object argIndex47 = Conversions.ToShort(aname);
                                EvalInfoFuncRet = u.FeatureData(ref argIndex47);
                            }
                            else
                            {
                                object argIndex48 = aname;
                                EvalInfoFuncRet = u.FeatureData(ref argIndex48);
                            }
                        }
                        else if (ud is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                object argIndex49 = Conversions.ToShort(aname);
                                EvalInfoFuncRet = ud.FeatureData(ref argIndex49);
                            }
                            else
                            {
                                object argIndex50 = aname;
                                EvalInfoFuncRet = ud.FeatureData(ref argIndex50);
                            }
                        }
                        else if (p is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                object argIndex51 = Conversions.ToShort(aname);
                                EvalInfoFuncRet = p.SkillData(ref argIndex51);
                            }
                            else
                            {
                                object argIndex52 = aname;
                                EvalInfoFuncRet = p.SkillData(ref argIndex52);
                            }
                        }
                        else if (pd is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                string localLIndex6() { string arglist = pd.Skill(100); var ret = GeneralLib.LIndex(ref arglist, Conversions.ToShort(aname)); return ret; }

                                string argsname4 = localLIndex6();
                                EvalInfoFuncRet = pd.SkillData(100, ref argsname4);
                            }
                            else
                            {
                                EvalInfoFuncRet = pd.SkillData(100, ref aname);
                            }
                        }
                        else if (it is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                object argIndex53 = Conversions.ToShort(aname);
                                EvalInfoFuncRet = it.FeatureData(ref argIndex53);
                            }
                            else
                            {
                                object argIndex54 = aname;
                                EvalInfoFuncRet = it.FeatureData(ref argIndex54);
                            }
                        }
                        else if (itd is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                object argIndex55 = Conversions.ToShort(aname);
                                EvalInfoFuncRet = itd.FeatureData(ref argIndex55);
                            }
                            else
                            {
                                object argIndex56 = aname;
                                EvalInfoFuncRet = itd.FeatureData(ref argIndex56);
                            }
                        }

                        break;
                    }

                case "特殊能力必要技能":
                    {
                        aname = @params[idx + 1];

                        // エリアスが定義されている？
                        object argIndex58 = aname;
                        if (SRC.ALDList.IsDefined(ref argIndex58))
                        {
                            object argIndex57 = aname;
                            {
                                var withBlock5 = SRC.ALDList.Item(ref argIndex57);
                                var loopTo6 = withBlock5.Count;
                                for (i = 1; i <= loopTo6; i++)
                                {
                                    string localLIndex7() { string arglist = withBlock5.get_AliasData(i); var ret = GeneralLib.LIndex(ref arglist, 1); withBlock5.get_AliasData(i) = arglist; return ret; }

                                    if ((localLIndex7() ?? "") == (aname ?? ""))
                                    {
                                        aname = withBlock5.get_AliasType(i);
                                        break;
                                    }
                                }

                                if (i > withBlock5.Count)
                                {
                                    aname = withBlock5.get_AliasType(1);
                                }
                            }
                        }

                        if (u is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                object argIndex59 = Conversions.ToShort(aname);
                                EvalInfoFuncRet = u.FeatureNecessarySkill(ref argIndex59);
                            }
                            else
                            {
                                object argIndex60 = aname;
                                EvalInfoFuncRet = u.FeatureNecessarySkill(ref argIndex60);
                            }
                        }
                        else if (ud is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                object argIndex61 = Conversions.ToShort(aname);
                                EvalInfoFuncRet = ud.FeatureNecessarySkill(ref argIndex61);
                            }
                            else
                            {
                                object argIndex62 = aname;
                                EvalInfoFuncRet = ud.FeatureNecessarySkill(ref argIndex62);
                            }
                        }
                        else if (it is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                object argIndex63 = Conversions.ToShort(aname);
                                EvalInfoFuncRet = it.FeatureNecessarySkill(ref argIndex63);
                            }
                            else
                            {
                                object argIndex64 = aname;
                                EvalInfoFuncRet = it.FeatureNecessarySkill(ref argIndex64);
                            }
                        }
                        else if (itd is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                object argIndex65 = Conversions.ToShort(aname);
                                EvalInfoFuncRet = itd.FeatureNecessarySkill(ref argIndex65);
                            }
                            else
                            {
                                object argIndex66 = aname;
                                EvalInfoFuncRet = itd.FeatureNecessarySkill(ref argIndex66);
                            }
                        }

                        break;
                    }

                case "特殊能力解説":
                    {
                        aname = @params[idx + 1];

                        // エリアスが定義されている？
                        object argIndex68 = aname;
                        if (SRC.ALDList.IsDefined(ref argIndex68))
                        {
                            object argIndex67 = aname;
                            {
                                var withBlock6 = SRC.ALDList.Item(ref argIndex67);
                                var loopTo7 = withBlock6.Count;
                                for (i = 1; i <= loopTo7; i++)
                                {
                                    string localLIndex8() { string arglist = withBlock6.get_AliasData(i); var ret = GeneralLib.LIndex(ref arglist, 1); withBlock6.get_AliasData(i) = arglist; return ret; }

                                    if ((localLIndex8() ?? "") == (aname ?? ""))
                                    {
                                        aname = withBlock6.get_AliasType(i);
                                        break;
                                    }
                                }

                                if (i > withBlock6.Count)
                                {
                                    aname = withBlock6.get_AliasType(1);
                                }
                            }
                        }

                        if (u is object)
                        {
                            if (GeneralLib.IsNumber(ref aname))
                            {
                                EvalInfoFuncRet = Help.FeatureHelpMessage(ref u, Conversions.ToShort(aname), false);
                            }
                            else
                            {
                                EvalInfoFuncRet = Help.FeatureHelpMessage(ref u, aname, false);
                            }

                            if (string.IsNullOrEmpty(EvalInfoFuncRet) & p is object)
                            {
                                EvalInfoFuncRet = Help.SkillHelpMessage(ref p, ref aname);
                            }
                        }
                        else if (p is object)
                        {
                            EvalInfoFuncRet = Help.SkillHelpMessage(ref p, ref aname);
                            if (string.IsNullOrEmpty(EvalInfoFuncRet) & u is object)
                            {
                                if (GeneralLib.IsNumber(ref aname))
                                {
                                    EvalInfoFuncRet = Help.FeatureHelpMessage(ref u, Conversions.ToShort(aname), false);
                                }
                                else
                                {
                                    EvalInfoFuncRet = Help.FeatureHelpMessage(ref u, aname, false);
                                }
                            }
                        }

                        break;
                    }

                case "規定パイロット数":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.Data.PilotNum);
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(ud.PilotNum);
                        }

                        break;
                    }

                case "パイロット数":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.CountPilot());
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(ud.PilotNum);
                        }

                        break;
                    }

                case "サポート数":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.CountSupport());
                        }

                        break;
                    }

                case "最大アイテム数":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.Data.ItemNum);
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(ud.ItemNum);
                        }

                        break;
                    }

                case "アイテム数":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.CountItem());
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(ud.ItemNum);
                        }

                        break;
                    }

                case "アイテム":
                    {
                        if (u is object)
                        {
                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                            {
                                i = Conversions.ToShort(@params[idx + 1]);
                                if (0 < i & i <= u.CountItem())
                                {
                                    Item localItem() { object argIndex1 = i; var ret = u.Item(ref argIndex1); return ret; }

                                    EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(localItem().Name);
                                }
                            }
                        }

                        break;
                    }

                case "アイテムＩＤ":
                    {
                        if (u is object)
                        {
                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                            {
                                i = Conversions.ToShort(@params[idx + 1]);
                                if (0 < i & i <= u.CountItem())
                                {
                                    Item localItem1() { object argIndex1 = i; var ret = u.Item(ref argIndex1); return ret; }

                                    EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(localItem1().ID);
                                }
                            }
                        }

                        break;
                    }

                case "移動可能地形":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = u.Transportation;
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = ud.Transportation;
                        }

                        break;
                    }

                case "移動力":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.Speed);
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(ud.Speed);
                        }

                        break;
                    }

                case "サイズ":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = u.Size;
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = ud.Size;
                        }

                        break;
                    }

                case "修理費":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = u.Value.ToString();
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = ud.Value.ToString();
                        }

                        break;
                    }

                case "最大ＨＰ":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.MaxHP);
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(ud.HP);
                        }

                        break;
                    }

                case "ＨＰ":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.HP);
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(ud.HP);
                        }

                        break;
                    }

                case "最大ＥＮ":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.MaxEN);
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(ud.EN);
                        }

                        break;
                    }

                case "ＥＮ":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.EN);
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(ud.EN);
                        }

                        break;
                    }

                case "装甲":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.get_Armor(""));
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(ud.Armor);
                        }

                        break;
                    }

                case "運動性":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.get_Mobility(""));
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(ud.Mobility);
                        }

                        break;
                    }

                case "武器数":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.CountWeapon());
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(ud.CountWeapon());
                        }
                        else if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.Data.CountWeapon());
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(pd.CountWeapon());
                        }
                        else if (it is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(it.CountWeapon());
                        }
                        else if (itd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(itd.CountWeapon());
                        }

                        break;
                    }

                case "武器":
                    {
                        idx = (short)(idx + 1);
                        if (u is object)
                        {
                            {
                                var withBlock7 = u;
                                // 何番目の武器かを判定
                                if (GeneralLib.IsNumber(ref @params[idx]))
                                {
                                    i = Conversions.ToShort(@params[idx]);
                                }
                                else
                                {
                                    var loopTo8 = withBlock7.CountWeapon();
                                    for (i = 1; i <= loopTo8; i++)
                                    {
                                        if ((@params[idx] ?? "") == (withBlock7.Weapon(i).Name ?? ""))
                                        {
                                            break;
                                        }
                                    }
                                }
                                // 指定した武器を持っていない
                                if (i <= 0 | withBlock7.CountWeapon() < i)
                                {
                                    return EvalInfoFuncRet;
                                }

                                idx = (short)(idx + 1);
                                switch (@params[idx] ?? "")
                                {
                                    case var case1 when case1 == "":
                                    case "名称":
                                        {
                                            EvalInfoFuncRet = withBlock7.Weapon(i).Name;
                                            break;
                                        }

                                    case "攻撃力":
                                        {
                                            string argtarea = "";
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock7.WeaponPower(i, ref argtarea));
                                            break;
                                        }

                                    case "射程":
                                    case "最大射程":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock7.WeaponMaxRange(i));
                                            break;
                                        }

                                    case "最小射程":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock7.Weapon(i).MinRange);
                                            break;
                                        }

                                    case "命中率":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock7.WeaponPrecision(i));
                                            break;
                                        }

                                    case "最大弾数":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock7.MaxBullet(i));
                                            break;
                                        }

                                    case "弾数":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock7.Bullet(i));
                                            break;
                                        }

                                    case "消費ＥＮ":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock7.WeaponENConsumption(i));
                                            break;
                                        }

                                    case "必要気力":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock7.Weapon(i).NecessaryMorale);
                                            break;
                                        }

                                    case "地形適応":
                                        {
                                            EvalInfoFuncRet = withBlock7.Weapon(i).Adaption;
                                            break;
                                        }

                                    case "クリティカル率":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock7.WeaponCritical(i));
                                            break;
                                        }

                                    case "属性":
                                        {
                                            EvalInfoFuncRet = withBlock7.WeaponClass(i);
                                            break;
                                        }

                                    case "属性所有":
                                        {
                                            if (withBlock7.IsWeaponClassifiedAs(i, ref @params[idx + 1]))
                                            {
                                                EvalInfoFuncRet = "1";
                                            }
                                            else
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }

                                    case "属性レベル":
                                        {
                                            EvalInfoFuncRet = withBlock7.WeaponLevel(i, ref @params[idx + 1]).ToString();
                                            break;
                                        }

                                    case "属性名称":
                                        {
                                            EvalInfoFuncRet = Help.AttributeName(ref u, ref @params[idx + 1], false);
                                            break;
                                        }

                                    case "属性解説":
                                        {
                                            EvalInfoFuncRet = Help.AttributeHelpMessage(ref u, ref @params[idx + 1], i, false);
                                            break;
                                        }

                                    case "必要技能":
                                        {
                                            EvalInfoFuncRet = withBlock7.Weapon(i).NecessarySkill;
                                            break;
                                        }

                                    case "使用可":
                                        {
                                            string argref_mode = "ステータス";
                                            if (withBlock7.IsWeaponAvailable(i, ref argref_mode))
                                            {
                                                EvalInfoFuncRet = "1";
                                            }
                                            else
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }

                                    case "修得":
                                        {
                                            if (withBlock7.IsWeaponMastered(i))
                                            {
                                                EvalInfoFuncRet = "1";
                                            }
                                            else
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }
                                }
                            }
                        }
                        else if (ud is object)
                        {
                            // 何番目の武器かを判定
                            if (GeneralLib.IsNumber(ref @params[idx]))
                            {
                                i = Conversions.ToShort(@params[idx]);
                            }
                            else
                            {
                                var loopTo9 = ud.CountWeapon();
                                for (i = 1; i <= loopTo9; i++)
                                {
                                    WeaponData localWeapon() { object argIndex1 = i; var ret = ud.Weapon(ref argIndex1); return ret; }

                                    if ((@params[idx] ?? "") == (localWeapon().Name ?? ""))
                                    {
                                        break;
                                    }
                                }
                            }
                            // 指定した武器を持っていない
                            if (i <= 0 | ud.CountWeapon() < i)
                            {
                                return EvalInfoFuncRet;
                            }

                            idx = (short)(idx + 1);
                            object argIndex69 = i;
                            {
                                var withBlock8 = ud.Weapon(ref argIndex69);
                                switch (@params[idx] ?? "")
                                {
                                    case var case2 when case2 == "":
                                    case "名称":
                                        {
                                            EvalInfoFuncRet = withBlock8.Name;
                                            break;
                                        }

                                    case "攻撃力":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock8.Power);
                                            break;
                                        }

                                    case "射程":
                                    case "最大射程":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock8.MaxRange);
                                            break;
                                        }

                                    case "最小射程":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock8.MinRange);
                                            break;
                                        }

                                    case "命中率":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock8.Precision);
                                            break;
                                        }

                                    case "最大弾数":
                                    case "弾数":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock8.Bullet);
                                            break;
                                        }

                                    case "消費ＥＮ":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock8.ENConsumption);
                                            break;
                                        }

                                    case "必要気力":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock8.NecessaryMorale);
                                            break;
                                        }

                                    case "地形適応":
                                        {
                                            EvalInfoFuncRet = withBlock8.Adaption;
                                            break;
                                        }

                                    case "クリティカル率":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock8.Critical);
                                            break;
                                        }

                                    case "属性":
                                        {
                                            EvalInfoFuncRet = withBlock8.Class_Renamed;
                                            break;
                                        }

                                    case "属性所有":
                                        {
                                            if (GeneralLib.InStrNotNest(ref withBlock8.Class_Renamed, ref @params[idx + 1]) > 0)
                                            {
                                                EvalInfoFuncRet = "1";
                                            }
                                            else
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }

                                    case "属性レベル":
                                        {
                                            string argstring2 = @params[idx + 1] + "L";
                                            j = GeneralLib.InStrNotNest(ref withBlock8.Class_Renamed, ref argstring2);
                                            if (j == 0)
                                            {
                                                EvalInfoFuncRet = "0";
                                                return EvalInfoFuncRet;
                                            }

                                            EvalInfoFuncRet = "";
                                            j = (short)(j + Strings.Len(@params[idx + 1]) + 1);
                                            string argstr_Renamed = Strings.Mid(withBlock8.Class_Renamed, j, 1);
                                            do
                                            {
                                                EvalInfoFuncRet = EvalInfoFuncRet + Strings.Mid(withBlock8.Class_Renamed, j, 1);
                                                j = (short)(j + 1);
                                            }
                                            while (GeneralLib.IsNumber(ref argstr_Renamed));
                                            if (!GeneralLib.IsNumber(ref EvalInfoFuncRet))
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }

                                    case "必要技能":
                                        {
                                            EvalInfoFuncRet = withBlock8.NecessarySkill;
                                            break;
                                        }

                                    case "使用可":
                                    case "修得":
                                        {
                                            EvalInfoFuncRet = "1";
                                            break;
                                        }
                                }
                            }
                        }
                        else if (p is object)
                        {
                            {
                                var withBlock9 = p.Data;
                                // 何番目の武器かを判定
                                if (GeneralLib.IsNumber(ref @params[idx]))
                                {
                                    i = Conversions.ToShort(@params[idx]);
                                }
                                else
                                {
                                    var loopTo10 = withBlock9.CountWeapon();
                                    for (i = 1; i <= loopTo10; i++)
                                    {
                                        WeaponData localWeapon1() { object argIndex1 = i; var ret = withBlock9.Weapon(ref argIndex1); return ret; }

                                        if ((@params[idx] ?? "") == (localWeapon1().Name ?? ""))
                                        {
                                            break;
                                        }
                                    }
                                }
                                // 指定した武器を持っていない
                                if (i <= 0 | withBlock9.CountWeapon() < i)
                                {
                                    return EvalInfoFuncRet;
                                }

                                idx = (short)(idx + 1);
                                object argIndex70 = i;
                                {
                                    var withBlock10 = withBlock9.Weapon(ref argIndex70);
                                    switch (@params[idx] ?? "")
                                    {
                                        case var case3 when case3 == "":
                                        case "名称":
                                            {
                                                EvalInfoFuncRet = withBlock10.Name;
                                                break;
                                            }

                                        case "攻撃力":
                                            {
                                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock10.Power);
                                                break;
                                            }

                                        case "射程":
                                        case "最大射程":
                                            {
                                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock10.MaxRange);
                                                break;
                                            }

                                        case "最小射程":
                                            {
                                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock10.MinRange);
                                                break;
                                            }

                                        case "命中率":
                                            {
                                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock10.Precision);
                                                break;
                                            }

                                        case "最大弾数":
                                        case "弾数":
                                            {
                                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock10.Bullet);
                                                break;
                                            }

                                        case "消費ＥＮ":
                                            {
                                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock10.ENConsumption);
                                                break;
                                            }

                                        case "必要気力":
                                            {
                                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock10.NecessaryMorale);
                                                break;
                                            }

                                        case "地形適応":
                                            {
                                                EvalInfoFuncRet = withBlock10.Adaption;
                                                break;
                                            }

                                        case "クリティカル率":
                                            {
                                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock10.Critical);
                                                break;
                                            }

                                        case "属性":
                                            {
                                                EvalInfoFuncRet = withBlock10.Class_Renamed;
                                                break;
                                            }

                                        case "属性所有":
                                            {
                                                if (GeneralLib.InStrNotNest(ref withBlock10.Class_Renamed, ref @params[idx + 1]) > 0)
                                                {
                                                    EvalInfoFuncRet = "1";
                                                }
                                                else
                                                {
                                                    EvalInfoFuncRet = "0";
                                                }

                                                break;
                                            }

                                        case "属性レベル":
                                            {
                                                string argstring21 = @params[idx + 1] + "L";
                                                j = GeneralLib.InStrNotNest(ref withBlock10.Class_Renamed, ref argstring21);
                                                if (j == 0)
                                                {
                                                    EvalInfoFuncRet = "0";
                                                    return EvalInfoFuncRet;
                                                }

                                                EvalInfoFuncRet = "";
                                                j = (short)(j + Strings.Len(@params[idx + 1]) + 1);
                                                string argstr_Renamed1 = Strings.Mid(withBlock10.Class_Renamed, j, 1);
                                                do
                                                {
                                                    EvalInfoFuncRet = EvalInfoFuncRet + Strings.Mid(withBlock10.Class_Renamed, j, 1);
                                                    j = (short)(j + 1);
                                                }
                                                while (GeneralLib.IsNumber(ref argstr_Renamed1));
                                                if (!GeneralLib.IsNumber(ref EvalInfoFuncRet))
                                                {
                                                    EvalInfoFuncRet = "0";
                                                }

                                                break;
                                            }

                                        case "必要技能":
                                            {
                                                EvalInfoFuncRet = withBlock10.NecessarySkill;
                                                break;
                                            }

                                        case "使用可":
                                        case "修得":
                                            {
                                                EvalInfoFuncRet = "1";
                                                break;
                                            }
                                    }
                                }
                            }
                        }
                        else if (pd is object)
                        {
                            // 何番目の武器かを判定
                            if (GeneralLib.IsNumber(ref @params[idx]))
                            {
                                i = Conversions.ToShort(@params[idx]);
                            }
                            else
                            {
                                var loopTo11 = pd.CountWeapon();
                                for (i = 1; i <= loopTo11; i++)
                                {
                                    WeaponData localWeapon2() { object argIndex1 = i; var ret = pd.Weapon(ref argIndex1); return ret; }

                                    if ((@params[idx] ?? "") == (localWeapon2().Name ?? ""))
                                    {
                                        break;
                                    }
                                }
                            }
                            // 指定した武器を持っていない
                            if (i <= 0 | pd.CountWeapon() < i)
                            {
                                return EvalInfoFuncRet;
                            }

                            idx = (short)(idx + 1);
                            object argIndex71 = i;
                            {
                                var withBlock11 = pd.Weapon(ref argIndex71);
                                switch (@params[idx] ?? "")
                                {
                                    case var case4 when case4 == "":
                                    case "名称":
                                        {
                                            EvalInfoFuncRet = withBlock11.Name;
                                            break;
                                        }

                                    case "攻撃力":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock11.Power);
                                            break;
                                        }

                                    case "射程":
                                    case "最大射程":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock11.MaxRange);
                                            break;
                                        }

                                    case "最小射程":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock11.MinRange);
                                            break;
                                        }

                                    case "命中率":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock11.Precision);
                                            break;
                                        }

                                    case "最大弾数":
                                    case "弾数":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock11.Bullet);
                                            break;
                                        }

                                    case "消費ＥＮ":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock11.ENConsumption);
                                            break;
                                        }

                                    case "必要気力":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock11.NecessaryMorale);
                                            break;
                                        }

                                    case "地形適応":
                                        {
                                            EvalInfoFuncRet = withBlock11.Adaption;
                                            break;
                                        }

                                    case "クリティカル率":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock11.Critical);
                                            break;
                                        }

                                    case "属性":
                                        {
                                            EvalInfoFuncRet = withBlock11.Class_Renamed;
                                            break;
                                        }

                                    case "属性所有":
                                        {
                                            if (GeneralLib.InStrNotNest(ref withBlock11.Class_Renamed, ref @params[idx + 1]) > 0)
                                            {
                                                EvalInfoFuncRet = "1";
                                            }
                                            else
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }

                                    case "属性レベル":
                                        {
                                            string argstring22 = @params[idx + 1] + "L";
                                            j = GeneralLib.InStrNotNest(ref withBlock11.Class_Renamed, ref argstring22);
                                            if (j == 0)
                                            {
                                                EvalInfoFuncRet = "0";
                                                return EvalInfoFuncRet;
                                            }

                                            EvalInfoFuncRet = "";
                                            j = (short)(j + Strings.Len(@params[idx + 1]) + 1);
                                            string argstr_Renamed2 = Strings.Mid(withBlock11.Class_Renamed, j, 1);
                                            do
                                            {
                                                EvalInfoFuncRet = EvalInfoFuncRet + Strings.Mid(withBlock11.Class_Renamed, j, 1);
                                                j = (short)(j + 1);
                                            }
                                            while (GeneralLib.IsNumber(ref argstr_Renamed2));
                                            if (!GeneralLib.IsNumber(ref EvalInfoFuncRet))
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }

                                    case "必要技能":
                                        {
                                            EvalInfoFuncRet = withBlock11.NecessarySkill;
                                            break;
                                        }

                                    case "使用可":
                                    case "修得":
                                        {
                                            EvalInfoFuncRet = "1";
                                            break;
                                        }
                                }
                            }
                        }
                        else if (it is object)
                        {
                            // 何番目の武器かを判定
                            if (GeneralLib.IsNumber(ref @params[idx]))
                            {
                                i = Conversions.ToShort(@params[idx]);
                            }
                            else
                            {
                                var loopTo12 = it.CountWeapon();
                                for (i = 1; i <= loopTo12; i++)
                                {
                                    WeaponData localWeapon3() { object argIndex1 = i; var ret = it.Weapon(ref argIndex1); return ret; }

                                    if ((@params[idx] ?? "") == (localWeapon3().Name ?? ""))
                                    {
                                        break;
                                    }
                                }
                            }
                            // 指定した武器を持っていない
                            if (i <= 0 | it.CountWeapon() < i)
                            {
                                return EvalInfoFuncRet;
                            }

                            idx = (short)(idx + 1);
                            object argIndex72 = i;
                            {
                                var withBlock12 = it.Weapon(ref argIndex72);
                                switch (@params[idx] ?? "")
                                {
                                    case var case5 when case5 == "":
                                    case "名称":
                                        {
                                            EvalInfoFuncRet = withBlock12.Name;
                                            break;
                                        }

                                    case "攻撃力":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock12.Power);
                                            break;
                                        }

                                    case "射程":
                                    case "最大射程":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock12.MaxRange);
                                            break;
                                        }

                                    case "最小射程":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock12.MinRange);
                                            break;
                                        }

                                    case "命中率":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock12.Precision);
                                            break;
                                        }

                                    case "最大弾数":
                                    case "弾数":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock12.Bullet);
                                            break;
                                        }

                                    case "消費ＥＮ":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock12.ENConsumption);
                                            break;
                                        }

                                    case "必要気力":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock12.NecessaryMorale);
                                            break;
                                        }

                                    case "地形適応":
                                        {
                                            EvalInfoFuncRet = withBlock12.Adaption;
                                            break;
                                        }

                                    case "クリティカル率":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock12.Critical);
                                            break;
                                        }

                                    case "属性":
                                        {
                                            EvalInfoFuncRet = withBlock12.Class_Renamed;
                                            break;
                                        }

                                    case "属性所有":
                                        {
                                            if (GeneralLib.InStrNotNest(ref withBlock12.Class_Renamed, ref @params[idx + 1]) > 0)
                                            {
                                                EvalInfoFuncRet = "1";
                                            }
                                            else
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }

                                    case "属性レベル":
                                        {
                                            string argstring23 = @params[idx + 1] + "L";
                                            j = GeneralLib.InStrNotNest(ref withBlock12.Class_Renamed, ref argstring23);
                                            if (j == 0)
                                            {
                                                EvalInfoFuncRet = "0";
                                                return EvalInfoFuncRet;
                                            }

                                            EvalInfoFuncRet = "";
                                            j = (short)(j + Strings.Len(@params[idx + 1]) + 1);
                                            string argstr_Renamed3 = Strings.Mid(withBlock12.Class_Renamed, j, 1);
                                            do
                                            {
                                                EvalInfoFuncRet = EvalInfoFuncRet + Strings.Mid(withBlock12.Class_Renamed, j, 1);
                                                j = (short)(j + 1);
                                            }
                                            while (GeneralLib.IsNumber(ref argstr_Renamed3));
                                            if (!GeneralLib.IsNumber(ref EvalInfoFuncRet))
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }

                                    case "必要技能":
                                        {
                                            EvalInfoFuncRet = withBlock12.NecessarySkill;
                                            break;
                                        }

                                    case "使用可":
                                    case "修得":
                                        {
                                            EvalInfoFuncRet = "1";
                                            break;
                                        }
                                }
                            }
                        }
                        else if (itd is object)
                        {
                            // 何番目の武器かを判定
                            if (GeneralLib.IsNumber(ref @params[idx]))
                            {
                                i = Conversions.ToShort(@params[idx]);
                            }
                            else
                            {
                                var loopTo13 = itd.CountWeapon();
                                for (i = 1; i <= loopTo13; i++)
                                {
                                    WeaponData localWeapon4() { object argIndex1 = i; var ret = itd.Weapon(ref argIndex1); return ret; }

                                    if ((@params[idx] ?? "") == (localWeapon4().Name ?? ""))
                                    {
                                        break;
                                    }
                                }
                            }
                            // 指定した武器を持っていない
                            if (i <= 0 | itd.CountWeapon() < i)
                            {
                                return EvalInfoFuncRet;
                            }

                            idx = (short)(idx + 1);
                            object argIndex73 = i;
                            {
                                var withBlock13 = itd.Weapon(ref argIndex73);
                                switch (@params[idx] ?? "")
                                {
                                    case var case6 when case6 == "":
                                    case "名称":
                                        {
                                            EvalInfoFuncRet = withBlock13.Name;
                                            break;
                                        }

                                    case "攻撃力":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock13.Power);
                                            break;
                                        }

                                    case "射程":
                                    case "最大射程":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock13.MaxRange);
                                            break;
                                        }

                                    case "最小射程":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock13.MinRange);
                                            break;
                                        }

                                    case "命中率":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock13.Precision);
                                            break;
                                        }

                                    case "最大弾数":
                                    case "弾数":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock13.Bullet);
                                            break;
                                        }

                                    case "消費ＥＮ":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock13.ENConsumption);
                                            break;
                                        }

                                    case "必要気力":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock13.NecessaryMorale);
                                            break;
                                        }

                                    case "地形適応":
                                        {
                                            EvalInfoFuncRet = withBlock13.Adaption;
                                            break;
                                        }

                                    case "クリティカル率":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock13.Critical);
                                            break;
                                        }

                                    case "属性":
                                        {
                                            EvalInfoFuncRet = withBlock13.Class_Renamed;
                                            break;
                                        }

                                    case "属性所有":
                                        {
                                            if (GeneralLib.InStrNotNest(ref withBlock13.Class_Renamed, ref @params[idx + 1]) > 0)
                                            {
                                                EvalInfoFuncRet = "1";
                                            }
                                            else
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }

                                    case "属性レベル":
                                        {
                                            string argstring24 = @params[idx + 1] + "L";
                                            j = GeneralLib.InStrNotNest(ref withBlock13.Class_Renamed, ref argstring24);
                                            if (j == 0)
                                            {
                                                EvalInfoFuncRet = "0";
                                                return EvalInfoFuncRet;
                                            }

                                            EvalInfoFuncRet = "";
                                            j = (short)(j + Strings.Len(@params[idx + 1]) + 1);
                                            string argstr_Renamed4 = Strings.Mid(withBlock13.Class_Renamed, j, 1);
                                            do
                                            {
                                                EvalInfoFuncRet = EvalInfoFuncRet + Strings.Mid(withBlock13.Class_Renamed, j, 1);
                                                j = (short)(j + 1);
                                            }
                                            while (GeneralLib.IsNumber(ref argstr_Renamed4));
                                            if (!GeneralLib.IsNumber(ref EvalInfoFuncRet))
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }

                                    case "必要技能":
                                        {
                                            EvalInfoFuncRet = withBlock13.NecessarySkill;
                                            break;
                                        }

                                    case "使用可":
                                    case "修得":
                                        {
                                            EvalInfoFuncRet = "1";
                                            break;
                                        }
                                }
                            }
                        }

                        break;
                    }

                case "アビリティ数":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.CountAbility());
                        }
                        else if (ud is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(ud.CountAbility());
                        }
                        else if (p is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(p.Data.CountAbility());
                        }
                        else if (pd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(pd.CountAbility());
                        }
                        else if (it is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(it.CountAbility());
                        }
                        else if (itd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(itd.CountAbility());
                        }

                        break;
                    }

                case "アビリティ":
                    {
                        idx = (short)(idx + 1);
                        if (u is object)
                        {
                            {
                                var withBlock14 = u;
                                // 何番目のアビリティかを判定
                                if (GeneralLib.IsNumber(ref @params[idx]))
                                {
                                    i = Conversions.ToShort(@params[idx]);
                                }
                                else
                                {
                                    var loopTo14 = withBlock14.CountAbility();
                                    for (i = 1; i <= loopTo14; i++)
                                    {
                                        if ((@params[idx] ?? "") == (withBlock14.Ability(i).Name ?? ""))
                                        {
                                            break;
                                        }
                                    }
                                }
                                // 指定したアビリティを持っていない
                                if (i <= 0 | withBlock14.CountAbility() < i)
                                {
                                    return EvalInfoFuncRet;
                                }

                                idx = (short)(idx + 1);
                                switch (@params[idx] ?? "")
                                {
                                    case var case7 when case7 == "":
                                    case "名称":
                                        {
                                            EvalInfoFuncRet = withBlock14.Ability(i).Name;
                                            break;
                                        }

                                    case "効果数":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock14.Ability(i).CountEffect());
                                            break;
                                        }

                                    case "効果タイプ":
                                        {
                                            // 何番目の効果かを判定
                                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                                            {
                                                j = Conversions.ToShort(@params[idx + 1]);
                                            }

                                            if (j <= 0 & withBlock14.Ability(i).CountEffect() < j)
                                            {
                                                return EvalInfoFuncRet;
                                            }

                                            object argIndex74 = j;
                                            EvalInfoFuncRet = withBlock14.Ability(i).EffectType(ref argIndex74);
                                            break;
                                        }

                                    case "効果レベル":
                                        {
                                            // 何番目の効果かを判定
                                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                                            {
                                                j = Conversions.ToShort(@params[idx + 1]);
                                            }

                                            if (j <= 0 & withBlock14.Ability(i).CountEffect() < j)
                                            {
                                                return EvalInfoFuncRet;
                                            }

                                            double localEffectLevel() { object argIndex1 = j; var ret = withBlock14.Ability(i).EffectLevel(ref argIndex1); return ret; }

                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(localEffectLevel());
                                            break;
                                        }

                                    case "効果データ":
                                        {
                                            // 何番目の効果かを判定
                                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                                            {
                                                j = Conversions.ToShort(@params[idx + 1]);
                                            }

                                            if (j <= 0 & withBlock14.Ability(i).CountEffect() < j)
                                            {
                                                return EvalInfoFuncRet;
                                            }

                                            object argIndex75 = j;
                                            EvalInfoFuncRet = withBlock14.Ability(i).EffectData(ref argIndex75);
                                            break;
                                        }

                                    case "射程":
                                    case "最大射程":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock14.AbilityMaxRange(i));
                                            break;
                                        }

                                    case "最小射程":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock14.AbilityMinRange(i));
                                            break;
                                        }

                                    case "最大使用回数":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock14.MaxStock(i));
                                            break;
                                        }

                                    case "使用回数":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock14.Stock(i));
                                            break;
                                        }

                                    case "消費ＥＮ":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock14.AbilityENConsumption(i));
                                            break;
                                        }

                                    case "必要気力":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock14.Ability(i).NecessaryMorale);
                                            break;
                                        }

                                    case "属性":
                                        {
                                            EvalInfoFuncRet = withBlock14.Ability(i).Class_Renamed;
                                            break;
                                        }

                                    case "属性所有":
                                        {
                                            if (withBlock14.IsAbilityClassifiedAs(i, ref @params[idx + 1]))
                                            {
                                                EvalInfoFuncRet = "1";
                                            }
                                            else
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }

                                    case "属性レベル":
                                        {
                                            EvalInfoFuncRet = withBlock14.AbilityLevel(i, ref @params[idx + 1]).ToString();
                                            break;
                                        }

                                    case "属性名称":
                                        {
                                            EvalInfoFuncRet = Help.AttributeName(ref u, ref @params[idx + 1], true);
                                            break;
                                        }

                                    case "属性解説":
                                        {
                                            EvalInfoFuncRet = Help.AttributeHelpMessage(ref u, ref @params[idx + 1], i, true);
                                            break;
                                        }

                                    case "必要技能":
                                        {
                                            EvalInfoFuncRet = withBlock14.Ability(i).NecessarySkill;
                                            break;
                                        }

                                    case "使用可":
                                        {
                                            string argref_mode1 = "移動前";
                                            if (withBlock14.IsAbilityAvailable(i, ref argref_mode1))
                                            {
                                                EvalInfoFuncRet = "1";
                                            }
                                            else
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }

                                    case "修得":
                                        {
                                            if (withBlock14.IsAbilityMastered(i))
                                            {
                                                EvalInfoFuncRet = "1";
                                            }
                                            else
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }
                                }
                            }
                        }
                        else if (ud is object)
                        {
                            // 何番目のアビリティかを判定
                            if (GeneralLib.IsNumber(ref @params[idx]))
                            {
                                i = Conversions.ToShort(@params[idx]);
                            }
                            else
                            {
                                var loopTo15 = ud.CountAbility();
                                for (i = 1; i <= loopTo15; i++)
                                {
                                    AbilityData localAbility() { object argIndex1 = i; var ret = ud.Ability(ref argIndex1); return ret; }

                                    if ((@params[idx] ?? "") == (localAbility().Name ?? ""))
                                    {
                                        break;
                                    }
                                }
                            }
                            // 指定したアビリティを持っていない
                            if (i <= 0 | ud.CountAbility() < i)
                            {
                                return EvalInfoFuncRet;
                            }

                            idx = (short)(idx + 1);
                            object argIndex78 = i;
                            {
                                var withBlock15 = ud.Ability(ref argIndex78);
                                switch (@params[idx] ?? "")
                                {
                                    case var case8 when case8 == "":
                                    case "名称":
                                        {
                                            EvalInfoFuncRet = withBlock15.Name;
                                            break;
                                        }

                                    case "効果数":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock15.CountEffect());
                                            break;
                                        }

                                    case "効果タイプ":
                                        {
                                            // 何番目の効果かを判定
                                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                                            {
                                                j = Conversions.ToShort(@params[idx + 1]);
                                            }

                                            if (j <= 0 | withBlock15.CountEffect() < j)
                                            {
                                                return EvalInfoFuncRet;
                                            }

                                            object argIndex76 = j;
                                            EvalInfoFuncRet = withBlock15.EffectType(ref argIndex76);
                                            break;
                                        }

                                    case "効果レベル":
                                        {
                                            // 何番目の効果かを判定
                                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                                            {
                                                j = Conversions.ToShort(@params[idx + 1]);
                                            }

                                            if (j <= 0 | withBlock15.CountEffect() < j)
                                            {
                                                return EvalInfoFuncRet;
                                            }

                                            double localEffectLevel1() { object argIndex1 = j; var ret = withBlock15.EffectLevel(ref argIndex1); return ret; }

                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(localEffectLevel1());
                                            break;
                                        }

                                    case "効果データ":
                                        {
                                            // 何番目の効果かを判定
                                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                                            {
                                                j = Conversions.ToShort(@params[idx + 1]);
                                            }

                                            if (j <= 0 | withBlock15.CountEffect() < j)
                                            {
                                                return EvalInfoFuncRet;
                                            }

                                            object argIndex77 = j;
                                            EvalInfoFuncRet = withBlock15.EffectData(ref argIndex77);
                                            break;
                                        }

                                    case "射程":
                                    case "最大射程":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock15.MaxRange);
                                            break;
                                        }

                                    case "最小射程":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock15.MinRange);
                                            break;
                                        }

                                    case "最大使用回数":
                                    case "使用回数":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock15.Stock);
                                            break;
                                        }

                                    case "消費ＥＮ":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock15.ENConsumption);
                                            break;
                                        }

                                    case "必要気力":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock15.NecessaryMorale);
                                            break;
                                        }

                                    case "属性":
                                        {
                                            EvalInfoFuncRet = withBlock15.Class_Renamed;
                                            break;
                                        }

                                    case "属性所有":
                                        {
                                            if (GeneralLib.InStrNotNest(ref withBlock15.Class_Renamed, ref @params[idx + 1]) > 0)
                                            {
                                                EvalInfoFuncRet = "1";
                                            }
                                            else
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }

                                    case "属性レベル":
                                        {
                                            string argstring25 = @params[idx + 1] + "L";
                                            j = GeneralLib.InStrNotNest(ref withBlock15.Class_Renamed, ref argstring25);
                                            if (j == 0)
                                            {
                                                EvalInfoFuncRet = "0";
                                                return EvalInfoFuncRet;
                                            }

                                            EvalInfoFuncRet = "";
                                            j = (short)(j + Strings.Len(@params[idx + 1]) + 1);
                                            string argstr_Renamed5 = Strings.Mid(withBlock15.Class_Renamed, j, 1);
                                            do
                                            {
                                                EvalInfoFuncRet = EvalInfoFuncRet + Strings.Mid(withBlock15.Class_Renamed, j, 1);
                                                j = (short)(j + 1);
                                            }
                                            while (GeneralLib.IsNumber(ref argstr_Renamed5));
                                            if (!GeneralLib.IsNumber(ref EvalInfoFuncRet))
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }

                                    case "必要技能":
                                        {
                                            EvalInfoFuncRet = withBlock15.NecessarySkill;
                                            break;
                                        }

                                    case "使用可":
                                    case "修得":
                                        {
                                            EvalInfoFuncRet = "1";
                                            break;
                                        }
                                }
                            }
                        }
                        else if (p is object)
                        {
                            {
                                var withBlock16 = p.Data;
                                // 何番目のアビリティかを判定
                                if (GeneralLib.IsNumber(ref @params[idx]))
                                {
                                    i = Conversions.ToShort(@params[idx]);
                                }
                                else
                                {
                                    var loopTo16 = withBlock16.CountAbility();
                                    for (i = 1; i <= loopTo16; i++)
                                    {
                                        AbilityData localAbility1() { object argIndex1 = i; var ret = withBlock16.Ability(ref argIndex1); return ret; }

                                        if ((@params[idx] ?? "") == (localAbility1().Name ?? ""))
                                        {
                                            break;
                                        }
                                    }
                                }
                                // 指定したアビリティを持っていない
                                if (i <= 0 | withBlock16.CountAbility() < i)
                                {
                                    return EvalInfoFuncRet;
                                }

                                idx = (short)(idx + 1);
                                object argIndex81 = i;
                                {
                                    var withBlock17 = withBlock16.Ability(ref argIndex81);
                                    switch (@params[idx] ?? "")
                                    {
                                        case var case9 when case9 == "":
                                        case "名称":
                                            {
                                                EvalInfoFuncRet = withBlock17.Name;
                                                break;
                                            }

                                        case "効果数":
                                            {
                                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock17.CountEffect());
                                                break;
                                            }

                                        case "効果タイプ":
                                            {
                                                // 何番目の効果かを判定
                                                if (GeneralLib.IsNumber(ref @params[idx + 1]))
                                                {
                                                    j = Conversions.ToShort(@params[idx + 1]);
                                                }

                                                if (j <= 0 | withBlock17.CountEffect() < j)
                                                {
                                                    return EvalInfoFuncRet;
                                                }

                                                object argIndex79 = j;
                                                EvalInfoFuncRet = withBlock17.EffectType(ref argIndex79);
                                                break;
                                            }

                                        case "効果レベル":
                                            {
                                                // 何番目の効果かを判定
                                                if (GeneralLib.IsNumber(ref @params[idx + 1]))
                                                {
                                                    j = Conversions.ToShort(@params[idx + 1]);
                                                }

                                                if (j <= 0 | withBlock17.CountEffect() < j)
                                                {
                                                    return EvalInfoFuncRet;
                                                }

                                                double localEffectLevel2() { object argIndex1 = j; var ret = withBlock17.EffectLevel(ref argIndex1); return ret; }

                                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(localEffectLevel2());
                                                break;
                                            }

                                        case "効果データ":
                                            {
                                                // 何番目の効果かを判定
                                                if (GeneralLib.IsNumber(ref @params[idx + 1]))
                                                {
                                                    j = Conversions.ToShort(@params[idx + 1]);
                                                }

                                                if (j <= 0 | withBlock17.CountEffect() < j)
                                                {
                                                    return EvalInfoFuncRet;
                                                }

                                                object argIndex80 = j;
                                                EvalInfoFuncRet = withBlock17.EffectData(ref argIndex80);
                                                break;
                                            }

                                        case "射程":
                                        case "最大射程":
                                            {
                                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock17.MaxRange);
                                                break;
                                            }

                                        case "最小射程":
                                            {
                                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock17.MinRange);
                                                break;
                                            }

                                        case "最大使用回数":
                                        case "使用回数":
                                            {
                                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock17.Stock);
                                                break;
                                            }

                                        case "消費ＥＮ":
                                            {
                                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock17.ENConsumption);
                                                break;
                                            }

                                        case "必要気力":
                                            {
                                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock17.NecessaryMorale);
                                                break;
                                            }

                                        case "属性":
                                            {
                                                EvalInfoFuncRet = withBlock17.Class_Renamed;
                                                break;
                                            }

                                        case "属性所有":
                                            {
                                                if (GeneralLib.InStrNotNest(ref withBlock17.Class_Renamed, ref @params[idx + 1]) > 0)
                                                {
                                                    EvalInfoFuncRet = "1";
                                                }
                                                else
                                                {
                                                    EvalInfoFuncRet = "0";
                                                }

                                                break;
                                            }

                                        case "属性レベル":
                                            {
                                                string argstring26 = @params[idx + 1] + "L";
                                                j = GeneralLib.InStrNotNest(ref withBlock17.Class_Renamed, ref argstring26);
                                                if (j == 0)
                                                {
                                                    EvalInfoFuncRet = "0";
                                                    return EvalInfoFuncRet;
                                                }

                                                EvalInfoFuncRet = "";
                                                j = (short)(j + Strings.Len(@params[idx + 1]) + 1);
                                                string argstr_Renamed6 = Strings.Mid(withBlock17.Class_Renamed, j, 1);
                                                do
                                                {
                                                    EvalInfoFuncRet = EvalInfoFuncRet + Strings.Mid(withBlock17.Class_Renamed, j, 1);
                                                    j = (short)(j + 1);
                                                }
                                                while (GeneralLib.IsNumber(ref argstr_Renamed6));
                                                if (!GeneralLib.IsNumber(ref EvalInfoFuncRet))
                                                {
                                                    EvalInfoFuncRet = "0";
                                                }

                                                break;
                                            }

                                        case "必要技能":
                                            {
                                                EvalInfoFuncRet = withBlock17.NecessarySkill;
                                                break;
                                            }

                                        case "使用可":
                                        case "修得":
                                            {
                                                EvalInfoFuncRet = "1";
                                                break;
                                            }
                                    }
                                }
                            }
                        }
                        else if (pd is object)
                        {
                            // 何番目のアビリティかを判定
                            if (GeneralLib.IsNumber(ref @params[idx]))
                            {
                                i = Conversions.ToShort(@params[idx]);
                            }
                            else
                            {
                                var loopTo17 = pd.CountAbility();
                                for (i = 1; i <= loopTo17; i++)
                                {
                                    AbilityData localAbility2() { object argIndex1 = i; var ret = pd.Ability(ref argIndex1); return ret; }

                                    if ((@params[idx] ?? "") == (localAbility2().Name ?? ""))
                                    {
                                        break;
                                    }
                                }
                            }
                            // 指定したアビリティを持っていない
                            if (i <= 0 | pd.CountAbility() < i)
                            {
                                return EvalInfoFuncRet;
                            }

                            idx = (short)(idx + 1);
                            object argIndex84 = i;
                            {
                                var withBlock18 = pd.Ability(ref argIndex84);
                                switch (@params[idx] ?? "")
                                {
                                    case var case10 when case10 == "":
                                    case "名称":
                                        {
                                            EvalInfoFuncRet = withBlock18.Name;
                                            break;
                                        }

                                    case "効果数":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock18.CountEffect());
                                            break;
                                        }

                                    case "効果タイプ":
                                        {
                                            // 何番目の効果かを判定
                                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                                            {
                                                j = Conversions.ToShort(@params[idx + 1]);
                                            }

                                            if (j <= 0 | withBlock18.CountEffect() < j)
                                            {
                                                return EvalInfoFuncRet;
                                            }

                                            object argIndex82 = j;
                                            EvalInfoFuncRet = withBlock18.EffectType(ref argIndex82);
                                            break;
                                        }

                                    case "効果レベル":
                                        {
                                            // 何番目の効果かを判定
                                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                                            {
                                                j = Conversions.ToShort(@params[idx + 1]);
                                            }

                                            if (j <= 0 | withBlock18.CountEffect() < j)
                                            {
                                                return EvalInfoFuncRet;
                                            }

                                            double localEffectLevel3() { object argIndex1 = j; var ret = withBlock18.EffectLevel(ref argIndex1); return ret; }

                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(localEffectLevel3());
                                            break;
                                        }

                                    case "効果データ":
                                        {
                                            // 何番目の効果かを判定
                                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                                            {
                                                j = Conversions.ToShort(@params[idx + 1]);
                                            }

                                            if (j <= 0 | withBlock18.CountEffect() < j)
                                            {
                                                return EvalInfoFuncRet;
                                            }

                                            object argIndex83 = j;
                                            EvalInfoFuncRet = withBlock18.EffectData(ref argIndex83);
                                            break;
                                        }

                                    case "射程":
                                    case "最大射程":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock18.MaxRange);
                                            break;
                                        }

                                    case "最小射程":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock18.MinRange);
                                            break;
                                        }

                                    case "最大使用回数":
                                    case "使用回数":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock18.Stock);
                                            break;
                                        }

                                    case "消費ＥＮ":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock18.ENConsumption);
                                            break;
                                        }

                                    case "必要気力":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock18.NecessaryMorale);
                                            break;
                                        }

                                    case "属性":
                                        {
                                            EvalInfoFuncRet = withBlock18.Class_Renamed;
                                            break;
                                        }

                                    case "属性所有":
                                        {
                                            if (GeneralLib.InStrNotNest(ref withBlock18.Class_Renamed, ref @params[idx + 1]) > 0)
                                            {
                                                EvalInfoFuncRet = "1";
                                            }
                                            else
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }

                                    case "属性レベル":
                                        {
                                            string argstring27 = @params[idx + 1] + "L";
                                            j = GeneralLib.InStrNotNest(ref withBlock18.Class_Renamed, ref argstring27);
                                            if (j == 0)
                                            {
                                                EvalInfoFuncRet = "0";
                                                return EvalInfoFuncRet;
                                            }

                                            EvalInfoFuncRet = "";
                                            j = (short)(j + Strings.Len(@params[idx + 1]) + 1);
                                            string argstr_Renamed7 = Strings.Mid(withBlock18.Class_Renamed, j, 1);
                                            do
                                            {
                                                EvalInfoFuncRet = EvalInfoFuncRet + Strings.Mid(withBlock18.Class_Renamed, j, 1);
                                                j = (short)(j + 1);
                                            }
                                            while (GeneralLib.IsNumber(ref argstr_Renamed7));
                                            if (!GeneralLib.IsNumber(ref EvalInfoFuncRet))
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }

                                    case "必要技能":
                                        {
                                            EvalInfoFuncRet = withBlock18.NecessarySkill;
                                            break;
                                        }

                                    case "使用可":
                                    case "修得":
                                        {
                                            EvalInfoFuncRet = "1";
                                            break;
                                        }
                                }
                            }
                        }
                        else if (it is object)
                        {
                            // 何番目のアビリティかを判定
                            if (GeneralLib.IsNumber(ref @params[idx]))
                            {
                                i = Conversions.ToShort(@params[idx]);
                            }
                            else
                            {
                                var loopTo18 = it.CountAbility();
                                for (i = 1; i <= loopTo18; i++)
                                {
                                    AbilityData localAbility3() { object argIndex1 = i; var ret = it.Ability(ref argIndex1); return ret; }

                                    if ((@params[idx] ?? "") == (localAbility3().Name ?? ""))
                                    {
                                        break;
                                    }
                                }
                            }
                            // 指定したアビリティを持っていない
                            if (i <= 0 | it.CountAbility() < i)
                            {
                                return EvalInfoFuncRet;
                            }

                            idx = (short)(idx + 1);
                            object argIndex87 = i;
                            {
                                var withBlock19 = it.Ability(ref argIndex87);
                                switch (@params[idx] ?? "")
                                {
                                    case var case11 when case11 == "":
                                    case "名称":
                                        {
                                            EvalInfoFuncRet = withBlock19.Name;
                                            break;
                                        }

                                    case "効果数":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock19.CountEffect());
                                            break;
                                        }

                                    case "効果タイプ":
                                        {
                                            // 何番目の効果かを判定
                                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                                            {
                                                j = Conversions.ToShort(@params[idx + 1]);
                                            }

                                            if (j <= 0 | withBlock19.CountEffect() < j)
                                            {
                                                return EvalInfoFuncRet;
                                            }

                                            object argIndex85 = j;
                                            EvalInfoFuncRet = withBlock19.EffectType(ref argIndex85);
                                            break;
                                        }

                                    case "効果レベル":
                                        {
                                            // 何番目の効果かを判定
                                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                                            {
                                                j = Conversions.ToShort(@params[idx + 1]);
                                            }

                                            if (j <= 0 | withBlock19.CountEffect() < j)
                                            {
                                                return EvalInfoFuncRet;
                                            }

                                            double localEffectLevel4() { object argIndex1 = j; var ret = withBlock19.EffectLevel(ref argIndex1); return ret; }

                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(localEffectLevel4());
                                            break;
                                        }

                                    case "効果データ":
                                        {
                                            // 何番目の効果かを判定
                                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                                            {
                                                j = Conversions.ToShort(@params[idx + 1]);
                                            }

                                            if (j <= 0 | withBlock19.CountEffect() < j)
                                            {
                                                return EvalInfoFuncRet;
                                            }

                                            object argIndex86 = j;
                                            EvalInfoFuncRet = withBlock19.EffectData(ref argIndex86);
                                            break;
                                        }

                                    case "射程":
                                    case "最大射程":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock19.MaxRange);
                                            break;
                                        }

                                    case "最小射程":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock19.MinRange);
                                            break;
                                        }

                                    case "最大使用回数":
                                    case "使用回数":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock19.Stock);
                                            break;
                                        }

                                    case "消費ＥＮ":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock19.ENConsumption);
                                            break;
                                        }

                                    case "必要気力":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock19.NecessaryMorale);
                                            break;
                                        }

                                    case "属性":
                                        {
                                            EvalInfoFuncRet = withBlock19.Class_Renamed;
                                            break;
                                        }

                                    case "属性所有":
                                        {
                                            if (GeneralLib.InStrNotNest(ref withBlock19.Class_Renamed, ref @params[idx + 1]) > 0)
                                            {
                                                EvalInfoFuncRet = "1";
                                            }
                                            else
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }

                                    case "属性レベル":
                                        {
                                            string argstring28 = @params[idx + 1] + "L";
                                            j = GeneralLib.InStrNotNest(ref withBlock19.Class_Renamed, ref argstring28);
                                            if (j == 0)
                                            {
                                                EvalInfoFuncRet = "0";
                                                return EvalInfoFuncRet;
                                            }

                                            EvalInfoFuncRet = "";
                                            j = (short)(j + Strings.Len(@params[idx + 1]) + 1);
                                            string argstr_Renamed8 = Strings.Mid(withBlock19.Class_Renamed, j, 1);
                                            do
                                            {
                                                EvalInfoFuncRet = EvalInfoFuncRet + Strings.Mid(withBlock19.Class_Renamed, j, 1);
                                                j = (short)(j + 1);
                                            }
                                            while (GeneralLib.IsNumber(ref argstr_Renamed8));
                                            if (!GeneralLib.IsNumber(ref EvalInfoFuncRet))
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }

                                    case "必要技能":
                                        {
                                            EvalInfoFuncRet = withBlock19.NecessarySkill;
                                            break;
                                        }

                                    case "使用可":
                                    case "修得":
                                        {
                                            EvalInfoFuncRet = "1";
                                            break;
                                        }
                                }
                            }
                        }
                        else if (itd is object)
                        {
                            // 何番目のアビリティかを判定
                            if (GeneralLib.IsNumber(ref @params[idx]))
                            {
                                i = Conversions.ToShort(@params[idx]);
                            }
                            else
                            {
                                var loopTo19 = itd.CountAbility();
                                for (i = 1; i <= loopTo19; i++)
                                {
                                    AbilityData localAbility4() { object argIndex1 = i; var ret = itd.Ability(ref argIndex1); return ret; }

                                    if ((@params[idx] ?? "") == (localAbility4().Name ?? ""))
                                    {
                                        break;
                                    }
                                }
                            }
                            // 指定したアビリティを持っていない
                            if (i <= 0 | itd.CountAbility() < i)
                            {
                                return EvalInfoFuncRet;
                            }

                            idx = (short)(idx + 1);
                            object argIndex90 = i;
                            {
                                var withBlock20 = itd.Ability(ref argIndex90);
                                switch (@params[idx] ?? "")
                                {
                                    case var case12 when case12 == "":
                                    case "名称":
                                        {
                                            EvalInfoFuncRet = withBlock20.Name;
                                            break;
                                        }

                                    case "効果数":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock20.CountEffect());
                                            break;
                                        }

                                    case "効果タイプ":
                                        {
                                            // 何番目の効果かを判定
                                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                                            {
                                                j = Conversions.ToShort(@params[idx + 1]);
                                            }

                                            if (j <= 0 | withBlock20.CountEffect() < j)
                                            {
                                                return EvalInfoFuncRet;
                                            }

                                            object argIndex88 = j;
                                            EvalInfoFuncRet = withBlock20.EffectType(ref argIndex88);
                                            break;
                                        }

                                    case "効果レベル":
                                        {
                                            // 何番目の効果かを判定
                                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                                            {
                                                j = Conversions.ToShort(@params[idx + 1]);
                                            }

                                            if (j <= 0 | withBlock20.CountEffect() < j)
                                            {
                                                return EvalInfoFuncRet;
                                            }

                                            double localEffectLevel5() { object argIndex1 = j; var ret = withBlock20.EffectLevel(ref argIndex1); return ret; }

                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(localEffectLevel5());
                                            break;
                                        }

                                    case "効果データ":
                                        {
                                            // 何番目の効果かを判定
                                            if (GeneralLib.IsNumber(ref @params[idx + 1]))
                                            {
                                                j = Conversions.ToShort(@params[idx + 1]);
                                            }

                                            if (j <= 0 | withBlock20.CountEffect() < j)
                                            {
                                                return EvalInfoFuncRet;
                                            }

                                            object argIndex89 = j;
                                            EvalInfoFuncRet = withBlock20.EffectData(ref argIndex89);
                                            break;
                                        }

                                    case "射程":
                                    case "最大射程":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock20.MaxRange);
                                            break;
                                        }

                                    case "最小射程":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock20.MinRange);
                                            break;
                                        }

                                    case "最大使用回数":
                                    case "使用回数":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock20.Stock);
                                            break;
                                        }

                                    case "消費ＥＮ":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock20.ENConsumption);
                                            break;
                                        }

                                    case "必要気力":
                                        {
                                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(withBlock20.NecessaryMorale);
                                            break;
                                        }

                                    case "属性":
                                        {
                                            EvalInfoFuncRet = withBlock20.Class_Renamed;
                                            break;
                                        }

                                    case "属性所有":
                                        {
                                            if (GeneralLib.InStrNotNest(ref withBlock20.Class_Renamed, ref @params[idx + 1]) > 0)
                                            {
                                                EvalInfoFuncRet = "1";
                                            }
                                            else
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }

                                    case "属性レベル":
                                        {
                                            string argstring29 = @params[idx + 1] + "L";
                                            j = GeneralLib.InStrNotNest(ref withBlock20.Class_Renamed, ref argstring29);
                                            if (j == 0)
                                            {
                                                EvalInfoFuncRet = "0";
                                                return EvalInfoFuncRet;
                                            }

                                            EvalInfoFuncRet = "";
                                            j = (short)(j + Strings.Len(@params[idx + 1]) + 1);
                                            string argstr_Renamed9 = Strings.Mid(withBlock20.Class_Renamed, j, 1);
                                            do
                                            {
                                                EvalInfoFuncRet = EvalInfoFuncRet + Strings.Mid(withBlock20.Class_Renamed, j, 1);
                                                j = (short)(j + 1);
                                            }
                                            while (GeneralLib.IsNumber(ref argstr_Renamed9));
                                            if (!GeneralLib.IsNumber(ref EvalInfoFuncRet))
                                            {
                                                EvalInfoFuncRet = "0";
                                            }

                                            break;
                                        }

                                    case "必要技能":
                                        {
                                            EvalInfoFuncRet = withBlock20.NecessarySkill;
                                            break;
                                        }

                                    case "使用可":
                                    case "修得":
                                        {
                                            EvalInfoFuncRet = "1";
                                            break;
                                        }
                                }
                            }
                        }

                        break;
                    }

                case "ランク":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.Rank);
                        }

                        break;
                    }

                case "ボスランク":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.BossRank);
                        }

                        break;
                    }

                case "エリア":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = u.Area;
                        }

                        break;
                    }

                case "思考モード":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = u.Mode;
                        }

                        break;
                    }

                case "最大攻撃力":
                    {
                        if (u is object)
                        {
                            {
                                var withBlock21 = u;
                                max_value = 0;
                                var loopTo20 = withBlock21.CountWeapon();
                                for (i = 1; i <= loopTo20; i++)
                                {
                                    string argattr = "合";
                                    if (withBlock21.IsWeaponMastered(i) & !withBlock21.IsDisabled(ref withBlock21.Weapon(i).Name) & !withBlock21.IsWeaponClassifiedAs(i, ref argattr))
                                    {
                                        string argtarea2 = "";
                                        if (withBlock21.WeaponPower(i, ref argtarea2) > max_value)
                                        {
                                            string argtarea1 = "";
                                            max_value = withBlock21.WeaponPower(i, ref argtarea1);
                                        }
                                    }
                                }

                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(max_value);
                            }
                        }
                        else if (ud is object)
                        {
                            max_value = 0;
                            var loopTo21 = ud.CountWeapon();
                            for (i = 1; i <= loopTo21; i++)
                            {
                                WeaponData localWeapon7() { object argIndex1 = i; var ret = ud.Weapon(ref argIndex1); return ret; }

                                if (Strings.InStr(localWeapon7().Class_Renamed, "合") == 0)
                                {
                                    WeaponData localWeapon6() { object argIndex1 = i; var ret = ud.Weapon(ref argIndex1); return ret; }

                                    if (localWeapon6().Power > max_value)
                                    {
                                        WeaponData localWeapon5() { object argIndex1 = i; var ret = ud.Weapon(ref argIndex1); return ret; }

                                        max_value = localWeapon5().Power;
                                    }
                                }
                            }

                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(max_value);
                        }

                        break;
                    }

                case "最長射程":
                    {
                        if (u is object)
                        {
                            {
                                var withBlock22 = u;
                                max_value = 0;
                                var loopTo22 = withBlock22.CountWeapon();
                                for (i = 1; i <= loopTo22; i++)
                                {
                                    string argattr1 = "合";
                                    if (withBlock22.IsWeaponMastered(i) & !withBlock22.IsDisabled(ref withBlock22.Weapon(i).Name) & !withBlock22.IsWeaponClassifiedAs(i, ref argattr1))
                                    {
                                        if (withBlock22.WeaponMaxRange(i) > max_value)
                                        {
                                            max_value = withBlock22.WeaponMaxRange(i);
                                        }
                                    }
                                }

                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(max_value);
                            }
                        }
                        else if (ud is object)
                        {
                            max_value = 0;
                            var loopTo23 = ud.CountWeapon();
                            for (i = 1; i <= loopTo23; i++)
                            {
                                WeaponData localWeapon10() { object argIndex1 = i; var ret = ud.Weapon(ref argIndex1); return ret; }

                                if (Strings.InStr(localWeapon10().Class_Renamed, "合") == 0)
                                {
                                    WeaponData localWeapon9() { object argIndex1 = i; var ret = ud.Weapon(ref argIndex1); return ret; }

                                    if (localWeapon9().MaxRange > max_value)
                                    {
                                        WeaponData localWeapon8() { object argIndex1 = i; var ret = ud.Weapon(ref argIndex1); return ret; }

                                        max_value = localWeapon8().MaxRange;
                                    }
                                }
                            }

                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(max_value);
                        }

                        break;
                    }

                case "残りサポートアタック数":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.MaxSupportAttack() - u.UsedSupportAttack);
                        }

                        break;
                    }

                case "残りサポートガード数":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.MaxSupportGuard() - u.UsedSupportGuard);
                        }

                        break;
                    }

                case "残り同時援護攻撃数":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.MaxSyncAttack() - u.UsedSyncAttack);
                        }

                        break;
                    }

                case "残りカウンター攻撃数":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(u.MaxCounterAttack() - u.UsedCounterAttack);
                        }

                        break;
                    }

                case "改造費":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(InterMission.RankUpCost(ref u));
                        }

                        break;
                    }

                case "最大改造数":
                    {
                        if (u is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(InterMission.MaxRank(ref u));
                        }

                        break;
                    }

                case "アイテムクラス":
                    {
                        if (it is object)
                        {
                            EvalInfoFuncRet = it.Class_Renamed();
                        }
                        else if (itd is object)
                        {
                            EvalInfoFuncRet = itd.Class_Renamed;
                        }

                        break;
                    }

                case "装備個所":
                    {
                        if (it is object)
                        {
                            EvalInfoFuncRet = it.Part();
                        }
                        else if (itd is object)
                        {
                            EvalInfoFuncRet = itd.Part;
                        }

                        break;
                    }

                case "最大ＨＰ修正値":
                    {
                        if (it is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(it.HP());
                        }
                        else if (itd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(itd.HP);
                        }

                        break;
                    }

                case "最大ＥＮ修正値":
                    {
                        if (it is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(it.EN());
                        }
                        else if (itd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(itd.EN);
                        }

                        break;
                    }

                case "装甲修正値":
                    {
                        if (it is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(it.Armor());
                        }
                        else if (itd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(itd.Armor);
                        }

                        break;
                    }

                case "運動性修正値":
                    {
                        if (it is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(it.Mobility());
                        }
                        else if (itd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(itd.Mobility);
                        }

                        break;
                    }

                case "移動力修正値":
                    {
                        if (it is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(it.Speed());
                        }
                        else if (itd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(itd.Speed);
                        }

                        break;
                    }

                case "解説文":
                case "コメント":
                    {
                        if (it is object)
                        {
                            EvalInfoFuncRet = it.Data.Comment;
                            string args2 = Constants.vbCr + Constants.vbLf;
                            string args3 = " ";
                            GeneralLib.ReplaceString(ref EvalInfoFuncRet, ref args2, ref args3);
                        }
                        else if (itd is object)
                        {
                            EvalInfoFuncRet = itd.Comment;
                            string args21 = Constants.vbCr + Constants.vbLf;
                            string args31 = " ";
                            GeneralLib.ReplaceString(ref EvalInfoFuncRet, ref args21, ref args31);
                        }
                        else if (spd is object)
                        {
                            EvalInfoFuncRet = spd.Comment;
                        }

                        break;
                    }

                case "短縮名":
                    {
                        if (spd is object)
                        {
                            EvalInfoFuncRet = spd.ShortName;
                        }

                        break;
                    }

                case "消費ＳＰ":
                    {
                        if (spd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(spd.SPConsumption);
                        }

                        break;
                    }

                case "対象":
                    {
                        if (spd is object)
                        {
                            EvalInfoFuncRet = spd.TargetType;
                        }

                        break;
                    }

                case "持続期間":
                    {
                        if (spd is object)
                        {
                            EvalInfoFuncRet = spd.Duration;
                        }

                        break;
                    }

                case "適用条件":
                    {
                        if (spd is object)
                        {
                            EvalInfoFuncRet = spd.NecessaryCondition;
                        }

                        break;
                    }

                case "アニメ":
                    {
                        if (spd is object)
                        {
                            EvalInfoFuncRet = spd.Animation;
                        }

                        break;
                    }

                case "効果数":
                    {
                        if (spd is object)
                        {
                            EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(spd.CountEffect());
                        }

                        break;
                    }

                case "効果タイプ":
                    {
                        if (spd is object)
                        {
                            idx = (short)(idx + 1);
                            i = (short)GeneralLib.StrToLng(ref @params[idx]);
                            if (1 <= i & i <= spd.CountEffect())
                            {
                                EvalInfoFuncRet = spd.EffectType(i);
                            }
                        }

                        break;
                    }

                case "効果レベル":
                    {
                        if (spd is object)
                        {
                            idx = (short)(idx + 1);
                            i = (short)GeneralLib.StrToLng(ref @params[idx]);
                            if (1 <= i & i <= spd.CountEffect())
                            {
                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(spd.EffectLevel(i));
                            }
                        }

                        break;
                    }

                case "効果データ":
                    {
                        if (spd is object)
                        {
                            idx = (short)(idx + 1);
                            i = (short)GeneralLib.StrToLng(ref @params[idx]);
                            if (1 <= i & i <= spd.CountEffect())
                            {
                                EvalInfoFuncRet = spd.EffectData(i);
                            }
                        }

                        break;
                    }

                case "マップ":
                    {
                        idx = (short)(idx + 1);
                        switch (@params[idx] ?? "")
                        {
                            case "ファイル名":
                                {
                                    EvalInfoFuncRet = Map.MapFileName;
                                    if (Strings.Len(EvalInfoFuncRet) > Strings.Len(SRC.ScenarioPath))
                                    {
                                        if ((Strings.Left(EvalInfoFuncRet, Strings.Len(SRC.ScenarioPath)) ?? "") == (SRC.ScenarioPath ?? ""))
                                        {
                                            EvalInfoFuncRet = Strings.Mid(EvalInfoFuncRet, Strings.Len(SRC.ScenarioPath) + 1);
                                        }
                                    }

                                    break;
                                }

                            case "幅":
                                {
                                    EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(Map.MapWidth);
                                    break;
                                }

                            case "時間帯":
                                {
                                    if (!string.IsNullOrEmpty(Map.MapDrawMode))
                                    {
                                        if (Map.MapDrawMode == "フィルタ")
                                        {
                                            buf = Conversion.Hex(Map.MapDrawFilterColor);
                                            var loopTo24 = (short)(6 - Strings.Len(buf));
                                            for (i = 1; i <= loopTo24; i++)
                                                buf = "0" + buf;
                                            buf = "#" + Strings.Mid(buf, 5, 2) + Strings.Mid(buf, 3, 2) + Strings.Mid(buf, 1, 2) + " " + (Map.MapDrawFilterTransPercent * 100d).ToString() + "%";
                                        }
                                        else
                                        {
                                            buf = Map.MapDrawMode;
                                        }

                                        if (Map.MapDrawIsMapOnly)
                                        {
                                            buf = buf + " マップ限定";
                                        }

                                        EvalInfoFuncRet = buf;
                                    }
                                    else
                                    {
                                        EvalInfoFuncRet = "昼";
                                    }

                                    break;
                                }

                            case "高さ":
                                {
                                    EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(Map.MapHeight);
                                    break;
                                }

                            default:
                                {
                                    if (GeneralLib.IsNumber(ref @params[idx]))
                                    {
                                        mx = Conversions.ToShort(@params[idx]);
                                    }

                                    idx = (short)(idx + 1);
                                    if (GeneralLib.IsNumber(ref @params[idx]))
                                    {
                                        my_Renamed = Conversions.ToShort(@params[idx]);
                                    }

                                    if (mx < 1 | Map.MapWidth < mx | my_Renamed < 1 | Map.MapHeight < my_Renamed)
                                    {
                                        return EvalInfoFuncRet;
                                    }

                                    idx = (short)(idx + 1);
                                    switch (@params[idx] ?? "")
                                    {
                                        case "地形名":
                                            {
                                                EvalInfoFuncRet = Map.TerrainName(mx, my_Renamed);
                                                break;
                                            }

                                        case "地形タイプ":
                                        case "地形クラス":
                                            {
                                                EvalInfoFuncRet = Map.TerrainClass(mx, my_Renamed);
                                                break;
                                            }

                                        case "移動コスト":
                                            {
                                                // 0.5刻みの移動コストを使えるようにするため、移動コストは
                                                // 実際の２倍の値で記録されている
                                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(Map.TerrainMoveCost(mx, my_Renamed) / 2d);
                                                break;
                                            }

                                        case "回避修正":
                                            {
                                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(Map.TerrainEffectForHit(mx, my_Renamed));
                                                break;
                                            }

                                        case "ダメージ修正":
                                            {
                                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(Map.TerrainEffectForDamage(mx, my_Renamed));
                                                break;
                                            }

                                        case "ＨＰ回復量":
                                            {
                                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(Map.TerrainEffectForHPRecover(mx, my_Renamed));
                                                break;
                                            }

                                        case "ＥＮ回復量":
                                            {
                                                EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(Map.TerrainEffectForENRecover(mx, my_Renamed));
                                                break;
                                            }

                                        case "ビットマップ名":
                                            {
                                                // MOD START 240a
                                                // Select Case MapImageFileTypeData(mx, my)
                                                // Case SeparateDirMapImageFileType
                                                // EvalInfoFunc = _
                                                // '                                        TDList.Bitmap(MapData(mx, my, 0)) & "\" & _
                                                // '                                        TDList.Bitmap(MapData(mx, my, 0)) & _
                                                // '                                        Format$(MapData(mx, my, 1), "0000") & ".bmp"
                                                // Case FourFiguresMapImageFileType
                                                // EvalInfoFunc = _
                                                // '                                        TDList.Bitmap(MapData(mx, my, 0)) & _
                                                // '                                        Format$(MapData(mx, my, 1), "0000") & ".bmp"
                                                // Case OldMapImageFileType
                                                // EvalInfoFunc = _
                                                // '                                        TDList.Bitmap(MapData(mx, my, 0)) & _
                                                // '                                        Format$(MapData(mx, my, 1)) & ".bmp"
                                                // End Select
                                                switch (Map.MapImageFileTypeData[mx, my_Renamed])
                                                {
                                                    case Map.MapImageFileType.SeparateDirMapImageFileType:
                                                        {
                                                            EvalInfoFuncRet = SRC.TDList.Bitmap(Map.MapData[mx, my_Renamed, Map.MapDataIndex.TerrainType]) + @"\" + SRC.TDList.Bitmap(Map.MapData[mx, my_Renamed, Map.MapDataIndex.TerrainType]) + VB.Compatibility.VB6.Support.Format(Map.MapData[mx, my_Renamed, Map.MapDataIndex.BitmapNo], "0000") + ".bmp";
                                                            break;
                                                        }

                                                    case Map.MapImageFileType.FourFiguresMapImageFileType:
                                                        {
                                                            EvalInfoFuncRet = SRC.TDList.Bitmap(Map.MapData[mx, my_Renamed, Map.MapDataIndex.TerrainType]) + VB.Compatibility.VB6.Support.Format(Map.MapData[mx, my_Renamed, Map.MapDataIndex.BitmapNo], "0000") + ".bmp";
                                                            break;
                                                        }

                                                    case Map.MapImageFileType.OldMapImageFileType:
                                                        {
                                                            EvalInfoFuncRet = SRC.TDList.Bitmap(Map.MapData[mx, my_Renamed, Map.MapDataIndex.TerrainType]) + VB.Compatibility.VB6.Support.Format(Map.MapData[mx, my_Renamed, Map.MapDataIndex.BitmapNo]) + ".bmp";
                                                            break;
                                                        }
                                                }

                                                break;
                                            }
                                        // MOD  END  240a
                                        // ADD START 240a
                                        case "レイヤービットマップ名":
                                            {
                                                switch (Map.MapImageFileTypeData[mx, my_Renamed])
                                                {
                                                    case Map.MapImageFileType.SeparateDirMapImageFileType:
                                                        {
                                                            EvalInfoFuncRet = SRC.TDList.Bitmap(Map.MapData[mx, my_Renamed, Map.MapDataIndex.LayerType]) + @"\" + SRC.TDList.Bitmap(Map.MapData[mx, my_Renamed, Map.MapDataIndex.LayerType]) + VB.Compatibility.VB6.Support.Format(Map.MapData[mx, my_Renamed, Map.MapDataIndex.LayerBitmapNo], "0000") + ".bmp";
                                                            break;
                                                        }

                                                    case Map.MapImageFileType.FourFiguresMapImageFileType:
                                                        {
                                                            EvalInfoFuncRet = SRC.TDList.Bitmap(Map.MapData[mx, my_Renamed, Map.MapDataIndex.LayerType]) + VB.Compatibility.VB6.Support.Format(Map.MapData[mx, my_Renamed, Map.MapDataIndex.LayerBitmapNo], "0000") + ".bmp";
                                                            break;
                                                        }

                                                    case Map.MapImageFileType.OldMapImageFileType:
                                                        {
                                                            EvalInfoFuncRet = SRC.TDList.Bitmap(Map.MapData[mx, my_Renamed, Map.MapDataIndex.LayerType]) + VB.Compatibility.VB6.Support.Format(Map.MapData[mx, my_Renamed, Map.MapDataIndex.LayerBitmapNo]) + ".bmp";
                                                            break;
                                                        }
                                                }

                                                break;
                                            }
                                        // ADD  END  240a
                                        case "ユニットＩＤ":
                                            {
                                                if (Map.MapDataForUnit[mx, my_Renamed] is object)
                                                {
                                                    EvalInfoFuncRet = Map.MapDataForUnit[mx, my_Renamed].ID;
                                                }

                                                break;
                                            }
                                    }

                                    break;
                                }
                        }

                        break;
                    }

                case "オプション":
                    {
                        idx = (short)(idx + 1);
                        switch (@params[idx] ?? "")
                        {
                            case "MessageWait":
                                {
                                    EvalInfoFuncRet = VB.Compatibility.VB6.Support.Format(GUI.MessageWait);
                                    break;
                                }

                            case "BattleAnimation":
                                {
                                    if (SRC.BattleAnimation)
                                    {
                                        EvalInfoFuncRet = "On";
                                    }
                                    else
                                    {
                                        EvalInfoFuncRet = "Off";
                                    }

                                    break;
                                }
                            // ADD START MARGE
                            case "ExtendedAnimation":
                                {
                                    if (SRC.ExtendedAnimation)
                                    {
                                        EvalInfoFuncRet = "On";
                                    }
                                    else
                                    {
                                        EvalInfoFuncRet = "Off";
                                    }

                                    break;
                                }
                            // ADD END MARGE
                            case "SpecialPowerAnimation":
                                {
                                    if (SRC.SpecialPowerAnimation)
                                    {
                                        EvalInfoFuncRet = "On";
                                    }
                                    else
                                    {
                                        EvalInfoFuncRet = "Off";
                                    }

                                    break;
                                }

                            case "AutoDeffence":
                                {
                                    // UPGRADE_ISSUE: Control mnuMapCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    if (GUI.MainForm.mnuMapCommandItem(Commands.AutoDefenseCmdID).Checked)
                                    {
                                        EvalInfoFuncRet = "On";
                                    }
                                    else
                                    {
                                        EvalInfoFuncRet = "Off";
                                    }

                                    break;
                                }

                            case "UseDirectMusic":
                                {
                                    if (Sound.UseDirectMusic)
                                    {
                                        EvalInfoFuncRet = "On";
                                    }
                                    else
                                    {
                                        EvalInfoFuncRet = "Off";
                                    }

                                    break;
                                }
                            // MOD START MARGE
                            // Case "Turn", "Square", "KeepEnemyBGM", "MidiReset", _
                            // '                    "AutoMoveCursor", "DebugMode", "LastFolder", _
                            // '                    "MIDIPortID", "MP3Volume", _
                            // '                    "BattleAnimation", "WeaponAnimation", "MoveAnimation", _
                            // '                    "ImageBufferNum", "MaxImageBufferSize", "KeepStretchedImage", _
                            // '                    "UseTransparentBlt"
                            // 「NewGUI」で探しに来たらINIの状態を返す。「新ＧＵＩ」で探しに来たらOptionの状態を返す。
                            case "Turn":
                            case "Square":
                            case "KeepEnemyBGM":
                            case "MidiReset":
                            case "AutoMoveCursor":
                            case "DebugMode":
                            case "LastFolder":
                            case "MIDIPortID":
                            case "MP3Volume":
                            case var case13 when case13 == "BattleAnimation":
                            case "WeaponAnimation":
                            case "MoveAnimation":
                            case "ImageBufferNum":
                            case "MaxImageBufferSize":
                            case "KeepStretchedImage":
                            case "UseTransparentBlt":
                            case "NewGUI":
                                {
                                    // MOD END MARGE
                                    string argini_section = "Option";
                                    EvalInfoFuncRet = GeneralLib.ReadIni(ref argini_section, ref @params[idx]);
                                    break;
                                }

                            default:
                                {
                                    // Optionコマンドのオプションを参照
                                    if (IsOptionDefined(ref @params[idx]))
                                    {
                                        EvalInfoFuncRet = "On";
                                    }
                                    else
                                    {
                                        EvalInfoFuncRet = "Off";
                                    }

                                    break;
                                }
                        }

                        break;
                    }
            }

            return EvalInfoFuncRet;
        }


        // === 変数に関する処理 ===

        // 変数の値を評価
        public static ValueType GetVariable(ref string var_name, ref ValueType etype, ref string str_result, ref double num_result)
        {
            ValueType GetVariableRet = default;
            string vname;
            short i, num;
            Unit u;
            int ret;
            string ipara, idx, buf = default;
            short start_idx, depth;
            bool in_single_quote = default, in_double_quote = default;
            bool is_term;
            vname = var_name;

            // 未定義値の設定
            str_result = var_name;

            // 変数が配列？
            ret = Strings.InStr(vname, "[");
            if (ret == 0)
            {
                goto SkipArrayHandling;
            }

            if (Strings.Right(vname, 1) != "]")
            {
                goto SkipArrayHandling;
            }

            // ここから配列専用の処理

            // インデックス部分の切りだし
            idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);

            // 多次元配列の処理
            if (Strings.InStr(idx, ",") > 0)
            {
                start_idx = 1;
                depth = 0;
                is_term = true;
                var loopTo = (short)Strings.Len(idx);
                for (i = 1; i <= loopTo; i++)
                {
                    if (in_single_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 96) // `
                        {
                            in_single_quote = false;
                        }
                    }
                    else if (in_double_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 34) // "
                        {
                            in_double_quote = false;
                        }
                    }
                    else
                    {
                        switch (Strings.Asc(Strings.Mid(idx, i, 1)))
                        {
                            case 9:
                            case 32: // タブ, 空白
                                {
                                    if (start_idx == i)
                                    {
                                        start_idx = (short)(i + 1);
                                    }
                                    else
                                    {
                                        is_term = false;
                                    }

                                    break;
                                }

                            case 40:
                            case 91: // (, [
                                {
                                    depth = (short)(depth + 1);
                                    break;
                                }

                            case 41:
                            case 93: // ), ]
                                {
                                    depth = (short)(depth - 1);
                                    break;
                                }

                            case 44: // ,
                                {
                                    if (depth == 0)
                                    {
                                        if (Strings.Len(buf) > 0)
                                        {
                                            buf = buf + ",";
                                        }

                                        ipara = Strings.Trim(Strings.Mid(idx, start_idx, i - start_idx));
                                        buf = buf + GetValueAsString(ref ipara, is_term);
                                        start_idx = (short)(i + 1);
                                        is_term = true;
                                    }

                                    break;
                                }

                            case 96: // `
                                {
                                    in_single_quote = true;
                                    break;
                                }

                            case 34: // "
                                {
                                    in_double_quote = true;
                                    break;
                                }
                        }
                    }
                }

                ipara = Strings.Trim(Strings.Mid(idx, start_idx, i - start_idx));
                if (Strings.Len(buf) > 0)
                {
                    idx = buf + "," + GetValueAsString(ref ipara, is_term);
                }
                else
                {
                    idx = GetValueAsString(ref ipara, is_term);
                }
            }
            else
            {
                idx = GetValueAsString(ref idx);
            }

            // 変数名を配列のインデックス部を計算して再構築
            vname = Strings.Left(vname, ret) + idx + "]";

            // 定義されていない要素を使って配列を読み出した場合は空文字列を返す
            str_result = "";

            // 配列専用の処理が終了

            SkipArrayHandling:
            ;


            // ここから配列と通常変数の共通処理

            // サブルーチンローカル変数
            if (Event_Renamed.CallDepth > 0)
            {
                var loopTo1 = Event_Renamed.VarIndex;
                for (i = (short)(Event_Renamed.VarIndexStack[Event_Renamed.CallDepth - 1] + 1); i <= loopTo1; i++)
                {
                    {
                        var withBlock = Event_Renamed.VarStack[i];
                        if ((vname ?? "") == (withBlock.Name ?? ""))
                        {
                            switch (etype)
                            {
                                case ValueType.NumericType:
                                    {
                                        if (withBlock.VariableType == ValueType.NumericType)
                                        {
                                            num_result = withBlock.NumericValue;
                                        }
                                        else
                                        {
                                            num_result = GeneralLib.StrToDbl(ref withBlock.StringValue);
                                        }

                                        GetVariableRet = ValueType.NumericType;
                                        break;
                                    }

                                case ValueType.StringType:
                                    {
                                        if (withBlock.VariableType == ValueType.StringType)
                                        {
                                            str_result = withBlock.StringValue;
                                        }
                                        else
                                        {
                                            str_result = GeneralLib.FormatNum(withBlock.NumericValue);
                                        }

                                        GetVariableRet = ValueType.StringType;
                                        break;
                                    }

                                case ValueType.UndefinedType:
                                    {
                                        if (withBlock.VariableType == ValueType.StringType)
                                        {
                                            str_result = withBlock.StringValue;
                                            GetVariableRet = ValueType.StringType;
                                        }
                                        else
                                        {
                                            num_result = withBlock.NumericValue;
                                            GetVariableRet = ValueType.NumericType;
                                        }

                                        break;
                                    }
                            }

                            return GetVariableRet;
                        }
                    }
                }
            }

            // ローカル変数
            if (IsLocalVariableDefined(ref vname))
            {
                {
                    var withBlock1 = Event_Renamed.LocalVariableList[vname];
                    switch (etype)
                    {
                        case ValueType.NumericType:
                            {
                                // UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(withBlock1.VariableType, ValueType.NumericType, false)))
                                {
                                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    num_result = Conversions.ToDouble(withBlock1.NumericValue);
                                }
                                else
                                {
                                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    string argexpr = Conversions.ToString(withBlock1.StringValue);
                                    num_result = GeneralLib.StrToDbl(ref argexpr);
                                }

                                GetVariableRet = ValueType.NumericType;
                                break;
                            }

                        case ValueType.StringType:
                            {
                                // UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(withBlock1.VariableType, ValueType.StringType, false)))
                                {
                                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    str_result = Conversions.ToString(withBlock1.StringValue);
                                }
                                else
                                {
                                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    str_result = GeneralLib.FormatNum(Conversions.ToDouble(withBlock1.NumericValue));
                                }

                                GetVariableRet = ValueType.StringType;
                                break;
                            }

                        case ValueType.UndefinedType:
                            {
                                // UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(withBlock1.VariableType, ValueType.StringType, false)))
                                {
                                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    str_result = Conversions.ToString(withBlock1.StringValue);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    num_result = Conversions.ToDouble(withBlock1.NumericValue);
                                    GetVariableRet = ValueType.NumericType;
                                }

                                break;
                            }
                    }
                }

                return GetVariableRet;
            }

            // グローバル変数
            if (IsGlobalVariableDefined(ref vname))
            {
                {
                    var withBlock2 = Event_Renamed.GlobalVariableList[vname];
                    switch (etype)
                    {
                        case ValueType.NumericType:
                            {
                                // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item(vname).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(withBlock2.VariableType, ValueType.NumericType, false)))
                                {
                                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    num_result = Conversions.ToDouble(withBlock2.NumericValue);
                                }
                                else
                                {
                                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    string argexpr1 = Conversions.ToString(withBlock2.StringValue);
                                    num_result = GeneralLib.StrToDbl(ref argexpr1);
                                }

                                GetVariableRet = ValueType.NumericType;
                                break;
                            }

                        case ValueType.StringType:
                            {
                                // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item(vname).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(withBlock2.VariableType, ValueType.StringType, false)))
                                {
                                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    str_result = Conversions.ToString(withBlock2.StringValue);
                                }
                                else
                                {
                                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    str_result = GeneralLib.FormatNum(Conversions.ToDouble(withBlock2.NumericValue));
                                }

                                GetVariableRet = ValueType.StringType;
                                break;
                            }

                        case ValueType.UndefinedType:
                            {
                                // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item(vname).VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(withBlock2.VariableType, ValueType.StringType, false)))
                                {
                                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    str_result = Conversions.ToString(withBlock2.StringValue);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    num_result = Conversions.ToDouble(withBlock2.NumericValue);
                                    GetVariableRet = ValueType.NumericType;
                                }

                                break;
                            }
                    }
                }

                return GetVariableRet;
            }

            // システム変数？
            switch (vname ?? "")
            {
                case "対象ユニット":
                case "対象パイロット":
                    {
                        if (Event_Renamed.SelectedUnitForEvent is object)
                        {
                            {
                                var withBlock3 = Event_Renamed.SelectedUnitForEvent;
                                if (withBlock3.CountPilot() > 0)
                                {
                                    str_result = withBlock3.MainPilot().ID;
                                }
                                else
                                {
                                    str_result = "";
                                }
                            }
                        }
                        else
                        {
                            str_result = "";
                        }

                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "相手ユニット":
                case "相手パイロット":
                    {
                        if (Event_Renamed.SelectedTargetForEvent is object)
                        {
                            {
                                var withBlock4 = Event_Renamed.SelectedTargetForEvent;
                                if (withBlock4.CountPilot() > 0)
                                {
                                    str_result = withBlock4.MainPilot().ID;
                                }
                                else
                                {
                                    str_result = "";
                                }
                            }
                        }
                        else
                        {
                            str_result = "";
                        }

                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "対象ユニットＩＤ":
                    {
                        if (Event_Renamed.SelectedUnitForEvent is object)
                        {
                            str_result = Event_Renamed.SelectedUnitForEvent.ID;
                        }
                        else
                        {
                            str_result = "";
                        }

                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "相手ユニットＩＤ":
                    {
                        if (Event_Renamed.SelectedTargetForEvent is object)
                        {
                            str_result = Event_Renamed.SelectedTargetForEvent.ID;
                        }
                        else
                        {
                            str_result = "";
                        }

                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "対象ユニット使用武器":
                    {
                        str_result = "";
                        if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedUnit))
                        {
                            {
                                var withBlock5 = Event_Renamed.SelectedUnitForEvent;
                                if (Commands.SelectedWeapon > 0)
                                {
                                    str_result = Commands.SelectedWeaponName;
                                }
                                else
                                {
                                    str_result = "";
                                }
                            }
                        }
                        else if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedTarget))
                        {
                            {
                                var withBlock6 = Event_Renamed.SelectedUnitForEvent;
                                if (Commands.SelectedTWeapon > 0)
                                {
                                    str_result = Commands.SelectedTWeaponName;
                                }
                                else
                                {
                                    str_result = Commands.SelectedDefenseOption;
                                }
                            }
                        }

                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "相手ユニット使用武器":
                    {
                        str_result = "";
                        if (ReferenceEquals(Event_Renamed.SelectedTargetForEvent, Commands.SelectedTarget))
                        {
                            {
                                var withBlock7 = Event_Renamed.SelectedTargetForEvent;
                                if (Commands.SelectedTWeapon > 0)
                                {
                                    str_result = Commands.SelectedTWeaponName;
                                }
                                else
                                {
                                    str_result = Commands.SelectedDefenseOption;
                                }
                            }
                        }
                        else if (ReferenceEquals(Event_Renamed.SelectedTargetForEvent, Commands.SelectedUnit))
                        {
                            {
                                var withBlock8 = Event_Renamed.SelectedTargetForEvent;
                                if (Commands.SelectedWeapon > 0)
                                {
                                    str_result = Commands.SelectedWeaponName;
                                }
                                else
                                {
                                    str_result = "";
                                }
                            }
                        }

                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "対象ユニット使用武器番号":
                    {
                        str_result = "";
                        if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedUnit))
                        {
                            {
                                var withBlock9 = Event_Renamed.SelectedUnitForEvent;
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(Commands.SelectedWeapon);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = Commands.SelectedWeapon;
                                    GetVariableRet = ValueType.NumericType;
                                }
                            }
                        }
                        else if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedTarget))
                        {
                            {
                                var withBlock10 = Event_Renamed.SelectedUnitForEvent;
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(Commands.SelectedTWeapon);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = Commands.SelectedTWeapon;
                                    GetVariableRet = ValueType.NumericType;
                                }
                            }
                        }

                        return GetVariableRet;
                    }

                case "相手ユニット使用武器番号":
                    {
                        str_result = "";
                        if (ReferenceEquals(Event_Renamed.SelectedTargetForEvent, Commands.SelectedTarget))
                        {
                            {
                                var withBlock11 = Event_Renamed.SelectedTargetForEvent;
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(Commands.SelectedTWeapon);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = Commands.SelectedTWeapon;
                                    GetVariableRet = ValueType.NumericType;
                                }
                            }
                        }
                        else if (ReferenceEquals(Event_Renamed.SelectedTargetForEvent, Commands.SelectedUnit))
                        {
                            {
                                var withBlock12 = Event_Renamed.SelectedTargetForEvent;
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(Commands.SelectedWeapon);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = Commands.SelectedWeapon;
                                    GetVariableRet = ValueType.NumericType;
                                }
                            }
                        }

                        return GetVariableRet;
                    }

                case "対象ユニット使用アビリティ":
                    {
                        str_result = "";
                        if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedUnit))
                        {
                            {
                                var withBlock13 = Event_Renamed.SelectedUnitForEvent;
                                if (Commands.SelectedAbility > 0)
                                {
                                    str_result = Commands.SelectedAbilityName;
                                }
                                else
                                {
                                    str_result = "";
                                }
                            }
                        }

                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "対象ユニット使用アビリティ番号":
                    {
                        str_result = "";
                        if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedUnit))
                        {
                            {
                                var withBlock14 = Event_Renamed.SelectedUnitForEvent;
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(Commands.SelectedAbility);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = Commands.SelectedAbility;
                                    GetVariableRet = ValueType.NumericType;
                                }
                            }
                        }

                        return GetVariableRet;
                    }

                case "対象ユニット使用スペシャルパワー":
                    {
                        str_result = "";
                        if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedUnit))
                        {
                            str_result = Commands.SelectedSpecialPower;
                        }

                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "サポートアタックユニットＩＤ":
                    {
                        if (Commands.SupportAttackUnit is object)
                        {
                            str_result = Commands.SupportAttackUnit.ID;
                        }
                        else
                        {
                            str_result = "";
                        }

                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "サポートガードユニットＩＤ":
                    {
                        if (Commands.SupportGuardUnit is object)
                        {
                            str_result = Commands.SupportGuardUnit.ID;
                        }
                        else
                        {
                            str_result = "";
                        }

                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "選択":
                    {
                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(ref Event_Renamed.SelectedAlternative);
                            GetVariableRet = ValueType.NumericType;
                        }
                        else
                        {
                            str_result = Event_Renamed.SelectedAlternative;
                            GetVariableRet = ValueType.StringType;
                        }

                        return GetVariableRet;
                    }

                case "ターン数":
                    {
                        if (etype == ValueType.StringType)
                        {
                            str_result = VB.Compatibility.VB6.Support.Format(SRC.Turn);
                            GetVariableRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = SRC.Turn;
                            GetVariableRet = ValueType.NumericType;
                        }

                        return GetVariableRet;
                    }

                case "総ターン数":
                    {
                        if (etype == ValueType.StringType)
                        {
                            str_result = VB.Compatibility.VB6.Support.Format(SRC.TotalTurn);
                            GetVariableRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = SRC.TotalTurn;
                            GetVariableRet = ValueType.NumericType;
                        }

                        return GetVariableRet;
                    }

                case "フェイズ":
                    {
                        str_result = SRC.Stage;
                        GetVariableRet = ValueType.StringType;
                        return GetVariableRet;
                    }

                case "味方数":
                    {
                        num = 0;
                        foreach (Unit currentU in SRC.UList)
                        {
                            u = currentU;
                            if (u.Party0 == "味方" & (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納"))
                            {
                                num = (short)(num + 1);
                            }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = VB.Compatibility.VB6.Support.Format(num);
                            GetVariableRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = num;
                            GetVariableRet = ValueType.NumericType;
                        }

                        return GetVariableRet;
                    }

                case "ＮＰＣ数":
                    {
                        num = 0;
                        foreach (Unit currentU1 in SRC.UList)
                        {
                            u = currentU1;
                            if (u.Party0 == "ＮＰＣ" & (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納"))
                            {
                                num = (short)(num + 1);
                            }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = VB.Compatibility.VB6.Support.Format(num);
                            GetVariableRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = num;
                            GetVariableRet = ValueType.NumericType;
                        }

                        return GetVariableRet;
                    }

                case "敵数":
                    {
                        num = 0;
                        foreach (Unit currentU2 in SRC.UList)
                        {
                            u = currentU2;
                            if (u.Party0 == "敵" & (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納"))
                            {
                                num = (short)(num + 1);
                            }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = VB.Compatibility.VB6.Support.Format(num);
                            GetVariableRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = num;
                            GetVariableRet = ValueType.NumericType;
                        }

                        return GetVariableRet;
                    }

                case "中立数":
                    {
                        num = 0;
                        foreach (Unit currentU3 in SRC.UList)
                        {
                            u = currentU3;
                            if (u.Party0 == "中立" & (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納"))
                            {
                                num = (short)(num + 1);
                            }
                        }

                        if (etype == ValueType.StringType)
                        {
                            str_result = VB.Compatibility.VB6.Support.Format(num);
                            GetVariableRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = num;
                            GetVariableRet = ValueType.NumericType;
                        }

                        return GetVariableRet;
                    }

                case "資金":
                    {
                        if (etype == ValueType.StringType)
                        {
                            str_result = GeneralLib.FormatNum(SRC.Money);
                            GetVariableRet = ValueType.StringType;
                        }
                        else
                        {
                            num_result = SRC.Money;
                            GetVariableRet = ValueType.NumericType;
                        }

                        return GetVariableRet;
                    }

                default:
                    {
                        // アルファベットの変数名はlow caseで判別
                        switch (Strings.LCase(vname) ?? "")
                        {
                            case "apppath":
                                {
                                    str_result = SRC.AppPath;
                                    GetVariableRet = ValueType.StringType;
                                    return GetVariableRet;
                                }

                            case "appversion":
                                {
                                    // UPGRADE_ISSUE: App オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
                                    {
                                        var withBlock15 = App;
                                        num = (short)(10000 * My.MyProject.Application.Info.Version.Major + 100 * My.MyProject.Application.Info.Version.Minor + My.MyProject.Application.Info.Version.Revision);
                                    }

                                    if (etype == ValueType.StringType)
                                    {
                                        str_result = VB.Compatibility.VB6.Support.Format(num);
                                        GetVariableRet = ValueType.StringType;
                                    }
                                    else
                                    {
                                        num_result = num;
                                        GetVariableRet = ValueType.NumericType;
                                    }

                                    return GetVariableRet;
                                }

                            case "argnum":
                                {
                                    // UpVarの呼び出し回数を累計
                                    num = Event_Renamed.UpVarLevel;
                                    i = Event_Renamed.CallDepth;
                                    while (num > 0)
                                    {
                                        i = (short)(i - num);
                                        if (i < 1)
                                        {
                                            i = 1;
                                            break;
                                        }

                                        num = Event_Renamed.UpVarLevelStack[i];
                                    }

                                    num = (short)(Event_Renamed.ArgIndex - Event_Renamed.ArgIndexStack[i - 1]);
                                    if (etype == ValueType.StringType)
                                    {
                                        str_result = VB.Compatibility.VB6.Support.Format(num);
                                        GetVariableRet = ValueType.StringType;
                                    }
                                    else
                                    {
                                        num_result = num;
                                        GetVariableRet = ValueType.NumericType;
                                    }

                                    return GetVariableRet;
                                }

                            case "basex":
                                {
                                    if (etype == ValueType.StringType)
                                    {
                                        str_result = VB.Compatibility.VB6.Support.Format(Event_Renamed.BaseX);
                                        GetVariableRet = ValueType.StringType;
                                    }
                                    else
                                    {
                                        num_result = Event_Renamed.BaseX;
                                        GetVariableRet = ValueType.NumericType;
                                    }

                                    return GetVariableRet;
                                }

                            case "basey":
                                {
                                    if (etype == ValueType.StringType)
                                    {
                                        str_result = VB.Compatibility.VB6.Support.Format(Event_Renamed.BaseY);
                                        GetVariableRet = ValueType.StringType;
                                    }
                                    else
                                    {
                                        num_result = Event_Renamed.BaseY;
                                        GetVariableRet = ValueType.NumericType;
                                    }

                                    return GetVariableRet;
                                }

                            case "extdatapath":
                                {
                                    str_result = SRC.ExtDataPath;
                                    GetVariableRet = ValueType.StringType;
                                    return GetVariableRet;
                                }

                            case "extdatapath2":
                                {
                                    str_result = SRC.ExtDataPath2;
                                    GetVariableRet = ValueType.StringType;
                                    return GetVariableRet;
                                }

                            case "mousex":
                                {
                                    if (etype == ValueType.StringType)
                                    {
                                        str_result = VB.Compatibility.VB6.Support.Format(GUI.MouseX);
                                        GetVariableRet = ValueType.StringType;
                                    }
                                    else
                                    {
                                        num_result = GUI.MouseX;
                                        GetVariableRet = ValueType.NumericType;
                                    }

                                    return GetVariableRet;
                                }

                            case "mousey":
                                {
                                    if (etype == ValueType.StringType)
                                    {
                                        str_result = VB.Compatibility.VB6.Support.Format(GUI.MouseY);
                                        GetVariableRet = ValueType.StringType;
                                    }
                                    else
                                    {
                                        num_result = GUI.MouseY;
                                        GetVariableRet = ValueType.NumericType;
                                    }

                                    return GetVariableRet;
                                }

                            case "now":
                                {
                                    str_result = Conversions.ToString(DateAndTime.Now);
                                    GetVariableRet = ValueType.StringType;
                                    return GetVariableRet;
                                }

                            case "scenariopath":
                                {
                                    str_result = SRC.ScenarioPath;
                                    GetVariableRet = ValueType.StringType;
                                    return GetVariableRet;
                                }
                        }

                        break;
                    }
            }

            // コンフィグ変数？
            if (BCVariable.IsConfig)
            {
                switch (vname ?? "")
                {
                    case "攻撃値":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = VB.Compatibility.VB6.Support.Format(BCVariable.AttackExp);
                                GetVariableRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.AttackExp;
                                GetVariableRet = ValueType.NumericType;
                            }

                            return GetVariableRet;
                        }

                    case "攻撃側ユニットＩＤ":
                        {
                            str_result = BCVariable.AtkUnit.ID;
                            GetVariableRet = ValueType.StringType;
                            return GetVariableRet;
                        }

                    case "防御側ユニットＩＤ":
                        {
                            if (BCVariable.DefUnit is object)
                            {
                                str_result = BCVariable.DefUnit.ID;
                                GetVariableRet = ValueType.StringType;
                                return GetVariableRet;
                            }

                            break;
                        }

                    case "武器番号":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = VB.Compatibility.VB6.Support.Format(BCVariable.WeaponNumber);
                                GetVariableRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.WeaponNumber;
                                GetVariableRet = ValueType.NumericType;
                            }

                            return GetVariableRet;
                        }

                    case "地形適応":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = VB.Compatibility.VB6.Support.Format(BCVariable.TerrainAdaption);
                                GetVariableRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.TerrainAdaption;
                                GetVariableRet = ValueType.NumericType;
                            }

                            return GetVariableRet;
                        }

                    case "武器威力":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = VB.Compatibility.VB6.Support.Format(BCVariable.WeaponPower);
                                GetVariableRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.WeaponPower;
                                GetVariableRet = ValueType.NumericType;
                            }

                            return GetVariableRet;
                        }

                    case "サイズ補正":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = VB.Compatibility.VB6.Support.Format(BCVariable.SizeMod);
                                GetVariableRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.SizeMod;
                                GetVariableRet = ValueType.NumericType;
                            }

                            return GetVariableRet;
                        }

                    case "装甲値":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = VB.Compatibility.VB6.Support.Format(BCVariable.Armor);
                                GetVariableRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.Armor;
                                GetVariableRet = ValueType.NumericType;
                            }

                            return GetVariableRet;
                        }

                    case "最終値":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = VB.Compatibility.VB6.Support.Format(BCVariable.LastVariable);
                                GetVariableRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.LastVariable;
                                GetVariableRet = ValueType.NumericType;
                            }

                            return GetVariableRet;
                        }

                    case "攻撃側補正":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = VB.Compatibility.VB6.Support.Format(BCVariable.AttackVariable);
                                GetVariableRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.AttackVariable;
                                GetVariableRet = ValueType.NumericType;
                            }

                            return GetVariableRet;
                        }

                    case "防御側補正":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = VB.Compatibility.VB6.Support.Format(BCVariable.DffenceVariable);
                                GetVariableRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.DffenceVariable;
                                GetVariableRet = ValueType.NumericType;
                            }

                            return GetVariableRet;
                        }

                    case "ザコ補正":
                        {
                            if (etype == ValueType.StringType)
                            {
                                str_result = VB.Compatibility.VB6.Support.Format(BCVariable.CommonEnemy);
                                GetVariableRet = ValueType.StringType;
                            }
                            else
                            {
                                num_result = BCVariable.CommonEnemy;
                                GetVariableRet = ValueType.NumericType;
                            }

                            return GetVariableRet;
                        }
                }

                // パイロットに関する変数
                {
                    var withBlock16 = BCVariable.MeUnit.MainPilot();
                    switch (vname ?? "")
                    {
                        case "気力":
                            {
                                num = 0;
                                string argoname = "気力効果小";
                                if (IsOptionDefined(ref argoname))
                                {
                                    num = (short)(50 + (withBlock16.Morale + withBlock16.MoraleMod) / 2); // 気力の補正込み値を代入
                                }
                                else
                                {
                                    num = (short)(withBlock16.Morale + withBlock16.MoraleMod);
                                } // 気力の補正込み値を代入

                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(num);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = num;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "耐久":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Defense);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Defense;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "ＬＶ":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Level);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Level;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "経験":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Exp);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Exp;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "ＳＰ":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.SP);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.SP;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "霊力":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Plana);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Plana;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "格闘":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Infight);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Infight;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "射撃":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Shooting);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Shooting;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "命中":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Hit);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Hit;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "回避":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Dodge);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Dodge;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "技量":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Technique);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Technique;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "反応":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock16.Intuition);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock16.Intuition;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }
                    }
                }

                // ユニットに関する変数
                {
                    var withBlock17 = BCVariable.MeUnit;
                    switch (vname ?? "")
                    {
                        case "最大ＨＰ":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock17.MaxHP);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.MaxHP;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "現在ＨＰ":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock17.HP);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.HP;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "最大ＥＮ":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock17.MaxEN);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.MaxEN;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "現在ＥＮ":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock17.EN);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.EN;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "移動力":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock17.Speed);
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.Speed;
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "装甲":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock17.get_Armor(""));
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.get_Armor("");
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }

                        case "運動性":
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = VB.Compatibility.VB6.Support.Format(withBlock17.get_Mobility(""));
                                    GetVariableRet = ValueType.StringType;
                                }
                                else
                                {
                                    num_result = withBlock17.get_Mobility("");
                                    GetVariableRet = ValueType.NumericType;
                                }

                                return GetVariableRet;
                            }
                    }
                }
            }

            if (etype == ValueType.NumericType)
            {
                num_result = 0d;
                GetVariableRet = ValueType.NumericType;
            }
            else
            {
                GetVariableRet = ValueType.StringType;
            }

            return GetVariableRet;
        }

        // 指定した変数が定義されているか？
        public static bool IsVariableDefined(ref string var_name)
        {
            bool IsVariableDefinedRet = default;
            string vname;
            short i, ret;
            string ipara, idx, buf = default;
            short start_idx, depth;
            bool in_single_quote = default, in_double_quote = default;
            bool is_term;
            switch (Strings.Asc(var_name))
            {
                case 36: // $
                    {
                        vname = Strings.Mid(var_name, 2);
                        break;
                    }

                default:
                    {
                        vname = var_name;
                        break;
                    }
            }

            // 変数が配列？
            ret = (short)Strings.InStr(vname, "[");
            if (ret == 0)
            {
                goto SkipArrayHandling;
            }

            if (Strings.Right(vname, 1) != "]")
            {
                goto SkipArrayHandling;
            }

            // ここから配列専用の処理

            // インデックス部分の切りだし
            idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);

            // 多次元配列の処理
            if (Strings.InStr(idx, ",") > 0)
            {
                start_idx = 1;
                depth = 0;
                is_term = true;
                var loopTo = (short)Strings.Len(idx);
                for (i = 1; i <= loopTo; i++)
                {
                    if (in_single_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 96) // `
                        {
                            in_single_quote = false;
                        }
                    }
                    else if (in_double_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 34) // "
                        {
                            in_double_quote = false;
                        }
                    }
                    else
                    {
                        switch (Strings.Asc(Strings.Mid(idx, i, 1)))
                        {
                            case 9:
                            case 32: // タブ, 空白
                                {
                                    if (start_idx == i)
                                    {
                                        start_idx = (short)(i + 1);
                                    }
                                    else
                                    {
                                        is_term = false;
                                    }

                                    break;
                                }

                            case 40:
                            case 91: // (, 
                                {
                                    depth = (short)(depth + 1);
                                    break;
                                }

                            case 41:
                            case 93: // ), 
                                {
                                    depth = (short)(depth - 1);
                                    break;
                                }

                            case 44: // ,
                                {
                                    if (depth == 0)
                                    {
                                        if (Strings.Len(buf) > 0)
                                        {
                                            buf = buf + ",";
                                        }

                                        ipara = Strings.Trim(Strings.Mid(idx, start_idx, i - start_idx));
                                        buf = buf + GetValueAsString(ref ipara, is_term);
                                        start_idx = (short)(i + 1);
                                        is_term = true;
                                    }

                                    break;
                                }

                            case 96: // `
                                {
                                    in_single_quote = true;
                                    break;
                                }

                            case 34: // "
                                {
                                    in_double_quote = true;
                                    break;
                                }
                        }
                    }
                }

                ipara = Strings.Trim(Strings.Mid(idx, start_idx, i - start_idx));
                if (Strings.Len(buf) > 0)
                {
                    idx = buf + "," + GetValueAsString(ref ipara, is_term);
                }
                else
                {
                    idx = GetValueAsString(ref ipara, is_term);
                }
            }
            else
            {
                string argexpr = Strings.Trim(idx);
                idx = GetValueAsString(ref argexpr);
            }

            // 変数名を配列のインデックス部を計算して再構築
            vname = Strings.Left(vname, ret) + idx + "]";

            // 配列専用の処理が終了

            SkipArrayHandling:
            ;


            // ここから配列と通常変数の共通処理

            // サブルーチンローカル変数
            if (Event_Renamed.CallDepth > 0)
            {
                var loopTo1 = Event_Renamed.VarIndex;
                for (i = (short)(Event_Renamed.VarIndexStack[Event_Renamed.CallDepth - 1] + 1); i <= loopTo1; i++)
                {
                    if ((vname ?? "") == (Event_Renamed.VarStack[i].Name ?? ""))
                    {
                        IsVariableDefinedRet = true;
                        return IsVariableDefinedRet;
                    }
                }
            }

            // ローカル変数
            if (IsLocalVariableDefined(ref vname))
            {
                IsVariableDefinedRet = true;
                return IsVariableDefinedRet;
            }

            // グローバル変数
            if (IsGlobalVariableDefined(ref vname))
            {
                IsVariableDefinedRet = true;
                return IsVariableDefinedRet;
            }

            return IsVariableDefinedRet;
        }

        // 指定した名前のサブルーチンローカル変数が定義されているか？
        public static bool IsSubLocalVariableDefined(ref string vname)
        {
            bool IsSubLocalVariableDefinedRet = default;
            short i;
            if (Event_Renamed.CallDepth > 0)
            {
                var loopTo = Event_Renamed.VarIndex;
                for (i = (short)(Event_Renamed.VarIndexStack[Event_Renamed.CallDepth - 1] + 1); i <= loopTo; i++)
                {
                    if ((vname ?? "") == (Event_Renamed.VarStack[i].Name ?? ""))
                    {
                        IsSubLocalVariableDefinedRet = true;
                        return IsSubLocalVariableDefinedRet;
                    }
                }
            }

            return IsSubLocalVariableDefinedRet;
        }

        // 指定した名前のローカル変数が定義されているか？
        public static bool IsLocalVariableDefined(ref string vname)
        {
            bool IsLocalVariableDefinedRet = default;
            VarData dummy;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 296278


            Input:

                    On Error GoTo ErrorHandler

             */
            dummy = (VarData)Event_Renamed.LocalVariableList[vname];
            IsLocalVariableDefinedRet = true;
            return IsLocalVariableDefinedRet;
            ErrorHandler:
            ;
            IsLocalVariableDefinedRet = false;
        }

        // 指定した名前のグローバル変数が定義されているか？
        public static bool IsGlobalVariableDefined(ref string vname)
        {
            bool IsGlobalVariableDefinedRet = default;
            VarData dummy;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 296649


            Input:

                    On Error GoTo ErrorHandler

             */
            dummy = (VarData)Event_Renamed.GlobalVariableList[vname];
            IsGlobalVariableDefinedRet = true;
            return IsGlobalVariableDefinedRet;
            ErrorHandler:
            ;
            IsGlobalVariableDefinedRet = false;
        }

        // 変数の値を設定
        public static void SetVariable(ref string var_name, ref ValueType etype, ref string str_value, ref double num_value)
        {
            VarData new_var;
            string vname;
            short i, ret;
            string ipara, idx, buf = default;
            var vname0 = default(string);
            Pilot p;
            Unit u;
            short start_idx, depth;
            bool in_single_quote = default, in_double_quote = default;
            bool is_term;
            var is_subroutine_local_array = default(bool);

            // Debug.Print "Set " & vname & " " & new_value

            vname = var_name;

            // 左辺値を伴う関数
            ret = (short)Strings.InStr(vname, "(");
            if (ret > 1 & Strings.Right(vname, 1) == ")")
            {
                switch (Strings.LCase(Strings.Left(vname, ret - 1)) ?? "")
                {
                    case "hp":
                        {
                            idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);
                            idx = GetValueAsString(ref idx);
                            bool localIsDefined() { object argIndex1 = idx; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                            object argIndex2 = idx;
                            if (SRC.UList.IsDefined2(ref argIndex2))
                            {
                                object argIndex1 = idx;
                                u = SRC.UList.Item2(ref argIndex1);
                            }
                            else if (localIsDefined())
                            {
                                Pilot localItem() { object argIndex1 = idx; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                u = localItem().Unit_Renamed;
                            }
                            else
                            {
                                u = Event_Renamed.SelectedUnitForEvent;
                            }

                            if (u is object)
                            {
                                if (etype == ValueType.NumericType)
                                {
                                    u.HP = (int)num_value;
                                }
                                else
                                {
                                    u.HP = GeneralLib.StrToLng(ref str_value);
                                }

                                if (u.HP <= 0)
                                {
                                    u.HP = 1;
                                }
                            }

                            return;
                        }

                    case "en":
                        {
                            idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);
                            idx = GetValueAsString(ref idx);
                            bool localIsDefined1() { object argIndex1 = idx; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                            object argIndex4 = idx;
                            if (SRC.UList.IsDefined2(ref argIndex4))
                            {
                                object argIndex3 = idx;
                                u = SRC.UList.Item2(ref argIndex3);
                            }
                            else if (localIsDefined1())
                            {
                                Pilot localItem1() { object argIndex1 = idx; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                u = localItem1().Unit_Renamed;
                            }
                            else
                            {
                                u = Event_Renamed.SelectedUnitForEvent;
                            }

                            if (u is object)
                            {
                                if (etype == ValueType.NumericType)
                                {
                                    u.EN = (int)num_value;
                                }
                                else
                                {
                                    u.EN = GeneralLib.StrToLng(ref str_value);
                                }

                                if (u.EN == 0 & u.Status_Renamed == "出撃")
                                {
                                    GUI.PaintUnitBitmap(ref u);
                                }
                            }

                            return;
                        }

                    case "sp":
                        {
                            idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);
                            idx = GetValueAsString(ref idx);
                            bool localIsDefined2() { object argIndex1 = idx; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                            object argIndex6 = idx;
                            if (SRC.UList.IsDefined2(ref argIndex6))
                            {
                                Unit localItem2() { object argIndex1 = idx; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                p = localItem2().MainPilot();
                            }
                            else if (localIsDefined2())
                            {
                                object argIndex5 = idx;
                                p = SRC.PList.Item(ref argIndex5);
                            }
                            else
                            {
                                p = Event_Renamed.SelectedUnitForEvent.MainPilot();
                            }

                            if (p is object)
                            {
                                {
                                    var withBlock = p;
                                    if (withBlock.MaxSP > 0)
                                    {
                                        if (etype == ValueType.NumericType)
                                        {
                                            withBlock.SP = (int)num_value;
                                        }
                                        else
                                        {
                                            withBlock.SP = GeneralLib.StrToLng(ref str_value);
                                        }
                                    }
                                }
                            }

                            return;
                        }

                    case "plana":
                        {
                            idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);
                            idx = GetValueAsString(ref idx);
                            bool localIsDefined3() { object argIndex1 = idx; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                            object argIndex8 = idx;
                            if (SRC.UList.IsDefined2(ref argIndex8))
                            {
                                Unit localItem21() { object argIndex1 = idx; var ret = SRC.UList.Item2(ref argIndex1); return ret; }

                                p = localItem21().MainPilot();
                            }
                            else if (localIsDefined3())
                            {
                                object argIndex7 = idx;
                                p = SRC.PList.Item(ref argIndex7);
                            }
                            else
                            {
                                p = Event_Renamed.SelectedUnitForEvent.MainPilot();
                            }

                            if (p is object)
                            {
                                if (p.MaxPlana() > 0)
                                {
                                    if (etype == ValueType.NumericType)
                                    {
                                        p.Plana = (int)num_value;
                                    }
                                    else
                                    {
                                        p.Plana = GeneralLib.StrToLng(ref str_value);
                                    }
                                }
                            }

                            return;
                        }

                    case "action":
                        {
                            idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);
                            idx = GetValueAsString(ref idx);
                            bool localIsDefined4() { object argIndex1 = idx; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                            object argIndex10 = idx;
                            if (SRC.UList.IsDefined2(ref argIndex10))
                            {
                                object argIndex9 = idx;
                                u = SRC.UList.Item2(ref argIndex9);
                            }
                            else if (localIsDefined4())
                            {
                                Pilot localItem3() { object argIndex1 = idx; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                u = localItem3().Unit_Renamed;
                            }
                            else
                            {
                                u = Event_Renamed.SelectedUnitForEvent;
                            }

                            if (u is object)
                            {
                                if (etype == ValueType.NumericType)
                                {
                                    u.UsedAction = (short)(u.MaxAction() - num_value);
                                }
                                else
                                {
                                    u.UsedAction = (short)(u.MaxAction() - GeneralLib.StrToLng(ref str_value));
                                }
                            }

                            return;
                        }

                    case "eval":
                        {
                            vname = Strings.Trim(Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1));
                            vname = GetValueAsString(ref vname);
                            break;
                        }
                }
            }

            // 変数が配列？
            ret = (short)Strings.InStr(vname, "[");
            if (ret == 0)
            {
                goto SkipArrayHandling;
            }

            if (Strings.Right(vname, 1) != "]")
            {
                goto SkipArrayHandling;
            }

            // ここから配列専用の処理

            // インデックス部分の切りだし
            idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);

            // 多次元配列の処理
            if (Strings.InStr(idx, ",") > 0)
            {
                start_idx = 1;
                depth = 0;
                is_term = true;
                var loopTo = (short)Strings.Len(idx);
                for (i = 1; i <= loopTo; i++)
                {
                    if (in_single_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 96) // `
                        {
                            in_single_quote = false;
                        }
                    }
                    else if (in_double_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 34) // "
                        {
                            in_double_quote = false;
                        }
                    }
                    else
                    {
                        switch (Strings.Asc(Strings.Mid(idx, i, 1)))
                        {
                            case 9:
                            case 32: // タブ, 空白
                                {
                                    if (start_idx == i)
                                    {
                                        start_idx = (short)(i + 1);
                                    }
                                    else
                                    {
                                        is_term = false;
                                    }

                                    break;
                                }

                            case 40:
                            case 91: // (, [
                                {
                                    depth = (short)(depth + 1);
                                    break;
                                }

                            case 41:
                            case 93: // ), ]
                                {
                                    depth = (short)(depth - 1);
                                    break;
                                }

                            case 44: // ,
                                {
                                    if (depth == 0)
                                    {
                                        if (Strings.Len(buf) > 0)
                                        {
                                            buf = buf + ",";
                                        }

                                        ipara = Strings.Trim(Strings.Mid(idx, start_idx, i - start_idx));
                                        buf = buf + GetValueAsString(ref ipara, is_term);
                                        start_idx = (short)(i + 1);
                                        is_term = true;
                                    }

                                    break;
                                }

                            case 96: // `
                                {
                                    in_single_quote = true;
                                    break;
                                }

                            case 34: // "
                                {
                                    in_double_quote = true;
                                    break;
                                }
                        }
                    }
                }

                ipara = Strings.Trim(Strings.Mid(idx, start_idx, i - start_idx));
                if (Strings.Len(buf) > 0)
                {
                    idx = buf + "," + GetValueAsString(ref ipara, is_term);
                }
                else
                {
                    idx = GetValueAsString(ref ipara, is_term);
                }
            }
            else
            {
                string argexpr = Strings.Trim(idx);
                idx = GetValueAsString(ref argexpr);
            }

            // 変数名を配列のインデックス部を計算して再構築
            vname = Strings.Left(vname, ret) + idx + "]";

            // 配列名
            vname0 = Strings.Left(vname, ret - 1);

            // サブルーチンローカルな配列として定義済みかどうかチェック
            if (IsSubLocalVariableDefined(ref vname0))
            {
                is_subroutine_local_array = true;
            }

            // 配列専用の処理が終了

            SkipArrayHandling:
            ;


            // ここから配列と通常変数の共通処理

            // サブルーチンローカル変数として定義済み？
            if (Event_Renamed.CallDepth > 0)
            {
                var loopTo1 = Event_Renamed.VarIndex;
                for (i = (short)(Event_Renamed.VarIndexStack[Event_Renamed.CallDepth - 1] + 1); i <= loopTo1; i++)
                {
                    {
                        var withBlock1 = Event_Renamed.VarStack[i];
                        if ((vname ?? "") == (withBlock1.Name ?? ""))
                        {
                            withBlock1.VariableType = etype;
                            withBlock1.StringValue = str_value;
                            withBlock1.NumericValue = num_value;
                            return;
                        }
                    }
                }
            }

            if (is_subroutine_local_array)
            {
                // サブルーチンローカル変数の配列の要素として定義
                Event_Renamed.VarIndex = (short)(Event_Renamed.VarIndex + 1);
                if (Event_Renamed.VarIndex > Event_Renamed.MaxVarIndex)
                {
                    Event_Renamed.VarIndex = Event_Renamed.MaxVarIndex;
                    Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, "作成したサブルーチンローカル変数の総数が" + VB.Compatibility.VB6.Support.Format(Event_Renamed.MaxVarIndex) + "個を超えています");
                    return;
                }

                {
                    var withBlock2 = Event_Renamed.VarStack[Event_Renamed.VarIndex];
                    withBlock2.Name = vname;
                    withBlock2.VariableType = etype;
                    withBlock2.StringValue = str_value;
                    withBlock2.NumericValue = num_value;
                }

                return;
            }

            // ローカル変数として定義済み？
            if (IsLocalVariableDefined(ref vname))
            {
                {
                    var withBlock3 = Event_Renamed.LocalVariableList[vname];
                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().Name の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock3.Name = vname;
                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock3.VariableType = etype;
                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock3.StringValue = str_value;
                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock3.NumericValue = num_value;
                }

                return;
            }

            // グローバル変数として定義済み？
            if (IsGlobalVariableDefined(ref vname))
            {
                {
                    var withBlock4 = Event_Renamed.GlobalVariableList[vname];
                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().Name の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock4.Name = vname;
                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock4.VariableType = etype;
                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock4.StringValue = str_value;
                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock4.NumericValue = num_value;
                }

                return;
            }

            // システム変数？
            switch (Strings.LCase(vname) ?? "")
            {
                case "basex":
                    {
                        if (etype == ValueType.NumericType)
                        {
                            Event_Renamed.BaseX = (int)num_value;
                        }
                        else
                        {
                            Event_Renamed.BaseX = GeneralLib.StrToLng(ref str_value);
                        }
                        // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                        GUI.MainForm.picMain(0).CurrentX = Event_Renamed.BaseX;
                        return;
                    }

                case "basey":
                    {
                        if (etype == ValueType.NumericType)
                        {
                            Event_Renamed.BaseY = (int)num_value;
                        }
                        else
                        {
                            Event_Renamed.BaseY = GeneralLib.StrToLng(ref str_value);
                        }
                        // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                        GUI.MainForm.picMain(0).CurrentY = Event_Renamed.BaseY;
                        return;
                    }

                case "ターン数":
                    {
                        if (etype == ValueType.NumericType)
                        {
                            SRC.Turn = (short)num_value;
                        }
                        else
                        {
                            SRC.Turn = (short)GeneralLib.StrToLng(ref str_value);
                        }

                        return;
                    }

                case "総ターン数":
                    {
                        if (etype == ValueType.NumericType)
                        {
                            SRC.TotalTurn = (int)num_value;
                        }
                        else
                        {
                            SRC.TotalTurn = GeneralLib.StrToLng(ref str_value);
                        }

                        return;
                    }

                case "資金":
                    {
                        SRC.Money = 0;
                        if (etype == ValueType.NumericType)
                        {
                            SRC.IncrMoney((int)num_value);
                        }
                        else
                        {
                            SRC.IncrMoney(GeneralLib.StrToLng(ref str_value));
                        }

                        return;
                    }
            }

            // 未定義だった場合

            // 配列の要素として作成
            VarData new_var2;
            if (Strings.Len(vname0) != 0)
            {
                // ローカル変数の配列の要素として定義
                if (IsLocalVariableDefined(ref vname0))
                {
                }
                // Nop
                // グローバル変数の配列の要素として定義
                else if (IsGlobalVariableDefined(ref vname0))
                {
                    DefineGlobalVariable(ref vname);
                    {
                        var withBlock5 = Event_Renamed.GlobalVariableList[vname];
                        // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().Name の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        withBlock5.Name = vname;
                        // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        withBlock5.VariableType = etype;
                        // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        withBlock5.StringValue = str_value;
                        // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        withBlock5.NumericValue = num_value;
                    }

                    return;
                }
                // 未定義の配列なのでローカル変数の配列を作成
                else
                {
                    // ローカル変数の配列のメインＩＤを作成
                    new_var2 = new VarData();
                    new_var2.Name = vname0;
                    new_var2.VariableType = ValueType.StringType;
                    if (Strings.InStr(new_var2.Name, "\"") > 0)
                    {
                        Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, "不正な変数「" + new_var2.Name + "」が作成されました");
                    }

                    Event_Renamed.LocalVariableList.Add(new_var2, vname0);
                }
            }

            // ローカル変数として作成
            new_var = new VarData();
            new_var.Name = vname;
            new_var.VariableType = etype;
            new_var.StringValue = str_value;
            new_var.NumericValue = num_value;
            if (Strings.InStr(new_var.Name, "\"") > 0)
            {
                Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, "不正な変数「" + new_var.Name + "」が作成されました");
            }

            Event_Renamed.LocalVariableList.Add(new_var, vname);
        }

        public static void SetVariableAsString(ref string vname, ref string new_value)
        {
            double argnum_value = 0d;
            SetVariable(ref vname, ref ValueType.StringType, ref new_value, ref argnum_value);
        }

        public static void SetVariableAsDouble(ref string vname, double new_value)
        {
            string argstr_value = "";
            SetVariable(ref vname, ref ValueType.NumericType, ref argstr_value, ref new_value);
        }

        public static void SetVariableAsLong(ref string vname, int new_value)
        {
            string argstr_value = "";
            double argnum_value = new_value;
            SetVariable(ref vname, ref ValueType.NumericType, ref argstr_value, ref argnum_value);
        }

        // グローバル変数を定義
        public static void DefineGlobalVariable(ref string vname)
        {
            var new_var = new VarData();
            new_var.Name = vname;
            new_var.VariableType = ValueType.StringType;
            new_var.StringValue = "";
            Event_Renamed.GlobalVariableList.Add(new_var, vname);
        }

        // ローカル変数を定義
        public static void DefineLocalVariable(ref string vname)
        {
            var new_var = new VarData();
            new_var.Name = vname;
            new_var.VariableType = ValueType.StringType;
            new_var.StringValue = "";
            Event_Renamed.LocalVariableList.Add(new_var, vname);
        }

        // 変数を消去
        public static void UndefineVariable(ref string var_name)
        {
            VarData var;
            string vname, vname2;
            short i, ret;
            string idx, buf = default;
            short start_idx, depth;
            bool in_single_quote = default, in_double_quote = default;
            bool is_term;
            if (Strings.Asc(var_name) == 36) // $
            {
                vname = Strings.Mid(var_name, 2);
            }
            else
            {
                vname = var_name;
            }

            // Eval関数
            if (Strings.LCase(Strings.Left(vname, 5)) == "eval(")
            {
                if (Strings.Right(vname, 1) == ")")
                {
                    vname = Strings.Mid(vname, 6, Strings.Len(vname) - 6);
                    vname = GetValueAsString(ref vname);
                }
            }

            // 配列の要素？
            ret = (short)Strings.InStr(vname, "[");
            if (ret == 0)
            {
                goto SkipArrayHandling;
            }

            if (Strings.Right(vname, 1) != "]")
            {
                goto SkipArrayHandling;
            }

            // 配列の要素を指定された場合

            // インデックス部分の切りだし
            idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);

            // 多次元配列の処理
            if (Strings.InStr(idx, ",") > 0)
            {
                start_idx = 1;
                depth = 0;
                is_term = true;
                var loopTo = (short)Strings.Len(idx);
                for (i = 1; i <= loopTo; i++)
                {
                    if (in_single_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 96) // `
                        {
                            in_single_quote = false;
                        }
                    }
                    else if (in_double_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 34) // "
                        {
                            in_double_quote = false;
                        }
                    }
                    else
                    {
                        switch (Strings.Asc(Strings.Mid(idx, i, 1)))
                        {
                            case 9:
                            case 32: // タブ, 空白
                                {
                                    if (start_idx == i)
                                    {
                                        start_idx = (short)(i + 1);
                                    }
                                    else
                                    {
                                        is_term = false;
                                    }

                                    break;
                                }

                            case 40:
                            case 91: // (, [
                                {
                                    depth = (short)(depth + 1);
                                    break;
                                }

                            case 41:
                            case 93: // ), ]
                                {
                                    depth = (short)(depth - 1);
                                    break;
                                }

                            case 44: // ,
                                {
                                    if (depth == 0)
                                    {
                                        if (Strings.Len(buf) > 0)
                                        {
                                            buf = buf + ",";
                                        }

                                        string localGetValueAsString() { string argexpr = Strings.Mid(idx, start_idx, i - start_idx); var ret = GetValueAsString(ref argexpr, is_term); return ret; }

                                        buf = buf + localGetValueAsString();
                                        start_idx = (short)(i + 1);
                                        is_term = true;
                                    }

                                    break;
                                }

                            case 96: // `
                                {
                                    in_single_quote = true;
                                    break;
                                }

                            case 34: // "
                                {
                                    in_double_quote = true;
                                    break;
                                }
                        }
                    }
                }

                if (Strings.Len(buf) > 0)
                {
                    string localGetValueAsString1() { string argexpr = Strings.Mid(idx, start_idx, i - start_idx); var ret = GetValueAsString(ref argexpr, is_term); return ret; }

                    idx = buf + "," + localGetValueAsString1();
                }
                else
                {
                    string argexpr = Strings.Mid(idx, start_idx, i - start_idx);
                    idx = GetValueAsString(ref argexpr, is_term);
                }
            }
            else
            {
                idx = GetValueAsString(ref idx);
            }

            // インデックス部分を評価して変数名を置き換え
            vname = Strings.Left(vname, ret) + idx + "]";

            // サブルーチンローカル変数？
            if (IsSubLocalVariableDefined(ref vname))
            {
                var loopTo1 = Event_Renamed.VarIndex;
                for (i = (short)(Event_Renamed.VarIndexStack[Event_Renamed.CallDepth - 1] + 1); i <= loopTo1; i++)
                {
                    {
                        var withBlock = Event_Renamed.VarStack[i];
                        if ((vname ?? "") == (withBlock.Name ?? ""))
                        {
                            withBlock.Name = "";
                            return;
                        }
                    }
                }
            }

            // ローカル変数？
            if (IsLocalVariableDefined(ref vname))
            {
                Event_Renamed.LocalVariableList.Remove(vname);
                return;
            }

            // グローバル変数？
            if (IsGlobalVariableDefined(ref vname))
            {
                Event_Renamed.GlobalVariableList.Remove(vname);
            }

            // 配列の場合はここで終了
            return;
            SkipArrayHandling:
            ;


            // 通常の変数名を指定された場合

            // 配列要素の判定用
            vname2 = vname + "[";

            // サブルーチンローカル変数？
            if (IsSubLocalVariableDefined(ref vname))
            {
                var loopTo2 = Event_Renamed.VarIndex;
                for (i = (short)(Event_Renamed.VarIndexStack[Event_Renamed.CallDepth - 1] + 1); i <= loopTo2; i++)
                {
                    {
                        var withBlock1 = Event_Renamed.VarStack[i];
                        if ((vname ?? "") == (withBlock1.Name ?? "") | Strings.InStr(withBlock1.Name, vname2) == 1)
                        {
                            withBlock1.Name = "";
                        }
                    }
                }

                return;
            }

            // ローカル変数？
            if (IsLocalVariableDefined(ref vname))
            {
                Event_Renamed.LocalVariableList.Remove(vname);
                foreach (VarData currentVar in Event_Renamed.LocalVariableList)
                {
                    var = currentVar;
                    if (Strings.InStr(var.Name, vname2) == 1)
                    {
                        Event_Renamed.LocalVariableList.Remove(var.Name);
                    }
                }

                return;
            }

            // グローバル変数？
            if (IsGlobalVariableDefined(ref vname))
            {
                Event_Renamed.GlobalVariableList.Remove(vname);
                foreach (VarData currentVar1 in Event_Renamed.GlobalVariableList)
                {
                    var = currentVar1;
                    if (Strings.InStr(var.Name, vname2) == 1)
                    {
                        Event_Renamed.GlobalVariableList.Remove(var.Name);
                    }
                }

                return;
            }
        }



        // === その他の関数 ===

        // 式を文字列として評価
        public static string GetValueAsString(ref string expr, bool is_term = false)
        {
            string GetValueAsStringRet = default;
            var num = default(double);
            if (is_term)
            {
                EvalTerm(ref expr, ref ValueType.StringType, ref GetValueAsStringRet, ref num);
            }
            else
            {
                EvalExpr(ref expr, ref ValueType.StringType, ref GetValueAsStringRet, ref num);
            }

            return GetValueAsStringRet;
        }

        // 式を浮動小数点数として評価
        public static double GetValueAsDouble(ref string expr, bool is_term = false)
        {
            double GetValueAsDoubleRet = default;
            var buf = default(string);
            if (is_term)
            {
                EvalTerm(ref expr, ref ValueType.NumericType, ref buf, ref GetValueAsDoubleRet);
            }
            else
            {
                EvalExpr(ref expr, ref ValueType.NumericType, ref buf, ref GetValueAsDoubleRet);
            }

            return GetValueAsDoubleRet;
        }

        // 式を整数として評価
        public static int GetValueAsLong(ref string expr, bool is_term = false)
        {
            int GetValueAsLongRet = default;
            var buf = default(string);
            var num = default(double);
            if (is_term)
            {
                EvalTerm(ref expr, ref ValueType.NumericType, ref buf, ref num);
            }
            else
            {
                EvalExpr(ref expr, ref ValueType.NumericType, ref buf, ref num);
            }

            GetValueAsLongRet = (int)num;
            return GetValueAsLongRet;
        }


        // strが式かどうかチェック
        // (疑わしきは式と判断している)
        // UPGRADE_NOTE: str は str_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public static bool IsExpr(ref string str_Renamed)
        {
            bool IsExprRet = default;
            switch (Strings.Asc(str_Renamed))
            {
                case 36: // $
                    {
                        IsExprRet = true;
                        break;
                    }

                case 40: // (
                    {
                        IsExprRet = true;
                        break;
                    }
            }

            return IsExprRet;
        }


        // 指定したオプションが設定されているか？
        public static bool IsOptionDefined(ref string oname)
        {
            bool IsOptionDefinedRet = default;
            VarData dummy;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 322090


            Input:

                    On Error GoTo ErrorHandler

             */
            dummy = (VarData)Event_Renamed.GlobalVariableList["Option(" + oname + ")"];
            IsOptionDefinedRet = true;
            return IsOptionDefinedRet;
            ErrorHandler:
            ;
            IsOptionDefinedRet = false;
        }


        // str に対して式置換を行う
        // UPGRADE_NOTE: str は str_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public static void ReplaceSubExpression(ref string str_Renamed)
        {
            short start_idx, end_idx = default;
            short str_len;
            short i, n;
            while (true)
            {
                // 式置換が存在する？
                start_idx = (short)Strings.InStr(str_Renamed, "$(");
                if (start_idx == 0)
                {
                    return;
                }

                // 式置換の終了位置を調べる
                str_len = (short)Strings.Len(str_Renamed);
                n = 1;
                var loopTo = str_len;
                for (i = (short)(start_idx + 2); i <= loopTo; i++)
                {
                    switch (Strings.Mid(str_Renamed, i, 1) ?? "")
                    {
                        case ")":
                            {
                                n = (short)(n - 1);
                                if (n == 0)
                                {
                                    end_idx = i;
                                    break;
                                }

                                break;
                            }

                        case "(":
                            {
                                n = (short)(n + 1);
                                break;
                            }
                    }
                }

                if (i > str_len)
                {
                    return;
                }

                // 式置換を実施
                string localGetValueAsString() { string argexpr = Strings.Mid(str_Renamed, start_idx + 2, end_idx - start_idx - 2); var ret = GetValueAsString(ref argexpr); return ret; }

                str_Renamed = Strings.Left(str_Renamed, start_idx - 1) + localGetValueAsString() + Strings.Right(str_Renamed, str_len - end_idx);
            }
        }

        // msg に対して式置換等の処理を行う
        public static void FormatMessage(ref string msg)
        {
            // ちゃんと横棒がつながって表示されるように罫線文字に置換
            string args22 = "――";
            string args32 = "──";
            string args23 = "ーー";
            string args33 = "──";
            if (GeneralLib.ReplaceString(ref msg, ref args22, ref args32))
            {
                string args2 = "─―";
                string args3 = "──";
                GeneralLib.ReplaceString(ref msg, ref args2, ref args3);
            }
            else if (GeneralLib.ReplaceString(ref msg, ref args23, ref args33))
            {
                string args21 = "─ー";
                string args31 = "──";
                GeneralLib.ReplaceString(ref msg, ref args21, ref args31);
            }

            // 式置換
            ReplaceSubExpression(ref msg);
        }


        // 用語tnameの表示名を参照する
        // tlenが指定された場合は文字列長を強制的にtlenに合わせる
        public static string Term(ref string tname, [Optional, DefaultParameterValue(null)] ref Unit u, short tlen = 0)
        {
            string TermRet = default;
            string vname;
            short i;

            // ユニットが用語名能力を持っている場合はそちらを優先
            if (u is object)
            {
                string argfname = "用語名";
                if (u.IsFeatureAvailable(ref argfname))
                {
                    var loopTo = u.CountFeature();
                    for (i = 1; i <= loopTo; i++)
                    {
                        object argIndex1 = i;
                        if (u.Feature(ref argIndex1) == "用語名")
                        {
                            string localFeatureData1() { object argIndex1 = i; var ret = u.FeatureData(ref argIndex1); return ret; }

                            string arglist1 = localFeatureData1();
                            if ((GeneralLib.LIndex(ref arglist1, 1) ?? "") == (tname ?? ""))
                            {
                                string localFeatureData() { object argIndex1 = i; var ret = u.FeatureData(ref argIndex1); return ret; }

                                string arglist = localFeatureData();
                                TermRet = GeneralLib.LIndex(ref arglist, 2);
                                break;
                            }
                        }
                    }
                }
            }

            // RenameTermで用語名が変更されているかチェック
            if (Strings.Len(TermRet) == 0)
            {
                switch (tname ?? "")
                {
                    case "HP":
                    case "EN":
                    case "SP":
                    case "CT":
                        {
                            vname = "ShortTerm(" + tname + ")";
                            break;
                        }

                    default:
                        {
                            vname = "Term(" + tname + ")";
                            break;
                        }
                }

                if (IsGlobalVariableDefined(ref vname))
                {
                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    TermRet = Conversions.ToString(Event_Renamed.GlobalVariableList[vname].StringValue);
                }
                else
                {
                    TermRet = tname;
                }
            }

            // 表示幅の調整
            if (tlen > 0)
            {
                // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                // UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                if (LenB(Strings.StrConv(TermRet, vbFromUnicode)) < tlen)
                {
                    TermRet = GeneralLib.RightPaddedString(ref TermRet, tlen);
                }
            }

            return TermRet;
        }


        // 引数1で指定した変数のオブジェクトを取得
        public static VarData GetVariableObject(ref string var_name)
        {
            VarData GetVariableObjectRet = default;
            string vname;
            short i, num;
            int ret;
            string ipara, idx, buf = default;
            short start_idx, depth;
            bool in_single_quote = default, in_double_quote = default;
            bool is_term;
            ValueType etype;
            string str_result;
            double num_result;
            vname = var_name;

            // 変数が配列？
            ret = Strings.InStr(vname, "[");
            if (ret == 0)
            {
                goto SkipArrayHandling;
            }

            if (Strings.Right(vname, 1) != "]")
            {
                goto SkipArrayHandling;
            }

            // ここから配列専用の処理

            // インデックス部分の切りだし
            idx = Strings.Mid(vname, ret + 1, Strings.Len(vname) - ret - 1);

            // 多次元配列の処理
            if (Strings.InStr(idx, ",") > 0)
            {
                start_idx = 1;
                depth = 0;
                is_term = true;
                var loopTo = (short)Strings.Len(idx);
                for (i = 1; i <= loopTo; i++)
                {
                    if (in_single_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 96) // `
                        {
                            in_single_quote = false;
                        }
                    }
                    else if (in_double_quote)
                    {
                        if (Strings.Asc(Strings.Mid(idx, i, 1)) == 34) // "
                        {
                            in_double_quote = false;
                        }
                    }
                    else
                    {
                        switch (Strings.Asc(Strings.Mid(idx, i, 1)))
                        {
                            case 9:
                            case 32: // タブ, 空白
                                {
                                    if (start_idx == i)
                                    {
                                        start_idx = (short)(i + 1);
                                    }
                                    else
                                    {
                                        is_term = false;
                                    }

                                    break;
                                }

                            case 40:
                            case 91: // (, [
                                {
                                    depth = (short)(depth + 1);
                                    break;
                                }

                            case 41:
                            case 93: // ), ]
                                {
                                    depth = (short)(depth - 1);
                                    break;
                                }

                            case 44: // ,
                                {
                                    if (depth == 0)
                                    {
                                        if (Strings.Len(buf) > 0)
                                        {
                                            buf = buf + ",";
                                        }

                                        ipara = Strings.Trim(Strings.Mid(idx, start_idx, i - start_idx));
                                        buf = buf + GetValueAsString(ref ipara, is_term);
                                        start_idx = (short)(i + 1);
                                        is_term = true;
                                    }

                                    break;
                                }

                            case 96: // `
                                {
                                    in_single_quote = true;
                                    break;
                                }

                            case 34: // "
                                {
                                    in_double_quote = true;
                                    break;
                                }
                        }
                    }
                }

                ipara = Strings.Trim(Strings.Mid(idx, start_idx, i - start_idx));
                if (Strings.Len(buf) > 0)
                {
                    idx = buf + "," + GetValueAsString(ref ipara, is_term);
                }
                else
                {
                    idx = GetValueAsString(ref ipara, is_term);
                }
            }
            else
            {
                idx = GetValueAsString(ref idx);
            }

            // 変数名を配列のインデックス部を計算して再構築
            vname = Strings.Left(vname, ret) + idx + "]";

            // 配列専用の処理が終了

            SkipArrayHandling:
            ;


            // ここから配列と通常変数の共通処理

            // サブルーチンローカル変数
            if (Event_Renamed.CallDepth > 0)
            {
                var loopTo1 = Event_Renamed.VarIndex;
                for (i = (short)(Event_Renamed.VarIndexStack[Event_Renamed.CallDepth - 1] + 1); i <= loopTo1; i++)
                {
                    if ((vname ?? "") == (Event_Renamed.VarStack[i].Name ?? ""))
                    {
                        GetVariableObjectRet = Event_Renamed.VarStack[i];
                        return GetVariableObjectRet;
                    }
                }
            }

            // ローカル変数
            if (IsLocalVariableDefined(ref vname))
            {
                GetVariableObjectRet = (VarData)Event_Renamed.LocalVariableList[vname];
                return GetVariableObjectRet;
            }

            // グローバル変数
            if (IsGlobalVariableDefined(ref vname))
            {
                GetVariableObjectRet = (VarData)Event_Renamed.GlobalVariableList[vname];
                return GetVariableObjectRet;
            }

            // システム変数？
            etype = ValueType.UndefinedType;
            str_result = "";
            num_result = 0d;
            switch (vname ?? "")
            {
                case "対象ユニット":
                case "対象パイロット":
                    {
                        if (Event_Renamed.SelectedUnitForEvent is object)
                        {
                            {
                                var withBlock = Event_Renamed.SelectedUnitForEvent;
                                if (withBlock.CountPilot() > 0)
                                {
                                    str_result = withBlock.MainPilot().ID;
                                }
                                else
                                {
                                    str_result = "";
                                }
                            }
                        }
                        else
                        {
                            str_result = "";
                        }

                        etype = ValueType.StringType;
                        break;
                    }

                case "相手ユニット":
                case "相手パイロット":
                    {
                        if (Event_Renamed.SelectedTargetForEvent is object)
                        {
                            {
                                var withBlock1 = Event_Renamed.SelectedTargetForEvent;
                                if (withBlock1.CountPilot() > 0)
                                {
                                    str_result = withBlock1.MainPilot().ID;
                                }
                                else
                                {
                                    str_result = "";
                                }
                            }
                        }
                        else
                        {
                            str_result = "";
                        }

                        etype = ValueType.StringType;
                        break;
                    }

                case "対象ユニットＩＤ":
                    {
                        if (Event_Renamed.SelectedUnitForEvent is object)
                        {
                            str_result = Event_Renamed.SelectedUnitForEvent.ID;
                        }
                        else
                        {
                            str_result = "";
                        }

                        etype = ValueType.StringType;
                        break;
                    }

                case "相手ユニットＩＤ":
                    {
                        if (Event_Renamed.SelectedTargetForEvent is object)
                        {
                            str_result = Event_Renamed.SelectedTargetForEvent.ID;
                        }
                        else
                        {
                            str_result = "";
                        }

                        etype = ValueType.StringType;
                        break;
                    }

                case "対象ユニット使用武器":
                    {
                        if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedUnit))
                        {
                            if (Commands.SelectedWeapon > 0)
                            {
                                str_result = Commands.SelectedWeaponName;
                            }
                            else
                            {
                                str_result = "";
                            }
                        }
                        else if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedTarget))
                        {
                            if (Commands.SelectedTWeapon > 0)
                            {
                                str_result = Commands.SelectedTWeaponName;
                            }
                            else
                            {
                                str_result = Commands.SelectedDefenseOption;
                            }
                        }

                        etype = ValueType.StringType;
                        break;
                    }

                case "相手ユニット使用武器":
                    {
                        if (ReferenceEquals(Event_Renamed.SelectedTargetForEvent, Commands.SelectedTarget))
                        {
                            if (Commands.SelectedTWeapon > 0)
                            {
                                str_result = Commands.SelectedTWeaponName;
                            }
                            else
                            {
                                str_result = Commands.SelectedDefenseOption;
                            }
                        }
                        else if (ReferenceEquals(Event_Renamed.SelectedTargetForEvent, Commands.SelectedUnit))
                        {
                            if (Commands.SelectedWeapon > 0)
                            {
                                str_result = Commands.SelectedWeaponName;
                            }
                            else
                            {
                                str_result = "";
                            }
                        }

                        etype = ValueType.StringType;
                        break;
                    }

                case "対象ユニット使用武器番号":
                    {
                        if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedUnit))
                        {
                            num_result = Commands.SelectedWeapon;
                        }
                        else if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedTarget))
                        {
                            num_result = Commands.SelectedTWeapon;
                        }

                        etype = ValueType.NumericType;
                        break;
                    }

                case "相手ユニット使用武器番号":
                    {
                        if (ReferenceEquals(Event_Renamed.SelectedTargetForEvent, Commands.SelectedTarget))
                        {
                            num_result = Commands.SelectedTWeapon;
                        }
                        else if (ReferenceEquals(Event_Renamed.SelectedTargetForEvent, Commands.SelectedUnit))
                        {
                            num_result = Commands.SelectedWeapon;
                        }

                        etype = ValueType.NumericType;
                        break;
                    }

                case "対象ユニット使用アビリティ":
                    {
                        if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedUnit))
                        {
                            if (Commands.SelectedAbility > 0)
                            {
                                str_result = Commands.SelectedAbilityName;
                            }
                            else
                            {
                                str_result = "";
                            }
                        }

                        etype = ValueType.StringType;
                        break;
                    }

                case "対象ユニット使用アビリティ番号":
                    {
                        if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedUnit))
                        {
                            num_result = Commands.SelectedAbility;
                        }

                        etype = ValueType.NumericType;
                        break;
                    }

                case "対象ユニット使用スペシャルパワー":
                    {
                        if (ReferenceEquals(Event_Renamed.SelectedUnitForEvent, Commands.SelectedUnit))
                        {
                            str_result = Commands.SelectedSpecialPower;
                        }

                        etype = ValueType.StringType;
                        break;
                    }

                case "選択":
                    {
                        if (Information.IsNumeric(Event_Renamed.SelectedAlternative))
                        {
                            num_result = GeneralLib.StrToDbl(ref Event_Renamed.SelectedAlternative);
                            etype = ValueType.NumericType;
                        }
                        else
                        {
                            str_result = Event_Renamed.SelectedAlternative;
                            etype = ValueType.StringType;
                        }

                        break;
                    }

                case "ターン数":
                    {
                        num_result = SRC.Turn;
                        etype = ValueType.NumericType;
                        break;
                    }

                case "総ターン数":
                    {
                        num_result = SRC.TotalTurn;
                        etype = ValueType.NumericType;
                        break;
                    }

                case "フェイズ":
                    {
                        str_result = SRC.Stage;
                        etype = ValueType.StringType;
                        break;
                    }

                case "味方数":
                case "ＮＰＣ数":
                case "敵数":
                case "中立数":
                    {
                        num = 0;
                        foreach (Unit u in SRC.UList)
                        {
                            if ((u.Party0 ?? "") == (Strings.Left(vname, Strings.Len(vname) - 1) ?? "") & (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納"))
                            {
                                num = (short)(num + 1);
                            }
                        }

                        num_result = num;
                        etype = ValueType.NumericType;
                        break;
                    }

                case "資金":
                    {
                        num_result = SRC.Money;
                        etype = ValueType.NumericType;
                        break;
                    }

                default:
                    {
                        // アルファベットの変数名はlow caseで判別
                        switch (Strings.LCase(vname) ?? "")
                        {
                            case "apppath":
                                {
                                    str_result = SRC.AppPath;
                                    etype = ValueType.StringType;
                                    break;
                                }

                            case "appversion":
                                {
                                    // UPGRADE_ISSUE: App オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
                                    {
                                        var withBlock2 = App;
                                        num = (short)(10000 * My.MyProject.Application.Info.Version.Major + 100 * My.MyProject.Application.Info.Version.Minor + My.MyProject.Application.Info.Version.Revision);
                                    }

                                    num_result = num;
                                    etype = ValueType.NumericType;
                                    break;
                                }

                            case "argnum":
                                {
                                    num = (short)(Event_Renamed.ArgIndex - Event_Renamed.ArgIndexStack[Event_Renamed.CallDepth - 1 - Event_Renamed.UpVarLevel]);
                                    num_result = num;
                                    etype = ValueType.NumericType;
                                    break;
                                }

                            case "basex":
                                {
                                    num_result = Event_Renamed.BaseX;
                                    etype = ValueType.NumericType;
                                    break;
                                }

                            case "basey":
                                {
                                    num_result = Event_Renamed.BaseY;
                                    etype = ValueType.NumericType;
                                    break;
                                }

                            case "extdatapath":
                                {
                                    str_result = SRC.ExtDataPath;
                                    etype = ValueType.StringType;
                                    break;
                                }

                            case "extdatapath2":
                                {
                                    str_result = SRC.ExtDataPath2;
                                    etype = ValueType.StringType;
                                    break;
                                }

                            case "mousex":
                                {
                                    num_result = GUI.MouseX;
                                    etype = ValueType.NumericType;
                                    break;
                                }

                            case "mousey":
                                {
                                    num_result = GUI.MouseY;
                                    etype = ValueType.NumericType;
                                    break;
                                }

                            case "now":
                                {
                                    str_result = Conversions.ToString(DateAndTime.Now);
                                    etype = ValueType.StringType;
                                    break;
                                }

                            case "scenariopath":
                                {
                                    str_result = SRC.ScenarioPath;
                                    etype = ValueType.StringType;
                                    break;
                                }
                        }

                        break;
                    }
            }

            if (etype != ValueType.UndefinedType)
            {
                GetVariableObjectRet = new VarData();
                GetVariableObjectRet.Name = vname;
                GetVariableObjectRet.VariableType = etype;
                GetVariableObjectRet.StringValue = str_result;
                GetVariableObjectRet.NumericValue = num_result;
            }

            return GetVariableObjectRet;
        }
    }
}