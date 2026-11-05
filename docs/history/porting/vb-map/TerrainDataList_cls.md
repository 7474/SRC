# TerrainDataList.cls

VB 版の `SRC/SRC_20121125/TerrainDataList.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 31 | 7 | （見つからない） | 未確認 |  |
| Sub Class_Terminate | 40 | 7 | （見つからない） | 未確認 |  |
| Function IsDefined | 50 | 7 | `SRCCore/Models/TerrainDataList.cs` TerrainDataList.IsDefined | 未確認 |  |
| Function Item | 60 | 3 | `SRCCore/Models/TerrainDataList.cs` TerrainDataList.Item | 未確認 | #1503 で見つけた手がかり: 同じ名前の地形が複数あるとき、VB は OrderedID の登録の順、C# の ItemByName は番号の小さい順で選ぶ |
| Function Name | 65 | 3 | `SRCCore/CmdDatas/CmdData.cs` CmdData.Name<br>`SRCCore/Config/LocalFileConfig.cs` ConfigSection.Name<br>`SRCCore/Config/LocalFileConfig.cs` ConfigItem.Name | 未確認 |  |
| Function Bitmap | 70 | 3 | `SRCCore/Models/NonPilotData.cs` NonPilotData.Bitmap<br>`SRCCore/Models/PilotData.cs` PilotData.Bitmap<br>`SRCCore/Models/UnitData.cs` UnitData.Bitmap | 未確認 |  |
| Function Class | 75 | 3 | `SRCCore/Items/Item.cs` Item.Class<br>`SRCCore/Pilots/Pilot.props.cs` Pilot.Class<br>`SRCCore/Units/Unit.props.cs` Unit.Class | 未確認 |  |
| Function MoveCost | 80 | 3 | （見つからない） | 未確認 |  |
| Function HitMod | 85 | 3 | （見つからない） | 未確認 |  |
| Function DamageMod | 90 | 3 | （見つからない） | 未確認 |  |
| Function IsFeatureAvailable | 97 | 3 | `SRCCore/Items/Item.cs` Item.IsFeatureAvailable<br>`SRCCore/Models/ItemData.cs` ItemData.IsFeatureAvailable<br>`SRCCore/Models/PilotData.cs` PilotData.IsFeatureAvailable | 未確認 |  |
| Function FeatureLevel | 101 | 3 | `SRCCore/Items/Item.cs` Item.FeatureLevel<br>`SRCCore/Models/FeatureData.cs` FeatureData.FeatureLevel<br>`SRCCore/Models/ItemData.cs` ItemData.FeatureLevel | 未確認 |  |
| Function FeatureData | 105 | 3 | `SRCCore/Extensions/UnitExtension.cs` CombineFeature.FeatureData<br>`SRCCore/Items/Item.cs` Item.FeatureData<br>`SRCCore/Models/FeatureData.cs` FeatureData | 未確認 |  |
| Function OrderedID | 111 | 3 | （見つからない） | 未確認 |  |
| Sub Load | 117 | 193 | `SRCCore/Models/TerrainDataList.cs` TerrainDataList.Load | 未確認 |  |
| Sub Clear | 312 | 8 | `SRCCore/Models/TerrainDataList.cs` TerrainDataList.Clear | 未確認 |  |
