using SRCCore.VB;
using System.Text.RegularExpressions;

namespace SRCCore.Extensions
{
    public static class StringExtension
    {
        private static readonly Regex ArrayIndexRegex = new Regex("^.+?\\[(.+)\\]$");
        private static readonly Regex InsideKakkoRegex = new Regex("\\((.+)\\)");
        private static readonly Regex NewLineRegex = new Regex("\r\n|\n|\r");

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

        public static string ReplaceNewLine(this string str, string replace)
        {
            return NewLineRegex.Replace(str, replace);
        }

        public static string RemoveLineComment(this string buf)
        {
            if (Strings.InStr(buf, "//") > 0)
            {
                var in_single_quote = false;
                var in_double_quote = false;
                char lastChar = default;
                for (var i = 0; i < buf.Length; i++)
                {
                    var c = buf[i];
                    switch (c)
                    {
                        case '\'':
                            {
                                // シングルクオート
                                if (!in_double_quote)
                                {
                                    in_single_quote = !in_single_quote;
                                }

                                break;
                            }

                        case '"':
                            {
                                // ダブルクオート
                                if (!in_single_quote)
                                {
                                    in_double_quote = !in_double_quote;
                                }

                                break;
                            }

                        case '/':
                            {
                                // コメント？
                                if (!in_double_quote && !in_single_quote && lastChar == '/')
                                {
                                    buf = Strings.Left(buf, i - 1);
                                }

                                break;
                            }
                    }
                    lastChar = c;
                }
            }

            return buf;
        }
    }
}
