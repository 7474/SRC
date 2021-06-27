using System.Drawing;

namespace SRCSharpForm.Extensions
{
    public static class GraphicsExtension
    {
        private static Font RefFont = SystemFonts.DefaultFont;
        public static SizeF MeasureStringWithoutRightMargin(this Graphics g, string str, Font font)
        {
            var size = g.MeasureString(str, font);
            if (!string.IsNullOrEmpty(str))
            {
                if (str.Length == str.TrimEnd().Length)
                {
                    // 終端が空白文字列ではない場合はMeasureが付与する余白をカットする
                    // https://github.com/dotnet/runtime/blob/33033b22eccf50550451387ac8927ad1b5f17768/src/libraries/System.Drawing.Common/src/System/Drawing/Graphics.cs#L1565
                    // 最終的には GdipMeasureString に移譲されていて余白の付与量が具体的にどうなのかサッとは分からなかった。
                    // 雰囲気デフォルトっぽい文字サイズの 0.5em 位な感じはするのでそのくらいにしておく。
                    // XXX 終端空白以外でも文字によって余白調整されていそう
                    size.Width -= RefFont.Size / 2f;
                }
                // XXX 終端空白無視されてそう。どうすっかな。
            }
            return size;
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
