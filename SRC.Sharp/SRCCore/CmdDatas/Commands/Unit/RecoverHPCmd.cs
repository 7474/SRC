using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RecoverHPCmd : CmdData
    {
        public RecoverHPCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RecoverHPCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            Unit u;
            //            double per;
            //            switch (ArgNum)
            //            {
            //                case 3:
            //                    {
            //                        u = GetArgAsUnit(2, true);
            //                        per = GetArgAsDouble(3);
            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        u = Event.SelectedUnitForEvent;
            //                        per = GetArgAsDouble(2);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "RecoverHPコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 406435


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            if (u is object)
            //            {
            //                u.RecoverHP(per);
            //                u.Update();
            //                u.CheckAutoHyperMode();
            //                u.CheckAutoNormalMode();
            //            }

            //return EventData.NextID;
        }
    }
}
