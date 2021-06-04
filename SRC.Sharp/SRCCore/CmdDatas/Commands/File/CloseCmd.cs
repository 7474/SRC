using SRCCore.Events;
using SRCCore.Exceptions;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class CloseCmd : CmdData
    {
        public CloseCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CloseCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 2)
            {
                throw new EventErrorException(this, "Closeコマンドの引数の数が違います");
            }

           SRC.FileHandleManager.Close(GetArgAsLong(2));
            return EventData.NextID;
        }
    }
}
