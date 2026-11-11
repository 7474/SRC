# NonPilotDataList.cls

VB 版の `SRC/SRC_20121125/NonPilotDataList.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 23 | 11 | `SRCCore/Models/NonPilotDataList.cs` NonPilotDataList | 一致 | ナレーターを登録する |
| Sub Class_Terminate | 36 | 10 | （なし） | SRC#で不要 | Collection を空にするだけ |
| Function Add | 48 | 7 | `SRCCore/Models/NonPilotDataList.cs` NonPilotDataList.Add | 一致 | 愛称の置き換えのために SRC を渡す |
| Function Count | 57 | 3 | `SRCCore/Models/NonPilotDataList.cs` NonPilotDataList.Count | 一致 |  |
| Sub Delete | 62 | 3 | `SRCCore/Models/NonPilotDataList.cs` NonPilotDataList.Delete | 一致 |  |
| Function Item | 67 | 17 | `SRCCore/Models/NonPilotDataList.cs` NonPilotDataList.Item | 一致 | キーが無ければ愛称（Nickname0）と序数で比べる |
| Function IsDefined | 86 | 19 | `SRCCore/Models/NonPilotDataList.cs` NonPilotDataList.IsDefined | 一致 |  |
| Function IsDefined2 | 107 | 11 | `SRCCore/Models/NonPilotDataList.cs` NonPilotDataList.IsDefined2 | 一致 |  |
| Sub Load | 120 | 97 | `SRCCore/Models/NonPilotDataList.cs` NonPilotDataList.Load | 修正済み | 大文字・小文字や全角・半角だけが違う名前のデータがあると、VB は Collection のキーの重複で実行時エラーになり読み込みを打ち切る。C# もデータエラーで打ち切るようにした（#1498）。.bmp 以外の画像も受け付けるのは SRC# の拡張（#1212） |
