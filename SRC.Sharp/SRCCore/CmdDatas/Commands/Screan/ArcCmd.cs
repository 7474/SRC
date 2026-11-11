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

            // 塗りつぶしの際は角度を負の値にする必要がある。
            // VB の Circle メソッドは、負の角度を渡すとその角度（の絶対値）から中心へ線を引き、
            // 開始・終了の両方が負なら閉じた扇形になって塗りつぶされる。
            // GUI の ArcCmd も同じ約束で角度を受け取る（IGUIScrean.ArcCmd）。
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

            // 背景への描き込み（中断・クイックセーブから再開するときに再開イベントで書き直す）
            if (Event.ObjDrawOption == "背景")
            {
                Map.IsMapDirty = true;
            }

            var clr = Event.ObjColor;
            for (var i = 7; i <= ArgNum; i++)
            {
                var opt = GetArgAsString(i);
                if (Strings.Asc(opt) == 35) // #
                {
                    // 元の SRC は #rrggbb の 7 文字だけを受け付ける。SRC# は Circle などほかの描画コマンドと同じく「#RGB」も受け付ける（利便性）
                    if (!ColorExtension.TryFromHexString(opt, out clr))
                    {
                        throw new EventErrorException(this, "色指定が不正です");
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
