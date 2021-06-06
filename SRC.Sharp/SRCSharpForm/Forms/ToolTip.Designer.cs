using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Project1
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    internal partial class frmToolTip
    {
        [DebuggerNonUserCode()]
        public frmToolTip() : base()
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
        public PictureBox picMessage;
        // メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
        // Windows フォーム デザイナを使って変更できます。
        // コード エディタを使用して、変更しないでください。
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            var resources = new System.Resources.ResourceManager(typeof(frmToolTip));
            components = new System.ComponentModel.Container();
            ToolTip1 = new ToolTip(components);
            picMessage = new PictureBox();
            SuspendLayout();
            ToolTip1.Active = true;
            StartPosition = FormStartPosition.Manual;
            FormBorderStyle = FormBorderStyle.None;
            ClientSize = new Size(231, 114);
            Location = new Point(0, 0);
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Visible = false;
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ControlBox = true;
            Enabled = true;
            KeyPreview = false;
            Cursor = Cursors.Default;
            RightToLeft = RightToLeft.No;
            HelpButton = false;
            WindowState = FormWindowState.Normal;
            Name = "frmToolTip";
            picMessage.BackColor = SystemColors.Info;
            picMessage.ForeColor = Color.Black;
            picMessage.Size = new Size(193, 104);
            picMessage.Location = new Point(0, 0);
            picMessage.TabIndex = 0;
            picMessage.Dock = DockStyle.None;
            picMessage.CausesValidation = true;
            picMessage.Enabled = true;
            picMessage.Cursor = Cursors.Default;
            picMessage.RightToLeft = RightToLeft.No;
            picMessage.TabStop = true;
            picMessage.Visible = true;
            picMessage.SizeMode = PictureBoxSizeMode.Normal;
            picMessage.BorderStyle = BorderStyle.None;
            picMessage.Name = "picMessage";
            Controls.Add(picMessage);
            Load += new EventHandler(frmToolTip_Load);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
