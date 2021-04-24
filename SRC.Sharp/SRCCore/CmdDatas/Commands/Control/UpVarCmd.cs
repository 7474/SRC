using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class UpVarCmd : CmdData
    {
        public UpVarCmd(SRC src, EventDataLine eventData) : base(src, CmdType.UpVarCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Event.UpVarLevel = Event.UpVarLevel + 1;
            return EventData.NextID;
        }
    }
}
