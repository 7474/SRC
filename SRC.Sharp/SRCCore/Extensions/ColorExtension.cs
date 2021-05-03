using System.Drawing;

namespace SRCCore.Extensions
{
    public static class ColorExtension
    {
        public static string ToHexString(this Color color)
        {
            return string.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
        }
    }
}
