using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    // 分類上はパイロット操作、ユニット操作
    public class JoinCmd : CmdData
    {
        public JoinCmd(SRC src, EventDataLine eventData) : base(src, CmdType.JoinCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            var pname = default(string);
            //            var u = default(Unit);
            //            short i;
            //            switch (ArgNum)
            //            {
            //                case 2:
            //                    {
            //                        pname = GetArgAsString(2);
            //                        bool localIsDefined() { object argIndex1 = (object)pname; var ret = SRC.NPDList.IsDefined(argIndex1); return ret; }

            //                        bool localIsDefined1() { object argIndex1 = (object)pname; var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                        if (SRC.PList.IsDefined((object)pname))
            //                        {
            //                            Pilot localItem() { object argIndex1 = (object)pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                            u = localItem().Unit;
            //                        }
            //                        else if (localIsDefined())
            //                        {
            //                            NonPilotData localItem1() { object argIndex1 = (object)pname; var ret = SRC.NPDList.Item(argIndex1); return ret; }

            //                            pname = "IsAway(" + localItem1().Name + ")";
            //                            if (Expression.IsGlobalVariableDefined(pname))
            //                            {
            //                                Expression.UndefineVariable(pname);
            //                            }

            //                            ExecJoinCmdRet = LineNum + 1;
            //                            return ExecJoinCmdRet;
            //                        }
            //                        else if (localIsDefined1())
            //                        {
            //                            Unit localItem2() { object argIndex1 = (object)pname; var ret = SRC.UList.Item(argIndex1); return ret; }

            //                            if ((pname ?? "") == (localItem2().ID ?? ""))
            //                            {
            //                                // UPGRADE_WARNING: オブジェクト u の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //                                u = SRC.UList.Item((object)pname);
            //                            }
            //                            else
            //                            {
            //                                foreach (Unit currentU in SRC.UList)
            //                                {
            //                                    u = currentU;
            //                                    {
            //                                        var withBlock = u;
            //                                        if ((withBlock.Name ?? "") == (pname ?? "") && withBlock.Party0 == "味方" && withBlock.CurrentForm().Status == "離脱")
            //                                        {
            //                                            u = withBlock.CurrentForm();
            //                                            break;
            //                                        }
            //                                    }
            //                                }

            //                                if ((u.Name ?? "") != (pname ?? ""))
            //                                {
            //                                    // UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //                                    u = null;
            //                                }
            //                            }
            //                        }
            //                        else
            //                        {
            //                            Event.EventErrorMessage = "「" + pname + "」というパイロットまたはユニットが見つかりません";
            //                            ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 299677


            //                            Input:
            //                                                Error(0)

            //                             */
            //                        }

            //                        break;
            //                    }

            //                case 1:
            //                    {
            //                        u = Event.SelectedUnitForEvent;
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "Joinコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 299864


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            if (u is null)
            //            {
            //                if (SRC.PList.IsDefined(pname))
            //                {
            //                    Pilot localItem3() { object argIndex1 = pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                    localItem3().Away = false;
            //                }
            //            }
            //            else
            //            {
            //                u.Status = "待機";
            //                var loopTo = u.CountPilot();
            //                for (i = 1; i <= loopTo; i++)
            //                {
            //                    Pilot localPilot() { object argIndex1 = i; var ret = u.Pilot(argIndex1); return ret; }

            //                    localPilot().Away = false;
            //                }

            //                var loopTo1 = u.CountSupport();
            //                for (i = 1; i <= loopTo1; i++)
            //                {
            //                    Pilot localSupport() { object argIndex1 = i; var ret = u.Support(argIndex1); return ret; }

            //                    localSupport().Away = false;
            //                }
            //            }

            //return EventData.NextID;
        }
    }
}
