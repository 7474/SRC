using SRCCore.Events;
using System;

namespace SRCCore.CmdDatas.Commands
{
    public class PolygonCmd : CmdData
    {
        public PolygonCmd(SRC src, EventDataLine eventData) : base(src, CmdType.PolygonCmd, eventData)
        {
        }

        protected override int ExecInternal()
        {
            throw new NotImplementedException();
            //            PictureBox pic, pic2 = default;
            //            var points = default(GUI.POINTAPI[]);
            //            short pnum;
            //            short xx, yy;
            //            short x1, y1;
            //            short x2, y2;
            //            int prev_clr;
            //            x1 = GUI.MainPWidth;
            //            y1 = GUI.MainPHeight;
            //            x2 = 0;
            //            y2 = 0;
            //            pnum = 1;
            //            while (2 * pnum < ArgNum)
            //            {
            //                Array.Resize(points, pnum);
            //                xx = (GetArgAsLong((2 * pnum)) + Event.BaseX);
            //                yy = (GetArgAsLong((2 * pnum + 1)) + Event.BaseY);
            //                points[pnum - 1].X = xx;
            //                points[pnum - 1].Y = yy;
            //                if (xx < x1)
            //                {
            //                    x1 = xx;
            //                }

            //                if (xx > x2)
            //                {
            //                    x2 = xx;
            //                }

            //                if (yy < y1)
            //                {
            //                    y1 = yy;
            //                }

            //                if (yy > y2)
            //                {
            //                    y2 = yy;
            //                }

            //                pnum = (pnum + 1);
            //            }

            //            if (pnum == 1)
            //            {
            //                Event.EventErrorMessage = "頂点数が少なすぎます";
            //                ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 383198


            //                Input:
            //                            Error(0)

            //                 */
            //            }

            //            GUI.SaveScreen();

            //            // 描画先
            //            switch (Event.ObjDrawOption ?? "")
            //            {
            //                case "背景":
            //                    {
            //                        pic = GUI.MainForm.picBack;
            //                        pic2 = GUI.MainForm.picMaskedBack;
            //                        Map.IsMapDirty = true;
            //                        break;
            //                    }

            //                case "保持":
            //                    {
            //                        pic = GUI.MainForm.picMain(0);
            //                        pic2 = GUI.MainForm.picMain(1);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        pic = GUI.MainForm.picMain(0);
            //                        break;
            //                    }
            //            }

            //            // 描画領域
            //            short tmp;
            //            if (Event.ObjDrawOption != "背景")
            //            {
            //                GUI.IsPictureVisible = true;
            //                tmp = (Event.ObjDrawWidth - 1);
            //                GUI.PaintedAreaX1 = GeneralLib.MinLng(GUI.PaintedAreaX1, GeneralLib.MaxLng(x1 - tmp, 0));
            //                GUI.PaintedAreaY1 = GeneralLib.MinLng(GUI.PaintedAreaY1, GeneralLib.MaxLng(y1 - tmp, 0));
            //                GUI.PaintedAreaX2 = GeneralLib.MaxLng(GUI.PaintedAreaX2, GeneralLib.MinLng(x2 + tmp, GUI.MapPWidth - 1));
            //                GUI.PaintedAreaY2 = GeneralLib.MaxLng(GUI.PaintedAreaY2, GeneralLib.MinLng(y2 + tmp, GUI.MapPHeight - 1));
            //            }

            //            prev_clr = ColorTranslator.ToOle(pic.ForeColor);
            //            pic.ForeColor = ColorTranslator.FromOle(Event.ObjColor);
            //            pic.DrawWidth = Event.ObjDrawWidth;
            //            pic.FillColor = Event.ObjFillColor;
            //            pic.FillStyle = Event.ObjFillStyle;

            //            GUI.Polygon(pic.hDC, points[0], pnum - 1);
            //            pic.ForeColor = ColorTranslator.FromOle(prev_clr);
            //            pic.DrawWidth = 1;
            //            pic.FillColor = ColorTranslator.ToOle(Color.White);
            //            pic.FillStyle = vbFSTransparent;
            //            if (pic2 is object)
            //            {
            //                prev_clr = ColorTranslator.ToOle(pic2.ForeColor);
            //                pic2.ForeColor = ColorTranslator.FromOle(Event.ObjColor);
            //                pic2.DrawWidth = Event.ObjDrawWidth;
            //                pic2.FillColor = Event.ObjFillColor;
            //                pic2.FillStyle = Event.ObjFillStyle;

            //                GUI.Polygon(pic2.hDC, points[0], pnum - 1);
            //                pic2.ForeColor = ColorTranslator.FromOle(prev_clr);
            //                pic2.DrawWidth = 1;
            //                pic2.FillColor = ColorTranslator.ToOle(Color.White);
            //                pic2.FillStyle = vbFSTransparent;
            //            }

            //return EventData.NextID;
        }
    }
}
