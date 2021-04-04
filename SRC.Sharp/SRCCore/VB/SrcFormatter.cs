using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
