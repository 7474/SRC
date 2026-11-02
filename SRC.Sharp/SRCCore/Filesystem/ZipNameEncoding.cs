using System.Text;

namespace SRCCore.Filesystem
{
    /// <summary>
    /// zip のファイル名の文字コード（UTF-8 の印が無い名前に使う）。UTF-8 として正しく読めれば UTF-8、読めなければ Shift_JIS として読む。
    /// Windows で作った zip は Shift_JIS、macOS・Linux で作った zip は UTF-8 の名前に印を付けないことがあるため
    /// </summary>
    public sealed class ZipNameEncoding : Encoding
    {
        public static readonly ZipNameEncoding Instance = new ZipNameEncoding();

        private static readonly Encoding Utf8 = new UTF8Encoding(false, true);

        static ZipNameEncoding()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public static Encoding ShiftJis => GetEncoding(932);

        private static Encoding Pick(byte[] bytes, int index, int count)
        {
            try
            {
                Utf8.GetCharCount(bytes, index, count);
                return Utf8;
            }
            catch (DecoderFallbackException)
            {
                return ShiftJis;
            }
        }

        public override int GetByteCount(char[] chars, int index, int count) => Utf8.GetByteCount(chars, index, count);
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) => Utf8.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
        public override int GetCharCount(byte[] bytes, int index, int count) => Pick(bytes, index, count).GetCharCount(bytes, index, count);
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex) => Pick(bytes, byteIndex, byteCount).GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        public override int GetMaxByteCount(int charCount) => Utf8.GetMaxByteCount(charCount);
        // UTF-8・Shift_JIS とも、1 バイトから 2 文字以上にはならない
        public override int GetMaxCharCount(int byteCount) => byteCount;
    }
}
