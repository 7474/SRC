// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.VB;
using System;
using System.IO;
using System.Text;

namespace SRCCore.Lib
{
    /// <summary>
    /// 元の SRC（VB5）のセーブデータ（クリアデータ・中断データ）を、VB の Input #・Line Input #・EOF と同じ区切りで読む（#1577）。
    /// 形式は docs/history/porting/vb-savedata.md。
    /// </summary>
    public sealed class VbSaveDataReader
    {
        private readonly string text;
        private int pos;

        public VbSaveDataReader(string text)
        {
            this.text = text ?? "";
        }

        /// <summary>
        /// 読んでいるセーブデータの版（VB: SRC.bas の SaveDataVersion）。版で古い形式を読み替える。
        /// 元の SRC はファイルを開き直しても同じ値を使うので、開き直すときは <see cref="Reopen"/> で引き継ぐ
        /// </summary>
        public int SaveDataVersion { get; set; }

        /// <summary>同じファイルを開き直す（VB: Close して Open し直す）。版は引き継ぐ</summary>
        public VbSaveDataReader Reopen()
        {
            return new VbSaveDataReader(text) { SaveDataVersion = SaveDataVersion };
        }

        /// <summary>ファイルの終わりか（VB: EOF）</summary>
        public bool EOF => pos >= text.Length;

        /// <summary>
        /// 値を 1 つ読む（VB: Input #）。先頭の空白を飛ばし、「"」で囲まれていれば次の「"」までを、
        /// そうでなければカンマか行の終わりまでを読む。区切り（カンマ・改行）は読み捨てる
        /// </summary>
        public string Input()
        {
            if (EOF)
            {
                // VB: 実行時エラー 62（ファイルの終わりを超えて読んだ）
                throw new EndOfStreamException("セーブデータの終わりを超えて読みました");
            }
            while (pos < text.Length && (text[pos] == ' ' || text[pos] == '\t'))
            {
                pos++;
            }

            if (pos < text.Length && text[pos] == '"')
            {
                pos++;
                var end = text.IndexOf('"', pos);
                if (end < 0)
                {
                    end = text.Length;
                }
                var value = text.Substring(pos, end - pos);
                pos = Math.Min(end + 1, text.Length);
                SkipToDelimiter();
                return value;
            }

            var start = pos;
            while (pos < text.Length && text[pos] != ',' && text[pos] != '\r' && text[pos] != '\n')
            {
                pos++;
            }
            var field = text.Substring(start, pos - start).TrimEnd(' ', '\t');
            SkipDelimiter();
            return field;
        }

        /// <summary>行の残りを読む（VB: Line Input #）。行の終わり（CR+LF・CR・LF）は読み捨てる</summary>
        public string LineInput()
        {
            if (EOF)
            {
                throw new EndOfStreamException("セーブデータの終わりを超えて読みました");
            }
            var start = pos;
            while (pos < text.Length && text[pos] != '\r' && text[pos] != '\n')
            {
                pos++;
            }
            var line = text.Substring(start, pos - start);
            SkipNewLine();
            return line;
        }

        /// <summary>Integer・Long の変数に読む（VB: Input # の数値の変数）</summary>
        public int InputInt()
        {
            return ToInt(Input());
        }

        /// <summary>Boolean の変数に読む（VB: Input # の真偽値の変数。Write # は #TRUE#・#FALSE# と書く）</summary>
        public bool InputBool()
        {
            return ToBool(Input());
        }

        public static int ToInt(string value)
        {
            switch (value)
            {
                case "#TRUE#":
                    return -1;
                case "#FALSE#":
                case "":
                    return 0;
            }
            return VBMath.CLng(Conversions.ToDouble(value));
        }

        public static bool ToBool(string value)
        {
            switch (value)
            {
                case "#TRUE#":
                    return true;
                case "#FALSE#":
                case "":
                    return false;
            }
            if (bool.TryParse(value, out var b))
            {
                return b;
            }
            return Conversions.ToDouble(value) != 0d;
        }

        private void SkipToDelimiter()
        {
            while (pos < text.Length && text[pos] != ',' && text[pos] != '\r' && text[pos] != '\n')
            {
                pos++;
            }
            SkipDelimiter();
        }

        private void SkipDelimiter()
        {
            if (pos < text.Length && text[pos] == ',')
            {
                pos++;
                return;
            }
            SkipNewLine();
        }

        private void SkipNewLine()
        {
            if (pos < text.Length && text[pos] == '\r')
            {
                pos++;
            }
            if (pos < text.Length && text[pos] == '\n')
            {
                pos++;
            }
        }

        private static readonly Lazy<Encoding> shiftJis = new Lazy<Encoding>(() =>
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            return Encoding.GetEncoding(932);
        });

        /// <summary>元の SRC のセーブデータの文字コード（Shift_JIS。日本語版の Windows のコードページ 932）</summary>
        public static Encoding ShiftJis => shiftJis.Value;

        /// <summary>
        /// 元の SRC の形式か。先頭（BOM と空白を除く）が「{」なら SRC# の JSON、そうでなければ元の SRC の形式とする
        /// </summary>
        public static bool IsVbSaveData(byte[] data)
        {
            var i = 0;
            if (data.Length >= 3 && data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF)
            {
                i = 3;
            }
            while (i < data.Length && (data[i] == ' ' || data[i] == '\t' || data[i] == '\r' || data[i] == '\n'))
            {
                i++;
            }
            return i < data.Length && data[i] != '{';
        }

        /// <summary>元の SRC のセーブデータを Shift_JIS で読む</summary>
        public static VbSaveDataReader FromBytes(byte[] data)
        {
            return new VbSaveDataReader(ShiftJis.GetString(data));
        }
    }
}
