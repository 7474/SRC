using SkiaSharp;
using System.Reflection;

namespace SRCRender
{
    /// <summary>
    /// ユニットの陣営枠（味方・敵・中立）の画像。SRC 本体のリソースと同じもの。
    /// </summary>
    public static class UnitFrames
    {
        public static readonly SKBitmap Ally = Load("unit_frame_ally.png");
        public static readonly SKBitmap Enemy = Load("unit_frame_enemy.png");
        public static readonly SKBitmap Neutral = Load("unit_frame_neutral.png");

        private static SKBitmap Load(string name)
        {
            var assembly = typeof(UnitFrames).GetTypeInfo().Assembly;
            using (var stream = assembly.GetManifestResourceStream("SRCRender.Assets." + name))
            {
                return stream == null ? null : SKBitmap.Decode(stream);
            }
        }

        public static SKBitmap ForParty(string party)
        {
            switch (party ?? "")
            {
                case "味方":
                case "ＮＰＣ":
                    return Ally;
                case "敵":
                    return Enemy;
                case "中立":
                    return Neutral;
                default:
                    return null;
            }
        }
    }
}
