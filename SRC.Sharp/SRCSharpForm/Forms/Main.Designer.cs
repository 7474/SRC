using SRCSharpForm.Lib;
using System.Diagnostics;
using System.Windows.Forms;

namespace SRCSharpForm
{
    internal partial class frmMain
    {
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
        public PictureBox picFace;
        public PictureBox picNeautral;
        public PictureBox picEnemy;
        public PictureBox picUnit;
        public PictureBox picPilotStatus;
        public PictureBox picUnitStatus;
        public PictureBox picMain;
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
            this.picFace = new System.Windows.Forms.PictureBox();
            this.picNeautral = new System.Windows.Forms.PictureBox();
            this.picEnemy = new System.Windows.Forms.PictureBox();
            this.picUnit = new System.Windows.Forms.PictureBox();
            this.picPilotStatus = new System.Windows.Forms.PictureBox();
            this.picUnitStatus = new System.Windows.Forms.PictureBox();
            this.HScrollBar = new SRCSharpForm.Lib.SrcHScrollBar();
            this.VScrollBar = new SRCSharpForm.Lib.SrcVScrollBar();
            this.picMain = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picFace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNeautral)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEnemy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUnit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPilotStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUnitStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMain)).BeginInit();
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
            // picFace
            // 
            this.picFace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.picFace.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picFace.Cursor = System.Windows.Forms.Cursors.Default;
            this.picFace.ForeColor = System.Drawing.SystemColors.ControlText;
            this.picFace.Location = new System.Drawing.Point(230, 12);
            this.picFace.Name = "picFace";
            this.picFace.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picFace.Size = new System.Drawing.Size(68, 68);
            this.picFace.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picFace.TabIndex = 16;
            this.picFace.TabStop = false;
            this.picFace.Click += new System.EventHandler(this.picFace_Click);
            // 
            // picNeautral
            // 
            this.picNeautral.BackColor = System.Drawing.SystemColors.Control;
            this.picNeautral.Cursor = System.Windows.Forms.Cursors.Default;
            this.picNeautral.ForeColor = System.Drawing.SystemColors.ControlText;
            this.picNeautral.Image = ((System.Drawing.Image)(resources.GetObject("picNeautral.Image")));
            this.picNeautral.Location = new System.Drawing.Point(192, 12);
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
            this.picEnemy.Location = new System.Drawing.Point(154, 12);
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
            this.picUnit.Location = new System.Drawing.Point(116, 12);
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
            this.picPilotStatus.Location = new System.Drawing.Point(304, 12);
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
            this.picUnitStatus.Location = new System.Drawing.Point(304, 51);
            this.picUnitStatus.Name = "picUnitStatus";
            this.picUnitStatus.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picUnitStatus.Size = new System.Drawing.Size(81, 33);
            this.picUnitStatus.TabIndex = 7;
            this.picUnitStatus.TabStop = false;
            // 
            // HScrollBar
            // 
            this.HScrollBar.Cursor = System.Windows.Forms.Cursors.Default;
            this.HScrollBar.Enabled = false;
            this.HScrollBar.LargeChange = 4;
            this.HScrollBar.Location = new System.Drawing.Point(12, 47);
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
            this.VScrollBar.Location = new System.Drawing.Point(96, 12);
            this.VScrollBar.Maximum = 23;
            this.VScrollBar.Minimum = 1;
            this.VScrollBar.Name = "VScrollBar";
            this.VScrollBar.Size = new System.Drawing.Size(17, 49);
            this.VScrollBar.TabIndex = 4;
            this.VScrollBar.Value = 1;
            this.VScrollBar.ValueChanged += new System.EventHandler(this.VScrollBar_ValueChanged);
            // 
            // picMain
            // 
            this.picMain.BackColor = System.Drawing.Color.White;
            this.picMain.Cursor = System.Windows.Forms.Cursors.Default;
            this.picMain.Font = new System.Drawing.Font("ＭＳ Ｐ明朝", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.picMain.ForeColor = System.Drawing.Color.White;
            this.picMain.Location = new System.Drawing.Point(12, 12);
            this.picMain.Name = "picMain";
            this.picMain.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picMain.Size = new System.Drawing.Size(81, 32);
            this.picMain.TabIndex = 0;
            this.picMain.TabStop = false;
            this.picMain.DoubleClick += new System.EventHandler(this.picMain_DoubleClick);
            this.picMain.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picMain_MouseClick);
            this.picMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picMain_MouseDown);
            this.picMain.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picMain_MouseMove);
            this.picMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picMain_MouseUp);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(508, 318);
            this.Controls.Add(this.picFace);
            this.Controls.Add(this.picNeautral);
            this.Controls.Add(this.picEnemy);
            this.Controls.Add(this.picUnit);
            this.Controls.Add(this.picPilotStatus);
            this.Controls.Add(this.picUnitStatus);
            this.Controls.Add(this.HScrollBar);
            this.Controls.Add(this.VScrollBar);
            this.Controls.Add(this.picMain);
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
            ((System.ComponentModel.ISupportInitialize)(this.picFace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picNeautral)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picEnemy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUnit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPilotStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picUnitStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMain)).EndInit();
            this.ResumeLayout(false);

        }

        public SrcHScrollBar HScrollBar;
        public SrcVScrollBar VScrollBar;
    }
}
