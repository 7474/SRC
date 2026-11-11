# SpecialPowerDataList.cls

VB 版の `SRC/SRC_20121125/SpecialPowerDataList.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Terminate | 24 | 10 | （なし） | SRC#で不要 | Collection の解放は GC が受け持つ |
| Function Add | 36 | 7 | `SRCCore/Models/SpecialPowerDataList.cs` SpecialPowerDataList.Add | 一致 |  |
| Function Count | 45 | 3 | `SRCCore/Models/SpecialPowerDataList.cs` SpecialPowerDataList.Count | 一致 |  |
| Sub Delete | 50 | 3 | `SRCCore/Models/SpecialPowerDataList.cs` SpecialPowerDataList.Delete | 一致 | 番号での削除は呼ばれていない |
| Function Item | 55 | 9 | `SRCCore/Models/SpecialPowerDataList.cs` SpecialPowerDataList.Item | 一致 | 無いときは VB と同じく null。SrcCollection のキーは大文字・小文字などを区別しない |
| Function IsDefined | 66 | 11 | `SRCCore/Models/SpecialPowerDataList.cs` SpecialPowerDataList.IsDefined | 一致 |  |
| Sub Load | 79 | 172 | `SRCCore/Models/SpecialPowerDataList.cs` SpecialPowerDataList.Load | 修正済み | エラーのデータ名に、data_name ではなく行の内容を渡していたので、VB と同じ data_name にした（#1498）。CInt は Conversions.ToInteger（偶数丸め）で同じ |
