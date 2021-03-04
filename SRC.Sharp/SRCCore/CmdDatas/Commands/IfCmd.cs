using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public class IfCmd : AIfCmd
    {
        public IfCmd(SRC src, EventDataLine eventData) : base(src, CmdType.IfCmd, eventData)
        {
            PrepareArgs();
        }

        protected override int ExecInternal()
        {
            //int ExecIfCmdRet = default;
            //string expr;
            //int i;
            //short depth;
            //string pname;
            //bool flag;
            //int ret;
            //expr = GetArg(2);

            //// Ifコマンドはあらかじめ構文解析されていて、第3引数に条件式の項数
            //// が入っている
            //switch (GetArgAsLong(3))
            //{
            //    case 1:
            //        {
            //            object argIndex2 = expr;
            //            if (SRC.PList.IsDefined(ref argIndex2))
            //            {
            //                object argIndex1 = expr;
            //                {
            //                    var withBlock = SRC.PList.Item(ref argIndex1);
            //                    if (withBlock.Unit_Renamed is null)
            //                    {
            //                        flag = false;
            //                    }
            //                    else
            //                    {
            //                        {
            //                            var withBlock1 = withBlock.Unit_Renamed;
            //                            if (withBlock1.Status_Renamed == "出撃" | withBlock1.Status_Renamed == "格納")
            //                            {
            //                                flag = true;
            //                            }
            //                            else
            //                            {
            //                                flag = false;
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //            else if (Expression.GetValueAsLong(ref expr, true) != 0)
            //            {
            //                flag = true;
            //            }
            //            else
            //            {
            //                flag = false;
            //            }

            //            break;
            //        }

            //    case 2:
            //        {
            //            pname = GeneralLib.ListIndex(ref expr, 2);
            //            object argIndex4 = pname;
            //            if (SRC.PList.IsDefined(ref argIndex4))
            //            {
            //                object argIndex3 = pname;
            //                {
            //                    var withBlock2 = SRC.PList.Item(ref argIndex3);
            //                    if (withBlock2.Unit_Renamed is null)
            //                    {
            //                        flag = true;
            //                    }
            //                    else
            //                    {
            //                        {
            //                            var withBlock3 = withBlock2.Unit_Renamed;
            //                            if (withBlock3.Status_Renamed == "出撃" | withBlock3.Status_Renamed == "格納")
            //                            {
            //                                flag = false;
            //                            }
            //                            else
            //                            {
            //                                flag = true;
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //            else if (Expression.GetValueAsLong(ref pname, true) == 0)
            //            {
            //                flag = true;
            //            }
            //            else
            //            {
            //                flag = false;
            //            }

            //            break;
            //        }

            //    default:
            //        {
            //            if (Expression.GetValueAsLong(ref expr) != 0)
            //            {
            //                flag = true;
            //            }
            //            else
            //            {
            //                flag = false;
            //            }

            //            break;
            //        }
            //}

            //switch (GetArg((short)4) ?? "")
            //{
            //    case "exit":
            //        {
            //            if (flag)
            //            {
            //                ExecIfCmdRet = 0;
            //            }
            //            else
            //            {
            //                ExecIfCmdRet = LineNum + 1;
            //            }

            //            break;
            //        }

            //    case "goto":
            //        {
            //            if (flag)
            //            {
            //                string arglname = GetArg((short)5);
            //                ret = Event_Renamed.FindLabel(ref arglname);
            //                if (ret == 0)
            //                {
            //                    string arglname1 = GetArgAsString((short)5);
            //                    ret = Event_Renamed.FindLabel(ref arglname1);
            //                    if (ret == 0)
            //                    {
            //                        Event_Renamed.EventErrorMessage = "ラベル「" + GetArg((short)5) + "」がみつかりません";
            //                        ;
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 290328


            //                        Input:
            //                                                    Error(0)

            //                         */
            //                    }
            //                }

            //                ExecIfCmdRet = ret + 1;
            //            }
            //            else
            //            {
            //                ExecIfCmdRet = LineNum + 1;
            //            }

            //            break;
            //        }

            //    case "then":
            //        {
            //            if (flag)
            //            {
            //                // Then節をそのまま実行
            //                ExecIfCmdRet = LineNum + 1;
            //                return ExecIfCmdRet;
            //            }

            //            // 条件式が成り立たない場合はElse節もしくはEndIfを探す
            //            depth = (short)1;
            //            var loopTo = Information.UBound(Event_Renamed.EventCmd);
            //            for (i = LineNum + 1; i <= loopTo; i++)
            //            {
            //                {
            //                    var withBlock4 = Event_Renamed.EventCmd[i];
            //                    switch (withBlock4.Name)
            //                    {
            //                        case Event_Renamed.CmdType.IfCmd:
            //                            {
            //                                if (withBlock4.GetArg((short)4) == "then")
            //                                {
            //                                    depth = (short)((int)depth + 1);
            //                                }

            //                                break;
            //                            }

            //                        case Event_Renamed.CmdType.ElseCmd:
            //                            {
            //                                if ((int)depth == 1)
            //                                {
            //                                    break;
            //                                }

            //                                break;
            //                            }

            //                        case Event_Renamed.CmdType.ElseIfCmd:
            //                            {
            //                                if ((int)depth != 1)
            //                                {
            //                                    goto NextLoop;
            //                                }
            //                                // 条件式が成り立つか判定
            //                                expr = withBlock4.GetArg((short)2);
            //                                switch (withBlock4.GetArgAsLong((short)3))
            //                                {
            //                                    case 1:
            //                                        {
            //                                            object argIndex6 = (object)expr;
            //                                            if (SRC.PList.IsDefined(ref argIndex6))
            //                                            {
            //                                                object argIndex5 = (object)expr;
            //                                                {
            //                                                    var withBlock5 = SRC.PList.Item(ref argIndex5);
            //                                                    if (withBlock5.Unit_Renamed is null)
            //                                                    {
            //                                                        flag = false;
            //                                                    }
            //                                                    else
            //                                                    {
            //                                                        {
            //                                                            var withBlock6 = withBlock5.Unit_Renamed;
            //                                                            if (withBlock6.Status_Renamed == "出撃" | withBlock6.Status_Renamed == "格納")
            //                                                            {
            //                                                                flag = true;
            //                                                            }
            //                                                            else
            //                                                            {
            //                                                                flag = false;
            //                                                            }
            //                                                        }
            //                                                    }
            //                                                }
            //                                            }
            //                                            else if (Expression.GetValueAsLong(ref expr, true) != 0)
            //                                            {
            //                                                flag = true;
            //                                            }
            //                                            else
            //                                            {
            //                                                flag = false;
            //                                            }

            //                                            break;
            //                                        }

            //                                    case 2:
            //                                        {
            //                                            pname = GeneralLib.ListIndex(ref expr, (short)2);
            //                                            object argIndex8 = (object)pname;
            //                                            if (SRC.PList.IsDefined(ref argIndex8))
            //                                            {
            //                                                object argIndex7 = (object)pname;
            //                                                {
            //                                                    var withBlock7 = SRC.PList.Item(ref argIndex7);
            //                                                    if (withBlock7.Unit_Renamed is null)
            //                                                    {
            //                                                        flag = true;
            //                                                    }
            //                                                    else
            //                                                    {
            //                                                        {
            //                                                            var withBlock8 = withBlock7.Unit_Renamed;
            //                                                            if (withBlock8.Status_Renamed == "出撃" | withBlock8.Status_Renamed == "格納")
            //                                                            {
            //                                                                flag = false;
            //                                                            }
            //                                                            else
            //                                                            {
            //                                                                flag = true;
            //                                                            }
            //                                                        }
            //                                                    }
            //                                                }
            //                                            }
            //                                            else if (Expression.GetValueAsLong(ref pname, true) == 0)
            //                                            {
            //                                                flag = true;
            //                                            }
            //                                            else
            //                                            {
            //                                                flag = false;
            //                                            }

            //                                            break;
            //                                        }

            //                                    default:
            //                                        {
            //                                            if (Expression.GetValueAsLong(ref expr) != 0)
            //                                            {
            //                                                flag = true;
            //                                            }
            //                                            else
            //                                            {
            //                                                flag = false;
            //                                            }

            //                                            break;
            //                                        }
            //                                }

            //                                if (flag)
            //                                {
            //                                    break;
            //                                }

            //                                break;
            //                            }

            //                        case Event_Renamed.CmdType.EndIfCmd:
            //                            {
            //                                depth = (short)((int)depth - 1);
            //                                if ((int)depth == 0)
            //                                {
            //                                    break;
            //                                }

            //                                break;
            //                            }
            //                    }
            //                }

            //            NextLoop:
            //                ;
            //            }

            //            if (i > Information.UBound(Event_Renamed.EventData))
            //            {
            //                Event_Renamed.EventErrorMessage = "IfとEndIfが対応していません";
            //                ;
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 293399


            //                Input:
            //                                    Error(0)

            //                 */
            //            }

            //            ExecIfCmdRet = i + 1;
            //            break;
            //        }

            //    default:
            //        {
            //            Event_Renamed.EventErrorMessage = "If行には Goto, Exit, Then のいずれかを指定して下さい";
            //            ;
            //            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 293568
            //            Input:
            //                            Error(0)
            //             */
            //            break;
            //        }

            //        return ExecIfCmdRet;
            //}

            return EventData.ID + 1;
        }
    }
}
