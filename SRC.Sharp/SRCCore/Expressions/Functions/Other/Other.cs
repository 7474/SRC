using SRCCore.Lib;
using SRCCore.VB;

namespace SRCCore.Expressions.Functions
{
    public class Count : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            var buf = @params[1] + "[";
            var num = 0;

            // サブルーチンローカル変数を検索
            if (SRC.Event.CallDepth > 0)
            {
                var loopTo5 = SRC.Event.VarIndex;
                for (var i = (SRC.Event.VarIndexStack[SRC.Event.CallDepth - 1] + 1); i <= loopTo5; i++)
                {
                    if (Strings.InStr(SRC.Event.VarStack[i].Name, buf) == 1)
                    {
                        num = (num + 1);
                    }
                }

                if (num > 0)
                {
                    if (etype == ValueType.StringType)
                    {
                        str_result = GeneralLib.FormatNum(num);
                        return ValueType.StringType;
                    }
                    else
                    {
                        num_result = num;
                        return ValueType.NumericType;
                    }
                }
            }

            // ローカル変数を検索
            foreach (VarData currentVar in SRC.Event.LocalVariableList.Values)
            {
                if (Strings.InStr(currentVar.Name, buf) == 1)
                {
                    num = (num + 1);
                }
            }

            if (num > 0)
            {
                if (etype == ValueType.StringType)
                {
                    str_result = GeneralLib.FormatNum(num);
                    return ValueType.StringType;
                }
                else
                {
                    num_result = num;
                    return ValueType.NumericType;
                }
            }

