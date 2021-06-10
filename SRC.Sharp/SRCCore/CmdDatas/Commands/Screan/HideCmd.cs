using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class HideCmd : CmdData
    {
        public HideCmd(SRC src, EventDataLine eventData) : base(src, CmdType.HideCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            GUI.MainFormHide();
            return EventData.NextID;
        }
    }
}
