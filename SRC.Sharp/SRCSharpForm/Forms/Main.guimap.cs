using SRCCore;
using SRCCore.Maps;
using SRCSharpForm.Extensions;
using SRCSharpForm.Resoruces;
using System.Drawing;
using System.Windows.Forms;

namespace SRCSharpForm
{
    // TODO インタフェースの切り方見直す
    internal partial class frmMain : IGUIMap
    {
        public const int MapCellPx = 32;
        private int MapWidth;
        private int MapHeight;
        public int MapPWidth => MapWidth * MapCellPx;
        public int MapPHeight => MapHeight * MapCellPx;
        public int MainPWidth => GUI.MainWidth * MapCellPx;
        public int MainPHeight => GUI.MainHeight * MapCellPx;

        private Pen MapLinePen = new Pen(Color.FromArgb(100, 100, 100));
        //private Brush MapMaskBrush = new HatchBrush(HatchStyle.BackwardDiagonal, Color.FromArgb(0, 0x39, 0x6b));
        //private Brush MapMaskBrush = new SolidBrush(Color.FromArgb(127, 0, 0x39, 0x6b));
        private Brush MapMaskBrush = new SolidBrush(Color.FromArgb(127, 100, 100, 100));
        private Brush UnitMaskBrush = new SolidBrush(Color.FromArgb(127, 100, 100, 100));

        /// <summary>
        /// マップ画面全体のバッファ。
        /// 地形や 背景 オプション指定時の描画先。
        /// <see cref="IGUI.RedrawScreen"/> 時にはこのバッファを元に画面を再描画する。
        /// </summary>
        public Image BackgroundBuffer { get; private set; }
        /// <summary>
        /// 最終的な画面描画バッファ。
        /// <see cref="IGUI.RefreshScreen"/> などではこのバッファを単に画面に転送する。
        /// </summary>        
        public Image MainBuffer { get; private set; }
        /// <summary>
        /// <see cref="SaveScreen"/> で MainBuffer を退避、復元できるようにするための領域。
        /// </summary>
        public Image MainBufferBack { get; private set; }
        /// <summary>
        /// 画面のダブルバッファ背景面。
        /// MainBuffer をベースにこのバッファ上で編集する場面もある。
        /// </summary>
        public Image MainDoubleBuffer { get; private set; }

        /// <summary>
        /// 画像リソースの総合的なバッファ。
        /// ユニットタイル（picNeautral など）以外の画像はこのバッファで取り扱う。
        /// TODO ユニットタイルの読み込み元を変える
        /// </summary>
        private ImageBuffer imageBuffer;
        public ImageBuffer ImageBuffer => imageBuffer;

        public void Init(ImageBuffer imageBuffer)
        {
            InitMainBuffer(1, 1);
            this.imageBuffer = imageBuffer;
        }

        private void InitMainBuffer(int w, int h)
        {
            if (MainBuffer != null) { MainBuffer.Dispose(); }
            if (MainBufferBack != null) { MainBufferBack.Dispose(); }
            if (MainDoubleBuffer != null) { MainDoubleBuffer.Dispose(); }
            MainBuffer = new Bitmap(w, h);
            MainBufferBack = new Bitmap(w, h);
            MainDoubleBuffer = new Bitmap(w, h);
        }

        private void UpdataMain()
        {
            using (var g = Graphics.FromImage(picMain.NewImageIfNull().Image))
            {
                g.DrawImage(MainDoubleBuffer, 0, 0);
            }
            // 画面を更新させる
            picMain.Invalidate();
            Application.DoEvents();
        }

        public void InitBackgroundBufferIfInvalid()
        {
            if (BackgroundBuffer == null)
            {
                InitBackgroundBuffer();
            }
        }

        private void InitBackgroundBuffer()
        {
            if (BackgroundBuffer != null)
            {
                BackgroundBuffer.Dispose();
            }
            BackgroundBuffer = new Bitmap(MapPWidth, MapPHeight);
            using (var g = Graphics.FromImage(BackgroundBuffer))
            {
                g.FillRectangle(Brushes.Black, 0, 0, MapPWidth, MapPHeight);
            }
        }

