// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRC.Core.Units;

namespace SRC.Core
{
    // ユーザーインターフェースと画面描画の処理を行うためのインタフェース
    public interface IGUI
    {
        void LoadMainFormAndRegisterFlash();
        void LoadForms();
        void SetNewGUIMode();
        void OpenMessageForm(Unit u1 = null, Unit u2 = null);
        void CloseMessageForm();
        void ClearMessageForm();
        void UpdateMessageForm(Unit u1, Unit u2 = null);
        void SaveMessageFormStatus();
        void KeepMessageFormStatus();
        void DisplayMessage(string pname, string msg, string msg_mode = "");
        void PrintMessage(string msg, bool is_sys_msg = false);
        int MessageLen(string msg);
        void DisplayBattleMessage(string pname, string msg, string msg_mode = "");
        void DisplaySysMessage(string msg, bool int_wait = false);
        void SetupBackground(string draw_mode = "", string draw_option = "", int filter_color = 0, double filter_trans_par = 0d);
        void RedrawScreen(bool late_refresh = false);
        void MaskScreen();
        void RefreshScreen(bool without_refresh = false, bool delay_refresh = false);
        void Center(int new_x, int new_y);
        int MapToPixelX(int X);
        int MapToPixelY(int Y);
        int PixelToMapX(int X);
        int PixelToMapY(int Y);
        int MakeUnitBitmap(Unit u);
        // XXX
        //void LoadUnitBitmap(Unit u, PictureBox pic, int dx, int dy, bool use_orig_color = false, string fname = "");
        void PaintUnitBitmap(Unit u, string smode = "");
        void EraseUnitBitmap(int X, int Y, bool do_refresh = true);
        void MoveUnitBitmap(Unit u, int x1, int y1, int x2, int y2, int wait_time0, int division = 2);
        void MoveUnitBitmap2(Unit u, int wait_time0, int division = 2);
        int ListBox(string lb_caption, string[] list, string lb_info, string lb_mode = "");
        void EnlargeListBoxHeight();
        void ReduceListBoxHeight();
        void EnlargeListBoxWidth();
        void ReduceListBoxWidth();
        void AddPartsToListBox();
        void RemovePartsOnListBox();
        int WeaponListBox(Unit u, string caption_msg, string lb_mode, string BGM = "");
        int AbilityListBox(Unit u, string caption_msg, string lb_mode, bool is_item = false);
        int LIPS(string lb_caption, string[] list, string lb_info, int time_limit);
        int MultiColumnListBox(string lb_caption, string[] list, bool is_center);
        int MultiSelectListBox(string lb_caption, string[] list, string lb_info, int max_num);
        bool DrawPicture(string fname, int dx, int dy, int dw, int dh, int sx, int sy, int sw, int sh, string draw_option);
        void MakePicBuf();
        void DrawString(string msg, int X, int Y, bool without_cr = false);
        void DrawSysString(int X, int Y, string msg, bool without_refresh = false);
        void SaveScreen();
        void ClearPicture();
        void ClearPicture2(int x1, int y1, int x2, int y2);
        void LockGUI();
        void UnlockGUI();
        void SaveCursorPos();
        void MoveCursorPos(string cursor_mode, Unit t = null);
        void RestoreCursorPos();
        void OpenTitleForm();
        void CloseTitleForm();
        void OpenNowLoadingForm();
        void CloseNowLoadingForm();
        void DisplayLoadingProgress();
        void SetLoadImageSize(int new_size);
        void ChangeDisplaySize(int w, int h);
        void ErrorMessage(string msg);
        void DataErrorMessage(string msg, string fname, int line_num, string line_buf, string dname);
        bool IsRButtonPressed(bool ignore_message_wait = false);
        void DisplayTelop(string msg);
    }
}
