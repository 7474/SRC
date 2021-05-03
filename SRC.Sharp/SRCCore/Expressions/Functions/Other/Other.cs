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
            str_result = "";
            num_result = 0d;

            // TODO Impl Eval

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

    public class IIf : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Iif

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

    public class IsDefined : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Isdefined

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