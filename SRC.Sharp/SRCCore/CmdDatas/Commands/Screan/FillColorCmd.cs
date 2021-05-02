using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class FillColorCmd : CmdData
    {
        public FillColorCmd(SRC src, EventDataLine eventData) : base(src, CmdType.FillColorCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
