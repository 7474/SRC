using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SetSkillCmd : CmdData
    {
        public SetSkillCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetSkillCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
