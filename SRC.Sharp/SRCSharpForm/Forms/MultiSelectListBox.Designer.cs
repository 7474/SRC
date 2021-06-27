using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace SRCSharpForm
{
    internal partial class frmMultiSelectListBox
    {
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

        public Label lblNumber;
        public Label lblCaption;
        // メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
        // Windows フォーム デザイナを使って変更できます。
        // コード エディタを使用して、変更しないでください。
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMultiSelectListBox));
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this._cmdResume = new System.Windows.Forms.Button();
            this._cmdSort = new System.Windows.Forms.Button();
            this._cmdSelectAll2 = new System.Windows.Forms.Button();
            this._cmdSelectAll = new System.Windows.Forms.Button();
            this._cmdFinish = new System.Windows.Forms.Button();
            this._lstItems = new System.Windows.Forms.ListBox();
            this.lblNumber = new System.Windows.Forms.Label();
            this.lblCaption = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _cmdResume
            // 
            this._cmdResume.BackColor = System.Drawing.SystemColors.Control;
            this._cmdResume.Cursor = System.Windows.Forms.Cursors.Default;
            this._cmdResume.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._cmdResume.ForeColor = System.Drawing.SystemColors.ControlText;
            this._cmdResume.Location = new System.Drawing.Point(96, 296);
            this._cmdResume.Name = "_cmdResume";
            this._cmdResume.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._cmdResume.Size = new System.Drawing.Size(137, 29);
            this._cmdResume.TabIndex = 7;
            this._cmdResume.TabStop = false;
            this._cmdResume.Text = "マップを見る";
            this._cmdResume.UseVisualStyleBackColor = false;
            this._cmdResume.Click += new System.EventHandler(this.cmdResume_Click);
            // 
            // _cmdSort
            // 
            this._cmdSort.BackColor = System.Drawing.SystemColors.Control;
            this._cmdSort.Cursor = System.Windows.Forms.Cursors.Default;
            this._cmdSort.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._cmdSort.ForeColor = System.Drawing.SystemColors.ControlText;
            this._cmdSort.Location = new System.Drawing.Point(336, 264);
            this._cmdSort.Name = "_cmdSort";
            this._cmdSort.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._cmdSort.Size = new System.Drawing.Size(145, 29);
            this._cmdSort.TabIndex = 6;
            this._cmdSort.TabStop = false;
            this._cmdSort.Text = "名称順に並べ替え";
            this._cmdSort.UseVisualStyleBackColor = false;
            this._cmdSort.Click += new System.EventHandler(this.cmdSort_Click);
            // 
            // _cmdSelectAll2
            // 
            this._cmdSelectAll2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this._cmdSelectAll2.Cursor = System.Windows.Forms.Cursors.Default;
            this._cmdSelectAll2.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._cmdSelectAll2.ForeColor = System.Drawing.SystemColors.ControlText;
            this._cmdSelectAll2.Location = new System.Drawing.Point(168, 264);
            this._cmdSelectAll2.Name = "_cmdSelectAll2";
            this._cmdSelectAll2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._cmdSelectAll2.Size = new System.Drawing.Size(161, 29);
            this._cmdSelectAll2.TabIndex = 5;
            this._cmdSelectAll2.TabStop = false;
            this._cmdSelectAll2.Text = "最後から選択";
            this._cmdSelectAll2.UseVisualStyleBackColor = false;
            this._cmdSelectAll2.Click += new System.EventHandler(this.cmdSelectAll2_Click);
            // 
            // _cmdSelectAll
            // 
            this._cmdSelectAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this._cmdSelectAll.Cursor = System.Windows.Forms.Cursors.Default;
            this._cmdSelectAll.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._cmdSelectAll.ForeColor = System.Drawing.SystemColors.ControlText;
            this._cmdSelectAll.Location = new System.Drawing.Point(8, 264);
            this._cmdSelectAll.Name = "_cmdSelectAll";
            this._cmdSelectAll.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._cmdSelectAll.Size = new System.Drawing.Size(153, 29);
            this._cmdSelectAll.TabIndex = 4;
            this._cmdSelectAll.TabStop = false;
            this._cmdSelectAll.Text = "先頭から選択";
            this._cmdSelectAll.UseVisualStyleBackColor = false;
            this._cmdSelectAll.Click += new System.EventHandler(this.cmdSelectAll_Click);
            // 
            // _cmdFinish
            // 
            this._cmdFinish.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this._cmdFinish.Cursor = System.Windows.Forms.Cursors.Default;
            this._cmdFinish.Enabled = false;
            this._cmdFinish.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this._cmdFinish.ForeColor = System.Drawing.SystemColors.ControlText;
            this._cmdFinish.Location = new System.Drawing.Point(264, 296);
            this._cmdFinish.Name = "_cmdFinish";
            this._cmdFinish.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._cmdFinish.Size = new System.Drawing.Size(137, 29);
            this._cmdFinish.TabIndex = 1;
            this._cmdFinish.TabStop = false;
            this._cmdFinish.Text = "終了";
            this._cmdFinish.UseVisualStyleBackColor = false;
            this._cmdFinish.Click += new System.EventHandler(this.cmdFinish_Click);
            // 
            // _lstItems
            // 
            this._lstItems.BackColor = System.Drawing.Color.White;
            this._lstItems.Cursor = System.Windows.Forms.Cursors.Default;
            this._lstItems.Font = new System.Drawing.Font("ＭＳ 明朝", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._lstItems.ForeColor = System.Drawing.Color.Black;
            this._lstItems.ItemHeight = 16;
            this._lstItems.Location = new System.Drawing.Point(8, 32);
            this._lstItems.Name = "_lstItems";
            this._lstItems.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._lstItems.Size = new System.Drawing.Size(479, 228);
            this._lstItems.TabIndex = 0;
            this._lstItems.DoubleClick += new System.EventHandler(this._lstItems_DoubleClick);
            this._lstItems.MouseDown += new System.Windows.Forms.MouseEventHandler(this._lstItems_MouseDown);
            this._lstItems.MouseMove += new System.Windows.Forms.MouseEventHandler(this._lstItems_MouseMove);
            // 
            // lblNumber
            // 
            this.lblNumber.BackColor = System.Drawing.Color.White;
            this.lblNumber.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNumber.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblNumber.Font = new System.Drawing.Font("ＭＳ 明朝", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblNumber.ForeColor = System.Drawing.Color.Black;
            this.lblNumber.Location = new System.Drawing.Point(424, 296);
            this.lblNumber.Name = "lblNumber";
            this.lblNumber.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblNumber.Size = new System.Drawing.Size(57, 29);
            this.lblNumber.TabIndex = 3;
            this.lblNumber.Text = "Label1";
            this.lblNumber.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblCaption
            // 
            this.lblCaption.BackColor = System.Drawing.Color.White;
            this.lblCaption.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblCaption.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblCaption.Font = new System.Drawing.Font("ＭＳ 明朝", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblCaption.ForeColor = System.Drawing.Color.Black;
            this.lblCaption.Location = new System.Drawing.Point(8, 6);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblCaption.Size = new System.Drawing.Size(479, 22);
            this.lblCaption.TabIndex = 2;
            this.lblCaption.Text = "Label1";
            // 
            // frmMultiSelectListBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(497, 331);
            this.Controls.Add(this._cmdResume);
            this.Controls.Add(this._cmdSort);
            this.Controls.Add(this._cmdSelectAll2);
            this.Controls.Add(this._cmdSelectAll);
            this.Controls.Add(this._cmdFinish);
            this.Controls.Add(this._lstItems);
            this.Controls.Add(this.lblNumber);
            this.Controls.Add(this.lblCaption);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(60, 165);
            this.MaximizeBox = false;
            this.Name = "frmMultiSelectListBox";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MultiSelectListBox";
            this.Activated += new System.EventHandler(this.frmMultiSelectListBox_Activated);
            this.ResumeLayout(false);

        }
    }
}
