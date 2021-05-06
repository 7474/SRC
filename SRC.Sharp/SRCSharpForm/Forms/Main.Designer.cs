using Microsoft.VisualBasic.CompilerServices;
using SRCSharpForm.Lib;
using System.Diagnostics;
using System.Windows.Forms;

namespace SRCSharpForm
{
    [DesignerGenerated()]
    internal partial class frmMain
    {
        /* TODO ERROR: Skipped RegionDirectiveTrivia */
        [DebuggerNonUserCode()]
        public frmMain() : base()
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
        public ContextMenuStrip mnuUnitCommand;
        public ContextMenuStrip mnuMapCommand;
        public PictureBox _picStretchedTmp_1;
        public PictureBox _picStretchedTmp_0;
        public Panel _picMain_1;
        public PictureBox _picBuf_0;
        public PictureBox _picTmp32_2;
        public PictureBox _picTmp32_1;
        public PictureBox picFace;
        public PictureBox _picTmp32_0;
        public PictureBox picMaskedBack;
        public PictureBox picMask2;
        public PictureBox picNeautral;
        public PictureBox picEnemy;
        public PictureBox picUnit;
        public PictureBox picPilotStatus;
        public PictureBox picUnitStatus;
        public PictureBox picUnitBitmap;
        public PictureBox picMask;
        public PictureBox picTmp;
        public PictureBox picBack;
        public Panel _picMain_0;
        // メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
        // Windows フォーム デザイナを使って変更できます。
        // コード エディタを使用して、変更しないでください。
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.mnuUnitCommand = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuMapCommand = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._picStretchedTmp_1 = new System.Windows.Forms.PictureBox();
            this._picStretchedTmp_0 = new System.Windows.Forms.PictureBox();
            this._picMain_1 = new System.Windows.Forms.Panel();
            this._picBuf_0 = new System.Windows.Forms.PictureBox();
            this._picTmp32_2 = new System.Windows.Forms.PictureBox();
            this._picTmp32_1 = new System.Windows.Forms.PictureBox();
            this.picFace = new System.Windows.Forms.PictureBox();
            this._picTmp32_0 = new System.Windows.Forms.PictureBox();
            this.picMaskedBack = new System.Windows.Forms.PictureBox();
            this.picMask2 = new System.Windows.Forms.PictureBox();
            this.picNeautral = new System.Windows.Forms.PictureBox();
            this.picEnemy = new System.Windows.Forms.PictureBox();
            this.picUnit = new System.Windows.Forms.PictureBox();
            this.picPilotStatus = new System.Windows.Forms.PictureBox();
            this.picUnitStatus = new System.Windows.Forms.PictureBox();
            this.picUnitBitmap = new System.Windows.Forms.PictureBox();
            this.HScrollBar = new SRCSharpForm.Lib.SrcHScrollBar();
            this.VScrollBar = new SRCSharpForm.Lib.SrcVScrollBar();
            this.picMask = new System.Windows.Forms.PictureBox();
            this.picTmp = new System.Windows.Forms.PictureBox();
            this.picBack = new System.Windows.Forms.PictureBox();
            this._picMain_0 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this._picStretchedTmp_1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._picStretchedTmp_0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._picBuf_0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._picTmp32_2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._picTmp32_1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._picTmp32_0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMaskedBack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMask2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNeautral)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEnemy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUnit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPilotStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUnitStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUnitBitmap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMask)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTmp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBack)).BeginInit();
            this.SuspendLayout();
            // 
            // mnuUnitCommand
            // 
            this.mnuUnitCommand.Name = "mnuUnitCommand";
            this.mnuUnitCommand.Size = new System.Drawing.Size(61, 4);
            this.mnuUnitCommand.Text = "ユニットコマンド";
            this.mnuUnitCommand.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mnuUnitCommand_MouseClick);
            // 
            // mnuMapCommand
            // 
            this.mnuMapCommand.Name = "mnuMapCommand";
            this.mnuMapCommand.Size = new System.Drawing.Size(61, 4);
            this.mnuMapCommand.Text = "マップコマンド";
            this.mnuMapCommand.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mnuMapCommand_MouseClick);
            // 
            // _picStretchedTmp_1
            // 
            this._picStretchedTmp_1.BackColor = System.Drawing.Color.White;
            this._picStretchedTmp_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._picStretchedTmp_1.ForeColor = System.Drawing.Color.Black;
            this._picStretchedTmp_1.Location = new System.Drawing.Point(288, 224);
            this._picStretchedTmp_1.Name = "_picStretchedTmp_1";
            this._picStretchedTmp_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._picStretchedTmp_1.Size = new System.Drawing.Size(32, 32);
            this._picStretchedTmp_1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this._picStretchedTmp_1.TabIndex = 21;
            this._picStretchedTmp_1.TabStop = false;
            this._picStretchedTmp_1.Visible = false;
            // 
            // _picStretchedTmp_0
            // 
            this._picStretchedTmp_0.BackColor = System.Drawing.Color.White;
            this._picStretchedTmp_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._picStretchedTmp_0.ForeColor = System.Drawing.Color.Black;
            this._picStretchedTmp_0.Location = new System.Drawing.Point(288, 176);
            this._picStretchedTmp_0.Name = "_picStretchedTmp_0";
            this._picStretchedTmp_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._picStretchedTmp_0.Size = new System.Drawing.Size(32, 32);
            this._picStretchedTmp_0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this._picStretchedTmp_0.TabIndex = 20;
            this._picStretchedTmp_0.TabStop = false;
            this._picStretchedTmp_0.Visible = false;
            // 
            // _picMain_1
            // 
            this._picMain_1.BackColor = System.Drawing.Color.Black;
            this._picMain_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._picMain_1.Font = new System.Drawing.Font("ＭＳ Ｐ明朝", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._picMain_1.ForeColor = System.Drawing.Color.White;
            this._picMain_1.Location = new System.Drawing.Point(96, 32);
            this._picMain_1.Name = "_picMain_1";
            this._picMain_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._picMain_1.Size = new System.Drawing.Size(81, 32);
            this._picMain_1.TabIndex = 13;
            this._picMain_1.TabStop = true;
            this._picMain_1.Visible = false;
            // 
            // _picBuf_0
            // 
            this._picBuf_0.BackColor = System.Drawing.Color.White;
            this._picBuf_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._picBuf_0.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._picBuf_0.ForeColor = System.Drawing.Color.Black;
            this._picBuf_0.Location = new System.Drawing.Point(224, 72);
            this._picBuf_0.Name = "_picBuf_0";
            this._picBuf_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._picBuf_0.Size = new System.Drawing.Size(32, 32);
            this._picBuf_0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this._picBuf_0.TabIndex = 19;
            this._picBuf_0.TabStop = false;
            this._picBuf_0.Visible = false;
            // 
            // _picTmp32_2
            // 
            this._picTmp32_2.BackColor = System.Drawing.Color.White;
            this._picTmp32_2.Cursor = System.Windows.Forms.Cursors.Default;
            this._picTmp32_2.ForeColor = System.Drawing.Color.Black;
            this._picTmp32_2.Location = new System.Drawing.Point(240, 224);
            this._picTmp32_2.Name = "_picTmp32_2";
            this._picTmp32_2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._picTmp32_2.Size = new System.Drawing.Size(32, 32);
            this._picTmp32_2.TabIndex = 18;
            this._picTmp32_2.TabStop = false;
            this._picTmp32_2.Visible = false;
            // 
            // _picTmp32_1
            // 
            this._picTmp32_1.BackColor = System.Drawing.Color.White;
            this._picTmp32_1.Cursor = System.Windows.Forms.Cursors.Default;
            this._picTmp32_1.ForeColor = System.Drawing.Color.Black;
            this._picTmp32_1.Location = new System.Drawing.Point(192, 264);
            this._picTmp32_1.Name = "_picTmp32_1";
            this._picTmp32_1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._picTmp32_1.Size = new System.Drawing.Size(32, 32);
            this._picTmp32_1.TabIndex = 17;
            this._picTmp32_1.TabStop = false;
            this._picTmp32_1.Visible = false;
            // 
            // picFace
            // 
            this.picFace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.picFace.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picFace.Cursor = System.Windows.Forms.Cursors.Default;
            this.picFace.ForeColor = System.Drawing.SystemColors.ControlText;
            this.picFace.Location = new System.Drawing.Point(8, 192);
            this.picFace.Name = "picFace";
            this.picFace.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picFace.Size = new System.Drawing.Size(68, 68);
            this.picFace.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picFace.TabIndex = 16;
            this.picFace.TabStop = false;
            this.picFace.Click += new System.EventHandler(this.picFace_Click);
            // 
            // _picTmp32_0
            // 
            this._picTmp32_0.BackColor = System.Drawing.Color.White;
            this._picTmp32_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._picTmp32_0.ForeColor = System.Drawing.Color.Black;
            this._picTmp32_0.Location = new System.Drawing.Point(192, 224);
            this._picTmp32_0.Name = "_picTmp32_0";
            this._picTmp32_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._picTmp32_0.Size = new System.Drawing.Size(32, 32);
            this._picTmp32_0.TabIndex = 15;
            this._picTmp32_0.TabStop = false;
            this._picTmp32_0.Visible = false;
            // 
            // picMaskedBack
            // 
            this.picMaskedBack.BackColor = System.Drawing.Color.Black;
            this.picMaskedBack.Cursor = System.Windows.Forms.Cursors.Default;
            this.picMaskedBack.ForeColor = System.Drawing.Color.White;
            this.picMaskedBack.Location = new System.Drawing.Point(288, 32);
            this.picMaskedBack.Name = "picMaskedBack";
            this.picMaskedBack.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picMaskedBack.Size = new System.Drawing.Size(81, 32);
            this.picMaskedBack.TabIndex = 14;
            this.picMaskedBack.TabStop = false;
            this.picMaskedBack.Visible = false;
            // 
            // picMask2
            // 
            this.picMask2.BackColor = System.Drawing.SystemColors.Control;
            this.picMask2.Cursor = System.Windows.Forms.Cursors.Default;
            this.picMask2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.picMask2.Location = new System.Drawing.Point(8, 133);
            this.picMask2.Name = "picMask2";
            this.picMask2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picMask2.Size = new System.Drawing.Size(32, 32);
            this.picMask2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picMask2.TabIndex = 12;
            this.picMask2.TabStop = false;
            this.picMask2.Visible = false;
            // 
            // picNeautral
            // 
            this.picNeautral.BackColor = System.Drawing.SystemColors.Control;
            this.picNeautral.Cursor = System.Windows.Forms.Cursors.Default;
            this.picNeautral.ForeColor = System.Drawing.SystemColors.ControlText;
            this.picNeautral.Image = ((System.Drawing.Image)(resources.GetObject("picNeautral.Image")));
            this.picNeautral.Location = new System.Drawing.Point(176, 140);
            this.picNeautral.Name = "picNeautral";
            this.picNeautral.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picNeautral.Size = new System.Drawing.Size(32, 32);
            this.picNeautral.TabIndex = 11;
            this.picNeautral.TabStop = false;
            this.picNeautral.Visible = false;
            // 
            // picEnemy
            // 
            this.picEnemy.BackColor = System.Drawing.SystemColors.Control;
            this.picEnemy.Cursor = System.Windows.Forms.Cursors.Default;
            this.picEnemy.ForeColor = System.Drawing.SystemColors.ControlText;
            this.picEnemy.Image = ((System.Drawing.Image)(resources.GetObject("picEnemy.Image")));
            this.picEnemy.Location = new System.Drawing.Point(124, 140);
            this.picEnemy.Name = "picEnemy";
            this.picEnemy.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picEnemy.Size = new System.Drawing.Size(32, 32);
            this.picEnemy.TabIndex = 10;
            this.picEnemy.TabStop = false;
            this.picEnemy.Visible = false;
            // 
            // picUnit
            // 
            this.picUnit.BackColor = System.Drawing.SystemColors.Control;
            this.picUnit.Cursor = System.Windows.Forms.Cursors.Default;
            this.picUnit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.picUnit.Image = ((System.Drawing.Image)(resources.GetObject("picUnit.Image")));
            this.picUnit.Location = new System.Drawing.Point(76, 140);
            this.picUnit.Name = "picUnit";
            this.picUnit.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picUnit.Size = new System.Drawing.Size(32, 32);
            this.picUnit.TabIndex = 9;
            this.picUnit.TabStop = false;
            this.picUnit.Visible = false;
            // 
            // picPilotStatus
            // 
            this.picPilotStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.picPilotStatus.Cursor = System.Windows.Forms.Cursors.Default;
            this.picPilotStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.picPilotStatus.Location = new System.Drawing.Point(416, 224);
            this.picPilotStatus.Name = "picPilotStatus";
            this.picPilotStatus.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picPilotStatus.Size = new System.Drawing.Size(81, 33);
            this.picPilotStatus.TabIndex = 8;
            this.picPilotStatus.TabStop = false;
            // 
            // picUnitStatus
            // 
            this.picUnitStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.picUnitStatus.Cursor = System.Windows.Forms.Cursors.Default;
            this.picUnitStatus.Font = new System.Drawing.Font("ＭＳ 明朝", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.picUnitStatus.ForeColor = System.Drawing.Color.Black;
            this.picUnitStatus.Location = new System.Drawing.Point(376, 240);
            this.picUnitStatus.Name = "picUnitStatus";
            this.picUnitStatus.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picUnitStatus.Size = new System.Drawing.Size(81, 33);
            this.picUnitStatus.TabIndex = 7;
            this.picUnitStatus.TabStop = false;
            // 
            // picUnitBitmap
            // 
            this.picUnitBitmap.BackColor = System.Drawing.Color.White;
            this.picUnitBitmap.Cursor = System.Windows.Forms.Cursors.Default;
            this.picUnitBitmap.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.picUnitBitmap.ForeColor = System.Drawing.Color.Black;
            this.picUnitBitmap.Location = new System.Drawing.Point(88, 192);
            this.picUnitBitmap.Name = "picUnitBitmap";
            this.picUnitBitmap.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picUnitBitmap.Size = new System.Drawing.Size(32, 96);
            this.picUnitBitmap.TabIndex = 6;
            this.picUnitBitmap.TabStop = false;
            this.picUnitBitmap.Visible = false;
            // 
            // HScrollBar
            // 
            this.HScrollBar.Cursor = System.Windows.Forms.Cursors.Default;
            this.HScrollBar.Enabled = false;
            this.HScrollBar.LargeChange = 4;
            this.HScrollBar.Location = new System.Drawing.Point(60, 92);
            this.HScrollBar.Maximum = 23;
            this.HScrollBar.Minimum = 1;
            this.HScrollBar.Name = "HScrollBar";
            this.HScrollBar.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.HScrollBar.Size = new System.Drawing.Size(49, 17);
            this.HScrollBar.TabIndex = 5;
            this.HScrollBar.Value = 1;
            this.HScrollBar.ValueChanged += new System.EventHandler(this.HScrollBar_ValueChanged);
            // 
            // VScrollBar
            // 
            this.VScrollBar.Cursor = System.Windows.Forms.Cursors.Default;
            this.VScrollBar.Enabled = false;
            this.VScrollBar.LargeChange = 4;
            this.VScrollBar.Location = new System.Drawing.Point(124, 84);
            this.VScrollBar.Maximum = 23;
            this.VScrollBar.Minimum = 1;
            this.VScrollBar.Name = "VScrollBar";
            this.VScrollBar.Size = new System.Drawing.Size(17, 49);
            this.VScrollBar.TabIndex = 4;
            this.VScrollBar.Value = 1;
            this.VScrollBar.ValueChanged += new System.EventHandler(this.VScrollBar_ValueChanged);
            // 
            // picMask
            // 
            this.picMask.BackColor = System.Drawing.SystemColors.Control;
            this.picMask.Cursor = System.Windows.Forms.Cursors.Default;
            this.picMask.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.picMask.ForeColor = System.Drawing.SystemColors.ControlText;
            this.picMask.Location = new System.Drawing.Point(8, 84);
            this.picMask.Name = "picMask";
            this.picMask.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picMask.Size = new System.Drawing.Size(32, 32);
            this.picMask.TabIndex = 3;
            this.picMask.TabStop = false;
            this.picMask.Visible = false;
            // 
            // picTmp
            // 
            this.picTmp.BackColor = System.Drawing.Color.White;
            this.picTmp.Cursor = System.Windows.Forms.Cursors.Default;
            this.picTmp.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.picTmp.ForeColor = System.Drawing.Color.Black;
            this.picTmp.Location = new System.Drawing.Point(240, 176);
            this.picTmp.Name = "picTmp";
            this.picTmp.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picTmp.Size = new System.Drawing.Size(32, 32);
            this.picTmp.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picTmp.TabIndex = 2;
            this.picTmp.TabStop = false;
            this.picTmp.Visible = false;
            // 
            // picBack
            // 
            this.picBack.BackColor = System.Drawing.Color.Black;
            this.picBack.Cursor = System.Windows.Forms.Cursors.Default;
            this.picBack.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.picBack.ForeColor = System.Drawing.SystemColors.ControlText;
            this.picBack.Location = new System.Drawing.Point(384, 32);
            this.picBack.Name = "picBack";
            this.picBack.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picBack.Size = new System.Drawing.Size(81, 32);
            this.picBack.TabIndex = 1;
            this.picBack.TabStop = false;
            this.picBack.Visible = false;
            // 
            // _picMain_0
            // 
            this._picMain_0.BackColor = System.Drawing.Color.White;
            this._picMain_0.Cursor = System.Windows.Forms.Cursors.Default;
            this._picMain_0.Font = new System.Drawing.Font("ＭＳ Ｐ明朝", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._picMain_0.ForeColor = System.Drawing.Color.White;
            this._picMain_0.Location = new System.Drawing.Point(8, 32);
            this._picMain_0.Name = "_picMain_0";
            this._picMain_0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._picMain_0.Size = new System.Drawing.Size(81, 32);
            this._picMain_0.TabIndex = 0;
            this._picMain_0.TabStop = true;
            this._picMain_0.DoubleClick += new System.EventHandler(this.picMain_DoubleClick);
            this._picMain_0.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picMain_MouseClick);
            this._picMain_0.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picMain_MouseDown);
            this._picMain_0.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picMain_MouseMove);
            this._picMain_0.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picMain_MouseUp);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(508, 318);
            this.Controls.Add(this._picStretchedTmp_1);
            this.Controls.Add(this._picStretchedTmp_0);
            this.Controls.Add(this._picMain_1);
            this.Controls.Add(this._picBuf_0);
            this.Controls.Add(this._picTmp32_2);
            this.Controls.Add(this._picTmp32_1);
            this.Controls.Add(this.picFace);
            this.Controls.Add(this._picTmp32_0);
            this.Controls.Add(this.picMaskedBack);
            this.Controls.Add(this.picMask2);
            this.Controls.Add(this.picNeautral);
            this.Controls.Add(this.picEnemy);
            this.Controls.Add(this.picUnit);
            this.Controls.Add(this.picPilotStatus);
            this.Controls.Add(this.picUnitStatus);
            this.Controls.Add(this.picUnitBitmap);
            this.Controls.Add(this.HScrollBar);
            this.Controls.Add(this.VScrollBar);
            this.Controls.Add(this.picMask);
            this.Controls.Add(this.picTmp);
            this.Controls.Add(this.picBack);
            this.Controls.Add(this._picMain_0);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("ＭＳ 明朝", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Location = new System.Drawing.Point(81, 218);
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "SRC#";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseMove);
            ((System.ComponentModel.ISupportInitialize)(this._picStretchedTmp_1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._picStretchedTmp_0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._picBuf_0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._picTmp32_2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._picTmp32_1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picFace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._picTmp32_0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMaskedBack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMask2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNeautral)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEnemy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUnit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPilotStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUnitStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUnitBitmap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMask)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTmp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBack)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public SrcHScrollBar HScrollBar;
        public SrcVScrollBar VScrollBar;
        /* TODO ERROR: Skipped EndRegionDirectiveTrivia */
    }
}
