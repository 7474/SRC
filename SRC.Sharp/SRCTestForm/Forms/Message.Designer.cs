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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMessage));
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._picFace = new System.Windows.Forms.PictureBox();
            this.picUnit1 = new System.Windows.Forms.PictureBox();
            this.picUnit2 = new System.Windows.Forms.PictureBox();
            this.txtHP2 = new System.Windows.Forms.TextBox();
            this.picHP2 = new System.Windows.Forms.PictureBox();
            this.picEN2 = new System.Windows.Forms.PictureBox();
            this.txtEN2 = new System.Windows.Forms.TextBox();
            this.txtEN1 = new System.Windows.Forms.TextBox();
            this.picEN1 = new System.Windows.Forms.PictureBox();
            this.picHP1 = new System.Windows.Forms.PictureBox();
            this.txtHP1 = new System.Windows.Forms.TextBox();
            this._picMessage = new System.Windows.Forms.PictureBox();
            this.labHP2 = new System.Windows.Forms.Label();
            this.labEN2 = new System.Windows.Forms.Label();
            this.labEN1 = new System.Windows.Forms.Label();
            this.labHP1 = new System.Windows.Forms.Label();
            this.labKariText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._picFace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUnit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUnit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picHP2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEN2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEN1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picHP1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._picMessage)).BeginInit();
            this.SuspendLayout();
            // 
            // _picFace
            // 
            this._picFace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this._picFace.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._picFace.Cursor = System.Windows.Forms.Cursors.Default;
            this._picFace.ForeColor = System.Drawing.SystemColors.ControlText;
            this._picFace.Location = new System.Drawing.Point(8, 43);
            this._picFace.Name = "_picFace";
            this._picFace.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._picFace.Size = new System.Drawing.Size(68, 68);
            this._picFace.TabIndex = 15;
            this._picFace.TabStop = false;
            this._picFace.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picFace_MouseDown);
            // 
            // picUnit1
            // 
            this.picUnit1.BackColor = System.Drawing.SystemColors.Window;
            this.picUnit1.Cursor = System.Windows.Forms.Cursors.Default;
            this.picUnit1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.picUnit1.Location = new System.Drawing.Point(8, 4);
            this.picUnit1.Name = "picUnit1";
            this.picUnit1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picUnit1.Size = new System.Drawing.Size(32, 32);
            this.picUnit1.TabIndex = 14;
            this.picUnit1.TabStop = false;
            this.picUnit1.Visible = false;
            // 
            // picUnit2
            // 
            this.picUnit2.BackColor = System.Drawing.SystemColors.Window;
            this.picUnit2.Cursor = System.Windows.Forms.Cursors.Default;
            this.picUnit2.ForeColor = System.Drawing.SystemColors.WindowText;
            this.picUnit2.Location = new System.Drawing.Point(260, 5);
            this.picUnit2.Name = "picUnit2";
            this.picUnit2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picUnit2.Size = new System.Drawing.Size(32, 32);
            this.picUnit2.TabIndex = 13;
            this.picUnit2.TabStop = false;
            this.picUnit2.Visible = false;
            // 
            // txtHP2
            // 
            this.txtHP2.AcceptsReturn = true;
            this.txtHP2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.txtHP2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtHP2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtHP2.Font = new System.Drawing.Font("ＭＳ 明朝", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtHP2.ForeColor = System.Drawing.Color.Black;
            this.txtHP2.Location = new System.Drawing.Point(323, 10);
            this.txtHP2.MaxLength = 0;
            this.txtHP2.Name = "txtHP2";
            this.txtHP2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtHP2.Size = new System.Drawing.Size(88, 13);
            this.txtHP2.TabIndex = 10;
            this.txtHP2.Text = "99999/99999";
            // 
            // picHP2
            // 
            this.picHP2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.picHP2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picHP2.Cursor = System.Windows.Forms.Cursors.Default;
            this.picHP2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.picHP2.Location = new System.Drawing.Point(297, 28);
            this.picHP2.Name = "picHP2";
            this.picHP2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picHP2.Size = new System.Drawing.Size(116, 8);
            this.picHP2.TabIndex = 9;
            this.picHP2.TabStop = false;
            // 
            // picEN2
            // 
            this.picEN2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.picEN2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picEN2.Cursor = System.Windows.Forms.Cursors.Default;
            this.picEN2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.picEN2.Location = new System.Drawing.Point(418, 28);
            this.picEN2.Name = "picEN2";
            this.picEN2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picEN2.Size = new System.Drawing.Size(82, 8);
            this.picEN2.TabIndex = 8;
            this.picEN2.TabStop = false;
            // 
            // txtEN2
            // 
            this.txtEN2.AcceptsReturn = true;
            this.txtEN2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.txtEN2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtEN2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtEN2.Font = new System.Drawing.Font("ＭＳ 明朝", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtEN2.ForeColor = System.Drawing.Color.Black;
            this.txtEN2.Location = new System.Drawing.Point(443, 10);
            this.txtEN2.MaxLength = 0;
            this.txtEN2.Name = "txtEN2";
            this.txtEN2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtEN2.Size = new System.Drawing.Size(57, 13);
            this.txtEN2.TabIndex = 7;
            this.txtEN2.Text = "999/999";
            // 
            // txtEN1
            // 
            this.txtEN1.AcceptsReturn = true;
            this.txtEN1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.txtEN1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtEN1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtEN1.Font = new System.Drawing.Font("ＭＳ 明朝", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtEN1.ForeColor = System.Drawing.Color.Black;
            this.txtEN1.Location = new System.Drawing.Point(192, 10);
            this.txtEN1.MaxLength = 0;
            this.txtEN1.Name = "txtEN1";
            this.txtEN1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtEN1.Size = new System.Drawing.Size(57, 13);
            this.txtEN1.TabIndex = 6;
            this.txtEN1.Text = "999/999";
            // 
            // picEN1
            // 
            this.picEN1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.picEN1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picEN1.Cursor = System.Windows.Forms.Cursors.Default;
            this.picEN1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.picEN1.Location = new System.Drawing.Point(166, 28);
            this.picEN1.Name = "picEN1";
            this.picEN1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picEN1.Size = new System.Drawing.Size(83, 8);
            this.picEN1.TabIndex = 5;
            this.picEN1.TabStop = false;
            // 
            // picHP1
            // 
            this.picHP1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.picHP1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picHP1.Cursor = System.Windows.Forms.Cursors.Default;
            this.picHP1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.picHP1.Location = new System.Drawing.Point(45, 28);
            this.picHP1.Name = "picHP1";
            this.picHP1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picHP1.Size = new System.Drawing.Size(116, 8);
            this.picHP1.TabIndex = 3;
            this.picHP1.TabStop = false;
            // 
            // txtHP1
            // 
            this.txtHP1.AcceptsReturn = true;
            this.txtHP1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.txtHP1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtHP1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtHP1.Font = new System.Drawing.Font("ＭＳ 明朝", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.txtHP1.ForeColor = System.Drawing.Color.Black;
            this.txtHP1.Location = new System.Drawing.Point(72, 10);
            this.txtHP1.MaxLength = 0;
            this.txtHP1.Name = "txtHP1";
            this.txtHP1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtHP1.Size = new System.Drawing.Size(88, 13);
            this.txtHP1.TabIndex = 2;
            this.txtHP1.Text = "99999/99999";
            // 
            // _picMessage
            // 
            this._picMessage.BackColor = System.Drawing.Color.White;
            this._picMessage.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._picMessage.Cursor = System.Windows.Forms.Cursors.Default;
            this._picMessage.Font = new System.Drawing.Font("ＭＳ Ｐ明朝", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._picMessage.ForeColor = System.Drawing.Color.Black;
            this._picMessage.Location = new System.Drawing.Point(84, 42);
            this._picMessage.Name = "_picMessage";
            this._picMessage.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._picMessage.Size = new System.Drawing.Size(417, 70);
            this._picMessage.TabIndex = 0;
            this._picMessage.TabStop = false;
            this._picMessage.DoubleClick += new System.EventHandler(this.picMessage_DoubleClick);
            this._picMessage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picMessage_MouseDown);
            // 
            // labHP2
            // 
            this.labHP2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.labHP2.Cursor = System.Windows.Forms.Cursors.Default;
            this.labHP2.ForeColor = System.Drawing.Color.Black;
            this.labHP2.Location = new System.Drawing.Point(296, 8);
            this.labHP2.Name = "labHP2";
            this.labHP2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labHP2.Size = new System.Drawing.Size(22, 17);
            this.labHP2.TabIndex = 12;
            this.labHP2.Text = "HP";
            // 
            // labEN2
            // 
            this.labEN2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.labEN2.Cursor = System.Windows.Forms.Cursors.Default;
            this.labEN2.ForeColor = System.Drawing.Color.Black;
            this.labEN2.Location = new System.Drawing.Point(417, 8);
            this.labEN2.Name = "labEN2";
            this.labEN2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labEN2.Size = new System.Drawing.Size(22, 17);
            this.labEN2.TabIndex = 11;
            this.labEN2.Text = "EN";
            // 
            // labEN1
            // 
            this.labEN1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.labEN1.Cursor = System.Windows.Forms.Cursors.Default;
            this.labEN1.ForeColor = System.Drawing.Color.Black;
            this.labEN1.Location = new System.Drawing.Point(165, 8);
            this.labEN1.Name = "labEN1";
            this.labEN1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labEN1.Size = new System.Drawing.Size(22, 17);
            this.labEN1.TabIndex = 4;
            this.labEN1.Text = "EN";
            // 
            // labHP1
            // 
            this.labHP1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.labHP1.Cursor = System.Windows.Forms.Cursors.Default;
            this.labHP1.ForeColor = System.Drawing.Color.Black;
            this.labHP1.Location = new System.Drawing.Point(44, 8);
            this.labHP1.Name = "labHP1";
            this.labHP1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labHP1.Size = new System.Drawing.Size(22, 17);
            this.labHP1.TabIndex = 1;
            this.labHP1.Text = "HP";
            // 
            // labKariText
            // 
            this.labKariText.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.labKariText.Location = new System.Drawing.Point(84, 42);
            this.labKariText.Name = "labKariText";
            this.labKariText.Size = new System.Drawing.Size(416, 70);
            this.labKariText.TabIndex = 16;
            this.labKariText.DoubleClick += new System.EventHandler(this.labKariText_DoubleClick);
            this.labKariText.MouseDown += new System.Windows.Forms.MouseEventHandler(this.labKariText_MouseDown);
            // 
            // frmMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(508, 118);
            this.Controls.Add(this.labKariText);
            this.Controls.Add(this._picFace);
            this.Controls.Add(this.picUnit1);
            this.Controls.Add(this.picUnit2);
            this.Controls.Add(this.txtHP2);
            this.Controls.Add(this.picHP2);
            this.Controls.Add(this.picEN2);
            this.Controls.Add(this.txtEN2);
            this.Controls.Add(this.txtEN1);
            this.Controls.Add(this.picEN1);
            this.Controls.Add(this.picHP1);
            this.Controls.Add(this.txtHP1);
            this.Controls.Add(this._picMessage);
            this.Controls.Add(this.labHP2);
            this.Controls.Add(this.labEN2);
            this.Controls.Add(this.labEN1);
            this.Controls.Add(this.labHP1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("ＭＳ Ｐ明朝", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Location = new System.Drawing.Point(93, 101);
            this.MaximizeBox = false;
            this.Name = "frmMessage";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "メッセージ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMessage_FormClosing);
            this.Click += new System.EventHandler(this.frmMessage_Click);
            this.DoubleClick += new System.EventHandler(this.frmMessage_DoubleClick);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMessage_KeyDown);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmMessage_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this._picFace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUnit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUnit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picHP2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEN2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEN1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picHP1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._picMessage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Label labKariText;
        /* TODO ERROR: Skipped EndRegionDirectiveTrivia */
    }
}