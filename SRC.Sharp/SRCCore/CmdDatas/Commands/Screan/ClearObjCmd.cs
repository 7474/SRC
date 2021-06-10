using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Extensions;
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
            var without_refresh = false;
            var n = ArgNum;
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
                        var oname = GetArgAsString(2);
                        Event.HotPointList.RemoveItem(x => x.Name == oname);
                        break;
                    }

                case 1:
                    {
                        Event.HotPointList.Clear();
                        break;
                    }

                default:
                    throw new EventErrorException(this, "ClearObjコマンドの引数の数が違います");
            }

            // TODO Update hotpoint
            //// まだマウスカーソルがホットポイント上にあるか？
            //var loopTo2 = Information.UBound(Event.HotPointList);
            //for (i = 1; i <= loopTo2; i++)
            //{
            //    {
            //        var withBlock1 = Event.HotPointList[i];
            //        if (withBlock1.Left <= GUI.MouseX && GUI.MouseX < withBlock1.Left + withBlock1.width && withBlock1.Top <= GUI.MouseY && GUI.MouseY < withBlock1.Top + withBlock1.Height)
            //        {
            //            return ExecClearObjCmdRet;
            //        }
            //    }
            //}

            //// ツールチップを消す
            //My.MyProject.Forms.frmToolTip.Hide();
            //if (!without_refresh)
            //{
            //    GUI.MainForm.picMain(0).Refresh();
            //}

            //// マウスカーソルを元に戻す
            //GUI.MainForm.picMain(0).MousePointer = 0;
            return EventData.NextID;
        }
    }
}
