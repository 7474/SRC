using SRCCore;
using SRCCore.VB;
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
            return new SolidBrush(option.FillColor);
        }

        private Rectangle GetCircleRect(int centerX, int centerY, int rad, float oval_ratio = 1f)
        {
            var rect = new Rectangle(centerX - rad, centerY - rad, rad * 2, rad * 2);

            if (oval_ratio != 1f)
            {
                // TODO 縦横比の解決の仕方確認
                rect = new Rectangle(
                    (int)(rect.X + (rad - rad * oval_ratio)),
                    rect.Y,
                    (int)(rect.Width - (rad - rad * oval_ratio) * 2),
                    rect.Height);
            }

            return rect;
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
            OvalCmd(option, x1, y1, rad, 1f);
        }

        public void LineCmd(ScreanDrawOption option, int x1, int y1, int x2, int y2)
        {
            // 描画先
            var buffers = TargetImages(option);

            // 描画領域
            if (option.DrawOption != ScreanDrawMode.Background)
            {
                GUI.IsPictureVisible = true;
            }

            using (var pen = GetPen(option))
            {
                foreach (var buffer in buffers)
                {
                    using (var g = Graphics.FromImage(buffer))
                    {
                        g.DrawLine(pen, x1, y1, x2, y2);
                    }
                }
            }
        }

        public void BoxCmd(ScreanDrawOption option, int x1, int y1, int x2, int y2)
        {
            // 描画先
            var buffers = TargetImages(option);

            // 描画領域
            if (option.DrawOption != ScreanDrawMode.Background)
            {
                GUI.IsPictureVisible = true;
            }

            var w = x2 - x1;
            var h = y2 - y1;
            using (var pen = GetPen(option))
            using (var brush = GetBrush(option))
            {
                foreach (var buffer in buffers)
                {
                    using (var g = Graphics.FromImage(buffer))
                    {
                        if (option.FillStyle != FillStyle.VbFSTransparent)
                        {
                            g.FillRectangle(brush, x1, y1, w, h);
                        }
                        g.DrawRectangle(pen, x1, y1, w, h);
                    }
                }
            }
        }

        public void PSetCmd(ScreanDrawOption option, int x1, int y1)
        {
            // 描画先
            var buffers = TargetImages(option);

            // 描画領域
            if (option.DrawOption != ScreanDrawMode.Background)
            {
                GUI.IsPictureVisible = true;
            }

            using (var pen = GetPen(option))
            {
                foreach (var buffer in buffers)
                {
                    using (var g = Graphics.FromImage(buffer))
                    {
                        g.DrawRectangle(pen, x1, y1, 1, 1);
                    }
                }
            }
        }

        public void OvalCmd(ScreanDrawOption option, int x1, int y1, int rad, float oval_ratio)
        {
            // 描画先
            var buffers = TargetImages(option);

            // 描画領域
            if (option.DrawOption != ScreanDrawMode.Background)
            {
                GUI.IsPictureVisible = true;
            }

            var rect = GetCircleRect(x1, y1, rad, oval_ratio);
            using (var pen = GetPen(option))
            using (var brush = GetBrush(option))
            {
                foreach (var buffer in buffers)
                {
                    using (var g = Graphics.FromImage(buffer))
                    {
                        if (option.FillStyle != FillStyle.VbFSTransparent)
                        {
                            g.FillEllipse(brush, rect);
                        }
                        g.DrawEllipse(pen, rect);
                    }
                }
            }
        }
    }
}
