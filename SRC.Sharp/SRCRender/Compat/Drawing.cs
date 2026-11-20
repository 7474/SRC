using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

// WinForms 版（GDI+）のコードを SkiaSharp で動かすための互換クラス。
// SRCSharpForm から移植したコードが同じ書き方で動くよう、使っている範囲だけを実装する。
namespace SRCRender.Compat
{
    [Flags]
    public enum FontStyle
    {
        Regular = 0,
        Bold = 1,
        Italic = 2,
        Underline = 4,
        Strikeout = 8,
    }

    public enum GraphicsUnit
    {
        World,
        Display,
        Pixel,
        Point,
        Inch,
        Document,
        Millimeter,
    }

    /// <summary>
    /// System.Drawing.Font 相当。描画には SkiaFonts で解決した SKFont を使う。
    /// </summary>
    public class Font
    {
        public Font(string familyName, float emSize, FontStyle style = FontStyle.Regular, GraphicsUnit unit = GraphicsUnit.Point)
        {
            Name = familyName;
            Size = emSize;
            Style = style;
            Unit = unit;
        }

        public Font(Font prototype, FontStyle newStyle)
            : this(prototype.Name, prototype.Size, newStyle, prototype.Unit)
        {
        }

        public string Name { get; }
        public float Size { get; }
        public FontStyle Style { get; }
        public GraphicsUnit Unit { get; }
        public bool Bold => Style.HasFlag(FontStyle.Bold);
        public bool Italic => Style.HasFlag(FontStyle.Italic);
        public Font FontFamily => this;

        public float SizeInPoints => Unit == GraphicsUnit.Pixel ? Size * 72f / 96f : Size;

        /// <summary>ピクセル単位のサイズ</summary>
        public float SizeInPixels => Unit == GraphicsUnit.Pixel ? Size : Size * 96f / 72f;

        /// <summary>行の高さ（ピクセル、切り上げ）</summary>
        public int Height => (int)Math.Ceiling(GetHeight());

        /// <summary>行の高さ（ピクセル）</summary>
        public float GetHeight(Graphics g = null)
        {
            using (var font = ToSKFont())
            {
                return SkiaFonts.LineHeight(font);
            }
        }

        public float GetHeight() => GetHeight(null);

        public SKFont ToSKFont()
        {
            return SkiaFonts.Create(Name, SizeInPixels, Bold, Italic);
        }
    }

    public abstract class Brush : IDisposable
    {
        public abstract Color Color { get; }

        public void Dispose()
        {
        }
    }

    public class SolidBrush : Brush
    {
        private readonly Color color;

        public SolidBrush(Color color)
        {
            this.color = color;
        }

        public override Color Color => color;
    }

    public static class Brushes
    {
        public static readonly Brush White = new SolidBrush(Color.White);
        public static readonly Brush Black = new SolidBrush(Color.Black);
        public static readonly Brush Red = new SolidBrush(Color.Red);
        public static readonly Brush Yellow = new SolidBrush(Color.Yellow);
    }

    public static class RenderingConfig
    {
        public static readonly object TextHint = null;
    }

    /// <summary>
    /// System.Drawing.Graphics 相当（SKBitmap への描画）。
    /// </summary>
    public sealed class Graphics : IDisposable
    {
        private readonly SKBitmap bitmap;
        private readonly SKCanvas canvas;

        private Graphics(SKBitmap bitmap)
        {
            this.bitmap = bitmap;
            canvas = new SKCanvas(bitmap);
        }

        public static Graphics FromImage(SKBitmap image)
        {
            return new Graphics(image);
        }

        public SKCanvas Canvas => canvas;

        public object TextRenderingHint { get; set; }

        public RectangleF VisibleClipBounds => new RectangleF(0, 0, bitmap.Width, bitmap.Height);

        public static SKColor ToSKColor(Color color) => new SKColor(color.R, color.G, color.B, color.A);

        public void Clear(Color color)
        {
            canvas.Clear(ToSKColor(color));
        }

