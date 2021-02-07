// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using System;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using SRC.Core;

namespace SRCTestForm
{
    // メインウィンドウのフォーム
    internal partial class frmMain : Form
    {
        public SRC.Core.SRC SRC { get; set; }
        public IGUI GUI => SRC.GUI;

        // マップウィンドウがドラッグされているか？
        private bool IsDragging;


        // フォーム上でキーを押す
        private void frmMain_KeyDown(object eventSender, KeyEventArgs eventArgs)
        {
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

                //// クリック待ちであれば待ちを解除
                //if (Commands.WaitClickMode)
                //{
                //    GUI.IsFormClicked = true;
                //}

                return;
            }

            if (!Shift)
            {
                // 方向キーを押した場合はマップを動かす
                switch (KeyCode)
                {
                    case Keys.Left:
                        {
                            if (GUI.MapX > 1)
                            {
                                GUI.MapX = (GUI.MapX - 1);
                                GUI.RefreshScreen();
                            }

                            break;
                        }

                    case Keys.Up:
                        {
                            if (GUI.MapY > 1)
                            {
                                GUI.MapY = (GUI.MapY - 1);
                                GUI.RefreshScreen();
                            }

                            break;
                        }

                        //case Keys.Right:
                        //    {
                        //        if (GUI.MapX < HScroll_Renamed.Maximum - HScroll_Renamed.LargeChange + 1)
                        //        {
                        //            GUI.MapX = (GUI.MapX + 1);
                        //            GUI.RefreshScreen();
                        //        }

                        //        break;
                        //    }

                        //case Keys.Down:
                        //    {
                        //        if (GUI.MapY < VScroll_Renamed.Maximum - VScroll_Renamed.LargeChange + 1)
                        //        {
                        //            GUI.MapY = (GUI.MapY + 1);
                        //            GUI.RefreshScreen();
                        //        }

                        //        break;
                        //    }

                        //case Keys.Escape:
                        //case Keys.Delete:
                        //case Keys.Back:
                        //    {
                        //        picMain_MouseDown(picMain[0], new MouseEventArgs((MouseButtons)0x100000, 0, 0, 0, 0));
                        //        break;
                        //    }

                        //default:
                        //    {
                        //        picMain_MouseDown(picMain[0], new MouseEventArgs((MouseButtons)0x100000, 0, 0, 0, 0));
                        //        break;
                        //    }
                }
            }
        }

        // フォーム上でマウスを動かす
        private void frmMain_MouseMove(object eventSender, MouseEventArgs eventArgs)
        {
            //int Button = (eventArgs.Button / 0x100000);
            //int Shift = (ModifierKeys / 0x10000);
            //float X = eventArgs.X;
            //float Y = eventArgs.Y;
            //// ツールチップを消す
            //My.MyProject.Forms.frmToolTip.Hide();
            //// UPGRADE_ISSUE: 定数 vbCustom はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
            //if (object.ReferenceEquals(picMain[0].Cursor, vbCustom))
            //{
            //    picMain[0].Cursor = Cursors.Default;
            //}
        }

        // フォームを閉じる
        private void frmMain_FormClosed(object eventSender, FormClosedEventArgs eventArgs)
        {
            int ret;
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

            //// SRCの終了を確認
            //ret = Interaction.MsgBox("SRCを終了しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "終了");
            //switch (ret)
            //{
            //    case 1:
            //        {
            //            // SRCを終了
            //            SRC.TerminateSRC();
            //            break;
            //        }

            //    case 2:
            //        {
            //            // 終了をキャンセル
            //            // UPGRADE_ISSUE: Event パラメータ Cancel はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FB723E3C-1C06-4D2B-B083-E6CD0D334DA8"' をクリックしてください。
            //            Cancel = 1;
            //            break;
            //        }
            //}

            //// エラーメッセージを表示
            //if (IsErrorMessageVisible)
            //{
            //    My.MyProject.Forms.frmErrorMessage.Show();
            //}
        }

