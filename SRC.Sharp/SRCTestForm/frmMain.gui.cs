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
    public partial class frmMain
    {
        public bool IsGUILocked { get; set; }
        public short TopItem { get; set; }
        public bool MessageWindowIsOut { get; set; }
        public bool IsFormClicked { get; set; }
        public bool IsMordal { get; set; }
        public int MessageWait { get; set; }
        public bool AutoMessageMode { get; set; }
        public bool HCentering { get; set; }
        public bool VCentering { get; set; }
        public bool PermanentStringMode { get; set; }
        public bool KeepStringMode { get; set; }
        public int MainWidth { get; set; }
        public int MainHeight { get; set; }
        public int MainPWidth { get; set; }
        public int MainPHeight { get; set; }
        public int MapPWidth { get; set; }
        public int MapPHeight { get; set; }
        public bool ScreenIsMasked { get; set; }
        public bool ScreenIsSaved { get; set; }
        public int MapX { get; set; }
        public int MapY { get; set; }
        public int PrevMapX { get; set; }
        public int PrevMapY { get; set; }
        public int MouseButton { get; set; }
        public float MouseX { get; set; }
        public float MouseY { get; set; }
        public float PrevMouseX { get; set; }
        public float PrevMouseY { get; set; }
        public int PrevUnitX { get; set; }
        public int PrevUnitY { get; set; }
        public string PrevUnitArea { get; set; }
        public string PrevCommand { get; set; }
        public bool IsPictureDrawn { get; set; }
        public bool IsPictureVisible { get; set; }
        public int PaintedAreaX1 { get; set; }
        public int PaintedAreaY1 { get; set; }
        public int PaintedAreaX2 { get; set; }
        public int PaintedAreaY2 { get; set; }
        public bool IsCursorVisible { get; set; }
        public int BGColor { get; set; }
        int IGUI.TopItem { get; set; }

        private frmNowLoading frmNowLoading;
        private frmTitle frmTitle;

        private frmMessage frmMessage;

        public void LoadMainFormAndRegisterFlash()
        {
            Console.WriteLine("LoadMainFormAndRegisterFlash");
        }

        public void LoadForms()
        {
            Console.WriteLine("LoadForms");

            //short X, Y;

            //// UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
            //Load(frmToolTip);
            frmMessage = new frmMessage()
            {
                SRC = SRC
            };
            //Load(frmListBox);
            LockGUI();
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

        public void SetNewGUIMode()
        {
            throw new NotImplementedException();
        }

        public bool MessageFormVisible => frmMessage.Visible;
        private string DisplayedPilot;
        private string DisplayMode;
        private Unit RightUnit;
        private Unit LeftUnit;
        private double RightUnitHPRatio;
        private double LeftUnitHPRatio;
        private double RightUnitENRatio;
        private double LeftUnitENRatio;

        public void OpenMessageForm(Unit u1, Unit u2)
        {
            if (!frmMessage.Visible)
            {
                frmMessage.ClearForm();
                frmMessage.Show(this);
            }
            Application.DoEvents();
        }

        public void CloseMessageForm()
        {
            frmMessage.Hide();
            Application.DoEvents();
        }

        public void ClearMessageForm()
        {
            DisplayedPilot = "";
            RightUnit = null;
            LeftUnit = null;

            frmMessage.ClearForm();
            Application.DoEvents();
        }

        public void UpdateMessageForm(Unit u1, Unit u2)
        {
            throw new NotImplementedException();
        }

        public void SaveMessageFormStatus()
        {
            throw new NotImplementedException();
        }

        public void KeepMessageFormStatus()
        {
            throw new NotImplementedException();
        }

        public void DisplayMessage(string pname, string msg, string msg_mode)
        {
            string pnickname;
            string left_margin;
            DisplayMessagePilot(pname, msg_mode, out pnickname, out left_margin);

            frmMessage.SetMessage(msg);
            Application.DoEvents();

            // 次のメッセージ待ち
            IsFormClicked = false;
            while (!IsFormClicked)
            {
                Thread.Sleep(100);
                Application.DoEvents();
            }
        }

        private void DisplayMessagePilot(string pname, string msg_mode, out string pnickname, out string left_margin)
        {
            pnickname = "";
            left_margin = "";
            // キャラ表示の描き換え
            if (pname == "システム")
            {
                // 「システム」
                frmMessage.picFace.Image = Image.FromFile("");
                frmMessage.picFace.Refresh();
                DisplayedPilot = "";
                left_margin = "";
            }
            else if (!string.IsNullOrEmpty(pname))
            {
                // どのキャラ画像を使うか？
                var fname = "-.bmp";
                // TODO
                //if (SRC.PList.IsDefined(pname))
                //{
                //    Pilot localItem() { object argIndex1 = pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                //    pnickname = localItem().get_Nickname(false);
                //    Pilot localItem1() { object argIndex1 = pname; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                //    fname = localItem1().get_Bitmap(false);
                //}
                //else
                if (SRC.PDList.IsDefined(pname))
                {
                    var pd = SRC.PDList.Item(pname);
                    pnickname = pd.Nickname;
                    fname = pd.Bitmap;
                }
                else if (SRC.NPDList.IsDefined(pname))
                {
                    var pd = SRC.NPDList.Item(pname);
                    pnickname = pd.Nickname;
                    fname = pd.Bitmap;
                }

                // キャラ画像の表示
                if (fname != "-.bmp")
                {
                    fname = Path.Combine("Pilot", fname);
                    if ((DisplayedPilot ?? "") != (fname ?? "") || (DisplayMode ?? "") != (msg_mode ?? ""))
                    {
                        string argdraw_option = "メッセージ " + msg_mode;
                        if (DrawPicture(fname, 0, 0, 64, 64, 0, 0, 0, 0, argdraw_option))
                        {
                            frmMessage.picFace.Refresh();
                            DisplayedPilot = fname;
                            DisplayMode = msg_mode;
                        }
                        else
                        {
                            frmMessage.picFace.Image = Image.FromFile("");
                            frmMessage.picFace.Refresh();
                            DisplayedPilot = "";
                            DisplayMode = "";

                            // TODO
                            //// パイロット画像が存在しないことを記録しておく
                            //object argIndex3 = pname;
                            //if (SRC.PList.IsDefined(pname))
                            //{
                            //    object argIndex2 = pname;
                            //    {
                            //        var withBlock = SRC.PList.Item(ref argIndex2);
                            //        if ((withBlock.get_Bitmap(false) ?? "") == (withBlock.Data.Bitmap ?? ""))
                            //        {
                            //            withBlock.Data.IsBitmapMissing = true;
                            //        }
                            //    }
                            //}
                            //else if (SRC.PDList.IsDefined(pname))
                            //{
                            //    PilotData localItem6() { object argIndex1 = pname; var ret = SRC.PDList.Item(ref argIndex1); return ret; }

                            //    localItem6().IsBitmapMissing = true;
                            //}
                            //else if (SRC.NPDList.IsDefined(pname))
                            //{
                            //    NonPilotData localItem7() { object argIndex1 = pname; var ret = SRC.NPDList.Item(ref argIndex1); return ret; }

                            //    localItem7().IsBitmapMissing = true;
                            //}
                        }
                    }
                }
                else
                {
                    frmMessage.picFace.Image = Image.FromFile("");
                    frmMessage.picFace.Refresh();
                    DisplayedPilot = "";
                    DisplayMode = "";
                }

                // TODO
                left_margin = "  ";
                //if (Expression.IsOptionDefined("会話パイロット名改行"))
                //{
                //    left_margin = " ";
                //}
                //else
                //{
                //    left_margin = "  ";
                //}
            }
        }

        public void PrintMessage(string msg, bool is_sys_msg)
        {
            throw new NotImplementedException();
        }

        public int MessageLen(string msg)
        {
            throw new NotImplementedException();
        }

        public void DisplayBattleMessage(string pname, string msg, string msg_mode)
        {
            throw new NotImplementedException();
        }

        public void DisplaySysMessage(string msg, bool int_wait)
        {
            throw new NotImplementedException();
        }

        public void SetupBackground(string draw_mode, string draw_option, int filter_color, double filter_trans_par)
        {
            throw new NotImplementedException();
        }

        public void RedrawScreen(bool late_refresh)
        {
            throw new NotImplementedException();
        }

        public void MaskScreen()
        {
            throw new NotImplementedException();
        }

        public void RefreshScreen(bool without_refresh, bool delay_refresh)
        {
            throw new NotImplementedException();
        }

        public void Center(int new_x, int new_y)
        {
            throw new NotImplementedException();
        }

        public int MapToPixelX(int X)
        {
            throw new NotImplementedException();
        }

        public int MapToPixelY(int Y)
        {
            throw new NotImplementedException();
        }

        public int PixelToMapX(int X)
        {
            throw new NotImplementedException();
        }

        public int PixelToMapY(int Y)
        {
            throw new NotImplementedException();
        }

        public int MakeUnitBitmap(Unit u)
        {
            throw new NotImplementedException();
        }

        public void PaintUnitBitmap(Unit u, string smode)
        {
            throw new NotImplementedException();
        }

        public void EraseUnitBitmap(int X, int Y, bool do_refresh)
        {
            throw new NotImplementedException();
        }

        public void MoveUnitBitmap(Unit u, int x1, int y1, int x2, int y2, int wait_time0, int division)
        {
            throw new NotImplementedException();
        }

        public void MoveUnitBitmap2(Unit u, int wait_time0, int division)
        {
            throw new NotImplementedException();
        }

        public int ListBox(string lb_caption, string[] list, string lb_info, string lb_mode)
        {
            throw new NotImplementedException();
        }

        public void EnlargeListBoxHeight()
        {
            throw new NotImplementedException();
        }

        public void ReduceListBoxHeight()
        {
            throw new NotImplementedException();
        }

        public void EnlargeListBoxWidth()
        {
            throw new NotImplementedException();
        }

        public void ReduceListBoxWidth()
        {
            throw new NotImplementedException();
        }

        public void AddPartsToListBox()
        {
            throw new NotImplementedException();
        }

        public void RemovePartsOnListBox()
        {
            throw new NotImplementedException();
        }

        public int WeaponListBox(Unit u, string caption_msg, string lb_mode, string BGM)
        {
            throw new NotImplementedException();
        }

        public int AbilityListBox(Unit u, string caption_msg, string lb_mode, bool is_item)
        {
            throw new NotImplementedException();
        }

        public int LIPS(string lb_caption, string[] list, string lb_info, int time_limit)
        {
            throw new NotImplementedException();
        }

        public int MultiColumnListBox(string lb_caption, string[] list, bool is_center)
        {
            throw new NotImplementedException();
        }

        public int MultiSelectListBox(string lb_caption, string[] list, string lb_info, int max_num)
        {
            throw new NotImplementedException();
        }

        public void MakePicBuf()
        {
            throw new NotImplementedException();
        }

        public void DrawString(string msg, int X, int Y, bool without_cr)
        {
            throw new NotImplementedException();
        }

        public void DrawSysString(int X, int Y, string msg, bool without_refresh)
        {
            throw new NotImplementedException();
        }

        public void SaveScreen()
        {
            throw new NotImplementedException();
        }

        public void ClearPicture()
        {
            throw new NotImplementedException();
        }

        public void ClearPicture2(int x1, int y1, int x2, int y2)
        {
            throw new NotImplementedException();
        }

        public void LockGUI()
        {
            IsGUILocked = true;
        }

        public void UnlockGUI()
        {
            IsGUILocked = false;
        }

        public void SaveCursorPos()
        {
            throw new NotImplementedException();
        }

        public void MoveCursorPos(string cursor_mode, Unit t)
        {
            throw new NotImplementedException();
        }

        public void RestoreCursorPos()
        {
            throw new NotImplementedException();
        }

        public void OpenTitleForm()
        {
            frmTitle = new frmTitle();
            frmTitle.Show(this);
        }

        public void CloseTitleForm()
        {
            if (frmTitle != null)
            {
                frmTitle.Close();
                frmTitle.Dispose();
                frmTitle = null;
            }
        }

        public void OpenNowLoadingForm()
        {
            frmNowLoading = new frmNowLoading();
            frmNowLoading.Show(this);
        }

        public void CloseNowLoadingForm()
        {
            frmNowLoading.Close();
            frmNowLoading.Dispose();
            frmNowLoading = null;
        }

        public void DisplayLoadingProgress()
        {
            frmNowLoading.Progress();
            Application.DoEvents();
        }

        public void SetLoadImageSize(int new_size)
        {
            frmNowLoading.Value = 0;
            frmNowLoading.Max = new_size;
        }

        public void ChangeDisplaySize(int w, int h)
        {
            throw new NotImplementedException();
        }

        public void ErrorMessage(string msg)
        {
            LogInfo(msg);
        }

        public void DataErrorMessage(string msg, string fname, int line_num, string line_buf, string dname)
        {
            throw new NotImplementedException();
        }

        public bool IsRButtonPressed(bool ignore_message_wait)
        {
            throw new NotImplementedException();
        }

        public void DisplayTelop(string msg)
        {
            Console.WriteLine("DisplayTelop: " + msg);
        }

        public void SetTitle(string title)
        {
            // XXX 別のフォームに設定
            Name = title;
        }

        public void LogDebug(string message)
        {
            Program.Log.Log(LogLevel.Debug, message);
        }

        public void LogInfo(string message)
        {
            Program.Log.Log(LogLevel.Information, message);
        }
    }
}
