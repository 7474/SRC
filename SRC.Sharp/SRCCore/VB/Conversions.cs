using System;

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
            return new string(chars).Trim(' ', '\t', '\r', '\n');
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
            return (int)rounded;
        }

        public static double ToDouble(string Value)
        {
            Value = NumericText(Value);
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
