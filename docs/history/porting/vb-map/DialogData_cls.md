# DialogData.cls

VB 版の `SRC/SRC_20121125/DialogData.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Sub Class_Initialize | 30 | 5 | （なし） | SRC#で不要 | 配列の初期化だけ |
| Sub Class_Terminate | 37 | 10 | （なし） | SRC#で不要 | 配列の解放だけ |
| Function AddDialog | 49 | 10 | `SRCCore/Models/DialogData.cs` DialogData.AddDialog | 一致 | シチュエーションを Dialog.Situation に持つ |
| Function CountDialog | 61 | 3 | `SRCCore/Models/DialogData.cs` DialogData.CountDialog | 一致 |  |
| Function Situation | 66 | 3 | `SRCCore/Models/Dialog.cs` Dialog.Situation | 一致 | DialogData.Items[idx-1].Situation |
| Function Dialog | 71 | 3 | `SRCCore/Models/DialogData.cs` DialogData.Items | 一致 |  |
| Function SelectDialog | 76 | 390 | `SRCCore/Models/DialogData.cs` DialogData.SelectDialog・`SRCCore/Extensions/SituationExtension.cs` SituationExtension.SelectMessage | 修正済み | (1) 相手限定メッセージが状況に合ったとき、VB は Dice(list_num) の後に Dice(2) を必ず呼び（Or は短絡しない）、外れても汎用メッセージが無ければ選んだものを返す。C# は「(とどめ)」などや汎用メッセージが無いときに Dice(2) を呼ばず、乱数の進み方が違っていた (2) シチュエーションの先頭の一致を序数で比べる (3) 先頭が一致したものだけ IsAvailable を調べる (4) 相手の武器の属性の要素が null のときの例外を直した（#1498）。合わせないもの: VB の DialogData は弱点・有効・武器の属性を Mid で 1 文字ずつ取り出し、有効を「(対弱点=」で作る。MessageData とヘルプは GetClassBundle と「(対有効=」なので、DialogData を書き間違いとみなして MessageData と同じ処理にした。Option「相手限定メッセージ等確率」は SRC# の拡張（#555） |
