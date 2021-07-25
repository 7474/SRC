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
            // TODO Waitコマンドによる「待ち」はSRC本体自身の実行を一時停止するだけでなく、PC上で実行されている他のアプリケーションに対してプログラムの実行権を渡すという動作を伴います。これによりアニメ表示など長時間に渡るイベントコマンドの実行の際にも他のアプリケーションの実行を阻害しないようにすることが出来ます。このような実行権の譲渡は「Wait 0」のように待ち時間を0にした場合にも有効です。この場合、Waitコマンドの動作は「他のアプリケーションが処理を行っていない場合はSRC本体が実行を続け、処理を行っているのであればそのアプリケーションに実行権を渡す」というものになります。

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

                            // マウスの左ボタンが押された場合はホットポイントの判定を行う
                            if (string.IsNullOrEmpty(Event.SelectedAlternative) && GUI.MouseButton == GuiButton.Left)
                            {
                                foreach (var hpoint in Event.HotPointList)
                                {
                                    if (hpoint.Left <= GUI.MouseX && GUI.MouseX < (hpoint.Left + hpoint.Width) && hpoint.Top <= GUI.MouseY && GUI.MouseY < (hpoint.Top + hpoint.Height))
                                    {
                                        Event.SelectedAlternative = hpoint.Name;
                                        break;
                                    }
                                }
                            }

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

            return EventData.NextID;
        }
    }
}
