using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Expressions;
using SRCCore.VB;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.CmdDatas.Commands
{
    public class AskCmd : CmdData
    {
        public AskCmd(SRC src, EventDataLine eventData) : base(src, CmdType.AskCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            var use_normal_list = false;
            var use_large_list = false;
            var use_continuous_mode = false;
            var enable_rbutton_cancel = false;
            string msg;

            // 表示オプションの処理
            var argIndex = ArgNum;
            var breakLoop = false;
            while (argIndex > 1 && !breakLoop)
            {
                switch (GetArg(argIndex) ?? "")
                {
                    case "通常":
                        use_normal_list = true;
                        break;

                    case "拡大":
                        use_large_list = true;
                        break;

                    case "連続表示":
                        use_continuous_mode = true;
                        break;

                    case "キャンセル可":
                        enable_rbutton_cancel = true;
                        break;

                    case "終了":
                        GUI.CloseListBox();
                        if (SRC.AutoMoveCursor)
                        {
                            GUI.RestoreCursorPos();
                        }
                        GUI.ReduceListBoxHeight();
                        return EventData.NextID;

                    default:
                        breakLoop = true;
                        break;
                }

                if (!breakLoop)
                {
                    argIndex = argIndex - 1;
                }
            }

            var answerList = new List<ListBoxItem>();
            string cancelResult;
            int ExecAskCmdRet;
            // オプションではない引数の数で書式タイプを判別
            switch (argIndex)
            {
                // イベントデータ中に選択肢を列挙している場合
                case 1:
                case 2:
                    {
                        cancelResult = "0";
                        if (ArgNum == 1)
                        {
                            msg = "いずれかを選んでください";
                        }
                        else
                        {
                            msg = GetArgAsString(2);
                        }

                        // 選択肢の読みこみ
                        CmdData hitEndCmd = null;
                        foreach (var i in AfterEventIdRange())
                        {
                            var buf = Event.EventData[i].Data;
                            Expression.FormatMessage(ref buf);
                            if (Strings.Len(buf) > 0)
                            {
                                if (Event.EventCmd[i].Name == CmdType.EndCmd)
                                {
                                    hitEndCmd = Event.EventCmd[i];
                                    break;
                                }

                                answerList.Add(new ListBoxItem
                                {
                                    Text = buf,
                                    ListItemID = "" + (i - EventData.ID),
                                    ListItemFlag = false,
                                });
                            }
                        }

                        if (hitEndCmd == null)
                        {
                            throw new EventErrorException(this, "AskとEndが対応していません");
                        }

                        ExecAskCmdRet = hitEndCmd.EventData.NextID;
                        break;
                    }

                // 選択肢を配列で指定する場合
                case 3:
                    // TODO Impl
                    cancelResult = "";
                    var vname = GetArg(2);
                    msg = GetArgAsString(3);

                    // 配列の検索
                    if (Expression.IsSubLocalVariableDefined(vname))
                    {
                        if (Strings.Left(vname, 1) == "$")
                        {
                            vname = Strings.Mid(vname, 2) + "[";
                        }
                        else
                        {
                            vname = vname + "[";
                        }

                        var loopTo1 = Event.VarIndex;
                        for (argIndex = Event.VarIndexStack[Event.CallDepth - 1] + 1; argIndex <= loopTo1; argIndex++)
                        {
                            {
                                var v = Event.VarStack[argIndex];
                                if (Strings.InStr(v.Name, vname) == 1)
                                {
                                    string buf;
                                    if (v.VariableType == Expressions.ValueType.StringType)
                                    {
                                        buf = v.StringValue;
                                    }
                                    else
                                    {
                                        buf = SrcFormatter.Format(v.NumericValue);
                                    }

                                    if (Strings.Len(buf) > 0)
                                    {
                                        Expression.FormatMessage(ref buf);
                                        answerList.Add(new ListBoxItem
                                        {
                                            Text = buf,
                                            ListItemID = "" + Strings.Mid(v.Name, Strings.Len(vname) + 1, Strings.Len(v.Name) - Strings.Len(vname) - 1),
                                            ListItemFlag = false,
                                        });
                                    }
                                }
                            }
                        }
                    }
                    else if (Expression.IsLocalVariableDefined(vname))
                    {
                        if (Strings.Left(vname, 1) == "$")
                        {
                            vname = Strings.Mid(vname, 2) + "[";
                        }
                        else
                        {
                            vname = vname + "[";
                        }

                        foreach (VarData v in Event.LocalVariableList.Values)
                        {
                            if (Strings.InStr(v.Name, vname) == 1)
                            {
                                string buf;
                                if (v.VariableType == Expressions.ValueType.StringType)
                                {
                                    buf = v.StringValue;
                                }
                                else
                                {
                                    buf = SrcFormatter.Format(v.NumericValue);
                                }

                                if (Strings.Len(buf) > 0)
                                {
                                    Expression.FormatMessage(ref buf);
                                    answerList.Add(new ListBoxItem
                                    {
                                        Text = buf,
                                        ListItemID = "" + Strings.Mid(v.Name, Strings.Len(vname) + 1, Strings.Len(v.Name) - Strings.Len(vname) - 1),
                                        ListItemFlag = false,
                                    });
                                }
                            }
                        }
                    }
                    else if (Expression.IsGlobalVariableDefined(vname))
                    {
                        if (Strings.Left(vname, 1) == "$")
                        {
                            vname = Strings.Mid(vname, 2) + "[";
                        }
                        else
                        {
                            vname = vname + "[";
                        }

                        foreach (VarData v in Event.GlobalVariableList.Values)
                        {
                            string buf;
                            if (Strings.InStr(v.Name, vname) == 1)
                            {
                                if (v.VariableType == Expressions.ValueType.StringType)
                                {
                                    buf = v.StringValue;
                                }
                                else
                                {
                                    buf = SrcFormatter.Format(v.NumericValue);
                                }

                                if (Strings.Len(buf) > 0)
                                {
                                    Expression.FormatMessage(ref buf);
                                    answerList.Add(new ListBoxItem
                                    {
                                        Text = buf,
                                        ListItemID = "" + Strings.Mid(v.Name, Strings.Len(vname) + 1, Strings.Len(v.Name) - Strings.Len(vname) - 1),
                                        ListItemFlag = false,
                                    });
                                }
                            }
                        }
                    }
                    ExecAskCmdRet = EventData.NextID;
                    break;

                default:
                    throw new EventErrorException(this, "Askコマンドのオプションが不正です");
            }

            // 選択肢がなければそのまま終了
            if (!answerList.Any())
            {
                Event.SelectedAlternative = cancelResult;
                return ExecAskCmdRet;
            }

            // ダイアログを拡大するか
            if (!use_normal_list && (answerList.Count > 6 || use_large_list))
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
                Commands.SelectedItem = GUI.ListBox(new ListBoxArgs
                {
                    lb_caption = "選択",
                    Items = answerList,
                    lb_info = msg,
                    lb_mode = "表示のみ",
                });
                GUI.MoveCursorPos("ダイアログ");
            }

            // 選択肢の入力
            do
            {
                GUI.TopItem = 1;
                if (use_continuous_mode)
                {
                    Commands.SelectedItem = GUI.ListBox(new ListBoxArgs
                    {
                        lb_caption = "選択",
                        Items = answerList,
                        lb_info = msg,
                        lb_mode = "連続表示",
                    });
                }
                else
                {
                    Commands.SelectedItem = GUI.ListBox(new ListBoxArgs
                    {
                        lb_caption = "選択",
                        Items = answerList,
                        lb_info = msg,
                        lb_mode = "",
                    });
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

            Event.SelectedAlternative = Commands.SelectedItem <= 0
                ? cancelResult
                : answerList[Commands.SelectedItem - 1].ListItemID;
            if (!use_continuous_mode)
            {
                if (SRC.AutoMoveCursor)
                {
                    GUI.RestoreCursorPos();
                }
            }

            // ダイアログを標準の大きさに戻す
            if (!use_normal_list && !use_continuous_mode && (answerList.Count > 6 || use_large_list))
            {
                GUI.ReduceListBoxHeight();
            }

            return ExecAskCmdRet;
        }
    }
}
