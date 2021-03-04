using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class IfCmd : AIfCmd
    {
        public IfCmd(SRC src, EventDataLine eventData) : base(src, CmdType.IfCmd, eventData)
        {
            PrepareArgs();
        }

        protected override int ExecInternal()
        {
            return EventData.ID + 1;
        }
    }
}
