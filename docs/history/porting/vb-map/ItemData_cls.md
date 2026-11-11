# ItemData.cls

VB 版の `SRC/SRC_20121125/ItemData.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Terminate | 51 | 30 | （なし） | SRC#で不要 | Collection の解放 |
| Property Get Nickname | 83 | 7 | `SRCCore/Models/ItemData.cs` ItemData.Nickname | 一致 |  |
| Property Let Nickname | 91 | 3 | `SRCCore/Models/ItemData.cs` ItemData.Nickname | 一致 |  |
| Property Get KanaName | 96 | 14 | `SRCCore/Models/ItemData.cs` ItemData.KanaName | 一致 |  |
| Property Let KanaName | 111 | 3 | `SRCCore/Models/ItemData.cs` ItemData.KanaName | 一致 |  |
| Sub AddFeature | 117 | 157 | `SRCCore/Models/ItemData.cs` ItemData.AddFeature | 修正済み | 「Lv」の後が数値でないとき 0 として読んでいた。FeatureData.ParseLevel で、Load が原因を書かないデータエラーで打ち切るようにした（#1498・#1526）。名前が大文字・小文字や全角・半角だけ違う特殊能力を 2 つ書くと、VB はキーの重複で打ち切るが、C# は例外が Load の外へ出る（めったに無いので合わせていない） |
| Function CountFeature | 276 | 6 | `SRCCore/Models/ItemData.cs` ItemData.CountFeature | 一致 |  |
| Function Feature | 284 | 6 | `SRCCore/Models/ItemData.cs` ItemData.Feature | 一致 | C# は FeatureData を返す |
| Function FeatureName | 292 | 14 | `SRCCore/Models/ItemData.cs` ItemData.FeatureName | 修正済み | 共通の FeatureNameWithLv（Level > 0）を使っていた。ItemData.cls と同じく Level <> DEFAULT_LEVEL で、Lv0 や負のレベルにも「Lv」を付ける（#1498） |
| Function FeatureLevel | 308 | 14 | `SRCCore/Models/ItemData.cs` ItemData.FeatureLevel | 修正済み | 範囲外の番号で例外を投げていた。0 を返すようにした（#1498） |
| Function FeatureData | 324 | 11 | `SRCCore/Models/ItemData.cs` ItemData.FeatureData | 修正済み | 範囲外の番号で例外を投げていた。"" を返すようにした（#1498） |
| Function FeatureNecessarySkill | 337 | 11 | `SRCCore/Items/Item.cs` Item.FeatureNecessarySkill | 修正済み | ItemData には無く、Item.FeatureNecessarySkill が Data.Feature(Index).NecessarySkill を読む。無い特殊能力の名前で NullReferenceException になっていたので、VB の On Error と同じく "" を返すようにした（#1498） |
| Function IsFeatureAvailable | 350 | 14 | `SRCCore/Models/ItemData.cs` ItemData.IsFeatureAvailable | 一致 | 名前を完全に一致させて比べる |
| Function IsFeatureLevelSpecified | 366 | 16 | （なし。FeatureData.HasLevel） | SRC#で不要 | VB でもほかから呼ばれていない |
| Function Weapon | 384 | 3 | `SRCCore/Models/ItemData.cs` ItemData.Weapon | 一致 |  |
| Function CountWeapon | 389 | 6 | `SRCCore/Models/ItemData.cs` ItemData.CountWeapon | 一致 |  |
| Function AddWeapon | 397 | 10 | `SRCCore/Models/ItemData.cs` ItemData.AddWeapon | 一致 |  |
| Function Ability | 409 | 3 | `SRCCore/Models/ItemData.cs` ItemData.Ability | 一致 |  |
| Function CountAbility | 414 | 6 | `SRCCore/Models/ItemData.cs` ItemData.CountAbility | 一致 |  |
| Function AddAbility | 422 | 10 | `SRCCore/Models/ItemData.cs` ItemData.AddAbility | 修正済み | VB はアイテムの名前と番号（Name & Format$(CountAbility)）をキーにする。C# はアビリティの名前を使っていたので、「A1」と 10 番目の「A」のようにキーが重なると例外になっていた（#1498） |
| Function Size | 434 | 3 | `SRCCore/Models/ItemData.cs` ItemData.Size | 一致 | 1 + FeatureLevel を Integer に入れるときの偶数丸めは VBMath.CLng で同じ |
