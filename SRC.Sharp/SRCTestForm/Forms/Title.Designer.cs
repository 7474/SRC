using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    [DesignerGenerated()]
    internal partial class frmTitle
    {
        /* TODO ERROR: Skipped RegionDirectiveTrivia */
        [DebuggerNonUserCode()]
        public frmTitle() : base()
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
        public PictureBox Picture1;
        public PictureBox Image1;
        public Label labAuthor;
        public Label labVersion;
        public GroupBox Frame1;
        public Label labLicense;
        // メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
        // Windows フォーム デザイナを使って変更できます。
        // コード エディタを使用して、変更しないでください。
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            var resources = new System.Resources.ResourceManager(typeof(frmTitle));
            components = new System.ComponentModel.Container();
            ToolTip1 = new ToolTip(components);
            Picture1 = new PictureBox();
            Frame1 = new GroupBox();
            Image1 = new PictureBox();
            labAuthor = new Label();
            labVersion = new Label();
            labLicense = new Label();
            Frame1.SuspendLayout();
            SuspendLayout();
            ToolTip1.Active = true;
            StartPosition = FormStartPosition.Manual;
            FormBorderStyle = FormBorderStyle.None;
            Text = "SRC";
            ClientSize = new Size(386, 233);
            Location = new Point(180, 197);
            Icon = (Icon)resources.GetObject("frmTitle.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ControlBox = true;
            Enabled = true;
            KeyPreview = false;
            Cursor = Cursors.Default;
            RightToLeft = RightToLeft.No;
            HelpButton = false;
            WindowState = FormWindowState.Normal;
            Name = "frmTitle";
            Picture1.Size = new Size(200, 40);
            Picture1.Location = new Point(168, 88);
            Picture1.Image = (Image)resources.GetObject("Picture1.Image");
            Picture1.TabIndex = 0;
            Picture1.Dock = DockStyle.None;
            Picture1.BackColor = SystemColors.Control;
            Picture1.CausesValidation = true;
            Picture1.Enabled = true;
            Picture1.ForeColor = SystemColors.ControlText;
            Picture1.Cursor = Cursors.Default;
            Picture1.RightToLeft = RightToLeft.No;
            Picture1.TabStop = true;
            Picture1.Visible = true;
            Picture1.SizeMode = PictureBoxSizeMode.AutoSize;
            Picture1.BorderStyle = BorderStyle.None;
            Picture1.Name = "Picture1";
            Frame1.Size = new Size(337, 201);
            Frame1.Location = new Point(24, 8);
            Frame1.TabIndex = 1;
            Frame1.BackColor = SystemColors.Control;
            Frame1.Enabled = true;
            Frame1.ForeColor = SystemColors.ControlText;
            Frame1.RightToLeft = RightToLeft.No;
            Frame1.Visible = true;
            Frame1.Padding = new Padding(0);
            Frame1.Name = "Frame1";
            Image1.Size = new Size(96, 96);
            Image1.Location = new Point(16, 56);
            Image1.Image = (Image)resources.GetObject("Image1.Image");
            Image1.SizeMode = PictureBoxSizeMode.StretchImage;
            Image1.Enabled = true;
            Image1.Cursor = Cursors.Default;
            Image1.Visible = true;
            Image1.BorderStyle = BorderStyle.None;
            Image1.Name = "Image1";
            labAuthor.TextAlign = ContentAlignment.TopRight;
            labAuthor.Text = "Kei Sakamoto / Inui Tetsuyuki";
            labAuthor.Font = new Font("Times New Roman", 11.25f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            labAuthor.Size = new Size(193, 17);
            labAuthor.Location = new Point(128, 168);
            labAuthor.TabIndex = 3;
            labAuthor.BackColor = SystemColors.Control;
            labAuthor.Enabled = true;
            labAuthor.ForeColor = SystemColors.ControlText;
            labAuthor.Cursor = Cursors.Default;
            labAuthor.RightToLeft = RightToLeft.No;
            labAuthor.UseMnemonic = true;
            labAuthor.Visible = true;
            labAuthor.AutoSize = false;
            labAuthor.BorderStyle = BorderStyle.None;
            labAuthor.Name = "labAuthor";
            labVersion.TextAlign = ContentAlignment.TopRight;
            labVersion.Text = "Ver 1.7.*";
            labVersion.Font = new Font("Times New Roman", 15.75f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(0));
            labVersion.Size = new Size(177, 25);
            labVersion.Location = new Point(144, 136);
            labVersion.TabIndex = 2;
            labVersion.BackColor = SystemColors.Control;
            labVersion.Enabled = true;
            labVersion.ForeColor = SystemColors.ControlText;
            labVersion.Cursor = Cursors.Default;
            labVersion.RightToLeft = RightToLeft.No;
            labVersion.UseMnemonic = true;
            labVersion.Visible = true;
            labVersion.AutoSize = false;
            labVersion.BorderStyle = BorderStyle.None;
            labVersion.Name = "labVersion";
            labLicense.TextAlign = ContentAlignment.TopCenter;
            labLicense.Text = "This program is distributed under the terms of GPL";
            labLicense.Size = new Size(369, 17);
            labLicense.Location = new Point(8, 216);
            labLicense.TabIndex = 4;
            labLicense.BackColor = SystemColors.Control;
            labLicense.Enabled = true;
            labLicense.ForeColor = SystemColors.ControlText;
            labLicense.Cursor = Cursors.Default;
            labLicense.RightToLeft = RightToLeft.No;
            labLicense.UseMnemonic = true;
            labLicense.Visible = true;
            labLicense.AutoSize = false;
            labLicense.BorderStyle = BorderStyle.None;
            labLicense.Name = "labLicense";
            Controls.Add(Picture1);
            Controls.Add(Frame1);
            Controls.Add(labLicense);
            Frame1.Controls.Add(Image1);
            Frame1.Controls.Add(labAuthor);
            Frame1.Controls.Add(labVersion);
            Frame1.ResumeLayout(false);
            Load += new EventHandler(frmTitle_Load);
            ResumeLayout(false);
            PerformLayout();
        }
        /* TODO ERROR: Skipped EndRegionDirectiveTrivia */
    }
}