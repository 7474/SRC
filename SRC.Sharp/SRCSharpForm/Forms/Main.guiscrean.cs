using SRCCore;
using SRCCore.Maps;
using SRCCore.VB;
using SRCSharpForm.Extensions;
using SRCSharpForm.Resoruces;
using System.Collections.Generic;
using System.Drawing;

namespace SRCSharpForm
{
    // TODO インタフェースの切り方見直す
    internal partial class frmMain : IGUIScrean
    {
        private IEnumerable<Image> TargetImages(ScreanDrawOption option)
        {
            switch (option.DrawOption)
            {
                case ScreanDrawMode.Background:
                    return new Image[] { BackgroundBuffer };
                case ScreanDrawMode.Preserve:
                    return new Image[] { MainBuffer, MainBufferBack };
                default:
                    return new Image[] { MainBuffer };
            }
        }

        private Pen GetPen(ScreanDrawOption option)
        {
            return new Pen(option.ForeColor, option.DrawWidth);
        }

        private Brush GetBrush(ScreanDrawOption option)
        {
            //TODO FillStyle
            return new SolidBrush(option.ForeColor);
        }

        private Rectangle GetCircleRect(int centerX, int centerY, int rad)
        {
            return new Rectangle(centerX - rad, centerY - rad, rad * 2, rad * 2);
        }

        public void ArcCmd(ScreanDrawOption option, int x1, int y1, int rad, float start_angle, float end_angle)
        {
            // 描画先
            var buffers = TargetImages(option);

            // 描画領域
            if (option.DrawOption != ScreanDrawMode.Background)
            {
                GUI.IsPictureVisible = true;
            }

            using (var pen = GetPen(option))
            using (var brush = GetBrush(option))
            {
                foreach (var buffer in buffers)
                {
                    using (var g = Graphics.FromImage(buffer))
                    {
                        if (option.FillStyle != FillStyle.VbFSTransparent)
                        {
                            g.FillPie(brush, GetCircleRect(x1, y1, rad), start_angle, end_angle - start_angle);

                        }
                        g.DrawArc(pen, GetCircleRect(x1, y1, rad), start_angle, end_angle - start_angle);
                    }
                }
            }
        }

        public void CircleCmd(ScreanDrawOption option, int x1, int y1, int rad)
        {
            // 描画先
            var buffers = TargetImages(option);

            // 描画領域
            if (option.DrawOption != ScreanDrawMode.Background)
            {
                GUI.IsPictureVisible = true;
            }

            using (var pen = GetPen(option))
            using (var brush = GetBrush(option))
            {
                foreach (var buffer in buffers)
                {
                    using (var g = Graphics.FromImage(buffer))
                    {
                        if (option.FillStyle != FillStyle.VbFSTransparent)
                        {
                            g.FillEllipse(brush, GetCircleRect(x1, y1, rad));
                        }
                        g.DrawEllipse(pen, GetCircleRect(x1, y1, rad));
                    }
                }
            }
        }
    }
}
