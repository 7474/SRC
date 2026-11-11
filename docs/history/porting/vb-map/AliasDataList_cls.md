# AliasDataList.cls

VB 版の `SRC/SRC_20121125/AliasDataList.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Terminate | 25 | 10 | （なし） | SRC#で不要 | Collection を空にするだけ |
| Function Add | 37 | 7 | `SRCCore/Models/AliasDataList.cs` AliasDataList.Add | 一致 |  |
| Function Count | 46 | 3 | `SRCCore/Models/AliasDataList.cs` AliasDataList.Count | 一致 |  |
| Sub Delete | 51 | 3 | `SRCCore/Models/AliasDataList.cs` AliasDataList.Delete | 一致 |  |
| Function Item | 56 | 3 | `SRCCore/Models/AliasDataList.cs` AliasDataList.Item | 一致 | キーが無いとき、VB は実行時エラー、C# は null を返す。呼ぶ側は先に IsDefined で確かめている |
| Function IsDefined | 61 | 11 | `SRCCore/Models/AliasDataList.cs` AliasDataList.IsDefined | 一致 | キーの大文字・小文字などを区別しないのは SrcCollection と同じ |
| Sub Load | 74 | 84 | `SRCCore/Models/AliasDataList.cs` AliasDataList.Load | 修正済み | VB はファイルの終わりではエリアスの数を調べずに読み込みを終える。C# は「エリアス対象のデータが定義されていません。」を出していたので合わせた。レベルが数値でないときは原因を書かないデータエラーで打ち切る（#1498） |
| Function NewEnum | 160 | 3 | `SRCCore/Models/AliasDataList.cs` AliasDataList.Items | 一致 |  |