        // マップ画面の横スクロールバーを操作
        // UPGRADE_NOTE: HScroll.Change はイベントからプロシージャに変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="4E2DC008-5EDA-4547-8317-C9316952674F"' をクリックしてください。
        // UPGRADE_WARNING: HScrollBar イベント HScroll.Change には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
        private void HScroll_Change(int newScrollValue)
        {
            //GUI.MapX = HScroll_Renamed.Value;

            //// ステータス表示中はスクロールバーを中央に固定
            //if (string.IsNullOrEmpty(Map.MapFileName))
            //{
            //    GUI.MapX = 8;
            //}

            //// 画面書き換え
            //if (Visible)
            //{
            //    GUI.RefreshScreen();
            //}
        }

        // マップコマンドメニューをクリック
        public void mnuMapCommandItem_Click(object eventSender, EventArgs eventArgs)
        {
            //int Index = mnuMapCommandItem.GetIndex((ToolStripMenuItem)eventSender);
            //if (GUI.GetAsyncKeyState(GUI.RButtonID) == 1)
            //{
            //    // 右ボタンでキャンセル
            //    Commands.CancelCommand();
            //    return;
            //}

            //// マップコマンドを実行
            //Commands.MapCommand(Index);
        }

        // ユニットコマンドメニューをクリック
        public void mnuUnitCommandItem_Click(object eventSender, EventArgs eventArgs)
        {
            //int Index = mnuUnitCommandItem.GetIndex((ToolStripMenuItem)eventSender);
            //if (GUI.GetAsyncKeyState(GUI.RButtonID) == 1)
            //{
            //    // 右ボタンでキャンセル
            //    Commands.CancelCommand();
            //    return;
            //}

            //// ユニットコマンドを実行
            //Commands.UnitCommand(Index);
        }

