using SRCCore.Events;

namespace SRCCore.CmdDatas.Commands
{
    public abstract class AIfCmd : CmdData
    {
        public AIfCmd(SRC src, CmdType name, EventDataLine eventData) : base(src, name, eventData)
        {
        }

        protected void PrepareArgs()
        {
            // TODO Impl
            //if (CmdName == CmdType.IfCmd | CmdName == CmdType.ElseIfCmd)
            //{
            //    // If文の処理の高速化のため、あらかじめ構文解析しておく
            //    if (ArgNum == 1)
            //    {
            //        // 書式エラー
            //        Event_Renamed.DisplayEventErrorMessage(Event_Renamed.CurrentLineNum, "Ifコマンドの書式に合っていません");
            //        ParseRet = false;
            //        return ParseRet;
            //    }

            //    expr = list[2];
            //    var loopTo2 = ArgNum;
            //    for (i = 3; i <= loopTo2; i++)
            //    {
            //        buf = list[i];
            //        switch (Strings.LCase(buf) ?? "")
            //        {
            //            case "then":
            //            case "exit":
            //                {
            //                    // UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
            //                    strArgs = new string[5];
            //                    // UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
            //                    lngArgs = new int[5];
            //                    // UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
            //                    dblArgs = new double[5];
            //                    // UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
            //                    ArgsType = new Expressions.ValueType[5];
            //                    strArgs[2] = expr;
            //                    lngArgs[3] = ArgNum - 2;
            //                    ArgsType[3] = Expressions.ValueType.NumericType;
            //                    strArgs[4] = Strings.LCase(buf);
            //                    break;
            //                }

            //            case "goto":
            //                {
            //                    buf = GetArg((int)(i + 1));
            //                    // UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
            //                    strArgs = new string[6];
            //                    // UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
            //                    lngArgs = new int[6];
            //                    // UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
            //                    dblArgs = new double[6];
            //                    // UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
            //                    ArgsType = new Expressions.ValueType[6];
            //                    strArgs[2] = expr;
            //                    lngArgs[3] = ArgNum - 3;
            //                    ArgsType[3] = Expressions.ValueType.NumericType;
            //                    strArgs[4] = "goto";
            //                    strArgs[5] = buf;
            //                    break;
            //                }

            //            case var case1 when case1 == "":
            //                {
            //                    buf = "\"\"";
            //                    break;
            //                }
            //        }

            //        expr = expr + " " + buf;
            //    }

            //    if (i > ArgNum)
            //    {
            //        if (CmdName == CmdType.IfCmd)
            //        {
            //            Event_Renamed.DisplayEventErrorMessage(LineNum, "Ifに対応する Then または Exit または Goto がありません");
            //        }
            //        else
            //        {
            //            Event_Renamed.DisplayEventErrorMessage(LineNum, "ElseIfに対応する Then または Exit または Goto がありません");
            //        }

            //        SRC.TerminateSRC();
            //    }

            //    // 条件式が式であることが確定していれば条件式の項数を0に
            //    switch (lngArgs[3])
            //    {
            //        case 0:
            //            {
            //                if (CmdName == CmdType.IfCmd)
            //                {
            //                    Event_Renamed.DisplayEventErrorMessage(LineNum, "Ifコマンドの条件式がありません");
            //                }
            //                else
            //                {
            //                    Event_Renamed.DisplayEventErrorMessage(LineNum, "ElseIfコマンドの条件式がありません");
            //                }

            //                SRC.TerminateSRC();
            //                break;
            //            }

            //        case 1:
            //            {
            //                switch (Strings.Asc(expr))
            //                {
            //                    case 36: // $
            //                        {
            //                            lngArgs[3] = 0;
            //                            break;
            //                        }

            //                    case 40: // (
            //                        {
            //                            // ()を除去
            //                            strArgs[2] = Strings.Mid(expr, 2, Strings.Len(expr) - 2);
            //                            lngArgs[3] = 0;
            //                            break;
            //                        }
            //                }

            //                break;
            //            }

            //        case 2:
            //            {
            //                if (Strings.LCase(GeneralLib.LIndex(expr, 1)) == "not")
            //                {
            //                    switch (Strings.Asc(GeneralLib.ListIndex(expr, 2)))
            //                    {
            //                        case 36:
            //                        case 40: // $, (
            //                            {
            //                                lngArgs[3] = 0;
            //                                break;
            //                            }
            //                    }
            //                }
            //                else
            //                {
            //                    lngArgs[3] = 0;
            //                }

            //                break;
            //            }

            //        default:
            //            {
            //                lngArgs[3] = 0;
            //                break;
            //            }
            //    }

            //    return ParseRet;
            //}
        }
    }
}
