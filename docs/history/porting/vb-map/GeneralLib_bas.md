# GeneralLib.bas

VB 版の `SRC/SRC_20121125/GeneralLib.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub RndReset | 60 | 13 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.RndReset | 未確認 |  |
| Function Dice | 75 | 18 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.Dice | 未確認 |  |
| Function LIndex | 96 | 53 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.LIndex | 未確認 |  |
| Function LLength | 151 | 29 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.LLength | 未確認 |  |
| Function LSplit | 183 | 35 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.LSplit | 未確認 |  |
| Function IsSpace | 220 | 11 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.IsSpace | 未確認 |  |
| Sub LAppend | 233 | 13 | （見つからない） | 未確認 |  |
| Function SearchList | 248 | 12 | （見つからない） | 未確認 |  |
| Function ListIndex | 263 | 150 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.ListIndex | 未確認 |  |
| Function ListLength | 415 | 93 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.ListLength | 未確認 |  |
| Function ListSplit | 511 | 100 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.ListSplit | 未確認 |  |
| Function ListTail | 613 | 98 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.ListTail | 未確認 |  |
| Sub TrimString | 714 | 32 | （見つからない） | 未確認 |  |
| Function InStr2 | 748 | 13 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.InStr2 | 未確認 |  |
| Function StrToDbl | 764 | 5 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.StrToDbl | 未確認 |  |
| Function StrToLng | 771 | 8 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.StrToLng | 未確認 |  |
| Function StrToHiragana | 783 | 11 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.StrToHiragana | 未確認 |  |
| Function MaxLng | 797 | 7 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.MaxLng | 未確認 |  |
| Function MinLng | 806 | 7 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.MinLng | 未確認 |  |
| Function MaxDbl | 815 | 7 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.MaxDbl | 未確認 |  |
| Function MinDbl | 824 | 7 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.MinDbl | 未確認 |  |
| Function LeftPaddedString | 834 | 3 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.LeftPaddedString | 未確認 |  |
| Function RightPaddedString | 839 | 3 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.RightPaddedString | 未確認 |  |
| Function ReadIni | 845 | 19 | （見つからない） | 未確認 |  |
| Sub WriteIni | 867 | 28 | （見つからない） | 未確認 |  |
| Function ReplaceString | 899 | 34 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.ReplaceString | 未確認 |  |
| Function FileExists | 936 | 5 | `SRCCore/Filesystem/LocalFilesystem.cs` LocalFileSystem.FileExists<br>`SRCCore/Filesystem/PathResolver.cs` PathResolver.FileExists | 未確認 |  |
| Sub GetLine | 948 | 35 | `SRCCore/Lib/SrcDataReader.cs` SrcDataReader.GetLine<br>`SRCCore/Lib/SrcEveReader.cs` SrcEveReader.GetLine | 未確認 |  |
| Function GetWinVersion | 986 | 17 | （見つからない） | 未確認 |  |
| Function FormatNum | 1006 | 7 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.FormatNum | 未確認 |  |
| Function IsNumber | 1016 | 12 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.IsNumber | 未確認 |  |
| Function GetClassBundle | 1037 | 35 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.GetClassBundle | 未確認 |  |
| Function InStrNotNest | 1074 | 25 | `SRCCore/Lib/GeneralLib.cs` GeneralLib.InStrNotNest | 未確認 |  |
