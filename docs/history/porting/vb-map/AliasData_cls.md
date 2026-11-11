# AliasData.cls

VB 版の `SRC/SRC_20121125/AliasData.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 33 | 9 | （なし） | SRC#で不要 | 配列を ReDim するだけ。C# は List で持つ |
| Property Get Count | 45 | 3 | `SRCCore/Models/AliasData.cs` AliasDataType.Count | 一致 | UBound と要素の数は同じ |
| Property Get AliasType | 50 | 3 | `SRCCore/Models/AliasData.cs` AliasDataElement.strAliasType | 一致 | 要素のフィールドとして持つ |
| Property Get AliasLevel | 55 | 3 | `SRCCore/Models/AliasData.cs` AliasDataElement.dblAliasLevel | 一致 | 同上 |
| Property Get AliasLevelIsPlusMod | 60 | 3 | `SRCCore/Models/AliasData.cs` AliasDataElement.blnAliasLevelIsPlusMod | 一致 | 同上 |
| Property Get AliasLevelIsMultMod | 65 | 3 | `SRCCore/Models/AliasData.cs` AliasDataElement.blnAliasLevelIsMultMod | 一致 | 同上 |
| Property Get AliasData | 70 | 3 | `SRCCore/Models/AliasData.cs` AliasDataElement.strAliasData | 一致 | 同上 |
| Property Get AliasNecessarySkill | 75 | 3 | `SRCCore/Models/AliasData.cs` AliasDataElement.strAliasNecessarySkill | 一致 | 指定が無いとき、VB は空文字列、C# は null。使う側は同じに扱うので動作は変わらない |
| Property Get AliasNecessaryCondition | 80 | 3 | `SRCCore/Models/AliasData.cs` AliasDataElement.strAliasNecessaryCondition | 一致 | 同上 |
| Sub AddAlias | 85 | 77 | `SRCCore/Models/AliasData.cs` AliasDataType.AddAlias | 修正済み | レベルが数値でないとき、VB は CDbl の実行時エラーで AliasDataList.Load が原因を書かないデータエラーにして打ち切る。C# は 0 にしていたので、例外を投げて Load で打ち切るようにした（#1498）。必要条件の位置を InStr(adef, " <") で探すのは VB と同じ |
