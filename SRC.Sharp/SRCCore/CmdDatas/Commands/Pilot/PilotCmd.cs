using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class PilotCmd : CmdData
    {
        public PilotCmd(SRC src, EventDataLine eventData) : base(src, CmdType.PilotCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum < 0)
            {
                throw new EventErrorException(this, "Pilotコマンドのパラメータの括弧の対応が取れていません");
            }
            else if (ArgNum != 3 & ArgNum != 4)
            {
                throw new EventErrorException(this, "Pilotコマンドの引数の数が違います");
            }

            var pname = GetArgAsString(2);
            if (!SRC.PDList.IsDefined(pname))
            {
                throw new EventErrorException(this, "指定したパイロット「" + pname + "」のデータが見つかりません");
            }

            var plevel = (short)GetArgAsLong(3);
            if (Expression.IsOptionDefined("レベル限界突破"))
            {
                if (plevel > 999)
                {
                    plevel = 999;
                }
            }
            else if (plevel > 99)
            {
                plevel = 99;
            }

            if (plevel < 1)
            {
                plevel = 1;
            }

            if (ArgNum == 3)
            {
                var p = SRC.PList.Add(pname, plevel, "味方", "");
                p.FullRecover();
            }
            else
            {
                string gid = GetArgAsString(4);
                var p = SRC.PList.Add(pname, plevel, "味方", gid);
                p.FullRecover();
            }

            return EventData.NextID;
        }
    }
}
