# DialogDataList.cls

VB 版の `SRC/SRC_20121125/DialogDataList.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Terminate | 23 | 10 | （なし） | SRC#で不要 | Collection を空にするだけ |
| Function Add | 35 | 7 | `SRCCore/Models/DialogDataList.cs` DialogDataList.Add | 一致 |  |
| Function Count | 44 | 3 | `SRCCore/Models/DialogDataList.cs` DialogDataList.Count | 一致 |  |
| Sub Delete | 48 | 3 | `SRCCore/Models/DialogDataList.cs` DialogDataList.Delete | 一致 |  |
| Function Item | 53 | 9 | `SRCCore/Models/DialogDataList.cs` DialogDataList.Item | 一致 | 無ければ null |
| Function IsDefined | 64 | 11 | `SRCCore/Models/DialogDataList.cs` DialogDataList.IsDefined | 一致 |  |
| Sub Load | 77 | 101 | `SRCCore/Models/DialogDataList.cs` DialogDataList.Load | 修正済み | (1) シチュエーションの行がファイルの最後にあるとき、VB はメッセージの無いダイアログとして登録するが、C# は登録していなかった (2) 末尾に空行が続くと名前が空のダイアログデータを作っていた (3) パイロット名の一覧の要素が 0 個なら、原因を書かないデータエラーで打ち切る（#1498）。パイロット名の一覧は LNormalize で作る（LIndex を空白でつないで Trim するのと同じ） |
