using System;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Project1
{
    internal partial class frmMultiSelectListBox : Form
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // 出撃ユニット選択用リストボックスのフォーム

        // 選択されたユニットの数
        private short SelectedItemNum;
        // ユニットが選択されたかどうかを示すフラグ
        private bool[] ItemFlag;

        // 選択終了ボタンをクリック
        private void cmdFinish_Click(object eventSender, EventArgs eventArgs)
        {
            //short i;
            //var loopTo = (short)Information.UBound(GUI.ListItemFlag);
            //for (i = 1; i <= loopTo; i++)
            //    GUI.ListItemFlag[i] = ItemFlag[i - 1];
            //Status.ClearUnitStatus();
            Close();
        }

        // マップを見るボタンをクリック
        private void cmdResume_Click(object eventSender, EventArgs eventArgs)
        {
            //short i;
            //var loopTo = (short)Information.UBound(GUI.ListItemFlag);
            //for (i = 1; i <= loopTo; i++)
            //    GUI.ListItemFlag[i] = false;
            //Status.ClearUnitStatus();
            Close();
        }

        // 「先頭から選択」ボタンをクリック
        private void cmdSelectAll_Click(object eventSender, EventArgs eventArgs)
        {
            //short i;
            //lstItems.Visible = false;
            //var loopTo = (short)lstItems.Items.Count;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    ItemFlag[i - 1] = false;
            //    Microsoft.VisualBasic.Compatibility.VB6.Support.SetItemString(lstItems, i - 1, "　" + Strings.Mid(Microsoft.VisualBasic.Compatibility.VB6.Support.GetItemString(lstItems, i - 1), 2));
            //}

            //var loopTo1 = (short)GeneralLib.MinLng(GUI.MaxListItem, lstItems.Items.Count);
            //for (i = 1; i <= loopTo1; i++)
            //{
            //    if (!ItemFlag[i - 1])
            //    {
            //        ItemFlag[i - 1] = true;
            //        Microsoft.VisualBasic.Compatibility.VB6.Support.SetItemString(lstItems, i - 1, "○" + Strings.Mid(Microsoft.VisualBasic.Compatibility.VB6.Support.GetItemString(lstItems, i - 1), 2));
            //    }
            //}

            //lstItems.TopIndex = 0;
            //lstItems.Visible = true;
            //SelectedItemNum = 0;
            //var loopTo2 = (short)(lstItems.Items.Count - 1);
            //for (i = 0; i <= loopTo2; i++)
            //{
            //    if (ItemFlag[i])
            //    {
            //        SelectedItemNum = (short)(SelectedItemNum + 1);
            //    }
            //}

            //lblNumber.Text = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SelectedItemNum) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(GUI.MaxListItem);
            //if (SelectedItemNum > 0 & SelectedItemNum <= GUI.MaxListItem)
            //{
            //    if (!cmdFinish.Enabled)
            //    {
            //        cmdFinish.Enabled = true;
            //    }
            //}
            //else if (cmdFinish.Enabled)
            //{
            //    cmdFinish.Enabled = false;
            //}
        }

        // 「最後から選択」ボタンをクリック
        private void cmdSelectAll2_Click(object eventSender, EventArgs eventArgs)
        {
            //short i;
            //lstItems.Visible = false;
            //var loopTo = (short)lstItems.Items.Count;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    ItemFlag[i - 1] = false;
            //    Microsoft.VisualBasic.Compatibility.VB6.Support.SetItemString(lstItems, i - 1, "　" + Strings.Mid(Microsoft.VisualBasic.Compatibility.VB6.Support.GetItemString(lstItems, i - 1), 2));
            //}

            //var loopTo1 = (short)GeneralLib.MinLng(GUI.MaxListItem, lstItems.Items.Count);
            //for (i = 1; i <= loopTo1; i++)
            //{
            //    if (!ItemFlag[lstItems.Items.Count - i])
            //    {
            //        ItemFlag[lstItems.Items.Count - i] = true;
            //        Microsoft.VisualBasic.Compatibility.VB6.Support.SetItemString(lstItems, lstItems.Items.Count - i, "○" + Strings.Mid(Microsoft.VisualBasic.Compatibility.VB6.Support.GetItemString(lstItems, lstItems.Items.Count - i), 2));
            //    }
            //}

            //lstItems.TopIndex = GeneralLib.MaxLng(lstItems.Items.Count - 14, 0);
            //lstItems.Visible = true;
            //SelectedItemNum = 0;
            //var loopTo2 = (short)(lstItems.Items.Count - 1);
            //for (i = 0; i <= loopTo2; i++)
            //{
            //    if (ItemFlag[i])
            //    {
            //        SelectedItemNum = (short)(SelectedItemNum + 1);
            //    }
            //}

            //lblNumber.Text = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SelectedItemNum) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(GUI.MaxListItem);
            //if (SelectedItemNum > 0 & SelectedItemNum <= GUI.MaxListItem)
            //{
            //    if (!cmdFinish.Enabled)
            //    {
            //        cmdFinish.Enabled = true;
            //    }
            //}
            //else if (cmdFinish.Enabled)
            //{
            //    cmdFinish.Enabled = false;
            //}
        }

        // 「～順」ボタンをクリック
        private void cmdSort_Click(object eventSender, EventArgs eventArgs)
        {
            //string[] item_list;
            //int[] key_list;
            //string[] strkey_list;
            //short max_item;
            //int max_value;
            //string max_str;
            //short i, j;
            //string buf;
            //bool flag;

            //// 現在のリスト表示内容をコピー
            //{
            //    var withBlock = lstItems;
            //    item_list = new string[withBlock.Items.Count + 1];
            //    var loopTo = (short)withBlock.Items.Count;
            //    for (i = 1; i <= loopTo; i++)
            //        item_list[i] = Microsoft.VisualBasic.Compatibility.VB6.Support.GetItemString(lstItems, i - 1);
            //}

            //if (cmdSort.Text == "レベル順に並べ替え")
            //{
            //    // メインパイロットのレベル順に並べ替え
            //    key_list = new int[Information.UBound(item_list) + 1];
            //    {
            //        var withBlock1 = SRC.UList;
            //        var loopTo1 = (short)Information.UBound(item_list);
            //        for (i = 1; i <= loopTo1; i++)
            //        {
            //            Unit localItem() { var tmp = GUI.ListItemID; object argIndex1 = tmp[i]; var ret = withBlock1.Item(ref argIndex1); return ret; }

            //            {
            //                var withBlock2 = localItem().MainPilot();
            //                key_list[i] = 500 * withBlock2.Level + withBlock2.Exp;
            //            }
            //        }
            //    }

            //    var loopTo2 = (short)(Information.UBound(item_list) - 1);
            //    for (i = 1; i <= loopTo2; i++)
            //    {
            //        max_item = i;
            //        max_value = key_list[i];
            //        var loopTo3 = (short)Information.UBound(item_list);
            //        for (j = (short)(i + 1); j <= loopTo3; j++)
            //        {
            //            if (key_list[j] > max_value)
            //            {
            //                max_item = j;
            //                max_value = key_list[j];
            //            }
            //        }

            //        if (max_item != i)
            //        {
            //            buf = item_list[i];
            //            item_list[i] = item_list[max_item];
            //            item_list[max_item] = buf;
            //            buf = GUI.ListItemID[i];
            //            GUI.ListItemID[i] = GUI.ListItemID[max_item];
            //            GUI.ListItemID[max_item] = buf;
            //            flag = ItemFlag[i - 1];
            //            ItemFlag[i - 1] = ItemFlag[max_item - 1];
            //            ItemFlag[max_item - 1] = flag;
            //            key_list[max_item] = key_list[i];
            //        }
            //    }
            //    // 並べ替え方法をトグルで切り替え
            //    cmdSort.Text = "名称順に並べ替え";
            //}
            //else
            //{
            //    // ユニットの名称順に並べ替え
            //    strkey_list = new string[Information.UBound(item_list) + 1];
            //    {
            //        var withBlock3 = SRC.UList;
            //        var loopTo4 = (short)Information.UBound(item_list);
            //        for (i = 1; i <= loopTo4; i++)
            //        {
            //            Unit localItem1() { var tmp = GUI.ListItemID; object argIndex1 = tmp[i]; var ret = withBlock3.Item(ref argIndex1); return ret; }

            //            strkey_list[i] = localItem1().KanaName;
            //        }
            //    }

            //    var loopTo5 = (short)(Information.UBound(item_list) - 1);
            //    for (i = 1; i <= loopTo5; i++)
            //    {
            //        max_item = i;
            //        max_str = strkey_list[i];
            //        var loopTo6 = (short)Information.UBound(item_list);
            //        for (j = (short)(i + 1); j <= loopTo6; j++)
            //        {
            //            if (Strings.StrComp(strkey_list[j], max_str, (CompareMethod)1) == -1)
            //            {
            //                max_item = j;
            //                max_str = strkey_list[j];
            //            }
            //        }

            //        if (max_item != i)
            //        {
            //            buf = item_list[i];
            //            item_list[i] = item_list[max_item];
            //            item_list[max_item] = buf;
            //            buf = GUI.ListItemID[i];
            //            GUI.ListItemID[i] = GUI.ListItemID[max_item];
            //            GUI.ListItemID[max_item] = buf;
            //            flag = ItemFlag[i - 1];
            //            ItemFlag[i - 1] = ItemFlag[max_item - 1];
            //            ItemFlag[max_item - 1] = flag;
            //            strkey_list[max_item] = strkey_list[i];
            //        }
            //    }
            //    // 並べ替え方法をトグルで切り替え
            //    cmdSort.Text = "レベル順に並べ替え";
            //}

            //// リスト表示を更新する
            //{
            //    var withBlock4 = lstItems;
            //    withBlock4.Visible = false;
            //    var loopTo7 = (short)withBlock4.Items.Count;
            //    for (i = 1; i <= loopTo7; i++)
            //        Microsoft.VisualBasic.Compatibility.VB6.Support.SetItemString(lstItems, i - 1, item_list[i]);
            //    withBlock4.TopIndex = 0;
            //    withBlock4.Visible = true;
            //}
        }

        // フォームを表示
        // UPGRADE_WARNING: Form イベント frmMultiSelectListBox.Activate には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
        private void frmMultiSelectListBox_Activated(object eventSender, EventArgs eventArgs)
        {
            //SelectedItemNum = 0;
            //lblNumber.Text = "0/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(GUI.MaxListItem);
            //ItemFlag = new bool[lstItems.Items.Count + 1];
            //if (lstItems.Items.Count > 0)
            //{
            //    Unit localItem() { var tmp = GUI.ListItemID; object argIndex1 = tmp[1]; var ret = SRC.UList.Item(ref argIndex1); return ret; }

            //    var argu = localItem();
            //    Status.DisplayUnitStatus(ref argu);
            //}
        }

        // リストボックス上でダブルクリック
        private void lstItems_DoubleClick(object eventSender, EventArgs eventArgs)
        {
            //short i;
            //i = (short)lstItems.SelectedIndex;
            //if (i >= 0)
            //{
            //    if (ItemFlag[i])
            //    {
            //        // 選択取り消し

            //        // 選択されたユニット数を減らす
            //        SelectedItemNum = (short)(SelectedItemNum - 1);
            //        lblNumber.Text = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SelectedItemNum) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(GUI.MaxListItem);
            //        ItemFlag[i] = false;

            //        // 選択状態の表示を更新
            //        Microsoft.VisualBasic.Compatibility.VB6.Support.SetItemString(lstItems, i, "　" + Strings.Mid(Microsoft.VisualBasic.Compatibility.VB6.Support.GetItemString(lstItems, i), 2));

            //        // 選択終了が可能か判定
            //        if (SelectedItemNum > 0 & SelectedItemNum <= GUI.MaxListItem)
            //        {
            //            if (!cmdFinish.Enabled)
            //            {
            //                cmdFinish.Enabled = true;
            //            }
            //        }
            //        else if (cmdFinish.Enabled)
            //        {
            //            cmdFinish.Enabled = false;
            //        }
            //    }
            //    else
            //    {
            //        // 選択

            //        // 選択されたユニット数を増やす
            //        SelectedItemNum = (short)(SelectedItemNum + 1);
            //        lblNumber.Text = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SelectedItemNum) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(GUI.MaxListItem);
            //        ItemFlag[i] = true;

            //        // 選択状態の表示を更新
            //        Microsoft.VisualBasic.Compatibility.VB6.Support.SetItemString(lstItems, i, "○" + Strings.Mid(Microsoft.VisualBasic.Compatibility.VB6.Support.GetItemString(lstItems, i), 2));

            //        // 選択終了が可能か判定
            //        if (SelectedItemNum > 0 & SelectedItemNum <= GUI.MaxListItem)
            //        {
            //            if (!cmdFinish.Enabled)
            //            {
            //                cmdFinish.Enabled = true;
            //            }
            //        }
            //        else if (cmdFinish.Enabled)
            //        {
            //            cmdFinish.Enabled = false;
            //        }
            //    }
            //}
        }

        // リストボックス上でマウスボタンを押す
        private void lstItems_MouseDown(object eventSender, MouseEventArgs eventArgs)
        {
            //short Button = (short)((int)eventArgs.Button / 0x100000);
            //short Shift = (short)((int)ModifierKeys / 0x10000);
            //float X = (float)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(eventArgs.X);
            //float Y = (float)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(eventArgs.Y);
            //short i;

            //// 左クリック以外は無視
            //if (Button != 1)
            //{
            //    return;
            //}

            //i = (short)lstItems.SelectedIndex;
            //if (i >= 0)
            //{
            //    if (ItemFlag[i])
            //    {
            //        // 選択取り消し

            //        // 選択されたユニット数を減らす
            //        SelectedItemNum = (short)(SelectedItemNum - 1);
            //        lblNumber.Text = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SelectedItemNum) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(GUI.MaxListItem);
            //        ItemFlag[i] = false;

            //        // 選択状態の表示を更新
            //        Microsoft.VisualBasic.Compatibility.VB6.Support.SetItemString(lstItems, i, "　" + Strings.Mid(Microsoft.VisualBasic.Compatibility.VB6.Support.GetItemString(lstItems, i), 2));

            //        // 選択終了が可能か判定
            //        if (SelectedItemNum > 0 & SelectedItemNum <= GUI.MaxListItem)
            //        {
            //            if (!cmdFinish.Enabled)
            //            {
            //                cmdFinish.Enabled = true;
            //            }
            //        }
            //        else if (cmdFinish.Enabled)
            //        {
            //            cmdFinish.Enabled = false;
            //        }
            //    }
            //    else
            //    {
            //        // 選択

            //        // 選択されたユニット数を増やす
            //        SelectedItemNum = (short)(SelectedItemNum + 1);
            //        lblNumber.Text = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SelectedItemNum) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(GUI.MaxListItem);
            //        ItemFlag[i] = true;

            //        // 選択状態の表示を更新
            //        Microsoft.VisualBasic.Compatibility.VB6.Support.SetItemString(lstItems, i, "○" + Strings.Mid(Microsoft.VisualBasic.Compatibility.VB6.Support.GetItemString(lstItems, i), 2));

            //        // 選択終了が可能か判定
            //        if (SelectedItemNum > 0 & SelectedItemNum <= GUI.MaxListItem)
            //        {
            //            if (!cmdFinish.Enabled)
            //            {
            //                cmdFinish.Enabled = true;
            //            }
            //        }
            //        else if (cmdFinish.Enabled)
            //        {
            //            cmdFinish.Enabled = false;
            //        }
            //    }
            //}
        }

        // リストボックス上でマウスカーソルを移動
        private void lstItems_MouseMove(object eventSender, MouseEventArgs eventArgs)
        {
            //short Button = (short)((int)eventArgs.Button / 0x100000);
            //short Shift = (short)((int)ModifierKeys / 0x10000);
            //float X = (float)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(eventArgs.X);
            //float Y = (float)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(eventArgs.Y);
            //short itm;

            //// カーソルがあるアイテムを算出
            //itm = (short)(((long)(Y * ClientRectangle.Width) / (long)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(Width) + 1L) / 16L);
            //itm = (short)(itm + lstItems.TopIndex);

            //// カーソルがあるアイテムをハイライト表示
            //if (itm < 0 | itm >= lstItems.Items.Count)
            //{
            //    lstItems.SelectedIndex = -1;
            //    return;
            //}

            //if (lstItems.SelectedIndex == itm)
            //{
            //    return;
            //}

            //lstItems.SelectedIndex = itm;
        }

        // カーソルが指すユニットを一定時間ごとに調べてステータスウィンドウに表示
        private void Timer1_Tick(object eventSender, EventArgs eventArgs)
        {
            //Unit u;
            //if (!Visible)
            //{
            //    return;
            //}

            //if (lstItems.SelectedIndex == -1)
            //{
            //    return;
            //}

            //var tmp = GUI.ListItemID;
            //object argIndex1 = tmp[lstItems.SelectedIndex + 1];
            //u = SRC.UList.Item(ref argIndex1);
            //if (!ReferenceEquals(Status.DisplayedUnit, u))
            //{
            //    // ユニット選択中だけ
            //    if (Commands.CommandState == "ユニット選択")
            //    {
            //        Status.DisplayUnitStatus(ref u);
            //    }
            //}
        }
    }
}