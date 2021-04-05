using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class EndIfCmd : CmdData
    {
        public EndIfCmd(SRC src, EventDataLine eventData) : base(src, CmdType.EndIfCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            return EventData.ID + 1;
        }
    }
}
