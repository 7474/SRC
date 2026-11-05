# DialogData.cls

VB 版の `SRC/SRC_20121125/DialogData.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 30 | 5 | （見つからない） | 未確認 |  |
| Sub Class_Terminate | 37 | 10 | （見つからない） | 未確認 |  |
| Function AddDialog | 49 | 10 | `SRCCore/Models/DialogData.cs` DialogData.AddDialog | 未確認 |  |
| Function CountDialog | 61 | 3 | `SRCCore/Models/DialogData.cs` DialogData.CountDialog | 未確認 |  |
| Function Situation | 66 | 3 | `SRCCore/Models/Dialog.cs` Dialog.Situation<br>`SRCCore/Models/MessageData.cs` MessageDataItem.Situation | 未確認 |  |
| Function Dialog | 71 | 3 | `SRCCore/Models/Dialog.cs` Dialog<br>`SRCRender/ScreenState.cs` ScreenState.Dialog | 未確認 |  |
| Function SelectDialog | 76 | 390 | `SRCCore/Models/DialogData.cs` DialogData.SelectDialog | 未確認 |  |
