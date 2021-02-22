using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SRCCore.VB
{
    // VBの Microsoft.VisualBasic.Information のうちSRCで使用していたものの仮実装。
    public static class Information
    {
        private static Regex whiteSpace = new Regex("\\s");

        public static bool IsNumeric(object Expression)
        {
            try
            {
                // 空白は無視して判断してるっぽい（ユニットのHP行判断）
                var str = whiteSpace.Replace(Expression?.ToString() ?? "", "");
                decimal dumy;
                if (decimal.TryParse(str, out dumy))
                {
                    return true;
                }
                Convert.ToDecimal(str);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