        public void InitStatus()
        {
            // ステータスウィンドウを設置
            picFace.Location = new Point(MainPWidth + 16 + 4, 4);
            picPilotStatus.Location = new Point(MainPWidth + 16 + 4 + 68 + 4, 4);
            picPilotStatus.Size = new Size(155, 72);
            picUnitStatus.Location = new Point(MainPWidth + 16 + 4, 4 + 68 + 4);
            picUnitStatus.Size = new Size(225 + 5, MainPHeight - 64 + 16);
            //    if (NewGUIMode)
            //    {
            //        withBlock.picUnitStatus.Move(MainPWidth - 230 - 10, 10, 230, MainPHeight - 20);
            //        withBlock.picUnitStatus.Visible = false;
            //        withBlock.picPilotStatus.Visible = false;
            //        withBlock.picFace.Visible = false;
            //        Status.StatusWindowBackBolor = STATUSBACK;
            //        Status.StatusWindowFrameColor = STATUSBACK;
            //        Status.StatusWindowFrameWidth = 1;
            //        withBlock.picUnitStatus.BackColor = Status.StatusWindowBackBolor;
            //        Status.StatusFontColorAbilityName = Information.RGB(0, 0, 150);
            //        Status.StatusFontColorAbilityEnable = ColorTranslator.ToOle(Color.Blue);
            //        Status.StatusFontColorAbilityDisable = Information.RGB(150, 0, 0);
            //        Status.StatusFontColorNormalString = ColorTranslator.ToOle(Color.Black);
            //    }
            //    else
            //    {
            //        withBlock.picFace.Move(MainPWidth + 24, 4);
            //        withBlock.picPilotStatus.Move(MainPWidth + 24 + 68 + 4, 4, 155, 72);
            //        withBlock.picUnitStatus.Move(MainPWidth + 24, 4 + 68 + 4, 225 + 5, MainPHeight - 64 + 16);
            //    }
        }

        public void InitMapSize(int w, int h)
        {
            MapWidth = w;
            MapHeight = h;

            VScrollBar.Visible = false;
            HScrollBar.Visible = false;

            // メインウィンドウの位置＆サイズを設定
            Width = Width - ClientRectangle.Width + (MainPWidth + 16 + 4 + 225 + 4);
            Height = Height - ClientRectangle.Height + (MainPHeight + 16);
            //if (!NewGUIMode)
            //{
            //    withBlock.Width = Width - ClientRectangle.Width + (MainPWidth + 24 + 225 + 4);
            //    withBlock.Height = Height - ClientRectangle.Height + (MainPHeight + 24);
            //}
            //else
            //{
            //    withBlock.Width = Width - ClientRectangle.Width + MainPWidth;
            //    withBlock.Height = Height - ClientRectangle.Height + MainPHeight;
            //}

            // XXX 画面中央に出すなら出す
            //withBlock.Left = (int)SrcFormatter.TwipsToPixelsX((SrcFormatter.PixelsToTwipsX(Screen.PrimaryScreen.Bounds.Width) - SrcFormatter.PixelsToTwipsX(withBlock.Width)) / 2d);
            //withBlock.Top = (int)SrcFormatter.TwipsToPixelsY((SrcFormatter.PixelsToTwipsY(Screen.PrimaryScreen.Bounds.Height) - SrcFormatter.PixelsToTwipsY(withBlock.Height)) / 2d);

            // スクロールバーの位置を設定
            VScrollBar.Location = new Point(MainPHeight, 0);
            VScrollBar.Visible = true;
            VScrollBar.Size = new Size(16, MainPWidth);
            HScrollBar.Location = new Point(0, MainPHeight);
            HScrollBar.Size = new Size(MainPWidth, 16);
            HScrollBar.Visible = true;
            //if (!NewGUIMode)
            //{
            //    withBlock.VScroll.Move(MainPWidth + 4, 4, 16, MainPWidth);
            //    withBlock.HScroll.Move(4, MainPHeight + 4, MainPWidth, 16);
            //}
            //else
            //{
            //    VScrollBar.Visible = false;
            //    HScrollBar.Visible = false;
            //}

            // マップウィンドウのサイズを設定
            picMain.Location = new Point(0, 0);
            picMain.Size = new Size(MainPWidth, MainPHeight);
            InitMainBuffer(MainPWidth, MainPHeight);

            //    if (!NewGUIMode)
            //    {
            //        withBlock.picMain(0).Move(4, 4, MainPWidth, MainPHeight);
            //        withBlock.picMain(1).Move(4, 4, MainPWidth, MainPHeight);
            //    }
            //    else
            //    {
            //        withBlock.picMain(0).Move(0, 0, MainPWidth, MainPHeight);
            //        withBlock.picMain(1).Move(0, 0, MainPWidth, MainPHeight);
            //    }
        }
        public void SetMapSize(int w, int h)
        {
            MapWidth = w;
            MapHeight = h;

            // マップ画像サイズを決定
            InitBackgroundBuffer();

            // スクロールバーの移動範囲を決定
            if (HScrollBar.Maximum != MapWidth)
            {
                HScrollBar.Maximum = MapWidth;
                //if (!GUI.NewGUIMode)
                //{
                //    withBlock2.Visible = true;
                //}
            }
            if (VScrollBar.Maximum != MapHeight)
            {
                VScrollBar.Maximum = MapHeight;
                //if (!GUI.NewGUIMode)
                //{
                //    withBlock2.Visible = true;
                //}
            }
        }

