using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Extensions;
using SRCCore.VB;

namespace SRCCore.CmdDatas.Commands
{
    public class LineCmd : CmdData
    {
        public LineCmd(SRC src, EventDataLine eventData) : base(src, CmdType.LineCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum < 5)
            {
                throw new EventErrorException(this, "Lineコマンドの引数の数が違います");
            }

            var x1 = (GetArgAsLong(2) + Event.BaseX);
            var y1 = (GetArgAsLong(3) + Event.BaseY);
            var x2 = (GetArgAsLong(4) + Event.BaseX);
            var y2 = (GetArgAsLong(5) + Event.BaseY);
            GUI.SaveScreen();

            var dtype = "";
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
                    if (opt != "B" && opt != "BF")
                    {
                        throw new EventErrorException(this, "Lineコマンドに不正なオプション「" + opt + "」が使われています");
                    }

                    dtype = opt;
                }
            }
            var drawOpt = new ScreanDrawOption(Event, clr);
            switch (dtype ?? "")
            {
                case "B":
                    // Box
                    SRC.GUIScrean.BoxCmd(drawOpt, x1, y1, x2, y2);
                    break;

                case "BF":
                    // Box Fill
                    drawOpt.FillStyle = FillStyle.VbFSSolid;
                    drawOpt.FillColor = drawOpt.ForeColor;
                    SRC.GUIScrean.BoxCmd(drawOpt, x1, y1, x2, y2);
                    break;

                default:
                    // Line
                    SRC.GUIScrean.LineCmd(drawOpt, x1, y1, x2, y2);
                    break;
            }

            return EventData.NextID;
        }
    }
}
