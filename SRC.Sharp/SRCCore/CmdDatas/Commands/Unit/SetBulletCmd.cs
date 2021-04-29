using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SetBulletCmd : CmdData
    {
        public SetBulletCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetBulletCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
