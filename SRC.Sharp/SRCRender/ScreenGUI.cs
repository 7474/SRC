using SkiaSharp;
using SRCCore;
using SRCCore.Commands;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Pilots;
using SRCCore.Threading;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;

namespace SRCRender
{
    /// <summary>
    /// SkiaSharp で描画する GUI 実装。UI フレームワークには依存しない。
    /// SRCCore からはゲームスレッドで呼ばれる前提で、待ち（クリック待ち・選択待ち）はゲームスレッドで行う。
    /// 表示側（IScreenHost）は描画済みの画面を表示し、入力を Pointer* / Key* メソッドで渡す。
    /// </summary>
    public partial class ScreenGUI : IGUI, IGUIMap, IGUIStatus, IGUIScrean, IDisposable
    {
        private const int WaitIntervalMillis = 20;

        private readonly SRC src;
        private readonly GameThread game;
        private readonly IScreenHost host;
        private readonly ScreenRenderer renderer = new ScreenRenderer();
        private readonly object renderLock = new object();
        private readonly SKBitmap frame = new SKBitmap(ScreenLayout.Width, ScreenLayout.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
        private ImageCache images;

        // UI スレッドから更新される入力状態
        private volatile bool isFormClicked;
        private volatile bool isRightButtonDown;
        private volatile int listResult = -1;
        private volatile int dialogResult = -1;

        // メッセージウィンドウ（WinForms 版の frmMessage に相当）
        private readonly Compat.MessageWindow frmMessage = new Compat.MessageWindow();

        // メッセージウィンドウに表示中のユニットなど（WinForms 版と同じ）
        private string messageDisplayedPilot;
        private string DisplayMode;
        private Unit RightUnit;
        private Unit LeftUnit;
        private double RightUnitHPRatio;
        private double LeftUnitHPRatio;
        private double RightUnitENRatio;
        private double LeftUnitENRatio;
        private bool _savedMessageFormVisible;
        private Unit _savedLeftUnit;
        private Unit _savedRightUnit;

        // ステータスウィンドウ（WinForms 版の frmMain の picFace / picPilotStatus / picUnitStatus に相当）。
        // 位置は ScreenLayout.StatusArea からの相対座標
        private readonly Compat.PictureBox picFace = new Compat.PictureBox { Left = 4, Top = 4, Width = 68, Height = 68, StretchImage = true };
        private readonly Compat.PictureBox picPilotStatus = new Compat.PictureBox { Left = 76, Top = 4, Width = 155, Height = 72 };
        private readonly Compat.PictureBox picUnitStatus = new Compat.PictureBox { Left = 4, Top = 76, Width = 230, Height = ScreenLayout.StatusHeightPx - 76 };

        /// <summary>メッセージウィンドウ</summary>
        public Compat.MessageWindow MessageWindow => frmMessage;

        public ScreenGUI(SRC src, GameThread game, IScreenHost host)
        {
            this.src = src;
            this.game = game;
            this.host = host;
            frmMessage.SetRefreshHandler(Present);
            picFace.Refreshed = Present;
            picPilotStatus.Refreshed = Present;
            picUnitStatus.Refreshed = Present;
            MainWidth = ScreenLayout.MapColumns;
            MainHeight = ScreenLayout.MapRows;
            MessageWait = 700;
        }

        /// <summary>画面に表示する内容</summary>
        public ScreenState State { get; } = new ScreenState();

        public ScreenRenderer Renderer => renderer;

        private ImageCache Images => images ?? (images = new ImageCache(src));
        private Map Map => src.Map;
        // WinForms 版から移植したコードとの互換のため
        private SRC SRC => src;
        private IGUI GUI => this;
        private const int MapCellPx = ScreenLayout.CellPx;
        private SKBitmap BackgroundBuffer => backgroundBuffer;
        private SRCCore.Expressions.Expression Expression => src.Expression;
        private Sound Sound => src.Sound;
        private SRCCore.Events.Event Event => src.Event;
        private Effect Effect => src.Effect;
        private Command Commands => src.Commands;

        /// <summary>
        /// SRC に GUI 系インターフェースの実装として設定する。
        /// </summary>
        public void Install()
        {
            src.GUI = this;
            src.GUIMap = this;
            src.GUIStatus = this;
            src.GUIScrean = this;
        }

        public void Dispose()
        {
            renderer.Dispose();
            frame.Dispose();
            DisposeMapBuffers();
        }

        #region 描画

        /// <summary>
        /// 画面を描画して表示側へ渡す。
        /// </summary>
        public void Present()
        {
            lock (renderLock)
            {
                State.MapX = MapX;
                State.MapY = MapY;
                using (var canvas = new SKCanvas(frame))
                {
                    renderer.Render(canvas, State, new ScreenRenderer.ScreenLayers(displayBuffer, new[] { picFace, picPilotStatus, picUnitStatus }, frmMessage));
                }
                host.Present(frame);
            }
        }

        /// <summary>
        /// 現在の画面を PNG として書き出す（テスト・デバッグ用）。
        /// </summary>
        public void SnapshotPng(Stream stream)
        {
            lock (renderLock)
            {
                using (var image = SKImage.FromBitmap(frame))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                {
                    data.SaveTo(stream);
                }
            }
        }

        #endregion

        #region 待ち

        public void DoEvents()
        {
            if (game.IsCurrentThread)
            {
                game.ProcessPending();
            }
        }

        public void Sleep(int dwMilliseconds, bool withEvents = true)
        {
            if (withEvents)
            {
                DoEvents();
            }
            if (dwMilliseconds > 0)
            {
                Thread.Sleep(dwMilliseconds);
            }
        }

        /// <summary>
        /// 条件を満たすまでゲームスレッドで待つ。待っている間も UI からの処理は実行する。
        /// </summary>
        private bool WaitUntil(Func<bool> condition, int timeoutMillis = -1)
        {
            var start = Environment.TickCount;
            while (!condition())
            {
                if (timeoutMillis >= 0 && Environment.TickCount - start >= timeoutMillis)
                {
                    return false;
                }
                Sleep(WaitIntervalMillis);
            }
            return true;
        }

        #endregion

        #region 入力（UI スレッドから呼ばれる）

        /// <summary>
        /// 画面上でポインタが押された。座標は画面（ScreenLayout）のピクセル座標。
        /// </summary>
        public void PointerPressed(float x, float y, GuiButton button)
        {
            if (button == GuiButton.Right)
            {
                isRightButtonDown = true;
            }
            MouseButton = button;
            MouseX = x;
            MouseY = y;

            var dialog = State.Dialog;
            if (dialog != null)
            {
                if (button == GuiButton.Left)
                {
                    var index = renderer.HitTestDialog(dialog, x, y);
                    if (index >= 0)
                    {
                        dialogResult = index;
                    }
                }
                return;
            }

            // メッセージウィンドウのクリック（顔グラフィックのクリックで自動送りの切り替え）
            if (frmMessage.Visible && button == GuiButton.Left)
            {
                var origin = renderer.MessageWindowOrigin(frmMessage);
                var mx = x - origin.X;
                var my = y - origin.Y;
                if (mx >= 0 && my >= 0 && mx < frmMessage.Width && my < frmMessage.Height)
                {
                    var face = frmMessage.picFace;
                    if (mx >= face.Left && my >= face.Top && mx < face.Left + face.Width && my < face.Top + face.Height && MessageWait < 10000)
                    {
                        AutoMessageMode = !AutoMessageMode;
                    }
                    isFormClicked = true;
                    return;
                }
            }

            var list = State.List;
            if (list != null && listResult < 0 && waitingList)
            {
                if (button == GuiButton.Right)
                {
                    listResult = 0;
                }
                else
                {
                    var index = renderer.HitTestList(list, x, y);
                    if (index >= 0 && !list.Items[index].Disabled)
                    {
                        listResult = index + 1;
                    }
                }
                return;
            }

            var menu = State.Menu;
            if (menu != null)
            {
                var commands = menuCommands;
                var isUnitMenu = isUnitCommandMenu;
                var index = button == GuiButton.Left ? renderer.HitTestMenu(menu, x, y) : -1;
                State.Menu = null;
                menuCommands = null;
                game.Post(() =>
                {
                    if (index >= 0 && commands != null)
                    {
                        if (isUnitMenu)
                        {
                            Commands.UnitCommand(commands[index]);
                        }
                        else
                        {
                            Commands.MapCommand(commands[index]);
                        }
                    }
                    else
                    {
                        Commands.CancelCommand();
                    }
                    Present();
                });
                return;
            }

            // ＧＵＩロック中は単なるクリックとして処理
            if (IsGUILocked)
            {
                SimpleClick();
                return;
            }

            if (ScreenLayout.IsInMap((int)x, (int)y))
            {
                var mx = PixelToMapX((int)x);
                var my = PixelToMapY((int)y);
                game.Post(() =>
                {
                    Commands.ProceedInput(button, Map.CellAtPoint(mx, my), Map.UnitAtPoint(mx, my));
                    Present();
                });
            }
            else
            {
                SimpleClick();
            }
        }

        public void PointerReleased(float x, float y, GuiButton button)
        {
            if (button == GuiButton.Right)
            {
                isRightButtonDown = false;
            }
        }

        private (int x, int y) lastHoverCell;

        public void PointerMoved(float x, float y)
        {
            MouseX = x;
            MouseY = y;
            HoverMap((int)x, (int)y);
            var list = State.List;
            if (list != null)
            {
                var index = renderer.HitTestList(list, x, y);
                if (index != list.Highlight)
                {
                    list.Highlight = index;
                    game.Post(Present);
                }
                return;
            }
            var menu = State.Menu;
            if (menu != null)
            {
                var index = renderer.HitTestMenu(menu, x, y);
                if (index != menu.Highlight)
                {
                    menu.Highlight = index;
                    game.Post(Present);
                }
            }
        }

        /// <summary>
        /// マップ上にカーソルがあるとき、そのマスのユニットまたは地形のステータスを表示する（WinForms 版の MouseMove と同じ）。
        /// </summary>
        private void HoverMap(int x, int y)
        {
            if (IsGUILocked || State.List != null || State.Menu != null || State.Dialog != null || !ScreenLayout.IsInMap(x, y))
            {
                return;
            }
            if (Map == null || Map.MapWidth < 15 || Map.MapHeight < 15)
            {
                return;
            }
            var cell = (x: PixelToMapX(x), y: PixelToMapY(y));
            if (cell == lastHoverCell)
            {
                return;
            }
            lastHoverCell = cell;
            game.Post(() =>
            {
                if (IsGUILocked || !Map.IsInside(cell.x, cell.y))
                {
                    return;
                }
                if (Map.UnitAtPoint(cell.x, cell.y) == null)
                {
                    if (!Map.IsStatusView)
                    {
                        DisplayGlobalStatus();
                    }
                }
                else
                {
                    InstantUnitStatusDisplay(cell.x, cell.y);
                }
            });
        }

        /// <summary>
        /// キー入力。Enter / Space は左クリック、Escape は右クリックとして扱う。
        /// </summary>
        public void KeyPressed(ScreenKey key)
        {
            switch (key)
            {
                case ScreenKey.Enter:
                    if (State.Dialog != null)
                    {
                        dialogResult = 0;
                    }
                    else if (State.List != null && waitingList && State.List.Highlight >= 0)
                    {
                        listResult = State.List.Highlight + 1;
                    }
                    else
                    {
                        SimpleClick();
                    }
                    break;

                case ScreenKey.Escape:
                    if (State.List != null && waitingList)
                    {
                        listResult = 0;
                    }
                    else
                    {
                        SimpleClick();
                    }
                    break;

                case ScreenKey.Left:
                case ScreenKey.Right:
                case ScreenKey.Up:
                case ScreenKey.Down:
                    if (State.List != null && (key == ScreenKey.Up || key == ScreenKey.Down))
                    {
                        var list = State.List;
                        var next = list.Highlight + (key == ScreenKey.Down ? 1 : -1);
                        list.Highlight = Math.Max(0, Math.Min(list.Items.Count - 1, next));
                        game.Post(Present);
                    }
                    else if (!IsGUILocked)
                    {
                        var dx = key == ScreenKey.Left ? -1 : key == ScreenKey.Right ? 1 : 0;
                        var dy = key == ScreenKey.Up ? -1 : key == ScreenKey.Down ? 1 : 0;
                        game.Post(() =>
                        {
                            Center(MapX + dx, MapY + dy);
                            Present();
                        });
                    }
                    break;
            }
        }

        /// <summary>選択肢の選択待ちか</summary>
        public bool IsWaitingList => waitingList;

        /// <summary>メッセージ以外のクリック待ち（Wait Click など）か</summary>
        public bool IsWaitingClick => Commands.WaitClickMode;

        private void SimpleClick()
        {
            if (MessageFormVisible || Commands.WaitClickMode || IsGUILocked)
            {
                isFormClicked = true;
            }
        }

        #endregion

        #region IGUI: 状態

        public bool Terminate()
        {
            return host.Terminate();
        }

        public int MainWidth { get; set; }
        public int MainHeight { get; set; }
        public int MainPWidth { get => MainWidth * ScreenLayout.CellPx; set { } }
        public int MainPHeight { get => MainHeight * ScreenLayout.CellPx; set { } }
        public int MapPWidth { get => (Map?.MapWidth ?? 0) * ScreenLayout.CellPx; set { } }
        public int MapPHeight { get => (Map?.MapHeight ?? 0) * ScreenLayout.CellPx; set { } }
        public bool ScreenIsMasked { get; set; }
        public bool ScreenIsSaved { get; set; }
        public int MapX { get; set; } = 1;
        public int MapY { get; set; } = 1;
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
        public Color BGColor { get; set; }
        public bool IsGUILocked { get; set; }
        public bool IsInDeploymentSelection => false;
        public int TopItem { get; set; }
        public bool MessageWindowIsOut { get; set; }
        public bool IsFormClicked { get => isFormClicked; set => isFormClicked = value; }
        public bool IsMordal { get; set; }
        public int MessageWait { get; set; }
        public bool AutoMessageMode { get; set; }
        public bool HCentering { get; set; }
        public bool VCentering { get; set; }
        public bool PermanentStringMode { get; set; }
        public bool KeepStringMode { get; set; }

        public bool GetKeyState(int key)
        {
            return false;
        }

        public bool IsRButtonPressed(bool ignore_message_wait = false)
        {
            if (!ignore_message_wait && MessageWait == 0)
            {
                return true;
            }
            return isRightButtonDown;
        }

        public void LockGUI()
        {
            IsGUILocked = true;
        }

        public void UnlockGUI()
        {
            IsGUILocked = false;
        }

        public void ChangeStatus(GuiStatus status)
        {
        }

        #endregion

        #region IGUI: フォーム

        public void LoadMainFormAndRegisterFlash()
        {
        }

        public void LoadForms()
        {
            LockGUI();
        }

        public void SetNewGUIMode()
        {
        }

        public bool MainFormVisible { get; private set; }

        private string mainFormText = "";
        public string MainFormText
        {
            get => mainFormText;
            set
            {
                mainFormText = value ?? "";
                host.SetTitle(mainFormText);
            }
        }

        public void SetTitle(string title)
        {
            MainFormText = title;
        }

        public void MainFormShow()
        {
            MainFormVisible = true;
            Present();
        }

        public void MainFormHide()
        {
            MainFormVisible = false;
        }

        public void OpenTitleForm()
        {
            State.Splash = "SRC#";
            Present();
        }

        public void CloseTitleForm()
        {
            State.Splash = null;
            Present();
        }

        public void OpenNowLoadingForm()
        {
            State.Splash = "Now Loading...";
            Present();
        }

        public void CloseNowLoadingForm()
        {
            State.Splash = null;
            Present();
        }

        public void DisplayLoadingProgress()
        {
        }

        public void SetLoadImageSize(int new_size)
        {
        }

        public void ChangeDisplaySize(int w, int h)
        {
        }

        public void Configure()
        {
            // TODO 設定画面
        }

        public void UpdateHotPoint()
        {
        }

        #endregion

        #region IGUI: ダイアログ

        private int ShowDialog(string title, string text, params string[] buttons)
        {
            dialogResult = -1;
            State.Dialog = new DialogState
            {
                Title = title,
                Text = text,
                Buttons = buttons,
            };
            Present();
            WaitUntil(() => dialogResult >= 0);
            var result = dialogResult;
            State.Dialog = null;
            Present();
            return result;
        }

        public void ErrorMessage(string msg)
        {
            ShowDialog("エラー", msg, "OK");
        }

        public void DataErrorMessage(string msg, string fname, int line_num, string line_buf, string dname)
        {
            var text = fname + "：" + line_num + "行目" + Environment.NewLine;
            if (!string.IsNullOrEmpty(dname))
            {
                text += dname + "のデータが不正です。" + Environment.NewLine;
            }
            text += line_buf + Environment.NewLine + msg;
            ErrorMessage(text);
        }

        public GuiDialogResult Confirm(string message, string title, GuiConfirmOption option)
        {
            if (option.HasFlag(GuiConfirmOption.OkCancel))
            {
                return ShowDialog(title, message, "OK", "キャンセル") == 0 ? GuiDialogResult.Ok : GuiDialogResult.Cancel;
            }
            ShowDialog(title, message, "OK");
            return GuiDialogResult.Ok;
        }

        public GuiDialogResult Input(string message, string title, string defaultValue, out string value)
        {
            // TODO 文字入力。現状は既定値で確定する
            value = defaultValue;
            return ShowDialog(title, message + Environment.NewLine + "（入力値: " + defaultValue + "）", "OK", "キャンセル") == 0
                ? GuiDialogResult.Ok
                : GuiDialogResult.Cancel;
        }

        public void DisplayTelop(string msg)
        {
            State.Telop = msg;
            Present();
            Sleep(1000);
            State.Telop = null;
            Present();
        }

        #endregion

        #region IGUI: ファイル

        private string SavePath(string name)
        {
            return Path.Combine(string.IsNullOrEmpty(src.ScenarioPath) ? src.AppPath : src.ScenarioPath, name);
        }

        public Stream SelectSaveStream(SRCSaveKind saveKind, string defaultName = null)
        {
            // TODO 保存先の選択。現状はシナリオフォルダへ保存する
            var name = defaultName ?? (saveKind == SRCSaveKind.Suspend ? "_suspend.srcq" : "_save.srcs");
            return new FileStream(SavePath(name), FileMode.Create);
        }

        public Stream OpenQuikSaveStream(FileAccess fileAccess)
        {
            var path = SavePath("_quik.srcq");
            return fileAccess == FileAccess.Read
                ? new FileStream(path, FileMode.Open, FileAccess.Read)
                : new FileStream(path, FileMode.Create, fileAccess);
        }

        public string SelectLoadFile(string title, string initialDirectory, string fileType, string fileExtension)
        {
            // TODO ファイル選択
            return "";
        }

        public string SelectSaveFile(string title, string initialDirectory, string initialFile, string fileType, string fileExtension)
        {
            // TODO ファイル選択
            return "";
        }

        #endregion
    }

    public enum ScreenKey
    {
        Enter,
        Escape,
        Left,
        Right,
        Up,
        Down,
    }
}
