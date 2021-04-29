using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class FadeOutCmd : CmdData
    {
        public FadeOutCmd(SRC src, EventDataLine eventData) : base(src, CmdType.FadeOutCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
