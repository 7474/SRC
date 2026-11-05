# SRC.bas

VB 版の `SRC/SRC_20121125/SRC.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Main | 117 | 748 | `SRCSharpForm/Program.cs` Program.Main | 未確認 |  |
| Sub CreateIniFile | 867 | 142 | `SRCCore/SRC.config.cs` SRC.CreateIniFile | 未確認 |  |
| Function IsBitBltFasterThanStretchBlt | 1012 | 54 | （見つからない） | 未確認 |  |
| Sub StartScenario | 1069 | 195 | `SRCCore/SRC.play.cs` SRC.StartScenario | 未確認 |  |
| Sub StartTurn | 1266 | 468 | `SRCCore/SRC.play.cs` SRC.StartTurn | 未確認 |  |
| Sub GameOver | 1736 | 37 | `SRCCore/SRC.play.cs` SRC.GameOver | 未確認 |  |
| Sub GameClear | 1775 | 3 | `SRCCore/SRC.play.cs` SRC.GameClear | 未確認 |  |
| Sub ExitGame | 1780 | 39 | `SRCCore/SRC.play.cs` SRC.ExitGame | 未確認 |  |
| Sub TerminateSRC | 1821 | 85 | `SRCCore/SRC.play.cs` SRC.TerminateSRC | 未確認 |  |
| Sub SaveData | 1909 | 37 | `SRCCore/SRC.save.cs` SRC.SaveData | 未確認 |  |
| Sub LoadData | 1948 | 137 | `SRCCore/SRC.save.cs` SRC.LoadData | 未確認 |  |
| Sub DumpData | 2088 | 63 | `SRCCore/SRC.save.cs` SRC.DumpData | 未確認 |  |
| Sub RestoreData | 2153 | 316 | `SRCCore/SRC.save.cs` SRC.RestoreData | 未確認 |  |
| Sub ConvertUnitID | 2474 | 22 | `SRCCore/SRC.util.cs` SRC.ConvertUnitID | 未確認 |  |
| Sub IncludeData | 2498 | 69 | `SRCCore/SRC.data.cs` SRC.IncludeData | 未確認 |  |
| Function SearchDataFolder | 2569 | 61 | `SRCCore/SRC.data.cs` SRC.SearchDataFolder | 未確認 |  |
| Sub IncrMoney | 2632 | 4 | `SRCCore/SRC.util.cs` SRC.IncrMoney | 未確認 |  |
