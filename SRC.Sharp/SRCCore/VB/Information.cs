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
                // 指数表記（「1e5」）・&H・&O・括弧（(1)）・通貨記号・後ろの符号も数値とみなす（Conversions.TryParseVbNumber。
                // 元の SRC の実機で確かめた。#1545・#1550）。
                // 書式は、実行する環境の言語によらず日本語の地域の設定（SrcCulture。小数点は「.」、桁区切りは「,」）で読む
                return Conversions.TryParseVbNumber(Conversions.ToString(Expression), out _);
            }
            catch
            {
                return false;
            }
        }
    }
}
