using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Linq;

namespace SRCCore.Expressions.Functions
{
    //Year時間データの年度を返す
    //Month時間データの月を返す
    //Weekday時間データの曜日を返す
    //Day時間データの日を返す
    //Hour時間データの時刻を返す
    //Minute時間データの分を返す
    //Second時間データの秒を返す
    //DiffTime２つの時間データの差を秒で返す
    //GetTimeＰＣが起動してからの時間を秒で返す

    public class Year : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            switch (pcount)
            {
                case 1:
                    {
                        var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                        DateTime dt;
                        if (Conversions.TryToDateTime(buf, out dt))
                        {
                            num_result = dt.Year;
                        }
                        else
                        {
                            num_result = 0d;
                        }

                        break;
                    }

                case 0:
                    {
                        num_result = Conversions.GetNow().Year;
                        break;
                    }
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
    }

    public class Month : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            switch (pcount)
            {
                case 1:
                    {
                        var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                        DateTime dt;
                        if (Conversions.TryToDateTime(buf, out dt))
                        {
                            num_result = dt.Month;
                        }
                        else
                        {
                            num_result = 0d;
                        }

                        break;
                    }

                case 0:
                    {
                        num_result = Conversions.GetNow().Month;
                        break;
                    }
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
    }

    public class Weekday : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            DayOfWeek weekday;
            switch (pcount)
            {
                case 1:
                    {
                        var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                        DateTime dt;
                        if (Conversions.TryToDateTime(buf, out dt))
                        {
                            weekday = dt.DayOfWeek;
                        }
                        else
                        {
                            return ValueType.StringType;
                        }

                        break;
                    }

                default:
                    {
                        weekday = Conversions.GetNow().DayOfWeek;
                        break;
                    }
            }
            switch (weekday)
            {
                case DayOfWeek.Sunday: str_result = "日曜"; break;
                case DayOfWeek.Monday: str_result = "月曜"; break;
                case DayOfWeek.Tuesday: str_result = "火曜"; break;
                case DayOfWeek.Wednesday: str_result = "水曜"; break;
                case DayOfWeek.Thursday: str_result = "木曜"; break;
                case DayOfWeek.Friday: str_result = "金曜"; break;
                case DayOfWeek.Saturday: str_result = "土曜"; break;
            }
            return ValueType.StringType;
        }
    }

    public class Day : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            switch (pcount)
            {
                case 1:
                    {
                        var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                        DateTime dt;
                        if (Conversions.TryToDateTime(buf, out dt))
                        {
                            num_result = dt.Day;
                        }
                        else
                        {
                            num_result = 0d;
                        }

                        break;
                    }

                case 0:
                    {
                        num_result = Conversions.GetNow().Day;
                        break;
                    }
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
    }

    public class Hour : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            switch (pcount)
            {
                case 1:
                    {
                        var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                        DateTime dt;
                        if (Conversions.TryToDateTime(buf, out dt))
                        {
                            num_result = dt.Hour;
                        }
                        else
                        {
                            num_result = 0d;
                        }

                        break;
                    }

                case 0:
                    {
                        num_result = Conversions.GetNow().Hour;
                        break;
                    }
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
    }

    public class Minute : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            switch (pcount)
            {
                case 1:
                    {
                        var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                        DateTime dt;
                        if (Conversions.TryToDateTime(buf, out dt))
                        {
                            num_result = dt.Minute;
                        }
                        else
                        {
                            num_result = 0d;
                        }

                        break;
                    }

                case 0:
                    {
                        num_result = Conversions.GetNow().Minute;
                        break;
                    }
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
    }

    public class Second : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            switch (pcount)
            {
                case 1:
                    {
                        var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                        DateTime dt;
                        if (Conversions.TryToDateTime(buf, out dt))
                        {
                            num_result = dt.Second;
                        }
                        else
                        {
                            num_result = 0d;
                        }

                        break;
                    }

                case 0:
                    {
                        num_result = Conversions.GetNow().Second;
                        break;
                    }
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
    }

    public class DiffTime : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            DateTime d1;
            DateTime d2;
            if (pcount == 2)
            {
                if (@params[1] == "Now")
                {
                    d1 = Conversions.GetNow();
                }
                else
                {
                    var buf = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                    if (!Conversions.TryToDateTime(buf, out d1))
                    {
                        return ValueType.UndefinedType;
                    }
                }

                if (@params[2] == "Now")
                {
                    d2 = Conversions.GetNow();
                }
                else
                {
                    var buf = SRC.Expression.GetValueAsString(@params[2], is_term[2]);
                    if (!Conversions.TryToDateTime(buf, out d2))
                    {
                        return ValueType.UndefinedType;
                    }
                }

                num_result = d2.Second - d1.Second;
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
    }

    public class GetTime : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            num_result = GeneralLib.timeGetTime();
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
