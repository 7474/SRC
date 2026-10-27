using SRCCore.Events;
using SRCCore.Exceptions;
using System;
using System.Drawing;

namespace SRCCore.CmdDatas.Commands
{
    public class FadeOutCmd : CmdData
    {
        public FadeOutCmd(SRC src, EventDataLine eventData) : base(src, CmdType.FadeOutCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            int num;
            switch (ArgNum)
            {
                case 1:
                    {
                        num = 10;
                        break;
                    }

                case 2:
                    {
                        num = GetArgAsLong(2);
                        break;
                    }

                default:
                    throw new EventErrorException(this, "FadeOutコマンドの引数の数が違います");
            }

            // 画面を退避してから塗りつぶし、塗りつぶした画面を描画済みの画面とする（VB 版と同じ）。
            // 続く PaintPicture などは塗りつぶした画面に描き、ClearPicture で退避した画面に戻る
            GUI.SaveScreen();
            GUI.TransionScrean(TransionPattern.FadeOut, Color.Black, num, 50);

            GUI.IsPictureVisible = true;
            GUI.PaintedAreaX1 = GUI.MainPWidth;
            GUI.PaintedAreaY1 = GUI.MainPHeight;
            GUI.PaintedAreaX2 = -1;
            GUI.PaintedAreaY2 = -1;

            return EventData.NextID;
        }
    }
}
