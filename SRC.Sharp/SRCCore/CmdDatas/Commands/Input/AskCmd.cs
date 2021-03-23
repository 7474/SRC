using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Expressions;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class AskCmd : CmdData
    {
        public AskCmd(SRC src, EventDataLine eventData) : base(src, CmdType.AskCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            int ExecAskCmdRet = default;
            var use_normal_list = default(bool);
            var use_large_list = default(bool);
            var use_continuous_mode = default(bool);
            var enable_rbutton_cancel = default(bool);
            string[] list;
            string msg;
            string vname;
            int i;
            string buf;
            VarData var;
            list = new string[1];
            GUI.ListItemID = new string[1];
            GUI.ListItemFlag = new bool[1];

            // 表示オプションの処理
            i = ArgNum;
            while (i > 1)
            {
                switch (GetArg((short)i) ?? "")
                {
                    case "通常":
                        {
                            use_normal_list = true;
                            break;
                        }

                    case "拡大":
                        {
                            use_large_list = true;
                            break;
                        }

                    case "連続表示":
                        {
                            use_continuous_mode = true;
                            break;
                        }

                    case "キャンセル可":
                        {
                            enable_rbutton_cancel = true;
                            break;
                        }

                    case "終了":
                        {
                            My.MyProject.Forms.frmListBox.Hide();
                            if (SRC.AutoMoveCursor)
                            {
                                GUI.RestoreCursorPos();
                            }

                            GUI.ReduceListBoxHeight();
                            ExecAskCmdRet = LineNum + 1;
                            return ExecAskCmdRet;
                        }

                    default:
                        {
                            break;
                        }
                }

                i = i - 1;
            }

            // オプションではない引数の数で書式タイプを判別
            switch (i)
            {
                // イベントデータ中に選択肢を列挙している場合
                case 1:
                case 2:
                    {
                        if ((int)ArgNum == 1)
                        {
                            msg = "いずれかを選んでください";
                        }
                        else
                        {
                            msg = GetArgAsString((short)2);
                        }

                        GUI.ListItemID[0] = "0";

                        // 選択肢の読みこみ
                        var loopTo = Information.UBound(Event_Renamed.EventData);
                        for (i = LineNum + 1; i <= loopTo; i++)
                        {
                            buf = Event_Renamed.EventData[i];
                            Expression.FormatMessage(ref buf);
                            if (Strings.Len(buf) > 0)
                            {
                                if (Event_Renamed.EventCmd[i].Name == Event_Renamed.CmdType.EndCmd)
                                {
                                    break;
                                }

                                Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                                Array.Resize(ref GUI.ListItemID, Information.UBound(list) + 1);
                                Array.Resize(ref GUI.ListItemFlag, Information.UBound(list) + 1);
                                list[Information.UBound(list)] = buf;
                                GUI.ListItemID[Information.UBound(list)] = Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)(i - LineNum));
                                GUI.ListItemFlag[Information.UBound(list)] = false;
                            }
                        }

                        if (i > Information.UBound(Event_Renamed.EventData))
                        {
                            Event_Renamed.EventErrorMessage = "AskとEndが対応していません";
                            ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 80314


                            Input:
                                                Error(0)

                             */
                        }

                        ExecAskCmdRet = i + 1;
                        break;
                    }

                // 選択肢を配列で指定する場合
                case 3:
                    {
                        vname = GetArg((short)2);
                        msg = GetArgAsString((short)3);
                        GUI.ListItemID[0] = "";

                        // 配列の検索
                        if (Expression.IsSubLocalVariableDefined(ref vname))
                        {
                            if (Strings.Left(vname, 1) == "$")
                            {
                                vname = Strings.Mid(vname, 2) + "[";
                            }
                            else
                            {
                                vname = vname + "[";
                            }

                            var loopTo1 = (int)Event_Renamed.VarIndex;
                            for (i = (int)Event_Renamed.VarIndexStack[(int)Event_Renamed.CallDepth - 1] + 1; i <= loopTo1; i++)
                            {
                                {
                                    var withBlock = Event_Renamed.VarStack[i];
                                    if (Strings.InStr(withBlock.Name, vname) == 1)
                                    {
                                        if (withBlock.VariableType == Expression.ValueType.StringType)
                                        {
                                            buf = withBlock.StringValue;
                                        }
                                        else
                                        {
                                            buf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)withBlock.NumericValue);
                                        }

                                        if (Strings.Len(buf) > 0)
                                        {
                                            Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                                            Array.Resize(ref GUI.ListItemID, Information.UBound(list) + 1);
                                            Array.Resize(ref GUI.ListItemFlag, Information.UBound(list) + 1);
                                            Expression.FormatMessage(ref buf);
                                            list[Information.UBound(list)] = buf;
                                            GUI.ListItemID[Information.UBound(list)] = Strings.Mid(withBlock.Name, Strings.Len(vname) + 1, Strings.Len(withBlock.Name) - Strings.Len(vname) - 1);
                                            GUI.ListItemFlag[Information.UBound(list)] = false;
                                        }
                                    }
                                }
                            }
                        }
                        else if (Expression.IsLocalVariableDefined(ref vname))
                        {
                            if (Strings.Left(vname, 1) == "$")
                            {
                                vname = Strings.Mid(vname, 2) + "[";
                            }
                            else
                            {
                                vname = vname + "[";
                            }

                            foreach (VarData currentVar in Event_Renamed.LocalVariableList)
                            {
                                var = currentVar;
                                if (Strings.InStr(var.Name, vname) == 1)
                                {
                                    if (var.VariableType == Expression.ValueType.StringType)
                                    {
                                        buf = var.StringValue;
                                    }
                                    else
                                    {
                                        buf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)var.NumericValue);
                                    }

                                    if (Strings.Len(buf) > 0)
                                    {
                                        Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                                        Array.Resize(ref GUI.ListItemID, Information.UBound(list) + 1);
                                        Array.Resize(ref GUI.ListItemFlag, Information.UBound(list) + 1);
                                        Expression.FormatMessage(ref buf);
                                        list[Information.UBound(list)] = buf;
                                        GUI.ListItemID[Information.UBound(list)] = Strings.Mid(var.Name, Strings.Len(vname) + 1, Strings.Len(var.Name) - Strings.Len(vname) - 1);
                                        GUI.ListItemFlag[Information.UBound(list)] = false;
                                    }
                                }
                            }
                        }
                        else if (Expression.IsGlobalVariableDefined(ref vname))
                        {
                            if (Strings.Left(vname, 1) == "$")
                            {
                                vname = Strings.Mid(vname, 2) + "[";
                            }
                            else
                            {
                                vname = vname + "[";
                            }

                            foreach (VarData currentVar1 in Event_Renamed.GlobalVariableList)
                            {
                                var = currentVar1;
                                if (Strings.InStr(var.Name, vname) == 1)
                                {
                                    if (var.VariableType == Expression.ValueType.StringType)
                                    {
                                        buf = var.StringValue;
                                    }
                                    else
                                    {
                                        buf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)var.NumericValue);
                                    }

                                    if (Strings.Len(buf) > 0)
                                    {
                                        Array.Resize(ref list, Information.UBound(list) + 1 + 1);
                                        Array.Resize(ref GUI.ListItemID, Information.UBound(list) + 1);
                                        Array.Resize(ref GUI.ListItemFlag, Information.UBound(list) + 1);
                                        Expression.FormatMessage(ref buf);
                                        list[Information.UBound(list)] = buf;
                                        GUI.ListItemID[Information.UBound(list)] = Strings.Mid(var.Name, Strings.Len(vname) + 1, Strings.Len(var.Name) - Strings.Len(vname) - 1);
                                        GUI.ListItemFlag[Information.UBound(list)] = false;
                                    }
                                }
                            }
                        }

                        ExecAskCmdRet = LineNum + 1;
                        break;
                    }

                default:
                    {
                        Event_Renamed.EventErrorMessage = "Askコマンドのオプションが不正です";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 85714


                        Input:
                                        Error(0)

                         */
                        break;
                    }
            }

            // 選択肢がなければそのまま終了
            if (Information.UBound(list) == 0)
            {
                Event_Renamed.SelectedAlternative = 0.ToString();
                return ExecAskCmdRet;
            }

            // ダイアログを拡大するか
            if (!use_normal_list & (Information.UBound(list) > 6 | use_large_list))
            {
                GUI.EnlargeListBoxHeight();
            }
            else
            {
                GUI.ReduceListBoxHeight();
            }

            if (SRC.AutoMoveCursor)
            {
                GUI.TopItem = 1;
                string arglb_caption = "選択";
                string arglb_mode = "表示のみ";
                Commands.SelectedItem = GUI.ListBox(ref arglb_caption, ref list, ref msg, ref arglb_mode);
                string argcursor_mode = "ダイアログ";
                GUI.MoveCursorPos(ref argcursor_mode);
            }

            // 選択肢の入力
            do
            {
                GUI.TopItem = 1;
                if (use_continuous_mode)
                {
                    string arglb_caption1 = "選択";
                    string arglb_mode1 = "連続表示";
                    Commands.SelectedItem = GUI.ListBox(ref arglb_caption1, ref list, ref msg, ref arglb_mode1);
                }
                else
                {
                    string arglb_caption2 = "選択";
                    string arglb_mode2 = "";
                    Commands.SelectedItem = GUI.ListBox(ref arglb_caption2, ref list, ref msg, lb_mode: ref arglb_mode2);
                }

                if (enable_rbutton_cancel)
                {
                    if (Commands.SelectedItem == 0)
                    {
                        break;
                    }
                }
            }
            while (Commands.SelectedItem == 0);
            Event_Renamed.SelectedAlternative = GUI.ListItemID[Commands.SelectedItem];
            GUI.ListItemID = new string[1];
            if (!use_continuous_mode)
            {
                if (SRC.AutoMoveCursor)
                {
                    GUI.RestoreCursorPos();
                }
            }

            // ダイアログを標準の大きさに戻す
            if (!use_normal_list & !use_continuous_mode & (Information.UBound(list) > 6 | use_large_list))
            {
                GUI.ReduceListBoxHeight();
            }

            return EventData.ID + 1;
        }
    }
}
