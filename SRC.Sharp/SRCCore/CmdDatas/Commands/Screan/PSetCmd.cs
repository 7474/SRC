using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Extensions;
using SRCCore.VB;

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

            // 座標は画面上にある？
            // 元の SRC と同じく、画面の外なら色指定を確かめずに終わる
            if (xx < 0 || GUI.MapPWidth <= xx || yy < 0 || GUI.MapPHeight <= yy)
            {
                return EventData.NextID;
            }

            // 描画色
            // 元の SRC と同じく「#」で始まる 7 文字の 16 進数だけを受け付ける（「#RGB」や色の名前はエラー）
            var clr = Event.ObjColor;
            if (ArgNum == 4)
            {
                var opt = GetArgAsString(4);
                if (Strings.Asc(opt) != 35 || Strings.Len(opt) != 7)
                {
                    throw new EventErrorException(this, "色指定が不正です");
                }
                if (!ColorExtension.TryFromHexString(opt, out clr))
                {
                    throw new EventErrorException(this, "色指定が不正です");
                }
            }

            GUI.SaveScreen();

            // 背景への描き込み（中断・クイックセーブから再開するときに再開イベントで書き直す）
            if (Event.ObjDrawOption == "背景")
            {
                Map.IsMapDirty = true;
            }
            // 点を描画
            SRC.GUIScrean.PSetCmd(new ScreanDrawOption(Event, clr), xx, yy);

            return EventData.NextID;
        }
    }
}