        public void SetupBackground(string draw_mode, string draw_option, Color filter_color, double filter_trans_par)
        {
            Map.MapDrawMode = draw_mode;
            Map.MapDrawFilterColor = filter_color;
            Map.MapDrawFilterTransPercent = filter_trans_par;

            Map.IsMapDirty = false;
            GUI.IsPictureVisible = false;
            GUI.IsCursorVisible = false;

            // TODO Impl SetupBackground
            using (var g = Graphics.FromImage(BackgroundBuffer))
            {
                switch (draw_option ?? "")
                {
                    case "ステータス":
                        g.FillRectangle(Brushes.Black, 0, 0, BackgroundBuffer.Width, BackgroundBuffer.Height);
                        return;

                    default:
                        GUI.MapX = (GUI.MainWidth / 2 + 1);
                        GUI.MapY = (GUI.MainHeight / 2 + 1);
                        break;
                }

                // 各マスのマップ画像を表示
                for (var x = 1; x <= Map.MapWidth; x++)
                {
                    for (var y = 1; y <= Map.MapHeight; y++)
                    {
                        var cell = Map.MapData[x, y];
                        var xpx = (x - 1) * MapCellPx;
                        var ypx = (y - 1) * MapCellPx;

                        // 画像を描き込み
                        var bitmapPath = Map.SearchTerrainImageFile(cell);
                        if (!string.IsNullOrEmpty(bitmapPath))
                        {
                            g.DrawImage(ImageBuffer.Get(bitmapPath), xpx, ypx);
                        }
                        else
                        {
                            g.FillRectangle(Brushes.Black, xpx, ypx, MapCellPx, MapCellPx);
                        }
                        //g.DrawString($"{cell.TerrainType}", SystemFonts.DefaultFont, Brushes.Gray, xpx + 2, ypx + 2);
                    }
                }
            }

            // マップ設定によって表示色を変更
            switch (draw_mode ?? "")
            {
                case "夜":
                    BackgroundBuffer.Dark();
                    break;

                case "セピア":
                    BackgroundBuffer.Sepia();
                    break;

                case "白黒":
                    BackgroundBuffer.Monotone();
                    break;

                case "夕焼け":
                    BackgroundBuffer.Sunset();
                    break;

                case "水中":
                    BackgroundBuffer.Water();
                    break;

                case "フィルタ":
                    BackgroundBuffer.ColorFilter(filter_color, (float)filter_trans_par);
                    break;
            }

            // マス目の表示
            if (SRC.ShowSquareLine)
            {
                using (var g = Graphics.FromImage(BackgroundBuffer))
                {
                    g.DrawRectangle(MapLinePen, 0, 0, MapPWidth, MapPHeight);
                    for (var x = 1; x <= Map.MapWidth - 1; x++)
                    {
                        g.DrawLine(MapLinePen, x * MapCellPx, 0, x * MapCellPx, MapPHeight);
                    }
                    for (var y = 1; y <= Map.MapHeight - 1; y++)
                    {
                        g.DrawLine(MapLinePen, 0, y * MapCellPx, MapPWidth, y * MapCellPx);
                    }
                }
            }

            // 画面を更新
            if (!Map.IsStatusView && string.IsNullOrEmpty(draw_option))
            {
                GUI.RefreshScreen();
            }
        }

