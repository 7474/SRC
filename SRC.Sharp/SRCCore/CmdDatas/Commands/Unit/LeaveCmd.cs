using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    // 分類上はパイロット操作、ユニット操作
    public class LeaveCmd : CmdData
    {
        public LeaveCmd(SRC src, EventDataLine eventData) : base(src, CmdType.LeaveCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            string pname = default, vname;
            //            var u = default(Unit);
            //            short i, num;
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

            //            switch (num)
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

            //                            vname = "IsAway(" + localItem1().Name + ")";
            //                            if (!Expression.IsGlobalVariableDefined(vname))
            //                            {
            //                                Expression.DefineGlobalVariable(vname);
            //                            }

            //                            Expression.SetVariableAsLong(vname, 1);
            //                            ExecLeaveCmdRet = LineNum + 1;
            //                            return ExecLeaveCmdRet;
            //                        }
            //                        else if (localIsDefined1())
            //                        {
            //                            Unit localItem2() { object argIndex1 = (object)pname; var ret = SRC.UList.Item(argIndex1); return ret; }

            //                            if ((pname ?? "") == (localItem2().ID ?? ""))
            //                            {
            //                                u = SRC.UList.Item((object)pname);
            //                            }
            //                            else
            //                            {
            //                                foreach (Unit currentU in SRC.UList)
            //                                {
            //                                    u = currentU;
            //                                    if ((u.Name ?? "") == (pname ?? "") && u.Party0 == "味方" && u.CurrentForm().Status != "離脱")
            //                                    {
            //                                        u = u.CurrentForm();
            //                                        break;
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
            //                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 305201


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
            //                        Event.EventErrorMessage = "Leaveコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 305389


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            if (u is null)
            //            {
            //                Pilot localItem3() { object argIndex1 = pname; var ret = SRC.PList.Item(argIndex1); return ret; }

            //                localItem3().Away = true;
            //            }
            //            else
            //            {
            //                if (u.Status == "出撃" || u.Status == "格納")
            //                {
            //                    u.Escape(opt);
            //                }

            //                if (u.Party0 != "味方")
            //                {
            //                    u.ChangeParty("味方");
            //                }

            //                if (u.Status != "他形態" && u.Status != "旧主形態" && u.Status != "旧形態")
            //                {
            //                    u.Status = "離脱";
            //                }

            //                var loopTo = u.CountPilot();
            //                for (i = 1; i <= loopTo; i++)
            //                {
            //                    Pilot localPilot() { object argIndex1 = i; var ret = u.Pilot(argIndex1); return ret; }

            //                    localPilot().Away = true;
            //                }

            //                var loopTo1 = u.CountSupport();
            //                for (i = 1; i <= loopTo1; i++)
            //                {
            //                    Pilot localSupport() { object argIndex1 = i; var ret = u.Support(argIndex1); return ret; }

            //                    localSupport().Away = true;
            //                }
            //            }

            //return EventData.NextID;
        }
    }
}
