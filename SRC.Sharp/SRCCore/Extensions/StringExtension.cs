using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SRCCore.Extensions
{
    public static class StringExtension
    {
        private static readonly Regex ArrayIndexRegex = new Regex("^.+?\\[(.+)\\]$");
        private static readonly Regex InsideKakkoRegex = new Regex("\\((.+)\\)");

        public static string ArrayIndexByName(this string vname)
        {
            var m = ArrayIndexRegex.Match(vname);
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            return "";
        }

        public static string InsideKakko(this string str)
        {
            var m = InsideKakkoRegex.Match(str);
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            return "";
        }
    }
}
