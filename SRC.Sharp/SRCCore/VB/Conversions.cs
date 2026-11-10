using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SRCCore.VB
{
    // VBの Microsoft.VisualBasic.Conversions のうちSRCで使用していたものの仮実装。
    public static class Conversions
    {
        public static string ToString(object Value)
        {
            return Value?.ToString() ?? "";
        }

        // 数値の文字列として読む前に、VB（OLE Automation の VarParseNumFromStr）と同じく全角の英数字・記号・空白を半角にし、前後の空白を除く。
        // 日本語の Windows（DBCS のロケール）の VB では、IsNumeric・CLng・CInt・CDbl などが「全角の文字を半角にした写し」を読むため、
        // 「１４００」「　1400」「1400　」は 1400 として扱われる。途中の空白（「1400　1」）は数値ではない。
        // https://learn.microsoft.com/en-us/previous-versions/windows/embedded/ee488050(v=winembedded.80)
        internal static string NumericText(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }
            var chars = value.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                var c = chars[i];
                if (c >= '\uFF01' && c <= '\uFF5E')
                {
                    chars[i] = (char)(c - 0xFEE0);
                }
                else if (c == '\u3000')
                {
                    chars[i] = ' ';
                }
            }
            var text = new string(chars).Trim(' ', '\t', '\r', '\n');
            // 符号と数字のあいだの空白（「- 10」「+ 20」）は詰めて読む。
            // GSC データパックの武器データなどにこの書き方があり、データの実態として数値として扱う（7474/SRC#1413）
            if (text.Length > 1 && (text[0] == '+' || text[0] == '-') && (text[1] == ' ' || text[1] == '\t'))
            {
                var rest = text.Substring(1).TrimStart(' ', '\t');
                if (rest.Length > 0 && ((rest[0] >= '0' && rest[0] <= '9') || rest[0] == '.'))
                {
                    text = text[0] + rest;
                }
            }
            return text;
        }

        public static int ToInteger(string Value)
        {
            Value = NumericText(Value);
            if (int.TryParse(Value, out int ret))
            {
                return ret;
            }
            return ToInteger(ToDouble(Value));
        }

        // VB の CLng や Long への代入と同様に偶数丸め（銀行型丸め）で整数に変換する。
        // 範囲外の値は Integer の範囲に収める（VBではオーバーフローエラー）。
        public static int ToInteger(double Value)
        {
            if (double.IsNaN(Value))
            {
                return 0;
            }
            var rounded = Math.Round(Value, MidpointRounding.ToEven);
            if (rounded >= int.MaxValue)
            {
                return int.MaxValue;
            }
            if (rounded <= int.MinValue)
            {
                return int.MinValue;
            }
            // rounded は偶数丸め済みの整数なので、VBMath.CLng はそのままの値を返す
            return VBMath.CLng(rounded);
        }

        // 指数表記の数値（「1e5」「1.5E-3」「1d5」）を読む。VB（OLE Automation の VarParseNumFromStr）と同じく、
        // 指数の記号には E のほか D も使え、記号の後ろには符号と 1 桁以上の数字が要る（「1e」「e5」は数値ではない）。
        // text は NumericText で全角を半角にし、前後の空白を除いたもの
        private static readonly Regex ExponentPattern = new Regex(
            @"^[+-]?(?:[0-9][0-9,]*(?:\.[0-9]*)?|\.[0-9]+)[eEdD][+-]?[0-9]+$",
            RegexOptions.CultureInvariant);
        internal static bool TryParseExponent(string text, out double value)
        {
            value = 0d;
            if (string.IsNullOrEmpty(text) || !ExponentPattern.IsMatch(text))
            {
                return false;
            }
            var normalized = text.Replace(",", "").Replace('d', 'e').Replace('D', 'e');
            if (!double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
                || double.IsInfinity(value) || double.IsNaN(value))
            {
                // 範囲を超える値（「1e400」）は数値とみなさない（VB ではオーバーフロー）
                value = 0d;
                return false;
            }
            return true;
        }

        public static double ToDouble(string Value)
        {
            Value = NumericText(Value);
            if (TryParseExponent(Value, out double exp))
            {
                return exp;
            }
            if (double.TryParse(Value, out double ret))
            {
                return ret;
            }
            return 0d;
        }

        // 以下 Microsoft.VisualBasic.Conversions とは関係ないが便宜上変換処理として配置している関数。
        public static bool TryToDateTime(string value, out DateTime dateTime)
        {
            // XXX DateTime パースの精査
            return DateTime.TryParse(value, out dateTime);
        }
        public static DateTime GetNow()
        {
            // XXX 変換ですらないがパースの隣にあったほうが見通しがいいので。。。
            return DateTime.Now;
        }
    }
}
