using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SupplyCmd : CmdData
    {
        public SupplyCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SupplyCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            Unit u;
            //            switch (ArgNum)
            //            {
            //                case 2:
            //                    {
            //                        u = GetArgAsUnit(2);
            //                        break;
            //                    }

            //                case 1:
            //                    {
            //                        u = Event.SelectedUnitForEvent;
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "Supplyコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 499859


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            if (u is object)
            //            {
            //                u.FullSupply();
            //            }

            //return EventData.NextID;
        }
    }
}
