using System.Drawing;

namespace SRCSharpForm.Extensions
{
    public static class GraphicsExtension
    {
        public static SizeF MeasureStringWithoutRightMargin(this Graphics g, string str, Font font)
        {
            var size = g.MeasureString(str, font);
            if (str.Length == str.TrimEnd().Length)
            {
                // 終端が空白文字列ではない場合はMeasureが付与する余白をカットする
                size.Width -= font.Size / 2f;
            }

            return size;
        }
    }
}
