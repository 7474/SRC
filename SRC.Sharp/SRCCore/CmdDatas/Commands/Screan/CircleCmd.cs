using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Extensions;
using SRCCore.VB;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class CircleCmd : CmdData
    {
        public CircleCmd(SRC src, EventDataLine eventData) : base(src, CmdType.CircleCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum < 4)
            {
                throw new EventErrorException(this, "Circleコマンドの引数の数が違います");
            }

            var x1 = (GetArgAsLong(2) + Event.BaseX);
            var y1 = (GetArgAsLong(3) + Event.BaseY);
            var rad = GetArgAsLong(4);
            GUI.SaveScreen();

            var clr = Event.ObjColor;
            for (var i = 5; i <= ArgNum; i++)
            {
                var opt = GetArgAsString(i);
                if (Strings.Asc(opt) == 35) // #
                {
                    if (!ColorExtension.TryFromHexString(opt, out clr))
                    {
                        throw new EventErrorException(this, "色指定が不正です");
                    }
                }
                else
                {
                    throw new EventErrorException(this, "Circleコマンドに不正なオプション「" + opt + "」が使われています");
                }
            }
            SRC.GUIScrean.CircleCmd(new ScreanDrawOption(Event, clr), x1, y1, rad);

            return EventData.NextID;
        }
    }
}
