// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore;
using SRCCore.Lib;
using SRCCore.VB;
using SRCRender.Compat;
using System.Linq;

namespace SRCRender
{
    // WinForms 版の設定変更ダイアログ（Forms/Configuration.cs）の移植。
    public partial class ScreenGUI
    {
        private readonly ConfigWindow frmConfiguration = new ConfigWindow();
        // MP3Volumeを記録
        private float savedMP3Volume;
        private bool configClosed;

        public void Configure()
        {
            var w = frmConfiguration;
            w.Refreshed = Present;
            var config = SRC.SystemConfig;

            // ダイアログを開く
            w.chkBattleAnimation.Checked = SRC.BattleAnimation;
            w.chkExtendedAnimation.Checked = SRC.ExtendedAnimation;
            w.chkWeaponAnimation.Checked = SRC.WeaponAnimation;
            w.chkMoveAnimation.Checked = SRC.MoveAnimation;
            w.chkSpecialPowerAnimation.Checked = SRC.SpecialPowerAnimation;
            w.chkAutoMoveCursor.Checked = SRC.AutoMoveCursor;
            w.chkShowSquareLine.Checked = SRC.ShowSquareLine;
            w.chkShowTurn.Checked = (config.GetItem("Option", "Turn") ?? "").ToLower() == "on";
            w.chkKeepEnemyBGM.Checked = config.KeepEnemyBGM;
            UpdateConfigEnabled(w);

            savedMP3Volume = Sound.Player.SoundVolume;
            SetConfigVolume(w, (int)(Sound.Player.SoundVolume * 100));

            w.PlaceOnScreen(ScreenLayout.Width, ScreenLayout.Height, false, false);
            configClosed = false;
            State.Config = w;
            w.Show();
            try
            {
                while (!configClosed)
                {
                    Sleep(WaitIntervalMillis);
                }
            }
            finally
            {
                State.Config = null;
                w.Hide();
            }
        }

        // 戦闘アニメを表示しない場合は拡張戦闘アニメ、武器アニメ選択の項目を選択不能にする
        private static void UpdateConfigEnabled(ConfigWindow w)
        {
            w.chkExtendedAnimation.Enabled = w.chkBattleAnimation.Checked;
            w.chkWeaponAnimation.Enabled = w.chkBattleAnimation.Checked;
        }

        private void SetConfigVolume(ConfigWindow w, int volume)
        {
            volume = System.Math.Max(0, System.Math.Min(100, volume));
            w.hscMP3Volume.Value = volume;
            w.txtMP3Volume.Text = SrcFormatter.Format(volume);
            // MP3音量はその場で変更する
            Sound.Player.SoundVolume = volume / 100f;
        }

        // OKボタンが押された
        private void ConfigOK(ConfigWindow w)
        {
            var config = SRC.SystemConfig;
            config.BattleAnimation = w.chkBattleAnimation.Checked;
            config.ExtendedAnimation = w.chkExtendedAnimation.Checked;
            config.WeaponAnimation = w.chkWeaponAnimation.Checked;
            config.MoveAnimation = w.chkMoveAnimation.Checked;
            config.SpecialPowerAnimation = w.chkSpecialPowerAnimation.Checked;
            config.AutoMoveCursor = w.chkAutoMoveCursor.Checked;
            config.ShowSquareLine = w.chkShowSquareLine.Checked;
            config.SetItem("Option", "Turn", w.chkShowTurn.Checked ? "On" : "Off");
            config.KeepEnemyBGM = w.chkKeepEnemyBGM.Checked;
            if (config is SRCCore.Config.LocalFileConfig localConfig)
            {
                localConfig.SoundVolume = (int)(Sound.Player.SoundVolume * 100);
            }
            try
            {
                config.Save();
            }
            catch (System.Exception ex)
            {
                SRC.LogError(ex);
            }
            configClosed = true;
        }

        // キャンセルボタンが押された
        private void ConfigCancel()
        {
            // MP3音量のみその場で変更しているので元に戻す必要がある
            Sound.Player.SoundVolume = savedMP3Volume;
            configClosed = true;
        }

        private void ConfigPointerPressed(float x, float y, GuiButton button)
        {
            var w = State.Config;
            if (w == null)
            {
                return;
            }
            if (button != GuiButton.Left)
            {
                return;
            }
            var client = w.ToClient(x, y);
            var check = w.CheckBoxes.FirstOrDefault(c => Form.HitTest(c, client));
            if (check != null)
            {
                if (check.Enabled)
                {
                    check.Checked = !check.Checked;
                    UpdateConfigEnabled(w);
                    Present();
                }
                return;
            }
            if (Form.HitTest(w.hscMP3Volume, client))
            {
                SetConfigVolume(w, w.hscMP3Volume.ValueFromPoint(client.X - w.hscMP3Volume.Left));
                Present();
                return;
            }
            if (Form.HitTest(w.cmdOK, client))
            {
                ConfigOK(w);
            }
            else if (Form.HitTest(w.cmdCancel, client))
            {
                ConfigCancel();
            }
        }

        private void ConfigKeyPressed(ScreenKey key)
        {
            var w = State.Config;
            switch (key)
            {
                case ScreenKey.Enter:
                    if (w != null)
                    {
                        ConfigOK(w);
                    }
                    break;
                case ScreenKey.Escape:
                    ConfigCancel();
                    break;
            }
        }
    }
}
