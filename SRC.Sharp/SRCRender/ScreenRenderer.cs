using SkiaSharp;
using SRCCore;
using SRCCore.Maps;
using SRCCore.Units;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCRender
{
    /// <summary>
    /// SRC の画面を SkiaSharp で描画する。UI フレームワークには依存しない。
    /// レイアウトの計算はヒットテストと共有する。
    /// </summary>
    public class ScreenRenderer : IDisposable
    {
        private const int MenuItemHeight = 20;
        private const int MenuMinWidth = 120;
        private const int DialogWidth = 420;
        private const int DialogMinWidth = 200;
        private const int DialogButtonWidth = 80;
        private const int DialogButtonHeight = 24;

        private static readonly SKColor BackColor = new SKColor(0x00, 0x00, 0x00);
        private static readonly SKColor PanelColor = new SKColor(0x10, 0x18, 0x30, 0xF0);
        private static readonly SKColor PanelBorderColor = new SKColor(0xC0, 0xC8, 0xE0);
        private static readonly SKColor TextColor = new SKColor(0xF0, 0xF0, 0xF0);
        private static readonly SKColor DisabledTextColor = new SKColor(0x80, 0x80, 0x80);
        private static readonly SKColor HighlightColor = new SKColor(0x40, 0x60, 0xC0);
        private static readonly SKColor MaskColor = new SKColor(0x00, 0x00, 0x00, 0x80);

        private readonly SKTypeface typeface;
        private readonly SKFont font;
        private readonly SKFont boldFont;
        private readonly SKFont largeFont;

        /// <summary>
        /// 既定で使うフォント。SRC のリストは等幅（ＭＳ 明朝）で桁を空白で揃えるため等幅フォントにする。
        /// 環境によらず同じ描画結果になるよう、あれば優先する。
        /// </summary>
        public static string PreferredFontFamily => Compat.SkiaFonts.FixedPitchFamily;

        public ScreenRenderer(SKTypeface typeface = null)
        {
            this.typeface = typeface ?? ResolveTypeface();
            font = new SKFont(this.typeface, 16);
            boldFont = new SKFont(this.typeface, 16) { Embolden = true };
            largeFont = new SKFont(this.typeface, 28) { Embolden = true };
        }

        /// <summary>描画に使っているフォント</summary>
        public SKTypeface Typeface => typeface;

        private static SKTypeface ResolveTypeface()
        {
            return Compat.SkiaFonts.ResolveTypeface("ＭＳ 明朝", SKFontStyle.Normal);
        }

        public void Dispose()
        {
            font.Dispose();
            boldFont.Dispose();
            largeFont.Dispose();
        }

        public SKSize MeasureText(string text)
        {
            var width = font.MeasureText(text ?? "");
            return new SKSize(width, font.Spacing);
        }

        #region 描画

        /// <summary>
        /// 画面を構成する画像。
        /// </summary>
        public struct ScreenLayers
        {
            public ScreenLayers(SKBitmap map, Compat.PictureBox[] status, Compat.MessageWindow message)
            {
                Map = map;
                Status = status;
                Message = message;
            }

            /// <summary>マップ表示領域（ScreenLayout.MapArea の大きさ）</summary>
            public SKBitmap Map { get; }
            /// <summary>ステータスウィンドウの部品（ScreenLayout.StatusArea からの相対座標）</summary>
            public Compat.PictureBox[] Status { get; }
            /// <summary>メッセージウィンドウ</summary>
            public Compat.MessageWindow Message { get; }
        }

        public void Render(SKCanvas canvas, ScreenState state, ScreenLayers layers)
        {
            canvas.Clear(BackColor);

            // メインウィンドウを隠している間（プロローグ・インターミッションなど）は、マップとステータスを描かない（黒いまま）
            if (layers.Map != null && state.MainFormVisible)
            {
                canvas.DrawBitmap(layers.Map, ScreenLayout.MapArea.Left, ScreenLayout.MapArea.Top);
                if (state.MapCursor is SKRectI cursor)
                {
                    DrawMapCursor(canvas, cursor);
                }
            }
            if (!string.IsNullOrEmpty(state.Splash))
            {
                // 元の SRC の frmNowLoading は画面の中央。メインウィンドウを表示していればマップの中央に出す
                DrawCenteredText(canvas, state.MainFormVisible ? ScreenLayout.MapArea : new SKRectI(0, 0, ScreenLayout.Width, ScreenLayout.Height), state.Splash, largeFont);
            }
            if (state.Title != null)
            {
                DrawTitle(canvas, state.Title);
            }
            if (state.MainFormVisible)
            {
                DrawStatus(canvas, layers.Status);
            }
            DrawMessageArea(canvas, layers.Message);
            if (!string.IsNullOrEmpty(state.Telop))
            {
                DrawTelop(canvas, state.Telop, state.MainFormVisible);
            }
            if (state.List != null)
            {
                DrawForm(canvas, state.List.Window);
            }
            if (state.Config != null)
            {
                DrawForm(canvas, state.Config);
            }
            if (state.Menu != null)
            {
                DrawMenu(canvas, state.Menu);
            }
            if (state.Dialog != null)
            {
                DrawDialog(canvas, state.Dialog);
            }
            if (state.KeyPointerTarget is SKRectI target)
            {
                DrawMapCursor(canvas, target);
            }
            if (state.KeyPointer is SKPointI pointer)
            {
                DrawKeyPointer(canvas, pointer);
            }
            if (state.Tooltip != null)
            {
                DrawTooltip(canvas, state.Tooltip);
            }
        }

        /// <summary>キー操作のポインタ（矢印のマウスカーソルの形）を描く</summary>
        private static void DrawKeyPointer(SKCanvas canvas, SKPointI p)
        {
            using (var path = new SKPath())
            using (var paint = new SKPaint { IsAntialias = true })
            {
                path.MoveTo(p.X, p.Y);
                path.LineTo(p.X, p.Y + 17);
                path.LineTo(p.X + 4, p.Y + 13);
                path.LineTo(p.X + 7, p.Y + 20);
                path.LineTo(p.X + 10, p.Y + 19);
                path.LineTo(p.X + 7, p.Y + 12);
                path.LineTo(p.X + 12, p.Y + 12);
                path.Close();
                paint.Color = SKColors.White;
                canvas.DrawPath(path, paint);
                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeWidth = 1;
                paint.Color = SKColors.Black;
                canvas.DrawPath(path, paint);
            }
        }

        /// <summary>
        /// マップ全体の背景（地形）を描画した画像を作る。
        /// </summary>
        public SKBitmap BuildBackground(SRC src, ImageCache images)
        {
            var map = src.Map;
            var bitmap = new SKBitmap(Math.Max(1, map.MapWidth * ScreenLayout.CellPx), Math.Max(1, map.MapHeight * ScreenLayout.CellPx));
            using (var canvas = new SKCanvas(bitmap))
            using (var paint = new SKPaint())
            {
                canvas.Clear(BackColor);
                for (var x = 1; x <= map.MapWidth; x++)
                {
                    for (var y = 1; y <= map.MapHeight; y++)
                    {
                        var cell = map.MapData[x, y];
                        var rect = SKRect.Create((x - 1) * ScreenLayout.CellPx, (y - 1) * ScreenLayout.CellPx, ScreenLayout.CellPx, ScreenLayout.CellPx);
                        var image = images.Get(SafeSearch(() => map.SearchTerrainImageFile(cell)));
                        if (image != null)
                        {
                            canvas.DrawBitmap(image, rect);
                        }
                        else
                        {
                            // 画像が無い場合は地形の種類の色で塗り、地形番号ごとに明度を変えて枠を付ける
                            paint.Style = SKPaintStyle.Fill;
                            paint.Color = Shade(TerrainColor(cell), (cell.TerrainType % 4) * 0.12f - 0.18f);
                            canvas.DrawRect(rect, paint);
                            paint.Style = SKPaintStyle.Stroke;
                            paint.Color = new SKColor(0, 0, 0, 0x40);
                            canvas.DrawRect(SKRect.Create(rect.Left + 0.5f, rect.Top + 0.5f, rect.Width - 1, rect.Height - 1), paint);
                            paint.Style = SKPaintStyle.Fill;
                        }
                        var layer = images.GetTransparent(SafeSearch(() => map.SearchLayerImageFile(cell)));
                        if (layer != null)
                        {
                            canvas.DrawBitmap(layer, rect);
                        }
                    }
                }
                if (src.ShowSquareLine)
                {
                    paint.Color = new SKColor(0, 0, 0, 0x60);
                    paint.Style = SKPaintStyle.Stroke;
                    for (var x = 1; x < map.MapWidth; x++)
                    {
                        canvas.DrawLine(x * ScreenLayout.CellPx, 0, x * ScreenLayout.CellPx, bitmap.Height, paint);
                    }
                    for (var y = 1; y < map.MapHeight; y++)
                    {
                        canvas.DrawLine(0, y * ScreenLayout.CellPx, bitmap.Width, y * ScreenLayout.CellPx, paint);
                    }
                }
            }
            return bitmap;
        }

        private static string SafeSearch(Func<string> search)
        {
            try
            {
                return search();
            }
            catch
            {
                return null;
            }
        }

        private static SKColor Shade(SKColor color, float amount)
        {
            byte Adjust(byte c) => (byte)Math.Max(0, Math.Min(255, c + c * amount));
            return new SKColor(Adjust(color.Red), Adjust(color.Green), Adjust(color.Blue), color.Alpha);
        }

        private static SKColor TerrainColor(MapCell cell)
        {
            switch (cell?.Terrain?.Class ?? "")
            {
                case "陸":
                    return new SKColor(0x58, 0x90, 0x48);
                case "屋内":
                    return new SKColor(0x90, 0x78, 0x58);
                case "月面":
                    return new SKColor(0x90, 0x90, 0x90);
                case "水":
                case "深水":
                    return new SKColor(0x30, 0x60, 0xB0);
                case "空":
                    return new SKColor(0x88, 0xB8, 0xE8);
                case "宇宙":
                    return new SKColor(0x10, 0x10, 0x28);
                default:
                    return new SKColor(0x40, 0x40, 0x40);
            }
        }

        /// <summary>
        /// ユニット画像が無い場合に、名前の頭文字を表示する。
        /// </summary>
        public void DrawUnitInitial(SKCanvas canvas, string name, SKRect dest)
        {
            name = string.IsNullOrEmpty(name) ? "?" : name;
            using (var paint = new SKPaint { Color = TextColor, IsAntialias = true })
            {
                canvas.DrawText(name.Substring(0, 1), dest.MidX, dest.MidY + font.Size / 2 - 2, SKTextAlign.Center, boldFont, paint);
            }
        }

        private static readonly SKColor FormGray = new SKColor(192, 192, 192);

        private void DrawStatus(SKCanvas canvas, Compat.PictureBox[] pictures)
        {
            using (var paint = new SKPaint { Color = FormGray })
            {
                canvas.DrawRect(ScreenLayout.StatusArea, paint);
                if (pictures == null)
                {
                    return;
                }
                canvas.Save();
                canvas.Translate(ScreenLayout.StatusArea.Left, ScreenLayout.StatusArea.Top);
                foreach (var picture in pictures)
                {
                    DrawPictureBox(canvas, picture, paint);
                }
                canvas.Restore();
            }
        }

        private static void DrawPictureBox(SKCanvas canvas, Compat.PictureBox picture, SKPaint paint)
        {
            if (!picture.Visible)
            {
                return;
            }
            paint.Color = Compat.Graphics.ToSKColor(picture.BackColor);
            canvas.DrawRect(picture.Left, picture.Top, picture.Width, picture.Height, paint);
            if (picture.Image == null)
            {
                return;
            }
            if (picture.StretchImage)
            {
                canvas.DrawBitmap(picture.Image, SKRect.Create(picture.Left, picture.Top, picture.Width, picture.Height));
            }
            else
            {
                canvas.Save();
                canvas.ClipRect(SKRect.Create(picture.Left, picture.Top, picture.Width, picture.Height));
                canvas.DrawBitmap(picture.Image, picture.Left, picture.Top);
                canvas.Restore();
            }
        }

        /// <summary>メッセージウィンドウのクライアント領域の左上（画面座標）</summary>
        public SKPointI MessageWindowOrigin(Compat.MessageWindow window)
        {
            // 元の SRC と同じくメインウィンドウの左下に重ねて表示する
            var rect = ScreenLayout.MessageWindowRect(window.Width, window.Height, window.Centered);
            return new SKPointI(rect.Left + ScreenLayout.MessageFramePx, rect.Top + ScreenLayout.MessageTitleBarPx);
        }

        private void DrawMessageArea(SKCanvas canvas, Compat.MessageWindow window)
        {
            if (window == null || !window.Visible)
            {
                return;
            }
            var rect = ScreenLayout.MessageWindowRect(window.Width, window.Height, window.Centered);
            using (var paint = new SKPaint { IsAntialias = false })
            {
                // 枠とタイトルバー（リストボックスなどのウィンドウと同じ見た目）
                paint.Color = Compat.Graphics.ToSKColor(window.BackColor);
                canvas.DrawRect(rect, paint);
                paint.Color = WindowsHighlight;
                canvas.DrawRect(rect.Left + 2, rect.Top + 2, rect.Width - 4, ScreenLayout.MessageTitleBarPx - 3, paint);
                DrawBevel(canvas, rect, raised: true, paint);
                using (var titleFont = ListTitleFont.ToSKFont())
                {
                    paint.Color = SKColors.White;
                    paint.IsAntialias = true;
                    var title = window.IsAutoMode ? $"{Resources.Strings.MessageWindowTitle} ({Resources.Strings.AutoAdvance})" : Resources.Strings.MessageWindowTitle;
                    canvas.DrawText(title, rect.Left + 6, rect.Top + 1 + (ScreenLayout.MessageTitleBarPx - Compat.SkiaFonts.LineHeight(titleFont)) / 2 + Compat.SkiaFonts.Ascent(titleFont), SKTextAlign.Left, titleFont, paint);
                }
            }
            var origin = MessageWindowOrigin(window);
            canvas.Save();
            canvas.Translate(origin.X, origin.Y);
            using (var paint = new SKPaint { IsAntialias = false })
            {
                foreach (var picture in new[] { window.picFace, window.picMessage, window.picUnit1, window.picUnit2, window.picHP1, window.picEN1, window.picHP2, window.picEN2 })
                {
                    DrawPictureBox(canvas, picture, paint);
                }
                foreach (var label in new[] { window.labHP1, window.labEN1, window.labHP2, window.labEN2, window.txtHP1, window.txtEN1, window.txtHP2, window.txtEN2 })
                {
                    if (!label.Visible || string.IsNullOrEmpty(label.Text))
                    {
                        continue;
                    }
                    using (var font = label.Font.ToSKFont())
                    {
                        paint.Color = Compat.Graphics.ToSKColor(label.ForeColor);
                        paint.IsAntialias = true;
                        canvas.DrawText(label.Text, label.Left, label.Top + Compat.SkiaFonts.Ascent(font), SKTextAlign.Left, font, paint);
                        paint.IsAntialias = false;
                    }
                }
            }
            canvas.Restore();
        }

        private void DrawTelop(SKCanvas canvas, string telop, bool mainFormVisible)
        {
            using (var paint = new SKPaint())
            {
                // 元の SRC（GUI.bas の DisplayTelop）と同じく、メインウィンドウを表示していればマップの中央、隠していれば画面の中央
                var area = mainFormVisible ? ScreenLayout.MapArea : new SKRectI(0, 0, ScreenLayout.Width, ScreenLayout.Height);
                var width = Math.Min(area.Width - 32, largeFont.MeasureText(telop) + 48);
                var box = SKRect.Create(area.Left + (area.Width - width) / 2, area.MidY - 32, width, 64);
                DrawPanel(canvas, box, paint);
                paint.Style = SKPaintStyle.Fill;
                paint.Color = TextColor;
                canvas.DrawText(telop, box.MidX, box.MidY + largeFont.Size / 2 - 4, SKTextAlign.Center, largeFont, paint);
            }
        }

        private static readonly SKColor WindowsHighlight = new SKColor(0, 0, 128);
        private static readonly SKColor WindowsShadow = new SKColor(128, 128, 128);
        private static readonly SKColor WindowsDarkShadow = new SKColor(64, 64, 64);
        private static readonly Compat.Font ListTitleFont = new Compat.Font("ＭＳ Ｐゴシック", 9F, Compat.FontStyle.Bold);

        /// <summary>
        /// リストボックス（WinForms 版 frmListBox と同じ部品配置）を描く。
        /// </summary>
        /// <summary>
        /// 画面上のウィンドウ（リストボックス・設定変更など、WinForms 版のフォームと同じ部品配置）を描く。
        /// </summary>
        private void DrawForm(SKCanvas canvas, Compat.Form w)
        {
            canvas.Save();
            canvas.Translate(w.ScreenLeft, w.ScreenTop);
            using (var paint = new SKPaint { IsAntialias = false })
            {
                // 枠とタイトルバー
                paint.Color = Compat.Graphics.ToSKColor(w.BackColor);
                canvas.DrawRect(0, 0, w.Width, w.OuterHeight, paint);
                paint.Color = WindowsHighlight;
                canvas.DrawRect(1, 1, w.Width - 2, Compat.Form.TitleBarHeight - 2, paint);
                DrawBevel(canvas, SKRect.Create(0, 0, w.Width, w.OuterHeight), raised: true, paint);
                using (var titleFont = ListTitleFont.ToSKFont())
                {
                    paint.Color = SKColors.White;
                    paint.IsAntialias = true;
                    canvas.DrawText(w.Text ?? "", 6, (Compat.Form.TitleBarHeight - Compat.SkiaFonts.LineHeight(titleFont)) / 2 + Compat.SkiaFonts.Ascent(titleFont), SKTextAlign.Left, titleFont, paint);
                    paint.IsAntialias = false;
                }

                canvas.Translate(0, Compat.Form.TitleBarHeight);
                switch (w)
                {
                    case Compat.ListWindow lw:
                        foreach (var picture in lw.Pictures)
                        {
                            DrawPictureBox(canvas, picture, paint);
                        }
                        foreach (var label in lw.TextLabels)
                        {
                            DrawLabel(canvas, label, paint);
                        }
                        DrawTextBox(canvas, lw.labCaption, paint);
                        DrawTextBox(canvas, lw.txtComment, paint);
                        break;
                    case Compat.MultiColumnListWindow mw:
                        DrawTextBox(canvas, mw.labCaption, paint);
                        break;
                    case Compat.ConfigWindow cw:
                        foreach (var check in cw.CheckBoxes)
                        {
                            DrawCheckBox(canvas, check, paint);
                        }
                        DrawLabel(canvas, cw.labMP3Volume, paint);
                        DrawTextBox(canvas, cw.txtMP3Volume, paint);
                        DrawScrollBar(canvas, cw.hscMP3Volume, paint);
                        foreach (var button in cw.Buttons)
                        {
                            DrawButton(canvas, button, paint);
                        }
                        break;
                    case Compat.MultiSelectListWindow sw:
                        DrawTextBox(canvas, sw.lblCaption, paint);
                        DrawLabel(canvas, sw.lblNumber, paint);
                        foreach (var button in sw.Buttons)
                        {
                            DrawButton(canvas, button, paint);
                        }
                        break;
                }
                DrawListBox(canvas, w.lstItems, paint);
            }
            canvas.Restore();
        }

        private static readonly Compat.Font TooltipFont = new Compat.Font("ＭＳ Ｐゴシック", 9F);

        /// <summary>ツールチップ（カーソルの右下に表示し、画面からはみ出さないようにする）</summary>
        private static void DrawTooltip(SKCanvas canvas, TooltipState tooltip)
        {
            using (var font = TooltipFont.ToSKFont())
            using (var paint = new SKPaint { IsAntialias = false })
            {
                var lines = (tooltip.Text ?? "").Replace("\r\n", "\n").Split('\n');
                var lineHeight = Compat.SkiaFonts.LineHeight(font);
                var width = lines.Max(x => font.MeasureText(x)) + 8;
                var height = lineHeight * lines.Length + 6;
                var left = Math.Min(tooltip.X + 12, ScreenLayout.Width - width - 1);
                var top = Math.Min(tooltip.Y + 20, ScreenLayout.Height - height - 1);
                var rect = SKRect.Create(Math.Max(0, left), Math.Max(0, top), width, height);
                paint.Color = new SKColor(255, 255, 225);
                canvas.DrawRect(rect, paint);
                paint.Style = SKPaintStyle.Stroke;
                paint.Color = SKColors.Black;
                canvas.DrawRect(rect.Left + 0.5f, rect.Top + 0.5f, rect.Width - 1, rect.Height - 1, paint);
                paint.Style = SKPaintStyle.Fill;
                paint.IsAntialias = true;
                var y = rect.Top + 3;
                foreach (var line in lines)
                {
                    canvas.DrawText(line, rect.Left + 4, y + Compat.SkiaFonts.Ascent(font), SKTextAlign.Left, font, paint);
                    y += lineHeight;
                }
            }
        }

        private static void DrawCheckBox(SKCanvas canvas, Compat.CheckBox check, SKPaint paint)
        {
            if (!check.Visible)
            {
                return;
            }
            const int box = 13;
            var top = check.Top + (check.Height - box) / 2f;
            var rect = SKRect.Create(check.Left, top, box, box);
            paint.Color = check.Enabled ? SKColors.White : new SKColor(192, 192, 192);
            canvas.DrawRect(rect, paint);
            DrawBevel(canvas, rect, raised: false, paint);
            if (check.Checked)
            {
                paint.Color = check.Enabled ? SKColors.Black : WindowsShadow;
                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeWidth = 2;
                paint.IsAntialias = true;
                using (var path = new SKPath())
                {
                    path.MoveTo(rect.Left + 3, rect.MidY);
                    path.LineTo(rect.Left + 5.5f, rect.Bottom - 3.5f);
                    path.LineTo(rect.Right - 3, rect.Top + 3);
                    canvas.DrawPath(path, paint);
                }
                paint.IsAntialias = false;
                paint.StrokeWidth = 1;
                paint.Style = SKPaintStyle.Fill;
            }
            using (var font = check.Font.ToSKFont())
            {
                paint.Color = check.Enabled ? SKColors.Black : WindowsShadow;
                paint.IsAntialias = true;
                canvas.DrawText(check.Text ?? "", rect.Right + 5, check.Top + (check.Height - Compat.SkiaFonts.LineHeight(font)) / 2 + Compat.SkiaFonts.Ascent(font), SKTextAlign.Left, font, paint);
                paint.IsAntialias = false;
            }
        }

        private static void DrawScrollBar(SKCanvas canvas, Compat.HScrollBar bar, SKPaint paint)
        {
            if (!bar.Visible)
            {
                return;
            }
            var rect = SKRect.Create(bar.Left, bar.Top, bar.Width, bar.Height);
            paint.Color = new SKColor(224, 224, 224);
            canvas.DrawRect(rect, paint);
            var thumb = SKRect.Create(rect.Left + bar.ThumbLeft, rect.Top, Compat.HScrollBar.ThumbWidth, rect.Height);
            paint.Color = new SKColor(192, 192, 192);
            canvas.DrawRect(thumb, paint);
            DrawBevel(canvas, thumb, raised: true, paint);
        }

        private static void DrawButton(SKCanvas canvas, Compat.Button button, SKPaint paint)
        {
            if (!button.Visible)
            {
                return;
            }
            var rect = SKRect.Create(button.Left, button.Top, button.Width, button.Height);
            paint.Color = new SKColor(192, 192, 192);
            canvas.DrawRect(rect, paint);
            DrawBevel(canvas, rect, raised: true, paint);
            using (var font = button.Font.ToSKFont())
            {
                paint.Color = button.Enabled ? SKColors.Black : WindowsShadow;
                paint.IsAntialias = true;
                canvas.DrawText(button.Text ?? "", rect.MidX, rect.Top + (rect.Height - Compat.SkiaFonts.LineHeight(font)) / 2 + Compat.SkiaFonts.Ascent(font), SKTextAlign.Center, font, paint);
                paint.IsAntialias = false;
            }
        }

        private static void DrawLabel(SKCanvas canvas, Compat.Label label, SKPaint paint)
        {
            if (!label.Visible || string.IsNullOrEmpty(label.Text))
            {
                return;
            }
            using (var font = label.Font.ToSKFont())
            {
                paint.Color = Compat.Graphics.ToSKColor(label.ForeColor);
                paint.IsAntialias = true;
                canvas.DrawText(label.Text, label.Left, label.Top + Compat.SkiaFonts.Ascent(font), SKTextAlign.Left, font, paint);
                paint.IsAntialias = false;
            }
        }

        /// <summary>白地でへこんだ枠の文字欄（Label / TextBox）。文字は縦中央に置く。</summary>
        private static void DrawTextBox(SKCanvas canvas, Compat.Label label, SKPaint paint)
        {
            if (!label.Visible)
            {
                return;
            }
            var rect = SKRect.Create(label.Left, label.Top, label.Width, label.Height);
            paint.Color = Compat.Graphics.ToSKColor(label.BackColor);
            canvas.DrawRect(rect, paint);
            DrawBevel(canvas, rect, raised: false, paint);
            if (string.IsNullOrEmpty(label.Text))
            {
                return;
            }
            using (var font = label.Font.ToSKFont())
            {
                canvas.Save();
                canvas.ClipRect(SKRect.Create(rect.Left + 2, rect.Top + 2, rect.Width - 4, rect.Height - 4));
                paint.Color = Compat.Graphics.ToSKColor(label.ForeColor);
                paint.IsAntialias = true;
                var lineHeight = Compat.SkiaFonts.LineHeight(font);
                var lines = label.Text.Replace("\r\n", "\n").Split('\n');
                var y = lines.Length == 1 ? rect.Top + (rect.Height - lineHeight) / 2 : rect.Top + 3;
                foreach (var line in lines)
                {
                    canvas.DrawText(line, rect.Left + 3, y + Compat.SkiaFonts.Ascent(font), SKTextAlign.Left, font, paint);
                    y += lineHeight;
                }
                paint.IsAntialias = false;
                canvas.Restore();
            }
        }

        private static void DrawListBox(SKCanvas canvas, Compat.ListBoxControl box, SKPaint paint)
        {
            if (!box.Visible)
            {
                return;
            }
            var rect = SKRect.Create(box.Left, box.Top, box.Width, box.Height);
            paint.Color = Compat.Graphics.ToSKColor(box.BackColor);
            canvas.DrawRect(rect, paint);
            using (var font = box.Font.ToSKFont())
            {
                canvas.Save();
                canvas.ClipRect(SKRect.Create(rect.Left + 2, rect.Top + 2, box.ItemsWidth, rect.Height - 4));
                for (var index = box.TopIndex; index < Math.Min(box.Items.Count, box.TopIndex + box.VisibleRows); index++)
                {
                    var bounds = box.ItemBounds(index).Value;
                    var item = SKRect.Create(rect.Left + bounds.X, rect.Top + bounds.Y, bounds.Width, bounds.Height);
                    var selected = index == box.SelectedIndex;
                    if (selected)
                    {
                        paint.Color = WindowsHighlight;
                        canvas.DrawRect(item, paint);
                    }
                    paint.Color = selected ? SKColors.White : Compat.Graphics.ToSKColor(box.ForeColor);
                    paint.IsAntialias = true;
                    canvas.Save();
                    canvas.ClipRect(item);
                    canvas.DrawText(box.Items[index] ?? "", item.Left + 1, item.Top + (item.Height - Compat.SkiaFonts.LineHeight(font)) / 2 + Compat.SkiaFonts.Ascent(font), SKTextAlign.Left, font, paint);
                    canvas.Restore();
                    paint.IsAntialias = false;
                }
                canvas.Restore();
            }
            if (box.HasScrollBar)
            {
                SKRect bar, thumb;
                if (box.MultiColumn)
                {
                    // 横スクロールバー（列単位）
                    bar = SKRect.Create(rect.Left + 2, rect.Bottom - 2 - Compat.ListBoxControl.ScrollBarWidth, rect.Width - 4, Compat.ListBoxControl.ScrollBarWidth);
                    var columns = (box.Items.Count + box.Rows - 1) / box.Rows;
                    var thumbWidth = Math.Max(8, bar.Width * box.VisibleColumns / columns);
                    var thumbLeft = bar.Left + (bar.Width - thumbWidth) * (box.TopIndex / box.Rows) / Math.Max(1, columns - box.VisibleColumns);
                    thumb = SKRect.Create(thumbLeft, bar.Top, thumbWidth, bar.Height);
                }
                else
                {
                    bar = SKRect.Create(rect.Left + 2 + box.ItemsWidth, rect.Top + 2, Compat.ListBoxControl.ScrollBarWidth, rect.Height - 4);
                    var rows = box.VisibleRows;
                    var thumbHeight = Math.Max(8, bar.Height * rows / box.Items.Count);
                    var thumbTop = bar.Top + (bar.Height - thumbHeight) * box.TopIndex / Math.Max(1, box.Items.Count - rows);
                    thumb = SKRect.Create(bar.Left, thumbTop, bar.Width, thumbHeight);
                }
                paint.Color = new SKColor(224, 224, 224);
                canvas.DrawRect(bar, paint);
                paint.Color = new SKColor(192, 192, 192);
                canvas.DrawRect(thumb, paint);
                DrawBevel(canvas, thumb, raised: true, paint);
            }
            DrawBevel(canvas, rect, raised: false, paint);
        }

        /// <summary>Windows クラシックの立体枠。</summary>
        private static void DrawBevel(SKCanvas canvas, SKRect rect, bool raised, SKPaint paint)
        {
            var light = raised ? SKColors.White : WindowsShadow;
            var dark = raised ? WindowsDarkShadow : SKColors.White;
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 1;
            paint.Color = light;
            canvas.DrawLine(rect.Left + 0.5f, rect.Bottom - 1, rect.Left + 0.5f, rect.Top + 0.5f, paint);
            canvas.DrawLine(rect.Left + 0.5f, rect.Top + 0.5f, rect.Right - 1, rect.Top + 0.5f, paint);
            paint.Color = dark;
            canvas.DrawLine(rect.Right - 0.5f, rect.Top, rect.Right - 0.5f, rect.Bottom - 0.5f, paint);
            canvas.DrawLine(rect.Left, rect.Bottom - 0.5f, rect.Right - 0.5f, rect.Bottom - 0.5f, paint);
            paint.Style = SKPaintStyle.Fill;
        }

        private static readonly Compat.Font MenuFont = new Compat.Font("ＭＳ Ｐゴシック", 9F);
        private static readonly Compat.Font DialogFont = new Compat.Font("ＭＳ Ｐゴシック", 9F);
        private static readonly Compat.Font DialogTitleFont = new Compat.Font("ＭＳ Ｐゴシック", 9F, Compat.FontStyle.Bold);

        /// <summary>
        /// ポップアップメニュー（WinForms 版の ContextMenuStrip と同じく Windows 標準の見た目）。
        /// </summary>
        private void DrawMenu(SKCanvas canvas, MenuState menu)
        {
            var box = LayoutMenu(menu);
            using (var font = MenuFont.ToSKFont())
            using (var paint = new SKPaint { IsAntialias = false })
            {
                paint.Color = new SKColor(192, 192, 192);
                canvas.DrawRect(box, paint);
                DrawBevel(canvas, box, raised: true, paint);
                for (var i = 0; i < menu.Items.Count; i++)
                {
                    var rect = MenuItemRect(box, i);
                    var highlighted = i == menu.Highlight;
                    if (highlighted)
                    {
                        paint.Color = WindowsHighlight;
                        canvas.DrawRect(rect, paint);
                    }
                    paint.Color = highlighted ? SKColors.White : SKColors.Black;
                    paint.IsAntialias = true;
                    var baseline = rect.Top + (rect.Height - Compat.SkiaFonts.LineHeight(font)) / 2 + Compat.SkiaFonts.Ascent(font);
                    if (i < menu.Checked.Count && menu.Checked[i])
                    {
                        canvas.DrawText("✓", rect.Left + 4, baseline, SKTextAlign.Left, font, paint);
                    }
                    canvas.DrawText(menu.Items[i] ?? "", rect.Left + MenuTextLeft, baseline, SKTextAlign.Left, font, paint);
                    paint.IsAntialias = false;
                }
            }
        }

        /// <summary>
        /// メッセージボックス（WinForms 版の MessageBox と同じく Windows 標準の見た目）。
        /// </summary>
        /// <summary>キー操作のマップカーソル（黒で縁取った黄色の枠）</summary>
        private static void DrawMapCursor(SKCanvas canvas, SKRectI cell)
        {
            using (var paint = new SKPaint { IsAntialias = false, Style = SKPaintStyle.Stroke })
            {
                paint.StrokeWidth = 4;
                paint.Color = SKColors.Black;
                canvas.DrawRect(cell.Left + 2, cell.Top + 2, cell.Width - 4, cell.Height - 4, paint);
                paint.StrokeWidth = 2;
                paint.Color = new SKColor(0xFF, 0xE0, 0x40);
                canvas.DrawRect(cell.Left + 2, cell.Top + 2, cell.Width - 4, cell.Height - 4, paint);
            }
        }

        private void DrawDialog(SKCanvas canvas, DialogState dialog)
        {
            var layout = LayoutDialog(dialog);
            var box = layout.Box;
            using (var font = DialogFont.ToSKFont())
            using (var titleFont = DialogTitleFont.ToSKFont())
            using (var paint = new SKPaint { IsAntialias = false })
            {
                paint.Color = new SKColor(192, 192, 192);
                canvas.DrawRect(box, paint);
                DrawBevel(canvas, box, raised: true, paint);
                paint.Color = WindowsHighlight;
                canvas.DrawRect(box.Left + 2, box.Top + 2, box.Width - 4, Compat.Form.TitleBarHeight - 2, paint);
                paint.Color = SKColors.White;
                paint.IsAntialias = true;
                canvas.DrawText(dialog.Title ?? "", box.Left + 6, box.Top + 2 + (Compat.Form.TitleBarHeight - 2 - Compat.SkiaFonts.LineHeight(titleFont)) / 2 + Compat.SkiaFonts.Ascent(titleFont), SKTextAlign.Left, titleFont, paint);
                paint.Color = SKColors.Black;
                var y = box.Top + Compat.Form.TitleBarHeight + DialogPadding;
                var lineHeight = Compat.SkiaFonts.LineHeight(font) + 2;
                foreach (var line in layout.Lines)
                {
                    canvas.DrawText(line, box.Left + DialogPadding, y + Compat.SkiaFonts.Ascent(font), SKTextAlign.Left, font, paint);
                    y += lineHeight;
                }
                paint.IsAntialias = false;
                for (var i = 0; i < dialog.Buttons.Count; i++)
                {
                    DrawButton(canvas, new Compat.Button
                    {
                        Left = (int)layout.ButtonRect(i).Left,
                        Top = (int)layout.ButtonRect(i).Top,
                        Width = DialogButtonWidth,
                        Height = DialogButtonHeight,
                        Text = dialog.Buttons[i],
                        Font = DialogFont,
                    }, paint);
                    if (i == dialog.Highlight && dialog.Buttons.Count > 1)
                    {
                        // キー操作で選んでいるボタン（Windows のフォーカスの点線枠）
                        var focus = layout.ButtonRect(i);
                        using (var dash = new SKPaint { IsAntialias = false, Style = SKPaintStyle.Stroke, Color = SKColors.Black, PathEffect = SKPathEffect.CreateDash(new[] { 1f, 1f }, 0) })
                        {
                            canvas.DrawRect(focus.Left + 4, focus.Top + 4, focus.Width - 9, focus.Height - 9, dash);
                        }
                    }
                }
            }
        }

        /// <summary>タイトルの枠の大きさ（元の SRC・SRC#Form の frmTitle と同じ）</summary>
        public static readonly SKSize TitleSize = new SKSize(386, 233);

        /// <summary>
        /// 起動時のタイトル。元の SRC の frmTitle（SRC#Form の Forms/Title.Designer.cs）の配置に合わせる。
        /// </summary>
        private void DrawTitle(SKCanvas canvas, TitleState title)
        {
            var area = ScreenLayout.MapArea;
            var box = SKRect.Create(area.Left + (area.Width - TitleSize.Width) / 2, area.Top + (area.Height - TitleSize.Height) / 2, TitleSize.Width, TitleSize.Height);
            using (var paint = new SKPaint { IsAntialias = false })
            {
                paint.Color = FormGray;
                canvas.DrawRect(box, paint);
                DrawBevel(canvas, box, raised: true, paint);
                // 枠（Frame1）
                var frame = SKRect.Create(box.Left + 24, box.Top + 8, 337, 201);
                paint.Style = SKPaintStyle.Stroke;
                paint.Color = WindowsShadow;
                canvas.DrawRect(frame, paint);
                paint.Color = SKColors.White;
                canvas.DrawRect(SKRect.Create(frame.Left + 1, frame.Top + 1, frame.Width, frame.Height), paint);
                paint.Style = SKPaintStyle.Fill;
                paint.Color = SKColors.Black;
                paint.IsAntialias = true;

                void Text(string text, float x, float y, float points, SKTextAlign align = SKTextAlign.Left)
                {
                    using (var font = new Compat.Font("Times New Roman", points).ToSKFont())
                    {
                        canvas.DrawText(text, x, y + Compat.SkiaFonts.Ascent(font), align, font, paint);
                    }
                }

                Text("SRC#", frame.Left + 118, frame.Top + 56, 20f);
                Text("- Simulation RPG Construction -", frame.Left + 118, frame.Top + 87, 9.75f);
                Text("Ver " + title.Version, frame.Left + 144 + 177, frame.Top + 127, 15.75f, SKTextAlign.Right);
                Text("koudenpa", frame.Left + 257, frame.Top + 156, 11.25f);
                Text("SRC by Kei Sakamoto / Inui Tetsuyuki", frame.Left + 16, frame.Top + 173, 11.25f);
                Text("This program is distributed under the terms of GPL", box.Left + 8, box.Top + 216, 9f);
            }
        }

        private static void DrawPanel(SKCanvas canvas, SKRect box, SKPaint paint)
        {
            paint.Style = SKPaintStyle.Fill;
            paint.Color = PanelColor;
            canvas.DrawRoundRect(box, 6, 6, paint);
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = 2;
            paint.Color = PanelBorderColor;
            canvas.DrawRoundRect(box, 6, 6, paint);
            paint.StrokeWidth = 1;
        }

        private static void DrawCenteredText(SKCanvas canvas, SKRectI area, string text, SKFont textFont)
        {
            using (var paint = new SKPaint { Color = TextColor })
            {
                canvas.DrawText(text, area.MidX, area.MidY, SKTextAlign.Center, textFont, paint);
            }
        }

        #endregion

        #region 座標・レイアウト

        /// <summary>マップ表示上の X 座標（ピクセル）からマップ座標へ</summary>
        public static int ViewToMapX(ScreenState state, int px)
        {
            return px / ScreenLayout.CellPx + 1 + state.MapX - (ScreenLayout.MapColumns + 1) / 2;
        }

        public static int ViewToMapY(ScreenState state, int py)
        {
            return py / ScreenLayout.CellPx + 1 + state.MapY - (ScreenLayout.MapRows + 1) / 2;
        }

        /// <summary>マップ座標からマップ表示上の X 座標（ピクセル）へ</summary>
        public static int MapToViewX(ScreenState state, int x)
        {
            return ScreenLayout.CellPx * ((ScreenLayout.MapColumns + 1) / 2 - 1 - (state.MapX - x));
        }

        public static int MapToViewY(ScreenState state, int y)
        {
            return ScreenLayout.CellPx * ((ScreenLayout.MapRows + 1) / 2 - 1 - (state.MapY - y));
        }

        public class ListLayout
        {
            public SKRect Box { get; set; }
            /// <summary>ListBox の左上（画面座標）</summary>
            public SKPoint Origin { get; set; }
            public Compat.ListBoxControl ListBox { get; set; }
            public int Top { get; set; }
            public int VisibleCount { get; set; }

            /// <summary>表示中の項目の位置（画面座標）</summary>
            public SKRect ItemRect(int visibleIndex)
            {
                var bounds = ListBox.ItemBounds(Top + visibleIndex) ?? default;
                return SKRect.Create(Origin.X + bounds.X, Origin.Y + bounds.Y, bounds.Width, bounds.Height);
            }
        }

        /// <summary>リストボックスの画面上の配置（項目の位置は画面座標）。</summary>
        public ListLayout LayoutList(ListState list)
        {
            var w = list.Window;
            var box = w.lstItems;
            return new ListLayout
            {
                Box = SKRect.Create(w.ScreenLeft, w.ScreenTop, w.Width, w.OuterHeight),
                Origin = new SKPoint(w.ScreenLeft + box.Left, w.ScreenTop + Compat.Form.TitleBarHeight + box.Top),
                ListBox = box,
                Top = box.TopIndex,
                VisibleCount = Math.Max(0, Math.Min(box.VisibleRows, box.Items.Count - box.TopIndex)),
            };
        }

        /// <summary>選択肢のヒットテスト。項目のインデックス（0 始まり）、外れは -1</summary>
        public int HitTestList(ListState list, float x, float y)
        {
            var layout = LayoutList(list);
            for (var i = 0; i < layout.VisibleCount; i++)
            {
                if (layout.ItemRect(i).Contains(x, y))
                {
                    return layout.Top + i;
                }
            }
            return -1;
        }

        private const int MenuTextLeft = 20;

        private static SKRect MenuItemRect(SKRect box, int index)
        {
            return SKRect.Create(box.Left + 3, box.Top + 3 + index * MenuItemHeight, box.Width - 6, MenuItemHeight);
        }

        public SKRect LayoutMenu(MenuState menu)
        {
            float textWidth;
            using (var font = MenuFont.ToSKFont())
            {
                textWidth = menu.Items.Select(x => font.MeasureText(x ?? "")).DefaultIfEmpty(0).Max();
            }
            var width = Math.Max(MenuMinWidth, (int)Math.Ceiling(textWidth) + MenuTextLeft + 24);
            var height = menu.Items.Count * MenuItemHeight + 6;
            var left = Math.Min(Math.Max(0, menu.X), ScreenLayout.Width - width);
            var top = Math.Min(Math.Max(0, menu.Y), ScreenLayout.Height - height);
            return SKRect.Create(left, top, width, height);
        }

        /// <summary>メニューの項目の中心（画面座標）</summary>
        public SKPoint MenuItemCenter(MenuState menu, int index)
        {
            var rect = MenuItemRect(LayoutMenu(menu), index);
            return new SKPoint(rect.MidX, rect.MidY);
        }

        /// <summary>メニューのヒットテスト。項目のインデックス（0 始まり）、外れは -1</summary>
        public int HitTestMenu(MenuState menu, float x, float y)
        {
            var box = LayoutMenu(menu);
            for (var i = 0; i < menu.Items.Count; i++)
            {
                if (MenuItemRect(box, i).Contains(x, y))
                {
                    return i;
                }
            }
            return -1;
        }

        private const int DialogPadding = 16;

        public class DialogLayout
        {
            public SKRect Box { get; set; }
            public IList<string> Lines { get; set; }
            public int ButtonCount { get; set; }

            public SKRect ButtonRect(int index)
            {
                var totalWidth = ButtonCount * DialogButtonWidth + (ButtonCount - 1) * 8;
                var left = (float)Math.Floor(Box.MidX - totalWidth / 2f) + index * (DialogButtonWidth + 8);
                return SKRect.Create(left, Box.Bottom - DialogButtonHeight - 12, DialogButtonWidth, DialogButtonHeight);
            }
        }

        public DialogLayout LayoutDialog(DialogState dialog)
        {
            using (var font = DialogFont.ToSKFont())
            {
                var lines = WrapText(dialog.Text ?? "", DialogWidth - DialogPadding * 2, font);
                var textWidth = lines.Select(x => font.MeasureText(x)).DefaultIfEmpty(0).Max();
                var buttonsWidth = dialog.Buttons.Count * (DialogButtonWidth + 8);
                var width = (float)Math.Ceiling(Math.Max(DialogMinWidth, Math.Max(textWidth, buttonsWidth) + DialogPadding * 2));
                var height = Compat.Form.TitleBarHeight + DialogPadding + lines.Count * (Compat.SkiaFonts.LineHeight(font) + 2) + DialogPadding + DialogButtonHeight + 12;
                return new DialogLayout
                {
                    Box = SKRect.Create((float)Math.Floor((ScreenLayout.Width - width) / 2f), (float)Math.Floor(Math.Max(16, (ScreenLayout.Height - height) / 2f)), width, height),
                    Lines = lines,
                    ButtonCount = dialog.Buttons.Count,
                };
            }
        }

        /// <summary>ダイアログのボタンのヒットテスト。ボタンのインデックス、外れは -1</summary>
        public int HitTestDialog(DialogState dialog, float x, float y)
        {
            var layout = LayoutDialog(dialog);
            for (var i = 0; i < dialog.Buttons.Count; i++)
            {
                if (layout.ButtonRect(i).Contains(x, y))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>指定幅に収まるように折り返す</summary>
        public IList<string> WrapText(string text, float width)
        {
            return WrapText(text, width, font);
        }

        private static IList<string> WrapText(string text, float width, SKFont font)
        {
            var result = new List<string>();
            foreach (var paragraph in text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n'))
            {
                var line = "";
                foreach (var c in paragraph)
                {
                    if (font.MeasureText(line + c) > width && line.Length > 0)
                    {
                        result.Add(line);
                        line = "";
                    }
                    line += c;
                }
                result.Add(line);
            }
            return result;
        }

        #endregion
    }
}
