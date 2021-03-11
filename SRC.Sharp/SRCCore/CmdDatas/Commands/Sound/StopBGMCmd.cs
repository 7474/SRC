using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.VB;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class StopBGMCmd : CmdData
    {
        public StopBGMCmd(SRC src, EventDataLine eventData) : base(src, CmdType.StopBGMCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Sound.KeepBGM = false;
            Sound.BossBGM = false;
            Sound.StopBGM(true);

            return EventData.ID + 1;
        }
    }
}
