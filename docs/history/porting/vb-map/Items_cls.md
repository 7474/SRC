# Items.cls

VB 版の `SRC/SRC_20121125/Items.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Terminate | 24 | 10 | （見つからない） | 未確認 |  |
| Function NewEnum | 36 | 5 | （見つからない） | 未確認 |  |
| Function Add | 44 | 16 | `SRCCore/Items/Items.cs` Items.Add | 未確認 |  |
| Function CreateID | 62 | 6 | `SRCCore/Items/Items.cs` Items.CreateID | 未確認 |  |
| Function Count | 70 | 3 | `SRCCore/Items/Items.cs` Items.Count | 未確認 |  |
| Sub Delete | 75 | 3 | `SRCCore/Items/Items.cs` Items.Delete | 未確認 |  |
| Function Item | 80 | 25 | `SRCCore/Items/Items.cs` Items.Item | 未確認 |  |
| Function IsDefined | 107 | 26 | `SRCCore/Items/Items.cs` Items.IsDefined | 未確認 |  |
| Function IsDefined2 | 135 | 11 | `SRCCore/Items/Items.cs` Items.IsDefined2 | 未確認 |  |
| Sub Update | 148 | 21 | `SRCCore/Items/Items.cs` Items.Update | 未確認 |  |
| Sub Save | 172 | 16 | `SRCCore/Config/ComposedConfig.cs` ComposedConfig.Save<br>`SRCCore/Config/LocalFileConfig.cs` LocalFileConfig.Save<br>`SRCCore/Config/ScenarioConfig.cs` Value.Save | 未確認 |  |
| Sub Load | 190 | 34 | `SRCCore/Config/ComposedConfig.cs` ComposedConfig.Load<br>`SRCCore/Config/LocalFileConfig.cs` LocalFileConfig.Load<br>`SRCCore/Models/AliasDataList.cs` AliasDataList.Load | 未確認 |  |
| Sub LoadLinkInfo | 226 | 19 | （見つからない） | 未確認 |  |
| Sub Dump | 248 | 8 | （見つからない） | 未確認 |  |
| Sub Restore | 258 | 20 | `SRCCore/Items/Items.cs` Items.Restore | 未確認 |  |
| Sub RestoreLinkInfo | 280 | 10 | （見つからない） | 未確認 |  |
| Sub RestoreParameter | 292 | 10 | （見つからない） | 未確認 |  |
| Sub Clear | 305 | 7 | `SRCCore/Items/Items.cs` Items.Clear | 未確認 |  |
