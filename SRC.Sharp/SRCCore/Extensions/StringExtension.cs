using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SRCCore.Extensions
{
    public static class StringExtension
    {
        private static readonly Regex ArrayIndexRegex = new Regex("^.+?\\[(.+)\\]$");
        public static string ArrayIndexByName(this string vname)
        {
            var m = ArrayIndexRegex.Match(vname);
            if(m.Success)
            {
                return m.Groups[1].Value;
            }
            return "";
        }
    }
}
