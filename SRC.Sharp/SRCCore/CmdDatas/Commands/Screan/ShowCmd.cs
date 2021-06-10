using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ShowCmd : CmdData
    {
        public ShowCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ShowCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (!GUI.MainFormVisible)
            {
                GUI.MainFormShow();
                GUI.UpdateScreen();
                GUI.Sleep(0, true);
            }

            if (!GUI.IsPictureVisible)
            {
                GUI.RedrawScreen();
            }
            return EventData.NextID;
        }
    }
}
