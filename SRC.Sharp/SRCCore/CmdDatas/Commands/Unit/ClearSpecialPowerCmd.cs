using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ClearSpecialPowerCmd : CmdData
    {
        public ClearSpecialPowerCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ClearSpecialPowerCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string sname;
            //            Unit u;
            //            switch (ArgNum)
            //            {
            //                case 3:
            //                    {
            //                        u = GetArgAsUnit(2);
            //                        sname = GetArgAsString(3);
            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        u = Event.SelectedUnitForEvent;
            //                        sname = GetArgAsString(2);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "ClearSpecialPowerコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 187952


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            if (u.IsSpecialPowerInEffect(sname))
            //            {
            //                u.RemoveSpecialPowerInEffect2(sname);
            //            }

            //return EventData.NextID;
        }
    }
}
