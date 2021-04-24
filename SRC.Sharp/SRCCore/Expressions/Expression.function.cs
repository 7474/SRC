// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Events;
using SRCCore.Expressions.Functions;
using SRCCore.Lib;
using SRCCore.VB;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Expressions
{
    public partial class Expression
    {
        // === 関数に関する処理 ===

        private IDictionary<string, IFunction> systemFunctionMap = new IFunction[] {
            new Args(),
            new Call(),
            new Functions.Unit(),
            new X(),
            new Y(),
            new WX(),
            new WY(),
            // Math
            new Abs(),
            new Atn(),
            new Cos(),
            new Int(),
            new Max(),
            new Min(),
            new Functions.Random(),
            new Round(),
            new RoundUp(),
            new RoundDown(),
            new Sin(),
            new Sqr(),
            new Tan(),
            // String
            //new Format(),
            new InStr(),
            //new InStrRev(),
            new IsNumeric(),
            //new Left(),
            //new Len(),
            //new LSet(),
            //new Mid(),
            //new Replace(),
            //new Right(),
            //new RSet(),
            //new StrComp(),
            //new String(),
            //new Wide(),
            //new InStrB(),
            //new InStrRevB(),
            //new LenB(),
            //new LeftB(),
            //new MidB(),
            //new RightB(),
        }.ToDictionary(x => x.Name.ToLower());

        // 式を関数呼び出しとして構文解析し、実行
        public ValueType CallFunction(string expr, ValueType etype, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            ValueType CallFunctionRet = default;
            string fname;
            int start_idx;
            int num, i, j, num2;
            string buf, buf2;
            double ldbl, rdbl;
            string pname2, pname, uname;
            int ret;
            int cur_depth;
            VarData var;
            //Item it;
            int depth;
            bool in_single_quote, in_double_quote;
            var @params = new string[(Event.MaxArgIndex + 1)];
            int pcount;
            var is_term = new bool[(Event.MaxArgIndex + 1)];
            string dir_path;
            //Static dir_list() As String
            //Static dir_index As int

            // 関数呼び出しの書式に合っているかチェック
            if (Strings.Right(expr, 1) != ")")
            {
                return ValueType.UndefinedType;
            }

            i = Strings.InStr(expr, " ");
            j = Strings.InStr(expr, "(");
            if (i > 0)
            {
                if (i < j)
                {
                    return ValueType.UndefinedType;
                }
            }
            else if (j == 0)
            {
                return ValueType.UndefinedType;
            }

            // ここまでくれば関数呼び出しと断定

            // パラメータの抽出
            pcount = 0;
            start_idx = (j + 1);
            depth = 0;
            in_single_quote = false;
            in_double_quote = false;
            num = Strings.Len(expr);
            int counter;
            counter = start_idx;
            var loopTo = (num - 1);
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
                                    start_idx = (i + 1);
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
                                depth = (depth + 1);
                                break;
                            }

                        case 41:
                        case 93: // ), ]
                            {
                                depth = (depth - 1);
                                break;
                            }

                        case 44: // ,
                            {
                                if (depth == 0)
                                {
                                    pcount = (pcount + 1);
                                    @params[pcount] = Strings.Mid(expr, start_idx, i - start_idx);
                                    start_idx = (i + 1);
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
                pcount = (pcount + 1);
                @params[pcount] = Strings.Mid(expr, start_idx, num - start_idx);
            }

            var isUserFunc = false;
            // 先頭の文字で関数の種類を判断する
            switch (Strings.Asc(expr))
            {
                case 95: // _
                    {
                        // 必ずユーザー定義関数
                        fname = Strings.Left(expr, j - 1);
                        isUserFunc = true;
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
                        if (SRC.UDList.IsDefined(expr))
                        {
                            CallFunctionRet = ValueType.UndefinedType;
                            return CallFunctionRet;
                        }

                        if (SRC.PDList.IsDefined(expr))
                        {
                            CallFunctionRet = ValueType.UndefinedType;
                            return CallFunctionRet;
                        }

                        if (SRC.NPDList.IsDefined(expr))
                        {
                            CallFunctionRet = ValueType.UndefinedType;
                            return CallFunctionRet;
                        }

                        if (SRC.IDList.IsDefined(expr))
                        {
                            CallFunctionRet = ValueType.UndefinedType;
                            return CallFunctionRet;
                        }

                        fname = Strings.Left(expr, j - 1);
                        isUserFunc = true;
                        break;
                    }
            }

            // システム関数？
            if (!isUserFunc)
            {
                IFunction systemFunc;
                if (systemFunctionMap.TryGetValue(fname.ToLower(), out systemFunc))
                {
                    var systemFunctionRet = systemFunc.Invoke(SRC, etype, @params, pcount, is_term, out str_result, out num_result);
                    if (systemFunctionRet != ValueType.UndefinedType)
                    {
                        return systemFunctionRet;
                    }
                }
            }

            // ユーザー定義関数？
            ret = Event.FindNormalLabel(fname);
            if (ret >= 0)
            {
                return CallUserFunction(etype, ret, @params, pcount, is_term, out str_result, out num_result);
            }

            // 実はシステム定義のグローバル変数？
            if (IsGlobalVariableDefined(expr))
            {
                return Event.GlobalVariableList[expr].ReferenceValue(etype, out str_result, out num_result);
            }

            // 結局ただの文字列……
            str_result = expr;
            return ValueType.StringType;
        }

        private ValueType CallSystemFunction(string expr, ValueType etype, string fname, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;
            var CallFunctionRet = ValueType.UndefinedType;

            //var PT = default(GUI.POINTAPI);
            //var in_window = default(bool);
            //int x2, x1, y1, y2;
            //DateTime d1, d2;
            //var list = default(string[]);
            //bool flag;
            //switch (Strings.LCase(fname) ?? "")
            //{
            //                // 多用される関数を先に判定

            //                case "info":
            //                    {
            //                        var loopTo3 = pcount;
            //                        for (i = 1; i <= loopTo3; i++)
            //                            @params[i] = GetValueAsString(@params[i], is_term[i]);
            //                        str_result = EvalInfoFunc(@params);
            //                        if (etype == ValueType.NumericType)
            //                        {
            //                            num_result = GeneralLib.StrToDbl(str_result);
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.StringType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "instrb":
            //                    {
            //                        if (pcount == 2)
            //                        {
            //                            i = InStrB(Strings.StrConv(GetValueAsString(@params[1], is_term[1]), vbFromUnicode), Strings.StrConv(GetValueAsString(@params[2], is_term[2]), vbFromUnicode));
            //                        }
            //                        else
            //                        {
            //                            // params(3)が指定されている場合は、それを検索開始位置似設定
            //                            // VBのInStrは引数1が開始位置になりますが、現仕様との兼ね合いを考え、
            //                            // eve上では引数3に設定するようにしています
            //                            i = InStrB(GetValueAsLong(@params[3], is_term[3]), Strings.StrConv(GetValueAsString(@params[1], is_term[1]), vbFromUnicode), Strings.StrConv(GetValueAsString(@params[2], is_term[2]), vbFromUnicode));
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum((double)i);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            num_result = (double)i;
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "lindex":
            //                    {
            //                        str_result = GeneralLib.ListIndex(GetValueAsString(@params[1], is_term[1]), GetValueAsLong(@params[2], is_term[2]));

            //                        // 全体が()で囲まれている場合は()を外す
            //                        if (Strings.Left(str_result, 1) == "(" & Strings.Right(str_result, 1) == ")")
            //                        {
            //                            str_result = Strings.Mid(str_result, 2, Strings.Len(str_result) - 2);
            //                        }

            //                        if (etype == ValueType.NumericType)
            //                        {
            //                            num_result = GeneralLib.StrToDbl(str_result);
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.StringType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "llength":
            //                    {
            //                        i = GeneralLib.ListLength(GetValueAsString(@params[1], is_term[1]));
            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum((double)i);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            num_result = (double)i;
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "list":
            //                    {
            //                        str_result = GetValueAsString(@params[1], is_term[1]);
            //                        var loopTo4 = pcount;
            //                        for (i = 2; i <= loopTo4; i++)
            //                            str_result = str_result + " " + GetValueAsString(@params[i], is_term[i]);
            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //                // これ以降はアルファベット順

            //                case "action":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        Unit localItem2() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

            //                                        num_result = (double)localItem2().Action;
            //                                    }
            //                                    else if (localIsDefined())
            //                                    {
            //                                        {
            //                                            var withBlock2 = SRC.PList.Item((object)pname);
            //                                            if (withBlock2.Unit is object)
            //                                            {
            //                                                {
            //                                                    var withBlock3 = withBlock2.Unit;
            //                                                    if (withBlock3.Status_Renamed == "出撃" | withBlock3.Status_Renamed == "格納")
            //                                                    {
            //                                                        num_result = (double)withBlock3.Action;
            //                                                    }
            //                                                    else
            //                                                    {
            //                                                        num_result = 0d;
            //                                                    }
            //                                                }
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        num_result = (double)Event.SelectedUnitForEvent.Action;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "area":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined1() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        Unit localItem21() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

            //                                        str_result = localItem21().Area;
            //                                    }
            //                                    else if (localIsDefined1())
            //                                    {
            //                                        {
            //                                            var withBlock4 = SRC.PList.Item((object)pname);
            //                                            if (withBlock4.Unit is object)
            //                                            {
            //                                                str_result = withBlock4.Unit.Area;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        str_result = Event.SelectedUnitForEvent.Area;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //                case "asc":
            //                    {
            //                        num_result = (double)Strings.Asc(GetValueAsString(@params[1], is_term[1]));
            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "chr":
            //                    {
            //                        str_result = Conversions.ToString((char)GetValueAsLong(@params[1], is_term[1]));
            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //                case "condition":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    buf = GetValueAsString(@params[2], is_term[2]);
            //                                    bool localIsDefined2() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        Unit localItem22() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

            //                                        if (localItem22().IsConditionSatisfied((object)buf))
            //                                        {
            //                                            num_result = 1d;
            //                                        }
            //                                    }
            //                                    else if (localIsDefined2())
            //                                    {
            //                                        {
            //                                            var withBlock5 = SRC.PList.Item((object)pname);
            //                                            if (withBlock5.Unit is object)
            //                                            {
            //                                                if (withBlock5.Unit.IsConditionSatisfied((object)buf))
            //                                                {
            //                                                    num_result = 1d;
            //                                                }
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 1:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        buf = GetValueAsString(@params[1], is_term[1]);
            //                                        if (Event.SelectedUnitForEvent.IsConditionSatisfied((object)buf))
            //                                        {
            //                                            num_result = 1d;
            //                                        }
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "count":
            //                    {
            //                        expr = Strings.Trim(expr);
            //                        buf = Strings.Mid(expr, 7, Strings.Len(expr) - 7) + "[";
            //                        num = 0;

            //                        // サブルーチンローカル変数を検索
            //                        if (Event.CallDepth > 0)
            //                        {
            //                            var loopTo5 = Event.VarIndex;
            //                            for (i = (Event.VarIndexStack[Event.CallDepth - 1] + 1); i <= loopTo5; i++)
            //                            {
            //                                if (Strings.InStr(Event.VarStack[i].Name, buf) == 1)
            //                                {
            //                                    num = (num + 1);
            //                                }
            //                            }

            //                            if (num > 0)
            //                            {
            //                                if (etype == ValueType.StringType)
            //                                {
            //                                    str_result = GeneralLib.FormatNum((double)num);
            //                                    CallFunctionRet = ValueType.StringType;
            //                                }
            //                                else
            //                                {
            //                                    num_result = (double)num;
            //                                    CallFunctionRet = ValueType.NumericType;
            //                                }

            //                                return CallFunctionRet;
            //                            }
            //                        }

            //                        // ローカル変数を検索
            //                        foreach (VarData currentVar in Event.LocalVariableList)
            //                        {
            //                            var = currentVar;
            //                            if (Strings.InStr(var.Name, buf) == 1)
            //                            {
            //                                num = (num + 1);
            //                            }
            //                        }

            //                        if (num > 0)
            //                        {
            //                            if (etype == ValueType.StringType)
            //                            {
            //                                str_result = GeneralLib.FormatNum((double)num);
            //                                CallFunctionRet = ValueType.StringType;
            //                            }
            //                            else
            //                            {
            //                                num_result = (double)num;
            //                                CallFunctionRet = ValueType.NumericType;
            //                            }

            //                            return CallFunctionRet;
            //                        }

            //                        // グローバル変数を検索
            //                        foreach (VarData currentVar1 in Event.GlobalVariableList)
            //                        {
            //                            var = currentVar1;
            //                            if (Strings.InStr(var.Name, buf) == 1)
            //                            {
            //                                num = (num + 1);
            //                            }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum((double)num);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            num_result = (double)num;
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "countitem":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined3() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        Unit localItem23() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

            //                                        num = localItem23().CountItem();
            //                                    }
            //                                    else if (!localIsDefined3())
            //                                    {
            //                                        if (pname == "未装備")
            //                                        {
            //                                            num = 0;
            //                                            foreach (Item currentIt in SRC.IList)
            //                                            {
            //                                                it = currentIt;
            //                                                if (it.Unit is null & it.Exist)
            //                                                {
            //                                                    num = (num + 1);
            //                                                }
            //                                            }
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        {
            //                                            var withBlock6 = SRC.PList.Item((object)pname);
            //                                            if (withBlock6.Unit is object)
            //                                            {
            //                                                num = withBlock6.Unit.CountItem();
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        num = Event.SelectedUnitForEvent.CountItem();
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum((double)num);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            num_result = (double)num;
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "countpartner":
            //                    {
            //                        num_result = (double)Information.UBound(Commands.SelectedPartners);
            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "countpilot":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        {
            //                                            var withBlock7 = SRC.UList.Item2((object)pname);
            //                                            num_result = (double)(withBlock7.CountPilot() + withBlock7.CountSupport());
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        {
            //                                            var withBlock8 = SRC.PList.Item((object)pname);
            //                                            if (withBlock8.Unit is object)
            //                                            {
            //                                                {
            //                                                    var withBlock9 = withBlock8.Unit;
            //                                                    num_result = (double)(withBlock9.CountPilot() + withBlock9.CountSupport());
            //                                                }
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        {
            //                                            var withBlock10 = Event.SelectedUnitForEvent;
            //                                            num_result = (double)(withBlock10.CountPilot() + withBlock10.CountSupport());
            //                                        }
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "damage":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined4() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    Pilot localItem1() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        {
            //                                            var withBlock11 = SRC.UList.Item2((object)pname);
            //                                            num_result = (double)(100 * (withBlock11.MaxHP - withBlock11.HP) / withBlock11.MaxHP);
            //                                        }
            //                                    }
            //                                    else if (!localIsDefined4())
            //                                    {
            //                                        num_result = 100d;
            //                                    }
            //                                    else if (localItem1().Unit is null)
            //                                    {
            //                                        num_result = 100d;
            //                                    }
            //                                    else
            //                                    {
            //                                        Pilot localItem() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                                        {
            //                                            var withBlock12 = localItem().Unit;
            //                                            num_result = (double)(100 * (withBlock12.MaxHP - withBlock12.HP) / withBlock12.MaxHP);
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    {
            //                                        var withBlock13 = Event.SelectedUnitForEvent;
            //                                        num_result = (double)(100 * (withBlock13.MaxHP - withBlock13.HP) / withBlock13.MaxHP);
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "dir":
            //                    {
            //                        CallFunctionRet = ValueType.StringType;
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    fname = GetValueAsString(@params[1], is_term[1]);

            //                                    // フルパス指定でなければシナリオフォルダを起点に検索
            //                                    if (Strings.Mid(fname, 2, 1) != ":")
            //                                    {
            //                                        fname = SRC.ScenarioPath + fname;
            //                                    }

            //                                    switch (GetValueAsString(@params[2], is_term[2]) ?? "")
            //                                    {
            //                                        case "ファイル":
            //                                            {
            //                                                num = Constants.vbNormal;
            //                                                break;
            //                                            }

            //                                        case "フォルダ":
            //                                            {
            //                                                num = FileAttribute.Directory;
            //                                                break;
            //                                            }
            //                                    }
            //                                    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                                    str_result = FileSystem.Dir(fname, (FileAttribute)num);
            //                                    if (Strings.Len(str_result) == 0)
            //                                    {
            //                                        return CallFunctionRet;
            //                                    }

            //                                    // ファイル属性チェック用に検索パスを作成
            //                                    dir_path = fname;
            //                                    if (num == FileAttribute.Directory)
            //                                    {
            //                                        i = GeneralLib.InStr2(fname, @"\");
            //                                        if (i > 0)
            //                                        {
            //                                            dir_path = Strings.Left(fname, i);
            //                                        }
            //                                    }

            //                                    // 単一ファイルの検索？
            //                                    if (Strings.InStr(fname, "*") == 0)
            //                                    {
            //                                        // フォルダの検索の場合は見つかったファイルがフォルダ
            //                                        // かどうかチェックする
            //                                        if (num == FileAttribute.Directory)
            //                                        {
            //                                            if ((FileSystem.GetAttr(dir_path + str_result) & num) == 0)
            //                                            {
            //                                                str_result = "";
            //                                            }
            //                                        }

            //                                        return CallFunctionRet;
            //                                    }

            //                                    if (str_result == ".")
            //                                    {
            //                                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                                        str_result = FileSystem.Dir();
            //                                    }

            //                                    if (str_result == "..")
            //                                    {
            //                                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                                        str_result = FileSystem.Dir();
            //                                    }

            //                                    // 検索されたファイル一覧を作成
            //                                    dir_list = new string[1];
            //                                    if (num == FileAttribute.Directory)
            //                                    {
            //                                        while (Strings.Len(str_result) > 0)
            //                                        {
            //                                            // フォルダの検索の場合は見つかったファイルがフォルダ
            //                                            // かどうかチェックする
            //                                            if ((FileSystem.GetAttr(dir_path + str_result) & num) != 0)
            //                                            {
            //                                                Array.Resize(dir_list, Information.UBound(dir_list) + 1 + 1);
            //                                                dir_list[Information.UBound(dir_list)] = str_result;
            //                                            }
            //                                            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                                            str_result = FileSystem.Dir();
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        while (Strings.Len(str_result) > 0)
            //                                        {
            //                                            Array.Resize(dir_list, Information.UBound(dir_list) + 1 + 1);
            //                                            dir_list[Information.UBound(dir_list)] = str_result;
            //                                            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                                            str_result = FileSystem.Dir();
            //                                        }
            //                                    }

            //                                    if (Information.UBound(dir_list) > 0)
            //                                    {
            //                                        str_result = dir_list[1];
            //                                        dir_index = 2;
            //                                    }
            //                                    else
            //                                    {
            //                                        str_result = "";
            //                                        dir_index = 1;
            //                                    }

            //                                    break;
            //                                }

            //                            case 1:
            //                                {
            //                                    fname = GetValueAsString(@params[1], is_term[1]);

            //                                    // フルパス指定でなければシナリオフォルダを起点に検索
            //                                    if (Strings.Mid(fname, 2, 1) != ":")
            //                                    {
            //                                        fname = SRC.ScenarioPath + fname;
            //                                    }

            //                                    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                                    str_result = FileSystem.Dir(fname, FileAttribute.Directory);
            //                                    if (Strings.Len(str_result) == 0)
            //                                    {
            //                                        return CallFunctionRet;
            //                                    }

            //                                    // 単一ファイルの検索？
            //                                    if (Strings.InStr(fname, "*") == 0)
            //                                    {
            //                                        return CallFunctionRet;
            //                                    }

            //                                    if (str_result == ".")
            //                                    {
            //                                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                                        str_result = FileSystem.Dir();
            //                                    }

            //                                    if (str_result == "..")
            //                                    {
            //                                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                                        str_result = FileSystem.Dir();
            //                                    }

            //                                    // 検索されたファイル一覧を作成
            //                                    dir_list = new string[1];
            //                                    while (Strings.Len(str_result) > 0)
            //                                    {
            //                                        Array.Resize(dir_list, Information.UBound(dir_list) + 1 + 1);
            //                                        dir_list[Information.UBound(dir_list)] = str_result;
            //                                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                                        str_result = FileSystem.Dir();
            //                                    }

            //                                    if (Information.UBound(dir_list) > 0)
            //                                    {
            //                                        str_result = dir_list[1];
            //                                        dir_index = 2;
            //                                    }
            //                                    else
            //                                    {
            //                                        str_result = "";
            //                                        dir_index = 1;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (dir_index <= Information.UBound(dir_list))
            //                                    {
            //                                        str_result = dir_list[dir_index];
            //                                        dir_index = (dir_index + 1);
            //                                    }
            //                                    else
            //                                    {
            //                                        str_result = "";
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "eof":
            //                    {
            //                        if (etype == ValueType.StringType)
            //                        {
            //                            if (FileSystem.EOF(GetValueAsLong(@params[1], is_term[1])))
            //                            {
            //                                str_result = "1";
            //                            }
            //                            else
            //                            {
            //                                str_result = "0";
            //                            }

            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            if (FileSystem.EOF(GetValueAsLong(@params[1], is_term[1])))
            //                            {
            //                                num_result = 1d;
            //                            }

            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "en":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined5() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        Unit localItem24() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

            //                                        num_result = (double)localItem24().EN;
            //                                    }
            //                                    else if (localIsDefined5())
            //                                    {
            //                                        {
            //                                            var withBlock14 = SRC.PList.Item((object)pname);
            //                                            if (withBlock14.Unit is object)
            //                                            {
            //                                                num_result = (double)withBlock14.Unit.EN;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        num_result = (double)Event.SelectedUnitForEvent.EN;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "eval":
            //                    {
            //                        buf = Strings.Trim(GetValueAsString(@params[1], is_term[1]));
            //                        CallFunctionRet = EvalExpr(buf, etype, str_result, num_result);
            //                        return CallFunctionRet;
            //                    }

            //                case "font":
            //                    {
            //                        switch (GetValueAsString(@params[1], is_term[1]) ?? "")
            //                        {
            //                            case "フォント名":
            //                                {
            //                                    str_result = GUI.MainForm.picMain(0).Font.Name;
            //                                    CallFunctionRet = ValueType.StringType;
            //                                    break;
            //                                }

            //                            case "サイズ":
            //                                {
            //                                    num_result = GUI.MainForm.picMain(0).Font.Size;
            //                                    if (etype == ValueType.StringType)
            //                                    {
            //                                        str_result = GeneralLib.FormatNum(num_result);
            //                                        CallFunctionRet = ValueType.StringType;
            //                                    }
            //                                    else
            //                                    {
            //                                        CallFunctionRet = ValueType.NumericType;
            //                                    }

            //                                    break;
            //                                }

            //                            case "太字":
            //                                {
            //                                    if (GUI.MainForm.picMain(0).Font.Bold)
            //                                    {
            //                                        num_result = 1d;
            //                                    }
            //                                    else
            //                                    {
            //                                        num_result = 0d;
            //                                    }

            //                                    if (etype == ValueType.StringType)
            //                                    {
            //                                        str_result = GeneralLib.FormatNum(num_result);
            //                                        CallFunctionRet = ValueType.StringType;
            //                                    }
            //                                    else
            //                                    {
            //                                        CallFunctionRet = ValueType.NumericType;
            //                                    }

            //                                    break;
            //                                }

            //                            case "斜体":
            //                                {
            //                                    if (GUI.MainForm.picMain(0).Font.Italic)
            //                                    {
            //                                        num_result = 1d;
            //                                    }
            //                                    else
            //                                    {
            //                                        num_result = 0d;
            //                                    }

            //                                    if (etype == ValueType.StringType)
            //                                    {
            //                                        str_result = GeneralLib.FormatNum(num_result);
            //                                        CallFunctionRet = ValueType.StringType;
            //                                    }
            //                                    else
            //                                    {
            //                                        CallFunctionRet = ValueType.NumericType;
            //                                    }

            //                                    break;
            //                                }

            //                            case "色":
            //                                {
            //                                    str_result = Hex(GUI.MainForm.picMain(0).ForeColor);
            //                                    var loopTo6 = (6 - Strings.Len(str_result));
            //                                    for (i = 1; i <= loopTo6; i++)
            //                                        str_result = "0" + str_result;
            //                                    str_result = "#" + str_result;
            //                                    CallFunctionRet = ValueType.StringType;
            //                                    break;
            //                                }

            //                            case "書き込み":
            //                                {
            //                                    if (GUI.PermanentStringMode)
            //                                    {
            //                                        str_result = "背景";
            //                                    }
            //                                    else if (GUI.KeepStringMode)
            //                                    {
            //                                        str_result = "保持";
            //                                    }
            //                                    else
            //                                    {
            //                                        str_result = "通常";
            //                                    }

            //                                    CallFunctionRet = ValueType.StringType;
            //                                    break;
            //                                }
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "format":
            //                    {
            //                        str_result = SrcFormatter.Format(GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]));
            //                        if (etype == ValueType.NumericType)
            //                        {
            //                            num_result = GeneralLib.StrToDbl(str_result);
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.StringType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "keystate":
            //                    {
            //                        if (pcount != 1)
            //                        {
            //                            return CallFunctionRet;
            //                        }

            //                        // キー番号
            //                        i = GetValueAsLong(@params[1], is_term[1]);

            //                        // 左利き設定に対応
            //                        switch (i)
            //                        {
            //                            case Keys.LButton:
            //                                {
            //                                    i = GUI.LButtonID;
            //                                    break;
            //                                }

            //                            case Keys.RButton:
            //                                {
            //                                    i = GUI.RButtonID;
            //                                    break;
            //                                }
            //                        }

            //                        if (i == Keys.LButton | i == Keys.RButton)
            //                        {
            //                            // マウスカーソルの位置を参照
            //                            GUI.GetCursorPos(PT);

            //                            // メインウインドウ上でマウスボタンを押している？
            //                            if (ReferenceEquals(Form.ActiveForm, GUI.MainForm))
            //                            {
            //                                {
            //                                    var withBlock15 = GUI.MainForm;
            //                                    x1 = (long)SrcFormatter.PixelsToTwipsX((double)withBlock15.Left) / (long)SrcFormatter.TwipsPerPixelX() + withBlock15.picMain(0).Left + 3;
            //                                    y1 = (long)SrcFormatter.PixelsToTwipsY((double)withBlock15.Top) / (long)SrcFormatter.TwipsPerPixelY() + withBlock15.picMain(0).Top + 28;
            //                                    x2 = x1 + withBlock15.picMain(0).Width;
            //                                    y2 = y1 + withBlock15.picMain(0).Height;
            //                                }

            //                                if (x1 <= PT.X & PT.X <= x2 & y1 <= PT.Y & PT.Y <= y2)
            //                                {
            //                                    in_window = true;
            //                                }
            //                            }
            //                        }
            //                        // メインウィンドウがアクティブになっている？
            //                        else if (ReferenceEquals(Form.ActiveForm, GUI.MainForm))
            //                        {
            //                            in_window = true;
            //                        }

            //                        // ウィンドウが選択されていない場合は常に0を返す
            //                        if (!in_window)
            //                        {
            //                            num_result = 0d;
            //                            if (etype == ValueType.StringType)
            //                            {
            //                                str_result = "0";
            //                                CallFunctionRet = ValueType.StringType;
            //                            }
            //                            else
            //                            {
            //                                CallFunctionRet = ValueType.NumericType;
            //                            }

            //                            return CallFunctionRet;
            //                        }

            //                        // キーの状態を参照
            //                        if (Conversions.ToBoolean(GUI.GetAsyncKeyState(i) & 0x8000))
            //                        {
            //                            num_result = 1d;
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "gettime":
            //                    {
            //                        num_result = (double)GeneralLib.timeGetTime();
            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "hp":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined6() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        Unit localItem25() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

            //                                        num_result = (double)localItem25().HP;
            //                                    }
            //                                    else if (localIsDefined6())
            //                                    {
            //                                        {
            //                                            var withBlock16 = SRC.PList.Item((object)pname);
            //                                            if (withBlock16.Unit is object)
            //                                            {
            //                                                num_result = (double)withBlock16.Unit.HP;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        num_result = (double)Event.SelectedUnitForEvent.HP;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "iif":
            //                    {
            //                        num = GeneralLib.ListSplit(@params[1], list);
            //                        switch (num)
            //                        {
            //                            case 1:
            //                                {
            //                                    var tmp1 = list;
            //                                    if (SRC.PList.IsDefined((object)tmp1[1]))
            //                                    {
            //                                        var tmp = list;
            //                                        {
            //                                            var withBlock17 = SRC.PList.Item((object)tmp[1]);
            //                                            if (withBlock17.Unit is null)
            //                                            {
            //                                                flag = false;
            //                                            }
            //                                            else
            //                                            {
            //                                                {
            //                                                    var withBlock18 = withBlock17.Unit;
            //                                                    if (withBlock18.Status_Renamed == "出撃" | withBlock18.Status_Renamed == "格納")
            //                                                    {
            //                                                        flag = true;
            //                                                    }
            //                                                    else
            //                                                    {
            //                                                        flag = false;
            //                                                    }
            //                                                }
            //                                            }
            //                                        }
            //                                    }
            //                                    else if (GetValueAsLong(@params[1]) != 0)
            //                                    {
            //                                        flag = true;
            //                                    }
            //                                    else
            //                                    {
            //                                        flag = false;
            //                                    }

            //                                    break;
            //                                }

            //                            case 2:
            //                                {
            //                                    pname = GeneralLib.ListIndex(expr, 2);
            //                                    var tmp3 = list;
            //                                    if (SRC.PList.IsDefined((object)tmp3[2]))
            //                                    {
            //                                        var tmp2 = list;
            //                                        {
            //                                            var withBlock19 = SRC.PList.Item((object)tmp2[2]);
            //                                            if (withBlock19.Unit is null)
            //                                            {
            //                                                flag = true;
            //                                            }
            //                                            else
            //                                            {
            //                                                {
            //                                                    var withBlock20 = withBlock19.Unit;
            //                                                    if (withBlock20.Status_Renamed == "出撃" | withBlock20.Status_Renamed == "格納")
            //                                                    {
            //                                                        flag = false;
            //                                                    }
            //                                                    else
            //                                                    {
            //                                                        flag = true;
            //                                                    }
            //                                                }
            //                                            }
            //                                        }
            //                                    }
            //                                    else if (GetValueAsLong(@params[1], true) == 0)
            //                                    {
            //                                        flag = true;
            //                                    }
            //                                    else
            //                                    {
            //                                        flag = false;
            //                                    }

            //                                    break;
            //                                }

            //                            default:
            //                                {
            //                                    if (GetValueAsLong(@params[1]) != 0)
            //                                    {
            //                                        flag = true;
            //                                    }
            //                                    else
            //                                    {
            //                                        flag = false;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (flag)
            //                        {
            //                            str_result = GetValueAsString(@params[2], is_term[2]);
            //                        }
            //                        else
            //                        {
            //                            str_result = GetValueAsString(@params[3], is_term[3]);
            //                        }

            //                        if (etype == ValueType.NumericType)
            //                        {
            //                            num_result = GeneralLib.StrToDbl(str_result);
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.StringType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "instrrev":
            //                    {
            //                        buf = GetValueAsString(@params[1], is_term[1]);
            //                        buf2 = GetValueAsString(@params[2], is_term[2]);
            //                        if (Strings.Len(buf2) > 0 & Strings.Len(buf) >= Strings.Len(buf2))
            //                        {
            //                            if (pcount == 2)
            //                            {
            //                                num = Strings.Len(buf);
            //                            }
            //                            else
            //                            {
            //                                num = GetValueAsLong(@params[3], is_term[3]);
            //                            }

            //                            i = (num - Strings.Len(buf2) + 1);
            //                            do
            //                            {
            //                                j = Strings.InStr(i, buf, buf2);
            //                                if (i == j)
            //                                {
            //                                    break;
            //                                }

            //                                i = (i - 1);
            //                            }
            //                            while (i != 0);
            //                        }
            //                        else
            //                        {
            //                            i = 0;
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum((double)i);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            num_result = (double)i;
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "instrrevb":
            //                    {
            //                        buf = GetValueAsString(@params[1], is_term[1]);
            //                        buf2 = GetValueAsString(@params[2], is_term[2]);

            //                        if (LenB(buf2) > 0 & LenB(buf) >= LenB(buf2))
            //                        {
            //                            if (pcount == 2)
            //                            {
            //                                num = LenB(buf);
            //                            }
            //                            else
            //                            {
            //                                num = GetValueAsLong(@params[3], is_term[3]);
            //                            }

            //                            i = num - LenB(buf2) + 1;
            //                            do
            //                            {
            //                                j = InStrB(i, buf, buf2);
            //                                if (i == j)
            //                                {
            //                                    break;
            //                                }

            //                                i = (i - 1);
            //                            }
            //                            while (i != 0);
            //                        }
            //                        else
            //                        {
            //                            i = 0;
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum((double)i);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            num_result = (double)i;
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "isavailable":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    buf = GetValueAsString(@params[2], is_term[2]);

            //                                    // エリアスが定義されている？
            //                                    if (SRC.ALDList.IsDefined((object)buf))
            //                                    {
            //                                        {
            //                                            var withBlock21 = SRC.ALDList.Item((object)buf);
            //                                            var loopTo7 = withBlock21.Count;
            //                                            for (i = 1; i <= loopTo7; i++)
            //                                            {
            //                                                string localLIndex() { string arglist = withBlock21.get_AliasData(i); var ret = GeneralLib.LIndex(arglist, 1); withBlock21.get_AliasData(i) = arglist; return ret; }

            //                                                if ((localLIndex() ?? "") == (buf ?? ""))
            //                                                {
            //                                                    buf = withBlock21.get_AliasType(i);
            //                                                    break;
            //                                                }
            //                                            }

            //                                            if (i > withBlock21.Count)
            //                                            {
            //                                                buf = withBlock21.get_AliasType(1);
            //                                            }
            //                                        }
            //                                    }

            //                                    bool localIsDefined7() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        Unit localItem26() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

            //                                        if (localItem26().IsFeatureAvailable(buf))
            //                                        {
            //                                            num_result = 1d;
            //                                        }
            //                                    }
            //                                    else if (localIsDefined7())
            //                                    {
            //                                        {
            //                                            var withBlock22 = SRC.PList.Item((object)pname);
            //                                            if (withBlock22.Unit is object)
            //                                            {
            //                                                if (withBlock22.Unit.IsFeatureAvailable(buf))
            //                                                {
            //                                                    num_result = 1d;
            //                                                }
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 1:
            //                                {
            //                                    buf = GetValueAsString(@params[1], is_term[1]);

            //                                    // エリアスが定義されている？
            //                                    if (SRC.ALDList.IsDefined((object)buf))
            //                                    {
            //                                        AliasDataType localItem3() { object argIndex1 = (object)buf; var ret = SRC.ALDList.Item(argIndex1); return ret; }

            //                                        buf = localItem3().get_AliasType(1);
            //                                    }

            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        if (Event.SelectedUnitForEvent.IsFeatureAvailable(buf))
            //                                        {
            //                                            num_result = 1d;
            //                                        }
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "isdefined":
            //                    {
            //                        pname = GetValueAsString(@params[1], is_term[1]);
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    switch (GetValueAsString(@params[2], is_term[2]) ?? "")
            //                                    {
            //                                        case "パイロット":
            //                                            {
            //                                                if (SRC.PList.IsDefined((object)pname))
            //                                                {
            //                                                    Pilot localItem4() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                                                    if (localItem4().Alive)
            //                                                    {
            //                                                        num_result = 1d;
            //                                                    }
            //                                                }

            //                                                break;
            //                                            }

            //                                        case "ユニット":
            //                                            {
            //                                                if (SRC.UList.IsDefined((object)pname))
            //                                                {
            //                                                    Unit localItem5() { object argIndex1 = (object)pname; var ret = SRC.UList.Item(argIndex1); return ret; }

            //                                                    if (localItem5().Status_Renamed != "破棄")
            //                                                    {
            //                                                        num_result = 1d;
            //                                                    }
            //                                                }

            //                                                break;
            //                                            }

            //                                        case "アイテム":
            //                                            {
            //                                                if (SRC.IList.IsDefined((object)pname))
            //                                                {
            //                                                    num_result = 1d;
            //                                                }

            //                                                break;
            //                                            }
            //                                    }

            //                                    break;
            //                                }

            //                            case 1:
            //                                {
            //                                    bool localIsDefined8() { object argIndex1 = (object)pname; var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                                    bool localIsDefined9() { object argIndex1 = (object)pname; var ret = SRC.IList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.PList.IsDefined((object)pname))
            //                                    {
            //                                        Pilot localItem6() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                                        if (localItem6().Alive)
            //                                        {
            //                                            num_result = 1d;
            //                                        }
            //                                    }
            //                                    else if (localIsDefined8())
            //                                    {
            //                                        Unit localItem7() { object argIndex1 = (object)pname; var ret = SRC.UList.Item(argIndex1); return ret; }

            //                                        if (localItem7().Status_Renamed != "破棄")
            //                                        {
            //                                            num_result = 1d;
            //                                        }
            //                                    }
            //                                    else if (localIsDefined9())
            //                                    {
            //                                        num_result = 1d;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "isequiped":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined10() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        Unit localItem27() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

            //                                        if (localItem27().IsEquiped(GetValueAsString(@params[2], is_term[2])))
            //                                        {
            //                                            num_result = 1d;
            //                                        }
            //                                    }
            //                                    else if (localIsDefined10())
            //                                    {
            //                                        {
            //                                            var withBlock23 = SRC.PList.Item((object)pname);
            //                                            if (withBlock23.Unit is object)
            //                                            {
            //                                                if (withBlock23.Unit.IsEquiped(GetValueAsString(@params[2], is_term[2])))
            //                                                {
            //                                                    num_result = 1d;
            //                                                }
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 1:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        if (Event.SelectedUnitForEvent.IsEquiped(GetValueAsString(@params[1], is_term[1])))
            //                                        {
            //                                            num_result = 1d;
            //                                        }
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "lsearch":
            //                    {
            //                        buf = GetValueAsString(@params[1], is_term[1]);
            //                        buf2 = GetValueAsString(@params[2], is_term[2]);
            //                        num = Conversions.Toint(Interaction.IIf(pcount < 3, (object)1, (object)GetValueAsLong(@params[3], is_term[3])));
            //                        num2 = GeneralLib.ListLength(buf);
            //                        var loopTo8 = num2;
            //                        for (i = num; i <= loopTo8; i++)
            //                        {
            //                            if ((GeneralLib.ListIndex(buf, i) ?? "") == (buf2 ?? ""))
            //                            {
            //                                if (etype == ValueType.StringType)
            //                                {
            //                                    str_result = SrcFormatter.Format((object)i);
            //                                    CallFunctionRet = ValueType.StringType;
            //                                }
            //                                else
            //                                {
            //                                    num_result = (double)i;
            //                                    CallFunctionRet = ValueType.NumericType;
            //                                }

            //                                return CallFunctionRet;
            //                            }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = "0";
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            num_result = 0d;
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }


            //                case "isvardefined":
            //                    {
            //                        if (IsVariableDefined(Strings.Trim(Strings.Mid(expr, 14, Strings.Len(expr) - 14))))
            //                        {
            //                            if (etype == ValueType.StringType)
            //                            {
            //                                str_result = "1";
            //                                CallFunctionRet = ValueType.StringType;
            //                            }
            //                            else
            //                            {
            //                                num_result = 1d;
            //                                CallFunctionRet = ValueType.NumericType;
            //                            }
            //                        }
            //                        else if (etype == ValueType.StringType)
            //                        {
            //                            str_result = "0";
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            num_result = 0d;
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "item":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined11() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    Pilot localItem11() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                                    Pilot localItem12() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        i = GetValueAsLong(@params[2], is_term[2]);
            //                                        {
            //                                            var withBlock24 = SRC.UList.Item2((object)pname);
            //                                            if (1 <= i & i <= withBlock24.CountItem())
            //                                            {
            //                                                Item localItem8() { object argIndex1 = (object)i; var ret = withBlock24.Item(argIndex1); return ret; }

            //                                                str_result = localItem8().Name;
            //                                            }
            //                                        }
            //                                    }
            //                                    else if (!localIsDefined11())
            //                                    {
            //                                        if (pname == "未装備")
            //                                        {
            //                                            i = 0;
            //                                            j = GetValueAsLong(@params[2], is_term[2]);
            //                                            foreach (Item currentIt1 in SRC.IList)
            //                                            {
            //                                                it = currentIt1;
            //                                                if (it.Unit is null & it.Exist)
            //                                                {
            //                                                    i = (i + 1);
            //                                                    if (i == j)
            //                                                    {
            //                                                        str_result = it.Name;
            //                                                        break;
            //                                                    }
            //                                                }
            //                                            }
            //                                        }
            //                                    }
            //                                    else if (localItem12().Unit is object)
            //                                    {
            //                                        i = GetValueAsLong(@params[2], is_term[2]);
            //                                        Pilot localItem10() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                                        {
            //                                            var withBlock25 = localItem10().Unit;
            //                                            if (1 <= i & i <= withBlock25.CountItem())
            //                                            {
            //                                                Item localItem9() { object argIndex1 = (object)i; var ret = withBlock25.Item(argIndex1); return ret; }

            //                                                str_result = localItem9().Name;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 1:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        i = GetValueAsLong(@params[1], is_term[1]);
            //                                        {
            //                                            var withBlock26 = Event.SelectedUnitForEvent;
            //                                            if (1 <= i & i <= withBlock26.CountItem())
            //                                            {
            //                                                Item localItem13() { object argIndex1 = (object)i; var ret = withBlock26.Item(argIndex1); return ret; }

            //                                                str_result = localItem13().Name;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //                case "itemid":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined12() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    Pilot localItem17() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                                    Pilot localItem18() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        i = GetValueAsLong(@params[2], is_term[2]);
            //                                        {
            //                                            var withBlock27 = SRC.UList.Item2((object)pname);
            //                                            if (1 <= i & i <= withBlock27.CountItem())
            //                                            {
            //                                                Item localItem14() { object argIndex1 = (object)i; var ret = withBlock27.Item(argIndex1); return ret; }

            //                                                str_result = localItem14().ID;
            //                                            }
            //                                        }
            //                                    }
            //                                    else if (!localIsDefined12())
            //                                    {
            //                                        if (pname == "未装備")
            //                                        {
            //                                            i = 0;
            //                                            j = GetValueAsLong(@params[2], is_term[2]);
            //                                            foreach (Item currentIt2 in SRC.IList)
            //                                            {
            //                                                it = currentIt2;
            //                                                if (it.Unit is null & it.Exist)
            //                                                {
            //                                                    i = (i + 1);
            //                                                    if (i == j)
            //                                                    {
            //                                                        str_result = it.ID;
            //                                                        break;
            //                                                    }
            //                                                }
            //                                            }
            //                                        }
            //                                    }
            //                                    else if (localItem18().Unit is object)
            //                                    {
            //                                        i = GetValueAsLong(@params[2], is_term[2]);
            //                                        Pilot localItem16() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                                        {
            //                                            var withBlock28 = localItem16().Unit;
            //                                            if (1 <= i & i <= withBlock28.CountItem())
            //                                            {
            //                                                Item localItem15() { object argIndex1 = (object)i; var ret = withBlock28.Item(argIndex1); return ret; }

            //                                                str_result = localItem15().ID;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 1:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        i = GetValueAsLong(@params[1], is_term[1]);
            //                                        {
            //                                            var withBlock29 = Event.SelectedUnitForEvent;
            //                                            if (1 <= i & i <= withBlock29.CountItem())
            //                                            {
            //                                                Item localItem19() { object argIndex1 = (object)i; var ret = withBlock29.Item(argIndex1); return ret; }

            //                                                str_result = localItem19().ID;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //                case "left":
            //                    {
            //                        str_result = Strings.Left(GetValueAsString(@params[1], is_term[1]), GetValueAsLong(@params[2], is_term[2]));
            //                        if (etype == ValueType.NumericType)
            //                        {
            //                            num_result = GeneralLib.StrToDbl(str_result);
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.StringType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "leftb":
            //                    {
            //                        buf = GetValueAsString(@params[1], is_term[1]);
            //                        str_result = LeftB(Strings.StrConv(buf, vbFromUnicode), GetValueAsLong(@params[2], is_term[2]));
            //                        str_result = Strings.StrConv(str_result, vbUnicode);
            //                        if (etype == ValueType.NumericType)
            //                        {
            //                            num_result = GeneralLib.StrToDbl(str_result);
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.StringType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "len":
            //                    {
            //                        num_result = (double)Strings.Len(GetValueAsString(@params[1], is_term[1]));
            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "lenb":
            //                    {
            //                        buf = GetValueAsString(@params[1], is_term[1]);
            //                        num_result = LenB(Strings.StrConv(buf, vbFromUnicode));
            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "level":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined13() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        Unit localItem28() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

            //                                        num = localItem28().MainPilot().Level;
            //                                    }
            //                                    else if (!localIsDefined13())
            //                                    {
            //                                        num_result = 0d;
            //                                    }
            //                                    else
            //                                    {
            //                                        Pilot localItem20() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                                        num_result = (double)localItem20().Level;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        {
            //                                            var withBlock30 = Event.SelectedUnitForEvent;
            //                                            if (withBlock30.CountPilot() > 0)
            //                                            {
            //                                                num_result = (double)withBlock30.MainPilot().Level;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "lcase":
            //                    {
            //                        str_result = Strings.LCase(GetValueAsString(@params[1], is_term[1]));
            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //                case "lset":
            //                    {
            //                        buf = GetValueAsString(@params[1], is_term[1]);
            //                        i = GetValueAsLong(@params[2], is_term[2]);
            //                        if (LenB(Strings.StrConv(buf, vbFromUnicode)) < i)
            //                        {
            //                            str_result = buf + Strings.Space(i - LenB(Strings.StrConv(buf, vbFromUnicode)));
            //                        }
            //                        else
            //                        {
            //                            str_result = buf;
            //                        }

            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //                case "max":
            //                    {
            //                        num_result = GetValueAsDouble(@params[1], is_term[1]);
            //                        var loopTo9 = pcount;
            //                        for (i = 2; i <= loopTo9; i++)
            //                        {
            //                            rdbl = GetValueAsDouble(@params[i], is_term[i]);
            //                            if (num_result < rdbl)
            //                            {
            //                                num_result = rdbl;
            //                            }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "mid":
            //                    {
            //                        buf = GetValueAsString(@params[1], is_term[1]);
            //                        switch (pcount)
            //                        {
            //                            case 3:
            //                                {
            //                                    i = GetValueAsLong(@params[2], is_term[2]);
            //                                    j = GetValueAsLong(@params[3], is_term[3]);
            //                                    str_result = Strings.Mid(buf, i, j);
            //                                    break;
            //                                }

            //                            case 2:
            //                                {
            //                                    i = GetValueAsLong(@params[2], is_term[2]);
            //                                    str_result = Strings.Mid(buf, i);
            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.NumericType)
            //                        {
            //                            num_result = GeneralLib.StrToDbl(str_result);
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.StringType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "midb":
            //                    {
            //                        buf = GetValueAsString(@params[1], is_term[1]);
            //                        switch (pcount)
            //                        {
            //                            case 3:
            //                                {
            //                                    i = GetValueAsLong(@params[2], is_term[2]);
            //                                    j = GetValueAsLong(@params[3], is_term[3]);
            //                                    str_result = MidB(Strings.StrConv(buf, vbFromUnicode), i, j);
            //                                    break;
            //                                }

            //                            case 2:
            //                                {
            //                                    i = GetValueAsLong(@params[2], is_term[2]);
            //                                    str_result = MidB(Strings.StrConv(buf, vbFromUnicode), i);
            //                                    break;
            //                                }
            //                        }
            //                        str_result = Strings.StrConv(str_result, vbUnicode);
            //                        if (etype == ValueType.NumericType)
            //                        {
            //                            num_result = GeneralLib.StrToDbl(str_result);
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.StringType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "min":
            //                    {
            //                        num_result = GetValueAsDouble(@params[1], is_term[1]);
            //                        var loopTo10 = pcount;
            //                        for (i = 2; i <= loopTo10; i++)
            //                        {
            //                            rdbl = GetValueAsDouble(@params[i], is_term[i]);
            //                            if (num_result > rdbl)
            //                            {
            //                                num_result = rdbl;
            //                            }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "morale":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined14() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        Unit localItem29() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

            //                                        num_result = (double)localItem29().MainPilot().Morale;
            //                                    }
            //                                    else if (localIsDefined14())
            //                                    {
            //                                        Pilot localItem30() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                                        num_result = (double)localItem30().Morale;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        {
            //                                            var withBlock31 = Event.SelectedUnitForEvent;
            //                                            if (withBlock31.CountPilot() > 0)
            //                                            {
            //                                                num_result = (double)withBlock31.MainPilot().Morale;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "nickname":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    buf = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined15() { object argIndex1 = (object)buf; var ret = SRC.PDList.IsDefined(argIndex1); return ret; }

            //                                    bool localIsDefined16() { object argIndex1 = (object)buf; var ret = SRC.NPDList.IsDefined(argIndex1); return ret; }

            //                                    bool localIsDefined17() { object argIndex1 = (object)buf; var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                                    bool localIsDefined18() { object argIndex1 = (object)buf; var ret = SRC.UDList.IsDefined(argIndex1); return ret; }

            //                                    bool localIsDefined19() { object argIndex1 = (object)buf; var ret = SRC.IDList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.PList.IsDefined((object)buf))
            //                                    {
            //                                        Pilot localItem31() { object argIndex1 = (object)buf; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                                        str_result = localItem31().get_Nickname(false);
            //                                    }
            //                                    else if (localIsDefined15())
            //                                    {
            //                                        PilotData localItem32() { object argIndex1 = (object)buf; var ret = SRC.PDList.Item(argIndex1); return ret; }

            //                                        str_result = localItem32().Nickname;
            //                                    }
            //                                    else if (localIsDefined16())
            //                                    {
            //                                        NonPilotData localItem33() { object argIndex1 = (object)buf; var ret = SRC.NPDList.Item(argIndex1); return ret; }

            //                                        str_result = localItem33().Nickname;
            //                                    }
            //                                    else if (localIsDefined17())
            //                                    {
            //                                        Unit localItem34() { object argIndex1 = (object)buf; var ret = SRC.UList.Item(argIndex1); return ret; }

            //                                        str_result = localItem34().Nickname0;
            //                                    }
            //                                    else if (localIsDefined18())
            //                                    {
            //                                        UnitData localItem35() { object argIndex1 = (object)buf; var ret = SRC.UDList.Item(argIndex1); return ret; }

            //                                        str_result = localItem35().Nickname;
            //                                    }
            //                                    else if (localIsDefined19())
            //                                    {
            //                                        ItemData localItem36() { object argIndex1 = (object)buf; var ret = SRC.IDList.Item(argIndex1); return ret; }

            //                                        str_result = localItem36().Nickname;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        str_result = Event.SelectedUnitForEvent.Nickname0;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //                case "partner":
            //                    {
            //                        i = GetValueAsLong(@params[1], is_term[1]);
            //                        if (i == 0)
            //                        {
            //                            str_result = Event.SelectedUnitForEvent.ID;
            //                        }
            //                        else if (1 <= i & i <= Information.UBound(Commands.SelectedPartners))
            //                        {
            //                            str_result = Commands.SelectedPartners[i].ID;
            //                        }

            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //                case "party":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined20() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        Unit localItem210() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

            //                                        str_result = localItem210().Party0;
            //                                    }
            //                                    else if (localIsDefined20())
            //                                    {
            //                                        Pilot localItem37() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                                        str_result = localItem37().Party;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        str_result = Event.SelectedUnitForEvent.Party0;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //                case "pilot":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    uname = GetValueAsString(@params[1], is_term[1]);
            //                                    if (SRC.UList.IsDefined((object)uname))
            //                                    {
            //                                        i = GetValueAsLong(@params[2], is_term[2]);
            //                                        {
            //                                            var withBlock32 = SRC.UList.Item((object)uname);
            //                                            if (0 < i & i <= withBlock32.CountPilot())
            //                                            {
            //                                                Pilot localPilot() { object argIndex1 = (object)i; var ret = withBlock32.Pilot(argIndex1); return ret; }

            //                                                str_result = localPilot().Name;
            //                                            }
            //                                            else if (withBlock32.CountPilot() < i & i <= (withBlock32.CountPilot() + withBlock32.CountSupport()))
            //                                            {
            //                                                Pilot localSupport() { object argIndex1 = (object)(i - withBlock32.CountPilot()); var ret = withBlock32.Support(argIndex1); return ret; }

            //                                                str_result = localSupport().Name;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 1:
            //                                {
            //                                    uname = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined21() { object argIndex1 = (object)uname; var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                                    if (GeneralLib.IsNumber(uname))
            //                                    {
            //                                        if (Event.SelectedUnitForEvent is object)
            //                                        {
            //                                            i = Conversions.Toint(uname);
            //                                            {
            //                                                var withBlock33 = Event.SelectedUnitForEvent;
            //                                                if (0 < i & i <= withBlock33.CountPilot())
            //                                                {
            //                                                    Pilot localPilot1() { object argIndex1 = (object)i; var ret = withBlock33.Pilot(argIndex1); return ret; }

            //                                                    str_result = localPilot1().Name;
            //                                                }
            //                                                else if (withBlock33.CountPilot() < i & i <= (withBlock33.CountPilot() + withBlock33.CountSupport()))
            //                                                {
            //                                                    Pilot localSupport1() { object argIndex1 = (object)(i - withBlock33.CountPilot()); var ret = withBlock33.Support(argIndex1); return ret; }

            //                                                    str_result = localSupport1().Name;
            //                                                }
            //                                            }
            //                                        }
            //                                    }
            //                                    else if (localIsDefined21())
            //                                    {
            //                                        {
            //                                            var withBlock34 = SRC.UList.Item((object)uname);
            //                                            if (withBlock34.CountPilot() > 0)
            //                                            {
            //                                                str_result = withBlock34.MainPilot().Name;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        {
            //                                            var withBlock35 = Event.SelectedUnitForEvent;
            //                                            if (withBlock35.CountPilot() > 0)
            //                                            {
            //                                                str_result = withBlock35.MainPilot().Name;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //                case "pilotid":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    uname = GetValueAsString(@params[1], is_term[1]);
            //                                    if (SRC.UList.IsDefined((object)uname))
            //                                    {
            //                                        i = GetValueAsLong(@params[2], is_term[2]);
            //                                        {
            //                                            var withBlock36 = SRC.UList.Item((object)uname);
            //                                            if (0 < i & i <= withBlock36.CountPilot())
            //                                            {
            //                                                Pilot localPilot2() { object argIndex1 = (object)i; var ret = withBlock36.Pilot(argIndex1); return ret; }

            //                                                str_result = localPilot2().ID;
            //                                            }
            //                                            else if (withBlock36.CountPilot() < i & i <= (withBlock36.CountPilot() + withBlock36.CountSupport()))
            //                                            {
            //                                                Pilot localSupport2() { object argIndex1 = (object)(i - withBlock36.CountPilot()); var ret = withBlock36.Support(argIndex1); return ret; }

            //                                                str_result = localSupport2().ID;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 1:
            //                                {
            //                                    uname = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined22() { object argIndex1 = (object)uname; var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                                    if (GeneralLib.IsNumber(uname))
            //                                    {
            //                                        if (Event.SelectedUnitForEvent is object)
            //                                        {
            //                                            i = Conversions.Toint(uname);
            //                                            {
            //                                                var withBlock37 = Event.SelectedUnitForEvent;
            //                                                if (0 < i & i <= withBlock37.CountPilot())
            //                                                {
            //                                                    Pilot localPilot3() { object argIndex1 = (object)i; var ret = withBlock37.Pilot(argIndex1); return ret; }

            //                                                    str_result = localPilot3().ID;
            //                                                }
            //                                                else if (withBlock37.CountPilot() < i & i <= (withBlock37.CountPilot() + withBlock37.CountSupport()))
            //                                                {
            //                                                    Pilot localSupport3() { object argIndex1 = (object)(i - withBlock37.CountPilot()); var ret = withBlock37.Support(argIndex1); return ret; }

            //                                                    str_result = localSupport3().ID;
            //                                                }
            //                                            }
            //                                        }
            //                                    }
            //                                    else if (localIsDefined22())
            //                                    {
            //                                        {
            //                                            var withBlock38 = SRC.UList.Item((object)uname);
            //                                            if (withBlock38.CountPilot() > 0)
            //                                            {
            //                                                str_result = withBlock38.MainPilot().ID;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        {
            //                                            var withBlock39 = Event.SelectedUnitForEvent;
            //                                            if (withBlock39.CountPilot() > 0)
            //                                            {
            //                                                str_result = withBlock39.MainPilot().ID;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //                case "plana":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined23() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        Unit localItem211() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

            //                                        num_result = (double)localItem211().MainPilot().Plana;
            //                                    }
            //                                    else if (localIsDefined23())
            //                                    {
            //                                        Pilot localItem38() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                                        num_result = (double)localItem38().Plana;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        num_result = (double)Event.SelectedUnitForEvent.MainPilot().Plana;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "random":
            //                    {
            //                        num_result = (double)GeneralLib.Dice(GetValueAsLong(@params[1], is_term[1]));
            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "rank":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined24() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        Unit localItem212() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

            //                                        num_result = (double)localItem212().Rank;
            //                                    }
            //                                    else if (!localIsDefined24())
            //                                    {
            //                                        num_result = 0d;
            //                                    }
            //                                    else
            //                                    {
            //                                        {
            //                                            var withBlock40 = SRC.PList.Item((object)pname);
            //                                            if (withBlock40.Unit is object)
            //                                            {
            //                                                num_result = (double)withBlock40.Unit.Rank;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        num_result = (double)Event.SelectedUnitForEvent.Rank;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "regexp":
            //                    {
            //                        ;
            //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            //                        /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo RegExp_Error' at character 111360


            //                        Input:
            //                                        On Error GoTo RegExp_Error

            //                         */
            //                        if (RegEx is null)
            //                        {
            //                            RegEx = Interaction.CreateObject("VBScript.RegExp");
            //                        }

            //                        // RegExp(文字列, パターン[,大小区別あり|大小区別なし])
            //                        buf = "";
            //                        if (pcount > 0)
            //                        {
            //                            // 文字列全体を検索
            //                            // UPGRADE_WARNING: オブジェクト RegEx.Global の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                            RegEx.Global = (object)true;
            //                            // 大文字小文字の区別（True=区別しない）
            //                            // UPGRADE_WARNING: オブジェクト RegEx.IgnoreCase の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                            RegEx.IgnoreCase = (object)false;
            //                            if (pcount >= 3)
            //                            {
            //                                if (GetValueAsString(@params[3], is_term[3]) == "大小区別なし")
            //                                {
            //                                    // UPGRADE_WARNING: オブジェクト RegEx.IgnoreCase の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                                    RegEx.IgnoreCase = (object)true;
            //                                }
            //                            }
            //                            // 検索パターン
            //                            // UPGRADE_WARNING: オブジェクト RegEx.Pattern の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                            RegEx.Pattern = GetValueAsString(@params[2], is_term[2]);
            //                            // UPGRADE_WARNING: オブジェクト RegEx.Execute の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                            Matches = RegEx.Execute(GetValueAsString(@params[1], is_term[1]));
            //                            // UPGRADE_WARNING: オブジェクト Matches.Count の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(Matches.Count, 0, false)))
            //                            {
            //                                regexp_index = -1;
            //                            }
            //                            else
            //                            {
            //                                regexp_index = 0;
            //                                // UPGRADE_WARNING: オブジェクト Matches() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                                buf = Conversions.ToString(Expression.Matches((object)regexp_index));
            //                            }
            //                        }
            //                        else if (regexp_index >= 0)
            //                        {
            //                            regexp_index = (regexp_index + 1);
            //                            // UPGRADE_WARNING: オブジェクト Matches.Count の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectLessEqual(regexp_index, Operators.SubtractObject(Matches.Count, 1), false)))
            //                            {
            //                                // UPGRADE_WARNING: オブジェクト Matches() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                                buf = Conversions.ToString(Expression.Matches((object)regexp_index));
            //                            }
            //                        }

            //                        str_result = buf;
            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    RegExp_Error:
            //                        ;
            //                        Event.DisplayEventErrorMessage(Event.CurrentLineNum, "VBScriptがインストールされていません");
            //                        return CallFunctionRet;
            //                    }
            //                // RegExpReplace(文字列, 検索パターン, 置換パターン[,大小区別あり|大小区別なし])

            //                case "regexpreplace":
            //                    {
            //                        ;
            //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            //                        /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo RegExpReplace...' at character 114835


            //                        Input:
            //                                        'RegExpReplace(文字列, 検索パターン, 置換パターン[,大小区別あり|大小区別なし])

            //                                        On Error GoTo RegExpReplace_Error

            //                         */
            //                        if (RegEx is null)
            //                        {
            //                            RegEx = Interaction.CreateObject("VBScript.RegExp");
            //                        }

            //                        // 文字列全体を検索
            //                        // UPGRADE_WARNING: オブジェクト RegEx.Global の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                        RegEx.Global = (object)true;
            //                        // 大文字小文字の区別（True=区別しない）
            //                        // UPGRADE_WARNING: オブジェクト RegEx.IgnoreCase の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                        RegEx.IgnoreCase = (object)false;
            //                        if (pcount >= 4)
            //                        {
            //                            if (GetValueAsString(@params[4], is_term[4]) == "大小区別なし")
            //                            {
            //                                // UPGRADE_WARNING: オブジェクト RegEx.IgnoreCase の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                                RegEx.IgnoreCase = (object)true;
            //                            }
            //                        }
            //                        // 検索パターン
            //                        // UPGRADE_WARNING: オブジェクト RegEx.Pattern の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                        RegEx.Pattern = GetValueAsString(@params[2], is_term[2]);

            //                        // 置換実行
            //                        // UPGRADE_WARNING: オブジェクト RegEx.Replace の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                        buf = Conversions.ToString(RegEx.Replace(GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[3], is_term[3])));
            //                        str_result = buf;
            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    RegExpReplace_Error:
            //                        ;
            //                        Event.DisplayEventErrorMessage(Event.CurrentLineNum, "VBScriptがインストールされていません");
            //                        return CallFunctionRet;
            //                    }

            //                case "relation":
            //                    {
            //                        pname = GetValueAsString(@params[1], is_term[1]);
            //                        bool localIsDefined25() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                        if (!localIsDefined25())
            //                        {
            //                            num_result = 0d;
            //                            if (etype == ValueType.StringType)
            //                            {
            //                                str_result = "0";
            //                                CallFunctionRet = ValueType.StringType;
            //                            }
            //                            else
            //                            {
            //                                CallFunctionRet = ValueType.NumericType;
            //                            }

            //                            return CallFunctionRet;
            //                        }

            //                        Pilot localItem39() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                        pname = localItem39().Name;
            //                        pname2 = GetValueAsString(@params[2], is_term[2]);
            //                        bool localIsDefined26() { object argIndex1 = (object)pname2; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                        if (!localIsDefined26())
            //                        {
            //                            num_result = 0d;
            //                            if (etype == ValueType.StringType)
            //                            {
            //                                str_result = "0";
            //                                CallFunctionRet = ValueType.StringType;
            //                            }
            //                            else
            //                            {
            //                                CallFunctionRet = ValueType.NumericType;
            //                            }

            //                            return CallFunctionRet;
            //                        }

            //                        Pilot localItem40() { object argIndex1 = (object)pname2; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                        pname2 = localItem40().Name;
            //                        num_result = (double)GetValueAsLong("関係:" + pname + ":" + pname2);
            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "replace":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 4:
            //                                {
            //                                    buf = GetValueAsString(@params[1], is_term[1]);
            //                                    num = GetValueAsLong(@params[4], is_term[4]);
            //                                    buf2 = Strings.Right(buf, Strings.Len(buf) - num + 1);
            //                                    GeneralLib.ReplaceString(buf2, GetValueAsString(@params[2], is_term[2]), GetValueAsString(@params[3], is_term[3]));
            //                                    str_result = Strings.Left(buf, num - 1) + buf2;
            //                                    break;
            //                                }

            //                            case 5:
            //                                {
            //                                    buf = GetValueAsString(@params[1], is_term[1]);
            //                                    num = GetValueAsLong(@params[4], is_term[4]);
            //                                    num2 = GetValueAsLong(@params[5], is_term[5]);
            //                                    buf2 = Strings.Mid(buf, num, num2);
            //                                    GeneralLib.ReplaceString(buf2, GetValueAsString(@params[2], is_term[2]), GetValueAsString(@params[3], is_term[3]));
            //                                    str_result = Strings.Left(buf, num - 1) + buf2 + Strings.Right(buf, Strings.Len(buf) - (num + num2 - 1) - 1);
            //                                    break;
            //                                }

            //                            default:
            //                                {
            //                                    str_result = GetValueAsString(@params[1], is_term[1]);
            //                                    GeneralLib.ReplaceString(str_result, GetValueAsString(@params[2], is_term[2]), GetValueAsString(@params[3], is_term[3]));
            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.NumericType)
            //                        {
            //                            num_result = GeneralLib.StrToDbl(str_result);
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.StringType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "rgb":
            //                    {
            //                        buf = Conversion.Hex(Information.RGB(GetValueAsLong(@params[1], is_term[1]), GetValueAsLong(@params[2], is_term[2]), GetValueAsLong(@params[3], is_term[3])));
            //                        var loopTo11 = (6 - Strings.Len(buf));
            //                        for (i = 1; i <= loopTo11; i++)
            //                            buf = "0" + buf;
            //                        str_result = "#000000";
            //                        var midTmp = Strings.Mid(buf, 5, 2);
            //                        StringType.MidStmtStr(str_result, 2, 2, midTmp);
            //                        var midTmp1 = Strings.Mid(buf, 3, 2);
            //                        StringType.MidStmtStr(str_result, 4, 2, midTmp1);
            //                        var midTmp2 = Strings.Mid(buf, 1, 2);
            //                        StringType.MidStmtStr(str_result, 6, 2, midTmp2);
            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //                case "right":
            //                    {
            //                        str_result = Strings.Right(GetValueAsString(@params[1], is_term[1]), GetValueAsLong(@params[2], is_term[2]));
            //                        if (etype == ValueType.NumericType)
            //                        {
            //                            num_result = GeneralLib.StrToDbl(str_result);
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.StringType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "rightb":
            //                    {
            //                        buf = GetValueAsString(@params[1], is_term[1]);
            //                        str_result = RightB(Strings.StrConv(buf, vbFromUnicode), GetValueAsLong(@params[2], is_term[2]));
            //                        str_result = Strings.StrConv(str_result, vbUnicode);
            //                        if (etype == ValueType.NumericType)
            //                        {
            //                            num_result = GeneralLib.StrToDbl(str_result);
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.StringType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "round":
            //                case "rounddown":
            //                case "roundup":
            //                    {
            //                        ldbl = GetValueAsDouble(@params[1], is_term[1]);
            //                        if (pcount == 1)
            //                        {
            //                            num2 = 0;
            //                        }
            //                        else
            //                        {
            //                            num2 = GetValueAsLong(@params[2], is_term[2]);
            //                        }

            //                        num = Conversion.Int(ldbl * Math.Pow(10d, (double)num2));
            //                        switch (Strings.LCase(fname) ?? "")
            //                        {
            //                            case "round":
            //                                {
            //                                    if (ldbl * Math.Pow(10d, (double)num2) - (double)num >= 0.5d)
            //                                    {
            //                                        num = (num + 1);
            //                                    }

            //                                    break;
            //                                }

            //                            case "roundup":
            //                                {
            //                                    if (ldbl * Math.Pow(10d, (double)num2) - (double)num > 0d)
            //                                    {
            //                                        num = (num + 1);
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        num_result = (double)num / Math.Pow(10d, (double)num2);
            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "rset":
            //                    {
            //                        buf = GetValueAsString(@params[1], is_term[1]);
            //                        i = GetValueAsLong(@params[2], is_term[2]);
            //                        if (LenB(Strings.StrConv(buf, vbFromUnicode)) < i)
            //                        {
            //                            str_result = Strings.Space(i - LenB(Strings.StrConv(buf, vbFromUnicode))) + buf;
            //                        }
            //                        else
            //                        {
            //                            str_result = buf;
            //                        }

            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //                case "sin":
            //                    {
            //                        num_result = Math.Sin(GetValueAsDouble(@params[1], is_term[1]));
            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "skill":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    buf = GetValueAsString(@params[2], is_term[2]);

            //                                    // エリアスが定義されている？
            //                                    if (SRC.ALDList.IsDefined((object)buf))
            //                                    {
            //                                        AliasDataType localItem41() { object argIndex1 = (object)buf; var ret = SRC.ALDList.Item(argIndex1); return ret; }

            //                                        buf = localItem41().get_AliasType(1);
            //                                    }

            //                                    bool localIsDefined27() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        Unit localItem213() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

            //                                        num_result = localItem213().MainPilot().SkillLevel((object)buf, ref_mode: "");
            //                                    }
            //                                    else if (localIsDefined27())
            //                                    {
            //                                        Pilot localItem42() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                                        num_result = localItem42().SkillLevel((object)buf, ref_mode: "");
            //                                    }

            //                                    break;
            //                                }

            //                            case 1:
            //                                {
            //                                    buf = GetValueAsString(@params[1], is_term[1]);

            //                                    // エリアスが定義されている？
            //                                    if (SRC.ALDList.IsDefined((object)buf))
            //                                    {
            //                                        AliasDataType localItem43() { object argIndex1 = (object)buf; var ret = SRC.ALDList.Item(argIndex1); return ret; }

            //                                        buf = localItem43().get_AliasType(1);
            //                                    }

            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        num_result = Event.SelectedUnitForEvent.MainPilot().SkillLevel((object)buf, ref_mode: "");
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "sp":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined28() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        Unit localItem214() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

            //                                        num_result = (double)localItem214().MainPilot().SP;
            //                                    }
            //                                    else if (localIsDefined28())
            //                                    {
            //                                        Pilot localItem44() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                                        num_result = (double)localItem44().SP;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        num_result = (double)Event.SelectedUnitForEvent.MainPilot().SP;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "specialpower":
            //                case "mind":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    buf = GetValueAsString(@params[2], is_term[2]);
            //                                    bool localIsDefined29() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        Unit localItem215() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

            //                                        if (localItem215().IsSpecialPowerInEffect(buf))
            //                                        {
            //                                            num_result = 1d;
            //                                        }
            //                                    }
            //                                    else if (localIsDefined29())
            //                                    {
            //                                        {
            //                                            var withBlock41 = SRC.PList.Item((object)pname);
            //                                            if (withBlock41.Unit is object)
            //                                            {
            //                                                if (withBlock41.Unit.IsSpecialPowerInEffect(buf))
            //                                                {
            //                                                    num_result = 1d;
            //                                                }
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 1:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        buf = GetValueAsString(@params[1], is_term[1]);
            //                                        if (Event.SelectedUnitForEvent.IsSpecialPowerInEffect(buf))
            //                                        {
            //                                            num_result = 1d;
            //                                        }
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "sqr":
            //                    {
            //                        num_result = Math.Sqrt(GetValueAsDouble(@params[1], is_term[1]));
            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "status":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined30() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        Unit localItem216() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

            //                                        str_result = localItem216().Status_Renamed;
            //                                    }
            //                                    else if (localIsDefined30())
            //                                    {
            //                                        {
            //                                            var withBlock42 = SRC.PList.Item((object)pname);
            //                                            if (withBlock42.Unit is object)
            //                                            {
            //                                                str_result = withBlock42.Unit.Status_Renamed;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        str_result = Event.SelectedUnitForEvent.Status_Renamed;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //                case "strcomp":
            //                    {
            //                        num_result = (double)Strings.StrComp(GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]));
            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "string":
            //                    {
            //                        buf = GetValueAsString(@params[2], is_term[2]);
            //                        if (Strings.Len(buf) <= 1)
            //                        {
            //                            str_result = new string(Conversions.ToChar(buf), GetValueAsLong(@params[1], is_term[1]));
            //                        }
            //                        else
            //                        {
            //                            // String関数では文字列の先頭しか繰り返しされないので、
            //                            // 長さが2以上の文字列の場合は別処理
            //                            str_result = "";
            //                            var loopTo12 = GetValueAsLong(@params[1], is_term[1]);
            //                            for (i = 1; i <= loopTo12; i++)
            //                                str_result = str_result + buf;
            //                        }

            //                        if (etype == ValueType.NumericType)
            //                        {
            //                            num_result = GeneralLib.StrToDbl(str_result);
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.StringType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "tan":
            //                    {
            //                        num_result = Math.Tan(GetValueAsDouble(@params[1], is_term[1]));
            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "term":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    pname = GetValueAsString(@params[2], is_term[2]);
            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        Unit localItem217() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

            //                                        str_result = Term(GetValueAsString(@params[1], is_term[1]), localItem217());
            //                                    }
            //                                    else
            //                                    {
            //                                        str_result = Term(GetValueAsString(@params[1], is_term[1]), u: null);
            //                                    }

            //                                    break;
            //                                }

            //                            case 1:
            //                                {
            //                                    str_result = Term(GetValueAsString(@params[1], is_term[1]), u: null);
            //                                    break;
            //                                }
            //                        }

            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //                case "textheight":
            //                    {
            //                        num_result = GUI.MainForm.picMain(0).TextHeight(GetValueAsString(@params[1], is_term[1]));
            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "textwidth":
            //                    {
            //                        num_result = GUI.MainForm.picMain(0).TextWidth(GetValueAsString(@params[1], is_term[1]));
            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "trim":
            //                    {
            //                        str_result = Strings.Trim(GetValueAsString(@params[1], is_term[1]));
            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //                case "unit":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined31() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        Unit localItem218() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

            //                                        str_result = localItem218().Name;
            //                                    }
            //                                    else if (localIsDefined31())
            //                                    {
            //                                        {
            //                                            var withBlock43 = SRC.PList.Item((object)pname);
            //                                            if (withBlock43.Unit is object)
            //                                            {
            //                                                str_result = withBlock43.Unit.Name;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        str_result = Event.SelectedUnitForEvent.Name;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //                case "unitid":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    pname = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined32() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.UList.IsDefined2((object)pname))
            //                                    {
            //                                        Unit localItem219() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

            //                                        str_result = localItem219().ID;
            //                                    }
            //                                    else if (localIsDefined32())
            //                                    {
            //                                        {
            //                                            var withBlock44 = SRC.PList.Item((object)pname);
            //                                            if (withBlock44.Unit is object)
            //                                            {
            //                                                str_result = withBlock44.Unit.ID;
            //                                            }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (Event.SelectedUnitForEvent is object)
            //                                    {
            //                                        str_result = Event.SelectedUnitForEvent.ID;
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //case "x":
            //    return this.X(etype, @params, pcount, is_term, out str_result, out num_result);

            //case "y":
            //    return this.Y(etype, @params, pcount, is_term, out str_result, out num_result);

            //                // ADD START 240a
            //                case "windowwidth":
            //                    {
            //                        if (etype == ValueType.NumericType)
            //                        {
            //                            num_result = (double)GUI.MainPWidth;
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }
            //                        else if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GUI.MainPWidth.ToString();
            //                            CallFunctionRet = ValueType.StringType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "windowheight":
            //                    {
            //                        if (etype == ValueType.NumericType)
            //                        {
            //                            num_result = (double)GUI.MainPHeight;
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }
            //                        else if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GUI.MainPHeight.ToString();
            //                            CallFunctionRet = ValueType.StringType;
            //                        }

            //                        return CallFunctionRet;
            //                    }
            //                // ADD  END  240a

            //                case "wide":
            //                    {
            //                        str_result = Strings.StrConv(GetValueAsString(@params[1], is_term[1]), VbStrConv.Wide);
            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //                // Date型の処理
            //                case "year":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    buf = GetValueAsString(@params[1], is_term[1]);
            //                                    if (Information.IsDate(buf))
            //                                    {
            //                                        num_result = (double)DateAndTime.Year(Conversions.ToDate(buf));
            //                                    }
            //                                    else
            //                                    {
            //                                        num_result = 0d;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    num_result = (double)DateAndTime.Year(DateAndTime.Now);
            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "month":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    buf = GetValueAsString(@params[1], is_term[1]);
            //                                    if (Information.IsDate(buf))
            //                                    {
            //                                        num_result = (double)DateAndTime.Month(Conversions.ToDate(buf));
            //                                    }
            //                                    else
            //                                    {
            //                                        num_result = 0d;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    num_result = (double)DateAndTime.Month(DateAndTime.Now);
            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "weekday":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    buf = GetValueAsString(@params[1], is_term[1]);
            //                                    if (Information.IsDate(buf))
            //                                    {
            //                                        switch (DateAndTime.Weekday(Conversions.ToDate(buf)))
            //                                        {
            //                                            case FirstDayOfWeek.Sunday:
            //                                                {
            //                                                    str_result = "日曜";
            //                                                    break;
            //                                                }

            //                                            case FirstDayOfWeek.Monday:
            //                                                {
            //                                                    str_result = "月曜";
            //                                                    break;
            //                                                }

            //                                            case FirstDayOfWeek.Tuesday:
            //                                                {
            //                                                    str_result = "火曜";
            //                                                    break;
            //                                                }

            //                                            case FirstDayOfWeek.Wednesday:
            //                                                {
            //                                                    str_result = "水曜";
            //                                                    break;
            //                                                }

            //                                            case FirstDayOfWeek.Thursday:
            //                                                {
            //                                                    str_result = "木曜";
            //                                                    break;
            //                                                }

            //                                            case FirstDayOfWeek.Friday:
            //                                                {
            //                                                    str_result = "金曜";
            //                                                    break;
            //                                                }

            //                                            case FirstDayOfWeek.Saturday:
            //                                                {
            //                                                    str_result = "土曜";
            //                                                    break;
            //                                                }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    switch (DateAndTime.Weekday(DateAndTime.Now))
            //                                    {
            //                                        case FirstDayOfWeek.Sunday:
            //                                            {
            //                                                str_result = "日曜";
            //                                                break;
            //                                            }

            //                                        case FirstDayOfWeek.Monday:
            //                                            {
            //                                                str_result = "月曜";
            //                                                break;
            //                                            }

            //                                        case FirstDayOfWeek.Tuesday:
            //                                            {
            //                                                str_result = "火曜";
            //                                                break;
            //                                            }

            //                                        case FirstDayOfWeek.Wednesday:
            //                                            {
            //                                                str_result = "水曜";
            //                                                break;
            //                                            }

            //                                        case FirstDayOfWeek.Thursday:
            //                                            {
            //                                                str_result = "木曜";
            //                                                break;
            //                                            }

            //                                        case FirstDayOfWeek.Friday:
            //                                            {
            //                                                str_result = "金曜";
            //                                                break;
            //                                            }

            //                                        case FirstDayOfWeek.Saturday:
            //                                            {
            //                                                str_result = "土曜";
            //                                                break;
            //                                            }
            //                                    }

            //                                    break;
            //                                }
            //                        }

            //                        CallFunctionRet = ValueType.StringType;
            //                        return CallFunctionRet;
            //                    }

            //                case "day":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    buf = GetValueAsString(@params[1], is_term[1]);
            //                                    if (Information.IsDate(buf))
            //                                    {
            //                                        num_result = (double)DateAndTime.Day(Conversions.ToDate(buf));
            //                                    }
            //                                    else
            //                                    {
            //                                        num_result = 0d;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    num_result = (double)DateAndTime.Day(DateAndTime.Now);
            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "hour":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    buf = GetValueAsString(@params[1], is_term[1]);
            //                                    if (Information.IsDate(buf))
            //                                    {
            //                                        num_result = (double)DateAndTime.Hour(Conversions.ToDate(buf));
            //                                    }
            //                                    else
            //                                    {
            //                                        num_result = 0d;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    num_result = (double)DateAndTime.Hour(DateAndTime.Now);
            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "minute":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    buf = GetValueAsString(@params[1], is_term[1]);
            //                                    if (Information.IsDate(buf))
            //                                    {
            //                                        num_result = (double)DateAndTime.Minute(Conversions.ToDate(buf));
            //                                    }
            //                                    else
            //                                    {
            //                                        num_result = 0d;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    num_result = (double)DateAndTime.Minute(DateAndTime.Now);
            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "second":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    buf = GetValueAsString(@params[1], is_term[1]);
            //                                    if (Information.IsDate(buf))
            //                                    {
            //                                        num_result = (double)DateAndTime.Second(Conversions.ToDate(buf));
            //                                    }
            //                                    else
            //                                    {
            //                                        num_result = 0d;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    num_result = (double)DateAndTime.Second(DateAndTime.Now);
            //                                    break;
            //                                }
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "difftime":
            //                    {
            //                        if (pcount == 2)
            //                        {
            //                            if (@params[1] == "Now")
            //                            {
            //                                d1 = DateAndTime.Now;
            //                            }
            //                            else
            //                            {
            //                                buf = GetValueAsString(@params[1], is_term[1]);
            //                                if (!Information.IsDate(buf))
            //                                {
            //                                    return CallFunctionRet;
            //                                }

            //                                d1 = Conversions.ToDate(buf);
            //                            }

            //                            if (@params[2] == "Now")
            //                            {
            //                                d2 = DateAndTime.Now;
            //                            }
            //                            else
            //                            {
            //                                buf = GetValueAsString(@params[2], is_term[2]);
            //                                if (!Information.IsDate(buf))
            //                                {
            //                                    return CallFunctionRet;
            //                                }

            //                                d2 = Conversions.ToDate(buf);
            //                            }

            //                            num_result = (double)DateAndTime.Second(DateTime.FromOADate(d2.ToOADate() - d1.ToOADate()));
            //                        }

            //                        if (etype == ValueType.StringType)
            //                        {
            //                            str_result = GeneralLib.FormatNum(num_result);
            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            CallFunctionRet = ValueType.NumericType;
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                // ダイアログ表示
            //                case "loadfiledialog":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    str_result = FileDialog.LoadFileDialog("ファイルを開く", SRC.ScenarioPath, "", 2, GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]), ftype2: GetValueAsString(@params[1], is_term[1])2, fsuffix2: GetValueAsString(@params[2], is_term[2])2, ftype3: GetValueAsString(@params[1], is_term[1])3, fsuffix3: GetValueAsString(@params[2], is_term[2])3);
            //                                    break;
            //                                }

            //                            case 3:
            //                                {
            //                                    str_result = FileDialog.LoadFileDialog("ファイルを開く", SRC.ScenarioPath, GetValueAsString(@params[3], is_term[3]), 2, GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]), ftype2: ""1, fsuffix2: ""1, ftype3: ""1, fsuffix3: ""1);
            //                                    break;
            //                                }

            //                            case 4:
            //                                {
            //                                    str_result = FileDialog.LoadFileDialog("ファイルを開く", SRC.ScenarioPath + GetValueAsString(@params[4], is_term[4]), GetValueAsString(@params[3], is_term[3]), 2, GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]), ftype2: "", fsuffix2: "", ftype3: "", fsuffix3: "");
            //                                    break;
            //                                }
            //                        }

            //                        CallFunctionRet = ValueType.StringType;

            //                        // 本当はこれだけでいいはずだけど……
            //                        if (Strings.InStr(str_result, SRC.ScenarioPath) > 0)
            //                        {
            //                            str_result = Strings.Mid(str_result, Strings.Len(SRC.ScenarioPath) + 1);
            //                            return CallFunctionRet;
            //                        }

            //                        // フルパス指定ならここで終了
            //                        if (Strings.Right(Strings.Left(str_result, 3), 2) == @":\")
            //                        {
            //                            str_result = "";
            //                            return CallFunctionRet;
            //                        }

            //                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                        while (string.IsNullOrEmpty(FileSystem.Dir(SRC.ScenarioPath + str_result, FileAttribute.Normal)))
            //                        {
            //                            if (Strings.InStr(str_result, @"\") == 0)
            //                            {
            //                                // シナリオフォルダ外のファイルだった
            //                                str_result = "";
            //                                return CallFunctionRet;
            //                            }

            //                            str_result = Strings.Mid(str_result, Strings.InStr(str_result, @"\") + 1);
            //                        }

            //                        return CallFunctionRet;
            //                    }

            //                case "savefiledialog":
            //                    {
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    str_result = FileDialog.SaveFileDialog("ファイルを保存", SRC.ScenarioPath, "", 2, GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]), ftype2: "", fsuffix2: "", ftype3: "", fsuffix3: "");
            //                                    break;
            //                                }

            //                            case 3:
            //                                {
            //                                    str_result = FileDialog.SaveFileDialog("ファイルを保存", SRC.ScenarioPath, GetValueAsString(@params[3], is_term[3]), 2, GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]), ftype2: "", fsuffix2: "", ftype3: "", fsuffix3: "");
            //                                    break;
            //                                }

            //                            case 4:
            //                                {
            //                                    str_result = FileDialog.SaveFileDialog("ファイルを保存", SRC.ScenarioPath + GetValueAsString(@params[4], is_term[4]), GetValueAsString(@params[3], is_term[3]), 2, GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]), ftype2: "", fsuffix2: "", ftype3: "", fsuffix3: "");
            //                                    break;
            //                                }
            //                        }

            //                        CallFunctionRet = ValueType.StringType;

            //                        // 本当はこれだけでいいはずだけど……
            //                        if (Strings.InStr(str_result, SRC.ScenarioPath) > 0)
            //                        {
            //                            str_result = Strings.Mid(str_result, Strings.Len(SRC.ScenarioPath) + 1);
            //                            return CallFunctionRet;
            //                        }

            //                        if (Strings.InStr(str_result, @"\") == 0)
            //                        {
            //                            return CallFunctionRet;
            //                        }

            //                        var loopTo13 = Strings.Len(str_result);
            //                        for (i = 1; i <= loopTo13; i++)
            //                        {
            //                            if (Strings.Mid(str_result, Strings.Len(str_result) - i + 1, 1) == @"\")
            //                            {
            //                                break;
            //                            }
            //                        }

            //                        buf = Strings.Left(str_result, Strings.Len(str_result) - i);
            //                        str_result = Strings.Mid(str_result, Strings.Len(str_result) - i + 2);
            //                        while (Strings.InStr(buf, @"\") > 0)
            //                        {
            //                            buf = Strings.Mid(buf, Strings.InStr(buf, @"\") + 1);
            //                            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                            if (!string.IsNullOrEmpty(FileSystem.Dir(SRC.ScenarioPath + buf, FileAttribute.Directory)))
            //                            {
            //                                str_result = buf + @"\" + str_result;
            //                                return CallFunctionRet;
            //                            }
            //                        }

            //                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                        if (!string.IsNullOrEmpty(FileSystem.Dir(SRC.ScenarioPath + buf, FileAttribute.Directory)))
            //                        {
            //                            str_result = buf + @"\" + str_result;
            //                        }

            //                        return CallFunctionRet;
            //                    }
            //}
            return ValueType.UndefinedType;
        }

        private ValueType CallUserFunction(ValueType etype, int labelId, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            // TODO CallCmdと合わせる
            str_result = "";
            num_result = 0d;

            // 関数が見つかった
            var ret = labelId + 1;

            // 呼び出し階層をチェック
            if (Event.CallDepth > Event.MaxCallDepth)
            {
                Event.CallDepth = Event.MaxCallDepth;
                Event.DisplayEventErrorMessage(Event.CurrentLineNum, GeneralLib.FormatNum(Event.MaxCallDepth) + "階層を越えるサブルーチンの呼び出しは出来ません");
                return ValueType.UndefinedType;
            }

            // 引数用スタックが溢れないかチェック
            if ((Event.ArgIndex + pcount) > Event.MaxArgIndex)
            {
                Event.DisplayEventErrorMessage(Event.CurrentLineNum, "サブルーチンの引数の総数が" + GeneralLib.FormatNum(Event.MaxArgIndex) + "個を超えています");
                return ValueType.UndefinedType;
            }

            // 引数の値を先に求めておく
            // (スタックに積みながら計算すると、引数での関数呼び出しで不正になる)
            var loopTo14 = pcount;
            for (var i = 1; i <= loopTo14; i++)
                @params[i] = GetValueAsString(@params[i], is_term[i]);

            // 現在の状態を保存
            Event.CallStack[Event.CallDepth] = Event.CurrentLineNum;
            Event.ArgIndexStack[Event.CallDepth] = Event.ArgIndex;
            Event.VarIndexStack[Event.CallDepth] = Event.VarIndex;
            Event.ForIndexStack[Event.CallDepth] = Event.ForIndex;
            Event.UpVarLevelStack[Event.CallDepth] = Event.UpVarLevel;

            // UpVarの階層数を初期化
            Event.UpVarLevel = 0;

            // 呼び出し階層数をインクリメント
            Event.CallDepth = (Event.CallDepth + 1);
            var cur_depth = Event.CallDepth;

            // 引数をスタックに積む
            Event.ArgIndex = (Event.ArgIndex + pcount);
            var loopTo15 = pcount;
            for (var i = 1; i <= loopTo15; i++)
                Event.ArgStack[Event.ArgIndex - i + 1] = @params[i];

            // サブルーチン本体を実行
            do
            {
                Event.CurrentLineNum = ret;
                if (Event.CurrentLineNum >= Event.EventCmd.Count)
                {
                    break;
                }

                {
                    var withBlock49 = Event.EventCmd[Event.CurrentLineNum];
                    if (cur_depth == Event.CallDepth & withBlock49.Name == CmdType.ReturnCmd)
                    {
                        break;
                    }

                    ret = withBlock49.Exec();
                }
            }
            while (ret >= 0);

            // 返り値
            {
                var withBlock50 = Event.EventCmd[Event.CurrentLineNum];
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
            Event.CallDepth = (Event.CallDepth - 1);

            // サブルーチン実行前の状態に復帰
            Event.CurrentLineNum = Event.CallStack[Event.CallDepth];
            Event.ArgIndex = Event.ArgIndexStack[Event.CallDepth];
            Event.VarIndex = Event.VarIndexStack[Event.CallDepth];
            Event.ForIndex = Event.ForIndexStack[Event.CallDepth];
            Event.UpVarLevel = Event.UpVarLevelStack[Event.CallDepth];
            if (etype == ValueType.NumericType)
            {
                num_result = GeneralLib.StrToDbl(str_result);
                return ValueType.NumericType;
            }
            else
            {
                return ValueType.StringType;
            }
        }
    }
}
