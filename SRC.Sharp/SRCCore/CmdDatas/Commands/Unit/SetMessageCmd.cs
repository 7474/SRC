using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class SetMessageCmd : CmdData
    {
        public SetMessageCmd(SRC src, EventDataLine eventData) : base(src, CmdType.SetMessageCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            Unit u;
            //            string pname = default, pname0 = default;
            //            string sit;
            //            string selected_msg;
            //            switch (ArgNum)
            //            {
            //                case 4:
            //                    {
            //                        pname = GetArgAsString(2);
            //                        u = SRC.UList.Item2((object)pname);
            //                        if (u is null)
            //                        {
            //                            {
            //                                var withBlock = SRC.PList;
            //                                bool localIsDefined1() { object argIndex1 = (object)pname; var ret = withBlock.IsDefined(argIndex1); return ret; }

            //                                if (!localIsDefined1())
            //                                {
            //                                    pname0 = pname;
            //                                    if (Strings.InStr(pname0, "(") > 0)
            //                                    {
            //                                        pname0 = Strings.Left(pname0, GeneralLib.InStr2(pname0, "(") - 1);
            //                                    }

            //                                    bool localIsDefined() { object argIndex1 = (object)pname0; var ret = withBlock.IsDefined(argIndex1); return ret; }

            //                                    if (!localIsDefined())
            //                                    {
            //                                        Event.EventErrorMessage = "「" + pname + "」というパイロットが見つかりません";
            //                                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 452698


            //                                        Input:
            //                                                                        Error(0)

            //                                         */
            //                                    }

            //                                    Pilot localItem() { object argIndex1 = (object)pname0; var ret = withBlock.Item(argIndex1); return ret; }

            //                                    u = localItem().Unit;
            //                                }
            //                                else
            //                                {
            //                                    Pilot localItem1() { object argIndex1 = (object)pname; var ret = withBlock.Item(argIndex1); return ret; }

            //                                    u = localItem1().Unit;
            //                                }
            //                            }

            //                            if (u is null)
            //                            {
            //                                Event.EventErrorMessage = "「" + pname + "」はユニットに乗っていません";
            //                                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 452962


            //                                Input:
            //                                                        Error(0)

            //                                 */
            //                            }
            //                        }
            //                        else if (u.CountPilot() == 0)
            //                        {
            //                            Event.EventErrorMessage = "指定されたユニットにはパイロットが乗っていません";
            //                            ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 453108


            //                            Input:
            //                                                Error(0)

            //                             */
            //                        }

            //                        sit = GetArgAsString(3);
            //                        selected_msg = GetArgAsString(4);
            //                        break;
            //                    }

            //                case 3:
            //                    {
            //                        u = Event.SelectedUnitForEvent;
            //                        if (u.CountPilot() == 0)
            //                        {
            //                            Event.EventErrorMessage = "指定されたユニットにはパイロットが乗っていません";
            //                            ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 453392


            //                            Input:
            //                                                Error(0)

            //                             */
            //                        }

            //                        sit = GetArgAsString(2);
            //                        selected_msg = GetArgAsString(3);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Event.EventErrorMessage = "SetMessageコマンドの引数の数が違います";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 453587


            //                        Input:
            //                                        Error(0)

            //                         */
            //                        break;
            //                    }
            //            }

            //            if (selected_msg == "解除")
            //            {
            //                // メッセージ用変数を削除
            //                Expression.UndefineVariable("Message(" + u.MainPilot().ID + "," + sit + ")");
            //            }
            //            else if (!string.IsNullOrEmpty(pname0))
            //            {
            //                // 表情指定付きメッセージをローカル変数として登録する
            //                Expression.SetVariableAsString("Message(" + u.MainPilot().ID + "," + sit + ")", pname + "::" + selected_msg);
            //            }
            //            else
            //            {
            //                // メッセージをローカル変数として登録する
            //                Expression.SetVariableAsString("Message(" + u.MainPilot().ID + "," + sit + ")", selected_msg);
            //            }
            //return EventData.NextID;
        }
    }
}