        // ステータスウィンドウのパイロット画像上をクリック
        private void picFace_Click(object eventSender, EventArgs eventArgs)
        {
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
            //    string argfname = "追加サポート";
            //    if (withBlock.IsFeatureAvailable(ref argfname))
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

        // マップ画面上でダブルクリック
        private void picMain_DoubleClick(object eventSender, EventArgs eventArgs)
        {
            //int Index = picMain.GetIndex((Panel)eventSender);
            //if (GUI.IsGUILocked)
            //{
            //    // ＧＵＩクロック中は単なるクリックとみなす
            //    if (My.MyProject.Forms.frmMessage.Visible)
            //    {
            //        GUI.IsFormClicked = true;
            //    }

            //    if (Commands.WaitClickMode)
            //    {
            //        GUI.IsFormClicked = true;
            //    }

            //    return;
            //}
            //// キャンセルの場合はキャンセルを連続実行
            //else if (GUI.MouseButton == 2)
            //{
            //    switch (Commands.CommandState ?? "")
            //    {
            //        case "マップコマンド":
            //            {
            //                Commands.CommandState = "ユニット選択";
            //                break;
            //            }

            //        case "ユニット選択":
            //            {
            //                Commands.ProceedCommand(true);
            //                break;
            //            }

            //        default:
            //            {
            //                Commands.CancelCommand();
            //                break;
            //            }
            //    }
            //}
        }

        // マップ画面上でマウスをクリック
        private void picMain_MouseDown(object eventSender, MouseEventArgs eventArgs)
        {
            //int Button = (eventArgs.Button / 0x100000);
            //int Shift = (ModifierKeys / 0x10000);
            //float X = (float)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(eventArgs.X);
            //float Y = (float)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(eventArgs.Y);
            //int Index = picMain.GetIndex((Panel)eventSender);
            //int xx, yy;

            //// 押されたマウスボタンの種類＆カーソルの座標を記録
            //GUI.MouseButton = Button;
            //GUI.MouseX = X;
            //GUI.MouseY = Y;

            //// ＧＵＩロック中は単なるクリックとして処理
            //if (GUI.IsGUILocked)
            //{
            //    if (My.MyProject.Forms.frmMessage.Visible)
            //    {
            //        GUI.IsFormClicked = true;
            //    }

            //    if (Commands.WaitClickMode)
            //    {
            //        GUI.IsFormClicked = true;
            //    }

            //    return;
            //}

            //switch (Button)
            //{
            //    case 1:
            //        {
            //            // 左クリック
            //            GUI.PrevMapX = GUI.MapX;
            //            GUI.PrevMapY = GUI.MapY;
            //            GUI.PrevMouseX = X;
            //            GUI.PrevMouseY = Y;
            //            switch (Commands.CommandState ?? "")
            //            {
            //                case "マップコマンド":
            //                    {
            //                        Commands.CommandState = "ユニット選択";
            //                        break;
            //                    }

            //                case "ユニット選択":
            //                    {
            //                        xx = GUI.PixelToMapX(X);
            //                        yy = GUI.PixelToMapY(Y);
            //                        if (xx < 1 | Map.MapWidth < xx | yy < 1 | Map.MapHeight < yy)
            //                        {
            //                            IsDragging = true;
            //                        }
            //                        else if (Map.MapDataForUnit[xx, yy] is object)
            //                        {
            //                            Commands.ProceedCommand();
            //                        }
            //                        else
            //                        {
            //                            IsDragging = true;
            //                        }

            //                        break;
            //                    }

            //                case "ターゲット選択":
            //                case "移動後ターゲット選択":
            //                    {
            //                        xx = GUI.PixelToMapX(X);
            //                        yy = GUI.PixelToMapY(Y);
            //                        if (xx < 1 | Map.MapWidth < xx | yy < 1 | Map.MapHeight < yy)
            //                        {
            //                            IsDragging = true;
            //                        }
            //                        else if (!Map.MaskData[xx, yy])
            //                        {
            //                            Commands.ProceedCommand();
            //                        }
            //                        else
            //                        {
            //                            IsDragging = true;
            //                        }

            //                        break;
            //                    }
            //                // MOD START MARGE
            //                // Case "コマンド選択", "移動後コマンド選択"
            //                case "コマンド選択":
            //                    {
            //                        // MOD  END  MARGE
            //                        Commands.CancelCommand();
            //                        // ADD START MARGE
            //                        // もし新しいクリック地点がユニットなら、ユニット選択の処理を進める
            //                        xx = GUI.PixelToMapX(X);
            //                        yy = GUI.PixelToMapY(Y);
            //                        if (xx < 1 | Map.MapWidth < xx | yy < 1 | Map.MapHeight < yy)
            //                        {
            //                            IsDragging = true;
            //                        }
            //                        else if (Map.MapDataForUnit[xx, yy] is object)
            //                        {
            //                            Commands.ProceedCommand();
            //                        }
            //                        else
            //                        {
            //                            IsDragging = true;
            //                        }

            //                        break;
            //                    }

            //                case "移動後コマンド選択":
            //                    {
            //                        // ADD  END  MARGE
            //                        Commands.CancelCommand();
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Commands.ProceedCommand();
            //                        break;
            //                    }
            //            }

            //            break;
            //        }

            //    case 2:
            //        {
            //            // 右クリック
            //            switch (Commands.CommandState ?? "")
            //            {
            //                case "マップコマンド":
            //                    {
            //                        Commands.CommandState = "ユニット選択";
            //                        break;
            //                    }

            //                case "ユニット選択":
            //                    {
            //                        Commands.ProceedCommand(true);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        Commands.CancelCommand();
            //                        break;
            //                    }
            //            }

            //            break;
            //        }
            //}
        }

        // マップ画面上でマウスカーソルを移動
        private void picMain_MouseMove(object eventSender, MouseEventArgs eventArgs)
        {
            //int Button = (eventArgs.Button / 0x100000);
            //int Shift = (ModifierKeys / 0x10000);
            //float X = (float)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(eventArgs.X);
            //float Y = (float)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(eventArgs.Y);
            //int Index = picMain.GetIndex((Panel)eventSender);
            //;
            //int xx, yy;
            //int i;

            //// 前回のマウス位置を記録
            //LastMouseX = GUI.MouseX;
            //LastMouseY = GUI.MouseY;

            //// 現在のマウス位置を記録
            //GUI.MouseX = X;
            //GUI.MouseY = Y;

            //// ＧＵＩロック中？
            //if (GUI.IsGUILocked)
            //{
            //    if (!Commands.WaitClickMode)
            //    {
            //        return;
            //    }

            //    // ホットポイントが定義されている場合はツールチップを変更
            //    var loopTo = Information.UBound(Event_Renamed.HotPointList);
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        {
            //            var withBlock = Event_Renamed.HotPointList[i];
            //            if (withBlock.Left_Renamed <= GUI.MouseX & GUI.MouseX < withBlock.Left_Renamed + withBlock.width & withBlock.Top <= GUI.MouseY & GUI.MouseY < withBlock.Top + withBlock.Height)
            //            {
            //                if (withBlock.Caption == "非表示" | string.IsNullOrEmpty(withBlock.Caption))
            //                {
            //                    break;
            //                }

            //                if ((withBlock.Name ?? "") != (LastHostSpot ?? "") & !string.IsNullOrEmpty(LastHostSpot))
            //                {
            //                    break;
            //                }

            //                // ツールチップの表示
            //                My.MyProject.Forms.frmToolTip.ShowToolTip(ref withBlock.Caption);
            //                {
            //                    var withBlock1 = picMain[0];
            //                    // UPGRADE_ISSUE:  プロパティ . はカスタム マウスポインタをサポートしません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="45116EAB-7060-405E-8ABE-9DBB40DC2E86"' をクリックしてください。
            //                    if (!withBlock1.Cursor.Equals(99))
            //                    {
            //                        withBlock1.Refresh();
            //                        // UPGRADE_ISSUE: PictureBox プロパティ picMain.MousePointer はカスタム マウスポインタをサポートしません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="45116EAB-7060-405E-8ABE-9DBB40DC2E86"' をクリックしてください。
            //                        withBlock1.Cursor = vbCustom;
            //                    }
            //                }

            //                LastHostSpot = withBlock.Name;
            //                return;
            //            }
            //        }
            //    }

            //    // ホットポイント上にカーソルがなければツールチップを消す
            //    My.MyProject.Forms.frmToolTip.Hide();
            //    LastHostSpot = "";
            //    picMain[0].Cursor = Cursors.Default;
            //    return;
            //}

            //// マップが設定されていない場合はこれ以降の判定は不要
            //if (Map.MapWidth < 15 | Map.MapHeight < 15)
            //{
            //    return;
            //}

            //// カーソル上にユニットがいればステータスウィンドウにそのユニットを表示
            //xx = GUI.PixelToMapX(X);
            //yy = GUI.PixelToMapY(Y);
            //// MOD START 240a
            //// If MainWidth = 15 Then
            //if (!GUI.NewGUIMode)
            //{
            //    // MOD  END
            //    if (1 <= xx & xx <= Map.MapWidth & 1 <= yy & yy <= Map.MapHeight)
            //    {
            //        // MOD START 240a
            //        // If Not MapDataForUnit(xx, yy) Is Nothing Then
            //        // InstantUnitStatusDisplay xx, yy
            //        // End If
            //        if (Map.MapDataForUnit[xx, yy] is null)
            //        {
            //            if (!string.IsNullOrEmpty(Map.MapFileName))
            //            {
            //                // ユニットがいない、かつステータス表示でなければ地形情報を表示
            //                Status.DisplayGlobalStatus();
            //            }
            //        }
            //        else
            //        {
            //            Status.InstantUnitStatusDisplay(xx, yy);
            //        }
            //    }
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
            //else if (Commands.CommandState == "ターゲット選択" | Commands.CommandState == "移動後ターゲット選択" | Commands.CommandState == "ユニット選択")
            //{
            //    if (1 <= xx & xx <= Map.MapWidth & 1 <= yy & yy <= Map.MapHeight)
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
            //else if (GUI.MouseX != (float)LastMouseX | GUI.MouseY != (float)LastMouseY)
            //{
            //    Status.ClearUnitStatus();
            //}

            //// マップをドラッグ中？
            //if (IsDragging & Button == 1)
            //{
            //    // Ｘ軸の移動量を算出
            //    GUI.MapX = (GUI.PrevMapX - (long)(X - GUI.PrevMouseX) / 32L);
            //    if (GUI.MapX < 1)
            //    {
            //        GUI.MapX = 1;
            //    }
            //    else if (GUI.MapX > HScroll_Renamed.Maximum - HScroll_Renamed.LargeChange + 1)
            //    {
            //        GUI.MapX = (HScroll_Renamed.Maximum - HScroll_Renamed.LargeChange + 1);
            //    }

            //    // Ｙ軸の移動量を算出
            //    GUI.MapY = (GUI.PrevMapY - (long)(Y - GUI.PrevMouseY) / 32L);
            //    if (GUI.MapY < 1)
            //    {
            //        GUI.MapY = 1;
            //    }
            //    else if (GUI.MapY > VScroll_Renamed.Maximum - VScroll_Renamed.LargeChange + 1)
            //    {
            //        GUI.MapY = (VScroll_Renamed.Maximum - VScroll_Renamed.LargeChange + 1);
            //    }

            //    if (string.IsNullOrEmpty(Map.MapFileName))
            //    {
            //        // ステータス画面の場合は移動量を限定
            //        GUI.MapX = 8;
            //        if (GUI.MapY < 8)
            //        {
            //            GUI.MapY = 8;
            //        }
            //        else if (GUI.MapY > Map.MapHeight - 7)
            //        {
            //            GUI.MapY = (Map.MapHeight - 7);
            //        }
            //    }

            //    // マップ画面を新しい座標で更新
            //    if (!(GUI.MapX == LastMapX) | !(GUI.MapY == LastMapY))
            //    {
            //        GUI.RefreshScreen();
            //    }
            //}
        }

        // マップ画面上でマウスボタンを離す
        private void picMain_MouseUp(object eventSender, MouseEventArgs eventArgs)
        {
            if (GUI.IsGUILocked)
            {
                return;
            }
            // マップ画面のドラッグを解除
            IsDragging = false;
        }

        // ＢＧＭ連続再生用タイマー
        private void Timer1_Tick(object eventSender, EventArgs eventArgs)
        {
            //if (!string.IsNullOrEmpty(Sound.BGMFileName))
            //{
            //    if (Sound.RepeatMode)
            //    {
            //        Sound.RestartBGM();
            //    }
            //}
        }

        // マップウィンドウの縦スクロールを操作
        // UPGRADE_NOTE: VScroll.Change はイベントからプロシージャに変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="4E2DC008-5EDA-4547-8317-C9316952674F"' をクリックしてください。
        // UPGRADE_WARNING: VScrollBar イベント VScroll.Change には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
        private void VScroll_Change(int newScrollValue)
        {
            //GUI.MapY = VScroll_Renamed.Value;
            //if (string.IsNullOrEmpty(Map.MapFileName))
            //{
            //    // ステータス画面の場合は移動量を制限
            //    if (GUI.MapY < 8)
            //    {
            //        GUI.MapY = 8;
            //    }
            //    else if (GUI.MapY > Map.MapHeight - 7)
            //    {
            //        GUI.MapY = (Map.MapHeight - 7);
            //    }
            //}

            // マップ画面を更新
            if (Visible)
            {
                GUI.RefreshScreen();
            }
        }

        private void HScroll_Renamed_Scroll(object eventSender, ScrollEventArgs eventArgs)
        {
            switch (eventArgs.Type)
            {
                case ScrollEventType.EndScroll:
                    {
                        HScroll_Change(eventArgs.NewValue);
                        break;
                    }
            }
        }

        private void VScroll_Renamed_Scroll(object eventSender, ScrollEventArgs eventArgs)
        {
            switch (eventArgs.Type)
            {
                case ScrollEventType.EndScroll:
                    {
                        VScroll_Change(eventArgs.NewValue);
                        break;
                    }
            }
        }
    }
}