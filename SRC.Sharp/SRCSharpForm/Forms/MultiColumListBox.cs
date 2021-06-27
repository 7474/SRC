// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore;
using SRCCore.Lib;
using System;
using System.Windows.Forms;

namespace SRCSharpForm
{
    // 多段のリストボックスのフォーム
    internal partial class frmMultiColumnListBox : Form
    {
        private SRCCore.SRC SRC { get; set; }

        public void Init(SRC src, ListBoxArgs args)
        {
            SRC = src;
            Text = args.lb_caption;

            _lstItems.DataSource = args.Items;
            _lstItems.DisplayMember = "TextWithFlag";

            // 先頭に表示するアイテムを設定
            if (SRC.GUI.TopItem > 0)
            {
                if (_lstItems.TopIndex != SRC.GUI.TopItem - 1)
                {
                    _lstItems.TopIndex = GeneralLib.MinLng(SRC.GUI.TopItem, _lstItems.Items.Count) - 1;
                }
            }
        }

        // フォームを表示
        private void frmMultiColumnListBox_Activated(object eventSender, EventArgs eventArgs)
        {
            SRC.Commands.SelectedItem = 0;
            labCaption.Text = "";
        }

        // フォームをロード
        private void frmMultiColumnListBox_Load(object eventSender, EventArgs eventArgs)
        {
            // 常に手前に表示
            TopMost = true;
        }

        // フォーム上でマウスボタンを押す
        private void lstItems_MouseDown(object eventSender, MouseEventArgs eventArgs)
        {
            switch (ResolveMouseButton(eventArgs))
            {

                case GuiButton.Left:
                    {
                        // 選択
                        if (!Visible)
                        {
                            return;
                        }

                        if (_lstItems.SelectedIndex < 0 || ((_lstItems.SelectedItem as ListBoxItem)?.ListItemFlag ?? true))
                        {
                            return;
                        }

                        SRC.Commands.SelectedItem = _lstItems.SelectedIndex + 1;
                        SRC.GUI.TopItem = _lstItems.TopIndex + 1;
                        if (SRC.GUI.IsFormClicked)
                        {
                            Hide();
                        }

                        SRC.GUI.IsFormClicked = true;
                        break;
                    }

                default:
                    {
                        // キャンセル
                        SRC.Commands.SelectedItem = 0;
                        SRC.GUI.TopItem = _lstItems.TopIndex + 1;
                        if (SRC.GUI.IsFormClicked)
                        {
                            Hide();
                        }

                        SRC.GUI.IsFormClicked = true;
                        break;
                    }
            }
        }

        // フォームを閉じる
        private void frmMultiColumnListBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            SRC.GUI.TopItem = _lstItems.TopIndex + 1;
            SRC.GUI.IsFormClicked = true;
            if (!SRC.GUI.IsMordal && Visible)
            {
                e.Cancel = true;
            }

            Hide();
        }

        // フォーム上でマウスボタンを押す
        private void frmMultiColumnListBox_MouseDown(object eventSender, MouseEventArgs eventArgs)
        {
            if (ResolveMouseButton(eventArgs) == GuiButton.Right)
            {
                // キャンセルのみ受け付け
                SRC.Commands.SelectedItem = 0;
                SRC.GUI.TopItem = _lstItems.TopIndex;
                if (SRC.GUI.IsFormClicked)
                {
                    Hide();
                }

                SRC.GUI.IsFormClicked = true;
            }
        }

        // リストボックス上でマウスカーソルを移動
        private void lstItems_MouseMove(object eventSender, MouseEventArgs eventArgs)
        {
            // カーソルがあるアイテムを算出
            var point = _lstItems.PointToClient(Cursor.Position);
            int index = _lstItems.IndexFromPoint(point);
            if (index >= 0 && index != _lstItems.SelectedIndex)
            {
                // カーソルがあるアイテムをハイライト表示
                _lstItems.SelectedIndex = index;
                _lstItems.Update();

                // 解説の表示
                labCaption.Text = (_lstItems.SelectedItem as ListBoxItem)?.ListItemComment;
            }
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
