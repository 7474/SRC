using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    // ���ޏ�̓p�C���b�g����A�A�C�e������
    public class ReleaseCmd : CmdData
    {
        public ReleaseCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ReleaseCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
