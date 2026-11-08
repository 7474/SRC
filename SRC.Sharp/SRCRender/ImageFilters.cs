using SkiaSharp;
using SRCCore.VB;
using System;
using System.Drawing;

namespace SRCRender
{
    /// <summary>
    /// SRC の画像効果（暗・明・セピア・白黒・夕焼け・水中・シルエット・ネガポジ反転・カラーフィルタ・回転・反転）。
    /// 元の SRC の Graphics.bas と同じく、画素ごとに同じ式（Double から Long・Byte への代入は銀行丸め、\ は切り捨て）で計算する。
    /// 透明な画素（アルファが 0）は変えない。元の SRC の透過の指定（is_transparent）では背景色の画素を変えずに後で抜くので、
    /// 先に背景色を抜いてある SRC# ではこれに当たる。
    /// </summary>
    public static class ImageFilters
    {
        /// <summary>
        /// 画素ごとに RGB を変えた画像を返す（元の画像は変更しない）。アルファは変えず、透明な画素はそのまま。
        /// </summary>
        private static SKBitmap MapPixels(SKBitmap source, Func<SKColor, SKColor> map)
        {
            var pixels = source.Pixels;
            for (var i = 0; i < pixels.Length; i++)
            {
                var c = pixels[i];
                if (c.Alpha == 0)
                {
                    continue;
                }
                pixels[i] = map(c).WithAlpha(c.Alpha);
            }
            var result = new SKBitmap(source.Width, source.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
            result.Pixels = pixels;
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

        private static byte ToByte(double x) => (byte)VBMath.CLng(x);

        // rgb = 0.299 * .Red + 0.587 * .Green + 0.114 * .Blue（Long への代入で銀行丸め）
        private static int Luminance(SKColor c) => VBMath.CLng(0.299 * c.Red + 0.587 * c.Green + 0.114 * c.Blue);

        /// <summary>Graphics.bas の Dark（各色 \ 2）</summary>
        public static SKBitmap ApplyDark(SKBitmap b, bool dispose = true)
            => Replace(b, MapPixels(b, c => new SKColor((byte)(c.Red / 2), (byte)(c.Green / 2), (byte)(c.Blue / 2))), dispose);

        /// <summary>Graphics.bas の Sepia</summary>
        public static SKBitmap ApplySepia(SKBitmap b, bool dispose = true)
            => Replace(b, MapPixels(b, c =>
            {
                var rgb = Luminance(c);
                return new SKColor((byte)Math.Min(VBMath.CLng(1.1 * rgb), 255), ToByte(0.9 * rgb), ToByte(0.7 * rgb));
            }), dispose);

        /// <summary>Graphics.bas の Monotone</summary>
        public static SKBitmap ApplyMonotone(SKBitmap b, bool dispose = true)
            => Replace(b, MapPixels(b, c =>
            {
                var rgb = (byte)Luminance(c);
                return new SKColor(rgb, rgb, rgb);
            }), dispose);

        /// <summary>Graphics.bas の Sunset</summary>
        public static SKBitmap ApplySunset(SKBitmap b, bool dispose = true)
            => Replace(b, MapPixels(b, c =>
            {
                var rgb = Luminance(c);
                return new SKColor(
                    (byte)Math.Min(VBMath.CLng(0.2 * c.Red + 1.3 * rgb), 255),
                    ToByte(0.2 * c.Green + 0.4 * rgb),
                    ToByte(0.2 * c.Blue + 0.2 * rgb));
            }), dispose);

        /// <summary>Graphics.bas の Water</summary>
        public static SKBitmap ApplyWater(SKBitmap b, bool dispose = true)
            => Replace(b, MapPixels(b, c =>
            {
                var rgb = Luminance(c);
                return new SKColor(ToByte(0.6 * rgb), ToByte(0.8 * rgb), (byte)rgb);
            }), dispose);

        /// <summary>
        /// Graphics.bas の Silhouette。背景色（省略したときは元の SRC の BGColor の既定の白）の画素を白に、ほかを黒にする。
        /// </summary>
        public static SKBitmap ApplySilhouette(SKBitmap b, bool dispose = true, SKColor? background = null)
        {
            var bg = background ?? SKColors.White;
            return Replace(b, MapPixels(b, c =>
                c.Red == bg.Red && c.Green == bg.Green && c.Blue == bg.Blue
                    ? SKColors.White
                    : SKColors.Black), dispose);
        }

        /// <summary>Graphics.bas の NegPosReverse</summary>
        public static SKBitmap ApplyNegPos(SKBitmap b, bool dispose = true)
            => Replace(b, MapPixels(b, c => new SKColor((byte)(255 - c.Red), (byte)(255 - c.Green), (byte)(255 - c.Blue))), dispose);

        /// <summary>
        /// Graphics.bas の ColorFilter。透過率 transPar（0〜1）を百分率の整数に丸め、
        /// (元の色 * (100 - 透過率) + 描画色 * 透過率) \ 100 にする。
        /// 透過のとき、描画色が背景色と同じなら、透過で抜けないように描画色を少しずらす（255 の成分を 254 にする）。
        /// </summary>
        public static SKBitmap ApplyColorFilter(SKBitmap b, Color color, double transPar, bool dispose = true,
            bool transparent = false, SKColor? background = null)
        {
            var bg = background ?? SKColors.White;
            // tratio = MinLng(MaxLng(100 * trans_par, 0), 100)（Long の引数への代入で銀行丸め）
            var tratio = Math.Min(Math.Max(VBMath.CLng(100 * transPar), 0), 100);
            if (tratio == 0)
            {
                // 透過しない場合はそのまま
                return dispose ? b : Copy(b);
            }

            int r2 = color.R, g2 = color.G, b2 = color.B;
            if (transparent && bg.Red == r2 && bg.Green == g2 && bg.Blue == b2)
            {
                if (r2 == 255)
                {
                    r2 = 254;
                }
                else if (g2 == 255)
                {
                    g2 = 254;
                }
                else if (b2 == 255)
                {
                    b2 = 254;
                }
            }

            // 元の SRC の「trans_par = 100」の分岐は、透過率が 0〜1 なので通らない（100% は下の式で描画色そのものになる）
            return Replace(b, MapPixels(b, c => new SKColor(
                (byte)((c.Red * (100 - tratio) + r2 * tratio) / 100),
                (byte)((c.Green * (100 - tratio) + g2 * tratio) / 100),
                (byte)((c.Blue * (100 - tratio) + b2 * tratio) / 100))), dispose);
        }

        /// <summary>
        /// Graphics.bas の Bright（各色 +80）。透過のときは、255 の成分はそのまま、ほかは 254 で止める
        /// （元の SRC は背景色の白を後で抜くので、白にならないようにしている）。
        /// </summary>
        public static SKBitmap ApplyBright(SKBitmap b, bool dispose = true, bool transparent = false)
        {
            if (transparent)
            {
                static byte Up(byte v) => v == 255 ? v : (byte)Math.Min(v + 80, 254);
                return Replace(b, MapPixels(b, c => new SKColor(Up(c.Red), Up(c.Green), Up(c.Blue))), dispose);
            }
            return Replace(b, MapPixels(b, c => new SKColor(
                (byte)Math.Min(c.Red + 80, 255), (byte)Math.Min(c.Green + 80, 255), (byte)Math.Min(c.Blue + 80, 255))), dispose);
        }

        /// <summary>
        /// マップの描画モード（夜・セピア・白黒・夕焼け・水中・フィルタ）を適用する。
        /// 元の SRC の GUI.bas の SetupBackground・DrawMapLayer で呼ぶ Graphics.bas の処理と同じ。
        /// </summary>
        public static SKBitmap ApplyDrawMode(SKBitmap b, string drawMode, Color filterColor, double filterTransPercent, bool dispose = true,
            bool transparent = false)
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
                    // レイヤーの画像は背景色を白として透過で処理する（元の SRC の BGColor = vbWhite）
                    return ApplyColorFilter(b, filterColor, filterTransPercent, dispose, transparent, SKColors.White);
                default:
                    return b;
            }
        }

