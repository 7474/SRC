using Microsoft.VisualBasic.CompilerServices;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SRCSharpForm
{
    [DesignerGenerated()]
    internal partial class frmTelop
    {
        [DebuggerNonUserCode()]
        public frmTelop() : base()
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
        public Label Label1;
        // メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
        // Windows フォーム デザイナを使って変更できます。
        // コード エディタを使用して、変更しないでください。
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            var resources = new System.Resources.ResourceManager(typeof(frmTelop));
            components = new System.ComponentModel.Container();
            ToolTip1 = new ToolTip(components);
            Label1 = new Label();
            SuspendLayout();
            ToolTip1.Active = true;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            ClientSize = new Size(378, 57);
            Location = new Point(131, 211);
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            AutoScaleMode = AutoScaleMode.Font;
            ControlBox = true;
            Enabled = true;
            KeyPreview = false;
            Cursor = Cursors.Default;
            RightToLeft = RightToLeft.No;
            HelpButton = false;
            WindowState = FormWindowState.Normal;
            Name = "frmTelop";
            Label1.TextAlign = ContentAlignment.TopCenter;
            Label1.Text = "シナリオタイトル";
            Label1.Font = new Font("ＭＳ Ｐ明朝", 15.75f, FontStyle.Bold | FontStyle.Italic | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            Label1.ForeColor = Color.Black;
            Label1.Size = new Size(357, 57);
            Label1.Location = new Point(8, 16);
            Label1.TabIndex = 0;
            Label1.BackColor = Color.Transparent;
            Label1.Enabled = true;
            Label1.Cursor = Cursors.Default;
            Label1.RightToLeft = RightToLeft.No;
            Label1.UseMnemonic = true;
            Label1.Visible = true;
            Label1.AutoSize = false;
            Label1.BorderStyle = BorderStyle.None;
            Label1.Name = "Label1";
            Controls.Add(Label1);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
