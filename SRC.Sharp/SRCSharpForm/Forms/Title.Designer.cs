using Microsoft.VisualBasic.CompilerServices;
using System.Diagnostics;
using System.Windows.Forms;

namespace SRCSharpForm
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
        public PictureBox Image1;
        public Label labOrgAuthor;
        public Label labVersion;
        public GroupBox Frame1;
        public Label labLicense;
        // メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
        // Windows フォーム デザイナを使って変更できます。
        // コード エディタを使用して、変更しないでください。
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTitle));
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.Frame1 = new System.Windows.Forms.GroupBox();
            this.labAuthor = new System.Windows.Forms.Label();
            this.labTitle = new System.Windows.Forms.Label();
            this.Image1 = new System.Windows.Forms.PictureBox();
            this.labOrgAuthor = new System.Windows.Forms.Label();
            this.labVersion = new System.Windows.Forms.Label();
            this.labLicense = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.Frame1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Image1)).BeginInit();
            this.SuspendLayout();
            // 
            // Frame1
            // 
            this.Frame1.BackColor = System.Drawing.SystemColors.Control;
            this.Frame1.Controls.Add(this.label1);
            this.Frame1.Controls.Add(this.labAuthor);
            this.Frame1.Controls.Add(this.labTitle);
            this.Frame1.Controls.Add(this.Image1);
            this.Frame1.Controls.Add(this.labOrgAuthor);
            this.Frame1.Controls.Add(this.labVersion);
            this.Frame1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame1.Location = new System.Drawing.Point(24, 8);
            this.Frame1.Name = "Frame1";
            this.Frame1.Padding = new System.Windows.Forms.Padding(0);
            this.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Frame1.Size = new System.Drawing.Size(337, 201);
            this.Frame1.TabIndex = 1;
            this.Frame1.TabStop = false;
            // 
            // labAuthor
            // 
            this.labAuthor.AutoSize = true;
            this.labAuthor.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labAuthor.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labAuthor.Location = new System.Drawing.Point(257, 156);
            this.labAuthor.Name = "labAuthor";
            this.labAuthor.Size = new System.Drawing.Size(64, 17);
            this.labAuthor.TabIndex = 5;
            this.labAuthor.Text = "koudenpa";
            this.labAuthor.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labTitle
            // 
            this.labTitle.AutoSize = true;
            this.labTitle.BackColor = System.Drawing.SystemColors.Control;
            this.labTitle.Font = new System.Drawing.Font("Times New Roman", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labTitle.Location = new System.Drawing.Point(118, 56);
            this.labTitle.Name = "labTitle";
            this.labTitle.Size = new System.Drawing.Size(181, 31);
            this.labTitle.TabIndex = 4;
            this.labTitle.Text = "SRC#TestForm";
            // 
            // Image1
            // 
            this.Image1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Image1.Image = ((System.Drawing.Image)(resources.GetObject("Image1.Image")));
            this.Image1.Location = new System.Drawing.Point(16, 56);
            this.Image1.Name = "Image1";
            this.Image1.Size = new System.Drawing.Size(96, 96);
            this.Image1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Image1.TabIndex = 0;
            this.Image1.TabStop = false;
            // 
            // labOrgAuthor
            // 
            this.labOrgAuthor.BackColor = System.Drawing.SystemColors.Control;
            this.labOrgAuthor.Cursor = System.Windows.Forms.Cursors.Default;
            this.labOrgAuthor.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labOrgAuthor.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labOrgAuthor.Location = new System.Drawing.Point(16, 173);
            this.labOrgAuthor.Name = "labOrgAuthor";
            this.labOrgAuthor.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labOrgAuthor.Size = new System.Drawing.Size(305, 17);
            this.labOrgAuthor.TabIndex = 3;
            this.labOrgAuthor.Text = "SRC by Kei Sakamoto / Inui Tetsuyuki";
            this.labOrgAuthor.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labVersion
            // 
            this.labVersion.BackColor = System.Drawing.SystemColors.Control;
            this.labVersion.Cursor = System.Windows.Forms.Cursors.Default;
            this.labVersion.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labVersion.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labVersion.Location = new System.Drawing.Point(144, 127);
            this.labVersion.Name = "labVersion";
            this.labVersion.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labVersion.Size = new System.Drawing.Size(177, 25);
            this.labVersion.TabIndex = 2;
            this.labVersion.Text = "Ver x.x.x";
            this.labVersion.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labLicense
            // 
            this.labLicense.BackColor = System.Drawing.SystemColors.Control;
            this.labLicense.Cursor = System.Windows.Forms.Cursors.Default;
            this.labLicense.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labLicense.Location = new System.Drawing.Point(8, 216);
            this.labLicense.Name = "labLicense";
            this.labLicense.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labLicense.Size = new System.Drawing.Size(369, 17);
            this.labLicense.TabIndex = 4;
            this.labLicense.Text = "This program is distributed under the terms of GPL";
            this.labLicense.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Rockwell Nova", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(118, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(212, 18);
            this.label1.TabIndex = 6;
            this.label1.Text = "- Simulation RPG Construction -";
            // 
            // frmTitle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(386, 233);
            this.Controls.Add(this.Frame1);
            this.Controls.Add(this.labLicense);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(180, 197);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTitle";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "SRC";
            this.Load += new System.EventHandler(this.frmTitle_Load);
            this.Frame1.ResumeLayout(false);
            this.Frame1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Image1)).EndInit();
            this.ResumeLayout(false);

        }

        private Label labAuthor;
        private Label labTitle;
        private Label label1;
        /* TODO ERROR: Skipped EndRegionDirectiveTrivia */
    }
}
