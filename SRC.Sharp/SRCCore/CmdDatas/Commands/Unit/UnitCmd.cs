using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Units;

namespace SRCCore.CmdDatas.Commands
{
    public class UnitCmd : CmdData
    {
        public UnitCmd(SRC src, EventDataLine eventData) : base(src, CmdType.UnitCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum < 0)
            {
                throw new EventErrorException(this, "Unitコマンドのパラメータの括弧の対応が取れていません");
            }
            else if (ArgNum != 3)
            {
                throw new EventErrorException(this, "Unitコマンドの引数の数が違います");
            }

            string uname = GetArgAsString(2);
            if (!SRC.UDList.IsDefined(uname))
            {
                throw new EventErrorException(this, "指定したユニット「" + uname + "」のデータが見つかりません");
            }

            var urank = GetArgAsLong(3);
            Unit u = SRC.UList.Add(uname, urank, "味方");
            if (u is null)
            {
                throw new EventErrorException(this, "のユニットデータが不正です");
            }

            Event.SelectedUnitForEvent = u;
            return EventData.NextID;
        }
    }
}
