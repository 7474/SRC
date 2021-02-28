using System;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Project1
{
    internal partial class frmListBox : Form
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // リストボックスのフォーム

        // リストボックスのサイズ (通常:M 大型:L)
        // 幅
        public string HorizontalSize;
        // 高さ
        public string VerticalSize;

        // Questionコマンド用変数
        public short CurrentTime;
        public short TimeLimit;

        // 最後に選択されたアイテム
        private short LastSelectedItem;

        // リストボックスへのキー入力
        private void frmListBox_KeyDown(object eventSender, KeyEventArgs eventArgs)
        {
            short KeyCode = (short)eventArgs.KeyCode;
            short Shift = (short)((int)eventArgs.KeyData / 0x10000);
            // 既にウィンドウが隠れている場合は無視
            if (!Visible)
            {
                return;
            }

            switch (KeyCode)
            {
                case (short)Keys.Up:
                    {
                        break;
                    }

                case (short)Keys.Down:
                    {
                        break;
                    }

                case (short)Keys.Left:
                case (short)Keys.Right:
                    {
                        break;
                    }

                case (short)Keys.Escape:
                case (short)Keys.Delete:
                case (short)Keys.Back:
                    {
                        // キャンセル
                        Commands.SelectedItem = 0;
                        LastSelectedItem = Commands.SelectedItem;
                        GUI.TopItem = (short)lstItems.TopIndex;
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

                        if (Information.UBound(GUI.ListItemFlag) >= lstItems.SelectedIndex + 1)
                        {
                            if (GUI.ListItemFlag[lstItems.SelectedIndex + 1])
                            {
                                return;
                            }
                        }

                        Commands.SelectedItem = (short)(lstItems.SelectedIndex + 1);
                        LastSelectedItem = Commands.SelectedItem;
                        GUI.TopItem = (short)(lstItems.TopIndex + 1);
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
            int ret;

            // リストボックスを常に手前に表示
            ret = GUI.SetWindowPos(Handle.ToInt32(), -1, 0, 0, 0, 0, 0x3);
            HorizontalSize = "M";
            VerticalSize = "M";
        }

        // リストボックスを閉じる
        private void frmListBox_FormClosed(object eventSender, FormClosedEventArgs eventArgs)
        {
            GUI.TopItem = (short)(lstItems.TopIndex + 1);
            GUI.IsFormClicked = true;
            if (!GUI.IsMordal & Visible)
            {
                // UPGRADE_ISSUE: Event パラメータ Cancel はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FB723E3C-1C06-4D2B-B083-E6CD0D334DA8"' をクリックしてください。
                Cancel = 1;
            }

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

            if (Information.UBound(GUI.ListItemFlag) >= lstItems.SelectedIndex + 1)
            {
                if (GUI.ListItemFlag[lstItems.SelectedIndex + 1])
                {
                    return;
                }
            }

            if (LastSelectedItem != 0)
            {
                // 連続で選択
                if (!Visible)
                {
                    return;
                }

                Commands.SelectedItem = (short)(lstItems.SelectedIndex + 1);
                LastSelectedItem = Commands.SelectedItem;
                GUI.TopItem = (short)(lstItems.TopIndex + 1);
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
                GUI.TopItem = (short)(lstItems.TopIndex + 1);
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
            short Button = (short)((int)eventArgs.Button / 0x100000);
            short Shift = (short)((int)ModifierKeys / 0x10000);
            float X = (float)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(eventArgs.X);
            float Y = (float)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(eventArgs.Y);
            switch (Button)
            {
                case 1:
                    {
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

                        if (Information.UBound(GUI.ListItemFlag) >= lstItems.SelectedIndex + 1)
                        {
                            if (GUI.ListItemFlag[lstItems.SelectedIndex + 1])
                            {
                                return;
                            }
                        }

                        Commands.SelectedItem = (short)(lstItems.SelectedIndex + 1);
                        LastSelectedItem = Commands.SelectedItem;
                        GUI.TopItem = (short)(lstItems.TopIndex + 1);
                        GUI.IsFormClicked = true;
                        if (CurrentTime > 0)
                        {
                            Timer1.Enabled = false;
                            Hide();
                        }

                        break;
                    }

                case 2:
                    {
                        // キャンセル
                        Commands.SelectedItem = 0;
                        LastSelectedItem = Commands.SelectedItem;
                        GUI.TopItem = (short)(lstItems.TopIndex + 1);
                        GUI.IsFormClicked = true;
                        break;
                    }
            }
        }

        // キャプション部分をクリック
        private void labCaption_MouseDown(object eventSender, MouseEventArgs eventArgs)
        {
            short Button = (short)((int)eventArgs.Button / 0x100000);
            short Shift = (short)((int)ModifierKeys / 0x10000);
            float X = (float)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(eventArgs.X);
            float Y = (float)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(eventArgs.Y);
            switch (Button)
            {
                case 1:
                    {
                        // ユニットステータスを表示しているユニットを入れ替え
                        if (GUI.MainForm.Visible)
                        {
                            if (Status.DisplayedUnit is object & Commands.SelectedUnit is object & Commands.SelectedTarget is object)
                            {
                                if ((Status.DisplayedUnit.ID ?? "") == (Commands.SelectedUnit.ID ?? ""))
                                {
                                    Status.DisplayUnitStatus(ref Commands.SelectedTarget);
                                }
                                else
                                {
                                    Status.DisplayUnitStatus(ref Commands.SelectedUnit);
                                }
                            }
                        }

                        break;
                    }

                case 2:
                    {
                        // キャンセル
                        Commands.SelectedItem = 0;
                        LastSelectedItem = Commands.SelectedItem;
                        GUI.TopItem = (short)lstItems.TopIndex;
                        if (GUI.IsFormClicked)
                        {
                            Hide();
                        }

                        GUI.IsFormClicked = true;
                        break;
                    }
            }
        }

        // リストボックスの端をクリック
        private void frmListBox_MouseDown(object eventSender, MouseEventArgs eventArgs)
        {
            short Button = (short)((int)eventArgs.Button / 0x100000);
            short Shift = (short)((int)ModifierKeys / 0x10000);
            float X = eventArgs.X;
            float Y = eventArgs.Y;
            if (Button == 2)
            {
                // キャンセル
                Commands.SelectedItem = 0;
                LastSelectedItem = Commands.SelectedItem;
                GUI.TopItem = (short)lstItems.TopIndex;
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
            short Button = (short)((int)eventArgs.Button / 0x100000);
            short Shift = (short)((int)ModifierKeys / 0x10000);
            float X = (float)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(eventArgs.X);
            float Y = (float)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(eventArgs.Y);
            short itm;

            // カーソルがあるアイテムを算出
            itm = (short)(((long)(Y * ClientRectangle.Width) / (long)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(Width) + 1L) / 16L);
            itm = (short)(itm + lstItems.TopIndex);

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
                txtComment.Text = GUI.ListItemComment[itm + 1];
            }
        }

        // Questionコマンド対応
        private void Timer1_Tick(object eventSender, EventArgs eventArgs)
        {
            CurrentTime = (short)(CurrentTime + 1);
            // UPGRADE_ISSUE: PictureBox メソッド picBar.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
            picBar.Cls();
            // UPGRADE_ISSUE: PictureBox メソッド picBar.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
            picBar.Line(0, 0); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            picBar.Refresh();
            if (CurrentTime >= TimeLimit)
            {
                Commands.SelectedItem = 0;
                LastSelectedItem = Commands.SelectedItem;
                GUI.TopItem = (short)lstItems.TopIndex;
                Hide();
                Timer1.Enabled = false;
            }
        }

        // 選択されているアイテムに対応するユニットのステータス表示
        private void Timer2_Tick(object eventSender, EventArgs eventArgs)
        {
            Unit u;
            if (!Visible | !GUI.MainForm.Visible)
            {
                return;
            }

            if (lstItems.SelectedIndex == -1)
            {
                return;
            };
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 10644


            Input:

                    On Error GoTo ErrorHandler

             */
            if (lstItems.SelectedIndex >= Information.UBound(GUI.ListItemID))
            {
                return;
            }

            // 選択されたユニットが存在する？
            bool localIsDefined2() { var tmp = GUI.ListItemID; object argIndex1 = tmp[lstItems.SelectedIndex + 1]; var ret = SRC.UList.IsDefined2(ref argIndex1); return ret; }

            if (!localIsDefined2())
            {
                return;
            }

            var tmp = GUI.ListItemID;
            object argIndex1 = tmp[lstItems.SelectedIndex + 1];
            u = SRC.UList.Item2(ref argIndex1);

            // 選択されたユニットにパイロットが乗っている？
            if (u.CountPilot() == 0)
            {
                return;
            }

            // 既に表示している？
            if (ReferenceEquals(Status.DisplayedUnit, u))
            {
                return;
            }

            // 選択されたユニットをステータスウィンドウに表示
            Status.DisplayUnitStatus(ref u);
            ErrorHandler:
            ;
        }
    }
}