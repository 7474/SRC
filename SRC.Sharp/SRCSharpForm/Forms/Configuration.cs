using System;
using System.Windows.Forms;
using SRCCore;
using SRCCore.Config;
using SRCCore.Lib;
using SRCCore.VB;

namespace SRCSharpForm
{
    internal partial class frmConfiguration : Form
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // マップコマンド「設定変更」用ダイアログ


        // MP3Volumeを記録
        private float SavedMP3Volume;

        public SRCCore.SRC SRC;
        private Sound Sound => SRC.Sound;
        // XXX 型
        private LocalFileConfig SystemConfig => SRC.SystemConfig as LocalFileConfig;

        // 戦闘アニメOn・Off切り替え
        // UPGRADE_WARNING: イベント chkBattleAnimation.CheckStateChanged は、フォームが初期化されたときに発生します。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"' をクリックしてください。
        private void chkBattleAnimation_CheckStateChanged(object eventSender, EventArgs eventArgs)
        {
            //// 戦闘アニメを表示しない場合は拡張戦闘アニメ、武器アニメ選択の項目を選択不能にする
            //if ((int)chkBattleAnimation.CheckState == 1)
            //{
            //    chkExtendedAnimation.Enabled = true;
            //    chkWeaponAnimation.Enabled = true;
            //}
            //else
            //{
            //    chkExtendedAnimation.Enabled = false;
            //    chkWeaponAnimation.Enabled = false;
            //}
        }

        // キャンセルボタンが押された
        private void cmdCancel_Click(object eventSender, EventArgs eventArgs)
        {
            // MP3音量のみその場で変更しているので元に戻す必要がある
            Sound.Player.SoundVolume = SavedMP3Volume;
            Close();
        }

        // OKボタンが押された
        private void cmdOK_Click(object eventSender, EventArgs eventArgs)
        {
            //// 各種設定を変更

            //// メッセージスピード
            //switch (cboMessageSpeed.Text ?? "")
            //{
            //    case "神の領域":
            //        {
            //            GUI.MessageWait = 0;
            //            break;
            //        }

            //    case "超高速":
            //        {
            //            GUI.MessageWait = 200;
            //            break;
            //        }

            //    case "高速":
            //        {
            //            GUI.MessageWait = 400;
            //            break;
            //        }

            //    case "普通":
            //        {
            //            GUI.MessageWait = 700;
            //            break;
            //        }

            //    case "低速":
            //        {
            //            GUI.MessageWait = 1000;
            //            break;
            //        }

            //    case "手動送り":
            //        {
            //            GUI.MessageWait = 10000000;
            //            break;
            //        }
            //}

            //SystemConfig.SetItem("Option", "MessageWait", SrcFormatter.Format(GUI.MessageWait));

            // 戦闘アニメ表示
            if ((int)chkBattleAnimation.CheckState == 1)
            {
                SystemConfig.BattleAnimation = true;
            }
            else
            {
                SystemConfig.BattleAnimation = false;
            }

            // 拡大戦闘アニメ表示
            if ((int)chkExtendedAnimation.CheckState == 1)
            {
                SystemConfig.ExtendedAnimation = true;
            }
            else
            {
                SystemConfig.ExtendedAnimation = false;
            }

            // 武器準備アニメ表示
            if ((int)chkWeaponAnimation.CheckState == 1)
            {
                SystemConfig.WeaponAnimation = true;
            }
            else
            {
                SystemConfig.WeaponAnimation = false;
            }

            // 移動アニメ表示
            if ((int)chkMoveAnimation.CheckState == 1)
            {
                SystemConfig.MoveAnimation = true;
            }
            else
            {
                SystemConfig.MoveAnimation = false;
            }

            // スペシャルパワーアニメ表示
            if ((int)chkSpecialPowerAnimation.CheckState == 1)
            {
                SystemConfig.SpecialPowerAnimation = true;
            }
            else
            {
                SystemConfig.SpecialPowerAnimation = false;
            }

            // マウスカーソルの自動移動
            if (chkAutoMoveCursor.Checked)
            {
                SystemConfig.AutoMoveCursor = true;
            }
            else
            {
                SystemConfig.AutoMoveCursor = false;
            }

            // マス目の表示
            if (chkShowSquareLine.Checked)
            {
                SystemConfig.ShowSquareLine = true;
            }
            else
            {
                SystemConfig.ShowSquareLine = false;
            }

            // 味方フェイズ開始時のターン表示
            if (chkShowTurn.Checked)
            {
                SystemConfig.SetItem("Option", "Turn", "On");
            }
            else
            {
                SystemConfig.SetItem("Option", "Turn", "Off");
            }

            // 敵フェイズ中にＢＧＭを変更しない
            if (chkKeepEnemyBGM.Checked)
            {
                SystemConfig.KeepEnemyBGM = true;
            }
            else
            {
                SystemConfig.KeepEnemyBGM = false;
            }

            //// MIDI音源リセットの種類
            //SystemConfig.MidiResetType = cboMidiReset.Text;
            //SystemConfig.SetItem("Option", "MidiReset", cboMidiReset.Text);
            //cboMidiReset.Text = argini_data21;

            // MP3再生音量
            SystemConfig.SoundVolume = (int)(Sound.Player.SoundVolume * 100);
            SystemConfig.Save();

            // ダイアログを閉じる
            Close();
        }

