using SkiaSharp;

namespace SRCRender
{
    /// <summary>
    /// 画面の合成で SKBitmap を描くときの補助。
    /// </summary>
    public static class SkiaCanvasExtensions
    {
        /// <summary>
        /// bitmap の画素を複製せずに描く。
        /// SKCanvas.DrawBitmap は描くたびに SKImage.FromBitmap で画像を作り、変更できる SKBitmap では画素をまるごと複製する。
        /// ここで描くキャンバスはラスタ（SKBitmap）で描画はその場で終わるので、画素を借りた SKImage で描く。
        /// </summary>
        public static void DrawBitmapShared(this SKCanvas canvas, SKBitmap bitmap, SKRect source, SKRect dest, SKPaint paint = null)
        {
            using (var pixmap = bitmap.PeekPixels())
            using (var image = pixmap != null ? SKImage.FromPixels(pixmap) : null)
            {
                if (image == null)
                {
                    canvas.DrawBitmap(bitmap, source, dest, SKSamplingOptions.Default, paint);
                    return;
                }
                canvas.DrawImage(image, source, dest, SKSamplingOptions.Default, paint);
            }
        }

        /// <inheritdoc cref="DrawBitmapShared(SKCanvas, SKBitmap, SKRect, SKRect, SKPaint)"/>
        public static void DrawBitmapShared(this SKCanvas canvas, SKBitmap bitmap, SKRect dest, SKPaint paint = null)
        {
            canvas.DrawBitmapShared(bitmap, SKRect.Create(bitmap.Width, bitmap.Height), dest, paint);
        }

        /// <inheritdoc cref="DrawBitmapShared(SKCanvas, SKBitmap, SKRect, SKRect, SKPaint)"/>
        public static void DrawBitmapShared(this SKCanvas canvas, SKBitmap bitmap, float x, float y, SKPaint paint = null)
        {
            canvas.DrawBitmapShared(bitmap, SKRect.Create(x, y, bitmap.Width, bitmap.Height), paint);
        }
    }
}
