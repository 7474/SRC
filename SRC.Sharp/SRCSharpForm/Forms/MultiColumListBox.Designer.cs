using System.Diagnostics;
using System.Windows.Forms;

namespace SRCSharpForm
{
    internal partial class frmMultiColumnListBox
    {
        [DebuggerNonUserCode()]
        public frmMultiColumnListBox() : base()
        {
            // この呼び出しは、Windows フォーム デザイナで必要です。
            InitializeComponent();
            _lstItems.Name = "lstItems";
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
        private ListBox _lstItems;
        public Label labCaption;
        // メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
        // Windows フォーム デザイナを使って変更できます。
        // コード エディタを使用して、変更しないでください。
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMultiColumnListBox));
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._lstItems = new System.Windows.Forms.ListBox();
            this.labCaption = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _lstItems
            // 
            this._lstItems.BackColor = System.Drawing.Color.White;
            this._lstItems.ColumnWidth = 164;
            this._lstItems.Cursor = System.Windows.Forms.Cursors.Default;
            this._lstItems.Font = new System.Drawing.Font("ＭＳ 明朝", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._lstItems.ForeColor = System.Drawing.Color.Black;
            this._lstItems.ItemHeight = 16;
            this._lstItems.Location = new System.Drawing.Point(8, 8);
            this._lstItems.MultiColumn = true;
            this._lstItems.Name = "_lstItems";
            this._lstItems.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._lstItems.Size = new System.Drawing.Size(654, 404);
            this._lstItems.TabIndex = 0;
            this._lstItems.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstItems_MouseDown);
            this._lstItems.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lstItems_MouseMove);
            // 
            // labCaption
            // 
            this.labCaption.BackColor = System.Drawing.Color.White;
            this.labCaption.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labCaption.Cursor = System.Windows.Forms.Cursors.Default;
            this.labCaption.Font = new System.Drawing.Font("ＭＳ 明朝", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labCaption.ForeColor = System.Drawing.Color.Black;
            this.labCaption.Location = new System.Drawing.Point(8, 416);
            this.labCaption.Name = "labCaption";
            this.labCaption.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labCaption.Size = new System.Drawing.Size(654, 23);
            this.labCaption.TabIndex = 1;
            // 
            // frmMultiColumnListBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(670, 446);
            this.Controls.Add(this._lstItems);
            this.Controls.Add(this.labCaption);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(72, 116);
            this.MaximizeBox = false;
            this.Name = "frmMultiColumnListBox";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MultiColumListBox";
            this.TopMost = true;
            this.Activated += new System.EventHandler(this.frmMultiColumnListBox_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMultiColumnListBox_FormClosing);
            this.Load += new System.EventHandler(this.frmMultiColumnListBox_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmMultiColumnListBox_MouseDown);
            this.ResumeLayout(false);

        }
    }
}
