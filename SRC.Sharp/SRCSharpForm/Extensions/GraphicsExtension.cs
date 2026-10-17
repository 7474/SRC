using System.Drawing;

namespace SRCSharpForm.Extensions
{
    public static class GraphicsExtension
    {
        /// <summary>
        /// 余白を付けずに測って描くための書式（元の SRC の Print と同じく、文字の幅だけ進める）。
        /// GDI+ の既定の書式は文字列の前後に余白を付けて測り、描くときも前に余白を空けるため、
        /// 区切って続けて描くと区切りごとに間が空く（7474/SRC#155）。末尾の空白も幅に含める。
        /// https://learn.microsoft.com/dotnet/api/system.drawing.stringformat.generictypographic
        /// </summary>
        private static readonly StringFormat TypographicFormat = CreateTypographicFormat();

        private static StringFormat CreateTypographicFormat()
        {
            var format = (StringFormat)StringFormat.GenericTypographic.Clone();
            format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            return format;
        }

        /// <summary>余白を付けずに文字列の大きさを測る。<see cref="DrawStringWithoutMargin(Graphics, string, Font, Brush, float, float)"/> で描いたときの幅になる。</summary>
        public static SizeF MeasureStringWithoutRightMargin(this Graphics g, string str, Font font)
        {
            if (string.IsNullOrEmpty(str))
            {
                return g.MeasureString(str, font);
            }
            return g.MeasureString(str, font, PointF.Empty, TypographicFormat);
        }

        /// <summary>余白を付けずに文字列を描く</summary>
        public static void DrawStringWithoutMargin(this Graphics g, string str, Font font, Brush brush, float x, float y)
        {
            g.DrawString(str, font, brush, x, y, TypographicFormat);
        }

        /// <summary>余白を付けずに文字列を描く</summary>
        public static void DrawStringWithoutMargin(this Graphics g, string str, Font font, Brush brush, PointF point)
        {
            g.DrawString(str, font, brush, point, TypographicFormat);
        }

        public static void DrawBar(this Graphics g, RectangleF rect, float ratio, Brush back, Brush fore)
        {
            g.FillRectangle(back, rect);
            g.FillRectangle(fore, rect.Left, rect.Top, rect.Width * ratio, rect.Height);
        }

        public static void DrawBarWithShadow(this Graphics g, RectangleF rect, float ratio, Brush back, Brush fore, Brush light, Brush dark)
        {
            g.FillRectangle(light, rect);
            g.FillRectangle(dark, new RectangleF(rect.Left, rect.Top, rect.Width - 1, rect.Height - 1));

            g.DrawBar(new RectangleF(rect.Left + 1, rect.Top + 1, rect.Width - 2, rect.Height - 2), ratio, back, fore);
        }
    }
}
