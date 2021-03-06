using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class FreeMemoryCmd : CmdData
    {
        public FreeMemoryCmd(SRC src, EventDataLine eventData) : base(src, CmdType.FreeMemoryCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            SRC.UList.Clean();
            SRC.PList.Clean();
            SRC.IList.Update();
            return EventData.NextID;
        }
    }
}
