using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class ElseCmd : CmdData
    {
        public ElseCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ElseCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            return EventData.ID + 1;
        }
    }
}
