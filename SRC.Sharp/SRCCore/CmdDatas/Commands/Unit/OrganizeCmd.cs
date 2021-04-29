using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class OrganizeCmd : CmdData
    {
        public OrganizeCmd(SRC src, EventDataLine eventData) : base(src, CmdType.OrganizeCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
