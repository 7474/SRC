using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class GotoCmd : CmdData
    {
        public GotoCmd(SRC src, EventDataLine eventData) : base(src, CmdType.GotoCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            int ExecGotoCmdRet;
            if (ArgNum != 2)
            {
                throw new EventErrorException(this, "Gotoコマンドの引数の数が違います");
            }

            // ラベルが式でないと仮定
            string arglname = GetArg(2);
            ExecGotoCmdRet = Event.FindLabel(arglname);

            // ラベルが見つかった？
            if (ExecGotoCmdRet > 0)
            {
                return ExecGotoCmdRet + 1;
            }

            // ラベルは式？
            string arglname1 = GetArgAsString(2);
            ExecGotoCmdRet = Event.FindLabel(arglname1);
            if (ExecGotoCmdRet == 0)
            {
                throw new EventErrorException(this, "ラベル「" + GetArg(2) + "」がみつかりません");
            }

            return ExecGotoCmdRet + 1;
        }
    }
}
