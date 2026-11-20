# BattleConfigDataList.cls

VB 版の `SRC/SRC_20121125/BattleConfigDataList.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Terminate | 25 | 10 | （なし） | SRC#で不要 | GC が受け持つ |
| Function Add | 37 | 7 | `SRCCore/Models/BattleConfigDataList.cs` BattleConfigDataList.Add | 一致 |  |
| Function Count | 46 | 3 | `SRCCore/Models/BattleConfigDataList.cs` BattleConfigDataList.Count | 一致 |  |
| Sub Delete | 51 | 3 | `SRCCore/Models/BattleConfigDataList.cs` BattleConfigDataList.Delete | 一致 |  |
| Function Item | 56 | 3 | `SRCCore/Models/BattleConfigDataList.cs` BattleConfigDataList.Item | 一致 | VB は無いとエラー 5。C# は null を返す（呼ぶ側は IsDefined で確かめている） |
| Function IsDefined | 61 | 11 | `SRCCore/Models/BattleConfigDataList.cs` BattleConfigDataList.IsDefined | 一致 |  |
| Sub Load | 74 | 62 | `SRCCore/Models/BattleConfigDataList.cs` BattleConfigDataList.Load | 一致 | 空行までの最後の行が式になる点、EOF での抜け方も同じ。ErrorHandler（開けないときの「fname が開けません。」、打ち切る誤りの DataErrorMessage と TerminateSRC）は SRC.LoadDataFile が受け持つ。ゲームとして実行するとき（SRC.ShowDataErrorDialogs）だけダイアログを出して終了し、続ける誤りも SRC.AddDataError でダイアログに出す（#1550） |