        public void FillRectangle(Brush brush, RectangleF rect)
        {
            FillRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public void FillRectangle(Brush brush, float x, float y, float width, float height)
        {
            using (var paint = new SKPaint { Color = ToSKColor(brush.Color), IsAntialias = false })
            {
                canvas.DrawRect(x, y, width, height, paint);
            }
        }

        /// <summary>y を文字の上端として描く（GDI+ と同じ）。</summary>
        public void DrawString(string s, Font font, Brush brush, PointF point)
        {
            DrawString(s, font, brush, point.X, point.Y);
        }

        /// <summary>改行を含む場合は複数行で描く（GDI+ と同じ）。</summary>
        public void DrawString(string s, Font font, Brush brush, float x, float y)
        {
            using (var skFont = font.ToSKFont())
            using (var paint = new SKPaint { Color = ToSKColor(brush.Color), IsAntialias = true })
            {
                foreach (var line in SplitLines(s))
                {
                    SkiaFonts.DrawText(canvas, line, x, y + SkiaFonts.Ascent(skFont), SKTextAlign.Left, skFont, paint);
                    y += SkiaFonts.LineHeight(skFont);
                }
            }
        }

        public SizeF MeasureString(string s, Font font)
        {
            using (var skFont = font.ToSKFont())
            {
                var lines = SplitLines(s);
                var width = 0f;
                foreach (var line in lines)
                {
                    width = Math.Max(width, SkiaFonts.MeasureText(skFont, line));
                }
                return new SizeF(width, SkiaFonts.LineHeight(skFont) * Math.Max(1, lines.Length));
            }
        }

        private static string[] SplitLines(string s)
        {
            return (s ?? "").Replace("\r\n", "\n").Replace('\r', '\n').TrimEnd('\n').Split('\n');
        }

        public void DrawImage(SKBitmap image, float x, float y)
        {
            canvas.DrawBitmap(image, x, y, SKSamplingOptions.Default);
        }

        public void DrawImage(SKBitmap image, Rectangle destRect)
        {
            canvas.DrawBitmap(image, SKRect.Create(destRect.X, destRect.Y, destRect.Width, destRect.Height), SKSamplingOptions.Default);
        }

        /// <summary>image の srcRect の部分を (x, y) に等倍で描く。</summary>
        public void DrawImage(SKBitmap image, float x, float y, Rectangle srcRect, GraphicsUnit unit)
        {
            canvas.DrawBitmap(image,
                SKRect.Create(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height),
                SKRect.Create(x, y, srcRect.Width, srcRect.Height), SKSamplingOptions.Default);
        }

        public void DrawImage(SKBitmap image, Rectangle destRect, Rectangle srcRect, GraphicsUnit unit)
        {
            canvas.DrawBitmap(image,
                SKRect.Create(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height),
                SKRect.Create(destRect.X, destRect.Y, destRect.Width, destRect.Height), SKSamplingOptions.Default);
        }

        public void Dispose()
        {
            canvas.Dispose();
        }
    }

