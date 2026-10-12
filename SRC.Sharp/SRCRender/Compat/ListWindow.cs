using System;
using System.Collections.Generic;
using System.Drawing;

namespace SRCRender.Compat
{
    /// <summary>
    /// ListBox 相当（項目・先頭行・選択行）。描画は ScreenRenderer が行う。
    /// </summary>
    public class ListBoxControl : Control
    {
        public ListBoxControl()
        {
            BackColor = Color.White;
        }

        public Font Font { get; set; }
        public int ItemHeight { get; set; } = 16;
        public List<string> Items { get; } = new List<string>();
        public int SelectedIndex { get; set; } = -1;

        private int topIndex;

        /// <summary>先頭に表示している項目（0 始まり）</summary>
        public int TopIndex
        {
            get => topIndex;
            set => topIndex = Math.Max(0, Math.Min(value, Math.Max(0, Items.Count - VisibleRows)));
        }

        /// <summary>一度に表示できる行数</summary>
        public int VisibleRows => Math.Max(1, (Height - 4) / ItemHeight);

        /// <summary>スクロールバーを表示するか</summary>
        public bool HasScrollBar => Items.Count > VisibleRows;

        public const int ScrollBarWidth = 16;

        /// <summary>ListBox 内の座標にある項目。無ければ -1。</summary>
        public int IndexFromPoint(float x, float y)
        {
            if (x < 0 || y < 0 || x >= Width - (HasScrollBar ? ScrollBarWidth : 0) || y >= Height)
            {
                return -1;
            }
            // 枠（2px）の内側に項目を並べる
            var index = TopIndex + (int)(Math.Max(0, y - 2) / ItemHeight);
            return index < Items.Count ? index : -1;
        }

        /// <summary>選択行が見えるように先頭行を調整する。</summary>
        public void EnsureVisible(int index)
        {
            if (index < 0)
            {
                return;
            }
            if (index < TopIndex)
            {
                TopIndex = index;
            }
            else if (index >= TopIndex + VisibleRows)
            {
                TopIndex = index - VisibleRows + 1;
            }
        }
    }

