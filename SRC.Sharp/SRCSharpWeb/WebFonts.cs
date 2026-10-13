using SkiaSharp;
using SRCRender.Compat;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SRCSharpWeb
{
    /// <summary>
    /// ブラウザには日本語フォントが無いため、静的サイトに置いた OFL のフォント（BIZ UD ゴシック・明朝）を読み込む。
    /// ＭＳ ゴシック・ＭＳ 明朝（等幅）は等幅の BIZ UDゴシック・BIZ UD明朝、プロポーショナルの指定は BIZ UDP ゴシック・明朝で描く。
    /// </summary>
    public static class WebFonts
    {
        private static readonly (string File, string[] Names)[] Fonts =
        {
            ("BIZUDGothic-Regular.ttf", new[] { "ＭＳ ゴシック", "MS Gothic" }),
            ("BIZUDPGothic-Regular.ttf", new[] { "ＭＳ Ｐゴシック", "MS PGothic", "ＭＳ UI Gothic", "MS UI Gothic", "メイリオ", "Meiryo", "Meiryo UI" }),
            ("BIZUDMincho-Regular.ttf", new[] { "ＭＳ 明朝", "MS Mincho" }),
            ("BIZUDPMincho-Regular.ttf", new[] { "ＭＳ Ｐ明朝", "MS PMincho" }),
        };

        public static async Task LoadAsync(HttpClient http)
        {
            var loaded = await Task.WhenAll(Fonts.Select(async font =>
            {
                var bytes = await http.GetByteArrayAsync("fonts/" + font.File);
                return (Typeface: SKTypeface.FromData(SKData.CreateCopy(bytes)), font.Names);
            }));
            foreach (var (typeface, names) in loaded)
            {
                SkiaFonts.Register(typeface);
                foreach (var name in names)
                {
                    SkiaFonts.Register(typeface, name);
                }
            }
            SkiaFonts.FixedPitchFamily = loaded[0].Typeface.FamilyName;
            SkiaFonts.SansFamily = loaded[1].Typeface.FamilyName;
            SkiaFonts.SerifFamily = loaded[3].Typeface.FamilyName;
        }
    }
}
