using SRCCore.Events;

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
