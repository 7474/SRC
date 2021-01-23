using System;
using System.Collections.Generic;
using System.Text;

namespace SRC.Core.VB
{
    // VBの Microsoft.VisualBasic.Conversions のうちSRCで使用していたものの仮実装。
    public static class Conversions
    {
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
    }
}
