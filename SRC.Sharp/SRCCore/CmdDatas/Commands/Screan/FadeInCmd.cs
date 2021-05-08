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

            return EventData.NextID;
        }
    }
}
