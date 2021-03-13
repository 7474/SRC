using SRCCore;
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
            int xx, yy;
            int ret;

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

            xx = GUI.MapToPixelX(X);
            yy = GUI.MapToPixelY(Y);

            SaveScreen();

            // 画面表示変更
            using (var g = Graphics.FromImage(mainBuffer))
            {
                var sourceRect = new Rectangle(xx, yy, MapCellPx, MapCellPx);
                var destRect = new Rectangle((X - 1) * MapCellPx, (Y - 1) * MapCellPx, MapCellPx, MapCellPx);
                g.DrawImage(picBack.Image, sourceRect, destRect, GraphicsUnit.Pixel);

            }
            if (do_refresh)
            {
                UpdateScreen();
            }
        }

        public void MoveUnitBitmap(Unit u, int x1, int y1, int x2, int y2, int wait_time0, int division)
        {
            short xx, yy;
            short vx, vy;
            int ret;
            short i;
            PictureBox pic;
            int cur_time, start_time = default, wait_time;
            var PT = default(POINTAPI);
            wait_time = wait_time0 / division;
            SaveScreen();
            {
                var withBlock = MainForm;
                // UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                pic = withBlock.picTmp32(0);

                // ユニット画像を作成
                // UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                ret = BitBlt(pic.hDC, 0, 0, 32, 32, withBlock.picUnitBitmap.hDC, 32 * ((int)u.BitmapID % 15), 96 * ((int)u.BitmapID / 15), SRCCOPY);

                // ユニットのいる場所に合わせて表示を変更
                switch (u.Area ?? "")
                {
                    case "空中":
                        {
                            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                            ret = MoveToEx(pic.hDC, 0, 28, ref PT);
                            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                            ret = LineTo(pic.hDC, 31, 28);
                            break;
                        }

                    case "水中":
                        {
                            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                            ret = MoveToEx(pic.hDC, 0, 3, ref PT);
                            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                            ret = LineTo(pic.hDC, 31, 3);
                            break;
                        }

                    case "地中":
                        {
                            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                            ret = MoveToEx(pic.hDC, 0, 28, ref PT);
                            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                            ret = LineTo(pic.hDC, 31, 28);
                            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                            ret = MoveToEx(pic.hDC, 0, 3, ref PT);
                            // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                            ret = LineTo(pic.hDC, 31, 3);
                            break;
                        }

                    case "宇宙":
                        {
                            if (Map.TerrainClass(u.x, u.y) == "月面")
                            {
                                // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                                ret = MoveToEx(pic.hDC, 0, 28, ref PT);
                                // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                                ret = LineTo(pic.hDC, 31, 28);
                            }

                            break;
                        }
                }

                // 移動の始点を設定
                xx = MapToPixelX(x1);
                yy = MapToPixelY(y1);

                // 背景上の画像をまず消去
                // (既に移動している場合を除く)
                if (ReferenceEquals(u, Map.MapDataForUnit[x1, y1]))
                {
                    // UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    ret = BitBlt(withBlock.picMain(0).hDC, xx, yy, 32, 32, withBlock.picBack.hDC, 32 * ((int)x1 - 1), 32 * ((int)y1 - 1), SRCCOPY);
                    // UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    ret = BitBlt(withBlock.picMain(1).hDC, xx, yy, 32, 32, withBlock.picBack.hDC, 32 * ((int)x1 - 1), 32 * ((int)y1 - 1), SRCCOPY);
                }

                // 最初の移動方向を設定
                if (Math.Abs((short)(x2 - x1)) > Math.Abs((short)(y2 - y1)))
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
                var loopTo = (short)(division * GeneralLib.MaxLng(Math.Abs((short)(x2 - x1)), Math.Abs((short)(y2 - y1))));
                for (i = 1; i <= loopTo; i++)
                {
                    // 画像を消去
                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    ret = BitBlt(withBlock.picMain(0).hDC, xx, yy, 32, 32, withBlock.picMain(1).hDC, xx, yy, SRCCOPY);

                    // 座標を移動
                    xx = (short)(xx + 32 * vx / division);
                    yy = (short)(yy + 32 * vy / division);

                    // 画像を描画
                    // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    ret = BitBlt(withBlock.picMain(0).hDC, xx, yy, 32, 32, pic.hDC, 0, 0, SRCCOPY);

                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    withBlock.picMain(0).Refresh();
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
                if (Math.Abs((short)(x2 - x1)) > Math.Abs((short)(y2 - y1)))
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
                var loopTo1 = (short)(division * GeneralLib.MinLng(Math.Abs((short)(x2 - x1)), Math.Abs((short)(y2 - y1))));
                for (i = 1; i <= loopTo1; i++)
                {
                    // 画像を消去
                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    ret = BitBlt(withBlock.picMain(0).hDC, xx, yy, 32, 32, withBlock.picMain(1).hDC, xx, yy, SRCCOPY);

                    // 座標を移動
                    xx = (short)(xx + 32 * vx / division);
                    yy = (short)(yy + 32 * vy / division);

                    // 画像を描画
                    // UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    ret = BitBlt(withBlock.picMain(0).hDC, xx, yy, 32, 32, pic.hDC, 0, 0, SRCCOPY);

                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    withBlock.picMain(0).Refresh();
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
            ScreenIsSaved = false;
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
                //PaintedAreaX1 = (short)GeneralLib.MinLng(PaintedAreaX1, GeneralLib.MaxLng(xx, 0));
                //PaintedAreaY1 = (short)GeneralLib.MinLng(PaintedAreaY1, GeneralLib.MaxLng(yy, 0));
                //PaintedAreaX2 = (short)GeneralLib.MaxLng(PaintedAreaX2, GeneralLib.MinLng(xx + 32, MainPWidth - 1));
                //PaintedAreaY2 = (short)GeneralLib.MaxLng(PaintedAreaY2, GeneralLib.MinLng(yy + 32, MainPHeight - 1));
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
