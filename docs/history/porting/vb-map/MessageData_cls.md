# MessageData.cls

VB 版の `SRC/SRC_20121125/MessageData.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 32 | 5 | （なし） | SRC#で不要 | 配列の初期化だけ |
| Sub AddMessage | 39 | 7 | `SRCCore/Models/MessageData.cs` MessageData.AddMessage | 一致 |  |
| Function CountMessage | 48 | 3 | `SRCCore/Models/MessageData.cs` MessageData.Items | 一致 | Items.Count |
| Function Situation | 53 | 3 | `SRCCore/Models/MessageData.cs` MessageDataItem.Situation | 一致 |  |
| Function Message | 58 | 3 | `SRCCore/Models/MessageData.cs` MessageDataItem.Message | 一致 |  |
| Function SelectMessage | 63 | 390 | `SRCCore/Models/MessageData.cs` MessageData.SelectMessage・`SRCCore/Extensions/SituationExtension.cs` SituationExtension.SelectMessage | 修正済み | SelectDialog と同じ乱数の呼び方（Dice(list_num) の後に Dice(2) を必ず呼ぶ）と、序数での先頭の一致に直した。相手の武器の属性は VB と同じく GetClassBundle のまとまり全体で「(対弱火属性)」のように作る（C# は先頭の 1 文字だけだった）（#1498）。Select Case ch / Case 0 To 127 は、まとまりの先頭の文字のコードが 127 より大きいかで判定している（VB6 で String を数値の範囲と比べたときの動作は確かめていない） |
