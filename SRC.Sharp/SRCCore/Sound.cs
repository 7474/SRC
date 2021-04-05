// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Filesystem;
using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.IO;
using System.Linq;

namespace SRCCore
{
    // ＢＧＭ＆効果音再生用のモジュール
    public class Sound : IDisposable
    {
        public IPlaySound Player { get; set; }

        private SRC SRC { get; }
        private IFileSystem FileSystem => SRC.FileSystem;

        public Sound(SRC src)
        {
            SRC = src;
        }

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

        private bool disposedValue;

        private const int CH_BGM = IPlaySound.CH_BGM;
        private const int CH_EFFECT = 2;

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
            fname0 = Path.GetFileNameWithoutExtension(bgm_name);

            // 同じＢＧＭを演奏中であれば演奏を継続
            if (Strings.Len(BGMFileName) > 0)
            {
                if (Strings.InStr(BGMFileName, @"\" + fname0 + ".") > 0)
                {
                    if (Player.BGMStatus == BGMStatus.Playing)
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

            // TODO Impl
            //// 同じＢＧＭにバリエーションがあればランダムで選択
            //i = 1;
            //if (Strings.InStr(fname, SRC.ScenarioPath) > 0)
            //{
            //    // シナリオ側にファイルが見つかった場合はバリエーションもシナリオ側からのみ選択
            //    do
            //    {
            //        i = (i + 1);
            //        fname2 = SearchMidiFile("(" + fname0 + "(" + SrcFormatter.Format(i) + ")" + Strings.Right(fname, 4) + ")");
            //    }
            //    while (Strings.InStr(fname2, SRC.ScenarioPath) > 0);
            //}
            //else
            //{
            //    // そうでなければ両方から選択
            //    do
            //    {
            //        i = (i + 1);
            //        fname2 = SearchMidiFile("(" + fname0 + "(" + SrcFormatter.Format(i) + ")" + Strings.Right(fname, 4) + ")");
            //    }
            //    while (!string.IsNullOrEmpty(fname2));
            //}

            //i = Conversion.Int((i - 1) * VBMath.Rnd() + 1f);
            //if (i > 1)
            //{
            //    fname = SearchMidiFile("(" + fname0 + "(" + SrcFormatter.Format(i) + ")" + Strings.Right(fname, 4) + ")");
            //}

            // ＢＧＭを連続演奏？
            RepeatMode = is_repeat_mode;

            // ファイルをロードし、演奏開始
            Player.Play(CH_BGM, fname, is_repeat_mode ? PlaySoundMode.Repeat : PlaySoundMode.None);
            // 演奏しているBGMのファイル名を記録
            BGMFileName = fname;
        }

        // ＢＧＭをリスタートさせる
        public void RestartBGM()
        {
            // 停止中でなければ何もしない
            if (Player.BGMStatus != BGMStatus.Stopped)
            {
                return;
            }
            // リスタート
            // XXX 止まってるってことはリピートしてないってことだろう。
            Player.Play(CH_BGM, BGMFileName, RepeatMode ? PlaySoundMode.Repeat : PlaySoundMode.None);
        }

        // ＢＧＭを停止する
        public void StopBGM(bool by_force = false)
        {
            // ＢＧＭを固定中？
            if (!by_force && KeepBGM)
            {
                return;
            }

            // 強制的に停止するのでなければ演奏中でない限りなにもしない
            if (!by_force && Strings.Len(BGMFileName) == 0)
            {
                return;
            }

            try
            {
                Player.Stop(CH_BGM);
            }
            finally
            {
                BGMFileName = "";
                RepeatMode = false;
            }
        }

        // ＢＧＭを変更する (指定したＢＧＭをすでに演奏中ならなにもしない)
        public void ChangeBGM(string bgm_name)
        {
            string fname, fname2;

            // ＢＧＭ固定中？
            if (KeepBGM || BossBGM)
            {
                return;
            }

            // XXX StartBGM でやればよくない？
            //// 正しいファイル名？
            //if (Strings.Len(bgm_name) < 5)
            //{
            //    return;
            //}

            //// ファイル名の本体部分を抜き出す
            //fname = Strings.Left(bgm_name, Strings.Len(bgm_name) - 4);
            //if (GeneralLib.InStr2(fname, @"\") > 0)
            //{
            //    fname = Strings.Mid(fname, GeneralLib.InStr2(fname, @"\") + 1);
            //}

            //// 既に同じMIDIが演奏されていればそのまま演奏し続ける
            //if (Strings.Len(BGMFileName) > 0)
            //{
            //    if (Strings.InStr(BGMFileName, @"\" + fname + ".") > 0)
            //    {
            //        return;
            //    }
            //}

            //// 番号違い？
            //if (Strings.Len(BGMFileName) > 5)
            //{
            //    fname2 = Strings.Left(BGMFileName, Strings.Len(BGMFileName) - 4);
            //    if (GeneralLib.InStr2(fname2, @"\") > 0)
            //    {
            //        fname2 = Strings.Mid(fname2, GeneralLib.InStr2(fname2, @"\") + 1);
            //    }

            //    if (Strings.Len(fname2) > 4)
            //    {
            //        switch (Strings.Right(fname2, 3) ?? "")
            //        {
            //            case "(2)":
            //            case "(3)":
            //            case "(4)":
            //            case "(5)":
            //            case "(6)":
            //            case "(7)":
            //            case "(8)":
            //            case "(9)":
            //                {
            //                    fname2 = Strings.Left(fname2, Strings.Len(fname2) - 3);
            //                    break;
            //                }
            //        }
            //    }

            //    if ((fname ?? "") == (fname2 ?? ""))
            //    {
            //        return;
            //    }
            //}

            // XXX なんで？
            // 繰り返し演奏に設定
            RepeatMode = true;

            // 演奏開始
            StartBGM(bgm_name);
        }

        // 各Midiフォルダから指定されたMIDIファイルを検索する
        public string SearchMidiFile(string midi_name)
        {
            // TODO FileSystemに逃がす
            var baseDirs = new string[]
            {
                SRC.ScenarioPath,
                SRC.ExtDataPath,
                SRC.ExtDataPath2,
                SRC.AppPath,
            }.Where(x => Directory.Exists(x))
                .Select(x => SRC.FileSystem.PathCombine(x, "Midi"))
                .Where(x => Directory.Exists(x))
                .ToList();

            var midiNames = GeneralLib.ToList(midi_name, true);
            var existFile = midiNames
                .SelectMany(x => baseDirs.Select(y => SRC.FileSystem.PathCombine(y, x)))
                .FirstOrDefault(x => File.Exists(x));

            return existFile;

            // TODO Impl
            //string SearchMidiFileRet = default;
            //string fname, fname_mp3 = default;
            //;
            ///* Cannot convert LocalDeclarationStatementSyntax, System.NotSupportedException: Keyword not supported!
            //         scenario_midi_dir_exists As Boolean
            //         extdata_midi_dir_exists As Boolean
            //         extdata2_midi_dir_exists As Boolean
            // */
            //;
            ////  fpath_history As New Collection     DEL MARGE
            //int j, i, num;
            //string buf, buf2;
            //var sub_folder = default(string);

            //// 初めて実行する際に、各フォルダにMidiフォルダがあるかチェック
            //if (!IsMidiSearchPathInitialized)
            //{
            //    if (Strings.Len(SRC.ScenarioPath) > 0)
            //    {
            //        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //        if (Strings.Len(FileSystem.Dir(SRC.ScenarioPath + "Midi", FileAttribute.Directory)) > 0)
            //        {
            //            scenario_midi_dir_exists = true;
            //        }
            //    }

            //    if (Strings.Len(SRC.ExtDataPath) > 0)
            //    {
            //        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //        if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath + "Midi", FileAttribute.Directory)) > 0)
            //        {
            //            extdata_midi_dir_exists = true;
            //        }
            //    }

            //    if (Strings.Len(SRC.ExtDataPath2) > 0)
            //    {
            //        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //        if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath2 + "Midi", FileAttribute.Directory)) > 0)
            //        {
            //            extdata2_midi_dir_exists = true;
            //        }
            //    }

            //    // MP3が演奏可能かどうかも調べておく
            //    if (GeneralLib.FileExists(SRC.AppPath + "VBMP3.dll"))
            //    {
            //        is_mp3_available = true;
            //    }

            //    IsMidiSearchPathInitialized = true;
            //}

            //// ダミーのファイル名？
            //if (Strings.Len(midi_name) < 5)
            //{
            //    return SearchMidiFileRet;
            //}

            //// 引数1として渡された文字列をリストとして扱い、左から順にMIDIを検索
            //num = GeneralLib.ListLength(midi_name);
            //i = 1;
            //while (i <= num)
            //{
            //    // スペースを含むファイル名への対応
            //    buf = "";
            //    var loopTo = num;
            //    for (j = i; j <= loopTo; j++)
            //    {
            //        buf2 = Strings.LCase(GeneralLib.ListIndex(midi_name, j));

            //        // 全体が()で囲まれている場合は()を外す
            //        if (Strings.Left(buf2, 1) == "(" & Strings.Right(buf2, 1) == ")")
            //        {
            //            buf2 = Strings.Mid(buf2, 2, Strings.Len(buf2) - 2);
            //        }

            //        buf = buf + " " + buf2;
            //        if (Strings.Right(buf, 4) == ".mid")
            //        {
            //            break;
            //        }
            //    }

            //    buf = Strings.Trim(buf);

            //    // 同名のMP3ファイルがある場合はMIDIファイルの代わりにMP3ファイルを使う
            //    if (is_mp3_available)
            //    {
            //        fname_mp3 = Strings.Left(buf, Strings.Len(buf) - 4) + ".mp3";
            //    }

            //    // フルパスでの指定？
            //    if (Strings.InStr(buf, ":") == 2)
            //    {
            //        if (is_mp3_available)
            //        {
            //            if (GeneralLib.FileExists(fname_mp3))
            //            {
            //                SearchMidiFileRet = fname_mp3;
            //                return SearchMidiFileRet;
            //            }
            //        }

            //        if (GeneralLib.FileExists(buf))
            //        {
            //            SearchMidiFileRet = buf;
            //        }

            //        return SearchMidiFileRet;
            //    }

            //    // DEL START MARGE
            //    // '履歴を検索してみる
            //    // On Error GoTo NotFound
            //    // fname = fpath_history.Item(buf)
            //    // 
            //    // '履歴上にファイルを発見
            //    // SearchMidiFile = fname
            //    // Exit Function

            //    // NotFound:
            //    // '履歴になかった
            //    // On Error GoTo 0
            //    // DEL END MARGE

            //    // サブフォルダ指定あり？
            //    if (Strings.InStr(buf, "_") > 0)
            //    {
            //        sub_folder = Strings.Left(buf, Strings.InStr(buf, "_") - 1) + @"\";
            //    }

            //    // シナリオ側のMidiフォルダ
            //    if (scenario_midi_dir_exists)
            //    {
            //        if (is_mp3_available)
            //        {
            //            if (!string.IsNullOrEmpty(sub_folder))
            //            {
            //                fname = SRC.ScenarioPath + @"Midi\" + sub_folder + fname_mp3;
            //                if (GeneralLib.FileExists(fname))
            //                {
            //                    SearchMidiFileRet = fname;
            //                    // fpath_history.Add fname, buf   DEL MARGE
            //                    return SearchMidiFileRet;
            //                }
            //            }

            //            fname = SRC.ScenarioPath + @"Midi\" + fname_mp3;
            //            if (GeneralLib.FileExists(fname))
            //            {
            //                SearchMidiFileRet = fname;
            //                // fpath_history.Add fname, buf   DEL MARGE
            //                return SearchMidiFileRet;
            //            }
            //        }

            //        if (!string.IsNullOrEmpty(sub_folder))
            //        {
            //            fname = SRC.ScenarioPath + @"Midi\" + sub_folder + buf;
            //            if (GeneralLib.FileExists(fname))
            //            {
            //                SearchMidiFileRet = fname;
            //                // fpath_history.Add fname, buf   DEL MARGE
            //                return SearchMidiFileRet;
            //            }
            //        }

            //        fname = SRC.ScenarioPath + @"Midi\" + buf;
            //        if (GeneralLib.FileExists(fname))
            //        {
            //            SearchMidiFileRet = fname;
            //            // fpath_history.Add fname, buf   DEL MARGE
            //            return SearchMidiFileRet;
            //        }
            //    }

            //    // ExtDataPath側のMidiフォルダ
            //    if (extdata_midi_dir_exists)
            //    {
            //        if (is_mp3_available)
            //        {
            //            if (!string.IsNullOrEmpty(sub_folder))
            //            {
            //                fname = SRC.ExtDataPath + @"Midi\" + sub_folder + fname_mp3;
            //                if (GeneralLib.FileExists(fname))
            //                {
            //                    SearchMidiFileRet = fname;
            //                    // fpath_history.Add fname, buf   DEL MARGE
            //                    return SearchMidiFileRet;
            //                }
            //            }

            //            fname = SRC.ExtDataPath + @"Midi\" + fname_mp3;
            //            if (GeneralLib.FileExists(fname))
            //            {
            //                SearchMidiFileRet = fname;
            //                // fpath_history.Add fname, buf   DEL MARGE
            //                return SearchMidiFileRet;
            //            }
            //        }

            //        if (!string.IsNullOrEmpty(sub_folder))
            //        {
            //            fname = SRC.ExtDataPath + @"Midi\" + sub_folder + buf;
            //            if (GeneralLib.FileExists(fname))
            //            {
            //                SearchMidiFileRet = fname;
            //                // fpath_history.Add fname, buf   DEL MARGE
            //                return SearchMidiFileRet;
            //            }
            //        }

            //        fname = SRC.ExtDataPath + @"Midi\" + buf;
            //        if (GeneralLib.FileExists(fname))
            //        {
            //            SearchMidiFileRet = fname;
            //            // fpath_history.Add fname, buf   DEL MARGE
            //            return SearchMidiFileRet;
            //        }
            //    }

            //    // ExtDataPath2側のMidiフォルダ
            //    if (extdata2_midi_dir_exists)
            //    {
            //        if (is_mp3_available)
            //        {
            //            if (!string.IsNullOrEmpty(sub_folder))
            //            {
            //                fname = SRC.ExtDataPath2 + @"Midi\" + sub_folder + fname_mp3;
            //                if (GeneralLib.FileExists(fname))
            //                {
            //                    SearchMidiFileRet = fname;
            //                    // fpath_history.Add fname, buf   DEL MARGE
            //                    return SearchMidiFileRet;
            //                }
            //            }

            //            fname = SRC.ExtDataPath2 + @"Midi\" + fname_mp3;
            //            if (GeneralLib.FileExists(fname))
            //            {
            //                SearchMidiFileRet = fname;
            //                // fpath_history.Add fname, buf   DEL MARGE
            //                return SearchMidiFileRet;
            //            }
            //        }

            //        if (!string.IsNullOrEmpty(sub_folder))
            //        {
            //            fname = SRC.ExtDataPath2 + @"Midi\" + sub_folder + buf;
            //            if (GeneralLib.FileExists(fname))
            //            {
            //                SearchMidiFileRet = fname;
            //                // fpath_history.Add fname, buf   DEL MARGE
            //                return SearchMidiFileRet;
            //            }
            //        }

            //        fname = SRC.ExtDataPath2 + @"Midi\" + buf;
            //        if (GeneralLib.FileExists(fname))
            //        {
            //            SearchMidiFileRet = fname;
            //            // fpath_history.Add fname, buf   DEL MARGE
            //            return SearchMidiFileRet;
            //        }
            //    }

            //    // 本体側のMidiフォルダ
            //    if (is_mp3_available)
            //    {
            //        if (!string.IsNullOrEmpty(sub_folder))
            //        {
            //            fname = SRC.AppPath + @"Midi\" + sub_folder + fname_mp3;
            //            if (GeneralLib.FileExists(fname))
            //            {
            //                SearchMidiFileRet = fname;
            //                // fpath_history.Add fname, buf   DEL MARGE
            //                return SearchMidiFileRet;
            //            }
            //        }

            //        fname = SRC.AppPath + @"Midi\" + fname_mp3;
            //        if (GeneralLib.FileExists(fname))
            //        {
            //            SearchMidiFileRet = fname;
            //            // fpath_history.Add fname, buf   DEL MARGE
            //            return SearchMidiFileRet;
            //        }
            //    }

            //    if (!string.IsNullOrEmpty(sub_folder))
            //    {
            //        fname = SRC.AppPath + @"Midi\" + sub_folder + buf;
            //        if (GeneralLib.FileExists(fname))
            //        {
            //            SearchMidiFileRet = fname;
            //            // fpath_history.Add fname, buf   DEL MARGE
            //            return SearchMidiFileRet;
            //        }
            //    }

            //    fname = SRC.AppPath + @"Midi\" + buf;
            //    if (GeneralLib.FileExists(fname))
            //    {
            //        SearchMidiFileRet = fname;
            //        // fpath_history.Add fname, buf   DEL MARGE
            //        return SearchMidiFileRet;
            //    }

            //    i = (j + 1);
            //}

            //return SearchMidiFileRet;
        }

        // MIDIファイルのサーチパスをリセットする
        public void ResetMidiSearchPath()
        {
            IsMidiSearchPathInitialized = false;
        }

        // ＢＧＭに割り当てられたMIDIファイル名を返す
        public string BGMName(string bgm_name)
        {
            return bgm_name + ".mid";
            // TODO Impl
            //string BGMNameRet = default;
            //string vname;

            //// RenameBGMコマンドでMIDIファイルが設定されていればそちらを使用
            //vname = "BGM(" + bgm_name + ")";
            //if (Expression.IsGlobalVariableDefined(vname))
            //{
            //    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //    BGMNameRet = Conversions.ToString(Event_Renamed.GlobalVariableList[vname].StringValue);
            //    return BGMNameRet;
            //}

            //// そうでなければSrc.iniで設定されているファイルを使用
            //BGMNameRet = GeneralLib.ReadIni("BGM", bgm_name);

            //// Src.iniでも設定されていなければ標準のファイルを使用
            //if (string.IsNullOrEmpty(BGMNameRet))
            //{
            //    BGMNameRet = bgm_name + ".mid";
            //}

            //return BGMNameRet;
        }

        // Waveファイルを再生する
        public void PlayWave(string wave_name)
        {

            // TODO Impl
            //// 初めて実行する際に、各フォルダにSoundフォルダがあるかチェック
            /* Cannot convert LocalDeclarationStatementSyntax, System.NotSupportedException: Keyword not supported!
                     init_play_wave As Boolean
                     scenario_sound_dir_exists As Boolean
                     extdata_sound_dir_exists As Boolean
             */

            // 特殊なファイル名
            switch (Strings.LCase(wave_name) ?? "")
            {
                case "-.wav":
                case "-.mp3":
                    // 再生をキャンセル
                    return;

                case "null.wav":
                case "null.mp3":
                    // WAVE再生を停止
                    // MP3再生を停止
                    StopWave();
                    return;
            }

            // 各フォルダをチェック
            string fname;

            // TODO FileSystemに逃がす
            var baseDirs = new string[]
            {
                SRC.ScenarioPath,
                SRC.ExtDataPath,
                SRC.ExtDataPath2,
                SRC.AppPath,
            }.Where(x => Directory.Exists(x))
                .Select(x => SRC.FileSystem.PathCombine(x, "Sound"))
                .Where(x => Directory.Exists(x))
                .ToList();

            var waveNames = GeneralLib.ToList(wave_name, true);
            var existFile = waveNames
                .SelectMany(x => baseDirs.Select(y => SRC.FileSystem.PathCombine(y, x)))
                .FirstOrDefault(x => File.Exists(x));

            if (string.IsNullOrEmpty(existFile))
            {
                // 絶対表記？
                if (!GeneralLib.FileExists(wave_name))
                {
                    // 見つからなかった
                    return;
                }
                existFile = wave_name;
            }

            Player.Play(CH_EFFECT, existFile, PlaySoundMode.None);

            // 効果音再生のフラグを立てる
            IsWavePlayed = true;
        }

        // Waveファイルの再生を終了する
        public void StopWave()
        {
            Player.Stop(CH_EFFECT);
        }

        // 本モジュールの解放処理を行う
        public void FreeSoundModule()
        {
            try
            {
                // BGM演奏の停止
                KeepBGM = false;
                BossBGM = false;
                StopBGM(true);

                //// 音源初期化
                //ResetBGM();

                // WAVEファイル再生の停止
                StopWave();
            }
            catch
            {
                // ignore
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    FreeSoundModule();

                    // マネージド状態を破棄します (マネージド オブジェクト)
                    Player?.Dispose();
                }

                // アンマネージド リソース (アンマネージド オブジェクト) を解放し、ファイナライザーをオーバーライドします
                // 大きなフィールドを null に設定します
                disposedValue = true;
            }
        }

        // // 'Dispose(bool disposing)' にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします
        // ~Sound()
        // {
        //     // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
