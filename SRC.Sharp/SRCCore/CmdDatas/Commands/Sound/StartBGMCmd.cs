using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class StartBGMCmd : ABGMCmd
    {
        public StartBGMCmd(SRC src, EventDataLine eventData) : base(src, CmdType.StartBGMCmd, eventData)
        {
        }

        protected override bool Repeat => true;
    }
}
