using SRCCore;
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.VB;
using SRCSharpForm.Extensions;
using SRCSharpForm.Lib;
using System;
using System.Drawing;

namespace SRCSharpForm
{
    public partial class SRCSharpFormGUI
    {
        private Color GetDrawBgColor(bool transparent)
        {
            return transparent ? Color.Transparent : BGColor;
        }

        // 画像をウィンドウに描画
        public bool DrawPicture(string fname, int dx, int dy, int dw, int dh, int sx, int sy, int sw, int sh, string draw_option)
        {
            // ダミーのファイル名？
            switch (fname ?? "")
            {
                case var @case when @case == "":
                case "-.bmp":
                case "EFFECT_Void.bmp":
                    return false;
            }

            SRC.LogTrace($"{fname} {draw_option}");

            // オプションの解析
            var BGColor = Color.White;
            // マスク画像に影響しないオプション
            var pic_option = "";
            // マスク画像に影響するオプション
            var pic_option2 = "";
            // フィルタ時の透過度を初期化
            float trans_par = -1;
            bool permanent = false;
            bool transparent = false;
            int bright_count = 0;
            int dark_count = 0;
            bool is_monotone = false;
            bool is_sepia = false;
            bool is_sunset = false;
            bool is_water = false;
            bool is_colorfilter = false;
            bool hrev = false;
            bool vrev = false;
            bool negpos = false;
            bool is_sil = false;
            bool top_part = false;
            bool bottom_part = false;
            bool right_part = false;
            bool left_part = false;
            bool tright_part = false;
            bool tleft_part = false;
            bool bright_part = false;
            bool bleft_part = false;
            bool on_msg_window = false;
            bool on_status_window = false;
            bool keep_picture = false;
            Color fcolor = default;
            int angle = 0;
            var opts = GeneralLib.ToL(draw_option);
            for (var i = 0; i < opts.Count; i++)
            {
                var opt = opts[i];
                switch (opt ?? "")
                {
                    case "背景":
                        permanent = true;
                        // 背景書き込みで夜やセピア色のマップの場合は指定がなくても特殊効果を付ける
                        switch (Map.MapDrawMode ?? "")
                        {
                            case "夜":
                                dark_count = (dark_count + 1);
                                pic_option = pic_option + " 暗";
                                break;

                            case "白黒":
                                is_monotone = true;
                                pic_option = pic_option + " 白黒";
                                break;

                            case "セピア":
                                is_sepia = true;
                                pic_option = pic_option + " セピア";
                                break;

                            case "夕焼け":
                                is_sunset = true;
                                pic_option = pic_option + " 夕焼け";
                                break;

                            case "水中":
                                is_water = true;
                                pic_option = pic_option + " 水中";
                                break;

                            case "フィルタ":
                                is_colorfilter = true;
                                fcolor = Map.MapDrawFilterColor;
                                pic_option2 = pic_option2 + " フィルタ=" + Map.MapDrawFilterColor.ToHexString();
                                break;
                        }

                        break;

                    case "透過":
                        transparent = true;
                        pic_option = pic_option + " " + opt;
                        break;

                    case "白黒":
                        is_monotone = true;
                        pic_option = pic_option + " " + opt;
                        break;

                    case "セピア":
                        is_sepia = true;
                        pic_option = pic_option + " " + opt;
                        break;

                    case "夕焼け":
                        is_sunset = true;
                        pic_option = pic_option + " " + opt;
                        break;

                    case "水中":
                        is_water = true;
                        pic_option = pic_option + " " + opt;
                        break;

                    case "明":
                        bright_count = (bright_count + 1);
                        pic_option = pic_option + " " + opt;
                        break;

                    case "暗":
                        dark_count = (dark_count + 1);
                        pic_option = pic_option + " " + opt;
                        break;

                    case "左右反転":
                        hrev = true;
                        pic_option2 = pic_option2 + " " + opt;
                        break;

                    case "上下反転":
                        vrev = true;
                        pic_option2 = pic_option2 + " " + opt;
                        break;

                    case "ネガポジ反転":
                        negpos = true;
                        pic_option = pic_option + " " + opt;
                        break;

                    case "シルエット":
                        is_sil = true;
                        pic_option = pic_option + " " + opt;
                        break;

                    case "上半分":
                        top_part = true;
                        pic_option2 = pic_option2 + " " + opt;
                        break;

                    case "下半分":
                        bottom_part = true;
                        pic_option2 = pic_option2 + " " + opt;
                        break;

                    case "右半分":
                        right_part = true;
                        pic_option2 = pic_option2 + " " + opt;
                        break;

                    case "左半分":
                        left_part = true;
                        pic_option2 = pic_option2 + " " + opt;
                        break;

                    case "右上":
                        tright_part = true;
                        pic_option2 = pic_option2 + " " + opt;
                        break;

                    case "左上":
                        tleft_part = true;
                        pic_option2 = pic_option2 + " " + opt;
                        break;

                    case "右下":
                        bright_part = true;
                        pic_option2 = pic_option2 + " " + opt;
                        break;

                    case "左下":
                        bleft_part = true;
                        pic_option2 = pic_option2 + " " + opt;
                        break;

                    case "メッセージ":
                        on_msg_window = true;
                        break;

                    case "ステータス":
                        on_status_window = true;
                        break;

                    case "保持":
                        keep_picture = true;
                        break;

                    case "右回転":
                        i = (i + 1);
                        angle = GeneralLib.StrToLng(opts[i]);
                        pic_option2 = pic_option2 + " 右回転=" + SrcFormatter.Format(angle % 360);
                        break;

                    case "左回転":
                        i = (i + 1);
                        angle = GeneralLib.StrToLng(opts[i]) * -1;
                        pic_option2 = pic_option2 + " 右回転=" + SrcFormatter.Format(angle % 360);
                        break;

                    case "フィルタ":
                        is_colorfilter = true;
                        break;

                    default:
                        // フィルタオプションの解決をここでしている様子
                        if (Strings.Right(opt, 1) == "%" && Information.IsNumeric(Strings.Left(opt, Strings.Len(opt) - 1)))
                        {
                            trans_par = (float)Math.Max(0d, Math.Min(1d, Conversions.ToDouble(Strings.Left(opt, Strings.Len(opt) - 1)) / 100d));
                            pic_option2 = pic_option2 + " フィルタ透過度=" + opt;
                        }
                        else if (is_colorfilter)
                        {
                            fcolor = ColorTranslator.FromHtml(opt);
                            pic_option2 = pic_option2 + " フィルタ=" + opt;
                        }
                        else
                        {
                            BGColor = ColorTranslator.FromHtml(opt);
                            pic_option2 = pic_option2 + " " + opt;
                        }
                        break;
                }
            }

            pic_option = Strings.Trim(pic_option);
            pic_option2 = Strings.Trim(pic_option2);

            // 描画先を設定
            Graphics g = null;
            if (on_msg_window)
            {
                // メッセージウィンドウへのパイロット画像の描画
                frmMessage.picFace.NewImageIfNull();
                g = Graphics.FromImage(frmMessage.picFace.Image);
                permanent = false;
            }
            else if (on_status_window)
            {
                //// ステータスウィンドウへのパイロット画像の描画
                MainForm.picUnitStatus.NewImageIfNull();
                g = Graphics.FromImage(MainForm.picUnitStatus.Image);
            }
            else if (permanent)
            {
                //// 背景への描画
                MainForm.InitBackgroundBufferIfInvalid();
                g = Graphics.FromImage(MainForm.BackgroundBuffer);
            }
            else
            {
                //// マップウィンドウへの通常の描画
                //pic = MainForm.picMain(0);
                SaveScreen();
                g = Graphics.FromImage(MainForm.MainBuffer);
            }
            if (g == null)
            {
                throw new NotSupportedException();
            }
            try
            {
                using (var fillIamge = new Bitmap((int)g.VisibleClipBounds.Width, (int)g.VisibleClipBounds.Height))
                {

                    // 特殊なファイル名を解決
                    // 読み込むファイルの探索
                    Image image = null;

                    // 特殊なファイル名
                    switch (Strings.LCase(fname) ?? "")
                    {
                        case "black.bmp":
                        case @"event\black.bmp":
                            // 黒で塗りつぶし
                            using(var fillG = Graphics.FromImage(fillIamge))
                            {
                                fillG.FillRectangle(Brushes.Black, g.VisibleClipBounds);
                            }
                            image = fillIamge;
                            break;

                        case "white.bmp":
                        case @"event\white.bmp":
                            // 白で塗りつぶし
                            using (var fillG = Graphics.FromImage(fillIamge))
                            {
                                fillG.FillRectangle(Brushes.White, g.VisibleClipBounds);
                            }
                            image = fillIamge;
                            break;

                        case @"common\effect_tile(ally).bmp":
                        case @"anime\common\effect_tile(ally).bmp":
                            // 味方ユニットタイル
                            image = MainForm.picUnit.Image;
                            break;

                        case @"common\effect_tile(enemy).bmp":
                        case @"anime\common\effect_tile(enemy).bmp":
                            // 敵ユニットタイル
                            image = MainForm.picEnemy.Image;
                            break;

                        case @"common\effect_tile(neutral).bmp":
                        case @"anime\common\effect_tile(neutral).bmp":
                            // 中立ユニットタイル
                            image = MainForm.picNeautral.Image;
                            break;
                    }

                    if (image == null)
                    {
                        // 各フォルダを検索する
                        image = transparent && BGColor == Color.White
                           ? imageBuffer.GetTransparent(fname)
                           : imageBuffer.Get(fname);
                    }

                    if (image == null)
                    {
                        // 表示を中止
                        return false;
                    }

                    var orig_width = image.Width;
                    var orig_height = image.Height;
                    Bitmap drawBuffer = null;


                    // 原画像の一部のみを描画？
                    if (sw != 0)
                    {
                        if (sw != orig_width || sh != orig_height)
                        {
                            // 原画像から描画部分をコピー
                            {
                                if (sx == Constants.DEFAULT_LEVEL)
                                {
                                    sx = (orig_width - sw) / 2;
                                }

                                if (sy == Constants.DEFAULT_LEVEL)
                                {
                                    sy = (orig_height - sh) / 2;
                                }
                                drawBuffer = new Bitmap(sw, sh);
                                using (var gBuf = Graphics.FromImage(drawBuffer))
                                {
                                    gBuf.DrawImage(image, new Rectangle(0, 0, sw, sh), new Rectangle(sx, sy, sw, sh), GraphicsUnit.Pixel);
                                }
                            }
                            orig_width = sw;
                            orig_height = sh;
                        }
                    }
                    if (drawBuffer == null)
                    {
                        drawBuffer = new Bitmap(image);
                    }
                    if (transparent && BGColor != Color.White)
                    {
                        drawBuffer.MakeTransparent(BGColor);
                    }
                    //LoadedPicture:

                    using (var gBuf = Graphics.FromImage(drawBuffer))
                    {
                        // 画像の一部を塗りつぶして描画する場合
                        if (top_part)
                        {
                            // 上半分
                            gBuf.FillRectangle(CurrentPaintBrush, 0f, orig_height / 2f, gBuf.VisibleClipBounds.Width, orig_height / 2f);
                        }

                        if (bottom_part)
                        {
                            // 下半分
                            gBuf.FillRectangle(CurrentPaintBrush, 0f, 0f, gBuf.VisibleClipBounds.Width, orig_height / 2f);
                        }

                        if (left_part)
                        {
                            // 左半分
                            gBuf.FillRectangle(CurrentPaintBrush, gBuf.VisibleClipBounds.Width / 2, 0f, gBuf.VisibleClipBounds.Width / 2f, orig_height);
                        }

                        if (right_part)
                        {
                            // 右半分
                            gBuf.FillRectangle(CurrentPaintBrush, 0f, 0f, gBuf.VisibleClipBounds.Width / 2f, orig_height);
                        }

                        if (tleft_part)
                        {
                            // 左上
                            gBuf.FillPolygon(CurrentPaintBrush, new Point[]{
                            new Point(orig_width -1,0),
                            new Point(orig_width -1,orig_height - 1),
                            new Point(0,orig_height - 1),
                        });
                        }

                        if (tright_part)
                        {
                            // 右上
                            gBuf.FillPolygon(CurrentPaintBrush, new Point[]{
                            new Point(0,0),
                            new Point(orig_width -1,orig_height - 1),
                            new Point(0,orig_height - 1),
                        });
                        }

                        if (bleft_part)
                        {
                            // 左下
                            gBuf.FillPolygon(CurrentPaintBrush, new Point[]{
                            new Point(0,0),
                            new Point(orig_width -1,0),
                            new Point(orig_width -1,orig_height - 1),
                        });
                        }

                        if (bright_part)
                        {
                            // 右下
                            gBuf.FillPolygon(CurrentPaintBrush, new Point[]{
                            new Point(0,0),
                            new Point(orig_width -1,0),
                            new Point(0,orig_height - 1),
                        });
                        }
                    }
                    // 特殊効果
                    if (is_monotone || is_sepia || is_sunset || is_water || is_colorfilter || bright_count > 0 || dark_count > 0 || negpos || is_sil || vrev || hrev || angle != 0)
                    {
                        // 画像のサイズをチェック
                        // XXX 別に4の倍数縛り要らないんじゃないかな。綺麗には出ないかもしれないけれど
                        //if (orig_width * orig_height % 4 != 0)
                        //{
                        //    ErrorMessage(fname + "の画像サイズが4の倍数になっていません");
                        //    return DrawPictureRet;
                        //}

                        // 白黒
                        if (is_monotone)
                        {
                            drawBuffer.Monotone();
                        }

                        // セピア
                        if (is_sepia)
                        {
                            drawBuffer.Sepia();
                        }

                        // 夕焼け
                        if (is_sunset)
                        {
                            drawBuffer.Sunset();
                        }

                        // 水中
                        if (is_water)
                        {
                            drawBuffer.Water();
                        }

                        // シルエット
                        if (is_sil)
                        {
                            drawBuffer.Silhouette();
                        }

                        // ネガポジ反転
                        if (negpos)
                        {
                            drawBuffer.NegPosReverse();
                        }

                        // フィルタ
                        if (is_colorfilter)
                        {
                            if (trans_par < 0f)
                            {
                                trans_par = 0.5f;
                            }
                            drawBuffer.ColorFilter(fcolor, trans_par);
                        }

                        //// 明 (多段指定可能)
                        //for (i = 1; i <= bright_count; i++)
                        //    Graphics.Bright(transparent);

                        //// 暗 (多段指定可能)
                        //for (i = 1; i <= dark_count; i++)
                        //    Graphics.Dark(transparent);

                        // 左右反転
                        if (vrev)
                        {
                            drawBuffer.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        }

                        // 上下反転
                        if (hrev)
                        {
                            drawBuffer.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        }

                        // 回転
                        if (angle != 0)
                        {
                            drawBuffer.Rotate(angle, GetDrawBgColor(transparent));
                        }
                    }
                    //EditedPicture:

                    // クリッピング処理
                    if (dw == Constants.DEFAULT_LEVEL)
                    {
                        dw = orig_width;
                    }

                    if (dh == Constants.DEFAULT_LEVEL)
                    {
                        dh = orig_height;
                    }

                    if (permanent)
                    {
                        // 背景描画の場合、センタリングはマップ中央に
                        if (dx == Constants.DEFAULT_LEVEL)
                        {
                            dx = (MapPWidth - dw) / 2;
                        }

                        if (dy == Constants.DEFAULT_LEVEL)
                        {
                            if (Map.IsStatusView)
                            {
                                // XXX
                                dy = (frmMain.MapCellPx * 15 - dh) / 2;
                            }
                            else
                            {
                                dy = (MapPHeight - dh) / 2;
                            }
                        }
                    }
                    // ユニット上で画像のセンタリングを行うことを意図している
                    // 場合は修正が必要
                    else if (Strings.InStr(fname, "EFFECT_") > 0
                        || Strings.InStr(fname, @"スペシャルパワー\") > 0
                        || Strings.InStr(fname, @"精神コマンド\") > 0)
                    {
                        if (dx == Constants.DEFAULT_LEVEL)
                        {
                            dx = (MainPWidth - dw) / 2;
                            if (MainWidth % 2 == 0)
                            {
                                dx = dx - 16;
                            }
                        }

                        if (dy == Constants.DEFAULT_LEVEL)
                        {
                            dy = (MainPHeight - dh) / 2;
                            if (MainHeight % 2 == 0)
                            {
                                dy = dy - 16;
                            }
                        }
                    }
                    else
                    {
                        // 通常描画の場合、センタリングは画面中央に
                        if (dx == Constants.DEFAULT_LEVEL)
                        {
                            dx = (MainPWidth - dw) / 2;
                        }

                        if (dy == Constants.DEFAULT_LEVEL)
                        {
                            dy = (MainPHeight - dh) / 2;
                        }
                    }

                    // 描画先が画面外の場合や描画サイズが0の場合は画像のロードのみを行う
                    if (dx >= g.VisibleClipBounds.Width
                        || dy >= g.VisibleClipBounds.Height
                        || dx + dw <= 0
                        || dy + dh <= 0
                        || dw <= 0
                        || dh <= 0)
                    {
                        //load_only = true;
                        return true;
                    }
                    g.DrawImage(drawBuffer, new Rectangle(dx, dy, dw, dh));

                    //DrewPicture:
                    if (permanent)
                    {
                        // 背景への描き込み
                        Map.IsMapDirty = true;
                        // XXX マスクは動的にやってるから要らん
                        //// マスク入り背景画像画面にも画像を描き込む
                        //ret = BitBlt(withBlock17.picMaskedBack.hDC, dx, dy, dw, dh, pic.hDC, dx, dy, SRCCOPY);
                        //var loopTo15 = ((dx + dw - 1) / 32);
                        //for (i = (dx / 32); i <= loopTo15; i++)
                        //{
                        //    var loopTo16 = ((dy + dh - 1) / 32);
                        //    for (j = (dy / 32); j <= loopTo16; j++)
                        //    {
                        //        ret = BitBlt(withBlock17.picMaskedBack.hDC, 32 * (int)i, 32 * (int)j, 32, 32, withBlock17.picMask.hDC, 0, 0, SRCAND);
                        //        ret = BitBlt(withBlock17.picMaskedBack.hDC, 32 * (int)i, 32 * (int)j, 32, 32, withBlock17.picMask2.hDC, 0, 0, SRCINVERT);
                        //    }
                        //}
                    }
                    else if (!on_msg_window && !on_status_window)
                    {
                        // 表示画像を消去する際に使う描画領域を設定
                        // XXX ダーティエリアは使わない
                        //PaintedAreaX1 = GeneralLib.MinLng(PaintedAreaX1, GeneralLib.MaxLng(dx, 0));
                        //PaintedAreaY1 = GeneralLib.MinLng(PaintedAreaY1, GeneralLib.MaxLng(dy, 0));
                        //PaintedAreaX2 = GeneralLib.MaxLng(PaintedAreaX2, GeneralLib.MinLng(dx + dw, MainPWidth - 1));
                        //PaintedAreaY2 = GeneralLib.MaxLng(PaintedAreaY2, GeneralLib.MinLng(dy + dh, MainPHeight - 1));
                        IsPictureDrawn = true;
                        IsPictureVisible = true;
                        IsCursorVisible = false;
                        if (keep_picture)
                        {
                            // picMain(1)にも描画
                            using (var gBack = Graphics.FromImage(MainForm.MainBufferBack))
                            {
                                gBack.DrawImage(drawBuffer, new Rectangle(dx, dy, dw, dh));
                            }
                        }
                    }
                }
            }
            finally
            {
                g.Dispose();
            }

            return true;
        }

        // ＭＳ Ｐ明朝、16pt、Bold、白色
        private static readonly Font defaultDrawFont = new Font("ＭＳ Ｐ明朝", 16, FontStyle.Bold, GraphicsUnit.Point);
        private static readonly Font statusDrawFont = new Font("ＭＳ Ｐ明朝", 9, FontStyle.Regular, GraphicsUnit.Point);
        private Font currentDrawFont = defaultDrawFont;
        private Brush currentDrawFontColor = Brushes.White;
        private PointF currentDrawStringPoint = new PointF();

        public void ResetDrawString()
        {
            currentDrawFont = defaultDrawFont;
            currentDrawFontColor = Brushes.White;
        }

        public void SetDrawString(DrawStringMode mode)
        {
            switch (mode)
            {
                case DrawStringMode.Status:
                    currentDrawFont = statusDrawFont;
                    currentDrawFontColor = Brushes.White;
                    break;
                default:
                    ResetDrawString();
                    break;
            }
        }

        public void SetDrawFont(DrawFontOption option)
        {
            var style = FontStyle.Regular;
            if (option.Bold) { style |= FontStyle.Bold; }
            if (option.Italic) { style |= FontStyle.Italic; }
            currentDrawFont = new Font(option.FontFamily, option.Size, style);
            if (CurrentPaintColor != option.Color)
            {
                currentDrawFontColor = new SolidBrush(option.Color);
            }
        }
        public void UpdateBaseX(int newX)
        {
            currentDrawStringPoint.X = newX;
        }
        public void UpdateBaseY(int newY)
        {
            currentDrawStringPoint.Y = newY;
        }

        public void DrawString(string msg, int X, int Y, bool without_cr = false)
        {
            Graphics g;
            if (PermanentStringMode)
            {
                // 背景書き込み
                //// 背景への描画
                MainForm.InitBackgroundBufferIfInvalid();
                g = Graphics.FromImage(MainForm.BackgroundBuffer);
            }
            else
            {
                //// マップウィンドウへの通常の描画
                // 通常の書き込み
                g = Graphics.FromImage(MainForm.MainBuffer);
                SaveScreen();
            }

            try
            {
                g.TextRenderingHint = RenderingConfig.TextHint;
                // 現在のX位置を記録しておく
                var prev_cx = currentDrawStringPoint.X;
                float tx = currentDrawStringPoint.X;
                float ty = currentDrawStringPoint.Y;
                var msgSize = g.MeasureStringWithoutRightMargin(msg, currentDrawFont);
                // 書き込み先の座標を求める
                if (HCentering)
                {
                    tx = (g.VisibleClipBounds.Width - msgSize.Width) / 2;
                }
                else if (X != Constants.DEFAULT_LEVEL)
                {
                    tx = X;
                }

                if (VCentering)
                {
                    ty = (g.VisibleClipBounds.Height - msgSize.Height) / 2;
                }
                else if (Y != Constants.DEFAULT_LEVEL)
                {
                    ty = Y;
                }

                g.DrawString(msg, currentDrawFont, currentDrawFontColor, tx, ty);

                // 背景書き込みの場合
                if (PermanentStringMode)
                {
                    Map.IsMapDirty = true;
                }

                // 保持オプション使用時
                if (KeepStringMode)
                {
                    // picMain(1)にも描画
                    using (var gBack = Graphics.FromImage(MainForm.MainBufferBack))
                    {
                        gBack.TextRenderingHint = RenderingConfig.TextHint;
                        gBack.DrawString(msg, currentDrawFont, currentDrawFontColor, tx, ty);
                    }
                }

                // 次回の書き込みのため、X座標位置を設定し直す
                currentDrawStringPoint = new PointF(
                    X != Constants.DEFAULT_LEVEL ? X : prev_cx,
                    without_cr ? ty : ty + currentDrawFont.GetHeight(g));

                if (!PermanentStringMode)
                {
                    IsPictureVisible = true;
                    PaintedAreaX1 = 0;
                    PaintedAreaY1 = 0;
                    PaintedAreaX2 = (MainPWidth - 1);
                    PaintedAreaY2 = (MainPHeight - 1);
                }
            }
            finally
            {
                g.Dispose();
            }
        }

        private Font sysFont = new Font("ＭＳ Ｐ明朝", 9, FontStyle.Bold, GraphicsUnit.Point);
        private Font battleAnimeFont = new Font("ＭＳ Ｐ明朝", 8, FontStyle.Regular, GraphicsUnit.Point);
        private Brush sysFontColor = Brushes.Black;
        private Brush sysFontBackColor = Brushes.White;
        public void DrawSysString(int X, int Y, string msg, bool without_refresh)
        {
            // 表示位置が画面外？
            if (X < MapX - MainWidth / 2 || MapX + MainWidth / 2 < X || Y < MapY - MainHeight / 2 || MapY + MainHeight / 2 < Y)
            {
                return;
            }

            SaveScreen();
            using (var g = Graphics.FromImage(MainForm.MainBuffer))
            {
                g.TextRenderingHint = RenderingConfig.TextHint;
                // フォント設定をシステム用に切り替え
                var font = SRC.BattleAnimation ? battleAnimeFont : sysFont;

                // メッセージの書き込み
                // ここでは描画にあたって既定の余白を取っておく
                var msgSize = g.MeasureString(msg, font);
                var tx = MapToPixelX(X) + (frmMain.MapCellPx - msgSize.Width) / 2 - 1;
                var ty = MapToPixelY(Y + 1) - msgSize.Height;

                g.FillRectangle(sysFontBackColor, tx, ty, msgSize.Width, msgSize.Height);
                g.DrawString(msg, font, sysFontColor, tx, ty);
            }
            // 表示を更新
            if (!without_refresh)
            {
                // XXX 画面更新回り
                MainForm.UpdateScreen();
            }

            PaintedAreaX1 = (short)GeneralLib.MinLng(PaintedAreaX1, MapToPixelX(X) - 4);
            PaintedAreaY1 = (short)GeneralLib.MaxLng(PaintedAreaY1, MapToPixelY(Y) + 16);
            PaintedAreaX2 = (short)GeneralLib.MinLng(PaintedAreaX2, MapToPixelX(X) + 36);
            PaintedAreaY2 = (short)GeneralLib.MaxLng(PaintedAreaY2, MapToPixelY(Y) + 32);
        }

        public SizeF MeasureString(string msg)
        {
            using var g = Graphics.FromImage(MainForm.BackgroundBuffer);
            g.TextRenderingHint = RenderingConfig.TextHint;
            return g.MeasureStringWithoutRightMargin(msg, currentDrawFont);
        }
    }
}
