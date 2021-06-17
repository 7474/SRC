using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class MoneyCmd : CmdData
    {
        public MoneyCmd(SRC src, EventDataLine eventData) : base(src, CmdType.MoneyCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            if (ArgNum != 2)
            //            {
            //                Event.EventErrorMessage = "Moneyコマンドの引数の数が間違っています";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 343971


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            SRC.IncrMoney(GetArgAsLong(2));
            //return EventData.NextID;
        }
    }
}
