# Pilots.cls

VB 版の `SRC/SRC_20121125/Pilots.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Terminate | 23 | 10 | （見つからない） | 未確認 |  |
| Function NewEnum | 35 | 5 | （見つからない） | 未確認 |  |
| Function Add | 43 | 60 | `SRCCore/Pilots/Pilots.cs` Pilots.Add | 未確認 |  |
| Function Count | 105 | 3 | `SRCCore/Pilots/Pilots.cs` Pilots.Count | 未確認 |  |
| Sub Delete | 110 | 3 | `SRCCore/Pilots/Pilots.cs` Pilots.Delete | 未確認 |  |
| Function Item | 115 | 31 | `SRCCore/Pilots/Pilots.cs` Pilots.Item | 未確認 |  |
| Function IsDefined | 148 | 32 | `SRCCore/Pilots/Pilots.cs` Pilots.IsDefined | 未確認 |  |
| Function Item2 | 182 | 8 | `SRCCore/Pilots/Pilots.cs` Pilots.Item2 | 未確認 |  |
| Function IsDefined2 | 192 | 11 | `SRCCore/Pilots/Pilots.cs` Pilots.IsDefined2 | 未確認 |  |
| Sub Update | 205 | 48 | `SRCCore/Pilots/Pilots.cs` Pilots.Update | 未確認 |  |
| Sub Save | 256 | 37 | `SRCCore/Config/ComposedConfig.cs` ComposedConfig.Save<br>`SRCCore/Config/LocalFileConfig.cs` LocalFileConfig.Save<br>`SRCCore/Config/ScenarioConfig.cs` Value.Save | 未確認 |  |
| Sub Load | 295 | 41 | `SRCCore/Config/ComposedConfig.cs` ComposedConfig.Load<br>`SRCCore/Config/LocalFileConfig.cs` LocalFileConfig.Load<br>`SRCCore/Models/AliasDataList.cs` AliasDataList.Load | 未確認 |  |
| Sub LoadLinkInfo | 338 | 65 | （見つからない） | 未確認 |  |
| Sub Dump | 406 | 23 | （見つからない） | 未確認 |  |
| Sub Restore | 431 | 20 | `SRCCore/Pilots/Pilots.cs` Pilots.Restore | 未確認 |  |
| Sub RestoreLinkInfo | 453 | 9 | （見つからない） | 未確認 |  |
| Sub RestoreParameter | 464 | 9 | （見つからない） | 未確認 |  |
| Sub Clear | 476 | 7 | `SRCCore/Expressions/VarData.cs` VarData.Clear<br>`SRCCore/Items/Items.cs` Items.Clear<br>`SRCCore/Models/ItemDataList.cs` ItemDataList.Clear | 未確認 |  |
| Sub UpdateSupportMod | 486 | 93 | `SRCCore/Pilots/Pilots.cs` Pilots.UpdateSupportMod | 未確認 |  |
| Sub Clean | 581 | 12 | `SRCCore/Pilots/Pilots.cs` Pilots.Clean | 未確認 |  |
