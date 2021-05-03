using SRCCore.Lib;

namespace SRCCore.Expressions.Functions
{
    public class Count : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

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

    public class Iif : AFunction
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

    public class Isdefined : AFunction
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

    public class Isvardefined : AFunction
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

    public class Keystate : AFunction
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