using System;
using System.Linq;
using System.Text;

namespace SRCCore.VB
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
        // Shift-JIS encoding instance, cached for performance
        private static readonly Lazy<System.Text.Encoding> _shiftJISEncoding = new Lazy<System.Text.Encoding>(() =>
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            return System.Text.Encoding.GetEncoding("Shift_JIS");
        });

        // Shift-JIS encoding with fallback for incomplete multi-byte characters, cached for performance
        private static readonly Lazy<System.Text.Encoding> _shiftJISEncodingWithFallback = new Lazy<System.Text.Encoding>(() =>
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            return System.Text.Encoding.GetEncoding(
                "Shift_JIS",
                new System.Text.EncoderReplacementFallback(""),
                new System.Text.DecoderReplacementFallback("")
            );
        });

        private static System.Text.Encoding ShiftJISEncoding => _shiftJISEncoding.Value;
        private static System.Text.Encoding ShiftJISEncodingWithFallback => _shiftJISEncodingWithFallback.Value;

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
            return InStr(1, String1 ?? "", String2 ?? "");
        }

        // https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.visualbasic.strings.instr?view=net-5.0
        public static int InStr(int Start, string String1, string String2)
        {
            // .NET Core と .NET 6 で同環境でも比較設定の規定値が異なっていそう。
            return (String1 ?? "").IndexOf(String2 ?? "", Start - 1, StringComparison.Ordinal) + 1;
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
            return Mid(str, 1, Length);
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

        public static string StrDup(string str, int n)
        {
            if (string.IsNullOrEmpty(str) || n <= 0)
            {
                return "";
            }
            else if (str.Length == 1)
            {
                return new string(str[0], n);
            }
            else
            {
                return string.Concat(Enumerable.Repeat(str, n));
            }
        }

        // https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.visualbasic.strings.strcomp?view=net-5.0
        public static int StrComp(string String1, string String2)
        {
            return String1.CompareTo(String2);
        }

        // https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.visualbasic.strings.strconv?view=net-5.0
        public static string StrConv(string str, VbStrConv Conversion)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str ?? "";
            }

            switch (Conversion)
            {
                case VbStrConv.Wide:
                    return ConvertToWide(str);
                case VbStrConv.Narrow:
                    return ConvertToNarrow(str);
                case VbStrConv.Hiragana:
                    return ConvertKatakanaToHiragana(str);
                default:
                    return str;
            }
        }

        /// <summary>
        /// Converts half-width (hankaku) characters to full-width (zenkaku) characters
        /// </summary>
        private static string ConvertToWide(string str)
        {
            var result = new StringBuilder();
            foreach (char c in str)
            {
                // ASCII alphanumeric and common symbols (0x20-0x7E) -> Full-width (0xFF01-0xFF5E)
                if (c >= 0x20 && c <= 0x7E)
                {
                    // Convert to full-width equivalent
                    // Space (0x20) -> Full-width space (0x3000)
                    if (c == 0x20)
                    {
                        result.Append((char)0x3000);
                    }
                    else
                    {
                        // Other ASCII characters: add 0xFEE0 to get full-width equivalent
                        result.Append((char)(c + 0xFEE0));
                    }
                }
                // Half-width katakana (0xFF61-0xFF9F) -> Full-width katakana (0x30A1-0x30FA)
                else if (c >= 0xFF61 && c <= 0xFF9F)
                {
                    result.Append(ConvertHalfKatakanaToFull(c));
                }
                else
                {
                    // Already full-width or not convertible
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Converts full-width (zenkaku) characters to half-width (hankaku) characters
        /// </summary>
        private static string ConvertToNarrow(string str)
        {
            var result = new StringBuilder();
            foreach (char c in str)
            {
                // Full-width ASCII (0xFF01-0xFF5E) -> ASCII (0x21-0x7E)
                if (c >= 0xFF01 && c <= 0xFF5E)
                {
                    result.Append((char)(c - 0xFEE0));
                }
                // Full-width space (0x3000) -> ASCII space (0x20)
                else if (c == 0x3000)
                {
                    result.Append((char)0x20);
                }
                // Full-width katakana (0x30A1-0x30FA) -> Half-width katakana (0xFF61-0xFF9F)
                else if (c >= 0x30A1 && c <= 0x30FA)
                {
                    result.Append(ConvertFullKatakanaToHalf(c));
                }
                else
                {
                    // Already half-width or not convertible
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// Converts half-width katakana to full-width katakana
        /// NOTE: This is a simplified implementation using arithmetic offsets.
        /// A complete implementation would require a full mapping table for all katakana characters,
        /// especially for dakuten (゛) and handakuten (゜) marks which don't map correctly with simple arithmetic.
        /// For now, this handles the most common cases used in the codebase.
        /// </summary>
        private static char ConvertHalfKatakanaToFull(char c)
        {
            // Simplified mapping for common half-width katakana to full-width
            // This is a basic implementation - a complete one would need a full mapping table
            if (c >= 0xFF66 && c <= 0xFF9D)
            {
                // Basic katakana: add offset to get full-width
                return (char)(c - 0xFF66 + 0x30A1);
            }
            // For dakuten/handakuten marks and other special cases, return as-is
            // A full implementation would handle these properly
            return c;
        }

        /// <summary>
        /// Converts full-width katakana to half-width katakana
        /// NOTE: This is a simplified implementation using arithmetic offsets.
        /// See ConvertHalfKatakanaToFull for limitations.
        /// </summary>
        private static char ConvertFullKatakanaToHalf(char c)
        {
            // Simplified mapping for common full-width katakana to half-width
            if (c >= 0x30A1 && c <= 0x30F6)
            {
                // Basic katakana: subtract offset to get half-width
                return (char)(c - 0x30A1 + 0xFF66);
            }
            // For special cases, return as-is
            return c;
        }

        /// <summary>
        /// Converts full-width katakana characters to full-width hiragana characters.
        /// In VB6, StrConv with vbHiragana converts katakana to hiragana.
        /// Full-width katakana (ァ-ン, 0x30A1-0x30F3) maps to hiragana (ぁ-ん, 0x3041-0x3093)
        /// by subtracting 0x60.
        /// </summary>
        private static string ConvertKatakanaToHiragana(string str)
        {
            var result = new StringBuilder(str.Length);
            foreach (char c in str)
            {
                // Full-width katakana ァ(0x30A1) to ン(0x30F3) -> hiragana ぁ(0x3041) to ん(0x3093)
                if (c >= 0x30A1 && c <= 0x30F3)
                {
                    result.Append((char)(c - 0x60));
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        // https://docs.microsoft.com/ja-jp/dotnet/api/microsoft.visualbasic.strings.trim?view=net-5.0
        public static string Trim(string str)
        {
            return string.IsNullOrEmpty(str) ? "" : str.Trim();
        }

        // ---- xxxB
        // Byte-based string functions using Shift-JIS encoding for backward compatibility
        // See: https://github.com/7474/SRC/issues/175

        /// <summary>
        /// Returns the byte length of a string in Shift-JIS encoding
        /// </summary>
        public static int LenB(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }
            return ShiftJISEncoding.GetByteCount(str);
        }

        /// <summary>
        /// Returns the leftmost bytes of a string based on Shift-JIS encoding
        /// </summary>
        public static string LeftB(string str, int byteCount)
        {
            if (string.IsNullOrEmpty(str) || byteCount <= 0)
            {
                return "";
            }

            var bytes = ShiftJISEncoding.GetBytes(str);
            if (byteCount >= bytes.Length)
            {
                return str;
            }

            // Decode only the requested number of bytes
            // Use cached encoding with fallback to handle incomplete multi-byte characters
            return ShiftJISEncodingWithFallback.GetString(bytes, 0, byteCount);
        }

        /// <summary>
        /// Returns the rightmost bytes of a string based on Shift-JIS encoding
        /// </summary>
        public static string RightB(string str, int byteCount)
        {
            if (string.IsNullOrEmpty(str) || byteCount <= 0)
            {
                return "";
            }

            var bytes = ShiftJISEncoding.GetBytes(str);
            if (byteCount >= bytes.Length)
            {
                return str;
            }

            return ShiftJISEncodingWithFallback.GetString(bytes, bytes.Length - byteCount, byteCount);
        }

        /// <summary>
        /// Returns a substring based on byte positions in Shift-JIS encoding
        /// </summary>
        public static string MidB(string str, int startByte)
        {
            return MidB(str, startByte, LenB(str));
        }

        /// <summary>
        /// Returns a substring based on byte positions in Shift-JIS encoding
        /// </summary>
        public static string MidB(string str, int startByte, int byteCount)
        {
            if (string.IsNullOrEmpty(str) || byteCount <= 0)
            {
                return "";
            }

            var bytes = ShiftJISEncoding.GetBytes(str);
            
            // Adjust for 1-based indexing (VB6 style)
            var startIndex = startByte - 1;
            
            if (startIndex >= bytes.Length || startIndex < 0)
            {
                return "";
            }

            var actualByteCount = System.Math.Min(byteCount, bytes.Length - startIndex);
            
            return ShiftJISEncodingWithFallback.GetString(bytes, startIndex, actualByteCount);
        }

        /// <summary>
        /// Returns the byte position of the first occurrence of a substring in Shift-JIS encoding
        /// </summary>
        public static int InStrB(string string1, string string2)
        {
            return InStrB(1, string1, string2);
        }

        /// <summary>
        /// Returns the byte position of the first occurrence of a substring in Shift-JIS encoding
        /// </summary>
        public static int InStrB(int start, string string1, string string2)
        {
            if (string.IsNullOrEmpty(string1) || string.IsNullOrEmpty(string2))
            {
                return 0;
            }

            var bytes1 = ShiftJISEncoding.GetBytes(string1);
            var bytes2 = ShiftJISEncoding.GetBytes(string2);
            
            // Adjust for 1-based indexing
            var startIndex = start - 1;
            
            if (startIndex < 0 || startIndex >= bytes1.Length)
            {
                return 0;
            }

            // Search for byte pattern
            for (int i = startIndex; i <= bytes1.Length - bytes2.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < bytes2.Length; j++)
                {
                    if (bytes1[i + j] != bytes2[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i + 1; // Return 1-based position
                }
            }

            return 0;
        }

        /// <summary>
        /// Returns the byte position of the last occurrence of a substring in Shift-JIS encoding
        /// </summary>
        public static int InStrRevB(string string1, string string2)
        {
            if (string.IsNullOrEmpty(string1) || string.IsNullOrEmpty(string2))
            {
                return 0;
            }
            return InStrRevB(LenB(string1), string1, string2);
        }

        /// <summary>
        /// Returns the byte position of the last occurrence of a substring in Shift-JIS encoding
        /// </summary>
        public static int InStrRevB(int start, string string1, string string2)
        {
            if (string.IsNullOrEmpty(string1) || string.IsNullOrEmpty(string2))
            {
                return 0;
            }

            var bytes1 = ShiftJISEncoding.GetBytes(string1);
            var bytes2 = ShiftJISEncoding.GetBytes(string2);
            
            var startIndex = System.Math.Min(start - 1, bytes1.Length - bytes2.Length);
            
            if (startIndex < 0)
            {
                return 0;
            }

            // Search backwards for byte pattern
            for (int i = startIndex; i >= 0; i--)
            {
                bool found = true;
                for (int j = 0; j < bytes2.Length; j++)
                {
                    if (i + j >= bytes1.Length || bytes1[i + j] != bytes2[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i + 1; // Return 1-based position
                }
            }

            return 0;
        }
    }
}
