// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using SRCCore.Filesystem;

namespace SRCCore
{
    // ＢＧＭ＆効果音再生用のモジュール
    public class Sound
    {
        private SRC SRC { get; }
        private IFileSystem FileSystem => SRC.FileSystem;

        public Sound(SRC src)
        {
            SRC = src;
        }

        // MCI制御用API
        [DllImport("winmm.dll", EntryPoint = "mciSendStringA")]
        extern int mciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);

        // WAVE再生用API
        [DllImport("winmm.dll", EntryPoint = "sndPlaySoundA")]
        extern int sndPlaySound(string lpszSoundName, int uFlags);

        public const int SND_SYNC = 0x0; // 再生終了後、制御を戻す
        public const int SND_ASYNC = 0x1; // 関数実行後、制御を戻す
        public const int SND_NODEFAULT = 0x2; // 指定したWAVEファイルが見つからなかった場合、
                                              // デフォルトのWAVEファイルを再生しない
        public const int SND_MEMORY = 0x4; // メモリファイルのWAVEを実行する
        public const int SND_LOOP = 0x8; // 停止を命令するまで再生を繰り返す。
        public const int SND_NOSTOP = 0x10; // 現在Waveファイルが再生中の場合、再生を中止する


        // 現在再生されているＢＧＭのファイル名
        public string BGMFileName;
        // ＢＧＭをリピート再生する？
        public bool RepeatMode;
        // 戦闘時にもＢＧＭを変更しない？
        public bool KeepBGM;
        // ボス用ＢＧＭを演奏中
        public bool BossBGM;

        // Waveファイルの再生を行った？
        public bool IsWavePlayed;

        // MIDIファイルのサーチパスの初期化が完了している？
        private bool IsMidiSearchPathInitialized;

        // MIDI再生方法の手段
        public bool UseMCI;
        public bool UseDirectMusic;

        // WAV再生方法の手段
        public bool UseDirectSound;

        // MP3再生時の音量
        public int MP3Volume;

        // DirectMusic用変数
        // UPGRADE_ISSUE: DirectX7 オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
        private DirectX7 DXObject;
        // UPGRADE_ISSUE: DirectMusicLoader オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
        private DirectMusicLoader DMLoader;
        // UPGRADE_ISSUE: DirectMusicPerformance オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
        private DirectMusicPerformance DMPerformance;
        // UPGRADE_ISSUE: DirectMusicSegment オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
        private DirectMusicSegment DMSegment;

        // VBMP3.dllの初期化が完了している？
        private bool IsMP3Supported;

        // DirectSound用変数
        // UPGRADE_ISSUE: DirectSound オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
        private DirectSound DSObject;
        // UPGRADE_ISSUE: DirectSoundBuffer オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
        private DirectSoundBuffer DSBuffer;

