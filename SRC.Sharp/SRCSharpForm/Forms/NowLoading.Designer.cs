using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace SRCSharpForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmNowLoading));
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.picBar = new System.Windows.Forms.PictureBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.picBar)).BeginInit();
            this.SuspendLayout();
            // 
            // picBar
            // 
            this.picBar.BackColor = System.Drawing.Color.White;
            this.picBar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picBar.Cursor = System.Windows.Forms.Cursors.Default;
            this.picBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(128)))));
            this.picBar.Location = new System.Drawing.Point(16, 56);
            this.picBar.Name = "picBar";
            this.picBar.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.picBar.Size = new System.Drawing.Size(183, 13);
            this.picBar.TabIndex = 1;
            this.picBar.TabStop = false;
            // 
            // Label1
            // 
            this.Label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label1.Font = new System.Drawing.Font("Times New Roman", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.Label1.ForeColor = System.Drawing.Color.Black;
            this.Label1.Location = new System.Drawing.Point(24, 16);
            this.Label1.Name = "Label1";
            this.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label1.Size = new System.Drawing.Size(169, 33);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "Now Loading ...";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(16, 52);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(186, 23);
            this.progressBar.TabIndex = 2;
            // 
            // frmNowLoading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(214, 88);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.picBar);
            this.Controls.Add(this.Label1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(76, 107);
            this.MaximizeBox = false;
            this.Name = "frmNowLoading";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "SRC";
            ((System.ComponentModel.ISupportInitialize)(this.picBar)).EndInit();
            this.ResumeLayout(false);

        }

        private ProgressBar progressBar;
        /* TODO ERROR: Skipped EndRegionDirectiveTrivia */
    }
}