using SRCCore.Lib;
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

            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    buf = GetValueAsString(@params[1], is_term[1]);
            //                                    if (Information.IsDate(buf))
            //                                    {
            //                                        num_result = (double)DateAndTime.Year(Conversions.ToDate(buf));
            //                                    }
            //                                    else
            //                                    {
            //                                        num_result = 0d;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    num_result = (double)DateAndTime.Year(DateAndTime.Now);
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

    public class Month : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Month
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    buf = GetValueAsString(@params[1], is_term[1]);
            //                                    if (Information.IsDate(buf))
            //                                    {
            //                                        num_result = (double)DateAndTime.Month(Conversions.ToDate(buf));
            //                                    }
            //                                    else
            //                                    {
            //                                        num_result = 0d;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    num_result = (double)DateAndTime.Month(DateAndTime.Now);
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

    public class Weekday : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Weekday
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    buf = GetValueAsString(@params[1], is_term[1]);
            //                                    if (Information.IsDate(buf))
            //                                    {
            //                                        switch (DateAndTime.Weekday(Conversions.ToDate(buf)))
            //                                        {
            //                                            case FirstDayOfWeek.Sunday:
            //                                                {
            //                                                    str_result = "日曜";
            //                                                    break;
            //                                                }

            //                                            case FirstDayOfWeek.Monday:
            //                                                {
            //                                                    str_result = "月曜";
            //                                                    break;
            //                                                }

            //                                            case FirstDayOfWeek.Tuesday:
            //                                                {
            //                                                    str_result = "火曜";
            //                                                    break;
            //                                                }

            //                                            case FirstDayOfWeek.Wednesday:
            //                                                {
            //                                                    str_result = "水曜";
            //                                                    break;
            //                                                }

            //                                            case FirstDayOfWeek.Thursday:
            //                                                {
            //                                                    str_result = "木曜";
            //                                                    break;
            //                                                }

            //                                            case FirstDayOfWeek.Friday:
            //                                                {
            //                                                    str_result = "金曜";
            //                                                    break;
            //                                                }

            //                                            case FirstDayOfWeek.Saturday:
            //                                                {
            //                                                    str_result = "土曜";
            //                                                    break;
            //                                                }
            //                                        }
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    switch (DateAndTime.Weekday(DateAndTime.Now))
            //                                    {
            //                                        case FirstDayOfWeek.Sunday:
            //                                            {
            //                                                str_result = "日曜";
            //                                                break;
            //                                            }

            //                                        case FirstDayOfWeek.Monday:
            //                                            {
            //                                                str_result = "月曜";
            //                                                break;
            //                                            }

            //                                        case FirstDayOfWeek.Tuesday:
            //                                            {
            //                                                str_result = "火曜";
            //                                                break;
            //                                            }

            //                                        case FirstDayOfWeek.Wednesday:
            //                                            {
            //                                                str_result = "水曜";
            //                                                break;
            //                                            }

            //                                        case FirstDayOfWeek.Thursday:
            //                                            {
            //                                                str_result = "木曜";
            //                                                break;
            //                                            }

            //                                        case FirstDayOfWeek.Friday:
            //                                            {
            //                                                str_result = "金曜";
            //                                                break;
            //                                            }

            //                                        case FirstDayOfWeek.Saturday:
            //                                            {
            //                                                str_result = "土曜";
            //                                                break;
            //                                            }
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

    public class Day : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Day
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    buf = GetValueAsString(@params[1], is_term[1]);
            //                                    if (Information.IsDate(buf))
            //                                    {
            //                                        num_result = (double)DateAndTime.Day(Conversions.ToDate(buf));
            //                                    }
            //                                    else
            //                                    {
            //                                        num_result = 0d;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    num_result = (double)DateAndTime.Day(DateAndTime.Now);
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

    public class Hour : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Hour
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    buf = GetValueAsString(@params[1], is_term[1]);
            //                                    if (Information.IsDate(buf))
            //                                    {
            //                                        num_result = (double)DateAndTime.Hour(Conversions.ToDate(buf));
            //                                    }
            //                                    else
            //                                    {
            //                                        num_result = 0d;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    num_result = (double)DateAndTime.Hour(DateAndTime.Now);
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

    public class Minute : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Minute
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    buf = GetValueAsString(@params[1], is_term[1]);
            //                                    if (Information.IsDate(buf))
            //                                    {
            //                                        num_result = (double)DateAndTime.Minute(Conversions.ToDate(buf));
            //                                    }
            //                                    else
            //                                    {
            //                                        num_result = 0d;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    num_result = (double)DateAndTime.Minute(DateAndTime.Now);
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

    public class Second : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Second
            //                        switch (pcount)
            //                        {
            //                            case 1:
            //                                {
            //                                    buf = GetValueAsString(@params[1], is_term[1]);
            //                                    if (Information.IsDate(buf))
            //                                    {
            //                                        num_result = (double)DateAndTime.Second(Conversions.ToDate(buf));
            //                                    }
            //                                    else
            //                                    {
            //                                        num_result = 0d;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    num_result = (double)DateAndTime.Second(DateAndTime.Now);
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

    public class Difftime : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Difftime
            //                        if (pcount == 2)
            //                        {
            //                            if (@params[1] == "Now")
            //                            {
            //                                d1 = DateAndTime.Now;
            //                            }
            //                            else
            //                            {
            //                                buf = GetValueAsString(@params[1], is_term[1]);
            //                                if (!Information.IsDate(buf))
            //                                {
            //                                    return CallFunctionRet;
            //                                }

            //                                d1 = Conversions.ToDate(buf);
            //                            }

            //                            if (@params[2] == "Now")
            //                            {
            //                                d2 = DateAndTime.Now;
            //                            }
            //                            else
            //                            {
            //                                buf = GetValueAsString(@params[2], is_term[2]);
            //                                if (!Information.IsDate(buf))
            //                                {
            //                                    return CallFunctionRet;
            //                                }

            //                                d2 = Conversions.ToDate(buf);
            //                            }

            //                            num_result = (double)DateAndTime.Second(DateTime.FromOADate(d2.ToOADate() - d1.ToOADate()));
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
}
