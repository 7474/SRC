using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class PrintCmd : CmdData
    {
        public PrintCmd(SRC src, EventDataLine eventData) : base(src, CmdType.PrintCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
