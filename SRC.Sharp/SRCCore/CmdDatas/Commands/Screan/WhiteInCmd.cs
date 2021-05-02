using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class WhiteInCmd : CmdData
    {
        public WhiteInCmd(SRC src, EventDataLine eventData) : base(src, CmdType.WhiteInCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
