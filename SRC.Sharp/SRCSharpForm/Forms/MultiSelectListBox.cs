// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore;
using SRCCore.Lib;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace SRCSharpForm
{
    // 出撃ユニット選択用リストボックスのフォーム
    internal partial class frmMultiSelectListBox : Form
    {
        private SRCCore.SRC SRC { get; set; }

        // 選択されたユニットの数
        public int SelectedItemNum { get; private set; }
        // 最大選択数
        private int MaxListItem;

        private BindingList<MultiSelectListBoxItem> ListBoxItems;

        public void Init(SRC src, ListBoxArgs args, int max_num)
        {
            SRC = src;
            Text = args.lb_caption;
            lblCaption.Text = "　" + args.lb_info;
            cmdSort.Text = "名称順に並べ替え";

            ListBoxItems = new BindingList<MultiSelectListBoxItem>(args.Items.Select(x => new MultiSelectListBoxItem(x)).ToList());
            MaxListItem = max_num;

            lstItems.DataSource = ListBoxItems;
            lstItems.DisplayMember = "Text";
        }

        // 選択終了ボタンをクリック
        private void cmdFinish_Click(object eventSender, EventArgs eventArgs)
        {
            SRC.GUIStatus.ClearUnitStatus();
            Close();
        }

        // マップを見るボタンをクリック
        private void cmdResume_Click(object eventSender, EventArgs eventArgs)
        {
            foreach (var item in ListBoxItems)
            {
                item.ListItemFlag = false;
            }
            SRC.GUIStatus.ClearUnitStatus();
            Close();
        }

        // 「先頭から選択」ボタンをクリック
        private void cmdSelectAll_Click(object eventSender, EventArgs eventArgs)
        {
            foreach (var item in ListBoxItems)
            {
                item.ListItemFlag = false;
            }
            foreach (var item in ListBoxItems.Take(MaxListItem))
            {
                item.ListItemFlag = true;
            }
            UpdateList();
            lstItems.TopIndex = 0;
        }

        // 「最後から選択」ボタンをクリック
        private void cmdSelectAll2_Click(object eventSender, EventArgs eventArgs)
        {
            foreach (var item in ListBoxItems)
            {
                item.ListItemFlag = false;
            }
            foreach (var item in ListBoxItems.Reverse().Take(MaxListItem))
            {
                item.ListItemFlag = true;
            }
            UpdateList();
            lstItems.TopIndex = GeneralLib.MaxLng(lstItems.Items.Count - 14, 0);
        }

        // 「～順」ボタンをクリック
        private void cmdSort_Click(object eventSender, EventArgs eventArgs)
        {
            //string[] item_list;
            //int[] key_list;
            //string[] strkey_list;
            //int max_item;
            //int max_value;
            //string max_str;
            //int i, j;
            //string buf;
            //bool flag;

            //// 現在のリスト表示内容をコピー
            //{
            //    var withBlock = lstItems;
            //    item_list = new string[withBlock.Items.Count + 1];
            //    var loopTo = (int)withBlock.Items.Count;
            //    for (i = 1; i <= loopTo; i++)
            //        item_list[i] = SrcFormatter.GetItemString(lstItems, i - 1);
            //}

            //if (cmdSort.Text == "レベル順に並べ替え")
            //{
            //    // メインパイロットのレベル順に並べ替え
            //    key_list = new int[Information.UBound(item_list) + 1];
            //    {
            //        var withBlock1 = SRC.UList;
            //        var loopTo1 = (int)Information.UBound(item_list);
            //        for (i = 1; i <= loopTo1; i++)
            //        {
            //            Unit localItem() { var tmp = GUI.ListItemID; object argIndex1 = tmp[i]; var ret = withBlock1.Item(ref argIndex1); return ret; }

            //            {
            //                var withBlock2 = localItem().MainPilot();
            //                key_list[i] = 500 * withBlock2.Level + withBlock2.Exp;
            //            }
            //        }
            //    }

            //    var loopTo2 = (int)(Information.UBound(item_list) - 1);
            //    for (i = 1; i <= loopTo2; i++)
            //    {
            //        max_item = i;
            //        max_value = key_list[i];
            //        var loopTo3 = (int)Information.UBound(item_list);
            //        for (j = (int)(i + 1); j <= loopTo3; j++)
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
            //        var loopTo4 = (int)Information.UBound(item_list);
            //        for (i = 1; i <= loopTo4; i++)
            //        {
            //            Unit localItem1() { var tmp = GUI.ListItemID; object argIndex1 = tmp[i]; var ret = withBlock3.Item(ref argIndex1); return ret; }

            //            strkey_list[i] = localItem1().KanaName;
            //        }
            //    }

            //    var loopTo5 = (int)(Information.UBound(item_list) - 1);
            //    for (i = 1; i <= loopTo5; i++)
            //    {
            //        max_item = i;
            //        max_str = strkey_list[i];
            //        var loopTo6 = (int)Information.UBound(item_list);
            //        for (j = (int)(i + 1); j <= loopTo6; j++)
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
            //    var loopTo7 = (int)withBlock4.Items.Count;
            //    for (i = 1; i <= loopTo7; i++)
            //        SrcFormatter.SetItemString(lstItems, i - 1, item_list[i]);
            //    withBlock4.TopIndex = 0;
            //    withBlock4.Visible = true;
            //}
        }

        // フォームを表示
        private void frmMultiSelectListBox_Activated(object eventSender, EventArgs eventArgs)
        {
            if (ListBoxItems.Any())
            {
                SRC.GUIStatus.DisplayUnitStatus(SRC.UList.Item(ListBoxItems.First().ListItemID));
            }
            UpdateList();
        }

        // リストボックス上でダブルクリック
        private void lstItems_DoubleClick(object eventSender, EventArgs eventArgs)
        {
            //int i;
            //i = (int)lstItems.SelectedIndex;
            //if (i >= 0)
            //{
            //    if (ItemFlag[i])
            //    {
            //        // 選択取り消し

            //        // 選択されたユニット数を減らす
            //        SelectedItemNum = (int)(SelectedItemNum - 1);
            //        lblNumber.Text = SrcFormatter.Format(SelectedItemNum) + "/" + SrcFormatter.Format(GUI.MaxListItem);
            //        ItemFlag[i] = false;

            //        // 選択状態の表示を更新
            //        SrcFormatter.SetItemString(lstItems, i, "　" + Strings.Mid(SrcFormatter.GetItemString(lstItems, i), 2));

            //        // 選択終了が可能か判定
            //        if (SelectedItemNum > 0 && SelectedItemNum <= GUI.MaxListItem)
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
            //        SelectedItemNum = (int)(SelectedItemNum + 1);
            //        lblNumber.Text = SrcFormatter.Format(SelectedItemNum) + "/" + SrcFormatter.Format(GUI.MaxListItem);
            //        ItemFlag[i] = true;

            //        // 選択状態の表示を更新
            //        SrcFormatter.SetItemString(lstItems, i, "○" + Strings.Mid(SrcFormatter.GetItemString(lstItems, i), 2));

            //        // 選択終了が可能か判定
            //        if (SelectedItemNum > 0 && SelectedItemNum <= GUI.MaxListItem)
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
            //int Button = (int)((int)eventArgs.Button / 0x100000);
            //int Shift = (int)((int)ModifierKeys / 0x10000);
            //float X = (float)SrcFormatter.PixelsToTwipsX(eventArgs.X);
            //float Y = (float)SrcFormatter.PixelsToTwipsY(eventArgs.Y);
            //int i;

            //// 左クリック以外は無視
            //if (Button != 1)
            //{
            //    return;
            //}

            //i = (int)lstItems.SelectedIndex;
            //if (i >= 0)
            //{
            //    if (ItemFlag[i])
            //    {
            //        // 選択取り消し

            //        // 選択されたユニット数を減らす
            //        SelectedItemNum = (int)(SelectedItemNum - 1);
            //        lblNumber.Text = SrcFormatter.Format(SelectedItemNum) + "/" + SrcFormatter.Format(GUI.MaxListItem);
            //        ItemFlag[i] = false;

            //        // 選択状態の表示を更新
            //        SrcFormatter.SetItemString(lstItems, i, "　" + Strings.Mid(SrcFormatter.GetItemString(lstItems, i), 2));

            //        // 選択終了が可能か判定
            //        if (SelectedItemNum > 0 && SelectedItemNum <= GUI.MaxListItem)
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
            //        SelectedItemNum = (int)(SelectedItemNum + 1);
            //        lblNumber.Text = SrcFormatter.Format(SelectedItemNum) + "/" + SrcFormatter.Format(GUI.MaxListItem);
            //        ItemFlag[i] = true;

            //        // 選択状態の表示を更新
            //        SrcFormatter.SetItemString(lstItems, i, "○" + Strings.Mid(SrcFormatter.GetItemString(lstItems, i), 2));

            //        // 選択終了が可能か判定
            //        if (SelectedItemNum > 0 && SelectedItemNum <= GUI.MaxListItem)
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
            //int Button = (int)((int)eventArgs.Button / 0x100000);
            //int Shift = (int)((int)ModifierKeys / 0x10000);
            //float X = (float)SrcFormatter.PixelsToTwipsX(eventArgs.X);
            //float Y = (float)SrcFormatter.PixelsToTwipsY(eventArgs.Y);
            //int itm;

            //// カーソルがあるアイテムを算出
            //itm = (int)(((long)(Y * ClientRectangle.Width) / (long)SrcFormatter.PixelsToTwipsX(Width) + 1L) / 16L);
            //itm = (int)(itm + lstItems.TopIndex);

            //// カーソルがあるアイテムをハイライト表示
            //if (itm < 0 || itm >= lstItems.Items.Count)
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
            //u = SRC.UList.Item(ref tmp[lstItems.SelectedIndex + 1]);
            //if (!ReferenceEquals(Status.DisplayedUnit, u))
            //{
            //    // ユニット選択中だけ
            //    if (Commands.CommandState == "ユニット選択")
            //    {
            //        Status.DisplayUnitStatus(ref u);
            //    }
            //}
        }

        private void UpdateList()
        {
            SelectedItemNum = ListBoxItems.Count(x => x.ListItemFlag);
            lblNumber.Text = $"{SelectedItemNum}/{MaxListItem}";

            ListBoxItems.ResetBindings();

            if (SelectedItemNum > 0 && SelectedItemNum <= MaxListItem)
            {
                if (!cmdFinish.Enabled)
                {
                    cmdFinish.Enabled = true;
                }
            }
            else if (cmdFinish.Enabled)
            {
                cmdFinish.Enabled = false;
            }
        }
    }
    class MultiSelectListBoxItem
    {
        private ListBoxItem _item;

        public string ListItemID => _item.ListItemID;
        public bool ListItemFlag { get => _item.ListItemFlag; set => _item.ListItemFlag = value; }
        public string Text => (_item.ListItemFlag ? "○" : "　") + _item.Text;

        public MultiSelectListBoxItem(ListBoxItem item)
        {
            _item = item;
        }
    }
}
