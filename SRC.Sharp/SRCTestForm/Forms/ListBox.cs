﻿// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using SRCCore;
using SRCCore.Lib;

namespace SRCTestForm
{
    // リストボックスのフォーム
    internal partial class frmListBox : Form
    {
        public SRCCore.SRC SRC { get; set; }
        private IGUI GUI => SRC.GUI;
        private SRCCore.Commands.Command Commands => SRC.Commands;
        private IGUIStatus Status => SRC.GUIStatus;

        // リストボックスのサイズ (通常:M 大型:L)
        // 幅
        public string HorizontalSize;
        // 高さ
        public string VerticalSize;

        // Questionコマンド用変数
        public int CurrentTime;
        public int TimeLimit;

        // 最後に選択されたアイテム
        private int LastSelectedItem;

        private IList<ListBoxItem> ListBoxItems;
        private ListBoxItem SelectedItem => lstItems.SelectedIndex >= 0 ? ListBoxItems[lstItems.SelectedIndex] : null;
        private bool HasFlag => ListBoxItems.Any(x => x.ListItemFlag);

        public void ShowItems(frmMain MainForm, string lb_caption, IList<ListBoxItem> list, string lb_info, string lb_mode)
        {
            ListBoxItems = list;

            // コメントウィンドウの処理
            if (Strings.InStr(lb_mode, "コメント") > 0)
            {
                if (!txtComment.Enabled)
                {
                    txtComment.Enabled = true;
                    txtComment.Visible = true;
                    txtComment.Width = labCaption.Width;
                    txtComment.Text = "";
                    txtComment.Top = lstItems.Top + lstItems.Height + 5;
                    Height = Height + 40;
                }
            }
            else if (txtComment.Enabled)
            {
                txtComment.Enabled = false;
                txtComment.Visible = false;
                Height = Height - 40;
            }

            // キャプション
            Text = lb_caption;
            if (HasFlag)
            {
                labCaption.Text = "  " + lb_info;
            }
            else
            {
                labCaption.Text = lb_info;
            }

            // リストボックスにアイテムを追加
            lstItems.Visible = false;
            lstItems.Items.Clear();
            if (HasFlag)
            {
                var itemTexts = list.Select(x => x.ListItemFlag ? $"×{x.Text}" : $"  {x.Text}");
                foreach (var text in itemTexts)
                {
                    lstItems.Items.Add(text);
                }

                // XXX 後でクリアしてるから意味ないのでは？
                //i = (short)Information.UBound(list);
                //while (i > 0)
                //{
                //    if (!ListItemFlag[i])
                //    {
                //        lstItems.SelectedIndex = i - 1;
                //        break;
                //    }

                //    i = (short)(i - 1);
                //}
            }
            else
            {
                foreach (var text in list.Select(x => x.Text))
                {
                    lstItems.Items.Add(text);
                }
            }

            lstItems.SelectedIndex = -1;
            lstItems.Visible = true;

            //// コメント付きのアイテム？
            //if (Information.UBound(ListItemComment) != Information.UBound(list))
            //{
            //    Array.Resize(ListItemComment, Information.UBound(list) + 1);
            //}

            // 先頭のアイテムを設定
            if (GUI.TopItem > 0)
            {
                if (lstItems.TopIndex != GUI.TopItem - 1)
                {
                    lstItems.TopIndex = GeneralLib.MaxLng(GeneralLib.MinLng(GUI.TopItem - 1, lstItems.Items.Count - 1), 0);
                }
                lstItems.SelectedIndex = GUI.TopItem - 1;
            }
            else
            {
                lstItems.SelectedIndex = 0;
            }

            // コメントウィンドウの表示
            if (txtComment.Enabled)
            {
                txtComment.Text = SelectedItem?.ListItemComment;
            }

            // 最小化されている場合は戻しておく
            if (WindowState != FormWindowState.Normal)
            {
                WindowState = FormWindowState.Normal;
                Show();
            }

            // 表示位置を設定
            if (MainForm.Visible && HorizontalSize == "S")
            {
                Left = MainForm.Left;
            }
            else
            {
                Left = Screen.PrimaryScreen.Bounds.Width - Width / 2;
            }

            if (MainForm.Visible
                && MainForm.WindowState != FormWindowState.Minimized
                && VerticalSize == "M"
                && Strings.InStr(lb_mode, "中央表示") == 0)
            {
                Top = MainForm.Top + MainForm.Height - Height;
            }
            else
            {
                Top = Screen.PrimaryScreen.Bounds.Height - Height / 2;
            }

            // XXX この辺からは親にいたほうがしっくりくる
            Commands.SelectedItem = 0;
            GUI.IsFormClicked = false;
            Application.DoEvents();

            // リストボックスを表示
            if (Strings.InStr(lb_mode, "表示のみ") > 0)
            {
                // 表示のみを行う
                GUI.IsMordal = false;
                Show();
                lstItems.Focus();
                //SetWindowPos(Handle.ToInt32(), -1, 0, 0, 0, 0, 0x3);
                Refresh();
                //return ListBoxRet;
            }
            else if (Strings.InStr(lb_mode, "連続表示") > 0)
            {
                // 選択が行われてもリストボックスを閉じない
                GUI.IsMordal = false;
                if (!Visible)
                {
                    Show();
                    //SetWindowPos(Handle.ToInt32(), -1, 0, 0, 0, 0, 0x3);
                    lstItems.Focus();
                }

                if (Strings.InStr(lb_mode, "カーソル移動") > 0)
                {
                    if (SRC.AutoMoveCursor)
                    {
                        string argcursor_mode1 = "ダイアログ";
                        GUI.MoveCursorPos(argcursor_mode1);
                    }
                }

                while (!GUI.IsFormClicked)
                {
                    Application.DoEvents();
                    //// 右ボタンでのダブルクリックの実現
                    //if ((GetAsyncKeyState(RButtonID) & 0x8000) == 0)
                    //{
                    //    is_rbutton_released = true;
                    //}
                    //else if (is_rbutton_released)
                    //{
                    //    IsFormClicked = true;
                    //}

                    Thread.Sleep(50);
                }
            }
            else
            {
                // 選択が行われた時点でリストボックスを閉じる
                GUI.IsMordal = false;
                Show();
                //SetWindowPos(Handle.ToInt32(), -1, 0, 0, 0, 0, 0x3);
                lstItems.Focus();
                if (Strings.InStr(lb_mode, "カーソル移動") > 0)
                {
                    if (SRC.AutoMoveCursor)
                    {
                        string argcursor_mode = "ダイアログ";
                        GUI.MoveCursorPos(argcursor_mode);
                    }
                }

                while (!GUI.IsFormClicked)
                {
                    Application.DoEvents();
                    //// 右ボタンでのダブルクリックの実現
                    //if ((GetAsyncKeyState(RButtonID) & 0x8000) == 0)
                    //{
                    //    is_rbutton_released = true;
                    //}
                    //else if (is_rbutton_released)
                    //{
                    //    IsFormClicked = true;
                    //}

                    Thread.Sleep(50);
                }

                Hide();
                if (Strings.InStr(lb_mode, "カーソル移動") > 0
                    && Strings.InStr(lb_mode, "カーソル移動(行きのみ)") == 0)
                {
                    if (SRC.AutoMoveCursor)
                    {
                        GUI.RestoreCursorPos();
                    }
                }

                if (txtComment.Enabled)
                {
                    txtComment.Enabled = false;
                    txtComment.Visible = false;
                    Height = Height - 40;
                }
            }
        }

