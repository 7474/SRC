using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ShowUnitStatusCmd : CmdData
    {
        public ShowUnitStatusCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ShowUnitStatusCmd, eventData)
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
            //                        if (GetArgAsString(2) == "終了")
            //                        {
            //                            Status.ClearUnitStatus();
            //                            ExecShowUnitStatusCmdRet = LineNum + 1;
            //                            return ExecShowUnitStatusCmdRet;
            //                        }

            //                        u = GetArgAsUnit(2);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "ShowUnitStatusコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 472786


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            if (u is object)
            //            {
            //                Status.DisplayUnitStatus(u);
            //            }

            //return EventData.NextID;
        }
    }
}