        public void ClearScrean()
        {
            // XXX _picMain_0 picturebox じゃなくて panel なんだけど
            using (var g = Graphics.FromImage(MainBuffer))
            {
                // マップウィンドウの内容を消去
                g.FillRectangle(Brushes.Black, 0, 0, MainPWidth, MainPHeight);
            }
            UpdateScreen();
            GUI.ScreenIsSaved = true;
        }

        public void RefreshScreen(int mapX, int mapY, bool without_refresh, bool delay_refresh)
        {
            // XXX _picMain_0 picturebox じゃなくて panel なんだけど
            using (var g = Graphics.FromImage(MainBuffer))
            {
                if (!without_refresh)
                {
                    GUI.IsPictureVisible = false;
                    GUI.IsCursorVisible = false;
                    // マップウィンドウのスクロールバーの位置を変更
                    if (!GUI.IsGUILocked)
                    {
                        if (HScrollBar.Value != GUI.MapX)
                        {
                            HScrollBar.Value = GUI.MapX;
                            return;
                        }
                        if (VScrollBar.Value != GUI.MapY)
                        {
                            VScrollBar.Value = GUI.MapY;
                            return;
                        }
                    }
                }

                // マップ画像の転送元と転送先を計算する
                var mx = (mapX - (GUI.MainWidth + 1) / 2 + 1);
                var my = (mapY - (GUI.MainHeight + 1) / 2 + 1);
                int sx, sy, dx, dy, dw, dh;

                if (mx < 1)
                {
                    sx = 1;
                    dx = (2 - mx);
                    dw = (GUI.MainWidth - (1 - mx));
                }
                else if (mx + GUI.MainWidth - 1 > Map.MapWidth)
                {
                    sx = mx;
                    dx = 1;
                    dw = (GUI.MainWidth - (mx + GUI.MainWidth - 1 - Map.MapWidth));
                }
                else
                {
                    sx = mx;
                    dx = 1;
                    dw = GUI.MainWidth;
                }

                if (dw > GUI.MainWidth)
                {
                    dw = GUI.MainWidth;
                }

                if (my < 1)
                {
                    sy = 1;
                    dy = (2 - my);
                    dh = (GUI.MainHeight - (1 - my));
                }
                else if (my + GUI.MainHeight - 1 > Map.MapHeight)
                {
                    sy = my;
                    dy = 1;
                    dh = (GUI.MainHeight - (my + GUI.MainHeight - 1 - Map.MapHeight));
                }
                else
                {
                    sy = my;
                    dy = 1;
                    dh = GUI.MainHeight;
                }

                if (dh > GUI.MainHeight)
                {
                    dh = GUI.MainHeight;
                }

                // 一旦マップウィンドウの内容を消去
                g.FillRectangle(Brushes.Black, 0, 0, MainPWidth, MainPHeight);

                // 表示内容を更新
                for (var i = 0; i < dw; i++)
                {
                    var xx = MapCellPx * (dx + i - 1);
                    for (var j = 0; j < dh; j++)
                    {
                        if (sx + i < 1 || (sx + i) > Map.MapWidth || sy + j < 1 || (sy + j) > Map.MapHeight)
                        {
                            continue;
                        }

                        var yy = MapCellPx * (dy + j - 1);
                        var cell = Map.MapData[sx + i, sy + j];
                        var u = Map.MapDataForUnit[sx + i, sy + j];

                        var destRect = new Rectangle(xx, yy, MapCellPx, MapCellPx);
                        var fromRect = new Rectangle(MapCellPx * (sx + i - 1), MapCellPx * (sy + j - 1), MapCellPx, MapCellPx);
                        // 地形
                        // XXX これバルクで転送でいいんじゃないかな。バッファの関係だろうか。ダブルバッファすればいい？
                        g.DrawImage(BackgroundBuffer, destRect, fromRect, GraphicsUnit.Pixel);

                        if (u != null)
                        {
                            DrawUnit(g, cell, u, destRect);
                        }

                        // マスク
                        if (GUI.ScreenIsMasked)
                        {
                            if (Map.MaskData[cell.X, cell.Y])
                            {
                                g.FillRectangle(MapMaskBrush, destRect);
                            }
                        }
                        // XXX マスク
                        //// マスク入りの画像を作成
                        //ret = BitBlt(withBlock.picUnitBitmap.hDC, xx, (int)yy + 64, 32, 32, withBlock.picUnitBitmap.hDC, xx, (int)yy + 32, SRCCOPY);
                        //ret = BitBlt(withBlock.picUnitBitmap.hDC, xx, (int)yy + 64, 32, 32, withBlock.picMask2.hDC, 0, 0, SRCINVERT);
                    }
                }

                //// 描画色を元に戻しておく
                //pic.ForeColor = ColorTranslator.FromOle(prev_color);

            }
            // 画面が書き換えられたことを記録
            GUI.ScreenIsSaved = false;
            if (!without_refresh && !delay_refresh)
            {
                UpdateScreen();
            }
        }

