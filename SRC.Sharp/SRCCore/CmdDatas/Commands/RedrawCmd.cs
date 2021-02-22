using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class RedrawCmd : CmdData
    {
        public RedrawCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RedrawCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            var late_refresh = false;
            if (ArgNum == 2)
            {
                if (GetArgAsString(2) == "非同期")
                {
                    late_refresh = true;
                }
            }

            GUI.RedrawScreen(late_refresh);

            return EventData.ID + 1;
        }
    }
}
