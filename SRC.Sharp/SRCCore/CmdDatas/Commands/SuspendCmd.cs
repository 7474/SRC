using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class SuspendCmd : CmdData
    {
        public SuspendCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SuspendCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            return EventData.ID + 1;
        }
    }
}
