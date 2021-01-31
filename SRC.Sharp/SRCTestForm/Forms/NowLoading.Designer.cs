using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    [DesignerGenerated()]
    internal partial class frmNowLoading
    {
        /* TODO ERROR: Skipped RegionDirectiveTrivia */
        [DebuggerNonUserCode()]
        public frmNowLoading() : base()
        {
            // この呼び出しは、Windows フォーム デザイナで必要です。
            InitializeComponent();
        }
        // Form は、コンポーネント一覧に後処理を実行するために dispose をオーバーライドします。
        [DebuggerNonUserCode()]
        protected override void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                if (components is object)
                {
                    components.Dispose();
                }
            }

            base.Dispose(Disposing);
        }
        // Windows フォーム デザイナで必要です。
        private System.ComponentModel.IContainer components;
        public ToolTip ToolTip1;
        public PictureBox picBar;
        public Label Label1;
        // メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
        // Windows フォーム デザイナを使って変更できます。
        // コード エディタを使用して、変更しないでください。
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            var resources = new System.Resources.ResourceManager(typeof(frmNowLoading));
            components = new System.ComponentModel.Container();
            ToolTip1 = new ToolTip(components);
            picBar = new PictureBox();
            Label1 = new Label();
            SuspendLayout();
            ToolTip1.Active = true;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.FromArgb(192, 192, 192);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Text = "SRC";
            ClientSize = new Size(214, 88);
            Location = new Point(76, 107);
            ForeColor = Color.Black;
            Icon = (Icon)resources.GetObject("frmNowLoading.Icon");
            MaximizeBox = false;
            AutoScaleMode = AutoScaleMode.Font;
            ControlBox = true;
            Enabled = true;
            KeyPreview = false;
            MinimizeBox = true;
            Cursor = Cursors.Default;
            RightToLeft = RightToLeft.No;
            ShowInTaskbar = true;
            HelpButton = false;
            WindowState = FormWindowState.Normal;
            Name = "frmNowLoading";
            picBar.BackColor = Color.White;
            picBar.ForeColor = Color.FromArgb(0, 0, 128);
            picBar.Size = new Size(183, 13);
            picBar.Location = new Point(16, 56);
            picBar.TabIndex = 1;
            picBar.Dock = DockStyle.None;
            picBar.CausesValidation = true;
            picBar.Enabled = true;
            picBar.Cursor = Cursors.Default;
            picBar.RightToLeft = RightToLeft.No;
            picBar.TabStop = true;
            picBar.Visible = true;
            picBar.SizeMode = PictureBoxSizeMode.Normal;
            picBar.BorderStyle = BorderStyle.Fixed3D;
            picBar.Name = "picBar";
            Label1.BackColor = Color.FromArgb(192, 192, 192);
            Label1.Text = "Now Loading ...";
            Label1.Font = new Font("Times New Roman", 18f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            Label1.ForeColor = Color.Black;
            Label1.Size = new Size(169, 33);
            Label1.Location = new Point(24, 16);
            Label1.TabIndex = 0;
            Label1.TextAlign = ContentAlignment.TopLeft;
            Label1.Enabled = true;
            Label1.Cursor = Cursors.Default;
            Label1.RightToLeft = RightToLeft.No;
            Label1.UseMnemonic = true;
            Label1.Visible = true;
            Label1.AutoSize = false;
            Label1.BorderStyle = BorderStyle.None;
            Label1.Name = "Label1";
            Controls.Add(picBar);
            Controls.Add(Label1);
            ResumeLayout(false);
            PerformLayout();
        }
        /* TODO ERROR: Skipped EndRegionDirectiveTrivia */
    }
}