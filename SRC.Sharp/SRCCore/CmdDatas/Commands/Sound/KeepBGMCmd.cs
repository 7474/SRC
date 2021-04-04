using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.VB;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class KeepBGMCmd : CmdData
    {
        public KeepBGMCmd(SRC src, EventDataLine eventData) : base(src, CmdType.KeepBGMCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 1)
            {
                throw new EventErrorException(this, "KeepBGMコマンドの引数の数が違います");
            }

            Sound.KeepBGM = true;

            return EventData.ID + 1;
        }
    }
}
