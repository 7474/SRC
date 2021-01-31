using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace SRCTestForm
{
    [DesignerGenerated()]
    internal partial class frmMessage
    {
        /* TODO ERROR: Skipped RegionDirectiveTrivia */
        [DebuggerNonUserCode()]
        public frmMessage() : base()
        {
            // この呼び出しは、Windows フォーム デザイナで必要です。
            InitializeComponent();
            _picFace.Name = "picFace";
            _picMessage.Name = "picMessage";
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
        private PictureBox _picFace;

        public PictureBox picFace
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _picFace;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_picFace != null)
                {
                    _picFace.MouseDown -= picFace_MouseDown;
                }

                _picFace = value;
                if (_picFace != null)
                {
                    _picFace.MouseDown += picFace_MouseDown;
                }
            }
        }

        public PictureBox picUnit1;
        public PictureBox picUnit2;
        public TextBox txtHP2;
        public PictureBox picHP2;
        public PictureBox picEN2;
        public TextBox txtEN2;
        public TextBox txtEN1;
        public PictureBox picEN1;
        public PictureBox picHP1;
        public TextBox txtHP1;
        private PictureBox _picMessage;

        public PictureBox picMessage
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _picMessage;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_picMessage != null)
                {
                    _picMessage.DoubleClick -= picMessage_DoubleClick;
                    _picMessage.MouseDown -= picMessage_MouseDown;
                }

                _picMessage = value;
                if (_picMessage != null)
                {
                    _picMessage.DoubleClick += picMessage_DoubleClick;
                    _picMessage.MouseDown += picMessage_MouseDown;
                }
            }
        }

        public Label labHP2;
        public Label labEN2;
        public Label labEN1;
        public Label labHP1;
        // メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
        // Windows フォーム デザイナを使って変更できます。
        // コード エディタを使用して、変更しないでください。
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            var resources = new System.Resources.ResourceManager(typeof(frmMessage));
            components = new System.ComponentModel.Container();
            ToolTip1 = new ToolTip(components);
            _picFace = new PictureBox();
            _picFace.MouseDown += new MouseEventHandler(picFace_MouseDown);
            picUnit1 = new PictureBox();
            picUnit2 = new PictureBox();
            txtHP2 = new TextBox();
            picHP2 = new PictureBox();
            picEN2 = new PictureBox();
            txtEN2 = new TextBox();
            txtEN1 = new TextBox();
            picEN1 = new PictureBox();
            picHP1 = new PictureBox();
            txtHP1 = new TextBox();
            _picMessage = new PictureBox();
            _picMessage.DoubleClick += new EventHandler(picMessage_DoubleClick);
            _picMessage.MouseDown += new MouseEventHandler(picMessage_MouseDown);
            labHP2 = new Label();
            labEN2 = new Label();
            labEN1 = new Label();
            labHP1 = new Label();
            SuspendLayout();
            ToolTip1.Active = true;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.FromArgb(192, 192, 192);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Text = "メッセージ";
            ClientSize = new Size(508, 118);
            Location = new Point(93, 101);
            Font = new Font("ＭＳ Ｐ明朝", 12f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            ForeColor = Color.Black;
            Icon = (Icon)resources.GetObject("frmMessage.Icon");
            KeyPreview = true;
            MaximizeBox = false;
            AutoScaleMode = AutoScaleMode.Font;
            ControlBox = true;
            Enabled = true;
            MinimizeBox = true;
            Cursor = Cursors.Default;
            RightToLeft = RightToLeft.No;
            ShowInTaskbar = true;
            HelpButton = false;
            WindowState = FormWindowState.Normal;
            Name = "frmMessage";
            _picFace.BackColor = Color.FromArgb(192, 192, 192);
            _picFace.Size = new Size(68, 68);
            _picFace.Location = new Point(8, 43);
            _picFace.TabIndex = 15;
            _picFace.Dock = DockStyle.None;
            _picFace.CausesValidation = true;
            _picFace.Enabled = true;
            _picFace.ForeColor = SystemColors.ControlText;
            _picFace.Cursor = Cursors.Default;
            _picFace.RightToLeft = RightToLeft.No;
            _picFace.TabStop = true;
            _picFace.Visible = true;
            _picFace.SizeMode = PictureBoxSizeMode.Normal;
            _picFace.BorderStyle = BorderStyle.Fixed3D;
            _picFace.Name = "_picFace";
            picUnit1.BackColor = SystemColors.Window;
            picUnit1.ForeColor = SystemColors.WindowText;
            picUnit1.Size = new Size(32, 32);
            picUnit1.Location = new Point(8, 4);
            picUnit1.TabIndex = 14;
            picUnit1.Visible = false;
            picUnit1.Dock = DockStyle.None;
            picUnit1.CausesValidation = true;
            picUnit1.Enabled = true;
            picUnit1.Cursor = Cursors.Default;
            picUnit1.RightToLeft = RightToLeft.No;
            picUnit1.TabStop = true;
            picUnit1.SizeMode = PictureBoxSizeMode.Normal;
            picUnit1.BorderStyle = BorderStyle.None;
            picUnit1.Name = "picUnit1";
            picUnit2.BackColor = SystemColors.Window;
            picUnit2.ForeColor = SystemColors.WindowText;
            picUnit2.Size = new Size(32, 32);
            picUnit2.Location = new Point(260, 5);
            picUnit2.TabIndex = 13;
            picUnit2.Visible = false;
            picUnit2.Dock = DockStyle.None;
            picUnit2.CausesValidation = true;
            picUnit2.Enabled = true;
            picUnit2.Cursor = Cursors.Default;
            picUnit2.RightToLeft = RightToLeft.No;
            picUnit2.TabStop = true;
            picUnit2.SizeMode = PictureBoxSizeMode.Normal;
            picUnit2.BorderStyle = BorderStyle.None;
            picUnit2.Name = "picUnit2";
            txtHP2.AutoSize = false;
            txtHP2.BackColor = Color.FromArgb(192, 192, 192);
            txtHP2.Font = new Font("ＭＳ 明朝", 9.75f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            txtHP2.ForeColor = Color.Black;
            txtHP2.Size = new Size(88, 13);
            txtHP2.Location = new Point(323, 10);
            txtHP2.TabIndex = 10;
            txtHP2.Text = "99999/99999";
            txtHP2.AcceptsReturn = true;
            txtHP2.TextAlign = HorizontalAlignment.Left;
            txtHP2.CausesValidation = true;
            txtHP2.Enabled = true;
            txtHP2.HideSelection = true;
            txtHP2.ReadOnly = false;
            txtHP2.MaxLength = 0;
            txtHP2.Cursor = Cursors.IBeam;
            txtHP2.Multiline = false;
            txtHP2.RightToLeft = RightToLeft.No;
            txtHP2.ScrollBars = ScrollBars.None;
            txtHP2.TabStop = true;
            txtHP2.Visible = true;
            txtHP2.BorderStyle = BorderStyle.None;
            txtHP2.Name = "txtHP2";
            picHP2.BackColor = Color.FromArgb(192, 0, 0);
            picHP2.ForeColor = Color.FromArgb(0, 192, 0);
            picHP2.Size = new Size(116, 8);
            picHP2.Location = new Point(297, 28);
            picHP2.TabIndex = 9;
            picHP2.Dock = DockStyle.None;
            picHP2.CausesValidation = true;
            picHP2.Enabled = true;
            picHP2.Cursor = Cursors.Default;
            picHP2.RightToLeft = RightToLeft.No;
            picHP2.TabStop = true;
            picHP2.Visible = true;
            picHP2.SizeMode = PictureBoxSizeMode.Normal;
            picHP2.BorderStyle = BorderStyle.Fixed3D;
            picHP2.Name = "picHP2";
            picEN2.BackColor = Color.FromArgb(192, 0, 0);
            picEN2.ForeColor = Color.FromArgb(0, 192, 0);
            picEN2.Size = new Size(82, 8);
            picEN2.Location = new Point(418, 28);
            picEN2.TabIndex = 8;
            picEN2.Dock = DockStyle.None;
            picEN2.CausesValidation = true;
            picEN2.Enabled = true;
            picEN2.Cursor = Cursors.Default;
            picEN2.RightToLeft = RightToLeft.No;
            picEN2.TabStop = true;
            picEN2.Visible = true;
            picEN2.SizeMode = PictureBoxSizeMode.Normal;
            picEN2.BorderStyle = BorderStyle.Fixed3D;
            picEN2.Name = "picEN2";
            txtEN2.AutoSize = false;
            txtEN2.BackColor = Color.FromArgb(192, 192, 192);
            txtEN2.Font = new Font("ＭＳ 明朝", 9.75f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            txtEN2.ForeColor = Color.Black;
            txtEN2.Size = new Size(57, 13);
            txtEN2.Location = new Point(443, 10);
            txtEN2.TabIndex = 7;
            txtEN2.Text = "999/999";
            txtEN2.AcceptsReturn = true;
            txtEN2.TextAlign = HorizontalAlignment.Left;
            txtEN2.CausesValidation = true;
            txtEN2.Enabled = true;
            txtEN2.HideSelection = true;
            txtEN2.ReadOnly = false;
            txtEN2.MaxLength = 0;
            txtEN2.Cursor = Cursors.IBeam;
            txtEN2.Multiline = false;
            txtEN2.RightToLeft = RightToLeft.No;
            txtEN2.ScrollBars = ScrollBars.None;
            txtEN2.TabStop = true;
            txtEN2.Visible = true;
            txtEN2.BorderStyle = BorderStyle.None;
            txtEN2.Name = "txtEN2";
            txtEN1.AutoSize = false;
            txtEN1.BackColor = Color.FromArgb(192, 192, 192);
            txtEN1.Font = new Font("ＭＳ 明朝", 9.75f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            txtEN1.ForeColor = Color.Black;
            txtEN1.Size = new Size(57, 13);
            txtEN1.Location = new Point(192, 10);
            txtEN1.TabIndex = 6;
            txtEN1.Text = "999/999";
            txtEN1.AcceptsReturn = true;
            txtEN1.TextAlign = HorizontalAlignment.Left;
            txtEN1.CausesValidation = true;
            txtEN1.Enabled = true;
            txtEN1.HideSelection = true;
            txtEN1.ReadOnly = false;
            txtEN1.MaxLength = 0;
            txtEN1.Cursor = Cursors.IBeam;
            txtEN1.Multiline = false;
            txtEN1.RightToLeft = RightToLeft.No;
            txtEN1.ScrollBars = ScrollBars.None;
            txtEN1.TabStop = true;
            txtEN1.Visible = true;
            txtEN1.BorderStyle = BorderStyle.None;
            txtEN1.Name = "txtEN1";
            picEN1.BackColor = Color.FromArgb(192, 0, 0);
            picEN1.ForeColor = Color.FromArgb(0, 192, 0);
            picEN1.Size = new Size(83, 8);
            picEN1.Location = new Point(166, 28);
            picEN1.TabIndex = 5;
            picEN1.Dock = DockStyle.None;
            picEN1.CausesValidation = true;
            picEN1.Enabled = true;
            picEN1.Cursor = Cursors.Default;
            picEN1.RightToLeft = RightToLeft.No;
            picEN1.TabStop = true;
            picEN1.Visible = true;
            picEN1.SizeMode = PictureBoxSizeMode.Normal;
            picEN1.BorderStyle = BorderStyle.Fixed3D;
            picEN1.Name = "picEN1";
            picHP1.BackColor = Color.FromArgb(192, 0, 0);
            picHP1.ForeColor = Color.FromArgb(0, 192, 0);
            picHP1.Size = new Size(116, 8);
            picHP1.Location = new Point(45, 28);
            picHP1.TabIndex = 3;
            picHP1.Dock = DockStyle.None;
            picHP1.CausesValidation = true;
            picHP1.Enabled = true;
            picHP1.Cursor = Cursors.Default;
            picHP1.RightToLeft = RightToLeft.No;
            picHP1.TabStop = true;
            picHP1.Visible = true;
            picHP1.SizeMode = PictureBoxSizeMode.Normal;
            picHP1.BorderStyle = BorderStyle.Fixed3D;
            picHP1.Name = "picHP1";
            txtHP1.AutoSize = false;
            txtHP1.BackColor = Color.FromArgb(192, 192, 192);
            txtHP1.Font = new Font("ＭＳ 明朝", 9.75f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            txtHP1.ForeColor = Color.Black;
            txtHP1.Size = new Size(88, 13);
            txtHP1.Location = new Point(72, 10);
            txtHP1.TabIndex = 2;
            txtHP1.Text = "99999/99999";
            txtHP1.AcceptsReturn = true;
            txtHP1.TextAlign = HorizontalAlignment.Left;
            txtHP1.CausesValidation = true;
            txtHP1.Enabled = true;
            txtHP1.HideSelection = true;
            txtHP1.ReadOnly = false;
            txtHP1.MaxLength = 0;
            txtHP1.Cursor = Cursors.IBeam;
            txtHP1.Multiline = false;
            txtHP1.RightToLeft = RightToLeft.No;
            txtHP1.ScrollBars = ScrollBars.None;
            txtHP1.TabStop = true;
            txtHP1.Visible = true;
            txtHP1.BorderStyle = BorderStyle.None;
            txtHP1.Name = "txtHP1";
            _picMessage.BackColor = Color.White;
            _picMessage.Font = new Font("ＭＳ Ｐ明朝", 12f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            _picMessage.ForeColor = Color.Black;
            _picMessage.Size = new Size(417, 70);
            _picMessage.Location = new Point(84, 42);
            _picMessage.TabIndex = 0;
            _picMessage.Dock = DockStyle.None;
            _picMessage.CausesValidation = true;
            _picMessage.Enabled = true;
            _picMessage.Cursor = Cursors.Default;
            _picMessage.RightToLeft = RightToLeft.No;
            _picMessage.TabStop = true;
            _picMessage.Visible = true;
            _picMessage.SizeMode = PictureBoxSizeMode.Normal;
            _picMessage.BorderStyle = BorderStyle.Fixed3D;
            _picMessage.Name = "_picMessage";
            labHP2.BackColor = Color.FromArgb(192, 192, 192);
            labHP2.Text = "HP";
            labHP2.ForeColor = Color.Black;
            labHP2.Size = new Size(22, 17);
            labHP2.Location = new Point(296, 8);
            labHP2.TabIndex = 12;
            labHP2.TextAlign = ContentAlignment.TopLeft;
            labHP2.Enabled = true;
            labHP2.Cursor = Cursors.Default;
            labHP2.RightToLeft = RightToLeft.No;
            labHP2.UseMnemonic = true;
            labHP2.Visible = true;
            labHP2.AutoSize = false;
            labHP2.BorderStyle = BorderStyle.None;
            labHP2.Name = "labHP2";
            labEN2.BackColor = Color.FromArgb(192, 192, 192);
            labEN2.Text = "EN";
            labEN2.ForeColor = Color.Black;
            labEN2.Size = new Size(22, 17);
            labEN2.Location = new Point(417, 8);
            labEN2.TabIndex = 11;
            labEN2.TextAlign = ContentAlignment.TopLeft;
            labEN2.Enabled = true;
            labEN2.Cursor = Cursors.Default;
            labEN2.RightToLeft = RightToLeft.No;
            labEN2.UseMnemonic = true;
            labEN2.Visible = true;
            labEN2.AutoSize = false;
            labEN2.BorderStyle = BorderStyle.None;
            labEN2.Name = "labEN2";
            labEN1.BackColor = Color.FromArgb(192, 192, 192);
            labEN1.Text = "EN";
            labEN1.ForeColor = Color.Black;
            labEN1.Size = new Size(22, 17);
            labEN1.Location = new Point(165, 8);
            labEN1.TabIndex = 4;
            labEN1.TextAlign = ContentAlignment.TopLeft;
            labEN1.Enabled = true;
            labEN1.Cursor = Cursors.Default;
            labEN1.RightToLeft = RightToLeft.No;
            labEN1.UseMnemonic = true;
            labEN1.Visible = true;
            labEN1.AutoSize = false;
            labEN1.BorderStyle = BorderStyle.None;
            labEN1.Name = "labEN1";
            labHP1.BackColor = Color.FromArgb(192, 192, 192);
            labHP1.Text = "HP";
            labHP1.ForeColor = Color.Black;
            labHP1.Size = new Size(22, 17);
            labHP1.Location = new Point(44, 8);
            labHP1.TabIndex = 1;
            labHP1.TextAlign = ContentAlignment.TopLeft;
            labHP1.Enabled = true;
            labHP1.Cursor = Cursors.Default;
            labHP1.RightToLeft = RightToLeft.No;
            labHP1.UseMnemonic = true;
            labHP1.Visible = true;
            labHP1.AutoSize = false;
            labHP1.BorderStyle = BorderStyle.None;
            labHP1.Name = "labHP1";
            Controls.Add(_picFace);
            Controls.Add(picUnit1);
            Controls.Add(picUnit2);
            Controls.Add(txtHP2);
            Controls.Add(picHP2);
            Controls.Add(picEN2);
            Controls.Add(txtEN2);
            Controls.Add(txtEN1);
            Controls.Add(picEN1);
            Controls.Add(picHP1);
            Controls.Add(txtHP1);
            Controls.Add(_picMessage);
            Controls.Add(labHP2);
            Controls.Add(labEN2);
            Controls.Add(labEN1);
            Controls.Add(labHP1);
            Click += new EventHandler(frmMessage_Click);
            DoubleClick += new EventHandler(frmMessage_DoubleClick);
            KeyDown += new KeyEventHandler(frmMessage_KeyDown);
            MouseDown += new MouseEventHandler(frmMessage_MouseDown);
            FormClosed += new FormClosedEventHandler(frmMessage_FormClosed);
            ResumeLayout(false);
            PerformLayout();
        }
        /* TODO ERROR: Skipped EndRegionDirectiveTrivia */
    }
}