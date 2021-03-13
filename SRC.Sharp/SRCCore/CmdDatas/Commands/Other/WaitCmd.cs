using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class WaitCmd : CmdData
    {
        public WaitCmd(SRC src, EventDataLine eventData) : base(src, CmdType.WaitCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            int wait_time, start_time, cur_time;
            switch (ArgNum)
            {
                case 2:
                    switch (Strings.LCase(GetArg(2)) ?? "")
                    {
                        case "start":
                            Event.WaitStartTime = GeneralLib.timeGetTime();
                            Event.WaitTimeCount = 0;
                            break;

                        case "reset":
                            Event.WaitStartTime = -1;
                            Event.WaitTimeCount = 0;
                            break;

                        case "click":
                            //// 先行入力されていたクリックイベントを解消
                            GUI.Sleep(0, true);
                            Commands.WaitClickMode = true;
                            GUI.IsFormClicked = false;
                            Event.SelectedAlternative = "";

                            // ウィンドウが表示されていない場合は表示
                            if (!GUI.MainFormVisible)
                            {
                                GUI.MainFormShow();
                                GUI.UpdateScreen();
                            }

                            // クリックされるまで待つ
                            while (!GUI.IsFormClicked)
                            {
                                if (GUI.IsRButtonPressed(true))
                                {
                                    GUI.MouseButton = GuiButton.Right;
                                    break;
                                }

                                GUI.Sleep(25);
                            }

                            // TODO Impl
                            //// マウスの左ボタンが押された場合はホットポイントの判定を行う
                            //if (string.IsNullOrEmpty(Event.SelectedAlternative) & (int)GUI.MouseButton == 1)
                            //{
                            //    var loopTo = Information.UBound(Event.HotPointList);
                            //    for (i = 1; i <= loopTo; i++)
                            //    {
                            //        {
                            //            var withBlock1 = Event.HotPointList[(int)i];
                            //            if ((float)withBlock1.Left_Renamed <= GUI.MouseX & GUI.MouseX < (float)(withBlock1.Left_Renamed + withBlock1.width) & (float)withBlock1.Top <= GUI.MouseY & GUI.MouseY < (float)(withBlock1.Top + withBlock1.Height))
                            //            {
                            //                Event.SelectedAlternative = withBlock1.Name;
                            //                break;
                            //            }
                            //        }
                            //    }
                            //}

                            Commands.WaitClickMode = false;
                            GUI.IsFormClicked = false;
                            break;

                        default:
                            wait_time = (int)(100d * GetArgAsDouble(2));

                            // 待ち時間が切れるまで待機
                            if (wait_time < 1000)
                            {
                                if (!GUI.IsRButtonPressed(true))
                                {
                                    GUI.Sleep(wait_time);
                                }
                            }
                            else
                            {
                                start_time = GeneralLib.timeGetTime();
                                while (start_time + wait_time > GeneralLib.timeGetTime())
                                {
                                    // 右ボタンを押されていたら早送り
                                    if (GUI.IsRButtonPressed(true))
                                    {
                                        break;
                                    }

                                    GUI.Sleep(25);
                                }
                            }

                            break;
                    }

                    break;

                case 3:
                    // Wait Until ～
                    wait_time = (int)(100d * GetArgAsDouble(3));
                    Event.WaitTimeCount = Event.WaitTimeCount + 1;
                    if (Event.WaitStartTime == -1)
                    {
                        // Wait Reset が実行されていた場合
                        Event.WaitStartTime = GeneralLib.timeGetTime();
                    }
                    else if (wait_time < 100)
                    {
                        // アニメの１回目の表示は例外的に時間がかかってしまうことがある
                        // ので、超過時間を無視する
                        if (Event.WaitTimeCount == 1)
                        {
                            cur_time = GeneralLib.timeGetTime();
                            if (Event.WaitStartTime + wait_time > cur_time)
                            {
                                Event.WaitStartTime = cur_time;
                            }
                        }
                    }

                    while (Event.WaitStartTime + wait_time > GeneralLib.timeGetTime())
                    {
                        if (GUI.IsRButtonPressed(true))
                        {
                            break;
                        }
                        GUI.Sleep(25);
                    }

                    break;

                default:
                    throw new EventErrorException(this, "Waitコマンドの引数の数が違います");
            }

            return EventData.ID + 1;
        }
    }
}
