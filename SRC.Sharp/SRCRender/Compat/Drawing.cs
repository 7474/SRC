using SkiaSharp;
using System;
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
                return font.Spacing;
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
                    canvas.DrawText(line, x, y - skFont.Metrics.Ascent, SKTextAlign.Left, skFont, paint);
                    y += skFont.Spacing;
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
                    width = Math.Max(width, skFont.MeasureText(line));
                }
                return new SizeF(width, skFont.Spacing * Math.Max(1, lines.Length));
            }
        }

        private static string[] SplitLines(string s)
        {
            return (s ?? "").Replace("\r\n", "\n").Replace('\r', '\n').TrimEnd('\n').Split('\n');
        }

        public void DrawImage(SKBitmap image, float x, float y)
        {
            canvas.DrawBitmap(image, x, y);
        }

        public void DrawImage(SKBitmap image, Rectangle destRect)
        {
            canvas.DrawBitmap(image, SKRect.Create(destRect.X, destRect.Y, destRect.Width, destRect.Height));
        }

        /// <summary>image の srcRect の部分を (x, y) に等倍で描く。</summary>
        public void DrawImage(SKBitmap image, float x, float y, Rectangle srcRect, GraphicsUnit unit)
        {
            canvas.DrawBitmap(image,
                SKRect.Create(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height),
                SKRect.Create(x, y, srcRect.Width, srcRect.Height));
        }

        public void DrawImage(SKBitmap image, Rectangle destRect, Rectangle srcRect, GraphicsUnit unit)
        {
            canvas.DrawBitmap(image,
                SKRect.Create(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height),
                SKRect.Create(destRect.X, destRect.Y, destRect.Width, destRect.Height));
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
    /// 日本語フォントの解決。明朝系は Noto Serif CJK JP、それ以外は Noto Sans CJK JP を優先する。
    /// </summary>
    public static class SkiaFonts
    {
        public static SKFont Create(string name, float sizeInPixels, bool bold, bool italic)
        {
            name = name ?? "";
            var style = new SKFontStyle(
                bold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal,
                SKFontStyleWidth.Normal,
                italic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright);
            SKTypeface typeface = null;
            foreach (var family in new[] { name, name.Contains("明朝") ? "Noto Serif CJK JP" : "Noto Sans CJK JP" })
            {
                var candidate = SKTypeface.FromFamilyName(family, style);
                if (candidate != null && candidate.FamilyName == family && candidate.ContainsGlyph('あ'))
                {
                    typeface = candidate;
                    break;
                }
            }
            typeface = typeface ?? SKFontManager.Default.MatchCharacter(null, style, null, 'あ') ?? SKTypeface.Default;
            return new SKFont(typeface, sizeInPixels)
            {
                Embolden = bold && !typeface.IsBold,
                SkewX = italic && !typeface.IsItalic ? -0.2f : 0,
                Edging = SKFontEdging.Antialias,
            };
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
