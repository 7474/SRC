# Units.cls

VB 版の `SRC/SRC_20121125/Units.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 26 | 3 | （見つからない） | 未確認 |  |
| Sub Class_Terminate | 31 | 10 | （見つからない） | 未確認 |  |
| Function NewEnum | 43 | 5 | （見つからない） | 未確認 |  |
| Function Add | 51 | 219 | `SRCCore/Units/Units.cs` Units.Add | 未確認 |  |
| Sub Add2 | 272 | 3 | `SRCCore/Units/Units.cs` Units.Add2 | 未確認 |  |
| Function CreateID | 277 | 6 | `SRCCore/Units/Units.cs` Units.CreateID | 未確認 |  |
| Function Count | 285 | 3 | `SRCCore/Units/Units.cs` Units.Count | 未確認 |  |
| Sub Delete | 290 | 3 | `SRCCore/Units/Units.cs` Units.Delete | 未確認 |  |
| Function Item | 295 | 23 | `SRCCore/Units/Units.cs` Units.Item | 未確認 |  |
| Function Item2 | 320 | 8 | `SRCCore/Units/Units.cs` Units.Item2 | 未確認 |  |
| Function IsDefined | 330 | 24 | `SRCCore/Units/Units.cs` Units.IsDefined | 未確認 |  |
| Function IsDefined2 | 356 | 11 | `SRCCore/Units/Units.cs` Units.IsDefined2 | 未確認 |  |
| Sub Update | 369 | 332 | `SRCCore/Units/Units.cs` Units.Update | 未確認 |  |
| Sub Save | 704 | 33 | `SRCCore/Config/ComposedConfig.cs` ComposedConfig.Save<br>`SRCCore/Config/LocalFileConfig.cs` LocalFileConfig.Save<br>`SRCCore/Config/ScenarioConfig.cs` Value.Save | 未確認 | #1498（Event.bas・Intermission.bas・SRC.bas・LabelData.cls）で見つけた手がかり: C# の候補は誤り。SRCCore/SRC.save.cs SRC.SaveData・SRC.DumpData（JSON）に当たる |
| Sub Load | 740 | 64 | `SRCCore/Config/ComposedConfig.cs` ComposedConfig.Load<br>`SRCCore/Config/LocalFileConfig.cs` LocalFileConfig.Load<br>`SRCCore/Models/AliasDataList.cs` AliasDataList.Load | 未確認 | #1498（Event.bas・Intermission.bas・SRC.bas・LabelData.cls）で見つけた手がかり: C# の候補は誤り。SRCCore/SRC.save.cs SRC.LoadData（JSON）に当たる |
| Sub LoadLinkInfo | 806 | 84 | （見つからない） | 未確認 |  |
| Sub Dump | 893 | 9 | （見つからない） | 未確認 |  |
| Sub Restore | 904 | 20 | `SRCCore/Units/Units.cs` Units.Restore | 未確認 |  |
| Sub RestoreLinkInfo | 926 | 10 | （見つからない） | 未確認 |  |
| Sub RestoreParameter | 938 | 10 | （見つからない） | 未確認 |  |
| Sub Clear | 951 | 7 | `SRCCore/Units/Units.cs` Units.Clear | 未確認 |  |
| Sub ClearUnitBitmap | 960 | 19 | （見つからない） | 未確認 |  |
| Sub CheckAutoHyperMode | 982 | 7 | `SRCCore/Units/Units.cs` Units.CheckAutoHyperMode | 未確認 |  |
| Sub CheckAutoNormalMode | 991 | 15 | `SRCCore/Units/Units.cs` Units.CheckAutoNormalMode | 未確認 |  |
| Sub Clean | 1008 | 40 | `SRCCore/Units/Units.cs` Units.Clean | 未確認 |  |
