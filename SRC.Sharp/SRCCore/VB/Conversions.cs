using System;
using System.Collections.Generic;
using System.Text;

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
            return 0;
        }

        public static double ToDouble(string Value)
        {
            if (double.TryParse(Value, out double ret))
            {
                return ret;
            }
            return 0d;
        }
    }
}
