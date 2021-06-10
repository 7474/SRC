using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ClearObjCmd : CmdData
    {
        public ClearObjCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ClearObjCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            n = ArgNum;
            if (n > 1)
            {
                if (GetArgAsString(n) == "非同期")
                {
                    n = (n - 1);
                    without_refresh = true;
                }
            }

            switch (n)
            {
                case 2:
                    {
                        oname = GetArgAsString(2);
                        var loopTo = Information.UBound(Event.HotPointList);
                        for (i = 1; i <= loopTo; i++)
                        {
                            if ((Event.HotPointList[i].Name ?? "") == (oname ?? ""))
                            {
                                break;
                            }
                        }

                        if (i <= Information.UBound(Event.HotPointList))
                        {
                            {
                                var withBlock = Event.HotPointList[i];
                                if (My.MyProject.Forms.frmToolTip.Visible && (Event.SelectedAlternative ?? "") == (withBlock.Name ?? ""))
                                {
                                    // ツールチップを消す
                                    My.MyProject.Forms.frmToolTip.Hide();
                                    // マウスカーソルを元に戻す
                                    GUI.MainForm.picMain(0).MousePointer = 0;
                                }
                            }

                            var loopTo1 = (Information.UBound(Event.HotPointList) - 1);
                            for (j = i; j <= loopTo1; j++)
                                // UPGRADE_WARNING: オブジェクト HotPointList(j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                Event.HotPointList[j] = Event.HotPointList[j + 1];
                            Array.Resize(Event.HotPointList, Information.UBound(Event.HotPointList));
                        }

                        break;
                    }

                case 1:
                    {
                        Event.HotPointList = new Event.HotPoint[1];
                        break;
                    }

                default:
                    {
                        Event.EventErrorMessage = "ClearObjコマンドの引数の数が違います";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 183065


                        Input:
                                        Error(0)

                         */
                        break;
                    }
            }

            ExecClearObjCmdRet = LineNum + 1;

            // まだマウスカーソルがホットポイント上にあるか？
            var loopTo2 = Information.UBound(Event.HotPointList);
            for (i = 1; i <= loopTo2; i++)
            {
                {
                    var withBlock1 = Event.HotPointList[i];
                    if (withBlock1.Left <= GUI.MouseX && GUI.MouseX < withBlock1.Left + withBlock1.width && withBlock1.Top <= GUI.MouseY && GUI.MouseY < withBlock1.Top + withBlock1.Height)
                    {
                        return ExecClearObjCmdRet;
                    }
                }
            }

            // ツールチップを消す
            My.MyProject.Forms.frmToolTip.Hide();
            if (!without_refresh)
            {
                GUI.MainForm.picMain(0).Refresh();
            }

            // マウスカーソルを元に戻す
            GUI.MainForm.picMain(0).MousePointer = 0;
            return EventData.NextID;
        }
    }
}
