using System;

namespace SRC.Core.VB
{

    public enum VbStrConv
    {
        Wide,
        Narrow,
        Hiragana
    }

    // VB6の文字列処理（Microsoft.VisualBasic.Strings）のうちSRCで使用していたものの仮実装。
    // memo:
    // - Binaryベースの処理はしていなさそうなので割愛
    public static class Strings
    {

        // https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.visualbasic.strings.asc?view=net-5.0
        public static int Asc(char _String)
        {
            int int32 = Convert.ToInt32(_String);
            if (int32 < 128)
            {
                return int32;
            }

            //throw new ArgumentException();
            return int32;
        }

        // https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.visualbasic.strings.asc?view=net-5.0
        public static int Asc(string _String)
        {
            return Asc(_String[0]);
        }

        // https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.visualbasic.strings.instr?view=net-5.0
        public static int InStr(string String1, string String2)
        {
            return InStr(1, String1, String2);
        }

        // https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.visualbasic.strings.instr?view=net-5.0
        public static int InStr(int Start, string String1, string String2)
        {
            return String1.IndexOf(String2, Start - 1) + 1;
        }

        // https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.visualbasic.strings.lcase?view=net-5.0
        public static string LCase(string Value)
        {
            return Value.ToLower();
        }

        // https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.visualbasic.strings.lcase?view=net-5.0
        public static char LCase(char Value)
        {
            return Value.ToString().ToLower()[0];
        }

        // https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.visualbasic.strings.left?view=net-5.0
        public static string Left(string str, int Length)
        {
            if (string.IsNullOrEmpty(str)) { return ""; }
            return str.Substring(0, Length);
        }

        // https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.visualbasic.strings.len?view=net-5.0
        public static int Len(string Expression)
        {
            if (Expression == null)
            {
                return 0;
            }
            return Expression.Length;
        }

        // https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.visualbasic.strings.mid?view=net-5.0
        public static string Mid(string str, int Start)
        {
            if (string.IsNullOrEmpty(str)) { return ""; }
            return Mid(str, Start, str.Length);
        }

        // https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.visualbasic.strings.mid?view=net-5.0
        public static string Mid(string str, int Start, int Length)
        {
            if (Length == 0 || string.IsNullOrEmpty(str))
            {
                return "";
            }
            if (Start > str.Length)
            {
                return "";
            }
            if (checked(Start + Length) > str.Length)
            {
                return str.Substring(Start - 1);
            }
            return str.Substring(Start - 1, Length);
        }

        // https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.visualbasic.strings.right?view=net-5.0
        public static string Right(string str, int Length)
        {
            if (Length == 0 || string.IsNullOrEmpty(str))
            {
                return "";
            }
            if (Length >= str.Length)
            {
                return str;
            }
            return str.Substring(str.Length - Length, Length);
        }

        // https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.visualbasic.strings.space?view=net-5.0
        public static string Space(int Number)
        {
            return new string(' ', Number);
        }

        // https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.visualbasic.strings.strcomp?view=net-5.0
        public static int StrComp(string String1, string String2)
        {
            return String1.CompareTo(String2);
        }

        // https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.visualbasic.strings.strconv?view=net-5.0
        public static string StrConv(string str, VbStrConv Conversion)
        {
            // TODO 要る分だけ実装ないし完全に置き換える
            switch (Conversion)
            {
                default:
                    return str;
            }
        }

        // https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.visualbasic.strings.trim?view=net-5.0
        public static string Trim(string str)
        {
            return string.IsNullOrEmpty(str) ? "" : str.Trim();
        }
    }
}