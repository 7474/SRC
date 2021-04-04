using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class GameClearCmd : CmdData
    {
        public GameClearCmd(SRC src, EventDataLine eventData) : base(src, CmdType.GameClearCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 1)
            {
                throw new EventErrorException(this, "GameClearコマンドの引数の数が違います");
            }

            SRC.GameClear();
            return -1;
        }
    }
}
