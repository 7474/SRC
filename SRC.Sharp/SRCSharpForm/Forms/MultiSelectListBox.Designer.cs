using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Project1
{
    [DesignerGenerated()]
    internal partial class frmMultiSelectListBox
    {
        /* TODO ERROR: Skipped RegionDirectiveTrivia */
        [DebuggerNonUserCode()]
        public frmMultiSelectListBox() : base()
        {
            // この呼び出しは、Windows フォーム デザイナで必要です。
            InitializeComponent();
            _cmdResume.Name = "cmdResume";
            _cmdSort.Name = "cmdSort";
            _cmdSelectAll2.Name = "cmdSelectAll2";
            _cmdSelectAll.Name = "cmdSelectAll";
            _cmdFinish.Name = "cmdFinish";
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
        private Button _cmdResume;

        public Button cmdResume
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _cmdResume;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_cmdResume != null)
                {
                    _cmdResume.Click -= cmdResume_Click;
                }

                _cmdResume = value;
                if (_cmdResume != null)
                {
                    _cmdResume.Click += cmdResume_Click;
                }
            }
        }

        private Button _cmdSort;

        public Button cmdSort
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _cmdSort;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_cmdSort != null)
                {
                    _cmdSort.Click -= cmdSort_Click;
                }

                _cmdSort = value;
                if (_cmdSort != null)
                {
                    _cmdSort.Click += cmdSort_Click;
                }
            }
        }

        private Button _cmdSelectAll2;

        public Button cmdSelectAll2
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _cmdSelectAll2;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_cmdSelectAll2 != null)
                {
                    _cmdSelectAll2.Click -= cmdSelectAll2_Click;
                }

                _cmdSelectAll2 = value;
                if (_cmdSelectAll2 != null)
                {
                    _cmdSelectAll2.Click += cmdSelectAll2_Click;
                }
            }
        }

        private Button _cmdSelectAll;

        public Button cmdSelectAll
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _cmdSelectAll;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_cmdSelectAll != null)
                {
                    _cmdSelectAll.Click -= cmdSelectAll_Click;
                }

                _cmdSelectAll = value;
                if (_cmdSelectAll != null)
                {
                    _cmdSelectAll.Click += cmdSelectAll_Click;
                }
            }
        }

        private Timer _Timer1;

        public Timer Timer1
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _Timer1;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_Timer1 != null)
                {
                    _Timer1.Tick -= Timer1_Tick;
                }

                _Timer1 = value;
                if (_Timer1 != null)
                {
                    _Timer1.Tick += Timer1_Tick;
                }
            }
        }

        private Button _cmdFinish;

        public Button cmdFinish
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _cmdFinish;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_cmdFinish != null)
                {
                    _cmdFinish.Click -= cmdFinish_Click;
                }

                _cmdFinish = value;
                if (_cmdFinish != null)
                {
                    _cmdFinish.Click += cmdFinish_Click;
                }
            }
        }

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
                    _lstItems.DoubleClick -= lstItems_DoubleClick;
                    _lstItems.MouseDown -= lstItems_MouseDown;
                    _lstItems.MouseMove -= lstItems_MouseMove;
                }

                _lstItems = value;
                if (_lstItems != null)
                {
                    _lstItems.DoubleClick += lstItems_DoubleClick;
                    _lstItems.MouseDown += lstItems_MouseDown;
                    _lstItems.MouseMove += lstItems_MouseMove;
                }
            }
        }

        public Label lblNumber;
        public Label lblCaption;
        // メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
        // Windows フォーム デザイナを使って変更できます。
        // コード エディタを使用して、変更しないでください。
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            var resources = new System.Resources.ResourceManager(typeof(frmMultiSelectListBox));
            components = new System.ComponentModel.Container();
            ToolTip1 = new ToolTip(components);
            _cmdResume = new Button();
            _cmdResume.Click += new EventHandler(cmdResume_Click);
            _cmdSort = new Button();
            _cmdSort.Click += new EventHandler(cmdSort_Click);
            _cmdSelectAll2 = new Button();
            _cmdSelectAll2.Click += new EventHandler(cmdSelectAll2_Click);
            _cmdSelectAll = new Button();
            _cmdSelectAll.Click += new EventHandler(cmdSelectAll_Click);
            _Timer1 = new Timer(components);
            _Timer1.Tick += new EventHandler(Timer1_Tick);
            _cmdFinish = new Button();
            _cmdFinish.Click += new EventHandler(cmdFinish_Click);
            _lstItems = new ListBox();
            _lstItems.DoubleClick += new EventHandler(lstItems_DoubleClick);
            _lstItems.MouseDown += new MouseEventHandler(lstItems_MouseDown);
            _lstItems.MouseMove += new MouseEventHandler(lstItems_MouseMove);
            lblNumber = new Label();
            lblCaption = new Label();
            SuspendLayout();
            ToolTip1.Active = true;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.FromArgb(192, 192, 192);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Text = "MultiSelectListBox";
            ClientSize = new Size(497, 331);
            Location = new Point(60, 165);
            Font = new Font("ＭＳ ゴシック", 12f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            Icon = (Icon)resources.GetObject("frmMultiSelectListBox.Icon");
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
            Name = "frmMultiSelectListBox";
            _cmdResume.TextAlign = ContentAlignment.MiddleCenter;
            _cmdResume.BackColor = SystemColors.Control;
            _cmdResume.Text = "マップを見る";
            _cmdResume.Font = new Font("ＭＳ Ｐゴシック", 9.75f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            _cmdResume.Size = new Size(137, 29);
            _cmdResume.Location = new Point(96, 296);
            _cmdResume.TabIndex = 7;
            _cmdResume.TabStop = false;
            _cmdResume.CausesValidation = true;
            _cmdResume.Enabled = true;
            _cmdResume.ForeColor = SystemColors.ControlText;
            _cmdResume.Cursor = Cursors.Default;
            _cmdResume.RightToLeft = RightToLeft.No;
            _cmdResume.Name = "_cmdResume";
            _cmdSort.TextAlign = ContentAlignment.MiddleCenter;
            _cmdSort.BackColor = SystemColors.Control;
            _cmdSort.Text = "名称順に並べ替え";
            _cmdSort.Font = new Font("ＭＳ Ｐゴシック", 9.75f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            _cmdSort.Size = new Size(145, 29);
            _cmdSort.Location = new Point(336, 264);
            _cmdSort.TabIndex = 6;
            _cmdSort.TabStop = false;
            _cmdSort.CausesValidation = true;
            _cmdSort.Enabled = true;
            _cmdSort.ForeColor = SystemColors.ControlText;
            _cmdSort.Cursor = Cursors.Default;
            _cmdSort.RightToLeft = RightToLeft.No;
            _cmdSort.Name = "_cmdSort";
            _cmdSelectAll2.TextAlign = ContentAlignment.MiddleCenter;
            _cmdSelectAll2.BackColor = Color.FromArgb(192, 192, 192);
            _cmdSelectAll2.Text = "最後から選択";
            _cmdSelectAll2.Font = new Font("ＭＳ Ｐゴシック", 9.75f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            _cmdSelectAll2.Size = new Size(161, 29);
            _cmdSelectAll2.Location = new Point(168, 264);
            _cmdSelectAll2.TabIndex = 5;
            _cmdSelectAll2.TabStop = false;
            _cmdSelectAll2.CausesValidation = true;
            _cmdSelectAll2.Enabled = true;
            _cmdSelectAll2.ForeColor = SystemColors.ControlText;
            _cmdSelectAll2.Cursor = Cursors.Default;
            _cmdSelectAll2.RightToLeft = RightToLeft.No;
            _cmdSelectAll2.Name = "_cmdSelectAll2";
            _cmdSelectAll.TextAlign = ContentAlignment.MiddleCenter;
            _cmdSelectAll.BackColor = Color.FromArgb(192, 192, 192);
            _cmdSelectAll.Text = "先頭から選択";
            _cmdSelectAll.Font = new Font("ＭＳ Ｐゴシック", 9.75f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            _cmdSelectAll.Size = new Size(153, 29);
            _cmdSelectAll.Location = new Point(8, 264);
            _cmdSelectAll.TabIndex = 4;
            _cmdSelectAll.TabStop = false;
            _cmdSelectAll.CausesValidation = true;
            _cmdSelectAll.Enabled = true;
            _cmdSelectAll.ForeColor = SystemColors.ControlText;
            _cmdSelectAll.Cursor = Cursors.Default;
            _cmdSelectAll.RightToLeft = RightToLeft.No;
            _cmdSelectAll.Name = "_cmdSelectAll";
            _Timer1.Interval = 100;
            _Timer1.Enabled = true;
            _cmdFinish.TextAlign = ContentAlignment.MiddleCenter;
            _cmdFinish.BackColor = Color.FromArgb(192, 192, 192);
            _cmdFinish.Text = "終了";
            _cmdFinish.Enabled = false;
            _cmdFinish.Font = new Font("ＭＳ Ｐゴシック", 9.75f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            _cmdFinish.Size = new Size(137, 29);
            _cmdFinish.Location = new Point(264, 296);
            _cmdFinish.TabIndex = 1;
            _cmdFinish.TabStop = false;
            _cmdFinish.CausesValidation = true;
            _cmdFinish.ForeColor = SystemColors.ControlText;
            _cmdFinish.Cursor = Cursors.Default;
            _cmdFinish.RightToLeft = RightToLeft.No;
            _cmdFinish.Name = "_cmdFinish";
            _lstItems.BackColor = Color.White;
            _lstItems.Font = new Font("ＭＳ 明朝", 12f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            _lstItems.ForeColor = Color.Black;
            _lstItems.Size = new Size(479, 231);
            _lstItems.Location = new Point(8, 32);
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
            _lstItems.MultiColumn = false;
            _lstItems.Name = "_lstItems";
            lblNumber.TextAlign = ContentAlignment.TopCenter;
            lblNumber.BackColor = Color.White;
            lblNumber.Text = "Label1";
            lblNumber.Font = new Font("ＭＳ 明朝", 15.75f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            lblNumber.ForeColor = Color.Black;
            lblNumber.Size = new Size(57, 29);
            lblNumber.Location = new Point(424, 296);
            lblNumber.TabIndex = 3;
            lblNumber.Enabled = true;
            lblNumber.Cursor = Cursors.Default;
            lblNumber.RightToLeft = RightToLeft.No;
            lblNumber.UseMnemonic = true;
            lblNumber.Visible = true;
            lblNumber.AutoSize = false;
            lblNumber.BorderStyle = BorderStyle.Fixed3D;
            lblNumber.Name = "lblNumber";
            lblCaption.BackColor = Color.White;
            lblCaption.Text = "Label1";
            lblCaption.Font = new Font("ＭＳ 明朝", 12f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            lblCaption.ForeColor = Color.Black;
            lblCaption.Size = new Size(479, 22);
            lblCaption.Location = new Point(8, 6);
            lblCaption.TabIndex = 2;
            lblCaption.TextAlign = ContentAlignment.TopLeft;
            lblCaption.Enabled = true;
            lblCaption.Cursor = Cursors.Default;
            lblCaption.RightToLeft = RightToLeft.No;
            lblCaption.UseMnemonic = true;
            lblCaption.Visible = true;
            lblCaption.AutoSize = false;
            lblCaption.BorderStyle = BorderStyle.Fixed3D;
            lblCaption.Name = "lblCaption";
            Controls.Add(_cmdResume);
            Controls.Add(_cmdSort);
            Controls.Add(_cmdSelectAll2);
            Controls.Add(_cmdSelectAll);
            Controls.Add(_cmdFinish);
            Controls.Add(_lstItems);
            Controls.Add(lblNumber);
            Controls.Add(lblCaption);
            Activated += new EventHandler(frmMultiSelectListBox_Activated);
            ResumeLayout(false);
            PerformLayout();
        }
        /* TODO ERROR: Skipped EndRegionDirectiveTrivia */
    }
}