        private void frmConfiguration_Load(object eventSender, EventArgs eventArgs)
        {
            // ダイアログを初期化

            //// メッセージスピード
            //cboMessageSpeed.Items.Add("手動送り");
            //cboMessageSpeed.Items.Add("低速");
            //cboMessageSpeed.Items.Add("普通");
            //cboMessageSpeed.Items.Add("高速");
            //cboMessageSpeed.Items.Add("超高速");
            //cboMessageSpeed.Items.Add("神の領域");
            //switch (GUI.MessageWait)
            //{
            //    case 0:
            //        {
            //            cboMessageSpeed.Text = "神の領域";
            //            break;
            //        }

            //    case 200:
            //        {
            //            cboMessageSpeed.Text = "超高速";
            //            break;
            //        }

            //    case 400:
            //        {
            //            cboMessageSpeed.Text = "高速";
            //            break;
            //        }

            //    case 700:
            //        {
            //            cboMessageSpeed.Text = "普通";
            //            break;
            //        }

            //    case 1000:
            //        {
            //            cboMessageSpeed.Text = "低速";
            //            break;
            //        }

            //    case 10000000:
            //        {
            //            cboMessageSpeed.Text = "手動送り";
            //            break;
            //        }
            //}

            // 戦闘アニメ表示
            if (SRC.BattleAnimation)
            {
                chkBattleAnimation.CheckState = CheckState.Checked;
            }
            else
            {
                chkBattleAnimation.CheckState = CheckState.Unchecked;
            }

            //bool localFileExists() { string argfname = SRC.AppPath + @"Lib\汎用戦闘アニメ\include.eve"; 
            //    var ret = GeneralLib.FileExists(argfname); 
            //    return ret; }

            //if (!localFileExists())
            //{
            //    chkBattleAnimation.CheckState = CheckState.Indeterminate; // 無効
            //}

            // 拡張戦闘アニメ表示
            if (SRC.ExtendedAnimation)
            {
                chkExtendedAnimation.CheckState = CheckState.Checked;
            }
            else
            {
                chkExtendedAnimation.CheckState = CheckState.Unchecked;
            }

            if ((int)chkBattleAnimation.CheckState == 1)
            {
                chkExtendedAnimation.Enabled = true;
            }
            else
            {
                chkExtendedAnimation.Enabled = false;
            }

            // 武器準備アニメ表示
            if (SRC.WeaponAnimation)
            {
                chkWeaponAnimation.CheckState = CheckState.Checked;
            }
            else
            {
                chkWeaponAnimation.CheckState = CheckState.Unchecked;
            }

            if ((int)chkBattleAnimation.CheckState == 1)
            {
                chkWeaponAnimation.Enabled = true;
            }
            else
            {
                chkWeaponAnimation.Enabled = false;
            }

            // 移動アニメ表示
            if (SRC.MoveAnimation)
            {
                chkMoveAnimation.CheckState = CheckState.Checked;
            }
            else
            {
                chkMoveAnimation.CheckState = CheckState.Unchecked;
            }

            // スペシャルパワーアニメ表示
            if (SRC.SpecialPowerAnimation)
            {
                chkSpecialPowerAnimation.CheckState = CheckState.Checked;
            }
            else
            {
                chkSpecialPowerAnimation.CheckState = CheckState.Unchecked;
            }

            // マウスカーソルの自動移動
            if (SRC.AutoMoveCursor)
            {
                chkAutoMoveCursor.CheckState = CheckState.Checked;
            }
            else
            {
                chkAutoMoveCursor.CheckState = CheckState.Unchecked;
            }

            // マス目の表示
            if (SRC.ShowSquareLine)
            {
                chkShowSquareLine.CheckState = CheckState.Checked;
            }
            else
            {
                chkShowSquareLine.CheckState = CheckState.Unchecked;
            }

            //// 味方フェイズ開始時のターン表示
            //if (Strings.LCase(GeneralLib.ReadIni("Option", "Turn")) == "on")
            //{
            //    chkShowTurn.CheckState = CheckState.Checked;
            //}
            //else
            //{
            //    chkShowTurn.CheckState = CheckState.Unchecked;
            //}

            //// 敵フェイズ中にＢＧＭを変更しない
            //if (SRC.KeepEnemyBGM)
            //{
            //    chkKeepEnemyBGM.CheckState = CheckState.Checked;
            //}
            //else
            //{
            //    chkKeepEnemyBGM.CheckState = CheckState.Unchecked;
            //}

            //// MIDI演奏にDirectMusicを使用する
            //if (Strings.LCase(GeneralLib.ReadIni("Option", "UseDirectMusic")) == "on")
            //{
            //    chkUseDirectMusic.CheckState = CheckState.Checked;
            //}
            //else
            //{
            //    chkUseDirectMusic.CheckState = CheckState.Unchecked;
            //}

            //// MIDI音源リセットの種類
            //cboMidiReset.Items.Add("None");
            //cboMidiReset.Items.Add("GM");
            //cboMidiReset.Items.Add("GS");
            //cboMidiReset.Items.Add("XG");
            //cboMidiReset.Text = SRC.MidiResetType;

            // MP3音量
            SavedMP3Volume = Sound.Player.SoundVolume;
            txtMP3Volume.Text = SrcFormatter.Format((int)(Sound.Player.SoundVolume * 100));
        }

