using System.Drawing;

namespace SRCSharpForm.Extensions
{
    public static class GraphicsExtension
    {
        private static Font RefFont = SystemFonts.DefaultFont;
        public static SizeF MeasureStringWithoutRightMargin(this Graphics g, string str, Font font)
        {
            var size = g.MeasureString(str, font);
            if (!string.IsNullOrEmpty(str) && str.Length == str.TrimEnd().Length)
            {
                // 終端が空白文字列ではない場合はMeasureが付与する余白をカットする
                // https://github.com/dotnet/runtime/blob/33033b22eccf50550451387ac8927ad1b5f17768/src/libraries/System.Drawing.Common/src/System/Drawing/Graphics.cs#L1565
                // 最終的には GdipMeasureString に移譲されていて余白の付与量が具体的にどうなのかサッとは分からなかった。
                // 雰囲気デフォルトっぽい文字サイズの 0.5em 位な感じはするのでそのくらいにしておく。
                size.Width -= RefFont.Size / 2f;
            }
            return size;
        }
    }
}
