using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    // ���ޏ�̓p�C���b�g����A�A�C�e������
    public class FixCmd : CmdData
    {
        public FixCmd(SRC src, EventDataLine eventData) : base(src, CmdType.FixCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //return EventData.NextID;
        }
    }
}
