using SRCCore.Events;
using SRCCore.Exceptions;

namespace SRCCore.CmdDatas.Commands
{
    public class PaintSysStringCmd : CmdData
    {
        public PaintSysStringCmd(SRC src, EventDataLine eventData) : base(src, CmdType.PaintSysStringCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum != 4 && ArgNum != 5)
            {
                throw new EventErrorException(this, "PaintSysStringコマンドの引数の数が違います");
            }

            bool without_refresh = false;
            if (ArgNum == 5)
            {
                if (GetArgAsString(5) == "非同期")
                {
                    without_refresh = true;
                }
            }

            GUI.DrawSysString(GetArgAsLong(2), GetArgAsLong(3), GetArgAsString(4), without_refresh);
            return EventData.NextID;
        }
    }
}
