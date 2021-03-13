using SRCCore;
using SRCCore.Maps;
using SRCCore.Units;
using SRCTestForm.Resoruces;
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
