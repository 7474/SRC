using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Extensions;
using System.Drawing;

namespace SRCCore.CmdDatas.Commands
{
    public class ColorCmd : CmdData
    {
        public ColorCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ColorCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 2)
            {
                throw new EventErrorException(this, "Colorコマンドの引数の数が違います");
            }

            var opt = GetArgAsString(2);
            Color color;
            if (!ColorExtension.TryFromHexString(opt, out color))
            {
                throw new EventErrorException(this, "色指定が不正です");
            }
            Event.ObjColor = color;

            return EventData.NextID;
        }
    }
}