        // ＢＧＭの再生を開始する
        public void StartBGM(string bgm_name, bool is_repeat_mode = true)
        {
            string fname0, fname, fname2;
            int i;

            // ＢＧＭを固定中？
            if (KeepBGM)
            {
                return;
            }

            // ダミーのファイル名？
            if (Strings.Len(bgm_name) < 5)
            {
                return;
            }

            // ファイル名の本体部分を抜き出す
            fname0 = Strings.Left(bgm_name, Strings.Len(bgm_name) - 4);
            string argstr21 = @"\";
            if (GeneralLib.InStr2(fname0, argstr21) > 0)
            {
                string argstr2 = @"\";
                fname0 = Strings.Mid(fname0, GeneralLib.InStr2(fname0, argstr2) + 1);
            }

            // 同じＢＧＭを演奏中であれば演奏を継続
            if (Strings.Len(BGMFileName) > 0)
            {
                if (Strings.InStr(BGMFileName, @"\" + fname0 + ".") > 0)
                {
                    if (BGMStatus() == "playing")
                    {
                        return;
                    }
                }
            }

            // ファイルを検索
            bgm_name = "(" + bgm_name + ")";
            fname = SearchMidiFile(bgm_name);

            // ファイルが見つかった？
            if (Strings.Len(fname) == 0)
            {
                return;
            }

            // 演奏をストップ
            StopBGM();

            // 同じＢＧＭにバリエーションがあればランダムで選択
            i = 1;
            if (Strings.InStr(fname, SRC.ScenarioPath) > 0)
            {
                // シナリオ側にファイルが見つかった場合はバリエーションもシナリオ側からのみ選択
                do
                {
                    i = (i + 1);
                    string argmidi_name = "(" + fname0 + "(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(i) + ")" + Strings.Right(fname, 4) + ")";
                    fname2 = SearchMidiFile(argmidi_name);
                }
                while (Strings.InStr(fname2, SRC.ScenarioPath) > 0);
            }
            else
            {
                // そうでなければ両方から選択
                do
                {
                    i = (i + 1);
                    string argmidi_name1 = "(" + fname0 + "(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(i) + ")" + Strings.Right(fname, 4) + ")";
                    fname2 = SearchMidiFile(argmidi_name1);
                }
                while (!string.IsNullOrEmpty(fname2));
            }

            i = Conversion.Int((i - 1) * VBMath.Rnd() + 1f);
            if (i > 1)
            {
                string argmidi_name2 = "(" + fname0 + "(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(i) + ")" + Strings.Right(fname, 4) + ")";
                fname = SearchMidiFile(argmidi_name2);
            }

            // ＢＧＭを連続演奏？
            RepeatMode = is_repeat_mode;

            // ファイルをロードし、演奏開始
            LoadBGM(fname);

            // リピート再生処理を行うためのタイマーを起動
            // UPGRADE_ISSUE: Control Timer1 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            GUI.MainForm.Timer1.Enabled = true;
        }

        // ＢＧＭのファイルを読み込む
        private void LoadBGM(string fname)
        {
            int ret;
            string cmd;
            var mp3_data = default(VBMP3.InputInfo);

            // ファイルの種類に応じた処理を行う
            switch (Strings.LCase(Strings.Right(fname, 4)) ?? "")
            {
                case ".mid":
                    {
                        // MIDIファイル

                        // MIDIを演奏するのが初めて？
                        if (!UseDirectMusic & !UseMCI)
                        {
                            // DirectMusicの初期化を試みる
                            InitDirectMusic();
                        }

                        // 音源リセット
                        ResetBGM();

                        // DirectMusicを使う？
                        // ファイルをロード
                        if (UseDirectMusic)
                        {
                            ;

                            // UPGRADE_WARNING: オブジェクト DMLoader.LoadSegment の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            DMSegment = DMLoader.LoadSegment(fname);
                            if (Information.Err().Number != 0)
                            {
                                string argmsg = "LoadSegment failed (" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Information.Err().Number) + ")";
                                GUI.ErrorMessage(argmsg);
                            }

                            // UPGRADE_WARNING: オブジェクト DMSegment.SetStandardMidiFile の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            DMSegment.SetStandardMidiFile();
                            if (Information.Err().Number != 0)
                            {
                                string argmsg1 = "SetStandardMidiFile failed (" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Information.Err().Number) + ")";
                                GUI.ErrorMessage(argmsg1);
                            }
                            // UPGRADE_WARNING: オブジェクト DMPerformance.SetMasterAutoDownload の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            DMPerformance.SetMasterAutoDownload(true);
                            if (Information.Err().Number != 0)
                            {
                                string argmsg2 = "SetMasterAutoDownload failed (" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Information.Err().Number) + ")";
                                GUI.ErrorMessage(argmsg2);
                            }

                            // ループ演奏の設定
                            // 繰り返し範囲を設定
                            // UPGRADE_WARNING: オブジェクト DMSegment.SetLoopPoints の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            DMSegment.SetLoopPoints(0, 0);
                            if (Information.Err().Number != 0)
                            {
                                string argmsg3 = "SetLoopPoints failed (" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Information.Err().Number) + ")";
                                GUI.ErrorMessage(argmsg3);
                            }
                            // 繰り返し回数を設定
                            if (RepeatMode)
                            {
                                // UPGRADE_WARNING: オブジェクト DMSegment.SetRepeats の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                DMSegment.SetRepeats(-1);
                            }
                            else
                            {
                                // UPGRADE_WARNING: オブジェクト DMSegment.SetRepeats の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                DMSegment.SetRepeats(0);
                            }

                            if (Information.Err().Number != 0)
                            {
                                string argmsg4 = "SetRepeats failed (" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Information.Err().Number) + ")";
                                GUI.ErrorMessage(argmsg4);
                            }

                            // 演奏開始
                            // UPGRADE_WARNING: オブジェクト DMPerformance.PlaySegment の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            DMPerformance.PlaySegment(DMSegment, 0, 0);
                            if (Information.Err().Number != 0)
                            {
                                string argmsg5 = "PlaySegment failed (" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Information.Err().Number) + ")";
                                GUI.ErrorMessage(argmsg5);
                            }

                            BGMFileName = fname;
                            return;
                        }

                        if (GeneralLib.GetWinVersion() >= 500)
                        {
                            cmd = " type mpegvideo alias bgm wait";
                        }
                        else
                        {
                            cmd = " type sequencer alias bgm wait";
                        }

                        break;
                    }

                case ".wav":
                    {
                        // WAVEファイル
                        cmd = " type waveaudio alias bgm wait";
                        break;
                    }

                case ".mp3":
                    {
                        // MP3ファイル

                        // VBMP3.dllを初期化
                        if (!IsMP3Supported)
                        {
                            InitVBMP3();
                            if (!IsMP3Supported)
                            {
                                // VBMP3.dllが利用不能
                                return;
                            }
                        }

                        // 演奏を停止
                        VBMP3.vbmp3_stop();
                        // ファイルを閉じる
                        VBMP3.vbmp3_close();

                        // ファイルを読み込む
                        if (VBMP3.vbmp3_open(fname, mp3_data))
                        {
                            // '繰り返し再生時は雑音が入らないようフェードアウトさせる
                            // If RepeatMode Then
                            // Call vbmp3_setFadeOut(1)
                            // Else
                            // Call vbmp3_setFadeOut(0)
                            // End If

                            // 演奏開始
                            VBMP3.vbmp3_play();
                            BGMFileName = fname;
                        }

                        return;
                    }

                default:
                    {
                        // 未サポートのファイル形式
                        return;
                    }
            }

            // ファイルを開く
            cmd = "open " + '"' + fname + '"' + cmd;
            ret = Sound.mciSendString(cmd, Constants.vbNullString, 0, 0);
            if (ret != 0)
            {
                // 開けなかった
                return;
            }

            // 演奏開始
            string arglpstrCommand = "play bgm";
            ret = Sound.mciSendString(arglpstrCommand, Constants.vbNullString, 0, 0);
            if (ret != 0)
            {
                // 演奏できなかった
                string arglpstrCommand1 = "close bgm wait";
                ret = Sound.mciSendString(arglpstrCommand1, Constants.vbNullString, 0, 0);
                return;
            }

            // 演奏しているBGMのファイル名を記録
            BGMFileName = fname;
            return;
        ErrorHandler:
            ;
            if (UseDirectMusic)
            {
                // DirectMusicが使用できない場合はMCIを使ってリトライ
                UseDirectMusic = false;
                UseMCI = true;
                LoadBGM(fname);
            }
        }

        // ＢＧＭをリスタートさせる
        public void RestartBGM()
        {
            var ret = default;

            // 停止中でなければ何もしない
            if (BGMStatus() != "stopped")
            {
                return;
            }

            // リスタート
            switch (Strings.LCase(Strings.Right(BGMFileName, 4)) ?? "")
            {
                case ".mid":
                    {
                        // MIDIファイル
                        if (UseMCI)
                        {
                            string arglpstrCommand = "seek bgm to start wait";
                            ret = Sound.mciSendString(arglpstrCommand, Constants.vbNullString, 0, 0);
                            if (ret != 0)
                            {
                                return;
                            }

                            string arglpstrCommand1 = "play bgm";
                            ret = Sound.mciSendString(arglpstrCommand1, Constants.vbNullString, 0, 0);
                        }

                        break;
                    }

                case ".wav":
                    {
                        // WAVEファイル
                        string arglpstrCommand2 = "seek bgm to start wait";
                        ret = Sound.mciSendString(arglpstrCommand2, Constants.vbNullString, 0, 0);
                        if (ret != 0)
                        {
                            return;
                        }

                        string arglpstrCommand3 = "play bgm";
                        ret = Sound.mciSendString(arglpstrCommand3, Constants.vbNullString, 0, 0);
                        break;
                    }

                case ".mp3":
                    {
                        // MP3ファイル
                        if (VBMP3.vbmp3_getState(ret) == 2)
                        {
                            VBMP3.vbmp3_restart();
                        }
                        else
                        {
                            VBMP3.vbmp3_play();
                        }

                        break;
                    }
            }
        }

        // ＢＧＭを停止する
        public void StopBGM(bool by_force = false)
        {
            int ret;

            // ＢＧＭを固定中？
            if (!by_force & KeepBGM)
            {
                return;
            }

            // 強制的に停止するのでなければ演奏中でない限りなにもしない
            if (!by_force & Strings.Len(BGMFileName) == 0)
            {
                return;
            }

            switch (Strings.LCase(Strings.Right(BGMFileName, 4)) ?? "")
            {
                case ".mid":
                case var @case when @case == "":
                    {
                        // MIDIファイル
                        // 演奏を停止
                        if (UseDirectMusic)
                        {
                            ;
                            // UPGRADE_WARNING: オブジェクト DMPerformance.Stop の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            DMPerformance.Stop(DMSegment, default, 0, 0);
                            if (Information.Err().Number != 0)
                            {
                                string argmsg = "DMPerformance.Stop failed (" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Information.Err().Number) + ")";
                                GUI.ErrorMessage(argmsg);
                            }
                        }
                        else
                        {
                            // 演奏を停止
                            string arglpstrCommand = "stop bgm wait";
                            ret = Sound.mciSendString(arglpstrCommand, Constants.vbNullString, 0, 0);
                            // ファイルを閉じる
                            string arglpstrCommand1 = "close bgm wait";
                            ret = Sound.mciSendString(arglpstrCommand1, Constants.vbNullString, 0, 0);
                        }

                        break;
                    }

                case ".wav":
                    {
                        // WAVEファイル
                        // 演奏を停止
                        string arglpstrCommand2 = "stop bgm wait";
                        ret = Sound.mciSendString(arglpstrCommand2, Constants.vbNullString, 0, 0);
                        // ファイルを閉じる
                        string arglpstrCommand3 = "close bgm wait";
                        ret = Sound.mciSendString(arglpstrCommand3, Constants.vbNullString, 0, 0);
                        break;
                    }

                case ".mp3":
                    {
                        // MP3ファイル
                        // 演奏を停止
                        VBMP3.vbmp3_stop();
                        // ファイルを閉じる
                        VBMP3.vbmp3_close();
                        break;
                    }
            }

            BGMFileName = "";
            RepeatMode = false;

            // リピート再生処理を行うためのタイマーを停止
            // UPGRADE_ISSUE: Control Timer1 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            GUI.MainForm.Timer1.Enabled = false;
        }

        // ＭＩＤＩ音源を初期化する(MCIを使用する場合のみ)
        private void ResetBGM()
        {
            int ret;
            string fname, cmd;

            // 音源の種類に応じた音源初期化用MIDIファイルを選択
            switch (SRC.MidiResetType ?? "")
            {
                case "GM":
                    {
                        // DirectMusicを使えばGMリセットが可能
                        if (UseDirectMusic)
                        {
                            ;
                            // UPGRADE_WARNING: オブジェクト DMPerformance.Reset の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            DMPerformance.Reset(0);
                            return;
                        }

                        fname = SRC.AppPath + @"Midi\Reset(GM).mid";
                        break;
                    }

                case "GS":
                    {
                        fname = SRC.AppPath + @"Midi\Reset(GS).mid";
                        break;
                    }

                case "XG":
                    {
                        fname = SRC.AppPath + @"Midi\Reset(XG).mid";
                        break;
                    }

                default:
                    {
                        return;
                    }
            }

            // ファイルがちゃんとある？
            if (!GeneralLib.FileExists(fname))
            {
                return;
            }

            BGMFileName = "";
            // DirectMusicを使う場合
            if (UseDirectMusic)
            {
                ;

                // ファイルをロード
                // UPGRADE_WARNING: オブジェクト DMLoader.LoadSegment の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                DMSegment = DMLoader.LoadSegment(fname);

                // MIDI再生のため各種パラメータを設定
                // UPGRADE_WARNING: オブジェクト DMSegment.SetStandardMidiFile の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                DMSegment.SetStandardMidiFile();
                // UPGRADE_WARNING: オブジェクト DMPerformance.SetMasterAutoDownload の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                DMPerformance.SetMasterAutoDownload(true);
                // UPGRADE_WARNING: オブジェクト DMSegment.SetLoopPoints の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                DMSegment.SetLoopPoints(0, 0);
                // UPGRADE_WARNING: オブジェクト DMSegment.SetRepeats の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                DMSegment.SetRepeats(0);

                // 音源リセット用MIDIファイルの演奏開始
                // UPGRADE_WARNING: オブジェクト DMPerformance.PlaySegment の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                DMPerformance.PlaySegment(DMSegment, 0, 0);

                // 演奏が終わるまで待つ
                // UPGRADE_WARNING: オブジェクト DMPerformance.IsPlaying の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                while (DMPerformance.IsPlaying(DMSegment, default))
                    Application.DoEvents();

                // 演奏を停止
                // UPGRADE_WARNING: オブジェクト DMPerformance.Stop の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                DMPerformance.Stop(DMSegment, default, 0, 0);
            }
            else
            {
                // MCIを使う場合

                // ファイルをオープン
                cmd = "open " + '"' + fname + '"' + " type sequencer alias bgm wait";
                ret = Sound.mciSendString(cmd, Constants.vbNullString, 0, 0);
                if (ret != 0)
                {
                    return;
                }

                // 音源リセット用MIDIファイルを演奏
                string arglpstrCommand = "play bgm wait";
                ret = Sound.mciSendString(arglpstrCommand, Constants.vbNullString, 0, 0);

                // ファイルをクローズ
                string arglpstrCommand1 = "close bgm wait";
                ret = Sound.mciSendString(arglpstrCommand1, Constants.vbNullString, 0, 0);
            }

            return;
        ErrorHandler:
            ;

            // DirectMusic使用時にエラーが発生したのでMCIを使う
            UseDirectMusic = false;
            UseMCI = true;
        }

        // ＢＧＭを再生中？
        private string BGMStatus()
        {
            string BGMStatusRet = default;
            string retstr;
            int ret, sec = default;

            // ＢＧＭを演奏中でなければ空文字列を返す
            if (Strings.Len(BGMFileName) == 0)
            {
                return BGMStatusRet;
            }

            switch (Strings.LCase(Strings.Right(BGMFileName, 4)) ?? "")
            {
                case ".mid":
                case var @case when @case == "":
                    {
                        // MIDIファイル
                        // DirectMusicを使う場合
                        if (UseDirectMusic)
                        {
                            ;
                            // UPGRADE_WARNING: オブジェクト DMPerformance.IsPlaying の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            if (DMPerformance.IsPlaying(DMSegment, default))
                            {
                                BGMStatusRet = "playing";
                            }
                            else
                            {
                                BGMStatusRet = "stopped";
                            }

                            if (Information.Err().Number != 0)
                            {
                                string argmsg = "DMPerformance.IsPlaying failed (" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Information.Err().Number) + ")";
                                GUI.ErrorMessage(argmsg);
                            }
                        }
                        else
                        {
                            // MCIを使う場合

                            // 結果を保存する領域を確保
                            retstr = Strings.Space(120);

                            // 再生状況を参照
                            string arglpstrCommand = "status bgm mode";
                            ret = Sound.mciSendString(arglpstrCommand, retstr, 120, 0);
                            if (ret != 0)
                            {
                                return BGMStatusRet;
                            }

                            // APIの結果はNULLターミネイト
                            ret = Strings.InStr(retstr, Conversions.ToString('\0'));
                            BGMStatusRet = Strings.Left(retstr, ret - 1);
                        }

                        break;
                    }

                case ".wav":
                    {
                        // WAVEファイル

                        // 結果を保存する領域を確保
                        retstr = Strings.Space(120);
                        string arglpstrCommand1 = "status bgm mode";
                        ret = Sound.mciSendString(arglpstrCommand1, retstr, 120, 0);
                        if (ret != 0)
                        {
                            return BGMStatusRet;
                        }

                        // APIの結果はNULLターミネイト
                        ret = Strings.InStr(retstr, Conversions.ToString('\0'));
                        BGMStatusRet = Strings.Left(retstr, ret - 1);
                        break;
                    }

                case ".mp3":
                    {
                        // MP3の再生状態と再生時間の取得
                        ret = VBMP3.vbmp3_getState(sec);
                        switch (ret)
                        {
                            case 0:
                                {
                                    // 停止中
                                    BGMStatusRet = "stopped";
                                    break;
                                }

                            case 1:
                                {
                                    // 再生中
                                    BGMStatusRet = "playing";
                                    break;
                                }

                            case 2:
                                {
                                    // 一時停止中
                                    BGMStatusRet = "stopped";
                                    break;
                                }
                        }

                        break;
                    }
            }

            return BGMStatusRet;
        }

        // ＢＧＭを変更する (指定したＢＧＭをすでに演奏中ならなにもしない)
        public void ChangeBGM(string bgm_name)
        {
            string fname, fname2;

            // ＢＧＭ固定中？
            if (KeepBGM | BossBGM)
            {
                return;
            }

            // 正しいファイル名？
            if (Strings.Len(bgm_name) < 5)
            {
                return;
            }

            // ファイル名の本体部分を抜き出す
            fname = Strings.Left(bgm_name, Strings.Len(bgm_name) - 4);
            string argstr21 = @"\";
            if (GeneralLib.InStr2(fname, argstr21) > 0)
            {
                string argstr2 = @"\";
                fname = Strings.Mid(fname, GeneralLib.InStr2(fname, argstr2) + 1);
            }

            // 既に同じMIDIが演奏されていればそのまま演奏し続ける
            if (Strings.Len(BGMFileName) > 0)
            {
                if (Strings.InStr(BGMFileName, @"\" + fname + ".") > 0)
                {
                    return;
                }
            }

            // 番号違い？
            if (Strings.Len(BGMFileName) > 5)
            {
                fname2 = Strings.Left(BGMFileName, Strings.Len(BGMFileName) - 4);
                string argstr23 = @"\";
                if (GeneralLib.InStr2(fname2, argstr23) > 0)
                {
                    string argstr22 = @"\";
                    fname2 = Strings.Mid(fname2, GeneralLib.InStr2(fname2, argstr22) + 1);
                }

                if (Strings.Len(fname2) > 4)
                {
                    switch (Strings.Right(fname2, 3) ?? "")
                    {
                        case "(2)":
                        case "(3)":
                        case "(4)":
                        case "(5)":
                        case "(6)":
                        case "(7)":
                        case "(8)":
                        case "(9)":
                            {
                                fname2 = Strings.Left(fname2, Strings.Len(fname2) - 3);
                                break;
                            }
                    }
                }

                if ((fname ?? "") == (fname2 ?? ""))
                {
                    return;
                }
            }

            // 繰り返し演奏に設定
            RepeatMode = true;

            // 演奏開始
            StartBGM(bgm_name);
        }


        // DirectMusicの初期化
        public void InitDirectMusic()
        {
            var DMUS_PC_GMINHARDWARE = default(object);
            var DMUS_PC_GSINHARDWARE = default(object);
            var DMUS_PC_XGINHARDWARE = default(object);
            var DMUS_PC_EXTERNAL = default(object);
            int port_id;
            // UPGRADE_ISSUE: DMUS_PORTCAPS オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
            var portcaps = default(DMUS_PORTCAPS);
            int i;
            ;

            // フラグを設定
            UseDirectMusic = true;
            UseMCI = false;

            // DirectXオブジェクト作成
            if (DXObject is null)
            {
                DXObject = CreateDirectXObject();
            }

            // Loader作成
            // UPGRADE_WARNING: オブジェクト DXObject.DirectMusicLoaderCreate の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            DMLoader = DXObject.DirectMusicLoaderCreate;

            // サーチパス設定(不要？)
            // UPGRADE_WARNING: オブジェクト DMLoader.SetSearchDirectory の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            DMLoader.SetSearchDirectory(SRC.AppPath + "Midi");

            // Performance作成
            // UPGRADE_WARNING: オブジェクト DXObject.DirectMusicPerformanceCreate の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            DMPerformance = DXObject.DirectMusicPerformanceCreate;

            // Performance初期化
            // DirectSoundと併用する時は、最初の引数に
            // DirectSoundのオブジェクトを入れておく
            // UPGRADE_WARNING: オブジェクト DMPerformance.Init の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            DMPerformance.Init(DSObject, GUI.MainForm.Handle.ToInt32());

            // MIDI音源一覧を作成
            CreateMIDIPortListFile();

            // ポート設定
            string argini_section = "Option";
            string argini_entry = "MIDIPortID";
            string argexpr = GeneralLib.ReadIni(argini_section, argini_entry);
            port_id = GeneralLib.StrToLng(argexpr);

            // 使用ポート番号を指定されていた場合
            if (port_id > 0)
            {
                // UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCount の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                if (port_id > DMPerformance.GetPortCount)
                {
                    string argmsg = "MIDIPortIDに正しいMIDIポートが設定されていません。";
                    GUI.ErrorMessage(argmsg);
                    Environment.Exit(0);
                }

                // UPGRADE_WARNING: オブジェクト DMPerformance.SetPort の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                DMPerformance.SetPort(port_id, 1);
                return;
            }

            // 指定がないのでSRC側で検索する

            // MIDIマッパーがあればそれを使う
            // UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCount の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            var loopTo = DMPerformance.GetPortCount;
            for (i = 1; i <= loopTo; i++)
            {
                // UPGRADE_WARNING: オブジェクト DMPerformance.GetPortName の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                if (Strings.InStr(DMPerformance.GetPortName(i), "MIDI マッパー") > 0 | Strings.InStr(DMPerformance.GetPortName(i), "MIDI Mapper") > 0)
                {
                    // UPGRADE_WARNING: オブジェクト DMPerformance.SetPort の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    DMPerformance.SetPort(i, 1);
                    return;
                }
            }

            // まずは外部MIDI音源を捜す
            // UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCount の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            var loopTo1 = DMPerformance.GetPortCount;
            for (i = 1; i <= loopTo1; i++)
            {
                // UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCaps の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                DMPerformance.GetPortCaps(i, portcaps);
                // UPGRADE_WARNING: オブジェクト portcaps.lFlags の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                if (portcaps.lFlags & DMUS_PC_EXTERNAL)
                {
                    // UPGRADE_WARNING: オブジェクト DMPerformance.SetPort の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    DMPerformance.SetPort(i, 1);
                    return;
                }
            }

            // 次にXG対応ハード音源を捜す
            // UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCount の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            var loopTo2 = DMPerformance.GetPortCount;
            for (i = 1; i <= loopTo2; i++)
            {
                // UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCaps の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                DMPerformance.GetPortCaps(i, portcaps);
                // UPGRADE_WARNING: オブジェクト portcaps.lFlags の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                if (portcaps.lFlags & DMUS_PC_XGINHARDWARE)
                {
                    // UPGRADE_WARNING: オブジェクト DMPerformance.SetPort の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    DMPerformance.SetPort(i, 1);
                    return;
                }
            }

            // 次にGS対応ハード音源を捜す
            // UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCount の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            var loopTo3 = DMPerformance.GetPortCount;
            for (i = 1; i <= loopTo3; i++)
            {
                // UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCaps の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                DMPerformance.GetPortCaps(i, portcaps);
                // UPGRADE_WARNING: オブジェクト portcaps.lFlags の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                if (portcaps.lFlags & DMUS_PC_GSINHARDWARE)
                {
                    // UPGRADE_WARNING: オブジェクト DMPerformance.SetPort の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    DMPerformance.SetPort(i, 1);
                    return;
                }
            }

            // 次にXG対応ソフト音源を捜す
            // UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCount の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            var loopTo4 = DMPerformance.GetPortCount;
            for (i = 1; i <= loopTo4; i++)
            {
                // UPGRADE_WARNING: オブジェクト DMPerformance.GetPortName の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                if (Strings.InStr(DMPerformance.GetPortName(i), "XG ") > 0)
                {
                    // UPGRADE_WARNING: オブジェクト DMPerformance.SetPort の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    DMPerformance.SetPort(i, 1);
                    return;
                }
            }

            // 次にGS対応ソフト音源を捜す
            // UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCount の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            var loopTo5 = DMPerformance.GetPortCount;
            for (i = 1; i <= loopTo5; i++)
            {
                // UPGRADE_WARNING: オブジェクト DMPerformance.GetPortName の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                if (Strings.InStr(DMPerformance.GetPortName(i), "GS ") > 0)
                {
                    // UPGRADE_WARNING: オブジェクト DMPerformance.SetPort の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    DMPerformance.SetPort(i, 1);
                    return;
                }
            }

            // 次にGM対応ハード音源を捜す
            // UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCount の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            var loopTo6 = DMPerformance.GetPortCount;
            for (i = 1; i <= loopTo6; i++)
            {
                // UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCaps の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                DMPerformance.GetPortCaps(i, portcaps);
                // UPGRADE_WARNING: オブジェクト portcaps.lFlags の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                if (portcaps.lFlags & DMUS_PC_GMINHARDWARE)
                {
                    // UPGRADE_WARNING: オブジェクト DMPerformance.SetPort の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    DMPerformance.SetPort(i, 1);
                    return;
                }
            }

            // あきらめてデフォルトポートを使う
            // UPGRADE_WARNING: オブジェクト DMPerformance.SetPort の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            DMPerformance.SetPort(-1, 1);
            return;
        ErrorHandler:
            ;


            // DirectMusic初期化時にエラーが発生したのでMCIを使う
            UseDirectMusic = false;
            UseMCI = true;
        }

        // DirectXオブジェクトを作成する
        private DirectX7 CreateDirectXObject()
        {
            DirectX7 CreateDirectXObjectRet = default;
            // UPGRADE_ISSUE: DirectX7 オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
            var new_obj = new DirectX7();
            CreateDirectXObjectRet = new_obj;
            return CreateDirectXObjectRet;
        }

        // 利用可能なMIDI音源の一覧を作成する
        private void CreateMIDIPortListFile()
        {
            int f, i;
            string pname;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 35847


            Input:

                    On Error GoTo ErrorHandler

             */
            f = FileSystem.FreeFile();
            FileSystem.FileOpen(f, SRC.AppPath + @"Midi\MIDI音源リスト.txt", OpenMode.Output, OpenAccess.Write);
            FileSystem.PrintLine(f, ";DirectMusicで利用可能なMIDI音源のリストです。");
            FileSystem.PrintLine(f, ";Src.iniのMIDIPortIDに使用したい音源の番号を指定して下さい。");
            FileSystem.PrintLine(f, "");

            // 各ポートの名称を参照
            // UPGRADE_WARNING: オブジェクト DMPerformance.GetPortCount の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            var loopTo = DMPerformance.GetPortCount;
            for (i = 1; i <= loopTo; i++)
            {
                // UPGRADE_WARNING: オブジェクト DMPerformance.GetPortName の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                pname = DMPerformance.GetPortName(i);
                if (Strings.InStr(pname, "[") > 0)
                {
                    pname = Strings.Left(pname, Strings.InStr(pname, "[") - 1);
                }

                FileSystem.PrintLine(f, Microsoft.VisualBasic.Compatibility.VB6.Support.Format(i) + ":" + pname);
            }

            FileSystem.FileClose(f);
        ErrorHandler:
            ;

            // エラー発生
        }


        // VBMP3を初期化
        private void InitVBMP3()
        {
            VBMP3.VBMP3_OPTION opt;
            string buf;
            bool localFileExists() { string argfname = SRC.AppPath + "VBMP3.dll"; var ret = GeneralLib.FileExists(argfname); return ret; }

            if (!localFileExists())
            {
                return;
            }

            VBMP3.vbmp3_init();
            VBMP3.vbmp3_setVolume(MP3Volume, MP3Volume);
            opt.inputBlock = 30;
            string argini_section = "Option";
            string argini_entry = "MP3OutputBlock";
            buf = GeneralLib.ReadIni(argini_section, argini_entry);
            opt.outputBlock = GeneralLib.MinLng(GeneralLib.StrToLng(buf), 30);
            string argini_section1 = "Option";
            string argini_entry1 = "MP3InputSleep";
            buf = GeneralLib.ReadIni(argini_section1, argini_entry1);
            opt.inputSleep = GeneralLib.MaxLng(GeneralLib.StrToLng(buf), 0);
            opt.outputSleep = 0;
            VBMP3.vbmp3_setVbmp3Option(opt);
            IsMP3Supported = true;
        }


        // 各Midiフォルダから指定されたMIDIファイルを検索する
        public string SearchMidiFile(string midi_name)
        {
            string SearchMidiFileRet = default;
            string fname, fname_mp3 = default;
            ;
#error Cannot convert LocalDeclarationStatementSyntax - see comment for details
            /* Cannot convert LocalDeclarationStatementSyntax, System.NotSupportedException: Keyword not supported!
               場所 ICSharpCode.CodeConverter.CSharp.SyntaxKindExtensions.ConvertToken(SyntaxKind t, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifier(SyntaxToken m, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.<ConvertModifiersCore>d__43.MoveNext()
               場所 System.Linq.Enumerable.<ConcatIterator>d__59`1.MoveNext()
               場所 System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
               場所 System.Linq.Buffer`1..ctor(IEnumerable`1 source)
               場所 System.Linq.OrderedEnumerable`1.<GetEnumerator>d__1.MoveNext()
               場所 Microsoft.CodeAnalysis.SyntaxTokenList.CreateNode(IEnumerable`1 tokens)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifiers(SyntaxNode node, IReadOnlyCollection`1 modifiers, TokenContext context, Boolean isVariableOrConst, SyntaxKind[] extraCsModifierKinds)
               場所 ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.<VisitLocalDeclarationStatement>d__31.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.HoistedNodeStateVisitor.<AddLocalVariablesAsync>d__6.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.<DefaultVisitInnerAsync>d__3.MoveNext()

            Input:
                     scenario_midi_dir_exists As Boolean

             */
            ;
#error Cannot convert LocalDeclarationStatementSyntax - see comment for details
            /* Cannot convert LocalDeclarationStatementSyntax, System.NotSupportedException: Keyword not supported!
               場所 ICSharpCode.CodeConverter.CSharp.SyntaxKindExtensions.ConvertToken(SyntaxKind t, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifier(SyntaxToken m, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.<ConvertModifiersCore>d__43.MoveNext()
               場所 System.Linq.Enumerable.<ConcatIterator>d__59`1.MoveNext()
               場所 System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
               場所 System.Linq.Buffer`1..ctor(IEnumerable`1 source)
               場所 System.Linq.OrderedEnumerable`1.<GetEnumerator>d__1.MoveNext()
               場所 Microsoft.CodeAnalysis.SyntaxTokenList.CreateNode(IEnumerable`1 tokens)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifiers(SyntaxNode node, IReadOnlyCollection`1 modifiers, TokenContext context, Boolean isVariableOrConst, SyntaxKind[] extraCsModifierKinds)
               場所 ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.<VisitLocalDeclarationStatement>d__31.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.HoistedNodeStateVisitor.<AddLocalVariablesAsync>d__6.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.<DefaultVisitInnerAsync>d__3.MoveNext()

            Input:
                     extdata_midi_dir_exists As Boolean

             */
            ;
#error Cannot convert LocalDeclarationStatementSyntax - see comment for details
            /* Cannot convert LocalDeclarationStatementSyntax, System.NotSupportedException: Keyword not supported!
               場所 ICSharpCode.CodeConverter.CSharp.SyntaxKindExtensions.ConvertToken(SyntaxKind t, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifier(SyntaxToken m, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.<ConvertModifiersCore>d__43.MoveNext()
               場所 System.Linq.Enumerable.<ConcatIterator>d__59`1.MoveNext()
               場所 System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
               場所 System.Linq.Buffer`1..ctor(IEnumerable`1 source)
               場所 System.Linq.OrderedEnumerable`1.<GetEnumerator>d__1.MoveNext()
               場所 Microsoft.CodeAnalysis.SyntaxTokenList.CreateNode(IEnumerable`1 tokens)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifiers(SyntaxNode node, IReadOnlyCollection`1 modifiers, TokenContext context, Boolean isVariableOrConst, SyntaxKind[] extraCsModifierKinds)
               場所 ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.<VisitLocalDeclarationStatement>d__31.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.HoistedNodeStateVisitor.<AddLocalVariablesAsync>d__6.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.<DefaultVisitInnerAsync>d__3.MoveNext()

            Input:
                     extdata2_midi_dir_exists As Boolean

             */
            ;
            //  fpath_history As New Collection     DEL MARGE
            int j, i, num;
            string buf, buf2;
            var sub_folder = default(string);

            // 初めて実行する際に、各フォルダにMidiフォルダがあるかチェック
            if (!IsMidiSearchPathInitialized)
            {
                if (Strings.Len(SRC.ScenarioPath) > 0)
                {
                    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                    if (Strings.Len(FileSystem.Dir(SRC.ScenarioPath + "Midi", FileAttribute.Directory)) > 0)
                    {
                        scenario_midi_dir_exists = true;
                    }
                }

                if (Strings.Len(SRC.ExtDataPath) > 0)
                {
                    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                    if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath + "Midi", FileAttribute.Directory)) > 0)
                    {
                        extdata_midi_dir_exists = true;
                    }
                }

                if (Strings.Len(SRC.ExtDataPath2) > 0)
                {
                    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                    if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath2 + "Midi", FileAttribute.Directory)) > 0)
                    {
                        extdata2_midi_dir_exists = true;
                    }
                }

                // MP3が演奏可能かどうかも調べておく
                string argfname = SRC.AppPath + "VBMP3.dll";
                if (GeneralLib.FileExists(argfname))
                {
                    is_mp3_available = true;
                }

                IsMidiSearchPathInitialized = true;
            }

            // ダミーのファイル名？
            if (Strings.Len(midi_name) < 5)
            {
                return SearchMidiFileRet;
            }

            // 引数1として渡された文字列をリストとして扱い、左から順にMIDIを検索
            num = GeneralLib.ListLength(midi_name);
            i = 1;
            while (i <= num)
            {
                // スペースを含むファイル名への対応
                buf = "";
                var loopTo = num;
                for (j = i; j <= loopTo; j++)
                {
                    buf2 = Strings.LCase(GeneralLib.ListIndex(midi_name, j));

                    // 全体が()で囲まれている場合は()を外す
                    if (Strings.Left(buf2, 1) == "(" & Strings.Right(buf2, 1) == ")")
                    {
                        buf2 = Strings.Mid(buf2, 2, Strings.Len(buf2) - 2);
                    }

                    buf = buf + " " + buf2;
                    if (Strings.Right(buf, 4) == ".mid")
                    {
                        break;
                    }
                }

                buf = Strings.Trim(buf);

                // 同名のMP3ファイルがある場合はMIDIファイルの代わりにMP3ファイルを使う
                if (is_mp3_available)
                {
                    fname_mp3 = Strings.Left(buf, Strings.Len(buf) - 4) + ".mp3";
                }

                // フルパスでの指定？
                if (Strings.InStr(buf, ":") == 2)
                {
                    if (is_mp3_available)
                    {
                        if (GeneralLib.FileExists(fname_mp3))
                        {
                            SearchMidiFileRet = fname_mp3;
                            return SearchMidiFileRet;
                        }
                    }

                    if (GeneralLib.FileExists(buf))
                    {
                        SearchMidiFileRet = buf;
                    }

                    return SearchMidiFileRet;
                }

                // DEL START MARGE
                // '履歴を検索してみる
                // On Error GoTo NotFound
                // fname = fpath_history.Item(buf)
                // 
                // '履歴上にファイルを発見
                // SearchMidiFile = fname
                // Exit Function

                // NotFound:
                // '履歴になかった
                // On Error GoTo 0
                // DEL END MARGE

                // サブフォルダ指定あり？
                if (Strings.InStr(buf, "_") > 0)
                {
                    sub_folder = Strings.Left(buf, Strings.InStr(buf, "_") - 1) + @"\";
                }

                // シナリオ側のMidiフォルダ
                if (scenario_midi_dir_exists)
                {
                    if (is_mp3_available)
                    {
                        if (!string.IsNullOrEmpty(sub_folder))
                        {
                            fname = SRC.ScenarioPath + @"Midi\" + sub_folder + fname_mp3;
                            if (GeneralLib.FileExists(fname))
                            {
                                SearchMidiFileRet = fname;
                                // fpath_history.Add fname, buf   DEL MARGE
                                return SearchMidiFileRet;
                            }
                        }

                        fname = SRC.ScenarioPath + @"Midi\" + fname_mp3;
                        if (GeneralLib.FileExists(fname))
                        {
                            SearchMidiFileRet = fname;
                            // fpath_history.Add fname, buf   DEL MARGE
                            return SearchMidiFileRet;
                        }
                    }

                    if (!string.IsNullOrEmpty(sub_folder))
                    {
                        fname = SRC.ScenarioPath + @"Midi\" + sub_folder + buf;
                        if (GeneralLib.FileExists(fname))
                        {
                            SearchMidiFileRet = fname;
                            // fpath_history.Add fname, buf   DEL MARGE
                            return SearchMidiFileRet;
                        }
                    }

                    fname = SRC.ScenarioPath + @"Midi\" + buf;
                    if (GeneralLib.FileExists(fname))
                    {
                        SearchMidiFileRet = fname;
                        // fpath_history.Add fname, buf   DEL MARGE
                        return SearchMidiFileRet;
                    }
                }

                // ExtDataPath側のMidiフォルダ
                if (extdata_midi_dir_exists)
                {
                    if (is_mp3_available)
                    {
                        if (!string.IsNullOrEmpty(sub_folder))
                        {
                            fname = SRC.ExtDataPath + @"Midi\" + sub_folder + fname_mp3;
                            if (GeneralLib.FileExists(fname))
                            {
                                SearchMidiFileRet = fname;
                                // fpath_history.Add fname, buf   DEL MARGE
                                return SearchMidiFileRet;
                            }
                        }

                        fname = SRC.ExtDataPath + @"Midi\" + fname_mp3;
                        if (GeneralLib.FileExists(fname))
                        {
                            SearchMidiFileRet = fname;
                            // fpath_history.Add fname, buf   DEL MARGE
                            return SearchMidiFileRet;
                        }
                    }

                    if (!string.IsNullOrEmpty(sub_folder))
                    {
                        fname = SRC.ExtDataPath + @"Midi\" + sub_folder + buf;
                        if (GeneralLib.FileExists(fname))
                        {
                            SearchMidiFileRet = fname;
                            // fpath_history.Add fname, buf   DEL MARGE
                            return SearchMidiFileRet;
                        }
                    }

                    fname = SRC.ExtDataPath + @"Midi\" + buf;
                    if (GeneralLib.FileExists(fname))
                    {
                        SearchMidiFileRet = fname;
                        // fpath_history.Add fname, buf   DEL MARGE
                        return SearchMidiFileRet;
                    }
                }

                // ExtDataPath2側のMidiフォルダ
                if (extdata2_midi_dir_exists)
                {
                    if (is_mp3_available)
                    {
                        if (!string.IsNullOrEmpty(sub_folder))
                        {
                            fname = SRC.ExtDataPath2 + @"Midi\" + sub_folder + fname_mp3;
                            if (GeneralLib.FileExists(fname))
                            {
                                SearchMidiFileRet = fname;
                                // fpath_history.Add fname, buf   DEL MARGE
                                return SearchMidiFileRet;
                            }
                        }

                        fname = SRC.ExtDataPath2 + @"Midi\" + fname_mp3;
                        if (GeneralLib.FileExists(fname))
                        {
                            SearchMidiFileRet = fname;
                            // fpath_history.Add fname, buf   DEL MARGE
                            return SearchMidiFileRet;
                        }
                    }

                    if (!string.IsNullOrEmpty(sub_folder))
                    {
                        fname = SRC.ExtDataPath2 + @"Midi\" + sub_folder + buf;
                        if (GeneralLib.FileExists(fname))
                        {
                            SearchMidiFileRet = fname;
                            // fpath_history.Add fname, buf   DEL MARGE
                            return SearchMidiFileRet;
                        }
                    }

                    fname = SRC.ExtDataPath2 + @"Midi\" + buf;
                    if (GeneralLib.FileExists(fname))
                    {
                        SearchMidiFileRet = fname;
                        // fpath_history.Add fname, buf   DEL MARGE
                        return SearchMidiFileRet;
                    }
                }

                // 本体側のMidiフォルダ
                if (is_mp3_available)
                {
                    if (!string.IsNullOrEmpty(sub_folder))
                    {
                        fname = SRC.AppPath + @"Midi\" + sub_folder + fname_mp3;
                        if (GeneralLib.FileExists(fname))
                        {
                            SearchMidiFileRet = fname;
                            // fpath_history.Add fname, buf   DEL MARGE
                            return SearchMidiFileRet;
                        }
                    }

                    fname = SRC.AppPath + @"Midi\" + fname_mp3;
                    if (GeneralLib.FileExists(fname))
                    {
                        SearchMidiFileRet = fname;
                        // fpath_history.Add fname, buf   DEL MARGE
                        return SearchMidiFileRet;
                    }
                }

                if (!string.IsNullOrEmpty(sub_folder))
                {
                    fname = SRC.AppPath + @"Midi\" + sub_folder + buf;
                    if (GeneralLib.FileExists(fname))
                    {
                        SearchMidiFileRet = fname;
                        // fpath_history.Add fname, buf   DEL MARGE
                        return SearchMidiFileRet;
                    }
                }

                fname = SRC.AppPath + @"Midi\" + buf;
                if (GeneralLib.FileExists(fname))
                {
                    SearchMidiFileRet = fname;
                    // fpath_history.Add fname, buf   DEL MARGE
                    return SearchMidiFileRet;
                }

                i = (j + 1);
            }

            return SearchMidiFileRet;
        }

        // MIDIファイルのサーチパスをリセットする
        public void ResetMidiSearchPath()
        {
            IsMidiSearchPathInitialized = false;
        }


        // ＢＧＭに割り当てられたMIDIファイル名を返す
        public string BGMName(string bgm_name)
        {
            string BGMNameRet = default;
            string vname;

            // RenameBGMコマンドでMIDIファイルが設定されていればそちらを使用
            vname = "BGM(" + bgm_name + ")";
            if (Expression.IsGlobalVariableDefined(vname))
            {
                // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                BGMNameRet = Conversions.ToString(Event_Renamed.GlobalVariableList[vname].StringValue);
                return BGMNameRet;
            }

            // そうでなければSrc.iniで設定されているファイルを使用
            string argini_section = "BGM";
            BGMNameRet = GeneralLib.ReadIni(argini_section, bgm_name);

            // Src.iniでも設定されていなければ標準のファイルを使用
            if (string.IsNullOrEmpty(BGMNameRet))
            {
                BGMNameRet = bgm_name + ".mid";
            }

            return BGMNameRet;
        }


        // DirectSoundの初期化
        public void InitDirectSound()
        {
            object DSSCL_PRIORITY;
            // On Error GoTo ErrorHandler
            return;

            // フラグを設定
            UseDirectSound = true;

            // DirectXオブジェクト作成
            if (DXObject is null)
            {
                DXObject = CreateDirectXObject();
            }

            // DirectSoundオブジェクト作成
            // UPGRADE_WARNING: オブジェクト DXObject.DirectSoundCreate の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            DSObject = DXObject.DirectSoundCreate("");

            // サウンドデバイスの協調レベルを設定
            // UPGRADE_WARNING: オブジェクト DSObject.SetCooperativeLevel の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            DSObject.SetCooperativeLevel(GUI.MainForm.Handle.ToInt32(), DSSCL_PRIORITY);
            return;
        ErrorHandler:
            ;
            UseDirectSound = false;
        }

        // Waveファイルを再生する
        public void PlayWave(string wave_name)
        {
            var DSBPLAY_DEFAULT = default(object);
            var DSBCAPS_ = default(object);
            var DSBCAPS_CTRLVOLUME = default(object);
            var DSBCAPS_CTRLPAN = default(object);
            var DSBCAPS_CTRLFREQUENCY = default(object);
            int ret;
            string fname;
            var mp3_data = default(VBMP3.InputInfo);
            ;
#error Cannot convert LocalDeclarationStatementSyntax - see comment for details
            /* Cannot convert LocalDeclarationStatementSyntax, System.NotSupportedException: Keyword not supported!
               場所 ICSharpCode.CodeConverter.CSharp.SyntaxKindExtensions.ConvertToken(SyntaxKind t, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifier(SyntaxToken m, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.<ConvertModifiersCore>d__43.MoveNext()
               場所 System.Linq.Enumerable.<ConcatIterator>d__59`1.MoveNext()
               場所 System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
               場所 System.Linq.Buffer`1..ctor(IEnumerable`1 source)
               場所 System.Linq.OrderedEnumerable`1.<GetEnumerator>d__1.MoveNext()
               場所 Microsoft.CodeAnalysis.SyntaxTokenList.CreateNode(IEnumerable`1 tokens)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifiers(SyntaxNode node, IReadOnlyCollection`1 modifiers, TokenContext context, Boolean isVariableOrConst, SyntaxKind[] extraCsModifierKinds)
               場所 ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.<VisitLocalDeclarationStatement>d__31.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.HoistedNodeStateVisitor.<AddLocalVariablesAsync>d__6.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.<DefaultVisitInnerAsync>d__3.MoveNext()

            Input:
                     init_play_wave As Boolean

             */
            ;
#error Cannot convert LocalDeclarationStatementSyntax - see comment for details
            /* Cannot convert LocalDeclarationStatementSyntax, System.NotSupportedException: Keyword not supported!
               場所 ICSharpCode.CodeConverter.CSharp.SyntaxKindExtensions.ConvertToken(SyntaxKind t, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifier(SyntaxToken m, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.<ConvertModifiersCore>d__43.MoveNext()
               場所 System.Linq.Enumerable.<ConcatIterator>d__59`1.MoveNext()
               場所 System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
               場所 System.Linq.Buffer`1..ctor(IEnumerable`1 source)
               場所 System.Linq.OrderedEnumerable`1.<GetEnumerator>d__1.MoveNext()
               場所 Microsoft.CodeAnalysis.SyntaxTokenList.CreateNode(IEnumerable`1 tokens)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifiers(SyntaxNode node, IReadOnlyCollection`1 modifiers, TokenContext context, Boolean isVariableOrConst, SyntaxKind[] extraCsModifierKinds)
               場所 ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.<VisitLocalDeclarationStatement>d__31.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.HoistedNodeStateVisitor.<AddLocalVariablesAsync>d__6.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.<DefaultVisitInnerAsync>d__3.MoveNext()

            Input:
                     scenario_sound_dir_exists As Boolean

             */
            ;
#error Cannot convert LocalDeclarationStatementSyntax - see comment for details
            /* Cannot convert LocalDeclarationStatementSyntax, System.NotSupportedException: Keyword not supported!
               場所 ICSharpCode.CodeConverter.CSharp.SyntaxKindExtensions.ConvertToken(SyntaxKind t, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifier(SyntaxToken m, TokenContext context)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.<ConvertModifiersCore>d__43.MoveNext()
               場所 System.Linq.Enumerable.<ConcatIterator>d__59`1.MoveNext()
               場所 System.Linq.Enumerable.WhereEnumerableIterator`1.MoveNext()
               場所 System.Linq.Buffer`1..ctor(IEnumerable`1 source)
               場所 System.Linq.OrderedEnumerable`1.<GetEnumerator>d__1.MoveNext()
               場所 Microsoft.CodeAnalysis.SyntaxTokenList.CreateNode(IEnumerable`1 tokens)
               場所 ICSharpCode.CodeConverter.CSharp.CommonConversions.ConvertModifiers(SyntaxNode node, IReadOnlyCollection`1 modifiers, TokenContext context, Boolean isVariableOrConst, SyntaxKind[] extraCsModifierKinds)
               場所 ICSharpCode.CodeConverter.CSharp.MethodBodyExecutableStatementVisitor.<VisitLocalDeclarationStatement>d__31.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.HoistedNodeStateVisitor.<AddLocalVariablesAsync>d__6.MoveNext()
            --- 直前に例外がスローされた場所からのスタック トレースの終わり ---
               場所 ICSharpCode.CodeConverter.CSharp.CommentConvertingMethodBodyVisitor.<DefaultVisitInnerAsync>d__3.MoveNext()

            Input:
                     extdata_sound_dir_exists As Boolean

             */
            ;

            // 初めて実行する際に、各フォルダにSoundフォルダがあるかチェック
            if (!init_play_wave)
            {
                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                if (Strings.Len(FileSystem.Dir(SRC.ScenarioPath + "Sound", FileAttribute.Directory)) > 0)
                {
                    scenario_sound_dir_exists = true;
                }

                if (Strings.Len(SRC.ExtDataPath) > 0)
                {
                    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                    if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath + "Sound", FileAttribute.Directory)) > 0)
                    {
                        extdata_sound_dir_exists = true;
                    }
                }

                if (Strings.Len(SRC.ExtDataPath2) > 0)
                {
                    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                    if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath2 + "Sound", FileAttribute.Directory)) > 0)
                    {
                        extdata2_sound_dir_exists = true;
                    }
                }

                init_play_wave = true;
            }

            // 特殊なファイル名
            switch (Strings.LCase(wave_name) ?? "")
            {
                case "-.wav":
                case "-.mp3":
                    {
                        // 再生をキャンセル
                        return;
                    }

                case "null.wav":
                    {
                        // WAVE再生を停止
                        StopWave();
                        return;
                    }

                case "null.mp3":
                    {
                        // MP3再生を停止
                        if (Strings.LCase(Strings.Right(BGMFileName, 4)) == ".mp3")
                        {
                            StopBGM(true);
                        }
                        else
                        {
                            // 演奏を停止
                            VBMP3.vbmp3_stop();
                            // ファイルを閉じる
                            VBMP3.vbmp3_close();
                        }

                        return;
                    }
            }

            // 各フォルダをチェック

            // シナリオ側のSoundフォルダ
            if (scenario_sound_dir_exists)
            {
                fname = SRC.ScenarioPath + @"Sound\" + wave_name;
                if (GeneralLib.FileExists(fname))
                {
                    goto FoundWave;
                }
            }

            // ExtDataPath側のSoundフォルダ
            if (extdata_sound_dir_exists)
            {
                fname = SRC.ExtDataPath + @"Sound\" + wave_name;
                if (GeneralLib.FileExists(fname))
                {
                    goto FoundWave;
                }
            }

            // ExtDataPath2側のSoundフォルダ
            if (extdata2_sound_dir_exists)
            {
                fname = SRC.ExtDataPath2 + @"Sound\" + wave_name;
                if (GeneralLib.FileExists(fname))
                {
                    goto FoundWave;
                }
            }

            // 本体側のSoundフォルダ
            fname = SRC.AppPath + @"Sound\" + wave_name;
            if (GeneralLib.FileExists(fname))
            {
                goto FoundWave;
            }

            // 絶対表記？
            fname = wave_name;
            if (GeneralLib.FileExists(fname))
            {
                goto FoundWave;
            }

            // 見つからなかった
            return;
        FoundWave:
            ;


            // UPGRADE_ISSUE: WAVEFORMATEX オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
            var wf = default(WAVEFORMATEX);
            // UPGRADE_ISSUE: DSBUFFERDESC オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
            var dsbd = default(DSBUFFERDESC);
            if (Strings.LCase(Strings.Right(fname, 4)) == ".mp3")
            {
                // 効果音はMP3ファイル

                // VBMP3.dllを初期化
                if (!IsMP3Supported)
                {
                    InitVBMP3();
                    if (!IsMP3Supported)
                    {
                        // VBMP3.dllが利用不能
                        return;
                    }
                }

                // MP3再生を停止
                if (Strings.LCase(Strings.Right(BGMFileName, 4)) == ".mp3")
                {
                    StopBGM(true);
                }
                else
                {
                    // 演奏を停止
                    VBMP3.vbmp3_stop();
                    // ファイルを閉じる
                    VBMP3.vbmp3_close();
                }

                // ファイルを読み込む
                if (VBMP3.vbmp3_open(fname, mp3_data))
                {
                    // 再生開始
                    VBMP3.vbmp3_play();
                }
            }
            else if (UseDirectSound)
            {
                // DirectSoundを使う場合


                // 再生中の場合は再生をストップ
                if (DSBuffer is object)
                {
                    // UPGRADE_WARNING: オブジェクト DSBuffer.Stop の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    DSBuffer.Stop();
                    // UPGRADE_NOTE: オブジェクト DSBuffer をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                    DSBuffer = null;
                }

                // サウンドバッファにWAVファイルを読み込む
                // UPGRADE_WARNING: オブジェクト dsbd.lFlags の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                dsbd.lFlags = Operators.OrObject(Operators.OrObject(Operators.OrObject(DSBCAPS_CTRLFREQUENCY, DSBCAPS_CTRLPAN), DSBCAPS_CTRLVOLUME), DSBCAPS_);
                // UPGRADE_WARNING: オブジェクト DSObject.CreateSoundBufferFromFile の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                DSBuffer = DSObject.CreateSoundBufferFromFile(fname, dsbd, wf);

                // WAVEを再生
                // UPGRADE_WARNING: オブジェクト DSBuffer.Play の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                DSBuffer.Play(DSBPLAY_DEFAULT);
            }
            else
            {
                // APIを使う場合

                // WAVEを再生
                ret = Sound.sndPlaySound(fname, SND_ASYNC + SND_NODEFAULT);
            }

            // 効果音再生のフラグを立てる
            IsWavePlayed = true;
        }

        // Waveファイルの再生を終了する
        public void StopWave()
        {
            int ret;
            if (UseDirectSound)
            {
                if (DSBuffer is object)
                {
                    // UPGRADE_WARNING: オブジェクト DSBuffer.Stop の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    DSBuffer.Stop();
                }
            }
            else
            {
                ret = Sound.sndPlaySound(Constants.vbNullString, 0);
            }
        }


        // 本モジュールの解放処理を行う
        public void FreeSoundModule()
        {
            // BGM演奏の停止
            KeepBGM = false;
            BossBGM = false;
            StopBGM(true);

            // 音源初期化
            ResetBGM();

            // WAVEファイル再生の停止
            StopWave();

            // DirectMusicの解放
            if (UseDirectMusic)
            {
                // 演奏停止
                // UPGRADE_WARNING: オブジェクト DMPerformance.CloseDown の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                DMPerformance.CloseDown();

                // オブジェクトの解放
                // UPGRADE_NOTE: オブジェクト DMLoader をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                DMLoader = null;
                // UPGRADE_NOTE: オブジェクト DMPerformance をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                DMPerformance = null;
                // UPGRADE_NOTE: オブジェクト DMSegment をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                DMSegment = null;
            }

            // DirectSoundの解放
            if (UseDirectSound)
            {
                // オブジェクトの解放
                // UPGRADE_NOTE: オブジェクト DSObject をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                DSObject = null;
                // UPGRADE_NOTE: オブジェクト DSBuffer をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                DSBuffer = null;
            }

            // DirectXの解放
            // UPGRADE_NOTE: オブジェクト DXObject をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            DXObject = null;

            // VBMP3.DLLの解放
            if (IsMP3Supported)
            {
                VBMP3.vbmp3_stop();
                VBMP3.vbmp3_free();
            }
        }
    }
}