using System;
using System.Collections.Generic;
using System.Drawing;

namespace SRCRender.Compat
{
    /// <summary>
    /// CheckBox 相当。
    /// </summary>
    public class CheckBox : Control
    {
        public string Text { get; set; } = "";
        public Font Font { get; set; }
        public bool Checked { get; set; }
        public bool Enabled { get; set; } = true;
    }

    /// <summary>
    /// HScrollBar 相当（値の範囲は Minimum から Maximum）。
    /// </summary>
    public class HScrollBar : Control
    {
        public int Minimum { get; set; }
        public int Maximum { get; set; } = 100;
        public int Value { get; set; }

        /// <summary>つまみの幅</summary>
        public const int ThumbWidth = 12;

        /// <summary>HScrollBar 内の x 座標に対応する値。</summary>
        public int ValueFromPoint(float x)
        {
            var ratio = (x - ThumbWidth / 2f) / Math.Max(1, Width - ThumbWidth);
            return Math.Max(Minimum, Math.Min(Maximum, Minimum + (int)Math.Round(ratio * (Maximum - Minimum))));
        }

        /// <summary>つまみの左端（HScrollBar 内の座標）</summary>
        public float ThumbLeft => (Width - ThumbWidth) * (float)(Value - Minimum) / Math.Max(1, Maximum - Minimum);
    }

    /// <summary>
    /// ComboBox（ドロップダウンリスト）相当。▼ を押すと下に選択肢を開く。
    /// </summary>
    public class ComboBox : Control
    {
        public Font Font { get; set; }
        public List<string> Items { get; } = new List<string>();
        /// <summary>選んでいる項目（-1 なら選んでいない）</summary>
        public int SelectedIndex { get; set; } = -1;
        public string Text => SelectedIndex >= 0 && SelectedIndex < Items.Count ? Items[SelectedIndex] : "";
        /// <summary>選択肢を開いているか</summary>
        public bool DroppedDown { get; set; }
        /// <summary>開いた選択肢の 1 行の高さ</summary>
        public const int ItemHeight = 16;

        /// <summary>開いた選択肢の範囲（ComboBox の下）</summary>
        public RectangleF DropDownRect => new RectangleF(Left, Top + Height, Width, ItemHeight * Items.Count + 2);

        /// <summary>開いた選択肢のうち、点（親のクライアント座標）にある項目。無ければ -1</summary>
        public int ItemAt(PointF client)
        {
            var rect = DropDownRect;
            if (!DroppedDown || client.X < rect.Left || client.X >= rect.Right || client.Y < rect.Top + 1 || client.Y >= rect.Bottom - 1)
            {
                return -1;
            }
            return Math.Min(Items.Count - 1, (int)((client.Y - rect.Top - 1) / ItemHeight));
        }
    }

    /// <summary>
    /// WinForms 版の設定変更ダイアログ（Forms/Configuration.Designer.cs）と同じ部品・配置を持つ。
    /// MIDI 音源リセットは SRC# では使わないため省く。
    /// </summary>
    public class ConfigWindow : Form
    {
        private static readonly Font ControlFont = new Font("ＭＳ Ｐゴシック", 9F, FontStyle.Regular, GraphicsUnit.Point);
        private readonly ListBoxControl items = new ListBoxControl { Visible = false };

        public ConfigWindow()
        {
            Width = 346;
            Height = 405;
            Text = Resources.Strings.ConfigTitle;
            // 元の SRC の設定変更の画面と同じく、一番上にメッセージスピード
            labMessageSpeed = new Label { Left = 33, Top = 20, Width = 109, Height = 17, Text = Resources.Strings.ConfigMessageSpeed, Font = ControlFont };
            cboMessageSpeed = new ComboBox { Left = 144, Top = 16, Width = 137, Height = 20, Font = ControlFont, BackColor = Color.White };
            cboMessageSpeed.Items.AddRange(GameConfigLabels.MessageSpeedNames);
            chkBattleAnimation = new CheckBox { Left = 32, Top = 40, Width = 249, Height = 25, Text = Resources.Strings.ConfigBattleAnimation, Font = ControlFont };
            chkExtendedAnimation = new CheckBox { Left = 48, Top = 64, Width = 233, Height = 33, Text = Resources.Strings.ConfigExtendedAnimation, Font = ControlFont };
            chkWeaponAnimation = new CheckBox { Left = 48, Top = 96, Width = 233, Height = 33, Text = Resources.Strings.ConfigWeaponAnimation, Font = ControlFont };
            chkSpecialPowerAnimation = new CheckBox { Left = 32, Top = 128, Width = 249, Height = 25, Text = Resources.Strings.ConfigSpecialPowerAnimation, Font = ControlFont };
            chkMoveAnimation = new CheckBox { Left = 32, Top = 152, Width = 249, Height = 25, Text = Resources.Strings.ConfigMoveAnimation, Font = ControlFont };
            chkAutoMoveCursor = new CheckBox { Left = 32, Top = 176, Width = 249, Height = 25, Text = Resources.Strings.ConfigAutoMoveCursor, Font = ControlFont };
            chkShowSquareLine = new CheckBox { Left = 32, Top = 200, Width = 265, Height = 25, Text = Resources.Strings.ConfigShowSquareLine, Font = ControlFont };
            chkShowTurn = new CheckBox { Left = 32, Top = 224, Width = 249, Height = 25, Text = Resources.Strings.ConfigShowTurn, Font = ControlFont };
            chkKeepEnemyBGM = new CheckBox { Left = 32, Top = 248, Width = 249, Height = 25, Text = Resources.Strings.ConfigKeepEnemyBGM, Font = ControlFont };
            labMP3Volume = new Label { Left = 33, Top = 330, Width = 49, Height = 17, Text = Resources.Strings.ConfigMP3Volume, Font = ControlFont };
            txtMP3Volume = new Label { Left = 87, Top = 327, Width = 33, Height = 19, Text = "100", Font = ControlFont, BackColor = Color.White };
            hscMP3Volume = new HScrollBar { Left = 128, Top = 328, Width = 153, Height = 17 };
            cmdOK = new Button { Left = 112, Top = 360, Width = 97, Height = 25, Text = "OK", Font = ControlFont };
            cmdCancel = new Button { Left = 216, Top = 360, Width = 97, Height = 25, Text = Resources.Strings.Cancel, Font = ControlFont };
        }

        public override ListBoxControl lstItems => items;

        public Label labMessageSpeed { get; }
        public ComboBox cboMessageSpeed { get; }
        public CheckBox chkBattleAnimation { get; }
        public CheckBox chkExtendedAnimation { get; }
        public CheckBox chkWeaponAnimation { get; }
        public CheckBox chkSpecialPowerAnimation { get; }
        public CheckBox chkMoveAnimation { get; }
        public CheckBox chkAutoMoveCursor { get; }
        public CheckBox chkShowSquareLine { get; }
        public CheckBox chkShowTurn { get; }
        public CheckBox chkKeepEnemyBGM { get; }
        public Label labMP3Volume { get; }
        public Label txtMP3Volume { get; }
        public HScrollBar hscMP3Volume { get; }
        public Button cmdOK { get; }
        public Button cmdCancel { get; }

        public IEnumerable<CheckBox> CheckBoxes => new[]
        {
            chkBattleAnimation, chkExtendedAnimation, chkWeaponAnimation, chkSpecialPowerAnimation, chkMoveAnimation,
            chkAutoMoveCursor, chkShowSquareLine, chkShowTurn, chkKeepEnemyBGM,
        };

        public IEnumerable<Button> Buttons => new[] { cmdOK, cmdCancel };
    }
}
