using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class OvalCmd : CmdData
    {
        public OvalCmd(SRC src, EventDataLine eventData) : base(src, CmdType.OvalCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
