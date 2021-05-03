namespace SRCCore.VB
{
    public static class SrcFormatter
    {
        // SrcFormatter.Format の代替え実装
        // https://docs.microsoft.com/ja-jp/dotnet/api/SrcFormatter.format?view=netframework-4.8
        public static string Format(object value)
        {
            return "" + value;
        }

        public static string Format(int value, string format)
        {
            try
            {
                return value.ToString(format);
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
                return value.ToString(format);
            }
            catch
            {
                return Format(value);
            }
        }
    }
}
