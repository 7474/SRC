using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class RemovePilotCmd : CmdData
    {
        public RemovePilotCmd(SRC src, EventDataLine eventData) : base(src, CmdType.RemovePilotCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string pname;
            //            short i, num;
            //            Pilot p;
            //            var opt = default(string);
            //            num = ArgNum;
            //            if (num > 1)
            //            {
            //                if (GetArgAsString(num) == "非同期")
            //                {
            //                    opt = "非同期";
            //                    num = (num - 1);
            //                }
            //            }

            //            ExecRemovePilotCmdRet = LineNum + 1;
            //            switch (num)
            //            {
            //                case 1:
            //                    {
            //                        {
            //                            var withBlock = Event.SelectedUnitForEvent;
            //                            if (withBlock.CountPilot() == 0)
            //                            {
            //                                Event.EventErrorMessage = "指定されたユニットにパイロットが乗っていません";
            //                                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 425061


            //                                Input:
            //                                                        Error(0)

            //                                 */
            //                            }

            //                            if (withBlock.Status == "出撃")
            //                            {
            //                                withBlock.Escape(opt);
            //                            }

            //                            var loopTo = withBlock.CountPilot();
            //                            for (i = 1; i <= loopTo; i++)
            //                            {
            //                                Pilot localPilot() { object argIndex1 = (object)i; var ret = withBlock.Pilot(argIndex1); return ret; }

            //                                localPilot().Alive = false;
            //                            }

            //                            var loopTo1 = withBlock.CountSupport();
            //                            for (i = 1; i <= loopTo1; i++)
            //                            {
            //                                Pilot localSupport() { object argIndex1 = (object)i; var ret = withBlock.Support(argIndex1); return ret; }

            //                                localSupport().Alive = false;
            //                            }

            //                            withBlock.Status = "破棄";
            //                            var loopTo2 = withBlock.CountOtherForm();
            //                            for (i = 1; i <= loopTo2; i++)
            //                            {
            //                                Unit localOtherForm1() { object argIndex1 = (object)i; var ret = withBlock.OtherForm(argIndex1); return ret; }

            //                                if (localOtherForm1().Status == "他形態")
            //                                {
            //                                    Unit localOtherForm() { object argIndex1 = (object)i; var ret = withBlock.OtherForm(argIndex1); return ret; }

            //                                    localOtherForm().Status = "破棄";
            //                                }
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        pname = GetArgAsString(2);
            //                        bool localIsDefined() { object argIndex1 = (object)pname; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //                        if (!localIsDefined())
            //                        {
            //                            Event.EventErrorMessage = "「" + pname + "」というパイロットが見つかりません";
            //                            ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 425712


            //                            Input:
            //                                                Error(0)

            //                             */
            //                        }

            //                        p = SRC.PList.Item((object)pname);
            //                        p.Alive = false;
            //                        if (p.Unit is object)
            //                        {
            //                            {
            //                                var withBlock1 = p.Unit;
            //                                if ((p.ID ?? "") == (withBlock1.MainPilot().ID ?? "") || (p.ID ?? "") == (withBlock1.Pilot((object)1).ID ?? ""))
            //                                {
            //                                    // メインパイロットの場合はパイロット＆サポートを全員削除
            //                                    // ユニットも削除する
            //                                    if (withBlock1.Status == "出撃" || withBlock1.Status == "格納")
            //                                    {
            //                                        withBlock1.Escape(opt);
            //                                    }

            //                                    var loopTo3 = withBlock1.CountPilot();
            //                                    for (i = 1; i <= loopTo3; i++)
            //                                    {
            //                                        Pilot localPilot1() { object argIndex1 = (object)i; var ret = withBlock1.Pilot(argIndex1); return ret; }

            //                                        localPilot1().Alive = false;
            //                                    }

            //                                    var loopTo4 = withBlock1.CountSupport();
            //                                    for (i = 1; i <= loopTo4; i++)
            //                                    {
            //                                        Pilot localSupport1() { object argIndex1 = (object)i; var ret = withBlock1.Support(argIndex1); return ret; }

            //                                        localSupport1().Alive = false;
            //                                    }

            //                                    withBlock1.Status = "破棄";
            //                                    var loopTo5 = withBlock1.CountOtherForm();
            //                                    for (i = 1; i <= loopTo5; i++)
            //                                    {
            //                                        Unit localOtherForm3() { object argIndex1 = (object)i; var ret = withBlock1.OtherForm(argIndex1); return ret; }

            //                                        if (localOtherForm3().Status == "他形態")
            //                                        {
            //                                            Unit localOtherForm2() { object argIndex1 = (object)i; var ret = withBlock1.OtherForm(argIndex1); return ret; }

            //                                            localOtherForm2().Status = "破棄";
            //                                        }
            //                                    }
            //                                }
            //                                else
            //                                {
            //                                    // メインパイロットが対象でなければ指定されたパイロットのみを削除
            //                                    var loopTo6 = withBlock1.CountPilot();
            //                                    for (i = 1; i <= loopTo6; i++)
            //                                    {
            //                                        Pilot localPilot2() { object argIndex1 = (object)i; var ret = withBlock1.Pilot(argIndex1); return ret; }

            //                                        if ((p.ID ?? "") == (localPilot2().ID ?? ""))
            //                                        {
            //                                            withBlock1.DeletePilot((object)i);
            //                                            // UPGRADE_NOTE: オブジェクト p.Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                                            p.Unit = null;
            //                                            return ExecRemovePilotCmdRet;
            //                                        }
            //                                    }

            //                                    var loopTo7 = withBlock1.CountSupport();
            //                                    for (i = 1; i <= loopTo7; i++)
            //                                    {
            //                                        Pilot localSupport2() { object argIndex1 = (object)i; var ret = withBlock1.Support(argIndex1); return ret; }

            //                                        if ((p.ID ?? "") == (localSupport2().ID ?? ""))
            //                                        {
            //                                            withBlock1.DeleteSupport((object)i);
            //                                            // UPGRADE_NOTE: オブジェクト p.Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                                            p.Unit = null;
            //                                            return ExecRemovePilotCmdRet;
            //                                        }
            //                                    }
            //                                }
            //                            }
            //                        }

            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "RemovePilotの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 427433


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //return EventData.NextID;
        }
    }
}
