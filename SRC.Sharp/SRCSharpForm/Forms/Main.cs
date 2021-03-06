// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SRCCore;
using SRCCore.Commands;
using SRCCore.Maps;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SRCSharpForm
{
    // メインウィンドウのフォーム
    internal partial class frmMain : Form
    {
        public SRCCore.SRC SRC { get; set; }
        public IGUI GUI => SRC.GUI;
        public Map Map => SRC.Map;
        public SRCCore.Commands.Command Commands => SRC.Commands;
        public IGUIStatus Status => SRC.GUIStatus;
        public SRCCore.Expressions.Expression Expression => SRC.Expression;

        // マップウィンドウがドラッグされているか？
        private bool IsDragging;
        private double LastMouseX;
        private double LastMouseY;
        private int LastMapX;
        private int LastMapY;
        private PointF lastDraggPoint;
        private const float draggMoveThresholdRate = 0.5f;

        // ポップアップメニュー選択が右クリックだったか？
        private bool IsRightClick;

        private string LastHostSpot = "";

        // フォーム上でキーを押す
        private void frmMain_KeyDown(object eventSender, KeyEventArgs eventArgs)
        {
            Program.Log.LogDebug("frmMain_KeyDown {0}", JsonConvert.SerializeObject(eventArgs));
            var KeyCode = eventArgs.KeyCode;
            bool Shift = eventArgs.KeyData.HasFlag(Keys.Shift);
            // ＧＵＩをロック中？
            if (GUI.IsGUILocked)
            {
                //// リストボックス表示中はキャンセル動作とみなす
                //if (My.MyProject.Forms.frmListBox.Visible)
                //{
                //    Commands.SelectedItem = 0;
                //    GUI.TopItem = (My.MyProject.Forms.frmListBox.lstItems.TopIndex + 1);
                //    if (GUI.IsFormClicked)
                //    {
                //        My.MyProject.Forms.frmListBox.Hide();
                //    }

                //    GUI.IsFormClicked = true;
                //}

                // メッセージ表示中はメッセージ送りとみなす
                if (GUI.MessageFormVisible)
                {
                    GUI.IsFormClicked = true;
                }

                // クリック待ちであれば待ちを解除
                if (Commands.WaitClickMode)
                {
                    GUI.IsFormClicked = true;
                }

                return;
            }

            if (!Shift)
            {
                Program.Log.LogDebug("Focus: {0}", ActiveControl?.Name);
                // 方向キーを押した場合はマップを動かす
                switch (KeyCode)
                {
                    case Keys.Left:
                        if (GUI.MapX > 1)
                        {
                            GUI.MapX = (GUI.MapX - 1);
                            GUI.RefreshScreen();
                        }
                        break;

                    case Keys.Up:
                        if (GUI.MapY > 1)
                        {
                            GUI.MapY = (GUI.MapY - 1);
                            GUI.RefreshScreen();
                        }
                        break;

                    case Keys.Right:
                        if (GUI.MapX < HScrollBar.Maximum - HScrollBar.LargeChange + 1)
                        {
                            GUI.MapX = (GUI.MapX + 1);
                            GUI.RefreshScreen();
                        }
                        break;

                    case Keys.Down:
                        if (GUI.MapY < VScrollBar.Maximum - VScrollBar.LargeChange + 1)
                        {
                            GUI.MapY = (GUI.MapY + 1);
                            GUI.RefreshScreen();
                        }
                        break;

                    case Keys.Escape:
                    case Keys.Delete:
                    case Keys.Back:
                        // XXX 元はマウスダウンでキャンセル相当の動作をさせていたかも
                        SimpleClick();
                        break;

                    default:
                        SimpleClick();
                        // XXX 元はマウスダウンでキャンセル相当の動作をさせていたかも
                        break;
                }
            }
        }

        // フォーム上でマウスを動かす
        private void frmMain_MouseMove(object eventSender, MouseEventArgs eventArgs)
        {
            // ツールチップを消す
            HideToolTip();
        }

        private void mnuUnitCommand_MouseClick(object sender, MouseEventArgs e)
        {
            //Program.Log.LogDebug("mnuUnitCommand_MouseClick {0}", JsonConvert.SerializeObject(e));
            IsRightClick = ResolveMouseButton(e) == GuiButton.Right;
        }

        private void mnuMapCommand_MouseClick(object sender, MouseEventArgs e)
        {
            //Program.Log.LogDebug("mnuMapCommand_MouseClick {0}", JsonConvert.SerializeObject(e));
            IsRightClick = ResolveMouseButton(e) == GuiButton.Right;
        }

        // マップコマンドメニューをクリック
        public void mnuMapCommandItem_Click(object eventSender, EventArgs eventArgs)
        {
            Program.Log.LogDebug("mnuMapCommandItem_Click {0}", JsonConvert.SerializeObject(eventArgs));

            var uiCommand = (eventSender as ToolStripItem)?.Tag as UiCommand;
            if (uiCommand != null)
            {
                Program.Log.LogDebug("{0} {1}", Commands.CommandState, JsonConvert.SerializeObject(uiCommand));

                if (IsRightClick)
                {
                    // 右ボタンでキャンセル
                    Commands.CancelCommand();
                    IsRightClick = false;
                    return;
                }

                //// マップコマンドを実行
                Commands.MapCommand(uiCommand);
            }
        }

        // ユニットコマンドメニューをクリック
        public void mnuUnitCommandItem_Click(object eventSender, EventArgs eventArgs)
        {
            Program.Log.LogDebug("mnuUnitCommandItem_Click {0}", JsonConvert.SerializeObject(eventArgs));

            var uiCommand = (eventSender as ToolStripItem)?.Tag as UiCommand;
            if (uiCommand != null)
            {
                Program.Log.LogDebug("{0} {1}", Commands.CommandState, JsonConvert.SerializeObject(uiCommand));

                if (IsRightClick)
                {
                    // 右ボタンでキャンセル
                    Commands.CancelCommand();
                    IsRightClick = false;
                    return;
                }

                // ユニットコマンドを実行
                Commands.UnitCommand(uiCommand);
            }
        }

        public void ShowUnitCommandMenu(IList<UiCommand> commands)
        {
            //    GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY + 5f);
            //    GUI.MainForm.PopupMenu(GUI.MainForm.mnuUnitCommand, 6, GUI.MouseX, GUI.MouseY - 6f);
            mnuUnitCommand.Items.Clear();
            mnuUnitCommand.Items.AddRange(commands.Select(x => new ToolStripMenuItem(
                x.Label,
                null,
                mnuUnitCommandItem_Click
                )
            {
                Width = 172,
                Tag = x,
            }).ToArray());
            mnuUnitCommand.Show(picMain, new Point((int)GUI.MouseX, (int)GUI.MouseY));
        }

        public void ShowMapCommandMenu(IList<UiCommand> commands)
        {
            //GUI.MainForm.PopupMenu(GUI.MainForm.mnuMapCommand, 6, GUI.MouseX, GUI.MouseY + 6f);
            mnuMapCommand.Items.Clear();
            mnuMapCommand.Items.AddRange(commands.Select(x => new ToolStripMenuItem(
                x.Label,
                null,
                mnuMapCommandItem_Click
                )
            {
                Checked = x.IsChecked,
                Width = 172,
                Tag = x,
            }).ToArray());
            mnuMapCommand.Show(picMain, new Point((int)GUI.MouseX, (int)GUI.MouseY));
        }

        // ステータスウィンドウのパイロット画像上をクリック
        private void picFace_Click(object eventSender, EventArgs eventArgs)
        {
            Program.Log.LogDebug("picFace_Click {0}", JsonConvert.SerializeObject(eventArgs));

            int n;

            // ＧＵＩのロック中は無視
            if (GUI.IsGUILocked)
            {
                return;
            }

            //// ステータスウィンドウで表示しているパイロットを変更
            //if (Status.DisplayedUnit is null)
            //{
            //    return;
            //}

            //{
            //    var withBlock = Status.DisplayedUnit;
            //    if (withBlock.CountPilot() == 0)
            //    {
            //        return;
            //    }

            //    Status.DisplayedPilotInd = (Status.DisplayedPilotInd + 1);
            //    n = (withBlock.CountPilot() + withBlock.CountSupport());
            //    if (withBlock.IsFeatureAvailable(ref "追加サポート"))
            //    {
            //        n = (n + 1);
            //    }

            //    if (Status.DisplayedPilotInd > n)
            //    {
            //        Status.DisplayedPilotInd = 1;
            //    }

            //    Status.DisplayUnitStatus(ref Status.DisplayedUnit, Status.DisplayedPilotInd);
            //}
        }

        private void SimpleClick()
        {
            if (GUI.MessageFormVisible)
            {
                GUI.IsFormClicked = true;
            }

            if (Commands.WaitClickMode)
            {
                GUI.IsFormClicked = true;
            }
        }

        private static GuiButton ResolveMouseButton(MouseEventArgs eventArgs)
        {
            // 右でなければ左とみなす
            return eventArgs.Button.HasFlag(MouseButtons.Right)
                ? GuiButton.Right
                : GuiButton.Left;
        }

        // マップ画面上でマウスをクリック
        private void picMain_MouseClick(object sender, MouseEventArgs eventArgs)
        {
            Program.Log.LogDebug("picMain_Click {0}", JsonConvert.SerializeObject(eventArgs));

            GuiButton button = ResolveMouseButton(eventArgs);
            var X = eventArgs.X;
            var Y = eventArgs.Y;

            // ＧＵＩロック中は単なるクリックとして処理
            if (GUI.IsGUILocked)
            {
                SimpleClick();
                return;
            }

            var xx = GUI.PixelToMapX(X);
            var yy = GUI.PixelToMapY(Y);
            var mapCell = Map.CellAtPoint(xx, yy);
            var cellUnit = Map.UnitAtPoint(xx, yy);
            Program.Log.LogDebug("xx:{0} yy:{1} Unit:{2}", xx, yy, cellUnit?.ID);

            Commands.ProceedInput(button, mapCell, cellUnit);
        }

        // マップ画面上でダブルクリック
        private void picMain_DoubleClick(object eventSender, EventArgs eventArgs)
        {
            Program.Log.LogDebug("picMain_DoubleClick {0}", JsonConvert.SerializeObject(eventArgs));

            if (GUI.IsGUILocked)
            {
                // ＧＵＩクロック中は単なるクリックとみなす
                SimpleClick();

                return;
            }
            else
            {
                // キャンセルの場合はキャンセルを連続実行
                if (GUI.MouseButton == GuiButton.Right)
                {
                    // 右クリック
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
            }
        }

        private void picMain_MouseDown(object eventSender, MouseEventArgs eventArgs)
        {
            Program.Log.LogDebug("picMain_MouseDown {0}", JsonConvert.SerializeObject(eventArgs));

            GuiButton Button = ResolveMouseButton(eventArgs);
            var X = eventArgs.X;
            var Y = eventArgs.Y;

            // 押されたマウスボタンの種類＆カーソルの座標を記録
            GUI.MouseButton = Button;
            GUI.MouseX = X;
            GUI.MouseY = Y;

            if (GUI.IsGUILocked)
            {
                return;
            }

            if (Button == GuiButton.Left)
            {
                GUI.PrevMapX = GUI.MapX;
                GUI.PrevMapY = GUI.MapY;
                GUI.PrevMouseX = X;
                GUI.PrevMouseY = Y;
                IsDragging = true;
                lastDraggPoint.X = X;
                lastDraggPoint.Y = Y;
            }
        }

        // マップ画面上でマウスカーソルを移動
        private void picMain_MouseMove(object eventSender, MouseEventArgs eventArgs)
        {
            //Program.Log.LogDebug("picMain_MouseMove {0}", JsonConvert.SerializeObject(eventArgs));

            var X = eventArgs.X;
            var Y = eventArgs.Y;

            // 前回のマウス位置を記録
            LastMouseX = GUI.MouseX;
            LastMouseY = GUI.MouseY;

            // 現在のマウス位置を記録
            GUI.MouseX = X;
            GUI.MouseY = Y;

            // ＧＵＩロック中？
            if (GUI.IsGUILocked)
            {
                if (!Commands.WaitClickMode)
                {
                    return;
                }
                UpdateHotPointTooltip();
                return;
            }
            HideToolTip();

            // マップが設定されていない場合はこれ以降の判定は不要
            if (Map.MapWidth < 15 || Map.MapHeight < 15)
            {
                return;
            }

            // カーソル上にユニットがいればステータスウィンドウにそのユニットを表示
            var xx = GUI.PixelToMapX(X);
            var yy = GUI.PixelToMapY(Y);
            var cellUnit = Map.UnitAtPoint(xx, yy);

            //// MOD START 240a
            //// If MainWidth = 15 Then
            //if (!GUI.NewGUIMode)
            //{
            //    // MOD  END
            if (Map.IsInside(xx, yy))
            {
                if (cellUnit == null)
                {
                    if (!Map.IsStatusView)
                    {
                        // ユニットがいない、かつステータス表示でなければ地形情報を表示
                        Status.DisplayGlobalStatus();
                    }
                }
                else
                {
                    Status.InstantUnitStatusDisplay(xx, yy);
                }
            }
            //    // MOD  END
            //    // ADD START 240a
            //    else
            //    {
            //        // マップ外にカーソルがある場合
            //        Status.DisplayGlobalStatus();
            //        // ADD  END
            //    }
            //}
            //// ADD ユニット選択追加・移動時も表示 240a
            //// If (CommandState = "ターゲット選択" Or CommandState = "移動後ターゲット選択") _
            //// '            And (SelectedCommand <> "移動" _
            //// '                And SelectedCommand <> "テレポート" _
            //// '                And SelectedCommand <> "ジャンプ") _
            //// '        Then
            //else if (Commands.CommandState == "ターゲット選択" || Commands.CommandState == "移動後ターゲット選択" || Commands.CommandState == "ユニット選択")
            //{
            //    if (1 <= xx && xx <= Map.MapWidth && 1 <= yy && yy <= Map.MapHeight)
            //    {
            //        if (Map.MapDataForUnit[xx, yy] is object)
            //        {
            //            picMain[0].Refresh();
            //            // RedrawScreen
            //            Status.InstantUnitStatusDisplay(xx, yy);
            //        }
            //        // ADD Else
            //        else
            //        {
            //            Status.ClearUnitStatus();
            //        }
            //    }
            //}
            //else if (GUI.MouseX != (float)LastMouseX || GUI.MouseY != (float)LastMouseY)
            //{
            //    Status.ClearUnitStatus();
            //}

            // マップをドラッグ中？
            if (IsDragging && ResolveMouseButton(eventArgs) == GuiButton.Left)
            {
                // 移動量が少なければ無視する
                if (Math.Abs(lastDraggPoint.X - X) + Math.Abs(lastDraggPoint.Y - Y) > MapCellPx * draggMoveThresholdRate)
                {
                    lastDraggPoint.X = X;
                    lastDraggPoint.Y = Y;
                    // Ｘ軸の移動量を算出
                    GUI.MapX = (int)(GUI.PrevMapX - (X - GUI.PrevMouseX) / MapCellPx);
                    if (GUI.MapX < 1)
                    {
                        GUI.MapX = 1;
                    }
                    else if (GUI.MapX > HScrollBar.Maximum - HScrollBar.LargeChange + 1)
                    {
                        GUI.MapX = (HScrollBar.Maximum - HScrollBar.LargeChange + 1);
                    }

                    // Ｙ軸の移動量を算出
                    GUI.MapY = (int)(GUI.PrevMapY - (Y - GUI.PrevMouseY) / 32L);
                    if (GUI.MapY < 1)
                    {
                        GUI.MapY = 1;
                    }
                    else if (GUI.MapY > VScrollBar.Maximum - VScrollBar.LargeChange + 1)
                    {
                        GUI.MapY = (VScrollBar.Maximum - VScrollBar.LargeChange + 1);
                    }

                    if (Map.IsStatusView)
                    {
                        // ステータス画面の場合は移動量を限定
                        GUI.MapX = 8;
                        if (GUI.MapY < 8)
                        {
                            GUI.MapY = 8;
                        }
                        else if (GUI.MapY > Map.MapHeight - 7)
                        {
                            GUI.MapY = (Map.MapHeight - 7);
                        }
                    }

                    // XXX LastXXX保存しないような気がする
                    // マップ画面を新しい座標で更新
                    if (!(GUI.MapX == LastMapX) || !(GUI.MapY == LastMapY))
                    {
                        GUI.RefreshScreen();
                    }
                }
            }
        }

        internal void UpdateHotPointTooltip()
        {
            // ホットポイントが定義されている場合はツールチップを変更
            foreach (var hpoint in SRC.Event.HotPointList)
            {
                if (hpoint.Left <= GUI.MouseX && GUI.MouseX < hpoint.Left + hpoint.Width && hpoint.Top <= GUI.MouseY && GUI.MouseY < hpoint.Top + hpoint.Height)
                {
                    if (hpoint.Caption == "非表示" || string.IsNullOrEmpty(hpoint.Caption))
                    {
                        break;
                    }

                    if (hpoint.Name != LastHostSpot && !string.IsNullOrEmpty(LastHostSpot))
                    {
                        break;
                    }

                    // ツールチップの表示
                    ToolTip1.Show(hpoint.Caption, this, (int)GUI.MouseX, (int)GUI.MouseY);
                    // ここではマウスイベントを処理しつつカーソルの状態を変えるのでグローバルなカーソル状態操作にはしない
                    picMain.Cursor = Cursors.Hand;
                    LastHostSpot = hpoint.Name;
                    return;
                }
            }

            // ホットポイント上にカーソルがなければツールチップを消す
            HideToolTip();
            return;
        }

        private void HideToolTip()
        {
            ToolTip1.Hide(this);
            LastHostSpot = "";
            picMain.Cursor = Cursors.Default;
        }

        // マップ画面上でマウスボタンを離す
        private void picMain_MouseUp(object eventSender, MouseEventArgs eventArgs)
        {
            Program.Log.LogDebug("picMain_MouseUp {0}", JsonConvert.SerializeObject(eventArgs));

            if (GUI.IsGUILocked)
            {
                return;
            }
            // マップ画面のドラッグを解除
            IsDragging = false;
        }

        // マップ画面の横スクロールバーを操作
        private void HScroll_Change(int newScrollValue)
        {
            GUI.MapX = newScrollValue;

            // ステータス表示中はスクロールバーを中央に固定
            if (Map.IsStatusView)
            {
                GUI.MapX = 8;
            }

            // 画面書き換え
            if (Visible)
            {
                GUI.RefreshScreen();
            }
        }

        // マップウィンドウの縦スクロールを操作
        private void VScroll_Change(int newScrollValue)
        {
            GUI.MapY = newScrollValue;
            if (Map.IsStatusView)
            {
                // ステータス画面の場合は移動量を制限
                if (GUI.MapY < 8)
                {
                    GUI.MapY = 8;
                }
                else if (GUI.MapY > Map.MapHeight - 7)
                {
                    GUI.MapY = (Map.MapHeight - 7);
                }
            }

            // マップ画面を更新
            if (Visible)
            {
                GUI.RefreshScreen();
            }
        }

        private void HScrollBar_ValueChanged(object sender, EventArgs e)
        {
            Program.Log.LogDebug("HScrollBar_ValueChanged {0}", JsonConvert.SerializeObject(e));
            HScroll_Change(HScrollBar.Value);
        }

        private void VScrollBar_ValueChanged(object sender, EventArgs e)
        {
            Program.Log.LogDebug("VScrollBar_ValueChanged {0}", JsonConvert.SerializeObject(e));
            VScroll_Change(VScrollBar.Value);
        }

        // フォームを閉じる
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 他所でキャンセルされていたら処理しない
            if (e.Cancel) { return; }
            var IsErrorMessageVisible = default(bool);

            //// エラーメッセージのダイアログは一番上に重ねられるため消去する必要がある
            //if (My.MyProject.Forms.m_frmErrorMessage is object)
            //{
            //    IsErrorMessageVisible = My.MyProject.Forms.frmErrorMessage.Visible;
            //}

            //if (IsErrorMessageVisible)
            //{
            //    My.MyProject.Forms.frmErrorMessage.Hide();
            //}

            // SRCを終了するか確認
            var ret = MsgBox.Show(this, "SRCを終了しますか？", "終了", MsgBoxButtons.OKCancel, MsgBoxIcon.Question);
            switch (ret)
            {
                case DialogResult.OK:
                    // SRCを終了
                    Hide();
                    SRC.TerminateSRC();
                    break;

                default:
                    // 終了をキャンセル
                    e.Cancel = true;
                    break;
            }

            //// エラーメッセージを表示
            //if (IsErrorMessageVisible)
            //{
            //    My.MyProject.Forms.frmErrorMessage.Show();
            //}
        }
    }
}
