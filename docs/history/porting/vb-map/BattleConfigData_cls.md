# BattleConfigData.cls

VB 版の `SRC/SRC_20121125/BattleConfigData.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 27 | 3 | `SRCCore/Models/BattleConfigData.cs` BattleConfigData | 修正済み | ConfigCalc が null だったので、VB と同じく "" にした。式の行が無いデータで Calculate が例外にならない（#1498） |
| Function Calculate | 33 | 15 | `SRCCore/Models/BattleConfigData.cs` BattleConfigData.Calculate | 一致 | IsConfig を finally で戻すのは、例外のときの SRC# の安全策 |
