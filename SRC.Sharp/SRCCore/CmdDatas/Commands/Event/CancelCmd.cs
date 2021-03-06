using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class CancelCmd : CmdData
    {
        public CancelCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CancelCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            SRC.IsCanceled = true;
            return EventData.NextID;
        }
    }
}
