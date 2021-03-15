using System;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    internal partial class frmConfiguration : Form
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // マップコマンド「設定変更」用ダイアログ


        // MP3Volumeを記録
        private short SavedMP3Volume;

        // 戦闘アニメOn・Off切り替え
        // UPGRADE_WARNING: イベント chkBattleAnimation.CheckStateChanged は、フォームが初期化されたときに発生します。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"' をクリックしてください。
        private void chkBattleAnimation_CheckStateChanged(object eventSender, EventArgs eventArgs)
        {
            // 戦闘アニメを表示しない場合は拡張戦闘アニメ、武器アニメ選択の項目を選択不能にする
            if ((int)chkBattleAnimation.CheckState == 1)
            {
                chkExtendedAnimation.Enabled = true;
                chkWeaponAnimation.Enabled = true;
            }
            else
            {
                chkExtendedAnimation.Enabled = false;
                chkWeaponAnimation.Enabled = false;
            }
        }

        // キャンセルボタンが押された
        private void cmdCancel_Click(object eventSender, EventArgs eventArgs)
        {
            var IsMP3Supported = default(object);
            // ダイアログを閉じる
            Hide();

            // MP3音量のみその場で変更しているので元に戻す必要がある
            Sound.MP3Volume = SavedMP3Volume;
            // UPGRADE_WARNING: オブジェクト IsMP3Supported の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            if (Conversions.ToBoolean(IsMP3Supported))
            {
                VBMP3.vbmp3_setVolume(Sound.MP3Volume, Sound.MP3Volume);
            }
        }

        // OKボタンが押された
        private void cmdOK_Click(object eventSender, EventArgs eventArgs)
        {
            // 各種設定を変更

            // メッセージスピード
            switch (cboMessageSpeed.Text ?? "")
            {
                case "神の領域":
                    {
                        GUI.MessageWait = 0;
                        break;
                    }

                case "超高速":
                    {
                        GUI.MessageWait = 200;
                        break;
                    }

                case "高速":
                    {
                        GUI.MessageWait = 400;
                        break;
                    }

                case "普通":
                    {
                        GUI.MessageWait = 700;
                        break;
                    }

                case "低速":
                    {
                        GUI.MessageWait = 1000;
                        break;
                    }

                case "手動送り":
                    {
                        GUI.MessageWait = 10000000;
                        break;
                    }
            }

            string argini_section = "Option";
            string argini_entry = "MessageWait";
            string argini_data = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(GUI.MessageWait);
            GeneralLib.WriteIni(ref argini_section, ref argini_entry, ref argini_data);

            // 戦闘アニメ表示
            if ((int)chkBattleAnimation.CheckState == 1)
            {
                SRC.BattleAnimation = true;
                string argini_section1 = "Option";
                string argini_entry1 = "BattleAnimation";
                string argini_data1 = "On";
                GeneralLib.WriteIni(ref argini_section1, ref argini_entry1, ref argini_data1);
            }
            else
            {
                SRC.BattleAnimation = false;
                string argini_section2 = "Option";
                string argini_entry2 = "BattleAnimation";
                string argini_data2 = "Off";
                GeneralLib.WriteIni(ref argini_section2, ref argini_entry2, ref argini_data2);
            }

            // 拡大戦闘アニメ表示
            if ((int)chkExtendedAnimation.CheckState == 1)
            {
                SRC.ExtendedAnimation = true;
                string argini_section3 = "Option";
                string argini_entry3 = "ExtendedAnimation";
                string argini_data3 = "On";
                GeneralLib.WriteIni(ref argini_section3, ref argini_entry3, ref argini_data3);
            }
            else
            {
                SRC.ExtendedAnimation = false;
                string argini_section4 = "Option";
                string argini_entry4 = "Extendednimation";
                string argini_data4 = "Off";
                GeneralLib.WriteIni(ref argini_section4, ref argini_entry4, ref argini_data4);
            }

            // 武器準備アニメ表示
            if ((int)chkWeaponAnimation.CheckState == 1)
            {
                SRC.WeaponAnimation = true;
                string argini_section5 = "Option";
                string argini_entry5 = "WeaponAnimation";
                string argini_data5 = "On";
                GeneralLib.WriteIni(ref argini_section5, ref argini_entry5, ref argini_data5);
            }
            else
            {
                SRC.WeaponAnimation = false;
                string argini_section6 = "Option";
                string argini_entry6 = "WeaponAnimation";
                string argini_data6 = "Off";
                GeneralLib.WriteIni(ref argini_section6, ref argini_entry6, ref argini_data6);
            }

            // 移動アニメ表示
            if ((int)chkMoveAnimation.CheckState == 1)
            {
                SRC.MoveAnimation = true;
                string argini_section7 = "Option";
                string argini_entry7 = "MoveAnimation";
                string argini_data7 = "On";
                GeneralLib.WriteIni(ref argini_section7, ref argini_entry7, ref argini_data7);
            }
            else
            {
                SRC.MoveAnimation = false;
                string argini_section8 = "Option";
                string argini_entry8 = "MoveAnimation";
                string argini_data8 = "Off";
                GeneralLib.WriteIni(ref argini_section8, ref argini_entry8, ref argini_data8);
            }

            // スペシャルパワーアニメ表示
            if ((int)chkSpecialPowerAnimation.CheckState == 1)
            {
                SRC.SpecialPowerAnimation = true;
                string argini_section9 = "Option";
                string argini_entry9 = "SpecialPowerAnimation";
                string argini_data9 = "On";
                GeneralLib.WriteIni(ref argini_section9, ref argini_entry9, ref argini_data9);
            }
            else
            {
                SRC.SpecialPowerAnimation = false;
                string argini_section10 = "Option";
                string argini_entry10 = "SpecialPowerAnimation";
                string argini_data10 = "Off";
                GeneralLib.WriteIni(ref argini_section10, ref argini_entry10, ref argini_data10);
            }

            // マウスカーソルの自動移動
            if (Conversions.ToBoolean(chkAutoMoveCursor.CheckState))
            {
                SRC.AutoMoveCursor = true;
                string argini_section11 = "Option";
                string argini_entry11 = "AutoMoveCursor";
                string argini_data11 = "On";
                GeneralLib.WriteIni(ref argini_section11, ref argini_entry11, ref argini_data11);
            }
            else
            {
                SRC.AutoMoveCursor = false;
                string argini_section12 = "Option";
                string argini_entry12 = "AutoMoveCursor";
                string argini_data12 = "Off";
                GeneralLib.WriteIni(ref argini_section12, ref argini_entry12, ref argini_data12);
            }

            // マス目の表示
            if (Conversions.ToBoolean(chkShowSquareLine.CheckState))
            {
                SRC.ShowSquareLine = true;
                string argini_section13 = "Option";
                string argini_entry13 = "Square";
                string argini_data13 = "On";
                GeneralLib.WriteIni(ref argini_section13, ref argini_entry13, ref argini_data13);
            }
            else
            {
                SRC.ShowSquareLine = false;
                string argini_section14 = "Option";
                string argini_entry14 = "Square";
                string argini_data14 = "Off";
                GeneralLib.WriteIni(ref argini_section14, ref argini_entry14, ref argini_data14);
            }

            // 味方フェイズ開始時のターン表示
            if (Conversions.ToBoolean(chkShowTurn.CheckState))
            {
                string argini_section15 = "Option";
                string argini_entry15 = "Turn";
                string argini_data15 = "On";
                GeneralLib.WriteIni(ref argini_section15, ref argini_entry15, ref argini_data15);
            }
            else
            {
                string argini_section16 = "Option";
                string argini_entry16 = "Turn";
                string argini_data16 = "Off";
                GeneralLib.WriteIni(ref argini_section16, ref argini_entry16, ref argini_data16);
            }

            // 敵フェイズ中にＢＧＭを変更しない
            if (Conversions.ToBoolean(chkKeepEnemyBGM.CheckState))
            {
                SRC.KeepEnemyBGM = true;
                string argini_section17 = "Option";
                string argini_entry17 = "KeepEnemyBGM";
                string argini_data17 = "On";
                GeneralLib.WriteIni(ref argini_section17, ref argini_entry17, ref argini_data17);
            }
            else
            {
                SRC.KeepEnemyBGM = false;
                string argini_section18 = "Option";
                string argini_entry18 = "KeepEnemyBGM";
                string argini_data18 = "Off";
                GeneralLib.WriteIni(ref argini_section18, ref argini_entry18, ref argini_data18);
            }

            // MIDI演奏にDirectMusicを使用する
            if (Conversions.ToBoolean(chkUseDirectMusic.CheckState))
            {
                string argini_section19 = "Option";
                string argini_entry19 = "UseDirectMusic";
                string argini_data19 = "On";
                GeneralLib.WriteIni(ref argini_section19, ref argini_entry19, ref argini_data19);
            }
            else
            {
                string argini_section20 = "Option";
                string argini_entry20 = "UseDirectMusic";
                string argini_data20 = "Off";
                GeneralLib.WriteIni(ref argini_section20, ref argini_entry20, ref argini_data20);
            }

            // MIDI音源リセットの種類
            SRC.MidiResetType = cboMidiReset.Text;
            string argini_section21 = "Option";
            string argini_entry21 = "MidiReset";
            string argini_data21 = cboMidiReset.Text;
            GeneralLib.WriteIni(ref argini_section21, ref argini_entry21, ref argini_data21);
            cboMidiReset.Text = argini_data21;

            // MP3再生音量
            string argini_section22 = "Option";
            string argini_entry22 = "MP3Volume";
            string argini_data22 = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Sound.MP3Volume);
            GeneralLib.WriteIni(ref argini_section22, ref argini_entry22, ref argini_data22);

            // ダイアログを閉じる
            Hide();
        }

        private void frmConfiguration_Load(object eventSender, EventArgs eventArgs)
        {
            // ダイアログを初期化

            // メッセージスピード
            cboMessageSpeed.Items.Add("手動送り");
            cboMessageSpeed.Items.Add("低速");
            cboMessageSpeed.Items.Add("普通");
            cboMessageSpeed.Items.Add("高速");
            cboMessageSpeed.Items.Add("超高速");
            cboMessageSpeed.Items.Add("神の領域");
            switch (GUI.MessageWait)
            {
                case 0:
                    {
                        cboMessageSpeed.Text = "神の領域";
                        break;
                    }

                case 200:
                    {
                        cboMessageSpeed.Text = "超高速";
                        break;
                    }

                case 400:
                    {
                        cboMessageSpeed.Text = "高速";
                        break;
                    }

                case 700:
                    {
                        cboMessageSpeed.Text = "普通";
                        break;
                    }

                case 1000:
                    {
                        cboMessageSpeed.Text = "低速";
                        break;
                    }

                case 10000000:
                    {
                        cboMessageSpeed.Text = "手動送り";
                        break;
                    }
            }

            // 戦闘アニメ表示
            if (SRC.BattleAnimation)
            {
                chkBattleAnimation.CheckState = CheckState.Checked;
            }
            else
            {
                chkBattleAnimation.CheckState = CheckState.Unchecked;
            }

            bool localFileExists() { string argfname = SRC.AppPath + @"Lib\汎用戦闘アニメ\include.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

            if (!localFileExists())
            {
                chkBattleAnimation.CheckState = CheckState.Indeterminate; // 無効
            }

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

            // 味方フェイズ開始時のターン表示
            string argini_section = "Option";
            string argini_entry = "Turn";
            if (Strings.LCase(GeneralLib.ReadIni(ref argini_section, ref argini_entry)) == "on")
            {
                chkShowTurn.CheckState = CheckState.Checked;
            }
            else
            {
                chkShowTurn.CheckState = CheckState.Unchecked;
            }

            // 敵フェイズ中にＢＧＭを変更しない
            if (SRC.KeepEnemyBGM)
            {
                chkKeepEnemyBGM.CheckState = CheckState.Checked;
            }
            else
            {
                chkKeepEnemyBGM.CheckState = CheckState.Unchecked;
            }

            // MIDI演奏にDirectMusicを使用する
            string argini_section1 = "Option";
            string argini_entry1 = "UseDirectMusic";
            if (Strings.LCase(GeneralLib.ReadIni(ref argini_section1, ref argini_entry1)) == "on")
            {
                chkUseDirectMusic.CheckState = CheckState.Checked;
            }
            else
            {
                chkUseDirectMusic.CheckState = CheckState.Unchecked;
            }

            // MIDI音源リセットの種類
            cboMidiReset.Items.Add("None");
            cboMidiReset.Items.Add("GM");
            cboMidiReset.Items.Add("GS");
            cboMidiReset.Items.Add("XG");
            cboMidiReset.Text = SRC.MidiResetType;

            // MP3音量
            SavedMP3Volume = Sound.MP3Volume;
            txtMP3Volume.Text = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Sound.MP3Volume);
        }

        // MP3音量変更
        // UPGRADE_NOTE: hscMP3Volume.Change はイベントからプロシージャに変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="4E2DC008-5EDA-4547-8317-C9316952674F"' をクリックしてください。
        // UPGRADE_WARNING: HScrollBar イベント hscMP3Volume.Change には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
        private void hscMP3Volume_Change(int newScrollValue)
        {
            var IsMP3Supported = default(object);
            Sound.MP3Volume = (short)newScrollValue;
            txtMP3Volume.Text = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Sound.MP3Volume);
            // UPGRADE_WARNING: オブジェクト IsMP3Supported の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            if (Conversions.ToBoolean(IsMP3Supported))
            {
                VBMP3.vbmp3_setVolume(Sound.MP3Volume, Sound.MP3Volume);
            }
        }

        // UPGRADE_NOTE: hscMP3Volume.Scroll はイベントからプロシージャに変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="4E2DC008-5EDA-4547-8317-C9316952674F"' をクリックしてください。
        private void hscMP3Volume_Scroll_Renamed(int newScrollValue)
        {
            var IsMP3Supported = default(object);
            Sound.MP3Volume = (short)newScrollValue;
            txtMP3Volume.Text = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Sound.MP3Volume);
            // UPGRADE_WARNING: オブジェクト IsMP3Supported の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            if (Conversions.ToBoolean(IsMP3Supported))
            {
                VBMP3.vbmp3_setVolume(Sound.MP3Volume, Sound.MP3Volume);
            }
        }

        // UPGRADE_WARNING: イベント txtMP3Volume.TextChanged は、フォームが初期化されたときに発生します。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"' をクリックしてください。
        private void txtMP3Volume_TextChanged(object eventSender, EventArgs eventArgs)
        {
            var IsMP3Supported = default(object);
            string argexpr = txtMP3Volume.Text;
            Sound.MP3Volume = (short)GeneralLib.StrToLng(ref argexpr);
            txtMP3Volume.Text = argexpr;
            if (Sound.MP3Volume < 0)
            {
                Sound.MP3Volume = 0;
                txtMP3Volume.Text = "0";
            }
            else if (Sound.MP3Volume > 100)
            {
                Sound.MP3Volume = 100;
                txtMP3Volume.Text = "100";
            }

            hscMP3Volume.Value = Sound.MP3Volume;

            // UPGRADE_WARNING: オブジェクト IsMP3Supported の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            if (Conversions.ToBoolean(IsMP3Supported))
            {
                VBMP3.vbmp3_setVolume(Sound.MP3Volume, Sound.MP3Volume);
            }
        }

        private void hscMP3Volume_Scroll(object eventSender, ScrollEventArgs eventArgs)
        {
            switch (eventArgs.Type)
            {
                case ScrollEventType.ThumbTrack:
                    {
                        hscMP3Volume_Scroll_Renamed(eventArgs.NewValue);
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