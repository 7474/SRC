using SRCCore.Commands;
using SRCCore.Exceptions;
using SRCCore.Units;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace SRCCore.TestLib
{
    public class MockGUI : IGUI
    {
        // ── プロパティ (自動実装バッキングフィールド) ────────────────────────────
        public bool IsGUILocked { get; set; }
        public bool IsInDeploymentSelection { get; set; }
        public int TopItem { get; set; }
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
        public GuiButton MouseButton { get; set; }
        public double MouseX { get; set; }
        public double MouseY { get; set; }
        public double PrevMouseX { get; set; }
        public double PrevMouseY { get; set; }
        public int PrevUnitX { get; set; }
        public int PrevUnitY { get; set; }
        public string PrevUnitArea { get; set; } = "";
        public string PrevCommand { get; set; } = "";
        public bool IsPictureDrawn { get; set; }
        public bool IsPictureVisible { get; set; }
        public int PaintedAreaX1 { get; set; }
        public int PaintedAreaY1 { get; set; }
        public int PaintedAreaX2 { get; set; }
        public int PaintedAreaY2 { get; set; }
        public bool IsCursorVisible { get; set; }
        public Color BGColor { get; set; }
        public string MainFormText { get; set; } = "";

        // ── 読み取り専用プロパティ (バッキングフィールド) ────────────────────────
        public bool MessageFormVisible { get; set; }
        public bool MainFormVisible { get; set; }
        public Font CurrentPaintFont { get; set; }
        public Color CurrentPaintColor { get; set; }

        // ── メソッドハンドラ ────────────────────────────────────────────────────

        public Func<Unit, UnitAbilityList, string, string, bool, UnitAbility> AbilityListBoxHandler { get; set; }
        public UnitAbility AbilityListBox(Unit u, UnitAbilityList abilities, string caption_msg, string lb_mode, bool is_item = false)
        {
            if (AbilityListBoxHandler != null) return AbilityListBoxHandler(u, abilities, caption_msg, lb_mode, is_item);
            throw new GUINotImplementedException(nameof(AbilityListBox));
        }

        public Action AddPartsToListBoxHandler { get; set; }
        public void AddPartsToListBox()
        {
            if (AddPartsToListBoxHandler != null) { AddPartsToListBoxHandler(); return; }
            throw new GUINotImplementedException(nameof(AddPartsToListBox));
        }

        public Action<int, int> CenterHandler { get; set; }
        public void Center(int new_x, int new_y)
        {
            if (CenterHandler != null) { CenterHandler(new_x, new_y); return; }
            throw new GUINotImplementedException(nameof(Center));
        }

        public Action<int, int> ChangeDisplaySizeHandler { get; set; }
        public void ChangeDisplaySize(int w, int h)
        {
            if (ChangeDisplaySizeHandler != null) { ChangeDisplaySizeHandler(w, h); return; }
            throw new GUINotImplementedException(nameof(ChangeDisplaySize));
        }

        public Action<GuiStatus> ChangeStatusHandler { get; set; }
        public void ChangeStatus(GuiStatus status)
        {
            if (ChangeStatusHandler != null) { ChangeStatusHandler(status); return; }
            throw new GUINotImplementedException(nameof(ChangeStatus));
        }

        public Action ClearMessageFormHandler { get; set; }
        public void ClearMessageForm()
        {
            if (ClearMessageFormHandler != null) { ClearMessageFormHandler(); return; }
            throw new GUINotImplementedException(nameof(ClearMessageForm));
        }

        public Action ClearPictureHandler { get; set; }
        public void ClearPicture()
        {
            if (ClearPictureHandler != null) { ClearPictureHandler(); return; }
            throw new GUINotImplementedException(nameof(ClearPicture));
        }

        public Action<int, int, int, int> ClearPicture2Handler { get; set; }
        public void ClearPicture2(int x1, int y1, int x2, int y2)
        {
            if (ClearPicture2Handler != null) { ClearPicture2Handler(x1, y1, x2, y2); return; }
            throw new GUINotImplementedException(nameof(ClearPicture2));
        }

        public Action ClearScreanHandler { get; set; }
        public void ClearScrean()
        {
            if (ClearScreanHandler != null) { ClearScreanHandler(); return; }
            throw new GUINotImplementedException(nameof(ClearScrean));
        }

        public Action ClearScreenHandler { get; set; }
        public void ClearScreen()
        {
            if (ClearScreenHandler != null) { ClearScreenHandler(); return; }
            throw new GUINotImplementedException(nameof(ClearScreen));
        }

        public Action CloseListBoxHandler { get; set; }
        public void CloseListBox()
        {
            if (CloseListBoxHandler != null) { CloseListBoxHandler(); return; }
            throw new GUINotImplementedException(nameof(CloseListBox));
        }

        public Action CloseMessageFormHandler { get; set; }
        public void CloseMessageForm()
        {
            if (CloseMessageFormHandler != null) { CloseMessageFormHandler(); return; }
            throw new GUINotImplementedException(nameof(CloseMessageForm));
        }

        public Action CloseNowLoadingFormHandler { get; set; }
        public void CloseNowLoadingForm()
        {
            if (CloseNowLoadingFormHandler != null) { CloseNowLoadingFormHandler(); return; }
            throw new GUINotImplementedException(nameof(CloseNowLoadingForm));
        }

        public Action CloseTitleFormHandler { get; set; }
        public void CloseTitleForm()
        {
            if (CloseTitleFormHandler != null) { CloseTitleFormHandler(); return; }
            throw new GUINotImplementedException(nameof(CloseTitleForm));
        }

        public Action ConfigureHandler { get; set; }
        public void Configure()
        {
            if (ConfigureHandler != null) { ConfigureHandler(); return; }
            throw new GUINotImplementedException(nameof(Configure));
        }

        public Func<string, string, GuiConfirmOption, GuiDialogResult> ConfirmHandler { get; set; }
        public GuiDialogResult Confirm(string message, string title, GuiConfirmOption option)
        {
            if (ConfirmHandler != null) return ConfirmHandler(message, title, option);
            throw new GUINotImplementedException(nameof(Confirm));
        }

        public void DataErrorMessage(string msg, string fname, int line_num, string line_buf, string dname)
        {
            Console.Error.WriteLine(msg);
        }

        public Action<Unit> DieAnimationHandler { get; set; }
        public void DieAnimation(Unit u)
        {
            if (DieAnimationHandler != null) { DieAnimationHandler(u); return; }
            throw new GUINotImplementedException(nameof(DieAnimation));
        }

        public Action<string, string, string> DisplayBattleMessageHandler { get; set; }
        public void DisplayBattleMessage(string pname, string msg, string msg_mode = "")
        {
            if (DisplayBattleMessageHandler != null) { DisplayBattleMessageHandler(pname, msg, msg_mode); return; }
            throw new GUINotImplementedException(nameof(DisplayBattleMessage));
        }

        public Action DisplayGlobalMapHandler { get; set; }
        public void DisplayGlobalMap()
        {
            if (DisplayGlobalMapHandler != null) { DisplayGlobalMapHandler(); return; }
            throw new GUINotImplementedException(nameof(DisplayGlobalMap));
        }

        public Action DisplayLoadingProgressHandler { get; set; }
        public void DisplayLoadingProgress()
        {
            if (DisplayLoadingProgressHandler != null) { DisplayLoadingProgressHandler(); return; }
            throw new GUINotImplementedException(nameof(DisplayLoadingProgress));
        }

        public Action<string, string, string> DisplayMessageHandler { get; set; }
        public void DisplayMessage(string pname, string msg, string msg_mode = "")
        {
            if (DisplayMessageHandler != null) { DisplayMessageHandler(pname, msg, msg_mode); return; }
            throw new GUINotImplementedException(nameof(DisplayMessage));
        }

        public Action<string, bool> DisplaySysMessageHandler { get; set; }
        public void DisplaySysMessage(string msg, bool int_wait = false)
        {
            if (DisplaySysMessageHandler != null) { DisplaySysMessageHandler(msg, int_wait); return; }
            throw new GUINotImplementedException(nameof(DisplaySysMessage));
        }

        public Action<string> DisplayTelopHandler { get; set; }
        public void DisplayTelop(string msg)
        {
            if (DisplayTelopHandler != null) { DisplayTelopHandler(msg); return; }
            throw new GUINotImplementedException(nameof(DisplayTelop));
        }

        public Action DoEventsHandler { get; set; }
        public void DoEvents()
        {
            if (DoEventsHandler != null) { DoEventsHandler(); return; }
            throw new GUINotImplementedException(nameof(DoEvents));
        }

        public Func<string, int, int, int, int, int, int, int, int, string, bool> DrawPictureHandler { get; set; }
        public bool DrawPicture(string fname, int dx, int dy, int dw, int dh, int sx, int sy, int sw, int sh, string draw_option)
        {
            if (DrawPictureHandler != null) return DrawPictureHandler(fname, dx, dy, dw, dh, sx, sy, sw, sh, draw_option);
            throw new GUINotImplementedException(nameof(DrawPicture));
        }

        public Action<string, int, int, bool> DrawStringHandler { get; set; }
        public void DrawString(string msg, int X, int Y, bool without_cr = false)
        {
            if (DrawStringHandler != null) { DrawStringHandler(msg, X, Y, without_cr); return; }
            throw new GUINotImplementedException(nameof(DrawString));
        }

        public Action<int, int, string, bool> DrawSysStringHandler { get; set; }
        public void DrawSysString(int X, int Y, string msg, bool without_refresh = false)
        {
            if (DrawSysStringHandler != null) { DrawSysStringHandler(X, Y, msg, without_refresh); return; }
            throw new GUINotImplementedException(nameof(DrawSysString));
        }

        public Action EnlargeListBoxHeightHandler { get; set; }
        public void EnlargeListBoxHeight()
        {
            if (EnlargeListBoxHeightHandler != null) { EnlargeListBoxHeightHandler(); return; }
            throw new GUINotImplementedException(nameof(EnlargeListBoxHeight));
        }

        public Action EnlargeListBoxWidthHandler { get; set; }
        public void EnlargeListBoxWidth()
        {
            if (EnlargeListBoxWidthHandler != null) { EnlargeListBoxWidthHandler(); return; }
            throw new GUINotImplementedException(nameof(EnlargeListBoxWidth));
        }

        public Action<int, int, bool> EraseUnitBitmapHandler { get; set; }
        public void EraseUnitBitmap(int X, int Y, bool do_refresh = true)
        {
            if (EraseUnitBitmapHandler != null) { EraseUnitBitmapHandler(X, Y, do_refresh); return; }
            throw new GUINotImplementedException(nameof(EraseUnitBitmap));
        }

        public void ErrorMessage(string msg)
        {
            Console.Error.WriteLine(msg);
        }

        public Action<string, int, int> ExplodeAnimationHandler { get; set; }
        public void ExplodeAnimation(string tsize, int tx, int ty)
        {
            if (ExplodeAnimationHandler != null) { ExplodeAnimationHandler(tsize, tx, ty); return; }
            throw new GUINotImplementedException(nameof(ExplodeAnimation));
        }

        public Func<int, bool> GetKeyStateHandler { get; set; }
        public bool GetKeyState(int key)
        {
            if (GetKeyStateHandler != null) return GetKeyStateHandler(key);
            throw new GUINotImplementedException(nameof(GetKeyState));
        }

        public Func<string, string, string, (GuiDialogResult result, string value)> InputHandler { get; set; }
        public GuiDialogResult Input(string message, string title, string defaultValue, out string value)
        {
            if (InputHandler != null)
            {
                var r = InputHandler(message, title, defaultValue);
                value = r.value;
                return r.result;
            }
            value = null;
            throw new GUINotImplementedException(nameof(Input));
        }

        public Func<bool, bool> IsRButtonPressedHandler { get; set; }
        public bool IsRButtonPressed(bool ignore_message_wait = false)
        {
            if (IsRButtonPressedHandler != null) return IsRButtonPressedHandler(ignore_message_wait);
            throw new GUINotImplementedException(nameof(IsRButtonPressed));
        }

        public Action KeepMessageFormStatusHandler { get; set; }
        public void KeepMessageFormStatus()
        {
            if (KeepMessageFormStatusHandler != null) { KeepMessageFormStatusHandler(); return; }
            throw new GUINotImplementedException(nameof(KeepMessageFormStatus));
        }

        public Func<ListBoxArgs, int, int> LIPSHandler { get; set; }
        public int LIPS(ListBoxArgs args, int time_limit)
        {
            if (LIPSHandler != null) return LIPSHandler(args, time_limit);
            throw new GUINotImplementedException(nameof(LIPS));
        }

        public Func<ListBoxArgs, int> ListBoxHandler { get; set; }
        public int ListBox(ListBoxArgs args)
        {
            if (ListBoxHandler != null) return ListBoxHandler(args);
            throw new GUINotImplementedException(nameof(ListBox));
        }

        public Action LoadFormsHandler { get; set; }
        public void LoadForms()
        {
            if (LoadFormsHandler != null) { LoadFormsHandler(); return; }
            throw new GUINotImplementedException(nameof(LoadForms));
        }

        public Action LoadMainFormAndRegisterFlashHandler { get; set; }
        public void LoadMainFormAndRegisterFlash()
        {
            if (LoadMainFormAndRegisterFlashHandler != null) { LoadMainFormAndRegisterFlashHandler(); return; }
            throw new GUINotImplementedException(nameof(LoadMainFormAndRegisterFlash));
        }

        public Action LockGUIHandler { get; set; }
        public void LockGUI()
        {
            if (LockGUIHandler != null) { LockGUIHandler(); return; }
            throw new GUINotImplementedException(nameof(LockGUI));
        }

        public Action MainFormHideHandler { get; set; }
        public void MainFormHide()
        {
            if (MainFormHideHandler != null) { MainFormHideHandler(); return; }
            throw new GUINotImplementedException(nameof(MainFormHide));
        }

        public Action MainFormShowHandler { get; set; }
        public void MainFormShow()
        {
            if (MainFormShowHandler != null) { MainFormShowHandler(); return; }
            throw new GUINotImplementedException(nameof(MainFormShow));
        }

        public Func<int, int> MapToPixelXHandler { get; set; }
        public int MapToPixelX(int X)
        {
            if (MapToPixelXHandler != null) return MapToPixelXHandler(X);
            throw new GUINotImplementedException(nameof(MapToPixelX));
        }

        public Func<int, int> MapToPixelYHandler { get; set; }
        public int MapToPixelY(int Y)
        {
            if (MapToPixelYHandler != null) return MapToPixelYHandler(Y);
            throw new GUINotImplementedException(nameof(MapToPixelY));
        }

        public Action MaskScreenHandler { get; set; }
        public void MaskScreen()
        {
            if (MaskScreenHandler != null) { MaskScreenHandler(); return; }
            throw new GUINotImplementedException(nameof(MaskScreen));
        }

        public Func<string, SizeF> MeasureStringHandler { get; set; }
        public SizeF MeasureString(string msg)
        {
            if (MeasureStringHandler != null) return MeasureStringHandler(msg);
            throw new GUINotImplementedException(nameof(MeasureString));
        }

        public Func<string, int> MessageLenHandler { get; set; }
        public int MessageLen(string msg)
        {
            if (MessageLenHandler != null) return MessageLenHandler(msg);
            throw new GUINotImplementedException(nameof(MessageLen));
        }

        public Action<string, Unit> MoveCursorPosHandler { get; set; }
        public void MoveCursorPos(string cursor_mode, Unit t = null)
        {
            if (MoveCursorPosHandler != null) { MoveCursorPosHandler(cursor_mode, t); return; }
            throw new GUINotImplementedException(nameof(MoveCursorPos));
        }

        public Action<Unit, int, int, int, int, int, int> MoveUnitBitmapHandler { get; set; }
        public void MoveUnitBitmap(Unit u, int x1, int y1, int x2, int y2, int wait_time0, int division = 2)
        {
            if (MoveUnitBitmapHandler != null) { MoveUnitBitmapHandler(u, x1, y1, x2, y2, wait_time0, division); return; }
            throw new GUINotImplementedException(nameof(MoveUnitBitmap));
        }

        public Action<Unit, int, int> MoveUnitBitmap2Handler { get; set; }
        public void MoveUnitBitmap2(Unit u, int wait_time0, int division = 2)
        {
            if (MoveUnitBitmap2Handler != null) { MoveUnitBitmap2Handler(u, wait_time0, division); return; }
            throw new GUINotImplementedException(nameof(MoveUnitBitmap2));
        }

        public Func<ListBoxArgs, bool, int> MultiColumnListBoxHandler { get; set; }
        public int MultiColumnListBox(ListBoxArgs args, bool is_center)
        {
            if (MultiColumnListBoxHandler != null) return MultiColumnListBoxHandler(args, is_center);
            throw new GUINotImplementedException(nameof(MultiColumnListBox));
        }

        public Func<ListBoxArgs, int, int> MultiSelectListBoxHandler { get; set; }
        public int MultiSelectListBox(ListBoxArgs args, int max_num)
        {
            if (MultiSelectListBoxHandler != null) return MultiSelectListBoxHandler(args, max_num);
            throw new GUINotImplementedException(nameof(MultiSelectListBox));
        }

        public Action<Unit, Unit> OpenMessageFormHandler { get; set; }
        public void OpenMessageForm(Unit u1 = null, Unit u2 = null)
        {
            if (OpenMessageFormHandler != null) { OpenMessageFormHandler(u1, u2); return; }
            throw new GUINotImplementedException(nameof(OpenMessageForm));
        }

        public Action OpenNowLoadingFormHandler { get; set; }
        public void OpenNowLoadingForm()
        {
            if (OpenNowLoadingFormHandler != null) { OpenNowLoadingFormHandler(); return; }
            throw new GUINotImplementedException(nameof(OpenNowLoadingForm));
        }

        public Func<FileAccess, Stream> OpenQuikSaveStreamHandler { get; set; }
        public Stream OpenQuikSaveStream(FileAccess fileAccess)
        {
            if (OpenQuikSaveStreamHandler != null) return OpenQuikSaveStreamHandler(fileAccess);
            throw new GUINotImplementedException(nameof(OpenQuikSaveStream));
        }

        public Action OpenTitleFormHandler { get; set; }
        public void OpenTitleForm()
        {
            if (OpenTitleFormHandler != null) { OpenTitleFormHandler(); return; }
            throw new GUINotImplementedException(nameof(OpenTitleForm));
        }

        public Action<Unit, string> PaintUnitBitmapHandler { get; set; }
        public void PaintUnitBitmap(Unit u, string smode = "")
        {
            if (PaintUnitBitmapHandler != null) { PaintUnitBitmapHandler(u, smode); return; }
            throw new GUINotImplementedException(nameof(PaintUnitBitmap));
        }

        public Func<int, int> PixelToMapXHandler { get; set; }
        public int PixelToMapX(int X)
        {
            if (PixelToMapXHandler != null) return PixelToMapXHandler(X);
            throw new GUINotImplementedException(nameof(PixelToMapX));
        }

        public Func<int, int> PixelToMapYHandler { get; set; }
        public int PixelToMapY(int Y)
        {
            if (PixelToMapYHandler != null) return PixelToMapYHandler(Y);
            throw new GUINotImplementedException(nameof(PixelToMapY));
        }

        public Action<string, bool> PrintMessageHandler { get; set; }
        public void PrintMessage(string msg, bool is_sys_msg = false)
        {
            if (PrintMessageHandler != null) { PrintMessageHandler(msg, is_sys_msg); return; }
            throw new GUINotImplementedException(nameof(PrintMessage));
        }

        public Action<bool> RedrawScreenHandler { get; set; }
        public void RedrawScreen(bool late_refresh = false)
        {
            if (RedrawScreenHandler != null) { RedrawScreenHandler(late_refresh); return; }
            throw new GUINotImplementedException(nameof(RedrawScreen));
        }

        public Action ReduceListBoxHeightHandler { get; set; }
        public void ReduceListBoxHeight()
        {
            if (ReduceListBoxHeightHandler != null) { ReduceListBoxHeightHandler(); return; }
            throw new GUINotImplementedException(nameof(ReduceListBoxHeight));
        }

        public Action ReduceListBoxWidthHandler { get; set; }
        public void ReduceListBoxWidth()
        {
            if (ReduceListBoxWidthHandler != null) { ReduceListBoxWidthHandler(); return; }
            throw new GUINotImplementedException(nameof(ReduceListBoxWidth));
        }

        public Action<bool, bool> RefreshScreenHandler { get; set; }
        public void RefreshScreen(bool without_refresh = false, bool delay_refresh = false)
        {
            if (RefreshScreenHandler != null) { RefreshScreenHandler(without_refresh, delay_refresh); return; }
            throw new GUINotImplementedException(nameof(RefreshScreen));
        }

        public Action RemovePartsOnListBoxHandler { get; set; }
        public void RemovePartsOnListBox()
        {
            if (RemovePartsOnListBoxHandler != null) { RemovePartsOnListBoxHandler(); return; }
            throw new GUINotImplementedException(nameof(RemovePartsOnListBox));
        }

        public Action ResetDrawStringHandler { get; set; }
        public void ResetDrawString()
        {
            if (ResetDrawStringHandler != null) { ResetDrawStringHandler(); return; }
            throw new GUINotImplementedException(nameof(ResetDrawString));
        }

        public Action RestoreCursorPosHandler { get; set; }
        public void RestoreCursorPos()
        {
            if (RestoreCursorPosHandler != null) { RestoreCursorPosHandler(); return; }
            throw new GUINotImplementedException(nameof(RestoreCursorPos));
        }

        public Action SaveCursorPosHandler { get; set; }
        public void SaveCursorPos()
        {
            if (SaveCursorPosHandler != null) { SaveCursorPosHandler(); return; }
            throw new GUINotImplementedException(nameof(SaveCursorPos));
        }

        public Action SaveMessageFormStatusHandler { get; set; }
        public void SaveMessageFormStatus()
        {
            if (SaveMessageFormStatusHandler != null) { SaveMessageFormStatusHandler(); return; }
            throw new GUINotImplementedException(nameof(SaveMessageFormStatus));
        }

        public Action SaveScreenHandler { get; set; }
        public void SaveScreen()
        {
            if (SaveScreenHandler != null) { SaveScreenHandler(); return; }
            throw new GUINotImplementedException(nameof(SaveScreen));
        }

        public string SelectLoadFile(string title, string initialDirectory, string fileType, string fileExtension)
        {
            return "";
        }

        public string SelectSaveFile(string title, string initialDirectory, string initialFile, string fileType, string fileExtension)
        {
            return "";
        }

        public Func<SRCSaveKind, string, Stream> SelectSaveStreamHandler { get; set; }
        public Stream SelectSaveStream(SRCSaveKind saveKind, string defaultName)
        {
            if (SelectSaveStreamHandler != null) return SelectSaveStreamHandler(saveKind, defaultName);
            throw new GUINotImplementedException(nameof(SelectSaveStream));
        }

        public Action<DrawFontOption> SetDrawFontHandler { get; set; }
        public void SetDrawFont(DrawFontOption option)
        {
            if (SetDrawFontHandler != null) { SetDrawFontHandler(option); return; }
            throw new GUINotImplementedException(nameof(SetDrawFont));
        }

        public Action<DrawStringMode> SetDrawStringHandler { get; set; }
        public void SetDrawString(DrawStringMode mode)
        {
            if (SetDrawStringHandler != null) { SetDrawStringHandler(mode); return; }
            throw new GUINotImplementedException(nameof(SetDrawString));
        }

        public Action<int> SetLoadImageSizeHandler { get; set; }
        public void SetLoadImageSize(int new_size)
        {
            if (SetLoadImageSizeHandler != null) { SetLoadImageSizeHandler(new_size); return; }
            throw new GUINotImplementedException(nameof(SetLoadImageSize));
        }

        public Action SetNewGUIModeHandler { get; set; }
        public void SetNewGUIMode()
        {
            if (SetNewGUIModeHandler != null) { SetNewGUIModeHandler(); return; }
            throw new GUINotImplementedException(nameof(SetNewGUIMode));
        }

        public Action<string> SetTitleHandler { get; set; }
        public void SetTitle(string title)
        {
            if (SetTitleHandler != null) { SetTitleHandler(title); return; }
            throw new GUINotImplementedException(nameof(SetTitle));
        }

        public Action<string, string, Color, double> SetupBackgroundHandler { get; set; }
        public void SetupBackground(string draw_mode = "", string draw_option = "", Color filter_color = default, double filter_trans_par = 0)
        {
            if (SetupBackgroundHandler != null) { SetupBackgroundHandler(draw_mode, draw_option, filter_color, filter_trans_par); return; }
            throw new GUINotImplementedException(nameof(SetupBackground));
        }

        public Action<IList<UiCommand>> ShowMapCommandMenuHandler { get; set; }
        public void ShowMapCommandMenu(IList<UiCommand> commands)
        {
            if (ShowMapCommandMenuHandler != null) { ShowMapCommandMenuHandler(commands); return; }
            throw new GUINotImplementedException(nameof(ShowMapCommandMenu));
        }

        public Action<IList<UiCommand>> ShowUnitCommandMenuHandler { get; set; }
        public void ShowUnitCommandMenu(IList<UiCommand> commands)
        {
            if (ShowUnitCommandMenuHandler != null) { ShowUnitCommandMenuHandler(commands); return; }
            throw new GUINotImplementedException(nameof(ShowUnitCommandMenu));
        }

        public Action<int, bool> SleepHandler { get; set; }
        public void Sleep(int dwMilliseconds, bool withEvents = true)
        {
            if (SleepHandler != null) { SleepHandler(dwMilliseconds, withEvents); return; }
            throw new GUINotImplementedException(nameof(Sleep));
        }

        public bool TerminateCalled { get; private set; }
        public bool Terminate()
        {
            TerminateCalled = true;
            // false を返すことで TerminateSRC() が Environment.Exit(0) を呼ばないようにする
            return false;
        }

        public Action<TransionPattern, Color, int, int> TransionScreanHandler { get; set; }
        public void TransionScrean(TransionPattern pattern, Color fillColor, int frame, int frameMillis)
        {
            if (TransionScreanHandler != null) { TransionScreanHandler(pattern, fillColor, frame, frameMillis); return; }
            throw new GUINotImplementedException(nameof(TransionScrean));
        }

        public Action UnlockGUIHandler { get; set; }
        public void UnlockGUI()
        {
            if (UnlockGUIHandler != null) { UnlockGUIHandler(); return; }
            throw new GUINotImplementedException(nameof(UnlockGUI));
        }

        public Action<int> UpdateBaseXHandler { get; set; }
        public void UpdateBaseX(int newX)
        {
            if (UpdateBaseXHandler != null) { UpdateBaseXHandler(newX); return; }
            throw new GUINotImplementedException(nameof(UpdateBaseX));
        }

        public Action<int> UpdateBaseYHandler { get; set; }
        public void UpdateBaseY(int newY)
        {
            if (UpdateBaseYHandler != null) { UpdateBaseYHandler(newY); return; }
            throw new GUINotImplementedException(nameof(UpdateBaseY));
        }

        public Action UpdateHotPointHandler { get; set; }
        public void UpdateHotPoint()
        {
            if (UpdateHotPointHandler != null) { UpdateHotPointHandler(); return; }
            throw new GUINotImplementedException(nameof(UpdateHotPoint));
        }

        public Action<Unit, Unit> UpdateMessageFormHandler { get; set; }
        public void UpdateMessageForm(Unit u1, Unit u2 = null)
        {
            if (UpdateMessageFormHandler != null) { UpdateMessageFormHandler(u1, u2); return; }
            throw new GUINotImplementedException(nameof(UpdateMessageForm));
        }

        public Action UpdateScreenHandler { get; set; }
        public void UpdateScreen()
        {
            if (UpdateScreenHandler != null) { UpdateScreenHandler(); return; }
            throw new GUINotImplementedException(nameof(UpdateScreen));
        }

        public Func<Unit, UnitWeaponList, string, string, string, UnitWeapon> WeaponListBoxHandler { get; set; }
        public UnitWeapon WeaponListBox(Unit u, UnitWeaponList weapons, string caption_msg, string lb_mode, string BGM = "")
        {
            if (WeaponListBoxHandler != null) return WeaponListBoxHandler(u, weapons, caption_msg, lb_mode, BGM);
            throw new GUINotImplementedException(nameof(WeaponListBox));
        }
    }
}
