using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.VB;
using System.Linq;

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
