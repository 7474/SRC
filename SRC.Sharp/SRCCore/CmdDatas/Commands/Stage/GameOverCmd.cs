using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class GameOverCmd : CmdData
    {
        public GameOverCmd(SRC src, EventDataLine eventData) : base(src, CmdType.GameOverCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 1)
            {
                throw new EventErrorException(this, "GameOverコマンドの引数の数が違います");
            }

            SRC.GameOver();
            SRC.IsScenarioFinished = true;
            return -1;
        }
    }
}