        // リストボックスへのキー入力
        private void frmListBox_KeyDown(object eventSender, KeyEventArgs eventArgs)
        {
            var KeyCode = eventArgs.KeyCode;
            // 既にウィンドウが隠れている場合は無視
            if (!Visible)
            {
                return;
            }

            switch (KeyCode)
            {
                case Keys.Up:
                    {
                        break;
                    }

                case Keys.Down:
                    {
                        break;
                    }

                case Keys.Left:
                case Keys.Right:
                    {
                        break;
                    }

                case Keys.Escape:
                case Keys.Delete:
                case Keys.Back:
                    {
                        // キャンセル
                        Commands.SelectedItem = 0;
                        LastSelectedItem = Commands.SelectedItem;
                        GUI.TopItem = lstItems.TopIndex;
                        if (GUI.IsFormClicked)
                        {
                            Hide();
                        }

                        GUI.IsFormClicked = true;
                        break;
                    }

                default:
                    {
                        // 選択
                        if (lstItems.SelectedIndex < 0)
                        {
                            return;
                        }

                        if (SelectedItem.ListItemFlag)
                        {
                            return;
                        }

                        Commands.SelectedItem = (lstItems.SelectedIndex + 1);
                        LastSelectedItem = Commands.SelectedItem;
                        GUI.TopItem = (lstItems.TopIndex + 1);
                        if (GUI.IsFormClicked)
                        {
                            Hide();
                        }

                        GUI.IsFormClicked = true;
                        break;
                    }
            }
        }

