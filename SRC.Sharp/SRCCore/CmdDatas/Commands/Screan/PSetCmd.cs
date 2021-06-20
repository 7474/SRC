using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Extensions;

namespace SRCCore.CmdDatas.Commands
{
    public class PSetCmd : CmdData
    {
        public PSetCmd(SRC src, EventDataLine eventData) : base(src, CmdType.PSetCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum < 3)
            {
                throw new EventErrorException(this, "PSetコマンドの引数の数が違います");
            }

            // 座標
            var xx = (GetArgAsLong(2) + Event.BaseX);
            var yy = (GetArgAsLong(3) + Event.BaseY);

            // 描画色
            var clr = Event.ObjColor;
            if (ArgNum == 4)
            {
                var opt = GetArgAsString(4);
                if (!ColorExtension.TryFromHexString(opt, out clr))
                {
                    throw new EventErrorException(this, "色指定が不正です");
                }
            }
            // 座標は画面上にある？
            if (xx < 0 || GUI.MapPWidth <= xx || yy < 0 || GUI.MapPHeight <= yy)
            {
                return EventData.NextID;
            }

            GUI.SaveScreen();
            // 点を描画
            SRC.GUIScrean.PSetCmd(new ScreanDrawOption(Event, clr), xx, yy);

            return EventData.NextID;
        }
    }
}
