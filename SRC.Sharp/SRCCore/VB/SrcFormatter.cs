using System;

namespace SRCCore.VB
{
    public static class SrcFormatter
    {
        // SrcFormatter.Format の代替え実装
        // https://docs.microsoft.com/ja-jp/dotnet/api/SrcFormatter.format?view=netframework-4.8
        // VB の CStr・Format と同じく、日本語の地域の設定（SrcCulture）で書く。実行する環境の言語によらない
        public static string Format(object value)
        {
            return Convert.ToString(value, SrcCulture.Japanese) ?? "";
        }

        public static string Format(int value, string format)
        {
            try
            {
                return value.ToString(format, SrcCulture.Japanese);
            }
            catch
            {
                return Format(value);
            }
        }

        public static string Format(double value, string format)
        {
            try
            {
                return value.ToString(format, SrcCulture.Japanese);
            }
            catch
            {
                return Format(value);
            }
        }
    }
}
