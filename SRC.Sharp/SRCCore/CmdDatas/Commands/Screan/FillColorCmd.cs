using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Extensions;
using System.Drawing;

namespace SRCCore.CmdDatas.Commands
{
    public class FillColorCmd : CmdData
    {
        public FillColorCmd(SRC src, EventDataLine eventData) : base(src, CmdType.FillColorCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 2)
            {
                throw new EventErrorException(this, "FillColorコマンドの引数の数が違います");
            }

            var opt = GetArgAsString(2);
            Color color;
            if (!ColorExtension.TryFromHexString(opt, out color))
            {
                throw new EventErrorException(this, "色指定が不正です");
            }

            Event.ObjFillColor = color;
            return EventData.NextID;
        }
    }
}