    public static class GraphicsExtension
    {
        /// <summary>
        /// 文字列の大きさ。WinForms 版は GDI+ の余白を差し引いていたが、Skia は余白を含まないためそのまま返す。
        /// </summary>
        public static SizeF MeasureStringWithoutRightMargin(this Graphics g, string str, Font font)
        {
            return g.MeasureString(str, font);
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

    /// <summary>
    /// 日本語フォントの解決と、Windows 版 SRC（ＭＳ 明朝等）に合わせた文字の寸法。
    /// </summary>
    public static class SkiaFonts
    {
        /// <summary>等幅指定（ＭＳ 明朝・ＭＳ ゴシック）の代替フォント</summary>
        public static string FixedPitchFamily { get; set; } = "Noto Sans Mono CJK JP";
        public static string SansFamily { get; set; } = "Noto Sans CJK JP";
        public static string SerifFamily { get; set; } = "Noto Serif CJK JP";

        // OS のフォント一覧に無い書体（ブラウザ版で読み込んだフォントなど）。名前ごとに太さ・斜体の違う書体を持つ
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, SKTypeface[]> registered
            = new System.Collections.Concurrent.ConcurrentDictionary<string, SKTypeface[]>();

        // 名前に書体を加える。同じ太さ・斜体の書体があれば、replace なら差し替え、そうでなければそのまま
        private static void AddRegistered(string familyName, SKTypeface typeface, bool replace)
        {
            registered.AddOrUpdate(familyName, _ => new[] { typeface }, (_, faces) =>
            {
                var same = faces.Where(x => x.FontWeight == typeface.FontWeight && x.FontSlant == typeface.FontSlant).ToArray();
                if (same.Length > 0 && !replace)
                {
                    return faces;
                }
                return faces.Except(same).Append(typeface).ToArray();
            });
        }

        // 登録した書体から選ぶ。exact なら太さ・斜体が同じものだけ、そうでなければ近いもの
        private static SKTypeface FindRegistered(string familyName, SKFontStyle style, bool exact)
        {
            if (!registered.TryGetValue(familyName, out var faces))
            {
                return null;
            }
            return exact
                ? faces.FirstOrDefault(x => x.FontWeight == style.Weight && x.FontSlant == style.Slant)
                : faces.OrderBy(x => x.FontSlant == style.Slant ? 0 : 1).ThenBy(x => Math.Abs(x.FontWeight - style.Weight)).FirstOrDefault();
        }

        /// <summary>
        /// 書体を登録する。登録した書体は OS のフォントより優先して名前で引く。
        /// 代替フォント（FixedPitchFamily など）を差し替える場合は、登録してから名前を設定する。
        /// </summary>
        public static void Register(SKTypeface typeface, string familyName = null)
        {
            AddRegistered(familyName ?? typeface.FamilyName, typeface, true);
            typefaceCache.Clear();
        }

        /// <summary>
        /// 等幅フォントの指定か。ＭＳ 明朝・ＭＳ ゴシックは等幅、ＭＳ Ｐ明朝・ＭＳ Ｐゴシック・ＭＳ UI ゴシックはプロポーショナル。
        /// </summary>
        public static bool IsFixedPitchName(string name)
        {
            name = name ?? "";
            var normalized = name.Replace("ＭＳ", "MS").Replace("Ｐ", "P").Replace("　", " ");
            if (normalized.StartsWith("MS ", StringComparison.OrdinalIgnoreCase))
            {
                var rest = normalized.Substring(3).TrimStart();
                return !rest.StartsWith("P", StringComparison.OrdinalIgnoreCase)
                    && !rest.StartsWith("UI", StringComparison.OrdinalIgnoreCase);
            }
            return name.Contains("等幅") || normalized.IndexOf("Mono", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// フォント名から書体を選ぶ。指定のフォントがあればそれを使い、無ければ
        /// 等幅指定は Noto Sans Mono CJK JP、明朝系は Noto Serif CJK JP、それ以外は Noto Sans CJK JP にする。
        /// </summary>
        // 書体の検索は OS のフォント一覧を引くため重い。名前とスタイルごとに結果を覚えておく
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<(string, int, int, int), SKTypeface> typefaceCache
            = new System.Collections.Concurrent.ConcurrentDictionary<(string, int, int, int), SKTypeface>();

        /// <summary>追加で読み込むフォントのファイル・フォルダ（パスの区切り文字で区切る）を指定する環境変数</summary>
        public const string FontsEnvironmentVariable = "SRCSHARP_FONTS";
        /// <summary>WSL から見た Windows のフォントのフォルダ</summary>
        public const string WslWindowsFontsDirectory = "/mnt/c/Windows/Fonts";

        // Windows のフォントの英語名と、SRC のデータ・設定で使う日本語名
        private static readonly (string English, string Japanese)[] windowsFontNames =
        {
            ("MS Gothic", "ＭＳ ゴシック"),
            ("MS PGothic", "ＭＳ Ｐゴシック"),
            ("MS UI Gothic", "MS UI Gothic"),
            ("MS Mincho", "ＭＳ 明朝"),
            ("MS PMincho", "ＭＳ Ｐ明朝"),
        };
        private static readonly string[] windowsFontFiles = { "msgothic.ttc", "msmincho.ttc" };
        private static int extraFontsLoaded;

        /// <summary>
        /// 追加のフォントを読み込む（初めて書体を引くときに一度だけ）。
        /// - 環境変数 SRCSHARP_FONTS のファイル・フォルダ
        /// - WSL（Linux から /mnt/c/Windows/Fonts が見える）では、Windows の ＭＳ ゴシック・ＭＳ 明朝。
        ///   WSL の Linux には日本語のフォントが入っていないことが多く、日本語が豆腐になるため
        /// </summary>
        private static void EnsureExtraFontsLoaded()
        {
            if (System.Threading.Interlocked.Exchange(ref extraFontsLoaded, 1) == 1)
            {
                return;
            }
            try
            {
                var paths = new System.Collections.Generic.List<string>();
                var env = Environment.GetEnvironmentVariable(FontsEnvironmentVariable);
                if (!string.IsNullOrEmpty(env))
                {
                    paths.AddRange(env.Split(System.IO.Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries));
                }
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux) && System.IO.Directory.Exists(WslWindowsFontsDirectory))
                {
                    paths.AddRange(windowsFontFiles.Select(x => System.IO.Path.Combine(WslWindowsFontsDirectory, x)));
                }
                foreach (var path in paths)
                {
                    var files = System.IO.Directory.Exists(path)
                        ? System.IO.Directory.GetFiles(path).Where(x => new[] { ".ttf", ".ttc", ".otf", ".otc" }.Contains(System.IO.Path.GetExtension(x).ToLowerInvariant()))
                        : new[] { path };
                    foreach (var file in files.Where(System.IO.File.Exists))
                    {
                        RegisterFile(file);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        /// <summary>フォントのファイル（.ttc はすべての書体）を登録する。Windows の ＭＳ 系は日本語名でも引けるようにする</summary>
        public static void RegisterFile(string path)
        {
            for (var index = 0; ; index++)
            {
                var typeface = SKTypeface.FromFile(path, index);
                if (typeface == null)
                {
                    break;
                }
                AddRegistered(typeface.FamilyName, typeface, false);
                foreach (var (english, japanese) in windowsFontNames.Where(x => x.English == typeface.FamilyName))
                {
                    AddRegistered(japanese, typeface, false);
                }
            }
            typefaceCache.Clear();
        }

        /// <summary>日本語を描ける書体があるか（無いと日本語が豆腐になる）</summary>
        public static bool HasJapaneseFont()
        {
            return HasJapaneseGlyph(ResolveTypeface("", SKFontStyle.Normal));
        }

        /// <summary>書体に「あ」の字形があるか</summary>
        private static bool HasJapaneseGlyph(SKTypeface typeface)
        {
            using (var font = typeface.ToFont())
            {
                return font.ContainsGlyph('あ');
            }
        }

        public static SKTypeface ResolveTypeface(string name, SKFontStyle style)
        {
            EnsureExtraFontsLoaded();
            name = name ?? "";
            return typefaceCache.GetOrAdd((name, style.Weight, style.Width, (int)style.Slant), _ => FindTypeface(name, style));
        }

        private static SKTypeface FindTypeface(string name, SKFontStyle style)
        {
            var fallbacks = IsFixedPitchName(name)
                ? new[] { name, FixedPitchFamily, SansFamily }
                : new[] { name, name.Contains("明朝") || name.Contains("Mincho") ? SerifFamily : SansFamily };
            foreach (var family in fallbacks)
            {
                if (string.IsNullOrEmpty(family))
                {
                    continue;
                }
                // 登録した書体は、太さ・斜体が合うときだけ OS のフォントより優先する。
                // 太字だけの .ttc（NotoSansCJK-Bold.ttc など）を登録しても、標準の太さを引いたときに太字にならないように
                var own = FindRegistered(family, style, true);
                if (own != null)
                {
                    return own;
                }
                var candidate = SKTypeface.FromFamilyName(family, style);
                if (candidate != null && candidate.FamilyName == family && HasJapaneseGlyph(candidate))
                {
                    return candidate;
                }
                own = FindRegistered(family, style, false);
                if (own != null)
                {
                    return own;
                }
            }
            // 登録した書体（WSL の Windows のフォントなど）から、明朝・ゴシック・等幅の別に合うものを選ぶ
            var preferred = name.Contains("明朝") || name.Contains("Mincho")
                ? new[] { "ＭＳ Ｐ明朝", "ＭＳ 明朝" }
                : IsFixedPitchName(name) ? new[] { "ＭＳ ゴシック" } : new[] { "ＭＳ Ｐゴシック", "ＭＳ ゴシック" };
            foreach (var family in preferred)
            {
                var own = FindRegistered(family, style, false);
                if (own != null)
                {
                    return own;
                }
            }
            return SKFontManager.Default.MatchCharacter(null, style, null, 'あ')
                ?? registered.Values.SelectMany(x => x).FirstOrDefault(HasJapaneseGlyph)
                ?? registered.Values.SelectMany(x => x).FirstOrDefault()
                ?? SKTypeface.Default;
        }

        // フォントを作ったときに指定されたフォント名の寸法（Windows の寸法表にある場合）
        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<SKFont, object> requestedMetrics
            = new System.Runtime.CompilerServices.ConditionalWeakTable<SKFont, object>();

        public static SKFont Create(string name, float sizeInPixels, bool bold, bool italic)
        {
            var style = new SKFontStyle(
                bold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal,
                SKFontStyleWidth.Normal,
                italic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright);
            var typeface = ResolveTypeface(name, style);
            var font = new SKFont(typeface, sizeInPixels)
            {
                Embolden = bold && !typeface.IsBold,
                SkewX = italic && !typeface.IsItalic ? -0.2f : 0,
                Edging = SKFontEdging.Antialias,
            };
            if (WindowsFontMetrics.TryGet(name, out var metrics))
            {
                requestedMetrics.Add(font, metrics);
            }
            if (IsFixedPitchName(name))
            {
                MarkFixedPitch(font);
            }
            return font;
        }

        // ---- 等幅の指定の文字の並べ方（7474/SRC#1650） ----
        //
        // 元の SRC の ＭＳ 明朝・ＭＳ ゴシックは、Shift_JIS の 1 バイトの文字が全角の半分、2 バイトの文字が全角の幅で、
        // リスト・ステータスなどは文字列のバイト数（半角 1・全角 2）で桁を揃える。代わりの書体（Noto Sans Mono CJK JP など）は、
        // ギリシャ文字・キリル文字・°・引用符など（Shift_JIS では 2 バイト）を半角でも全角でもない幅で描くので桁がずれる。
        // 等幅の指定で作ったフォントは、文字ごとに Shift_JIS のバイト数の升目（1 バイト = フォントの大きさの半分）に置いて描き・測る。

        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<SKFont, object> fixedPitchFonts
            = new System.Runtime.CompilerServices.ConditionalWeakTable<SKFont, object>();
        private static readonly object FixedPitchMark = new object();

        /// <summary>このフォントを等幅の指定として扱う（Shift_JIS のバイト数の升目に並べる）</summary>
        public static void MarkFixedPitch(SKFont font)
        {
            fixedPitchFonts.Remove(font);
            fixedPitchFonts.Add(font, FixedPitchMark);
        }

        /// <summary>等幅の指定として作ったフォントか</summary>
        public static bool IsFixedPitch(SKFont font) => font != null && fixedPitchFonts.TryGetValue(font, out _);

        // 文字ごとの Shift_JIS のバイト数（0: Shift_JIS に無い）。初めて使うときに作る
        private static byte[] sjisBytes;

        private static byte[] SjisBytes()
        {
            if (sjisBytes != null)
            {
                return sjisBytes;
            }
            var table = new byte[0x10000];
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                var sjis = System.Text.Encoding.GetEncoding(932, System.Text.EncoderFallback.ExceptionFallback, System.Text.DecoderFallback.ExceptionFallback);
                var buffer = new char[1];
                for (var c = 0x20; c < 0x10000; c++)
                {
                    if (c >= 0xD800 && c <= 0xDFFF)
                    {
                        continue;
                    }
                    buffer[0] = (char)c;
                    try
                    {
                        table[c] = (byte)sjis.GetByteCount(buffer);
                    }
                    catch (System.Text.EncoderFallbackException)
                    {
                    }
                }
            }
            catch (Exception)
            {
                // Shift_JIS を使えない環境では、すべて書体の幅から決める
            }
            sjisBytes = table;
            return table;
        }

        /// <summary>
        /// 升目の数（半角 1・全角 2）。Shift_JIS にある文字はそのバイト数、無い文字（絵文字など）は書体の幅を半角の幅で割って丸めた数
        /// </summary>
        private static int Cells(SKFont font, string ch)
        {
            if (ch.Length == 1)
            {
                var n = SjisBytes()[ch[0]];
                if (n > 0)
                {
                    return n;
                }
            }
            var half = font.Size / 2;
            return half <= 0 ? 0 : (int)Math.Round(font.MeasureText(ch) / half);
        }

        /// <summary>文字（サロゲートペアは 1 文字）ごとに分ける</summary>
        private static IEnumerable<string> TextElements(string text)
        {
            for (var i = 0; i < text.Length; i++)
            {
                if (char.IsHighSurrogate(text[i]) && i + 1 < text.Length && char.IsLowSurrogate(text[i + 1]))
                {
                    yield return text.Substring(i, 2);
                    i++;
                }
                else
                {
                    yield return text.Substring(i, 1);
                }
            }
        }

        /// <summary>文字列の幅。等幅の指定のフォントは升目の数 × 半角の幅、ほかは書体の幅</summary>
        public static float MeasureText(SKFont font, string text)
        {
            text = text ?? "";
            if (!IsFixedPitch(font))
            {
                return font.MeasureText(text);
            }
            var cells = 0;
            foreach (var ch in TextElements(text))
            {
                cells += Cells(font, ch);
            }
            return cells * font.Size / 2;
        }

        /// <summary>
        /// 文字列を描く（SKCanvas.DrawText と同じ引数。y はベースライン）。等幅の指定のフォントは、文字ごとに升目の中央に置く。
        /// 書体の幅が升目と同じ文字が続く所は、まとめて描く
        /// </summary>
        public static void DrawText(SKCanvas canvas, string text, float x, float y, SKTextAlign align, SKFont font, SKPaint paint)
        {
            text = text ?? "";
            if (!IsFixedPitch(font))
            {
                canvas.DrawText(text, x, y, align, font, paint);
                return;
            }
            var width = MeasureText(font, text);
            var left = align == SKTextAlign.Center ? x - width / 2 : align == SKTextAlign.Right ? x - width : x;
            var half = font.Size / 2;
            var run = new System.Text.StringBuilder();
            var runLeft = left;
            var pen = left;
            void Flush()
            {
                if (run.Length > 0)
                {
                    canvas.DrawText(run.ToString(), runLeft, y, SKTextAlign.Left, font, paint);
                    run.Clear();
                }
            }
            foreach (var ch in TextElements(text))
            {
                var cell = Cells(font, ch) * half;
                var advance = font.MeasureText(ch);
                if (Math.Abs(advance - cell) < 0.01f)
                {
                    if (run.Length == 0)
                    {
                        runLeft = pen;
                    }
                    run.Append(ch);
                }
                else if (advance > cell && cell > 0)
                {
                    // 升目より広い字形（等幅でない代わりの書体の W・M など）は、横に縮めて升目に収める（隣の文字に重ねない）
                    Flush();
                    var scaleX = font.ScaleX;
                    font.ScaleX = scaleX * cell / advance;
                    canvas.DrawText(ch, pen, y, SKTextAlign.Left, font, paint);
                    font.ScaleX = scaleX;
                }
                else
                {
                    Flush();
                    canvas.DrawText(ch, pen + (cell - advance) / 2, y, SKTextAlign.Left, font, paint);
                }
                pen += cell;
            }
            Flush();
        }

        /// <summary>
        /// Noto CJK は行の高さ（hhea）が 1.448em あり、ＭＳ 明朝等（1em）より大きいため行がずれて下が見切れる。
        /// Noto CJK では文字枠（OS/2 typo: 上 0.88em・下 0.12em）を行の高さとして扱う。
        /// </summary>
        private static bool UsesEmBoxMetrics(SKFont font)
        {
            var family = font.Typeface?.FamilyName ?? "";
            return family.StartsWith("Noto ") && family.Contains(" CJK ");
        }

        /// <summary>
        /// 行の上端からベースラインまでの距離（ピクセル）。
        /// 指定されたフォント名が Windows の寸法表にあれば、その寸法（元の SRC と同じ位置）にする。
        /// </summary>
        public static float Ascent(SKFont font)
        {
            if (requestedMetrics.TryGetValue(font, out var m))
            {
                return font.Size * ((WindowsFontMetrics.Metrics)m).Ascent;
            }
            return UsesEmBoxMetrics(font) ? font.Size * 0.88f : -font.Metrics.Ascent;
        }

        /// <summary>行の高さ（ピクセル）。TextHeight 関数の値にもなる。</summary>
        public static float LineHeight(SKFont font)
        {
            if (requestedMetrics.TryGetValue(font, out var m))
            {
                return font.Size * ((WindowsFontMetrics.Metrics)m).LineHeight;
            }
            return UsesEmBoxMetrics(font) ? font.Size : font.Spacing;
        }
    }
}

namespace SRCRender.Compat
{
    public static class FontExtension
    {
        public static Font ReSize(this Font font, float emSize)
        {
            return new Font(font.Name, emSize, font.Style, font.Unit);
        }

        public static Font Bold(this Font font)
        {
            return new Font(font, font.Style | FontStyle.Bold);
        }

        public static Font UnBold(this Font font)
        {
            return new Font(font, font.Style & ~FontStyle.Bold);
        }

        public static Font Italic(this Font font)
        {
            return new Font(font, font.Style | FontStyle.Italic);
        }

        public static Font UnItalic(this Font font)
        {
            return new Font(font, font.Style & ~FontStyle.Italic);
        }
    }

    public static class PointFExtension
    {
        public static System.Drawing.PointF Add(this System.Drawing.PointF p, float x, float y)
        {
            return new System.Drawing.PointF(p.X + x, p.Y + y);
        }

        public static System.Drawing.PointF AddX(this System.Drawing.PointF p, float v)
        {
            return Add(p, v, 0);
        }

        public static System.Drawing.PointF AddY(this System.Drawing.PointF p, float v)
        {
            return Add(p, 0, v);
        }
    }
}
