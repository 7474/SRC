using SRCCore.Extensions;
using SRCCore.Lib;

namespace SRCCore.Expressions.Functions
{
    public class Font : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            switch (SRC.Expression.GetValueAsString(@params[1], is_term[1]) ?? "")
            {
                case "�t�H���g��":
                    {
                        str_result = SRC.GUI.CurrentPaintFont.Name;
                        return ValueType.StringType;
                    }

                case "�T�C�Y":
                    {
                        num_result = SRC.GUI.CurrentPaintFont.Size;
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

                case "����":
                    {
                        if (SRC.GUI.CurrentPaintFont.Bold)
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
                            return ValueType.StringType;
                        }
                        else
                        {
                            return ValueType.NumericType;
                        }
                    }

                case "�Α�":
                    {
                        if (SRC.GUI.CurrentPaintFont.Italic)
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
                            return ValueType.StringType;
                        }
                        else
                        {
                            return ValueType.NumericType;
                        }
                    }

                case "�F":
                    {
                        str_result = SRC.GUI.CurrentPaintColor.ToHexString();
                        return ValueType.StringType;
                    }

                case "��������":
                    {
                        if (SRC.GUI.PermanentStringMode)
                        {
                            str_result = "�w�i";
                        }
                        else if (SRC.GUI.KeepStringMode)
                        {
                            str_result = "�ێ�";
                        }
                        else
                        {
                            str_result = "�ʏ�";
                        }
                        return ValueType.StringType;
                    }
            }
            return ValueType.UndefinedType;
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