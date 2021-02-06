using SRC.Core;
using SRC.Core.Units;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRCCoreTests.TestLib
{
    class MockGUI : IGUI
    {
        public bool IsGUILocked { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int TopItem { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool MessageWindowIsOut { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsFormClicked { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsMordal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MessageWait { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool AutoMessageMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool HCentering { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool VCentering { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool PermanentStringMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool KeepStringMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MainWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MainHeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MainPWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MainPHeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MapPWidth { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MapPHeight { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ScreenIsMasked { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ScreenIsSaved { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MapX { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MapY { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int PrevMapX { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int PrevMapY { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int MouseButton { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float MouseX { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float MouseY { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float PrevMouseX { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public float PrevMouseY { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int PrevUnitX { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int PrevUnitY { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string PrevUnitArea { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string PrevCommand { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsPictureDrawn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsPictureVisible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int PaintedAreaX1 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int PaintedAreaY1 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int PaintedAreaX2 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int PaintedAreaY2 { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsCursorVisible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int BGColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool MessageFormVisible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int AbilityListBox(Unit u, string caption_msg, string lb_mode, bool is_item = false)
        {
            throw new NotImplementedException();
        }

        public void AddPartsToListBox()
        {
            throw new NotImplementedException();
        }

        public void Center(int new_x, int new_y)
        {
            throw new NotImplementedException();
        }

        public void ChangeDisplaySize(int w, int h)
        {
            throw new NotImplementedException();
        }

        public void ClearMessageForm()
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

        public void CloseMessageForm()
        {
            throw new NotImplementedException();
        }

        public void CloseNowLoadingForm()
        {
            throw new NotImplementedException();
        }

        public void CloseTitleForm()
        {
            throw new NotImplementedException();
        }

        public void DataErrorMessage(string msg, string fname, int line_num, string line_buf, string dname)
        {
            Console.Error.WriteLine(msg);
        }

        public void DisplayBattleMessage(string pname, string msg, string msg_mode = "")
        {
            throw new NotImplementedException();
        }

        public void DisplayLoadingProgress()
        {
            throw new NotImplementedException();
        }

        public void DisplayMessage(string pname, string msg, string msg_mode = "")
        {
            throw new NotImplementedException();
        }

        public void DisplaySysMessage(string msg, bool int_wait = false)
        {
            throw new NotImplementedException();
        }

        public void DisplayTelop(string msg)
        {
            throw new NotImplementedException();
        }

        public bool DrawPicture(string fname, int dx, int dy, int dw, int dh, int sx, int sy, int sw, int sh, string draw_option)
        {
            throw new NotImplementedException();
        }

        public void DrawString(string msg, int X, int Y, bool without_cr = false)
        {
            throw new NotImplementedException();
        }

        public void DrawSysString(int X, int Y, string msg, bool without_refresh = false)
        {
            throw new NotImplementedException();
        }

        public void EnlargeListBoxHeight()
        {
            throw new NotImplementedException();
        }

        public void EnlargeListBoxWidth()
        {
            throw new NotImplementedException();
        }

        public void EraseUnitBitmap(int X, int Y, bool do_refresh = true)
        {
            throw new NotImplementedException();
        }

        public void ErrorMessage(string msg)
        {
            Console.Error.WriteLine(msg);
        }

        public bool IsRButtonPressed(bool ignore_message_wait = false)
        {
            throw new NotImplementedException();
        }

        public void KeepMessageFormStatus()
        {
            throw new NotImplementedException();
        }

        public int LIPS(string lb_caption, string[] list, string lb_info, int time_limit)
        {
            throw new NotImplementedException();
        }

        public int ListBox(string lb_caption, string[] list, string lb_info, string lb_mode = "")
        {
            throw new NotImplementedException();
        }

        public void LoadForms()
        {
            throw new NotImplementedException();
        }

        public void LoadMainFormAndRegisterFlash()
        {
            throw new NotImplementedException();
        }

        public void LockGUI()
        {
            throw new NotImplementedException();
        }

        public void MakePicBuf()
        {
            throw new NotImplementedException();
        }

        public int MakeUnitBitmap(Unit u)
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

        public void MaskScreen()
        {
            throw new NotImplementedException();
        }

        public int MessageLen(string msg)
        {
            throw new NotImplementedException();
        }

        public void MoveCursorPos(string cursor_mode, Unit t = null)
        {
            throw new NotImplementedException();
        }

        public void MoveUnitBitmap(Unit u, int x1, int y1, int x2, int y2, int wait_time0, int division = 2)
        {
            throw new NotImplementedException();
        }

        public void MoveUnitBitmap2(Unit u, int wait_time0, int division = 2)
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

        public void OpenMessageForm(Unit u1 = null, Unit u2 = null)
        {
            throw new NotImplementedException();
        }

        public void OpenNowLoadingForm()
        {
            throw new NotImplementedException();
        }

        public void OpenTitleForm()
        {
            throw new NotImplementedException();
        }

        public void PaintUnitBitmap(Unit u, string smode = "")
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

        public void PrintMessage(string msg, bool is_sys_msg = false)
        {
            throw new NotImplementedException();
        }

        public void RedrawScreen(bool late_refresh = false)
        {
            throw new NotImplementedException();
        }

        public void ReduceListBoxHeight()
        {
            throw new NotImplementedException();
        }

        public void ReduceListBoxWidth()
        {
            throw new NotImplementedException();
        }

        public void RefreshScreen(bool without_refresh = false, bool delay_refresh = false)
        {
            throw new NotImplementedException();
        }

        public void RemovePartsOnListBox()
        {
            throw new NotImplementedException();
        }

        public void RestoreCursorPos()
        {
            throw new NotImplementedException();
        }

        public void SaveCursorPos()
        {
            throw new NotImplementedException();
        }

        public void SaveMessageFormStatus()
        {
            throw new NotImplementedException();
        }

        public void SaveScreen()
        {
            throw new NotImplementedException();
        }

        public void SetLoadImageSize(int new_size)
        {
            throw new NotImplementedException();
        }

        public void SetNewGUIMode()
        {
            throw new NotImplementedException();
        }

        public void SetTitle(string title)
        {
            throw new NotImplementedException();
        }

        public void SetupBackground(string draw_mode = "", string draw_option = "", int filter_color = 0, double filter_trans_par = 0)
        {
            throw new NotImplementedException();
        }

        public void UnlockGUI()
        {
            throw new NotImplementedException();
        }

        public void UpdateMessageForm(Unit u1, Unit u2 = null)
        {
            throw new NotImplementedException();
        }

        public int WeaponListBox(Unit u, string caption_msg, string lb_mode, string BGM = "")
        {
            throw new NotImplementedException();
        }
    }
}
