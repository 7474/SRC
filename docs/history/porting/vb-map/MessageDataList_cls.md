# MessageDataList.cls

VB 版の `SRC/SRC_20121125/MessageDataList.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Terminate | 23 | 10 | （なし） | SRC#で不要 | Collection を空にするだけ |
| Function Add | 35 | 7 | `SRCCore/Models/MassageDataList.cs` MessageDataList.Add | 一致 |  |
| Function Count | 44 | 3 | `SRCCore/Models/MassageDataList.cs` MessageDataList.Count | 一致 |  |
| Sub Delete | 48 | 3 | `SRCCore/Models/MassageDataList.cs` MessageDataList.Delete | 一致 |  |
| Function Item | 53 | 9 | `SRCCore/Models/MassageDataList.cs` MessageDataList.Item | 一致 |  |
| Function IsDefined | 64 | 11 | `SRCCore/Models/MassageDataList.cs` MessageDataList.IsDefined | 一致 |  |
| Sub Load | 77 | 107 | `SRCCore/Models/MassageDataList.cs` MessageDataList.Load・MessageDataList.IsEffectFileName | 修正済み | VB はファイル名に effect.txt・animation.txt を含むか（ext_animation.txt も当たる）で特殊効果データかを決め、特殊効果データなら同じ名前のデータに追加する。C# は呼ぶ側が animation.txt・ext_animation.txt に false を渡していたので、ファイル名でも決めるようにした。末尾に空行が続くと名前が空のデータを作っていたのも直した（#1498）。VB はどのエラーでもメッセージを空にして DataErrorMessage を呼ぶ（err_msg を使わない不具合）が、C# は原因を書く（合わせない）。ErrorHandler（開けないときの「fname が開けません。」、打ち切る誤りの DataErrorMessage と TerminateSRC）は SRC.LoadDataFile が受け持つ。ゲームとして実行するとき（SRC.ShowDataErrorDialogs）だけダイアログを出して終了し、続ける誤りも SRC.AddDataError でダイアログに出す（#1550） |
| Sub AddDefaultAnimation | 186 | 54 | `SRCCore/Models/MassageDataList.cs` MessageDataList.AddDefaultAnimation | 修正済み | Data\System\animation.txt を特殊効果データとして読む（上の Load の修正による。#1498）。FindNormalLabel > 0 は、C# では 0 から始まる番号なので >= 0 にしていて一致 |
