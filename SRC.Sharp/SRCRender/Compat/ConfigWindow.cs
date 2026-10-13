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
    /// WinForms 版の設定変更ダイアログ（Forms/Configuration.Designer.cs）と同じ部品・配置を持つ。
    /// メッセージスピードと MIDI 音源リセットは WinForms 版でも反映していないため省く。
    /// </summary>
    public class ConfigWindow : Form
    {
        private static readonly Font ControlFont = new Font("ＭＳ Ｐゴシック", 9F, FontStyle.Regular, GraphicsUnit.Point);
        private readonly ListBoxControl items = new ListBoxControl { Visible = false };

        public ConfigWindow()
        {
            Width = 346;
            Height = 405;
            Text = "設定変更";
            chkBattleAnimation = new CheckBox { Left = 32, Top = 40, Width = 249, Height = 25, Text = "戦闘アニメを表示する", Font = ControlFont };
            chkExtendedAnimation = new CheckBox { Left = 48, Top = 64, Width = 233, Height = 33, Text = "戦闘アニメの拡張機能を使用する", Font = ControlFont };
            chkWeaponAnimation = new CheckBox { Left = 48, Top = 96, Width = 233, Height = 33, Text = "武器準備アニメを自動選択表示する", Font = ControlFont };
            chkSpecialPowerAnimation = new CheckBox { Left = 32, Top = 128, Width = 249, Height = 25, Text = "スペシャルパワーアニメを表示する", Font = ControlFont };
            chkMoveAnimation = new CheckBox { Left = 32, Top = 152, Width = 249, Height = 25, Text = "移動アニメを表示する", Font = ControlFont };
            chkAutoMoveCursor = new CheckBox { Left = 32, Top = 176, Width = 249, Height = 25, Text = "マウスカーソルを自動的に移動する", Font = ControlFont };
            chkShowSquareLine = new CheckBox { Left = 32, Top = 200, Width = 265, Height = 25, Text = "マス目を表示する (要再起動)", Font = ControlFont };
            chkShowTurn = new CheckBox { Left = 32, Top = 224, Width = 249, Height = 25, Text = "味方フェイズ開始時にターン表示を行う", Font = ControlFont };
            chkKeepEnemyBGM = new CheckBox { Left = 32, Top = 248, Width = 249, Height = 25, Text = "敵フェイズ中にＢＧＭを変更しない", Font = ControlFont };
            labMP3Volume = new Label { Left = 33, Top = 330, Width = 49, Height = 17, Text = "MP3音量", Font = ControlFont };
            txtMP3Volume = new Label { Left = 87, Top = 327, Width = 33, Height = 19, Text = "100", Font = ControlFont, BackColor = Color.White };
            hscMP3Volume = new HScrollBar { Left = 128, Top = 328, Width = 153, Height = 17 };
            cmdOK = new Button { Left = 112, Top = 360, Width = 97, Height = 25, Text = "OK", Font = ControlFont };
            cmdCancel = new Button { Left = 216, Top = 360, Width = 97, Height = 25, Text = "キャンセル", Font = ControlFont };
        }

        public override ListBoxControl lstItems => items;

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
