using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class EndCmd : CmdData
    {
        public EndCmd(SRC src, EventDataLine eventData) : base(src, CmdType.EndCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            return EventData.NextID;
        }
    }
}
