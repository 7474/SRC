using SRCCore.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRCCore.Expressions.Functions
{
    // Abs(数値)
    // Atn(数値)
    // Cos(数値)
    // Int(数値)
    // Max(値１, 値２, 値３, …)
    // Min(値１, 値２, 値３, …)
    // Random(数値)
    // Round(数値, 桁数)
    // RoundUp(数値, 桁数)
    // RoundDown(数値, 桁数)
    // Sin(数値)
    // Sqr(数値)
    // Tan(数値)

    public class Abs : AFunction
    {
        public override ValueType Invoke(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            num_result = Math.Abs(SRC.Expression.GetValueAsDouble(@params[1], is_term[1]));
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
    public class Atn : AFunction
    {
        public override ValueType Invoke(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            num_result = Math.Atan(SRC.Expression.GetValueAsDouble(@params[1], is_term[1]));
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
    public class Cos : AFunction
    {
        public override ValueType Invoke(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            num_result = Math.Cos(SRC.Expression.GetValueAsDouble(@params[1], is_term[1]));
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
    public class Int : AFunction
    {
        public override ValueType Invoke(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            num_result = Math.Floor(SRC.Expression.GetValueAsDouble(@params[1], is_term[1]));
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
    public class Max : AFunction
    {
        public override ValueType Invoke(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            num_result = Enumerable.Range(1, pcount)
                .Select(x => SRC.Expression.GetValueAsDouble(@params[1], is_term[1]))
                .Max();
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
    public class Min : AFunction
    {
        public override ValueType Invoke(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            num_result = Enumerable.Range(1, pcount)
                .Select(x => SRC.Expression.GetValueAsDouble(@params[1], is_term[1]))
                .Min();
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
    public class Random : AFunction
    {
        public override ValueType Invoke(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            num_result = GeneralLib.Dice((int)SRC.Expression.GetValueAsDouble(@params[1], is_term[1]));
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
    public class Round : AFunction
    {
        public override ValueType Invoke(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            num_result = Math.Round(
                    (double)SRC.Expression.GetValueAsDouble(@params[1], is_term[1]),
                    (int)SRC.Expression.GetValueAsDouble(@params[2], is_term[2]),
                    MidpointRounding.AwayFromZero);
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
    public class RoundUp : AFunction
    {
        public override ValueType Invoke(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            var num1 = SRC.Expression.GetValueAsDouble(@params[1], is_term[1]);
            var num2 = SRC.Expression.GetValueAsDouble(@params[2], is_term[2]);
            num_result = (double)(((decimal)Math.Ceiling(Math.Pow(10, num2) * num1)) / (decimal)Math.Pow(10, num2));
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
    public class RoundDown : AFunction
    {
        public override ValueType Invoke(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            var num1 = SRC.Expression.GetValueAsDouble(@params[1], is_term[1]);
            var num2 = SRC.Expression.GetValueAsDouble(@params[2], is_term[2]);
            num_result = (double)(((decimal)Math.Floor(Math.Pow(10, num2) * num1)) / (decimal)Math.Pow(10, num2));
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
    public class Sin : AFunction
    {
        public override ValueType Invoke(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            num_result = Math.Sin(SRC.Expression.GetValueAsDouble(@params[1], is_term[1]));
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
    public class Sqr : AFunction
    {
        public override ValueType Invoke(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            num_result = Math.Sqrt(SRC.Expression.GetValueAsDouble(@params[1], is_term[1]));
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
    public class Tan : AFunction
    {
        public override ValueType Invoke(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            num_result = Math.Tan(SRC.Expression.GetValueAsDouble(@params[1], is_term[1]));
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
