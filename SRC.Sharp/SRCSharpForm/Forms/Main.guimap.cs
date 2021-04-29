using SRCCore;
using SRCCore.Maps;
using SRCSharpForm.Resoruces;
using System.Drawing;

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

        private Bitmap BackBitmap;
        private Bitmap MaskedBackBitmap;

        private Pen MapLinePen = new Pen(Color.FromArgb(100, 100, 100));
        //private Brush MapMaskBrush = new HatchBrush(HatchStyle.BackwardDiagonal, Color.FromArgb(0, 0x39, 0x6b));
        //private Brush MapMaskBrush = new SolidBrush(Color.FromArgb(127, 0, 0x39, 0x6b));
        private Brush MapMaskBrush = new SolidBrush(Color.FromArgb(127, 100, 100, 100));
        private Brush UnitMaskBrush = new SolidBrush(Color.FromArgb(127, 100, 100, 100));

        private Image mainBuffer;
        private Image mainBufferBack;
        private ImageBuffer imageBuffer;

        public void Init(ImageBuffer imageBuffer)
        {
            InitMainBuffer(1, 1);
            this.imageBuffer = imageBuffer;
        }

        private void InitMainBuffer(int w, int h)
        {
            mainBuffer = new Bitmap(w, h);
            mainBufferBack = new Bitmap(w, h);
        }

        public Image MainBuffer => mainBuffer;
        public Image MainBufferBack => mainBufferBack;

        public void InitStatus()
        {
            // ステータスウィンドウを設置
            picFace.Location = new Point(MainPWidth + 24, 4);
            picPilotStatus.Location = new Point(MainPWidth + 24 + 68 + 4, 4);
            picPilotStatus.Size = new Size(155, 72);
            picUnitStatus.Location = new Point(MainPWidth + 24, 4 + 68 + 4);
            picUnitStatus.Size = new Size(225 + 5, MainPHeight - 64 + 16);
            // TODO Impl
            //    // MOD START MARGE
            //    // If MainWidth = 15 Then
            //    // .picFace.Move MainPWidth + 24, 4
            //    // .picPilotStatus.Move MainPWidth + 24 + 68 + 4, 4, 155, 72
            //    // .picUnitStatus.Move MainPWidth + 24, 4 + 68 + 4, _
            //    // '                225 + 5, MainPHeight - 64 + 16
            //    // Else
            //    // .picUnitStatus.Move MainPWidth - 230 - 10, 10, 230, MainPHeight - 20
            //    // .picUnitStatus.Visible = False
            //    // .picPilotStatus.Visible = False
            //    // .picFace.Visible = False
            //    // End If
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
            //    // MOD END MARGE

        }

        public void InitMapSize(int w, int h)
        {
            MapWidth = w;
            MapHeight = h;

            VScrollBar.Visible = false;
            HScrollBar.Visible = false;
            Width = Width - ClientRectangle.Width + (MainPWidth + 24 + 225 + 4);
            Height = Height - ClientRectangle.Height + (MainPHeight + 24);

            // TODO Impl オフセットあるのダルいから原点0にしようかな。
            ////{
            ////    var withBlock = MainForm;
            //// メインウィンドウの位置＆サイズを設定
            //// If MainWidth = 15 Then
            //if (!NewGUIMode)
            //{
            //    // MOD END MARGE
            //    withBlock.Width = Width - ClientRectangle.Width + (MainPWidth + 24 + 225 + 4);
            //    withBlock.Height = Height - ClientRectangle.Height + (MainPHeight + 24);
            //}
            //else
            //{
            //    withBlock.Width = Width - ClientRectangle.Width + MainPWidth;
            //    withBlock.Height = Height - ClientRectangle.Height + MainPHeight;
            //}

            //// TODO 画面中央に出す？
            ////withBlock.Left = (int)SrcFormatter.TwipsToPixelsX((SrcFormatter.PixelsToTwipsX(Screen.PrimaryScreen.Bounds.Width) - SrcFormatter.PixelsToTwipsX(withBlock.Width)) / 2d);
            ////withBlock.Top = (int)SrcFormatter.TwipsToPixelsY((SrcFormatter.PixelsToTwipsY(Screen.PrimaryScreen.Bounds.Height) - SrcFormatter.PixelsToTwipsY(withBlock.Height)) / 2d);

            // スクロールバーの位置を設定
            VScrollBar.Location = new Point(MainPHeight, 0);
            VScrollBar.Visible = true;
            VScrollBar.Size = new Size(16, MainPWidth);
            HScrollBar.Location = new Point(0, MainPHeight);
            HScrollBar.Size = new Size(MainPWidth, 16);
            HScrollBar.Visible = true;
            //// MOD START MARGE
            //// If MainWidth = 15 Then
            //if (!NewGUIMode)
            //{
            //    // MOD END MARGE
            //    withBlock.VScroll.Move(MainPWidth + 4, 4, 16, MainPWidth);
            //    withBlock.HScroll.Move(4, MainPHeight + 4, MainPWidth, 16);
            //}
            //else
            //{
            //    VScrollBar.Visible = false;
            //    HScrollBar.Visible = false;
            //}

            // マップウィンドウのサイズを設定
            _picMain_0.Location = new Point(0, 0);
            _picMain_0.Size = new Size(MainPWidth, MainPHeight);
            _picMain_1.Location = new Point(0, 0);
            _picMain_1.Size = new Size(MainPWidth, MainPHeight);
            InitMainBuffer(MainPWidth, MainPHeight);

            //    // MOD START MARGE
            //    // If MainWidth = 15 Then
            //    if (!NewGUIMode)
            //    {
            //        // MOD END MARGE
            //        withBlock.picMain(0).Move(4, 4, MainPWidth, MainPHeight);
            //        withBlock.picMain(1).Move(4, 4, MainPWidth, MainPHeight);
            //    }
            //    else
            //    {
            //        withBlock.picMain(0).Move(0, 0, MainPWidth, MainPHeight);
            //        withBlock.picMain(1).Move(0, 0, MainPWidth, MainPHeight);
            //    }
            //}
        }
        public void SetMapSize(int w, int h)
        {
            MapWidth = w;
            MapHeight = h;

            // マップ画像サイズを決定
            picBack.Location = new Point(0, 0);
            picBack.Size = new Size(MapPWidth, MapPHeight);
            BackBitmap = new Bitmap(MapPWidth, MapPHeight);
            using (var g = Graphics.FromImage(BackBitmap))
            {
                g.FillRectangle(Brushes.Black, 0, 0, MapPWidth, MapPHeight);
            }
            picBack.Image = BackBitmap;

            picMaskedBack.Location = new Point(0, 0);
            picMaskedBack.Size = new Size(MapPWidth, MapPHeight);
            MaskedBackBitmap = new Bitmap(MapPWidth, MapPHeight);
            using (var g = Graphics.FromImage(MaskedBackBitmap))
            {
                g.FillRectangle(Brushes.Transparent, 0, 0, MapPWidth, MapPHeight);
            }
            picMaskedBack.Image = MaskedBackBitmap;

            // スクロールバーの移動範囲を決定
            if (HScrollBar.Maximum != MapWidth)
            {
                HScrollBar.Maximum = MapWidth;
                // If MainWidth = 15 Then
                //if (!GUI.NewGUIMode)
                //{
                //    // MOD  END  240a
                //    withBlock2.Visible = true;
                //}
            }
            if (VScrollBar.Maximum != MapHeight)
            {
                VScrollBar.Maximum = MapHeight;
                // If MainWidth = 15 Then
                //if (!GUI.NewGUIMode)
                //{
                //    // MOD  END  240a
                //    withBlock2.Visible = true;
                //}
            }
        }
        public void SetupBackground(string draw_mode, string draw_option, int filter_color, double filter_trans_par)
        {
            Map.IsMapDirty = false;
            GUI.IsPictureVisible = false;
            GUI.IsCursorVisible = false;

            // TODO Impl
            using (var g = Graphics.FromImage(picBack.Image))
            {
                switch (draw_option ?? "")
                {
                    case "ステータス":
                        g.FillRectangle(Brushes.Black, 0, 0, picBack.Image.Width, picBack.Image.Height);
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
                            // XXX 初回だけとはいえキャッシュはしたほうがいいやろな
                            g.DrawImage(Image.FromFile(bitmapPath), xpx, ypx);
                        }
                        else
                        {
                            g.FillRectangle(Brushes.Black, xpx, ypx, MapCellPx, MapCellPx);
                        }
                        //g.DrawString($"{cell.TerrainType}", SystemFonts.DefaultFont, Brushes.Gray, xpx + 2, ypx + 2);
                    }
                }

                // マス目の表示
                //if (SRC.ShowSquareLine)
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
            using (var g = Graphics.FromImage(mainBuffer))
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
            using (var g = Graphics.FromImage(mainBuffer))
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
                        if (sx + i < 1 | (sx + i) > Map.MapWidth | sy + j < 1 | (sy + j) > Map.MapHeight)
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
                        g.DrawImage(picBack.Image, destRect, fromRect, GraphicsUnit.Pixel);

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
            GUI.ScreenIsSaved = false;
            if (Visible)
            {
                using (var g = _picMain_0.CreateGraphics())
                {
                    g.DrawImage(mainBuffer, 0, 0);
                }
                _picMain_0.Update();
            }
        }

        public void DrawUnit(Graphics g, MapCell cell, SRCCore.Units.Unit u, Rectangle destRect)
        {
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

            // XXX BitmapMissing
            // TODO 対象画像の解決
            var image = imageBuffer.GetTransparent(SRC.FileSystem.PathCombine("Unit", u.CurrentForm().Data.Bitmap));
            if (image != null)
            {
                g.DrawImage(image, destRect);
            }
            else
            {
                u.CurrentForm().Data.IsBitmapMissing = true;
            }

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
            //                // 色をステージの状況に合わせて変更
            //                if (!use_orig_color & !Map.MapDrawIsMapOnly)
            //{
            //    switch (Map.MapDrawMode ?? "")
            //    {
            //        case "夜":
            //            {
            //                Graphics.GetImage(ref withBlock.picTmp32(1));
            //                Graphics.Dark();
            //                Graphics.SetImage(ref withBlock.picTmp32(1));
            //                // ユニットが"発光"の特殊能力を持つ場合、
            //                // ユニット画像を、暗くしたタイル画像の上に描画する。
            //                if (emit_light)
            //                {
            //                    if (SRC.UseTransparentBlt)
            //                    {
            //                        ret = TransparentBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picTmp32(0).hDC, 0, 0, 32, 32, ColorTranslator.ToOle(Color.White));
            //                    }
            //                    else
            //                    {
            //                        ret = BitBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picTmp32(2).hDC, 0, 0, SRCERASE);
            //                        ret = BitBlt(withBlock.picTmp32(1).hDC, 0, 0, 32, 32, withBlock.picTmp32(0).hDC, 0, 0, SRCINVERT);
            //                    }
            //                }

            //                break;
            //            }

            //        case "セピア":
            //            {
            //                Graphics.GetImage(ref withBlock.picTmp32(1));
            //                Graphics.Sepia();
            //                Graphics.SetImage(ref withBlock.picTmp32(1));
            //                break;
            //            }

            //        case "白黒":
            //            {
            //                Graphics.GetImage(ref withBlock.picTmp32(1));
            //                Graphics.Monotone();
            //                Graphics.SetImage(ref withBlock.picTmp32(1));
            //                break;
            //            }

            //        case "夕焼け":
            //            {
            //                Graphics.GetImage(ref withBlock.picTmp32(1));
            //                Graphics.Sunset();
            //                Graphics.SetImage(ref withBlock.picTmp32(1));
            //                break;
            //            }

            //        case "水中":
            //            {
            //                Graphics.GetImage(ref withBlock.picTmp32(1));
            //                Graphics.Water();
            //                Graphics.SetImage(ref withBlock.picTmp32(1));
            //                break;
            //            }

            //        case "フィルタ":
            //            {
            //                Graphics.GetImage(ref withBlock.picTmp32(1));
            //                Graphics.ColorFilter(ref Map.MapDrawFilterColor, ref Map.MapDrawFilterTransPercent);
            //                Graphics.SetImage(ref withBlock.picTmp32(1));
            //                break;
            //            }
            //    }
            //}

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
            using (var g = _picMain_0.CreateGraphics())
            {
                // マップの全体画像を作成
                bufG.DrawImage(picBack.Image, 0, 0);
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
            _picMain_0.Update();
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
