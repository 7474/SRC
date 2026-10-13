using SkiaSharp;
using SRCCore;
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Drawing;

namespace SRCRender
{
    // 画像・文字列・図形の描画（PaintPicture / PaintString / Line など）。
    // WinForms 版（SRCSharpFormGUI.draw.cs、Forms/Main.guiscrean.cs）に合わせる。
    public partial class ScreenGUI
    {
        private const float PointToPixel = 96f / 72f;

        private static readonly PaintFont DefaultPaintFont = new PaintFont { Name = "ＭＳ Ｐ明朝", Size = 16, Bold = true };
        private static readonly PaintFont StatusPaintFont = new PaintFont { Name = "ＭＳ Ｐ明朝", Size = 9 };
        private static readonly PaintFont SysPaintFont = new PaintFont { Name = "ＭＳ Ｐ明朝", Size = 9, Bold = true };
        private static readonly PaintFont BattleAnimePaintFont = new PaintFont { Name = "ＭＳ Ｐ明朝", Size = 8 };

        private SKPoint currentDrawStringPoint;

        public PaintFont CurrentPaintFont { get; private set; } = DefaultPaintFont;
        public Color CurrentPaintColor { get; private set; } = Color.White;

        private static SKColor ToSKColor(Color color)
        {
            return new SKColor(color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// SRC のフォント指定を SKFont にする（解決規則は SkiaFonts）。
        /// </summary>
        internal SKFont CreateFont(PaintFont paintFont)
        {
            return Compat.SkiaFonts.Create(paintFont.Name, paintFont.Size * PointToPixel, paintFont.Bold, paintFont.Italic);
        }

        #region 画像

        public bool DrawPicture(string fname, int dx, int dy, int dw, int dh, int sx, int sy, int sw, int sh, string draw_option)
        {
            switch (fname ?? "")
            {
                case "":
                case "-.bmp":
                case "EFFECT_Void.bmp":
                    return false;
            }

            var bgColor = Color.White;
            float transPar = -1;
            bool permanent = false, transparent = false;
            int brightCount = 0, darkCount = 0;
            bool isMonotone = false, isSepia = false, isSunset = false, isWater = false, isColorFilter = false;
            bool hrev = false, vrev = false, negpos = false, isSil = false;
            bool topPart = false, bottomPart = false, rightPart = false, leftPart = false;
            bool trightPart = false, tleftPart = false, brightPart = false, bleftPart = false;
            bool onMsgWindow = false, onStatusWindow = false, keepPicture = false;
            Color fcolor = default;
            var angle = 0;

            var opts = GeneralLib.ToL(draw_option);
            for (var i = 0; i < opts.Count; i++)
            {
                var opt = opts[i];
                switch (opt ?? "")
                {
                    case "背景":
                        permanent = true;
                        switch (Map.MapDrawMode ?? "")
                        {
                            case "夜": darkCount++; break;
                            case "白黒": isMonotone = true; break;
                            case "セピア": isSepia = true; break;
                            case "夕焼け": isSunset = true; break;
                            case "水中": isWater = true; break;
                            case "フィルタ":
                                isColorFilter = true;
                                fcolor = Map.MapDrawFilterColor;
                                break;
                        }
                        break;
                    case "透過": transparent = true; break;
                    case "白黒": isMonotone = true; break;
                    case "セピア": isSepia = true; break;
                    case "夕焼け": isSunset = true; break;
                    case "水中": isWater = true; break;
                    case "明": brightCount++; break;
                    case "暗": darkCount++; break;
                    case "左右反転": hrev = true; break;
                    case "上下反転": vrev = true; break;
                    case "ネガポジ反転": negpos = true; break;
                    case "シルエット": isSil = true; break;
                    case "上半分": topPart = true; break;
                    case "下半分": bottomPart = true; break;
                    case "右半分": rightPart = true; break;
                    case "左半分": leftPart = true; break;
                    case "右上": trightPart = true; break;
                    case "左上": tleftPart = true; break;
                    case "右下": brightPart = true; break;
                    case "左下": bleftPart = true; break;
                    case "メッセージ": onMsgWindow = true; break;
                    case "ステータス": onStatusWindow = true; break;
                    case "保持": keepPicture = true; break;
                    case "右回転":
                        i++;
                        angle = GeneralLib.StrToLng(i < opts.Count ? opts[i] : "0");
                        break;
                    case "左回転":
                        i++;
                        angle = -GeneralLib.StrToLng(i < opts.Count ? opts[i] : "0");
                        break;
                    case "フィルタ": isColorFilter = true; break;
                    default:
                        if (Strings.Right(opt, 1) == "%" && Information.IsNumeric(Strings.Left(opt, Strings.Len(opt) - 1)))
                        {
                            transPar = (float)Math.Max(0d, Math.Min(1d, Conversions.ToDouble(Strings.Left(opt, Strings.Len(opt) - 1)) / 100d));
                        }
                        else if (isColorFilter)
                        {
                            ColorExtension.TryFromHexString(opt, out fcolor);
                        }
                        else if (ColorExtension.TryFromHexString(opt, out var color))
                        {
                            bgColor = color;
                        }
                        break;
                }
            }

            SKBitmap target;
            if (onMsgWindow)
            {
                target = frmMessage.picFace.NewImageIfNull().Image;
                permanent = false;
            }
            else if (onStatusWindow)
            {
                target = picUnitStatus.NewImageIfNull().Image;
            }
            else if (permanent)
            {
                InitBackgroundBufferIfInvalid();
                target = backgroundBuffer;
            }
            else
            {
                SaveScreen();
                target = mainBuffer;
            }

            var image = LoadPicture(fname, target, transparent && bgColor == Color.White);
            if (image == null)
            {
                return false;
            }

            var origWidth = image.Width;
            var origHeight = image.Height;
            SKBitmap drawBuffer;
            if (sw != 0 && (sw != origWidth || sh != origHeight))
            {
                // 画像の一部を切り出す
                if (sx == Constants.DEFAULT_LEVEL)
                {
                    sx = (origWidth - sw) / 2;
                }
                if (sy == Constants.DEFAULT_LEVEL)
                {
                    sy = (origHeight - sh) / 2;
                }
                drawBuffer = new SKBitmap(Math.Max(1, sw), Math.Max(1, sh), SKColorType.Bgra8888, SKAlphaType.Premul);
                using (var canvas = new SKCanvas(drawBuffer))
                {
                    canvas.Clear(SKColors.Transparent);
                    canvas.DrawBitmap(image, SKRect.Create(sx, sy, sw, sh), SKRect.Create(0, 0, sw, sh));
                }
                origWidth = sw;
                origHeight = sh;
            }
            else
            {
                drawBuffer = ImageFilters.Copy(image);
            }

            try
            {
                if (transparent && bgColor != Color.White)
                {
                    var t = ImageFilters.MakeTransparent(drawBuffer, ToSKColor(bgColor));
                    drawBuffer.Dispose();
                    drawBuffer = t;
                }

                // 部分表示（指定外の部分を描画色で塗りつぶす）
                if (topPart || bottomPart || leftPart || rightPart || tleftPart || trightPart || bleftPart || brightPart)
                {
                    using (var canvas = new SKCanvas(drawBuffer))
                    using (var paint = new SKPaint { Color = ToSKColor(CurrentPaintColor), IsAntialias = false })
                    {
                        float w = drawBuffer.Width, h = origHeight;
                        if (topPart) canvas.DrawRect(0, h / 2f, w, h / 2f, paint);
                        if (bottomPart) canvas.DrawRect(0, 0, w, h / 2f, paint);
                        if (leftPart) canvas.DrawRect(w / 2f, 0, w / 2f, h, paint);
                        if (rightPart) canvas.DrawRect(0, 0, w / 2f, h, paint);
                        if (tleftPart) FillTriangle(canvas, paint, origWidth - 1, 0, origWidth - 1, origHeight - 1, 0, origHeight - 1);
                        if (trightPart) FillTriangle(canvas, paint, 0, 0, origWidth - 1, origHeight - 1, 0, origHeight - 1);
                        if (bleftPart) FillTriangle(canvas, paint, 0, 0, origWidth - 1, 0, origWidth - 1, origHeight - 1);
                        if (brightPart) FillTriangle(canvas, paint, 0, 0, origWidth - 1, 0, 0, origHeight - 1);
                    }
                }

                if (isMonotone) drawBuffer = ImageFilters.ApplyMonotone(drawBuffer);
                if (isSepia) drawBuffer = ImageFilters.ApplySepia(drawBuffer);
                if (isSunset) drawBuffer = ImageFilters.ApplySunset(drawBuffer);
                if (isWater) drawBuffer = ImageFilters.ApplyWater(drawBuffer);
                if (isSil) drawBuffer = ImageFilters.ApplySilhouette(drawBuffer);
                if (negpos) drawBuffer = ImageFilters.ApplyNegPos(drawBuffer);
                if (isColorFilter)
                {
                    drawBuffer = ImageFilters.ApplyColorFilter(drawBuffer, fcolor, transPar < 0f ? 0.5f : transPar);
                }
                for (var i = 0; i < brightCount; i++) drawBuffer = ImageFilters.ApplyBright(drawBuffer);
                for (var i = 0; i < darkCount; i++) drawBuffer = ImageFilters.ApplyDark(drawBuffer);
                if (hrev || vrev) drawBuffer = ImageFilters.Flip(drawBuffer, hrev, vrev);
                if (angle != 0)
                {
                    drawBuffer = ImageFilters.Rotate(drawBuffer, angle, transparent ? SKColors.Transparent : ToSKColor(BGColor));
                }

                if (dw == Constants.DEFAULT_LEVEL) dw = origWidth;
                if (dh == Constants.DEFAULT_LEVEL) dh = origHeight;

                if (permanent)
                {
                    if (dx == Constants.DEFAULT_LEVEL) dx = (MapPWidth - dw) / 2;
                    if (dy == Constants.DEFAULT_LEVEL)
                    {
                        dy = Map.IsStatusView ? (ScreenLayout.CellPx * 15 - dh) / 2 : (MapPHeight - dh) / 2;
                    }
                }
                else if (Strings.InStr(fname, "EFFECT_") > 0
                    || Strings.InStr(fname, @"スペシャルパワー\") > 0
                    || Strings.InStr(fname, @"精神コマンド\") > 0)
                {
                    if (dx == Constants.DEFAULT_LEVEL)
                    {
                        dx = (MainPWidth - dw) / 2;
                        if (MainWidth % 2 == 0) dx -= 16;
                    }
                    if (dy == Constants.DEFAULT_LEVEL)
                    {
                        dy = (MainPHeight - dh) / 2;
                        if (MainHeight % 2 == 0) dy -= 16;
                    }
                }
                else
                {
                    if (dx == Constants.DEFAULT_LEVEL) dx = (MainPWidth - dw) / 2;
                    if (dy == Constants.DEFAULT_LEVEL) dy = (MainPHeight - dh) / 2;
                }

                if (dx >= target.Width || dy >= target.Height || dx + dw <= 0 || dy + dh <= 0 || dw <= 0 || dh <= 0)
                {
                    return true;
                }

                var dest = SKRect.Create(dx, dy, dw, dh);
                using (var canvas = new SKCanvas(target))
                {
                    canvas.DrawBitmap(drawBuffer, dest);
                }
                if (permanent)
                {
                    Map.IsMapDirty = true;
                }
                else if (!onMsgWindow && !onStatusWindow)
                {
                    IsPictureDrawn = true;
                    IsPictureVisible = true;
                    IsCursorVisible = false;
                    if (keepPicture)
                    {
                        using (var canvas = new SKCanvas(mainBufferBack))
                        {
                            canvas.DrawBitmap(drawBuffer, dest);
                        }
                    }
                }
                return true;
            }
            finally
            {
                drawBuffer.Dispose();
            }
        }

        private static void FillTriangle(SKCanvas canvas, SKPaint paint, float x1, float y1, float x2, float y2, float x3, float y3)
        {
            using (var path = new SKPath())
            {
                path.MoveTo(x1, y1);
                path.LineTo(x2, y2);
                path.LineTo(x3, y3);
                path.Close();
                canvas.DrawPath(path, paint);
            }
        }

        /// <summary>
        /// 描画する画像を取得する。black.bmp / white.bmp や陣営枠は特別扱いする。
        /// 返す画像はキャッシュと共有するため変更しないこと。
        /// </summary>
        private SKBitmap LoadPicture(string fname, SKBitmap target, bool whiteTransparent)
        {
            switch (Strings.LCase(fname) ?? "")
            {
                case "black.bmp":
                case @"event\black.bmp":
                    return SolidImage(target, SKColors.Black);
                case "white.bmp":
                case @"event\white.bmp":
                    return SolidImage(target, SKColors.White);
                case @"common\effect_tile(ally).bmp":
                case @"anime\common\effect_tile(ally).bmp":
                    return UnitFrames.Ally;
                case @"common\effect_tile(enemy).bmp":
                case @"anime\common\effect_tile(enemy).bmp":
                    return UnitFrames.Enemy;
                case @"common\effect_tile(neutral).bmp":
                case @"anime\common\effect_tile(neutral).bmp":
                    return UnitFrames.Neutral;
            }
            return whiteTransparent ? Images.GetTransparent(fname) : Images.Get(fname);
        }

        private readonly System.Collections.Generic.Dictionary<(int, int, uint), SKBitmap> solidImages
            = new System.Collections.Generic.Dictionary<(int, int, uint), SKBitmap>();

        private SKBitmap SolidImage(SKBitmap target, SKColor color)
        {
            var key = (target.Width, target.Height, (uint)color);
            if (!solidImages.TryGetValue(key, out var image))
            {
                image = new SKBitmap(target.Width, target.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
                image.Erase(color);
                solidImages[key] = image;
            }
            return image;
        }

        #endregion

        #region 文字列

        public void ResetDrawString()
        {
            CurrentPaintFont = DefaultPaintFont;
            CurrentPaintColor = Color.White;
        }

        public void SetDrawString(DrawStringMode mode)
        {
            if (mode == DrawStringMode.Status)
            {
                CurrentPaintFont = StatusPaintFont;
                CurrentPaintColor = Color.White;
            }
            else
            {
                ResetDrawString();
            }
        }

        public void SetDrawFont(DrawFontOption option)
        {
            CurrentPaintFont = new PaintFont
            {
                Name = option.FontFamily,
                Size = option.Size,
                Bold = option.Bold,
                Italic = option.Italic,
            };
            CurrentPaintColor = option.Color;
        }

        public void UpdateBaseX(int newX)
        {
            currentDrawStringPoint.X = newX;
        }

        public void UpdateBaseY(int newY)
        {
            currentDrawStringPoint.Y = newY;
        }

        /// <summary>
        /// GDI+ の DrawString に合わせ、y を文字の上端として描く。
        /// </summary>
        private static void DrawTextTop(SKCanvas canvas, string text, float x, float y, SKFont font, SKPaint paint)
        {
            canvas.DrawText(text ?? "", x, y + Compat.SkiaFonts.Ascent(font), SKTextAlign.Left, font, paint);
        }

        private static SKSize MeasureText(string text, SKFont font)
        {
            return new SKSize(font.MeasureText(text ?? ""), Compat.SkiaFonts.LineHeight(font));
        }

        public void DrawString(string msg, int X, int Y, bool without_cr = false)
        {
            SKBitmap target;
            if (PermanentStringMode)
            {
                InitBackgroundBufferIfInvalid();
                target = backgroundBuffer;
            }
            else
            {
                target = mainBuffer;
                SaveScreen();
            }

            using (var font = CreateFont(CurrentPaintFont))
            using (var paint = new SKPaint { Color = ToSKColor(CurrentPaintColor), IsAntialias = true })
            {
                var prevX = currentDrawStringPoint.X;
                var tx = currentDrawStringPoint.X;
                var ty = currentDrawStringPoint.Y;
                var size = MeasureText(msg, font);
                if (HCentering)
                {
                    tx = (target.Width - size.Width) / 2;
                }
                else if (X != Constants.DEFAULT_LEVEL)
                {
                    tx = X;
                }
                if (VCentering)
                {
                    ty = (target.Height - size.Height) / 2;
                }
                else if (Y != Constants.DEFAULT_LEVEL)
                {
                    ty = Y;
                }

                using (var canvas = new SKCanvas(target))
                {
                    DrawTextTop(canvas, msg, tx, ty, font, paint);
                }
                if (PermanentStringMode)
                {
                    Map.IsMapDirty = true;
                }
                if (KeepStringMode)
                {
                    using (var canvas = new SKCanvas(mainBufferBack))
                    {
                        DrawTextTop(canvas, msg, tx, ty, font, paint);
                    }
                }
                currentDrawStringPoint = new SKPoint(
                    X != Constants.DEFAULT_LEVEL ? X : prevX,
                    without_cr ? ty : ty + Compat.SkiaFonts.LineHeight(font));
                if (!PermanentStringMode)
                {
                    // 元の SRC は Print で描くため、次の DoEvents で画面に出る
                    InvalidateMain();
                    IsPictureVisible = true;
                    PaintedAreaX1 = 0;
                    PaintedAreaY1 = 0;
                    PaintedAreaX2 = MainPWidth - 1;
                    PaintedAreaY2 = MainPHeight - 1;
                }
            }
        }

        public void DrawSysString(int X, int Y, string msg, bool without_refresh = false)
        {
            if (X < MapX - MainWidth / 2 || MapX + MainWidth / 2 < X || Y < MapY - MainHeight / 2 || MapY + MainHeight / 2 < Y)
            {
                return;
            }
            SaveScreen();
            using (var font = CreateFont(src.BattleAnimation ? BattleAnimePaintFont : SysPaintFont))
            using (var paint = new SKPaint { IsAntialias = true })
            using (var canvas = new SKCanvas(mainBuffer))
            {
                var size = MeasureText(msg, font);
                var tx = MapToPixelX(X) + (ScreenLayout.CellPx - size.Width) / 2 - 1;
                var ty = MapToPixelY(Y + 1) - size.Height;
                paint.Color = SKColors.White;
                canvas.DrawRect(tx, ty, size.Width, size.Height, paint);
                paint.Color = SKColors.Black;
                DrawTextTop(canvas, msg, tx, ty, font, paint);
            }
            if (!without_refresh)
            {
                UpdateScreen();
            }
            PaintedAreaX1 = Math.Min(PaintedAreaX1, MapToPixelX(X) - 4);
            PaintedAreaY1 = Math.Max(PaintedAreaY1, MapToPixelY(Y) + 16);
            PaintedAreaX2 = Math.Min(PaintedAreaX2, MapToPixelX(X) + 36);
            PaintedAreaY2 = Math.Max(PaintedAreaY2, MapToPixelY(Y) + 32);
        }

        public SizeF MeasureString(string msg)
        {
            using (var font = CreateFont(CurrentPaintFont))
            {
                var size = MeasureText(msg, font);
                return new SizeF(size.Width, size.Height);
            }
        }

        #endregion

        #region 退避・消去

        public void SaveScreen()
        {
            if (!ScreenIsSaved)
            {
                CopyBitmap(mainBuffer, mainBufferBack);
                ScreenIsSaved = true;
            }
        }

        public void ClearPicture()
        {
            if (!ScreenIsSaved)
            {
                return;
            }
            IsPictureVisible = false;
            IsCursorVisible = false;
            CopyBitmap(mainBufferBack, mainBuffer);
        }

        public void ClearPicture2(int x1, int y1, int x2, int y2)
        {
            if (!ScreenIsSaved)
            {
                return;
            }
            using (var canvas = new SKCanvas(mainBuffer))
            {
                // 元の SRC（GUI.bas ClearPicture2）と同じく x2, y2 を含む
                var rect = SKRect.Create(x1, y1, x2 - x1 + 1, y2 - y1 + 1);
                canvas.DrawBitmap(mainBufferBack, rect, rect);
            }
        }

        #endregion

        #region 図形（IGUIScrean）

        private SKBitmap[] TargetBuffers(ScreanDrawOption option)
        {
            switch (option.DrawOption)
            {
                case ScreanDrawMode.Background:
                    InitBackgroundBufferIfInvalid();
                    return new[] { backgroundBuffer };
                case ScreanDrawMode.Preserve:
                    return new[] { mainBuffer, mainBufferBack };
                default:
                    return new[] { mainBuffer };
            }
        }

        private void DrawShape(ScreanDrawOption option, Action<SKCanvas, SKPaint, SKPaint> draw)
        {
            if (option.DrawOption != ScreanDrawMode.Background)
            {
                // 元の SRC は Line・Circle・PSet などのメソッドで描くため、次の DoEvents で画面に出る
                InvalidateMain();
                IsPictureVisible = true;
            }
            using (var pen = new SKPaint { Color = ToSKColor(option.ForeColor), Style = SKPaintStyle.Stroke, StrokeWidth = Math.Max(1, option.DrawWidth), IsAntialias = false })
            using (var brush = option.FillStyle != FillStyle.VbFSTransparent
                ? new SKPaint { Color = ToSKColor(option.FillColor), Style = SKPaintStyle.Fill, IsAntialias = false }
                : null)
            {
                foreach (var buffer in TargetBuffers(option))
                {
                    using (var canvas = new SKCanvas(buffer))
                    {
                        draw(canvas, pen, brush);
                    }
                }
            }
        }

        private static SKRect CircleRect(int centerX, int centerY, int rad, float ovalRatio = 1f)
        {
            var rect = SKRect.Create(centerX - rad, centerY - rad, rad * 2, rad * 2);
            if (ovalRatio != 1f)
            {
                var shrink = (int)(rad - rad * ovalRatio);
                rect = SKRect.Create(rect.Left + shrink, rect.Top, rect.Width - shrink * 2, rect.Height);
            }
            return rect;
        }

        public void ArcCmd(ScreanDrawOption option, int x1, int y1, int rad, float start_angle, float end_angle)
        {
            DrawShape(option, (canvas, pen, brush) =>
            {
                var rect = CircleRect(x1, y1, rad);
                if (brush != null)
                {
                    canvas.DrawArc(rect, -start_angle, -(end_angle - start_angle), true, brush);
                }
                canvas.DrawArc(rect, -start_angle, -(end_angle - start_angle), false, pen);
            });
        }

        public void CircleCmd(ScreanDrawOption option, int x1, int y1, int rad)
        {
            OvalCmd(option, x1, y1, rad, 1f);
        }

        public void OvalCmd(ScreanDrawOption option, int x1, int y1, int rad, float oval_ratio)
        {
            DrawShape(option, (canvas, pen, brush) =>
            {
                var rect = CircleRect(x1, y1, rad, oval_ratio);
                if (brush != null)
                {
                    canvas.DrawOval(rect, brush);
                }
                canvas.DrawOval(rect, pen);
            });
        }

        public void LineCmd(ScreanDrawOption option, int x1, int y1, int x2, int y2)
        {
            DrawShape(option, (canvas, pen, brush) => canvas.DrawLine(x1, y1, x2, y2, pen));
        }

        public void BoxCmd(ScreanDrawOption option, int x1, int y1, int x2, int y2)
        {
            DrawShape(option, (canvas, pen, brush) =>
            {
                var rect = SKRect.Create(x1, y1, x2 - x1, y2 - y1);
                if (brush != null)
                {
                    canvas.DrawRect(rect, brush);
                }
                canvas.DrawRect(rect, pen);
            });
        }

        public void PSetCmd(ScreanDrawOption option, int x1, int y1)
        {
            DrawShape(option, (canvas, pen, brush) => canvas.DrawRect(SKRect.Create(x1, y1, 1, 1), pen));
        }

        public void PolygonCmd(ScreanDrawOption option, Point[] points)
        {
            if (points == null || points.Length == 0)
            {
                return;
            }
            DrawShape(option, (canvas, pen, brush) =>
            {
                using (var path = new SKPath())
                {
                    path.MoveTo(points[0].X, points[0].Y);
                    for (var i = 1; i < points.Length; i++)
                    {
                        path.LineTo(points[i].X, points[i].Y);
                    }
                    path.Close();
                    if (brush != null)
                    {
                        canvas.DrawPath(path, brush);
                    }
                    canvas.DrawPath(path, pen);
                }
            });
        }

        #endregion
    }
}
