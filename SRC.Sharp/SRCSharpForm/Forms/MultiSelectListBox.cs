// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore;
using SRCCore.Lib;
using System;
using System.Collections.Generic;
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

        private List<MultiSelectListBoxItem> ListBoxItemSource;
        private BindingList<MultiSelectListBoxItem> ListBoxItems;

        public void Init(SRC src, ListBoxArgs args, int max_num)
        {
            SRC = src;
            Text = args.lb_caption;
            lblCaption.Text = "　" + args.lb_info;
            cmdSort.Text = "名称順に並べ替え";

            ListBoxItemSource = args.Items.Select(x => new MultiSelectListBoxItem(x)).ToList();
            ListBoxItems = new BindingList<MultiSelectListBoxItem>(ListBoxItemSource);
            MaxListItem = max_num;

            _lstItems.DataSource = ListBoxItems;
            _lstItems.DisplayMember = "Text";
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
            UpdateList();
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
            _lstItems.TopIndex = 0;
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
            _lstItems.TopIndex = GeneralLib.MaxLng(_lstItems.Items.Count - 14, 0);
        }

        // 「～順」ボタンをクリック
        private void cmdSort_Click(object eventSender, EventArgs eventArgs)
        {
            if (cmdSort.Text == "レベル順に並べ替え")
            {
                // メインパイロットのレベル順に並べ替え
                ListBoxItemSource.Sort((x, y) => SRC.UList.Item(x.ListItemID).MainPilot().Level
                    .CompareTo(SRC.UList.Item(y.ListItemID).MainPilot().Level));
                ListBoxItems.ResetBindings();
                // 並べ替え方法をトグルで切り替え
                cmdSort.Text = "名称順に並べ替え";
            }
            else
            {
                // ユニットの名称順に並べ替え
                ListBoxItemSource.Sort((x, y) => -SRC.UList.Item(x.ListItemID).KanaName
                    .CompareTo(SRC.UList.Item(y.ListItemID).KanaName));
                ListBoxItems.ResetBindings();
                // 並べ替え方法をトグルで切り替え
                cmdSort.Text = "レベル順に並べ替え";
            }
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
        private void _lstItems_DoubleClick(object eventSender, EventArgs eventArgs)
        {
            var item = _lstItems.SelectedItem as MultiSelectListBoxItem;
            if (item != null)
            {
                var preIndex = _lstItems.TopIndex;
                item.ListItemFlag = !item.ListItemFlag;
                UpdateList(item);
                _lstItems.TopIndex = preIndex;
            }
        }

        // リストボックス上でマウスボタンを押す
        private void _lstItems_MouseDown(object eventSender, MouseEventArgs eventArgs)
        {
            // 左クリック以外は無視
            if (ResolveMouseButton(eventArgs) != GuiButton.Left)
            {
                return;
            }

            var item = _lstItems.SelectedItem as MultiSelectListBoxItem;
            if (item != null)
            {
                var preIndex = _lstItems.TopIndex;
                item.ListItemFlag = !item.ListItemFlag;
                UpdateList(item);
                _lstItems.TopIndex = preIndex;
            }
        }

        private static GuiButton ResolveMouseButton(MouseEventArgs eventArgs)
        {
            // 右でなければ左とみなす
            return eventArgs.Button.HasFlag(MouseButtons.Right)
                ? GuiButton.Right
                : GuiButton.Left;
        }

        // リストボックス上でマウスカーソルを移動
        private void _lstItems_MouseMove(object sender, MouseEventArgs e)
        {
            // カーソルがあるアイテムを算出
            var point = _lstItems.PointToClient(Cursor.Position);
            int index = _lstItems.IndexFromPoint(point);
            if (index >= 0 && index != _lstItems.SelectedIndex)
            {
                // カーソルがあるアイテムをハイライト表示
                _lstItems.SelectedIndex = index;
                _lstItems.Update();

                // ユニット選択中だけ
                if (SRC.Commands.CommandState == "ユニット選択")
                {
                    SRC.GUIStatus.DisplayUnitStatus(
                        SRC.UList.Item((_lstItems.SelectedItem as MultiSelectListBoxItem)?.ListItemID));
                }
            }
        }

        private void UpdateList(MultiSelectListBoxItem item = null)
        {
            SelectedItemNum = ListBoxItems.Count(x => x.ListItemFlag);
            lblNumber.Text = $"{SelectedItemNum}/{MaxListItem}";

            if (item == null)
            {
                ListBoxItems.ResetBindings();
            }
            else
            {
                ListBoxItems.ResetItem(ListBoxItems.IndexOf(item));
            }

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
