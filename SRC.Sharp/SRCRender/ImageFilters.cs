using SkiaSharp;
using System.Drawing;

namespace SRCRender
{
    /// <summary>
    /// SRC の画像効果（暗・セピア・白黒・夕焼け・水中・シルエット・ネガポジ反転・カラーフィルタ・回転・反転）。
    /// 行列は WinForms 版（GDI+ の ColorMatrix）と同じ値を使う。
    /// </summary>
    public static class ImageFilters
    {
        // GDI+ の ColorMatrix（行: 入力 R,G,B,A,1 / 列: 出力 R,G,B,A,W）
        private static readonly float[,] Dark =
        {
            { 0.5f, 0, 0, 0, 0 },
            { 0, 0.5f, 0, 0, 0 },
            { 0, 0, 0.5f, 0, 0 },
            { 0, 0, 0, 1, 0 },
            { 0, 0, 0, 0, 1 },
        };

        private static readonly float[,] Sepia =
        {
            { 0.299f * 1.1f, 0.299f * 0.9f, 0.299f * 0.7f, 0, 0 },
            { 0.587f * 1.1f, 0.587f * 0.9f, 0.587f * 0.7f, 0, 0 },
            { 0.114f * 1.1f, 0.114f * 0.9f, 0.114f * 0.7f, 0, 0 },
            { 0, 0, 0, 1, 0 },
            { 0, 0, 0, 0, 1 },
        };

        private static readonly float[,] Monochrome =
        {
            { 0.3086f, 0.3086f, 0.3086f, 0, 0 },
            { 0.6094f, 0.6094f, 0.6094f, 0, 0 },
            { 0.0820f, 0.0820f, 0.0820f, 0, 0 },
            { 0, 0, 0, 1, 0 },
            { 0, 0, 0, 0, 1 },
        };

        private static readonly float[,] Water =
        {
            { 0.299f * 0.6f, 0.299f * 0.8f, 0.299f, 0, 0 },
            { 0.587f * 0.6f, 0.587f * 0.8f, 0.587f, 0, 0 },
            { 0.114f * 0.6f, 0.114f * 0.8f, 0.114f, 0, 0 },
            { 0, 0, 0, 1, 0 },
            { 0, 0, 0, 0, 1 },
        };

        private static readonly float[,] Sunset =
        {
            { 0.299f * 1.3f + 0.2f, 0.299f * 0.4f, 0.299f * 0.2f, 0, 0 },
            { 0.587f * 1.3f, 0.587f * 0.4f + 0.2f, 0.587f * 0.2f, 0, 0 },
            { 0.114f * 1.3f, 0.114f * 0.4f, 0.114f * 0.2f + 0.2f, 0, 0 },
            { 0, 0, 0, 1, 0 },
            { 0, 0, 0, 0, 1 },
        };

        private static readonly float[,] Silhouette =
        {
            { 255, 255, 255, 0, 0 },
            { 255, 255, 255, 0, 0 },
            { 255, 255, 255, 0, 0 },
            { 0, 0, 0, 1, 0 },
            { -764, -764, -764, 0, 1 },
        };

        private static readonly float[,] NegPos =
        {
            { -1, 0, 0, 0, 0 },
            { 0, -1, 0, 0, 0 },
            { 0, 0, -1, 0, 0 },
            { 0, 0, 0, 1, 0 },
            { 1, 1, 1, 0, 1 },
        };

        private static float[,] ColorFilterMatrix(Color color, float alpha)
        {
            return new float[,]
            {
                { 1f - alpha, 0, 0, 0, 0 },
                { 0, 1f - alpha, 0, 0, 0 },
                { 0, 0, 1f - alpha, 0, 0 },
                { 0, 0, 0, 1, 0 },
                { color.R * alpha / 255f, color.G * alpha / 255f, color.B * alpha / 255f, 0, 1 },
            };
        }

        /// <summary>
        /// GDI+ の ColorMatrix を Skia のカラーマトリクス（4x5、平行移動は 0〜1）に変換する。
        /// </summary>
        private static SKColorFilter ToColorFilter(float[,] m)
        {
            var skia = new float[20];
            for (var c = 0; c < 4; c++)
            {
                for (var i = 0; i < 4; i++)
                {
                    skia[c * 5 + i] = m[i, c];
                }
                skia[c * 5 + 4] = m[4, c];
            }
            return SKColorFilter.CreateColorMatrix(skia);
        }

