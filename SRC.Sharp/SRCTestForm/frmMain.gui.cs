using SRC.Core;
using SRC.Core.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SRCTestForm
{
    public partial class frmMain : IGUI
    {
        bool IGUI.IsGUILocked { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        short IGUI.TopItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool IGUI.MessageWindowIsOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool IGUI.IsFormClicked { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool IGUI.IsMordal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        int IGUI.MessageWait { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool IGUI.AutoMessageMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool IGUI.HCentering { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool IGUI.VCentering { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool IGUI.PermanentStringMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        bool IGUI.KeepStringMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private frmNowLoading frmNowLoading;
        private frmTitle frmTitle;

        void IGUI.LoadMainFormAndRegisterFlash()
        {
            Console.WriteLine("LoadMainFormAndRegisterFlash");
        }

        void IGUI.LoadForms()
        {
            Console.WriteLine("LoadForms");

            //short X, Y;

            //// UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
            //Load(My.MyProject.Forms.frmToolTip);
            //// UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
            //Load(My.MyProject.Forms.frmMessage);
            //// UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
            //Load(My.MyProject.Forms.frmListBox);
            //LockGUI();
            //Commands.CommandState = "ユニット選択";

            //// マップ画面に表示できるマップのサイズ
            //string argini_section1 = "Option";
            //string argini_entry1 = "NewGUI";
            //switch (Strings.LCase(GeneralLib.ReadIni(ref argini_section1, ref argini_entry1)) ?? "")
            //{
            //    case "on":
            //        {
            //            // MOD START MARGE
            //            NewGUIMode = true;
            //            // MOD END MARGE
            //            MainWidth = 20;
            //            break;
            //        }

            //    case "off":
            //        {
            //            MainWidth = 15;
            //            break;
            //        }

            //    default:
            //        {
            //            MainWidth = 15;
            //            string argini_section = "Option";
            //            string argini_entry = "NewGUI";
            //            string argini_data = "Off";
            //            GeneralLib.WriteIni(ref argini_section, ref argini_entry, ref argini_data);
            //            break;
            //        }
            //}
            //// ADD START MARGE
            //// Optionで定義されていればそちらを優先する
            //string argoname = "新ＧＵＩ";
            //if (Expression.IsOptionDefined(ref argoname))
            //{
            //    NewGUIMode = true;
            //    MainWidth = 20;
            //}
            //// ADD END MARGE
            //MainHeight = 15;

            //// マップ画面のサイズ（ピクセル）
            //MainPWidth = (short)(MainWidth * 32);
            //MainPHeight = (short)(MainHeight * 32);
            //{
            //    var withBlock = MainForm;
            //    // メインウィンドウの位置＆サイズを設定
            //    X = (short)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelX();
            //    Y = (short)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelY();
            //    // MOD START MARGE
            //    // If MainWidth = 15 Then
            //    if (!NewGUIMode)
            //    {
            //        // MOD END MARGE
            //        withBlock.Width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock.Width) - Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock.ClientRectangle.Width) * X + (MainPWidth + 24 + 225 + 4) * X);
            //        withBlock.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock.Height) - Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock.ClientRectangle.Height) * Y + (MainPHeight + 24) * Y);
            //    }
            //    else
            //    {
            //        withBlock.Width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock.Width) - Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock.ClientRectangle.Width) * X + MainPWidth * X);
            //        withBlock.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock.Height) - Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock.ClientRectangle.Height) * Y + MainPHeight * Y);
            //    }

            //    withBlock.Left = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX((Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(Screen.PrimaryScreen.Bounds.Width) - Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock.Width)) / 2d);
            //    withBlock.Top = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY((Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(Screen.PrimaryScreen.Bounds.Height) - Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock.Height)) / 2d);

            //    // スクロールバーの位置を設定
            //    // MOD START MARGE
            //    // If MainWidth = 15 Then
            //    if (!NewGUIMode)
            //    {
            //        // MOD END MARGE
            //        // UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //        withBlock.VScroll.Move(MainPWidth + 4, 4, 16, MainPWidth);
            //        // UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //        withBlock.HScroll.Move(4, MainPHeight + 4, MainPWidth, 16);
            //    }
            //    else
            //    {
            //        // UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //        withBlock.VScroll.Visible = false;
            //        // UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            //        withBlock.HScroll.Visible = false;
            //    }

            //    // ステータスウィンドウを設置
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

            //    // マップウィンドウのサイズを設定
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

        void IGUI.SetNewGUIMode()
        {
            throw new NotImplementedException();
        }

        void IGUI.OpenMessageForm(Unit u1, Unit u2)
        {
            throw new NotImplementedException();
        }

        void IGUI.CloseMessageForm()
        {
            throw new NotImplementedException();
        }

        void IGUI.ClearMessageForm()
        {
            throw new NotImplementedException();
        }

        void IGUI.UpdateMessageForm(Unit u1, Unit u2)
        {
            throw new NotImplementedException();
        }

        void IGUI.SaveMessageFormStatus()
        {
            throw new NotImplementedException();
        }

        void IGUI.KeepMessageFormStatus()
        {
            throw new NotImplementedException();
        }

        void IGUI.DisplayMessage(string pname, string msg, string msg_mode)
        {
            throw new NotImplementedException();
        }

        void IGUI.PrintMessage(string msg, bool is_sys_msg)
        {
            throw new NotImplementedException();
        }

        int IGUI.MessageLen(string msg)
        {
            throw new NotImplementedException();
        }

        void IGUI.DisplayBattleMessage(string pname, string msg, string msg_mode)
        {
            throw new NotImplementedException();
        }

        void IGUI.DisplaySysMessage(string msg, bool int_wait)
        {
            throw new NotImplementedException();
        }

        void IGUI.SetupBackground(string draw_mode, string draw_option, int filter_color, double filter_trans_par)
        {
            throw new NotImplementedException();
        }

        void IGUI.RedrawScreen(bool late_refresh)
        {
            throw new NotImplementedException();
        }

        void IGUI.MaskScreen()
        {
            throw new NotImplementedException();
        }

        void IGUI.RefreshScreen(bool without_refresh, bool delay_refresh)
        {
            throw new NotImplementedException();
        }

        void IGUI.Center(int new_x, int new_y)
        {
            throw new NotImplementedException();
        }

        int IGUI.MapToPixelX(int X)
        {
            throw new NotImplementedException();
        }

        int IGUI.MapToPixelY(int Y)
        {
            throw new NotImplementedException();
        }

        int IGUI.PixelToMapX(int X)
        {
            throw new NotImplementedException();
        }

        int IGUI.PixelToMapY(int Y)
        {
            throw new NotImplementedException();
        }

        int IGUI.MakeUnitBitmap(Unit u)
        {
            throw new NotImplementedException();
        }

        void IGUI.PaintUnitBitmap(Unit u, string smode)
        {
            throw new NotImplementedException();
        }

        void IGUI.EraseUnitBitmap(int X, int Y, bool do_refresh)
        {
            throw new NotImplementedException();
        }

        void IGUI.MoveUnitBitmap(Unit u, int x1, int y1, int x2, int y2, int wait_time0, int division)
        {
            throw new NotImplementedException();
        }

        void IGUI.MoveUnitBitmap2(Unit u, int wait_time0, int division)
        {
            throw new NotImplementedException();
        }

        int IGUI.ListBox(string lb_caption, string[] list, string lb_info, string lb_mode)
        {
            throw new NotImplementedException();
        }

        void IGUI.EnlargeListBoxHeight()
        {
            throw new NotImplementedException();
        }

        void IGUI.ReduceListBoxHeight()
        {
            throw new NotImplementedException();
        }

        void IGUI.EnlargeListBoxWidth()
        {
            throw new NotImplementedException();
        }

        void IGUI.ReduceListBoxWidth()
        {
            throw new NotImplementedException();
        }

        void IGUI.AddPartsToListBox()
        {
            throw new NotImplementedException();
        }

        void IGUI.RemovePartsOnListBox()
        {
            throw new NotImplementedException();
        }

        int IGUI.WeaponListBox(Unit u, string caption_msg, string lb_mode, string BGM)
        {
            throw new NotImplementedException();
        }

        int IGUI.AbilityListBox(Unit u, string caption_msg, string lb_mode, bool is_item)
        {
            throw new NotImplementedException();
        }

        int IGUI.LIPS(string lb_caption, string[] list, string lb_info, int time_limit)
        {
            throw new NotImplementedException();
        }

        int IGUI.MultiColumnListBox(string lb_caption, string[] list, bool is_center)
        {
            throw new NotImplementedException();
        }

        int IGUI.MultiSelectListBox(string lb_caption, string[] list, string lb_info, int max_num)
        {
            throw new NotImplementedException();
        }

        bool IGUI.DrawPicture(string fname, int dx, int dy, int dw, int dh, int sx, int sy, int sw, int sh, string draw_option)
        {
            throw new NotImplementedException();
        }

        void IGUI.MakePicBuf()
        {
            throw new NotImplementedException();
        }

        void IGUI.DrawString(string msg, int X, int Y, bool without_cr)
        {
            throw new NotImplementedException();
        }

        void IGUI.DrawSysString(int X, int Y, string msg, bool without_refresh)
        {
            throw new NotImplementedException();
        }

        void IGUI.SaveScreen()
        {
            throw new NotImplementedException();
        }

        void IGUI.ClearPicture()
        {
            throw new NotImplementedException();
        }

        void IGUI.ClearPicture2(int x1, int y1, int x2, int y2)
        {
            throw new NotImplementedException();
        }

        void IGUI.LockGUI()
        {
            throw new NotImplementedException();
        }

        void IGUI.UnlockGUI()
        {
            throw new NotImplementedException();
        }

        void IGUI.SaveCursorPos()
        {
            throw new NotImplementedException();
        }

        void IGUI.MoveCursorPos(string cursor_mode, Unit t)
        {
            throw new NotImplementedException();
        }

        void IGUI.RestoreCursorPos()
        {
            throw new NotImplementedException();
        }

        void IGUI.OpenTitleForm()
        {
            frmTitle = new frmTitle();
            frmTitle.Show(this);
        }

        void IGUI.CloseTitleForm()
        {
            frmTitle.Close();
            frmTitle.Dispose();
            frmTitle = null;
        }

        void IGUI.OpenNowLoadingForm()
        {
            frmNowLoading = new frmNowLoading();
            frmNowLoading.Show(this);
        }

        void IGUI.CloseNowLoadingForm()
        {
            frmNowLoading.Close();
            frmNowLoading.Dispose();
            frmNowLoading = null;
        }

        void IGUI.DisplayLoadingProgress()
        {
            frmNowLoading.Progress();
            Application.DoEvents();
        }

        void IGUI.SetLoadImageSize(int new_size)
        {
            frmNowLoading.Value = 0;
            frmNowLoading.Max = new_size;
        }

        void IGUI.ChangeDisplaySize(int w, int h)
        {
            throw new NotImplementedException();
        }

        void IGUI.ErrorMessage(string msg)
        {
            SetStatusText(msg);
        }

        void IGUI.DataErrorMessage(string msg, string fname, int line_num, string line_buf, string dname)
        {
            throw new NotImplementedException();
        }

        bool IGUI.IsRButtonPressed(bool ignore_message_wait)
        {
            throw new NotImplementedException();
        }

        void IGUI.DisplayTelop(string msg)
        {
            Console.WriteLine("DisplayTelop: " + msg);
        }

        void IGUI.SetTitle(string title)
        {
            // XXX 別のフォームに設定
            Name = title;
        }
    }
}
