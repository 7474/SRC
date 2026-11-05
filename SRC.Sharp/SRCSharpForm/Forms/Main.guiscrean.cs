using SRCCore;
using SRCCore.VB;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SRCSharpForm
{
// TODO インタフェースの切り方見直す (Issue #367)
// インタフェースの切り方: IGUIMap/IGUIScrean/IGUIStatus は将来的に分割・整理の余地がある
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
            // 元の SRC（VB の FillStyle）と同じく、横線・縦線・斜線などは 8 ピクセル間隔の模様で塗る。
            // 線の間は塗らない（元の画像が見える）。透明（VbFSTransparent）は呼び出し側で塗らない
            switch (option.FillStyle)
            {
                case FillStyle.VbHorizontalLine:
                    return new HatchBrush(HatchStyle.Horizontal, option.FillColor, Color.Transparent);
                case FillStyle.VbVerticalLine:
                    return new HatchBrush(HatchStyle.Vertical, option.FillColor, Color.Transparent);
                case FillStyle.VbUpwardDiagonal:
                    // VB の vbUpwardDiagonal は Windows の HS_FDIAGONAL（左上から右下への線）
                    return new HatchBrush(HatchStyle.ForwardDiagonal, option.FillColor, Color.Transparent);
                case FillStyle.VbDownwardDiagonal:
                    // VB の vbDownwardDiagonal は Windows の HS_BDIAGONAL（左下から右上への線）
                    return new HatchBrush(HatchStyle.BackwardDiagonal, option.FillColor, Color.Transparent);
                case FillStyle.VbCross:
                    return new HatchBrush(HatchStyle.Cross, option.FillColor, Color.Transparent);
                case FillStyle.VbDiagonalCross:
                    return new HatchBrush(HatchStyle.DiagonalCross, option.FillColor, Color.Transparent);
                default:
                    return new SolidBrush(option.FillColor);
            }
        }

        private Rectangle GetCircleRect(int centerX, int centerY, int rad, float oval_ratio = 1f)
        {
            // VB の Circle メソッドの aspect（縦÷横）と同じにする。1 未満なら半径は横、1 以上なら縦の半径で、
            // 1 未満は横長、1 より大きいと縦長の楕円になる
            float rx = rad, ry = rad;
            if (oval_ratio > 0f && oval_ratio < 1f)
            {
                ry = rad * oval_ratio;
            }
            else if (oval_ratio > 1f)
            {
                rx = rad / oval_ratio;
            }
            var rect = new Rectangle((int)(centerX - rx), (int)(centerY - ry), (int)(rx * 2), (int)(ry * 2));
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
                        // GraphicsのインタフェースとSRCのインタフェースで回転方向が逆
                        if (option.FillStyle != FillStyle.VbFSTransparent)
                        {
                            g.FillPie(brush, GetCircleRect(x1, y1, rad), -start_angle, -(end_angle - start_angle));

                        }
                        g.DrawArc(pen, GetCircleRect(x1, y1, rad), -start_angle, -(end_angle - start_angle));
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

        public void PolygonCmd(ScreanDrawOption option, Point[] points)
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
                            g.FillPolygon(brush, points);
                        }
                        g.DrawPolygon(pen, points);
                    }
                }
            }
        }
    }
}
