# Event.bas

VB 版の `SRC/SRC_20121125/Event.bas` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub InitEventData | 359 | 18 | `SRCCore/Events/Event.data.cs` Event.InitEventData | 未確認 |  |
| Sub LoadEventData | 379 | 883 | `SRCCore/Events/Event.data.cs` Event.LoadEventData | 未確認 |  |
| Sub LoadEventData2 | 1264 | 119 | `SRCCore/Events/Event.data.cs` Event.LoadEventData2 | 未確認 |  |
| Sub HandleEvent | 1386 | 339 | `SRCCore/Events/Event.handle.cs` Event.HandleEvent | 未確認 | #1503 で見つけた手がかり: Event.CurrentLabel の初期値 0 のため、HandleEvent を通らずに引数なしの ClearEvent を実行すると 0 行目のラベルを消しうる（初期値 -1 で済む） |
| Sub RegisterEvent | 1727 | 9 | `SRCCore/Events/Event.handle.cs` Event.RegisterEvent | 未確認 |  |
| Function SearchLabel | 1739 | 273 | `SRCCore/Events/Event.label.cs` Event.SearchLabel | 未確認 |  |
| Function IsEventDefined | 2016 | 25 | `SRCCore/Events/Event.label.cs` Event.IsEventDefined | 未確認 |  |
| Function IsLabelDefined | 2043 | 11 | `SRCCore/Events/Event.label.cs` Event.IsLabelDefined | 未確認 |  |
| Sub AddLabel | 2056 | 41 | `SRCCore/Events/Event.label.cs` Event.AddLabel | 未確認 |  |
| Sub AddSysLabel | 2099 | 43 | `SRCCore/Events/Event.label.cs` Event.AddSysLabel | 未確認 |  |
| Sub ClearLabel | 2144 | 25 | `SRCCore/Events/Event.label.cs` LabelIdIndex.ClearLabel | 未確認 |  |
| Sub RestoreLabel | 2171 | 12 | `SRCCore/Events/Event.label.cs` LabelIdIndex.RestoreLabel | 未確認 |  |
| Function FindLabel | 2185 | 26 | `SRCCore/Events/Event.label.cs` LabelIdIndex.FindLabel | 未確認 |  |
| Function FindEventLabel | 2213 | 11 | `SRCCore/Events/Event.label.cs` LabelIdIndex.FindEventLabel | 未確認 |  |
| Function FindNormalLabel | 2226 | 6 | `SRCCore/Events/Event.label.cs` LabelIdIndex.FindNormalLabel | 未確認 |  |
| Function FindNormalLabel0 | 2234 | 11 | `SRCCore/Events/Event.label.cs` LabelIdIndex.FindNormalLabel0 | 未確認 |  |
| Function FindSysNormalLabel | 2247 | 11 | `SRCCore/Events/Event.label.cs` LabelIdIndex.FindSysNormalLabel | 未確認 |  |
| Sub ClearEventData | 2262 | 52 | `SRCCore/Events/Event.data2.cs` Event.ClearEventData | 未確認 |  |
| Sub ClearAllEventData | 2316 | 14 | （見つからない） | 未確認 |  |
| Sub DumpEventData | 2333 | 20 | （見つからない） | 未確認 |  |
| Sub RestoreEventData | 2355 | 176 | （見つからない） | 未確認 |  |
| Sub SkipEventData | 2533 | 29 | （見つからない） | 未確認 |  |
| Sub SaveGlobalVariables | 2564 | 14 | （見つからない） | 未確認 |  |
| Sub LoadGlobalVariables | 2580 | 143 | （見つからない） | 未確認 |  |
| Sub SaveLocalVariables | 2725 | 17 | （見つからない） | 未確認 |  |
| Sub LoadLocalVariables | 2744 | 61 | （見つからない） | 未確認 |  |
| Sub DisplayEventErrorMessage | 2808 | 19 | `SRCCore/Events/Event.ui.cs` Event.DisplayEventErrorMessage | 未確認 |  |
| Sub MakeUnitList | 2829 | 283 | `SRCCore/Events/Event.unitlist.cs` Event.MakeUnitList | 未確認 |  |
| Sub SaveBasePoint | 3115 | 8 | `SRCCore/Events/Event.draw.cs` Event.SaveBasePoint | 未確認 |  |
| Sub RestoreBasePoint | 3125 | 8 | `SRCCore/Events/Event.draw.cs` Event.RestoreBasePoint | 未確認 |  |
| Sub ResetBasePoint | 3135 | 11 | `SRCCore/Events/Event.draw.cs` Event.ResetBasePoint | 未確認 |  |