        /// <summary>
        /// カラーマトリクスを適用した画像を返す（元の画像は変更しない）。
        /// </summary>
        private static SKBitmap Apply(SKBitmap source, float[,] matrix)
        {
            var result = new SKBitmap(source.Width, source.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
            using (var canvas = new SKCanvas(result))
            using (var filter = ToColorFilter(matrix))
            using (var paint = new SKPaint { ColorFilter = filter })
            {
                canvas.Clear(SKColors.Transparent);
                canvas.DrawBitmap(source, 0, 0, paint);
            }
            return result;
        }

        private static SKBitmap Replace(SKBitmap source, SKBitmap result, bool disposeSource)
        {
            if (disposeSource)
            {
                source.Dispose();
            }
            return result;
        }

        public static SKBitmap ApplyDark(SKBitmap b, bool dispose = true) => Replace(b, Apply(b, Dark), dispose);
        public static SKBitmap ApplySepia(SKBitmap b, bool dispose = true) => Replace(b, Apply(b, Sepia), dispose);
        public static SKBitmap ApplyMonotone(SKBitmap b, bool dispose = true) => Replace(b, Apply(b, Monochrome), dispose);
        public static SKBitmap ApplySunset(SKBitmap b, bool dispose = true) => Replace(b, Apply(b, Sunset), dispose);
        public static SKBitmap ApplyWater(SKBitmap b, bool dispose = true) => Replace(b, Apply(b, Water), dispose);
        public static SKBitmap ApplySilhouette(SKBitmap b, bool dispose = true) => Replace(b, Apply(b, Silhouette), dispose);
        public static SKBitmap ApplyNegPos(SKBitmap b, bool dispose = true) => Replace(b, Apply(b, NegPos), dispose);
        public static SKBitmap ApplyColorFilter(SKBitmap b, Color color, float alpha, bool dispose = true) => Replace(b, Apply(b, ColorFilterMatrix(color, alpha)), dispose);

        /// <summary>明るくする（元の SRC と同じく各色 +80）</summary>
        public static SKBitmap ApplyBright(SKBitmap b, bool dispose = true)
        {
            const float add = 80f / 255f;
            var m = new float[,]
            {
                { 1, 0, 0, 0, 0 },
                { 0, 1, 0, 0, 0 },
                { 0, 0, 1, 0, 0 },
                { 0, 0, 0, 1, 0 },
                { add, add, add, 0, 1 },
            };
            return Replace(b, Apply(b, m), dispose);
        }

        /// <summary>マップの描画モード（夜・セピア・白黒・夕焼け・水中・フィルタ）を適用する。</summary>
        public static SKBitmap ApplyDrawMode(SKBitmap b, string drawMode, Color filterColor, double filterTransPercent, bool dispose = true)
        {
            switch (drawMode ?? "")
            {
                case "夜":
                    return ApplyDark(b, dispose);
                case "セピア":
                    return ApplySepia(b, dispose);
                case "白黒":
                    return ApplyMonotone(b, dispose);
                case "夕焼け":
                    return ApplySunset(b, dispose);
                case "水中":
                    return ApplyWater(b, dispose);
                case "フィルタ":
                    return ApplyColorFilter(b, filterColor, (float)filterTransPercent, dispose);
                default:
                    return b;
            }
        }

        /// <summary>左右・上下反転</summary>
        public static SKBitmap Flip(SKBitmap source, bool horizontal, bool vertical, bool dispose = true)
        {
            var result = new SKBitmap(source.Width, source.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
            using (var canvas = new SKCanvas(result))
            {
                canvas.Clear(SKColors.Transparent);
                canvas.Scale(horizontal ? -1 : 1, vertical ? -1 : 1, source.Width / 2f, source.Height / 2f);
                canvas.DrawBitmap(source, 0, 0);
            }
            return Replace(source, result, dispose);
        }

        /// <summary>中心を軸に右回転（度）。はみ出した部分は切り取り、空いた部分は background で塗る。</summary>
        public static SKBitmap Rotate(SKBitmap source, float angle, SKColor background, bool dispose = true)
        {
            var result = new SKBitmap(source.Width, source.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
            using (var canvas = new SKCanvas(result))
            {
                canvas.Clear(background);
                canvas.RotateDegrees(angle % 360, source.Width / 2f, source.Height / 2f);
                canvas.DrawBitmap(source, 0, 0);
            }
            return Replace(source, result, dispose);
        }

        /// <summary>透明・半透明の画素を持つか（PNG などのアルファチャネル）。</summary>
        public static bool HasTransparency(SKBitmap source)
        {
            if (source == null || source.AlphaType == SKAlphaType.Opaque)
            {
                return false;
            }
            foreach (var c in source.Pixels)
            {
                if (c.Alpha < 255)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 指定色を透明にした画像を返す（元の画像は変更しない）。
        /// 画像が透明な画素を持つ（アルファチャネルで透過を指定している）場合は、指定色では抜かずにそのまま使う。
        /// </summary>
        public static SKBitmap MakeTransparent(SKBitmap source, SKColor transparentColor)
        {
            if (HasTransparency(source))
            {
                return Copy(source);
            }
            var result = new SKBitmap(source.Width, source.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
            for (var y = 0; y < source.Height; y++)
            {
                for (var x = 0; x < source.Width; x++)
                {
                    var c = source.GetPixel(x, y);
                    result.SetPixel(x, y, c.Red == transparentColor.Red && c.Green == transparentColor.Green && c.Blue == transparentColor.Blue
                        ? SKColors.Transparent
                        : c);
                }
            }
            return result;
        }

        public static SKBitmap Copy(SKBitmap source)
        {
            var result = new SKBitmap(source.Width, source.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
            using (var canvas = new SKCanvas(result))
            {
                canvas.Clear(SKColors.Transparent);
                canvas.DrawBitmap(source, 0, 0);
            }
            return result;
        }
    }
}
