using SRCCore.Extensions;
using SRCCore.Lib;
using System.Drawing;

namespace SRCCore.Expressions.Functions
{
    //Font������`��̃t�H���g�ݒ��Ԃ�
    //RGB�`��F��Ԃ�
    //TextHeight�w�肵���������`�悵���ۂ̍������s�N�Z�����ŕԂ�
    //TextWidth�w�肵���������`�悵���ۂ̕����s�N�Z�����ŕԂ�

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