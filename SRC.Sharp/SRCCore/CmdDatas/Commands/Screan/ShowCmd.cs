using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ShowCmd : CmdData
    {
        public ShowCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ShowCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
