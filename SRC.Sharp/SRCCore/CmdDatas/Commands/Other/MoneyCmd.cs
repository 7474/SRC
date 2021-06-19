using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class MoneyCmd : CmdData
    {
        public MoneyCmd(SRC src, EventDataLine eventData) : base(src, CmdType.MoneyCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 2)
            {
                throw new EventErrorException(this, "Moneyコマンドの引数の数が間違っています");
            }

            SRC.IncrMoney(GetArgAsLong(2));
            return EventData.NextID;
        }
    }
}
