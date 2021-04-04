using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class RefreshCmd : CmdData
    {
        public RefreshCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RefreshCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            GUI.UpdateScreen();

            return EventData.ID + 1;
        }
    }
}
