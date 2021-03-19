﻿
using SRCCore.Units;
using SRCCore.VB;

namespace SRCCore.Events
{
    public partial class Event
    {
        // イベントの実行
        public void HandleEvent(params string[] Args)
        {
            SRC.LogDebug("", Args);

            // 画面入力をロック
            var prev_is_gui_locked = GUI.IsGUILocked;
            if (!GUI.IsGUILocked)
            {
                GUI.LockGUI();
            }

            //// 現在選択されているユニット＆ターゲットをイベント用に設定
            //// (SearchLabel()実行時の式計算用にあらかじめ設定しておく)
            //SelectedUnitForEvent = Commands.SelectedUnit;
            //// 引数に指定されたユニットを優先
            //if (Information.UBound(Args) > 0)
            //{
            //    if (SRC.PList.IsDefined(Args[1]))
            //    {
            //        {
            //            var withBlock = SRC.PList.Item(Args[1]);
            //            if (withBlock.Unit is object)
            //            {
            //                SelectedUnitForEvent = withBlock.Unit;
            //            }
            //        }
            //    }
            //}

            //SelectedTargetForEvent = Commands.SelectedTarget;

            // イベントキューを作成
            //event_que_idx = Information.UBound(EventQue);
            switch (Args[0])
            {
                case "プロローグ":
                    {
                        EventQue.Enqueue("プロローグ");
                        SRC.Stage = "プロローグ";
                        break;
                    }

                case "エピローグ":
                    {
                        EventQue.Enqueue("エピローグ");
                        SRC.Stage = "エピローグ";
                        break;
                    }

                case "破壊":
                    {
                        EventQue.Enqueue("破壊 " + Args[1]);
                        var p = SRC.PList.Item(Args[1]);
                        var uparty = p.Party;
                        if (p.Unit != null)
                        {
                            var u = p.Unit;
                            // TODO Impl
                            //// 格納されていたユニットも破壊しておく
                            //while (u.CountUnitOnBoard() > 0)
                            //{
                            //    object argIndex1 = 1;
                            //    u = u.UnitOnBoard(argIndex1);
                            //    u.UnloadUnit((object)u.ID);
                            //    u.Status_Renamed = "破壊";
                            //    u.HP = 0;
                            //    Array.Resize(EventQue, Information.UBound(EventQue) + 1 + 1);
                            //    EventQue[Information.UBound(EventQue)] = "マップ攻撃破壊 " + u.MainPilot().ID;
                            //}
                            uparty = u.Party0;
                        }

                        // 全滅の判定
                        var flag = false;
                        foreach (Unit currentU in SRC.UList.Items)
                        {
                            if ((currentU.Party0 ?? "") == (uparty ?? "")
                                && currentU.Status == "出撃"
                                && !currentU.IsConditionSatisfied("憑依"))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            EventQue.Enqueue("全滅 " + uparty);
                        }
                        break;
                    }

                //case "マップ攻撃破壊":
                //    {
                //        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                //        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject("マップ攻撃破壊 ", Args[1]));
                //        {
                //            var withBlock4 = SRC.PList.Item(Args[1]);
                //            uparty = withBlock4.Party;
                //            if (withBlock4.Unit is object)
                //            {
                //                {
                //                    var withBlock5 = withBlock4.Unit;
                //                    // 格納されていたユニットも破壊しておく
                //                    var loopTo = withBlock5.CountUnitOnBoard();
                //                    for (i = 1; i <= loopTo; i++)
                //                    {
                //                        object argIndex3 = i;
                //                        u = withBlock5.UnitOnBoard(argIndex3);
                //                        withBlock5.UnloadUnit((object)u.ID);
                //                        u.Status_Renamed = "破壊";
                //                        u.HP = 0;
                //                        Array.Resize(EventQue, Information.UBound(EventQue) + 1 + 1);
                //                        EventQue[Information.UBound(EventQue)] = "マップ攻撃破壊 " + u.MainPilot().ID;
                //                    }

                //                    uparty = withBlock5.Party0;
                //                }
                //            }
                //        }

                //        break;
                //    }

                case "ターン":
                    EventQue.Enqueue(string.Join(" ", "ターン ", "全", Args[2]));
                    EventQue.Enqueue(string.Join(" ", "ターン ", Args[1], Args[2]));
                    break;

                case "損傷率":
                    EventQue.Enqueue(string.Join(" ", "損傷率", Args[1], Args[2]));
                    break;

                case "攻撃":
                    EventQue.Enqueue(string.Join(" ", "攻撃", Args[1], Args[2]));
                    break;

                case "攻撃後":
                    EventQue.Enqueue(string.Join(" ", "攻撃後", Args[1], Args[2]));
                    break;

                case "会話":
                    EventQue.Enqueue(string.Join(" ", "会話", Args[1], Args[2]));
                    break;

                case "接触":
                    EventQue.Enqueue(string.Join(" ", "接触", Args[1], Args[2]));
                    break;

                //case "進入":
                //    {
                //        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                //        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("進入 ", Args[1]), " "), SrcFormatter.Format(Args[2])), " "), SrcFormatter.Format(Args[3])));
                //        Array.Resize(EventQue, Information.UBound(EventQue) + 1 + 1);
                //        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                //        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("進入 ", Args[1]), " "), Map.TerrainName(Conversions.Toint(Args[2]), Conversions.Toint(Args[3]))));
                //        // UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                //        if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(Args[2], 1, false)))
                //        {
                //            Array.Resize(EventQue, Information.UBound(EventQue) + 1 + 1);
                //            // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                //            EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("脱出 ", Args[1]), " W"));
                //        }
                //        // UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                //        else if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(Args[2], Map.MapWidth, false)))
                //        {
                //            Array.Resize(EventQue, Information.UBound(EventQue) + 1 + 1);
                //            // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                //            EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("脱出 ", Args[1]), " E"));
                //        }
                //        // UPGRADE_WARNING: オブジェクト Args(3) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                //        if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(Args[3], 1, false)))
                //        {
                //            Array.Resize(EventQue, Information.UBound(EventQue) + 1 + 1);
                //            // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                //            EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("脱出 ", Args[1]), " N"));
                //        }
                //        // UPGRADE_WARNING: オブジェクト Args(3) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                //        else if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(Args[3], Map.MapHeight, false)))
                //        {
                //            Array.Resize(EventQue, Information.UBound(EventQue) + 1 + 1);
                //            // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                //            EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("脱出 ", Args[1]), " S"));
                //        }

                //        break;
                //    }

                case "収納":
                    EventQue.Enqueue(string.Join(" ", "収納", Args[1]));
                    break;

                case "使用":
                    EventQue.Enqueue(string.Join(" ", "使用", Args[1], Args[2]));
                    break;

                case "使用後":
                    EventQue.Enqueue(string.Join(" ", "使用後", Args[1], Args[2]));
                    break;

                case "行動終了":
                    EventQue.Enqueue(string.Join(" ", "行動終了", Args[1]));
                    break;

                //case "ユニットコマンド":
                //    {
                //        // UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                //        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                //        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("ユニットコマンド ", Args[1]), " "), Args[2]));
                //        if (!IsEventDefined(EventQue[Information.UBound(EventQue)]))
                //        {
                //            // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                //            EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("ユニットコマンド ", Args[1]), " "), SRC.PList.Item(Args[2]).Unit.Name));
                //        }

                //        break;
                //    }

                default:
                    EventQue.Enqueue(string.Join(" ", Args));
                    break;
            }

            //if (CallDepth > MaxCallDepth)
            //{
            //    string argmsg = "サブルーチンの呼び出し階層が" + SrcFormatter.Format(MaxCallDepth) + "を超えているため、イベントの処理が出来ません";
            //    GUI.ErrorMessage(argmsg);
            //    CallDepth = MaxCallDepth;
            //    return;
            //}

            //// 現在の状態を保存
            //ArgIndexStack[CallDepth] = ArgIndex;
            //VarIndexStack[CallDepth] = VarIndex;
            //ForIndexStack[CallDepth] = ForIndex;
            //SaveBasePoint();

            // 呼び出し階層数をインクリメント
            var prev_call_depth = CallDepth;
            CallDepth = (CallDepth + 1);

            // 各イベントを発生させる
            // XXX キューのスキップすんの？
            //i = event_que_idx;
            SRC.IsCanceled = false;
            while (EventQue.Count > 0)
            {
                var eventItem = EventQue.Dequeue();

                // Debug.Print "HandleEvent (" & EventQue(i) & ")"

                //// 前のイベントで他のユニットが出現している可能性があるので
                //// 本当に全滅したのか判定
                //if (GeneralLib.LIndex(EventQue[i], 1) == "全滅")
                //{
                //    uparty = GeneralLib.LIndex(EventQue[i], 2);
                //    foreach (Unit currentU1 in SRC.UList)
                //    {
                //        u = currentU1;
                //        object argIndex4 = "憑依";
                //        if ((u.Party0 ?? "") == (uparty ?? "") & u.Status_Renamed == "出撃" & !u.IsConditionSatisfied(argIndex4))
                //        {
                //            continue;
                //        }
                //    }
                //}

                CurrentLabel = -1;
                var ret = -1;
                var main_event_done = false;
                while (true)
                {
                    //// 現在選択されているユニット＆ターゲットをイベント用に設定
                    //// SearchLabel()で入れ替えられる可能性があるので、毎回設定し直す必要あり
                    //SelectedUnitForEvent = Commands.SelectedUnit;
                    //// 引数に指定されたユニットを優先
                    //if (Information.UBound(Args) > 0)
                    //{
                    //    if (SRC.PList.IsDefined(Args[1]))
                    //    {
                    //        {
                    //            var withBlock6 = SRC.PList.Item(Args[1]);
                    //            if (withBlock6.Unit is object)
                    //            {
                    //                SelectedUnitForEvent = withBlock6.Unit;
                    //            }
                    //        }
                    //    }
                    //}

                    //SelectedTargetForEvent = Commands.SelectedTarget;

                    // 実行するイベントラベルを探す
                    do
                    {
                        if (Information.IsNumeric(eventItem))
                        {
                            if (CurrentLabel < 0)
                            {
                                ret = Conversions.ToInteger(eventItem);
                            }
                            else
                            {
                                ret = -1;
                            }
                        }
                        else
                        {
                            ret = SearchLabel(eventItem, CurrentLabel + 1);
                        }

                        // ラベルが見つからなければ終わり
                        if (ret < 0)
                        {
                            break;
                        }

                        CurrentLabel = ret;
                        if (!EventData[ret].IsAlwaysEventLabel)
                        {
                            // 常時イベントではないイベントは１度しか実行しない
                            if (main_event_done)
                            {
                                ret = -1;
                            }
                            else
                            {
                                main_event_done = true;
                            }
                        }
                    }
                    while (ret < 0);

                    // 戦闘後のイベント実行前にはいくつかの後始末が必要
                    //if (Strings.Left(EventData[ret], 1) != "*")
                    //{
                    //    // UPGRADE_WARNING: オブジェクト Args(0) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    //    if (Conversions.ToBoolean(Operators.OrObject(Operators.OrObject(Operators.OrObject(Operators.ConditionalCompareObjectEqual(Args[0], "破壊", false), Operators.ConditionalCompareObjectEqual(Args[0], "損傷率", false)), Operators.ConditionalCompareObjectEqual(Args[0], "攻撃後", false)), Operators.ConditionalCompareObjectEqual(Args[0], "全滅", false))))
                    //    {
                    //        // 画面をクリア
                    //        if (GUI.MainForm.Visible == true)
                    //        {
                    //            Status.ClearUnitStatus();
                    //            GUI.RedrawScreen();
                    //        }

                    //        // メッセージウィンドウを閉じる
                    //        if (My.MyProject.Forms.frmMessage.Visible == true)
                    //        {
                    //            GUI.CloseMessageForm();
                    //        }
                    //    }
                    //}

                    if (ret < 0) { break; }

                    // ラベルの行は実行しても無駄なので
                    ret = ret + 1;
                    //Application.DoEvents();

                    // イベントの各コマンドを実行
                    do
                    {
                        CurrentLineNum = ret;
                        if (CurrentLineNum >= EventCmd.Count)
                        {
                            break;
                        }

                        ret = EventCmd[CurrentLineNum].Exec();
                    }
                    while (ret > 0);

                    // ステージが終了 or キャンセル？
                    if (SRC.IsScenarioFinished | SRC.IsCanceled)
                    {
                        break;
                    }
                }
                // ステージが終了 or キャンセル？
                if (SRC.IsScenarioFinished | SRC.IsCanceled)
                {
                    break;
                }
            };

            //if (CallDepth >= 0)
            //{
            //    // 呼び出し階層数を元に戻す
            //    // （サブルーチン内でExitが呼ばれることがあるので単純に-1出来ない）
            //    CallDepth = prev_call_depth;

            //    // イベント実行前の状態に復帰
            //    ArgIndex = ArgIndexStack[CallDepth];
            //    VarIndex = VarIndexStack[CallDepth];
            //    ForIndex = ForIndexStack[CallDepth];
            //}
            //else
            //{
            //    ArgIndex = 0;
            //    VarIndex = 0;
            //    ForIndex = 0;
            //}

            //// イベントキューを元に戻す
            //Array.Resize(EventQue, GeneralLib.MinLng(event_que_idx - 1, Information.UBound(EventQue)) + 1);

            //// フォント設定をデフォルトに戻す
            //{
            //    var withBlock7 = GUI.MainForm.picMain(0);
            //    withBlock7.ForeColor = Information.RGB(255, 255, 255);
            //    {
            //        var withBlock8 = withBlock7.Font;
            //        withBlock8.Size = 16;
            //        withBlock8.Name = "ＭＳ Ｐ明朝";
            //        withBlock8.Bold = true;
            //        withBlock8.Italic = false;
            //    }

            //    GUI.PermanentStringMode = false;
            //    GUI.KeepStringMode = false;
            //}

            //// オブジェクト色をデフォルトに戻す
            //ObjColor = ColorTranslator.ToOle(Color.White);
            //ObjFillColor = ColorTranslator.ToOle(Color.White);
            //ObjFillStyle = vbFSTransparent;
            //ObjDrawWidth = 1;
            //ObjDrawOption = "";

            //// 描画の基準座標位置を元に戻す
            //RestoreBasePoint();

            // 画面入力のロックを解除
            if (!prev_is_gui_locked)
            {
                GUI.UnlockGUI();
            }
        }

        // イベントを登録しておき、後で実行
        public void RegisterEvent(params string[] Args)
        {
            var item = string.Join(" ", Args);
            EventQue.Enqueue(item);
        }
    }
}