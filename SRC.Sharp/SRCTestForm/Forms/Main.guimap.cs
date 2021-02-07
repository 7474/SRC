using Microsoft.Extensions.Logging;
using SRC.Core;
using SRC.Core.Lib;
using SRC.Core.Units;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SRCTestForm
{
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
    }
}
