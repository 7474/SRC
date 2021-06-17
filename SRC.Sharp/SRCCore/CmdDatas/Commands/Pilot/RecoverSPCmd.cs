using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RecoverSPCmd : CmdData
    {
        public RecoverSPCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RecoverSPCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            Pilot p;
            //            double per;

            //            // UPGRADE_NOTE: オブジェクト p をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //            p = null;
            //            switch (ArgNum)
            //            {
            //                case 3:
            //                    {
            //                        p = GetArgAsPilot(2);
            //                        per = GetArgAsDouble(3);
            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        {
            //                            var withBlock = Event.SelectedUnitForEvent;
            //                            if (withBlock.CountPilot() > 0)
            //                            {
            //                                p = withBlock.MainPilot();
            //                            }
            //                        }

            //                        per = GetArgAsDouble(2);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "RecoverSPコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 408702


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            if (p is object)
            //            {
            //                if (p.MaxSP > 0)
            //                {
            //                    p.SP = (p.SP + per * p.MaxSP / 100d);
            //                }
            //            }

            //return EventData.NextID;
        }
    }
}
