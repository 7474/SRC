using SRCCore.Commands;
using SRCCore.Units;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace SRCCore.TestLib
{
    public class MockGUI : IGUI
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
        public GuiButton MouseButton { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double MouseX { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double MouseY { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double PrevMouseX { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double PrevMouseY { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
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
        public Color BGColor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool MessageFormVisible { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool MainFormVisible => throw new NotImplementedException();

        public Font CurrentPaintFont => throw new NotImplementedException();

        public Color CurrentPaintColor => throw new NotImplementedException();

        public string MainFormText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public UnitAbility AbilityListBox(Unit u, UnitAbilityList abilities, string caption_msg, string lb_mode, bool is_item = false)
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

        public void ChangeStatus(GuiStatus status)
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

        public void ClearScrean()
        {
            throw new NotImplementedException();
        }

        public void ClearScreen()
        {
            throw new NotImplementedException();
        }

        public void CloseListBox()
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

        public void Configure()
        {
            throw new NotImplementedException();
        }

        public GuiDialogResult Confirm(string message, string title, GuiConfirmOption option)
        {
            throw new NotImplementedException();
        }

        public void DataErrorMessage(string msg, string fname, int line_num, string line_buf, string dname)
        {
            Console.Error.WriteLine(msg);
        }

        public void DieAnimation(Unit u)
        {
            throw new NotImplementedException();
        }

        public void DisplayBattleMessage(string pname, string msg, string msg_mode = "")
        {
            throw new NotImplementedException();
        }

        public void DisplayGlobalMap()
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

        public void DoEvents()
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

        public void ExplodeAnimation(string tsize, int tx, int ty)
        {
            throw new NotImplementedException();
        }

        public bool GetKeyState(int key)
        {
            throw new NotImplementedException();
        }

        public GuiDialogResult Input(string message, string title, string defaultValue, out string value)
        {
            throw new NotImplementedException();
        }

        public bool IsRButtonPressed(bool ignore_message_wait = false)
        {
            throw new NotImplementedException();
        }

        public void KeepMessageFormStatus()
        {
            throw new NotImplementedException();
        }

        public int LIPS(ListBoxArgs args, int time_limit)
        {
            throw new NotImplementedException();
        }

        public int ListBox(ListBoxArgs args)
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

        public void MainFormHide()
        {
            throw new NotImplementedException();
        }

        public void MainFormShow()
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

        public SizeF MeasureString(string msg)
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

        public int MultiColumnListBox(ListBoxArgs args, bool is_center)
        {
            throw new NotImplementedException();
        }

        public int MultiSelectListBox(ListBoxArgs args, int max_num)
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

        public Stream OpenQuikSaveStream(FileAccess fileAccess)
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

        public void ResetDrawString()
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

        public Stream SelectSaveStream(SRCSaveKind saveKind, string defaultName)
        {
            throw new NotImplementedException();
        }

        public void SetDrawFont(DrawFontOption option)
        {
            throw new NotImplementedException();
        }

        public void SetDrawString(DrawStringMode mode)
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

        public void SetupBackground(string draw_mode = "", string draw_option = "", Color filter_color = default, double filter_trans_par = 0)
        {
            throw new NotImplementedException();
        }

        public void ShowMapCommandMenu(IList<UiCommand> commands)
        {
            throw new NotImplementedException();
        }

        public void ShowUnitCommandMenu(IList<UiCommand> commands)
        {
            throw new NotImplementedException();
        }

        public void Sleep(int dwMilliseconds, bool withEvents = true)
        {
            throw new NotImplementedException();
        }

        public bool Terminate()
        {
            throw new NotImplementedException();
        }

        public void TransionScrean(TransionPattern pattern, Color fillColor, int frame, int frameMillis)
        {
            throw new NotImplementedException();
        }

        public void UnlockGUI()
        {
            throw new NotImplementedException();
        }

        public void UpdateBaseX(int newX)
        {
            throw new NotImplementedException();
        }

        public void UpdateBaseY(int newY)
        {
            throw new NotImplementedException();
        }

        public void UpdateHotPoint()
        {
            throw new NotImplementedException();
        }

        public void UpdateMessageForm(Unit u1, Unit u2 = null)
        {
            throw new NotImplementedException();
        }

        public void UpdateScreen()
        {
            throw new NotImplementedException();
        }

        public UnitWeapon WeaponListBox(Unit u, UnitWeaponList weapons, string caption_msg, string lb_mode, string BGM = "")
        {
            throw new NotImplementedException();
        }
    }
}
