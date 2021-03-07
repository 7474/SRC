// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Events;
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;
using System;

namespace SRCCore.Expressions
{
    public partial class Expression
    {
        // === 関数に関する処理 ===

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
                        goto LookUpUserDefinedID;
                        break;
                    }
            }

            // システム関数？
            var PT = default(GUI.POINTAPI);
            var in_window = default(bool);
            int x2, x1, y1, y2;
            DateTime d1, d2;
            var list = default(string[]);
            bool flag;
            switch (Strings.LCase(fname) ?? "")
            {
                // 多用される関数を先に判定
                case "args":
                    {
                        // UpVarコマンドの呼び出し回数を累計
                        num = Event.UpVarLevel;
                        i = Event.CallDepth;
                        while (num > 0)
                        {
                            i = (i - num);
                            if (i < 1)
                            {
                                i = 1;
                                break;
                            }

                            num = Event.UpVarLevelStack[i];
                        }

                        if (i < 1)
                        {
                            i = 1;
                        }

                        // 引数の範囲内に納まっているかチェック
                        num = GetValueAsLong(@params[1], is_term[1]);
                        if (num <= (Event.ArgIndex - Event.ArgIndexStack[i - 1]))
                        {
                            str_result = Event.ArgStack[Event.ArgIndex - num + 1];
                        }

                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(str_result);
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
                        ret = Event.FindNormalLabel(@params[1]);
                        if (ret == 0)
                        {
                            // 式で指定されている？
                            string arglname = GetValueAsString(@params[1], is_term[1]);
                            ret = Event.FindNormalLabel(arglname);
                            if (ret == 0)
                            {
                                Event.DisplayEventErrorMessage(Event.CurrentLineNum, "指定されたサブルーチン「" + @params[1] + "」が見つかりません");
                                return CallFunctionRet;
                            }
                        }

                        ret = ret + 1;

                        // 呼び出し階層をチェック
                        if (Event.CallDepth > Event.MaxCallDepth)
                        {
                            Event.CallDepth = Event.MaxCallDepth;
                            Event.DisplayEventErrorMessage(Event.CurrentLineNum, GeneralLib.FormatNum((double)Event.MaxCallDepth) + "階層を越えるサブルーチンの呼び出しは出来ません");
                            return CallFunctionRet;
                        }

                        // 引数用スタックが溢れないかチェック
                        if ((Event.ArgIndex + pcount) > Event.MaxArgIndex)
                        {
                            Event.DisplayEventErrorMessage(Event.CurrentLineNum, "サブルーチンの引数の総数が" + GeneralLib.FormatNum((double)Event.MaxArgIndex) + "個を超えています");
                            return CallFunctionRet;
                        }

                        // 引数を評価しておく
                        var loopTo1 = pcount;
                        for (i = 2; i <= loopTo1; i++)
                            @params[i] = GetValueAsString(@params[i], is_term[i]);

                        // 現在の状態を保存
                        Event.CallStack[Event.CallDepth] = Event.CurrentLineNum;
                        Event.ArgIndexStack[Event.CallDepth] = Event.ArgIndex;
                        Event.VarIndexStack[Event.CallDepth] = Event.VarIndex;
                        Event.ForIndexStack[Event.CallDepth] = Event.ForIndex;

                        // UpVarが実行された場合、UpVar実行数は累計する
                        if (Event.UpVarLevel > 0)
                        {
                            Event.UpVarLevelStack[Event.CallDepth] = (Event.UpVarLevel + Event.UpVarLevelStack[Event.CallDepth - 1]);
                        }
                        else
                        {
                            Event.UpVarLevelStack[Event.CallDepth] = 0;
                        }

                        // UpVarの階層数を初期化
                        Event.UpVarLevel = 0;

                        // 呼び出し階層数をインクリメント
                        Event.CallDepth = (Event.CallDepth + 1);
                        cur_depth = Event.CallDepth;

                        // 引数をスタックに積む
                        Event.ArgIndex = (Event.ArgIndex + pcount - 1);
                        var loopTo2 = pcount;
                        for (i = 2; i <= loopTo2; i++)
                            Event.ArgStack[Event.ArgIndex - i + 2] = @params[i];

                        // サブルーチン本体を実行
                        do
                        {
                            Event.CurrentLineNum = ret;
                            if (Event.CurrentLineNum > Information.UBound(Event.EventCmd))
                            {
                                break;
                            }

                            {
                                var withBlock = Event.EventCmd[Event.CurrentLineNum];
                                if (cur_depth == Event.CallDepth & withBlock.Name == Event.CmdType.ReturnCmd)
                                {
                                    break;
                                }

                                ret = withBlock.Exec();
                            }
                        }
                        while (ret > 0);

                        // 返り値
                        {
                            var withBlock1 = Event.EventCmd[Event.CurrentLineNum];
                            if (withBlock1.ArgNum == 2)
                            {
                                str_result = withBlock1.GetArgAsString(2);
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
                        for (i = 1; i <= loopTo3; i++)
                            @params[i] = GetValueAsString(@params[i], is_term[i]);
                        str_result = EvalInfoFunc(@params);
                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(str_result);
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
                        if (pcount == 2)
                        {
                            i = Strings.InStr(GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]));
                        }
                        else
                        {
                            // params(3)が指定されている場合は、それを検索開始位置似設定
                            // VBのInStrは引数1が開始位置になりますが、現仕様との兼ね合いを考え、
                            // eve上では引数3に設定するようにしています
                            i = Strings.InStr(GetValueAsLong(@params[3], is_term[3]), GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]));
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
                        if (pcount == 2)
                        {
                            // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                            // UPGRADE_ISSUE: InStrB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                            i = InStrB(Strings.StrConv(GetValueAsString(@params[1], is_term[1]), vbFromUnicode), Strings.StrConv(GetValueAsString(@params[2], is_term[2]), vbFromUnicode));
                        }
                        else
                        {
                            // params(3)が指定されている場合は、それを検索開始位置似設定
                            // VBのInStrは引数1が開始位置になりますが、現仕様との兼ね合いを考え、
                            // eve上では引数3に設定するようにしています
                            // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                            // UPGRADE_ISSUE: InStrB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                            i = InStrB(GetValueAsLong(@params[3], is_term[3]), Strings.StrConv(GetValueAsString(@params[1], is_term[1]), vbFromUnicode), Strings.StrConv(GetValueAsString(@params[2], is_term[2]), vbFromUnicode));
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
                        string arglist = GetValueAsString(@params[1], is_term[1]);
                        str_result = GeneralLib.ListIndex(arglist, GetValueAsLong(@params[2], is_term[2]));

                        // 全体が()で囲まれている場合は()を外す
                        if (Strings.Left(str_result, 1) == "(" & Strings.Right(str_result, 1) == ")")
                        {
                            str_result = Strings.Mid(str_result, 2, Strings.Len(str_result) - 2);
                        }

                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(str_result);
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
                        string arglist1 = GetValueAsString(@params[1], is_term[1]);
                        i = GeneralLib.ListLength(arglist1);
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
                        str_result = GetValueAsString(@params[1], is_term[1]);
                        var loopTo4 = pcount;
                        for (i = 2; i <= loopTo4; i++)
                            str_result = str_result + " " + GetValueAsString(@params[i], is_term[i]);
                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                // これ以降はアルファベット順
                case "abs":
                    {
                        num_result = Math.Abs(GetValueAsDouble(@params[1], is_term[1]));
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    object argIndex6 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex6))
                                    {
                                        Unit localItem2() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        num_result = (double)localItem2().Action;
                                    }
                                    else if (localIsDefined())
                                    {
                                        object argIndex5 = (object)pname;
                                        {
                                            var withBlock2 = SRC.PList.Item(argIndex5);
                                            if (withBlock2.Unit is object)
                                            {
                                                {
                                                    var withBlock3 = withBlock2.Unit;
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
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        num_result = (double)Event.SelectedUnitForEvent.Action;
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined1() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    object argIndex8 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex8))
                                    {
                                        Unit localItem21() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        str_result = localItem21().Area;
                                    }
                                    else if (localIsDefined1())
                                    {
                                        object argIndex7 = (object)pname;
                                        {
                                            var withBlock4 = SRC.PList.Item(argIndex7);
                                            if (withBlock4.Unit is object)
                                            {
                                                str_result = withBlock4.Unit.Area;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        str_result = Event.SelectedUnitForEvent.Area;
                                    }

                                    break;
                                }
                        }

                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "asc":
                    {
                        num_result = (double)Strings.Asc(GetValueAsString(@params[1], is_term[1]));
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
                        num_result = Math.Atan(GetValueAsDouble(@params[1], is_term[1]));
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
                        str_result = Conversions.ToString((char)GetValueAsLong(@params[1], is_term[1]));
                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "condition":
                    {
                        switch (pcount)
                        {
                            case 2:
                                {
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    buf = GetValueAsString(@params[2], is_term[2]);
                                    bool localIsDefined2() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    object argIndex12 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex12))
                                    {
                                        Unit localItem22() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        object argIndex9 = (object)buf;
                                        if (localItem22().IsConditionSatisfied(argIndex9))
                                        {
                                            num_result = 1d;
                                        }
                                    }
                                    else if (localIsDefined2())
                                    {
                                        object argIndex11 = (object)pname;
                                        {
                                            var withBlock5 = SRC.PList.Item(argIndex11);
                                            if (withBlock5.Unit is object)
                                            {
                                                object argIndex10 = (object)buf;
                                                if (withBlock5.Unit.IsConditionSatisfied(argIndex10))
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
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        buf = GetValueAsString(@params[1], is_term[1]);
                                        object argIndex13 = (object)buf;
                                        if (Event.SelectedUnitForEvent.IsConditionSatisfied(argIndex13))
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
                        num = 0;

                        // サブルーチンローカル変数を検索
                        if (Event.CallDepth > 0)
                        {
                            var loopTo5 = Event.VarIndex;
                            for (i = (Event.VarIndexStack[Event.CallDepth - 1] + 1); i <= loopTo5; i++)
                            {
                                if (Strings.InStr(Event.VarStack[i].Name, buf) == 1)
                                {
                                    num = (num + 1);
                                }
                            }

                            if (num > 0)
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
                        foreach (VarData currentVar in Event.LocalVariableList)
                        {
                            var = currentVar;
                            if (Strings.InStr(var.Name, buf) == 1)
                            {
                                num = (num + 1);
                            }
                        }

                        if (num > 0)
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
                        foreach (VarData currentVar1 in Event.GlobalVariableList)
                        {
                            var = currentVar1;
                            if (Strings.InStr(var.Name, buf) == 1)
                            {
                                num = (num + 1);
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined3() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    object argIndex15 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex15))
                                    {
                                        Unit localItem23() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        num = localItem23().CountItem();
                                    }
                                    else if (!localIsDefined3())
                                    {
                                        if (pname == "未装備")
                                        {
                                            num = 0;
                                            foreach (Item currentIt in SRC.IList)
                                            {
                                                it = currentIt;
                                                if (it.Unit is null & it.Exist)
                                                {
                                                    num = (num + 1);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        object argIndex14 = (object)pname;
                                        {
                                            var withBlock6 = SRC.PList.Item(argIndex14);
                                            if (withBlock6.Unit is object)
                                            {
                                                num = withBlock6.Unit.CountItem();
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        num = Event.SelectedUnitForEvent.CountItem();
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    object argIndex18 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex18))
                                    {
                                        object argIndex16 = (object)pname;
                                        {
                                            var withBlock7 = SRC.UList.Item2(argIndex16);
                                            num_result = (double)(withBlock7.CountPilot() + withBlock7.CountSupport());
                                        }
                                    }
                                    else
                                    {
                                        object argIndex17 = (object)pname;
                                        {
                                            var withBlock8 = SRC.PList.Item(argIndex17);
                                            if (withBlock8.Unit is object)
                                            {
                                                {
                                                    var withBlock9 = withBlock8.Unit;
                                                    num_result = (double)(withBlock9.CountPilot() + withBlock9.CountSupport());
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        {
                                            var withBlock10 = Event.SelectedUnitForEvent;
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
                        num_result = Math.Cos(GetValueAsDouble(@params[1], is_term[1]));
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined4() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    Pilot localItem1() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

                                    object argIndex20 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex20))
                                    {
                                        object argIndex19 = (object)pname;
                                        {
                                            var withBlock11 = SRC.UList.Item2(argIndex19);
                                            num_result = (double)(100 * (withBlock11.MaxHP - withBlock11.HP) / withBlock11.MaxHP);
                                        }
                                    }
                                    else if (!localIsDefined4())
                                    {
                                        num_result = 100d;
                                    }
                                    else if (localItem1().Unit is null)
                                    {
                                        num_result = 100d;
                                    }
                                    else
                                    {
                                        Pilot localItem() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

                                        {
                                            var withBlock12 = localItem().Unit;
                                            num_result = (double)(100 * (withBlock12.MaxHP - withBlock12.HP) / withBlock12.MaxHP);
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    {
                                        var withBlock13 = Event.SelectedUnitForEvent;
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
                                    fname = GetValueAsString(@params[1], is_term[1]);

                                    // フルパス指定でなければシナリオフォルダを起点に検索
                                    if (Strings.Mid(fname, 2, 1) != ":")
                                    {
                                        fname = SRC.ScenarioPath + fname;
                                    }

                                    switch (GetValueAsString(@params[2], is_term[2]) ?? "")
                                    {
                                        case "ファイル":
                                            {
                                                // UPGRADE_ISSUE: vbNormal をアップグレードする定数を決定できません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3B44E51-B5F1-4FD7-AA29-CAD31B71F487"' をクリックしてください。
                                                num = Constants.vbNormal;
                                                break;
                                            }

                                        case "フォルダ":
                                            {
                                                num = FileAttribute.Directory;
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
                                    if (num == FileAttribute.Directory)
                                    {
                                        string argstr2 = @"\";
                                        i = GeneralLib.InStr2(fname, argstr2);
                                        if (i > 0)
                                        {
                                            dir_path = Strings.Left(fname, i);
                                        }
                                    }

                                    // 単一ファイルの検索？
                                    if (Strings.InStr(fname, "*") == 0)
                                    {
                                        // フォルダの検索の場合は見つかったファイルがフォルダ
                                        // かどうかチェックする
                                        if (num == FileAttribute.Directory)
                                        {
                                            if ((FileSystem.GetAttr(dir_path + str_result) & num) == 0)
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
                                    if (num == FileAttribute.Directory)
                                    {
                                        while (Strings.Len(str_result) > 0)
                                        {
                                            // フォルダの検索の場合は見つかったファイルがフォルダ
                                            // かどうかチェックする
                                            if ((FileSystem.GetAttr(dir_path + str_result) & num) != 0)
                                            {
                                                Array.Resize(dir_list, Information.UBound(dir_list) + 1 + 1);
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
                                            Array.Resize(dir_list, Information.UBound(dir_list) + 1 + 1);
                                            dir_list[Information.UBound(dir_list)] = str_result;
                                            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                                            str_result = FileSystem.Dir();
                                        }
                                    }

                                    if (Information.UBound(dir_list) > 0)
                                    {
                                        str_result = dir_list[1];
                                        dir_index = 2;
                                    }
                                    else
                                    {
                                        str_result = "";
                                        dir_index = 1;
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    fname = GetValueAsString(@params[1], is_term[1]);

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
                                        Array.Resize(dir_list, Information.UBound(dir_list) + 1 + 1);
                                        dir_list[Information.UBound(dir_list)] = str_result;
                                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                                        str_result = FileSystem.Dir();
                                    }

                                    if (Information.UBound(dir_list) > 0)
                                    {
                                        str_result = dir_list[1];
                                        dir_index = 2;
                                    }
                                    else
                                    {
                                        str_result = "";
                                        dir_index = 1;
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (dir_index <= Information.UBound(dir_list))
                                    {
                                        str_result = dir_list[dir_index];
                                        dir_index = (dir_index + 1);
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
                            if (FileSystem.EOF(GetValueAsLong(@params[1], is_term[1])))
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
                            if (FileSystem.EOF(GetValueAsLong(@params[1], is_term[1])))
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined5() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    object argIndex22 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex22))
                                    {
                                        Unit localItem24() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        num_result = (double)localItem24().EN;
                                    }
                                    else if (localIsDefined5())
                                    {
                                        object argIndex21 = (object)pname;
                                        {
                                            var withBlock14 = SRC.PList.Item(argIndex21);
                                            if (withBlock14.Unit is object)
                                            {
                                                num_result = (double)withBlock14.Unit.EN;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        num_result = (double)Event.SelectedUnitForEvent.EN;
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
                        buf = Strings.Trim(GetValueAsString(@params[1], is_term[1]));
                        CallFunctionRet = EvalExpr(buf, etype, str_result, num_result);
                        return CallFunctionRet;
                    }

                case "font":
                    {
                        switch (GetValueAsString(@params[1], is_term[1]) ?? "")
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
                                    var loopTo6 = (6 - Strings.Len(str_result));
                                    for (i = 1; i <= loopTo6; i++)
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
                        str_result = SrcFormatter.Format(GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]));
                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(str_result);
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
                        if (pcount != 1)
                        {
                            return CallFunctionRet;
                        }

                        // キー番号
                        i = GetValueAsLong(@params[1], is_term[1]);

                        // 左利き設定に対応
                        switch (i)
                        {
                            case Keys.LButton:
                                {
                                    i = GUI.LButtonID;
                                    break;
                                }

                            case Keys.RButton:
                                {
                                    i = GUI.RButtonID;
                                    break;
                                }
                        }

                        if (i == Keys.LButton | i == Keys.RButton)
                        {
                            // マウスカーソルの位置を参照
                            GUI.GetCursorPos(PT);

                            // メインウインドウ上でマウスボタンを押している？
                            if (ReferenceEquals(Form.ActiveForm, GUI.MainForm))
                            {
                                {
                                    var withBlock15 = GUI.MainForm;
                                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    x1 = (long)SrcFormatter.PixelsToTwipsX((double)withBlock15.Left) / (long)SrcFormatter.TwipsPerPixelX() + withBlock15.picMain(0).Left + 3;
                                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    y1 = (long)SrcFormatter.PixelsToTwipsY((double)withBlock15.Top) / (long)SrcFormatter.TwipsPerPixelY() + withBlock15.picMain(0).Top + 28;
                                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    x2 = x1 + withBlock15.picMain(0).Width;
                                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                                    y2 = y1 + withBlock15.picMain(0).Height;
                                }

                                if (x1 <= PT.X & PT.X <= x2 & y1 <= PT.Y & PT.Y <= y2)
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
                        if (Conversions.ToBoolean(GUI.GetAsyncKeyState(i) & 0x8000))
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined6() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    object argIndex24 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex24))
                                    {
                                        Unit localItem25() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        num_result = (double)localItem25().HP;
                                    }
                                    else if (localIsDefined6())
                                    {
                                        object argIndex23 = (object)pname;
                                        {
                                            var withBlock16 = SRC.PList.Item(argIndex23);
                                            if (withBlock16.Unit is object)
                                            {
                                                num_result = (double)withBlock16.Unit.HP;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        num_result = (double)Event.SelectedUnitForEvent.HP;
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
                        num = GeneralLib.ListSplit(@params[1], list);
                        switch (num)
                        {
                            case 1:
                                {
                                    var tmp1 = list;
                                    object argIndex26 = (object)tmp1[1];
                                    if (SRC.PList.IsDefined(argIndex26))
                                    {
                                        var tmp = list;
                                        object argIndex25 = (object)tmp[1];
                                        {
                                            var withBlock17 = SRC.PList.Item(argIndex25);
                                            if (withBlock17.Unit is null)
                                            {
                                                flag = false;
                                            }
                                            else
                                            {
                                                {
                                                    var withBlock18 = withBlock17.Unit;
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
                                    else if (GetValueAsLong(@params[1]) != 0)
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
                                    pname = GeneralLib.ListIndex(expr, 2);
                                    var tmp3 = list;
                                    object argIndex28 = (object)tmp3[2];
                                    if (SRC.PList.IsDefined(argIndex28))
                                    {
                                        var tmp2 = list;
                                        object argIndex27 = (object)tmp2[2];
                                        {
                                            var withBlock19 = SRC.PList.Item(argIndex27);
                                            if (withBlock19.Unit is null)
                                            {
                                                flag = true;
                                            }
                                            else
                                            {
                                                {
                                                    var withBlock20 = withBlock19.Unit;
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
                                    else if (GetValueAsLong(@params[1], true) == 0)
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
                                    if (GetValueAsLong(@params[1]) != 0)
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
                            str_result = GetValueAsString(@params[2], is_term[2]);
                        }
                        else
                        {
                            str_result = GetValueAsString(@params[3], is_term[3]);
                        }

                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(str_result);
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
                        buf = GetValueAsString(@params[1], is_term[1]);
                        buf2 = GetValueAsString(@params[2], is_term[2]);
                        if (Strings.Len(buf2) > 0 & Strings.Len(buf) >= Strings.Len(buf2))
                        {
                            if (pcount == 2)
                            {
                                num = Strings.Len(buf);
                            }
                            else
                            {
                                num = GetValueAsLong(@params[3], is_term[3]);
                            }

                            i = (num - Strings.Len(buf2) + 1);
                            do
                            {
                                j = Strings.InStr(i, buf, buf2);
                                if (i == j)
                                {
                                    break;
                                }

                                i = (i - 1);
                            }
                            while (i != 0);
                        }
                        else
                        {
                            i = 0;
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
                        buf = GetValueAsString(@params[1], is_term[1]);
                        buf2 = GetValueAsString(@params[2], is_term[2]);

                        // UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                        if (LenB(buf2) > 0 & LenB(buf) >= LenB(buf2))
                        {
                            if (pcount == 2)
                            {
                                // UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                                num = LenB(buf);
                            }
                            else
                            {
                                num = GetValueAsLong(@params[3], is_term[3]);
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

                                i = (i - 1);
                            }
                            while (i != 0);
                        }
                        else
                        {
                            i = 0;
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
                        num_result = Conversion.Int(GetValueAsDouble(@params[1], is_term[1]));
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    buf = GetValueAsString(@params[2], is_term[2]);

                                    // エリアスが定義されている？
                                    object argIndex30 = (object)buf;
                                    if (SRC.ALDList.IsDefined(argIndex30))
                                    {
                                        object argIndex29 = (object)buf;
                                        {
                                            var withBlock21 = SRC.ALDList.Item(argIndex29);
                                            var loopTo7 = withBlock21.Count;
                                            for (i = 1; i <= loopTo7; i++)
                                            {
                                                string localLIndex() { string arglist = withBlock21.get_AliasData(i); var ret = GeneralLib.LIndex(arglist, 1); withBlock21.get_AliasData(i) = arglist; return ret; }

                                                if ((localLIndex() ?? "") == (buf ?? ""))
                                                {
                                                    buf = withBlock21.get_AliasType(i);
                                                    break;
                                                }
                                            }

                                            if (i > withBlock21.Count)
                                            {
                                                buf = withBlock21.get_AliasType(1);
                                            }
                                        }
                                    }

                                    bool localIsDefined7() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    object argIndex32 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex32))
                                    {
                                        Unit localItem26() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        if (localItem26().IsFeatureAvailable(buf))
                                        {
                                            num_result = 1d;
                                        }
                                    }
                                    else if (localIsDefined7())
                                    {
                                        object argIndex31 = (object)pname;
                                        {
                                            var withBlock22 = SRC.PList.Item(argIndex31);
                                            if (withBlock22.Unit is object)
                                            {
                                                if (withBlock22.Unit.IsFeatureAvailable(buf))
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
                                    buf = GetValueAsString(@params[1], is_term[1]);

                                    // エリアスが定義されている？
                                    object argIndex33 = (object)buf;
                                    if (SRC.ALDList.IsDefined(argIndex33))
                                    {
                                        AliasDataType localItem3() { object argIndex1 = (object)buf; var ret = SRC.ALDList.Item(argIndex1); return ret; }

                                        buf = localItem3().get_AliasType(1);
                                    }

                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        if (Event.SelectedUnitForEvent.IsFeatureAvailable(buf))
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
                        pname = GetValueAsString(@params[1], is_term[1]);
                        switch (pcount)
                        {
                            case 2:
                                {
                                    switch (GetValueAsString(@params[2], is_term[2]) ?? "")
                                    {
                                        case "パイロット":
                                            {
                                                object argIndex34 = (object)pname;
                                                if (SRC.PList.IsDefined(argIndex34))
                                                {
                                                    Pilot localItem4() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

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
                                                if (SRC.UList.IsDefined(argIndex35))
                                                {
                                                    Unit localItem5() { object argIndex1 = (object)pname; var ret = SRC.UList.Item(argIndex1); return ret; }

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
                                                if (SRC.IList.IsDefined(argIndex36))
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
                                    bool localIsDefined8() { object argIndex1 = (object)pname; var ret = SRC.UList.IsDefined(argIndex1); return ret; }

                                    bool localIsDefined9() { object argIndex1 = (object)pname; var ret = SRC.IList.IsDefined(argIndex1); return ret; }

                                    object argIndex37 = (object)pname;
                                    if (SRC.PList.IsDefined(argIndex37))
                                    {
                                        Pilot localItem6() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

                                        if (localItem6().Alive)
                                        {
                                            num_result = 1d;
                                        }
                                    }
                                    else if (localIsDefined8())
                                    {
                                        Unit localItem7() { object argIndex1 = (object)pname; var ret = SRC.UList.Item(argIndex1); return ret; }

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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined10() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    object argIndex39 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex39))
                                    {
                                        Unit localItem27() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        string arginame = GetValueAsString(@params[2], is_term[2]);
                                        if (localItem27().IsEquiped(arginame))
                                        {
                                            num_result = 1d;
                                        }
                                    }
                                    else if (localIsDefined10())
                                    {
                                        object argIndex38 = (object)pname;
                                        {
                                            var withBlock23 = SRC.PList.Item(argIndex38);
                                            if (withBlock23.Unit is object)
                                            {
                                                string arginame1 = GetValueAsString(@params[2], is_term[2]);
                                                if (withBlock23.Unit.IsEquiped(arginame1))
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
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        string arginame2 = GetValueAsString(@params[1], is_term[1]);
                                        if (Event.SelectedUnitForEvent.IsEquiped(arginame2))
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
                        buf = GetValueAsString(@params[1], is_term[1]);
                        buf2 = GetValueAsString(@params[2], is_term[2]);
                        num = Conversions.Toint(Interaction.IIf(pcount < 3, (object)1, (object)GetValueAsLong(@params[3], is_term[3])));
                        num2 = GeneralLib.ListLength(buf);
                        var loopTo8 = num2;
                        for (i = num; i <= loopTo8; i++)
                        {
                            if ((GeneralLib.ListIndex(buf, i) ?? "") == (buf2 ?? ""))
                            {
                                if (etype == ValueType.StringType)
                                {
                                    str_result = SrcFormatter.Format((object)i);
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
                        string argstr_Renamed = GetValueAsString(@params[1], is_term[1]);
                        if (GeneralLib.IsNumber(argstr_Renamed))
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
                        if (IsVariableDefined(argvar_name))
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined11() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    Pilot localItem11() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

                                    Pilot localItem12() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

                                    object argIndex41 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex41))
                                    {
                                        i = GetValueAsLong(@params[2], is_term[2]);
                                        object argIndex40 = (object)pname;
                                        {
                                            var withBlock24 = SRC.UList.Item2(argIndex40);
                                            if (1 <= i & i <= withBlock24.CountItem())
                                            {
                                                Item localItem8() { object argIndex1 = (object)i; var ret = withBlock24.Item(argIndex1); return ret; }

                                                str_result = localItem8().Name;
                                            }
                                        }
                                    }
                                    else if (!localIsDefined11())
                                    {
                                        if (pname == "未装備")
                                        {
                                            i = 0;
                                            j = GetValueAsLong(@params[2], is_term[2]);
                                            foreach (Item currentIt1 in SRC.IList)
                                            {
                                                it = currentIt1;
                                                if (it.Unit is null & it.Exist)
                                                {
                                                    i = (i + 1);
                                                    if (i == j)
                                                    {
                                                        str_result = it.Name;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (localItem12().Unit is object)
                                    {
                                        i = GetValueAsLong(@params[2], is_term[2]);
                                        Pilot localItem10() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

                                        {
                                            var withBlock25 = localItem10().Unit;
                                            if (1 <= i & i <= withBlock25.CountItem())
                                            {
                                                Item localItem9() { object argIndex1 = (object)i; var ret = withBlock25.Item(argIndex1); return ret; }

                                                str_result = localItem9().Name;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        i = GetValueAsLong(@params[1], is_term[1]);
                                        {
                                            var withBlock26 = Event.SelectedUnitForEvent;
                                            if (1 <= i & i <= withBlock26.CountItem())
                                            {
                                                Item localItem13() { object argIndex1 = (object)i; var ret = withBlock26.Item(argIndex1); return ret; }

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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined12() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    Pilot localItem17() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

                                    Pilot localItem18() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

                                    object argIndex43 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex43))
                                    {
                                        i = GetValueAsLong(@params[2], is_term[2]);
                                        object argIndex42 = (object)pname;
                                        {
                                            var withBlock27 = SRC.UList.Item2(argIndex42);
                                            if (1 <= i & i <= withBlock27.CountItem())
                                            {
                                                Item localItem14() { object argIndex1 = (object)i; var ret = withBlock27.Item(argIndex1); return ret; }

                                                str_result = localItem14().ID;
                                            }
                                        }
                                    }
                                    else if (!localIsDefined12())
                                    {
                                        if (pname == "未装備")
                                        {
                                            i = 0;
                                            j = GetValueAsLong(@params[2], is_term[2]);
                                            foreach (Item currentIt2 in SRC.IList)
                                            {
                                                it = currentIt2;
                                                if (it.Unit is null & it.Exist)
                                                {
                                                    i = (i + 1);
                                                    if (i == j)
                                                    {
                                                        str_result = it.ID;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (localItem18().Unit is object)
                                    {
                                        i = GetValueAsLong(@params[2], is_term[2]);
                                        Pilot localItem16() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

                                        {
                                            var withBlock28 = localItem16().Unit;
                                            if (1 <= i & i <= withBlock28.CountItem())
                                            {
                                                Item localItem15() { object argIndex1 = (object)i; var ret = withBlock28.Item(argIndex1); return ret; }

                                                str_result = localItem15().ID;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        i = GetValueAsLong(@params[1], is_term[1]);
                                        {
                                            var withBlock29 = Event.SelectedUnitForEvent;
                                            if (1 <= i & i <= withBlock29.CountItem())
                                            {
                                                Item localItem19() { object argIndex1 = (object)i; var ret = withBlock29.Item(argIndex1); return ret; }

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
                        str_result = Strings.Left(GetValueAsString(@params[1], is_term[1]), GetValueAsLong(@params[2], is_term[2]));
                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(str_result);
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
                        buf = GetValueAsString(@params[1], is_term[1]);
                        // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                        // UPGRADE_ISSUE: LeftB$ 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                        str_result = LeftB(Strings.StrConv(buf, vbFromUnicode), GetValueAsLong(@params[2], is_term[2]));
                        // UPGRADE_ISSUE: 定数 vbUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                        str_result = Strings.StrConv(str_result, vbUnicode);
                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(str_result);
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
                        num_result = (double)Strings.Len(GetValueAsString(@params[1], is_term[1]));
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
                        buf = GetValueAsString(@params[1], is_term[1]);
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined13() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    object argIndex44 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex44))
                                    {
                                        Unit localItem28() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        num = localItem28().MainPilot().Level;
                                    }
                                    else if (!localIsDefined13())
                                    {
                                        num_result = 0d;
                                    }
                                    else
                                    {
                                        Pilot localItem20() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

                                        num_result = (double)localItem20().Level;
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        {
                                            var withBlock30 = Event.SelectedUnitForEvent;
                                            if (withBlock30.CountPilot() > 0)
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
                        str_result = Strings.LCase(GetValueAsString(@params[1], is_term[1]));
                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "lset":
                    {
                        buf = GetValueAsString(@params[1], is_term[1]);
                        i = GetValueAsLong(@params[2], is_term[2]);
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
                        num_result = GetValueAsDouble(@params[1], is_term[1]);
                        var loopTo9 = pcount;
                        for (i = 2; i <= loopTo9; i++)
                        {
                            rdbl = GetValueAsDouble(@params[i], is_term[i]);
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
                        buf = GetValueAsString(@params[1], is_term[1]);
                        switch (pcount)
                        {
                            case 3:
                                {
                                    i = GetValueAsLong(@params[2], is_term[2]);
                                    j = GetValueAsLong(@params[3], is_term[3]);
                                    str_result = Strings.Mid(buf, i, j);
                                    break;
                                }

                            case 2:
                                {
                                    i = GetValueAsLong(@params[2], is_term[2]);
                                    str_result = Strings.Mid(buf, i);
                                    break;
                                }
                        }

                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(str_result);
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
                        buf = GetValueAsString(@params[1], is_term[1]);
                        switch (pcount)
                        {
                            case 3:
                                {
                                    i = GetValueAsLong(@params[2], is_term[2]);
                                    j = GetValueAsLong(@params[3], is_term[3]);
                                    // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                                    // UPGRADE_ISSUE: MidB$ 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                                    str_result = MidB(Strings.StrConv(buf, vbFromUnicode), i, j);
                                    break;
                                }

                            case 2:
                                {
                                    i = GetValueAsLong(@params[2], is_term[2]);
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
                            num_result = GeneralLib.StrToDbl(str_result);
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
                        num_result = GetValueAsDouble(@params[1], is_term[1]);
                        var loopTo10 = pcount;
                        for (i = 2; i <= loopTo10; i++)
                        {
                            rdbl = GetValueAsDouble(@params[i], is_term[i]);
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined14() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    object argIndex45 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex45))
                                    {
                                        Unit localItem29() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        num_result = (double)localItem29().MainPilot().Morale;
                                    }
                                    else if (localIsDefined14())
                                    {
                                        Pilot localItem30() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

                                        num_result = (double)localItem30().Morale;
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        {
                                            var withBlock31 = Event.SelectedUnitForEvent;
                                            if (withBlock31.CountPilot() > 0)
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
                                    buf = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined15() { object argIndex1 = (object)buf; var ret = SRC.PDList.IsDefined(argIndex1); return ret; }

                                    bool localIsDefined16() { object argIndex1 = (object)buf; var ret = SRC.NPDList.IsDefined(argIndex1); return ret; }

                                    bool localIsDefined17() { object argIndex1 = (object)buf; var ret = SRC.UList.IsDefined(argIndex1); return ret; }

                                    bool localIsDefined18() { object argIndex1 = (object)buf; var ret = SRC.UDList.IsDefined(argIndex1); return ret; }

                                    bool localIsDefined19() { object argIndex1 = (object)buf; var ret = SRC.IDList.IsDefined(argIndex1); return ret; }

                                    object argIndex46 = (object)buf;
                                    if (SRC.PList.IsDefined(argIndex46))
                                    {
                                        Pilot localItem31() { object argIndex1 = (object)buf; var ret = SRC.PList.Item(argIndex1); return ret; }

                                        str_result = localItem31().get_Nickname(false);
                                    }
                                    else if (localIsDefined15())
                                    {
                                        PilotData localItem32() { object argIndex1 = (object)buf; var ret = SRC.PDList.Item(argIndex1); return ret; }

                                        str_result = localItem32().Nickname;
                                    }
                                    else if (localIsDefined16())
                                    {
                                        NonPilotData localItem33() { object argIndex1 = (object)buf; var ret = SRC.NPDList.Item(argIndex1); return ret; }

                                        str_result = localItem33().Nickname;
                                    }
                                    else if (localIsDefined17())
                                    {
                                        Unit localItem34() { object argIndex1 = (object)buf; var ret = SRC.UList.Item(argIndex1); return ret; }

                                        str_result = localItem34().Nickname0;
                                    }
                                    else if (localIsDefined18())
                                    {
                                        UnitData localItem35() { object argIndex1 = (object)buf; var ret = SRC.UDList.Item(argIndex1); return ret; }

                                        str_result = localItem35().Nickname;
                                    }
                                    else if (localIsDefined19())
                                    {
                                        ItemData localItem36() { object argIndex1 = (object)buf; var ret = SRC.IDList.Item(argIndex1); return ret; }

                                        str_result = localItem36().Nickname;
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        str_result = Event.SelectedUnitForEvent.Nickname0;
                                    }

                                    break;
                                }
                        }

                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "partner":
                    {
                        i = GetValueAsLong(@params[1], is_term[1]);
                        if (i == 0)
                        {
                            str_result = Event.SelectedUnitForEvent.ID;
                        }
                        else if (1 <= i & i <= Information.UBound(Commands.SelectedPartners))
                        {
                            str_result = Commands.SelectedPartners[i].ID;
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined20() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    object argIndex47 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex47))
                                    {
                                        Unit localItem210() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        str_result = localItem210().Party0;
                                    }
                                    else if (localIsDefined20())
                                    {
                                        Pilot localItem37() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

                                        str_result = localItem37().Party;
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        str_result = Event.SelectedUnitForEvent.Party0;
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
                                    uname = GetValueAsString(@params[1], is_term[1]);
                                    object argIndex49 = (object)uname;
                                    if (SRC.UList.IsDefined(argIndex49))
                                    {
                                        i = GetValueAsLong(@params[2], is_term[2]);
                                        object argIndex48 = (object)uname;
                                        {
                                            var withBlock32 = SRC.UList.Item(argIndex48);
                                            if (0 < i & i <= withBlock32.CountPilot())
                                            {
                                                Pilot localPilot() { object argIndex1 = (object)i; var ret = withBlock32.Pilot(argIndex1); return ret; }

                                                str_result = localPilot().Name;
                                            }
                                            else if (withBlock32.CountPilot() < i & i <= (withBlock32.CountPilot() + withBlock32.CountSupport()))
                                            {
                                                Pilot localSupport() { object argIndex1 = (object)(i - withBlock32.CountPilot()); var ret = withBlock32.Support(argIndex1); return ret; }

                                                str_result = localSupport().Name;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    uname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined21() { object argIndex1 = (object)uname; var ret = SRC.UList.IsDefined(argIndex1); return ret; }

                                    if (GeneralLib.IsNumber(uname))
                                    {
                                        if (Event.SelectedUnitForEvent is object)
                                        {
                                            i = Conversions.Toint(uname);
                                            {
                                                var withBlock33 = Event.SelectedUnitForEvent;
                                                if (0 < i & i <= withBlock33.CountPilot())
                                                {
                                                    Pilot localPilot1() { object argIndex1 = (object)i; var ret = withBlock33.Pilot(argIndex1); return ret; }

                                                    str_result = localPilot1().Name;
                                                }
                                                else if (withBlock33.CountPilot() < i & i <= (withBlock33.CountPilot() + withBlock33.CountSupport()))
                                                {
                                                    Pilot localSupport1() { object argIndex1 = (object)(i - withBlock33.CountPilot()); var ret = withBlock33.Support(argIndex1); return ret; }

                                                    str_result = localSupport1().Name;
                                                }
                                            }
                                        }
                                    }
                                    else if (localIsDefined21())
                                    {
                                        object argIndex50 = (object)uname;
                                        {
                                            var withBlock34 = SRC.UList.Item(argIndex50);
                                            if (withBlock34.CountPilot() > 0)
                                            {
                                                str_result = withBlock34.MainPilot().Name;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        {
                                            var withBlock35 = Event.SelectedUnitForEvent;
                                            if (withBlock35.CountPilot() > 0)
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
                                    uname = GetValueAsString(@params[1], is_term[1]);
                                    object argIndex52 = (object)uname;
                                    if (SRC.UList.IsDefined(argIndex52))
                                    {
                                        i = GetValueAsLong(@params[2], is_term[2]);
                                        object argIndex51 = (object)uname;
                                        {
                                            var withBlock36 = SRC.UList.Item(argIndex51);
                                            if (0 < i & i <= withBlock36.CountPilot())
                                            {
                                                Pilot localPilot2() { object argIndex1 = (object)i; var ret = withBlock36.Pilot(argIndex1); return ret; }

                                                str_result = localPilot2().ID;
                                            }
                                            else if (withBlock36.CountPilot() < i & i <= (withBlock36.CountPilot() + withBlock36.CountSupport()))
                                            {
                                                Pilot localSupport2() { object argIndex1 = (object)(i - withBlock36.CountPilot()); var ret = withBlock36.Support(argIndex1); return ret; }

                                                str_result = localSupport2().ID;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    uname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined22() { object argIndex1 = (object)uname; var ret = SRC.UList.IsDefined(argIndex1); return ret; }

                                    if (GeneralLib.IsNumber(uname))
                                    {
                                        if (Event.SelectedUnitForEvent is object)
                                        {
                                            i = Conversions.Toint(uname);
                                            {
                                                var withBlock37 = Event.SelectedUnitForEvent;
                                                if (0 < i & i <= withBlock37.CountPilot())
                                                {
                                                    Pilot localPilot3() { object argIndex1 = (object)i; var ret = withBlock37.Pilot(argIndex1); return ret; }

                                                    str_result = localPilot3().ID;
                                                }
                                                else if (withBlock37.CountPilot() < i & i <= (withBlock37.CountPilot() + withBlock37.CountSupport()))
                                                {
                                                    Pilot localSupport3() { object argIndex1 = (object)(i - withBlock37.CountPilot()); var ret = withBlock37.Support(argIndex1); return ret; }

                                                    str_result = localSupport3().ID;
                                                }
                                            }
                                        }
                                    }
                                    else if (localIsDefined22())
                                    {
                                        object argIndex53 = (object)uname;
                                        {
                                            var withBlock38 = SRC.UList.Item(argIndex53);
                                            if (withBlock38.CountPilot() > 0)
                                            {
                                                str_result = withBlock38.MainPilot().ID;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        {
                                            var withBlock39 = Event.SelectedUnitForEvent;
                                            if (withBlock39.CountPilot() > 0)
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined23() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    object argIndex54 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex54))
                                    {
                                        Unit localItem211() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        num_result = (double)localItem211().MainPilot().Plana;
                                    }
                                    else if (localIsDefined23())
                                    {
                                        Pilot localItem38() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

                                        num_result = (double)localItem38().Plana;
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        num_result = (double)Event.SelectedUnitForEvent.MainPilot().Plana;
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
                        num_result = (double)GeneralLib.Dice(GetValueAsLong(@params[1], is_term[1]));
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined24() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    object argIndex56 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex56))
                                    {
                                        Unit localItem212() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

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
                                            var withBlock40 = SRC.PList.Item(argIndex55);
                                            if (withBlock40.Unit is object)
                                            {
                                                num_result = (double)withBlock40.Unit.Rank;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        num_result = (double)Event.SelectedUnitForEvent.Rank;
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
                        if (pcount > 0)
                        {
                            // 文字列全体を検索
                            // UPGRADE_WARNING: オブジェクト RegEx.Global の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            RegEx.Global = (object)true;
                            // 大文字小文字の区別（True=区別しない）
                            // UPGRADE_WARNING: オブジェクト RegEx.IgnoreCase の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            RegEx.IgnoreCase = (object)false;
                            if (pcount >= 3)
                            {
                                if (GetValueAsString(@params[3], is_term[3]) == "大小区別なし")
                                {
                                    // UPGRADE_WARNING: オブジェクト RegEx.IgnoreCase の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                    RegEx.IgnoreCase = (object)true;
                                }
                            }
                            // 検索パターン
                            // UPGRADE_WARNING: オブジェクト RegEx.Pattern の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            RegEx.Pattern = GetValueAsString(@params[2], is_term[2]);
                            // UPGRADE_WARNING: オブジェクト RegEx.Execute の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            Matches = RegEx.Execute(GetValueAsString(@params[1], is_term[1]));
                            // UPGRADE_WARNING: オブジェクト Matches.Count の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(Matches.Count, 0, false)))
                            {
                                regexp_index = -1;
                            }
                            else
                            {
                                regexp_index = 0;
                                // UPGRADE_WARNING: オブジェクト Matches() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                buf = Conversions.ToString(Expression.Matches((object)regexp_index));
                            }
                        }
                        else if (regexp_index >= 0)
                        {
                            regexp_index = (regexp_index + 1);
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
                        Event.DisplayEventErrorMessage(Event.CurrentLineNum, "VBScriptがインストールされていません");
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
                        if (pcount >= 4)
                        {
                            if (GetValueAsString(@params[4], is_term[4]) == "大小区別なし")
                            {
                                // UPGRADE_WARNING: オブジェクト RegEx.IgnoreCase の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                RegEx.IgnoreCase = (object)true;
                            }
                        }
                        // 検索パターン
                        // UPGRADE_WARNING: オブジェクト RegEx.Pattern の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        RegEx.Pattern = GetValueAsString(@params[2], is_term[2]);

                        // 置換実行
                        // UPGRADE_WARNING: オブジェクト RegEx.Replace の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        buf = Conversions.ToString(RegEx.Replace(GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[3], is_term[3])));
                        str_result = buf;
                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    RegExpReplace_Error:
                        ;
                        Event.DisplayEventErrorMessage(Event.CurrentLineNum, "VBScriptがインストールされていません");
                        return CallFunctionRet;
                    }

                case "relation":
                    {
                        pname = GetValueAsString(@params[1], is_term[1]);
                        bool localIsDefined25() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

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

                        Pilot localItem39() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

                        pname = localItem39().Name;
                        pname2 = GetValueAsString(@params[2], is_term[2]);
                        bool localIsDefined26() { object argIndex1 = (object)pname2; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

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

                        Pilot localItem40() { object argIndex1 = (object)pname2; var ret = SRC.PList.Item(argIndex1); return ret; }

                        pname2 = localItem40().Name;
                        string argexpr = "関係:" + pname + ":" + pname2;
                        num_result = (double)GetValueAsLong(argexpr);
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
                                    buf = GetValueAsString(@params[1], is_term[1]);
                                    num = GetValueAsLong(@params[4], is_term[4]);
                                    buf2 = Strings.Right(buf, Strings.Len(buf) - num + 1);
                                    string args2 = GetValueAsString(@params[2], is_term[2]);
                                    string args3 = GetValueAsString(@params[3], is_term[3]);
                                    GeneralLib.ReplaceString(buf2, args2, args3);
                                    str_result = Strings.Left(buf, num - 1) + buf2;
                                    break;
                                }

                            case 5:
                                {
                                    buf = GetValueAsString(@params[1], is_term[1]);
                                    num = GetValueAsLong(@params[4], is_term[4]);
                                    num2 = GetValueAsLong(@params[5], is_term[5]);
                                    buf2 = Strings.Mid(buf, num, num2);
                                    string args21 = GetValueAsString(@params[2], is_term[2]);
                                    string args31 = GetValueAsString(@params[3], is_term[3]);
                                    GeneralLib.ReplaceString(buf2, args21, args31);
                                    str_result = Strings.Left(buf, num - 1) + buf2 + Strings.Right(buf, Strings.Len(buf) - (num + num2 - 1) - 1);
                                    break;
                                }

                            default:
                                {
                                    str_result = GetValueAsString(@params[1], is_term[1]);
                                    string args22 = GetValueAsString(@params[2], is_term[2]);
                                    string args32 = GetValueAsString(@params[3], is_term[3]);
                                    GeneralLib.ReplaceString(str_result, args22, args32);
                                    break;
                                }
                        }

                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(str_result);
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
                        buf = Conversion.Hex(Information.RGB(GetValueAsLong(@params[1], is_term[1]), GetValueAsLong(@params[2], is_term[2]), GetValueAsLong(@params[3], is_term[3])));
                        var loopTo11 = (6 - Strings.Len(buf));
                        for (i = 1; i <= loopTo11; i++)
                            buf = "0" + buf;
                        str_result = "#000000";
                        var midTmp = Strings.Mid(buf, 5, 2);
                        StringType.MidStmtStr(str_result, 2, 2, midTmp);
                        var midTmp1 = Strings.Mid(buf, 3, 2);
                        StringType.MidStmtStr(str_result, 4, 2, midTmp1);
                        var midTmp2 = Strings.Mid(buf, 1, 2);
                        StringType.MidStmtStr(str_result, 6, 2, midTmp2);
                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "right":
                    {
                        str_result = Strings.Right(GetValueAsString(@params[1], is_term[1]), GetValueAsLong(@params[2], is_term[2]));
                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(str_result);
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
                        buf = GetValueAsString(@params[1], is_term[1]);
                        // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                        // UPGRADE_ISSUE: RightB$ 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                        str_result = RightB(Strings.StrConv(buf, vbFromUnicode), GetValueAsLong(@params[2], is_term[2]));
                        // UPGRADE_ISSUE: 定数 vbUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                        str_result = Strings.StrConv(str_result, vbUnicode);
                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(str_result);
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
                        ldbl = GetValueAsDouble(@params[1], is_term[1]);
                        if (pcount == 1)
                        {
                            num2 = 0;
                        }
                        else
                        {
                            num2 = GetValueAsLong(@params[2], is_term[2]);
                        }

                        num = Conversion.Int(ldbl * Math.Pow(10d, (double)num2));
                        switch (Strings.LCase(fname) ?? "")
                        {
                            case "round":
                                {
                                    if (ldbl * Math.Pow(10d, (double)num2) - (double)num >= 0.5d)
                                    {
                                        num = (num + 1);
                                    }

                                    break;
                                }

                            case "roundup":
                                {
                                    if (ldbl * Math.Pow(10d, (double)num2) - (double)num > 0d)
                                    {
                                        num = (num + 1);
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
                        buf = GetValueAsString(@params[1], is_term[1]);
                        i = GetValueAsLong(@params[2], is_term[2]);
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
                        num_result = Math.Sin(GetValueAsDouble(@params[1], is_term[1]));
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    buf = GetValueAsString(@params[2], is_term[2]);

                                    // エリアスが定義されている？
                                    object argIndex57 = (object)buf;
                                    if (SRC.ALDList.IsDefined(argIndex57))
                                    {
                                        AliasDataType localItem41() { object argIndex1 = (object)buf; var ret = SRC.ALDList.Item(argIndex1); return ret; }

                                        buf = localItem41().get_AliasType(1);
                                    }

                                    bool localIsDefined27() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    object argIndex60 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex60))
                                    {
                                        Unit localItem213() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        object argIndex58 = (object)buf;
                                        string argref_mode = "";
                                        num_result = localItem213().MainPilot().SkillLevel(argIndex58, ref_mode: argref_mode);
                                    }
                                    else if (localIsDefined27())
                                    {
                                        Pilot localItem42() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

                                        object argIndex59 = (object)buf;
                                        string argref_mode1 = "";
                                        num_result = localItem42().SkillLevel(argIndex59, ref_mode: argref_mode1);
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    buf = GetValueAsString(@params[1], is_term[1]);

                                    // エリアスが定義されている？
                                    object argIndex61 = (object)buf;
                                    if (SRC.ALDList.IsDefined(argIndex61))
                                    {
                                        AliasDataType localItem43() { object argIndex1 = (object)buf; var ret = SRC.ALDList.Item(argIndex1); return ret; }

                                        buf = localItem43().get_AliasType(1);
                                    }

                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        object argIndex62 = (object)buf;
                                        string argref_mode2 = "";
                                        num_result = Event.SelectedUnitForEvent.MainPilot().SkillLevel(argIndex62, ref_mode: argref_mode2);
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined28() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    object argIndex63 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex63))
                                    {
                                        Unit localItem214() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        num_result = (double)localItem214().MainPilot().SP;
                                    }
                                    else if (localIsDefined28())
                                    {
                                        Pilot localItem44() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

                                        num_result = (double)localItem44().SP;
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        num_result = (double)Event.SelectedUnitForEvent.MainPilot().SP;
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    buf = GetValueAsString(@params[2], is_term[2]);
                                    bool localIsDefined29() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    object argIndex65 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex65))
                                    {
                                        Unit localItem215() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        if (localItem215().IsSpecialPowerInEffect(buf))
                                        {
                                            num_result = 1d;
                                        }
                                    }
                                    else if (localIsDefined29())
                                    {
                                        object argIndex64 = (object)pname;
                                        {
                                            var withBlock41 = SRC.PList.Item(argIndex64);
                                            if (withBlock41.Unit is object)
                                            {
                                                if (withBlock41.Unit.IsSpecialPowerInEffect(buf))
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
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        buf = GetValueAsString(@params[1], is_term[1]);
                                        if (Event.SelectedUnitForEvent.IsSpecialPowerInEffect(buf))
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
                        num_result = Math.Sqrt(GetValueAsDouble(@params[1], is_term[1]));
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined30() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    object argIndex67 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex67))
                                    {
                                        Unit localItem216() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        str_result = localItem216().Status_Renamed;
                                    }
                                    else if (localIsDefined30())
                                    {
                                        object argIndex66 = (object)pname;
                                        {
                                            var withBlock42 = SRC.PList.Item(argIndex66);
                                            if (withBlock42.Unit is object)
                                            {
                                                str_result = withBlock42.Unit.Status_Renamed;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        str_result = Event.SelectedUnitForEvent.Status_Renamed;
                                    }

                                    break;
                                }
                        }

                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "strcomp":
                    {
                        num_result = (double)Strings.StrComp(GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]));
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
                        buf = GetValueAsString(@params[2], is_term[2]);
                        if (Strings.Len(buf) <= 1)
                        {
                            str_result = new string(Conversions.ToChar(buf), GetValueAsLong(@params[1], is_term[1]));
                        }
                        else
                        {
                            // String関数では文字列の先頭しか繰り返しされないので、
                            // 長さが2以上の文字列の場合は別処理
                            str_result = "";
                            var loopTo12 = GetValueAsLong(@params[1], is_term[1]);
                            for (i = 1; i <= loopTo12; i++)
                                str_result = str_result + buf;
                        }

                        if (etype == ValueType.NumericType)
                        {
                            num_result = GeneralLib.StrToDbl(str_result);
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
                        num_result = Math.Tan(GetValueAsDouble(@params[1], is_term[1]));
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
                                    pname = GetValueAsString(@params[2], is_term[2]);
                                    object argIndex68 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex68))
                                    {
                                        Unit localItem217() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        string argtname = GetValueAsString(@params[1], is_term[1]);
                                        var argu = localItem217();
                                        str_result = Term(argtname, argu);
                                    }
                                    else
                                    {
                                        string argtname1 = GetValueAsString(@params[1], is_term[1]);
                                        Unit argu1 = null;
                                        str_result = Term(argtname1, u: argu1);
                                    }

                                    break;
                                }

                            case 1:
                                {
                                    string argtname2 = GetValueAsString(@params[1], is_term[1]);
                                    Unit argu2 = null;
                                    str_result = Term(argtname2, u: argu2);
                                    break;
                                }
                        }

                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "textheight":
                    {
                        // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                        num_result = GUI.MainForm.picMain(0).TextHeight(GetValueAsString(@params[1], is_term[1]));
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
                        num_result = GUI.MainForm.picMain(0).TextWidth(GetValueAsString(@params[1], is_term[1]));
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
                        str_result = Strings.Trim(GetValueAsString(@params[1], is_term[1]));
                        CallFunctionRet = ValueType.StringType;
                        return CallFunctionRet;
                    }

                case "unit":
                    {
                        switch (pcount)
                        {
                            case 1:
                                {
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined31() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    object argIndex70 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex70))
                                    {
                                        Unit localItem218() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        str_result = localItem218().Name;
                                    }
                                    else if (localIsDefined31())
                                    {
                                        object argIndex69 = (object)pname;
                                        {
                                            var withBlock43 = SRC.PList.Item(argIndex69);
                                            if (withBlock43.Unit is object)
                                            {
                                                str_result = withBlock43.Unit.Name;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        str_result = Event.SelectedUnitForEvent.Name;
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined32() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    object argIndex72 = (object)pname;
                                    if (SRC.UList.IsDefined2(argIndex72))
                                    {
                                        Unit localItem219() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        str_result = localItem219().ID;
                                    }
                                    else if (localIsDefined32())
                                    {
                                        object argIndex71 = (object)pname;
                                        {
                                            var withBlock44 = SRC.PList.Item(argIndex71);
                                            if (withBlock44.Unit is object)
                                            {
                                                str_result = withBlock44.Unit.ID;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        str_result = Event.SelectedUnitForEvent.ID;
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
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
                                                bool localIsDefined33() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                                object argIndex74 = (object)pname;
                                                if (SRC.UList.IsDefined2(argIndex74))
                                                {
                                                    Unit localItem220() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                                    num_result = (double)localItem220().x;
                                                }
                                                else if (localIsDefined33())
                                                {
                                                    object argIndex73 = (object)pname;
                                                    {
                                                        var withBlock45 = SRC.PList.Item(argIndex73);
                                                        if (withBlock45.Unit is object)
                                                        {
                                                            num_result = (double)withBlock45.Unit.x;
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
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        num_result = (double)Event.SelectedUnitForEvent.x;
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
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
                                                bool localIsDefined34() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                                object argIndex76 = (object)pname;
                                                if (SRC.UList.IsDefined2(argIndex76))
                                                {
                                                    Unit localItem221() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                                    num_result = (double)localItem221().y;
                                                }
                                                else if (localIsDefined34())
                                                {
                                                    object argIndex75 = (object)pname;
                                                    {
                                                        var withBlock46 = SRC.PList.Item(argIndex75);
                                                        if (withBlock46.Unit is object)
                                                        {
                                                            num_result = (double)withBlock46.Unit.y;
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
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        num_result = (double)Event.SelectedUnitForEvent.y;
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined210() { object argIndex1 = (object)pname; var ret = SRC.UList.IsDefined2(argIndex1); return ret; }

                                    bool localIsDefined35() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    if (GeneralLib.IsNumber(pname))
                                    {
                                        num_result = (double)GeneralLib.StrToLng(pname);
                                    }
                                    else if (pname == "目標地点")
                                    {
                                        num_result = (double)Commands.SelectedX;
                                    }
                                    else if (localIsDefined210())
                                    {
                                        Unit localItem222() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        num_result = (double)localItem222().x;
                                    }
                                    else if (localIsDefined35())
                                    {
                                        object argIndex77 = (object)pname;
                                        {
                                            var withBlock47 = SRC.PList.Item(argIndex77);
                                            if (withBlock47.Unit is object)
                                            {
                                                num_result = (double)withBlock47.Unit.x;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        num_result = (double)Event.SelectedUnitForEvent.x;
                                    }

                                    break;
                                }
                        }

                        num_result = (double)GUI.MapToPixelX(num_result);
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
                                    pname = GetValueAsString(@params[1], is_term[1]);
                                    bool localIsDefined211() { object argIndex1 = (object)pname; var ret = SRC.UList.IsDefined2(argIndex1); return ret; }

                                    bool localIsDefined36() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                                    if (GeneralLib.IsNumber(pname))
                                    {
                                        num_result = (double)GeneralLib.StrToLng(pname);
                                    }
                                    else if (pname == "目標地点")
                                    {
                                        num_result = (double)Commands.SelectedY;
                                    }
                                    else if (localIsDefined211())
                                    {
                                        Unit localItem223() { object argIndex1 = (object)pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

                                        num_result = (double)localItem223().y;
                                    }
                                    else if (localIsDefined36())
                                    {
                                        object argIndex78 = (object)pname;
                                        {
                                            var withBlock48 = SRC.PList.Item(argIndex78);
                                            if (withBlock48.Unit is object)
                                            {
                                                num_result = (double)withBlock48.Unit.y;
                                            }
                                        }
                                    }

                                    break;
                                }

                            case 0:
                                {
                                    if (Event.SelectedUnitForEvent is object)
                                    {
                                        num_result = (double)Event.SelectedUnitForEvent.y;
                                    }

                                    break;
                                }
                        }

                        num_result = (double)GUI.MapToPixelY(num_result);
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
                        str_result = Strings.StrConv(GetValueAsString(@params[1], is_term[1]), VbStrConv.Wide);
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
                                    buf = GetValueAsString(@params[1], is_term[1]);
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
                                    buf = GetValueAsString(@params[1], is_term[1]);
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
                                    buf = GetValueAsString(@params[1], is_term[1]);
                                    if (Information.IsDate(buf))
                                    {
                                        switch (DateAndTime.Weekday(Conversions.ToDate(buf)))
                                        {
                                            case FirstDayOfWeek.Sunday:
                                                {
                                                    str_result = "日曜";
                                                    break;
                                                }

                                            case FirstDayOfWeek.Monday:
                                                {
                                                    str_result = "月曜";
                                                    break;
                                                }

                                            case FirstDayOfWeek.Tuesday:
                                                {
                                                    str_result = "火曜";
                                                    break;
                                                }

                                            case FirstDayOfWeek.Wednesday:
                                                {
                                                    str_result = "水曜";
                                                    break;
                                                }

                                            case FirstDayOfWeek.Thursday:
                                                {
                                                    str_result = "木曜";
                                                    break;
                                                }

                                            case FirstDayOfWeek.Friday:
                                                {
                                                    str_result = "金曜";
                                                    break;
                                                }

                                            case FirstDayOfWeek.Saturday:
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
                                        case FirstDayOfWeek.Sunday:
                                            {
                                                str_result = "日曜";
                                                break;
                                            }

                                        case FirstDayOfWeek.Monday:
                                            {
                                                str_result = "月曜";
                                                break;
                                            }

                                        case FirstDayOfWeek.Tuesday:
                                            {
                                                str_result = "火曜";
                                                break;
                                            }

                                        case FirstDayOfWeek.Wednesday:
                                            {
                                                str_result = "水曜";
                                                break;
                                            }

                                        case FirstDayOfWeek.Thursday:
                                            {
                                                str_result = "木曜";
                                                break;
                                            }

                                        case FirstDayOfWeek.Friday:
                                            {
                                                str_result = "金曜";
                                                break;
                                            }

                                        case FirstDayOfWeek.Saturday:
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
                                    buf = GetValueAsString(@params[1], is_term[1]);
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
                                    buf = GetValueAsString(@params[1], is_term[1]);
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
                                    buf = GetValueAsString(@params[1], is_term[1]);
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
                                    buf = GetValueAsString(@params[1], is_term[1]);
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
                        if (pcount == 2)
                        {
                            if (@params[1] == "Now")
                            {
                                d1 = DateAndTime.Now;
                            }
                            else
                            {
                                buf = GetValueAsString(@params[1], is_term[1]);
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
                                buf = GetValueAsString(@params[2], is_term[2]);
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
                                    string argftype = GetValueAsString(@params[1], is_term[1]);
                                    string argfsuffix = GetValueAsString(@params[2], is_term[2]);
                                    string argftype2 = "";
                                    string argfsuffix2 = "";
                                    string argftype3 = "";
                                    string argfsuffix3 = "";
                                    str_result = FileDialog.LoadFileDialog(argdtitle, SRC.ScenarioPath, argdefault_file, 2, argftype, argfsuffix, ftype2: argftype2, fsuffix2: argfsuffix2, ftype3: argftype3, fsuffix3: argfsuffix3);
                                    break;
                                }

                            case 3:
                                {
                                    string argdtitle1 = "ファイルを開く";
                                    string argdefault_file1 = GetValueAsString(@params[3], is_term[3]);
                                    string argftype1 = GetValueAsString(@params[1], is_term[1]);
                                    string argfsuffix1 = GetValueAsString(@params[2], is_term[2]);
                                    string argftype21 = "";
                                    string argfsuffix21 = "";
                                    string argftype31 = "";
                                    string argfsuffix31 = "";
                                    str_result = FileDialog.LoadFileDialog(argdtitle1, SRC.ScenarioPath, argdefault_file1, 2, argftype1, argfsuffix1, ftype2: argftype21, fsuffix2: argfsuffix21, ftype3: argftype31, fsuffix3: argfsuffix31);
                                    break;
                                }

                            case 4:
                                {
                                    string argdtitle2 = "ファイルを開く";
                                    string argfpath = SRC.ScenarioPath + GetValueAsString(@params[4], is_term[4]);
                                    string argdefault_file2 = GetValueAsString(@params[3], is_term[3]);
                                    string argftype4 = GetValueAsString(@params[1], is_term[1]);
                                    string argfsuffix4 = GetValueAsString(@params[2], is_term[2]);
                                    string argftype22 = "";
                                    string argfsuffix22 = "";
                                    string argftype32 = "";
                                    string argfsuffix32 = "";
                                    str_result = FileDialog.LoadFileDialog(argdtitle2, argfpath, argdefault_file2, 2, argftype4, argfsuffix4, ftype2: argftype22, fsuffix2: argfsuffix22, ftype3: argftype32, fsuffix3: argfsuffix32);
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
                                    string argftype5 = GetValueAsString(@params[1], is_term[1]);
                                    string argfsuffix5 = GetValueAsString(@params[2], is_term[2]);
                                    string argftype23 = "";
                                    string argfsuffix23 = "";
                                    string argftype33 = "";
                                    string argfsuffix33 = "";
                                    str_result = FileDialog.SaveFileDialog(argdtitle3, SRC.ScenarioPath, argdefault_file3, 2, argftype5, argfsuffix5, ftype2: argftype23, fsuffix2: argfsuffix23, ftype3: argftype33, fsuffix3: argfsuffix33);
                                    break;
                                }

                            case 3:
                                {
                                    string argdtitle4 = "ファイルを保存";
                                    string argdefault_file4 = GetValueAsString(@params[3], is_term[3]);
                                    string argftype6 = GetValueAsString(@params[1], is_term[1]);
                                    string argfsuffix6 = GetValueAsString(@params[2], is_term[2]);
                                    string argftype24 = "";
                                    string argfsuffix24 = "";
                                    string argftype34 = "";
                                    string argfsuffix34 = "";
                                    str_result = FileDialog.SaveFileDialog(argdtitle4, SRC.ScenarioPath, argdefault_file4, 2, argftype6, argfsuffix6, ftype2: argftype24, fsuffix2: argfsuffix24, ftype3: argftype34, fsuffix3: argfsuffix34);
                                    break;
                                }

                            case 4:
                                {
                                    string argdtitle5 = "ファイルを保存";
                                    string argfpath1 = SRC.ScenarioPath + GetValueAsString(@params[4], is_term[4]);
                                    string argdefault_file5 = GetValueAsString(@params[3], is_term[3]);
                                    string argftype7 = GetValueAsString(@params[1], is_term[1]);
                                    string argfsuffix7 = GetValueAsString(@params[2], is_term[2]);
                                    string argftype25 = "";
                                    string argfsuffix25 = "";
                                    string argftype35 = "";
                                    string argfsuffix35 = "";
                                    str_result = FileDialog.SaveFileDialog(argdtitle5, argfpath1, argdefault_file5, 2, argftype7, argfsuffix7, ftype2: argftype25, fsuffix2: argfsuffix25, ftype3: argftype35, fsuffix3: argfsuffix35);
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

                        var loopTo13 = Strings.Len(str_result);
                        for (i = 1; i <= loopTo13; i++)
                        {
                            if (Strings.Mid(str_result, Strings.Len(str_result) - i + 1, 1) == @"\")
                            {
                                break;
                            }
                        }

                        buf = Strings.Left(str_result, Strings.Len(str_result) - i);
                        str_result = Strings.Mid(str_result, Strings.Len(str_result) - i + 2);
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
            ret = Event.FindNormalLabel(fname);
            if (ret > 0)
            {
                // 関数が見つかった
                ret = ret + 1;

                // 呼び出し階層をチェック
                if (Event.CallDepth > Event.MaxCallDepth)
                {
                    Event.CallDepth = Event.MaxCallDepth;
                    Event.DisplayEventErrorMessage(Event.CurrentLineNum, GeneralLib.FormatNum(Event.MaxCallDepth) + "階層を越えるサブルーチンの呼び出しは出来ません");
                    return CallFunctionRet;
                }

                // 引数用スタックが溢れないかチェック
                if ((Event.ArgIndex + pcount) > Event.MaxArgIndex)
                {
                    Event.DisplayEventErrorMessage(Event.CurrentLineNum, "サブルーチンの引数の総数が" + GeneralLib.FormatNum(Event.MaxArgIndex) + "個を超えています");
                    return CallFunctionRet;
                }

                // 引数の値を先に求めておく
                // (スタックに積みながら計算すると、引数での関数呼び出しで不正になる)
                var loopTo14 = pcount;
                for (i = 1; i <= loopTo14; i++)
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
                cur_depth = Event.CallDepth;

                // 引数をスタックに積む
                Event.ArgIndex = (Event.ArgIndex + pcount);
                var loopTo15 = pcount;
                for (i = 1; i <= loopTo15; i++)
                    Event.ArgStack[Event.ArgIndex - i + 1] = @params[i];

                // サブルーチン本体を実行
                do
                {
                    Event.CurrentLineNum = ret;
                    if (Event.CurrentLineNum > Information.UBound(Event.EventCmd))
                    {
                        break;
                    }

                    {
                        var withBlock49 = Event.EventCmd[Event.CurrentLineNum];
                        if (cur_depth == Event.CallDepth & withBlock49.Name == Event.CmdType.ReturnCmd)
                        {
                            break;
                        }

                        ret = withBlock49.Exec();
                    }
                }
                while (ret > 0);

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
                    CallFunctionRet = ValueType.NumericType;
                }
                else
                {
                    CallFunctionRet = ValueType.StringType;
                }

                return CallFunctionRet;
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
    }
}
