using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RecoverPlanaCmd : CmdData
    {
        public RecoverPlanaCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RecoverPlanaCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            Pilot p;
            //            double per;
            //            double hp_ratio = default, en_ratio = default;

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
            //                        Event.EventErrorMessage = "RecoverPlanaコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 407406


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            ExecRecoverPlanaCmdRet = LineNum + 1;
            //            if (p is null)
            //            {
            //                return ExecRecoverPlanaCmdRet;
            //            }

            //            if (p.MaxPlana() == 0)
            //            {
            //                return ExecRecoverPlanaCmdRet;
            //            }

            //            if (p.Unit is object)
            //            {
            //                {
            //                    var withBlock1 = p.Unit;
            //                    hp_ratio = 100 * withBlock1.HP / (double)withBlock1.MaxHP;
            //                    en_ratio = 100 * withBlock1.EN / (double)withBlock1.MaxEN;
            //                }
            //            }

            //            p.Plana = (p.Plana + per * p.MaxPlana() / 100d);
            //            if (p.Unit is object)
            //            {
            //                {
            //                    var withBlock2 = p.Unit;
            //                    withBlock2.HP = (withBlock2.MaxHP * hp_ratio / 100d);
            //                    withBlock2.EN = (withBlock2.MaxEN * en_ratio / 100d);
            //                }
            //            }

            //return EventData.NextID;
        }
    }
}
