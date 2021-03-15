using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace SRCSharpForm
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

        private ListBox lstItems;
        public PictureBox picBar;
        private Label labCaption;
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmListBox));
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._Timer2 = new System.Windows.Forms.Timer(this.components);
            this.txtComment = new System.Windows.Forms.TextBox();
            this.txtMorale2 = new System.Windows.Forms.TextBox();
            this.txtMorale1 = new System.Windows.Forms.TextBox();
            this.txtLevel2 = new System.Windows.Forms.TextBox();
            this.txtLevel1 = new System.Windows.Forms.TextBox();
            this.txtHP1 = new System.Windows.Forms.TextBox();
            this.picHP1 = new System.Windows.Forms.PictureBox();
            this.picEN1 = new System.Windows.Forms.PictureBox();
            this.txtEN1 = new System.Windows.Forms.TextBox();
            this.txtEN2 = new System.Windows.Forms.TextBox();
            this.picEN2 = new System.Windows.Forms.PictureBox();
            this.picHP2 = new System.Windows.Forms.PictureBox();
            this.txtHP2 = new System.Windows.Forms.TextBox();
            this.picUnit2 = new System.Windows.Forms.PictureBox();
            this.picUnit1 = new System.Windows.Forms.PictureBox();
            this._Timer1 = new System.Windows.Forms.Timer(this.components);
            this.lstItems = new System.Windows.Forms.ListBox();
            this.picBar = new System.Windows.Forms.PictureBox();
            this.labCaption = new System.Windows.Forms.Label();
            this.labMorale2 = new System.Windows.Forms.Label();
            this.labMorale1 = new System.Windows.Forms.Label();
            this.labLevel2 = new System.Windows.Forms.Label();
            this.imgPilot2 = new System.Windows.Forms.PictureBox();
            this.labLevel1 = new System.Windows.Forms.Label();
            this.imgPilot1 = new System.Windows.Forms.PictureBox();
            this.labHP1 = new System.Windows.Forms.Label();
            this.labEN1 = new System.Windows.Forms.Label();
            this.labEN2 = new System.Windows.Forms.Label();
            this.labHP2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picHP1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEN1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEN2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picHP2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUnit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUnit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgPilot2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgPilot1)).BeginInit();
            this.SuspendLayout();
            // 
            // _Timer2
            // 
            this._Timer2.Enabled = true;
            this._Timer2.Tick += new System.EventHandler(this.Timer2_Tick);
            // 
            // txtComment
            // 
            this.txtComment.AcceptsReturn = true;
            this.txtComment.BackColor = System.Drawing.SystemColors.Window;
            this.txtComment.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtComment.Enabled = false;
            this.txtComment.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtComment.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtComment.Location = new System.Drawing.Point(6, 141);
            this.txtComment.MaxLength = 0;
            this.txtComment.Multiline = true;
            this.txtComment.Name = "txtComment";
            this.txtComment.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtComment.Size = new System.Drawing.Size(637, 38);
            this.txtComment.TabIndex = 24;
            this.txtComment.TabStop = false;
            this.txtComment.Visible = false;
            // 
            // txtMorale2
            // 
            this.txtMorale2.AcceptsReturn = true;
            this.txtMorale2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.txtMorale2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMorale2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtMorale2.Font = new System.Drawing.Font("ＭＳ 明朝", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtMorale2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtMorale2.Location = new System.Drawing.Point(385, 23);
            this.txtMorale2.MaxLength = 0;
            this.txtMorale2.Name = "txtMorale2";
            this.txtMorale2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtMorale2.Size = new System.Drawing.Size(25, 13);
            this.txtMorale2.TabIndex = 22;
            this.txtMorale2.Text = "100";
            this.txtMorale2.Visible = false;
            // 
            // txtMorale1
            // 
            this.txtMorale1.AcceptsReturn = true;
            this.txtMorale1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.txtMorale1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMorale1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtMorale1.Font = new System.Drawing.Font("ＭＳ 明朝", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtMorale1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtMorale1.Location = new System.Drawing.Point(59, 22);
            this.txtMorale1.MaxLength = 0;
            this.txtMorale1.Name = "txtMorale1";
            this.txtMorale1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtMorale1.Size = new System.Drawing.Size(25, 13);
            this.txtMorale1.TabIndex = 20;
            this.txtMorale1.Text = "100";
            this.txtMorale1.Visible = false;
            // 
            // txtLevel2
            // 
            this.txtLevel2.AcceptsReturn = true;
            this.txtLevel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.txtLevel2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLevel2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtLevel2.Font = new System.Drawing.Font("ＭＳ 明朝", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtLevel2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtLevel2.Location = new System.Drawing.Point(391, 7);
            this.txtLevel2.MaxLength = 0;
            this.txtLevel2.Name = "txtLevel2";
            this.txtLevel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtLevel2.Size = new System.Drawing.Size(17, 13);
            this.txtLevel2.TabIndex = 19;
            this.txtLevel2.Text = "99";
            this.txtLevel2.Visible = false;
            // 
            // txtLevel1
            // 
            this.txtLevel1.AcceptsReturn = true;
            this.txtLevel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.txtLevel1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLevel1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtLevel1.Font = new System.Drawing.Font("ＭＳ 明朝", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtLevel1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtLevel1.Location = new System.Drawing.Point(66, 6);
            this.txtLevel1.MaxLength = 0;
            this.txtLevel1.Name = "txtLevel1";
            this.txtLevel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtLevel1.Size = new System.Drawing.Size(17, 13);
            this.txtLevel1.TabIndex = 17;
            this.txtLevel1.Text = "99";
            this.txtLevel1.Visible = false;
            // 
            // txtHP1
            // 
            this.txtHP1.AcceptsReturn = true;
            this.txtHP1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.txtHP1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtHP1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtHP1.Font = new System.Drawing.Font("ＭＳ 明朝", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtHP1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtHP1.Location = new System.Drawing.Point(148, 8);
            this.txtHP1.MaxLength = 0;
            this.txtHP1.Name = "txtHP1";
            this.txtHP1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtHP1.Size = new System.Drawing.Size(88, 13);
            this.txtHP1.TabIndex = 11;
            this.txtHP1.Text = "99999/99999";
            this.txtHP1.Visible = false;
            // 
            // picHP1
            // 
            this.picHP1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.picHP1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picHP1.Cursor = System.Windows.Forms.Cursors.Default;
            this.picHP1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.picHP1.Location = new System.Drawing.Point(122, 26);
            this.picHP1.Name = "picHP1";
            this.picHP1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picHP1.Size = new System.Drawing.Size(114, 8);
            this.picHP1.TabIndex = 10;
            this.picHP1.TabStop = false;
            this.picHP1.Visible = false;
            // 
            // picEN1
            // 
            this.picEN1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.picEN1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picEN1.Cursor = System.Windows.Forms.Cursors.Default;
            this.picEN1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.picEN1.Location = new System.Drawing.Point(240, 26);
            this.picEN1.Name = "picEN1";
            this.picEN1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picEN1.Size = new System.Drawing.Size(79, 8);
            this.picEN1.TabIndex = 9;
            this.picEN1.TabStop = false;
            this.picEN1.Visible = false;
            // 
            // txtEN1
            // 
            this.txtEN1.AcceptsReturn = true;
            this.txtEN1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.txtEN1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtEN1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtEN1.Font = new System.Drawing.Font("ＭＳ 明朝", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtEN1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtEN1.Location = new System.Drawing.Point(263, 8);
            this.txtEN1.MaxLength = 0;
            this.txtEN1.Name = "txtEN1";
            this.txtEN1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtEN1.Size = new System.Drawing.Size(57, 13);
            this.txtEN1.TabIndex = 8;
            this.txtEN1.Text = "999/999";
            this.txtEN1.Visible = false;
            // 
            // txtEN2
            // 
            this.txtEN2.AcceptsReturn = true;
            this.txtEN2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.txtEN2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtEN2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtEN2.Font = new System.Drawing.Font("ＭＳ 明朝", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtEN2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtEN2.Location = new System.Drawing.Point(587, 8);
            this.txtEN2.MaxLength = 0;
            this.txtEN2.Name = "txtEN2";
            this.txtEN2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtEN2.Size = new System.Drawing.Size(57, 13);
            this.txtEN2.TabIndex = 7;
            this.txtEN2.Text = "999/999";
            this.txtEN2.Visible = false;
            // 
            // picEN2
            // 
            this.picEN2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.picEN2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picEN2.Cursor = System.Windows.Forms.Cursors.Default;
            this.picEN2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.picEN2.Location = new System.Drawing.Point(565, 27);
            this.picEN2.Name = "picEN2";
            this.picEN2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picEN2.Size = new System.Drawing.Size(78, 8);
            this.picEN2.TabIndex = 6;
            this.picEN2.TabStop = false;
            this.picEN2.Visible = false;
            // 
            // picHP2
            // 
            this.picHP2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.picHP2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picHP2.Cursor = System.Windows.Forms.Cursors.Default;
            this.picHP2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.picHP2.Location = new System.Drawing.Point(449, 27);
            this.picHP2.Name = "picHP2";
            this.picHP2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picHP2.Size = new System.Drawing.Size(112, 8);
            this.picHP2.TabIndex = 5;
            this.picHP2.TabStop = false;
            this.picHP2.Visible = false;
            // 
            // txtHP2
            // 
            this.txtHP2.AcceptsReturn = true;
            this.txtHP2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.txtHP2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtHP2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtHP2.Font = new System.Drawing.Font("ＭＳ 明朝", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtHP2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtHP2.Location = new System.Drawing.Point(473, 8);
            this.txtHP2.MaxLength = 0;
            this.txtHP2.Name = "txtHP2";
            this.txtHP2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtHP2.Size = new System.Drawing.Size(88, 13);
            this.txtHP2.TabIndex = 4;
            this.txtHP2.Text = "99999/99999";
            this.txtHP2.Visible = false;
            // 
            // picUnit2
            // 
            this.picUnit2.BackColor = System.Drawing.SystemColors.Window;
            this.picUnit2.Cursor = System.Windows.Forms.Cursors.Default;
            this.picUnit2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.picUnit2.Location = new System.Drawing.Point(412, 5);
            this.picUnit2.Name = "picUnit2";
            this.picUnit2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picUnit2.Size = new System.Drawing.Size(32, 32);
            this.picUnit2.TabIndex = 3;
            this.picUnit2.TabStop = false;
            this.picUnit2.Visible = false;
            // 
            // picUnit1
            // 
            this.picUnit1.BackColor = System.Drawing.SystemColors.Window;
            this.picUnit1.Cursor = System.Windows.Forms.Cursors.Default;
            this.picUnit1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.picUnit1.Location = new System.Drawing.Point(85, 4);
            this.picUnit1.Name = "picUnit1";
            this.picUnit1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picUnit1.Size = new System.Drawing.Size(32, 32);
            this.picUnit1.TabIndex = 2;
            this.picUnit1.TabStop = false;
            this.picUnit1.Visible = false;
            // 
            // _Timer1
            // 
            this._Timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // lstItems
            // 
            this.lstItems.BackColor = System.Drawing.Color.White;
            this.lstItems.Cursor = System.Windows.Forms.Cursors.Default;
            this.lstItems.Font = new System.Drawing.Font("ＭＳ 明朝", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lstItems.ForeColor = System.Drawing.Color.Black;
            this.lstItems.ItemHeight = 16;
            this.lstItems.Location = new System.Drawing.Point(6, 32);
            this.lstItems.Name = "lstItems";
            this.lstItems.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lstItems.Size = new System.Drawing.Size(643, 100);
            this.lstItems.TabIndex = 0;
            this.lstItems.DoubleClick += new System.EventHandler(this.lstItems_DoubleClick);
            this.lstItems.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstItems_MouseDown);
            this.lstItems.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lstItems_MouseMove);
            // 
            // picBar
            // 
            this.picBar.BackColor = System.Drawing.Color.White;
            this.picBar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picBar.Cursor = System.Windows.Forms.Cursors.Default;
            this.picBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))));
            this.picBar.Location = new System.Drawing.Point(6, 123);
            this.picBar.Name = "picBar";
            this.picBar.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picBar.Size = new System.Drawing.Size(643, 13);
            this.picBar.TabIndex = 25;
            this.picBar.TabStop = false;
            this.picBar.Visible = false;
            // 
            // labCaption
            // 
            this.labCaption.BackColor = System.Drawing.Color.White;
            this.labCaption.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labCaption.Cursor = System.Windows.Forms.Cursors.Default;
            this.labCaption.Font = new System.Drawing.Font("ＭＳ 明朝", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labCaption.ForeColor = System.Drawing.Color.Black;
            this.labCaption.Location = new System.Drawing.Point(6, 5);
            this.labCaption.Name = "labCaption";
            this.labCaption.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labCaption.Size = new System.Drawing.Size(643, 23);
            this.labCaption.TabIndex = 1;
            this.labCaption.MouseDown += new System.Windows.Forms.MouseEventHandler(this.labCaption_MouseDown);
            // 
            // labMorale2
            // 
            this.labMorale2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.labMorale2.Cursor = System.Windows.Forms.Cursors.Default;
            this.labMorale2.Font = new System.Drawing.Font("ＭＳ Ｐ明朝", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labMorale2.ForeColor = System.Drawing.Color.Black;
            this.labMorale2.Location = new System.Drawing.Point(372, 22);
            this.labMorale2.Name = "labMorale2";
            this.labMorale2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labMorale2.Size = new System.Drawing.Size(12, 17);
            this.labMorale2.TabIndex = 23;
            this.labMorale2.Text = "M";
            this.labMorale2.Visible = false;
            // 
            // labMorale1
            // 
            this.labMorale1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.labMorale1.Cursor = System.Windows.Forms.Cursors.Default;
            this.labMorale1.Font = new System.Drawing.Font("ＭＳ Ｐ明朝", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labMorale1.ForeColor = System.Drawing.Color.Black;
            this.labMorale1.Location = new System.Drawing.Point(46, 20);
            this.labMorale1.Name = "labMorale1";
            this.labMorale1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labMorale1.Size = new System.Drawing.Size(12, 17);
            this.labMorale1.TabIndex = 21;
            this.labMorale1.Text = "M";
            this.labMorale1.Visible = false;
            // 
            // labLevel2
            // 
            this.labLevel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.labLevel2.Cursor = System.Windows.Forms.Cursors.Default;
            this.labLevel2.Font = new System.Drawing.Font("ＭＳ Ｐ明朝", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labLevel2.ForeColor = System.Drawing.Color.Black;
            this.labLevel2.Location = new System.Drawing.Point(370, 4);
            this.labLevel2.Name = "labLevel2";
            this.labLevel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labLevel2.Size = new System.Drawing.Size(20, 17);
            this.labLevel2.TabIndex = 18;
            this.labLevel2.Text = "Lv";
            this.labLevel2.Visible = false;
            // 
            // imgPilot2
            // 
            this.imgPilot2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.imgPilot2.Cursor = System.Windows.Forms.Cursors.Default;
            this.imgPilot2.Location = new System.Drawing.Point(331, 3);
            this.imgPilot2.Name = "imgPilot2";
            this.imgPilot2.Size = new System.Drawing.Size(36, 36);
            this.imgPilot2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgPilot2.TabIndex = 26;
            this.imgPilot2.TabStop = false;
            this.imgPilot2.Visible = false;
            // 
            // labLevel1
            // 
            this.labLevel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.labLevel1.Cursor = System.Windows.Forms.Cursors.Default;
            this.labLevel1.Font = new System.Drawing.Font("ＭＳ Ｐ明朝", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labLevel1.ForeColor = System.Drawing.Color.Black;
            this.labLevel1.Location = new System.Drawing.Point(45, 4);
            this.labLevel1.Name = "labLevel1";
            this.labLevel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labLevel1.Size = new System.Drawing.Size(20, 17);
            this.labLevel1.TabIndex = 16;
            this.labLevel1.Text = "Lv";
            this.labLevel1.Visible = false;
            // 
            // imgPilot1
            // 
            this.imgPilot1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.imgPilot1.Cursor = System.Windows.Forms.Cursors.Default;
            this.imgPilot1.Location = new System.Drawing.Point(6, 3);
            this.imgPilot1.Name = "imgPilot1";
            this.imgPilot1.Size = new System.Drawing.Size(36, 36);
            this.imgPilot1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgPilot1.TabIndex = 27;
            this.imgPilot1.TabStop = false;
            this.imgPilot1.Visible = false;
            // 
            // labHP1
            // 
            this.labHP1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.labHP1.Cursor = System.Windows.Forms.Cursors.Default;
            this.labHP1.Font = new System.Drawing.Font("ＭＳ Ｐ明朝", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labHP1.ForeColor = System.Drawing.Color.Black;
            this.labHP1.Location = new System.Drawing.Point(121, 6);
            this.labHP1.Name = "labHP1";
            this.labHP1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labHP1.Size = new System.Drawing.Size(23, 17);
            this.labHP1.TabIndex = 15;
            this.labHP1.Text = "HP";
            this.labHP1.Visible = false;
            // 
            // labEN1
            // 
            this.labEN1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.labEN1.Cursor = System.Windows.Forms.Cursors.Default;
            this.labEN1.Font = new System.Drawing.Font("ＭＳ Ｐ明朝", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labEN1.ForeColor = System.Drawing.Color.Black;
            this.labEN1.Location = new System.Drawing.Point(238, 6);
            this.labEN1.Name = "labEN1";
            this.labEN1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labEN1.Size = new System.Drawing.Size(22, 17);
            this.labEN1.TabIndex = 14;
            this.labEN1.Text = "EN";
            this.labEN1.Visible = false;
            // 
            // labEN2
            // 
            this.labEN2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.labEN2.Cursor = System.Windows.Forms.Cursors.Default;
            this.labEN2.Font = new System.Drawing.Font("ＭＳ Ｐ明朝", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labEN2.ForeColor = System.Drawing.Color.Black;
            this.labEN2.Location = new System.Drawing.Point(563, 6);
            this.labEN2.Name = "labEN2";
            this.labEN2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labEN2.Size = new System.Drawing.Size(25, 17);
            this.labEN2.TabIndex = 13;
            this.labEN2.Text = "EN";
            this.labEN2.Visible = false;
            // 
            // labHP2
            // 
            this.labHP2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.labHP2.Cursor = System.Windows.Forms.Cursors.Default;
            this.labHP2.Font = new System.Drawing.Font("ＭＳ Ｐ明朝", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labHP2.ForeColor = System.Drawing.Color.Black;
            this.labHP2.Location = new System.Drawing.Point(448, 6);
            this.labHP2.Name = "labHP2";
            this.labHP2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labHP2.Size = new System.Drawing.Size(25, 17);
            this.labHP2.TabIndex = 12;
            this.labHP2.Text = "HP";
            this.labHP2.Visible = false;
            // 
            // frmListBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(654, 137);
            this.Controls.Add(this.txtComment);
            this.Controls.Add(this.txtMorale2);
            this.Controls.Add(this.txtMorale1);
            this.Controls.Add(this.txtLevel2);
            this.Controls.Add(this.txtLevel1);
            this.Controls.Add(this.txtHP1);
            this.Controls.Add(this.picHP1);
            this.Controls.Add(this.picEN1);
            this.Controls.Add(this.txtEN1);
            this.Controls.Add(this.txtEN2);
            this.Controls.Add(this.picEN2);
            this.Controls.Add(this.picHP2);
            this.Controls.Add(this.txtHP2);
            this.Controls.Add(this.picUnit2);
            this.Controls.Add(this.picUnit1);
            this.Controls.Add(this.lstItems);
            this.Controls.Add(this.picBar);
            this.Controls.Add(this.labCaption);
            this.Controls.Add(this.labMorale2);
            this.Controls.Add(this.labMorale1);
            this.Controls.Add(this.labLevel2);
            this.Controls.Add(this.imgPilot2);
            this.Controls.Add(this.labLevel1);
            this.Controls.Add(this.imgPilot1);
            this.Controls.Add(this.labHP1);
            this.Controls.Add(this.labEN1);
            this.Controls.Add(this.labEN2);
            this.Controls.Add(this.labHP2);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Location = new System.Drawing.Point(72, 116);
            this.MaximizeBox = false;
            this.Name = "frmListBox";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ListBox";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmListBox_FormClosing);
            this.Load += new System.EventHandler(this.frmListBox_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmListBox_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmListBox_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.picHP1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEN1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEN2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picHP2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUnit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUnit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgPilot2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgPilot1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        /* TODO ERROR: Skipped EndRegionDirectiveTrivia */
    }
}