using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace SRCSharpForm
{
    internal partial class frmConfiguration
    {
        [DebuggerNonUserCode()]
        public frmConfiguration() : base()
        {
            // この呼び出しは、Windows フォーム デザイナで必要です。
            InitializeComponent();
            _txtMP3Volume.Name = "txtMP3Volume";
            _hscMP3Volume.Name = "hscMP3Volume";
            _cmdCancel.Name = "cmdCancel";
            _cmdOK.Name = "cmdOK";
            _chkBattleAnimation.Name = "chkBattleAnimation";
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
        public CheckBox chkExtendedAnimation;
        public CheckBox chkMoveAnimation;
        public CheckBox chkWeaponAnimation;
        private TextBox _txtMP3Volume;

        public TextBox txtMP3Volume
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _txtMP3Volume;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_txtMP3Volume != null)
                {
                    _txtMP3Volume.TextChanged -= txtMP3Volume_TextChanged;
                }

                _txtMP3Volume = value;
                if (_txtMP3Volume != null)
                {
                    _txtMP3Volume.TextChanged += txtMP3Volume_TextChanged;
                }
            }
        }

        private HScrollBar _hscMP3Volume;

        public HScrollBar hscMP3Volume
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _hscMP3Volume;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_hscMP3Volume != null)
                {
                    _hscMP3Volume.Scroll -= hscMP3Volume_Scroll;
                }

                _hscMP3Volume = value;
                if (_hscMP3Volume != null)
                {
                    _hscMP3Volume.Scroll += hscMP3Volume_Scroll;
                }
            }
        }

        public ComboBox cboMidiReset;
        private Button _cmdCancel;

        public Button cmdCancel
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _cmdCancel;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_cmdCancel != null)
                {
                    _cmdCancel.Click -= cmdCancel_Click;
                }

                _cmdCancel = value;
                if (_cmdCancel != null)
                {
                    _cmdCancel.Click += cmdCancel_Click;
                }
            }
        }

        private Button _cmdOK;

        public Button cmdOK
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _cmdOK;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_cmdOK != null)
                {
                    _cmdOK.Click -= cmdOK_Click;
                }

                _cmdOK = value;
                if (_cmdOK != null)
                {
                    _cmdOK.Click += cmdOK_Click;
                }
            }
        }

        public CheckBox chkKeepEnemyBGM;
        public ComboBox cboMessageSpeed;
        public CheckBox chkAutoMoveCursor;
        public CheckBox chkShowSquareLine;
        public CheckBox chkShowTurn;
        public CheckBox chkSpecialPowerAnimation;
        private CheckBox _chkBattleAnimation;

        public CheckBox chkBattleAnimation
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _chkBattleAnimation;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_chkBattleAnimation != null)
                {
                    _chkBattleAnimation.CheckStateChanged -= chkBattleAnimation_CheckStateChanged;
                }

                _chkBattleAnimation = value;
                if (_chkBattleAnimation != null)
                {
                    _chkBattleAnimation.CheckStateChanged += chkBattleAnimation_CheckStateChanged;
                }
            }
        }

        public Label labMP3Volume;
        public Label labMidiReset;
        public Label labMessageSpeed;
        // メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
        // Windows フォーム デザイナを使って変更できます。
        // コード エディタを使用して、変更しないでください。
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConfiguration));
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkExtendedAnimation = new System.Windows.Forms.CheckBox();
            this.chkMoveAnimation = new System.Windows.Forms.CheckBox();
            this.chkWeaponAnimation = new System.Windows.Forms.CheckBox();
            this._txtMP3Volume = new System.Windows.Forms.TextBox();
            this._hscMP3Volume = new System.Windows.Forms.HScrollBar();
            this.cboMidiReset = new System.Windows.Forms.ComboBox();
            this._cmdCancel = new System.Windows.Forms.Button();
            this._cmdOK = new System.Windows.Forms.Button();
            this.chkKeepEnemyBGM = new System.Windows.Forms.CheckBox();
            this.cboMessageSpeed = new System.Windows.Forms.ComboBox();
            this.chkAutoMoveCursor = new System.Windows.Forms.CheckBox();
            this.chkShowSquareLine = new System.Windows.Forms.CheckBox();
            this.chkShowTurn = new System.Windows.Forms.CheckBox();
            this.chkSpecialPowerAnimation = new System.Windows.Forms.CheckBox();
            this._chkBattleAnimation = new System.Windows.Forms.CheckBox();
            this.labMP3Volume = new System.Windows.Forms.Label();
            this.labMidiReset = new System.Windows.Forms.Label();
            this.labMessageSpeed = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chkExtendedAnimation
            // 
            this.chkExtendedAnimation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.chkExtendedAnimation.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkExtendedAnimation.ForeColor = System.Drawing.Color.Black;
            this.chkExtendedAnimation.Location = new System.Drawing.Point(48, 64);
            this.chkExtendedAnimation.Name = "chkExtendedAnimation";
            this.chkExtendedAnimation.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkExtendedAnimation.Size = new System.Drawing.Size(233, 33);
            this.chkExtendedAnimation.TabIndex = 18;
            this.chkExtendedAnimation.Text = "戦闘アニメの拡張機能を使用する";
            this.chkExtendedAnimation.UseVisualStyleBackColor = false;
            // 
            // chkMoveAnimation
            // 
            this.chkMoveAnimation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.chkMoveAnimation.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkMoveAnimation.ForeColor = System.Drawing.Color.Black;
            this.chkMoveAnimation.Location = new System.Drawing.Point(32, 152);
            this.chkMoveAnimation.Name = "chkMoveAnimation";
            this.chkMoveAnimation.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkMoveAnimation.Size = new System.Drawing.Size(249, 25);
            this.chkMoveAnimation.TabIndex = 17;
            this.chkMoveAnimation.Text = "移動アニメを表示する";
            this.chkMoveAnimation.UseVisualStyleBackColor = false;
            // 
            // chkWeaponAnimation
            // 
            this.chkWeaponAnimation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.chkWeaponAnimation.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkWeaponAnimation.ForeColor = System.Drawing.Color.Black;
            this.chkWeaponAnimation.Location = new System.Drawing.Point(48, 96);
            this.chkWeaponAnimation.Name = "chkWeaponAnimation";
            this.chkWeaponAnimation.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkWeaponAnimation.Size = new System.Drawing.Size(233, 33);
            this.chkWeaponAnimation.TabIndex = 16;
            this.chkWeaponAnimation.Text = "武器準備アニメを自動選択表示する";
            this.chkWeaponAnimation.UseVisualStyleBackColor = false;
            // 
            // _txtMP3Volume
            // 
            this._txtMP3Volume.AcceptsReturn = true;
            this._txtMP3Volume.BackColor = System.Drawing.Color.White;
            this._txtMP3Volume.Cursor = System.Windows.Forms.Cursors.IBeam;
            this._txtMP3Volume.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this._txtMP3Volume.ForeColor = System.Drawing.Color.Black;
            this._txtMP3Volume.Location = new System.Drawing.Point(87, 327);
            this._txtMP3Volume.MaxLength = 0;
            this._txtMP3Volume.Name = "_txtMP3Volume";
            this._txtMP3Volume.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._txtMP3Volume.Size = new System.Drawing.Size(33, 19);
            this._txtMP3Volume.TabIndex = 11;
            this._txtMP3Volume.Text = "100";
            this._txtMP3Volume.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this._txtMP3Volume.TextChanged += new System.EventHandler(this.txtMP3Volume_TextChanged);
            // 
            // _hscMP3Volume
            // 
            this._hscMP3Volume.Cursor = System.Windows.Forms.Cursors.Default;
            this._hscMP3Volume.Location = new System.Drawing.Point(128, 328);
            this._hscMP3Volume.Maximum = 109;
            this._hscMP3Volume.Name = "_hscMP3Volume";
            this._hscMP3Volume.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._hscMP3Volume.Size = new System.Drawing.Size(153, 17);
            this._hscMP3Volume.TabIndex = 12;
            this._hscMP3Volume.TabStop = true;
            this._hscMP3Volume.Value = 50;
            this._hscMP3Volume.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hscMP3Volume_Scroll);
            // 
            // cboMidiReset
            // 
            this.cboMidiReset.BackColor = System.Drawing.Color.White;
            this.cboMidiReset.Cursor = System.Windows.Forms.Cursors.Default;
            this.cboMidiReset.ForeColor = System.Drawing.Color.Black;
            this.cboMidiReset.Location = new System.Drawing.Point(168, 296);
            this.cboMidiReset.Name = "cboMidiReset";
            this.cboMidiReset.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cboMidiReset.Size = new System.Drawing.Size(115, 23);
            this.cboMidiReset.TabIndex = 10;
            // 
            // _cmdCancel
            // 
            this._cmdCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this._cmdCancel.Cursor = System.Windows.Forms.Cursors.Default;
            this._cmdCancel.ForeColor = System.Drawing.SystemColors.ControlText;
            this._cmdCancel.Location = new System.Drawing.Point(216, 360);
            this._cmdCancel.Name = "_cmdCancel";
            this._cmdCancel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._cmdCancel.Size = new System.Drawing.Size(97, 25);
            this._cmdCancel.TabIndex = 14;
            this._cmdCancel.Text = "キャンセル";
            this._cmdCancel.UseVisualStyleBackColor = false;
            this._cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // _cmdOK
            // 
            this._cmdOK.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this._cmdOK.Cursor = System.Windows.Forms.Cursors.Default;
            this._cmdOK.ForeColor = System.Drawing.SystemColors.ControlText;
            this._cmdOK.Location = new System.Drawing.Point(112, 360);
            this._cmdOK.Name = "_cmdOK";
            this._cmdOK.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._cmdOK.Size = new System.Drawing.Size(97, 25);
            this._cmdOK.TabIndex = 13;
            this._cmdOK.Text = "OK";
            this._cmdOK.UseVisualStyleBackColor = false;
            this._cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // chkKeepEnemyBGM
            // 
            this.chkKeepEnemyBGM.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.chkKeepEnemyBGM.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkKeepEnemyBGM.ForeColor = System.Drawing.Color.Black;
            this.chkKeepEnemyBGM.Location = new System.Drawing.Point(32, 248);
            this.chkKeepEnemyBGM.Name = "chkKeepEnemyBGM";
            this.chkKeepEnemyBGM.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkKeepEnemyBGM.Size = new System.Drawing.Size(249, 25);
            this.chkKeepEnemyBGM.TabIndex = 8;
            this.chkKeepEnemyBGM.Text = "敵フェイズ中にＢＧＭを変更しない";
            this.chkKeepEnemyBGM.UseVisualStyleBackColor = false;
            // 
            // cboMessageSpeed
            // 
            this.cboMessageSpeed.BackColor = System.Drawing.Color.White;
            this.cboMessageSpeed.Cursor = System.Windows.Forms.Cursors.Default;
            this.cboMessageSpeed.ForeColor = System.Drawing.Color.Black;
            this.cboMessageSpeed.Location = new System.Drawing.Point(144, 16);
            this.cboMessageSpeed.Name = "cboMessageSpeed";
            this.cboMessageSpeed.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cboMessageSpeed.Size = new System.Drawing.Size(137, 23);
            this.cboMessageSpeed.TabIndex = 2;
            // 
            // chkAutoMoveCursor
            // 
            this.chkAutoMoveCursor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.chkAutoMoveCursor.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkAutoMoveCursor.ForeColor = System.Drawing.Color.Black;
            this.chkAutoMoveCursor.Location = new System.Drawing.Point(32, 176);
            this.chkAutoMoveCursor.Name = "chkAutoMoveCursor";
            this.chkAutoMoveCursor.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkAutoMoveCursor.Size = new System.Drawing.Size(249, 25);
            this.chkAutoMoveCursor.TabIndex = 5;
            this.chkAutoMoveCursor.Text = "マウスカーソルを自動的に移動する";
            this.chkAutoMoveCursor.UseVisualStyleBackColor = false;
            // 
            // chkShowSquareLine
            // 
            this.chkShowSquareLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.chkShowSquareLine.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkShowSquareLine.ForeColor = System.Drawing.Color.Black;
            this.chkShowSquareLine.Location = new System.Drawing.Point(32, 200);
            this.chkShowSquareLine.Name = "chkShowSquareLine";
            this.chkShowSquareLine.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkShowSquareLine.Size = new System.Drawing.Size(265, 25);
            this.chkShowSquareLine.TabIndex = 6;
            this.chkShowSquareLine.Text = "マス目を表示する (要再起動)";
            this.chkShowSquareLine.UseVisualStyleBackColor = false;
            // 
            // chkShowTurn
            // 
            this.chkShowTurn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.chkShowTurn.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkShowTurn.ForeColor = System.Drawing.Color.Black;
            this.chkShowTurn.Location = new System.Drawing.Point(32, 224);
            this.chkShowTurn.Name = "chkShowTurn";
            this.chkShowTurn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkShowTurn.Size = new System.Drawing.Size(249, 25);
            this.chkShowTurn.TabIndex = 7;
            this.chkShowTurn.Text = "味方フェイズ開始時にターン表示を行う";
            this.chkShowTurn.UseVisualStyleBackColor = false;
            // 
            // chkSpecialPowerAnimation
            // 
            this.chkSpecialPowerAnimation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.chkSpecialPowerAnimation.Cursor = System.Windows.Forms.Cursors.Default;
            this.chkSpecialPowerAnimation.ForeColor = System.Drawing.Color.Black;
            this.chkSpecialPowerAnimation.Location = new System.Drawing.Point(32, 128);
            this.chkSpecialPowerAnimation.Name = "chkSpecialPowerAnimation";
            this.chkSpecialPowerAnimation.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkSpecialPowerAnimation.Size = new System.Drawing.Size(249, 25);
            this.chkSpecialPowerAnimation.TabIndex = 4;
            this.chkSpecialPowerAnimation.Text = "スペシャルパワーアニメを表示する";
            this.chkSpecialPowerAnimation.UseVisualStyleBackColor = false;
            // 
            // _chkBattleAnimation
            // 
            this._chkBattleAnimation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this._chkBattleAnimation.Cursor = System.Windows.Forms.Cursors.Default;
            this._chkBattleAnimation.ForeColor = System.Drawing.Color.Black;
            this._chkBattleAnimation.Location = new System.Drawing.Point(32, 40);
            this._chkBattleAnimation.Name = "_chkBattleAnimation";
            this._chkBattleAnimation.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this._chkBattleAnimation.Size = new System.Drawing.Size(249, 25);
            this._chkBattleAnimation.TabIndex = 3;
            this._chkBattleAnimation.Text = "戦闘アニメを表示する";
            this._chkBattleAnimation.UseVisualStyleBackColor = false;
            this._chkBattleAnimation.CheckStateChanged += new System.EventHandler(this.chkBattleAnimation_CheckStateChanged);
            // 
            // labMP3Volume
            // 
            this.labMP3Volume.BackColor = System.Drawing.Color.Transparent;
            this.labMP3Volume.Cursor = System.Windows.Forms.Cursors.Default;
            this.labMP3Volume.ForeColor = System.Drawing.Color.Black;
            this.labMP3Volume.Location = new System.Drawing.Point(33, 330);
            this.labMP3Volume.Name = "labMP3Volume";
            this.labMP3Volume.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labMP3Volume.Size = new System.Drawing.Size(49, 17);
            this.labMP3Volume.TabIndex = 15;
            this.labMP3Volume.Text = "MP3音量";
            // 
            // labMidiReset
            // 
            this.labMidiReset.BackColor = System.Drawing.Color.Transparent;
            this.labMidiReset.Cursor = System.Windows.Forms.Cursors.Default;
            this.labMidiReset.ForeColor = System.Drawing.Color.Black;
            this.labMidiReset.Location = new System.Drawing.Point(33, 301);
            this.labMidiReset.Name = "labMidiReset";
            this.labMidiReset.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labMidiReset.Size = new System.Drawing.Size(192, 17);
            this.labMidiReset.TabIndex = 1;
            this.labMidiReset.Text = "MIDI音源リセットの種類";
            // 
            // labMessageSpeed
            // 
            this.labMessageSpeed.BackColor = System.Drawing.Color.Transparent;
            this.labMessageSpeed.Cursor = System.Windows.Forms.Cursors.Default;
            this.labMessageSpeed.Font = new System.Drawing.Font("ＭＳ Ｐゴシック", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labMessageSpeed.ForeColor = System.Drawing.Color.Black;
            this.labMessageSpeed.Location = new System.Drawing.Point(33, 20);
            this.labMessageSpeed.Name = "labMessageSpeed";
            this.labMessageSpeed.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.labMessageSpeed.Size = new System.Drawing.Size(129, 17);
            this.labMessageSpeed.TabIndex = 0;
            this.labMessageSpeed.Text = "メッセージスピード";
            // 
            // frmConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(346, 405);
            this.Controls.Add(this.chkExtendedAnimation);
            this.Controls.Add(this.chkMoveAnimation);
            this.Controls.Add(this.chkWeaponAnimation);
            this.Controls.Add(this._txtMP3Volume);
            this.Controls.Add(this._hscMP3Volume);
            this.Controls.Add(this.cboMidiReset);
            this.Controls.Add(this._cmdCancel);
            this.Controls.Add(this._cmdOK);
            this.Controls.Add(this.chkKeepEnemyBGM);
            this.Controls.Add(this.cboMessageSpeed);
            this.Controls.Add(this.chkAutoMoveCursor);
            this.Controls.Add(this.chkShowSquareLine);
            this.Controls.Add(this.chkShowTurn);
            this.Controls.Add(this.chkSpecialPowerAnimation);
            this.Controls.Add(this._chkBattleAnimation);
            this.Controls.Add(this.labMP3Volume);
            this.Controls.Add(this.labMidiReset);
            this.Controls.Add(this.labMessageSpeed);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(3, 29);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfiguration";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ShowInTaskbar = false;
            this.Text = "設定変更";
            this.Load += new System.EventHandler(this.frmConfiguration_Load);
            this.ResumeLayout(false);

        }
    }
}
