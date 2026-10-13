using SRCCore.Lib;
using System;
using System.Linq;

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
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
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
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
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
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
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
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
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
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            num_result = Enumerable.Range(1, pcount)
                .Select(x => SRC.Expression.GetValueAsDouble(@params[x], is_term[x]))
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
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            num_result = Enumerable.Range(1, pcount)
                .Select(x => SRC.Expression.GetValueAsDouble(@params[x], is_term[x]))
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
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            num_result = GeneralLib.Dice(SRC.Expression.GetValueAsLong(@params[1], is_term[1]));
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
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            return RoundFunctionHelper.Invoke(SRC, RoundFunctionHelper.RoundMode.Round, etype, @params, pcount, is_term, out str_result, out num_result);
        }
    }
    public class RoundUp : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            return RoundFunctionHelper.Invoke(SRC, RoundFunctionHelper.RoundMode.RoundUp, etype, @params, pcount, is_term, out str_result, out num_result);
        }
    }
    public class RoundDown : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            return RoundFunctionHelper.Invoke(SRC, RoundFunctionHelper.RoundMode.RoundDown, etype, @params, pcount, is_term, out str_result, out num_result);
        }
    }

    // Round、RoundUp、RoundDown関数の共通処理
    public static class RoundFunctionHelper
    {
        public enum RoundMode
        {
            Round,
            RoundUp,
            RoundDown,
        }

        public static ValueType Invoke(SRC SRC, RoundMode mode, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";

            var ldbl = SRC.Expression.GetValueAsDouble(@params[1], is_term[1]);
            var num2 = pcount == 1 ? 0 : SRC.Expression.GetValueAsLong(@params[2], is_term[2]);
            num_result = Calc(mode, ldbl, num2);

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

        // VB版と同様に Int(x * 10 ^ n) を求め、端数に応じて1を加える
        // （Roundは端数が0.5以上なら切り上げるため、負数の場合は -2.5 → -2 となる）
        public static double Calc(RoundMode mode, double ldbl, int num2)
        {
            var scale = Math.Pow(10, num2);
            if (scale == 0d)
            {
                return 0d;
            }
            var scaled = ldbl * scale;
            if (double.IsInfinity(scale) || double.IsInfinity(scaled) || double.IsNaN(scaled))
            {
                // 桁数が大きすぎる場合は丸めようがないのでそのまま返す
                return ldbl;
            }

            var num = Math.Floor(scaled);
            switch (mode)
            {
                case RoundMode.Round:
                    if (scaled - num >= 0.5d)
                    {
                        num = num + 1d;
                    }
                    break;
                case RoundMode.RoundUp:
                    if (scaled - num > 0d)
                    {
                        num = num + 1d;
                    }
                    break;
            }
            return num / scale;
        }
    }
    public class Sin : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
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
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
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
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
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
