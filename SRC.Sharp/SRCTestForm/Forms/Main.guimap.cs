using SRCCore;
using SRCTestForm.Resoruces;
using System.Drawing;
using System.Windows.Forms;

namespace SRCTestForm
{
    // TODO インタフェースの切り方見直す
    internal partial class frmMain : IGUIMap
    {
        const int MapCellPx = 32;
        private int MapWidth;
        private int MapHeight;
        private int MapPWidth => MapWidth * MapCellPx;
        private int MapPHeight => MapHeight * MapCellPx;
        private int MainPWidth => GUI.MainWidth * MapCellPx;
        private int MainPHeight => GUI.MainHeight * MapCellPx;

        private Bitmap BackBitmap;
        private Bitmap MaskedBackBitmap;

        private Pen MapLinePen = new Pen(Color.FromArgb(100, 100, 100));

        private ImageBuffer imageBuffer;

        public void Init()
        {
            imageBuffer = new ImageBuffer(SRC);
        }

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
            //        // UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //        withBlock.picUnitStatus.Move(MainPWidth - 230 - 10, 10, 230, MainPHeight - 20);
            //        // UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //        withBlock.picUnitStatus.Visible = false;
            //        // UPGRADE_ISSUE: Control picPilotStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //        withBlock.picPilotStatus.Visible = false;
            //        // UPGRADE_ISSUE: Control picFace は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //        withBlock.picFace.Visible = false;
            //        Status.StatusWindowBackBolor = STATUSBACK;
            //        Status.StatusWindowFrameColor = STATUSBACK;
            //        Status.StatusWindowFrameWidth = 1;
            //        // UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //        withBlock.picUnitStatus.BackColor = Status.StatusWindowBackBolor;
            //        Status.StatusFontColorAbilityName = Information.RGB(0, 0, 150);
            //        Status.StatusFontColorAbilityEnable = ColorTranslator.ToOle(Color.Blue);
            //        Status.StatusFontColorAbilityDisable = Information.RGB(150, 0, 0);
            //        Status.StatusFontColorNormalString = ColorTranslator.ToOle(Color.Black);
            //    }
            //    else
            //    {
            //        // UPGRADE_ISSUE: Control picFace は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //        withBlock.picFace.Move(MainPWidth + 24, 4);
            //        // UPGRADE_ISSUE: Control picPilotStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //        withBlock.picPilotStatus.Move(MainPWidth + 24 + 68 + 4, 4, 155, 72);
            //        // UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
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
            ////withBlock.Left = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX((Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(Screen.PrimaryScreen.Bounds.Width) - Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock.Width)) / 2d);
            ////withBlock.Top = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY((Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(Screen.PrimaryScreen.Bounds.Height) - Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock.Height)) / 2d);

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
            //    // UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //    withBlock.VScroll.Move(MainPWidth + 4, 4, 16, MainPWidth);
            //    // UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
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
            //    // MOD START MARGE
            //    // If MainWidth = 15 Then
            //    if (!NewGUIMode)
            //    {
            //        // MOD END MARGE
            //        // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //        withBlock.picMain(0).Move(4, 4, MainPWidth, MainPHeight);
            //        // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //        withBlock.picMain(1).Move(4, 4, MainPWidth, MainPHeight);
            //    }
            //    else
            //    {
            //        // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //        withBlock.picMain(0).Move(0, 0, MainPWidth, MainPHeight);
            //        // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
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
                //    // UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
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
                //    // UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //    withBlock2.Visible = true;
                //}
            }
        }
        public void SetupBackground(string draw_mode, string draw_option, int filter_color, double filter_trans_par)
        {
            // TODO Impl
            using (var g = Graphics.FromImage(picBack.Image))
            {
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
        }

        public void RefreshScreen(int mapX, int mapY)
        {
            // XXX _picMain_0 picturebox じゃなくて panel なんだけど
            using (var g = _picMain_0.CreateGraphics())
            {
                //// マップウィンドウのスクロールバーの位置を変更
                //if (!IsGUILocked)
                //{
                //    // UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //    if (withBlock.HScroll.Value != MapX)
                //    {
                //        // UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //        withBlock.HScroll.Value = MapX;
                //        return;
                //    }
                //    // UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //    if (withBlock.VScroll.Value != MapY)
                //    {
                //        // UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //        withBlock.VScroll.Value = MapY;
                //        return;
                //    }
                //}
                // 一旦マップウィンドウの内容を消去
                g.FillRectangle(Brushes.Black, 0, 0, MainPWidth, MainPHeight);

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

                // 表示内容を更新
                // TODO マスク
                //if (!ScreenIsMasked)
                //{
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
                        // XXX ユニット画像は専用のPictureboxをバッファにしてタイル状に並べていたみたい
                        //ret = BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picUnitBitmap.hDC, 32 * ((int)u.BitmapID % 15), 96 * ((int)u.BitmapID / 15), SRCCOPY);

                        var destRect = new Rectangle(xx, yy, MapCellPx, MapCellPx);
                        var fromRect = new Rectangle(MapCellPx * (sx + i - 1), MapCellPx * (sy + j - 1), MapCellPx, MapCellPx);
                        // 地形
                        // XXX これバルクで転送でいいんじゃないかな。バッファの関係だろうか。ダブルバッファすればいい？
                        g.DrawImage(picBack.Image, destRect, fromRect, GraphicsUnit.Pixel);

                        if (u != null)
                        {
                            // XXX BitmapMissing
                            var image = imageBuffer.Get("Unit", u.CurrentForm().Data.Bitmap);
                            if (image != null)
                            {
                                g.DrawImage(image, destRect);
                            }
                            else
                            {
                                u.CurrentForm().Data.IsBitmapMissing = true;
                            }
                        }
                    }
                }

                //// 描画色を元に戻しておく
                //pic.ForeColor = ColorTranslator.FromOle(prev_color);

                //// 画面が書き換えられたことを記録
                //ScreenIsSaved = false;
                //if (!without_refresh & !delay_refresh)
                //{
                //    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                //    withBlock.picMain(0).Refresh();
                //}
            }
        }
    }
}
