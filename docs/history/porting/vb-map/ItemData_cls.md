# ItemData.cls

VB 版の `SRC/SRC_20121125/ItemData.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Terminate | 51 | 30 | （見つからない） | 未確認 |  |
| Property Get Nickname | 83 | 7 | `SRCCore/Models/ItemData.cs` ItemData.Nickname | 未確認 |  |
| Property Let Nickname | 91 | 3 | `SRCCore/Models/ItemData.cs` ItemData.Nickname | 未確認 |  |
| Property Get KanaName | 96 | 14 | `SRCCore/Models/ItemData.cs` ItemData.KanaName | 未確認 |  |
| Property Let KanaName | 111 | 3 | `SRCCore/Models/ItemData.cs` ItemData.KanaName | 未確認 |  |
| Sub AddFeature | 117 | 157 | `SRCCore/Models/ItemData.cs` ItemData.AddFeature | 未確認 |  |
| Function CountFeature | 276 | 6 | `SRCCore/Models/ItemData.cs` ItemData.CountFeature | 未確認 |  |
| Function Feature | 284 | 6 | `SRCCore/Models/ItemData.cs` ItemData.Feature | 未確認 |  |
| Function FeatureName | 292 | 14 | `SRCCore/Models/ItemData.cs` ItemData.FeatureName | 未確認 | #1498（Pilot）で見つけた手がかり: VB は `Level <> DEFAULT_LEVEL` で「Lv」を付ける（UnitData だけが `> 0`）。共通の FeatureNameWithLv は Level > 0 のまま。PilotData は #1498 で直した |
| Function FeatureLevel | 308 | 14 | `SRCCore/Models/ItemData.cs` ItemData.FeatureLevel | 未確認 |  |
| Function FeatureData | 324 | 11 | `SRCCore/Models/ItemData.cs` ItemData.FeatureData | 未確認 |  |
| Function FeatureNecessarySkill | 337 | 11 | `SRCCore/Items/Item.cs` Item.FeatureNecessarySkill | 未確認 |  |
| Function IsFeatureAvailable | 350 | 14 | `SRCCore/Models/ItemData.cs` ItemData.IsFeatureAvailable | 未確認 |  |
| Function IsFeatureLevelSpecified | 366 | 16 | `SRCCore/Units/Unit.feature.cs` Unit.IsFeatureLevelSpecified | 未確認 |  |
| Function Weapon | 384 | 3 | `SRCCore/Models/ItemData.cs` ItemData.Weapon | 未確認 |  |
| Function CountWeapon | 389 | 6 | `SRCCore/Models/ItemData.cs` ItemData.CountWeapon | 未確認 |  |
| Function AddWeapon | 397 | 10 | `SRCCore/Models/ItemData.cs` ItemData.AddWeapon | 未確認 |  |
| Function Ability | 409 | 3 | `SRCCore/Models/ItemData.cs` ItemData.Ability | 未確認 |  |
| Function CountAbility | 414 | 6 | `SRCCore/Models/ItemData.cs` ItemData.CountAbility | 未確認 |  |
| Function AddAbility | 422 | 10 | `SRCCore/Models/ItemData.cs` ItemData.AddAbility | 未確認 |  |
| Function Size | 434 | 3 | `SRCCore/Models/ItemData.cs` ItemData.Size | 未確認 |  |
