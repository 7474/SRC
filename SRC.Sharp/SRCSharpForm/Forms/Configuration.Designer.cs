using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    [DesignerGenerated()]
    internal partial class frmConfiguration
    {
        /* TODO ERROR: Skipped RegionDirectiveTrivia */
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
        public CheckBox chkUseDirectMusic;
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
            var resources = new System.Resources.ResourceManager(typeof(frmConfiguration));
            components = new System.ComponentModel.Container();
            ToolTip1 = new ToolTip(components);
            chkExtendedAnimation = new CheckBox();
            chkMoveAnimation = new CheckBox();
            chkWeaponAnimation = new CheckBox();
            _txtMP3Volume = new TextBox();
            _txtMP3Volume.TextChanged += new EventHandler(txtMP3Volume_TextChanged);
            _hscMP3Volume = new HScrollBar();
            _hscMP3Volume.Scroll += new ScrollEventHandler(hscMP3Volume_Scroll);
            cboMidiReset = new ComboBox();
            chkUseDirectMusic = new CheckBox();
            _cmdCancel = new Button();
            _cmdCancel.Click += new EventHandler(cmdCancel_Click);
            _cmdOK = new Button();
            _cmdOK.Click += new EventHandler(cmdOK_Click);
            chkKeepEnemyBGM = new CheckBox();
            cboMessageSpeed = new ComboBox();
            chkAutoMoveCursor = new CheckBox();
            chkShowSquareLine = new CheckBox();
            chkShowTurn = new CheckBox();
            chkSpecialPowerAnimation = new CheckBox();
            _chkBattleAnimation = new CheckBox();
            _chkBattleAnimation.CheckStateChanged += new EventHandler(chkBattleAnimation_CheckStateChanged);
            labMP3Volume = new Label();
            labMidiReset = new Label();
            labMessageSpeed = new Label();
            SuspendLayout();
            ToolTip1.Active = true;
            BackColor = Color.FromArgb(192, 192, 192);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Text = "設定変更";
            ClientSize = new Size(346, 405);
            Location = new Point(3, 29);
            ForeColor = Color.Black;
            Icon = (Icon)resources.GetObject("frmConfiguration.Icon");
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
            Name = "frmConfiguration";
            chkExtendedAnimation.BackColor = Color.FromArgb(192, 192, 192);
            chkExtendedAnimation.Text = "戦闘アニメの拡張機能を使用する";
            chkExtendedAnimation.ForeColor = Color.Black;
            chkExtendedAnimation.Size = new Size(233, 33);
            chkExtendedAnimation.Location = new Point(48, 64);
            chkExtendedAnimation.TabIndex = 18;
            chkExtendedAnimation.CheckAlign = ContentAlignment.MiddleLeft;
            chkExtendedAnimation.FlatStyle = FlatStyle.Standard;
            chkExtendedAnimation.CausesValidation = true;
            chkExtendedAnimation.Enabled = true;
            chkExtendedAnimation.Cursor = Cursors.Default;
            chkExtendedAnimation.RightToLeft = RightToLeft.No;
            chkExtendedAnimation.Appearance = Appearance.Normal;
            chkExtendedAnimation.TabStop = true;
            chkExtendedAnimation.CheckState = CheckState.Unchecked;
            chkExtendedAnimation.Visible = true;
            chkExtendedAnimation.Name = "chkExtendedAnimation";
            chkMoveAnimation.BackColor = Color.FromArgb(192, 192, 192);
            chkMoveAnimation.Text = "移動アニメを表示する";
            chkMoveAnimation.ForeColor = Color.Black;
            chkMoveAnimation.Size = new Size(249, 25);
            chkMoveAnimation.Location = new Point(32, 152);
            chkMoveAnimation.TabIndex = 17;
            chkMoveAnimation.CheckAlign = ContentAlignment.MiddleLeft;
            chkMoveAnimation.FlatStyle = FlatStyle.Standard;
            chkMoveAnimation.CausesValidation = true;
            chkMoveAnimation.Enabled = true;
            chkMoveAnimation.Cursor = Cursors.Default;
            chkMoveAnimation.RightToLeft = RightToLeft.No;
            chkMoveAnimation.Appearance = Appearance.Normal;
            chkMoveAnimation.TabStop = true;
            chkMoveAnimation.CheckState = CheckState.Unchecked;
            chkMoveAnimation.Visible = true;
            chkMoveAnimation.Name = "chkMoveAnimation";
            chkWeaponAnimation.BackColor = Color.FromArgb(192, 192, 192);
            chkWeaponAnimation.Text = "武器準備アニメを自動選択表示する";
            chkWeaponAnimation.ForeColor = Color.Black;
            chkWeaponAnimation.Size = new Size(233, 33);
            chkWeaponAnimation.Location = new Point(48, 96);
            chkWeaponAnimation.TabIndex = 16;
            chkWeaponAnimation.CheckAlign = ContentAlignment.MiddleLeft;
            chkWeaponAnimation.FlatStyle = FlatStyle.Standard;
            chkWeaponAnimation.CausesValidation = true;
            chkWeaponAnimation.Enabled = true;
            chkWeaponAnimation.Cursor = Cursors.Default;
            chkWeaponAnimation.RightToLeft = RightToLeft.No;
            chkWeaponAnimation.Appearance = Appearance.Normal;
            chkWeaponAnimation.TabStop = true;
            chkWeaponAnimation.CheckState = CheckState.Unchecked;
            chkWeaponAnimation.Visible = true;
            chkWeaponAnimation.Name = "chkWeaponAnimation";
            _txtMP3Volume.AutoSize = false;
            _txtMP3Volume.TextAlign = HorizontalAlignment.Center;
            _txtMP3Volume.BackColor = Color.White;
            _txtMP3Volume.Font = new Font("ＭＳ Ｐゴシック", 9.75f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            _txtMP3Volume.ForeColor = Color.Black;
            _txtMP3Volume.Size = new Size(33, 19);
            _txtMP3Volume.Location = new Point(87, 327);
            _txtMP3Volume.TabIndex = 11;
            _txtMP3Volume.Text = "100";
            _txtMP3Volume.AcceptsReturn = true;
            _txtMP3Volume.CausesValidation = true;
            _txtMP3Volume.Enabled = true;
            _txtMP3Volume.HideSelection = true;
            _txtMP3Volume.ReadOnly = false;
            _txtMP3Volume.MaxLength = 0;
            _txtMP3Volume.Cursor = Cursors.IBeam;
            _txtMP3Volume.Multiline = false;
            _txtMP3Volume.RightToLeft = RightToLeft.No;
            _txtMP3Volume.ScrollBars = ScrollBars.None;
            _txtMP3Volume.TabStop = true;
            _txtMP3Volume.Visible = true;
            _txtMP3Volume.BorderStyle = BorderStyle.Fixed3D;
            _txtMP3Volume.Name = "_txtMP3Volume";
            _hscMP3Volume.Size = new Size(153, 17);
            _hscMP3Volume.LargeChange = 10;
            _hscMP3Volume.Location = new Point(128, 328);
            _hscMP3Volume.Maximum = 109;
            _hscMP3Volume.TabIndex = 12;
            _hscMP3Volume.Value = 50;
            _hscMP3Volume.CausesValidation = true;
            _hscMP3Volume.Enabled = true;
            _hscMP3Volume.Minimum = 0;
            _hscMP3Volume.Cursor = Cursors.Default;
            _hscMP3Volume.RightToLeft = RightToLeft.No;
            _hscMP3Volume.SmallChange = 1;
            _hscMP3Volume.TabStop = true;
            _hscMP3Volume.Visible = true;
            _hscMP3Volume.Name = "_hscMP3Volume";
            cboMidiReset.BackColor = Color.White;
            cboMidiReset.ForeColor = Color.Black;
            cboMidiReset.Size = new Size(115, 20);
            cboMidiReset.Location = new Point(168, 296);
            cboMidiReset.TabIndex = 10;
            cboMidiReset.CausesValidation = true;
            cboMidiReset.Enabled = true;
            cboMidiReset.IntegralHeight = true;
            cboMidiReset.Cursor = Cursors.Default;
            cboMidiReset.RightToLeft = RightToLeft.No;
            cboMidiReset.Sorted = false;
            cboMidiReset.DropDownStyle = ComboBoxStyle.DropDown;
            cboMidiReset.TabStop = true;
            cboMidiReset.Visible = true;
            cboMidiReset.Name = "cboMidiReset";
            chkUseDirectMusic.BackColor = Color.FromArgb(192, 192, 192);
            chkUseDirectMusic.Text = "MIDI演奏にDirectMusicを使用する (要再起動)";
            chkUseDirectMusic.ForeColor = Color.Black;
            chkUseDirectMusic.Size = new Size(281, 25);
            chkUseDirectMusic.Location = new Point(32, 272);
            chkUseDirectMusic.TabIndex = 9;
            chkUseDirectMusic.CheckAlign = ContentAlignment.MiddleLeft;
            chkUseDirectMusic.FlatStyle = FlatStyle.Standard;
            chkUseDirectMusic.CausesValidation = true;
            chkUseDirectMusic.Enabled = true;
            chkUseDirectMusic.Cursor = Cursors.Default;
            chkUseDirectMusic.RightToLeft = RightToLeft.No;
            chkUseDirectMusic.Appearance = Appearance.Normal;
            chkUseDirectMusic.TabStop = true;
            chkUseDirectMusic.CheckState = CheckState.Unchecked;
            chkUseDirectMusic.Visible = true;
            chkUseDirectMusic.Name = "chkUseDirectMusic";
            _cmdCancel.TextAlign = ContentAlignment.MiddleCenter;
            _cmdCancel.BackColor = Color.FromArgb(192, 192, 192);
            _cmdCancel.Text = "キャンセル";
            _cmdCancel.Size = new Size(97, 25);
            _cmdCancel.Location = new Point(216, 360);
            _cmdCancel.TabIndex = 14;
            _cmdCancel.CausesValidation = true;
            _cmdCancel.Enabled = true;
            _cmdCancel.ForeColor = SystemColors.ControlText;
            _cmdCancel.Cursor = Cursors.Default;
            _cmdCancel.RightToLeft = RightToLeft.No;
            _cmdCancel.TabStop = true;
            _cmdCancel.Name = "_cmdCancel";
            _cmdOK.TextAlign = ContentAlignment.MiddleCenter;
            _cmdOK.BackColor = Color.FromArgb(192, 192, 192);
            _cmdOK.Text = "OK";
            _cmdOK.Size = new Size(97, 25);
            _cmdOK.Location = new Point(112, 360);
            _cmdOK.TabIndex = 13;
            _cmdOK.CausesValidation = true;
            _cmdOK.Enabled = true;
            _cmdOK.ForeColor = SystemColors.ControlText;
            _cmdOK.Cursor = Cursors.Default;
            _cmdOK.RightToLeft = RightToLeft.No;
            _cmdOK.TabStop = true;
            _cmdOK.Name = "_cmdOK";
            chkKeepEnemyBGM.BackColor = Color.FromArgb(192, 192, 192);
            chkKeepEnemyBGM.Text = "敵フェイズ中にＢＧＭを変更しない";
            chkKeepEnemyBGM.ForeColor = Color.Black;
            chkKeepEnemyBGM.Size = new Size(249, 25);
            chkKeepEnemyBGM.Location = new Point(32, 248);
            chkKeepEnemyBGM.TabIndex = 8;
            chkKeepEnemyBGM.CheckAlign = ContentAlignment.MiddleLeft;
            chkKeepEnemyBGM.FlatStyle = FlatStyle.Standard;
            chkKeepEnemyBGM.CausesValidation = true;
            chkKeepEnemyBGM.Enabled = true;
            chkKeepEnemyBGM.Cursor = Cursors.Default;
            chkKeepEnemyBGM.RightToLeft = RightToLeft.No;
            chkKeepEnemyBGM.Appearance = Appearance.Normal;
            chkKeepEnemyBGM.TabStop = true;
            chkKeepEnemyBGM.CheckState = CheckState.Unchecked;
            chkKeepEnemyBGM.Visible = true;
            chkKeepEnemyBGM.Name = "chkKeepEnemyBGM";
            cboMessageSpeed.BackColor = Color.White;
            cboMessageSpeed.ForeColor = Color.Black;
            cboMessageSpeed.Size = new Size(137, 20);
            cboMessageSpeed.Location = new Point(144, 16);
            cboMessageSpeed.TabIndex = 2;
            cboMessageSpeed.CausesValidation = true;
            cboMessageSpeed.Enabled = true;
            cboMessageSpeed.IntegralHeight = true;
            cboMessageSpeed.Cursor = Cursors.Default;
            cboMessageSpeed.RightToLeft = RightToLeft.No;
            cboMessageSpeed.Sorted = false;
            cboMessageSpeed.DropDownStyle = ComboBoxStyle.DropDown;
            cboMessageSpeed.TabStop = true;
            cboMessageSpeed.Visible = true;
            cboMessageSpeed.Name = "cboMessageSpeed";
            chkAutoMoveCursor.BackColor = Color.FromArgb(192, 192, 192);
            chkAutoMoveCursor.Text = "マウスカーソルを自動的に移動する";
            chkAutoMoveCursor.ForeColor = Color.Black;
            chkAutoMoveCursor.Size = new Size(249, 25);
            chkAutoMoveCursor.Location = new Point(32, 176);
            chkAutoMoveCursor.TabIndex = 5;
            chkAutoMoveCursor.CheckAlign = ContentAlignment.MiddleLeft;
            chkAutoMoveCursor.FlatStyle = FlatStyle.Standard;
            chkAutoMoveCursor.CausesValidation = true;
            chkAutoMoveCursor.Enabled = true;
            chkAutoMoveCursor.Cursor = Cursors.Default;
            chkAutoMoveCursor.RightToLeft = RightToLeft.No;
            chkAutoMoveCursor.Appearance = Appearance.Normal;
            chkAutoMoveCursor.TabStop = true;
            chkAutoMoveCursor.CheckState = CheckState.Unchecked;
            chkAutoMoveCursor.Visible = true;
            chkAutoMoveCursor.Name = "chkAutoMoveCursor";
            chkShowSquareLine.BackColor = Color.FromArgb(192, 192, 192);
            chkShowSquareLine.Text = "マス目を表示する (要再起動)";
            chkShowSquareLine.ForeColor = Color.Black;
            chkShowSquareLine.Size = new Size(265, 25);
            chkShowSquareLine.Location = new Point(32, 200);
            chkShowSquareLine.TabIndex = 6;
            chkShowSquareLine.CheckAlign = ContentAlignment.MiddleLeft;
            chkShowSquareLine.FlatStyle = FlatStyle.Standard;
            chkShowSquareLine.CausesValidation = true;
            chkShowSquareLine.Enabled = true;
            chkShowSquareLine.Cursor = Cursors.Default;
            chkShowSquareLine.RightToLeft = RightToLeft.No;
            chkShowSquareLine.Appearance = Appearance.Normal;
            chkShowSquareLine.TabStop = true;
            chkShowSquareLine.CheckState = CheckState.Unchecked;
            chkShowSquareLine.Visible = true;
            chkShowSquareLine.Name = "chkShowSquareLine";
            chkShowTurn.BackColor = Color.FromArgb(192, 192, 192);
            chkShowTurn.Text = "味方フェイズ開始時にターン表示を行う";
            chkShowTurn.ForeColor = Color.Black;
            chkShowTurn.Size = new Size(249, 25);
            chkShowTurn.Location = new Point(32, 224);
            chkShowTurn.TabIndex = 7;
            chkShowTurn.CheckAlign = ContentAlignment.MiddleLeft;
            chkShowTurn.FlatStyle = FlatStyle.Standard;
            chkShowTurn.CausesValidation = true;
            chkShowTurn.Enabled = true;
            chkShowTurn.Cursor = Cursors.Default;
            chkShowTurn.RightToLeft = RightToLeft.No;
            chkShowTurn.Appearance = Appearance.Normal;
            chkShowTurn.TabStop = true;
            chkShowTurn.CheckState = CheckState.Unchecked;
            chkShowTurn.Visible = true;
            chkShowTurn.Name = "chkShowTurn";
            chkSpecialPowerAnimation.BackColor = Color.FromArgb(192, 192, 192);
            chkSpecialPowerAnimation.Text = "スペシャルパワーアニメを表示する";
            chkSpecialPowerAnimation.ForeColor = Color.Black;
            chkSpecialPowerAnimation.Size = new Size(249, 25);
            chkSpecialPowerAnimation.Location = new Point(32, 128);
            chkSpecialPowerAnimation.TabIndex = 4;
            chkSpecialPowerAnimation.CheckAlign = ContentAlignment.MiddleLeft;
            chkSpecialPowerAnimation.FlatStyle = FlatStyle.Standard;
            chkSpecialPowerAnimation.CausesValidation = true;
            chkSpecialPowerAnimation.Enabled = true;
            chkSpecialPowerAnimation.Cursor = Cursors.Default;
            chkSpecialPowerAnimation.RightToLeft = RightToLeft.No;
            chkSpecialPowerAnimation.Appearance = Appearance.Normal;
            chkSpecialPowerAnimation.TabStop = true;
            chkSpecialPowerAnimation.CheckState = CheckState.Unchecked;
            chkSpecialPowerAnimation.Visible = true;
            chkSpecialPowerAnimation.Name = "chkSpecialPowerAnimation";
            _chkBattleAnimation.BackColor = Color.FromArgb(192, 192, 192);
            _chkBattleAnimation.Text = "戦闘アニメを表示する";
            _chkBattleAnimation.ForeColor = Color.Black;
            _chkBattleAnimation.Size = new Size(249, 25);
            _chkBattleAnimation.Location = new Point(32, 40);
            _chkBattleAnimation.TabIndex = 3;
            _chkBattleAnimation.CheckAlign = ContentAlignment.MiddleLeft;
            _chkBattleAnimation.FlatStyle = FlatStyle.Standard;
            _chkBattleAnimation.CausesValidation = true;
            _chkBattleAnimation.Enabled = true;
            _chkBattleAnimation.Cursor = Cursors.Default;
            _chkBattleAnimation.RightToLeft = RightToLeft.No;
            _chkBattleAnimation.Appearance = Appearance.Normal;
            _chkBattleAnimation.TabStop = true;
            _chkBattleAnimation.CheckState = CheckState.Unchecked;
            _chkBattleAnimation.Visible = true;
            _chkBattleAnimation.Name = "_chkBattleAnimation";
            labMP3Volume.Text = "MP3音量";
            labMP3Volume.ForeColor = Color.Black;
            labMP3Volume.Size = new Size(49, 17);
            labMP3Volume.Location = new Point(33, 330);
            labMP3Volume.TabIndex = 15;
            labMP3Volume.TextAlign = ContentAlignment.TopLeft;
            labMP3Volume.BackColor = Color.Transparent;
            labMP3Volume.Enabled = true;
            labMP3Volume.Cursor = Cursors.Default;
            labMP3Volume.RightToLeft = RightToLeft.No;
            labMP3Volume.UseMnemonic = true;
            labMP3Volume.Visible = true;
            labMP3Volume.AutoSize = false;
            labMP3Volume.BorderStyle = BorderStyle.None;
            labMP3Volume.Name = "labMP3Volume";
            labMidiReset.Text = "MIDI音源リセットの種類";
            labMidiReset.ForeColor = Color.Black;
            labMidiReset.Size = new Size(192, 17);
            labMidiReset.Location = new Point(33, 301);
            labMidiReset.TabIndex = 1;
            labMidiReset.TextAlign = ContentAlignment.TopLeft;
            labMidiReset.BackColor = Color.Transparent;
            labMidiReset.Enabled = true;
            labMidiReset.Cursor = Cursors.Default;
            labMidiReset.RightToLeft = RightToLeft.No;
            labMidiReset.UseMnemonic = true;
            labMidiReset.Visible = true;
            labMidiReset.AutoSize = false;
            labMidiReset.BorderStyle = BorderStyle.None;
            labMidiReset.Name = "labMidiReset";
            labMessageSpeed.Text = "メッセージスピード";
            labMessageSpeed.Font = new Font("ＭＳ Ｐゴシック", 9.75f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            labMessageSpeed.ForeColor = Color.Black;
            labMessageSpeed.Size = new Size(129, 17);
            labMessageSpeed.Location = new Point(33, 20);
            labMessageSpeed.TabIndex = 0;
            labMessageSpeed.TextAlign = ContentAlignment.TopLeft;
            labMessageSpeed.BackColor = Color.Transparent;
            labMessageSpeed.Enabled = true;
            labMessageSpeed.Cursor = Cursors.Default;
            labMessageSpeed.RightToLeft = RightToLeft.No;
            labMessageSpeed.UseMnemonic = true;
            labMessageSpeed.Visible = true;
            labMessageSpeed.AutoSize = false;
            labMessageSpeed.BorderStyle = BorderStyle.None;
            labMessageSpeed.Name = "labMessageSpeed";
            Controls.Add(chkExtendedAnimation);
            Controls.Add(chkMoveAnimation);
            Controls.Add(chkWeaponAnimation);
            Controls.Add(_txtMP3Volume);
            Controls.Add(_hscMP3Volume);
            Controls.Add(cboMidiReset);
            Controls.Add(chkUseDirectMusic);
            Controls.Add(_cmdCancel);
            Controls.Add(_cmdOK);
            Controls.Add(chkKeepEnemyBGM);
            Controls.Add(cboMessageSpeed);
            Controls.Add(chkAutoMoveCursor);
            Controls.Add(chkShowSquareLine);
            Controls.Add(chkShowTurn);
            Controls.Add(chkSpecialPowerAnimation);
            Controls.Add(_chkBattleAnimation);
            Controls.Add(labMP3Volume);
            Controls.Add(labMidiReset);
            Controls.Add(labMessageSpeed);
            Load += new EventHandler(frmConfiguration_Load);
            ResumeLayout(false);
            PerformLayout();
        }
        /* TODO ERROR: Skipped EndRegionDirectiveTrivia */
    }
}