        public void UpdateScreen()
        {
            if (Visible)
            {
                using (var g = Graphics.FromImage(MainDoubleBuffer))
                {
                    g.DrawImage(MainBuffer, 0, 0);
                }
                UpdataMain();
            }
        }

        public void DrawUnit(Graphics g, MapCell cell, SRCCore.Units.Unit u, Rectangle destRect)
        {
            using var image = DrawUnit(cell, u);
            if (image != null)
            {
                g.DrawImage(image, destRect);
            }
        }

        private Image DrawUnit(MapCell cell, SRCCore.Units.Unit u, bool use_orig_color = false)
        {
            var image = new Bitmap(MapCellPx, MapCellPx);
            var destRect = new Rectangle(0, 0, MapCellPx, MapCellPx);
            var emit_light = false;

            // ユニットが自分で発光しているかをあらかじめチェック
            if (Map.MapDrawMode == "夜" && !Map.MapDrawIsMapOnly && !use_orig_color && u.IsFeatureAvailable("発光"))
            {
                emit_light = true;
            }

            using var g = Graphics.FromImage(image);
            // タイル
            switch (u.Party0 ?? "")
            {
                case "味方":
                case "ＮＰＣ":
                    g.DrawImage(picUnit.Image, destRect);
                    break;
                case "敵":
                    g.DrawImage(picEnemy.Image, destRect);
                    break;
                case "中立":
                    g.DrawImage(picNeautral.Image, destRect);
                    break;
            }

            var unitImage = ImageBuffer.GetTransparent(u.CurrentForm().CurrentBitmap());
            // (発光している場合は２度塗りを防ぐため描画しない)
            if (unitImage != null && !emit_light)
            {
                g.DrawImage(unitImage, destRect);
            }

            // TODO Impl フィルタ
            // フィルタ
            //            if (u.IsFeatureAvailable(ref "地形ユニット"))
            //            {
            //                // 地形ユニットの場合は画像をそのまま使う
            //                ret = BitBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picTmp32(0).hDC, 0, 0, SRCCOPY);
            //            }
            //            else
            //            {
            //                // BitBltを使ってユニット画像とタイルを重ね合わせる

            //                // マスクを作成
            //                Graphics.MakeMask(ref withBlock.picTmp32(0).hDC, ref withBlock.picTmp32(2).hDC, ref 32, ref 32, ref ColorTranslator.ToOle(Color.White));


            //                // 画像の重ね合わせ
            //                // (発光している場合は２度塗りを防ぐため描画しない)
            //                if (!emit_light)
            //                {
            //                    ret = BitBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picTmp32(2).hDC, 0, 0, SRCERASE);
            //                    ret = BitBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picTmp32(0).hDC, 0, 0, SRCINVERT);
            //                }
            //            }
            // 色をステージの状況に合わせて変更
            if (!use_orig_color && !Map.MapDrawIsMapOnly)
            {
                switch (Map.MapDrawMode ?? "")
                {
                    case "夜":
                        image.Dark();
                        // ユニットが"発光"の特殊能力を持つ場合、
                        // ユニット画像を、暗くしたタイル画像の上に描画する。
                        if (emit_light)
                        {
                            if (unitImage != null)
                            {
                                g.DrawImage(unitImage, destRect);
                            }
                        }
                        break;

                    case "セピア":
                        image.Sepia();
                        break;

                    case "白黒":
                        image.Monotone();
                        break;

                    case "夕焼け":
                        image.Sunset();
                        break;

                    case "水中":
                        image.Water();
                        break;

                    case "フィルタ":
                        image.ColorFilter(Map.MapDrawFilterColor, (float)Map.MapDrawFilterTransPercent);
                        break;
                }
            }

            // 行動済のフィルタ
            if (u.Action <= 0 && !u.IsFeatureAvailable("地形ユニット"))
            {
                // 行動済のユニット
                g.FillRectangle(UnitMaskBrush, destRect);
            }

            // ユニットのいる場所に合わせて表示を変更
            var unitAreaPen = Pens.Black;
            switch (u.Area ?? "")
            {
                case "空中":
                    // XXX Cellサイズに対応する
                    g.DrawLine(unitAreaPen,
                        destRect.Left, destRect.Top + 28,
                        destRect.Left + 31, destRect.Top + 28);
                    break;

                case "水中":
                    g.DrawLine(unitAreaPen,
                        destRect.Left, destRect.Top + 3,
                        destRect.Left + 31, destRect.Top + 3);
                    break;

                case "地中":
                    g.DrawLine(unitAreaPen,
                        destRect.Left, destRect.Top + 28,
                        destRect.Left + 31, destRect.Top + 28);
                    g.DrawLine(unitAreaPen,
                        destRect.Left, destRect.Top + 3,
                        destRect.Left + 31, destRect.Top + 3);
                    break;

                case "宇宙":
                    if (cell.Terrain.Class == "月面")
                    {
                        g.DrawLine(unitAreaPen,
                            destRect.Left, destRect.Top + 28,
                            destRect.Left + 31, destRect.Top + 28);
                    }
                    break;
            }

            return image;
        }

