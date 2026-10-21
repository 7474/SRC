using SkiaSharp;
using SRCCore;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Units;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SRCRender
{
    // マップ表示。WinForms 版（Forms/Main.guimap.cs、Forms/Main.gui.cs）と同じく、
    // 表示用（mainBuffer）・退避用（mainBufferBack）・マップ全体の背景（backgroundBuffer）の3枚で管理し、
    // 画面へは displayBuffer を表示する。
    public partial class ScreenGUI
    {
        private static readonly SKColor MapMaskColor = new SKColor(100, 100, 100, 127);
        private static readonly SKColor UnitMaskColor = new SKColor(100, 100, 100, 127);
        private static readonly SKColor MapLineColor = new SKColor(100, 100, 100);

        private SKBitmap mainBuffer = NewBuffer(ScreenLayout.MapWidthPx, ScreenLayout.MapHeightPx);
        private SKBitmap mainBufferBack = NewBuffer(ScreenLayout.MapWidthPx, ScreenLayout.MapHeightPx);
        private SKBitmap displayBuffer = NewBuffer(ScreenLayout.MapWidthPx, ScreenLayout.MapHeightPx);
        private SKBitmap backgroundBuffer;

        private static SKBitmap NewBuffer(int width, int height)
        {
            var bitmap = new SKBitmap(Math.Max(1, width), Math.Max(1, height), SKColorType.Bgra8888, SKAlphaType.Premul);
            bitmap.Erase(SKColors.Black);
            return bitmap;
        }

        private static void CopyBitmap(SKBitmap from, SKBitmap to)
        {
            using (var canvas = new SKCanvas(to))
            {
                canvas.Clear(SKColors.Black);
                canvas.DrawBitmapShared(from, 0, 0);
            }
        }

        private void DisposeMapBuffers()
        {
            mainBuffer.Dispose();
            mainBufferBack.Dispose();
            displayBuffer.Dispose();
            backgroundBuffer?.Dispose();
        }

        #region IGUIMap

        public void InitMapSize(int w, int h)
        {
        }

        public void SetMapSize(int w, int h)
        {
            InitBackgroundBuffer();
        }

        #endregion

        private void InitBackgroundBuffer()
        {
            backgroundBuffer?.Dispose();
            backgroundBuffer = NewBuffer(MapPWidth, MapPHeight);
        }

        private void InitBackgroundBufferIfInvalid()
        {
            if (backgroundBuffer == null)
            {
                InitBackgroundBuffer();
            }
        }

        /// <summary>
        /// 表示用バッファを画面へ反映する。
        /// </summary>
        public void UpdateScreen()
        {
            lock (renderLock)
            {
                CopyBitmap(mainBuffer, displayBuffer);
            }
            isMainInvalidated = false;
            Present();
        }

        public void ClearScrean()
        {
            State.ShowMap = false;
            mainBuffer.Erase(SKColors.Black);
            UpdateScreen();
            ScreenIsSaved = true;
        }

        public void SetupBackground(string draw_mode = "", string draw_option = "", Color filter_color = default, double filter_trans_par = 0d)
        {
            Map.MapDrawMode = draw_mode;
            Map.MapDrawFilterColor = filter_color;
            Map.MapDrawFilterTransPercent = filter_trans_par;
            Map.IsMapDirty = false;
            IsPictureVisible = false;
            IsCursorVisible = false;

            InitBackgroundBufferIfInvalid();
            if (draw_option == "ステータス")
            {
                backgroundBuffer.Erase(SKColors.Black);
                return;
            }
            MapX = (MainWidth / 2 + 1);
            MapY = (MainHeight / 2 + 1);

            var built = renderer.BuildBackground(src, Images);
            built = ImageFilters.ApplyDrawMode(built, draw_mode, filter_color, filter_trans_par);
            if (src.ShowSquareLine)
            {
                using (var canvas = new SKCanvas(built))
                using (var paint = new SKPaint { Color = MapLineColor, Style = SKPaintStyle.Stroke })
                {
                    canvas.DrawRect(0, 0, MapPWidth, MapPHeight, paint);
                    for (var x = 1; x <= Map.MapWidth - 1; x++)
                    {
                        canvas.DrawLine(x * ScreenLayout.CellPx, 0, x * ScreenLayout.CellPx, MapPHeight, paint);
                    }
                    for (var y = 1; y <= Map.MapHeight - 1; y++)
                    {
                        canvas.DrawLine(0, y * ScreenLayout.CellPx, MapPWidth, y * ScreenLayout.CellPx, paint);
                    }
                }
            }
            backgroundBuffer?.Dispose();
            backgroundBuffer = built;

            if (!Map.IsStatusView && string.IsNullOrEmpty(draw_option))
            {
                RefreshScreen();
            }
        }

        public void RedrawScreen(bool late_refresh = false)
        {
            ScreenIsMasked = false;
            RefreshScreen(false, late_refresh);
        }

        public void MaskScreen()
        {
            ScreenIsMasked = true;
            RefreshScreen();
        }

        public void RefreshScreen(bool without_refresh = false, bool delay_refresh = false)
        {
            if (Map.MapWidth == 1)
            {
                return;
            }
            MapX = Math.Max(1, Math.Min(Map.MapWidth, MapX));
            MapY = Math.Max(1, Math.Min(Map.MapHeight, MapY));
            if (!without_refresh)
            {
                IsPictureVisible = false;
                IsCursorVisible = false;
            }
            if (backgroundBuffer == null && Map.MapData != null && Map.MapWidth > 0)
            {
                backgroundBuffer = renderer.BuildBackground(src, Images);
            }

            State.ShowMap = true;
            using (var canvas = new SKCanvas(mainBuffer))
            using (var paint = new SKPaint())
            {
                canvas.Clear(SKColors.Black);
                var mx = MapX - (MainWidth + 1) / 2 + 1;
                var my = MapY - (MainHeight + 1) / 2 + 1;
                if (backgroundBuffer != null)
                {
                    // 見えているマスの背景は、背景の上でも並びが同じなので一度に写す
                    var x0 = Math.Max(1, mx);
                    var y0 = Math.Max(1, my);
                    var x1 = Math.Min(Map.MapWidth, mx + MainWidth - 1);
                    var y1 = Math.Min(Map.MapHeight, my + MainHeight - 1);
                    if (x0 <= x1 && y0 <= y1)
                    {
                        var source = SKRect.Create(ScreenLayout.CellPx * (x0 - 1), ScreenLayout.CellPx * (y0 - 1),
                            ScreenLayout.CellPx * (x1 - x0 + 1), ScreenLayout.CellPx * (y1 - y0 + 1));
                        var dest = SKRect.Create(ScreenLayout.CellPx * (x0 - mx), ScreenLayout.CellPx * (y0 - my), source.Width, source.Height);
                        canvas.DrawBitmapShared(backgroundBuffer, source, dest);
                    }
                }
                for (var i = 0; i < MainWidth; i++)
                {
                    for (var j = 0; j < MainHeight; j++)
                    {
                        var x = mx + i;
                        var y = my + j;
                        if (x < 1 || x > Map.MapWidth || y < 1 || y > Map.MapHeight)
                        {
                            continue;
                        }
                        var dest = SKRect.Create(ScreenLayout.CellPx * i, ScreenLayout.CellPx * j, ScreenLayout.CellPx, ScreenLayout.CellPx);
                        var cell = Map.MapData[x, y];
                        var u = Map.MapDataForUnit[x, y];
                        if (u != null)
                        {
                            DrawUnit(canvas, cell, u, dest);
                        }
                        if (ScreenIsMasked && Map.MaskData != null && Map.MaskData[x, y])
                        {
                            paint.Color = MapMaskColor;
                            canvas.DrawRect(dest, paint);
                        }
                    }
                }
            }
            ScreenIsSaved = false;
            if (!without_refresh && !delay_refresh)
            {
                UpdateScreen();
            }
        }

        public void TransionScrean(TransionPattern pattern, Color fillColor, int frame, int frameMillis)
        {
            var start = GeneralLib.timeGetTime();
            using (var copy = ImageFilters.Copy(mainBuffer))
            {
                for (var i = 0; i <= frame; i++)
                {
                    if (IsRButtonPressed())
                    {
                        i = frame + 1;
                    }
                    var opacity = frame <= 0 ? 0xff
                        : pattern == TransionPattern.FadeIn ? 0xff * (frame - i) / frame : 0xff * i / frame;
                    opacity = Math.Max(0, Math.Min(0xff, opacity));
                    using (var canvas = new SKCanvas(mainBuffer))
                    using (var paint = new SKPaint { Color = new SKColor(fillColor.R, fillColor.G, fillColor.B, (byte)opacity) })
                    {
                        canvas.Clear(SKColors.Black);
                        canvas.DrawBitmap(copy, 0, 0);
                        canvas.DrawRect(0, 0, mainBuffer.Width, mainBuffer.Height, paint);
                    }
                    UpdateScreen();
                    if (frame != i)
                    {
                        while (GeneralLib.timeGetTime() < start + frameMillis * (i + 1))
                        {
                            Sleep(1);
                        }
                    }
                }
                CopyBitmap(copy, mainBuffer);
            }
        }

        public void Center(int new_x, int new_y)
        {
            if (Map.IsStatusView)
            {
                return;
            }
            MapX = Math.Max(1, Math.Min(Map.MapWidth, new_x));
            MapY = Math.Max(1, Math.Min(Map.MapHeight, new_y));
        }

        public int MapToPixelX(int X)
        {
            return ScreenLayout.CellPx * ((MainWidth + 1) / 2 - 1 - (MapX - X));
        }

        public int MapToPixelY(int Y)
        {
            return ScreenLayout.CellPx * ((MainHeight + 1) / 2 - 1 - (MapY - Y));
        }

        public int PixelToMapX(int X)
        {
            X = Math.Max(0, Math.Min(MainPWidth - 1, X));
            return X / ScreenLayout.CellPx + 1 + MapX - (MainWidth + 1) / 2;
        }

        public int PixelToMapY(int Y)
        {
            Y = Math.Max(0, Math.Min(MainPHeight - 1, Y));
            return Y / ScreenLayout.CellPx + 1 + MapY - (MainHeight + 1) / 2;
        }

        private bool IsInsideWindow(int x, int y)
        {
            // WinForms 版と同じ判定（実質的に常に true）
            return x >= MapX - (MainWidth + 1) / 2
                || MapX + (MainWidth + 1) / 2 >= x
                || y >= MapY - (MainHeight + 1) / 2
                || MapY + (MainHeight + 1) / 2 >= y;
        }

        #region ユニット

        /// <summary>
        /// ユニットを1マス分描画する（陣営枠、ユニット画像、描画モード、行動済み、地形の線）。
        /// </summary>
        private void DrawUnit(SKCanvas target, MapCell cell, Unit u, SKRect dest, bool useOrigColor = false)
        {
            using (var image = CreateUnitImage(cell, u, useOrigColor))
            {
                target.DrawBitmap(image, dest);
            }
        }

        private SKBitmap CreateUnitImage(MapCell cell, Unit u, bool useOrigColor = false)
        {
            var px = ScreenLayout.CellPx;
            var image = new SKBitmap(px, px, SKColorType.Bgra8888, SKAlphaType.Premul);
            var rect = SKRect.Create(0, 0, px, px);
            var emitLight = Map.MapDrawMode == "夜" && !Map.MapDrawIsMapOnly && !useOrigColor && SafeFeature(u, "発光");

            SKBitmap unitImage = null;
            try
            {
                unitImage = Images.GetTransparent(u.CurrentForm().CurrentBitmap());
            }
            catch
            {
                // 画像の解決に失敗した場合は画像なしとして扱う
            }

            using (var canvas = new SKCanvas(image))
            {
                canvas.Clear(SKColors.Transparent);
                var frame = UnitFrames.ForParty(u.Party0);
                if (frame != null)
                {
                    canvas.DrawBitmap(frame, rect);
                }
                if (unitImage != null && !emitLight)
                {
                    canvas.DrawBitmap(unitImage, rect);
                }
                else if (unitImage == null)
                {
                    renderer.DrawUnitInitial(canvas, u.Nickname0 ?? u.Name, rect);
                }
            }

            if (!useOrigColor && !Map.MapDrawIsMapOnly)
            {
                image = ImageFilters.ApplyDrawMode(image, Map.MapDrawMode, Map.MapDrawFilterColor, Map.MapDrawFilterTransPercent);
                if (emitLight && unitImage != null)
                {
                    using (var canvas = new SKCanvas(image))
                    {
                        canvas.DrawBitmap(unitImage, rect);
                    }
                }
            }

            using (var canvas = new SKCanvas(image))
            using (var paint = new SKPaint { Color = UnitMaskColor })
            {
                if (u.Action <= 0 && !SafeFeature(u, "地形ユニット"))
                {
                    canvas.DrawRect(rect, paint);
                }
                paint.Color = SKColors.Black;
                paint.Style = SKPaintStyle.Stroke;
                switch (u.Area ?? "")
                {
                    case "空中":
                        canvas.DrawLine(0, 28.5f, 31, 28.5f, paint);
                        break;
                    case "水中":
                        canvas.DrawLine(0, 3.5f, 31, 3.5f, paint);
                        break;
                    case "地中":
                        canvas.DrawLine(0, 28.5f, 31, 28.5f, paint);
                        canvas.DrawLine(0, 3.5f, 31, 3.5f, paint);
                        break;
                    case "宇宙":
                        if (cell?.Terrain?.Class == "月面")
                        {
                            canvas.DrawLine(0, 28.5f, 31, 28.5f, paint);
                        }
                        break;
                }
            }
            return image;
        }

        private static bool SafeFeature(Unit u, string feature)
        {
            try
            {
                return u.IsFeatureAvailable(feature);
            }
            catch
            {
                return false;
            }
        }

        public void PaintUnitBitmap(Unit u, string smode = "")
        {
            if (!IsInsideWindow(u.x, u.y))
            {
                return;
            }
            var buffer = smode == "リフレッシュ無し" && ScreenIsSaved ? mainBufferBack : mainBuffer;
            using (var canvas = new SKCanvas(buffer))
            {
                var dest = SKRect.Create(MapToPixelX(u.x), MapToPixelY(u.y), ScreenLayout.CellPx, ScreenLayout.CellPx);
                DrawUnit(canvas, Map.MapData[u.x, u.y], u, dest);
            }
            if (smode != "リフレッシュ無し")
            {
                UpdateScreen();
            }
        }

        public void EraseUnitBitmap(int X, int Y, bool do_refresh = true)
        {
            if (!IsInsideWindow(X, Y))
            {
                return;
            }
            if (IsPictureVisible)
            {
                return;
            }
            SaveScreen();
            EraseMapPoint(X, Y);
            if (do_refresh)
            {
                UpdateScreen();
            }
        }

        private void EraseMapPoint(int X, int Y)
        {
            if (backgroundBuffer == null)
            {
                return;
            }
            var dest = SKRect.Create(MapToPixelX(X), MapToPixelY(Y), ScreenLayout.CellPx, ScreenLayout.CellPx);
            var source = SKRect.Create((X - 1) * ScreenLayout.CellPx, (Y - 1) * ScreenLayout.CellPx, ScreenLayout.CellPx, ScreenLayout.CellPx);
            foreach (var buffer in new[] { mainBuffer, mainBufferBack })
            {
                using (var canvas = new SKCanvas(buffer))
                {
                    canvas.DrawBitmapShared(backgroundBuffer, source, dest);
                }
            }
        }

        /// <summary>
        /// 移動アニメーションの1コマ。表示用バッファの上にユニット画像を重ねて表示する。
        /// </summary>
        private void PresentMovingUnit(SKBitmap unitImage, int xx, int yy)
        {
            lock (renderLock)
            {
                using (var canvas = new SKCanvas(displayBuffer))
                {
                    canvas.Clear(SKColors.Black);
                    canvas.DrawBitmap(mainBuffer, 0, 0);
                    canvas.DrawBitmap(unitImage, xx, yy);
                }
            }
            Present();
        }

        /// <summary>
        /// 移動アニメーションの最後のコマを画面のバッファに残す。
        /// 元の SRC は picMain(0) に描いたまま終わるので、呼び出し元が描き直さなくてもユニットが見える
        /// （表示用バッファだけに描くと、次に画面を更新したときに消える）。
        /// </summary>
        private void LeaveMovedUnit(SKBitmap unitImage, int xx, int yy)
        {
            using (var canvas = new SKCanvas(mainBuffer))
            {
                canvas.DrawBitmap(unitImage, xx, yy);
            }
        }

        private void WaitFrame(ref int startTime, int waitTime)
        {
            if (waitTime <= 0)
            {
                return;
            }
            int now;
            do
            {
                Sleep(1);
                now = GeneralLib.timeGetTime();
            }
            while (startTime + waitTime > now);
            startTime = now;
        }

        public void MoveUnitBitmap(Unit u, int x1, int y1, int x2, int y2, int wait_time0, int division = 2)
        {
            var waitTime = wait_time0 / division;
            var startTime = GeneralLib.timeGetTime();
            SaveScreen();
            using (var unitImage = CreateUnitImage(Map.CellAtPoint(u.x, u.y), u))
            {
                var xx = MapToPixelX(x1);
                var yy = MapToPixelY(y1);
                if (ReferenceEquals(u, Map.MapDataForUnit[x1, y1]))
                {
                    EraseUnitBitmap(x1, y1, false);
                }

                // 長い方の軸を先に移動し、次に短い方の軸を移動する
                var horizontalFirst = Math.Abs(x2 - x1) > Math.Abs(y2 - y1);
                var steps = new[]
                {
                    (horizontal: horizontalFirst, count: division * Math.Max(Math.Abs(x2 - x1), Math.Abs(y2 - y1))),
                    (horizontal: !horizontalFirst, count: division * Math.Min(Math.Abs(x2 - x1), Math.Abs(y2 - y1))),
                };
                foreach (var (horizontal, count) in steps)
                {
                    var vx = horizontal ? (x2 > x1 ? 1 : -1) : 0;
                    var vy = horizontal ? 0 : (y2 > y1 ? 1 : -1);
                    for (var i = 1; i <= count; i++)
                    {
                        xx += ScreenLayout.CellPx * vx / division;
                        yy += ScreenLayout.CellPx * vy / division;
                        PresentMovingUnit(unitImage, xx, yy);
                        WaitFrame(ref startTime, waitTime);
                    }
                }
                LeaveMovedUnit(unitImage, xx, yy);
            }
            ScreenIsSaved = false;
        }

        public void MoveUnitBitmap2(Unit u, int wait_time0, int division = 2)
        {
            var waitTime = wait_time0 / division;
            var startTime = GeneralLib.timeGetTime();
            SaveScreen();
            using (var unitImage = CreateUnitImage(Map.CellAtPoint(u.x, u.y), u))
            {
                Map.SearchMoveRoute(u.x, u.y, out IList<int> routeX, out IList<int> routeY);
                if (routeX.Count == 0)
                {
                    return;
                }
                var xx = MapToPixelX(routeX.Last());
                var yy = MapToPixelY(routeY.Last());
                for (var i = routeX.Count - 2; i >= 0; i--)
                {
                    var vx = MapToPixelX(routeX[i]) - xx;
                    var vy = MapToPixelY(routeY[i]) - yy;
                    for (var j = 0; j < division; j++)
                    {
                        xx += vx / division;
                        yy += vy / division;
                        PresentMovingUnit(unitImage, xx, yy);
                        WaitFrame(ref startTime, waitTime);
                    }
                }
                LeaveMovedUnit(unitImage, xx, yy);
            }
            ScreenIsSaved = false;
        }

        #endregion

        public void DisplayGlobalMap()
        {
            int mwidth, mheight;
            if (Map.MapWidth > Map.MapHeight)
            {
                mwidth = 300;
                mheight = 300 * Map.MapHeight / Map.MapWidth;
            }
            else
            {
                mheight = 300;
                mwidth = 300 * Map.MapWidth / Map.MapHeight;
            }
            using (var whole = NewBuffer(MapPWidth, MapPHeight))
            {
                using (var canvas = new SKCanvas(whole))
                {
                    if (backgroundBuffer != null)
                    {
                        canvas.DrawBitmap(backgroundBuffer, 0, 0);
                    }
                    for (var x = 1; x <= Map.MapWidth; x++)
                    {
                        for (var y = 1; y <= Map.MapHeight; y++)
                        {
                            var u = Map.MapDataForUnit[x, y];
                            if (u != null)
                            {
                                DrawUnit(canvas, Map.MapData[x, y], u,
                                    SKRect.Create(ScreenLayout.CellPx * (x - 1), ScreenLayout.CellPx * (y - 1), ScreenLayout.CellPx, ScreenLayout.CellPx));
                            }
                        }
                    }
                }
                lock (renderLock)
                {
                    using (var canvas = new SKCanvas(displayBuffer))
                    using (var image = SKImage.FromBitmap(whole))
                    {
                        canvas.Clear(SKColors.Black);
                        canvas.DrawImage(image,
                            SKRect.Create(0, 0, whole.Width, whole.Height),
                            SKRect.Create((MainPWidth - mwidth) / 2, (MainPHeight - mheight) / 2, mwidth, mheight),
                            new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear));
                    }
                }
            }
            Present();

            // 味方ユニット数、敵ユニット数のカウント
            var num = 0;
            var num2 = 0;
            foreach (Unit u in SRC.UList.Items)
            {
                if (u.Status == "出撃" || u.Status == "格納")
                {
                    if (u.Party0 == "味方" || u.Party0 == "ＮＰＣ")
                    {
                        num++;
                    }
                    else
                    {
                        num2++;
                    }
                }
            }

            // 各ユニット数の表示
            var prev_mode = AutoMessageMode;
            AutoMessageMode = false;
            OpenMessageForm(u1: null, u2: null);
            DisplayMessage("システム", "味方ユニット： " + num + ";" + "敵ユニット  ： " + num2, "");
            CloseMessageForm();
            AutoMessageMode = prev_mode;

            // 画面を元に戻す
            RefreshScreen();
        }
    }
}
