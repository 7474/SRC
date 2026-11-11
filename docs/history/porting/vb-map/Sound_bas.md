# Sound.bas

VB 版の `SRC/SRC_20121125/Sound.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub StartBGM | 69 | 71 | `SRCCore/Sound.cs` Sound.StartBGM | 修正済み | ファイル名の本体部分の取り出しと、演奏中の曲との比較（InStr(BGMFileName, "\" & fname0 & ".")）で、/ も \ とみなすようにした（BGMBodyName・IsBGMFileNameOf）。BGMFileName がフォルダの付かない名前のこともあるので、先頭に \ を補って比べる（#1498）。VB の SearchMidiFile は名前を小文字にして返すため、大文字を含む曲名は同じ曲と判定できず最初から演奏し直すが、VB の不具合とみて合わせない |
| Sub LoadBGM | 142 | 139 | `SRCCore/Sound.cs` Sound.StartBGM | SRC#で不要 | DirectMusic・MCI・VBMP3 の操作は IPlaySound が受け持つ。繰り返すかどうかは PlaySoundMode で渡す |
| Sub RestartBGM | 283 | 35 | `SRCCore/Sound.cs` Sound.RestartBGM | 修正済み | 演奏中のファイルが無いときは何もしない（VB の BGMStatus は空文字列を返すので "stopped" にならない）。直す前は例外になっていた（#1498）。VB は Timer1 から RepeatMode のときに呼ぶ。SRC# は Player が繰り返すので、SRCCore からの呼び出しは無い |
| Sub StopBGM | 320 | 49 | `SRCCore/Sound.cs` Sound.StopBGM | 一致 | by_force と KeepBGM の分岐、BGMFileName と RepeatMode の初期化が同じ。止め方とタイマーは Player が受け持つ |
| Sub ResetBGM | 371 | 76 | （なし） | SRC#で不要 | MIDI 音源のリセットは MCI・DirectMusic の操作なので、GUI と Player の範囲 |
| Function BGMStatus | 449 | 71 | `SRCCore/UIInterface/IPlaySound.cs` IPlaySound.BGMStatus | SRC#で不要 | 演奏の状態は Player が返す。BGMFileName が空のときの空文字列は RestartBGM の側で見る |
| Sub ChangeBGM | 522 | 49 | `SRCCore/Sound.cs` Sound.ChangeBGM | 修正済み | 同じ曲かの判定と、番号違い（(2)〜(9)）の判定で / も \ とみなす（#1498）。RepeatMode = True は VB と同じ（続く StartBGM が設定し直す） |
| Sub InitDirectMusic | 574 | 122 | （なし） | SRC#で不要 | DirectMusic の初期化 |
| Function CreateDirectXObject | 698 | 5 | （なし） | SRC#で不要 | DirectX の作成 |
| Sub CreateMIDIPortListFile | 705 | 27 | （なし） | SRC#で不要 | MIDI ポートの一覧ファイルを作る処理 |
| Sub InitVBMP3 | 735 | 24 | （なし） | SRC#で不要 | VBMP3.dll の初期化。SRC# は MP3 をいつも鳴らせる |
| Function SearchMidiFile | 762 | 253 | `SRCCore/Sound.cs` Sound.SearchMidiFile | 修正済み | 候補の分け方を ListLength・ListIndex にした（括弧の対応が取れていなければ探さない）。フルパスの候補が見つからなければ残りは探さずに終える（#1498）。合わせないもの: 名前を小文字にしない（大文字・小文字を区別するファイルシステムのため）。サブフォルダを候補ごとに決め直す（VB は前の候補のものを持ち越す不具合）。素材パックの zip と、読み込み対象のフォルダの Midi・直下も探すのは SRC# 独自 |
| Sub ResetMidiSearchPath | 1017 | 3 | `SRCCore/Sound.cs` Sound.ResetMidiSearchPath | 一致 | フラグを戻すだけ。C# は Midi フォルダの有無を覚えないので結果は同じ |
| Function BGMName | 1023 | 18 | `SRCCore/Sound.cs` Sound.BGMName | 一致 | ReadIni は ComposedConfig.GetItem に当たる |
| Sub InitDirectSound | 1044 | 24 | （なし） | SRC#で不要 | VB でも先頭で Exit Sub して何もしない |
| Sub PlayWave | 1070 | 150 | `SRCCore/Sound.cs` Sound.PlayWave | 一致 | 探す順（シナリオ → ExtDataPath → ExtDataPath2 → 本体の Sound → 絶対表記）と IsWavePlayed が同じ。合わせないもの: VB は MP3 を 1 系統でしか鳴らせないため、null.mp3 と MP3 の効果音で MP3 の BGM を止める。SRC# は BGM と効果音を別の系統で鳴らすので止めない |
| Sub StopWave | 1222 | 11 | `SRCCore/Sound.cs` Sound.StopWave | 一致 | 効果音の系統を止める |
| Sub FreeSoundModule | 1236 | 39 | `SRCCore/Sound.cs` Sound.FreeSoundModule | 一致 | KeepBGM・BossBGM を戻して StopBGM True、StopWave を呼ぶ。DirectX・VBMP3 の解放は Player が受け持つ |