        public void DisplayGlobalMap()
        {
            int mwidth, mheight;

            // マップの縦横の比率を元に縮小マップの大きさを決める
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
            using (var buf = new Bitmap(Map.MapWidth * MapCellPx, Map.MapHeight * MapCellPx))
            using (var bufG = Graphics.FromImage(buf))
            using (var g = Graphics.FromImage(MainDoubleBuffer))
            {
                // マップの全体画像を作成
                bufG.DrawImage(BackgroundBuffer, 0, 0);
                for (var i = 0; i < Map.MapWidth; i++)
                {
                    var xx = MapCellPx * i;
                    for (var j = 0; j < Map.MapHeight; j++)
                    {
                        var yy = MapCellPx * j;
                        var cell = Map.MapData[1 + i, 1 + j];
                        var u = Map.MapDataForUnit[1 + i, 1 + j];

                        if (u != null)
                        {
                            var destRect = new Rectangle(xx, yy, MapCellPx, MapCellPx);
                            var fromRect = new Rectangle(MapCellPx * i, MapCellPx * j, MapCellPx, MapCellPx);
                            DrawUnit(bufG, cell, u, destRect);
                        }
                    }
                }

                // 見やすいように背景を設定
                g.FillRectangle(Brushes.Black, 0, 0, MainPWidth, MainPHeight);

                // マップ全体を縮小して描画
                g.DrawImage(buf,
                    new Rectangle((MapPWidth - mwidth) / 2, (MapPHeight - mheight) / 2, mheight, mwidth),
                    new Rectangle(0, 0, buf.Width, buf.Height),
                    GraphicsUnit.Pixel);
            }
            UpdataMain();
        }

        private bool IsInsideWindow(int x, int y)
        {
            return x >= GUI.MapX - (GUI.MainWidth + 1) / 2
                || GUI.MapX + (GUI.MainWidth + 1) / 2 >= x
                || y >= GUI.MapY - (GUI.MainHeight + 1) / 2
                || GUI.MapY + (GUI.MainHeight + 1) / 2 >= y;
        }
    }
}
