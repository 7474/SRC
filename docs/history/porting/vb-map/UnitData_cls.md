# UnitData.cls

VB 版の `SRC/SRC_20121125/UnitData.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Terminate | 69 | 30 | （なし） | SRC#で不要 | Collection の解放 |
| Property Get Nickname | 101 | 7 | `SRCCore/Models/UnitData.cs` UnitData.Nickname | 一致 |  |
| Property Let Nickname | 109 | 3 | `SRCCore/Models/UnitData.cs` UnitData.Nickname | 一致 |  |
| Property Get KanaName | 114 | 14 | `SRCCore/Models/UnitData.cs` UnitData.KanaName | 一致 |  |
| Property Let KanaName | 129 | 3 | `SRCCore/Models/UnitData.cs` UnitData.KanaName | 一致 |  |
| Property Get Bitmap0 | 134 | 3 | `SRCCore/Models/UnitData.cs` UnitData.Bitmap0 | 一致 |  |
| Property Get Bitmap | 138 | 7 | `SRCCore/Models/UnitData.cs` UnitData.Bitmap | 一致 |  |
| Property Let Bitmap | 146 | 3 | `SRCCore/Models/UnitData.cs` UnitData.Bitmap | 一致 |  |
| Sub AddFeature | 152 | 157 | `SRCCore/Models/UnitData.cs` UnitData.AddFeature | 修正済み | 「Lv」の後を Convert.ToDouble で読んでいたので、数値でないときに例外が Load の外へ出ていた。FeatureData.ParseLevel にそろえ、Load で原因を書かないデータエラーにした（#1498・#1526）。エリアスの展開は AliasData.ExpandFeatureAlias |
| Function CountFeature | 311 | 6 | `SRCCore/Models/UnitData.cs` UnitData.CountFeature | 一致 |  |
| Function Feature | 319 | 6 | `SRCCore/Models/UnitData.cs` UnitData.Feature | 一致 | VB は名前を返す。C# は FeatureData を返し、呼ぶ側が Name を読む |
| Function FeatureName | 327 | 14 | `SRCCore/Models/UnitData.cs` UnitData.FeatureName | 一致 | Level > 0 で「Lv」を付ける（FeatureNameWithLv） |
| Function FeatureLevel | 343 | 15 | `SRCCore/Models/UnitData.cs` UnitData.FeatureLevel | 修正済み | 範囲外の番号で例外を投げていた。VB と同じく 0 を返すようにした（#1498） |
| Function FeatureData | 360 | 11 | `SRCCore/Models/UnitData.cs` UnitData.FeatureData | 一致 | 範囲外のときは "" を返す（try/catch を範囲を確かめる書き方に直した） |
| Function FeatureNecessarySkill | 373 | 11 | （なし。FeatureData.NecessarySkill） | SRC#で不要 | VB でもほかから呼ばれていない（Unit は自分の colFeature を見る） |
| Function IsFeatureAvailable | 386 | 11 | `SRCCore/Models/UnitData.cs` UnitData.IsFeatureAvailable | 一致 | Collection のキー（大小を区別しない）で探す |
| Function IsFeatureLevelSpecified | 399 | 16 | （なし。FeatureData.HasLevel） | SRC#で不要 | VB でもほかから呼ばれていない |
| Function AddWeapon | 417 | 10 | `SRCCore/Models/UnitData.cs` UnitData.AddWeapon | 一致 |  |
| Function CountWeapon | 429 | 6 | `SRCCore/Models/UnitData.cs` UnitData.CountWeapon | 一致 |  |
| Function Weapon | 437 | 3 | `SRCCore/Models/UnitData.cs` UnitData.Weapon | 一致 |  |
| Function AddAbility | 442 | 10 | `SRCCore/Models/UnitData.cs` UnitData.AddAbility | 一致 | キーはアビリティの名前と番号（ItemData とは違う） |
| Function CountAbility | 454 | 6 | `SRCCore/Models/UnitData.cs` UnitData.CountAbility | 一致 |  |
| Function Ability | 462 | 3 | `SRCCore/Models/UnitData.cs` UnitData.Ability | 一致 |  |
| Sub Clear | 467 | 19 | `SRCCore/Models/UnitData.cs` UnitData.Clear | 一致 |  |
