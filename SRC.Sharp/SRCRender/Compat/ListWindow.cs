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

        /// <summary>複数列で表示するか（項目は列ごとに上から下へ並ぶ）</summary>
        public bool MultiColumn { get; set; }
        public int ColumnWidth { get; set; }
        public List<string> Items { get; } = new List<string>();
        public int SelectedIndex { get; set; } = -1;

        private int topIndex;

        /// <summary>先頭に表示している項目（0 始まり）。複数列では列の先頭の項目。</summary>
        public int TopIndex
        {
            get => topIndex;
            set
            {
                var max = Math.Max(0, Items.Count - VisibleRows);
                if (MultiColumn)
                {
                    var rows = Rows;
                    var columns = (Items.Count + rows - 1) / rows;
                    max = Math.Max(0, columns - VisibleColumns) * rows;
                    value = value / rows * rows;
                }
                topIndex = Math.Max(0, Math.Min(value, max));
            }
        }

        public const int ScrollBarWidth = 16;

        /// <summary>1 列の行数</summary>
        public int Rows => Math.Max(1, (Height - 4 - (MultiColumn && HasScrollBar ? ScrollBarWidth : 0)) / ItemHeight);

        /// <summary>表示できる列数</summary>
        public int VisibleColumns => MultiColumn ? Math.Max(1, (Width - 4) / ColumnWidth) : 1;

        /// <summary>一度に表示できる項目数</summary>
        public int VisibleRows => MultiColumn ? Rows * VisibleColumns : Math.Max(1, (Height - 4) / ItemHeight);

        /// <summary>スクロールバーを表示するか（複数列では横、それ以外は縦）</summary>
        public bool HasScrollBar => MultiColumn
            ? Items.Count > Math.Max(1, (Height - 4) / ItemHeight) * VisibleColumns
            : Items.Count > VisibleRows;

        /// <summary>項目を表示する領域の幅</summary>
        public int ItemsWidth => Width - 4 - (!MultiColumn && HasScrollBar ? ScrollBarWidth : 0);

        /// <summary>項目の位置（ListBox 内の座標）。表示範囲外なら null。</summary>
        public RectangleF? ItemBounds(int index)
        {
            var offset = index - TopIndex;
            if (offset < 0 || offset >= VisibleRows || index >= Items.Count)
            {
                return null;
            }
            if (MultiColumn)
            {
                return new RectangleF(2 + offset / Rows * ColumnWidth, 2 + offset % Rows * ItemHeight, ColumnWidth, ItemHeight);
            }
            return new RectangleF(2, 2 + offset * ItemHeight, ItemsWidth, ItemHeight);
        }

        /// <summary>ListBox 内の座標にある項目。無ければ -1。</summary>
        public int IndexFromPoint(float x, float y)
        {
            for (var index = TopIndex; index < Math.Min(Items.Count, TopIndex + VisibleRows); index++)
            {
                var rect = ItemBounds(index);
                if (rect.HasValue && rect.Value.Contains(x, y))
                {
                    return index;
                }
            }
            return -1;
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
                TopIndex = MultiColumn ? index - VisibleRows + Rows : index - VisibleRows + 1;
            }
        }

        /// <summary>1 ページ（複数列では 1 列）単位のスクロール。</summary>
        public void Scroll(int pages)
        {
            TopIndex += pages * (MultiColumn ? Rows : VisibleRows);
        }
    }

    /// <summary>
    /// ボタン相当。
    /// </summary>
    public class Button : Control
    {
        public string Text { get; set; } = "";
        public Font Font { get; set; }
        public bool Enabled { get; set; } = true;
    }

    /// <summary>
    /// 画面上のウィンドウ（タイトルバー付き）。
    /// </summary>
    public abstract class Form : Control
    {
        /// <summary>タイトルバーの高さ</summary>
        public const int TitleBarHeight = 20;

        protected Form()
        {
            Visible = false;
            BackColor = Color.FromArgb(192, 192, 192);
        }

        /// <summary>タイトルバーの文字列</summary>
        public string Text { get; set; }

        /// <summary>画面上の左上（タイトルバーを含む）</summary>
        public int ScreenLeft { get; set; }
        public int ScreenTop { get; set; }

        /// <summary>タイトルバーを含む高さ</summary>
        public int OuterHeight => Height + TitleBarHeight;

        /// <summary>項目のリスト</summary>
        public abstract ListBoxControl lstItems { get; }

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

        /// <summary>画面の左端・縦位置を決める（下寄せまたは中央）。</summary>
        public void PlaceOnScreen(int screenWidth, int screenHeight, bool left, bool bottom)
        {
            ScreenLeft = left ? 0 : Math.Max(0, (screenWidth - Width) / 2);
            ScreenTop = bottom ? screenHeight - OuterHeight : Math.Max(0, (screenHeight - OuterHeight) / 2);
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

    /// <summary>
    /// WinForms 版の多段リストボックス（Forms/MultiColumListBox.Designer.cs）と同じ部品・配置を持つ。
    /// </summary>
    public class MultiColumnListWindow : Form
    {
        private static readonly Font ListFont = new Font("ＭＳ 明朝", 12F, FontStyle.Regular, GraphicsUnit.Point);
        private readonly ListBoxControl items;

        public MultiColumnListWindow()
        {
            Width = 670;
            Height = 446;
            Text = "MultiColumListBox";
            items = new ListBoxControl { Left = 8, Top = 8, Width = 654, Height = 404, Font = ListFont, MultiColumn = true, ColumnWidth = 164 };
            labCaption = new Label { Left = 8, Top = 416, Width = 654, Height = 23, BackColor = Color.White, Font = ListFont };
        }

        public override ListBoxControl lstItems => items;
        public Label labCaption { get; }
    }

    /// <summary>
    /// WinForms 版の出撃ユニット選択用リストボックス（Forms/MultiSelectListBox.Designer.cs）と同じ部品・配置を持つ。
    /// </summary>
    public class MultiSelectListWindow : Form
    {
        private static readonly Font ListFont = new Font("ＭＳ 明朝", 12F, FontStyle.Regular, GraphicsUnit.Point);
        private static readonly Font NumberFont = new Font("ＭＳ 明朝", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
        private static readonly Font ButtonFont = new Font("ＭＳ Ｐゴシック", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
        private static readonly Font BoldButtonFont = new Font("ＭＳ Ｐゴシック", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
        private readonly ListBoxControl items;

        public MultiSelectListWindow()
        {
            Width = 479;
            Height = 331;
            Text = "MultiSelectListBox";
            lblCaption = new Label { Left = 0, Top = 7, Width = 479, Height = 22, BackColor = Color.White, Font = ListFont };
            items = new ListBoxControl { Left = 0, Top = 32, Width = 479, Height = 228, Font = ListFont };
            cmdSelectAll = new Button { Left = 3, Top = 264, Width = 153, Height = 29, Text = "先頭から選択", Font = ButtonFont };
            cmdSelectAll2 = new Button { Left = 163, Top = 264, Width = 161, Height = 29, Text = "最後から選択", Font = ButtonFont };
            cmdSort = new Button { Left = 331, Top = 264, Width = 145, Height = 29, Text = "名称順に並べ替え", Font = ButtonFont };
            cmdResume = new Button { Left = 91, Top = 296, Width = 137, Height = 29, Text = "マップを見る", Font = ButtonFont };
            cmdFinish = new Button { Left = 259, Top = 296, Width = 137, Height = 29, Text = "終了", Font = BoldButtonFont };
            lblNumber = new Label { Left = 417, Top = 297, Width = 57, Height = 25, Font = NumberFont };
        }

        public override ListBoxControl lstItems => items;
        public Label lblCaption { get; }
        public Button cmdSelectAll { get; }
        public Button cmdSelectAll2 { get; }
        public Button cmdSort { get; }
        public Button cmdResume { get; }
        public Button cmdFinish { get; }
        public Label lblNumber { get; }

        public IEnumerable<Button> Buttons => new[] { cmdSelectAll, cmdSelectAll2, cmdSort, cmdResume, cmdFinish };
    }

    /// <summary>
    /// WinForms 版のリストボックス（Forms/ListBox.Designer.cs）と同じ部品・配置を持つ。
    /// ScreenRenderer がこの内容を描画する。
    /// </summary>
    public class ListWindow : Form
    {
        private static readonly Color Gray = Color.FromArgb(192, 192, 192);
        private static readonly Color Red = Color.FromArgb(192, 0, 0);
        private static readonly Color Green = Color.FromArgb(0, 192, 0);
        private static readonly Font ListFont = new Font("ＭＳ 明朝", 12F, FontStyle.Regular, GraphicsUnit.Point);
        private static readonly Font CommentFont = new Font("ＭＳ ゴシック", 12F, FontStyle.Regular, GraphicsUnit.Point);
        private static readonly Font LabelFont = new Font("ＭＳ Ｐ明朝", 12F, FontStyle.Bold, GraphicsUnit.Point);
        private static readonly Font SmallLabelFont = new Font("ＭＳ Ｐ明朝", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
        private static readonly Font ValueFont = new Font("ＭＳ 明朝", 9.75F, FontStyle.Bold, GraphicsUnit.Point);

        private readonly ListBoxControl items;

        public ListWindow()
        {
            Width = 654;
            Height = 137;
            BackColor = Gray;
            Text = "ListBox";
            HorizontalSize = "M";
            VerticalSize = "M";

            labCaption = new Label { Left = 6, Top = 5, Width = 643, Height = 23, BackColor = Color.White, Font = ListFont };
            items = new ListBoxControl { Left = 6, Top = 32, Width = 643, Height = 100, Font = ListFont };
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

        /// <summary>リストボックスのサイズ (通常:M 大型:L)。幅は S / M。</summary>
        public string HorizontalSize { get; set; }
        public string VerticalSize { get; set; }

        /// <summary>コメント欄を使っているか（WinForms 版の txtComment.Enabled）</summary>
        public bool CommentEnabled { get; set; }

        public Label labCaption { get; }
        public override ListBoxControl lstItems => items;
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
    }
}
