// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Events;
using SRCCore.Exceptions;

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
                var buf = f.Reader.ReadLine() ?? "";
                Expression.SetVariableAsString(GetArg(i), buf);
            }

            return EventData.NextID;
        }
    }
}
