using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SetRelationCmd : CmdData
    {
        public SetRelationCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetRelationCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
