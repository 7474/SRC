using SRCCore.Events;
using SRCCore.Exceptions;
using System;
using System.Drawing;

namespace SRCCore.CmdDatas.Commands
{
    public class FadeInCmd : CmdData
    {
        public FadeInCmd(SRC src, EventDataLine eventData) : base(src, CmdType.FadeInCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (GUI.IsRButtonPressed())
            {
                GUI.RefreshScreen();
                return EventData.NextID;
            }

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
                    throw new EventErrorException(this, "FadeInコマンドの引数の数が違います");
            }

            GUI.SaveScreen();
            GUI.TransionScrean(TransionPattern.FadeIn, Color.Black, num, 50);
            //{
            //    var withBlock = GUI.MainForm;
            //    {
            //        var withBlock1 = withBlock.picTmp;
            //        withBlock1.Picture = Image.FromFile("");
            //        withBlock1.width = GUI.MainPWidth;
            //        withBlock1.Height = GUI.MainPHeight;
            //    }

            //    // MOD START マージ
            //    // ret = BitBlt(.picTmp.hDC, _
            //    // '            0, 0, MapPWidth, MapPHeight, _
            //    // '            .picMain(0).hDC, 0, 0, SRCCOPY)
            //    ret = GUI.BitBlt(withBlock.picTmp.hDC, 0, 0, GUI.MainPWidth, GUI.MainPHeight, withBlock.picMain(0).hDC, 0, 0, GUI.SRCCOPY);
            //    // MOD END マージ

            //    Graphics.InitFade(withBlock.picMain(0), num);
            //    start_time = GeneralLib.timeGetTime();
            //    wait_time = 50;
            //    var loopTo = num;
            //    for (i = 0; i <= loopTo; i++)
            //    {
            //        if (i % 4 == 0)
            //        {
            //            if (GUI.IsRButtonPressed())
            //            {
            //                break;
            //            }
            //        }

            //        Graphics.DoFade(withBlock.picMain(0), i);
            //        withBlock.picMain(0).Refresh();
            //        cur_time = GeneralLib.timeGetTime();
            //        while (cur_time < start_time + wait_time * (i + 1))
            //        {
            //            Application.DoEvents();
            //            cur_time = GeneralLib.timeGetTime();
            //        }
            //    }

            //    Graphics.FinishFade();

            //    ret = GUI.BitBlt(withBlock.picMain(0).hDC, 0, 0, GUI.MapPWidth, GUI.MapPHeight, withBlock.picTmp.hDC, 0, 0, GUI.SRCCOPY);
            //    withBlock.picMain(0).Refresh();

            //    {
            //        var withBlock2 = withBlock.picTmp;
            //        withBlock2.Picture = Image.FromFile("");
            //        withBlock2.width = 32;
            //        withBlock2.Height = 32;
            //    }
            //}

            return EventData.NextID;
        }
    }
}
