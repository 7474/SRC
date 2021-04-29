using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class TransformCmd : CmdData
    {
        public TransformCmd(SRC src, EventDataLine eventData) : base(src, CmdType.TransformCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
