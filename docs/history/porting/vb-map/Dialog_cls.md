# Dialog.cls

VB 版の `SRC/SRC_20121125/Dialog.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 27 | 5 | （見つからない） | 未確認 |  |
| Sub Class_Terminate | 34 | 4 | （見つからない） | 未確認 |  |
| Sub AddMessage | 40 | 7 | `SRCCore/Models/Dialog.cs` Dialog.AddMessage | 未確認 |  |
| Property Get Count | 49 | 3 | `SRCCore/Models/Dialog.cs` Dialog.Count | 未確認 |  |
| Function Name | 54 | 3 | `SRCCore/CmdDatas/CmdData.cs` CmdData.Name<br>`SRCCore/Config/LocalFileConfig.cs` ConfigSection.Name<br>`SRCCore/Config/LocalFileConfig.cs` ConfigItem.Name | 未確認 |  |
| Function Message | 59 | 3 | `SRCCore/Models/MessageData.cs` MessageDataItem.Message<br>`SRCRender/ScreenRenderer.cs` ScreenLayers.Message<br>`SRCRender/ScreenState.cs` ScreenState.Message | 未確認 |  |
| Property Get IsAvailable | 64 | 138 | `SRCCore/Models/Dialog.cs` Dialog.IsAvailable | 未確認 |  |
