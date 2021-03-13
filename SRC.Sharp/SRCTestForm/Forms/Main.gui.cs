using SRCCore;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Units;
using SRCTestForm.Resoruces;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SRCTestForm
{
    internal partial class frmMain
    {
        public void SaveScreen()
        {
            using (var g = Graphics.FromImage(mainBufferBack))
            {
                g.DrawImage(mainBuffer, 0, 0);
            }
        }
        public void EraseUnitBitmap(int X, int Y, bool do_refresh)
        {
            // 画面外？

            if (!IsInsideWindow(X, Y))
            {
                return;
            }

            // 画面が乱れるので書き換えない？
            if (GUI.IsPictureVisible)
            {
                return;
            }

            SaveScreen();

            // 画面表示変更
            EraseMapPoint(X, Y);
            if (do_refresh)
            {
                UpdateScreen();
            }
        }

        private void EraseMapPoint(int X, int Y)
        {
            using (var g = Graphics.FromImage(mainBuffer))
            {
                var xx = GUI.MapToPixelX(X);
                var yy = GUI.MapToPixelY(Y);
                var sourceRect = new Rectangle(xx, yy, MapCellPx, MapCellPx);
                var destRect = new Rectangle((X - 1) * MapCellPx, (Y - 1) * MapCellPx, MapCellPx, MapCellPx);
                g.DrawImage(picBack.Image, sourceRect, destRect, GraphicsUnit.Pixel);
            }
        }

        public void MoveUnitBitmap(Unit u, int x1, int y1, int x2, int y2, int wait_time0, int division)
        {
            int start_time = 0;
            int cur_time = 0;
            int wait_time = wait_time0 / division;
            SaveScreen();

            using (var unitImage = new Bitmap(MapCellPx, MapCellPx))
            {
                // ユニット画像を作成
                using (var g = Graphics.FromImage(unitImage))
                {
                    DrawUnit(g, Map.CellAtPoint(u.x, u.y), u, new Rectangle(0, 0, unitImage.Width, unitImage.Height));
                }

                // 移動の始点を設定
                var xx = GUI.MapToPixelX(x1);
                var yy = GUI.MapToPixelY(y1);
                var vx = 0;
                var vy = 0;

                // 背景上の画像をまず消去
                // (既に移動している場合を除く)
                if (ReferenceEquals(u, Map.MapDataForUnit[x1, y1]))
                {
                    EraseUnitBitmap(x1, y1, false);
                }

                // 最初の移動方向を設定
                if (Math.Abs((x2 - x1)) > Math.Abs((y2 - y1)))
                {
                    if (x2 > x1)
                    {
                        vx = 1;
                    }
                    else
                    {
                        vx = -1;
                    }

                    vy = 0;
                }
                else
                {
                    if (y2 > y1)
                    {
                        vy = 1;
                    }
                    else
                    {
                        vy = -1;
                    }

                    vx = 0;
                }

                if (wait_time > 0)
                {
                    start_time = GeneralLib.timeGetTime();
                }

                // 移動の描画
                var loopTo = (division * GeneralLib.MaxLng(Math.Abs((x2 - x1)), Math.Abs((y2 - y1))));
                for (var i = 1; i <= loopTo; i++)
                {
                    using (var g = _picMain_0.CreateGraphics())
                    {
                        // 画像を消去
                        var sourceRect = new Rectangle(xx, yy, MapCellPx, MapCellPx);
                        g.DrawImage(mainBuffer, sourceRect, sourceRect, GraphicsUnit.Pixel);

                        // 座標を移動
                        xx = (xx + MapCellPx * vx / division);
                        yy = (yy + MapCellPx * vy / division);

                        // 画像を描画
                        g.DrawImage(unitImage, xx, yy);
                    }

                    _picMain_0.Update();

                    if (wait_time > 0)
                    {
                        do
                        {
                            Application.DoEvents();
                            cur_time = GeneralLib.timeGetTime();
                        }
                        while (start_time + wait_time > cur_time);
                        start_time = cur_time;
                    }
                }

                // ２回目の移動方向を設定
                if (Math.Abs((x2 - x1)) > Math.Abs((y2 - y1)))
                {
                    if (y2 > y1)
                    {
                        vy = 1;
                    }
                    else
                    {
                        vy = -1;
                    }

                    vx = 0;
                }
                else
                {
                    if (x2 > x1)
                    {
                        vx = 1;
                    }
                    else
                    {
                        vx = -1;
                    }

                    vy = 0;
                }

                // 移動の描画
                var loopTo1 = (division * GeneralLib.MinLng(Math.Abs((x2 - x1)), Math.Abs((y2 - y1))));
                for (var i = 1; i <= loopTo1; i++)
                {
                    using (var g = _picMain_0.CreateGraphics())
                    {
                        // 画像を消去
                        var sourceRect = new Rectangle(xx, yy, MapCellPx, MapCellPx);
                        g.DrawImage(mainBuffer, sourceRect, sourceRect, GraphicsUnit.Pixel);

                        // 座標を移動
                        xx = (xx + MapCellPx * vx / division);
                        yy = (yy + MapCellPx * vy / division);

                        // 画像を描画
                        g.DrawImage(unitImage, xx, yy);
                    }

                    _picMain_0.Update();

                    if (wait_time > 0)
                    {
                        do
                        {
                            Application.DoEvents();
                            cur_time = GeneralLib.timeGetTime();
                        }
                        while (start_time + wait_time > cur_time);
                        start_time = cur_time;
                    }
                }
            }

            // 画面が書き換えられたことを記録
            GUI.ScreenIsSaved = false;
        }

        public void MoveUnitBitmap2(Unit u, int wait_time0, int division)
        {
            throw new NotImplementedException();
        }

        public void PaintUnitBitmap(Unit u, string smode)
        {
            //// 非表示？
            //if (u.BitmapID == -1)
            //{
            //    return;
            //}

            // 画面外？
            if (!IsInsideWindow(u.x, u.y))
            {
                return;
            }

            // 描き込み先の座標を設定
            var xx = GUI.MapToPixelX(u.x);
            var yy = GUI.MapToPixelY(u.y);
            Image buffer;
            if (smode == "リフレッシュ無し" && GUI.ScreenIsSaved)
            {
                buffer = mainBufferBack;
                //// 表示画像を消去する際に使う描画領域を設定
                //PaintedAreaX1 = GeneralLib.MinLng(PaintedAreaX1, GeneralLib.MaxLng(xx, 0));
                //PaintedAreaY1 = GeneralLib.MinLng(PaintedAreaY1, GeneralLib.MaxLng(yy, 0));
                //PaintedAreaX2 = GeneralLib.MaxLng(PaintedAreaX2, GeneralLib.MinLng(xx + 32, MainPWidth - 1));
                //PaintedAreaY2 = GeneralLib.MaxLng(PaintedAreaY2, GeneralLib.MinLng(yy + 32, MainPHeight - 1));
            }
            else
            {
                buffer = mainBuffer;
            }

            // ユニット画像の書き込み
            using (var g = Graphics.FromImage(buffer))
            {
                var cell = Map.MapData[u.x, u.y];
                var destRect = new Rectangle(xx, yy, MapCellPx, MapCellPx);
                DrawUnit(g, cell, u, destRect);
            }

            if (smode != "リフレッシュ無し")
            {
                UpdateScreen();
            }
        }
    }
}
