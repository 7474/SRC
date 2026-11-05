# Sound.bas

VB 版の `SRC/SRC_20121125/Sound.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub StartBGM | 69 | 71 | `SRCCore/Sound.cs` Sound.StartBGM | 未確認 |  |
| Sub LoadBGM | 142 | 139 | （見つからない） | 未確認 |  |
| Sub RestartBGM | 283 | 35 | `SRCCore/Sound.cs` Sound.RestartBGM | 未確認 |  |
| Sub StopBGM | 320 | 49 | `SRCCore/Sound.cs` Sound.StopBGM | 未確認 |  |
| Sub ResetBGM | 371 | 76 | （見つからない） | 未確認 |  |
| Function BGMStatus | 449 | 71 | `SRCCore/UIInterface/IPlaySound.cs` BGMStatus<br>`SRCRender/NullSoundPlayer.cs` NullSoundPlayer.BGMStatus<br>`SRCSharpForm/Resoruces/WindowsManagedPlayer.cs` WindowsManagedPlayer.BGMStatus | 未確認 |  |
| Sub ChangeBGM | 522 | 49 | `SRCCore/Sound.cs` Sound.ChangeBGM | 未確認 |  |
| Sub InitDirectMusic | 574 | 122 | （見つからない） | 未確認 |  |
| Function CreateDirectXObject | 698 | 5 | （見つからない） | 未確認 |  |
| Sub CreateMIDIPortListFile | 705 | 27 | （見つからない） | 未確認 |  |
| Sub InitVBMP3 | 735 | 24 | （見つからない） | 未確認 |  |
| Function SearchMidiFile | 762 | 253 | `SRCCore/Sound.cs` Sound.SearchMidiFile | 未確認 |  |
| Sub ResetMidiSearchPath | 1017 | 3 | `SRCCore/Sound.cs` Sound.ResetMidiSearchPath | 未確認 |  |
| Function BGMName | 1023 | 18 | `SRCCore/Sound.cs` Sound.BGMName | 未確認 |  |
| Sub InitDirectSound | 1044 | 24 | （見つからない） | 未確認 |  |
| Sub PlayWave | 1070 | 150 | `SRCCore/Sound.cs` Sound.PlayWave | 未確認 |  |
| Sub StopWave | 1222 | 11 | `SRCCore/Sound.cs` Sound.StopWave | 未確認 |  |
| Sub FreeSoundModule | 1236 | 39 | `SRCCore/Sound.cs` Sound.FreeSoundModule | 未確認 |  |
