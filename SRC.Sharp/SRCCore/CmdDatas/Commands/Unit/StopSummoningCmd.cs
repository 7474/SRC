using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class StopSummoningCmd : CmdData
    {
        public StopSummoningCmd(SRC src, EventDataLine eventData) : base(src, CmdType.StopSummoningCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            Unit u;
            //            switch (ArgNum)
            //            {
            //                case 1:
            //                    {
            //                        u = Event.SelectedUnitForEvent;
            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        u = GetArgAsUnit(2);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "StopSummoningコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 497442


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            // 召喚したユニットを解放
            //            u.DismissServant();
            //return EventData.NextID;
        }
    }
}
