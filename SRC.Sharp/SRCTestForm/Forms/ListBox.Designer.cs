using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    [DesignerGenerated()]
    internal partial class frmListBox
    {
        /* TODO ERROR: Skipped RegionDirectiveTrivia */
        [DebuggerNonUserCode()]
        public frmListBox() : base()
        {
            // この呼び出しは、Windows フォーム デザイナで必要です。
            InitializeComponent();
            _lstItems.Name = "lstItems";
            _labCaption.Name = "labCaption";
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
        private Timer _Timer2;

        public Timer Timer2
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _Timer2;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_Timer2 != null)
                {
                    _Timer2.Tick -= Timer2_Tick;
                }

                _Timer2 = value;
                if (_Timer2 != null)
                {
                    _Timer2.Tick += Timer2_Tick;
                }
            }
        }

        public TextBox txtComment;
        public TextBox txtMorale2;
        public TextBox txtMorale1;
        public TextBox txtLevel2;
        public TextBox txtLevel1;
        public TextBox txtHP1;
        public PictureBox picHP1;
        public PictureBox picEN1;
        public TextBox txtEN1;
        public TextBox txtEN2;
        public PictureBox picEN2;
        public PictureBox picHP2;
        public TextBox txtHP2;
        public PictureBox picUnit2;
        public PictureBox picUnit1;
        private Timer _Timer1;

        public Timer Timer1
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _Timer1;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_Timer1 != null)
                {
                    _Timer1.Tick -= Timer1_Tick;
                }

                _Timer1 = value;
                if (_Timer1 != null)
                {
                    _Timer1.Tick += Timer1_Tick;
                }
            }
        }

        private ListBox _lstItems;

        public ListBox lstItems
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _lstItems;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_lstItems != null)
                {
                    _lstItems.DoubleClick -= lstItems_DoubleClick;
                    _lstItems.MouseDown -= lstItems_MouseDown;
                    _lstItems.MouseMove -= lstItems_MouseMove;
                }

                _lstItems = value;
                if (_lstItems != null)
                {
                    _lstItems.DoubleClick += lstItems_DoubleClick;
                    _lstItems.MouseDown += lstItems_MouseDown;
                    _lstItems.MouseMove += lstItems_MouseMove;
                }
            }
        }

        public PictureBox picBar;
        private Label _labCaption;

        public Label labCaption
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _labCaption;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_labCaption != null)
                {
                    _labCaption.MouseDown -= labCaption_MouseDown;
                }

                _labCaption = value;
                if (_labCaption != null)
                {
                    _labCaption.MouseDown += labCaption_MouseDown;
                }
            }
        }

        public Label labMorale2;
        public Label labMorale1;
        public Label labLevel2;
        public PictureBox imgPilot2;
        public Label labLevel1;
        public PictureBox imgPilot1;
        public Label labHP1;
        public Label labEN1;
        public Label labEN2;
        public Label labHP2;
        // メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
        // Windows フォーム デザイナを使って変更できます。
        // コード エディタを使用して、変更しないでください。
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            var resources = new System.Resources.ResourceManager(typeof(frmListBox));
            components = new System.ComponentModel.Container();
            ToolTip1 = new ToolTip(components);
            _Timer2 = new Timer(components);
            _Timer2.Tick += new EventHandler(Timer2_Tick);
            txtComment = new TextBox();
            txtMorale2 = new TextBox();
            txtMorale1 = new TextBox();
            txtLevel2 = new TextBox();
            txtLevel1 = new TextBox();
            txtHP1 = new TextBox();
            picHP1 = new PictureBox();
            picEN1 = new PictureBox();
            txtEN1 = new TextBox();
            txtEN2 = new TextBox();
            picEN2 = new PictureBox();
            picHP2 = new PictureBox();
            txtHP2 = new TextBox();
            picUnit2 = new PictureBox();
            picUnit1 = new PictureBox();
            _Timer1 = new Timer(components);
            _Timer1.Tick += new EventHandler(Timer1_Tick);
            _lstItems = new ListBox();
            _lstItems.DoubleClick += new EventHandler(lstItems_DoubleClick);
            _lstItems.MouseDown += new MouseEventHandler(lstItems_MouseDown);
            _lstItems.MouseMove += new MouseEventHandler(lstItems_MouseMove);
            picBar = new PictureBox();
            _labCaption = new Label();
            _labCaption.MouseDown += new MouseEventHandler(labCaption_MouseDown);
            labMorale2 = new Label();
            labMorale1 = new Label();
            labLevel2 = new Label();
            imgPilot2 = new PictureBox();
            labLevel1 = new Label();
            imgPilot1 = new PictureBox();
            labHP1 = new Label();
            labEN1 = new Label();
            labEN2 = new Label();
            labHP2 = new Label();
            SuspendLayout();
            ToolTip1.Active = true;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.FromArgb(192, 192, 192);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Text = "ListBox";
            ClientSize = new Size(654, 137);
            Location = new Point(72, 116);
            Icon = (Icon)resources.GetObject("frmListBox.Icon");
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
            Name = "frmListBox";
            _Timer2.Interval = 100;
            _Timer2.Enabled = true;
            txtComment.AutoSize = false;
            txtComment.Enabled = false;
            txtComment.Font = new Font("ＭＳ ゴシック", 12f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            txtComment.Size = new Size(637, 38);
            txtComment.Location = new Point(6, 141);
            txtComment.Multiline = true;
            txtComment.TabIndex = 24;
            txtComment.TabStop = false;
            txtComment.Visible = false;
            txtComment.AcceptsReturn = true;
            txtComment.TextAlign = HorizontalAlignment.Left;
            txtComment.BackColor = SystemColors.Window;
            txtComment.CausesValidation = true;
            txtComment.ForeColor = SystemColors.WindowText;
            txtComment.HideSelection = true;
            txtComment.ReadOnly = false;
            txtComment.MaxLength = 0;
            txtComment.Cursor = Cursors.IBeam;
            txtComment.RightToLeft = RightToLeft.No;
            txtComment.ScrollBars = ScrollBars.None;
            txtComment.BorderStyle = BorderStyle.Fixed3D;
            txtComment.Name = "txtComment";
            txtMorale2.AutoSize = false;
            txtMorale2.BackColor = Color.FromArgb(192, 192, 192);
            txtMorale2.Font = new Font("ＭＳ 明朝", 9.75f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            txtMorale2.Size = new Size(25, 13);
            txtMorale2.Location = new Point(385, 23);
            txtMorale2.TabIndex = 22;
            txtMorale2.Text = "100";
            txtMorale2.Visible = false;
            txtMorale2.AcceptsReturn = true;
            txtMorale2.TextAlign = HorizontalAlignment.Left;
            txtMorale2.CausesValidation = true;
            txtMorale2.Enabled = true;
            txtMorale2.ForeColor = SystemColors.WindowText;
            txtMorale2.HideSelection = true;
            txtMorale2.ReadOnly = false;
            txtMorale2.MaxLength = 0;
            txtMorale2.Cursor = Cursors.IBeam;
            txtMorale2.Multiline = false;
            txtMorale2.RightToLeft = RightToLeft.No;
            txtMorale2.ScrollBars = ScrollBars.None;
            txtMorale2.TabStop = true;
            txtMorale2.BorderStyle = BorderStyle.None;
            txtMorale2.Name = "txtMorale2";
            txtMorale1.AutoSize = false;
            txtMorale1.BackColor = Color.FromArgb(192, 192, 192);
            txtMorale1.Font = new Font("ＭＳ 明朝", 9.75f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            txtMorale1.Size = new Size(25, 13);
            txtMorale1.Location = new Point(59, 22);
            txtMorale1.TabIndex = 20;
            txtMorale1.Text = "100";
            txtMorale1.Visible = false;
            txtMorale1.AcceptsReturn = true;
            txtMorale1.TextAlign = HorizontalAlignment.Left;
            txtMorale1.CausesValidation = true;
            txtMorale1.Enabled = true;
            txtMorale1.ForeColor = SystemColors.WindowText;
            txtMorale1.HideSelection = true;
            txtMorale1.ReadOnly = false;
            txtMorale1.MaxLength = 0;
            txtMorale1.Cursor = Cursors.IBeam;
            txtMorale1.Multiline = false;
            txtMorale1.RightToLeft = RightToLeft.No;
            txtMorale1.ScrollBars = ScrollBars.None;
            txtMorale1.TabStop = true;
            txtMorale1.BorderStyle = BorderStyle.None;
            txtMorale1.Name = "txtMorale1";
            txtLevel2.AutoSize = false;
            txtLevel2.BackColor = Color.FromArgb(192, 192, 192);
            txtLevel2.Font = new Font("ＭＳ 明朝", 9.75f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            txtLevel2.Size = new Size(17, 13);
            txtLevel2.Location = new Point(391, 7);
            txtLevel2.TabIndex = 19;
            txtLevel2.Text = "99";
            txtLevel2.Visible = false;
            txtLevel2.AcceptsReturn = true;
            txtLevel2.TextAlign = HorizontalAlignment.Left;
            txtLevel2.CausesValidation = true;
            txtLevel2.Enabled = true;
            txtLevel2.ForeColor = SystemColors.WindowText;
            txtLevel2.HideSelection = true;
            txtLevel2.ReadOnly = false;
            txtLevel2.MaxLength = 0;
            txtLevel2.Cursor = Cursors.IBeam;
            txtLevel2.Multiline = false;
            txtLevel2.RightToLeft = RightToLeft.No;
            txtLevel2.ScrollBars = ScrollBars.None;
            txtLevel2.TabStop = true;
            txtLevel2.BorderStyle = BorderStyle.None;
            txtLevel2.Name = "txtLevel2";
            txtLevel1.AutoSize = false;
            txtLevel1.BackColor = Color.FromArgb(192, 192, 192);
            txtLevel1.Font = new Font("ＭＳ 明朝", 9.75f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            txtLevel1.Size = new Size(17, 13);
            txtLevel1.Location = new Point(66, 6);
            txtLevel1.TabIndex = 17;
            txtLevel1.Text = "99";
            txtLevel1.Visible = false;
            txtLevel1.AcceptsReturn = true;
            txtLevel1.TextAlign = HorizontalAlignment.Left;
            txtLevel1.CausesValidation = true;
            txtLevel1.Enabled = true;
            txtLevel1.ForeColor = SystemColors.WindowText;
            txtLevel1.HideSelection = true;
            txtLevel1.ReadOnly = false;
            txtLevel1.MaxLength = 0;
            txtLevel1.Cursor = Cursors.IBeam;
            txtLevel1.Multiline = false;
            txtLevel1.RightToLeft = RightToLeft.No;
            txtLevel1.ScrollBars = ScrollBars.None;
            txtLevel1.TabStop = true;
            txtLevel1.BorderStyle = BorderStyle.None;
            txtLevel1.Name = "txtLevel1";
            txtHP1.AutoSize = false;
            txtHP1.BackColor = Color.FromArgb(192, 192, 192);
            txtHP1.Font = new Font("ＭＳ 明朝", 9.75f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            txtHP1.Size = new Size(88, 13);
            txtHP1.Location = new Point(148, 8);
            txtHP1.TabIndex = 11;
            txtHP1.Text = "99999/99999";
            txtHP1.Visible = false;
            txtHP1.AcceptsReturn = true;
            txtHP1.TextAlign = HorizontalAlignment.Left;
            txtHP1.CausesValidation = true;
            txtHP1.Enabled = true;
            txtHP1.ForeColor = SystemColors.WindowText;
            txtHP1.HideSelection = true;
            txtHP1.ReadOnly = false;
            txtHP1.MaxLength = 0;
            txtHP1.Cursor = Cursors.IBeam;
            txtHP1.Multiline = false;
            txtHP1.RightToLeft = RightToLeft.No;
            txtHP1.ScrollBars = ScrollBars.None;
            txtHP1.TabStop = true;
            txtHP1.BorderStyle = BorderStyle.None;
            txtHP1.Name = "txtHP1";
            picHP1.BackColor = Color.FromArgb(192, 0, 0);
            picHP1.ForeColor = Color.FromArgb(0, 192, 0);
            picHP1.Size = new Size(114, 8);
            picHP1.Location = new Point(122, 26);
            picHP1.TabIndex = 10;
            picHP1.Visible = false;
            picHP1.Dock = DockStyle.None;
            picHP1.CausesValidation = true;
            picHP1.Enabled = true;
            picHP1.Cursor = Cursors.Default;
            picHP1.RightToLeft = RightToLeft.No;
            picHP1.TabStop = true;
            picHP1.SizeMode = PictureBoxSizeMode.Normal;
            picHP1.BorderStyle = BorderStyle.Fixed3D;
            picHP1.Name = "picHP1";
            picEN1.BackColor = Color.FromArgb(192, 0, 0);
            picEN1.ForeColor = Color.FromArgb(0, 192, 0);
            picEN1.Size = new Size(79, 8);
            picEN1.Location = new Point(240, 26);
            picEN1.TabIndex = 9;
            picEN1.Visible = false;
            picEN1.Dock = DockStyle.None;
            picEN1.CausesValidation = true;
            picEN1.Enabled = true;
            picEN1.Cursor = Cursors.Default;
            picEN1.RightToLeft = RightToLeft.No;
            picEN1.TabStop = true;
            picEN1.SizeMode = PictureBoxSizeMode.Normal;
            picEN1.BorderStyle = BorderStyle.Fixed3D;
            picEN1.Name = "picEN1";
            txtEN1.AutoSize = false;
            txtEN1.BackColor = Color.FromArgb(192, 192, 192);
            txtEN1.Font = new Font("ＭＳ 明朝", 9.75f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            txtEN1.Size = new Size(57, 13);
            txtEN1.Location = new Point(263, 8);
            txtEN1.TabIndex = 8;
            txtEN1.Text = "999/999";
            txtEN1.Visible = false;
            txtEN1.AcceptsReturn = true;
            txtEN1.TextAlign = HorizontalAlignment.Left;
            txtEN1.CausesValidation = true;
            txtEN1.Enabled = true;
            txtEN1.ForeColor = SystemColors.WindowText;
            txtEN1.HideSelection = true;
            txtEN1.ReadOnly = false;
            txtEN1.MaxLength = 0;
            txtEN1.Cursor = Cursors.IBeam;
            txtEN1.Multiline = false;
            txtEN1.RightToLeft = RightToLeft.No;
            txtEN1.ScrollBars = ScrollBars.None;
            txtEN1.TabStop = true;
            txtEN1.BorderStyle = BorderStyle.None;
            txtEN1.Name = "txtEN1";
            txtEN2.AutoSize = false;
            txtEN2.BackColor = Color.FromArgb(192, 192, 192);
            txtEN2.Font = new Font("ＭＳ 明朝", 9.75f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            txtEN2.Size = new Size(57, 13);
            txtEN2.Location = new Point(587, 8);
            txtEN2.TabIndex = 7;
            txtEN2.Text = "999/999";
            txtEN2.Visible = false;
            txtEN2.AcceptsReturn = true;
            txtEN2.TextAlign = HorizontalAlignment.Left;
            txtEN2.CausesValidation = true;
            txtEN2.Enabled = true;
            txtEN2.ForeColor = SystemColors.WindowText;
            txtEN2.HideSelection = true;
            txtEN2.ReadOnly = false;
            txtEN2.MaxLength = 0;
            txtEN2.Cursor = Cursors.IBeam;
            txtEN2.Multiline = false;
            txtEN2.RightToLeft = RightToLeft.No;
            txtEN2.ScrollBars = ScrollBars.None;
            txtEN2.TabStop = true;
            txtEN2.BorderStyle = BorderStyle.None;
            txtEN2.Name = "txtEN2";
            picEN2.BackColor = Color.FromArgb(192, 0, 0);
            picEN2.ForeColor = Color.FromArgb(0, 192, 0);
            picEN2.Size = new Size(78, 8);
            picEN2.Location = new Point(565, 27);
            picEN2.TabIndex = 6;
            picEN2.Visible = false;
            picEN2.Dock = DockStyle.None;
            picEN2.CausesValidation = true;
            picEN2.Enabled = true;
            picEN2.Cursor = Cursors.Default;
            picEN2.RightToLeft = RightToLeft.No;
            picEN2.TabStop = true;
            picEN2.SizeMode = PictureBoxSizeMode.Normal;
            picEN2.BorderStyle = BorderStyle.Fixed3D;
            picEN2.Name = "picEN2";
            picHP2.BackColor = Color.FromArgb(192, 0, 0);
            picHP2.ForeColor = Color.FromArgb(0, 192, 0);
            picHP2.Size = new Size(112, 8);
            picHP2.Location = new Point(449, 27);
            picHP2.TabIndex = 5;
            picHP2.Visible = false;
            picHP2.Dock = DockStyle.None;
            picHP2.CausesValidation = true;
            picHP2.Enabled = true;
            picHP2.Cursor = Cursors.Default;
            picHP2.RightToLeft = RightToLeft.No;
            picHP2.TabStop = true;
            picHP2.SizeMode = PictureBoxSizeMode.Normal;
            picHP2.BorderStyle = BorderStyle.Fixed3D;
            picHP2.Name = "picHP2";
            txtHP2.AutoSize = false;
            txtHP2.BackColor = Color.FromArgb(192, 192, 192);
            txtHP2.Font = new Font("ＭＳ 明朝", 9.75f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            txtHP2.Size = new Size(88, 13);
            txtHP2.Location = new Point(473, 8);
            txtHP2.TabIndex = 4;
            txtHP2.Text = "99999/99999";
            txtHP2.Visible = false;
            txtHP2.AcceptsReturn = true;
            txtHP2.TextAlign = HorizontalAlignment.Left;
            txtHP2.CausesValidation = true;
            txtHP2.Enabled = true;
            txtHP2.ForeColor = SystemColors.WindowText;
            txtHP2.HideSelection = true;
            txtHP2.ReadOnly = false;
            txtHP2.MaxLength = 0;
            txtHP2.Cursor = Cursors.IBeam;
            txtHP2.Multiline = false;
            txtHP2.RightToLeft = RightToLeft.No;
            txtHP2.ScrollBars = ScrollBars.None;
            txtHP2.TabStop = true;
            txtHP2.BorderStyle = BorderStyle.None;
            txtHP2.Name = "txtHP2";
            picUnit2.BackColor = SystemColors.Window;
            picUnit2.ForeColor = SystemColors.WindowText;
            picUnit2.Size = new Size(32, 32);
            picUnit2.Location = new Point(412, 5);
            picUnit2.TabIndex = 3;
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
            picUnit1.BackColor = SystemColors.Window;
            picUnit1.ForeColor = SystemColors.WindowText;
            picUnit1.Size = new Size(32, 32);
            picUnit1.Location = new Point(85, 4);
            picUnit1.TabIndex = 2;
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
            _Timer1.Enabled = false;
            _Timer1.Interval = 100;
            _lstItems.BackColor = Color.White;
            _lstItems.Font = new Font("ＭＳ 明朝", 12f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            _lstItems.ForeColor = Color.Black;
            _lstItems.Size = new Size(643, 103);
            _lstItems.Location = new Point(6, 32);
            _lstItems.TabIndex = 0;
            _lstItems.BorderStyle = BorderStyle.Fixed3D;
            _lstItems.CausesValidation = true;
            _lstItems.Enabled = true;
            _lstItems.IntegralHeight = true;
            _lstItems.Cursor = Cursors.Default;
            _lstItems.SelectionMode = SelectionMode.One;
            _lstItems.RightToLeft = RightToLeft.No;
            _lstItems.Sorted = false;
            _lstItems.TabStop = true;
            _lstItems.Visible = true;
            _lstItems.MultiColumn = false;
            _lstItems.Name = "_lstItems";
            picBar.BackColor = Color.White;
            picBar.ForeColor = Color.FromArgb(0, 0, 128);
            picBar.Size = new Size(643, 13);
            picBar.Location = new Point(6, 123);
            picBar.TabIndex = 25;
            picBar.Visible = false;
            picBar.Dock = DockStyle.None;
            picBar.CausesValidation = true;
            picBar.Enabled = true;
            picBar.Cursor = Cursors.Default;
            picBar.RightToLeft = RightToLeft.No;
            picBar.TabStop = true;
            picBar.SizeMode = PictureBoxSizeMode.Normal;
            picBar.BorderStyle = BorderStyle.Fixed3D;
            picBar.Name = "picBar";
            _labCaption.BackColor = Color.White;
            _labCaption.Font = new Font("ＭＳ 明朝", 12f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            _labCaption.ForeColor = Color.Black;
            _labCaption.Size = new Size(643, 23);
            _labCaption.Location = new Point(6, 5);
            _labCaption.TabIndex = 1;
            _labCaption.TextAlign = ContentAlignment.TopLeft;
            _labCaption.Enabled = true;
            _labCaption.Cursor = Cursors.Default;
            _labCaption.RightToLeft = RightToLeft.No;
            _labCaption.UseMnemonic = true;
            _labCaption.Visible = true;
            _labCaption.AutoSize = false;
            _labCaption.BorderStyle = BorderStyle.Fixed3D;
            _labCaption.Name = "_labCaption";
            labMorale2.BackColor = Color.FromArgb(192, 192, 192);
            labMorale2.Text = "M";
            labMorale2.Font = new Font("ＭＳ Ｐ明朝", 11.25f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            labMorale2.ForeColor = Color.Black;
            labMorale2.Size = new Size(12, 17);
            labMorale2.Location = new Point(372, 22);
            labMorale2.TabIndex = 23;
            labMorale2.Visible = false;
            labMorale2.TextAlign = ContentAlignment.TopLeft;
            labMorale2.Enabled = true;
            labMorale2.Cursor = Cursors.Default;
            labMorale2.RightToLeft = RightToLeft.No;
            labMorale2.UseMnemonic = true;
            labMorale2.AutoSize = false;
            labMorale2.BorderStyle = BorderStyle.None;
            labMorale2.Name = "labMorale2";
            labMorale1.BackColor = Color.FromArgb(192, 192, 192);
            labMorale1.Text = "M";
            labMorale1.Font = new Font("ＭＳ Ｐ明朝", 11.25f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            labMorale1.ForeColor = Color.Black;
            labMorale1.Size = new Size(12, 17);
            labMorale1.Location = new Point(46, 20);
            labMorale1.TabIndex = 21;
            labMorale1.Visible = false;
            labMorale1.TextAlign = ContentAlignment.TopLeft;
            labMorale1.Enabled = true;
            labMorale1.Cursor = Cursors.Default;
            labMorale1.RightToLeft = RightToLeft.No;
            labMorale1.UseMnemonic = true;
            labMorale1.AutoSize = false;
            labMorale1.BorderStyle = BorderStyle.None;
            labMorale1.Name = "labMorale1";
            labLevel2.BackColor = Color.FromArgb(192, 192, 192);
            labLevel2.Text = "Lv";
            labLevel2.Font = new Font("ＭＳ Ｐ明朝", 12f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            labLevel2.ForeColor = Color.Black;
            labLevel2.Size = new Size(20, 17);
            labLevel2.Location = new Point(370, 4);
            labLevel2.TabIndex = 18;
            labLevel2.Visible = false;
            labLevel2.TextAlign = ContentAlignment.TopLeft;
            labLevel2.Enabled = true;
            labLevel2.Cursor = Cursors.Default;
            labLevel2.RightToLeft = RightToLeft.No;
            labLevel2.UseMnemonic = true;
            labLevel2.AutoSize = false;
            labLevel2.BorderStyle = BorderStyle.None;
            labLevel2.Name = "labLevel2";
            imgPilot2.Size = new Size(36, 36);
            imgPilot2.Location = new Point(331, 3);
            imgPilot2.SizeMode = PictureBoxSizeMode.StretchImage;
            imgPilot2.Visible = false;
            imgPilot2.Enabled = true;
            imgPilot2.Cursor = Cursors.Default;
            imgPilot2.BorderStyle = BorderStyle.Fixed3D;
            imgPilot2.Name = "imgPilot2";
            labLevel1.BackColor = Color.FromArgb(192, 192, 192);
            labLevel1.Text = "Lv";
            labLevel1.Font = new Font("ＭＳ Ｐ明朝", 12f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            labLevel1.ForeColor = Color.Black;
            labLevel1.Size = new Size(20, 17);
            labLevel1.Location = new Point(45, 4);
            labLevel1.TabIndex = 16;
            labLevel1.Visible = false;
            labLevel1.TextAlign = ContentAlignment.TopLeft;
            labLevel1.Enabled = true;
            labLevel1.Cursor = Cursors.Default;
            labLevel1.RightToLeft = RightToLeft.No;
            labLevel1.UseMnemonic = true;
            labLevel1.AutoSize = false;
            labLevel1.BorderStyle = BorderStyle.None;
            labLevel1.Name = "labLevel1";
            imgPilot1.Size = new Size(36, 36);
            imgPilot1.Location = new Point(6, 3);
            imgPilot1.SizeMode = PictureBoxSizeMode.StretchImage;
            imgPilot1.Visible = false;
            imgPilot1.Enabled = true;
            imgPilot1.Cursor = Cursors.Default;
            imgPilot1.BorderStyle = BorderStyle.Fixed3D;
            imgPilot1.Name = "imgPilot1";
            labHP1.BackColor = Color.FromArgb(192, 192, 192);
            labHP1.Text = "HP";
            labHP1.Font = new Font("ＭＳ Ｐ明朝", 12f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            labHP1.ForeColor = Color.Black;
            labHP1.Size = new Size(23, 17);
            labHP1.Location = new Point(121, 6);
            labHP1.TabIndex = 15;
            labHP1.Visible = false;
            labHP1.TextAlign = ContentAlignment.TopLeft;
            labHP1.Enabled = true;
            labHP1.Cursor = Cursors.Default;
            labHP1.RightToLeft = RightToLeft.No;
            labHP1.UseMnemonic = true;
            labHP1.AutoSize = false;
            labHP1.BorderStyle = BorderStyle.None;
            labHP1.Name = "labHP1";
            labEN1.BackColor = Color.FromArgb(192, 192, 192);
            labEN1.Text = "EN";
            labEN1.Font = new Font("ＭＳ Ｐ明朝", 12f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            labEN1.ForeColor = Color.Black;
            labEN1.Size = new Size(22, 17);
            labEN1.Location = new Point(238, 6);
            labEN1.TabIndex = 14;
            labEN1.Visible = false;
            labEN1.TextAlign = ContentAlignment.TopLeft;
            labEN1.Enabled = true;
            labEN1.Cursor = Cursors.Default;
            labEN1.RightToLeft = RightToLeft.No;
            labEN1.UseMnemonic = true;
            labEN1.AutoSize = false;
            labEN1.BorderStyle = BorderStyle.None;
            labEN1.Name = "labEN1";
            labEN2.BackColor = Color.FromArgb(192, 192, 192);
            labEN2.Text = "EN";
            labEN2.Font = new Font("ＭＳ Ｐ明朝", 12f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            labEN2.ForeColor = Color.Black;
            labEN2.Size = new Size(25, 17);
            labEN2.Location = new Point(563, 6);
            labEN2.TabIndex = 13;
            labEN2.Visible = false;
            labEN2.TextAlign = ContentAlignment.TopLeft;
            labEN2.Enabled = true;
            labEN2.Cursor = Cursors.Default;
            labEN2.RightToLeft = RightToLeft.No;
            labEN2.UseMnemonic = true;
            labEN2.AutoSize = false;
            labEN2.BorderStyle = BorderStyle.None;
            labEN2.Name = "labEN2";
            labHP2.BackColor = Color.FromArgb(192, 192, 192);
            labHP2.Text = "HP";
            labHP2.Font = new Font("ＭＳ Ｐ明朝", 12f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            labHP2.ForeColor = Color.Black;
            labHP2.Size = new Size(25, 17);
            labHP2.Location = new Point(448, 6);
            labHP2.TabIndex = 12;
            labHP2.Visible = false;
            labHP2.TextAlign = ContentAlignment.TopLeft;
            labHP2.Enabled = true;
            labHP2.Cursor = Cursors.Default;
            labHP2.RightToLeft = RightToLeft.No;
            labHP2.UseMnemonic = true;
            labHP2.AutoSize = false;
            labHP2.BorderStyle = BorderStyle.None;
            labHP2.Name = "labHP2";
            Controls.Add(txtComment);
            Controls.Add(txtMorale2);
            Controls.Add(txtMorale1);
            Controls.Add(txtLevel2);
            Controls.Add(txtLevel1);
            Controls.Add(txtHP1);
            Controls.Add(picHP1);
            Controls.Add(picEN1);
            Controls.Add(txtEN1);
            Controls.Add(txtEN2);
            Controls.Add(picEN2);
            Controls.Add(picHP2);
            Controls.Add(txtHP2);
            Controls.Add(picUnit2);
            Controls.Add(picUnit1);
            Controls.Add(_lstItems);
            Controls.Add(picBar);
            Controls.Add(_labCaption);
            Controls.Add(labMorale2);
            Controls.Add(labMorale1);
            Controls.Add(labLevel2);
            Controls.Add(imgPilot2);
            Controls.Add(labLevel1);
            Controls.Add(imgPilot1);
            Controls.Add(labHP1);
            Controls.Add(labEN1);
            Controls.Add(labEN2);
            Controls.Add(labHP2);
            KeyDown += new KeyEventHandler(frmListBox_KeyDown);
            Load += new EventHandler(frmListBox_Load);
            FormClosed += new FormClosedEventHandler(frmListBox_FormClosed);
            MouseDown += new MouseEventHandler(frmListBox_MouseDown);
            ResumeLayout(false);
            PerformLayout();
        }
        /* TODO ERROR: Skipped EndRegionDirectiveTrivia */
    }
}