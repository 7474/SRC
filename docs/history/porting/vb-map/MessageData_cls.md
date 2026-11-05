# MessageData.cls

VB 版の `SRC/SRC_20121125/MessageData.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 32 | 5 | （見つからない） | 未確認 |  |
| Sub AddMessage | 39 | 7 | `SRCCore/Models/MessageData.cs` MessageData.AddMessage | 未確認 |  |
| Function CountMessage | 48 | 3 | （見つからない） | 未確認 |  |
| Function Situation | 53 | 3 | `SRCCore/Models/Dialog.cs` Dialog.Situation<br>`SRCCore/Models/MessageData.cs` MessageDataItem.Situation | 未確認 |  |
| Function Message | 58 | 3 | `SRCCore/Models/MessageData.cs` MessageDataItem.Message<br>`SRCRender/ScreenRenderer.cs` ScreenLayers.Message<br>`SRCRender/ScreenState.cs` ScreenState.Message | 未確認 |  |
| Function SelectMessage | 63 | 390 | `SRCCore/Models/MessageData.cs` MessageData.SelectMessage | 未確認 |  |
