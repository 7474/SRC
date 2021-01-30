// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRC.Core.Exceptions;
using SRC.Core.VB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SRC.Core.Lib
{
    public class SrcDataReader : StreamReader
    {
        public string FileName { get; }
        public int LineNumber { get; private set; }
        public string LastLine { get; private set; }

        // TODO Encoding
        public SrcDataReader(string fname, Stream stream) : base(stream)
        {
            FileName = fname;
        }

        public bool EOT => !HasMore;
        public bool HasMore => Peek() >= 0;

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
                var buf = ReadLine().Trim();
                LineNumber++;

                // 空行はそのまま返す
                if (string.IsNullOrEmpty(buf))
                {
                    break;
                }

                // コメント行はスキップ
                if (Strings.Left(buf, 1) == "#")
                {
                    continue;
                }

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