        /// <summary>Graphics.bas の HReverse（左右反転）・VReverse（上下反転）</summary>
        public static SKBitmap Flip(SKBitmap source, bool horizontal, bool vertical, bool dispose = true)
        {
            int w = source.Width, h = source.Height;
            var src = source.Pixels;
            var dst = new SKColor[src.Length];
            for (var i = 0; i < h; i++)
            {
                var si = vertical ? h - i - 1 : i;
                for (var j = 0; j < w; j++)
                {
                    var sj = horizontal ? w - j - 1 : j;
                    dst[w * i + j] = src[w * si + sj];
                }
            }
            var result = new SKBitmap(w, h, SKColorType.Bgra8888, SKAlphaType.Premul);
            result.Pixels = dst;
            return Replace(source, result, dispose);
        }

        /// <summary>
        /// Graphics.bas の Rotate。中心を軸に右に angle 度回す。はみ出した部分は切り取り、空いた部分は background で塗る。
        /// 元の SRC と同じく、各画素について回転前の位置を三角関数で求め、CLng で丸めた画素を取る（補間しない）。
        /// doSame が false のとき、90 度の倍数（90・270 度は正方形のときだけ）は画素の入れ替えで回す。
        /// </summary>
        public static SKBitmap Rotate(SKBitmap source, int angle, SKColor background, bool dispose = true, bool doSame = false)
        {
            int w = source.Width, h = source.Height;
            var src = source.Pixels;
            var dst = new SKColor[src.Length];

            // 360 度で一回転。負の場合は正の角度に
            angle %= 360;
            if (angle < 0)
            {
                angle = 360 + angle;
            }

            var done = false;
            if (!doSame)
            {
                switch (angle)
                {
                    case 0:
                        return dispose ? source : Copy(source);
                    case 90:
                        if (w == h)
                        {
                            for (var i = 0; i < h; i++)
                            {
                                for (var j = 0; j < w; j++)
                                {
                                    dst[w * i + j] = src[w * (w - j - 1) + i];
                                }
                            }
                            done = true;
                        }
                        break;
                    case 180:
                        for (var i = 0; i < h; i++)
                        {
                            for (var j = 0; j < w; j++)
                            {
                                dst[w * i + j] = src[w * (h - i - 1) + w - j - 1];
                            }
                        }
                        done = true;
                        break;
                    case 270:
                        if (w == h)
                        {
                            for (var i = 0; i < h; i++)
                            {
                                for (var j = 0; j < w; j++)
                                {
                                    dst[w * i + j] = src[w * j + h - i - 1];
                                }
                            }
                            done = true;
                        }
                        break;
                }
            }

            if (!done)
            {
                // 座標の計算は画像の中心を座標原点にして行う
                var xbase = (w - 1) / 2d;
                var ybase = (h - 1) / 2d;
                // 回転によるベクトル（元の SRC の円周率は 3.14159265）
                var rad = (double)(90 - angle) * 3.14159265 / 180;
                var dsin = Math.Sin(rad);
                var dcos = Math.Cos(rad);
                for (var i = 0; i < h; i++)
                {
                    var yoffset = i - ybase;
                    var xsrc0 = xbase + yoffset * dcos;
                    var ysrc0 = ybase + yoffset * dsin;
                    for (var j = 0; j < w; j++)
                    {
                        var xoffset = j - xbase;
                        var xsrc = VBMath.CLng(xsrc0 + xoffset * dsin);
                        var ysrc = VBMath.CLng(ysrc0 - xoffset * dcos);
                        dst[w * i + j] = xsrc < 0 || w <= xsrc || ysrc < 0 || h <= ysrc
                            ? background
                            : src[w * ysrc + xsrc];
                    }
                }
            }

            var result = new SKBitmap(w, h, SKColorType.Bgra8888, SKAlphaType.Premul);
            result.Pixels = dst;
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
                canvas.DrawBitmap(source, 0, 0, SKSamplingOptions.Default);
            }
            return result;
        }
    }
}
