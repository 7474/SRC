# Dialog.cls

VB 版の `SRC/SRC_20121125/Dialog.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 27 | 5 | （なし） | SRC#で不要 | 配列の初期化だけ |
| Sub Class_Terminate | 34 | 4 | （なし） | SRC#で不要 | 配列の解放だけ |
| Sub AddMessage | 40 | 7 | `SRCCore/Models/Dialog.cs` Dialog.AddMessage | 一致 |  |
| Property Get Count | 49 | 3 | `SRCCore/Models/Dialog.cs` Dialog.Count | 一致 |  |
| Function Name | 54 | 3 | `SRCCore/Models/Dialog.cs` DialogItem.strName | 一致 | Dialog.Items[idx-1].strName |
| Function Message | 59 | 3 | `SRCCore/Models/Dialog.cs` DialogItem.strMessage | 一致 | Dialog.Items[idx-1].strMessage |
| Property Get IsAvailable | 64 | 138 | `SRCCore/Models/Dialog.cs` Dialog.IsAvailable | 修正済み | 表情パターンの括弧を探す位置が VB の Mid$(pname, Len(pname) - j, 1) より 1 文字後ろにずれていて、「テスト(笑)」が「テス」になっていた。合体技のパートナーの名前の前方の一致を InStr（序数）にした（#1498）。括弧が見つからないときの VB の Mid$ の開始位置 0 の実行時エラーは合わせず、見つからなかったものとして扱う。u が null のとき true を返すのは SRC# の都合 |
