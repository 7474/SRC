using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class UseAbilityCmd : CmdData
    {
        public UseAbilityCmd(SRC src, EventDataLine eventData) : base(src, CmdType.UseAbilityCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
