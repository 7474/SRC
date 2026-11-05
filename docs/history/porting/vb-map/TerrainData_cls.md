# TerrainData.cls

VB 版の `SRC/SRC_20121125/TerrainData.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 39 | 3 | （見つからない） | 未確認 |  |
| Sub Class_Terminate | 44 | 12 | （見つからない） | 未確認 |  |
| Sub AddFeature | 59 | 47 | `SRCCore/Models/TerrainData.cs` TerrainData.AddFeature | 未確認 |  |
| Function CountFeature | 108 | 6 | `SRCCore/Models/TerrainData.cs` TerrainData.CountFeature | 未確認 |  |
| Function Feature | 116 | 6 | `SRCCore/Models/TerrainData.cs` TerrainData.Feature | 未確認 |  |
| Function FeatureName | 124 | 14 | `SRCCore/Items/Item.cs` Item.FeatureName<br>`SRCCore/Models/FeatureData.cs` FeatureData.FeatureName<br>`SRCCore/Models/ItemData.cs` ItemData.FeatureName | 未確認 |  |
| Function FeatureLevel | 140 | 14 | `SRCCore/Models/TerrainData.cs` TerrainData.FeatureLevel | 未確認 |  |
| Function FeatureData | 156 | 11 | `SRCCore/Extensions/UnitExtension.cs` CombineFeature.FeatureData<br>`SRCCore/Items/Item.cs` Item.FeatureData<br>`SRCCore/Models/FeatureData.cs` FeatureData | 未確認 |  |
| Function IsFeatureAvailable | 169 | 14 | `SRCCore/Models/TerrainData.cs` TerrainData.IsFeatureAvailable | 未確認 |  |
| Function IsFeatureLevelSpecified | 185 | 16 | `SRCCore/Units/Unit.feature.cs` Unit.IsFeatureLevelSpecified | 未確認 |  |
| Sub Clear | 203 | 13 | `SRCCore/Expressions/VarData.cs` VarData.Clear<br>`SRCCore/Items/Items.cs` Items.Clear<br>`SRCCore/Models/ItemDataList.cs` ItemDataList.Clear | 未確認 |  |
