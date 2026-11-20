using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SRCCore.VB
{
    /// <summary>
    /// VB5 の Format$ の、文字列を引数に取る形（元の SRC の Format 関数。Expression.bas の CallFunction の "format" は
    /// Format$(GetValueAsString(引数 1), GetValueAsString(引数 2)) を呼ぶ）。
    /// 日本語版の Windows の地域の設定（日本）で動いたときの結果を返す。実行する環境の言語によらない。
    ///
    /// 書式の解釈は Microsoft の Format 関数の資料による。
    /// https://learn.microsoft.com/ja-jp/office/vba/language/reference/user-interface-help/format-function-visual-basic-for-applications
    /// - 書式の名前（General Number・Currency・Fixed・Standard・Percent・Scientific・Yes/No・True/False・On/Off、
    ///   General Date・Long Date・Medium Date・Short Date・Long Time・Medium Time・Short Time）。大文字・小文字は区別しない
    /// - ユーザー定義の書式は、最初に現れた書式文字で種類（数値・日付・文字列）が決まる。
    ///   数値の書式は 0・#・.、日付の書式は /・:・c・d・h・m・n・q・s・ttttt・w・y・AM/PM などと、日本語版の
    ///   aaa・aaaa（曜日）・g・gg・ggg（年号）・e・ee（和暦の年）。文字列の書式は @・&amp;・&lt;・&gt;・!。
    ///   種類が決まった後の、ほかの種類の書式文字は文字のまま書く
    /// - 文字列の引数は、書式の種類に合わせて数値・日付に変換する。変換できなければ、文字列をそのまま返す
    ///
    /// 資料に無い細部は、日本語版の Windows の元の SRC の実機の出力に合わせた（実機との比較 #1550。SRC.Sharp/Compat/PROBE.md）:
    /// Currency の通貨記号（Shift_JIS の 0x5C。SRC# では「\」）と負の数の形（-\1,235）、Medium Date（yy-mm-dd）、
    /// Medium Time（hh:mm 午前/午後）、Long Time などの時刻の終わりの空白、Yes/No・True/False・On/Off の文言（英語）、
    /// ddd・dddd・mmm・mmmm（英語）、和暦（令和は無い）、空の書式、日付の書式の中の 0・#（エラー）、
    /// 書式の種類の決め方（OLE Automation の VarFormat の実装として Wine の varformat.c を参考にした）。
    /// </summary>
    public static class VbFormat
    {
        private enum FormatType
        {
            Unknown,
            Number,
            Date,
            String,
        }

        // 日本語版の Windows の地域の設定（日本）
        private const string ShortDatePattern = "yyyy/MM/dd";
        private const string LongDatePattern = "yyyy年M月d日";
        private const string MediumDatePattern = "yy-MM-dd";   // 元の SRC の実機（#1550）
        private const string LongTimePattern = "H:mm:ss";
        private const string CurrencySymbol = "\\";            // 推定（Shift_JIS の 0x5C。円記号の字形）
        private const string AMDesignator = "午前";
        private const string PMDesignator = "午後";
        private static readonly string[] DayNamesShort = { "日", "月", "火", "水", "木", "金", "土" };
        private static readonly string[] DayNames = { "日曜日", "月曜日", "火曜日", "水曜日", "木曜日", "金曜日", "土曜日" };
        // ddd・dddd・mmm・mmmm は、日本語版の Windows でも英語の名前（元の SRC の実機。#1550）。aaa・aaaa は日本語
        private static readonly string[] DayNamesShortEnglish = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        private static readonly string[] DayNamesEnglish = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
        private static readonly string[] MonthNamesShortEnglish = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        private static readonly string[] MonthNamesEnglish = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

        // 和暦の年号（開始日・頭文字・漢字の 1 文字・名前）。
        // VB5 のランタイムは令和を知らない（元の SRC の実機で 2019/05/01 は「平成31年」。#1550）
        private static readonly (DateTime start, string initial, string shortName, string name)[] Eras =
        {
            (new DateTime(1989, 1, 8), "H", "平", "平成"),
            (new DateTime(1926, 12, 25), "S", "昭", "昭和"),
            (new DateTime(1912, 7, 30), "T", "大", "大正"),
            (new DateTime(1868, 9, 8), "M", "明", "明治"),
        };

        // VB の日付の 0（CDate(0)）
        private static readonly DateTime VBDateZero = new DateTime(1899, 12, 30);

        public static string Format(string expression, string format)
        {
            expression = expression ?? "";
            format = format ?? "";

            // 書式が無ければ、数値の文字列は数値として書き（元の SRC の実機で Format("01.50","") は「1.5」。#1550）、
            // ほかの文字列はそのまま返す
            if (format.Length == 0)
            {
                return WithNumber(expression, x => x.ToString("G15", CultureInfo.InvariantCulture));
            }

            var named = FormatNamed(expression, format);
            if (named != null)
            {
                return named;
            }

            switch (DetectType(format))
            {
                case FormatType.String:
                    return FormatAsString(expression, format);
                case FormatType.Date:
                    // 日付の書式の中の数値の書式文字（0・#）はエラー（元の SRC の実機で Format(5,"No.0")・Format(5,"Level 0") は
                    // 実行時エラー。#1550）
                    if (HasNumberPlaceholder(format))
                    {
                        throw new ArgumentException("プロシージャの呼び出し、または引数が不正です。");
                    }
                    return TryToDate(expression, out var date) ? FormatDate(date, FirstSection(format)) : expression;
                default:
                    // 数値の書式。書式文字の無い書式（文字だけ）も、以前の SRC# と同じく数値なら .NET の書式で書く
                    // （SrcFormatter は .NET のカスタム数値書式。0・#・.・,・%・E+・;・\・引用符は VB と同じ意味）
                    return Information.IsNumeric(expression)
                        ? FormatNumber(Conversions.ToDouble(expression), format)
                        : expression;
            }
        }

        #region 書式の名前

        private static string FormatNamed(string expression, string format)
        {
            switch (format.ToLowerInvariant())
            {
                case "general number":
                    return WithNumber(expression, x => x.ToString("G15", CultureInfo.InvariantCulture));
                case "currency":
                    // 日本の地域の設定: 通貨の小数部 0 桁・桁区切りあり・負の数は記号の前に -
                    return WithNumber(expression, x =>
                    {
                        var s = Math.Abs(x).ToString("#,##0", CultureInfo.InvariantCulture);
                        return (x < 0 && s != "0" ? "-" : "") + CurrencySymbol + s;
                    });
                case "fixed":
                    return WithNumber(expression, x => SrcFormatter.Format(x, "0.00"));
                case "standard":
                    return WithNumber(expression, x => SrcFormatter.Format(x, "#,##0.00"));
                case "percent":
                    return WithNumber(expression, x => SrcFormatter.Format(x, "0.00%"));
                case "scientific":
                    return WithNumber(expression, x => SrcFormatter.Format(x, "0.00E+00"));
                case "yes/no":
                    return WithNumber(expression, x => x != 0d ? "Yes" : "No");
                case "true/false":
                    return WithNumber(expression, x => x != 0d ? "True" : "False");
                case "on/off":
                    return WithNumber(expression, x => x != 0d ? "On" : "Off");
                case "general date":
                    return WithDate(expression, GeneralDate);
                case "long date":
                    return WithDate(expression, x => x.ToString(LongDatePattern, CultureInfo.InvariantCulture));
                case "medium date":
                    return WithDate(expression, x => x.ToString(MediumDatePattern, CultureInfo.InvariantCulture));
                case "short date":
                    return WithDate(expression, x => x.ToString(ShortDatePattern, CultureInfo.InvariantCulture));
                case "long time":
                    return WithDate(expression, LongTime);
                case "medium time":
                    return WithDate(expression, x => Hour12(x).ToString("00", CultureInfo.InvariantCulture)
                        + ":" + x.Minute.ToString("00", CultureInfo.InvariantCulture)
                        + " " + (x.Hour < 12 ? AMDesignator : PMDesignator));
                case "short time":
                    return WithDate(expression, x => x.ToString("HH:mm", CultureInfo.InvariantCulture));
                default:
                    return null;
            }
        }

        // ユーザー定義の数値の書式。負の数が丸めて 0 になるとき、VB は「-」を付けない
        // （元の SRC の実機で Format(-0.4,"0") は「0」。.NET は「-0」。#1550）
        private static string FormatNumber(double x, string format)
        {
            if (x < 0d && format.IndexOf(';') < 0)
            {
                var abs = SrcFormatter.Format(-x, format);
                var hasNonZeroDigit = false;
                foreach (var c in abs)
                {
                    if (c >= '1' && c <= '9')
                    {
                        hasNonZeroDigit = true;
                        break;
                    }
                }
                if (!hasNonZeroDigit)
                {
                    return SrcFormatter.Format(0d, format);
                }
            }
            return SrcFormatter.Format(x, format);
        }

        private static string WithNumber(string expression, Func<double, string> f)
        {
            return Information.IsNumeric(expression) ? f(Conversions.ToDouble(expression)) : expression;
        }

        private static string WithDate(string expression, Func<DateTime, string> f)
        {
            return TryToDate(expression, out var date) ? f(date) : expression;
        }

        // General Date・c: 時刻が 0 なら日付だけ、日付が 0（1899/12/30）なら時刻だけ
        private static string GeneralDate(DateTime x)
        {
            if (x.TimeOfDay == TimeSpan.Zero)
            {
                return x.ToString(ShortDatePattern, CultureInfo.InvariantCulture);
            }
            if (x.Date == VBDateZero)
            {
                return LongTime(x);
            }
            return x.ToString(ShortDatePattern, CultureInfo.InvariantCulture) + " " + LongTime(x);
        }

        // Long Time・ttttt・General Date の時刻。日本語版の Windows の元の SRC の実機では、終わりに空白が 1 つ付く（「13:04:05 」。#1550）
        private static string LongTime(DateTime x)
        {
            return x.ToString(LongTimePattern, CultureInfo.InvariantCulture) + " ";
        }

        // 書式（引用符の中と \ の次の 1 文字を除く）に数値の書式文字 0・# があるか
        private static bool HasNumberPlaceholder(string format)
        {
            for (var i = 0; i < format.Length; i++)
            {
                var c = format[i];
                if (c == '\\')
                {
                    i++;
                }
                else if (c == '"')
                {
                    var end = format.IndexOf('"', i + 1);
                    i = end < 0 ? format.Length : end;
                }
                else if (c == '0' || c == '#')
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region 書式の種類

        // 最初に現れた書式文字で、書式の種類を決める（引用符の中と \ の次の 1 文字は文字）
        private static FormatType DetectType(string format)
        {
            for (var i = 0; i < format.Length; i++)
            {
                var c = format[i];
                if (c == '\\')
                {
                    i++;
                    continue;
                }
                if (c == '"')
                {
                    var end = format.IndexOf('"', i + 1);
                    i = end < 0 ? format.Length : end;
                    continue;
                }
                switch (c)
                {
                    case '0':
                    case '#':
                    case '.':
                        return FormatType.Number;
                    case '@':
                    case '&':
                    case '<':
                    case '>':
                    case '!':
                        return FormatType.String;
                    case '/':
                    case ':':
                        return FormatType.Date;
                }
                switch (char.ToLowerInvariant(c))
                {
                    case 'c':
                    case 'd':
                    case 'h':
                    case 'm':
                    case 'n':
                    case 'q':
                    case 's':
                    case 'w':
                    case 'y':
                    case 'g':
                    case 'e':
                        return FormatType.Date;
                    case 't':
                        if (StartsWithIgnoreCase(format, i, "ttttt"))
                        {
                            return FormatType.Date;
                        }
                        break;
                    case 'a':
                        if (AmPmTokenLength(format, i) > 0 || StartsWithIgnoreCase(format, i, "aaa"))
                        {
                            return FormatType.Date;
                        }
                        break;
                }
            }
            return FormatType.Unknown;
        }

        private static string FirstSection(string format)
        {
            for (var i = 0; i < format.Length; i++)
            {
                var c = format[i];
                if (c == '\\')
                {
                    i++;
                }
                else if (c == '"')
                {
                    var end = format.IndexOf('"', i + 1);
                    i = end < 0 ? format.Length : end;
                }
                else if (c == ';')
                {
                    return format.Substring(0, i);
                }
            }
            return format;
        }

        private static bool StartsWithIgnoreCase(string s, int index, string value)
        {
            return index + value.Length <= s.Length
                && string.Compare(s, index, value, 0, value.Length, StringComparison.OrdinalIgnoreCase) == 0;
        }

        private static bool StartsWithOrdinal(string s, int index, string value)
        {
            return index + value.Length <= s.Length && string.CompareOrdinal(s, index, value, 0, value.Length) == 0;
        }

        // AMPM（大文字・小文字を問わない）・AM/PM・am/pm・A/P・a/p の長さ。当たらなければ 0
        private static int AmPmTokenLength(string format, int i)
        {
            if (StartsWithIgnoreCase(format, i, "ampm"))
            {
                return 4;
            }
            if (StartsWithOrdinal(format, i, "AM/PM") || StartsWithOrdinal(format, i, "am/pm"))
            {
                return 5;
            }
            if ((format[i] == 'A' || format[i] == 'a') && i + 2 < format.Length && format[i + 1] == '/'
                && (format[i + 2] == 'P' || format[i + 2] == 'p'))
            {
                return 3;
            }
            return 0;
        }

        #endregion

        #region 日付の書式

        private static bool TryToDate(string expression, out DateTime date)
        {
            // 数値の文字列は日付のシリアル値（VB の Date の内部の値）として読む
            if (Information.IsNumeric(expression))
            {
                var d = Conversions.ToDouble(expression);
                try
                {
                    date = DateTime.FromOADate(d);
                    return true;
                }
                catch (ArgumentException)
                {
                    date = default;
                    return false;
                }
            }
            return Conversions.TryToDateTime(expression, out date);
        }

        private static int Hour12(DateTime x)
        {
            var h = x.Hour % 12;
            return h == 0 ? 12 : h;
        }

        private static string FormatDate(DateTime x, string format)
        {
            // AM/PM などがあれば、時は 12 時間制で書く
            var use12 = false;
            for (var i = 0; i < format.Length; i++)
            {
                if (format[i] == '\\')
                {
                    i++;
                }
                else if (format[i] == '"')
                {
                    var end = format.IndexOf('"', i + 1);
                    i = end < 0 ? format.Length : end;
                }
                else if (AmPmTokenLength(format, i) > 0)
                {
                    use12 = true;
                    break;
                }
            }

            var sb = new StringBuilder();
            // h の後の最初の m・mm は分（d が来たら、また月）
            var seenHours = false;
            var wroteMinutes = false;
            var i2 = 0;
            while (i2 < format.Length)
            {
                var c = format[i2];
                var lc = char.ToLowerInvariant(c);
                if (c == '\\')
                {
                    if (i2 + 1 < format.Length)
                    {
                        sb.Append(format[i2 + 1]);
                    }
                    i2 += 2;
                    continue;
                }
                if (c == '"')
                {
                    var end = format.IndexOf('"', i2 + 1);
                    if (end < 0)
                    {
                        sb.Append(format, i2 + 1, format.Length - i2 - 1);
                        break;
                    }
                    sb.Append(format, i2 + 1, end - i2 - 1);
                    i2 = end + 1;
                    continue;
                }
                if (c == '/')
                {
                    sb.Append('/');
                    i2++;
                    continue;
                }
                if (c == ':')
                {
                    sb.Append(':');
                    i2++;
                    continue;
                }

                var ampm = AmPmTokenLength(format, i2);
                if (ampm > 0)
                {
                    var am = x.Hour < 12;
                    if (ampm == 4)
                    {
                        sb.Append(am ? AMDesignator : PMDesignator);
                    }
                    else if (ampm == 5)
                    {
                        sb.Append(c == 'A' ? (am ? "AM" : "PM") : (am ? "am" : "pm"));
                    }
                    else
                    {
                        sb.Append(c == 'A' ? (am ? "A" : "P") : (am ? "a" : "p"));
                    }
                    i2 += ampm;
                    continue;
                }

                switch (lc)
                {
                    case 'a':
                        if (StartsWithIgnoreCase(format, i2, "aaaa"))
                        {
                            sb.Append(DayNames[(int)x.DayOfWeek]);
                            i2 += 4;
                        }
                        else if (StartsWithIgnoreCase(format, i2, "aaa"))
                        {
                            sb.Append(DayNamesShort[(int)x.DayOfWeek]);
                            i2 += 3;
                        }
                        else
                        {
                            sb.Append(c);
                            i2++;
                        }
                        continue;
                    case 'c':
                        sb.Append(GeneralDate(x));
                        i2++;
                        continue;
                    case 'd':
                        {
                            var n = CountRun(format, i2, 'd', 6);
                            switch (n)
                            {
                                case 1: sb.Append(Num(x.Day)); break;
                                case 2: sb.Append(x.Day.ToString("00", CultureInfo.InvariantCulture)); break;
                                case 3: sb.Append(DayNamesShortEnglish[(int)x.DayOfWeek]); break;
                                case 4: sb.Append(DayNamesEnglish[(int)x.DayOfWeek]); break;
                                case 5: sb.Append(x.ToString(ShortDatePattern, CultureInfo.InvariantCulture)); break;
                                default: sb.Append(x.ToString(LongDatePattern, CultureInfo.InvariantCulture)); break;
                            }
                            seenHours = false;
                            i2 += n;
                            continue;
                        }
                    case 'h':
                        {
                            var n = CountRun(format, i2, 'h', 2);
                            var h = use12 ? Hour12(x) : x.Hour;
                            sb.Append(n == 2 ? h.ToString("00", CultureInfo.InvariantCulture) : h.ToString(CultureInfo.InvariantCulture));
                            seenHours = true;
                            i2 += n;
                            continue;
                        }
                    case 'm':
                        {
                            var n = CountRun(format, i2, 'm', 4);
                            if (n <= 2 && seenHours && !wroteMinutes)
                            {
                                sb.Append(n == 2 ? x.Minute.ToString("00", CultureInfo.InvariantCulture) : x.Minute.ToString(CultureInfo.InvariantCulture));
                                wroteMinutes = true;
                            }
                            else
                            {
                                switch (n)
                                {
                                    case 1: sb.Append(Num(x.Month)); break;
                                    case 2: sb.Append(x.Month.ToString("00", CultureInfo.InvariantCulture)); break;
                                    case 3: sb.Append(MonthNamesShortEnglish[x.Month - 1]); break;
                                    default: sb.Append(MonthNamesEnglish[x.Month - 1]); break;
                                }
                            }
                            i2 += n;
                            continue;
                        }
                    case 'n':
                        {
                            var n = CountRun(format, i2, 'n', 2);
                            sb.Append(n == 2 ? x.Minute.ToString("00", CultureInfo.InvariantCulture) : x.Minute.ToString(CultureInfo.InvariantCulture));
                            i2 += n;
                            continue;
                        }
                    case 's':
                        {
                            var n = CountRun(format, i2, 's', 2);
                            sb.Append(n == 2 ? x.Second.ToString("00", CultureInfo.InvariantCulture) : x.Second.ToString(CultureInfo.InvariantCulture));
                            i2 += n;
                            continue;
                        }
                    case 'q':
                        sb.Append(Num((x.Month - 1) / 3 + 1));
                        i2++;
                        continue;
                    case 'w':
                        {
                            var n = CountRun(format, i2, 'w', 2);
                            if (n == 2)
                            {
                                // 1 月 1 日を含む週を第 1 週とし、週は日曜日から始める（Format の既定）
                                var jan1 = new DateTime(x.Year, 1, 1);
                                sb.Append(Num((x.DayOfYear - 1 + (int)jan1.DayOfWeek) / 7 + 1));
                            }
                            else
                            {
                                sb.Append(Num((int)x.DayOfWeek + 1));
                            }
                            i2 += n;
                            continue;
                        }
                    case 'y':
                        {
                            var n = CountRun(format, i2, 'y', 4);
                            if (n == 3)
                            {
                                // yyy は yy と y
                                n = 2;
                            }
                            switch (n)
                            {
                                case 1: sb.Append(Num(x.DayOfYear)); break;
                                case 2: sb.Append((x.Year % 100).ToString("00", CultureInfo.InvariantCulture)); break;
                                default: sb.Append(Num(x.Year)); break;
                            }
                            i2 += n;
                            continue;
                        }
                    case 't':
                        if (StartsWithIgnoreCase(format, i2, "ttttt"))
                        {
                            sb.Append(LongTime(x));
                            i2 += 5;
                        }
                        else
                        {
                            sb.Append(c);
                            i2++;
                        }
                        continue;
                    case 'g':
                        {
                            var n = CountRun(format, i2, 'g', 3);
                            var era = FindEra(x);
                            if (era >= 0)
                            {
                                sb.Append(n == 1 ? Eras[era].initial : n == 2 ? Eras[era].shortName : Eras[era].name);
                            }
                            i2 += n;
                            continue;
                        }
                    case 'e':
                        {
                            var n = CountRun(format, i2, 'e', 2);
                            var era = FindEra(x);
                            var year = era >= 0 ? x.Year - Eras[era].start.Year + 1 : x.Year;
                            sb.Append(n == 2 ? year.ToString("00", CultureInfo.InvariantCulture) : year.ToString(CultureInfo.InvariantCulture));
                            i2 += n;
                            continue;
                        }
                    default:
                        sb.Append(c);
                        i2++;
                        continue;
                }
            }
            return sb.ToString();
        }

        private static string Num(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        private static int CountRun(string s, int index, char lower, int max)
        {
            var n = 0;
            while (index + n < s.Length && n < max && char.ToLowerInvariant(s[index + n]) == lower)
            {
                n++;
            }
            return n;
        }

        private static int FindEra(DateTime x)
        {
            for (var i = 0; i < Eras.Length; i++)
            {
                if (x.Date >= Eras[i].start)
                {
                    return i;
                }
            }
            return -1;
        }

        #endregion

        #region 文字列の書式

        private static string FormatAsString(string value, string format)
        {
            if (format.IndexOf('@') >= 0 || format.IndexOf('&') >= 0)
            {
                return FormatStringWithPlaceholder(value, format);
            }
            return ConvertCase(value, format);
        }

        private static string ConvertCase(string value, string format)
        {
            if (format.IndexOf('<') >= 0)
            {
                return value.ToLowerInvariant();
            }
            if (format.IndexOf('>') >= 0)
            {
                return value.ToUpperInvariant();
            }
            return value;
        }

        private static string FormatStringWithPlaceholder(string value, string format)
        {
            value = ConvertCase(value, format);
            var leftToRight = format.IndexOf('!') >= 0;
            var pattern = format.Replace("!", "").Replace("<", "").Replace(">", "");
            var result = new StringBuilder();
            if (leftToRight)
            {
                // 左から順に文字を割り当てる
                var idx = 0;
                foreach (var c in pattern)
                {
                    if (c == '@' || c == '&')
                    {
                        if (idx < value.Length)
                        {
                            result.Append(value[idx++]);
                        }
                        else if (c == '@')
                        {
                            result.Append(' ');
                        }
                    }
                    else
                    {
                        result.Append(c);
                    }
                }
                // 割り当てられなかった文字は末尾に付ける
                if (idx < value.Length)
                {
                    result.Append(value.Substring(idx));
                }
            }
            else
            {
                // 右から順に文字を割り当てる
                var idx = value.Length - 1;
                for (var k = pattern.Length - 1; k >= 0; k--)
                {
                    var c = pattern[k];
                    if (c == '@' || c == '&')
                    {
                        if (idx >= 0)
                        {
                            result.Insert(0, value[idx--]);
                        }
                        else if (c == '@')
                        {
                            result.Insert(0, ' ');
                        }
                    }
                    else
                    {
                        result.Insert(0, c);
                    }
                }
                // 割り当てられなかった文字は先頭に付ける
                if (idx >= 0)
                {
                    result.Insert(0, value.Substring(0, idx + 1));
                }
            }
            return result.ToString();
        }

        #endregion
    }
}
