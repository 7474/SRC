using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ClsCmd : CmdData
    {
        public ClsCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ClsCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
