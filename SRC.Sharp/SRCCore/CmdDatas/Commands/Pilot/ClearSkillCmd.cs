using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ClearSkillCmd : CmdData
    {
        public ClearSkillCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ClearSkillCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