        // MP3音量変更
        private void hscMP3Volume_Change(int newScrollValue)
        {
            Sound.Player.SoundVolume = newScrollValue / 100f;
            txtMP3Volume.Text = SrcFormatter.Format(newScrollValue);
        }

        private void hscMP3Volume_Scroll(int newScrollValue)
        {
            Sound.Player.SoundVolume = newScrollValue / 100f;
            txtMP3Volume.Text = SrcFormatter.Format(newScrollValue);
        }

        private void txtMP3Volume_TextChanged(object eventSender, EventArgs eventArgs)
        {
            var volume = GeneralLib.StrToLng(txtMP3Volume.Text);
            if (volume < 0)
            {
                volume = 0;
                txtMP3Volume.Text = "0";
            }
            else if (volume > 100)
            {
                volume = 100;
                txtMP3Volume.Text = "100";
            }

            hscMP3Volume.Value = volume;
        }

        private void hscMP3Volume_Scroll(object eventSender, ScrollEventArgs eventArgs)
        {
            switch (eventArgs.Type)
            {
                case ScrollEventType.ThumbTrack:
                    {
                        hscMP3Volume_Scroll(eventArgs.NewValue);
                        break;
                    }

                case ScrollEventType.EndScroll:
                    {
                        hscMP3Volume_Change(eventArgs.NewValue);
                        break;
                    }
            }
        }
    }
}