        // リストボックスを開く
        private void frmListBox_Load(object eventSender, EventArgs eventArgs)
        {
            // リストボックスを常に手前に表示
            TopMost = true;
            HorizontalSize = "M";
            VerticalSize = "M";
        }

        // リストボックスを閉じる
        private void frmListBox_FormClosed(object eventSender, FormClosedEventArgs eventArgs)
        {
            GUI.TopItem = (lstItems.TopIndex + 1);
            GUI.IsFormClicked = true;
            // XXX これ要る？
            //if (!GUI.IsMordal & Visible)
            //{
            //    // UPGRADE_ISSUE: Event パラメータ Cancel はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FB723E3C-1C06-4D2B-B083-E6CD0D334DA8"' をクリックしてください。
            //    Cancel = 1;
            //}

            Hide();
        }

        // 項目をダブルクリック
        private void lstItems_DoubleClick(object eventSender, EventArgs eventArgs)
        {
            // 無効なアイテムが選択されている？
            if (lstItems.SelectedIndex < 0)
            {
                return;
            }

            if (SelectedItem.ListItemFlag)
            {
                return;
            }

            if (LastSelectedItem != 0)
            {
                // 連続で選択
                if (!Visible)
                {
                    return;
                }

                Commands.SelectedItem = (lstItems.SelectedIndex + 1);
                LastSelectedItem = Commands.SelectedItem;
                GUI.TopItem = (lstItems.TopIndex + 1);
                if (GUI.IsFormClicked)
                {
                    Hide();
                }

                GUI.IsFormClicked = true;
            }
            else
            {
                // 連続でキャンセル
                Commands.SelectedItem = 0;
                LastSelectedItem = Commands.SelectedItem;
                GUI.TopItem = (lstItems.TopIndex + 1);
                if (GUI.IsFormClicked)
                {
                    Hide();
                }

                GUI.IsFormClicked = true;
            }
        }

        // マウスでクリック
        private void lstItems_MouseDown(object eventSender, MouseEventArgs eventArgs)
        {
            var Button = ResolveMouseButton(eventArgs);
            //int Button = (eventArgs.Button / 0x100000);
            //int Shift = (ModifierKeys / 0x10000);
            //float X = (float)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(eventArgs.X);
            //float Y = (float)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(eventArgs.Y);
            switch (Button)
            {
                case GuiButton.Left:
                    // 選択
                    if (!Visible)
                    {
                        return;
                    }

                    // 無効なアイテムが選択されている？
                    if (lstItems.SelectedIndex < 0)
                    {
                        return;
                    }

                    if (SelectedItem.ListItemFlag)
                    {
                        return;
                    }

                    Commands.SelectedItem = (lstItems.SelectedIndex + 1);
                    LastSelectedItem = Commands.SelectedItem;
                    GUI.TopItem = (lstItems.TopIndex + 1);
                    GUI.IsFormClicked = true;
                    if (CurrentTime > 0)
                    {
                        Timer1.Enabled = false;
                        Hide();
                    }

                    break;

                case GuiButton.Right:
                default:
                    // キャンセル
                    Commands.SelectedItem = 0;
                    LastSelectedItem = Commands.SelectedItem;
                    GUI.TopItem = (lstItems.TopIndex + 1);
                    GUI.IsFormClicked = true;
                    break;
            }
        }

