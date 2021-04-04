using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    [DesignerGenerated()]
    internal partial class frmMultiColumnListBox
    {
        /* TODO ERROR: Skipped RegionDirectiveTrivia */
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

        public ListBox lstItems
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _lstItems;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_lstItems != null)
                {
                    _lstItems.MouseDown -= lstItems_MouseDown;
                    _lstItems.MouseMove -= lstItems_MouseMove;
                }

                _lstItems = value;
                if (_lstItems != null)
                {
                    _lstItems.MouseDown += lstItems_MouseDown;
                    _lstItems.MouseMove += lstItems_MouseMove;
                }
            }
        }

        public Label labCaption;
        // メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
        // Windows フォーム デザイナを使って変更できます。
        // コード エディタを使用して、変更しないでください。
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            var resources = new System.Resources.ResourceManager(typeof(frmMultiColumnListBox));
            components = new System.ComponentModel.Container();
            ToolTip1 = new ToolTip(components);
            _lstItems = new ListBox();
            _lstItems.MouseDown += new MouseEventHandler(lstItems_MouseDown);
            _lstItems.MouseMove += new MouseEventHandler(lstItems_MouseMove);
            labCaption = new Label();
            SuspendLayout();
            ToolTip1.Active = true;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.FromArgb(192, 192, 192);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Text = "MultiColumListBox";
            ClientSize = new Size(670, 446);
            Location = new Point(72, 116);
            Icon = (Icon)resources.GetObject("frmMultiColumnListBox.Icon");
            MaximizeBox = false;
            AutoScaleMode = AutoScaleMode.Font;
            ControlBox = true;
            Enabled = true;
            KeyPreview = false;
            MinimizeBox = true;
            Cursor = Cursors.Default;
            RightToLeft = RightToLeft.No;
            ShowInTaskbar = true;
            HelpButton = false;
            WindowState = FormWindowState.Normal;
            Name = "frmMultiColumnListBox";
            _lstItems.BackColor = Color.White;
            _lstItems.Font = new Font("ＭＳ 明朝", 12f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            _lstItems.ForeColor = Color.Black;
            _lstItems.Size = new Size(654, 407);
            _lstItems.Location = new Point(8, 8);
            _lstItems.TabIndex = 0;
            _lstItems.BorderStyle = BorderStyle.Fixed3D;
            _lstItems.CausesValidation = true;
            _lstItems.Enabled = true;
            _lstItems.IntegralHeight = true;
            _lstItems.Cursor = Cursors.Default;
            _lstItems.SelectionMode = SelectionMode.One;
            _lstItems.RightToLeft = RightToLeft.No;
            _lstItems.Sorted = false;
            _lstItems.TabStop = true;
            _lstItems.Visible = true;
            _lstItems.MultiColumn = true;
            _lstItems.ColumnWidth = 164;
            _lstItems.Name = "_lstItems";
            labCaption.BackColor = Color.White;
            labCaption.Font = new Font("ＭＳ 明朝", 12f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            labCaption.ForeColor = Color.Black;
            labCaption.Size = new Size(654, 23);
            labCaption.Location = new Point(8, 416);
            labCaption.TabIndex = 1;
            labCaption.TextAlign = ContentAlignment.TopLeft;
            labCaption.Enabled = true;
            labCaption.Cursor = Cursors.Default;
            labCaption.RightToLeft = RightToLeft.No;
            labCaption.UseMnemonic = true;
            labCaption.Visible = true;
            labCaption.AutoSize = false;
            labCaption.BorderStyle = BorderStyle.Fixed3D;
            labCaption.Name = "labCaption";
            Controls.Add(_lstItems);
            Controls.Add(labCaption);
            Activated += new EventHandler(frmMultiColumnListBox_Activated);
            Load += new EventHandler(frmMultiColumnListBox_Load);
            FormClosed += new FormClosedEventHandler(frmMultiColumnListBox_FormClosed);
            MouseDown += new MouseEventHandler(frmMultiColumnListBox_MouseDown);
            ResumeLayout(false);
            PerformLayout();
        }
        /* TODO ERROR: Skipped EndRegionDirectiveTrivia */
    }
}
