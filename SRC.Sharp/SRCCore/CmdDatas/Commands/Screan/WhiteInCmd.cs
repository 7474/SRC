using SRCCore.Events;
using SRCCore.Exceptions;
using System;
using System.Drawing;

namespace SRCCore.CmdDatas.Commands
{
    public class WhiteInCmd : CmdData
    {
        public WhiteInCmd(SRC src, EventDataLine eventData) : base(src, CmdType.WhiteInCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            // 元の SRC の WhiteIn は FadeIn と違い、最初に右ボタンを確かめて省略しない
            // （引数を確かめて画面を退避し、右ボタンが押されていればフェードの途中で元の画面に戻す。TransionScrean が行う）
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
                    throw new EventErrorException(this, "WhiteInコマンドの引数の数が違います");
            }

            GUI.SaveScreen();
            GUI.TransionScrean(TransionPattern.FadeIn, Color.White, num, 50);

            return EventData.NextID;
        }
    }
}
