using SRCCore.Lib;

namespace SRCCore.Expressions.Functions
{
    public class Font : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Font
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


    public class RGB : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Rgb
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

    public class TextHeight : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Textheight
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


    public class TextWidth : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Textwidth
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