// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore;
using SRCCore.Commands;
using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using SRCRender.Compat;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCRender
{
    // WinForms 版のリストボックス（Forms/ListBox.cs）の移植。
    // 表示・選択待ちはゲームスレッドで行い、入力は UI スレッドからゲームスレッドに渡して処理する。
    public partial class ScreenGUI
    {
        private readonly ListWindow frmListBox = new ListWindow();
        private volatile bool waitingList;
        private IList<UiCommand> menuCommands;
        private bool isUnitCommandMenu;

        // リストボックスの状態
        private IList<ListBoxItem> listBoxItems;
        private bool listHasFlag;
        private int lastSelectedItem;
        // Questionコマンド用（経過時間と制限時間、ミリ秒）
        private int listCurrentTime;
        private int listTimeLimit;

        /// <summary>リストボックス</summary>
        public ListWindow ListWindow => frmListBox;

        private ListBoxItem SelectedListItem => frmListBox.lstItems.SelectedIndex >= 0 && listBoxItems != null && frmListBox.lstItems.SelectedIndex < listBoxItems.Count
            ? listBoxItems[frmListBox.lstItems.SelectedIndex]
            : null;

        public int ListBox(ListBoxArgs args)
        {
            ShowItems(args);
            return Commands.SelectedItem;
        }

        public void CloseListBox()
        {
            if (frmListBox.Visible)
            {
                HideListBox();
                if (frmListBox.CommentEnabled)
                {
                    frmListBox.txtComment.Visible = false;
                    frmListBox.CommentEnabled = false;
                    frmListBox.Height -= 40;
                }
            }
        }

        private void HideListBox()
        {
            waitingList = false;
            State.List = null;
            frmListBox.Hide();
        }

        // リストボックスの高さを大きくする
        public void EnlargeListBoxHeight()
        {
            switch (frmListBox.VerticalSize ?? "")
            {
                case "M":
                    frmListBox.Height += 160;
                    frmListBox.lstItems.Height = 260;
                    frmListBox.VerticalSize = "L";
                    break;
            }
        }

        // リストボックスの高さを小さくする
        public void ReduceListBoxHeight()
        {
            switch (frmListBox.VerticalSize ?? "")
            {
                case "L":
                    frmListBox.Height -= 160;
                    frmListBox.lstItems.Height = 100;
                    frmListBox.VerticalSize = "M";
                    break;
            }
        }

        // リストボックスの幅を大きくする
        public void EnlargeListBoxWidth()
        {
            switch (frmListBox.HorizontalSize ?? "")
            {
                case "S":
                    frmListBox.Width += 157;
                    frmListBox.lstItems.Width = 643;
                    frmListBox.labCaption.Width = 643;
                    frmListBox.HorizontalSize = "M";
                    break;
            }
        }

        // リストボックスの幅を小さくする
        public void ReduceListBoxWidth()
        {
            switch (frmListBox.HorizontalSize ?? "")
            {
                case "M":
                    frmListBox.Width -= 157;
                    frmListBox.lstItems.Width = 486;
                    frmListBox.labCaption.Width = 486;
                    frmListBox.HorizontalSize = "S";
                    break;
            }
        }

        public void AddPartsToListBox()
        {
            var u = Commands.SelectedUnit;
            var t = Commands.SelectedTarget;
            var w = frmListBox;
            // リストボックスにユニットやＨＰのゲージを追加
            w.Height += 35;
            w.labCaption.Top = 42;
            w.lstItems.Top = 69;
            foreach (var part in w.UnitParts)
            {
                part.Visible = true;
            }

            DisplayListBoxUnit(u, t, w.imgPilot1, w.txtLevel1, w.txtMorale1, w.picUnit1, w.labHP1, w.txtHP1, w.picHP1, w.labEN1, w.txtEN1, w.picEN1);
            DisplayListBoxUnit(t, t, w.imgPilot2, w.txtLevel2, w.txtMorale2, w.picUnit2, w.labHP2, w.txtHP2, w.picHP2, w.labEN2, w.txtEN2, w.picEN2);
        }

        private void DisplayListBoxUnit(Unit u, Unit t, PictureBox imgPilot, Label txtLevel, Label txtMorale, PictureBox picUnit,
            Label labHP, Label txtHP, PictureBox picHP, Label labEN, Label txtEN, PictureBox picEN)
        {
            if (u == null)
            {
                return;
            }
            imgPilot.Image = Images.Get(SRC.FileSystem.PathCombine("Pilot", u.MainPilot().get_Bitmap(false)));

            txtLevel.Text = SrcFormatter.Format(u.MainPilot().Level);
            txtMorale.Text = SrcFormatter.Format(u.MainPilot().Morale);

            picUnit.NewImageIfNull();
            using (var g = Graphics.FromImage(picUnit.Image))
            {
                g.Clear(System.Drawing.Color.White);
                DrawUnit(g.Canvas, Map.CellAtPoint(u.x, u.y), u, SkiaSharp.SKRect.Create(0, 0, picUnit.Width, picUnit.Height));
            }

            if (u.IsConditionSatisfied("データ不明"))
            {
                labHP.Text = SRC.Expression.Term("HP", u: null);
                txtHP.Text = "?????/?????";
            }
            else
            {
                labHP.Text = SRC.Expression.Term("HP", u);
                txtHP.Text = (u.HP < 100000 ? SrcFormatter.Format(u.HP) : "?????")
                    + "/" + (u.MaxHP < 100000 ? SrcFormatter.Format(u.MaxHP) : "?????");
            }
            picHP.DrawBar((float)u.HP / u.MaxHP, BarBackBrush, BarForeBrush);

            if (u.IsConditionSatisfied("データ不明"))
            {
                labEN.Text = SRC.Expression.Term("EN", u: null);
                txtEN.Text = "???/???";
            }
            else
            {
                labEN.Text = SRC.Expression.Term("EN", t);
                txtEN.Text = (u.EN < 1000 ? SrcFormatter.Format(u.EN) : "???")
                    + "/" + (u.MaxEN < 1000 ? SrcFormatter.Format(u.MaxEN) : "???");
            }
            picEN.DrawBar((float)u.EN / u.MaxEN, BarBackBrush, BarForeBrush);
        }

        public void RemovePartsOnListBox()
        {
            var w = frmListBox;
            w.Height -= 35;
            w.labCaption.Top = 5;
            w.lstItems.Top = 32;
            foreach (var part in w.UnitParts)
            {
                part.Visible = false;
            }
        }

        private void ShowItems(ListBoxArgs args)
        {
            var w = frmListBox;
            listBoxItems = args.Items;
            listHasFlag = args.HasFlag;
            var list = args.Items;
            var lb_caption = args.lb_caption;
            var lb_info = args.lb_info;
            var lb_mode = args.lb_mode ?? "";

            // コメントウィンドウの処理
            if (Strings.InStr(lb_mode, "コメント") > 0)
            {
                if (!w.CommentEnabled)
                {
                    w.CommentEnabled = true;
                    w.txtComment.Visible = true;
                    w.txtComment.Width = w.labCaption.Width;
                    w.txtComment.Text = "";
                    w.txtComment.Top = w.lstItems.Top + w.lstItems.Height + 5;
                    w.Height += 40;
                }
            }
            else if (w.CommentEnabled)
            {
                w.CommentEnabled = false;
                w.txtComment.Visible = false;
                w.Height -= 40;
            }

            // キャプション
            w.Text = lb_caption;
            w.labCaption.Text = listHasFlag ? "  " + lb_info : lb_info;

            // リストボックスにアイテムを追加
            w.lstItems.Items.Clear();
            w.lstItems.Items.AddRange(listHasFlag
                ? list.Select(x => x.ListItemFlag ? $"×{x.Text}" : $"  {x.Text}")
                : list.Select(x => x.Text));
            w.lstItems.SelectedIndex = -1;
            w.lstItems.TopIndex = 0;

            // 先頭のアイテムを設定
            if (TopItem > 0)
            {
                w.lstItems.TopIndex = GeneralLib.MaxLng(GeneralLib.MinLng(TopItem - 1, w.lstItems.Items.Count - 1), 0);
                w.lstItems.SelectedIndex = Math.Min(w.lstItems.Items.Count, TopItem) - 1;
            }
            else if (w.lstItems.Items.Count > 0)
            {
                w.lstItems.SelectedIndex = 0;
            }

            // コメントウィンドウの表示
            if (w.CommentEnabled)
            {
                w.txtComment.Text = SelectedListItem?.ListItemComment;
            }

            // 表示位置を設定（メイン画面を基準にする）
            w.ScreenLeft = w.HorizontalSize == "S" ? 0 : Math.Max(0, (ScreenLayout.Width - w.Width) / 2);
            w.ScreenTop = w.VerticalSize == "M" && Strings.InStr(lb_mode, "中央表示") == 0
                ? ScreenLayout.Height - w.OuterHeight
                : Math.Max(0, (ScreenLayout.Height - w.OuterHeight) / 2);

            Commands.SelectedItem = 0;
            IsFormClicked = false;

            var listState = new ListState(w, list, listHasFlag);
            // リストボックスを表示
            if (Strings.InStr(lb_mode, "表示のみ") > 0)
            {
                // 表示のみを行う
                IsMordal = false;
                waitingList = false;
                State.List = listState;
                w.Show();
                return;
            }

            // 選択待ちの状態にしてから表示する（表示直後のクリックを取りこぼさないため）
            IsMordal = false;
            waitingList = true;
            State.List = listState;
            w.Show();
            if (Strings.InStr(lb_mode, "カーソル移動") > 0 && SRC.AutoMoveCursor)
            {
                MoveCursorPos("ダイアログ");
            }

            var start = Environment.TickCount;
            while (!IsFormClicked)
            {
                Sleep(WaitIntervalMillis);
                // Questionコマンドの制限時間
                if (listTimeLimit > 0)
                {
                    listCurrentTime = Environment.TickCount - start;
                    UpdateTimeLimitBar();
                    if (listCurrentTime >= listTimeLimit)
                    {
                        CancelListBox();
                    }
                }
            }
            waitingList = false;

            if (Strings.InStr(lb_mode, "連続表示") > 0)
            {
                // 選択が行われてもリストボックスを閉じない
                Present();
                return;
            }

            // 選択が行われた時点でリストボックスを閉じる
            HideListBox();
            if (Strings.InStr(lb_mode, "カーソル移動") > 0
                && Strings.InStr(lb_mode, "カーソル移動(行きのみ)") == 0
                && SRC.AutoMoveCursor)
            {
                RestoreCursorPos();
            }

            if (w.CommentEnabled)
            {
                w.CommentEnabled = false;
                w.txtComment.Visible = false;
                w.Height -= 40;
            }
        }

        private void UpdateTimeLimitBar()
        {
            var bar = frmListBox.picBar;
            bar.ClearImage(new SolidBrush(bar.BackColor));
            var barWidth = (int)((float)bar.Width * listCurrentTime / listTimeLimit);
            if (barWidth > 0)
            {
                using (var g = Graphics.FromImage(bar.Image))
                {
                    g.FillRectangle(BarForeBrush, 0, 0, Math.Min(barWidth, bar.Width), bar.Height);
                }
            }
            Present();
        }

        #region リストボックスの入力（ゲームスレッドで処理する）

        // 選択
        private void SelectListItem()
        {
            var lstItems = frmListBox.lstItems;
            if (!frmListBox.Visible || lstItems.SelectedIndex < 0 || SelectedListItem == null)
            {
                return;
            }
            if (listHasFlag && SelectedListItem.ListItemFlag)
            {
                return;
            }
            Commands.SelectedItem = lstItems.SelectedIndex + 1;
            lastSelectedItem = Commands.SelectedItem;
            TopItem = lstItems.TopIndex + 1;
            IsFormClicked = true;
        }

        // キャンセル
        private void CancelListBox()
        {
            Commands.SelectedItem = 0;
            lastSelectedItem = Commands.SelectedItem;
            TopItem = frmListBox.lstItems.TopIndex + 1;
            IsFormClicked = true;
        }

        private void ListBoxPointerPressed(float x, float y, GuiButton button)
        {
            var w = frmListBox;
            var client = w.ToClient(x, y);
            if (button == GuiButton.Right)
            {
                CancelListBox();
                return;
            }
            if (ListWindow.HitTest(w.lstItems, client))
            {
                var lx = client.X - w.lstItems.Left;
                var ly = client.Y - w.lstItems.Top;
                if (w.lstItems.HasScrollBar && lx >= w.lstItems.Width - ListBoxControl.ScrollBarWidth)
                {
                    // スクロールバー（上半分で上へ、下半分で下へ 1 ページ）
                    w.lstItems.TopIndex += ly < w.lstItems.Height / 2 ? -w.lstItems.VisibleRows : w.lstItems.VisibleRows;
                    Present();
                    return;
                }
                var index = w.lstItems.IndexFromPoint(lx, ly);
                if (index >= 0)
                {
                    w.lstItems.SelectedIndex = index;
                    SelectListItem();
                }
                return;
            }
            if (ListWindow.HitTest(w.labCaption, client))
            {
                // ユニットステータスを表示しているユニットを入れ替え
                var su = Commands.SelectedUnit;
                var st = Commands.SelectedTarget;
                if (DisplayedUnit != null && su != null && st != null)
                {
                    DisplayUnitStatus((DisplayedUnit.ID ?? "") == (su.ID ?? "") ? st : su);
                }
            }
        }

        private void ListBoxPointerMoved(float x, float y)
        {
            var w = frmListBox;
            var client = w.ToClient(x, y);
            var index = w.lstItems.IndexFromPoint(client.X - w.lstItems.Left, client.Y - w.lstItems.Top);
            if (index < 0 || index == w.lstItems.SelectedIndex)
            {
                return;
            }
            w.lstItems.SelectedIndex = index;
            OnListSelectionChanged();
        }

        private void ListBoxWheel(int delta)
        {
            var lstItems = frmListBox.lstItems;
            lstItems.TopIndex -= Math.Sign(delta) * 3;
            Present();
        }

        private void ListBoxKeyPressed(ScreenKey key)
        {
            var lstItems = frmListBox.lstItems;
            switch (key)
            {
                case ScreenKey.Up:
                case ScreenKey.Down:
                    if (lstItems.Items.Count == 0)
                    {
                        return;
                    }
                    lstItems.SelectedIndex = Math.Max(0, Math.Min(lstItems.Items.Count - 1,
                        lstItems.SelectedIndex + (key == ScreenKey.Down ? 1 : -1)));
                    lstItems.EnsureVisible(lstItems.SelectedIndex);
                    OnListSelectionChanged();
                    break;

                case ScreenKey.Escape:
                    CancelListBox();
                    break;

                case ScreenKey.Enter:
                    SelectListItem();
                    break;
            }
        }

        private void OnListSelectionChanged()
        {
            var w = frmListBox;
            // コメント欄を更新
            if (w.CommentEnabled)
            {
                w.txtComment.Text = SelectedListItem?.ListItemComment;
            }
            // 選択されているアイテムに対応するユニットのステータス表示
            var id = SelectedListItem?.ListItemID;
            if (!string.IsNullOrEmpty(id) && MainFormVisible && SRC.UList.IsDefined2(id))
            {
                var u = SRC.UList.Item2(id);
                if (u.CountPilot() > 0 && !ReferenceEquals(DisplayedUnit, u))
                {
                    DisplayUnitStatus(u);
                }
            }
            Present();
        }

        #endregion

        public UnitWeapon WeaponListBox(Unit u, UnitWeaponList weapons, string caption_msg, string lb_mode, string BGM = "")
        {
            TopItem = -1;
            var list = weapons.Items.Select(x => new ListBoxItem
            {
                Text = $"{GeneralLib.RightPaddedString(x.Weapon.Name, 28)}" +
                    $" {GeneralLib.LeftPaddedString(x.Weapon.WeaponPower("") + "", 4)}" +
                    $" {GeneralLib.LeftPaddedString(x.Weapon.WeaponPrecision() + "", 3)}" +
                    $" {GeneralLib.LeftPaddedString(x.Weapon.WeaponCritical() + "", 3)}" +
                    $" {GeneralLib.LeftPaddedString(x.Weapon.Bullet() + "", 3)}" +
                    $" {GeneralLib.LeftPaddedString(x.Weapon.WeaponENConsumption() + "", 4)}" +
                    $" {x.Weapon.WeaponData.Adaption}" +
                    $" {x.Weapon.WeaponClass()}",
                ListItemComment = "",
                ListItemFlag = !x.CanUse,
                ListItemID = "",
            }).ToList();
            var ret = ListBox(new ListBoxArgs
            {
                Items = list,
                HasFlag = true,
                lb_caption = caption_msg,
                lb_info = "名称                         攻撃 命中 CT   弾  EN 適応 分類",
                lb_mode = "",
            });
            return ret > 0 ? weapons.Items[ret - 1].Weapon : null;
        }

        public UnitAbility AbilityListBox(Unit u, UnitAbilityList abilities, string caption_msg, string lb_mode, bool is_item = false)
        {
            TopItem = -1;
            var list = abilities.Items.Select(x => new ListBoxItem
            {
                Text = $"{x.Ability.Data.Name}",
                ListItemComment = "",
                ListItemFlag = !x.CanUse,
                ListItemID = "",
            }).ToList();
            if (list.Count == 1 && lb_mode != "一覧" && !is_item)
            {
                return abilities.Items[0].Ability;
            }
            var ret = ListBox(new ListBoxArgs
            {
                Items = list,
                HasFlag = true,
                lb_caption = caption_msg,
                lb_info = "名称",
                lb_mode = "",
            });
            return ret > 0 ? abilities.Items[ret - 1].Ability : null;
        }

        public int LIPS(ListBoxArgs args, int time_limit)
        {
            listCurrentTime = 0;
            listTimeLimit = Math.Max(0, time_limit) * 1000;
            frmListBox.picBar.Visible = listTimeLimit > 0;
            frmListBox.picBar.Image = null;
            try
            {
                ShowItems(args);
                return Commands.SelectedItem;
            }
            finally
            {
                listTimeLimit = 0;
                frmListBox.picBar.Visible = false;
            }
        }

        public int MultiColumnListBox(ListBoxArgs args, bool is_center)
        {
            return ListBox(args);
        }

        public int MultiSelectListBox(ListBoxArgs args, int max_num)
        {
            // TODO 複数選択。現状は1件のみ選択できる
            var ret = ListBox(args);
            if (ret > 0)
            {
                args.Items[ret - 1].ListItemFlag = true;
                return 1;
            }
            return 0;
        }

        public void ShowUnitCommandMenu(IList<UiCommand> commands)
        {
            ShowMenu(commands, true);
        }

        public void ShowMapCommandMenu(IList<UiCommand> commands)
        {
            ShowMenu(commands, false);
        }

        private void ShowMenu(IList<UiCommand> commands, bool isUnit)
        {
            menuCommands = commands;
            isUnitCommandMenu = isUnit;
            State.Menu = new MenuState
            {
                Items = commands.Select(x => x.Label).ToList(),
                Checked = commands.Select(x => x.IsChecked).ToList(),
                X = (int)MouseX,
                Y = (int)MouseY + 6,
            };
            Present();
        }
    }
}
