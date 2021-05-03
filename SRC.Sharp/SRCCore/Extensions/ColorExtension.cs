using System.Drawing;

namespace SRCCore.Extensions
{
    public static class ColorExtension
    {
        public static string ToHexString(this Color color)
        {
            return ColorTranslator.ToHtml(color);
        }
    }
}