    /// <summary>
    /// WinForms 版のリストボックス（Forms/ListBox.Designer.cs）と同じ部品・配置を持つ。
    /// ScreenRenderer がこの内容を描画する。
    /// </summary>
    public class ListWindow : Control
    {
        private static readonly Color Gray = Color.FromArgb(192, 192, 192);
        private static readonly Color Red = Color.FromArgb(192, 0, 0);
        private static readonly Color Green = Color.FromArgb(0, 192, 0);
        private static readonly Font ListFont = new Font("ＭＳ 明朝", 12F, FontStyle.Regular, GraphicsUnit.Point);
        private static readonly Font CommentFont = new Font("ＭＳ ゴシック", 12F, FontStyle.Regular, GraphicsUnit.Point);
        private static readonly Font LabelFont = new Font("ＭＳ Ｐ明朝", 12F, FontStyle.Bold, GraphicsUnit.Point);
        private static readonly Font SmallLabelFont = new Font("ＭＳ Ｐ明朝", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
        private static readonly Font ValueFont = new Font("ＭＳ 明朝", 9.75F, FontStyle.Bold, GraphicsUnit.Point);

        /// <summary>タイトルバーの高さ</summary>
        public const int TitleBarHeight = 20;

        public ListWindow()
        {
            Visible = false;
            Width = 654;
            Height = 137;
            BackColor = Gray;
            Text = "ListBox";
            HorizontalSize = "M";
            VerticalSize = "M";

            labCaption = new Label { Left = 6, Top = 5, Width = 643, Height = 23, BackColor = Color.White, Font = ListFont };
            lstItems = new ListBoxControl { Left = 6, Top = 32, Width = 643, Height = 100, Font = ListFont };
            txtComment = new Label { Left = 6, Top = 141, Width = 637, Height = 38, BackColor = Color.White, Font = CommentFont, Visible = false };
            picBar = new PictureBox { Left = 6, Top = 123, Width = 643, Height = 13, BackColor = Color.White, Visible = false };

            imgPilot1 = new PictureBox { Left = 6, Top = 3, Width = 36, Height = 36, StretchImage = true, Visible = false };
            labLevel1 = new Label { Left = 45, Top = 4, Width = 20, Height = 17, Text = "Lv", Font = LabelFont, Visible = false };
            txtLevel1 = new Label { Left = 66, Top = 6, Width = 17, Height = 13, Text = "99", Font = ValueFont, Visible = false };
            labMorale1 = new Label { Left = 46, Top = 20, Width = 12, Height = 17, Text = "M", Font = SmallLabelFont, Visible = false };
            txtMorale1 = new Label { Left = 59, Top = 22, Width = 25, Height = 13, Text = "100", Font = ValueFont, Visible = false };
            picUnit1 = new PictureBox { Left = 85, Top = 4, Width = 32, Height = 32, BackColor = Color.White, Visible = false };
            labHP1 = new Label { Left = 121, Top = 6, Width = 23, Height = 17, Text = "HP", Font = LabelFont, Visible = false };
            txtHP1 = new Label { Left = 148, Top = 8, Width = 88, Height = 13, Text = "99999/99999", Font = ValueFont, Visible = false };
            picHP1 = new PictureBox { Left = 122, Top = 26, Width = 114, Height = 8, BackColor = Red, ForeColor = Green, Visible = false };
            labEN1 = new Label { Left = 238, Top = 6, Width = 22, Height = 17, Text = "EN", Font = LabelFont, Visible = false };
            txtEN1 = new Label { Left = 263, Top = 8, Width = 57, Height = 13, Text = "999/999", Font = ValueFont, Visible = false };
            picEN1 = new PictureBox { Left = 240, Top = 26, Width = 79, Height = 8, BackColor = Red, ForeColor = Green, Visible = false };

            imgPilot2 = new PictureBox { Left = 331, Top = 3, Width = 36, Height = 36, StretchImage = true, Visible = false };
            labLevel2 = new Label { Left = 370, Top = 4, Width = 20, Height = 17, Text = "Lv", Font = LabelFont, Visible = false };
            txtLevel2 = new Label { Left = 391, Top = 7, Width = 17, Height = 13, Text = "99", Font = ValueFont, Visible = false };
            labMorale2 = new Label { Left = 372, Top = 22, Width = 12, Height = 17, Text = "M", Font = SmallLabelFont, Visible = false };
            txtMorale2 = new Label { Left = 385, Top = 23, Width = 25, Height = 13, Text = "100", Font = ValueFont, Visible = false };
            picUnit2 = new PictureBox { Left = 412, Top = 5, Width = 32, Height = 32, BackColor = Color.White, Visible = false };
            labHP2 = new Label { Left = 448, Top = 6, Width = 25, Height = 17, Text = "HP", Font = LabelFont, Visible = false };
            txtHP2 = new Label { Left = 473, Top = 8, Width = 88, Height = 13, Text = "99999/99999", Font = ValueFont, Visible = false };
            picHP2 = new PictureBox { Left = 449, Top = 27, Width = 112, Height = 8, BackColor = Red, ForeColor = Green, Visible = false };
            labEN2 = new Label { Left = 563, Top = 6, Width = 25, Height = 17, Text = "EN", Font = LabelFont, Visible = false };
            txtEN2 = new Label { Left = 587, Top = 8, Width = 57, Height = 13, Text = "999/999", Font = ValueFont, Visible = false };
            picEN2 = new PictureBox { Left = 565, Top = 27, Width = 78, Height = 8, BackColor = Red, ForeColor = Green, Visible = false };
        }

        /// <summary>タイトルバーの文字列</summary>
        public string Text { get; set; }

        /// <summary>リストボックスのサイズ (通常:M 大型:L)。幅は S / M。</summary>
        public string HorizontalSize { get; set; }
        public string VerticalSize { get; set; }

        /// <summary>コメント欄を使っているか（WinForms 版の txtComment.Enabled）</summary>
        public bool CommentEnabled { get; set; }

        /// <summary>画面上の左上（タイトルバーを含む）</summary>
        public int ScreenLeft { get; set; }
        public int ScreenTop { get; set; }

        /// <summary>タイトルバーを含む高さ</summary>
        public int OuterHeight => Height + TitleBarHeight;

        public Label labCaption { get; }
        public ListBoxControl lstItems { get; }
        public Label txtComment { get; }
        public PictureBox picBar { get; }

        public PictureBox imgPilot1 { get; }
        public Label labLevel1 { get; }
        public Label txtLevel1 { get; }
        public Label labMorale1 { get; }
        public Label txtMorale1 { get; }
        public PictureBox picUnit1 { get; }
        public Label labHP1 { get; }
        public Label txtHP1 { get; }
        public PictureBox picHP1 { get; }
        public Label labEN1 { get; }
        public Label txtEN1 { get; }
        public PictureBox picEN1 { get; }

        public PictureBox imgPilot2 { get; }
        public Label labLevel2 { get; }
        public Label txtLevel2 { get; }
        public Label labMorale2 { get; }
        public Label txtMorale2 { get; }
        public PictureBox picUnit2 { get; }
        public Label labHP2 { get; }
        public Label txtHP2 { get; }
        public PictureBox picHP2 { get; }
        public Label labEN2 { get; }
        public Label txtEN2 { get; }
        public PictureBox picEN2 { get; }

        /// <summary>ユニット表示部品（AddPartsToListBox で表示する）</summary>
        public IEnumerable<Control> UnitParts => new Control[]
        {
            imgPilot1, labLevel1, txtLevel1, labMorale1, txtMorale1, picUnit1, labHP1, txtHP1, picHP1, labEN1, txtEN1, picEN1,
            imgPilot2, labLevel2, txtLevel2, labMorale2, txtMorale2, picUnit2, labHP2, txtHP2, picHP2, labEN2, txtEN2, picEN2,
        };

        public IEnumerable<PictureBox> Pictures => new[]
        {
            picBar, imgPilot1, picUnit1, picHP1, picEN1, imgPilot2, picUnit2, picHP2, picEN2,
        };

        /// <summary>背景を塗らない文字の部品</summary>
        public IEnumerable<Label> TextLabels => new[]
        {
            labLevel1, txtLevel1, labMorale1, txtMorale1, labHP1, txtHP1, labEN1, txtEN1,
            labLevel2, txtLevel2, labMorale2, txtMorale2, labHP2, txtHP2, labEN2, txtEN2,
        };

        /// <summary>画面座標をウィンドウのクライアント座標にする。</summary>
        public PointF ToClient(float x, float y)
        {
            return new PointF(x - ScreenLeft, y - ScreenTop - TitleBarHeight);
        }

        /// <summary>画面座標がウィンドウ（タイトルバーを含む）の中か。</summary>
        public bool Contains(float x, float y)
        {
            return x >= ScreenLeft && y >= ScreenTop && x < ScreenLeft + Width && y < ScreenTop + OuterHeight;
        }

        public static bool HitTest(Control control, PointF client)
        {
            return control.Visible
                && client.X >= control.Left && client.Y >= control.Top
                && client.X < control.Left + control.Width && client.Y < control.Top + control.Height;
        }

        public void Show()
        {
            Visible = true;
            Refresh();
        }

        public void Hide()
        {
            Visible = false;
            Refresh();
        }
    }
}
