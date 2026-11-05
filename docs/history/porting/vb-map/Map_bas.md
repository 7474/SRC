# Map.bas

VB 版の `SRC/SRC_20121125/Map.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub InitMap | 87 | 19 | `SRCCore/Maps/Map.cs` Map.InitMap | 未確認 |  |
| Function TerrainEffectForHit | 108 | 13 | （見つからない） | 未確認 |  |
| Function TerrainEffectForDamage | 123 | 13 | （見つからない） | 未確認 |  |
| Function TerrainEffectForHPRecover | 138 | 13 | （見つからない） | 未確認 |  |
| Function TerrainEffectForENRecover | 153 | 13 | （見つからない） | 未確認 |  |
| Function TerrainName | 168 | 13 | （見つからない） | 未確認 |  |
| Function TerrainClass | 183 | 13 | （見つからない） | 未確認 |  |
| Function TerrainMoveCost | 198 | 13 | （見つからない） | 未確認 |  |
| Function TerrainHasObstacle | 213 | 13 | （見つからない） | 未確認 |  |
| Function TerrainHasMoveStop | 229 | 10 | （見つからない） | 未確認 |  |
| Function TerrainDoNotEnter | 241 | 19 | （見つからない） | 未確認 |  |
| Function TerrainHasFeature | 262 | 10 | （見つからない） | 未確認 |  |
| Function UnitAtPoint | 275 | 11 | `SRCCore/Maps/Map.cs` Map.UnitAtPoint | 未確認 |  |
| Function SearchTerrainImageFile | 288 | 106 | `SRCCore/Maps/Map.cs` Map.SearchTerrainImageFile | 未確認 |  |
| Sub LoadMapData | 397 | 110 | `SRCCore/Maps/Map.cs` Map.LoadMapData | 未確認 |  |
| Sub SetMapSize | 509 | 75 | `SRCCore/Maps/Map.cs` Map.SetMapSize | 未確認 |  |
| Sub ClearMap | 586 | 43 | `SRCCore/Maps/Map.cs` Map.ClearMap | 未確認 |  |
| Sub DumpMapData | 632 | 40 | （見つからない） | 未確認 |  |
| Function RestoreMapData | 677 | 135 | （見つからない） | 未確認 |  |
| Sub AreaInRange | 816 | 136 | `SRCCore/Maps/Map.cs` Map.AreaInRange | 未確認 |  |
| Sub AreaInReachable | 955 | 133 | `SRCCore/Maps/Map.cs` Map.AreaInReachable | 未確認 |  |
| Sub AreaWithUnit | 1090 | 51 | `SRCCore/Maps/Map.cs` Map.AreaWithUnit | 未確認 |  |
| Sub AreaInCross | 1143 | 32 | `SRCCore/Maps/Map.cs` Map.AreaInCross | 未確認 |  |
| Sub AreaInLine | 1177 | 38 | `SRCCore/Maps/Map.cs` Map.AreaInLine | 未確認 |  |
| Sub AreaInWideCross | 1217 | 76 | `SRCCore/Maps/Map.cs` Map.AreaInWideCross | 未確認 |  |
| Sub AreaInCone | 1295 | 82 | `SRCCore/Maps/Map.cs` Map.AreaInCone | 未確認 |  |
| Sub AreaInSector | 1379 | 121 | `SRCCore/Maps/Map.cs` Map.AreaInSector | 未確認 |  |
| Sub AreaInSectorCross | 1502 | 15 | `SRCCore/Maps/Map.cs` Map.AreaInSectorCross | 未確認 |  |
| Sub AreaInPointToPoint | 1519 | 40 | `SRCCore/Maps/Map.cs` Map.AreaInPointToPoint | 未確認 |  |
| Sub AreaInSpeed | 1562 | 998 | `SRCCore/Maps/Map.cs` Map.AreaInSpeed | 未確認 |  |
| Sub AreaInTeleport | 2563 | 288 | `SRCCore/Maps/Map.cs` Map.AreaInTeleport | 未確認 |  |
| Sub AreaInMoveAction | 2853 | 268 | `SRCCore/Maps/Map.cs` Map.AreaInMoveAction | 未確認 |  |
| Function IsLineBlocked | 3123 | 74 | `SRCCore/Maps/Map.cs` Map.IsLineBlocked | 未確認 |  |
| Sub NearestPoint | 3199 | 359 | `SRCCore/Maps/Map.cs` Map.NearestPoint | 未確認 |  |
| Sub SafetyPoint | 3560 | 67 | `SRCCore/Maps/Map.cs` Map.SafetyPoint | 未確認 | #1501 で見つけた手がかり: VB は ByRef の X・Y を移動先が見つからなければ渡された値のまま残すが、C# は out で u.x・u.y を入れる（遊撃の再移動の目標が変わる） |
| Sub SearchMoveRoute | 3630 | 178 | `SRCCore/Maps/Map.cs` Map.SearchMoveRoute | 未確認 |  |
