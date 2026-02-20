using SRCCore.Events;
using SRCCore.Exceptions;
using SRCCore.Extensions;
using SRCCore.VB;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class ArcCmd : CmdData
    {
        public ArcCmd(SRC src, EventDataLine eventData) : base(src, CmdType.ArcCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            if (ArgNum < 6)
            {
                throw new EventErrorException(this, "Arcコマンドの引数の数が違います");
            }

            var x1 = (GetArgAsLong(2) + Event.BaseX);
            var y1 = (GetArgAsLong(3) + Event.BaseY);
            var rad = GetArgAsLong(4);
            // 円弧の開始・終了角度はstart,end で指定します。角度の指定は右向きが0度で、そこから時計と逆周りに増加していきます(例 上向きが90)。0から360までの値で指定して下さい。
            var start_angle = GetArgAsDouble(5) % 360;
            var end_angle = GetArgAsDouble(6) % 360;

            // 塗りつぶしの際は角度を負の値にする必要がある（VB6の Circle/Arc 関数との互換仕様）。
            // VB6 Circle コマンドでは、start > end のとき中心への線分が描画されるため、
            // 塗りつぶし(扇形)を描くには負の角度値を渡す必要があった。
            // DrawArc (GDI+) では sweep_angle の符号で描画方向が決まるため同等の変換を適用している。
            if (Event.ObjFillStyle != FillStyle.VbFSTransparent)
            {
                start_angle = -start_angle;
                if (start_angle == 0d)
                {
                    start_angle = -0.000001d;
                }

                end_angle = -end_angle;
                if (end_angle == 0d)
                {
                    end_angle = -0.000001d;
                }
            }

            GUI.SaveScreen();

            var clr = Event.ObjColor;
            for (var i = 7; i <= ArgNum; i++)
            {
                var opt = GetArgAsString(i);
                if (Strings.Asc(opt) == 35) // #
                {
                    if (!ColorExtension.TryFromHexString(opt, out clr))
                    {
                        throw new EventErrorException(this, "ColorFilterコマンドのカラー指定が不正です");
                    }
                }
                else
                {
                    throw new EventErrorException(this, "Arcコマンドに不正なオプション「" + opt + "」が使われています");
                }
            }

            SRC.GUIScrean.ArcCmd(new ScreanDrawOption(Event, clr), x1, y1, rad, (float)start_angle, (float)end_angle);

            return EventData.NextID;
        }
    }
}
