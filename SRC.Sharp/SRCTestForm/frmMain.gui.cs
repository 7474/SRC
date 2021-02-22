using Microsoft.Extensions.Logging;
using SRCCore;
using SRCCore.Commands;
using SRCCore.Lib;
using SRCCore.Units;
using SRCTestForm.Resoruces;
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
    public partial class frmTeatMain : IGUI
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
        public int MainPWidth
        {
            get => MainForm.MainPWidth;
            set => throw new NotSupportedException();
        }
        public int MainPHeight
        {
            get => MainForm.MainPHeight;
            set => throw new NotSupportedException();
        }
        public int MapPWidth
        {
            get => MainForm.MapPWidth;
            set => throw new NotSupportedException();
        }
        public int MapPHeight
        {
            get => MainForm.MapPHeight;
            set => throw new NotSupportedException();
        }
        public bool ScreenIsMasked { get; set; }
        public bool ScreenIsSaved { get; set; }
        public int MapX { get; set; }
        public int MapY { get; set; }
        public int PrevMapX { get; set; }
        public int PrevMapY { get; set; }
        public GuiButton MouseButton { get; set; }
        public double MouseX { get; set; }
        public double MouseY { get; set; }
        public double PrevMouseX { get; set; }
        public double PrevMouseY { get; set; }
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
        private frmMain MainForm;

        private ImageBuffer imageBuffer;

        public void LoadMainFormAndRegisterFlash()
        {
            imageBuffer = new ImageBuffer(SRC);

            MainForm = new frmMain()
            {
                SRC = SRC,
            };
            SRC.GUIMap = MainForm;
            SRC.GUIStatus = MainForm;
            MainForm.Init(imageBuffer);

            Program.Log.LogDebug("LoadMainFormAndRegisterFlash");
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
            MainWidth = 15;
            MainHeight = 15;

            MainForm.InitMapSize(MainWidth, MainHeight);
            MainForm.InitStatus();
        }

        public void SetNewGUIMode()
        {
            throw new NotImplementedException();
        }
        public void MainFormShow()
        {
            if (!MainFormVisible)
            {
                MainForm.Show();
            }
        }

        public bool MessageFormVisible => frmMessage.Visible;

        public bool MainFormVisible => MainForm.Visible;

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
            // ユニット表示を伴う場合はキャプションから「(自動送り)」を削除
            if (u1 is object)
            {
                if (frmMessage.Text == "メッセージ (自動送り)")
                {
                    frmMessage.Text = "メッセージ";
                }
            }

            // メッセージウィンドウを強制的に最小化解除
            if (frmMessage.WindowState != FormWindowState.Normal)
            {
                frmMessage.WindowState = FormWindowState.Normal;
                frmMessage.Show(MainForm);
                frmMessage.Activate();
            }

            //if (u1 is null)
            //{
            //    // ユニット表示なし
            //    frmMessage.labHP1.Visible = false;
            //    frmMessage.labHP2.Visible = false;
            //    frmMessage.labEN1.Visible = false;
            //    frmMessage.labEN2.Visible = false;
            //    frmMessage.picHP1.Visible = false;
            //    frmMessage.picHP2.Visible = false;
            //    frmMessage.picEN1.Visible = false;
            //    frmMessage.picEN2.Visible = false;
            //    frmMessage.txtHP1.Visible = false;
            //    frmMessage.txtHP2.Visible = false;
            //    frmMessage.txtEN1.Visible = false;
            //    frmMessage.txtEN2.Visible = false;
            //    frmMessage.picUnit1.Visible = false;
            //    frmMessage.picUnit2.Visible = false;
            //    frmMessage.Width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(frmMessage.Width) - frmMessage.ClientRectangle.Width * tppx + 508 * tppx);
            //    frmMessage.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(frmMessage.Height) - frmMessage.ClientRectangle.Height * tppy + 84 * tppy);
            //    frmMessage.picFace.Top = 8;
            //    frmMessage.picFace.Left = 8;
            //    frmMessage.picMessage.Top = 7;
            //    frmMessage.picMessage.Left = 84;
            //}
            //else if (u2 is null)
            //{
            //    // ユニット表示１体のみ
            //    if (u1.Party == "味方" | u1.Party == "ＮＰＣ")
            //    {
            //        frmMessage.labHP1.Visible = false;
            //        frmMessage.labEN1.Visible = false;
            //        frmMessage.picHP1.Visible = false;
            //        frmMessage.picEN1.Visible = false;
            //        frmMessage.txtHP1.Visible = false;
            //        frmMessage.txtEN1.Visible = false;
            //        frmMessage.picUnit1.Visible = false;
            //        frmMessage.labHP2.Visible = true;
            //        frmMessage.labEN2.Visible = true;
            //        frmMessage.picHP2.Visible = true;
            //        frmMessage.picEN2.Visible = true;
            //        frmMessage.txtHP2.Visible = true;
            //        frmMessage.txtEN2.Visible = true;
            //        frmMessage.picUnit2.Visible = true;
            //    }
            //    else
            //    {
            //        frmMessage.labHP1.Visible = true;
            //        frmMessage.labEN1.Visible = true;
            //        frmMessage.picHP1.Visible = true;
            //        frmMessage.picEN1.Visible = true;
            //        frmMessage.txtHP1.Visible = true;
            //        frmMessage.txtEN1.Visible = true;
            //        frmMessage.picUnit1.Visible = true;
            //        frmMessage.labHP2.Visible = false;
            //        frmMessage.labEN2.Visible = false;
            //        frmMessage.picHP2.Visible = false;
            //        frmMessage.picEN2.Visible = false;
            //        frmMessage.txtHP2.Visible = false;
            //        frmMessage.txtEN2.Visible = false;
            //        frmMessage.picUnit2.Visible = false;
            //    }

            //    object argu21 = null;
            //    UpdateMessageForm(ref u1, u2: ref argu21);
            //    frmMessage.Width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(frmMessage.Width) - frmMessage.ClientRectangle.Width * tppx + 508 * tppx);
            //    frmMessage.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(frmMessage.Height) - frmMessage.ClientRectangle.Height * tppy + 118 * tppy);
            //    frmMessage.picFace.Top = 42;
            //    frmMessage.picFace.Left = 8;
            //    frmMessage.picMessage.Top = 41;
            //    frmMessage.picMessage.Left = 84;
            //}
            //else
            //{
            //    // ユニットを２体表示
            //    frmMessage.labHP1.Visible = true;
            //    frmMessage.labHP2.Visible = true;
            //    frmMessage.labEN1.Visible = true;
            //    frmMessage.labEN2.Visible = true;
            //    frmMessage.picHP1.Visible = true;
            //    frmMessage.picHP2.Visible = true;
            //    frmMessage.picEN1.Visible = true;
            //    frmMessage.picEN2.Visible = true;
            //    frmMessage.txtHP1.Visible = true;
            //    frmMessage.txtHP2.Visible = true;
            //    frmMessage.txtEN1.Visible = true;
            //    frmMessage.txtEN2.Visible = true;
            //    frmMessage.picUnit1.Visible = true;
            //    frmMessage.picUnit2.Visible = true;
            //    object argu2 = u2;
            //    UpdateMessageForm(ref u1, ref argu2);
            //    frmMessage.Width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(frmMessage.Width) - frmMessage.ClientRectangle.Width * tppx + 508 * tppx);
            //    frmMessage.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(frmMessage.Height) - frmMessage.ClientRectangle.Height * tppy + 118 * tppy);
            //    frmMessage.picFace.Top = 42;
            //    frmMessage.picFace.Left = 8;
            //    frmMessage.picMessage.Top = 41;
            //    frmMessage.picMessage.Left = 84;
            //}

            // メッセージウィンドウの位置設定
            if (MainForm.Visible && MainForm.WindowState != FormWindowState.Minimized)
            {
                // メインウィンドウが表示されていればメインウィンドウの下端に合わせて表示
                if (!frmMessage.Visible)
                {
                    if (MainWidth == 15)
                    {
                        frmMessage.Left = MainForm.Left;
                    }
                    else
                    {
                        frmMessage.Left = MainForm.Left - (MainForm.Width - frmMessage.Width) / 2;
                    }

                    if (MessageWindowIsOut)
                    {
                        frmMessage.Top = MainForm.Top + MainForm.Height;// - 350;
                    }
                    else
                    {
                        frmMessage.Top = MainForm.Top + MainForm.Height - frmMessage.Height;
                    }
                }
            }
            else
            {
                //// メインウィンドウが表示されていない場合は画面中央に表示
                //frmMessage.Left = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX((Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(Screen.PrimaryScreen.Bounds.Width) - Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(frmMessage.Width)) / 2d);
                //frmMessage.Top = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY((Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(Screen.PrimaryScreen.Bounds.Height) - Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(frmMessage.Height)) / 2d);
            }

            // ウィンドウをクリアしておく
            ClearMessageForm();

            // ウィンドウを表示
            if (!frmMessage.Visible)
            {
                frmMessage.Show(MainForm);
            }

            // 常に手前に表示する
            frmMessage.TopMost = true;

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
                frmMessage.picFace.Image = null;
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
                            frmMessage.picFace.Image = null;
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
                    frmMessage.picFace.Image = null;
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
            MainForm.SetupBackground(draw_mode, draw_option, filter_color, filter_trans_par);
        }

        public void RedrawScreen(bool late_refresh)
        {
            ScreenIsMasked = false;

            // 画面を更新
            RefreshScreen(false, late_refresh);

            //'カーソルを再描画
            //GetCursorPos PT
            //ret = SetCursorPos(PT.X, PT.Y)
        }

        public void MaskScreen()
        {
            ScreenIsMasked = true;

            // 画面を更新
            RefreshScreen();
        }

        public void RefreshScreen(bool without_refresh = false, bool delay_refresh = false)
        {
            // マップデータが設定されていなければ画面書き換えを行わない
            if (Map.MapWidth == 1)
            {
                return;
            }

            // 表示位置がマップ外にある場合はマップ内に合わせる
            if (MapX < 1)
            {
                MapX = 1;
            }
            else if (MapX > Map.MapWidth)
            {
                MapX = Map.MapWidth;
            }

            if (MapY < 1)
            {
                MapY = 1;
            }
            else if (MapY > Map.MapHeight)
            {
                MapY = Map.MapHeight;
            }

            MainForm.RefreshScreen(MapX, MapY, without_refresh, delay_refresh);
        }

        public void Center(int new_x, int new_y)
        {
            if (string.IsNullOrEmpty(Map.MapFileName))
            {
                return;
            }

            // XXX スクロールバーのMax見るでいいの？
            MapX = new_x;
            if (MapX < 1)
            {
                MapX = 1;
            }
            else if (MapX > MainForm.HScrollBar.Maximum)
            {
                MapX = MainForm.HScrollBar.Maximum;
            }

            MapY = new_y;
            if (MapY < 1)
            {
                MapY = 1;
            }
            else if (MapY > MainForm.VScrollBar.Maximum)
            {
                MapY = MainForm.VScrollBar.Maximum;
            }
        }

        public int MapToPixelX(int X)
        {
            return frmMain.MapCellPx * ((MainWidth + 1) / 2 - 1 - (MapX - X));
        }

        public int MapToPixelY(int Y)
        {
            return frmMain.MapCellPx * ((MainHeight + 1) / 2 - 1 - (MapY - Y));
        }

        public int PixelToMapX(int X)
        {
            if (X < 0)
            {
                X = 0;
            }
            else if (X >= MainPWidth)
            {
                X = MainPWidth - 1;
            }

            return X / frmMain.MapCellPx + 1 + MapX - (MainWidth + 1) / 2;
        }

        public int PixelToMapY(int Y)
        {
            if (Y < 0)
            {
                Y = 0;
            }
            else if (Y >= MainPHeight)
            {
                Y = MainPHeight - 1;
            }

            return Y / frmMain.MapCellPx + 1 + MapY - (MainHeight + 1) / 2;
        }

        public int MakeUnitBitmap(Unit u)
        {
            throw new NotImplementedException();
        }

        public void PaintUnitBitmap(Unit u, string smode)
        {
            MainForm.PaintUnitBitmap(u, smode);
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

        public void DrawSysString(int X, int Y, string msg, bool without_refresh)
        {
            throw new NotImplementedException();
        }

        public void SaveScreen()
        {
            if (!ScreenIsSaved)
            {
                // XXX 何で半端にMainFormに追い出してあるんだ。
                // 画像をpicMain(1)に保存
                MainForm.SaveScreen();

                ScreenIsSaved = true;
            }
        }

        public void ClearPicture()
        {
            if (!ScreenIsSaved)
            {
                return;
            }

            IsPictureVisible = false;
            IsCursorVisible = false;

            // XXX 全体クリアしておく
            using (var g = Graphics.FromImage(MainForm.MainBuffer))
            {
                g.DrawImage(MainForm.MainBufferBack, 0, 0);
            }
        }

        public void ClearPicture2(int x1, int y1, int x2, int y2)
        {
            if (!ScreenIsSaved)
            {
                return;
            }

            using (var g = Graphics.FromImage(MainForm.MainBuffer))
            {
                var rect = new Rectangle(x1, y1, x2 - x1, y2 - y1);
                g.DrawImage(MainForm.MainBufferBack, rect, rect, GraphicsUnit.Pixel);
            }
        }

        public void LockGUI()
        {
            IsGUILocked = true;
        }

        public void UnlockGUI()
        {
            IsGUILocked = false;
            Application.DoEvents();
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
            frmTitle.Show(MainForm);
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
            frmNowLoading.Show(MainForm);
        }

        public void CloseNowLoadingForm()
        {
            frmNowLoading.Close();
            frmNowLoading.Dispose();
            frmNowLoading = null;
            // XXX シナリオのパスが決まってるタイミングでという意味でファイルシステムの状態を再処理してる。カス。
            imageBuffer.InitFileSystemInfo();
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
            // メッセージがウエイト無しならスキップ
            if (!ignore_message_wait & MessageWait == 0)
            {
                return true;
            }

            // TODO Impl ネイティブAPIでシビアに取る？
            if (MouseButtons.HasFlag(MouseButtons.Right))
            {
                return true;
            }
            //// メインウインドウ上でマウスボタンを押した場合
            //if (MainForm.Handle.ToInt32() == GetForegroundWindow())
            //{
            //    GetCursorPos(ref PT);
            //    {
            //        var withBlock = MainForm;
            //        if ((long)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock.Left) / (long)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelX() <= PT.X & PT.X <= (long)(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock.Left) + Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock.Width)) / (long)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelX() & (long)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock.Top) / (long)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelY() <= PT.Y & PT.Y <= (long)(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock.Top) + Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock.Height)) / (long)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelY())
            //        {
            //            if ((GetAsyncKeyState(RButtonID) & 0x8000) != 0)
            //            {
            //                // 右ボタンでスキップ
            //                return true;
            //            }
            //        }
            //    }
            //}
            //// メッセージウインドウ上でマウスボタンを押した場合
            //else if (My.MyProject.Forms.frmMessage.Handle.ToInt32() == GetForegroundWindow())
            //{
            //    GetCursorPos(ref PT);
            //    {
            //        var withBlock1 = My.MyProject.Forms.frmMessage;
            //        if ((long)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock1.Left) / (long)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelX() <= PT.X & PT.X <= (long)(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock1.Left) + Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock1.Width)) / (long)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelX() & (long)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock1.Top) / (long)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelY() <= PT.Y & PT.Y <= (long)(Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock1.Top) + Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock1.Height)) / (long)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelY())
            //        {
            //            if ((GetAsyncKeyState(RButtonID) & 0x8000) != 0)
            //            {
            //                // 右ボタンでスキップ
            //                return true;
            //            }
            //        }
            //    }
            //}

            return false;
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
            Program.Log.LogDebug(message);
        }

        public void LogInfo(string message)
        {
            Program.Log.LogInformation(message);
        }

        public void ChangeStatus(GuiStatus status)
        {
            switch (status)
            {
                case GuiStatus.WaitCursor:
                    Cursor.Current = Cursors.WaitCursor;
                    break;
                case GuiStatus.IBeam:
                    Cursor.Current = Cursors.IBeam;
                    break;
                default:
                    Cursor.Current = Cursors.Default;
                    break;
            }
        }

        public void ShowUnitCommandMenu(IList<UiCommand> commands)
        {
            MainForm.ShowUnitCommandMenu(commands);
        }

        public void ShowMapCommandMenu(IList<UiCommand> commands)
        {
            MainForm.ShowMapCommandMenu(commands);
        }

        public void UpdateScreen()
        {
            MainForm.UpdateScreen();
        }

        public void Sleep(int dwMilliseconds, bool withEvents = true)
        {
            if (withEvents)
            {
                Application.DoEvents();
            }
            Thread.Sleep(dwMilliseconds);
        }
    }
}
