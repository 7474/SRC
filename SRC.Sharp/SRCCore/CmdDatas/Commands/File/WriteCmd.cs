// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class WriteCmd : CmdData
    {
        public WriteCmd(SRC src, EventDataLine eventData) : base(src, CmdType.WriteCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum < 3)
            {
                throw new EventErrorException(this, "Writeコマンドの引数の数が違います");
            }

            var f = SRC.FileHandleManager.Get(GetArgAsLong(2));
            for (var i = 3; i <= ArgNum; i++)
            {
                f.Writer.WriteLine(GetArgAsString(i));
            }

            return EventData.NextID;
        }
    }
}
