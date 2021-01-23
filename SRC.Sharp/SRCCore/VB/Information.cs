using System;
using System.Collections.Generic;
using System.Text;

namespace SRC.Core.VB
{
    // VBの Microsoft.VisualBasic.Information のうちSRCで使用していたものの仮実装。
    public static class Information
    {
        public static bool IsNumeric(object Expression)
        {
            try
            {
                Convert.ToDecimal(Expression);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
