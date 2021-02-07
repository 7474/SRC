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
            try
            {
                return Convert.ToInt32(Value);
            }
            catch
            {
                return 0;
            }
        }

        public static double ToDouble(string Value)
        {
            try
            {
                return Convert.ToDouble(Value);
            }
            catch
            {
                return 0d;
            }
        }
    }
}
