using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.VB;
using System.Linq;

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
