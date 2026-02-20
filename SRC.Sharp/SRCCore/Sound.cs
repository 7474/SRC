// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Extensions;
using SRCCore.Filesystem;
using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Collections.Generic;
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

        private const int CH_BGM = PlaySoundConstants.CH_BGM;
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

            // TODO 同じＢＧＭにバリエーションがあればランダムで選択 のパス解決
            // 同じＢＧＭにバリエーションがあればランダムで選択
            i = 1;
            if (!string.IsNullOrEmpty(SRC.ScenarioPath) && Strings.InStr(fname, SRC.ScenarioPath) > 0)
            {
                // シナリオ側にファイルが見つかった場合はバリエーションもシナリオ側からのみ選択
                do
                {
                    i = (i + 1);
                    fname2 = SearchMidiFile("(" + fname0 + "(" + SrcFormatter.Format(i) + ")" + Strings.Right(fname, 4) + ")");
                }
                while (Strings.InStr(fname2, SRC.ScenarioPath) > 0);
            }
            else
            {
                // そうでなければ両方から選択
                do
                {
                    i = (i + 1);
                    fname2 = SearchMidiFile("(" + fname0 + "(" + SrcFormatter.Format(i) + ")" + Strings.Right(fname, 4) + ")");
                }
                while (!string.IsNullOrEmpty(fname2));
            }

            i = (int)(i * SRC.Rand());
            if (i > 1)
            {
                fname = SearchMidiFile("(" + fname0 + "(" + SrcFormatter.Format(i) + ")" + Strings.Right(fname, 4) + ")");
            }

            // ＢＧＭを連続演奏？
            RepeatMode = is_repeat_mode;

            // ファイルをロードし、演奏開始
            // ライブラリが再生時に get_Length してエラーする対応などとして複製したストリームを渡す
            Player.Play(CH_BGM, FileSystem.Open(fname).ToMemoryStream(), Player.ResolveSoundType(fname), is_repeat_mode ? PlaySoundMode.Repeat : PlaySoundMode.None);
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
            // ライブラリが再生時に get_Length してエラーする対応などとして複製したストリームを渡す
            // XXX 止まってるってことはリピートしてないってことだろう。
            Player.Play(CH_BGM, FileSystem.Open(BGMFileName).ToMemoryStream(), Player.ResolveSoundType(BGMFileName), RepeatMode ? PlaySoundMode.Repeat : PlaySoundMode.None);
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

            // 正しいファイル名？
            if (Strings.Len(bgm_name) < 5)
            {
                return;
            }

            // ファイル名の本体部分を抜き出す
            fname = Strings.Left(bgm_name, Strings.Len(bgm_name) - 4);
            if (GeneralLib.InStr2(fname, @"\") > 0)
            {
                fname = Strings.Mid(fname, GeneralLib.InStr2(fname, @"\") + 1);
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
                if (GeneralLib.InStr2(fname2, @"\") > 0)
                {
                    fname2 = Strings.Mid(fname2, GeneralLib.InStr2(fname2, @"\") + 1);
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

            // XXX なんで？
            // 繰り返し演奏に設定
            RepeatMode = true;

            // 演奏開始
            StartBGM(bgm_name);
        }

        // 各Midiフォルダから指定されたMIDIファイルを検索する
        public string SearchMidiFile(string midi_name)
        {
            // ダミーのファイル名？
            if (Strings.Len(midi_name) < 5)
            {
                return null;
            }

            // 引数1として渡された文字列をリストとして扱い、左から順にMIDIを検索
            var midiNames = GeneralLib.ToList(midi_name);
            var i = 0;
            while (i < midiNames.Count)
            {
                // スペースを含むファイル名への対応
                var buf = "";
                for (var j = i; j < midiNames.Count; j++)
                {
                    var buf2 = midiNames[j];

                    // 全体が()で囲まれている場合は()を外す
                    if (Strings.Left(buf2, 1) == "(" && Strings.Right(buf2, 1) == ")")
                    {
                        buf2 = Strings.Mid(buf2, 2, Strings.Len(buf2) - 2);
                    }

                    buf = buf + " " + buf2;
                    if (buf.ToLower().EndsWith(".mid"))
                    {
                        i = j + 1;
                        break;
                    }
                }

                buf = Strings.Trim(buf);
                // 同名のMP3ファイルがある場合はMIDIファイルの代わりにMP3ファイルを使う
                var fname = buf;
                var fnameMp3 = Strings.Left(buf, Strings.Len(buf) - 4) + ".mp3";
                var fnames = new List<string>();
                if (FileSystem.IsAbsolutePath(fname))
                {
                    fnames.Add(fnameMp3);
                    fnames.Add(fname);
                }
                else
                {
                    // TODO 一度検索したものを再検索している感じ
                    // なので Midi ディレクトリ無し指定もしてるがイマイチ、絶対パスで返すAPI作る？
                    fnames.Add(FileSystem.PathCombine("Midi", fnameMp3));
                    fnames.Add(FileSystem.PathCombine("Midi", fname));
                    fnames.Add(fnameMp3);
                    fnames.Add(fname);
                }
                //// XXX これ何に作用してるの？
                //// サブフォルダ指定あり？
                //if (Strings.InStr(buf, "_") > 0)
                //{
                //    sub_folder = Strings.Left(buf, Strings.InStr(buf, "_") - 1) + @"\";
                //}
                var existFileName = fnames.FirstOrDefault(x => FileSystem.FileExists(x));

                if (!string.IsNullOrEmpty(existFileName))
                {
                    return existFileName;
                }
            }
            return null;
        }

        // MIDIファイルのサーチパスをリセットする
        public void ResetMidiSearchPath()
        {
            IsMidiSearchPathInitialized = false;
        }

        // ＢＧＭに割り当てられたMIDIファイル名を返す
        public string BGMName(string bgm_name)
        {
            // RenameBGMコマンドでMIDIファイルが設定されていればそちらを使用
            var vname = "BGM(" + bgm_name + ")";
            if (SRC.Expression.IsGlobalVariableDefined(vname))
            {
                return SRC.Event.GlobalVariableList[vname].StringValue;
            }

            // そうでなければSrc.iniで設定されているファイルを使用
            var configName = SRC.SystemConfig.GetItem("BGM", bgm_name);

            // Src.iniでも設定されていなければ標準のファイルを使用
            if (string.IsNullOrEmpty(configName))
            {
                return bgm_name + ".mid";
            }

            return configName;
        }

        // Waveファイルを再生する
        public void PlayWave(string wave_name)
        {
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

            var baseDirs = new string[]
            {
                SRC.ScenarioPath,
                SRC.ExtDataPath,
                SRC.ExtDataPath2,
                SRC.AppPath,
            }
                .Where(x => !string.IsNullOrEmpty(x) && Directory.Exists(x))
                .Select(x => SRC.FileSystem.PathCombine(x, "Sound"))
                .Where(x => Directory.Exists(x))
                .ToList();

            // XXX なんで wave_name 分解したんだろう？
            //var waveNames = GeneralLib.ToList(wave_name, true);
            //var existFile = waveNames
            //    .SelectMany(x => baseDirs.Select(y => SRC.FileSystem.PathCombine(y, x)))
            //    .FirstOrDefault(x => SRC.FileSystem.FileExists(x));
            var existFile = baseDirs.Select(y => SRC.FileSystem.PathCombine(y, wave_name))
                .FirstOrDefault(x => SRC.FileSystem.FileExists(x));

            if (string.IsNullOrEmpty(existFile))
            {
                // 絶対表記？
                if (!SRC.FileSystem.FileExists(wave_name))
                {
                    // 見つからなかった
                    return;
                }
                existFile = wave_name;
            }

            // ライブラリが再生時に get_Length してエラーする対応などとして複製したストリームを渡す
            Player.Play(CH_EFFECT, FileSystem.Open(existFile).ToMemoryStream(), Player.ResolveSoundType(existFile), PlaySoundMode.None);

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
