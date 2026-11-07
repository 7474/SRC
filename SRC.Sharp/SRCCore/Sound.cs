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
            fname0 = BGMBodyName(bgm_name);

            // 同じＢＧＭを演奏中であれば演奏を継続
            if (Strings.Len(BGMFileName) > 0)
            {
                if (IsBGMFileNameOf(fname0))
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
                // 素材の置き場所を確かめられるよう、見つからなかった名前を残す
                SRC.LogInfo("BGM {BGM} が見つかりません（シナリオ・拡張データフォルダ・SRC# のフォルダの Midi フォルダを探しました）", bgm_name);
                return;
            }

            // 演奏をストップ
            StopBGM();

            // 同じＢＧＭにバリエーションがあればランダムで選択
            i = 1;
            if (IsFileInScenarioPath(fname))
            {
                // シナリオ側にファイルが見つかった場合はバリエーションもシナリオ側からのみ選択
                do
                {
                    i = (i + 1);
                    fname2 = SearchMidiFile("(" + fname0 + "(" + SrcFormatter.Format(i) + ")" + Strings.Right(fname, 4) + ")");
                }
                while (IsFileInScenarioPath(fname2));
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

            // VB: Sound.bas StartBGM。1 ～ (見つかったバリエーションの数) から一様に選ぶ
            i = VBMath.Int(((i - 1) * SRC.Rand()) + 1);
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
            // （元の SRC の BGMStatus は演奏中のファイルが無ければ空文字列を返すので、そのときも何もしない）
            if (Strings.Len(BGMFileName) == 0 || Player.BGMStatus != BGMStatus.Stopped)
            {
                return;
            }
            // リスタート
            // 元の SRC はタイマー（Timer1）から RepeatMode のときに呼んで繰り返し演奏する。
            // SRC# の Player は PlaySoundMode.Repeat で繰り返すので、SRCCore からは呼んでいない
            // ライブラリが再生時に get_Length してエラーする対応などとして複製したストリームを渡す
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
            fname = BGMBodyName(bgm_name);

            // 既に同じMIDIが演奏されていればそのまま演奏し続ける
            if (Strings.Len(BGMFileName) > 0)
            {
                if (IsBGMFileNameOf(fname))
                {
                    return;
                }
            }

            // 番号違い？
            if (Strings.Len(BGMFileName) > 5)
            {
                fname2 = BGMBodyName(BGMFileName);

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
            // （元の SRC と同じ。続く StartBGM が is_repeat_mode の既定値 true で設定し直すので結果は変わらない）
            RepeatMode = true;

            // 演奏開始
            StartBGM(bgm_name);
        }

        // ファイル名の本体部分（末尾の 4 文字（拡張子）を除き、最後の区切りより後ろ）を返す。
        // 元の SRC は \ で区切る。SRC# のパスは / で区切ることもあるので、どちらも区切りとみなす
        private static string BGMBodyName(string fname)
        {
            var body = Strings.Left(fname, Strings.Len(fname) - 4);
            var idx = GeneralLib.InStr2(body.Replace('/', '\\'), @"\");
            if (idx > 0)
            {
                body = Strings.Mid(body, idx + 1);
            }
            return body;
        }

        // 演奏中のファイル（BGMFileName）が、本体部分が body のファイルか。
        // 元の SRC の InStr(BGMFileName, "\" & body & ".") > 0。/ の区切りも \ とみなし、
        // フォルダの付いていない BGMFileName（SRC# で読み込み対象のフォルダの直下で見つけたとき）も同じに扱う
        private bool IsBGMFileNameOf(string body)
        {
            return Strings.InStr(@"\" + (BGMFileName ?? "").Replace('/', '\\'), @"\" + body + ".") > 0;
        }

        // 指定されたファイルがシナリオパス内に存在するかを確認する
        private bool IsFileInScenarioPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(SRC.ScenarioPath))
            {
                return false;
            }
            // 絶対パスの場合はシナリオパスが含まれるかで判定
            if (FileSystem.IsAbsolutePath(filePath))
            {
                return Strings.InStr(filePath, SRC.ScenarioPath) > 0;
            }
            // 相対パスの場合はシナリオパス配下にファイルが存在するかで判定
            return FileSystem.FileExists(FileSystem.PathCombine(SRC.ScenarioPath, filePath));
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
            // （元の SRC と同じく、括弧・クォートの対応が取れていなければ ListLength が -1 なので探さない）
            var num = GeneralLib.ListLength(midi_name);
            var i = 1;
            while (i <= num)
            {
                // スペースを含むファイル名への対応
                // 元の SRC は名前を小文字にして探す（Windows はファイル名の大文字・小文字を区別しない）。
                // SRC# は大文字・小文字を区別するファイルシステムでも見つけられるよう、名前をそのまま使う
                var buf = "";
                int j;
                for (j = i; j <= num; j++)
                {
                    var buf2 = GeneralLib.ListIndex(midi_name, j);

                    // 全体が()で囲まれている場合は()を外す
                    if (Strings.Left(buf2, 1) == "(" && Strings.Right(buf2, 1) == ")")
                    {
                        buf2 = Strings.Mid(buf2, 2, Strings.Len(buf2) - 2);
                    }

                    buf = buf + " " + buf2;
                    if (buf.ToLower().EndsWith(".mid"))
                    {
                        break;
                    }
                }
                // 次の候補へ（.mid で終わらない指定（.mp3 など）は残り全体を1つの名前として扱い、検索を終える）
                i = j + 1;

                buf = Strings.Trim(buf);
                // 同名のMP3ファイルがある場合はMIDIファイルの代わりにMP3ファイルを使う
                var fname = buf;
                var fnameMp3 = Strings.Left(buf, Strings.Len(buf) - 4) + ".mp3";
                var fnames = new List<string>();
                if (FileSystem.IsAbsolutePath(fname))
                {
                    // フルパスでの指定。元の SRC と同じく、見つからなければ残りの候補は探さずに終える
                    return new[] { fnameMp3, fname }.FirstOrDefault(x => FileSystem.FileExists(x));
                }
                else
                {
                    // サブフォルダ指定あり？（「作品名_曲名.mid」なら Midi\作品名\ も探す）
                    // 元の SRC は前の候補のサブフォルダを次の候補に持ち越すが、候補ごとに決め直す
                    var subFolder = Strings.InStr(buf, "_") > 0 ? Strings.Left(buf, Strings.InStr(buf, "_") - 1) : "";

                    // 元の SRC と同じく、シナリオ → ExtDataPath → ExtDataPath2 → SRC のフォルダの順に、
                    // フォルダごとに サブフォルダの MP3 → 直下の MP3 → サブフォルダの MIDI → 直下の MIDI の順に探す。
                    // SRC のフォルダには素材パック（Packs の zip）や SRC#Web で追加した素材も重なっている
                    var midiDirs = new string[]
                    {
                        SRC.ScenarioPath,
                        SRC.ExtDataPath,
                        SRC.ExtDataPath2,
                        SRC.AppPath,
                    }
                        .Where(x => !string.IsNullOrEmpty(x))
                        .Distinct()
                        // フォルダが実在するかは見ない（素材パックの zip の中の Midi も探すため）
                        .Select(x => FileSystem.PathCombine(x, "Midi"))
                        // SRC# 独自: 読み込み対象に加えたフォルダ（相対パス）の Midi フォルダも探す
                        .Concat(new[] { "Midi" });
                    foreach (var midiDir in midiDirs)
                    {
                        if (subFolder != "")
                        {
                            fnames.Add(FileSystem.PathCombine(midiDir, subFolder, fnameMp3));
                        }
                        fnames.Add(FileSystem.PathCombine(midiDir, fnameMp3));
                        if (subFolder != "")
                        {
                            fnames.Add(FileSystem.PathCombine(midiDir, subFolder, fname));
                        }
                        fnames.Add(FileSystem.PathCombine(midiDir, fname));
                    }
                    // SRC# 独自: Midi フォルダに無ければ、読み込み対象に加えたフォルダの直下も探す
                    fnames.Add(fnameMp3);
                    fnames.Add(fname);
                }
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
            var configName = SRC.Config.GetItem("BGM", bgm_name);

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
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                // フォルダが実在するかは見ない（素材パックの zip の中の Sound も探すため。FileExists が実在するファイルと zip の両方を見る）
                .Select(x => SRC.FileSystem.PathCombine(x, "Sound"))
                .ToList();

            // 元の SRC と同じく、各フォルダの Sound フォルダに wave_name（サブフォルダを含んでよい）を付けて探す
            var existFile = baseDirs.Select(y => SRC.FileSystem.PathCombine(y, wave_name))
                .FirstOrDefault(x => SRC.FileSystem.FileExists(x));

            if (string.IsNullOrEmpty(existFile))
            {
                // 絶対表記？
                if (!SRC.FileSystem.FileExists(wave_name))
                {
                    // 見つからなかった。素材の置き場所を確かめられるよう、名前を残す
                    SRC.LogInfo("効果音 {Wave} が見つかりません（シナリオ・拡張データフォルダ・SRC# のフォルダの Sound フォルダを探しました）", wave_name);
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

        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを 'Dispose(bool disposing)' メソッドに記述します
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
