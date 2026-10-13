// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Events;
using SRCCore.Exceptions;
using System.IO;
using System.Text;

namespace SRCCore.CmdDatas.Commands
{
    public class ReadCmd : CmdData
    {
        public ReadCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ReadCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum < 3)
            {
                throw new EventErrorException(this, "Readコマンドの引数の数が違います");
            }

            var f = SRC.FileHandleManager.Get(GetArgAsLong(2));
            for (var i = 3; i <= ArgNum; i++)
            {
                var buf = ReadInputField(f.Reader);
                Expression.SetVariableAsString(GetArg(i), buf);
            }

            return EventData.NextID;
        }

        // VB の Input # 文と同様にカンマ区切り、または改行区切りのフィールドを1つ読み込む
        public static string ReadInputField(TextReader reader)
        {
            // 先頭の空白を読み飛ばす
            while (reader.Peek() == ' ' || reader.Peek() == '\t')
            {
                reader.Read();
            }

            var sb = new StringBuilder();
            if (reader.Peek() == '"')
            {
                // 引用符で囲まれた文字列
                reader.Read();
                int c;
                while ((c = reader.Read()) >= 0 && c != '"')
                {
                    sb.Append((char)c);
                }
                // 次の区切り文字までを読み飛ばす
                SkipToDelimiter(reader, null);
                return sb.ToString();
            }

            SkipToDelimiter(reader, sb);
            return sb.ToString().TrimEnd(' ', '\t');
        }

        private static void SkipToDelimiter(TextReader reader, StringBuilder sb)
        {
            int c;
            while ((c = reader.Read()) >= 0)
            {
                if (c == ',' || c == '\n')
                {
                    return;
                }
                if (c == '\r')
                {
                    if (reader.Peek() == '\n')
                    {
                        reader.Read();
                    }
                    return;
                }
                sb?.Append((char)c);
            }
        }
    }
}
