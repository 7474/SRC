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

        public static int ToInteger(string Value)
        {
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
