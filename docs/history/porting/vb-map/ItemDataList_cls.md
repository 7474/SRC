# ItemDataList.cls

VB 版の `SRC/SRC_20121125/ItemDataList.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Terminate | 24 | 10 | （なし） | SRC#で不要 | Collection の解放 |
| Function Add | 36 | 7 | `SRCCore/Models/ItemDataList.cs` ItemDataList.Add | 一致 |  |
| Function Count | 45 | 3 | `SRCCore/Models/ItemDataList.cs` ItemDataList.Count | 一致 |  |
| Sub Delete | 50 | 3 | `SRCCore/Models/ItemDataList.cs` ItemDataList.Delete | 一致 |  |
| Function Item | 55 | 9 | `SRCCore/Models/ItemDataList.cs` ItemDataList.Item | 一致 |  |
| Function IsDefined | 66 | 11 | `SRCCore/Models/ItemDataList.cs` ItemDataList.IsDefined | 一致 |  |
| Sub Load | 79 | 763 | `SRCCore/Models/ItemDataList.cs` ItemDataList.Load・LoadItem<br>`SRCCore/Models/UnitDataList.cs` UnitDataList.LoadFeatureOuter・LoadFeature・LoadWepon・LoadAbility | 修正済み | 直したもの（#1498）: (1) 特殊能力の「Lv」の後が数値でないときは、原因を書かないデータエラーで打ち切る（#1526） (2) 新形式の特殊能力の節を、VB と同じく行頭の項目が数値で「,」が続くときだけ終え、それ以外の数値は特殊能力として読み、空行はエラーを出して読み続ける (3) 最小射程を VB と同じく 99 までにした (4) アビリティの「,」が無いときのエラーを「終りには」にした (5) 「*」で始まる行で武器・アビリティを終える処理はアイテムだけにした。合わせないもの: 2 つ目以降のデータで「名称の設定が抜けています。」などを出すとき、VB は data_name を初期化しないので前のデータの名前を出す（VB の不具合）。C# は空 |
