using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Extensions;
using SRCCore.VB;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class OvalCmd : CmdData
    {
        public OvalCmd(SRC src, EventDataLine eventData) : base(src, CmdType.OvalCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum < 5)
            {
                throw new EventErrorException(this, "Ovalコマンドの引数の数が違います");
            }

            var x1 = (GetArgAsLong(2) + Event.BaseX);
            var y1 = (GetArgAsLong(3) + Event.BaseY);
            var rad = GetArgAsLong(4);
            var oval_ratio = GetArgAsDouble(5);

            var clr = Event.ObjColor;
            for (var i = 6; i <= ArgNum; i++)
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
                    throw new EventErrorException(this, "Ovalコマンドに不正なオプション「" + opt + "」が使われています");
                }
            }

            GUI.SaveScreen();

            // 背景への描き込み（中断・クイックセーブから再開するときに再開イベントで書き直す）
            if (Event.ObjDrawOption == "背景")
            {
                Map.IsMapDirty = true;
            }
            SRC.GUIScrean.OvalCmd(new ScreanDrawOption(Event, clr), x1, y1, rad, (float)oval_ratio);

            return EventData.NextID;
        }
    }
}