        // キャプション部分をクリック
        private void labCaption_MouseDown(object eventSender, MouseEventArgs eventArgs)
        {
            var Button = ResolveMouseButton(eventArgs);
            //int Button = (eventArgs.Button / 0x100000);
            //int Shift = (ModifierKeys / 0x10000);
            //float X = (float)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(eventArgs.X);
            //float Y = (float)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(eventArgs.Y);
            switch (Button)
            {
                case GuiButton.Left:
                    // ユニットステータスを表示しているユニットを入れ替え
                    if (GUI.MainFormVisible)
                    {
                        if (Status.DisplayedUnit is object & Commands.SelectedUnit is object & Commands.SelectedTarget is object)
                        {
                            if ((Status.DisplayedUnit.ID ?? "") == (Commands.SelectedUnit.ID ?? ""))
                            {
                                Status.DisplayUnitStatus(Commands.SelectedTarget);
                            }
                            else
                            {
                                Status.DisplayUnitStatus(Commands.SelectedUnit);
                            }
                        }
                    }

                    break;

                case GuiButton.Right:
                default:
                    // キャンセル
                    Commands.SelectedItem = 0;
                    LastSelectedItem = Commands.SelectedItem;
                    GUI.TopItem = lstItems.TopIndex;
                    if (GUI.IsFormClicked)
                    {
                        Hide();
                    }

                    GUI.IsFormClicked = true;
                    break;
            }
        }

        // リストボックスの端をクリック
        private void frmListBox_MouseDown(object eventSender, MouseEventArgs eventArgs)
        {
            var Button = ResolveMouseButton(eventArgs);
            if (Button == GuiButton.Right)
            {
                // キャンセル
                Commands.SelectedItem = 0;
                LastSelectedItem = Commands.SelectedItem;
                GUI.TopItem = lstItems.TopIndex;
                if (GUI.IsFormClicked)
                {
                    Hide();
                }

                GUI.IsFormClicked = true;
            }
        }

        // リストボックス上でマウスカーソルを動かす
        private void lstItems_MouseMove(object eventSender, MouseEventArgs eventArgs)
        {
            // カーソルがあるアイテムを算出
            var point = lstItems.PointToClient(Cursor.Position);
            var itm = lstItems.IndexFromPoint(point);

            // カーソル上のアイテムをハイライト表示
            if (itm < 0 | itm >= lstItems.Items.Count)
            {
                lstItems.SelectedIndex = -1;
                return;
            }

            if (lstItems.SelectedIndex == itm)
            {
                return;
            }

            lstItems.SelectedIndex = itm;

            // コメント欄を更新
            if (txtComment.Enabled)
            {
                txtComment.Text = SelectedItem?.ListItemComment;
            }
        }

        // Questionコマンド対応
        private void Timer1_Tick(object eventSender, EventArgs eventArgs)
        {
            throw new NotImplementedException();
            //CurrentTime = (CurrentTime + 1);
            //picBar.Cls();
            //picBar.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //picBar.Refresh();
            //if (CurrentTime >= TimeLimit)
            //{
            //    Commands.SelectedItem = 0;
            //    LastSelectedItem = Commands.SelectedItem;
            //    GUI.TopItem = lstItems.TopIndex;
            //    Hide();
            //    Timer1.Enabled = false;
            //}
        }

        // 選択されているアイテムに対応するユニットのステータス表示
        private void Timer2_Tick(object eventSender, EventArgs eventArgs)
        {
//            Unit u;
//            if (!Visible | !GUI.MainForm.Visible)
//            {
//                return;
//            }

//            if (lstItems.SelectedIndex == -1)
//            {
//                return;
//            };
//#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
//            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 10644


//            Input:

//                    On Error GoTo ErrorHandler

//             */
//            if (lstItems.SelectedIndex >= Information.UBound(GUI.ListItemID))
//            {
//                return;
//            }

//            // 選択されたユニットが存在する？
//            bool localIsDefined2() { var tmp = GUI.ListItemID; object argIndex1 = tmp[lstItems.SelectedIndex + 1]; var ret = SRC.UList.IsDefined2(argIndex1); return ret; }

//            if (!localIsDefined2())
//            {
//                return;
//            }

//            var tmp = GUI.ListItemID;
//            object argIndex1 = tmp[lstItems.SelectedIndex + 1];
//            u = SRC.UList.Item2(argIndex1);

//            // 選択されたユニットにパイロットが乗っている？
//            if (u.CountPilot() == 0)
//            {
//                return;
//            }

//            // 既に表示している？
//            if (ReferenceEquals(Status.DisplayedUnit, u))
//            {
//                return;
//            }

//            // 選択されたユニットをステータスウィンドウに表示
//            Status.DisplayUnitStatus(u);
//        ErrorHandler:
//            ;
        }
        private static GuiButton ResolveMouseButton(MouseEventArgs eventArgs)
        {
            // 右でなければ左とみなす
            return eventArgs.Button.HasFlag(MouseButtons.Right)
                ? GuiButton.Right
                : GuiButton.Left;
        }
    }
}