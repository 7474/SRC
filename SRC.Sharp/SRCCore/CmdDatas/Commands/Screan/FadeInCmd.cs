using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class FadeInCmd : CmdData
    {
        public FadeInCmd(SRC src, EventDataLine eventData) : base(src, CmdType.FadeInCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
