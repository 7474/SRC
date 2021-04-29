using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class LevelUpCmd : CmdData
    {
        public LevelUpCmd(SRC src, EventDataLine eventData) : base(src, CmdType.LevelUpCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
