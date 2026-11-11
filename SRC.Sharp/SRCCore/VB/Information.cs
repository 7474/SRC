using System.Globalization;

namespace SRCCore.VB
{
    // VBの Microsoft.VisualBasic.Information のうちSRCで使用していたものの仮実装。
    public static class Information
    {
        public static bool IsNumeric(object Expression)
        {
            try
            {
                // 前後の空白は無視するが、途中の空白は無視しない（VB: IsNumeric("1400 1") は False）。
                // 全角の数字・空白は半角として読む（Conversions.NumericText。変換の側と同じ規則にする）
                var str = Conversions.NumericText(Conversions.ToString(Expression));
                decimal dumy;
                if (decimal.TryParse(str, NumberStyles.Number, SrcCulture.Japanese, out dumy))
                {
                    return true;
                }
                // 指数表記（「1e5」「1d5」）も数値とみなす（VB: IsNumeric("1e5") は True）
                if (Conversions.TryParseExponent(str, out _))
                {
                    return true;
                }
                // XXX VB の IsNumeric は &H・&O・括弧（(1)）・通貨記号も数値とみなすが、ここでは読まない（7474/SRC#1545）。
                // 書式は、実行する環境の言語によらず日本語の地域の設定（SrcCulture。小数点は「.」、桁区切りは「,」）で読む
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
