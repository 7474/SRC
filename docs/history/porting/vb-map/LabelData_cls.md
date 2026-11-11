# LabelData.cls

VB 版の `SRC/SRC_20121125/LabelData.cls` の手続きと、C# の対応（[説明](README.md)）。「C#」の候補は名前だけで選んでいるので、突き合わせるときに確かめて直す。

| VB | 行 | 行数 | C# | 状態 | メモ |
|---|---|---|---|---|---|
| Function CountPara | 38 | 3 | `SRCCore/Events/LabelData.cs` LabelData.CountPara | 修正済み | 括弧・クォートの対応が取れていなければ -1 を返す（Data で直したため）（#1498） |
| Function Para | 43 | 9 | `SRCCore/Events/LabelData.cs` LabelData.Para | 修正済み | 範囲外なら空文字列を返す（null を返していた）。1 番目は GetValueAsString("") を評価する（#1498） |
| Property Get Data | 54 | 3 | `SRCCore/Events/LabelData.cs` LabelData.Data | 一致 |  |
| Property Let Data | 59 | 133 | `SRCCore/Events/LabelData.cs` LabelData.Data | 修正済み | 要素の数を VB の ListLength と同じ ListSplitStrict で数え、括弧・クォートの対応が取れていなければ、ラベルを登録している行（CurrentLineNum）で「ラベルの引数の括弧の対応が取れていません」を表示する。GeneralLib.ListLength が -1 を返さないので、このエラーが出ていなかった。行が範囲外なら EventDataLine.Empty で表示する（#1498） |
