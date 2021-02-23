using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class ChangeModeCmd : CmdData
    {
        public ChangeModeCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ChangeModeCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            Unit[] uarrary;
            Unit u;
            string new_mode;
            string pname;
            short i;
            short dst_x, dst_y;
            var uarray = new object[2];
            switch (ArgNum)
            {
                case 2:
                    {
                        uarray[1] = Event_Renamed.SelectedUnitForEvent;
                        new_mode = GetArgAsString((short)2);
                        break;
                    }

                case 3:
                    {
                        if (GetArgAsLong((short)2) > 0 & GetArgAsLong((short)3) > 0)
                        {
                            uarray[1] = Event_Renamed.SelectedUnitForEvent;
                            dst_x = (short)GetArgAsLong((short)2);
                            dst_y = (short)GetArgAsLong((short)3);
                            if ((int)dst_x < 1 | Map.MapWidth < dst_x | (int)dst_y < 1 | Map.MapHeight < dst_y)
                            {
                                Event_Renamed.EventErrorMessage = "ChangeModeコマンドの目的地の座標が不正です";
                                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 134202


                                Input:
                                                        Error(0)

                                 */
                            }

                            new_mode = Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)dst_x) + " " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)dst_y);
                        }
                        else
                        {
                            pname = GetArgAsString((short)2);
                            switch (pname ?? "")
                            {
                                case "味方":
                                case "ＮＰＣ":
                                case "敵":
                                case "中立":
                                    {
                                        uarray = new object[1];
                                        foreach (Unit currentU in SRC.UList)
                                        {
                                            u = currentU;
                                            if ((u.Party0 ?? "") == (pname ?? ""))
                                            {
                                                Array.Resize(ref uarray, Information.UBound(uarray) + 1 + 1);
                                                uarray[Information.UBound(uarray)] = u;
                                            }
                                        }

                                        break;
                                    }

                                default:
                                    {
                                        object argIndex1 = (object)pname;
                                        uarray[1] = SRC.UList.Item2(ref argIndex1);
                                        if (uarray[1] is null)
                                        {
                                            {
                                                var withBlock = SRC.PList;
                                                bool localIsDefined() { object argIndex1 = (object)pname; var ret = withBlock.IsDefined(ref argIndex1); return ret; }

                                                if (!localIsDefined())
                                                {
                                                    Event_Renamed.EventErrorMessage = "「" + pname + "」というパイロットが見つかりません";
                                                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                                                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 135082


                                                    Input:
                                                                                            Error(0)

                                                     */
                                                }

                                                Pilot localItem() { object argIndex1 = (object)pname; var ret = withBlock.Item(ref argIndex1); return ret; }

                                                uarray[1] = localItem().Unit_Renamed;
                                                i = (short)2;
                                                object argIndex2 = (object)(pname + ":" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)i));
                                                while (withBlock.IsDefined(ref argIndex2))
                                                {
                                                    Array.Resize(ref uarray, Information.UBound(uarray) + 1 + 1);
                                                    Pilot localItem1() { object argIndex1 = (object)(pname + ":" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)i)); var ret = withBlock.Item(ref argIndex1); return ret; }

                                                    uarray[Information.UBound(uarray)] = localItem1().Unit_Renamed;
                                                    i = (short)((int)i + 1);
                                                }
                                            }
                                        }

                                        break;
                                    }
                            }

                            new_mode = GetArgAsString((short)3);
                        }

                        break;
                    }

                case 4:
                    {
                        pname = GetArgAsString((short)2);
                        switch (pname ?? "")
                        {
                            case "味方":
                            case "ＮＰＣ":
                            case "敵":
                            case "中立":
                                {
                                    uarray = new object[1];
                                    foreach (Unit currentU1 in SRC.UList)
                                    {
                                        u = currentU1;
                                        if ((u.Party0 ?? "") == (pname ?? ""))
                                        {
                                            Array.Resize(ref uarray, Information.UBound(uarray) + 1 + 1);
                                            uarray[Information.UBound(uarray)] = u;
                                        }
                                    }

                                    break;
                                }

                            default:
                                {
                                    object argIndex3 = (object)pname;
                                    uarray[1] = SRC.UList.Item2(ref argIndex3);
                                    if (uarray[1] is null)
                                    {
                                        {
                                            var withBlock1 = SRC.PList;
                                            bool localIsDefined1() { object argIndex1 = (object)pname; var ret = withBlock1.IsDefined(ref argIndex1); return ret; }

                                            if (!localIsDefined1())
                                            {
                                                Event_Renamed.EventErrorMessage = "「" + pname + "」というパイロットが見つかりません";
                                                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                                                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 136368


                                                Input:
                                                                                    Error(0)

                                                 */
                                            }

                                            Pilot localItem2() { object argIndex1 = (object)pname; var ret = withBlock1.Item(ref argIndex1); return ret; }

                                            uarray[1] = localItem2().Unit_Renamed;
                                            i = (short)2;
                                            object argIndex4 = (object)(pname + ":" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)i));
                                            while (withBlock1.IsDefined(ref argIndex4))
                                            {
                                                Array.Resize(ref uarray, Information.UBound(uarray) + 1 + 1);
                                                Pilot localItem3() { object argIndex1 = (object)(pname + ":" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)i)); var ret = withBlock1.Item(ref argIndex1); return ret; }

                                                uarray[Information.UBound(uarray)] = localItem3().Unit_Renamed;
                                                i = (short)((int)i + 1);
                                            }
                                        }
                                    }

                                    break;
                                }
                        }

                        dst_x = (short)GetArgAsLong((short)3);
                        dst_y = (short)GetArgAsLong((short)4);
                        if ((int)dst_x < 1 | Map.MapWidth < dst_x | (int)dst_y < 1 | Map.MapHeight < dst_y)
                        {
                            Event_Renamed.EventErrorMessage = "ChangeModeコマンドの目的地の座標が不正です";
                            ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 137170


                            Input:
                                                Error(0)

                             */
                        }

                        new_mode = Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)dst_x) + " " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)dst_y);
                        break;
                    }

                default:
                    {
                        Event_Renamed.EventErrorMessage = "ChangeModeコマンドの引数の数が違います";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 137438


                        Input:
                                        Error(0)

                         */
                        break;
                    }
            }

            var loopTo = (short)Information.UBound(uarray);
            for (i = 1; i <= loopTo; i++)
            {
                if (uarray[i] is object)
                {
                    // UPGRADE_WARNING: オブジェクト uarray().Mode の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    uarray[i].Mode = new_mode;
                }
            }

            return EventData.ID + 1;
        }
    }
}
