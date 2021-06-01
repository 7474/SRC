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
            var start_angle = Math.PI * GetArgAsDouble(5) / 180d;
            var end_angle = Math.PI * GetArgAsDouble(6) / 180d;

            // TODO 振る舞い確認していない
            // 塗りつぶしの際は角度を負の値にする必要がある
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
            var drawOption = new ScreanDrawOption(Event, clr);
            SRC.GUIScrean.ArcCmd(drawOption, x1, y1, rad, start_angle, end_angle);

            //// 描画先
            //switch (Event.ObjDrawOption ?? "")
            //{
            //    case "背景":
            //        {
            //            pic = GUI.MainForm.picBack;
            //            pic2 = GUI.MainForm.picMaskedBack;
            //            Map.IsMapDirty = true;
            //            break;
            //        }

            //    case "保持":
            //        {
            //            pic = GUI.MainForm.picMain(0);
            //            pic2 = GUI.MainForm.picMain(1);
            //            break;
            //        }

            //    default:
            //        {
            //            pic = GUI.MainForm.picMain(0);
            //            break;
            //        }
            //}

            //// 描画領域
            //short tmp;
            //if (Event.ObjDrawOption != "背景")
            //{
            //    GUI.IsPictureVisible = true;
            //    tmp = (rad + Event.ObjDrawWidth - 1);
            //    GUI.PaintedAreaX1 = GeneralLib.MinLng(GUI.PaintedAreaX1, GeneralLib.MaxLng(x1 - tmp, 0));
            //    GUI.PaintedAreaY1 = GeneralLib.MinLng(GUI.PaintedAreaY1, GeneralLib.MaxLng(y1 - tmp, 0));
            //    GUI.PaintedAreaX2 = GeneralLib.MaxLng(GUI.PaintedAreaX2, GeneralLib.MinLng(x1 + tmp, GUI.MainPWidth - 1));
            //    GUI.PaintedAreaY2 = GeneralLib.MaxLng(GUI.PaintedAreaY2, GeneralLib.MinLng(y1 + tmp, GUI.MainPHeight - 1));
            //}

            //pic.Circle(x1, y1);
            //pic.DrawWidth = 1;
            //pic.FillColor = ColorTranslator.ToOle(Color.White);
            //pic.FillStyle = vbFSTransparent;
            //if (pic2 is object)
            //{
            //    pic2.DrawWidth = Event.ObjDrawWidth;
            //    pic2.FillColor = Event.ObjFillColor;
            //    pic2.FillStyle = Event.ObjFillStyle;

            //    pic2.Circle(x1, y1);
            //    pic2.DrawWidth = 1;
            //    pic2.FillColor = ColorTranslator.ToOle(Color.White);
            //    pic2.FillStyle = vbFSTransparent;
            //}

            return EventData.NextID;
        }
    }
}
