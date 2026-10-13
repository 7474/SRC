using System.Drawing;

namespace SRCRender.Compat
{
    /// <summary>
    /// WinForms 版のメッセージウィンドウ（Forms/Message.Designer.cs）と同じ部品・配置を持つ。
    /// ScreenRenderer がこの内容を描画する。
    /// </summary>
    public class MessageWindow : Control
    {
        private static readonly Color Gray = Color.FromArgb(192, 192, 192);
        private static readonly Color Red = Color.FromArgb(192, 0, 0);
        private static readonly Color Green = Color.FromArgb(0, 192, 0);
        private static readonly Font LabelFont = new Font("ＭＳ Ｐ明朝", 12F, FontStyle.Bold, GraphicsUnit.Point);
        private static readonly Font ValueFont = new Font("ＭＳ 明朝", 9.75F, FontStyle.Bold, GraphicsUnit.Point);

        public MessageWindow()
        {
            Visible = false;
            Width = 508;
            Height = 118;
            BackColor = Gray;
            Text = "メッセージ";

            picFace = new PictureBox { Left = 8, Top = 43, Width = 68, Height = 68, BackColor = Gray };
            picMessage = new PictureBox { Left = 84, Top = 42, Width = 417, Height = 70, BackColor = Color.White, ForeColor = Color.Black };
            picUnit1 = new PictureBox { Left = 8, Top = 4, Width = 32, Height = 32, BackColor = Color.White };
            picUnit2 = new PictureBox { Left = 260, Top = 5, Width = 32, Height = 32, BackColor = Color.White };
            picHP1 = new PictureBox { Left = 45, Top = 28, Width = 116, Height = 8, BackColor = Red, ForeColor = Green };
            picEN1 = new PictureBox { Left = 166, Top = 28, Width = 83, Height = 8, BackColor = Red, ForeColor = Green };
            picHP2 = new PictureBox { Left = 297, Top = 28, Width = 116, Height = 8, BackColor = Red, ForeColor = Green };
            picEN2 = new PictureBox { Left = 418, Top = 28, Width = 82, Height = 8, BackColor = Red, ForeColor = Green };
            labHP1 = new Label { Left = 44, Top = 8, Width = 22, Height = 17, Text = "HP", Font = LabelFont };
            labEN1 = new Label { Left = 165, Top = 8, Width = 22, Height = 17, Text = "EN", Font = LabelFont };
            labHP2 = new Label { Left = 296, Top = 8, Width = 22, Height = 17, Text = "HP", Font = LabelFont };
            labEN2 = new Label { Left = 417, Top = 8, Width = 22, Height = 17, Text = "EN", Font = LabelFont };
            txtHP1 = new Label { Left = 72, Top = 10, Width = 88, Height = 13, Text = "99999/99999", Font = ValueFont };
            txtEN1 = new Label { Left = 192, Top = 10, Width = 57, Height = 13, Text = "999/999", Font = ValueFont };
            txtHP2 = new Label { Left = 323, Top = 10, Width = 88, Height = 13, Text = "99999/99999", Font = ValueFont };
            txtEN2 = new Label { Left = 443, Top = 10, Width = 57, Height = 13, Text = "999/999", Font = ValueFont };
        }

        public string Text { get; set; }

        /// <summary>
        /// 部品の Refresh、表示・非表示の切り替えで呼ぶ処理を設定する。
        /// </summary>
        public void SetRefreshHandler(System.Action handler)
        {
            Refreshed = handler;
            foreach (var control in new Control[] { picFace, picMessage, picUnit1, picUnit2, picHP1, picEN1, picHP2, picEN2, labHP1, labEN1, labHP2, labEN2, txtHP1, txtEN1, txtHP2, txtEN2 })
            {
                control.Refreshed = handler;
            }
        }

        public Size ClientRectangle => new Size(Width, Height);

        public PictureBox picFace { get; }
        public PictureBox picMessage { get; }
        public PictureBox picUnit1 { get; }
        public PictureBox picUnit2 { get; }
        public PictureBox picHP1 { get; }
        public PictureBox picEN1 { get; }
        public PictureBox picHP2 { get; }
        public PictureBox picEN2 { get; }
        public Label labHP1 { get; }
        public Label labEN1 { get; }
        public Label labHP2 { get; }
        public Label labEN2 { get; }
        public Label txtHP1 { get; }
        public Label txtEN1 { get; }
        public Label txtHP2 { get; }
        public Label txtEN2 { get; }

        public bool IsAutoMode => Text == "メッセージ (自動送り)";

        public void ClearForm()
        {
            picFace.Image = null;
            picMessage.Image = null;
        }

        public void SetMessageModeCaption(bool isAuto)
        {
            Text = isAuto ? "メッセージ (自動送り)" : "メッセージ";
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
