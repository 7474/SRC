namespace SRCCore.VB
{
    // VBの Microsoft.VisualBasic.Information のうちSRCで使用していたものの仮実装。
    public static class Information
    {
        public static bool IsNumeric(object Expression)
        {
            try
            {
                // 前後の空白は無視するが、途中の空白は無視しない（VB: IsNumeric("1400 1") は False）
                var str = (Expression?.ToString() ?? "").Trim();
                decimal dumy;
                if (decimal.TryParse(str, out dumy))
                {
                    return true;
                }
                // XXX
                //Convert.ToDecimal(str);
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
