using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ExpUpCmd : CmdData
    {
        public ExpUpCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ExpUpCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string pname;
            //            Pilot p;
            //            short prev_lv;
            //            double hp_ratio = default, en_ratio = default;
            //            short num;
            //            switch (ArgNum)
            //            {
            //                case 3:
            //                    {
            //                        p = GetArgAsPilot(2);
            //                        num = GetArgAsLong(3);
            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        {
            //                            var withBlock = Event.SelectedUnitForEvent;
            //                            if (withBlock.CountPilot() > 0)
            //                            {
            //                                p = withBlock.Pilot((object)1);
            //                            }
            //                            else
            //                            {
            //                                ExecExpUpCmdRet = LineNum + 1;
            //                                return ExecExpUpCmdRet;
            //                            }
            //                        }

            //                        num = GetArgAsLong(2);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "ExpUpコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 236403


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            if (p.Unit is object)
            //            {
            //                {
            //                    var withBlock1 = p.Unit;
            //                    hp_ratio = 100 * withBlock1.HP / (double)withBlock1.MaxHP;
            //                    en_ratio = 100 * withBlock1.EN / (double)withBlock1.MaxEN;
            //                }
            //            }

            //            prev_lv = p.Level;
            //            p.Exp = p.Exp + num;
            //            if (p.Level == prev_lv)
            //            {
            //                ExecExpUpCmdRet = LineNum + 1;
            //                return ExecExpUpCmdRet;
            //            }

            //            p.Update();

            //            // ＳＰ＆霊力をアップデート
            //            p.SP = p.SP;
            //            p.Plana = p.Plana;
            //            if (p.Unit is object)
            //            {
            //                {
            //                    var withBlock2 = p.Unit;
            //                    withBlock2.Update();
            //                    withBlock2.HP = (withBlock2.MaxHP * hp_ratio / 100d);
            //                    withBlock2.EN = (withBlock2.MaxEN * en_ratio / 100d);
            //                }

            //                SRC.PList.UpdateSupportMod(p.Unit);
            //            }

            //return EventData.NextID;
        }
    }
}
