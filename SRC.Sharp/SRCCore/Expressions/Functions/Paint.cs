using SRCCore.Extensions;
using SRCCore.Lib;
using System.Drawing;

namespace SRCCore.Expressions.Functions
{
    //Font文字列描画のフォント設定を返す
    //RGB描画色を返す
    //TextHeight指定した文字列を描画した際の高さをピクセル数で返す
    //TextWidth指定した文字列を描画した際の幅をピクセル数で返す

    public class Font : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            switch (SRC.Expression.GetValueAsString(@params[1], is_term[1]) ?? "")
            {
                case "フォント名":
                    {
                        str_result = SRC.GUI.CurrentPaintFont.Name;
                        return ValueType.StringType;
                    }

                case "サイズ":
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

                case "太字":
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

                case "斜体":
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

                case "色":
                    {
                        str_result = SRC.GUI.CurrentPaintColor.ToHexString();
                        return ValueType.StringType;
                    }

                case "書き込み":
                    {
                        if (SRC.GUI.PermanentStringMode)
                        {
                            str_result = "背景";
                        }
                        else if (SRC.GUI.KeepStringMode)
                        {
                            str_result = "保持";
                        }
                        else
                        {
                            str_result = "通常";
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
            str_result = "#000000";
            num_result = 0d;
            try
            {
                str_result = Color.FromArgb(
                    SRC.Expression.GetValueAsLong(@params[1], is_term[1]),
                    SRC.Expression.GetValueAsLong(@params[2], is_term[2]),
                    SRC.Expression.GetValueAsLong(@params[3], is_term[3])
                    ).ToHexString();
            }
            catch
            {
                // ignore
            }
            return ValueType.StringType;
        }
    }

    public class TextHeight : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            num_result =SRC. GUI.MeasureString(SRC.Expression.GetValueAsString(@params[1], is_term[1])).Height;
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
            num_result = SRC.GUI.MeasureString(SRC.Expression.GetValueAsString(@params[1], is_term[1])).Width;
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