using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class LandCmd : CmdData
    {
        public LandCmd(SRC src, EventDataLine eventData) : base(src, CmdType.LandCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            Unit u1, u2;
            //            switch (ArgNum)
            //            {
            //                case 2:
            //                    {
            //                        u1 = Event.SelectedUnitForEvent;
            //                        u2 = GetArgAsUnit(2);
            //                        break;
            //                    }

            //                case 3:
            //                    {
            //                        u1 = GetArgAsUnit(2);
            //                        u2 = GetArgAsUnit(3);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "Landコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 300956


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            if (u1.IsFeatureAvailable("母艦"))
            //            {
            //                Event.EventErrorMessage = u1.Name + "は母艦なので格納出来ません";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 301106


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            if (!u2.IsFeatureAvailable("母艦"))
            //            {
            //                Event.EventErrorMessage = u2.Name + "は母艦能力を持っていません";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 301252


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            u1.Land(u2, true, true);
            //return EventData.NextID;
        }
    }
}
