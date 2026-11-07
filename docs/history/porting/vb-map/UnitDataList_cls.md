# UnitDataList.cls

VB 版の `SRC/SRC_20121125/UnitDataList.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 26 | 14 | `SRCCore/Models/UnitDataList.cs` UnitDataList.AddDummyData | 一致 | ダミーユニットを登録する（コンストラクタから呼ぶ） |
| Sub Class_Terminate | 42 | 10 | （なし） | SRC#で不要 | Collection の解放 |
| Function Add | 54 | 12 | `SRCCore/Models/UnitDataList.cs` UnitDataList.Add | 一致 |  |
| Function Count | 68 | 3 | `SRCCore/Models/UnitDataList.cs` UnitDataList.Count | 一致 |  |
| Sub Delete | 73 | 3 | `SRCCore/Models/UnitDataList.cs` UnitDataList.Delete | 一致 |  |
| Function Item | 78 | 9 | `SRCCore/Models/UnitDataList.cs` UnitDataList.Item | 一致 |  |
| Function IsDefined | 89 | 11 | `SRCCore/Models/UnitDataList.cs` UnitDataList.IsDefined | 一致 |  |
| Sub Load | 102 | 913 | `SRCCore/Models/UnitDataList.cs` UnitDataList.Load・LoadUnit・LoadFeatureOuter・LoadFeature・LoadWepon・LoadAbility | 修正済み | 直したもの（#1498）: (1) 特殊能力の「Lv」の後が数値でないとき、アビリティの効果の「Lv」の後が数値でないとき・「回復=」のようにデータが空のときは、VB の CDbl・Asc の実行時エラーと同じく、原因を書かないデータエラーで打ち切る（#1526。パイロット・ユニット・アイテムで共通。SRCDataLinter でも行番号付きのエラーになる） (2) 新形式の特殊能力の節は、VB と同じく空行で「行頭から1番目の特殊能力の設定が間違っています。」を出して読み続け、IsNumeric の項目で終える（「===」や「+」「-」を除く SRC# 独自の処理をやめた） (3) 「*」で始まる行で武器・アビリティを終えるのはアイテムだけにした (4) パイロット数の「(」に「)」が無いときは、原因を書かないデータエラーにする。合わせないもの: 節の途中でファイルが終わると VB は空行を読み続けて止まらないが、SRC# はそこで節を終える。.png なども受け付けるのは SRC# の拡張（#1212）。データエラーのデータ名は、VB は先に定義したデータの .Name、C# はその行の data_name（大文字・小文字だけが違う名前で定義し直したときだけ違う） |
