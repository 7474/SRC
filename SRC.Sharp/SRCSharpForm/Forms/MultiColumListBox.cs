using System;
using System.Windows.Forms;

namespace Project1
{
    internal partial class frmMultiColumnListBox : Form
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // 多段のリストボックスのフォーム

        // フォームを表示
        // UPGRADE_WARNING: Form イベント frmMultiColumnListBox.Activate には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
        private void frmMultiColumnListBox_Activated(object eventSender, EventArgs eventArgs)
        {
            //Commands.SelectedItem = 0;
            //labCaption.Text = "";
        }

        // フォームをロード
        private void frmMultiColumnListBox_Load(object eventSender, EventArgs eventArgs)
        {
            int ret;

            // 常に手前に表示
            //ret = GUI.SetWindowPos(Handle.ToInt32(), -1, 0, 0, 0, 0, 0x3);
        }

        // フォームを閉じる
        private void frmMultiColumnListBox_FormClosed(object eventSender, FormClosedEventArgs eventArgs)
        {
            //GUI.TopItem = (short)(lstItems.TopIndex + 1);
            //GUI.IsFormClicked = true;
            //if (!GUI.IsMordal & Visible)
            //{
            //    Cancel = 1;
            //}

            Hide();
        }

        // フォーム上でマウスボタンを押す
        private void lstItems_MouseDown(object eventSender, MouseEventArgs eventArgs)
        {
            //short Button = (short)((int)eventArgs.Button / 0x100000);
            //short Shift = (short)((int)ModifierKeys / 0x10000);
            //float X = (float)SrcFormatter.PixelsToTwipsX(eventArgs.X);
            //float Y = (float)SrcFormatter.PixelsToTwipsY(eventArgs.Y);
            //switch (Button)
            //{
            //    case 1:
            //        {
            //            // 選択
            //            if (!Visible)
            //            {
            //                return;
            //            }

            //            if (lstItems.SelectedIndex < 0 | GUI.ListItemFlag[lstItems.SelectedIndex + 1])
            //            {
            //                return;
            //            }

            //            Commands.SelectedItem = (short)(lstItems.SelectedIndex + 1);
            //            GUI.TopItem = (short)(lstItems.TopIndex + 1);
            //            if (GUI.IsFormClicked)
            //            {
            //                Hide();
            //            }

            //            GUI.IsFormClicked = true;
            //            break;
            //        }

            //    case 2:
            //        {
            //            // キャンセル
            //            Commands.SelectedItem = 0;
            //            GUI.TopItem = (short)(lstItems.TopIndex + 1);
            //            if (GUI.IsFormClicked)
            //            {
            //                Hide();
            //            }

            //            GUI.IsFormClicked = true;
            //            break;
            //        }
            //}
        }

        // フォーム上でマウスボタンを押す
        private void frmMultiColumnListBox_MouseDown(object eventSender, MouseEventArgs eventArgs)
        {
            //short Button = (short)((int)eventArgs.Button / 0x100000);
            //short Shift = (short)((int)ModifierKeys / 0x10000);
            //float X = eventArgs.X;
            //float Y = eventArgs.Y;
            //if (Button == 2)
            //{
            //    // キャンセルのみ受け付け
            //    Commands.SelectedItem = 0;
            //    GUI.TopItem = (short)lstItems.TopIndex;
            //    if (GUI.IsFormClicked)
            //    {
            //        Hide();
            //    }

            //    GUI.IsFormClicked = true;
            //}
        }

        // リストボックス上でマウスカーソルを移動
        private void lstItems_MouseMove(object eventSender, MouseEventArgs eventArgs)
        {
            //short Button = (short)((int)eventArgs.Button / 0x100000);
            //short Shift = (short)((int)ModifierKeys / 0x10000);
            //float X = (float)SrcFormatter.PixelsToTwipsX(eventArgs.X);
            //float Y = (float)SrcFormatter.PixelsToTwipsY(eventArgs.Y);
            //short itm;
            //short lines;
            //{
            //    var withBlock = lstItems;
            //    // リストボックスの行数
            //    lines = 25;
            //    if (withBlock.Items.Count > lines * withBlock.Columns)
            //    {
            //        lines = (short)(lines - 1);
            //    }

            //    // マウスカーソルがあるアイテムを算出
            //    itm = (long)(X * ClientRectangle.Width) / (long)SrcFormatter.PixelsToTwipsX(Width) / (withBlock.Width / withBlock.Columns) * lines;
            //    itm = (short)(itm + ((long)(Y * ClientRectangle.Width) / (long)SrcFormatter.PixelsToTwipsX(Width) + 1L) / 16L);
            //    itm = (short)(itm + withBlock.TopIndex);

            //    // カーソル上のアイテムをハイライト表示
            //    if (itm < 0 | itm >= withBlock.Items.Count)
            //    {
            //        withBlock.SelectedIndex = -1;
            //        return;
            //    }

            //    if (withBlock.SelectedIndex == itm)
            //    {
            //        return;
            //    }

            //    withBlock.SelectedIndex = itm;

            //    // 解説の表示
            //    labCaption.Text = GUI.ListItemComment[itm + 1];
            //}
        }
    }
}
