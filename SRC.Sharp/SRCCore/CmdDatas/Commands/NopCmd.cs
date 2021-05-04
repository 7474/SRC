using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class NopCmd : CmdData
    {
        public NopCmd(SRC src, EventDataLine eventData) : base(src, CmdType.NopCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            return EventData.NextID;
        }
    }
}
