using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace SRCSharpForm
{
    [DesignerGenerated()]
    internal partial class frmErrorMessage
    {
        /* TODO ERROR: Skipped RegionDirectiveTrivia */
        [DebuggerNonUserCode()]
        public frmErrorMessage() : base()
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
        public TextBox txtMessage;
        // メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
        // Windows フォーム デザイナを使って変更できます。
        // コード エディタを使用して、変更しないでください。
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            var resources = new System.Resources.ResourceManager(typeof(frmErrorMessage));
            components = new System.ComponentModel.Container();
            ToolTip1 = new ToolTip(components);
            txtMessage = new TextBox();
            SuspendLayout();
            ToolTip1.Active = true;
            BackColor = Color.FromArgb(192, 192, 192);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Text = "エラー";
            ClientSize = new Size(671, 118);
            Location = new Point(3, 22);
            Icon = (Icon)resources.GetObject("frmErrorMessage.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.WindowsDefaultLocation;
            AutoScaleMode = AutoScaleMode.Font;
            ControlBox = true;
            Enabled = true;
            KeyPreview = false;
            Cursor = Cursors.Default;
            RightToLeft = RightToLeft.No;
            HelpButton = false;
            WindowState = FormWindowState.Normal;
            Name = "frmErrorMessage";
            txtMessage.AutoSize = false;
            txtMessage.BackColor = Color.White;
            txtMessage.Font = new Font("ＭＳ ゴシック", 12f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            txtMessage.ForeColor = Color.Black;
            txtMessage.Size = new Size(659, 106);
            txtMessage.Location = new Point(6, 6);
            txtMessage.Multiline = true;
            txtMessage.TabIndex = 0;
            txtMessage.Text = "Text1" + '\r' + '\n';
            txtMessage.AcceptsReturn = true;
            txtMessage.TextAlign = HorizontalAlignment.Left;
            txtMessage.CausesValidation = true;
            txtMessage.Enabled = true;
            txtMessage.HideSelection = true;
            txtMessage.ReadOnly = false;
            txtMessage.MaxLength = 0;
            txtMessage.Cursor = Cursors.IBeam;
            txtMessage.RightToLeft = RightToLeft.No;
            txtMessage.ScrollBars = ScrollBars.None;
            txtMessage.TabStop = true;
            txtMessage.Visible = true;
            txtMessage.BorderStyle = BorderStyle.Fixed3D;
            txtMessage.Name = "txtMessage";
            Controls.Add(txtMessage);
            ResumeLayout(false);
            PerformLayout();
        }
        /* TODO ERROR: Skipped EndRegionDirectiveTrivia */
    }
}
