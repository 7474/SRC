// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Exceptions;
using SRCCore.VB;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SRCCore.Lib
{
    public class SrcDataReader : StreamReader
    {
        public string FileName { get; }
        public int LineNumber { get; private set; }
        public string LastLine { get; private set; }

        public SrcDataReader(string fname, Stream stream) : base(stream, Encoding.UTF8)
        {
            FileName = fname;
        }

        public bool EOT => !HasMore;
        public bool HasMore => Peek() >= 0;

        private StringBuilder _rawBuffer = new StringBuilder();
        public string Raw => _rawBuffer.ToString();
        private StringBuilder _commentBuffer = new StringBuilder();
        public string RawComment => _commentBuffer.ToString();
        private StringBuilder _rawDataBuffer = new StringBuilder();
        public string RawData => _rawDataBuffer.ToString();
        public void ClearRaw()
        {
            _rawBuffer.Clear();
        }
        public void ClearRawComment()
        {
            _commentBuffer.Clear();
        }
        public void ClearRawData()
        {
            _rawDataBuffer.Clear();
        }

        // From GeneralLib#GetLine
        // 行頭に「#」がある場合は行の読み飛ばしを行う。
        // 行中に「//」がある場合、そこからはコメントと見なして無視する。
        // 行末に「_」がある場合は行の結合を行う。
        public string GetLine()
        {
            string line_buf = "";
            while (HasMore)
            {
                // 完全な互換性にはこだわらないので先にTrimしてしまう
                var buf = ReadLine().Trim().Trim(
                        // EOF
                        (char)0x1a
                    );
                _rawBuffer.AppendLine(buf);
                LineNumber++;

                // 空行はそのまま返す
                if (string.IsNullOrEmpty(buf))
                {
                    _commentBuffer.AppendLine(buf);
                    break;
                }

                // コメント行はスキップ
                if (Strings.Left(buf, 1) == "#")
                {
                    _commentBuffer.AppendLine(buf);
                    continue;
                }

                _rawDataBuffer.AppendLine(buf);

                // コメント部分を削除
                var idx = Strings.InStr(buf, "//");
                if (idx > 0)
                {
                    buf = Strings.Left(buf, idx - 1);
                }

                // 行末が「_」でなければ行の読み込みを完了
                if (Strings.Right(buf, 1) != "_")
                {
                    line_buf = line_buf + buf;
                    break;
                }

                // 行末が「_」の場合は行を結合
                line_buf = line_buf + Strings.Left(buf, Strings.Len(buf) - 1);
            }

            // 全角カンマ -> 半角カンマ
            LastLine = line_buf.Replace("，", ", ");
            return LastLine;
        }

        public InvalidSrcData InvalidData(string msg, string dname)
        {
            return new InvalidSrcData(msg, FileName, LineNumber, LastLine, dname);
        }

        public InvalidSrcDataException InvalidDataException(string msg, string dname)
        {
            return new InvalidSrcDataException(new List<InvalidSrcData> { InvalidData(msg, dname) });
        }
    }
}