            // グローバル変数を検索
            foreach (VarData currentVar1 in SRC.Event.GlobalVariableList.Values)
            {
                if (Strings.InStr(currentVar1.Name, buf) == 1)
                {
                    num = (num + 1);
                }
            }

            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num);
                return ValueType.StringType;
            }
            else
            {
                num_result = num;
                return ValueType.NumericType;
            }
        }
    }

    public class Eval : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            var buf = Strings.Trim(SRC.Expression.GetValueAsString(@params[1], is_term[1]));
            return SRC.Expression.EvalExpr(buf, etype, out str_result, out num_result);
        }
    }

    public class IIf : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            var list = GeneralLib.ToList(@params[1]);
            var num = list.Count;
            var flag = false;
            // XXX このSwitch何を意図したものなのか分からん。。。
            switch (num)
            {
                case 1:
                    {
                        if (SRC.PList.IsDefined(list[0]))
                        {
                            {
                                var withBlock17 = SRC.PList.Item(list[0]);
                                if (withBlock17.Unit is null)
                                {
                                    flag = false;
                                }
                                else
                                {
                                    {
                                        var withBlock18 = withBlock17.Unit;
                                        if (withBlock18.Status == "出撃" || withBlock18.Status == "格納")
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
                        else if (SRC.Expression.GetValueAsLong(@params[1]) != 0)
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
                        if (SRC.PList.IsDefined(list[1]))
                        {
                            var withBlock19 = SRC.PList.Item(list[1]);
                            if (withBlock19.Unit is null)
                            {
                                flag = true;
                            }
                            else
                            {
                                {
                                    var withBlock20 = withBlock19.Unit;
                                    if (withBlock20.Status == "出撃" || withBlock20.Status == "格納")
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
                        else if (SRC.Expression.GetValueAsLong(@params[1], true) == 0)
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
                        if (SRC.Expression.GetValueAsLong(@params[1]) != 0)
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
                str_result = SRC.Expression.GetValueAsString(@params[2], is_term[2]);
            }
            else
            {
                str_result = SRC.Expression.GetValueAsString(@params[3], is_term[3]);
            }

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

    public class IsDefined : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            //                        pname = GetValueAsString(@params[1], is_term[1]);
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    switch (GetValueAsString(@params[2], is_term[2]) ?? "")
            //                                    {
            //                                        case "パイロット":
            //                                            {
            //                                                if (SRC.PList.IsDefined(pname))
            //                                                {
            //                                                    Pilot localItem4() { object argIndex1 = pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                                                    if (localItem4().Alive)
            //                                                    {
            //                                                        num_result = 1d;
            //                                                    }
            //                                                }

            //                                                break;
            //                                            }

            //                                        case "ユニット":
            //                                            {
            //                                                if (SRC.UList.IsDefined(pname))
            //                                                {
            //                                                    Unit localItem5() { object argIndex1 = pname; var ret = SRC.UList.Item(argIndex1); return ret; }

            //                                                    if (localItem5().Status != "破棄")
            //                                                    {
            //                                                        num_result = 1d;
            //                                                    }
            //                                                }

            //                                                break;
            //                                            }

            //                                        case "アイテム":
            //                                            {
            //                                                if (SRC.IList.IsDefined(pname))
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
            //                                    bool localIsDefined8() { object argIndex1 = pname; var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                                    bool localIsDefined9() { object argIndex1 = pname; var ret = SRC.IList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.PList.IsDefined(pname))
            //                                    {
            //                                        Pilot localItem6() { object argIndex1 = pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                                        if (localItem6().Alive)
            //                                        {
            //                                            num_result = 1d;
            //                                        }
            //                                    }
            //                                    else if (localIsDefined8())
            //                                    {
            //                                        Unit localItem7() { object argIndex1 = pname; var ret = SRC.UList.Item(argIndex1); return ret; }

            //                                        if (localItem7().Status != "破棄")
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


            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class IsVarDefined : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Isvardefined
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


            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class KeyState : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Keystate
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

            //                        if (i == Keys.LButton || i == Keys.RButton)
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


            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class Nickname : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Nickname
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    buf = GetValueAsString(@params[1], is_term[1]);
            //                                    bool localIsDefined15() { object argIndex1 = buf; var ret = SRC.PDList.IsDefined(argIndex1); return ret; }

            //                                    bool localIsDefined16() { object argIndex1 = buf; var ret = SRC.NPDList.IsDefined(argIndex1); return ret; }

            //                                    bool localIsDefined17() { object argIndex1 = buf; var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                                    bool localIsDefined18() { object argIndex1 = buf; var ret = SRC.UDList.IsDefined(argIndex1); return ret; }

            //                                    bool localIsDefined19() { object argIndex1 = buf; var ret = SRC.IDList.IsDefined(argIndex1); return ret; }

            //                                    if (SRC.PList.IsDefined(buf))
            //                                    {
            //                                        Pilot localItem31() { object argIndex1 = buf; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                                        str_result = localItem31().get_Nickname(false);
            //                                    }
            //                                    else if (localIsDefined15())
            //                                    {
            //                                        PilotData localItem32() { object argIndex1 = buf; var ret = SRC.PDList.Item(argIndex1); return ret; }

            //                                        str_result = localItem32().Nickname;
            //                                    }
            //                                    else if (localIsDefined16())
            //                                    {
            //                                        NonPilotData localItem33() { object argIndex1 = buf; var ret = SRC.NPDList.Item(argIndex1); return ret; }

            //                                        str_result = localItem33().Nickname;
            //                                    }
            //                                    else if (localIsDefined17())
            //                                    {
            //                                        Unit localItem34() { object argIndex1 = buf; var ret = SRC.UList.Item(argIndex1); return ret; }

            //                                        str_result = localItem34().Nickname0;
            //                                    }
            //                                    else if (localIsDefined18())
            //                                    {
            //                                        UnitData localItem35() { object argIndex1 = buf; var ret = SRC.UDList.Item(argIndex1); return ret; }

            //                                        str_result = localItem35().Nickname;
            //                                    }
            //                                    else if (localIsDefined19())
            //                                    {
            //                                        ItemData localItem36() { object argIndex1 = buf; var ret = SRC.IDList.Item(argIndex1); return ret; }

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

            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class Term : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Term
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    pname = GetValueAsString(@params[2], is_term[2]);
            //                                    if (SRC.UList.IsDefined2(pname))
            //                                    {
            //                                        Unit localItem217() { object argIndex1 = pname; var ret = SRC.UList.Item2(argIndex1); return ret; }

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

            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }
}