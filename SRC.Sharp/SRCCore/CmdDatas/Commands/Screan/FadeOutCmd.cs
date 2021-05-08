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

            GUI.TransionScrean(TransionPattern.FadeOut, Color.Black, num, 50);
            //GUI.SaveScreen();
            // XXX 処理後の画面状態

            return EventData.NextID;
        }
    }
}
