using System.Drawing;

namespace SRCCore.Extensions
{
    public static class ColorExtension
    {
        public static string ToHexString(this Color color)
        {
            return string.Format("#{0:x2}{1:x2}{2:x2}", color.R, color.G, color.B);
        }

        public static Color FromHexString(string str)
        {
            return ColorTranslator.FromHtml(str);
        }

        public static bool TryFromHexString(string str, out Color color)
        {
            try
            {
                color = FromHexString(str);
                return true;
            }
            catch
            {
                // ignore
                color = Color.Empty;
                return false;
            }
        }
    }
}
