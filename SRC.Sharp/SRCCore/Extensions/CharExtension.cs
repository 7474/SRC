namespace SRCCore.Extensions
{
    public static class CharExtension
    {
        public static bool IsAscii(this char c)
        {
            return c < 0x7F;
        }
    }
}
