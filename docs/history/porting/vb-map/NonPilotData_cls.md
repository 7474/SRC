# NonPilotData.cls

VB 版の `SRC/SRC_20121125/NonPilotData.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Property Get Nickname0 | 32 | 3 | `SRCCore/Models/NonPilotData.cs` NonPilotData.Nickname0 | 一致 |  |
| Property Get Nickname | 36 | 56 | `SRCCore/Models/NonPilotData.cs` NonPilotData.Nickname | 修正済み | C# は「// Impl」のままで元の愛称を返していた。主人公・ヒロインの愛称の変数、ReplaceSubExpression、表情パターンのときのメインパイロットのユニットの「パイロット愛称」（$(愛称) の置き換え）を実装した（#1498）。SRC を渡さずに作ったデータ（テスト用）は置き換えずに返す |
| Property Let Nickname | 93 | 3 | `SRCCore/Models/NonPilotData.cs` NonPilotData.Nickname | 一致 |  |
| Property Get Bitmap0 | 98 | 3 | `SRCCore/Models/NonPilotData.cs` NonPilotData.Bitmap0 | 一致 |  |
| Property Get Bitmap | 102 | 7 | `SRCCore/Models/NonPilotData.cs` NonPilotData.Bitmap | 一致 |  |
| Property Let Bitmap | 110 | 3 | `SRCCore/Models/NonPilotData.cs` NonPilotData.Bitmap | 一致 |  |
