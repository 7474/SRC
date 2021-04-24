using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class PlayMIDICmd : ABGMCmd
    {
        public PlayMIDICmd(SRC src, EventDataLine eventData) : base(src, CmdType.PlayMIDICmd, eventData)
        {
        }

        protected override bool Repeat => false;
    }
}
