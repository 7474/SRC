using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    [DesignerGenerated()]
    internal partial class frmSafeMain
    {
        /* TODO ERROR: Skipped RegionDirectiveTrivia */
        [DebuggerNonUserCode()]
        public frmSafeMain() : base()
        {
            // この呼び出しは、Windows フォーム デザイナで必要です。
            InitializeComponent();
            _picFace.Name = "picFace";
            _HScroll_Renamed.Name = "HScroll_Renamed";
            _VScroll_Renamed.Name = "VScroll_Renamed";
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
        public ToolStripMenuItem _mnuUnitCommandItem_0;
        public ToolStripMenuItem _mnuUnitCommandItem_1;
        public ToolStripMenuItem _mnuUnitCommandItem_2;
        public ToolStripMenuItem _mnuUnitCommandItem_3;
        public ToolStripMenuItem _mnuUnitCommandItem_4;
        public ToolStripMenuItem _mnuUnitCommandItem_5;
        public ToolStripMenuItem _mnuUnitCommandItem_6;
        public ToolStripMenuItem _mnuUnitCommandItem_7;
        public ToolStripMenuItem _mnuUnitCommandItem_8;
        public ToolStripMenuItem _mnuUnitCommandItem_9;
        public ToolStripMenuItem _mnuUnitCommandItem_10;
        public ToolStripMenuItem _mnuUnitCommandItem_11;
        public ToolStripMenuItem _mnuUnitCommandItem_12;
        public ToolStripMenuItem _mnuUnitCommandItem_13;
        public ToolStripMenuItem _mnuUnitCommandItem_14;
        public ToolStripMenuItem _mnuUnitCommandItem_15;
        public ToolStripMenuItem _mnuUnitCommandItem_16;
        public ToolStripMenuItem _mnuUnitCommandItem_17;
        public ToolStripMenuItem _mnuUnitCommandItem_18;
        public ToolStripMenuItem _mnuUnitCommandItem_19;
        public ToolStripMenuItem _mnuUnitCommandItem_20;
        public ToolStripMenuItem _mnuUnitCommandItem_21;
        public ToolStripMenuItem _mnuUnitCommandItem_22;
        public ToolStripMenuItem _mnuUnitCommandItem_23;
        public ToolStripMenuItem _mnuUnitCommandItem_24;
        public ToolStripMenuItem _mnuUnitCommandItem_25;
        public ToolStripMenuItem _mnuUnitCommandItem_26;
        public ToolStripMenuItem _mnuUnitCommandItem_27;
        public ToolStripMenuItem _mnuUnitCommandItem_28;
        public ToolStripMenuItem _mnuUnitCommandItem_29;
        public ToolStripMenuItem _mnuUnitCommandItem_30;
        public ToolStripMenuItem _mnuUnitCommandItem_31;
        public ToolStripMenuItem _mnuUnitCommandItem_32;
        public ToolStripMenuItem _mnuUnitCommandItem_33;
        public ToolStripMenuItem _mnuUnitCommandItem_34;
        public ToolStripMenuItem _mnuUnitCommandItem_35;
        public ToolStripMenuItem mnuUnitCommand;
        public ToolStripMenuItem _mnuMapCommandItem_0;
        public ToolStripMenuItem _mnuMapCommandItem_1;
        public ToolStripMenuItem _mnuMapCommandItem_2;
        public ToolStripMenuItem _mnuMapCommandItem_3;
        public ToolStripMenuItem _mnuMapCommandItem_4;
        public ToolStripMenuItem _mnuMapCommandItem_5;
        public ToolStripMenuItem _mnuMapCommandItem_6;
        public ToolStripMenuItem _mnuMapCommandItem_7;
        public ToolStripMenuItem _mnuMapCommandItem_8;
        public ToolStripMenuItem _mnuMapCommandItem_9;
        public ToolStripMenuItem _mnuMapCommandItem_10;
        public ToolStripMenuItem _mnuMapCommandItem_11;
        public ToolStripMenuItem _mnuMapCommandItem_12;
        public ToolStripMenuItem _mnuMapCommandItem_13;
        public ToolStripMenuItem _mnuMapCommandItem_14;
        public ToolStripMenuItem _mnuMapCommandItem_15;
        public ToolStripMenuItem _mnuMapCommandItem_16;
        public ToolStripMenuItem _mnuMapCommandItem_17;
        public ToolStripMenuItem _mnuMapCommandItem_18;
        public ToolStripMenuItem _mnuMapCommandItem_19;
        public ToolStripMenuItem _mnuMapCommandItem_20;
        public ToolStripMenuItem mnuMapCommand;
        public MenuStrip MainMenu1;
        public PictureBox _picStretchedTmp_1;
        public PictureBox _picStretchedTmp_0;
        public PictureBox _picMain_1;
        public PictureBox _picBuf_0;
        public PictureBox _picTmp32_2;
        public PictureBox _picTmp32_1;
        private PictureBox _picFace;

        public PictureBox picFace
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _picFace;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_picFace != null)
                {
                    _picFace.Click -= picFace_Click;
                    _picFace.DoubleClick -= picFace_DoubleClick;
                }

                _picFace = value;
                if (_picFace != null)
                {
                    _picFace.Click += picFace_Click;
                    _picFace.DoubleClick += picFace_DoubleClick;
                }
            }
        }

        public PictureBox _picTmp32_0;
        public PictureBox picMaskedBack;
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

        public PictureBox picMask2;
        public PictureBox picNeautral;
        public PictureBox picEnemy;
        public PictureBox picUnit;
        public PictureBox picPilotStatus;
        public PictureBox picUnitStatus;
        public PictureBox picUnitBitmap;
        private HScrollBar _HScroll_Renamed;

        public HScrollBar HScroll_Renamed
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _HScroll_Renamed;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_HScroll_Renamed != null)
                {
                    _HScroll_Renamed.Scroll -= HScroll_Renamed_Scroll;
                }

                _HScroll_Renamed = value;
                if (_HScroll_Renamed != null)
                {
                    _HScroll_Renamed.Scroll += HScroll_Renamed_Scroll;
                }
            }
        }

        private VScrollBar _VScroll_Renamed;

        public VScrollBar VScroll_Renamed
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _VScroll_Renamed;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_VScroll_Renamed != null)
                {
                    _VScroll_Renamed.Scroll -= VScroll_Renamed_Scroll;
                }

                _VScroll_Renamed = value;
                if (_VScroll_Renamed != null)
                {
                    _VScroll_Renamed.Scroll += VScroll_Renamed_Scroll;
                }
            }
        }

        public PictureBox picMask;
        public PictureBox picTmp;
        public PictureBox picBack;
        public PictureBox _picMain_0;
        private Microsoft.VisualBasic.Compatibility.VB6.ToolStripMenuItemArray _mnuMapCommandItem;

        public Microsoft.VisualBasic.Compatibility.VB6.ToolStripMenuItemArray mnuMapCommandItem
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _mnuMapCommandItem;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_mnuMapCommandItem != null)
                {
                    _mnuMapCommandItem.Click -= mnuMapCommandItem_Click;
                }

                _mnuMapCommandItem = value;
                if (_mnuMapCommandItem != null)
                {
                    _mnuMapCommandItem.Click += mnuMapCommandItem_Click;
                }
            }
        }

        private Microsoft.VisualBasic.Compatibility.VB6.ToolStripMenuItemArray _mnuUnitCommandItem;

        public Microsoft.VisualBasic.Compatibility.VB6.ToolStripMenuItemArray mnuUnitCommandItem
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _mnuUnitCommandItem;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_mnuUnitCommandItem != null)
                {
                    _mnuUnitCommandItem.Click -= mnuUnitCommandItem_Click;
                }

                _mnuUnitCommandItem = value;
                if (_mnuUnitCommandItem != null)
                {
                    _mnuUnitCommandItem.Click += mnuUnitCommandItem_Click;
                }
            }
        }

        public Microsoft.VisualBasic.Compatibility.VB6.PictureBoxArray picBuf;
        private Microsoft.VisualBasic.Compatibility.VB6.PictureBoxArray _picMain;

        public Microsoft.VisualBasic.Compatibility.VB6.PictureBoxArray picMain
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _picMain;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_picMain != null)
                {
                    _picMain.DoubleClick -= picMain_DoubleClick;
                    _picMain.MouseDown -= picMain_MouseDown;
                    _picMain.MouseMove -= picMain_MouseMove;
                    _picMain.MouseUp -= picMain_MouseUp;
                }

                _picMain = value;
                if (_picMain != null)
                {
                    _picMain.DoubleClick += picMain_DoubleClick;
                    _picMain.MouseDown += picMain_MouseDown;
                    _picMain.MouseMove += picMain_MouseMove;
                    _picMain.MouseUp += picMain_MouseUp;
                }
            }
        }

        public Microsoft.VisualBasic.Compatibility.VB6.PictureBoxArray picStretchedTmp;
        public Microsoft.VisualBasic.Compatibility.VB6.PictureBoxArray picTmp32;
        // メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
        // Windows フォーム デザイナを使って変更できます。
        // コード エディタを使用して、変更しないでください。
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            var resources = new System.Resources.ResourceManager(typeof(frmSafeMain));
            components = new System.ComponentModel.Container();
            ToolTip1 = new ToolTip(components);
            MainMenu1 = new MenuStrip();
            mnuUnitCommand = new ToolStripMenuItem();
            _mnuUnitCommandItem_0 = new ToolStripMenuItem();
            _mnuUnitCommandItem_1 = new ToolStripMenuItem();
            _mnuUnitCommandItem_2 = new ToolStripMenuItem();
            _mnuUnitCommandItem_3 = new ToolStripMenuItem();
            _mnuUnitCommandItem_4 = new ToolStripMenuItem();
            _mnuUnitCommandItem_5 = new ToolStripMenuItem();
            _mnuUnitCommandItem_6 = new ToolStripMenuItem();
            _mnuUnitCommandItem_7 = new ToolStripMenuItem();
            _mnuUnitCommandItem_8 = new ToolStripMenuItem();
            _mnuUnitCommandItem_9 = new ToolStripMenuItem();
            _mnuUnitCommandItem_10 = new ToolStripMenuItem();
            _mnuUnitCommandItem_11 = new ToolStripMenuItem();
            _mnuUnitCommandItem_12 = new ToolStripMenuItem();
            _mnuUnitCommandItem_13 = new ToolStripMenuItem();
            _mnuUnitCommandItem_14 = new ToolStripMenuItem();
            _mnuUnitCommandItem_15 = new ToolStripMenuItem();
            _mnuUnitCommandItem_16 = new ToolStripMenuItem();
            _mnuUnitCommandItem_17 = new ToolStripMenuItem();
            _mnuUnitCommandItem_18 = new ToolStripMenuItem();
            _mnuUnitCommandItem_19 = new ToolStripMenuItem();
            _mnuUnitCommandItem_20 = new ToolStripMenuItem();
            _mnuUnitCommandItem_21 = new ToolStripMenuItem();
            _mnuUnitCommandItem_22 = new ToolStripMenuItem();
            _mnuUnitCommandItem_23 = new ToolStripMenuItem();
            _mnuUnitCommandItem_24 = new ToolStripMenuItem();
            _mnuUnitCommandItem_25 = new ToolStripMenuItem();
            _mnuUnitCommandItem_26 = new ToolStripMenuItem();
            _mnuUnitCommandItem_27 = new ToolStripMenuItem();
            _mnuUnitCommandItem_28 = new ToolStripMenuItem();
            _mnuUnitCommandItem_29 = new ToolStripMenuItem();
            _mnuUnitCommandItem_30 = new ToolStripMenuItem();
            _mnuUnitCommandItem_31 = new ToolStripMenuItem();
            _mnuUnitCommandItem_32 = new ToolStripMenuItem();
            _mnuUnitCommandItem_33 = new ToolStripMenuItem();
            _mnuUnitCommandItem_34 = new ToolStripMenuItem();
            _mnuUnitCommandItem_35 = new ToolStripMenuItem();
            mnuMapCommand = new ToolStripMenuItem();
            _mnuMapCommandItem_0 = new ToolStripMenuItem();
            _mnuMapCommandItem_1 = new ToolStripMenuItem();
            _mnuMapCommandItem_2 = new ToolStripMenuItem();
            _mnuMapCommandItem_3 = new ToolStripMenuItem();
            _mnuMapCommandItem_4 = new ToolStripMenuItem();
            _mnuMapCommandItem_5 = new ToolStripMenuItem();
            _mnuMapCommandItem_6 = new ToolStripMenuItem();
            _mnuMapCommandItem_7 = new ToolStripMenuItem();
            _mnuMapCommandItem_8 = new ToolStripMenuItem();
            _mnuMapCommandItem_9 = new ToolStripMenuItem();
            _mnuMapCommandItem_10 = new ToolStripMenuItem();
            _mnuMapCommandItem_11 = new ToolStripMenuItem();
            _mnuMapCommandItem_12 = new ToolStripMenuItem();
            _mnuMapCommandItem_13 = new ToolStripMenuItem();
            _mnuMapCommandItem_14 = new ToolStripMenuItem();
            _mnuMapCommandItem_15 = new ToolStripMenuItem();
            _mnuMapCommandItem_16 = new ToolStripMenuItem();
            _mnuMapCommandItem_17 = new ToolStripMenuItem();
            _mnuMapCommandItem_18 = new ToolStripMenuItem();
            _mnuMapCommandItem_19 = new ToolStripMenuItem();
            _mnuMapCommandItem_20 = new ToolStripMenuItem();
            _picStretchedTmp_1 = new PictureBox();
            _picStretchedTmp_0 = new PictureBox();
            _picMain_1 = new PictureBox();
            _picBuf_0 = new PictureBox();
            _picTmp32_2 = new PictureBox();
            _picTmp32_1 = new PictureBox();
            _picFace = new PictureBox();
            _picFace.Click += new EventHandler(picFace_Click);
            _picFace.DoubleClick += new EventHandler(picFace_DoubleClick);
            _picTmp32_0 = new PictureBox();
            picMaskedBack = new PictureBox();
            _Timer1 = new Timer(components);
            _Timer1.Tick += new EventHandler(Timer1_Tick);
            picMask2 = new PictureBox();
            picNeautral = new PictureBox();
            picEnemy = new PictureBox();
            picUnit = new PictureBox();
            picPilotStatus = new PictureBox();
            picUnitStatus = new PictureBox();
            picUnitBitmap = new PictureBox();
            _HScroll_Renamed = new HScrollBar();
            _HScroll_Renamed.Scroll += new ScrollEventHandler(HScroll_Renamed_Scroll);
            _VScroll_Renamed = new VScrollBar();
            _VScroll_Renamed.Scroll += new ScrollEventHandler(VScroll_Renamed_Scroll);
            picMask = new PictureBox();
            picTmp = new PictureBox();
            picBack = new PictureBox();
            _picMain_0 = new PictureBox();
            _mnuMapCommandItem = new Microsoft.VisualBasic.Compatibility.VB6.ToolStripMenuItemArray(components);
            _mnuMapCommandItem.Click += new EventHandler(mnuMapCommandItem_Click);
            _mnuUnitCommandItem = new Microsoft.VisualBasic.Compatibility.VB6.ToolStripMenuItemArray(components);
            _mnuUnitCommandItem.Click += new EventHandler(mnuUnitCommandItem_Click);
            picBuf = new Microsoft.VisualBasic.Compatibility.VB6.PictureBoxArray(components);
            _picMain = new Microsoft.VisualBasic.Compatibility.VB6.PictureBoxArray(components);
            _picMain.DoubleClick += new EventHandler(picMain_DoubleClick);
            _picMain.MouseDown += new MouseEventHandler(picMain_MouseDown);
            _picMain.MouseMove += new MouseEventHandler(picMain_MouseMove);
            _picMain.MouseUp += new MouseEventHandler(picMain_MouseUp);
            picStretchedTmp = new Microsoft.VisualBasic.Compatibility.VB6.PictureBoxArray(components);
            picTmp32 = new Microsoft.VisualBasic.Compatibility.VB6.PictureBoxArray(components);
            MainMenu1.SuspendLayout();
            SuspendLayout();
            ToolTip1.Active = true;
            ((System.ComponentModel.ISupportInitialize)_mnuMapCommandItem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_mnuUnitCommandItem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picBuf).BeginInit();
            ((System.ComponentModel.ISupportInitialize)_picMain).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picStretchedTmp).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picTmp32).BeginInit();
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.FromArgb(192, 192, 192);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Text = "SRC開発版";
            ClientSize = new Size(508, 318);
            Location = new Point(81, 218);
            Font = new Font("ＭＳ 明朝", 9.75f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            Icon = (Icon)resources.GetObject("frmSafeMain.Icon");
            KeyPreview = true;
            MaximizeBox = false;
            Visible = false;
            AutoScaleMode = AutoScaleMode.Font;
            ControlBox = true;
            Enabled = true;
            MinimizeBox = true;
            Cursor = Cursors.Default;
            RightToLeft = RightToLeft.No;
            ShowInTaskbar = true;
            HelpButton = false;
            WindowState = FormWindowState.Normal;
            Name = "frmSafeMain";
            mnuUnitCommand.Name = "mnuUnitCommand";
            mnuUnitCommand.Text = "ユニットコマンド";
            mnuUnitCommand.Visible = false;
            mnuUnitCommand.Checked = false;
            mnuUnitCommand.Enabled = true;
            _mnuUnitCommandItem_0.Name = "_mnuUnitCommandItem_0";
            _mnuUnitCommandItem_0.Text = "移動";
            _mnuUnitCommandItem_0.Checked = false;
            _mnuUnitCommandItem_0.Enabled = true;
            _mnuUnitCommandItem_0.Visible = true;
            _mnuUnitCommandItem_1.Name = "_mnuUnitCommandItem_1";
            _mnuUnitCommandItem_1.Text = "テレポート";
            _mnuUnitCommandItem_1.Visible = false;
            _mnuUnitCommandItem_1.Checked = false;
            _mnuUnitCommandItem_1.Enabled = true;
            _mnuUnitCommandItem_2.Name = "_mnuUnitCommandItem_2";
            _mnuUnitCommandItem_2.Text = "ジャンプ";
            _mnuUnitCommandItem_2.Visible = false;
            _mnuUnitCommandItem_2.Checked = false;
            _mnuUnitCommandItem_2.Enabled = true;
            _mnuUnitCommandItem_3.Name = "_mnuUnitCommandItem_3";
            _mnuUnitCommandItem_3.Text = "会話";
            _mnuUnitCommandItem_3.Visible = false;
            _mnuUnitCommandItem_3.Checked = false;
            _mnuUnitCommandItem_3.Enabled = true;
            _mnuUnitCommandItem_4.Name = "_mnuUnitCommandItem_4";
            _mnuUnitCommandItem_4.Text = "攻撃";
            _mnuUnitCommandItem_4.Checked = false;
            _mnuUnitCommandItem_4.Enabled = true;
            _mnuUnitCommandItem_4.Visible = true;
            _mnuUnitCommandItem_5.Name = "_mnuUnitCommandItem_5";
            _mnuUnitCommandItem_5.Text = "修理";
            _mnuUnitCommandItem_5.Checked = false;
            _mnuUnitCommandItem_5.Enabled = true;
            _mnuUnitCommandItem_5.Visible = true;
            _mnuUnitCommandItem_6.Name = "_mnuUnitCommandItem_6";
            _mnuUnitCommandItem_6.Text = "補給";
            _mnuUnitCommandItem_6.Checked = false;
            _mnuUnitCommandItem_6.Enabled = true;
            _mnuUnitCommandItem_6.Visible = true;
            _mnuUnitCommandItem_7.Name = "_mnuUnitCommandItem_7";
            _mnuUnitCommandItem_7.Text = "アビリティ";
            _mnuUnitCommandItem_7.Checked = false;
            _mnuUnitCommandItem_7.Enabled = true;
            _mnuUnitCommandItem_7.Visible = true;
            _mnuUnitCommandItem_8.Name = "_mnuUnitCommandItem_8";
            _mnuUnitCommandItem_8.Text = "チャージ";
            _mnuUnitCommandItem_8.Visible = false;
            _mnuUnitCommandItem_8.Checked = false;
            _mnuUnitCommandItem_8.Enabled = true;
            _mnuUnitCommandItem_9.Name = "_mnuUnitCommandItem_9";
            _mnuUnitCommandItem_9.Text = "スペシャルパワー";
            _mnuUnitCommandItem_9.Checked = false;
            _mnuUnitCommandItem_9.Enabled = true;
            _mnuUnitCommandItem_9.Visible = true;
            _mnuUnitCommandItem_10.Name = "_mnuUnitCommandItem_10";
            _mnuUnitCommandItem_10.Text = "変形";
            _mnuUnitCommandItem_10.Checked = false;
            _mnuUnitCommandItem_10.Enabled = true;
            _mnuUnitCommandItem_10.Visible = true;
            _mnuUnitCommandItem_11.Name = "_mnuUnitCommandItem_11";
            _mnuUnitCommandItem_11.Text = "分離";
            _mnuUnitCommandItem_11.Visible = false;
            _mnuUnitCommandItem_11.Checked = false;
            _mnuUnitCommandItem_11.Enabled = true;
            _mnuUnitCommandItem_12.Name = "_mnuUnitCommandItem_12";
            _mnuUnitCommandItem_12.Text = "合体";
            _mnuUnitCommandItem_12.Visible = false;
            _mnuUnitCommandItem_12.Checked = false;
            _mnuUnitCommandItem_12.Enabled = true;
            _mnuUnitCommandItem_13.Name = "_mnuUnitCommandItem_13";
            _mnuUnitCommandItem_13.Text = "ハイパーモード";
            _mnuUnitCommandItem_13.Checked = false;
            _mnuUnitCommandItem_13.Enabled = true;
            _mnuUnitCommandItem_13.Visible = true;
            _mnuUnitCommandItem_14.Name = "_mnuUnitCommandItem_14";
            _mnuUnitCommandItem_14.Text = "地上";
            _mnuUnitCommandItem_14.Checked = false;
            _mnuUnitCommandItem_14.Enabled = true;
            _mnuUnitCommandItem_14.Visible = true;
            _mnuUnitCommandItem_15.Name = "_mnuUnitCommandItem_15";
            _mnuUnitCommandItem_15.Text = "空中";
            _mnuUnitCommandItem_15.Checked = false;
            _mnuUnitCommandItem_15.Enabled = true;
            _mnuUnitCommandItem_15.Visible = true;
            _mnuUnitCommandItem_16.Name = "_mnuUnitCommandItem_16";
            _mnuUnitCommandItem_16.Text = "地中";
            _mnuUnitCommandItem_16.Checked = false;
            _mnuUnitCommandItem_16.Enabled = true;
            _mnuUnitCommandItem_16.Visible = true;
            _mnuUnitCommandItem_17.Name = "_mnuUnitCommandItem_17";
            _mnuUnitCommandItem_17.Text = "水中";
            _mnuUnitCommandItem_17.Checked = false;
            _mnuUnitCommandItem_17.Enabled = true;
            _mnuUnitCommandItem_17.Visible = true;
            _mnuUnitCommandItem_18.Name = "_mnuUnitCommandItem_18";
            _mnuUnitCommandItem_18.Text = "発進";
            _mnuUnitCommandItem_18.Checked = false;
            _mnuUnitCommandItem_18.Enabled = true;
            _mnuUnitCommandItem_18.Visible = true;
            _mnuUnitCommandItem_19.Name = "_mnuUnitCommandItem_19";
            _mnuUnitCommandItem_19.Text = "アイテム";
            _mnuUnitCommandItem_19.Checked = false;
            _mnuUnitCommandItem_19.Enabled = true;
            _mnuUnitCommandItem_19.Visible = true;
            _mnuUnitCommandItem_20.Name = "_mnuUnitCommandItem_20";
            _mnuUnitCommandItem_20.Text = "召喚解除";
            _mnuUnitCommandItem_20.Checked = false;
            _mnuUnitCommandItem_20.Enabled = true;
            _mnuUnitCommandItem_20.Visible = true;
            _mnuUnitCommandItem_21.Name = "_mnuUnitCommandItem_21";
            _mnuUnitCommandItem_21.Text = "命令";
            _mnuUnitCommandItem_21.Checked = false;
            _mnuUnitCommandItem_21.Enabled = true;
            _mnuUnitCommandItem_21.Visible = true;
            _mnuUnitCommandItem_22.Name = "_mnuUnitCommandItem_22";
            _mnuUnitCommandItem_22.Text = "特殊能力一覧";
            _mnuUnitCommandItem_22.Checked = false;
            _mnuUnitCommandItem_22.Enabled = true;
            _mnuUnitCommandItem_22.Visible = true;
            _mnuUnitCommandItem_23.Name = "_mnuUnitCommandItem_23";
            _mnuUnitCommandItem_23.Text = "武装一覧";
            _mnuUnitCommandItem_23.Checked = false;
            _mnuUnitCommandItem_23.Enabled = true;
            _mnuUnitCommandItem_23.Visible = true;
            _mnuUnitCommandItem_24.Name = "_mnuUnitCommandItem_24";
            _mnuUnitCommandItem_24.Text = "アビリティ一覧";
            _mnuUnitCommandItem_24.Checked = false;
            _mnuUnitCommandItem_24.Enabled = true;
            _mnuUnitCommandItem_24.Visible = true;
            _mnuUnitCommandItem_25.Name = "_mnuUnitCommandItem_25";
            _mnuUnitCommandItem_25.Text = "";
            _mnuUnitCommandItem_25.Visible = false;
            _mnuUnitCommandItem_25.Checked = false;
            _mnuUnitCommandItem_25.Enabled = true;
            _mnuUnitCommandItem_26.Name = "_mnuUnitCommandItem_26";
            _mnuUnitCommandItem_26.Text = "";
            _mnuUnitCommandItem_26.Visible = false;
            _mnuUnitCommandItem_26.Checked = false;
            _mnuUnitCommandItem_26.Enabled = true;
            _mnuUnitCommandItem_27.Name = "_mnuUnitCommandItem_27";
            _mnuUnitCommandItem_27.Text = "";
            _mnuUnitCommandItem_27.Visible = false;
            _mnuUnitCommandItem_27.Checked = false;
            _mnuUnitCommandItem_27.Enabled = true;
            _mnuUnitCommandItem_28.Name = "_mnuUnitCommandItem_28";
            _mnuUnitCommandItem_28.Text = "";
            _mnuUnitCommandItem_28.Visible = false;
            _mnuUnitCommandItem_28.Checked = false;
            _mnuUnitCommandItem_28.Enabled = true;
            _mnuUnitCommandItem_29.Name = "_mnuUnitCommandItem_29";
            _mnuUnitCommandItem_29.Text = "";
            _mnuUnitCommandItem_29.Visible = false;
            _mnuUnitCommandItem_29.Checked = false;
            _mnuUnitCommandItem_29.Enabled = true;
            _mnuUnitCommandItem_30.Name = "_mnuUnitCommandItem_30";
            _mnuUnitCommandItem_30.Text = "";
            _mnuUnitCommandItem_30.Visible = false;
            _mnuUnitCommandItem_30.Checked = false;
            _mnuUnitCommandItem_30.Enabled = true;
            _mnuUnitCommandItem_31.Name = "_mnuUnitCommandItem_31";
            _mnuUnitCommandItem_31.Text = "";
            _mnuUnitCommandItem_31.Visible = false;
            _mnuUnitCommandItem_31.Checked = false;
            _mnuUnitCommandItem_31.Enabled = true;
            _mnuUnitCommandItem_32.Name = "_mnuUnitCommandItem_32";
            _mnuUnitCommandItem_32.Text = "";
            _mnuUnitCommandItem_32.Visible = false;
            _mnuUnitCommandItem_32.Checked = false;
            _mnuUnitCommandItem_32.Enabled = true;
            _mnuUnitCommandItem_33.Name = "_mnuUnitCommandItem_33";
            _mnuUnitCommandItem_33.Text = "";
            _mnuUnitCommandItem_33.Visible = false;
            _mnuUnitCommandItem_33.Checked = false;
            _mnuUnitCommandItem_33.Enabled = true;
            _mnuUnitCommandItem_34.Name = "_mnuUnitCommandItem_34";
            _mnuUnitCommandItem_34.Text = "";
            _mnuUnitCommandItem_34.Visible = false;
            _mnuUnitCommandItem_34.Checked = false;
            _mnuUnitCommandItem_34.Enabled = true;
            _mnuUnitCommandItem_35.Name = "_mnuUnitCommandItem_35";
            _mnuUnitCommandItem_35.Text = "待機";
            _mnuUnitCommandItem_35.Checked = false;
            _mnuUnitCommandItem_35.Enabled = true;
            _mnuUnitCommandItem_35.Visible = true;
            mnuMapCommand.Name = "mnuMapCommand";
            mnuMapCommand.Text = "マップコマンド";
            mnuMapCommand.Visible = false;
            mnuMapCommand.Checked = false;
            mnuMapCommand.Enabled = true;
            _mnuMapCommandItem_0.Name = "_mnuMapCommandItem_0";
            _mnuMapCommandItem_0.Text = "ターン終了";
            _mnuMapCommandItem_0.Checked = false;
            _mnuMapCommandItem_0.Enabled = true;
            _mnuMapCommandItem_0.Visible = true;
            _mnuMapCommandItem_1.Name = "_mnuMapCommandItem_1";
            _mnuMapCommandItem_1.Text = "中断";
            _mnuMapCommandItem_1.Checked = false;
            _mnuMapCommandItem_1.Enabled = true;
            _mnuMapCommandItem_1.Visible = true;
            _mnuMapCommandItem_2.Name = "_mnuMapCommandItem_2";
            _mnuMapCommandItem_2.Text = "部隊表";
            _mnuMapCommandItem_2.Checked = false;
            _mnuMapCommandItem_2.Enabled = true;
            _mnuMapCommandItem_2.Visible = true;
            _mnuMapCommandItem_3.Name = "_mnuMapCommandItem_3";
            _mnuMapCommandItem_3.Text = "スペシャルパワー検索";
            _mnuMapCommandItem_3.Checked = false;
            _mnuMapCommandItem_3.Enabled = true;
            _mnuMapCommandItem_3.Visible = true;
            _mnuMapCommandItem_4.Name = "_mnuMapCommandItem_4";
            _mnuMapCommandItem_4.Text = "全体マップ";
            _mnuMapCommandItem_4.Checked = false;
            _mnuMapCommandItem_4.Enabled = true;
            _mnuMapCommandItem_4.Visible = true;
            _mnuMapCommandItem_5.Name = "_mnuMapCommandItem_5";
            _mnuMapCommandItem_5.Text = "作戦目的";
            _mnuMapCommandItem_5.Visible = false;
            _mnuMapCommandItem_5.Checked = false;
            _mnuMapCommandItem_5.Enabled = true;
            _mnuMapCommandItem_6.Name = "_mnuMapCommandItem_6";
            _mnuMapCommandItem_6.Text = "";
            _mnuMapCommandItem_6.Visible = false;
            _mnuMapCommandItem_6.Checked = false;
            _mnuMapCommandItem_6.Enabled = true;
            _mnuMapCommandItem_7.Name = "_mnuMapCommandItem_7";
            _mnuMapCommandItem_7.Text = "";
            _mnuMapCommandItem_7.Visible = false;
            _mnuMapCommandItem_7.Checked = false;
            _mnuMapCommandItem_7.Enabled = true;
            _mnuMapCommandItem_8.Name = "_mnuMapCommandItem_8";
            _mnuMapCommandItem_8.Text = "";
            _mnuMapCommandItem_8.Visible = false;
            _mnuMapCommandItem_8.Checked = false;
            _mnuMapCommandItem_8.Enabled = true;
            _mnuMapCommandItem_9.Name = "_mnuMapCommandItem_9";
            _mnuMapCommandItem_9.Text = "";
            _mnuMapCommandItem_9.Visible = false;
            _mnuMapCommandItem_9.Checked = false;
            _mnuMapCommandItem_9.Enabled = true;
            _mnuMapCommandItem_10.Name = "_mnuMapCommandItem_10";
            _mnuMapCommandItem_10.Text = "";
            _mnuMapCommandItem_10.Visible = false;
            _mnuMapCommandItem_10.Checked = false;
            _mnuMapCommandItem_10.Enabled = true;
            _mnuMapCommandItem_11.Name = "_mnuMapCommandItem_11";
            _mnuMapCommandItem_11.Text = "";
            _mnuMapCommandItem_11.Visible = false;
            _mnuMapCommandItem_11.Checked = false;
            _mnuMapCommandItem_11.Enabled = true;
            _mnuMapCommandItem_12.Name = "_mnuMapCommandItem_12";
            _mnuMapCommandItem_12.Text = "";
            _mnuMapCommandItem_12.Visible = false;
            _mnuMapCommandItem_12.Checked = false;
            _mnuMapCommandItem_12.Enabled = true;
            _mnuMapCommandItem_13.Name = "_mnuMapCommandItem_13";
            _mnuMapCommandItem_13.Text = "";
            _mnuMapCommandItem_13.Visible = false;
            _mnuMapCommandItem_13.Checked = false;
            _mnuMapCommandItem_13.Enabled = true;
            _mnuMapCommandItem_14.Name = "_mnuMapCommandItem_14";
            _mnuMapCommandItem_14.Text = "";
            _mnuMapCommandItem_14.Visible = false;
            _mnuMapCommandItem_14.Checked = false;
            _mnuMapCommandItem_14.Enabled = true;
            _mnuMapCommandItem_15.Name = "_mnuMapCommandItem_15";
            _mnuMapCommandItem_15.Text = "";
            _mnuMapCommandItem_15.Visible = false;
            _mnuMapCommandItem_15.Checked = false;
            _mnuMapCommandItem_15.Enabled = true;
            _mnuMapCommandItem_16.Name = "_mnuMapCommandItem_16";
            _mnuMapCommandItem_16.Text = "自動反撃モード";
            _mnuMapCommandItem_16.Checked = false;
            _mnuMapCommandItem_16.Enabled = true;
            _mnuMapCommandItem_16.Visible = true;
            _mnuMapCommandItem_17.Name = "_mnuMapCommandItem_17";
            _mnuMapCommandItem_17.Text = "設定変更";
            _mnuMapCommandItem_17.Checked = false;
            _mnuMapCommandItem_17.Enabled = true;
            _mnuMapCommandItem_17.Visible = true;
            _mnuMapCommandItem_18.Name = "_mnuMapCommandItem_18";
            _mnuMapCommandItem_18.Text = "リスタート";
            _mnuMapCommandItem_18.Visible = false;
            _mnuMapCommandItem_18.Checked = false;
            _mnuMapCommandItem_18.Enabled = true;
            _mnuMapCommandItem_19.Name = "_mnuMapCommandItem_19";
            _mnuMapCommandItem_19.Text = "クイックロード";
            _mnuMapCommandItem_19.Checked = false;
            _mnuMapCommandItem_19.Enabled = true;
            _mnuMapCommandItem_19.Visible = true;
            _mnuMapCommandItem_20.Name = "_mnuMapCommandItem_20";
            _mnuMapCommandItem_20.Text = "クイックセーブ";
            _mnuMapCommandItem_20.Checked = false;
            _mnuMapCommandItem_20.Enabled = true;
            _mnuMapCommandItem_20.Visible = true;
            _picStretchedTmp_1.BackColor = Color.White;
            _picStretchedTmp_1.ForeColor = Color.Black;
            _picStretchedTmp_1.Size = new Size(32, 32);
            _picStretchedTmp_1.Location = new Point(288, 224);
            _picStretchedTmp_1.TabIndex = 21;
            _picStretchedTmp_1.Visible = false;
            _picStretchedTmp_1.Dock = DockStyle.None;
            _picStretchedTmp_1.CausesValidation = true;
            _picStretchedTmp_1.Enabled = true;
            _picStretchedTmp_1.Cursor = Cursors.Default;
            _picStretchedTmp_1.RightToLeft = RightToLeft.No;
            _picStretchedTmp_1.TabStop = true;
            _picStretchedTmp_1.SizeMode = PictureBoxSizeMode.AutoSize;
            _picStretchedTmp_1.BorderStyle = BorderStyle.None;
            _picStretchedTmp_1.Name = "_picStretchedTmp_1";
            _picStretchedTmp_0.BackColor = Color.White;
            _picStretchedTmp_0.ForeColor = Color.Black;
            _picStretchedTmp_0.Size = new Size(32, 32);
            _picStretchedTmp_0.Location = new Point(288, 176);
            _picStretchedTmp_0.TabIndex = 20;
            _picStretchedTmp_0.Visible = false;
            _picStretchedTmp_0.Dock = DockStyle.None;
            _picStretchedTmp_0.CausesValidation = true;
            _picStretchedTmp_0.Enabled = true;
            _picStretchedTmp_0.Cursor = Cursors.Default;
            _picStretchedTmp_0.RightToLeft = RightToLeft.No;
            _picStretchedTmp_0.TabStop = true;
            _picStretchedTmp_0.SizeMode = PictureBoxSizeMode.AutoSize;
            _picStretchedTmp_0.BorderStyle = BorderStyle.None;
            _picStretchedTmp_0.Name = "_picStretchedTmp_0";
            _picMain_1.BackColor = Color.Black;
            _picMain_1.Font = new Font("ＭＳ Ｐ明朝", 15.75f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            _picMain_1.ForeColor = Color.White;
            _picMain_1.Size = new Size(81, 32);
            _picMain_1.Location = new Point(96, 32);
            _picMain_1.TabIndex = 13;
            _picMain_1.Visible = false;
            _picMain_1.Dock = DockStyle.None;
            _picMain_1.CausesValidation = true;
            _picMain_1.Enabled = true;
            _picMain_1.Cursor = Cursors.Default;
            _picMain_1.RightToLeft = RightToLeft.No;
            _picMain_1.TabStop = true;
            _picMain_1.SizeMode = PictureBoxSizeMode.Normal;
            _picMain_1.BorderStyle = BorderStyle.None;
            _picMain_1.Name = "_picMain_1";
            _picBuf_0.BackColor = Color.White;
            _picBuf_0.Font = new Font("ＭＳ Ｐゴシック", 9f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            _picBuf_0.ForeColor = Color.Black;
            _picBuf_0.Size = new Size(32, 32);
            _picBuf_0.Location = new Point(224, 72);
            _picBuf_0.TabIndex = 19;
            _picBuf_0.Visible = false;
            _picBuf_0.Dock = DockStyle.None;
            _picBuf_0.CausesValidation = true;
            _picBuf_0.Enabled = true;
            _picBuf_0.Cursor = Cursors.Default;
            _picBuf_0.RightToLeft = RightToLeft.No;
            _picBuf_0.TabStop = true;
            _picBuf_0.SizeMode = PictureBoxSizeMode.AutoSize;
            _picBuf_0.BorderStyle = BorderStyle.None;
            _picBuf_0.Name = "_picBuf_0";
            _picTmp32_2.BackColor = Color.White;
            _picTmp32_2.ForeColor = Color.Black;
            _picTmp32_2.Size = new Size(32, 32);
            _picTmp32_2.Location = new Point(240, 224);
            _picTmp32_2.TabIndex = 18;
            _picTmp32_2.Visible = false;
            _picTmp32_2.Dock = DockStyle.None;
            _picTmp32_2.CausesValidation = true;
            _picTmp32_2.Enabled = true;
            _picTmp32_2.Cursor = Cursors.Default;
            _picTmp32_2.RightToLeft = RightToLeft.No;
            _picTmp32_2.TabStop = true;
            _picTmp32_2.SizeMode = PictureBoxSizeMode.Normal;
            _picTmp32_2.BorderStyle = BorderStyle.None;
            _picTmp32_2.Name = "_picTmp32_2";
            _picTmp32_1.BackColor = Color.White;
            _picTmp32_1.ForeColor = Color.Black;
            _picTmp32_1.Size = new Size(32, 32);
            _picTmp32_1.Location = new Point(192, 264);
            _picTmp32_1.TabIndex = 17;
            _picTmp32_1.Visible = false;
            _picTmp32_1.Dock = DockStyle.None;
            _picTmp32_1.CausesValidation = true;
            _picTmp32_1.Enabled = true;
            _picTmp32_1.Cursor = Cursors.Default;
            _picTmp32_1.RightToLeft = RightToLeft.No;
            _picTmp32_1.TabStop = true;
            _picTmp32_1.SizeMode = PictureBoxSizeMode.Normal;
            _picTmp32_1.BorderStyle = BorderStyle.None;
            _picTmp32_1.Name = "_picTmp32_1";
            _picFace.BackColor = Color.FromArgb(192, 192, 192);
            _picFace.Size = new Size(68, 68);
            _picFace.Location = new Point(8, 192);
            _picFace.TabIndex = 16;
            _picFace.Dock = DockStyle.None;
            _picFace.CausesValidation = true;
            _picFace.Enabled = true;
            _picFace.ForeColor = SystemColors.ControlText;
            _picFace.Cursor = Cursors.Default;
            _picFace.RightToLeft = RightToLeft.No;
            _picFace.TabStop = true;
            _picFace.Visible = true;
            _picFace.SizeMode = PictureBoxSizeMode.Normal;
            _picFace.BorderStyle = BorderStyle.Fixed3D;
            _picFace.Name = "_picFace";
            _picTmp32_0.BackColor = Color.White;
            _picTmp32_0.ForeColor = Color.Black;
            _picTmp32_0.Size = new Size(32, 32);
            _picTmp32_0.Location = new Point(192, 224);
            _picTmp32_0.TabIndex = 15;
            _picTmp32_0.Visible = false;
            _picTmp32_0.Dock = DockStyle.None;
            _picTmp32_0.CausesValidation = true;
            _picTmp32_0.Enabled = true;
            _picTmp32_0.Cursor = Cursors.Default;
            _picTmp32_0.RightToLeft = RightToLeft.No;
            _picTmp32_0.TabStop = true;
            _picTmp32_0.SizeMode = PictureBoxSizeMode.Normal;
            _picTmp32_0.BorderStyle = BorderStyle.None;
            _picTmp32_0.Name = "_picTmp32_0";
            picMaskedBack.BackColor = Color.Black;
            picMaskedBack.ForeColor = Color.White;
            picMaskedBack.Size = new Size(81, 32);
            picMaskedBack.Location = new Point(288, 32);
            picMaskedBack.TabIndex = 14;
            picMaskedBack.Visible = false;
            picMaskedBack.Dock = DockStyle.None;
            picMaskedBack.CausesValidation = true;
            picMaskedBack.Enabled = true;
            picMaskedBack.Cursor = Cursors.Default;
            picMaskedBack.RightToLeft = RightToLeft.No;
            picMaskedBack.TabStop = true;
            picMaskedBack.SizeMode = PictureBoxSizeMode.Normal;
            picMaskedBack.BorderStyle = BorderStyle.None;
            picMaskedBack.Name = "picMaskedBack";
            _Timer1.Enabled = false;
            _Timer1.Interval = 1000;
            picMask2.Size = new Size(32, 32);
            picMask2.Location = new Point(8, 133);
            picMask2.Image = (Image)resources.GetObject("picMask2.Image");
            picMask2.TabIndex = 12;
            picMask2.Visible = false;
            picMask2.Dock = DockStyle.None;
            picMask2.BackColor = SystemColors.Control;
            picMask2.CausesValidation = true;
            picMask2.Enabled = true;
            picMask2.ForeColor = SystemColors.ControlText;
            picMask2.Cursor = Cursors.Default;
            picMask2.RightToLeft = RightToLeft.No;
            picMask2.TabStop = true;
            picMask2.SizeMode = PictureBoxSizeMode.AutoSize;
            picMask2.BorderStyle = BorderStyle.None;
            picMask2.Name = "picMask2";
            picNeautral.Size = new Size(32, 32);
            picNeautral.Location = new Point(176, 140);
            picNeautral.Image = (Image)resources.GetObject("picNeautral.Image");
            picNeautral.TabIndex = 11;
            picNeautral.Visible = false;
            picNeautral.Dock = DockStyle.None;
            picNeautral.BackColor = SystemColors.Control;
            picNeautral.CausesValidation = true;
            picNeautral.Enabled = true;
            picNeautral.ForeColor = SystemColors.ControlText;
            picNeautral.Cursor = Cursors.Default;
            picNeautral.RightToLeft = RightToLeft.No;
            picNeautral.TabStop = true;
            picNeautral.SizeMode = PictureBoxSizeMode.Normal;
            picNeautral.BorderStyle = BorderStyle.None;
            picNeautral.Name = "picNeautral";
            picEnemy.Size = new Size(32, 32);
            picEnemy.Location = new Point(124, 140);
            picEnemy.Image = (Image)resources.GetObject("picEnemy.Image");
            picEnemy.TabIndex = 10;
            picEnemy.Visible = false;
            picEnemy.Dock = DockStyle.None;
            picEnemy.BackColor = SystemColors.Control;
            picEnemy.CausesValidation = true;
            picEnemy.Enabled = true;
            picEnemy.ForeColor = SystemColors.ControlText;
            picEnemy.Cursor = Cursors.Default;
            picEnemy.RightToLeft = RightToLeft.No;
            picEnemy.TabStop = true;
            picEnemy.SizeMode = PictureBoxSizeMode.Normal;
            picEnemy.BorderStyle = BorderStyle.None;
            picEnemy.Name = "picEnemy";
            picUnit.Size = new Size(32, 32);
            picUnit.Location = new Point(76, 140);
            picUnit.Image = (Image)resources.GetObject("picUnit.Image");
            picUnit.TabIndex = 9;
            picUnit.Visible = false;
            picUnit.Dock = DockStyle.None;
            picUnit.BackColor = SystemColors.Control;
            picUnit.CausesValidation = true;
            picUnit.Enabled = true;
            picUnit.ForeColor = SystemColors.ControlText;
            picUnit.Cursor = Cursors.Default;
            picUnit.RightToLeft = RightToLeft.No;
            picUnit.TabStop = true;
            picUnit.SizeMode = PictureBoxSizeMode.Normal;
            picUnit.BorderStyle = BorderStyle.None;
            picUnit.Name = "picUnit";
            picPilotStatus.BackColor = Color.FromArgb(192, 192, 192);
            picPilotStatus.Size = new Size(81, 33);
            picPilotStatus.Location = new Point(416, 224);
            picPilotStatus.TabIndex = 8;
            picPilotStatus.Dock = DockStyle.None;
            picPilotStatus.CausesValidation = true;
            picPilotStatus.Enabled = true;
            picPilotStatus.ForeColor = SystemColors.ControlText;
            picPilotStatus.Cursor = Cursors.Default;
            picPilotStatus.RightToLeft = RightToLeft.No;
            picPilotStatus.TabStop = true;
            picPilotStatus.Visible = true;
            picPilotStatus.SizeMode = PictureBoxSizeMode.Normal;
            picPilotStatus.BorderStyle = BorderStyle.None;
            picPilotStatus.Name = "picPilotStatus";
            picUnitStatus.BackColor = Color.FromArgb(192, 192, 192);
            picUnitStatus.Font = new Font("ＭＳ 明朝", 9f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            picUnitStatus.ForeColor = Color.Black;
            picUnitStatus.Size = new Size(81, 33);
            picUnitStatus.Location = new Point(376, 240);
            picUnitStatus.TabIndex = 7;
            picUnitStatus.Dock = DockStyle.None;
            picUnitStatus.CausesValidation = true;
            picUnitStatus.Enabled = true;
            picUnitStatus.Cursor = Cursors.Default;
            picUnitStatus.RightToLeft = RightToLeft.No;
            picUnitStatus.TabStop = true;
            picUnitStatus.Visible = true;
            picUnitStatus.SizeMode = PictureBoxSizeMode.Normal;
            picUnitStatus.BorderStyle = BorderStyle.None;
            picUnitStatus.Name = "picUnitStatus";
            picUnitBitmap.BackColor = Color.White;
            picUnitBitmap.Font = new Font("ＭＳ Ｐゴシック", 9f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            picUnitBitmap.ForeColor = Color.Black;
            picUnitBitmap.Size = new Size(32, 96);
            picUnitBitmap.Location = new Point(88, 192);
            picUnitBitmap.TabIndex = 6;
            picUnitBitmap.Visible = false;
            picUnitBitmap.Dock = DockStyle.None;
            picUnitBitmap.CausesValidation = true;
            picUnitBitmap.Enabled = true;
            picUnitBitmap.Cursor = Cursors.Default;
            picUnitBitmap.RightToLeft = RightToLeft.No;
            picUnitBitmap.TabStop = true;
            picUnitBitmap.SizeMode = PictureBoxSizeMode.Normal;
            picUnitBitmap.BorderStyle = BorderStyle.None;
            picUnitBitmap.Name = "picUnitBitmap";
            _HScroll_Renamed.Enabled = false;
            _HScroll_Renamed.Size = new Size(49, 17);
            _HScroll_Renamed.LargeChange = 4;
            _HScroll_Renamed.Location = new Point(60, 92);
            _HScroll_Renamed.Maximum = 23;
            _HScroll_Renamed.Minimum = 1;
            _HScroll_Renamed.TabIndex = 5;
            _HScroll_Renamed.TabStop = false;
            _HScroll_Renamed.Value = 1;
            _HScroll_Renamed.CausesValidation = true;
            _HScroll_Renamed.Cursor = Cursors.Default;
            _HScroll_Renamed.RightToLeft = RightToLeft.No;
            _HScroll_Renamed.SmallChange = 1;
            _HScroll_Renamed.Visible = true;
            _HScroll_Renamed.Name = "_HScroll_Renamed";
            _VScroll_Renamed.Enabled = false;
            _VScroll_Renamed.Size = new Size(17, 49);
            _VScroll_Renamed.LargeChange = 4;
            _VScroll_Renamed.Location = new Point(116, 80);
            _VScroll_Renamed.Maximum = 23;
            _VScroll_Renamed.Minimum = 1;
            _VScroll_Renamed.TabIndex = 4;
            _VScroll_Renamed.TabStop = false;
            _VScroll_Renamed.Value = 1;
            _VScroll_Renamed.CausesValidation = true;
            _VScroll_Renamed.Cursor = Cursors.Default;
            _VScroll_Renamed.RightToLeft = RightToLeft.No;
            _VScroll_Renamed.SmallChange = 1;
            _VScroll_Renamed.Visible = true;
            _VScroll_Renamed.Name = "_VScroll_Renamed";
            picMask.Font = new Font("ＭＳ Ｐゴシック", 9f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            picMask.Size = new Size(32, 32);
            picMask.Location = new Point(8, 84);
            picMask.Image = (Image)resources.GetObject("picMask.Image");
            picMask.TabIndex = 3;
            picMask.Visible = false;
            picMask.Dock = DockStyle.None;
            picMask.BackColor = SystemColors.Control;
            picMask.CausesValidation = true;
            picMask.Enabled = true;
            picMask.ForeColor = SystemColors.ControlText;
            picMask.Cursor = Cursors.Default;
            picMask.RightToLeft = RightToLeft.No;
            picMask.TabStop = true;
            picMask.SizeMode = PictureBoxSizeMode.Normal;
            picMask.BorderStyle = BorderStyle.None;
            picMask.Name = "picMask";
            picTmp.BackColor = Color.White;
            picTmp.Font = new Font("ＭＳ Ｐゴシック", 9f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            picTmp.ForeColor = Color.Black;
            picTmp.Size = new Size(32, 32);
            picTmp.Location = new Point(240, 176);
            picTmp.TabIndex = 2;
            picTmp.Visible = false;
            picTmp.Dock = DockStyle.None;
            picTmp.CausesValidation = true;
            picTmp.Enabled = true;
            picTmp.Cursor = Cursors.Default;
            picTmp.RightToLeft = RightToLeft.No;
            picTmp.TabStop = true;
            picTmp.SizeMode = PictureBoxSizeMode.AutoSize;
            picTmp.BorderStyle = BorderStyle.None;
            picTmp.Name = "picTmp";
            picBack.BackColor = Color.Black;
            picBack.Font = new Font("ＭＳ Ｐゴシック", 9f, FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            picBack.Size = new Size(81, 32);
            picBack.Location = new Point(384, 32);
            picBack.TabIndex = 1;
            picBack.Visible = false;
            picBack.Dock = DockStyle.None;
            picBack.CausesValidation = true;
            picBack.Enabled = true;
            picBack.ForeColor = SystemColors.ControlText;
            picBack.Cursor = Cursors.Default;
            picBack.RightToLeft = RightToLeft.No;
            picBack.TabStop = true;
            picBack.SizeMode = PictureBoxSizeMode.Normal;
            picBack.BorderStyle = BorderStyle.None;
            picBack.Name = "picBack";
            _picMain_0.BackColor = Color.White;
            _picMain_0.Font = new Font("ＭＳ Ｐ明朝", 15.75f, FontStyle.Bold | FontStyle.Regular, GraphicsUnit.Point, Conversions.ToByte(128));
            _picMain_0.ForeColor = Color.White;
            _picMain_0.Size = new Size(81, 32);
            _picMain_0.Location = new Point(8, 32);
            _picMain_0.TabIndex = 0;
            _picMain_0.Dock = DockStyle.None;
            _picMain_0.CausesValidation = true;
            _picMain_0.Enabled = true;
            _picMain_0.Cursor = Cursors.Default;
            _picMain_0.RightToLeft = RightToLeft.No;
            _picMain_0.TabStop = true;
            _picMain_0.Visible = true;
            _picMain_0.SizeMode = PictureBoxSizeMode.Normal;
            _picMain_0.BorderStyle = BorderStyle.None;
            _picMain_0.Name = "_picMain_0";
            Controls.Add(_picStretchedTmp_1);
            Controls.Add(_picStretchedTmp_0);
            Controls.Add(_picMain_1);
            Controls.Add(_picBuf_0);
            Controls.Add(_picTmp32_2);
            Controls.Add(_picTmp32_1);
            Controls.Add(_picFace);
            Controls.Add(_picTmp32_0);
            Controls.Add(picMaskedBack);
            Controls.Add(picMask2);
            Controls.Add(picNeautral);
            Controls.Add(picEnemy);
            Controls.Add(picUnit);
            Controls.Add(picPilotStatus);
            Controls.Add(picUnitStatus);
            Controls.Add(picUnitBitmap);
            Controls.Add(_HScroll_Renamed);
            Controls.Add(_VScroll_Renamed);
            Controls.Add(picMask);
            Controls.Add(picTmp);
            Controls.Add(picBack);
            Controls.Add(_picMain_0);
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_0, Conversions.ToShort(0));
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_1, Conversions.ToShort(1));
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_2, Conversions.ToShort(2));
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_3, Conversions.ToShort(3));
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_4, Conversions.ToShort(4));
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_5, Conversions.ToShort(5));
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_6, Conversions.ToShort(6));
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_7, Conversions.ToShort(7));
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_8, Conversions.ToShort(8));
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_9, Conversions.ToShort(9));
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_10, Conversions.ToShort(10));
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_11, Conversions.ToShort(11));
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_12, Conversions.ToShort(12));
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_13, Conversions.ToShort(13));
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_14, Conversions.ToShort(14));
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_15, Conversions.ToShort(15));
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_16, Conversions.ToShort(16));
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_17, Conversions.ToShort(17));
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_18, Conversions.ToShort(18));
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_19, Conversions.ToShort(19));
            _mnuMapCommandItem.SetIndex(_mnuMapCommandItem_20, Conversions.ToShort(20));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_0, Conversions.ToShort(0));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_1, Conversions.ToShort(1));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_2, Conversions.ToShort(2));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_3, Conversions.ToShort(3));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_4, Conversions.ToShort(4));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_5, Conversions.ToShort(5));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_6, Conversions.ToShort(6));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_7, Conversions.ToShort(7));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_8, Conversions.ToShort(8));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_9, Conversions.ToShort(9));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_10, Conversions.ToShort(10));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_11, Conversions.ToShort(11));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_12, Conversions.ToShort(12));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_13, Conversions.ToShort(13));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_14, Conversions.ToShort(14));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_15, Conversions.ToShort(15));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_16, Conversions.ToShort(16));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_17, Conversions.ToShort(17));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_18, Conversions.ToShort(18));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_19, Conversions.ToShort(19));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_20, Conversions.ToShort(20));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_21, Conversions.ToShort(21));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_22, Conversions.ToShort(22));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_23, Conversions.ToShort(23));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_24, Conversions.ToShort(24));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_25, Conversions.ToShort(25));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_26, Conversions.ToShort(26));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_27, Conversions.ToShort(27));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_28, Conversions.ToShort(28));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_29, Conversions.ToShort(29));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_30, Conversions.ToShort(30));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_31, Conversions.ToShort(31));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_32, Conversions.ToShort(32));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_33, Conversions.ToShort(33));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_34, Conversions.ToShort(34));
            _mnuUnitCommandItem.SetIndex(_mnuUnitCommandItem_35, Conversions.ToShort(35));
            picBuf.SetIndex(_picBuf_0, Conversions.ToShort(0));
            _picMain.SetIndex(_picMain_1, Conversions.ToShort(1));
            _picMain.SetIndex(_picMain_0, Conversions.ToShort(0));
            picStretchedTmp.SetIndex(_picStretchedTmp_1, Conversions.ToShort(1));
            picStretchedTmp.SetIndex(_picStretchedTmp_0, Conversions.ToShort(0));
            picTmp32.SetIndex(_picTmp32_2, Conversions.ToShort(2));
            picTmp32.SetIndex(_picTmp32_1, Conversions.ToShort(1));
            picTmp32.SetIndex(_picTmp32_0, Conversions.ToShort(0));
            ((System.ComponentModel.ISupportInitialize)picTmp32).EndInit();
            ((System.ComponentModel.ISupportInitialize)picStretchedTmp).EndInit();
            ((System.ComponentModel.ISupportInitialize)_picMain).EndInit();
            ((System.ComponentModel.ISupportInitialize)picBuf).EndInit();
            ((System.ComponentModel.ISupportInitialize)_mnuUnitCommandItem).EndInit();
            ((System.ComponentModel.ISupportInitialize)_mnuMapCommandItem).EndInit();
            MainMenu1.Items.AddRange(new ToolStripItem[] { mnuUnitCommand, mnuMapCommand });
            mnuUnitCommand.DropDownItems.AddRange(new ToolStripItem[] { _mnuUnitCommandItem_0, _mnuUnitCommandItem_1, _mnuUnitCommandItem_2, _mnuUnitCommandItem_3, _mnuUnitCommandItem_4, _mnuUnitCommandItem_5, _mnuUnitCommandItem_6, _mnuUnitCommandItem_7, _mnuUnitCommandItem_8, _mnuUnitCommandItem_9, _mnuUnitCommandItem_10, _mnuUnitCommandItem_11, _mnuUnitCommandItem_12, _mnuUnitCommandItem_13, _mnuUnitCommandItem_14, _mnuUnitCommandItem_15, _mnuUnitCommandItem_16, _mnuUnitCommandItem_17, _mnuUnitCommandItem_18, _mnuUnitCommandItem_19, _mnuUnitCommandItem_20, _mnuUnitCommandItem_21, _mnuUnitCommandItem_22, _mnuUnitCommandItem_23, _mnuUnitCommandItem_24, _mnuUnitCommandItem_25, _mnuUnitCommandItem_26, _mnuUnitCommandItem_27, _mnuUnitCommandItem_28, _mnuUnitCommandItem_29, _mnuUnitCommandItem_30, _mnuUnitCommandItem_31, _mnuUnitCommandItem_32, _mnuUnitCommandItem_33, _mnuUnitCommandItem_34, _mnuUnitCommandItem_35 });
            mnuMapCommand.DropDownItems.AddRange(new ToolStripItem[] { _mnuMapCommandItem_0, _mnuMapCommandItem_1, _mnuMapCommandItem_2, _mnuMapCommandItem_3, _mnuMapCommandItem_4, _mnuMapCommandItem_5, _mnuMapCommandItem_6, _mnuMapCommandItem_7, _mnuMapCommandItem_8, _mnuMapCommandItem_9, _mnuMapCommandItem_10, _mnuMapCommandItem_11, _mnuMapCommandItem_12, _mnuMapCommandItem_13, _mnuMapCommandItem_14, _mnuMapCommandItem_15, _mnuMapCommandItem_16, _mnuMapCommandItem_17, _mnuMapCommandItem_18, _mnuMapCommandItem_19, _mnuMapCommandItem_20 });
            Controls.Add(MainMenu1);
            MainMenu1.ResumeLayout(false);
            KeyDown += new KeyEventHandler(frmSafeMain_KeyDown);
            MouseMove += new MouseEventHandler(frmSafeMain_MouseMove);
            FormClosed += new FormClosedEventHandler(frmSafeMain_FormClosed);
            ResumeLayout(false);
            PerformLayout();
        }
        /* TODO ERROR: Skipped EndRegionDirectiveTrivia */
    }
}