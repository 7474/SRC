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
            frmListBox.Refreshed = Present;
            MainWidth = ScreenLayout.MapColumns;
            MainHeight = ScreenLayout.MapRows;
            MessageWait = 700;
        }

        /// <summary>画面に表示する内容</summary>
        public ScreenState State { get; } = new ScreenState();

        public ScreenRenderer Renderer => renderer;

        private ImageCache Images => images ?? (images = new ImageCache(src));

        /// <summary>
        /// 読み込んだ画像を捨てる。次に描く時にファイルから読み直す（デバッガのホットリロード用）。ゲームスレッドで呼ぶ。
        /// </summary>
        public void ClearImageCache()
        {
            images = null;
        }
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

        private bool disposed;

        public void Dispose()
        {
            // ゲームスレッドが描画中の可能性があるため、描画と排他にして破棄する
            lock (renderLock)
            {
                disposed = true;
                renderer.Dispose();
                frame.Dispose();
                DisposeMapBuffers();
            }
        }

        #region 描画

        /// <summary>
        /// 画面を描画して表示側へ渡す。
        /// </summary>
        public void Present()
        {
            lock (renderLock)
            {
                if (disposed)
                {
                    return;
                }
                State.MapX = MapX;
                State.MapY = MapY;
                State.MapCursor = KeyCursorRect();
                State.MainFormVisible = MainFormVisible;
                UpdateKeyPointerState();
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
            Snapshot(stream, SKEncodedImageFormat.Png, 100);
        }

        /// <summary>
        /// 現在の画面を format の画像として書き出す（VS Code 拡張の画面への転送など）。
        /// </summary>
        public void Snapshot(Stream stream, SKEncodedImageFormat format, int quality)
        {
            lock (renderLock)
            {
                if (disposed)
                {
                    return;
                }
                using (var image = SKImage.FromBitmap(frame))
                using (var data = image.Encode(format, quality))
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
                // 終了処理中は待ちを抜けてゲームスレッドを終わらせる
                if (game.IsStopping)
                {
                    throw new OperationCanceledException("The game thread is stopping.");
                }
                game.ProcessPending();
                FlushInvalidatedScreen();
            }
        }

        // 元の SRC の picMain(0) は AutoRedraw の PictureBox で、VB のグラフィックメソッド（Print・Line・Circle・PSet）で
        // 描くと再描画が要求され、Refresh しなくても次の DoEvents（Wait コマンドやメッセージ表示の待ちなど）で画面に出る。
        // API（BitBlt）で描く PaintPicture や ClearPicture は再描画を要求しないため、Refresh までは出ない。
        // （ヘルプ「Waitコマンド」「PaintStringコマンド」）
        private bool isMainInvalidated;

        /// <summary>
        /// マップウィンドウへの描画が次の DoEvents で画面へ反映されるようにする（VB のグラフィックメソッドでの描画）。
        /// </summary>
        private void InvalidateMain()
        {
            isMainInvalidated = true;
        }

        private void FlushInvalidatedScreen()
        {
            // 非表示のウィンドウは描かれない。表示したとき（MainFormShow）に反映する
            if (isMainInvalidated && MainFormVisible)
            {
                UpdateScreen();
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
        public void PointerPressed(float x, float y, GuiButton button, int clickCount = 1)
        {
            HideKeyCursorOnPointer();
            if (button == GuiButton.Right)
            {
                isRightButtonDown = true;
            }
            KeyStateChanged(button == GuiButton.Right ? VirtualKeyRightButton : VirtualKeyLeftButton, true);
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

            // 設定変更ダイアログ（モーダル）
            if (State.Config != null)
            {
                game.Post(() => ConfigPointerPressed(x, y, button));
                return;
            }

            // リストボックスの選択待ち（右クリックはどこでもキャンセル）
            var list = State.List;
            if (list != null && waitingList)
            {
                if (button == GuiButton.Right || list.Window.Contains(x, y))
                {
                    game.Post(() => ListBoxPointerPressed(x, y, button));
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

            var menu = State.Menu;
            if (menu != null)
            {
                ChooseMenu(button == GuiButton.Left ? renderer.HitTestMenu(menu, x, y) : -1);
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
                if (button == GuiButton.Left)
                {
                    // ドラッグによるマップのスクロールの開始
                    PrevMapX = MapX;
                    PrevMapY = MapY;
                    PrevMouseX = x;
                    PrevMouseY = y;
                    isDragging = true;
                    lastDragPoint = (x, y);
                }
                game.Post(() =>
                {
                    Commands.ProceedInput(button, Map.CellAtPoint(mx, my), Map.UnitAtPoint(mx, my));
                    // 右ダブルクリックはキャンセルを連続実行
                    if (button == GuiButton.Right && clickCount >= 2 && !IsGUILocked)
                    {
                        switch (Commands.CommandState ?? "")
                        {
                            case "マップコマンド":
                                Commands.CommandState = "ユニット選択";
                                break;

                            case "ユニット選択":
                                Commands.ProceedCommand(true);
                                break;

                            default:
                                Commands.CancelCommand();
                                break;
                        }
                    }
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
            KeyStateChanged(button == GuiButton.Right ? VirtualKeyRightButton : VirtualKeyLeftButton, false);
            if (button == GuiButton.Right)
            {
                isRightButtonDown = false;
            }
            else
            {
                // マップ画面のドラッグを解除
                isDragging = false;
            }
        }

        /// <summary>
        /// クリックにせず、マップのドラッグによるスクロールだけを始める（タッチ操作で指を滑らせたとき）。
        /// マウスでは左ボタンを押すとクリックとドラッグの開始の両方になるが、タッチではタップを指を離した時のクリックにするため分ける。
        /// </summary>
        public void BeginMapDrag(float x, float y)
        {
            if (IsGUILocked || !ScreenLayout.IsInMap((int)x, (int)y))
            {
                return;
            }
            PrevMapX = MapX;
            PrevMapY = MapY;
            PrevMouseX = x;
            PrevMouseY = y;
            isDragging = true;
            lastDragPoint = (x, y);
        }

        /// <summary>マップのドラッグを終える（<see cref="BeginMapDrag"/> の対）。</summary>
        public void EndMapDrag()
        {
            isDragging = false;
        }

        // マップウィンドウがドラッグされているか？
        private volatile bool isDragging;
        private (float x, float y) lastDragPoint;
        private const float DragMoveThresholdRate = 0.5f;

        /// <summary>
        /// マップのドラッグ（WinForms 版 picMain_MouseMove と同じ移動量）。
        /// </summary>
        private void DragMap(float x, float y)
        {
            if (!isDragging || IsGUILocked || Map == null || Map.MapWidth < 15 || Map.MapHeight < 15)
            {
                return;
            }
            // 移動量が少なければ無視する
            if (Math.Abs(lastDragPoint.x - x) + Math.Abs(lastDragPoint.y - y) <= ScreenLayout.CellPx * DragMoveThresholdRate)
            {
                return;
            }
            lastDragPoint = (x, y);
            var newX = Math.Max(1, Math.Min(Map.MapWidth, (int)(PrevMapX - (x - PrevMouseX) / ScreenLayout.CellPx)));
            var newY = Math.Max(1, Math.Min(Map.MapHeight, (int)(PrevMapY - (y - PrevMouseY) / ScreenLayout.CellPx)));
            if (Map.IsStatusView)
            {
                // ステータス画面の場合は移動量を限定
                newX = 8;
                newY = Math.Max(8, Math.Min(Map.MapHeight - 7, newY));
            }
            if (newX == MapX && newY == MapY)
            {
                return;
            }
            game.Post(() =>
            {
                MapX = newX;
                MapY = newY;
                RefreshScreen();
            });
        }

        private (int x, int y) lastHoverCell;

        public void PointerMoved(float x, float y)
        {
            HideKeyCursorOnPointer();
            MouseX = x;
            MouseY = y;
            UpdateTooltip();
            HoverMap((int)x, (int)y);
            DragMap(x, y);
            var list = State.List;
            if (list != null)
            {
                if (list.Window.Contains(x, y))
                {
                    game.Post(() => ListBoxPointerMoved(x, y));
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
            if (State.Config != null)
            {
                game.Post(() => ConfigKeyPressed(key));
                return;
            }
            // キー操作（物理キー・SRC#Web の仮想パッド）だけで遊べるようにする。
            // Enter: 決定（ダイアログ・リスト・メニューの選択、マップカーソルのマスのクリック、メッセージ送り）
            // Esc: 取り消し（リスト・メニュー、ダイアログは最後のボタン、マップでは右クリック）
            // 矢印: ダイアログのボタン（←→）、リスト・メニューの項目（↑↓）、マップカーソルの移動
            var dialog = State.Dialog;
            var menu = State.Menu;
            var listWaiting = State.List != null && waitingList;
            keyOperating = true;
            switch (key)
            {
                case ScreenKey.Enter:
                    if (dialog != null)
                    {
                        dialogResult = Math.Max(0, Math.Min(dialog.Buttons.Count - 1, dialog.Highlight));
                    }
                    else if (listWaiting)
                    {
                        game.Post(() => ListBoxKeyPressed(key));
                    }
                    else if (menu != null)
                    {
                        if (menu.Highlight < 0)
                        {
                            menu.Highlight = 0;
                            game.Post(Present);
                        }
                        else
                        {
                            ChooseMenu(menu.Highlight);
                        }
                    }
                    else if (IsMapOperable && keyCursor != null)
                    {
                        ClickKeyCursor(GuiButton.Left);
                    }
                    else if (IsKeyPointerOperable && keyPointer != null)
                    {
                        ClickKeyPointer(GuiButton.Left);
                    }
                    else
                    {
                        SimpleClick();
                    }
                    break;

                case ScreenKey.Escape:
                    if (listWaiting)
                    {
                        game.Post(() => ListBoxKeyPressed(key));
                    }
                    else if (dialog != null)
                    {
                        dialogResult = dialog.Buttons.Count - 1;
                    }
                    else if (menu != null)
                    {
                        ChooseMenu(-1);
                    }
                    else if (IsMapOperable)
                    {
                        ClickKeyCursor(GuiButton.Right);
                    }
                    else if (IsKeyPointerOperable && keyPointer != null)
                    {
                        ClickKeyPointer(GuiButton.Right);
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
                    var dx = key == ScreenKey.Left ? -1 : key == ScreenKey.Right ? 1 : 0;
                    var dy = key == ScreenKey.Up ? -1 : key == ScreenKey.Down ? 1 : 0;
                    if (dialog != null)
                    {
                        if (dx != 0 && dialog.Buttons.Count > 1)
                        {
                            dialog.Highlight = Math.Max(0, Math.Min(dialog.Buttons.Count - 1, dialog.Highlight + dx));
                            game.Post(Present);
                        }
                    }
                    else if (State.List != null)
                    {
                        if (dy != 0)
                        {
                            game.Post(() => ListBoxKeyPressed(key));
                        }
                    }
                    else if (menu != null)
                    {
                        var count = menu.Items.Count;
                        if (dy != 0 && count > 0)
                        {
                            menu.Highlight = menu.Highlight < 0
                                ? (dy > 0 ? 0 : count - 1)
                                : (menu.Highlight + dy + count) % count;
                            game.Post(Present);
                        }
                    }
                    else if (IsKeyPointerOperable)
                    {
                        MoveKeyPointer(dx, dy);
                    }
                    else if (!IsGUILocked)
                    {
                        MoveKeyCursor(dx, dy);
                    }
                    break;
            }
        }

        #region キー操作のマップカーソル

        /// <summary>キー操作のマップカーソルのマス（マップの座標）。差し替えて更新する（UI スレッドとゲームスレッドから読む）</summary>
        private sealed class KeyCursorCell
        {
            public KeyCursorCell(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; }
            public int Y { get; }
        }

        private volatile KeyCursorCell keyCursor;
        // キー操作から作ったクリック（マウス・タッチの操作ではないので、マップカーソルを消さない）
        private bool keyClicking;
        // 直前の操作がキー（物理キー・仮想パッド）か。キーで開いたメニューは先頭の項目を選んだ状態にする
        private volatile bool keyOperating;

        /// <summary>マップでのクリック（ユニットの選択・移動先・攻撃対象など）を受け付けるか</summary>
        private bool IsMapOperable => !IsGUILocked && MainFormVisible && State.ShowMap && Map != null && !MessageFormVisible && !Commands.WaitClickMode;

        /// <summary>マップカーソルの画面上の矩形（表示しないときは null）</summary>
        private SKRectI? KeyCursorRect()
        {
            var cursor = keyCursor;
            if (cursor == null || !State.ShowMap || Map == null || IsGUILocked)
            {
                return null;
            }
            var px = MapToPixelX(cursor.X);
            var py = MapToPixelY(cursor.Y);
            if (!ScreenLayout.IsInMap(px, py))
            {
                return null;
            }
            return new SKRectI(px, py, px + ScreenLayout.CellPx, py + ScreenLayout.CellPx);
        }

        /// <summary>マップカーソルを動かす。出ていなければ、最後にポインタを置いたマス（無ければ表示の中心）に出す。画面の外に出たらマップをずらす</summary>
        private void MoveKeyCursor(int dx, int dy)
        {
            if (Map == null || !State.ShowMap)
            {
                return;
            }
            var current = keyCursor;
            int x, y;
            if (current == null)
            {
                (x, y) = lastHoverCell.x > 0 && Map.IsInside(lastHoverCell.x, lastHoverCell.y) ? lastHoverCell : (MapX, MapY);
            }
            else
            {
                x = current.X + dx;
                y = current.Y + dy;
            }
            x = Math.Max(1, Math.Min(Map.MapWidth, x));
            y = Math.Max(1, Math.Min(Map.MapHeight, y));
            keyCursor = new KeyCursorCell(x, y);
            game.Post(() =>
            {
                var left = MapX - ((MainWidth + 1) / 2 - 1);
                var top = MapY - ((MainHeight + 1) / 2 - 1);
                var newX = MapX + Math.Min(0, x - left) + Math.Max(0, x - (left + MainWidth - 1));
                var newY = MapY + Math.Min(0, y - top) + Math.Max(0, y - (top + MainHeight - 1));
                if (newX != MapX || newY != MapY)
                {
                    Center(newX, newY);
                    RefreshScreen();
                }
                // カーソルのマスのユニット・地形をステータスに出す（マウスを置いたときと同じ）
                lastHoverCell = (0, 0);
                HoverMap(MapToPixelX(x) + ScreenLayout.CellPx / 2, MapToPixelY(y) + ScreenLayout.CellPx / 2);
                Present();
            });
        }

        /// <summary>マップカーソルのマス（出ていなければ表示の中心）をクリックする</summary>
        private void ClickKeyCursor(GuiButton button)
        {
            var cursor = keyCursor;
            var px = (cursor != null ? MapToPixelX(cursor.X) : ScreenLayout.MapWidthPx / 2 - ScreenLayout.CellPx / 2) + ScreenLayout.CellPx / 2;
            var py = (cursor != null ? MapToPixelY(cursor.Y) : ScreenLayout.MapHeightPx / 2 - ScreenLayout.CellPx / 2) + ScreenLayout.CellPx / 2;
            keyClicking = true;
            try
            {
                PointerMoved(px, py);
                PointerPressed(px, py, button);
                PointerReleased(px, py, button);
            }
            finally
            {
                keyClicking = false;
            }
        }

        /// <summary>マウス・タッチを使ったら、マップカーソルを消す</summary>
        private void HideKeyCursorOnPointer()
        {
            if (keyClicking)
            {
                return;
            }
            keyOperating = false;
            if (keyPointer != null)
            {
                keyPointer = null;
                game.Post(Present);
            }
            if (keyCursor != null)
            {
                keyCursor = null;
                game.Post(Present);
            }
        }

        #endregion

        #region キー操作のポインタ（クリック待ちの間）

        // クリック待ち（Wait Click）の間、十字キーで画面上の位置を指す。押した方向にホットポイントがあれば最も近いものへ跳び、
        // 無ければ KeyPointerStepPx ずつ動く。Enter で左クリック、Esc で右クリックをその位置で行う
        private const int KeyPointerStepPx = 16;

        private sealed class KeyPointerPosition
        {
            public KeyPointerPosition(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; }
            public int Y { get; }
        }

        private volatile KeyPointerPosition keyPointer;

        /// <summary>キー操作のポインタを使うか（クリック待ちで、メニュー・リスト・ダイアログを出していない）</summary>
        private bool IsKeyPointerOperable => Commands.WaitClickMode && State.Dialog == null && State.Menu == null && State.List == null;

        /// <summary>ポインタの跳び先（ホットポイントの矩形）</summary>
        private IList<SKRectI> KeyPointerTargets()
        {
            try
            {
                return (SRC.Event?.HotPointList ?? new List<SRCCore.Events.HotPoint>())
                    .ToArray()
                    .Where(x => x.Width > 0 && x.Height > 0)
                    .Select(x => SKRectI.Create(x.Left, x.Top, x.Width, x.Height))
                    .ToList();
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentException)
            {
                // ゲームスレッドが一覧を書き換えている途中
                return new List<SKRectI>();
            }
        }

        private void UpdateKeyPointerState()
        {
            var pointer = keyPointer;
            if (pointer == null || !Commands.WaitClickMode)
            {
                State.KeyPointer = null;
                State.KeyPointerTarget = null;
                return;
            }
            State.KeyPointer = new SKPointI(pointer.X, pointer.Y);
            State.KeyPointerTarget = KeyPointerTargets()
                .Where(x => x.Contains(pointer.X, pointer.Y))
                .Select(x => (SKRectI?)x)
                .FirstOrDefault();
        }

        private void MoveKeyPointer(int dx, int dy)
        {
            var targets = KeyPointerTargets();
            var current = keyPointer;
            int x, y;
            if (current == null)
            {
                // 最初はホットポイントの先頭（無ければマップの中央）を指す
                var first = targets.Select(t => (SKRectI?)t).FirstOrDefault();
                x = first?.MidX ?? ScreenLayout.MapWidthPx / 2;
                y = first?.MidY ?? ScreenLayout.MapHeightPx / 2;
            }
            else
            {
                // 押した方向にあるホットポイントのうち、方向からのずれを重く見て最も近いもの
                var best = targets
                    .Select(t => new
                    {
                        Target = t,
                        Along = (t.MidX - current.X) * dx + (t.MidY - current.Y) * dy,
                        Across = Math.Abs((t.MidX - current.X) * dy - (t.MidY - current.Y) * dx),
                    })
                    .Where(c => c.Along > 4)
                    .OrderBy(c => c.Along + c.Across * 2)
                    .FirstOrDefault();
                if (best != null)
                {
                    x = best.Target.MidX;
                    y = best.Target.MidY;
                }
                else
                {
                    x = Math.Max(0, Math.Min(ScreenLayout.Width - 1, current.X + dx * KeyPointerStepPx));
                    y = Math.Max(0, Math.Min(ScreenLayout.Height - 1, current.Y + dy * KeyPointerStepPx));
                }
            }
            keyPointer = new KeyPointerPosition(x, y);
            // 元の SRC はホットポイントから隣のホットポイントへ直接移るとツールチップを一度消すが、
            // ポインタは跳んで移るため、跳んだ先の説明をすぐに出す
            game.Post(() => lastHotSpot = "");
            keyClicking = true;
            try
            {
                // マウスを動かしたのと同じにする（ホットポイントのツールチップを出す）
                PointerMoved(x, y);
            }
            finally
            {
                keyClicking = false;
            }
            game.Post(Present);
        }

        /// <summary>キー操作のポインタの位置をクリックする</summary>
        private void ClickKeyPointer(GuiButton button)
        {
            var pointer = keyPointer;
            keyClicking = true;
            try
            {
                PointerMoved(pointer.X, pointer.Y);
                PointerPressed(pointer.X, pointer.Y, button);
                PointerReleased(pointer.X, pointer.Y, button);
            }
            finally
            {
                keyClicking = false;
            }
        }

        #endregion

        /// <summary>ポップアップメニューの項目を選ぶ（index が負なら取り消し）</summary>
        private void ChooseMenu(int index)
        {
            var commands = menuCommands;
            var isUnitMenu = isUnitCommandMenu;
            State.Menu = null;
            menuCommands = null;
            game.Post(() =>
            {
                if (index >= 0 && commands != null && index < commands.Count)
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
        }

        /// <summary>
        /// マウスホイール。delta は上方向が正（1 ノッチで 1）。
        /// </summary>
        public void PointerWheel(float x, float y, int delta)
        {
            var list = State.List;
            if (list != null && list.Window.Contains(x, y))
            {
                game.Post(() => ListBoxWheel(delta));
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

        // 押されているキー・マウスボタン（Windows の仮想キーコード）
        private readonly HashSet<int> pressedKeys = new HashSet<int>();

        /// <summary>Windows の仮想キーコード（VK_LBUTTON = 1、VK_RBUTTON = 2）。</summary>
        public const int VirtualKeyLeftButton = 0x01;
        public const int VirtualKeyRightButton = 0x02;

        /// <summary>
        /// キーが押された・離された（KeyState 関数用）。key は Windows の仮想キーコード。
        /// </summary>
        public void KeyStateChanged(int key, bool pressed)
        {
            lock (pressedKeys)
            {
                if (pressed)
                {
                    pressedKeys.Add(key);
                }
                else
                {
                    pressedKeys.Remove(key);
                }
            }
        }

        /// <summary>
        /// ウィンドウが非アクティブになった。押されている状態をすべて解除する。
        /// </summary>
        public void ReleaseAllKeys()
        {
            lock (pressedKeys)
            {
                pressedKeys.Clear();
            }
            isRightButtonDown = false;
            isDragging = false;
        }

        public bool GetKeyState(int key)
        {
            lock (pressedKeys)
            {
                return pressedKeys.Contains(key);
            }
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
            // 元の SRC ではウィンドウを表示すると picMain(0) 全体が再描画され、描画済みの内容がすべて出る
            MainFormVisible = true;
            UpdateScreen();
        }

        public void MainFormHide()
        {
            MainFormVisible = false;
            Present();
        }

        public void OpenTitleForm()
        {
            // 元の SRC と同じく、起動時のタイトルにだけバージョンを出す（docs/versioning.md）
            var v = src.Version;
            State.Title = new TitleState { Version = v == null ? "" : $"{v.Major}.{v.Minor}.{Math.Max(0, v.Build)}" };
            Present();
        }

        public void CloseTitleForm()
        {
            State.Title = null;
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

        public void UpdateHotPoint()
        {
            UpdateTooltip();
        }

        private string lastHotSpot = "";

        /// <summary>
        /// クリック待ちの間、ホットポイント上にカーソルがあればツールチップを表示する。
        /// </summary>
        private void UpdateTooltip()
        {
            if (!IsGUILocked && State.Tooltip == null && string.IsNullOrEmpty(lastHotSpot))
            {
                return;
            }
            var mouseX = MouseX;
            var mouseY = MouseY;
            game.Post(() =>
            {
                string caption = null;
                if (IsGUILocked && Commands.WaitClickMode)
                {
                    // ホットポイントが定義されている場合はツールチップを変更
                    foreach (var hpoint in SRC.Event.HotPointList ?? new List<SRCCore.Events.HotPoint>())
                    {
                        if (hpoint.Left <= mouseX && mouseX < hpoint.Left + hpoint.Width && hpoint.Top <= mouseY && mouseY < hpoint.Top + hpoint.Height)
                        {
                            if (hpoint.Caption == "非表示" || string.IsNullOrEmpty(hpoint.Caption))
                            {
                                break;
                            }
                            if (hpoint.Name != lastHotSpot && !string.IsNullOrEmpty(lastHotSpot))
                            {
                                break;
                            }
                            caption = hpoint.Caption;
                            lastHotSpot = hpoint.Name;
                            break;
                        }
                    }
                }
                if (caption == null)
                {
                    lastHotSpot = "";
                }
                var tooltip = caption == null ? null : new TooltipState { Text = caption, X = (int)mouseX, Y = (int)mouseY };
                if (tooltip?.Text != State.Tooltip?.Text || (tooltip != null && (tooltip.X != State.Tooltip.X || tooltip.Y != State.Tooltip.Y)))
                {
                    State.Tooltip = tooltip;
                    Present();
                }
            });
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

        /// <summary>エラーのダイアログを出すとき（イベントエラーなど）。ホストがエラーを記録するため</summary>
        public event Action<string> ErrorShown;

        public void ErrorMessage(string msg)
        {
            ErrorShown?.Invoke(msg);
            ShowDialog(Resources.Strings.ErrorTitle, msg, "OK");
        }

        public void DataErrorMessage(string msg, string fname, int line_num, string line_buf, string dname)
        {
            var text = string.Format(Resources.Strings.DataErrorLine, fname, line_num) + Environment.NewLine;
            if (!string.IsNullOrEmpty(dname))
            {
                text += string.Format(Resources.Strings.DataErrorInvalidData, dname) + Environment.NewLine;
            }
            text += line_buf + Environment.NewLine + msg;
            ErrorMessage(text);
        }

        public GuiDialogResult Confirm(string message, string title, GuiConfirmOption option)
        {
            if (option.HasFlag(GuiConfirmOption.OkCancel))
            {
                return ShowDialog(title, message, "OK", Resources.Strings.Cancel) == 0 ? GuiDialogResult.Ok : GuiDialogResult.Cancel;
            }
            ShowDialog(title, message, "OK");
            return GuiDialogResult.Ok;
        }

        public GuiDialogResult Input(string message, string title, string defaultValue, out string value)
        {
            if (host is IScreenDialogHost dialogs)
            {
                var input = WaitTask(dialogs.InputTextAsync(title, message, defaultValue));
                value = input ?? defaultValue;
                return input == null ? GuiDialogResult.Cancel : GuiDialogResult.Ok;
            }
            // 文字入力の手段が無い場合は既定値で確定する
            value = defaultValue;
            return ShowDialog(title, message + Environment.NewLine + string.Format(Resources.Strings.InputDefaultValue, defaultValue), "OK", Resources.Strings.Cancel) == 0
                ? GuiDialogResult.Ok
                : GuiDialogResult.Cancel;
        }

        /// <summary>
        /// UI 側の処理（ダイアログ）の完了を、UI からの処理を続けながらゲームスレッドで待つ。
        /// </summary>
        private T WaitTask<T>(System.Threading.Tasks.Task<T> task)
        {
            WaitUntil(() => task.IsCompleted);
            return task.GetAwaiter().GetResult();
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
            var ext = saveKind == SRCSaveKind.Normal ? "srcs" : "srcq";
            if (host is IScreenDialogHost dialogs)
            {
                var path = WaitTask(dialogs.SaveFileAsync(new FileDialogRequest
                {
                    Title = Resources.Strings.SaveDialogTitle,
                    InitialDirectory = src.ScenarioPath,
                    InitialFile = string.IsNullOrEmpty(defaultName)
                        ? Expression.GetValueAsString("セーブデータファイル名")
                        : defaultName,
                    FileType = "save files",
                    FileExtension = ext,
                }));
                return string.IsNullOrEmpty(path) ? null : src.FileSystem.Create(path);
            }
            // 保存先を選ぶ手段が無い場合はシナリオフォルダへ保存する
            var name = defaultName ?? (saveKind == SRCSaveKind.Suspend ? "_suspend.srcq" : "_save.srcs");
            return src.FileSystem.Create(SavePath(name));
        }

        public Stream OpenQuikSaveStream(FileAccess fileAccess)
        {
            var path = SavePath("_quik.srcq");
            return fileAccess == FileAccess.Read
                ? new FileStream(path, FileMode.Open, FileAccess.Read)
                : src.FileSystem.Create(path);
        }

        private string EffectiveDirectory(string initialDirectory)
        {
            return new[] { initialDirectory, src.ScenarioPath }
                .FirstOrDefault(x => !string.IsNullOrEmpty(x) && Directory.Exists(x));
        }

        public string SelectLoadFile(string title, string initialDirectory, string fileType, string fileExtension)
        {
            if (!(host is IScreenDialogHost dialogs))
            {
                return "";
            }
            return WaitTask(dialogs.OpenFileAsync(new FileDialogRequest
            {
                Title = title,
                InitialDirectory = EffectiveDirectory(initialDirectory),
                FileType = fileType,
                FileExtension = fileExtension,
            })) ?? "";
        }

        public string SelectSaveFile(string title, string initialDirectory, string initialFile, string fileType, string fileExtension)
        {
            if (!(host is IScreenDialogHost dialogs))
            {
                return "";
            }
            return WaitTask(dialogs.SaveFileAsync(new FileDialogRequest
            {
                Title = title,
                InitialDirectory = EffectiveDirectory(initialDirectory),
                InitialFile = initialFile ?? "",
                FileType = fileType,
                FileExtension = fileExtension,
            })) ?? "";
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
